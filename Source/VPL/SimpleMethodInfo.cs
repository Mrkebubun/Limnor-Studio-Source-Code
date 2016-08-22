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
	public class SimpleMethodInfo : MethodInfo
	{
		private MethodBase _mb;
		private IDynamicMethodParameters _owner;
		private object _attrs;
		private RuntimeMethodHandle _handle = new RuntimeMethodHandle();
		public SimpleMethodInfo(MethodBase method, IDynamicMethodParameters owner, object attrs)
		{
			_attrs = attrs;
			_mb = method;
			_owner = owner;
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
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
			return MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
		}

		public override ParameterInfo[] GetParameters()
		{
			return _owner.GetDynamicMethodParameters(_mb.Name, _attrs);
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			IDynamicMethodParameters dmp = obj as IDynamicMethodParameters;
			if (dmp != null)
			{
				return dmp.InvokeWithDynamicMethodParameters(_mb.Name, invokeAttr, binder, parameters, culture);
			}
			return null;
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
				return _mb.DeclaringType;
			}
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return _mb.GetCustomAttributes(attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _mb.GetCustomAttributes(inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return _mb.IsDefined(attributeType, inherit);
		}

		public override string Name
		{
			get
			{
				return _mb.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return _mb.ReflectedType;
			}
		}
	}
}
