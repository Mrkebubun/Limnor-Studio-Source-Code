/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using MathExp.RaisTypes;
using System.Windows.Forms.Design;

namespace MathExp
{
	public class UITypeEditorArgumentSelector : UITypeEditor
	{
		public UITypeEditorArgumentSelector()
		{
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				edSvc.CloseDropDown();
				ArguementListControl list = new ArguementListControl(edSvc);
				edSvc.DropDownControl(list);
				if (list.SelectedObj != null)
				{
					value = list.SelectedObj.Name;
					MathNodeArgument a = context.Instance as MathNodeArgument;
					if (a != null)
					{
						a.SetDataType(list.SelectedObj);
					}
				}
			}
			return value;
		}
	}
	public class ArguementListControl : ListBox
	{
		IWindowsFormsEditorService _srv;
		public Parameter SelectedObj;
		public ArguementListControl(IWindowsFormsEditorService edSvc)
		{
			_srv = edSvc;
			MethodType mt = (MethodType)MathNode.GetService(typeof(MethodType));
			if (mt != null)
			{
				for (int i = 0; i < mt.ParameterCount; i++)
				{
					Items.Add(mt.Parameters[i]);
				}
			}
		}
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (SelectedIndex >= 0)
			{
				SelectedObj = (Parameter)Items[SelectedIndex];
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
					SelectedObj = (Parameter)Items[SelectedIndex];
				}
				_srv.CloseDropDown();
			}
		}
	}
}
