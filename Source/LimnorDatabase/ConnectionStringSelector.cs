/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using WindowsUtility;
using System.Globalization;
using VPL;

namespace LimnorDatabase
{
	public enum EnumConnectionType { OleDb, SqlServer, Odbc, Other }
	public class ConnectionStringSelector : UITypeEditor
	{
		#region constants

		const string ACCESS_USERPASS = "Password";
		const string ACCESS_DBPASS = "Jet OLEDB:Database Password";
		const string ACCESS_MODE = "Mode";
		//
		const string DB_DATASOURCE = "Data Source";
		const string DB_USERID = "User ID";
		//
		const string DB_USER = "User";
		//
		const string SQL_SERVER = "Server";
		const string SQL_Database = "Database";
		const string SQL_USERID = "UID";
		const string SQL_USERPWD = "PWD";
		const string SQL_USERPASS = "Password";
		const string SQL_Catalog = "Initial Catalog";
		const string SQL_AuthenMode1 = "Trusted_Connection";
		const string SQL_AuthenMode2 = "Integrated Security";
		#endregion

		#region Constructors
		public ConnectionStringSelector()
		{
		}
		#endregion

		#region MS SQL

		public static string SetCredentialToMsSqlServerConnectionString(string connectionString, string user, string userPassword)
		{
			string s = connectionString;
			ConnectionStringSelector.InsertValue(ref s, SQL_USERID, user);
			ConnectionStringSelector.InsertValue(ref s, SQL_USERPASS, userPassword);
			return s;
		}
		public static string GetServerName(string connectionString)
		{
			return GetItemInString(connectionString, SQL_SERVER);
		}
		public static string GetDatabaseName(string connectionString)
		{
			return GetItemInString(connectionString, SQL_Database);
		}
		public static string GetCatalogName(string connectionString)
		{
			return GetItemInString(connectionString, SQL_Catalog);
		}
		public static string GetSQLUser(string connectionString)
		{
			return GetItemInString(connectionString, SQL_USERID);
		}
		public static string GetSQLUserPassword(string connectionString)
		{
			string pwd = GetItemInString(connectionString, SQL_USERPASS);
			if (string.IsNullOrEmpty(pwd))
			{
				pwd = GetItemInString(connectionString, SQL_USERPWD);
			}
			return pwd;
		}
		public static bool IsTrustedSQLServerConnection(string connectionString)
		{
			string au = GetItemInString(connectionString, SQL_AuthenMode1);
			if (!string.IsNullOrEmpty(au))
			{
				if (string.Compare(au, "yes", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				if (string.Compare(au, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			else
			{
				au = GetItemInString(connectionString, SQL_AuthenMode2);
				if (!string.IsNullOrEmpty(au))
				{
					if (string.Compare(au, "SSPI", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		#region Database
		public static void SetDataSource(ref string connectionString, string dataSource)
		{
			ConnectionStringSelector.InsertValue(ref connectionString, DB_DATASOURCE, dataSource);
		}
		public static string GetDataSource(string connectionString)
		{
			return GetItemInString(connectionString, DB_DATASOURCE);
		}
		public static string GetUser(string connectionString)
		{
			return GetItemInString(connectionString, DB_USERID);
		}
		#endregion
		#region MS Access
		public static string MakeAccessConnectionString(string file, bool bReadOnly, bool bExclusive, string dbPass, string user, string userPass)
		{
			StringBuilder sb = new StringBuilder("Provider=Microsoft.Jet.OLEDB.4.0;");
			if (!string.IsNullOrEmpty(userPass))
			{
				sb.Append("Password=;");
			}
			if (!string.IsNullOrEmpty(user))
			{
				sb.Append("User ID=");
				sb.Append(user);
				sb.Append(";");
			}
			sb.Append("Data Source=");
			sb.Append(file);

			if (bExclusive)
				sb.Append(";Mode=Share Deny Read|Share Deny Write;");
			else
			{
				if (bReadOnly)
					sb.Append(";Mode=Read;");
				else
					sb.Append(";Mode=ReadWrite;");
			}
			if (!string.IsNullOrEmpty(dbPass))
			{
				sb.Append("Jet OLEDB:Database Password=");
				sb.Append(dbPass);
				sb.Append(";");
			}
			return sb.ToString();
		}
		public static string SetCredentialToAccessConnectionString(string connectionString, string user, string userPassword, string databasePassword)
		{
			string s = connectionString;
			string file = GetDataSource(s);
			if (!string.IsNullOrEmpty(file))
			{
				file = file.Replace(Filepath.FOLD_CPMMPMAPPDATA, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
				if (!System.IO.File.Exists(file))
				{
					string fn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), System.IO.Path.GetFileName(file));
					if (System.IO.File.Exists(fn))
					{
						file = fn;
						ConnectionStringSelector.InsertValue(ref s, DB_DATASOURCE, file);
					}
				}
			}
			ConnectionStringSelector.InsertValue(ref s, DB_USERID, user);
			ConnectionStringSelector.InsertValue(ref s, ACCESS_USERPASS, userPassword);
			if (!string.IsNullOrEmpty(databasePassword))
			{
				ConnectionStringSelector.InsertValue(ref s, ACCESS_DBPASS, databasePassword);
			}
			return s;
		}
		public static string SetCredentialToFirebirdConnectionString(string connectionString, string user, string userPassword)
		{
			string s = connectionString;
			string file = GetDatabaseName(s);
			if (!string.IsNullOrEmpty(file))
			{
				file = file.Replace(Filepath.FOLD_CPMMPMAPPDATA, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
				if (!System.IO.File.Exists(file))
				{
					string fn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), System.IO.Path.GetFileName(file));
					if (System.IO.File.Exists(fn))
					{
						file = fn;
						ConnectionStringSelector.InsertValue(ref s, SQL_Database, file);
					}
				}
			}
			if (!string.IsNullOrEmpty(user))
			{
				ConnectionStringSelector.InsertValue(ref s, DB_USER, user);
			}
			if (!string.IsNullOrEmpty(userPassword))
			{
				ConnectionStringSelector.InsertValue(ref s, ACCESS_USERPASS, userPassword);
			}
			return s;
		}
		public static string RemovePasswords(string connectionString)
		{
			ConnectionStringSelector.InsertValue(ref connectionString, ACCESS_USERPASS, null);
			ConnectionStringSelector.InsertValue(ref connectionString, ACCESS_DBPASS, null);
			return connectionString;
		}


		public static string GetAccessUserPassword(string connectionString)
		{
			return GetItemInString(connectionString, ACCESS_USERPASS);
		}
		public static string GetAccessDatabasePassword(string connectionString)
		{
			return GetItemInString(connectionString, ACCESS_DBPASS);
		}

		public static string GetConnectionMode(string connectionString)
		{
			return GetItemInString(connectionString, ACCESS_MODE);
		}
		#endregion

		#region string utility
		public static string GetItemInString(string sConnect, string name)
		{
			string sRet = "";
			int n = GetNamePos(sConnect, name);
			if (n >= 0)
			{
				n += name.Length + 1;
				int n2 = sConnect.IndexOf(";", n, StringComparison.OrdinalIgnoreCase);
				if (n2 > n)
				{
					sRet = sConnect.Substring(n, n2 - n);
				}
				else if (n2 < n)
				{
					sRet = sConnect.Substring(n);
				}
			}
			return sRet;
		}
		public static int GetNamePos(string s, string name)
		{
			if (string.IsNullOrEmpty(s))
			{
				return -1;
			}
			int i;
			name += "=";
			int n = s.IndexOf(name, 0, StringComparison.OrdinalIgnoreCase);
			while (n > 0)
			{
				for (i = n - 1; i > 0; i--)
				{
					if (s[i] != ' ')
					{
						if (s[i] == ';')
							return n;
						else
						{
							n = s.IndexOf(name, n + 1, StringComparison.OrdinalIgnoreCase);
							break;
						}
					}
				}
			}
			return n;
		}
		public static void InsertValue(ref string s, string name, string v)
		{
			int n = GetNamePos(s, name);
			if (n >= 0)
			{
				n += name.Length + 1;
				int n2 = s.IndexOf(";", n, StringComparison.OrdinalIgnoreCase);
				if (n2 >= n)
				{
					string s1 = s.Substring(0, n);
					string s2 = s.Substring(n2);
					s = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", s1, v, s2);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(v))
				{
					//s += name + "=" + v + ";";
					s = s.TrimEnd();
					if (s.EndsWith(";", StringComparison.OrdinalIgnoreCase))
					{
						s = string.Format(CultureInfo.InvariantCulture,
							"{0}{1}={2};", s, name, v);
					}
					else
					{
						s = string.Format(CultureInfo.InvariantCulture,
							"{0};{1}={2};", s, name, v);
					}
				}
			}
		}
		#endregion

		#region Connection utility
		public static EnumConnectionType GetConnectionType(DbConnection connection)
		{
			if (connection == null)
				return EnumConnectionType.Other;
			if (connection is OleDbConnection)
				return EnumConnectionType.OleDb;
			if (connection is SqlConnection)
				return EnumConnectionType.SqlServer;
			if (connection is OdbcConnection)
				return EnumConnectionType.Odbc;
			return EnumConnectionType.Other;
		}
		#endregion

		#region UITypeEditor
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					Connection cnn = context.Instance as Connection;
					if (cnn != null)
					{
						if (cnn.DatabaseType != null)
						{
							if (typeof(OleDbConnection).Equals(cnn.DatabaseType))
							{
								dlgDBAccess dlg = new dlgDBAccess();
								dlg.SetForDatabaseEdit();
								dlg.LoadData(cnn.ConnectionString);
								if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
								{
									cnn.SetCredential0(dlg.sUser, dlg.sPass, dlg.sDBPass);
									return dlg.sConnectionString;
								}
							}
							if (typeof(SqlConnection).Equals(cnn.DatabaseType))
							{
								dlgSQLServer dlg = new dlgSQLServer();
								dlg.LoadData(cnn.ConnectionString);
								if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
								{
									cnn.SetCredential(dlg.sDBUser, dlg.sPassword, null);
									return dlg.sConnectionString;
								}
							}
							else
							{
								DlgText dlg = new DlgText();
								dlg.LoadData(cnn.ConnectionString, "Specify connection string");
								if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
								{
									return dlg.GetText();
								}
							}
						}
						else
						{
							MessageBox.Show("DatabaseType has not been set", "Connection string", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
				}
			}
			return value;
		}
		#endregion
	}
}
