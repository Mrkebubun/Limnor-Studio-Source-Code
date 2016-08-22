/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Globalization;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTextArea), "Resources.textarea.bmp")]
	[Description("This is a multi-line text box on a web page.")]
	public class HtmlTextArea : HtmlTextBox
	{
		public HtmlTextArea()
		{
			Text = "";
			this.Multiline = true;
			this.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "SetHeightToContent") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetTextHeightToContent(document.getElementById('{0}'));\r\n", this.CodeName));
			}
			else
			{
				base.CreateActionJavaScript(methodName, code, parameters, returnReceiver);
			}
		}
		[Description("Occurs when the height is adjusted according to the content via executing a SetHeightToContent action")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onHeightAdjusted { add { } remove { } }

		[Description("Set the height of the control to the height of its contents. Note that the height change does not happen immediately after calling this method. At the event of onHeightAdjusted the height change is completed.")]
		[WebClientMember]
		public void SetHeightToContent()
		{
		}
		[Browsable(false)]
		public override string ElementName { get { return "textarea"; } }

		[Browsable(false)]
		protected override void OnSetType(XmlNode node)
		{
			node.InnerText = this.Text;
			XmlElement xe = (XmlElement)node;
			xe.IsEmpty = false;
		}
	}
}
