using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using LimnorUI;
using System.Data.Common;
using System.ComponentModel;
using System.Drawing.Design;

namespace LimnorDatabase
{
	/// <summary>
    /// Data source is provided by cmdSelection
	/// </summary>
	public class EPDataGrid : System.Windows.Forms.DataGrid
	{
        public event System.EventHandler CurrentRowIndexChange = null;
        public event System.EventHandler RequeryFinished = null;
        //
        private EasyQuery _query;
        //
        private System.Windows.Forms.ComboBox[] cbx = null; //look ups including Yes/No
		private System.Windows.Forms.Button[] bts=null; //datetime
        private int nCurrentRowIndex = -1;
        private int nCurrentCellColumn = -1;
        private System.Windows.Forms.Button btSave = null;

        private string sCurCaption = "";
        private bool bNewTable = false;
        private bool bNoEvents = false;
        private DataGridTableStyle myGridTableStyle = null;
        private bool bLoadingData = false;
        private bool pageLoaded = false;

		private uint _lastTableClick = 0;
		public EPDataGrid()
		{
			this.AllowSorting = false;
			btSave = new System.Windows.Forms.Button();
			btSave.Parent = this;
			btSave.Width = 20;
			btSave.Height = 20;
			btSave.Top = 0;
            btSave.Image = Resource1.save; 
			btSave.Left = this.Width - btSave.Width;
			btSave.BackColor = System.Drawing.Color.FromArgb(255,236,233,216);
			btSave.Click += new System.EventHandler(onSaveButtonClick);
			//
			this.Scroll += new EventHandler(EPDataGrid_Scroll);
		}
        //public System.Data.OleDb.OleDbConnection GetOleDbConnection()
        //{
        //    if( daOle != null )
        //    {
        //        if( daOle.SelectCommand != null )
        //            return daOle.SelectCommand.Connection;
        //    }
			
        //    return null;
        //}
		public virtual bool ActAsVisible()
		{
			return true;
		}
		protected virtual string NullText
		{
			get
			{
				return "{null}";
			}
		}
        /// <summary>
        /// save changes by moving mouse to the last row and click
        /// </summary>
		protected void moveMouseOff()
		{
			bool bOK = false;
			if( _query != null )
			{
                if (_query.Tables.Count > 0)
				{
    				bOK = true;
				}
			}
			if( bOK )
			{
				try
				{
                    //remember current position
					Point curPoint = Cursor.Position;
                    //
					Rectangle rc = GetCurrentCellBounds();
					Point p2;
					Point p = new Point(rc.X, rc.Y+rc.Height + 2);
					p = PointToScreen(p);
					p2 = new Point(rc.X, rc.Y + rc.Height + 2);
					p2 = PointToScreen(p2);
					//move to the next row
                    UIUtil.ClickMouse(p2.X, p2.Y);
					Application.DoEvents();
					Application.DoEvents();
                    UIUtil.ClickMouse(p2.X, p2.Y);
					Application.DoEvents();
					Application.DoEvents();
					//move back
                    UIUtil.ClickMouse(p.X, p.Y);
					Application.DoEvents();
					Application.DoEvents();
                    UIUtil.ClickMouse(p.X, p.Y);
					Application.DoEvents();
					Application.DoEvents();
					//move to original position
                    UIUtil.MoveMouse(curPoint.X, curPoint.Y);
				}
				catch
				{
				}
			}
		}
        /// <summary>
        /// save changes by moving mouse to the last row and click
        /// </summary>
		protected void acceptByMouse()
		{
			if( !this.ReadOnly && ActAsVisible() )
			{
				moveMouseOff();
				
			}
		}
		public string DataTableName
		{
			get
			{
                if (_query != null && _query.Tables.Count > 0)
                {
                    return _query.Tables[0].TableName;
                }
                return string.Empty;
			}
		}
		public string CurrentColumnCaption
		{
            get
            {
                if (_query != null && _query.Tables.Count > 0)
                {
                    if (_query.Tables[0] != null)
                    {
                        if (this.CurrentCell.ColumnNumber >= 0 && this.CurrentCell.ColumnNumber < _query.Tables[0].Columns.Count)
                        {
                            return _query.Tables[0].Columns[this.CurrentCell.ColumnNumber].Caption;
                        }
                    }
                }
                return string.Empty;
            }
		}
		public virtual void CloseConnections()
		{
            if (_query != null)
            {
                _query.CloseConnection();
            }
		}
        //[Editor(typeof(QuerySelector), typeof(UITypeEditor))]
        [Description("The SQL query to get the data. The Query Builder helps you build SQL query.")]
        public EasyQuery Query
        {
            get
            {
                return _query;
            }
            //set
            //{
            //    _query = value;
            //    QueryDatabase();
            //}
        }
        public virtual void QueryDatabase()
        {
            Requery();
        }
        protected bool daReady
        {
            get
            {
                return (_query != null && _query.Adapter != null);
            }
        }
        [Description("Set a new query for the data table.")]
        public void SetQuery(EasyQuery query)
        {
            _query = query;
            QueryDatabase();
        }
        protected virtual void cancelChanges()
        {
            if (_query != null)
            {
                Form frm = this.FindForm();
                if (frm != null)
                {
                    if (!frm.IsDisposed)
                    {
                        try
                        {
                            frm.BindingContext[_query, DataTableName].CancelCurrentEdit();
                        }
                        catch
                        {
                        }
                        Requery();
                    }
                }
            }
        }
		protected void onSaveButtonClick(object sender,System.EventArgs e)
		{
            //for error log
            string sUpdate = "";
            string sInsert = "";
            string sDelete = "";
            try
            {
                if (_query != null)
                {
                    sDelete = _query.DeleteSql;
                    sInsert = _query.InsertSql;
                    sUpdate = _query.UpdateSql;
                    if (_query.Tables[0] != null)
                    {
                        int nCurrentRowIndex = this.CurrentRowIndex;
                        acceptByMouse();
                        Form frm = this.FindForm();
                        if (frm != null)
                        {
                            if (!frm.IsDisposed)
                            {
                                frm.BindingContext[_query, DataTableName].EndCurrentEdit();
                                if (_query.Update())
                                {
                                    this.CurrentRowIndex = nCurrentRowIndex;
                                    Requery();
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Data.DBConcurrencyException)
            {
                RequeryAfterError();
            }
            catch (Exception er)
            {
                FormLog.NotifyException(er, "Update command:{0}.\r\nInsert command:{1}.\r\nDelete command {2}", sUpdate, sInsert, sDelete);
                RequeryAfterError();
            }
		}
        protected void RequeryAfterError()
        {
            int n = nCurrentRowIndex;
            Requery();
            if (_query != null && _query.Tables.Count > 0)
            {
                if (n >= 0 && n < _query.Tables[0].Rows.Count)
                {
                    this.CurrentRowIndex = n;
                    nCurrentRowIndex = n;
                }
            }
        }
		protected virtual void onBeforeBind()
		{
		}
        protected virtual bool Requery()
        {
            if (bLoadingData)
                return false;
            bLoadingData = true;
            int nCurrentRowIndex = this.CurrentRowIndex;

            this.DataBindings.Clear();

            if (_query != null)
            {
                _query.RequerySelection();

                if (_query.Tables.Count > 0)
                {
                    bNewTable = (_query.Tables[0].Rows.Count == 0);
                    _query.Tables[0].RowDeleted += new DataRowChangeEventHandler(EPDataGrid_RowDeleted);

                    bindData();
                    if (nCurrentRowIndex >= 0 && nCurrentRowIndex < _query.Tables[0].Rows.Count)
                    {
                        this.CurrentRowIndex = nCurrentRowIndex;
                    }
                    onRowIndexChanged();
                    if (RequeryFinished != null)
                    {
                        RequeryFinished(this, EventArgs.Empty);
                    }
                    OnCurrentCellChanged(null);
                    _query.Tables[0].DefaultView.ListChanged += new ListChangedEventHandler(DefaultView_ListChanged);
                }
            }
            bLoadingData = false;
            this.Invalidate();
            return (_query != null && _query.Tables.Count > 0);
        }
		protected void bindData()
		{
			onBeforeBind();
			bLoadingData = true;
            if (_query != null)
            {
                this.DataSource = _query.Tables[0].DefaultView;
            }
			bLoadingData = false;
		}
		public void BindData()
		{
			try
			{
				this.DataBindings.Clear();
                if (_query != null)
				{
                    if (_query.Tables.Count > 0)
					{
                        if (_query.Tables[0] != null)
						{
							bindData();
						}
					}
				}
			}
			catch(Exception er)
			{
				FormLog.NotifyException(er);
			}
		}
		protected void setFieldEditor(int i)
		{
			if( _query == null )
				return;
            if (i < 0 || i >= _query.Fields.Count)
				return;
            if (_query.Fields[i].editor != null)
			{
                DataEditorButton bn = _query.Fields[i].editor as DataEditorButton;
				if( bn != null )
				{
					bts[i] = bn.MakeButton(this.FindForm());
					if( bts[i] != null )
					{
						bts[i].Parent = this;
						bts[i].Visible = false;
                        _query.Fields[i].editor.OnPickValue += new fnOnPickValue(onPickValueByButton);
					}
				}
				else
				{
                    DataEditorLookupYesNo LK = _query.Fields[i].editor as DataEditorLookupYesNo;
					if( LK != null )
					{
						cbx[i] = LK.MakeComboBox();
						if( cbx[i] != null )
						{
							cbx[i].Parent = this;
							cbx[i].Visible = false;
							cbx[i].SelectedIndexChanged += new System.EventHandler(onLookupSelected);
						}
					}
				}
			}
			else
			{
				cbx[i] = null;
				bts[i] = null;
			}
		}
		public virtual void ApplyStyle()
		{
            //btSave.Visible = (!bReadOnly);
			if( _query == null )
				return;
            if (_query.Fields.Count == 0)
				return;
			int i;
            int n = _query.Fields.Count;
			if( cbx != null )
			{
				for(i=0;i<cbx.Length;i++)
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
			if( bts != null )
			{
				for(i=0;i<bts.Length;i++)
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
            for (i = 0; i < _query.Fields.Count; i++)
			{
                if (ReadOnly || !EPField.IsBinary(_query.Fields[i].OleDbType))
				{
                    if (_query.Fields[i].OleDbType == System.Data.OleDb.OleDbType.Boolean)
					{
						TextCol = new DataGridBoolColumn();
					}
					else
					{
                        TextCol = new TextColumnStyle(_query.Fields[i].HeaderAlignment);// new DataGridTextBoxColumn();
						//						((DataGridTextBoxColumn)TextCol).TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
						((DataGridTextBoxColumn)TextCol).TextBox.DoubleClick +=new EventHandler(TextBox_DoubleClick);
						((DataGridTextBoxColumn)TextCol).TextBox.Click +=new EventHandler(TextBox_Click);
						((DataGridTextBoxColumn)TextCol).TextBox.GotFocus +=new EventHandler(TextBox_GotFocus);
                        ((TextColumnStyle)TextCol).Format = _query.Fields[i].Format;
						if(NullText != null)
						{
							((TextColumnStyle)TextCol).NullText = NullText;
						}
					}

                    TextCol.MappingName = _query.Fields[i].Name;
                    TextCol.Alignment = _query.Fields[i].TxtAlignment;
                    cap = _query.Fields[i].FieldCaption;
					if(string.IsNullOrEmpty(cap) )
                        cap = _query.Fields[i].Name;
					TextCol.HeaderText = cap;
					if( ReadOnly )
						TextCol.ReadOnly = true;
					else
					{
                        if (_query.Fields[i].IsIdentity || _query.Fields[i].ReadOnly
                            || EPField.IsBinary(_query.Fields[i].OleDbType)
                            || _query.Fields[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp)
							TextCol.ReadOnly = true;
					}
                    if (_query.Fields[i].Visible)
					{
                        TextCol.Width = _query.Fields[i].ColumnWidth;
					}
					else
						TextCol.Width = 0;
				
					myGridTableStyle.GridColumnStyles.Add(TextCol);
                    if (_query.Fields[i].editor != null)
					{
                        DataEditorButton bn = _query.Fields[i].editor as DataEditorButton;
						if( bn != null )
						{
							bts[i] = bn.MakeButton(this.FindForm());
							if( bts[i] != null )
							{
								bts[i].Parent = this;
								bts[i].Visible = false;
                                _query.Fields[i].editor.OnPickValue += new fnOnPickValue(onPickValueByButton);
							}
						}
						else
						{
                            DataEditorLookupYesNo LK = _query.Fields[i].editor as DataEditorLookupYesNo;
							if( LK != null )
							{
								cbx[i] = LK.MakeComboBox();
								if( cbx[i] != null )
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
            //this.ReadOnly = bReadOnly;
			this.TableStyles.Clear();
			this.TableStyles.Add(myGridTableStyle);
			
		}
        //public void SetLinks(FieldList flds,string tblName,DataSet ds, object d)
        //{
        //    fields = flds;
        //    TableName = tblName;
        //    dataSet = ds;
        //    if( d is System.Data.OleDb.OleDbDataAdapter )
        //    {
        //        daOle = d as System.Data.OleDb.OleDbDataAdapter;
        //        daSql = null;
        //        daOdbc = null;
        //    }
        //    else if( d is System.Data.SqlClient.SqlDataAdapter )
        //    {
        //        daOle = null;
        //        daSql = d as System.Data.SqlClient.SqlDataAdapter;
        //        daOdbc = null;
        //    }
        //    else if( d is System.Data.Odbc.OdbcDataAdapter )
        //    {
        //        daOle = null;
        //        daSql = null;
        //        daOdbc = d as System.Data.Odbc.OdbcDataAdapter;
        //    }
        //    else
        //    {
        //        daOle = null;
        //        daSql = null;
        //        daOdbc = null;
        //    }
        //}
        [ReadOnly(true)]
        public int CurrentColumnIndex
        {
            get
            {
                return nCurrentCellColumn;
            }
            set
            {
                nCurrentCellColumn = value;
            }
        }
		bool currentCellInSynch()
		{
            try
            {
                if (_query != null && nCurrentCellColumn >= 0 && nCurrentRowIndex >= 0 && _query.Fields != null)
                {
                    if (_query.Tables.Count > 0)
                    {
                        if (_query.Tables[0] != null)
                        {
                            if (_query.Fields.Count != _query.Tables[0].Columns.Count)
                            {
                                FormLog.LogMessage("Column count mismatch");
                            }
                            else
                            {
                                if (_query.Tables[0].Rows.Count >= nCurrentRowIndex)
                                {
                                    if (nCurrentCellColumn < _query.Tables[0].Columns.Count)
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
		void onPickValueByButton(object Sender,object Value)
		{
            if (_query != null && _query.Adapter != null && currentCellInSynch())
			{
				try
				{
					System.Data.DataRow dw = CurrentRow;
					if( dw != null )
					{
						dw.BeginEdit();
						dw[nCurrentCellColumn] = Value;
						dw.EndEdit();
                        _query.Adapter.Update(_query);
					}
				}
				catch(Exception er)
				{
					FormLog.NotifyException(er);
				}
			}
		}
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
                    if (_query != null && _query.Tables.Count > 0)
                    {
                        if (_query.Tables[0] != null)
                        {
                            BindingManagerBase bmb = BindingContext[_query.Tables[0].DefaultView];
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
		protected virtual void onLookup()
		{
		}
		void onLookupSelected(object sender,System.EventArgs e)
		{
//			int xx= 0;
			
			ComboLook cb = (ComboLook)sender;
			if(cb.bNoEvent )
				return;
			int n = cb.SelectedIndex;
			if( n >= 0 )
			{
				object Value = cb.GetLookupData();
				if( currentCellInSynch() )
				{
					bool bMatch = false;
					if(cbx != null && nCurrentCellColumn >= 0 && nCurrentCellColumn < cbx.Length)
					{
						bMatch = (cbx[nCurrentCellColumn] == sender);
					}
					try
					{
						System.Data.DataRow dw = CurrentRow;
                        if (bMatch && dw != null && _query.Fields[nCurrentCellColumn].editor != null)
						{
							DataBind databind = null;
                            DataEditorLookupDB lk = _query.Fields[nCurrentCellColumn].editor as DataEditorLookupDB;
							if(lk != null)
							{
								databind = lk.valuesMaps;
							}
							DataRow rv = Value as DataRow;
							if(databind != null && rv != null)
							{
								if(databind.AdditionalJoins != null && databind.AdditionalJoins.StringMaps != null)
								{
									for(int i=0;i<databind.AdditionalJoins.StringMaps.Length;i++)
									{
										dw[databind.AdditionalJoins.StringMaps[i].Field1] = rv[databind.AdditionalJoins.StringMaps[i].Field2];
									}
								}
								onLookup();
							}
							else
							{
								if(rv != null)
								{
									Value = rv[1];
								}
								bool bEQ = false;
								int nPos = cb.GetUpdateFieldIndex();
								if( nPos < 0 )
								{
									nPos = nCurrentCellColumn;
								}
								if( Value == null )
								{
									if( dw[nPos] == null )
									{
										bEQ = true;
									}
								}
								else if( Value == System.DBNull.Value )
								{
									bEQ = (dw[nPos] == System.DBNull.Value);
								}
								else if( Value.Equals(dw[nPos]) )
									bEQ = true;
								if( !bEQ )
								{
									dw.BeginEdit();
									dw[nPos] = Value;
									dw.EndEdit();
									dw[nCurrentCellColumn] = ValueConvertor.ToObject(cb.Text,_query.Tables[0].Columns[nCurrentCellColumn].DataType);
								}
								//							if(xx>=0)
								//								return;
								//							bNoEvents = true;
								//							cb.Visible = false;
								//							bNoEvents = false;
								//							onRowIndexChanged();

								if( !bEQ )
								{
									onLookup();
								}
							}
						}
					}
					catch(Exception er)
					{
						FormLog.NotifyException(er);
					}
				}
			}
		}
		protected virtual bool columnEditable(int i)
		{
			if( _query != null )
			{
                if (i >= 0 && i < _query.Fields.Count)
				{
                    EPField fld = _query.Fields[i];
					if( fld != null )
					{
						return !fld.ReadOnly;
					}
				}
			}
			return false;
		}
		protected virtual void OnCellNumberChanged()
		{
		}
		protected override void OnCurrentCellChanged(EventArgs e)
		{
			try
			{
				base.OnCurrentCellChanged(e);
				if( bNoEvents || !pageLoaded)
				{
					return;
				}
				bool bRowChanged = false;
				if( nCurrentCellColumn != this.CurrentCell.ColumnNumber )
				{
					OnCellNumberChanged();
					nCurrentCellColumn = this.CurrentCell.ColumnNumber;
				}
				if( nCurrentRowIndex != this.CurrentRowIndex )
				{
					nCurrentRowIndex = this.CurrentRowIndex;
					bRowChanged = true;
				}
				if( _query != null )
				{
                    if (_query.Fields.Count > 0)
					{
                        for (int i = 0; i < _query.Fields.Count; i++)
						{
							if( cbx != null )
								if( cbx[i] != null )
									cbx[i].Visible = false;
							if( bts != null )
								if( bts[i] != null )
									bts[i].Visible = false;
						}
					}
				}
                if (currentCellInSynch())
                {
                    sCurCaption = _query.Tables[0].Columns[nCurrentCellColumn].Caption;
                    //					if( !bReadOnly && columnEditable(nCurrentCellColumn) )
                    //					{
                    if (cbx != null)
                    {
                        if (cbx[nCurrentCellColumn] != null)
                        {
                            if ( /*!fields[nCurrentCellColumn].ReadOnly &&*/ _query.Fields[nCurrentCellColumn].OleDbType != System.Data.OleDb.OleDbType.DBTimeStamp)
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
                                if ( /*!fields[nCurrentCellColumn].ReadOnly &&*/ _query.Fields[nCurrentCellColumn].OleDbType != System.Data.OleDb.OleDbType.DBTimeStamp)
                                {
                                    System.Drawing.Rectangle rc = this.GetCurrentCellBounds();
                                    bts[nCurrentCellColumn].SetBounds(rc.Left + rc.Width - 20, rc.Top, 20, rc.Height);
                                    bts[nCurrentCellColumn].Visible = true;
                                    bts[nCurrentCellColumn].BringToFront();
                                    if (_query.Fields[nCurrentCellColumn].editor != null)
                                    {
                                        if (this[nCurrentRowIndex, nCurrentCellColumn] == null)
                                            _query.Fields[nCurrentCellColumn].editor.currentValue = "";
                                        else
                                            _query.Fields[nCurrentCellColumn].editor.currentValue = this[nCurrentRowIndex, nCurrentCellColumn].ToString();
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
                            for (int i = 0; i < _query.Fields.Count; i++)
                            {
                                _query.Fields[i].Value = dw[i];
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
			catch(Exception er)
			{
				FormLog.NotifyException(er);
			}
		}
		protected virtual void onRowIndexChanged()
		{
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			btSave.Left = this.Width - btSave.Width;
		}
		protected void EPDataGrid_Scroll(object sender,EventArgs e)
		{
			if( _query != null )
			{
                if (_query.Fields.Count > 0)
				{
                    for (int i = 0; i < _query.Fields.Count; i++)
					{
						if( cbx != null )
							if( cbx[i] != null )
								cbx[i].Visible = false;
						if( bts != null )
							if( bts[i] != null )
								bts[i].Visible = false;
					}
				}
			}
			
		}

		private void EPDataGrid_RowDeleted(object sender, DataRowChangeEventArgs e)
		{
			try
			{
                if (_query != null && _query.Adapter != null)
                {
                    _query.Adapter.Update(_query);
                }
			}
			catch(System.Data.DBConcurrencyException)
			{
				Requery();
			}
			catch(Exception er)
			{
				System.Data.DBConcurrencyException err = er as System.Data.DBConcurrencyException;
				if(err != null)
				{
				}
				FormLog.NotifyException(er);
			}
		}

		//		private void EPDataGrid_RowChanged(object sender, DataRowChangeEventArgs e)
		//		{
		//			//			if( e.Action == System.Data.DataRowAction.Add )
		//			//			{
		//			//				for(int i=0;i<fields.Count;i++)
		//			//				{
		//			//					if( fields[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp )
		//			//					{
		//			//						e.Row[i] = System.DateTime.Now;
		//			//					}
		//			//				}
		//			//			}
		//		}

		private void EPDataGrid_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
            if (_query != null)
            {
                if (_query.Fields[e.Column.Ordinal].OleDbType != System.Data.OleDb.OleDbType.DBTimeStamp)
                {
                    for (int i = 0; i < _query.Fields.Count; i++)
                    {
                        if (_query.Fields[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp)
                        {
                            e.Row[i] = System.DateTime.Now;
                        }
                    }
                }
            }
		}

		private void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
//			if(e.ListChangedType == System.ComponentModel.ListChangedType.Reset)
//			{
				onRowIndexChanged();
//			}
		}
		protected virtual void OnFireDoubleClick(EventArgs e)
		{
		}
		private void TextBox_DoubleClick(object sender, EventArgs e)
		{
			OnFireDoubleClick(e);
		}
		protected virtual void OnFireClick(EventArgs e)
		{
		}
		private void TextBox_Click(object sender, EventArgs e)
		{
			uint n = NativeWIN32.GetTickCount();
			if(Math.Abs(n-_lastTableClick)>500)
			{
				_lastTableClick = n;
				OnFireClick(e);
			}
		}
		protected virtual void OnFireGotFocus(EventArgs e)
		{
		}
//		FRMdebugTrace trace = null;
		private void TextBox_GotFocus(object sender, EventArgs e)
		{
			OnFireGotFocus(e);
			uint n = NativeWIN32.GetTickCount();
			if(Math.Abs(n-_lastTableClick)>500)
			{
				System.Drawing.Point curPoint = this.PointToClient( System.Windows.Forms.Cursor.Position);
				Rectangle rc = this.GetCellBounds(this.CurrentCell);
//				if(clsAppGlobals.RunMode)
//				{
//					if(trace == null)
//					{
//						trace = new FRMdebugTrace();
//						trace.Show();
//					}
//					trace.addTrace(curPoint.ToString()+"  "+rc.ToString()+"  "+rc.Contains(curPoint).ToString());
//				}
				if(rc.Contains(curPoint))
				{
					_lastTableClick = n;
					OnFireClick(e);
				}
			}
		}
	}
}
