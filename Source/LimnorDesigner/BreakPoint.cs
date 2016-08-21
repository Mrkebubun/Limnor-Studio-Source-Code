/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorDesigner
{
	public interface IBreakPointOwner
	{
		bool BreakBeforeExecute { get; set; }
		bool BreakAfterExecute { get; set; }
	}
}
