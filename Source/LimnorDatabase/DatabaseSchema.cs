/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data;

namespace LimnorDatabase
{
	public enum enumRecSource { Table, View, StoredProcedure }
	/// <summary>
	/// Summary description for DatabaseSchema.
	/// </summary>
	public class DatabaseSchema
	{
		public Connection dbCon = null;
		public DatabaseTable[] Tables = null;
		public TableRelations Relations = null;
		public DatabaseView[] Views = null;
		public DatabaseSchema()
		{
		}
		public int ViewCount
		{
			get
			{
				if (Views == null)
					return 0;
				return Views.Length;
			}
		}
		public DatabaseView GetView(int i)
		{
			if (Views != null)
				if (i >= 0 && i < Views.Length)
					return Views[i];
			return null;
		}
		public int TableCount
		{
			get
			{
				if (Tables == null)
					return 0;
				return Tables.Length;
			}
		}
		public DatabaseTable GetTable(int i)
		{
			if (Tables != null)
				if (i >= 0 && i < Tables.Length)
					return Tables[i];
			return null;
		}
		public DatabaseTable FindTable(string name)
		{
			if (Tables != null)
			{
				for (int i = 0; i < Tables.Length; i++)
				{
					if (string.Compare(Tables[i].TableName, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (Tables[i].FieldCount == 0)
						{
							Tables[i].GetFields(dbCon);
							Tables[i].GetIndexes(dbCon);
						}
						return Tables[i];
					}
				}
			}
			return null;
		}
		public DatabaseView FindView(string name)
		{
			if (Views != null)
			{
				for (int i = 0; i < Views.Length; i++)
				{
					if (string.Compare(Views[i].ViewName, name, StringComparison.OrdinalIgnoreCase) == 0)
						return Views[i];
				}
			}
			return null;
		}
		public DatabaseTable[] GetChildTables(int Index)
		{
			if (Relations != null)
			{
				DatabaseTable tbl = GetTable(Index);
				if (tbl != null)
				{
					DatabaseTable[] ret = null;
					int n, j;
					for (int i = 0; i < Relations.Count; i++)
					{
						if (Relations[i].MainTable == tbl.TableName)
						{
							if (ret == null)
							{
								n = 0;
								ret = new DatabaseTable[1];
							}
							else
							{
								n = ret.Length;
								DatabaseTable[] a = new DatabaseTable[n + 1];
								for (j = 0; j < n; j++)
									a[j] = ret[j];
								ret = a;
							}
							ret[n] = FindTable(Relations[i].SubTable);
						}
					}
					return ret;
				}
			}
			return null;
		}
		public void LoadSchema()
		{
			LoadTables();
			LoadRelations();
			LoadViews();
		}
		public void LoadViews()
		{
			Views = null;
			try
			{
				if (dbCon.TheConnection is OleDbConnection)
				{
				}
				else if (dbCon.TheConnection is SqlConnection)
				{
					dbWrapper db = new dbWrapper();
					db.CreateCommand(dbCon);
					db.SetCommandText("SELECT [name] FROM sysobjects (nolock) WHERE [type] = 'V'");
					db.SetCommandType(System.Data.CommandType.Text);
					db.CreateAdapter();
					System.Data.DataSet ds = new System.Data.DataSet("DS");
					db.Fill(ds, "Views");
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							Views = new DatabaseView[ds.Tables[0].Rows.Count];
							for (int i = 0; i < Views.Length; i++)
							{
								Views[i] = new DatabaseView();
								Views[i].ViewName = ds.Tables[0].Rows[i][0].ToString();
							}
						}
					}
				}
				else
				{
					DataTable vs = dbCon.GetViews();
					if (vs != null && vs.Rows.Count > 0)
					{
						Views = new DatabaseView[vs.Rows.Count];
						for (int i = 0; i < Views.Length; i++)
						{
							Views[i] = new DatabaseView();
							Views[i].ViewName = vs.Rows[i][0].ToString();
						}
					}
				}
			}
			catch
			{
			}
		}
		public FieldList LoadTableFields(string table)
		{
			if (Tables != null)
			{
				for (int i = 0; i < Tables.Length; i++)
				{
					if (string.Compare(Tables[i].TableName, table, StringComparison.OrdinalIgnoreCase) == 0)
					{
						Tables[i].GetFields(dbCon);
						Tables[i].GetIndexes(dbCon);
						return Tables[i].fields;
					}
				}
			}
			return null;
		}
		public FieldList LoadViewFields(string view)
		{
			if (Views != null)
			{
				for (int i = 0; i < Views.Length; i++)
				{
					if (string.Compare(Views[i].ViewName, view, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (!(dbCon.TheConnection is OdbcConnection))
						{
							Views[i].GetFields(dbCon);
						}
						return Views[i].fields;
					}
				}
			}
			return null;
		}
		public void LoadTables()
		{
			Tables = null;
			Relations = null;
			if (dbCon.TheConnection is OdbcConnection)
			{
				Tables = dbCon.ODBC_GetTables();
			}
			else
			{
				System.Data.DataTable schema = dbCon.GetTables();
				if (schema == null)
					return;
				if (schema.Rows.Count > 0)
				{
					System.Data.DataRow row = null;
					int n;
					string s;
					int nCount = 0;
					DatabaseTable[] Tables0 = new DatabaseTable[schema.Rows.Count];
					for (n = 0; n < schema.Rows.Count; n++)
					{
						row = schema.Rows[n];
						s = row["Table_type"].ToString();
						if (string.Compare(s, "TABLE", StringComparison.OrdinalIgnoreCase) == 0 ||
							string.Compare(s, "BASE TABLE", StringComparison.OrdinalIgnoreCase) == 0)
						{
							Tables0[nCount] = new DatabaseTable();
							Tables0[nCount].TableName = row["Table_Name"].ToString();
							nCount++;
						}
					}
					Tables = new DatabaseTable[nCount];
					for (n = 0; n < nCount; n++)
					{
						Tables[n] = Tables0[n];
					}
				}
			}
		}

		void addRelation(string refName, string mainTable, string subTable, string mainField, string subField)
		{
			DatabaseTable mTbl = FindTable(mainTable);
			DatabaseTable sbTbl = FindTable(subTable);
			if (mTbl != null && sbTbl != null)
			{
				EPField mFld = mTbl.FindField(mainField);
				EPField subFld = sbTbl.FindField(subField);
				if (Relations == null)
				{
					Relations = new TableRelations();
				}
				Relations.AddJoinFields(refName, mainTable, subTable, mFld, subFld);
			}
		}
		public void LoadRelations()
		{
			int i;
			Relations = new TableRelations();
			string refName, mainTable, subTable, mainField, subField;
			System.Data.DataTable schemaRelation = dbCon.GetRelations();
			if (schemaRelation == null)
				return;
			for (i = 0; i < schemaRelation.Rows.Count; i++)
			{
				mainTable = schemaRelation.Rows[i]["PK_TABLE_NAME"].ToString();
				subTable = schemaRelation.Rows[i]["FK_TABLE_NAME"].ToString();
				mainField = schemaRelation.Rows[i]["PK_COLUMN_NAME"].ToString();
				subField = schemaRelation.Rows[i]["FK_COLUMN_NAME"].ToString();
				refName = schemaRelation.Rows[i]["FK_NAME"].ToString();
				addRelation(refName, mainTable, subTable, mainField, subField);
			}
		}
		public bool RelationExists(string tbl1, string tbl2)
		{
			if (Relations != null)
				return Relations.RelationExists(tbl1, tbl2);
			return false;
		}
		public TableReference FindRelation(string mnTable, string subTable)
		{
			if (Relations != null)
				return Relations.FindRelation(mnTable, subTable);
			return null;
		}
	}
	//
	public class DatabaseTable : ICloneable
	{
		public string TableName = "";
		public FieldList fields = null;
		public TableIndex[] Indexes = null;
		public bool InUse = false;
		public DatabaseTable()
		{
		}
		public int FieldCount
		{
			get
			{
				if (fields == null)
					return 0;
				return fields.Count;
			}
		}
		public EPField GetField(int i)
		{
			if (fields == null)
				return null;
			return fields[i];
		}
		public bool HasPrimaryKey()
		{
			if (Indexes != null)
			{
				for (int i = 0; i < Indexes.Length; i++)
				{
					if (Indexes[i].IsPrimaryKey)
						return true;
				}
			}
			return false;
		}
		public bool HasUniqueKey()
		{
			if (Indexes != null)
			{
				for (int i = 0; i < Indexes.Length; i++)
				{
					if (Indexes[i].IsUnique)
						return true;
				}
			}
			return false;
		}
		public void GetFields(Connection cnt)
		{
			System.Data.DataTable schemaColumn;
			fields = new FieldList();
			Indexes = null;
			//
			//=====================================================
			dbWrapper db = new dbWrapper();
			db.CreateCommand((Connection)cnt.Clone());
			EPField fld;
			if (cnt.IsOleDb)
			{
				db.SetCommandText(TableName);
				db.SetCommandType(System.Data.CommandType.TableDirect);
			}
			else
			{
				db.SetCommandText("SELECT * FROM " + DatabaseEditUtil.SepBegin(cnt.NameDelimiterStyle) + TableName + DatabaseEditUtil.SepEnd(cnt.NameDelimiterStyle));
				db.SetCommandType(System.Data.CommandType.Text);
			}
			try
			{
				if (cnt.IsOleDb || cnt.IsMSSQL)
				{
					db.OpenReader(System.Data.CommandBehavior.KeyInfo);
				}
				else
				{
					db.OpenReader(System.Data.CommandBehavior.SchemaOnly);
				}
				schemaColumn = db.GetSchemaTable();
				db.CloseReader();
				db.Close();
				if (schemaColumn != null)
				{
					for (int i = 0; i < schemaColumn.Rows.Count; i++)
					{
						fld = EPField.MakeFieldFromColumnInfo(i, schemaColumn.Rows[i]);
						if (!string.IsNullOrEmpty(fld.Name))
						{
							fld.FromTableName = TableName;
							fields.AddField(fld);
						}
					}
				}

			}
			catch (Exception er)
			{
				FormLog.NotifyException(true, er);
			}
			finally
			{
				db.CloseReader();
				db.Close();
			}
		}
		public EPField FindField(string sCol)
		{
			if (fields != null)
			{
				for (int i = 0; i < fields.Count; i++)
				{
					if (fields[i].Name == sCol)
					{
						return fields[i];
					}
				}
			}
			return null;
		}
		public TableIndex AddIndex(string sIndexName)
		{
			if (Indexes == null)
			{
				Indexes = new TableIndex[1];
				Indexes[0] = new TableIndex();
				Indexes[0].IndexName = sIndexName;
				return Indexes[0];
			}
			int i;
			int n = Indexes.Length;
			for (i = 0; i < n; i++)
			{
				if (Indexes[i].IndexName == sIndexName)
					return Indexes[i];
			}
			TableIndex[] a = new TableIndex[n + 1];
			for (i = 0; i < n; i++)
				a[i] = Indexes[i];
			Indexes = a;
			Indexes[n] = new TableIndex();
			Indexes[n].IndexName = sIndexName;
			return Indexes[n];
		}
		public void GetIndexes(Connection cnt)
		{
			int i;
			Indexes = null;
			string sTable = this.ToString();
			if (string.IsNullOrEmpty(sTable))
				return;
			System.Data.DataTable schemaIndex = cnt.GetIndexes(sTable);
			if (schemaIndex == null)
				return;
			if (cnt.IsOdbc)
			{
				for (i = 0; i < schemaIndex.Rows.Count; i++)
				{
					TableIndex idx = AddIndex((string)schemaIndex.Rows[i]["Key_name"]);
					idx.IsPrimaryKey = string.CompareOrdinal(((string)schemaIndex.Rows[i]["Key_name"]), "PRIMARY") == 0;
					idx.IsUnique = ((Int64)schemaIndex.Rows[i]["Non_unique"] == 0);
					idx.fields.AddField(FindField((string)schemaIndex.Rows[i]["Column_name"]));
				}
				return;
			}
			//
			string sTblName, sIndexName, sCol;
			bool bPK, bUnique;
			EPField fld;
			for (i = 0; i < schemaIndex.Rows.Count; i++)
			{
				sTblName = schemaIndex.Rows[i]["TABLE_NAME"].ToString();
				if (string.Compare(sTblName, sTable, StringComparison.OrdinalIgnoreCase) == 0)
				{
					sIndexName = schemaIndex.Rows[i]["INDEX_NAME"].ToString();
					sCol = schemaIndex.Rows[i]["COLUMN_NAME"].ToString();
					bPK = Convert.ToBoolean(schemaIndex.Rows[i]["PRIMARY_KEY"]);
					bUnique = Convert.ToBoolean(schemaIndex.Rows[i]["UNIQUE"]);
					fld = FindField(sCol);
					TableIndex idx = AddIndex(sIndexName);
					idx.IsPrimaryKey = bPK;
					idx.IsUnique = bUnique;
					idx.fields.AddField(fld);
				}
			}
		}
		public override string ToString()
		{
			if (TableName == null)
				return "";
			return TableName;
		}
		#region ICloneable Members

		public object Clone()
		{
			DatabaseTable obj = new DatabaseTable();
			obj.fields = (FieldList)fields.Clone();
			obj.TableName = TableName;
			obj.InUse = InUse;
			return obj;
		}

		#endregion
	}
	public class TableIndex
	{
		public string IndexName = "";
		public bool IsPrimaryKey = false;
		public bool IsUnique = false;
		public FieldList fields = new FieldList();
		public TableIndex()
		{
		}
		public override string ToString()
		{
			return IndexName;
		}
	}
	/// <summary>
	/// All relations in a database
	/// </summary>
	public class TableRelations
	{
		public TableReference[] References = null;
		public TableRelations()
		{
		}
		public int Count
		{
			get
			{
				if (References == null)
					return 0;
				return References.Length;
			}
		}
		public TableReference this[int Index]
		{
			get
			{
				if (References != null)
				{
					if (Index >= 0 && Index < References.Length)
						return References[Index];
				}
				return null;
			}
		}
		public bool RelationExists(string tbl1, string tbl2)
		{
			if (References != null)
			{
				for (int i = 0; i < References.Length; i++)
				{
					if (References[i].RelationExists(tbl1, tbl2))
						return true;
				}
			}
			return false;
		}
		public TableReference FindRelation(string mnTable, string subTable)
		{
			if (References != null)
			{
				for (int i = 0; i < References.Length; i++)
				{
					if (References[i].MainTable == mnTable && References[i].SubTable == subTable)
						return References[i];
				}
			}
			return null;
		}
		public void AddJoinFields(string refName, string mainTable, string subTable, EPField mainField, EPField subField)
		{
			int n = -1;
			if (References == null)
			{
				References = new TableReference[1];
				References[0] = new TableReference();
				References[0].RefName = refName;
				References[0].MainTable = mainTable;
				References[0].SubTable = subTable;
				n = 0;
			}
			else
			{
				int i;
				for (i = 0; i < References.Length; i++)
				{
					if (References[i].MainTable == mainTable && References[i].SubTable == subTable)
					{
						n = i;
						break;
					}
				}
				if (n < 0)
				{
					n = References.Length;
					TableReference[] a = new TableReference[n + 1];
					for (i = 0; i < n; i++)
						a[i] = References[i];
					References = a;
					References[n] = new TableReference();
					References[n].RefName = refName;
					References[n].MainTable = mainTable;
					References[n].SubTable = subTable;
				}
			}
			References[n].AddJoinFields(mainField, subField);
		}
	}
	public class TableReference
	{
		public string RefName;
		public string MainTable;
		public string SubTable;
		public EPJoin[] JoinFields = null;
		public TableReference()
		{
		}
		public bool RelationExists(string tbl1, string tbl2)
		{
			if (MainTable == tbl1 && SubTable == tbl2)
				return true;
			if (MainTable == tbl2 && SubTable == tbl1)
				return true;
			return false;
		}
		public void AddJoinFields(EPField mnField, EPField sbField)
		{
			int n;
			if (JoinFields == null)
			{
				JoinFields = new EPJoin[1];
				n = 0;
			}
			else
			{
				n = JoinFields.Length;
				EPJoin[] a = new EPJoin[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = JoinFields[i];
				JoinFields = a;
			}
			JoinFields[n] = new EPJoin();
			JoinFields[n].field1 = mnField;
			JoinFields[n].field2 = sbField;
		}
	}
	public class DatabaseView : ICloneable
	{
		public string ViewName = "";
		public FieldList fields = null;
		public bool InUse = false;
		public DatabaseView()
		{
		}
		public int FieldCount
		{
			get
			{
				if (fields == null)
					return 0;
				return fields.Count;
			}
		}
		public EPField GetField(int i)
		{
			if (fields == null)
				return null;
			return fields[i];
		}
		public void GetFields(Connection cnt)
		{
			int i;
			System.Data.DataTable schemaColumn = null;
			fields = new FieldList();
			if (cnt.IsOdbc)
				return;
			//=====================================================
			EPField fld;
			dbWrapper db = new dbWrapper();
			db.CreateCommand(cnt);
			string sep1 = DatabaseEditUtil.SepBegin(cnt.NameDelimiterStyle);
			string sep2 = DatabaseEditUtil.SepEnd(cnt.NameDelimiterStyle);
			db.SetCommandText("SELECT * FROM " + sep1 + ViewName + sep2);
			db.SetCommandType(System.Data.CommandType.Text);
			try
			{
				db.OpenReader(System.Data.CommandBehavior.SchemaOnly);
				schemaColumn = db.GetSchemaTable();
				db.CloseReader();
				db.Close();
				if (schemaColumn != null)
				{
					for (i = 0; i < schemaColumn.Rows.Count; i++)
					{
						fld = EPField.MakeFieldFromColumnInfo(i, schemaColumn.Rows[i]);
						fld.FromTableName = ViewName;
						fields.AddField(fld);
					}
				}
			}
			catch
			{
			}
			finally
			{
				db.CloseReader();
				db.Close();
			}
		}
		public EPField FindField(string sCol)
		{
			if (fields != null)
			{
				for (int i = 0; i < fields.Count; i++)
				{
					if (fields[i].Name == sCol)
					{
						return fields[i];
					}
				}
			}
			return null;
		}

		public override string ToString()
		{
			if (ViewName == null)
				return "";
			return ViewName;
		}
		#region ICloneable Members

		public object Clone()
		{
			DatabaseView obj = new DatabaseView();
			obj.fields = (FieldList)fields.Clone();
			obj.ViewName = ViewName;
			obj.InUse = InUse;
			return obj;
		}

		#endregion
	}
}
