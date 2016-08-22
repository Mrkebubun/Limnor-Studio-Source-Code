/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Remoting Support based on Windows Communication Foundation (WCF)
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using VPL;
using System.Xml;
using XmlUtility;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Net;

namespace Limnor.Remoting.Host
{
	[ToolboxBitmapAttribute(typeof(RemotingHost), "Resources.remoting.bmp")]
	[Description("It provides a host for remoting services. Subclass it and add methods to be called by clients. Call Open method to start servicing clients. Call Close method to stop servicing clients.")]
	public class RemotingHost : IComponent, IXmlNodeSerializable, IAppConfigConsumer
	{
		#region fields and constructors
		private ServiceHost _host;
		private string _serviceName;
		private int _baseport;
		private int _wsport;
		private int _tcpport;
		public RemotingHost()
		{
			init();
		}
		public RemotingHost(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			init();
		}
		[Browsable(false)]
		private void init()
		{
			_host = new ServiceHost(this.GetType(), new Uri[] { });
			_serviceName = "mydomain.myservices";
			_baseport = 8001;
			_wsport = 8002;
			_tcpport = 8003;
		}
		[Browsable(false)]
		void _host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			if (UnknownMessageReceived != null)
			{
				UnknownMessageReceived(this, e);
			}
		}
		[Browsable(false)]
		void _host_Opening(object sender, EventArgs e)
		{
			if (Opening != null)
			{
				Opening(this, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		void _host_Opened(object sender, EventArgs e)
		{
			if (Opened != null)
			{
				Opened(this, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		void _host_Faulted(object sender, EventArgs e)
		{
			if (Faulted != null)
			{
				Faulted(this, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		void _host_Closing(object sender, EventArgs e)
		{
			if (Closing != null)
			{
				Closing(this, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		void _host_Closed(object sender, EventArgs e)
		{
			if (Closed != null)
			{
				Closed(this, EventArgs.Empty);
			}
		}
		#endregion

		#region Methods
		[Description("Causes a communication object to transition from the created state into the opened state.")]
		public void Open()
		{
			ServiceHost host = new ServiceHost(this.GetType());
			//
			_host = host;
			//
			_host.Closed += new EventHandler(_host_Closed);
			_host.Closing += new EventHandler(_host_Closing);
			_host.Faulted += new EventHandler(_host_Faulted);
			_host.Opened += new EventHandler(_host_Opened);
			_host.Opening += new EventHandler(_host_Opening);
			_host.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(_host_UnknownMessageReceived);
			_host.Open();
		}
		[Description("Causes a communication object to transition from its current state into the closed state")]
		public void Close()
		{
			_host.Close();
		}
		#endregion

		#region Properties
		[Description("Gets and sets the name for the service host")]
		public string ServiceName
		{
			get { return _serviceName; }
			set { if (!string.IsNullOrEmpty(value)) _serviceName = value; }
		}
		[Description("Gets and sets the meta data port number for the service host")]
		public int BasePort
		{
			get { return _baseport; }
			set { if (value > 0) _baseport = value; }
		}
		[Description("Gets and sets the port number for the wsHttpBinding of the service host")]
		public int WsHttpPort
		{
			get { return _wsport; }
			set { if (value > 0) _wsport = value; }
		}
		[Description("Gets and sets the port number for the netTcpBinding of the service host")]
		public int TcpPort
		{
			get { return _tcpport; }
			set { if (value > 0) _tcpport = value; }
		}
		[Browsable(false)]
		public IList<ServiceEndpoint> EndPoints
		{
			get
			{
				List<ServiceEndpoint> list = new List<ServiceEndpoint>();
				if (_host != null && _host.Description != null && _host.Description.Endpoints != null)
				{
					list.AddRange(_host.Description.Endpoints);
				}

				return list;
			}
		}
		[Description("Gets a value that indicates the current state of the communication object.")]
		public CommunicationState State
		{
			get
			{
				return _host.State;
			}
		}

		[Browsable(false)]
		protected virtual Type ContractInterface
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public string ContractInterfaceName
		{
			get
			{
				if (Site != null && Site.DesignMode && !string.IsNullOrEmpty(Site.Name))
				{
					return string.Format(CultureInfo.InvariantCulture, "I{0}", Site.Name);
				}
				if (ContractInterface == null)
				{
					return "?";
				}
				return ContractInterface.Name;
			}
		}
		#endregion

		#region Events
		[Description("Occurs when a communication object transitions into the closed state")]
		public event EventHandler Closed;
		[Description("Occurs when a communication object transitions into the closing state")]
		public event EventHandler Closing;
		[Description("Occurs when a communication object transitions into the faulted state")]
		public event EventHandler Faulted;
		[Description("Occurs when a communication object transitions into the opened state")]
		public event EventHandler Opened;
		[Description("Occurs when a communication object transitions into the opening state")]
		public event EventHandler Opening;
		[Description("Occurs when an unknown message is received")]
		public event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived;
		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_ServiceName = "serviceName";
		const string XMLATT_ServicePort = "servicePort";
		const string XMLATT_wsPort = "wsPort";
		const string XMLATT_tcpPort = "tcpPort";
		[Browsable(false)]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			ServiceName = XmlUtil.GetAttribute(node, XMLATT_ServiceName);
			BasePort = XmlUtil.GetAttributeInt(node, XMLATT_ServicePort);
			WsHttpPort = XmlUtil.GetAttributeInt(node, XMLATT_wsPort);
			TcpPort = XmlUtil.GetAttributeInt(node, XMLATT_tcpPort);
		}
		[Browsable(false)]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_ServiceName, ServiceName);
			XmlUtil.SetAttribute(node, XMLATT_ServicePort, BasePort);
			XmlUtil.SetAttribute(node, XMLATT_wsPort, WsHttpPort);
			XmlUtil.SetAttribute(node, XMLATT_tcpPort, TcpPort);
		}

		#endregion

		#region IAppConfigConsumer Members

		public void MergeAppConfig(XmlNode rootNode, string projectFolder, string projectNamespace)
		{
			string serviceAppName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", projectNamespace, Site.Name);
			string contracteName = string.Format(CultureInfo.InvariantCulture, "{0}.I{1}", projectNamespace, Site.Name);
			XmlDocument doc = new XmlDocument();
			string xml = Resource1.config.Replace("{name}", Site.Name);
			xml = xml.Replace("{serviceName}", ServiceName);
			xml = xml.Replace("{serviceapp}", serviceAppName);
			xml = xml.Replace("{contractname}", contracteName);
			xml = xml.Replace("{baseport}", _baseport.ToString(CultureInfo.InvariantCulture));
			xml = xml.Replace("{tcpport}", _tcpport.ToString(CultureInfo.InvariantCulture));
			xml = xml.Replace("{wsport}", _wsport.ToString(CultureInfo.InvariantCulture));
			doc.LoadXml(xml);

			////merge into target
			XmlNode serviceModelTarget = rootNode.SelectSingleNode("system.serviceModel");
			if (serviceModelTarget == null)
			{
				serviceModelTarget = rootNode.OwnerDocument.CreateElement("system.serviceModel");
				rootNode.AppendChild(serviceModelTarget);
			}
			XmlNode bindingsNodeTarget = serviceModelTarget.SelectSingleNode("bindings");
			if (bindingsNodeTarget == null)
			{
				bindingsNodeTarget = rootNode.OwnerDocument.CreateElement("bindings");
				serviceModelTarget.AppendChild(bindingsNodeTarget);
			}
			XmlNodeList nodes = doc.DocumentElement.SelectNodes("system.serviceModel/bindings/*");
			foreach (XmlNode nd in nodes)
			{
				XmlNode ndTarget = rootNode.OwnerDocument.ImportNode(nd, true);
				bindingsNodeTarget.AppendChild(ndTarget);
			}
			//
			XmlNode servicesNode = serviceModelTarget.SelectSingleNode("services");
			if (servicesNode == null)
			{
				servicesNode = rootNode.OwnerDocument.CreateElement("services");
				serviceModelTarget.AppendChild(servicesNode);
			}
			nodes = doc.DocumentElement.SelectNodes("system.serviceModel/services/*");
			foreach (XmlNode nd in nodes)
			{
				XmlNode ndTarget = rootNode.OwnerDocument.ImportNode(nd, true);
				servicesNode.AppendChild(ndTarget);
			}
			//
			XmlNode behavioursNode = serviceModelTarget.SelectSingleNode("behaviors");
			if (behavioursNode == null)
			{
				behavioursNode = rootNode.OwnerDocument.CreateElement("behaviors");
				serviceModelTarget.AppendChild(behavioursNode);
			}
			nodes = doc.DocumentElement.SelectNodes("system.serviceModel/behaviors/*");
			foreach (XmlNode nd in nodes)
			{
				XmlNode ndTarget = rootNode.OwnerDocument.ImportNode(nd, true);
				behavioursNode.AppendChild(ndTarget);
			}
		}

		public bool IsSameConsumer(IAppConfigConsumer consumer)
		{
			RemotingHost rh = consumer as RemotingHost;
			if (rh != null)
			{
				return (string.Compare(rh.ServiceName, this.ServiceName, StringComparison.OrdinalIgnoreCase) == 0);
			}
			return false;
		}

		#endregion
	}
}
