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
	public class MethodInfoIndexer : MethodInfo
	{
		private ParameterInfo[] _ps;
		private PropertyInfo _pif;
		private RuntimeMethodHandle _handle = new RuntimeMethodHandle();
		public MethodInfoIndexer()
		{
		}
		public MethodInfoIndexer(PropertyInfo property, ParameterInfo[] parameters)
		{
			_pif = property;
			_ps = parameters;
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				return null;
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return MethodAttributes.Public;
			}
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Runtime;
		}

		public override ParameterInfo[] GetParameters()
		{
			return _ps;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			return _pif.GetValue(obj, invokeAttr, binder, parameters, culture);
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return _handle;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return _pif.DeclaringType;
			}
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return _pif.GetCustomAttributes(attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _pif.GetCustomAttributes(inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return _pif.IsDefined(attributeType, inherit);
		}

		public override string Name
		{
			get
			{
				return _pif.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return _pif.ReflectedType;
			}
		}
	}
}
