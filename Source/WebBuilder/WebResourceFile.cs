/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using VPL;

namespace Limnor.WebBuilder
{
	public class WebResourceFile
	{
		public const string WEBFOLDER_Images = "images";
		public const string WEBFOLDER_Php = "libphp";
		public const string WEBFOLDER_Javascript = "libjs";
		public const string WEBFOLDER_Css = "css";
		private string _resourceFile;
		private string _webFolder;
		public WebResourceFile(string file, string webFolder, out bool replaced)
		{
			replaced = false;
			if (!string.IsNullOrEmpty(file))
			{
				try
				{
					if (File.Exists(file))
					{
					}
					else
					{
						DialogFileMap dlg = new DialogFileMap();
						dlg.LoadData(file);
						if (!string.IsNullOrEmpty(dlg.ReplaceFile))
						{
							file = dlg.ReplaceFile;
							replaced = true;
						}
						else if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							file = dlg.ReplaceFile;
							replaced = true;
						}
					}
				}
				catch
				{
				}
			}
			_resourceFile = file;
			_webFolder = webFolder;
		}
		public string WebAddress
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", _webFolder, Path.GetFileName(_resourceFile));
			}
		}
		public string ResourceFile
		{
			get
			{
				return _resourceFile;
			}
		}
		public string WebFolder
		{
			get
			{
				return _webFolder;
			}
		}
	}
}
