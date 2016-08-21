/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Data;
using MathExp;
using System.Windows.Forms;

namespace LimnorDatabase
{


	/// <summary>
	/// Summary description for QueryParser.
	/// </summary>
	public class QueryParser
	{
		#region SQL Language string constants
		public static bool _UseLowerCase = false;
		public const string _SQL_Distinct = "DISTINCT ";
		public const string _SQL_Select = "SELECT ";
		public const string _SQL_Top_MS = "TOP ";
		public const string _SQL_Top_MySql = " LIMIT ";
		public const string _SQL_Percent = "PERCENT ";
		public const string _SQL_WithTies = "WITH TIES ";

		public const string _SQL_UNION = " UNION ";
		//
		public const string _SQL_From = " FROM ";
		public const string _SQL_Where = " WHERE ";
		public const string _SQL_GroupBy = " GROUP BY ";
		public const string _SQL_Having = " HAVING ";
		public const string _SQL_OrderBy = " ORDER BY ";

		public const string _SQL_Update = "UPDATE ";
		public const string _SQL_Set = " SET ";
		public const string _SQL_And = " AND ";

		public const string _SQL_Insert = "INSERT ";
		public const string _SQL_Into = " INTO ";
		public const string _SQL_Values = " VALUES ";

		public const string _SQL_Delete = "DELETE ";

		public static string SQL_Distinct() { return _UseLowerCase ? _SQL_Distinct.ToLowerInvariant() : _SQL_Distinct; }
		public static string SQL_Select() { return _UseLowerCase ? _SQL_Select.ToLowerInvariant() : _SQL_Select; }
		public static string SQL_Top_MS() { return _UseLowerCase ? _SQL_Top_MS.ToLowerInvariant() : _SQL_Top_MS; }
		public static string SQL_Top_MySql() { return _UseLowerCase ? _SQL_Top_MySql.ToLowerInvariant() : _SQL_Top_MySql; }
		public static string SQL_Percent() { return _UseLowerCase ? _SQL_Percent.ToLowerInvariant() : _SQL_Percent; }
		public static string SQL_WithTies() { return _UseLowerCase ? _SQL_WithTies.ToLowerInvariant() : _SQL_WithTies; }
		public static string SQL_UNION() { return _UseLowerCase ? _SQL_UNION.ToLowerInvariant() : _SQL_UNION; }
		//
		public static string SQL_From() { return _UseLowerCase ? _SQL_From.ToLowerInvariant() : _SQL_From; }
		public static string SQL_Where() { return _UseLowerCase ? _SQL_Where.ToLowerInvariant() : _SQL_Where; }
		public static string SQL_GroupBy() { return _UseLowerCase ? _SQL_GroupBy.ToLowerInvariant() : _SQL_GroupBy; }
		public static string SQL_Having() { return _UseLowerCase ? _SQL_Having.ToLowerInvariant() : _SQL_Having; }
		public static string SQL_OrderBy() { return _UseLowerCase ? _SQL_OrderBy.ToLowerInvariant() : _SQL_OrderBy; }
		//
		public static string SQL_Update() { return _UseLowerCase ? _SQL_Update.ToLowerInvariant() : _SQL_Update; }
		public static string SQL_Set() { return _UseLowerCase ? _SQL_Set.ToLowerInvariant() : _SQL_Set; }
		public static string SQL_And() { return _UseLowerCase ? _SQL_And.ToLowerInvariant() : _SQL_And; }
		//
		public static string SQL_Insert() { return _UseLowerCase ? _SQL_Insert.ToLowerInvariant() : _SQL_Insert; }
		public static string SQL_Into() { return _UseLowerCase ? _SQL_Into.ToLowerInvariant() : _SQL_Into; }
		public static string SQL_Values() { return _UseLowerCase ? _SQL_Values.ToLowerInvariant() : _SQL_Values; }
		//
		public static string SQL_Delete() { return _UseLowerCase ? _SQL_Delete.ToLowerInvariant() : _SQL_Delete; }
		#endregion

		protected DatabaseSchema schema = null;
		protected bool bSchemaLoaded = false;
		public string[] tablenames = null; //all table names in db in up case
		//intermediate parsing results
		public FieldsParser fieldParser = new FieldsParser();
		public FieldList fields = null;
		public ParameterList parameters = null;
		public TableAliasList T = null;
		//
		public EasyQuery query = null; //save completed result
		public string ParseError = "";
		public string Sep1 = "[", Sep2 = "]";
		private EnumTopRecStyle _topRecStyle;
		private int _topNsample;
		public QueryParser()
		{
			//
			//
		}
		public void SetSampleTopN(int topn)
		{
			_topNsample = topn;
		}
		public void ResetConnection()
		{
			if (schema != null)
			{
				if (schema.dbCon != null)
				{
					schema.dbCon.Close();
				}
			}
			schema = new DatabaseSchema();
			schema.dbCon = query.DatabaseConnection.ConnectionObject;
			LoadSchema();
		}
		public string GetConnectionString()
		{
			if (schema != null)
			{
				if (schema.dbCon != null)
				{
					return schema.dbCon.ConnectionString;
				}
			}
			return "";
		}
		public void LoadData(EasyQuery qry)
		{
			query = (EasyQuery)qry.Clone();
			ResetConnection();
			Sep1 = query.DatabaseConnection.ConnectionObject.NameDelimiterBegin;
			Sep2 = query.DatabaseConnection.ConnectionObject.NameDelimiterEnd;
			_topRecStyle = qry.DatabaseConnection.TopRecordStyle;
		}
		public EnumTopRecStyle TopRecStyle
		{
			get
			{
				return _topRecStyle;
			}
		}
		public bool BuildQuery(EasyQuery qry, System.Windows.Forms.Form frmCaller)
		{
			LoadData(qry);
			dlgQueryBuilder dlg = new dlgQueryBuilder();
			if (dlg.LoadData(this))
			{
				if (dlg.ShowDialog(frmCaller) == System.Windows.Forms.DialogResult.OK)
				{
					checkQueryreadOnly(query);
					return true;
				}
			}
			return false;
		}
		public bool BuildFilter(EasyQuery qry, System.Windows.Forms.Form frmCaller)
		{
			LoadData(qry);
			dlgQueryBuilder dlg = new dlgQueryBuilder();
			if (dlg.LoadData(this))
			{
				dlg.Show();
				dlg.Hide();
				if (dlg.BuildFilter())
				{
					dlg.Close();
					return true;
				}
			}
			else
			{
				dlg.Close();
			}
			return false;
		}
		public void LoadSchema()
		{
			if (!bSchemaLoaded)
			{
				schema.LoadSchema();
				int nCount = schema.TableCount;
				tablenames = new string[nCount];
				for (int i = 0; i < nCount; i++)
				{
					if (schema.Tables[i] == null)
					{
						tablenames[i] = "";
					}
					else
					{
						tablenames[i] = schema.Tables[i].TableName;
					}
				}
				bSchemaLoaded = true;
			}
		}
		public DatabaseSchema GetSchema()
		{
			return schema;
		}
		public bool ParseFields(string sSQL, ref string sInvalid)
		{
			sSQL = sSQL.Replace('\r', ' ');
			sSQL = sSQL.Replace('\n', ' ');
			sSQL = sSQL.Replace('\t', ' ');
			if (fieldParser.ParseQuery(sSQL, Sep1, Sep2, ref sInvalid))
			{
				if (query.IsOleDb)
				{
					if (fieldParser.Fields != null)
					{
						EPField fld;
						bool bFound;
						string sc;
						int i, z, k;
						//find out table name
						if (query.T != null)
						{
							for (i = 0; i < fieldParser.Fields.Length; i++)
							{
								if (fieldParser.Fields[i].Table.Length == 0 && fieldParser.Fields[i].FieldName.Length == 0)
								{
									//if fieldParser.Fields[i].Field belong to T, assume it belongs to T
									for (k = 0; k < query.T.Count; k++)
									{
										fld = query.T[k].FindField(fieldParser.Fields[i].Field);
										if (fld != null)
										{
											fieldParser.Fields[i].FieldName = fld.Name;
											fieldParser.Fields[i].Table = query.T[k].TableName;
											fieldParser.Fields[i].FieldText = fld.Name;
											fieldParser.Fields[i].Field = fieldParser.Fields[i].FieldText + " AS " + Sep1 + fieldParser.Fields[i].FieldName + Sep2;
											break;
										}
									}
								}
							}
						}
						//make field names unique
						for (i = 0; i < fieldParser.Fields.Length; i++)
						{
							if (fieldParser.Fields[i].Table.Length == 0 && fieldParser.Fields[i].FieldName.Length == 0)
							{
								//assume it is an expression
								sc = "FIELD" + i.ToString();
								bFound = true;
								z = 0;
								while (bFound)
								{
									bFound = false;
									for (k = 0; k < fieldParser.Fields.Length; k++)
									{
										if (k != i)
										{
											if (string.Compare(sc, fieldParser.Fields[k].FieldName, StringComparison.OrdinalIgnoreCase) == 0)
											{
												bFound = true;
												break;
											}
										}
									}
									if (!bFound)
									{
										z++;
										sc = "FIELD" + i.ToString() + "_";
										sc += z.ToString();
									}
								}
								fieldParser.Fields[i].FieldName += sc;
								fieldParser.Fields[i].Field = fieldParser.Fields[i].FieldText + " AS " + Sep1 + fieldParser.Fields[i].FieldName + Sep2;
							}
							if (fieldParser.Fields[i].Table.Length > 0 && fieldParser.Fields[i].FieldName.Length > 0)
							{
								sc = fieldParser.Fields[i].FieldName;
								bFound = true;
								z = 0;
								while (bFound)
								{
									bFound = false;
									for (k = 0; k < fieldParser.Fields.Length; k++)
									{
										if (k != i)
										{
											if (string.Compare(sc, fieldParser.Fields[k].FieldName, StringComparison.OrdinalIgnoreCase) == 0)
											{
												bFound = true;
												break;
											}
										}
									}
									if (bFound)
									{
										z++;
										sc = fieldParser.Fields[i].FieldName;
										sc += z.ToString();
									}
								}
								if (z > 0)
								{
									fieldParser.Fields[i].FieldName += z.ToString();
									fieldParser.Fields[i].Field = fieldParser.Fields[i].FieldText + " AS " + Sep1 + fieldParser.Fields[i].FieldName + Sep2;
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}
		public string GetFieldListText()
		{
			return fieldParser.GetFieldListText();
		}
		/// <summary>
		/// parse sSQL into member variables fields and parameters.
		/// return true if the query is read only. this return value is not used since the read-only flag is determined by search unique index and identity.
		/// </summary>
		/// <param name="sSQL"></param>
		/// <returns></returns>
		private bool getQueryFields(Form caller, string sSQL, bool isCommand)
		{
			if (schema == null)
			{
				throw new ExceptionLimnorDatabase("QueryParser.GetQueryFields:schema is null");
			}
			if (schema.dbCon == null)
			{
				throw new ExceptionLimnorDatabase("QueryParser.GetQueryFields:schema.dbCon is null");
			}
			if (string.IsNullOrEmpty(schema.dbCon.ConnectionString))
			{
				throw new ExceptionLimnorDatabase("QueryParser.GetQueryFields:schema.dCon.ConnectionString is null");
			}
			bool bReadOnly = false;
			dbWrapper db = new dbWrapper();
			try
			{
				if (schema.dbCon.State != ConnectionState.Closed)
				{
					schema.dbCon.Close();
				}
				ParameterList psOld = new ParameterList();
				if (parameters != null && parameters.Count > 0)
				{
					psOld = (ParameterList)parameters.Clone();
				}
				EnumParameterStyle pstyle = schema.dbCon.ParameterStyle;
				fields = new FieldList();

				db.CreateCommand((Connection)schema.dbCon.Clone());
				int sepLeng = 0;
				if (!string.IsNullOrEmpty(Sep1) && !string.IsNullOrEmpty(Sep2))
				{
					sepLeng = Sep1.Length + Sep2.Length;
				}
				//parameters contains unique names
				ParameterList ps = SQLParser.ParseParameters(sSQL, Sep1, Sep2);
				if (ps.Count > 0 && parameters != null && parameters.Count > 0)
				{
					foreach (EPField p in ps)
					{
						EPField p0 = parameters[p.Name];
						if (p0 != null)
						{
							p.OleDbType = p0.OleDbType;
							p.SetValue(p0.Value);
						}
						else
						{
							psOld.Add(p0.Clone() as EPField);
						}
					}
				}
				parameters = ps;
				EPField fld;
				if (parameters.Count > 0)
				{
					//let the user specifies values and thus taking the responsibility of giving correct data types
					DlgParameters dlg = new DlgParameters();
					dlg.LoadData(parameters);
					dlg.NoCancel();
					dlg.ShowDialog();
					FieldList pmap = new FieldList();
					string sSQL0;
					sSQL0 = SQLParser.MapParameters(sSQL, pstyle, Sep1, Sep2, parameters, pmap, psOld);
					db.SetCommandText(sSQL0);
					//
					if (pstyle == EnumParameterStyle.QuestionMark)
					{
						for (int i = 0; i < pmap.Count; i++)
						{
							fld = (EPField)pmap[i].Clone();
							fld.Name = "p" + i.ToString();
							fld.SetValue(pmap[i].Value);
							db.AddCommandParameter(fld, pstyle);
						}
					}
					else
					{
						for (int i = 0; i < parameters.Count; i++)
						{
							fld = (EPField)parameters[i].Clone();
							fld.Name = ParameterList.GetParameterName(pstyle, parameters[i].Name);
							fld.SetValue(parameters[i].Value);
							db.AddCommandParameter(fld, pstyle);
						}
					}
				}
				else
					db.SetCommandText(sSQL);
				db.OpenReader(System.Data.CommandBehavior.KeyInfo);
				DataTable schemaColumn = db.GetSchemaTable();
				db.CloseReader();
				db.Close();
				if (schemaColumn != null)
				{
					for (int i = 0; i < schemaColumn.Rows.Count; i++)
					{
						fld = EPField.MakeFieldFromColumnInfo(i, schemaColumn.Rows[i]);
						if (fld.Name.Length == 0)
						{
							int n = 0;
							fld.Name = "EXP0";
							while (fields[fld.Name] != null)
							{
								n++;
								fld.Name = string.Format(CultureInfo.InvariantCulture, "EXP{0}", n);
							}
						}
						else if (fields.FindFieldIndex(fld.Name) >= 0)
						{
							fld.FieldCaption = fld.Name;
							string bs = fld.Name;
							int n = 0;
							while (fields[fld.Name] != null)
							{
								n++;
								fld.Name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", bs, n);
							}
						}
						fields.AddField(fld);
					}
				}
				if (!isCommand)
				{
					//parse field text
					//name1, name2,...,*,name<n1>,name<n2>,...,*,name<m1>,name<m2>,...
					//how to identify the end of *?
					//Use the BaseTableName to determine the field count and thus we know the 
					//field count of *
					string[] saFields = parseFieldNames(sSQL);
					if (saFields != null)
					{
						int i, j;
						int fldIndex = 0;
						bool bAll;
						string s1, s2;
						string sTable = "";
						if (fields.Count > 0)
						{
							for (i = 0; i < saFields.Length; i++)
							{
								if (!bReadOnly)
								{
									if (fields[fldIndex].FromTableName == null)
										bReadOnly = true;
									else
									{
										if (sTable.Length == 0)
										{
											sTable = fields[fldIndex].FromTableName;
										}
										else
										{
											if (string.Compare(sTable, fields[fldIndex].FromTableName, StringComparison.OrdinalIgnoreCase) != 0)
												bReadOnly = true;
										}
									}
								}
								bAll = false;
								if (string.Compare(saFields[i], "*", StringComparison.OrdinalIgnoreCase) == 0)
									bAll = true;
								else
								{
									if (saFields[i].Length > 2)
									{
										if (string.Compare(saFields[i].Substring(saFields[i].Length - 2), ".*", StringComparison.Ordinal) == 0)
											bAll = true;
									}
								}
								if (bAll)
								{
									TableAlias tbl = T[fields[fldIndex].FromTableName];
									if (tbl == null)
									{
										tbl = new TableAlias(fields[fldIndex].FromTableName, "");
										tbl.LoadSchema(schema);
										T.AddTable(tbl);
									}
									for (j = 0; j < tbl.FieldCount; j++)
									{
										fields[fldIndex + j].FieldText = Sep1 + fields[fldIndex].FromTableName + Sep2 + "." + Sep1 + fields[fldIndex + j].Name + Sep2;
									}
									fldIndex += tbl.FieldCount;
								}
								else
								{
									if (fields[fldIndex].FieldText.Length == 0)
									{
										j = FieldsParser.FindStringIndex(saFields[i], " AS ", 0, Sep1, Sep2);
										if (j > 0)
										{
											saFields[i] = saFields[i];
											fields[fldIndex].FieldText = saFields[i];
											if (!bReadOnly)
											{
												//
												s1 = FieldsParser.GetFieldNameFromFieldText(saFields[i], Sep1, Sep2);
												s2 = saFields[i].Substring(j + 4).Trim();
												if (sepLeng > 0 && s2.Length > sepLeng)
												{
													if (s2.StartsWith(Sep1, StringComparison.OrdinalIgnoreCase) && s2.EndsWith(Sep2, StringComparison.OrdinalIgnoreCase))
														s2 = s2.Substring(Sep1.Length, s2.Length - sepLeng);
												}
												if (string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) != 0)
												{
													bReadOnly = true;
												}
											}
										}
										else
										{
											//not using AS
											//compare 1.[table].[field]; 2.[field]; 3.field to fieldname
											//if same, do not use AS, else use AS and set readonly
											s1 = FieldsParser.GetFieldNameFromFieldText(saFields[i], Sep1, Sep2);
											s2 = fields[fldIndex].Name;
											if (string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0)
											{
												fields[fldIndex].FieldText = saFields[i];
											}
											else
											{
												fields[fldIndex].FieldText = saFields[i] + " AS " + Sep1 + fields[fldIndex].Name + Sep2;
												bReadOnly = true;
											}
										}
									}
									fldIndex++;
								}
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				MathNode.Log(caller, err);
				throw;
			}
			finally
			{
				db.Close();
			}
			return bReadOnly;
		}
		string[] parseFieldNames(string sSQL)
		{
			string s;
			int ix = FieldsParser.FindStringIndex(sSQL, SQL_Select(), 0, Sep1, Sep2);
			if (ix >= 0)
			{
				sSQL = sSQL.Substring(ix + SQL_Select().Length).Trim();
			}
			int i;
			//parse ALL | DISTINCT
			if (sSQL.Length > 4)
			{
				if (sSQL.StartsWith("ALL ", StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(4);
					sSQL = sSQL.Trim();
				}
			}
			if (sSQL.Length > 9)
			{
				if (sSQL.StartsWith(SQL_Distinct(), StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(9);
					sSQL = sSQL.Trim();
				}
			}
			EnumTopRecStyle topStyle = schema.dbCon.TopRecStyle;
			if (sSQL.Length > 4)
			{
				//parse TOP n [ PERCENT ]
				if (topStyle == EnumTopRecStyle.TopN)
				{
					if (sSQL.StartsWith(QueryParser.SQL_Top_MS(), StringComparison.OrdinalIgnoreCase))
					{
						sSQL = sSQL.Substring(4);
						sSQL = sSQL.Trim();
						i = sSQL.IndexOf(' ');
						if (i < 0)
						{
							return null;
						}
						try
						{
							sSQL = sSQL.Substring(i);
							sSQL = sSQL.Trim();
							//parse PERCENT
							if (sSQL.Length > 8)
							{
								if (sSQL.StartsWith(QueryParser.SQL_Percent(), StringComparison.OrdinalIgnoreCase))
								{
									sSQL = sSQL.Substring(8);
									sSQL = sSQL.Trim();
								}
							}
						}
						catch
						{
							return null;
						}
					}
				}
			}
			if (topStyle == EnumTopRecStyle.TopN)
			{
				if (sSQL.Length > 10)
				{
					//parse WITH TIES
					if (sSQL.StartsWith(QueryParser.SQL_WithTies(), StringComparison.OrdinalIgnoreCase))
					{
						sSQL = sSQL.Substring(10);
						sSQL = sSQL.Trim();
					}
				}
			}
			//parse fields
			i = FieldsParser.FindStringIndex(sSQL, QueryParser.SQL_From(), 0, Sep1, Sep2);
			if (i > 0)
			{
				sSQL = sSQL.Substring(0, i);
				sSQL = sSQL.Trim();
			}
			LinkedList lk = new LinkedList();
			while (sSQL.Length > 0)
			{
				s = FieldsParser.PopValue(ref sSQL, ",", Sep1, Sep2);
				lk.Add(s.Trim());
			}
			int n = lk.Count;
			if (n > 0)
			{
				string[] ss = new string[n];
				LinkedListItem item = lk.First;
				for (i = 0; i < n; i++)
				{
					ss[i] = item.Value.ToString();
					item = item.Next;
				}
				return ss;
			}
			return null;
		}
		public void SetFieldText()
		{
			if (fields != null)
			{
				QryField fld;
				for (int i = 0; i < fields.Count; i++)
				{
					fld = fieldParser.FindField(fields[i].Name);
					if (fld != null)
					{
						fields[i].FieldText = fld.FieldText;
						fields[i].FromTableName = fld.Table;
					}
				}
			}
		}
		public void SetFieldTable()
		{
			if (fields != null && query.T != null)
			{
				EPField fld;
				for (int i = 0; i < fields.Count; i++)
				{
					if (!fields[i].TableSet())
					{
						for (int k = 0; k < query.T.Count; k++)
						{
							fld = query.T[k].FindField(fields[i].Name);
							if (fld != null)
							{
								if (query.T[k].Alias.Length > 0)
									fields[i].FromTableName = query.T[k].Alias;
								else
									fields[i].FromTableName = query.T[k].TableName;
								break;
							}
						}
					}
				}
			}
		}
		//=======================================================
		public void Clear()
		{
			query.ClearQuery();
			query.T = null;
			fieldParser = new FieldsParser();
			fields = null;
			parameters = null;
		}
		//=======================================================
		public EasyQuery ParseQuery(Form caller, Connection dbConn, string sSelect, out string sMsg)
		{
			query = new EasyQuery();
			query.DatabaseConnection = new ConnectionItem(dbConn);
			ResetConnection();
			return ParseQuery(caller, sSelect, out sMsg);
		}
		//=======================================================
		public EasyQuery ParseQuery(Form caller, string sSelect, out string sMsg)
		{
			bool bSqlOK = false;
			bool isCommand = false;
			string sSQL2 = string.Empty;
			string sFrom = string.Empty;
			string sWhere = string.Empty;
			string sGroupBy = string.Empty;
			string sHaving = string.Empty;
			string sOrder = string.Empty;
			string sLimit = string.Empty;
			string[] sqlPsNames = new string[] { };
			string[] dpPsNames = new string[] { };
			if (sSelect.EndsWith(";", StringComparison.Ordinal))
			{
				sSelect = sSelect.Substring(0, sSelect.Length - 1);
			}
			sSelect = sSelect.Replace('\r', ' ');
			sSelect = sSelect.Replace('\n', ' ');
			sSelect = sSelect.Replace('\t', ' ');
			EasyQuery qry = null;
			sMsg = "";
			sSelect = sSelect.Trim();
			string dataPreparer = string.Empty;
			string sSQL = sSelect;
			bool Distinct = false, Percent = false, WithTies = false;
			int Top = 0; //top value in original SQL
			int ix = FieldsParser.FindStringIndex(sSelect, SQL_Select(), 0, Sep1, Sep2);
			int ixSELECT = ix;
			if (ix < 0)
			{
				//use commands
				sSQL2 = sSelect;
				bSqlOK = true;
				isCommand = true;
				ParameterList plst = parameters;//save original parameters
				StringCollection ps = ParseHavingclause(sSQL2);//get parameters
				//restore parameter attributes
				if (plst != null && plst.Count > 0 && parameters != null && parameters.Count > 0)
				{
					foreach (EPField f in parameters)
					{
						EPField p = plst[f.Name];
						if (p != null)
						{
							f.OleDbType = p.OleDbType;
						}
					}
				}
				sqlPsNames = new string[ps.Count];
				ps.CopyTo(sqlPsNames, 0);
			}
			else
			{
				dataPreparer = sSelect.Substring(0, ix).Trim();
				ix += SQL_Select().Length;
				//}
				int i;
				i = FindNonSpaceWord(sSelect, ix, SQL_Distinct());
				if (i > 0)
				{
					Distinct = true;
					i += 9;
				}
				else
				{
					i = ix;
				}
				int i2;
				EnumTopRecStyle topStyle = schema.dbCon.TopRecStyle;
				if (topStyle == EnumTopRecStyle.TopN)
				{
					i2 = FindNonSpaceWord(sSelect, i, QueryParser.SQL_Top_MS());
					if (i2 > 0)
					{
						//find number following TOP
						Top = 0;
						i = i2 + 3;
						while (i < sSelect.Length)
						{
							if (sSelect[i] == ' ')
								i++;
							else
								break;
						}
						int i0 = i;
						while (i < sSelect.Length)
						{
							if (sSelect[i] != ' ')
								i++;
							else
								break;
						}
						if (i >= sSelect.Length)
						{
							sMsg = "Incomplete SQL statement after TOP clause";
							return null;
						}
						string s0 = sSelect.Substring(i0, i - i0 + 1);
						s0 = s0.Trim();
						if (s0.Length > 0)
						{
							try
							{
								Top = Convert.ToInt32(s0);
							}
							catch
							{
							}
						}
					}
				}
				i2 = FindNonSpaceWord(sSelect, i, QueryParser.SQL_Percent());
				if (i2 > 0)
				{
					Percent = true;
					i = i2 + 7;
				}
				i2 = FindNonSpaceWord(sSelect, i, QueryParser.SQL_WithTies());
				if (i2 > 0)
				{
					WithTies = true;
				}
				int nFrom = 0, nWhere = 0, nGroupBy = 0, nHaving = 0, nOrderBy = 0, nLimit = 0;
				nFrom = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_From(), 0, Sep1, Sep2);
				if (nFrom < 0)
					sMsg = "Missing FROM clause";
				else
				{
					try
					{
						nWhere = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Where(), nFrom, Sep1, Sep2);
						if (nWhere < 0)
							nWhere = 0;
						nGroupBy = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_GroupBy(), nWhere, Sep1, Sep2);
						if (nGroupBy < 0)
							nGroupBy = 0;
						else
						{
							nHaving = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Having(), nGroupBy, Sep1, Sep2);
							if (nHaving < 0)
								nHaving = 0;
						}
						if (nHaving > 0)
						{
							nOrderBy = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_OrderBy(), nHaving, Sep1, Sep2);
						}
						else if (nGroupBy > 0)
						{
							nOrderBy = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_OrderBy(), nGroupBy, Sep1, Sep2);
						}
						else if (nWhere > 0)
						{
							nOrderBy = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_OrderBy(), nWhere, Sep1, Sep2);
						}
						else
						{
							nOrderBy = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_OrderBy(), nFrom, Sep1, Sep2);
						}
						if (nOrderBy < 0)
						{
							nOrderBy = 0;
						}
						if (topStyle == EnumTopRecStyle.Limit)
						{
							if (nOrderBy > 0)
							{
								nLimit = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Top_MySql(), nOrderBy, Sep1, Sep2);
							}
							else if (nHaving > 0)
							{
								nLimit = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Top_MySql(), nHaving, Sep1, Sep2);
							}
							else if (nGroupBy > 0)
							{
								nLimit = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Top_MySql(), nGroupBy, Sep1, Sep2);
							}
							else if (nWhere > 0)
							{
								nLimit = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Top_MySql(), nWhere, Sep1, Sep2);
							}
							else
							{
								nLimit = FieldsParser.FindStringIndex(sSelect, QueryParser.SQL_Top_MySql(), nFrom, Sep1, Sep2);
							}
						}
					}
					catch (Exception er)
					{
						sMsg = er.Message;
					}
					//=============================================
					i = nWhere;
					if (i <= 0)
						i = nGroupBy;
					if (i <= 0)
						i = nOrderBy;
					if (i <= 0)
						i = nLimit;
					if (i > nFrom + 6)
						sFrom = sSelect.Substring(nFrom + 6, i - nFrom - 6);
					else
						sFrom = sSelect.Substring(nFrom + 6);
					sFrom = sFrom.Trim();
					//
					if (nWhere > 0)
					{
						i = nGroupBy;
						if (i <= 0)
							i = nOrderBy;
						if (i <= 0)
							i = nLimit;
						if (i > nWhere + 7)
							sWhere = sSelect.Substring(nWhere + 7, i - nWhere - 7);
						else
							sWhere = sSelect.Substring(nWhere + 7);
					}
					if (nGroupBy > 0)
					{
						if (nHaving > 0)
							i = nHaving;
						else
							i = nOrderBy;
						if (i <= 0)
							i = nLimit;
						if (i > nGroupBy + 10)
							sGroupBy = sSelect.Substring(nGroupBy + 10, i - nGroupBy - 10);
						else
							sGroupBy = sSelect.Substring(nGroupBy + 10);
					}
					if (nHaving > 0)
					{
						i = nOrderBy;
						if (i <= 0)
							i = nLimit;
						if (i > nHaving + 8)
							sHaving = sSelect.Substring(nHaving + 8, i - nHaving - 8);
						else
							sHaving = sSelect.Substring(nHaving + 8);
					}
					if (nOrderBy > 0)
					{
						i = nLimit;
						if (i > nOrderBy + SQL_OrderBy().Length)
							sOrder = sSelect.Substring(nOrderBy + SQL_OrderBy().Length, i - nOrderBy - SQL_OrderBy().Length);
						else
							sOrder = sSelect.Substring(nOrderBy + SQL_OrderBy().Length);
					}
					if (nLimit > 0)
					{
						sLimit = sSelect.Substring(nLimit + SQL_Top_MySql().Length);
						if (!string.IsNullOrEmpty(sLimit))
						{
							sLimit = sLimit.Trim();
							string sli = sLimit;
							if (sLimit.Length > 0)
							{
								i2 = sLimit.IndexOf(' ');
								if (i2 > 0)
								{
									sli = sLimit.Substring(0, i2).Trim();
								}
								try
								{
									Top = Convert.ToInt32(sli);
								}
								catch
								{
								}
							}
						}
					}
					ParameterList plst = parameters;
					ParseFROMclause(sFrom); //get T
					StringCollection scw = ParseWHEREclause(sWhere); //get Parameters
					StringCollection sch = ParseHavingclause(sHaving); //get additional Parameters
					StringCollection dpPs = ParseHavingclause(dataPreparer); //get additional Parameters
					if (plst != null && plst.Count > 0 && parameters != null && parameters.Count > 0)
					{
						foreach (EPField f in parameters)
						{
							EPField p = plst[f.Name];
							if (p != null)
							{
								f.OleDbType = p.OleDbType;
							}
						}
					}
					sqlPsNames = new string[scw.Count + sch.Count];
					dpPsNames = new string[dpPs.Count];
					scw.CopyTo(sqlPsNames, 0);
					sch.CopyTo(sqlPsNames, scw.Count);
					dpPs.CopyTo(dpPsNames, 0);


					if (_topRecStyle != EnumTopRecStyle.NotAllowed && _topNsample > 0)
					{
						if (topStyle == EnumTopRecStyle.TopN)
						{
							if (Top > 0)
							{
								int nTop = FieldsParser.FindStringIndex(sSQL, " TOP ", 0, Sep1, Sep2);
								nTop += 4;
								int nTop2 = sSQL.IndexOf(' ', nTop + 1);
								while (nTop2 == nTop + 1)
								{
									nTop++;
									nTop2 = sSQL.IndexOf(' ', nTop + 1);
								}
								if (nTop2 > nTop)
								{
									sSQL2 = string.Format(CultureInfo.InvariantCulture,
										"{0} {1} {2}", sSQL.Substring(0, nTop), _topNsample, sSQL.Substring(nTop2));
								}
								else
								{
									sSQL2 = sSQL;
								}
							}
							else
							{
								//SELECT TOP n / SELECT DISTINCT TOP n
								int nDist = FindNonSpaceWord(sSQL, 6 + ixSELECT, SQL_Distinct());
								if (nDist < 0)
								{
									sSQL2 = string.Format(CultureInfo.InvariantCulture,
										"{0} {3} {1} {2}", sSQL.Substring(0, 6 + ixSELECT), _topNsample, sSQL.Substring(7 + ixSELECT), QueryParser.SQL_Top_MS());
								}
								else
								{
									sSQL2 = string.Format(CultureInfo.InvariantCulture,
										"{0} {3} {1} {2}", sSQL.Substring(0, nDist + 8), _topNsample, sSQL.Substring(nDist + 9), QueryParser.SQL_Top_MS());
								}
							}
						}
						else if (topStyle == EnumTopRecStyle.Limit)
						{
							if (sSQL2.EndsWith(";", StringComparison.Ordinal))
							{
								sSQL2 = sSQL2.Substring(0, sSQL2.Length - 1);
							}
							if (nLimit > 0)
							{
								sSQL2 = string.Format(CultureInfo.InvariantCulture,
									"{0} {2} {1}", sSQL.Substring(0, nLimit), _topNsample, QueryParser.SQL_Top_MySql());
							}
							else
							{
								sSQL2 = string.Format(CultureInfo.InvariantCulture,
									"{0} {2} {1}", sSQL, _topNsample, QueryParser.SQL_Top_MySql());
							}
						}
					}
					else
						sSQL2 = sSQL;
					bSqlOK = true;
				}
			}
			if (bSqlOK)
			{
				bSqlOK = false;
				if (sSQL2.Length > 0)
				{
					try
					{
						//create fields, parameters
						getQueryFields(caller, sSQL2, isCommand);
						bSqlOK = true;
					}
					catch (Exception er2)
					{
						if (string.IsNullOrEmpty(sMsg))
							sMsg = er2.Message;
						else
							sMsg = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", sMsg, er2.Message);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(sMsg))
						sMsg = "SQL Syntax error.";
					else
						sMsg = string.Format(CultureInfo.InvariantCulture, "{0}\r\nSQL Syntax error.", sMsg);
				}
				if (bSqlOK) //get fieldParser.Fields
				{
					try
					{
						//try to find table name or alias from T
						//for those missing table names in fields
						query.T = T;
						qry = new EasyQuery();
						qry.IsCommand = isCommand;
						qry.DataPreparer = dataPreparer;
						qry.DataPreparerParameternames = dpPsNames;
						qry.SqlParameternames = sqlPsNames;
						qry.Where = sWhere;
						qry.From = sFrom;
						qry.GroupBy = sGroupBy;
						qry.Having = sHaving;
						qry.OrderBy = sOrder;
						qry.Limit = sLimit;
						qry.Top = Top;
						qry.Fields = fields;
						qry.Parameters = parameters;
						qry.T = T;
						qry.Distinct = Distinct;
						qry.Percent = Percent;
						qry.WithTies = WithTies;
						qry.SampleTopRec = _topNsample;

						qry.DatabaseConnection = query.DatabaseConnection;
						if (isCommand)
						{
							qry.SetSQL(new SQLStatement(sSelect));
						}
						checkQueryreadOnly(qry);

					}
					catch (Exception er)
					{
						if (string.IsNullOrEmpty(sMsg))
							sMsg = er.Message;
						else
							sMsg = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", sMsg, er.Message);
					}
				}
			}
			return qry;
		}
		/// <summary>
		/// check if the query is updateable
		/// 1. single table
		/// 2. include all fields for an unique index
		/// ===changed: no need to be a single table
		/// </summary>
		/// <param name="qry"></param>
		public void checkQueryreadOnly(EasyQuery qry)
		{
			qry.UpdatableTableName = string.Empty;
			if (!qry.IsCommand)
			{
				if (qry.Fields != null)
				{
					int i;
					bool bFound = false;
					//find out all tables
					StringCollection lst = new StringCollection();
					for (i = 0; i < qry.Fields.Count; i++)
					{
						if (!string.IsNullOrEmpty(qry.Fields[i].FromTableName))
						{
							bFound = false;
							for (int j = 0; j < lst.Count; j++)
							{
								if (string.Compare(lst[j], qry.Fields[i].FromTableName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									bFound = true;
									break;
								}
							}
							if (!bFound)
							{
								lst.Add(qry.Fields[i].FromTableName);
							}
						}
					}
					//go through all tables to find unique index
					int nC = qry.Fields.Count;
					//
					bFound = false;
					for (i = 0; i < nC; i++)
					{
						if (qry.Fields[i].IsIdentity)
						{
							qry.UpdatableTableName = qry.Fields[i].FromTableName;
							bFound = true;
							for (int j = 0; j < nC; j++)
							{
								if (!qry.Fields[j].IsIdentity)
								{
									qry.Fields[j].Indexed = false;
								}
							}
							break;
						}
					}
					if (!bFound)
					{
						for (i = 0; i < nC; i++)
						{
							if (qry.Fields[i].Indexed)
							{
								qry.UpdatableTableName = qry.Fields[i].FromTableName;
								bFound = true;
								break;
							}
						}
					}
					if (!bFound)
					{
						for (int t = 0; t < lst.Count; t++)
						{
							TableAlias tbl = new TableAlias();
							tbl.TableName = lst[t];
							tbl.LoadSchema(schema);
							//if the unique index found for this table, indexed Fields are marked
							bFound = tbl.MarkUniqueIndexFields(qry.Fields);
							if (bFound)
							{
								qry.UpdatableTableName = lst[t];
								break;
							}
						}
					}

					//updatable query, set default editor
					if (!string.IsNullOrEmpty(qry.UpdatableTableName))
					{
						for (i = 0; i < qry.Fields.Count; i++)
						{
							if (!qry.Fields[i].ReadOnly)
							{
								if (string.Compare(qry.Fields[i].FromTableName, qry.UpdatableTableName, StringComparison.OrdinalIgnoreCase) != 0)
								{
									qry.Fields[i].ReadOnly = true;
								}
								else
								{
									qry.Fields[i].SetDefaultEditor();
								}
							}
						}
					}
				}
			}
		}
		public EPField FindParameter(string sName)
		{
			if (parameters != null)
			{
				return parameters[sName];
			}
			return null;
		}
		public StringCollection ParseWHEREclause(string sWhere)
		{
			StringCollection sc = new StringCollection();
			sWhere = sWhere.Replace('\r', ' ');
			sWhere = sWhere.Replace('\n', ' ');
			sWhere = sWhere.Replace('\t', ' ');
			ParameterList ps = new ParameterList();
			int i;
			string sName;
			EPField fld;
			sWhere = sWhere.Trim();
			int n = SQLParser.FindParameterIndex(sWhere, 0, Sep1, Sep2, out ParseError);
			while (n >= 0)
			{
				i = SQLParser.FindNameEnd(sWhere, n + 2);
				sName = sWhere.Substring(n, i - n);
				fld = FindParameter(sName);
				if (fld == null)
				{
					fld = new EPField();
					fld.Name = sName;
					fld.OleDbType = System.Data.OleDb.OleDbType.VarWChar;
					fld.DataSize = 255;
				}
				sc.Add(sName);
				ps.AddField(fld);
				n = SQLParser.FindParameterIndex(sWhere, i + 1, Sep1, Sep2, out ParseError);
			}
			parameters = ps;
			return sc;
		}
		public StringCollection ParseHavingclause(string sHaving)
		{
			StringCollection sc = new StringCollection();
			sHaving = sHaving.Replace('\r', ' ');
			sHaving = sHaving.Replace('\n', ' ');
			sHaving = sHaving.Replace('\t', ' ');
			ParameterList ps = new ParameterList();
			int i;
			string sName;
			EPField fld;
			sHaving = sHaving.Trim();
			int n = SQLParser.FindParameterIndex(sHaving, 0, Sep1, Sep2, out ParseError);
			while (n >= 0)
			{
				i = SQLParser.FindNameEnd(sHaving, n + 2);
				sName = sHaving.Substring(n, i - n);
				fld = FindParameter(sName);
				if (fld == null)
				{
					fld = new EPField();
					fld.Name = sName;
					fld.OleDbType = System.Data.OleDb.OleDbType.VarWChar;
					fld.DataSize = 255;
				}
				sc.Add(sName);
				ps.AddField(fld);
				n = SQLParser.FindParameterIndex(sHaving, i + 1, Sep1, Sep2, out ParseError);
			}
			if (parameters == null)
				parameters = ps;
			else
			{
				parameters.AppendList(ps);
			}
			return sc;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFrom">FROM clause only, no other clauses</param>
		private void ParseFROMclause(string sFrom)
		{
			int i;
			string s;
			string sc;
			int sepLeng = 0;
			if (!string.IsNullOrEmpty(Sep1) && !string.IsNullOrEmpty(Sep2))
			{
				sepLeng = Sep1.Length + Sep2.Length;
			}
			sFrom = sFrom.Replace('\r', ' ');
			sFrom = sFrom.Replace('\n', ' ');
			sFrom = sFrom.Replace('\t', ' ');
			T = new TableAliasList();
			while (sFrom.Length > 0)
			{
				sFrom = sFrom.Trim();
				if (sFrom.Length <= 0)
					return;
				while (sFrom[0] == '(')
				{
					sFrom = sFrom.Substring(1);
					sFrom = sFrom.Trim();
					if (sFrom.Length == 0)
						return;
				}
				while (sFrom[0] == ')')
				{
					sFrom = sFrom.Substring(1);
					sFrom = sFrom.Trim();
					if (sFrom.Length == 0)
						return;
				}
				if (sFrom[0] == '\'')
				{
					i = 1;
					while (i < sFrom.Length)
					{
						if (sFrom[i] != '\'')
							i++;
						else
						{
							if (i + 1 < sFrom.Length)
							{
								if (sFrom[i + 1] == '\'') //double \'
								{
									i += 2;
								}
								else //single \'
								{
									i++;
									break;
								}
							}
							else
							{
								i++;
							}
						}
					}
					sFrom = sFrom.Substring(i);
					sFrom = sFrom.Trim();
					if (sFrom.Length == 0)
						return;
				}
				string sTable = "";
				i = sFrom.IndexOf(' ');
				if (i < 1)
				{
					sTable = sFrom;
					sFrom = "";
				}
				else
				{
					sTable = sFrom.Substring(0, i);
					sFrom = sFrom.Substring(i + 1);
				}
				i = sTable.IndexOf('\r');
				if (i < 0)
					i = sTable.IndexOf('\n');
				if (i == 0)
				{
					sFrom = sTable.Substring(1) + " " + sFrom;
					sTable = "";
				}
				else if (i > 0)
				{
					sFrom = sTable.Substring(i + 1) + " " + sFrom;
					sTable = sTable.Substring(0, i);
				}
				i = sTable.IndexOf('\n');
				if (i == 0)
				{
					sFrom = sTable.Substring(1) + " " + sFrom;
					sTable = "";
				}
				else if (i > 0)
				{
					sFrom = sTable.Substring(i + 1) + " " + sFrom;
					sTable = sTable.Substring(0, i);
				}
				i = sTable.IndexOf('\t');
				if (i == 0)
				{
					sFrom = sTable.Substring(1) + " " + sFrom;
					sTable = "";
				}
				else if (i > 0)
				{
					sFrom = sTable.Substring(i + 1) + " " + sFrom;
					sTable = sTable.Substring(0, i);
				}
				while (sTable.Length > 0)
				{
					if (sTable[sTable.Length - 1] == ')')
					{
						if (sTable.Length == 1)
						{
							sTable = "";
							break;
						}
						else
							sTable = sTable.Substring(0, sTable.Length - 1);
					}
					else
						break;
				}
				bool bCheck = true;
				if (sTable.Length > 0)
				{
					i = sTable.IndexOf(",", StringComparison.Ordinal);
					if (i >= 0)
					{
						if (i == 0)
						{
							sTable = sTable.Substring(1);
							sTable = sTable.Trim();
						}
						else if (i == sTable.Length - 1)
						{
							sTable = sTable.Substring(0, i);
							sTable = sTable.Trim();
							bCheck = false;
						}
						else
						{
							sFrom = sTable.Substring(i) + " " + sFrom;
							sFrom = sFrom.Trim();
							sTable = sTable.Substring(0, i);
							sTable = sTable.Trim();
							bCheck = false;
						}
					}
				}
				if (sTable.Length > 0)
				{
					sTable = sTable.Trim();
					if (sepLeng > 0 && sTable.StartsWith(Sep1, StringComparison.OrdinalIgnoreCase) && sTable.EndsWith(Sep2, StringComparison.OrdinalIgnoreCase))
					{
						if (sTable.Length > sepLeng)
							sTable = sTable.Substring(Sep1.Length, sTable.Length - sepLeng);
					}
					sFrom = sFrom.Trim();

					if (sTable.Length > 0)
					{
						sc = sTable;
						if (!FieldsParser.IsFieldKeyword(sc))
						{
							if (sepLeng > 0 && sTable.StartsWith(Sep1, StringComparison.OrdinalIgnoreCase)
								&& sTable.EndsWith(Sep2, StringComparison.OrdinalIgnoreCase) && sTable.Length > sepLeng)
							{
								sTable = sTable.Substring(Sep1.Length, sTable.Length - sepLeng);
							}
							if (IsTable(sc))
							{
								TableAlias tbl = new TableAlias();
								tbl.TableName = sTable;

								//check (Nolock)
								if (bCheck)
								{
									if (sFrom.Length > 0)
									{
										if (sFrom[0] == '(')
										{
											bCheck = false;
											i = sFrom.IndexOf(')');
											if (i > 0)
											{
												s = sFrom.Substring(1, i - 1);
												s = s.Trim();
												if (string.Compare(s, "NOLOCK", StringComparison.OrdinalIgnoreCase) == 0)
												{
													bCheck = true;
													sFrom = sFrom.Substring(i + 1);
													sFrom = sFrom.Trim();
												}
											}
										}
									}
									else
										bCheck = false;
								}
								if (bCheck && sFrom.Length > 0)
								{
									//check alias
									if (sFrom.StartsWith("AS ", StringComparison.OrdinalIgnoreCase))
									{
										sFrom = sFrom.Substring(3);
										sFrom = sFrom.Trim();

										if (sepLeng > 0 && sFrom.StartsWith(Sep1, StringComparison.OrdinalIgnoreCase))
										{
											i = sFrom.IndexOf(Sep2);
											if (i > 0)
											{
												tbl.Alias = sFrom.Substring(0, i + Sep2.Length);
												sFrom = sFrom.Substring(i + Sep2.Length);
											}
										}
										else
										{
											i = sFrom.IndexOf(' ');
											if (i > 0)
											{
												tbl.Alias = sFrom.Substring(0, i);
												sFrom = sFrom.Substring(i + 1);
											}
										}
										sFrom = sFrom.Trim();
									}
									else if (sFrom[0] != ',')
									{
										i = sFrom.IndexOf(' ');
										if (i < 0)
										{
											s = sFrom;
										}
										else
											s = sFrom.Substring(0, i);
										s = s.Trim();
										if (!FieldsParser.IsJoinKeyword(s))
										{
											tbl.Alias = s;
										}

									}
								}
								T.AddTable(tbl);
							}
						}
					}
				}
			}
			for (i = 0; i < T.Count; i++)
			{
				T[i].LoadSchema(schema);
			}
		}
		public bool IsTable(string table)
		{
			if (!string.IsNullOrEmpty(Sep1) && !string.IsNullOrEmpty(Sep2))
			{
				if (table.Length > Sep1.Length + Sep2.Length)
				{
					if (table.StartsWith(Sep1, StringComparison.OrdinalIgnoreCase) && table.EndsWith(Sep2, StringComparison.OrdinalIgnoreCase))
					{
						table = table.Substring(Sep1.Length, table.Length - Sep1.Length - Sep2.Length);
					}
				}
			}
			for (int i = 0; i < schema.TableCount; i++)
			{
				if (string.Compare(table, tablenames[i], StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}
			return false;
		}
		public static bool IsSingleTable(string sFrom, string sepStart, string sepEnd, out string tableName)
		{
			char c1 = '\0';
			char c2 = '\0';
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				c1 = sepStart[0];
				c2 = sepEnd[0];
			}
			tableName = "";
			sFrom = sFrom.Trim();
			if (sFrom.Length <= 0)
				throw new Exception("Invalid FROM clause");
			//sFrom = sFrom;
			if (sFrom[0] == '(')
			{
				return false;
			}
			int i;
			bool bGotTable = false;
			//remove table name=====================
			if (sFrom[0] == c1)
			{
				for (i = 1; i < sFrom.Length; i++)
				{
					if (sFrom[i] == c2)
					{
						if (i > 1)
						{
							tableName = sFrom.Substring(1, i - 1);
							if (i == sFrom.Length - 1)
								return true;
							bGotTable = true;
							sFrom = sFrom.Substring(i + 1);
						}
						break;
					}
				}
				if (!bGotTable)
					throw new Exception("Invalid FROM clause");
			}
			else
			{
				i = sFrom.IndexOf(' ');
				if (i < 0)
				{
					tableName = sFrom;
					return true;
				}
				if (i == sFrom.Length - 1)
				{
					tableName = sFrom;
					return true;
				}
				tableName = sFrom.Substring(0, i);
				sFrom = sFrom.Substring(i + 1);
			}
			//find table alias
			sFrom = sFrom.Trim();
			if (sFrom.Length == 0)
				return true;
			if (sFrom.Length > 2)
			{
				if (sFrom.StartsWith("AS ", StringComparison.OrdinalIgnoreCase))
				{
					sFrom = sFrom.Substring(3);
					sFrom = sFrom.Trim();
					if (sFrom.Length == 0)
						throw new Exception("Invalid FROM clause");
				}
			}

			if (sFrom[0] == c1)
			{
				for (i = 1; i < sFrom.Length; i++)
				{
					if (sFrom[i] == c2)
					{
						if (i == sFrom.Length - 1)
							return true;
						bGotTable = true;
						sFrom = sFrom.Substring(i + 1);
						break;
					}
				}
				if (!bGotTable)
					throw new Exception("Invalid FROM clause");
			}
			else
			{
				i = sFrom.IndexOf(' ');
				if (i < 0)
					return true;
				if (i == sFrom.Length - 1)
					return true;
				sFrom = sFrom.Substring(i + 1);
			}
			//find nolock
			sFrom = sFrom.Trim();
			if (sFrom.IndexOf("(NOLOCK)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				sFrom = sFrom.Substring(7);
				sFrom = sFrom.Trim();
			}
			if (sFrom.Length == 0)
				return true;
			//if we still have chars left, we have more than one table
			return false;
		}
		/// <summary>
		/// search starting from nStart, check if the next non-space is word
		/// </summary>
		/// <param name="s"></param>
		/// <param name="nStart"></param>
		/// <param name="word"></param>
		/// <returns></returns>
		public static int FindNonSpaceWord(string s, int nStart, string word)
		{
			if (s == null)
				return -1;
			if (s.Length <= nStart)
				return -1;
			for (int i = nStart; i < s.Length - word.Length; i++)
			{
				if (s[i] != ' ') //skip space
				{
					if (s.IndexOf(word, i, StringComparison.OrdinalIgnoreCase) == i)
					{
						return i;
					}
					else
						return -1;
				}
			}
			return -1;
		}
	}
}
