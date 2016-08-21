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
using VPL;

namespace LimnorDatabase
{
	public partial class DlgFieldEditorEnum : DlgSetEditorAttributes
	{
		private Type _type;
		public DlgFieldEditorEnum()
		{
			InitializeComponent();
		}
		public DlgFieldEditorEnum(DataEditor current)
			: base(current)
		{
			InitializeComponent();
			DataEditorLookupEnum de = current as DataEditorLookupEnum;
			if (de != null)
			{
				_type = de.EnumType;
				if (_type != null && _type.IsEnum)
				{
					labelEnum.Text = _type.AssemblyQualifiedName;
					listBox1.Items.Clear();
					string[] names = Enum.GetNames(_type);
					if (names != null && names.Length > 0)
					{
						for (int i = 0; i < names.Length; i++)
						{
							listBox1.Items.Add(names[i]);
						}
					}
				}
			}
		}

		private void buttonEnum_Click(object sender, EventArgs e)
		{
			FormTypeSelection dlg = new FormTypeSelection();
			dlg.Text = "Select enumerator type";
			dlg.SetSelectionBaseType(typeof(Enum));
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				if (dlg.SelectedType.IsEnum)
				{
					_type = dlg.SelectedType;
					labelEnum.Text = _type.AssemblyQualifiedName;
					listBox1.Items.Clear();
					string[] names = Enum.GetNames(_type);
					if (names != null && names.Length > 0)
					{
						for (int i = 0; i < names.Length; i++)
						{
							listBox1.Items.Add(names[i]);
						}
					}
					buttonOK.Enabled = true;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_type != null && _type.IsEnum)
			{
				DataEditorLookupEnum de = this.SelectedEditor as DataEditorLookupEnum;
				if (de == null)
				{
					de = new DataEditorLookupEnum();
					this.SetSelection(de);
				}
				de.SetType(_type);
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
