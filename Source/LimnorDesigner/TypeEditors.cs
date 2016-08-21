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
using System.Reflection;
using VSPrj;
using System.Windows.Forms;
using System.Xml;
using XmlSerializer;
using System.Drawing;
using MathExp;
using XmlUtility;
using LimnorKiosk;
using ProgElements;
using System.ComponentModel;
using LimnorDesigner.MethodBuilder;
using System.Runtime.InteropServices;
using LimnorDesigner.Action;
using VPL;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using TraceLog;

namespace LimnorDesigner
{
	/// <summary>
	/// Modal editor
	/// </summary>
	public abstract class PropEditorModal : UITypeEditor
	{
		public PropEditorModal()
		{
		}
		protected abstract object OnEditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService service, object value);
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					return OnEditValue(context, provider, service, value);
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
	public abstract class PropEditorDropDown : UITypeEditor
	{
		public PropEditorDropDown()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return UITypeEditorEditStyle.None;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService wfes = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				if (wfes != null)
				{
					value = OnGetDropDownControl(context, provider, wfes, value);
				}
			}
			return value;
		}
		public abstract object OnGetDropDownControl(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService service, object value);
	}
	/// <summary>
	/// property select dialogue box
	/// ParameterValue and MathNodePointer use it.
	/// Can we assume the scope is the current designer? Maybe not?
	/// Creating action from a designer: yes
	/// Assigning action to an event: yes => creating action while assignning action, in scope: 1) the current designer; 2) the static members of other components
	/// Conclusion: at anytime, the scope is defined by 1) the current designer; 2) the static members of other components
	/// </summary>
	public class PropEditorPropertyPointer : UITypeEditor
	{
		Font font;
		public PropEditorPropertyPointer()
		{
		}
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			if (font == null)
			{
				font = new Font(System.Drawing.FontFamily.GenericSansSerif, 8);
			}
			if (e.Value == null)
			{
				e.Graphics.DrawString("{null}", font, System.Drawing.Brushes.Black, 0, 0);
			}
			else
			{
				e.Graphics.DrawString(e.Value.ToString(), font, System.Drawing.Brushes.Black, 0, 0);
			}
		}
		protected virtual void OnDialogCreated(FrmObjectExplorer dlg)
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					IObjectPointer mm = value as IObjectPointer;
					if (mm == null)
					{
						PropertyPointer pp = new PropertyPointer();
						IDataScope dv = context.Instance as IDataScope;
						if (dv != null)
						{
							pp.Scope = dv.ScopeDataType;
							pp.Owner = dv.ScopeOwner;
						}
						mm = pp;
					}
					if (mm != null)
					{
						IMethod imScope = null;
						Type t = service.GetType();
						PropertyInfo pif0 = t.GetProperty("OwnerGrid");
						if (pif0 != null)
						{
							object pv = pif0.GetValue(service, null);
							MathPropertyGrid pg = pv as MathPropertyGrid;
							if (pg != null)
							{
								imScope = pg.ScopeMethod;
							}
						}
						if (imScope == null)
						{
							IScopeMethodHolder mh = context.Instance as IScopeMethodHolder;
							if (mh != null)
							{
								imScope = mh.GetScopeMethod();
							}
						}
						DataTypePointer scope = null;
						IOwnerScope ios = context.Instance as IOwnerScope;
						if (ios != null)
						{
							scope = ios.OwnerScope;
						}
						if (scope == null)
						{
							ParameterValue pv = context.Instance as ParameterValue;
							if (pv != null && pv.DataType != null && pv.DataType.BaseClassType != null)
							{
								if (typeof(Delegate).IsAssignableFrom(pv.DataType.BaseClassType))
								{
									scope = pv.DataType;
								}
							}
						}
						FrmObjectExplorer dlg = DesignUtil.GetPropertySelector(mm, imScope, scope);
						if (dlg != null)
						{
							OnDialogCreated(dlg);
							if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
							{
								bool isValid = true;
								string msg = string.Empty;
								if (MethodEditContext.IsWebPage)
								{
									IObjectPointer iop = dlg.SelectedObject as IObjectPointer;
									if (iop != null)
									{
										if (MethodEditContext.UseClientPropertyOnly)
										{
											if (iop.RunAt == EnumWebRunAt.Server)
											{
												isValid = false;
												msg = "Server value is not allowed";
											}
										}
										else if (MethodEditContext.UseServerPropertyOnly)
										{
											if (iop.RunAt == EnumWebRunAt.Client)
											{
												isValid = false;
												msg = "Client value is not allowed";
											}
										}
									}
								}
								if (!isValid)
								{
									MessageBox.Show(msg, "Select value", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
								else
								{
									value = dlg.SelectedObject;
									if (context.PropertyDescriptor.IsReadOnly)
									{
										//manually set it
										PropertyInfo pif = context.Instance.GetType().GetProperty(context.PropertyDescriptor.Name);
										pif.SetValue(context.Instance, value, new object[] { });
									}
								}
							}
							dlg.ResetSelectLValue();
						}
					}
				}
			}
			return value;
		}
	}
	public class LValueSelector : PropEditorPropertyPointer
	{
		public LValueSelector()
		{
		}
		protected override void OnDialogCreated(FrmObjectExplorer dlg)
		{
			dlg.SetSelectLValue();
		}
	}
	public class TypeScopeAttribute : Attribute
	{
		private Type _scope;
		public TypeScopeAttribute(Type scope)
		{
			_scope = scope;
		}
		public Type Scope
		{
			get
			{
				return _scope;
			}
		}
	}
	/// <summary>
	/// data type selection
	/// </summary>
	public class PropEditorDataType : UITypeEditor
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		public PropEditorDataType()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				MethodDiagramViewer mdv = context.Instance as MethodDiagramViewer;
				if (mdv != null)
				{
					WebClientEventHandlerMethod wcehm = mdv.Method as WebClientEventHandlerMethod;
					if (wcehm != null)
					{
						if (string.CompareOrdinal("ReturnType", context.PropertyDescriptor.Name) == 0)
						{
							return UITypeEditorEditStyle.None;
						}
					}
				}
				DataTypePointer dp = context.Instance as DataTypePointer;
				if (dp != null)
				{
					if (dp.ReadOnly)
					{
						return UITypeEditorEditStyle.None;
					}
					ParameterClass pc = dp as ParameterClass;
					if (pc != null)
					{
						ConstructorClass cc = pc.Method as ConstructorClass;
						if (cc != null)
						{
							ClassPointer cp = cc.Owner as ClassPointer;
							if (cp != null)
							{
								if (typeof(Attribute).IsAssignableFrom(cp.BaseClassType))
								{
									return UITypeEditorEditStyle.DropDown;
								}
							}
						}
					}
				}
			}
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					DataTypePointer typeScope = null;
					ITypeScopeHolder th = context.Instance as ITypeScopeHolder;
					if (th != null)
					{
						typeScope = th.GetTypeScope(context.PropertyDescriptor.Name);
					}
					if (typeScope == null)
					{
						foreach (Attribute a in context.PropertyDescriptor.Attributes)
						{
							TypeScopeAttribute ts = a as TypeScopeAttribute;
							if (ts != null && ts.Scope != null)
							{
								if (ts.Scope.IsGenericParameter)
								{
									Type[] ctps = ts.Scope.GetGenericParameterConstraints();
									if (ctps != null && ctps.Length > 0)
									{
										typeScope = new DataTypePointer(new TypePointer(ctps[0]));
										break;
									}
								}
								else
								{
									typeScope = new DataTypePointer(new TypePointer(ts.Scope));
									break;
								}
							}
						}
					}
					MethodClass scopeMethod = null;
					IScopeMethodHolder mh = context.Instance as IScopeMethodHolder;
					if (mh != null)
					{
						scopeMethod = mh.GetScopeMethod();
					}
					IWithProject mc = context.Instance as IWithProject;
					if (mc == null)
					{
						ComponentInterfaceWrapper ciw = context.Instance as ComponentInterfaceWrapper;
						if (ciw != null)
						{
							mc = ciw.Value as IWithProject;
						}
					}
					if (mc == null)
					{
						MathNode.Log(TraceLogClass.GetForm(provider),new DesignerException("{0} does not implement IWithProject", context.Instance.GetType()));
					}
					else
					{
						if (mc.Project == null)
						{
							MathNode.Log(TraceLogClass.GetForm(provider), new DesignerException("Project not set for {0} [{1}]", mc, mc.GetType()));
						}
						else
						{
							MethodDiagramViewer mdv = mc as MethodDiagramViewer;
							if (mdv != null)
							{
								WebClientEventHandlerMethod wcehm = mdv.Method as WebClientEventHandlerMethod;
								if (wcehm != null)
								{
									if (string.CompareOrdinal("ReturnType", context.PropertyDescriptor.Name) == 0)
									{
										return value;
									}
								}
							}
							bool isMethodReturn = false;
							IObjectPointer op = value as IObjectPointer;
							DataTypePointer val = new DataTypePointer();
							if (op != null)
							{
								val.SetDataType(op);
							}
							for (int i = 0; i < context.PropertyDescriptor.Attributes.Count; i++)
							{
								MethodReturnAttribute mra = context.PropertyDescriptor.Attributes[i] as MethodReturnAttribute;
								if (mra != null)
								{
									isMethodReturn = true;
									break;
								}
							}
							//
							bool bUseDropDown = false;
							EnumWebRunAt runAt = EnumWebRunAt.Inherit;
							PropertyClassWebClient pcwc = context.Instance as PropertyClassWebClient;
							if (pcwc != null)
							{
								bUseDropDown = true;
								runAt = EnumWebRunAt.Client;
							}
							else
							{
								PropertyClassWebServer pcws = context.Instance as PropertyClassWebServer;
								if (pcws != null)
								{
									bUseDropDown = true;
									runAt = EnumWebRunAt.Server;
								}
							}
							if (!bUseDropDown)
							{
								ParameterClass pc = context.Instance as ParameterClass;
								if (pc != null)
								{
									ConstructorClass cc = pc.Method as ConstructorClass;
									if (cc != null)
									{
										ClassPointer cp = cc.Owner as ClassPointer;
										if (cp != null)
										{
											if (typeof(Attribute).IsAssignableFrom(cp.BaseClassType))
											{
												//use drop down
												bUseDropDown = true;
											}
										}
									}
								}
							}
							//
							if (bUseDropDown)
							{
								TypeSelector drp = TypeSelector.GetAttributeParameterDialogue(service, runAt, val.BaseClassType);
								service.DropDownControl(drp);
								if (drp.SelectedType != null)
								{
									val.SetDataType(drp.SelectedType);
									bool bIsForLocalType = false;
									if (string.Compare(context.PropertyDescriptor.Name, ActionAssignInstance.Instance_Type, StringComparison.Ordinal) == 0)
									{
										PropertiesWrapper pw = context.Instance as PropertiesWrapper;
										if (pw != null)
										{
											AB_SingleAction sa = pw.Owner as AB_SingleAction;
											if (sa != null)
											{
												ActionAssignInstance aa = sa.ActionData as ActionAssignInstance;
												if (aa != null)
												{
													bIsForLocalType = true;
													aa.SetParameterValue(ConstObjectPointer.VALUE_Type, drp.SelectedType);
												}
											}
										}
									}
									if (bIsForLocalType)
									{
									}
									else
									{
										value = val;
									}
								}
							}
							else
							{
								Type typeAttr = null;
								MethodClass mc0 = scopeMethod;
								if (mc0 == null)
								{
									mc0 = mc as MethodClass;
								}
								if (mc0 == null)
								{
									MethodDiagramViewer mcv = mc as MethodDiagramViewer;
									if (mcv != null)
									{
										mc0 = mcv.Method;
									}
								}
								if (mc0 != null)
								{
									if (mc.Project.ProjectType == EnumProjectType.WebAppPhp)
									{
										if (mc0.WebUsage == EnumMethodWebUsage.Server)
										{
											typeAttr = typeof(PhpTypeAttribute);
										}
										else
										{
											typeAttr = typeof(JsTypeAttribute);
										}
									}
									else if (mc.Project.ProjectType == EnumProjectType.WebAppAspx)
									{
										if (mc0.WebUsage == EnumMethodWebUsage.Client)
										{
											typeAttr = typeof(JsTypeAttribute);
										}
									}
								}
								FrmObjectExplorer dlg = DesignUtil.GetDataTypeSelectionDialogue(mc.Project, scopeMethod, val, isMethodReturn, typeScope, typeAttr);
								if (service.ShowDialog(dlg) == DialogResult.OK)
								{
									val.SetDataType(dlg.SelectedDataType);
									bool bIsForLocalType = false;
									if (string.Compare(context.PropertyDescriptor.Name, ActionAssignInstance.Instance_Type, StringComparison.Ordinal) == 0)
									{
										PropertiesWrapper pw = context.Instance as PropertiesWrapper;
										if (pw != null)
										{
											AB_SingleAction sa = pw.Owner as AB_SingleAction;
											if (sa != null)
											{
												ActionAssignInstance aa = sa.ActionData as ActionAssignInstance;
												if (aa != null)
												{
													bIsForLocalType = true;
													aa.SetParameterValue(ActionAssignInstance.Instance_Type, dlg.SelectedDataType);
												}
											}
										}
									}
									else
									{
										ComponentIconLocal cil = context.Instance as ComponentIconLocal;
										bIsForLocalType = (cil != null);
										if (bIsForLocalType)
										{
											cil.LocalPointer.ClassType = dlg.SelectedDataType;
										}
									}
									if (bIsForLocalType)
									{
									}
									else
									{
										ExceptionHandlerList.PropertyDescriptorExceptionHandler pdeh = context.PropertyDescriptor as ExceptionHandlerList.PropertyDescriptorExceptionHandler;
										if (pdeh != null)
										{
											pdeh.Handler.ExceptionType = val;
										}
										else
										{
											value = val;
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
