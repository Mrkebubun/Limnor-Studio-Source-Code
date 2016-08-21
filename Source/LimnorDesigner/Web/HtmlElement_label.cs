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
using System.Drawing.Design;
using VPL;
using System.Xml;
using XmlUtility;
using System.Xml.Serialization;

namespace LimnorDesigner.Web
{
	public class HtmlElement_label : HtmlElement_ItemBase
	{
		public HtmlElement_label(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_label(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "label"; }
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		[WebClientMember]
		public string htmlFor
		{
			get;
			set;
		}
		public override object Clone()
		{
			HtmlElement_label himg = base.Clone() as HtmlElement_label;
			himg.htmlFor = htmlFor;
			return himg;
		}
	}
}
