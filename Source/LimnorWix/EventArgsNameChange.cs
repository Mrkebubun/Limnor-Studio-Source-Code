/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorWix
{
	class EventArgsNameChange:EventArgs
	{
		public EventArgsNameChange(string oldname, string newname)
		{
			OldName = oldname;
			NewName = newname;
		}
		public string OldName { get; private set; }
		public string NewName { get; private set; }
	}
}
