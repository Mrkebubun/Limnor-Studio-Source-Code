/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database connection manager
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace ConnectionManager
{
	static class Program
	{
		[DllImportAttribute("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);
		static Mutex mutex;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool b;
			mutex = new Mutex(true, "69E7EC26-8FF7-422d-B153-EE6D3EDB685E", out b);
			if (b || mutex != null)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new FormConnectionManager());
			}
			else
			{
				System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
				System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(p.ProcessName);
				for (int i = 0; (i < ps.Length); i++
				)
				{
					if ((((ps[i].Id != p.Id)
								&& (string.Compare(ps[i].MainModule.FileName, p.MainModule.FileName, System.StringComparison.OrdinalIgnoreCase) == 0))
								&& (p.MainWindowHandle != System.IntPtr.Zero)))
					{
						SetForegroundWindow(ps[i].MainWindowHandle);
						break;
					}
				}
				System.Windows.Forms.MessageBox.Show("The Connection Manager is already running");
				return;
			}
		}
	}
}
