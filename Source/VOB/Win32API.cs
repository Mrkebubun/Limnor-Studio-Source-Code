using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VOB
{
    public sealed class Win32API
    {
        const int CSIDL_LOCAL_APPDATA = 0x001c;
        const int CSIDL_PERSONAL = 0x0005;
        const int SHGFP_TYPE_CURRENT = 0;   // current value for user, verify it exists
        const int SHGFP_TYPE_DEFAULT = 1;   // default value, may not exist
        [DllImport("shell32.dll")]
        static extern int SHGetFolderPath(System.IntPtr hwndOwner, int nFolder, System.IntPtr hToken, uint dwFlags, System.Text.StringBuilder pszPath);
        [DllImport("WinUtil.dll")]
        public static extern void MakePoint(uint l, out Int16 x, out Int16 y);
        //
        static string GetSpecialFolder(int folder)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(200);
            SHGetFolderPath(System.IntPtr.Zero, folder, System.IntPtr.Zero, SHGFP_TYPE_CURRENT, sb);
            return sb.ToString();
        }
        public static string GetLocalAppDataFolder()
        {
            return GetSpecialFolder(CSIDL_LOCAL_APPDATA);
        }
        public static string GetDefaultProjectFolder()
        {
            //return System.IO.Path.Combine(GetSpecialFolder(CSIDL_PERSONAL),"Longflow\\LimnorStudio");
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Longflow\\LimnorStudio");
        }
        private Win32API()
        {
        }
    }
}
