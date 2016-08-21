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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing;

namespace LimnorDatabase
{
	class TypeEditorSelectFieldNames : UITypeEditor
	{
		private Font _font;
		public TypeEditorSelectFieldNames()
		{
			_font = new Font("Times New Roman", 8);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			base.PaintValue(e);
			string[] ss = e.Value as string[];
			if (ss != null && ss.Length > 0)
			{
				string s;
				if (ss.Length > 1)
				{
					StringBuilder sb = new StringBuilder(ss[0]);
					for (int i = 1; i < ss.Length; i++)
					{
						sb.Append(",");
						sb.Append(ss[i]);
					}
					s = sb.ToString();
				}
				else
				{
					s = ss[0];
				}
				e.Graphics.DrawString(s, _font, Brushes.Black, 2F, 2F);
			}
			//
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
					EasyGridDetail dg = context.Instance as EasyGridDetail;
					if (dg != null)
					{
						UserControlSelectFieldNames fns = new UserControlSelectFieldNames();
						string[] ss = value as string[];
						fns.LoadData(edSvc, dg.GetFieldNames(context.PropertyDescriptor.Name), ss);
						edSvc.DropDownControl(fns);
						if (fns.SelectedStrings != null)
						{
							value = fns.SelectedStrings;
						}
					}
					else
					{
						EasyDataSet eds = context.Instance as EasyDataSet;
						if (eds != null)
						{
							EasyDataSet master = eds.Master as EasyDataSet;
							if (master != null)
							{
								if (eds.Field_Name != null && master.Field_Name != null)
								{
									List<string> commonFlds = new List<string>();
									for (int i = 0; i < master.Field_Name.Length; i++)
									{
										for (int j = 0; j < eds.Field_Name.Length; j++)
										{
											if (string.Compare(eds.Field_Name[j], master.Field_Name[i], StringComparison.OrdinalIgnoreCase) == 0)
											{
												commonFlds.Add(master.Field_Name[i]);
												break;
											}
										}
									}
									if (commonFlds.Count > 0)
									{
										UserControlSelectFieldNames fns = new UserControlSelectFieldNames();
										string[] ss = value as string[];
										fns.LoadData(edSvc, commonFlds.ToArray(), ss);
										edSvc.DropDownControl(fns);
										if (fns.SelectedStrings != null)
										{
											value = fns.SelectedStrings;
										}
									}
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
}
