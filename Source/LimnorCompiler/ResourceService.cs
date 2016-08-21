/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Resources;
using System.Globalization;

namespace LimnorCompiler
{
	public class ResourceService : IResourceService
	{
		private ResXResourceReader reader;
		private ResXResourceWriter writer;
		private string resxFile = "";
		public ResourceService(string file)
		{
			resxFile = file;
			if (resxFile != null && resxFile.Length > 0)
			{
				createResxFile();
				reader = new ResXResourceReader(resxFile);
				writer = new ResXResourceWriter(resxFile);
			}
		}
		public string ResxFile
		{
			get
			{
				return resxFile;
			}
			set
			{
				resxFile = value;
				if (resxFile != null && resxFile.Length > 0)
				{
					createResxFile();
					reader = new ResXResourceReader(resxFile);
					writer = new ResXResourceWriter(resxFile);
				}
			}
		}
		private void createResxFile()
		{
			if (!System.IO.File.Exists(resxFile))
			{
				string s = string.Format(CultureInfo.InvariantCulture, "{0}.Resources1", this.GetType().Assembly.GetName().Name);
				ResourceManager resourceManager = new ResourceManager(s, GetType().Assembly);
				object v = resourceManager.GetObject("resxtemplate");
				if (v == null)
				{
					throw new Exception("resource template not found");
				}
				else
				{
					System.IO.StreamWriter sw = new System.IO.StreamWriter(resxFile);
					sw.Write(v);
					sw.Close();
				}
			}
		}
		#region IResourceService Members

		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			return reader;
		}

		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info)
		{
			return writer;
		}

		#endregion
	}
}
