/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public partial class FormTypeSelection : Form
	{
		private Type _baseType;
		private string _namespace;
		private string _typename;
		public Type SelectedType = null;
		public FormTypeSelection()
		{
			InitializeComponent();
		}
		public static Type SelectType(Form caller, params string[] namespaceAndType)
		{
			FormTypeSelection dlg = new FormTypeSelection();
			dlg.SetForStatic();
			if (namespaceAndType != null)
			{
				if (namespaceAndType.Length > 0)
				{
					dlg._namespace = namespaceAndType[0];
					if (namespaceAndType.Length > 1)
					{
						dlg._typename = namespaceAndType[1];
					}
				}
			}
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.SelectedType;
			}
			return null;
		}
		public void SetForStatic()
		{
			treeView1.SetForStatic();
		}
		public void SetSelectionBaseType(Type type)
		{
			_baseType = type;
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			treeView1.LoadGac(new string[] { _namespace, _typename });
		}

		private void btLoad_Click(object sender, EventArgs e)
		{
			treeView1.LoadDLL();
		}

		private void treeView1_TypeSelected(object sender, EventArgs e)
		{
			btOK.Enabled = false;
			SelectedType = null;

			EventArgObjectSelected os = e as EventArgObjectSelected;
			if (os != null)
			{
				if (os.SelectedObject != null)
				{
					Type t = os.SelectedObject as Type;
					if (t != null)
					{
						if (_baseType == null || _baseType.IsAssignableFrom(t))
						{
							SelectedType = t;
							btOK.Enabled = true;
						}
					}
				}
			}
		}
	}
}
