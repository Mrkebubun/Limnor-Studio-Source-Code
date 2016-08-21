/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Drawing.Drawing2D;
using VPL;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawTextRect))]
	[ToolboxBitmapAttribute(typeof(DrawTextRect), "Resources.rectrtxt.bmp")]
	[Description("This object represents a Rectangle with rounded corners and a text in the center.")]
	public class Draw2DTextRect : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DTextRect()
		{
		}
		static Draw2DTextRect()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("CornerRadius");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("LineWidth");
			propertyNames.Add("RotateAngle");
			propertyNames.Add("TextString");
			propertyNames.Add("TextFont");
			propertyNames.Add("TextColor");
			propertyNames.Add("WordWrap");
			propertyNames.Add("HideRectangle");
			propertyNames.Add("TextAlign");

			propertyNames.Add("EnableEditing");
			propertyNames.Add("TextBoxWidth");
			propertyNames.Add("TabIndex");

		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (TextRect != null)
			{
				TextRect.TextBoxWidth = this.Width;
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		private DrawTextRect TextRect
		{
			get
			{
				return (DrawTextRect)Item;
			}
		}
		[Description("Text alignment")]
		public ContentAlignment TextAlign
		{
			get
			{
				if (TextRect != null)
					return TextRect.TextAlign;
				return ContentAlignment.MiddleCenter;
			}
			set
			{
				if (TextRect != null)
					TextRect.TextAlign = value;
			}
		}
		[Description("Indicates if the rectangle is hidden")]
		public bool HideRectangle
		{
			get
			{
				if (TextRect != null)
					return TextRect.HideRectangle;
				return false;
			}
			set
			{
				if (TextRect != null)
					TextRect.HideRectangle = value;
			}
		}
		[Description("Indicates if lines are automatically word-wrapped for multiline text")]
		public bool WordWrap
		{
			get
			{
				if (TextRect != null)
					return TextRect.WordWrap;
				return false;
			}
			set
			{
				if (TextRect != null)
					TextRect.WordWrap = value;
			}
		}
		[Description("The rectangle defining the object")]
		public Rectangle Rectangle
		{
			get
			{
				if (TextRect != null)
					return TextRect.Rectangle;
				return this.Bounds;
			}
			set
			{
				if (TextRect != null)
					TextRect.Rectangle = value;
			}
		}
		[Description("The radius for each corner")]
		public float CornerRadius
		{
			get
			{
				if (TextRect != null)
					return TextRect.CornerRadius;
				return 20F;
			}
			set
			{
				if (TextRect != null)
					TextRect.CornerRadius = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				if (TextRect != null)
					return TextRect.RotateAngle;
				return 0.0;
			}
			set
			{
				if (TextRect != null)
					TextRect.RotateAngle = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				if (TextRect != null)
					return TextRect.Fill;
				return false;
			}
			set
			{
				if (TextRect != null)
					TextRect.Fill = value;
			}
		}
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				if (TextRect != null)
					return TextRect.FillColor;
				return this.ForeColor;
			}
			set
			{
				if (TextRect != null)
					TextRect.FillColor = value;
			}
		}
		[Description("Line width")]
		public float LineWidth
		{
			get
			{
				if (TextRect != null)
					return TextRect.LineWidth;
				return 1;
			}
			set
			{
				if (TextRect != null)
					TextRect.LineWidth = value;
			}
		}
		[Bindable(true)]
		[Description("The text to be displayed")]
		public string TextString
		{
			get
			{
				if (TextRect != null)
					return TextRect.TextString;
				return string.Empty;
			}
			set
			{
				if (TextRect != null)
					TextRect.TextString = value;
			}
		}
		[Description("Font of the text")]
		public Font TextFont
		{
			get
			{
				if (TextRect != null)
					return TextRect.TextFont;
				return this.Font;
			}
			set
			{
				if (TextRect != null)
					TextRect.TextFont = value;
			}
		}
		[Description("Color of the text")]
		public Color TextColor
		{
			get
			{
				if (TextRect != null)
					return TextRect.TextColor;
				return this.ForeColor;
			}
			set
			{
				if (TextRect != null)
					TextRect.TextColor = value;
			}
		}
		[Description("Gets and sets an integer indicating the order of data entry")]
		public new int TabIndex
		{
			get
			{
				if (TextRect != null)
					return TextRect.TabIndex;
				return 0;
			}
			set
			{
				if (TextRect != null)
					TextRect.TabIndex = value;
			}
		}
		[DefaultValue(120)]
		[Description("Gets and sets an integer indicating width of a text box for data entry")]
		public int TextBoxWidth
		{
			get
			{
				if (TextRect != null)
					return TextRect.TextBoxWidth;
				return 120;
			}
			set
			{
				if (TextRect != null && value > 0)
				{
					TextRect.TextBoxWidth = value;
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
				if (TextRect != null)
					return TextRect.EnableEditing;
				return false;
			}
			set
			{
				if (TextRect != null)
					TextRect.EnableEditing = value;
			}
		}
	}
	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DTextRect))]
	[ToolboxBitmapAttribute(typeof(DrawTextRect), "Resources.rectrtxt.bmp")]
	[Description("This object represents a Rectangle with rounded corners and a text in the center.")]
	public class DrawTextRect : DrawRoundRectangle,ITextInput
	{
		#region fields and constructors
		private string text = "Text";
		private Font font = new Font("Times new roman", 8);
		private Color _txtcolor = Color.Black;
		private Rectangle _textRect = Rectangle.Empty;
		private ContentAlignment _allign = ContentAlignment.MiddleCenter;
		public DrawTextRect()
		{
		}
		#endregion
		#region Methods
		public string GetTextPropertyName()
		{
			return "TextString";
		}
		public override void AddToPath(GraphicsPath path)
		{
			base.AddToPath(path);
			if (!string.IsNullOrEmpty(text))
			{
				if (_textRect != Rectangle.Empty)
				{
					path.FillMode = FillMode.Winding;
					path.AddRectangle(_textRect);
				}
			}
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			if (base.HitTest(owner, x, y))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (_textRect != Rectangle.Empty)
				{
					return DrawRect.HitTestRectangle(owner, x, y, _textRect, RotateAngle, true, 1);
				}
			}
			return DrawRect.HitTestRectangle(owner, x, y, this.Rectangle, RotateAngle, true, 1); ;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawTextRect r = obj as DrawTextRect;
			if (r != null)
			{
				font = r.font;
				TextColor = r.TextColor;
				text = r.text;
				HideRectangle = r.HideRectangle;
				WordWrap = r.WordWrap;
				_textRect = r._textRect;
				TextAlign = r.TextAlign;
				this.EnableEditing = r.EnableEditing;
				this.TabIndex = r.TabIndex;
			}
		}
		public override void OnDraw(Graphics g)
		{
			if (string.IsNullOrEmpty(text))
			{
				base.OnDraw(g);
			}
			else
			{
				_textRect = new Rectangle();
				SizeF szText = g.MeasureString(text, font);
				float maxLineHeight = szText.Height;
				//estimate the letter size
				SizeF szChar = g.MeasureString("W", font);
				int lineSpace = 2; //2 pixels seperating lines
				int maxLines = (int)(Height / (szChar.Height + lineSpace));
				float widthTxt = szText.Width;
				List<string> ss = new List<string>();
				if (szText.Width <= Width)
				{
					ss.Add(text);
				}
				else
				{
					widthTxt = Width;
					if (WordWrap)
					{
						string s = text;
						float h;
						while (s.Length > 0 && ss.Count < maxLines)
						{
							string s0 = getLine(ref s, out h, g, szChar.Width, true);
							ss.Add(s0);
							if (h > maxLineHeight)
							{
								maxLineHeight = h;
							}
						}
					}
					else
					{
						string s = text;
						float h;
						string s0 = getLine(ref s, out h, g, szChar.Width, false);
						ss.Add(s0);
						if (h > maxLineHeight)
						{
							maxLineHeight = h;
						}
					}
				}

				float dy = maxLineHeight + lineSpace;
				float heightTxt = (float)(ss.Count) * dy;
				//
				float x;
				float y;
				Brush _textBrush = new SolidBrush(TextColor);
				switch (_allign)
				{
					case ContentAlignment.BottomCenter:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						else
							x = x / 2F;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						//else
						//    y = Rectangle.Y + y;
						break;
					case ContentAlignment.BottomLeft:
						x = 0;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						//else
						//    y = Rectangle.Y + y;
						break;
					case ContentAlignment.BottomRight:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						//else
						//    x = Rectangle.X + x;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						break;
					case ContentAlignment.MiddleCenter:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						else
							x = x / 2F;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						else
							y = y / 2F;
						break;
					case ContentAlignment.MiddleLeft:
						x = 0;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						else
							y = y / 2F;
						break;
					case ContentAlignment.MiddleRight:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						y = Rectangle.Height - heightTxt;
						if (y < 0)
							y = 0;
						else
							y = y / 2F;
						break;
					case ContentAlignment.TopCenter:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						else
							x = x / 2F;
						y = 0;
						break;
					case ContentAlignment.TopLeft:
						x = 0;
						y = 0;
						break;
					case ContentAlignment.TopRight:
						x = Rectangle.Width - widthTxt;
						if (x < 0)
							x = 0;
						y = 0;
						break;
					default:
						x = 0;
						y = 0;
						break;
				}

				_textRect.Width = (int)widthTxt;
				if (RotateAngle == 0)
				{
					GraphicsPath path = getGraphicsPath(Rectangle, this.CornerRadius);
					x += Rectangle.X;
					y += Rectangle.Y;
					_textRect.Y = (int)y;
					_textRect.X = (int)x;
					if (Fill)
					{
						g.FillPath(new SolidBrush(FillColor), path);
					}
					if (!HideRectangle)
					{
						g.DrawPath(new Pen(this.Color, this.LineWidth), path);
					}
					for (int i = 0; i < ss.Count; i++)
					{
						g.DrawString(ss[i], font, _textBrush, x, y);
						y += dy;
					}
					_textRect.Height = (int)(y - (float)_textRect.Y);
				}
				else
				{
					_textRect.Y = (int)y;
					_textRect.X = (int)x;
					GraphicsState transState = g.Save();
					double angle = (RotateAngle / 180) * Math.PI;
					Rectangle rc = this.Rectangle;
					//
					g.TranslateTransform(
						(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
						(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
					g.RotateTransform((float)RotateAngle);
					Rectangle rc0 = new Rectangle(0, 0, rc.Width, rc.Height);
					GraphicsPath path = getGraphicsPath(rc0, this.CornerRadius);
					if (Fill)
					{
						g.FillPath(new SolidBrush(FillColor), path);
					}
					if (!HideRectangle)
					{
						g.DrawPath(new Pen(this.Color, this.LineWidth), path);
					}
					for (int i = 0; i < ss.Count; i++)
					{
						g.DrawString(ss[i], font, _textBrush, x, y);
						y += dy;
					}
					_textRect.Height = (int)(y - (float)_textRect.Y);
					g.Restore(transState);
				}
			}
		}
		/// <summary>
		/// cut off s to fit a line
		/// </summary>
		/// <param name="s"></param>
		/// <param name="charWidth">width for W</param>
		/// <param name="keepWord">do not break the last word</param>
		/// <returns></returns>
		private string getLine(ref string s, out float hTxt, Graphics g, float charWidth, bool keepWord)
		{
			string s1 = "";
			SizeF size = g.MeasureString(s, font); ;
			if (size.Width <= Width)
			{
				s1 = s;
				s = "";
				hTxt = size.Height;
				return s1;
			}
			int n = (int)(Width / charWidth); //estimated length
			if (n < 2)
			{
				if (s.Length > 0)
				{
					s1 = s.Substring(0, 1);
				}
				size = g.MeasureString(s1, font);
				hTxt = size.Height;
				s = "";
			}
			else
			{
				if (n >= s.Length)
				{
					s1 = s;
					size = g.MeasureString(s1, font);
					hTxt = size.Height;
					s = "";
				}
				else
				{
					int dn = n / 2; //starting delta
					bool bForward = true;
					hTxt = 10;
					while (dn > 0)
					{
						s1 = s.Substring(0, n);
						size = g.MeasureString(s1, font);
						if (size.Width > Width)
						{
							if (bForward)
							{
								bForward = false;
								dn /= 2;
							}
							n -= dn;
						}
						else
						{
							if (!bForward)
							{
								bForward = true;
								dn /= 2;
							}
							while (dn > 0 && n + dn > s.Length)
							{
								dn /= 2;
							}
							n += dn;
						}
						hTxt = size.Height;
					}
					if (keepWord)
					{
						if (n < s.Length)
						{
							int n2 = s1.LastIndexOf(' ');
							if (n2 > 0)
							{
								n = n2 + 1;
								s1 = s1.Substring(0, n);
							}
						}
					}
					s = s.Substring(n);
				}
			}
			return s1;
		}
		#endregion
		#region Properties
		//
		[Description("Text alignment")]
		public ContentAlignment TextAlign
		{
			get
			{
				return _allign;
			}
			set
			{
				_allign = value;
			}
		}
		[Description("Indicates if the rectangle is hidden")]
		public bool HideRectangle
		{
			get;
			set;
		}
		[Description("Indicates if lines are automatically word-wrapped for multiline text")]
		public bool WordWrap
		{
			get;
			set;
		}
		[Bindable(true)]
		[Description("The text to be displayed")]
		public string TextString
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
		[XmlIgnore]
		[Description("Color of the text")]
		public Color TextColor
		{
			get
			{
				if (_txtcolor == Color.Empty)
					_txtcolor = Color.Black;
				return _txtcolor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_txtcolor = value;
					OnRefresh();
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string TColorString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				return converter.ConvertToInvariantString(_txtcolor);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_txtcolor = System.Drawing.Color.Black;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
					_txtcolor = (Color)converter.ConvertFromInvariantString(value);
				}
			}
		}
		#endregion

		#region ITextInput
		[Browsable(false)]
		[NotForProgramming]
		public bool MultiLine { get { return true; } }
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
		[Browsable(false)]
		[NotForProgramming]
		[ReadOnly(true)]
		[XmlIgnore]
		public int TextBoxWidth
		{
			get
			{
				return this.Rectangle.Width;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public int TextBoxHeight
		{
			get
			{
				return this.Rectangle.Height;
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
			get;
			set;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetText()
		{
			return this.TextString;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetText(string txt)
		{
			this.TextString = txt;
		}
		#endregion
	}
}
