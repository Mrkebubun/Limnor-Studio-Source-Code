/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Data;
using WindowsUtility;
using VPL;

namespace LimnorDatabase
{
	[DesignTimeColumnsHolder]
	[ToolboxBitmapAttribute(typeof(EasyGridDetail), "Resources.detailsdatagrid.bmp")]
	[Description("This is a data grid for showing detail records in a master-detail relationship. The master records may be provided by an EasyGrid or an EasyDataSet")]
	public class EasyGridDetail : EasyGrid
	{
		#region fields and constructors
		private IMasterSource _master;
		private string _relname;
		private EventHandler _masterDataFill;
		private EventHandler _afterMasterUpdate;
		private EventHandler _masterIsEmptyChange;
		public EasyGridDetail()
		{
		}
		#endregion
		#region private methods
		private void bindData(IMasterSource master)
		{
			_master = master;
			if (_master != null)
			{
				QueryDef.DatabaseConnection = _master.DatabaseConnection;
				QueryDef.DataStorage = _master.DataStorage;
				if (_masterDataFill == null)
				{
					_masterDataFill = new EventHandler(_master_DataFilled);
				}
				else
				{
					_master.DataFilled -= _masterDataFill;
					_master.CurrentRowIndexChanged -= _masterDataFill;
				}
				_master.DataFilled += _masterDataFill;
				if (_afterMasterUpdate == null)
				{
					_afterMasterUpdate = new EventHandler(_master_afterUpdate);
				}
				else
				{
					_master.AfterUpdate -= _afterMasterUpdate;
				}
				if (_masterIsEmptyChange == null)
				{
					_masterIsEmptyChange = new EventHandler(_master_isEmptyChange);
				}
				else
				{
					_master.IsEmptyChanged -= _masterIsEmptyChange;
				}
				_master.IsEmptyChanged += _masterIsEmptyChange;
				_master.AfterUpdate += _afterMasterUpdate;
				_master.EnterAddingRecord += new EventHandler(_master_EnterAddingRecord);
				_master.LeaveAddingRecord += new EventHandler(_master_LeaveAddingRecord);
			}
		}

		void _master_LeaveAddingRecord(object sender, EventArgs e)
		{
			if (!this.Enabled)
			{
				this.Enabled = _enabled;
			}
		}
		private bool _enabled;
		void _master_EnterAddingRecord(object sender, EventArgs e)
		{
			_enabled = this.Enabled;
			this.Enabled = false;
		}
		private void _master_isEmptyChange(object sender, EventArgs e)
		{
			_master_afterUpdate(sender, e);
		}
		private void _master_afterUpdate(object sender, EventArgs e)
		{
			EventArgsDataFill e0 = e as EventArgsDataFill;
			if (e0 != null)
			{
				if (e0.RowCount == 0)
				{
					ShowNewButton(false);
				}
				else
				{
					ShowNewButton(true);
					processOnCellEnter();
				}
			}
		}
		private void _master_DataFilled(object sender, EventArgs e)
		{
			if (CurrentDataTable == null)
			{
				EasyQuery.LogMessage2("{0} - Master data filled. {0} does not exist. Execute Query.", TableName);
				Query();
			}
			else
			{
				EasyQuery.LogMessage2("{0} - Master data filled. {0} exist. Execute OnBindDataSource.", TableName);
				OnBindDataSource();
			}
			OnEmptyRecord(this, EventArgs.Empty);
		}
		#endregion
		#region Methods
		protected override void OnQueryObjectCreated()
		{
		}
		protected override void OnBindDataSource()
		{
			try
			{
				EasyQuery.LogMessage2("{0} - EasyDetailsGrid.OnBindDataSource starts...", TableName);
				this.DataSource = null;
				if (QueryDef.DataStorage != null && QueryDef.DataStorage.Tables.Count > 0
					&& _master != null && _master.DataStorage != null)
				{
					if (QueryDef.DataStorage.Tables[TableName] != null && _master.DataStorage.Tables[_master.TableName] != null)
					{
						if (string.IsNullOrEmpty(_relname))
						{
							_relname = string.Format(System.Globalization.CultureInfo.InvariantCulture, "rel{0}", (UInt32)(Guid.NewGuid().GetHashCode()));
						}
						bool bExist = false;
						foreach (DataRelation d0 in _master.DataStorage.Relations)
						{
							if (string.CompareOrdinal(d0.RelationName, _relname) == 0)
							{
								bExist = true;
								break;
							}
						}
						if (!bExist)
						{
							DataColumn[] masterColumns = _master.GetColumnsByNames(MasterKeyColumns);
							DataColumn[] detailColumns = this.GetColumnsByNames(DetailsKeyColumns);
							if (masterColumns.Length == 0 || detailColumns.Length == 0)
							{
							}
							else
							{
								for (int i = 0; i < masterColumns.Length; i++)
								{
									masterColumns[i].ReadOnly = true;
								}
								for (int i = 0; i < detailColumns.Length; i++)
								{
									detailColumns[i].ReadOnly = true;
								}
								DataRelation dr = new DataRelation(_relname, masterColumns, detailColumns);
								foreach (DataRelation dr0 in _master.DataStorage.Relations)
								{
									if (string.Compare(_relname, dr0.RelationName, StringComparison.Ordinal) == 0)
									{
										_master.DataStorage.Relations.Remove(dr0);
										break;
									}
								}
								_master.DataStorage.EnforceConstraints = false;
								_master.DataStorage.Relations.Add(dr);
								bExist = true;
							}
						}
						if (bExist)
						{
							BindSource.DataSource = _master.MasterSource;
							BindSource.DataMember = _relname;
							BindSource.AllowNew = !ReadOnly;
						}
						else
						{
							DataTable tbl = QueryDef.DataStorage.Tables[TableName];
							if (tbl == null)
							{
								base.Query();
								tbl = QueryDef.DataStorage.Tables[TableName];
							}
							if (tbl != null)
							{
								BindSource.DataSource = _master.DataStorage;
								BindSource.DataMember = TableName;
							}
						}

						base.DataSource = BindSource;
						if (BindingContext != null)
						{
							BindingManagerBase bm = BindingContext[BindSource];
							if (bm != null)
							{
								QueryDef.SetBindingContext(bm);
							}
						}
						OnFillData();
					}
				}
			}
			catch (Exception e)
			{
				FormLog.NotifyException(ShowErrorMessage, e, "{0} - EasyDetailsGrid.OnBindDataSource", TableName);
			}
			EasyQuery.LogMessage2("{0} - EasyDetailsGrid.OnBindDataSource ends---", TableName);
		}

		public override bool Query()
		{
			QueryDef.DatabaseConnection = _master.DatabaseConnection;
			QueryDef.SetDataSet(_master.DataStorage);
			QueryDef.RemoveTable(TableName);
			OnRemoveTable();
			return base.Query();
		}

		protected override void OnRemoveTable()
		{
			DataTable tbl = QueryDef.DataStorage.Tables[TableName];
			if (tbl != null)
			{
				QueryDef.DataStorage.Tables.Remove(tbl);
			}
		}
		[Browsable(false)]
		internal string[] GetFieldNames(string propName)
		{
			if (string.CompareOrdinal(propName, "MasterKeyColumns") == 0)
			{
				if (_master != null)
				{
					return _master.GetFieldNames();
				}
			}
			else if (string.CompareOrdinal(propName, "DetailsKeyColumns") == 0)
			{
				return GetFieldNames();
			}
			return null;
		}
		[Browsable(false)]
		public override bool OnBeforeSetSQL()
		{
			if (_master == null)
			{
				FormMessage.DisplayMessage("Set SQL", "Master property has not been specified. Please set the Master property first.");
				return false;
			}
			return true;
		}
		[Browsable(false)]
		public override bool CanShowNewButton()
		{
			if (_master != null)
			{
				if (_master.RowCount > 0)
				{
					this.AllowUserToAddRows = AllowAddNewrow;
					return true;
				}
			}
			if (this.AllowUserToAddRows)
			{
				AllowAddNewrow = true;
			}
			this.AllowUserToAddRows = false;
			return false;
		}
		#endregion
		#region Properties
		[Category("Database")]
		public override bool HasData
		{
			get
			{
				if (IsQuerying)
				{
					return false;
				}
				return base.HasData;
			}
		}
		[Browsable(false)]
		public override bool IsQuerying
		{
			get
			{
				if (base.IsQuerying)
				{
					return true;
				}
				if (_master != null)
				{
					return _master.IsQuerying;
				}
				return false;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public override ConnectionItem DatabaseConnection
		{
			get
			{
				if (_master != null)
				{
					return _master.DatabaseConnection;
				}
				return null;
			}
			set
			{
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public override Guid ConnectionID
		{
			get
			{
				return base.ConnectionID;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[XmlIgnore]
		[ReadOnly(true)]
		public override string DefaultConnectionString
		{
			get
			{
				return base.DefaultConnectionString;
			}
			set
			{
				base.DefaultConnectionString = value;
			}
		}
		[Browsable(false)]
		[XmlIgnore]
		[ReadOnly(true)]
		public override Type DefaultConnectionType
		{
			get
			{
				return base.DefaultConnectionType;
			}
			set
			{
				base.DefaultConnectionType = value;
			}
		}
		[Category("Database")]
		[Description("Key fields of the master data table for setting the master-details relationship")]
		[Editor(typeof(TypeEditorSelectFieldNames), typeof(UITypeEditor))]
		public string[] MasterKeyColumns
		{
			get;
			set;
		}
		[Category("Database")]
		[Description("Key fields of the details data table for setting the master-details relationship")]
		[Editor(typeof(TypeEditorSelectFieldNames), typeof(UITypeEditor))]
		public string[] DetailsKeyColumns
		{
			get;
			set;
		}
		[Category("Database")]
		[Description("This is the component holding the master records")]
		public IMasterSource Master
		{
			get
			{
				return _master;
			}
			set
			{
				if (this != value && _master != value)
				{
					bindData(value);
				}
			}
		}
		#endregion
	}
}
