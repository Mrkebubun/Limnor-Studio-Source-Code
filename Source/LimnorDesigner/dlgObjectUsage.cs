/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
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

namespace LimnorDesigner
{
	public partial class dlgObjectUsage : Form
	{
		public dlgObjectUsage()
		{
			InitializeComponent();
		}
		public void LoadData(string msg, string title, List<ObjectTextID> list)
		{
			this.Text = title;
			label1.Text = msg;
			foreach (ObjectTextID id in list)
			{
				listBox1.Items.Add(id);
			}
		}
	}
}
