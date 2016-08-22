/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;

namespace XmlUtility
{
	public sealed class AppConfig
	{
		public const string CFG_LS_MONO = "LimnorStudioMono";
		public const string CAT_SOLUTION = "Solution";
		private static string getCfgFile(string file)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file);
		}
		private static XmlNode getCateXmlNode(string filename, string category)
		{
			string f = getCfgFile(filename);
			if (File.Exists(f))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(f);
				if (doc.DocumentElement != null)
				{
					return doc.DocumentElement.SelectSingleNode(category);
				}
			}
			return null;
		}
		private static XmlNode getValXmlNode(string filename, string category, string valueName)
		{
			string f = getCfgFile(filename);
			if (File.Exists(f))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(f);
				if (doc.DocumentElement != null)
				{
					return doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", category, valueName));
				}
			}
			return null;
		}
		public static string GetStringValue(string filename, string category, string valueName)
		{
			XmlNode node = getValXmlNode(filename, category, valueName);
			if (node != null)
			{
				return node.InnerText;
			}
			return string.Empty;
		}
		public static void SetStringValue(string filename, string category, string valueName, string value)
		{
			XmlDocument doc = new XmlDocument();
			string f = getCfgFile(filename);
			if (File.Exists(f))
			{
				doc.Load(f);
			}
			XmlNode r;
			if (doc.DocumentElement == null)
			{
				r = doc.CreateElement("AppConfig");
				doc.AppendChild(r);
			}
			else
			{
				r = doc.DocumentElement;
			}
			XmlNode c = r.SelectSingleNode(category);
			if (c == null)
			{
				c = doc.CreateElement(category);
				r.AppendChild(c);
			}
			XmlNode v = c.SelectSingleNode(valueName);
			if (v == null)
			{
				v = doc.CreateElement(valueName);
				c.AppendChild(v);
			}
			v.InnerText = value;
			doc.Save(f);
		}
		//
		public static string GetSolutionStringValue(string name)
		{
			return GetStringValue(CFG_LS_MONO, CAT_SOLUTION, name);
		}
		public static void SetSolutionStringValue(string name, string value)
		{
			SetStringValue(CFG_LS_MONO, CAT_SOLUTION, name, value);
		}
	}
}
