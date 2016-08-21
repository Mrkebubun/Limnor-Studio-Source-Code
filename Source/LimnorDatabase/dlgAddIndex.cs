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

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgAddIndex.
	/// </summary>
	public class dlgAddIndex : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblTable;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btNotUse;
		private System.Windows.Forms.Button btUse;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox chkUnique;
		private System.Windows.Forms.CheckBox chkPK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btDown;
		private System.Windows.Forms.Button btUP;
		//
        Connection _connection;
		//
		public TableIndex tblIndex = null;
		public dlgAddIndex()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgAddIndex));
			this.lblTable = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btNotUse = new System.Windows.Forms.Button();
			this.btUse = new System.Windows.Forms.Button();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.chkUnique = new System.Windows.Forms.CheckBox();
			this.chkPK = new System.Windows.Forms.CheckBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.btDown = new System.Windows.Forms.Button();
			this.btUP = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblTable
			// 
			this.lblTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTable.Location = new System.Drawing.Point(144, 16);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(216, 23);
			this.lblTable.TabIndex = 3;
			this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 16);
			this.label1.Name = "label1";
			this.label1.TabIndex = 2;
			this.label1.Tag = "1";
			this.label1.Text = "Table name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 48);
			this.label2.Name = "label2";
			this.label2.TabIndex = 4;
			this.label2.Tag = "2";
			this.label2.Text = "Index name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(144, 48);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(216, 20);
			this.txtName.TabIndex = 5;
			this.txtName.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 88);
			this.label3.Name = "label3";
			this.label3.TabIndex = 6;
			this.label3.Tag = "3";
			this.label3.Text = "Table fields";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(32, 112);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(120, 95);
			this.listBox1.TabIndex = 7;
			// 
			// btNotUse
			// 
			this.btNotUse.Image = ((System.Drawing.Image)(resources.GetObject("btNotUse.Image")));
			this.btNotUse.Location = new System.Drawing.Point(176, 160);
			this.btNotUse.Name = "btNotUse";
			this.btNotUse.Size = new System.Drawing.Size(40, 32);
			this.btNotUse.TabIndex = 9;
			this.btNotUse.Click += new System.EventHandler(this.btNotUse_Click);
			// 
			// btUse
			// 
			this.btUse.Image = ((System.Drawing.Image)(resources.GetObject("btUse.Image")));
			this.btUse.Location = new System.Drawing.Point(176, 120);
			this.btUse.Name = "btUse";
			this.btUse.Size = new System.Drawing.Size(40, 32);
			this.btUse.TabIndex = 8;
			this.btUse.Click += new System.EventHandler(this.btUse_Click);
			// 
			// listBox2
			// 
			this.listBox2.Location = new System.Drawing.Point(240, 112);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(120, 95);
			this.listBox2.TabIndex = 10;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(240, 88);
			this.label4.Name = "label4";
			this.label4.TabIndex = 11;
			this.label4.Tag = "4";
			this.label4.Text = "Index fields";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkUnique
			// 
			this.chkUnique.Location = new System.Drawing.Point(208, 216);
			this.chkUnique.Name = "chkUnique";
			this.chkUnique.TabIndex = 13;
			this.chkUnique.Tag = "6";
			this.chkUnique.Text = "Unique";
			// 
			// chkPK
			// 
			this.chkPK.Location = new System.Drawing.Point(88, 216);
			this.chkPK.Name = "chkPK";
			this.chkPK.TabIndex = 12;
			this.chkPK.Tag = "5";
			this.chkPK.Text = "Primary key";
			this.chkPK.CheckedChanged += new System.EventHandler(this.chkPK_CheckedChanged);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(288, 256);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 15;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(192, 256);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 14;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btDown
			// 
			this.btDown.Image = ((System.Drawing.Image)(resources.GetObject("btDown.Image")));
			this.btDown.Location = new System.Drawing.Point(376, 144);
			this.btDown.Name = "btDown";
			this.btDown.Size = new System.Drawing.Size(40, 24);
			this.btDown.TabIndex = 17;
			this.btDown.Click += new System.EventHandler(this.btDown_Click);
			// 
			// btUP
			// 
			this.btUP.Image = ((System.Drawing.Image)(resources.GetObject("btUP.Image")));
			this.btUP.Location = new System.Drawing.Point(376, 112);
			this.btUP.Name = "btUP";
			this.btUP.Size = new System.Drawing.Size(40, 24);
			this.btUP.TabIndex = 16;
			this.btUP.Click += new System.EventHandler(this.btUP_Click);
			// 
			// dlgAddIndex
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(434, 296);
			this.Controls.Add(this.btDown);
			this.Controls.Add(this.btUP);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.chkUnique);
			this.Controls.Add(this.chkPK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listBox2);
			this.Controls.Add(this.btNotUse);
			this.Controls.Add(this.btUse);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblTable);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgAddIndex";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Index";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(string sTable,FieldList flds,bool bHasPK, Connection cn)
		{
			_connection = cn;
			lblTable.Text = sTable;
			listBox1.Items.Clear();
			for(int i=0;i<flds.Count;i++)
			{
				listBox1.Items.Add(flds[i]);
			}
			chkPK.Checked = false;
			chkPK.Enabled = !bHasPK;
		}
		private void btUse_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if( n >= 0 )
			{
				EPField fld = (EPField)listBox1.Items[n];
				listBox1.Items.RemoveAt(n);
				listBox2.Items.Add(fld);
			}
		}

		private void btNotUse_Click(object sender, System.EventArgs e)
		{
			int n = listBox2.SelectedIndex;
			if( n >= 0 )
			{
				EPField fld = (EPField)listBox2.Items[n];
				listBox2.Items.RemoveAt(n);
				listBox1.Items.Add(fld);
			}
		}

		private void btUP_Click(object sender, System.EventArgs e)
		{
			int n = listBox2.SelectedIndex;
			if( n > 0 )
			{
				EPField fld = (EPField)listBox2.Items[n];
				listBox2.Items.RemoveAt(n);
				n--;
				listBox2.Items.Insert(n,fld);
			}
		}

		private void btDown_Click(object sender, System.EventArgs e)
		{
			int n = listBox2.SelectedIndex;
			if( n >= 0 && n < listBox2.Items.Count-1 )
			{
				EPField fld = (EPField)listBox2.Items[n];
				listBox2.Items.RemoveAt(n);
				n++;
				listBox2.Items.Insert(n,fld);
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string sName = txtName.Text.Trim();
			sName = System.Text.RegularExpressions.Regex.Replace(sName,"~[a-zA-Z~_]","");
			if(sName.Length == 0 )
			{
				MessageBox.Show(this,Resource1.indexNameMissing,this.Text,System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
			else if( listBox2.Items.Count == 0 )
			{
                MessageBox.Show(this, Resource1.SelFlds, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
			else
			{
				try
				{
                    string sep1 = _connection.NameDelimiterBegin;
					string sep2= _connection.NameDelimiterEnd;
					tblIndex = new TableIndex();
					tblIndex.IndexName = sName;
					tblIndex.fields = new FieldList();
					StringBuilder sSQL = new StringBuilder();
					if( chkPK.Checked )
					{
						tblIndex.IsUnique = true;
						tblIndex.IsPrimaryKey = true;
                        sSQL.Append("ALTER TABLE ");
                        sSQL.Append(sep1);
                        sSQL.Append(lblTable.Text);
                        sSQL.Append(sep2);
						if(_connection.IsOdbc)
						{
                            tblIndex.IndexName = "PRIMARY";
                            sSQL.Append(" ADD PRIMARY KEY ");
                            sSQL.Append(" (");
                            sSQL.Append(sep1);
						}
						else 
						{
                            sSQL.Append(" ADD CONSTRAINT ");
                            sSQL.Append(sep1);
                            sSQL.Append(sName);
                            sSQL.Append(sep2);
                            sSQL.Append(" PRIMARY KEY (");
                            sSQL.Append(sep1);
						}
					}
					else
					{
						sSQL.Append("CREATE ");
						if( chkUnique.Checked )
						{
							tblIndex.IsUnique = true;
							sSQL.Append(" UNIQUE ");
						}
                        sSQL.Append("INDEX ");
                        sSQL.Append(sep1);
                        sSQL.Append(sName);
                        sSQL.Append(sep2);
                        sSQL.Append(" ON ");
                        sSQL.Append(sep1);
                        sSQL.Append(lblTable.Text);
                        sSQL.Append(sep2);
                        sSQL.Append(" (");
                        sSQL.Append(sep1);
					}
					EPField fld = listBox2.Items[0] as EPField;
					sSQL.Append(fld.Name);
					sSQL.Append(sep2);
					tblIndex.fields.AddField(fld);
					for(int i=1;i<listBox2.Items.Count;i++)
					{
						fld = listBox2.Items[i] as EPField;
                        sSQL.Append(",");
                        sSQL.Append(sep1);
                        sSQL.Append(fld.Name);
						sSQL.Append(sep2);
						tblIndex.fields.AddField(fld);
					}
					sSQL.Append(")");
                    try
                    {
                        _connection.ExecuteNonQuery(sSQL.ToString());
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(this, err.Message, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    }
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
				catch(Exception er)
				{
					MessageBox.Show(this,er.Message,this.Text,System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
			}
		}
		private void chkPK_CheckedChanged(object sender, System.EventArgs e)
		{
			if( chkPK.Checked )
			{
				chkUnique.Checked = true;
				chkUnique.Enabled = false;
				txtName.ReadOnly = (_connection.IsOdbc);
			}
			else
			{
				chkUnique.Enabled = true;
				txtName.ReadOnly = false;
			}
		}


	}
}
