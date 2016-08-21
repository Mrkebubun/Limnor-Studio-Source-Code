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
	/// Summary description for dlgQryOrder.
	/// </summary>
	public class dlgQryOrder : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckedListBox clsOrder;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDel;
		private System.Windows.Forms.Button btUp;
		private System.Windows.Forms.Button btDn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.CheckBox chkDesc;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox lstFields;
		//
		QueryParser qParser;
		public dlgQryOrder()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgQryOrder));
			this.label1 = new System.Windows.Forms.Label();
			this.clsOrder = new System.Windows.Forms.CheckedListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDel = new System.Windows.Forms.Button();
			this.btUp = new System.Windows.Forms.Button();
			this.btDn = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.chkDesc = new System.Windows.Forms.CheckBox();
			this.lstFields = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label1.Location = new System.Drawing.Point(-24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(552, 32);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Set query order";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// clsOrder
			// 
			this.clsOrder.Location = new System.Drawing.Point(232, 88);
			this.clsOrder.Name = "clsOrder";
			this.clsOrder.Size = new System.Drawing.Size(240, 109);
			this.clsOrder.TabIndex = 2;
			this.clsOrder.SelectedIndexChanged += new System.EventHandler(this.clsOrder_SelectedIndexChanged);
			this.clsOrder.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clsOrder_ItemCheck);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 23);
			this.label2.TabIndex = 3;
			this.label2.Tag = "2";
			this.label2.Text = "Available fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btAdd
			// 
			this.btAdd.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.Image")));
			this.btAdd.Location = new System.Drawing.Point(168, 96);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(48, 32);
			this.btAdd.TabIndex = 4;
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDel
			// 
			this.btDel.Image = ((System.Drawing.Image)(resources.GetObject("btDel.Image")));
			this.btDel.Location = new System.Drawing.Point(168, 144);
			this.btDel.Name = "btDel";
			this.btDel.Size = new System.Drawing.Size(48, 32);
			this.btDel.TabIndex = 5;
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			// 
			// btUp
			// 
			this.btUp.Image = ((System.Drawing.Image)(resources.GetObject("btUp.Image")));
			this.btUp.Location = new System.Drawing.Point(480, 88);
			this.btUp.Name = "btUp";
			this.btUp.Size = new System.Drawing.Size(40, 32);
			this.btUp.TabIndex = 6;
			this.btUp.Click += new System.EventHandler(this.btUp_Click);
			// 
			// btDn
			// 
			this.btDn.Image = ((System.Drawing.Image)(resources.GetObject("btDn.Image")));
			this.btDn.Location = new System.Drawing.Point(480, 120);
			this.btDn.Name = "btDn";
			this.btDn.Size = new System.Drawing.Size(40, 32);
			this.btDn.TabIndex = 7;
			this.btDn.Click += new System.EventHandler(this.btDn_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(232, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 23);
			this.label3.TabIndex = 8;
			this.label3.Tag = "3";
			this.label3.Text = "Sorting order";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(232, 256);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 9;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(320, 256);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 10;
			this.btCancel.Text = "Cancel";
			// 
			// chkDesc
			// 
			this.chkDesc.Location = new System.Drawing.Point(232, 200);
			this.chkDesc.Name = "chkDesc";
			this.chkDesc.TabIndex = 11;
			this.chkDesc.Tag = "4";
			this.chkDesc.Text = "Descending";
			this.chkDesc.CheckedChanged += new System.EventHandler(this.chkDesc_CheckedChanged);
			// 
			// lstFields
			// 
			this.lstFields.Location = new System.Drawing.Point(8, 88);
			this.lstFields.Name = "lstFields";
			this.lstFields.Size = new System.Drawing.Size(152, 108);
			this.lstFields.TabIndex = 12;
			this.lstFields.SelectedIndexChanged += new System.EventHandler(this.lstFields_SelectedIndexChanged);
			// 
			// dlgQryOrder
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(538, 296);
			this.Controls.Add(this.lstFields);
			this.Controls.Add(this.chkDesc);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btDn);
			this.Controls.Add(this.btUp);
			this.Controls.Add(this.btDel);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.clsOrder);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgQryOrder";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Order by";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(QueryParser q)
		{
			qParser = q;
			lstFields.Items.Clear();
			EPField fld;
			int i;
			FieldList fl = q.query.Fields;
			for (i = 0; i < fl.Count; i++)
			{
				fld = fl[i];
				if (!EPField.IsBinary(fld.OleDbType))
				{
					lstFields.Items.Add(new OrderByField(fld, qParser.Sep1, qParser.Sep2));
					//lstFields.Items.Add(fld);
				}
			}
			clsOrder.Items.Clear();
			int nType;
			string s, s2;
			string sOrder = q.query.OrderBy;
			if (sOrder == null)
				sOrder = "";
			sOrder = sOrder.Trim();
			if (sOrder.StartsWith("ORDER BY ", StringComparison.OrdinalIgnoreCase))
			{
				sOrder = sOrder.Substring(9);
				sOrder = sOrder.Trim();
			}
			i = FieldsParser.FindStringIndex(sOrder, ",", 0, q.Sep1, q.Sep2);
			while (sOrder.Length > 0)
			{
				if (i < 0)
				{
					s = sOrder;
					sOrder = "";
				}
				else
				{
					s = sOrder.Substring(0, i);
					sOrder = sOrder.Substring(i + 1);
					sOrder = sOrder.Trim();
				}
				s = s.Trim();
				nType = -1;
				if (s.Length > 4)
				{
					s2 = s.Substring(s.Length - 4, 4);
					if (string.Compare(s2, " ASC", StringComparison.OrdinalIgnoreCase) == 0)
					{
						s = s.Substring(0, s.Length - 4);
						s = s.Trim();
						nType = 0;
					}
				}
				if (nType < 0)
				{
					if (s.Length > 5)
					{
						s2 = s.Substring(s.Length - 5, 5);
						if (string.Compare(s2, " DESC", StringComparison.OrdinalIgnoreCase) == 0)
						{
							s = s.Substring(0, s.Length - 5);
							s = s.Trim();
							nType = 1;
						}
					}
				}
				clsOrder.Items.Add(s, (nType == 1));
				i = FieldsParser.FindStringIndex(sOrder, ",", 0, q.Sep1, q.Sep2);
			}
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			int n = lstFields.SelectedIndex;
			if (n >= 0)
			{
				OrderByField fld = lstFields.Items[n] as OrderByField;
				bool bFound = false;
				string order = fld.ToString();
				for (int i = 0; i < clsOrder.Items.Count; i++)
				{
					if (string.Compare(order, clsOrder.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					clsOrder.Items.Add(order);
				}
			}
		}

		private void btDel_Click(object sender, System.EventArgs e)
		{
			int n = clsOrder.SelectedIndex;
			if (n >= 0)
			{
				clsOrder.Items.RemoveAt(n);
			}
		}

		private void btUp_Click(object sender, System.EventArgs e)
		{
			int n = clsOrder.SelectedIndex;
			if (n > 0)
			{
				object s = clsOrder.Items[n];
				bool b = clsOrder.GetItemChecked(n);
				clsOrder.Items.RemoveAt(n);
				clsOrder.Items.Insert(n - 1, s);
				clsOrder.SetItemChecked(n - 1, b);
				clsOrder.SelectedIndex = n - 1;
			}
		}

		private void btDn_Click(object sender, System.EventArgs e)
		{
			int n = clsOrder.SelectedIndex;
			if (n >= 0 && n < clsOrder.Items.Count - 1)
			{
				object s = clsOrder.Items[n];
				bool b = clsOrder.GetItemChecked(n);
				clsOrder.Items.RemoveAt(n);
				clsOrder.Items.Insert(n + 1, s);
				clsOrder.SetItemChecked(n + 1, b);
				clsOrder.SelectedIndex = n + 1;
			}
		}

		private void clsOrder_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = clsOrder.SelectedIndex;
			if (n >= 0 && n < clsOrder.Items.Count)
			{
				chkDesc.Checked = clsOrder.GetItemChecked(n);
			}
		}

		private void clsOrder_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			chkDesc.Checked = (e.NewValue == System.Windows.Forms.CheckState.Checked);
		}

		private void chkDesc_CheckedChanged(object sender, System.EventArgs e)
		{
			int n = clsOrder.SelectedIndex;
			if (n >= 0)
			{
				clsOrder.SetItemChecked(n, chkDesc.Checked);
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string s = "";
			if (clsOrder.Items.Count > 0)
			{
				s = clsOrder.Items[0].ToString();
				if (clsOrder.GetItemChecked(0))
					s += " DESC";
			}
			for (int i = 1; i < clsOrder.Items.Count; i++)
			{
				s += "," + clsOrder.Items[i].ToString();
				if (clsOrder.GetItemChecked(i))
					s += " DESC";
			}
			qParser.query.OrderBy = s;
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void lstFields_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = lstFields.SelectedIndex;
			if (n >= 0)
			{
			}
		}
	}

	public class OrderByField
	{
		public EPField field;
		public string Sep1 = "[";
		public string Sep2 = "]";
		public OrderByField(EPField fld, string s1, string s2)
		{
			Sep1 = s1;
			Sep2 = s2;
			field = fld;
		}
		public override string ToString()
		{
			return field.GetFieldTextAsValue(Sep1, Sep2);
		}
	}
}
