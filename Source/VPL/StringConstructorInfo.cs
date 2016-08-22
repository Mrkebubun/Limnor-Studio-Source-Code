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
	/// <summary>
	/// make it as if a (string = value) constructor
	/// </summary>
	public class StringConstructorInfo : ConstructorInfo
	{
		public StringConstructorInfo()
		{
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override MethodAttributes Attributes
		{
			get { throw new NotImplementedException(); }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			throw new NotImplementedException();
		}

		public override ParameterInfo[] GetParameters()
		{
			throw new NotImplementedException();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { throw new NotImplementedException(); }
		}

		public override Type DeclaringType
		{
			get { throw new NotImplementedException(); }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override string Name
		{
			get { throw new NotImplementedException(); }
		}

		public override Type ReflectedType
		{
			get { throw new NotImplementedException(); }
		}
	}
}
