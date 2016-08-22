/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PerformerImport
{
	public partial class DlgManageReferences : Form
	{
		public Type[] RetTypes;
		public DlgManageReferences()
		{
			InitializeComponent();
		}
		public void AddType(Type t)
		{
			listBox1.Items.Add(t);
		}

		private void btDel_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				listBox1.Items.RemoveAt(n);
				if (n >= listBox1.Items.Count)
				{
					n = listBox1.Items.Count - 1;
				}
				listBox1.SelectedIndex = n;
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			RetTypes = new Type[listBox1.Items.Count];
			listBox1.Items.CopyTo(RetTypes, 0);
			this.DialogResult = DialogResult.OK;
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			frmPerformerImport f = new frmPerformerImport();
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Type[] types = frmPerformerImport.WizardInfo.GetSelectedTypes();
				if (types != null && types.Length > 0)
				{
					for (int i = 0; i < types.Length; i++)
					{
						bool b = false;
						for (int j = 0; j < listBox1.Items.Count; j++)
						{
							Type t = listBox1.Items[j] as Type;
							if (types[i].AssemblyQualifiedName == t.AssemblyQualifiedName)
							{
								b = true;
								break;
							}
						}
						if (!b)
						{
							listBox1.Items.Add(types[i]);
						}
					}
				}
			}
		}
	}
}
