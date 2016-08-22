/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;

namespace VPL
{
	public interface IControlDesigner
	{
		void LoadControl(IDesignerLoaderHost host);
	}
	/// <summary>
	/// for designing a control
	/// </summary>
	/// <typeparam name="T">it must be a control</typeparam>
	public partial class ControlDesigner<T> : UserControl, ICustomTypeDescriptor
	{
		Control ctrl;
		public ControlDesigner()
		{
			InitializeComponent();
		}
		private void loadControl(IDesignerLoaderHost host)
		{
			ctrl = host.CreateComponent(typeof(T), "A" + Guid.NewGuid().GetHashCode().ToString("x")) as Control;
			ctrl.Location = new Point(0, 0);
			this.Controls.Add(ctrl);
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(ctrl, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(ctrl, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(ctrl, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(ctrl, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(ctrl, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(ctrl, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(ctrl, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(ctrl, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection prop0 = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection prop1 = TypeDescriptor.GetProperties(ctrl, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in prop0)
			{
				if (string.CompareOrdinal(p.Name, "Name") == 0)
				{
					list.Add(p);
					break;
				}
			}
			foreach (PropertyDescriptor p in prop1)
			{
				if (p.Name != "Name" && p.Name != "Controls")
				{
					list.Add(p);
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			if (pd == null)
				return ctrl;
			if (string.CompareOrdinal(pd.Name, "Name") == 0)
				return this;
			return ctrl;
		}

		#endregion
	}
}
