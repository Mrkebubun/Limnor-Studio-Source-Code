/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionMan
{
	public interface IParentNode
	{
		void LoadNextLevel(TreeNodeLoader level);
		bool NextLevelLoaded { get; }
	}
}
