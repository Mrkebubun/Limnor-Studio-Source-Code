using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Data.Common;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using LimnorUI;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
    /// query is provided by _query which generates cmdSelection;
    /// Data source is provided by cmdSelection; updating is done by cmdUpdate, cmdDeletion, and cmdInsert
	/// Wrap DataTable
	/// </summary>
    [Description("It shows data in a grid. The data come from database")]
    [ToolboxBitmapAttribute(typeof(EasyDataTable), "Resources.datagrid.bmp")]
    public class EasyDataTable : DataGrid, ICloneable, IDataBinder, IDataConsumer, ICustomTypeDescriptor, IPostDeserializeProcess
	{
		#region EPTable Special
        //==data grid========================
        public event System.EventHandler CurrentRowIndexChange = null;
        public event System.EventHandler RequeryFinished = null;
        //
        private EasyQuery _query;
        private EventHandler _onBeforePrepareCommands;
        private EventHandler _onBeforeQuery;
        private EventHandler _onAfterQuery;
        private FormClosingEventHandler _onFormClosing;
        //
        private System.Windows.Forms.ComboBox[] cbx = null; //look ups including Yes/No
        private System.Windows.Forms.Button[] bts = null; //datetime
        private int nCurrentRowIndex = -1;
        private int nCurrentCellColumn = -1;
        private System.Windows.Forms.Button btSave = null;

        //private string sCurCaption = "";
        //private bool bNewTable = false;
        private bool bNoEvents = false;
        private DataGridTableStyle myGridTableStyle = null;
        private bool bLoadingData = false;
        private bool pageLoaded = false;

        private uint _lastTableClick = 0;
        //===================================
		public event fnOnRowIndexChange OnRowIndexChange = null;
		//
        private static StringCollection _st_excludedPropertyNames;
        //
		const string Parent_ID="Parent_LinkID";
        //properties===================================
        private bool _autoSave;
        private bool _loop;
        private Color _AlternatingBackColor;
        private Color _BKcolor;
        private Color _Forecolor;
        private Color _HeaderColor;
        private Color _HeaderBKcolor;
        private bool _allowNew;
        private bool _allowDelete;
        private bool _HideSaveButton;
        private string _changedColumnName;
        private int _left;
        private int _top;
        private int _width;
        private int _height;
		//=============================================
		private bool bExecuting = false;
		protected int nNullRow = -1;
		protected bool bRowChanging = false;
		//
        //bool bFirstDone = false;
        //bool bNewCall=true;
        bool bLoaded;
		//
//		System.Threading.Thread thRefresh = null;
        static EasyDataTable()
        {
            _st_excludedPropertyNames = new StringCollection();
            //_st_excludedPropertyNames.Add("DataSource");
            _st_excludedPropertyNames.Add("TableStyles");
        }
		//
        //public void LinkToTable(DataTable tbl)
        //{
        //    btSave.Visible = false;
        //    this.ReadOnly = true;
        //    this.DataSource = tbl;
        //    fields = new FieldList();
        //    for(int i=0;i<tbl.Columns.Count;i++)
        //    {
        //        EPField fld = new EPField();
        //        fld.Name = tbl.Columns[i].ColumnName;
        //        fld.FieldCaption = fld.Name;
        //        fld.FieldText = fld.Name;
        //        fld.DataSize = tbl.Columns[i].MaxLength;
        //        fld.OleDbType = EPField.ToOleDBType(tbl.Columns[i].DataType);
        //        fields.AddField(fld);
        //    }
        //    this.Refresh();
        //}
		//
		private void gotoRow(int n)
		{
			if(this.CurrentRowIndex != n)
			{
				if( !ReadOnly )
				{
					if(_autoSave)
					{
						onSaveButtonClick(this,new System.EventArgs());
					}
				}
				this.CurrentRowIndex = n;
				this.Select(n);
			}
		}
		private void EPTable_RowChanging(object sender, DataRowChangeEventArgs e)
		{
			if( !pageLoaded )
				return;
			if( e.Action == System.Data.DataRowAction.Commit && !bLoadingData )
			{
                if (e.Row.RowState == System.Data.DataRowState.Modified)
                {
                    if (_query != null)
                    {
                        _query.OnRowChanging(sender, e);
                    }
                }
			}
			if( !bLoadingData )
			{
				switch( e.Action )
				{
					case System.Data.DataRowAction.Add:
                        if (BeforeRowAdd != null)
                        {
                            BeforeRowAdd(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Change:
                        if (BeforeRowChange != null)
                        {
                            BeforeRowChange(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Delete:
                        if (BeforeRowDelete != null)
                        {
                            BeforeRowDelete(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Commit:
						if( e.Row.RowState == System.Data.DataRowState.Deleted )
						{
                            if (AfterRowDelete != null)
                            {
                                AfterRowDelete(this, EventArgs.Empty);
                            }
						}
						break;
				}
			}
		}
		protected void EPTable_RowChanged(object sender,DataRowChangeEventArgs e)
		{
			if( !pageLoaded )
				return;
			if( e.Action == System.Data.DataRowAction.Add && !bLoadingData )
			{
                EasyQuery qry = _query;
				if( qry != null )
				{
                    qry.OnRowChanged(this, e);
                    //bool bOK=false;
                    //if( RowIDFields != null )
                    //{
                    //    bOK = (cmdTimestamp != null);
                    //}
                    //if( bOK )
                    //{
                    //    try
                    //    {
                    //        string s = "";
                    //        s = cmdTimestamp.Parameters[0].ParameterName.Substring(1); //remove @
                    //        bOK = (e.Row[s] == System.DBNull.Value);
                    //        if (bOK)
                    //        {
                    //            for (int i = 0; i < RowIDFields.Count; i++)
                    //            {
                    //                if (e.Row[RowIDFields[i].Name] == System.DBNull.Value)
                    //                {
                    //                    bOK = false;
                    //                    break;
                    //                }
                    //                cmdTimestamp.Parameters[i + 1].Value = e.Row[RowIDFields[i].Name];
                    //            }
                    //        }
                    //        if (bOK)
                    //        {
                    //            if (cmdTimestamp.Connection.State == System.Data.ConnectionState.Closed)
                    //                cmdTimestamp.Connection.Open();
                    //            cmdTimestamp.Parameters[0].Value = System.DateTime.Now;
                    //            cmdTimestamp.ExecuteNonQuery();
                    //        }
                    //    }
                    //    catch (Exception er)
                    //    {
                    //        FormLog.NotifyException(er);
                    //    }
                    //    finally
                    //    {
                    //        if (cmdTimestamp != null)
                    //        {
                    //            if (cmdTimestamp.Connection.State != System.Data.ConnectionState.Closed)
                    //            {
                    //                cmdTimestamp.Connection.Close();
                    //            }
                    //        }
                    //    }
                    //}
                    //bOK = false;
                    //int nCount = 0;
                    //if(cmdSelection != null)
                    //{
                    //    bOK = true;
                    //    nCount = cmdSelection.Parameters.Count; 
                    //}
                    //if( bOK )
                    //{
                    //    EPParentTable pTable = _pTable;// propDescs[ID_ParentTable].objProperty.getCoreValue() as EPParentTable;
                    //    if( pTable != null )
                    //    {
                    //        if( pTable.JoinFields != null )
                    //        {
                    //            for(int i=0;i<pTable.JoinFields.Length;i++)
                    //            {
                    //                for(int n=0;n<nCount;n++)
                    //                {
                    //                    if( qry.paramMap[n].Name == pTable.JoinFields[i].field2.Name )
                    //                    {
                    //                        e.Row[pTable.JoinFields[i].field2.Name] = cmdSelection.Parameters[n].Value;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
				}
				//update time stamp
			}
			if( !bLoadingData )
			{
				switch( e.Action )
				{
					case System.Data.DataRowAction.Add:
                        if (AfterRowAdd != null)
                        {
                            AfterRowAdd(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Change:
                        if (AfterRowChange != null)
                        {
                            AfterRowChange(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Delete:
                        if (AfterRowDelete != null)
                        {
                            AfterRowDelete(this, EventArgs.Empty);
                        }
						break;
					case System.Data.DataRowAction.Commit:
						if(e.Row.RowState == DataRowState.Detached)
						{
                            if (AfterRowDelete != null)
                            {
                                AfterRowDelete(this, EventArgs.Empty);
                            }
						}
						break;
				}
			}
		}
        //protected void addDeletedRow()
        //{
        //    if( deletedRows == null )
        //        deletedRows = new DataValues();
        //    deletedRows.AddRow(RowIDFields);
        //}
        //protected void updateDeletedRows()
        //{
        //    if( deletedRows != null )
        //    {
        //        try
        //        {
        //            if (cmdDeletion != null)
        //            {
        //                int n = deletedRows.Count;
        //                DataValueRow r;
        //                for (int i = 0; i < n; i++)
        //                {
        //                    r = deletedRows[i];
        //                    for (int j = 0; j < r.row.Length; j++)
        //                    {
        //                        cmdDeletion.Parameters[j].Value = r.row[j];
        //                    }
        //                    cmdDeletion.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch(Exception er)
        //        {
        //            FormLog.NotifyException(er);
        //        }
        //    }
        //    deletedRows = null;
        //}
        //public void ResetDataForm()
        //{
        //    frmData = null;
        //}
		public bool FieldExists(string sName)
		{
			if( _query != null )
			{
				if( _query.Fields[sName] != null )
					return true;
			}
			return false;
		}
        //public FieldList GetUniqueKeyFields()
        //{
        //    if( RowIDFields == null )
        //    {
        //        RowIDFields = new FieldList();
        //        //FieldList fieldsAll = propDescs[ID_Fields].objProperty.getCoreValue() as FieldList;
        //        for(int i=0;i<fields.Count;i++)
        //        {
        //            if( fields[i].IsIdentity )
        //            {
        //                RowIDFields.AddField(fields[i]);
        //                UseID = true;
        //                break;
        //            }
        //        }
        //        if( RowIDFields.Count == 0 )
        //        {
        //            UseID = false;
        //            for(int i=0;i<fields.Count;i++)
        //            {
        //                if( fields[i].Indexed )
        //                    RowIDFields.AddField(fields[i]);
        //            }
        //        }
        //    }
        //    return RowIDFields;
        //}
        internal dlgPropFields GetFieldsDialog()
        {
            if (_query != null)
            {
                if (_query.Fields != null)
                {
                    if (_query.Fields.Count > 0)
                    {
                        dlgPropFields dlg = new dlgPropFields();
                        dlg.LoadData(_query);
                        dlg.ownerPerformer = this;
                        return dlg;
                    }
                }
            }
            return null;
        }
        internal void SetFieldsAfterFieldsDialog(FieldList fields)
        {
            EasyQuery qry = _query;
            qry.Fields = fields;
            //fldsCur = fields;
            if (qry.UpdatableTableName == null || qry.UpdatableTableName.Length == 0)
            {
                string mainTable = "";
                for (int i = 0; i < qry.Fields.Count; i++)
                {
                    if (qry.Fields[i].Indexed)
                    {
                        if (qry.Fields[i].FromTableName == null || qry.Fields[i].FromTableName.Length == 0)
                        {
                            mainTable = "";
                            break;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(mainTable))
                            {
                                if (qry.Fields[i].FromTableName != mainTable)
                                {
                                    mainTable = "";
                                    break;
                                }
                            }
                            else
                            {
                                mainTable = qry.Fields[i].FromTableName;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(mainTable))
                {
                    qry.UpdatableTableName = mainTable;
                }
            }
        }
		public bool showFieldsDialog(Form frmOwner,ref object retValue)
		{
			EasyQuery qry = _query;
			if( qry != null )
			{
				if( qry.Fields != null )
				{
					if( qry.Fields.Count > 0 )
					{
						dlgPropFields dlg = new dlgPropFields();
						dlg.LoadData(qry);
						dlg.ownerPerformer = this;
						if( dlg.ShowDialog(frmOwner) == System.Windows.Forms.DialogResult.OK )
						{
							qry.Fields = dlg.fields;
                            //fldsCur = dlg.fields;
							if(qry.UpdatableTableName == null || qry.UpdatableTableName.Length == 0)
							{
								string mainTable = "";
								for(int i=0;i<qry.Fields.Count;i++)
								{
									if(qry.Fields[i].Indexed)
									{
										if(qry.Fields[i].FromTableName == null || qry.Fields[i].FromTableName.Length == 0)
										{
											mainTable = "";
											break;
										}
										else
										{
											if(!string.IsNullOrEmpty(mainTable))
											{
												if(qry.Fields[i].FromTableName != mainTable)
												{
													mainTable = "";
													break;
												}
											}
											else
											{
												mainTable = qry.Fields[i].FromTableName;
											}
										}
									}
								}
								if(!string.IsNullOrEmpty(mainTable))
								{
									qry.UpdatableTableName = mainTable;
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		}
        //public bool showParamsFieldsDialog(System.Windows.Forms.Form frmOwner,ref object retValue)
        //{
        //    EasyQuery qry = _query;
        //    if( qry != null )
        //    {
        //        FieldList parms = qry.Parameters;
        //        if( parms != null )
        //        {
        //            if( parms.Count > 0 )
        //            {
        //                dlgPropParams dlg = new dlgPropParams();
        //                dlg.LoadData(parms);
        //                if( dlg.ShowDialog(frmOwner) == System.Windows.Forms.DialogResult.OK )
        //                {
        //                    qry.Parameters = dlg.fields;
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}
		public void SetFieldValue(string fieldName,object val)
		{
			System.Data.DataRow dw = CurrentRow;
			if( dw != null )
			{
				try
				{
					dw[fieldName] = val;
					if( _query != null )
					{
						EPField fld = _query.Fields[fieldName];
						if( fld != null )
							fld.SetValue(val);
					}
				}
				catch(Exception er)
				{
					FormLog.NotifyException(er);
				}
			}
		}
		public event System.EventHandler NewRecordAdded = null;
		public void LinkConsumeData(IDataConsumer childTable)
		{
			AskCurrentRowData(childTable);
            NewRecordAdded += new EventHandler(childTable.OnAddNewRecord);
		}
		public void AskCurrentRowData(IDataConsumer childTable)
		{
			System.Data.DataRow dw = CurrentRow;
			BLOBRow row = null;
			if( _query != null )
			{
				row = _query.GetBlobRow(dw);
			}
			childTable.OnGetRow(this,dw,row);
		}
        public string CurrentColumnCaption
        {
            get
            {
                EasyQuery qry = _query;
                if (qry != null && qry.Tables.Count > 0)
                {
                    if (qry.Tables[0] != null)
                    {
                        if (this.CurrentCell.ColumnNumber >= 0 && this.CurrentCell.ColumnNumber < qry.Tables[0].Columns.Count)
                        {
                            return qry.Tables[0].Columns[this.CurrentCell.ColumnNumber].Caption;
                        }
                    }
                }
                return string.Empty;
            }
        }
        //protected void onBeforeBind()
        //{
        //    if (_query != null)
        //    {
        //        _query.Tables[0].DefaultView.AllowNew = _allowNew;
        //        _query.Tables[0].DefaultView.AllowDelete = AllowDelete;
        //    }
        //}
        //[Description("The SQL query to get the data. The _query Builder helps you build SQL query.")]
        //public EasyQuery _query
        //{
        //    get
        //    {
        //        EasyQuery qry = DataSource as EasyQuery;
        //        //if (qry == null)
        //        //{
        //        //    if (DataBindings.Count > 0)
        //        //    {
        //        //        for (int i = 0; i < DataBindings.Count; i++)
        //        //        {
        //        //            qry = DataBindings[0].DataSource as EasyQuery;
        //        //            if (qry != null)
        //        //            {
        //        //                break;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        return qry;
        //    }
            //set
            //{
            //    _query = value;
            //    QueryDatabase();
            //}
        //}
        public DataRow CurrentRow
        {
            get
            {
                try
                {
                    //if(this.CurrentRowIndex >= 0)
                    //{
                    //    return _query.Tables[0].Rows[this.CurrentRowIndex];
                    //}
                    EasyQuery qry = _query;
                    if (qry != null && qry.Tables.Count > 0)
                    {
                        if (qry.Tables[0] != null)
                        {
                            BindingManagerBase bmb = BindingContext[qry.Tables[0].DefaultView];
                            DataRowView drv = (DataRowView)bmb.Current;
                            return drv.Row;
                        }
                    }
                }
                catch//(Exception e)
                {
                    //FormLog.NotifyException(e);
                }
                return null;
            }
        }
		public void onParentRowIndexChange(object sender,System.Data.DataRow row,BLOBRow blobRow)
		{
			try
			{
                //if( row != null )
                //{
                //    if( _pTable != null )
                //    {
                //        EasyQuery qry = _query;
                //        if( qry != null )
                //        {
                //            //byte dbType = qry.ServerType;
                //            bool bOK = (cmdSelection != null);
                //            int npCount = 0;
                //            if (bOK)
                //            {
                //                npCount = cmdSelection.Parameters.Count;
                //            }
							
                //            if( bOK )
                //            {
                //                for(int i=0;i<_pTable.JoinFields.Length;i++)
                //                {
                //                    for(int n=0;n<npCount;n++)
                //                    {
                //                        if( qry.paramMap[n].Name == _pTable.JoinFields[i].field2.Name )
                //                        {
                //                            cmdSelection.Parameters[n].Value = row[_pTable.JoinFields[i].field1.Name];
                //                        }
                //                    }
                //                }
                //                if (dataSet == null)
                //                {
                //                    dataSet = new System.Data.DataSet("DataSource");
                //                }
                //                else
                //                {
                //                    if (dataSet.Tables.Count > 0)
                //                    {
                //                        dataSet.Tables[0].Rows.Clear();
                //                    }
                //                }
                //                if (_da == null)
                //                {
                //                    _da = DataAdapterFinder.CreateDataAdapter(cmdSelection);
                //                }
                //                _da.Fill(dataSet);
                //                bindData();
                //                onRowIndexChanged();
                //            }
                //        }
                //    }
                //}
			}
			catch(Exception er)
			{
				FormLog.NotifyException(er);
			}
		}
		protected void onLookup()
		{
			if( bLoaded )
			{
                if (Lookup != null)
                {
                    Lookup(this, EventArgs.Empty);
                }
			}
		}
		protected void OnCellNumberChanged()
		{
			if( bNoEvents )
				return;
            if (ColChange != null)
            {
                ColChange(this, EventArgs.Empty);
            }
		}
		protected void onRowIndexChanged()
		{
			if( bNoEvents )
				return;
			try
			{
				System.Data.DataRow dw = CurrentRow;
                if (_query != null)
                {
                    _query.SetFieldValues(dw);
                }
                //if( RowIDFields != null )
                //{
                //    bool bOK = ( dw != null );
                //    if( bOK )
                //    {
                //        for(int i=0;i<RowIDFields.Count;i++)
                //        {
                //            RowIDFields[i].Value = dw[RowIDFields[i].Name];
                //        }
                //    }
                //    else
                //    {
                //        for(int i=0;i<RowIDFields.Count;i++)
                //        {
                //            RowIDFields[i].Value = null;
                //        }
                //    }
                //}
                //if( fields != null )
                //{
                //    int i;
                //    if( dw == null )
                //    {
                //        for(i=0;i<fields.Count;i++)
                //        {
                //            fields[i].Value = null;
                //        }
                //    }
                //    else
                //    {
                //        for(i=0;i<fields.Count;i++)
                //        {
                //            fields[i].Value = dw[i];
                //        }
                //    }
                //}
				if( pageLoaded && _query != null )
				{
					if( OnRowIndexChange != null )
					{
						BLOBRow row = _query.GetBlobRow(dw);
						bRowChanging = true;
						try
						{
							OnRowIndexChange(this,dw,row);
						}
						catch
						{
						}
						bRowChanging = false;
					}
                    if (CursorMove != null)
                    {
                        CursorMove(this, EventArgs.Empty);
                    }
				}
			}
			catch(Exception er)
			{
				FormLog.NotifyException(er);
			}
		}
		protected override void OnDoubleClick(EventArgs e)
		{
			if(!bExecuting)
			{
				base.OnDoubleClick (e);
			}
		}
		protected void OnFireDoubleClick(EventArgs e)
		{
            OnDoubleClick(e);
		}
		protected void OnFireClick(EventArgs e)
		{
			if(!bExecuting)
			{
				OnClick(e);
			}
		}
		protected void OnFireGotFocus(EventArgs e)
		{
			if(!bExecuting)
			{
                if (CellFocus != null)
                {
                    CellFocus(this, e);
                }
			}
		}
		public int GetBlobFieldIndex(string fld,bool isBinary)
		{
			if( isBinary )
			{
				if(	_query != null )
				{
                    return _query.GetBlobFieldIndex(fld);
				}
			}
			else
			{
                if (_query != null)
                {
                    for (int i = 0; i < _query.Fields.Count; i++)
                    {
                        if (_query.Fields[i].Name == fld)
                            return i;
                    }
                }
			}
			return -1;
		}
		public void SaveBlob(int index,EPBLOB blob)
		{
			if( _query != null && nCurrentRowIndex >= 0 )
			{
				_query.SaveBlob(index, blob);
			}
		}
        public void SaveField(int index, object v)
        {
            if (_query != null && index >= 0 && nCurrentRowIndex >= 0)
            {
                if (_query.Tables[0] != null)
                {
                    if (index < _query.Tables[0].Columns.Count)
                    {
                        System.Data.DataRow dw = CurrentRow;
                        //if( nCurrentRowIndex < dataSet.ds.Tables[TableName].Rows.Count )
                        if (dw != null)
                        {
                            dw.BeginEdit();
                            dw[index] = v;
                            dw.EndEdit();
                        }
                    }
                }
            }
        }

        //void frmData_OnRowIndexChange(object sender,System.EventArgs e)
        //{
        //    if( frmData != null )
        //    {
        //        try
        //        {
        //            this.CurrentRowIndex = frmData.CurrentRowIndex;
        //            this.nCurrentCellColumn = frmData.CurrentColumnIndex;
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}
        //public void CloseConnections()
        //{
        //    EasyQuery qry = _query;
        //    if (qry != null)
        //    {
        //        qry.CloseConnection();
        //    }
        //}
		private void page_OnPageWindowStateChange(Form frmOwner, bool ChildVisible)
		{
            //if( frmData != null )
            //{
            //    if( ChildVisible )
            //    {
            //        frmData.Visible = true;
            //        frmData.Owner = frmOwner;
            //    }
            //    else
            //        frmData.Visible = false;
            //}
		}
		public bool ActAsVisible()
		{
            return Visible;
		}
		
		#endregion
		
		#region Initilializer
		public EasyDataTable()
		{
			Init();
		}
		private void Init()
		{
            _left = this.Left;
            _top = Top;
            _width = ClientSize.Width;
            _height = ClientSize.Height;
            //
            this.AllowSorting = false;
            btSave = new System.Windows.Forms.Button();
            btSave.Parent = this;
            btSave.Width = 20;
            btSave.Height = 20;
            btSave.Top = 0;
            btSave.Image = Resource1.save;
            btSave.Left = this.Width - btSave.Width;
            btSave.BackColor = System.Drawing.Color.FromArgb(255, 236, 233, 216);
            btSave.Click += new System.EventHandler(onSaveButtonClick);
            //
            this.Scroll += new EventHandler(EPDataGrid_Scroll);
            //
            //this.BindingContextChanged += new EventHandler(EasyDataGrid_BindingContextChanged);
            //this.DataBindings.CollectionChanged += new CollectionChangeEventHandler(DataBindings_CollectionChanged);
		}

        //void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        //{
        //    //EasyQuery qry = _query;
        //    //if (qry != null)
        //    //{
        //    //    qry.BeforeQuery += new EventHandler(qry_BeforeQuery);
        //    //    qry.AfterQuery += new EventHandler(qry_AfterQuery);
        //    //}
        //}

        //void EasyDataGrid_BindingContextChanged(object sender, EventArgs e)
        //{
        //    //EasyQuery qry = _query;
        //    //if (qry != null)
        //    //{
        //    //    qry.BeforeQuery += new EventHandler(qry_BeforeQuery);
        //    //    qry.AfterQuery += new EventHandler(qry_AfterQuery);
        //    //}
        //}

        //void qry_AfterQuery(object sender, EventArgs e)
        //{
            
        //}

        //void qry_BeforeQuery(object sender, EventArgs e)
        //{
            
        //}
		#endregion

        #region private methods
        private void onSaveButtonClick(object sender, System.EventArgs e)
        {
            //for error log
            string sUpdate = "";
            string sInsert = "";
            string sDelete = "";
            try
            {
                EasyQuery qry = _query;
                if (qry != null)
                {
                    sDelete = qry.DeleteSql;
                    sInsert = qry.InsertSql;
                    sUpdate = qry.UpdateSql;
                    if (qry.Tables.Count > 0)
                    {
                        int nCurrentRowIndex = this.CurrentRowIndex;
                        acceptByMouse();
                        Form frm = this.FindForm();
                        if (frm != null)
                        {
                            if (!frm.IsDisposed)
                            {
                                frm.BindingContext[qry, DataTableName].EndCurrentEdit();
                                if (qry.Update())
                                {
                                    this.CurrentRowIndex = nCurrentRowIndex;
                                    qry.Query();
                                    this.Refresh();
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Data.DBConcurrencyException)
            {
                requeryAfterError();
            }
            catch (Exception er)
            {
                FormLog.NotifyException(er, "Update command:{0}.\r\nInsert command:{1}.\r\nDelete command {2}", sUpdate, sInsert, sDelete);
                requeryAfterError();
            }
        }
        private void requeryAfterError()
        {
            int n = nCurrentRowIndex;
            QueryDatabase();
            EasyQuery qry = _query;
            if (qry != null && qry.Tables.Count > 0)
            {
                if (n >= 0 && n < qry.Tables[0].Rows.Count)
                {
                    this.CurrentRowIndex = n;
                    nCurrentRowIndex = n;
                }
            }
        }
        /// <summary>
        /// save changes by moving mouse to the last row and click
        /// </summary>
        private void moveMouseOff()
        {
            //bool bOK = false;
            //EasyQuery qry = _query;
            //if (qry != null)
            //{
            //    if (qry.Tables.Count > 0)
            //    {
            //        bOK = true;
            //    }
            //}
            //if (bOK)
            //{
            //    try
            //    {
            //        //remember current position
            //        Point curPoint = Cursor.Position;
            //        //
            //        Rectangle rc = GetCurrentCellBounds();
            //        Point p2;
            //        Point p = new Point(rc.X, rc.Y + rc.Height + 2);
            //        p = PointToScreen(p);
            //        p2 = new Point(rc.X, rc.Y + rc.Height + 2);
            //        p2 = PointToScreen(p2);
            //        //move to the next row
            //        UIUtil.ClickMouse(p2.X, p2.Y);
            //        Application.DoEvents();
            //        Application.DoEvents();
            //        UIUtil.ClickMouse(p2.X, p2.Y);
            //        Application.DoEvents();
            //        Application.DoEvents();
            //        //move back
            //        UIUtil.ClickMouse(p.X, p.Y);
            //        Application.DoEvents();
            //        Application.DoEvents();
            //        UIUtil.ClickMouse(p.X, p.Y);
            //        Application.DoEvents();
            //        Application.DoEvents();
            //        //move to original position
            //        UIUtil.MoveMouse(curPoint.X, curPoint.Y);
            //    }
            //    catch
            //    {
            //    }
            //}
        }
        /// <summary>
        /// save changes by moving mouse to the last row and click
        /// </summary>
        private void acceptByMouse()
        {
            if (!this.ReadOnly && ActAsVisible())
            {
                moveMouseOff();

            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            btSave.Left = this.Width - btSave.Width;
        }
        private void EPDataGrid_Scroll(object sender, EventArgs e)
        {
            EasyQuery qry = _query;
            if (qry != null)
            {
                if (qry.Fields.Count > 0)
                {
                    for (int i = 0; i < qry.Fields.Count; i++)
                    {
                        if (cbx != null)
                            if (cbx[i] != null)
                                cbx[i].Visible = false;
                        if (bts != null)
                            if (bts[i] != null)
                                bts[i].Visible = false;
                    }
                }
            }

        }

        private void EPDataGrid_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                EasyQuery qry = _query;
                if (qry != null && qry.Adapter != null)
                {
                    qry.Update();
                }
            }
            catch (System.Data.DBConcurrencyException)
            {
                QueryDatabase();
            }
            catch (Exception er)
            {
                System.Data.DBConcurrencyException err = er as System.Data.DBConcurrencyException;
                if (err != null)
                {
                }
                FormLog.NotifyException(er);
            }
        }
        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
		{
			EasyDataTable obj = new EasyDataTable();
            for (int i = 0; i < DataBindings.Count; i++)
            {
                obj.DataBindings.Add(DataBindings[i]);
            }
            
            obj._autoSave = _autoSave;
            obj._loop = _loop;
            obj._AlternatingBackColor = _AlternatingBackColor;
            obj._BKcolor = _BKcolor;
            obj._Forecolor = _Forecolor;

            obj._HeaderColor = _HeaderColor;
            obj._HeaderBKcolor = _HeaderBKcolor;
            obj._allowNew = _allowNew;
            obj._allowDelete = _allowDelete;
            obj._HideSaveButton = _HideSaveButton;

            obj._left = _left;
            obj._top = _top;
            obj._width = _width;
            obj._height = _height;
            obj.DataSource = DataSource;
			return obj;
		}

		#endregion

		#region IPerformer Members
		//events
        [Description("Occurs when a value is being changed for the column.")]
        public event EventHandler BeforeColChange;
        [Description("Occurs after a value has been changed for the column.")]
        public event EventHandler AfterColChange;
        [Description("Occurs when the current row is changing.")]
        public event EventHandler BeforeRowChange;
        [Description("Occurs after the current row has been changed successfully.")]
        public event EventHandler AfterRowChange;
        [Description("Occurs before a row in the table is about to be deleted.")]
        public event EventHandler BeforeRowDelete;
        [Description("Occurs after a row in the table has been deleted.")]
        public event EventHandler AfterRowDelete;
        [Description("Occurs before a new row is added.")]
        public event EventHandler BeforeRowAdd;
        [Description("Occurs after a new row is added.")]
        public event EventHandler AfterRowAdd;
        [Description("Occurs when another row becomes the current row.")]
        public event EventHandler CursorMove;
        //public event EventHandler DropMe;
        [Description("Occurs when the user select an item from a dropdown list for editing a field.")]
        public event EventHandler Lookup;
        [Description("Occurs when the current record is at the last row and the MoveNext method is called. After this event occurs if the Loop property is True then the first row becomes the current record; if the Loop property is False then the last row is still the current record.")]
        public event EventHandler PassLast;
        [Description("Occurs when the current record is at the first row and the MoveBack method is called. After this event occurs if the Loop property is True then the last row becomes the current record; if the Loop property is False then the first row is still the current record.")]
        public event EventHandler PassFirst;
        [Description("Occurs when a cell got input focus.")]
        public event EventHandler CellFocus;
        [Description("Occurs when another column becomes the current column.")]
        public event EventHandler ColChange;
        //public event EventHandler DropKnownData;
        //    EventNames[7] = "DragEnter";  EventDescs[7] = "Occurs when an object is dragged into the performer's bounds.";
        //    EventNames[8] = "DragOver";   EventDescs[8] = "Occurs when an object is dragged over the performer's bounds.";
        //    EventNames[9] = "DragLeave";  EventDescs[9] = "Occurs when an object is dragged out of the performer's bounds.";
        //    EventNames[10] = "DragDrop";   EventDescs[10] = "Occurs when a drag-and-drop operation is completed and data is dropped to the performer.";
        //    EventNames[11] = "DropMe";     EventDescs[11] = "Occurs when a drag-and-drop operation is completed and this performer is dropped to somewhere.";
        
        //    objMethods[IDM_Execute] = new clsMethod("BatchExecute","Execute actions on all records. It goes from the first record to the last record, executes the actions on every record.",IDM_Execute,1);
        //    objMethods[IDM_Execute].setParamDesc(0,"Actions","The actions to be executed for every record.");
        //    objMethods[IDM_Execute].setParameterType(0,new clsPropActionsDesc());
        //    //
        //    objMethods[IDM_Cancel] = new clsMethod("Cancel","Cancel all the changes since the last time Update method is called.",IDM_Cancel,0);
        //    //
        //    objMethods[IDM_BuildQry] = new clsMethod("BuildQuery","Show _query Build to build a query and use it.",IDM_BuildQry,1);
        //    objMethods[IDM_BuildQry].setParamDesc(0,"Change database","Set it to true to select a database to build a new query. Set it to False to use the database used by the current query and modify the current query.");
        //    objMethods[IDM_BuildQry].setParameterType(0,new clsPropBoolDesc(0,false,false));
        //    //
        //    //      objMethods[IDM_SetParmNull] = new clsMethod("SetParameterToNull","Set the value of the SQL command parameter to null.",IDM_SetParmNull,1);
        //    //      objMethods[IDM_SetParmNull].setParamDesc(0,"Parameter","The parameter to set");
        //    //      obj2 = new PropFieldDesc();
        //    //      objMethods[IDM_SetParmNull].setParameterType(0,obj2);
        //    //
        //    objMethods[IDM_DelFilter] = new clsMethod("RemoveFilters","Remove all filters. All filters are defined in WHERE clause of a SQL SELECT statement.",IDM_DelFilter,0);
        //    //
        //    objMethods[IDM_ANDFilter] = new clsMethod("AddFilterAsAND","Add a filter to the WHERE clause of the SQL SELECT statement. The new filter is a logic AND with the existing filters.",IDM_ANDFilter,1);
        //    objMethods[IDM_ANDFilter].setParamDesc(0,"Filter","This is a string representing the filter. For example LastName='Mike'. For syntax of filter (WHERE clause of a SQL SELECT statement) refer to your database engine documents.");
        //    objMethods[IDM_ANDFilter].setParameterType(0,new clsPropertyDesc(0,false,""));
        //    //
        //    objMethods[IDM_ORFilter] = new clsMethod("AddFilterAsOR","Add a filter to the WHERE clause of the SQL SELECT statement. The new filter is a logic OR with the existing filters.",IDM_ORFilter,1);
        //    objMethods[IDM_ORFilter].setParamDesc(0,"Filter","This is a string representing the filter. For example LastName='Mike'. For syntax of filter (WHERE clause of a SQL SELECT statement) refer to your database engine documents.");
        //    objMethods[IDM_ORFilter].setParameterType(0,new clsPropertyDesc(0,false,""));
        //    //
        //    objMethods[IDM_MoveToRow] = new clsMethod("MoveToRow","Make the specified row as the current row.",IDM_MoveToRow,1);
        //    objMethods[IDM_MoveToRow].DefineParameter(0,"RowNumber","Row number. 0 indicates the first row; 1 indicates the second row, etc.",new clsPropertyDesc(0,false,0));
        //    objMethods[IDM_BringUp] = new clsMethod("BringToFront","Bring this control to the front of all other controls.",IDM_BringUp,0);
        //    //
        //    objMethods[IDM_SetFldAtt] = new clsMethod("SetFieldAttributes","Change field attributes at runtime.",IDM_SetFldAtt,6);
        //    objMethods[IDM_SetFldAtt].DefineParameter(0,"FieldName","The name of the field (case-insensitive)",new PropStringEnumDesc(0,false,""));
        //    objMethods[IDM_SetFldAtt].DefineParameter(1,"IsFile","Indicates whether this field is a file path",new clsPropBoolDesc(1,false,false));
        //    objMethods[IDM_SetFldAtt].DefineParameter(2,"ColumnWidth","Column width. Use -1 to not change the width", new clsPropertyDesc(2,false,-1));
        //    objMethods[IDM_SetFldAtt].DefineParameter(3,"Visible","Indicates whether this field is visible", new clsPropBoolDesc(3,false,true));
        //    objMethods[IDM_SetFldAtt].DefineParameter(4,"ReadOnly","Indicates whether this field is read-only", new clsPropBoolDesc(4,false,false));
        //    objMethods[IDM_SetFldAtt].DefineParameter(5,"Caption","Caption of the field. If it is empty then the caption is not changed.",new clsPropMultiLangStringDesc(5,false,""));
        //    //
        //    objMethods[IDM_RefreshLK] = new clsMethod("RefreshLookup","Refresh the lookup table for the specified field.",IDM_RefreshLK,1);
        //    objMethods[IDM_RefreshLK].DefineParameter(0,"FieldName","The name of the field, with which the lookup is associated. ",new PropStringEnumDesc(0,false,""));
        //    //
		/// <summary>
		/// copy parameter fields
		/// </summary>
        //void initializeFields()
        //{
        //    EasyQuery qry = _query;
        //    if( qry == null )
        //    {
        //        return;
        //    }
        //    int i;
        //    for(i=0;i<qry.Fields.Count;i++)
        //    {
        //        if( fldsCur != null )
        //        {
        //            //copy current settings
        //            EPField fld = fldsCur[qry.Fields[i].Name];
        //            if( fld != null )
        //            {
        //                qry.Fields[i].IsFile = fld.IsFile;
        //                qry.Fields[i].ColumnWidth = fld.ColumnWidth;
        //                qry.Fields[i].Visible = fld.Visible;
        //                qry.Fields[i].ReadOnly = fld.ReadOnly;
        //                if( fld.editor != null )
        //                {
        //                    qry.Fields[i].editor = fld.editor;
        //                }
        //            }
        //        }
        //    }
        //    fldsCur = qry.Fields;
        //    _isNulls = (FieldList)qry.Fields.Clone();
        //    if( paramCur != null && qry.Parameters != null )
        //    {
        //        for(i=0;i<qry.Parameters.Count;i++)
        //        {
        //            //copy parameter values
        //            EPField fld = paramCur[qry.Parameters[i].Name];
        //            if( fld != null )
        //            {
        //                qry.Parameters[i].Value = fld.Value;
        //            }
        //        }
        //    }
        //    paramCur = qry.Parameters;
        //    qry.CreateBlobFields();
        //    _sumFields = (FieldList)qry.Fields.Clone();
        //}
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent != null)
            {
                Form f = this.FindForm();
                if (f != null)
                {
                    if (_onFormClosing == null)
                    {
                        _onFormClosing = new FormClosingEventHandler(f_FormClosing);
                    }
                    else
                    {
                        f.FormClosing -= _onFormClosing;
                    }
                    f.FormClosing += _onFormClosing;
                }
            }
        }

        void f_FormClosing(object sender, FormClosingEventArgs e)
        {
			pageLoaded = false;
			bRowChanging = true;
			try
			{
                EasyQuery qry = _query;
				if( qry != null )
				{
                    if (_autoSave)
                    {
                        try
                        {
                            if (Site == null || !Site.DesignMode)
                            {
                                qry.Update();
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        //qry.ClearQuery();
                    }
                    //CloseConnections();
				}
			}
			catch
			{
			}
		}

//        public clsMethod getMethod(int Index)
//        {
//            if( Index >= 0 && Index < objMethods.Length )
//            {
//                if( Index == IDM_SetParm /*|| Index == IDM_SetParmNull*/ )
//                {
//                    PropFieldDesc obj2 = new PropFieldDesc();
//                    obj2.SetOwner(this);
//                    obj2.AllowInPlaceEditing = true;
//                    obj2.ShowFieldsDialog = new fnShowSelectionDialog(showSelParamDialog);
//                    objMethods[Index].setParameterType(0,obj2);
//                }
//                else if( Index == IDM_SetField )
//                {
//                    PropFieldDesc obj2 = new PropFieldDesc();
//                    obj2.SetOwner(this);
//                    objMethods[IDM_SetField].setParameterType(0,obj2);
//                }
//                else if( Index == IDM_RefreshLK)
//                {
////					string s = clsProperty.ToString(
//                    string[] list = null;
//                    if(fields != null)
//                    {
//                        int n = 0;
//                        string[] list0 = new string[fields.Count];
//                        for(int i=0;i<fields.Count;i++)
//                        {
//                            DataEditorLookupDB lk = fields[i].editor as DataEditorLookupDB;
//                            if( lk != null)
//                            {
//                                list0[i] = fields[i].Name;
//                                n++;
//                            }
//                        }
//                        if(n > 0)
//                        {
//                            list = new string[n];
//                            for(int i=0,k=0;i<list0.Length;i++)
//                            {
//                                if(list0[i] != null)
//                                {
//                                    list[k++] = list0[i];
//                                }
//                            }
//                        }
//                    }
//                    PropStringEnumDesc obj2 = new PropStringEnumDesc();
//                    obj2.SetOwner(this);
//                    obj2.StringEnum = list;
//                    objMethods[Index].setParameterType(0,obj2);
//                }
//                else if(Index == IDM_SetFldAtt)
//                {
//                    string[] list = null;
//                    if(fields != null)
//                    {
//                        list = new string[fields.Count];
//                        for(int i=0;i<fields.Count;i++)
//                        {
//                            list[i] = fields[i].Name;
//                        }
//                    }
//                    PropStringEnumDesc obj2 = new PropStringEnumDesc();
//                    obj2.SetOwner(this);
//                    obj2.StringEnum = list;
//                    objMethods[Index].setParameterType(0,obj2);
//                }
//                objMethods[Index].SetDescription(clsAppGlobals.objUIText.GetText((IUIText)objUIMethod,(ushort)Index),clsAppGlobals.objUIText.GetText((IUIText)objUIMethodDesc,(ushort)Index));
//                return objMethods[Index];
//            }
//            return null;
//        }
        //[Description("Connect to the database and query for the data")]
        //public void FetchDataFromDatabase()
        //{
        //    bNewCall = true;
        //    QueryDatabase();
        //}
        [Description("Make the next row the current row.")]
        public void MoveNext()
        {
            if (_query != null)
            {
                if (_query.Tables.Count > 0)
                {
                    if (_query.Tables[0].Rows.Count > 0)
                    {
                        nCurrentRowIndex = this.CurrentRowIndex;
                        if (nCurrentRowIndex < _query.Tables[0].Rows.Count - 1)
                        {
                            int n = nCurrentRowIndex + 1;
                            if (n < 0)
                                n = 0;
                            gotoRow(n);
                        }
                        else
                        {
                            if (PassLast != null)
                            {
                                PassLast(this, EventArgs.Empty);
                                if (_loop)
                                {
                                    gotoRow(0);
                                }
                            }
                        }
                    }
                }
            }
        }
        [Description("Make the previous row the current row.")]
        public void MoveBack()
        {
            if (_query != null)
            {
                if (_query.Tables.Count > 0)
                {
                    if (_query.Tables[0].Rows.Count > 0)
                    {
                        nCurrentRowIndex = this.CurrentRowIndex;
                        if (nCurrentRowIndex > 0)
                        {
                            int n = nCurrentRowIndex - 1;
                            if (n < 0)
                                n = 0;
                            gotoRow(n);
                        }
                        else
                        {
                            if (PassFirst != null)
                            {
                                PassFirst(this, EventArgs.Empty);
                                if (_loop && _query.Tables[0].Rows.Count > 1)
                                {
                                    gotoRow(_query.Tables[0].Rows.Count - 1);
                                }
                            }
                        }
                    }
                }
            }
        }
        [Description("Make the last row the current row.")]
        public void MoveLast()
        {
            if (_query != null && _query.Tables.Count > 0)
            {
                if (_query.Tables[0].Rows.Count > 0 && nCurrentRowIndex != _query.Tables[0].Rows.Count - 1)
                {
                    gotoRow(_query.Tables[0].Rows.Count - 1);
                }
            }
        }
        [Description("Make the first row the current row.")]
        public void MoveFirst()
        {
            if (_query != null && _query.Tables.Count > 0)
            {
                if (_query.Tables[0].Rows.Count > 0 && nCurrentRowIndex != 0)
                {
                    gotoRow(0);
                }
            }
        }
        [Description("Delete the current row.")]
        public void DeleteCurrentRow()
        {
            if (!ReadOnly)
            {
                if (_query != null && _query.Adapter != null && _query.Tables.Count > 0 && _query.Tables[0].Rows.Count > 0)
                {
                    if (nCurrentRowIndex >= 0 && nCurrentRowIndex < _query.Tables[0].Rows.Count)
                    {
                        int nIdx = nCurrentRowIndex;
                        System.Data.DataRow dw = CurrentRow;
                        if (dw != null)
                        {
                            dw.Delete();
                            _query.Update();
                            QueryDatabase();
                            if (nIdx < _query.Tables[0].Rows.Count)
                            {
                                this.CurrentRowIndex = nIdx;
                            }
                            else
                            {
                                if (_query.Tables[0].Rows.Count > 0)
                                {
                                    this.CurrentRowIndex = _query.Tables[0].Rows.Count - 1;
                                }
                            }
                        }
                    }
                }
            }
        }
        [Description("Add a new record.")]
        public void AddNewRecord()
        {
            if (!ReadOnly)
            {
                if (_query != null && _query.Tables.Count > 0)
                {
                    System.Data.DataRow rw = _query.Tables[0].NewRow();
                    for (int i = 0; i < _query.Tables[0].Columns.Count; i++)
                    {
                        if (_query.Tables[0].Columns[i].DataType == typeof(System.DateTime))
                        {
                            rw[i] = System.DateTime.Now.ToString("d");
                        }
                    }
                    _query.Tables[0].Rows.Add(rw);
                    if (_query.Tables[0].Rows.Count > 0)
                    {
                        this.CurrentRowIndex = _query.Tables[0].Rows.Count - 1;
                        if (NewRecordAdded != null)
                        {
                            NewRecordAdded(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }
        [Description("Save the changes back to the database.")]
        public void Submit()
        {
            if (!ReadOnly)
            {
                onSaveButtonClick(this, EventArgs.Empty);
            }
        }
        //[Description("Set a new query for the data table.")]
        //public override void SetQuery(EasyQuery query)
        //{
        //    base.SetQuery(query);
        //    QueryDatabase();
        //}
        [Description("Locate record by a field value and set the located record to be the current record.")]
        public bool Search(string fieldName, object value, enumLogicType compare, bool ignoreCase)
        {
            bool bFound = false;
            if (value != null && _query != null && _query.Tables.Count > 0)
            {
                int nColIdx = -1;
                for (int i = 0; i < _query.Tables[0].Columns.Count; i++)
                {
                    if (string.Compare(_query.Tables[0].Columns[i].ColumnName, fieldName, true) == 0)
                    {
                        nColIdx = i;
                        break;
                    }
                }
                if (nColIdx >= 0)
                {
                    double dv = 0;
                    DateTime dt0 = DateTime.Now;
                    string s = value.ToString();
                    string su = s.ToUpper();
                    bool bIsNum = EPField.IsNumber(EPField.ToOleDBType(_query.Tables[0].Columns[nColIdx].DataType));
                    bool bIsDateTime = _query.Tables[0].Columns[nColIdx].DataType.Equals(typeof(System.DateTime));
                    if (bIsNum)
                    {
                        try
                        {
                            dv = ValueConvertor.ToDouble(value);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    if (bIsDateTime)
                    {
                        try
                        {
                            dt0 = ValueConvertor.ToDateTime(value);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    for (int n = 0; n < _query.Tables[0].Rows.Count; n++)
                    {
                        object val = _query.Tables[0].Rows[n][fieldName];
                        if (val != null && val != DBNull.Value)
                        {
                            switch (compare)
                            {
                                case enumLogicType.BeginWith:
                                    if (val.ToString().StartsWith(s, ignoreCase, System.Globalization.CultureInfo.InvariantCulture))
                                    {
                                        gotoRow(n);
                                        return true;
                                    }
                                    break;
                                case enumLogicType.Contains:
                                    if (ignoreCase)
                                    {
                                        if (val.ToString().IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (val.ToString().IndexOf(s, StringComparison.InvariantCulture) >= 0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    break;
                                case enumLogicType.EndWith:
                                    if (val.ToString().EndsWith(s, ignoreCase, System.Globalization.CultureInfo.InvariantCulture))
                                    {
                                        gotoRow(n);
                                        return true;
                                    }
                                    break;
                                case enumLogicType.Equal:
                                    if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt0 == dt1)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsNum)
                                    {
                                        if (dv == ValueConvertor.ToDouble(val))
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (string.Compare(val.ToString(), s, ignoreCase) == 0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    break;
                                case enumLogicType.Included:
                                    if (ignoreCase)
                                    {
                                        if (s.IndexOf(val.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (s.IndexOf(val.ToString(), StringComparison.InvariantCulture) >= 0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    break;
                                case enumLogicType.Larger:
                                    if( bIsNum )
                                    {
                                        if (ValueConvertor.ToDouble(val) > dv)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt1 > dt0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (ignoreCase)
                                        {
                                            if (val.ToString().ToUpper().CompareTo(su) > 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            if (val.ToString().CompareTo(s) > 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                    }
                                    break;
                                case enumLogicType.LargerEqual:
                                    if (bIsNum)
                                    {
                                        if (ValueConvertor.ToDouble(val) >= dv)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt1 >= dt0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (ignoreCase)
                                        {
                                            if (val.ToString().ToUpper().CompareTo(su) >= 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            if (val.ToString().CompareTo(s) >= 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                    }
                                    break;
                                case enumLogicType.NotEqual:
                                    if (bIsNum)
                                    {
                                        if (ValueConvertor.ToDouble(val) != dv)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt1 != dt0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (ignoreCase)
                                        {
                                            if (val.ToString().ToUpper().CompareTo(su) != 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            if (val.ToString().CompareTo(s) != 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                    }
                                    break;
                                case enumLogicType.Smaller:
                                    if (bIsNum)
                                    {
                                        if (ValueConvertor.ToDouble(val) < dv)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt1 < dt0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (ignoreCase)
                                        {
                                            if (val.ToString().ToUpper().CompareTo(su) < 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            if (val.ToString().CompareTo(s) < 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                    }
                                    break;
                                case enumLogicType.SmallerEqual:
                                    if (bIsNum)
                                    {
                                        if (ValueConvertor.ToDouble(val) <= dv)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else if (bIsDateTime)
                                    {
                                        System.DateTime dt1 = ValueConvertor.ToDateTime(val);
                                        if (dt1 <= dt0)
                                        {
                                            gotoRow(n);
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        if (ignoreCase)
                                        {
                                            if (val.ToString().ToUpper().CompareTo(su) <= 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            if (val.ToString().CompareTo(s) <= 0)
                                            {
                                                gotoRow(n);
                                                return true;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return bFound;
        }
		public bool showSelParamDialog(System.Windows.Forms.Form frmOwner,ref object retValue)
		{
            EasyQuery qry = _query;// propDescs[ID_Query].objProperty.getCoreValue() as EPQuery;
			if( qry != null )
			{
				if( qry.Parameters != null )
				{
					if( qry.Parameters.Count > 0 )
					{
						dlgSelParam dlg = new dlgSelParam();
						dlg.LoadData(qry.Parameters);
						EPField fld = retValue as EPField;
						if( fld != null )
							dlg.SetSelection(fld);
						if( dlg.ShowDialog(frmOwner) == System.Windows.Forms.DialogResult.OK )
						{
							retValue = dlg.objRet;
							return true;
						}
					}
				}
			}
			return false;
		}
		protected void OnMouseDown2(MouseEventArgs e)
		{
            try
            {
                if (_query != null && (Site == null || !Site.DesignMode))
                {
                    if (_query.Tables.Count > 0)
                    {
                        if (_query.Tables[0] != null)
                        {
                            if (this.TableStyles.Count > 0)
                            {
                                DataGridTableStyle myGridTableStyle = this.TableStyles[0];
                                if (_query.Fields.Count == myGridTableStyle.GridColumnStyles.Count)
                                {
                                    for (int i = 0; i < myGridTableStyle.GridColumnStyles.Count; i++)
                                    {
                                        if (myGridTableStyle.GridColumnStyles[i].Width != _query.Fields[i].ColumnWidth)
                                        {
                                            _query.Fields[i].ColumnWidth = myGridTableStyle.GridColumnStyles[i].Width;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
		}
        //protected bool Requery()
        //{
        //    if (bNewCall)
        //    {
        //        QueryDatabase();
        //        this.Invalidate();
        //        return true;
        //    }
        //    nNullRow = -1;
        //    if( requery() )
        //    {
        //        this.Invalidate();
        //        return true;
        //    }
        //    return false;
        //}
        //private bool requery()
        //{
        //    bLoadingData = true;
        //    int nCurrentRowIndex = this.CurrentRowIndex;

        //    //this.DataBindings.Clear();
        //    EasyQuery qry = _query;
        //    if (qry != null)
        //    {
        //        qry.FetchData();

        //        if (qry.Tables.Count > 0)
        //        {
        //            bNewTable = (qry.Tables[0].Rows.Count == 0);

        //            //bindData();
        //            if (nCurrentRowIndex >= 0 && nCurrentRowIndex < qry.Tables[0].Rows.Count)
        //            {
        //                this.CurrentRowIndex = nCurrentRowIndex;
        //            }
        //            onRowIndexChanged();
        //            if (RequeryFinished != null)
        //            {
        //                RequeryFinished(this, EventArgs.Empty);
        //            }
        //            OnCurrentCellChanged(null);
        //        }
        //    }
        //    bLoadingData = false;
        //    this.Invalidate();
        //    return (qry != null && qry.Tables.Count > 0);
        //}
        private void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            //			if(e.ListChangedType == System.ComponentModel.ListChangedType.Reset)
            //			{
            onRowIndexChanged();
            //			}
        }
        //private void bindData()
        //{
        //    bLoadingData = true;
        //    EasyQuery qry = _query;
        //    if (qry != null && qry.Tables.Count > 0)
        //    {
        //        base.DataSource = qry.Tables[0].DefaultView;
        //        qry.Tables[0].RowChanged += new DataRowChangeEventHandler(EPTable_RowChanged);
        //        qry.Tables[0].RowChanging += new DataRowChangeEventHandler(EPTable_RowChanging);
        //        qry.Tables[0].ColumnChanging += new DataColumnChangeEventHandler(EPTable_ColumnChanging);
        //        qry.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(EPTable_ColumnChanged);
        //        qry.Tables[0].RowDeleted += new DataRowChangeEventHandler(EPDataGrid_RowDeleted);
        //        qry.Tables[0].DefaultView.ListChanged += new ListChangedEventHandler(DefaultView_ListChanged);

        //        qry.Tables[0].DefaultView.AllowNew = _allowNew;
        //        qry.Tables[0].DefaultView.AllowDelete = AllowDelete;
        //    }
        //    bLoadingData = false;
        //}
        //protected void actOnAll(clsPerformerActions acts,object sender,EPSTDEventArgs e)
        //{
        //    if( clsAppGlobals.OnLoadEventHandler != null )
        //    {
        //        clsTask objTask = clsAppGlobals.OnLoadEventHandler(!acts.bIsTask,acts.ActionID);
        //        if( objTask != null )
        //        {
        //            int nCount = clsProperty.ToInt(((IProperties)this)[ID_RowCount].objProperty.getCoreValue());
        //            for(int i=0;i<nCount;i++)
        //            {
        //                this.CurrentRowIndex = i;
        //                onRowIndexChanged();
        //                System.Threading.Thread.Sleep(0);
        //                clsTask.OnExecuteTask(objTask,sender,e,e.EventID,null );
        //            }
        //        }
        //    }
        //}
        //public int getEventCount()
        //{
        //    return eventCount;
        //}

        //public IEvent getEventByIndex(int Index)
        //{
        //    if( Index >= 0 && Index < EPevents.Length )
        //    {
        //        return EPevents[Index];
        //    }
        //    return null;
        //}

        //public IEvent getEventByID(int eventID)
        //{
        //    int Index = EventIDtoIndex(eventID);
        //    if( Index >= 0 && Index < EPevents.Length )
        //        return EPevents[Index];
        //    return null;
        //}

        //public string getEventNameByID(int eventID)
        //{
        //    int Index = EventIDtoIndex(eventID);
        //    if( Index >= 0 && Index < EventNames.Length )
        //        return clsAppGlobals.objUIText.GetText((IUIText)objUIEvent,(ushort)Index);
        //    return null;
        //}

        //public string getEventDescByID(int eventID)
        //{
        //    int Index = EventIDtoIndex(eventID);
        //    if( Index >= 0 && Index < EventDescs.Length )
        //        return clsAppGlobals.objUIText.GetText((IUIText)objUIEventDesc,(ushort)Index);
        //    return null;
        //}

		#endregion

		#region Properties
        [Description("An EasyQuery object for providing data")]
        public new EasyQuery DataSource
        {
            get
            {
                if (_query == null)
                {
                    _query = new EasyQuery();
                    hookQuery();
                }
                return _query;
            }
            set
            {
                _query = value;
                hookQuery();
                QueryDatabase();
            }
        }
        void hookQuery()
        {
            if (_query != null)
            {
                if (_onBeforeQuery == null)
                {
                    _onBeforeQuery = new EventHandler(_query_BeforeQuery);
                }
                else
                {
                    _query.BeforeQuery -= _onBeforeQuery;
                }
                if (_onAfterQuery == null)
                {
                    _onAfterQuery = new EventHandler(_query_AfterQuery);
                }
                else
                {
                    _query.AfterQuery -= _onAfterQuery;
                }
                if (_onBeforePrepareCommands == null)
                {
                    _onBeforePrepareCommands = new EventHandler(_query_BeforePrepareCommands);
                }
                else
                {
                    _query.BeforePrepareCommands -= _onBeforePrepareCommands;
                }
                _query.BeforeQuery += _onBeforeQuery;
                _query.AfterQuery += _onAfterQuery;
                _query.BeforePrepareCommands += _onBeforePrepareCommands;
            }
        }
        void _query_BeforeQuery(object sender, EventArgs e)
        {
            ApplyStyle();
        }
        void _query_BeforePrepareCommands(object sender, EventArgs e)
        {
            EasyQuery qry = sender as EasyQuery;
            if (qry != null)
            {
                qry.ForReadOnly = ReadOnly;
            }
        }
        void _query_AfterQuery(object sender, EventArgs e)
        {
            EasyQuery qry = sender as EasyQuery;
            if (qry != null && qry.Tables.Count >0)
            {
                _query = qry;
                if (qry.ReadOnly)
                {
                    ReadOnly = true;
                }
                base.DataSource = qry.Tables[0].DefaultView;
                qry.Tables[0].RowChanged += new DataRowChangeEventHandler(EPTable_RowChanged);
                qry.Tables[0].RowChanging += new DataRowChangeEventHandler(EPTable_RowChanging);
                qry.Tables[0].ColumnChanging += new DataColumnChangeEventHandler(EPTable_ColumnChanging);
                qry.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(EPTable_ColumnChanged);
                qry.Tables[0].RowDeleted += new DataRowChangeEventHandler(EPDataGrid_RowDeleted);
                qry.Tables[0].DefaultView.ListChanged += new ListChangedEventHandler(DefaultView_ListChanged);

                qry.Tables[0].DefaultView.AllowNew = _allowNew;
                qry.Tables[0].DefaultView.AllowDelete = AllowDelete;
            }
        }
        public string DataTableName
        {
            get
            {
                EasyQuery qry = _query;
                if (qry != null && qry.Tables.Count > 0)
                {
                    return qry.Tables[0].TableName;
                }
                return string.Empty;
            }
        }
        [Description("Total number of records in this data table.")]
        public int RowCount
        {
            get
            {
                if (_query != null)
                {
                    if (_query.Tables.Count > 0)
                    {
                        return _query.Tables[0].Rows.Count;
                    }
                }
                return 0;
            }
        }
//        [Description("Gets or sets the background color of even-numbered rows of the grid.")]
//        public Color BackColorEven
//        {
//            get
//            {
//                return base.BackColor;
//            }
//            set
//            {
//                base.BackColor = value;
//            }
//        }
////            //			PROPNames[ID_BKColorOdd] = "BackColorOdd"; PROPDescs[ID_BKColorOdd] = "Gets or sets the background color of odd-numbered rows of the grid.";
//        public Color BackColorOdd
//        {
//            get
//            {
//                return AlternatingBackColor;
//            }
//            set
//            {
//                AlternatingBackColor = value;
//            }
//        }
//            PROPNames[ID_AllowDrop] = "AllowDrop"; PROPDescs[ID_AllowDrop] = "Determines if the performer will receive drag-drop notifications and generate related events";
//            PROPNames[ID_DropTypes] = "DropTypes"; PROPDescs[ID_DropTypes] = "It includes the data types this performer will accept when the data is dropped to it. A Drop event will be fired when an accepted data is dropped.";
//            PROPNames[ID_DropData] = "DropData"; PROPDescs[ID_DropData] = "This is the data dragged over or dropped to this performer. You may only use this property in drag/drop events.";
//            PROPNames[ID_AllowDrag] = "AllowDrag"; PROPDescs[ID_AllowDrag] = "Determines if the performer can be dragged. If it is true, hold down left mouse button and move the mouse will start dragging this performer.";
//            PROPNames[ID_DragCursor] = "DragCursor"; PROPDescs[ID_DragCursor] = "This is the cursor you want to use when this performer is dragged over an object and that object will accept the drop of this performer.";
//            PROPNames[ID_NoDropCurs] = "NoDropCursor"; PROPDescs[ID_NoDropCurs] = "This is the cursor you want to use when this performer is dragged over an object and that object will accept the drop of this performer.";
        [Description("If this property is True, when the page closes, it will try to save changed data to database. If this property is False, when the page closes all unsaved data will be discarded.")]
        public bool AutoSave
        {
            get
            {
                return _autoSave;
            }
            set
            {
                _autoSave = value;
            }
        }
//            PROPNames[ID_SQL] = "SQL Statement"; PROPDescs[ID_SQL] = "This is the database SQL statement for retrieving data. This property is read-only. To assign a new SQL statement to this performer, set your new SQL statement to _query property.";
//            PROPNames[ID_IsNull]      = "FieldIsNull";      PROPDescs[ID_IsNull]      = "This property indicates whether the value of each field in the current row is null.";
//            PROPNames[ID_RowIndex]      = "CurrentRowNumber";      PROPDescs[ID_RowIndex]      = "This property indicates the row number for the current row. 0 indicates the first row; 1 indicates the second row, etc.";
//            PROPNames[ID_SearchFound] = "SearchFound";      PROPDescs[ID_SearchFound] = "This property indicates whether the Search method is successful.";
//            PROPNames[ID_ZOrder]  = "Z-Order"; PROPDescs[ID_ZOrder] = "Z-position of this control. The control with larger z-order number is on the front of other controls with smaller z-order numbers.";
        [Description("Gets or sets the background color of odd-numbered rows of the grid.")]
        public Color BackColorOddRow
        {
            get
            {
                return _AlternatingBackColor;
            }
            set
            {
                _AlternatingBackColor = value;
            }
        }
        [Description("Gets or sets the background color of even-numbered rows of the grid.")]
        public Color BackColorEvenRow
        {
            get
            {
                return _BKcolor;
            }
            set
            {
                _BKcolor = value;
            }
        }
        [Description("Text color")]
        public Color TextColor
        {
            get
            {
                return _Forecolor;
            }
            set
            {
                _Forecolor = value;
            }
        }
//            PROPNames[ID_BKGcolor] = "BackgroundColor"; PROPDescs[ID_BKGcolor] = "Gets or sets the color of the non-row area of the grid.";
//            PROPNames[ID_BorderStyle] = "BorderStyle"; PROPDescs[ID_BorderStyle] = "Gets or sets the border style of the grid control.";
//            //
//            PROPNames[ID_CapColor] = "CaptionColor"; PROPDescs[ID_CapColor] = "Gets or sets the foreground color of the caption area.";
//            PROPNames[ID_CapBKcolor] = "CaptionBackColor"; PROPDescs[ID_CapBKcolor] = "Gets or sets the background color of the caption area.";
//            PROPNames[ID_CapFont] = "CaptionFont"; PROPDescs[ID_CapFont] = "Gets or sets the font of the grid's caption.";
//            PROPNames[ID_ColHeadVisible] = "ColumnHeadersVisible"; PROPDescs[ID_ColHeadVisible] = "Gets or sets a value indicating whether the column headers a table are visible.";
//            PROPNames[ID_FlatMode] = "FlatMode"; PROPDescs[ID_FlatMode] = "Gets or sets a value indicating whether the grid displays in flat mode.";
//            PROPNames[ID_Font] = "Font"; PROPDescs[ID_Font] = "Gets or sets the font of the text displayed by the control.";
//            PROPNames[ID_Forecolor] = "ForeColor"; PROPDescs[ID_Forecolor] = "Text color";
        //[Description("Gets or sets the color of the grid lines.")]
        //public Color GridLineColor
        //{
        //    get
        //    {
        //        return _GridLineColor;
        //    }
        //    set
        //    {
        //        _GridLineColor = value;
        //    }
        //}
        //[Description("Gets or sets the line style of the grid.")]
        //public DataGridLineStyle GridLineStyle
        //{
        //    get
        //    {
        //        return _gridLineStyle;
        //    }
        //    set
        //    {
        //        _gridLineStyle = value;
        //    }
        //}
        [Description("Gets or sets the foreground color of headers.")]
        public Color HeaderColor
        {
            get
            {
                return _HeaderColor;
            }
            set
            {
                _HeaderColor = value;
            }
        }
        [Description("Gets or sets the background color of all row and column headers.")]
        public Color HeaderBKcolor
        {
            get
            {
                return _HeaderBKcolor;
            }
            set
            {
                _HeaderBKcolor = value;
            }
        }
        //[Description("Gets or sets the font used for column headers.")]
        //public Font HeaderFont
        //{
        //    get
        //    {
        //        return _HeaderFont;
        //    }
        //    set
        //    {
        //        _HeaderFont = value;
        //    }
        //}
        //[Description("Gets or sets a value that specifies whether row headers are visible.")]
        //public bool RowHeadersVisible
        //{
        //    get
        //    {
        //        return _RowHeadersVisible;
        //    }
        //    set
        //    {
        //        _RowHeadersVisible = value;
        //    }
        //}
        //[Description("Gets or sets the width of row headers.")]
        //public int RowHeaderWidth
        //{
        //    get
        //    {
        //        return _RowHeaderWidth;
        //    }
        //    set
        //    {
        //        _RowHeaderWidth = value;
        //    }
        //}
        //[Description("Gets or sets the background color of selected rows.")]
        //public Color SelectionBackColor
        //{
        //    get
        //    {
        //        return _SelectionBackColor;
        //    }
        //    set
        //    {
        //        _SelectionBackColor = value;
        //    }
        //}
        //[Description("Gets or set the foreground color of selected rows.")]
        //public Color SelectionForeColor
        //{
        //    get
        //    {
        //        return _SelectionForeColor;
        //    }
        //    set
        //    {
        //        _SelectionForeColor = value;
        //    }
        //}
        [Description("When the current record is at the last row and the MoveNext method is called the PassLast event occurs. After the PassLast event if the Loop property is True then the first row becomes the current record; if the Loop property is False then the last row is still the current record. When the current record is at the first row and the MoveBack method is called the PassFirst event occurs. After the PassFirst event if the Loop property is True then the last row becomes the current record; if the Loop property is False then the first row is still the current record.")]
        public bool Loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
            }
        }
        [Description("Gets or sets a value indicating whether the new rows can be added if the _query is not read-only.")]
        public bool AllowNew
        {
            get
            {
                return _allowNew;
            }
            set
            {
                if (value != _allowNew)
                {
                    _allowNew = value;
                    QueryDatabase();
                }
            }
        }
//            PROPNames[ID_AllowDelete] = "AllowDelete"; PROPDescs[ID_AllowDelete] = "Sets or gets a value indicating whether deletes are allowed if the _query is not read-only.";
        //[Description("Gets or sets a value indicating whether the grid can be resorted by clicking on a column header.")]
        //public bool AllowSort
        //{
        //    get
        //    {this.AllowSorting 
        //        return _AllowSort;
        //    }
        //    set
        //    {
        //        _AllowSort = value;
        //    }
        //}
//            PROPNames[ID_ChangeColumn] = "ChangeColumn"; PROPDescs[ID_ChangeColumn] = "Indicates the column name when BeforeColumnChange and AfterColumnChange events occur.";
//            PROPNames[ID_CurColumnNum] = "CurrentColumnNumber"; PROPDescs[ID_CurColumnNum] = "Indicates the column index of the currently selected cell.";
        
        [Description("Indicates whether to hide the save button on the upper-right corner.")]
        public bool HideSaveButton
        {
            get
            {
                return _HideSaveButton;
            }
            set
            {
                _HideSaveButton = value;
                btSave.Visible = !_HideSaveButton;
            }
        }
        string _nullText = Resource1._null;
        [Description("The text to be displayed in cells when cell data is null.")]
        public string NullCellText
        {
            get
            {
                return _nullText;
            }
            set
            {
                _nullText = value;
                ApplyStyle();
            }
        }
		protected string NullText
		{
			get
			{
                return NullCellText;
			}
		}
		public void ApplyStyle()
		{
            try
            {
                EasyQuery qry = _query;
                if (qry != null && qry.Fields.Count > 0)
                {
                    int i;
                    int n = qry.Fields.Count;
                    if (cbx != null)
                    {
                        for (i = 0; i < cbx.Length; i++)
                        {
                            try
                            {
                                this.Controls.Remove(cbx[i]);
                            }
                            catch
                            {
                            }
                            cbx[i] = null;
                        }
                    }
                    if (bts != null)
                    {
                        for (i = 0; i < bts.Length; i++)
                        {
                            try
                            {
                                this.Controls.Remove(bts[i]);
                            }
                            catch
                            {
                            }
                            bts[i] = null;
                        }
                    }
                    cbx = new System.Windows.Forms.ComboBox[n];
                    bts = new System.Windows.Forms.Button[n];
                    DataGridColumnStyle TextCol;
                    myGridTableStyle = new DataGridTableStyle();
                    myGridTableStyle.MappingName = DataTableName;
                    //			myGridTableStyle.AlternatingBackColor = System.Drawing.Color.LightPink;
                    //			int j=0;

                    string cap;
                    for (i = 0; i < qry.Fields.Count; i++)
                    {
                        if (ReadOnly || !EPField.IsBinary(qry.Fields[i].OleDbType))
                        {
                            if (qry.Fields[i].OleDbType == System.Data.OleDb.OleDbType.Boolean)
                            {
                                TextCol = new DataGridBoolColumn();
                            }
                            else
                            {
                                TextCol = new TextColumnStyle(qry.Fields[i].HeaderAlignment);// new DataGridTextBoxColumn();
                                //						((DataGridTextBoxColumn)TextCol).TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                                ((DataGridTextBoxColumn)TextCol).TextBox.DoubleClick += new EventHandler(TextBox_DoubleClick);
                                ((DataGridTextBoxColumn)TextCol).TextBox.Click += new EventHandler(TextBox_Click);
                                ((DataGridTextBoxColumn)TextCol).TextBox.GotFocus += new EventHandler(TextBox_GotFocus);
                                ((TextColumnStyle)TextCol).Format = qry.Fields[i].Format;
                                if (NullText != null)
                                {
                                    ((TextColumnStyle)TextCol).NullText = NullText;
                                }
                            }

                            TextCol.MappingName = qry.Fields[i].Name;
                            TextCol.Alignment = qry.Fields[i].TxtAlignment;
                            cap = qry.Fields[i].FieldCaption;
                            if (string.IsNullOrEmpty(cap))
                                cap = qry.Fields[i].Name;
                            TextCol.HeaderText = cap;
                            if (ReadOnly)
                                TextCol.ReadOnly = true;
                            else
                            {
                                if (qry.Fields[i].IsIdentity || qry.Fields[i].ReadOnly
                                    || EPField.IsBinary(qry.Fields[i].OleDbType)
                                    || qry.Fields[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp)
                                    TextCol.ReadOnly = true;
                            }
                            if (qry.Fields[i].Visible)
                            {
                                TextCol.Width = qry.Fields[i].ColumnWidth;
                            }
                            else
                                TextCol.Width = 0;

                            myGridTableStyle.GridColumnStyles.Add(TextCol);
                            if (qry.Fields[i].editor != null)
                            {
                                DataEditorButton bn = qry.Fields[i].editor as DataEditorButton;
                                if (bn != null)
                                {
                                    bts[i] = bn.MakeButton(this.FindForm());
                                    if (bts[i] != null)
                                    {
                                        bts[i].Parent = this;
                                        bts[i].Visible = false;
                                        qry.Fields[i].editor.OnPickValue += new fnOnPickValue(onPickValueByButton);
                                    }
                                }
                                else
                                {
                                    DataEditorLookupYesNo LK = qry.Fields[i].editor as DataEditorLookupYesNo;
                                    if (LK != null)
                                    {
                                        cbx[i] = LK.MakeComboBox();
                                        if (cbx[i] != null)
                                        {
                                            cbx[i].Parent = this;
                                            cbx[i].Visible = false;
                                            cbx[i].SelectedIndexChanged += new System.EventHandler(onLookupSelected);
                                        }
                                    }
                                }
                            }
                            //					j++;
                        }
                    }
                    this.TableStyles.Clear();
                    this.TableStyles.Add(myGridTableStyle);
                }
                if (myGridTableStyle != null)
                {
                    if (!_AlternatingBackColor.IsEmpty)
                    {
                        myGridTableStyle.AlternatingBackColor = _AlternatingBackColor;
                    }
                    if (!_BKcolor.IsEmpty)
                    {
                        myGridTableStyle.BackColor = _BKcolor;
                    }
                    if (!_Forecolor.IsEmpty)
                    {
                        myGridTableStyle.ForeColor = _Forecolor;
                    }
                    if (!GridLineColor.IsEmpty)
                    {
                        myGridTableStyle.GridLineColor = this.GridLineColor;
                    }
                    if (!_HeaderColor.IsEmpty)
                    {
                        myGridTableStyle.HeaderForeColor = _HeaderColor;
                    }
                    if (!_HeaderBKcolor.IsEmpty)
                    {
                        myGridTableStyle.HeaderBackColor = _HeaderBKcolor;
                    }
                    myGridTableStyle.HeaderFont = HeaderFont;
                    myGridTableStyle.RowHeadersVisible = RowHeadersVisible;
                    myGridTableStyle.RowHeaderWidth = RowHeaderWidth;
                    if (!SelectionBackColor.IsEmpty)
                    {
                        myGridTableStyle.SelectionBackColor = SelectionBackColor;
                    }
                    if (!SelectionForeColor.IsEmpty)
                    {
                        myGridTableStyle.SelectionForeColor = SelectionForeColor;
                    }
                    myGridTableStyle.AllowSorting = this.AllowSorting;        //protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
                    myGridTableStyle.GridLineStyle = GridLineStyle;
                }
                if (ReadOnly)
                {
                    btSave.Visible = false;
                }
                else
                {
                    if (_HideSaveButton)
                    {
                        btSave.Visible = false;
                    }
                    else
                    {
                        btSave.Visible = true;
                        btSave.Refresh();
                    }
                }
            }
            catch (Exception err)
            {
                FormLog.NotifyException(err);
            }
        }
        private void TextBox_Click(object sender, EventArgs e)
        {
            uint n = NativeWIN32.GetTickCount();
            if (Math.Abs(n - _lastTableClick) > 500)
            {
                _lastTableClick = n;
                OnFireClick(e);
            }
        }
        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            OnFireDoubleClick(e);
        }
        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            OnFireGotFocus(e);
            uint n = NativeWIN32.GetTickCount();
            if (Math.Abs(n - _lastTableClick) > 500)
            {
                System.Drawing.Point curPoint = this.PointToClient(System.Windows.Forms.Cursor.Position);
                Rectangle rc = this.GetCellBounds(this.CurrentCell);
                if (rc.Contains(curPoint))
                {
                    _lastTableClick = n;
                    OnFireClick(e);
                }
            }
        }
        bool currentCellInSynch()
        {
            try
            {
                EasyQuery qry = _query;
                if (qry != null && nCurrentCellColumn >= 0 && nCurrentRowIndex >= 0 && qry.Fields != null)
                {
                    if (qry.Tables.Count > 0)
                    {
                        if (qry.Tables[0] != null)
                        {
                            if (qry.Fields.Count != qry.Tables[0].Columns.Count)
                            {
                                FormLog.LogMessage("Column count mismatch");
                            }
                            else
                            {
                                if (qry.Tables[0].Rows.Count >= nCurrentRowIndex)
                                {
                                    if (nCurrentCellColumn < qry.Tables[0].Columns.Count)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        void onPickValueByButton(object Sender, object Value)
        {
            EasyQuery qry = _query;
            if (qry != null && qry.Adapter != null && currentCellInSynch())
            {
                try
                {
                    System.Data.DataRow dw = CurrentRow;
                    if (dw != null)
                    {
                        dw.BeginEdit();
                        dw[nCurrentCellColumn] = Value;
                        dw.EndEdit();
                        qry.Update();
                    }
                }
                catch (Exception er)
                {
                    FormLog.NotifyException(er);
                }
            }
        }
        void onLookupSelected(object sender, System.EventArgs e)
        {
            //			int xx= 0;

            ComboLook cb = (ComboLook)sender;
            if (cb.bNoEvent)
                return;
            int n = cb.SelectedIndex;
            if (n >= 0)
            {
                object Value = cb.GetLookupData();
                if (currentCellInSynch())
                {
                    bool bMatch = false;
                    if (cbx != null && nCurrentCellColumn >= 0 && nCurrentCellColumn < cbx.Length)
                    {
                        bMatch = (cbx[nCurrentCellColumn] == sender);
                    }
                    try
                    {
                        EasyQuery qry = _query;
                        System.Data.DataRow dw = CurrentRow;
                        if (bMatch && dw != null && qry.Fields[nCurrentCellColumn].editor != null)
                        {
                            DataBind databind = null;
                            DataEditorLookupDB lk = qry.Fields[nCurrentCellColumn].editor as DataEditorLookupDB;
                            if (lk != null)
                            {
                                databind = lk.valuesMaps;
                            }
                            DataRow rv = Value as DataRow;
                            if (databind != null && rv != null)
                            {
                                if (databind.AdditionalJoins != null && databind.AdditionalJoins.StringMaps != null)
                                {
                                    for (int i = 0; i < databind.AdditionalJoins.StringMaps.Length; i++)
                                    {
                                        dw[databind.AdditionalJoins.StringMaps[i].Field1] = rv[databind.AdditionalJoins.StringMaps[i].Field2];
                                    }
                                }
                                onLookup();
                            }
                            else
                            {
                                if (rv != null)
                                {
                                    Value = rv[1];
                                }
                                bool bEQ = false;
                                int nPos = cb.GetUpdateFieldIndex();
                                if (nPos < 0)
                                {
                                    nPos = nCurrentCellColumn;
                                }
                                if (Value == null)
                                {
                                    if (dw[nPos] == null)
                                    {
                                        bEQ = true;
                                    }
                                }
                                else if (Value == System.DBNull.Value)
                                {
                                    bEQ = (dw[nPos] == System.DBNull.Value);
                                }
                                else if (Value.Equals(dw[nPos]))
                                    bEQ = true;
                                if (!bEQ)
                                {
                                    dw.BeginEdit();
                                    dw[nPos] = Value;
                                    dw.EndEdit();
                                    dw[nCurrentCellColumn] = ValueConvertor.ToObject(cb.Text, qry.Tables[0].Columns[nCurrentCellColumn].DataType);
                                }
                                //							if(xx>=0)
                                //								return;
                                //							bNoEvents = true;
                                //							cb.Visible = false;
                                //							bNoEvents = false;
                                //							onRowIndexChanged();

                                if (!bEQ)
                                {
                                    onLookup();
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        FormLog.NotifyException(er);
                    }
                }
            }
        }
        //{
        //    //
        //    base.OnMouseMove(e);
        //    //if( Site == null || !Site.DesignMode )
        //    //{
        //    //    if( _AllowDrag )
        //    //    {
        //    //        if ((e.Button & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left) 
        //    //        {
        //    //            // If the mouse moves outside the rectangle, start the drag.
        //    //            if (dragBoxFromMouseDown != System.Drawing.Rectangle.Empty && 
        //    //                !dragBoxFromMouseDown.Contains(e.X, e.Y)) 
        //    //            {

        //    //                // Create custom cursors for the drag-and-drop operation.
        //    //                try 
        //    //                {
        //    //                    MyNormalCursor = new System.Windows.Forms.Cursor(_DragCursor);
        //    //                    MyNoDropCursor = new System.Windows.Forms.Cursor(_NoDropCurs);
        //    //                    UsecustomerCursor = true;
        //    //                } 
        //    //                catch 
        //    //                {
        //    //                    // An error occurred while attempting to load the cursors, so use
        //    //                    // standard cursors.
        //    //                    UsecustomerCursor = false;
        //    //                }
        //    //                finally 
        //    //                {

        //    //                    // The screenOffset is used to account for any desktop bands 
        //    //                    // that may be at the top or left side of the screen when 
        //    //                    // determining when to cancel the drag drop operation.
        //    //                    screenOffset = System.Windows.Forms.SystemInformation.WorkingArea.Location;

        //    //                    // Proceed with the drag and drop, passing in the list item.                    
        //    //                    //                DragDropEffects dropEffect = ListDragSource.DoDragDrop(ListDragSource.Items[indexOfItemUnderMouseToDrag], DragDropEffects.All | DragDropEffects.Link);
        //    //                    //
        //    //                    //                // If the drag operation was a move then remove the item.
        //    //                    //                if (dropEffect == DragDropEffects.Move) 
        //    //                    //                {                        
        //    //                    //                  ListDragSource.Items.RemoveAt(indexOfItemUnderMouseToDrag);
        //    //                    //
        //    //                    //                  // Selects the previous item in the list as long as the list has an item.
        //    //                    //                  if (indexOfItemUnderMouseToDrag > 0)
        //    //                    //                    ListDragSource.SelectedIndex = indexOfItemUnderMouseToDrag -1;
        //    //                    //
        //    //                    //                  else if (ListDragSource.Items.Count > 0)
        //    //                    //                    // Selects the first item.
        //    //                    //                    ListDragSource.SelectedIndex =0;
        //    //                    //                }
        //    //                    System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject ();
        //    //                    data.SetData("performer",this);
        //    //                    System.Windows.Forms.DragDropEffects dropEffect = this.DoDragDrop(data,System.Windows.Forms.DragDropEffects.All);
        //    //                    if (dropEffect != System.Windows.Forms.DragDropEffects.None )
        //    //                    {
        //    //                        if (DropMe != null)
        //    //                        {
        //    //                            DropMe(this, EventArgs.Empty);
        //    //                        }
        //    //                    }
        //    //                    // Dispose of the cursors since they are no longer needed.
        //    //                    if (MyNormalCursor != null)
        //    //                        MyNormalCursor.Dispose();

        //    //                    if (MyNoDropCursor != null)
        //    //                        MyNoDropCursor.Dispose();
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}
        //protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //    // Reset the drag rectangle when the mouse button is raised.
        //    dragBoxFromMouseDown = System.Drawing.Rectangle.Empty;
        //}
        //protected bool GotDragData(System.Windows.Forms.DragEventArgs e,bool drop)
        //{
        //    bool bRet = false;
        //    DropTypes types = _DropTypes;
        //    if( types.Count > 0 )
        //    {
        //        string[] st = e.Data.GetFormats();
        //        if( st != null )
        //        {
        //            object v;
        //            string s;
        //            string[] ss;
        //            for(int i=0;i<st.Length;i++)
        //            {
        //                s = st[i];
        //                //s == "FileDrop"/"FileNameW" : v[0] is the file path
        //                ///if( s == "FileDrop" || s == "FileNameW" || "FileName" )
        //                if( s == "FileDrop" )
        //                {
        //                    bool bHasFiles = false;
        //                    bool bHasFolders = false;
        //                    int j;
        //                    v = e.Data.GetData(st[i]);
        //                    DropData objDropData = new DropData(); //PropDropData.ToDropData(propDescs[ID_DropData].objProperty.getCoreValue());
        //                    objDropData.dropType = new DropType();
        //                    objDropData.data = v;
        //                    _DropData = objDropData;
        //                    if( v != null )
        //                    {
        //                        ss = v as String[];
        //                        if( ss != null )
        //                        {
        //                            if( ss.Length > 0 )
        //                            {
        //                                for(j=0;j<ss.Length;j++)
        //                                {
        //                                    if( ! bHasFiles )
        //                                    {
        //                                        if( System.IO.File.Exists(ss[j]) )
        //                                        {
        //                                            bHasFiles = true;
        //                                        }
        //                                    }
        //                                    if( !bHasFolders )
        //                                    {
        //                                        if( System.IO.Directory.Exists(ss[j]) )
        //                                        {
        //                                            bHasFolders = true;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    for(j=0;j<types.Count;j++)
        //                    {
        //                        if( types[j] == enumDroptype.FILE && bHasFiles )
        //                        {                    
        //                            if( drop )
        //                            {
        //                                objDropData.dropType.droptype = enumDroptype.FILE;
        //                                if (DropKnownData != null)
        //                                {
        //                                    DropKnownData(this, EventArgs.Empty);
        //                                }
        //                            }
        //                            return true;
        //                        }
        //                        else if( types[j] == enumDroptype.FOLDER && bHasFolders )
        //                        {
        //                            if( drop )
        //                            {
        //                                objDropData.dropType.droptype = enumDroptype.FOLDER;
        //                                if (DropKnownData != null)
        //                                {
        //                                    DropKnownData(this, EventArgs.Empty);
        //                                }
        //                            }
        //                            return true;
        //                        }
        //                        else if( types[j] == enumDroptype.FILES && bHasFiles )
        //                        {
        //                            if( drop )
        //                            {
        //                                objDropData.dropType.droptype = enumDroptype.FILES;
        //                                if (DropKnownData != null)
        //                                {
        //                                    DropKnownData(this, EventArgs.Empty);
        //                                }
        //                            }
        //                            return true;
        //                        }
        //                        else if( types[j] == enumDroptype.FOLDERS && bHasFolders )
        //                        {
        //                            if( drop )
        //                            {
        //                                objDropData.dropType.droptype = enumDroptype.FOLDERS;
        //                                if (DropKnownData != null)
        //                                {
        //                                    DropKnownData(this, EventArgs.Empty);
        //                                }
        //                            }
        //                            return true;
        //                        }
        //                    }
        //                    return false;
        //                }
        //                if( s == "performer" )
        //                {
        //                    Object p = e.Data.GetData(s);
        //                    DropData objDropData2 = new DropData(); 
        //                    objDropData2.dropType = new DropType();
        //                    objDropData2.data = p;
        //                    _DropData = objDropData2;
        //                    if( p != null )
        //                    {
        //                        for(int j=0;j<types.Count;j++)
        //                        {
        //                            if( types[j] == enumDroptype.OBJECT )
        //                            {                    
        //                                if( drop )
        //                                {
        //                                    objDropData2.dropType.droptype = enumDroptype.OBJECT;
        //                                    if (DropKnownData != null)
        //                                    {
        //                                        DropKnownData(this, EventArgs.Empty);
        //                                    }
        //                                }
        //                                return true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return bRet;
        //}
        void updateQueryConnection()
        {
            EasyQuery qry = _query;// propDescs[ID_Query].objProperty.getCoreValue() as EPQuery;
            if (qry == null)
            {
                qry = new EasyQuery();
            }
            qry.SelectConnection(this.FindForm());
        }
//        public void OnPropertyChange(int index, object rawData, ref bool bReload)
//        {
//            if( index >= 0 && index < PropertyCount )
//            {
//                try
//                {
//                    clsEPFont ft;
//                    EPQuery qry;
//                    object v = propDescs[index].objProperty.getCoreValue();
//                    if( v != null )
//                    {
//                        switch(index)
//                        {
//                            case ID_Fields:
//                                qry = propDescs[ID_Query].objProperty.getCoreValue() as EPQuery;
//                                if( qry != null )
//                                {
//                                    fields = propDescs[ID_Fields].objProperty.getCoreValue() as FieldList;
//                                    qry.Fields = fields;
//                                }
//                                resetQuery();
//                                break;
//                            case ID_Parms:
//                                qry = propDescs[ID_Query].objProperty.getCoreValue() as EPQuery;
//                                if( qry != null )
//                                {
//                                    qry.Parameters = propDescs[ID_Parms].objProperty.getCoreValue() as FieldList;
//                                }
//                                resetQuery();
//                                break;
//                            case ID_ParentTable:
//                                resetQuery();
//                                EPParentTable pTable = v as EPParentTable;
//                                if( pTable != null )
//                                {
//                                    if( pTable.ParentTable != null )
//                                    {
//                                        EPTable pt = pTable.ParentTable.Owner as EPTable;
//                                        if( pt != null )
//                                        {
//                                            pt.OnRowIndexChange += new fnOnRowIndexChange(onParentRowIndexChange);
//                                            pt.AskCurrentRowData(this);
//                                        }
//                                    }
//                                }
//                                break;
//                            case ID_TableCaption:
//                                if( v == null )
//                                    this.CaptionText = "";
//                                else
//                                    this.CaptionText = v.ToString();
//                                break;
//                            case ID_CapVisible:
//                                if( clsProperty.ToBool(v) )
//                                {
//                                    this.CaptionVisible = true;
//                                    if( !bReadOnly )
//                                    {
//                                        if(clsProperty.ToBool(propDescs[ID_HideSaveButton].objProperty.getCoreValue()))
//                                        {
//                                            btSave.Visible = false;
//                                        }
//                                        else
//                                        {
//                                            btSave.Visible = true;
//                                        }

//                                    }
//                                }
//                                else
//                                {
//                                    this.CaptionVisible = false;
//                                    btSave.Visible = false;
//                                }
//                                break;
//                                //							case ID_BKColorEven:
//                                //								base.BackColor = clsProperty.ToColor(v);
//                                //								break;
//                                //							case ID_BKColorOdd:
//                                //								base.AlternatingBackColor =System.Drawing.Color.Azure;// clsProperty.ToColor(v);
//                                //break;
//                            case  ID_AllowDrop  :
//                                if( clsAppGlobals.RunMode )
//                                    this.AllowDrop = clsProperty.ToBool(propDescs[ID_AllowDrop].objProperty.getValue());
//                                else
//                                    this.AllowDrop = true;
//                                break;
//                            case ID_DropTypes:
//                                ((PropDropDataDesc)propDescs[ID_DropData]).dataTypes = PropDropTypes.ToDropTypes(propDescs[index].objProperty.getCoreValue());
//                                break;
//                            case ID_RowIndex:
//                                this.RowIndex = clsProperty.ToInt(v);
//                                break;
//                            case ID_ZOrder:
//                                ZOrder = clsProperty.ToInt(propDescs[ID_ZOrder].objProperty.getCoreValue());
//                                if( bLoaded )
//                                {
//                                    EPForm fOwner = this.FindForm() as EPForm;
//                                    if( fOwner != null )
//                                    {
//                                        if( !fOwner.IsDisposed )
//                                        {
//                                            fOwner.SetZOrders();
//                                        }
//                                    }
//                                }
//                                break;
//                            case ID_AlternatingBackColor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.AlternatingBackColor = clsProperty.ToColor(v);
//                                else
//                                    this.AlternatingBackColor = clsProperty.ToColor(v);
//                                break;
//                            case ID_BKcolor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.BackColor = clsProperty.ToColor(v);
//                                else
//                                    this.BackColor = clsProperty.ToColor(v);
//                                break;
//                            case ID_BKGcolor:
//                                this.BackgroundColor = clsProperty.ToColor(v);
//                                break;
//                            case ID_BorderStyle:
//                                this.BorderStyle = clsPropBorderStyle.ToBorderStyle(v);
//                                break;
//                            case ID_CapColor:
//                                ft = propDescs[ID_CapFont].objProperty.getCoreValue() as clsEPFont;
//                                if( ft != null )
//                                {
//                                    ft.color = clsProperty.ToColor(v);
//                                    this.CaptionForeColor = ft.color;
//                                }
//                                else
//                                {
//                                    this.CaptionForeColor = clsProperty.ToColor(v);
//                                }
//                                break;
//                            case ID_CapBKcolor:
//                                this.CaptionBackColor = clsProperty.ToColor(v);
//                                break;
//                            case ID_CapFont:
//                                ft = clsPropFont.ToEPFont(v);
//                                this.CaptionForeColor = ft.color;
//                                this.CaptionFont = ft.font;
//                                propDescs[ID_CapColor].objProperty.setProperty(ft.color);
//                                break;
//                            case ID_ColHeadVisible:
//                                this.ColumnHeadersVisible = clsProperty.ToBool(v);
//                                break;
//                            case ID_FlatMode:
//                                this.FlatMode = clsProperty.ToBool(v);
//                                break;
//                            case ID_Forecolor:
//                                ft = propDescs[ID_Font].objProperty.getCoreValue() as clsEPFont;
//                                ft.color = clsProperty.ToColor(v);
//                                if( myGridTableStyle != null )
//                                {
//                                    myGridTableStyle.ForeColor = ft.color;
//                                }
//                                else
//                                {
//                                    this.ForeColor = ft.color;
//                                }
//                                break;
//                            case ID_Font:
//                                ft = clsPropFont.ToEPFont(v);
//                                if( myGridTableStyle != null )
//                                {
//                                    myGridTableStyle.ForeColor = ft.color;
//                                }
//                                else
//                                {
//                                    this.ForeColor = ft.color;
//                                }
//                                this.Font = ft.font;
//                                propDescs[ID_Forecolor].objProperty.setProperty(ft.color);
//                                break;
//                            case ID_GridLineColor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.GridLineColor = clsProperty.ToColor(v);
//                                else
//                                    this.GridLineColor = clsProperty.ToColor(v);
//                                break;
//                            case ID_GridLineStyle:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.GridLineStyle = PropGridLineStyle.ToGridLineStyle(v);
//                                else
//                                    this.GridLineStyle = PropGridLineStyle.ToGridLineStyle(v);
//                                break;
//                            case ID_HeaderColor:
//                                ft = propDescs[ID_HeaderFont].objProperty.getCoreValue() as clsEPFont;
//                                if( ft != null )
//                                {
//                                    ft.color = clsProperty.ToColor(v);
//                                    if( myGridTableStyle != null )
//                                        myGridTableStyle.HeaderForeColor = ft.color;
//                                    else
//                                        this.HeaderForeColor = ft.color;
//                                }
//                                else
//                                {
//                                    if( myGridTableStyle != null )
//                                        myGridTableStyle.HeaderForeColor = clsProperty.ToColor(v);
//                                    else
//                                        this.HeaderForeColor = clsProperty.ToColor(v);
//                                }
//                                break;
//                            case ID_HeaderBKcolor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.HeaderBackColor = clsProperty.ToColor(v); 
//                                else
//                                    this.HeaderBackColor = clsProperty.ToColor(v); 
//                                break;
//                            case ID_HeaderFont:
//                                ft = clsPropFont.ToEPFont(v);
//                                if( myGridTableStyle != null )
//                                {
//                                    myGridTableStyle.HeaderForeColor = ft.color;
//                                    myGridTableStyle.HeaderFont = ft.font;
//                                }
//                                else
//                                {
//                                    this.HeaderForeColor = ft.color;
//                                    this.HeaderFont = ft.font;
//                                }
//                                propDescs[ID_HeaderColor].objProperty.setProperty(ft.color);
//                                break;
//                            case ID_RowHeadersVisible:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.RowHeadersVisible = clsProperty.ToBool(v);
//                                else
//                                    this.RowHeadersVisible = clsProperty.ToBool(v);
//                                break;
//                            case ID_RowHeaderWidth:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.RowHeaderWidth = clsProperty.ToInt(v);
//                                else
//                                    this.RowHeaderWidth = clsProperty.ToInt(v);
//                                break;
//                            case ID_SelectionBackColor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.SelectionBackColor = clsProperty.ToColor(v); 
//                                else
//                                    this.SelectionBackColor = clsProperty.ToColor(v); 
//                                break;
//                            case ID_SelectionForeColor:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.SelectionForeColor = clsProperty.ToColor(v); 
//                                else
//                                    this.SelectionForeColor = clsProperty.ToColor(v); 
//                                break;
//                            case ID_AllowNew:
//                                onBeforeBind();
//                                break;
//                            case ID_AllowDelete:
//                                onBeforeBind();
//                                break;
//                            case ID_AllowSort:
//                                if( myGridTableStyle != null )
//                                    myGridTableStyle.AllowSorting = clsProperty.ToBool (v); 
//                                else
//                                    this.AllowSorting = clsProperty.ToBool(v); 
//                                break;
////							case ID_CurColumnNum:
////								this.ColIndex = clsProperty.ToInt(v);
////								break;
//                            case ID_Dock:
//                                this.Dock = PropDockStyle.ToDockStyle(v);
//                                break;
//                            case ID_NullText:
//                                this.ApplyStyle();
//                                break;
//                            case ID_HideSaveButton:
//                                if(clsProperty.ToBool(v))
//                                {
//                                    btSave.Visible = false;
//                                }
//                                break;
//                        }
//                        if( !clsAppGlobals.RunMode )
//                            bDirty = true;
//                    }
//                }
//                catch
//                {
//                }
//            }
//        }
		/// <summary>
		/// prepare for parseQuery
		/// </summary>
        //void clearQuery()
        //{
        //    blobTable = null;
        //    RowIDFields = null;
        //    cmdTimestamp = null;
        //    _da = null;
        //    cmdDeletion = null;
        //    cmdSelection = null;
        //    cmdUpdate = null;
        //}
        [Description("_query the database to get the data. You may call this method after changing filter parameters to get new query result.")]
		/// <summary>
		/// create all commands
		/// </summary>
        public void QueryDatabase()
        {
            try
            {
                moveMouseOff();
                EasyQuery qry = _query;
                if (qry == null)
                {
                    return;
                }
                qry.Query();
                bLoaded = true;
            }
            catch (Exception er)
            {
                FormLog.NotifyException(er);
            }
        }
		#endregion
		
		#region IDataConsumer Members

		public void OnGetRow(object sender, DataRow row, BLOBRow blobRow)
		{
			onParentRowIndexChange(sender,row,blobRow);
		}
        public void OnAddNewRecord(object sender, System.EventArgs e)
        {
        }
		#endregion

		#region IDesignKeyControl Members
        [Description("Sets or gets a value indicating whether deletes are allowed if the _query is not read-only.")]
        public bool AllowDelete
        {
            get
            {
                if (ReadOnly)
                    return false;
                if (_query == null)
                    return false;
                if (_query.Tables.Count == 0)
                {
                    return false;
                }
                return _allowDelete;
            }
            set
            {
                _allowDelete = value;
            }
        }
		public bool AllowMenu
		{
			get
			{
				return true;
			}
		}
		public bool AllowMove(int x,int y)
		{
			if( this.ColumnHeadersVisible )
			{
				try
				{
					System.Drawing.Rectangle rc = this.GetCellBounds(0,0);
					if( this.CaptionVisible )
					{
						if( y >= rc.Height && y <= rc.Y )
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					else
					{
						if( y > rc.Y )
							return true;
						else
							return false;
					}
				}
				catch
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}
		#endregion

		#region DragDropSupport
		private System.Drawing.Rectangle dragBoxFromMouseDown = System.Drawing.Rectangle.Empty;
		private System.Windows.Forms.Cursor MyNoDropCursor = null;
		private System.Windows.Forms.Cursor MyNormalCursor = null;
		private bool UsecustomerCursor=true;
        //private System.Drawing.Point screenOffset;
        //private bool bDragEnter = false;
		protected override void OnGiveFeedback(System.Windows.Forms.GiveFeedbackEventArgs e)
		{
			// Use custom cursors if the check box is checked.
			if (UsecustomerCursor) 
			{

				// Sets the custom cursor based upon the effect.
				e.UseDefaultCursors = false;
				if ((e.Effect & System.Windows.Forms.DragDropEffects.Move) == System.Windows.Forms.DragDropEffects.Move)
					System.Windows.Forms.Cursor.Current = MyNormalCursor;
				else 
					System.Windows.Forms.Cursor.Current = MyNoDropCursor;
			}

		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				base.OnMouseDown(e);
			
				OnMouseDown2(e);
				// Remember the point where the mouse down occurred. The DragSize indicates
				// the size that the mouse can move before a drag event should be started.                
				System.Drawing.Size dragSize = System.Windows.Forms.SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				dragBoxFromMouseDown = new System.Drawing.Rectangle(new System.Drawing.Point(e.X - (dragSize.Width /2),
					e.Y - (dragSize.Height /2)), dragSize);
			}
			catch
			{
			}
			
		}
//		static FRMdebugTrace frmDebug = null;

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            try
            {
                base.OnCurrentCellChanged(e);
                if (bNoEvents || !pageLoaded)
                {
                    return;
                }
                bool bRowChanged = false;
                if (nCurrentCellColumn != this.CurrentCell.ColumnNumber)
                {
                    OnCellNumberChanged();
                    nCurrentCellColumn = this.CurrentCell.ColumnNumber;
                }
                if (nCurrentRowIndex != this.CurrentRowIndex)
                {
                    nCurrentRowIndex = this.CurrentRowIndex;
                    bRowChanged = true;
                }
                EasyQuery qry = _query;
                if (qry != null)
                {
                    if (qry.Fields.Count > 0)
                    {
                        for (int i = 0; i < qry.Fields.Count; i++)
                        {
                            if (cbx != null)
                                if (cbx[i] != null)
                                    cbx[i].Visible = false;
                            if (bts != null)
                                if (bts[i] != null)
                                    bts[i].Visible = false;
                        }
                    }
                }
                if (currentCellInSynch())
                {
                    //sCurCaption = _query.Tables[0].Columns[nCurrentCellColumn].Caption;
                    //					if( !bReadOnly && columnEditable(nCurrentCellColumn) )
                    //					{
                    if (cbx != null)
                    {
                        if (cbx[nCurrentCellColumn] != null)
                        {
                            if ( /*!fields[nCurrentCellColumn].ReadOnly &&*/ qry.Fields[nCurrentCellColumn].OleDbType != System.Data.OleDb.OleDbType.DBTimeStamp)
                            {
                                object v0 = this[nCurrentRowIndex, nCurrentCellColumn];
                                //									string sValue = clsProperty.ToString(v0);
                                System.Drawing.Rectangle rc = this.GetCurrentCellBounds();
                                cbx[nCurrentCellColumn].SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
                                ComboLook cbxLK = cbx[nCurrentCellColumn] as ComboLook;
                                if (cbxLK != null)
                                {
                                    DataRowView rv;
                                    for (int i = 0; i < cbxLK.Items.Count; i++)
                                    {
                                        //											string sv;
                                        object v;
                                        rv = cbxLK.Items[i] as DataRowView;
                                        if (rv != null)
                                        {
                                            v = rv[0];
                                        }
                                        else
                                        {
                                            v = cbxLK.Items[i];
                                        }
                                        if (LogicExp.Compare(v, enumLogicType2.Equal, v0))
                                        {
                                            cbxLK.bNoEvent = true;
                                            cbxLK.SelectedIndex = i;
                                            cbxLK.bNoEvent = false;
                                            break;
                                        }
                                    }
                                    if (cbxLK.SelectedIndex < 0)
                                    {
                                        cbxLK.SetSelectedIndex(v0);
                                    }
                                    //
                                }
                                else
                                {
                                    cbx[nCurrentCellColumn].SelectedIndex = -1;
                                    if (this[nCurrentRowIndex, nCurrentCellColumn] != null)
                                        cbx[nCurrentCellColumn].Text = ValueConvertor.ToString(v0);
                                    else
                                        cbx[nCurrentCellColumn].Text = "";
                                }
                                cbx[nCurrentCellColumn].Visible = true;
                                cbx[nCurrentCellColumn].BringToFront();
                            }
                        }
                    }
                    if (!ReadOnly && columnEditable(nCurrentCellColumn))
                    {
                        if (bts != null)
                        {
                            if (bts[nCurrentCellColumn] != null)
                            {
                                if ( /*!fields[nCurrentCellColumn].ReadOnly &&*/ qry.Fields[nCurrentCellColumn].OleDbType != System.Data.OleDb.OleDbType.DBTimeStamp)
                                {
                                    System.Drawing.Rectangle rc = this.GetCurrentCellBounds();
                                    bts[nCurrentCellColumn].SetBounds(rc.Left + rc.Width - 20, rc.Top, 20, rc.Height);
                                    bts[nCurrentCellColumn].Visible = true;
                                    bts[nCurrentCellColumn].BringToFront();
                                    if (qry.Fields[nCurrentCellColumn].editor != null)
                                    {
                                        if (this[nCurrentRowIndex, nCurrentCellColumn] == null)
                                            qry.Fields[nCurrentCellColumn].editor.currentValue = "";
                                        else
                                            qry.Fields[nCurrentCellColumn].editor.currentValue = this[nCurrentRowIndex, nCurrentCellColumn].ToString();
                                    }
                                }
                            }
                        }
                    }
                    if (bRowChanged)
                    {
                        System.Data.DataRow dw = CurrentRow;
                        if (dw != null)
                        {
                            for (int i = 0; i < qry.Fields.Count; i++)
                            {
                                qry.Fields[i].Value = dw[i];
                            }
                        }
                        onRowIndexChanged();
                        if (CurrentRowIndexChange != null)
                            CurrentRowIndexChange(this, new System.EventArgs());
                    }
                }
                //else
                //{
                //    if (_query != null)
                //    {
                //        if (_query.Tables.Count > 0)
                //        {
                //            if (_query.Tables[0] != null)
                //            {
                //                if (nCurrentRowIndex == 0 && _query.Tables[0].Rows.Count == 0)
                //                {
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception er)
            {
                FormLog.NotifyException(er);
            }
        }
        private bool columnEditable(int i)
        {
            EasyQuery qry = _query;
            if (qry != null)
            {
                if (i >= 0 && i < qry.Fields.Count)
                {
                    EPField fld = qry.Fields[i];
                    if (fld != null)
                    {
                        return !fld.ReadOnly;
                    }
                }
            }
            return false;
        }
		#endregion

		#region IDataBinder Members
        //public FieldList Fields
        //{
        //    get
        //    {
        //        EPQuery qry = _query;// propDescs[ID_Query].objProperty.getCoreValue() as EPQuery;
        //        if( qry != null )
        //        {
        //            return qry.Fields;
        //        }
        //        return null;
        //    }
        //}
		public DataSet SourceDataSet
		{
			get
			{
                return _query;
			}
		}

		public string DataMemberTable
		{
			get
			{
				return DataTableName;
			}
		}
        [Browsable(false)]
        [Description("Fields in this data table. It represents the current row at runtime.")]
        public FieldList Fields
        {
            get
            {
                if (_query != null)
                {
                    return _query.Fields;
                }
                return new FieldList();
            }
        }
		public object GetDataSource()
		{
            if (_query != null && _query.Tables.Count > 0)
            {
                if (_query.Tables[0] != null)
                {
                    return _query.Tables[0].DefaultView;
                }
            }
			return null;
		}
		public void SetOnQuery(System.EventHandler eh)
		{
			RequeryFinished += eh;
		}
		public void RemoveOnQuery(System.EventHandler eh)
		{
			RequeryFinished -= eh;
		}
		public void SetFieldAsFile(string fieldName)
		{
			if( Site != null && Site.DesignMode && _query != null )
			{
				if( !string.IsNullOrEmpty(fieldName) )
				{
					//FieldList fields = propDescs[ID_Fields].objProperty.getCoreValue() as FieldList;
					EPField fld = _query.Fields[fieldName];
					if( fld != null )
					{
						fld.IsFile = true;
					}
				}
			}
		}
		public void SetOnRowChange(fnOnRowIndexChange eh)
		{
			OnRowIndexChange += eh;
		}
        //private void onHideData()
        //{
        //    if (_Visible)
        //    {
        //        if (frmData != null)
        //        {
        //            frmData.bCanClose = true;
        //            frmData.Close();
        //            frmData = null;
        //        }
        //        this.Visible = true;
        //        int x = _left;
        //        int y = _top;
        //        this.Location = new System.Drawing.Point(x, y);
        //        y = _height;
        //        if (y < 10)
        //            y = 10;
        //        x = _width;
        //        if (x < 10)
        //            x = 10;
        //        this.ClientSize = new System.Drawing.Size(x, y);
        //    }
        //    else
        //    {
        //        //this.Visible = false;
        //        //this.Height = 1;
        //        //this.Width  = 1;
        //        this.Left = -this.Width - 10;
        //        if (Site != null && Site.DesignMode)
        //        {
        //            //EPForm page = this.FindForm() as EPForm;
        //            //if (page != null)
        //            //{
        //            //    if (clsAppGlobals.ClearControlSelection != null)
        //            //    {
        //            //        clsAppGlobals.ClearControlSelection(page);
        //            //    }
        //            //}
        //            if (frmData == null)
        //            {
        //                frmData = new frmDataTable();
        //                if (page != null)
        //                {
        //                    //page.VisibleChanged +=new EventHandler(page_VisibleChanged);
        //                    //page.Resize += new EventHandler(page_Resize);
        //                    page.OnPageWindowStateChange += new fnPageWindowStateChange(page_OnPageWindowStateChange);
        //                }
        //            }
        //            frmData.tblOwner = this;
        //            frmData.Owner = page;
        //            if (daOle != null)
        //            {
        //                frmData.SetLinks(fields, TableName, dataSet, daOle);
        //            }
        //            else if (daSql != null)
        //            {
        //                frmData.SetLinks(fields, TableName, dataSet, daSql);
        //            }
        //            else if (daOdbc != null)
        //            {
        //                frmData.SetLinks(fields, TableName, dataSet, daOdbc);
        //            }
        //            frmData.ApplyStyle();
        //            frmData.BindData();
        //            frmData.Text = this.FindForm().Text;
        //            frmData.OnCellIndexChange += new EventHandler(frmData_OnRowIndexChange);
        //            if (page != null)
        //            {
        //                if (page.ModuleState)
        //                {
        //                    frmData.TopLevel = false;
        //                    frmData.Parent = page;
        //                }
        //            }
        //            frmData.Show();
        //        }
        //    }
        //}
        //public bool HideData
        //{
        //    get
        //    {
        //        return !_Visible;
        //    }
        //    set
        //    {
        //        _Visible = !value;
        //        onHideData();   
        //    }
        //}
		public bool AutoNavigate
		{
			get
			{ 
				return true;
			}
		}
        [ReadOnly(true)]
		public int RowIndex
		{
			get
			{
				return this.CurrentRowIndex;
			}
			set
			{
				if( value >= 0 )
				{
					try
					{
						this.CurrentRowIndex = value;
					}
					catch
					{
					}
				}
			}
		}
        [Description("The column number for the current cell")]
		public int ColIndex
		{
			get
			{
				return this.CurrentCell.ColumnNumber;
			}
//			set
//			{
//				if( value >= 0 )
//				{
//					try
//					{
//						this.CurrentCell.ColumnNumber = value;
//					}
//					finally
//					{
//					}
//				}
//			}
		}
        
		/// <summary>
		/// force firing of OnRowIndexChange event
		/// </summary>
		public void FireOnRowIndexChange()
		{
			pageLoaded = true;
			bRowChanging = true;
			try
			{
				onRowIndexChanged();
			}
			catch
			{
			}
			bRowChanging = false;
		}
		public bool RowChanging { get { return bRowChanging; } }
		public void RemoveOnRowIndexChange(fnOnRowIndexChange eh)
		{
			OnRowIndexChange -= eh;
		}
        [Browsable(false)]
		/// <summary>
		/// Database connection
		/// </summary>
        public Connection DatabaseConnection
        {
            get
            {
                if (_query != null)
                    return _query.DatabaseConnection;
                return null;
            }
        }
		#endregion

        #region Column changes

        bool bColumnEvent = false;
		bool bCancelColumnChange = false;
		object propVal;
		protected void cancelChanges()
		{
			if( bColumnEvent )
			{
				bCancelColumnChange = true;
			}
			else
			{
                EasyQuery qry = _query;
                if (qry != null)
                {
                    Form frm = this.FindForm();
                    if (frm != null)
                    {
                        if (!frm.IsDisposed)
                        {
                            try
                            {
                                frm.BindingContext[qry, DataTableName].CancelCurrentEdit();
                            }
                            catch
                            {
                            }
                            QueryDatabase();
                        }
                    }
                }
			}
		}
		
		private void EPTable_ColumnChanging(object sender, DataColumnChangeEventArgs e)
		{
			if( !bColumnEvent )
			{
				bColumnEvent = true;
				bCancelColumnChange = false;
                _changedColumnName = e.Column.ColumnName;
				propVal = e.ProposedValue;
                if (BeforeColChange != null)
                {
                    BeforeColChange(this, EventArgs.Empty);
                }
				bColumnEvent = false;
				if( bCancelColumnChange )
				{
					e.ProposedValue = e.Row[e.Column.Ordinal];
				}
				else
				{
					e.ProposedValue = propVal;
				}
			}
		}

		private void EPTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
            _changedColumnName = e.Column.ColumnName;
			EasyQuery qry = _query;
			if( qry != null )
			{
                //fields = propDescs[ID_Fields].objProperty.getCoreValue() as FieldList;
				qry.Fields[e.Column.ColumnName].Value = e.Row[e.Column.ColumnName];
			}
            if (AfterColChange != null)
            {
                AfterColChange(this, EventArgs.Empty);
            }
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
                PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
                List<PropertyDescriptor> list = new List<PropertyDescriptor>();
                foreach (PropertyDescriptor p in ps)
                {
                    if (_st_excludedPropertyNames.Contains(p.Name))
                    {
                            continue;
                    }
                    list.Add(p);
                }
                ps = new PropertyDescriptorCollection(list.ToArray());
                return ps;
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[] { });
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region IPostDeserializeProcess Members

        public void OnDeserialize(object context)
        {
            Form f = this.FindForm();
            if (f != null)
            {
                if (_onFormClosing == null)
                {
                    _onFormClosing = new FormClosingEventHandler(f_FormClosing);
                }
                else
                {
                    f.FormClosing -= _onFormClosing;
                }
                f.FormClosing += _onFormClosing;
            }
        }

        #endregion
    }
}
