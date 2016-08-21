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
using System.Collections.Specialized;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgQryGroup.
	/// </summary>
	public class dlgQryGroup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox lstOp;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		//
		public EasyQuery qry;
		//QueryParser qParser;
		FieldsParser groupFields;
		public dlgQryGroup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			listView1.Columns[0].Text = Resource1.Field;
			listView1.Columns[1].Text = Resource1.Operation;
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
			this.label1 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lstOp = new System.Windows.Forms.ListBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label1.Location = new System.Drawing.Point(1, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(465, 32);
			this.label1.TabIndex = 1;
			this.label1.Tag = "1";
			this.label1.Text = "Make summarizing query";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(24, 64);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(296, 144);
			this.listView1.TabIndex = 2;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Field";
			this.columnHeader1.Width = 166;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Operation";
			this.columnHeader2.Width = 120;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 3;
			this.label2.Tag = "2";
			this.label2.Text = "Query fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(328, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 4;
			this.label3.Tag = "3";
			this.label3.Text = "Aggregate operations";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lstOp
			// 
			this.lstOp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstOp.Items.AddRange(new object[] {
            "",
            "Group by",
            "SUM",
            "MAX",
            "MIN",
            "AVG",
            "COUNT",
            "STDEV",
            "STDEVP",
            "VAR",
            "VARP"});
			this.lstOp.Location = new System.Drawing.Point(328, 64);
			this.lstOp.Name = "lstOp";
			this.lstOp.Size = new System.Drawing.Size(120, 147);
			this.lstOp.TabIndex = 5;
			this.lstOp.SelectedIndexChanged += new System.EventHandler(this.lstOp_SelectedIndexChanged);
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(111, 231);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btOK.Location = new System.Drawing.Point(23, 231);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 11;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// dlgQryGroup
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(466, 266);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lstOp);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgQryGroup";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Group by";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(QueryParser q)
		{
			int i;
			//qParser = q;
			qry = (EasyQuery)q.query.Clone();
			//parse group by
			groupFields = new FieldsParser();
			//=============================================
			string s;
			string sGroupBy = qry.GroupBy;
			if (!string.IsNullOrEmpty(sGroupBy))
			{
				sGroupBy = sGroupBy.Trim();
				if (sGroupBy.StartsWith("GROUP BY ", StringComparison.OrdinalIgnoreCase))
				{
					sGroupBy = sGroupBy.Substring(9);
					sGroupBy = sGroupBy.Trim();
				}
				i = FieldsParser.FindStringIndex(sGroupBy, ",", 0, q.Sep1, q.Sep2);
				while (sGroupBy.Length > 0)
				{
					if (i < 0)
					{
						s = sGroupBy;
						sGroupBy = "";
					}
					else
					{
						s = sGroupBy.Substring(0, i);
						sGroupBy = sGroupBy.Substring(i + 1);
						sGroupBy = sGroupBy.Trim();
					}
					s = s.Trim();
					groupFields.AddField(new QryField(s, q.Sep1, q.Sep2));
					i = FieldsParser.FindStringIndex(sGroupBy, ",", 0, q.Sep1, q.Sep2);
				}
			}
			//=============================================
			ListViewItem item;
			listView1.Items.Clear();
			FieldList fl = qry.Fields;
			for (i = 0; i < fl.Count; i++)
			{
				item = new ListViewItem(fl[i].FieldText);
				if (groupFields.FindFieldByText(fl[i].FieldText) != null)
					item.SubItems.Add("Group by");
				else
					item.SubItems.Add("");
				listView1.Items.Add(item);
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string s;
			StringCollection scGroup = new StringCollection();
			FieldList flds = new FieldList();
			qry.GroupBy = "";
			FieldList fl = qry.Fields;
			for (int i = 0; i < fl.Count; i++)
			{
				s = listView1.Items[i].SubItems[1].Text.Trim();
				if (!string.IsNullOrEmpty(s))
				{
					if (string.Compare(s, "Group by", StringComparison.OrdinalIgnoreCase) == 0)
					{
						scGroup.Add(fl[i].GetFieldTextAsValue(qry.NameDelimiterBegin, qry.NameDelimiterEnd));
						flds.Add(fl[i]);
					}
					else
					{
						EPField f = (EPField)fl[i].Clone();
						f.FieldText = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}({1}) AS {2}{3}{4}", s, f.GetFieldTextAsValue(qry.NameDelimiterBegin, qry.NameDelimiterEnd), qry.NameDelimiterBegin, f.Name, qry.NameDelimiterEnd);
						flds.Add(f);
					}
				}
			}
			if (scGroup.Count == 0)
			{
				MessageBox.Show(this, Resource1.Err_group_field, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
			else
			{
				StringBuilder sg = new StringBuilder(scGroup[0]);
				for (int i = 1; i < scGroup.Count; i++)
				{
					sg.Append(",");
					sg.Append(scGroup[i]);
				}
				qry.GroupBy = sg.ToString();
				qry.Fields = flds;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void lstOp_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = lstOp.SelectedIndex;
			if (n >= 0 && listView1.SelectedIndices.Count > 0)
			{
				int m = listView1.SelectedIndices[0];
				listView1.Items[m].SubItems[1].Text = lstOp.Text;
			}
		}
	}
}
