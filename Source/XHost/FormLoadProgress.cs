/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XHost
{
	public partial class FormLoadProgress : Form
	{
		static bool _showed;
		static FormLoadProgress _form;
		public FormLoadProgress()
		{
			InitializeComponent();
		}
		public void SetInfo(string info)
		{
			lblInfo.Text = info;
			lblInfo.Refresh();
			lblDots.Refresh();
			label1.Refresh();
		}
		public void Step2()
		{
			int n = lblDots.Left + 10;
			if (n > label1.Left - lblDots.Width)
			{
				n = lblInfo.Left + 10;
			}
			lblDots.Left = n;
			lblDots.Refresh();
			label1.Refresh();
		}
		private void label1_Click(object sender, EventArgs e)
		{
			Close();
		}
		public static void ShowLoadProgress()
		{
			if (!_showed)
			{
				_showed = true;
				_form = new FormLoadProgress();
				_form.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - _form.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - _form.Height - 30));
				_form.Show();
			}
		}
		public static void CloseProgress()
		{
			if (_form != null && !_form.IsDisposed && !_form.Disposing)
			{
				_form.Close();
				_form = null;
			}
		}
		public static void SetProgInfo(string info)
		{
			if (_form != null && !_form.IsDisposed && !_form.Disposing)
			{
				_form.SetInfo(info);
			}
		}
		public static void Step()
		{
			if (_form != null && !_form.IsDisposed && !_form.Disposing)
			{
				_form.Step2();
			}
		}
	}
}
