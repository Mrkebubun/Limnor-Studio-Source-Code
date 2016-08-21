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

namespace MathExp
{
	public partial class FormDebug : Form
	{
		public FormDebug()
		{
			InitializeComponent();
		}
		public void SetStr1(object s)
		{
			textBox1.Text = s.ToString();
		}
		public void SetStr2(object s)
		{
			textBox2.Text = s.ToString();
		}
		public void SetStr3(object s)
		{
			textBox3.Text = s.ToString();
		}
	}
}