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
	public interface IObjectCompiler
	{
		void CreateActionJavaScript(string codeName, string methodName, StringCollection sb, StringCollection parameters, string returnReceiver);
	}
	public interface IJavascriptEventHolder
	{
		void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode);
	}
	public interface IJavascriptPropertyHolder
	{
		string GetJavascriptPropertyCodeName(string propertyName);
	}
	public class ObjectCompilerAttribute : Attribute
	{
		private Type _compilerType;
		public ObjectCompilerAttribute(Type compilerObjectType)
		{
			_compilerType = compilerObjectType;
		}
		public static bool UsObjectCompiler(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(ObjectCompilerAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
		public static IObjectCompiler GetCompilerObject(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(ObjectCompilerAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					ObjectCompilerAttribute oc = vs[0] as ObjectCompilerAttribute;
					return Activator.CreateInstance(oc._compilerType) as IObjectCompiler;
				}
			}
			return null;
		}
		public static IJavascriptEventHolder GetJavascriptEventHolder(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(ObjectCompilerAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					ObjectCompilerAttribute oc = vs[0] as ObjectCompilerAttribute;
					return Activator.CreateInstance(oc._compilerType) as IJavascriptEventHolder;
				}
			}
			return null;
		}
	}
}
