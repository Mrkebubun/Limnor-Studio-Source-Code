/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Limnor.WebBuilder;
using Limnor.Drawing2D;
using System.Drawing;

namespace LimnorDesigner.Web
{
	public class HtmlElement_map : HtmlElement_ItemBase
	{
		private List<HtmlElement_area> _areas;
		public HtmlElement_map(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_map(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "map"; }
		}
		[Description("Sets or returns the map name")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string name
		{
			get;
			set;
		}
		public List<HtmlElement_area> Areas
		{
			get
			{
				return _areas;
			}
		}
		public void SetAreas(List<HtmlElement_area> areas)
		{
			_areas = areas;
		}
		public IList<DrawingItem> ExportShapes()
		{
			List<DrawingItem> items = new List<DrawingItem>();
			if (_areas != null)
			{
				foreach (HtmlElement_area hb in _areas)
				{
					DrawingItem di = null;
					switch (hb.shape)
					{
						case EnumAreaShape.circle:
							DrawCircle dc = new DrawCircle();
							if (hb.coords != null && hb.coords.Length > 1)
							{
								dc.CircleCenter = new System.Drawing.Point(hb.coords[0], hb.coords[1]);
								if (hb.coords.Length > 2)
								{
									dc.Radius = hb.coords[2];
								}
								else
								{
									dc.Radius = 20;
								}
							}
							else
							{
								dc.CircleCenter = new System.Drawing.Point(60, 60);
								dc.Radius = 20;
							}
							di = dc;
							break;
						case EnumAreaShape.poly:
							if (hb.coords != null && hb.coords.Length > 1)
							{
								List<CPoint> lst = new List<CPoint>();
								int i = 0;
								while (i < hb.coords.Length - 1)
								{
									lst.Add(new CPoint(new Point(hb.coords[i], hb.coords[i + 1])));
									i += 2;
								}
								if (lst.Count > 3)
								{
									DrawPolygon dp = new DrawPolygon();
									dp.Points = lst;
									di = dp;
								}
							}
							break;
						case EnumAreaShape.rect:
						default:
							if (hb.coords != null && hb.coords.Length > 3)
							{
								if (hb.coords[2] > hb.coords[0] && hb.coords[3] > hb.coords[1])
								{
									Rectangle rc = new Rectangle(hb.coords[0], hb.coords[1], hb.coords[2] - hb.coords[0], hb.coords[3] - hb.coords[1]);
									DrawRect dr = new DrawRect();
									dr.Rectangle = rc;
									di = dr;
								}
							}
							break;
					}
					di.SetGuid(hb.ElementGuid);
					di.Name = hb.id;
					items.Add(di);
				}
			}
			return items;
		}
	}
}
