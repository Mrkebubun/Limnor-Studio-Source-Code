/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlUtility
{
	public class NotForLightReadAttribute : Attribute
	{
		private bool _forLightRead;
		public NotForLightReadAttribute()
		{
		}
		public NotForLightReadAttribute(bool forLightRead)
		{
			_forLightRead = forLightRead;
		}
		public bool ForLightRead
		{
			get
			{
				return _forLightRead;
			}
		}
	}
}
