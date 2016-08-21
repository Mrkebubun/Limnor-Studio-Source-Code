/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Limnor.Drawing2D
{
	static class Config
	{
		const string GUID = "81FDE7F0-B5AB-41c0-B304-DF5E5AA76229";

		const string XMLATT_Left = "left";
		const string XMLATT_Top = "top";
		const string XMLATT_Guid = "guid";
		const string XMLATT_Name = "name";
		static private XmlNode GetConfigNode()
		{
			string sPath = getConfigFile();
			if (System.IO.File.Exists(sPath))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(sPath);
				if (doc.DocumentElement != null)
				{
					return doc.DocumentElement.SelectSingleNode(string.Format(
						System.Globalization.CultureInfo.InvariantCulture, "Config[@guid='{0}']", GUID));
				}
			}
			return null;
		}
		static private string getConfigFile()
		{
			string sPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Limnor Studio");
			if (!System.IO.Directory.Exists(sPath))
			{
				System.IO.Directory.CreateDirectory(sPath);
			}
			return System.IO.Path.Combine(sPath, "appConfig.xml");
		}
		static private void saveConfig(XmlDocument doc)
		{
			string sPath = getConfigFile();
			doc.Save(sPath);
		}
		static private XmlNode MakeConfigNode()
		{
			XmlNode cfg;
			XmlDocument doc = new XmlDocument();
			string sPath = getConfigFile();
			if (System.IO.File.Exists(sPath))
			{
				doc.Load(sPath);
				if (doc.DocumentElement != null)
				{
					cfg = doc.DocumentElement.SelectSingleNode(
						string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Config[@guid='{0}']", GUID));
					if (cfg == null)
					{
						cfg = doc.CreateElement("Config");
						doc.DocumentElement.AppendChild(cfg);
						SetAttribute(cfg, XMLATT_Guid, GUID);
						SetAttribute(cfg, XMLATT_Name, "Limnor2D");
					}
				}
				else
				{
					XmlNode root = doc.CreateElement("Root");
					doc.AppendChild(root);
					cfg = doc.CreateElement("Config");
					root.AppendChild(cfg);
					SetAttribute(cfg, XMLATT_Guid, GUID);
					SetAttribute(cfg, XMLATT_Name, "Limnor2D");
				}
			}
			else
			{
				XmlNode root = doc.CreateElement("Root");
				doc.AppendChild(root);
				cfg = doc.CreateElement("Config");
				root.AppendChild(cfg);
				SetAttribute(cfg, XMLATT_Guid, GUID);
				SetAttribute(cfg, XMLATT_Name, "Limnor2D");
			}
			return cfg;
		}
		static public Point GetEditorPropertyGridPosition(string windowName)
		{
			XmlNode node = GetConfigNode();
			if (node != null)
			{
				XmlNode nodeP = node.SelectSingleNode(windowName);
				if (nodeP != null)
				{
					int x = GetAttributeInt(nodeP, XMLATT_Left);
					int y = GetAttributeInt(nodeP, XMLATT_Top);
					return new Point(x, y);
				}
			}
			return new Point(0, 0);
		}
		static public void SetEditorPropertyGridPosition(Point p, string windowName)
		{
			XmlNode node = MakeConfigNode();
			XmlNode nodeP = node.SelectSingleNode(windowName);
			if (nodeP == null)
			{
				nodeP = node.OwnerDocument.CreateElement(windowName);
				node.AppendChild(nodeP);
			}
			SetAttribute(nodeP, XMLATT_Left, p.X);
			SetAttribute(nodeP, XMLATT_Top, p.Y);
			saveConfig(node.OwnerDocument);
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new Exception("getting attribute missing name. Xml path:" + GetPath(node));
				}
				if (node.Attributes != null)
				{
					XmlAttribute xa = node.Attributes[name];
					if (xa != null)
					{
						return xa.Value;
					}
				}
			}
			return "";
		}
		public static int GetAttributeInt(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return 0;
					return Convert.ToInt32(xa.Value);
				}
			}
			return 0;
		}
		public static void SetAttribute(XmlNode node, string name, string value)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa == null)
				{
					xa = node.OwnerDocument.CreateAttribute(name);
					node.Attributes.Append(xa);
				}
				xa.Value = value;
			}
		}
		public static void SetAttribute(XmlNode node, string name, object val)
		{
			SetAttribute(node, name, val.ToString());
		}
		public static string GetPath(XmlNode node)
		{
			StringBuilder sb = new StringBuilder();
			List<string> names = new List<string>();
			while (node != null)
			{
				string s = GetAttribute(node, "name");
				if (!string.IsNullOrEmpty(s))
				{
					names.Insert(0, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[@name='{1}']", node.Name, s));
				}
				else
				{
					names.Insert(0, node.Name);
				}
				node = node.ParentNode;
			}
			sb.Append(names[0]);
			for (int i = 1; i < names.Count; i++)
			{
				sb.Append("/");
				sb.Append(names[i]);
			}
			return sb.ToString();
		}
	}
}
