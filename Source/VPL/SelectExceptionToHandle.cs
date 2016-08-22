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
	public sealed class SelectExceptionToHandle : Exception
	{
		public SelectExceptionToHandle()
			: base("Select an exception to be handled")
		{
		}
	}
}
