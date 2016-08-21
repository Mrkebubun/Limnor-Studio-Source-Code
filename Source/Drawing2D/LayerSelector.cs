/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;

namespace Limnor.Drawing2D
{
	class LayerSelector : UITypeEditor
	{
		public LayerSelector()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					DrawingItem item = context.Instance as DrawingItem;
					if (item == null)
					{
						IDrawDesignControl dc = context.Instance as IDrawDesignControl;
						if (dc != null)
						{
							item = dc.Item;
						}
					}
					if (item != null && item.Page != null)
					{
						ListBox list = new ListBox();
						foreach (DrawingLayer l in item.Page.DrawingLayers)
						{
							list.Items.Add(l);
						}
						list.Tag = service;
						list.Click += new EventHandler(list_Click);
						service.DropDownControl(list);
						if (list.SelectedIndex >= 0)
						{
							DrawingLayer ly = list.Items[list.SelectedIndex] as DrawingLayer;
							item.Page.MoveItemToLayer(item, ly);
							value = ly.LayerId.ToString();
							Type t = service.GetType();
							PropertyInfo pif0 = t.GetProperty("OwnerGrid");
							if (pif0 != null)
							{
								PropertyGrid pg = pif0.GetValue(service, null) as PropertyGrid;
								if (pg != null)
								{
									pg.Refresh();
								}
							}
						}
					}
				}
			}
			return value;
		}
		void list_Click(object sender, EventArgs e)
		{
			ListBox list = sender as ListBox;
			if (list.SelectedIndex >= 0)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)(list.Tag);
				service.CloseDropDown();
			}
		}
	}
}
