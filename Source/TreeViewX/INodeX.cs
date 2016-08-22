/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Limnor.TreeViewExt
{
	/// <summary>
	/// implemented by TreeNodeX and TreeNodeValue
	/// </summary>
	public interface INodeX
	{
		TreeNode CreatePointer();
		bool IsShortcut { get; }
		Guid TreeNodeId { get; set; }
	}
}
