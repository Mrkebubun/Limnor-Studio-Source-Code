/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
	public class TableAlias : ICloneable
	{
		public string TableName = "";
		public string Alias = "";
		protected DatabaseTable tableDef = null;
		protected DatabaseView viewDef = null;
		public enumRecSource srcType = enumRecSource.Table;
		public TableAlias()
		{
		}
		public TableAlias(string t, string a)
		{
			TableName = t;
			Alias = a;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Alias))
				return TableName;
			return TableName + " AS " + Alias;
		}
		public EPField FindField(string sName)
		{
			if (srcType == enumRecSource.Table)
			{
				if (tableDef != null)
					return tableDef.FindField(sName);
			}
			else if (srcType == enumRecSource.View)
			{
				if (viewDef != null)
					return viewDef.FindField(sName);
			}
			return null;
		}
		public void LoadSchema(DatabaseSchema schema)
		{
			if (srcType == enumRecSource.Table)
			{
				if (tableDef == null)
					tableDef = schema.FindTable(TableName);
			}
			else if (srcType == enumRecSource.View)
			{
				if (viewDef == null)
					viewDef = schema.FindView(TableName);
			}
		}
		public int FieldCount
		{
			get
			{
				if (srcType == enumRecSource.Table)
				{
					if (tableDef != null)
						return tableDef.FieldCount;
				}
				else if (srcType == enumRecSource.View)
				{
					if (viewDef != null)
						return viewDef.FieldCount;
				}
				return 0;
			}
		}
		public EPField GetField(int Index)
		{
			if (srcType == enumRecSource.Table)
			{
				if (tableDef != null)
					return tableDef.GetField(Index);
			}
			else if (srcType == enumRecSource.View)
			{
				if (viewDef != null)
					return viewDef.GetField(Index);
			}
			return null;
		}
		/// <summary>
		/// find unique index in the fields, mark indexed attribute, set query.SingleTableName
		/// </summary>
		/// <param name="fields">fields to set indexed attributes</param>
		/// <returns>true: index found</returns>
		public bool MarkUniqueIndexFields(FieldList fields)
		{
			bool bFound = false;
			if (srcType == enumRecSource.Table)
			{
				if (tableDef != null)
				{
					if (tableDef.Indexes != null)
					{
						int i;
						for (i = 0; i < tableDef.Indexes.Length; i++)
						{
							if (tableDef.Indexes[i].IsUnique)
							{
								bFound = true;
								List<EPField> idxFields = new List<EPField>();
								for (int k = 0; k < tableDef.Indexes[i].fields.Count; k++)
								{
									EPField fld = fields.FindField(tableDef.TableName, tableDef.Indexes[i].fields[k].Name);
									if (fld == null)
									{
										bFound = false;
										break;
									}
									else
									{
										idxFields.Add(fld);
									}
								}
								if (bFound)
								{
									for (int k = 0; k < fields.Count; k++)
									{
										fields[k].Indexed = false;
									}
									for (int k = 0; k < idxFields.Count; k++)
									{
										idxFields[k].Indexed = true;
									}
									break;
								}
							}
						}
					}
				}
			}
			return bFound;
		}
		#region ICloneable Members

		public object Clone()
		{
			TableAlias obj = new TableAlias();
			obj.TableName = this.TableName;
			obj.Alias = this.Alias;
			obj.srcType = srcType;
			return obj;
		}

		#endregion

	}
	public class TableAliasList : ICloneable
	{
		protected TableAlias[] tables = null;
		public TableAliasList()
		{
		}
		public int Count
		{
			get
			{
				if (tables == null)
					return 0;
				return tables.Length;
			}
		}
		public TableAlias this[int Index]
		{
			get
			{
				if (tables != null)
				{
					if (Index >= 0 && Index < tables.Length)
						return tables[Index];
				}
				return null;
			}
			set
			{
				if (tables != null)
				{
					if (Index >= 0 && Index < tables.Length)
						tables[Index] = value;
				}
			}
		}
		public TableAlias this[string tableName]
		{
			get
			{
				if (tables != null)
				{
					for (int i = 0; i < tables.Length; i++)
					{
						if (!string.IsNullOrEmpty(tables[i].Alias))
						{
							if (string.Compare(tableName, tables[i].Alias, StringComparison.OrdinalIgnoreCase) == 0)
								return tables[i];
						}
						else if (string.Compare(tables[i].TableName, tableName, StringComparison.OrdinalIgnoreCase) == 0)
							return tables[i];
					}
				}
				return null;
			}
		}
		public bool IsTableIncluded(string s)
		{
			int nCount = this.Count;
			for (int i = 0; i < nCount; i++)
			{
				if (!string.IsNullOrEmpty(tables[i].Alias))
				{
					if (string.Compare(s, tables[i].Alias, StringComparison.OrdinalIgnoreCase) == 0)
						return true;
				}
				else if (string.Compare(s, tables[i].TableName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public bool IsTableIncluded(TableAlias tbl)
		{
			if (!string.IsNullOrEmpty(tbl.Alias))
				return IsTableIncluded(tbl.Alias);
			else
				return IsTableIncluded(tbl.TableName);
		}
		public void AddTable(TableAlias tbl)
		{
			if (IsTableIncluded(tbl))
				return;
			int n;
			if (tables == null)
			{
				n = 0;
				tables = new TableAlias[1];
			}
			else
			{
				n = tables.Length;
				TableAlias[] a = new TableAlias[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = tables[i];
				tables = a;
			}
			tables[n] = tbl;
		}

		#region ICloneable Members

		public object Clone()
		{
			TableAliasList obj = new TableAliasList();
			int n = this.Count;
			for (int i = 0; i < n; i++)
				obj.AddTable((TableAlias)tables[i].Clone());
			return obj;
		}

		#endregion
	}


}
