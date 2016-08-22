/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
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

namespace Limnor.WebBuilder
{
	public partial class DlgMenuItems : Form
	{
		private HtmlMenu _menu;
		public DlgMenuItems()
		{
			InitializeComponent();
		}
		public void LoadData(HtmlMenu menu)
		{
			_menu = menu;
			if (menu.MenuItems.Count > 0)
			{
				for (int i = 0; i < menu.MenuItems.Count; i++)
				{
					loadItem(treeView1.Nodes, menu.MenuItems[i]);
				}
			}
		}
		private void loadItem(TreeNodeCollection nodes, HtmlMenuItem item)
		{
			TreeNodeMenuItem tn = new TreeNodeMenuItem(item, this.imageList1);
			nodes.Add(tn);
			if (item.MenuItems.Count > 0)
			{
				for (int i = 0; i < item.MenuItems.Count; i++)
				{
					loadItem(tn.Nodes, item.MenuItems[i]);
				}
			}
		}
		class TreeNodeMenuItem : TreeNode
		{
			private HtmlMenuItem _item;
			public TreeNodeMenuItem(HtmlMenuItem item, ImageList images)
			{
				_item = item;
				Text = item.Text;
				ImageIndex = -1;
				SelectedImageIndex = -1;
				RefreshNodeImage(images);

			}
			public void RefreshNodeImage(ImageList images)
			{
				if (!string.IsNullOrEmpty(_item.ImagePath))
				{
					if (File.Exists(_item.ImagePath))
					{
						string key = _item.ImagePath.ToLowerInvariant();
						int n = -1;
						if (images.Images.ContainsKey(key))
						{
							n = images.Images.IndexOfKey(key);
						}
						if (n < 0)
						{
							try
							{
								Image img = Image.FromFile(_item.ImagePath);
								if (img != null)
								{
									images.Images.Add(key, img);
									n = images.Images.IndexOfKey(key);
								}
							}
							catch
							{
							}
						}
						if (n >= 0)
						{
							ImageIndex = n;
							SelectedImageIndex = n;
						}
					}
				}
			}
			public void CollectItems()
			{
				_item.MenuItems.Clear();
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					TreeNodeMenuItem tn = this.Nodes[i] as TreeNodeMenuItem;
					if (tn != null)
					{
						_item.MenuItems.Add(tn.MenuItem);
						tn.CollectItems();
					}
				}
			}
			public HtmlMenuItem MenuItem
			{
				get
				{
					return _item;
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			_menu.MenuItems.Clear();
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				TreeNodeMenuItem tn = treeView1.Nodes[i] as TreeNodeMenuItem;
				if (tn != null)
				{
					_menu.MenuItems.Add(tn.MenuItem);
					tn.CollectItems();
				}
			}
		}

		private void btAddRoot_Click(object sender, EventArgs e)
		{
			HtmlMenuItem item = new HtmlMenuItem(null, _menu);
			TreeNodeMenuItem tn = new TreeNodeMenuItem(item, imageList1);
			treeView1.Nodes.Add(tn);
		}

		private void btAddSub_Click(object sender, EventArgs e)
		{
			TreeNodeMenuItem tp = treeView1.SelectedNode as TreeNodeMenuItem;
			if (tp != null)
			{
				HtmlMenuItem item = new HtmlMenuItem(tp.MenuItem, _menu);
				TreeNodeMenuItem tn = new TreeNodeMenuItem(item, imageList1);
				tp.Nodes.Add(tn);
			}
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			TreeNode tn = treeView1.SelectedNode;
			if (tn != null)
			{
				if (MessageBox.Show(this, "Do you want to delete the selected menu and all its sub menus?", "Delete menu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					tn.Remove();
				}
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeMenuItem tp = e.Node as TreeNodeMenuItem;
			if (tp != null)
			{
				propertyGrid1.SelectedObject = tp.MenuItem;
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			TreeNodeMenuItem tp = treeView1.SelectedNode as TreeNodeMenuItem;
			if (tp != null)
			{
				if (string.CompareOrdinal("Text", e.ChangedItem.PropertyDescriptor.Name) == 0)
				{
					tp.Text = e.ChangedItem.Value as string;
				}
				else if (string.CompareOrdinal("ImagePath", e.ChangedItem.PropertyDescriptor.Name) == 0)
				{
					tp.RefreshNodeImage(this.imageList1);
				}
			}
		}
		private void moveDown(TreeNodeCollection ns)
		{
			TreeNode nd = treeView1.SelectedNode;
			if (nd != null)
			{
				int n = nd.Index;
				if (n < ns.Count - 1)
				{
					nd.Remove();
					ns.Insert(n + 1, nd);
					treeView1.SelectedNode = nd;
				}
			}
		}
		private void moveUp(TreeNodeCollection ns)
		{
			TreeNode nd = treeView1.SelectedNode;
			if (nd != null)
			{
				int n = nd.Index;
				if (n > 0)
				{
					nd.Remove();
					ns.Insert(n - 1, nd);
					treeView1.SelectedNode = nd;
				}
			}
		}
		private void btDown_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				if (treeView1.SelectedNode.Parent == null)
				{
					moveDown(treeView1.Nodes);
				}
				else
				{
					moveDown(treeView1.SelectedNode.Parent.Nodes);
				}
			}
		}

		private void btUp_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				if (treeView1.SelectedNode.Parent == null)
				{
					moveUp(treeView1.Nodes);
				}
				else
				{
					moveUp(treeView1.SelectedNode.Parent.Nodes);
				}
			}
		}
	}
}
