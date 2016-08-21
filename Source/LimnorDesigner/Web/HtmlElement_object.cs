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
using Limnor.WebBuilder;
using System.Drawing.Design;
using VPL;

namespace LimnorDesigner.Web
{
	public class HtmlElement_object : HtmlElement_ItemBase
	{
		public HtmlElement_object(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_object(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		[Browsable(false)]
		[NotForProgramming]
		public void AppendArchiveFile(string name, string archiveFilePath)
		{
			WebPage p = Page;
			if (p != null)
			{
				string s = p.AppendArchiveFile(name, archiveFilePath);
				if (!string.IsNullOrEmpty(s))
				{
					archive = s;
				}
			}
		}
		public override string tagName
		{
			get { return "object"; }
		}
		[FilePath("data files|*.*", "Select object data file")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		[WebClientMember]
		public string data
		{
			get;
			set;
		}
		[FilePath("archive files|*.*", "Select a file to append to object archive")]
		[Editor(typeof(TypeEditorHtmlImgSrc), typeof(UITypeEditor))]
		[WebClientMember]
		public string archive
		{
			get;
			set;
		}
	}
}
