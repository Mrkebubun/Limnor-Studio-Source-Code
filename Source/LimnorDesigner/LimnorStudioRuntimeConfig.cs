/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner
{
	public static class LimnorStudioRuntimeConfig
	{
		private static string cfgFile
		{
			get
			{
				string sdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Limnor Studio");
				if (!Directory.Exists(sdir))
				{
					Directory.CreateDirectory(sdir);
				}
				return Path.Combine(sdir, "LimnorStudio.config");
			}
		}
		private static XmlNode RootNode
		{
			get
			{
				XmlDocument doc = new XmlDocument();
				string f = cfgFile;
				if (File.Exists(f))
				{
					doc.Load(f);
				}
				if (doc.DocumentElement == null)
				{
					XmlNode r = doc.CreateElement("LimnorStudio");
					doc.AppendChild(r);
				}
				return doc.DocumentElement;
			}
		}
		const string XML_CopyProtection = "CopyProtection";
		const string XMLATT_disableModulePrompt = "disableModulePrompt";
		public static bool CopyProtectionModuleWarningDisabled
		{
			get
			{
				XmlNode r = RootNode;
				XmlNode node = r.SelectSingleNode(XML_CopyProtection);
				if (node != null)
				{
					return XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_disableModulePrompt);
				}
				return false;
			}
			set
			{
				XmlNode r = RootNode;
				XmlNode node = XmlUtil.CreateSingleNewElement(r, XML_CopyProtection);
				XmlUtil.SetAttribute(node, XMLATT_disableModulePrompt, value);
				r.OwnerDocument.Save(cfgFile);
			}
		}
	}
}
