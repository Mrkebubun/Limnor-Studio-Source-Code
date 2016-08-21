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
using System.Text;
using System.Data.OleDb;
using System.Data;
using VPL;
using System.Globalization;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgDatabase.
	/// </summary>
	public class dlgDatabase : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		EasyQuery qry;
		string sOriginalPath = "";
		string sBackupPath = "";
		string sConnectionString = "";
		//
		DatabaseSchema schema = new DatabaseSchema();
		//
		bool bMouseDown = false;
		clsDragTableData dragData = null;
		TableNode ndDrop = null;
		private int x0 = 0, y0 = 0;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Button btRename;
		private System.Windows.Forms.Button btDelChild;
		private System.Windows.Forms.Button btDelTable;
		private System.Windows.Forms.Button btAddTable;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private DataGridView dataGrid1;
		private System.Windows.Forms.Button btIndex;
		private System.Windows.Forms.Button btEdit;
		private System.Windows.Forms.Button btDelField;
		private System.Windows.Forms.Button btAddField;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.PictureBox picSep;
		private System.Windows.Forms.ComboBox cbxFields;
		private int nDeltaDragMove = 3; //in pixels

		public dlgDatabase()
		{
			qry = new EasyQuery();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			dlgDatabase_Resize(null, null);
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

			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.btRename = new System.Windows.Forms.Button();
			this.btDelChild = new System.Windows.Forms.Button();
			this.btDelTable = new System.Windows.Forms.Button();
			this.btAddTable = new System.Windows.Forms.Button();
			this.picSep = new System.Windows.Forms.PictureBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbxFields = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dataGrid1 = new DataGridView();
			this.btIndex = new System.Windows.Forms.Button();
			this.btEdit = new System.Windows.Forms.Button();
			this.btDelField = new System.Windows.Forms.Button();
			this.btAddField = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picSep)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.treeView1);
			this.groupBox1.Controls.Add(this.btRename);
			this.groupBox1.Controls.Add(this.btDelChild);
			this.groupBox1.Controls.Add(this.btDelTable);
			this.groupBox1.Controls.Add(this.btAddTable);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(240, 256);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Tag = "5";
			this.groupBox1.Text = "Tables";
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.FullRowSelect = true;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(8, 48);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(224, 200);
			this.treeView1.TabIndex = 9;
			this.treeView1.DragLeave += new System.EventHandler(this.treeView1_DragLeave);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseMove);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
			// 
			// btRename
			// 
			this.btRename.Location = new System.Drawing.Point(168, 24);
			this.btRename.Name = "btRename";
			this.btRename.Size = new System.Drawing.Size(64, 23);
			this.btRename.TabIndex = 8;
			this.btRename.Tag = "2";
			this.btRename.Text = "Rename";
			this.btRename.Visible = false;
			// 
			// btDelChild
			// 
			this.btDelChild.Enabled = false;
			this.btDelChild.Location = new System.Drawing.Point(104, 24);
			this.btDelChild.Name = "btDelChild";
			this.btDelChild.Size = new System.Drawing.Size(64, 23);
			this.btDelChild.TabIndex = 7;
			this.btDelChild.Tag = "1";
			this.btDelChild.Text = "De-link";
			this.btDelChild.Click += new System.EventHandler(this.btDelChild_Click);
			// 
			// btDelTable
			// 
			this.btDelTable.Location = new System.Drawing.Point(56, 24);
			this.btDelTable.Name = "btDelTable";
			this.btDelTable.Size = new System.Drawing.Size(48, 23);
			this.btDelTable.TabIndex = 6;
			this.btDelTable.Text = "Delete";
			this.btDelTable.Click += new System.EventHandler(this.btDelTable_Click);
			// 
			// btAddTable
			// 
			this.btAddTable.Location = new System.Drawing.Point(8, 24);
			this.btAddTable.Name = "btAddTable";
			this.btAddTable.Size = new System.Drawing.Size(48, 23);
			this.btAddTable.TabIndex = 5;
			this.btAddTable.Text = "Add";
			this.btAddTable.Click += new System.EventHandler(this.btAddTable_Click);
			// 
			// picSep
			// 
			this.picSep.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.picSep.Cursor = System.Windows.Forms.Cursors.VSplit;
			this.picSep.Location = new System.Drawing.Point(248, 0);
			this.picSep.Name = "picSep";
			this.picSep.Size = new System.Drawing.Size(3, 256);
			this.picSep.TabIndex = 15;
			this.picSep.TabStop = false;
			this.picSep.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picSep_MouseMove);
			this.picSep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSep_MouseDown);
			this.picSep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picSep_MouseUp);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbxFields);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.dataGrid1);
			this.groupBox2.Controls.Add(this.btIndex);
			this.groupBox2.Controls.Add(this.btEdit);
			this.groupBox2.Controls.Add(this.btDelField);
			this.groupBox2.Controls.Add(this.btAddField);
			this.groupBox2.Location = new System.Drawing.Point(264, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(320, 256);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Tag = "6";
			this.groupBox2.Text = "Fields";
			// 
			// cbxFields
			// 
			this.cbxFields.Location = new System.Drawing.Point(120, 56);
			this.cbxFields.Name = "cbxFields";
			this.cbxFields.Size = new System.Drawing.Size(192, 21);
			this.cbxFields.TabIndex = 21;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 20;
			this.label2.Tag = "4";
			this.label2.Text = "Selected field:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dataGrid1
			// 
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Location = new System.Drawing.Point(8, 80);
			this.dataGrid1.Name = "dataGrid1";
			/*
			EasyQuery easyQuery1 = new EasyQuery();
			easyQuery1.DatabaseConnection = null;
			easyQuery1.Distinct = false;
			easyQuery1.From = null;
			easyQuery1.GroupBy = null;
			easyQuery1.Having = null;
			easyQuery1.OrderBy = null;
			easyQuery1.Parameters = null;
			easyQuery1.Percent = false;
			easyQuery1.Description = null;
			easyQuery1.UpdatableTableName = null;
			easyQuery1.Top = 0;
			easyQuery1.Where = null;
			easyQuery1.WithTies = false;
			this.dataGrid1.DataSource = easyQuery1;
			*/
			this.dataGrid1.Size = new System.Drawing.Size(304, 168);
			this.dataGrid1.TabIndex = 18;
			this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			// 
			// btIndex
			// 
			this.btIndex.Location = new System.Drawing.Point(152, 24);
			this.btIndex.Name = "btIndex";
			this.btIndex.Size = new System.Drawing.Size(64, 23);
			this.btIndex.TabIndex = 17;
			this.btIndex.Tag = "3";
			this.btIndex.Text = "Index";
			this.btIndex.Click += new System.EventHandler(this.btIndex_Click);
			// 
			// btEdit
			// 
			this.btEdit.Location = new System.Drawing.Point(56, 24);
			this.btEdit.Name = "btEdit";
			this.btEdit.Size = new System.Drawing.Size(48, 23);
			this.btEdit.TabIndex = 16;
			this.btEdit.Text = "Edit";
			this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// btDelField
			// 
			this.btDelField.Location = new System.Drawing.Point(104, 24);
			this.btDelField.Name = "btDelField";
			this.btDelField.Size = new System.Drawing.Size(48, 23);
			this.btDelField.TabIndex = 15;
			this.btDelField.Text = "Delete";
			this.btDelField.Click += new System.EventHandler(this.btDelField_Click);
			// 
			// btAddField
			// 
			this.btAddField.Location = new System.Drawing.Point(8, 24);
			this.btAddField.Name = "btAddField";
			this.btAddField.Size = new System.Drawing.Size(48, 23);
			this.btAddField.TabIndex = 14;
			this.btAddField.Text = "Add";
			this.btAddField.Click += new System.EventHandler(this.btAddField_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(232, 280);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 19;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(144, 280);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 18;
			this.btOK.Text = "OK";
			// 
			// dlgDatabase
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 318);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.picSep);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDatabase";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database structure";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgDatabase_Closing);
			this.Resize += new System.EventHandler(this.dlgDatabase_Resize);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picSep)).EndInit();
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		public bool LoadAccess(string sPath, string sDBPass, string sUser, string sUserPass)//,out string errStr)
		{
			try
			{
				sOriginalPath = sPath;
				this.Text = this.Text + " - " + System.IO.Path.GetFileName(sPath);
				sBackupPath = sPath + ".BACKUP";
				System.IO.File.Copy(sOriginalPath, sBackupPath, true);
				sConnectionString = ConnectionStringSelector.MakeAccessConnectionString(sOriginalPath, false, true, sDBPass, sUser, sUserPass);
				ConnectionStringSelector.InsertValue(ref sConnectionString, "Password", sUserPass);
				ConnectionStringSelector.InsertValue(ref sConnectionString, "Jet OLEDB:Database Password", sDBPass);
				qry = new EasyQuery();
				Connection objCn = new Connection();
				objCn.DatabaseType = typeof(OleDbConnection);
				objCn.ConnectionString = sConnectionString;

				qry.DatabaseConnection = new ConnectionItem(objCn);
				qry.Description = "Table editor";
				//
				objCn.SetCredential(sUser, sUserPass, sDBPass);
				schema.dbCon = objCn;
				schema.LoadSchema();
				loadTables();
				return true;
			}
			catch (Exception er)
			{
				FormLog.NotifyException(true, er);
			}
			finally
			{

			}
			return false;
		}
		public bool LoadData(Connection conn)
		{
			try
			{
				this.Text = this.Text + " - " + conn.ConnectionString;
				qry = new EasyQuery();
				Connection objCn = new Connection();
				objCn.NameDelimiterStyle = conn.NameDelimiterStyle;
				objCn.DatabaseType = conn.DatabaseType;
				objCn.ConnectionString = conn.ConnectionString;
				qry.DatabaseConnection = new ConnectionItem(objCn);
				qry.Description = "Table editor";
				//
				schema.dbCon = objCn;
				schema.LoadSchema();
				loadTables();
				btCancel.Enabled = false;
				return true;
			}
			catch (Exception er)
			{
				FormLog.NotifyException(true, er);
			}
			finally
			{

			}
			return false;
		}
		void clearQuery()
		{
			qry.ClearQuery();
		}
		void loadTables()
		{
			clearQuery();
			treeView1.Nodes.Clear();
			TableNode nd, ndSub;
			int n, i;
			DatabaseTable[] subTables;
			for (n = 0; n < schema.TableCount; n++)
			{
				nd = new TableNode();
				nd.table = schema.GetTable(n);
				nd.Text = nd.table.ToString();
				subTables = schema.GetChildTables(n);
				treeView1.Nodes.Add(nd);
				if (subTables != null)
				{
					for (i = 0; i < subTables.Length; i++)
					{
						ndSub = new TableNode();
						ndSub.table = subTables[i];
						ndSub.Text = subTables[i].ToString();
						nd.Nodes.Add(ndSub);
					}
				}
			}
		}
		private void dlgDatabase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult != System.Windows.Forms.DialogResult.OK)
			{
				//restore from backup
				try
				{
					if (sOriginalPath != null && sOriginalPath.Length > 0)
					{
						string sEditPath = sOriginalPath + ".Edit";
						if (System.IO.File.Exists(sEditPath))
							System.IO.File.Delete(sEditPath);
						System.IO.File.Move(sOriginalPath, sEditPath);
						System.IO.File.Move(sBackupPath, sOriginalPath);
					}
				}
				catch (Exception er)
				{
					StringBuilder s = new StringBuilder(er.Message);
					s.Append("\r\n");
					s.Append(Resource1.dbMsg5); s.Append("\r\n");
					s.Append(Resource1.dbMsg6); s.Append(sBackupPath); s.Append("\r\n");
					s.Append(Resource1.dbMsg7); s.Append(sOriginalPath); s.Append("\r\n");
					MessageBox.Show(this, s.ToString(), this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			btDelChild.Enabled = false;
			cbxFields.Items.Clear();
			TableNode nd = treeView1.SelectedNode as TableNode;
			if (nd != null)
			{
				try
				{
					int i;
					btDelChild.Enabled = (nd.Parent != null);
					//
					if (!nd.FieldsLoaded)
					{
						nd.FieldsLoaded = true;
						nd.table.GetFields(schema.dbCon);
						nd.table.GetIndexes(schema.dbCon);
					}
					for (i = 0; i < nd.table.FieldCount; i++)
					{
						cbxFields.Items.Add(nd.table.GetField(i));
					}
					//check unique index
					bool bIndexed = false;
					for (i = 0; i < nd.table.FieldCount; i++)
					{
						if (nd.table.GetField(i).Indexed)
						{
							bIndexed = true;
							break;
						}
					}
					if (!bIndexed)
					{
						if (nd.table.Indexes != null)
						{
							for (i = 0; i < nd.table.Indexes.Length; i++)
							{
								if (nd.table.Indexes[i].IsUnique)
								{
									for (int j = 0; j < nd.table.Indexes[i].fields.Count; j++)
									{
										nd.table.fields[nd.table.Indexes[i].fields[i].Name].Indexed = true;
									}
									break;
								}
							}
						}
					}
					//
					dataGrid1.Refresh();
					dataGrid1.DataBindings.Clear();
					dataGrid1.DataSource = null;
					dataGrid1.DataMember = null;
					//
					qry.ClearQuery();
					//
					dataGrid1.Refresh();
					dataGrid1.Name = nd.Text;
					//create a query for the table
					qry.UpdatableTableName = nd.Text;
					qry.Description = nd.Text;
					qry.SampleTopRec = nd.TopRec;
					qry.UseSampleTopRec = true;
					if (qry.DatabaseConnection.TopRecordStyle == EnumTopRecStyle.NotAllowed)
					{
						qry.Where = "1=2";
					}
					else
					{
						qry.Where = "";
					}
					qry.From = string.Format(CultureInfo.InvariantCulture,
						"{0}{1}{2}", qry.NameDelimiterBegin, nd.Text, qry.NameDelimiterEnd);
					FieldList fl = new FieldList();
					for (i = 0; i < nd.table.FieldCount; i++)
					{
						fl.AddField(nd.table.GetField(i));
					}
					for (i = 0; i < fl.Count; i++)
					{
						fl[i].SetDefaultEditor();
					}
					qry.Fields = fl;
					qry.Query();
					//
					dataGrid1.DataSource = qry.DataStorage;
					if (qry.DataStorage != null)
					{
						dataGrid1.DataMember = qry.DataStorage.Tables[0].TableName;
					}
					dataGrid1.Refresh();
					//
					schema.dbCon = qry.DatabaseConnection.ConnectionObject;
					dataGrid1_CurrentCellChanged(null, null);
					dlgDatabase_Resize(null, null);
					dataGrid1.Visible = (qry.Fields.Count > 0);
					if (cbxFields.Items.Count > 0)
					{
						cbxFields.SelectedIndex = 0;
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(this, VPLUtil.FormExceptionText(err), this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}
		}

		private void btAddTable_Click(object sender, System.EventArgs e)
		{
			string sRet = "";
			string sID = "";
			dlgNewTable dlg = new dlgNewTable();
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				sRet = dlg.sName;
				sID = dlg.sID.Trim();
				bool bUsed = false;
				int i;
				for (i = 0; i < treeView1.Nodes.Count; i++)
				{
					if (string.Compare(sRet, treeView1.Nodes[i].Text, StringComparison.OrdinalIgnoreCase) == 0)
					{
						bUsed = true;
						break;
					}
				}
				if (bUsed)
				{
					MessageBox.Show(this, Resource1.dbMsg1, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
				else
				{
					try
					{
						Connection connect = qry.DatabaseConnection.ConnectionObject;
						connect.CreateTable(sRet, sID);
						schema.dbCon = connect;
						schema.LoadSchema();
						loadTables();
						for (i = 0; i < treeView1.Nodes.Count; i++)
						{
							if (string.Compare(sRet, treeView1.Nodes[i].Text, StringComparison.OrdinalIgnoreCase) == 0)
							{
								treeView1.SelectedNode = treeView1.Nodes[i];
								break;
							}
						}
					}
					catch (Exception er)
					{
						MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void btDelTable_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			if (nd != null)
			{
				try
				{
					string s = "Table name:" + nd.Text + "\r\n" + "Do you want to delete this table from the database?";
					if (MessageBox.Show(this, s, this.Text, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						qry.DatabaseConnection.ConnectionObject.DropTable(nd.Text);
						Connection cn = new Connection();
						cn.TheConnection = qry.DatabaseConnection.ConnectionObject.TheConnection;
						schema.dbCon = cn;
						schema.LoadSchema();
						loadTables();
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btAddField_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			if (nd != null)
			{
				try
				{
					dlgEditField dlg = new dlgEditField();
					dlg.LoadData(nd.Text, null, qry.DatabaseConnection.ConnectionObject);
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						nd.table.GetFields(qry.DatabaseConnection.ConnectionObject);
						nd.table.GetIndexes(qry.DatabaseConnection.ConnectionObject);

					}

					int n = cbxFields.Items.Count;
					treeView1_AfterSelect(null, null);
					if (n < cbxFields.Items.Count)
					{
						cbxFields.SelectedIndex = n;
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			for (int i = 0; i < cbxFields.Items.Count; i++)
			{
				if (dataGrid1.CurrentCell != null)
				{
					if (((EPField)cbxFields.Items[i]).Name == dataGrid1.Columns[dataGrid1.CurrentCell.ColumnIndex].Name)//  .CurrentColumnCaption)
					{
						cbxFields.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void btEdit_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			int n = cbxFields.SelectedIndex;
			if (nd != null && n >= 0)
			{
				try
				{
					dlgEditField dlg = new dlgEditField();
					EPField fld = (EPField)((EPField)cbxFields.Items[n]).Clone();
					dlg.LoadData(nd.Text, fld, qry.DatabaseConnection.ConnectionObject);
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						nd.table.GetFields(qry.DatabaseConnection.ConnectionObject);
						nd.table.GetIndexes(qry.DatabaseConnection.ConnectionObject);
						treeView1_AfterSelect(null, null);
						if (n < cbxFields.Items.Count)
						{
							cbxFields.SelectedIndex = n;
						}
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btDelField_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			int n = cbxFields.SelectedIndex;
			if (nd != null && n >= 0)
			{
				try
				{
					EPField fld = (EPField)cbxFields.Items[n];
					string s = fld.Name + "\r\n";
					s += Resource1.askDeleteField;
					if (MessageBox.Show(this, s, this.Text, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						qry.DatabaseConnection.ConnectionObject.DropColumn(nd.Text, fld.Name);
						nd.table.GetFields(qry.DatabaseConnection.ConnectionObject);
						nd.table.GetIndexes(qry.DatabaseConnection.ConnectionObject);
						treeView1_AfterSelect(null, null);
						if (n < cbxFields.Items.Count)
						{
							cbxFields.SelectedIndex = n;
						}
						else
						{
							if (cbxFields.Items.Count > 0)
							{
								cbxFields.SelectedIndex = cbxFields.Items.Count - 1;
							}
						}
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btIndex_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			if (nd != null)
			{
				dlgIndexes dlg = new dlgIndexes();
				dlg.LoadData(nd.table, qry.DatabaseConnection.ConnectionObject);
				dlg.ShowDialog(this);
				if (dlg.bChanged)
				{
					nd.table.GetIndexes(qry.DatabaseConnection.ConnectionObject);
				}
			}
		}

		private void btDelChild_Click(object sender, System.EventArgs e)
		{
			TableNode nd = treeView1.SelectedNode as TableNode;
			if (nd != null)
			{
				TableNode ndParent = nd.Parent as TableNode;
				if (ndParent != null)
				{
					string s = Resource1.deLinkTbl;
					if (MessageBox.Show(this, s, this.Text, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						TableReference rel = schema.FindRelation(ndParent.table.TableName, nd.table.TableName);
						if (rel == null)
						{
							MessageBox.Show(this, Resource1.relNotFound, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
						}
						else
						{
							qry.DatabaseConnection.ConnectionObject.DropRelation(nd.table.TableName, rel.RefName);
							ndParent.Nodes.Remove(nd);
							treeView1.SelectedNode = ndParent;
						}
					}
				}
			}
		}

		private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			bMouseDown = true;
		}

		private void treeView1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left && bMouseDown)
			{
				bMouseDown = false;
				//start drag group for re-grouping
				System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
				TableNode nd = (TableNode)treeView1.GetNodeAt(pt);
				if (nd != null)
				{
					treeView1.SelectedNode = nd;
					dragData = new clsDragTableData();
					dragData.objNode = nd;
					System.Windows.Forms.DragDropEffects ret = treeView1.DoDragDrop(dragData, System.Windows.Forms.DragDropEffects.All);
					if (ret == System.Windows.Forms.DragDropEffects.Move)
					{
					}
				}
			}
		}

		private void treeView1_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			bool bCanDrop = false;
			System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
			pt = treeView1.PointToClient(pt);
			TableNode nd = (TableNode)treeView1.GetNodeAt(pt);
			if (dragData != null && nd != null)
			{
				//nd to be parent, must be top node
				if (nd.Parent == null)
				{
					if (nd != dragData.objNode)
					{
						//nd must have unique key cannot be a child of dragData
						if (nd.table.HasUniqueKey())
							bCanDrop = !schema.RelationExists(nd.table.TableName, dragData.objNode.table.TableName);
					}
					if (bCanDrop)
					{
						e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
						if (ndDrop != nd)
						{
							if (ndDrop != null)
							{
								ndDrop.BackColor = System.Drawing.Color.White;
							}
						}
						ndDrop = nd;
						ndDrop.BackColor = System.Drawing.Color.GreenYellow;
					}
				}
			}
			if (!bCanDrop)
			{
				if (ndDrop != null)
				{
					ndDrop.BackColor = System.Drawing.Color.White;
					ndDrop = null;
				}
			}

			//scroll up/down
			int nDltX, nDltY;
			if (pt.Y < treeView1.ItemHeight)
			{
				//scroll up
				nDltY = pt.Y - y0;
				if (nDltY < 0) nDltY = -nDltY;
				nDltX = pt.X - x0;
				if (nDltX < 0) nDltX = -nDltX;
				if (nDltX < nDltY) nDltX = nDltY;
				if (nDltX > nDeltaDragMove)
				{
					y0 = pt.Y;
					x0 = pt.X;
					nd = (TableNode)treeView1.TopNode;
					//find the first invisible node 
					TreeNode nd1;
					while (nd != null)
					{
						nd1 = FindInvisibleNodeUp(nd);
						if (nd1 != null)
						{
							nd1.EnsureVisible();
							break;
						}
						nd = (TableNode)nd.Parent;
					}
				}
			}
			else if (pt.Y > treeView1.ClientSize.Height - treeView1.ItemHeight)
			{
				//scroll down
				nDltY = pt.Y - y0;
				if (nDltY < 0) nDltY = -nDltY;
				nDltX = pt.X - x0;
				if (nDltX < 0) nDltX = -nDltX;
				if (nDltX < nDltY) nDltX = nDltY;
				if (nDltX > nDeltaDragMove)
				{
					y0 = pt.Y;
					x0 = pt.X;
					nd = (TableNode)treeView1.GetNodeAt(pt);
					if (nd != null)
						nd = (TableNode)FindInvisibleNodeDn(nd);
					if (nd != null)
						nd.EnsureVisible();
				}
			}
			///
		}
		static public TreeNode FindInvisibleNodeUp(TreeNode nd0)
		{
			if (nd0.PrevNode != null)
				return nd0.PrevNode;
			return nd0.Parent;
		}
		static public TreeNode FindInvisibleNodeDn(TreeNode nd0)
		{
			if (nd0 != null)
			{
				TreeNode nd;
				//find child first
				nd0.Expand();
				nd = nd0.FirstNode;
				if (nd != null)
					return nd;
				//find sibling 
				nd = nd0.NextNode;
				if (nd != null)
					return nd;
				//find parent's sibling
				while (nd0 != null)
				{
					nd0 = nd0.Parent;
					if (nd0 != null)
					{
						nd = nd0.NextNode;
						if (nd != null)
							return nd;
					}
				}
			}
			return null;
		}
		private void treeView1_DragLeave(object sender, System.EventArgs e)
		{
			if (ndDrop != null)
			{
				ndDrop.BackColor = System.Drawing.Color.White;
				ndDrop = null;
			}
		}
		/*
		 * For MySQL
		 CREATE TABLE  child_table(
			foo INT
			,bar INT
			,FOREIGN KEY (bar) REFERENCES parent_table(parent_key)
			ON UPDATE CASCADE ON DELETE SET NULL
		 */
		private void treeView1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			try
			{
				bool bCanDrop = false;
				System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
				pt = treeView1.PointToClient(pt);
				TableNode nd = (TableNode)treeView1.GetNodeAt(pt);
				if (dragData != null && nd != null)
				{
					if (nd.Parent == null)
					{
						if (nd != dragData.objNode)
						{
							//nd must have unique key and cannot be a child of dragData
							if (nd.table.HasUniqueKey())
							{
								bCanDrop = !schema.RelationExists(nd.table.TableName, dragData.objNode.table.TableName);
							}
						}
						if (bCanDrop)
						{
							dlgSetRelation dlg = new dlgSetRelation();
							dlg.LoadData(nd.table, dragData.objNode.table, qry.DatabaseConnection.ConnectionObject);
							if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
							{
								schema.dbCon = qry.DatabaseConnection.ConnectionObject;
								schema.LoadSchema();
								this.loadTables();
							}
						}
					}
				}
			}
			catch (Exception er)
			{
				MessageBox.Show(this, VPLUtil.FormExceptionText(er), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				clearDrag();
			}
		}
		private void clearDrag()
		{
			if (ndDrop != null)
			{
				ndDrop.BackColor = System.Drawing.Color.White;
				ndDrop = null;
			}
			dragData = null;
		}

		private void dlgDatabase_Resize(object sender, System.EventArgs e)
		{
			dataGrid1.Location = new Point(8, 80);
			//btOK.Top is the bottom
			//picSep.Left is the separator
			btOK.Top = this.ClientSize.Height - btOK.Height - 8;
			btCancel.Top = btOK.Top;
			btOK.Left = (this.ClientSize.Width - btOK.Width) / 2;
			btCancel.Left = btOK.Left + btOK.Width + 8;
			int nBottom = btOK.Top - 8;
			if (nBottom > groupBox1.Top + 100)
			{
				groupBox1.Height = nBottom - 8 - groupBox1.Top;
				groupBox2.Height = groupBox1.Height;
				treeView1.Height = groupBox1.Height - 56;
				dataGrid1.Height = groupBox1.Height - 92;
				picSep.Top = 0;
				picSep.Height = groupBox1.Top + groupBox1.Height;
			}
			if (picSep.Left > 30 && picSep.Left < this.ClientSize.Width - 80)
			{
				groupBox1.Width = picSep.Left - groupBox1.Left;
				groupBox2.Left = picSep.Left + picSep.Width;
				groupBox2.Width = this.ClientSize.Width - groupBox2.Left - 8;
				treeView1.Width = groupBox1.Width - 16;
				dataGrid1.Width = groupBox2.Width - 16;
				if (groupBox2.Width > cbxFields.Left + 8)
				{
					cbxFields.Width = groupBox2.Width - cbxFields.Left - 3;
				}
			}
		}

		private void picSep_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			x0 = e.X;
			y0 = e.Y;
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				picSep.Capture = true;
			}
		}

		private void picSep_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (picSep.Capture)
			{
				int x = e.X - x0 + picSep.Left;
				if (x > 30 && x < this.ClientSize.Width - 80)
				{
					picSep.Left = x;
					dlgDatabase_Resize(null, null);
				}
			}
		}

		private void picSep_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			picSep.Capture = false;
		}
	}
	public class clsDragData
	{
		public clsDragData()
		{
			//
			//
		}
	}
	public class clsDragTableData : clsDragData
	{
		public TableNode objNode = null;
		public clsDragTableData()
		{
			//
			//
		}
	}
	public class TableNode : TreeNode
	{
		public DatabaseTable table = null;
		public int TopRec = 0;
		public string Filter = "";
		public bool FieldsLoaded = false;
		public TableNode()
		{
		}


		public override string ToString()
		{
			if (table != null)
				return table.ToString();
			return "";
		}

	}
}
