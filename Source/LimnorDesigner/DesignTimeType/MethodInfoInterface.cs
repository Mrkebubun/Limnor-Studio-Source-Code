/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LimnorDesigner.Interface;

namespace LimnorDesigner.DesignTimeType
{
	public class MethodInfoInterface : MethodInfo
	{
		private InterfaceElementMethod _method;
		public MethodInfoInterface(InterfaceElementMethod method)
		{
			_method = method;
		}

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
			get { return MethodAttributes.Public; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Managed;
		}

		public override ParameterInfo[] GetParameters()
		{
			List<InterfaceElementMethodParameter> ps = _method.Parameters;
			if (ps != null && ps.Count > 0)
			{
				ParameterInfo[] aps = new ParameterInfo[ps.Count];
				for (int i = 0; i < ps.Count; i++)
				{
					aps[i] = new ParameterInfoInterfaceMethod(ps[i]);
				}
				return aps;
			}
			return new ParameterInfo[] { };
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
			get
			{
				return _method.Interface.DataTypeEx;
			}
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			List<object> vs = new List<object>();
			if (attributeType != null)
			{
				if (_method.Interface != null)
				{
					IList<ConstObjectPointer> lst = _method.Interface.GetCustomAttributeList();
					if (lst != null && lst.Count > 0)
					{
						foreach (ConstObjectPointer o in lst)
						{
							if (attributeType == null || attributeType.IsAssignableFrom(o.BaseClassType))
							{
								vs.Add(o.Value);
							}
						}
					}
					if (inherit)
					{
						object[] vsh = _method.Interface.BaseClassType.GetCustomAttributes(inherit);
						if (vsh != null)
						{
							for (int i = 0; i < vsh.Length; i++)
							{
								if (vsh[i] != null)
								{
									if (attributeType == null || attributeType.IsAssignableFrom(vsh[i].GetType()))
									{
										vs.Add(vsh[i]);
									}
								}
							}
						}
					}
				}
			}
			return vs.ToArray();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return GetCustomAttributes(null, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			object[] vs = GetCustomAttributes(attributeType, inherit);
			if (vs != null)
			{
				for (int i = 0; i < vs.Length; i++)
				{
					if (vs[i] != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string Name
		{
			get { return _method.Name; }
		}

		public override Type ReflectedType
		{
			get { return _method.Interface.DataTypeEx; }
		}
		public override Type ReturnType
		{
			get
			{
				if (_method.ReturnType == null)
				{
					return typeof(void);
				}
				return _method.ReturnType.DataTypeEx;
			}
		}
	}
	public class ParameterInfoInterfaceMethod : ParameterInfo
	{
		private InterfaceElementMethodParameter _p;
		public ParameterInfoInterfaceMethod(InterfaceElementMethodParameter p)
		{
			_p = p;
		}
		public override string Name
		{
			get
			{
				return _p.Name;
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _p.DataTypeEx;
			}
		}
	}
}
