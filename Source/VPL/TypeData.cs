/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public class FuTypeData
	{
		Image _img;
		public FuTypeData()
		{
		}
		public FuTypeData(Type t, string desc)
		{
			this.Type = t;
			this.Description = desc;
		}
		public Type Type { get; set; }
		public string Description { get; set; }
		public Image Image
		{
			get
			{
				if (_img == null)
				{
					_img = VPLUtil.GetTypeIcon(this.Type);
				}
				return _img;
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}. {1}", Type.Name, Description);
		}
	}
	public class TypeDataListBox : ListBox
	{
		Pen pen;
		public TypeDataListBox()
		{
			pen = new Pen(Brushes.Blue, 3);
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.ItemHeight = 32;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0 && e.Index < this.Items.Count)
			{
				FuTypeData iid = this.Items[e.Index] as FuTypeData;
				e.Graphics.DrawImage(iid.Image, e.Bounds);
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
		}
	}
}
