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
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using MathExp;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using ProgElements;
using System.Reflection;
using LimnorDesigner.Property;

namespace LimnorDesigner
{
	/// <summary>
	/// bring up math editor to edit MathNodeRoot/MathExpGroup and set the result to ParameterValue
	/// </summary>
	public class UITypeEditorMathExpression2 : UITypeEditor
	{
		Font f;
		Brush br;
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public UITypeEditorMathExpression2()
		{
			f = new Font("Times New Roman", 10);
			br = Brushes.Blue;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			rc = e.Bounds;
			ParameterValue dv = e.Context.Instance as ParameterValue;
			if (dv != null)
			{
				if (dv.DataIcon == null)
				{
					IMathExpression mew = e.Value as IMathExpression;
					if (mew != null)
					{
						dv.DataIcon = mew.CreateIcon(e.Graphics);
					}
				}
				if (dv.DataIcon != null)
				{
					e.Graphics.DrawImage(dv.DataIcon, e.Bounds);
				}
				e.Graphics.DrawString(dv.ToString(), f, br, (float)16, (float)1);
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return UITypeEditorEditStyle.None;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				ParameterValue pv = context.Instance as ParameterValue;
				if (pv == null)
				{
					MapItem item = context.Instance as MapItem;
					if (item != null)
					{
						pv = item.Item.Value as ParameterValue;
					}
				}
				if (pv == null)
				{
					pv = value as ParameterValue;
				}
				if (pv != null)
				{
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
						IMathExpression mew = value as IMathExpression;
						if (mew == null)
						{
							mew = pv.MathExpression;
							if (mew == null)
							{
								MathNodeRoot r = new MathNodeRoot();
								r.Name = pv.Name;
								mew = r;
							}
						}
						System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
						rc.X = curPoint.X;
						rc.Y = curPoint.Y;
						IMathEditor dlg = mew.CreateEditor(rc);
						//
						MathPropertyGrid pg = null;
						Type t = edSvc.GetType();
						PropertyInfo pif0 = t.GetProperty("OwnerGrid");
						if (pif0 != null)
						{
							object g = pif0.GetValue(edSvc, null);
							pg = g as MathPropertyGrid;
						}
						IMethod imScope = pv.ScopeMethod;
						if (imScope == null)
						{
							imScope = mew.ScopeMethod;
							if (imScope == null)
							{
								if (pg != null)
								{
									imScope = pg.ScopeMethod;
								}
							}
						}
						//
						dlg.ActionContext = pv.ActionContext;
						dlg.SetScopeMethod(imScope);
						dlg.VariableMapTargetType = typeof(ParameterValue);
						dlg.MathExpression = (IMathExpression)mew.Clone();
						dlg.MathExpression.ScopeMethod = imScope;
						dlg.MathExpression.EnableUndo = true;
						if (edSvc.ShowDialog((Form)dlg) == DialogResult.OK)
						{
							mew = dlg.MathExpression;
							if (value != pv)
							{
								value = mew;
								pv.SetValue(value);
							}
							else
							{
								pv.SetValue(mew);
							}
							if (pg != null)
							{
								pg.OnValueChanged(mew, EventArgs.Empty);
							}
						}
					}
				}
			}
			return value;
		}
	}

}
