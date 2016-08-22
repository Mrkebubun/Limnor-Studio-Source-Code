/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project -- Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Services.Protocols;

namespace Limnor.WebServer
{
	public static class WebServerUtility
	{
		public const string XML_Error = "Error";
		public const string XML_ErrorNumber = "ErrorNumber";
		public const string XML_ErrorMessage = "ErrorMessage";
		public const string XML_ErrorSource = "ErrorSource";
		public static SoapException CreateSoapException(string uri,
									string webServiceNamespace,
									string errorMessage,
									string errorNumber,
									string errorSource,
									EnumNetLocation errorLocation)
		{
			XmlQualifiedName faultCodeLocation = null;
			//Identify the location of the FaultCode
			if (errorLocation == EnumNetLocation.Client)
			{
				faultCodeLocation = SoapException.ClientFaultCode;
			}
			else
			{
				faultCodeLocation = SoapException.ServerFaultCode;
			}
			XmlDocument xmlDoc = new XmlDocument();
			//Create the Detail node
			XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element,
							   SoapException.DetailElementName.Name,
							   SoapException.DetailElementName.Namespace);
			//Build specific details for the SoapException
			//Add first child of detail XML element.
			XmlNode errorNode = xmlDoc.CreateNode(XmlNodeType.Element, XML_Error,
												  webServiceNamespace);
			//Create and set the value for the ErrorNumber node
			XmlNode errorNumberNode =
			  xmlDoc.CreateNode(XmlNodeType.Element, XML_ErrorNumber,
								webServiceNamespace);
			errorNumberNode.InnerText = errorNumber;
			//Create and set the value for the ErrorMessage node
			XmlNode errorMessageNode = xmlDoc.CreateNode(XmlNodeType.Element,
														XML_ErrorMessage,
														webServiceNamespace);
			errorMessageNode.InnerText = errorMessage;
			//Create and set the value for the ErrorSource node
			XmlNode errorSourceNode =
			  xmlDoc.CreateNode(XmlNodeType.Element, XML_ErrorSource,
								webServiceNamespace);
			errorSourceNode.InnerText = errorSource;
			//Append the Error child element nodes to the root detail node.
			errorNode.AppendChild(errorNumberNode);
			errorNode.AppendChild(errorMessageNode);
			errorNode.AppendChild(errorSourceNode);
			//Append the Detail node to the root node
			rootNode.AppendChild(errorNode);
			//Construct the exception
			SoapException soapEx = new SoapException(errorMessage,
													 faultCodeLocation, uri,
													 rootNode);
			//Raise the exception  back to the caller
			return soapEx;
		}

	}
}
