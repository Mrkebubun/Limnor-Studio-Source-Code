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
	public class HtmlElement_input_text : HtmlElement_input
	{
		public HtmlElement_input_text(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_input_text(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string type
		{
			get { return "text"; }
		}
		[Description("Specifies the maximum number of characters allowed in an <input> element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int maxlength
		{
			get;
			set;
		}
		[Description("Specifies that an input field should be read-only")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool readOnly
		{
			get;
			set;
		}
		[Description("Specifies the width, in characters, of an <input> element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int size
		{
			get;
			set;
		}
	}
}
