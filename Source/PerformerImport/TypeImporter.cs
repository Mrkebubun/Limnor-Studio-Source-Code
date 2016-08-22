/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using PerformerImport;
using System.Windows.Forms;
using System.Globalization;
//using PerformerImport;

namespace PerformerImport
{
	public class TypeImporter : MarshalByRefObject
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
		public void Run(int supportActiveX)
		{
			DialogResult ret;
			_selectedTypes = null;
			if (supportActiveX != 0)
			{
				frmPerformerImport f = new frmPerformerImport();
				ret = f.ShowDialog();
			}
			else
			{
				dlgClassFile dlg = new dlgClassFile();
				dlg.refreshInfo();
				dlg.frmPrev = null;
				ret = dlg.ShowDialog();
			}
			if (ret == DialogResult.OK)
			{
				Type[] types;
				if (frmPerformerImport.WizardInfo.SourceType == enumSourceType.ActiveX)
				{
					types = ActiveXImporter.ActiveXInfo.GetSelectedTypes();
				}
				else
				{
					types = frmPerformerImport.WizardInfo.GetSelectedTypes();
				}
				if (types != null && types.Length > 0)
				{
					_selectedTypes = new string[types.Length];
					for (int i = 0; i < types.Length; i++)
					{
						_selectedTypes[i] = string.Format(CultureInfo.InvariantCulture, "{0};{1}", types[i].Assembly.Location, types[i].AssemblyQualifiedName);
					}
				}
			}
		}
		public static string[] SelectTypes(int supportActiveX)
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
			try
			{
				obj.Run(supportActiveX);
				return obj.SelectedTypes;
			}
			catch
			{
			}
			return null;
		}

	}
}
