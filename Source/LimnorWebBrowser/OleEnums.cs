/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Wrapper of Web Browser Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorWebBrowser
{
	public enum OLECMDF
	{
		OLECMDF_DEFHIDEONCTXTMENU = 0x20,
		OLECMDF_ENABLED = 2,
		OLECMDF_INVISIBLE = 0x10,
		OLECMDF_LATCHED = 4,
		OLECMDF_NINCHED = 8,
		OLECMDF_SUPPORTED = 1
	}

	public enum OLECMDID
	{
		OLECMDID_PAGESETUP = 8,
		OLECMDID_PRINT = 6,
		OLECMDID_PRINTPREVIEW = 7,
		OLECMDID_PROPERTIES = 10,
		OLECMDID_SAVEAS = 4,
	}
	public enum OLECMDEXECOPT
	{
		OLECMDEXECOPT_DODEFAULT = 0,
		OLECMDEXECOPT_DONTPROMPTUSER = 2,
		OLECMDEXECOPT_PROMPTUSER = 1,
		OLECMDEXECOPT_SHOWHELP = 3
	}
}
