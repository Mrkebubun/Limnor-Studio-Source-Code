/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using Limnor.WebBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LimnorDesigner.Web
{
	[Description("it is used to override the current text direction")]
	public class HtmlElement_bdo : HtmlElement_ItemBase
	{
		public HtmlElement_bdo(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_bdo(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "bdo"; }
		}
	}
}
