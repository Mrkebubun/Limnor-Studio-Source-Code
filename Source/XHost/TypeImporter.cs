using System;
using System.Collections.Generic;
using System.Text;
using PerformerImport;
using System.Windows.Forms;

namespace XHost
{
    
    class TypeImporter : MarshalByRefObject
    {
        private string[] _selectedTypes;
        public TypeImporter()
        {
        }
        public string[] SelectedTypes
        {
            get
            {
                return _selectedTypes;
            }
            set
            {
                _selectedTypes = value;
            }
        }
        static void ad2_DomainUnload(object sender, EventArgs e)
        {
        }
        public void Run()
        {
            _selectedTypes = null;
            frmPerformerImport f = new frmPerformerImport();
            if (f.ShowDialog() == DialogResult.OK)
            {
                Type[] types = frmPerformerImport.WizardInfo.GetSelectedTypes();
                if (types != null && types.Length > 0)
                {
                    _selectedTypes = new string[types.Length];
                    for (int i = 0; i < types.Length; i++)
                    {
                        _selectedTypes[i] = types[i].AssemblyQualifiedName;
                    }
                }
            }
        }
        public static string[] SelectTypes()
        {
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase =
                "file:///" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;
            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            //
            // Create the second AppDomain.
            AppDomain ad2 = AppDomain.CreateDomain("LimnorStudioSelectTypes", null, ads);
            ad2.DomainUnload += new EventHandler(ad2_DomainUnload);
            TypeImporter obj = (TypeImporter)ad2.CreateInstanceAndUnwrap(
                        typeof(TypeImporter).Assembly.FullName,
                        typeof(TypeImporter).FullName
                    );
            obj.Run();
            return obj.SelectedTypes;

        }

    }
}
