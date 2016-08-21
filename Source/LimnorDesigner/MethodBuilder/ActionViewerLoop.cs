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
using System.Windows.Forms;
using LimnorDesigner.Action;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerLoop : ActionViewer
	{
		#region fields and constructors
		public ActionViewerLoop()
		{
			AddPropertyName("ActionName");
		}
		#endregion
		#region private methods
		private void miEditCondition_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				dlgMathEditor dlg = new dlgMathEditor(this.Parent.RectangleToScreen(this.Bounds));
				AB_LoopActions loop = this.ActionObject as AB_LoopActions;
				loop.Condition.Project = mv.Project;
				loop.Condition.ScopeMethod = mv.Method;
				dlg.MathExpression = loop.Condition;
				dlg.SetScopeMethod(mv.Method);

				if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					loop.Condition = (MathNodeRoot)dlg.MathExpression;

					mv.Changed = true;
				}
			}
		}
		private void miEditActions_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				AB_LoopActions loop = this.ActionObject as AB_LoopActions;
				MethodDesignerHolder holder = mv.DesignerHolder;
				MethodClass mc = loop.Method as MethodClass;
				DlgMethod dlg = mc.CreateSubMethodEditor(typeof(ActionGroupDesignerHolder), this.Parent.RectangleToScreen(this.Bounds), mv, loop.BranchId);
				try
				{
					mc.SubMethod.Push(loop);
					dlg.LoadActions(loop);
					if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
					{
						UpdateAction(dlg.ActionResult);
						mv.Changed = true;
					}
				}
				catch (Exception err)
				{
					MathNode.Log(this.FindForm(),err);
				}
				finally
				{
					mc.SubMethod.Pop();
					mc.CurrentSubEditor = null;
				}
			}
		}
		#endregion
		#region protected overrides
		protected override bool CanEditAction { get { return false; } }
		protected override void OnCreateContextMenu(ContextMenu cm)
		{
			MenuItem mi;
			mi = new MenuItemWithBitmap("Edit Repeated Actions", miEditActions_Click, Resources._method.ToBitmap());
			cm.MenuItems.Add(mi);
		}
		protected override void OnPaintActionView(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaintActionView(e);
			int d = 5;
			float d2 = 2.5F;
			if (this.ClientSize.Width > 10 && this.ClientSize.Height > 10)
			{
				System.Drawing.Drawing2D.GraphicsState gt = e.Graphics.Save();
				e.Graphics.TranslateTransform((float)d2, (float)d2);
				Size size = new Size(this.ClientSize.Width - d, this.ClientSize.Height - d);
				VPLDrawing.VplDrawing.DrawRoundRectangle(e.Graphics, size, 100, 4, Pens.Blue, Pens.LightGray);
				e.Graphics.Restore(gt);
			}
			float y = d2;
			//draw condition text
			AB_LoopActions loop = ActionObject as AB_LoopActions;
			if (loop.Condition != null)
			{
				System.Drawing.Drawing2D.GraphicsState st = e.Graphics.Save();
				SizeF sizeC = loop.Condition.CalculateDrawSize(e.Graphics);
				float x = (this.Width - sizeC.Width - d) / 2;
				if (x < d2)
					x = d2;
				y = d2 + sizeC.Height;
				e.Graphics.TranslateTransform(x, d2);
				loop.Condition.Draw(e.Graphics);
				e.Graphics.Restore(st);
			}
			//draw action description
			if (y < this.Height - d)
			{
				string s = ActionName;
				if (string.IsNullOrEmpty(s))
				{
					s = "Repeated actions";
				}
				System.Drawing.Drawing2D.GraphicsState st = e.Graphics.Save();
				e.Graphics.TranslateTransform(d2, y);
				e.Graphics.DrawString(s, TextFont, TextBrush, (float)0, (float)0);
				e.Graphics.Restore(st);
			}
		}
		#endregion
	}
}
