/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Limnor.TreeViewExt
{

	[Flags]
	public enum EnumTreeViewMenu
	{
		AddRootNode = 1,
		AddSubNode = 2,
		MoveNodeToRoot = 4,
		AttachValue = 8,
		Refresh = 16,
		Properties = 32,
		Copy = 64,
		Paste = 128,
		RemoveSelectedValue = 256,
		RemoveSelectedNode = 512,
		RemoveSelectedShortcut = 1024,
		MoveNodeUp = 2048,
		MoveNodeDown = 4096
	}
}
