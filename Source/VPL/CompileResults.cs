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
	public class CompileResults
	{
		public CompileResults()
		{
		}
		public bool HasErrors { get; set; }
		public int CompiledComponents { get; set; }
		public int SkippedComponents { get; set; }
		public bool CopyBinFileFailed { get; set; }
	}
}
