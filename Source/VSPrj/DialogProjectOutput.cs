/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LimnorWeb;
using System.IO;
using System.Globalization;

namespace VSPrj
{
	public partial class DialogProjectOutput : Form
	{
		private LimnorProject _prj;
		private string _webSiteName;
		private VirtualWebDir _webSite;
		public DialogProjectOutput()
		{
			InitializeComponent();
		}
		public VirtualWebDir WebSite
		{
			get
			{
				return _webSite;
			}
		}
		public void LoadData(LimnorProject project, string siteName)
		{
			_prj = project;
			txtPrj.Text = _prj.ProjectName;
			_webSiteName = siteName;
			VirtualWebDir vw = _prj.GetTestWebSite(this);
			if (vw != null)
			{
				_webSite = vw;
				if (!VirtualWebDir.IsNetworkDrive(vw.PhysicalDirectory))
				{
					txtWebFolder.Text = vw.PhysicalDirectory;
				}
				if (string.IsNullOrEmpty(vw.WebName))
				{
					txtWebName.Text = _webSiteName;
				}
				else
				{
					txtWebName.Text = vw.WebName;
				}
				if (vw.IsValid)
				{
					picWeb.Image = Resource1._webOK.ToBitmap();
				}
				else
				{
					picWeb.Image = Resource1._webNotOK.ToBitmap();
				}
			}
			else
			{
				txtWebName.Text = _webSiteName;
				string sh = _prj.WebPhysicalFolder(this);
				if (!VirtualWebDir.IsNetworkDrive(sh))
				{
					txtWebFolder.Text = sh;
				}
				picWeb.Image = Resource1._webNotOK.ToBitmap();
			}
			txtUrl.Text = string.Format(CultureInfo.InvariantCulture, "http://localhost/{0}", txtWebName.Text);
			IList<OutputFolder> lst = _prj.GetOutputFolders();
			foreach (OutputFolder f in lst)
			{
				checkedListBox1.Items.Add(f, !f.Disabled);
			}
		}

		private void btDel_Click(object sender, EventArgs e)
		{
			int n = checkedListBox1.SelectedIndex;
			if (n >= 0)
			{
				checkedListBox1.Items.RemoveAt(n);
				if (n < checkedListBox1.Items.Count)
				{
					checkedListBox1.SelectedIndex = n;
				}
				else
				{
					checkedListBox1.SelectedIndex = checkedListBox1.Items.Count - 1;
				}
			}
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				OutputFolder f = new OutputFolder(false, dlg.SelectedPath);
				checkedListBox1.Items.Add(f, true);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (_webSite != null)
			{
				List<OutputFolder> fs = new List<OutputFolder>();
				for (int i = 0; i < checkedListBox1.Items.Count; i++)
				{
					OutputFolder f = checkedListBox1.Items[i] as OutputFolder;
					f.Disabled = !checkedListBox1.GetItemChecked(i);
					fs.Add(f);
				}
				_prj.SetOutputFolders(fs);
				_prj.SaveTestWebSite(_webSite);
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				MessageBox.Show(this, "Test web site is not created.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btWeb_Click(object sender, EventArgs e)
		{
			DialogNewWeb dlg = new DialogNewWeb();
			string ph = txtWebFolder.Text;
			string wn = txtWebName.Text;
			if (string.IsNullOrEmpty(ph) && _webSite != null)
			{
				ph = _webSite.PhysicalDirectory;
			}
			if (string.IsNullOrEmpty(wn))
			{
				wn = _webSiteName;
			}
			dlg.LoadData("Create a test web site", wn, ph, true);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				if (dlg.Ret != null)
				{
					_webSite = dlg.Ret;
					txtWebName.Text = _webSite.WebName;
					txtWebFolder.Text = _webSite.PhysicalDirectory;
					txtUrl.Text = string.Format(CultureInfo.InvariantCulture, "http://localhost/{0}", txtWebName.Text);
					picWeb.Image = Resource1._webOK.ToBitmap();
				}
			}
		}

		private void btHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "A web site must be created for a web project so that Limnor Studio may manipulate web page contents. The web physical folder must be on the local computer.", "Create web site", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btHelp2_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Web pages and support files in the web site physical folder will be copied to every outpur folder, if the folder is checked. The web physical folder may contain some design-time-only files. Those files will not be copied.", "Create web site", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
