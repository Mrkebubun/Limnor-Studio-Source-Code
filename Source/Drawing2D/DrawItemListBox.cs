/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.Drawing2D
{
	class DrawItemListItem
	{
		Type _type;
		Image _img;
		public DrawItemListItem(Type t)
		{
			_type = t;
		}
		public Type ItemType
		{
			get
			{
				return _type;
			}
		}
		public Image ItemImage
		{
			get
			{
				if (_img == null)
				{
					_img = DrawingItem.GetTypeIcon(_type);
				}
				return _img;
			}
		}
		public override string ToString()
		{
			return _type.Name;
		}
	}
	class DrawItemListBox : ListBox
	{
		System.Drawing.SolidBrush m_brushLine, m_brushBlack, m_brushWhite, m_brushBKSelected;
		System.Drawing.Pen m_penLine;
		System.Drawing.Font m_font;
		public DrawItemListBox()
		{
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			ItemHeight = 26;
			//
			m_brushLine = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);
			m_brushBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
			m_brushWhite = new System.Drawing.SolidBrush(System.Drawing.Color.White);
			m_brushBKSelected = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue);
			m_penLine = new System.Drawing.Pen(m_brushLine, 1);
			m_font = new System.Drawing.Font("Times New Roman", 8);
		}
		public void AddDrawItem(Type itemType)
		{
			Items.Add(new DrawItemListItem(itemType));
		}
		public void UseSubset(Type[] types)
		{
			if (types != null)
			{
				int i = 0;
				while (i < this.Items.Count)
				{
					bool b = true;
					DrawItemListItem t = this.Items[i] as DrawItemListItem;
					if (t != null)
					{
						for (int j = 0; j < types.Length; j++)
						{
							if (t.ItemType.Equals(types[j]))
							{
								b = false;
								break;
							}
						}
					}
					if (b)
					{
						this.Items.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < Items.Count)
			{
				System.Drawing.Rectangle rc = e.Bounds;
				rc.X = e.Bounds.Left + 2;
				rc.Y = e.Bounds.Top + 2;
				DrawItemListItem tp = (DrawItemListItem)Items[e.Index];
				Image img = tp.ItemImage;
				if ((e.State & DrawItemState.Selected) != 0)
				{
					//fill background
					e.Graphics.FillRectangle(m_brushBKSelected, e.Bounds);
					//draw image
					e.Graphics.DrawImage(img, rc.Left, rc.Top);
					//write name
					rc.X = rc.Left + img.Width + 2;
					e.Graphics.DrawString(tp.ToString(), m_font, m_brushWhite, rc);
				}
				else
				{
					//fill name background
					e.Graphics.FillRectangle(m_brushWhite, e.Bounds);
					//draw image
					e.Graphics.DrawImage(img, rc.Left, rc.Top);
					//write name
					rc.X = rc.Left + img.Width + 2;
					e.Graphics.DrawString(tp.ToString(), m_font, m_brushBlack, rc);
				}
				//draw name box
				e.Graphics.DrawRectangle(m_penLine, e.Bounds);
			}
		}
	}
}
