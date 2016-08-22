/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VPL
{
	public class CheckedListBoxLanguages : CheckedListBox
	{
		public CheckedListBoxLanguages()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0 && e.Index < Items.Count)
			{
				if (this.CheckedIndices.Contains(e.Index))
				{
					e.Graphics.FillRectangle(Brushes.LightGreen, e.Bounds);
					CheckBoxRenderer.DrawCheckBox(e.Graphics, new System.Drawing.Point(e.Bounds.X, e.Bounds.Y), System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
				}
				else
				{
					CheckBoxRenderer.DrawCheckBox(e.Graphics, new System.Drawing.Point(e.Bounds.X, e.Bounds.Y), System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
				}
				Cultrue c = Items[e.Index] as Cultrue;
				float x = (float)(e.Bounds.X + 16);
				if (c.Image != null)
				{
					e.Graphics.DrawImage(c.Image, x, (float)(e.Bounds.Y + 1));
					x += 16;
				}
				e.Graphics.DrawString(c.ToString(), this.Font, Brushes.Black, x, (float)(e.Bounds.Y + 1));
			}
		}
	}
}
