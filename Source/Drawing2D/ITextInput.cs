/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	public interface ITextInput
	{
		bool EnableEditing { get; set; }
		bool MultiLine { get; }
		int TextBoxWidth { get; }
		int TextBoxHeight { get; }
		Point Location { get; }
		Font TextBoxFont { get; }
		int TabIndex { get; set; }
		DrawGroupBox Container { get; }
		ControlBindingsCollection DataBindings { get; }
		string GetText();
		string GetTextPropertyName();
		void SetText(string text);
		Point GetAbsolutePosition();
	}
}
