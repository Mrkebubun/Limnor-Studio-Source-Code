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
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Data;
using VPL;
using System.Data.Common;
using System.Globalization;
using Limnor.WebServerBuilder;
using System.Collections.Specialized;

namespace LimnorDatabase
{
	[WebServerMember]
	[ToolboxBitmapAttribute(typeof(DatabaseExecuter), "Resources.dbExec.bmp")]
	[Description("This component executes database commands. It can be used for executing database stored procedures and other commands.")]
	public class DatabaseExecuter : EasyUpdator, IWebServerComponentCreator
	{
		#region fields and constructors
		private DataSet _dataset;
		private int[] _dataSizes;
		private ParameterDirection[] _paramDirections;
		private DbParameterList _plist;
		public DatabaseExecuter()
		{
		}
		public DatabaseExecuter(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		#endregion

		#region Properties
		[Category("Database")]
		public override bool IsStoredProc
		{
			get
			{
				return (ExecutionCommand.CommandType == enmNonQueryType.StoredProcedure);
			}
		}
		[Category("Database")]
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorCommand), typeof(UITypeEditor))]
		[Description("Database command to be executed")]
		public override SQLNoneQuery ExecutionCommand
		{
			get
			{
				if (base.ExecutionCommand == null)
				{
					SQLNoneQuery sq = new SQLNoneQuery();
					sq.CommandType = enmNonQueryType.StoredProcedure;
					base.ExecutionCommand = sq;
				}
				return base.ExecutionCommand;
			}
			set
			{
				base.ExecutionCommand = value;
			}
		}
		[Category("Database")]
		[Description("Gets a DataSet containing the data returned from executing the database commands.")]
		public DataSet Results
		{
			get
			{
				if (_dataset == null)
				{
					_dataset = new DataSet("Results");
				}
				return _dataset;
			}
		}
		[Browsable(false)]
		[Category("Database")]
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		[Description("Parameter directions")]
		public ParameterDirection[] Param_Directions
		{
			get
			{
				if (_paramDirections == null)
				{
					_paramDirections = new ParameterDirection[this.ParameterCount];
					for (int i = 0; i < this.ParameterCount; i++)
					{
						_paramDirections[i] = ParameterDirection.Input;
					}
				}
				else
				{
					if (_paramDirections.Length != this.ParameterCount)
					{
						_paramDirections = new ParameterDirection[this.ParameterCount];
						for (int i = 0; i < this.ParameterCount; i++)
						{
							_paramDirections[i] = ParameterDirection.Input;
						}
					}
				}
				return _paramDirections;
			}
			set
			{
				_paramDirections = value;
				if (_plist != null)
				{
					OnFormedParameterList(_plist);
				}
			}
		}
		[Browsable(false)]
		public int[] Param_DataSize
		{
			get
			{
				if (_dataSizes == null)
				{
					int n = Parameters.Count;
					_dataSizes = new int[n];
					for (int i = 0; i < n; i++)
					{
						_dataSizes[i] = ExecutionCommand.Parameters[i].DataSize;
					}
				}
				return _dataSizes;
			}
			set
			{
				if (value != null)
				{
					_dataSizes = new int[value.Length];
					for (int i = 0; i < value.Length; i++)
					{
						_dataSizes[i] = value[i];
					}
					if (_plist != null)
					{
						OnFormedParameterList(_plist);
					}
				}
			}
		}
		#endregion

		#region Methods
		[Browsable(false)]
		[NotForProgramming]
		public void SetParameterList(DbParameterList pl)
		{
			OnFormedParameterList(pl);
		}
		protected override void OnFormedParameterList(DbParameterList pl)
		{
			_plist = pl;
			for (int i = 0; i < pl.Count; i++)
			{
				if (_dataSizes != null && i < _dataSizes.Length)
				{
					pl[i].DataSize = _dataSizes[i];
				}
				if (_paramDirections != null && i < _paramDirections.Length)
				{
					pl[i].Direction = _paramDirections[i];
				}
				pl[i].DataSizeChange += new EventHandler(DatabaseExecuter_DataSizeChange);
				pl[i].DirectionChange += new EventHandler(DatabaseExecuter_DirectionChange);
			}
		}

		void DatabaseExecuter_DirectionChange(object sender, EventArgs e)
		{
			if (_plist != null && _paramDirections != null)
			{
				DbCommandParam dp = sender as DbCommandParam;
				if (dp != null)
				{
					for (int i = 0; i < _plist.Count; i++)
					{
						if (string.CompareOrdinal(dp.Name, _plist[i].Name) == 0)
						{
							if (i < _paramDirections.Length)
							{
								_paramDirections[i] = dp.Direction;
							}
							break;
						}
					}
				}
			}
		}

		void DatabaseExecuter_DataSizeChange(object sender, EventArgs e)
		{
			if (_plist != null && _dataSizes != null)
			{
				DbCommandParam dp = sender as DbCommandParam;
				if (dp != null)
				{
					for (int i = 0; i < _plist.Count; i++)
					{
						if (string.CompareOrdinal(dp.Name, _plist[i].Name) == 0)
						{
							if (i < _dataSizes.Length)
							{
								_dataSizes[i] = dp.DataSize;
							}
							break;
						}
					}
				}
			}
		}
		[Description("Execute ExecutionCommand as a database stored procedure or a script, depending on IsStoredProc property. If more than one Execute action is assigned to event ExecuteInOneTransaction then those Execute actions will be executed in one single database transaction.")]
		public override string Execute()
		{
			string sMsg = string.Empty;
			SetError(sMsg);
			ResetAffectedRows();
			SQLNoneQuery sql = base.ExecutionCommand;
			ConnectionItem connect = Connection;
			DbTransaction transaction = Transaction;
			if (sql != null && connect != null)
			{
				DbCommand cmd = connect.ConnectionObject.CreateCommand();
				if (transaction != null)
				{
					cmd.Transaction = transaction;
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
						cmd.CommandText = sql.SQL;
						if (sql.CommandType == enmNonQueryType.StoredProcedure)
						{
							cmd.CommandType = CommandType.StoredProcedure;
						}
						else
						{
							cmd.CommandType = CommandType.Text;
						}
						int nCount = ParameterCount;
						for (i = 0; i < nCount; i++)
						{
							DbParameter pam = cmd.CreateParameter();
							if (pstyle == EnumParameterStyle.LeadingQuestionMark)
							{
								if (sql.Param_Name[i].StartsWith("@", StringComparison.OrdinalIgnoreCase))
								{
									pam.ParameterName = string.Format(CultureInfo.InvariantCulture, "?{0}", sql.Param_Name[i].Substring(1));
								}
								else if (sql.Param_Name[i].StartsWith("?", StringComparison.OrdinalIgnoreCase))
								{
									pam.ParameterName = sql.Param_Name[i];
								}
								else
								{
									pam.ParameterName = string.Format(CultureInfo.InvariantCulture, "?{0}", sql.Param_Name[i]);
								}
							}
							else
							{
								if (sql.Param_Name[i].StartsWith("@", StringComparison.OrdinalIgnoreCase))
								{
									pam.ParameterName = sql.Param_Name[i];
								}
								else
								{
									pam.ParameterName = string.Format(CultureInfo.InvariantCulture, "@{0}", sql.Param_Name[i]);
								}
							}
							pam.DbType = ValueConvertor.OleDbTypeToDbType(sql.Param_OleDbType[i]);
							pam.Direction = this.Param_Directions[i];
							pam.Size = EPField.FieldDataSize(sql.Param_OleDbType[i], this.Param_DataSize[i]);
							pam.Value = ValueConvertor.ConvertByOleDbType(sql.Parameters[i].Value, sql.Param_OleDbType[i]);
							cmd.Parameters.Add(pam);
						}
						cmd.Prepare();
						DbDataReader dr = cmd.ExecuteReader();

						_dataset = new DataSet("Results");
						int n = 1;
						while (true)
						{
							DataTable tbl = new DataTable(string.Format("Table{0}", n));
							for (i = 0; i < dr.FieldCount; i++)
							{
								DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
								tbl.Columns.Add(dc);
							}
							_dataset.Tables.Add(tbl);
							while (dr.Read())
							{
								object[] vs = new object[dr.FieldCount];
								for (int k = 0; k < dr.FieldCount; k++)
								{
									vs[k] = dr.GetValue(k);
								}
								tbl.Rows.Add(vs);
							}
							n++;
							if (!dr.NextResult())
							{
								break;
							}
						}
						dr.Close();
						if (bClosed)
						{
							closeConnections();
						}
						for (i = 0; i < nCount; i++)
						{
							ParameterDirection pt = cmd.Parameters[i].Direction;
							if (pt != ParameterDirection.Input)
							{
								sql.Parameters[i].Value = cmd.Parameters[i].Value;
							}
						}
						FireExecuteFinish();
					}
					catch (Exception er)
					{
						if (transaction != null)
						{
							transaction.Rollback();
							transaction.Dispose();
							ResetTransaction();
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
			}
			return sMsg;
		}
		#endregion

		#region IWebServerComponentCreator Members

		public void CreateServerComponentPhp(StringCollection objectDecl, StringCollection initCode, ServerScriptHolder scripts)
		{
			string decName = string.Format(CultureInfo.InvariantCulture,
				"private ${0};\r\n", this.Site.Name);
			if (!objectDecl.Contains(decName))
			{
				objectDecl.Add(decName);
			}
			initCode.Add(string.Format(CultureInfo.InvariantCulture,
				"$this->{0} = new DatabaseExecuter();\r\n", this.Site.Name));
		}

		#endregion

		#region IWebServerComponentCreatorBase Members

		public bool RemoveFromComponentInitializer(string propertyName)
		{
			return false;
		}

		#endregion
	}
}
