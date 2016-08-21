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
using VPL;
using Limnor.WebServerBuilder;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.Web
{
	public class HtmlElement_input_file : HtmlElement_input, IFormSubmitter
	{
		public HtmlElement_input_file(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_input_file(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string type
		{
			get { return "file"; }
		}
		[WebServerMember]
		[Description("Upload the file.")]
		public void Upload()
		{
		}

		#region IFormSubmitter Members

		public string FormName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}f", this.Site.Name);
			}
		}
		public bool IsSubmissionMethod(string method)
		{
			return (string.CompareOrdinal(method, "Upload") == 0);
		}
		public bool RequireSubmissionMethod(string method)
		{
			return false;
		}

		#endregion
	}
}
