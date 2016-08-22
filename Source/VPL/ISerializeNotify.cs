/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;

namespace VPL
{
	public interface ISerializeNotify
	{
		/// <summary>
		/// true: is in the process of reading properties.
		/// To be set by the serializer
		/// </summary>
		bool ReadingProperties { get; set; }
	}
}
