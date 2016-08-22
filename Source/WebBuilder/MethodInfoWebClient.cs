/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Limnor.WebServerBuilder;
using VPL;
using System.Windows.Forms;

namespace Limnor.WebBuilder
{
	public class MethodInfoWebClient : MethodInfo
	{
		private string _name;
		private ParameterInfo[] _ps;
		private MethodAttributes _attr = MethodAttributes.Public;
		private MethodImplAttributes _iattr = MethodImplAttributes.Runtime;
		private Type _declaringType;
		private Attribute[] _customAttrs;
		public MethodInfoWebClient(string name, ParameterInfo[] parameters, Type componentType, Attribute[] attrs)
		{
			_name = name;
			_ps = parameters;
			_declaringType = componentType;
			_customAttrs = attrs;
		}
		public static bool IsWebObject(object obj)
		{
			if (obj is IWebClientComponent || obj is IWebClientControl || obj is IWebServerProgrammingSupport || obj is ISupportWebClientMethods || obj is IJavascriptType)
				return true;
			return false;
		}
		public static bool IsWebClientObject(object obj)
		{
			Type t = obj as Type;
			if (t != null)
			{
				return JsTypeAttribute.IsJsType(t);
			}
			return (obj is IJavascriptType || obj is IWebClientComponent || obj is IWebClientControl || obj is ISupportWebClientMethods);
		}
		public static bool IsWebServerObject(object obj)
		{
			Type t = obj as Type;
			if (t != null)
			{
				return PhpTypeAttribute.IsPhpType(t);
			}
			return (obj is IPhpType || obj is IWebServerProgrammingSupport || obj is IWebPage);
		}
		public static MethodInfo[] GetWebMethods(bool isStatic, object obj)
		{
			bool includeClient = MethodInfoWebClient.IsWebClientObject(obj);
			bool includeServer = MethodInfoWebClient.IsWebServerObject(obj);
			List<MethodInfo> lst = new List<MethodInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.Public | BindingFlags.Instance;
			}
			Type t = obj as Type;
			if (t == null)
			{
				t = obj.GetType();
			}
			bool isPhp = PhpTypeAttribute.IsPhpType(t);
			bool isJs = JsTypeAttribute.IsJsType(t);
			MethodInfo[] ret = t.GetMethods(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						if (VPLUtil.IsNotForProgramming(ret[i]))
						{
							continue;
						}
						bool include = false;
						if (isPhp)
						{
							if (WebServerMemberAttribute.IsServerMethod(ret[i]))
							{
								lst.Add(ret[i]);
							}
							continue;
						}
						if (isJs)
						{
							if (WebClientMemberAttribute.IsClientMethod(ret[i]))
							{
								lst.Add(ret[i]);
							}
							continue;
						}
						if (includeClient)
						{
							object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
							if (objs != null && objs.Length > 0)
							{
								include = true;
							}
						}
						if (!include)
						{
							if (includeServer)
							{
								object[] objs = ret[i].GetCustomAttributes(typeof(WebServerMemberAttribute), true);
								if (objs != null && objs.Length > 0)
								{
									include = true;
								}
							}
						}
						if (include)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}
		public static PropertyDescriptorCollection GetObjectProperties(EnumReflectionMemberInfoSelectScope scope, object obj, bool browsableOnly)
		{
			bool includeClient = MethodInfoWebClient.IsWebClientObject(obj);
			bool includeServer = MethodInfoWebClient.IsWebServerObject(obj);
			PropertyDescriptorCollection ps = VPLUtil.GetProperties(obj, scope, false, browsableOnly, true);// TypeDescriptor.GetProperties(obj, attrs, false);
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				bool include = false;
				if (includeClient)
				{
					if (WebClientMemberAttribute.IsClientProperty(p))
					{
						include = true;
					}
				}
				if (!include)
				{
					if (includeServer)
					{
						if (WebServerMemberAttribute.IsServerProperty(p))
						{
							include = true;
						}
					}
				}
				if (!include)
				{
					if (includeClient)
					{
						if (!(obj is Form) && (obj is IWebClientControl))
						{
							if (string.CompareOrdinal(p.Name, "Location") != 0)
							{
								include = true;
							}
						}
					}
				}
				if (!include)
				{
					if (includeServer)
					{
						if (obj is IWebServerProgrammingSupport)
						{
							include = true;
						}
					}
				}
				if (include)
				{
					l.Add(p);
				}
			}
			return new PropertyDescriptorCollection(l.ToArray());
		}
		public static EventInfo[] GetWebClientEvents(bool isStatic, object obj)
		{
			bool includeClient = MethodInfoWebClient.IsWebClientObject(obj);
			bool includeServer = MethodInfoWebClient.IsWebServerObject(obj);
			List<EventInfo> lst = new List<EventInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.Public | BindingFlags.Instance;
			}
			EventInfo[] ret;
			IWebClientControlCustomEvents wcce = obj as IWebClientControlCustomEvents;
			if (wcce != null)
			{
				ret = wcce.GetWebClientEvents(isStatic);
			}
			else
			{
				ret = obj.GetType().GetEvents(flags);
			}
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						bool include = false;
						if (includeClient)
						{
							object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
							if (objs != null && objs.Length > 0)
							{
								include = true;
							}
						}
						if (!include)
						{
							if (includeServer)
							{
								object[] objs = ret[i].GetCustomAttributes(typeof(WebServerMemberAttribute), true);
								if (objs != null && objs.Length > 0)
								{
									include = true;
								}
								else
								{
									objs = ret[i].GetCustomAttributes(typeof(WebClientEventByServerObjectAttribute), true);
									if (objs != null && objs.Length > 0)
									{
										include = true;
									}
								}
							}
						}
						if (include)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}
		#region MethodInfo implementation
		public override MethodInfo GetBaseDefinition()
		{
			return null;
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get { return null; }
		}

		public override MethodAttributes Attributes
		{
			get { return _attr; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return _iattr;
		}

		public override ParameterInfo[] GetParameters()
		{
			return _ps;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { return new RuntimeMethodHandle(); }
		}

		public override Type DeclaringType
		{
			get { return _declaringType; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			List<object> lst = new List<object>();
			if (_customAttrs != null && _customAttrs.Length > 0)
			{
				for (int i = 0; i < _customAttrs.Length; i++)
				{
					if (_customAttrs[i] != null)
					{
						if (attributeType != null)
						{
							if (attributeType.IsAssignableFrom(_customAttrs[i].GetType()))
							{
								lst.Add(_customAttrs[i]);
							}
						}
						else
						{
						}
					}
				}
			}
			return lst.ToArray();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _customAttrs;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (_customAttrs != null && _customAttrs.Length > 0)
			{
				for (int i = 0; i < _customAttrs.Length; i++)
				{
					if (_customAttrs[i] != null)
					{
						if (attributeType != null)
						{
							if (attributeType.IsAssignableFrom(_customAttrs[i].GetType()))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override Type ReflectedType
		{
			get { return _declaringType; }
		}
		#endregion
	}
}
