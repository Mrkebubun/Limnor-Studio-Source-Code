/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace VPL
{
	public interface IWithCollection
	{
		void OnCollectionChanged(string propertyName);
		void OnItemCreated(string propertyName, object obj);
	}
	/// <summary>
	/// sub-class CollectionEditor so that we can catch the change event
	/// </summary>
	public class CollectionEditorX : CollectionEditor
	{
		private IWithCollection iw;
		private string _propertyName;
		public CollectionEditorX(Type type)
			: base(type)
		{
		}
		protected override object CreateInstance(Type itemType)
		{
			object v = base.CreateInstance(itemType);
			if (iw != null)
			{
				iw.OnItemCreated(_propertyName, v);
			}
			return v;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			iw = context.Instance as IWithCollection;
			_propertyName = context.PropertyDescriptor.Name;
			object v = base.EditValue(context, provider, value);
			if (iw != null)
			{
				iw.OnCollectionChanged(context.PropertyDescriptor.Name);
			}
			if (provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					Type t = edSvc.GetType();
					PropertyInfo pif0 = t.GetProperty("OwnerGrid");
					if (pif0 != null)
					{
						object g = pif0.GetValue(edSvc, null);
						PropertyGrid pg = g as PropertyGrid;
						if (pg != null)
						{
							pg.Refresh();
						}
					}
				}
			}
			return v;
		}
	}
}
