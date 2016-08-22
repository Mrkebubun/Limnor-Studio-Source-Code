/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class XDesignerAttribute : Attribute
	{
		private Type _type;
		public XDesignerAttribute(Type typeToDesign)
		{
			_type = typeToDesign;
		}
		public Type TypeToDesign
		{
			get
			{
				return _type;
			}
		}
	}
}
