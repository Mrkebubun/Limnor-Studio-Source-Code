/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;

namespace VPL
{
	public class WcfServiceProxy : IAppConfigConsumer
	{
		private string _proxyDllFile;
		private string _proxyConfig;
		private string _serverUrl;
		private Assembly _assembly;
		private List<Type> _proxyTypes;
		public WcfServiceProxy(string dllFile, Assembly assembly, string serverUrl, string config)
		{
			_proxyDllFile = dllFile;
			_assembly = assembly;
			_serverUrl = serverUrl;
			_proxyConfig = config;
		}
		public string ServerUrl
		{
			get
			{
				return _serverUrl;
			}
		}
		public string ProxyDllFile
		{
			get
			{
				return _proxyDllFile;
			}
		}
		public string ConfigFile
		{
			get
			{
				return _proxyConfig;
			}
		}
		public Assembly ProxyAssembly
		{
			get
			{
				return _assembly;
			}
		}
		public List<Type> ProxyTypes
		{
			get
			{
				if (_proxyTypes == null)
				{
					_proxyTypes = new List<Type>();
					Type[] tps = _assembly.GetExportedTypes();
					if (tps != null && tps.Length > 0)
					{
						for (int i = 0; i < tps.Length; i++)
						{
							Type baseType = tps[i].BaseType;
							if (baseType != null)
							{
								Type[] gts = baseType.GetGenericArguments();
								if (gts != null && gts.Length == 1)
								{
									Type t0 = typeof(System.ServiceModel.ClientBase<>).MakeGenericType(gts);
									if (t0 != null)
									{
										if (t0.IsAssignableFrom(tps[i]))
										{
											_proxyTypes.Add(tps[i]);
										}
									}
								}
							}
						}
					}
				}
				return _proxyTypes;
			}
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			List<Type> ps = ProxyTypes;
			if (ps.Count > 0)
			{
				sb.Append(ps[0].Name);
				for (int i = 1; i < ps.Count; i++)
				{
					sb.Append(", ");
					sb.Append(ps[i].Name);
				}
			}
			return sb.ToString();
		}

		#region IAppConfigConsumer Members

		public void MergeAppConfig(XmlNode rootNode, string projectFolder, string projectNamespace)
		{
			const string XML_Service = "system.serviceModel";
			const string XML_Bindings = "bindings";
			const string XML_client = "client";
			string cfg = ConfigFile;
			bool bCfg = File.Exists(cfg);
			if (!bCfg)
			{
				cfg = Path.Combine(projectFolder, Path.GetFileName(cfg));
				bCfg = File.Exists(cfg);
			}
			if (bCfg)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(cfg);
				if (doc.DocumentElement != null)
				{
					XmlNode serviceNode = rootNode.SelectSingleNode(XML_Service);
					if (serviceNode == null)
					{
						serviceNode = rootNode.OwnerDocument.CreateElement(XML_Service);
						rootNode.AppendChild(serviceNode);
					}
					XmlNode serviceNodeSrc = doc.DocumentElement.SelectSingleNode(XML_Service);
					if (serviceNodeSrc != null)
					{
						XmlNode bindingsSrc = serviceNodeSrc.SelectSingleNode(XML_Bindings);
						if (bindingsSrc != null)
						{
							XmlNode bindings = serviceNode.SelectSingleNode(XML_Bindings);
							if (bindings == null)
							{
								bindings = rootNode.OwnerDocument.CreateElement(XML_Bindings);
								serviceNode.AppendChild(bindings);
							}
							foreach (XmlNode nd in bindingsSrc.ChildNodes)
							{
								XmlNode nd2 = rootNode.OwnerDocument.ImportNode(nd, true);
								bindings.AppendChild(nd2);
							}
						}
					}
					XmlNode clientSrc = serviceNodeSrc.SelectSingleNode(XML_client);
					if (clientSrc != null)
					{
						XmlNode client = serviceNode.SelectSingleNode(XML_client);
						if (client == null)
						{
							client = rootNode.OwnerDocument.ImportNode(clientSrc, true);
							serviceNode.AppendChild(client);
						}
					}
				}
			}
		}
		public bool IsSameConsumer(IAppConfigConsumer consumer)
		{
			WcfServiceProxy wcf = consumer as WcfServiceProxy;
			if (wcf != null)
			{
				if (string.Compare(Path.GetFileName(ProxyDllFile), Path.GetFileName(wcf.ProxyDllFile), StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
