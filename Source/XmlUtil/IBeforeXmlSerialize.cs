/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VPL;

namespace XmlUtility
{
	public interface IBeforeXmlSerialize
	{
		void OnBeforeXmlSerialize(XmlNode node, IXmlCodeWriter writer);
	}
}
