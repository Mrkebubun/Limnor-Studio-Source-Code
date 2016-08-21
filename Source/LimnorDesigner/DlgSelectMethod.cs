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
using ProgElements;
using VSPrj;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner
{
	public partial class DlgSelectMethod : Form
	{
		public bool FrequentlyUsedMethodsChanged = false;
		public IMethod ReturnMethodInfo;
		private LimnorContextMenuCollection _menus;
		public DlgSelectMethod()
		{
			InitializeComponent();
			lstP.SetToolTipControl(toolTip1);
			lstS.SetToolTipControl(toolTip1);
		}
		public void SetProject(LimnorProject project)
		{
			treeViewAll.SetProject(project);
		}
		public void LoadData(LimnorContextMenuCollection menus)
		{
			_menus = menus;
			List<MenuItemDataMethod> pms = _menus.PrimaryMethods;
			if (pms != null && pms.Count > 0)
			{
				foreach (MenuItemDataMethod m in pms)
				{
					lstP.Items.Add(m);
				}
			}
			List<MenuItemDataMethod> sms = _menus.SecondaryMethods;
			if (sms != null && sms.Count > 0)
			{
				foreach (MenuItemDataMethod m in sms)
				{
					lstS.Items.Add(m);
				}
			}
			SortedDictionary<string, IMethod> all = _menus.GetAllMethods();
			SortedDictionary<string, TreeNode> customNodes = new SortedDictionary<string, TreeNode>();
			foreach (KeyValuePair<string, IMethod> kv in all)
			{
				MethodInfoPointer mi = kv.Value as MethodInfoPointer;
				if (mi != null)
				{
					treeViewAll.Nodes.Add(new TreeNodeMethod(false, mi));
				}
				else
				{
					MethodClass mc = kv.Value as MethodClass;
					if (mc != null)
					{
						//use 0 as the scope because method selection does not need scope
						TreeNodeCustomMethod tnc = new TreeNodeCustomMethod(treeViewAll, mc.IsStatic, mc, menus.Pointer, 0);
						customNodes.Add(tnc.Text, tnc);
					}
				}
			}
			int i = 0;
			foreach (KeyValuePair<string, TreeNode> kv in customNodes)
			{
				treeViewAll.Nodes.Insert(i++, kv.Value);
			}
		}
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			if (txtName.Resetting)
				return;
			for (int i = 0; i < treeViewAll.Nodes.Count; i++)
			{
				IMethod ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IMethod;
				string key = ep.MethodSignature;
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
				ReturnMethodInfo = ne.OwnerPointer as IMethod;
			}
			else
			{
				ReturnMethodInfo = null;
			}
			if (FrequentlyUsedMethodsChanged)
			{
				XmlNode nodeType = _menus.GetTypeNode();
				XmlNode nodeMethods = nodeType.SelectSingleNode(LimnorContextMenuCollection.XML_Methods);
				if (nodeMethods == null)
				{
					nodeMethods = nodeType.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Methods);
					nodeType.AppendChild(nodeMethods);
				}
				nodeMethods.RemoveAll();
				XmlNode nodePrimary = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Primary);
				nodeMethods.AppendChild(nodePrimary);
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					XmlNode node = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Method);
					node.InnerText = lstP.Items[i].ToString();
					nodePrimary.AppendChild(node);
				}
				XmlNode nodeSecondary = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Secondary);
				nodeMethods.AppendChild(nodeSecondary);
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					XmlNode node = nodeMethods.OwnerDocument.CreateElement(LimnorContextMenuCollection.XML_Method);
					node.InnerText = lstS.Items[i].ToString();
					nodeSecondary.AppendChild(node);
				}
				try
				{
					nodeMethods.OwnerDocument.Save(_menus.FilePath);
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, "Error saving frequently used methods", MessageBoxButtons.OK, MessageBoxIcon.Error);
					FrequentlyUsedMethodsChanged = false;
				}
			}
			//===============
			if (ReturnMethodInfo == null)
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
				IMethod ep = ne.OwnerPointer as IMethod;
				string key = ep.MethodSignature;
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					MenuItemDataMethod m = lstP.Items[i] as MenuItemDataMethod;
					if (m.Key == key)
					{
						lstP.SelectedIndex = i;
						return;
					}
				}
				MenuItemDataMethod mi = MenuItemDataMethod.CreateMenuItem(ep, _menus);
				lstP.SelectedIndex = lstP.Items.Add(mi);
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					MenuItemDataMethod m = lstS.Items[i] as MenuItemDataMethod;
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
				IMethod ep = ne.OwnerPointer as IMethod;
				string key = ep.MethodSignature;
				for (int i = 0; i < lstS.Items.Count; i++)
				{
					MenuItemDataMethod m = lstS.Items[i] as MenuItemDataMethod;
					if (m.Key == key)
					{
						lstS.SelectedIndex = i;
						return;
					}
				}
				MenuItemDataMethod mi = MenuItemDataMethod.CreateMenuItem(ep, _menus);
				lstS.SelectedIndex = lstS.Items.Add(mi);
				for (int i = 0; i < lstP.Items.Count; i++)
				{
					MenuItemDataMethod m = lstP.Items[i] as MenuItemDataMethod;
					if (m.Key == key)
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
				if (e.Node.Parent == null)
				{
					btOK.Enabled = true;
				}
			}
		}

		private void lstP_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = lstP.SelectedIndex;
			if (n >= 0)
			{
				MenuItemDataMethod mi = lstP.SelectedItem as MenuItemDataMethod;
				if (mi != null)
				{
					for (int i = 0; i < treeViewAll.Nodes.Count; i++)
					{
						IMethod ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IMethod;
						string key = ep.MethodSignature;
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
				MenuItemDataMethod mi = lstS.SelectedItem as MenuItemDataMethod;
				if (mi != null)
				{
					for (int i = 0; i < treeViewAll.Nodes.Count; i++)
					{
						IMethod ep = ((TreeNodeObject)(treeViewAll.Nodes[i])).OwnerPointer as IMethod;
						string key = ep.MethodSignature;
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
