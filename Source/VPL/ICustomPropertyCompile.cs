/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace VPL
{
	/// <summary>
	/// TreeViewXTemplate uses it to generate property reference code 
	/// </summary>
	public interface ICustomPropertyCompile
	{
		CodeExpression GetReferenceCode();
	}
}
