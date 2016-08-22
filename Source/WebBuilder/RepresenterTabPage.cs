/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace Limnor.WebBuilder
{
	public class RepresenterTabPage : PropertyRepresenter
	{
		static StringCollection _props;
		private TabPage _page;
		public RepresenterTabPage(TabPage page)
			: base(page)
		{
			_page = page;
			
		}
		static RepresenterTabPage()
		{
			_props = new StringCollection();
			_props.Add("BackColor");
			_props.Add("Name");
			_props.Add("Text");
		}
		public TabPage Page
		{
			get
			{
				return _page;
			}
		}
		#region PropertyRepresenter members
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps0 = TypeDescriptor.GetProperties(this.Object);
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps0)
			{
				if (_props.Contains(p.Name))
				{
					lst.Add(p);
				}
			}
			PropertyDescriptorCollection ps = new PropertyDescriptorCollection(lst.ToArray());
			return ps;
		}
		#endregion
	}
}
