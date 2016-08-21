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
using System.Drawing.Design;
using LFilePath;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.Web
{
	public class HtmlElement_script : HtmlElement_Base
	{
		public HtmlElement_script(ClassPointer owner, Guid guid)
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
			get { return "script"; }
		}
		public override string tagName
		{
			get { return "script"; }
		}
		public override string CodeName
		{
			get { return "unknown"; }
		}
		[FilePath("Script files|*.js", "Select script file")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		public string src
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
