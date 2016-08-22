/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace XHost
{
	using System.Runtime.InteropServices;
	using System;
	using System.Security.Permissions;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;

	internal static class UnsafeNativeMethods
	{
		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GlobalLock(HandleRef handle);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool GlobalUnlock(HandleRef handle);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int GlobalSize(HandleRef handle);

		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalLock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern IntPtr GlobalLock(IntPtr h);

		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalUnlock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool GlobalUnLock(IntPtr h);

		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalSize", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GlobalSize(IntPtr h);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleFlushClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OpenClipboard(IntPtr newOwner);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int EmptyClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int CloseClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hParent);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern int ImageList_GetImageCount(HandleRef himl);

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

		[DllImport(ExternDll.Shell32, EntryPoint = "DragQueryFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);

		[DllImport(ExternDll.User32, EntryPoint = "RegisterClipboardFormatW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern ushort RegisterClipboardFormat(string format);

		[DllImport(ExternDll.Shell32, EntryPoint = "SHGetSpecialFolderLocation")]
		internal static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] ppidl);

		[DllImport(ExternDll.Shell32, EntryPoint = "SHGetPathFromIDList", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

		[DllImport(ExternDll.User32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			int _left;
			int _top;
			int _right;
			int _bottom;

			public RECT(global::System.Drawing.Rectangle rectangle)
				: this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
			{
			}
			public RECT(int left, int top, int right, int bottom)
			{
				_left = left;
				_top = top;
				_right = right;
				_bottom = bottom;
			}

			public int X
			{
				get { return Left; }
				set { Left = value; }
			}
			public int Y
			{
				get { return Top; }
				set { Top = value; }
			}
			public int Left
			{
				get { return _left; }
				set { _left = value; }
			}
			public int Top
			{
				get { return _top; }
				set { _top = value; }
			}
			public int Right
			{
				get { return _right; }
				set { _right = value; }
			}
			public int Bottom
			{
				get { return _bottom; }
				set { _bottom = value; }
			}
			public int Height
			{
				get { return Bottom - Top; }
				set { Bottom = value - Top; }
			}
			public int Width
			{
				get { return Right - Left; }
				set { Right = value + Left; }
			}
			public global::System.Drawing.Point Location
			{
				get { return new global::System.Drawing.Point(Left, Top); }
				set
				{
					Left = value.X;
					Top = value.Y;
				}
			}
			public global::System.Drawing.Size Size
			{
				get { return new global::System.Drawing.Size(Width, Height); }
				set
				{
					Right = value.Width + Left;
					Bottom = value.Height + Top;
				}
			}

			public global::System.Drawing.Rectangle ToRectangle()
			{
				return new global::System.Drawing.Rectangle(this.Left, this.Top, this.Width, this.Height);
			}
			public static global::System.Drawing.Rectangle ToRectangle(RECT Rectangle)
			{
				return Rectangle.ToRectangle();
			}
			public static RECT FromRectangle(global::System.Drawing.Rectangle Rectangle)
			{
				return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
			}

			public static implicit operator global::System.Drawing.Rectangle(RECT Rectangle)
			{
				return Rectangle.ToRectangle();
			}
			public static implicit operator RECT(global::System.Drawing.Rectangle Rectangle)
			{
				return new RECT(Rectangle);
			}
			public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
			{
				return Rectangle1.Equals(Rectangle2);
			}
			public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
			{
				return !Rectangle1.Equals(Rectangle2);
			}

			public override string ToString()
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{Left: {0}; Top: {1}; Right: {2}; Bottom: {3} }",
					Left, Top, Right, Bottom);
			}

			public bool Equals(RECT Rectangle)
			{
				return Rectangle.Left == Left && Rectangle.Top == Top && Rectangle.Right == Right && Rectangle.Bottom == Bottom;
			}
			public override bool Equals(object Object)
			{
				if (Object is RECT)
				{
					return Equals((RECT)Object);
				}
				else if (Object is Rectangle)
				{
					return Equals(new RECT((global::System.Drawing.Rectangle)Object));
				}

				return false;
			}

			public override int GetHashCode()
			{
				return Left.GetHashCode() ^ Right.GetHashCode() ^ Top.GetHashCode() ^ Bottom.GetHashCode();
			}
		}

	}
}

