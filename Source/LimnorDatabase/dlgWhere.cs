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
	/// Summary description for dlgWhere.
	/// </summary>
	public class dlgWhere : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxTable;
		private System.Windows.Forms.ComboBox cbxField;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rdoField;
		private System.Windows.Forms.RadioButton rdoParam;
		private System.Windows.Forms.RadioButton rdoConst;
		private System.Windows.Forms.ComboBox cbxValField;
		private System.Windows.Forms.ComboBox cbxValTable;
		private System.Windows.Forms.Label lblValField;
		private System.Windows.Forms.Label lblValTable;
		private System.Windows.Forms.Label lblParam;
		private System.Windows.Forms.TextBox txtParam;
		private System.Windows.Forms.TextBox txtConst;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btAddAND;
		private System.Windows.Forms.Button btAddOR;
		private System.Windows.Forms.TextBox txtWhere;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Label label4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox lstOP;
		//
		QueryParser qParser;
		//===save old values so that we can restore them when cancel===
		EasyQuery oldQuery;
		ParameterList oldParameters;
		//=============================================================
		bool bChanged = false;
		private System.Windows.Forms.Button btOK;
		FilterOperatorList opList;
		//
		public dlgWhere()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//

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
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxField = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxTable = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lstOP = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtConst = new System.Windows.Forms.TextBox();
			this.txtParam = new System.Windows.Forms.TextBox();
			this.lblParam = new System.Windows.Forms.Label();
			this.cbxValField = new System.Windows.Forms.ComboBox();
			this.lblValField = new System.Windows.Forms.Label();
			this.cbxValTable = new System.Windows.Forms.ComboBox();
			this.lblValTable = new System.Windows.Forms.Label();
			this.rdoConst = new System.Windows.Forms.RadioButton();
			this.rdoParam = new System.Windows.Forms.RadioButton();
			this.rdoField = new System.Windows.Forms.RadioButton();
			this.btAdd = new System.Windows.Forms.Button();
			this.btAddAND = new System.Windows.Forms.Button();
			this.btAddOR = new System.Windows.Forms.Button();
			this.txtWhere = new System.Windows.Forms.TextBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label3.Location = new System.Drawing.Point(0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(576, 48);
			this.label3.TabIndex = 7;
			this.label3.Tag = "1";
			this.label3.Text = "Query Builder - Add filters";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxField);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbxTable);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 56);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Tag = "2";
			this.groupBox1.Text = "Select field";
			// 
			// cbxField
			// 
			this.cbxField.Location = new System.Drawing.Point(72, 56);
			this.cbxField.Name = "cbxField";
			this.cbxField.Size = new System.Drawing.Size(121, 21);
			this.cbxField.TabIndex = 3;
			this.cbxField.SelectedIndexChanged += new System.EventHandler(this.cbxField_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 2;
			this.label2.Tag = "4";
			this.label2.Text = "Field:";
			// 
			// cbxTable
			// 
			this.cbxTable.Location = new System.Drawing.Point(72, 24);
			this.cbxTable.Name = "cbxTable";
			this.cbxTable.Size = new System.Drawing.Size(121, 21);
			this.cbxTable.TabIndex = 1;
			this.cbxTable.SelectedIndexChanged += new System.EventHandler(this.cbxTable_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Tag = "3";
			this.label1.Text = "Table:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lstOP);
			this.groupBox2.Location = new System.Drawing.Point(216, 56);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(112, 100);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Tag = "5";
			this.groupBox2.Text = "Operator";
			// 
			// lstOP
			// 
			this.lstOP.Location = new System.Drawing.Point(8, 24);
			this.lstOP.Name = "lstOP";
			this.lstOP.Size = new System.Drawing.Size(96, 69);
			this.lstOP.TabIndex = 0;
			this.lstOP.SelectedIndexChanged += new System.EventHandler(this.lstOP_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtConst);
			this.groupBox3.Controls.Add(this.txtParam);
			this.groupBox3.Controls.Add(this.lblParam);
			this.groupBox3.Controls.Add(this.cbxValField);
			this.groupBox3.Controls.Add(this.lblValField);
			this.groupBox3.Controls.Add(this.cbxValTable);
			this.groupBox3.Controls.Add(this.lblValTable);
			this.groupBox3.Controls.Add(this.rdoConst);
			this.groupBox3.Controls.Add(this.rdoParam);
			this.groupBox3.Controls.Add(this.rdoField);
			this.groupBox3.Location = new System.Drawing.Point(336, 56);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(240, 100);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Tag = "6";
			this.groupBox3.Text = "Value";
			// 
			// txtConst
			// 
			this.txtConst.Location = new System.Drawing.Point(16, 56);
			this.txtConst.Name = "txtConst";
			this.txtConst.Size = new System.Drawing.Size(192, 20);
			this.txtConst.TabIndex = 10;
			// 
			// txtParam
			// 
			this.txtParam.Location = new System.Drawing.Point(88, 56);
			this.txtParam.Name = "txtParam";
			this.txtParam.Size = new System.Drawing.Size(120, 20);
			this.txtParam.TabIndex = 9;
			this.txtParam.Visible = false;
			// 
			// lblParam
			// 
			this.lblParam.AutoSize = true;
			this.lblParam.Location = new System.Drawing.Point(16, 56);
			this.lblParam.Name = "lblParam";
			this.lblParam.Size = new System.Drawing.Size(58, 13);
			this.lblParam.TabIndex = 8;
			this.lblParam.Tag = "12";
			this.lblParam.Text = "Name:   @";
			this.lblParam.Visible = false;
			// 
			// cbxValField
			// 
			this.cbxValField.Location = new System.Drawing.Point(88, 72);
			this.cbxValField.Name = "cbxValField";
			this.cbxValField.Size = new System.Drawing.Size(121, 21);
			this.cbxValField.TabIndex = 7;
			this.cbxValField.Visible = false;
			// 
			// lblValField
			// 
			this.lblValField.Location = new System.Drawing.Point(32, 72);
			this.lblValField.Name = "lblValField";
			this.lblValField.Size = new System.Drawing.Size(48, 16);
			this.lblValField.TabIndex = 6;
			this.lblValField.Tag = "11";
			this.lblValField.Text = "Field:";
			this.lblValField.Visible = false;
			// 
			// cbxValTable
			// 
			this.cbxValTable.Location = new System.Drawing.Point(88, 48);
			this.cbxValTable.Name = "cbxValTable";
			this.cbxValTable.Size = new System.Drawing.Size(121, 21);
			this.cbxValTable.TabIndex = 5;
			this.cbxValTable.Visible = false;
			this.cbxValTable.SelectedIndexChanged += new System.EventHandler(this.cbxValTable_SelectedIndexChanged);
			// 
			// lblValTable
			// 
			this.lblValTable.Location = new System.Drawing.Point(32, 48);
			this.lblValTable.Name = "lblValTable";
			this.lblValTable.Size = new System.Drawing.Size(48, 16);
			this.lblValTable.TabIndex = 4;
			this.lblValTable.Tag = "10";
			this.lblValTable.Text = "Table:";
			this.lblValTable.Visible = false;
			// 
			// rdoConst
			// 
			this.rdoConst.Checked = true;
			this.rdoConst.Location = new System.Drawing.Point(160, 16);
			this.rdoConst.Name = "rdoConst";
			this.rdoConst.Size = new System.Drawing.Size(64, 24);
			this.rdoConst.TabIndex = 2;
			this.rdoConst.TabStop = true;
			this.rdoConst.Tag = "9";
			this.rdoConst.Text = "Const";
			this.rdoConst.CheckedChanged += new System.EventHandler(this.rdoConst_CheckedChanged);
			// 
			// rdoParam
			// 
			this.rdoParam.Location = new System.Drawing.Point(88, 16);
			this.rdoParam.Name = "rdoParam";
			this.rdoParam.Size = new System.Drawing.Size(72, 24);
			this.rdoParam.TabIndex = 1;
			this.rdoParam.Tag = "8";
			this.rdoParam.Text = "Param";
			this.rdoParam.CheckedChanged += new System.EventHandler(this.rdoParam_CheckedChanged);
			// 
			// rdoField
			// 
			this.rdoField.Location = new System.Drawing.Point(16, 16);
			this.rdoField.Name = "rdoField";
			this.rdoField.Size = new System.Drawing.Size(64, 24);
			this.rdoField.TabIndex = 0;
			this.rdoField.Tag = "7";
			this.rdoField.Text = "Field";
			this.rdoField.CheckedChanged += new System.EventHandler(this.rdoField_CheckedChanged);
			// 
			// btAdd
			// 
			this.btAdd.Location = new System.Drawing.Point(440, 168);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(136, 23);
			this.btAdd.TabIndex = 11;
			this.btAdd.Tag = "15";
			this.btAdd.Text = "Add filter";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btAddAND
			// 
			this.btAddAND.Location = new System.Drawing.Point(440, 168);
			this.btAddAND.Name = "btAddAND";
			this.btAddAND.Size = new System.Drawing.Size(136, 23);
			this.btAddAND.TabIndex = 12;
			this.btAddAND.Tag = "13";
			this.btAddAND.Text = "Add AND filter";
			this.btAddAND.Click += new System.EventHandler(this.btAddAND_Click);
			// 
			// btAddOR
			// 
			this.btAddOR.Location = new System.Drawing.Point(296, 168);
			this.btAddOR.Name = "btAddOR";
			this.btAddOR.Size = new System.Drawing.Size(136, 23);
			this.btAddOR.TabIndex = 13;
			this.btAddOR.Tag = "14";
			this.btAddOR.Text = "Add OR filter";
			this.btAddOR.Click += new System.EventHandler(this.btAddOR_Click);
			// 
			// txtWhere
			// 
			this.txtWhere.Location = new System.Drawing.Point(8, 200);
			this.txtWhere.Multiline = true;
			this.txtWhere.Name = "txtWhere";
			this.txtWhere.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtWhere.Size = new System.Drawing.Size(568, 80);
			this.txtWhere.TabIndex = 14;
			this.txtWhere.TextChanged += new System.EventHandler(this.txtWhere_TextChanged);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(440, 296);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(60, 23);
			this.btOK.TabIndex = 18;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(512, 296);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(60, 23);
			this.btCancel.TabIndex = 19;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 168);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(208, 23);
			this.label4.TabIndex = 20;
			this.label4.Tag = "16";
			this.label4.Text = "Filters (WHERE clause)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dlgWhere
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(586, 336);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.txtWhere);
			this.Controls.Add(this.btAddOR);
			this.Controls.Add(this.btAddAND);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgWhere";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Query builder";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgWhere_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private void loadOperators()
		{
			opList = new FilterOperatorList();
			opList.AddOp(new FilterOperator(false, "=", "", "="));
			opList.AddOp(new FilterOperator(false, ">", "", ">"));
			opList.AddOp(new FilterOperator(false, ">=", "", ">="));
			opList.AddOp(new FilterOperator(false, "<", "", "<"));
			opList.AddOp(new FilterOperator(false, "<=", "", "<="));
			opList.AddOp(new FilterOperator(false, "<>", "", "<>"));
			if (qParser.query.IsMySql)
			{
				opList.AddOp(new FilterOperator(true, "LIKE CONCAT('%',", ",'%')", Resource1.w0));
				opList.AddOp(new FilterOperator(true, "LIKE CONCAT(", ",'%')", Resource1.w1));
				opList.AddOp(new FilterOperator(true, "LIKE CONCAT('%',", ")", Resource1.w2));
			}
			else
			{
				opList.AddOp(new FilterOperator(true, "LIKE '%'+", "+'%'", Resource1.w0));
				opList.AddOp(new FilterOperator(true, "LIKE ", "+'%'", Resource1.w1));
				opList.AddOp(new FilterOperator(true, "LIKE '%'+", "", Resource1.w2));
			}


			opList.AddOp(new FilterOperator(false, "Is Null", "", "Is Null", true));
		}
		public void LoadData(QueryParser qp)
		{
			qParser = qp;
			oldQuery = (EasyQuery)qp.query.Clone();
			loadOperators();
			if (oldQuery.Parameters == null)
				oldParameters = null;
			else
				oldParameters = (ParameterList)oldQuery.Parameters.Clone();
			qParser.parameters = oldQuery.Parameters;
			cbxTable.Items.Clear();
			cbxValTable.Items.Clear();
			int i;
			DatabaseSchema schema = qParser.GetSchema();
			for (i = 0; i < qParser.query.T.Count; i++)
			{
				qParser.query.T[i].LoadSchema(schema);
				cbxTable.Items.Add(qParser.query.T[i]);
				cbxValTable.Items.Add(qParser.query.T[i]);
			}
			if (cbxTable.Items.Count > 0)
			{
				cbxTable.SelectedIndex = 0;
				cbxValTable.SelectedIndex = 0;
			}
			if (qParser.query.Where == null)
				qParser.query.Where = "";
			txtWhere.Text = qParser.query.Where;
			txtWhere.ForeColor = System.Drawing.Color.Black;
			enableButtons();
			enableValueUI();
			bChanged = false;
		}
		void addFilter(bool asAND)
		{
			string s = txtWhere.Text.Trim();
			if (s.Length > 0)
			{
				if (asAND)
					s += " AND ";
				else
					s += " OR ";
			}
			bool bOK = false;
			string sMsg = "";
			if (lstOP.SelectedIndex < 0)
				sMsg = Resource1.w3;
			else
			{
				EPField fld0 = cbxField.SelectedItem as EPField;
				if (fld0 == null)
					sMsg = Resource1.w4;
				else
				{
					EPField fld = (EPField)fld0.Clone();
					s += qParser.Sep1 + fld.FromTableName + qParser.Sep2 + "." + qParser.Sep1 + fld.Name + qParser.Sep2 + " ";
					FilterOperator op = lstOP.Items[lstOP.SelectedIndex] as FilterOperator;
					s += op.sOpStart;
					s += " ";
					if (op.StartOnly)
					{
						bOK = true;
					}
					else
					{
						string s2;
						if (rdoField.Checked)
						{
							EPField fldVal = cbxValField.SelectedItem as EPField;
							if (fldVal != null)
							{
								s += qParser.Sep1 + fldVal.FromTableName + qParser.Sep2 + "." + qParser.Sep1 + fldVal.Name + qParser.Sep2;
								bOK = true;
							}
							else
								sMsg = Resource1.w5;
						}
						else if (rdoParam.Checked)
						{
							s2 = txtParam.Text.Trim();
							if (s2.Length > 0)
							{
								fld.Name = "@" + s2;
								s += fld.Name;
								if (qParser.parameters == null)
									qParser.parameters = new ParameterList();
								qParser.parameters.AddField(fld);
								bOK = true;
							}
							else
								sMsg = Resource1.w6;
						}
						else if (rdoConst.Checked)
						{
							string sQuote = fld.ConstQuote(oldQuery.IsOleDb);
							s2 = txtConst.Text.Trim();
							if (s2.Length > 0)
							{
								s2 = s2.Replace("'", "''");
								s += sQuote;
								s += s2;
								s += sQuote;
								bOK = true;
							}
							else
								sMsg = Resource1.w7;
						}
					}
					if (bOK)
					{
						s += op.sOpEnd;
						txtWhere.Text = s;
					}
				}
			}
			if (sMsg.Length > 0)
			{
				MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
		}
		void enableButtons()
		{
			string s = txtWhere.Text.Trim();
			if (s.Length == 0)
			{
				btAddAND.Visible = false;
				btAddOR.Visible = false;
				btAdd.Visible = true;
				btOK.Enabled = true;
			}
			else
			{
				btAddAND.Visible = true;
				btAddOR.Visible = true;
				btAdd.Visible = false;
			}
		}
		void enableValueUI()
		{
			if (rdoConst.Checked)
			{
				txtConst.Visible = true;
				cbxValField.Visible = false;
				cbxValTable.Visible = false;
				lblValTable.Visible = false;
				lblValField.Visible = false;
				txtParam.Visible = false;
				lblParam.Visible = false;
			}
			else if (rdoParam.Checked)
			{
				txtConst.Visible = false;
				cbxValField.Visible = false;
				cbxValTable.Visible = false;
				lblValTable.Visible = false;
				lblValField.Visible = false;
				txtParam.Visible = true;
				lblParam.Visible = true;
			}
			else if (rdoField.Checked)
			{
				txtConst.Visible = false;
				cbxValField.Visible = true;
				cbxValTable.Visible = true;
				lblValTable.Visible = true;
				lblValField.Visible = true;
				txtParam.Visible = false;
				lblParam.Visible = false;
			}
			else
			{
				txtConst.Visible = false;
				cbxValField.Visible = false;
				cbxValTable.Visible = false;
				lblValTable.Visible = false;
				lblValField.Visible = false;
				txtParam.Visible = false;
				lblParam.Visible = false;
			}
		}
		private void cbxTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cbxField.Items.Clear();
			int n = cbxTable.SelectedIndex;
			if (n >= 0)
			{
				EPField fld;
				TableAlias tbl = cbxTable.Items[n] as TableAlias;
				for (int i = 0; i < tbl.FieldCount; i++)
				{
					fld = tbl.GetField(i);
					cbxField.Items.Add(fld);
				}
				if (cbxField.Items.Count > 0)
				{
					cbxField.SelectedIndex = 0;
				}
			}
		}

		private void txtWhere_TextChanged(object sender, System.EventArgs e)
		{
			//btOK.Enabled = false;
			enableButtons();
			txtWhere.ForeColor = System.Drawing.Color.Blue;
			bChanged = true;
		}

		private void cbxField_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstOP.Items.Clear();
			int n = cbxField.SelectedIndex;
			if (n >= 0)
			{
				int i;
				EPField fld = cbxField.Items[n] as EPField;
				if (EPField.IsString(fld.OleDbType) || EPField.IsBinary(fld.OleDbType))
				{
					for (i = 0; i < opList.Count; i++)
					{
						lstOP.Items.Add(opList[i]);
					}
				}
				else if (EPField.IsBoolean(fld.OleDbType))
				{
					lstOP.Items.Add(opList[0]);
				}
				else
				{
					for (i = 0; i < opList.Count; i++)
					{
						if (!opList[i].bStringOnly)
							lstOP.Items.Add(opList[i]);
					}
				}
				if (lstOP.Items.Count > 0)
					lstOP.SelectedIndex = 0;
			}
		}

		private void btApply_Click(object sender, System.EventArgs e)
		{
			if (bChanged)
			{
			}
		}

		private void btAddAND_Click(object sender, System.EventArgs e)
		{
			addFilter(true);
		}

		private void btAddOR_Click(object sender, System.EventArgs e)
		{
			addFilter(false);
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			addFilter(true);
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			qParser.query = oldQuery;
			qParser.parameters = oldParameters;
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void dlgWhere_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
			{
				qParser.query = oldQuery;
				qParser.parameters = oldParameters;
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			qParser.query.Where = txtWhere.Text;
			qParser.ParseWHEREclause(txtWhere.Text);
			qParser.query.Parameters = qParser.parameters;
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void rdoConst_CheckedChanged(object sender, System.EventArgs e)
		{
			this.enableValueUI();
		}

		private void rdoParam_CheckedChanged(object sender, System.EventArgs e)
		{
			this.enableValueUI();
		}

		private void rdoField_CheckedChanged(object sender, System.EventArgs e)
		{
			this.enableValueUI();
		}

		private void cbxValTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cbxValField.Items.Clear();
			int n = cbxValTable.SelectedIndex;
			if (n >= 0)
			{
				EPField fld;
				TableAlias tbl = cbxValTable.Items[n] as TableAlias;
				for (int i = 0; i < tbl.FieldCount; i++)
				{
					fld = tbl.GetField(i);
					if (!EPField.IsBinary(fld.OleDbType))
						cbxValField.Items.Add(fld);
				}
				if (cbxValField.Items.Count > 0)
				{
					cbxValField.SelectedIndex = 0;
				}
			}
		}

		private void lstOP_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstOP.SelectedIndex >= 0)
			{
				FilterOperator op = lstOP.Items[lstOP.SelectedIndex] as FilterOperator;
				txtConst.Enabled = !op.StartOnly;
			}
		}
	}

	class FilterOperatorList
	{
		FilterOperator[] ops = null;
		public FilterOperatorList()
		{
		}
		public int Count
		{
			get
			{
				if (ops == null)
					return 0;
				return ops.Length;
			}
		}
		public FilterOperator this[int Index]
		{
			get
			{
				if (ops != null)
				{
					if (Index >= 0 && Index < ops.Length)
						return ops[Index];
				}
				return null;
			}
		}
		public void AddOp(FilterOperator op)
		{
			int n;
			if (ops == null)
			{
				n = 0;
				ops = new FilterOperator[1];
			}
			else
			{
				n = ops.Length;
				FilterOperator[] a = new FilterOperator[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = ops[i];
				ops = a;
			}
			ops[n] = op;
		}
	}
	class FilterOperator
	{
		public bool bStringOnly = false;
		public bool bBoolOnly = false;
		public string sOpStart = "";
		public string sOpEnd = "";
		public string sText = "";
		public bool StartOnly = false;
		public FilterOperator()
		{
		}
		public FilterOperator(bool forString, string opBegin, string opEnd, string text)
		{
			bStringOnly = forString;
			sOpStart = opBegin;
			sOpEnd = opEnd;
			sText = text;
		}
		public FilterOperator(bool forString, string opBegin, string opEnd, string text, bool startOnly)
			: this(forString, opBegin, opEnd, text)
		{
			StartOnly = startOnly;
		}
		public override string ToString()
		{
			return sText;
		}
	}
}
