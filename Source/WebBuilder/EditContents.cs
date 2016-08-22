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
	public class EditContents
	{
		private HtmlContent _htmlEditor;
		public EditContents(HtmlContent editor)
		{
			_htmlEditor = editor;
		}
		public Color BackColor
		{
			get
			{
				return _htmlEditor.BackColor;
			}
		}
		public string DocumentText
		{
			get
			{
				return _htmlEditor.DocumentText;
			}
		}
		public string HtmlContents
		{
			get
			{
				return _htmlEditor.BodyHtml;
			}
			set
			{
				PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(_htmlEditor, true);
				foreach (PropertyDescriptor p in ps)
				{
					if (string.CompareOrdinal("BodyHtml", p.Name) == 0)
					{
						p.SetValue(_htmlEditor, value);
						break;
					}
				}
			}
		}
		public override string ToString()
		{
			return "...";
		}
	}
}
