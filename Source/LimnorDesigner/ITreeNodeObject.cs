/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	interface ITreeNodeObject
	{
		MenuItem[] GetContextMenuItems(bool bReadOnly);
		bool IsInDesign { get; }
	}
	interface ITreeNodeObjectSelection
	{
		void OnNodeAfterSelection(TreeViewEventArgs e);
	}
	interface ITreeNodeLabelEdit
	{
		bool AlloLabelEdit { get; }
		void OnBeforeLabelEdit(NodeLabelEditEventArgs e);
		void OnAfterLabelEdit(NodeLabelEditEventArgs e);
	}
}
