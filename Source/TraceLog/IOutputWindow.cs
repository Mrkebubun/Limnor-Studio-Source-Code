/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Trace and log
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TraceLog
{
	public interface IOutputWindow
	{
		void AppendMessage(string msg, params object[] values);
		void ShowException(Exception e);
		Form FindForm();
	}
}
