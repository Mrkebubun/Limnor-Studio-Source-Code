/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Information Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LimnorWeb
{
	public partial class DialogNewWeb : Form
	{
		public VirtualWebDir Ret;
		public DialogNewWeb()
		{
			InitializeComponent();
		}
		public void LoadData(string title, string webName, string folder, bool changeFolder)
		{
			Text = title;
			textBoxVdir.Text = webName;
			textBoxPdir.Text = folder;
			buttonFolder.Enabled = changeFolder;
		}
		private void buttonFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			try
			{
				dlg.Description = "Select a physical folder for the new web site";
				dlg.ShowNewFolderButton = true;
				dlg.SelectedPath = textBoxPdir.Text;
			}
			catch
			{
			}
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				if (VirtualWebDir.IsNetworkDrive(dlg.SelectedPath))
				{
					MessageBox.Show(this, "A network folder cannot be used for creating a web site", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					textBoxPdir.Text = dlg.SelectedPath;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			bool bOK = false;
			buttonOK.Enabled = false;
			buttonCancel.Enabled = false;
			this.Cursor = Cursors.WaitCursor;
			string svDir = textBoxVdir.Text.Trim();
			if (svDir.Length == 0)
			{
				MessageBox.Show(this, "Web site name is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (svDir.Contains(' ') || svDir.Contains('\t'))
				{
					MessageBox.Show(this, "Do not use spaces and tabs in the web site name", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					Regex rg = new Regex(@"^([\w])*$");
					if (rg.IsMatch(svDir))
					{
						string spDir = textBoxPdir.Text.Trim();
						if (spDir.Length == 0)
						{
							MessageBox.Show(this, "Web site folder is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
						else
						{
							if (VirtualWebDir.IsNetworkDrive(spDir))
							{
								MessageBox.Show(this, "Cannot use a network folder for web site", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
							else
							{
								bOK = Directory.Exists(spDir);
								if (!bOK)
								{
									try
									{
										Directory.CreateDirectory(spDir);
										bOK = true;
									}
									catch (Exception er)
									{
										bOK = false;
										MessageBox.Show(this, er.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
								}
							}
							if (bOK && Directory.Exists(spDir))
							{
								bool exists = false;
								try
								{
									IList<VirtualWebDir> vs = IisUtility.GetVirtualDirectories(IisUtility.LocalWebPath);
									foreach (VirtualWebDir v in vs)
									{
										if (string.Compare(v.WebName, svDir, StringComparison.OrdinalIgnoreCase) == 0)
										{
											exists = true;
											if (string.Compare(v.PhysicalDirectory, spDir, StringComparison.OrdinalIgnoreCase) == 0)
											{
												MessageBox.Show(this, "The web site is already created", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
												Ret = v;
											}
											else
											{
												MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Web site name is used by folder {0}", v.PhysicalDirectory), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
												bOK = false;
											}
											break;
										}
										if (string.Compare(v.PhysicalDirectory, spDir, StringComparison.OrdinalIgnoreCase) == 0)
										{
											MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Web site folder is used by web site {0}.", v.WebName), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
											Ret = v;
											exists = true;
											bOK = false;
											break;
										}
									}
									if (!exists)
									{
										try
										{
											IisUtility.CreateLocalWebSite(svDir, spDir);
											Ret = new VirtualWebDir(svDir, spDir);
										}
										catch (Exception err)
										{
											bOK = false;
											MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										}
									}
								}
								catch (Exception err)
								{
									bOK = false;
									MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								}
								if (bOK)
								{
									//set access permissions
									VirtualWebDir.AddDirectorySecurity(spDir);
								}
							}
							else
							{
								MessageBox.Show(this, "Web site folder does not exist", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						}
					}
					else
					{
						MessageBox.Show(this, "Please use only alphanumeric characters in Web site name", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}
			if (bOK)
			{
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				buttonOK.Enabled = true;
				buttonCancel.Enabled = true;
				this.Cursor = Cursors.Default;
			}
		}
	}
}
