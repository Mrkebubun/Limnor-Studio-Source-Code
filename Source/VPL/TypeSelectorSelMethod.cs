/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace VPL
{
	public class TypeSelectorSelMethod:UITypeEditor
	{
		private Font _font = new Font("Times New Roman", 12);
		public TypeSelectorSelMethod()
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
					bool runAtWebClient = false;
					TypeAttribute fa = null;
					if (context.PropertyDescriptor.Attributes != null)
					{
						foreach (Attribute a in context.PropertyDescriptor.Attributes)
						{
							if (fa == null)
							{
								fa = a as TypeAttribute;
							}
							if (a is RunAtWebClientAttribute)
							{
								runAtWebClient = true;
							}
						}
					}
					
					if (fa != null)
					{
						IVplMethodSelector ims = context.Instance as IVplMethodSelector;
						if (ims != null)
						{
							object v = value;
							if (ims.SelectMethodForParam(edSvc, runAtWebClient, fa.Type, ref v))
							{
								value = v;
							}
						}
					}
				}
			}
			return value;
		}
	}
	public class RunAtWebClientAttribute : Attribute
	{
		public RunAtWebClientAttribute()
		{
		}
	}
}
