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

namespace Limnor.Windows
{
	[Description("It contains event arguments for event ApplicationActivate")]
	public class EventArgsApplicationActivate : EventArgs
	{
		public EventArgsApplicationActivate(bool activate, int threadId)
		{
			IsActivated = activate;
			TheOtherThreadId = threadId;
		}
		[Description("Gets a Boolean indicating whether the window is being activated or deactivated. This value is TRUE if the window is being activated; it is FALSE if the window is being deactivated.")]
		public bool IsActivated { get; private set; }
		[Description("Gets the thread identifier of the other application. If IsActivated is TRUE, it is the identifier of the thread that owns the window being deactivated. If IsActivated is FALSE, it is the identifier of the thread that owns the window being activated. It can be 0.")]
		public int TheOtherThreadId { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"App Active:{0} - {1}", IsActivated, TheOtherThreadId);
		}
	}
	[Description("It contains event arguments for event WindowActivate")]
	public class EventArgsWindowActivate : EventArgs
	{
		public EventArgsWindowActivate(bool activate, bool byMouse, bool minimized, IntPtr otherWindow)
		{
			IsActivated = activate;
			IsMinimized = minimized;
			IsActivatedByMouse = byMouse;
			TheOtherWindowHandle = otherWindow;
		}
		[Description("Gets a Boolean indicating whether the window is being activated or deactivated")]
		public bool IsActivated { get; private set; }
		[Description("Gets a Boolean indicating whether the window is being activated by mouse click")]
		public bool IsActivatedByMouse { get; private set; }
		[Description("Gets a Boolean indicating the minimized state of the window being activated or deactivated. A True value indicates the window is minimized.")]
		public bool IsMinimized { get; private set; }
		[Description("Gets a handle to the window being activated or deactivated, depending on the value of the IsActivated. If IsActivated is False then it is the handle to the window being activated. If IsActivated is True then it is the handle to the window being deactivated. This handle can be NULL")]
		public IntPtr TheOtherWindowHandle { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"Window Active:{0} - by click:{1} - minimized:{2} - other window:{3}", IsActivated, IsActivatedByMouse, IsMinimized, TheOtherWindowHandle);
		}
	}
	[Description("It contains event arguments for events WindowGotFocus and WindowKillFocus")]
	public class EventArgsWindowFocus : EventArgs
	{
		public EventArgsWindowFocus(IntPtr otherWindow, bool focus)
		{
			GotFocus = focus;
			TheOtherWindowHandle = otherWindow;
		}
		[Description("Gets a Boolean indicating whether it is getting or losing keyboard input focus.")]
		public bool GotFocus { get; private set; }
		[Description("Gets a handle to the window that has lost/gained the keyboard focus. If the current window gets the focus then TheOtherWindowHandle loses focus. If the current window loses focus then TheOtherWindowHandle gains the focus. It can be NULL. ")]
		public IntPtr TheOtherWindowHandle { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Window focus:{0} - The other handle:{1}", GotFocus, TheOtherWindowHandle);
		}
	}
	[Description("It contains event arguments for event SystemCommand")]
	public class EventArgsSystemCommand : EventArgs
	{
		public EventArgsSystemCommand(EnumSystemCommand command)
		{
			SystemCommand = command;
		}
		[Description("Gets the type of system command requested")]
		public EnumSystemCommand SystemCommand { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Command:{0}-{1}", SystemCommand, ((int)SystemCommand).ToString("x"));
		}
	}
	[Description("It contains event arguments for event WindowSizing")]
	public class EventArgsWindowSizing : EventArgs
	{
		public EventArgsWindowSizing(Rectangle rect, EnumSizeEdge edge)
		{
			Rectangle = rect;
			SizeEdge = edge;
		}
		[Description("Gets the drag rectangle")]
		public Rectangle Rectangle { get; private set; }
		[Description("Gets the edge of the window that is being sized.")]
		public EnumSizeEdge SizeEdge { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Sizing Edge:{0}-({1})", SizeEdge, Rectangle);
		}
	}
	[Description("It contains event arguments for event WindowResized")]
	public class EventArgsWindowSized : EventArgs
	{
		public EventArgsWindowSized(EnumWindowSizeType type, Size newSize)
		{
			ResizeType = type;
			NewSize = newSize;
		}
		[Description("Gets the type of resizing requested")]
		public EnumWindowSizeType ResizeType { get; private set; }
		[Description("Gets the new size")]
		public Size NewSize { get; private set; }
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Resize type:{0}-({1})", ResizeType, NewSize);
		}
	}
	[Description("It contains event arguments for events WindowPositionChanging and WindowPositionChanged")]
	public class EventArgsWindowPositionChange : EventArgs
	{
		public EventArgsWindowPositionChange(Point pos, Size size, EnumWindowPositionChangeStyle changeStyle, IntPtr hwnd, IntPtr insertAfter, EnumWindowRelativePostion relativePosition)
		{
			Location = pos;
			Size = size;
			Handle = hwnd;
			InsertAfter = insertAfter;
			ChangeStyle = changeStyle;
			WindowRelativePosition = relativePosition;
			IsMoved = ((changeStyle & EnumWindowPositionChangeStyle.SWP_NOMOVE) != EnumWindowPositionChangeStyle.SWP_NOMOVE);
			IsResized = ((changeStyle & EnumWindowPositionChangeStyle.SWP_NOSIZE) != EnumWindowPositionChangeStyle.SWP_NOSIZE);
			IsResizedOrMoved = (IsMoved || IsResized);
		}
		[Description("Gets the location of the window")]
		public Point Location { get; private set; }
		[Description("Gets the size of the window")]
		public Size Size { get; private set; }
		[Description("Gets the style for the window postion change")]
		public EnumWindowPositionChangeStyle ChangeStyle { get; private set; }
		[Description("Gets a handle to the window. ")]
		public IntPtr Handle { get; private set; }
		[Description("Gets a handle to the window presenting the position of the window in Z order (front-to-back position). If this member is not 0 then it is a handle to the window behind which this window is placed. If this member is 0 then WindowRelativePosition property represents the window position change.")]
		public IntPtr InsertAfter { get; private set; }
		[Description("Gets the window relative position if InsertAfter is 0")]
		public EnumWindowRelativePostion WindowRelativePosition { get; private set; }
		[Description("Gets a Boolean indicating whether the window is moved")]
		public bool IsMoved { get; private set; }
		[Description("Gets a Boolean indicating whether the window is resized")]
		public bool IsResized { get; private set; }
		[Description("Gets a Boolean indicating whether the window is resized or moved")]
		public bool IsResizedOrMoved { get; private set; }
	}
	public delegate void EventHandlerApplicationActivate(object sender, EventArgsApplicationActivate e);
	public delegate void EventHandlerWindowActivate(object sender, EventArgsWindowActivate e);
	public delegate void EventHandlerWindowFocus(object sender, EventArgsWindowFocus e);
	public delegate void EventHandlerSystemCommand(object sender, EventArgsSystemCommand e);
	public delegate void EventHandlerWindowSizing(object sender, EventArgsWindowSizing e);
	public delegate void EventHandlerWindowResized(object sender, EventArgsWindowSized e);
	public delegate void EventHandlerWindowPositionChange(object sender, EventArgsWindowPositionChange e);

	public enum EnumSystemCommand : int
	{
		SC_CLOSE = 0xF060, // Closes the window.
		SC_CONTEXTHELP = 0xF180, // Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message.
		SC_DEFAULT = 0xF160, // Selects the default item; the user double-clicked the window menu.
		SC_HOTKEY = 0xF150, // Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate.
		SC_HSCROLL = 0xF080, // Scrolls horizontally.
		SCF_ISSECURE = 0x00000001, // Indicates whether the screen saver is secure. 
		SC_KEYMENU = 0xF100, // Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section.
		SC_MAXIMIZE = 0xF030, // Maximizes the window.
		SC_MINIMIZE = 0xF020, // Minimizes the window.
		SC_MONITORPOWER = 0xF170, // Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer. 
		//The lParam parameter can have the following values:
		//-1 (the display is powering on)
		//1 (the display is going to low power)
		//2 (the display is being shut off)
		SC_MOUSEMENU = 0xF090, // Retrieves the window menu as a result of a mouse click.
		SC_MOVE = 0xF010, // Moves the window.
		SC_MOVE_HTCAPTION = 0xF012, //
		SC_NEXTWINDOW = 0xF040, // Moves to the next window.
		SC_PREVWINDOW = 0xF050, // Moves to the previous window.

		SC_RESTORE = 0xF120, // Restores the window to its normal position and size.
		SC_SCREENSAVE = 0xF140, // Executes the screen saver application specified in the [boot] section of the System.ini file.
		SC_SIZE = 0xF000, // Sizes the window.
		SC_SIZE_WEST = 0xF001, //Resize from left
		SC_SIZE_EAST = 0xF002, // (Resize from right)
		SC_SIZE_NORTH = 0xF003,// (Resize from up) 
		SC_SIZE_NORTH_WEST = 0xF004, // (Lock the bottom right corner of the form, the up left corner move for resize) 
		SC_SIZE_NORTH_EAST = 0xF005, // (Same from bottom left corner) 
		SC_SIZE_NORTH_SOUTH = 0xF006, // (Lock up right and left border, resize other) 
		SC_SIZE_SOUTH_WEST = 0xF007, // (Lock up and right border, resize other border) 
		SC_SIZE_SOUTH_EAST = 0xF008, //Lock left and up border and resize other
		SC_TASKLIST = 0xF130, // Activates the Start menu.
		SC_VSCROLL = 0xF070, // Scrolls vertically.
	}
	/*
	 0xF001 (Resize from left) 
0xF008 (Lock left and up border and resize other) 
	 */
	public enum EnumSizeEdge : int
	{
		WMSZ_BOTTOM = 6, // Bottom edge
		WMSZ_BOTTOMLEFT = 7, // Bottom-left corner
		WMSZ_BOTTOMRIGHT = 8, // Bottom-right corner
		WMSZ_LEFT = 1,// Left edge
		WMSZ_RIGHT = 2,// Right edge
		WMSZ_TOP = 3,// Top edge
		WMSZ_TOPLEFT = 4,// Top-left corner
		WMSZ_TOPRIGHT = 5,// Top-right corner
	}
	public enum EnumWindowSizeType : int
	{
		SIZE_MAXHIDE = 4,// Message is sent to all pop-up windows when some other window is maximized.
		SIZE_MAXIMIZED = 2, // The window has been maximized.
		SIZE_MAXSHOW = 3,// Message is sent to all pop-up windows when some other window has been restored to its former size.
		SIZE_MINIMIZED = 1, // The window has been minimized.
		SIZE_RESTORED = 0, // The window has been resized, but neither the SIZE_MINIMIZED nor SIZE_MAXIMIZED value applies.
	}

	[Flags]
	public enum EnumWindowPositionChangeStyle : int
	{
		SWP_DRAWFRAME = 0x0020,// Draws a frame (defined in the window's class description) around the window. Same as the SWP_FRAMECHANGED flag.
		SWP_FRAMECHANGED = 0x0020,// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
		SWP_HIDEWINDOW = 0x0080, // Hides the window.
		SWP_NOACTIVATE = 0x0010, // Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hwndInsertAfter member).
		SWP_NOCOPYBITS = 0x0100,// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
		SWP_NOMOVE = 0x0002, // Retains the current position (ignores the x and y members).
		SWP_NOOWNERZORDER = 0x0200,// Does not change the owner window's position in the Z order.
		SWP_NOREDRAW = 0x0008,// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
		SWP_NOREPOSITION = 0x0200,// Does not change the owner window's position in the Z order. Same as the SWP_NOOWNERZORDER flag.
		SWP_NOSENDCHANGING = 0x0400,// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
		SWP_NOSIZE = 0x0001,// Retains the current size (ignores the cx and cy members).
		SWP_NOZORDER = 0x0004,// Retains the current Z order (ignores the hwndInsertAfter member).
		SWP_SHOWWINDOW = 0x0040,// Displays the window.
	}

	public enum EnumWindowRelativePostion : int
	{
		HWND_BOTTOM = 1,// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
		HWND_NOTOPMOST = -2,// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
		HWND_TOP = 0,// Places the window at the top of the Z order.
		HWND_TOPMOST = -1,// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
	}
}
