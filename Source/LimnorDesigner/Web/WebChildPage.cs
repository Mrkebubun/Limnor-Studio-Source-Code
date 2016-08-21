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

namespace LimnorDesigner.Web
{
	public class WebChildPage
	{
		public WebChildPage()
		{
		}
		[WebClientMember]
		public UInt32 getPageId()
		{
			return 0;
		}
		[WebClientMember]
		public string getPageUrl()
		{
			return null;
		}
		[WebClientMember]
		public HtmlElement_document getPageDoc()
		{
			return null;
		}
		[WebClientMember]
		public HtmlElement_window getPageWindow()
		{
			return null;
		}
		[WebClientMember]
		public void closeDialog()
		{
		}
		[WebClientMember]
		public void cancelDialog()
		{
		}
		[WebClientMember]
		public void hideWindow()
		{
		}
		[WebClientMember]
		public HtmlElement_ItemBase getChildElement(string id)
		{
			return null;
		}
		[WebClientMember]
		public void bringToFront()
		{
		}
		[WebClientMember]
		public bool isDialog()
		{
			return false;
		}
		[WebClientMember]
		public bool isVisible()
		{
			return true;
		}
	}
}
