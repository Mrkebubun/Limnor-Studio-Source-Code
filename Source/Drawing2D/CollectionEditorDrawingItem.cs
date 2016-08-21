
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
using System.ComponentModel;

namespace Limnor.Drawing2D
{
	public interface IDrawingOwnerPointer
	{
		void OnDrawingCollectionChanged(DrawingLayer layer);
	}
	public class CollectionEditorDrawingItem : CollectionEditor
	{
		public CollectionEditorDrawingItem(Type type)
			: base(type)
		{
		}
		protected override object CreateInstance(Type itemType)
		{
			dlgNewDrawing dlg = new dlgNewDrawing();
			dlg.LoadData(false);
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				DrawingLayerCollection ls = this.Context.Instance as DrawingLayerCollection;
				if (ls != null)
				{
					ls.SetNewName(dlg.objRet);
					dlg.objRet.Page = (DrawingPage)ls.Page;
				}
				return dlg.objRet;
			}
			return null;
		}
		protected override bool CanRemoveInstance(object value)
		{
			return false;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object v = base.EditValue(context, provider, value);
			DrawingLayer layer = value as DrawingLayer;
			DrawingLayerCollection ls = context.Instance as DrawingLayerCollection;
			if (layer != null && ls != null)
			{
				foreach (DrawingItem item in layer)
				{
					item.LayerId = layer.LayerId;
				}
				IDrawingOwnerPointer iw = ls.Page.DesignerPointer;
				if (iw != null)
				{
					iw.OnDrawingCollectionChanged(layer);
				}
			}
			if (provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					Type t = edSvc.GetType();
					PropertyInfo pif0 = t.GetProperty("OwnerGrid");
					if (pif0 != null)
					{
						object g = pif0.GetValue(edSvc, null);
						PropertyGrid pg = g as PropertyGrid;
						if (pg != null)
						{
							pg.Refresh();
						}
					}
				}
			}
			return v;
		}
	}
}
