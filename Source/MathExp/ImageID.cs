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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Windows.Forms;

namespace MathExp
{
	[Editor(typeof(ImageIDEditor), typeof(UITypeEditor))]
	public class ImageID
	{
		public ImageID()
		{
		}
		public override string ToString()
		{
			return "Image image";
		}
		private UInt32 _id;
		public UInt32 ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		private Bitmap _img;
		public Bitmap SelectedImage
		{
			get
			{
				return _img;
			}
			set
			{
				_img = value;
			}
		}
	}
	class ImageIDEditor : UITypeEditor
	{
		public ImageIDEditor()
		{
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				edSvc.CloseDropDown();
				ImageSelectControl isc = new ImageSelectControl((IImageList)context.Instance, edSvc);
				edSvc.DropDownControl(isc);
				if (isc.ImageIDSelected != null)
				{
					return isc.ImageIDSelected;
				}
			}
			return value;
		}
		// Indicates whether the UITypeEditor supports painting a 
		// representation of a property's value.
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		// Draws a representation of the property's value.
		public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
		{
			ImageID imgid = (ImageID)e.Value;
			if (imgid.SelectedImage != null)
			{
				e.Graphics.DrawImage(imgid.SelectedImage, e.Bounds);
			}
			else
			{
				Font ft = new Font("Times New Roman", e.Bounds.Height, GraphicsUnit.Pixel);
				e.Graphics.DrawString(imgid.ID.ToString(), ft, Brushes.Black, (float)0, (float)0);
			}
		}
	}
	internal class ImageSelectControl : System.Windows.Forms.UserControl
	{
		private IImageList _owner;
		private ImageListControl _list;
		public ImageID ImageIDSelected;
		public IWindowsFormsEditorService EdSvc;
		public ImageSelectControl(IImageList owner, IWindowsFormsEditorService edSvc)
		{
			EdSvc = edSvc;
			_owner = owner;
			_list = new ImageListControl();
			_list.Dock = DockStyle.Fill;
			List<ImageID> images = _owner.ImageList;
			if (images != null)
			{
				for (int i = 0; i < images.Count; i++)
				{
					_list.Items.Add(images[i]);
				}
			}
			this.Controls.Add(_list);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		class ImageListControl : ListBox
		{
			Pen pen;
			public ImageListControl()
			{
				pen = new Pen(Brushes.Blue, 3);
				this.DrawMode = DrawMode.OwnerDrawFixed;
				this.ItemHeight = 48;
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				e.DrawBackground();
				if (e.Index >= 0)
				{
					ImageID iid = this.Items[e.Index] as ImageID;
					e.Graphics.DrawImage(iid.SelectedImage, e.Bounds);
					if ((e.State & DrawItemState.Selected) != 0)
					{
						e.DrawFocusRectangle();
					}
				}
			}
			protected override void OnMouseMove(MouseEventArgs e)
			{
				this.SelectedIndex = this.IndexFromPoint(e.X, e.Y);
			}
			protected override void OnMouseDown(MouseEventArgs e)
			{
				this.SelectedIndex = this.IndexFromPoint(e.X, e.Y);
				if (this.SelectedIndex >= 0)
					((ImageSelectControl)Parent).ImageIDSelected = this.Items[this.SelectedIndex] as ImageID;
				else
					((ImageSelectControl)Parent).ImageIDSelected = null;
				((ImageSelectControl)Parent).EdSvc.CloseDropDown();
			}
		}
	}

}
