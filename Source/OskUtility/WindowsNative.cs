/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	On-Screen-Keyboard component
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OskUtility
{
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	sealed class WindowsNative
	{
		const uint SWP_NOSIZE = 0x0001;
		const uint SWP_SHOWWINDOW = 0x0040;

		const uint MF_GRAYED = 0x1;
		const uint MF_BYCOMMAND = 0x0;
		const uint SC_CLOSE = 0xF060;
		const uint MF_ENABLED = 0x00000000;

		const UInt32 WM_CLOSE = 0x0010;

		const int GWL_STYLE = -16;
		const int WS_MINIMIZEBOX = 0x20000;
		const int WS_MAXIMIZEBOX = 0x10000;
		const int WS_CAPTION = 0x00C00000;
		const int WS_BORDER = 0x00800000;

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		[DllImport("user32")]
		public static extern int EnumWindows(EnumWindowsProc cb, int lparam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool IsWindow(IntPtr hWnd);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[DllImport("user32.dll")]
		public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int CloseWindow(IntPtr hWnd);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("User32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("User32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("User32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nIndex);

		public static void MoveWindow(IntPtr hWnd, int x, int y)
		{
			SetWindowPos(hWnd, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_SHOWWINDOW);
		}
		public static void DisableCloseButton(IntPtr hWnd)
		{
			IntPtr hMenu = GetSystemMenu(hWnd, false);
			EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
		}
		public static void CloseWindow2(IntPtr hWnd)
		{
			SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
		}

		public static void DisableMinimizeButton(IntPtr hwnd)
		{
			SetWindowLong(hwnd, GWL_STYLE,
						  GetWindowLong(hwnd, GWL_STYLE) & ~WS_MINIMIZEBOX);
		}

		public static void EnableCloseButton(IntPtr hwnd)
		{
			EnableMenuItem(GetSystemMenu(hwnd, false), SC_CLOSE,
						   MF_BYCOMMAND | MF_ENABLED);
		}

		public static void EnableMinimizeButton(IntPtr hwnd)
		{
			SetWindowLong(hwnd, GWL_STYLE,
						  GetWindowLong(hwnd, GWL_STYLE) | WS_MINIMIZEBOX);
		}

		public static void DisableMaximizeButton(IntPtr hwnd)
		{
			SetWindowLong(hwnd, GWL_STYLE,
						  GetWindowLong(hwnd, GWL_STYLE) & ~WS_MAXIMIZEBOX);
		}

		public static void EnableMaximizeButton(IntPtr hwnd)
		{
			SetWindowLong(hwnd, GWL_STYLE,
						  GetWindowLong(hwnd, GWL_STYLE) | WS_MAXIMIZEBOX);
		}

		public static int GetWindowBoarderStyle(IntPtr hwnd)
		{
			return GetWindowLong(hwnd, GWL_STYLE);
		}

		public static void SetWindowBoarderStyle(IntPtr hwnd, int style)
		{
			SetWindowLong(hwnd, GWL_STYLE, style);
		}

		public static void RemoveWindowBoarder(IntPtr hwnd, int style)
		{
			SetWindowLong(hwnd, GWL_STYLE, style & ~WS_BORDER);
		}

		public static void SetWindowVisible(IntPtr hwnd, bool visible)
		{
			int n = SW_HIDE;
			if (visible)
			{
				n = SW_SHOW;
			}
			ShowWindow(hwnd, n);
		}
	}
}
