/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner.MethodBuilder
{
	public class FileName
	{
		private string _filename;
		private string _name;
		public FileName(string file)
		{
			_filename = file;
		}
		public FileName(string file, string name)
		{
			_filename = file;
			_name = name;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(_name))
				return System.IO.Path.GetFileName(_filename);
			return _name;
		}
		public string Filename
		{
			get
			{
				return _filename;
			}
		}
	}
}
