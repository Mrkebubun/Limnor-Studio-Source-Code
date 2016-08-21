/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Limnor.WebServerBuilder;
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using System.Web.UI;
using WebServerProcessor;
using System.Globalization;
using System.Data.Common;
using System.Security.Cryptography;
using System.Web;
using VPL;
using System.Reflection;
using System.Xml;
using WindowsUtility;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace LimnorDatabase
{
	[global::System.ComponentModel.ToolboxItem(true)]
	[Description("Use this component to manage user credentials so that web pages may restrict accessing to authenticated users only.")]
	[ToolboxBitmapAttribute(typeof(WebLoginManager), "Resources.key.bmp")]
	public class WebLoginManager : IComponent, IDatabaseAccess, IWebServerProgrammingSupport, IWebServerComponentCreator, ISingleton, IFieldsHolder
	{
		#region fields and contructors
		private EasyQuery _qry;
		private JsonWebServerProcessor jsp;
		private StringCollection _debugLogs;
		private Guid _guid;

		private string _componentName;
		private string[] _fieldnames;
		//
		private string _loginName;
		private int _loginID;
		private int _userLevel;

		public WebLoginManager()
		{
			_qry = new EasyQuery();
			_debugLogs = new StringCollection();
		}
		public WebLoginManager(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			_qry = new EasyQuery();
			_debugLogs = new StringCollection();
		}
		#endregion

		#region private methods
		private void showFailMsg(string sInfo)
		{
			if (jsp != null)
			{
				if (!string.IsNullOrEmpty(this.FailedMessageLableId))
				{
					if (string.IsNullOrEmpty(sInfo))
					{
						jsp.AddClientScript("JsonDataBinding.SetInnerText({0},'');", this.FailedMessageLableId);
					}
					else
					{
						jsp.AddClientScript("JsonDataBinding.SetInnerText({0},'{1}');", this.FailedMessageLableId, sInfo.Replace("'", "\'"));
					}
				}
			}
			else
			{
				Label lb = this.LabelToShowLoginFailedMessage as Label;
				if (lb != null)
				{
					lb.Text = sInfo;
				}
			}
		}
		private void showErrorMsg(string msg)
		{
			if (jsp != null)
			{
				jsp.LogDebugInfo(msg);
			}
			_debugLogs.Add(msg);
			showFailMsg(msg);
		}
		private bool checkLogin(string loginName, string password, StringBuilder info)
		{
			bool bPassed = false;
			string sSQL;
			DbCommand cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
			_userLevel = -1;
			_loginName = string.Empty;
			_loginID = 0;
			if (this.DatabaseConnection.ConnectionObject.IsMySql)
			{
				if (string.IsNullOrEmpty(this.UserAccountLevelFieldName))
				{
					if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
							"SELECT {0}{2}{1}, PASSWORD(@c1) FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountPasswordFieldName,
							this.UserAccountTableName,
							this.UserAccountLoginFieldName
							);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
							"SELECT {0}{2}{1}, PASSWORD(@c1),{0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountPasswordFieldName,
							this.UserAccountTableName,
							this.UserAccountLoginFieldName,
							this.UserAccountIdFieldName
							);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
							"SELECT {0}{2}{1}, PASSWORD(@c1), {0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountPasswordFieldName,
							this.UserAccountTableName,
							this.UserAccountLoginFieldName,
							this.UserAccountSaltFieldName
							);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
							"SELECT {0}{2}{1}, PASSWORD(@c1), {0}{5}{1}, {0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountPasswordFieldName,
							this.UserAccountTableName,
							this.UserAccountLoginFieldName,
							this.UserAccountSaltFieldName,
							this.UserAccountIdFieldName
							);
						}
					}
				}
				else
				{
					if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1}, PASSWORD(@c1),{0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName
								);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},PASSWORD(@c1),{0}{5}{1}, {0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1}, PASSWORD(@c1),{0}{5}{1},{0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountSaltFieldName
								);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},PASSWORD(@c1),{0}{5}{1},{0}{6}{1}, {0}{7}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c2",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountSaltFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
				}
				if (jsp != null)
				{
					jsp.LogDebugInfo("SQL:{0}", sSQL);
					jsp.LogDebugInfo("MySQL  @c1:{0}  @c2:{1} ", password, loginName);
				}
				cmd.CommandText = sSQL;
				DbParameter p = cmd.CreateParameter();
				p = cmd.CreateParameter();
				p.ParameterName = "@c1";
				p.DbType = System.Data.DbType.String;
				p.Value = password;
				cmd.Parameters.Add(p);
				p = cmd.CreateParameter();
				p = cmd.CreateParameter();
				p.ParameterName = "@c2";
				p.DbType = System.Data.DbType.String;
				p.Value = loginName;
				cmd.Parameters.Add(p);
				this.QueryDef.DatabaseConnection.ConnectionObject.Open();
				DbDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
				if (dr.Read())
				{
					string mysqlPass = string.Empty;
					string passwordhashInDB = ValueConvertor.ToString(dr[0]);
					if (!string.IsNullOrEmpty(passwordhashInDB))
					{
						string salt = string.Empty;
						mysqlPass = ValueConvertor.ToString(dr[1]);
						if (string.IsNullOrEmpty(this.UserAccountLevelFieldName))
						{
							if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
							{
								if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
								{

								}
								else
								{
									_loginID = ValueConvertor.ToInt(dr[2]);
								}
							}
							else
							{
								salt = ValueConvertor.ToString(dr[2]);
								if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
								{
								}
								else
								{
									_loginID = ValueConvertor.ToInt(dr[3]);
								}
							}
						}
						else
						{
							_userLevel = ValueConvertor.ToInt(dr[2]);
							if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
							{
								if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
								{
								}
								else
								{
									_loginID = ValueConvertor.ToInt(dr[3]);
								}
							}
							else
							{
								salt = ValueConvertor.ToString(dr[3]);
								if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
								{
								}
								else
								{
									_loginID = ValueConvertor.ToInt(dr[4]);
								}
							}
						}
						string passwordHashInput = WinUtil.GetHash(password, salt, this.PasswordHash);
						bPassed = (string.CompareOrdinal(passwordhashInDB, passwordHashInput) == 0);
						if (!bPassed)
						{
							//backwards compatibility: check mysqlPass
							bPassed = (string.CompareOrdinal(passwordhashInDB, mysqlPass) == 0);
						}
						if (!bPassed)
						{
							info.Append("log in failed. result:[]");
							info.Append(passwordhashInDB);
							info.Append(",");
							info.Append(passwordHashInput);
							info.Append(",");
							info.Append(mysqlPass);
							info.Append("]");
						}
						else
						{
							_loginName = loginName;
						}
					}
					else
					{
						info.Append("log in failed. result:[]");
					}
				}
				else
				{
					info.Append("log in failed. invalid user:[");
					info.Append(loginName);
					info.Append("]");
				}
				dr.Close();
			}
			else
			{
				if (string.IsNullOrEmpty(this.UserAccountLevelFieldName))
				{
					if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},{0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1}, {0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountSaltFieldName
								);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1}, {0}{5}{1}, {0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountSaltFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
				}
				else
				{
					if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},{0}{5}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName
								);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},{0}{5}{1},{0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},{0}{5}{1},{0}{6}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountSaltFieldName
								);
						}
						else
						{
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"SELECT {0}{2}{1},{0}{5}{1},{0}{6}{1},{0}{7}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
								this.QueryDef.NameDelimiterBegin,
								this.QueryDef.NameDelimiterEnd,
								this.UserAccountPasswordFieldName,
								this.UserAccountTableName,
								this.UserAccountLoginFieldName,
								this.UserAccountLevelFieldName,
								this.UserAccountSaltFieldName,
								this.UserAccountIdFieldName
								);
						}
					}
				}
				if (jsp != null)
				{
					jsp.LogDebugInfo("SQL:{0}", sSQL);
					jsp.LogDebugInfo("  @c1:{0}  @c2:{1} ", loginName, password);
				}
				string pn = "@c1";
				if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
				{
					sSQL = sSQL.Replace("@c1", "?");
					pn = "c1";
				}
				else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
				{
					sSQL = sSQL.Replace("@c1", "?c1");
					pn = "c1";
				}
				cmd.CommandText = sSQL;
				DbParameter p = cmd.CreateParameter();
				p.ParameterName = pn;
				p.DbType = System.Data.DbType.String;
				p.Value = loginName;
				cmd.Parameters.Add(p);
				this.QueryDef.DatabaseConnection.ConnectionObject.Open();
				DbDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
				if (dr.Read())
				{
					string salt = string.Empty;
					string passwordhashInDB = ValueConvertor.ToString(dr[0]);
					if (string.IsNullOrEmpty(this.UserAccountLevelFieldName))
					{
						if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
						{
							if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
							{
							}
							else
							{
								_loginID = ValueConvertor.ToInt(dr[1]);
							}
						}
						else
						{
							salt = ValueConvertor.ToString(dr[1]);
							if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
							{
							}
							else
							{
								_loginID = ValueConvertor.ToInt(dr[2]);
							}
						}
					}
					else
					{
						_userLevel = ValueConvertor.ToInt(dr[1]);
						if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
						{
							if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
							{
							}
							else
							{
								_loginID = ValueConvertor.ToInt(dr[2]);
							}
						}
						else
						{
							salt = ValueConvertor.ToString(dr[2]);
							if (string.IsNullOrEmpty(this.UserAccountIdFieldName))
							{
							}
							else
							{
								_loginID = ValueConvertor.ToInt(dr[3]);
							}
						}
					}
					string passwordhashInput = WinUtil.GetHash(password, salt, this.PasswordHash);
					if (string.CompareOrdinal(passwordhashInDB, passwordhashInput) == 0)
					{
						bPassed = true;
					}
					else
					{
						info.Append("log in failed. password in db:[");
						info.Append(passwordhashInDB);
						info.Append("] password tried:[");
						info.Append(passwordhashInput);
						info.Append("]");
					}
				}
				else
				{
					info.Append("log in failed. invalid user:[");
					info.Append(loginName);
					info.Append("]");
				}
				dr.Close();
			}
			if (bPassed)
			{
				LogonUser.SetLogon(this.ID, new LogonUser(loginName, _loginID, _userLevel, this.InactivityMinutes));
			}
			else
			{
				LogonUser.RemoveLogon(this.ID);
			}
			return bPassed;
		}
		#endregion

		#region Properties

		[WebServerMember]
		[Description("Gets current user login name. For a web application, use CurrentUserAlias property of a web page. It is empty if there is not a current user, for example, after logging off.")]
		public string LoginName
		{
			get
			{
				return _loginName;
			}
		}
		[WebServerMember]
		[Description("Gets current user ID. For a web application, use CurrentUserID property of a web page. It is 0 if there is not a current user, for example, after logging off.")]
		public int LoginID
		{
			get
			{
				return _loginID;
			}
		}
		[WebServerMember]
		[Description("Gets current user permission level. For a web application, use CurrentUserLevel property of a web page.")]
		public int UserLevel
		{
			get
			{
				return _userLevel;
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
				return _qry.DatabaseConnection;
			}
			set
			{
				_qry.DatabaseConnection = value;
				_fieldnames = null;
			}
		}
		private string _tblName;
		[Editor(typeof(TypeSelectorTable), typeof(UITypeEditor))]
		[Description("Gets and sets the table name of the table storing the user accounts")]
		public string UserAccountTableName
		{
			get { return _tblName; }
			set { _tblName = value; _fieldnames = null; }
		}

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the login field name of the login account.")]
		public string UserAccountLoginFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the ID field name of the login account.If your login account table uses an ID filed as a primary key in relationships then you may want to get the value of this field for building joined queries. It is optional to include this field. ")]
		public string UserAccountIdFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the password field name of the login account.")]
		public string UserAccountPasswordFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the field name of the login account for saving password salt. The field, if used, should be a VARCHAR field of proper length for hashing algoruthm specified by PasswordHash. Bytes needed: MD5=32, SHA1=40, SHA256=64, SHA384=96, SHA512=128")]
		public string UserAccountSaltFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the user level field name of the login account. It is optional.")]
		public string UserAccountLevelFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the field name of the login account for saving password-resetting code. It is optional.")]
		public string UserAccountResetCodeFieldName { get; set; }

		[Editor(typeof(TypeEditorSelectFieldName), typeof(UITypeEditor))]
		[Description("Gets and sets the field name of the login account for saving expiration time of password-resetting code. It is optional.")]
		public string UserAccountResetCodeTimeFieldName { get; set; }

		[Description("The hash algorithm used for storing login password.")]
		public EnumPasswordHash PasswordHash
		{
			get;
			set;
		}

		[Description("Gets and sets an integer indicating how long of inactivity, in minutes, log in will expire.")]
		public int InactivityMinutes { get; set; }

		private IComponent _lable;
		[Description("Gets and sets a label on the page for displaying login error message set in property LoginFailedMessage and LoginPermissionFailedMessage")]
		[ComponentReferenceSelectorTypeWeb(typeof(HtmlLabel))]
		[ComponentReferenceSelectorType(typeof(Label))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent LabelToShowLoginFailedMessage
		{
			get
			{
				return _lable;
			}
			set
			{
				_lable = value;
				if (_lable != null)
				{
					if (_lable.Site != null)
					{
						if (!string.IsNullOrEmpty(_lable.Site.Name))
						{
							_lableId = _lable.Site.Name;
						}
					}
				}
			}
		}

		[Description("Gets and sets a string as the message to display when log in fails due to credential information error.")]
		public string LoginFailedMessage
		{
			get;
			set;
		}
		[Description("Gets and sets a string as the message to display when log in fails due to permission restriction.")]
		public string LoginPermissionFailedMessage
		{
			get;
			set;
		}
		private string _lableId;
		[NotForProgramming]
		[Browsable(false)]
		public string FailedMessageLableId
		{
			get
			{
				if (LabelToShowLoginFailedMessage != null)
				{
					if (LabelToShowLoginFailedMessage.Site != null)
					{
						if (!string.IsNullOrEmpty(LabelToShowLoginFailedMessage.Site.Name))
						{
							_lableId = LabelToShowLoginFailedMessage.Site.Name;
						}
					}
				}
				return _lableId;
			}
			set
			{
				_lableId = value;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public EasyQuery QueryDef
		{
			get
			{
				if (_qry == null)
				{
					_qry = new EasyQuery();
				}
				return _qry;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public Guid ID
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public string COOKIE_UserLogin
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "WebLgin{0}",
					((UInt32)ID.GetHashCode()).ToString("x", CultureInfo.InvariantCulture));
			}
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public virtual bool OnBeforeSetSQL()
		{
			return true;
		}
		[WebServerMember]
		[Description("Reset password")]
		public bool ResetPassword(string loginName, string newPassword)
		{
			string err = string.Empty;
			if (string.IsNullOrEmpty(loginName))
				err = "Missing loginName";
			else if (string.IsNullOrEmpty(newPassword))
				err = "Missing new password";
			else
			{
				try
				{
					int pCount = 2;
					string salt = string.Empty;
					string sSQL;
					DbCommand cmd;
					if (string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						sSQL = string.Format(CultureInfo.InvariantCulture,
							"UPDATE {0}{2}{1} SET {0}{3}{1} = @c1 WHERE {0}{4}{1} = @c2",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountTableName,
							this.UserAccountPasswordFieldName,
							this.UserAccountLoginFieldName
						);
					}
					else
					{
						pCount = 3;
						salt = WinUtil.GetRandomString(64);
						sSQL = string.Format(CultureInfo.InvariantCulture,
							"UPDATE {0}{2}{1} SET {0}{3}{1} = @c1, {0}{5}{1}=@c2  WHERE {0}{4}{1} = @c3",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							this.UserAccountTableName,
							this.UserAccountPasswordFieldName,
							this.UserAccountLoginFieldName,
							this.UserAccountSaltFieldName
						);
					}
					cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
					string pn1 = "@c1", pn2 = "@c2", pn3 = "@c3";
					if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
					{
						sSQL = sSQL.Replace("@c1", "?").Replace("@c2", "?").Replace("@c3", "?");
					}
					else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
					{
						sSQL = sSQL.Replace("@c1", "?c1").Replace("@c2", "?c2").Replace("@c3", "?c3");
						pn1 = "c1"; pn2 = "c2"; pn3 = "c3";
					}
					cmd.CommandText = sSQL;
					if (jsp != null)
					{
						jsp.LogDebugInfo("SQL:{0}", sSQL);
					}

					DbParameter p = cmd.CreateParameter();
					p.ParameterName = pn1;
					p.DbType = System.Data.DbType.String;
					p.Value = WinUtil.GetHash(newPassword, salt, this.PasswordHash);
					cmd.Parameters.Add(p);
					//
					p = cmd.CreateParameter();
					p.ParameterName = pn2;
					p.DbType = System.Data.DbType.String;
					if (pCount == 2)
					{
						p.Value = loginName;
					}
					else
					{
						p.Value = salt;
					}
					cmd.Parameters.Add(p);
					//
					if (pCount == 3)
					{
						p = cmd.CreateParameter();
						p.ParameterName = pn3;
						p.DbType = System.Data.DbType.String;
						p.Value = loginName;
						cmd.Parameters.Add(p);
					}
					this.QueryDef.DatabaseConnection.ConnectionObject.Open();
					cmd.ExecuteNonQuery();
					return true;
				}
				catch (Exception errExp)
				{
					err = ExceptionLimnorDatabase.FormExceptionText(errExp);
				}
			}
			showErrorMsg(err);
			return false;
		}
		[WebServerMember]
		[Description("Create password-reset code so that a user may reset his/her password in the case of forgetting password")]
		public string CreatePasswordResetCode(string loginName, int expiratonInMinutes)
		{
			_debugLogs = new StringCollection();
			try
			{
				if (jsp != null)
				{
					jsp.LogDebugInfo("Create password-reset code<br>");
				}
				if (this.DatabaseConnection != null && this.DatabaseConnection.IsValid)
				{
					if (!string.IsNullOrEmpty(this.UserAccountTableName)
						&& !string.IsNullOrEmpty(this.UserAccountLoginFieldName)
						&& !string.IsNullOrEmpty(this.UserAccountResetCodeFieldName)
						&& !string.IsNullOrEmpty(this.UserAccountResetCodeTimeFieldName))
					{
						if (string.IsNullOrEmpty(loginName))
						{
							showErrorMsg("login name cannot be empty");
						}
						else
						{
							DbCommand cmd;
							DbParameter p;
							string sSQL;
							string salt = string.Empty;
							string pn1 = "@c1";
							if (!string.IsNullOrEmpty(this.UserAccountSaltFieldName))
							{
								sSQL = string.Format(CultureInfo.InvariantCulture,
									"SELECT {0}{2}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
									this.QueryDef.NameDelimiterBegin,
									this.QueryDef.NameDelimiterEnd,
									this.UserAccountSaltFieldName,
									this.UserAccountTableName,
									this.UserAccountLoginFieldName);
								cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
								if (jsp != null)
								{
									jsp.LogDebugInfo("SQL:{0}", sSQL);
								}
								if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
								{
									sSQL = sSQL.Replace("@c1", "?c1");
									pn1 = "c1";
								}
								else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
								{
									sSQL = sSQL.Replace("@c1", "?");
								}
								cmd.CommandText = sSQL;
								p = cmd.CreateParameter();
								p = cmd.CreateParameter();
								p.ParameterName = pn1;
								p.DbType = System.Data.DbType.String;
								p.Value = loginName;
								cmd.Parameters.Add(p);
								this.QueryDef.DatabaseConnection.ConnectionObject.Open();
								DbDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
								if (dr.Read())
								{
									salt = dr.GetString(0);
								}
								dr.Close();
							}
							sSQL = string.Format(CultureInfo.InvariantCulture,
								"UPDATE {0}{2}{1} SET {0}{3}{1}=@c1, {0}{4}{1}=@ct WHERE {0}{5}{1}=@clogin",
								this.QueryDef.NameDelimiterBegin,
									this.QueryDef.NameDelimiterEnd,
									this.UserAccountTableName,
									this.UserAccountResetCodeFieldName,
									this.UserAccountResetCodeTimeFieldName,
									this.UserAccountLoginFieldName
									);
							string resetCode = WinUtil.GetRandomString(8);
							string codeHash = WinUtil.GetHash(resetCode, salt, this.PasswordHash);
							cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
							if (jsp != null)
							{
								jsp.LogDebugInfo("SQL:{0}", sSQL);
							}
							string pnct = "@ct", pnclogin = "@clogin";
							if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
							{
								sSQL = sSQL.Replace("@c1", "?c1").Replace(pnct, "?ct").Replace(pnclogin, "?clogin");
								pn1 = "c1"; pnct = "ct"; pnclogin = "clogin";
							}
							else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
							{
								sSQL = sSQL.Replace("@c1", "?").Replace(pnct, "?").Replace(pnclogin, "?"); ;
							}
							cmd.CommandText = sSQL;
							//
							p = cmd.CreateParameter();
							p.ParameterName = pn1;
							p.DbType = System.Data.DbType.String;
							p.Value = codeHash;
							cmd.Parameters.Add(p);
							//
							p = cmd.CreateParameter();
							p.ParameterName = pnct;
							p.DbType = System.Data.DbType.DateTime;
							p.Value = DateTime.Now.AddMinutes(expiratonInMinutes);
							cmd.Parameters.Add(p);
							//
							p = cmd.CreateParameter();
							p.ParameterName = pnclogin;
							p.DbType = System.Data.DbType.String;
							p.Value = loginName;
							cmd.Parameters.Add(p);
							this.QueryDef.DatabaseConnection.ConnectionObject.Open();
							cmd.ExecuteNonQuery();
							if (jsp != null)
							{
								jsp.LogDebugInfo("<br>password-reset code generated:{0}<br>", resetCode);
							}
							return resetCode;
						}
					}
					else
					{
						showErrorMsg("Missing required properties");
					}
				}
				else
					showErrorMsg("In valid database connection");
			}
			catch (Exception err)
			{
				showErrorMsg(ExceptionLimnorDatabase.FormExceptionText(err));
			}
			return string.Empty;
		}
		[WebServerMember]
		[Description("Reset password on receiving reset code.")]
		public bool ResetPasswordByUser(string loginName, string resetCode, string newPassword)
		{
			_debugLogs = new StringCollection();
			if (jsp != null)
			{
				jsp.LogDebugInfo("ResetPasswordByUser<br>");
			}
			if (string.IsNullOrEmpty(this.UserAccountResetCodeFieldName)
				|| string.IsNullOrEmpty(this.UserAccountResetCodeTimeFieldName)
				|| string.IsNullOrEmpty(this.UserAccountLoginFieldName)
				|| string.IsNullOrEmpty(this.UserAccountTableName))
			{
				showErrorMsg("Missing required properties");
			}
			else
			{
				try
				{
					string sSQL = string.Format(CultureInfo.InvariantCulture,
						"SELECT {0}{2}{1}, {0}{3}{1}",
						this.QueryDef.NameDelimiterBegin,
						this.QueryDef.NameDelimiterEnd,
						this.UserAccountResetCodeFieldName,
						this.UserAccountResetCodeTimeFieldName);
					if (!string.IsNullOrEmpty(this.UserAccountSaltFieldName))
					{
						sSQL = string.Format(CultureInfo.InvariantCulture,
							"{2},{0}{3}{1}",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							sSQL,
							this.UserAccountSaltFieldName);
					}
					sSQL = string.Format(CultureInfo.InvariantCulture,
							"{2} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
							this.QueryDef.NameDelimiterBegin,
							this.QueryDef.NameDelimiterEnd,
							sSQL,
							this.UserAccountTableName,
							this.UserAccountLoginFieldName);
					DbCommand cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
					if (jsp != null)
					{
						jsp.LogDebugInfo("SQL:{0}", sSQL);
					}
					string pn1 = "@c1";
					if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
					{
						sSQL = sSQL.Replace(pn1, "?");
					}
					else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
					{
						sSQL = sSQL.Replace(pn1, "?c1");
					}
					cmd.CommandText = sSQL;
					DbParameter p = cmd.CreateParameter();
					p = cmd.CreateParameter();
					p.ParameterName = pn1;
					p.DbType = System.Data.DbType.String;
					p.Value = loginName;
					cmd.Parameters.Add(p);
					this.QueryDef.DatabaseConnection.ConnectionObject.Open();
					DbDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
					if (dr.Read())
					{
						string salt = string.Empty;
						string resetHash = dr.GetString(0);
						if (string.IsNullOrEmpty(resetHash))
						{
							showErrorMsg("Reset code not created");
						}
						else
						{
							DateTime expi = dr.GetDateTime(1);
							if (dr.IsDBNull(1) || DateTime.Now > expi)
							{
								showErrorMsg("Reset code expired");
							}
							else
							{
								if (!string.IsNullOrEmpty(this.UserAccountSaltFieldName))
								{
									salt = dr.GetString(2);
								}
								string inputHash = WinUtil.GetHash(resetCode, salt, this.PasswordHash);
								if (string.CompareOrdinal(inputHash, resetHash) == 0)
								{
									return ResetPassword(loginName, newPassword);
								}
								else
								{
									showErrorMsg("Invalid user name or reset code");
								}
							}
						}
					}
					else
					{
						showErrorMsg("Invalid user name or reset code");
					}
					dr.Close();
				}
				catch (Exception err)
				{
					string emsg = ExceptionLimnorDatabase.FormExceptionText(err);
					if (jsp != null)
					{
						jsp.LogDebugInfo("Error resetting password by user. {0}", emsg);
					}
					else
					{
						_debugLogs.Add(emsg);
					}
					showFailMsg(emsg);
				}
			}
			return false;
		}
		[WebServerMember]
		[Description("Change password")]
		public bool ChangePassword(string loginName, string currentPassword, string newPassword)
		{
			_debugLogs = new StringCollection();
			try
			{
				if (jsp != null)
				{
					jsp.LogDebugInfo("Changing password<br>");
				}
				if (this.DatabaseConnection != null)
				{
					if (this.DatabaseConnection.IsValid)
					{
						if (!string.IsNullOrEmpty(this.UserAccountTableName)
							&& !string.IsNullOrEmpty(this.UserAccountLoginFieldName)
							&& !string.IsNullOrEmpty(this.UserAccountPasswordFieldName))
						{
							if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(newPassword))
							{
								StringBuilder sberr = new StringBuilder();
								if (string.IsNullOrEmpty(loginName))
								{
									sberr.Append("Missing login name");
								}
								if (string.IsNullOrEmpty(newPassword))
								{
									sberr.Append("Missing new password");
								}
								showErrorMsg(sberr.ToString());
							}
							else
							{
								bool bOK = false;
								string sSQL = string.Empty;
								StringBuilder info = new StringBuilder();
								if (string.IsNullOrEmpty(currentPassword))
								{
									DbCommand cmd = this.QueryDef.DatabaseConnection.ConnectionObject.CreateCommand();
									sSQL = string.Format(CultureInfo.InvariantCulture,
										"SELECT {0}{2}{1} FROM {0}{3}{1} WHERE {0}{4}{1}=@c1",
										this.QueryDef.NameDelimiterBegin,
										this.QueryDef.NameDelimiterEnd,
										this.UserAccountPasswordFieldName,
										this.UserAccountTableName,
										this.UserAccountLoginFieldName
										);
									if (jsp != null)
									{
										jsp.LogDebugInfo("SQL:{0}", sSQL);
									}
									string pn1 = "@c1";
									if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.LeadingQuestionMark)
									{
										sSQL = sSQL.Replace(pn1, "?c1");
										pn1 = "c1";
									}
									else if (this.QueryDef.DatabaseConnection.ParameterStyle == EnumParameterStyle.QuestionMark)
									{
										sSQL = sSQL.Replace(pn1, "?");
									}
									cmd.CommandText = sSQL;
									DbParameter p = cmd.CreateParameter();
									p = cmd.CreateParameter();
									p.ParameterName = pn1;
									p.DbType = System.Data.DbType.String;
									p.Value = loginName;
									cmd.Parameters.Add(p);
									this.QueryDef.DatabaseConnection.ConnectionObject.Open();
									DbDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
									if (dr.Read())
									{
										object v = dr[0];
										if (v == null || v == DBNull.Value)
										{
											bOK = true; //password in db is NULL
										}
										else
										{
											string s1 = v.ToString();
											if (string.IsNullOrEmpty(s1))
											{
												bOK = true; //password in db is empty
											}
											else
											{
												info.Append("Empty current password entered. Password in the database is not empty.");
											}
										}
									}
									else
									{
										info.Append("Invalid user:[");
										info.Append(loginName);
										info.Append("]");
									}
									dr.Close();
								}
								else
								{
									bOK = checkLogin(loginName, currentPassword, info);
								}
								if (bOK)
								{
									return ResetPassword(loginName, newPassword);
								}
								else
								{
									info.Append("Invalid login or current password");
								}
								string sInfo = info.ToString();
								if (jsp != null)
								{
									jsp.LogDebugInfo(sInfo);
								}
								else
								{
									_debugLogs.Add(sInfo);
								}
								if (!bOK)
								{
									showFailMsg(sInfo);
								}
							}
						}
						else
						{
							showErrorMsg("Missing required database field names");
						}
					}
					else
					{
						showErrorMsg("Database connection not valid");
					}
				}
				else
				{
					showErrorMsg("Database connection not set");
				}
			}
			catch (Exception err)
			{
				string emsg = ExceptionLimnorDatabase.FormExceptionText(err);
				if (jsp != null)
				{
					jsp.LogDebugInfo("Error changing password. {0}", emsg);
				}
				else
				{
					_debugLogs.Add(emsg);
				}
				showFailMsg(emsg);
			}
			return false;
		}
		[WebServerMember]
		[Description("log in the user")]
		public bool Login(string loginName, string password)
		{
			string userLogin = null;
			_debugLogs = new StringCollection();
			if (jsp != null)
			{
				if (jsp.Response.Cookies[COOKIE_UserLogin] != null)
				{
					userLogin = jsp.Response.Cookies[COOKIE_UserLogin].Value;
				}
				if (!string.IsNullOrEmpty(userLogin))
				{
					jsp.LogDebugInfo("Already log in to:{0}", userLogin);
					return true;
				}
				jsp.LogDebugInfo("Start log in<br>");
				jsp.LogDebugInfo("loginName:");
				jsp.LogDebugInfo(loginName);
				jsp.LogDebugInfo(" password:");
				jsp.LogDebugInfo(password);
				jsp.LogDebugInfo("<br>");
			}
			else
			{
				if (!string.IsNullOrEmpty(_loginName) && string.CompareOrdinal(loginName, _loginName) == 0)
				{
					return true;
				}
			}
			//
			bool bPassed = false;
			if (this.DatabaseConnection != null)
			{
				if (this.DatabaseConnection.IsValid)
				{
					if (!string.IsNullOrEmpty(this.UserAccountTableName)
						&& !string.IsNullOrEmpty(this.UserAccountLoginFieldName)
						&& !string.IsNullOrEmpty(this.UserAccountPasswordFieldName))
					{
						string sSQL = string.Empty;
						if (!string.IsNullOrEmpty(loginName)
							&& !string.IsNullOrEmpty(password))
						{
							StringBuilder info = new StringBuilder();
							bPassed = checkLogin(loginName, password, info);
							if (bPassed)
							{

							}
							else
							{
								showErrorMsg(info.ToString());
							}
						}
					}
					else
					{
						showErrorMsg("Missing required properties.");
					}
				}
				else
				{
					showErrorMsg("Database connection not valid");
				}
			}
			else
			{
				showErrorMsg("Database connection not set");
			}
			if (bPassed)
			{
				if (this.InactivityMinutes <= 0)
				{
					this.InactivityMinutes = 10;
				}
				if (jsp != null)
				{
					jsp.AddClientScript("JsonDataBinding.LoginPassed('{0}',{1},{2}, {3});", loginName, this.InactivityMinutes, _userLevel, _loginID);
				}
				else
				{
					Label lb = this.LabelToShowLoginFailedMessage as Label;
					if (lb != null)
					{
						lb.Text = "";
					}
					if (UserLogin != null)
					{
						UserLogin();
					}
				}
			}
			else
			{
				if (jsp != null)
				{
					jsp.AddClientScript("JsonDataBinding.LoginFailed('{0}','{1}');", this.FailedMessageLableId, this.LoginFailedMessage);
				}
				else
				{
					Label lb = this.LabelToShowLoginFailedMessage as Label;
					if (lb != null)
					{
						if (string.IsNullOrEmpty(this.LoginFailedMessage))
							lb.Text = "Login failed";
						else
							lb.Text = this.LoginFailedMessage;
					}
					if (LoginFailed != null)
					{
						LoginFailed();
					}
				}
			}
			return bPassed;
		}
		[WebClientMember]
		public void LogOff()
		{
			_loginName = string.Empty;
			_loginID = 0;
			_userLevel = -1;
			LogonUser.RemoveLogon(this.ID);
		}
		public string GetDebugLogInf()
		{
			if (_debugLogs != null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < _debugLogs.Count; i++)
				{
					sb.Append(_debugLogs[i]);
				}
				return sb.ToString();
			}
			return string.Empty;
		}
		#endregion

		#region Events
		[WebClientEventByServerObject]
		[Description("It occurs when a user logs on")]
		[WebClientMember]
		public event SimpleCall UserLogin;//{ add { } remove { } }

		[WebClientEventByServerObject]
		[Description("It occurs when a user tries to log on and fails")]
		[WebClientMember]
		public event SimpleCall LoginFailed;// { add { } remove { } }
		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		private ISite _site;
		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
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
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IDatabaseAccess Members
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedDesignTimeSQL
		{
			get { return false; }
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateDataTable()
		{
		}
		[Browsable(false)]
		public void SetSqlContext(string name)
		{
		}
		[NotForProgramming]
		[Browsable(false)]
		public string ComponentName
		{
			get
			{
				if (Site != null && Site.DesignMode && !string.IsNullOrEmpty(Site.Name))
				{
					_componentName = Site.Name;
				}
				return _componentName;
			}
			set
			{
				_componentName = value;
			}
		}
		[Browsable(false)]
		public string Name
		{
			get
			{
				if (Site != null && !string.IsNullOrEmpty(Site.Name))
				{
					return Site.Name;
				}
				if (_qry != null)
					return _qry.Name;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public bool Query()
		{
			if (_qry != null)
			{
				_qry.LogMessage("Calling Query from EasyDataSet");
				if (_qry.DataStorage != null && _qry.DataStorage.Tables.Count > 0)
				{
					_qry.RemoveTable(_qry.TableName);
				}
				_qry.ResetCanChangeDataSet(false);
				return _qry.Query();
			}
			return false;
		}
		[NotForProgramming]
		[Browsable(false)]
		[Category("Database")]
		[ReadOnly(true)]
		[TypeConverter(typeof(TypeConverterSQLString))]
		[XmlIgnore]
		[Description("SQL statement for querying database")]
		[Editor(typeof(UIQueryEditor), typeof(UITypeEditor))]
		public SQLStatement SQL
		{
			get
			{
				if (_qry == null)
				{
					_qry = new EasyQuery();
				}
				return _qry.SQL;
			}
			set
			{
				if (_qry == null)
				{
					_qry = new EasyQuery();
				}
				_qry.SQL = value;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public bool IsConnectionReady
		{
			get
			{
				if (_qry != null)
					return _qry.IsConnectionReady;
				return false;
			}
		}
		[NotForProgramming]
		[ReadOnly(true)]
		[Browsable(false)]
		public bool QueryOnStart
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public void CopyFrom(EasyQuery query)
		{
			if (_qry != null && query != null)
			{
				_qry.CopyFrom(query);
			}
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (_qry != null)
			{
				return _qry.Report32Usage();
			}
			return string.Empty;
		}
		[NotForProgramming]
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				if (_qry != null)
				{
					return _qry.DatabaseConnectionsUsed;
				}
				return new List<Guid>();
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				if (_qry != null)
				{
					return _qry.DatabaseConnectionTypesUsed;
				}
				return new List<Type>();
			}
		}

		#endregion

		#region IWebServerProgrammingSupport Members
		[Browsable(false)]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}
		[Browsable(false)]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			string ret = string.Empty;
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				ret = string.Format(CultureInfo.InvariantCulture, "{0}=", returnReceiver);
			}
			if (string.CompareOrdinal(methodName, "Login") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{3}$this->{0}->Login({1},{2});", Site.Name, parameters[0], parameters[1], ret));
			}
			else if (string.CompareOrdinal(methodName, "ChangePassword") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{4}$this->{0}->ChangePassword({1},{2},{3});", Site.Name, parameters[0], parameters[1], parameters[2], ret));
			}
			else if (string.CompareOrdinal(methodName, "ResetPassword") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{3}$this->{0}->ResetPassword({1},{2});", Site.Name, parameters[0], parameters[1], ret));
			}
			else if (string.CompareOrdinal(methodName, "CreatePasswordResetCode") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{3}$this->{0}->CreatePasswordResetCode({1},{2});", Site.Name, parameters[0], parameters[1], ret));
			}
			else if (string.CompareOrdinal(methodName, "ResetPasswordByUser") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{4}$this->{0}->ResetPasswordByUser({1},{2},{3});", Site.Name, parameters[0], parameters[1], parameters[2], ret));
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("WebLoginManager.php", Resource1.WebLoginManager_php);
			return files;
		}
		[Browsable(false)]
		public bool DoNotCreate()
		{
			return false;
		}
		[Browsable(false)]
		public void OnRequestStart(Page/*JsonWebServerProcessor*/ webPage)
		{
			jsp = webPage as JsonWebServerProcessor;
			jsp.SetServerComponentName(this._componentName);
		}
		[Browsable(false)]
		public void CreateOnRequestStartPhp(StringCollection code)
		{
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
		}
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedObjectName { get { return true; } }
		#endregion

		#region private methods

		#endregion

		#region IWebServerComponentCreator Members
		[Browsable(false)]
		public void CreateServerComponentPhp(StringCollection objectDecl, StringCollection initCode, ServerScriptHolder scripts)
		{
			initCode.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->NameDelimiterBegin=\"{1}\";\r\n$this->{0}->NameDelimiterEnd=\"{2}\";\r\n$this->{0}->Initialize($this, $this->{3}, '{4}');\r\n",
				this.Site.Name,
				this.QueryDef.NameDelimiterBegin,
				this.QueryDef.NameDelimiterEnd,
				ServerCodeutility.GetPhpMySqlConnectionName(this.ConnectionID),
				this.COOKIE_UserLogin
				));
		}
		[Browsable(false)]
		public bool ExcludePropertyForPhp(string name)
		{
			if (string.CompareOrdinal(name, "DefaultConnectionString") == 0)
			{
				return true;
			}
			return false;
		}
		[Browsable(false)]
		public bool RemoveFromComponentInitializer(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "LabelToShowLoginFailedMessage") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(propertyName, "UsePassword") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(propertyName, "UseLoginValue") == 0)
			{
				return true;
			}
			return false;
		}
		#endregion

		#region non-browsable Properties
		[NotForProgramming]
		[Browsable(false)]
		public Guid ConnectionID
		{
			get
			{
				return _qry.ConnectionID;
			}
			set
			{
				_qry.ConnectionID = value;
			}
		}
		/// <summary>
		/// for setting default connection
		/// </summary>
		[NotForProgramming]
		[Browsable(false)]
		[XmlIgnore]
		public string DefaultConnectionString
		{
			get
			{
				return _qry.DefaultConnectionString;
			}
			set
			{
				_qry.DefaultConnectionString = value;
			}
		}
		/// <summary>
		/// for setting default connection
		/// </summary>
		[NotForProgramming]
		[Browsable(false)]
		[XmlIgnore]
		public Type DefaultConnectionType
		{
			get
			{
				return _qry.DefaultConnectionType;
			}
			set
			{
				_qry.DefaultConnectionType = value;
			}
		}
		#endregion

		#region IFieldsHolder Members
		[Browsable(false)]
		public string[] GetFieldNames()
		{
			if (_fieldnames == null)
			{
				if (!string.IsNullOrEmpty(this.UserAccountTableName))
				{
					ConnectionItem dbc = DatabaseConnection;
					if (dbc != null)
					{
						Connection cc = dbc.ConnectionObject;
						if (cc != null)
						{
							FieldList fl = cc.GetTableFields(this.UserAccountTableName);
							if (fl != null)
							{
								_fieldnames = new string[fl.Count];
								for (int i = 0; i < fl.Count; i++)
								{
									_fieldnames[i] = fl[i].Name;
								}
							}
						}
					}
				}
			}
			return _fieldnames;
		}

		#endregion
	}
}
