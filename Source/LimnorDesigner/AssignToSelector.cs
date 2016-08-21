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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using LimnorDesigner.MethodBuilder;
using System.Reflection;

namespace LimnorDesigner
{
	class AssignToSelector : UITypeEditor
	{
		public AssignToSelector()
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
				ActionClass act = context.Instance as ActionClass;
				if (act == null)
				{
					LimnorDesigner.PropertiesWrapper pw = context.Instance as LimnorDesigner.PropertiesWrapper;
					if (pw != null)
					{
						LimnorDesigner.Action.AB_SingleAction abs = pw.Owner as LimnorDesigner.Action.AB_SingleAction;
						if (abs != null)
						{
							act = abs.ActionData as ActionClass;
						}
					}
				}
				if (act != null && act.ScopeMethod != null)
				{
					MethodClass mc = act.ScopeMethod as MethodClass;
					if (mc != null && (mc.CurrentEditor != null || mc.CurrentSubEditor != null))
					{
						IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
						if (edSvc != null)
						{
							listAssignedTo list = new listAssignedTo(edSvc);
							edSvc.DropDownControl(list);
							if (list.SelectedAssignedTo == 0)
							{
								ComponentIconLocal cil = null;
								MethodClass actMethod = act.ActionMethod as MethodClass;
								if (actMethod == null)
								{
									CustomMethodPointer cmp = act.ActionMethod as CustomMethodPointer;
									if (cmp != null)
									{
										actMethod = cmp.MethodDef;
									}
								}
								if (act.ReturnValueType.IsLibType)
								{
									Type t = act.ReturnValueType.BaseClassType;
									if (t.IsArray)
									{
										ArrayPointer ap = new ArrayPointer(new TypePointer(t.GetElementType()));
										if (mc.CurrentSubEditor != null)
											cil = mc.CurrentSubEditor.CreateLocalVariable(ap, new System.Drawing.Point(0, 0), actMethod, false);
										else
											cil = mc.CurrentEditor.CreateLocalVariable(ap, new System.Drawing.Point(0, 0), actMethod, false);
									}
								}
								if (cil == null)
								{
									if (mc.CurrentSubEditor != null)
										cil = mc.CurrentSubEditor.CreateLocalVariable(act.ReturnValueType, new System.Drawing.Point(0, 0), actMethod, false);
									else
										cil = mc.CurrentEditor.CreateLocalVariable(act.ReturnValueType, new System.Drawing.Point(0, 0), actMethod, false);
								}
								cil.LocalPointer.Owner = mc;
								if (cil.LocalPointer.BaseClassType.IsGenericParameter)
								{
									if (cil.LocalPointer.ClassType.ConcreteType == null)
									{
										DataTypePointer dp = act.GetConcreteType(cil.LocalPointer.BaseClassType);
										if (dp != null)
										{
											cil.LocalPointer.ClassType.SetConcreteType(dp);
										}
									}
								}
								else if (cil.LocalPointer.BaseClassType.IsGenericType)
								{
									MethodPointer mp = act.ActionMethod as MethodPointer;
									if (mp != null)
									{
										if (mp.ReturnTypeConcrete != null)
										{
											cil.LocalPointer.SetDataType(mp.ReturnTypeConcrete);
										}
										else
										{
											DataTypePointer dp = mp.GetConcreteType(cil.LocalPointer.BaseClassType);
											if (dp != null)
											{
												cil.LocalPointer.SetDataType(dp);
											}
										}
									}
								}
								value = cil.LocalPointer;
							}
							else if (list.SelectedAssignedTo == 1)
							{
								LValueSelector pe = new LValueSelector();
								value = pe.EditValue(context, provider, value);
							}
						}
					}
				}
			}
			return value;
		}
		class listAssignedTo : ListBox
		{
			public int SelectedAssignedTo = -1;
			IWindowsFormsEditorService _srvc;
			public listAssignedTo(IWindowsFormsEditorService edSvc)
			{
				_srvc = edSvc;
				Items.Add("New local variable");
				Items.Add("Select existing object");
			}
			protected override void OnClick(EventArgs e)
			{
				SelectedAssignedTo = SelectedIndex;
				_srvc.CloseDropDown();
			}
			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					SelectedAssignedTo = SelectedIndex;
					_srvc.CloseDropDown();
				}
			}
		}
	}
}
