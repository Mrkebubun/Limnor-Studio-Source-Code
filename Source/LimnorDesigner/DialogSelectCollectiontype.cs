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
	public partial class DialogSelectCollectiontype : Form
	{
		public DataTypePointer Ret;
		public DialogSelectCollectiontype()
		{
			InitializeComponent();
		}
		public void AddType(DataTypePointer tp)
		{
			int n = listBox1.Items.Add(tp);
			listBox1.SelectedIndex = n;
		}
		public int Count
		{
			get
			{
				return listBox1.Items.Count;
			}
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			Ret = listBox1.Items[listBox1.SelectedIndex] as DataTypePointer;
			this.DialogResult = DialogResult.OK;
		}
	}
}
