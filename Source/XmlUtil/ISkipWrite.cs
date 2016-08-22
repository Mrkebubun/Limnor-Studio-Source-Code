/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlUtility
{
	public interface ISkipWrite
	{
		bool SkipSerialize { get; }
	}
}
