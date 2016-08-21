/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	/// <summary>
	/// for debugging a single form
	/// </summary>
	public class DebugRunForm : DebugRun
	{
		public DebugRunForm()
		{
		}
		public override void Run()
		{
			Form f = RootObject as Form;
			f.Show();
		}
	}
	/// <summary>
	/// for debugging a win form app
	/// </summary>
	public class DebugRunWinApp : DebugRun
	{
		public DebugRunWinApp()
		{
		}
		public override void Run()
		{
			LimnorWinApp app = RootObject as LimnorWinApp;
			app.Run();
		}
	}
}
