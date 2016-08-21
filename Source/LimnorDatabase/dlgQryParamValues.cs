using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgQryParamValues.
	/// </summary>
	public class dlgQryParamValues : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		FieldList parameters;
		//
		public dlgQryParamValues()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            listView1.Columns[0].Text = Resource1.ParamName;
            listView1.Columns[1].Text = Resource1.TestValue;
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
			this.label1 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 24);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Test query parameters";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2});
			this.listView1.FullRowSelect = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(16, 48);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(264, 152);
			this.listView1.TabIndex = 1;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Parameter name";
			this.columnHeader1.Width = 113;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Test value";
			this.columnHeader2.Width = 141;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 208);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(216, 23);
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "Set test value for the selected parameter:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtValue
			// 
			this.txtValue.Location = new System.Drawing.Point(56, 232);
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(216, 20);
			this.txtValue.TabIndex = 3;
			this.txtValue.Text = "";
			this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(200, 264);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(112, 264);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 11;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// dlgQryParamValues
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(314, 304);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgQryParamValues";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Test query parameters";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(FieldList ps)
		{
			parameters = ps;
			ListViewItem item;
			listView1.Items.Clear();
			for(int i=0;i<ps.Count;i++)
			{
				item = new ListViewItem(ps[i].Name);
				if( ps[i].Value != null )
					item.SubItems.Add(ps[i].Value.ToString());
				else
					item.SubItems.Add("");
				listView1.Items.Add(item);
			}
		}

		private void txtValue_TextChanged(object sender, System.EventArgs e)
		{
			int m = 0;
			if( listView1.SelectedIndices.Count > 0 )
				m = listView1.SelectedIndices[0];
			listView1.Items[m].SubItems[1].Text = txtValue.Text;
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				for(int i=0;i<parameters.Count;i++)
				{
					parameters[i].Value = listView1.Items[i].SubItems[1].Text;
				}
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
			catch(Exception er)
			{
                FormLog.NotifyException(er);// clsAppGlobals.MsgBox(this, er.Message, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
			}
		}
	}
}
