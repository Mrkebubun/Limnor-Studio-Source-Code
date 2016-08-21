/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VSPrj;

namespace LimnorDesigner
{
	public interface IPropertiesWrapperOwner : IWithProject
	{
		PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes);
		object GetPropertyOwner(int id, string propertyName);
		bool AsWrapper { get; }
	}
	public class PropertiesWrapper : ICustomTypeDescriptor, IWithProject, ITypeScopeHolder, IScopeMethodHolder
	{
		private IPropertiesWrapperOwner _owner;
		private int _id;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="wrapperId">the owner defined value for identifying different wrappers in the case the owner may define more than one set of properties</param>
		public PropertiesWrapper(IPropertiesWrapperOwner owner, int wrapperId)
		{
			_owner = owner;
			_id = wrapperId;
		}
		public IPropertiesWrapperOwner Owner
		{
			get
			{
				return _owner;
			}
		}
		public int ID
		{
			get
			{
				return _id;
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
			return _owner.GetWrappedProperties(_id, attributes);
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			if (pd != null)
			{
				return _owner.GetPropertyOwner(_id, pd.Name);
			}
			return this;
		}

		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _owner.Project; }
		}

		#endregion

		#region ITypeScopeHolder Members

		public DataTypePointer GetTypeScope(string name)
		{
			ITypeScopeHolder th = _owner as ITypeScopeHolder;
			if (th != null)
			{
				return th.GetTypeScope(name);
			}
			return null;
		}

		#endregion

		#region IScopeMethodHolder Members

		public MethodClass GetScopeMethod()
		{
			IScopeMethodHolder mh = _owner as IScopeMethodHolder;
			if (mh != null)
			{
				return mh.GetScopeMethod();
			}
			return null;
		}

		#endregion
	}
}
