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

namespace LimnorDesigner.ResourcesManager
{
	public partial class DlgSelectResourceName : Form
	{
		public ResourcePointer SelectedResource;
		private ProjectResources _resman;
		public DlgSelectResourceName()
		{
			InitializeComponent();
		}
		public void LoadData(ProjectResources rm)
		{
			_resman = rm;
			TreeNodeResourceCollection.AddResourceCollections(_resman, treeView1.Nodes, true);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			TreeNodeResourceCode tnr = treeView1.SelectedNode as TreeNodeResourceCode;
			if (tnr != null)
			{
				SelectedResource = tnr.Pointer.Resource;
				this.DialogResult = DialogResult.OK;
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeResourceCode tnr = e.Node as TreeNodeResourceCode;
			buttonOK.Enabled = (tnr != null);
		}
	}
}
