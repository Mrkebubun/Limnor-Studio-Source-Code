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
	public interface IDistributeFile
	{
		string FileName { get; set; }
		string GetTargetPathWithToken();
	}
	public interface IDistributor
	{
		void CollectAddedFiles(string contentFile, List<IDistributeFile> distrubtedFiles);
		string Compile(EnumPlatform pl, IShowMessage log, string xmlContentFile, string targetName, string mode);
	}
	public enum EnumPlatform { x64 = 0, x32 }
}
