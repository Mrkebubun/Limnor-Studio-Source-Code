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
	/// <summary>
	/// for resetting context menu
	/// </summary>
	public interface IPropertyListChangeNotify
	{
		void OnPropertyListChanged(object owner);
	}
	public interface IPropertyListchangeable
	{
		void SetPropertyListNotifyTarget(IPropertyListChangeNotify target);
		void OnPropertyListChanged();
	}
}
