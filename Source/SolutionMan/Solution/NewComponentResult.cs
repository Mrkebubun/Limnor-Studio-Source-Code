/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;

namespace SolutionMan.Solution
{
	public class NewComponentResult
	{
		private string _file;
		private bool _open;
		public NewComponentResult(string file, bool open)
		{
			_file = file;
			_open = open;
		}
		public string ClassFile
		{
			get
			{
				return _file;
			}
		}
		public bool Open
		{
			get
			{
				return _open;
			}
		}
	}
}
