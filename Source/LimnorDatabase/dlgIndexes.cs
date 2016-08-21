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
	/// Summary description for dlgIndexes.
	/// </summary>
	public class dlgIndexes : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblTable;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.CheckBox chkPK;
		private System.Windows.Forms.CheckBox chkUnique;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDel;
		private System.Windows.Forms.Button btFinish;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DatabaseTable tblNode = null;

		Connection _connection;
		//
		public bool bChanged = false;
		//
		public dlgIndexes()
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
			this.label1 = new System.Windows.Forms.Label();
			this.lblTable = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.chkPK = new System.Windows.Forms.CheckBox();
			this.chkUnique = new System.Windows.Forms.CheckBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDel = new System.Windows.Forms.Button();
			this.btFinish = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Table name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblTable
			// 
			this.lblTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTable.Location = new System.Drawing.Point(136, 24);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(216, 23);
			this.lblTable.TabIndex = 1;
			this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 64);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "Indexes";
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(16, 88);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(136, 95);
			this.listBox1.TabIndex = 3;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// chkPK
			// 
			this.chkPK.Enabled = false;
			this.chkPK.Location = new System.Drawing.Point(16, 184);
			this.chkPK.Name = "chkPK";
			this.chkPK.TabIndex = 4;
			this.chkPK.Tag = "4";
			this.chkPK.Text = "Primary key";
			// 
			// chkUnique
			// 
			this.chkUnique.Enabled = false;
			this.chkUnique.Location = new System.Drawing.Point(16, 208);
			this.chkUnique.Name = "chkUnique";
			this.chkUnique.TabIndex = 5;
			this.chkUnique.Tag = "5";
			this.chkUnique.Text = "Unique";
			// 
			// listBox2
			// 
			this.listBox2.Location = new System.Drawing.Point(176, 88);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(176, 95);
			this.listBox2.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(176, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(144, 23);
			this.label3.TabIndex = 7;
			this.label3.Tag = "3";
			this.label3.Text = "Fields";
			// 
			// btAdd
			// 
			this.btAdd.Location = new System.Drawing.Point(16, 240);
			this.btAdd.Name = "btAdd";
			this.btAdd.TabIndex = 8;
			this.btAdd.Text = "Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDel
			// 
			this.btDel.Location = new System.Drawing.Point(96, 240);
			this.btDel.Name = "btDel";
			this.btDel.TabIndex = 9;
			this.btDel.Text = "Delete";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			// 
			// btFinish
			// 
			this.btFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btFinish.Location = new System.Drawing.Point(272, 240);
			this.btFinish.Name = "btFinish";
			this.btFinish.TabIndex = 10;
			this.btFinish.Text = "Finish";
			// 
			// dlgIndexes
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(370, 288);
			this.Controls.Add(this.btFinish);
			this.Controls.Add(this.btDel);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listBox2);
			this.Controls.Add(this.chkUnique);
			this.Controls.Add(this.chkPK);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblTable);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgIndexes";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Indexes";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DatabaseTable tbl, Connection cn)
		{
			tblNode = tbl;
			_connection = cn;
			lblTable.Text = tblNode.TableName;
			listBox1.Items.Clear();
			if (tblNode.Indexes != null)
			{
				for (int i = 0; i < tblNode.Indexes.Length; i++)
				{
					listBox1.Items.Add(tblNode.Indexes[i]);
				}
			}
			btDel.Enabled = (listBox1.Items.Count > 0);
			if (tblNode.fields == null)
				btAdd.Enabled = false;
			else
				btAdd.Enabled = (tblNode.fields.Count > 0);
		}
		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			listBox2.Items.Clear();
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				TableIndex tbl = listBox1.Items[n] as TableIndex;
				chkPK.Checked = tbl.IsPrimaryKey;
				chkUnique.Checked = tbl.IsUnique;
				if (tbl.fields != null)
				{
					for (int i = 0; i < tbl.fields.Count; i++)
					{
						listBox2.Items.Add(tbl.fields[i].Name);
					}
				}
			}
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			dlgAddIndex dlg = new dlgAddIndex();
			dlg.LoadData(lblTable.Text, tblNode.fields, tblNode.HasPrimaryKey(), _connection);
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				listBox1.Items.Add(dlg.tblIndex);
				btDel.Enabled = true;
				bChanged = true;
			}
		}

		private void btDel_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				string sep1 = _connection.NameDelimiterBegin;
				string sep2 = _connection.NameDelimiterEnd;
				TableIndex tbl = listBox1.Items[n] as TableIndex;
				string sSQL = "DROP INDEX ";
				if (!_connection.IsOdbc || !chkPK.Checked)
				{
					sSQL += sep1 + tbl.IndexName + sep2;
					sSQL += " ON " + sep1;
					sSQL += lblTable.Text;
					sSQL += sep2;
				}
				else
				{
					sSQL = " ALTER Table ";
					sSQL += sep1 + lblTable.Text + sep2;
					sSQL += " DROP PRIMARY KEY";
				}

				try
				{
					_connection.ExecuteNonQuery(sSQL);
					listBox1.Items.RemoveAt(n);
					btDel.Enabled = (listBox1.Items.Count > 0);
					bChanged = true;
				}
				catch (Exception er)
				{
					MessageBox.Show(this, er.Message, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}
		}
	}
}
