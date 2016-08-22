/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
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
using System.Globalization;
using XmlUtility;
using LimnorDesigner;

namespace SolutionMan.Solution
{
	public partial class DialogNewProject : Form
	{
		const int IMG_DEF_Image = 0;
		//
		const string CFG_Loc = "Location";
		//
		public SolutionSelection Result;
		private bool _forNewSoltion = true;
		private List<ProjectTemplate> _templates;
		private bool _loaded;
		public DialogNewProject()
		{
			InitializeComponent();
			loadTemplates();
			setView(true);
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (!_loaded)
			{
				_loaded = true;
				setView(false);
				setView(true);
			}
		}
		public void LoadData(string solutionFile)
		{
			if (string.IsNullOrEmpty(solutionFile))
			{
				textBoxLocation.Text = AppConfig.GetSolutionStringValue(CFG_Loc);
				_forNewSoltion = true;
				checkBox1.Enabled = true;
			}
			else
			{
				_forNewSoltion = false;
				string name = Path.GetFileNameWithoutExtension(solutionFile);
				string folder = Path.GetDirectoryName(solutionFile);
				textBoxLocation.Text = folder;
				textBoxSolution.Text = name;
				checkBox1.Checked = false;
				checkBox1.Enabled = false;
			}
		}
		private void loadTemplates()
		{
			string sdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
			if (!Directory.Exists(sdir))
			{
				MessageBox.Show("Project templates folder not exist at " + sdir, "New Project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				string[] folders = Directory.GetDirectories(sdir);
				if (folders != null && folders.Length > 0)
				{
					_templates = new List<ProjectTemplate>();
					for (int i = 0; i < folders.Length; i++)
					{
						string[] temps = Directory.GetFiles(folders[i], "*.vstemplate");
						if (temps != null && temps.Length > 0)
						{
							for (int k = 0; k < temps.Length; k++)
							{
								try
								{
									ProjectTemplate pt = new ProjectTemplate(temps[k]);
									pt.LoadName();
									_templates.Add(pt);
								}
								catch (Exception err)
								{
									MessageBox.Show("Error loading " + temps[k] + ". " + err.Message, "New Project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								}
							}
						}
					}
					if (_templates.Count == 0)
					{
						MessageBox.Show("No project templates found at " + sdir, "New Project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						foreach (ProjectTemplate p in _templates)
						{
							int idx;
							Icon ic = p.TemplateIcon;
							if (ic == null)
							{
								idx = IMG_DEF_Image;
							}
							else
							{
								idx = imageList1.Images.Add(ic.ToBitmap(), Color.White);
								imageList2.Images.Add(ic.ToBitmap(), Color.White);
							}
							ListViewItem li = new ListViewItem(p.Name, idx);
							li.Tag = p;
							listView1.Items.Add(li);
						}
					}
				}
				else
				{
					MessageBox.Show("Project templates not installed at " + sdir, "New Project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			listView1.Refresh();
		}
		private void setView(bool small)
		{
			if (small)
			{
				listView1.View = View.SmallIcon;
				pictureBoxSmall.Image = Resource1.smallIcon_active;
				pictureBoxLarge.Image = Resource1.largeIcon_inactive;
			}
			else
			{
				listView1.View = View.LargeIcon;
				pictureBoxSmall.Image = Resource1.smallIcon_inactive;
				pictureBoxLarge.Image = Resource1.largeIcon_active;
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			lblSolution.Enabled = checkBox1.Checked;
			textBoxSolution.Enabled = checkBox1.Checked;
			if (!checkBox1.Checked)
			{
				if (_forNewSoltion)
				{
					textBoxSolution.Text = textBoxName.Text;
				}
			}
		}

		private void buttonFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select project folder";
			if (!string.IsNullOrEmpty(textBoxLocation.Text))
			{
				if (Directory.Exists(textBoxLocation.Text))
				{
					dlg.SelectedPath = textBoxLocation.Text;
				}
			}
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				textBoxLocation.Text = dlg.SelectedPath;
				listView1_SelectedIndexChanged(sender, e);
			}
		}

		private void pictureBoxLarge_Click(object sender, EventArgs e)
		{
			setView(false);
		}

		private void pictureBoxSmall_Click(object sender, EventArgs e)
		{
			setView(true);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				ListViewItem item = listView1.SelectedItems[0];
				if (item != null)
				{
					ProjectTemplate p = item.Tag as ProjectTemplate;
					if (p != null)
					{
						lblDesc.Text = p.Description;
						string loc = textBoxLocation.Text.Trim();
						if (string.IsNullOrEmpty(loc))
						{
							textBoxName.Text = p.DefaultName + "1";
						}
						else
						{
							int n = 1;
							string name = p.DefaultName + "1";
							try
							{
								if (Directory.Exists(textBoxLocation.Text))
								{
									string path = Path.Combine(loc, name);
									while (Directory.Exists(path))
									{
										n++;
										name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", p.DefaultName, n);
										path = Path.Combine(loc, name);
									}
									textBoxName.Text = name;
								}
							}
							catch
							{
								textBoxName.Text = name;
							}
						}
						buttonOK.Enabled = true;
					}
				}
			}
			else
			{
				buttonOK.Enabled = false;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				ListViewItem item = listView1.SelectedItems[0];
				if (item != null)
				{
					ProjectTemplate p = item.Tag as ProjectTemplate;
					if (p != null)
					{
						Result = new SolutionSelection();
						Result.ProjectTemplate = p.EntityTemplateFile;
						Result.ProjectName = textBoxName.Text.Trim();
						if (Result.ProjectName.Length == 0)
						{
							MessageBox.Show(this, "Missing project name", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						Result.Location = textBoxLocation.Text.Trim();
						if (Result.Location.Length == 0)
						{
							MessageBox.Show(this, "Missing project location", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						Result.SolutionName = textBoxSolution.Text.Trim();
						Result.CreateSolutionFolder = checkBox1.Checked;
						if (checkBox1.Checked)
						{
							if (Result.SolutionName.Length == 0)
							{
								MessageBox.Show(this, "Missing solution name", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
						}
						if (Result.SolutionName.Length == 0)
						{
							Result.SolutionName = Result.ProjectName;
						}
						if (checkBox1.Checked)
						{
							string s = Path.Combine(Result.Location, Result.SolutionName);
							if (Directory.Exists(s))
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Solution folder exists: {0}", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
							s = Path.Combine(s, Result.ProjectName);
							if (Directory.Exists(s))
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Project folder exists: {0}", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
						}
						else
						{
							string s = Path.Combine(Result.Location, Result.ProjectName);
							if (Directory.Exists(s))
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Project folder exists: {0}", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
						}
						string wizardName = p.GetWizardName();
						if (!string.IsNullOrEmpty(wizardName))
						{
							if (string.CompareOrdinal(wizardName, "Longflow.VSProject.WebWizard") == 0)
							{
								WebWizard cw = new WebWizard();
								string destKey = "$destinationdirectory$";
								if (Result.ReplaceNames.ContainsKey(destKey))
								{
									Result.ReplaceNames[destKey] = Path.Combine(Result.Location, Result.ProjectName);
								}
								else
								{
									Result.ReplaceNames.Add(destKey, Path.Combine(Result.Location, Result.ProjectName));
								}
								if (!cw.RunWizard(Result.ReplaceNames))
								{
									return;
								}
							}
						}
						AppConfig.SetSolutionStringValue(CFG_Loc, Result.Location);
						this.DialogResult = DialogResult.OK;
					}
				}
			}
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			if (_forNewSoltion)
			{
				if (!checkBox1.Checked)
				{
					textBoxSolution.Text = textBoxName.Text;
				}
			}
		}
	}
}
