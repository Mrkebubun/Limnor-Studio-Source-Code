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
using System.Globalization;

namespace LimnorDesigner.ResourcesManager
{
	public class TextBoxResEditor : TextBox
	{
		private ResourcePointer _pointer;
		private CultureInfo _culture;
		private bool _loading;
		public TextBoxResEditor()
		{
		}
		public void SetResourceOwner(ResourcePointer pointer, CultureInfo culture)
		{
			_pointer = pointer;
			_culture = culture;
			_loading = true;
			if (_pointer == null)
			{
				Text = "";
			}
			else
			{
				if (_culture == null)
				{
					Text = _pointer.GetResourceString(string.Empty);
				}
				else
				{
					Text = _pointer.GetResourceString(_culture.Name);
				}
			}
			_loading = false;
		}
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			if (!_loading && _pointer != null)
			{
				if (_culture == null)
				{
					_pointer.SetResourceString(string.Empty, Text);
				}
				else
				{
					_pointer.SetResourceString(_culture.Name, Text);
				}
				_pointer.IsChanged = true;
			}
		}
	}
}
