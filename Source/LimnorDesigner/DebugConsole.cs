/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LimnorDesigner
{
	public class DebugConsole : DebugRun
	{
		[DllImport("Kernel32.dll")]
		static extern bool AllocConsole();
		[DllImport("Kernel32.dll")]
		static extern bool AttachConsole(int dwProcessId);
		[DllImport("Kernel32.dll")]
		static extern bool FreeConsole();
		public DebugConsole()
		{
		}
		public override void Run()
		{
			if (AllocConsole())
			{
				LimnorConsole app = RootObject as LimnorConsole;
				app.Run();
				Console.Read();
				FreeConsole();
			}
		}
	}
}
