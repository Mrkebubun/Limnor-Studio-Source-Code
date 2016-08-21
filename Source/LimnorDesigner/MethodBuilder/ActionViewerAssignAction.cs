/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MathExp;
using System.Drawing;
using LimnorDesigner.Event;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerAssignAction : ActionViewer
	{
		public ActionViewerAssignAction()
		{
			AddPropertyName("ActionName");
		}
		#region private methods
		private void miEditActions_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				AB_AssignActions eaAct = this.ActionObject as AB_AssignActions;
				if (eaAct != null)
				{
					MethodDesignerHolder holder = mv.DesignerHolder;
					if (holder != null)
					{
						EventHandlerMethod m = eaAct.GetHandlerMethod();
						if (m == null)
						{
							MessageBox.Show(mv.FindForm(), "Action data for event handler method not found. You may delete the action and re-create the action.", "Edit action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
						else
						{
							m.Edit(eaAct.BranchId, this.Bounds, holder.Loader, this.FindForm());
						}
					}
				}
			}
		}
		#endregion
		#region protected overrides
		protected override bool CanEditAction { get { return false; } }
		protected override void OnCreateContextMenu(ContextMenu cm)
		{
			MenuItem mi;
			mi = new MenuItemWithBitmap("Edit event handler", miEditActions_Click, Resources._method.ToBitmap());
			cm.MenuItems.Add(mi);
		}
		protected override void OnPaintActionView(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaintActionView(e);
			int d = 5;
			float d2 = 2.5F;
			//draw icon
			Image img = Resources._handlerMethod.ToBitmap();
			float x = d + 2;
			float y = (float)(d2 + (float)(this.ClientSize.Height - d2 - img.Height) / 2.0);
			if (y < d2)
				y = d2;
			e.Graphics.DrawImage(img, x, y);
			x += img.Width + 2;
			if (x < this.ClientSize.Width - d)
			{
				string s = ActionName;
				if (string.IsNullOrEmpty(s))
				{
					s = "Attach handler";
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
