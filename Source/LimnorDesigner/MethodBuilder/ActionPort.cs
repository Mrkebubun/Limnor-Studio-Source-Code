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
using MathExp;
using System.Drawing;
using System.Xml;
using VPLDrawing;
using XmlSerializer;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	[UseParentObject]
	public class ActionPortIn : LinkLineNodeInPort, ICustomSerialization
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.Red;

		public ActionPortIn(IPortOwner owner)
			: base(owner)
		{
			this.Size = new System.Drawing.Size(11, 11);
			LabelVisible = false;
		}
		#endregion
		#region properties
		public override bool RemoveLineJoint
		{
			get
			{
				return true;
			}
		}
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return true;
			}
		}
		public Brush DrawBrush
		{
			get
			{
				return _drawBrush;
			}
			set
			{
				_drawBrush = value;
			}
		}
		public override bool CanAssignValue
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region IObjectCenter members
		public override Point Center
		{
			get
			{
				return new Point(this.Left + this.Width / 2, this.Top + 3);
			}
		}
		#endregion
		#region methods
		protected override enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			return ActionViewerIF.GetPosTypeByCornerPos(cornerIndex);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			VplDrawing.DrawInArrow(e.Graphics, _drawBrush, this.Size, this.PositionType);
		}


		#endregion
		#region IXmlNodeSerializable Members
		object _serializer;
		public void SetSerializer(object serializer)
		{
			_serializer = serializer;
		}
		public object Serializer
		{
			get
			{
				return _serializer;
			}
		}
		#endregion
	}
	[UseParentObject]
	public class ActionPortOut : LinkLineNodeOutPort, ICustomSerialization
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.Red;
		private bool _removeLineJoint = true;
		public ActionPortOut(IPortOwner owner)
			: base(owner)
		{
			this.Size = new System.Drawing.Size(11, 11);
			LabelVisible = false;
		}
		#endregion
		#region IObjectCenter members
		public override Point Center
		{
			get
			{
				return new Point(this.Left + this.Width / 2, this.Top + 6);
			}
		}
		#endregion
		#region properties
		public override bool RemoveLineJoint
		{
			get
			{
				return _removeLineJoint;
			}
		}
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return false;
			}
		}
		public Brush DrawBrush
		{
			get
			{
				return _drawBrush;
			}
			set
			{
				_drawBrush = value;
			}
		}
		#endregion
		#region methods
		protected override enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			return ActionViewerIF.GetPosTypeByCornerPos(cornerIndex);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			VplDrawing.DrawOutArrow(e.Graphics, _drawBrush, this.Size, this.PositionType);
		}
		public void SetRemoveLineJoint(bool remove)
		{
			_removeLineJoint = remove;
		}
		#endregion
		#region IXmlNodeSerializable Members
		object _serializer;
		public void SetSerializer(object serializer)
		{
			_serializer = serializer;
		}
		public object Serializer
		{
			get
			{
				return _serializer;
			}
		}
		#endregion
	}
}
