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
using System.Collections.Specialized;
using System.Reflection;
using System.Collections.Generic;
using VPL;
using System.IO;
using System.Globalization;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for dlgClassFile.
	/// </summary>
	public class dlgClassFile : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btNext;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btBack;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btFile;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox chkControlOnly;
		private Label label5;
		private Button buttonGac;
		//
		public System.Windows.Forms.Form frmPrev = null;
		public dlgClassFile()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgClassFile));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btNext = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btBack = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btFile = new System.Windows.Forms.Button();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkControlOnly = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonGac = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(565, 96);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(448, 392);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 23;
			this.btCancel.Text = "&Cancel";
			// 
			// btNext
			// 
			this.btNext.Location = new System.Drawing.Point(312, 392);
			this.btNext.Name = "btNext";
			this.btNext.Size = new System.Drawing.Size(75, 23);
			this.btNext.TabIndex = 22;
			this.btNext.Text = "&Finish";
			this.btNext.Click += new System.EventHandler(this.btNext_Click);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.DimGray;
			this.label3.Location = new System.Drawing.Point(0, 354);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(568, 2);
			this.label3.TabIndex = 21;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(0, 353);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(568, 1);
			this.label4.TabIndex = 20;
			// 
			// btBack
			// 
			this.btBack.Location = new System.Drawing.Point(224, 392);
			this.btBack.Name = "btBack";
			this.btBack.Size = new System.Drawing.Size(75, 23);
			this.btBack.TabIndex = 19;
			this.btBack.Text = "&Back";
			this.btBack.Click += new System.EventHandler(this.btBack_Click);
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(16, 184);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(536, 134);
			this.listBox1.TabIndex = 18;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Blue;
			this.label2.Location = new System.Drawing.Point(16, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(448, 23);
			this.label2.TabIndex = 17;
			this.label2.Text = "Select components to be placed in the Toolbox";
			// 
			// btFile
			// 
			this.btFile.Location = new System.Drawing.Point(16, 101);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(184, 23);
			this.btFile.TabIndex = 16;
			this.btFile.Text = "&Select a DLL file";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(16, 128);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(536, 20);
			this.txtFile.TabIndex = 14;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(254, 20);
			this.label1.TabIndex = 13;
			this.label1.Text = "DLL file containing .NET class:";
			this.label1.Visible = false;
			// 
			// chkControlOnly
			// 
			this.chkControlOnly.Location = new System.Drawing.Point(16, 328);
			this.chkControlOnly.Name = "chkControlOnly";
			this.chkControlOnly.Size = new System.Drawing.Size(240, 24);
			this.chkControlOnly.TabIndex = 24;
			this.chkControlOnly.Text = "Show screen elements only";
			this.chkControlOnly.CheckedChanged += new System.EventHandler(this.chkControlOnly_CheckedChanged);
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.White;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.label5.Location = new System.Drawing.Point(158, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(336, 35);
			this.label5.TabIndex = 25;
			this.label5.Text = "Library Import Wizard";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonGac
			// 
			this.buttonGac.Location = new System.Drawing.Point(206, 101);
			this.buttonGac.Name = "buttonGac";
			this.buttonGac.Size = new System.Drawing.Size(224, 23);
			this.buttonGac.TabIndex = 26;
			this.buttonGac.Text = "Browse Global Assembly Cache";
			this.buttonGac.UseVisualStyleBackColor = true;
			this.buttonGac.Click += new System.EventHandler(this.buttonGac_Click);
			// 
			// dlgClassFile
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(560, 438);
			this.ControlBox = false;
			this.Controls.Add(this.buttonGac);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.chkControlOnly);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btNext);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgClassFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select components";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgClassFile_Closing);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void dlgClassFile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (frmPrev != null)
			{
				frmPrev.Close();
				frmPrev = null;
			}
		}

		private void btBack_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
			Close();
		}

		public bool refreshInfo()
		{
			bool bRet = false;
			string sMsg;
			btNext.Enabled = false;
			btBack.Enabled = false;
			btCancel.Enabled = false;
			listBox1.Items.Clear();
			txtFile.Text = frmPerformerImport.WizardInfo.DotNetDLL;
			btFile.Enabled = true;
			if (frmPerformerImport.WizardInfo.DotNetDLLReady)
			{
				label1.ForeColor = System.Drawing.Color.Black;
				label2.ForeColor = System.Drawing.Color.Blue;
				chkControlOnly.Checked = frmPerformerImport.WizardInfo.ControlOnly;
				if (frmPerformerImport.WizardInfo.ShowPublicTypes(listBox1, out sMsg))
				{
					btNext.Enabled = (listBox1.Items.Count > 0);
					if (listBox1.Items.Count > 0)
					{
						System.Type tp;
						for (int i = 0; i < listBox1.Items.Count; i++)
						{
							tp = listBox1.Items[i] as System.Type;
							if (tp.Equals(frmPerformerImport.WizardInfo.DotNetType))
							{
								listBox1.SelectedIndex = i;
								break;
							}
						}
						if (listBox1.SelectedIndex < 0)
						{
							listBox1.SelectedIndex = 0;
							frmPerformerImport.WizardInfo.DotNetType = listBox1.Items[0] as System.Type;
						}
					}
					bRet = true;
				}
				else
				{
					MessageBox.Show(sMsg);
				}
			}
			else
			{
				label1.ForeColor = System.Drawing.Color.Blue;
				label2.ForeColor = System.Drawing.Color.Black;
			}
			btBack.Enabled = true;
			btCancel.Enabled = true;
			return bRet;
		}

		private void chkControlOnly_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkControlOnly.Checked != frmPerformerImport.WizardInfo.ControlOnly)
			{
				frmPerformerImport.WizardInfo.ControlOnly = chkControlOnly.Checked;
				refreshInfo();
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				frmPerformerImport.WizardInfo.DotNetType = listBox1.Items[n] as System.Type;
				btNext.Enabled = true;
			}
		}

		private void btFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			try
			{
				dlg.FileName = frmPerformerImport.WizardInfo.DotNetDLL;
			}
			catch
			{
			}
			dlg.Title = "Select .NET DLL";
			dlg.Filter = ".Net DLL|*.DLL";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				frmPerformerImport.WizardInfo.SetDotNetDLL(dlg.FileName);
				refreshInfo();
			}
		}
		private static void addNonGacReferenceLocations(Dictionary<string, List<Type>> sc, Assembly a)
		{
			if (!a.GlobalAssemblyCache)
			{
				List<Type> lst;
				string s = a.Location.ToLowerInvariant();
				if (!sc.TryGetValue(s, out lst))
				{
					lst = new List<Type>();
					sc.Add(s, lst);
				}
				AssemblyName[] names = a.GetReferencedAssemblies();
				if (names != null)
				{
					for (int i = 0; i < names.Length; i++)
					{
						Assembly a0 = Assembly.Load(names[i]);
						addNonGacReferenceLocations(sc, a0);
					}
				}
			}
		}
		private static void findNonGacReferenceLocations(Dictionary<string, List<Type>> sc, Type t)
		{
			if (!t.Assembly.GlobalAssemblyCache)
			{
				List<Type> lst;
				string s = t.Assembly.Location.ToLowerInvariant();
				if (!sc.TryGetValue(s, out lst))
				{
					lst = new List<Type>();

					sc.Add(s, lst);
				}
				if (!lst.Contains(t))
				{
					lst.Add(t);
				}
				string curDir = Directory.GetCurrentDirectory();
				string sDir = Path.GetDirectoryName(s);
				Directory.SetCurrentDirectory(sDir);
				try
				{
					AssemblyName[] names = t.Assembly.GetReferencedAssemblies();
					if (names != null)
					{
						for (int i = 0; i < names.Length; i++)
						{
							Assembly a0 = Assembly.ReflectionOnlyLoad(names[i].FullName);
							addNonGacReferenceLocations(sc, a0);
						}
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message, "Load DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				Directory.SetCurrentDirectory(curDir);
			}
		}
		private void btNext_Click(object sender, System.EventArgs e)
		{
			if (listBox1.SelectedItems.Count > 0)
			{
				for (int i = 0; i < listBox1.SelectedItems.Count; i++)
				{
					Type t = listBox1.SelectedItems[i] as Type;
					if (t != null)
					{
						frmPerformerImport.WizardInfo.AddSelectedType(t);
					}
				}
				this.DialogResult = DialogResult.OK;
			}
		}

		private void buttonGac_Click(object sender, EventArgs e)
		{
			Type t = FormTypeSelection.SelectType(this);
			if (t != null)
			{
				listBox1.Items.Add(t);
			}
		}
	}
}
