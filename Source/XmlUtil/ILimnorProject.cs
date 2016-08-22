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

namespace XmlUtility
{
	public interface ILimnorProject
	{
		XmlNode ResourcesXmlNode { get; }
		string ResourcesFile { get; }
	}
	public interface IProjectAccessor
	{
		ILimnorProject Project { get; set; }
	}
}
