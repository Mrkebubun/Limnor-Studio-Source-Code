/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace WindowsUtility
{
	/// <summary>
	/// it is for replacing the system StringCollectionEditor because the system one does not work
	/// </summary>
	public class StringCollectionEditor : UITypeEditor
	{
		public StringCollectionEditor()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					try
					{
						List<object> list = null;
						DialogStringCollection dlg = new DialogStringCollection();
						StringCollection sc = value as StringCollection;
						if (sc != null)
						{
							dlg.LoadData(sc);
						}
						else
						{
							list = value as List<object>;
							if (list != null)
							{
								dlg.LoadData(list);
							}
						}
						if (edSvc.ShowDialog(dlg) == DialogResult.OK)
						{
							if (list != null)
							{
								list = new List<object>();
								foreach (string s in dlg.Ret)
								{
									list.Add(s);
								}
								return list;
							}
							else
							{
								if (sc != null)
								{
									return dlg.Ret;
								}
							}
						}
					}
					catch (Exception err)
					{
						MessageBox.Show(err.Message);
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
}
