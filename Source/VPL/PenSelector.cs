/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace VPL
{
	public class PenSelector : UITypeEditor
	{
		Font _font = new Font("Times New Roman", 8);
		public PenSelector()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					DlgSelectPen dlg = new DlgSelectPen();
					Pen p = value as Pen;
					if (p != null)
					{
						dlg.SetData(p);
					}
					PenWrapper pw = value as PenWrapper;
					if (pw != null)
					{
						dlg.SetData(pw.Pen);
					}
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						if (context.PropertyDescriptor.PropertyType.Equals(typeof(Pen)))
						{
							return dlg._pen.Pen;
						}
						return dlg._pen;
					}
				}
			}
			return value;
		}
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			Pen p = e.Value as Pen;
			if (p != null)
			{
				e.Graphics.FillRectangle(new SolidBrush(p.Color), e.Bounds.Left, e.Bounds.Top, 16, 16);
				e.Graphics.DrawString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", p.Width, p.Color.Name), _font, Brushes.Black, (float)(e.Bounds.Left + 20), (float)(e.Bounds.Top + 2));
			}
			else
			{
				PenWrapper pw = e.Value as PenWrapper;
				if (pw != null)
				{
					e.Graphics.FillRectangle(new SolidBrush(pw.Color), e.Bounds.Left, e.Bounds.Top, 16, 16);
				}
				else
				{
					base.PaintValue(e);
				}
			}
		}
	}
}
