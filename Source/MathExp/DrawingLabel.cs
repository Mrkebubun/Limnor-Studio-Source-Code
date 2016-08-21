/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MathExp
{
	public class DrawingLabel : RelativeDrawing
	{
		public DrawingLabel(Control owner)
			: base(owner)
		{
			Text = "Label";
			this.Size = new Size(20, 20);
		}
		private bool _is;
		public bool IsSelected { get { return _is; } set { _is = value; } }
		public override UInt32 OwnerID
		{
			get
			{
				IControlWithID c = _owner as IControlWithID;
				if (c != null)
				{
					return c.ControlID;
				}
				return 0;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			SizeF s = e.Graphics.MeasureString(this.Text, this.Font);
			int w = (int)s.Width;
			int h = (int)s.Height;
			if (w != Size.Width || h != Height)
			{
				this.Size = new Size(w, h);
			}
			e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, 0, 0);
		}
		#region ICloneable
		public override object Clone()
		{
			DrawingLabel obj = (DrawingLabel)base.Clone();
			obj.Text = Text;
			obj.Font = Font;
			obj.ForeColor = ForeColor;
			obj.BackColor = BackColor;
			return obj;
		}
		#endregion
	}
}
