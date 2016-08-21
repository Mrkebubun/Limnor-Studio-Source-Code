namespace NativeMethods
{
    using System;
    using System.Runtime.InteropServices;
    //
    public static class WindowsAPI
    {
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        public const uint MAPVK_VK_TO_VSC = 0x00;
        public const uint MAPVK_VSC_TO_VK = 0x01;
        public const uint MAPVK_VK_TO_CHAR = 0x02;
        public const uint MAPVK_VSC_TO_VK_EX = 0x03;
        public const uint MAPVK_VK_TO_VSC_EX = 0x04;


        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, long lParam);
    }
}