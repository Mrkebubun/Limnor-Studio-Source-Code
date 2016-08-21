/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.Web
{
	class HtmlElementUnknown : HtmlElement_BodyBase
	{
		public HtmlElementUnknown(ClassPointer owner)
			: base(owner)
		{
		}
		public override string ToString()
		{
			return "unknown";
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string id
		{
			get
			{
				return string.Empty;
			}
		}
		public override string tagName
		{
			get { return "unknown"; }
		}
		public override string ReferenceName
		{
			get { return "unknown"; }
		}
		public override string CodeName
		{
			get { return "unknown"; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string GetJavaScriptReferenceCode(StringCollection code, string attributeName, string[] parameters)
		{
			return null;
		}
		#region ICloneable Members

		public override object Clone()
		{
			return new HtmlElementUnknown(this.RootPointer);
		}
		#endregion
	}
}
