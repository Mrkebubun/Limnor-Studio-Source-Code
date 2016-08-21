/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FormComponents
{
	public class KeyPair : ICloneable
	{
		public KeyPair()
		{
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}?:{1}", PreviousKey, Value);
		}
		//
		/// <summary>
		/// key-combination
		/// </summary>
		[Description("key combination")]
		public string PreviousKey { get; set; }

		/// <summary>
		/// result keys for the key-combination
		/// </summary>
		[Description("result keys for the key-combination")]
		public string Value { get; set; }

		#region ICloneable Members

		public object Clone()
		{
			KeyPair obj = new KeyPair();
			obj.PreviousKey = PreviousKey;
			obj.Value = Value;
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(KeyMapsConverter))]
	public class KeyPairList : ICloneable
	{
		#region fields and constructors
		public string thisKey = "";
		private KeyPair[] list = null;
		public KeyPairList()
		{
		}
		#endregion
		#region static utility
		static public KeyBuffer theFirstKey = new KeyBuffer(); //keyboard recording
		static public int KeyLength = 180; //buffer size for keyboard recording
		static private KeyBuffer replacedKeys = new KeyBuffer(); //to make backspace work

		static public KeyBuffer ReplacedKeys
		{
			get
			{
				return replacedKeys;
			}
		}
		static public void ClearKeys()
		{
			theFirstKey.Clear();
			replacedKeys.Clear();
		}
		/// <summary>
		/// remember the key sent 
		/// </summary>
		/// <param name="key">it is the SendKeys property of the button being clicked. It is to be combined with the next SendKeys to form a lookup key for the result</param>
		static public void PushKey(string key)
		{
			theFirstKey.PushKey(key);
			replacedKeys.PushKey(key);
		}
		static public void PushKeyBuffer(string key)
		{
			replacedKeys.PushKey(key);
		}
		static public void PopKeyBuffer()
		{
			replacedKeys.PopKey();
			theFirstKey.PopKey();
		}
		/// <summary>
		/// replace the key recording with the combined key 
		/// so that recursive combinations are possible.
		/// </summary>
		/// <param name="len"></param>
		/// <param name="key"></param>
		static public void ReplaceKey(int len, string key)
		{
			for (int i = 0; i < len; i++)
			{
				theFirstKey.PopKey();
			}
			theFirstKey.PushKey(key);
		}
		#endregion
		#region Methods
		public KeyPair GetCombinedKey()
		{
			KeyPair kp = this[replacedKeys.TopKey()];
			if (kp == null)
			{
				kp = this[theFirstKey.TopKey()];
			}
			return kp;
		}
		public void Clear()
		{
			list = null;
		}
		public void SetList(KeyPair[] a)
		{
			list = a;
		}
		public KeyPair SetKeyPair(string key, string val)
		{
			KeyPair kp = null;
			if (list != null)
			{
				for (int i = 0; i < list.Length; i++)
				{
					if (list[i].PreviousKey == key)
					{
						kp = list[i];
						break;
					}
				}
			}
			if (kp == null)
			{
				int n;
				kp = new KeyPair();
				kp.PreviousKey = key;
				if (list == null)
				{
					list = new KeyPair[1];
					n = 0;
				}
				else
				{
					n = list.Length;
					KeyPair[] a = new KeyPair[n + 1];
					for (int i = 0; i < n; i++)
					{
						a[i] = list[i];
					}
					list = a;
				}
				list[n] = kp;
			}
			kp.Value = val;
			return kp;
		}
		public KeyPair DeleteKeyPair(string key)
		{
			KeyPair kp = null;
			int n = Count;
			for (int i = 0; i < n; i++)
			{
				if (list[i].PreviousKey == key)
				{
					kp = list[i];
					if (n == 1)
					{
						list = null;
					}
					else
					{
						KeyPair[] a = new KeyPair[n - 1];
						for (int j = 0; j < n; j++)
						{
							if (j < i)
								a[j] = list[j];
							else if (j > i)
							{
								a[j - 1] = list[j];
							}
						}
						list = a;
					}
					break;
				}
			}
			return kp;
		}
		public override string ToString()
		{
			string s = "";
			int n = Count;
			for (int i = 0; i < n; i++)
			{
				if (i > 0)
					s += ",";
				s += list[i].ToString();
			}
			return s;
		}
		#endregion
		#region Properties
		public int Count
		{
			get
			{
				if (list == null)
					return 0;
				return list.Length;
			}
		}
		public KeyPair this[int Index]
		{
			get
			{
				if (Index >= 0 && Index < Count)
					return list[Index];
				return null;
			}
		}
		/// <summary>
		/// usually key is the static member theFirstKey
		/// </summary>
		public KeyPair this[string key]
		{
			get
			{
				if (list != null)
				{
					for (int i = 0; i < list.Length; i++)
					{
						if (key.Length >= list[i].PreviousKey.Length)
						{
							if (list[i].PreviousKey == key.Substring(0, list[i].PreviousKey.Length))
							{
								return list[i];
							}
						}
					}
				}
				return null;
			}
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			KeyPairList obj = new KeyPairList();
			obj.thisKey = thisKey;
			int n = Count;
			if (n > 0)
			{
				KeyPair[] a = new KeyPair[n];
				for (int i = 0; i < n; i++)
				{
					a[i] = this[i];
				}
				obj.SetList(a);
			}
			return obj;
		}

		#endregion
	}

	public class KeyBuffer : List<string>
	{
		int max = 180;
		public KeyBuffer()
		{
		}
		public int BufferSize
		{
			get
			{
				return max;
			}
			set
			{
				max = value;

			}
		}
		public void PushKey(string s)
		{
			this.Insert(0, s);
			while (this.Count > max)
			{
				this.RemoveAt(this.Count - 1);
			}
		}
		public string PopKey()
		{
			if (this.Count > 0)
			{
				string s = this[0];
				this.RemoveAt(0);
				return s;
			}
			return string.Empty;
		}
		public string TopKey()
		{
			if (this.Count > 0)
			{
				return this[0];
			}
			return string.Empty;
		}
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			for (int i = 0; i < this.Count; i++)
			{
				s.Append("[");
				s.Append(this[i]);
				s.Append("]");
			}
			return s.ToString();
		}

	}

}
