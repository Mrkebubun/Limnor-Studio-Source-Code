using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// Summary description for dlgDTSTextSource.
	/// </summary>
	public class dlgDTSTextSource : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Button btFile;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox chkHeader;
		private System.Windows.Forms.RadioButton rdoTAB;
		private System.Windows.Forms.RadioButton rdoComma;
		//
		public DTSSourceText objRet = null;
		//
		private System.Windows.Forms.Button btView;
		private System.Windows.Forms.Button btOK;
		const ushort nSubsetID = 2300;
		private System.Windows.Forms.Button btProp;
		//
		bool bLoading = false;
		bool bOpen = true;
		public dlgDTSTextSource()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgDTSTextSource));
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.btFile = new System.Windows.Forms.Button();
			this.chkHeader = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoComma = new System.Windows.Forms.RadioButton();
			this.rdoTAB = new System.Windows.Forms.RadioButton();
			this.btView = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.btProp = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(65, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(274, 23);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Text file as data source";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(27, 28);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(20, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 17);
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "File path:";
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(21, 111);
			this.txtFile.Name = "txtFile";
			this.txtFile.Size = new System.Drawing.Size(300, 20);
			this.txtFile.TabIndex = 3;
			this.txtFile.Text = "";
			this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
			// 
			// btFile
			// 
			this.btFile.Location = new System.Drawing.Point(324, 108);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(26, 23);
			this.btFile.TabIndex = 4;
			this.btFile.Text = "...";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// chkHeader
			// 
			this.chkHeader.Location = new System.Drawing.Point(21, 142);
			this.chkHeader.Name = "chkHeader";
			this.chkHeader.Size = new System.Drawing.Size(204, 24);
			this.chkHeader.TabIndex = 5;
			this.chkHeader.Tag = "3";
			this.chkHeader.Text = "The first line is header";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdoComma);
			this.groupBox1.Controls.Add(this.rdoTAB);
			this.groupBox1.Location = new System.Drawing.Point(21, 172);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(303, 70);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Tag = "4";
			this.groupBox1.Text = "Field delimiter";
			// 
			// rdoComma
			// 
			this.rdoComma.Location = new System.Drawing.Point(165, 29);
			this.rdoComma.Name = "rdoComma";
			this.rdoComma.TabIndex = 1;
			this.rdoComma.Tag = "6";
			this.rdoComma.Text = "Comma";
			// 
			// rdoTAB
			// 
			this.rdoTAB.Checked = true;
			this.rdoTAB.Location = new System.Drawing.Point(22, 26);
			this.rdoTAB.Name = "rdoTAB";
			this.rdoTAB.TabIndex = 0;
			this.rdoTAB.TabStop = true;
			this.rdoTAB.Tag = "5";
			this.rdoTAB.Text = "TAB";
			// 
			// btView
			// 
			this.btView.Location = new System.Drawing.Point(48, 259);
			this.btView.Name = "btView";
			this.btView.TabIndex = 7;
			this.btView.Tag = "7";
			this.btView.Text = "View";
			this.btView.Click += new System.EventHandler(this.btNext_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(223, 258);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 11;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(137, 258);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 12;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btProp
			// 
			this.btProp.Location = new System.Drawing.Point(154, 82);
			this.btProp.Name = "btProp";
			this.btProp.Size = new System.Drawing.Size(165, 23);
			this.btProp.TabIndex = 13;
			this.btProp.Text = "From performer property";
			this.btProp.Click += new System.EventHandler(this.btProp_Click);
			// 
			// dlgDTSTextSource
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(363, 298);
			this.Controls.Add(this.btProp);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btView);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.chkHeader);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDTSTextSource";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select text file";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DTSSourceText txt)
		{
			bOpen = true;
			objRet = (DTSSourceText) txt.Clone();
			bLoading = true;
			if( objRet.prop != null )
				txtFile.Text = objRet.prop.ToString();
			else
				txtFile.Text = objRet.sFile;
			bLoading = false;
			chkHeader.Checked = objRet.HasHeader;
			switch(objRet.delimiter)
			{
				case enumSourceTextDelimiter.Comma:
					rdoComma.Checked = true;
					break;
				case enumSourceTextDelimiter.TAB:
					rdoTAB.Checked = true;
					break;
			}
		}
		public void LoadData(DTDestTextFile txt)
		{
			DTSSourceText obj = new DTSSourceText();
			obj.Filename = txt.Filename;
			obj.prop = txt.prop;
			obj.HasHeader = txt.HasHeader;
			obj.delimiter = txt.delimiter;
			LoadData(obj);
			bOpen = false;
			label1.Text = "Select text file as data destination";
			btView.Visible = false;
		}
        private void btNext_Click(object sender, System.EventArgs e)
        {
            string sFile = objRet.Filename;// txtFile.Text.Trim();
            if (System.IO.File.Exists(sFile))
            {
                dlgDTSTextSample dlg = new dlgDTSTextSample();

                objRet.Filename = sFile;
                objRet.HasHeader = chkHeader.Checked;
                if (rdoTAB.Checked)
                    objRet.delimiter = enumSourceTextDelimiter.TAB;
                else
                    objRet.delimiter = enumSourceTextDelimiter.Comma;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                dlg.LoadData(objRet);
                this.Cursor = System.Windows.Forms.Cursors.Default;
                System.Windows.Forms.DialogResult ret = dlg.ShowDialog(this);
                switch (ret)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        this.DialogResult = ret;
                        Close();
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        this.DialogResult = ret;
                        Close();
                        break;
                }
            }
            else
            {
                MessageBox.Show(this, "File not found", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

		
		private void btFile_Click(object sender, System.EventArgs e)
		{
			if(bOpen)
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "Text file|*.txt";
				try
				{
					dlg.FileName = txtFile.Text;
				}
				catch
				{
				}
				dlg.Title = this.Text;
				if( dlg.ShowDialog (this) == System.Windows.Forms.DialogResult.OK )
				{
					txtFile.Text = dlg.FileName;
				}
			}
			else
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "Text file|*.txt";
				try
				{
					dlg.FileName = txtFile.Text;
				}
				catch
				{
				}
				dlg.Title = this.Text;
				if( dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
				{
					txtFile.Text = dlg.FileName;
				}
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string sFile;
			if(bOpen)
			{
				sFile = objRet.Filename;//txtFile.Text.Trim();
			}
			else
			{
				sFile = txtFile.Text.Trim();
			}
			if( objRet.prop == null || System.IO.File.Exists(sFile) )
			{
				objRet.Filename = sFile;
				objRet.HasHeader = chkHeader.Checked;
				if( rdoTAB.Checked )
					objRet.delimiter = enumSourceTextDelimiter.TAB;
				else
					objRet.delimiter = enumSourceTextDelimiter.Comma;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
			else
			{
				clsAppGlobals.MsgBox(this,clsAppGlobals.objUIText.GetText((IUIText)objMsg,0),this.Text,System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
		}

		private void btProp_Click(object sender, System.EventArgs e)
		{
			clsPropPerformerPropDesc propDesc = new clsPropPerformerPropDesc();
			clsPerformerProp v = objRet.prop;
			if( v == null )
				v = new clsPerformerProp();
			propDesc.objProperty.setProperty(v);
			object val = v;
			if( propDesc.showSelectionDialog(this,ref val) )
			{
				bLoading = true;
				objRet.prop = (clsPerformerProp) val;
				txtFile.Text = objRet.prop.ToString();
				bLoading = false;
			}
		}

		private void txtFile_TextChanged(object sender, System.EventArgs e)
		{
			if( !bLoading )
			{
				string s = txtFile.Text.Trim();
				if( System.IO.File.Exists(s) )
				{
					objRet.prop = null;
					objRet.Filename = s;
				}
			}
		}
	}
}
