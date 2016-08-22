/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using LFilePath;
using System.Globalization;
using System.Drawing;
using System.IO;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class HtmlMenuItem
	{
		#region fields and constructors
		private HtmlMenuItem _parent;
		private HtmlMenuItemCollection _items;
		private HtmlMenu _menu;
		private Image _img;
		private string _imagePath;
		private Guid _guid;
		public HtmlMenuItem()
		{
			Text = "New menu";
		}
		public HtmlMenuItem(HtmlMenuItem parent, HtmlMenu menu)
		{
			Text = "New menu";
			_parent = parent;
			_menu = menu;
		}
		#endregion
		#region Methods
		public bool IsChild(HtmlMenuItem c)
		{
			if (_items != null)
			{
				for (int i = 0; i < _items.Count; i++)
				{
					if (c.MenuID == _items[i].MenuID)
					{
						return true;
					}
					if (_items[i].IsChild(c))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void SetParent(HtmlMenuItem parent, HtmlMenu menu)
		{
			_parent = parent;
			_menu = menu;
			if (_items != null)
			{
				_items.SetOwner(_menu, this);
			}
		}
		public int Paint(Graphics g)
		{
			int w = 0;
			if (_img != null)
			{
				g.DrawImage(_img, new Point(0, 0));
				w = _img.Width + 1;
			}
			if (!string.IsNullOrEmpty(Text))
			{
				SizeF sf;
				sf = g.MeasureString(Text, _menu.Font);
				SolidBrush br = new SolidBrush(_menu.ForeColor);
				g.DrawString(Text, _menu.Font, br, (float)w, (float)1);
				w += (int)sf.Width;
				w++;
			}
			return w;
		}
		public void add_Click(SimpleCall handler)
		{
		}
		public void raiseClick()
		{
		}
		public void remove_Click(SimpleCall handler)
		{
		}
		public override string ToString()
		{
			return Text;
		}
		#endregion
		#region Properties
		[Description("The unique id for the menu item")]
		public string id
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "e{0}", MenuID.GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			}
		}
		[Browsable(false)]
		public Guid MenuID
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}
		[Description("Gets and sets menu item caption")]
		public string Text { get; set; }

		[FilePath("Image files|*.jpg;*.gif;*.png;*.bmp", "Select menu item image")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Description("Gets and sets menu item image path")]
		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_img = null;
				_imagePath = value;
				if (!string.IsNullOrEmpty(_imagePath))
				{
					if (File.Exists(_imagePath))
					{
						try
						{
							_img = Image.FromFile(_imagePath);
						}
						catch
						{
						}
					}
				}
			}
		}
		[Description("Gets and sets menu items")]
		public HtmlMenuItemCollection MenuItems
		{
			get
			{
				if (_items == null)
				{
					_items = new HtmlMenuItemCollection();
					_items.SetOwner(_menu, this);
				}
				return _items;
			}
			set
			{
				_items = value;
				_items.SetOwner(_menu, this);
			}
		}
		#endregion
	}
	public class HtmlMenuItemCollection
	{
		#region fields and constructors
		private List<HtmlMenuItem> _items;
		private HtmlMenuItem _parent;
		private HtmlMenu _menu;
		public HtmlMenuItemCollection()
		{
		}
		public void SetOwner(HtmlMenu menu, HtmlMenuItem parent)
		{
			_menu = menu;
			_parent = parent;
			if (_items != null)
			{
				foreach (HtmlMenuItem item in _items)
				{
					item.SetParent(parent, _menu);
				}
			}
		}
		#endregion

		#region Properties
		public int Count
		{
			get
			{
				if (_items == null)
					return 0;
				return _items.Count;
			}
		}
		public HtmlMenuItem this[int index]
		{
			get
			{
				if (_items != null && index >= 0 && index < _items.Count)
				{
					return _items[index];
				}
				return null;
			}
		}
		[Browsable(false)]
		public HtmlMenuItem[] ItemArray
		{
			get
			{
				if (_items != null)
				{
					HtmlMenuItem[] ret = new HtmlMenuItem[_items.Count];
					_items.CopyTo(ret);
					return ret;
				}
				return null;
			}
			set
			{
				if (value == null)
					_items = null;
				else
				{
					for (int i = 0; i < value.Length; i++)
					{
						this.Add(value[i]);
					}
				}
			}
		}
		#endregion

		#region Methods
		public void Clear()
		{
			_items = new List<HtmlMenuItem>();
		}
		public void Add(HtmlMenuItem item)
		{
			if (_items == null)
			{
				_items = new List<HtmlMenuItem>();
			}
			item.SetParent(_parent, _menu);
			_items.Add(item);
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Count:{0}", Count);
		}
		#endregion
	}
}
