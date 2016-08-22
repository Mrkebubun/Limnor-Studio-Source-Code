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
	public class SupportFileAttribute : Attribute
	{
		private string[] _files;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileList">file name separated by semicolumn</param>
		public SupportFileAttribute(string fileList)
		{
			_files = fileList.Split(';');
		}
		public IList<string> Filenames
		{
			get
			{
				return _files;
			}
		}
	}
}
