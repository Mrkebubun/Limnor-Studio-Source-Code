/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VPL
{
	public interface ICustomSerialization : IXmlNodeSerializable
	{
		XmlNode CachedXmlNode { get; }
	}
	public interface ICustomContentSerialization
	{
		Dictionary<string, object> CustomContents { get; set; }
	}
	public interface ISerializeAsObject
	{
		bool NeedSerializeAsObject { get; }
	}
	public interface ISerializationProcessor
	{
		void OnDeserialization(XmlNode objectNode);
	}
	/// <summary>
	/// remember the XmlNode holding the contents for the owner
	/// </summary>
	public interface IXmlNodeHolder
	{
		XmlNode DataXmlNode { get; set; }
	}
	public interface ITransferBeforeWrite { }
	public class UseParentObjectAttribute : Attribute
	{
		public UseParentObjectAttribute()
		{
		}
	}
}
