/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// represent a null value
	/// </summary>
	public class NullObjectPointer : DataTypePointer, ICustomTypeDescriptor
	{
		#region fields and constructors
		public NullObjectPointer()
		{
		}
		public NullObjectPointer(DataTypePointer type)
		{
			SetDataType(type);
		}
		public NullObjectPointer(TypePointer type)
			: base(type)
		{
		}
		public NullObjectPointer(ClassPointer type)
			: base(type)
		{
		}
		public NullObjectPointer(string name, Type type)
			: base(new TypePointer(type))
		{
			Name = name;
		}
		#endregion
		#region NullDescriptor
		class NullDescriptor : PropertyDescriptor
		{
			private NullObjectPointer _owner;
			public NullDescriptor(NullObjectPointer owner)
				: base(owner.Name, new Attribute[] { })
			{
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(NullObjectPointer); }
			}

			public override object GetValue(object component)
			{
				return "null";
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region Properties
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (VPLUtil.CompilerContext_JS)
					return EnumWebRunAt.Client;
				return base.RunAt;
			}
		}
		public override string DisplayName
		{
			get
			{
				return "null";
			}
		}
		public override string CodeName
		{
			get
			{
				return "null";
			}
		}
		#endregion
		#region Methods
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodePrimitiveExpression(null);
		}
		public override string CreatePhpScript(StringCollection sb)
		{
			return null;
		}
		public override string CreateJavaScript(StringCollection sb)
		{
			return null;
		}
		public override Type BaseClassType
		{
			get
			{
				return typeof(void);
			}
		}
		public override IObjectPointer DataType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string ToString()
		{
			return "null";
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { new NullDescriptor(this) });
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}

}
