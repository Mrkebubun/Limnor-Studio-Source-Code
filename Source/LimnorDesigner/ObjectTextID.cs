/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorDesigner
{
	/// <summary>
	/// for showing object (property) usages
	/// </summary>
	public class ObjectTextID
	{
		string _textId;
		public ObjectTextID(string className, string type, string name)
		{
			ClassName = className;
			ObjectType = type;
			ObjectName = name;
		}
		public string ClassName { get; set; }
		public string ObjectType { get; set; }
		public string ObjectName { get; set; }
		public override string ToString()
		{
			if (string.IsNullOrEmpty(_textId))
			{
				_textId = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"class:{0}, object:{1} {2}", ClassName, ObjectType, ObjectName);
			}
			return _textId;
		}
	}
}
