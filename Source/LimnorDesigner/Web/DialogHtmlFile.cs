/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
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

namespace LimnorDesigner.Web
{
	public partial class DialogHtmlFile : Form
	{
		private string _webPhysicalFolder;
		private string _fileSelectionTitle;
		private string _fileSelectionFilter;
		public string WebFile;
		public DialogHtmlFile()
		{
			InitializeComponent();
		}
		public void LoadData(string webPhysicalFolder, string value, string title, string filter)
		{
			_webPhysicalFolder = webPhysicalFolder;
			this.Text = title;
			_fileSelectionTitle = title;
			_fileSelectionFilter = filter;
			treeView1.Nodes.Clear();
			treeView1.Nodes.Add(new TreeNodeWebRoot(_webPhysicalFolder));
			if (!string.IsNullOrEmpty(value))
			{
				string defv;
				string s0 = "http://localhost";
				if (value.StartsWith(s0, StringComparison.OrdinalIgnoreCase))
				{
					defv = value.Substring(s0.Length).Replace("/", "\\");
				}
				else
				{
					defv = value.Replace("/", "\\");
				}
				textBoxFile.Text = Path.Combine(_webPhysicalFolder, defv);
			}
		}
		private void buttonFile_Click(object sender, EventArgs e)
		{
			openFileDialog1.Title = _fileSelectionTitle;
			openFileDialog1.Filter = _fileSelectionFilter;
			openFileDialog1.ShowDialog(this);
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			textBoxFile.Text = openFileDialog1.FileName;
			buttonOK.Enabled = true;
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNodeFolder tnf = e.Node as TreeNodeFolder;
			if (tnf != null && !tnf.NextLevelLoaded)
			{
				tnf.NextLevelLoaded = true;
				List<TreeNodeLoader> l = new List<TreeNodeLoader>();
				for (int i = 0; i < e.Node.Nodes.Count; i++)
				{
					TreeNodeLoader tnl = e.Node.Nodes[i] as TreeNodeLoader;
					if (tnl != null)
					{
						l.Add(tnl);
					}
				}
				if (l.Count > 0)
				{
					for (int i = 0; i < l.Count; i++)
					{
						e.Node.Nodes.Remove(l[i]);
					}
					for (int i = 0; i < l.Count; i++)
					{
						l[i].LoadNextLevel(tnf);
					}
				}
			}
		}
		class TreeNodeFolder : TreeNode
		{
			private string _folder;
			public TreeNodeFolder(string folder)
			{
				_folder = folder;
				Text = Path.GetFileName(folder);
				ImageIndex = 2;
				SelectedImageIndex = 2;
				Nodes.Add(new TreeNodeLoader());
			}
			public string Folder { get { return _folder; } }
			public virtual string WebFolder
			{
				get
				{
					TreeNodeFolder tnf = this.Parent as TreeNodeFolder;
					if (tnf != null)
					{
						string w = tnf.WebFolder;
						if (string.IsNullOrEmpty(w))
							return Text;
						else
							return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", w, Text);
					}
					else
						return string.Empty;
				}
			}
			public bool NextLevelLoaded { get; set; }

		}
		class TreeNodeWebRoot : TreeNodeFolder
		{
			public TreeNodeWebRoot(string folder)
				: base(folder)
			{
				Text = "WebFiles";
			}
			public override string WebFolder
			{
				get
				{
					return string.Empty;
				}
			}
		}
		class TreeNodeLoader : TreeNode
		{
			public TreeNodeLoader()
			{
			}
			public void LoadNextLevel(TreeNodeFolder parent)
			{
				string[] ss = Directory.GetDirectories(parent.Folder);
				if (ss != null && ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						parent.Nodes.Add(new TreeNodeFolder(ss[i]));
					}
				}
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeFolder tnf = e.Node as TreeNodeFolder;
			if (tnf != null)
			{
				textBoxFolder.Text = tnf.WebFolder;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			try
			{
				string f = textBoxFile.Text.Trim();
				if (string.IsNullOrEmpty(f))
				{
					MessageBox.Show(this, "The filename is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					if (!File.Exists(f))
					{
						WebFile = f;
						this.DialogResult = System.Windows.Forms.DialogResult.OK;
					}
					else
					{
						string webFolder = string.Empty;
						string folder = _webPhysicalFolder;
						TreeNodeFolder tnf = treeView1.SelectedNode as TreeNodeFolder;
						if (tnf != null)
						{
							folder = tnf.Folder; //web physical folder to copy file to
							webFolder = tnf.WebFolder; //web virtual folder to appear in html
						}
						if (string.IsNullOrEmpty(folder))
						{
							MessageBox.Show(this, "The web folder is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							if (!Directory.Exists(folder))
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "The web folder does not exist. [{0}]", folder), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							else
							{
								try
								{
									WebFile = Path.GetFileName(f);
									string tgt = Path.Combine(folder, WebFile);
									if (string.Compare(f, tgt, StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (!File.Exists(tgt))
										{
											File.Copy(f, tgt);
										}
									}
									if (!string.IsNullOrEmpty(webFolder))
									{
										WebFile = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", webFolder, WebFile);
									}
									this.DialogResult = DialogResult.OK;
								}
								catch (Exception e0)
								{
									MessageBox.Show(this, e0.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonCreateFolder_Click(object sender, EventArgs e)
		{
			TreeNodeFolder tnf = treeView1.SelectedNode as TreeNodeFolder;
			if (tnf == null)
			{
				tnf = treeView1.Nodes[0] as TreeNodeFolder;
			}
			tnf.Expand();
			DialogCreateWebFolder dlg = new DialogCreateWebFolder();
			dlg.LoadData(tnf.Folder);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				tnf.Nodes.Add(new TreeNodeFolder(dlg.NewFolder));
			}
		}

		private void textBoxFile_TextChanged(object sender, EventArgs e)
		{
			string f = textBoxFile.Text.Trim();
			if (string.IsNullOrEmpty(f))
			{
				buttonOK.Enabled = false;
			}
			else
			{
				buttonOK.Enabled = true;
			}
		}
	}
}
