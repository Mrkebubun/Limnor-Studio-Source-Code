/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// Summary description for dlgPropTimestamp.
	/// </summary>
	public class dlgPropTimestamp : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkTS;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public EPField fldRet = null;
		//
		public dlgPropTimestamp()
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkTS = new System.Windows.Forms.CheckBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(46, 105);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(187, 95);
			this.listBox1.TabIndex = 0;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(45, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(181, 59);
			this.label1.TabIndex = 1;
			this.label1.Tag = "1";
			this.label1.Text = "If you select a timestamp  field, data transfer will only be done for newer recor" +
				"ds according to the timestamp field";
			// 
			// chkTS
			// 
			this.chkTS.Location = new System.Drawing.Point(46, 75);
			this.chkTS.Name = "chkTS";
			this.chkTS.TabIndex = 2;
			this.chkTS.Tag = "2";
			this.chkTS.Text = "Use timestamp";
			this.chkTS.CheckedChanged += new System.EventHandler(this.chkTS_CheckedChanged);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(77, 216);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 3;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(155, 216);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 4;
			this.btCancel.Text = "Cancel";
			// 
			// dlgPropTimestamp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 253);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.chkTS);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropTimestamp";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Specify timestamp";
			this.ResumeLayout(false);

		}
		#endregion
		public bool LoadData(EasyQuery qry)
		{
			listBox1.Items.Clear();
			if( qry != null )
			{
				FieldList fl = qry.Fields;
				if( fl != null )
				{
					int n;
					for(int i=0;i<fl.Count;i++)
					{
						if( EPField.IsDatetime(fl[i].OleDbType) )
						{
							n = listBox1.Items.Add(fl[i]);
							if( fl[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp )
							{
								listBox1.SelectedIndex = n;
								chkTS.Checked = true;
							}
						}
					}
				}
			}
			return (listBox1.Items.Count>0);
		}

		private void chkTS_CheckedChanged(object sender, System.EventArgs e)
		{
			if( !chkTS.Checked )
			{
				listBox1.SelectedIndex = -1;
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chkTS.Checked = (listBox1.SelectedIndex >= 0 );
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if( n >= 0 )
				fldRet = listBox1.Items[n] as EPField;
			else
				fldRet = null;
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

	}
}
