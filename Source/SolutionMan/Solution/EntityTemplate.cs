/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;

namespace SolutionMan.Solution
{
	public class EntityTemplate
	{
		private XmlDocument _doc;
		private XmlNamespaceManager _xm;
		public EntityTemplate(string file)
		{
			EntityTemplateFile = file;
			_doc = new XmlDocument();
			_doc.Load(file);
			_xm = new XmlNamespaceManager(_doc.NameTable);
			_xm.AddNamespace("x", "http://schemas.microsoft.com/developer/vstemplate/2005");
		}
		public string EntityTemplateFile;
		public string IconFile;
		public string Name;
		public string Description;
		public string DefaultName;
		public bool HasIconFile
		{
			get
			{
				if (!string.IsNullOrEmpty(IconFile))
				{

					if (File.Exists(IconFullPath))
					{
						return true;
					}
				}
				return false;
			}
		}
		public string IconFullPath
		{
			get
			{
				return Path.Combine(TemplateFolder, IconFile);
			}
		}
		public Icon TemplateIcon
		{
			get
			{
				if (HasIconFile)
				{
					Icon ic = Icon.ExtractAssociatedIcon(IconFullPath);
					return ic;
				}
				return null;
			}
		}
		public string TemplateFolder
		{
			get
			{
				return Path.GetDirectoryName(EntityTemplateFile);
			}
		}
		protected XmlDocument Document
		{
			get
			{
				return _doc;
			}
		}
		protected XmlNamespaceManager XmlNamespace
		{
			get
			{
				return _xm;
			}
		}
		public void LoadName()
		{
			if (_doc.DocumentElement != null)
			{
				XmlNode node = _doc.DocumentElement.SelectSingleNode("//x:TemplateData/x:Name", _xm);
				if (node != null)
				{
					Name = node.InnerText;
				}
				node = _doc.DocumentElement.SelectSingleNode("//x:TemplateData/x:Description", _xm);
				if (node != null)
				{
					Description = node.InnerText;
				}
				node = _doc.DocumentElement.SelectSingleNode("//x:TemplateData/x:Icon", _xm);
				if (node != null)
				{
					IconFile = node.InnerText;
				}
				node = _doc.DocumentElement.SelectSingleNode("//x:TemplateData/x:DefaultName", _xm);
				if (node != null)
				{
					DefaultName = node.InnerText;
				}
			}
		}

	}
}
