//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Limnor Studio.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DefaultNamespace
{
    using System;
    using System.Xml;
    using System.Text;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Threading;
    
    [System.Drawing.ToolboxBitmapAttribute(typeof(DefaultNamespace.WinFormApp1), "writeSlides.ICO")]
    public class WinFormApp1
    {
        protected internal static DefaultNamespace.Form1 var5a08dedd;
        static System.Threading.Mutex mf0f5c6d1;
        [DllImportAttribute("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static WinFormApp1()
        {
        }
        public static DefaultNamespace.Form1 Page1
        {
            get
            {
                if (((var5a08dedd == null) 
                            || (var5a08dedd.IsDisposed == true)))
                {
                    var5a08dedd = new DefaultNamespace.Form1();
                }
                return var5a08dedd;
            }
        }
        private static void InitializeComponent()
        {
            // 
            // Page1
            // 
            Page1.ClientSize = new System.Drawing.Size(373, 286);
            Page1.Visible = false;
            // 
            // WinFormApp1
            // 
        }
        [STAThread()]
        public static void Main()
        {
            bool b21b8d2ff;
            mf0f5c6d1 = new System.Threading.Mutex(true, "043e962a-802e-417c-9701-6ef9e34e515a", out b21b8d2ff);
            if ((b21b8d2ff == false))
            {
                System.Diagnostics.Process p6507634b = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process[] pf85e70f6 = System.Diagnostics.Process.GetProcessesByName(p6507634b.ProcessName);
                for (int i = 0; (i < pf85e70f6.Length); i++
                )
                {
                    if ((((pf85e70f6[i].Id != p6507634b.Id) 
                                && (string.Compare(pf85e70f6[i].MainModule.FileName, p6507634b.MainModule.FileName, System.StringComparison.OrdinalIgnoreCase) == 0)) 
                                && (p6507634b.MainWindowHandle != System.IntPtr.Zero)))
                    {
                        WinFormApp1.SetForegroundWindow(pf85e70f6[i].MainWindowHandle);
                        break;
                    }
                }
                return;
            }
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeComponent();
            System.Windows.Forms.Application.Run(Page1);
        }
    }
}
