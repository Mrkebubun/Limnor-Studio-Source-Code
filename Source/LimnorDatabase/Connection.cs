/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Security.Cryptography;
using WindowsUtility;
using System.Globalization;
using System.Reflection;
using System.IO;
using VPL;

/// schema names for DbConnection:
/// MetaDataCollections;
/*DataSourceInformation;
DataTypes;
Restrictions;
ReservedWords;
Databases;
Tables;
Columns;
Users;
Foreign Keys;
IndexColumns;
Indexes;
Foreign Key Columns;
UDF;
Views;
ViewColumns;
Procedure Parameters;
Procedures;
Triggers*/

namespace LimnorDatabase
{
	/// <summary>
	/// a database connection.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[ToolboxBitmapAttribute(typeof(Connection), "Resources.connect.bmp")]
	public class Connection : DbConnection, ICloneable
	{
		#region fields and constructors
		private Guid _guid = Guid.Empty;
		private DbConnection _connection;
		private Type _type;
		private string _connectionString;
		private DbCredential _credential;
		private EnumQulifierDelimiter _delimiterType;
		private EnumParameterStyle _paramStyle = EnumParameterStyle.QuestionMark;
		private string _error;
		private bool _encryptStr;
		private ODBCSchema odbcSchema;
		internal const string MySqlConnectionTypeName = "MySql.Data.MySqlClient.MySqlConnection";
		public Connection()
		{
			SavePasswordInConnectionString = true;
			LowerCaseSqlKeywords = false;
		}
		#endregion

		#region Credential Management
		internal class DbCredential
		{
			public string User { get; set; }
			public string Password { get; set; }
			public string DatabasePassword { get; set; }
			public DbCredential(string user, string password, string databasePassword)
			{
				User = user;
				Password = password;
				DatabasePassword = databasePassword;
			}
		}
		private static Dictionary<Guid, DbCredential> _credentials;
		internal static void AddCredential(Guid id, DbCredential credential)
		{
			if (_credentials == null)
			{
				_credentials = new Dictionary<Guid, DbCredential>();
			}
			if (_credentials.ContainsKey(id))
			{
				_credentials[id] = credential;
			}
			else
			{
				_credentials.Add(id, credential);
			}
		}
		internal static DbCredential GetCredential(Guid id)
		{
			if (_credentials != null)
			{
				if (_credentials.ContainsKey(id))
				{
					return _credentials[id];
				}
			}
			return null;
		}
		#endregion

		#region private methods
		private string get_errormessage(string message)
		{
			try
			{
				TripleDESCryptoServiceProvider p = new TripleDESCryptoServiceProvider();
				MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
				byte[] bs = Convert.FromBase64String(message);
				p.Key = m5.ComputeHash(UnicodeEncoding.Unicode.GetBytes(typeof(ConnectionItem).Name));
				p.Mode = CipherMode.ECB;
				message = UnicodeEncoding.Unicode.GetString(p.CreateDecryptor().TransformFinalBlock(bs, 0, bs.Length));
				p = null;
				m5 = null;
				_encryptStr = true;
				return message;
			}
			catch (Exception err)
			{
				return err.Message;
			}
		}
		private bool isFireDird()
		{
			if (_connection != null)
			{
				string s = _connection.GetType().AssemblyQualifiedName;
				if (!string.IsNullOrEmpty(s))
				{
					return s.Contains("FirebirdSql.Data.FirebirdClient");
				}
			}
			return false;
		}
		private void set_readonly()
		{
			if (_connection != null)
			{
				if (_connection.State != ConnectionState.Closed)
				{
					_connection.Close();
				}
				int n = 0x1F;
				n--;
				if (string.IsNullOrEmpty(_connectionString) || string.CompareOrdinal(_connectionString, new string((char)(n + 3), 1)) == 0)
				{
					_connectionString = ConnectionConfig.GetConnectionString(_guid);
				}
				if (!string.IsNullOrEmpty(_connectionString))
				{
					_connectionString = _connectionString.Trim();
					if (_connectionString.Length > 0)
					{
						DbCredential dbc = Connection.GetCredential(_guid);
						if (dbc != null)
						{
							_credential = dbc;
						}
						if (_connectionString.StartsWith(new string((char)(n + 3), 1), StringComparison.Ordinal))
						{
							_connectionString = get_errormessage(_connectionString.Substring(1));
						}
						int nStep = 0;
						try
						{
							if (_connection is OleDbConnection)
							{
								if (_connectionString.IndexOf("Jet.OLEDB") >= 0)
								{
									//check file exist
									if (_credential != null)
									{
										nStep = 1;
										_connection.ConnectionString = ConnectionStringSelector.SetCredentialToAccessConnectionString(_connectionString, _credential.User, _credential.Password, _credential.DatabasePassword);
									}
									else
									{
										nStep = 2;
										_connection.ConnectionString = ConnectionStringSelector.SetCredentialToAccessConnectionString(_connectionString, null, null, null);
									}
								}
								else
								{
									nStep = 3;
									_connection.ConnectionString = _connectionString;
								}
							}
							else if (_connection is SqlConnection)
							{
								if (ConnectionStringSelector.IsTrustedSQLServerConnection(_connectionString))
								{
									nStep = 4;
									_connection.ConnectionString = _connectionString;
								}
								else
								{
									if (_credential != null)
									{
										nStep = 5;
										_connection.ConnectionString = ConnectionStringSelector.SetCredentialToMsSqlServerConnectionString(_connectionString, _credential.User, _credential.Password);
									}
									else
									{
										nStep = 6;
										_connection.ConnectionString = _connectionString;
									}
								}
							}
							else if (isFireDird())
							{
								//check file exist
								if (_credential != null)
								{
									nStep = 7;
									_connection.ConnectionString = ConnectionStringSelector.SetCredentialToFirebirdConnectionString(_connectionString, _credential.User, _credential.Password);
								}
								else
								{
									nStep = 8;
									_connection.ConnectionString = ConnectionStringSelector.SetCredentialToFirebirdConnectionString(_connectionString, null, null);
								}
							}
							else
							{
								nStep = 9;
								_connection.ConnectionString = _connectionString;
							}
						}
						catch (Exception err)
						{
							FormLog.NotifyException(true,err, "Error setting connection string [{0}] for connection [{1}] at step {2}", _connectionString, this.ConnectionGuid, nStep);
						}
					}
				}
			}
		}
		/// <summary>
		/// test a connection and report by MessageBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void testConnection(object sender, EventArgs e)
		{
			Connection cnn = sender as Connection;
			if (cnn != null)
			{
				cnn.TestConnection(null, true);
			}
			else
			{
				if (sender == null)
				{
					MessageBox.Show("Connection is null", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					MessageBox.Show(sender.GetType().Name + " is not a connection", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		#endregion

		#region Methods
		public void SetDataFolder()
		{
			if (!string.IsNullOrEmpty(_connectionString))
			{
				_connectionString = _connectionString.Replace(Filepath.FOLD_CPMMPMAPPDATA, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
			}
		}
		public void SetConnectionString(string str)
		{
			_connectionString = str;
		}
		public DbConnection GetConnection()
		{
			set_readonly();
			return _connection;
		}
		public bool SetCredentialByUI(Form caller, string owner)
		{
			DlgSetCredential dlg = new DlgSetCredential();
			dlg.LoadData(owner, _credential);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				_credential = dlg.dbc;
				Connection.AddCredential(_guid, _credential);
				return true;
			}
			return false;
		}
		public bool TestConnection(Form caller, bool showResult)
		{
			bool bOK = false;
			try
			{
				if (string.IsNullOrEmpty(ConnectionString))
				{
					if (showResult)
					{
						MessageBox.Show(caller, "Connection string is empty", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
				else
				{
					Open();
					bOK = (State != ConnectionState.Closed);
					Close();
					if (bOK)
					{
						if (showResult)
						{
							MessageBox.Show(caller, "OK", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
					else
					{
						if (showResult)
						{
							MessageBox.Show(caller, "Connection failed", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
				}
			}
			catch (Exception err)
			{
				if (showResult)
				{
					MessageBox.Show(caller, err.Message, "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			finally
			{
				if (State != ConnectionState.Closed)
				{
					Close();
				}
			}
			return bOK;
		}
		public DlgConnection CreateDataDialog()
		{
			DlgConnection dlg = new DlgConnection();
			dlg.LoadData(this);
			dlg.AddRefreshName("DatabaseType");
			dlg.Text = Resource1.dbConnection;
			dlg.SetTest(testConnection);
			return dlg;
		}
		public void SetCredential(string user, string userPassword, string databasePassword)
		{
			_credential = new DbCredential(user, userPassword, databasePassword);
			Connection.AddCredential(_guid, _credential);
			set_readonly();
		}
		public void SetCredential0(string user, string userPassword, string databasePassword)
		{
			_credential = new DbCredential(user, userPassword, databasePassword);
			Connection.AddCredential(_guid, _credential);
		}
		/// <summary>
		/// execute a non-query
		/// </summary>
		/// <param name="nonQuery">non-query SQL script</param>
		public void ExecuteNonQuery(string nonQuery)
		{
			if (_connection == null)
			{
				throw new ExceptionLimnorDatabase("Database connection not prepared");
			}
			if (string.IsNullOrEmpty(_connection.ConnectionString))
			{
				throw new ExceptionLimnorDatabase("Database connection string is empty");
			}
			try
			{
				DbCommand cmd = _connection.CreateCommand();
				cmd.CommandText = nonQuery;
				_connection.Open();
				cmd.ExecuteNonQuery();
				_connection.Close();
			}
			catch
			{
				throw;
			}
			finally
			{
				if (_connection.State != ConnectionState.Closed)
				{
					_connection.Close();
				}
			}
		}
		/// <summary>
		/// drop a table
		/// </summary>
		/// <param name="tableName">table name without delimeters</param>
		public void DropTable(string tableName)
		{
			StringBuilder sSQL = new StringBuilder("Drop table ");
			sSQL.Append(NameDelimiterBegin);
			sSQL.Append(tableName);
			sSQL.Append(NameDelimiterEnd);
			ExecuteNonQuery(sSQL.ToString());
		}
		public void CreateTable(string tableName, string autoNumberName)
		{
			StringBuilder sSQL = new StringBuilder();
			if (IsOdbc)
			{
				sSQL.Append("CREATE TABLE ");
				sSQL.Append(NameDelimiterBegin);
				sSQL.Append(tableName);
				sSQL.Append(NameDelimiterEnd);
				if (!string.IsNullOrEmpty(autoNumberName))
				{
					sSQL.Append(" (");
					sSQL.Append(NameDelimiterBegin);
					sSQL.Append(autoNumberName);
					sSQL.Append(NameDelimiterEnd);
					sSQL.Append(" INTEGER AUTO_INCREMENT PRIMARY KEY);");
				}
			}
			else
			{
				if (IsMySql)
				{
					sSQL.Append("CREATE TABLE ");
					sSQL.Append(NameDelimiterBegin);
					sSQL.Append(tableName);
					sSQL.Append(NameDelimiterEnd);
					if (!string.IsNullOrEmpty(autoNumberName))
					{
						sSQL.Append(" (");
						sSQL.Append(NameDelimiterBegin);
						sSQL.Append(autoNumberName);
						sSQL.Append(NameDelimiterEnd);
						sSQL.Append(" INT NOT NULL AUTO_INCREMENT PRIMARY KEY )");
					}
				}
				else
				{
					sSQL.Append("CREATE TABLE ");
					sSQL.Append(NameDelimiterBegin);
					sSQL.Append(tableName);
					sSQL.Append(NameDelimiterEnd);
					if (!string.IsNullOrEmpty(autoNumberName))
					{
						sSQL.Append(" (");
						sSQL.Append(NameDelimiterBegin);
						sSQL.Append(autoNumberName);
						sSQL.Append(NameDelimiterEnd);
						sSQL.Append(" INTEGER IDENTITY CONSTRAINT PrimaryKey PRIMARY KEY);");
					}
				}
			}
			ExecuteNonQuery(sSQL.ToString());
		}
		public void DropColumn(string tableName, string columnName)
		{
			StringBuilder s = new StringBuilder("ALTER TABLE ");
			s.Append(NameDelimiterBegin);
			s.Append(tableName);
			s.Append(NameDelimiterEnd);
			s.Append(" DROP COLUMN ");
			s.Append(NameDelimiterBegin);
			s.Append(columnName);
			s.Append(NameDelimiterEnd);
			ExecuteNonQuery(s.ToString());
		}
		public void DropRelation(string tableName, string relationName)
		{
			StringBuilder s = new StringBuilder("ALTER TABLE ");
			s.Append(NameDelimiterBegin);
			s.Append(tableName);
			s.Append(NameDelimiterEnd);
			s.Append(" DROP CONSTRAINT ");
			s.Append(NameDelimiterBegin);
			s.Append(relationName);
			s.Append(NameDelimiterEnd);
			ExecuteNonQuery(s.ToString());
		}
		public string GetDataSource()
		{
			return ConnectionStringSelector.GetDataSource(ConnectionString);
		}
		public void SetDataSource(string filename)
		{
			string s = ConnectionString;
			ConnectionStringSelector.SetDataSource(ref s, filename);
			_connectionString = s;
		}
		public override string ToString()
		{
			if (EncryptConnectionString)
			{
				return _guid.ToString();
			}
			return ConnectionString;
		}
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			if (_connection == null)
				return null;
			return _connection.BeginTransaction(isolationLevel);
		}

		public override void ChangeDatabase(string databaseName)
		{
			if (_connection != null)
			{
				_connection.ChangeDatabase(databaseName);
			}
		}

		public override void Close()
		{
			if (_connection != null)
			{
				if (_connection.State != ConnectionState.Closed)
				{
					_connection.Close();
				}
			}
		}

		protected override DbCommand CreateDbCommand()
		{
			if (_connection == null)
				return null;
			return _connection.CreateCommand();
		}
		public override void Open()
		{
			if (_connection != null)
			{
				set_readonly();
				if (!string.IsNullOrEmpty(_connection.ConnectionString))
				{
					bool inGAC = _connection.GetType().Assembly.GlobalAssemblyCache;
					try
					{
						if (_connection.State == ConnectionState.Closed)
						{
							if (!inGAC)
							{
								VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(_connection.GetType().Assembly.Location));
							}
							_connection.Open();
						}
					}
					catch (Exception err)
					{
						FormLog.NotifyException(true,err);
					}
					finally
					{
						if (!inGAC)
						{
							VPLUtil.RemoveExternalDllResolve();
						}
					}
				}
			}
		}
		public string[] GetTableNames()
		{
			DataTable tbl = GetTables();
			if (tbl != null)
			{
				string[] ret = new string[tbl.Rows.Count];
				for (int i = 0; i < tbl.Rows.Count; i++)
				{
					ret[i] = tbl.Rows[i]["Table_Name"] as string;
				}
				return ret;
			}
			return new string[] { };
		}
		public FieldList GetTableFields(string tableName)
		{
			DatabaseTable dt = new DatabaseTable();
			dt.TableName = tableName;
			dt.GetFields(this);
			return dt.fields;
		}
		public DataTable GetTables()
		{
			System.Data.DataTable schema = null;
			bool bClosed = !Opened;
			try
			{
				/*SELECT TABLE_NAME 
   FROM INFORMATION_SCHEMA.TABLES
   WHERE TABLE_TYPE = 'BASE TABLE'*/
				if (bClosed)
					Open();
				if (Opened)
				{
					OleDbConnection oleCnt = TheConnection as OleDbConnection;
					if (oleCnt != null)
					{
						schema = oleCnt.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables,
									new object[] { null, null, null, "TABLE" });
					}
					else
					{
						if (IsOdbc)
						{
							schema = new System.Data.DataTable();
							ODBCSchema odbcSchema = new ODBCSchema();
							if (odbcSchema.Open(this.ConnectionString))
							{
								object[] vs;
								schema.Columns.Add("Table_Name");
								schema.Columns.Add("Table_type");
								string[] ss = odbcSchema.TableNames();
								if (ss != null)
								{
									for (int i = 0; i < ss.Length; i++)
									{
										vs = new object[2];
										vs[0] = ss[i];
										vs[1] = "TABLE";
										schema.Rows.Add(vs);
									}
								}
								odbcSchema.Close();
							}
						}
						else
						{
							schema = TheConnection.GetSchema("Tables");
						}
					}
				}
				if (bClosed)
				{
					if (TheConnection != null && TheConnection.State != ConnectionState.Closed)
					{
						Close();
					}
				}
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err);
			}
			finally
			{
				if (bClosed)
				{
					if (TheConnection != null && TheConnection.State != ConnectionState.Closed)
					{
						Close();
					}
				}
			}
			return schema;
		}

		public System.Data.DataTable GetViews()
		{
			System.Data.DataTable schema = null;
			bool bClosed = !Opened;
			try
			{
				if (bClosed)
					Open();
				if (Opened)
				{
					System.Data.DataTable vs = TheConnection.GetSchema("Views");
					schema = new DataTable("Views");
					schema.Columns.Add("Name", typeof(string));
					if (vs != null)
					{
						for (int i = 0; i < vs.Rows.Count; i++)
						{
							schema.Rows.Add(vs.Rows[i]["TABLE_NAME"]);
						}
					}
				}
				if (bClosed)
				{
					if (TheConnection != null && TheConnection.State != ConnectionState.Closed)
					{
						Close();
					}
				}
			}
			catch (NotSupportedException)
			{
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err);
			}
			finally
			{
				if (bClosed)
				{
					if (TheConnection != null && TheConnection.State != ConnectionState.Closed)
					{
						Close();
					}
				}
			}
			return schema;
		}

		public System.Data.DataTable GetRelations()
		{
			System.Data.DataTable schema = null;
			bool bClosed = !Opened;
			try
			{
				if (bClosed)
					Open();
				if (Opened)
				{
					OleDbConnection oleCnt = TheConnection as OleDbConnection;
					if (oleCnt != null)
					{
						try
						{
							schema = oleCnt.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys, null);
						}
						catch (Exception err)
						{
							FormLog.NotifyException(true,err);
							schema = null;
						}
					}
					else
					{
						try
						{
							if (IsMySql && !IsOdbc)
							{
								DataTable tb = new DataTable("relations");
								tb.Columns.Add("PK_TABLE_NAME", typeof(string));
								tb.Columns.Add("FK_TABLE_NAME", typeof(string));
								tb.Columns.Add("PK_COLUMN_NAME", typeof(string));
								tb.Columns.Add("FK_COLUMN_NAME", typeof(string));
								tb.Columns.Add("FK_NAME", typeof(string));
								DbCommand cmd = TheConnection.CreateCommand();
								cmd.CommandText = "select referenced_table_name as PK_TABLE_NAME,table_name as FK_TABLE_NAME, referenced_column_name as PK_COLUMN_NAME,column_name as FK_COLUMN_NAME,constraint_name as FK_NAME FROM INFORMATION_SCHEMA.key_column_usage where referenced_column_name IS NOT NULL;";
								DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
								while (dr.Read())
								{
									object[] vs = new object[5];
									vs[0] = dr["PK_TABLE_NAME"];
									vs[1] = dr["FK_TABLE_NAME"];
									vs[2] = dr["PK_COLUMN_NAME"];
									vs[3] = dr["FK_COLUMN_NAME"];
									vs[4] = dr["FK_NAME"];
									tb.Rows.Add(vs);
								}
								schema = tb;
							}
							else
							{
								schema = TheConnection.GetSchema("Foreign Keys");
							}
						}
						catch (ArgumentException)
						{
							schema = null;
						}
						catch (NotSupportedException)
						{
							schema = null;
						}
						catch (Exception err)
						{
							FormLog.NotifyException(true,err);
							schema = null;
						}
					}
					if (bClosed)
					{
						if (TheConnection.State != ConnectionState.Closed)
						{
							Close();
						}
					}
				}
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err);
			}
			finally
			{
				if (bClosed)
				{
					if (TheConnection != null && TheConnection.State != ConnectionState.Closed)
					{
						Close();
					}
				}
			}
			return schema;
		}
		public System.Data.DataTable GetIndexes(string sTable)
		{
			System.Data.DataTable schemaIndex = null;
			bool bClosed = !Opened;
			try
			{
				if (bClosed)
					Open();
				OleDbConnection oleCnt = TheConnection as OleDbConnection;
				if (oleCnt != null)
				{
					schemaIndex = oleCnt.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Indexes, null);
				}
				else
				{
					OdbcConnection odbcCnt = TheConnection as OdbcConnection;
					if (odbcCnt != null)
					{
						if (this.NameDelimiterStyle == EnumQulifierDelimiter.MySQL)
						{
							System.Data.Odbc.OdbcCommand cmd = odbcCnt.CreateCommand();
							cmd.CommandText = "SHOW INDEX FROM " + DatabaseEditUtil.SepBegin(this.NameDelimiterStyle) + sTable + DatabaseEditUtil.SepEnd(this.NameDelimiterStyle);
							System.Data.Odbc.OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(cmd);
							System.Data.DataSet ds = new DataSet();
							da.Fill(ds);
							schemaIndex = ds.Tables[0];
						}
					}
					else
					{
						SqlConnection sqlCnt = TheConnection as SqlConnection;
						if (sqlCnt != null)
						{
							try
							{
								SqlCommand cmd = sqlCnt.CreateCommand();
								StringBuilder sb = new StringBuilder();
								sb.Append("select c.[name] as [COLUMN_NAME], obj.[name] as [TABLE_NAME], ti.[name] as [INDEX_NAME],ti.[is_primary_key] as [PRIMARY_KEY],ti.[is_unique] as [UNIQUE] from sys.indexes ti ");
								sb.Append("inner join sys.objects obj on ti.object_id=obj.object_id ");
								sb.Append("inner join sys.index_columns ic on ti.object_id=ic.object_id and ic.index_id=ti.index_id ");
								sb.Append("inner join sys.columns c on (c.object_id=obj.object_id and c.column_id=ic.column_id) ");
								sb.Append("where obj.[name]='");
								sb.Append(sTable);
								sb.Append("'");
								cmd.CommandText = sb.ToString();
								SqlDataAdapter da = new SqlDataAdapter(cmd);
								System.Data.DataSet ds = new DataSet();
								da.Fill(ds);
								schemaIndex = ds.Tables[0];
							}
							catch
							{
								schemaIndex = null;
							}
						}
						else
						{
							try
							{
								DbCommand cmd = TheConnection.CreateCommand();
								StringBuilder sb = new StringBuilder();
								sb.Append("SHOW INDEXES FROM ");
								sb.Append(this.NameDelimiterBegin);
								sb.Append(sTable);
								sb.Append(this.NameDelimiterEnd);
								cmd.CommandText = sb.ToString();
								DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
								schemaIndex = new DataTable(sTable);
								schemaIndex.Columns.Add("COLUMN_NAME", typeof(string));
								schemaIndex.Columns.Add("TABLE_NAME", typeof(string));
								schemaIndex.Columns.Add("INDEX_NAME", typeof(string));
								schemaIndex.Columns.Add("PRIMARY_KEY", typeof(bool));
								schemaIndex.Columns.Add("UNIQUE", typeof(bool));
								while (dr.Read())
								{
									object[] vs = new object[5];
									vs[0] = dr["Column_name"];
									vs[1] = dr["Table"];
									vs[2] = dr["Key_name"];

									vs[3] = false;
									if (vs[2] != null && vs[2] != DBNull.Value)
									{
										if (string.Compare("PRIMARY", vs[2].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
										{
											vs[3] = true;
										}
									}
									vs[4] = false;
									object v = dr["Non_unique"];
									if (v != null && v != DBNull.Value)
									{
										int n;
										if (int.TryParse(v.ToString(), out n))
										{
											vs[4] = (n == 0);
										}
									}
									schemaIndex.Rows.Add(vs);
								}
							}
							catch
							{
								schemaIndex = null;
							}
						}
					}
				}

				if (bClosed)
					Close();
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err);
			}
			finally
			{
				if (bClosed)
					Close();
			}
			return schemaIndex;
		}
		public static bool IsForExcel(string s)
		{
			return (s.IndexOf("SQLOLEDB", StringComparison.OrdinalIgnoreCase) < 0 && s.IndexOf("MICROSOFT.JET.OLEDB", StringComparison.OrdinalIgnoreCase) > 0 && s.IndexOf("EXTENDED PROPERTIES=\"EXCEL ", StringComparison.OrdinalIgnoreCase) > 0);
		}
		#endregion

		#region Properties
		/// <summary>
		/// accessing it will actually test the connection
		/// </summary>
		public bool IsConnectionReady
		{
			get
			{
				if (_connection != null)
				{
					if (_connection.State != System.Data.ConnectionState.Closed)
					{
						return true;
					}
					bool bOK = false;

					if (!string.IsNullOrEmpty(_connectionString))
					{
						try
						{
							set_readonly();
							_connection.Open();
							bOK = (_connection.State != ConnectionState.Closed);
							_connection.Close();
						}
						catch (Exception e)
						{
							_error = e.Message;
							bOK = false;
						}
						finally
						{
							try
							{
								if (_connection.State != System.Data.ConnectionState.Closed)
								{
									_connection.Close();
								}
							}
							catch
							{
							}
						}
					}
					return bOK;
				}
				return false;
			}
		}
		[DefaultValue(false)]
		[Description("Indicates whether the ConnectionString should be encrypted. Another option to use passwords in the connection string is to set SavePasswordInConnectionString to False and call SetCredential action to pass passwords at runtime. Another option is to use connection methods that do not require putting passwords in the connection string. For example, use Windows Authentication instead of SQL Authentication when connecting to Microsoft SQL Server.")]
		public bool EncryptConnectionString
		{
			get
			{
				return _encryptStr;
			}
			set
			{
				if (_encryptStr != value)
				{
					_encryptStr = value;
					if (!_encryptStr)
					{
						_connectionString = "";
						ConnectionConfig.SetConnection(_guid, "");
					}
				}
			}
		}

		[DefaultValue(true)]
		[Description("Indicates whether passwords expressed as 'Password={string}' and 'Jet OLEDB:Database Password={string}' in a connection string should be saved together with the ConnectionString. Another option to use passwords in the connection string is to set SavePasswordInConnectionString to False and call SetCredential to pass passwords at runtime. If EncryptConnectionString is True then passwords are always saved even if SavePasswordInConnectionString is False. Another option is to use connection methods that do not require putting passwords in the connection string. For example, use Windows Authentication instead of SQL Authentication when connecting to Microsoft SQL Server. ")]
		public bool SavePasswordInConnectionString { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		[ReadOnly(true)]
		public DbConnection TheConnection
		{
			get
			{
				return _connection;
			}
			set
			{
				_connection = value;
				if (_connection != null)
				{
					_type = _connection.GetType();
					_connectionString = _connection.ConnectionString;
				}
			}
		}
		[XmlIgnore]
		[Editor(typeof(ConnectionTypeSelector), typeof(UITypeEditor))]
		[Description("The database connection type")]
		public Type DatabaseType
		{
			get
			{
				return _type;
			}
			set
			{
				if (value != null)
				{
					if (value.IsAbstract)
					{
						throw new ExceptionLimnorDatabase("DatabaseType [{0}] is abstract", value);
					}
					if (!typeof(DbConnection).IsAssignableFrom(value))
					{
						throw new ExceptionLimnorDatabase("Type [{0}] is not a database connection", value);
					}
					_type = value;
					if (_type.Equals(typeof(OleDbConnection)) || _type.Equals(typeof(SqlConnection)))
					{
						_delimiterType = EnumQulifierDelimiter.Microsoft;
					}
					else
					{
						_delimiterType = EnumQulifierDelimiter.MySQL;
					}
					if (_type.Equals(typeof(SqlConnection)))
					{
						_paramStyle = EnumParameterStyle.LeadingAt;
					}
					if (_type.FullName.IndexOf("MySql.Data.MySqlClient.MySqlConnection", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						_paramStyle = EnumParameterStyle.LeadingQuestionMark;
					}
					if (typeof(OdbcConnection).IsAssignableFrom(_type))
					{
						_paramStyle = EnumParameterStyle.QuestionMark;
					}
					if (_connection != null)
					{
						if (!_type.Equals(_connection.GetType()))
						{
							_connection = null;
						}
					}
					if (_connection == null)
					{
						_connection = (DbConnection)VPLUtil.CreateObject(_type, "server=localhost;database=test");
						set_readonly();
					}
				}
			}
		}
		[Browsable(false)]
		public string LastError
		{
			get
			{
				return _error;
			}
		}
		[Browsable(false)]
		public Guid ConnectionGuid
		{
			get
			{
				return _guid;
			}
			set
			{
				_guid = value;
				if (string.IsNullOrEmpty(_connectionString) || string.CompareOrdinal(_connectionString, "!") == 0)
				{
					_connectionString = ConnectionConfig.GetConnectionString(_guid);
				}
			}
		}
		[XmlIgnore]
		[Description("The information needed for making the database connection")]
		[Editor(typeof(ConnectionStringSelector), typeof(UITypeEditor))]
		public string ConnectionString_
		{
			get
			{
				if (EncryptConnectionString)
				{
					return "***";
				}
				return ConnectionString;
			}
			set
			{
				ConnectionString = value;
			}
		}
		[Browsable(false)]
		public override string ConnectionString
		{
			get
			{
				if (_connectionString == null)
				{
					_connectionString = "";
				}
				if (_connectionString.StartsWith("!", StringComparison.Ordinal))
				{
					set_readonly();
				}
				return _connectionString;
			}
			set
			{
				_connectionString = value;
				set_readonly();
			}
		}
		public override string DataSource
		{
			get
			{
				string s = string.Empty;
				if (_connection != null)
				{
					try
					{
						s = _connection.DataSource;
					}
					catch
					{
					}
				}
				return s;
			}
		}

		public override string Database
		{
			get
			{
				string s = string.Empty;
				if (_connection != null)
				{
					try
					{
						s = _connection.Database;
					}
					catch
					{
					}
				}
				return s;
			}
		}


		public override string ServerVersion
		{
			get
			{
				string s = string.Empty;
				if (_connection != null)
				{
					try
					{
						s = _connection.ServerVersion;
					}
					catch
					{
					}
				}
				return s;
			}
		}

		public override ConnectionState State
		{
			get
			{
				if (_connection == null)
					return ConnectionState.Closed;
				return _connection.State;
			}
		}
		public bool Opened
		{
			get
			{
				return (State != ConnectionState.Closed) && (State != ConnectionState.Broken);
			}
		}

		public string NameDelimiterBegin
		{
			get
			{
				return DatabaseEditUtil.SepBegin(NameDelimiterStyle);
			}
		}
		public string NameDelimiterEnd
		{
			get
			{
				return DatabaseEditUtil.SepEnd(NameDelimiterStyle);
			}
		}
		public EnumQulifierDelimiter NameDelimiterStyle
		{
			get
			{
				return _delimiterType;
			}
			set
			{
				_delimiterType = value;
			}
		}
		[Description("Gets and sets a parameter style used by the database driver. A parameter in a query may be represented by a question mark, by a name with a prefix of question mark or a @.")]
		public EnumParameterStyle ParameterStyle
		{
			get
			{
				if (_paramStyle != EnumParameterStyle.LeadingAt)
				{
					if (IsMSSQL)
					{
						_paramStyle = EnumParameterStyle.LeadingAt;
					}
				}
				return _paramStyle;
			}
			set
			{
				_paramStyle = value;
			}
		}
		[Browsable(false)]
		public string DatabaseDisplayText
		{
			get
			{
				string s = ConnectionStringSelector.GetDataSource(_connectionString);
				if (string.IsNullOrEmpty(s))
				{
					s = ConnectionStringSelector.GetDatabaseName(_connectionString);
					if (string.IsNullOrEmpty(s))
					{
						return _connectionString;
					}
				}
				return s;
			}
		}
		public bool IsOdbc
		{
			get
			{
				if (_connection != null)
				{
					return (_connection is OdbcConnection);
				}
				if (_type != null)
				{
					return typeof(OdbcConnection).IsAssignableFrom(_type);
				}
				return false;
			}
		}

		public bool IsExcel
		{
			get
			{
				string s = this.ConnectionString;
				if (!string.IsNullOrEmpty(s))
				{
					return IsForExcel(s);
				}
				return false;
			}
		}
		public static bool IsForJet(string s)
		{
			return (s.IndexOf("SQLOLEDB", StringComparison.OrdinalIgnoreCase) < 0 && s.IndexOf("MICROSOFT.JET.OLEDB", StringComparison.OrdinalIgnoreCase) > 0);
		}
		public bool IsJet
		{
			get
			{
				string s = this.ConnectionString;
				if (!string.IsNullOrEmpty(s))
				{
					return IsForJet(s);
				}
				return false;
			}
		}
		public bool IsMSSQL
		{
			get
			{
				if (this.TheConnection is SqlConnection)
					return true;
				if (this.TheConnection is OleDbConnection)
				{
					if (ConnectionString.IndexOf("SQLOLEDB", StringComparison.OrdinalIgnoreCase) > 0)
						return true;
				}
				return false;
			}
		}
		public bool IsMySql
		{
			get
			{
				if (this.TheConnection != null)
				{
					if (string.Compare(this.TheConnection.GetType().FullName, MySqlConnectionTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
				return false;
			}
		}
		public EnumTopRecStyle TopRecStyle
		{
			get
			{
				if (IsMySql)
				{
					return EnumTopRecStyle.Limit;
				}
				else
				{
					if (IsMSSQL || IsJet)
					{
						return EnumTopRecStyle.TopN;
					}
					else
					{
						return EnumTopRecStyle.NotAllowed;
					}
				}
			}
		}
		public bool IsOleDb
		{
			get
			{
				if (this.TheConnection is OleDbConnection)
					return true;
				return false;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether SQL keywords should be lower case.")]
		public bool LowerCaseSqlKeywords
		{
			get;
			set;
		}
		#endregion

		#region ODBC support
		public DatabaseTable[] ODBC_GetTables()
		{
			string sCon = this.ConnectionString;
			DatabaseTable[] tbls = null;
			if (odbcSchema == null)
				odbcSchema = new ODBCSchema();
			if (odbcSchema.Open(sCon))
			{
				string[] ss = odbcSchema.TableNames();
				if (ss != null)
				{
					tbls = new DatabaseTable[ss.Length];
					for (int i = 0; i < ss.Length; i++)
					{
						tbls[i] = new DatabaseTable();
						tbls[i].TableName = ss[i];
					}
				}
				odbcSchema.Close();
			}
			return tbls;
		}
		public FieldList ODBC_GetFields(string sTable)
		{
			FieldList flds = null;
			if (odbcSchema == null)
				odbcSchema = new ODBCSchema();
			if (odbcSchema.Open(this.ConnectionString))
			{
				flds = odbcSchema.GetFields(sTable);
				odbcSchema.Close();
			}
			return flds;
		}
		public DatabaseView[] ODBC_GetViews()
		{
			DatabaseView[] tbls = null;
			if (odbcSchema == null)
				odbcSchema = new ODBCSchema();
			if (odbcSchema.Open(this.ConnectionString))
			{
				string[] ss = odbcSchema.ViewNames();
				if (ss != null)
				{
					tbls = new DatabaseView[ss.Length];
					for (int i = 0; i < ss.Length; i++)
					{
						tbls[i] = new DatabaseView();
						tbls[i].ViewName = ss[i];
						tbls[i].fields = odbcSchema.GetFields(ss[i]);
					}
				}
				odbcSchema.Close();
			}
			return tbls;
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			Connection obj = new Connection();
			if (_connection != null)
			{
				obj._connection = _connection;
			}
			obj._connectionString = _connectionString;
			obj._delimiterType = _delimiterType;
			obj._paramStyle = _paramStyle;
			obj._guid = _guid;
			obj._type = _type;
			obj._credential = _credential;
			obj.LowerCaseSqlKeywords = this.LowerCaseSqlKeywords;
			return obj;
		}

		#endregion
	}
}
