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
using System.CodeDom;
using System.Collections;

namespace VPL
{
	public enum WriteResult { NoValue, WriteOK, WriteFail }
	public interface IXmlCodeReader
	{
		UInt32 ClassId { get; }
		T ReadValue<T>(XmlNode node, object parent);
		T ReadObject<T>(XmlNode node);
		object ReadValue(XmlNode node, object parentObject, Type t);
		Stack ObjectStack { get; }
		object ReadValue(XmlNode node);
		void ReadArray(XmlNodeList nodes, Array list, object parentObject);
		object ReadObject(XmlNode node, object parent);
		object ReadObject(XmlNode node, object parent, Type type);
		void ReadObjectFromXmlNode(XmlNode node, object obj, Type type, object parentObject);
		string ProjectFolder { get; }
		string GetName(XmlNode node);
		bool GetAttributeBoolDefFalse(XmlNode node, string name);
		IPostOwnersSerialize[] PopPostSerializers();
		void PushPostOwnersDeserializers(List<IPostOwnersSerialize> objs);
		void AddPostOwnersDeserializers(IPostOwnersSerialize obj);
	}
	public interface IXmlCodeWriter
	{
		WriteResult WriteObjectToNode(XmlNode node, object value);
		WriteResult WriteObjectToNode(XmlNode node, object value, bool saveType);
		WriteResult WriteValue(XmlNode parent, object value, object parentObject);
		WriteResult WriteArray(XmlNode parent, Array propValue);
		XmlNode CreateSingleNewElement(XmlNode nodeParent, string name);
		void SetName(XmlNode node, string name);
		void SetAttribute(XmlNode node, string name, object value);
	}
	public interface IXmlCodeReaderWriterHolder
	{
		IXmlCodeWriter GetWriter();
		void SetWriter(IXmlCodeWriter writer);
		IXmlCodeReader GetReader();
		void SetReader(IXmlCodeReader reader);
	}
	/// <summary>
	/// take over the serialization
	/// </summary>
	public interface IXmlNodeSerializable
	{
		void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node);
		void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node);
	}
	/// <summary>
	/// normal properties read/write first, then customized serialization
	/// </summary>
	public interface IPostXmlNodeSerialize
	{
		void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node);
		void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node);
	}
	/// <summary>
	/// customized serialization first, then normal properties read/write
	/// </summary>
	public interface IBeforeXmlNodeSerialize
	{
		void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node);
		void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node);
	}
	/// <summary>
	/// for cross-objects information passing
	/// After reading owners, some child objects with this interface need to get information other child objects 
	/// </summary>
	public interface IPostOwnersSerialize
	{
		void OnAfterReadOwners(IXmlCodeReader serializer, XmlNode node, object[] owners);
	}
	/// <summary>
	/// marked with ReadOnlyAttribute but still save it
	/// </summary>
	public class IgnoreReadOnlyAttribute : Attribute
	{
		public IgnoreReadOnlyAttribute()
		{
		}
	}
}
