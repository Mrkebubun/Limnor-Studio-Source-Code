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
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public partial class DlgComponents : Form
	{
		public object objSelected;
		public DlgComponents()
		{
			InitializeComponent();
			listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
		}
		public static bool SelectItem(object[] list, string title, ref object returnObject, Form caller)
		{
			DlgComponents dlg = new DlgComponents();
			dlg.LoadData(list, title);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				returnObject = dlg.objSelected;
				return true;
			}
			return false;
		}
		void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				objSelected = listBox1.Items[listBox1.SelectedIndex];
			}
		}
		public void LoadData(object[] list, string title)
		{
			this.Text = title;
			if (list != null && list.Length > 0)
			{
				for (int i = 0; i < list.Length; i++)
				{
					listBox1.Items.Add(list[i]);
				}
				listBox1.SelectedIndex = 0;
				objSelected = listBox1.Items[0];
			}
			btOK.Enabled = (listBox1.SelectedIndex >= 0);
		}
		public int ItemCount
		{
			get
			{
				return listBox1.Items.Count;
			}
		}
		public object GetItem(int i)
		{
			if (i >= 0 && i < listBox1.Items.Count)
			{
				return listBox1.Items[i];
			}
			return null;
		}
		public void SetSelection(int i)
		{
			if (i >= 0 && i < listBox1.Items.Count)
			{
				listBox1.SelectedIndex = i;
				objSelected = listBox1.Items[i];
			}
		}
	}
}
