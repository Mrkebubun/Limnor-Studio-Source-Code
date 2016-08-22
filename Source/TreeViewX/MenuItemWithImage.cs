/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.TreeViewExt
{
	class MenuItemWithImage : MenuItem
	{
		Image _img;
		public MenuItemWithImage()
		{
		}
		public MenuItemWithImage(string text)
			: base(text)
		{
		}
		public MenuItemWithImage(string text, EventHandler handler)
			: base(text, handler)
		{
		}
		public MenuItemWithImage(string text, Image img)
			: base(text)
		{
			_img = img;
			OwnerDraw = true;
		}
		public MenuItemWithImage(string text, EventHandler handler, Image img)
			: base(text, handler)
		{
			_img = img;
			OwnerDraw = true;
		}
		protected override void OnSelect(EventArgs e)
		{
			base.OnSelect(e);
			//show tooltips here?
		}
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			if (_img == null)
			{
				base.OnMeasureItem(e);
			}
			else
			{
				SizeF sizef = e.Graphics.MeasureString(Text, SystemInformation.MenuFont);
				int h = (int)Math.Ceiling(sizef.Height);
				if (h < _img.Height)
					h = _img.Height;
				e.ItemWidth = (int)Math.Ceiling(sizef.Width) + _img.Width + 1;
				e.ItemHeight = h;
			}
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (_img == null)
			{
				base.OnDrawItem(e);
			}
			else
			{
				Font menuFont = SystemInformation.MenuFont;
				SolidBrush menuBrush = null;

				// Determine menu brush for painting
				if (!Enabled)
				{
					// disabled text if menu item not enabled
					menuBrush = new SolidBrush(SystemColors.GrayText);
				}
				else // Normal (enabled) text
				{
					if ((e.State & DrawItemState.Selected) != 0)
					{
						// Text color when selected (highlighted)
						menuBrush = new SolidBrush(SystemColors.HighlightText);
					}
					else
					{
						// Text color during normal drawing
						menuBrush = new SolidBrush(SystemColors.MenuText);
					}
				}
				// Center the text portion (out to side of image portion)
				SizeF sizef = e.Graphics.MeasureString(Text, SystemInformation.MenuFont);
				StringFormat strfmt = new StringFormat();
				strfmt.LineAlignment = System.Drawing.StringAlignment.Near;

				// Rectangle for image portion
				Rectangle rectImage = e.Bounds;
				// Set image rectangle same dimensions as image
				rectImage.Width = _img.Width;
				rectImage.Height = _img.Height;

				// Fill rectangle with proper background color
				// [use this instead of e.DrawBackground() ]
				if ((e.State & DrawItemState.Selected) != 0)
				{
					// Selected color
					e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
				}
				else
				{
					// Normal background color (when not selected)
					e.Graphics.FillRectangle(SystemBrushes.Menu, e.Bounds);
				}

				// Draw image portion
				e.Graphics.DrawImage(_img, rectImage);

				// Draw string/text portion
				//
				// text portion
				// using menu font
				// using brush determined earlier
				// Start at offset of image rect already drawn
				// Total height,divided to be centered
				// Formated string
				e.Graphics.DrawString(Text,
					   menuFont,
					   menuBrush,
					   e.Bounds.Left + _img.Width + 1,
					   e.Bounds.Top + ((e.Bounds.Height - sizef.Height) / 2),
					   strfmt);
			}
		}
	}
}
