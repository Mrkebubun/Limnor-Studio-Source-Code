/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for dlgDone.
	/// </summary>
	public class dlgDone : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btNext;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btBack;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.TextBox txtMsg;
		private System.Windows.Forms.Button btTest;
		private System.Windows.Forms.Button btSave;
		//
		public System.Windows.Forms.Form frmPrev = null;
		public dlgDone()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgDone));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btNext = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblFile = new System.Windows.Forms.Label();
			this.btBack = new System.Windows.Forms.Button();
			this.btTest = new System.Windows.Forms.Button();
			this.txtMsg = new System.Windows.Forms.TextBox();
			this.btSave = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(561, 92);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// btNext
			// 
			this.btNext.Location = new System.Drawing.Point(304, 384);
			this.btNext.Name = "btNext";
			this.btNext.TabIndex = 21;
			this.btNext.Text = "&Finish";
			this.btNext.Click += new System.EventHandler(this.btNext_Click);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.DimGray;
			this.label3.Location = new System.Drawing.Point(0, 354);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(568, 2);
			this.label3.TabIndex = 20;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(0, 353);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(568, 1);
			this.label4.TabIndex = 19;
			// 
			// lblTitle
			// 
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTitle.ForeColor = System.Drawing.Color.Black;
			this.lblTitle.Location = new System.Drawing.Point(24, 128);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(488, 23);
			this.lblTitle.TabIndex = 23;
			this.lblTitle.Text = "DLL file created:";
			// 
			// lblFile
			// 
			this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFile.Location = new System.Drawing.Point(32, 184);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(496, 64);
			this.lblFile.TabIndex = 24;
			// 
			// btBack
			// 
			this.btBack.Location = new System.Drawing.Point(216, 384);
			this.btBack.Name = "btBack";
			this.btBack.TabIndex = 25;
			this.btBack.Text = "&Back";
			this.btBack.Click += new System.EventHandler(this.btBack_Click);
			// 
			// btTest
			// 
			this.btTest.Location = new System.Drawing.Point(32, 256);
			this.btTest.Name = "btTest";
			this.btTest.TabIndex = 26;
			this.btTest.Text = "&Test load";
			this.btTest.Click += new System.EventHandler(this.btTest_Click);
			// 
			// txtMsg
			// 
			this.txtMsg.Location = new System.Drawing.Point(32, 280);
			this.txtMsg.Multiline = true;
			this.txtMsg.Name = "txtMsg";
			this.txtMsg.ReadOnly = true;
			this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtMsg.Size = new System.Drawing.Size(496, 72);
			this.txtMsg.TabIndex = 27;
			this.txtMsg.Text = "";
			// 
			// btSave
			// 
			this.btSave.Enabled = false;
			this.btSave.Location = new System.Drawing.Point(112, 256);
			this.btSave.Name = "btSave";
			this.btSave.TabIndex = 29;
			this.btSave.Text = "&Save to file";
			this.btSave.Click += new System.EventHandler(this.btSave_Click);
			// 
			// dlgDone
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(558, 436);
			this.ControlBox = false;
			this.Controls.Add(this.btSave);
			this.Controls.Add(this.txtMsg);
			this.Controls.Add(this.btTest);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.lblFile);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.btNext);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDone";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Limnor Performer Created";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgDone_Closing);
			this.Activated += new System.EventHandler(this.dlgDone_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		private void dlgDone_Activated(object sender, System.EventArgs e)
		{
			lblFile.Text = frmPerformerImport.WizardInfo.CompiledFile;
			if( frmPerformerImport.WizardInfo.SourceType == enumSourceType.Compile )
			{
				lblTitle.Text = "DLL file created:";
			}
			else
			{
				lblTitle.Text = "DLL file created (Limnor performer inside):";
			}
		}

		private void btNext_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void dlgDone_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if( frmPrev != null )
			{
				frmPrev.Close();
				frmPrev = null;
			}
		}

		private void btBack_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form f = frmPrev;
			frmPrev = null;
			Close();
			if( f != null )
				f.Show();
		}

		private void btTest_Click(object sender, System.EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			try
			{
				string sExe = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath),"TestLoadWin.exe");
				if( System.IO.File.Exists(sExe) )
				{
					string sType = "";
					System.Reflection.Assembly a = System.Reflection.Assembly.LoadFile(frmPerformerImport.WizardInfo.CompiledFile);
					System.Type[] tps = a.GetExportedTypes();
					if( tps != null )
					{
						for(int i=0;i<tps.Length;i++)
						{
							if( tps[i].GetInterface("EPControl.IPerformer") != null )
							{
								sType = tps[i].AssemblyQualifiedName;
								break;
							}
						}
					}
					if( sType.Length > 0 )
					{
						System.Diagnostics.Process pr = new System.Diagnostics.Process();
						pr.StartInfo =  new System.Diagnostics.ProcessStartInfo(sExe,"\""+sType+"\"");
						Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Longflow\\Limnor");
						reg.SetValue("TestLoad","");
						reg.Close();
						pr.Start();
						pr.WaitForExit();
						reg = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Longflow\\Limnor");
						string s = (string)reg.GetValue("TestLoad");
						txtMsg.Text = s;
						pr.Dispose();
					}
					else
					{
						txtMsg.Text = "No Performer found in this file";
					}
				}
				else
				{
					txtMsg.Text = "File not found:"+sExe;
				}
			}
			catch(Exception er)
			{
				txtMsg.Text = "Failed:\r\n"+ImportException.FormExceptionText( er );
			}
			this.Cursor = System.Windows.Forms.Cursors.Default;
			btSave.Enabled = true;
		}

		private void btSave_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Save test results to a file";
			dlg.CheckFileExists = false;
			if( dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
			{
				System.IO.StreamWriter sw = null;
				try
				{
					sw = new System.IO.StreamWriter(dlg.FileName,false);
					sw.Write(txtMsg.Text);
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
}
