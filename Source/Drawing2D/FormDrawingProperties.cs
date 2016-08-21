/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
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
using System.Drawing.Design;
using VPL;

namespace Limnor.Drawing2D
{
	public partial class FormDrawingsProperties : Form
	{
		private dlgDrawings _page;
		private LayerHolder _holder;
		public FormDrawingsProperties()
		{
			InitializeComponent();
			propertyGrid2.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid2_SelectedGridItemChanged);
			propertyGrid2.PropertySort = PropertySort.NoSort;

			//
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
		}

		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			_page.Refresh();
			DrawingItem item = propertyGrid1.SelectedObject as DrawingItem;
			if (item != null)
			{
				item.Refresh();
			}
		}
		public void SelectItem(DrawingItem item)
		{
			propertyGrid1.SelectedObject = item;
			VPLUtil.AdjustImagePropertyAttribute(item);
			if (item != null)
			{
				Text = "Drawing item: " + item.Name;
			}
			else
			{
				Text = "Drawing item:";
			}
			propertyGrid1.Refresh();
		}
		internal void SetPage(dlgDrawings page)
		{
			_page = page;
			_holder = new LayerHolder(page);
			propertyGrid2.SelectedObject = _holder;
		}
		public void RefreshPropertyGrids()
		{
			propertyGrid2.Refresh();
			propertyGrid1.Refresh();
		}
		void propertyGrid2_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
		{
			DrawingItem item = e.NewSelection.Value as DrawingItem;
			if (item != null)
			{
				SelectItem(item);
				_page.SetItemSelection(item);
			}
		}
		#region LayerHolder class
		class LayerHolder : IDrawingPageHolder
		{
			private DrawingPage _page;
			public LayerHolder(DrawingPage page)
			{
				_page = page;
			}
			[Editor(typeof(CollectionEditorDrawingLayer), typeof(UITypeEditor))]
			public DrawingLayerCollection DrawingLayers
			{
				get
				{
					return _page.DrawingLayers;
				}
			}

			#region IDrawingPageHolder Members

			public IDrawingPage GetDrawingPage()
			{
				return _page;
			}

			#endregion
		}
		#endregion
	}

}
