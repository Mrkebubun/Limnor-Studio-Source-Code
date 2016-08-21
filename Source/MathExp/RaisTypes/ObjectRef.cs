/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Xml;
using System.CodeDom;
using System.Reflection;
using XmlUtility;
using VPL;
using System.Collections.Specialized;
using System.Globalization;

namespace MathExp.RaisTypes
{
	public enum ObjectRefType { This, XPath, EventSender, Type, Field, Property, Const, Argv }
	/// <summary>
	/// <xs:complexType name="ObjectRef"><!-- reference to an object -->
	///   <xs:sequence>
	///     <xs:element name="Owner" type="ObjectRef" minOccurs="0" maxOccurs="1" />
	///     <xs:element name="Value" type="Data" minOccurs="0" maxOccurs="1" />
	///   </xs:sequence>
	///   <xs:attribute name="name" type="xs:string" use="optional" />
	///   <xs:attribute name="type">
	///     <xs:simpleType>
	///       <xs:restriction base="xs:string">
	///         <xs:enumeration value="This" /> <!-- compiling type; other parameters are ignored -->
	///         <xs:enumeration value="XPath" /> <!-- an XType object; name is an absolute XPath pointing to the object -->
	///         <xs:enumeration value="EventSender" /> <!-- the sender argument of an event, Value defines the type to cast -->
	///         <xs:enumeration value="Type" /> <!-- reference to a library type. the Value defines the type -->
	///         <xs:enumeration value="Field" /> <!-- Owner is the object owing the field; name is the field name; _value is the type and value -->
	///         <xs:enumeration value="Property" /> <!-- Owner is the object owing the property; name is the property name; _value is the type and value -->
	///         <xs:enumeration value="Const" /> <!-- a constant. Value holds the data -->
	///         <xs:enumeration value="Argv" /> <!-- method argument. name is the argument name; other parameters are ignored -->
	///       </xs:restriction>
	///     </xs:simpleType>
	///   </xs:attribute>
	/// </xs:complexType>
	/// </summary>
	public class ObjectRef : IXmlNodeSerializable, ICloneable
	{
		#region fields and constructors
		private ObjectRef _owner;
		private Data _value;
		private ObjectRefType _type;
		private string _name;
		private XmlNode _xpathNode;
		public ObjectRef(XmlNode node)
		{
			_xpathNode = node;
		}
		#endregion
		#region properties
		public string TypeString
		{
			get
			{
				if (_xpathNode == null)
					throw new MathException("Accessing TypeString without xmlNode assigned");
				if (_type == ObjectRefType.XPath)
				{
					XmlNode node = XmlSerialization.GetXmlNodeByPath(_xpathNode.OwnerDocument, _name);
					return XmlSerialization.GetComponentFullTypeName(node);
				}
				if (_type == ObjectRefType.Type)
				{
					if (_value == null)
						throw new MathException("Accessing TypeString without Value assigned as a library type");
					return _value.LibType.FullName;
				}
				if (_type == ObjectRefType.Property || _type == ObjectRefType.Field)
				{
					if (Owner == null)
					{
						throw new MathException("Accessing TypeString without Owner value");
					}
					return Owner.TypeString + "." + _name;
				}
				throw new MathException(XmlSerialization.FormatString("Accessing TypeString: type not implemented: {0}", _type));
			}
		}
		public ObjectRef Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
		public Data Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		public ObjectRefType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		public string localName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
					return "";
				return _name;
			}
		}
		public string Name
		{
			get
			{
				if (_type == ObjectRefType.Type)
					return _value.LibType.Name;
				if (_type == ObjectRefType.This)
					return "this";
				if (_type == ObjectRefType.Property || _type == ObjectRefType.Field)
				{
					if (_owner == null)
					{
						return localName;
					}
					if (string.IsNullOrEmpty(_owner.Name))
					{
						return localName;
					}
					return _owner.Name + "." + localName;
				}
				if (_type == ObjectRefType.Const)
				{
					return _value.ToString();
				}
				if (_type == ObjectRefType.XPath)
				{
					if (_xpathNode != null)
					{
						return XmlSerialization.GetAttribute(_xpathNode, XmlSerialization.XMLATT_NAME);
					}
				}
				return localName;
			}
			set
			{
				_name = value;
			}
		}
		public UInt32 ID
		{
			get
			{
				return XmlSerialization.GetNodeID(_xpathNode);
			}
		}
		public Type DataType
		{
			get
			{
				switch (_type)
				{
					case ObjectRefType.This:
						return XmlSerialization.GetObjectLibType(_xpathNode);
					case ObjectRefType.Type:
						return _value.LibType;
					case ObjectRefType.Field:
						return _value.DataType.Type;
					case ObjectRefType.Property:
						if (Owner == null)
							throw new MathException(XmlSerialization.FormatString("ObjectRef.DataType: Owner is null for {0}", this.Name));
						if (_xpathNode == null)
							throw new MathException(XmlSerialization.FormatString("ObjectRef.DataType: _xpathNode is null for {0}", this.Name));
						XmlNode nd = XmlSerialization.GetXmlNodeByPath(_xpathNode.OwnerDocument, Owner.XPath);
						string qry = XmlSerialization.FormatString("{0}[@{1}='{2}']",
							XmlSerialization.XML_PROPERTY, XmlSerialization.XMLATT_NAME, _name);
						XmlNode ndProp = nd.SelectSingleNode(qry);
						if (ndProp == null)
						{
							Type t = XmlSerialization.GetObjectLibType(nd);
							if (string.CompareOrdinal(_name, "ObjectValue") == 0 && string.CompareOrdinal(t.Name, "Class") == 0)
							{
								string sQry = XmlSerialization.FormatString("{0}/{1}[@{2}='ValueType']",
									XmlSerialization.RAIS_B,
									XmlSerialization.XML_PROPERTY, XmlSerialization.XMLATT_NAME, _name);
								ndProp = nd.SelectSingleNode(sQry);
								if (ndProp != null)
								{
									return System.Type.GetType(ndProp.InnerText);
								}
							}
							else
							{
								PropertyInfo pif = t.GetProperty(_name);
								if (pif != null)
								{
									return pif.PropertyType;
								}
							}
							throw new MathException(XmlSerialization.FormatString("ObjectRef.DataType: property node not found for {0} using {1}", this.Name, qry));
						}
						XmlNode nType = ndProp.SelectSingleNode(XmlSerialization.XML_TYPE);
						if (nType != null)
						{
							ObjectRef oP = new ObjectRef(nType);
							oP.OnReadFromXmlNode(null, nType);
							return oP.DataType;
						}
						else
						{
							nType = ndProp.SelectSingleNode(XmlSerialization.LIBTYPE);
							return XmlUtil.GetLibTypeAttribute(nType);
						}
					case ObjectRefType.XPath:
						if (_xpathNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("Missing _xpathNode for type resolving for {0}", _name));
						}
						XmlNode node = XmlSerialization.GetXmlNodeByPath(_xpathNode.OwnerDocument, _name);
						if (node == null)
						{
							throw new MathException(XmlSerialization.FormatString("Invalid xpath {0} for type resolving", _name));
						}
						return XmlSerialization.GetObjectLibType(node);
				}
				throw new MathException(XmlSerialization.FormatString("Type resolving not implemented for {0}", _type));
			}
		}
		public string XPath
		{
			get
			{
				if (_type == ObjectRefType.XPath)
					return _name;
				return "";
			}
		}
		/// <summary>
		/// determine whether the referenced object needs to be compiled
		/// </summary>
		public bool NeedCompile
		{
			get
			{
				switch (this.Type)
				{
					case ObjectRefType.EventSender:
						return true;
					case ObjectRefType.Field:
					case ObjectRefType.Property:
						return Owner.NeedCompile;
					case ObjectRefType.This:
						return true;
					case ObjectRefType.XPath:
						return true;
				}
				return false;
			}
		}
		#endregion
		#region methods
		/// <summary>
		/// when it is used as a data type, determine whether two objects are of the same type
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsSameType(ObjectRef obj)
		{
			if (this.Type == obj.Type)
			{
				switch (this.Type)
				{
					case ObjectRefType.Argv:
						return (this.localName == obj.localName);
					case ObjectRefType.XPath:
						return (this.localName == obj.localName);
				}
			}
			return false;
		}
		public bool IsStatic
		{
			get
			{
				if (this.Type == ObjectRefType.XPath)
				{
					if (_xpathNode == null)
						throw new MathException(XmlSerialization.FormatString("Accessing ObjectRef.IsStatic for {0}: _xpathNode is null", this.Name));
					XmlNode node = XmlSerialization.GetRootComponent(_xpathNode);
					if (XmlSerialization.GetName(node) == XmlSerialization.STARTERCLASS)
						return true;
					return XmlSerialization.GetAttributeBool(node, XmlSerialization.XMLATT_STATIC);
				}
				throw new MathException(XmlSerialization.FormatString("Accessing ObjectRef.IsStatic: not implemented for type {0}", this.Type));
			}
		}
		/// <summary>
		/// generate reference code
		/// </summary>
		/// <param name="methodOwnerXPath"></param>
		/// <returns></returns>
		public CodeExpression ExportCode(string methodOwnerXPath)
		{
			MathNode.Trace("ObjectRef.ExportCode for {0}", this);
			switch (Type)
			{
				case ObjectRefType.Argv:
				case ObjectRefType.EventSender:
					return new CodeArgumentReferenceExpression(_name);
				case ObjectRefType.This:
					return new CodeThisReferenceExpression();
				case ObjectRefType.Type:
					return new CodeTypeReferenceExpression(DataType);
				case ObjectRefType.Field:
				case ObjectRefType.Property:
					if (Owner == null)
					{
						throw new MathException("property reference does not have an owner");
					}
					return new CodePropertyReferenceExpression(Owner.ExportCode(methodOwnerXPath), _name);
				case ObjectRefType.Const:
					if (_value == null)
						return new CodePrimitiveExpression(null);
					else
						return ObjectCreationCodeGen.ObjectCreationCode(_value.DataValue);
				case ObjectRefType.XPath:
					if (_xpathNode == null)
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath Node");
					}
					if (string.IsNullOrEmpty(_name))
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath value");
					}
					CodeExpression codeExp = new CodeThisReferenceExpression();
					if (methodOwnerXPath != _name)
					{
						//object for the reference
						//_name is the xpath
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.ExportCode is called for XPath with invalid xpath value: {0}", _name));
						}
						//
						if (thisNode.Name == XmlSerialization.RAIS_R)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = new CodeTypeReferenceExpression(TypeString);
							}
							else
							{
								//find all names for each part R|A[@ID='?']R|A[@ID='?']...
								string xpath0 = _name;
								string name = XmlSerialization.GetName(thisNode);
								while (xpath0.Length > 0 && xpath0 != methodOwnerXPath)
								{
									int pos = xpath0.LastIndexOf('/');
									if (pos > 0)
									{
										xpath0 = xpath0.Substring(0, pos);
										if (xpath0 != methodOwnerXPath)
										{
											XmlNode n0 = XmlSerialization.GetXmlNodeByPath(thisNode.OwnerDocument, xpath0);
											if (n0 == null)
											{
												throw new MathException(XmlSerialization.FormatString("Invalid path {0}", xpath0));
											}
											name = XmlSerialization.GetName(n0) + "." + name;
										}
									}
									else
									{
										break;
									}
								}
								codeExp = new CodeFieldReferenceExpression(codeExp, name);
							}
						}
					}
					else
					{
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.ExportCode is called for XPath with invalid xpath value: {0}", _name));
						}
						//use the parent node to generate code
						if (thisNode.Name == XmlSerialization.RAIS_R)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = new CodeTypeReferenceExpression(TypeString);
							}
						}
					}
					return codeExp;
			}
			return new CodeThisReferenceExpression();
		}
		public string CreateJavaScript(StringCollection method, string methodOwnerXPath)
		{
			MathNode.Trace("ObjectRef.CreateJavaScript for {0}", this);
			switch (Type)
			{
				case ObjectRefType.Argv:
				case ObjectRefType.EventSender:
					return _name;
				case ObjectRefType.This:
					return "window";
				case ObjectRefType.Type:
					return DataType.FullName;
				case ObjectRefType.Field:
				case ObjectRefType.Property:
					if (Owner == null)
					{
						throw new MathException("property reference does not have an owner");
					}
					return MathNode.FormString("{0}.{1}", Owner.CreateJavaScript(method, methodOwnerXPath), _name);
				case ObjectRefType.Const:
					if (_value == null)
						return "null";
					else
						return _value.DataValue.ToString();
				case ObjectRefType.XPath:
					if (_xpathNode == null)
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath Node");
					}
					if (string.IsNullOrEmpty(_name))
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath value");
					}
					string codeExp = "window";
					if (string.CompareOrdinal(methodOwnerXPath, _name) != 0)
					{
						//object for the reference
						//_name is the xpath
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.ExportCode is called for XPath with invalid xpath value: {0}", _name));
						}
						//
						if (string.CompareOrdinal(thisNode.Name, XmlSerialization.RAIS_R) == 0)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = TypeString;
							}
							else
							{
								//find all names for each part R|A[@ID='?']R|A[@ID='?']...
								string xpath0 = _name;
								string name = XmlSerialization.GetName(thisNode);
								while (xpath0.Length > 0 && xpath0 != methodOwnerXPath)
								{
									int pos = xpath0.LastIndexOf('/');
									if (pos > 0)
									{
										xpath0 = xpath0.Substring(0, pos);
										if (xpath0 != methodOwnerXPath)
										{
											XmlNode n0 = XmlSerialization.GetXmlNodeByPath(thisNode.OwnerDocument, xpath0);
											if (n0 == null)
											{
												throw new MathException(XmlSerialization.FormatString("Invalid path {0}", xpath0));
											}
											name = XmlSerialization.GetName(n0) + "." + name;
										}
									}
									else
									{
										break;
									}
								}
								codeExp = MathNode.FormString("{0}.{1}", codeExp, name);
							}
						}
					}
					else
					{
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.CreateJavaScript is called for XPath with invalid xpath value: {0}", _name));
						}
						//use the parent node to generate code
						if (thisNode.Name == XmlSerialization.RAIS_R)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = TypeString;
							}
						}
					}
					return codeExp;
			}
			return "window";
		}
		public string CreatePhpScript(StringCollection method, string methodOwnerXPath)
		{
			MathNode.Trace("ObjectRef.CreatePhpScript for {0}", this);
			switch (Type)
			{
				case ObjectRefType.Argv:
				case ObjectRefType.EventSender:
					return string.Format(CultureInfo.InvariantCulture, "${0}", _name);
				case ObjectRefType.This:
					return "$this";
				case ObjectRefType.Type:
					return DataType.FullName;
				case ObjectRefType.Field:
				case ObjectRefType.Property:
					if (Owner == null)
					{
						throw new MathException("property reference does not have an owner");
					}
					return MathNode.FormString("{0}->{1}", Owner.CreatePhpScript(method, methodOwnerXPath), _name);
				case ObjectRefType.Const:
					if (_value == null)
						return "NULL";
					else
						return _value.DataValue.ToString();
				case ObjectRefType.XPath:
					if (_xpathNode == null)
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath Node");
					}
					if (string.IsNullOrEmpty(_name))
					{
						throw new MathException("ObjectRef.ExportCode is called for XPath with null xpath value");
					}
					string codeExp = "$this";
					if (string.CompareOrdinal(methodOwnerXPath, _name) != 0)
					{
						//object for the reference
						//_name is the xpath
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.ExportCode is called for XPath with invalid xpath value: {0}", _name));
						}
						//
						if (string.CompareOrdinal(thisNode.Name, XmlSerialization.RAIS_R) == 0)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = TypeString;
							}
							else
							{
								//find all names for each part R|A[@ID='?']R|A[@ID='?']...
								string xpath0 = _name;
								string name = XmlSerialization.GetName(thisNode);
								while (xpath0.Length > 0 && xpath0 != methodOwnerXPath)
								{
									int pos = xpath0.LastIndexOf('/');
									if (pos > 0)
									{
										xpath0 = xpath0.Substring(0, pos);
										if (xpath0 != methodOwnerXPath)
										{
											XmlNode n0 = XmlSerialization.GetXmlNodeByPath(thisNode.OwnerDocument, xpath0);
											if (n0 == null)
											{
												throw new MathException(XmlSerialization.FormatString("Invalid path {0}", xpath0));
											}
											name = MathNode.FormString("{0}->{1}", XmlSerialization.GetName(n0), name);
										}
									}
									else
									{
										break;
									}
								}
								codeExp = MathNode.FormString("{0}->{1}", codeExp, name);
							}
						}
					}
					else
					{
						XmlNode thisNode = _xpathNode.OwnerDocument.DocumentElement.SelectSingleNode(_name);
						if (thisNode == null)
						{
							throw new MathException(XmlSerialization.FormatString("ObjectRef.CreateJavaScript is called for XPath with invalid xpath value: {0}", _name));
						}
						//use the parent node to generate code
						if (thisNode.Name == XmlSerialization.RAIS_R)
						{
							if (XmlSerialization.IsStaticComponent(thisNode))
							{
								codeExp = TypeString;
							}
						}
					}
					return codeExp;
			}
			return "$this";
		}
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder("");
			switch (_type)
			{
				case ObjectRefType.Argv:
					sb.Append(_name);
					break;
				case ObjectRefType.Const:
					if (_value == null)
						sb.Append("null");
					else
						sb.Append(_value.ToString());
					break;
				case ObjectRefType.EventSender:
					if (_value == null)
						sb.Append("null");
					else
						sb.Append(_value.ToString());
					sb.Append(" ");
					sb.Append(_name);
					break;
				case ObjectRefType.Field:
				case ObjectRefType.Property:
					sb.Append(_type.ToString());
					sb.Append(" ");
					if (_owner == null)
						sb.Append("null ");
					else
					{
						sb.Append(_owner.ToString());
						sb.Append(".");
					}
					sb.Append(_name);
					break;
				case ObjectRefType.This:
					sb.Append(_type.ToString());
					break;
				case ObjectRefType.Type:
					if (_value == null)
						sb.Append("null");
					else
						sb.Append(_value.ToString());
					break;
				case ObjectRefType.XPath:
					if (_xpathNode == null)
						sb.Append(_name);
					else
					{
						string s = XmlSerialization.GetAttribute(_xpathNode, XmlSerialization.XMLATT_NAME);
						if (string.IsNullOrEmpty(s))
						{
							s = _xpathNode.Name;
						}
						sb.Append(s);
					}
					break;
			}
			return sb.ToString();
		}
		public void SetXPathNode(XmlNode node)
		{
			_xpathNode = node;
		}
		public XmlDocument GetXmlDocument()
		{
			if (_xpathNode != null)
			{
				return _xpathNode.OwnerDocument;
			}
			return null;
		}
		public XmlNode GetXmlNode()
		{
			return _xpathNode;
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XMLATT_ObjRefType = "refType";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_xpathNode = node;
			_name = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
			_type = (ObjectRefType)XmlSerialization.GetAttributeEnum(node, XMLATT_ObjRefType, typeof(ObjectRefType));
			if (_type == ObjectRefType.XPath)
			{
				if (!string.IsNullOrEmpty(_name))
				{
					_xpathNode = XmlSerialization.GetXmlNodeByPath(node.OwnerDocument, _name);
					if (_xpathNode == null)
					{
						throw new MathException(string.Format("XPath with an invalid path value {0} for {1}", _name, node.OwnerDocument.DocumentElement.Name));
					}
				}
			}
			else
			{
				_owner = (ObjectRef)XmlSerialization.ReadFromChildXmlNode(serializer, node, "Owner", new object[] { null });
				_value = (Data)XmlSerialization.ReadFromChildXmlNode(serializer, node, "Value");
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_owner != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, "Owner", _owner);
			}
			if (_value != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, "Value", _value);
			}
			if (!string.IsNullOrEmpty(_name))
			{
				XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, _name);
			}
			XmlSerialization.SetAttribute(node, XMLATT_ObjRefType, _type.ToString());
		}

		#endregion
		#region ICloneable Members

		public object Clone()
		{
			ObjectRef obj = new ObjectRef(_xpathNode);
			obj.Type = _type;
			obj.Name = _name;
			if (_owner != null)
			{
				obj.Owner = (ObjectRef)_owner.Clone();
			}
			if (_value != null)
			{
				obj.Value = (Data)_value.Clone();
			}
			return obj;
		}

		#endregion
	}
}
