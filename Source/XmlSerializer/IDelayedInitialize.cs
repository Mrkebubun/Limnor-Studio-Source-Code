/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlSerializer
{
	/// <summary>
	/// reading ISerializerProcessor will call OnPostSerialize
	/// but at that time some data may not available.
	/// if this interface is implemented then it may add itself to XmlObjectReader
	/// and call OnDelayedPostSerialize when all objects are read
	/// </summary>
	public interface IDelayedInitialize
	{
		void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader);
		/// <summary>
		/// when it is not the time to do initialization, set the parameters so that later it can be done
		/// </summary>
		/// <param name="objMap"></param>
		/// <param name="objectNode"></param>
		/// <param name="reader"></param>
		void SetReader(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader);
	}
}
