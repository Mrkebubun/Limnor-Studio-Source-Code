/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Limnor.WebServerBuilder
{
	public class WebServerMemberAttribute : Attribute
	{
		public WebServerMemberAttribute()
		{
		}
		public static bool IsServerEvent(EventInfo eif)
		{
			object[] vs = eif.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsServerProperty(PropertyDescriptor pif)
		{
			if (pif.Attributes != null)
			{
				Attribute a = pif.Attributes[typeof(WebServerMemberAttribute)];
				if (a != null)
				{
					return true;
				}
			}
			return false;
		}
		public static bool IsServerProperty(PropertyInfo pif)
		{
			object[] vs = pif.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsServerMethod(MethodInfo mif)
		{
			object[] vs = mif.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsServerMethod(Type t, string methodName)
		{
			MethodInfo mif = t.GetMethod(methodName);
			if (mif != null)
			{
				object[] vs = mif.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
		public static bool IsWebServerMember(object[] vs)
		{
			if (vs != null)
			{
				for (int i = 0; i < vs.Length; i++)
				{
					WebServerMemberAttribute w = vs[i] as WebServerMemberAttribute;
					if (w != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
