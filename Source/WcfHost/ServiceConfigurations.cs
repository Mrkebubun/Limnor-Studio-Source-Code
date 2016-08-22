/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Remoting Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using XmlUtility;

namespace Limnor.Remoting.Host
{
	class ServiceConfigurations
	{
		private string _configFile;
		public ServiceConfigurations()
		{
		}
		private string ConfigFile
		{
			get
			{
				if (string.IsNullOrEmpty(_configFile))
				{
					_configFile = string.Format(CultureInfo.InvariantCulture,
						"{0}.config", Application.ExecutablePath);
				}
				return _configFile;
			}
		}
		protected XmlNode GetServiceModelNode()
		{
			if (File.Exists(ConfigFile))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(ConfigFile);
				if (doc.DocumentElement != null)
				{
					return doc.DocumentElement.SelectSingleNode("system.serviceModel");
				}
			}
			return null;
		}
	}
	class BindingConfig : ServiceConfigurations
	{
		private XmlNode _node;
		public BindingConfig(string name)
		{
			XmlNode sv = GetServiceModelNode();
			if (sv != null)
			{
				_node = sv.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "//binding[@name=\"{0}\"]", name));
			}
		}
		public XmlNode XmlData
		{
			get
			{
				return _node;
			}
		}
	}
	class ServiceConfig : ServiceConfigurations
	{
		private XmlNode _node;
		public ServiceConfig(string name)
		{
			XmlNode sv = GetServiceModelNode();
			if (sv != null)
			{
				_node = sv.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "//service[@name=\"{0}\"]", name));
			}
		}
		public XmlNode XmlData
		{
			get
			{
				return _node;
			}
		}
		public string GetBaseAddress(string bindingName)
		{
			XmlNode bindingNode = _node.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
				"endpoint[@bindingConfiguration=\"{0}\"]", bindingName));
			if (bindingNode != null)
			{
				return XmlUtil.GetAttribute(bindingNode, "address");
			}
			return string.Empty;
		}
	}
}
