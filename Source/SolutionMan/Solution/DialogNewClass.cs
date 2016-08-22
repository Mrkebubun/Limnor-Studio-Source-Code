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
using VSPrj;
using LimnorDesigner;

namespace SolutionMan.Solution
{
	public partial class DialogNewClass : Form
	{
		const int IMG_DEF_Image = 0;
		//
		public ItemSelection Result;
		private List<ItemTemplate> _templates;
		private LimnorProject _prj;
		public DialogNewClass()
		{
			InitializeComponent();
			loadTemplates();
			setView(true);
		}
		public void Loaddata(LimnorProject project, string select)
		{
			_prj = project;
			Text = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", Text, project.ProjectName);
			if (!string.IsNullOrEmpty(select))
			{
				for (int i = 0; i < listView1.Items.Count; i++)
				{
					ListViewItem li = listView1.Items[i];
					if (string.Compare(select, li.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						li.Selected = true;
						break;
					}
				}
			}
		}
		private void loadTemplates()
		{
			string sdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectItems");
			if (!Directory.Exists(sdir))
			{
				MessageBox.Show("Project item templates folder not exist at " + sdir, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				string[] folders = Directory.GetDirectories(sdir);
				if (folders != null && folders.Length > 0)
				{
					_templates = new List<ItemTemplate>();
					for (int i = 0; i < folders.Length; i++)
					{
						string[] temps = Directory.GetFiles(folders[i], "*.vstemplate");
						if (temps != null && temps.Length > 0)
						{
							for (int k = 0; k < temps.Length; k++)
							{
								try
								{
									ItemTemplate pt = new ItemTemplate(temps[k]);
									pt.LoadName();
									_templates.Add(pt);
								}
								catch (Exception err)
								{
									MessageBox.Show("Error loading " + temps[k] + ". " + err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
								}
							}
						}
					}
					if (_templates.Count == 0)
					{
						MessageBox.Show("No project item templates found at " + sdir, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						foreach (ItemTemplate p in _templates)
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
							li.Name = p.Name;
							li.Tag = p;
							listView1.Items.Add(li);
						}
					}
				}
				else
				{
					MessageBox.Show("Project item templates not installed at " + sdir, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
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
					ItemTemplate p = item.Tag as ItemTemplate;
					if (p != null)
					{
						lblDesc.Text = p.Description;
						string loc = _prj.ProjectFolder;
						int n = 1;
						string name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", p.DefaultName, n);
						try
						{
							string path = Path.Combine(loc, string.Format(CultureInfo.InvariantCulture, "{0}.{1}", name, NodeDataSolution.FILE_EXT_CLASS));
							while (File.Exists(path))
							{
								n++;
								name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", p.DefaultName, n);
								path = Path.Combine(loc, string.Format(CultureInfo.InvariantCulture, "{0}.{1}", name, NodeDataSolution.FILE_EXT_CLASS));
							}
							textBoxName.Text = name;

						}
						catch
						{
							textBoxName.Text = name;
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
					ItemTemplate p = item.Tag as ItemTemplate;
					if (p != null)
					{
						Result = new ItemSelection();
						Result.ItemTemplate = p.EntityTemplateFile;
						Result.ClassTemplate = Path.Combine(Path.GetDirectoryName(p.EntityTemplateFile), p.GetClassTemplate());
						if (!File.Exists(Result.ClassTemplate))
						{
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Class template file exists: {0}", Result.ClassTemplate), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						Result.ItemName = textBoxName.Text.Trim();
						if (!VOB.VobUtil.IsGoodVarName(Result.ItemName))
						{
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Invalid name: {0}. Only English alphabets, numbers, and underscore are allowed. The first letter cannot be a number.", Result.ItemName), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						Result.ReplaceNames.Add(ItemSelection.Safeitemname, Result.ItemName);
						Result.NewClassFile = Path.Combine(_prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Result.ItemName, NodeDataSolution.FILE_EXT_CLASS));
						if (File.Exists(Result.NewClassFile))
						{
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "New class file already exists: {0}", Result.NewClassFile), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						string wizardName = p.GetWizardName();
						if (!string.IsNullOrEmpty(wizardName))
						{
							if (string.CompareOrdinal(wizardName, "Longflow.VSProject.ClassWizard") == 0)
							{
								ClassWizard cw = new ClassWizard(_prj);
								if (!cw.RunWizard(Result.ReplaceNames))
								{
									return;
								}
							}
						}
					}
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
		}
	}
}
