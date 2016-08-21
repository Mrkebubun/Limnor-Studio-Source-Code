/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlSerializer;
using XmlUtility;

namespace LimnorDesigner
{
	/// <summary>
	/// programming serialization 
	/// replace clsPrjDB by read/write XML instead of database.
	/// Project file organization:
	/// one project.lrprj manages many rootObject.limnor
	/// </summary>
	public class ProjectSerializer
	{
		private string _prjFilename;
		private XmlNode _xmlNode;
		public ProjectSerializer()
		{
		}
		public XmlNode Load(string projectFilename)
		{
			_prjFilename = projectFilename;
			XmlDocument doc = new XmlDocument();
			if (System.IO.File.Exists(_prjFilename))
			{
				doc.Load(_prjFilename);
			}
			_xmlNode = doc.DocumentElement;
			if (_xmlNode == null)
			{
				_xmlNode = doc.CreateElement(XmlTags.XML_Project);
				doc.AppendChild(_xmlNode);
				doc.Save(_prjFilename);
			}
			return _xmlNode;
		}
		public void Save()
		{
			_xmlNode.OwnerDocument.Save(_prjFilename);
		}
		public void SaveAs(string projectFilename)
		{
			_prjFilename = projectFilename;
			_xmlNode.OwnerDocument.Save(_prjFilename);
		}
		public XmlNode GetComponentNode(string componentFile)
		{
			XmlNodeList nodes = _xmlNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}", XmlTags.XML_Components, XmlTags.XML_Component));
			foreach (XmlNode nd in nodes)
			{
				string s = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_filename);
				if (string.Compare(componentFile, s, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return nd;
				}
			}
			return null;
		}
		public bool AddComponentFile(string componentFile)
		{
			List<int> ids = new List<int>();
			XmlNodeList nodes = _xmlNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
			"{0}/{1}", XmlTags.XML_Components, XmlTags.XML_Component));
			foreach (XmlNode nd in nodes)
			{
				string s = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_filename);
				if (string.Compare(componentFile, s, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return false;
				}
				else
				{
					int n = XmlUtil.GetAttributeInt(nd, XmlTags.XMLATT_ComponentID);
					if (n > 0)
						ids.Add(n);
				}
			}
			XmlNode node = XmlUtil.CreateNewElement(ComponentsNode, XmlTags.XML_Component);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_filename, componentFile);
			int id = 1;
			while (true)
			{
				if (ids.Contains(id))
				{
					id++;
				}
				else
				{
					break;
				}
			}
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ComponentID, id);
			return true;
		}
		public bool DeleteComponent(string componentFile)
		{
			XmlNode node = GetComponentNode(componentFile);
			if (node != null)
			{
				XmlNode p = node.ParentNode;
				p.RemoveChild(node);
				return true;
			}
			return false;
		}
		public XmlNode ComponentsNode
		{
			get
			{
				XmlNode node = _xmlNode.SelectSingleNode(XmlTags.XML_Components);
				if (node == null)
				{
					node = XmlUtil.CreateNewElement(_xmlNode, XmlTags.XML_Components);
				}
				return node;
			}
		}
		public XmlNode GetComponentNodeByID(Int32 componentId)
		{
			return _xmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']", XmlTags.XML_Components, XmlTags.XML_Component, XmlTags.XMLATT_ComponentID, componentId));
		}
		private byte[] readBinary(string tag)
		{
			XmlNode nd = _xmlNode.SelectSingleNode(tag);
			if (nd != null)
			{
				return Convert.FromBase64String(nd.InnerText);
			}
			return null;
		}
		private void saveBinary(string tag, byte[] obj)
		{
			XmlNode nd = _xmlNode.SelectSingleNode(tag);
			if (nd == null)
			{
				nd = _xmlNode.OwnerDocument.CreateElement(tag);
				_xmlNode.AppendChild(nd);
			}
			nd.InnerText = Convert.ToBase64String(obj);
		}
		public byte[] ReadTraceConfig()
		{
			return readBinary(XmlTags.XML_TraceLog);
		}
		public void SaveTraceConfig(byte[] obj)
		{
			saveBinary(XmlTags.XML_TraceLog, obj);
		}
	}
}
