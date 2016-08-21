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
using System.Drawing.Drawing2D;
using System.Collections.Specialized;
using VPL;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawRectText))]
	[ToolboxBitmapAttribute(typeof(DrawRectText), "Resources.rectrtxt2.bmp")]
	[Description("This object represents a Rectangle with rounded corners and a text on top.")]
	public class Draw2DRectText : DrawingControl
	{
		#region fields and constructors
		static StringCollection propertyNames;
		public Draw2DRectText()
		{
		}
		static Draw2DRectText()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("CornerRadius");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("RotateAngle");
			propertyNames.Add("TextString");
			propertyNames.Add("TextFont");
			propertyNames.Add("TextBoxSize");
			propertyNames.Add("TextBoxCornerRadius");
			propertyNames.Add("TextBackColor");
			propertyNames.Add("UseFormBackgroundColor");
			//
			propertyNames.Add("EnableEditing");
			propertyNames.Add("TabIndex");
		}
		#endregion
		#region Methods
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		#endregion
		#region Properties
		private DrawRectText RectText
		{
			get
			{
				return (DrawRectText)Item;
			}
		}
		[Description("The rectangle size for displaying the text")]
		public Size TextBoxSize
		{
			get
			{
				if (RectText != null)
					return RectText.TextBoxSize;
				return new Size(20, 30);
			}
			set
			{
				if (RectText != null)
					RectText.TextBoxSize = value;
			}
		}
		[Description("The radius for each corner of the text box")]
		public float TextBoxCornerRadius
		{
			get
			{
				if (RectText != null)
					return RectText.TextBoxCornerRadius;
				return 10F;
			}
			set
			{
				if (RectText != null)
					RectText.TextBoxCornerRadius = value;
			}
		}
		[Description("It indicates the Background color of the text when the UseFormBackgroundColor property is false")]
		public Color TextBackColor
		{
			get
			{
				if (RectText != null)
					return RectText.TextBackColor;
				return this.BackColor;
			}
			set
			{
				if (RectText != null)
					RectText.TextBackColor = value;
			}
		}
		[Description("If this property is true then the TextBackColor property is ignored and the form's background color is used as the text background color")]
		public bool UseFormBackgroundColor
		{
			get
			{
				if (RectText != null)
					return RectText.UseFormBackgroundColor;
				return false;
			}
			set
			{
				if (RectText != null)
					RectText.UseFormBackgroundColor = value;
			}
		}
		[Description("The rectangle defining the object")]
		public Rectangle Rectangle
		{
			get
			{
				if (RectText != null)
					return RectText.Rectangle;
				return this.Bounds;
			}
			set
			{
				if (RectText != null)
					RectText.Rectangle = value;
			}
		}
		[Description("The radius for each corner")]
		public float CornerRadius
		{
			get
			{
				if (RectText != null)
					return RectText.CornerRadius;
				return 20F;
			}
			set
			{
				if (RectText != null)
					RectText.CornerRadius = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				if (RectText != null)
					return RectText.RotateAngle;
				return 0.0;
			}
			set
			{
				if (RectText != null)
					RectText.RotateAngle = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				if (RectText != null)
					return RectText.Fill;
				return false;
			}
			set
			{
				if (RectText != null)
					RectText.Fill = value;
			}
		}
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				if (RectText != null)
					return RectText.FillColor;
				return this.ForeColor;
			}
			set
			{
				if (RectText != null)
					RectText.FillColor = value;
			}
		}
		[Description("Line width")]
		public float LineWidth
		{
			get
			{
				if (RectText != null)
					return RectText.LineWidth;
				return 1;
			}
			set
			{
				if (RectText != null)
					RectText.LineWidth = value;
			}
		}
		[Bindable(true)]
		[Description("The text to be displayed")]
		public string TextString
		{
			get
			{
				if (RectText != null)
					return RectText.TextString;
				return string.Empty;
			}
			set
			{
				if (RectText != null)
					RectText.TextString = value;
			}
		}
		[Description("Font of the text")]
		public Font TextFont
		{
			get
			{
				if (RectText != null)
					return RectText.TextFont;
				return this.Font;
			}
			set
			{
				if (RectText != null)
					RectText.TextFont = value;
			}
		}
		[Description("Gets and sets an integer indicating the order of data entry")]
		public new int TabIndex
		{
			get
			{
				if (RectText != null)
					return RectText.TabIndex;
				return 0;
			}
			set
			{
				if (RectText != null)
					RectText.TabIndex = value;
			}
		}
		[Description("Gets and sets a Boolean indicating whether data entry is enabled.")]
		public bool EnableEditing
		{
			get
			{
				if (RectText != null)
					return RectText.EnableEditing;
				return false;
			}
			set
			{
				if (RectText != null)
					RectText.EnableEditing = value;
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DRectText))]
	[ToolboxBitmapAttribute(typeof(DrawRectText), "Resources.rectrtxt2.bmp")]
	[Description("This object represents a Rectangle with rounded corners and a text on top.")]
	public class DrawRectText : DrawRoundRectangle, ITextInput
	{
		#region fields and constructors
		private string text = "Text";
		private Font font = new Font("Times new roman", 8);
		private Color _bkColor = Color.Wheat;
		private Size _size = new Size(20, 60);
		private float _cornerRadius = 10F;
		private bool _useFormBackGroundColor;
		public DrawRectText()
		{
		}
		#endregion
		#region Methods
		public string GetTextPropertyName()
		{
			return "TextString";
		}
		public override void OnDraw(Graphics g)
		{
			SizeF szText = g.MeasureString(text, font);
			Brush _textBrush = new SolidBrush(Color);
			SizeF szText2 = new SizeF(TextBoxSize.Width, TextBoxSize.Height);
			if (szText2.Height < szText.Height)
			{
				szText2.Height = szText.Height;
			}
			if (szText2.Height > Rectangle.Height)
			{
				szText2.Height = Rectangle.Height;
			}
			//create big rectangle from half of text height
			float topx = szText2.Height / 2F;
			float heightx = Rectangle.Height - topx; //it will be greater than or equal to half the text box height
			float txtLeft = Rectangle.Width - szText2.Width;
			if (txtLeft < 0)
				txtLeft = 0;
			else
				txtLeft = txtLeft / 2F;
			if (RotateAngle == 0)
			{
				Rectangle rc = new Rectangle(Rectangle.X, Rectangle.Top + (int)topx, Rectangle.Width, (int)heightx);
				GraphicsPath path = getGraphicsPath(rc, this.CornerRadius);
				if (Fill)
				{
					g.FillPath(new SolidBrush(FillColor), path);
				}
				else
				{
					g.DrawPath(new Pen(this.Color, this.LineWidth), path);
				}
				Rectangle rcTxtBox = new Rectangle(Rectangle.X + (int)txtLeft, Rectangle.Top, (int)szText2.Width, (int)szText2.Height);
				path = getGraphicsPath(rcTxtBox, _cornerRadius);
				if (_useFormBackGroundColor)
				{
					if (Page != null)
					{
						g.FillPath(new SolidBrush(Page.BackColor), path);
					}
					else
					{
						g.FillPath(new SolidBrush(this.TextBackColor), path);
					}
				}
				else
				{
					g.FillPath(new SolidBrush(this.TextBackColor), path);
				}
				float x = rcTxtBox.Width - szText.Width;
				if (x < 0)
					x = rcTxtBox.X;
				else
					x = rcTxtBox.X + x / 2F;
				float y = rcTxtBox.Height - szText.Height;
				if (y < 0)
					y = rcTxtBox.Y;
				else
					y = rcTxtBox.Y + y / 2F;
				g.DrawString(text, font, _textBrush, x, y);
			}
			else
			{
				GraphicsState transState = g.Save();
				double angle = (RotateAngle / 180) * Math.PI;
				Rectangle rc = this.Rectangle;
				//
				g.TranslateTransform(
					(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
					(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
				g.RotateTransform((float)RotateAngle);

				Rectangle rcx = new Rectangle(0, (int)topx, rc.Width, (int)heightx);
				GraphicsPath path = getGraphicsPath(rcx, this.CornerRadius);
				if (Fill)
				{
					g.FillPath(new SolidBrush(FillColor), path);
				}
				else
				{
					g.DrawPath(new Pen(this.Color, this.LineWidth), path);
				}
				Rectangle rcTxtBox = new Rectangle((int)txtLeft, 0, (int)szText2.Width, (int)szText2.Height);
				path = getGraphicsPath(rcTxtBox, _cornerRadius);
				if (_useFormBackGroundColor)
				{
					if (Page != null)
					{
						g.FillPath(new SolidBrush(Page.BackColor), path);
					}
					else
					{
						g.FillPath(new SolidBrush(this.TextBackColor), path);
					}
				}
				else
				{
					g.FillPath(new SolidBrush(this.TextBackColor), path);
				}
				float x = rcTxtBox.Width - szText.Width;
				if (x < 0)
					x = rcTxtBox.X;
				else
					x = rcTxtBox.X + x / 2F;
				float y = rcTxtBox.Height - szText.Height;
				if (y < 0)
					y = rcTxtBox.Y;
				else
					y = rcTxtBox.Y + y / 2F;
				g.DrawString(text, font, _textBrush, x, y);
				g.Restore(transState);
			}
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			return DrawRect.HitTestRectangle(owner, x, y, this.Rectangle, RotateAngle, true, 1);
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawRectText r = obj as DrawRectText;
			if (r != null)
			{
				font = r.font;
				Color = r.Color;
				_bkColor = r._bkColor;
				_size = r._size;
				_cornerRadius = r._cornerRadius;
				_useFormBackGroundColor = r._useFormBackGroundColor;
				this.EnableEditing = r.EnableEditing;
				this.TabIndex = r.TabIndex;
			}
		}
		#endregion
		#region IProperties Members
		//
		[Description("The rectangle size for displaying the text")]
		public Size TextBoxSize
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				OnRefresh();
			}
		}
		[Description("The radius for each corner of the text box")]
		public float TextBoxCornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				_cornerRadius = value;
			}
		}
		[XmlIgnore]
		[Description("It indicates the Background color of the text when the UseFormBackgroundColor property is false")]
		public Color TextBackColor
		{
			get
			{
				return _bkColor;
			}
			set
			{
				_bkColor = value;
				OnRefresh();
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string TBColorString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				return converter.ConvertToInvariantString(_bkColor);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_bkColor = System.Drawing.Color.Black;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
					_bkColor = (Color)converter.ConvertFromInvariantString(value);
				}
			}
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
		[Description("If this property is true then the TextBackColor property is ignored and the form's background color is used as the text background color")]
		public bool UseFormBackgroundColor
		{
			get
			{
				return _useFormBackGroundColor;
			}
			set
			{
				_useFormBackGroundColor = value;
			}
		}
		#endregion
		#region ITextInput
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
		[Browsable(false)]
		[NotForProgramming]
		[ReadOnly(true)]
		[XmlIgnore]
		public int TextBoxWidth
		{
			get
			{
				if (this.Rectangle == Rectangle.Empty)
					return 30;
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
