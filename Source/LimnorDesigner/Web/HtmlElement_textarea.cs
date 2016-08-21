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
using System.Xml.Serialization;
using Limnor.WebBuilder;

namespace LimnorDesigner.Web
{
	public class HtmlElement_textarea : HtmlElement_ItemBase
	{
		public HtmlElement_textarea(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_textarea(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "textarea"; }
		}
		[Bindable(true)]
		[Description("Sets or returns the text of a text area")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public JsString value
		{
			get;
			set;
		}
	}
}
