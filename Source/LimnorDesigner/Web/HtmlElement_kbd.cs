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
	public class HtmlElement_kbd : HtmlElement_ItemBase
	{
		public HtmlElement_kbd(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_kbd(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "kbd"; }
		}
	}
	public class HtmlElement_em : HtmlElement_ItemBase
	{
		public HtmlElement_em(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_em(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "em"; }
		}
	}
	public class HtmlElement_dfn : HtmlElement_ItemBase
	{
		public HtmlElement_dfn(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_dfn(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "dfn"; }
		}
	}
	public class HtmlElement_samp : HtmlElement_ItemBase
	{
		public HtmlElement_samp(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_samp(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "samp"; }
		}
	}
	public class HtmlElement_strong : HtmlElement_ItemBase
	{
		public HtmlElement_strong(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_strong(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "strong"; }
		}
	}
	public class HtmlElement_var : HtmlElement_ItemBase
	{
		public HtmlElement_var(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_var(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "var"; }
		}
	}
}
