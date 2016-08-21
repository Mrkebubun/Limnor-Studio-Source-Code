/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LimnorDatabase
{
	static class NativeWIN32
	{
		[DllImport("Kernel32.dll")]
		public static extern uint GetTickCount();
	}
}
