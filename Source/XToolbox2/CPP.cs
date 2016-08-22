/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace XToolbox2
{
	public sealed class CPP
	{
		[DllImport("LimnorUtil.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr DLLLoadRes_Bitmap(string lpszModule, int res);
		[DllImport("LimnorUtil.dll", CharSet = CharSet.Unicode)]
		public static extern void DLLGetLastError(System.Text.StringBuilder lpszError, int size);
		[DllImport("Kernel32.dll")]
		public static extern uint GetLastError();
		public const int WM_USER = 0x0400;
		public const int WM_ANIM_SHOW = WM_USER + 1;
		public const int WM_ANIM_HIDE = WM_USER + 2;
		[DllImport("User32")]
		public static extern int AnimateWindow(IntPtr hwnd, uint dwTime, uint dwFlags);
		const int AW_HOR_POSITIVE = 0x00000001;
		const int AW_HOR_NEGATIVE = 0x00000002;
		const int AW_VER_POSITIVE = 0x00000004;
		const int AW_VER_NEGATIVE = 0x00000008;
		const int AW_CENTER = 0x00000010;
		const int AW_HIDE = 0x00010000;
		const int AW_ACTIVATE = 0x00020000;
		const int AW_SLIDE = 0x00040000;
		const int AW_BLEND = 0x00080000;
		[DllImport("gdi32")]
		public static extern bool DeleteObject(IntPtr h);
		[DllImport("User32")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("User32")]
		public static extern int PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		private CPP()
		{
		}
		public static void AnimateWindowShow(IntPtr hWnd)//,ref string err)
		{
			AnimateWindow(hWnd, 200, AW_SLIDE | AW_HOR_POSITIVE);
		}
		public static void AnimateWindowHide(IntPtr hWnd)//, ref string err)
		{
			AnimateWindow(hWnd, 200, AW_SLIDE | AW_HOR_NEGATIVE | AW_HIDE);
		}
	}
}
