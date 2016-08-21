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
	public class HtmlElement_head : HtmlElement_Base
	{
		private static Guid _headId;
		public HtmlElement_head(ClassPointer owner)
			: base(owner)
		{
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
		[Browsable(false)]
		[NotForProgramming]
		public override Guid ElementGuid
		{
			get
			{
				if (_headId == Guid.Empty)
					_headId = new Guid("{0756BBA2-6AB5-4120-B637-9F8F5D289B3B}");
				return _headId;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ReferenceName
		{
			get { return "head"; }
		}
		public override string tagName
		{
			get { return "head"; }
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
	}
}
