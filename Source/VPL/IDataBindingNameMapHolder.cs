/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;

namespace VPL
{
	public interface IDataBindingNameMapHolder
	{
		Dictionary<string, string> DataBindNameMap { get; }
	}
}
