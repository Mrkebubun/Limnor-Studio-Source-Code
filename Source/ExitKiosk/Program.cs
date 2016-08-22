/*
 
 * Author:	Flora Lee (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Kiosk Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorKiosk;

namespace ExitKiosk
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args != null && args.Length > 0)
			{
				if (string.Compare(args[0], "start", StringComparison.OrdinalIgnoreCase) == 0)
				{
					LKiosk.Enterkiosk();
				}
				else if (string.Compare(args[0], "stop", StringComparison.OrdinalIgnoreCase) == 0)
				{
					LKiosk.Exitkiosk();
				}
				else
				{
					Console.WriteLine("Usage:");
					Console.WriteLine("kioskMode [start|stop]");
					Console.WriteLine();
					Console.WriteLine("If no argument is given then stop is used.");
				}
			}
			else
			{
				LKiosk.Exitkiosk();
			}
		}
	}
}
