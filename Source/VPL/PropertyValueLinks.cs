/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Globalization;

namespace VPL
{
	class PropertyValueLinker
	{
		public PropertyValueLinker()
		{
		}
		public bool UserRuntimeValue { get; set; }
		public object RuntimeValue { get; set; }
		/// <summary>
		/// runtime accessor compiled from Linker
		/// </summary>
		public fnGetPropertyValue Getter { get; set; }
		/// <summary>
		/// designtime setting
		/// </summary>
		public IPropertyValueLink Linker { get; set; }
	}
	public class PropertyValueLinks
	{
		#region fields and constructors
		private Dictionary<string, PropertyValueLinker> _properties;
		private IPropertyValueLinkHolder _holder;
		public PropertyValueLinks(IPropertyValueLinkHolder holder, params string[] names)
		{
			_holder = holder;
			_properties = new Dictionary<string, PropertyValueLinker>();
			if (names != null && names.Length > 0)
			{
				for (int i = 0; i < names.Length; i++)
				{
					_properties.Add(names[i], new PropertyValueLinker());
				}
			}
		}
		#endregion
		#region Methods
		public void Clear()
		{
			_properties = new Dictionary<string, PropertyValueLinker>();
		}
		public void AdjustNames(string[] names)
		{
			if (names == null || names.Length == 0)
			{
				_properties = new Dictionary<string, PropertyValueLinker>();
			}
			else
			{
				StringCollection sc = new StringCollection();
				foreach (string s in _properties.Keys)
				{
					bool bExist = false;
					for (int i = 0; i < names.Length; i++)
					{
						if (string.CompareOrdinal(names[i], s) == 0)
						{
							bExist = true;
							break;
						}
					}
					if (!bExist)
					{
						sc.Add(s);
					}
				}
				foreach (string s in sc)
				{
					_properties.Remove(s);
				}
				for (int i = 0; i < names.Length; i++)
				{
					if (!_properties.ContainsKey(names[i]))
					{
						_properties.Add(names[i], new PropertyValueLinker());
					}
				}
			}
		}
		public void AddName(string name)
		{
			if (!_properties.ContainsKey(name))
			{
				_properties.Add(name, new PropertyValueLinker());
			}
		}
		public IPropertyValueLinkHolder Holder
		{
			get
			{
				return _holder;
			}
		}
		public void CopyData(PropertyValueLinks pls)
		{
			foreach (KeyValuePair<string, PropertyValueLinker> kv in pls._properties)
			{
				_properties.Add(kv.Key, kv.Value);
			}
		}
		public object GetValue(string name)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				if (pvl.UserRuntimeValue)
				{
					return pvl.RuntimeValue;
				}
				if (pvl.Getter != null)
				{
					return pvl.Getter();
				}
				if (pvl.Linker != null)
				{
					return pvl.Linker.GetConstValue();
				}
			}
			return null;
		}
		public void SetRuntimeValue(string name, object value)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				System.Reflection.Missing mv = value as System.Reflection.Missing;
				if (mv != null)
				{
					pvl.UserRuntimeValue = false;
				}
				else
				{
					pvl.UserRuntimeValue = true;
					pvl.RuntimeValue = value;
				}
			}
		}
		public Int16 ValueInt16(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToInt16(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public Int32 ValueInt32(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToInt32(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public Int64 ValueInt64(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToInt64(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public sbyte ValueSByte(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToSByte(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public UInt64 ValueUInt64(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToUInt64(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public UInt32 ValueUInt32(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToUInt32(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public UInt16 ValueUInt16(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToUInt16(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public byte ValueByte(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToByte(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public bool ValueBool(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToBoolean(v, CultureInfo.InvariantCulture);
			}
			return false;
		}
		public char ValueChar(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToChar(v, CultureInfo.InvariantCulture);
			}
			return '\0';
		}
		public double ValueDouble(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToDouble(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public DateTime ValueDateTime(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToDateTime(v, CultureInfo.InvariantCulture);
			}
			return DateTime.MinValue;
		}
		public float ValueFloat(string name)
		{
			object v = GetValue(name);
			if (v != null)
			{
				return Convert.ToSingle(v, CultureInfo.InvariantCulture);
			}
			return 0;
		}
		public void SetConstValue(string name, object value)
		{
			IPropertyValueLink lk = GetValueLink(name);
			if (lk != null)
			{
				lk.SetConstValue(value);
			}
		}
		public IPropertyValueLink GetValueLink(string name)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				if (pvl.Linker == null && VPLUtil.PropertyValueLinkType != null)
				{
					Type valueType = null;
					PropertyInfo pif = _holder.GetType().GetProperty(name);
					if (pif != null)
					{
						valueType = pif.PropertyType;
					}
					else
					{
						ICustomPropertyCollection cpc = _holder as ICustomPropertyCollection;
						if (cpc != null)
						{
							valueType = cpc.GetCustomPropertyType(name);
						}
						if (valueType == null)
						{
							valueType = typeof(object);
						}
					}
					pvl.Linker = Activator.CreateInstance(VPLUtil.PropertyValueLinkType, _holder, name) as IPropertyValueLink;
					pvl.Linker.SetDataType(valueType);
				}
				return pvl.Linker;
			}
			return null;
		}
		public bool IsValueLinkSet(string name)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				if (pvl.Linker != null)
				{
					return pvl.Linker.IsValueLinkSet();
				}
			}
			return false;
		}
		public bool IsLinkableProperty(string name)
		{
			return _properties.ContainsKey(name);
		}
		public bool IsReady(string name)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				if (pvl.Linker != null)
				{
					if (pvl.Linker.IsValueLinkSet())
					{
						return true;
					}
				}
				if (pvl.Getter != null)
				{
					return true;
				}
			}
			return false;
		}
		public void SetPropertyLink(string name, IPropertyValueLink link)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				pvl.Linker = link;
			}
		}
		public void SetPropertyGetter(string name, fnGetPropertyValue getter)
		{
			PropertyValueLinker pvl;
			if (_properties.TryGetValue(name, out pvl))
			{
				pvl.Getter = getter;
			}
		}
		public string[] GetLinkablePropertyNames()
		{
			string[] names = new string[_properties.Count];
			_properties.Keys.CopyTo(names, 0);
			return names;
		}
		public PropertyDescriptor GetPropertyDescriptor(PropertyDescriptor p)
		{
			if (VPLUtil.PropertyValueLinkEditor == null)
			{
				return p;
			}
			if (_properties.ContainsKey(p.Name))
			{
				Attribute[] a;
				if (p.Attributes != null)
				{
					IPropertyValueLink pl = GetValueLink(p.Name);
					List<Attribute> lst = new List<Attribute>();
					for (int i = 0; i < p.Attributes.Count; i++)
					{
						if (pl != null)
						{
							pl.SetValueAttribute(p.Attributes[i]);
						}
						EditorAttribute e = p.Attributes[i] as EditorAttribute;
						if (e == null)
						{
							lst.Add(p.Attributes[i]);
						}
					}
					a = new Attribute[lst.Count + 1];
					lst.CopyTo(a, 0);
				}
				a = new Attribute[1];
				a[a.Length - 1] = new EditorAttribute(VPLUtil.PropertyValueLinkEditor, typeof(UITypeEditor));
				PropertyDescriptorValueLink p0 = new PropertyDescriptorValueLink(this, p.Name, a);
				return p0;
			}
			return p;
		}
		#endregion
		#region xml serialization
		const string XML_Item = "Item";
		const string XMLATT_name = "name";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNodeList ns = node.SelectNodes(XML_Item);
			foreach (XmlNode nd in ns)
			{
				if (nd.Attributes != null)
				{
					XmlAttribute xa = nd.Attributes[XMLATT_name];
					if (xa != null)
					{
						string key = xa.Value;
						if (!string.IsNullOrEmpty(key))
						{
							PropertyValueLinker lk;
							if (_properties.TryGetValue(key, out lk))
							{
								object vl = Activator.CreateInstance(VPLUtil.PropertyValueLinkType, _holder, key);
								serializer.ReadObjectFromXmlNode(nd, vl, VPLUtil.PropertyValueLinkType, this);
								lk.Linker = vl as IPropertyValueLink;
							}
						}
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_properties != null)
			{
				foreach (KeyValuePair<string, PropertyValueLinker> kv in _properties)
				{
					if (kv.Value.Linker != null)
					{
						XmlNode itemNode = node.OwnerDocument.CreateElement(XML_Item);
						node.AppendChild(itemNode);
						XmlAttribute xa = node.OwnerDocument.CreateAttribute(XMLATT_name);
						itemNode.Attributes.Append(xa);
						xa.Value = kv.Key;
						serializer.WriteObjectToNode(itemNode, kv.Value.Linker);
					}
				}
			}

		}
		#endregion
	}
	class PropertyDescriptorValueLink : PropertyDescriptor
	{
		private PropertyValueLinks _owner;
		public PropertyDescriptorValueLink(PropertyValueLinks owner, string name, Attribute[] attrs)
			: base(name, attrs)
		{
			_owner = owner;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _owner.Holder.GetType(); }
		}

		public override object GetValue(object component)
		{
			return _owner.GetValueLink(this.Name);
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get
			{
				return VPLUtil.PropertyValueLinkType;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			_owner.GetValueLink(this.Name).SetValue(value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
