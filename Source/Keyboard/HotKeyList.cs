/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VPL;
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Limnor.InputDevice
{
	[Flags()]
	public enum KeyModifiers
	{
		None = 0,
		Alt = 1,
		Control = 2,
		Shift = 4,
		Windows = 8
	}
	/// <summary>
	/// mark it serializable for CodeDom
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(TypeConverterKey))]
	public class Key : IXmlNodeSerializable, IEventHolder, ICloneable
	{
		#region fields and constructors
		private int _keyId;
		private Keys _keys;
		private string _keyname;
		public Key()
		{
			_keyId = Guid.NewGuid().GetHashCode();
		}
		public Key(Keys key)
		{
			Keys = key;
			_keyId = Guid.NewGuid().GetHashCode();
		}
		public Key(Keys key, int id)
		{
			Keys = key;
			_keyId = id;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public bool HotKeySet
		{
			get;
			set;
		}
		[Browsable(false)]
		public int KeyId
		{
			get
			{
				if (_keyId == 0)
				{
					_keyId = Guid.NewGuid().GetHashCode();
				}
				return _keyId;
			}
		}
		public Keys Keys
		{
			get
			{
				return _keys;
			}
			set
			{
				_keys = value;
				_keyname = null;
			}
		}
		public KeyModifiers Modifiers
		{
			get
			{
				KeyModifiers m = KeyModifiers.None;
				if ((Keys & Keys.Alt) == Keys.Alt)
				{
					m |= KeyModifiers.Alt;
				}
				if ((Keys & Keys.Shift) == Keys.Shift)
				{
					m |= KeyModifiers.Shift;
				}
				if ((Keys & Keys.Control) == Keys.Control)
				{
					m |= KeyModifiers.Control;
				}
				return m;
			}
		}
		public Keys VirtuakKeyCode
		{
			get
			{
				Keys k = this.Keys & (~Keys.Alt);
				k = k & (~Keys.Shift);
				k = k & (~Keys.Control);
				return k;
			}
		}
		[Browsable(false)]
		public string KeyName
		{
			get
			{
				if (string.IsNullOrEmpty(_keyname))
				{
					_keyname = "KeyPress_" + Keys.ToString().Replace('+', '_').Replace(',', '_').Replace(' ', '_');
				}
				return _keyname;
			}
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			return Keys.ToString();
		}
		public void FireKeyPress(object sender)
		{
			if (Event != null)
			{
				Event(sender, EventArgs.Empty);
			}
		}
		public override int GetHashCode()
		{
			return KeyName.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			Key k = obj as Key;
			if (k != null)
			{
				return (k.Keys == this.Keys);
			}
			return false;
		}
		public static bool operator ==(Key k1, Key k2)
		{
			object v1 = k1;
			object v2 = k2;
			if (v2 == null)
			{
				if (v1 == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			if (v1 == null)
				return false;
			return (k1.Keys == k2.Keys);
		}
		public static bool operator !=(Key k1, Key k2)
		{
			object v1 = k1;
			object v2 = k2;
			if (v2 == null)
			{
				if (v1 == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			if (v1 == null)
				return true;
			return (k1.Keys != k2.Keys);
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XMLATT_KEY = "keys";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));
			Keys = (Keys)converter.ConvertFromInvariantString(XmlUtil.GetAttribute(node, XMLATT_KEY));
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_KEY, Keys);
		}

		#endregion

		#region IEventHolder Members

		public event EventHandler Event;

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new Key(this.Keys, KeyId);
		}

		#endregion
	}
	/// <summary>
	/// marked as Serializable for CodeDom
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(TypeConverterKeyList))]
	public class HotKeyList : IXmlNodeSerializable, ICloneable
	{
		#region fields and constructors
		private SortedList<string, Key> _hotKeys;
		public HotKeyList()
		{
		}
		#endregion
		#region Methods
		public bool ContainsKey(string key)
		{
			return HotKeys.ContainsKey(key);
		}
		public bool Remove(string key)
		{
			return HotKeys.Remove(key);
		}
		public void AddKey(Key k)
		{
			if (!HotKeys.ContainsKey(k.KeyName))
			{
				HotKeys.Add(k.KeyName, k);
			}
		}
		public bool TryGetKey(string keyName, out Key key)
		{
			if (_hotKeys != null)
			{
				return _hotKeys.TryGetValue(keyName, out key);
			}
			key = new Key();
			return false;
		}
		public Key GetKeyById(int id)
		{
			if (_hotKeys != null)
			{
				for (int i = 0; i < _hotKeys.Count; i++)
				{
					if (_hotKeys.Values[i].KeyId == id)
					{
						return _hotKeys.Values[i];
					}
				}
			}
			return null;
		}
		public void UpdateKeyById(Key k)
		{
			if (_hotKeys != null)
			{
				for (int i = 0; i < _hotKeys.Count; i++)
				{
					if (_hotKeys.Values[i].KeyId == k.KeyId)
					{
						_hotKeys.RemoveAt(i);
						break;
					}
				}
			}
			else
			{
				_hotKeys = new SortedList<string, Key>();
			}
			if (!_hotKeys.ContainsKey(k.KeyName))
			{
				_hotKeys.Add(k.KeyName, k);
			}
		}
		public override string ToString()
		{
			return "Keys:" + Count.ToString();
		}
		#endregion
		#region Properties
		public int Count
		{
			get
			{
				if (_hotKeys == null)
					return 0;
				return _hotKeys.Count;
			}
		}
		public Key this[int i]
		{
			get
			{
				if (_hotKeys == null)
					throw new IndexOutOfRangeException("HotKeys is null. i=" + i.ToString());
				if (i < 0)
					throw new IndexOutOfRangeException("Index cannot be negative. i=" + i.ToString());
				if (i >= _hotKeys.Count)
					throw new IndexOutOfRangeException("Index too large. i=" + i.ToString());
				return _hotKeys.Values[i];
			}
		}
		public SortedList<string, Key> HotKeys
		{
			get
			{
				if (_hotKeys == null)
				{
					_hotKeys = new SortedList<string, Key>();
				}
				return _hotKeys;
			}
			set
			{
				_hotKeys = value;
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XML_Item = "Item";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_hotKeys = new SortedList<string, Key>();
			XmlNodeList nodes = node.SelectNodes(XML_Item);
			foreach (XmlNode nd in nodes)
			{
				Key k = new Key();
				k.OnReadFromXmlNode(serializer, nd);
				_hotKeys.Add(k.KeyName, k);
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_hotKeys != null)
			{
				for (int i = 0; i < _hotKeys.Count; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XML_Item);
					node.AppendChild(nd);
					_hotKeys.Values[i].OnWriteToXmlNode(serializer, nd);
				}
			}
		}

		#endregion
		#region ICloneable Members

		public object Clone()
		{
			HotKeyList kl = new HotKeyList();
			if (_hotKeys != null)
			{
				foreach (Key k in _hotKeys.Values)
				{
					kl.AddKey(k);
				}
			}
			return kl;
		}

		#endregion
	}
}
