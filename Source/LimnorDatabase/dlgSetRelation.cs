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
	/// Summary description for dlgSetRelation.
	/// </summary>
	public class dlgSetRelation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DatabaseTable mainTable = null;
		private System.Windows.Forms.ComboBox cbxKey;
		private System.Windows.Forms.Label label4;
		DatabaseTable subTable = null;
		System.Windows.Forms.ComboBox cbxFK;
		Connection _connection;
		//
		private System.Windows.Forms.Label lblParent;
		private System.Windows.Forms.Label lblChild;
		//
		public dlgSetRelation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			cbxFK = new System.Windows.Forms.ComboBox();
			cbxFK.Parent = listBox2;
			cbxFK.Visible = false;
			cbxFK.SelectedIndexChanged += new System.EventHandler(this.cbxFK_SelectedIndexChanged);
			listBox1.ItemHeight = cbxFK.ItemHeight + 5;
			listBox2.ItemHeight = cbxFK.ItemHeight + 5;
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
			this.label1 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxKey = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lblParent = new System.Windows.Forms.Label();
			this.lblChild = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(48, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(304, 23);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Set table relation (one-to-many)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(272, 248);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 17;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(176, 248);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 16;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 23);
			this.label2.TabIndex = 18;
			this.label2.Tag = "2";
			this.label2.Text = "Parent table key fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(40, 112);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(152, 95);
			this.listBox1.TabIndex = 19;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// listBox2
			// 
			this.listBox2.Location = new System.Drawing.Point(192, 112);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(152, 95);
			this.listBox2.TabIndex = 21;
			this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(192, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(184, 23);
			this.label3.TabIndex = 20;
			this.label3.Tag = "3";
			this.label3.Text = "Child table foreign key fields";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbxKey
			// 
			this.cbxKey.Location = new System.Drawing.Point(96, 208);
			this.cbxKey.Name = "cbxKey";
			this.cbxKey.Size = new System.Drawing.Size(96, 21);
			this.cbxKey.TabIndex = 22;
			this.cbxKey.SelectedIndexChanged += new System.EventHandler(this.cbxKey_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 23);
			this.label4.TabIndex = 23;
			this.label4.Tag = "4";
			this.label4.Text = "Key:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblParent
			// 
			this.lblParent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblParent.Location = new System.Drawing.Point(40, 56);
			this.lblParent.Name = "lblParent";
			this.lblParent.Size = new System.Drawing.Size(144, 23);
			this.lblParent.TabIndex = 24;
			this.lblParent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblChild
			// 
			this.lblChild.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblChild.Location = new System.Drawing.Point(192, 56);
			this.lblChild.Name = "lblChild";
			this.lblChild.Size = new System.Drawing.Size(144, 23);
			this.lblChild.TabIndex = 25;
			this.lblChild.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dlgSetRelation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(410, 296);
			this.Controls.Add(this.lblChild);
			this.Controls.Add(this.lblParent);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbxKey);
			this.Controls.Add(this.listBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSetRelation";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Set table relation";
			this.ResumeLayout(false);

		}
		#endregion
		private void init(DatabaseTable mn, DatabaseTable sub)
		{
			mainTable = mn;
			subTable = sub;
			lblParent.Text = mainTable.TableName;
			lblChild.Text = subTable.TableName;
			int i;
			cbxFK.Items.Clear();
			cbxKey.Items.Clear();
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			for (i = 0; i < subTable.fields.Count; i++)
			{
				cbxFK.Items.Add(subTable.fields[i]);
			}
			for (i = 0; i < mainTable.Indexes.Length; i++)
			{
				if (mainTable.Indexes[i].IsUnique)
				{
					cbxKey.Items.Add(mainTable.Indexes[i]);
					if (mainTable.Indexes[i].IsPrimaryKey)
						cbxKey.SelectedIndex = i;
				}
			}
			if (cbxKey.SelectedIndex < 0)
				cbxKey.SelectedIndex = 0;
			cbxKey_SelectedIndexChanged(null, null);
		}
		public void LoadData(DatabaseTable mn, DatabaseTable sub, Connection cn)
		{
			_connection = cn;
			init(mn, sub);
		}
		private void cbxKey_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			cbxFK.Visible = false;
			int n = cbxKey.SelectedIndex;
			if (n >= 0)
			{
				TableIndex Index = cbxKey.Items[n] as TableIndex;
				if (Index != null)
				{
					for (int i = 0; i < Index.fields.Count; i++)
					{
						listBox1.Items.Add(Index.fields[i]);
						listBox2.Items.Add("");
					}
					listBox1.SelectedIndex = 0;
				}
			}
		}
		private void cbxFK_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbxFK.SelectedIndex;
			if (n >= 0)
			{
				int k = listBox2.SelectedIndex;
				if (k >= 0)
				{
					listBox2.Items.RemoveAt(k);
					listBox2.Items.Insert(k, cbxFK.Items[n]);
				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				listBox2.SelectedIndex = n;
				System.Drawing.Rectangle rc = listBox2.GetItemRectangle(n);
				cbxFK.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
				EPField fld = listBox2.Items[n] as EPField;
				if (fld != null)
				{
					for (int i = 0; i < subTable.fields.Count; i++)
					{
						if (fld == subTable.fields[i])
						{
							cbxFK.SelectedIndex = i;
							break;
						}
					}
				}
				cbxFK.Visible = true;
			}
		}

		private void listBox2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = listBox2.SelectedIndex;
			if (n >= 0)
			{
				listBox1.SelectedIndex = n;
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
		private void btOK_Click(object sender, System.EventArgs e)
		{
			string script = null;
			try
			{
				string sep1 = _connection.NameDelimiterBegin;
				string sep2 = _connection.NameDelimiterEnd;
				string sName = mainTable.TableName + subTable.TableName;
				sName = System.Text.RegularExpressions.Regex.Replace(sName, "[ ]", "");
				sName += "EPFK";
				int i;
				StringBuilder sSQL = new StringBuilder("ALTER TABLE ");
				sSQL.Append(sep1);
				sSQL.Append(subTable.TableName);
				sSQL.Append(sep2);
				sSQL.Append(" ADD CONSTRAINT ");
				sSQL.Append(sep1);
				sSQL.Append(sName);
				sSQL.Append(sep2);
				sSQL.Append(" FOREIGN KEY (");
				sSQL.Append(sep1);
				EPField fld = listBox2.Items[0] as EPField;
				sSQL.Append(fld.Name);
				sSQL.Append(sep2);
				for (i = 1; i < listBox2.Items.Count; i++)
				{
					fld = listBox2.Items[i] as EPField;
					sSQL.Append(",");
					sSQL.Append(sep1);
					sSQL.Append(fld.Name);
					sSQL.Append(sep2);
				}
				sSQL.Append(") REFERENCES ");
				sSQL.Append(sep1);
				sSQL.Append(mainTable.TableName);
				sSQL.Append(sep2);
				sSQL.Append(" (");
				sSQL.Append(sep1);
				fld = listBox1.Items[0] as EPField;
				if (fld == null)
				{
					MessageBox.Show(this, Resource1.fkMissing, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					return;
				}
				sSQL.Append(fld.Name);
				sSQL.Append(sep2);
				for (i = 1; i < listBox2.Items.Count; i++)
				{
					fld = listBox1.Items[i] as EPField;
					if (fld == null)
					{
						MessageBox.Show(this, Resource1.fkMissing, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
						return;
					}
					sSQL.Append(",");
					sSQL.Append(sep1);
					sSQL.Append(fld.Name);
					sSQL.Append(sep2);
				}
				sSQL.Append(") ON DELETE CASCADE ON UPDATE CASCADE");
				script = sSQL.ToString();
				_connection.ExecuteNonQuery(script);
				//
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				//Close();
			}
			catch (Exception er)
			{
				string sMsg = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Could not set table relation. The database engine may not support it.\r\n{0}\r\n {1}", script, er.Message);
				MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
		}
	}
}
