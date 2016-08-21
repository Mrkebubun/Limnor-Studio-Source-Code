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

namespace LimnorDesigner.EventMap
{
	public partial class DialogSelectComponentIcon : Form
	{
		public ComponentIconEvent SelectedIcon;
		public DialogSelectComponentIcon()
		{
			InitializeComponent();
		}
		public void LoadData(List<ComponentIconEvent> icons)
		{
			foreach (ComponentIconEvent cie in icons)
			{
				listBox1.Items.Add(cie);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				SelectedIcon = listBox1.Items[n] as ComponentIconEvent;
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
