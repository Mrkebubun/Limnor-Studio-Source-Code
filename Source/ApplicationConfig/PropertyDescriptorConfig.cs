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

namespace Limnor.Application
{
	class PropertyDescriptorConfig : PropertyDescriptor
	{
		private ApplicationConfiguration _config;
		private ConfigCategory _cat;
		private ConfigProperty _prop;
		public PropertyDescriptorConfig(string name, Attribute[] attrs, ApplicationConfiguration config, ConfigCategory category, ConfigProperty property)
			: base(name, attrs)
		{
			_config = config;
			_cat = category;
			_prop = property;
		}
		public ApplicationConfiguration Config { get { return _config; } }
		public ConfigCategory ConfigCategory { get { return _cat; } }
		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get { return typeof(ApplicationConfiguration); }
		}

		public override object GetValue(object component)
		{
			return _prop.DefaultData;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return _prop.DataType; }
		}

		public override void ResetValue(object component)
		{
			_prop.ResetValue();
		}

		public override void SetValue(object component, object value)
		{
			_prop.SetSetting(value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}

}
