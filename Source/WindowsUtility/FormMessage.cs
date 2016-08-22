using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsUtility
{
	public partial class FormMessage : Form
	{
		const int AW_HIDE = 0X10000;
		const int AW_BLEND = 0X80000;
		[DllImport("user32.dll")]
		static extern bool AnimateWindow(System.IntPtr hwnd,
			int dwTime,
			int dwFlags
			);
		public FormMessage()
		{
			InitializeComponent();
		}
		public static void DisplayMessage(string title, string message)
		{
			FormMessage f = new FormMessage();
			f.ShowMessage(title, message);
			f.Show();
		}
		public void ShowMessage(string title, string message)
		{
			Text = title;
			label1.Text = message;
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			AnimateWindow(this.Handle, 2000, AW_HIDE | AW_BLEND);
		}
	}
}
