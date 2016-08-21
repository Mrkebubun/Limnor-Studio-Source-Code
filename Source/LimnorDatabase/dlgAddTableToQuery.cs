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

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgAddTableToQuery.
	/// </summary>
	public class dlgAddTableToQuery : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.RadioButton rdoInner;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblTable;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btHelpField;
		private System.Windows.Forms.Button btHelpType;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		//
		public TableRelation join = null;
		QueryParser parser = null;
		DatabaseSchema schema = null;
		DatabaseTable newTable = null;
		DatabaseView newView = null;
		enumRecSource srcType = enumRecSource.Table;
		private System.Windows.Forms.ComboBox cbx0;
		private System.Windows.Forms.ComboBox cbx1;
		private System.Windows.Forms.ComboBox cbx2;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.Label label7;
		System.Data.DataSet ds = null;
		public dlgAddTableToQuery()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgAddTableToQuery));
			this.label1 = new System.Windows.Forms.Label();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.rdoInner = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblTable = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btHelpField = new System.Windows.Forms.Button();
			this.btHelpType = new System.Windows.Forms.Button();
			this.cbx0 = new System.Windows.Forms.ComboBox();
			this.cbx1 = new System.Windows.Forms.ComboBox();
			this.cbx2 = new System.Windows.Forms.ComboBox();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.label7 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(424, 23);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Select join fields for the new table";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dataGrid1
			// 
			this.dataGrid1.CaptionVisible = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(24, 104);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(432, 96);
			this.dataGrid1.TabIndex = 1;
			this.dataGrid1.Scroll += new System.EventHandler(this.dataGrid1_Scroll);
			this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			// 
			// rdoInner
			// 
			this.rdoInner.Checked = true;
			this.rdoInner.Location = new System.Drawing.Point(24, 232);
			this.rdoInner.Name = "rdoInner";
			this.rdoInner.Size = new System.Drawing.Size(424, 24);
			this.rdoInner.TabIndex = 2;
			this.rdoInner.TabStop = true;
			this.rdoInner.Tag = "5";
			this.rdoInner.Text = "Records in the new table must exist";
			this.rdoInner.CheckedChanged += new System.EventHandler(this.rdoInner_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 256);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(408, 48);
			this.label2.TabIndex = 3;
			this.label2.Tag = "6";
			this.label2.Text = "If you select this option, the query will only return those records matching the " +
				"join fields in the new table";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 4;
			this.label3.Tag = "2";
			this.label3.Text = "New table name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblTable
			// 
			this.lblTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTable.Location = new System.Drawing.Point(136, 48);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(216, 23);
			this.lblTable.TabIndex = 5;
			this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 320);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(408, 48);
			this.label4.TabIndex = 7;
			this.label4.Tag = "8";
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(24, 296);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(424, 24);
			this.radioButton1.TabIndex = 6;
			this.radioButton1.Tag = "7";
			this.radioButton1.Text = "Records in the new table do not have to exist";
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(24, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 8;
			this.label5.Tag = "3";
			this.label5.Text = "Join fields:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(24, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 9;
			this.label6.Tag = "4";
			this.label6.Text = "Join type:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(168, 456);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 10;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(256, 456);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 11;
			this.btCancel.Text = "Cancel";
			// 
			// btHelpField
			// 
			this.btHelpField.Location = new System.Drawing.Point(128, 80);
			this.btHelpField.Name = "btHelpField";
			this.btHelpField.Size = new System.Drawing.Size(32, 23);
			this.btHelpField.TabIndex = 12;
			this.btHelpField.Text = "?";
			this.btHelpField.Visible = false;
			// 
			// btHelpType
			// 
			this.btHelpType.Location = new System.Drawing.Point(128, 208);
			this.btHelpType.Name = "btHelpType";
			this.btHelpType.Size = new System.Drawing.Size(32, 23);
			this.btHelpType.TabIndex = 13;
			this.btHelpType.Text = "?";
			this.btHelpType.Visible = false;
			// 
			// cbx0
			// 
			this.cbx0.Location = new System.Drawing.Point(208, 80);
			this.cbx0.Name = "cbx0";
			this.cbx0.Size = new System.Drawing.Size(121, 21);
			this.cbx0.TabIndex = 14;
			this.cbx0.Visible = false;
			this.cbx0.SelectedIndexChanged += new System.EventHandler(this.cbx0_SelectedIndexChanged);
			// 
			// cbx1
			// 
			this.cbx1.Location = new System.Drawing.Point(232, 80);
			this.cbx1.Name = "cbx1";
			this.cbx1.Size = new System.Drawing.Size(121, 21);
			this.cbx1.TabIndex = 15;
			this.cbx1.Visible = false;
			this.cbx1.SelectedIndexChanged += new System.EventHandler(this.cbx1_SelectedIndexChanged);
			// 
			// cbx2
			// 
			this.cbx2.Location = new System.Drawing.Point(272, 80);
			this.cbx2.Name = "cbx2";
			this.cbx2.Size = new System.Drawing.Size(121, 21);
			this.cbx2.TabIndex = 16;
			this.cbx2.Visible = false;
			this.cbx2.SelectedIndexChanged += new System.EventHandler(this.cbx2_SelectedIndexChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.Location = new System.Drawing.Point(24, 368);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(104, 24);
			this.radioButton2.TabIndex = 17;
			this.radioButton2.Tag = "9";
			this.radioButton2.Text = "No join";
			this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(48, 392);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(384, 56);
			this.label7.TabIndex = 18;
			this.label7.Tag = "10";
			this.label7.Text = resources.GetString("label7.Text");
			// 
			// dlgAddTableToQuery
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 488);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.cbx2);
			this.Controls.Add(this.cbx1);
			this.Controls.Add(this.cbx0);
			this.Controls.Add(this.btHelpType);
			this.Controls.Add(this.btHelpField);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.lblTable);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.rdoInner);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgAddTableToQuery";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Join a new table";
			this.Activated += new System.EventHandler(this.dlgAddTableToQuery_Activated);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(QueryParser p, string sTable, enumRecSource recSrc)
		{
			parser = p;
			lblTable.Text = sTable;
			schema = p.GetSchema();
			srcType = recSrc;
			int i;
			for (i = 0; i < p.query.T.Count; i++)
			{
				p.query.T[i].LoadSchema(schema);
			}
			if (srcType == enumRecSource.Table)
				newTable = schema.FindTable(sTable);
			else if (srcType == enumRecSource.View)
				newView = schema.FindView(sTable);
			//
			ds = new System.Data.DataSet("Rel");
			ds.Tables.Add("JF");
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			//
			ds.Tables[0].Columns[0].Caption = Resource1.addT0;
			ds.Tables[0].Columns[0].ColumnName = "NewField";
			ds.Tables[0].Columns[0].DataType = typeof(string);
			ds.Tables[0].Columns[0].MaxLength = 80;
			//
			ds.Tables[0].Columns[1].Caption = Resource1.addT1;
			ds.Tables[0].Columns[1].ColumnName = "JoinTable";
			ds.Tables[0].Columns[1].DataType = typeof(string);
			ds.Tables[0].Columns[1].MaxLength = 80;
			//
			ds.Tables[0].Columns[2].Caption = Resource1.addT2;
			ds.Tables[0].Columns[2].ColumnName = "JoinField";
			ds.Tables[0].Columns[2].DataType = typeof(string);
			ds.Tables[0].Columns[2].MaxLength = 80;
			//
			DataGridColumnStyle TextCol;
			DataGridTableStyle myGridTableStyle = new DataGridTableStyle();
			myGridTableStyle.MappingName = "JF";
			for (i = 0; i < 3; i++)
			{
				TextCol = new DataGridTextBoxColumn();
				TextCol.MappingName = ds.Tables[0].Columns[i].ColumnName;
				TextCol.HeaderText = ds.Tables[0].Columns[i].Caption;
				TextCol.Width = 120;
				myGridTableStyle.GridColumnStyles.Add(TextCol);
			}
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(myGridTableStyle);
			//
			dataGrid1.SetDataBinding(ds, "JF");
			//
			cbx0.Parent = dataGrid1;
			cbx1.Parent = dataGrid1;
			cbx2.Parent = dataGrid1;
			//
			cbx0.Items.Clear();
			int n = 0;
			if (srcType == enumRecSource.Table)
				n = newTable.FieldCount;
			else if (srcType == enumRecSource.View)
				n = newView.FieldCount;
			for (i = 0; i < n; i++)
			{
				if (srcType == enumRecSource.Table)
					cbx0.Items.Add(newTable.GetField(i));
				else if (srcType == enumRecSource.View)
					cbx0.Items.Add(newView.GetField(i));
			}
			//
			cbx1.Items.Clear();
			for (i = 0; i < p.query.T.Count; i++)
			{
				cbx1.Items.Add(p.query.T[i]);
			}
			//
			cbx2.Items.Clear();
		}
		public void SetDropdownWidth(System.Windows.Forms.ComboBox cbx)
		{
			// Create a Graphics object.
			Graphics g = this.CreateGraphics();
			int width = 0;
			for (int i = 0; i < cbx.Items.Count; i++)
			{
				// Get the Size needed to accommodate the formatted Text.
				int w = g.MeasureString(cbx.Items[i].ToString(), cbx.Font).ToSize().Width;
				if (w > width)
					width = w;
			}
			cbx.DropDownWidth = width + 20;
			// Clean up the Graphics object.
			g.Dispose();

		}
		private void cbx0_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbx0.SelectedIndex;
			if (n >= 0)
			{
				int r = dataGrid1.CurrentRowIndex;
				if (r >= ds.Tables[0].Rows.Count)
				{
					object[] vs = new object[3];
					vs[0] = cbx0.Items[n].ToString();
					vs[1] = "";
					vs[2] = "";
					ds.Tables[0].Rows.Add(vs);
					cbx2.Items.Clear();
				}
				else
				{
					ds.Tables[0].Rows[r][0] = cbx0.Items[n].ToString();
				}
			}
		}

		private void cbx1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbx1.SelectedIndex;
			if (n >= 0)
			{
				TableAlias tbl = cbx1.Items[n] as TableAlias;
				cbx2.Items.Clear();
				for (int i = 0; i < tbl.FieldCount; i++)
				{
					cbx2.Items.Add(tbl.GetField(i));
				}
				int r = dataGrid1.CurrentRowIndex;
				if (r >= ds.Tables[0].Rows.Count)
				{
					object[] vs = new object[3];
					vs[0] = "";
					vs[1] = cbx1.Items[n].ToString();
					vs[2] = "";
					ds.Tables[0].Rows.Add(vs);
				}
				else
				{
					ds.Tables[0].Rows[r][1] = cbx1.Items[n].ToString();
					ds.Tables[0].Rows[r][2] = "";
				}
			}
		}

		private void cbx2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbx2.SelectedIndex;
			if (n >= 0)
			{
				int r = dataGrid1.CurrentRowIndex;
				if (r >= ds.Tables[0].Rows.Count)
				{
					object[] vs = new object[3];
					vs[0] = "";
					vs[1] = cbx1.Text;
					vs[2] = cbx2.Items[n].ToString();
					ds.Tables[0].Rows.Add(vs);
				}
				else
				{
					ds.Tables[0].Rows[r][2] = cbx2.Items[n].ToString();
				}
			}
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			cbx0.Visible = false;
			cbx1.Visible = false;
			cbx2.Visible = false;
			System.Windows.Forms.ComboBox cbx = null;
			if (dataGrid1.CurrentCell.ColumnNumber == 0)
				cbx = cbx0;
			if (dataGrid1.CurrentCell.ColumnNumber == 1)
				cbx = cbx1;
			if (dataGrid1.CurrentCell.ColumnNumber == 2)
				cbx = cbx2;
			if (cbx != null)
			{
				System.Drawing.Rectangle rc = dataGrid1.GetCurrentCellBounds();
				cbx.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
				cbx.SelectedIndex = -1;
				if (dataGrid1.CurrentCell.RowNumber < ds.Tables[0].Rows.Count)
				{
					if (dataGrid1.CurrentCell.ColumnNumber < ds.Tables[0].Columns.Count)
					{
						cbx.Text = ds.Tables[0].Rows[dataGrid1.CurrentCell.RowNumber][dataGrid1.CurrentCell.ColumnNumber].ToString();
					}
				}
				SetDropdownWidth(cbx);
				cbx.Visible = true;
				cbx.BringToFront();
			}
		}

		private void dataGrid1_Scroll(object sender, System.EventArgs e)
		{
			cbx0.Visible = false;
			cbx1.Visible = false;
			cbx2.Visible = false;
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool bOK = false;
				EPJoin J;
				join = new TableRelation();
				if (rdoInner.Checked)
					join.Relation = enmTableRelation.INNER;
				else if (radioButton1.Checked)
					join.Relation = enmTableRelation.LEFT;
				else
				{
					//no join
					join = new TableRelation();
					join.Relation = enmTableRelation.NONE;
					join.Table2 = lblTable.Text;
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
					return;
				}
				string s;
				EPField fld = null;
				TableAlias tbl;
				for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					bOK = false;
					J = new EPJoin();
					s = ds.Tables[0].Rows[i][0].ToString();
					if (s.Length > 0)
					{
						if (srcType == enumRecSource.Table)
							fld = newTable.FindField(s);
						else if (srcType == enumRecSource.View)
							fld = newView.FindField(s);
						if (fld != null)
						{
							J.field2 = (EPField)fld.Clone();
							J.field2.FromTableName = lblTable.Text;
							s = ds.Tables[0].Rows[i][1].ToString();
							tbl = parser.query.T[s];
							if (tbl != null)
							{
								s = ds.Tables[0].Rows[i][2].ToString();
								J.field1 = tbl.FindField(s);
								if (J.field1 != null)
								{
									J.field1.FromTableName = tbl.TableName;
									join.AddJoin(J);
									bOK = true;
								}
							}
						}
					}
					if (!bOK)
						break;
				}
				if (!bOK)
					MessageBox.Show(this, Resource1.addT3, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				else if (join.FieldCount <= 0)
					MessageBox.Show(this, Resource1.addT4, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				else
				{
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
			}
			catch (Exception er)
			{
				MessageBox.Show(this, er.Message, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
		}

		private void dlgAddTableToQuery_Activated(object sender, System.EventArgs e)
		{
			dataGrid1_CurrentCellChanged(null, null);
		}

		private void radioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioButton2.Checked)
				dataGrid1.Visible = false;
		}

		private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioButton1.Checked)
				dataGrid1.Visible = true;
		}

		private void rdoInner_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rdoInner.Checked)
				dataGrid1.Visible = false;
		}


	}
}
