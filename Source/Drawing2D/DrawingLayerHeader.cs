/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Limnor.Drawing2D
{
	[TypeConverter(typeof(TypeConverterLayerHead))]
	[Serializable]
	public class DrawingLayerHeader
	{
		public DrawingLayerHeader()
		{
			Visible = true;
		}
		public DrawingLayerHeader(DrawingLayer layer)
		{
			LayerId = layer.LayerId;
			Name = layer.LayerName;
			Visible = layer.Visible;
		}
		public DrawingLayerHeader(Guid id, bool visible, string name)
		{
			LayerId = id;
			Visible = visible;
			Name = name;
		}
		public Guid LayerId
		{
			get;
			set;
		}
		public string Name
		{
			get;
			set;
		}
		[DefaultValue(true)]
		public bool Visible
		{
			get;
			set;
		}
	}
	[TypeConverter(typeof(TypeConverterLayerHeaderList))]
	[Serializable]
	public class DrawingLayerHeaderList : List<DrawingLayerHeader>
	{
		public DrawingLayerHeaderList()
		{
		}
	}
}
