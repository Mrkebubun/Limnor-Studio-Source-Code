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
using VPL;
using System.Globalization;

namespace LimnorDesigner.Web
{
	public abstract class HtmlElement_input : HtmlElement_ItemBase
	{
		#region fields and constructors
		public HtmlElement_input(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_input(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		#endregion
		#region Properties
		public override string tagName
		{
			get { return "input"; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ImageKey
		{
			get
			{
				return type;
			}
		}
		[Description("Type of the input control")]
		[WebClientMember]
		public abstract string type { get; }
		//
		[Description("Specifies that an input should be disabled")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool disabled
		{
			get;
			set;
		}
		[Description("Sets or returns the name of an input")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string name
		{
			get;
			set;
		}
		[Bindable(true)]
		[Description("Sets or returns the text of an input")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public JsString value
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string ExpressionDisplay
		{
			get
			{
				if (string.IsNullOrEmpty(id))
					return type;
				return id;
			}
		}
		#endregion
		#region Events
		[Description("It occurs when an element when an element change")]
		[WebClientMember]
		public event WebSimpleEventHandler onchange { add { } remove { } }
		#endregion
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			if (string.IsNullOrEmpty(id))
				return type;
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", type, id);
		}
	}
}
