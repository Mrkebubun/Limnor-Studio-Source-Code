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
using System.Reflection;
using Parser;
using System.Xml;
using VPL;
using LimnorDesigner.MenuUtil;
using VSPrj;
using LimnorDesigner.Property;

namespace LimnorDesigner
{
	public partial class DlgSelectProperty : Form
	{
		public bool FrequentlyUsedMethodsChanged = false;
		public IProperty ReturnPropertyInfo;
		private LimnorContextMenuCollection _menus;
		public DlgSelectProperty()
		{
			InitializeComponent();
			lstP.SetToolTipControl(toolTip1);
			lstS.SetToolTipControl(toolTip1);
		}
		public void SetProject(LimnorProject project)
		{
			treeViewAll.SetProject(project);
		}
		public void LoadData(LimnorContextMenuCollection type)
		{
			_menus = type;
			//
			List<MenuItemDataProperty> ps = _menus.PrimaryProperties;
			if (ps != null && ps.Count > 0)
			{
				foreach (MenuItemDataProperty p in ps)
				{
					lstP.Items.Add(p);
				}
			}
			List<MenuItemDataProperty> sps = _menus.SecondaryProperties;
			if (sps != null && sps.Count > 0)
			{
				foreach (MenuItemDataProperty p in sps)
				{
					lstS.Items.Add(p);
				}
			}
			SortedDictionary<string, IProperty> all = _menus.GetAllProperties();
			SortedDictionary<string, TreeNode> customNodes = new SortedDictionary<string, TreeNode>();
			foreach (KeyValuePair<string, IProperty> kv in all)
			{
				PropertyPointer pi = kv.Value as PropertyPointer;
				if (pi != null)
				{
					treeViewAll.Nodes.Add(new TreeNodeProperty(false, pi));
				}
				else
				{
					PropertyClass pc = kv.Value as PropertyClass;
					if (pc != null)
					{
						//use 0 as the scope method Id because property selection does not method scope
						TreeNodeCustomProperty tnc = new TreeNodeCustomProperty(treeViewAll, pc.IsStatic, pc, 0);
						customNodes.Add(tnc.Text, tnc);
					}
				}
			}
			int i = 0;
			foreach (KeyValuePair<string, TreeNode> kv in customNodes)
			{
				treeViewAll.Nodes.Insert(i++, kv.Value);
			}
			//
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			if (txtName.Resetting)
				return;
			for (int i = 0; i < treeViewAll.Nodes.Count; i++)
			{
				IProperty ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IProperty;
				string key = ep.ObjectKey;
				if (key.StartsWith(txtName.Text, true, System.Globalization.CultureInfo.InvariantCulture))
				{
					treeViewAll.SelectedNode = treeViewAll.Nodes[i];
					txtName.SetText(treeViewAll.Nodes[i].Text);
					break;
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			TreeNodeObject ne = treeViewAll.SelectedNode as TreeNodeObject;
			if (ne != null)
			{
				IProperty ep = ne.OwnerPointer as IProperty;
				ReturnPropertyInfo = ep;
			}
			else
			{
				ReturnPropertyInfo = null;
			}
			if (FrequentlyUsedMethodsChanged)
			{
				XmlNode nodeType = _menus.GetTypeNode();
				XmlNode nodeMethods = nodeType.SelectSingleNode(LimnorContextMenuCollection.XML_Properties);
				if (nodeMethods == null)
				{
					nodeMethods = nodeType.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Properties);
					nodeType.AppendChild(nodeMethods);
				}
				nodeMethods.RemoveAll();
				XmlNode nodePrimary = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Primary);
				nodeMethods.AppendChild(nodePrimary);
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					XmlNode node = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Property);
					node.InnerText = lstP.Items[i].ToString();
					nodePrimary.AppendChild(node);
				}
				XmlNode nodeSecondary = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Secondary);
				nodeMethods.AppendChild(nodeSecondary);
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					XmlNode node = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Property);
					node.InnerText = lstS.Items[i].ToString();
					nodeSecondary.AppendChild(node);
				}
				try
				{
					nodeMethods.OwnerDocument.Save(_menus.FilePath);
					_menus.RemoveMenuCollection();
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, "Error saving frequently used properties", MessageBoxButtons.OK, MessageBoxIcon.Error);
					FrequentlyUsedMethodsChanged = false;
				}
			}
			//===============
			if (ReturnPropertyInfo == null)
			{
				this.DialogResult = DialogResult.Abort;
			}
			else
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		private void btAddPM_Click(object sender, EventArgs e)
		{
			TreeNodeObject ne = treeViewAll.SelectedNode as TreeNodeObject;
			if (ne != null && ne.Parent == null)
			{
				IProperty ep = ne.OwnerPointer as IProperty;
				string key = ep.ObjectKey;
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					MenuItemDataProperty m = lstP.Items[i] as MenuItemDataProperty;
					if (m.Key == key)
					{
						lstP.SelectedIndex = i;
						return;
					}
				}
				MenuItemDataProperty mi = MenuItemDataProperty.CreateMenuItem(ep, _menus);// new PropertyItem(key, ep.PropertyInformation, _type);
				lstP.SelectedIndex = lstP.Items.Add(mi);
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					MenuItemDataProperty m = lstS.Items[i] as MenuItemDataProperty;
					if (m.Key == mi.Key)
					{
						lstS.Items.RemoveAt(i);
						break;
					}
				}
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btAddSM_Click(object sender, EventArgs e)
		{
			TreeNodeObject ne = treeViewAll.SelectedNode as TreeNodeObject;
			if (ne != null && ne.Parent == null)
			{
				IProperty ep = ne.OwnerPointer as IProperty;
				string key = ep.ObjectKey;
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					MenuItemDataProperty m = lstS.Items[i] as MenuItemDataProperty;
					if (m.Key == key)
					{
						lstS.SelectedIndex = i;
						return;
					}
				}
				MenuItemDataProperty mi = MenuItemDataProperty.CreateMenuItem(ep, _menus);
				lstS.SelectedIndex = lstS.Items.Add(mi);
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					MenuItemDataProperty m = lstP.Items[i] as MenuItemDataProperty;
					if (m.Key == mi.Key)
					{
						lstP.Items.RemoveAt(i);
						break;
					}
				}
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btDelMP_Click(object sender, EventArgs e)
		{
			int n = lstP.SelectedIndex;
			if (n >= 0)
			{
				lstP.Items.RemoveAt(n);
				if (n >= lstP.Items.Count)
				{
					n = lstP.Items.Count - 1;
				}
				lstP.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btDelSM_Click(object sender, EventArgs e)
		{
			int n = lstS.SelectedIndex;
			if (n >= 0)
			{
				lstS.Items.RemoveAt(n);
				if (n >= lstS.Items.Count)
				{
					n = lstS.Items.Count - 1;
				}
				lstS.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btUPp_Click(object sender, EventArgs e)
		{
			int n = lstP.SelectedIndex;
			if (n > 0)
			{
				object v1 = lstP.Items[n];
				lstP.Items.RemoveAt(n);
				n--;
				lstP.Items.Insert(n, v1);
				lstP.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btDNp_Click(object sender, EventArgs e)
		{
			int n = lstP.SelectedIndex;
			if (n < lstP.Items.Count - 1)
			{
				object v1 = lstP.Items[n];
				lstP.Items.RemoveAt(n);
				n++;
				lstP.Items.Insert(n, v1);
				lstP.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btUPs_Click(object sender, EventArgs e)
		{
			int n = lstS.SelectedIndex;
			if (n > 0)
			{
				object v1 = lstS.Items[n];
				lstS.Items.RemoveAt(n);
				n--;
				lstS.Items.Insert(n, v1);
				lstS.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void btDNs_Click(object sender, EventArgs e)
		{
			int n = lstS.SelectedIndex;
			if (n < lstS.Items.Count - 1)
			{
				object v1 = lstS.Items[n];
				lstS.Items.RemoveAt(n);
				n++;
				lstS.Items.Insert(n, v1);
				lstS.SelectedIndex = n;
				FrequentlyUsedMethodsChanged = true;
			}
		}

		private void treeViewAll_AfterSelect(object sender, TreeViewEventArgs e)
		{
			btOK.Enabled = false;
			if (e.Node != null)
			{
				TreeNodeProperty tnp = e.Node as TreeNodeProperty;
				if (tnp != null)
				{
					btOK.Enabled = !tnp.Property.IsReadOnly;
				}
				else
				{
					TreeNodeCustomProperty tncp = e.Node as TreeNodeCustomProperty;
					if (tncp != null)
					{
						btOK.Enabled = tncp.Property.CanWrite;
					}
				}
			}
		}

		private void lstP_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = lstP.SelectedIndex;
			if (n >= 0)
			{
				MenuItemDataProperty mi = lstP.SelectedItem as MenuItemDataProperty;
				if (mi != null)
				{
					for (int i = 0; i < treeViewAll.Nodes.Count; i++)
					{
						IProperty ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IProperty;
						string key = ep.ObjectKey;
						if (key == mi.Key)
						{
							treeViewAll.SelectedNode = treeViewAll.Nodes[i];
							break;
						}
					}
					lstS.SelectedIndex = -1;
				}
			}
		}

		private void lstS_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = lstS.SelectedIndex;
			if (n >= 0)
			{
				MenuItemDataProperty mi = lstS.SelectedItem as MenuItemDataProperty;
				if (mi != null)
				{
					for (int i = 0; i < treeViewAll.Nodes.Count; i++)
					{
						IProperty ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IProperty;
						string key = ep.ObjectKey;
						if (key == mi.Key)
						{
							treeViewAll.SelectedNode = treeViewAll.Nodes[i];
							break;
						}
					}
					lstP.SelectedIndex = -1;
				}
			}
		}
	}
}
