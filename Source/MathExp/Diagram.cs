/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using TraceLog;
using WindowsUtility;

namespace MathExp
{
	/// <summary>
	/// a diagram to show and manipulate link lines and nodes.
	/// </summary>
	public abstract partial class Diagram : UserControl
	{
		#region fields and constructors
		private LinkLineEnds _selectedLine;
		private ActiveDrawing _mouseDownControl;
		private Point _menuPoint = Point.Empty;
		public Diagram()
		{
			InitializeComponent();
		}
		#endregion
		#region protected methods
		protected abstract IUndoHost UndoHost { get; }

		protected virtual void OnCreateContextMenu(ContextMenu mnu, Point pt)
		{
		}
		protected virtual void OnCreateContextMenuForLinkLine(LinkLineNode lineNode, ContextMenu mnu, Point pt)
		{
		}
		protected virtual void OnLineSelectionChanged()
		{
		}
		protected virtual void OnLineNodeAdded(LinkLineNode newNode)
		{
		}
		public virtual void OnLineNodeDeleted(LinkLineNode previousNode, LinkLineNode nodeRemoved, LinkLineNode nextNode)
		{
		}
		public virtual void OnActiveDrawingMouseDown() { }
		public virtual void OnActiveDrawingMouseUp(ActiveDrawing a) { }
		#endregion
		#region protected properties
		protected virtual bool AllowLineSelection { get { return false; } }
		protected virtual bool AllowLineDisconnect { get { return true; } }
		protected ActiveDrawing MouseDownControl { get { return _mouseDownControl; } }
		protected Point MenuPoint { get { return _menuPoint; } }
		#endregion
		#region Public properties
		public LinkLineEnds SelectedLinkLine { get { return _selectedLine; } }
		#endregion
		#region public methods
		public void SetMouseDownControl(ActiveDrawing ad)
		{
			_mouseDownControl = ad;
			OnActiveDrawingMouseDown();
		}
		public void SetMouseUpControl()
		{
			ActiveDrawing a = _mouseDownControl;
			_mouseDownControl = null;
			OnActiveDrawingMouseUp(a);
		}
		public void ClearLineSelection()
		{
			_selectedLine = null;
		}
		public void OnChildActiveDrawingMouseEnter(ActiveDrawing actDrwing)
		{
			bMouseIn = false;
		}
		public ActiveDrawing GetActiveDrawingById(UInt32 id)
		{
			foreach (Control c in Controls)
			{
				ActiveDrawing ad = c as ActiveDrawing;
				if (ad != null)
				{
					if (ad.ActiveDrawingID == id)
					{
						return ad;
					}
				}
			}
			return null;
		}
		public void EnlargeForChildren()
		{
			Size sz = new Size(this.ClientSize.Width, this.ClientSize.Height);
			foreach (Control c in Controls)
			{
				if (c.Bounds.Bottom > sz.Height)
				{
					sz.Height = c.Bounds.Bottom;
				}
				if (c.Bounds.Right > sz.Width)
				{
					sz.Width = c.Bounds.Right;
				}
			}
			if (sz.Width > this.ClientSize.Width || sz.Height > this.ClientSize.Height)
			{
				this.ClientSize = sz;
			}
		}
		#endregion
		#region private methods
		private LinkLineNode HitTest(int x, int y)
		{
			LinkLineNode nodeTest = null;
			for (int i = 0; i < Controls.Count; i++)
			{
				LinkLineNode node = Controls[i] as LinkLineNode;
				if (node != null)
				{
					if (nodeTest == null)
					{
						if (node.HitTest(x, y))
						{
							nodeTest = node;
						}
						else
						{
							setLineSelection(node, false);
						}
					}
					else
					{
						setLineSelection(node, false);
					}
				}
			}
			return nodeTest;
		}
		private void setLineSelection(LinkLineNode node, bool selected)
		{
			if (node != null)
			{
				if (selected)
				{
					node.SelectLine(selected);
				}
				else
				{
					bool bCanDeSelect = true;
					if (_selectedLine != null)
					{
						if (_selectedLine.IsOnTheLine(node))
						{
							bCanDeSelect = false;
						}
					}
					if (bCanDeSelect)
					{
						node.SelectLine(selected);
					}
				}
			}
		}
		#endregion
		#region menu handlers
		private void miDisconnectNodes_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				NodeData nd = mi.Tag as NodeData;
				if (nd != null)
				{
					LinkLineNode node = nd.Node;

					if (node != null)
					{
						if (nd.Data != null)
						{
							if (nd.Data.GetType().Equals(typeof(Point)))
							{
								Point pt = (Point)nd.Data;
								if (node.Line != null)
								{
									//===================
									UInt32 key = node.ActiveDrawingID;
									UInt32 key1 = 0;
									UInt32 key2 = 0;
									LinkLineNodeInPort ip = node.LinkedInPort;
									node.BreakLine(pt, ref key1, ref key2);
									OnDisconnectLine(ip);
									if (UndoHost != null)
									{
										if (!UndoHost.DisableUndo)
										{
											LinkLineUndoReconnect undo = new LinkLineUndoReconnect(UndoHost, key, key1, key2);
											LinkLineUndoBreak redo = new LinkLineUndoBreak(UndoHost, key, key1, key2, pt);
											UndoHost.AddUndoEntity(new UndoEntity(undo, redo));
										}
									}
									ActiveDrawing.RefreshControl(this);
								}
							}
						}
					}
				}
			}
		}
		private void miAddLineNode_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				NodeData nd = mi.Tag as NodeData;
				if (nd != null)
				{
					LinkLineNode node = nd.Node;

					if (node != null)
					{
						if (nd.Data != null)
						{
							if (nd.Data.GetType().Equals(typeof(Point)))
							{
								Point pt = (Point)nd.Data;
								if (node.Line != null)
								{
									UInt32 key2 = node.ActiveDrawingID;
									LinkLineNode newNode = node.InsertNode(pt.X + 5, pt.Y + 5);
									if (UndoHost != null)
									{
										if (!UndoHost.DisableUndo)
										{
											UInt32 key1 = newNode.ActiveDrawingID;
											UndoEntity entity = new UndoEntity(
												new LinkLineUndoDelete(UndoHost, key1), new LinkLineUndoAdd(UndoHost, key2, pt));
											UndoHost.AddUndoEntity(entity);
										}
									}
									newNode.SaveLocation();
									newNode.Selected = true;
									ActiveDrawing.RefreshControl(this);
									OnLineNodeAdded(newNode);
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region Protected methods
		protected virtual void OnDisconnectLine(LinkLineNodeInPort port)
		{
		}
		#endregion
		#region event handlers
		private bool bMouseIn;
		protected override void OnPaint(PaintEventArgs e)
		{
			for (int i = 0; i < Controls.Count; i++)
			{
				LinkLineNode node = Controls[i] as LinkLineNode;
				if (node != null)
				{
					node.DrawLine(e.Graphics);
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			LinkLineNode nodeTest = HitTest(e.X, e.Y);
			if (nodeTest != null)
			{
				this.Cursor = Cursors.Hand;
				nodeTest.SelectLine(true);
			}
			else
			{
				this.Cursor = Cursors.Default;
			}
			if (!bMouseIn)
			{
				bMouseIn = true;
				for (int i = 0; i < Controls.Count; i++)
				{
					ActiveDrawing ad = Controls[i] as ActiveDrawing;
					if (ad != null)
					{
						ad.FireOnLeave(e);
					}
				}
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			LinkLineNode nodeTest = HitTest(e.X, e.Y);
			if (nodeTest != null)
			{
				nodeTest.SelectLine(true);
				nodeTest.InsertNode(e.X, e.Y);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			LinkLineNode nodeTest = HitTest(e.X, e.Y);
			if (nodeTest != null)
			{
				if (AllowLineSelection)
				{
					nodeTest.SelectLine(true);
					if (nodeTest.Start != null && nodeTest.End != null)
					{
						LinkLineNodeOutPort startNode = nodeTest.Start as LinkLineNodeOutPort;
						LinkLineNodeInPort endNode = nodeTest.End as LinkLineNodeInPort;
						if (startNode != null && endNode != null)
						{
							bool bChanged = true;
							if (_selectedLine != null)
							{
								if (_selectedLine.StartNode.PortID == startNode.PortID
									&& _selectedLine.StartNode.PortInstanceID == startNode.PortInstanceID
									&& _selectedLine.EndNode.PortID == endNode.PortID
									&& _selectedLine.EndNode.PortInstanceID == endNode.PortInstanceID)
								{
									bChanged = false;
								}
							}
							if (bChanged)
							{
								_selectedLine = new LinkLineEnds(startNode, endNode);
								OnLineSelectionChanged();
							}
						}
					}
				}
			}
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu mnu = new ContextMenu();
				Point pt = new Point(e.X, e.Y);
				if (nodeTest != null)
				{
					//nodeTest is the line holder
					nodeTest.SelectLine(true);
					MenuItemWithBitmap mi = new MenuItemWithBitmap("Add line join", Resource1.newLineBreak);
					mi.Click += new EventHandler(miAddLineNode_Click);
					mi.Tag = new NodeData(nodeTest, pt);
					mnu.MenuItems.Add(mi);
					//
					if (AllowLineDisconnect)
					{
						if (nodeTest.LinkedInPort != null && nodeTest.LinkedOutPort != null)
						{
							mi = new MenuItemWithBitmap("Disconnect", Resource1.disconnect);
							mi.Click += new EventHandler(miDisconnectNodes_Click);
							mi.Tag = new NodeData(nodeTest, pt);
							mnu.MenuItems.Add(mi);
						}
					}
					OnCreateContextMenuForLinkLine(nodeTest, mnu, pt);
				}
				else
				{
					OnCreateContextMenu(mnu, pt);
				}
				if (mnu.MenuItems.Count > 0)
				{
					_menuPoint = pt;
					mnu.Show(this, pt);
				}
			}
		}
		#endregion
	}
}
