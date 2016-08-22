/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace SolutionMan.Solution
{
	public class SolutionSelection
	{
		private Dictionary<string, string> _replaceNames;
		public SolutionSelection()
		{
			_replaceNames = new Dictionary<string, string>();
		}
		public bool CreateSolutionFolder;
		public string SolutionName;
		public string ProjectName;
		public string Location; //folder
		public string ProjectTemplate;
		public Dictionary<string, string> ReplaceNames
		{
			get
			{
				return _replaceNames;
			}
		}
	}
}
