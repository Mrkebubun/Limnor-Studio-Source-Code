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
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using VSPrj;
using System.Collections;
using System.Windows.Forms;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner
{
	public class TypeSelectorWebPage : UITypeEditor
	{
		public TypeSelectorWebPage()
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
					IWithProject ip = context.Instance as IWithProject;
					if (ip != null)
					{
						LimnorProject prj = ip.Project;
						if (prj != null)
						{
							IList<ComponentID> lst = prj.GetAllComponents();
							UInt32 id = 0;
							bool isString = false;
							if (context.PropertyDescriptor != null)
							{
								isString = (typeof(string).Equals(context.PropertyDescriptor.PropertyType));
							}
							ComponentID cid = value as ComponentID;
							if (cid != null)
							{
								id = cid.ComponentId;
							}
							string pageAddr = null;
							if (isString)
							{
								pageAddr = value as string;
							}
							ComponentListControl list = new ComponentListControl(edSvc, lst, id, prj, isString, pageAddr);
							edSvc.DropDownControl(list);
							if (isString)
							{
								if (list.SelectedPageAddress != null)
								{
									value = list.SelectedPageAddress;
								}
							}
							else
							{
								if (list.SelectedObj != null)
								{
									value = list.SelectedObj;
								}
							}
						}
					}

				}
			}
			return value;
		}
		class ComponentListControl : ListBox
		{
			private IWindowsFormsEditorService _srv;
			public ComponentID SelectedObj;
			public string SelectedPageAddress;
			public ComponentListControl(IWindowsFormsEditorService edSvc, IList<ComponentID> components, UInt32 selectedId, LimnorProject prj, bool isPageAddress, string pageAddr)
			{
				_srv = edSvc;
				ComponentID non = new ComponentID(prj, 0, "none", typeof(void), "");
				Items.Add(non);
				if (components != null)
				{
					for (int i = 0; i < components.Count; i++)
					{
						if (typeof(WebPage).IsAssignableFrom(components[i].ComponentType))
						{
							ILimnorDesignPane pn = prj.GetTypedData<ILimnorDesignPane>(components[i].ComponentId);
							if (pn != null)
							{
								IComponent ic = pn.RootClass.ObjectInstance as IComponent;
								if (ic != null && ic.Site != null && ic.Site.DesignMode)
								{
									if (!string.IsNullOrEmpty(ic.Site.Name))
									{
										components[i].Rename(ic.Site.Name);
									}
								}
							}
							if (isPageAddress)
							{
								string pr = string.Format(CultureInfo.InvariantCulture, "{0}.html", components[i].ComponentName);
								int n = Items.Add(pr);
								if (string.Compare(pr, pageAddr, StringComparison.OrdinalIgnoreCase) == 0)
								{
									SelectedIndex = n;
								}
							}
							else
							{
								int n = Items.Add(components[i]);
								if (components[i].ComponentId == selectedId)
								{
									SelectedIndex = n;
								}
							}
						}
					}
				}
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				if (SelectedIndex >= 0)
				{
					SelectedObj = Items[SelectedIndex] as ComponentID;
					SelectedPageAddress = Items[SelectedIndex] as string;
					if (SelectedPageAddress == null)
					{
						SelectedPageAddress = string.Empty;
					}
				}
				_srv.CloseDropDown();
			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					if (SelectedIndex >= 0)
					{
						SelectedObj = Items[SelectedIndex] as ComponentID;
						SelectedPageAddress = Items[SelectedIndex] as string;
						if (SelectedPageAddress == null)
						{
							SelectedPageAddress = string.Empty;
						}
					}
					_srv.CloseDropDown();
				}
			}
		}
	}
}
