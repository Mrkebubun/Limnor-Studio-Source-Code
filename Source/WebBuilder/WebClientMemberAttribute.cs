using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Limnor.WebBuilder
{
	public class WebClientMemberAttribute : Attribute
	{
		public WebClientMemberAttribute()
		{
		}
		public static bool IsClientType(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsClientEvent(EventInfo eif)
		{
			object[] vs = eif.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsClientMethod(MethodInfo mif)
		{
			object[] vs = mif.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsClientProperty(PropertyInfo pif)
		{
			object[] vs = pif.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsClientProperty(PropertyDescriptor pif)
		{
			if (pif.Attributes != null)
			{
				Attribute a = pif.Attributes[typeof(WebClientMemberAttribute)];
				if (a != null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
