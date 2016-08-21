/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MathExp.RaisTypes;
using System.Drawing;
#if DOTNET40
using System.Numerics;
#endif

namespace MathExp
{
	/// <summary>
	/// select primary type
	/// </summary>
	public class PrimaryTypeSelector : ITypeSelector
	{
		public PrimaryTypeSelector()
		{
		}

		#region ITypeSelector Members

		public bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return false;
		}

		public void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
		{
		}

		public IDataSelectionControl GetUIEditorDropdown(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				PrimaryTypeList c = new PrimaryTypeList();
				Type t = value as Type;
				if (t == null)
				{
					RaisDataType rt = value as RaisDataType;
					if (rt != null)
					{
						t = rt.LibType;
					}
				}
				if (t != null)
				{
					c.SelectType(t);
				}
				MathNodeValue mv = context.Instance as MathNodeValue;
				if (mv != null)
				{
					RaisDataType rt = mv.GetRelatedType();
					c.InsertType(rt.LibType, rt.LibType.Name);
				}
				c.SetCaller(edSvc);
				return c;
			}
			return null;
		}

		public IDataSelectionControl GetUIEditorModal(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			return null;
		}

		public System.Drawing.Design.UITypeEditorEditStyle GetUIEditorStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}

		#endregion
	}
	public class PrimaryTypeList : UserControl, IDataSelectionControl
	{
		private ListBox list;
		public PrimaryTypeList()
		{
			list = new ListBox();
			list.Dock = DockStyle.Fill;
			list.Items.Add(new TypeDesc(typeof(byte), "Byte"));
			list.Items.Add(new TypeDesc(typeof(sbyte), "Signed Byte"));
			list.Items.Add(new TypeDesc(typeof(bool), "Boolean"));
			list.Items.Add(new TypeDesc(typeof(Int16), "Integer(16-bit)"));
			list.Items.Add(new TypeDesc(typeof(Int32), "Integer(32-bit)"));
			list.Items.Add(new TypeDesc(typeof(Int64), "Integer(64-bit)"));
#if DOTNET40
			list.Items.Add(new TypeDesc(typeof(BigInteger), "Integer(any size)"));
#endif
			list.Items.Add(new TypeDesc(typeof(string), "Text(string)"));
			list.Items.Add(new TypeDesc(typeof(float), "Decimal number(singlle precision 32-bit)"));
			list.Items.Add(new TypeDesc(typeof(double), "Decimal number(double precision 64-bit)"));
			list.Items.Add(new TypeDesc(typeof(decimal), "Decimal number(high precision 128-bit)"));
			list.Items.Add(new TypeDesc(typeof(UInt16), "Usigned Integer(16-bit)"));
			list.Items.Add(new TypeDesc(typeof(UInt32), "Usigned Integer(32-bit)"));
			list.Items.Add(new TypeDesc(typeof(UInt64), "Usigned Integer(64-bit)"));
			list.Items.Add(new TypeDesc(typeof(char), "One character"));
			list.Items.Add(new TypeDesc(typeof(DateTime), "DateTime"));
			list.Items.Add(new TypeDesc(typeof(TimeSpan), "TimeSpan"));
			list.Items.Add(new TypeDesc(typeof(Color), "Color"));
			//
			list.IntegralHeight = false;
			this.Controls.Add(list);
			list.SelectedIndexChanged += new EventHandler(list_SelectedIndexChanged);
			list.MouseClick += new MouseEventHandler(list_MouseClick);
		}

		void list_MouseClick(object sender, MouseEventArgs e)
		{
			_editorService.CloseDropDown();
		}

		void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (list.SelectedIndex >= 0)
			{
				_selected = list.Items[list.SelectedIndex];

			}
		}
		public void InsertType(Type type, string name)
		{
			bool b = false;
			for (int i = 0; i < list.Items.Count; i++)
			{
				if (((TypeDesc)list.Items[i]).Type.Equals(type))
				{
					b = true;
					break;
				}
			}
			if (!b)
			{
				list.Items.Insert(0, new TypeDesc(type, name));
			}
		}
		public void SelectType(Type type)
		{
			for (int i = 0; i < list.Items.Count; i++)
			{
				if (((TypeDesc)list.Items[i]).Type.Equals(type))
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		#region IDataSelectionControl Members
		private object _selected;
		public object UITypeEditorSelectedValue
		{
			get
			{
				if (_selected != null)
				{
					RaisDataType rt = new RaisDataType();
					rt.LibType = ((TypeDesc)_selected).Type;
					return rt;
				}
				return null;
			}
		}
		private IWindowsFormsEditorService _editorService;
		public void SetCaller(IWindowsFormsEditorService wfe)
		{
			_editorService = wfe;
		}
		#endregion
	}
}
