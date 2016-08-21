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

namespace LimnorDesigner.DesignTimeType
{
	public class MethodInfoX : MethodInfo
	{
		private TypeX _type;
		private UInt32 _methodId;
		private MethodClass _method;
		public MethodInfoX(TypeX type, UInt32 methodId)
		{
			_methodId = methodId;
			_type = type;
			_method = _type.Owner.GetCustomMethodById(methodId);
		}
		public MethodInfoX(MethodClass method)
		{
			_methodId = method.MethodID;
			_method = method;
			_type = method.RootPointer.TypeDesigntime as TypeX;
		}
		public TypeX Owner
		{
			get
			{
				return _type;
			}
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
			List<ParameterInfo> lst = new List<ParameterInfo>();
			List<ParameterClass> ps = _method.Parameters;
			if (ps != null)
			{
				foreach (ParameterClass p in ps)
				{
					lst.Add(new ParameterInfoX(p));
				}
			}
			return lst.ToArray();
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
			get { return _type; }
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			List<object> vs = new List<object>();
			if (attributeType != null)
			{
				IList<ConstObjectPointer> lst = _method.GetCustomAttributeList();
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
			get { return _type; }
		}

		public override Type ReturnType
		{
			get
			{
				if (_method.ReturnValue == null)
					return typeof(void);
				return _method.ReturnValue.DataTypeEx;
			}
		}
	}
}
