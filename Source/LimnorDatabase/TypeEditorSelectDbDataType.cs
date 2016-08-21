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
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows.Forms;

namespace LimnorDatabase
{
	class TypeEditorSelectDbDataType : UITypeEditor
	{
		public TypeEditorSelectDbDataType()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					DbTypeListBox ct = new DbTypeListBox(edSvc);
					if (value != null)
					{
						if (value.GetType().Equals(typeof(OleDbType)))
						{
							OleDbType t = (OleDbType)value;
							ct.SetSelectionByOleDbType(t);
						}
					}
					edSvc.DropDownControl(ct);

					if (ct.SelectedOleDbType != OleDbType.IUnknown)
					{
						value = ct.SelectedOleDbType;
					}
				}
			}
			return value;
		}
	}
}
