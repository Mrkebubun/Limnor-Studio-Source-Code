/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Limnor.WebServerBuilder;
using System.Drawing;
using System.Drawing.Design;

namespace Limnor.PhpComponents
{
	[TypeMapping(typeof(Array))]
	[ToolboxBitmapAttribute(typeof(PhpArray), "Resources.array.bmp")]
	[PhpType("array()", true)]
	public class PhpArray : IPhpType
	{
		#region fields and constructors
		private IPhpType[] _value;
		private Type _itemType;
		public PhpArray()
		{
			_itemType = typeof(PhpString);
		}
		#endregion
		#region Properties
		[WebServerMember]
		public int Length
		{
			get
			{
				return 0;
			}
		}
		[Editor(typeof(TypeEditorSelectPhpType), typeof(UITypeEditor))]
		[Description("Data type for each array item")]
		public Type ItemType
		{
			get
			{
				return _itemType;
			}
			set
			{
				if (value == null)
				{
				}
				else
				{
					_itemType = value;
				}
			}
		}
		#endregion
		#region Methods
		[WebServerMember]
		[Description("Remove string array elements by comparing its string values. Returns number of elements removed.")]
		public int RemoveStringElementByValue(string value, bool caseSensitive, EnumCompareString compare)
		{
			return 0;
		}
		//
		[WebServerMember]
		[Description("Merge the array into one string, using the specified delimeter.")]
		public PhpString MergeToString(string delimeter)
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("Gets an array item by array index.")]
		public object Get(int index)
		{
			return null;
		}
		[WebServerMember]
		[Description("Sets an array item by array index.")]
		public void Set(int index, object value)
		{
		}
		[WebServerMember]
		[Description("Gets an array item by array key.")]
		public object GetByKey(string key)
		{
			return null;
		}
		[WebServerMember]
		[Description("Sets an array item by array key.")]
		public void SetByKey(string key, object value)
		{
		}
		#endregion
		#region IPhpType Members
		[NotForProgramming]
		[Browsable(false)]
		public string GetMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, "RemoveStringElementByValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "removeStringArrayElement({0},{1},{2},{3})", objectName, parameters[0], parameters[1], parameters[2]);
			}
			else if (string.CompareOrdinal(methodname, "MergeToString") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "implode({0},{1})", parameters[0], objectName);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Get") == 0 || string.CompareOrdinal(methodname, "GetByKey") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", objectName, parameters[0]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Set") == 0 || string.CompareOrdinal(methodname, "SetByKey") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}[{1}] = {2}", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				return "array()";
			}
			else if (string.CompareOrdinal(methodname, "MergeArray") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
					}
				}
			}
			return "null";
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetPropertyRef(string objectName, string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Length") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "count({0})", objectName);
			}
			return "null";
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetValuePhpCode()
		{
			if (_value == null)
			{
				return "null";
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("array(");
			if (_value.Length > 0)
			{
				sb.Append("0=>");
				sb.Append(_value[0].GetValuePhpCode());
				for (int i = 1; i < _value.Length; i++)
				{
					sb.Append(",");
					sb.Append(i);
					sb.Append("=>");
					sb.Append(_value[i].GetValuePhpCode());
				}
			}
			sb.Append(")");
			return sb.ToString();
		}
		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			PhpArray obj = new PhpArray();
			obj._itemType = _itemType;
			if (_value != null)
			{
				IPhpType[] jss = new IPhpType[_value.Length];
				for (int i = 0; i < _value.Length; i++)
				{
					jss[i] = _value[i].Clone() as IPhpType;
				}
				obj._value = jss;
			}
			return obj;
		}

		#endregion
	}
}
