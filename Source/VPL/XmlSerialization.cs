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
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace VPL
{
	public sealed class XmlSerializerUtility
	{
		public const string XML_VarMap = "VarMap";
		const string XMLATT_MainType = "mainType";
		const string XMLATT_Filename = "filename";
		public static IXmlCodeReader ActiveReader { get; set; }
		public static IXmlCodeWriter ActiveWriter { get; set; }
		public static fnCreateWriterFromReader OnCreateWriterFromReader = null;
		public static IXmlCodeWriter GetWriter(IXmlCodeReader reader)
		{
			IXmlCodeWriter w = null;
			if (OnCreateWriterFromReader != null)
			{
				if (reader != null)
				{
					w = OnCreateWriterFromReader(reader);
				}
				if (w == null)
				{
					if (ActiveReader != null)
					{
						w = OnCreateWriterFromReader(ActiveReader);
					}
				}
			}
			if (w == null)
			{
				w = ActiveWriter;
			}
			return w;
		}
		public static XmlDocument Save(object obj, List<Type> types)
		{
			Type tMain = obj.GetType();
			XmlSerializer s = new XmlSerializer(tMain, types.ToArray());
			StringWriter w = new StringWriter();
			try
			{
				s.Serialize(w, obj);
			}
			catch (Exception err)
			{
				StringBuilder sb = new StringBuilder(err.Message);
				sb.Append(".\r\nMain type:");
				sb.Append(tMain.AssemblyQualifiedName);
				sb.Append(".\r\n");
				if (types == null || types.Count == 0)
				{
					sb.Append("No additional types");
				}
				else
				{
					sb.Append("Additional types:\r\n");
					foreach (Type t in types)
					{
						sb.Append(t.AssemblyQualifiedName);
						sb.Append(".\r\n");
					}
				}
				throw new VPLException(err, sb.ToString());
			}
			finally
			{
				w.Close();
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(w.ToString());
			XmlAttribute xa = doc.CreateAttribute(XMLATT_MainType);
			doc.DocumentElement.Attributes.Append(xa);
			xa.Value = tMain.AssemblyQualifiedName;
			if (!tMain.Assembly.GlobalAssemblyCache)
			{
				xa = doc.CreateAttribute(XMLATT_Filename);
				doc.DocumentElement.Attributes.Append(xa);
				xa.Value = tMain.Assembly.Location;
			}
			XmlNode typesNode = doc.CreateElement("Types");
			doc.DocumentElement.AppendChild(typesNode);
			foreach (Type t in types)
			{
				XmlNode tn = doc.CreateElement("Type");
				typesNode.AppendChild(tn);
				tn.InnerText = t.AssemblyQualifiedName;
			}
			return doc;
		}
		public static T LoadFromXmlFile<T>(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			return LoadFromXmlDocument<T>(doc);
		}
		public static T LoadFromXmlString<T>(string xmlContents)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContents);
			return LoadFromXmlDocument<T>(doc);
		}
		public static T LoadFromXmlDocument<T>(XmlDocument xmlDoc)
		{
			Type tMain = typeof(T);
			List<Type> types = new List<Type>();
			if (xmlDoc.DocumentElement != null)
			{
				XmlNodeList typeNodes = xmlDoc.DocumentElement.SelectNodes("Types/Type");
				if (typeNodes != null)
				{
					string sVer = null;
					foreach (XmlNode nd in typeNodes)
					{
						Type t = Type.GetType(nd.InnerText);
						if (t == null)
						{
							//switch the version
							int p1 = nd.InnerText.IndexOf("Version=");
							if (p1 > 0)
							{
								int p2 = nd.InnerText.IndexOf(",", p1);
								if (p2 > 0)
								{
									if (string.IsNullOrEmpty(sVer))
									{
										sVer = tMain.Assembly.GetName().Version.ToString();
									}
									string sType = nd.InnerText.Substring(0, p1 + "Version=".Length) + sVer + nd.InnerText.Substring(p2);
									t = Type.GetType(sType);
								}
							}
						}
						if (t != null)
						{
							types.Add(t);
						}
					}
				}
			}
			StringReader r = new StringReader(xmlDoc.OuterXml);
			XmlSerializer s = new XmlSerializer(tMain, types.ToArray());
			object ret = s.Deserialize(r);
			r.Close();
			return (T)ret;
		}
		public static object LoadFromXmlFile(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			return LoadFromXmlDocument(doc);
		}
		public static object LoadFromXmlString(string xmlContents)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContents);
			return LoadFromXmlDocument(doc);
		}
		public static object LoadFromXmlDocument(XmlDocument xmlDoc)
		{
			Type tMain = null;
			List<Type> types = new List<Type>();
			if (xmlDoc.DocumentElement != null && xmlDoc.DocumentElement.Attributes != null)
			{
				XmlAttribute xa = xmlDoc.DocumentElement.Attributes[XMLATT_MainType];
				if (xa != null)
				{
					string sType = xa.Value;
					tMain = Type.GetType(sType);
					if (tMain == null)
					{
						xa = xmlDoc.DocumentElement.Attributes[XMLATT_Filename];
						if (xa != null && !string.IsNullOrEmpty(xa.Value))
						{
							if (System.IO.File.Exists(xa.Value))
							{
								Assembly ab = Assembly.LoadFile(xa.Value);
								if (ab != null)
								{
									int h = sType.IndexOf(',');
									if (h > 0)
									{
										sType = sType.Substring(0, h);
									}
									tMain = ab.GetType(sType);
								}
							}
						}
					}
				}
				if (tMain != null)
				{
					XmlNodeList typeNodes = xmlDoc.DocumentElement.SelectNodes("Types/Type");
					if (typeNodes != null)
					{
						string sVer = null;
						foreach (XmlNode nd in typeNodes)
						{
							Type t = Type.GetType(nd.InnerText);
							if (t == null)
							{
								//switch the version
								int p1 = nd.InnerText.IndexOf("Version=");
								if (p1 > 0)
								{
									int p2 = nd.InnerText.IndexOf(",", p1);
									if (p2 > 0)
									{
										if (string.IsNullOrEmpty(sVer))
										{
											sVer = tMain.Assembly.GetName().Version.ToString();
										}
										string sType = nd.InnerText.Substring(0, p1 + "Version=".Length) + sVer + nd.InnerText.Substring(p2);
										t = Type.GetType(sType);
									}
								}
							}
							if (t != null)
							{
								types.Add(t);
							}
						}
					}
				}
			}
			if (tMain == null)
			{
				new VPLException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error reading DataEditor. cannot resolve the object type from the xml contents:{0}", xmlDoc.OuterXml));
			}
			else
			{
				StringReader r = new StringReader(xmlDoc.OuterXml);
				XmlSerializer s = new XmlSerializer(tMain, types.ToArray());
				object ret = s.Deserialize(r);
				r.Close();
				return ret;
			}
			return null;
		}
	}
	public delegate IXmlCodeWriter fnCreateWriterFromReader(IXmlCodeReader reader);
}
