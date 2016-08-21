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
	public class PropertyInfoInterface : PropertyInfo
	{
		private InterfaceElementProperty _prop;
		public PropertyInfoInterface(InterfaceElementProperty prop)
		{
			_prop = prop;
		}

		public override PropertyAttributes Attributes
		{
			get { return PropertyAttributes.None; }
		}

		public override bool CanRead
		{
			get { return _prop.CanRead; }
		}

		public override bool CanWrite
		{
			get { return _prop.CanWrite; }
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			if (!nonPublic)
			{
			}
			return new MethodInfo[] { };
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			if (!nonPublic)
			{
			}
			return null;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			return new ParameterInfo[] { };
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			if (!nonPublic)
			{
			}
			return null;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		public override Type PropertyType
		{
			get { return _prop.DataTypeEx; }
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
		}

		public override Type DeclaringType
		{
			get { return _prop.Owner.ObjectType; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[] { };
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[] { };
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return _prop.Name; }
		}

		public override Type ReflectedType
		{
			get { return _prop.Owner.ObjectType; }
		}
	}
}
