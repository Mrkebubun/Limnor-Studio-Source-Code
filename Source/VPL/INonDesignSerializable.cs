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
	/// For controls added to the designer but not in design mode. Indicating whether it should be serialized.
	/// Implemented by ActiveDrawing
	/// </summary>
	public interface INonDesignSerializable
	{
		bool ShouldSerialize { get; }
	}
}
