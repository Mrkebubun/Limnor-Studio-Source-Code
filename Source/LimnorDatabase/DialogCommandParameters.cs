/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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
using System.Globalization;

namespace LimnorDatabase
{
	public partial class DialogCommandParameters : Form
	{
		public DbParameterListExt Results;
		public DialogCommandParameters()
		{
			InitializeComponent();
		}
		public void LoadData(DbParameterListExt list)
		{
			Results = list;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					listBox1.Items.Add(list[i]);
					list[i].AfterNameChange += new EventHandler(DialogCommandParameters_AfterNameChange);
				}
			}
			else
			{
				Results = new DbParameterListExt();
			}
		}

		void DialogCommandParameters_AfterNameChange(object sender, EventArgs e)
		{
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				if (sender == listBox1.Items[i])
				{
					listBox1.Items.RemoveAt(i);
					listBox1.Items.Insert(i, sender);
					listBox1.SelectedIndex = i;
					break;
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			Results = new DbParameterListExt();
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				Results.Add(listBox1.Items[i] as DbCommandParam);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btNew_Click(object sender, EventArgs e)
		{
			int n = 1;
			string nm = "@p1";
			while (true)
			{
				bool bFound = false;
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					DbCommandParam p = listBox1.Items[i] as DbCommandParam;
					if (p != null)
					{
						if (string.Compare(nm, p.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							bFound = true;
							break;
						}
					}
				}
				if (bFound)
				{
					n++;
					nm = string.Format(CultureInfo.InvariantCulture, "@p{0}", n);
				}
				else
				{
					break;
				}
			}
			DbCommandParam pNew = new DbCommandParam(new EPField());
			pNew.Name = nm;
			if (Results == null)
			{
				Results = new DbParameterListExt();
			}
			Results.Add(pNew);
			pNew.AfterNameChange += DialogCommandParameters_AfterNameChange;
			n = listBox1.Items.Add(pNew);
			listBox1.SelectedIndex = n;
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				if (MessageBox.Show(this, "Do you want to remove selected parameter?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					listBox1.Items.RemoveAt(n);
					if (n < listBox1.Items.Count)
					{
						listBox1.SelectedIndex = n;
					}
					else
					{
						listBox1.SelectedIndex = listBox1.Items.Count - 1;
					}
				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				DbCommandParam p = listBox1.Items[n] as DbCommandParam;
				if (p != null)
				{
					propertyGrid1.SelectedObject = p;
				}
			}
		}
	}
}
