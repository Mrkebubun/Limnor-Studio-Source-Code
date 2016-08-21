/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using LimnorWeb;
using System.IO;

namespace LimnorDesigner
{
	public class WebWizard
	{
		public WebWizard()
		{
		}
		public bool RunWizard(Dictionary<string, string> replacementsDictionary)
		{
			bool bOK;
			string projectFolder = replacementsDictionary["$destinationdirectory$"];
			string websitename;
			if (string.IsNullOrEmpty(projectFolder))
			{
				websitename = "Test";
			}
			else
			{
				websitename = Path.GetFileNameWithoutExtension(projectFolder);
				if (string.IsNullOrEmpty(websitename))
				{
					websitename = "test";
				}
				else
				{
					websitename = websitename.Replace(" ", "");
					if (string.IsNullOrEmpty(websitename))
					{
						websitename = "TEST";
					}
				}
			}
			DialogSelectWebName dlg = new DialogSelectWebName();
			dlg.SetWebsiteName(websitename);
			bOK = (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK);
			if (bOK)
			{
				replacementsDictionary.Add("$websitename$", dlg.WebsiteName);
			}
			return bOK;
		}
	}
}
