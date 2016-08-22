/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	UI Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TraceLog;
using System.Runtime.InteropServices;
using WindowsUtility;

namespace LimnorUI
{
	public sealed class UIUtil
	{
		private UIUtil()
		{
		}
		public static void ClickMouseAndBack(int x0, int y0)
		{
			System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
			WinUtil.ClickMouse(x0, y0);
			System.Threading.Thread.Sleep(10);
			WinUtil.MoveMouse(curPoint.X, curPoint.Y);
		}
		/// <summary>
		/// ask whether to cancel the closing of the dialogue box
		/// </summary>
		/// <param name="caller">the dialogue box</param>
		/// <returns>true:cancel the closing</returns>
		public static bool AskCancelCloseDialog(Form caller)
		{
			string title;
			if (caller == null)
				title = "VOB";
			else
				title = caller.Text;
			if (MessageBox.Show(caller, "Do you want to close this window and discard all the modifications?\r\n\r\nClick Yes to close this window and discard all the modifications.\r\nClick No to resume working on this window.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return true;
			}
			return false;
		}
		public static void ShowWarning(Form caller, string message)
		{
			string title;
			if (caller == null)
				title = "VOB";
			else
				title = caller.Text;
			MessageBox.Show(caller, message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}
		public static void ShowError(Form caller, string message)
		{
			string title;
			if (caller == null)
				title = "VOB";
			else
				title = caller.Text;
			MessageBox.Show(caller, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
		}
		public static void ShowError(Form caller, string messageFormat, params object[] values)
		{
			if (values == null)
			{
				ShowError(caller, messageFormat);
			}
			else
			{
				ShowError(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture, messageFormat, values));
			}
		}
		public static void ShowError(Form caller, Exception e)
		{
			ShowError(caller, TraceLogClass.ExceptionMessage0(e));
		}
	}
}
