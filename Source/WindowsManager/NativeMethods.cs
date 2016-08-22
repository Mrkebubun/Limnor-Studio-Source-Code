/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace Limnor.Windows
{
	static class NativeMethods
	{
		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int AttachThreadInput(int idAttach, int idAttachTo, int fAttach);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

		[DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
		public static extern Int32 GetCurrentWin32ThreadId();

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern bool SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr SetFocus(IntPtr hwnd);

		[DllImport("user32", SetLastError = true)]
		public static extern bool GetCursorPos(ref POINT point);
		[DllImport("user32", SetLastError = true)]
		public static extern IntPtr WindowFromPoint(POINT point);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		internal static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll")]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("gdi32.dll")]
		internal static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
		[DllImport("user32.dll")]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);



		//
		/// <summary>
		/// 
		/// Locks the workstation's display. Locking a workstation protects it from unauthorized use.
		/// 
		/// </summary>
		/// 
		/// <returns>
		/// 
		/// If the function succeeds, the return value is true. Because the function executes asynchronously, a true return value indicates that the operation has been initiated. It does not indicate whether the workstation has been successfully locked.
		/// 
		/// If the function fails, the return value is false. To get extended error information, call GetLastError.
		/// 
		/// </returns>
		/// 
		[DllImport("User32.Dll", EntryPoint = "LockWorkStation"), Description("Locks the workstation's display. Locking a workstation protects it from unauthorized use.")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LockWorkStation();
		//
		[StructLayout(LayoutKind.Sequential)]
		internal struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		};

		[DllImport("USER32.DLL")]
		internal static extern bool GetLastInputInfo(ref LASTINPUTINFO ii);

		//
		const int MAXTITLE = 255;
		private delegate bool EnumDelegate(IntPtr hWnd, int lParam);
		public enum EnumTitleSearchType { StartWith = 0, Contains = 1, EndWith = 2 }

		[DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
		ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool EnumDesktopWindows(IntPtr hDesktop,
		EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "EnumWindows",
		ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool EnumWindows(EnumDelegate lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "EnumChildWindows",
		ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool EnumChildWindows(IntPtr hWndParent, EnumDelegate lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "GetWindowText",
		ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int _GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

		[DllImport("user32.dll", EntryPoint = "GetParent", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "GetWindowLong", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetDlgCtrlID", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetDlgCtrlID(IntPtr hwndCtl);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int RegisterWindowMessage(
			[MarshalAs(UnmanagedType.LPWStr)]
			string msg);

		[StructLayout(LayoutKind.Sequential)]
		public struct COPYDATASTRUCT
		{
			public int dwData;
			public int cbData;
			public IntPtr lpData;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HOOK_MSG
		{
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
		}
		public enum Msgs : int
		{
			WM_NULL = 0x0000,
			WM_CREATE = 0x0001,
			WM_DESTROY = 0x0002,
			WM_MOVE = 0x0003,
			WM_SIZE = 0x0005,
			WM_ACTIVATE = 0x0006,
			WM_SETFOCUS = 0x0007,
			WM_KILLFOCUS = 0x0008,
			WM_ENABLE = 0x000A,
			WM_SETREDRAW = 0x000B,
			WM_SETTEXT = 0x000C,
			WM_GETTEXT = 0x000D,
			WM_GETTEXTLENGTH = 0x000E,
			WM_PAINT = 0x000F,
			WM_CLOSE = 0x0010,
			WM_QUERYENDSESSION = 0x0011,
			WM_QUIT = 0x0012,
			WM_QUERYOPEN = 0x0013,
			WM_ERASEBKGND = 0x0014,
			WM_SYSCOLORCHANGE = 0x0015,
			WM_ENDSESSION = 0x0016,
			WM_SHOWWINDOW = 0x0018,
			WM_WININICHANGE = 0x001A,
			WM_SETTINGCHANGE = 0x001A,
			WM_DEVMODECHANGE = 0x001B,
			WM_ACTIVATEAPP = 0x001C,
			WM_FONTCHANGE = 0x001D,
			WM_TIMECHANGE = 0x001E,
			WM_CANCELMODE = 0x001F,
			WM_SETCURSOR = 0x0020,
			WM_MOUSEACTIVATE = 0x0021,
			WM_CHILDACTIVATE = 0x0022,
			WM_QUEUESYNC = 0x0023,
			WM_GETMINMAXINFO = 0x0024,
			WM_PAINTICON = 0x0026,
			WM_ICONERASEBKGND = 0x0027,
			WM_NEXTDLGCTL = 0x0028,
			WM_SPOOLERSTATUS = 0x002A,
			WM_DRAWITEM = 0x002B,
			WM_MEASUREITEM = 0x002C,
			WM_DELETEITEM = 0x002D,
			WM_VKEYTOITEM = 0x002E,
			WM_CHARTOITEM = 0x002F,
			WM_SETFONT = 0x0030,
			WM_GETFONT = 0x0031,
			WM_SETHOTKEY = 0x0032,
			WM_GETHOTKEY = 0x0033,
			WM_QUERYDRAGICON = 0x0037,
			WM_COMPAREITEM = 0x0039,
			WM_GETOBJECT = 0x003D,
			WM_COMPACTING = 0x0041,
			WM_COMMNOTIFY = 0x0044,
			WM_WINDOWPOSCHANGING = 0x0046,
			WM_WINDOWPOSCHANGED = 0x0047,
			WM_POWER = 0x0048,
			WM_COPYDATA = 0x004A,
			WM_CANCELJOURNAL = 0x004B,
			WM_NOTIFY = 0x004E,
			WM_INPUTLANGCHANGEREQUEST = 0x0050,
			WM_INPUTLANGCHANGE = 0x0051,
			WM_TCARD = 0x0052,
			WM_HELP = 0x0053,
			WM_USERCHANGED = 0x0054,
			WM_NOTIFYFORMAT = 0x0055,
			WM_CONTEXTMENU = 0x007B,
			WM_STYLECHANGING = 0x007C,
			WM_STYLECHANGED = 0x007D,
			WM_DISPLAYCHANGE = 0x007E,
			WM_GETICON = 0x007F,
			WM_SETICON = 0x0080,
			WM_NCCREATE = 0x0081,
			WM_NCDESTROY = 0x0082,
			WM_NCCALCSIZE = 0x0083,
			WM_NCHITTEST = 0x0084,
			WM_NCPAINT = 0x0085,
			WM_NCACTIVATE = 0x0086,
			WM_GETDLGCODE = 0x0087,
			WM_SYNCPAINT = 0x0088,
			WM_NCMOUSEMOVE = 0x00A0,
			WM_NCLBUTTONDOWN = 0x00A1,
			WM_NCLBUTTONUP = 0x00A2,
			WM_NCLBUTTONDBLCLK = 0x00A3,
			WM_NCRBUTTONDOWN = 0x00A4,
			WM_NCRBUTTONUP = 0x00A5,
			WM_NCRBUTTONDBLCLK = 0x00A6,
			WM_NCMBUTTONDOWN = 0x00A7,
			WM_NCMBUTTONUP = 0x00A8,
			WM_NCMBUTTONDBLCLK = 0x00A9,
			WM_KEYDOWN = 0x0100,
			WM_KEYUP = 0x0101,
			WM_CHAR = 0x0102,
			WM_DEADCHAR = 0x0103,
			WM_SYSKEYDOWN = 0x0104,
			WM_SYSKEYUP = 0x0105,
			WM_SYSCHAR = 0x0106,
			WM_SYSDEADCHAR = 0x0107,
			WM_KEYLAST = 0x0108,
			WM_IME_STARTCOMPOSITION = 0x010D,
			WM_IME_ENDCOMPOSITION = 0x010E,
			WM_IME_COMPOSITION = 0x010F,
			WM_IME_KEYLAST = 0x010F,
			WM_INITDIALOG = 0x0110,
			WM_COMMAND = 0x0111,
			WM_SYSCOMMAND = 0x0112,
			WM_TIMER = 0x0113,
			WM_HSCROLL = 0x0114,
			WM_VSCROLL = 0x0115,
			WM_INITMENU = 0x0116,
			WM_INITMENUPOPUP = 0x0117,
			WM_MENUSELECT = 0x011F,
			WM_MENUCHAR = 0x0120,
			WM_ENTERIDLE = 0x0121,
			WM_MENURBUTTONUP = 0x0122,
			WM_MENUDRAG = 0x0123,
			WM_MENUGETOBJECT = 0x0124,
			WM_UNINITMENUPOPUP = 0x0125,
			WM_MENUCOMMAND = 0x0126,
			WM_CTLCOLORMSGBOX = 0x0132,
			WM_CTLCOLOREDIT = 0x0133,
			WM_CTLCOLORLISTBOX = 0x0134,
			WM_CTLCOLORBTN = 0x0135,
			WM_CTLCOLORDLG = 0x0136,
			WM_CTLCOLORSCROLLBAR = 0x0137,
			WM_CTLCOLORSTATIC = 0x0138,
			WM_MOUSEMOVE = 0x0200,
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_LBUTTONDBLCLK = 0x0203,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_RBUTTONDBLCLK = 0x0206,
			WM_MBUTTONDOWN = 0x0207,
			WM_MBUTTONUP = 0x0208,
			WM_MBUTTONDBLCLK = 0x0209,
			WM_MOUSEWHEEL = 0x020A,
			WM_PARENTNOTIFY = 0x0210,
			WM_ENTERMENULOOP = 0x0211,
			WM_EXITMENULOOP = 0x0212,
			WM_NEXTMENU = 0x0213,
			WM_SIZING = 0x0214,
			WM_CAPTURECHANGED = 0x0215,
			WM_MOVING = 0x0216,
			WM_DEVICECHANGE = 0x0219,
			WM_MDICREATE = 0x0220,
			WM_MDIDESTROY = 0x0221,
			WM_MDIACTIVATE = 0x0222,
			WM_MDIRESTORE = 0x0223,
			WM_MDINEXT = 0x0224,
			WM_MDIMAXIMIZE = 0x0225,
			WM_MDITILE = 0x0226,
			WM_MDICASCADE = 0x0227,
			WM_MDIICONARRANGE = 0x0228,
			WM_MDIGETACTIVE = 0x0229,
			WM_MDISETMENU = 0x0230,
			WM_ENTERSIZEMOVE = 0x0231,
			WM_EXITSIZEMOVE = 0x0232,
			WM_DROPFILES = 0x0233,
			WM_MDIREFRESHMENU = 0x0234,
			WM_IME_SETCONTEXT = 0x0281,
			WM_IME_NOTIFY = 0x0282,
			WM_IME_CONTROL = 0x0283,
			WM_IME_COMPOSITIONFULL = 0x0284,
			WM_IME_SELECT = 0x0285,
			WM_IME_CHAR = 0x0286,
			WM_IME_REQUEST = 0x0288,
			WM_IME_KEYDOWN = 0x0290,
			WM_IME_KEYUP = 0x0291,
			WM_MOUSEHOVER = 0x02A1,
			WM_MOUSELEAVE = 0x02A3,
			WM_CUT = 0x0300,
			WM_COPY = 0x0301,
			WM_PASTE = 0x0302,
			WM_CLEAR = 0x0303,
			WM_UNDO = 0x0304,
			WM_RENDERFORMAT = 0x0305,
			WM_RENDERALLFORMATS = 0x0306,
			WM_DESTROYCLIPBOARD = 0x0307,
			WM_DRAWCLIPBOARD = 0x0308,
			WM_PAINTCLIPBOARD = 0x0309,
			WM_VSCROLLCLIPBOARD = 0x030A,
			WM_SIZECLIPBOARD = 0x030B,
			WM_ASKCBFORMATNAME = 0x030C,
			WM_CHANGECBCHAIN = 0x030D,
			WM_HSCROLLCLIPBOARD = 0x030E,
			WM_QUERYNEWPALETTE = 0x030F,
			WM_PALETTEISCHANGING = 0x0310,
			WM_PALETTECHANGED = 0x0311,
			WM_HOTKEY = 0x0312,
			WM_PRINT = 0x0317,
			WM_PRINTCLIENT = 0x0318,
			WM_HANDHELDFIRST = 0x0358,
			WM_HANDHELDLAST = 0x035F,
			WM_AFXFIRST = 0x0360,
			WM_AFXLAST = 0x037F,
			WM_PENWINFIRST = 0x0380,
			WM_PENWINLAST = 0x038F,
			WM_APP = 0x8000,
			WM_USER = 0x0400,
		}

		public const int PROCESS_VM_OPERATION = 0x008;
		public const int PROCESS_VM_WRITE = 0x020;
		public const int PROCESS_VM_READ = 0x10;

		public const uint MEM_COMMIT = 0x1000;
		public const uint MEM_RELEASE = 0x8000;
		public const uint PAGE_READWRITE = 0x04;

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd,
			out int lpdwProcessId);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess,
			[MarshalAs(UnmanagedType.Bool)]
			bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);

		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
			[MarshalAs(UnmanagedType.LPWStr)]
			string buffer,
			int dwSize,
			out int lpNumberOfBytesWritten);

		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
		  int[] buffer, int dwSize, out int lpNumberOfBytesWritten);

		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
			int dwSize, uint flAllocationType, uint flProtect);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		public static void SetWindowRect(IntPtr hWnd, Rectangle rect)
		{
			SetWindowPos(hWnd, HWND_TOP, rect.X, rect.Y, rect.Width, rect.Height, SetWindowPosFlags.DoNotChangeOwnerZOrder | SetWindowPosFlags.IgnoreZOrder);
		}
		public static void SetWindowSize(IntPtr hWnd, Size size)
		{
			SetWindowPos(hWnd, HWND_TOP, 0, 0, size.Width, size.Height, SetWindowPosFlags.DoNotChangeOwnerZOrder | SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.IgnoreMove);
		}
		public static void SetWindowLocation(IntPtr hWnd, Point p)
		{
			SetWindowPos(hWnd, HWND_TOP, p.X, p.Y, 0, 0, SetWindowPosFlags.DoNotChangeOwnerZOrder | SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.IgnoreResize);
		}
		static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		static readonly IntPtr HWND_TOP = new IntPtr(0);
		static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[Flags()]
		private enum SetWindowPosFlags : uint
		{
			/// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
			/// the system posts the request to the thread that owns the window. This prevents the calling thread from 
			/// blocking its execution while other threads process the request.</summary>
			/// <remarks>SWP_ASYNCWINDOWPOS</remarks>
			SynchronousWindowPosition = 0x4000,
			/// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
			/// <remarks>SWP_DEFERERASE</remarks>
			DeferErase = 0x2000,
			/// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
			/// <remarks>SWP_DRAWFRAME</remarks>
			DrawFrame = 0x0020,
			/// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
			/// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
			/// is sent only when the window's size is being changed.</summary>
			/// <remarks>SWP_FRAMECHANGED</remarks>
			FrameChanged = 0x0020,
			/// <summary>Hides the window.</summary>
			/// <remarks>SWP_HIDEWINDOW</remarks>
			HideWindow = 0x0080,
			/// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
			/// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
			/// parameter).</summary>
			/// <remarks>SWP_NOACTIVATE</remarks>
			DoNotActivate = 0x0010,
			/// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
			/// contents of the client area are saved and copied back into the client area after the window is sized or 
			/// repositioned.</summary>
			/// <remarks>SWP_NOCOPYBITS</remarks>
			DoNotCopyBits = 0x0100,
			/// <summary>Retains the current position (ignores X and Y parameters).</summary>
			/// <remarks>SWP_NOMOVE</remarks>
			IgnoreMove = 0x0002,
			/// <summary>Does not change the owner window's position in the Z order.</summary>
			/// <remarks>SWP_NOOWNERZORDER</remarks>
			DoNotChangeOwnerZOrder = 0x0200,
			/// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
			/// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
			/// window uncovered as a result of the window being moved. When this flag is set, the application must 
			/// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
			/// <remarks>SWP_NOREDRAW</remarks>
			DoNotRedraw = 0x0008,
			/// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
			/// <remarks>SWP_NOREPOSITION</remarks>
			DoNotReposition = 0x0200,
			/// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
			/// <remarks>SWP_NOSENDCHANGING</remarks>
			DoNotSendChangingEvent = 0x0400,
			/// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
			/// <remarks>SWP_NOSIZE</remarks>
			IgnoreResize = 0x0001,
			/// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
			/// <remarks>SWP_NOZORDER</remarks>
			IgnoreZOrder = 0x0004,
			/// <summary>Displays the window.</summary>
			/// <remarks>SWP_SHOWWINDOW</remarks>
			ShowWindow = 0x0040,
		}

		///
		public static string GetWindowText(IntPtr hWnd)
		{
			StringBuilder strbTitle = new StringBuilder(MAXTITLE);
			int nLength = _GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
			strbTitle.Length = nLength;
			return strbTitle.ToString();
		}
		class WindowSearchData
		{
			private int _id;
			private string _searchText;
			private EnumTitleSearchType _searchType;
			public WindowSearchData(EnumTitleSearchType searchType, string search)
			{
				_id = Guid.NewGuid().GetHashCode();
				_searchType = searchType;
				_searchText = search;
				WindowFound = IntPtr.Zero;
			}
			public int ID
			{
				get
				{
					return _id;
				}
			}
			public string SearchText
			{
				get
				{
					return _searchText;
				}
			}
			public EnumTitleSearchType SearchType
			{
				get
				{
					return _searchType;
				}
			}
			public IntPtr WindowFound { get; set; }
		}
		private static Dictionary<int, WindowSearchData> _windowSearchResults;
		public static IntPtr FindWindowByTitle(string title, EnumTitleSearchType searchType)
		{
			WindowSearchData search = new WindowSearchData(searchType, title);
			IntPtr p = (IntPtr)search.ID;
			if (_windowSearchResults == null)
			{
				_windowSearchResults = new Dictionary<int, WindowSearchData>();
			}
			_windowSearchResults.Add(search.ID, search);
			EnumWindows(enumerateWindowsProcess, p);
			_windowSearchResults.Remove(search.ID);
			return search.WindowFound;
		}
		private static Dictionary<int, List<WindowControl>> _childSearchResults;
		public static List<WindowControl> GetChildWindows(IntPtr hWndParent)
		{
			List<WindowControl> ret = new List<WindowControl>();
			int id = Guid.NewGuid().GetHashCode();
			if (_childSearchResults == null)
			{
				_childSearchResults = new Dictionary<int, List<WindowControl>>();
			}
			_childSearchResults.Add(id, ret);
			bool b = EnumChildWindows(hWndParent, enumerateChilWindowsProcess, (IntPtr)id);
			if (b)
			{
			}
			return ret;
		}
		private static bool enumerateChilWindowsProcess(IntPtr hWnd, int lParam)
		{
			List<WindowControl> ret;
			if (_childSearchResults.TryGetValue(lParam, out ret))
			{
				WindowControl wc = new WindowControl();
				wc.ControlHandle = hWnd;
				ret.Add(wc);
				return true;
			}
			else
			{
				return false;
			}
		}
		private static bool enumerateWindowsProcess(IntPtr hWnd, int lParam)
		{
			WindowSearchData search;
			if (_windowSearchResults.TryGetValue(lParam, out search))
			{
				string text = GetWindowText(hWnd);
				switch (search.SearchType)
				{
					case EnumTitleSearchType.Contains:
						if (text.Contains(search.SearchText))
						{
							search.WindowFound = hWnd;
							return false;
						}
						break;
					case EnumTitleSearchType.EndWith:
						if (text.EndsWith(search.SearchText, StringComparison.Ordinal))
						{
							search.WindowFound = hWnd;
							return false;
						}
						break;
					case EnumTitleSearchType.StartWith:
						if (text.StartsWith(search.SearchText, StringComparison.Ordinal))
						{
							search.WindowFound = hWnd;
							return false;
						}
						break;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int x;
		public int y;
	}
}
