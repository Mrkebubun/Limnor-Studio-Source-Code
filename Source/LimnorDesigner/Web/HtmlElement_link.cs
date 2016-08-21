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
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.Web
{
	public class HtmlElement_link : HtmlElement_Base
	{
		public HtmlElement_link(ClassPointer owner, Guid guid)
			: base(owner, guid)
		{
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string id
		{
			get
			{
				return string.Empty;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ReferenceName
		{
			get { return "link"; }
		}
		public override string tagName
		{
			get { return "link"; }
		}
		public override string CodeName
		{
			get { return "unknown"; }
		}
		[FilePath("Stylesheet files|*.css", "Select stylesheet file")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		public string href
		{
			get;
			set;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string GetJavaScriptReferenceCode(StringCollection code, string attributeName, string[] parameters)
		{
			return null;
		}
	}
}
