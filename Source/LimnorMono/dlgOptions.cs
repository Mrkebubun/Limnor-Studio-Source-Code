/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VOB;

namespace LimnorVOB
{
	public partial class dlgOptions : Form
	{
		public dlgOptions()
		{
			InitializeComponent();
			propertyGrid1.SelectedObject = new VobOptions();
		}
	}
}