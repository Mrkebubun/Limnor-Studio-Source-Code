/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;
using System.CodeDom;
using System.Globalization;

namespace Limnor.Application
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class CategoryWrapper : ICustomTypeDescriptor, IIndexerAsProperty
	{
		private ConfigCategory _cat;
		private ApplicationConfiguration _owner;
		public CategoryWrapper(ConfigCategory category, ApplicationConfiguration owner)
		{
			_cat = category;
			_owner = owner;
		}
		public override string ToString()
		{
			return "Number of properties:" + _cat.Properties.Properties.Count.ToString();
		}
		#region PropertyDescriptorCategory
		/// <summary>
		/// represent a category
		/// </summary>
		class PropertyDescriptorConfig : PropertyDescriptor
		{
			private ConfigProperty _property;
			private ApplicationConfiguration _owner;
			/// <summary>
			/// 
			/// </summary>
			/// <param name="name">it is the category name</param>
			/// <param name="attrs"></param>
			public PropertyDescriptorConfig(string name, Attribute[] attrs, ConfigProperty property, ApplicationConfiguration owner)
				: base(name, attrs)
			{
				_owner = owner;
				_property = property;
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(ConfigProperty); }
			}

			public override object GetValue(object component)
			{
				return _property.DefaultData;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get
				{
					return _property.DataType;
				}
			}

			public override void ResetValue(object component)
			{
				_property.ResetValue();
				_owner.Save();
			}

			public override void SetValue(object component, object value)
			{
				_property.SetSetting(value);
				_owner.Save();
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
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
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			//
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			PropertyDescriptor[] a = new PropertyDescriptor[ps.Count];
			ps.CopyTo(a, 0);
			list.AddRange(a);
			//
			foreach (ConfigProperty cp in _cat.Properties.Properties)
			{
				PropertyDescriptorConfig pc = new PropertyDescriptorConfig(cp.DataName, attributes, cp, _owner);
				list.Add(pc);
			}
			ps = new PropertyDescriptorCollection(list.ToArray());
			//
			return ps;
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

		#region IIndexerAsProperty Members

		public bool IsIndexer(string name)
		{
			foreach (ConfigProperty p in _cat.Properties.Properties)
			{
				if (string.Compare(p.DataName, name, StringComparison.Ordinal) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public CodeExpression CreateCodeExpression(CodeExpression target, string name)
		{
			return new CodeIndexerExpression(target, new CodePrimitiveExpression(name));
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		public string GetPhpScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		public Type IndexerDataType(string name)
		{
			foreach (ConfigProperty p in _cat.Properties.Properties)
			{
				if (string.Compare(p.DataName, name, StringComparison.Ordinal) == 0)
				{
					return p.DataType;
				}
			}
			return null;
		}
		#endregion
	}
}
