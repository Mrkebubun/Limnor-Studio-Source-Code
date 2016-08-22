/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using VSPrj;
using System.Drawing;

namespace SolutionMan
{
	class TypeEditorTargetFramework : UITypeEditor
	{
		public TypeEditorTargetFramework()
		{
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			Font ft = new Font("Times New Roman", e.Bounds.Height, GraphicsUnit.Pixel);
			AssemblyTargetFramework tf = (AssemblyTargetFramework)e.Value;
			if (tf == AssemblyTargetFramework.V35)
			{
				e.Graphics.DrawString(".Net Framework 3.5", ft, Brushes.Black, (float)0, (float)0);
			}
			else if (tf == AssemblyTargetFramework.V40)
			{
				e.Graphics.DrawString(".Net Framework 4.0", ft, Brushes.Black, (float)0, (float)0);
			}
			base.PaintValue(e);
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
					object[] vs = new object[2];
					vs[0] = ".Net Framework 3.5";
					vs[1] = ".Net Framework 4.0";
					ValueList list = new ValueList(edSvc, vs);
					if (value != null)
					{
						try
						{
							AssemblyTargetFramework tf = (AssemblyTargetFramework)value;
							if (tf == AssemblyTargetFramework.V35)
							{
								list.SelectedIndex = 0;
							}
							else if (tf == AssemblyTargetFramework.V40)
							{
								list.SelectedIndex = 1;
							}
						}
						catch
						{
						}
					}
					edSvc.DropDownControl(list);
					if (list.MadeSelection)
					{
						if (list.Selection == 0)
							value = AssemblyTargetFramework.V35;
						else
							value = AssemblyTargetFramework.V40;
					}
				}
			}
			return value;
		}
		class ValueList : ListBox
		{
			public bool MadeSelection;
			public int Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, object[] values)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
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
