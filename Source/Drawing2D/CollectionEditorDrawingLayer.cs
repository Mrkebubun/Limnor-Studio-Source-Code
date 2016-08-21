/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Windows.Forms;
using VPL;

namespace Limnor.Drawing2D
{
	public class CollectionEditorDrawingLayer : CollectionEditorX
	{
		public CollectionEditorDrawingLayer(Type type)
			: base(type)
		{
		}
		protected override object CreateInstance(Type itemType)
		{
			IDrawingPageHolder holder = this.Context.Instance as IDrawingPageHolder;
			if (holder != null)
			{
				IDrawingPage page = holder.GetDrawingPage();
				if (page != null)
				{
					Guid id = page.DrawingLayers.AddLayer();
					return page.DrawingLayers.GetLayerById(id);
				}
			}
			return null;
		}
	}
}
