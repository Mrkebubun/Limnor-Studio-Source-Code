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
	/// all events are defined as EventHandler type
	/// with EventArgGeneric as the EventArgs.
	/// 
	/// </summary>
	public class EventArgGeneric : EventArgs
	{
		private Dictionary<string, object> _params;
		public EventArgGeneric()
		{
		}
		public int ParamCount
		{
			get
			{
				if (_params == null)
					return 0;
				return _params.Count;
			}
		}
		public void AddParameterName(string name)
		{
			if (_params == null)
				_params = new Dictionary<string, object>();
			if (!_params.ContainsKey(name))
			{
				_params.Add(name, null);
			}
		}
		public void DeleteParameter(string name)
		{
			if (_params != null)
			{
				if (_params.ContainsKey(name))
				{
					_params.Remove(name);
				}
			}
		}
		public string GetParameterName(int index)
		{
			if (_params != null && index >= 0 && index < _params.Count)
			{
				Dictionary<string, object>.KeyCollection.Enumerator kc = _params.Keys.GetEnumerator();
				int i = 0;
				while (kc.MoveNext())
				{
					if (i == index)
						return kc.Current;
					i++;
				}
			}
			return null;
		}
		public void SetParameterValue(string name, object v)
		{
			if (_params == null)
				_params = new Dictionary<string, object>();
			if (!_params.ContainsKey(name))
			{
				_params.Add(name, v);
			}
			else
			{
				_params[name] = v;
			}
		}
		public object GetParamValue(string name)
		{
			if (_params != null)
			{
				if (_params.ContainsKey(name))
					return _params[name];
			}
			return null;
		}
	}
}
