using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace VPL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TextPropertyBag:ICustomTypeDescriptor
	{
		private Dictionary<string, string> _values;
		public TextPropertyBag()
		{
		}
		public int Count
		{
			get
			{
				if (_values == null)
					return 0;
				return _values.Count;
			}
		}
		[Browsable(false)]
		public string this[string key]
		{
			get
			{
				if (_values != null)
				{
					if (_values.ContainsKey(key))
					{
						return _values[key];
					}
				}
				return null;
			}
		}
		[Browsable(false)]
		public void SetValue(string key, string value)
		{
			if (_values == null)
				_values = new Dictionary<string, string>();
			if (_values.ContainsKey(key))
			{
				_values[key] = value;
			}
			else
			{
				_values.Add(key, value);
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> GetValues()
		{
			if (_values == null)
				_values = new Dictionary<string, string>();
			return _values;
		}
		[Browsable(false)]
		public string[] Values
		{
			get
			{
				List<string> lst = new List<string>();
				if (_values != null)
				{
					foreach (KeyValuePair<string, string> kv in _values)
					{
						if (string.IsNullOrEmpty(kv.Value))
						{
							lst.Add(string.Format(CultureInfo.InvariantCulture, "{0}", kv.Key));
						}
						else
						{
							lst.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}", kv.Key,kv.Value));
						}
					}
				}
				return lst.ToArray();
			}
			set
			{
				_values = new Dictionary<string, string>();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						if (!string.IsNullOrEmpty(value[i]))
						{
							string k,v;
							int pos = value[i].IndexOf('=');
							if (pos < 0)
							{
								k = value[i];
								v = string.Empty;
							}
							else if (pos == 0)
							{
								k = null;
								v = null;
							}
							else
							{
								k = value[i].Substring(0, pos);
								v = value[i].Substring(pos + 1);
							}
							if (!string.IsNullOrEmpty(k))
							{
								_values.Add(k, v);
							}
						}
					}
				}
			}
		}
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
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				lst.Add(p);
			}
			//test
			if (_values == null)
			{
				_values = new Dictionary<string, string>();
				_values.Add("p1", "test");
			}
			if (_values != null)
			{
				foreach (KeyValuePair<string, string> kv in _values)
				{
					lst.Add(new PropertyDescriptorValue(kv.Key, attributes));
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
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
		#region PropertyDescriptorValue
		class PropertyDescriptorValue : PropertyDescriptor
		{
			public PropertyDescriptorValue(string name, Attribute[] attrs)
				: base(name, attrs)
			{
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(TextPropertyBag); }
			}

			public override object GetValue(object component)
			{
				TextPropertyBag pb = (TextPropertyBag)component;
				return pb[this.Name];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
				TextPropertyBag pb = (TextPropertyBag)component;
				pb.SetValue(this.Name, string.Empty);
			}

			public override void SetValue(object component, object value)
			{
				TextPropertyBag pb = (TextPropertyBag)component;
				pb.SetValue(this.Name, (string)value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
	}
}
