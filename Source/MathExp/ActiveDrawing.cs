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
using TraceLog;
using VPL;
using XmlUtility;

namespace MathExp
{
	public interface IObjectCenter
	{
		Point Center { get; }
	}
	public interface IMessageReceiver
	{
		bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers);
		bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers);
		bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers);
		bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers);
		bool FireKeyDown(KeyEventArgs e);
		bool FireKeyUp(KeyEventArgs e);
	}
	public abstract class ActiveDrawing : Control, IObjectCenter, IMessageReceiver, IXmlNodeSerializable, ICloneable, IActiveDrawing, INonDesignSerializable
	{
		#region fields and constructors
		const string XMLATTR_ActID = "actId";
		const string XML_Location = "Location";
		const string XML_Size = "Size";
		public event EventHandler OnMouseSelect = null;
		private Point _location; //to load and reset location
		private object[] _constructorParameters; //for clone
		private UInt32 _id;
		protected bool _messageReturn = true;
		public ActiveDrawing()
		{
			_location = this.Location;
		}
		#endregion
		#region properties
		public bool IsMouseIn
		{
			get
			{
				return bMouseEnter;
			}
		}
		/// <summary>
		/// control ID for dynamic use, not persisted
		/// </summary>
		public UInt32 ActiveDrawingID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		public virtual bool NeedSaveSize
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region public methods
		public void RefreshParent()
		{
			RefreshControl(this.Parent);
		}
		public static void RefreshControl(Control c)
		{
			ScrollableControl sc = c as ScrollableControl;
			if (sc != null)
			{
				sc.Refresh();
			}
			else
			{
				if (c != null)
				{
					c.Refresh();
				}
			}
		}
		protected virtual void OnSelectByMouseDown()
		{
		}
		public virtual void ResetActiveDrawingID()
		{
			_id = (UInt32)Guid.NewGuid().GetHashCode();
		}
		public void SetIniLocation(Point loc)
		{
			_location = loc;
		}
		public virtual void SaveLocation()
		{
			_location = this.Location;
		}
		public virtual void RestoreLocation()
		{
			this.Location = _location;
		}
		public virtual void OnDesirialize()
		{
			this.Location = _location;
		}
		public virtual void OnRelativeDrawingMouseEnter(RelativeDrawing relDraw)
		{
		}
		public virtual void OnRelativeDrawingMouseLeave(RelativeDrawing relDraw)
		{
		}
		public virtual void OnRelativeDrawingMouseDown(RelativeDrawing relDraw, MouseEventArgs e)
		{
		}
		public virtual void OnRelativeDrawingMouseDoubleClick(RelativeDrawing relDraw, MouseEventArgs e)
		{
		}
		public virtual void OnRelativeDrawingMove(RelativeDrawing relDraw)
		{
		}
		#endregion
		#region private methods
		private void setChanged()
		{
			IChangeControl2 dv = this.Parent as IChangeControl2;
			if (dv != null)
			{
				dv.Changed = true;
			}
		}
		#endregion
		#region Mouse events
		protected int x0;
		protected int y0;
		protected bool bMD;
		private bool bMouseEnter;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			x0 = e.X;
			y0 = e.Y;
			bMD = true;
			_location = this.Location;
			Diagram dg = this.Parent as Diagram;
			if (dg != null)
			{
				dg.SetMouseDownControl(this);
			}
			if (OnMouseSelect != null)
			{
				OnMouseSelect(this, e);
			}
			base.OnMouseDown(e);
			OnSelectByMouseDown();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!bMouseEnter)
			{
				bMouseEnter = true;
				if (this.Parent != null)
				{
					for (int i = 0; i < this.Parent.Controls.Count; i++)
					{
						ActiveDrawing ad = this.Parent.Controls[i] as ActiveDrawing;
						if (ad != null && ad != this)
						{
							ad.FireOnLeave(EventArgs.Empty);
						}
					}
				}
				Diagram dg = this.Parent as Diagram;
				if (dg != null)
				{
					dg.OnChildActiveDrawingMouseEnter(this);
				}
				OnEnter(EventArgs.Empty);
			}
			const int MARGIN_D = 2;
			if (this.Parent != null && (e.Button & MouseButtons.Left) == MouseButtons.Left && bMD)
			{
				int offsetX = 0;
				int offsetY = 0;
				bool b = false;
				if (!this.Capture)
					this.Capture = true;
				int x = this.Left + e.X - x0;
				int xd = this.Left;
				int yd = this.Top;
				Point p0 = this.Parent.PointToClient(this.PointToScreen(e.Location));
				if (p0.X > MARGIN_D && p0.X < this.Parent.ClientSize.Width - MARGIN_D)
				{
					xd = x;
					b = true;
				}
				int y = this.Top + e.Y - y0;
				if (p0.Y > MARGIN_D && p0.Y < this.Parent.ClientSize.Height - MARGIN_D)
				{
					yd = y;
					b = true;
				}
				if (b)
				{
					this.Location = new Point(xd - offsetX, yd - offsetY);
					RefreshParent();
				}
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (bMD && this.Capture)
			{
				IMathDesigner p = this.Parent as IMathDesigner;
				if (p != null && !p.DisableUndo)
				{
					UndoEntity entity = new UndoEntity(new PositionUndo(p, this.ActiveDrawingID, _location), new PositionUndo(p, this.ActiveDrawingID, this.Location));
					p.AddUndoEntity(entity);
					_location = this.Location;
				}
			}
			bMD = false;
			this.Capture = false;
			Diagram dg = this.Parent as Diagram;
			if (dg != null)
			{
				dg.SetMouseUpControl();
			}
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			setChanged();
		}
		#endregion
		#region IObjectCenter members
		public virtual Point Center
		{
			get
			{
				return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
			}
		}
		#endregion
		#region IMessageReceiver members
		public void FireOnLeave(EventArgs e)
		{
			if (bMouseEnter)
			{
				bMouseEnter = false;
				OnLeave(e);
			}
		}
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDown(e);
			return _messageReturn;
		}
		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseMove(e);
			return _messageReturn;
		}
		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseUp(e);
			return _messageReturn;
		}
		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDoubleClick(e);
			return _messageReturn;
		}
		public bool FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			return _messageReturn;
		}
		public bool FireKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
			return _messageReturn;
		}
		#endregion
		#region IXmlNodeSerializable Members

		public virtual void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			ObjectXmlReader xr = new ObjectXmlReader();
			Point l;
			if (xr.ReadValueFromChildNode<Point>(node, XML_Location, out l))
			{
				_location = l;
			}
			else
			{
				_location = this.Location;
			}
			if (NeedSaveSize)
			{
				Size s;
				if (xr.ReadValueFromChildNode<Size>(node, XML_Size, out s))
				{
					this.Size = s;
				}
			}
			if (_location.X < 0)
				_location.X = 0;
			if (_location.Y < 0)
				_location.Y = 0;
			this.Location = _location;
		}

		public virtual void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlSerialization.WriteValueToChildNode(node, XML_Location, _location);
			if (NeedSaveSize)
			{
				XmlSerialization.WriteValueToChildNode(node, XML_Size, this.Size);
			}
		}

		#endregion
		#region ICloneable Members
		public object[] ConstructorParameters
		{
			set { _constructorParameters = value; }
		}
		public virtual object Clone()
		{
			ActiveDrawing obj;
			if (_constructorParameters == null)
				obj = (ActiveDrawing)Activator.CreateInstance(this.GetType());
			else
				obj = (ActiveDrawing)Activator.CreateInstance(this.GetType(), _constructorParameters);
			obj.Location = this.Location;
			obj.SetIniLocation(_location);
			obj.Size = this.Size;
			obj.ActiveDrawingID = this.ActiveDrawingID;
			return obj;
		}

		#endregion
		#region INonDesignSerializable Members

		public virtual bool ShouldSerialize
		{
			get { return true; }
		}

		#endregion
	}
}
