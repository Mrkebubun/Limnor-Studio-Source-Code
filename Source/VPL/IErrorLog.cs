/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	/// <summary>
	/// Build implements it. VPLUtil holds it. 
	/// When VPLUtil has it some components uses to log errors; otherwise throw
	/// </summary>
	public interface IErrorLog
	{
		void LogError(string msg, params object[] values);
	}
	public interface IShowMessage : IErrorLog
	{
		void ShowMessage(string message, params object[] values);
	}
}
