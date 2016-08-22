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
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace VPL
{
	public class ComponentReferenceSelectorTypeAttribute : Attribute
	{
		private Type _type;
		public ComponentReferenceSelectorTypeAttribute(Type type)
			: base()
		{
			_type = type;
		}
		public Type ScopeType
		{
			get
			{
				return _type;
			}
		}
	}
	public class ComponentReferenceSelectorTypeWebAttribute : Attribute
	{
		private Type _type;
		public ComponentReferenceSelectorTypeWebAttribute(Type type)
			: base()
		{
			_type = type;
		}
		public Type ScopeType
		{
			get
			{
				return _type;
			}
		}
	}
	/// <summary>
	/// select a compoennt from a container. 
	/// it needs ComponentReferenceSelectorTypeAttribute to specify the type of the components
	/// </summary>
	public class ComponentReferenceSelector : UITypeEditor
	{
		public ComponentReferenceSelector()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					Type scopeType = null;
					Type scopeTypeWeb = null;
					if (context.PropertyDescriptor.Attributes != null)
					{
						foreach (Attribute a in context.PropertyDescriptor.Attributes)
						{
							ComponentReferenceSelectorTypeAttribute cr = a as ComponentReferenceSelectorTypeAttribute;
							if (cr != null)
							{
								scopeType = cr.ScopeType;
							}
							else
							{
								ComponentReferenceSelectorTypeWebAttribute crw = a as ComponentReferenceSelectorTypeWebAttribute;
								if (crw != null)
								{
									scopeTypeWeb = crw.ScopeType;
								}
							}
						}
					}
					if (scopeType != null || scopeTypeWeb != null)
					{
						bool bForName = false;
						IComponent ic = context.Instance as IComponent;
						if (ic == null)
						{
							IClassId icid = context.Instance as IClassId;
							if (icid != null)
							{
								ic = icid.ObjectInstance as IComponent;
							}
						}
						if (ic == null)
						{
							CollectionComponentNames cc = context.Instance as CollectionComponentNames;
							if (cc != null)
							{
								ic = cc.Owner;
								bForName = true;
							}
						}
						if (ic != null && ic.Site != null)
						{
							ComponentList list = new ComponentList(edSvc);
							list.Items.Add(string.Empty);
							foreach (IComponent c in ic.Site.Container.Components)
							{
								Type t = c.GetType();
								if (scopeType != null && scopeType.IsAssignableFrom(t))
								{
									list.Items.Add(c);
								}
								else if (scopeTypeWeb != null && scopeTypeWeb.IsAssignableFrom(t))
								{
									list.Items.Add(c);
								}
							}
							if (list.Items.Count > 0)
							{
								edSvc.DropDownControl(list);
								if (list.SelectedObj != null)
								{
									if (typeof(string).Equals(list.SelectedObj.GetType()))
										value = null;
									else
									{
										if (bForName)
										{
											IComponent ic2 = list.SelectedObj as IComponent;
											if (ic2 != null && ic2.Site != null)
											{
												value = ic2.Site.Name;
											}
										}
										else
										{
											value = list.SelectedObj;
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
		class ComponentList : ListBox
		{
			private IWindowsFormsEditorService _service;
			public object SelectedObj;
			public ComponentList(IWindowsFormsEditorService service)
			{
				_service = service;
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				if (SelectedIndex >= 0)
				{
					SelectedObj = Items[SelectedIndex];
				}
				_service.CloseDropDown();
			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					if (SelectedIndex >= 0)
					{
						SelectedObj = Items[SelectedIndex];
					}
					_service.CloseDropDown();
				}
			}
		}
	}
}
