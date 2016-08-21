/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * Copyrights (C) Longflow Enterprises Ltd. All rights reserved.
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

namespace MathExp
{
	public class RelativePosition : ICloneable
	{
		int x;
		int y;
		bool xTo0;
		bool yTo0;
		Control c;
		bool _moving;
		public RelativePosition(Control rel, int x0, int y0, bool isXto0, bool isYto0)
		{
			c = rel;
			x = x0;
			y = y0;
			xTo0 = isXto0;
			yTo0 = isYto0;
		}
		public void SavePosition(Point ownerPos, Size size)
		{
			if (_moving)
				return;
			if (c.Left > ownerPos.X + size.Width)
			{
				x = c.Left - ownerPos.X - size.Width;
				xTo0 = false;
			}
			else
			{
				x = c.Left - ownerPos.X;
				xTo0 = true;
			}
			if (c.Top > ownerPos.Y + size.Height)
			{
				y = c.Top - ownerPos.Y - size.Height;
				yTo0 = false;
			}
			else
			{
				y = c.Top - ownerPos.Y;
				yTo0 = true;
			}
		}
		public void AdjustPosition(Point ownerPos, Size size)
		{
			int x0;
			int y0;
			if (xTo0)
			{
				x0 = ownerPos.X + x;
			}
			else
			{
				x0 = ownerPos.X + size.Width + x;
			}
			if (yTo0)
			{
				y0 = ownerPos.Y + y;
			}
			else
			{
				y0 = ownerPos.Y + size.Height + y;
			}
			if (x0 > ownerPos.X - 5 && x0 < ownerPos.X + size.Width &&
				y0 > ownerPos.Y - 5 && y0 < ownerPos.Y + size.Height)
			{
				x0 = ownerPos.X + size.Width;
				y0 = ownerPos.Y + size.Height;
			}
			_moving = true;
			c.Location = new Point(x0, y0);
			_moving = false;
		}
		public Point Location
		{
			get
			{
				return new Point(x, y);
			}
			set
			{
				x = value.X;
				y = value.Y;
			}
		}
		public bool IsXto0
		{
			get
			{
				return xTo0;
			}
			set
			{
				xTo0 = value;
			}
		}
		public bool IsYto0
		{
			get
			{
				return yTo0;
			}
			set
			{
				yTo0 = value;
			}
		}
		public void SetOwner(Control c0)
		{
			c = c0;
		}
		#region ICloneable Members

		public object Clone()
		{
			return new RelativePosition(c, x, y, xTo0, yTo0);
		}

		#endregion
	}
	public abstract class RelativeDrawing : ActiveDrawing
	{
		private RelativePosition _relativePosition;
		private bool _moveDeLinked;
		protected Control _owner;
		public RelativeDrawing()
		{
			_relativePosition = new RelativePosition(this, -20, 20, true, true);
		}
		public RelativeDrawing(Control owner)
		{
			_relativePosition = new RelativePosition(this, -20, 20, true, true);
			SetOwner(owner);
		}
		public abstract UInt32 OwnerID { get; }
		public void SetOwner(Control owner)
		{
			_owner = owner;
			_owner.Move += new EventHandler(_owner_Move);
			_owner.Resize += new EventHandler(_owner_Resize);
		}
		public void SetMoveUnLink(bool unlink)
		{
			_moveDeLinked = unlink;
		}
		void _owner_Resize(object sender, EventArgs e)
		{
			AdjustPosition();
		}
		void _owner_Move(object sender, EventArgs e)
		{
			if (_moveDeLinked)
			{
				SaveRelativePosition();
			}
			else
			{
				AdjustPosition();
			}
		}
		public void AdjustPosition()
		{
			if (_owner != null)
			{
				if (_relativePosition == null)
					_relativePosition = new RelativePosition(this, 20, 20, true, true);
				_relativePosition.AdjustPosition(_owner.Location, _owner.Size);
			}
		}
		public Control RelativeOwner
		{
			get
			{
				return _owner;
			}
		}
		public RelativePosition RelativePosition
		{
			get
			{
				if (_relativePosition == null)
					_relativePosition = new RelativePosition(this, 20, 20, true, true);
				return _relativePosition;
			}
			set
			{
				_relativePosition = value;
				_relativePosition.SetOwner(this);
				AdjustPosition();
			}
		}
		public void SaveRelativePosition()
		{
			if (_owner != null)
			{
				_relativePosition.SavePosition(_owner.Location, _owner.Size);
			}
		}
		public override void SaveLocation()
		{
			base.SaveLocation();
			SaveRelativePosition();
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			ActiveDrawing ad = _owner as ActiveDrawing;
			if (ad != null)
			{
				ad.OnRelativeDrawingMouseDoubleClick(this, e);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			SaveRelativePosition();
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			ActiveDrawing ad = _owner as ActiveDrawing;
			if (ad != null)
			{
				ad.OnRelativeDrawingMouseDown(this, e);
			}
		}
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			ActiveDrawing ad = _owner as ActiveDrawing;
			if (ad != null)
			{
				ad.OnRelativeDrawingMouseEnter(this);
			}
			Diagram dv = this.Parent as Diagram;
			if (dv != null)
			{
				dv.OnChildActiveDrawingMouseEnter(this);
			}
		}
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			ActiveDrawing ad = _owner as ActiveDrawing;
			if (ad != null)
			{
				ad.OnRelativeDrawingMouseLeave(this);
			}
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			SaveLocation();
			SaveRelativePosition();
			ActiveDrawing ad = _owner as ActiveDrawing;
			if (ad != null)
			{
				ad.OnRelativeDrawingMove(this);
			}
		}
		#region IXmlNodeSerializable Members
		const string XML_RelativePosition = "RelativePosition";
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			object v;
			if (XmlSerialization.ReadValueFromChildNode(node, XML_RelativePosition, out v))
			{
				if (_relativePosition == null)
				{
					_relativePosition = new RelativePosition(this, 20, 20, true, true);
				}
				_relativePosition.Location = (Point)v;
				XmlNode nd = node.SelectSingleNode(XML_RelativePosition);
				_relativePosition.IsXto0 = XmlSerialization.GetAttributeBool(nd, "xTo0", true);
				_relativePosition.IsYto0 = XmlSerialization.GetAttributeBool(nd, "yTo0", true);
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.WriteValueToChildNode(node, XML_RelativePosition, _relativePosition.Location);
			XmlNode nd = node.SelectSingleNode(XML_RelativePosition);
			XmlSerialization.SetAttribute(nd, "xTo0", _relativePosition.IsXto0);
			XmlSerialization.SetAttribute(nd, "yTo0", _relativePosition.IsYto0);
		}
		#endregion
		#region ICloneable
		public override object Clone()
		{
			if (_owner == null)
				ConstructorParameters = null;
			else
				ConstructorParameters = new object[] { _owner };
			RelativeDrawing clone = (RelativeDrawing)base.Clone();
			clone.RelativePosition = (RelativePosition)_relativePosition.Clone();
			return clone;
		}
		#endregion
	}
}
