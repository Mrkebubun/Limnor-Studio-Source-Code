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
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Limnor.WebBuilder;

namespace VPL
{
	/// <summary>
	/// web client event switch
	/// </summary>
	public class TypeSelectorSelEvent:UITypeEditor
	{
		public TypeSelectorSelEvent()
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
					TypeAttribute fa = null;
					if (context.PropertyDescriptor.Attributes != null)
					{
						foreach (Attribute a in context.PropertyDescriptor.Attributes)
						{
							fa = a as TypeAttribute;
							if (fa != null)
							{
								break;
							}
						}
					}
					if (fa != null)
					{
						EventInfo[] eifs = fa.Type.GetEvents();
						if (eifs != null)
						{
							List<string> nms = new List<string>();
							nms.Add(string.Empty);
							for (int i = 0; i < eifs.Length; i++)
							{
								if (WebClientMemberAttribute.IsClientEvent(eifs[i]))
								{
									nms.Add(eifs[i].Name);
								}
							}
							ValueList list = new ValueList(edSvc, nms.ToArray());
							edSvc.DropDownControl(list);
							if (list.MadeSelection)
							{
								value = list.Selection;
							}
						}
					}
				}
			}
			return value;
		}
		class ValueList : ListBox
		{
			public bool MadeSelection;
			public string Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, string[] values)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = Items[SelectedIndex] as string;
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
	public class TypeAttribute : Attribute
	{
		private Type _type;
		public TypeAttribute(Type t)
		{
			_type = t;
		}
		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
	public class MethodParanAttribute : Attribute
	{
		private string _name;
		public MethodParanAttribute(string name)
		{
			_name = name;
		}
		public string ParamName
		{
			get
			{
				return _name;
			}
		}
	}
}
