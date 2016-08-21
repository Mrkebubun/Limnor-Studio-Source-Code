/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Application
{
	public partial class DlgDeleteProfile : Form
	{
		private ApplicationConfiguration _appCfg;
		public DlgDeleteProfile()
		{
			InitializeComponent();
			_appCfg = new ApplicationConfiguration(true);
		}
		public void SetExeFile(string file)
		{
			if (_appCfg.SetApplicationFullPath(file))
			{
				txtExeFile.Text = file;
				listBox1.Items.Clear();
				string uf = _appCfg.userProfilePath;
				if (File.Exists(uf))
				{
					listBox1.Items.Add(new ConfigProfile("current user", uf, EnumProfileType.User));
				}
				string[] names = _appCfg.GetNames();
				if (names != null && names.Length > 0)
				{
					for (int i = 0; i < names.Length; i++)
					{
						if (string.IsNullOrEmpty(names[i]))
						{
							listBox1.Items.Add(new ConfigProfile(names[i], _appCfg.namedProfilePath(string.Empty), EnumProfileType.Default));
							break;
						}
					}
					for (int i = 0; i < names.Length; i++)
					{
						if (!string.IsNullOrEmpty(names[i]))
						{
							listBox1.Items.Add(new ConfigProfile(names[i], _appCfg.namedProfilePath(names[i]), EnumProfileType.Named));
						}
					}
				}
			}
			else
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Program {0} is not configurable", file), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void btExeFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Program files|*.exe";
			dlg.DefaultExt = "EXE";
			dlg.Title = "Select EXE file";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				SetExeFile(dlg.FileName);
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				ConfigProfile cp = listBox1.Items[listBox1.SelectedIndex] as ConfigProfile;
				if (cp != null)
				{
					txtCfgFile.Text = cp.ProfilePath;
				}
			}
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				ConfigProfile cp = listBox1.Items[n] as ConfigProfile;
				if (cp != null)
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture,"Do you want to delete this configuration file [{0}]?",cp), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						try
						{
							File.Delete(cp.ProfilePath);
							listBox1.Items.RemoveAt(n);
							if (n < listBox1.Items.Count)
							{
								listBox1.SelectedIndex = n;
							}
							else if (listBox1.Items.Count > 0)
							{
								listBox1.SelectedIndex = listBox1.Items.Count - 1;
							}
							else
							{
								listBox1.SelectedIndex = -1;
							}
						}
						catch (Exception err)
						{
							MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
		}
	}
}
