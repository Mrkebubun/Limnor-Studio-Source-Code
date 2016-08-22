/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Limnor.Windows
{
	[ToolboxBitmapAttribute(typeof(WindowsManager), "Resources.process.png")]
	[Description("This object is used to execute an EXE program, open a file with associated program, visit a web URL with the default web browser, etc.")]
	public partial class ExecuteFile : Component
	{
		private Process _proc = null;
		private string _errorMessage;
		public ExecuteFile()
		{
			InitializeComponent();
		}

		public ExecuteFile(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
		[Description("Occurs when the process exits")]
		public event EventHandler Exit;
		[Description("Gets a string representing the error message of the last operation")]
		public string LastError
		{
			get
			{
				return _errorMessage;
			}
		}
		[Description("Execute a program or open a file with associated program, or visit web URL with the default web browser.")]
		public bool Execute(string file, string arguments)
		{
			try
			{
				_proc = new Process();
				_proc.StartInfo.FileName = file;
				_proc.StartInfo.Arguments = arguments;
				_proc.Exited += _proc_Exited;
				bool b = _proc.Start();
				return b;
			}
			catch (Exception err)
			{
				_errorMessage = err.Message;
			}
			return false;
		}

		private void _proc_Exited(object sender, EventArgs e)
		{
			if (Exit != null)
			{
				Exit(sender, e);
			}
		}
	}
}
