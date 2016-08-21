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
	public class HtmlElement_button : HtmlElement_ItemBase
	{
		public HtmlElement_button(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_button(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "button"; }
		}
		[Description("Sets or returns the name of a button")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string name
		{
			get;
			set;
		}
		[Description("Sets or returns the text of a button")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string value
		{
			get;
			set;
		}
		[Description("Specifies that a button should be disabled")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool disabled
		{
			get;
			set;
		}
		[Description("Specifies the type of button")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public EnumButtonType type
		{
			get { return EnumButtonType.button; }
		}
	}
	public enum EnumButtonType
	{
		button,
		reset,
		submit
	}
}
