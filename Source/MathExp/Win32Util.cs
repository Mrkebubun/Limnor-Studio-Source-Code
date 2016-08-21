/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MathExp
{
    public class Win32Util
    {
        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(System.IntPtr hwnd, uint dwTime, uint dwFlags);
        public const uint AW_HOR_POSITIVE = 0x00000001;
        public const uint AW_HOR_NEGATIVE = 0x00000002;
        public const uint AW_VER_POSITIVE = 0x00000004;
        public const uint AW_VER_NEGATIVE = 0x00000008;
        public const uint AW_CENTER = 0x00000010;
        public const uint AW_HIDE = 0x00010000;
        public const uint AW_ACTIVATE = 0x00020000;
        public const uint AW_SLIDE = 0x00040000;
        public const uint AW_BLEND = 0x00080000;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_HIDEWINDOW = 0x0080;
        public const uint SWP_NOCOPYBITS = 0x0100;
        public const uint SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        public const uint SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */

        public const uint SWP_DRAWFRAME = SWP_FRAMECHANGED;
        public const uint SWP_NOREPOSITION = SWP_NOOWNERZORDER;

        public const uint SWP_DEFERERASE = 0x2000;
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;

        public static IntPtr HWND_TOP = IntPtr.Zero;// ((HWND)0)
        public static IntPtr HWND_BOTTOM = ((IntPtr)1);
        public static IntPtr HWND_TOPMOST = ((IntPtr)(-1));
        public static IntPtr HWND_NOTOPMOST = ((IntPtr)(-2));
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc,    // handle to DC
            int nXPos,  // x-coordinate of pixel
            int nYPos   // y-coordinate of pixel
        );
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr h);
        private Win32Util()
        {
        }
    }
}
