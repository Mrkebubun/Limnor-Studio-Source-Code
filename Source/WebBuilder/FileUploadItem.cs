/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Limnor.WebBuilder
{
	public class FileUploadItem
	{
		public FileUploadItem()
		{
			CaptionFont = new Font("Times New Roman", (float)12);
			CaptionColor = Color.Black;
		}
		[Description("Gets and sets a string for file upload caption.")]
		public string CaptionText { get; set; }
		[Description("Gets and sets a font for file upload caption.")]
		public Font CaptionFont { get; set; }
		[Description("Gets and sets a color for file upload caption.")]
		public Color CaptionColor { get; set; }
		[Description("Gets and sets file size limitation")]
		public int FileSizeLimit { get; set; }
	}
	public class FileUploadItemList
	{
	}
}
