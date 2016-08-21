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
using System.Drawing;
using System.Xml;

namespace MathExp
{
	public class DrawTextAttributes : IComponent, ICloneable
	{
		public DrawTextAttributes()
		{
		}
		public override string ToString()
		{
			return this.Font.ToString();
		}
		private Font _font;
		public Font Font
		{
			get
			{
				if (_font == null)
				{
					_font = new Font("Times New Roman", 12);
				}
				return _font;
			}
			set
			{
				_font = value;
			}
		}
		private Color _color = Color.Blue;
		public Color TextColor
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}
		private Color _bkcolor = Color.White;
		public Color BackColor
		{
			get
			{
				return _bkcolor;
			}
			set
			{
				_bkcolor = value;
			}
		}
		public void SaveToXmlNode(XmlNode node)
		{
			XmlNode nd = node.OwnerDocument.CreateElement("Font");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.Font);
			nd = node.OwnerDocument.CreateElement("TextColor");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.TextColor);
			nd = node.OwnerDocument.CreateElement("BackColor");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.BackColor);
		}
		public void LoadFromXmlNode(XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode("Font");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.Font = (Font)v;
				}
			}
			nd = node.SelectSingleNode("TextColor");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.TextColor = (Color)v;
				}
			}
			nd = node.SelectSingleNode("BackColor");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.BackColor = (Color)v;
				}
			}
		}
		#region IComponent Members

		public event EventHandler Disposed;
		ISite _site;
		[Browsable(false)]
		public ISite Site
		{
			get
			{
				if (_site == null)
					_site = new MathSite(this);
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
				Disposed(this, null);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			DrawTextAttributes obj = new DrawTextAttributes();
			obj.Font = (Font)Font.Clone();
			obj.TextColor = TextColor;
			obj.BackColor = BackColor;
			return obj;
		}

		#endregion
	}
}
