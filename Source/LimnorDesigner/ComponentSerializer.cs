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
	public class ComponentSerializer
	{
		XmlNode _xmlNode;
		public ComponentSerializer(XmlNode componentNode)
		{
			_xmlNode = componentNode;
		}
		public XmlNode GetActionNode(Int32 nActID)
		{
			return _xmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']", XmlTags.XML_ACTIONS, XmlTags.XML_ACTION, XmlTags.XMLATT_ActionID, nActID));
		}
		public XmlNode CreateActionNode()
		{
			XmlNode actNodes = _xmlNode.SelectSingleNode(XmlTags.XML_ACTIONS);
			if (actNodes == null)
			{
				actNodes = _xmlNode.OwnerDocument.CreateElement(XmlTags.XML_ACTIONS);
				_xmlNode.AppendChild(actNodes);
			}
			XmlNode act = _xmlNode.OwnerDocument.CreateElement(XmlTags.XML_ACTION);
			actNodes.AppendChild(act);
			return act;
		}
	}
}
