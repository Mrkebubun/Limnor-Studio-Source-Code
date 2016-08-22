/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace VOBMsg
{
	public sealed class VobMsg
	{
		const int WM_USER = 0x0400;
		public const int WM_USERMSG = WM_USER + 1;
		private VobMsg()
		{
		}

	}
}
