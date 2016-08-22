using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for dlgCode.
	/// </summary>
	public class dlgCode : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Label lblFile;
		private System.Windows.Forms.TextBox txtCode;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		private string filename = "";
		public dlgCode()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.lblFile = new System.Windows.Forms.Label();
			this.txtCode = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(0, 0);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 0;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(75, 0);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 1;
			this.btCancel.Text = "Cancel";
			// 
			// lblFile
			// 
			this.lblFile.Location = new System.Drawing.Point(152, 0);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(448, 23);
			this.lblFile.TabIndex = 2;
			this.lblFile.Text = "File name";
			this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtCode
			// 
			this.txtCode.Location = new System.Drawing.Point(0, 24);
			this.txtCode.MaxLength = 0;
			this.txtCode.Multiline = true;
			this.txtCode.Name = "txtCode";
			this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtCode.Size = new System.Drawing.Size(608, 240);
			this.txtCode.TabIndex = 3;
			this.txtCode.Text = "";
			// 
			// dlgCode
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(608, 266);
			this.ControlBox = false;
			this.Controls.Add(this.txtCode);
			this.Controls.Add(this.lblFile);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgCode";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Source code editor";
			this.Resize += new System.EventHandler(this.dlgCode_Resize);
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(string sFile)
		{
			System.IO.StreamReader sr = null;
			filename = sFile;
			lblFile.Text = filename;
			try
			{
				sr = new System.IO.StreamReader(sFile);
				txtCode.Text = sr.ReadToEnd();
			}
			catch(Exception er)
			{
				txtCode.Text = er.Message;
				txtCode.ReadOnly = true;
				txtCode.ForeColor = System.Drawing.Color.Red;
				btOK.Enabled = false;
			}
			finally
			{
				if( sr != null )
				{
					sr.Close();
				}
			}
		}
		private void dlgCode_Resize(object sender, System.EventArgs e)
		{
			int n = this.ClientSize.Width - lblFile.Left-2;
			if( n > 30 )
			{
				lblFile.Width = n;
			}
			txtCode.Width = this.ClientSize.Width;
			n  = this.ClientSize.Height - txtCode.Top - 2;
			if( n > 30 )
			{
				txtCode.Height = n;
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			System.IO.StreamWriter sw = null;
			try
			{
				sw = new System.IO.StreamWriter(filename,false);
				sw.Write(txtCode.Text);
				sw.Close();
				sw = null;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
			catch(Exception er)
			{
				MessageBox.Show(er.Message);
			}
			finally
			{
				if( sw != null )
				{
					sw.Close();
				}
			}
		}
	}
}
