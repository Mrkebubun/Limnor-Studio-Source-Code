using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace Limnor.Windows
{
    public sealed class RegistryData
    {
        public static string GetTutorialFolder()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Longflow Enterprises\Limnor Tutorial");
            if (key != null)
            {
                object v = key.GetValue("Tutorial");
                if (v != null)
                {
                    return Path.Combine(v.ToString(),"Tutorials");
                }
            }
            return Path.Combine( AppDomain.CurrentDomain.BaseDirectory,"Tutorials");
        }
    }
}
