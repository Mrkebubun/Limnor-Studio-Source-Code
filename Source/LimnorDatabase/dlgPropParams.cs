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
	/// Summary description for dlgPropParams.
	/// </summary>
	public class dlgPropParams : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.TextBox txtSize;
		private DbTypeComboBox cbType;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		ListViewItem item = null;
		public FieldList fields = null;
		//
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.TextBox txtDef;
		private System.Windows.Forms.Label label2;
		//
		public dlgPropParams()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbType = new DbTypeComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.txtSize = new System.Windows.Forms.TextBox();
			this.txtDef = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(215, 254);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 21;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(119, 254);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 20;
			this.btOK.Text = "OK";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(368, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 19;
			this.label4.Tag = "3";
			this.label4.Text = "Data size:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(368, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 17;
			this.label3.Tag = "2";
			this.label3.Text = "Data type";
			// 
			// cbType
			// 
			//this.cbType.Items.AddRange(new object[] {
			//                                            "Integer",
			//                                            "Decimal",
			//                                            "String",
			//                                            "DateTime",
			//                                            "Yes/No",
			//                                            "Large memory",
			//                                            "Binary"});
			/*
			 String
Integer
Long integer
Decimal
Currency
Date
Time
Date and time
Yes/No
Large text
Large binary
			 */
			this.cbType.Location = new System.Drawing.Point(368, 64);
			this.cbType.Name = "cbType";
			this.cbType.Size = new System.Drawing.Size(96, 21);
			this.cbType.TabIndex = 16;
			this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(24, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(360, 23);
			this.label1.TabIndex = 15;
			this.label1.Tag = "1";
			this.label1.Text = "Set parameter attributes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(24, 40);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(312, 192);
			this.listView1.TabIndex = 14;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "FieldName";
			this.columnHeader1.Width = 80;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "DataType";
			this.columnHeader2.Width = 71;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "DataSize";
			this.columnHeader3.Width = 69;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Default value";
			this.columnHeader4.Width = 86;
			// 
			// txtSize
			// 
			this.txtSize.Location = new System.Drawing.Point(368, 112);
			this.txtSize.Name = "txtSize";
			this.txtSize.Size = new System.Drawing.Size(88, 20);
			this.txtSize.TabIndex = 22;
			this.txtSize.Text = "8";
			this.txtSize.TextChanged += new System.EventHandler(this.txtSize_TextChanged);
			// 
			// txtDef
			// 
			this.txtDef.Location = new System.Drawing.Point(368, 160);
			this.txtDef.Name = "txtDef";
			this.txtDef.Size = new System.Drawing.Size(88, 20);
			this.txtDef.TabIndex = 24;
			this.txtDef.Text = "";
			this.txtDef.TextChanged += new System.EventHandler(this.txtDef_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(368, 144);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 23;
			this.label2.Tag = "4";
			this.label2.Text = "Default value:";
			// 
			// dlgPropParams
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 288);
			this.Controls.Add(this.txtDef);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtSize);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cbType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropParams";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Parameters Attributes";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(FieldList flds)
		{
			fields = (FieldList)flds.Clone();
			ListViewItem v;
			for (int i = 0; i < fields.Count; i++)
			{
				v = new ListViewItem(fields[i].Name);
				v.SubItems.Add(EPField.TypeString(fields[i].OleDbType));
				v.SubItems.Add(fields[i].DataSize.ToString());
				if (fields[i].Value == null)
					v.SubItems.Add("");
				else
					v.SubItems.Add(fields[i].Value.ToString());
				listView1.Items.Add(v);
			}
			if (listView1.Items.Count > 0)
				listView1.Items[0].Selected = true;
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				item = listView1.SelectedItems[0];
				if (item != null)
				{
					cbType.SetSelectionByOleDbType(fields[item.Index].OleDbType);
					txtSize.Text = fields[item.Index].DataSize.ToString();
					if (fields[item.Index].Value != null)
						txtDef.Text = fields[item.Index].Value.ToString();
					else
						txtDef.Text = "";
				}
			}
		}

		private void cbType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				if (cbType.SelectedIndex >= 0)
				{
					txtSize.ReadOnly = !cbType.CurrentSelectAllowChangeSize();
					int n = cbType.CurrentSelectDataSize();
					if (n == 0)
					{
						txtSize.Text = "";
					}
					else
					{
						txtSize.Text = n.ToString();
					}
					fields[item.Index].OleDbType = cbType.CurrentSelectOleDbType();
				}
				item.SubItems[1].Text = cbType.Text;
			}
		}

		private void txtSize_TextChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				try
				{
					int d = Convert.ToInt32(txtSize.Text);
					if (d > 0)
					{
						item.SubItems[2].Text = d.ToString();
						fields[item.Index].DataSize = d;
					}
				}
				catch
				{
				}
			}
		}

		private void txtDef_TextChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				try
				{
					item.SubItems[3].Text = txtDef.Text;
					fields[item.Index].SetValue(EPField.StringToTypedValue(fields[item.Index].OleDbType, txtDef.Text));
					if (fields[item.Index].Value == null)
						item.SubItems[3].Text = "";
					else
						item.SubItems[3].Text = fields[item.Index].Value.ToString();
				}
				catch
				{
				}
			}
		}
	}
}
