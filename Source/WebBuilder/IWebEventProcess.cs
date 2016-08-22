/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	public interface IWebEventProcess
	{
		void OnFinishEvent(string eventName, StringCollection handler);

	}
}
