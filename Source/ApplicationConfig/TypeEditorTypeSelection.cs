/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.Application
{
	class TypeEditorTypeSelection : UITypeEditor
	{
		public TypeEditorTypeSelection()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				edSvc.CloseDropDown();
				TypeListBox lst = new TypeListBox(edSvc);
				Type t = value as Type;
				if (t != null)
				{
					lst.SelectType(t);
				}
				edSvc.DropDownControl(lst);
				value = lst.SelectedType;
			}
			return value;
		}
		class TypeDesc
		{
			private Type _type;
			private string _desc;
			public TypeDesc(Type type, string description)
			{
				_type = type;
				_desc = description;
			}
			public Type Type
			{
				get
				{
					return _type;
				}
			}
			public override string ToString()
			{
				return _desc;
			}
		}
		class TypeListBox : ListBox
		{
			private IWindowsFormsEditorService _service;
			private Type _selected;
			public TypeListBox(IWindowsFormsEditorService service)
			{
				_service = service;
				IntegralHeight = false;
				Items.Add(new TypeDesc(typeof(byte), "Byte"));
				Items.Add(new TypeDesc(typeof(sbyte), "Signed Byte"));
				Items.Add(new TypeDesc(typeof(bool), "Boolean"));
				Items.Add(new TypeDesc(typeof(Int16), "Integer(16-bit)"));
				Items.Add(new TypeDesc(typeof(Int32), "Integer(32-bit)"));
				Items.Add(new TypeDesc(typeof(Int64), "Integer(64-bit)"));
				Items.Add(new TypeDesc(typeof(char), "One character"));
				Items.Add(new TypeDesc(typeof(string), "Text(a string of characters)"));
				Items.Add(new TypeDesc(typeof(float), "Decimal number(singlle precision 32-bit)"));
				Items.Add(new TypeDesc(typeof(double), "Decimal number(double precision 64-bit)"));
				Items.Add(new TypeDesc(typeof(decimal), "Decimal number(high precision 128-bit)"));
				Items.Add(new TypeDesc(typeof(UInt16), "Usigned Integer(16-bit)"));
				Items.Add(new TypeDesc(typeof(UInt32), "Usigned Integer(32-bit)"));
				Items.Add(new TypeDesc(typeof(UInt64), "Usigned Integer(64-bit)"));
				Items.Add(new TypeDesc(typeof(DateTime), "DateTime"));
				Items.Add(new TypeDesc(typeof(TimeSpan), "TimeSpan"));
				Items.Add(new TypeDesc(typeof(Color), "Color"));
				Items.Add(new TypeDesc(typeof(Font), "Font"));
				Items.Add(new TypeDesc(typeof(Size), "Size"));
				Items.Add(new TypeDesc(typeof(Point), "Point"));
				Items.Add(new TypeDesc(typeof(Rectangle), "Rectangle"));
			}
			public Type SelectedType
			{
				get
				{
					return _selected;
				}
			}
			public void SelectType(Type type)
			{
				for (int i = 0; i < Items.Count; i++)
				{
					if (((TypeDesc)Items[i]).Type.Equals(type))
					{
						SelectedIndex = i;
						break;
					}
				}
			}
			protected override void OnSelectedIndexChanged(EventArgs e)
			{
				base.OnSelectedIndexChanged(e);
				if (SelectedIndex >= 0)
				{
					_selected = ((TypeDesc)Items[SelectedIndex]).Type;
				}
			}
			protected override void OnMouseClick(MouseEventArgs e)
			{
				base.OnMouseClick(e);
				_service.CloseDropDown();
			}
		}
	}
}
