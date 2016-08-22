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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Globalization;

namespace VPL
{
	public class TypeSelectorEditTypedNamedValue : UITypeEditor
	{
		public TypeSelectorEditTypedNamedValue()
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
					ITypedNamedValuesHolder vep = context.Instance as ITypedNamedValuesHolder;
					if (vep != null)
					{
						CommandList list = new CommandList(edSvc);
						edSvc.DropDownControl(list);
						if (list.MadeSelection)
						{
							bool edited = false;
							switch (list.Selection)
							{
								case 0:
									DlgNewTypedNamedValue dlg = new DlgNewTypedNamedValue();
									TypedNamedValue tn = vep.GetTypedNamedValueByName(context.PropertyDescriptor.Name);
									dlg.LoadData(vep.GetValueNames(), "Modify value name and type", tn);
									if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
									{
										edited = vep.RenameTypedNamedValue(context.PropertyDescriptor.Name, dlg.DataName, dlg.DataType);
									}
									break;
								case 1:
									DlgText dlgt = new DlgText();
									dlgt.LoadData(value as string, string.Format(CultureInfo.InvariantCulture, "Modify {0}", context.PropertyDescriptor.Name));
									if (edSvc.ShowDialog(dlgt) == DialogResult.OK)
									{
										value = dlgt.GetText();
									}
									break;
								case 2:
									edited = vep.DeleteTypedNamedValue(context.PropertyDescriptor.Name);
									break;
							}
							if (edited)
							{
								PropertyGrid pg = null;
								Type t = edSvc.GetType();
								PropertyInfo pif0 = t.GetProperty("OwnerGrid");
								if (pif0 != null)
								{
									object g = pif0.GetValue(edSvc, null);
									pg = g as PropertyGrid;
									if (pg != null)
									{
										pg.Refresh();
									}
								}
								PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(context.Instance, true);
								PropertyDescriptor p = ps["Dummy"];
								if (p != null)
								{
									p.SetValue(context.Instance, Guid.NewGuid().GetHashCode());
								}
								IPropertyListchangeable plc = context.Instance as IPropertyListchangeable;
								if (plc != null)
								{
									plc.OnPropertyListChanged();
								}
							}
						}
					}
				}
			}
			return value;
		}
		class CommandList : ListBox
		{
			public bool MadeSelection;
			public int Selection;
			private IWindowsFormsEditorService _service;
			public CommandList(IWindowsFormsEditorService service)
			{
				_service = service;
				this.Items.Add("Rename");
				this.Items.Add("Edit value");
				this.Items.Add("Delete");
				Graphics g = this.CreateGraphics();
				SizeF sf = g.MeasureString("R", this.Font);
				g.Dispose();
				this.ItemHeight = (int)(sf.Height) + 2;
				this.DrawMode = DrawMode.OwnerDrawFixed;
			}
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				double dn = (double)(e.Y) / (double)(this.ItemHeight);
				int n = (int)dn;
				if (n >= this.Items.Count)
				{
					n = -1;
				}
				this.SelectedIndex = n;
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				e.DrawBackground();
				if (e.Index >= 0)
				{
					if ((e.State & DrawItemState.Selected) != 0)
					{
						e.DrawFocusRectangle();
					}
					System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
					if (e.Index == 0)
					{
						e.Graphics.DrawImage(Resource1._rename.ToBitmap(), rc.X, rc.Y);
					}
					else if (e.Index == 1)
					{
						e.Graphics.DrawImage(Resource1.property, rc.X, rc.Y);
					}
					else if (e.Index == 2)
					{
						e.Graphics.DrawImage(Resource1._cancel.ToBitmap(), rc.X, rc.Y);
					}
					rc.X += 16;
					if (rc.Width > 16)
					{
						rc.Width -= 16;
					}
					e.Graphics.DrawString(Items[e.Index] as string, this.Font, Brushes.Black, rc);
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = SelectedIndex;
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
