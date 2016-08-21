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
using System.Drawing;
using MathExp;
using VPLDrawing;
using System.Drawing.Drawing2D;
using LimnorDesigner.Action;
using VSPrj;
using XmlSerializer;
using System.Xml;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerIF : ActionViewer
	{
		public const int CORNER_TOP = 0;
		public const int CORNER_LEFT = 1;
		public const int CORNER_RIGHT = 2;
		public const int CORNER_BOTTOM = 3;
		private Point[] _portPostions;
		private Brush _questionMarkBrush;
		private Font _questionMarkFont;
		public ActionViewerIF()
		{
			AddPropertyName("DrawingStyle");
		}
		public static enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			switch (cornerIndex)
			{
				case CORNER_TOP:
					return enumPositionType.Top;
				case CORNER_LEFT:
					return enumPositionType.Left;
				case CORNER_BOTTOM:
					return enumPositionType.Bottom;
				case CORNER_RIGHT:
					return enumPositionType.Right;
			}
			return enumPositionType.Top;
		}
		[Description("Indicates how to display this action on the screen")]
		public EnumDrawAction DrawingStyle
		{
			get
			{
				return ((IBranchingAction)ActionObject).DrawingStyle;
			}
			set
			{
				((IBranchingAction)ActionObject).DrawingStyle = value;
			}
		}
		private void calculatePortPositions()
		{
			if (!IsLoading)
			{
				_portPostions = new Point[4];
				_portPostions[CORNER_TOP] = new Point(this.ClientSize.Width / 2, 0);
				_portPostions[CORNER_LEFT] = new Point(0, this.ClientSize.Height / 2);
				_portPostions[CORNER_RIGHT] = new Point(this.ClientSize.Width, this.ClientSize.Height / 2);
				_portPostions[CORNER_BOTTOM] = new Point(this.ClientSize.Width / 2, this.ClientSize.Height);
				if (this.ActionObject != null)
				{
					this.ActionObject.InPortList[0].Corners = _portPostions;
					this.ActionObject.OutPortList[0].Corners = _portPostions;
					this.ActionObject.OutPortList[1].Corners = _portPostions;
				}
			}
		}
		private void onConditionChanged(object sender, EventArgs e)
		{
			CreateImage();
			this.Refresh();
		}
		public Point[] PortPositions
		{
			get
			{
				if (_portPostions == null)
					calculatePortPositions();
				return _portPostions;
			}
		}
		protected override bool CanEditAction
		{
			get
			{
				return false;
			}
		}
		protected override void OnImportAction()
		{
			base.OnImportAction();
			AB_ConditionBranch branch = this.ActionObject as AB_ConditionBranch;
			if (branch != null)
			{
				branch.SetOnConditionChanged(onConditionChanged);
			}
		}
		protected override void OnInitNewPorts()
		{
			OnInitExistingPorts();
			this.ActionObject.InitializePortPositions(this);
		}
		protected override void OnInitExistingPorts()
		{
			calculatePortPositions();
			this.ActionObject.InPortList[0].FixedLocation = true;
			this.ActionObject.OutPortList[0].FixedLocation = true;
			this.ActionObject.OutPortList[0].LabelVisible = true;
			((DrawingVariable)this.ActionObject.OutPortList[0].Label).Variable.VariableName = "Yes";
			this.ActionObject.OutPortList[1].FixedLocation = true;
			this.ActionObject.OutPortList[1].LabelVisible = true;
			((DrawingVariable)this.ActionObject.OutPortList[1].Label).Variable.VariableName = "No";
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			calculatePortPositions();
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//draw a background question mark
			Point[] corners = PortPositions;
			double x = (corners[CORNER_RIGHT].X - corners[CORNER_TOP].X);
			double y = (corners[CORNER_RIGHT].Y - corners[CORNER_TOP].Y);

			RectangleF rc = new RectangleF(
				(float)(corners[CORNER_LEFT].X + x / 2),
				(float)(corners[CORNER_TOP].Y + y / 2),
				(float)x,
				(float)y);
			StringFormat sf = new StringFormat(StringFormatFlags.FitBlackBox);
			if (_questionMarkBrush == null)
				_questionMarkBrush = new SolidBrush(Color.LightGray);
			if (_questionMarkFont == null)
				_questionMarkFont = new Font("Time New Roman", 12);
			e.Graphics.DrawString("?", _questionMarkFont, _questionMarkBrush, rc, sf);
		}
		protected override void OnPaintActionView(PaintEventArgs e)
		{
			OnPaintValidIcon(e);
			string s;
			SizeF size;
			float x, y;
			Point[] corners = PortPositions;
			IBranchingAction ba;
			switch (DrawingStyle)
			{
				case EnumDrawAction.ActionName:
					ba = ActionObject as IBranchingAction;
					if (ba != null)
					{
						ba.OnPaintName(e.Graphics, (float)this.Width, (float)this.Height);
					}
					break;
				case EnumDrawAction.Description:
					if (string.IsNullOrEmpty(Description))
					{
						s = ActionName;
					}
					else
					{
						s = Description;
					}
					size = e.Graphics.MeasureString(s, ActionObject.TextFont);
					x = ((float)this.Width - size.Width) / (float)2;
					if (x < 0) x = 0;
					y = ((float)this.Height - size.Height) / (float)2;
					if (y < 0) y = 0;
					e.Graphics.DrawString(s, ActionObject.TextFont, new SolidBrush(ActionObject.TextColor), new RectangleF(x, y, this.Width, this.Height));
					break;
				case EnumDrawAction.Image:
					//draw action image
					if (ActionImage != null)
					{
						Rectangle dest = new Rectangle(
							corners[CORNER_TOP].X / 2,
							corners[CORNER_LEFT].Y / 2,
							corners[CORNER_RIGHT].X / 2,
							corners[CORNER_BOTTOM].Y / 2);
						e.Graphics.DrawImage(ActionImage,
							dest,
							(float)0, (float)0,
							(float)ActionImage.Width,
							(float)ActionImage.Height,
							GraphicsUnit.Pixel);
					}
					else
					{
						//draw condition text
						ba = ActionObject as IBranchingAction;
						if (ba != null)
						{
							ba.OnPaintBox(e.Graphics, corners[CORNER_TOP].X, corners[CORNER_LEFT].Y);
						}
					}
					break;
			}
			//draw frame
			e.Graphics.DrawLine(Pens.Blue, corners[CORNER_TOP], corners[CORNER_LEFT]);
			e.Graphics.DrawLine(Pens.Blue, corners[CORNER_LEFT], corners[CORNER_BOTTOM]);
			e.Graphics.DrawLine(Pens.Blue, corners[CORNER_BOTTOM], corners[CORNER_RIGHT]);
			e.Graphics.DrawLine(Pens.Blue, corners[CORNER_RIGHT], corners[CORNER_TOP]);
			//
			e.Graphics.DrawLine(this.ShapePen, corners[CORNER_BOTTOM].X, corners[CORNER_BOTTOM].Y + 1, corners[CORNER_RIGHT].X, corners[CORNER_RIGHT].Y + 1);
		}
	}
}
