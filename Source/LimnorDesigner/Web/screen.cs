/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LimnorDesigner.Web
{
	public class screen
	{
		public screen()
		{
		}
		[Description("The total width of the screen, in pixels.")]
		public int width
		{
			get
			{
				return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			}
		}
		[Description("The total height of the screen, in pixels.")]
		public int height
		{
			get
			{
				return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
			}
		}
		[Description("Specifies the width of the screen, in pixels, minus interface features such as the taskbar in Windows.")]
		public int availWidth
		{
			get
			{
				return System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
			}
		}
		[Description("Specifies the height of the screen, in pixels, minus interface features such as the taskbar in Windows.")]
		public int availHeight
		{
			get
			{
				return System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
			}
		}
		[Description("The bit depth of the color palette available for displaying images in bits per pixel.")]
		public int colorDepth
		{
			get
			{
				return System.Windows.Forms.Screen.PrimaryScreen.BitsPerPixel;
			}
		}
	}
}
