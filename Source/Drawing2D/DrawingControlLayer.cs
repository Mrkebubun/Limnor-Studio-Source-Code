/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Drawing2D
{
	class DrawingControlLayer : List<IDrawDesignControl>
	{
		public DrawingControlLayer()
		{
		}
		public DrawingControlLayer(DrawingLayerHeader header)
		{
			LayerId = header.LayerId;
		}
		public Guid LayerId { get; set; }
	}
	class DrawingControlLayerList : List<DrawingControlLayer>
	{
		public DrawingControlLayerList()
		{
		}
		public void AddControl(IDrawDesignControl dc)
		{
			if (this.Count == 0)
			{
				Add(new DrawingControlLayer());
			}
			DrawingControlLayer l = null;
			for (int i = 1; i < this.Count; i++)
			{
				if (this[i].LayerId == dc.LayerId)
				{
					l = this[i];
					break;
				}
			}
			if (l == null)
			{
				l = this[0];
			}
			l.Add(dc);
		}
	}
}
