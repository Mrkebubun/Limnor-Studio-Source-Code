/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	class ConstObjectPointerJsValue : ConstObjectPointer
	{
		private string _jsValue;
		public ConstObjectPointerJsValue()
			: base(new TypePointer(typeof(object)))
		{
			_jsValue = string.Empty;
		}
		public ConstObjectPointerJsValue(string jsValue)
			: base(new TypePointer(typeof(object)))
		{
			_jsValue = jsValue;
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return _jsValue;
		}
	}
}
