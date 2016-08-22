/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using VPL;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using WindowsUtility;
using System.Drawing.Printing;
using System.IO;
using System.Globalization;
using Microsoft.Win32;

namespace Limnor.Windows
{
	[ToolboxBitmapAttribute(typeof(WindowsManager), "Resources.winForm.bmp")]
	[Description("This object provides Windows Management operations. If this component is used by a 32-bit application then the events of this component work for windows of 32-bit applications. If this component is used by a 64-bit application then the events work for windows from 64-bit applications.")]
	public class WindowsManager : IComponent
	{
		#region fields and constructors
		private Win32Exception _lastError;
		private IntPtr _hWnd;
		private ListenerForm _listener;
		private WindowControl[] _controls;
		private bool _enableEvents;
		public WindowsManager()
		{
			_enableEvents = true;
		}
		public WindowsManager(IContainer container)
		{
			if (container != null)
				container.Add(this);
			_enableEvents = true;
		}
		#endregion

		#region private methods
		private Bitmap CaptureControlShot(Control control)
		{
			// get control bounding rectangle
			Rectangle bounds = control.Bounds;

			// create the new bitmap that holds the image of the control
			Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

			// copy controls image data to the bitmaps graphics device
			using (Graphics gr = Graphics.FromImage(bitmap))
			{
				gr.CopyFromScreen(control.Location, Point.Empty, bounds.Size);
			}

			return bitmap;
		}
		#endregion

		#region Static Methods
		[Description("It forms a full file path using the specified filename and the common application data path that is used by all users")]
		public static string GetCommonAppDataFilePath(string filename)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), filename);
		}
		[Description("Start rebooting the computer")]
		public static int Reboot()
		{
			return WinUtil.Reboot();
		}
		[Description("It creates a shortcut. filename is a full path of the shortcut file to be created; targetPath is a full path of a program to be executed by the shortcut; workingDirection is a folder acts as a working folder of the program to be invoked by the shortcut.")]
		public static void CreateShort(string fileName, string targetPath, string arguments, string workingDirectory, string description, string hotkey, string iconPath)
		{
			Shortcut.Create(fileName, targetPath, arguments, workingDirectory, description, hotkey, iconPath);
		}
		[Description("It creates a shortcut on the current desktop. filename is a file name of the shortcut file to be created and placed on the current desktop; targetPath is a full path of a program to be executed by the shortcut; workingDirection is a folder acts as a working folder of the program to be invoked by the shortcut.")]
		public static void CreateShortOnCurrentDesktop(string fileName, string targetPath, string arguments, string workingDirectory, string description, string hotkey, string iconPath)
		{
			string lnk = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
			Shortcut.Create(lnk, targetPath, arguments, workingDirectory, description, hotkey, iconPath);
		}
		[Description("It creates a shortcut on the common desktop. filename is a file name of the shortcut file to be created and placed on the common desktop; targetPath is a full path of a program to be executed by the shortcut; workingDirection is a folder acts as a working folder of the program to be invoked by the shortcut.")]
		public static void CreateShortOnCommonDesktop(string fileName, string targetPath, string arguments, string workingDirectory, string description, string hotkey, string iconPath)
		{
			string sRegShellPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(sRegShellPath, false);
			string commonDesktop = regKey.GetValue("Common Desktop") as string;
			string lnk = System.IO.Path.Combine(commonDesktop, fileName);
			Shortcut.Create(lnk, targetPath, arguments, workingDirectory, description, hotkey, iconPath);
		}
		#endregion

		#region Events
		[Description("Occurs when a window belonging to a different application than the active window is about to be activated. The message is sent to the application whose window is being activated and to the application whose window is being deactivated.")]
		public event EventHandlerApplicationActivate ApplicationActivate;
		[Description("Occurs to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately. ")]
		public event EventHandlerWindowActivate WindowActivate;
		[Description("Occurs when a window has gained the keyboard focus. ")]
		public event EventHandlerWindowFocus WindowGotFocus;
		[Description("Occurs when the user chooses a command from the Window menu (formerly known as the system or control menu) or when the user chooses the maximize button, minimize button, restore button, or close button.")]
		public event EventHandlerSystemCommand SystemCommand;
		[Description("Occurs one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. ")]
		public event EventHandler EnterSizeMove;
		[Description("Occurs one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. ")]
		public event EventHandler ExitSizeMove;
		[Description("Occurs to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position.")]
		public event EventHandlerWindowSizing WindowSizing;
		[Description("Occurs to a window after its size has changed.")]
		public event EventHandlerWindowResized WindowResized;
		[Description("Occurs to a window immediately before it loses the keyboard focus. ")]
		public event EventHandlerWindowFocus WindowKillFocus;
		[Description("Occurs to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.")]
		public event EventHandlerWindowPositionChange WindowPositionChanging;
		[Description("Occurs to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.")]
		public event EventHandlerWindowPositionChange WindowPositionChanged;
		[Description("Occurs when the window is moved or resized")]
		public event EventHandler WindowResizeOrMove;
		#endregion

		#region Non-browsable Methods
		[Browsable(false)]
		public void OnWindowEvent(int msg, string[] ss)
		{

			const int WM_ACTIVATEAPP = 0x001C;
			const int WM_ACTIVATE = 0x0006;
			const int WM_SETFOCUS = 0x0007;
			const int WM_SYSCOMMAND = 0x0112;
			const int WM_ENTERSIZEMOVE = 0x0231;
			const int WM_EXITSIZEMOVE = 0x0232;
			const int WM_SIZING = 0x0214;
			const int WM_SIZE = 0x0005;
			const int WM_KILLFOCUS = 0x0008;
			const int WM_WINDOWPOSCHANGING = 0x0046;
			const int WM_WINDOWPOSCHANGED = 0x0047;
			switch (msg)
			{
				case WM_ACTIVATEAPP:
					if (ApplicationActivate != null)
					{
						if (ss.Length > 2)
						{
							bool b = (ss[1].Length == 1 && ss[1][0] == '1');
							int id = int.Parse(ss[2]);
							ApplicationActivate(this, new EventArgsApplicationActivate(b, id));
						}
					}
					break;
				case WM_ACTIVATE:
					if (WindowActivate != null)
					{
						if (ss.Length > 3)
						{
							bool bMinimize = (ss[1].Length == 1 && ss[1][0] == '1');
							int n = int.Parse(ss[2]);
							IntPtr id = new IntPtr(int.Parse(ss[3]));
							bool b2 = (n == 2);
							bool b1 = (n != 0);
							WindowActivate(this, new EventArgsWindowActivate(b1, b2, bMinimize, id));
						}
					}
					break;
				case WM_SETFOCUS:
					if (WindowGotFocus != null)
					{
						if (ss.Length > 1)
						{
							IntPtr id = new IntPtr(int.Parse(ss[1]));
							WindowGotFocus(this, new EventArgsWindowFocus(id, true));
						}
					}
					break;
				case WM_SYSCOMMAND:
					if (SystemCommand != null)
					{
						if (ss.Length > 1)
						{
							int n = int.Parse(ss[1]);
							EnumSystemCommand cmd = (EnumSystemCommand)n;
							SystemCommand(this, new EventArgsSystemCommand(cmd));
						}
					}
					break;
				case WM_ENTERSIZEMOVE:
					if (EnterSizeMove != null)
					{
						EnterSizeMove(this, EventArgs.Empty);
					}
					break;
				case WM_EXITSIZEMOVE:
					if (ExitSizeMove != null)
					{
						ExitSizeMove(this, EventArgs.Empty);
					}
					break;
				case WM_SIZING:
					if (WindowSizing != null)
					{
						if (ss.Length > 5)
						{
							int n = int.Parse(ss[1]);
							EnumSizeEdge edge = (EnumSizeEdge)n;
							int x = int.Parse(ss[2]);
							int y = int.Parse(ss[3]);
							int w = int.Parse(ss[4]);
							int h = int.Parse(ss[5]);
							WindowSizing(this, new EventArgsWindowSizing(new Rectangle(x, y, w, h), edge));
						}

					}
					break;
				case WM_SIZE:
					if (WindowResized != null || WindowResizeOrMove != null)
					{
						if (ss.Length > 3)
						{
							if (WindowResized != null)
							{
								int n = int.Parse(ss[1]);
								EnumWindowSizeType edge = (EnumWindowSizeType)n;
								int w = int.Parse(ss[2]);
								int h = int.Parse(ss[3]);
								WindowResized(this, new EventArgsWindowSized(edge, new Size(w, h)));
							}
							if (WindowResizeOrMove != null)
							{
								WindowResizeOrMove(this, EventArgs.Empty);
							}
						}
					}
					break;
				case WM_KILLFOCUS:
					if (WindowKillFocus != null)
					{
						if (ss.Length > 1)
						{
							IntPtr id = new IntPtr(int.Parse(ss[1]));
							WindowKillFocus(this, new EventArgsWindowFocus(id, false));
						}
					}
					break;
				case WM_WINDOWPOSCHANGING:
					if (WindowPositionChanging != null)
					{
						if (ss.Length > 7)
						{
							int x = int.Parse(ss[1]);
							int y = int.Parse(ss[2]);
							int w = int.Parse(ss[3]);
							int h = int.Parse(ss[4]);
							int f = int.Parse(ss[5]);
							EnumWindowPositionChangeStyle changeStyle = (EnumWindowPositionChangeStyle)f;
							IntPtr hnd = new IntPtr(int.Parse(ss[6]));
							IntPtr hAfter = IntPtr.Zero;
							EnumWindowRelativePostion rp = EnumWindowRelativePostion.HWND_TOP;
							int nf = int.Parse(ss[7]);
							if (nf < 2)
							{
								rp = (EnumWindowRelativePostion)nf;
							}
							else
							{
								hAfter = new IntPtr(nf);
							}
							WindowPositionChanging(this, new EventArgsWindowPositionChange(new Point(x, y), new Size(w, h), changeStyle, hnd, hAfter, rp));
						}
					}
					break;
				case WM_WINDOWPOSCHANGED:
					if (WindowPositionChanged != null || WindowResizeOrMove != null)
					{
						if (ss.Length > 7)
						{
							int x = int.Parse(ss[1]);
							int y = int.Parse(ss[2]);
							int w = int.Parse(ss[3]);
							int h = int.Parse(ss[4]);
							int f = int.Parse(ss[5]);
							EnumWindowPositionChangeStyle changeStyle = (EnumWindowPositionChangeStyle)f;
							IntPtr hnd = new IntPtr(int.Parse(ss[6]));
							IntPtr hAfter = IntPtr.Zero;
							EnumWindowRelativePostion rp = EnumWindowRelativePostion.HWND_TOP;
							int nf = int.Parse(ss[7]);
							if (nf < 2)
							{
								rp = (EnumWindowRelativePostion)nf;
							}
							else
							{
								hAfter = new IntPtr(nf);
							}
							EventArgsWindowPositionChange e = new EventArgsWindowPositionChange(new Point(x, y), new Size(w, h), changeStyle, hnd, hAfter, rp);
							if (WindowPositionChanged != null)
							{
								WindowPositionChanged(this, e);
							}
							if (WindowResizeOrMove != null)
							{
								if (e.IsResizedOrMoved)
								{
									WindowResizeOrMove(this, EventArgs.Empty);
								}
							}
						}
					}
					break;
			}
		}
		[Browsable(false)]
		public void SetError(string message)
		{
			_lastError = new Win32Exception(0, message);
		}
		#endregion

		#region Methods
		[Description("Gets color of the specified point in the screen")]
		public Color GetPointColor(Point p)
		{
			IntPtr hwnd = NativeMethods.GetDesktopWindow();
			IntPtr pDc = NativeMethods.GetWindowDC(hwnd);
			uint pi = NativeMethods.GetPixel(pDc, p.X, p.Y);
			Color c = Color.FromArgb((int)pi);
			NativeMethods.ReleaseDC(hwnd, pDc);
			return c;
		}
		[Description("Gets color of the specified point in the screen")]
		public Color GetPointColor(int x, int y)
		{
			IntPtr hwnd = NativeMethods.GetDesktopWindow();
			IntPtr pDc = NativeMethods.GetWindowDC(hwnd);
			uint pi = NativeMethods.GetPixel(pDc, x, y);
			Color c = Color.FromArgb((int)pi);
			NativeMethods.ReleaseDC(hwnd, pDc);
			return c;
		}
		[Description("Gets color of the cursor point in the screen")]
		public Color GetColorAtCursorPoint()
		{
			return GetPointColor(Cursor.Position);
		}
		[Description("Click mouse at its current location")]
		public void ClickMouse()
		{
			InputUtil.ClickMouse();
		}
		[Description("Move mouse to a specified location and click the mouse")]
		public void MoveClickMouse(Point p)
		{
			InputUtil.ClickMouse(p.X, p.Y);
		}
		[Description("The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation. The FoundWindow property is set to the return value of this call.")]
		public IntPtr FindForegroundWindow()
		{
			FoundWindow = NativeMethods.GetForegroundWindow();
			return _hWnd;
		}
		[Description("Copies the text of the FoundWindow's title bar (if it has one) into a buffer.")]
		public string GetFoundWindowText()
		{
			if (_hWnd != IntPtr.Zero)
			{
				int n = NativeMethods.GetWindowTextLength(_hWnd);
				if (n > 0)
				{
					StringBuilder sb = new StringBuilder(n + 4);
					n = NativeMethods.GetWindowText(_hWnd, sb, n + 2);
					string s = sb.ToString();
					return s;
				}
			}
			return string.Empty;
		}
		[Description("Send keys to the FoundWindow")]
		public void SendKeys(string keys)
		{
			if (_hWnd != IntPtr.Zero)
			{
				int pid;
				int threadId = NativeMethods.GetWindowThreadProcessId(_hWnd, out pid);
				int tid = NativeMethods.GetCurrentWin32ThreadId();
				if (threadId != tid)
				{
					NativeMethods.AttachThreadInput(threadId, tid, 1);
				}
				NativeMethods.SetForegroundWindow(_hWnd);
				NativeMethods.SetFocus(_hWnd);

				try
				{
					System.Windows.Forms.SendKeys.Send(keys);
				}
				catch
				{
				}
				finally
				{
					if (threadId != tid)
					{
						NativeMethods.AttachThreadInput(threadId, tid, 0);
					}
				}
			}
		}
		[Description("Set input focus to the FoundWindow")]
		public void SetFocus()
		{
			int pid;
			int threadId = NativeMethods.GetWindowThreadProcessId(_hWnd, out pid);
			int tid = NativeMethods.GetCurrentWin32ThreadId();
			if (threadId != tid)
			{
				NativeMethods.AttachThreadInput(threadId, tid, 1);
			}
			NativeMethods.SetForegroundWindow(_hWnd);
			NativeMethods.SetFocus(_hWnd);
			if (threadId != tid)
			{
				NativeMethods.AttachThreadInput(threadId, tid, 0);
			}
		}
		[Description("Retrieves the dimensions of the bounding rectangle of the FoundWindow. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.")]
		public Rectangle GetFoundWindowRect()
		{
			Rectangle r = Rectangle.Empty;
			if (_hWnd != IntPtr.Zero)
			{
				Limnor.Windows.NativeMethods.RECT r0 = new NativeMethods.RECT();
				if (NativeMethods.GetWindowRect(_hWnd, ref r0))
				{
					r = new Rectangle(r0.Left, r0.Top, r0.Right - r0.Left, r0.Bottom - r0.Top);
				}
				else
				{
				}
			}
			return r;
		}
		[Description("Locks the workstation's display. Locking a workstation protects it from unauthorized use.")]
		public static bool LockWorkStation()
		{
			return NativeMethods.LockWorkStation();
		}
		[Description("Get the idle time ")]
		public static long GetIdleTime()
		{
			Limnor.Windows.NativeMethods.LASTINPUTINFO lastInPut = new Limnor.Windows.NativeMethods.LASTINPUTINFO();
			lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);

			if (NativeMethods.GetLastInputInfo(ref lastInPut))
			{
				long el = lastInPut.dwTime;
				long ui = (Environment.TickCount - el);

				//tickcount Overflow
				if (ui < 0)
					ui = ui + uint.MaxValue + 1;

				return ui;
			}
			else
			{
				throw new WindowManagerException("Error calling GetLastInputInfo");
			}

		}
		[Description("Get the idle time ")]
		public static TimeSpan GetIdleTimeSpan()
		{
			return new TimeSpan(GetIdleTime() * 10000);
		}
		[Description("Get image data in byte array")]
		public static byte[] GetImageData(Image img)
		{
			if (img == null)
			{
				return null;
			}
			using (MemoryStream ms = new MemoryStream())
			{
				img.Save(ms, img.RawFormat);
				return ms.GetBuffer();
			}
		}
		[Description("Crop image according the specified rectangle")]
		public static Image CropImage(Image img, Rectangle cropRectangle)
		{
			Bitmap bmpImage = new Bitmap(img);
			Bitmap bmpCrop = bmpImage.Clone(cropRectangle, bmpImage.PixelFormat);
			return (Image)(bmpCrop);
		}
		[Description("Save a bitmap image to a file")]
		public static bool SaveBitmapToFile(Bitmap bitmap, string filename, EnumImageFormat format, bool cropImage)
		{
			if (bitmap != null && !string.IsNullOrEmpty(filename))
			{
				if (cropImage)
				{
					DlgCropImage dlg = new DlgCropImage();
					dlg.LoadData(bitmap);
					dlg.ShowDialog();
					bitmap = dlg.Result as Bitmap;
				}
				if (bitmap != null)
				{
					if (System.IO.File.Exists(filename))
					{
						System.IO.File.Delete(filename);
					}
					try
					{
						using (Bitmap bm = new Bitmap(bitmap))
						{
							bm.Save(filename, VPLUtil.GetImageFormat(format));
						}
						return true;
					}
					catch (Exception err)
					{
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error saving image to [{0}]. Verify it is a valid file name. {1}", filename, err.Message));
					}
				}
			}
			return false;
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
		[Description("Capture control image to a bitmap")]
		public static Bitmap CaptureControlImage(Control control)
		{
			if (control != null && !control.IsDisposed && !control.Disposing && control.IsHandleCreated)
			{
				return CaptureWindowImage(control.Handle);
			}
			return null;
		}
		[Description("Find the Window by its title")]
		public IntPtr GetWindowByTitle(string windowTitle)
		{
			FoundWindow = WinUtil.FindWindowByTitle(windowTitle);
			_controls = null;
			return _hWnd;
		}
		[Description("Find a Window with its title starting with a specified string")]
		public IntPtr GetWindowTitleStartWith(string titleStart)
		{
			FoundWindow = NativeMethods.FindWindowByTitle(titleStart, NativeMethods.EnumTitleSearchType.StartWith);
			_controls = null;
			return _hWnd;
		}
		[Description("Find a Window with its title ending with a specified string")]
		public IntPtr GetWindowTitleEndWith(string titleEnd)
		{
			FoundWindow = NativeMethods.FindWindowByTitle(titleEnd, NativeMethods.EnumTitleSearchType.EndWith);
			_controls = null;
			return _hWnd;
		}
		[Description("Find a Window with its title containing a specified string")]
		public IntPtr GetWindowTitleContains(string titlePart)
		{
			FoundWindow = NativeMethods.FindWindowByTitle(titlePart, NativeMethods.EnumTitleSearchType.Contains);
			_controls = null;
			return _hWnd;
		}
		[Description("Get the text of the control identified by control id. The parent window must be found first")]
		public string GetControlText(int controlId)
		{
			WindowControl[] ctrls = Controls;
			if (ctrls != null)
			{
				for (int i = 0; i < ctrls.Length; i++)
				{
					if (ctrls[i].ControlId == controlId)
					{
						return ctrls[i].Text;
					}
				}
			}
			return string.Empty;
		}
		[Description("Find the Window by its title and capture the Window image into a bitmap")]
		public Bitmap CaptureWindowImage(string windowTitle)
		{
			IntPtr h = GetWindowByTitle(windowTitle);
			if (h != IntPtr.Zero)
			{
				Bitmap bmp = CaptureWindowImage(h);
				if (bmp != null)
				{
					return bmp;
				}
				else
				{
					_lastError = new Win32Exception(); //Marshal.GetLastWin32Error();
				}
			}
			else
			{
				_lastError = new Win32Exception();
			}
			return null;
		}
		[Description("Find the Window by its title, capture the Window image, and save the image to a file")]
		public bool SaveWindowImageToFile(string windowTitle, string filename, EnumImageFormat format, bool cropImage)
		{
			return SaveBitmapToFile(CaptureWindowImage(windowTitle), filename, format, cropImage);
		}
		[Description("Capture the Control image, and save the image to a file")]
		public bool SaveControlImageToFile(Control control, string filename, EnumImageFormat format, bool cropImage)
		{
			return SaveBitmapToFile(CaptureWindowImage(control.Handle), filename, format, cropImage);
		}
		[Description("Print the specified control as it appears on the screen")]
		public void PrintControl(Control control, string documentName, bool preview)
		{
			if (control != null && !control.Disposing && !control.IsDisposed && control.IsHandleCreated)
			{
				PrintDocumentControl pdoc = new PrintDocumentControl();
				pdoc.ControlToPrint = control;
				pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintControl);
				pdoc.DocumentName = documentName;
				if (preview)
				{
					PrintPreviewDialog dlg = new PrintPreviewDialog();
					dlg.Document = pdoc;
					dlg.ShowDialog(control.FindForm());
				}
				else
				{
					pdoc.Print();
				}
			}
		}
		[Description("Find the Window under the mouse pointer")]
		public IntPtr GetWindowUnderMouse()
		{
			POINT p = new POINT();
			FoundWindow = IntPtr.Zero;
			if (NativeMethods.GetCursorPos(ref p))
			{
				FoundWindow = NativeMethods.WindowFromPoint(p);
			}
			else
			{
				_lastError = new Win32Exception();
			}
			return _hWnd;
		}
		[Description("Find the Window under the mouse pointer and capture the window image")]
		public Bitmap CaptureWindowUnderMouse()
		{
			IntPtr h = GetWindowUnderMouse();
			if (h != IntPtr.Zero)
			{
				Bitmap bmp = CaptureWindowImage(h);
				if (bmp != null)
				{
					return bmp;
				}
				else
				{
					_lastError = new Win32Exception();
				}
			}
			return null;
		}
		[Description("Find the Window under the mouse pointer, capture the window image, and save the image to a file")]
		public bool SaveWindowUnderMouseImageToFile(string filename, EnumImageFormat format, bool cropImage)
		{
			return SaveBitmapToFile(CaptureWindowUnderMouse(), filename, format, cropImage);
		}

		[Description("Capture the whole screen image")]
		public Bitmap CaptureScreen()
		{
			Cursor c = null;
			Rectangle cursorBounds = Rectangle.Empty;
			IntPtr cptr = WinUtil.GetCurrentCursor();
			if (cptr != IntPtr.Zero)
			{
				c = new Cursor(cptr);
				int x0 = Cursor.Position.X;
				int y0 = Cursor.Position.Y;
				if (x0 < 0)
					x0 = 0;
				if (y0 < 0)
					y0 = 0;
				Point pos = new Point(x0, y0);
				cursorBounds = new Rectangle(pos, Cursor.Current.Size);
			}
			//===capture===
			Bitmap bmp = WinUtil.CaptureScreen();
			//===end of capture===
			if (bmp != null && c != null && cursorBounds != Rectangle.Empty)
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					c.Draw(g, cursorBounds);
				}
			}
			return bmp;
		}
		[Description("Capture the whole screen image, and save the image to a file")]
		public bool SaveScreenImageToFile(string filename, EnumImageFormat format, bool cropImage)
		{
			return SaveBitmapToFile(CaptureScreen(), filename, format, cropImage);
		}
		#endregion

		#region Properties
		[XmlIgnore]
		[Description("The window handle found by calling GetWindowByTitle, GetWindowUnderMouse, CaptureWindowImage, CaptureWindowUnderMouse, and other methods")]
		public IntPtr FoundWindow
		{
			get
			{
				return _hWnd;
			}
			set
			{
				if (_hWnd != value)
				{
					_hWnd = value;
					_controls = null;
					if (_listener != null)
					{
						_listener.End();
					}
					if (_hWnd != IntPtr.Zero)
					{
						if (Site != null && Site.DesignMode)
						{
						}
						else
						{
							if (EventsEnabled)
							{
								if (_listener == null)
								{
									_listener = new ListenerForm(this);
								}
								_listener.Start(_hWnd);
							}
						}
					}
				}
			}
		}
		[Description("The child windows contained in the window represented by FoundWindow")]
		public WindowControl[] Controls
		{
			get
			{
				if (_controls == null && _hWnd != IntPtr.Zero)
				{
					List<WindowControl> l = NativeMethods.GetChildWindows(_hWnd);
					_controls = l.ToArray();
				}
				return _controls;
			}
		}
		[Description("Windows API error code for the last failed operation")]
		public int LastErrorCode
		{
			get
			{
				if (_lastError == null)
					return 0;
				return _lastError.ErrorCode;
			}
		}
		[Description("Windows API error message for the last failed operation")]
		public string LastErrorMessage
		{
			get
			{
				if (_lastError == null)
					return "No error";
				return _lastError.Message;
			}
		}
		[DefaultValue(true)]
		[Description("Gets and sets a Boolean indicating whether the events are enabled. Disabling events makes the program run faster.")]
		public bool EventsEnabled
		{
			get { return _enableEvents; }
			set
			{
				if (_enableEvents != value)
				{
					_enableEvents = value;
					if (_enableEvents)
					{
						if (_hWnd != IntPtr.Zero)
						{
							if (_listener == null)
							{
								_listener = new ListenerForm(this);
							}
							_listener.Start(_hWnd);
						}
					}
					else
					{
						if (_listener != null)
						{
							_listener.End();
						}
					}
				}
			}
		}
		[Description("Gets and sets the location and size of the FoundWindow")]
		public Rectangle WindowBounds
		{
			get
			{
				return GetFoundWindowRect();
			}
			set
			{
				NativeMethods.SetWindowRect(_hWnd, value);
			}
		}
		[Description("Gets and sets the size of the FoundWindow")]
		public Size WindowSize
		{
			get
			{
				Rectangle r = GetFoundWindowRect();
				return new Size(r.Width, r.Height);
			}
			set
			{
				NativeMethods.SetWindowSize(_hWnd, value);
			}
		}
		[Description("Gets and sets the location of the FoundWindow")]
		public Point WindowLocation
		{
			get
			{
				Rectangle r = GetFoundWindowRect();
				return new Point(r.X, r.Y);
			}
			set
			{
				NativeMethods.SetWindowLocation(_hWnd, value);
			}
		}
		#endregion

		#region image utilities
		private void saveJpeg(string path, Bitmap img, long quality)
		{
			// Encoder parameter for image quality
			EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

			// Jpeg image codec
			ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");

			if (jpegCodec == null)
			{
				throw new WindowManagerException("Jpeg codec not found");
			}

			EncoderParameters encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = qualityParam;

			img.Save(path, jpegCodec, encoderParams);
		}

		private ImageCodecInfo getEncoderInfo(string mimeType)
		{
			// Get image codecs for all image formats
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			// Find the correct image codec
			for (int i = 0; i < codecs.Length; i++)
			{
				if (string.Compare(codecs[i].MimeType, mimeType, StringComparison.OrdinalIgnoreCase) == 0)
					return codecs[i];
			}
			return null;
		}

		void pdoc_PrintControl(object sender, PrintPageEventArgs e)
		{
			PrintDocumentControl pdc = sender as PrintDocumentControl;
			if (pdc != null)
			{
				if (pdc.ControlToPrint != null)
				{
					Bitmap bmp = CaptureWindowImage(pdc.ControlToPrint.Handle);
					if (bmp != null)
					{
						e.Graphics.DrawImage(bmp, 0, 0);
					}
				}
			}
		}
		private static Image resizeImage(Image imgToResize, Size size)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)size.Width / (float)sourceWidth);
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;

			g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			g.Dispose();

			return (Image)b;
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_listener != null)
			{
				_listener.End();
				_listener.Dispose();
				_listener = null;
			}

			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region class PrintDocumentControl
		class PrintDocumentControl : PrintDocument
		{
			public PrintDocumentControl()
			{
			}
			public Control ControlToPrint { get; set; }
		}
		#endregion
	}
}
