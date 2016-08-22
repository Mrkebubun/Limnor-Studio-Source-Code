/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.ComponentModel;
using System.Threading;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Globalization;

namespace WindowsUtility
{
	public enum EnumWindowsOS
	{
		Unknown = 0,
		Windows3,
		Windows95,
		Windows98,
		WindowsMe,
		WindowsNT3,
		WindowsNT4,
		Windows2000,
		WindowsXP,
		Windows2003,
		WindowsVista,
		Windows7
	}
	public delegate void fnSimpleFunction();
	public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
	public sealed class WinUtil
	{
		#region consts
		public const ushort KEYEVENTF_KEYDOWN = 0x0000;
		public const ushort KEYEVENTF_KEYUP = 0x0002;

		const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
		const int IMAGE_BITMAP = 0;
		const int IMAGE_ICON = 1;
		const int IMAGE_CURSOR = 2;
		const int IMAGE_ENHMETAFILE = 3;

		const int LR_DEFAULTCOLOR = 0x0000;
		const int LR_MONOCHROME = 0x0001;
		const int LR_COLOR = 0x0002;
		const int LR_COPYRETURNORG = 0x0004;
		const int LR_COPYDELETEORG = 0x0008;
		const int LR_LOADFROMFILE = 0x0010;
		const int LR_LOADTRANSPARENT = 0x0020;
		const int LR_DEFAULTSIZE = 0x0040;
		const int LR_VGACOLOR = 0x0080;
		const int LR_LOADMAP3DCOLORS = 0x1000;
		const int LR_CREATEDIBSECTION = 0x2000;
		const int LR_COPYFROMRESOURCE = 0x4000;
		const int LR_SHARED = 0x8000;

		const int SM_CXSCREEN = 0;
		const int SM_CYSCREEN = 1;
		const int SM_CYCAPTION = 4;
		const int SM_CYBORDER = 6;

		const UInt32 SRCCOPY = (UInt32)0x00CC0020; /* dest = source                   */

		const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
		const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
		const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
		const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
		const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
		const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
		const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
		const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
		const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
		const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
		const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
		const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */
		#endregion
		#region structs
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct CURSORINFO
		{
			public UInt32 cbSize;
			public UInt32 flags;
			public IntPtr hCursor;
			POINT ptScreenPos;
		}

		#endregion
		#region win API
		[DllImport("kernel32", SetLastError = true)]
		static extern IntPtr LoadLibrary(string lpFileName);
		[DllImport("kernel32", SetLastError = true)]
		static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
		[DllImport("kernel32", SetLastError = true)]
		static extern IntPtr FreeLibrary(IntPtr hLibModule);
		[DllImport("User32.dll")]
		public static extern IntPtr LoadImage(IntPtr hInstance, int uID, uint type, int width, int height, int load);
		[DllImport("User32.dll")]
		public static extern IntPtr LoadBitmap(IntPtr hInstance, int uID);
		[DllImport("kernel32.dll")]
		static extern IntPtr FindResource(IntPtr hModule, int lpID, string lpType);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
		[DllImport("User32.dll")]
		static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);
		[DllImport("user32")]
		static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);
		[DllImport("kernel32.dll")]
		private static extern int IsDebuggerPresent();
		[DllImport("user32")]
		public static extern IntPtr GetWindowDC(IntPtr hwnd);
		[DllImport("gdi32")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		[DllImport("user32")]
		public static extern int GetSystemMetrics(int nIndex);
		[DllImport("gdi32")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
		[DllImport("gdi32")]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		[DllImport("gdi32")]
		public static extern int DeleteObject(IntPtr hObject);
		[DllImport("gdi32")]
		public static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, UInt32 dwRop);
		[DllImport("gdi32")]
		public static extern int StretchBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, UInt32 dwRop);

		[DllImport("user32")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("gdi32")]
		public static extern int DeleteDC(IntPtr hDC);
		[DllImport("user32")]
		public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);
		[DllImport("user32")]
		static extern int InvalidateRect(IntPtr hwnd, ref RECT lpRect, int bErase);
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, uint msg, int p, int w);
		[DllImport("user32")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32")]
		public static extern int EnableWindow(IntPtr hwnd, int fEnable);

		//This is the Import for the SetWindowsHookEx function.
		//Use this function to install a thread-specific hook.
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

		//This is the Import for the UnhookWindowsHookEx function.
		//Call this function to uninstall the hook.
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(IntPtr idHook);

		//This is the Import for the CallNextHookEx function.
		//Use this function to pass the hook information to the next hook procedure in chain.
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32")]
		public static extern short GetAsyncKeyState(int vKey);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SystemParametersInfo(int uiAction, uint uiParam, IntPtr pvParam, int fWinIni);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorInfo(ref CURSORINFO pci);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
		#endregion
		#region windows util
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lpszModule"></param>
		/// <param name="res"></param>
		/// <returns></returns>
		public static IntPtr LoadRes_Bitmap(string lpszModule, int res)
		{
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr hModule = LoadLibraryEx(lpszModule, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
			if (hModule != IntPtr.Zero)
			{
				hBitmap = LoadImage(hModule,
							res, IMAGE_BITMAP, 0, 0, LR_DEFAULTCOLOR);
				FreeLibrary(hModule);
			}
			return hBitmap;
		}
		public Bitmap LoadRes_PNG(string moduleFile, int res)
		{
			IntPtr hMod = LoadLibraryEx(moduleFile, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
			IntPtr hRes = FindResource(hMod, res, "PNG");
			uint size = SizeofResource(hMod, hRes);
			IntPtr pt = LoadResource(hMod, hRes);
			byte[] bPtr = new byte[size];
			Marshal.Copy(pt, bPtr, 0, (int)size);
			using (MemoryStream m = new MemoryStream(bPtr))
				return (Bitmap)Bitmap.FromStream(m);
		}
		public static EnumWindowsOS GetWindowsOS()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
					return EnumWindowsOS.Windows3;
				case PlatformID.Win32Windows:
					switch (Environment.OSVersion.Version.Minor)
					{
						case 0:
							return EnumWindowsOS.Windows95;
						case 10:
							return EnumWindowsOS.Windows98;
						case 90:
							return EnumWindowsOS.WindowsMe;
						default:
							return EnumWindowsOS.Windows98;
					}
				case PlatformID.Win32NT:
					switch (Environment.OSVersion.Version.Major)
					{
						case 3:
							return EnumWindowsOS.WindowsNT3;
						case 4:
							return EnumWindowsOS.WindowsNT4;
						case 5:
							switch (Environment.OSVersion.Version.Minor)
							{
								case 0:
									return EnumWindowsOS.Windows2000;
								case 1:
									return EnumWindowsOS.WindowsXP;
								case 2:
									return EnumWindowsOS.Windows2003;
								default:
									return EnumWindowsOS.Windows2000;
							}
						case 6:
							if (Environment.OSVersion.Version.Minor == 0)
							{
								return EnumWindowsOS.WindowsVista;
							}
							else
							{
								return EnumWindowsOS.Windows7;
							}
						default:
							return EnumWindowsOS.WindowsNT3;
					}
				default:
					return EnumWindowsOS.Unknown;

			}
		}
		public static void SendKeyPress(VK key)
		{
			InputUtil.SendKeyPress(key);
		}
		static ManualResetEvent _captureEvent;
		private static void sendcaptureKeys()
		{
			InputUtil.SendKeyPress(VK.VK_SNAPSHOT);
			_captureEvent.Set();
		}
		public static Bitmap CaptureScreen()
		{
			Clipboard.Clear();
			Thread th = new Thread(sendcaptureKeys);
			_captureEvent = new ManualResetEvent(false);
			th.Start();
			_captureEvent.WaitOne();
			Thread.Sleep(300);
			Bitmap bmp = null;
			int n = 0;
			while (bmp == null && n < 10)
			{
				try
				{
					if (Clipboard.ContainsImage())
					{
						bmp = (Bitmap)Clipboard.GetImage();
					}
				}
				catch { }
				if (bmp == null)
				{
					n++;
					Thread.Sleep(100);
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
				}
			}
			return bmp;
		}
		public static IntPtr FindWindowByTitle(string windowTitle)
		{
			return FindWindow(null, windowTitle);
		}
		[Browsable(false)]
		public static void InitApp()
		{
			string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CopyProtectionClient.dll");
			if (File.Exists(s))
			{
				if (System.Diagnostics.Debugger.IsAttached)
				{
					kill();
				}
				else if (IsDebuggerPresent() != 0)
				{
					kill();
				}
			}
		}
		//capture window image starting from (x,y). if width/height is 0 then the window size is used.
		//the calling program is responsible for calling DeleteObject to avoid memory leak
		public static IntPtr CaptureScreen(IntPtr hwnd, Int32 x, Int32 y, Int32 width, Int32 height)
		{
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr hDC;
			hDC = GetWindowDC(hwnd);
			if (hDC != IntPtr.Zero)
			{
				IntPtr hMemDC = CreateCompatibleDC(hDC);
				if (hMemDC != IntPtr.Zero)
				{
					//determine the valid starting point (x,y) and size
					Size size = new Size();
					int w;
					int h;
					if (hwnd == IntPtr.Zero)
					{
						w = GetSystemMetrics(SM_CXSCREEN);
						h = GetSystemMetrics(SM_CYSCREEN);
					}
					else
					{
						RECT r = new RECT();
						GetWindowRect(hwnd, ref r);
						w = r.Right - r.Left;
						h = r.Bottom - r.Top;
					}
					if (x >= w)
						x = 0;
					if (y >= h)
						y = 0;
					if (width <= 0)
						width = w;
					if (height <= 0)
						height = h;
					if (width + x > w)
					{
						size.Width = w - x; //the maximum width
					}
					else
					{
						size.Width = width;
					}
					if (height + y > h)
					{
						size.Height = h - y; //the maximum height
					}
					else
					{
						size.Height = height;
					}
					//create the image of the right size and paint it starting from (x,y)
					hBitmap = CreateCompatibleBitmap(hDC, size.Width, size.Height);
					if (hBitmap != IntPtr.Zero)
					{
						IntPtr hOld = SelectObject(hMemDC, hBitmap);
						BitBlt(hMemDC, 0, 0, size.Width, size.Height, hDC, x, y, SRCCOPY);
						SelectObject(hMemDC, hOld);
					}
					DeleteDC(hMemDC);
				}
				ReleaseDC(IntPtr.Zero, hDC);
			}
			return hBitmap;
		}
		public static Bitmap CaptureScreenImage(IntPtr hwnd, Int32 x, Int32 y, Int32 width, Int32 height, bool excludeBorder)
		{
			if (excludeBorder)
			{
				int x0 = 0;
				int y0 = 0;
				RECT crect = new RECT();
				RECT wrect = new RECT();
				GetClientRect(hwnd, out crect);
				GetWindowRect(hwnd, ref wrect);
				x0 = ((wrect.Right - wrect.Left) - (crect.Right - crect.Left)) / 2;
				y0 = (wrect.Bottom - wrect.Top) - (crect.Bottom - crect.Top) - x0;
				x = x + x0;
				y = y + y0;
			}
			IntPtr hImg = CaptureScreen(hwnd, x, y, width, height);
			if (hImg != IntPtr.Zero)
			{
				Bitmap bmp = Image.FromHbitmap(hImg);
				WinUtil.DeleteObject(hImg);
				return bmp;
			}
			return null;
		}
		public static IntPtr CaptureWindow(IntPtr hwnd)
		{
			return CaptureScreen(hwnd, 0, 0, 0, 0);
		}
		[Description("Capture window image to a bitmap")]
		public static Bitmap CaptureWindowImage(IntPtr hWnd)
		{
			if (hWnd != IntPtr.Zero)
			{
				IntPtr hImg = WinUtil.CaptureWindow(hWnd);
				if (hImg != IntPtr.Zero)
				{
					Bitmap bmp = Image.FromHbitmap(hImg);
					WinUtil.DeleteObject(hImg);
					return bmp;
				}
			}
			return null;
		}
		public static Bitmap SetImageOpacity(Image image, float opacity)
		{
			try
			{
				//create a Bitmap the size of the image provided   
				Bitmap bmp = new Bitmap(image.Width, image.Height);
				//create a graphics object from the image   
				using (Graphics gfx = Graphics.FromImage(bmp))
				{
					//create a color matrix object   
					ColorMatrix matrix = new ColorMatrix();
					//set the opacity   
					matrix.Matrix33 = opacity;
					//create image attributes   
					ImageAttributes attributes = new ImageAttributes();
					//set the color(opacity) of the image   
					attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					//now draw the image   
					gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
				}
				return bmp;
			}
			catch
			{
				return null;
			}
		}
		public static void DrawBmp(IntPtr hBitmap, Int32 width, Int32 height, Int32 x0, Int32 y0, Int32 w0, Int32 h0, Int32 x1, Int32 y1, Int32 w1, Int32 h1, Int32 n, Int32 timer, bool removeBitmap)
		{
			IntPtr hOld;
			IntPtr hOld2;
			IntPtr hdcScreen;
			IntPtr hdcCompatible;
			IntPtr hdcMem;
			IntPtr hbmScreen;
			//ddx2,ddy2,ddx,ddy
			double dx, dy, dw, dh, x, y, w, h, dt;
			Int32 wm, hm;
			RECT rc;
			//
			if (hBitmap == IntPtr.Zero)
			{
				return;
			}
			dt = (double)timer / (double)n;
			dx = ((double)x1 - (double)x0) / (double)n;
			dy = ((double)y1 - (double)y0) / (double)n;
			dw = ((double)w1 - (double)w0) / (double)n;
			dh = ((double)h1 - (double)h0) / (double)n;
			wm = w1;
			if (wm < w0)
				wm = w0;
			hm = h1;
			if (hm < h0)
				hm = h0;
			//create objects
			hdcScreen = GetDC(IntPtr.Zero); //+h1

			//animation
			x = x0;
			y = y0;
			w = w0;
			h = h0;
			if (dw < 0 && dh < 0)
			{
				//collapse 

				//find out the coverring area
				Int32 coverLeft = x0;
				Int32 coverTop = y0;
				Int32 coverRight = x1 + w1;
				Int32 coverBottom = y1 + h1;
				if (coverLeft > x1)
					coverLeft = x1;
				if (coverTop > y1)
					coverTop = y1;
				if (coverRight < x0 + w0)
					coverRight = x0 + w0;
				if (coverBottom < y0 + h0)
					coverBottom = y0 + h0;
				Int32 coverWdith = coverRight - coverLeft;
				Int32 coverHeight = coverBottom - coverTop;
				//
				//for saving screen image
				hbmScreen = CreateCompatibleBitmap(hdcScreen, coverWdith, coverHeight); //+h2 
				hdcMem = CreateCompatibleDC(hdcScreen); //+h3
				hdcCompatible = CreateCompatibleDC(hdcScreen); //+h4

				//using objects
				hOld = SelectObject(hdcMem, hBitmap);
				hOld2 = SelectObject(hdcCompatible, hbmScreen);

				//for memory drawing
				IntPtr hdcDrawBuffer = CreateCompatibleDC(hdcScreen); //+h5
				IntPtr hbmBuffer = CreateCompatibleBitmap(hdcScreen, coverWdith, coverHeight); //+h6
				IntPtr hOld3 = SelectObject(hdcDrawBuffer, hbmBuffer);
				//save the screen
				BitBlt(hdcCompatible,
					   0, 0,
					   coverWdith, coverHeight, //size of the coverring area
					   hdcScreen,
					   coverLeft, coverTop, //starting point of the coverring area
					   SRCCOPY);
				BitBlt(hdcDrawBuffer,
					   0, 0,
					   coverWdith, coverHeight,
					   hdcCompatible,
					   0, 0,
					   SRCCOPY);
				for (int i = 1; i <= n; i++)
				{
					double di = (double)i;
					//screen coordinates for the new position and size
					x = (double)x0 + di * dx;
					y = (double)y0 + di * dy;
					w = (double)w0 + di * dw;
					h = (double)h0 + di * dh;
					//draw to buffer, the origin is at (coverLeft, coverTop)
					StretchBlt(
					  hdcDrawBuffer,      // handle to destination DC
					  (int)(x - coverLeft), // x-coord of destination upper-left corner
					  (int)(y - coverTop), // y-coord of destination upper-left corner
					  (int)w,   // width of destination rectangle
					  (int)h,  // height of destination rectangle
					  hdcMem,       // handle to source DC
					  0,  // x-coord of source upper-left corner
					  0,  // y-coord of source upper-left corner
					  width,    // width of source rectangle
					  height,   // height of source rectangle
					  SRCCOPY       // raster operation code
					);
					//copy to screen
					BitBlt(hdcScreen,
					   (int)coverLeft, (int)coverTop,
					   (int)coverWdith, (int)coverHeight,
					   hdcDrawBuffer,
					   0, 0,
					   SRCCOPY);
					//restore draw buffer
					BitBlt(hdcDrawBuffer,
						   0, 0,
						   coverWdith, coverHeight,
						   hdcCompatible,
						   0, 0,
						   SRCCOPY);
					System.Threading.Thread.Sleep((int)(dt * di));
				}
				rc.Left = x0;
				rc.Top = y0;
				rc.Right = x0 + w0;
				rc.Bottom = y0 + h0;
				InvalidateRect(IntPtr.Zero, ref rc, 1);
				//
				SelectObject(hdcDrawBuffer, hOld3);
				DeleteObject(hbmBuffer); //-h6
				DeleteDC(hdcDrawBuffer);//-h5
			}
			else
			{
				//expand
				hbmScreen = CreateCompatibleBitmap(hdcScreen, wm, hm); //+h2
				hdcMem = CreateCompatibleDC(hdcScreen); //+h3
				hdcCompatible = CreateCompatibleDC(hdcScreen); //+h4

				//using objects
				hOld = SelectObject(hdcMem, hBitmap);
				hOld2 = SelectObject(hdcCompatible, hbmScreen);
				//
				for (int i = 0; i < n; i++)
				{
					double di = (double)i;
					x = (double)x0 + di * dx;
					y = (double)y0 + di * dy;
					w = (double)w0 + di * dw;
					h = (double)h0 + di * dh;
					//save the screen
					BitBlt(hdcCompatible,
						   0, 0,
						   (int)w, (int)h,
						   hdcScreen,
						   (int)x, (int)y,
						   SRCCOPY);
					//draw on screen
					StretchBlt(
					  hdcScreen,      // handle to destination DC
					  (int)x, // x-coord of destination upper-left corner
					  (int)y, // y-coord of destination upper-left corner
					  (int)w,   // width of destination rectangle
					  (int)h,  // height of destination rectangle
					  hdcMem,       // handle to source DC
					  0,  // x-coord of source upper-left corner
					  0,  // y-coord of source upper-left corner
					  width,    // width of source rectangle
					  height,   // height of source rectangle
					  SRCCOPY       // raster operation code
					);
					System.Threading.Thread.Sleep(timer);
					//restore the screen
					BitBlt(hdcScreen, (int)x, (int)y, (int)w, (int)h, hdcCompatible, 0, 0, SRCCOPY);
				}
			}
			//clean up
			SelectObject(hdcMem, hOld);
			SelectObject(hdcCompatible, hOld2);
			DeleteDC(hdcCompatible); //-h4
			DeleteDC(hdcMem); //-h3
			DeleteObject(hbmScreen); //-h2
			ReleaseDC(IntPtr.Zero, hdcScreen); //-h1
			if (removeBitmap)
			{
				DeleteObject(hBitmap);
			}
		}
		public static void ClickMouse(int x0, int y0)
		{
			InputUtil.ClickMouse(x0, y0);

		}
		public static void MoveMouse(int x0, int y0)
		{
			InputUtil.MoveMouse(x0, y0);
		}
		public static string GetWinAPIErrorMessage(int code)
		{
			return new Win32Exception(code).Message;
		}
		public static IntPtr GetCurrentCursor()
		{
			CURSORINFO CursorInfo = new CURSORINFO();
			CursorInfo.flags = 0;
			CursorInfo.hCursor = IntPtr.Zero;
			CursorInfo.cbSize = (uint)Marshal.SizeOf(CursorInfo);
			if (GetCursorInfo(ref CursorInfo))
			{
				return CursorInfo.hCursor;
			}
			return IntPtr.Zero;
		}
		#endregion
		#region kiosk
		const string REG_POLICIES = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
		const string REG_GROUPPLICY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Group Policy";
		const string REG_EXPLORER = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
		const string REG_NTLOGON = "Software\\Microsoft\\Windows NT\\CurrentVersion\\WinLogon";

		const string REGV_STR_IgnoreShiftOverride = "IgnoreShiftOverride";
		const string REGV_DW_ForceAutoLogon = "ForceAutoLogon";
		const string REGV_DW_DisableCAD = "DisableCAD";
		const string REGV_DW_DisableTaskMgr = "DisableTaskMgr";
		//
		const string REG_Longflow = "SOFTWARE\\Longflow Enterprises";
		const string REGV_DW_KioskMode = "KioskMode";
		//////////////////////////////////////////////////////////
		// OS Flags
		const UInt32 dwWin95 = 0x1;
		const UInt32 dwWin98 = 0x2;
		const UInt32 dwWin98SE = 0x4;
		const UInt32 dwWinME = 0x8;
		const UInt32 dwWinNT = 0x10;
		const UInt32 dwWin2K = 0x20;
		const UInt32 dwWinXP = 0x40;
		private static IntPtr hHookKey = IntPtr.Zero;
		//Declare the mouse hook constant.
		//For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
		const int WH_MOUSE = 7;
		const int WH_KEYBOARD = 2;
		const int WH_KEYBOARD_LL = 13;
		const int WH_MOUSE_LL = 14;

		const int SPI_SETSCREENSAVERRUNNING = 0x0061;
		const int SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING;
		/*
		 * Hook Codes
		 */
		const int HC_ACTION = 0;
		const int HC_GETNEXT = 1;
		const int HC_SKIP = 2;
		const int HC_NOREMOVE = 3;
		const int HC_NOREM = HC_NOREMOVE;
		const int HC_SYSMODALON = 4;
		const int HC_SYSMODALOFF = 5;

		const int VK_TAB = 0x09;
		const int VK_CONTROL = 0x11;
		const int VK_ESCAPE = 0x1B;
		const int VK_LWIN = 0x5B;
		const int VK_RWIN = 0x5C;
		const int VK_APPS = 0x5D;

		const int KF_ALTDOWN = 0x2000;
		const int LLKHF_ALTDOWN = (KF_ALTDOWN >> 8);
		public struct KBDLLHOOKSTRUCT
		{
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		static int LowLevelKeyboardProc(
			int nCode,     // hook code
			IntPtr wParam, // message identifier
			IntPtr lParam  // pointer to structure with message data
		)
		{
			if (nCode == HC_ACTION)
			{
				KBDLLHOOKSTRUCT pKey = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

				bool bControlKeyDown =
				 (GetAsyncKeyState(VK_CONTROL) >> ((sizeof(short) * 8) - 1)) != 0;
				// Disable CTRL+ESC 
				if (pKey.vkCode == VK_ESCAPE && bControlKeyDown)
					return 1;
				// Disable ATL+TAB 
				if (pKey.vkCode == VK_TAB && ((pKey.flags & LLKHF_ALTDOWN) != 0))
					return 1;
				// Disable ALT+ESC 
				if (pKey.vkCode == VK_ESCAPE && ((pKey.flags & LLKHF_ALTDOWN) != 0))
					return 1;
				// Disable the WINDOWS key 
				if (pKey.vkCode == VK_LWIN || pKey.vkCode == VK_RWIN || pKey.vkCode == VK_APPS)
				{
					return 1;
				}
			}
			return CallNextHookEx(hHookKey, nCode, wParam, lParam);
		}
		static HookProc keyfun;
		static IntPtr hHookTaskMan = IntPtr.Zero;
		static IntPtr hAppInst = IntPtr.Zero;
		const string Shell_traywnd = "Shell_traywnd";
		public static Int32 fnSetupKiosk()
		{
			Int32 dwRet = 0;
			EnumWindowsOS os = GetWindowsOS();
			IntPtr hwnd = FindWindow(Shell_traywnd, null);
			if (hwnd != IntPtr.Zero)
			{
				if ((int)os > (int)(EnumWindowsOS.WindowsXP))
				{
					PostMessage(hwnd, 0x5B4, 0, 0);
				}
				else
				{
					EnableWindow(hwnd, 0);
				}
			}
			if (hHookKey == IntPtr.Zero)
			{
				if ((int)os > (int)(EnumWindowsOS.WindowsNT4))
				{
					if (keyfun == null)
					{
						keyfun = new HookProc(LowLevelKeyboardProc);
					}
					using (Process curProcess = Process.GetCurrentProcess())
					{
						using (ProcessModule curModule = curProcess.MainModule)
						{
							if (hAppInst == IntPtr.Zero)
							{
								hAppInst = GetModuleHandle(curModule.ModuleName);
							}
							hHookKey = SetWindowsHookEx(
								WH_KEYBOARD_LL,        // type of hook to install
								keyfun, //LowLevelKeyboardProc,     // address of hook procedure
								hAppInst, //(HINSTANCE)hMode,    // handle to application instance
								0   // identity of thread to install hook for
								);
						}
					}

					if (hHookKey == IntPtr.Zero)
						dwRet = Marshal.GetLastWin32Error();
				}
				else
				{
					SystemParametersInfo(SPI_SCREENSAVERRUNNING, 1, IntPtr.Zero, 0);
				}
			}
			uint dwValue = 1;
			//1.
			RegistryKey hKey = Registry.CurrentUser.CreateSubKey(
				REG_POLICIES,
				 RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue("DisableLockWorkstation", dwValue, RegistryValueKind.DWord);
			hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
			hKey.SetValue("DisableChangePassword", dwValue, RegistryValueKind.DWord);
			hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
			hKey.Close();
			//2.
			hKey = Registry.CurrentUser.CreateSubKey(
				REG_GROUPPLICY,
				 RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue("DisableLockWorkstation", dwValue, RegistryValueKind.DWord);
			hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
			hKey.SetValue("DisableChangePassword", dwValue, RegistryValueKind.DWord);
			hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
			hKey.Close();
			//3.
			hKey = Registry.LocalMachine.CreateSubKey(
				REG_POLICIES,
				 RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
			hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
			hKey.Close();
			//4.
			hKey = Registry.LocalMachine.CreateSubKey(
				REG_NTLOGON,
				 RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue(REGV_DW_DisableCAD, dwValue, RegistryValueKind.DWord);
			hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
			hKey.Close();
			//5
			//6
			try
			{
				hKey = Registry.Users.CreateSubKey(REG_POLICIES, RegistryKeyPermissionCheck.ReadWriteSubTree);
				hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			catch
			{
			}
			//7
			hKey = Registry.CurrentUser.CreateSubKey(REG_EXPLORER, RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue("NoLogoff", dwValue, RegistryValueKind.DWord);
			hKey.SetValue("NoClose", dwValue, RegistryValueKind.DWord);
			hKey.SetValue("StartMenuLogOff", dwValue, RegistryValueKind.DWord);
			hKey.Close();
			//8
			hKey = Registry.LocalMachine.CreateSubKey(REG_Longflow, RegistryKeyPermissionCheck.ReadWriteSubTree);
			hKey.SetValue(REGV_DW_KioskMode, dwValue, RegistryValueKind.DWord);
			hKey.Close();
			return dwRet;
		}
		////////////////////////////////////////////////////
		public static Int32 fnExitKiosk()
		{
			Int32 nRet = 0;
			Process p = new Process();
			p.StartInfo.FileName = "explorer.exe";
			p.Start();
			p.WaitForInputIdle();
			IntPtr hwnd = FindWindow(Shell_traywnd, null);
			if (hwnd != IntPtr.Zero)
			{
				EnableWindow(hwnd, 1);
			}
			if (hHookKey != IntPtr.Zero)
			{
				if (!UnhookWindowsHookEx(hHookKey))
					nRet = Marshal.GetLastWin32Error();
				hHookKey = IntPtr.Zero;
			}
			if (hHookTaskMan != IntPtr.Zero)
			{
				if (!UnhookWindowsHookEx(hHookTaskMan))
				{
					if (nRet == 0)
						nRet = Marshal.GetLastWin32Error();
				}
				hHookTaskMan = IntPtr.Zero;
			}
			RegistryKey hKey;
			uint dwValue = 0;
			//1.
			hKey = Registry.CurrentUser.OpenSubKey(REG_POLICIES, true);
			if (hKey != null)
			{
				hKey.SetValue("DisableLockWorkstation", dwValue, RegistryValueKind.DWord);
				hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
				hKey.SetValue("DisableChangePassword", dwValue, RegistryValueKind.DWord);
				hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);

				hKey.Close();
			}
			//2.
			hKey = Registry.CurrentUser.OpenSubKey(REG_GROUPPLICY, true);
			if (hKey != null)
			{
				hKey.SetValue("DisableLockWorkstation", dwValue, RegistryValueKind.DWord);
				hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
				hKey.SetValue("DisableChangePassword", dwValue, RegistryValueKind.DWord);
				hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);

				hKey.Close();
			}
			//3
			hKey = Registry.LocalMachine.OpenSubKey(REG_POLICIES, true);
			if (hKey != null)
			{
				hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
				hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			//4.
			hKey = Registry.LocalMachine.OpenSubKey(REG_NTLOGON, true);
			if (hKey != null)
			{
				hKey.SetValue(REGV_DW_DisableCAD, dwValue, RegistryValueKind.DWord);
				hKey.SetValue("HideFastUserSwitching", dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			//5.
			//6
			hKey = Registry.Users.OpenSubKey(REG_POLICIES, true);
			if (hKey != null)
			{
				hKey.SetValue(REGV_DW_DisableTaskMgr, dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			//7
			hKey = Registry.CurrentUser.OpenSubKey(REG_EXPLORER, true);
			if (hKey != null)
			{
				hKey.SetValue("NoLogoff", dwValue, RegistryValueKind.DWord);
				hKey.SetValue("NoClose", dwValue, RegistryValueKind.DWord);
				hKey.SetValue("StartMenuLogOff", dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			//8
			hKey = Registry.LocalMachine.OpenSubKey(REG_Longflow, true);
			if (hKey != null)
			{
				hKey.SetValue(REGV_DW_KioskMode, dwValue, RegistryValueKind.DWord);
				hKey.Close();
			}
			SystemParametersInfo(SPI_SCREENSAVERRUNNING, 0, IntPtr.Zero, 0);

			return nRet;
		}
		#endregion
		#region generic util
		public static string GetRandomString(int length)
		{
			byte[] randBuffer = new byte[length];
			RandomNumberGenerator.Create().GetBytes(randBuffer);
			return System.Convert.ToBase64String(randBuffer).Remove(length);
		}
		public static string GetHash(string input, string salt, EnumPasswordHash algo)
		{
			if (!string.IsNullOrEmpty(salt))
			{
				input = string.Format(CultureInfo.InvariantCulture, "{0}{1}", salt, input);
			}
			byte[] data;
			byte[] inputBytes = Encoding.Default.GetBytes(input);
			switch (algo)
			{
				case EnumPasswordHash.MD5:
					MD5 md5Hasher = MD5.Create();
					data = md5Hasher.ComputeHash(inputBytes);
					break;
				case EnumPasswordHash.SHA1:
					SHA1 sha = new SHA1CryptoServiceProvider();
					data = sha.ComputeHash(inputBytes);
					break;
				case EnumPasswordHash.SHA256:
					SHA256 sha256 = new SHA256Managed();
					data = sha256.ComputeHash(inputBytes);
					break;
				case EnumPasswordHash.SHA384:
					SHA384 sha384 = new SHA384Managed();
					data = sha384.ComputeHash(inputBytes);
					break;
				case EnumPasswordHash.SHA512:
					SHA512 sha512 = new SHA512Managed();
					data = sha512.ComputeHash(inputBytes);
					break;
				default:
					data = new byte[] { };
					break;
			}
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
			}
			return sBuilder.ToString();
		}
		#endregion
		#region reboot
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct TokPriv1Luid
		{
			public int Count;
			public long Luid;
			public int Attr;
		}
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GetCurrentProcess();
		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
		[DllImport("advapi32.dll", SetLastError = true)]
		internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool ExitWindowsEx(int flg, int rea);

		internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
		internal const int TOKEN_QUERY = 0x00000008;
		internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
		internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
		internal const int EWX_LOGOFF = 0x00000000;
		internal const int EWX_SHUTDOWN = 0x00000001;
		internal const int EWX_REBOOT = 0x00000002;
		internal const int EWX_FORCE = 0x00000004;
		internal const int EWX_POWEROFF = 0x00000008;
		internal const int EWX_FORCEIFHUNG = 0x00000010;

		public static Thread thread1;
		public static int DoExitWin(int flg)
		{
			int ret = 0;
			bool ok;
			TokPriv1Luid tp;
			IntPtr hproc = GetCurrentProcess();
			IntPtr htok = IntPtr.Zero;
			ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
			if (!ok)
			{
				ret = 1;
			}
			tp.Count = 1;
			tp.Luid = 0;
			tp.Attr = SE_PRIVILEGE_ENABLED;
			ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
			if (!ok)
			{
				ret |= 2;
			}
			ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
			if (!ok)
			{
				ret |= 4;
			}
			ok = ExitWindowsEx(flg, 0);
			if (!ok)
			{
				ret |= 8;
			}
			return ret;
		}
		public static int Reboot()
		{
			return DoExitWin(EWX_REBOOT | EWX_FORCE);
		}
		private static void kill()
		{
			Process p = Process.GetCurrentProcess();
			p.Close();
			try
			{
				p = Process.GetCurrentProcess();
				if (p != null)
				{
					if (!p.HasExited)
					{
						p.Kill();
					}
				}
			}
			catch
			{
			}
		}
		#endregion
	}
	public enum EnumPasswordHash
	{
		MD5 = 0,
		SHA1 = 1,
		SHA256 = 2,
		SHA384 = 3,
		SHA512 = 4
	}
}
