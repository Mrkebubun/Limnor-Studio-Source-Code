/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Data;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.Data.Common;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.OracleClient;
using VPL;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using Limnor.WebServerBuilder;
using System.Globalization;
using Limnor.WebBuilder;
using System.Text;
using WebServerProcessor;

namespace LimnorDatabase
{
	/// <summary>
	/// 
	/// </summary>
	[WebServerMember]
	[ToolboxBitmapAttribute(typeof(EasyUpdator), "Resources.dbUpdator.bmp")]
	[Description("This component executes database updating commands. It can be used for adding new records, modifying existing records, or deleting records.")]
	public class EasyUpdator : IComponent, IReport32Usage, ISourceValueEnumProvider, IDynamicMethodParameters, IDatabaseConnectionUserExt, IWebServerProgrammingSupport, ICustomTypeDescriptor
	{
		#region fields and constructors
		//=========================================
		private ConnectionItem connect = null;
		private string _defConnectionString;
		private Type _defConnectionType;
		private DbTransaction _transaction;
		//=========================================
		private SQLNoneQuery _sql;
		//=========================================
		protected string TableName = "";
		protected bool bLoaded = false;
		//
		private string _errMsg;
		private int _affectedRows;
		//
		private JsonWebServerProcessor _webPage;
		private string _name;
		//
		static private StringCollection _subClassReadOnlyProperties;
		static EasyUpdator()
		{
			_subClassReadOnlyProperties = new StringCollection();
			_subClassReadOnlyProperties.Add("ExecutionCommand");
			_subClassReadOnlyProperties.Add("Parameters");

		}
		public EasyUpdator()
		{
			ShowErrorMessage = true;
		}
		public EasyUpdator(IContainer c)
		{
			ShowErrorMessage = true;
			if (c != null)
			{
				c.Add(this);
			}
		}
		internal void SetTransaction(DbTransaction tr, ConnectionItem connection)
		{
			_transaction = tr;
			connect = connection;
		}
		//
		#endregion

		#region private methods
		private EPField getParameterByName(string parameterName)
		{
			if (ParameterList != null)
			{
				return ParameterList[parameterName];
			}
			return null;
		}


		private void setDefaultConnection()
		{
			if (_defConnectionType != null && !string.IsNullOrEmpty(_defConnectionString))
			{
				if (connect != null)
				{
					Guid id;
					if (!string.IsNullOrEmpty(connect.Filename))
					{
						id = new Guid(connect.Filename);
					}
					else
					{
						id = connect.ConnectionObject.ConnectionGuid;
					}
					Connection _def = new Connection();
					_def.ConnectionGuid = id;
					_def.DatabaseType = _defConnectionType;
					_def.ConnectionString = _defConnectionString;
					ConnectionItem.AddDefaultConnection(_def);
					if (connect.ConnectionObject.DatabaseType == null)
					{
						connect.ConnectionObject.DatabaseType = _defConnectionType;
					}
					if (string.IsNullOrEmpty(connect.ConnectionObject.ConnectionString))
					{
						connect.ConnectionObject.ConnectionString = _defConnectionString;
					}
				}
			}
		}

		#endregion

		#region Protected Methods
		protected virtual void OnFormedParameterList(DbParameterList pl)
		{
		}
		protected void SetError(string sErr)
		{
			_errMsg = sErr;
			if (!string.IsNullOrEmpty(_errMsg))
			{
				if (ShowErrorMessage)
				{
					FormLog.ShowMessage(_errMsg);
				}
				if (ExecuteError != null)
				{
					ExecuteError(this, EventArgs.Empty);
				}
			}
		}
		protected void ResetAffectedRows()
		{
			_affectedRows = 0;
		}
		protected void ResetTransaction()
		{
			_transaction = null;
		}
		protected void closeConnections()
		{
			if (connect != null)
			{
				connect.ConnectionObject.Close();
			}
		}
		protected void FireExecuteFinish()
		{
			if (this.ExecuteFinish != null)
			{
				ExecuteFinish(this, EventArgs.Empty);
			}
		}
		#endregion

		#region Protected Properties
		protected ConnectionItem Connection
		{
			get
			{
				return connect;
			}
		}
		protected DbTransaction Transaction
		{
			get
			{
				return _transaction;
			}
		}
		#endregion

		#region events
		[WebClientMember]
		[WebClientEventByServerObject]
		[Description("Occurs when the Execute method completes successfully.")]
		public event EventHandler ExecuteFinish = null;
		//
		[Description("Occurs when the Execute method fails. The ErrorMessage property gives information about why the execution failed.")]
		public event EventHandler ExecuteError = null;
		//
		#endregion

		#region Methods
		[Description("Set the full file path for the log file for database activities. ")]
		public static void SetLogFilePath(string logFileName)
		{
			EasyQuery.SetLogFilePath(logFileName);
		}
		[Browsable(false)]
		public string GetConnectionCodeName()
		{
			return ServerCodeutility.GetPhpMySqlConnectionName(this.ConnectionID);
		}
		[Description("Use a dialogue box to make database query.")]
		public void SelectQuery(Form dialogParent)
		{
			Connection cnn;
			if (connect != null)
			{
				cnn = (Connection)connect.ConnectionObject.Clone();
			}
			else
			{
				cnn = new Connection();
			}
			DlgConnection dlg = cnn.CreateDataDialog();
			if (dlg.ShowDialog(dialogParent) == DialogResult.OK)
			{
				connect = new ConnectionItem(cnn);
			}
		}
		[Description("Set connection string for accessing the database.")]
		public void SetConnectionString(string connectionString)
		{
			if (connect == null)
			{
				connect = new ConnectionItem();
			}
			connect.ConnectionString = connectionString;
		}
		[Description("Execute database command. Return an error message if failed. Returns an empty string if no error detected.")]
		public virtual string ExecuteCommand(string command)
		{
			string sMsg = string.Empty;
			SetError(sMsg);
			_affectedRows = 0;
			connect.Log("Executing command:{0}", command);
			if (!string.IsNullOrEmpty(command))
			{
				DbCommand cmd = connect.ConnectionObject.CreateCommand();
				if (_transaction != null)
				{
					cmd.Transaction = _transaction;
				}
				bool bClosed = !connect.ConnectionObject.Opened;
				if (bClosed)
					connect.ConnectionObject.Open();
				if (connect.ConnectionObject.Opened)
				{
					try
					{
						cmd.CommandText = command;
						cmd.CommandType = CommandType.Text;
						_affectedRows = cmd.ExecuteNonQuery();
						connect.Log("Affected rows:{0}", _affectedRows);
						if (bClosed)
						{
							closeConnections();
						}
						if (this.ExecuteFinish != null)
						{
							ExecuteFinish(this, EventArgs.Empty);
						}
						if (_webPage != null && !string.IsNullOrEmpty(_name))
						{
							_webPage.SetServerComponentName(_name);
						}
					}
					catch (Exception er)
					{
						if (_transaction != null)
						{
							_transaction.Rollback();
							_transaction.Dispose();
							_transaction = null;
							connect.Log("rollback with error {0}", er.Message);
							throw;
						}
						else
						{
							sMsg = ExceptionLimnorDatabase.FormExceptionText(er);
						}
					}
					finally
					{
						if (bClosed)
						{
							if (connect.ConnectionObject.State != ConnectionState.Closed)
							{
								connect.ConnectionObject.Close();
							}
						}
					}
				}
			}
			return sMsg;
		}
		[Description("Execute database updating command. If more than one Execute action is assigned to event ExecuteInOneTransaction then those Execute actions will be executed in one single database transaction.")]
		public virtual string Execute()
		{
			string sMsg = string.Empty;
			SetError(sMsg);
			_affectedRows = 0;
			if (_sql != null && connect != null)
			{
				DbCommand cmd = connect.ConnectionObject.CreateCommand();
				if (_transaction != null)
				{
					cmd.Transaction = _transaction;
				}
				bool bClosed = !connect.ConnectionObject.Opened;
				if (bClosed)
					connect.ConnectionObject.Open();
				if (connect.ConnectionObject.Opened)
				{
					try
					{
						int i;
						EnumParameterStyle pstyle = connect.ParameterStyle;
						FieldList pmMap = new FieldList();
						string s;
						int nCount;
						s = _sql.GetSQLStatement(pmMap, pstyle);
						if (pstyle == EnumParameterStyle.QuestionMark)
						{
							nCount = pmMap.Count;
						}
						else
						{
							nCount = _sql.ParamCount;
						}
						connect.Log("Command:{0}, params:{1}", s, nCount);
						cmd.CommandText = s;
						cmd.CommandType = CommandType.Text;
						for (i = 0; i < nCount; i++)
						{
							DbParameter pam = cmd.CreateParameter();
							EPField f;
							if (pstyle == EnumParameterStyle.QuestionMark)
							{
								f = pmMap[i];
								pam.ParameterName = "@P" + i.ToString();
							}
							else
							{
								f = _sql.Parameters[i];
								pam.ParameterName = ParameterList.GetParameterName(pstyle, _sql.Parameters[i].Name);
							}
							pam.DbType = ValueConvertor.OleDbTypeToDbType(f.OleDbType);
							pam.Size = f.DataSize;

							OleDbParameter op = pam as OleDbParameter;
							if (op != null && f.OleDbType == OleDbType.DBTimeStamp)
							{
								if (f.Value != null && f.Value != DBNull.Value)
								{
									DateTime dt = (DateTime)(f.Value);
									dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
									f.SetValue(dt);
								}
							}
							connect.Log("param{0}:{1},{2},{3}. Value type:{4}. Value:{5}", i, pam.ParameterName, pam.DbType, pam.Size, f.OleDbType, f.Value);
							if (EPField.IsDatetime(f.OleDbType))
							{
								if (f.IsNullOrEmpty)
									pam.Value = System.DBNull.Value;
								else
								{
									object dt0 = ValueConvertor.ConvertByOleDbType(f.Value, f.OleDbType);
									try
									{
										DateTime dt = (DateTime)dt0;
										if (dt.Ticks == 0)
										{
											pam.Value = System.DBNull.Value;
										}
										else
										{
											pam.Value = dt;
										}
									}
									catch
									{
										pam.Value = dt0;
									}
								}
							}
							else
							{
								pam.Value = ValueConvertor.ConvertByOleDbType(f.Value, f.OleDbType);
							}
							cmd.Parameters.Add(pam);
						}
						_affectedRows = cmd.ExecuteNonQuery();
						connect.Log("Affected rows:{0}", _affectedRows);
						if (bClosed)
						{
							closeConnections();
						}
						if (this.ExecuteFinish != null)
						{
							ExecuteFinish(this, EventArgs.Empty);
						}
						if (_webPage != null && !string.IsNullOrEmpty(_name))
						{
							_webPage.SetServerComponentName(_name);
						}
					}
					catch (Exception er)
					{
						if (_transaction != null)
						{
							_transaction.Rollback();
							_transaction.Dispose();
							_transaction = null;
							connect.Log("rollback with error {0}", er.Message);
							throw;
						}
						else
						{
							sMsg = ExceptionLimnorDatabase.FormExceptionText(er);
						}
					}
					finally
					{
						if (bClosed)
						{
							if (connect.ConnectionObject.State != ConnectionState.Closed)
							{
								connect.ConnectionObject.Close();
							}
						}
					}
				}
				else
				{
					sMsg = "Database connection not set";
				}
			}
			else
			{
				sMsg = "SQL statement not set";
			}
			if (!string.IsNullOrEmpty(sMsg))
			{
				SetError(sMsg);
				if (connect != null)
				{
					connect.Log("Error executing EasyUpdator.Execute. {0}", sMsg);
				}
			}
			return sMsg;
		}
		
		[WebServerMember]
		[Description("Execute the database updating command with all the parameter values. In case of database error it returns an error message string; otherwise it return an empty string.")]
		public string ExecuteWithParameterValues(params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				ParameterList pl = ParameterList;
				if (pl != null)
				{
					int n = Math.Min(pl.Count, values.Length);
					for (int i = 0; i < n; i++)
					{
						pl[i].SetValue(values[i]);
					}
				}
			}
			return Execute();
		}
		[Description("Execute the database updating command using values from each row of a DataTable for parameter values. The value of the first column is for the first parameter, the second column is for the second parameter, and so on.  It executes as many times as the number of rows in the DataTable.")]
		public void ExecuteUseDataTableForParameterValuesByColumnPosition(DataTable parameterValues)
		{
			if (parameterValues != null)
			{
				for (int i = 0; i < parameterValues.Rows.Count; i++)
				{
					ExecuteWithParameterValues(parameterValues.Rows[i].ItemArray);
				}
			}
		}
		[Description("Execute the database updating command using values from each row of a DataTable for parameter values. The values of the columns are mapped to the parameters by column names. White spaces, '(', ')', '\\', '/', tab, carriage return and new-line are ignored in column names when doing matching. For example, column Price is mapped to parameter @Price, column Quantity is mapped to parameter @Quantity, etc. For example, Order Qty matches @OrderQty. It executes as many times as the number of rows in the DataTable.")]
		public void ExecuteUseDataTableForParameterValuesByColumnName(DataTable parameterValues)
		{
			if (parameterValues != null)
			{
				for (int i = 0; i < parameterValues.Rows.Count; i++)
				{
					for (int k = 0; k < parameterValues.Columns.Count; k++)
					{
						string name = Regex.Replace(parameterValues.Columns[k].ColumnName, "[()\t \r\n\\/]", "");
						SetParameterValue("@" + name, parameterValues.Rows[i][k]);
					}
					Execute();
				}
			}
		}
		[Description("Set the query parameter value to DBNull. ")]
		public void SetParameterValueToNull(string parameterName)
		{
			EPField p = getParameterByName(parameterName);
			if (p != null)
			{
				p.SetValue(DBNull.Value);
			}
		}
		[Description("Set query parameter value. ")]
		public void SetParameterValue(string parameterName, object value)
		{
			EPField p = getParameterByName(parameterName);
			if (p != null)
			{
				if (IsOleDb)
				{
					if (value != null)
					{
						Type t = value.GetType();
						if (typeof(DateTime).Equals(t))
						{
							DateTime dt = (DateTime)value;
							dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
							p.SetValue(dt);
							return;
						}
					}
				}
				p.SetValue(value);
			}
		}
		#endregion

		#region Properties
		[Category("Database")]
		public virtual bool IsStoredProc
		{
			get
			{
				return false;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		[Editor(typeof(HideUITypeEditor), typeof(UITypeEditor))]
		[Description("Parameters for filtering the data.")]
		public ParameterList ParameterList
		{
			get
			{
				if (_sql != null)
				{
					return _sql.Parameters;
				}
				return null;
			}
		}
		[Category("Database")]
		[Description("Gets a value indicating whether the last Execute action failed")]
		public bool HasError
		{
			get
			{
				return !string.IsNullOrEmpty(_errMsg);
			}
		}
		[WebServerMember]
		[Category("Database")]
		[Description("Gets the error message for the last Execute action if it failed")]
		public string ErrorMessage
		{
			get
			{
				return _errMsg;
			}
		}
		[XmlIgnore]
		[WebServerMember]
		[Category("Database")]
		[Description("Gets an integer which is the last auto number generated by an INSERT command.")]
		public int LastInsertID
		{
			get;
			set;
		}
		[Category("Database")]
		[DefaultValue(true)]
		[Description("Controls whether error message is displayed when event ExecuteError occurs.")]
		public bool ShowErrorMessage
		{
			get;
			set;
		}
		[Category("Database")]
		[Description("Gets a value indicating the number of affected rows of the last Execute action")]
		public int AffectedRowCount
		{
			get
			{
				return _affectedRows;
			}
		}
		[Browsable(false)]
		public Guid ConnectionID
		{
			get
			{
				if (connect != null)
					return connect.ConnectionObject.ConnectionGuid;
				return Guid.Empty;
			}
			set
			{
				if (value != Guid.Empty)
				{
					connect = ConnectionItem.LoadConnection(value, true, false);
					setDefaultConnection();
				}
			}
		}
		[Category("Database")]
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
		[Description("Connection to the database")]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				if (connect == null)
				{
					connect = new ConnectionItem();
				}
				return connect;
			}
			set
			{
				connect = value;
				if (_sql == null)
				{
					_sql = new SQLNoneQuery();
				}
				_sql.SetConnection(connect);
			}
		}
		[Category("Database")]
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorSQLNoneQuery), typeof(UITypeEditor))]
		[Description("Database command to update the database")]
		public virtual SQLNoneQuery ExecutionCommand
		{
			get
			{
				return _sql;
			}
			set
			{
				_sql = value;
			}
		}
		[Category("Database")]
		[Description("Parameters for filtering the data.")]
		public DbParameterList Parameters
		{
			get
			{
				DbParameterList pl = new DbParameterList(ParameterList);
				OnFormedParameterList(pl);
				return pl;
			}
		}
		[Category("Database")]
		[Description("Gets an integer indicating the number of parameters")]
		public int ParameterCount
		{
			get
			{
				if (_sql != null)
				{
					return _sql.ParamCount;
				}
				return 0;
			}
		}
		[Category("Database")]
		[Description("Connection string used for connecting to the database")]
		public string ConnectionString
		{
			get
			{
				if (connect != null)
				{
					return connect.ConnectionStringDisplay;
				}
				return "";
			}
		}
		[Category("Database")]
		[Description("Accessing this property will test connect to the database. If the connection is made then this property is True; otherwise it is False.")]
		public bool IsConnectionReady
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					return connect.ConnectionObject.IsConnectionReady;
				}
				return false;
			}
		}
		[Category("Database")]
		[Description("Indicates the database connection is through an OleDbConnection for OleDb drives such as Microsoft Access, Excel, etc.")]
		public bool IsOleDb
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					return (connect.ConnectionObject.TheConnection is OleDbConnection);
				}
				return false;
			}
		}
		[Category("Database")]
		[Description("Indicates the database connection is through an OdbcConnection for ODBC drives such as MySQL, PostgreSQL, etc.")]
		public bool IsOdbc
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					return (connect.ConnectionObject.TheConnection is OdbcConnection);
				}
				return false;
			}
		}
		[Category("Database")]
		[Description("Indicates the database connection is through a SqlConnection for Microsoft SQL Server")]
		public bool IsSQLSever
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					return (connect.ConnectionObject.TheConnection is SqlConnection);
				}
				return false;
			}
		}
		[Category("Database")]
		[Description("Indicates the database connection is through an OracleConnection for Oracle database")]
		public bool IsOracle
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null && connect.ConnectionObject.TheConnection != null)
				{
#if NET35
					return (connect.ConnectionObject.TheConnection is OracleConnection);
#else
					return connect.ConnectionObject.TheConnection.GetType().Name.IndexOf("OracleConnection") >= 0;
#endif
				}
				return false;
			}
		}
		#endregion

		#region non-browsable properties
		public string Name
		{
			get
			{
				if (_site != null && _site.DesignMode)
				{
					if (!string.IsNullOrEmpty(_site.Name))
					{
						_name = _site.Name;
					}
				}
				return _name;
			}
			set
			{
				_name = value;
				if (_site != null && _site.DesignMode)
				{
					if (!string.IsNullOrEmpty(_name))
					{
						_site.Name = _name;
					}
				}
			}
		}
		[Browsable(false)]
		[XmlIgnore]
		public string DefaultConnectionString
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					if (!string.IsNullOrEmpty(connect.ConnectionObject.ConnectionString))
					{
						return connect.ConnectionObject.ConnectionString;
					}
				}
				return _defConnectionString;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string s = value.Trim();
					if (s.Length > 0)
					{
						_defConnectionString = value;
						setDefaultConnection();
					}
				}
			}
		}
		/// <summary>
		/// for setting default connection
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Type DefaultConnectionType
		{
			get
			{
				if (connect != null && connect.ConnectionObject != null)
				{
					if (connect.ConnectionObject.DatabaseType != null)
					{
						return connect.ConnectionObject.DatabaseType;
					}
				}
				return _defConnectionType;
			}
			set
			{
				if (value != null)
				{
					_defConnectionType = value;
					setDefaultConnection();
				}
			}
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			closeConnections();
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IReport32Usage Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (connect != null)
			{
				if (connect.ConnectionObject != null)
				{
					if (connect.ConnectionObject.IsJet)
					{
						return Resource1.UseJet;
					}
				}
			}
			return string.Empty;
		}

		#endregion

		#region ISourceValueEnumProvider Members
		[Browsable(false)]
		public object[] GetValueEnum(string section, string item)
		{
			if (!string.IsNullOrEmpty(section))
			{
				if (section.StartsWith("SetParameterValue", StringComparison.Ordinal) && string.CompareOrdinal(item, "parameterName") == 0)
				{
					ParameterList pl = this.ParameterList;
					if (pl != null)
					{
						object[] vs = new object[pl.Count];
						for (int i = 0; i < pl.Count; i++)
						{
							vs[i] = pl[i].Name;
						}
						return vs;
					}
				}
			}
			return null;
		}

		#endregion

		#region IDynamicMethodParameters Members
		[Browsable(false)]
		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object av)
		{
			if (string.CompareOrdinal(methodName, "ExecuteWithParameterValues") == 0)
			{
				ParameterList pl = ParameterList;
				if (pl != null && pl.Count > 0)
				{
					ParameterInfo[] ps = new ParameterInfo[pl.Count];
					for (int i = 0; i < pl.Count; i++)
					{
						EPField f = pl[i];
						ps[i] = new SimpleParameterInfo(f.Name, methodName, EPField.ToSystemType(f.OleDbType), string.Format(System.Globalization.CultureInfo.InvariantCulture, "parameter {0}", f.Name));
					}
					return ps;
				}
				return new ParameterInfo[] { };
			}
			return null;
		}
		[Browsable(false)]
		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "ExecuteWithParameterValues") == 0)
			{
				ExecuteWithParameterValues(parameters);
			}
			return null;
		}
		[Browsable(false)]
		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			return (string.CompareOrdinal(methodName, "ExecuteWithParameterValues") == 0);
		}
		[NotForProgramming]
		[Browsable(false)]
		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}
		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public System.Collections.Generic.IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				List<Guid> lst = new List<Guid>();
				if (connect != null)
				{
					if (connect.ConnectionGuid != Guid.Empty)
					{
						lst.Add(connect.ConnectionGuid);
					}
				}
				return lst;
			}
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				List<Type> lst = new List<Type>();
				if (connect != null)
				{
					if (connect.ConnectionObject != null && connect.ConnectionObject.DatabaseType != null)
					{
						lst.Add(connect.ConnectionObject.DatabaseType);
					}
				}
				return lst;
			}
		}
		#endregion

		#region IWebServerProgrammingSupport Members
		[NotForProgramming]
		[Browsable(false)]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			string connName = GetConnectionCodeName();
			if (string.CompareOrdinal(methodName, "ExecuteWithParameterValues") == 0)
			{
				if (_sql != null && connect != null)
				{
					code.Add("$GLOBALS[\"debugError\"] = '';\r\n");
					code.Add("$msql = new JsonSourceMySql();\r\n");
					code.Add(string.Format(CultureInfo.InvariantCulture, "$msql->SetCredential($this->{0});\r\n", connName));
					code.Add("$msql->SetDebug($this->DEBUG);\r\n");
					//
					string sql;
					string[] sqlParams;
					List<string> inputParams = new List<string>();
					List<string> outputParams = new List<string>();
					if (this.IsStoredProc)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("call ");
						sb.Append(_sql.SQL);
						sb.Append("(");
						DatabaseExecuter de = this as DatabaseExecuter;
						DbParameterList ps = de.Parameters;
						if (ps != null && ps.Count > 0)
						{
							ParameterDirection[] pds = de.Param_Directions;
							for (int i = 0; i < ps.Count; i++)
							{
								if (i > 0)
								{
									sb.Append(",");
								}
								ParameterDirection pd = ParameterDirection.Input;
								if (pds != null && pds.Length > i)
								{
									pd = pds[i];
								}
								switch (pd)
								{
									case ParameterDirection.Input:
										inputParams.Add(ps[i].Name);
										sb.Append(" ? ");
										break;
									case ParameterDirection.InputOutput:
										outputParams.Add(ps[i].Name);
										sb.Append(string.Format(CultureInfo.InvariantCulture, " @{0} ", ps[i].Name));
										break;
									case ParameterDirection.Output:
										outputParams.Add(ps[i].Name);
										sb.Append(string.Format(CultureInfo.InvariantCulture, " @{0} ", ps[i].Name));
										break;
								}
							}
							sqlParams = inputParams.ToArray();
						}
						else
						{
							sqlParams = new string[] { };
						}
						sb.Append(")");
						sql = sb.ToString();
					}
					else
					{
						sql = _sql.SQL;
						sqlParams = EasyQuery.GetParameterNames(sql, connect.NameDelimiterBegin, connect.NameDelimiterEnd);
						if (sqlParams != null)
						{
							for (int i = 0; i < sqlParams.Length; i++)
							{
								sql = sql.Replace(sqlParams[i], "?");
							}
						}
					}
					code.Add(string.Format(CultureInfo.InvariantCulture, "$sql = \"{0} \";\r\n", sql));
					code.Add("$ps = array();\r\n");
					if (this.Parameters != null)
					{
						for (int i = 0; i < sqlParams.Length; i++)
						{
							int k = this.Parameters.GetIndex(sqlParams[i]);
							if (k < 0)
							{
								throw new ExceptionLimnorDatabase("Query Parameter [{0}] not found", sqlParams[i]);
							}
							DbCommandParam dp = this.Parameters[k];
							code.Add("$p = new SqlClientParameter();\r\n");
							code.Add(string.Format(CultureInfo.InvariantCulture, "$p->name = '{0}';\r\n", dp.Name));
							code.Add(string.Format(CultureInfo.InvariantCulture, "$p->type = '{0}';\r\n", ValueConvertor.OleDbTypeToPhpMySqlType(dp.Type)));
							code.Add(string.Format(CultureInfo.InvariantCulture, "$p->value = {0};\r\n", parameters[k]));
							code.Add("$ps[] = $p;\r\n//\r\n");
						}
					}
					if (outputParams.Count > 0)
					{
						StringBuilder sb2 = new StringBuilder();
						sb2.Append("SELECT ");
						for (int i = 0; i < outputParams.Count; i++)
						{
							if (i > 0)
							{
								sb2.Append(",");
							}
							sb2.Append(string.Format(CultureInfo.InvariantCulture, "@{0} AS {0} ", outputParams[i]));
						}
						string s2 = string.Format(CultureInfo.InvariantCulture, "$qry{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						code.Add(string.Format(CultureInfo.InvariantCulture, "{0} = \"{1}\";\r\n", s2, sb2.ToString()));
						code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->ExecuteWithOutputs($sql,{1},$ps,$msql);\r\n", this.Site.Name, s2));
						for (int i = 0; i < outputParams.Count; i++)
						{
							code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->{1}=$this->{0}->outputValues['{1}'];\r\n", this.Site.Name, outputParams[i]));
						}
					}
					else
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,"$this->{0}->ExecuteNonQuery($sql,$ps,$msql);\r\n", this.Site.Name));
					}
					if (!string.IsNullOrEmpty(returnReceiver))
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}=$this->{1}->errorMessage;\r\n", returnReceiver, this.Site.Name));
					}
					code.Add(string.Format(CultureInfo.InvariantCulture, "$this->SetServerComponentName('{0}');\r\n", this.Site.Name));
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			return files;
		}
		/// <summary>
		/// if it is true then the component instance declaration will be removed from the page
		/// </summary>
		/// <returns></returns>
		[NotForProgramming]
		[Browsable(false)]
		public bool DoNotCreate()
		{
			return false;
		}
		[NotForProgramming]
		[Browsable(false)]
		public void OnRequestStart(System.Web.UI.Page webPage)
		{
			_webPage = webPage as JsonWebServerProcessor;
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestStartPhp(StringCollection code)
		{

		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{

		}
		[NotForProgramming]
		[Browsable(false)]
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		/// <summary>
		/// true:component name needs to be passed into PHP constructor 
		/// </summary>
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedObjectName
		{
			get { return false; }
		}

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			DatabaseExecuter de = this as DatabaseExecuter;
			if (de != null)
			{
				ParameterDirection[] pds = de.Param_Directions;
				if (pds != null && pds.Length > 0)
				{
					DbParameterList pms = de.Parameters;
					if (pms != null && pms.Count > 0)
					{
						for (int i = 0; i < pms.Count; i++)
						{
							ParameterDirection pd = ParameterDirection.Input;
							if (pds != null && pds.Length > i)
							{
							    pd = pds[i];
							}
							if (pd == ParameterDirection.Output || pd == ParameterDirection.InputOutput)
							{
								lst.Add(new PropertyDescriptorParam(pms[i]));
							}
						}
					}
				}
			}
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (typeof(EasyUpdator).Equals(this.GetType()) || typeof(DatabaseExecuter).Equals(this.GetType()))
			{
				foreach (PropertyDescriptor p in ps)
				{
					lst.Add(p);
				}
			}
			else
			{
				bool forBrowsing = VPLUtil.GetBrowseableProperties(attributes);
				foreach (PropertyDescriptor p in ps)
				{
					if (_subClassReadOnlyProperties.Contains(p.Name))
					{
						if (forBrowsing)
						{
							object v = p.GetValue(this);
							string s;
							if (v == null)
								s = string.Empty;
							else
								s = v.ToString();
							lst.Add(new PropertyDescriptorForDisplay(this.GetType(), p.Name, s, new Attribute[] { }));
						}
					}
					else
					{
						lst.Add(p);
					}
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
	class PropertyDescriptorParam : PropertyDescriptor
	{
		private DbCommandParam _param;
		public PropertyDescriptorParam(DbCommandParam p)
			: base(p.Name, new Attribute[] {
				new WebServerMemberAttribute(),
				new ReadOnlyInProgrammingAttribute(),
				new DescriptionAttribute("Output parameter")
			})
		{
			_param = p;
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get { return typeof(DatabaseExecuter); }
		}

		public override object GetValue(object component)
		{
			return _param.Value;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return EPField.ToSystemType(_param.Type); }
		}

		public override void ResetValue(object component)
		{
			_param.Field.Value = VPLUtil.GetDefaultValue(EPField.ToSystemType(_param.Type));
		}

		public override void SetValue(object component, object value)
		{
			_param.Field.Value = value;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
