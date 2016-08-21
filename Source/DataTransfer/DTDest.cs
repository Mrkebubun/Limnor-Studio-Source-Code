/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using VPL;
using System.Data.Common;
using TraceLog;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;

namespace LimnorDatabase.DataTransfer
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DTDest : IEPDataDest, IDatabaseTableUser, IDevClassReferencerHolder
	{
		#region fields and constructors
		private ConnectionItem connect = null;
		private FieldCollection _fields = null;
		private string _defConnectionString;
		private Type _defConnectionType;
		private IDevClassReferencer _owner;
		public DTDest()
		{
		}
		#endregion
		#region methods
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
		#region Properties
		[RefreshProperties(RefreshProperties.All)]
		[Description("Gets and sets the fields of the database table to receive data")]
		[Editor(typeof(TypeEditorTableFieldsSelection), typeof(UITypeEditor))]
		[DefaultValue(null)]
		public FieldCollection Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				_fields = value;
			}
		}
		[Description("Gets and sets the table name of the database table to receive data")]
		[Editor(typeof(TypeSelectorTable), typeof(UITypeEditor))]
		[DefaultValue(null)]
		public string TableName { get; set; }

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

		[DefaultValue(null)]
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
		[DefaultValue(null)]
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
		#region Methods
		public void SetOwner(IDevClassReferencer owner)
		{
			_owner = owner;
		}
		public string ReceiveData(DataTable tblSrc, bool bSilent)
		{
			string error = string.Empty;
			try
			{
				if (TableName.Length > 0 && _fields != null && connect != null && tblSrc != null)
				{
					if (_fields.Count > 0 && tblSrc.Columns.Count >= _fields.Count)
					{
						if (tblSrc.Rows.Count > 0)
						{
							bool bNeedUpdate = false;
							bool bNeedUpdateRun = false;
							int i;
							for (i = 0; i < _fields.Count; i++)
							{
								if (_fields[i].Indexed)
								{
									bNeedUpdate = true;
									break;
								}
							}
							EnumParameterStyle pStyle = connect.ConnectionObject.ParameterStyle;
							string n1 = connect.ConnectionObject.NameDelimiterBegin;
							string n2 = connect.ConnectionObject.NameDelimiterEnd;
							string sInsert = StringUtility.FormatInvString(
								"INSERT INTO {0}{1}{2} ({0}{3}{2}",
								n1, TableName, n2, _fields[0].Name);

							string sValues = _fields[0].GetParameterName(pStyle);
							string sUpdate = StringUtility.FormatInvString(
								"UPDATE {0}{1}{2} SET ",
								n1, TableName, n2);
							string sExist;
							if (connect.ConnectionObject.IsJet || connect.ConnectionObject.IsMSSQL)
							{
								sExist = StringUtility.FormatInvString(
									"SELECT TOP 1 1 FROM {0}{1}{2} WHERE ",
									n1, TableName, n2);
							}
							else
							{
								sExist = StringUtility.FormatInvString(
									"SELECT 1 FROM {0}{1}{2} WHERE ",
									n1, TableName, n2);
							}
							string sWhere = "";
							int k = 0, n = 0;
							if (bNeedUpdate)
							{
								if (_fields[0].Indexed)
								{
									sWhere = StringUtility.FormatInvString(
										"{0}{1}{2}={3}", n1, _fields[0].Name, n2, _fields[0].GetParameterName(pStyle));
								}
								else
								{
									sUpdate += StringUtility.FormatInvString("{0}{1}{2}={3}", n1, _fields[0].Name, n2, _fields[0].GetParameterName(pStyle));
									k++;
								}
								for (i = 0; i < _fields.Count; i++)
								{
									if (_fields[i].Indexed)
									{
										if (n == 0)
										{
											sExist += StringUtility.FormatInvString("{0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										}
										else
										{
											sExist += StringUtility.FormatInvString(" AND {0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										}
										n++;
									}
								}
							}
							for (i = 1; i < _fields.Count; i++)
							{
								sInsert += StringUtility.FormatInvString(",{0}{1}{2}", n1, _fields[i].Name, n2);
								sValues += "," + _fields[i].GetParameterName(pStyle);
								if (bNeedUpdate)
								{
									if (_fields[i].Indexed)
									{
										if (sWhere.Length == 0)
										{
											sWhere = StringUtility.FormatInvString("{0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										}
										else
										{
											sWhere += StringUtility.FormatInvString(" AND {0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										}
									}
									else
									{
										if (k == 0)
											sUpdate += StringUtility.FormatInvString("{0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										else
											sUpdate += StringUtility.FormatInvString(",{0}{1}{2}={3}", n1, _fields[i].Name, n2, _fields[i].GetParameterName(pStyle));
										k++;
									}
								}
							}
							if (bNeedUpdate && k > 0)
								bNeedUpdateRun = true;
							sInsert += ") VALUES (";
							sInsert += sValues;
							sInsert += ")";
							//
							connect.ConnectionObject.Open();
							//
							if (connect.ConnectionObject.IsMSSQL)
							{
								dbWrapper dbHelp = new dbWrapper();
								dbHelp.CreateCommand(connect.ConnectionObject);
								string sHelp = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT {0} ON", TableName);
								dbHelp.SetCommandText(sHelp);
								dbHelp.ExecuteNonQuery();
							}
							//
							dbWrapper dbUpdate = null;
							dbWrapper dbExists = null;
							dbWrapper dbInsert = new dbWrapper();
							dbInsert.CreateCommand(connect.ConnectionObject);
							dbInsert.SetCommandText(sInsert);
							//
							object vExist;
							bool bInsert;
							//
							for (i = 0; i < _fields.Count; i++)
							{
								dbInsert.AddCommandParameter(_fields[i], pStyle);
							}
							//
							if (bNeedUpdate)
							{
								if (bNeedUpdateRun)
								{
									dbUpdate = new dbWrapper();
									dbUpdate.CreateCommand(connect.ConnectionObject);
									dbUpdate.SetCommandText(StringUtility.FormatInvString("{0} WHERE {1}", sUpdate, sWhere));
								}
								dbExists = new dbWrapper();
								dbExists.CreateCommand(connect.ConnectionObject);
								dbExists.SetCommandText(sExist);
								for (i = 0; i < _fields.Count; i++)
								{
									if (_fields[i].Indexed)
										dbExists.AddCommandParameter(_fields[i], pStyle);
									else
									{
										if (bNeedUpdateRun)
											dbUpdate.AddCommandParameter(_fields[i], pStyle);
									}
								}
								if (bNeedUpdateRun)
								{
									for (i = 0; i < _fields.Count; i++)
									{
										if (_fields[i].Indexed)
										{
											dbUpdate.AddCommandParameter(_fields[i], pStyle);
										}
									}
								}
							}
							for (int m = 0; m < tblSrc.Rows.Count; m++)
							{
								bInsert = true;
								if (bNeedUpdate)
								{
									k = 0;
									for (i = 0; i < _fields.Count; i++)
									{
										if (_fields[i].Indexed)
										{
											dbExists.SetParameterValue(k, tblSrc.Rows[m][i]);
											k++;
										}
									}
									vExist = dbExists.ExecuteScalar();
									if (vExist != null && vExist != System.DBNull.Value)
									{
										bInsert = false;
									}
									if (!bInsert)
									{
										if (bNeedUpdateRun)
										{
											k = 0;
											for (i = 0; i < _fields.Count; i++)
											{
												if (!_fields[i].Indexed)
												{
													if (dbUpdate.Parameters[k].DbType == System.Data.DbType.Binary)
													{
														object v = tblSrc.Rows[m][i];
														if (v == null || v == System.DBNull.Value)
															dbUpdate.Parameters[k].Value = System.DBNull.Value;
														else
														{
															string s = v.ToString();
															byte[] bs = StringUtility.StringToBytes(s);
															dbUpdate.Parameters[k].Value = bs;
														}
													}
													else
													{
														dbUpdate.Parameters[k].Value = tblSrc.Rows[m][i];
													}
													k++;
												}
											}
											for (i = 0; i < _fields.Count; i++)
											{
												if (_fields[i].Indexed)
												{
													if (dbUpdate.Parameters[k].DbType == System.Data.DbType.Binary)
													{
														object v = tblSrc.Rows[m][i];
														if (v == null || v == System.DBNull.Value)
															dbUpdate.Parameters[k].Value = System.DBNull.Value;
														else
														{
															string s = v.ToString();
															byte[] bs = StringUtility.StringToBytes(s);
															dbUpdate.Parameters[k].Value = bs;
														}
													}
													else
													{
														dbUpdate.Parameters[k].Value = tblSrc.Rows[m][i];
													}
													k++;
												}
											}
											dbUpdate.ExecuteNonQuery();
										}
									}
								}
								if (bInsert)
								{
									for (i = 0; i < _fields.Count; i++)
									{
										object v = tblSrc.Rows[m][i];
										if (v == null || v == System.DBNull.Value)
										{
											dbInsert.Parameters[i].Value = System.DBNull.Value;
										}
										else
										{
											if (dbInsert.Parameters[i].DbType == System.Data.DbType.Binary)
											{
												string s = v.ToString();
												byte[] bs = StringUtility.StringToBytes(s);
												dbInsert.Parameters[i].Value = bs;
											}
											else
											{
												bool b;
												object v2 = VPLUtil.ConvertObject(v, EPField.ToSystemType(dbInsert.Parameters[i].DbType), out b); 
												dbInsert.Parameters[i].Value = v2;
											}
										}
									}
									dbInsert.ExecuteNonQuery();
								}
							}
							if (connect.ConnectionObject.IsMSSQL)
							{
								dbWrapper dbHelp = new dbWrapper();
								dbHelp.CreateCommand(connect.ConnectionObject);
								string sHelp = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT {0} OFF", TableName);
								dbHelp.SetCommandText(sHelp);
								dbHelp.ExecuteNonQuery();
							}
							connect.ConnectionObject.Close();
							//

						}
					}
				}
			}
			catch (Exception er)
			{
				error = ExceptionLimnorDatabase.FormExceptionText(er, "Error saving data to database");
				TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
				TraceLogClass.TraceLog.Log(er);
			}
			finally
			{
				connect.ConnectionObject.Close();
			}
			return error;
		}
		public override string ToString()
		{
			string s = TableName + "(";
			if (_fields != null)
			{
				if (_fields.Count > 0)
				{
					s += _fields[0].Name;
					for (int i = 1; i < _fields.Count; i++)
						s += "," + _fields[i].Name;
				}
			}
			s += ")";
			return s;
		}


		#endregion
		#region ICloneable Members

		public object Clone()
		{
			DTDest obj = new DTDest();
			obj.SetOwner(_owner);
			obj.TableName = TableName;
			if (_fields != null)
				obj._fields = (FieldCollection)_fields.Clone();
			if (connect != null)
			{
				obj.connect = (ConnectionItem)connect.Clone();
			}
			return obj;
		}

		#endregion

		#region IEPDataDest Members
		[Browsable(false)]
		public bool IsReady
		{
			get { return (connect != null && connect.ConnectionObject.IsConnectionReady); }
		}

		#endregion

		#region IDatabaseConnectionUserExt0 Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (connect != null)
			{
				if (connect.ConnectionObject != null)
				{
					if (connect.ConnectionObject.IsJet)
					{
						return "Jet Database Driver is used, which is a 32-bit software.";
					}
				}
			}
			return string.Empty;
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				List<Type> l = new List<Type>();
				if (connect != null && connect.ConnectionObject != null && connect.ConnectionObject.DatabaseType != null)
				{
					l.Add(connect.ConnectionObject.DatabaseType);
				}
				return l;
			}
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
		[Description("Connection to the database")]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				return connect;
			}
			set
			{
				connect = value;
			}
		}
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				List<Guid> l = new List<Guid>();
				if (connect != null && connect.ConnectionGuid != Guid.Empty)
				{
					l.Add(connect.ConnectionGuid);
				}
				return l;
			}
		}

		#endregion

		#region IDevClassReferencerHolder Members

		public IDevClassReferencer DevClass
		{
			get { return _owner; }
		}

		#endregion
	}
}
