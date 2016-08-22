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
	public interface ILibCaller
	{
		void ShowMessage(string message);
	}
}
