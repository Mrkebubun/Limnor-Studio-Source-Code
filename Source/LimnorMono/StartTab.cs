/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LimnorVOB
{
	class StartTab : Control
	{
		private string _displayString = "Limnor Visual Object Builder lets you build software applications and components visually and codelessly.";
		public StartTab()
		{
			this.Dock = DockStyle.Fill;
		}
		protected override void OnPaint(PaintEventArgs pe)
		{
			try
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				pe.Graphics.FillRectangle(new LinearGradientBrush(this.Bounds, Color.White, Color.Blue, 45F), this.Bounds);
				pe.Graphics.DrawString(_displayString, Font, new SolidBrush(Color.Black), this.Bounds, sf);
			}
			catch (Exception ex)
			{
				pe.Graphics.DrawString(ex.ToString(), this.Font, Brushes.Red, 0, 0);
			}
		} // OnPaint

	}
}
