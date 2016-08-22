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
	public sealed class ProjectEnvironment
	{
		private static bool _rm;
		/// <summary>
		/// run mode must be in a separate app domain
		/// </summary>

		static public bool RunMode { get { return _rm; } set { _rm = value; } }
		private ProjectEnvironment()
		{
		}
	}
}
