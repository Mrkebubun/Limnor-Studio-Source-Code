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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using TraceLog;

namespace MathExp
{
	public partial class TypeIcons : UserControl
	{
		public event EventHandler OnTypeSelected;
		public Label lblTooltips;
		public enumOperatorCategory Category;
		public TypeIcons()
		{
			InitializeComponent();
		}
		public void SetIconVisible(Type t, bool visible)
		{
			for (int i = 0; i < this.Controls.Count; i++)
			{
				iconLabel l = this.Controls[i] as iconLabel;
				if (l != null)
				{
					if (t.Equals(l.Tag))
					{
						l.Visible = visible;
						break;
					}
				}
			}
		}

		class iconLabel : Label
		{
			public bool Selected;
			public iconLabel()
			{
			}
			protected override void OnMouseEnter(EventArgs e)
			{
				base.OnMouseEnter(e);
				for (int i = 0; i < this.Parent.Controls.Count; i++)
				{
					iconLabel l = this.Parent.Controls[i] as iconLabel;
					if (l != null)
					{
						l.BorderStyle = BorderStyle.None;
						l.Selected = false;
					}
				}
				this.BorderStyle = BorderStyle.Fixed3D;
				this.Selected = true;
			}
			protected override void OnMouseLeave(EventArgs e)
			{
				base.OnMouseLeave(e);
				this.Selected = false;
				this.BorderStyle = BorderStyle.None;
				TypeIcons p = this.Parent as TypeIcons;
				if (p != null && p.lblTooltips != null)
				{
					p.lblTooltips.Visible = false;
				}
				this.Refresh();
			}
			protected override void OnMouseHover(EventArgs e)
			{
				base.OnMouseHover(e);
				bool b = false;
				TypeIcons p = this.Parent as TypeIcons;
				if (p != null && p.lblTooltips != null)
				{
					Type t = this.Tag as Type;
					if (t != null)
					{
						DescriptionAttribute desc = TypeDescriptor.GetAttributes(t)[typeof(DescriptionAttribute)] as DescriptionAttribute;
						if (desc != null)
						{
							if (!string.IsNullOrEmpty(desc.Description))
							{
								p.lblTooltips.Text = desc.Description;
								p.lblTooltips.Left = (p.lblTooltips.Parent.ClientSize.Width - p.lblTooltips.Width) / 2;
								b = true;
							}
						}
					}
					p.lblTooltips.Visible = b;
				}
			}
			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				if (this.Selected)
				{
					e.Graphics.DrawRectangle(new Pen(Brushes.Yellow, 3), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
				}
			}
		}
		private iconLabel createIcon(Type type)
		{
			Bitmap bmp = null;
			iconLabel icon = new iconLabel();
			icon.Text = "";
			icon.Tag = type;
			icon.ImageAlign = ContentAlignment.MiddleCenter;
			System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(type)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
			if (tba != null)
			{
				bmp = (System.Drawing.Bitmap)tba.GetImage(type);
			}
			if (bmp != null)
			{
				icon.Image = bmp;
			}
			else
			{
				icon.Text = type.Name;
			}
			icon.Size = new Size(22, 22);
			icon.Click += new EventHandler(TypeIcons_Click);
			return icon;
		}
		public bool Contains(Type type)
		{
			for (int i = 0; i < Controls.Count; i++)
			{
				iconLabel icon = Controls[i] as iconLabel;
				if (icon != null)
				{
					Type t = icon.Tag as Type;
					if (t.Equals(type))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void AddIcon(Type type)
		{
			this.Controls.Add(createIcon(type));
			TypeIcons_Resize(null, null);
		}
		public void LoadData(List<Type> types)
		{
			_types = types;
			_iconsLoaded = false;
		}
		private bool _iconsLoaded = false;
		private List<Type> _types = null;
		public void LoadIcons()
		{
			if (!_iconsLoaded)
			{
				_iconsLoaded = true;
				Graphics g = this.CreateGraphics();
				g.DrawString("Loading operators, please wait ,,,", this.Font, Brushes.Red, (float)10, (float)10);
				g.Dispose();
				iconLabel[] lbls = new iconLabel[_types.Count];
				for (int i = 0; i < _types.Count; i++)
				{
					Type type = _types[i] as Type;
					lbls[i] = createIcon(type);
				}
				this.Controls.AddRange(lbls);
				TypeIcons_Resize(null, null);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (lblTooltips != null)
			{
				lblTooltips.Visible = false;
			}
		}
		void TypeIcons_Click(object sender, EventArgs e)
		{
			Control c = sender as Control;
			if (c != null)
			{
				Type t = c.Tag as Type;
				if (t != null)
				{
					if (OnTypeSelected != null)
					{
						OnTypeSelected(t, new EventArgsEletementSelect(Category));
					}
				}
			}
		}

		private void TypeIcons_Resize(object sender, EventArgs e)
		{
			if (Controls.Count > 0)
			{
				int cols = this.ClientSize.Width / Controls[0].Width;
				int r = 0;
				int x = 0, y = 0;
				while (r < Controls.Count)
				{
					for (int i = 0; i < cols && r < Controls.Count; i++, r++)
					{
						Controls[r].Location = new Point(x, y);
						x += Controls[0].Width;
					}
					y += Controls[0].Height;
					x = 0;
				}
			}
		}

	}
	class EventArgsEletementSelect : EventArgs
	{
		private enumOperatorCategory _cat;
		public EventArgsEletementSelect(enumOperatorCategory category)
		{
			_cat = category;
		}
		public enumOperatorCategory Category
		{
			get
			{
				return _cat;
			}
		}
	}
}
