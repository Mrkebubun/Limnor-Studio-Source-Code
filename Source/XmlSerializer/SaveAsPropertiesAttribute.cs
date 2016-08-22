/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlSerializer
{
	public class SaveAsPropertiesAttribute : Attribute
	{
		private bool _hasParent;
		public SaveAsPropertiesAttribute()
		{
		}
		public SaveAsPropertiesAttribute(bool hasParent)
		{
			_hasParent = hasParent;
		}
		public bool HasParent
		{
			get
			{
				return _hasParent;
			}
		}
	}
	public class SaveAsObjectAttribute : Attribute
	{
		private bool _asObject = true;
		public SaveAsObjectAttribute()
		{
		}
		public SaveAsObjectAttribute(bool asObject)
		{
			_asObject = asObject;
		}
		public bool AsObject
		{
			get
			{
				return _asObject;
			}
			set
			{
				_asObject = value;
			}
		}
	}

	/// <summary>
	/// a property has a setter but still do not save it
	/// </summary>
	public class IgnoreSetterAttribute : Attribute
	{
		public IgnoreSetterAttribute()
		{
		}
	}

	public interface IBeforeSerializeNotify
	{
		void OnBeforeRead(XmlObjectReader reader, XmlNode node);
		void OnBeforeWrite(XmlObjectWriter writer, XmlNode node);
		void ReloadFromXmlNode();
		void UpdateXmlNode(XmlObjectWriter writer);
		XmlNode XmlData { get; }
	}
	/// <summary>
	/// give it a chance to initialize the member before deserialize the member start
	/// </summary>
	public interface ISerializeParent
	{
		void OnMemberCreated(object member);
	}
	/// <summary>
	/// when reading, set class id by the reader. later the class id can be used to search for the class
	/// </summary>
	public interface IUseClassId
	{
		void SetClassId(UInt32 classId);
	}
}
