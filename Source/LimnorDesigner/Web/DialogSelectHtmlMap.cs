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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Limnor.Drawing2D;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.Web
{
	public partial class DialogSelectHtmlMap : Form
	{
		public string MapID;
		private WebPage _webpage;
		private HtmlElement_map _currentMap;
		public DialogSelectHtmlMap()
		{
			InitializeComponent();
		}
		public void LoadData(Image bkImg, WebPage page, string mapId)
		{
			_webpage = page;
			mapCtrl.DisableRotation = true;
			mapCtrl.ImgBK = bkImg;
			mapCtrl.ShowButtons(new int[] { 1, 2, 3, 11 });
			mapCtrl.ResetToolPositions();
			mapCtrl.UseSubset(new Type[] { typeof(DrawCircle), typeof(DrawRect), typeof(DrawPolygon) });
			IList<HtmlElement_map> maps = page.GetMaps();
			for (int i = 0; i < maps.Count; i++)
			{
				int n = comboBox1.Items.Add(maps[i]);
				if (!string.IsNullOrEmpty(mapId) && string.CompareOrdinal(maps[i].id, mapId) == 0)
				{
					comboBox1.SelectedIndex = n;
				}
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			mapCtrl.Refresh();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_currentMap != null)
			{
				List<HtmlElement_area> areas = new List<HtmlElement_area>();
				List<DrawingItem> lst = mapCtrl.ExportDrawingItems();
				if (lst != null && lst.Count > 0)
				{
					foreach (DrawingItem di in lst)
					{
						HtmlElement_area ha = null;
						ClassPointer root = _webpage.GetDevClass() as ClassPointer;
						DrawCircle dc = di as DrawCircle;
						if (dc != null)
						{
							ha = new HtmlElement_area(root);
							ha.shape = EnumAreaShape.circle;
							ha.coords = new int[3];
							ha.coords[0] = dc.CircleCenter.X;
							ha.coords[1] = dc.CircleCenter.Y;
							ha.coords[2] = dc.Radius;
						}
						else
						{
							DrawRect dr = di as DrawRect;
							if (dr != null)
							{
								ha = new HtmlElement_area(root);
								ha.shape = EnumAreaShape.rect;
								ha.coords = new int[4];
								ha.coords[0] = dr.Rectangle.X;
								ha.coords[1] = dr.Rectangle.Y;
								ha.coords[2] = dr.Rectangle.X + dr.Rectangle.Width;
								ha.coords[3] = dr.Rectangle.Y + dr.Rectangle.Height;
							}
							else
							{
								DrawPolygon dp = di as DrawPolygon;
								if (dp != null)
								{
									ha = new HtmlElement_area(root);
									ha.shape = EnumAreaShape.poly;
									ha.coords = new int[dp.PointCount * 2];
									for (int k = 0; k < dp.PointCount; k++)
									{
										ha.coords[2 * k] = dp.PointList[k].X;
										ha.coords[2 * k + 1] = dp.PointList[k].Y;
									}
								}
							}
						}
						if (ha != null)
						{
							ha.SetGuid(di.DrawingId);
							ha.SetId(di.Name);
							areas.Add(ha);
						}
					}
				}
				_currentMap.SetAreas(areas);
				_webpage.UpdateMapAreas(_currentMap);
				MapID = _currentMap.id;
				this.DialogResult = DialogResult.OK;
			}
		}

		private void buttonNewMap_Click(object sender, EventArgs e)
		{
			HtmlElement_map map = _webpage.CreateNewMap();
			if (map != null)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "A new map is created and named '{0}'", map.id), "Create map", MessageBoxButtons.OK, MessageBoxIcon.Information);
				int n = comboBox1.Items.Add(map);
				comboBox1.SelectedIndex = n;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex >= 0)
			{
				HtmlElement_map map = comboBox1.Items[comboBox1.SelectedIndex] as HtmlElement_map;
				onSelectMap(map);
			}
		}
		private void onSelectMap(HtmlElement_map map)
		{
			if (map != null && map != _currentMap)
			{
				_currentMap = map;
				if (map.Areas == null)
				{
					List<HtmlElement_area> areas = _webpage.GetAreas(map.id);
					map.SetAreas(areas);
				}
				mapCtrl.ReloadShapes(map.ExportShapes());
			}
		}
	}
}
