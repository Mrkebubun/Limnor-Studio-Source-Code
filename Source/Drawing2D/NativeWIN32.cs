/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Windows.Forms; // for Key namespace
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Limnor.Drawing2D
{
	public enum MessageBeepType
	{
		Default = -1,
		Ok = 0x00000000,
		Error = 0x00000010,
		Question = 0x00000020,
		Warning = 0x00000030,
		Information = 0x00000040,
	}
	/// <summary>
	/// Summary description for NativeWIN32.
	/// </summary>
	class NativeWIN32
	{
		public NativeWIN32()
		{
		}

		public const int WM_CTLCOLORMSGBOX = 0x0132;
		public const int WM_CTLCOLOREDIT = 0x0133;
		public const int WM_CTLCOLORLISTBOX = 0x0134;
		public const int WM_CTLCOLORBTN = 0x0135;
		public const int WM_CTLCOLORDLG = 0x0136;
		public const int WM_CTLCOLORSCROLLBAR = 0x0137;
		public const int WM_CTLCOLORSTATIC = 0x0138;
		public const int MN_GETHMENU = 0x01E1;

		public const int WM_KEYDOWN = 0x0100;
		public const int WM_KEYUP = 0x0101;

		public const int WM_MOUSEFIRST = 0x0200;
		public const int WM_MOUSEMOVE = 0x0200;
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_LBUTTONUP = 0x0202;
		public const int WM_LBUTTONDBLCLK = 0x0203;
		public const int WM_RBUTTONDOWN = 0x0204;
		public const int WM_RBUTTONUP = 0x0205;
		public const int WM_RBUTTONDBLCLK = 0x0206;
		public const int WM_MBUTTONDOWN = 0x0207;
		public const int WM_MBUTTONUP = 0x0208;
		public const int WM_MBUTTONDBLCLK = 0x0209;
		//#if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
		public const int WM_MOUSEWHEEL = 0x020A;
		//#endif
		//#if (_WIN32_WINNT >= 0x0500)
		public const int WM_XBUTTONDOWN = 0x020B;
		public const int WM_XBUTTONUP = 0x020C;
		public const int WM_XBUTTONDBLCLK = 0x020D;
		//#endif
		//#if (_WIN32_WINNT >= 0x0500)
		//public const int WM_MOUSELAST                    = 0x020D;
		//#elif (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
		//public const int WM_MOUSELAST                    = 0x020A;
		//#else
		//public const int WM_MOUSELAST = 0x0209;
		//#endif /* (_WIN32_WINNT >= 0x0500) */

		public const int MK_CONTROL = 0x8;
		public const int MK_LBUTTON = 0x1;
		public const int MK_MBUTTON = 0x10;
		public const int MK_RBUTTON = 0x2;
		public const int MK_SHIFT = 0x4;
		public const int MK_XBUTTON1 = 0x20;
		public const int MK_XBUTTON2 = 0x40;

		const int AW_HIDE = 0X10000;
		const int AW_ACTIVATE = 0X20000;
		const int AW_HOR_POSITIVE = 0X1;
		const int AW_HOR_NEGATIVE = 0X2;
		const int AW_VER_POSITIVE = 0x00000004;
		const int AW_VER_NEGATIVE = 0x00000008;
		const int AW_SLIDE = 0X40000;
		const int AW_BLEND = 0X80000;
		const int AW_CENTER = 0x00000010;

		public const uint WS_EX_TOOLWINDOW = 0x00000080;
		public const uint WS_EX_APPWINDOW = 0x00040000;
		public const uint WS_EX_NOACTIVATE = 0x08000000;
		public const int GWL_EXSTYLE = -20;
		[DllImport("User32.dll")]    //LONG_PTR for return and parameter
		public static extern System.IntPtr SetWindowLongPtr(System.IntPtr hWnd, int nIndex, System.IntPtr dwNewLong);
		[DllImport("User32.dll")]
		public static extern uint GetWindowLong(System.IntPtr hWnd, int nIndex);
		[DllImport("User32.dll")]
		public static extern uint SetWindowLong(System.IntPtr hWnd, int nIndex, uint dwNewLong);
		[DllImport("User32.dll")]
		public static extern System.IntPtr SetFocus(System.IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern bool AnimateWindow(System.IntPtr hwnd,
			int dwTime,
			int dwFlags
			);
		//
		[DllImport("gdi32.dll")]
		public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
		[DllImport("Kernel32.dll")]
		public static extern uint GetLogicalDrives();
		[DllImport("Kernel32.dll")]
		public static extern uint GetDriveType(string lpRootPathName);
		[DllImport("Kernel32.dll")]
		public static extern uint GetTickCount();
		[DllImport("Kernel32.dll")]
		public static extern uint GetCurrentThreadId();
		[DllImport("User32.dll")]
		public static extern uint GetWindowThreadProcessId(System.IntPtr hWnd, ref uint lpdwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool Beep(int freq, int duration);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MessageBeep(MessageBeepType type);

		public const int DRIVE_UNKNOWN = 0;
		public const int DRIVE_NO_ROOT_DIR = 1;
		public const int DRIVE_REMOVABLE = 2;
		public const int DRIVE_FIXED = 3;
		public const int DRIVE_REMOTE = 4;
		public const int DRIVE_CDROM = 5;
		public const int DRIVE_RAMDISK = 6;

		/* ------- using WIN32 Windows API in a C# application ------- */

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static public extern IntPtr GetForegroundWindow(); // 

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct STRINGBUFFER
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szText;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hWnd, out STRINGBUFFER ClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, long lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_CLOSE = 0xF060;

		public delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr next, string sClassName, IntPtr sWindowTitle);


		/* ------- using HOTKEYs in a C# application -------

		 in form load :
			bool success = RegisterHotKey(Handle, 100,     KeyModifiers.Control | KeyModifiers.Shift, Keys.J);

		 in form closing :
			UnregisterHotKey(Handle, 100);
 

		 protected override void WndProc( ref Message m )
		 {	
			const int WM_HOTKEY = 0x0312; 	
	
			switch(m.Msg)	
			{	
				case WM_HOTKEY:		
					clsAppGlobals.MsgBox("Hotkey pressed");		
					break;	
			} 	
			base.WndProc(ref m );
		}

		------- using HOTKEYs in a C# application ------- */

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool RegisterHotKey(IntPtr hWnd, // handle to window    
			int id,            // hot key identifier    
			KeyModifiers fsModifiers,  // key-modifier options    
			Keys vk            // virtual-key code    
			);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool UnregisterHotKey(IntPtr hWnd,  // handle to window    
			int id      // hot key identifier    
			);

		[Flags()]
		public enum KeyModifiers
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			Windows = 8
		}
		public static void SetToolWin(System.IntPtr hWnd)
		{
			uint style = GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_TOOLWINDOW;
			style &= (~WS_EX_APPWINDOW);
			SetWindowLong(hWnd, GWL_EXSTYLE, style);
		}
		public static bool LowerThanWinXP()
		{
			if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT)
				return true;
			if (System.Environment.OSVersion.Version.Major < 5)
				return true;
			if (System.Environment.OSVersion.Version.Minor < 1)
				return true;
			return false;
		}
		public static void ShowFading(IntPtr hWnd, int fadeTime)
		{
			AnimateWindow(hWnd, fadeTime, AW_ACTIVATE | AW_BLEND);
		}
		public static void ClodeFading(IntPtr hWnd, int fadeTime)
		{
			AnimateWindow(hWnd, fadeTime, AW_HIDE | AW_BLEND);
		}
		public static string[] GetDriverLetters()
		{
			string[] ret = null;
			uint n = GetLogicalDrives();
			uint mask = 1;
			for (uint i = 0; i < 26; i++)
			{
				if ((mask & n) != 0)
				{
					if (ret == null)
					{
						ret = new string[1];
						ret[0] = new string((char)(i + 65), 1);
						ret[0] += ":";
					}
					else
					{
						int m = ret.Length;
						string[] a = new string[m + 1];
						for (int j = 0; j < m; j++)
							a[j] = ret[j];
						ret = a;
						ret[m] = new string((char)(i + 65), 1);
						ret[m] += ":";
					}
				}
				mask *= 2;
			}
			return ret;
		}
		public static bool IsCDRom(string sRoot)
		{
			uint n = GetDriveType(sRoot);
			return ((n == DRIVE_CDROM));
		}
		public static string GetLastWinAPIError()
		{
			return new Win32Exception(Marshal.GetLastWin32Error()).Message;
		}
		static public long MakeDWord(int LoWord, int HiWord)
		{
			return (HiWord * 0x10000) | (LoWord & 0xFFFF);
		}
	}
}
