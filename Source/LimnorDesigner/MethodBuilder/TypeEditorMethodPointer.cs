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
using System.Windows.Forms.Design;
using LimnorDesigner;
using System.ComponentModel;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;
using ProgElements;
using System.Reflection;
using MathExp;
using LimnorDesigner.Property;

namespace MathItem
{
	/// <summary>
	/// choose method for an action
	/// </summary>
	public class TypeEditorMethodPointer : UITypeEditor
	{
		public TypeEditorMethodPointer()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return UITypeEditorEditStyle.None;// base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					IMethod m = value as IMethod;
					IActionMethodPointer im = null;
					MethodPointer method = value as MethodPointer;
					if (m == null)
					{
						IMethodPointerHolder mh = context.Instance as IMethodPointerHolder;
						if (mh != null)
						{
							im = mh.GetMethodPointer();
							if (im != null)
							{
								m = im.MethodPointed;
							}
						}
					}
					MethodClass scopeMethod = null;
					Type t = edSvc.GetType();
					PropertyInfo pif0 = t.GetProperty("OwnerGrid");
					if (pif0 != null)
					{
						object g = pif0.GetValue(edSvc, null);
						MathPropertyGrid pg = g as MathPropertyGrid;
						if (pg != null)
						{
							scopeMethod = pg.ScopeMethod as MethodClass;
						}
					}
					if (scopeMethod == null)
					{
						IAction ia = context.Instance as IAction;
						if (ia != null)
						{
							scopeMethod = ia.ScopeMethod as MethodClass;
						}
					}
					FrmObjectExplorer dlg = DesignUtil.CreateSelectMethodDialog(scopeMethod, m);
					if (edSvc.ShowDialog(dlg) == DialogResult.OK)
					{
						IAction act = null;
						if (method != null)
						{
							act = method.Action;
						}
						if (act == null)
						{
							if (im != null)
							{
								act = im.Action;
							}
						}
						IPropertyEx p = dlg.SelectedObject as IPropertyEx;
						if (p != null)
						{
							value = p.CreateSetterMethodPointer(act);
						}
						else
						{
							MethodPointer mp = dlg.SelectedObject as MethodPointer;
							if (mp != null)
							{
								mp.Action = act;
								value = mp;
							}
							else
							{
								CustomMethodPointer cmp = dlg.SelectedObject as CustomMethodPointer;
								if (cmp != null)
								{
									cmp.Action = act;
									value = cmp;
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
