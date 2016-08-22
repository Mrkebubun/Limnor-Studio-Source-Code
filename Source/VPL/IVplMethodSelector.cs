/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;

namespace VPL
{
	public interface IVplMethodSelector
	{
		bool SelectMethodForParam(IWindowsFormsEditorService edSvc, bool runAtWebClient, Type t, ref object value);
	}
}
