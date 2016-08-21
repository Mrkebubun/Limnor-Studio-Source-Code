/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MathExp
{
	/// <summary>
	/// at design time, control/component is identified by name;
	/// at runtime, control/component is manually selected
	/// </summary>
	[TypeConverter(typeof(TypeConverterMathPropertyPointer))]
	public class MathPropertyPointer
	{
		const string XMLATT_typeName = "typename";
		const string XML_Child = "Child";
		protected const string XML_StrData = "StringData";
		protected const string XML_BinData = "BinData";
		protected const string XML_BinsData = "BinsData";
		private MathPropertyPointer _child;
		public MathPropertyPointer()
		{
		}
		public void ResetValue()
		{
		}
		public void setValue(object value)
		{
		}
		public override string ToString()
		{
			return Top.SaveToString();
		}
		public MathPropertyPointer Top
		{
			get
			{
				if (Parent != null)
				{
					return Parent.Top;
				}
				return this;
			}
		}
		public MathPropertyPointer Bottom
		{
			get
			{
				if (_child != null)
				{
					return _child.Bottom;
				}
				return this;
			}
		}
		public virtual string SaveToString()
		{
			if (!string.IsNullOrEmpty(PropertyName))
			{
				if (_child != null)
				{
					string s = _child.SaveToString();
					if (string.IsNullOrEmpty(s))
					{
						return PropertyName;
					}
					else
					{
						return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", PropertyName, s);
					}
				}
				else
				{
					return PropertyName;
				}
			}
			return string.Empty;
		}
		public void LoadFromString(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new MathException("cannot call LoadFromString with null");
			}
			data = data.Trim();
			if (string.IsNullOrEmpty(data))
			{
				throw new MathException("cannot call LoadFromString with empty data");
			}
			int pos = data.IndexOf('.');
			if (pos >= 0)
			{
				if (pos == 0)
				{
					throw new MathException("Invalid data [{0}] calling LoadFromString", data);
				}
				string s = data.Substring(0, pos);
				s = s.Trim();
				if (string.IsNullOrEmpty(s))
				{
					throw new MathException("Invalid data [{0}] calling LoadFromString. ", data);
				}
				PropertyName = s;
				data = data.Substring(pos + 1);
				data = data.Trim();
				if (!string.IsNullOrEmpty(data))
				{
					_child = new MathPropertyPointer();
					_child.LoadFromString(data);
				}
			}
			else
			{
				PropertyName = data;
			}
		}
		public virtual void SaveToXml(XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_typeName, PropertyName);
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			if (_child != null)
			{
				XmlNode childNode = XmlUtil.CreateSingleNewElement(node, XML_Child);
				_child.SaveToXml(childNode);
			}
		}
		public virtual void LoadFromXml(XmlNode node)
		{
			PropertyName = XmlUtil.GetAttribute(node, XMLATT_typeName);
			XmlNode childNode = node.SelectSingleNode(XML_Child);
			if (childNode != null)
			{
				Type t = XmlUtil.GetLibTypeAttribute(childNode);
				MathPropertyPointer cmp = (MathPropertyPointer)Activator.CreateInstance(t);
				Child = cmp;
				cmp.LoadFromXml(childNode);
			}
		}
		public virtual void GetInstance(object parent)
		{
			if (parent == null)
			{
				throw new MathException("parent cannot be null when calling GetInstance");
			}
			else if (!string.IsNullOrEmpty(PropertyName))
			{
				Form f = parent as Form;
				if (f != null)
				{
					for (int i = 0; i < f.Controls.Count; i++)
					{
						if (string.CompareOrdinal(PropertyName, f.Controls[i].Name) == 0)
						{
							Instance = f.Controls[i];
							break;
						}
					}
				}
				if (Instance == null)
				{
					Type t = parent.GetType();
					FieldInfo[] fifs = t.GetFields();
					if (fifs != null && fifs.Length > 0)
					{
						for (int i = 0; i < fifs.Length; i++)
						{
							if (string.CompareOrdinal(PropertyName, fifs[i].Name) == 0)
							{
								Instance = fifs[i].GetValue(parent);
								break;
							}
						}
					}
					if (Instance == null)
					{
						PropertyInfo[] pifs = t.GetProperties();
						if (pifs != null && pifs.Length > 0)
						{
							for (int i = 0; i < pifs.Length; i++)
							{
								if (string.CompareOrdinal(PropertyName, pifs[i].Name) == 0)
								{
									Instance = pifs[i].GetValue(parent, null);
									break;
								}
							}
						}
					}
				}
				if (Instance != null && _child != null)
				{
					_child.GetInstance(Instance);
				}
			}
		}
		public object InstancePointed
		{
			get
			{
				if (_child != null)
				{
					return _child.InstancePointed;
				}
				return Instance;
			}
		}
		private string _pn;
		private object _inst;
		/// <summary>
		/// the object instance
		/// </summary>
		public object Instance { get { return _inst; } set { _inst = value; } }
		/// <summary>
		/// the topmost level object is a Form. this PropertyName identifies the Instance of this level.
		/// 
		/// </summary>
		public string PropertyName { get { return _pn; } set { _pn = value; } }
		public MathPropertyPointer Child
		{
			get
			{
				return _child;
			}
			set
			{
				_child = value;
				_child.Parent = this;
			}
		}
		private MathPropertyPointer _p;
		public MathPropertyPointer Parent { get { return _p; } set { _p = value; } }
	}
	public class MathPropertyPointerConstant : MathPropertyPointer
	{
		public MathPropertyPointerConstant()
		{
		}
		public override void GetInstance(object parent)
		{
		}
		public override string SaveToString()
		{
			if (Instance != null)
			{
				return Instance.ToString();
			}
			return string.Empty;
		}
		public override void SaveToXml(XmlNode node)
		{
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			XmlNode ndData = null;
			if (Instance != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(Instance);
				Type conversionType = typeof(string);
				if (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType))
				{
					ndData = XmlUtil.CreateSingleNewElement(node, XML_StrData);
					ndData.RemoveAll();
					ndData.InnerText = converter.ConvertToInvariantString(Instance);
				}
				else
				{
					conversionType = typeof(byte[]);
					if (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType))
					{
						byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, Instance, typeof(byte[]));
						ndData = XmlUtil.CreateSingleNewElement(node, XML_BinData);
						ndData.RemoveAll();
						ndData.InnerText = Convert.ToBase64String(data);
					}
					else
					{
						if (Instance.GetType().IsSerializable)
						{
							BinaryFormatter formatter = new BinaryFormatter();
							MemoryStream stream = new MemoryStream();

							formatter.Serialize(stream, Instance);
							byte[] data = stream.ToArray();
							stream.Close();
							ndData = XmlUtil.CreateSingleNewElement(node, XML_BinsData);
							ndData.RemoveAll();
							ndData.InnerText = Convert.ToBase64String(data);
						}
						else
						{
							throw new MathException("Cannot save [{0}]", Instance.GetType().AssemblyQualifiedName);
						}
					}
				}
				if (ndData != null)
				{
					XmlUtil.SetLibTypeAttribute(ndData, Instance.GetType());
				}
			}
		}
		public override void LoadFromXml(XmlNode node)
		{
			TypeConverter converter;
			Type t;
			XmlNode ndData = node.SelectSingleNode(XML_StrData);
			if (ndData != null)
			{
				t = XmlUtil.GetLibTypeAttribute(ndData);
				converter = TypeDescriptor.GetConverter(t);
				Instance = converter.ConvertFromInvariantString(ndData.InnerText);
			}
			else
			{
				ndData = node.SelectSingleNode(XML_BinData);
				if (ndData != null)
				{
					t = XmlUtil.GetLibTypeAttribute(ndData);
					converter = TypeDescriptor.GetConverter(t);
					byte[] data = Convert.FromBase64String(ndData.InnerText);
					Instance = converter.ConvertFrom(null, CultureInfo.InvariantCulture, data);
				}
				else
				{
					ndData = node.SelectSingleNode(XML_BinsData);
					if (ndData != null)
					{
						byte[] data = Convert.FromBase64String(ndData.InnerText);
						BinaryFormatter formatter = new BinaryFormatter();
						MemoryStream stream = new MemoryStream(data);
						Instance = formatter.Deserialize(stream);
						stream.Close();
					}
				}
			}
		}
	}
	public class MathPropertyPointerForm : MathPropertyPointer
	{
		public MathPropertyPointerForm()
		{
		}
		public override void GetInstance(object parent)
		{
			if (parent == null || !(parent is Form))
			{
				throw new MathException("parent is not a Form");
			}
			Instance = parent;
			if (Child != null)
			{
				Child.GetInstance(parent);
			}
		}
	}
	public class MathPropertyPointerComponent : MathPropertyPointer
	{
		public MathPropertyPointerComponent()
		{
		}
		private bool onFindInterface(Type t, object v)
		{
			bool bRet = t.Equals(typeof(IComponent));
			return bRet;
		}
		public override void GetInstance(object parent)
		{
			if (!string.IsNullOrEmpty(PropertyName))
			{
				FieldInfo[] fifs = parent.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
				if (fifs != null && fifs.Length > 0)
				{
					for (int i = 0; i < fifs.Length; i++)
					{
						if (string.CompareOrdinal(PropertyName, fifs[i].Name) == 0)
						{
							Instance = fifs[i].GetValue(parent);
							break;
						}
					}
				}
				if (Child != null && Instance != null)
				{
					Child.GetInstance(Instance);
				}
			}
		}
	}
	public class MathPropertyPointerControl : MathPropertyPointer
	{
		public MathPropertyPointerControl()
		{
		}
		public override void GetInstance(object parent)
		{
			if (!string.IsNullOrEmpty(PropertyName))
			{
				Form f = (Form)parent;
				for (int i = 0; i < f.Controls.Count; i++)
				{
					if (string.CompareOrdinal(PropertyName, f.Controls[i].Name) == 0)
					{
						Instance = f.Controls[i];
						break;
					}
				}
				if (Child != null && Instance != null)
				{
					Child.GetInstance(Instance);
				}
			}
		}
	}
	public class MathPropertyPointerProperty : MathPropertyPointer
	{
		public MathPropertyPointerProperty()
		{
		}
		public override void GetInstance(object parent)
		{
			if (!string.IsNullOrEmpty(PropertyName))
			{
				PropertyInfo pif = parent.GetType().GetProperty(PropertyName);
				if (pif == null)
				{
					throw new MathException("Property [{0}] not found in [{1}]", PropertyName, parent.GetType().AssemblyQualifiedName);
				}
				Instance = pif.GetValue(parent, null);
				if (Instance != null && Child != null)
				{
					Child.GetInstance(Instance);
				}
			}
		}
	}
	public class MathPropertyPointerField : MathPropertyPointer
	{
		public MathPropertyPointerField()
		{
		}
		public override void GetInstance(object parent)
		{
			if (!string.IsNullOrEmpty(PropertyName))
			{
				FieldInfo pif = parent.GetType().GetField(PropertyName);
				if (pif == null)
				{
					throw new MathException("Field [{0}] not found in [{1}]", PropertyName, parent.GetType().AssemblyQualifiedName);
				}
				Instance = pif.GetValue(parent);
				if (Instance != null && Child != null)
				{
					Child.GetInstance(Instance);
				}
			}
		}
	}
}
