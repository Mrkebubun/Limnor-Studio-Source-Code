/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PerformerImport
{
	public partial class FormError : Form
	{
		public FormError()
		{
			InitializeComponent();
		}
		public void SetMessage(string msg)
		{
			textBox1.Text = msg;
		}
	}
}
