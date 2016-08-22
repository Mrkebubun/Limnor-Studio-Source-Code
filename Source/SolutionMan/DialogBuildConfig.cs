/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
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
using VSPrj;

namespace SolutionMan
{
	public partial class DialogBuildConfig : Form
	{
		private SolutionNode _sol;
		public DialogBuildConfig()
		{
			InitializeComponent();
		}
		public void LoadData(SolutionNode node)
		{
			_sol = node;
			for (int i = 0; i < node.Nodes.Count; i++)
			{
				ProjectNode pn = node.Nodes[i] as ProjectNode;
				if (pn != null)
				{
					ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
					int n = checkedListBox1.Items.Add(pnd.Project);
					if (node.IsProjectBuildIncluded(pn))
					{
						checkedListBox1.SetItemChecked(n, true);
					}
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				LimnorProject p = checkedListBox1.Items[i] as LimnorProject;
				if (p != null)
				{
					_sol.SetProjectInclude(p.ProjectGuid, checkedListBox1.GetItemChecked(i));
				}
			}
			_sol.SaveConfig();
			this.DialogResult = DialogResult.OK;
		}
	}
}
