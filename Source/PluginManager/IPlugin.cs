/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Plugin Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorVisualProgramming
{
	public class EventArgsDataName : EventArgs
	{
		private string _name;
		private object _data;
		public EventArgsDataName(string dataName, object data)
		{
			_name = dataName;
			_data = data;
		}
		public string DataName
		{
			get
			{
				return _name;
			}
		}
		public object Data
		{
			get
			{
				return _data;
			}
		}
	}
	public delegate void EventHandlerDataChanged(IPlugin sender, EventArgsDataName e);
	public interface IPlugin
	{
		void OnInitialize(IPluginManager manager);
		void OnDataChanged(IPluginManager manager, EventArgsDataName data);
		event EventHandlerDataChanged DataChanged;
	}
}
