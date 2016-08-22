/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer by WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace WixLib
{
	class SpecialFolderInfo
	{
		public SpecialFolderInfo(string folder,string folder64, string component, string guid)
		{
			FolderName = folder;
			FolderName64 = folder64;
			ComponentName = component;
			GuidString = guid;
		}
		public string FolderName { get; private set; }
		public string FolderName64 { get; private set; }
		public string ComponentName { get; private set; }
		public string GuidString { get; private set; }
	}
}
