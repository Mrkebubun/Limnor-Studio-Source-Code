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
using LimnorDesigner.Property;

namespace LimnorDesigner.DesignTimeType
{
	public class PropertyInfoX : PropertyInfo
	{
		private TypeX _type;
		private UInt32 _propertyId;
		private PropertyClass _property;
		public PropertyInfoX(TypeX type, UInt32 propertyId)
		{
			_type = type;
			_propertyId = propertyId;
			_property = type.Owner.GetCustomPropertyById(_propertyId);
		}
		public PropertyInfoX(PropertyClass prop)
		{
			_property = prop;
			_type = prop.RootPointer.TypeDesigntime as TypeX;
			_propertyId = prop.MemberId;
		}
		public PropertyClass OwnerProperty
		{
			get
			{
				return _property;
			}
		}
		public override PropertyAttributes Attributes
		{
			get { return PropertyAttributes.None; }
		}

		public override bool CanRead
		{
			get
			{
				if (_property != null)
					return _property.CanRead;
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				if (_property != null)
					return _property.CanWrite;
				return false;
			}
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			MethodInfo getter = GetGetMethod(nonPublic);
			MethodInfo setter = GetSetMethod(nonPublic);
			if (getter != null)
			{
				if (setter != null)
				{
					return new MethodInfo[] { getter, setter };
				}
				else
				{
					return new MethodInfo[] { getter };
				}
			}
			else
			{
				if (setter != null)
				{
					return new MethodInfo[] { setter };
				}
				else
				{
					return new MethodInfo[] { };
				}
			}
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return new MethodInfoGetter(_property);
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			return new ParameterInfo[] { };
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return new MethodInfoSetter(_property);
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
			return _property.DefaultValue;
		}

		public override Type PropertyType
		{
			get { return _property.PropertyType.DataTypeEx; }
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
		{
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
				IList<ConstObjectPointer> lst = _property.GetCustomAttributeList();
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
					object[] vsh = _property.PropertyType.BaseClassType.GetCustomAttributes(inherit);
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
			get
			{
				if (_property != null)
					return _property.Name;
				return string.Empty;
			}
		}

		public override Type ReflectedType
		{
			get { return _type; }
		}
	}
	public abstract class MethodInfoPropertyClassAccessor : MethodInfo
	{
		private PropertyClass _prop;
		public MethodInfoPropertyClassAccessor(PropertyClass prop)
		{
			_prop = prop;
		}
		public PropertyClass Owner
		{
			get
			{
				return _prop;
			}
		}
		public override MethodInfo GetBaseDefinition()
		{
			return null;
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
			get { return MethodAttributes.Public; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Managed;
		}

		public override ParameterInfo[] GetParameters()
		{
			return new ParameterInfo[] { };
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			return _prop.DefaultValue;
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { return new RuntimeMethodHandle(); }
		}

		public override Type DeclaringType
		{
			get { return _prop.PropertyType.DataTypeEx; }
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

		public override Type ReflectedType
		{
			get { return _prop.PropertyType.DataTypeEx; }
		}
	}
	public class MethodInfoGetter : MethodInfoPropertyClassAccessor
	{
		public MethodInfoGetter(PropertyClass prop)
			: base(prop)
		{
		}
		public override string Name
		{
			get { return "getter"; }
		}
	}
	public class MethodInfoSetter : MethodInfoPropertyClassAccessor
	{
		public MethodInfoSetter(PropertyClass prop)
			: base(prop)
		{
		}
		public override string Name
		{
			get { return "setter"; }
		}
		public override ParameterInfo[] GetParameters()
		{
			return new ParameterInfo[] { new ParameterInfoPropertyValue(this.Owner) };
		}
	}
	public class ParameterInfoPropertyValue : ParameterInfo
	{
		private PropertyClass _prop;
		public ParameterInfoPropertyValue(PropertyClass prop)
		{
			_prop = prop;
		}
		public override string Name
		{
			get
			{
				return "value";
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _prop.PropertyType.DataTypeEx;
			}
		}
	}
}
