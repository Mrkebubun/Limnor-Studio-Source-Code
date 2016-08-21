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
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using VPL;

namespace MathExp.RaisTypes
{
	/// <summary>
	/// reference a method, not defining it.
	/// </summary>
	public class MethodRef : IXmlNodeSerializable
	{
		//these two fields identify the method to be called
		private ObjectRef _methodOwnerObjectRef;
		private string _methodName;
		//for a customer method the method id uniquely identifies it
		private int _methodId;
		//these two fields come from the method.
		//they should be set when the method is selected.
		//they are saved
		private ParameterDef[] _parameters;
		private RaisDataType _retType;
		public MethodRef()
		{
		}
		public MethodRef(ObjectRef owner, string name, int id, ParameterDef[] parameters, RaisDataType returnType)
		{
			_methodOwnerObjectRef = owner;
			_methodName = name;
			_methodId = id;
			_parameters = parameters;
			_retType = returnType;
		}
		public override string ToString()
		{
			if (_methodName == XmlSerialization.CONSTRUCTOR_METHOD)
				return "new " + MethodOwnerName;
			return MethodOwnerName + "." + MethodName;
		}
		[Browsable(false)]
		public int MethodID
		{
			get
			{
				return _methodId;
			}
			set
			{
				_methodId = value;
			}
		}
		[Browsable(false)]
		public ObjectRef MethodOwner
		{
			get
			{
				return _methodOwnerObjectRef;
			}
			set
			{
				_methodOwnerObjectRef = value;
			}
		}
		public string MethodOwnerName
		{
			get
			{
				if (_methodOwnerObjectRef != null)
					return _methodOwnerObjectRef.Name;
				return "";
			}
		}
		public string MethodName
		{
			get
			{
				if (string.IsNullOrEmpty(_methodName))
					return "";
				return _methodName;
			}
			set
			{
				_methodName = value;
			}
		}
		public int ParameterCount
		{
			get
			{
				if (_parameters == null)
					return 0;
				return _parameters.Length;
			}
		}
		[ReadOnly(true)]
		public ParameterDef[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}
		[Browsable(false)]
		public RaisDataType ReturnType
		{
			get
			{
				if (_retType == null)
					_retType = new RaisDataType(typeof(void));
				return _retType;
			}
			set
			{
				_retType = value;
			}
		}
		/// <summary>
		/// find out whether this type is a static method
		/// </summary>
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (string.IsNullOrEmpty(_methodName))
					throw new MathException("Accessing IsStatic with empty method name");
				if (_methodOwnerObjectRef == null)
					throw new MathException("Accessing IsStatic with null owner");

				if (_methodOwnerObjectRef.Type == ObjectRefType.Type)
					return true;
				if (_methodOwnerObjectRef.Type == ObjectRefType.Field || _methodOwnerObjectRef.Type == ObjectRefType.Property)
				{
					if (_methodOwnerObjectRef.Owner.Type == ObjectRefType.XPath)
					{
						return _methodOwnerObjectRef.Owner.IsStatic;
					}

					throw new MathException(XmlSerialization.FormatString("Accessing IsStatic for Field. Owner type {0} not implemented", _methodOwnerObjectRef.Owner.Type));
				}
				if (_methodOwnerObjectRef.Type == ObjectRefType.XPath)
				{
					XmlDocument doc = _methodOwnerObjectRef.GetXmlDocument();
					if (doc == null)
						throw new MathException("Accessing IsStatic with null document");
					XmlNode prj = XmlSerialization.GetProjectNode(doc);
					if (prj == null)
						throw new MathException("Accessing IsStatic with null project");
					XmlNode ownNode = prj.SelectSingleNode(_methodOwnerObjectRef.XPath);
					if (ownNode == null)
						throw new MathException(XmlSerialization.FormatString("Accessing IsStatic with invalid xpath {0}", _methodOwnerObjectRef.XPath));
					if (_methodName == XmlSerialization.CONSTRUCTOR_METHOD)
						return true;
					XmlNode methodNode = ownNode.SelectSingleNode(XmlSerialization.FormatString("{0}[@{1}='{2}']",
						XmlSerialization.XML_METHOD, XmlSerialization.XMLATT_NAME, _methodName));
					if (methodNode == null)
					{
						Type t = XmlSerialization.GetObjectLibType(ownNode);
						if (t == null)
							throw new MathException(XmlSerialization.FormatString("Accessing IsStatic with valid xpath {0} but method name {1} not found and owner library type not found", _methodOwnerObjectRef.XPath, _methodName));
						MethodInfo mif = t.GetMethod(_methodName);
						if (mif == null)
							throw new MathException(XmlSerialization.FormatString("Accessing IsStatic with valid xpath {0} but method name {1} not found in owner library type {2}", _methodOwnerObjectRef.XPath, _methodName, t.Name));
						return mif.IsStatic;
					}
					else
					{
						return XmlSerialization.GetAttributeBool(methodNode, XmlSerialization.XMLATT_STATIC);
					}
				}
				throw new MathException(XmlSerialization.FormatString("Accessing IsStatic: type not implemented: {0}", _methodOwnerObjectRef.Type));
			}
		}
		private void getSignature(MethodInfo mi)
		{
			_retType = new RaisDataType(mi.ReturnType);
			ParameterInfo[] ps = mi.GetParameters();
			if (ps != null && ps.Length > 0)
			{
				_parameters = new ParameterDef[ps.Length];
				for (int i = 0; i < ps.Length; i++)
				{
					_parameters[i] = new ParameterDef(ps[i].ParameterType, ps[i].Name);
				}
			}
		}
		/// <summary>
		/// retrieve _retyrnType and parameters
		/// </summary>
		private void getSignature()
		{
			if (string.IsNullOrEmpty(_methodName))
				throw new MathException("Accessing getSignature with empty method name");
			if (_methodOwnerObjectRef == null)
				throw new MathException("Accessing getSignature with null owner");

			if (_methodOwnerObjectRef.Type == ObjectRefType.Type)
			{
				Type t = _methodOwnerObjectRef.Value.LibType;
				MethodInfo mi = t.GetMethod(_methodName);
				if (mi == null)
				{
					throw new MathException("Accessing getSignature with invalid method name '{0}'", _methodName);
				}
				getSignature(mi);
			}
			else if (_methodOwnerObjectRef.Type == ObjectRefType.Field || _methodOwnerObjectRef.Type == ObjectRefType.Property)
			{
				if (_methodOwnerObjectRef.Owner.Type == ObjectRefType.XPath)
				{

				}
				else if (_methodOwnerObjectRef.Owner.Type == ObjectRefType.Type)
				{
					MethodInfo mi = null;
					Type t = _methodOwnerObjectRef.Owner.Value.LibType;
					if (_methodOwnerObjectRef.Type == ObjectRefType.Field)
					{
						FieldInfo fi = t.GetField(_methodOwnerObjectRef.Name);
						if (fi == null)
						{
							throw new MathException(XmlSerialization.FormatString("Accessing getSignature for Field with invalid field name {0} for type {1}", _methodOwnerObjectRef.Name, t));
						}
						mi = fi.FieldType.GetMethod(_methodName);
						if (mi == null)
						{
							throw new MathException(XmlSerialization.FormatString("Accessing getSignature for Field with invalid method name {0} for field name {1} and type {2}", _methodName, _methodOwnerObjectRef.Name, t));
						}
					}
					else if (_methodOwnerObjectRef.Type == ObjectRefType.Property)
					{
						PropertyInfo pi = t.GetProperty(_methodOwnerObjectRef.Name);
						if (pi == null)
						{
							throw new MathException(XmlSerialization.FormatString("Accessing getSignature for Property with invalid property name {0} for type {1}", _methodOwnerObjectRef.Name, t));
						}
						mi = pi.PropertyType.GetMethod(_methodName);
						if (mi == null)
						{
							throw new MathException(XmlSerialization.FormatString("Accessing getSignature for Property with invalid method name {0} for Property name {1} and type {2}", _methodName, _methodOwnerObjectRef.Name, t));
						}
					}
					getSignature(mi);
				}
				throw new MathException(XmlSerialization.FormatString("Accessing getSignature for Field. Owner type {0} not implemented", _methodOwnerObjectRef.Owner.Type));
			}
			else if (_methodOwnerObjectRef.Type == ObjectRefType.XPath)
			{
				XmlDocument doc = _methodOwnerObjectRef.GetXmlDocument();
				if (doc == null)
					throw new MathException("Accessing getSignature with null document");
				XmlNode prj = XmlSerialization.GetProjectNode(doc);
				if (prj == null)
					throw new MathException("Accessing getSignature with null project");
				XmlNode ownNode = prj.SelectSingleNode(_methodOwnerObjectRef.XPath);
				if (ownNode == null)
					throw new MathException(XmlSerialization.FormatString("Accessing getSignature with invalid xpath {0}", _methodOwnerObjectRef.XPath));
				if (_methodName == XmlSerialization.CONSTRUCTOR_METHOD)
				{
					throw new MathException("Accessing getSignature when the method is a constructor");
				}
				XmlNode methodNode = ownNode.SelectSingleNode(XmlSerialization.FormatString("{0}[@{1}='{2}']",
					XmlSerialization.XML_METHOD, XmlSerialization.XMLATT_NAME, _methodName));
				if (methodNode == null)
				{
					Type t = XmlSerialization.GetObjectLibType(ownNode);
					if (t == null)
						throw new MathException(XmlSerialization.FormatString("Accessing getSignature with valid xpath {0} but method name {1} not found and owner library type not found", _methodOwnerObjectRef.XPath, _methodName));
					MethodInfo mif = t.GetMethod(_methodName);
					if (mif == null)
						throw new MathException(XmlSerialization.FormatString("Accessing getSignature with valid xpath {0} but method name {1} not found in owner library type {2}", _methodOwnerObjectRef.XPath, _methodName, t.Name));
					getSignature(mif);
				}
				else
				{
					MethodType mt = new MethodType();
					mt.OnReadFromXmlNode(null, methodNode);
					_retType = mt.ReturnType;
					Parameter[] ps = mt.Parameters;
					if (ps != null && ps.Length > 0)
					{
						_parameters = new ParameterDef[ps.Length];
						for (int i = 0; i < ps.Length; i++)
						{
							_parameters[i] = new ParameterDef(ps[i]);
						}
					}
				}
			}
			throw new MathException(XmlSerialization.FormatString("Accessing getSignature: type not implemented: {0}", _methodOwnerObjectRef.Type));
		}
		#region IXmlNodeSerializable Members
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode methodXmlNode = null;
			_methodName = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
			_methodId = XmlSerialization.GetAttributeInt(node, XmlSerialization.XMLATT_ID);
			_methodOwnerObjectRef = (ObjectRef)XmlSerialization.ReadFromChildXmlNode(serializer, node, XmlSerialization.XML_METHODOWNER, new object[] { null });
			if (_methodId != 0)
			{
				methodXmlNode = node.OwnerDocument.SelectSingleNode(
					XmlSerialization.FormatString("//{0}[@{1}='{2}']", XmlSerialization.XML_METHOD, XmlSerialization.XMLATT_ID, _methodId));
			}
			if (methodXmlNode == null)
			{
				if (_methodId != 0)
				{
					throw new MathException("Method {0} not found", _methodId);
				}
				_retType = (RaisDataType)XmlSerialization.ReadFromChildXmlNode(serializer, node, XmlSerialization.XML_RETURNTYPE);
				XmlNodeList pNodes = node.SelectNodes(XmlSerialization.XML_PARAM);
				if (pNodes != null)
				{
					_parameters = new ParameterDef[pNodes.Count];
					for (int i = 0; i < pNodes.Count; i++)
					{
						_parameters[i] = (ParameterDef)XmlSerialization.ReadFromXmlNode(serializer, pNodes[i]);
					}
				}
			}
			else
			{
				MethodType mt = new MethodType();
				mt.OnReadFromXmlNode(serializer, methodXmlNode);
				_retType = mt.ReturnType;
				Parameter[] ps = mt.Parameters;
				if (ps != null && ps.Length > 0)
				{
					_parameters = new ParameterDef[ps.Length];
					for (int i = 0; i < ps.Length; i++)
					{
						_parameters[i] = new ParameterDef(ps[i], ps[i].Name);
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, _methodName);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_ID, _methodId);
			XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_METHODOWNER, _methodOwnerObjectRef);
			if (_methodId == 0)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_RETURNTYPE, _retType);
				if (_parameters != null)
				{
					for (int i = 0; i < _parameters.Length; i++)
					{
						XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_PARAM, _parameters[i]);
					}
				}
			}
		}

		#endregion
	}
}
