/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using VPL;
using System.Windows.Forms;
using LFilePath;

namespace LimnorDesigner.Web
{
	class TypeEditorHtmlImgSrc : UITypeEditor
	{
		public TypeEditorHtmlImgSrc()
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
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					string title = null;
					string filter = null;
					FilePathAttribute fa = null;
					if (context.PropertyDescriptor.Attributes != null)
					{
						foreach (Attribute a in context.PropertyDescriptor.Attributes)
						{
							fa = a as FilePathAttribute;
							if (fa != null)
							{
								title = fa.Title;
								filter = fa.Pattern;
								break;
							}
						}
					}
					if (string.IsNullOrEmpty(title))
					{
						title = "Select web file";
						filter = "All files|*.*";
					}
					ParameterValue pv = context.Instance as ParameterValue;
					if (pv != null)
					{
						OpenFileDialog dlg = new OpenFileDialog();
						if (value != null)
						{
							dlg.FileName = value.ToString();
						}
						dlg.Title = title;
						dlg.Filter = filter;
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							value = dlg.FileName;
						}
					}
					else
					{
						HtmlElement_Base heb = context.Instance as HtmlElement_Base;
						if (heb != null)
						{
							try
							{
								//the dialog will copy the selected file to web folder
								DialogHtmlFile dlg = new DialogHtmlFile();
								dlg.LoadData(heb.WebPhysicalFolder, value as string, title, filter);
								if (service.ShowDialog(dlg) == DialogResult.OK)
								{
									HtmlElement_object hob = heb as HtmlElement_object;
									if (hob != null && string.CompareOrdinal(context.PropertyDescriptor.Name, "archive") == 0)
									{
										hob.AppendArchiveFile(hob.id, dlg.WebFile);
									}
									else
									{
										heb.SetWebProperty(context.PropertyDescriptor.Name, dlg.WebFile);
										value = dlg.WebFile;
									}
								}
							}
							catch (Exception err)
							{
								MessageBox.Show(err.Message);
							}
						}
					}
				}
			}
			return value;
		}
	}
}
