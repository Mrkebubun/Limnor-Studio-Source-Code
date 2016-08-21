/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Limnor.WebBuilder;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using VPL;
using XmlUtility;
using LFilePath;

namespace LimnorDesigner.Web
{
	public class HtmlElement_img : HtmlElement_ItemBase
	{
		public HtmlElement_img(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_img(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "img"; }
		}
		[FilePath("Image files|*.jpg;*.gif;*.png;*.bmp", "Select image file")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		[WebClientMember]
		public string src
		{
			get;
			set;
		}
		[Browsable(false)]
		[Description("Specifies an alternate text for an image")]
		[WebClientMember]
		public string alt
		{
			get;
			set;
		}
		[Browsable(false)]
		[Description("Specifies the width of an image, in pixels or in percentage")]
		[WebClientMember]
		public string width
		{
			get;
			set;
		}
		[Browsable(false)]
		[Description("Specifies the height of an image, in pixels or in percentage")]
		[WebClientMember]
		public string height
		{
			get;
			set;
		}
		[Editor(typeof(TypeEditorHtmlMap), typeof(UITypeEditor))]
		[Description("Specifies the map element which defines map areas on the image")]
		[WebClientMember]
		public string usemap
		{
			get;
			set;
		}
		public override object Clone()
		{
			HtmlElement_img himg = base.Clone() as HtmlElement_img;
			himg.src = src;
			return himg;
		}
		#region IXmlNodeSerializable Members
		const string XMLATT_src = "src";
		const string XMLATT_usemap = "usemap";
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			src = XmlUtil.GetAttribute(node, XMLATT_src);
			usemap = XmlUtil.GetAttribute(node, XMLATT_usemap);
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
#if NET35
			if(string.IsNullOrEmpty(usemap))
#else
			if (string.IsNullOrWhiteSpace(usemap))
#endif
			{
				XmlUtil.RemoveAttribute(node, XMLATT_usemap);
			}
			else
			{
				XmlUtil.SetAttribute(node, XMLATT_usemap, usemap);
			}
		}
		#endregion
	}
}
