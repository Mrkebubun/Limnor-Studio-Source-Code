/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MathExp;
using LimnorDesigner.MethodBuilder;
using VSPrj;
using System.Windows.Forms;
using System.ComponentModel;
using TraceLog;

namespace LimnorDesigner.Property
{
	class TypeEditorSetter : PropEditorModal
	{
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public TypeEditorSetter()
		{
		}

		protected override object OnEditValue(ITypeDescriptorContext context, IServiceProvider provider, System.Windows.Forms.Design.IWindowsFormsEditorService service, object value)
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
					SetterClass val = value as SetterClass;
					System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
					rc.X = curPoint.X;
					rc.Y = curPoint.Y;
					DlgMethod dlg = val.CreateMethodEditor(rc);
					try
					{
						dlg.LoadMethod(val, EnumParameterEditType.ReadOnly);
						dlg.SetNameReadOnly();
						if (service.ShowDialog((Form)dlg) == DialogResult.OK)
						{
							value = val;
							ILimnorDesignerLoader l = LimnorProject.ActiveDesignerLoader as ILimnorDesignerLoader;
							if (l != null)
							{
								DesignUtil.SaveCustomProperty(LimnorProject.ActiveDesignerLoader.Node, l.Writer, val.Property);
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
