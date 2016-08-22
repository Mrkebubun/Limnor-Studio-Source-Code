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
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	[GlobalFunction]
	[WebClientClassAttribute]
	[ToolboxBitmapAttribute(typeof(WebMessageBox), "Resources.msg.bmp")]
	[Description("Its methods can be used to create pop up message boxes.")]
	public sealed class WebMessageBox
	{
		[WebClientMember]
		public static void alert(string message) { }
		[WebClientMember]
		public static bool confirm(string message) { return false; }
		[WebClientMember]
		public static string prompt(string message, string initialValue) { return string.Empty; }
	}
}
