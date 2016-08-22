/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSPrj
{
	/// <summary>
	/// configurations to be written to the root node
	/// </summary>
	public interface IConfig
	{
		void WriteConfig(XmlNode rootNode);
		bool ConfigChanged { get; set; }
	}
}
