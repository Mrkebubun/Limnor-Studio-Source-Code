/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using XmlUtility;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	public static class WebClientData
	{
		private static StringCollection _clientControlProperties;
		public static Dictionary<string, Type> GetJavascriptTypes()
		{
			Dictionary<string, Type> l = new Dictionary<string, Type>();
			l.Add("Object", typeof(JsObject));
			l.Add("Boolean", typeof(JsBool));
			l.Add("Number", typeof(JsNumber));
			l.Add("String", typeof(JsString));
			l.Add("DateTime", typeof(JsDateTime));
			l.Add("TimeSpan", typeof(JsTimeSpan));
			l.Add("Array", typeof(JsArray));
			return l;
		}
		public static StringCollection ClientControlProperties
		{
			get
			{
				if (_clientControlProperties == null)
				{
					_clientControlProperties = new StringCollection();
					_clientControlProperties.Add("Text");
					_clientControlProperties.Add("Opacity");
					_clientControlProperties.Add("Visible");
					_clientControlProperties.Add("Enabled");
					_clientControlProperties.Add("BackColor");
					_clientControlProperties.Add("ForeColor");
					_clientControlProperties.Add("Left");
					_clientControlProperties.Add("Top");
					_clientControlProperties.Add("Width");
					_clientControlProperties.Add("Height");
				}
				return _clientControlProperties;
			}
		}
		public static IJavascriptType FromString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new JsString();
			}
			if (value[0] == '[' && value[value.Length - 1] == ']')
			{
				JsArray a = new JsArray();
				a.SetValueString(value);
				return a;
			}
			else
			{
				int pos = value.IndexOf('!');
				if (pos <= 0)
				{
					return new JsString(value);
				}
				string st = value.Substring(0, pos);
				Type t = XmlUtil.GetKnownType(st);
				if (t == null)
				{
					return new JsString(value);
				}
				IJavascriptType js = (IJavascriptType)Activator.CreateInstance(t);
				js.SetValueString(value.Substring(pos + 1));
				return js;
			}
		}
	}
	[KeyMouseEvent]
	public delegate bool WebControlSimpleEventHandler(IWebClientControl sender);
	[KeyMouseEvent]
	public delegate bool WebControlMouseEventHandler(IWebClientControl sender, WebMouseEventArgs e);
	[KeyMouseEvent]
	public delegate bool WebControlKeyEventHandler(IWebClientControl sender, WebKeyEventArgs e);
}
