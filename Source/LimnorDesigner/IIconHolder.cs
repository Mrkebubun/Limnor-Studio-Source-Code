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
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner
{
	/// <summary>
	/// implemented by MethodDesignerHolder and EventPath
	/// </summary>
	public interface IIconHolder
	{
		void ClearIconSelection();
		void SetIconSelection(ComponentIcon icon);
	}
}
