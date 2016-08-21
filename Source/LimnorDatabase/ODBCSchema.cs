/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Data.Odbc;
using System.Data.Common;
using System.Collections.Specialized;
using System.Data;
using System.Collections.Generic;
using VPL;
using System.Data.OleDb;
using System.Windows.Forms;
using System.IO;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for ODBCSchema.
	/// </summary>
	public class ODBCSchema
	{
		//
		private bool bOpened = false;
		private Type _connectionType = typeof(OdbcConnection);
		private DbConnection _connection;
		//
		private string[] _tableNames;
		private string[] _viewNames;
		private Dictionary<string, FieldList> _columns;
		private string _schema;
		//
		public ODBCSchema()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public bool Opened
		{
			get
			{
				return bOpened;
			}
		}
		#region Private Database API
		private bool OpenDatabase(string sSDN)
		{
			try
			{
				_schema = string.Empty;
				_tableNames = null;
				_columns = null;
				_viewNames = null;
				_connection = Activator.CreateInstance(_connectionType) as DbConnection;
				if (_connection != null)
				{
					_connection.ConnectionString = sSDN;
					_connection.Open();
					_connection.Close();
				}
			}
			catch
			{
				_connection = null;
				throw;
			}
			finally
			{
				CloseDatabase();
			}
			return (_connection != null);
		}
		private void CloseDatabase()
		{
			if (_connection != null)
			{
				if (_connection.State != System.Data.ConnectionState.Closed)
				{
					_connection.Close();
				}
			}
		}
		private string Schema
		{
			get
			{
				if (string.IsNullOrEmpty(_schema))
				{
					if (bOpened)
					{
						DbCommand cmd = _connection.CreateCommand();
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.CommandText = "select DATABASE();";
						try
						{
							if (_connection.State == System.Data.ConnectionState.Closed)
							{
								_connection.Open();
							}
							object v = cmd.ExecuteScalar();
							_schema = VPL.VPLUtil.ObjectToString(v);
						}
						catch
						{
							_schema = string.Empty;
						}
						finally
						{
							if (_connection.State != System.Data.ConnectionState.Closed)
							{
								_connection.Close();
							}
						}
					}
				}
				return _schema;
			}
		}
		#endregion
		#region Public Methods
		public bool Open(string sSDN)
		{
			if (bOpened)
				return true;
			bOpened = OpenDatabase(sSDN);
			return bOpened;
		}
		public void Close()
		{
			if (Opened)
			{
				CloseDatabase();
				bOpened = false;
			}
		}
		private string[] getTableNamesBySchema()
		{
			try
			{
				if (_connection.State == System.Data.ConnectionState.Closed)
				{
					_connection.Open();
				}
				DataTable schema = _connection.GetSchema(OdbcMetaDataCollectionNames.Tables);
				List<string> l = new List<string>();
				for (int i = 0; i < schema.Rows.Count; i++)
				{
					l.Add(Convert.ToString(schema.Rows[i]["TABLE_NAME"]));
				}
				_tableNames = l.ToArray();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
			finally
			{
				_connection.Close();
			}
			return _tableNames;
		}
		public string[] TableNames()
		{
			if (_tableNames == null)
			{
				if (bOpened)
				{
					DbDataReader dr = null;
					string sSQL;
					sSQL = "select table_name from information_schema.tables where table_schema='{0}' and table_type <> 'VIEW' ";
					if (_tableNames == null)
					{
						DbCommand cmd = _connection.CreateCommand();
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.CommandText = string.Format(System.Globalization.CultureInfo.InvariantCulture, sSQL, Schema);
						try
						{
							if (_connection.State == System.Data.ConnectionState.Closed)
							{
								_connection.Open();
							}
							dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
							StringCollection sc = new StringCollection();
							while (dr.Read())
							{
								sc.Add(VPL.VPLUtil.ObjectToString(dr[0]));
							}
							_tableNames = new string[sc.Count];
							sc.CopyTo(_tableNames, 0);
						}
						catch
						{
						}
						finally
						{
							if (dr != null)
							{
								dr.Close();
								dr = null;
							}
							if (_connection.State != System.Data.ConnectionState.Closed)
							{
								_connection.Close();
							}
						}
						if (_tableNames == null)
						{
							getTableNamesBySchema();
							if (_tableNames == null)
							{
								DialogTableNames dlg = new DialogTableNames();
								dlg.LoadData(_connection);
								if (dlg.ShowDialog() == DialogResult.OK)
								{
									_tableNames = dlg.TableNames;
								}
							}
						}
					}
				}
			}
			return _tableNames;
		}
		public string[] ViewNames()
		{
			if (_viewNames == null)
			{
				if (bOpened)
				{
					DbDataReader dr = null;
					string sSQL = "select table_name from information_schema.tables where table_schema='{0}' and table_type='VIEW' ";

					DbCommand cmd = _connection.CreateCommand();
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = string.Format(System.Globalization.CultureInfo.InvariantCulture, sSQL, Schema);
					try
					{
						if (_connection.State == System.Data.ConnectionState.Closed)
						{
							_connection.Open();
						}
						dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
						StringCollection sc = new StringCollection();
						while (dr.Read())
						{
							sc.Add(VPL.VPLUtil.ObjectToString(dr[0]));
						}
						_viewNames = new string[sc.Count];
						sc.CopyTo(_viewNames, 0);
					}
					finally
					{
						if (dr != null)
						{
							dr.Close();
							dr = null;
						}
						if (_connection.State != System.Data.ConnectionState.Closed)
						{
							_connection.Close();
						}
					}
				}
			}
			return _viewNames;
		}
		public FieldList GetFields(string tbl)
		{
			FieldList flds = null;
			if (_columns == null)
			{
				_columns = new Dictionary<string, FieldList>();
			}
			if (!_columns.TryGetValue(tbl, out flds))
			{
				if (bOpened)
				{
					DbDataReader dr = null;

					string sSQL;
					sSQL = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,COLUMN_KEY,COLUMN_TYPE, EXTRA from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}' and TABLE_SCHEMA = '{1}'  order by ORDINAL_POSITION", tbl, Schema);

					DbCommand cmd = _connection.CreateCommand();
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = sSQL;
					try
					{
						if (_connection.State == System.Data.ConnectionState.Closed)
						{
							_connection.Open();
						}
						dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
						flds = new FieldList();
						int n = 0;
						while (dr.Read())
						{
							EPField fld = new EPField();
							fld.Name = (string)dr[0];
							fld.Indexed = (string.CompareOrdinal("PRI", VPLUtil.ObjectToString(dr[3])) == 0);
							fld.Index = n;
							fld.FromTableName = tbl;
							bool bUnsigned = (VPLUtil.ObjectToString(dr[4]).IndexOf("unsigned", StringComparison.OrdinalIgnoreCase) >= 0);
							fld.OleDbType = stringToOleDbType(VPLUtil.ObjectToString(dr[1]), bUnsigned);

							if (dr[2] != null && dr[2] != DBNull.Value)
							{
								fld.DataSize = VPLUtil.ObjectToInt(dr[2]);
							}
							fld.IsIdentity = (string.Compare("auto_increment", VPLUtil.ObjectToString(dr[5]), StringComparison.OrdinalIgnoreCase) == 0);
							flds.Add(fld);
							n++;
						}
						_columns.Add(tbl, flds);
					}
					finally
					{
						if (dr != null)
						{
							dr.Close();
							dr = null;
						}
						if (_connection.State != System.Data.ConnectionState.Closed)
						{
							_connection.Close();
						}
					}
				}
			}
			if (flds == null)
			{
				flds = new FieldList();
			}
			return flds;
		}
		#endregion
		#region private utility
		private static OleDbType stringToOleDbType(string mySqlType, bool isUnsigned)
		{
			if (string.Compare(mySqlType, "INTEGER", StringComparison.OrdinalIgnoreCase) == 0
			|| string.Compare(mySqlType, "INT", StringComparison.OrdinalIgnoreCase) == 0
			|| string.Compare(mySqlType, "MEDIUMINT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (isUnsigned)
				{
					return OleDbType.UnsignedInt;
				}
				else
				{
					return OleDbType.Integer;
				}
			}
			if (string.Compare(mySqlType, "SMALLINT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (isUnsigned)
				{
					return OleDbType.UnsignedSmallInt;
				}
				else
				{
					return OleDbType.SmallInt;
				}
			}
			if (string.Compare(mySqlType, "TINYINT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (isUnsigned)
				{
					return OleDbType.UnsignedTinyInt;
				}
				else
				{
					return OleDbType.TinyInt;
				}
			}
			if (string.Compare(mySqlType, "BIGINT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (isUnsigned)
				{
					return OleDbType.UnsignedBigInt;
				}
				else
				{
					return OleDbType.BigInt;
				}
			}
			if (string.Compare(mySqlType, "DOUBLE", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Double;
			}
			if (string.Compare(mySqlType, "DECIMAL", StringComparison.OrdinalIgnoreCase) == 0
			|| string.Compare(mySqlType, "DEC", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Double;
			}
			if (string.Compare(mySqlType, "NUMERIC", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Numeric;
			}
			if (string.Compare(mySqlType, "FLOAT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Single;
			}
			if (string.Compare(mySqlType, "REAL", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Single;
			}
			if (string.Compare(mySqlType, "DATETIME", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.DBTimeStamp;
			}
			if (string.Compare(mySqlType, "DATE", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.DBDate;
			}
			if (string.Compare(mySqlType, "TIME", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.DBTime;
			}
			if (string.Compare(mySqlType, "TIMESTAMP", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.DBTimeStamp;
			}
			if (string.Compare(mySqlType, "YEAR", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.TinyInt;
			}
			if (string.Compare(mySqlType, "CHAR", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Char;
			}
			if (string.Compare(mySqlType, "VARCHAR", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.VarChar;
			}
			if (string.Compare(mySqlType, "BINARY", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.Binary;
			}
			if (string.Compare(mySqlType, "VARBINARY", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.VarBinary;
			}
			if (string.Compare(mySqlType, "BLOB", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.VarBinary;
			}
			if (string.Compare(mySqlType, "TEXT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.LongVarWChar;
			}
			if (string.Compare(mySqlType, "ENUM", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.VarChar;
			}
			if (string.Compare(mySqlType, "SET", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return OleDbType.VarChar;
			}
			return OleDbType.VarWChar;
		}

		private static OleDbType OdbcType2OleDbType(int tp)
		{
			switch (tp)
			{
				case -7:
					return System.Data.OleDb.OleDbType.Boolean;
				case -6:
					return System.Data.OleDb.OleDbType.TinyInt;
				case -1:
					return System.Data.OleDb.OleDbType.LongVarChar;
				case 2:
					return System.Data.OleDb.OleDbType.Decimal;
				case 4:
					return System.Data.OleDb.OleDbType.Integer;
				case 5:
					return System.Data.OleDb.OleDbType.SmallInt;
				case 7:
					return System.Data.OleDb.OleDbType.Single;
				case 8:
					return System.Data.OleDb.OleDbType.Double;
				case 11:
					return System.Data.OleDb.OleDbType.DBTimeStamp;
				case 12:
					return System.Data.OleDb.OleDbType.VarWChar;
				default:
					return System.Data.OleDb.OleDbType.VarWChar;
			}
		}
		private static OleDbType OdbcType2OleDbType(OdbcType odbcType)
		{
			switch (odbcType)
			{
				case System.Data.Odbc.OdbcType.BigInt:
					return System.Data.OleDb.OleDbType.BigInt;
				case System.Data.Odbc.OdbcType.Binary:
					return System.Data.OleDb.OleDbType.Binary;
				case System.Data.Odbc.OdbcType.Bit:
					return System.Data.OleDb.OleDbType.Boolean;
				case System.Data.Odbc.OdbcType.NVarChar:
					return System.Data.OleDb.OleDbType.BSTR;
				case System.Data.Odbc.OdbcType.Char:
					return System.Data.OleDb.OleDbType.Char;
				case System.Data.Odbc.OdbcType.Decimal:
					return System.Data.OleDb.OleDbType.Currency;
				case System.Data.Odbc.OdbcType.Date:
					return System.Data.OleDb.OleDbType.Date;
				case System.Data.Odbc.OdbcType.DateTime:
					return System.Data.OleDb.OleDbType.DBTimeStamp;
				case System.Data.Odbc.OdbcType.Timestamp:
					return System.Data.OleDb.OleDbType.DBTimeStamp;
				case System.Data.Odbc.OdbcType.Double:
					return System.Data.OleDb.OleDbType.Double;
				case System.Data.Odbc.OdbcType.Int:
					return System.Data.OleDb.OleDbType.Integer;
				case System.Data.Odbc.OdbcType.VarBinary:
					return System.Data.OleDb.OleDbType.LongVarBinary;
				case System.Data.Odbc.OdbcType.Text:
					return System.Data.OleDb.OleDbType.LongVarChar;
				case System.Data.Odbc.OdbcType.NText:
					return System.Data.OleDb.OleDbType.LongVarWChar;
				case System.Data.Odbc.OdbcType.Numeric:
					return System.Data.OleDb.OleDbType.Numeric;
				case System.Data.Odbc.OdbcType.Real:
					return System.Data.OleDb.OleDbType.Single;
				case System.Data.Odbc.OdbcType.SmallInt:
					return System.Data.OleDb.OleDbType.SmallInt;
				case System.Data.Odbc.OdbcType.TinyInt:
					return System.Data.OleDb.OleDbType.TinyInt;
				case System.Data.Odbc.OdbcType.VarChar:
					return System.Data.OleDb.OleDbType.VarChar;
				case System.Data.Odbc.OdbcType.NChar:
					return System.Data.OleDb.OleDbType.WChar;
				default:
					return System.Data.OleDb.OleDbType.VarWChar;
			}
		}
		private static string popPipe(ref string s)
		{
			if (s.Length == 0)
				return "";
			string sRet;
			int n = s.IndexOf('|');
			if (n < 0)
			{
				sRet = s;
				s = "";
			}
			else if (n == 0)
			{
				sRet = "";
				s = s.Substring(1);
			}
			else if (n == s.Length - 1)
			{
				sRet = s.Substring(0, n);
				s = "";
			}
			else
			{
				sRet = s.Substring(0, n);
				s = s.Substring(n + 1);
			}
			return sRet;
		}
		#endregion
	}
}
