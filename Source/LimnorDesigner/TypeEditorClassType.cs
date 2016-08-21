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
using System.Windows.Forms;
using VSPrj;
using WindowsUtility;

namespace LimnorDesigner
{
	class TypeEditorClassType : UITypeEditor
	{
		public TypeEditorClassType()
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
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					LimnorWinApp mc = context.Instance as LimnorWinApp;
					if (mc == null)
					{
						ClassPointer cp = context.Instance as ClassPointer;
						if (cp != null)
						{
							mc = cp.ObjectInstance as LimnorWinApp;
						}
					}
					if (mc != null)
					{
						if (mc.Project != null)
						{
							ClassTypeList ctl = new ClassTypeList(service, mc.Project, mc.StartForm);
							service.DropDownControl(ctl);
							if (ctl.SelectedObj != null)
							{
								PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(mc);
								foreach (PropertyDescriptor p in ps)
								{
									if (string.CompareOrdinal(p.Name, "StartClassId") == 0)
									{
										p.SetValue(mc, ctl.SelectedObj.ComponentId);
										break;
									}
								}
							}
						}
					}
				}
			}
			return value;
		}
		class ClassTypeList : ListBox
		{
			IWindowsFormsEditorService _srv;
			public ComponentID SelectedObj;
			public ClassTypeList(IWindowsFormsEditorService edSvc, LimnorProject project, ClassTypePointer initSelected)
			{
				_srv = edSvc;
				IList<ComponentID> cis = project.GetAllComponents();
				for (int i = 0; i < cis.Count; i++)
				{
					if (typeof(Form).IsAssignableFrom(cis[i].ComponentType))
					{
						object[] attrs = cis[i].ComponentType.GetCustomAttributes(typeof(SupportFormAttribute), false);
						if (attrs != null && attrs.Length > 0)
						{
							continue;
						}
						int k = this.Items.Add(cis[i]);
						if (this.SelectedIndex < 0)
						{
							if (initSelected != null)
							{
								if (cis[i].ComponentId == initSelected.ClassId)
								{
									this.SelectedIndex = k;
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
					SelectedObj = (ComponentID)Items[SelectedIndex];
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
						SelectedObj = (ComponentID)Items[SelectedIndex];
					}
					_srv.CloseDropDown();
				}
			}
		}
	}
}
