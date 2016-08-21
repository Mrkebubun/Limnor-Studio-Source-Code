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
	public class HtmlElement_menu : HtmlElement_ItemBase
	{
		public HtmlElement_menu(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_menu(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "menu"; }
		}
	}
}
