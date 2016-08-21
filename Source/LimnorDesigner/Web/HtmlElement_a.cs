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
using System.Xml.Serialization;

namespace LimnorDesigner.Web
{
	public class HtmlElement_a : HtmlElement_ItemBase
	{
		public HtmlElement_a(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_a(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "a"; }
		}
		[Bindable(true)]
		[Description("Sets or returns the text of an input")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string href
		{
			get;
			set;
		}
	}
}
