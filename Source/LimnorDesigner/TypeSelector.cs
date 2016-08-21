/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// load primary types and Type into the treeview for selection for creating Attribute
	/// </summary>
	public partial class TypeSelector : UserControl
	{
		public Type SelectedType;
		IWindowsFormsEditorService _svc;
		public TypeSelector()
		{
			InitializeComponent();
		}
		public void LoadAttributeParameterTypes(IWindowsFormsEditorService service, EnumWebRunAt runAt, Type initSelection)
		{
			_svc = service;
			treeView1.LoadAttributeParameterTypes(runAt, initSelection);
			treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
			treeView1.Click += new EventHandler(treeView1_Click);
			treeView1.KeyPress += new KeyPressEventHandler(treeView1_KeyPress);
		}

		void treeView1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				if (_svc != null)
				{
					_svc.CloseDropDown();
				}
			}
		}

		void treeView1_Click(object sender, EventArgs e)
		{
			if (_svc != null)
			{
				_svc.CloseDropDown();
			}
		}

		void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeClassType tnct = e.Node as TreeNodeClassType;
			if (tnct != null)
			{
				SelectedType = tnct.OwnerDataType;
			}
		}
		public static Bitmap GetTypeImageByName(string name)
		{
			if (string.CompareOrdinal("Number", name) == 0)
			{
				return Resources._decimal;
			}
			if (string.CompareOrdinal("String", name) == 0)
			{
				return Resources.abc;
			}
			if (string.CompareOrdinal("Array", name) == 0)
			{
				return Resources._array.ToBitmap();
			}
			if (string.CompareOrdinal("DateTime", name) == 0)
			{
				return Resources.date;
			}
			if (string.CompareOrdinal("TimeSpan", name) == 0)
			{
				return Resources.date;
			}
			return Resources._decimal;
		}
		public static TypeSelector GetAttributeParameterDialogue(IWindowsFormsEditorService service, EnumWebRunAt runAt, Type initSelection)
		{
			TypeSelector dlg = new TypeSelector();
			dlg.LoadAttributeParameterTypes(service, runAt, initSelection);
			return dlg;
		}
	}
}
