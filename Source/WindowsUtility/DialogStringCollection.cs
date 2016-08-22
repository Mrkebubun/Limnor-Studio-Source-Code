/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace WindowsUtility
{
	public partial class DialogStringCollection : Form
	{
		public StringCollection Ret;
		private bool _loading;
		public DialogStringCollection()
		{
			InitializeComponent();
		}
		public void LoadData(StringCollection sc)
		{
			Ret = sc;
			if (sc != null && sc.Count > 0)
			{
				for (int i = 0; i < sc.Count; i++)
				{
					treeView1.Nodes.Add(new TreeNode(sc[i]));
				}
			}
		}
		public void LoadData(List<object> sc)
		{
			if (sc != null && sc.Count > 0)
			{
				for (int i = 0; i < sc.Count; i++)
				{
					if (sc[i] == null)
					{
						treeView1.Nodes.Add(new TreeNode(string.Empty));
					}
					else
					{
						treeView1.Nodes.Add(new TreeNode(sc[i].ToString()));
					}
				}
			}
		}
		private void buttonAdd_Click(object sender, EventArgs e)
		{
			TreeNode nd = new TreeNode(string.Empty);
			treeView1.Nodes.Add(nd);
			treeView1.SelectedNode = nd;
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				treeView1.Nodes.Remove(treeView1.SelectedNode);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Ret = new StringCollection();
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				Ret.Add(treeView1.Nodes[i].Text);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			_loading = true;
			textBox1.Text = e.Node.Text;
			_loading = false;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				if (treeView1.SelectedNode != null)
				{
					treeView1.SelectedNode.Text = textBox1.Text;
				}
			}
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					textBox1.Text = dlg.FileName;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
	}
}
