/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner.Web
{
	public class HtmlElement_h1 : HtmlElement_ItemBase
	{
		public HtmlElement_h1(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h1(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h1"; }
		}
	}
	public class HtmlElement_h2 : HtmlElement_ItemBase
	{
		public HtmlElement_h2(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h2(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h2"; }
		}
	}
	public class HtmlElement_h3 : HtmlElement_ItemBase
	{
		public HtmlElement_h3(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h3(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h3"; }
		}
	}
	public class HtmlElement_h4 : HtmlElement_ItemBase
	{
		public HtmlElement_h4(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h4(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h4"; }
		}
	}
	public class HtmlElement_h5 : HtmlElement_ItemBase
	{
		public HtmlElement_h5(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h5(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h5"; }
		}
	}
	public class HtmlElement_h6 : HtmlElement_ItemBase
	{
		public HtmlElement_h6(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_h6(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "h6"; }
		}
	}
}
