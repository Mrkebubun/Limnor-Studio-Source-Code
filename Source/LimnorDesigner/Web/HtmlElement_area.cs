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

namespace LimnorDesigner.Web
{
	public class HtmlElement_area : HtmlElement_ItemBase
	{
		public HtmlElement_area(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_area(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "area"; }
		}
		[Browsable(false)]
		public EnumAreaShape shape
		{
			get;
			set;
		}
		[Browsable(false)]
		public int[] coords
		{
			get;
			set;
		}
	}
	public enum EnumAreaShape { circle, rect, poly }
}
