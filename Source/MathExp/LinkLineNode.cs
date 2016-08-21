/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.CodeDom;
using MathExp.RaisTypes;
using VPLDrawing;
using VPL;

namespace MathExp
{
	enum EnumTestStartType
	{
		Unknown = 0,
		Inport,
		OutPort
	}
	public interface IPortOwner
	{
		bool IsDummyPort { get; }
		void AddOutPort(LinkLineNodeOutPort port);
		void AddInPort(LinkLineNodeInPort port);
		void RemovePort(LinkLineNodePort port);
		int InPortCount { get; }
		string TraceInfo { get; }
		UInt32 PortOwnerID { get; }
	}
	public interface ILinkLineNode : IObjectCenter, ICloneable
	{
		ILinkLineNode NextNode { get; }
		ILinkLineNode PrevNode { get; }
		int Left { get; set; }
		int Top { get; set; }
		LinkLine Line { get; }
		void ClearLine();
	}
	public delegate void fnLinkVariable(ILinkLineNodePort port, UInt32 id);

	public interface ILinkLineNodePort
	{
		Point DefaultNextNodePosition();
		enumPositionType PositionType { get; set; }
		bool FixedLocation { get; set; }
		Point[] Corners { get; set; }
		int Position { get; set; }
		Control Owner { get; set; }
		RelativeDrawing Label { get; }
		UInt32 PortID { get; }
		UInt32 LinkedPortID { get; set; }
		void SetVariable(IVariable variable);
		Color LineColor { get; }
	}
	public class NodeData
	{
		public LinkLineNode Node;
		public object Data;
		public NodeData(LinkLineNode node, object data)
		{
			Node = node;
			Data = data;
		}
	}
	public class EventArgcLinkPorts : EventArgs
	{
		private LinkLineNodeInPort _inPort;
		private LinkLineNodeOutPort _outPort;
		public EventArgcLinkPorts(LinkLineNodeInPort inPort, LinkLineNodeOutPort outPort)
		{
			_inPort = inPort;
			_outPort = outPort;
		}
		public LinkLineNodeInPort InPort
		{
			get
			{
				return _inPort;
			}
		}
		public LinkLineNodeOutPort OutPort
		{
			get
			{
				return _outPort;
			}
		}
	}
	public delegate void EventHandlerLinkPorts(object sender, EventArgcLinkPorts e);
	public class LinkLineNode : ActiveDrawing, ILinkLineNode, ICloneable
	{
		#region Fields and constructors
		public static event EventHandlerLinkPorts PortsLinked;
		protected LinkLineNode previous = null;
		protected LinkLineNode next = null;
		protected LinkLine line = null;//in-port:from previous to this; out-pot: from this to next
		public const int dotSize = 10;
		private Rectangle linkIndicate;
		private Pen penLinkIndicator;
		private int indicatorSize = 3;
		private int linkDetectSenseSize = 10;
		public LinkLineNode()
		{
			Init();
		}

		public LinkLineNode(LinkLineNode nextNode, LinkLineNode previousNode)
		{
			Init();
			next = nextNode;
			if (nextNode != null)
			{
				nextNode.SetPrevious(this);
			}
			previous = previousNode;
			if (previousNode != null)
			{
				previousNode.SetNext(this);
			}
		}
		#endregion
		#region properties
		public virtual bool CanStartLink
		{
			get
			{
				return true;
			}
		}
		public virtual bool CanCreateDuplicatedLink
		{
			get
			{
				return false;
			}
		}
		public override bool NeedSaveSize
		{
			get
			{
				return false;
			}
		}
		public LinkLineNodeInPort LinkedInPort
		{
			get
			{
				if (this is LinkLineNodeInPort)
					return (LinkLineNodeInPort)this;
				if (this == next)
				{
					next = null;
				}
				if (this == previous)
				{
					previous = null;
				}
				if (next != null)
				{
					return next.LinkedInPort;
				}
				return null;
			}
		}
		public LinkLineNodeOutPort LinkedOutPort
		{
			get
			{
				if (this is LinkLineNodeOutPort)
					return (LinkLineNodeOutPort)this;
				if (previous != null)
				{
					return previous.LinkedOutPort;
				}

				return null;
			}
		}
		/// <summary>
		/// go alomg previous until end
		/// </summary>
		public LinkLineNode Start
		{
			get
			{
				if (previous == null)
					return this;
				//add cyclic detection
				List<LinkLineNode> ps = new List<LinkLineNode>();
				ps.Add(this);
				LinkLineNode p = this.previous;
				while (true)
				{
					if (ps.Contains(p))
					{
						throw new MathException("Error getting line start: cyclic line detected");
					}
					if (p.previous == null)
						return p;
					ps.Add(p);
					p = p.previous;
				}
			}
		}
		/// <summary>
		/// go along next until end
		/// </summary>
		public LinkLineNode End
		{
			get
			{
				if (next == null)
					return this;
				//add cyclic detection
				List<LinkLineNode> ps = new List<LinkLineNode>();
				ps.Add(this);
				LinkLineNode p = this.next;
				while (true)
				{
					if (ps.Contains(p))
					{
						throw new MathException("Error getting line end: cyclic line detected");
					}
					if (p.next == null)
						return p;
					ps.Add(p);
					p = p.next;
				}
			}
		}
		public LinkLine Line
		{
			get
			{
				return line;
			}
		}
		public ILinkLineNode PrevNode
		{
			get
			{
				return previous;
			}
		}
		public ILinkLineNode NextNode
		{
			get
			{
				return next;
			}
		}


		private bool _selected;
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
			}
		}
		protected Rectangle CircleBound
		{
			get
			{
				return new Rectangle(1, 1, dotSize - 2, dotSize - 2);
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			ClearAllLinks();//remove defaults
			XmlNode nd = node.SelectSingleNode("PrevNode");
			if (nd != null)
			{
				previous = (LinkLineNode)XmlSerialization.ReadFromXmlNode(serializer, nd);
				previous.SetNext(this);
			}
			nd = node.SelectSingleNode("NextNode");
			if (nd != null)
			{
				next = (LinkLineNode)XmlSerialization.ReadFromXmlNode(serializer, nd);
				next.SetPrevious(this);
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			//* saving starts from all ports.
			//* save all previous nodes except the linked output port.
			//* save next only when it is not linked
			//
			//linked with an in-port
			if (LinkedInPort != null)
			{
				//prevent double-saving: do not save ports because ports are starting points
				if (previous != null && !(previous is LinkLineNodePort))
				{
					XmlSerialization.WriteToChildXmlNode(serializer, node, "PrevNode", previous);
				}
			}
			else //only save in one way
			{
				//linked with an out-port but not linked with in-port: open ended
				if (LinkedOutPort != null)
				{
					//prevent double-saving: do not save ports because ports are starting points
					if (next != null && !(next is LinkLineNodePort))
					{
						//prevent double-saving: save next only when it is not linked
						XmlSerialization.WriteToChildXmlNode(serializer, node, "NextNode", next);
					}
				}
			}
		}
		#endregion
		#region Serialization
		#endregion
		#region methods
		public static void JoinToEnd(LinkLineNode startNode, LinkLineNode endNode)
		{
			if (!(startNode is LinkLineNodePort) && !(endNode is LinkLineNodePort))
			{
				startNode.Location = endNode.Location;
			}
			makeJoin(startNode, endNode);
		}
		/// <summary>
		/// merge two line node into one node and hence join the two lines,
		/// remove one of the two nodes.
		/// </summary>
		/// <param name="startNode">one node to join, not neccessarily the start of the line</param>
		/// <param name="endNode">one node to join, not neccessarily the end of the line</param>
		private static void makeJoin(LinkLineNode startNode, LinkLineNode endNode)
		{
			Control parent = startNode.Parent;
			//identify the starting and ending of the line for the join===
			LinkLineNode linkStart; //starting line node for linking
			LinkLineNode linkend;   //ending line node for linking
			//if the outport/inport is already linked to an inport/outport, create a new outport/inport =====
			if (startNode is LinkLineNodeOutPort && startNode.NextNode != null)
			{
				LinkLineNodeOutPort po = startNode as LinkLineNodeOutPort;
				po.ConstructorParameters = new object[] { po.PortOwner };
				LinkLineNodeOutPort newOut = (LinkLineNodeOutPort)po.Clone();
				newOut.ResetActiveDrawingID();
				parent.Controls.Add(newOut);
				newOut.Owner = po.Owner;
				newOut.LabelVisible = false;
				newOut.SetLabelOwner();
				po.PortOwner.AddOutPort(newOut);
				startNode = newOut;
			}
			else if (endNode is LinkLineNodeOutPort && endNode.NextNode != null)
			{
				LinkLineNodeOutPort po = endNode as LinkLineNodeOutPort;
				po.ConstructorParameters = new object[] { po.PortOwner };
				LinkLineNodeOutPort newOut = (LinkLineNodeOutPort)po.Clone();
				newOut.ResetActiveDrawingID();
				parent.Controls.Add(newOut);
				newOut.Owner = po.Owner;
				newOut.LabelVisible = false;
				newOut.SetLabelOwner();
				po.PortOwner.AddOutPort(newOut);
				endNode = newOut;
			}
			else if (startNode is LinkLineNodeInPort && startNode.PrevNode != null)
			{
				LinkLineNodeInPort pi = startNode as LinkLineNodeInPort;
				pi.ConstructorParameters = new object[] { pi.PortOwner };
				LinkLineNodeInPort newIn = (LinkLineNodeInPort)pi.Clone();
				newIn.ResetActiveDrawingID();
				parent.Controls.Add(newIn);
				newIn.Owner = pi.Owner;
				newIn.LabelVisible = false;
				newIn.SetLabelOwner();
				pi.PortOwner.AddInPort(newIn);
				startNode = newIn;
			}
			else if (endNode is LinkLineNodeInPort && endNode.PrevNode != null)
			{
				LinkLineNodeInPort pi = endNode as LinkLineNodeInPort;
				pi.ConstructorParameters = new object[] { pi.PortOwner };
				LinkLineNodeInPort newIn = (LinkLineNodeInPort)pi.Clone();
				newIn.ResetActiveDrawingID();
				parent.Controls.Add(newIn);
				newIn.Owner = pi.Owner;
				newIn.LabelVisible = false;
				newIn.SetLabelOwner();
				pi.PortOwner.AddInPort(newIn);
				endNode = newIn;
			}
			//===============================================================
			//identifying starting and ending nodes, remove one of the two nodes
			if (!(startNode is LinkLineNodeInPort) && startNode.next == null)
			{
				//startNode is the starting node
				linkend = endNode as LinkLineNode;
				if (startNode.previous != null)
				{
					//use the previous node as the starting node
					//remove the current start node
					linkStart = startNode.previous;
					parent.Controls.Remove(startNode);
				}
				else if (linkend.next != null)
				{
					//use the next of the end node as the ending node
					//remove the current end node
					linkStart = startNode;
					linkend = linkend.next;
					parent.Controls.Remove((Control)endNode);
				}
				else
				{
					//startNode is an outport and endNode is an inport
					linkStart = startNode;
				}
			}
			else
			{
				//endNode is the starting node
				linkStart = endNode as LinkLineNode;
				if (startNode.next != null)
				{
					//use the next of the startNode as the ending node
					//remove the startNode
					linkend = startNode.next;
					parent.Controls.Remove((Control)startNode);
				}
				else if (linkStart.previous != null)
				{
					//use the previous node of endNode as the starting node
					//remove endNode
					linkend = startNode;
					linkStart = linkStart.previous;
					parent.Controls.Remove((Control)endNode);
				}
				else
				{
					//startNode is an inport and endNode is an outport
					linkend = startNode;
				}
			}
			if (linkStart != linkend)
			{
				//make the link
				bool b1 = false;
				bool b2 = false;
				if (linkStart.LinkedOutPort != null)
				{
					if (linkStart.LinkedOutPort.RemoveLineJoint)
					{
						b1 = true;
					}
				}
				else if (linkStart.LinkedInPort != null)
				{
					if (linkStart.LinkedInPort.RemoveLineJoint)
					{
						b1 = true;
					}
				}
				if (linkend.LinkedInPort != null)
				{
					b2 = linkend.LinkedInPort.RemoveLineJoint;
				}
				else if (linkend.LinkedOutPort != null)
				{
					b2 = linkend.LinkedOutPort.RemoveLineJoint;
				}

				if (b2 && b1)
				{
					if (!(linkStart is LinkLineNodePort))
					{
						if (linkStart.previous != null)
						{
							LinkLineNode nl = linkStart;
							linkStart = linkStart.previous;
							parent.Controls.Remove(nl);
						}
					}
					if (!(linkend is LinkLineNodePort))
					{
						if (linkend.next != null)
						{
							LinkLineNode nl = linkend;
							linkend = linkend.next;
							parent.Controls.Remove(nl);
						}
					}
				}
				joinNodes(linkStart, linkend);
				ActiveDrawing.RefreshControl(parent);
			}
		}
		/// <summary>
		/// join two line node together
		/// </summary>
		/// <param name="linkStart"></param>
		/// <param name="linkend"></param>
		private static void joinNodes(LinkLineNode linkStart, LinkLineNode linkend)
		{
			linkStart.SetNext(linkend);
			linkend.SetPrevious(linkStart);
			linkStart.LinkedInPort.LinkedPortID = linkStart.LinkedOutPort.PortID;
			linkStart.LinkedInPort.LinkedPortInstanceID = linkStart.LinkedOutPort.PortInstanceID;
			linkStart.LinkedOutPort.LinkedPortID = linkStart.LinkedInPort.PortID;
			linkStart.LinkedOutPort.LinkedPortInstanceID = linkStart.LinkedInPort.PortInstanceID;
			if (linkStart.Line != null)
			{
				if (linkStart.line.StartPoint == linkStart)
				{
					//start owns a line starting from it, it can be reused
					//by adjusting the end point
					if (linkend.Line != null)
					{
						//if linkend owns a line pointing to it then remove it
						if (linkend.Line.EndPoint == linkend)
						{
							linkend.ClearLine();
						}
					}
					linkStart.Line.SetEnd(linkend);
				}
				else if (linkStart.line.EndPoint == linkStart)
				{
					//start owns a line pointing to it, the new line has to be owned by the end node
					if (linkend.Line != null)
					{
						if (linkend.Line.EndPoint == linkend)
						{
							//end owns a line pointing to it. it can be reused by
							//adjusting the start point
							linkend.Line.SetStart(linkStart);
						}
						else if (linkend.Line.StartPoint == linkend)
						{
							throw new MathException("conflict line link directions, a new line node is needed");
						}
					}
					else
					{
						linkend.CreateBackwardLine();
					}
				}
				else
				{
					throw new MathException("isolated line-break detected");
				}
			}
			else if (linkend.Line != null)
			{
				//the start node does not own a line. use it to create new line to point to the end
				//the end node owns a line
				if (linkend.Line.EndPoint == linkend)
				{
					//the end owns a line pointing to it, remove the line
					linkend.ClearLine();
				}
				linkStart.CreateForwardLine();
			}
			else
			{
				//both start and end do not own lines
				linkStart.CreateForwardLine();
			}
			if (linkStart.Line != null)
			{
				linkStart.Line.AdjustLineEndVisibility();
			}
			if (linkStart.PrevNode != null)
			{
				if (linkStart.PrevNode.Line != null)
				{
					linkStart.PrevNode.Line.AdjustLineEndVisibility();
				}
			}
			if (linkStart.NextNode != null)
			{
				if (linkStart.NextNode.Line != null)
				{
					linkStart.NextNode.Line.AdjustLineEndVisibility();
				}
			}
			if (linkend.Line != null)
			{
				linkend.Line.AdjustLineEndVisibility();
			}
			if (linkend.NextNode != null)
			{
				if (linkend.NextNode.Line != null)
				{
					linkend.NextNode.Line.AdjustLineEndVisibility();
				}
			}
			if (linkend.PrevNode != null)
			{
				if (linkend.PrevNode.Line != null)
				{
					linkend.PrevNode.Line.AdjustLineEndVisibility();
				}
			}
			if (PortsLinked != null)
			{
				PortsLinked(null, new EventArgcLinkPorts(linkStart.LinkedInPort, linkend.LinkedOutPort));
			}
		}
		public void ClearLine()
		{
			line = null;
		}
		public void ClearAllLinks()
		{
			line = null;
			next = null;
			previous = null;
		}
		public void CreateForwardLine()
		{
			if (next != null)
			{
				//remove existing line 
				if (next is LinkLineNodePort)
				{
					next.ClearLine();
				}
				else if (next.Line != null)
				{
					if (next.Line.StartPoint == this)
					{
						next.ClearLine();
					}
				}
				line = new LinkLine();
				ILinkLineNodePort port = next.End as ILinkLineNodePort;
				line = new LinkLine();
				if (port != null)
				{
					if (port.LineColor != Color.Empty)
					{
						line.SetLineColor(port.LineColor);
					}
				}
				//line from this to next
				line.AssignMapObjects(this, next);
				line.EndPointVisible = (next.NextNode == null);
				line.StartPointVisible = (this.PrevNode == null);
				if (this.Parent != null)
				{
					line.SetDrawOwner(this.Parent);
				}
			}
		}
		public void CreateBackwardLine()
		{
			if (previous != null)
			{
				//remove existing line 
				if (previous is LinkLineNodePort)
				{
					previous.ClearLine();
				}
				else
				{
					if (previous.Line != null)
					{
						if (previous.Line.EndPoint == this)
						{
							previous.ClearLine();
						}
					}
				}
				line = new LinkLine();
				ILinkLineNodePort port = previous.Start as ILinkLineNodePort;
				if (port != null)
				{
					if (port.LineColor != Color.Empty && port.LineColor != Color.Blue)
					{
						line.SetLineColor(port.LineColor);
					}
				}
				//line from previous to this
				line.AssignMapObjects(previous, this);
				line.EndPointVisible = (this.NextNode == null);
				line.StartPointVisible = (previous.PrevNode == null);
				if (this.Parent != null)
				{
					line.SetDrawOwner(this.Parent);
				}
			}
		}
		public static void CreateLinkLines(LinkLineNodePort[] ports)
		{
			PortCollection ports0 = new PortCollection();
			ports0.AddRange(ports);
			ports0.CreateLinkLines();
		}
		public void Delete()
		{
			if (this.PrevNode != null && this.NextNode != null)
			{
				Diagram dg = this.Parent as Diagram;
				LinkLineNode pn = (LinkLineNode)this.PrevNode;
				LinkLineNode nn = (LinkLineNode)this.NextNode;
				pn.SetNext(nn);
				nn.SetPrevious(pn);
				if (pn.Line == null)
				{
					pn.CreateForwardLine();
					if (nn.Line != null)
					{
						if (nn.Line.StartPoint == this || nn.Line.EndPoint == this)
						{
							nn.ClearLine();
						}
					}
					pn.Line.EndPointVisible = (nn.NextNode == null);
					pn.Line.StartPointVisible = (pn.previous == null);
				}
				else if (nn.Line == null)
				{
					nn.CreateBackwardLine();
					if (pn.Line != null)
					{
						if (pn.Line.StartPoint == this || pn.Line.EndPoint == this)
						{
							pn.ClearLine();
						}
					}
					nn.Line.EndPointVisible = (nn.NextNode == null);
					nn.Line.StartPointVisible = (pn.previous == null);
				}
				else if (pn.Line.EndPoint == this)
				{
					pn.Line.SetEnd(nn);
					if (nn.Line != null)
					{
						if (nn.Line.StartPoint == this || nn.Line.EndPoint == this)
						{
							nn.ClearLine();
						}
					}
					pn.Line.EndPointVisible = (nn.NextNode == null);
					pn.Line.StartPointVisible = (pn.previous == null);
				}
				else if (pn.Line.StartPoint == this)
				{
					pn.Line.SetStart(nn);
					if (nn.Line != null)
					{
						if (nn.Line.StartPoint == this || nn.Line.EndPoint == this)
						{
							nn.ClearLine();
						}
					}
					pn.Line.EndPointVisible = (nn.NextNode == null);
					pn.Line.StartPointVisible = (pn.previous == null);
				}
				else if (nn.Line.StartPoint == this)
				{
					nn.Line.SetStart(pn);
					if (pn.Line != null)
					{
						if (pn.Line.StartPoint == this || pn.Line.EndPoint == this)
						{
							pn.ClearLine();
						}
					}
					nn.Line.EndPointVisible = (nn.NextNode == null);
					nn.Line.StartPointVisible = (pn.previous == null);
				}
				else if (nn.Line.EndPoint == this)
				{
					nn.Line.SetEnd(pn);
					if (pn.Line != null)
					{
						if (pn.Line.StartPoint == this || pn.Line.EndPoint == this)
						{
							pn.ClearLine();
						}
					}
					nn.Line.EndPointVisible = (nn.NextNode == null);
					nn.Line.StartPointVisible = (pn.previous == null);
				}
				else
				{
					//exception case: pn and nn both have lines but not linked to this
					throw new MathException("Incorrect linking lines");
				}
				//
				Control p = this.Parent;
				if (p != null)
				{
					p.Controls.Remove(this);
					ActiveDrawing.RefreshControl(p);
				}
				if (dg != null)
				{
					dg.OnLineNodeDeleted(pn, this, nn);
				}
			}
		}
		public LinkLineNode InsertNode(int x, int y)
		{
			LinkLineNode node = null;
			if (line != null)
			{
				if (line.StartPoint == this)
				{
					//this->next => this->node->next
					//-------------------------------
					//this call makes following arrangements
					//this<-previous.node.next->cureNext
					//cureNext.previous->node
					//this.next->node
					node = new LinkLineNode(next, this);
					//
					node.Left = x - this.Width / 2;
					node.Top = y - this.Width / 2;
					line.SetEnd(node);
					line.EndPointVisible = false;
					node.CreateForwardLine();
					if (this.Parent != null)
					{
						this.Parent.Controls.Add(node);
					}
				}
				else if (line.EndPoint == this)
				{
					int d = this.Width / 2;
					//previous->this => previous->node->this
					//---------------------------------------
					//this call makes following arrangements:
					//previous<-previous.node.next->this
					//this.previous->node
					//previous.next->node
					node = new LinkLineNode(this, previous);
					node.Left = x - d;
					node.Top = y - d;
					//
					line.SetStart(node);
					line.StartPointVisible = false;
					//
					node.CreateBackwardLine();
					if (this.Parent != null)
					{
						this.Parent.Controls.Add(node);
					}
				}
			}
			return node;
		}
		public void BreakLine(Point pt, ref UInt32 nodeId1, ref UInt32 nodeId2)
		{
			LinkLineNode nodeStart = (LinkLineNode)this.Line.StartPoint;
			LinkLineNode nodeEnd = (LinkLineNode)this.Line.EndPoint;
			this.ClearLine();
			//break this line at the point
			LinkLineNode node1 = new LinkLineNode(null, nodeStart);
			((Control)node1).Location = new Point(pt.X + 5, pt.Y + 5);
			this.Parent.Controls.Add(node1);
			//create a line between the new node and nodeStart
			if (nodeStart.Line == null)
			{
				nodeStart.CreateForwardLine();
			}
			else
			{
				node1.CreateBackwardLine();
			}
			nodeStart.LinkedOutPort.LinkedPortID = 0;
			if (nodeId1 == 0)
			{
				nodeId1 = node1.ActiveDrawingID;
			}
			else
			{
				node1.ActiveDrawingID = nodeId1;
			}
			//
			LinkLineNode node2 = new LinkLineNode(nodeEnd, null);
			((Control)node2).Location = new Point(pt.X - 5, pt.Y - 5);
			this.Parent.Controls.Add(node2);
			//create a line between the new node and nodeEnd
			if (nodeEnd.Line == null)
			{
				nodeEnd.CreateBackwardLine();
			}
			else
			{
				node2.CreateForwardLine();
			}
			nodeEnd.LinkedInPort.LinkedPortID = 0;
			if (nodeId2 == 0)
			{
				nodeId2 = node2.ActiveDrawingID;
			}
			else
			{
				node2.ActiveDrawingID = nodeId2;
			}
		}
		public bool HitTest(int x, int y)
		{
			if (line != null)
			{
				return line.HitTest(x, y);
			}
			return false;
		}
		public void SelectLine(bool select)
		{
			if (line != null)
			{
				if (line.Highlighted != select)
				{
					if (select == false)
					{
						select = false;
					}
					line.Highlighted = select;
					if (line.StartPoint == this)
					{
						if (this.previous != null)
						{
							this.Visible = select;
							this.Selected = select;
						}
						else
						{
							this.Visible = true;
						}
						LinkLineNode end = line.EndPoint as LinkLineNode;
						if (end != null)
						{
							if (end.NextNode != null)
							{
								end.Visible = select;
								end.Selected = select;
							}
							else
							{
								end.Visible = true;
							}
						}
					}
					else if (line.EndPoint == this)
					{
						if (this.NextNode != null)
						{
							this.Visible = select;
							this.Selected = select;
						}
						else
						{
							this.Visible = true;
						}
						LinkLineNode start = line.StartPoint as LinkLineNode;
						if (start != null)
						{
							if (start.PrevNode != null)
							{
								start.Visible = select;
								start.Selected = select;
							}
							else
							{
								start.Visible = true;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// set previous node. in a line, the outport is the starting point and the inport is the ending point.
		/// outport->next->next->...->inport
		/// </summary>
		/// <param name="p">the node to be attached to this node as the previous</param>
		public void SetPrevious(LinkLineNode p)
		{
			if (p == this)
			{
#if DEBUG
				MessageBox.Show("Cannot assign a node as its own previous node", "DEBUG - SetPrevious");
#endif
			}
			else if (p != null && (this is LinkLineNodeOutPort))
			{
#if DEBUG
				MessageBox.Show("Cannot assign a previous node to an OutPort", "DEBUG - SetPrevious");
#endif
			}
			else
			{
				previous = p;
			}
		}
		/// <summary>
		/// set next node. in a line, the outport is the starting point and the inport is the ending point.
		/// outport->next->next->...->inport
		/// </summary>
		/// <param name="n">the node to be attached to this node as the next</param>
		public void SetNext(LinkLineNode n)
		{
			if (n == this)
			{
#if DEBUG
				MessageBox.Show("Cannot assign a node as its own next node","DEBUG - SetNext");
#endif
			}
			else if (n != null && (this is LinkLineNodeInPort))
			{
#if DEBUG
				MessageBox.Show(string.Format("Cannot assign a next node [{0}] to an InPort [{1}]", n.ToString(), ToString()), "DEBUG - SetNext");
#endif
			}
			else
			{
				next = n;
			}
		}
		public override string ToString()
		{
			return "LinkLineNode";
		}
		protected virtual void Init()
		{
			this.Width = dotSize;
			this.Height = dotSize;
			this.BackColor = Color.White;
			this.Cursor = Cursors.SizeAll;
			this.ParentChanged += new EventHandler(LinkLineNode_ParentChanged);
			linkIndicate = new Rectangle(0, 0, 0, 0);
			penLinkIndicator = new Pen(Brushes.Red, indicatorSize);
		}
		public void DrawLine(Graphics g)
		{
			if (line != null)
				line.Draw(g);
		}
		protected virtual LinkLineNode CreateTargetLinkNode()
		{
			return this;
		}
		#endregion
		#region event handlers
		void LinkLineNode_ParentChanged(object sender, EventArgs e)
		{
			if (line != null && this.Parent != null)
			{
				line.SetDrawOwner(this.Parent);
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			if (_selected || next == null || previous == null)
			{
				if (previous == null)
					e.Graphics.DrawEllipse(Pens.Black, this.CircleBound);
				else
					e.Graphics.FillEllipse(Brushes.Black, this.CircleBound);
			}
		}
		/// <summary>
		/// highlight linkable node
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			LinkLineNode l2 = getJoinableNode(e);
			if (l2 != null)
			{
				Graphics g = this.Parent.CreateGraphics();
				linkIndicate = new Rectangle(l2.Left - indicatorSize, l2.Top - indicatorSize, l2.Width + indicatorSize * 2, l2.Height + indicatorSize * 2);
				g.DrawRectangle(penLinkIndicator, linkIndicate);
			}
		}
		/// <summary>
		/// determine the node that can be joined based on the mouse position
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private LinkLineNode getJoinableNode(MouseEventArgs e)
		{
			if (CanStartLink && (this.next == null || this.previous == null)) //this is an end node
			{
				if ((e.Button & MouseButtons.Left) == MouseButtons.Left && this.Parent != null)
				{
					EnumTestStartType currentNodeType = EnumTestStartType.Unknown;
					if (this.next == null && this.LinkedOutPort != null)
					{
						if (!(this is LinkLineNodeInPort))
						{
							currentNodeType = EnumTestStartType.OutPort;
						}
					}
					else if (this.previous == null && this.LinkedInPort != null)
					{
						if (!(this is LinkLineNodeOutPort))
						{
							currentNodeType = EnumTestStartType.Inport;
						}
					}
					if (currentNodeType != EnumTestStartType.Unknown)
					{
						Point p2;
						Point p = this.Center;
						//find a linkable node that is near enough
						for (int i = 0; i < this.Parent.Controls.Count; i++)
						{
							if (this == this.Parent.Controls[i]) //cannot link to itself
							{
								continue;
							}
							LinkLineNode testNode = this.Parent.Controls[i] as LinkLineNode;
							if (testNode != null)
							{
								bool canLink = false;
								//can link this to the next?
								if (currentNodeType == EnumTestStartType.OutPort 
									&& (testNode.PrevNode == null || testNode.CanCreateDuplicatedLink)
									&& !(testNode is LinkLineNodeOutPort)
									)
								{
									//link an out port to an in port
									canLink = true;
									//check cyclic reference
									LinkLineNode l = this.previous;
									while (l != null)
									{
										if (l == this.Parent.Controls[i]) //cannot make cyclic link
										{
											canLink = false;
											break;
										}
										l = l.previous;
									}
									if (canLink)
									{
										if (testNode is LinkLineNodeInPort) //it is at the port
										{
											//the port has linked open-end. linking should not be done at the end
											if (testNode.previous != null && testNode.LinkedOutPort == null)
											{
												canLink = false;
											}
										}
									}
								}
								else if (currentNodeType == EnumTestStartType.Inport/*this.previous == null*/
									&& (testNode.NextNode == null || testNode.CanCreateDuplicatedLink)
									&& !(testNode is LinkLineNodeInPort)
									)
								{
									canLink = true;
									//check cyclic reference
									LinkLineNode l = this.next;
									while (l != null)
									{
										if (l == this.Parent.Controls[i]) //cannot make cyclic link
										{
											canLink = false;
											break;
										}
										l = l.next;
									}
									if (canLink)
									{
										if (testNode is LinkLineNodeOutPort)
										{
											//the port has linked open-end. linking should not be done at the end
											if (testNode.next != null && testNode.LinkedInPort == null)
											{
												canLink = false;
											}
										}
									}
								}
								if (canLink)
								{
									//is it close enough?
									p2 = testNode.Center;
									if (Math.Abs(p.X - p2.X) + Math.Abs(p.Y - p2.Y) < linkDetectSenseSize)
									{
										return testNode;
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// make line link
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			bool bProcess = true;
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				LinkLineNode l2 = getJoinableNode(e);
				if (l2 != null)
				{
					LinkLineNode l3 = l2.CreateTargetLinkNode();
					if (l3 != null)
					{
						makeJoin(this, l3);
						bProcess = false;
					}
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				ContextMenu mnu = new ContextMenu();
				OnCreateContextMenu(mnu);
				if (mnu.MenuItems.Count > 0)
				{
					mnu.Show(this, new System.Drawing.Point(e.X, e.Y));
					bProcess = false;
				}
			}
			if (!bProcess)
			{
				this.Capture = false;
			}
			base.OnMouseUp(e);//finish base class processing
		}
		protected virtual void OnCreateContextMenu(ContextMenu mnu)
		{
			if (this.PrevNode != null && this.NextNode != null)
			{
				MenuItem mi = new MenuItem("Delete line join");
				mi.Click += new EventHandler(miDelete_Click);
				mnu.MenuItems.Add(mi);
			}
			if (this.PrevNode == null)
			{
				if (this.LinkedInPort != null && !(this is LinkLineNodeOutPort) && this.LinkedInPort.CanAssignValue)
				{
					MenuItem mi = new MenuItem("Assign value to the input");
					mi.Click += new EventHandler(miAssignInput_Click);
					mi.Tag = new ObjectAndPos(new Point(0, 0), this);
					mnu.MenuItems.Add(mi);
				}
			}
		}
		void miAssignInput_Click(object sender, EventArgs e)
		{
			MenuItem mnu = sender as MenuItem;
			if (mnu != null)
			{
				ObjectAndPos data = mnu.Tag as ObjectAndPos;
				if (data != null)
				{
					LinkLineNode portEnd = data.Data as LinkLineNode;
					LinkLineNodeInPort inPort = portEnd.LinkedInPort;
					if (inPort.Variable != null)
					{
						IMathDesigner md = this.Parent as IMathDesigner;
						if (md != null)
						{
							md.CreateValue(inPort.Variable.VariableType, inPort, portEnd.PointToScreen(data.Location));
						}
					}
				}
			}
		}
		void miDelete_Click(object sender, EventArgs e)
		{
			if (this.PrevNode != null && this.NextNode != null)
			{
				IUndoHost host = this.Parent as IUndoHost;
				LinkLineUndoDelete redo = null;
				Point p = this.Location;
				UInt32 key = this.ActiveDrawingID;
				UInt32 keyPrev = ((ActiveDrawing)PrevNode).ActiveDrawingID;
				UInt32 keyNext = ((ActiveDrawing)NextNode).ActiveDrawingID;
				if (host != null && !host.DisableUndo)
				{
					redo = new LinkLineUndoDelete(host, this.ActiveDrawingID);
				}
				Delete();
				if (host != null && !host.DisableUndo)
				{
					LinkLineUndoJoin undo = new LinkLineUndoJoin(host, keyPrev, keyNext, key, p);
					host.AddUndoEntity(new UndoEntity(undo, redo));
				}
			}
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			LinkLineNode clone = (LinkLineNode)base.Clone();
			if (this.PrevNode == null || this.PrevNode is LinkLineNodePort)
			{
				clone.SetPrevious(null);
			}
			if (this.NextNode == null || this.NextNode is LinkLineNodePort)
			{
				clone.SetNext(null);
			}
			//* saving starts from all ports.
			//* save all previous nodes except the linked output port.
			//* save next only when it is not linked
			//
			return clone;
		}

		#endregion
		#region Transparent
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		#endregion
	}
	/// <summary>
	/// Clone of LinkLineNodePort needs to set ConstructorParameters to the new owner
	/// </summary>
	public abstract class LinkLineNodePort : LinkLineNode, ILinkLineNodePort, IControlWithID
	{
		#region fields and constructors
		const string XML_LINKID = "LinkedPortID";
		const string XML_POS = "Position";
		const string XML_POSTYPE = "PositionType";
		const string XML_LABEL = "Label";
		const string XMLATT_LinkedIndex = "linkedPortIndex";
		const string XMLATT_PortId = "portId";
		const string XMLATT_InstId = "instanceId";
		private UInt32 _linkedPortInstanceId;
		private UInt32 _portId;
		protected DrawingVariable _label; //contains an IVariable 
		private UInt32 _linkedPortID;
		private enumPositionType _posType = enumPositionType.Top;
		private Point[] _corners; //positions allowed when _fixAtCorners is true
		private bool _fixAtCorners;
		private int _pos = 0;
		private Control _owner; //usually a viewer of the port owner
		private bool _labelVisible = true;
		private IPortOwner _portOwner;
		private XmlNode _xmlNode;
		private bool _adjustingPosition;
		private Color _lineColor;
		public LinkLineNodePort(IPortOwner owner)
			: base()
		{
			_portOwner = owner;
			_label = new DrawingVariable(this);
		}
		#endregion
		#region static utility
		#endregion
		#region Properties
		public Color LineColor
		{
			get
			{
				if (_lineColor == Color.Empty)
				{
					_lineColor = Color.Blue;
				}
				return _lineColor;
			}
		}
		/// <summary>
		/// this is the PortIndex of the linked LinkLineNodeOutPort.
		/// together with LinkedPortID uniquely identify the linked out port.
		/// it also becomees the index for this in port.
		/// </summary>
		public UInt32 LinkedPortInstanceID
		{
			get
			{
				return _linkedPortInstanceId;
			}
			set
			{
				_linkedPortInstanceId = value;
			}
		}
		public UInt32 PortInstanceID
		{
			get
			{
				return this.ActiveDrawingID;
			}
		}
		public IPortOwner PortOwner
		{
			get
			{
				return _portOwner;
			}
		}
		public Point[] Corners
		{
			get
			{
				return _corners;
			}
			set
			{
				_corners = value;
			}
		}
		/// <summary>
		/// true: the port is always located at one of the corners
		/// </summary>
		public bool FixedLocation
		{
			get
			{
				return _fixAtCorners;
			}
			set
			{
				_fixAtCorners = value;
			}
		}
		public bool AdjustingPosition
		{
			get
			{
				return _adjustingPosition;
			}
		}
		public IVariable Variable
		{
			get
			{
				return ((DrawingVariable)_label).Variable;
			}
		}
		public virtual bool LabelVisible
		{
			get
			{
				return _labelVisible;
			}
			set
			{
				_labelVisible = value;
				if (_label != null)
				{
					_label.LabelVisible = value;
				}
			}
		}
		public virtual bool CanRemovePort
		{
			get
			{
				return true;
			}
		}
		public virtual bool RemoveLineJoint
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region Methods
		public void SetLineColor(Color c)
		{
			_lineColor = c;
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"PortID:{0},inst:{1},LinkToPort:{2},LinkToInst:{3}", PortID, PortInstanceID, LinkedPortID, LinkedPortInstanceID);
		}
		public virtual void SetLoaded()
		{
		}
		public override void ResetActiveDrawingID()
		{
			base.ResetActiveDrawingID();
			_label.ResetActiveDrawingID();
			_label.Variable.ResetID((UInt32)Guid.NewGuid().GetHashCode());
		}
		public void MoveTo(int x, int y)
		{
			MoveTo(x, y, new Point(x, y));
		}
		/// <summary>
		/// move the port to a new position.
		/// see OnMouseMove for the calculation of the parameters
		/// </summary>
		/// <param name="x">the new x position</param>
		/// <param name="y">the new y position</param>
		/// <param name="p">mouse position</param>
		public void MoveTo(int x, int y, Point p)
		{
			if (PositionType == enumPositionType.Circle)
			{
				adjustCirclePosition(x, y);
				if (this.Parent != null)
				{
					ActiveDrawing.RefreshControl(this.Parent);
				}
				return;
			}
			if (_fixAtCorners && _corners != null && _corners.Length > 0)
			{
				adjustCornerPosition(x, y);
				if (this.Parent != null)
				{
					ActiveDrawing.RefreshControl(this.Parent);
				}
				return;
			}
			if (_owner == null)
			{
				MathNode.Log(this.FindForm(), new MathException("Port missing owner"));
			}
			else
			{
				bool b = false;
				int dxLeft = Math.Abs(p.X - _owner.Left);
				int dyTop = Math.Abs(p.Y - _owner.Top);
				int dxRight = Math.Abs(p.X - _owner.Left - _owner.Width);
				int dyBottom = Math.Abs(p.Y - _owner.Top - _owner.Height);
				//
				enumPositionType posType = PositionType;
				if (dxLeft < dxRight)//near left
				{
					if (dyTop < dyBottom)//near (left, top)
					{
						if (x < _owner.Left - this.Width) //x outside
						{
							if (y < _owner.Top - this.Height) //y out side
							{
								if (dxLeft > dyTop) //left
								{
									posType = enumPositionType.Left;
								}
								else if (dxLeft < dyTop) //top
								{
									posType = enumPositionType.Top;
								}
								else
								{
									if (posType == enumPositionType.Bottom)
									{
										posType = enumPositionType.Left;
									}
									else if (posType == enumPositionType.Right)
									{
										posType = enumPositionType.Top;
									}
								}
							}
							else //y inside
							{
								posType = enumPositionType.Left;
							}
						}
						else //x inside
						{
							if (y < _owner.Top - this.Height) //y outside
							{
								posType = enumPositionType.Top;
							}
							else //y inside
							{
								if (dxLeft < dyTop) //left
								{
									posType = enumPositionType.Left;
								}
								else if (dxLeft > dyTop)
								{
									posType = enumPositionType.Top;
								}
							}
						}
					}
					else //near (left, bottom)
					{
						if (x < _owner.Left - this.Width) //x out side
						{
							if (y > _owner.Top + _owner.Height) // y out side
							{
								if (dxLeft > dyBottom) //left
								{
									posType = enumPositionType.Left;
								}
								else if (dxLeft < dyBottom) //bottom
								{
									posType = enumPositionType.Bottom;
								}
								else
								{
									if (posType == enumPositionType.Top)
									{
										posType = enumPositionType.Left;
									}
									else if (posType == enumPositionType.Right)
									{
										posType = enumPositionType.Bottom;
									}
								}
							}
							else //y inside
							{
								posType = enumPositionType.Left;
							}
						}
						else //x inside
						{
							if (y > _owner.Top + _owner.Height) // y out side
							{
								posType = enumPositionType.Bottom;
							}
							else //y inside
							{
								if (dxLeft < dyBottom) //left
								{
									posType = enumPositionType.Left;
								}
								else if (dxLeft > dyBottom) //bottom
								{
									posType = enumPositionType.Bottom;
								}
							}
						}
					}
				}
				else //near right
				{
					if (dyTop < dyBottom)//near (right, top)
					{
						if (y < _owner.Top - this.Height) //y outside
						{
							if (x > _owner.Left + _owner.Width) //x outside
							{
								if (dxRight > dyTop) //right
								{
									posType = enumPositionType.Right;
								}
								else if (dxRight < dyTop) //top
								{
									posType = enumPositionType.Top;
								}
								else
								{
								}
							}
							else //x inside
							{
								posType = enumPositionType.Top;
							}
						}
						else //y inside
						{
							if (x > _owner.Left + _owner.Width) //x outside
							{
								posType = enumPositionType.Right;
							}
							else //x inside
							{
								if (dxRight < dyTop) //right
								{
									posType = enumPositionType.Right;
								}
								else
								{
									posType = enumPositionType.Top;
								}
							}
						}
					}
					else //near (right, bottom)
					{
						if (x > _owner.Left + _owner.Width) //x outside
						{
							if (y > _owner.Top + _owner.Height) //y outside
							{
								if (dxRight > dyBottom) //right
								{
									posType = enumPositionType.Right;
								}
								else if (dxRight < dyBottom) //bottom
								{
									posType = enumPositionType.Bottom;
								}
								else
								{
								}
							}
							else //y inside
							{
								posType = enumPositionType.Right;
							}
						}
						else //x inside
						{
							if (y > _owner.Top + _owner.Height) //y outside
							{
								posType = enumPositionType.Bottom;
							}
							else //y inside
							{
								if (dxRight < dyBottom) //right
								{
									posType = enumPositionType.Right;
								}
								else
								{
									posType = enumPositionType.Bottom;
								}
							}
						}
					}
				}
				if (posType != PositionType)
				{
					PositionType = posType;
					if (posType == enumPositionType.Bottom || posType == enumPositionType.Top)
					{
						if (x > _owner.Left + _owner.Width)
							_pos = _owner.Width;
						else if (x < _owner.Left - this.Width)
							_pos = -this.Width;
						else
							_pos = x - _owner.Left;
					}
					else
					{
						if (y < _owner.Top - this.Height)
							_pos = -this.Height;
						else if (y > _owner.Top + _owner.Height)
							_pos = _owner.Height;
						else
							_pos = y - _owner.Top;
					}
					adjustPosition();
					b = true;
				}
				//
				if (!b)
				{
					if (PositionType == enumPositionType.Bottom || PositionType == enumPositionType.Top)
					{
						if (x > _owner.Left - this.Width && x < _owner.Left + _owner.Width)
						{
							this.Left = x;
							_pos = x - _owner.Left;
							b = true;
						}
					}
					if (PositionType == enumPositionType.Left || PositionType == enumPositionType.Right)
					{
						if (y > _owner.Top - this.Height && y < _owner.Top + _owner.Height)
						{
							this.Top = y;
							_pos = y - _owner.Top;
							b = true;
						}
					}
				}
				if (b)
				{
					if (this.Parent != null)
					{
						ActiveDrawing.RefreshControl(this.Parent);
					}
				}
			}
		}
		/// <summary>
		/// assign the variable to the label
		/// </summary>
		/// <param name="variable"></param>
		public void SetVariable(IVariable variable)
		{
			((DrawingVariable)_label).SetVariable(variable);
		}
		public void SetPortOwner(IPortOwner owner)
		{
			_portOwner = owner;
		}
		public void SetLabel(DrawingVariable lbl)
		{
			_label = lbl;
			if (_label != null)
			{
				_label.SetOwner(this);
			}
		}
		public void SetLabelOwner()
		{
			if (_label != null)
			{
				_label.SetOwner(this);
			}
		}
		public void HideLabel()
		{
			_label.Visible = false;
		}
		protected virtual enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			return enumPositionType.Top;
		}
		#endregion
		#region event handlers
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (_label != null && this.Parent != null)
			{
				this.Parent.Controls.Add(_label);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.Parent != null && (e.Button & MouseButtons.Left) == MouseButtons.Left && bMD)
			{
				if (!this.Capture)
				{
					IMathDesigner md = this.Parent as IMathDesigner;
					if (md != null)
					{
						ISelectionService ss = md.GetDesignerService(typeof(ISelectionService)) as ISelectionService;
						if (ss != null && ss.PrimarySelection == this.Owner)
						{
							ss.SetSelectedComponents(null);
						}
					}
					this.Capture = true;
				}
				//check which side is the nearest
				Point p = this.Parent.PointToClient(this.PointToScreen(new Point(e.X, e.Y)));
				//new position
				int x = this.Left + e.X - x0;
				int y = this.Top + e.Y - y0;
				MoveTo(x, y, p);
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		[Browsable(false)]
		public XmlNode CachedXmlNode
		{
			get
			{
				return _xmlNode;
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_xmlNode = node;
			base.OnReadFromXmlNode(serializer, node);
			_portId = XmlSerialization.GetAttributeUInt(node, XMLATT_PortId);
			ActiveDrawingID = XmlSerialization.GetAttributeUInt(node, XMLATT_InstId);
			XmlNode ln = node.SelectSingleNode(XML_LINKID);
			if (ln != null)
			{
				if (!string.IsNullOrEmpty(ln.InnerText))
				{
					_linkedPortID = Convert.ToUInt32(ln.InnerText);
					_linkedPortInstanceId = XmlSerialization.GetAttributeUInt(ln, XMLATT_LinkedIndex);
					if (_linkedPortID == _portId)
					{
						throw new MathException(XmlSerialization.FormatString("Reading port {0} failed: port cannot be linked to itself", _portId));
					}
				}
			}
			object v;
			if (XmlSerialization.ReadValueFromChildNode(node, XML_POSTYPE, out v))
			{
				this.PositionType = (enumPositionType)v;
			}
			this.Position = XmlSerialization.ReadIntValueFromChildNode(node, XML_POS);
			DrawingVariable lb = (DrawingVariable)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_LABEL);
			if (lb != null)
			{
				_label = lb;
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_xmlNode = node;
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.SetAttribute(node, XMLATT_PortId, PortID);
			XmlSerialization.SetAttribute(node, XMLATT_InstId, PortInstanceID);
			XmlSerialization.WriteValueToChildNode(node, XML_POSTYPE, this.PositionType);
			XmlSerialization.WriteIntValueToChildNode(node, XML_POS, this.Position);
			XmlSerialization.WriteUIntValueToChildNode(node, XML_LINKID, this.LinkedPortID);
			XmlNode ln = node.SelectSingleNode(XML_LINKID);
			if (ln != null)
			{
				XmlSerialization.SetAttribute(ln, XMLATT_LinkedIndex, _linkedPortInstanceId);
			}
			if (_label != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, XML_LABEL, _label);
			}
		}
		#endregion
		#region ILinkLineNodePort Members
		public UInt32 PortID
		{
			get
			{
				if (_portId == 0)
				{
					_portId = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _portId;
			}
		}
		public UInt32 LinkedPortID
		{
			get
			{
				return _linkedPortID;
			}
			set
			{
				if (value != 0 && value == PortID)
				{
					throw new MathException(XmlSerialization.FormatString("Cannot make a self link for port {0}", value));
				}
				_linkedPortID = value;
			}
		}
		public void SetInstanceId(UInt32 id, UInt32 instanceId)
		{
			_portId = id;
			ActiveDrawingID = instanceId;
		}
		public Point DefaultNextNodePosition()
		{
			int x = 30;
			int y = 30;
			switch (PositionType)
			{
				case enumPositionType.Bottom:
					x = Left;
					y = Top + Height + 30;
					break;
				case enumPositionType.Left:
					x = Left - 30;
					y = Top;
					break;
				case enumPositionType.Right:
					x = Left + Width + 30;
					y = Top;
					break;
				case enumPositionType.Top:
					x = Left;
					y = Top - 30;
					break;
				case enumPositionType.Circle:
					x = Left + Width / 2;
					y = Top + 30;
					break;
			}
			if (x < 0)
				x = 0;
			if (y < 0)
				y = 0;
			if (this.Parent != null)
			{
				if (x > this.Parent.ClientSize.Width)
					x = this.Parent.ClientSize.Width - this.Width;
				if (y > this.Parent.ClientSize.Height)
					y = this.Parent.ClientSize.Height - this.Height;
			}
			return new Point(x, y);
		}

		public virtual enumPositionType PositionType
		{
			get
			{
				return _posType;
			}
			set
			{
				_posType = value;
			}
		}

		public int Position
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
			}
		}

		/// <summary>
		/// the math viewer control owing the port
		/// </summary>
		[ReadOnly(true)]
		public Control Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
				if (_owner != null)
				{
					if (_owner.Parent != null)
					{
						if (!IsPositionValid())
						{
							adjustPosition();
						}
					}
					_owner.Resize += new EventHandler(_owner_Resize);
					_owner.Move += new EventHandler(_owner_Move);
				}
			}
		}
		public RelativeDrawing Label
		{
			get
			{
				return _label;
			}
		}
		private bool IsPositionValid()
		{
			if (_owner != null)
			{
				switch (_posType)
				{
					case enumPositionType.Bottom:
						if (this.Top != _owner.Top + _owner.Height)
							return false;
						if (this.Left < _owner.Left - this.Width)
							return false;
						if (this.Left > _owner.Left + _owner.Width)
							return false;
						break;
					case enumPositionType.Left:
						if (this.Left != _owner.Left - this.Width)
							return false;
						if (this.Top < _owner.Top - this.Height)
							return false;
						if (this.Top > _owner.Top + _owner.Height)
							return false;
						break;
					case enumPositionType.Right:
						if (this.Left != _owner.Left + _owner.Width)
							return false;
						if (this.Top < _owner.Top - this.Height)
							return false;
						if (this.Top > _owner.Top + _owner.Height)
							return false;
						break;
					case enumPositionType.Top:
						if (this.Top != _owner.Top - this.Height)
							return false;
						if (this.Left < _owner.Left - this.Width)
							return false;
						if (this.Left > _owner.Left + _owner.Width)
							return false;
						break;
					case enumPositionType.Circle:
						double r = (_owner.Width + this.Width) / 2;
						double x0 = _owner.Left + _owner.Width / 2;
						double y0 = _owner.Top + _owner.Height / 2;
						double x1 = this.Left + this.Width / 2;
						double y1 = this.Top + this.Height / 2;
						if (Math.Abs(x0 - x1) < 0.1)
						{
							if (y1 > y0)
							{
								return Math.Abs(y1 - y0 - r) < 0.1;
							}
							else
							{
								return Math.Abs(y0 - y1 - r) < 0.1;
							}
						}
						else
						{
							double v = (y1 - y0) / (x1 - x0);
							double a = r / Math.Sqrt(1 + v * v);
							double xi = x0 + a;
							double yi = y0 + v * a;
							double xj = x0 - a;
							double yj = y0 - v * a;
							double li = (x1 - xi) * (x1 - xi) + (y1 - yi) * (y1 - yi);
							double lj = (x1 - xj) * (x1 - xj) + (y1 - yj) * (y1 - yj);
							return Math.Min(li, lj) < 0.1;
						}
				}
			}
			return true;
		}
		public void adjustCornerPosition(int x, int y)
		{
			if (_owner == null || _corners == null || _corners.Length == 0)
				return;
			int n = 0;
			Point[] ps = _corners;
			if (_corners.Length > 1)
			{
				float x0 = x - _owner.Left;
				float y0 = y - _owner.Top;
				float f = ((float)ps[0].X - x0) * ((float)ps[0].X - x0) + ((float)ps[0].Y - y0) * ((float)ps[0].Y - y0);
				for (int i = 1; i < _corners.Length; i++)
				{
					float f1 = ((float)ps[i].X - x0) * ((float)ps[i].X - x0) + ((float)ps[i].Y - y0) * ((float)ps[i].Y - y0);
					if (f1 < f)
					{
						f = f1;
						n = i;
					}
				}
			}
			Point p = this.Location;
			Position = n;
			adjustPosition();
			_owner.Invalidate(new Rectangle(p, this.Size));
			_owner.Invalidate(new Rectangle(this.Location, this.Size));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">in _owner's parent coordinates</param>
		/// <param name="y"></param>
		private void adjustCirclePosition(int x, int y)
		{
			if (_owner == null)
				return;
			//radius
			double r = (Math.Sqrt(2.0) * _owner.Width + this.Width) / 2;
			//center of the owner: (x0,y0)
			double x0 = _owner.Left + _owner.Width / 2;
			double y0 = _owner.Top + _owner.Height / 2;
			//taking (x, y) as the location, (x1, y1) is the center
			double x1 = x + this.Width / 2;
			double y1 = y + this.Height / 2;
			//adjust the location
			if (Math.Abs(x0 - x1) < 0.1)
			{
				if (y1 > y0)
				{
					this.Top = (int)(y0 + r - this.Height / 2);
				}
				else
				{
					this.Top = (int)(y0 - r - this.Height / 2);
				}
			}
			else
			{
				double v = (y1 - y0) / (x1 - x0);
				double a = r / Math.Sqrt(1 + v * v);
				double xi = x0 + a;
				double yi = y0 + v * a;
				double xj = x0 - a;
				double yj = y0 - v * a;
				double li = (x1 - xi) * (x1 - xi) + (y1 - yi) * (y1 - yi);
				double lj = (x1 - xj) * (x1 - xj) + (y1 - yj) * (y1 - yj);
				if (li < lj)
				{
					this.Location = new Point((int)(xi - this.Width / 2), (int)(yi - this.Height / 2));
				}
				else
				{
					this.Location = new Point((int)(xj - this.Width / 2), (int)(yj - this.Height / 2));
				}
			}
		}
		public void adjustPosition()
		{
			if (_owner == null)
				return;
			_adjustingPosition = true;
			if (_posType == enumPositionType.Circle)
			{
				adjustCirclePosition(this.Left, this.Top);
			}
			else
			{
				if (FixedLocation && _corners != null && _corners.Length > 0 && _corners.Length > _pos)
				{
					//need to decide which end should be attached to the point
					//decide corner type:
					_posType = GetPosTypeByCornerPos(_pos);
					switch (_posType)
					{
						case enumPositionType.Top:
							this.Top = _owner.Top + _corners[_pos].Y - this.Height;
							this.Left = _owner.Left + _corners[_pos].X - this.Width / 2;
							break;
						case enumPositionType.Left:
							this.Top = _owner.Top + _corners[_pos].Y - this.Height / 2;
							this.Left = _owner.Left + _corners[_pos].X - this.Width;
							break;
						case enumPositionType.Right:
							this.Top = _owner.Top + _corners[_pos].Y - this.Height / 2;
							this.Left = _owner.Left + _corners[_pos].X;
							break;
						case enumPositionType.Bottom:
							this.Top = _owner.Top + _corners[_pos].Y;
							this.Left = _owner.Left + _corners[_pos].X - this.Width / 2;
							break;
					}
					return;
				}
				switch (_posType)
				{
					case enumPositionType.Bottom:
						this.Top = _owner.Top + _owner.Height;
						if (_pos > _owner.Width || _pos < -this.Width)
							_pos = _owner.Width / 2;
						this.Left = _owner.Left + _pos;
						break;
					case enumPositionType.Left:
						if (_pos > _owner.Height || _pos < -this.Height)
							_pos = _owner.Height / 2;
						this.Top = _owner.Top + _pos;
						this.Left = _owner.Left - this.Width;
						break;
					case enumPositionType.Right:
						if (_pos > _owner.Height || _pos < -this.Height)
							_pos = _owner.Height / 2;
						this.Top = _owner.Top + _pos;
						this.Left = _owner.Left + _owner.Width;
						break;
					case enumPositionType.Top:
						this.Top = _owner.Top - this.Height;
						if (_pos > _owner.Width || _pos < -this.Width)
							_pos = _owner.Width / 2;
						this.Left = _owner.Left + _pos;
						break;
					case enumPositionType.Circle:
						adjustCirclePosition(this.Left, this.Top);
						break;
				}
			}
			_adjustingPosition = false;
		}
		void _owner_Move(object sender, EventArgs e)
		{
			adjustPosition();
		}

		void _owner_Resize(object sender, EventArgs e)
		{
			adjustPosition();
		}
		#endregion
		#region ICloneable
		public override object Clone()
		{
			LinkLineNodePort clone = (LinkLineNodePort)base.Clone();
			clone.SetInstanceId(this.PortID, this.PortInstanceID);
			clone.PositionType = _posType;
			clone.Position = _pos;
			clone.FixedLocation = FixedLocation;
			if (_label != null)
			{
				bool b = _label.ClonePorts;
				_label.ClonePorts = false;
				clone.SetLabel((DrawingVariable)_label.Clone());
				_label.ClonePorts = b;
			}
			clone.LinkedPortID = _linkedPortID;
			clone.LinkedPortInstanceID = _linkedPortInstanceId;
			return clone;
		}
		#endregion
		#region IControlWithID Members

		public UInt32 ControlID
		{
			get
			{
				return PortInstanceID;
			}
		}

		#endregion
	}
	/// <summary>
	/// represents a parameter
	/// </summary>
	public class LinkLineNodeInPort : LinkLineNodePort
	{
		#region fields and constructors
		public LinkLineNodeInPort(IPortOwner owner)
			: base(owner)
		{
			PositionType = enumPositionType.Top;
		}
		#endregion
		#region properties
		public virtual bool CanAssignValue
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region methods
		public override void ResetActiveDrawingID()
		{
			base.ResetActiveDrawingID();
			LinkLineNode l = (LinkLineNode)this.PrevNode;
			while (l != null && !(l is LinkLineNodePort))
			{
				l.ResetActiveDrawingID();
				l = (LinkLineNode)l.PrevNode;
			}
		}
		public void CheckCreatePreviousNode()
		{
			if (previous == null)
			{
				previous = new LinkLineNode(this, null);
				line = new LinkLine();
				line.AssignMapObjects(previous, this);
				previous.Location = DefaultNextNodePosition();
				previous.SaveLocation();
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawEllipse(Pens.Black, this.CircleBound);
			if (PortOwner.IsDummyPort)
			{
				e.Graphics.DrawLine(Pens.Blue, this.CircleBound.Left, this.CircleBound.Top + this.CircleBound.Height / 2, this.CircleBound.Left + this.CircleBound.Width, this.CircleBound.Top + this.CircleBound.Height / 2);
				e.Graphics.DrawLine(Pens.Blue, this.CircleBound.Left + this.CircleBound.Width / 2, this.CircleBound.Top, this.CircleBound.Left + this.CircleBound.Width / 2, this.CircleBound.Top + this.CircleBound.Height);
			}
		}
		public override object Clone()
		{
			LinkLineNodeInPort clone = (LinkLineNodeInPort)base.Clone();
			//clone.LinkedPortIndex = LinkedPortIndex;
			//inport clone all nodes; outport only close nodes when not linked
			LinkLineNode l = (LinkLineNode)this.PrevNode;
			LinkLineNode c = clone;
			while (l != null && !(l is LinkLineNodePort))
			{
				LinkLineNode p = (LinkLineNode)l.Clone();
				c.SetPrevious(p);
				p.SetNext(c);
				l = (LinkLineNode)l.PrevNode;
				c = p;
			}
			return clone;
		}
		#endregion
	}
	public class LinkLineNodeOutPort : LinkLineNodePort
	{
		#region fields and constructors
		public LinkLineNodeOutPort(IPortOwner owner)
			: base(owner)
		{
			PositionType = enumPositionType.Bottom;
			_label.RelativePosition = new RelativePosition(this, 20, 20, true, true);
		}
		#endregion
		#region properties
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region methods
		/// <summary>
		/// for error fixing
		/// </summary>
		/// <returns></returns>
		public virtual LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			LinkLineNodeOutPort po = (LinkLineNodeOutPort)Activator.CreateInstance(this.GetType(), this.PortOwner);
			po.SetInstanceId(this.PortID, (UInt32)(Guid.NewGuid().GetHashCode()));
			po._label.Text = _label.Text;
			return po;
		}
		public override void ResetActiveDrawingID()
		{
			base.ResetActiveDrawingID();
			LinkLineNode l = (LinkLineNode)this.NextNode;
			while (l != null && !(l is LinkLineNodePort))
			{
				l.ResetActiveDrawingID();
				l = (LinkLineNode)l.NextNode;
			}
		}
		public void CheckCreateNextNode()
		{
			if (next == null)
			{
				next = new LinkLineNode(null, this);
				line = new LinkLine();
				line.AssignMapObjects(this, next);
				next.Location = DefaultNextNodePosition();
				next.SaveLocation();
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillEllipse(Brushes.Black, this.CircleBound);
		}
		public override object Clone()
		{
			LinkLineNode clone = (LinkLineNode)base.Clone();
			//inport clone all nodes; outport only close nodes when not linked
			if (this.LinkedInPort == null)
			{
				LinkLineNode l = (LinkLineNode)this.NextNode;
				LinkLineNode c = clone;
				while (l != null && !(l is LinkLineNodePort))
				{
					LinkLineNode p = (LinkLineNode)l.Clone();
					c.SetNext(p);
					p.SetPrevious(c);
					l = (LinkLineNode)l.NextNode;
					c = p;
				}
			}
			return clone;
		}
		#endregion
	}
	public class PortCollection : List<LinkLineNodePort>
	{
		public PortCollection()
		{
		}
		public void ValidatePortLinks()
		{
			List<LinkLineNodePort> invalids = new List<LinkLineNodePort>();
			foreach (LinkLineNodePort p in this)
			{
				if (p.LinkedPortID != 0)
				{
					LinkLineNodePort pl = GetPortByID(p.LinkedPortID, p.LinkedPortInstanceID);
					if (pl != null)
					{
						LinkLineNodePort pl2 = GetPortByID(pl.LinkedPortID, pl.LinkedPortInstanceID);
						if (pl2 != null)
						{
							if (pl2.PortID != p.PortID && pl2.PortInstanceID != p.PortInstanceID)
							{
								invalids.Add(pl2);
							}
						}
					}
				}
			}
			foreach (LinkLineNodePort p in invalids)
			{
				this.Remove(p);
				p.PortOwner.RemovePort(p);
			}
		}
		public void SetPortOwner(Control owner)
		{
			foreach (LinkLineNodePort p in this)
			{
				p.Owner = owner;
			}
		}
		public void HideLabel()
		{
			foreach (LinkLineNodePort p in this)
			{
				p.HideLabel();
			}
		}
		public LinkLineNodePort GetPortByID(UInt32 id, UInt32 instanceId)
		{
			foreach (LinkLineNodePort p in this)
			{
				if (p.PortID == id && p.PortInstanceID == instanceId)
				{
					return p;
				}
			}
			return null;
		}
		public LinkLineNodeOutPort GetLinkedPort(LinkLineNodeInPort inPort)
		{
			foreach (LinkLineNodePort p in this)
			{
				LinkLineNodeOutPort po = p as LinkLineNodeOutPort;
				if (po != null)
				{
					if (po.PortID == inPort.LinkedPortID && po.PortInstanceID == inPort.LinkedPortInstanceID)
					{
						if (po.LinkedPortID == inPort.PortID && po.LinkedPortInstanceID == inPort.PortInstanceID)
						{
							return po;
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// can be used at the end of reading from file by OnReadFromXmlNode
		/// and at the end of Clone
		/// </summary>
		public void MakeLinks(Form caller)
		{
			foreach (LinkLineNodePort p in this)
			{
				if (p == null)
				{
					throw new MathException("Port is null");
				}
				((DrawingVariable)(p.Label)).SetOwner(p);
				LinkLineNodeInPort pi = p as LinkLineNodeInPort;
				if (pi != null)
				{
					if (p.LinkedPortID != 0 && p.LinkedOutPort == null)
					{
						//make the links
						LinkLineNodeOutPort port = this.GetLinkedPort(pi);
						if (port == null)
						{
							LinkLineNodePort port2 = this.GetPortByID(p.LinkedPortID, ((LinkLineNodeInPort)p).LinkedPortInstanceID);
							if (port2 == null)
							{
								MathNode.Log(caller,new MathException("InPort [{0},{1}] is linked to [{2},{3}], but port [{2},{3}] is not found", p.PortID, p.PortInstanceID, p.LinkedPortID, p.LinkedPortInstanceID));
								continue;
							}
							else
							{
								LinkLineNodeOutPort po = port2 as LinkLineNodeOutPort;
								if (po == null)
								{
									MathNode.Log(caller,new MathException("InPort [{0},{1}] is linked to [{2},{3}], but port [{2},{3}] is not a source port", p.PortID, p.PortInstanceID, p.LinkedPortID, p.LinkedPortInstanceID));
									continue;
								}
								else
								{
									//try to fix it
									if (po.LinkedPortID == 0)
									{
										//make the link
										po.LinkedPortID = p.PortID;
										po.LinkedPortInstanceID = p.PortInstanceID;
										port = po;
									}
									else
									{
										MathNode.Log(caller,new MathException("InPort [{0},{1}] is linked to [{2},{3}], but port [{2},{3}] is linked to [{4},{5}]", p.PortID, p.PortInstanceID, p.LinkedPortID, p.LinkedPortInstanceID, po.LinkedPortID, po.LinkedPortInstanceID));
										continue;
									}
								}
							}
						}
						LinkLineNode start = p.Start;
						LinkLineNode end = port.End;
						end.SetNext(start);
						start.SetPrevious(end);
					}
				}
			}
		}
		/// <summary>
		/// it should be called after all controls have been added to the parent
		/// </summary>
		public void CreateLinkLines()
		{
			foreach (LinkLineNodePort p in this)
			{
				p.ClearLine();
			}
			foreach (LinkLineNodePort p in this)
			{
				LinkLineNodeInPort pi = p as LinkLineNodeInPort;
				if (pi != null)
				{
					LinkLineNodeOutPort plo = pi.LinkedOutPort;
					if (plo != null)
					{
						LinkLineNodePort plo2 = this.GetPortByID(pi.LinkedPortID, pi.LinkedPortInstanceID);
						if (plo2 != null && plo2 != plo)
						{
							//plo2 should be used. 
							ILinkLineNode ln = pi;
							if (ln.PrevNode != null)
							{
								if (!(ln.PrevNode is LinkLineNodePort))
								{
									ln = ln.PrevNode;
									while (!(ln is LinkLineNodePort))
									{
										if (ln.PrevNode == null)
										{
											break;
										}
										if (ln.PrevNode is LinkLineNodePort)
										{
											break;
										}
										ln = ln.PrevNode;
									}
								}
							}
							LinkLineNode l0 = (LinkLineNode)ln;
							l0.SetPrevious(plo2);
							plo2.SetNext(l0);
						}
					}
				}
			}
			foreach (LinkLineNodePort p in this)
			{
				((DrawingVariable)(p.Label)).SetOwner(p);
				p.Label.AdjustPosition();
				if (p.LinkedPortID != 0 && p.LinkedPortInstanceID != 0)
				{
					LinkLineNodePort port = this.GetPortByID(p.LinkedPortID, p.LinkedPortInstanceID);
					if (port != null)
					{
						LinkLineNode start = null; //from start to end
						LinkLineNode end = null;
						LinkLineNodeInPort pi = p as LinkLineNodeInPort;
						if (pi != null)
						{
							if (pi.LinkedOutPort == null)
							{
								end = pi.Start;
								start = port.End; //port is out
								if (start == end)
								{
									//already linked by port
									start = port;
									while (start.NextNode != null && start.NextNode != end)
									{
										start = (LinkLineNode)start.NextNode;
									}
								}
							}
						}
						else
						{
							LinkLineNodeOutPort po = p as LinkLineNodeOutPort;
							if (po != null)
							{
								if (po.LinkedInPort == null)
								{
									end = port.Start; //port is in
									start = po.End;
									if (start == end)
									{
										//port already linked
										end = port;
										while (end.PrevNode != null && end.PrevNode != start)
										{
											end = (LinkLineNode)end.PrevNode;
										}
									}
								}
							}
						}
						if (start != null && end != null)
						{
							start.SetNext(end);
							end.SetPrevious(start);
						}
					}
				}
			}
			foreach (LinkLineNodePort p in this)
			{
				if (p is LinkLineNodeInPort)
				{
					LinkLineNode l = p;
					while (l != null)
					{
						l.CreateBackwardLine();
						((Control)l).Visible = (l is LinkLineNodePort);
						l = (LinkLineNode)l.PrevNode;
					}
				}
				else if (p is LinkLineNodeOutPort)
				{
					if (p.LinkedPortID == 0)
					{
						LinkLineNode l = p;
						while (l != null)
						{
							((Control)l).Visible = (l is LinkLineNodePort);
							l.CreateForwardLine();
							l = (LinkLineNode)l.NextNode;
						}
					}
				}
			}
		}
		/// <summary>
		/// get all controls in one list to be added to the parent.
		/// it also make the line links. But lines are not created yet.
		/// CreateLinkLines() should be called after adding all controls to the parent.
		/// </summary>
		/// <returns></returns>
		public List<Control> GetAllControls(bool bErrorRecover)
		{
			List<Control> cs = new List<Control>();
			foreach (LinkLineNodePort p in this)
			{
				cs.Add(p.Label);
				if (p is LinkLineNodeInPort)
				{
					if (p.LinkedPortID == 0)
					{
						if (bErrorRecover)
						{
							//error recovery: add previous node if it is missing
							if (p.CanStartLink)
							{
								if (p.PrevNode == null)
								{
									LinkLineNode prev = new LinkLineNode();
									prev.SetNext(p);
									p.SetPrevious(prev);
								}
							}
						}
					}
					LinkLineNode l = p;
					while (l != null && !(l is LinkLineNodeOutPort))
					{
						cs.Add(l);
						l = (LinkLineNode)l.PrevNode;
					}
				}
				else if (p is LinkLineNodeOutPort)
				{
					if (p.LinkedPortID == 0)
					{
						if (bErrorRecover)
						{
							//error recovery: add next node if it is missing
							if (p.CanStartLink)
							{
								if (p.NextNode == null)
								{
									LinkLineNode next = new LinkLineNode();
									next.SetPrevious(p);
									p.SetNext(next);
								}
							}
						}
					}
					else
					{
						LinkLineNodePort port = this.GetPortByID(p.LinkedPortID, 0);
						if (port == null)
						{
							if (bErrorRecover)
							{
								throw new MathException("Port {0}({1}) is linkied to port {2}, but port {2} not found", p.PortID, p.PortOwner.TraceInfo, p.LinkedPortID);
							}
						}
						else
						{
							if (p.LinkedInPort != null)
							{
								if (p.LinkedInPort != port)
								{
									if (bErrorRecover)
									{
										throw new MathException("Port {0}({1}) is linked to port {2}, but mismatching ports are found.", p.PortID, p.PortOwner.TraceInfo, p.LinkedPortID);
									}
								}
							}
							else
							{
								LinkLineNode pStart = p.End;
								LinkLineNode pEnd = port.Start;
								if (pEnd.LinkedOutPort != null)
								{
									if (bErrorRecover)
									{
										throw new MathException("Port {0}({2}) is linked to port {1}, but more than one port {0} are found.", port.PortID, port.LinkedPortID, p.PortOwner.TraceInfo);
									}
								}
								else
								{
									pStart.SetNext(pEnd);
									pEnd.SetPrevious(pStart);
								}
							}
						}
					}
					LinkLineNode l = p;
					while (l != null && !(l is LinkLineNodeInPort))
					{
						cs.Add(l);
						l = (LinkLineNode)l.NextNode;
						if (l is LinkLineNodePort)
							break;
					}
				}
			}
			return cs;
		}
	}
}
