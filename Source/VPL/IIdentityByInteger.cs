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
	/// ID by integer
	/// </summary>
	public interface IIdentityByInteger
	{
		/// <summary>
		/// usually it is formed by 32-bit class ID and member ID
		/// </summary>
		UInt64 WholeId { get; }
	}
}
