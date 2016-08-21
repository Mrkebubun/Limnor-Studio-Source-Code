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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Application
{
	public partial class DlgCategories : Form
	{
		public CategoryList Ret;
		private bool _isDataOnly;
		public DlgCategories()
		{
			InitializeComponent();
		}
		public bool IsDataOnly
		{
			get
			{
				return _isDataOnly;
			}
		}
		public void SetDataOnly()
		{
			_isDataOnly = true;
			toolStripButtonAddCat.Enabled = false;
			toolStripButtonDelCat.Enabled = false;
			toolStripButtonAddProperty.Enabled = false;
			toolStripButtonDelProp.Enabled = false;
		}
		public void LoadData(CategoryList list)
		{
			if (list == null)
			{
				Ret = new CategoryList();
			}
			else
			{
				Ret = list;
			}
			TreeNodeAppConfig ta = new TreeNodeAppConfig(Ret, _isDataOnly);
			treeView1.Nodes.Add(ta);
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNodeCat nc = e.Node as TreeNodeCat;
			if (nc != null)
			{
				nc.LoadNextLevel();
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeCat nc = e.Node as TreeNodeCat;
			if (nc != null)
			{
				propertyGrid1.SelectedObject = nc.Data;
			}
			else
			{
				propertyGrid1.SelectedObject = null;
			}
		}
		private void toolStripButtonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void toolStripButtonOK_Click(object sender, EventArgs e)
		{
			Ret = new CategoryList();
			TreeNodeAppConfig ta = null;
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				ta = treeView1.Nodes[i] as TreeNodeAppConfig;
				if (ta != null)
				{
					break;
				}
			}
			if (ta != null)
			{
				for (int i = 0; i < ta.Nodes.Count; i++)
				{
					TreeNodeCategory tc = ta.Nodes[i] as TreeNodeCategory;
					if (tc != null)
					{
						ConfigCategory cat = tc.Category;
						cat.Properties.Properties.Clear();
						Ret.Categories.Add(cat);
						for (int j = 0; j < tc.Nodes.Count; j++)
						{
							TreeNodeConfigProperty tp = tc.Nodes[j] as TreeNodeConfigProperty;
							if (tp != null)
							{
								cat.Properties.Properties.Add(tp.Property);
							}
						}
					}
				}
			}
			this.DialogResult = DialogResult.OK;
		}

		private void toolStripButtonAddCat_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				TreeNodeAppConfig ta = treeView1.Nodes[i] as TreeNodeAppConfig;
				if (ta != null)
				{
					ta.AddCategory();
					break;
				}
			}
		}

		private void toolStripButtonDelCat_Click(object sender, EventArgs e)
		{
			TreeNodeCategory tc = treeView1.SelectedNode as TreeNodeCategory;
			if (tc != null)
			{
				tc.Delete();
			}
		}

		private void toolStripButtonAddProperty_Click(object sender, EventArgs e)
		{
			TreeNodeCategory tc = treeView1.SelectedNode as TreeNodeCategory;
			if (tc == null)
			{
				if (treeView1.SelectedNode != null)
				{
					tc = treeView1.SelectedNode.Parent as TreeNodeCategory;
				}
			}
			if (tc != null)
			{
				tc.AddProperty();
			}
		}

		private void toolStripButtonDelProp_Click(object sender, EventArgs e)
		{
			TreeNodeConfigProperty tp = treeView1.SelectedNode as TreeNodeConfigProperty;
			if (tp != null)
			{
				tp.Delete();
			}
		}
	}
	abstract class TreeNodeCat : TreeNode
	{
		public const int IMG_APP = 0;
		public const int IMG_CAT = 1;
		public const int IMG_PRO = 2;

		private bool _nextLevelLoaded;
		public TreeNodeCat()
		{
		}
		public abstract object Data { get; }
		public abstract bool IsDataOnly { get; }
		public void LoadNextLevel()
		{
			if (!_nextLevelLoaded)
			{
				_nextLevelLoaded = true;
				List<TreeNodeLoader> l = new List<TreeNodeLoader>();
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeLoader nl = Nodes[i] as TreeNodeLoader;
					if (nl != null)
					{
						l.Add(nl);
					}
				}
				foreach (TreeNodeLoader nl in l)
				{
					nl.Remove();
				}
				foreach (TreeNodeLoader nl in l)
				{
					nl.LoadNextLevel(this);
				}
			}
		}
	}
	abstract class TreeNodeLoader : TreeNode
	{
		public TreeNodeLoader()
		{
		}
		public abstract void LoadNextLevel(TreeNodeCat parent);
	}
	class TreeNodeAppConfig : TreeNodeCat
	{
		private CategoryList _list;
		private bool _isDataOnly;
		public TreeNodeAppConfig(CategoryList list, bool dataOnly)
		{
			_isDataOnly = dataOnly;
			_list = list;
			Text = "Application Configurations";
			ImageIndex = TreeNodeCat.IMG_APP;
			SelectedImageIndex = TreeNodeCat.IMG_APP;
			Nodes.Add(new CLoader());
			if (!dataOnly)
			{
				this.ContextMenu = new ContextMenu();
				MenuItem mi = new MenuItem("Add Category", mnu_addCat);
				ContextMenu.MenuItems.Add(mi);
			}
		}
		public override bool IsDataOnly
		{
			get { return _isDataOnly; }
		}
		public void AddCategory()
		{
			Expand();
			ConfigCategory cat = new ConfigCategory();
			int n = 1;
			string nameBase = "Category";
			cat.CategoryName = nameBase + n.ToString();
			bool b = true;
			while (b)
			{
				b = false;
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeCategory cn = Nodes[i] as TreeNodeCategory;
					if (cn != null)
					{
						ConfigCategory c = cn.Category;
						if (string.Compare(cat.CategoryName, c.CategoryName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							n++;
							cat.CategoryName = nameBase + n.ToString();
							b = true;
						}
					}
				}
			}
			TreeNodeCategory tc = new TreeNodeCategory(cat, false);
			Nodes.Add(tc);
			this.Expand();
			this.TreeView.SelectedNode = tc;
		}
		private void mnu_addCat(object sender, EventArgs e)
		{
			AddCategory();
		}
		public override object Data { get { return null; } }
		public CategoryList List
		{
			get
			{
				return _list;
			}
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeCat parent)
			{
				TreeNodeAppConfig ta = (TreeNodeAppConfig)parent;
				foreach (ConfigCategory cat in ta._list.Categories)
				{
					TreeNodeCategory tc = new TreeNodeCategory(cat, parent.IsDataOnly);
					parent.Nodes.Add(tc);
				}
			}
		}
	}
	class TreeNodeCategory : TreeNodeCat
	{
		private ConfigCategory _cat;
		private bool _isDataOnly;
		public TreeNodeCategory(ConfigCategory cat, bool dataOnly)
		{
			_isDataOnly = dataOnly;
			_cat = cat;
			_cat.IsDataOnly = dataOnly;
			Text = _cat.CategoryName;
			ImageIndex = TreeNodeCat.IMG_CAT;
			SelectedImageIndex = TreeNodeCat.IMG_CAT;
			Nodes.Add(new CLoader());
			if (!dataOnly)
			{
				this.ContextMenu = new ContextMenu();
				MenuItem mi = new MenuItem("Add Property", mnu_addProp);
				ContextMenu.MenuItems.Add(mi);
				mi = new MenuItem("-");
				ContextMenu.MenuItems.Add(mi);
				mi = new MenuItem("Delete Category", mnu_delCat);
				ContextMenu.MenuItems.Add(mi);
			}
			_cat.NameChanging = nameChanging;
		}
		public override bool IsDataOnly
		{
			get { return _isDataOnly; }
		}
		public void Delete()
		{
			if (MessageBox.Show(this.TreeView.FindForm(), "Do you want to delete this category", "Application Configurations", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				if (this.TreeView != null && this.TreeView.Nodes.Count > 0)
				{
					this.TreeView.SelectedNode = this.TreeView.Nodes[0];
				}
				this.Remove();
			}
		}
		public void AddProperty()
		{
			Expand();
			ConfigProperty pr = new ConfigProperty();
			int n = 1;
			string nameBase = "Property";
			pr.DataName = nameBase + n.ToString();
			bool b = true;
			while (b)
			{
				b = false;
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeConfigProperty cn = Nodes[i] as TreeNodeConfigProperty;
					if (cn != null)
					{
						ConfigProperty p = cn.Property;
						if (string.Compare(pr.DataName, p.DataName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							n++;
							pr.DataName = nameBase + n.ToString();
							b = true;
						}
					}
				}
			}
			TreeNodeConfigProperty tc = new TreeNodeConfigProperty(pr, false);
			Nodes.Add(tc);
			this.Expand();
			this.TreeView.SelectedNode = tc;
		}
		private void mnu_delCat(object sender, EventArgs e)
		{
			Delete();
		}
		private void mnu_addProp(object sender, EventArgs e)
		{
			AddProperty();
		}
		public ConfigCategory Category
		{
			get
			{
				return _cat;
			}
		}
		public override object Data { get { return _cat; } }
		private void nameChanging(object sender, EventArgs e)
		{
			TreeNodeCategory changedNode = null;
			EventArgvNameChange en = (EventArgvNameChange)e;
			if (ApplicationConfiguration.IsReservedWord(en.NewName))
			{
				MessageBox.Show(this.TreeView.FindForm(), "The name is reserved", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				en.Cancel = true;
			}
			else
			{
				for (int i = 0; i < Parent.Nodes.Count; i++)
				{
					TreeNodeCategory tp = Parent.Nodes[i] as TreeNodeCategory;
					if (tp != null)
					{
						if (string.CompareOrdinal(en.OldName, tp.Category.CategoryName) != 0)
						{
							if (string.CompareOrdinal(en.NewName, tp.Category.CategoryName) == 0)
							{
								MessageBox.Show(this.TreeView.FindForm(), "The name is in use", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								en.Cancel = true;
								break;
							}
						}
						else
						{
							changedNode = tp;
						}
					}
				}
			}
			if (!en.Cancel)
			{
				if (changedNode != null)
				{
					changedNode.Text = en.NewName;
				}
			}
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeCat parent)
			{
				TreeNodeCategory tc = (TreeNodeCategory)parent;
				foreach (ConfigProperty p in tc._cat.Properties.Properties)
				{
					TreeNodeConfigProperty tp = new TreeNodeConfigProperty(p, parent.IsDataOnly);
					parent.Nodes.Add(tp);
				}
			}
		}
	}
	class TreeNodeConfigProperty : TreeNodeCat
	{
		private ConfigProperty _property;
		private bool _isDataOnly;
		public TreeNodeConfigProperty(ConfigProperty property, bool dataOnly)
		{
			_isDataOnly = dataOnly;
			_property = property;
			_property.IsDataOnly = dataOnly;
			Text = _property.DataName;
			ImageIndex = TreeNodeCat.IMG_PRO;
			SelectedImageIndex = TreeNodeCat.IMG_PRO;
			this.ContextMenu = new ContextMenu();
			MenuItem mi = new MenuItem("Delete Property", mnu_delProp);
			ContextMenu.MenuItems.Add(mi);
			_property.NameChanging = nameChanging;
		}
		public override bool IsDataOnly
		{
			get { return _isDataOnly; }
		}
		public ConfigProperty Property
		{
			get
			{
				return _property;
			}
		}
		public override object Data { get { return _property; } }
		public void Delete()
		{
			if (MessageBox.Show(this.TreeView.FindForm(), "Do you want to delete this property", "Application Configurations", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				if (this.TreeView != null && this.TreeView.Nodes.Count > 0)
				{
					this.TreeView.SelectedNode = this.TreeView.Nodes[0];
				}
				this.Remove();
			}
		}
		private void nameChanging(object sender, EventArgs e)
		{
			TreeNode changedNode = null;
			EventArgvNameChange en = (EventArgvNameChange)e;
			TreeNodeCategory tc = (TreeNodeCategory)Parent;
			for (int i = 0; i < tc.Nodes.Count; i++)
			{
				TreeNodeConfigProperty tp = tc.Nodes[i] as TreeNodeConfigProperty;
				if (tp != null)
				{
					if (string.CompareOrdinal(en.OldName, tp.Property.DataName) != 0)
					{
						if (string.CompareOrdinal(en.NewName, tp.Property.DataName) == 0)
						{
							MessageBox.Show(this.TreeView.FindForm(), "The name is in use", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							en.Cancel = true;
							break;
						}
					}
					else
					{
						changedNode = tc.Nodes[i];
					}
				}
			}
			if (!en.Cancel)
			{
				if (changedNode != null)
				{
					changedNode.Text = en.NewName;
				}
			}
		}
		private void mnu_delProp(object sender, EventArgs e)
		{
			Delete();
		}
	}
}
