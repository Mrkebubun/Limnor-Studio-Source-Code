/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Windows
{
	public class WindowManagerException : Exception
	{
		public WindowManagerException()
		{
		}
		public WindowManagerException(string message)
			: base(message)
		{
		}
	}
}
