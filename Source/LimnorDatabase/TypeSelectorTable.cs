/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VPL;
using System.Windows.Forms;

namespace LimnorDatabase
{
	public class TypeSelectorTable : UITypeEditor
	{
		public TypeSelectorTable()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					IDatabaseConnectionUserExt dbu = context.Instance as IDatabaseConnectionUserExt;
					if (dbu != null)
					{
						if (!(dbu.DatabaseConnection != null && dbu.DatabaseConnection.ConnectionObject != null && dbu.DatabaseConnection.ConnectionObject.IsConnectionReady))
						{
							ConnectionItem ci = dbu.DatabaseConnection;
							DlgConnectionManager dlg = new DlgConnectionManager();
							dlg.UseProjectScope = true;
							dlg.SetSelection(ci);
							if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
							{
								dbu.DatabaseConnection = dlg.SelectedConnection;
							}
						}
						if (dbu.DatabaseConnection != null && dbu.DatabaseConnection.ConnectionObject != null && dbu.DatabaseConnection.ConnectionObject.IsConnectionReady)
						{
							string v = value as string;
							string[] vs = dbu.DatabaseConnection.ConnectionObject.GetTableNames();
							if (vs != null && vs.Length > 0)
							{
								ValueList list = new ValueList(edSvc, vs, v);
								edSvc.DropDownControl(list);
								if (list.MadeSelection)
								{
									value = list.Selection;
								}
							}
						}
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		class ValueList : ListBox
		{
			public bool MadeSelection;
			public object Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, string[] values, string v)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
					for (int i = 0; i < Items.Count; i++)
					{
						if (string.Compare(v, Items[i] as string, StringComparison.OrdinalIgnoreCase) == 0)
						{
							SelectedIndex = i;
							break;
						}
					}
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = Items[SelectedIndex];
				}
				_service.CloseDropDown();
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				finishSelection();

			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					finishSelection();
				}
			}
		}
	}
}
