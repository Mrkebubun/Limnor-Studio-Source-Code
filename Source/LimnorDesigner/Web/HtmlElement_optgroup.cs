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
	public class HtmlElement_optgroup : HtmlElement_ItemBase
	{
		public HtmlElement_optgroup(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_optgroup(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "optgroup"; }
		}
		[Description("Specifies that the element should be disabled")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool disabled
		{
			get;
			set;
		}
		[Description("Specifies a label for an option-group")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string label
		{
			get;
			set;
		}
	}
}
