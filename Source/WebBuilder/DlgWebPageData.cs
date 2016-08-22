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
using XmlUtility;
using System.Globalization;
using VPL;
using System.Collections.Specialized;
using System.Xml;

namespace Limnor.WebBuilder
{
	public partial class DlgWebPageData : Form
	{
		private ILimnorProject _project;
		private WebPageDataSet _dataset;
		private XmlNode _projectResourcesNode;
		private bool _changed;
		private string _baseCaption;
		private PropertyDescriptor _currentProperty;
		private bool _updating;
		public DlgWebPageData()
		{
			InitializeComponent();
			if (VPLUtil.CollectLanguageIcons != null)
			{
				VPLUtil.CollectLanguageIcons(imageList1);
			}
		}
		public void LoadData(string dataName, WebPageDataSet data, ILimnorProject project)
		{
			_baseCaption = string.Format(CultureInfo.InvariantCulture, "{0} - [{1}]", this.Text, dataName);
			_project = project;
			_dataset = data;
			//
			_projectResourcesNode = _project.ResourcesXmlNode;
			updateLanguagesNodes(null);
			//
			WebDataTable[] tbls = _dataset.GetData();
			for (int i = 0; i < tbls.Length; i++)
			{
				WebDataTableSingleRow sr = tbls[i] as WebDataTableSingleRow;
				if (sr != null)
				{
					TreeNodeNamedData tnn = new TreeNodeNamedData(sr);
					treeViewDataSet.Nodes[0].Nodes.Add(tnn);
				}
				else
				{
					TreeNodeTabledData tnt = new TreeNodeTabledData(tbls[i]);
					treeViewDataSet.Nodes[0].Nodes.Add(tnt);
				}
			}
			treeViewDataSet.ExpandAll();
			if (treeViewDataSet.Nodes[0].Nodes.Count > 0)
			{
				treeViewDataSet.SelectedNode = treeViewDataSet.Nodes[0].Nodes[0];
			}
		}

		#region TreeNodeCulture
		class TreeNodeCulture : TreeNode
		{
			private CultureInfo _culture;
			public TreeNodeCulture(CultureInfo ci, ImageList imgs)
			{
				_culture = ci;
				Text = string.Format(CultureInfo.InvariantCulture, "{0} [{1}]", ci.Name, ci.NativeName);
				ImageIndex = GetLangaugeImageByName(ci.Name, imgs);
				SelectedImageIndex = ImageIndex;
			}
			public CultureInfo Culture
			{
				get
				{
					return _culture;
				}
			}
			public static int GetLangaugeImageByName(string name, ImageList imgs)
			{
				if (!string.IsNullOrEmpty(name))
				{
					if (imgs.Images.ContainsKey(name))
					{
						return imgs.Images.IndexOfKey(name);
					}
					else
					{
						int pos = name.IndexOf('-');
						if (pos > 0)
						{
							string s = name.Substring(0, pos);
							if (imgs.Images.ContainsKey(s))
							{
								return imgs.Images.IndexOfKey(s);
							}
						}
					}
				}
				return 7;
			}
		}
		#endregion
		#region TreeNodeData
		abstract class TreeNodeData : TreeNode
		{
			private WebDataTable _table;
			public TreeNodeData(WebDataTable table)
			{
				_table = table;
			}
			public string TableName
			{
				get
				{
					return _table.TableName;
				}
			}
			public WebDataTable Table
			{
				get
				{
					return _table;
				}
			}
		}
		class TreeNodeTabledData : TreeNodeData
		{
			public TreeNodeTabledData(WebDataTable table)
				: base(table)
			{
				Text = table.TableName;
				ImageIndex = 10;
				SelectedImageIndex = ImageIndex;
			}
		}
		class TreeNodeNamedData : TreeNodeData
		{
			public TreeNodeNamedData(WebDataTableSingleRow table)
				: base(table)
			{
				Text = table.TableName;
				ImageIndex = 9;
				SelectedImageIndex = ImageIndex;
			}
		}
		#endregion
		private void updateLanguagesNodes(CultureInfo current)
		{
			treeViewLanguages.Nodes.Clear();
			IList<string> lnames = XmlUtil.GetLanguages(_projectResourcesNode);
			foreach (string s in lnames)
			{
				try
				{
					CultureInfo ci = new CultureInfo(s);
					TreeNodeCulture tn = new TreeNodeCulture(ci, imageList1);
					treeViewLanguages.Nodes.Add(tn);
					if (current != null)
					{
						if (string.CompareOrdinal(ci.Name, current.Name) == 0)
						{
							treeViewLanguages.SelectedNode = tn;
						}
					}
				}
				catch
				{
				}
			}
			if (treeViewLanguages.SelectedNode == null)
			{
				if (treeViewLanguages.Nodes.Count > 0)
				{
					treeViewLanguages.SelectedNode = treeViewLanguages.Nodes[0];
				}
			}
		}
		private void save()
		{
			//save languages
			_projectResourcesNode.OwnerDocument.Save(_project.ResourcesFile);
			//collect data from nodes
			List<WebDataTable> tbls = new List<WebDataTable>();
			for (int i = 0; i < treeViewDataSet.Nodes[0].Nodes.Count; i++)
			{
				TreeNodeData tnd = treeViewDataSet.Nodes[0].Nodes[i] as TreeNodeData;
				if (tnd != null)
				{
					tnd.Table.FinishEdit();
					tbls.Add(tnd.Table);
				}
			}
			_dataset.SetData(tbls.ToArray());
		}
		private StringCollection getAllDataNames()
		{
			StringCollection names = new StringCollection();
			for (int i = 0; i < treeViewDataSet.Nodes[0].Nodes.Count; i++)
			{
				TreeNodeData tnd = treeViewDataSet.Nodes[0].Nodes[i] as TreeNodeData;
				if (tnd != null)
				{
					names.Add(tnd.TableName);
				}
			}
			return names;
		}
		private void showCulture(CultureInfo c)
		{
			_dataset.SetCurrentLanguage(c.Name);
			Text = string.Format(CultureInfo.InvariantCulture, "{0} - {1} ({2})", _baseCaption, c.Name, c.NativeName);
		}
		private void showData()
		{
			CultureInfo cultrue = null;
			WebDataTable table = null;
			TreeNodeCulture tnc = treeViewLanguages.SelectedNode as TreeNodeCulture;
			if (tnc != null)
			{
				cultrue = tnc.Culture;
				TreeNodeData tnd = treeViewDataSet.SelectedNode as TreeNodeData;
				if (tnd != null)
				{
					table = tnd.Table;
				}
			}
			_currentProperty = null;
			dataGridView1.DataSource = null;
			dataGridView1.DataBindings.Clear();
			dataGridView1.Columns.Clear();
			if (cultrue != null && table != null)
			{
				showCulture(cultrue);
				table.CurrentCulture = cultrue.Name;
				WebDataTableSingleRow singleRow = table as WebDataTableSingleRow;
				if (singleRow != null)
				{
					propertyGrid1.SelectedObject = singleRow;
					propertyGrid1.Visible = true;
					//
					dataGridView1.Visible = false;
				}
				else
				{
					dataGridView1.Visible = true;
					propertyGrid1.Visible = false;
					dataGridView1.DataSource = table.CreateDataTable(cultrue.Name);
				}
			}
			else
			{
				propertyGrid1.SelectedObject = null;
			}
		}
		private void clearDataDisplay()
		{
			_currentProperty = null;
			dataGridView1.DataSource = null;
			dataGridView1.DataBindings.Clear();
			dataGridView1.Columns.Clear();
			propertyGrid1.SelectedObject = null;
		}
		private void mi_renameData(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				TreeNodeData tnd = mi.Tag as TreeNodeData;
				if (tnd != null)
				{
					string newName = DlgSelectName.GetNewName(this, string.Format(CultureInfo.CurrentUICulture, "Rename for data [{0}]", tnd.TableName), getAllDataNames(), true);
					if (!string.IsNullOrEmpty(newName))
					{
						tnd.Table.TableName = newName;
						tnd.Text = newName;
					}
				}
			}
		}
		private void mi_editDataColumns(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				TreeNodeData tnd = mi.Tag as TreeNodeData;
				if (tnd != null)
				{
					DlgDataColumns dlg = new DlgDataColumns();
					dlg.LoadData(tnd.Table.Columns);
					if (dlg.ShowDialog(this) == DialogResult.OK)
					{
						tnd.Table.UpdateColumns(dlg.RetColumns);
						showData();
					}
				}
			}
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			if (_changed)
			{
				save();
			}
			clearDataDisplay();
			this.DialogResult = DialogResult.OK;
		}

		private void buttonLanguage_Click(object sender, EventArgs e)
		{
			TreeNodeCulture tnc = treeViewLanguages.SelectedNode as TreeNodeCulture;
			CultureInfo current = null;
			if (tnc != null)
			{
				current = tnc.Culture;
			}
			StringCollection sc = DlgLanguages.SelectLanguages(this, XmlUtil.GetLanguages(_projectResourcesNode));
			if (sc != null)
			{
				if (sc.Count == 0)
				{
					MessageBox.Show(this, "Do not remove all languages", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					XmlUtil.UpdateLanguages(_projectResourcesNode, sc);
					updateLanguagesNodes(current);
					_changed = true;
				}
			}
		}

		private void btCancel_Click(object sender, EventArgs e)
		{
			if (_changed)
			{
				DialogResult ret = MessageBox.Show(this, "Do you want to save changes?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (ret == DialogResult.Cancel)
				{
					return;
				}
				if (ret == DialogResult.Yes)
				{
					save();
				}
			}
			clearDataDisplay();
			this.DialogResult = DialogResult.Cancel;
		}
		private void mi_addLanguages(object sender, EventArgs e)
		{
			buttonLanguage_Click(sender, e);
		}
		private void mi_removeLanguage(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				TreeNodeCulture tnc = mi.Tag as TreeNodeCulture;
				if (tnc != null)
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Do you want to remove language {0} ({1})", tnc.Culture.Name, tnc.Culture.NativeName), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						XmlUtil.RemoveLanguage(_projectResourcesNode, tnc.Culture.Name);
						_changed = true;
						int n = tnc.Index;
						tnc.Remove();
						if (n < treeViewLanguages.Nodes.Count)
						{
							treeViewLanguages.SelectedNode = treeViewLanguages.Nodes[n];
						}
						else
						{
							if (treeViewLanguages.Nodes.Count > 0)
							{
								treeViewLanguages.SelectedNode = treeViewLanguages.Nodes[treeViewLanguages.Nodes.Count - 1];
							}
							else
							{
								showData();
							}
						}
					}
				}
			}
		}

		private void treeViewLanguages_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu cm = new ContextMenu();
				MenuItem mi = new MenuItem("Add languages", mi_addLanguages);
				cm.MenuItems.Add(mi);
				if (treeViewLanguages.Nodes.Count > 1)
				{
					TreeNode nd = treeViewLanguages.GetNodeAt(e.X, e.Y);
					if (nd != null)
					{
						TreeNodeCulture tnc = nd as TreeNodeCulture;
						if (tnc != null)
						{
							mi = new MenuItem(string.Format(CultureInfo.InvariantCulture, "Remove language {0} ({1})", tnc.Culture.Name, tnc.Culture.NativeName), mi_removeLanguage);
							mi.Tag = tnc;
							cm.MenuItems.Add(mi);
							if (treeViewLanguages.SelectedNode != nd)
							{
								treeViewLanguages.SelectedNode = nd;
							}
						}
					}
				}
				cm.Show(treeViewLanguages, new Point(e.X, e.Y));
			}
		}

		private void treeViewLanguages_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeCulture tnc = e.Node as TreeNodeCulture;
			if (tnc != null)
			{
				showCulture(tnc.Culture);
				showData();
			}
			else
			{
				Text = _baseCaption;
			}
		}

		private void buttonTable_Click(object sender, EventArgs e)
		{
			string newName = DlgSelectName.GetNewName(this, "Name for the new tabled data", getAllDataNames(), true);
			if (!string.IsNullOrEmpty(newName))
			{
				WebDataTable tbl = new WebDataTable(_dataset);
				tbl.TableName = newName;
				TreeNodeTabledData tnt = new TreeNodeTabledData(tbl);
				treeViewDataSet.Nodes[0].Nodes.Add(tnt);
				_changed = true;
				treeViewDataSet.Nodes[0].Expand();
				treeViewDataSet.SelectedNode = tnt;
			}
		}

		private void buttonData_Click(object sender, EventArgs e)
		{
			string newName = DlgSelectName.GetNewName(this, "Name for the new named data", getAllDataNames(), true);
			if (!string.IsNullOrEmpty(newName))
			{
				WebDataTableSingleRow tbl = new WebDataTableSingleRow(_dataset);
				tbl.TableName = newName;
				TreeNodeNamedData tnt = new TreeNodeNamedData(tbl);
				treeViewDataSet.Nodes[0].Nodes.Add(tnt);
				_changed = true;
				treeViewDataSet.Nodes[0].Expand();
				treeViewDataSet.SelectedNode = tnt;
			}
		}
		private void mi_addTabledData(object sender, EventArgs e)
		{
			buttonTable_Click(sender, e);
		}
		private void mi_addNamedData(object sender, EventArgs e)
		{
			buttonData_Click(sender, e);
		}
		private void mi_removeData(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				TreeNodeData tnc = mi.Tag as TreeNodeData;
				if (tnc != null)
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Do you want to remove data [{0}]", tnc.TableName), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						_dataset.RemoveData(tnc.TableName);
						tnc.Remove();
						showData();
					}
				}
			}
		}
		private void treeViewDataSet_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu cm = new ContextMenu();
				MenuItem mi = new MenuItem("Add tabled data", mi_addTabledData);
				cm.MenuItems.Add(mi);
				mi = new MenuItem("Add named data", mi_addNamedData);
				cm.MenuItems.Add(mi);
				//
				TreeNode nd = treeViewDataSet.GetNodeAt(e.X, e.Y);
				if (nd != null)
				{
					TreeNodeData tnc = nd as TreeNodeData;
					if (tnc != null)
					{
						mi = new MenuItem(string.Format(CultureInfo.InvariantCulture, "Rename [{0}]", tnc.TableName), mi_renameData);
						mi.Tag = tnc;
						cm.MenuItems.Add(mi);
						mi = new MenuItem(string.Format(CultureInfo.InvariantCulture, "Edit data names/column names for [{0}]", tnc.TableName), mi_editDataColumns);
						mi.Tag = tnc;
						cm.MenuItems.Add(mi);
						cm.MenuItems.Add(new MenuItem("-"));
						mi = new MenuItem(string.Format(CultureInfo.InvariantCulture, "Remove data [{0}]", tnc.TableName), mi_removeData);
						mi.Tag = tnc;
						cm.MenuItems.Add(mi);
						if (treeViewDataSet.SelectedNode != nd)
						{
							treeViewDataSet.SelectedNode = nd;
						}
					}
				}
				//
				cm.Show(treeViewDataSet, new Point(e.X, e.Y));
			}
		}

		private void treeViewDataSet_AfterSelect(object sender, TreeViewEventArgs e)
		{
			showData();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (!_updating)
			{
				_changed = true;
				if (e.ChangedItem != null)
				{
					_updating = true;
					if (e.ChangedItem.Value != null)
					{
						textBox1.Text = e.ChangedItem.Value.ToString();
					}
					else
					{
						textBox1.Text = "";
					}
					_updating = false;
				}
			}
		}

		private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
		{
			_currentProperty = e.NewSelection.PropertyDescriptor;
			bool b = _updating;
			_updating = true;
			if (e.NewSelection.Value == null)
			{
				textBox1.Text = "";
			}
			else
			{
				textBox1.Text = e.NewSelection.Value.ToString();
			}
			_updating = b;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (!_updating)
			{
				if (_currentProperty != null)
				{
					_changed = true;
					_updating = true;
					_currentProperty.SetValue(sender, textBox1.Text);
					propertyGrid1.Refresh();
					_updating = false;
				}
			}
		}
	}
}
