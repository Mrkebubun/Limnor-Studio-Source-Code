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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MathExp
{
	public partial class dlgValue : Form, IDataSelectionControl
	{
		private PropertyBag propertyBag;
		public dlgValue()
		{
			InitializeComponent();
		}
		protected override void OnResize(EventArgs e)
		{
			propertyGrid1.Width = this.ClientSize.Width;
			if (this.ClientSize.Height > propertyGrid1.Top)
			{
				propertyGrid1.Height = this.ClientSize.Height - propertyGrid1.Top;
			}
		}
		public void SetProperty(PropertySpec spec)
		{
			propertyBag = new PropertyBag();
			propertyBag.GetValue += new PropertySpecEventHandler(propertyBag_GetValue);
			propertyBag.SetValue += new PropertySpecEventHandler(propertyBag_SetValue);
			propertyBag.Properties.Add(spec);
			propertyGrid1.SelectedObject = propertyBag;
		}
		object val;
		void propertyBag_SetValue(object sender, PropertySpecEventArgs e)
		{
			val = e.Value;
		}

		void propertyBag_GetValue(object sender, PropertySpecEventArgs e)
		{
			e.Value = val;
		}
		public object ReturnValue
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
			}
		}

		#region IDataSelectionControl Members

		public object UITypeEditorSelectedValue
		{
			get { return val; }
		}
		public void SetCaller(System.Windows.Forms.Design.IWindowsFormsEditorService wfe)
		{
		}

		#endregion
	}
}