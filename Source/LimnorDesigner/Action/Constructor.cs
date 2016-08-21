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

namespace LimnorDesigner.Action
{
	/// <summary>
	/// action to create a local variable within a method.
	/// its _methodPointer should point to a Constructor
	/// </summary>
	public class Constructor : ActionClass
	{
		#region fields and constructors
		public Constructor(ClassPointer owner)
			: base(owner)
		{
		}
		#endregion

	}
}
