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
	public interface IEventHolder
	{
		event EventHandler Event;
	}
	public interface ICustomEventDescriptor
	{
		EventInfo[] GetEvents();
		EventInfo GetEvent(string eventName);
		EventInfo GetEventById(int eventId);
		int GetEventId(string eventName);
		string GetEventNameById(int eventId);
		bool IsCustomEvent(string eventName);
	}
	public interface ICustomEventMethodDescriptor : ICustomEventDescriptor
	{
		IEventHolder GetEventHolder(string eventName);
		Type GetEventArgumentType(string eventName);
		void SetEventChangeMonitor(EventHandler monitor);
		MethodInfo[] GetMethods();
	}
}
