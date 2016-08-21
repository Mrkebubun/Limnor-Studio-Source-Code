/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml;
using System.Security.Cryptography;
using System.IO;
using System.Security.AccessControl;
using WindowsUtility;
using System.Globalization;
using LFilePath;
using System.Drawing.Design;
using MathExp;
using System.Reflection;
using System.Data.Common;

namespace LimnorDatabase
{
	public class ConnectionItem : ICloneable
	{
		#region fields and constructors
		public const string EXT = ".dbc";
		const string STUDIO = "Limnor Studio\\database config";
		//
		private string _filename; //configuration file name
		private string _dllpath; //driver DLL file path
		private Connection _cnn;
		private bool _connectionChecked;
		private string _name;
		private string _desc;
		private string _key;
		private string _str;
		private string _num = "!";

		private bool _loaded;

		private string[] _usedByApps;
		public ConnectionItem()
		{
			LoggingEnabled = false;
			
		}
		public ConnectionItem(Connection connect)
		{
			_cnn = connect;
			_key = GetConnectionKey(_cnn.ConnectionGuid);
			if (_cnn.DatabaseType != null && !_cnn.DatabaseType.Assembly.GlobalAssemblyCache)
			{
				_dllpath = _cnn.DatabaseType.Assembly.Location;
			}
			_loaded = true;
			LoggingEnabled = false;
		}
		public ConnectionItem(string filename)
		{
			_filename = filename;
			_key = System.IO.Path.GetFileNameWithoutExtension(_filename);
			LoggingEnabled = false;
		}
		public ConnectionItem(Guid g, string filename)
		{
			_filename = filename;
			_key = g.ToString("D");
			LoggingEnabled = false;
		}
		#endregion
		#region static functions
		public static GetObjectGuidList GetProjectDatabaseConnections;
		public static SetObjectGuidList SetProjectDatabaseConnections;
		private static Dictionary<Guid, Connection> _defaultConnections;
		public static void AddDefaultConnection(Connection connect)
		{
			if (connect != null)
			{
				if (connect.DatabaseType != null)
				{
					if (!string.IsNullOrEmpty(connect.ConnectionString))
					{
						if (connect.ConnectionGuid == Guid.Empty)
						{
							FormLog.NotifyException(true,new ExceptionLimnorDatabase("Error setting default connection: ConnectionGuid is empty"));
						}
						else
						{
							if (_defaultConnections == null)
							{
								_defaultConnections = new Dictionary<Guid, Connection>();
							}
							if (_defaultConnections.ContainsKey(connect.ConnectionGuid))
							{
								_defaultConnections[connect.ConnectionGuid] = connect;
							}
							else
							{
								_defaultConnections.Add(connect.ConnectionGuid, connect);
							}
						}
					}
				}
			}
		}
		public static Connection GetDefaultConnection(Guid id)
		{
			if (_defaultConnections != null)
			{
				if (_defaultConnections.ContainsKey(id))
				{
					return _defaultConnections[id];
				}
			}
			return null;
		}
		public static string GetConnectionFileFolder()
		{
			string folder;
			folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), STUDIO);
			if (!System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.CreateDirectory(folder);
				bool isModified = false;
				DirectoryInfo myDirectoryInfo = new DirectoryInfo(folder);
				DirectorySecurity myDirectorySecurity = myDirectoryInfo.GetAccessControl();
				AccessRule rule = new FileSystemAccessRule("Users", FileSystemRights.Write | FileSystemRights.ReadAndExecute | FileSystemRights.Modify, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
				myDirectorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out isModified);
				myDirectoryInfo.SetAccessControl(myDirectorySecurity);
				//backward compatability:move *.dbc,connections.xml, and dbConnects.xml from uplevel to this folder
				string oldFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Limnor Studio");
				string[] ss = System.IO.Directory.GetFiles(oldFolder, "connections.xml");
				if (ss != null && ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							try
							{
								System.IO.File.Move(ss[i], System.IO.Path.Combine(folder, System.IO.Path.GetFileName(ss[i])));
							}
							catch
							{
							}
						}
					}
				}
				ss = System.IO.Directory.GetFiles(oldFolder, "dbConnects.xml");
				if (ss != null && ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							try
							{
								System.IO.File.Move(ss[i], System.IO.Path.Combine(folder, System.IO.Path.GetFileName(ss[i])));
							}
							catch
							{
							}
						}
					}
				}
				ss = System.IO.Directory.GetFiles(oldFolder, "*.dbc");
				if (ss != null && ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							try
							{
								System.IO.File.Move(ss[i], System.IO.Path.Combine(folder, System.IO.Path.GetFileName(ss[i])));
							}
							catch
							{
							}
						}
					}
				}
			}
			return folder;
		}
		public static string GetConnectionKey(Guid g)
		{
			return g.ToString("N", CultureInfo.InvariantCulture);
		}
		public static string GetConnectionFilename(Guid g)
		{
			string dir;
			if (EasyQuery.ApplicationLocation == EnumAppLocation.Server)
			{
				dir = EasyQuery.CodeBaseFolder;
			}
			else
			{
				dir = GetConnectionFileFolder();
			}
			string f = System.IO.Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, "{0}{1}", GetConnectionKey(g), EXT));
			if (System.IO.File.Exists(f))
			{
				return f;
			}
			else
			{
				string f0 = System.IO.Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", GetConnectionKey(g), EXT));
				if (File.Exists(f0))
				{
					try
					{
						File.Copy(f0, f);
					}
					catch
					{
					}
				}
			}
			return f;
		}

		public static string[] GetConnectionFiles()
		{
			return System.IO.Directory.GetFiles(GetConnectionFileFolder(), "*" + EXT); //EXT = .dbc
		}
		public static ConnectionItem LoadConnection(string dbcFile)
		{
			if (System.IO.File.Exists(dbcFile))
			{
				try
				{
					ConnectionItem ci = XmlSerializerUtility.LoadFromXmlFile<ConnectionItem>(dbcFile);
					ci.SetLoaded();
					return ci;
				}
				catch (Exception err)
				{
					FormLog.NotifyException(true,err, "Invalid connection file [{0}]", dbcFile);
				}
			}
			return null;
		}
		public static ConnectionItem LoadConnection(Guid g, bool updateUsage, bool quickLoad)
		{
			ConnectionItem ci = null;
			try
			{
				string f = GetConnectionFilename(g);
				if (!System.IO.File.Exists(f))
				{
					f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
						System.IO.Path.GetFileName(f));
				}
				if (System.IO.File.Exists(f))
				{
					try
					{
						ci = XmlSerializerUtility.LoadFromXmlFile<ConnectionItem>(f);
						ci.SetDataFolder();
						ci.SetLoaded();
						if (updateUsage)
						{
							ci.UpdateUsage(Application.ExecutablePath);
						}
					}
					catch (Exception errXml)
					{
						FormLog.NotifyException(true,errXml, "Invalid connection file [{0}] for connection item {1}.", f, g);
					}
				}
				if (quickLoad)
				{
					if (ci == null)
					{
						Connection cn = GetDefaultConnection(g);
						if (cn == null)
						{
							ci = new ConnectionItem(f);
						}
						else
						{
							ci = new ConnectionItem(cn);
						}
						if (updateUsage)
						{
							ci.UpdateUsage(Application.ExecutablePath);
						}
						ci.Save();
						ci.SetLoaded();
					}
					else
					{
						ci.setConnectionID(g);
					}
				}
				else
				{
					if (ci == null || !ci.ConnectionObject.IsConnectionReady)
					{
						Connection cn = GetDefaultConnection(g);
						if (cn == null)
						{
							if (ci == null)
							{
								ci = new ConnectionItem(f);
							}
						}
						else
						{
							ci = new ConnectionItem(cn);
						}
						if (updateUsage)
						{
							ci.UpdateUsage(Application.ExecutablePath);
						}
						ci.Save();
						ci.SetLoaded();
						ci._connectionChecked = true;
					}
				}
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err, "Error loading connection item {0}.", g);
			}
			return ci;
		}
		#endregion
		#region Serializable properties
		[Browsable(false)]
		public string DatabaseType
		{
			get
			{
				if (ConnectionObject.DatabaseType != null)
				{
					return ConnectionObject.DatabaseType.AssemblyQualifiedName;
				}
				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					ConnectionObject.DatabaseType = Type.GetType(value);
				}
			}
		}
		
		[Browsable(false)]
		public string DllPath
		{
			get
			{
				try
				{
					Type dbType = ConnectionObject.DatabaseType;
					if (dbType != null)
					{
						if (dbType.Assembly.GlobalAssemblyCache)
						{
							_dllpath = string.Empty;
						}
						else
						{
							_dllpath = dbType.Assembly.Location;
						}
					}
				}
				catch
				{
				}
				return _dllpath;
			}
			set
			{
				_dllpath = value;
				if (!string.IsNullOrEmpty(_dllpath))
				{
					if (File.Exists(_dllpath))
					{
						if (_cnn != null && _cnn.DatabaseType == null)
						{
							VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(_dllpath));
							Assembly a = Assembly.LoadFrom(_dllpath);
							if (a != null)
							{
								Type[] tps = a.GetExportedTypes();
								if (tps != null && tps.Length > 0)
								{
									for (int i = 0; i < tps.Length; i++)
									{
										if (typeof(DbConnection).IsAssignableFrom(tps[i]))
										{
											if (!tps[i].IsAbstract)
											{
												_cnn.DatabaseType = tps[i];
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public string ConnectionString
		{
			get
			{
				_str = ConnectionObject.ConnectionString;
				if (!string.IsNullOrEmpty(_str))
				{
					if (EncryptConnectionString)
					{
						if (!_str.StartsWith(_num, StringComparison.Ordinal))
						{
							_str = string.Format(CultureInfo.InvariantCulture, "{0}{1}", _num, get_pagenumber(_str));
						}
					}
					else if (!SavePasswordInConnectionString)
					{
						_str = ConnectionStringSelector.RemovePasswords(_str);
					}
				}
				return _str;
			}
			set
			{
				_str = value;
				ConnectionObject.ConnectionString = _str;
			}
		}
		/// <summary>
		/// for connection
		/// </summary>
		[Browsable(false)]
		public string ConnectionStringPlain
		{
			get
			{
				string s = ConnectionObject.ConnectionString;
				return s;
			}
		}
		[Browsable(false)]
		public EnumTopRecStyle TopRecordStyle
		{
			get
			{
				return ConnectionObject.TopRecStyle;
			}
		}
		[Browsable(false)]
		public EnumParameterStyle ParameterStyle
		{
			get
			{
				return ConnectionObject.ParameterStyle;
			}
			set
			{
				ConnectionObject.ParameterStyle = value;
			}
		}
		[Browsable(false)]
		public EnumQulifierDelimiter NameDelimiterStyle
		{
			get
			{
				return ConnectionObject.NameDelimiterStyle;
			}
			set
			{
				ConnectionObject.NameDelimiterStyle = value;
			}
		}
		[Browsable(false)]
		public string NameDelimiterBegin
		{
			get
			{
				return DatabaseEditUtil.SepBegin(NameDelimiterStyle);
			}
		}
		[Browsable(false)]
		public string NameDelimiterEnd
		{
			get
			{
				return DatabaseEditUtil.SepEnd(NameDelimiterStyle);
			}
		}
		[Browsable(false)]
		public bool EncryptConnectionString
		{
			get
			{
				return ConnectionObject.EncryptConnectionString;
			}
			set
			{
				ConnectionObject.EncryptConnectionString = value;
			}
		}
		[Browsable(false)]
		public bool SavePasswordInConnectionString
		{
			get
			{
				return ConnectionObject.SavePasswordInConnectionString;
			}
			set
			{
				ConnectionObject.SavePasswordInConnectionString = value;
			}
		}
		[Browsable(false)]
		public bool LowerCaseSqlKeywords
		{
			get
			{
				return ConnectionObject.LowerCaseSqlKeywords;
			}
			set
			{
				ConnectionObject.LowerCaseSqlKeywords = value;
			}
		}
		/// <summary>
		/// return _key, the guid string
		/// </summary>
		[Browsable(false)]
		[Description("Connection file name")]
		public string Filename
		{
			get
			{
				if (string.IsNullOrEmpty(_key))
				{
					if (!string.IsNullOrEmpty(_filename))
					{
						_key = System.IO.Path.GetFileNameWithoutExtension(_filename);
					}
				}
				return _key;
			}
			set
			{
				_key = value;
				if (!string.IsNullOrEmpty(_key))
				{
					_filename = System.IO.Path.Combine(GetConnectionFileFolder(), _key + EXT);
					ConnectionObject.ConnectionGuid = new Guid(_key);
				}
			}
		}
		[Description("Connection name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[Description("Connection description")]
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}
		
		[Browsable(false)]
		public string[] UsedByApplications
		{
			get
			{
				if (_usedByApps == null)
				{
					_usedByApps = new string[] { };
				}
				return _usedByApps;
			}
			set
			{
				_usedByApps = value;
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public bool ConnectionChecked
		{
			get
			{
				return _connectionChecked;
			}
		}
		[Browsable(false)]
		public string ConnectionStringDisplay
		{
			get
			{
				if (_cnn == null)
				{
					return "";
				}
				return _cnn.ConnectionString_;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether database activities should be recorded in a log file represented by LogFilePath.")]
		public bool LoggingEnabled
		{
			get;
			set;
		}
		[FilePath("Log file|*.log;*.txt", "Select database log file", true)]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Gets and sets a full file path for a log file. Database activities will be recorded in the log file if LoggingEnabled is True.")]
		public string LogFilePath
		{
			get;
			set;
		}
		[Description("Gets a guid identifying the connection")]
		public Guid ConnectionGuid
		{
			get
			{
				if (_cnn == null)
				{
					if (!string.IsNullOrEmpty(_key))
					{
						return new Guid(_key);
					}
					if (!string.IsNullOrEmpty(_filename))
					{
						_key = System.IO.Path.GetFileNameWithoutExtension(_filename);
						return new Guid(_key);
					}
					return Guid.Empty;
				}
				return _cnn.ConnectionGuid;
			}
		}
		[Description("Gets an object representing the database connection")]
		public Connection ConnectionObject
		{
			get
			{
				if (_cnn == null)
				{
					if (string.IsNullOrEmpty(_key))
					{
						if (!string.IsNullOrEmpty(_filename))
						{
							_key = System.IO.Path.GetFileNameWithoutExtension(_filename);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(_filename))
						{
							_filename = GetConnectionFilename(new Guid(_key));
						}
					}
					if (!string.IsNullOrEmpty(_filename))
					{
						if (System.IO.File.Exists(_filename))
						{
							try
							{
								object obj = XmlSerializerUtility.LoadFromXmlFile(_filename);
								if (obj != null)
								{
									_cnn = obj as Connection;
								}
								if (_cnn == null)
								{
								}
								else
								{
									_cnn.SetDataFolder();
								}
							}
							catch (Exception err)
							{
								FormLog.NotifyException(true,err, "Error loading database connection");
							}
						}
					}
					if (_cnn == null)
					{
						if (!string.IsNullOrEmpty(_key))
						{
							_cnn = GetDefaultConnection(new Guid(_key));
						}
						if (_cnn == null)
						{
							//create a new Connection
							_cnn = new Connection();
							if (!string.IsNullOrEmpty(_key))
							{
								_cnn.ConnectionGuid = new Guid(_key);
							}
							else
							{
								_cnn.ConnectionGuid = Guid.NewGuid();
								_key = GetConnectionKey(_cnn.ConnectionGuid);
							}
							_filename = GetConnectionFilename(_cnn.ConnectionGuid);
						}
					}
				}
				return _cnn;
			}
		}
		[Browsable(false)]
		public string FullFilePath
		{
			get
			{
				return _filename;
			}
		}
		[Browsable(false)]
		public string CommonProgramDataFolder
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			}
		}
		[Browsable(false)]
		public bool HasConfiguration
		{
			get
			{
				if (ConnectionObject.DatabaseType != null)
				{
					return true;
				}
				if (!string.IsNullOrEmpty(ConnectionObject.ConnectionString))
				{
					return true;
				}
				return false;
			}
		}
		[Description("Gets a value indicating whether all the parameters for the database connection are specified. It does not actually test the connection to verify the parameters.")]
		public bool IsValid
		{
			get
			{
				if (ConnectionObject.DatabaseType == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Connection's DatabaseType is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				if (string.IsNullOrEmpty(ConnectionObject.ConnectionString))
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Connection's connection string is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				string s = ConnectionObject.ConnectionString;
				if (ConnectionObject.IsJet)
				{
					string file = ConnectionStringSelector.GetDataSource(s);
					if (string.IsNullOrEmpty(file))
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MS Access missing file for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
						return false;
					}
					if (!System.IO.File.Exists(file))
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MS Access file not exist [{2}] for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, file);
						return false;
					}
				}
				return true;
			}
		}
		#endregion
		#region Methods
		public void CheckConnection()
		{
			if (!_connectionChecked)
			{
				_connectionChecked = true;
				if (!ConnectionObject.IsConnectionReady)
				{
					Guid g = new Guid(_key);
					_cnn = GetDefaultConnection(g);
					Save();
					SetLoaded();
				}
			}
		}
		public void Log(string message, params object[] values)
		{
			if (LoggingEnabled)
			{
				if (!string.IsNullOrEmpty(this.LogFilePath))
				{
					string msg;
					try
					{
						if (values != null && values.Length > 0)
						{
							msg = string.Format(message, values);
						}
						else
						{
							msg = message;
						}
					}
					catch (Exception e)
					{
						msg = ExceptionLimnorDatabase.FormExceptionText(e) + " " + message;
					}
					if (!string.IsNullOrEmpty(msg))
					{
						StreamWriter sw = null;
						try
						{
							sw = new StreamWriter(LogFilePath, true, Encoding.ASCII);
							sw.Write(DateTime.Now.ToString("u"));
							sw.Write(" - ");
							sw.WriteLine(msg);
						}
						catch (Exception er)
						{
							FormLog.ShowMessage(ExceptionLimnorDatabase.FormExceptionText(er));
						}
						finally
						{
							if (sw != null)
							{
								sw.Close();
							}
						}
					}
				}
			}
		}
		public void SetLoaded()
		{
			_loaded = true;
		}
		public void SetConnectionChecked()
		{
			_connectionChecked = true;
		}
		public void setConnectionID(Guid g)
		{
			_key = g.ToString("D");
		}
		public void SetDataFolder()
		{
			if (!string.IsNullOrEmpty(_str))
			{
				_str = _str.Replace(Filepath.FOLD_CPMMPMAPPDATA, this.CommonProgramDataFolder);
			}
			if (_cnn != null)
			{
				_cnn.SetDataFolder();
			}
		}
		public void MergeFromFile()
		{
			string f = GetConnectionFilename(this.ConnectionGuid);
			if (System.IO.File.Exists(f))
			{
				ConnectionItem ci = XmlSerializerUtility.LoadFromXmlFile<ConnectionItem>(f);
				if (ci != null)
				{
					ci.SetDataFolder();
					if (!string.IsNullOrEmpty(ci._name))
					{
						_name = ci._name;
					}
					if (ci._cnn != null)
					{
						_cnn = ci._cnn;
					}
				}
			}
		}
		public string GetDataSource()
		{
			return ConnectionObject.GetDataSource();
		}
		public void MergaeUsage(string[] apps)
		{
			if (apps != null && apps.Length > 0)
			{
				if (_usedByApps != null && _usedByApps.Length > 0)
				{
					StringCollection sc = new StringCollection();
					sc.AddRange(_usedByApps);
					for (int i = 0; i < apps.Length; i++)
					{
						bool bExist = false;
						for (int k = 0; k < _usedByApps.Length; k++)
						{
							if (string.Compare(apps[i], _usedByApps[k], StringComparison.OrdinalIgnoreCase) == 0)
							{
								bExist = true;
								break;
							}
						}
						if (!bExist)
						{
							sc.Add(apps[i]);
						}
					}
					_usedByApps = new string[sc.Count];
					sc.CopyTo(_usedByApps, 0);
				}
				else
				{
					_usedByApps = apps;
				}
			}
		}
		public void UpdateUsage(string app)
		{
			if (!app.EndsWith("ConnectionManager.exe", StringComparison.OrdinalIgnoreCase))
			{
				if (!app.EndsWith("devenv.exe", StringComparison.OrdinalIgnoreCase))
				{
					if (!app.EndsWith("limnorMain.exe", StringComparison.OrdinalIgnoreCase))
					{
						bool newApp = true;
						if (_usedByApps != null && _usedByApps.Length > 0)
						{
							for (int i = 0; i < _usedByApps.Length; i++)
							{
								if (string.Compare(app, _usedByApps[i], StringComparison.OrdinalIgnoreCase) == 0)
								{
									newApp = false;
									break;
								}
							}
						}
						if (newApp)
						{
							int n = 0;
							if (_usedByApps != null)
							{
								n = _usedByApps.Length;
							}
							string[] a = new string[n + 1];
							if (n > 0)
							{
								_usedByApps.CopyTo(a, 0);
							}
							a[n] = app;
							_usedByApps = a;
							Save();
						}
					}
				}
			}
		}
		public void ReloadFromCache()
		{
			if (!string.IsNullOrEmpty(_key))
			{
				Guid g = new Guid(_key);
				Connection cn = GetDefaultConnection(g);
				if (cn != null)
				{
					if (cn.IsConnectionReady)
					{
						_cnn = cn;
					}
				}
			}
		}
		/// <summary>
		/// used by setup
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="targetConfig"></param>
		public void SaveDataSource(string filename, string targetConfig)
		{
			ConnectionObject.SetDataSource(filename);
			List<Type> types = new List<Type>();
			XmlDocument doc = VPL.XmlSerializerUtility.Save(this, types);
			doc.Save(targetConfig);
		}
		public void Save()
		{
			if (!_loaded)
			{
			}
			try
			{
				int c = 0x20;
				List<Type> types = new List<Type>();
				XmlDocument doc = VPL.XmlSerializerUtility.Save(this, types);
				doc.Save(ConnectionItem.GetConnectionFilename(ConnectionObject.ConnectionGuid));
				if (EncryptConnectionString)
				{
					if (!ConnectionObject.ConnectionString.StartsWith(new string(new char[] { (char)(c + 1) }), StringComparison.Ordinal))
					{
						ConnectionConfig.SetConnection(ConnectionObject.ConnectionGuid, new string(new char[] { (char)(c + 1) }) + get_pagenumber(ConnectionObject.ConnectionString));
					}
					else
					{
						ConnectionConfig.SetConnection(ConnectionObject.ConnectionGuid, ConnectionObject.ConnectionString);
					}
				}
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true,err, "Error saving connection {0}", this.ToString());
			}
		}
		public bool SetCredentialByUI(Form caller)
		{
			return ConnectionObject.SetCredentialByUI(caller, ToString());
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", Name, Filename);
		}
		#endregion
		#region private methods
		private string get_pagenumber(string pagenumber)
		{
			try
			{
				TripleDESCryptoServiceProvider p = new TripleDESCryptoServiceProvider();
				MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
				byte[] b2;
				p.Key = m5.ComputeHash(UnicodeEncoding.Unicode.GetBytes(this.GetType().Name));
				p.Mode = CipherMode.ECB;
				b2 = UnicodeEncoding.Unicode.GetBytes(pagenumber);
				pagenumber = Convert.ToBase64String(p.CreateEncryptor().TransformFinalBlock(b2, 0, b2.Length));
				m5 = null;
				p = null;
				return pagenumber;
			}
			catch (Exception err)
			{
				return err.Message;
			}
		}
		#endregion
		#region ICloneable Members
		[Description("Make a clone of this object")]
		public object Clone()
		{
			ConnectionItem obj = new ConnectionItem();
			obj._desc = _desc;
			obj._filename = _filename;
			obj._key = _key;
			obj._name = _name;
			if (_cnn != null)
			{
				obj._cnn = (Connection)_cnn.Clone();
			}
			return obj;
		}

		#endregion
	}
}
