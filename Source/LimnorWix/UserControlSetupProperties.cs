/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Specialized;
using VPL;

namespace LimnorWix
{
	public partial class UserControlSetupProperties : SetupContentsHolder
	{
		private PropertyGrid _externalPropertyGrid;
		public UserControlSetupProperties()
		{
			InitializeComponent();
			treeView1.SelectFile += new EventHandler(treeView1_SelectFile);
			treeView1.PropertyChanged += new EventHandler(treeView1_PropertyChanged);
			treeView1.FilesRemoved += new EventHandler(treeView1_FilesRemoved);
			checkedListBox1.ItemCheck += new ItemCheckEventHandler(checkedListBox1_ItemCheck);
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
			checkedListBox1.MouseDown += new MouseEventHandler(checkedListBox1_MouseDown);
		}

		void treeView1_FilesRemoved(object sender, EventArgs e)
		{
			EventArgsFiles ef = (EventArgsFiles)e;
			for (int k = 0; k < ef.Files.Count; k++)
			{
				for (int i = 0; i < checkedListBox1.Items.Count; i++)
				{
					WixSourceFileNode f = checkedListBox1.Items[i] as WixSourceFileNode;
					if (f != null)
					{
						if (string.Compare(ef.Files[k].Filename, f.Filename, StringComparison.OrdinalIgnoreCase) == 0)
						{
							checkedListBox1.Items.Remove(f);
							break;
						}
					}
				}
			}
			OnPropertyChanged();
		}

		void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (!treeView1.ReadOnly)
			{
				if (e.Button == MouseButtons.Right)
				{
					MenuItem mi;
					ContextMenu cm = new ContextMenu();
					TreeNodeWixFolder tnf = treeView1.SelectedNode as TreeNodeWixFolder;
					if (tnf != null)
					{
						mi = new MenuItem("Add files", tnf.mnu_addFiles);
						cm.MenuItems.Add(mi);
					}
					mi = new MenuItem("Remove unchecked files", tnf.mnu_removeUncheckedFiles);
					cm.MenuItems.Add(mi);
					cm.Show(checkedListBox1, new Point(e.X, e.Y));
				}
			}
		}
		void treeView1_PropertyChanged(object sender, EventArgs e)
		{
			OnPropertyChanged();
		}

		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			OnPropertyChanged();
		}
		public override void SetPropertyGrid(PropertyGrid pg)
		{
			_externalPropertyGrid = pg;
		}
		void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.Index >= 0)
			{
				WixSourceFileNode f = checkedListBox1.Items[e.Index] as WixSourceFileNode;
				if (f != null)
				{
					bool b = (e.NewValue != CheckState.Checked);
					if (f.IsRemoved != b)
					{
						f.IsRemoved = b;
						OnPropertyChanged();
					}
				}
			}
		}

		void treeView1_SelectFile(object sender, EventArgs e)
		{
			WixSourceFileNode mf = (WixSourceFileNode)sender;
			if (mf != null)
			{
				for (int i = 0; i < checkedListBox1.Items.Count; i++)
				{
					WixSourceFileNode f = checkedListBox1.Items[i] as WixSourceFileNode;
					if (f != null)
					{
						if (string.Compare(mf.Filename, f.Filename, StringComparison.OrdinalIgnoreCase) == 0)
						{
							checkedListBox1.SelectedIndex = i;
							return;
						}
					}
				}
				int n = checkedListBox1.Items.Add(mf);
				checkedListBox1.SetItemChecked(n, !mf.IsRemoved);
				checkedListBox1.SelectedIndex = n;
			}
		}
		public void LoadData(string filename)
		{
			treeView1.LoadData(filename);
		}
		public void Save()
		{
			treeView1.Save();
		}
		public override void OnBeforeSave()
		{
			treeView1.OnBeforeSave();
		}
		public override bool LoadData(XmlNode rootXmlNode, string filename)
		{
			return treeView1.LoadData(rootXmlNode, filename);
		}
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			OnSelectTreeNode(e.Node);
		}
		public void OnSelectTreeNode(TreeNode node)
		{
			ITreeNodeFolder folder = node as ITreeNodeFolder;
			if (folder != null)
			{
				folder.OnSelected();
				checkedListBox1.Visible = true;
				propertyGrid1.Visible = false;
				checkedListBox1.Items.Clear();
				IList<WixSourceFileNode> files = folder.Files;
				if (files != null && files.Count > 0)
				{
					SortedList<string, WixSourceFileNode> ss = new SortedList<string, WixSourceFileNode>();
					foreach (WixSourceFileNode f in files)
					{
						ss.Add(f.ToString(), f);
					}
					IEnumerator<KeyValuePair<string, WixSourceFileNode>> ie = ss.GetEnumerator();
					while (ie.MoveNext())
					{
						int n = checkedListBox1.Items.Add(ie.Current.Value);
						checkedListBox1.SetItemChecked(n, !ie.Current.Value.IsRemoved);
					}
				}
			}
			else
			{
				TreeNodeWix mn = node as TreeNodeWix;
				if (mn != null)
				{
					object o;
					WixCultureNode c = mn.WixNode as WixCultureNode;
					if (c != null)
						o = c.CultureNode;
					else
						o = mn.WixNode;
					propertyGrid1.SelectedObject = o;
					propertyGrid1.Visible = true;
					checkedListBox1.Visible = false;
				}
			}
		}

		private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_externalPropertyGrid != null)
			{
				int n = checkedListBox1.SelectedIndex;
				if (n >= 0 && n < checkedListBox1.Items.Count)
				{
					WixSourceFileNode f = checkedListBox1.Items[n] as WixSourceFileNode;
					if (f != null)
					{
						_externalPropertyGrid.SelectedObject = f;
					}
				}
			}
		}
	}
}
