/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
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

namespace Limnor.Drawing2D
{
	public partial class FormDebug : Form
	{
		private static FormDebug _frm;
		public FormDebug()
		{
			InitializeComponent();
		}
		public void AddMsg(string msg)
		{
			listBox1.Items.Add(msg + ":" + System.DateTime.Now.ToString("u"));
		}
		public static void AddMessage(string msg)
		{
			if (_frm == null)
			{
				_frm = new FormDebug();
				_frm.Show();
			}
			_frm.AddMsg(msg);
		}
	}
}
