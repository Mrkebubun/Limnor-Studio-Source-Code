/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using VPL;
using System.Collections.Generic;
using System.Globalization;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgPropSQLNonQuery.
	/// </summary>
	public class dlgPropSQLNonQuery : Form
	{
		enum EnumEditState { NotLoaded, Loading, Ready, WhereTextChanged, ValueTextChanged, ValueCellChanged, SqlTextChanged, TableChanged, CommandTypeChanged, FieldSelectionChanged, CheckSyntaxClick, OkClick }
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox lstTable;
		private System.Windows.Forms.DataGridView dataGrid1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.TextBox txtWhere;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btQueryBuilder;
		private System.ComponentModel.IContainer components;
		//
		public SQLNoneQuery objRet = null;
		private ConnectionItem dbConn = null;
		DatabaseTable dt = null;
		private FieldList _currentFields;
		//
		private System.Windows.Forms.TextBox txtSQL;
		//
		DataSet ds = null;
		EnumEditState editState = EnumEditState.NotLoaded;

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button btChkSyntax;
		int nCurRow = -1;
		private System.Windows.Forms.TextBox txtInfo;
		bool bSQLChanged = false;
		private TableAliasList _tables;
		//
		private Dictionary<enmNonQueryType, Dictionary<string, SQLNoneQuery>> _commandCache;
		//
		public dlgPropSQLNonQuery()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			cbxType.Items.Add(enmNonQueryType.Insert);
			cbxType.Items.Add(enmNonQueryType.Update);
			cbxType.Items.Add(enmNonQueryType.Delete);
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.label1 = new System.Windows.Forms.Label();
			this.cbxType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lstTable = new System.Windows.Forms.ListBox();
			this.dataGrid1 = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtWhere = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.txtSQL = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btQueryBuilder = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.btChkSyntax = new System.Windows.Forms.Button();
			this.txtInfo = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 13);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Command type:";
			// 
			// cbxType
			// 
			this.cbxType.Location = new System.Drawing.Point(13, 35);
			this.cbxType.Name = "cbxType";
			this.cbxType.Size = new System.Drawing.Size(426, 21);
			this.cbxType.TabIndex = 1;
			this.cbxType.SelectedIndexChanged += new System.EventHandler(this.cbxType_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(460, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "Table name:";
			// 
			// lstTable
			// 
			this.lstTable.Location = new System.Drawing.Point(460, 35);
			this.lstTable.Name = "lstTable";
			this.lstTable.Size = new System.Drawing.Size(197, 69);
			this.lstTable.TabIndex = 3;
			this.lstTable.SelectedIndexChanged += new System.EventHandler(this.lstTable_SelectedIndexChanged);
			// 
			// dataGrid1
			// 
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGrid1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGrid1.ColumnHeadersVisible = false;
			this.dataGrid1.Location = new System.Drawing.Point(11, 94);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(430, 155);
			this.dataGrid1.TabIndex = 4;
			this.dataGrid1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid1_CellValueChanged);
			this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(458, 113);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(201, 13);
			this.label3.TabIndex = 5;
			this.label3.Tag = "4";
			this.label3.Text = "Value to be set to the field:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 70);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(231, 23);
			this.label4.TabIndex = 6;
			this.label4.Tag = "3";
			this.label4.Text = "Select fields to be changed:";
			// 
			// txtValue
			// 
			this.txtValue.Location = new System.Drawing.Point(460, 134);
			this.txtValue.Multiline = true;
			this.txtValue.Name = "txtValue";
			this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtValue.Size = new System.Drawing.Size(197, 54);
			this.txtValue.TabIndex = 7;
			this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(457, 336);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(212, 50);
			this.label5.TabIndex = 8;
			this.label5.Tag = "8";
			this.label5.Text = "Use @<parameter name> to include a parameter in the value or filter. For example," +
				" @Notes.";
			// 
			// txtWhere
			// 
			this.txtWhere.Location = new System.Drawing.Point(460, 216);
			this.txtWhere.Multiline = true;
			this.txtWhere.Name = "txtWhere";
			this.txtWhere.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtWhere.Size = new System.Drawing.Size(197, 63);
			this.txtWhere.TabIndex = 10;
			this.txtWhere.TextChanged += new System.EventHandler(this.txtWhere_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(460, 197);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(201, 13);
			this.label6.TabIndex = 9;
			this.label6.Tag = "5";
			this.label6.Text = "Filter (FROM/WHERE clause):";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(483, 393);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 11;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(570, 393);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "Cancel";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 258);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(235, 16);
			this.label7.TabIndex = 13;
			this.label7.Tag = "6";
			this.label7.Text = "Database command (SQL statement):";
			// 
			// txtSQL
			// 
			this.txtSQL.Location = new System.Drawing.Point(10, 280);
			this.txtSQL.Multiline = true;
			this.txtSQL.Name = "txtSQL";
			this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSQL.Size = new System.Drawing.Size(433, 94);
			this.txtSQL.TabIndex = 14;
			this.txtSQL.TextChanged += new System.EventHandler(this.txtSQL_TextChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(460, 289);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(207, 47);
			this.label8.TabIndex = 15;
			this.label8.Tag = "7";
			this.label8.Text = "Use single quotes to include string literals in the value the filter, for example" +
				", \'English\'";
			// 
			// btQueryBuilder
			// 
			this.btQueryBuilder.Location = new System.Drawing.Point(332, 253);
			this.btQueryBuilder.Name = "btQueryBuilder";
			this.btQueryBuilder.Size = new System.Drawing.Size(109, 23);
			this.btQueryBuilder.TabIndex = 16;
			this.btQueryBuilder.Tag = "9";
			this.btQueryBuilder.Text = "Filter builder";
			this.btQueryBuilder.Click += new System.EventHandler(this.btQueryBuilder_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 200;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// btChkSyntax
			// 
			this.btChkSyntax.Location = new System.Drawing.Point(221, 253);
			this.btChkSyntax.Name = "btChkSyntax";
			this.btChkSyntax.Size = new System.Drawing.Size(107, 23);
			this.btChkSyntax.TabIndex = 17;
			this.btChkSyntax.Tag = "10";
			this.btChkSyntax.Text = "Check syntax";
			this.btChkSyntax.Click += new System.EventHandler(this.btChkSyntax_Click);
			// 
			// txtInfo
			// 
			this.txtInfo.Location = new System.Drawing.Point(10, 378);
			this.txtInfo.Multiline = true;
			this.txtInfo.Name = "txtInfo";
			this.txtInfo.ReadOnly = true;
			this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtInfo.Size = new System.Drawing.Size(433, 44);
			this.txtInfo.TabIndex = 18;
			// 
			// dlgPropSQLNonQuery
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(675, 429);
			this.Controls.Add(this.txtInfo);
			this.Controls.Add(this.btChkSyntax);
			this.Controls.Add(this.btQueryBuilder);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtSQL);
			this.Controls.Add(this.txtWhere);
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.lstTable);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbxType);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropSQLNonQuery";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database command builder";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		/// <summary>
		/// load from outside
		/// </summary>
		/// <param name="sql"></param>
		public void LoadData(SQLNoneQuery sql)
		{
			if (editState != EnumEditState.NotLoaded)
			{
				throw new ExceptionLimnorDatabase("Calling LoadData at the state of {0}", editState);
			}

			dbConn = sql.Connection;
			//load table names into lstTable
			QueryParser._UseLowerCase = sql.Connection.ConnectionObject.LowerCaseSqlKeywords;
			System.Data.DataTable tbl = sql.Connection.ConnectionObject.GetTables();
			if (tbl != null)
			{
				for (int i = 0; i < tbl.Rows.Count; i++)
				{
					if (tbl.Rows[i]["Table_Name"] != null)
					{
						string s = tbl.Rows[i]["Table_Name"].ToString();
						int n = lstTable.Items.Add(s);
						if (string.Compare(s, sql.TableName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							lstTable.SelectedIndex = n;
						}
					}
				}
			}
			objRet = ((SQLNoneQuery)sql.Clone());

			int k = (int)(objRet.CommandType);
			if (k >= 0 && k < cbxType.Items.Count)
				cbxType.SelectedIndex = k;

			txtWhere.Text = objRet.Filter;
			txtSQL.Text = objRet.SQL;
			btQueryBuilder.Enabled = (objRet.CommandType != enmNonQueryType.Insert);

			if (!string.IsNullOrEmpty(objRet.TableName))
			{
				for (int i = 0; i < lstTable.Items.Count; i++)
				{
					if (string.Compare(objRet.TableName, lstTable.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						lstTable.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				if (lstTable.Items.Count > 0)
				{
					lstTable.SelectedIndex = 0;
				}
			}
			editState = EnumEditState.Ready;
			lstTable_SelectedIndexChanged(this, null);
		}
		private void reloadFields()
		{
			if (editState == EnumEditState.Ready)
			{
				editState = EnumEditState.Loading;
				if (ds != null)
				{
					try
					{
						if (ds.Tables.Count > 0)
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								EPField fld;
								FieldList flds = new FieldList();
								for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
								{
									if (VPLUtil.ObjectToBool(ds.Tables[0].Rows[i][1]))
									{
										fld = new EPField();
										fld.Name = ds.Tables[0].Rows[i][0].ToString();
										if (ds.Tables[0].Rows[i][2] != null && ds.Tables[0].Rows[i][2] != System.DBNull.Value)
										{
											fld.FieldText = ds.Tables[0].Rows[i][2].ToString();
										}
										fld.FromTableName = ds.Tables[0].TableName;
										flds.AddField(fld);
									}
								}
								objRet.SetFields(flds);
								showSQL();
							}
						}
					}
					catch (Exception er)
					{
						FormLog.NotifyException(true, er);
					}
				}
				editState = EnumEditState.Ready;
			}
		}
		private void saveTocCache()
		{
			if (_commandCache == null)
			{
				_commandCache = new Dictionary<enmNonQueryType, Dictionary<string, SQLNoneQuery>>();
			}
			Dictionary<string, SQLNoneQuery> c;
			if (!_commandCache.TryGetValue(objRet.CommandType, out c))
			{
				c = new Dictionary<string, SQLNoneQuery>();
				_commandCache.Add(objRet.CommandType, c);
			}
			if (c.ContainsKey(objRet.TableName))
			{
				c[objRet.TableName] = objRet;
			}
			else
			{
				c.Add(objRet.TableName, objRet);
			}
		}
		/// <summary>
		/// load SQLNoneQuery from cache
		/// </summary>
		/// <returns>true:loaded objRet from cache; false current objRet is already good</returns>
		private bool loadFromCache()
		{
			enmNonQueryType qt = enmNonQueryType.Insert;
			if (cbxType.SelectedIndex >= 0)
			{
				qt = (enmNonQueryType)(cbxType.SelectedIndex);
			}
			if (dt != null)
			{
				if (objRet != null)
				{
					if (objRet.CommandType == qt && string.Compare(objRet.TableName, dt.TableName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						saveTocCache();
						return false;
					}
				}
				SQLNoneQuery nq = null;

				if (_commandCache != null)
				{
					Dictionary<string, SQLNoneQuery> c;
					if (_commandCache.TryGetValue(qt, out c))
					{
						c.TryGetValue(dt.TableName, out nq);
					}
				}

				if (nq == null)
				{
					nq = new SQLNoneQuery();
					nq.TableName = dt.TableName;
					nq.CommandType = qt;
					objRet = nq;
					saveTocCache();
				}
				else
				{
					objRet = nq;
				}
			}
			return true;
		}
		private string adjustFilter(string s)
		{
			const string WHERE = "WHERE ";
			if (!string.IsNullOrEmpty(s))
			{
				s = s.Trim();
				while (s.StartsWith(WHERE, StringComparison.OrdinalIgnoreCase))
				{
					s = s.Substring(WHERE.Length).Trim();
				}
			}
			return s;
		}
		private void setFieldTypes()
		{
			int n = objRet.FieldCount;
			if (n > 0 && _currentFields != null)
			{
				for (int i = 0; i < n; i++)
				{
					EPField f = objRet.GetField(i);
					EPField p = _currentFields[f.Name];
					if (p != null)
					{
						f.OleDbType = p.OleDbType;
					}
				}
				int m = objRet.ParamCount;
				if (m > 0)
				{
					for (int i = 0; i < m; i++)
					{
						EPField f0 = null;
						EPField p = objRet.Parameters[i];
						for (int j = 0; j < n; j++)
						{
							EPField f = objRet.GetField(j);
							if (!string.IsNullOrEmpty(f.FieldText))
							{
								if (string.Compare(f.FieldText, p.Name, StringComparison.OrdinalIgnoreCase) == 0)
								{
									f0 = f;
									break;
								}
								if (f0 == null)
								{
									if (f.FieldText.Contains(f.Name))
									{
										f0 = f;
									}
								}
							}
						}
						if (f0 != null)
						{
							p.OleDbType = f0.OleDbType;
						}
					}
				}
			}
		}
		private void showSQL()
		{
			EnumEditState st = editState;
			editState = EnumEditState.Loading;
			//
			loadFromCache();
			//
			objRet.SetConnection(dbConn);
			//
			if (ds != null && ds.Tables.Count > 0)
			{
				for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
				{
					ds.Tables[0].Rows[j][1] = false;
					ds.Tables[0].Rows[j][2] = string.Empty;
				}
				//
				bool b2 = bSQLChanged;
				bSQLChanged = true;
				txtWhere.Text = objRet.Filter;
				txtSQL.Text = objRet.SQL;
				if (checkSyntax())
				{
					objRet.SetFilter(adjustFilter(objRet.Filter));
					txtWhere.Text = objRet.Filter;
					txtSQL.Text = objRet.SQL;
					txtWhere.SelectionStart = txtWhere.Text.Length;
					txtSQL.SelectionStart = txtSQL.Text.Length;
					btQueryBuilder.Enabled = (objRet.CommandType != enmNonQueryType.Insert);
					bSQLChanged = b2;
					txtSQL.ForeColor = Color.Black;
				}
			}
			editState = st;
		}
		private void lstTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.TableChanged;
			try
			{
				int n = lstTable.SelectedIndex;
				if (n >= 0)
				{
					//reload Field Grid ======================================
					//
					dt = new DatabaseTable();
					dt.TableName = lstTable.Items[n].ToString();
					//
					loadFromCache();
					//
					ds = new System.Data.DataSet("Fields");
					ds.Tables.Add(dt.TableName);
					ds.Tables[0].Columns.Add();
					ds.Tables[0].Columns.Add();
					ds.Tables[0].Columns.Add();
					//
					ds.Tables[0].Columns[0].Caption = "FieldName";
					ds.Tables[0].Columns[0].ColumnName = "FieldName";
					ds.Tables[0].Columns[0].DataType = typeof(string);
					ds.Tables[0].Columns[0].MaxLength = 30;
					ds.Tables[0].Columns[0].ReadOnly = true;
					//
					ds.Tables[0].Columns[1].Caption = "Include";
					ds.Tables[0].Columns[1].ColumnName = "Include";
					ds.Tables[0].Columns[1].DataType = typeof(bool);
					//
					ds.Tables[0].Columns[2].Caption = "Value";
					ds.Tables[0].Columns[2].ColumnName = "Value";
					ds.Tables[0].Columns[2].DataType = typeof(string);
					ds.Tables[0].Columns[2].MaxLength = 80;
					//
					dataGrid1.AllowUserToOrderColumns = false;
					//
					dt.GetFields(dbConn.ConnectionObject);
					int count = dt.FieldCount;
					object[] vs;
					EPField fld, fld2;
					_currentFields = new FieldList();
					for (int i = 0; i < count; i++)
					{
						fld = dt.GetField(i);
						_currentFields.AddField(fld);
						vs = new object[3];
						vs[0] = fld.Name;
						fld2 = objRet.GetField(fld.Name);
						if (fld2 != null)
						{
							fld2.OleDbType = fld.OleDbType;
							vs[1] = true;
							vs[2] = fld2.FieldText;
						}
						else
						{
							vs[1] = false;
							vs[2] = "";
						}
						ds.Tables[0].Rows.Add(vs);
					}
					//
					ds.Tables[0].ColumnChanged += new System.Data.DataColumnChangeEventHandler(dlgPropSQLNonQuery_ColumnChanged);
					ds.Tables[0].ColumnChanging += new System.Data.DataColumnChangeEventHandler(dlgPropSQLNonQuery_ColumnChanging);
					//
					dataGrid1.DataSource = null;
					dataGrid1.Columns.Clear();
					dataGrid1.AutoGenerateColumns = true;
					dataGrid1.DataSource = ds;
					dataGrid1.DataMember = dt.TableName;
					dataGrid1.AllowUserToOrderColumns = false;
					dataGrid1.AllowUserToAddRows = false;
					dataGrid1.AllowUserToDeleteRows = false;
					dataGrid1.AllowUserToResizeRows = false;
					dataGrid1.AllowUserToResizeColumns = true;
					dataGrid1.ColumnHeadersVisible = true;

					//
					dataGrid1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
					//
					dataGrid1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
					dataGrid1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
					dataGrid1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
					//
					txtValue.Text = "";
					//
					if (string.Compare(objRet.TableName, dt.TableName, StringComparison.OrdinalIgnoreCase) != 0)
					{
						saveTocCache();
					}
					showSQL();
				}
			}
			finally
			{
				editState = EnumEditState.Ready;
			}
		}

		private void cbxType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.CommandTypeChanged;
			try
			{
				if (cbxType.SelectedIndex >= 0)
				{
					enmNonQueryType qt = (enmNonQueryType)cbxType.SelectedIndex;
					if (objRet.CommandType != qt)
					{
						saveTocCache();
						showSQL();
					}
					btQueryBuilder.Enabled = (qt != enmNonQueryType.Insert);
				}
			}
			finally
			{
				editState = EnumEditState.Ready;
			}
		}

		private void txtValue_TextChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.ValueTextChanged;
			try
			{
				int n = dataGrid1.CurrentCell.RowIndex;
				if (n >= 0 && ds != null && ds.Tables.Count > 0 && n < ds.Tables[0].Rows.Count)
				{
					string fn = (string)(dataGrid1.CurrentRow.Cells[0].Value);
					EPField fld = objRet.GetField(fn);
					if (fld != null)
					{
						fld.FieldText = txtValue.Text;
						ds.Tables[0].Rows[n][2] = txtValue.Text;
						objRet.SQL = string.Empty;
						txtSQL.Text = objRet.SQL;
					}
				}
			}
			catch (Exception er)
			{
				FormLog.NotifyException(true, er);
			}
			finally
			{
				editState = EnumEditState.Ready;
			}
		}

		private void txtWhere_TextChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.WhereTextChanged;
			string filter = adjustFilter(txtWhere.Text);
			objRet.SetFilter(filter);
			objRet.ResetSQL();
			txtSQL.Text = objRet.SQL;
			editState = EnumEditState.Ready;
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.FieldSelectionChanged;

			if (dataGrid1.CurrentCell != null && dataGrid1.CurrentCell.RowIndex != nCurRow)
			{
				nCurRow = dataGrid1.CurrentCell.RowIndex;
				if (nCurRow >= 0)
				{
					if (ds.Tables[0].Rows[nCurRow][2] != null)
						txtValue.Text = ds.Tables[0].Rows[nCurRow][2].ToString();
					else
						txtValue.Text = "";
				}
			}
			editState = EnumEditState.Ready;
		}
		private void dlgPropSQLNonQuery_ColumnChanging(object sender, System.Data.DataColumnChangeEventArgs e)
		{
		}
		private void dlgPropSQLNonQuery_ColumnChanged(object sender, System.Data.DataColumnChangeEventArgs e)
		{
		}
		protected void acceptByMouse()
		{
			if (dataGrid1.IsCurrentCellDirty)
			{
				dataGrid1.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
			reloadFields();
		}
		private void btQueryBuilder_Click(object sender, System.EventArgs e)
		{
			acceptByMouse();
			EasyQuery qry = objRet.SourceQuery;
			if (qry == null)
			{
				qry = new EasyQuery();
				qry.DatabaseConnection = dbConn;
				if (_tables == null)
				{

					_tables = new TableAliasList();
					for (int i = 0; i < lstTable.Items.Count; i++)
					{
						TableAlias t = new TableAlias((string)(lstTable.Items[i]), string.Empty);
						_tables.AddTable(t);
					}
				}
				qry.T = _tables;
				FieldList lst = objRet.GetFields();
				if (lst != null)
				{
					FieldList fl = (FieldList)lst.Clone();
					for (int i = 0; i < fl.Count; i++)
					{
						fl[i].FieldText = fl[i].Name;
						if (lst[i].FromTableName != null)
						{
							if (lst[i].FromTableName.Length > 0)
							{
								if (!qry.T.IsTableIncluded(lst[i].FromTableName))
								{
									TableAlias t = new TableAlias(lst[i].FromTableName, "");
									t.srcType = enumRecSource.Table;
									qry.T.AddTable(t);
								}
							}
						}
					}
					qry.Fields = fl;
				}
				string sl = objRet.Filter;
				if (sl == null)
					sl = "";
				if (sl.Length > 0)
				{
					if (sl.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase))
					{
						sl = sl.Substring(5);
						sl = sl.Trim();
						qry.From = sl;
					}
					else
					{
						qry.From = objRet.TableName;
						if (sl.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
						{
							sl = sl.Substring(6);
							sl = sl.Trim();
						}
						qry.Where = sl;
					}
				}
				else
				{
					qry.From = objRet.TableName;
				}
			}
			qry.DatabaseConnection = dbConn;
			QueryParser qp = new QueryParser();
			if (qp.BuildQuery(qry, this))
			{
				string filter = "";
				bool bOK = false;
				objRet.SourceQuery = qp.query;
				qry = qp.query;
				if (!qry.Fields.FromSingleTable() || objRet.CommandType == enmNonQueryType.Delete)
				{
					filter = qry.From;
					if (filter.Length > 0)
					{
						filter = string.Format(CultureInfo.InvariantCulture, "{0}{1}", QueryParser.SQL_From(), filter);
					}
				}
				if (qry.Where.Length > 0)
				{
					filter = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", filter, QueryParser.SQL_Where(), qry.Where);
				}
				if (objRet.CommandType == enmNonQueryType.Delete)
				{
					if (qry.From.IndexOf(objRet.TableName, StringComparison.OrdinalIgnoreCase) < 0)
					{
						MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Table [{0}] not found in [{1}]", objRet.TableName, qry.From), this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					}
					else
					{
						objRet.MultiRow = true;
						txtWhere.Text = filter;
						objRet.SetFilter(filter);
						bOK = true;
					}
				}
				else
				{
					filter = adjustFilter(filter);
					txtWhere.Text = filter;
					objRet.SetFilter(filter);
					objRet.ResetSQL();
					bOK = true;
				}
				if (bOK)
				{
					showSQL();
				}
			}
		}
		private void txtSQL_TextChanged(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.SqlTextChanged;

			try
			{
				txtSQL.ForeColor = System.Drawing.Color.Red;
				bSQLChanged = true;
			}
			finally
			{
				editState = EnumEditState.Ready;
			}
		}
		bool parseSQLNoneQuery(string sSQL, SQLNoneQuery sql)
		{
			string sMsg;
			if (string.IsNullOrEmpty(sql.TableName) && objRet != null)
			{
				sql.TableName = objRet.TableName;
			}
			sql.SetConnection(dbConn);
			bool b = sql.parseSQLNoneQuery(this, sSQL, out sMsg);
			txtInfo.Text = sMsg;
			return b;
		}


		private void timer1_Tick(object sender, System.EventArgs e)
		{
			timer1.Enabled = false;
			reloadFields();
		}
		private bool checkSyntax()
		{
			if (bSQLChanged)
			{
				QueryParser._UseLowerCase = dbConn.LowerCaseSqlKeywords;
				SQLNoneQuery sql = new SQLNoneQuery();
				sql.SetConnection(dbConn);
				ParameterList ps = objRet.ParseParams();
				if (ps != null)
				{
					sql.SetParameters(ps);
				}
				if (parseSQLNoneQuery(txtSQL.Text, sql))
				{
					string tblName = objRet.TableName;
					objRet = sql;
					if (string.IsNullOrEmpty(objRet.TableName))
					{
						objRet.TableName = tblName;
					}

					enmNonQueryType qt = enmNonQueryType.Insert;
					if (cbxType.SelectedIndex >= 0)
					{
						qt = (enmNonQueryType)(cbxType.SelectedIndex);
					}
					if (qt != objRet.CommandType)
					{
						cbxType.SelectedIndex = (int)qt;
					}
					for (int i = 0; i < lstTable.Items.Count; i++)
					{
						if (string.Compare(objRet.TableName, lstTable.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							if (i != lstTable.SelectedIndex)
							{
								lstTable.SelectedIndex = i;
								EnumEditState st = editState;
								editState = EnumEditState.Ready;
								lstTable_SelectedIndexChanged(this, EventArgs.Empty);
								editState = st;
							}
							break;
						}
					}
					setFieldTypes();
					saveTocCache();
					if (ds != null)
					{
						for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
						{
							ds.Tables[0].Rows[j][1] = false;
							ds.Tables[0].Rows[j][2] = "";
						}
						for (int i = 0; i < objRet.FieldCount; i++)
						{
							EPField f = objRet.GetField(i);
							if (f != null)
							{
								for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
								{
									string fn = ds.Tables[0].Rows[j][0].ToString();
									if (string.Compare(f.Name, fn, StringComparison.OrdinalIgnoreCase) == 0)
									{
										ds.Tables[0].Rows[j][1] = true;
										ds.Tables[0].Rows[j][2] = f.FieldText;
										break;
									}
								}
							}
						}
					}
					return true;
				}
				else
				{
					txtSQL.ForeColor = System.Drawing.Color.Red;
					return false;
				}
			}
			return true;
		}

		private void btChkSyntax_Click(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.CheckSyntaxClick;
			if (bSQLChanged && checkSyntax())
			{
				txtSQL.ForeColor = System.Drawing.Color.Black;
			}
			editState = EnumEditState.Ready;
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.OkClick;
			bSQLChanged = true;
			if (checkSyntax())
			{
				if (objRet.ParamCount > 0)
				{
					DlgParameters dlg = new DlgParameters();
					dlg.LoadData(objRet.Parameters);
					dlg.ShowDialog(this);
				}
			}
			else
			{
				MessageBox.Show(txtInfo.Text);
			}

			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void dataGrid1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (editState != EnumEditState.Ready)
				return;
			editState = EnumEditState.ValueCellChanged;
			try
			{
				if (e.ColumnIndex == 1)
				{
					bool b = (bool)(dataGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
					string fn = (string)(dataGrid1.Rows[e.RowIndex].Cells[0].Value);
					if (b)
					{
						EPField f = objRet.GetField(fn);
						if (f == null)
						{
							EPField f0 = _currentFields[fn];
							if (f0 != null)
							{
								f = (EPField)f0.Clone();
								objRet.AddField(f);
							}
						}
					}
					else
					{
						objRet.RemoveField(fn);
					}
					objRet.SQL = string.Empty;
					txtSQL.Text = objRet.SQL;
				}
				else if (e.ColumnIndex == 2)
				{
					string fn = (string)(dataGrid1.Rows[e.RowIndex].Cells[0].Value);
					string val = (string)(dataGrid1.Rows[e.RowIndex].Cells[2].Value);
					EPField f = objRet.GetField(fn);
					if (f != null)
					{
						f.FieldText = val;
						objRet.SQL = string.Empty;
						txtSQL.Text = objRet.SQL;
					}
				}
			}
			finally
			{
				editState = EnumEditState.Ready;
			}
		}
	}
}
