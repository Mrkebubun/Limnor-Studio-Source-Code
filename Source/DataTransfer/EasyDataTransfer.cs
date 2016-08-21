/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data;
using Microsoft.Win32;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
using VPL;
using System.CodeDom;
using Limnor.WebServerBuilder;
using WebServerProcessor;
using System.Reflection;
using System.Text;
using Limnor.WebBuilder;
using System.Windows.Forms;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxBitmapAttribute(typeof(EasyTransfer), "Resources.dt.bmp")]
	[Description("This component can be used to do data transfer.")]
	public class EasyTransfer : IComponent, ICustomTypeDescriptor, IDevClassReferencer, IPropertyValueLinkOwner, IDatabaseConnectionUserExt0, IWebServerProgrammingSupport, IDynamicMethodParameters
	{
		#region fields and constructors
		const string FILENAME = "Filename";
		private string _name;
		private Guid _id;
		private TransMethod _transMethod;
		private DTDataDestination _dest;
		private DTDataSource _source;
		private string _error;
		private JsonWebServerProcessor _webPage;
		public EasyTransfer()
		{
			//
			Enabled = true;
			Silent = true;
			//
		}
		#endregion

		#region private methods
		private bool phpSupported()
		{
			if (this.EndPointType != enumDTType.Both)
			{
				MessageBox.Show("This version of data transfer component only supports EndPointType being 'Both' in a PHP project.", "Data Transfer Component for PHP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (DataDestination.DestinationType != EnumDataDestination.Database)
				{
					MessageBox.Show("This version of data transfer component only supports using MySQL database as the data destination in a PHP project.", "Data Transfer Component for PHP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					return true;
				}
			}
			return false;
		}
		private static void addCode(StringCollection code, string frm, params object[] values)
		{
			code.Add(string.Format(CultureInfo.InvariantCulture, frm, values));
		}
		void startDT()
		{
			_error = null;
			bool bSilent = Silent;
			enumDTType tp = this.EndPointType;
			if (tp == enumDTType.Both || tp == enumDTType.Sender)
			{
				IEPDataSource dtsSrc = DataSource;
				ClientDate cp = new ClientDate(ID);
				cp.LoadSettings();
				dtsSrc.Timestamp = cp.dt;
				if (tp == enumDTType.Both)
				{
					DTDataDestination dest = this.DataDestination;
					if (dest.IsReady)
					{
						if (StartedDataTransfer != null)
						{
							StartedDataTransfer(this, EventArgs.Empty);
						}
						DataTable tblSrc = dtsSrc.DataSource;
						if (tblSrc != null)
						{
							_error = dest.ReceiveData(tblSrc, bSilent);
						}
						else
						{
							_error = dtsSrc.LastError;
						}
						if (string.IsNullOrEmpty(_error))
						{
							if (FinishedDataTransfer != null)
							{
								FinishedDataTransfer();
							}
						}
						else
						{
							if (ErrorDataTransfer != null)
							{
								ErrorDataTransfer(this, EventArgs.Empty);
							}
						}
					}
				}
				else
				{
					if (StartedDataTransfer != null)
					{
						StartedDataTransfer(this, EventArgs.Empty);
					}
					System.Data.DataTable tblSrc = dtsSrc.DataSource;
					if (tblSrc != null)
					{
						TransMethod method = DataTransferMethod;
						if (method.SendFile(tblSrc, Name, bSilent))
						{
							if (FinishedDataTransfer != null)
							{
								FinishedDataTransfer();
							}
						}
						else
						{
							_error = method.ErrorMessage;
							if (ErrorDataTransfer != null)
							{
								ErrorDataTransfer(this, EventArgs.Empty);
							}
						}
					}
				}
				if (string.IsNullOrEmpty(_error))
				{
					cp.dt = dtsSrc.Timestamp;
					cp.SaveSettings();
				}
				dtsSrc.ClearData();
			}
			else
			{
				DTDataDestination dest = DataDestination;
				if (dest.IsReady)
				{
					if (StartedDataTransfer != null)
					{
						StartedDataTransfer(this, EventArgs.Empty);
					}
					TransMethod method = DataTransferMethod;
					if (method.ReceiveFile(Name, dest, bSilent))
					{
						if (FinishedDataTransfer != null)
						{
							FinishedDataTransfer();
						}
					}
					else
					{
						_error = method.ErrorMessage;
						if (ErrorDataTransfer != null)
						{
							ErrorDataTransfer(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		#endregion

		#region Events
		[Description("Data transfer started")]
		public event EventHandler StartedDataTransfer;

		[WebClientMember]
		[WebClientEventByServerObject]
		[Description("Data transfer finished")]
		public event SimpleCall FinishedDataTransfer;

		[Description("It occurs when there is an error in the data transfer")]
		public event EventHandler ErrorDataTransfer;
		#endregion

		#region Methods
		[WebServerMember]
		[Description("Start data transfer.")]
		public void StartWithParameterValues(params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				FieldList pl = Parameters;
				if (pl != null)
				{
					int n = Math.Min(pl.Count, values.Length);
					for (int i = 0; i < n; i++)
					{
						if (values[i] is DateTime)
						{
							if (!EPField.IsDatetime(pl[i].OleDbType))
							{
								pl[i].OleDbType = OleDbType.DBTimeStamp;
							}
							if (this.IsJet)
							{
								DateTime dt = (DateTime)values[i];
								pl[i].SetValue(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
							}
							else
							{
								pl[i].SetValue(values[i]);
							}
						}
						else
						{
							if (values[i] == null || values[i] == DBNull.Value)
							{
								pl[i].OleDbType = OleDbType.VarWChar;
								pl[i].SetValue(null);
							}
							else
							{
								pl[i].OleDbType = EPField.ToOleDBType(values[i].GetType());
								pl[i].SetValue(values[i]);
							}
						}
					}
				}
			}
			startDT();
		}
		#endregion

		#region Properties
		[DefaultValue(0)]
		[WebServerMember]
		[Description("Gets and sets the timeout, in minutes, for PHP operations.")]
		public int PhpTimeoutInMinutes
		{
			get;
			set;
		}
		[WebServerMember]
		[Browsable(false)]
		[Description("Gets the error message when event ErrorDataTransfer occurs")]
		public string ErrorMessage
		{
			get
			{
				if (_error == null)
					return string.Empty;
				return _error;
			}
		}
		[Browsable(false)]
		public Guid ID
		{
			get
			{
				if (_id == Guid.Empty)
				{
					_id = Guid.NewGuid();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Description("Gets the id for this instance")]
		public Guid ComponentID
		{
			get
			{
				return ID;
			}
		}
		[Browsable(false)]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				if (Site != null && Site.DesignMode && !string.IsNullOrEmpty(Site.Name))
				{
					_name = Site.Name;
				}
				return _name;
			}
			set
			{
				_name = value;
				if (Site != null && Site.DesignMode)
				{
					Site.Name = value;
				}
			}
		}
		[Browsable(false)]
		[DefaultValue(true)]
		[Description("Set it to False to display error message on the screen; set it to True not to show error message. In both cases, the error message will be logged in easyplay.log file.")]
		public bool Silent { get; set; }

		[DefaultValue(true)]
		[Description("Indicates whether it is enabled.")]
		public bool Enabled { get; set; }

		[Description("Data source. It can be a database query or a text file.")]
		public DTDataSource DataSource
		{
			get
			{
				if (_source == null)
				{
					_source = new DTDataSource();
				}
				_source.SetOwner(this);
				return _source;
			}
			set
			{
				_source = value;
				if (_source != null)
				{
					_source.SetOwner(this);
				}
			}
		}

		[Description("The data table to receive data from the data source.")]
		public DTDataDestination DataDestination
		{
			get
			{
				if (_dest == null)
				{
					_dest = new DTDataDestination();
				}
				_dest.SetOwner(this);
				return _dest;
			}
			set
			{
				_dest = value;
				if (_dest != null)
				{
					_dest.SetOwner(this);
				}
			}
		}
		[Browsable(false)]
		[Description("The parameters to help defining the data source.")]
		public FieldList Parameters
		{
			get
			{
				if (DataSource != null)
				{
					return DataSource.Parameters;
				}
				return null;
			}
		}
		protected bool IsJet
		{
			get
			{
				if (DataSource != null)
				{
					return DataSource.IsJet;
				}
				return false;
			}
		}

		[ParenthesizePropertyName(true)]
		[Description("Indicate it is a data sender, a data receiver, or both a sender and a receiver.")]
		public enumDTType EndPointType { get; set; }

		[Description("Indicate the data transfer method.")]
		public TransMethod DataTransferMethod
		{
			get
			{
				if (_transMethod == null)
				{
					_transMethod = new TransMethod();
				}
				return _transMethod;

			}
			set
			{
				_transMethod = value;
			}
		}
		[Browsable(false)]
		[Description("Timestamp value for the last data transfer.")]
		public DateTime LastTimestamp { get; private set; }

		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get;
			set;
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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (!VPLUtil.GetBrowseableProperties(attributes))
			{
				return ps;
			}
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal(p.Name, "DataSource") == 0)
				{
					if (EndPointType == enumDTType.Receiver)
					{
						continue;
					}
				}
				else if (string.CompareOrdinal(p.Name, "DataDestination") == 0)
				{
					if (EndPointType == enumDTType.Sender)
					{
						continue;
					}
				}
				else if (string.CompareOrdinal(p.Name, "DataTransferMethod") == 0)
				{
					if (EndPointType == enumDTType.Both)
					{
						continue;
					}
				}
				lst.Add(p);
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

		#region class ClientDate
		class ClientDate
		{
			private Guid _id;
			public ClientDate(Guid id)
			{
				_id = id;
			}
			public void LoadSettings()
			{
				RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Longflow\\LimnorStudio\\EasyDataTransfer\\LastTimestamp");
				if (key != null)
				{
					object v = key.GetValue(_id.ToString("N"), 0);
					key.Close();
					if (v != null)
					{
						try
						{
							long tk = Convert.ToInt64(v, CultureInfo.InvariantCulture);
							dt = new DateTime(tk);
						}
						catch
						{
						}
					}
				}
				dt = DateTime.MinValue;
			}
			public void SaveSettings()
			{
				RegistryKey key = Registry.LocalMachine.CreateSubKey("Software\\Longflow\\LimnorStudio\\EasyDataTransfer\\LastTimestamp", RegistryKeyPermissionCheck.ReadWriteSubTree);
				if (key != null)
				{
					key.SetValue(_id.ToString("N"), dt.Ticks, RegistryValueKind.DWord);
					key.Close();
				}
			}
			public DateTime dt { get; set; }
		}
		#endregion

		#region IDevClassReferencer Members
		private IDevClass _devClass;
		[Browsable(false)]
		public void SetDevClass(IDevClass c)
		{
			_devClass = c;
			DataSource.SetDevClass(c);
		}
		[Browsable(false)]
		public IDevClass GetDevClass()
		{
			return _devClass;
		}

		#endregion

		#region IPropertyValueLinkOwner Members

		[Browsable(false)]
		public Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> GetPropertyValueLinks()
		{
			Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> ret = new Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>>();
			CodePropertyReferenceExpression p1;
			CodePropertyReferenceExpression p2;
			Dictionary<IPropertyValueLink, CodeExpression> kv;
			IPropertyValueLink pl = this.DataSource.TextSource.GetPropertyLink(FILENAME);
			if (pl != null && pl.IsValueLinkSet())
			{
				p1 = new CodePropertyReferenceExpression();
				p1.PropertyName = "TextSource";
				p2 = new CodePropertyReferenceExpression();
				p1.TargetObject = p2;
				p2.PropertyName = "DataSource";
				p1.UserData.Add("name", FILENAME);

				kv = new Dictionary<IPropertyValueLink, CodeExpression>();

				kv.Add(pl, p1);
				ret.Add(this.DataSource.TextSource, kv);
			}
			//
			pl = this.DataDestination.TextDestination.GetPropertyLink(FILENAME);
			if (pl != null && pl.IsValueLinkSet())
			{
				p1 = new CodePropertyReferenceExpression();
				p1.PropertyName = "TextDestination";
				p2 = new CodePropertyReferenceExpression();
				p1.TargetObject = p2;
				p2.PropertyName = "DataDestination";
				p1.UserData.Add("name", FILENAME);

				kv = new Dictionary<IPropertyValueLink, CodeExpression>();
				kv.Add(pl, p1);
				ret.Add(this.DataDestination.TextDestination, kv);
			}
			return ret;
		}

		#endregion

		#region IDatabaseConnectionUserExt0 Members
		[Browsable(false)]
		public string Report32Usage()
		{
			string s = string.Empty;
			if (_dest != null)
			{
				if (_dest.DestinationType == EnumDataDestination.Database)
				{
					s = _dest.DatabaseDestination.Report32Usage();
				}
			}
			if (string.IsNullOrEmpty(s))
			{
				if (_source != null)
				{
					if (_source.SourceType == EnumDataSource.Database)
					{
						s = _source.DatabaseSource.Report32Usage();
					}
				}
			}
			return s;
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				List<Type> l = new List<Type>();
				if (DataDestination.DestinationType == EnumDataDestination.Database)
				{
					if (DataDestination.DatabaseDestination != null &&
						DataDestination.DatabaseDestination.DatabaseConnection != null &&
						DataDestination.DatabaseDestination.DatabaseConnection.ConnectionObject != null &&
						DataDestination.DatabaseDestination.DatabaseConnection.ConnectionObject.DatabaseType != null)
					{
						l.Add(DataDestination.DatabaseDestination.DatabaseConnection.ConnectionObject.DatabaseType);
					}
				}
				if (DataSource.SourceType == EnumDataSource.Database)
				{
					if (DataSource.DatabaseSource != null &&
						DataSource.DatabaseSource.DatabaseConnection != null &&
						DataSource.DatabaseSource.DatabaseConnection.ConnectionObject != null &&
						DataSource.DatabaseSource.DatabaseConnection.ConnectionObject.DatabaseType != null)
					{
						l.Add(DataSource.DatabaseSource.DatabaseConnection.ConnectionObject.DatabaseType);
					}
				}
				return l;
			}
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				List<Guid> l = new List<Guid>();
				if (DataDestination.DestinationType == EnumDataDestination.Database)
				{
					if (DataDestination.DatabaseDestination.ConnectionID != Guid.Empty)
					{
						l.Add(DataDestination.DatabaseDestination.ConnectionID);
					}
				}
				if (DataSource.SourceType == EnumDataSource.Database)
				{
					if (DataSource.DatabaseSource.DatabaseConnection.ConnectionGuid != Guid.Empty)
					{
						l.Add(DataSource.DatabaseSource.DatabaseConnection.ConnectionGuid);
					}
				}
				return l;
			}
		}

		#endregion

		#region IWebServerProgrammingSupport Members

		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}

		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "StartWithParameterValues") == 0)
			{
				if (!phpSupported())
				{
					return;
				}
				if (DataDestination != null && DataDestination.DatabaseDestination != null && DataDestination.DatabaseDestination.Fields.Count > 0)
				{
					if (this.PhpTimeoutInMinutes > 0)
					{
						addCode(code, "if($this->{0}->PhpTimeoutInMinutes>0) ini_set('max_execution_time', $this->{0}->PhpTimeoutInMinutes * 60);\r\n", this.Site.Name);
					}
					string tblVar = string.Format(CultureInfo.InvariantCulture, "tbl{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					if (DataSource.SourceType == EnumDataSource.Database)
					{
						EasyQuery qry = DataSource.DatabaseSource.QueryDef;
						if (qry != null)
						{
							//===fetch data==================================================================
							string mySql = string.Format(CultureInfo.InvariantCulture, "mysql{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
							addCode(code, "\t${0} = new JsonSourceMySql();\r\n", mySql);
							//
							addCode(code, "\t${0}->SetCredential($this->{1}->sourceCredential);\r\n", mySql, this.Site.Name);
							addCode(code, "\t${0}->SetDebug($this->DEBUG);\r\n", mySql);
							string sql = qry.SqlQuery;
							//sqlParams contains parameter names orderred by the appearance in sql, may duplicate
							string[] sqlParams = EasyQuery.GetParameterNames(sql, qry.NameDelimiterBegin, qry.NameDelimiterEnd);
							if (this.Parameters != null)
							{
								for (int i = 0; i < this.Parameters.Count; i++)
								{
									sql = sql.Replace(this.Parameters[i].Name, "?");
								}
							}
							string sqlVar = string.Format(CultureInfo.InvariantCulture, "sql{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
							addCode(code, "\t${0} = \"{1} \";\r\n", sqlVar, sql);
							addCode(code, "\t${0} = new JsonDataTable();\r\n", tblVar);
							addCode(code, "\t${0}->TableName = '{1}';\r\n", tblVar, qry.TableName);
							string psVar = string.Format(CultureInfo.InvariantCulture, "ps{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
							addCode(code, "\t${0} = array();\r\n", psVar);
							if (qry.Parameters != null)
							{
								DbParameterList dbps = new DbParameterList(qry.Parameters);
								for (int i = 0; i < sqlParams.Length; i++)
								{
									int k = dbps.GetIndex(sqlParams[i]);
									if (k < 0)
									{
										throw new ExceptionLimnorDatabase("Query Parameter [{0}] not found", sqlParams[i]);
									}
									DbCommandParam dp = dbps[k];
									string pVar = string.Format(CultureInfo.InvariantCulture, "p{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
									addCode(code, "\t${0} = new SqlClientParameter();\r\n", pVar);
									addCode(code, "\t${0}->name = '{1}';\r\n", pVar, dp.Name);
									addCode(code, "\t${0}->type = '{1}';\r\n", pVar, ValueConvertor.OleDbTypeToPhpMySqlType(dp.Type));
									addCode(code, "\t${0}->value = {1};\r\n", pVar, parameters[k]);
									addCode(code, "\t${0}[] = ${1};\r\n//\r\n", psVar, pVar);
								}
							}
							addCode(code, "\t${0}->GetData(${1},${2},${3});\r\n", mySql, tblVar, sqlVar, psVar);
							addCode(code, "\t$this->{0}->ErrorMessage = ${1}->errorMessage;\r\n", this.Site.Name, mySql);
							//===data fetched on tblVar=============================================================
						}
					}
					else if (DataSource.SourceType == EnumDataSource.TextFile)
					{
						string tblName = string.Format(CultureInfo.InvariantCulture, "srcTbl{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						string txtFileVar = string.Format(CultureInfo.InvariantCulture, "txtFile{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						addCode(code, "${0} = new DataSourceTextFile();\r\n", txtFileVar);
						addCode(code, "\t${0} = new JsonDataTable();\r\n", tblVar);
						addCode(code, "\t${0}->TableName = '{1}';\r\n", tblVar, tblName);
						//
						if (this.DataSource.TextSource.Delimiter == enumSourceTextDelimiter.Comma)
						{
							addCode(code, "\t${0}->delimiter=',';\r\n", txtFileVar);
						}
						else if (this.DataSource.TextSource.Delimiter == enumSourceTextDelimiter.TAB)
						{
							addCode(code, "\t${0}->delimiter='\\t';\r\n", txtFileVar);
						}
						if (this.DataSource.TextSource.HasHeader)
						{
							addCode(code, "\t${0}->hasHeader=true;\r\n", txtFileVar);
						}
						else
						{
							addCode(code, "\t${0}->hasHeader=false;\r\n", txtFileVar);
						}
						IPropertyValueLink fnl = this.DataSource.TextSource.GetPropertyLink("Filename");
						if (fnl != null && fnl.IsValueLinkSet())
						{
							addCode(code, "\t${0}->filepath={1};\r\n", txtFileVar, fnl.GetPhpScriptReferenceCode(code));
						}
						else
						{
							addCode(code, "\t${0}->filepath='{1}';\r\n", txtFileVar, this.DataSource.TextSource.Filename);
						}
						//
						addCode(code, "\t${0}->GetData(${1});\r\n", txtFileVar, tblVar);
						addCode(code, "\t$this->{0}->ErrorMessage = ${1}->errorMessage;\r\n",this.Site.Name, txtFileVar);
						addCode(code, "\tif(strlen(${0}->errorMessage) > 0)\r\n", txtFileVar);
						code.Add("\t{\r\n");
						code.Add("\t\tif($this->DEBUG)\r\n\t\t{\r\n");
						addCode(code, "\t\t\techo 'Error reading text file: '.${0}->filepath.' Error message:'.${0}->errorMessage;\r\n", txtFileVar);
						code.Add("\t\t\treturn;\r\n");
						code.Add("\t\t}\r\n");
						code.Add("\t}\r\n");
					}
					//
					string mySqlTVar = string.Format(CultureInfo.InvariantCulture, "mySqlT{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					addCode(code, "\t${0} = new JsonSourceMySql();\r\n", mySqlTVar);
					addCode(code, "\t${0}->SetCredential($this->{1}->destinationCredential);\r\n", mySqlTVar, this.Site.Name);
					addCode(code, "\t${0}->SetDebug($this->DEBUG);\r\n", mySqlTVar);
					//
					StringBuilder sbSql = new StringBuilder();
					StringBuilder sbSqlValues = new StringBuilder();
					sbSql.Append("Insert into ");
					sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterBegin);
					sbSql.Append(DataDestination.DatabaseDestination.TableName);
					sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterEnd);
					sbSql.Append(" (");
					sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterBegin);
					sbSql.Append(DataDestination.DatabaseDestination.Fields[0].Name);
					sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterEnd);
					sbSqlValues.Append("?");
					for (int i = 1; i < DataDestination.DatabaseDestination.Fields.Count; i++)
					{
						sbSql.Append(",");
						sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterBegin);
						sbSql.Append(DataDestination.DatabaseDestination.Fields[i].Name);
						sbSql.Append(DataDestination.DatabaseDestination.DatabaseConnection.NameDelimiterEnd);
						sbSqlValues.Append(",?");
					}
					sbSql.Append(") values (");
					sbSql.Append(sbSqlValues.ToString());
					sbSql.Append(")");
					//
					string myIVar = string.Format(CultureInfo.InvariantCulture, "mySqlI{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					addCode(code, "\t${0} = ${1}->GetMySqli();\r\n", myIVar, mySqlTVar);
					string statementVar = string.Format(CultureInfo.InvariantCulture, "state{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					addCode(code, "\t${0}=${1}->prepareStatement(${2},\"{3}\");\r\n", statementVar, mySqlTVar, myIVar, sbSql.ToString());
					//
					string paramsVar = string.Format(CultureInfo.InvariantCulture, "params{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					//
					code.Add("\t//loop through rows of $tbl\r\n");
					string idxVar = string.Format(CultureInfo.InvariantCulture, "idx{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					string rowVar = string.Format(CultureInfo.InvariantCulture, "row{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					addCode(code, "\tforeach (${0}->Rows as ${1} => ${2})\r\n", tblVar, idxVar, rowVar);
					code.Add("\t{\r\n");
					addCode(code, "\t\t${0}=array();\r\n", paramsVar);
					string paramVar = string.Format(CultureInfo.InvariantCulture, "param{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					for (int i = 0; i < DataDestination.DatabaseDestination.Fields.Count; i++)
					{
						addCode(code, "\t\t${0}=new SqlClientParameter();\r\n", paramVar);

						addCode(code, "\t\t${0}->name = '{1}';\r\n", paramVar, DataDestination.DatabaseDestination.Fields[i].Name);
						addCode(code, "\t\t${0}->type = '{1}';\r\n", paramVar, ValueConvertor.OleDbTypeToPhpMySqlType(DataDestination.DatabaseDestination.Fields[i].OleDbType));
						addCode(code, "\t\t${0}->value = ${1}->ItemArray[{2}];\r\n", paramVar, rowVar, i);
						addCode(code, "\t\t${0}[] = ${1};\r\n//\r\n", paramsVar, paramVar);
					}
					addCode(code, "\t\t${0}->executeStatement(${1},${2}, ${3});\r\n", mySqlTVar, myIVar, statementVar, paramsVar);
					//
					code.Add("\t}\r\n");
					//
					addCode(code, "\t${0}->free_result();\r\n", statementVar);
					addCode(code, "\t${0}->close();\r\n", statementVar);
					addCode(code, "\t${0}->close();\r\n", myIVar);
				}
				//
				code.Add(string.Format(CultureInfo.InvariantCulture, "\t$this->SetServerComponentName('{0}');\r\n", this.Site.Name));
			}
		}

		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("DataTransfer.php", Resource1.dataTransfer_php);
			return files;
		}

		public bool DoNotCreate()
		{
			return false;
		}

		public void OnRequestStart(System.Web.UI.Page webPage)
		{
			_webPage = webPage as JsonWebServerProcessor;
		}

		public void CreateOnRequestStartPhp(StringCollection code)
		{
			code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->DEBUG=$this->DEBUG;\r\n", this.Site.Name)); 
			code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->TransferType={1};\r\n", this.Site.Name, (int)(this.EndPointType)));
			code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->PhpTimeoutInMinutes={1};\r\n", this.Site.Name, this.PhpTimeoutInMinutes));
			if (DataDestination.DatabaseDestination.ConnectionID == Guid.Empty)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->destinationCredential='';\r\n", this.Site.Name));
			}
			else
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->destinationCredential=$this->{1};\r\n", this.Site.Name,
				ServerCodeutility.GetPhpMySqlConnectionName(DataDestination.DatabaseDestination.ConnectionID)));
			}
			if (DataSource.DatabaseSource.DatabaseConnection.ConnectionGuid == Guid.Empty)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0}->sourceCredential='';\r\n", this.Site.Name));
			}
			else
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"$this->{0}->sourceCredential=$this->{1};\r\n", this.Site.Name, ServerCodeutility.GetPhpMySqlConnectionName(DataSource.DatabaseSource.ConnectionID)));
			}

		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		public void CreateOnRequestFinishPhp(StringCollection code)
		{

		}

		public bool ExcludePropertyForPhp(string name)
		{
			if (string.CompareOrdinal(name, "Name") == 0)
				return true;
			return false;
		}

		public bool NeedObjectName
		{
			get { return true; }
		}

		#endregion

		#region IDynamicMethodParameters Members

		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object attrs)
		{
			if (string.CompareOrdinal(methodName, "StartWithParameterValues") == 0)
			{
				FieldList pl = Parameters;
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

		public object InvokeWithDynamicMethodParameters(string methodName, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] parameters, CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "StartWithParameterValues") == 0)
			{
				StartWithParameterValues(parameters);
			}
			return null;
		}

		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			return (string.CompareOrdinal(methodName, "StartWithParameterValues") == 0);
		}

		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}

		#endregion
	}
}
