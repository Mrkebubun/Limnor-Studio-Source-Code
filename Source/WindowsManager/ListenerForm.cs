/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Limnor.Windows
{
	class ListenerForm : Form
	{

		[DllImport("NativeWindowsUtility.dll")]
		static extern int fnHook(IntPtr hWndMessageSource, IntPtr hWndNotify);
		[DllImport("NativeWindowsUtility.dll")]
		static extern void fnUnhook();

		[DllImport("NativeWindowsUtility64.dll")]
		static extern int fnHook64(IntPtr hWndMessageSource, IntPtr hWndNotify);
		[DllImport("NativeWindowsUtility64.dll")]
		static extern void fnUnhook64();

		private WindowsManager _owner;
		private IntPtr _hWnd;
		public ListenerForm(WindowsManager owner)
		{
			_owner = owner;
		}
		public void Start(IntPtr hWnd)
		{
			End();

			_hWnd = hWnd;
			if (IntPtr.Size == 4)
			{
				fnHook(_hWnd, this.Handle);
			}
			else
			{
				fnHook64(_hWnd, this.Handle);
			}
		}
		public void End()
		{
			if (IntPtr.Size == 4)
			{
				fnUnhook();
			}
			else
			{
				fnUnhook64();
			}
		}
		protected override void WndProc(ref Message m)
		{
			const int WM_SETTEXT = 0x000C;

			if (m.Msg == WM_SETTEXT)
			{
				string[] ss = Text.Split(',');
				if (ss != null && ss.Length > 0)
				{
					try
					{
						int msg = int.Parse(ss[0]);
						_owner.OnWindowEvent(msg, ss);

					}
					catch (Exception err)
					{
						_owner.SetError(err.Message);
					}
				}
			}
			base.WndProc(ref m);
		}
	}
}
