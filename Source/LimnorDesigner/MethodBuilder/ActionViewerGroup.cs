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
using MathExp;
using System.Drawing;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerGroup : ActionViewer
	{
		#region fields and constructors
		public ActionViewerGroup()
		{
			AddPropertyName("ActionName");
		}
		#endregion
		#region private methods
		private void miEditActions_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				AB_Group group = this.ActionObject as AB_Group;
				MethodDesignerHolder holder = mv.DesignerHolder;
				holder.OpenActionGroup(group);
			}
		}
		#endregion
		#region protected overrides
		protected override bool CanEditAction { get { return false; } }
		protected override void OnCreateContextMenu(ContextMenu cm)
		{
			MenuItem mi;
			mi = new MenuItemWithBitmap("Open", miEditActions_Click, Resources._method.ToBitmap());
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
			//draw icon
			Image img = Resources._method.ToBitmap();
			float x = d + 2;
			float y = (float)(d2 + (float)(this.ClientSize.Height - d2 - img.Height) / 2.0);
			if (y < d2)
				y = d2;
			e.Graphics.DrawImage(img, x, y);
			//draw action description
			x += img.Width + 2;
			if (x < this.ClientSize.Width - d)
			{
				string s = ActionName;
				if (string.IsNullOrEmpty(s))
				{
					s = "Action group";
				}
				SizeF sf = e.Graphics.MeasureString(s, TextFont);
				y = (float)(d2 + (float)(this.ClientSize.Height - d2 - sf.Height) / 2.0);
				if (y < d2)
					y = d2;
				e.Graphics.DrawString(s, TextFont, TextBrush, x, y);
			}
		}
		#endregion
	}
}
