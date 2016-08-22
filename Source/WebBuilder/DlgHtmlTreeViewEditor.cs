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
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using VPL;

namespace Limnor.WebBuilder
{
	public partial class DlgHtmlTreeViewEditor : Form, IHtmlTreeViewDesigner
	{
		public DlgHtmlTreeViewEditor()
		{
			InitializeComponent();
		}
		private void copyNodes(TreeNodeCollection nodesSrc, TreeNodeCollection nodesTgt, ImageList imageList)
		{
			foreach (TreeNode sn in nodesSrc)
			{
				HtmlTreeNode sn0 = sn as HtmlTreeNode;
				if (sn0 != null)
				{
					HtmlTreeNode tn = sn0.CloneHtmlTreeNode();
					nodesTgt.Add(tn);
					tn.ImageIndex = VPLUtil.GetImageIndex(tn.IconImagePath, imageList);
					tn.SelectedImageIndex = tn.ImageIndex;
					copyNodes(sn0.Nodes, tn.Nodes, imageList);
				}
			}
		}
		public void LoadData(HtmlTreeView treeView)
		{
			copyNodes(treeView.Nodes, treeView1.Nodes, imageList1);
		}
		public void SaveData(HtmlTreeView treeView)
		{
			treeView.Nodes.Clear();
			if (treeView.ImageList == null)
			{
				IWebPage wp = treeView.FindForm() as IWebPage;
				if (wp != null)
				{
					treeView.ImageList = wp.AddComponent(typeof(ImageList)) as ImageList;
				}
			}
			copyNodes(treeView1.Nodes, treeView.Nodes, treeView.ImageList);
		}

		public int GetImageIndex(string filepath)
		{
			return VPLUtil.GetImageIndex(filepath, imageList1);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			HtmlTreeNode tnx = e.Node as HtmlTreeNode;
			propertyGrid1.SelectedObject = tnx;
			buttonDelNode.Enabled = (tnx != null);
			buttonSubNode.Enabled = (tnx != null);
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			HtmlTreeNode tnx = propertyGrid1.SelectedObject as HtmlTreeNode;
			if (tnx != null)
			{
			}
		}

		private void buttonRootNode_Click(object sender, EventArgs e)
		{
			HtmlTreeNode tn = new HtmlTreeNode();
			tn.Text = "New root node";
			tn.ImageIndex = -1;
			tn.SelectedImageIndex = -1;
			treeView1.Nodes.Add(tn);
		}

		private void buttonSubNode_Click(object sender, EventArgs e)
		{
			TreeNode sn = treeView1.SelectedNode;
			if (sn != null)
			{
				HtmlTreeNode tn = new HtmlTreeNode();
				tn.Text = "New sub node";
				tn.ImageIndex = -1;
				tn.SelectedImageIndex = -1;
				sn.Nodes.Add(tn);
			}
		}

		private void buttonDelNode_Click(object sender, EventArgs e)
		{
			TreeNode sn = treeView1.SelectedNode;
			if (sn != null)
			{
				if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Do you want to remove the selected node? {0}", sn.Text), "Remove selected node", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					sn.Remove();
				}
			}
		}

	}
}
