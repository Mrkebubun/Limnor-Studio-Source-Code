/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using VPL;
using System.ComponentModel;

namespace Limnor.PhpComponents
{
	public class TypeEditorSelectPhpType : UITypeEditor
	{
		public TypeEditorSelectPhpType()
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
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					Type t = value as Type;
					TypeList list = new TypeList(edSvc, t);
					edSvc.DropDownControl(list);
					if (list.MadeSelection)
					{
						value = list.Selection;
					}
				}
			}
			return value;
		}
		class NamedImage
		{
			public NamedImage(string n, Type t, Image i)
			{
				Name = n;
				Image = i;
				Type = t;
			}
			public string Name { get; set; }
			public Image Image { get; set; }
			public Type Type { get; set; }
			public override string ToString()
			{
				return Name;
			}
		}
		class TypeList : ListBox
		{
			public bool MadeSelection;
			public Type Selection;
			private IWindowsFormsEditorService _service;
			public TypeList(IWindowsFormsEditorService service, Type t)
			{
				this.DrawMode = DrawMode.OwnerDrawFixed;
				_service = service;
				Dictionary<string, Type> types = WebPhpData.GetPhpTypes();
				foreach (KeyValuePair<string, Type> kv in types)
				{
					int n = Items.Add(new NamedImage(kv.Key, kv.Value, VPLUtil.GetTypeIcon(kv.Value)));
					if (t != null && t.Equals(kv.Value))
					{
						this.SelectedIndex = n;
					}
				}
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				if (e.Index >= 0 && e.Index < this.Items.Count)
				{
					bool selected = ((e.State & DrawItemState.Selected) != 0);
					NamedImage item = (NamedImage)this.Items[e.Index];
					Rectangle rcBK = new Rectangle(e.Bounds.Left, e.Bounds.Top, 1, this.ItemHeight);
					if (e.Bounds.Width > this.ItemHeight)
					{
						rcBK.Width = e.Bounds.Width - this.ItemHeight;
						rcBK.X = this.ItemHeight;
						if (selected)
						{
							e.Graphics.FillRectangle(Brushes.LightBlue, rcBK);
						}
						else
						{
							e.Graphics.FillRectangle(Brushes.White, rcBK);
						}
					}
					Rectangle rc = new Rectangle(e.Bounds.Left, e.Bounds.Top, this.ItemHeight, this.ItemHeight);
					float w = (float)(e.Bounds.Width - this.ItemHeight);
					if (w > 0)
					{
						RectangleF rcf = new RectangleF((float)(rc.Left + this.ItemHeight + 2), (float)(rc.Top), w, (float)this.ItemHeight);
						if (selected)
						{
							e.Graphics.DrawString(item.Name, this.Font, Brushes.White, rcf);
						}
						else
						{
							e.Graphics.DrawString(item.Name, this.Font, Brushes.Black, rcf);
						}
					}
					e.Graphics.DrawImage(item.Image, rc);
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					NamedImage kv = (NamedImage)Items[SelectedIndex];
					Selection = kv.Type;
				}
				_service.CloseDropDown();
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				finishSelection();

			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					finishSelection();
				}
			}
		}
	}
}
