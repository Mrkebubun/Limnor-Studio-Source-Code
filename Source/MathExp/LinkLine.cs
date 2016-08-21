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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace MathExp
{

	public enum enumLineState { Design, Unexecuted, BreakPoint, Executed }
	public class LinkLineDrawAtts
	{
		public System.Drawing.Color c = System.Drawing.Color.Blue;
		public System.Drawing.Brush br = System.Drawing.Brushes.Blue;
		public System.Drawing.Pen pen = System.Drawing.Pens.Blue;
		public System.Drawing.Color cNormal = System.Drawing.Color.Blue;
		public System.Drawing.Color cHighlight = System.Drawing.Color.DarkBlue;
		//
		static public LinkLineDrawAtts Normal;
		static public LinkLineDrawAtts Selected;
		static public LinkLineDrawAtts NotExecuted;
		static public LinkLineDrawAtts Executed;
		static public LinkLineDrawAtts BreakPoint;
		static LinkLineDrawAtts()
		{
			Normal = new LinkLineDrawAtts();
			Selected = new LinkLineDrawAtts();
			Selected.c = System.Drawing.Color.Yellow;
			Selected.br = System.Drawing.Brushes.Yellow;
			Selected.pen = System.Drawing.Pens.Yellow;
			Selected.cNormal = System.Drawing.Color.Yellow;
			Selected.cHighlight = System.Drawing.Color.DarkOrange;
			//
			NotExecuted = new LinkLineDrawAtts();
			NotExecuted.c = System.Drawing.Color.Gray;
			NotExecuted.br = System.Drawing.Brushes.Gray;
			NotExecuted.pen = new Pen(NotExecuted.br);
			NotExecuted.pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
			NotExecuted.cNormal = System.Drawing.Color.Gray;
			NotExecuted.cHighlight = System.Drawing.Color.Gray;
			//
			Executed = new LinkLineDrawAtts();
			Executed.c = System.Drawing.Color.Green;
			Executed.br = System.Drawing.Brushes.Green;
			Executed.pen = new Pen(Executed.br);
			Executed.cNormal = System.Drawing.Color.Green;
			Executed.cHighlight = System.Drawing.Color.Green;
			//
			BreakPoint = new LinkLineDrawAtts();
			BreakPoint.c = System.Drawing.Color.Red;
			BreakPoint.br = System.Drawing.Brushes.Red;
			BreakPoint.pen = new Pen(BreakPoint.br);
			BreakPoint.pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			BreakPoint.cNormal = System.Drawing.Color.Red;
			BreakPoint.cHighlight = System.Drawing.Color.Red;
		}
		public LinkLineDrawAtts()
		{
		}
	}
	/// <summary>
	/// one line linking two controls
	/// </summary>
	public class LinkLine
	{
		public static bool ShowArrows;
		protected System.Windows.Forms.Control Owner = null;
		protected System.Drawing.Size Size = new Size(10, 10);
		protected bool bShifted = false;
		protected int Left = 0;
		protected int Right = 0;
		protected int Top = 0;
		protected int Bottom = 0;
		protected double a = 1.0;
		protected double b = 0.0;
		protected double kc = 1.0;
		protected double ht = 0.0;
		//
		public event System.EventHandler OnDrawMouseDown = null;
		protected double arrowSize = 8;
		protected int dotSize = 6;
		double arrowHeight = 4;
		double arrowHeight2 = 16;
		double arrowWidth2 = 48;
		double arrowWidth = Math.Sqrt(64 * 3) / 2;
		protected int nObjectSize = 5;
		protected double px0 = 0;
		protected double py0 = 0;
		protected double px1 = 0;
		protected double py1 = 0;
		protected double x0 = 0;
		protected double y0 = 0;
		protected double x1 = 0;
		protected double y1 = 0;
		protected int nOffsetX = 0;
		protected int nOffsetY = 0;
		protected System.Drawing.Point[] pts = new Point[4];
		//drawing properties=========================================================
		protected System.Drawing.Color c;
		protected System.Drawing.Brush br;
		protected System.Drawing.Pen pen;
		protected System.Drawing.Color cNormal;
		protected System.Drawing.Color cHighlight;
		protected System.Drawing.Color cGroup;
		//===========================================================================
		protected bool bSelected = false;
		protected bool bVisible = true;
		protected bool bHighlighted = false;
		//
		protected float fWidth = 2;
		protected double arrowSizeNormal = 8;
		protected int dotSizeNormal = 6;
		protected double arrowSizeHighlight = 10;
		protected int dotSizeHighlight = 8;
		protected float fWidthNormal = 2;
		protected float fWidthHighlight = 3;
		//
		protected System.Drawing.Point scrollPos = new Point(0, 0);
		protected ILinkLineNode objStart = null;
		protected ILinkLineNode objEnd = null;
		public enumLineState State = enumLineState.Unexecuted;
		public LinkLine()
		{
			//
			//
			arrowHeight = arrowSize / 2;
			arrowHeight2 = arrowSize * arrowSize / 4;
			arrowWidth2 = arrowSize * arrowSize * 3 / 4;
			arrowWidth = Math.Sqrt(arrowSize * arrowSize * 3) / 2;
			//
			c = System.Drawing.Color.Blue;
			br = System.Drawing.Brushes.Blue;
			pen = System.Drawing.Pens.Blue;
			cNormal = System.Drawing.Color.Blue;
			cHighlight = System.Drawing.Color.DarkBlue;
			cGroup = System.Drawing.Color.Cyan;
			fWidth = 1;
			fWidthNormal = 1;
			//
			Calculate();
		}
		public void AdjustLineEndVisibility()
		{
			StartPointVisible = (objStart.PrevNode == null);
			EndPointVisible = (objEnd.NextNode == null);
		}
		public void SetLineColor(Color cl)
		{
			c = cl;
			br = new SolidBrush(c);
			cNormal = c;
			pen = new Pen(c);
		}
		public ILinkLineNode StartPoint
		{
			get
			{
				return objStart;
			}
		}
		public ILinkLineNode EndPoint
		{
			get
			{
				return objEnd;
			}
		}
		private bool _drawDot = true;
		public bool StartPointVisible
		{
			get
			{
				return _drawDot;
			}
			set
			{
				_drawDot = value;
			}
		}
		private bool _drawArrow = true;
		public bool EndPointVisible
		{
			get
			{
				return _drawArrow;
			}
			set
			{
				_drawArrow = value;
			}
		}
		private bool _hitOnDot = true;
		public bool HitOnDot
		{
			get
			{
				return _hitOnDot;
			}
			set
			{
				_hitOnDot = value;
			}
		}
		public bool Visible
		{
			get
			{
				return bVisible;
			}
			set
			{
				bVisible = false;
			}
		}
		public virtual void Refresh()
		{
			if (Owner != null)
			{
				if (!Owner.IsDisposed)
				{
					if (Right == Left)
					{
						Owner.Invalidate(new System.Drawing.Rectangle(Left - 4, Top - 3, 8, Bottom - Top + 6));
					}
					else if (Bottom == Top)
					{
						Owner.Invalidate(new System.Drawing.Rectangle(Left - 3, Top - 4, Right - Left + 6, 8));
					}
					else
					{
						Owner.Invalidate(new System.Drawing.Rectangle(Left - 4, Top - 4, Right - Left + 8, Bottom - Top + 8));
					}
				}
			}
		}
		public void ResetUnexecuted()
		{
			c = LinkLineDrawAtts.NotExecuted.c;
			br = LinkLineDrawAtts.NotExecuted.br;
			cNormal = LinkLineDrawAtts.NotExecuted.cNormal;
			cHighlight = LinkLineDrawAtts.NotExecuted.cHighlight;
			pen = new Pen(br, fWidth);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
			this.Refresh();
		}
		public void ResetExecuted()
		{
			c = LinkLineDrawAtts.Executed.c;
			br = LinkLineDrawAtts.Executed.br;
			cNormal = LinkLineDrawAtts.Executed.cNormal;
			cHighlight = LinkLineDrawAtts.Executed.cHighlight;
			pen = new Pen(br, fWidth);
			this.Refresh();
		}
		public void ResetBreakPoint()
		{
			c = LinkLineDrawAtts.BreakPoint.c;
			br = LinkLineDrawAtts.BreakPoint.br;
			cNormal = LinkLineDrawAtts.BreakPoint.cNormal;
			cHighlight = LinkLineDrawAtts.BreakPoint.cHighlight;
			pen = new Pen(br, fWidth);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			this.Refresh();
		}
		public void Select()
		{
			if (!bSelected)
			{
				bSelected = true;
				c = LinkLineDrawAtts.Selected.c;
				br = LinkLineDrawAtts.Selected.br;
				cNormal = LinkLineDrawAtts.Selected.cNormal;
				cHighlight = LinkLineDrawAtts.Selected.cHighlight;
				pen = new Pen(br, fWidth);
				this.Refresh();
			}
		}
		public void Reset(bool RunMode)
		{
			if (RunMode)
			{
				switch (State)
				{
					case enumLineState.BreakPoint:
						ResetBreakPoint();
						break;
					case enumLineState.Executed:
						ResetExecuted();
						break;
					case enumLineState.Unexecuted:
						ResetUnexecuted();
						break;
				}
			}
			else
			{
				c = LinkLineDrawAtts.Normal.c;
				br = LinkLineDrawAtts.Normal.br;
				cNormal = LinkLineDrawAtts.Normal.cNormal;
				cHighlight = LinkLineDrawAtts.Normal.cHighlight;
				pen = new Pen(br, fWidth);
			}
			this.Refresh();
		}
		public void Unselect(bool runMode)
		{
			if (bSelected)
			{
				bSelected = false;
				Reset(runMode);
			}
		}
		public void AssignMapObjects(ILinkLineNode src, ILinkLineNode tgt)
		{
			objStart = src;
			objEnd = tgt;
			Control c = src as Control;
			if (c != null)
				c.Move += new EventHandler(src_Move);
			c = tgt as Control;
			if (c != null)
				c.Move += new EventHandler(tgt_Move);
			this.SetEnds(objStart.Center.X - scrollPos.X, objStart.Center.Y - scrollPos.Y, objEnd.Center.X - scrollPos.X, objEnd.Center.Y - scrollPos.Y);
		}

		public bool Grouped
		{
			get
			{
				return (c == cGroup);
			}
			set
			{
				if (value)
				{
					if (c != cGroup)
					{
						c = cGroup;
						br = new SolidBrush(c);
						pen = new Pen(br, fWidth);
					}
				}
				else
				{
					if (this.Highlighted)
					{
						c = cHighlight;
					}
					else
					{
						c = cNormal;
					}
					br = new SolidBrush(c);
					pen = new Pen(br, fWidth);
				}
			}
		}
		public virtual bool Highlighted
		{
			get
			{
				return bHighlighted;
			}
			set
			{
				if (bHighlighted != value)
				{
					bHighlighted = value;
					if (bHighlighted)
					{
						arrowSize = arrowSizeHighlight;
						dotSize = dotSizeHighlight;
						if (c != cGroup)
						{
							c = cHighlight;
						}
						fWidth = fWidthHighlight;
					}
					else
					{
						arrowSize = arrowSizeNormal;
						dotSize = dotSizeNormal;
						if (c != cGroup)
						{
							c = cNormal;
						}
						fWidth = fWidthNormal;
					}
					Recalculate();
					Refresh();
				}
			}
		}
		public void SetDrawOwner(Control frmOwner)
		{
			this.Owner = frmOwner;
			frmOwner.Move += new EventHandler(this.frmOwner_Move);
			frmOwner.Resize += new EventHandler(this.frmOwner_Resize);
			//
			this.Size = new Size(frmOwner.ClientSize.Width, frmOwner.ClientSize.Height);
			this.SetEnds(objStart.Center.X - scrollPos.X, objStart.Center.Y - scrollPos.Y, objEnd.Center.X - scrollPos.X, objEnd.Center.Y - scrollPos.Y);
			Calculate();
		}
		public System.Drawing.Color DrawColor
		{
			get
			{
				return c;
			}
			set
			{
				c = value;
				br = new SolidBrush(c);
				pen = new Pen(br, fWidth);
			}
		}
		public float DrawLineWidth
		{
			get
			{
				return fWidth;
			}
			set
			{
				fWidth = value;
				pen = new Pen(br, fWidth);
			}
		}
		public int DrawDotSize
		{
			get
			{
				return dotSize;
			}
			set
			{
				dotSize = value;
			}
		}
		public double ArrowSize
		{
			get
			{
				return arrowSize;
			}
			set
			{
				arrowSize = value;
				arrowHeight = arrowSize / 2;
				arrowHeight2 = arrowSize * arrowSize / 4;
				arrowWidth2 = arrowSize * arrowSize * 3 / 4;
				arrowWidth = Math.Sqrt(arrowSize * arrowSize * 3) / 2;
				Calculate();
			}
		}
		public void SetStart(ILinkLineNode obj)
		{
			if (objStart != null)
			{
				Control c = objStart as Control;
				if (c != null)
					c.Move -= new EventHandler(src_Move);
			}
			objStart = obj;
			if (objStart != null)
			{
				Control c = objStart as Control;
				if (c != null)
					c.Move += new EventHandler(src_Move);
				SetStart(objStart.Center.X - scrollPos.X, objStart.Center.Y - scrollPos.Y);
			}
		}
		public virtual void SetStart(double x, double y)
		{
			px0 = x;
			py0 = y;
			Calculate();
		}
		public void SetEnd(ILinkLineNode obj)
		{
			if (objEnd != null)
			{
				Control c = objEnd as Control;
				if (c != null)
					c.Move -= new EventHandler(tgt_Move);
			}
			objEnd = obj;
			if (objEnd != null)
			{
				Control c = objEnd as Control;
				if (c != null)
					c.Move += new EventHandler(tgt_Move);
				SetEnd(objEnd.Center.X - scrollPos.X, objEnd.Center.Y - scrollPos.Y);
			}
		}
		public virtual void SetEnd(double x, double y)
		{
			px1 = x;
			py1 = y;
			Calculate();
		}
		public void SetEnds(ILinkLineNode start, ILinkLineNode end)
		{
			Control c;
			if (objStart != null)
			{
				c = objStart as Control;
				if (c != null)
					c.Move -= new EventHandler(src_Move);
			}
			objStart = start;
			c = objStart as Control;
			if (c != null)
				c.Move += new EventHandler(src_Move);
			if (objEnd != null)
			{
				c = objEnd as Control;
				if (c != null)
					c.Move -= new EventHandler(tgt_Move);
			}
			objEnd = end;
			c = objEnd as Control;
			if (c != null)
				c.Move += new EventHandler(tgt_Move);
			this.SetEnds(objStart.Center.X - scrollPos.X, objStart.Center.Y - scrollPos.Y, objEnd.Center.X - scrollPos.X, objEnd.Center.Y - scrollPos.Y);
		}
		public virtual void SetEnds(double X0, double Y0, double X1, double Y1)
		{
			px0 = X0;
			py0 = Y0;
			px1 = X1;
			py1 = Y1;
			Calculate();
		}
		public virtual void Shift()
		{
			if (!bShifted)
			{
				bShifted = true;

				calculateShift();
			}
		}
		public void SetShift()
		{
			bShifted = true;
		}
		public bool Shifted
		{
			get
			{
				return bShifted;
			}
		}
		public int ObjectSize
		{
			get
			{
				return nObjectSize;
			}
			set
			{
				nObjectSize = value;
				Calculate();
			}
		}
		public System.Drawing.Point[] GetArrowPoints()
		{
			return pts;
		}
		public virtual void Draw(System.Drawing.Graphics g)
		{
			if (!bVisible)
				return;
			bool b = true;
			int X0 = Convert.ToInt32(x0);
			int Y0 = Convert.ToInt32(y0);
			int X1 = Convert.ToInt32(x1);
			int Y1 = Convert.ToInt32(y1);
			if (X0 < 0)
			{
				b = false;
				//(x0-x1)*(y-y0) = (x-x0)*(y0-y1)
				if (X0 != X1)
				{
					Y0 = (int)(y0 - x0 * (y0 - y1) / (x0 - x1));
				}
				X0 = 0;
			}
			else if (X0 > this.Size.Width)
			{
				b = false;
				if (X0 != X1)
				{
					Y0 = (int)(y0 + (this.Size.Width - x0) * (y0 - y1) / (x0 - x1));
				}
				X0 = this.Size.Width;
			}
			if (Y0 < 0)
			{
				b = false;
				//(x0-x1)*(y-y0) = (x-x0)*(y0-y1)
				if (Y0 != Y1)
				{
					X0 = (int)(x0 - y0 * (x0 - x1) / (y0 - y1));
				}
				Y0 = 0;
			}
			else if (Y0 > this.Size.Height)
			{
				b = false;
				if (Y0 != Y1)
				{
					X0 = (int)(x0 + (this.Size.Height - y0) * (x0 - x1) / (y0 - y1));
				}
				Y0 = this.Size.Height;
			}
			//draw dot indicating the starting point
			if (b)
			{
				if (_drawDot)
				{
					g.FillEllipse(br, (int)(x0 - dotSize / 2), (int)(y0 - dotSize / 2), dotSize, dotSize);
				}
			}
			b = true;
			if (X1 < 0)
			{
				b = false;
				if (Math.Abs(x0 - x1) > 0.0001)
				{
					Y1 = (int)(y0 - x0 * (y0 - y1) / (x0 - x1));
				}
				X1 = 0;
			}
			else if (X1 > this.Size.Width)
			{
				b = false;
				//(x0-x1)*(y-y0) = (x-x0)*(y0-y1)
				if (Math.Abs(x0 - x1) > 0.0001)
				{
					Y1 = (int)(y0 + (this.Size.Width - x0) * (y0 - y1) / (x0 - x1));
				}
				X1 = this.Size.Width;
			}
			if (Y1 < 0)
			{
				b = false;

				//(x0-x1)*(y-y0) = (x-x0)*(y0-y1)
				if (Math.Abs(y0 - y1) > 0.0001)
				{
					X1 = (int)(x0 - y0 * (x0 - x1) / (y0 - y1));
				}
				Y1 = 0;

			}
			else if (Y1 > this.Size.Height)
			{
				b = false;

				//(x0-x1)*(y-y0) = (x-x0)*(y0-y1)
				if (Math.Abs(y0 - y1) > 0.0001)
				{
					X1 = (int)(x0 + (x0 - x1) * (this.Size.Height - y0) / (y0 - y1));
				}
				Y1 = this.Size.Height;
			}
			//draw line
			try
			{
				if (_drawDot && _drawArrow)
				{
					g.DrawLine(pen, X0, Y0, X1, Y1);
				}
				else
				{
					if (_drawDot)
						g.DrawLine(pen, X0, Y0, (float)px1, (float)py1);
					else if (_drawArrow)
						g.DrawLine(pen, (float)px0, (float)py0, X1, Y1);
					else
						g.DrawLine(pen, (float)px0, (float)py0, (float)px1, (float)py1);
				}
			}
			catch
			{
			}
			//draw arrow indicating the end point
			if (b)
			{
				if (_drawArrow || ShowArrows)
				{
					g.FillPolygon(br, pts);
				}
			}
		}
		public int OffsetX
		{
			get
			{
				return nOffsetX;
			}
			set
			{
				nOffsetX = value;
				Calculate();
			}
		}
		public int OffsetY
		{
			get
			{
				return nOffsetY;
			}
			set
			{
				nOffsetY = value;
				Calculate();
			}
		}
		public virtual void Calculate()
		{
			int nCase = 0;
			double cx0 = px0 + scrollPos.X + nOffsetX;
			double cy0 = py0 + scrollPos.Y + nOffsetY;
			double cx1 = px1 + scrollPos.X + nOffsetX;
			double cy1 = py1 + scrollPos.Y + nOffsetY;
			double dx = cx0 - cx1;
			double dy = cy0 - cy1;
			double len = 0;
			double r = Math.Sqrt(dx * dx + dy * dy);
			if (r > 0.0001)
			{
				len = nObjectSize / Math.Sqrt(dx * dx + dy * dy);
			}
			x0 = cx0 - dx * len;
			y0 = cy0 - dy * len;
			x1 = cx1 + dx * len;
			y1 = cy1 + dy * len;
			if (x0 > x1)
			{
				Left = (int)x1;
				Right = (int)x0;
			}
			else
			{
				Left = (int)x0;
				Right = (int)x1;
			}
			if (y0 > y1)
			{
				Top = (int)y1;
				Bottom = (int)y0;
			}
			else
			{
				Top = (int)y0;
				Bottom = (int)y1;
			}
			if (Left != Right && Top != Bottom)
			{
				a = (y0 - y1) / (x0 - x1);
				b = y0 - a * x0;
				kc = 1.0 / (a * a + 1);
			}
			double x10 = x1 - x0;
			double y10 = y1 - y0;
			double xy10;
			double y001;
			double y002;
			double x001;
			double x002;
			double xy102;
			pts[0] = new Point((int)x1, (int)y1);
			if (Math.Abs(y10) < 0.0000001)
			{
				nCase = 1;
				xy10 = 0;
				y001 = y1;
				y002 = y1;
				x001 = x1 + arrowWidth;
				x002 = x1 - arrowWidth;
			}
			else if (Math.Abs(x10) < 0.0000001)
			{
				nCase = 2;
				x10 = 0;
				x001 = x1;
				x002 = x1;
				y001 = y1 + arrowWidth;
				y002 = y1 - arrowWidth;
			}
			else
			{
				xy10 = x10 / y10;
				xy102 = xy10 * xy10;
				y001 = y1 + Math.Sqrt(arrowWidth2 / (1 + xy102));
				y002 = y1 - Math.Sqrt(arrowWidth2 / (1 + xy102));
				x001 = x1 + (y001 - y1) * xy10;
				x002 = x1 + (y002 - y1) * xy10;
			}

			double y = y001;
			double x = x001;
			if ((x001 - x0) * (x001 - x0) + (y001 - y0) * (y001 - y0) > (x002 - x0) * (x002 - x0) + (y002 - y0) * (y002 - y0))
			{
				y = y002;
				x = x002;
			}
			//min{ (x-x0)^2 + (y-y0)^2 }
			switch (nCase)
			{
				case 0:
					double H1 = x - x1;
					double W1 = y - y1;
					double U = (arrowWidth2 - H1 * H1 - W1 * W1) / (2.0 * H1);
					double V = W1 / H1;
					double A = 1 + V * V;
					double B = -2.0 * U * V;
					double C = U * U - arrowHeight2;
					B = B / A;
					C = C / A;
					A = 1;
					double R = Math.Sqrt(B * B - 4 * A * C);
					double W = (-B + R) / 2.0;
					double H = U - V * W;
					//
					double a = x + H;
					double b = y + W;
					//
					pts[1] = new Point((int)a, (int)b);
					W = (-B - R) / 2.0;
					H = U - V * W;
					//
					a = x + H;
					b = y + W;
					//
					pts[2] = new Point((int)a, (int)b);
					break;
				case 1:
					pts[1] = new Point((int)x, (int)(y - arrowHeight));
					pts[2] = new Point((int)x, (int)(y + arrowHeight));
					break;
				case 2:
					pts[1] = new Point((int)(x - arrowHeight), (int)y);
					pts[2] = new Point((int)(x + arrowHeight), (int)y);
					break;
			}
			//
			pts[3] = pts[0];
		}
		public void SetDrawAttributes(int dSize, double arrSize, float lineWidth, System.Drawing.Color cr)
		{
			arrowSize = arrSize;
			dotSize = dSize;
			fWidth = lineWidth;
			c = cr;
			Recalculate();
		}
		public void Recalculate()
		{
			arrowHeight = arrowSize / 2;
			arrowHeight2 = arrowSize * arrowSize / 4;
			arrowWidth2 = arrowSize * arrowSize * 3 / 4;
			arrowWidth = Math.Sqrt(arrowSize * arrowSize * 3) / 2;
			//
			br = new SolidBrush(c);
			pen = new Pen(br, fWidth);
			//
			Calculate();
		}
		protected virtual void frmOwner_Move(object sender, EventArgs e)
		{
			Control frmOwner = this.Owner;
			if (frmOwner != null)
			{
				this.Size = new Size(frmOwner.ClientSize.Width, frmOwner.ClientSize.Height);
			}
		}

		protected virtual void frmOwner_Resize(object sender, EventArgs e)
		{
			Control frmOwner = this.Owner;
			if (frmOwner != null)
			{
				this.Size = new Size(frmOwner.ClientSize.Width, frmOwner.ClientSize.Height);
			}
		}
		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			if (OnDrawMouseDown != null)
			{
				OnDrawMouseDown(this, e);
			}
		}
		public virtual void OnOwnerScroll()
		{
			Form frmOwner = this.Owner as Form;
			if (frmOwner != null)
			{
				scrollPos = frmOwner.AutoScrollPosition;
				Calculate();
			}
		}

		private void src_Move(object sender, EventArgs e)
		{
			if (bShifted)
			{
				calculateShift();
			}
			else
			{
				this.SetStart(objStart.Center.X - scrollPos.X, objStart.Center.Y - scrollPos.Y);
			}
		}

		private void tgt_Move(object sender, EventArgs e)
		{
			if (bShifted)
			{
				calculateShift();
			}
			else
			{
				this.SetEnd(objEnd.Center.X - scrollPos.X, objEnd.Center.Y - scrollPos.Y);
			}
		}
		/// <summary>
		/// calculate px0,py0,px1,py1
		/// </summary>
		protected virtual void calculateShift()
		{
			double L = nObjectSize / 3.0; //
			int X0 = objStart.Center.X - scrollPos.X;
			int Y0 = objStart.Center.Y - scrollPos.Y;
			int X1 = objEnd.Center.X - scrollPos.X;
			int Y1 = objEnd.Center.Y - scrollPos.Y;
			if (Y0 == Y1)
			{
				if (X1 > X0)
				{
					Y0 += (int)L;
				}
				else
				{
					Y0 -= (int)L;
				}
				Y1 = Y0;
			}
			else if (X0 == X1)
			{
				if (Y1 > Y0)
				{
					X0 += (int)L;
				}
				else
				{
					X0 -= (int)L;
				}
				X1 = X0;
			}
			else
			{
				double dx, dy;
				double r = ((double)(X1 - X0)) / ((double)(Y1 - Y0));
				r = Math.Sqrt(1 + r * r);
				dx = L / r;
				dy = Math.Sqrt(L * L - dx * dx);
				if (Y1 < Y0)
				{
					X0 += (int)dx;
					X1 += (int)dx;
				}
				else
				{
					X0 -= (int)dx;
					X1 -= (int)dx;
				}
				if (X1 > X0)
				{
					Y0 += (int)dy;
					Y1 += (int)dy;
				}
				else
				{
					Y0 -= (int)dy;
					Y1 -= (int)dy;
				}
			}
			this.SetEnds(X0, Y0, X1, Y1);
		}
		public virtual bool HitTest(int x, int y)
		{
			if (Left == Right)
			{
				if (y >= Top && y <= Bottom)
				{
					x -= Left;
					if (x > -2 && x < 2)
					{
						return true;
					}
				}
			}
			else if (Top == Bottom)
			{
				if (x >= Left && x <= Right)
				{
					y -= Top;
					if (y > -2 && y < 2)
					{
						return true;
					}
				}
			}
			else if (x >= Left && x <= Right && y >= Top && y <= Bottom)
			{

				ht = a * x + b - y;
				ht = kc * ht * ht;
				if (ht < 5)
				{
					return true;
				}
			}
			if (_hitOnDot)
			{
				ht = (x - px0) * (x - px0) + (y - py0) * (y - py0);
				if (ht < 5)
				{
					return true;
				}
				ht = (x - px1) * (x - px1) + (y - py1) * (y - py1);
				if (ht < 5)
				{
					return true;
				}
			}
			return false;
		}
	}
}
