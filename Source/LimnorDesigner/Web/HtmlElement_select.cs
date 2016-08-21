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
using System.Xml.Serialization;

namespace LimnorDesigner.Web
{
	public class HtmlElement_select : HtmlElement_ItemBase
	{
		public HtmlElement_select(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_select(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "select"; }
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
		[Description("Sets or returns the name of the element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string name
		{
			get;
			set;
		}
		[Description("Defines the number of visible options in a drop-down list")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int size
		{
			get;
			set;
		}
		[Description("The multiple attribute specifies that multiple options can be selected at once.")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool multiple
		{
			get;
			set;
		}
	}
}
