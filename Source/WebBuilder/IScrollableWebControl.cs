/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebBuilder
{
	public enum EnumOverflow
	{
		visible = 0, // The overflow is not clipped. It renders outside the element's box. This is default 
		hidden = 1, //The overflow is clipped, and the rest of the content will be invisible
		scroll = 2, //The overflow is clipped, but a scroll-bar is added to see the rest of the content
		auto = 3, //If overflow is clipped, a scroll-bar should be added to see the rest of the content
		inherit = 4 //Specifies that the value of the overflow property should be inherited from the parent element 
	}
	public interface IScrollableWebControl
	{
		EnumOverflow Overflow { get; set; }
	}
}
