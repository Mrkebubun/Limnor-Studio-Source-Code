/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	/// <summary>
	/// all items should be ParameterRef
	/// </summary>
	public partial class ParameterListBox : ListBox
	{
		public ParameterListBox()
		{
			InitializeComponent();
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}
		public int FixedParameterCount { get; set; }
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < this.Items.Count)
			{
				bool selected = ((e.State & DrawItemState.Selected) != 0);
				ParameterClass item = this.Items[e.Index] as ParameterClass;
				if (item != null)
				{
					Rectangle rcBK = new Rectangle(e.Bounds.Left, e.Bounds.Top, 1, this.ItemHeight);
					if (e.Bounds.Width > this.ItemHeight)
					{
						rcBK.Width = e.Bounds.Width - this.ItemHeight;
						rcBK.X = this.ItemHeight;
						if (selected)
						{
							e.Graphics.FillRectangle(Brushes.LightBlue, rcBK);
						}
						else
						{
							if (e.Index < FixedParameterCount)
							{
								e.Graphics.FillRectangle(Brushes.LightGray, rcBK);
							}
							else
							{
								e.Graphics.FillRectangle(Brushes.White, rcBK);
							}
						}
					}
					Rectangle rc = new Rectangle(e.Bounds.Left, e.Bounds.Top, this.ItemHeight, this.ItemHeight);
					float w = (float)(e.Bounds.Width - this.ItemHeight);
					if (w > 0)
					{
						RectangleF rcf = new RectangleF((float)(rc.Left + this.ItemHeight + 2), (float)(rc.Top), w, (float)this.ItemHeight);
						if (selected)
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.White, rcf);
						}
						else
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.Black, rcf);
						}
					}
					e.Graphics.DrawImage(item.Icon, rc);
				}
				else
				{
					if (selected)
					{
						e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
					}
					else
					{
						e.Graphics.FillRectangle(Brushes.White, e.Bounds);
					}
					if (this.Items[e.Index] != null)
					{
						e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds.Left, e.Bounds.Top);
					}
				}
			}
		}
	}
}
