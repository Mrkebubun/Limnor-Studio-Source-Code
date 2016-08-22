/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace VPL
{
	public interface IEventInfoTree
	{
		IEventInfoTree[] GetSubEventInfo();
		string Name { get; }
		EventInfo GetEventInfo();
		int GetEventId();
		bool IsChild(IEventInfoTree eif);
	}
}
