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
using Limnor.WebBuilder;

namespace LimnorDesigner.Web
{
	public class HtmlElement_form : HtmlElement_ItemBase
	{
		public HtmlElement_form(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_form(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "form"; }
		}
		#region Events
		[Description("It occurs when a form is reset")]
		[WebClientMember]
		public event WebSimpleEventHandler onreset { add { } remove { } }

		[Description("It occurs when a form is submitted")]
		[WebClientMember]
		public event WebSimpleEventHandler onsubmit { add { } remove { } }
		#endregion
	}
}
