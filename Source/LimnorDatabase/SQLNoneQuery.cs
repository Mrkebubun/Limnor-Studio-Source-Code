/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace LimnorDatabase
{
	public enum enmNonQueryType { Insert = 0, Update, Delete, StoredProcedure, Script }
	/// <summary>
	/// 
	/// </summary>
	public class SQLNoneQuery : ICloneable
	{
		#region fields and constructors
		//for insert
		public const string SQL_Ignore = "IGNORE ";
		public const string SQL_LowPriority = "LOW_PRIORITY ";
		public const string SQL_HighPriority = "HIGH_PRIORITY ";
		//
		private enmNonQueryType cmdType = enmNonQueryType.Insert;
		private string _filter = string.Empty; //from/where
		private bool _multiRow = true; //use SELECT for insert
		private bool _useIgnore;
		private bool _useLowPriority;
		private bool _useHighPriority;
		private EasyQuery qry = null; //save query for multi-row operations
		private ConnectionItem connect = null; //to be set from outside
		private string sTableName = string.Empty;
		private FieldList fields = null;
		private ParameterList _parameters = null;
		private string ParseError = string.Empty;
		private int nAttributes = 0;
		private string _rawSQL = string.Empty;
		private string sepStart = "";
		private string sepEnd = "";
		public SQLNoneQuery()
		{
		}
		public SQLNoneQuery(string sTable, FieldList flds, ParameterList pams)
		{
			sTableName = sTable;
			fields = flds;
			_parameters = pams;
		}
		#endregion

		#region Properties
		public string ErrorMessage
		{
			get
			{
				return ParseError;
			}
		}
		public bool MultiRow
		{
			get
			{
				return _multiRow;
			}
			set
			{
				_multiRow = value;
			}
		}
		public ConnectionItem Connection
		{
			get
			{
				return connect;
			}
		}
		public enmNonQueryType CommandType
		{
			get
			{
				return cmdType;
			}
			set
			{
				cmdType = value;
			}
		}
		public string Filter
		{
			get
			{
				return _filter;
			}
		}
		public string TableName
		{
			get
			{
				return sTableName;
			}
			set
			{
				if (value == null)
					sTableName = string.Empty;
				else
					sTableName = value;
			}
		}
		public int FieldCount
		{
			get
			{
				if (fields != null)
					return fields.Count;
				return 0;
			}
		}
		[XmlIgnore]
		[DefaultValue(null)]
		[Description("For creating Insert/Update command this query provides the records as the source data for inserting and updating. For creating Delete command this query represents the records to be deleted.")]
		public EasyQuery SourceQuery
		{
			get
			{
				return qry;
			}
			set
			{
				qry = value;
			}
		}
		public int ParamCount
		{
			get
			{
				if (_parameters != null)
					return _parameters.Count;
				return 0;
			}
		}
		public ParameterList Parameters
		{
			get
			{
				return _parameters;
			}
		}
		public string SQL
		{
			get
			{
				if (!string.IsNullOrEmpty(_rawSQL))
				{
					return _rawSQL;
				}
				if (cmdType != enmNonQueryType.StoredProcedure)
				{
					int i;
					int n = FieldCount;
					StringBuilder s = new StringBuilder();
					bool b;
					string sep1 = "[", sep2 = "]";
					if (connect != null)
					{
						sep1 = DatabaseEditUtil.SepBegin(connect.NameDelimiterStyle);
						sep2 = DatabaseEditUtil.SepEnd(connect.NameDelimiterStyle);
					}
					if (Filter != null)
						_filter = Filter.Trim();
					switch (cmdType)
					{
						case enmNonQueryType.Insert:
							s.Append("INSERT ");
							if (_useIgnore)
							{
								s.Append(SQL_Ignore);
							}
							if (_useLowPriority)
							{
								s.Append(SQL_LowPriority);
							}
							else if (_useHighPriority)
							{
								s.Append(SQL_HighPriority);
							}
							if (Filter == null)
								MultiRow = false;
							else if (string.IsNullOrEmpty(Filter))
								MultiRow = false;
							else
								MultiRow = true;
							if (!MultiRow)
								s.Append("INTO ");
							else if ((nAttributes & 1) != 0)
								s.Append("INTO ");
							s.Append(sep1);
							s.Append(sTableName);
							s.Append(sep2);
							s.Append(" (");
							if (n > 0)
							{
								s.Append(sep1);
								s.Append(fields[0].Name);
								s.Append(sep2);
								for (i = 1; i < n; i++)
								{
									s.Append(",");
									s.Append(sep1);
									s.Append(fields[i].Name);
									s.Append(sep2);
								}
							}
							s.Append(") ");
							if (MultiRow)
							{
								s.Append(QueryParser.SQL_Select());
							}
							else
							{
								s.Append(QueryParser.SQL_Values());
								s.Append("(");
							}
							if (n > 0)
							{
								s.Append(fields[0].GetFieldTextAsValue(sep1, sep2));
								for (i = 1; i < n; i++)
								{
									s.Append(",");
									s.Append(fields[i].GetFieldTextAsValue(sep1, sep2));
								}
							}
							if (MultiRow)
							{
								if (!string.IsNullOrEmpty(Filter))
								{
									_filter = Filter.Trim();
									if (Filter.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
									{
										s.Append(" ");
										s.Append(Filter);
									}
									else
									{
										s.Append(QueryParser.SQL_From());
										s.Append(Filter);
									}
								}
							}
							else
							{
								s.Append(")");
							}
							break;
						case enmNonQueryType.Update:
							s.Append(QueryParser.SQL_Update());
							s.Append(sep1);
							s.Append(sTableName);
							s.Append(sep2);
							s.Append(QueryParser.SQL_Set());
							if (n > 0)
							{
								s.Append(sep1);
								s.Append(fields[0].Name);
								s.Append(sep2);
								s.Append("=");
								s.Append(fields[0].GetFieldTextAsValue(sep1, sep2));
								for (i = 1; i < n; i++)
								{
									s.Append(",");
									s.Append(sep1);
									s.Append(fields[i].Name);
									s.Append(sep2);
									s.Append("=");
									s.Append(fields[i].GetFieldTextAsValue(sep1, sep2));
								}
							}
							//check FROM WHERE
							b = false;
							if (Filter.Length > 5)
							{
								if (Filter.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
								{
									b = true;
								}
							}
							if (!b)
							{

								if (Filter.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
								{
									b = true;
								}
								else if (string.Compare(Filter, "WHERE", StringComparison.OrdinalIgnoreCase) == 0)
								{
									b = true;
								}
							}
							if (b)
							{
								s.Append(" ");
								s.Append(Filter);
							}
							else
							{
								if (Filter.Length > 0)
								{
									s.Append(QueryParser.SQL_Where());
									s.Append(Filter);
								}
							}
							break;
						case enmNonQueryType.Delete:
							s.Append(QueryParser.SQL_Delete());
							s.Append(sep1);
							s.Append(sTableName);
							s.Append(sep2);
							if (Filter == null)
								_filter = "";
							MultiRow = false;
							if (Filter.Length > 0)
							{
								if (Filter.Length > 5)
								{
									if (Filter.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
									{
										MultiRow = true;
									}
								}
							}
							if (MultiRow) //multi-table
							{
								s.Append(" ");
								s.Append(Filter);
							}
							else
							{
								//delete tablename.* FROM tablename [WHERE <filter>]
								s.Append(QueryParser.SQL_From());
								s.Append(sep1);
								s.Append(sTableName);
								s.Append(sep2);
								s.Append(" ");
								if (Filter.Length > 5)
								{
									if (!Filter.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
										s.Append(QueryParser.SQL_Where());
									s.Append(Filter);
								}
								else if (Filter.Length > 0) //Filter.Length <= 5
								{
									if (!("WHERE ".StartsWith(Filter, StringComparison.OrdinalIgnoreCase)))
									{
										s.Append(QueryParser.SQL_Where());
										s.Append(Filter);
									}
								}

							}
							break;
					}
					return s.ToString();
				}
				return string.Empty;
			}
			set
			{
				_rawSQL = value;
			}
		}

		#endregion

		#region Methods
		public void RemoveField(string name)
		{
			if (fields != null)
			{
				EPField f = fields[name];
				if (f != null)
				{
					fields.Remove(f);
				}
			}
		}
		public void AddField(EPField f0)
		{
			if (fields == null)
			{
				fields = new FieldList();
			}
			EPField f = fields[f0.Name];
			if (f == null)
			{
				fields.Add(f0);
			}
		}
		public void AddField(string name)
		{
			if (fields == null)
			{
				fields = new FieldList();
			}
			EPField f = fields[name];
			if (f == null)
			{
				f = new EPField();
				f.Name = name;
				fields.AddField(f);
			}
		}
		public string GetSQLStatement(FieldList pmMap, EnumParameterStyle paramStyle)
		{
			string sSQL = SQL;
			if (_parameters != null)
			{
				if (_parameters.Count > 0)
				{
					sSQL = SQLParser.MapParameters(sSQL, paramStyle, sepStart, sepEnd, _parameters, pmMap, _parameters.Clone() as ParameterList);
				}
			}
			return sSQL;
		}



		public void SetConnection(ConnectionItem connection)
		{
			connect = connection;
			sepStart = connect.ConnectionObject.NameDelimiterBegin;
			sepEnd = connect.ConnectionObject.NameDelimiterEnd;
		}
		public FieldList GetFields()
		{
			return fields;
		}
		public void SetParameters(ParameterList ps)
		{
			_parameters = ps;
		}
		public ParameterList ParseParams()
		{
			ParameterList flds = parseParams(SQL);
			//merge types from old parameters
			if (_parameters != null)
			{
				EPField fld;
				for (int i = 0; i < _parameters.Count; i++)
				{
					fld = flds[_parameters[i].Name];
					if (fld != null)
					{
						fld.OleDbType = _parameters[i].OleDbType;
						fld.SetValue(_parameters[i].Value);
					}
				}
			}
			_parameters = flds;
			return _parameters;
		}
		private ParameterList parseParams(string sWhere)
		{
			sWhere = sWhere.Replace('\r', ' ');
			sWhere = sWhere.Replace('\n', ' ');
			sWhere = sWhere.Replace('\t', ' ');
			ParameterList ps = new ParameterList();
			int i;
			string sName;
			EPField fld;
			sWhere = sWhere.Trim();
			int n = SQLParser.FindParameterIndex(sWhere, 0, sepStart, sepEnd, out ParseError);
			while (n >= 0)
			{
				i = SQLParser.FindNameEnd(sWhere, n + 2);
				sName = sWhere.Substring(n, i - n);
				if (string.CompareOrdinal(sName, "@") != 0)
				{
					fld = ps[sName];
					if (fld == null)
					{
						fld = new EPField();
						fld.Name = sName;
						fld.OleDbType = System.Data.OleDb.OleDbType.Empty; //unknown for now
						fld.DataSize = 255;
					}
					ps.AddField(fld);
				}
				n = SQLParser.FindParameterIndex(sWhere, i + 1, sepStart, sepEnd, out ParseError);
			}
			return ps;
		}
		public void SetFields(FieldList flds)
		{
			fields = flds;
		}


		public EPField GetField(int index)
		{
			if (fields != null)
			{
				return fields[index];
			}
			return null;
		}
		public EPField GetField(string name)
		{
			if (fields != null)
			{
				return fields[name];
			}
			return null;
		}
		public void SetFilter(string f)
		{
			_filter = f;
			//
		}
		public void ResetSQL()
		{
			_rawSQL = string.Empty;
		}

		public EPField GetParam(int index)
		{
			if (_parameters != null && index >= 0 && index < _parameters.Count)
			{
				return _parameters[index];
			}
			return null;
		}
		public override string ToString()
		{
			return SQL;
		}
		public bool parseSQLNoneQuery(Form caller, string sSQL, out string sMsg)
		{
			sMsg = "";
			if (sSQL == null)
				sSQL = "";
			_rawSQL = sSQL;
			sSQL = sSQL.Trim();
			if (sSQL.Length > 0)
			{
				if (string.IsNullOrEmpty(sTableName))
				{
					return true;
				}
				int i;
				string s;
				fields = new FieldList();
				if (sSQL.StartsWith("INSERT ", StringComparison.OrdinalIgnoreCase) || sSQL.StartsWith("REPLACE ", StringComparison.OrdinalIgnoreCase))
				{
					cmdType = enmNonQueryType.Insert;
				}
				else if (sSQL.StartsWith("UPDATE ", StringComparison.OrdinalIgnoreCase))
				{
					cmdType = enmNonQueryType.Update;
				}
				else if (sSQL.StartsWith("DELETE ", StringComparison.OrdinalIgnoreCase))
				{
					cmdType = enmNonQueryType.Delete;
				}
				else
				{
					sMsg = "Invalid command type.";
					return false;
				}
				sSQL = sSQL.Substring(7);
				sSQL = sSQL.Trim();
				//
				if (sSQL.StartsWith(SQL_Ignore, StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(SQL_Ignore.Length);
					sSQL = sSQL.Trim();
					_useIgnore = true;
				}
				else
				{
					_useIgnore = false;
				}
				//
				if (sSQL.StartsWith(SQL_LowPriority, StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(SQL_LowPriority.Length);
					sSQL = sSQL.Trim();
					_useLowPriority = true;
				}
				else
				{
					_useLowPriority = false;
				}
				//
				if (sSQL.StartsWith(SQL_HighPriority, StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(SQL_HighPriority.Length);
					sSQL = sSQL.Trim();
					_useHighPriority = true;
				}
				else
				{
					_useHighPriority = false;
				}
				//
				ParameterList ps = SQLParser.ParseParameters(sSQL, sepStart, sepEnd);
				if (_parameters != null)
				{
					if (_parameters.Count > 0)
					{
						for (i = 0; i < _parameters.Count && i < ps.Count; i++)
						{
							if (string.Compare(ps[i].Name, _parameters[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
							{
								ps[i].OleDbType = _parameters[i].OleDbType;
								ps[i].SetValue(_parameters[i].Value);
							}
						}
					}
				}
				////////////////////////////////////////////////////////////////
				if (cmdType == enmNonQueryType.Delete)
				{
					_parameters = ps;
					//MultiRow is what its name means
					if (sSQL.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
					{
						//delete FROM tablename [WHERE <filter>]
						//if filter does not begin with WHERE, it needs to be added
						MultiRow = false;
						sSQL = sSQL.Substring(5);
						sSQL = sSQL.Trim();
					}
					else
					{
						//delete tablename.* [FROM|WHERE <filter>]
						//the part after tablename is the FROM and WHERE properties of EPQuery
						//filter must begin with FROM or WHERE.
						MultiRow = true;
					}
					i = finTabledNameEnd(sSQL); //i is the index including end-delimiter length
					if (i < 0)
					{
						sMsg = "Table name not found.";
						return false;
					}
					s = sSQL.Substring(0, i + 1);
					if (s.Length > 2)
					{
						if (s[s.Length - 1] == '*')
						{
							s = s.Substring(0, s.Length - 1);
						}
						if (s[s.Length - 1] == '.')
						{
							s = s.Substring(0, s.Length - 1);
						}
					}
					TableName = getname(s);
					//pop sSQL
					if (i >= sSQL.Length - 1)
						sSQL = "";
					else
						sSQL = sSQL.Substring(i + 1);
					if (sSQL.StartsWith(".*", StringComparison.Ordinal))
					{
						sSQL = sSQL.Substring(2);
					}
					sSQL = sSQL.Trim();
					if (MultiRow)
					{
						//delete tablename[.*] FROM <tables joins> [WHERE <filter>]
						if (sSQL.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
						{

							//							sSQL = sSQL.Substring(5);
							//							sSQL = sSQL.Trim();
							_filter = sSQL;
						}
						else
						{
							sMsg = "Missing FROM";
							return false;
						}
					}
					else
					{
						//delete FROM tablename [WHERE <filter>]
						if (sSQL.Length > 0)
						{
							if (sSQL.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
							{
								_filter = sSQL;
							}
							else
							{
								sMsg = "Missing WHERE";
								return false;
							}
						}
					}
				}
				//////////////////////////////////////////////////////////
				QueryParser qp = new QueryParser();
				if (cmdType == enmNonQueryType.Insert)
				{
					if (sSQL.StartsWith("INTO ", StringComparison.OrdinalIgnoreCase))
					{
						sSQL = sSQL.Substring(4);
						sSQL = sSQL.Trim();
						MultiRow = (sSQL.IndexOf(" SELECT ", StringComparison.OrdinalIgnoreCase) > 0);
						nAttributes |= 1;
					}
					else
					{
						nAttributes &= ~1;
						MultiRow = true;
					}
					i = finTabledNameEnd(sSQL);
					if (i < 0)
					{
						sMsg = "Missing table name.";
						return false;
					}
					s = sSQL.Substring(0, i + 1);
					TableName = getname(s);
					if (i >= sSQL.Length - 1)
						sSQL = "";
					else
						sSQL = sSQL.Substring(i + 1);
					sSQL = sSQL.Trim();
					if (sSQL.Length < 7)
					{
						sMsg = "SQL statement not completed";
						return false;
					}
					if (!MultiRow)
					{
						if (sSQL[0] != '(')
						{
							sMsg = "Missing '('";
							return false;
						}
						sSQL = sSQL.Substring(1); //sSQL.Length >= 7
						sSQL = sSQL.Trim();
						fields = new FieldList();
						i = 0;
						while (true)
						{
							s = popFieldName(ref sSQL);
							s = s.Trim();
							if (s.Length > 0)
							{
								fields.AddField(new EPField(i, getname(s)));
								i++;
								if (sSQL.Length == 0)
									break;
								if (sSQL[0] == ')')
								{
									break;
								}
							}
							else
								break;
						}
						if (sSQL.Length == 0)
						{
							sMsg = "Missing ')'";
							return false;
						}
						if (sSQL[0] != ')')
						{
							sMsg = "Missing ')'";
							return false;
						}
						sSQL = sSQL.Substring(1);
						sSQL = sSQL.Trim();
						if (sSQL.StartsWith("VALUES ", StringComparison.OrdinalIgnoreCase))
						{
							sSQL = sSQL.Substring(6);
							sSQL = sSQL.Trim();
						}
						else
						{
							sMsg = "Missing VALUES";
							return false;
						}
						if (sSQL.Length == 0)
						{
							sMsg = "Values missing";
							return false;
						}
						if (sSQL[0] != '(')
						{
							sMsg = "Missing '('";
							return false;
						}
						sSQL = sSQL.Substring(1);
						sSQL = sSQL.Trim();
						//parse values
						i = 0;
						while (sSQL.Length > 0 && i < fields.Count)
						{
							if (popInsertFieldValue(ref sSQL, out s))
							{
								fields[i].FieldText = s.Trim();
								i++;
								sSQL = sSQL.Trim();
								if (sSQL.Length == 0)
								{
									sMsg = "Incomplete VALUES part.";
									return false;
								}
								if (sSQL[0] == ')')
								{
									sSQL = sSQL.Substring(1);
									break;
								}
								else if (sSQL[0] == ',')
								{
									sSQL = sSQL.Substring(1);
								}
							}
							else
							{
								sMsg = "Invalid VALUES part";
								return false;
							}
						}
						if (i != fields.Count)
						{
							sMsg = "Field count and value count mismatch";
							return false;
						}
						//
						sSQL = sSQL.Trim();
						if (sSQL.Length > 0)
						{
							if (string.CompareOrdinal(sSQL, ";") == 0)
							{
								sSQL = string.Empty;
							}
							else
							{
								sMsg = "Invalid statement:" + sSQL;
								return false;
							}
						}
					}
					else //multi-rows
					{
						if (sSQL.StartsWith(QueryParser.SQL_Select(), StringComparison.OrdinalIgnoreCase))
						{
							qp.parameters = this._parameters;
							qry = qp.ParseQuery(caller, this.connect.ConnectionObject, sSQL, out sMsg);
							if (qry == null || sMsg.Length > 0)
							{
								return false;
							}
							FieldList fl = qry.Fields;
							if (fields.Count > fl.Count)
							{
								sMsg = "Not enough Query fields for the new records.";
								return false;
							}
							_parameters = qp.parameters;
							for (i = 0; i < fields.Count; i++)
								fields[i].FieldText = fl[i].FieldText;
							StringBuilder f = new StringBuilder();
							//Filter = "";
							if (qry.From.Length > 0)
							{
								f.Append(QueryParser.SQL_From());
								f.Append(qry.From);
							}
							if (qry.Where.Length > 0)
							{
								f.Append(QueryParser.SQL_Where());
								f.Append(qry.Where);
							}
							_filter = f.ToString();
						}
						else
						{
							sMsg = "Missing SELECT";
							return false;
						}
					}//MultiRow/Single row
				}
				////////////////////////////////////////////////////////////
				if (cmdType == enmNonQueryType.Update)
				{
					i = finTabledNameEnd(sSQL); //end of table name pos
					if (i < 0)
					{
						sMsg = "Table name not specified";
						return false;
					}
					s = sSQL.Substring(0, i + 1);
					TableName = getname(s);
					if (i >= sSQL.Length - 1)
						sSQL = "";
					else
						sSQL = sSQL.Substring(i + 1);
					sSQL = sSQL.Trim();
					if (sSQL.Length < 7)
					{
						sMsg = "Incomplete SQL statement";
						return false;
					}
					int nPos = sSQL.IndexOf("SET ", StringComparison.OrdinalIgnoreCase);
					if (nPos == 0)
					{
						sSQL = sSQL.Substring(4);
						sSQL = sSQL.Trim();
					}
					else
					{
						nPos = sSQL.IndexOf(" SET ", StringComparison.OrdinalIgnoreCase);
						if (nPos >= 0)
						{
							sSQL = sSQL.Substring(nPos + 5);
							sSQL = sSQL.Trim();
						}
						else
						{
							sSQL = "Missing SET";
							return false;
						}
					}
					if (sSQL.Length == 0)
					{
						sMsg = "Incomplete SET statement";
						return false;
					}
					fields = new FieldList();
					i = 0;
					while (true)
					{
						s = popUpdateFieldName(ref sSQL);
						if (s.Length > 0)
						{
							s = getname(s);
							if (s.Length == 0)
							{
								sMsg = "Field name missing";
								return false;
							}
							fields.AddField(new EPField(i, s));
							s = popUpdateFieldValue(ref sSQL);
							if (s.Length == 0)
							{
								sMsg = "Field value missing";
								return false;
							}
							fields[i].FieldText = s.Trim();
							sSQL = sSQL.Trim();
							if (sSQL.Length == 0)
								break;
							if (sSQL[0] != ',')
								break;
							sSQL = sSQL.Substring(1);
							sSQL = sSQL.Trim();
							//FROM / WHERE 
							if (sSQL.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
								break;
							if (sSQL.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
								break;

							i++;
						}
						else
							break;
					}
					SetFields(fields);
					_filter = sSQL;
				}
			}
			return true;
		}
		#endregion

		#region Parameters Serialization
		private void setParameterCount(int len)
		{
			if (_parameters == null)
			{
				_parameters = new ParameterList();
			}
			int n = _parameters.Count;
			for (int i = n; i < len; i++)
			{
				_parameters.Add(new EPField());
			}
			while (_parameters.Count > len)
			{
				_parameters.RemoveAt(_parameters.Count - 1);
			}
		}
		[Browsable(false)]
		public string[] Param_Name
		{
			get
			{
				string[] ss = new string[ParamCount];
				for (int i = 0; i < ss.Length; i++)
				{
					ss[i] = _parameters[i].Name;
				}
				return ss;
			}
			set
			{
				if (value == null)
				{
					_parameters = null;
				}
				else
				{
					setParameterCount(value.Length);
					for (int i = 0; i < value.Length; i++)
					{
						_parameters[i].Name = value[i];
					}
				}
			}
		}
		[Browsable(false)]
		public OleDbType[] Param_OleDbType
		{
			get
			{
				OleDbType[] ss = new OleDbType[ParamCount];
				for (int i = 0; i < ss.Length; i++)
				{
					ss[i] = _parameters[i].OleDbType;
				}
				return ss;
			}
			set
			{
				if (value == null)
				{
					_parameters = null;
				}
				else
				{
					setParameterCount(value.Length);
					for (int i = 0; i < value.Length; i++)
					{
						_parameters[i].OleDbType = value[i];
					}
				}
			}
		}

		[Browsable(false)]
		public string[] Param_Value
		{
			get
			{
				string[] ss = new string[ParamCount];
				for (int i = 0; i < ss.Length; i++)
				{
					if (_parameters[i].Value != null && _parameters[i].Value != DBNull.Value)
					{
						TypeConverter tc = TypeDescriptor.GetConverter(_parameters[i].Value);
						ss[i] = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}|{1}", _parameters[i].Value.GetType().AssemblyQualifiedName, tc.ConvertToInvariantString(_parameters[i].Value));
					}
					else
					{
						ss[i] = string.Empty;
					}
				}
				return ss;
			}
			set
			{
				if (value == null)
				{
					_parameters = null;
				}
				else
				{
					setParameterCount(value.Length);
					for (int i = 0; i < value.Length; i++)
					{
						if (string.IsNullOrEmpty(value[i]))
						{
							_parameters[i].SetValue(null);
						}
						else
						{
							string[] ss = value[i].Split('|');
							if (ss.Length == 2)
							{
								Type t = Type.GetType(ss[0]);
								if (t != null)
								{
									if (typeof(byte[]).Equals(t) && string.CompareOrdinal("Byte[] Array", ss[1]) == 0)
									{
										_parameters[i].SetValue(null);
									}
									else
									{
										TypeConverter tc = TypeDescriptor.GetConverter(t);
										if (tc != null)
										{
											_parameters[i].SetValue(tc.ConvertFromInvariantString(ss[1]));
										}
									}
								}
							}
							else
							{
								_parameters[i].SetValue(null);
							}
						}
					}
				}
			}
		}

		#endregion

		#region ICloneable Members
		public object Clone()
		{
			FieldList fields0 = null;
			ParameterList parameters0 = null;
			if (fields != null)
				fields0 = (FieldList)fields.Clone();
			if (_parameters != null)
				parameters0 = (ParameterList)_parameters.Clone();
			SQLNoneQuery obj = new SQLNoneQuery(sTableName, fields0, parameters0);
			obj.MultiRow = MultiRow;
			obj.cmdType = cmdType;
			obj._filter = Filter;
			if (qry != null)
			{
				obj.qry = (EasyQuery)qry.Clone();
			}
			if (connect != null)
			{
				obj.connect = (ConnectionItem)connect.Clone();
			}
			obj.SQL = _rawSQL;
			return obj;
		}

		#endregion

		#region private methods
		private string getname(string name)
		{
			if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				int n = sepStart.Length + sepEnd.Length;
				if (name.Length > n)
				{
					if (name.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase) &&
						name.EndsWith(sepEnd, StringComparison.OrdinalIgnoreCase))
					{
						return name.Substring(sepStart.Length, name.Length - n).Trim();
					}
				}
			}
			return name;
		}
		/// <summary>
		/// table name ends with a space
		/// </summary>
		/// <param name="sSQL"></param>
		/// <returns></returns>
		private int finTabledNameEnd(string sSQL)
		{
			int i;
			if (sSQL == null)
				return -1;
			if (sSQL.Length == 0)
				return -1;
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				if (sSQL.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase))
				{
					int n = sSQL.IndexOf(sepEnd, StringComparison.OrdinalIgnoreCase);
					if (n > 0)
					{
						return n + sepEnd.Length - 1;
					}
				}
			}
			i = sSQL.IndexOf(' ');
			if (i < 0)
				i = sSQL.Length - 1;
			else
				i--;
			return i;
		}
		private string popFieldName(ref string sSQL)
		{
			string sRet = "";
			if (sSQL.Length > 0)
			{
				int i;
				if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd) && sSQL.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase))
				{
					i = sSQL.IndexOf(sepEnd, sepStart.Length, StringComparison.OrdinalIgnoreCase);
					if (i > 0)
					{
						sRet = sSQL.Substring(0, i + sepEnd.Length); //sRet is the name
						//pop sSQL
						if (i >= sSQL.Length - sepEnd.Length)
							sSQL = "";
						else
							sSQL = sSQL.Substring(i + sepEnd.Length);
						sSQL = sSQL.Trim();
						//remove comma
						if (sSQL.Length > 0)
						{
							if (sSQL[0] == ',')
							{
								if (sSQL.Length == 1)
									sSQL = "";
								else
									sSQL = sSQL.Substring(1);
							}
						}
					}
				}
				else
				{
					int i2 = sSQL.IndexOf(')');
					i = sSQL.IndexOf(',');
					if (i <= 0)
						i = i2;
					else if (i2 > 0 && i2 < i)
						i = i2;
					if (i >= 0)
					{
						if (i > 0)
						{
							sRet = sSQL.Substring(0, i);
						}
						else
						{
							sRet = "";
						}
						if (i >= sSQL.Length - 1)
							sSQL = "";
						else
						{
							if (sSQL[i] == ')')
								sSQL = sSQL.Substring(i);
							else
								sSQL = sSQL.Substring(i + 1);
						}
					}
				}
			}
			sSQL = sSQL.Trim();
			return sRet;
		}
		/// <summary>
		/// value 1,value2,...,valuen<n> )
		/// </summary>
		/// <param name="sSQL"></param>
		/// <param name="sRet"></param>
		/// <returns></returns>
		private bool popInsertFieldValue(ref string sSQL, out string sRet)
		{
			sRet = "";
			sSQL = sSQL.Trim();
			if (sSQL.Length == 0)
				return false;
			if (sSQL[0] == ',' || sSQL[0] == ')')
				return false;
			int i = 0;
			bool Quote = false;
			int bracket = 0; //()
			while (i < sSQL.Length)
			{
				if (Quote)
				{
					if (sSQL[i] == '\'')
					{
						if (i >= sSQL.Length - 1)
						{
							Quote = false;
							break;
						}
						if (sSQL[i + 1] != '\'')
						{
							Quote = false;
						}
						else
						{
							i++;
						}
					}
				}
				else if (bracket > 0)
				{
					if (sSQL[i] == ')')
					{
						bracket--;
					}
					else if (sSQL[i] == '\'')
					{
						Quote = true;
					}
					else if (sSQL[i] == '(')
					{
						bracket++;
					}
				}
				else
				{
					if (sSQL[i] == '\'')
					{
						Quote = true;
					}
					else
					{
						if (sSQL[i] == '(')
						{
							bracket++;
						}
						else if (sSQL[i] == ',')
							break;
						else if (sSQL[i] == ')')
							break;
					}
				}
				i++;
			}
			if (Quote)
				return false;
			sRet = sSQL.Substring(0, i);
			sSQL = sSQL.Substring(i); //keep the terminating character
			sSQL = sSQL.Trim();
			return true;
		}
		private string popUpdateFieldName(ref string sSQL)
		{
			string sRet = "";
			int i = sSQL.IndexOf('=');
			if (i > 0)
			{
				sRet = sSQL.Substring(0, i);
				sSQL = sSQL.Substring(i + 1);
				sRet = sRet.Trim();
				sSQL = sSQL.Trim();
			}
			return sRet;
		}
		private string popUpdateFieldValue(ref string sSQL)
		{
			string sRet = "";
			sSQL = sSQL.Trim();
			int i = 0;
			//char cQ0 = '\0';
			char cQuote = '\0';
			bool Quote = false;
			Stack<char> stk = new Stack<char>();

			while (i < sSQL.Length)
			{
				if (Quote)
				{
					if (sSQL[i] == cQuote)
					{
						if (i >= sSQL.Length - 1)
						{
							Quote = false;
							break;
						}
						if (cQuote == '\'')
						{
							if (sSQL[i + 1] != '\'')
							{
								if (stk.Count == 0)
								{
									Quote = false;
								}
								else
								{
									cQuote = stk.Pop();
								}
							}
							else
							{
								i++;
							}
						}
						else
						{
							if (stk.Count == 0)
							{
								Quote = false;
							}
							else
							{
								cQuote = stk.Pop();
							}
						}
					}
					else if (sSQL[i] == '\'')
					{
						stk.Push(cQuote);
						cQuote = '\'';
					}
					else if (sSQL[i] == '(')
					{
						stk.Push(cQuote);
						cQuote = ')';
					}
					else if (sSQL[i] == '[')
					{
						stk.Push(cQuote);
						cQuote = ']';
					}
				}
				else
				{
					if (sSQL[i] == '\'')
					{
						cQuote = '\'';
						Quote = true;
					}
					else if (sSQL[i] == '(')
					{
						cQuote = ')';
						Quote = true;
					}
					else if (sSQL[i] == '[')
					{
						cQuote = ']';
						Quote = true;
					}
					else
					{
						if (sSQL[i] == ',')
							break;
						else
						{
							if (sSQL.IndexOf("FROM ", i, StringComparison.OrdinalIgnoreCase) == i)
								break;
							else if (sSQL.IndexOf("WHERE ", i, StringComparison.OrdinalIgnoreCase) == i)
								break;
						}
					}
				}
				i++;
			}
			if (Quote)
			{
				sRet = "";
			}
			else
			{
				if (i >= sSQL.Length - 1)
				{
					sRet = sSQL;
					sSQL = "";
				}
				else
				{
					sRet = sSQL.Substring(0, i);
					sSQL = sSQL.Substring(i);
				}
				sSQL = sSQL.Trim();
				sRet = sRet.Trim();
			}
			return sRet;
		}
		#endregion
	}
}
