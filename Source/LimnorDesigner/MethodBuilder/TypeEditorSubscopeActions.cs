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
using System.ComponentModel;
using LimnorDesigner.Action;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using MathExp;
using XmlSerializer;

namespace LimnorDesigner.MethodBuilder
{
	class TypeEditorSubscopeActions : UITypeEditor
	{
		public TypeEditorSubscopeActions()
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
					MethodClass scopeMethod = context.Instance as MethodClass;
					if (scopeMethod != null)
					{
						//this version only edit FinalActions
						ILimnorDesignPane dp = scopeMethod.Project.GetTypedData<ILimnorDesignPane>(scopeMethod.RootPointer.ClassId);
						if (dp != null)
						{
							//save the method contents
							BranchList bl = scopeMethod.ActionList;
							string methodName = scopeMethod.MethodName;
							ParameterClass returnType = scopeMethod.ReturnValue;
							List<ComponentIcon> icons = scopeMethod.ComponentIconList;

							//switch method contents
							scopeMethod.ActionList = scopeMethod.FinalActions.Actions;
							scopeMethod.SetName("FinalActions");
							List<ComponentIcon> icons2 = new List<ComponentIcon>();
							foreach (ComponentIcon ic in icons)
							{
								icons2.Add((ComponentIcon)ic.Clone());
							}
							foreach (ComponentIconSubscopeVariable ic in scopeMethod.FinalActions.ComponentIconList)
							{
								icons2.Add((ComponentIcon)ic.Clone());
							}
							scopeMethod.SetComponentIcons(icons2);
							//
							Rectangle rc = new Rectangle(Cursor.Position, new Size(32, 232));
							if (scopeMethod.Owner == null)
							{
								scopeMethod.Owner = dp.Loader.GetRootId();
							}
							DlgMethod dlg = scopeMethod.CreateBlockScopeMethodEditor(rc, 1);
							try
							{
								dlg.LoadMethod(scopeMethod, EnumParameterEditType.ReadOnly);
								dlg.Text = string.Format(CultureInfo.InvariantCulture, "Specify final actionsfor {0}", methodName);
								if (edSvc.ShowDialog(dlg) == DialogResult.OK)
								{
									scopeMethod.FinalActions.Actions = scopeMethod.ActionList;
									List<ComponentIconSubscopeVariable> lst = new List<ComponentIconSubscopeVariable>();
									foreach (ComponentIcon ic in scopeMethod.ComponentIconList)
									{
										ComponentIconSubscopeVariable sv = ic as ComponentIconSubscopeVariable;
										if (sv != null)
										{
											lst.Add(sv);
										}
									}
									scopeMethod.FinalActions.ComponentIconList = lst;
									XmlObjectWriter xw = dp.Loader.Writer;
									xw.WriteObject(scopeMethod.FinalActions.DataXmlNode, scopeMethod.FinalActions, null);
									value = scopeMethod.FinalActions;
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
								scopeMethod.SetComponentIcons(icons);
								scopeMethod.ExitEditor();
							}
						}
					}
				}
			}
			return value;
		}
	}
}
