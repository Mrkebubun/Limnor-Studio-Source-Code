/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MathExp
{
	abstract class LinkLineUndo : IUndoUnit
	{
		UInt32 _nodeKey;
		IUndoHost _host;
		public LinkLineUndo(IUndoHost host, UInt32 nodeKey)
		{
			if (nodeKey == 0)
			{
				throw new MathException("Cannot create LinkLineUndo with objKey=0");
			}
			_nodeKey = nodeKey;
			_host = host;
		}
		protected LinkLineNode GetLineNode(UInt32 key)
		{
			return _host.GetUndoControl(key) as LinkLineNode;
		}
		protected abstract void OnApply(LinkLineNode node);
		#region IUndoUnit Members

		public void Apply()
		{
			bool b = _host.DisableUndo;
			_host.DisableUndo = true;
			LinkLineNode c = GetLineNode(_nodeKey);
			if (c != null)
			{
				OnApply(c);
			}
			_host.DisableUndo = b;
		}

		#endregion
	}
	class LinkLineUndoDelete : LinkLineUndo
	{
		public LinkLineUndoDelete(IUndoHost host, UInt32 node)
			: base(host, node)
		{
		}
		protected override void OnApply(LinkLineNode node)
		{
			node.Delete();
		}
	}
	class LinkLineUndoAdd : LinkLineUndo
	{
		private Point _point;
		public LinkLineUndoAdd(IUndoHost host, UInt32 node, Point p)
			: base(host, node)
		{
			_point = p;
		}
		protected override void OnApply(LinkLineNode node)
		{
			node.InsertNode(_point.X, _point.Y);
		}
	}
	/// <summary>
	/// for undo a delete: re-insert the deleted node
	/// </summary>
	class LinkLineUndoJoin : LinkLineUndo
	{
		private UInt32 _next;
		private UInt32 _deleted;
		private Point _point;
		public LinkLineUndoJoin(IUndoHost host, UInt32 prev, UInt32 next, UInt32 deleted, Point pointOfDeleted)
			: base(host, prev)
		{
			_next = next;
			_deleted = deleted;
			_point = pointOfDeleted;
		}
		protected override void OnApply(LinkLineNode node)
		{
			LinkLineNode prevNode = node;
			if (prevNode != null)
			{
				LinkLineNode nextNode = GetLineNode(_next);
				if (nextNode != null)
				{
					if (prevNode.Line != null)
					{
						if (prevNode.Line.EndPoint == nextNode)
						{
							prevNode.ClearLine();
						}
					}
					if (nextNode.Line != null)
					{
						if (nextNode.Line.StartPoint == prevNode)
						{
							nextNode.ClearLine();
						}
					}
					LinkLineNode newNode = new LinkLineNode(nextNode, prevNode);
					newNode.Location = _point;
					newNode.ActiveDrawingID = _deleted;
					nextNode.Parent.Controls.Add(newNode);
					if (prevNode.Line == null)
					{
						prevNode.CreateForwardLine();
						newNode.CreateForwardLine();
					}
					else
					{
						newNode.CreateBackwardLine();
						nextNode.CreateBackwardLine();
					}
					ActiveDrawing.RefreshControl(nextNode.Parent);
				}
			}
		}
	}
	/// <summary>
	/// for disconnect redo
	/// </summary>
	class LinkLineUndoBreak : LinkLineUndo
	{
		Point _point;
		UInt32 _node1;
		UInt32 _node2;
		public LinkLineUndoBreak(IUndoHost host, UInt32 node, UInt32 n1, UInt32 n2, Point p)
			: base(host, node)
		{
			_point = p;
			_node1 = n1;
			_node2 = n2;
		}
		protected override void OnApply(LinkLineNode node)
		{
			node.BreakLine(_point, ref _node1, ref _node2);
			ActiveDrawing.RefreshControl(node.Parent);
		}
	}
	/// <summary>
	/// for undo disconnect
	/// </summary>
	class LinkLineUndoReconnect : LinkLineUndo
	{
		UInt32 _node1;
		UInt32 _node2;
		public LinkLineUndoReconnect(IUndoHost host, UInt32 node, UInt32 n1, UInt32 n2)
			: base(host, node)
		{
			_node1 = n1;
			_node2 = n2;
		}
		protected override void OnApply(LinkLineNode node)
		{
			LinkLineNode node1 = GetLineNode(_node1);
			LinkLineNode node2 = GetLineNode(_node2);
			LinkLineNode start = (LinkLineNode)node1.PrevNode;
			LinkLineNode end = (LinkLineNode)node2.NextNode;
			node1.ClearLine();
			node2.ClearLine();
			node.Parent.Controls.Remove(node1);
			node.Parent.Controls.Remove(node2);
			start.SetNext(end);
			end.SetPrevious(start);
			if (start.Line == null)
			{
				start.CreateForwardLine();
			}
			else
			{
				end.CreateBackwardLine();
			}
			if (start.LinkedOutPort != null)
			{
				if (end.LinkedInPort != null)
				{
					start.LinkedOutPort.LinkedPortID = end.LinkedInPort.PortID;
					end.LinkedInPort.LinkedPortID = start.LinkedOutPort.PortID;
				}
			}
			ActiveDrawing.RefreshControl(node.Parent);
		}
	}
}
