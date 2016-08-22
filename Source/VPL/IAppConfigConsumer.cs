/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VPL
{
	public interface IAppConfigConsumer
	{
		void MergeAppConfig(XmlNode rootNode, string projectFolder, string projectNamespace);
		bool IsSameConsumer(IAppConfigConsumer consumer);
	}
}
