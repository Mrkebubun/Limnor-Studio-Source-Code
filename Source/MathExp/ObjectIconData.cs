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
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace MathExp
{
	public enum EnumIconType { IconImage, NameText, ItemImage, ShortDescription }
	/// <summary>
	/// describes how to draw an icon for math expression group
	/// </summary>
	[TypeConverter(typeof(PropertySorter))]
	[ExtenderProvidedProperty]
	public class ObjectIconData : ICloneable
	{
		public ObjectIconData()
		{
		}
		public override string ToString()
		{
			return _iconType.ToString();
		}
		#region Properties
		private Size _size = new Size(32, 32);
		[PropertyOrder(10)]
		public Size IconSize
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}
		private EnumIconType _iconType = EnumIconType.ItemImage;
		[PropertyOrder(20)]
		public EnumIconType IconType
		{
			get
			{
				return _iconType;
			}
			set
			{
				_iconType = value;
			}
		}
		private Image _icon;
		[PropertyOrder(30)]
		public Image IconImage
		{
			get
			{
				return _icon;
			}
			set
			{
				_icon = value;
			}
		}
		private DrawTextAttributes _textAttributes;
		[PropertyOrder(50)]
		public DrawTextAttributes TextAttributes
		{
			get
			{
				if (_textAttributes == null)
					_textAttributes = new DrawTextAttributes();
				return _textAttributes;
			}
			set
			{
				_textAttributes = value;
			}
		}
		#endregion
		#region Serialization
		public void SaveToXmlNode(XmlNode node)
		{
			XmlNode nd = node.OwnerDocument.CreateElement("IconType");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.IconType);
			nd = node.OwnerDocument.CreateElement("IconSize");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.IconSize);
			if (_icon != null)
			{
				nd = node.OwnerDocument.CreateElement("IconImage");
				node.AppendChild(nd);
				XmlSerialization.WriteValue(nd, this._icon);
			}
			nd = node.OwnerDocument.CreateElement("TextAttributes");
			node.AppendChild(nd);
			this.TextAttributes.SaveToXmlNode(nd);
		}
		public void LoadFromXmlNode(XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode("IconType");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.IconType = (EnumIconType)v;
				}
			}
			nd = node.SelectSingleNode("IconSize");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.IconSize = (Size)v;
				}
			}
			nd = node.SelectSingleNode("IconImage");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.IconImage = (Image)v;
				}
			}
			nd = node.SelectSingleNode("TextAttributes");
			if (nd != null)
			{
				this.TextAttributes.LoadFromXmlNode(nd);
			}
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ObjectIconData obj = new ObjectIconData();
			obj.IconSize = IconSize;
			obj.IconType = IconType;
			if (_icon != null)
				obj.IconImage = (Image)_icon.Clone();
			if (_textAttributes != null)
			{
				obj.TextAttributes = (DrawTextAttributes)_textAttributes.Clone();
			}
			return obj;
		}

		#endregion
	}
}
