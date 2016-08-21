using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DirectShowLib
{
    [ComVisible(false)]
    public class DsHlp
    {
        public const int OATRUE = -1;
        public const int OAFALSE = 0;

        [DllImport("quartz.dll", CharSet = CharSet.Auto)]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);
    }
}
