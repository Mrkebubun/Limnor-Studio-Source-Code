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
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Specialized;
using VSPrj;
using MathExp;

namespace LimnorDesigner
{
	public partial class DlgSelAssemblies : Form, IWithProject
	{
		public Type SelectedType = null;
		private LimnorProject _project;
		private List<Type> _types;
		private bool _changed;
		private UInt32 _scopeMethodId;
		public DlgSelAssemblies()
		{
			InitializeComponent();
		}
		public void LoadUsedTypes(LimnorProject project, UInt32 scopeMethodId)
		{
			_project = project;
			_scopeMethodId = scopeMethodId;
			StringCollection sc = new StringCollection();
			_types = _project.GetToolboxItems(sc);
			if (sc.Count > 0)
			{
				MathNode.Log(sc);
			}
			foreach (Type t in _types)
			{
				tvUsed.Nodes.Add(new TreeNodeClassType(tvUsed, t, scopeMethodId));
			}
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			tvGac.LoadGac();
			//
			StringCollection sc = new StringCollection();
			List<Assembly> refs = _project.GetReferences(sc);
			if (sc.Count > 0)
			{
				MathNode.Log(sc);
			}
			foreach (Assembly a in refs)
			{
				tvGac.Nodes.Add(new TreeNodeAssembly(a));
			}
			//add an empty node to make sure all nodes are visible
			tvGac.Nodes.Add(new TreeNodeDummy());
		}

		private void tvGac_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ITypeNode n = e.Node as ITypeNode;
			if (n != null)
			{
				SelectedType = n.OwnerDataType;
				btOK.Enabled = true;
				tvUsed.SelectedNode = null;
			}
			else
			{
				btOK.Enabled = false;
			}
		}

		private void tvUsed_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ITypeNode n = e.Node as ITypeNode;
			if (n != null)
			{
				SelectedType = n.OwnerDataType;
				btOK.Enabled = true;
				tvGac.SelectedNode = null;
			}
			else
			{
				btOK.Enabled = false;
			}
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			tvGac.LoadDLL();
		}

		private void btUse_Click(object sender, EventArgs e)
		{
			ITypeNode n = tvGac.SelectedNode as ITypeNode;
			if (n != null)
			{
				for (int i = 0; i < tvUsed.Nodes.Count; i++)
				{
					ITypeNode m = tvUsed.Nodes[i] as ITypeNode;
					if (m != null)
					{
						if (n.OwnerDataType.Equals(m.OwnerDataType))
						{
							return;
						}
					}
				}
				tvUsed.Nodes.Add(new TreeNodeClassType(tvUsed, n.OwnerDataType, _scopeMethodId));
				_changed = true;
			}
		}

		private void btNotUse_Click(object sender, EventArgs e)
		{
			ITypeNode n = tvUsed.SelectedNode as ITypeNode;
			if (n != null)
			{
				int i = tvUsed.SelectedNode.Index;
				tvUsed.SelectedNode.Remove();
				_changed = true;
				if (tvUsed.Nodes.Count > 0)
				{
					if (i < tvUsed.Nodes.Count)
					{
						tvUsed.SelectedNode = tvUsed.Nodes[i];
					}
					else
					{
						tvUsed.SelectedNode = tvUsed.Nodes[tvUsed.Nodes.Count - 1];
					}
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (_changed)
			{
				List<Type> list = new List<Type>();
				for (int i = 0; i < tvUsed.Nodes.Count; i++)
				{
					ITypeNode n = tvUsed.Nodes[i] as ITypeNode;
					if (n != null)
					{
						list.Add(n.OwnerDataType);
					}
				}
				_project.AddToolboxItems(list.ToArray(), false);
			}
			this.DialogResult = DialogResult.OK;
		}

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _project; }
		}

		#endregion
	}
}
