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
	public class HtmlElement_input_password : HtmlElement_input
	{
		public HtmlElement_input_password(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_input_password(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string type
		{
			get { return "password"; }
		}
	}
}
