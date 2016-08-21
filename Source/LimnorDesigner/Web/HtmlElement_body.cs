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
using VPL;
using System.Collections.Specialized;
using System.Globalization;

namespace LimnorDesigner.Web
{
	public class HtmlElement_body : HtmlElement_BodyBase
	{
		private static Guid _bodyId;
		public HtmlElement_body(ClassPointer owner)
			: base(owner)
		{
		}
		[Browsable(false)]
		[NotForProgramming]
		public static Guid BodyGuid
		{
			get
			{
				if (_bodyId == Guid.Empty)
					_bodyId = new Guid("{9DA3F46C-713E-46f4-A820-36AEC0FCA896}");
				return _bodyId;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override Guid ElementGuid
		{
			get
			{
				return BodyGuid;
			}
		}
		public override string id
		{
			get
			{
				return string.Empty;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override bool IsValid
		{
			get { return true; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string CodeName
		{
			get
			{
				return "document.body";
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string GetJavaScriptReferenceCode(StringCollection code, string attributeName, string[] parameters)
		{
			return "document.body";
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ReferenceName
		{
			get { return "document.body"; }
		}
		public override string tagName
		{
			get { return "body"; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public override bool IsSameElement(HtmlElement_Base e)
		{
			HtmlElement_body bd = e as HtmlElement_body;
			return (bd != null);
		}
		#region ICloneable Members

		public override object Clone()
		{
			return new HtmlElement_body(this.RootPointer);
		}
		#endregion
	}
}
