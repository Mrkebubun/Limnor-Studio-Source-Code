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
using XmlUtility;
using VPL;

namespace Limnor.WebBuilder
{
	public partial class DialogJsVariables : Form
	{
		private VplPropertyBag _vars;
		public DialogJsVariables()
		{
			InitializeComponent();
		}
		public void LoadData(VplPropertyBag vars)
		{
			_vars = vars;
			IList<TypedNamedValue> ps = vars.PropertyList;
			foreach (TypedNamedValue tn in ps)
			{
				checkedListBox1.Items.Add(tn.Name, true);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				if (!checkedListBox1.GetItemChecked(i))
				{
					string name = checkedListBox1.Items[i] as string;
					_vars.RemoveItemByKey(name);
				}
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
