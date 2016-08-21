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
using MathExp;
using System.Drawing;
using System.Windows.Forms;
using LimnorDesigner.Action;

namespace LimnorDesigner
{
	class TypeEditorExpressionValue : UITypeEditor
	{
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public TypeEditorExpressionValue()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					PropertiesWrapper pw = context.Instance as PropertiesWrapper;
					AB_SingleAction sa = null;
					if (pw != null)
					{
						sa = pw.Owner as AB_SingleAction;
					}
					ActionClass a = context.Instance as ActionClass;
					if (a == null)
					{
						if (sa != null)
						{
							a = sa.ActionData as ActionClass;
						}
					}
					ExpressionValue ev = (ExpressionValue)value;
					if (ev == null)
					{
						ev = new ExpressionValue();
					}
					MathNodeRoot root = ev.GetExpression();
					if (root == null)
					{
						root = new MathNodeRoot();
						ev.SetExpression(root);
					}
					if (root.ScopeMethod == null)
					{
						if (a != null)
						{
							if (a.ScopeMethod == null && sa != null)
							{
								a.ScopeMethod = sa.Method;
							}
							root.ScopeMethod = a.ScopeMethod;
						}
						else if (sa != null)
						{
							if (sa.ActionData != null && sa.ActionData.ScopeMethod != null)
							{
								root.ScopeMethod = sa.ActionData.ScopeMethod;
							}
						}
					}
					if (root.ActionContext == null)
					{
						if (a != null)
						{
							root.ActionContext = a;
						}
						else if (sa != null)
						{
							root.ActionContext = sa;
						}
					}
					System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
					rc.X = curPoint.X;
					rc.Y = curPoint.Y;
					IMathEditor dlg = root.CreateEditor(rc);
					if (a != null)
					{
						dlg.ActionContext = a;
					}
					dlg.SetScopeMethod(root.ScopeMethod);
					dlg.VariableMapTargetType = typeof(ParameterValue);
					dlg.MathExpression = (IMathExpression)root.Clone();
					dlg.MathExpression.ScopeMethod = root.ScopeMethod;
					dlg.MathExpression.EnableUndo = true;
					if (edSvc.ShowDialog((Form)dlg) == DialogResult.OK)
					{
						root = (MathNodeRoot)dlg.MathExpression;
						ev = new ExpressionValue();
						ev.SetExpression(root);
						value = ev;
					}
				}
			}
			return value;
		}
	}
}
