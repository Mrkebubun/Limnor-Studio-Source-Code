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
	public delegate void EventHandlerTreeNodeX(object sender, TreeNodeXEventArgs e);
	public delegate void EventHandlerTreeNodeShortcut(object sender, TreeNodeShortcutEventArgs e);
	public delegate void EventHandlerTreeNodeValue(object sender, TreeNodeValueEventArgs e);
	public delegate void MouseEventHandlerTreeNodeShortcut(object sender, TreeNodeShortcutMouseClickEventArgs e);
	public delegate void MouseEventHandlerTreeNodeX(object sender, TreeNodeXMouseClickEventArgs e);
	public delegate void MouseEventHandlerTreeNodeValue(object sender, TreeNodePropertyMouseClickEventArgs e);
	public class TreeNodeXEventArgs : EventArgs
	{
		private TreeNodeX _node;
		private TreeViewAction _act;
		public TreeNodeXEventArgs(TreeNodeX node, TreeViewAction action)
		{
			_node = node;
			_act = action;
		}
		public TreeViewAction Action
		{
			get
			{
				return _act;
			}
		}
		public TreeNodeX Node
		{
			get
			{
				return _node;
			}
		}
	}
	public class TreeNodeShortcutEventArgs : EventArgs
	{
		private TreeNodeShortcut _node;
		private TreeViewAction _act;
		public TreeNodeShortcutEventArgs(TreeNodeShortcut node, TreeViewAction action)
		{
			_node = node;
			_act = action;
		}
		public TreeViewAction Action
		{
			get
			{
				return _act;
			}
		}
		public TreeNodeShortcut Node
		{
			get
			{
				return _node;
			}
		}
	}
	public class TreeNodeValueEventArgs : EventArgs
	{
		private TreeNodeValue _node;
		private TreeViewAction _act;
		public TreeNodeValueEventArgs(TreeNodeValue node, TreeViewAction action)
		{
			_node = node;
			_act = action;
		}
		public TreeViewAction Action
		{
			get
			{
				return _act;
			}
		}
		public TreeNodeValue Node
		{
			get
			{
				return _node;
			}
		}
	}

	public class TreeNodeShortcutMouseClickEventArgs : TreeNodeMouseClickEventArgs
	{
		private TreeNodeShortcut _node;
		public TreeNodeShortcutMouseClickEventArgs(TreeNodeShortcut node, MouseButtons buttons, int clicks, int x, int y)
			: base(node, buttons, clicks, x, y)
		{
			_node = node;
		}
		public TreeNodeShortcutMouseClickEventArgs(TreeNodeShortcut node, TreeNodeMouseClickEventArgs e)
			: base(node, e.Button, e.Clicks, e.X, e.Y)
		{
			_node = node;
		}
		public TreeNodeShortcut ShortcutNode
		{
			get
			{
				return _node;
			}
		}
	}
	public class TreeNodeXMouseClickEventArgs : TreeNodeMouseClickEventArgs
	{
		private TreeNodeX _node;
		public TreeNodeXMouseClickEventArgs(TreeNodeX node, MouseButtons buttons, int clicks, int x, int y)
			: base(node, buttons, clicks, x, y)
		{
			_node = node;
		}
		public TreeNodeXMouseClickEventArgs(TreeNodeX node, TreeNodeMouseClickEventArgs e)
			: base(node, e.Button, e.Clicks, e.X, e.Y)
		{
			_node = node;
		}
		public TreeNodeX CategoryNode
		{
			get
			{
				return _node;
			}
		}
	}
	public class TreeNodePropertyMouseClickEventArgs : TreeNodeMouseClickEventArgs
	{
		private TreeNodeValue _node;
		public TreeNodePropertyMouseClickEventArgs(TreeNodeValue node, MouseButtons buttons, int clicks, int x, int y)
			: base(node, buttons, clicks, x, y)
		{
			_node = node;
		}
		public TreeNodePropertyMouseClickEventArgs(TreeNodeValue node, TreeNodeMouseClickEventArgs e)
			: base(node, e.Button, e.Clicks, e.X, e.Y)
		{
			_node = node;
		}
		public TreeNodeValue PropertyNode
		{
			get
			{
				return _node;
			}
		}
	}
}
