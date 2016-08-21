/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;

namespace LimnorDesigner.Action
{
	public interface IInlineActionHolder
	{
		MethodClass Method { get; }
	}
}
