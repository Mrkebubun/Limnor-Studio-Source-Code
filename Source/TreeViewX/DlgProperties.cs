/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.TreeViewExt
{
	public partial class DlgProperties : Form
	{
		public DlgProperties()
		{
			InitializeComponent();
		}
		public void SetObject(object v, string name)
		{
			Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"Properties - {0}", name);
			propertyGrid1.SelectedObject = v;
		}
	}
}
