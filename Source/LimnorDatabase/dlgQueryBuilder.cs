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
using System.Data.Common;
using System.Globalization;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgQueryBuilder.
	/// </summary>
	public class dlgQueryBuilder : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeView1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtSELECT;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblDatabase;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDel;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btLeft;
		private System.Windows.Forms.Button btRight;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btCompile;
		private System.Windows.Forms.Button btResetSelect;
		//
		DatabaseSchema schema = null;
		System.Data.DataSet ds = null;
		Connection _cnn = null;

		private bool _loadingText;

		QueryParser qParser = null;
		private string Sep1 = "[", Sep2 = "]";
		private System.Windows.Forms.Button byGroupBy;
		private System.Windows.Forms.Button btOrderBy;
		private System.Windows.Forms.Button btWhere;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.CheckBox chkSQL;
		private System.Windows.Forms.Label lblSQL;
		private System.Windows.Forms.Button btParam;
		bool bSELECTchanged = true;
		private EnumTopRecStyle _topRecStyle;
		//
		private System.Windows.Forms.Button btDB;
		private System.Windows.Forms.TextBox txtQueryName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtTop;
		private DataGridView dataGridView1;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private System.Windows.Forms.ComboBox cbbFields;
		//
		public dlgQueryBuilder()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//

		}
		/// <summary>
		/// qp.query cannot be null because it carries the connection string
		/// </summary>
		/// <param name="qp"></param>
		/// <returns></returns>
		public bool LoadData(QueryParser qp)
		{
			bool bRet = false;
			string sMsg = "";
			Application.DoEvents();
			try
			{
				qParser = qp;
				txtQueryName.Text = qp.query.Description;

				Sep1 = qp.Sep1;
				Sep2 = qp.Sep2;
				QueryParser._UseLowerCase = qp.query.DatabaseConnection.ConnectionObject.LowerCaseSqlKeywords;
				_topRecStyle = qp.query.DatabaseConnection.TopRecordStyle;
				if (_topRecStyle == EnumTopRecStyle.NotAllowed)
				{
					txtTop.Text = "0";
					txtTop.Enabled = false;
				}
				ReloadSchema(ref sMsg);
				bRet = true;
			}
			catch (Exception er)
			{
				sMsg = ExceptionLimnorDatabase.FormExceptionText(er);
			}
			if (sMsg.Length > 0)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error loading query builder. {0}", sMsg), this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
			this.setQueryState(qParser.query.QueryMade());
			chkSQL.Checked = (qParser.query.DatabaseConnection.ConnectionObject.IsOdbc);
			showSQL();
			return bRet;
		}
		void ReloadSchema(ref string sMsg)
		{
			lblDatabase.Text = qParser.query.DatabaseDisplayText;
			qParser.LoadSchema();
			schema = qParser.GetSchema();
			loadSchema();
			if (applyQuery(qParser.query, ref sMsg))
			{
			}
		}
		void loadSchema()
		{
			int i;
			TreeNode nd, ndRoot;
			treeView1.Nodes.Clear();
			ndRoot = treeView1.Nodes.Add("Tables");
			ndRoot.Tag = enumRecSource.Table;
			for (i = 0; i < schema.TableCount; i++)
			{
				nd = ndRoot.Nodes.Add(schema.Tables[i].TableName);
				nd.Tag = schema.Tables[i];
				TreeNode ndSub = nd.Nodes.Add("");
				ndSub.Tag = 1;
			}
			ndRoot = treeView1.Nodes.Add("Views");
			ndRoot.Tag = enumRecSource.View;
			for (i = 0; i < schema.ViewCount; i++)
			{
				nd = ndRoot.Nodes.Add(schema.Views[i].ViewName);
				nd.Tag = schema.Views[i];
				TreeNode ndSub = nd.Nodes.Add("");
				ndSub.Tag = 2;
			}
		}
		void applyQuery()
		{
			string sMsg = "";
			if (!applyQuery(qParser.query, ref sMsg))
			{
				MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
		}
		bool applyQuery(EasyQuery qry, ref string sMsg)
		{
			cbbFields.Items.Clear();
			bool bRet = false;
			FieldList fl = qry.Fields;
			if (fl == null)
				return false;
			if (fl.Count == 0)
				return false;
			for (int i = 0; i < fl.Count; i++)
			{
				cbbFields.Items.Add(fl[i]);
			}
			cbbFields.SelectedIndex = 0;
			try
			{
				qry.bQueryOK = false;
				int nTop = qry.Top;
				if (_topRecStyle == EnumTopRecStyle.NotAllowed)
				{
					if (string.IsNullOrEmpty(qry.Where))
					{
						qry.Where = "1=2";
						qry.IsFilterInsertedByQueryBuilder = true;
					}
					else
					{
						qry.IsFilterInsertedByQueryBuilder = false;
					}
				}
				else
				{
					if (nTop == 0)
					{
						try
						{
							nTop = Convert.ToInt32(txtTop.Text);
						}
						catch
						{
							nTop = 10;
						}
						qry.SampleTopRec = nTop;
					}
				}
				qParser.GetSchema();
				qry.UseSampleTopRec = true;
				DbCommand cmd = qry.MakeCommand(qry.Parameters.Clone() as ParameterList);
				qry.UseSampleTopRec = false;
				if (qry.IsFilterInsertedByQueryBuilder)
				{
					qry.Where = string.Empty;
				}
				string sSQL = qry.SQL.GetSQL();
				_loadingText = true;
				txtSELECT.Text = sSQL;
				_loadingText = false;
				dataGridView1.DataBindings.Clear();
				using (DbDataAdapter _da = DataAdapterFinder.CreateDataAdapter(cmd))
				{
					if (_da == null)
					{
						MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Cannot create data adapter from SQL [{0}]", cmd.CommandText), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						ds = new System.Data.DataSet("QB");
						int npCount = cmd.Parameters.Count;
						if (npCount > 0)
						{
							EnumParameterStyle pstyle = qry.ParameterStyle;
							ParameterList ps = null;
							if (pstyle != EnumParameterStyle.QuestionMark)
							{
								ps = qry.Parameters;
							}
							for (int i = 0; i < npCount; i++)
							{
								EPField f;
								if (pstyle == EnumParameterStyle.QuestionMark)
								{
									f = qry.GetMappedParameter(i);
								}
								else
								{
									f = ps[i];
								}
								if (f.Value == null)
								{
									f.SetValue(EPField.DefaultFieldValue(f.OleDbType));
								}
								cmd.Parameters[i].Value = f.Value;
							}
						}
						dataGridView1.DataBindings.Clear();
						dataGridView1.DataSource = null;
						dataGridView1.DataMember = null;
						_da.Fill(ds);
					}
				}
				dataGridView1.DataSource = ds;
				dataGridView1.DataMember = ds.Tables[0].TableName;
				for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
				{
					dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
					if (ds.Tables[0].Columns[i].DataType.Equals(typeof(System.DateTime)))
					{
						dataGridView1.Columns[i].HeaderText = ds.Tables[0].Columns[i].ColumnName;
						dataGridView1.Columns[i].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss";
						dataGridView1.Columns[i].DefaultCellStyle.NullValue = string.Empty;
					}
				}
				txtSELECT.ForeColor = System.Drawing.Color.Black;
				btResetSelect.Enabled = false;
				btCompile.Enabled = false;
				bSELECTchanged = false;
				setQueryState(true);
				qry.bQueryOK = true;
				//
				dataGrid1_CurrentCellChanged(null, null);
				bRet = true;
			}
			catch (Exception er)
			{
				sMsg = er.Message;
				txtSELECT.ForeColor = System.Drawing.Color.Blue;
				bSELECTchanged = true;
				setQueryState(false);
				showSQL();
			}
			finally
			{
			}
			return bRet;
		}
		bool CheckAndCompile()
		{
			if (bSELECTchanged)
			{
				//===============================================
				string sSelect = txtSELECT.Text.Trim();
				//===============================================
				btOK.Enabled = false;
				//===============================================
				if (string.IsNullOrEmpty(sSelect))
				{
					dataGridView1.DataBindings.Clear();
					dataGridView1.Refresh();
					qParser.Clear();
					return false;
				}
				//===============================================
				bool bRet = false;
				string sMsg = "";
				frmQryMsg frm = new frmQryMsg();
				frm.TopLevel = false;
				frm.SetMsg("Compiling SQL statement, please wait......");
				frm.Parent = this;
				frm.Show();
				frm.Location = new System.Drawing.Point((this.Width - frm.Width) / 2, (this.Height - frm.Height) / 2);
				Application.DoEvents();
				this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
				//========================================
				EasyQuery qry = null;
				try
				{
					int nTop = getTopN();
					qParser.SetSampleTopN(nTop);
					qry = qParser.ParseQuery(this, sSelect, out sMsg);
				}
				catch (Exception err)
				{
					qry = null;
					if (string.IsNullOrEmpty(sMsg))
						sMsg = err.Message;
					else
						sMsg = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", sMsg, err.Message);
				}
				if (qry != null)
				{
					qry.DatabaseConnection = qParser.query.DatabaseConnection;
					if (applyQuery(qry, ref sMsg))
					{
						if (!string.IsNullOrEmpty(qry.UpdatableTableName))
						{
							if (qry.HasRowID())
							{
								DatabaseTable tbl = schema.FindTable(qry.UpdatableTableName);
								if (tbl != null)
								{
									FieldList fl = qry.Fields;
									for (int i = 0; i < fl.Count; i++)
									{
										if (tbl.FindField(fl[i].Name) == null)
										{
											fl[i].ReadOnly = true;
										}
									}
								}
							}
						}
						qry.Description = txtQueryName.Text;
						qParser.query = qry;
						bRet = true;
					}
				}
				if (frm != null)
				{
					if (!frm.Disposing)
					{
						if (!frm.IsDisposed)
						{
							if (frm.IsHandleCreated)
							{
								frm.Close();
							}
						}
					}
				}
				frm = null;
				this.Cursor = System.Windows.Forms.Cursors.Default;
				if (!bRet)
				{
					MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				}
				else
				{
					if (!string.IsNullOrEmpty(sMsg))
					{
						MessageBox.Show(this, sMsg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
				return bRet;
			}
			else
			{
				return true;
			}
		}
		string AddNewTable(TableRelation join)
		{
			string s = "";
			if (join.Relation == enmTableRelation.NONE)
			{
				return "," + Sep1 + join.Table2 + Sep2;
			}
			if (join.FieldCount > 0)
			{
				if (join.Relation == enmTableRelation.INNER)
					s = " INNER JOIN " + Sep1;
				else
					s = " LEFT JOIN " + Sep1;
				s += join.JoinFields[0].field2.FromTableName;
				s += Sep2 + " ON ((" + Sep1;
				s += join.JoinFields[0].field1.FromTableName;
				s += Sep2 + "." + Sep1;
				s += join.JoinFields[0].field1.Name;
				s += Sep2 + "=" + Sep1;
				s += join.JoinFields[0].field2.FromTableName;
				s += Sep2 + "." + Sep1;
				s += join.JoinFields[0].field2.Name;
				s += Sep2;
				for (int i = 1; i < join.FieldCount; i++)
				{
					s += ") AND (" + Sep1;
					s += join.JoinFields[i].field1.FromTableName;
					s += Sep2 + "." + Sep1;
					s += join.JoinFields[i].field1.Name;
					s += Sep2 + "=" + Sep1;
					s += join.JoinFields[i].field2.FromTableName;
					s += Sep2 + "." + Sep1;
					s += join.JoinFields[i].field2.Name;
					s += Sep2;
				}
				s += "))";
			}
			return s;
		}
		void setQueryState(bool bVerified)
		{
			btDel.Enabled = bVerified;
			btOK.Enabled = bVerified;
			btLeft.Enabled = bVerified;
			btRight.Enabled = bVerified;
			btAdd.Enabled = bVerified;
			btWhere.Enabled = bVerified;
			btOrderBy.Enabled = bVerified;
			byGroupBy.Enabled = bVerified;
			chkSQL.Enabled = bVerified;
			btParam.Enabled = false;
			if (bVerified)
			{
				if (qParser.query.Parameters != null)
				{
					if (qParser.query.Parameters.Count > 0)
						btParam.Enabled = true;
				}
			}
		}
		int getTopN()
		{
			int n = 0;
			try
			{
				n = Convert.ToInt32(txtTop.Text);
			}
			catch
			{
				n = 0;
			}
			return n;
		}
		void showSQL()
		{
			bool bShow = true;
			chkSQL.Enabled = true;
			if (!chkSQL.Checked)
			{
				if (qParser.query.bQueryOK && !bSELECTchanged)
					bShow = false;
				else
				{
					if (txtSELECT.Text.Trim().Length > 0)
					{
						chkSQL.Checked = true;
						chkSQL.Enabled = false;
					}
					else
					{
						bShow = false;
					}
				}
			}
			lblSQL.Visible = bShow;
			txtSELECT.Visible = bShow;
			splitContainer1.Panel2Collapsed = !bShow;
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
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.txtSELECT = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblDatabase = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDel = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.btLeft = new System.Windows.Forms.Button();
			this.btRight = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btCompile = new System.Windows.Forms.Button();
			this.btResetSelect = new System.Windows.Forms.Button();
			this.byGroupBy = new System.Windows.Forms.Button();
			this.btOrderBy = new System.Windows.Forms.Button();
			this.btWhere = new System.Windows.Forms.Button();
			this.lblSQL = new System.Windows.Forms.Label();
			this.chkSQL = new System.Windows.Forms.CheckBox();
			this.btParam = new System.Windows.Forms.Button();
			this.btDB = new System.Windows.Forms.Button();
			this.txtQueryName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtTop = new System.Windows.Forms.TextBox();
			this.cbbFields = new System.Windows.Forms.ComboBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
#if NET35
#else
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
#endif
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
#if NET35
#else
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
#endif
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(3, 22);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(189, 132);
			this.treeView1.TabIndex = 1;
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
			// 
			// txtSELECT
			// 
			this.txtSELECT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSELECT.Location = new System.Drawing.Point(70, 3);
			this.txtSELECT.Multiline = true;
			this.txtSELECT.Name = "txtSELECT";
			this.txtSELECT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSELECT.Size = new System.Drawing.Size(524, 147);
			this.txtSELECT.TabIndex = 2;
			this.txtSELECT.TextChanged += new System.EventHandler(this.txtSELECT_TextChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(8, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(592, 32);
			this.label3.TabIndex = 6;
			this.label3.Tag = "1";
			this.label3.Text = "Query Builder";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 13);
			this.label4.TabIndex = 7;
			this.label4.Tag = "4";
			this.label4.Text = "Tables and fields available";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 6);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 13);
			this.label5.TabIndex = 8;
			this.label5.Tag = "5";
			this.label5.Text = "Selected data fields";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 78);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 23);
			this.label6.TabIndex = 9;
			this.label6.Tag = "2";
			this.label6.Text = "Description:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDatabase
			// 
			this.lblDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDatabase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblDatabase.Location = new System.Drawing.Point(75, 44);
			this.lblDatabase.Name = "lblDatabase";
			this.lblDatabase.Size = new System.Drawing.Size(471, 23);
			this.lblDatabase.TabIndex = 12;
			this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 44);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 23);
			this.label8.TabIndex = 11;
			this.label8.Tag = "3";
			this.label8.Text = "Database:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btAdd
			// 
			this.btAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btAdd.Location = new System.Drawing.Point(139, 0);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(32, 22);
			this.btAdd.TabIndex = 13;
			this.btAdd.Text = ">";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDel
			// 
			this.btDel.Enabled = false;
			this.btDel.Location = new System.Drawing.Point(285, 0);
			this.btDel.Name = "btDel";
			this.btDel.Size = new System.Drawing.Size(56, 24);
			this.btDel.TabIndex = 14;
			this.btDel.Text = "Delete";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(109, 6);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(66, 13);
			this.label7.TabIndex = 15;
			this.label7.Tag = "6";
			this.label7.Text = "Current field:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btLeft
			// 
			this.btLeft.Enabled = false;
			this.btLeft.Location = new System.Drawing.Point(337, 0);
			this.btLeft.Name = "btLeft";
			this.btLeft.Size = new System.Drawing.Size(32, 24);
			this.btLeft.TabIndex = 17;
			this.btLeft.Text = "<";
			this.btLeft.Click += new System.EventHandler(this.btLeft_Click);
			// 
			// btRight
			// 
			this.btRight.Enabled = false;
			this.btRight.Location = new System.Drawing.Point(363, 0);
			this.btRight.Name = "btRight";
			this.btRight.Size = new System.Drawing.Size(32, 24);
			this.btRight.TabIndex = 18;
			this.btRight.Text = ">";
			this.btRight.Click += new System.EventHandler(this.btRight_Click);
			// 
			// btOK
			// 
			this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btOK.Enabled = false;
			this.btOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btOK.Location = new System.Drawing.Point(456, 424);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(56, 23);
			this.btOK.TabIndex = 19;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btNext_Click);
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(512, 424);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 20;
			this.btCancel.Text = "Cancel";
			// 
			// btCompile
			// 
			this.btCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btCompile.Enabled = false;
			this.btCompile.Location = new System.Drawing.Point(360, 424);
			this.btCompile.Name = "btCompile";
			this.btCompile.Size = new System.Drawing.Size(98, 23);
			this.btCompile.TabIndex = 21;
			this.btCompile.Tag = "14";
			this.btCompile.Text = "Check syntax";
			this.btCompile.Click += new System.EventHandler(this.btCompile_Click);
			// 
			// btResetSelect
			// 
			this.btResetSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btResetSelect.Enabled = false;
			this.btResetSelect.Location = new System.Drawing.Point(0, 127);
			this.btResetSelect.Name = "btResetSelect";
			this.btResetSelect.Size = new System.Drawing.Size(64, 23);
			this.btResetSelect.TabIndex = 22;
			this.btResetSelect.Tag = "8";
			this.btResetSelect.Text = "Reset";
			this.btResetSelect.Visible = false;
			this.btResetSelect.Click += new System.EventHandler(this.btResetSelect_Click);
			// 
			// byGroupBy
			// 
			this.byGroupBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.byGroupBy.Location = new System.Drawing.Point(88, 424);
			this.byGroupBy.Name = "byGroupBy";
			this.byGroupBy.Size = new System.Drawing.Size(64, 23);
			this.byGroupBy.TabIndex = 24;
			this.byGroupBy.Tag = "10";
			this.byGroupBy.Text = "Group";
			this.byGroupBy.Click += new System.EventHandler(this.byGroupBy_Click);
			// 
			// btOrderBy
			// 
			this.btOrderBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btOrderBy.Location = new System.Drawing.Point(152, 424);
			this.btOrderBy.Name = "btOrderBy";
			this.btOrderBy.Size = new System.Drawing.Size(64, 23);
			this.btOrderBy.TabIndex = 25;
			this.btOrderBy.Tag = "11";
			this.btOrderBy.Text = "Sort";
			this.btOrderBy.Click += new System.EventHandler(this.btOrderBy_Click);
			// 
			// btWhere
			// 
			this.btWhere.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btWhere.Location = new System.Drawing.Point(216, 424);
			this.btWhere.Name = "btWhere";
			this.btWhere.Size = new System.Drawing.Size(64, 23);
			this.btWhere.TabIndex = 26;
			this.btWhere.Tag = "12";
			this.btWhere.Text = "Filter";
			this.btWhere.Click += new System.EventHandler(this.btWhere_Click);
			// 
			// lblSQL
			// 
			this.lblSQL.Location = new System.Drawing.Point(3, 0);
			this.lblSQL.Name = "lblSQL";
			this.lblSQL.Size = new System.Drawing.Size(64, 64);
			this.lblSQL.TabIndex = 27;
			this.lblSQL.Tag = "7";
			this.lblSQL.Text = "SQL statement";
			// 
			// chkSQL
			// 
			this.chkSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkSQL.Location = new System.Drawing.Point(8, 424);
			this.chkSQL.Name = "chkSQL";
			this.chkSQL.Size = new System.Drawing.Size(80, 24);
			this.chkSQL.TabIndex = 28;
			this.chkSQL.Tag = "9";
			this.chkSQL.Text = "Show SQL";
			this.chkSQL.CheckedChanged += new System.EventHandler(this.chkSQL_CheckedChanged);
			// 
			// btParam
			// 
			this.btParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btParam.Location = new System.Drawing.Point(280, 424);
			this.btParam.Name = "btParam";
			this.btParam.Size = new System.Drawing.Size(80, 23);
			this.btParam.TabIndex = 29;
			this.btParam.Tag = "13";
			this.btParam.Text = "Parameters";
			this.btParam.Click += new System.EventHandler(this.btParam_Click);
			// 
			// btDB
			// 
			this.btDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btDB.Location = new System.Drawing.Point(552, 44);
			this.btDB.Name = "btDB";
			this.btDB.Size = new System.Drawing.Size(32, 23);
			this.btDB.TabIndex = 30;
			this.btDB.Text = "...";
			this.btDB.Click += new System.EventHandler(this.btDB_Click);
			// 
			// txtQueryName
			// 
			this.txtQueryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtQueryName.Location = new System.Drawing.Point(75, 75);
			this.txtQueryName.Name = "txtQueryName";
			this.txtQueryName.Size = new System.Drawing.Size(329, 20);
			this.txtQueryName.TabIndex = 31;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(410, 79);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 16);
			this.label1.TabIndex = 32;
			this.label1.Tag = "15";
			this.label1.Text = "Number of sample records:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtTop
			// 
			this.txtTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTop.Location = new System.Drawing.Point(556, 78);
			this.txtTop.Name = "txtTop";
			this.txtTop.Size = new System.Drawing.Size(34, 20);
			this.txtTop.TabIndex = 33;
			this.txtTop.Text = "10";
			// 
			// cbbFields
			// 
			this.cbbFields.Location = new System.Drawing.Point(181, 3);
			this.cbbFields.Name = "cbbFields";
			this.cbbFields.Size = new System.Drawing.Size(104, 21);
			this.cbbFields.TabIndex = 34;
			this.cbbFields.SelectedIndexChanged += new System.EventHandler(this.cbbFields_SelectedIndexChanged);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(3, 22);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.Size = new System.Drawing.Size(392, 132);
			this.dataGridView1.TabIndex = 35;
			this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(6, 104);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtSELECT);
			this.splitContainer1.Panel2.Controls.Add(this.btResetSelect);
			this.splitContainer1.Panel2.Controls.Add(this.lblSQL);
			this.splitContainer1.Size = new System.Drawing.Size(594, 314);
			this.splitContainer1.SplitterDistance = 157;
			this.splitContainer1.TabIndex = 36;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.btAdd);
			this.splitContainer2.Panel1.Controls.Add(this.label4);
			this.splitContainer2.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
			this.splitContainer2.Panel2.Controls.Add(this.cbbFields);
			this.splitContainer2.Panel2.Controls.Add(this.label5);
			this.splitContainer2.Panel2.Controls.Add(this.label7);
			this.splitContainer2.Panel2.Controls.Add(this.btDel);
			this.splitContainer2.Panel2.Controls.Add(this.btLeft);
			this.splitContainer2.Panel2.Controls.Add(this.btRight);
			this.splitContainer2.Size = new System.Drawing.Size(594, 157);
			this.splitContainer2.SplitterDistance = 195;
			this.splitContainer2.TabIndex = 0;
			// 
			// dlgQueryBuilder
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(602, 456);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.txtTop);
			this.Controls.Add(this.txtQueryName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btDB);
			this.Controls.Add(this.btParam);
			this.Controls.Add(this.chkSQL);
			this.Controls.Add(this.btWhere);
			this.Controls.Add(this.btOrderBy);
			this.Controls.Add(this.byGroupBy);
			this.Controls.Add(this.btCompile);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblDatabase);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.MinimizeBox = false;
			this.Name = "dlgQueryBuilder";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database Query Builder";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgQueryBuilder_Closing);
			this.Resize += new System.EventHandler(this.dlgQueryBuilder_Resize);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
#if NET35
#else
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
#endif
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
#if NET35
#else
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
#endif
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion


		private void txtSELECT_TextChanged(object sender, System.EventArgs e)
		{
			if (!_loadingText)
			{
				txtSELECT.ForeColor = System.Drawing.Color.Blue;
				setQueryState(false);
				bSELECTchanged = true;
				btCompile.Enabled = true;
			}
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				EPField fld0 = treeView1.SelectedNode.Tag as EPField;
				if (fld0 != null)
				{
					this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
					bool bOK = true;
					EasyQuery query = (EasyQuery)qParser.query.Clone();
					if (bSELECTchanged)
					{
						string s = txtSELECT.Text.Trim();
						if (s.Length > 0)
							bOK = CheckAndCompile();
						else
						{
							bOK = true;
							query.ClearQuery();
						}
					}
					if (bOK)
					{
						enumRecSource src = (enumRecSource)treeView1.SelectedNode.Parent.Parent.Tag;
						string sTableName = fld0.FromTableName;
						EPField fld = (EPField)fld0.Clone();
						fld.FromTableName = sTableName;
						fld.FieldText = Sep1 + sTableName + Sep2 + "." + Sep1 + fld.Name + Sep2;
						FieldList fl = query.Fields;
						if (fl == null)
						{
							fl = new FieldList();
							query.Fields = fl;
						}
						bool bFound = false;
						int n = fl.Count;
						for (int i = 0; i < n; i++)
						{
							if (fld.FieldMatch(fl[i]))
							{
								bFound = true;
								break;
							}
						}
						if (!bFound)
						{
							bool bTableOK = true;
							if (query.T == null)
								query.T = new TableAliasList();
							if (query.T.Count == 0)
							{
								query.From = Sep1 + sTableName + Sep2;
								TableAlias t = new TableAlias(sTableName, "");
								t.srcType = src;
								query.T.AddTable(t);
							}
							else
							{
								if (!query.T.IsTableIncluded(sTableName))
								{
									dlgAddTableToQuery dlg = new dlgAddTableToQuery();
									dlg.LoadData(qParser, sTableName, src);
									bTableOK = (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK);
									if (bTableOK)
									{
										if (query.T.Count <= 1)
											query.From = query.From + AddNewTable(dlg.join);
										else
											query.From = "(" + query.From + ")" + AddNewTable(dlg.join);
										TableAlias t = new TableAlias(sTableName, "");
										t.srcType = src;
										query.T.AddTable(t);
									}
								}
							}
							if (bTableOK)
							{
								if (!string.IsNullOrEmpty(query.GroupBy))
								{
									dlgQryGroupField dlg = new dlgQryGroupField();
									dlg.LoadData(fld);
									dlg.Sep1 = qParser.Sep1;
									dlg.Sep2 = qParser.Sep2;
									bTableOK = (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK);
									if (bTableOK)
									{
										fld = dlg.field;
										if (string.Compare(dlg.Aggregate, "Group by", StringComparison.OrdinalIgnoreCase) == 0)
											query.GroupBy += "," + fld.FieldText;
									}
								}
								if (bTableOK)
								{
									query.AddField(fld);
									string sMsg = "";
									if (applyQuery(query, ref sMsg))
									{
										qParser.query = query;
									}
									else
										MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
								}
							}
						}
					}
					this.Cursor = System.Windows.Forms.Cursors.Default;
				}
			}
		}

		private void btCompile_Click(object sender, System.EventArgs e)
		{
			CheckAndCompile();
		}

		private void btNext_Click(object sender, System.EventArgs e)
		{
			if (CheckAndCompile())
			{
				if (!qParser.query.IsCommand && string.IsNullOrEmpty(qParser.query.Where) && qParser.query.Top == 0)
				{
					if (MessageBox.Show(this, "This query does not use a filter. It may return large amount of data and take too much system resources. Do you want to continue?", "Query Builder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return;
					}
				}
				if (qParser.query.Parameters.Count > 0)
				{
					DlgParameters dlg = new DlgParameters();
					dlg.LoadData(qParser.query.Parameters);
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
					}
				}
				qParser.query.Description = txtQueryName.Text;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			if (ds != null)
			{
				if (ds.Tables.Count > 0 && dataGridView1.CurrentCell != null)
				{
					if (dataGridView1.CurrentCell.ColumnIndex >= 0 && dataGridView1.CurrentCell.ColumnIndex < ds.Tables[0].Columns.Count)
					{
						cbbFields.SelectedIndex = dataGridView1.CurrentCell.ColumnIndex;
					}
					else
						cbbFields.SelectedIndex = -1;
				}
			}
		}

		private void btResetSelect_Click(object sender, System.EventArgs e)
		{
			txtSELECT.Text = qParser.query.MakeSelectionQuery(null);
			txtSELECT.ForeColor = System.Drawing.Color.Black;
			bSELECTchanged = false;
			btResetSelect.Enabled = false;
			if (qParser.query.bQueryOK)
			{
				btCompile.Enabled = false;
				setQueryState(qParser.query.QueryMade());
			}
		}

		private void btDel_Click(object sender, System.EventArgs e)
		{
			if (!bSELECTchanged)
			{
				FieldList fl = qParser.query.Fields;
				if (fl != null)
				{
					if (fl.Count > 0)
					{
						if (fl.Count == 1)
						{
							qParser.query.Fields = null;
							qParser.query.T = null;
							ds.Tables[0].Rows.Clear();
							dataGridView1.DataBindings.Clear();
							dataGridView1.Refresh();
							txtSELECT.Text = "";
							btCompile.Enabled = false;
							btResetSelect.Enabled = false;
							setQueryState(false);
							btAdd.Enabled = true;
							cbbFields.Items.Clear();
							cbbFields.Text = "";
						}
						else
						{
							int n = cbbFields.SelectedIndex;
							EasyQuery qry = (EasyQuery)qParser.query.Clone();
							if (qry.DeleteField(n))
							{
								string sMsg = "";
								if (applyQuery(qry, ref sMsg))
								{
									qParser.query = qry;
									if (n >= cbbFields.Items.Count)
									{
										n = cbbFields.Items.Count - 1;
									}
									cbbFields.SelectedIndex = n;
								}
							}
						}
					}
				}
			}
		}

		private void btLeft_Click(object sender, System.EventArgs e)
		{
			int n = cbbFields.SelectedIndex;
			if (n > 0)
			{
				if (qParser.query.Fields != null)
				{
					if (qParser.query.Fields.Count > 1)
					{
						qParser.query.SwapFields(n, n - 1);
						applyQuery();
						cbbFields.SelectedIndex = n - 1;
					}
				}
			}
		}

		private void btRight_Click(object sender, System.EventArgs e)
		{
			int n = cbbFields.SelectedIndex;
			if (qParser.query.Fields != null)
			{
				if (n >= 0 && n < qParser.query.Fields.Count - 1)
				{
					qParser.query.SwapFields(n, n + 1);
					applyQuery();
					cbbFields.SelectedIndex = n + 1;
				}
			}
		}

		private void btOrderBy_Click(object sender, System.EventArgs e)
		{
			if (qParser.query.bQueryOK)
			{
				dlgQryOrder dlg = new dlgQryOrder();
				dlg.LoadData(qParser);
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					applyQuery();
				}
			}
		}

		private void chkSQL_CheckedChanged(object sender, System.EventArgs e)
		{
			showSQL();
		}

		private void byGroupBy_Click(object sender, System.EventArgs e)
		{
			if (qParser.query.bQueryOK)
			{
				dlgQryGroup dlg = new dlgQryGroup();
				dlg.LoadData(qParser);
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					string sMsg = "";
					if (applyQuery(dlg.qry, ref sMsg))
					{
						qParser.query = dlg.qry;
					}
					else
						MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}
		}

		private void treeView1_DoubleClick(object sender, System.EventArgs e)
		{
			btAdd_Click(null, null);
		}

		private void btWhere_Click(object sender, System.EventArgs e)
		{
			if (qParser.query.bQueryOK)
			{
				dlgWhere dlg = new dlgWhere();
				dlg.LoadData(qParser);
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					applyQuery();
				}
			}
		}
		public bool BuildFilter()
		{
			dlgWhere dlg = new dlgWhere();
			dlg.LoadData(qParser);
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				return true;
			}
			return false;
		}
		private void btParam_Click(object sender, System.EventArgs e)
		{
			DlgParameters dlg = new DlgParameters();
			dlg.LoadData(qParser.query.Parameters);
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				applyQuery();
			}
		}

		private void btDB_Click(object sender, System.EventArgs e)
		{
			Guid ID = qParser.query.ConnectionID;
			qParser.query.SelectConnection(this);
			{
				if (ID != qParser.query.ConnectionID)
				{
					qParser.ResetConnection();
					qParser.query.ClearQuery();
					txtSELECT.Text = "";
					bSELECTchanged = true;
					string sMsg = "";
					ReloadSchema(ref sMsg);
					if (sMsg.Length > 0)
					{
					}
				}
			}
		}

		private void dlgQueryBuilder_Resize(object sender, System.EventArgs e)
		{
		}

		private void dlgQueryBuilder_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (schema != null)
			{
				if (schema.dbCon != null)
					schema.dbCon.Close();
			}
			if (_cnn != null)
			{
				_cnn.Close();
			}
		}

		private void treeView1_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			TreeNode nd = e.Node;
			if (nd != null)
			{
				if (nd.Nodes.Count == 1)
				{
					if (nd.Nodes[0].Tag.GetType().IsValueType)
					{
						int n = 0;
						try
						{
							n = Convert.ToInt32(nd.Nodes[0].Tag);
						}
						catch
						{
						}
						nd.Nodes.Clear();
						FieldList flds = null;
						if (n == 1)
						{
							flds = schema.LoadTableFields(nd.Text);
						}
						else if (n == 2)
						{
							flds = schema.LoadViewFields(nd.Text);
						}
						if (flds != null)
						{
							for (int i = 0; i < flds.Count; i++)
							{
								TreeNode ndSub = nd.Nodes.Add(flds[i].Name);
								ndSub.Tag = flds[i];
							}
						}
					}
				}
			}
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode nd = e.Node;
			if (nd != null)
			{
				if (nd.Tag is EPField)
				{
					btAdd.Enabled = true;
				}
				else
				{
					btAdd.Enabled = false;
				}
			}
			else
			{
				btAdd.Enabled = false;
			}
		}

		private void cbbFields_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = cbbFields.SelectedIndex;
			if (n >= 0 && dataGridView1.Rows.Count > 0)
			{
				if (n < dataGridView1.Columns.Count)
				{
					if (dataGridView1.CurrentRow == null)
					{
						dataGridView1.Rows[0].Selected = true;
					}
					if (dataGridView1.CurrentCell == null || n != dataGridView1.CurrentCell.ColumnIndex)
					{
						if (dataGridView1.CurrentRow != null)
						{
							dataGridView1.CurrentCell = dataGridView1.CurrentRow.Cells[n];
						}
						else
						{
							dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[n];
						}
					}
				}
			}
		}

		private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}
	}
}
