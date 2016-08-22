/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace XmlUtility
{
	/// <summary>
	/// used for converting a string into a value during reading
	/// </summary>
	public class GenericConverterContext : ITypeDescriptorContext
	{
		private PropertyDescriptor _prop;
		private object _inst;
		public GenericConverterContext(PropertyDescriptor property, object instance)
		{
			_prop = property;
			_inst = instance;
		}

		#region ITypeDescriptorContext Members

		public IContainer Container
		{
			get { return null; }
		}

		public object Instance
		{
			get { return _inst; }
		}

		public void OnComponentChanged()
		{

		}

		public bool OnComponentChanging()
		{
			return false;
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get { return _prop; }
		}

		#endregion

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			return null;
		}

		#endregion
	}
}
