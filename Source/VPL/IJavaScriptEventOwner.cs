/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace VPL
{
	/// <summary>
	/// implemented by web client components for adding event-handler maps
	/// and generating event-linking at the time of producing html code for the component
	/// </summary>
	public interface IJavaScriptEventOwner
	{
		void LinkJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode, bool isDynamic);
		void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode);
	}
}
