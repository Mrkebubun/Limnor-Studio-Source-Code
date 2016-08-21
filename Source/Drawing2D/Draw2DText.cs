/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawText))]
	[ToolboxBitmapAttribute(typeof(DrawText), "Resources.text.bmp")]
	[Description("This object represents a Text.")]
	public class Draw2DText : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DText()
		{
		}
		static Draw2DText()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("TextContent");
			propertyNames.Add("Location");
			propertyNames.Add("TextFont");
			propertyNames.Add("TextColor");
			propertyNames.Add("TextAngle");
			propertyNames.Add("TextPosition");
			propertyNames.Add("TextSize");

			propertyNames.Add("EnableEditing");
			propertyNames.Add("TextBoxWidth");
			propertyNames.Add("TabIndex");
		}
		protected override void OnResize(EventArgs e)
		{
			if (!this.IsAdjustingSizeLocation)
			{
				base.OnResize(e);
				if (Text0 != null)
				{
					if (this.Width > Text0.TextBoxWidth)
					{
						Text0.TextBoxWidth = this.Width;
					}
				}
			}
		}
		protected override void OnItemAssigned()
		{
			DrawText dt = Text0;
			if (dt != null)
			{
				if (this.Width < dt.TextBoxWidth)
				{
					this.Width = dt.TextBoxWidth;
				}
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		public override bool IsPropertyReadOnly(string propertyName)
		{
			return false;
		}
		public override bool ExcludeFromInitialize(string propertyName)
		{
			return false;
		}
		private DrawText Text0
		{
			get
			{
				return (DrawText)Item;
			}
		}
		#region Properties
		[Bindable(true)]
		[Description("The text to be displayed")]
		public string TextContent
		{
			get
			{
				if (Text0 != null)
				{
					return Text0.TextContent;
				}
				return string.Empty;
			}
			set
			{
				DrawText dt = Text0;
				if (dt != null)
				{
					dt.TextContent = value;
					if (this.Width < dt.TextBoxWidth)
					{
						this.Width = dt.TextBoxWidth;
					}
				}
			}
		}
		[Description("Font of the text")]
		public Font TextFont
		{
			get
			{
				if (Text0 != null)
				{
					return Text0.TextFont;
				}
				return this.Font;
			}
			set
			{
				if (Text0 != null)
				{
					Text0.TextFont = value;
				}
			}
		}
		[Description("The angle to rotate the text")]
		public double TextAngle
		{
			get
			{
				if (Text0 != null)
				{
					return Text0.TextAngle;
				}
				return 0;
			}
			set
			{
				if (Text0 != null)
				{
					Text0.TextAngle = value;
				}
			}
		}
		[Description("The width and height of the text")]
		public Size TextSize
		{
			get
			{
				if (Text0 != null)
				{
					return Text0.TextSize;
				}
				return this.Size;
			}
		}

		[Description("Gets and sets an integer indicating the order of data entry")]
		public new int TabIndex
		{
			get
			{
				if (Text0 != null)
					return Text0.TabIndex;
				return 0;
			}
			set
			{
				if (Text0 != null)
					Text0.TabIndex = value;
			}
		}
		[DefaultValue(120)]
		[Description("Gets and sets an integer indicating width of a text box for data entry")]
		public int TextBoxWidth
		{
			get
			{
				if (Text0 != null)
					return Text0.TextBoxWidth;
				return 120;
			}
			set
			{
				if (Text0 != null && value > 0)
				{
					Text0.TextBoxWidth = value;
					IsAdjustingSizeLocation = true;
					this.Width = value;
					IsAdjustingSizeLocation = false;
				}
			}
		}
		[Description("Gets and sets a Boolean indicating whether data entry is enabled.")]
		public bool EnableEditing
		{
			get
			{
				if (Text0 != null)
					return Text0.EnableEditing;
				return false;
			}
			set
			{
				if (Text0 != null)
					Text0.EnableEditing = value;
			}
		}
		#endregion
	}
	/// <summary>
	/// </summary>
	[TypeMapping(typeof(Draw2DText))]
	[ToolboxBitmapAttribute(typeof(DrawText), "Resources.text.bmp")]
	[Description("This object represents a Text.")]
	public class DrawText : DrawingItem, ITextInput
	{
		#region fields and constructor
		private string text = "Text";
		private Point pos = new Point(30, 30);
		private Font font = new Font("Times new roman", 8);
		private double Angle = 0;
		private Size _size;
		public DrawText()
		{
			_size = new Size(30, 30);
			EnableEditing = false;
			TabIndex = 0;
		}
		#endregion
		#region Properties
		//
		[Bindable(true)]
		[Description("The text to be displayed")]
		public string TextContent
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				OnRefresh();
			}
		}
		[Description("The location of the text")]
		public override Point Location
		{
			get
			{
				return pos;
			}
			set
			{
				pos = value;
				OnRefresh();
			}
		}
		[XmlIgnore]
		[Description("Font of the text")]
		public Font TextFont
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				OnRefresh();
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string TextFontString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
				return converter.ConvertToInvariantString(font);
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
					font = (Font)converter.ConvertFromInvariantString(value);
				}
			}
		}
		[Description("The angle to rotate the text")]
		public double TextAngle
		{
			get
			{
				return Angle;
			}
			set
			{
				Angle = value;
				OnRefresh();
			}
		}
		[Description("The width and height of the text")]
		public Size TextSize
		{
			get
			{
				return _size;
			}
		}
		public override int Left
		{
			get
			{
				return pos.X;
			}
			set
			{
				pos.X = value;
				Refresh();
			}
		}
		public override int Top
		{
			get
			{
				return pos.Y;
			}
			set
			{
				pos.Y = value;
				Refresh();
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				return new Point(pos.X + _size.Width / 2, pos.Y + _size.Height / 2);
			}
			set
			{
				pos.X = value.X - _size.Width / 2;
				pos.Y = value.Y - _size.Height / 2;
				Refresh();
			}
		}
		#endregion
		#region Design
		public override void MoveByStep(int dx, int dy)
		{
			pos.X += dx;
			pos.Y += dy;
			Refresh();
		}
		public override void MoveTo(Point p)
		{
			pos = p;
			Refresh();
		}
		public override void MoveCenterTo(Point p)
		{
			Point c = Center;
			int dx = p.X - c.X;
			int dy = p.Y - c.Y;
			MoveByStep(dx, dy);
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = owner as Form;
			if (page == null)
			{
				return false;
			}
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			//
			Graphics g = owner.CreateGraphics();
			SizeF sf;
			if (string.IsNullOrEmpty(text))
			{
				sf = g.MeasureString("H", font, new SizeF(g.ClipBounds.Width, g.ClipBounds.Height));
				sf.Width = 1;
			}
			else
			{
				sf = g.MeasureString(text, font, new SizeF(g.ClipBounds.Width, g.ClipBounds.Height));
			}
			_size.Width = (int)sf.Width;
			_size.Height = (int)sf.Height;
			g.Dispose();
			//
			int w = this.EnableEditing ? (this.TextBoxWidth > _size.Width ? this.TextBoxWidth : _size.Width) : _size.Width;
			//
			if (Angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, pos.X + w / 2, pos.Y + _size.Height / 2, Angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			//
			if (x >= pos.X && x <= pos.X + w && y >= pos.Y && y < pos.Y + _size.Height)
				return true;
			return false;
		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			designer = new DrawingDesign();
			designer.Marks = new DrawingMark[2];
			//
			designer.Marks[0] = new DrawingMover();
			designer.Marks[0].Index = 0;
			designer.Marks[0].Owner = this;
			Page.Controls.Add(designer.Marks[0]);
			//
			designer.Marks[1] = new DrawingMark();
			designer.Marks[1].Index = 1;
			designer.Marks[1].BackColor = System.Drawing.Color.Red;
			designer.Marks[1].Owner = this;
			designer.Marks[1].Cursor = System.Windows.Forms.Cursors.Cross;
			Page.Controls.Add(designer.Marks[1]);
			//
			SetMarks();
			//
			return designer;
		}
		public override void SetMarks()
		{
			Point ap = this.GetAbsolutePosition();
			double angle = (Angle / 180) * Math.PI;
			designer.Marks[0].X = ap.X + _size.Width / 2 + Page.AutoScrollPosition.X;
			designer.Marks[0].Y = ap.Y + _size.Height / 2 + Page.AutoScrollPosition.Y;
			designer.Marks[1].X = designer.Marks[0].X + (int)((_size.Width / 2.0) * Math.Cos(angle));
			designer.Marks[1].Y = designer.Marks[0].Y + (int)((_size.Width / 2.0) * Math.Sin(angle));
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			if (sender.Parent == null)
				return;
			Point ap = this.GetAbsolutePosition();
			int dx = ap.X-pos.X;
			int dy = ap.Y-pos.Y;
			switch (sender.Index)
			{
				case 0:
					double xo = designer.Marks[0].X - _size.Width / 2;
					double yo = designer.Marks[0].Y - _size.Height / 2;
					pos.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0) - dx;
					pos.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0) - dy;
					break;
				case 1:
					int x0 = designer.Marks[1].X - designer.Marks[0].X;
					int y0 = designer.Marks[1].Y - designer.Marks[0].Y;
					if (x0 != 0 || y0 != 0)
					{
						if (x0 >= 0 && y0 >= 0)
						{
							if (y0 == 0)
								Angle = 0;
							else if (x0 == 0)
								Angle = 90;
							else
							{
								Angle = Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
							}
						}
						else if (x0 < 0 && y0 >= 0)
						{
							if (y0 == 0)
								Angle = 180;
							else
							{
								Angle = 180 - Math.Atan((double)y0 / (-(double)x0)) * 180.0 / Math.PI;
							}
						}
						else if (x0 >= 0 && y0 < 0)
						{
							y0 = -y0;
							Angle = -Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
						}
						else
						{
							Angle = 180 + Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
						}
					}
					break;
			}
			if (sender.Index != 1)
			{
				double angle = (Angle / 180) * Math.PI;
				designer.Marks[1].X = designer.Marks[0].X + (int)((_size.Width / 2.0) * Math.Cos(angle));
				designer.Marks[1].Y = designer.Marks[0].Y + (int)((_size.Width / 2.0) * Math.Sin(angle));
			}
			sender.Parent.Invalidate(Bounds);
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditText dlgText = new dlgEditText();
			dlgText.LoadData(this, owner);
			ret = dlgText.ShowDialog(owner);
			return ret;
		}
		#endregion
		#region Methods
		public override string Help()
		{
			return "  Left-click to specify Text location";
		}
		public override System.Drawing.Rectangle Bounds
		{
			get
			{
				return DrawingItem.GetBounds(new Rectangle(pos, _size), Angle, Page);
			}
			set
			{
				SizeAcceptedBySetBounds = false;
				if (value.Width > 0)
				{
					_size.Width = value.Width;
				}
				if (value.Height > 0)
				{
					_size.Height = value.Height;
				}
			}
		}

		public override bool SetPage(int n)
		{
			return false;
		}
		public override void OnDraw(Graphics g)
		{
			bool bDB = false;
			string textToDisplay = "";
			if (!bDB)
			{
				textToDisplay = text;
			}
			SizeF sf = g.MeasureString(textToDisplay, font);
			_size.Height = (int)sf.Height;
			_size.Width = (int)sf.Width;
			double angle = (Angle / 180) * Math.PI;
			GraphicsState transState = g.Save();
			g.TranslateTransform(
				(sf.Width + (float)(sf.Height * Math.Sin(angle)) - (float)(sf.Width * Math.Cos(angle))) / 2 + pos.X,
				(sf.Height - (float)(sf.Height * Math.Cos(angle)) - (float)(sf.Width * Math.Sin(angle))) / 2 + pos.Y);
			g.RotateTransform((float)Angle);
			g.DrawString(textToDisplay, font, new SolidBrush(Color), 0, 0);
			g.Restore(transState);
		}
		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path)
		{
			path.AddString(text, font.FontFamily, (int)font.Style, font.Size, pos, System.Drawing.StringFormat.GenericDefault);
		}
		public override void ConvertOrigin(System.Drawing.Point pt)
		{
			pos.X -= pt.X;
			pos.Y -= pt.Y;
		}
		public override void ConvertToScreen(System.Drawing.Point pt)
		{
			pos.X += pt.X;
			pos.Y += pt.Y;
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			pos.X = e.X;
			pos.Y = e.Y;
			dlgEditText dlgText = new dlgEditText();
			dlgText.LoadData(this, owner);
			ret = dlgText.ShowDialog(owner);
			return ret;
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep == 0)
			{
				pos.X = e.X;
				pos.Y = e.Y;
				bRet = true;
			}
			return bRet;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawText v = obj as DrawText;
			if (v != null)
			{
				this.text = v.text;
				this.font = v.font;
				this.pos = v.pos;
				this.Angle = v.Angle;
				this._size = v._size;
				this.EnableEditing = v.EnableEditing;
				this.TabIndex = v.TabIndex;
			}

		}
		public override string ToString()
		{
			return Name + "Text:[" + text + "]";
		}

		public override void Randomize(Rectangle bounds)
		{
			int n = 0;
			Random r = new Random(Guid.NewGuid().GetHashCode());
			while (n < 3)
			{
				int x0 = r.Next(bounds.Left, bounds.Right);
				int y0 = r.Next(bounds.Top, bounds.Bottom);
				int m = bounds.Right - x0;
				if (m > 0)
				{
					pos = new Point(x0, y0);
					Angle = r.NextDouble() * 360.0;
					break;
				}
				n++;
			}
		}
		#endregion
		#region ITextInput
		public string GetTextPropertyName()
		{
			return "TextContent";
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool MultiLine { get { return false; } }
		private bool _enableEdit = false;
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether data entry is enabled.")]
		public bool EnableEditing
		{
			get
			{
				return _enableEdit;
			}
			set
			{
				_enableEdit = value;
				SizeAcceptedBySetBounds = _enableEdit;
			}
		}
		private int _txtboxwidth=120;
		[DefaultValue(120)]
		[Description("Gets and sets an integer indicating width of a text box for data entry")]
		public int TextBoxWidth
		{
			get
			{
				if (_txtboxwidth <= 0)
				{
					if (this.TextSize.Width <= 0)
						return 120;
					return this.TextSize.Width;
				}
				return _txtboxwidth;
			}
			set
			{
				if (value > 0)
					_txtboxwidth = value;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public int TextBoxHeight
		{
			get
			{
				return _size.Height;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Font TextBoxFont
		{
			get { return this.TextFont; }
		}
		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating the order of data entry")]
		public int TabIndex
		{
			get;set;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetText()
		{
			return this.TextContent;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetText(string txt)
		{
			this.TextContent = txt;
		}
		#endregion
	}

}
