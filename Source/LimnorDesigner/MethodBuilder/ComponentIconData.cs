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

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// for showing a component as an icon in  designer
	/// </summary>
	public class ComponentIconData
	{
		private IObjectPointer _pointer;
		public ComponentIconData()
		{
		}
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				return _pointer;
			}
			set
			{
				_pointer = value;
			}
		}

	}
}
