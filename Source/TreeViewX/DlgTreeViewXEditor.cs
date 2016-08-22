/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
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

namespace Limnor.TreeViewExt
{
	public partial class DlgTreeViewXEditor : Form
	{
		public DlgTreeViewXEditor()
		{
			InitializeComponent();
		}
		public void LoadData(TreeViewX treeView)
		{
			if (treeView.ImageList != null)
			{
				for (int i = 0; i < treeView.ImageList.Images.Count; i++)
				{
					imageList1.Images.Add(treeView.ImageList.Images[i]);
				}
			}
			treeView1.XmlString = treeView.XmlString;
			treeView1.SetEditMenu();
		}
		public string XmlString
		{
			get
			{
				//treeView1.ExpandAll();
				return treeView1.SaveToXmlDocument().OuterXml;
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeShortcut tns = e.Node as TreeNodeShortcut;
			if (tns != null)
			{
				propertyGrid1.SelectedObject = tns;
				//
				buttonAddValue.Enabled = false;
				buttonDelNode.Enabled = true;
				buttonDelVal.Enabled = false;
				buttonSubNode.Enabled = false;
			}
			else
			{
				TreeNodeX tnx = e.Node as TreeNodeX;
				if (tnx != null)
				{
					propertyGrid1.SelectedObject = tnx;
					//
					buttonAddValue.Enabled = !tnx.IsShortcut;
					buttonDelNode.Enabled = !tnx.IsShortcut;
					buttonDelVal.Enabled = false;
					buttonSubNode.Enabled = !tnx.IsShortcut;
				}
				else
				{
					TreeNodeValue tnv = e.Node as TreeNodeValue;
					if (tnv != null)
					{
						propertyGrid1.SelectedObject = tnv.Data;
						//
						buttonAddValue.Enabled = false;
						buttonDelNode.Enabled = false;
						buttonDelVal.Enabled = true;
						buttonSubNode.Enabled = false;
					}
				}
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			TreeNodeX tnx = propertyGrid1.SelectedObject as TreeNodeX;
			if (tnx != null)
			{
				if (!tnx.IsShortcut)
				{
					treeView1.SyncShortcuts(tnx);
				}
			}
		}

		private void buttonRootNode_Click(object sender, EventArgs e)
		{
			treeView1.AddRootNode("Root node");
		}

		private void buttonSubNode_Click(object sender, EventArgs e)
		{
			treeView1.AddSubNode("Sub node");
		}

		private void buttonDelNode_Click(object sender, EventArgs e)
		{
			treeView1.DeleteSelectedCategoryNode(true);
		}

		private void buttonAddValue_Click(object sender, EventArgs e)
		{
			treeView1.CreateNewProperty();
		}

		private void buttonDelVal_Click(object sender, EventArgs e)
		{
			treeView1.DeleteSelectedValue(true);
		}
	}
}
