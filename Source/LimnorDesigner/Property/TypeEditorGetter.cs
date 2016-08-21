/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using MathExp;
using LimnorDesigner.MethodBuilder;
using VSPrj;
using System.Drawing;
using System.Windows.Forms;
using TraceLog;

namespace LimnorDesigner.Property
{
	class TypeEditorGetter : PropEditorModal
	{
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public TypeEditorGetter()
		{
		}

		protected override object OnEditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, System.Windows.Forms.Design.IWindowsFormsEditorService service, object value)
		{
			IWithProject mc = context.Instance as IWithProject;
			if (mc != null)
			{
				if (mc.Project == null)
				{
					MathNode.Log(TraceLogClass.GetForm(provider), new DesignerException("Project not set for {0} [{1}]", mc, mc.GetType()));
				}
				else
				{
					GetterClass val = value as GetterClass;
					System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
					rc.X = curPoint.X;
					rc.Y = curPoint.Y;
					DlgMethod dlg = val.CreateMethodEditor(rc);
					try
					{
						dlg.SetNameReadOnly();
						dlg.LoadMethod(val, EnumParameterEditType.ReadOnly);
						dlg.Text = val.ToString();
						if (service.ShowDialog(dlg) == DialogResult.OK)
						{
							value = val;
							ILimnorDesignerLoader l = LimnorProject.ActiveDesignerLoader as ILimnorDesignerLoader;
							if (l != null)
							{
								//save the property and hence save the getter
								DesignUtil.SaveCustomProperty(LimnorProject.ActiveDesignerLoader.Node, l.Writer, val.Property);
								//save actions
								List<IAction> actions = val.GetActions();
								if (actions != null)
								{
									foreach (IAction a in actions)
									{
										l.GetRootId().SaveAction(a, l.Writer);
									}
								}
								LimnorProject.ActiveDesignerLoader.NotifyChanges();
							}
						}
					}
					catch (Exception err)
					{
						MathNode.Log(TraceLogClass.GetForm(provider),err);
					}
					finally
					{
						val.ExitEditor();
					}
				}
			}
			return value;
		}
	}
}
