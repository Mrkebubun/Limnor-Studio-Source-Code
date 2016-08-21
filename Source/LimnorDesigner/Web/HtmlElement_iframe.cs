/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LFilePath;
using System.ComponentModel;
using System.Drawing.Design;
using Limnor.WebBuilder;
using VPL;
using XmlUtility;
using System.Xml;

namespace LimnorDesigner.Web
{
	public class HtmlElement_iframe : HtmlElement_ItemBase
	{
		public HtmlElement_iframe(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_iframe(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "iframe"; }
		}
		[FilePath("Html files|*.html;*.htm", "Select html file")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		[WebClientMember]
		public string src
		{
			get;
			set;
		}
		public override object Clone()
		{
			HtmlElement_iframe himg = base.Clone() as HtmlElement_iframe;
			himg.src = src;
			return himg;
		}
		#region IXmlNodeSerializable Members
		const string XMLATT_src = "src";
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			src = XmlUtil.GetAttribute(node, XMLATT_src);
		}

		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
#if NET35
			if(string.IsNullOrEmpty(src))
#else
			if (string.IsNullOrWhiteSpace(src))
#endif
			{
				XmlUtil.RemoveAttribute(node, XMLATT_src);
			}
			else
			{
				XmlUtil.SetAttribute(node, XMLATT_src, src);
			}
		}

		#endregion
	}
}
