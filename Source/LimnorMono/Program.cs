/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MathExp;
using System.Text;
using VPL;
#if USECEF
using CefSharp.LimnorStudio;
using System.IO;
#endif
namespace LimnorVOB
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
#if USECEF
                string cachePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "WebCache");
                LimnorStudioPresenter.Init(cachePath);
#endif
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
				Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				FormSplash.ShowSplash();
				Application.Run(new LimnorVOBMain());
			}
			catch (Exception err)
			{
				FormSplash.CloseSplash();
				MessageBox.Show(FormExceptionText(err), "Exception in Main");
			}
		}
		static public string FormExceptionText(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			if (e.StackTrace != null)
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				if (e.StackTrace != null)
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			FormSplash.CloseSplash();
			if (e.Exception is ExceptionIgnore)
			{
			}
			else
			{
				MessageBox.Show(FormExceptionText(e.Exception), "Application Thread Exception");
			}
		}
	}
}