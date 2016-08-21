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
	public partial class DlgCopyProfile : Form
	{
		private string _exeName;
		public DlgCopyProfile()
		{
			InitializeComponent();
		}
		public void LoadData(string curfile)
		{
			lblFile.Text = curfile;
			_exeName = Path.GetFileName(curfile);
			int pos = _exeName.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
			if (pos > 0)
			{
				_exeName = _exeName.Substring(0, pos + 4);
			}
			else
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Invalid configuration file name:{0}", curfile));
			}
		}
		private void btFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select Copy-to folder";
			dlg.ShowNewFolderButton = true;
			try
			{
				dlg.SelectedPath = txtFolder.Text;
			}
			catch
			{
			}
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				txtFolder.Text = dlg.SelectedPath;
				lblTargetFile.Text = string.Format(CultureInfo.InvariantCulture, "{0}.cfg", Path.Combine(dlg.SelectedPath, _exeName));
			}
		}

		private void btCopy_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(lblTargetFile.Text))
				{
					MessageBox.Show(this, "Copy-to folder not specified", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					if (File.Exists(lblTargetFile.Text))
					{
						if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "File exists:\r\n{0}\r\nDo you want to overwrite it?", lblTargetFile.Text), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
						{
							return;
						}
					}
					File.Copy(lblFile.Text, lblTargetFile.Text, true);
					MessageBox.Show(this, "File copied", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
