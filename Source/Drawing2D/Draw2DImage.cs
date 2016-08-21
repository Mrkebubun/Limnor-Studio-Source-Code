/*
 
 * * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Drawing.Design;
using VPL;
using System.IO;
using System.Data;
using LFilePath;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawImage))]
	[ToolboxBitmapAttribute(typeof(DrawImage), "Resources.img.bmp")]
	[Description("This object represents an Image.")]
	public class Draw2DImage : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DImage()
		{
			AllowSelectFileByClick = false;
			AllowPanningByMouse = false;
		}
		static Draw2DImage()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Angle");
			propertyNames.Add("SizeMode");
			propertyNames.Add("Rectangle");
			propertyNames.Add("Image");
			propertyNames.Add("Filename");
			propertyNames.Add("ImageStartPoint");
			propertyNames.Add("AllowPanningByMouse");
			propertyNames.Add("AllowSelectFileByClick");
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
		private DrawImage Image0
		{
			get
			{
				return (DrawImage)Item;
			}
		}
		#region Properties
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether clicking on this image will launch a file selection dialogue box for selecting an image file.")]
		public bool AllowSelectFileByClick
		{
			get
			{
				if (Image0 != null)
					return Image0.AllowSelectFileByClick;
				return false;
			}
			set
			{
				if (Image0 != null)
					Image0.AllowSelectFileByClick = value;
			}
		}
		[DefaultValue(false)]
		[Description("When the SizeMode is Normal, the Angle is 0, and the image is larger than the size of this component, setting this property to true will allow the user to pan the image by mouse")]
		public bool AllowPanningByMouse
		{
			get
			{
				if (Image0 != null)
					return Image0.AllowPanningByMouse;
				return false;
			}
			set
			{
				if (Image0 != null)
					Image0.AllowPanningByMouse = value;
			}
		}
		[Description("When the SizeMode is Normal, the Angle is 0, and the image is larger than the size of this component, this property indicate the point to start drawing the image.")]
		public Point ImageStartPoint
		{
			get
			{
				if (Image0 != null)
					return Image0.ImageStartPoint;
				return new Point(0, 0);
			}
			set
			{
				if (Image0 != null)
					Image0.ImageStartPoint = value;
			}
		}
		[FilePath("Image files|*.png;*.jpg;*.gif", "Select background image")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Bindable(true)]
		[Description("Full path to the image file")]
		public string Filename
		{
			get
			{
				if (Image0 != null)
					return Image0.Filename;
				return string.Empty;
			}
			set
			{
				if (Image0 != null)
					Image0.Filename = value;
			}
		}
		[Bindable(true)]
		[Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
		[Description("Image to be displayed")]
		public Image Image
		{
			get
			{
				if (Image0 != null)
					return Image0.Image;
				return null;
			}
			set
			{
				if (Image0 != null)
					Image0.Image = value;
			}
		}
		[Description("Rectangle containing the image")]
		public Rectangle Rectangle
		{
			get
			{
				if (Image0 != null)
					return Image0.Rectangle;
				return this.Bounds;
			}
			set
			{
				if (Image0 != null)
					Image0.Rectangle = value;
			}
		}
		[Description("This property defines how to resize the picture")]
		public PictureBoxSizeMode SizeMode
		{
			get
			{
				if (Image0 != null)
					return Image0.SizeMode;
				return PictureBoxSizeMode.Normal;
			}
			set
			{
				if (Image0 != null)
				{
					Image0.SizeMode = value;
					if (Image0.SizeMode == PictureBoxSizeMode.AutoSize)
					{
						if (Image0.Image != null)
						{
							this.Size = Image0.Image.Size;
						}
					}
					this.Refresh();
				}
			}
		}
		[Description("The angle to rotate the image")]
		public double Angle
		{
			get
			{
				if (Image0 != null)
					return Image0.Angle;
				return 0;
			}
			set
			{
				if (Image0 != null)
					Image0.Angle = value;
			}
		}
		#endregion
	}
	//====================================================================
	[TypeMapping(typeof(Draw2DImage))]
	[ToolboxBitmapAttribute(typeof(DrawImage), "Resources.img.bmp")]
	[Description("This object represents an Image.")]
	public class DrawImage : DrawingItem, IImageOnRect
	{
		#region fields and constructors
		private string sFilename = "";
		private Rectangle rc = new Rectangle(30, 30, 100, 62);
		private bool bCenter = false;
		private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;
		private double _angle = 0;
		private Image bmp;
		private Point _imageStartPoint = new Point(0, 0); //only used when size mode is Normal and bmp is larger than rc
		public DrawImage()
		{
			AllowSelectFileByClick = false;
			AllowPanningByMouse = false;
		}
		#endregion

		#region IImageOnRect
		[Browsable(false)]
		public Pen BorderPen { get{return Pens.Black; }}
		[Browsable(false)]
		public Rectangle Rect { get { return rc; } }
		#endregion
		#region Properties
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether clicking on this image will launch a file selection dialogue box for selecting an image file.")]
		public bool AllowSelectFileByClick
		{
			get;
			set;
		}
		[DefaultValue(false)]
		[Description("When the SizeMode is Normal, the Angle is 0, and the image is larger than the size of this component, setting this property to true will allow the user to pan the image by mouse")]
		public bool AllowPanningByMouse
		{
			get;
			set;
		}
		[Description("When the SizeMode is Normal, the Angle is 0, and the image is larger than the size of this component, this property indicate the point to start drawing the image.")]
		public Point ImageStartPoint
		{
			get
			{
				return _imageStartPoint;
			}
			set
			{
				_imageStartPoint = value;
			}
		}
		[Bindable(true)]
		[Description("Full path to the image file")]
		public string Filename
		{
			get
			{
				return sFilename;
			}
			set
			{
				sFilename = value;
				if (!string.IsNullOrEmpty(sFilename))
				{
					if (System.IO.File.Exists(sFilename))
					{
						try
						{
							bmp = Image.FromFile(sFilename);
						}
						catch
						{
						}
					}
					else
					{
						bmp = null;
					}
				}
				OnRefresh();
			}
		}

		[Bindable(true)]
		[XmlIgnore]
		[Description("Image to be displayed")]
		public Image Image
		{
			get
			{
				return bmp;
			}
			set
			{
				bmp = value;
				OnRefresh();
			}
		}
		[Browsable(false)]
		[XmlElementAttribute("ImageData")]
		public byte[] ImageByteArray
		{
			get
			{
				if (Image != null)
				{
					TypeConverter BitmapConverter =
						 TypeDescriptor.GetConverter(Image.GetType());
					return (byte[])
						 BitmapConverter.ConvertTo(Image, typeof(byte[]));
				}
				else
					return null;
			}

			set
			{
				if (value != null)
					Image = new Bitmap(new MemoryStream(value));
				else
					Image = null;
			}
		}

		[Description("Rectangle containing the image")]
		public Rectangle Rectangle
		{
			get
			{
				return rc;
			}
			set
			{
				rc = value;
				Refresh();
			}
		}
		[Description("This property defines how to resize the picture")]
		public PictureBoxSizeMode SizeMode
		{
			get
			{
				return _sizeMode;
			}
			set
			{
				_sizeMode = value;
				if (_sizeMode == PictureBoxSizeMode.AutoSize && bmp != null)
				{
					rc.Width = bmp.Width;
					rc.Height = bmp.Height;
					OnSizeChanged();
				}
				OnRefresh();
			}
		}
		[Description("The angle to rotate the image")]
		public double Angle
		{
			get
			{
				return _angle;
			}
			set
			{
				_angle = value;
				Refresh();
			}
		}
		public override System.Drawing.Rectangle Bounds
		{
			get
			{
				return DrawingItem.GetBounds(rc, Angle, Page);
			}
			set
			{
				if (Angle == 0 || Angle == 180 || Angle == -180)
				{
					rc = value;
					SizeAcceptedBySetBounds = true;
					if (_sizeMode == PictureBoxSizeMode.AutoSize)
					{
						if (bmp != null)
						{
							if (rc.Height != bmp.Height || rc.Width != bmp.Width)
							{
								_sizeMode = PictureBoxSizeMode.Normal;
							}
						}
					}
				}
				else
				{
					SizeAcceptedBySetBounds = false;
				}
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				return new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
			}
			set
			{
				rc.X = value.X - rc.Width / 2;
				rc.Y = value.Y - rc.Height / 2;
			}
		}
		public override int Left
		{
			get
			{
				return rc.X;
			}
			set
			{
				rc.X = value;
				Refresh();
			}
		}
		public override int Top
		{
			get
			{
				return rc.Y;
			}
			set
			{
				rc.Y = value;
				Refresh();
			}
		}
		#endregion

		#region Design
		public override void ResetDefaultProperties()
		{
			bmp = Resource1.img;
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			if (Angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc.X + rc.Width / 2, rc.Y + rc.Height / 2, Angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (x >= rc.X && x <= rc.Right && y >= rc.Y && y <= rc.Bottom)
			{
				return true;
			}
			return false;
		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			designer = new DrawingDesign();
			designer.Marks = new DrawingMark[6];
			//
			designer.Marks[0] = new DrawingMark();
			designer.Marks[0].Index = 0;
			designer.Marks[0].Owner = this;
			Page.Controls.Add(designer.Marks[0]);
			//
			designer.Marks[1] = new DrawingMark();
			designer.Marks[1].Index = 1;
			designer.Marks[1].Owner = this;
			Page.Controls.Add(designer.Marks[1]);
			//
			designer.Marks[2] = new DrawingMark();
			designer.Marks[2].Index = 2;
			designer.Marks[2].Owner = this;
			Page.Controls.Add(designer.Marks[2]);
			//
			designer.Marks[3] = new DrawingMark();
			designer.Marks[3].Index = 3;
			designer.Marks[3].Owner = this;
			Page.Controls.Add(designer.Marks[3]);
			//
			designer.Marks[4] = new DrawingMover();
			designer.Marks[4].Index = 4;
			designer.Marks[4].Owner = this;
			Page.Controls.Add(designer.Marks[4]);
			//
			//double angle = (Angle/180)*Math.PI;
			designer.Marks[5] = new DrawingMark();
			designer.Marks[5].Index = 5;
			designer.Marks[5].BackColor = System.Drawing.Color.Red;
			designer.Marks[5].Owner = this;
			designer.Marks[5].Cursor = System.Windows.Forms.Cursors.Cross;
			Page.Controls.Add(designer.Marks[5]);
			//
			SetMarks();
			//
			return designer;
		}
		protected void SetMarks2()
		{
			if (Angle != 0)
			{
				double xo, yo;
				double xc = designer.Marks[4].X;
				double yc = designer.Marks[4].Y;
				xo = rc.X + Page.AutoScrollPosition.X;
				yo = rc.Y + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
				designer.Marks[0].X = (int)xo;
				designer.Marks[0].Y = (int)yo;
				//
				xo = rc.Right + Page.AutoScrollPosition.X;
				yo = rc.Y + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
				designer.Marks[1].X = (int)xo;
				designer.Marks[1].Y = (int)yo;
				//
				xo = rc.X + Page.AutoScrollPosition.X;
				yo = rc.Bottom + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
				designer.Marks[2].X = (int)xo;
				designer.Marks[2].Y = (int)yo;
				//
				xo = rc.Right + Page.AutoScrollPosition.X;
				yo = rc.Bottom + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
				designer.Marks[3].X = (int)xo;
				designer.Marks[3].Y = (int)yo;
			}
			else
			{
				designer.Marks[0].X = rc.X + Page.AutoScrollPosition.X;
				designer.Marks[0].Y = rc.Y + Page.AutoScrollPosition.Y;
				designer.Marks[1].X = rc.Right + Page.AutoScrollPosition.X;
				designer.Marks[1].Y = rc.Y + Page.AutoScrollPosition.Y;
				designer.Marks[2].X = rc.X + Page.AutoScrollPosition.X;
				designer.Marks[2].Y = rc.Bottom + Page.AutoScrollPosition.Y;
				designer.Marks[3].X = rc.Right + Page.AutoScrollPosition.X;
				designer.Marks[3].Y = rc.Bottom + Page.AutoScrollPosition.Y;
			}
		}
		public override void SetMarks()
		{
			double angle = (Angle / 180) * Math.PI;
			//
			designer.Marks[4].X = rc.X + rc.Width / 2 + Page.AutoScrollPosition.X;
			designer.Marks[4].Y = rc.Y + rc.Height / 2 + Page.AutoScrollPosition.Y;
			designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
			designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
			//
			SetMarks2();
		}
		private int x0;
		private int y0;
		private int xp;
		private int yp;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			x0 = e.X;
			y0 = e.Y;
			xp = _imageStartPoint.X;
			yp = _imageStartPoint.Y;

		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (SizeMode == PictureBoxSizeMode.Normal && Angle == 0)
			{
				//pan the image
				if (this.AllowPanningByMouse)
				{
					if (e.Button == MouseButtons.Left)
					{
						_imageStartPoint.X = xp + x0 - e.X;
						_imageStartPoint.Y = yp + y0 - e.Y;
						if (this.Page != null)
						{
							this.Page.Invalidate(this.GetBoundsOnPage());
						}
					}
				}
			}
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			//0   1
			//  4  5
			//2   3
			if (sender.Parent == null)
				return;
			while (Angle <= -360)
				Angle += 360;
			while (Angle >= 360)
				Angle -= 360;

			double angle = (Angle / 180) * Math.PI;
			switch (sender.Index)
			{
				case 0:
					if (Math.Abs(Angle) > 0.001 && !(Math.Abs(Angle - 180) < 0.001 || Math.Abs(Angle + 180) < 0.001))
					{
						if (Math.Abs(Angle - 90) < 0.001 || Math.Abs(Angle + 270) < 0.001)
						{
							designer.Marks[1].X = designer.Marks[0].X;
							designer.Marks[2].Y = designer.Marks[0].Y;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[0].X < designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[0].Y > designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(Angle + 90) < 0.001 || Math.Abs(Angle - 270) < 0.001)
						{
							designer.Marks[2].Y = designer.Marks[0].Y;
							designer.Marks[1].X = designer.Marks[0].X;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[0].X > designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[0].Y < designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//point 1, use point 2 as fixed point
							double c = designer.Marks[0].Y - designer.Marks[3].Y - (designer.Marks[0].X - designer.Marks[3].X) * Math.Sin(angle) / Math.Cos(angle);
							double x1 = -Math.Cos(angle) * Math.Sin(angle) * c;
							double y1 = Math.Cos(angle) * Math.Cos(angle) * c;
							//
							c = designer.Marks[0].Y - designer.Marks[3].Y + (designer.Marks[0].X - designer.Marks[3].X) * Math.Cos(angle) / Math.Sin(angle);
							double x2 = Math.Sin(angle) * Math.Cos(angle) * c;
							double y2 = Math.Sin(angle) * Math.Sin(angle) * c;
							//
							designer.Marks[1].X = (int)Math.Round(x1 + designer.Marks[3].X, 0);
							designer.Marks[1].Y = (int)Math.Round(y1 + designer.Marks[3].Y, 0);
							designer.Marks[2].X = (int)Math.Round(x2 + designer.Marks[3].X, 0);
							designer.Marks[2].Y = (int)Math.Round(y2 + designer.Marks[3].Y, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((Angle >= -90 && Angle <= 90) || (Angle > 270) || (Angle < -270))
							{
								if (designer.Marks[1].Y > designer.Marks[3].Y && designer.Marks[1].X > designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[1].Y < designer.Marks[3].Y && designer.Marks[1].X < designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);

						}
					}
					else
					{
						designer.Marks[1].Y = designer.Marks[0].Y;
						designer.Marks[2].X = designer.Marks[0].X;
						designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
						designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
						if (designer.Marks[0].X > designer.Marks[1].X)
							rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
						else
							rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
						if (designer.Marks[0].Y > designer.Marks[3].Y)
							rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
						else
							rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
						rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
						rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
					break;
				case 1:
					if (Math.Abs(Angle) > 0.001 && !(Math.Abs(Angle - 180) < 0.001 || Math.Abs(Angle + 180) < 0.001))
					{
						if (Math.Abs(Angle - 90) < 0.001 || Math.Abs(Angle + 270) < 0.001)
						{
							designer.Marks[0].X = designer.Marks[1].X;
							designer.Marks[3].Y = designer.Marks[1].Y;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[1].X < designer.Marks[3].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[1].Y < designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(Angle + 90) < 0.001 || Math.Abs(Angle - 270) < 0.001)
						{
							designer.Marks[3].Y = designer.Marks[1].Y;
							designer.Marks[0].X = designer.Marks[1].X;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[1].X > designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[1].Y > designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//point 1, use point 2 as fixed point
							double c = designer.Marks[1].Y - designer.Marks[2].Y - (designer.Marks[1].X - designer.Marks[2].X) * Math.Sin(angle) / Math.Cos(angle);
							double x00 = -Math.Cos(angle) * Math.Sin(angle) * c;
							double y00 = Math.Cos(angle) * Math.Cos(angle) * c;
							//
							c = designer.Marks[1].Y - designer.Marks[2].Y + (designer.Marks[1].X - designer.Marks[2].X) * Math.Cos(angle) / Math.Sin(angle);
							double x3 = Math.Sin(angle) * Math.Cos(angle) * c;
							double y3 = Math.Sin(angle) * Math.Sin(angle) * c;
							//
							designer.Marks[3].X = (int)Math.Round(x3 + designer.Marks[2].X, 0);
							designer.Marks[3].Y = (int)Math.Round(y3 + designer.Marks[2].Y, 0);
							designer.Marks[0].X = (int)Math.Round(x00 + designer.Marks[2].X, 0);
							designer.Marks[0].Y = (int)Math.Round(y00 + designer.Marks[2].Y, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);

							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((Angle >= -90 && Angle <= 90) || (Angle > 270) || (Angle < -270))
							{
								if (designer.Marks[1].Y > designer.Marks[3].Y && designer.Marks[1].X > designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[1].Y < designer.Marks[3].Y && designer.Marks[1].X < designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);

						}
					}
					else
					{
						designer.Marks[0].Y = designer.Marks[1].Y;
						designer.Marks[3].X = designer.Marks[1].X;
						designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
						designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
						if (designer.Marks[0].X > designer.Marks[1].X)
							rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
						else
							rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
						if (designer.Marks[0].Y > designer.Marks[3].Y)
							rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
						else
							rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
						rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
						rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
					break;
				case 2:
					if (Math.Abs(Angle) > 0.001 && !(Math.Abs(Angle - 180) < 0.001 || Math.Abs(Angle + 180) < 0.001))
					{
						if (Math.Abs(Angle - 90) < 0.001 || Math.Abs(Angle + 270) < 0.001)
						{
							designer.Marks[0].Y = designer.Marks[2].Y;
							designer.Marks[3].X = designer.Marks[2].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[2].X > designer.Marks[0].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[2].Y > designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(Angle + 90) < 0.001 || Math.Abs(Angle - 270) < 0.001)
						{
							designer.Marks[0].Y = designer.Marks[2].Y;
							designer.Marks[3].X = designer.Marks[2].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[2].X < designer.Marks[0].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[2].Y < designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//
							double c = designer.Marks[2].Y - designer.Marks[1].Y + (designer.Marks[2].X - designer.Marks[1].X) * Math.Cos(angle) / Math.Sin(angle);
							double x00 = Math.Cos(angle) * Math.Sin(angle) * c;
							double y00 = Math.Sin(angle) * Math.Sin(angle) * c;
							//
							c = designer.Marks[2].Y - designer.Marks[1].Y - (designer.Marks[2].X - designer.Marks[1].X) * Math.Sin(angle) / Math.Cos(angle);
							double x3 = -Math.Sin(angle) * Math.Cos(angle) * c;
							double y3 = Math.Cos(angle) * Math.Cos(angle) * c;
							//
							designer.Marks[3].X = (int)Math.Round(x3 + designer.Marks[1].X, 0);
							designer.Marks[3].Y = (int)Math.Round(y3 + designer.Marks[1].Y, 0);
							designer.Marks[0].X = (int)Math.Round(x00 + designer.Marks[1].X, 0);
							designer.Marks[0].Y = (int)Math.Round(y00 + designer.Marks[1].Y, 0);


							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((Angle >= -90 && Angle <= 90) || (Angle > 270) || (Angle < -270))
							{
								if (designer.Marks[2].Y < designer.Marks[0].Y && designer.Marks[2].X < designer.Marks[3].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[2].X > designer.Marks[3].X && designer.Marks[2].Y > designer.Marks[0].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[2].X > designer.Marks[3].X && designer.Marks[2].Y < designer.Marks[0].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[2].Y > designer.Marks[0].Y && designer.Marks[2].X > designer.Marks[3].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[2].X < designer.Marks[3].X && designer.Marks[2].Y < designer.Marks[0].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[2].X < designer.Marks[3].X && designer.Marks[2].Y > designer.Marks[0].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
						}
					}
					else
					{
						designer.Marks[3].Y = designer.Marks[2].Y;
						designer.Marks[0].X = designer.Marks[2].X;
						designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
						designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
						if (designer.Marks[0].X > designer.Marks[1].X)
							rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
						else
							rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
						if (designer.Marks[0].Y > designer.Marks[3].Y)
							rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
						else
							rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
						rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
						rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
					break;
				case 3:
					if (Math.Abs(Angle) > 0.001 && !(Math.Abs(Angle - 180) < 0.001 || Math.Abs(Angle + 180) < 0.001))
					{
						if (Math.Abs(Angle - 90) < 0.001 || Math.Abs(Angle + 270) < 0.001)
						{
							designer.Marks[1].Y = designer.Marks[3].Y;
							designer.Marks[2].X = designer.Marks[3].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[3].X > designer.Marks[1].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[3].Y < designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(Angle + 90) < 0.001 || Math.Abs(Angle - 270) < 0.001)
						{
							designer.Marks[1].Y = designer.Marks[3].Y;
							designer.Marks[2].X = designer.Marks[3].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[3].X < designer.Marks[1].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[3].Y > designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//
							double c = designer.Marks[3].Y - designer.Marks[0].Y + (designer.Marks[3].X - designer.Marks[0].X) * Math.Cos(angle) / Math.Sin(angle);
							designer.Marks[1].X = (int)Math.Round(designer.Marks[0].X + Math.Cos(angle) * Math.Sin(angle) * c, 0);
							designer.Marks[1].Y = (int)Math.Round(designer.Marks[0].Y + Math.Sin(angle) * Math.Sin(angle) * c, 0);
							//
							c = designer.Marks[3].Y - designer.Marks[0].Y - (designer.Marks[3].X - designer.Marks[0].X) * Math.Sin(angle) / Math.Cos(angle);
							designer.Marks[2].X = designer.Marks[0].X - (int)Math.Round(Math.Sin(angle) * Math.Cos(angle) * c, 0);
							designer.Marks[2].Y = designer.Marks[0].Y + (int)Math.Round(Math.Cos(angle) * Math.Cos(angle) * c, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((Angle >= -90 && Angle <= 90) || (Angle > 270) || (Angle < -270))
							{
								if (designer.Marks[3].X < designer.Marks[2].X && designer.Marks[3].Y > designer.Marks[1].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[3].Y < designer.Marks[1].Y && designer.Marks[3].X > designer.Marks[2].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[3].X < designer.Marks[2].X && designer.Marks[3].Y < designer.Marks[1].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[3].X > designer.Marks[2].X && designer.Marks[3].Y < designer.Marks[1].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[3].Y > designer.Marks[1].Y && designer.Marks[3].X < designer.Marks[2].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[3].X > designer.Marks[2].X && designer.Marks[3].Y > designer.Marks[1].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, Angle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
						}
					}
					else
					{
						designer.Marks[2].Y = designer.Marks[3].Y;
						designer.Marks[1].X = designer.Marks[3].X;
						designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
						designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
						if (designer.Marks[0].X > designer.Marks[1].X)
							rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
						else
							rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
						if (designer.Marks[0].Y > designer.Marks[3].Y)
							rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
						else
							rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
						rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
						rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
					break;
				case 4:
					if (Angle != 0)
					{
						double xo, yo;
						double xc = designer.Marks[4].X;
						double yc = designer.Marks[4].Y;
						xo = designer.Marks[4].X - rc.Width / 2;
						yo = designer.Marks[4].Y - rc.Height / 2;
						rc.X = (int)xo - Page.AutoScrollPosition.X;
						rc.Y = (int)yo - Page.AutoScrollPosition.Y;
						DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
						designer.Marks[0].X = (int)xo;
						designer.Marks[0].Y = (int)yo;
						//
						xo = designer.Marks[4].X + rc.Width / 2;
						yo = designer.Marks[4].Y - rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
						designer.Marks[1].X = (int)xo;
						designer.Marks[1].Y = (int)yo;
						//
						xo = designer.Marks[4].X - rc.Width / 2;
						yo = designer.Marks[4].Y + rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
						designer.Marks[2].X = (int)xo;
						designer.Marks[2].Y = (int)yo;
						//
						xo = designer.Marks[4].X + rc.Width / 2;
						yo = designer.Marks[4].Y + rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -Angle, out xo, out yo);
						designer.Marks[3].X = (int)xo;
						designer.Marks[3].Y = (int)yo;
					}
					else
					{
						designer.Marks[0].X = designer.Marks[4].X - rc.Width / 2;
						designer.Marks[0].Y = designer.Marks[4].Y - rc.Height / 2;
						designer.Marks[1].X = designer.Marks[4].X + rc.Width / 2;
						designer.Marks[1].Y = designer.Marks[4].Y - rc.Height / 2;
						designer.Marks[2].X = designer.Marks[4].X - rc.Width / 2;
						designer.Marks[2].Y = designer.Marks[4].Y + rc.Height / 2;
						designer.Marks[3].X = designer.Marks[4].X + rc.Width / 2;
						designer.Marks[3].Y = designer.Marks[4].Y + rc.Height / 2;
						if (designer.Marks[0].X > designer.Marks[1].X)
							rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
						else
							rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
						if (designer.Marks[0].Y > designer.Marks[3].Y)
							rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
						else
							rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					}

					break;
				case 5:
					int x0 = designer.Marks[5].X - designer.Marks[4].X;
					int y0 = designer.Marks[5].Y - designer.Marks[4].Y;
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
			if (sender.Index != 5)
			{
				designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(angle));
				designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(angle));
			}
			else
			{
				SetMarks2();
			}
			if (sender.Index != 4)
				sender.Parent.Invalidate(Bounds);
			else
				sender.Parent.Invalidate();
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditImg dlgImg = new dlgEditImg();
			dlgImg.LoadData(this, owner);
			ret = dlgImg.ShowDialog(owner);
			return ret;
		}
		#endregion

		#region Methods
		[Browsable(false)]
		[Description("Add types to be serialized")]
		public override void OnGetSerializationTypes(List<Type> types)
		{
			if (Image != null)
			{
				Type t = Image.GetType();
				if (!types.Contains(t))
				{
					types.Add(t);
				}
				t = typeof(Image);
				if (!types.Contains(t))
				{
					types.Add(t);
				}
			}
		}
		/// <summary>
		/// move _imageStartPoint to newStartPoint:
		/// add 2 pixels to x and y towards newStartPoint in each loop
		/// </summary>
		/// <param name="newStartPoint"></param>
		[Description("Pan the image by moving the image to the new starting point specified by parameter newStartPoint. Parameter speed is a value greater than 0 indicating how many pixels it pans each time. The larger the value is the faster of the panning.")]// Parameter timeInMilliseconds specifies the time to be used to do the panning, in milliseconds.")]
		public void PanImage(Point newStartPoint, int speed)
		{
			if (this.Page != null)
			{
				Graphics g = this.Page.CreateGraphics();
				if (speed < 1)
					speed = 1;
				int dx, dy;
				//determine the moving direction for X
				if (newStartPoint.X > _imageStartPoint.X)
					dx = speed;
				else
					dx = -speed;
				//determine the moving direction for Y
				if (newStartPoint.Y > _imageStartPoint.Y)
					dy = speed;
				else
					dy = -speed;
				//do the panning
				bool bx = true;
				bool by = true;
				while (bx || by)
				{
					bx = false;
					if (dx > 0)
					{
						if (_imageStartPoint.X < newStartPoint.X)
						{
							bx = true;
						}
					}
					else
					{
						if (_imageStartPoint.X > newStartPoint.X)
						{
							bx = true;
						}
					}
					if (bx)
					{
						_imageStartPoint.X += dx;
						if (dx > 0)
						{
							if (_imageStartPoint.X > newStartPoint.X)
							{
								_imageStartPoint.X = newStartPoint.X;
							}
						}
						else
						{
							if (_imageStartPoint.X < newStartPoint.X)
							{
								_imageStartPoint.X = newStartPoint.X;
							}
						}
					}
					by = false;
					if (dy > 0)
					{
						if (_imageStartPoint.Y < newStartPoint.Y)
						{
							by = true;
						}
					}
					else
					{
						if (_imageStartPoint.Y > newStartPoint.Y)
						{
							by = true;
						}
					}
					if (by)
					{
						_imageStartPoint.Y += dy;
						if (dy > 0)
						{
							if (_imageStartPoint.Y > newStartPoint.Y)
							{
								_imageStartPoint.Y = newStartPoint.Y;
							}
						}
						else
						{
							if (_imageStartPoint.Y < newStartPoint.Y)
							{
								_imageStartPoint.Y = newStartPoint.Y;
							}
						}
					}
					OnDraw(g);
					if (bmp == null)
					{
						break;
					}
					if (_imageStartPoint.X >= bmp.Width - rc.Width)
					{
						bx = false;
					}
					if (_imageStartPoint.Y >= bmp.Height - rc.Height)
					{
						by = false;
					}
				}
			}
			else
			{
				_imageStartPoint = newStartPoint;
			}
		}
		public override void MoveByStep(int dx, int dy)
		{
			rc.X += dx;
			rc.Y += dy;
			Refresh();
		}
		public override void MoveTo(Point p)
		{
			rc.X = p.X;
			rc.Y = p.Y;
			Refresh();
		}
		public override void MoveCenterTo(Point p)
		{
			rc.X = p.X - rc.Width / 2;
			rc.Y = p.Y - rc.Height / 2;
			Refresh();
		}
		public override void ConvertOrigin(Point pt)
		{
			rc.X -= pt.X;
			rc.Y -= pt.Y;
			Refresh();
		}
		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			_angle = r.NextDouble() * 360.0;
		}
		public override string Help()
		{
			return "  Left-click to specify image location";
		}
		public override bool ForHotspots()
		{
			return false;
		}

		public override bool SetPage(int n)
		{
			return false;
		}
		public override void OnDraw(Graphics g)
		{
			DrawImageOnRect(g, this);
		}
		public static void DrawImageOnRect(Graphics g, IImageOnRect imgrect)
		{
			try
			{
				Rectangle rc = imgrect.Rect;
				g.DrawRectangle(imgrect.BorderPen, imgrect.Rect);
				if (imgrect.Image == null)
				{
					string fileToDraw = imgrect.Filename;
					if (!string.IsNullOrEmpty(fileToDraw))
					{
						imgrect.Image = (Bitmap)System.Drawing.Bitmap.FromFile(fileToDraw);
					}
				}
				Image bmp = imgrect.Image;
				if (bmp != null)
				{
					Point _imageStartPoint = imgrect.ImageStartPoint;
					if (imgrect.SizeMode == PictureBoxSizeMode.Normal && imgrect.Angle == 0)//support panning
					{
						if (_imageStartPoint.X < 0)
							_imageStartPoint.X = 0;
						if (_imageStartPoint.Y < 0)
							_imageStartPoint.Y = 0;
						if (imgrect.Image.Width <= rc.Width && imgrect.Image.Height <= rc.Height)
						{
							//image is smaller in width and height
							g.DrawImageUnscaled(imgrect.Image, rc);
						}
						else
						{
							Rectangle rcDest = new Rectangle(rc.Location, rc.Size);
							Rectangle rcSrc = new Rectangle(new Point(0, 0), imgrect.Image.Size);
							rcDest.Width = Math.Min(imgrect.Image.Width, rc.Width);
							rcSrc.Width = rcDest.Width;
							rcDest.Height = Math.Min(imgrect.Image.Height, rc.Height);
							rcSrc.Height = rcDest.Height;
							if (_imageStartPoint.X != 0 || _imageStartPoint.Y != 0)
							{
								if (bmp.Width > rc.Width)
								{
									if (bmp.Width - _imageStartPoint.X < rc.Width)
									{
										_imageStartPoint.X = bmp.Width - rc.Width;
									}
								}
								else
								{
									_imageStartPoint.X = 0;
								}
								if (bmp.Height > rc.Height)
								{
									if (bmp.Height - _imageStartPoint.Y < rc.Height)
									{
										_imageStartPoint.Y = bmp.Height - rc.Height;
									}
								}
								else
								{
									_imageStartPoint.Y = 0;
								}
								rcSrc.Location = _imageStartPoint;
							}
							g.DrawImage(bmp, rcDest, rcSrc, GraphicsUnit.Pixel);
						}
					}
					else
					{
						if (imgrect.SizeMode == PictureBoxSizeMode.Zoom)
						{
							double nWidth = bmp.Width;
							double nHeight = bmp.Height;
							double nLeft = rc.X;
							double nTop = rc.Y;
							if (rc.Width > 0 && rc.Height > 0 && nWidth > 0 && nHeight > 0)
							{
								double Height = rc.Height;
								double Width = rc.Width;

								double r = imgrect.Angle;
								double a = r;
								while (a < 0)
								{
									a += 360;
								}
								while (a >= 360)
								{
									a -= 360;
								}
								double a0 = Math.Atan(nHeight / nWidth) * 180.0 / Math.PI;
								double rt = nHeight / nWidth;
								if (a <= 90)
								{
									double gNew = Height / Math.Cos((90 - a0 - a) * Math.PI / 180.0);
									double gNew2 = Width / Math.Cos((a0 - a) * Math.PI / 180.0);

									if (gNew < gNew2)
										nWidth = gNew / Math.Sqrt(1 + rt * rt);
									else
										nWidth = gNew2 / Math.Sqrt(1 + rt * rt);
									nHeight = rt * nWidth;
								}
								else if (a <= 180)
								{
									double gNew = Height / Math.Sin((a - a0) * Math.PI / 180.0);
									double gNew2 = Width / Math.Cos((180 - a0 - a) * Math.PI / 180.0);
									if (gNew < gNew2)
										nWidth = gNew / Math.Sqrt(1 + rt * rt);
									else
										nWidth = gNew2 / Math.Sqrt(1 + rt * rt);
									nHeight = rt * nWidth;
								}
								else if (a <= 270)
								{
									double gNew = Height / Math.Cos((270 - a - a0) * Math.PI / 180.0);
									double gNew2 = Width / Math.Cos((a - 180 - a0) * Math.PI / 180.0);
									if (gNew < gNew2)
										nWidth = gNew / Math.Sqrt(1 + rt * rt);
									else
										nWidth = gNew2 / Math.Sqrt(1 + rt * rt);
									nHeight = rt * nWidth;
								}
								else // if( a < 360 )
								{
									double gNew = Height / Math.Cos((a - 270 - a0) * Math.PI / 180.0);
									double gNew2 = Width / Math.Cos((360 - a - a0) * Math.PI / 180.0);
									if (gNew < gNew2)
										nWidth = gNew / Math.Sqrt(1 + rt * rt);
									else
										nWidth = gNew2 / Math.Sqrt(1 + rt * rt);
									nHeight = rt * nWidth;
								}
								nLeft = (Width - nWidth) / 2;
								nTop = (Height - nHeight) / 2;
								System.Drawing.Drawing2D.GraphicsState transState = g.Save();
								g.TranslateTransform((float)(rc.Width / 2.0 + rc.X), (float)(rc.Height / 2.0 + rc.Y));
								g.RotateTransform((float)r);
								nLeft -= Width / 2;
								nTop -= Height / 2;

								g.DrawImage(bmp, (float)nLeft, (float)nTop, (float)nWidth, (float)nHeight);
								g.Restore(transState);
							}
						}
						else
						{
							Rectangle rcDest = new Rectangle(rc.Location, rc.Size);
							Rectangle rcSrc = new Rectangle(new Point(0, 0), bmp.Size);
							if (imgrect.SizeMode == PictureBoxSizeMode.AutoSize)
							{
								rc.Width = bmp.Width;
								rc.Height = bmp.Height;
								rcDest.Width = bmp.Width;
								rcDest.Height = bmp.Height;
							}
							else
							{
								if (imgrect.SizeMode == PictureBoxSizeMode.Normal)
								{
									rcDest.Width = Math.Min(bmp.Width, rc.Width);
									rcSrc.Width = rcDest.Width;
									rcDest.Height = Math.Min(bmp.Height, rc.Height);
									rcSrc.Height = rcDest.Height;
								}
								else if (imgrect.SizeMode == PictureBoxSizeMode.CenterImage)
								{
									rcDest.Width = Math.Min(bmp.Width, rc.Width);
									rcSrc.Width = rcDest.Width;
									rcDest.Height = Math.Min(bmp.Height, rc.Height);
									rcSrc.Height = rcDest.Height;
									float x, y;
									x = (float)((rc.Width - bmp.Width) / 2.0);
									if (x < 0)
									{
										rcDest.X = rc.X;
									}
									else
									{
										rcDest.X = rc.X + (int)x;
									}
									y = (float)((rc.Height - bmp.Height) / 2.0);
									if (y < 0)
									{
										rcDest.Y = rc.Y;
									}
									else
									{
										rcDest.Y = rc.Y + (int)y;
									}
								}
								else if (imgrect.SizeMode == PictureBoxSizeMode.StretchImage)
								{
								}
							}
							System.Drawing.Drawing2D.GraphicsState transState = g.Save();
							double angle = (imgrect.Angle / 180) * Math.PI;
							g.TranslateTransform(
								(rcDest.Width + (float)(rcDest.Height * Math.Sin(angle)) - (float)(rcDest.Width * Math.Cos(angle))) / 2 + rcDest.X,
								(rcDest.Height - (float)(rcDest.Height * Math.Cos(angle)) - (float)(rcDest.Width * Math.Sin(angle))) / 2 + rcDest.Y);
							g.RotateTransform((float)imgrect.Angle);
							System.Drawing.Rectangle rc2 = new Rectangle(0, 0, rcDest.Width, rcDest.Height);
							g.DrawImage(bmp, rc2, rcSrc, System.Drawing.GraphicsUnit.Pixel);
							g.Restore(transState);
						}
					}
				}
			}
			catch
			{
			}
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			rc.X = e.X;
			rc.Y = e.Y;
			dlgEditImg dlgImg = new dlgEditImg();
			dlgImg.LoadData(this, owner);
			ret = dlgImg.ShowDialog(owner);
			return ret;
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep == 0)
			{
				rc.X = e.X;
				rc.Y = e.Y;
				bRet = true;
			}
			return bRet;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawImage v = obj as DrawImage;
			if (v != null)
			{
				this.sFilename = v.sFilename;
				this.bmp = v.bmp;
				this.rc = v.rc;
				this.bCenter = v.bCenter;
				this.Angle = v.Angle;
				this.SizeMode = v.SizeMode;
				this.AllowSelectFileByClick = v.AllowSelectFileByClick;
				this.AllowPanningByMouse = v.AllowPanningByMouse;
			}

		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[{1}]", Name, sFilename);
		}
		#endregion

	}
	public interface IImageOnRect
	{
		Pen BorderPen { get; }
		Rectangle Rect { get; }
		Image Image { get; set; }
		PictureBoxSizeMode SizeMode { get; }
		double Angle { get; }
		Point ImageStartPoint { get;}
		string Filename { get; }
	}
	public abstract class ImageOnRect : IImageOnRect
	{
		public abstract Pen BorderPen
		{
			get;
		}

		public abstract Rectangle Rect
		{
			get;
		}

		public abstract Image Image
		{
			get;set;
		}

		public abstract PictureBoxSizeMode SizeMode
		{
			get;
		}

		public abstract double Angle
		{
			get;
		}

		public abstract Point ImageStartPoint
		{
			get;
		}

		public abstract string Filename
		{
			get;
		}
	}
}
