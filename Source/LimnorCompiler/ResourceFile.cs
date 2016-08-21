/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorCompiler
{
	public enum EnumResourceBuildType { Embed, Link }
	[Serializable]
	public class ResourceFile
	{
		EnumResourceBuildType _buildType = EnumResourceBuildType.Embed;
		public ResourceFile()
		{
		}
		public string Name
		{
			get
			{
				return System.IO.Path.GetFileName(ResourceFilename);
			}
		}
		public string ResourceFilename { get; set; }
		public string TargetFilename { get; set; }
		public string KeyName { get; set; }
		public EnumResourceBuildType BuildAction { get { return _buildType; } set { _buildType = value; } }
	}
}
