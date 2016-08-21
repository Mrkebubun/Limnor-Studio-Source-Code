/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViwerForLoop : ActionViewer
	{
		public ActionViwerForLoop()
		{
			AddPropertyName("ActionName");
		}
		private void miEditActions_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				AB_ForLoop loop = this.ActionObject as AB_ForLoop;
				MethodDesignerHolder holder = mv.DesignerHolder;
				MethodClass mc = loop.Method;
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
					MathNode.Log(this.FindForm(), err);
				}
				finally
				{
					mc.SubMethod.Pop();
					mc.CurrentSubEditor = null;
				}
			}
		}
		protected override bool CanEditAction { get { return false; } }
		protected override void OnCreateContextMenu(ContextMenu cm)
		{
			MenuItem mi;
			mi = new MenuItemWithBitmap("Edit Repeated Actions", miEditActions_Click, Resources._condition.ToBitmap());
			cm.MenuItems.Add(mi);
			cm.MenuItems.Add("-");
		}
		protected override void OnPaintActionView(PaintEventArgs e)
		{
			base.OnPaintActionView(e);
			int d = 5;
			float d2 = 2.5F;
			if (this.ClientSize.Width > 10 && this.ClientSize.Height > 10)
			{
				GraphicsState gt = e.Graphics.Save();
				e.Graphics.TranslateTransform((float)d2, (float)d2);
				Size size = new Size(this.ClientSize.Width - d, this.ClientSize.Height - d);
				VPLDrawing.VplDrawing.DrawRoundRectangle(e.Graphics, size, 100, 4, Pens.Blue, Pens.LightGray);
				e.Graphics.Restore(gt);
			}
			float y = d2;
			//draw icon
			y = (this.ClientSize.Height - Resources.loop.Height) / 2.0F;
			if (y < 0)
				y = 0;
			e.Graphics.DrawImage(Resources.loop, d2 + 2F, y);
			//draw action description
			string s = ActionName;
			if (string.IsNullOrEmpty(s))
			{
				s = "Repeated actions";
			}
			SizeF sf = e.Graphics.MeasureString(s, TextFont);
			y = (this.ClientSize.Height - sf.Height) / 2.0F;
			if (y < 0)
				y = 0;
			e.Graphics.DrawString(s, TextFont, TextBrush, (float)(d2 + 4F + Resources.loop.Width), y);
		}
	}
}
