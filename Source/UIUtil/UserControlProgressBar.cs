/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	UI Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LimnorUI
{
	public partial class UserControlProgressBar : UserControl
	{
		public int Step = 2;
		public UserControlProgressBar()
		{
			InitializeComponent();
			progressBar1.Value = 20;
		}
		public void SetMessage(string message)
		{
			lblMessage.Text = message;
		}
		public static UserControlProgressBar ShowProgressBar(Control owner, string message)
		{
			UserControlProgressBar bar = new UserControlProgressBar();
			bar.SetMessage(message);
			owner.Controls.Add(bar);
			if (bar.Width > owner.ClientSize.Width)
				bar.Width = owner.ClientSize.Width;
			int x = (owner.ClientSize.Width - bar.Width) / 2;
			int y = (owner.ClientSize.Height - bar.Height) / 2;
			if (y < 0)
				y = 0;
			bar.Location = new Point(x, y);
			bar.BringToFront();
			owner.Show();
			Application.DoEvents();
			return bar;
		}
		public void UnloadProgressBar()
		{
			progressBar1.Value = progressBar1.Maximum;
			progressBar1.Refresh();

			System.Threading.Thread.Sleep(500);
			if (this.Parent != null)
			{
				this.Parent.Controls.Remove(this);
			}
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (progressBar1.Value < progressBar1.Maximum)
			{
				progressBar1.Increment(Step);
			}
			else
			{
				progressBar1.Value = progressBar1.Minimum;
			}
			progressBar1.Refresh();
			Application.DoEvents();
		}
	}
}
