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
using LimnorDesigner.Action;
using System.Windows.Forms;
using System.Drawing;
using VSPrj;
using System.Globalization;
using MathExp;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// launch Method Editor to create exception handler
	/// </summary>
	class TypeEditorExceptionHandler : UITypeEditor
	{
		public TypeEditorExceptionHandler()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return UITypeEditorEditStyle.None;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					ExceptionHandlerList.PropertyDescriptorExceptionHandler pd = context.PropertyDescriptor as ExceptionHandlerList.PropertyDescriptorExceptionHandler;
					if (pd == null)
					{
						throw new DesignerException("Property descriptor [{0}] is not a PropertyDescriptorExceptionHandler", context.PropertyDescriptor == null ? "null" : context.PropertyDescriptor.GetType().FullName);
					}
					ExceptionHandler eh = pd.Handler;
					BranchList branches = eh.ActionList;
					ExceptionHandlerList ehs = context.Instance as ExceptionHandlerList;
					if (ehs != null)
					{
						MethodClass scopeMethod = ehs.OwnerMethod;
						if (scopeMethod != null)
						{
							if (branches == null)
							{
								branches = new BranchList(eh, new List<ActionBranch>());
							}

							ILimnorDesignPane dp = scopeMethod.Project.GetTypedData<ILimnorDesignPane>(scopeMethod.RootPointer.ClassId);
							if (dp != null)
							{
								//save the method contents
								BranchList bl = scopeMethod.ActionList;
								string methodName = scopeMethod.MethodName;
								ParameterClass returnType = scopeMethod.ReturnValue;
								List<ComponentIcon> icons = scopeMethod.ComponentIconList;

								//switch method contents
								scopeMethod.ActionList = branches;
								scopeMethod.SetName(string.Format(CultureInfo.InvariantCulture, "HandlerFor{0}", eh.ExceptionType.Name));
								scopeMethod.ReturnValue = null;
								List<ComponentIcon> icons2 = new List<ComponentIcon>();
								foreach (ComponentIcon ic in icons)
								{
									icons2.Add((ComponentIcon)ic.Clone());
								}
								foreach (ComponentIconSubscopeVariable ic in eh.ComponentIconList)
								{
									icons2.Add((ComponentIcon)ic.Clone());
								}
								icons2.Add(eh.ExceptionObject);
								scopeMethod.SetComponentIcons(icons2);
								//
								Rectangle rc = new Rectangle(Cursor.Position, new Size(32, 232));
								if (scopeMethod.Owner == null)
								{
									scopeMethod.Owner = dp.Loader.GetRootId();
								}
								DlgMethod dlg = scopeMethod.CreateBlockScopeMethodEditor(rc, eh.ExceptionType.MemberId);
								try
								{
									dlg.LoadMethod(scopeMethod, EnumParameterEditType.ReadOnly);
									dlg.Text = string.Format(CultureInfo.InvariantCulture, "Create exception handler for {0}", eh.ExceptionType.Name);
									if (edSvc.ShowDialog(dlg) == DialogResult.OK)
									{
										bool delete = false;
										eh.ActionList = scopeMethod.ActionList;
										if (eh.ActionList.Count == 0)
										{
											if (MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "Do you want to remove this exception [{0}]?", eh.ExceptionType.Name), "Capture exception", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
											{
												delete = true;
											}
										}
										if (delete)
										{
											scopeMethod.RemoveExceptionCapture(eh);
										}
										else
										{
											List<ComponentIconSubscopeVariable> lst = new List<ComponentIconSubscopeVariable>();
											foreach (ComponentIcon ic in scopeMethod.ComponentIconList)
											{
												ComponentIconSubscopeVariable sv = ic as ComponentIconSubscopeVariable;
												if (sv != null)
												{
													lst.Add(sv);
												}
											}
											eh.ComponentIconList = lst;
											scopeMethod.RootPointer.SaveMethod(scopeMethod, eh);
										}
									}
								}
								catch (Exception err)
								{
									MathNode.LogError(DesignerException.FormExceptionText(err));
								}
								finally
								{
									//restore method contents
									scopeMethod.SetName(methodName);
									scopeMethod.ActionList = bl;
									scopeMethod.ReturnValue = returnType;
									scopeMethod.SetComponentIcons(icons);
									scopeMethod.ExitEditor();
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
