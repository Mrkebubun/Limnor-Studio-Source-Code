/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
	public class LinkLineEnds
	{
		private LinkLineNodeOutPort _startNode;
		private LinkLineNodeInPort _endNode;
		public LinkLineEnds(LinkLineNodeOutPort startNode, LinkLineNodeInPort endNode)
		{
			_startNode = startNode;
			_endNode = endNode;
		}
		public LinkLineNodeOutPort StartNode
		{
			get
			{
				return _startNode;
			}
		}
		public LinkLineNodeInPort EndNode
		{
			get
			{
				return _endNode;
			}
		}
		public bool IsOnTheLine(LinkLineNode lineNode)
		{
			LinkLineNodeOutPort start = lineNode.Start as LinkLineNodeOutPort;
			if (start != null)
			{
				if (start.PortID == _startNode.PortID && start.PortInstanceID == _startNode.PortInstanceID)
				{
					LinkLineNodeInPort end = lineNode.End as LinkLineNodeInPort;
					if (end != null)
					{
						if (end.PortID == _endNode.PortID && end.PortInstanceID == _endNode.PortInstanceID)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
