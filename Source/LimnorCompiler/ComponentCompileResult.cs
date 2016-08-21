/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorCompiler
{
	class ComponentCompileResult
	{
		public ComponentCompileResult()
		{
		}
		public string SourceFile { get; set; }
		public bool IsForm { get; set; }
		public string ResourceFile { get; set; }
		public string ResourceFileX { get; set; }
		public string SourceFileX { get; set; }
		public List<ResourceFile> ResourceFiles { get; set; }
		public bool UseResources
		{
			get;
			set;
		}
		public bool UseResourcesX
		{
			get;
			set;
		}
	}
}
