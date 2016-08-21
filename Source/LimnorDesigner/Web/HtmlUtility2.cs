/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Xml;
using Limnor.WebServerBuilder;
using XmlUtility;
using System.Globalization;

namespace LimnorDesigner.Web
{
	public static class HtmlUtility2
	{
		private static void removeLimnrId(HtmlNode node, string iframeId, string serverFile)
		{
			if (node.Attributes.Contains("limnorId"))
			{
				node.Attributes.Remove("limnorId");
			}
			if (node.Attributes.Contains("stylename"))
			{
				node.Attributes.Remove("stylename");
			}
			if (node.Attributes.Contains("styleshare"))
			{
				node.Attributes.Remove("styleshare");
			}
			if (string.Compare("form", node.Name, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.Compare(node.GetAttributeValue("typename", ""), "fileupload", StringComparison.OrdinalIgnoreCase) == 0)
				{
					node.SetAttributeValue("target", iframeId);
					node.SetAttributeValue("action", serverFile);
					List<HtmlAttribute> attrs = new List<HtmlAttribute>();
					for (int i = 0; i < node.Attributes.Count; i++)
					{
						if (string.Compare("hidechild", node.Attributes[i].Name, StringComparison.OrdinalIgnoreCase) == 0
							|| string.Compare("typename", node.Attributes[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							attrs.Add(node.Attributes[i]);
						}
					}
					foreach (HtmlAttribute ha in attrs)
					{
						node.Attributes.Remove(ha);
					}
				}
			}
			else if (string.Compare("iframe", node.Name, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (node.Attributes.Contains("srcdesign"))
				{
					string src = node.GetAttributeValue("srcdesign", "");
					if (!string.IsNullOrEmpty(src))
					{
						node.SetAttributeValue("src", src);
					}
					node.Attributes.Remove("srcdesign");
				}
			}
			if (node.ChildNodes != null && node.ChildNodes.Count > 0)
			{
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					removeLimnrId(node.ChildNodes[i], iframeId, serverFile);
				}
			}
		}
		private static void setDataBind0(HtmlNode node, HtmlElement_BodyBase hbb)
		{
			bool first = true;
			StringBuilder dbs = new StringBuilder();
			for (int i = 0; i < hbb.DataBindings.Count; i++)
			{
				if (hbb.DataBindings[i].BindingMemberInfo != null
					&& !string.IsNullOrEmpty(hbb.DataBindings[i].BindingMemberInfo.BindingPath)
					&& !string.IsNullOrEmpty(hbb.DataBindings[i].BindingMemberInfo.BindingField)
					&& !string.IsNullOrEmpty(hbb.DataBindings[i].PropertyName)
					)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						dbs.Append(";");
					}
					dbs.Append(hbb.DataBindings[i].BindingMemberInfo.BindingPath);
					dbs.Append(":");
					dbs.Append(hbb.DataBindings[i].BindingMemberInfo.BindingField);
					dbs.Append(":");
					dbs.Append(hbb.DataBindings[i].PropertyName);
				}
			}
			node.SetAttributeValue("jsdb", dbs.ToString());
		}
		private static void setDataBind(HtmlNode node, IList<HtmlElement_BodyBase> htmlElements)
		{
			string id = node.GetAttributeValue("id", string.Empty);
			if (!string.IsNullOrEmpty(id))
			{
				foreach (HtmlElement_BodyBase hbb in htmlElements)
				{
					if (string.CompareOrdinal(id, hbb.id) == 0)
					{
						setDataBind0(node, hbb);
						IDataSourceBinder dsb = hbb as IDataSourceBinder;
						if (dsb != null)
						{
							IDataSetSource ids = dsb.DataSource as IDataSetSource;
							if (ids != null && !string.IsNullOrEmpty(ids.TableName))
							{
								node.SetAttributeValue("jsdb", ids.TableName);
							}
						}
						break;
					}
				}
			}
			if (node.ChildNodes.Count > 0)
			{
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					setDataBind(node.ChildNodes[i], htmlElements);
				}
			}
		}
		public static HtmlAgilityPack.HtmlDocument LoadHtmlDocument(string htmlFile)
		{
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(htmlFile);
			return doc;
		}
		public static void CopyXmlNodeToHtml(HtmlDocument hDoc, HtmlNode hNode, XmlNode xNode)
		{
			HtmlNode hn = hDoc.CreateElement(xNode.Name);
			hNode.AppendChild(hn);
			foreach (XmlNode nc in xNode.ChildNodes)
			{
				if (nc.NodeType == XmlNodeType.Text)
				{
					HtmlTextNode htxt = hDoc.CreateTextNode(nc.InnerText);
					hn.AppendChild(htxt);
					break;
				}
				else if (nc.NodeType == XmlNodeType.CDATA)
				{
					HtmlTextNode htxt = hDoc.CreateTextNode(nc.Value);
					hn.AppendChild(htxt);
					break;
				}
				else
				{
					CopyXmlNodeToHtml(hDoc, hn, nc);
				}
			}
			if (xNode.Attributes != null)
			{
				foreach (XmlAttribute xa in xNode.Attributes)
				{
					hn.SetAttributeValue(xa.Name, xa.Value);
				}
			}
		}
		private static string getHtmlAttribute(HtmlNode node, string attName)
		{
			if (node.Attributes != null)
			{
				HtmlAttribute a = node.Attributes[attName];
				if(a != null)
				{
					return a.Value;
				}
			}
			return null;
		}
		private static void setHtmlAttribute(HtmlNode node, string attName, string val)
		{
			if (node.Attributes != null)
			{
				HtmlAttribute a = node.Attributes[attName];
				if (a == null)
				{
					a = node.OwnerDocument.CreateAttribute(attName, val);
					node.Attributes.Append(a);
				}
				else
				{
					a.Value = val;
				}
			}
		}
		private static void mergeXmlHeadToHtml(HtmlDocument hDoc, HtmlNode hHead, XmlNode xHead)
		{
			string s;
			bool found;
			for (int i = 0; i < xHead.ChildNodes.Count; i++)
			{
				if (string.Compare(xHead.ChildNodes[i].Name, "title", StringComparison.OrdinalIgnoreCase) == 0)
				{
					s = xHead.ChildNodes[i].InnerText;
					if (s != null) s = s.Trim();
					if (string.IsNullOrEmpty(s))
					{
						continue;
					}
					found = false;
					for (int j = 0; j < hHead.ChildNodes.Count; j++)
					{
						if (string.Compare(hHead.ChildNodes[j].Name, "title", StringComparison.OrdinalIgnoreCase) == 0)
						{
							found = true;
							hHead.ChildNodes[j].InnerHtml = s;
							break;
						}
					}
					if (found)
						continue;
				}
				else if (string.Compare(xHead.ChildNodes[i].Name, "meta", StringComparison.OrdinalIgnoreCase) == 0)
				{
					string name = XmlUtil.GetNameAttribute(xHead.ChildNodes[i]);
					if (string.Compare(name, "description", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(name, "keywords", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(name, "generator", StringComparison.OrdinalIgnoreCase) == 0)
					{
						s = XmlUtil.GetAttribute(xHead.ChildNodes[i], "content");
						if (s != null) s = s.Trim();
						if (string.IsNullOrEmpty(s))
						{
							continue;
						}
						found = false;
						for (int j = 0; j < hHead.ChildNodes.Count; j++)
						{
							string name2;
							if (string.Compare(hHead.ChildNodes[j].Name, "meta", StringComparison.OrdinalIgnoreCase) == 0)
							{
								name2 = getHtmlAttribute(hHead.ChildNodes[j], "name");
								if (string.Compare(name, name2, StringComparison.OrdinalIgnoreCase) == 0)
								{
									found = true;
									setHtmlAttribute(hHead.ChildNodes[j], "content", s);
									break;
								}
							}
						}
						if (found)
							continue;
					}
					else
					{
						name = XmlUtil.GetAttribute(xHead.ChildNodes[i], "http-equiv");
						if (string.Compare(name, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0)
						{
							s = XmlUtil.GetAttribute(xHead.ChildNodes[i], "content");
							if (s != null) s = s.Trim();
							if (string.IsNullOrEmpty(s))
							{
								continue;
							}
							found = false;
							for (int j = 0; j < hHead.ChildNodes.Count; j++)
							{
								string name2;
								if (string.Compare(hHead.ChildNodes[j].Name, "meta", StringComparison.OrdinalIgnoreCase) == 0)
								{
									name2 = getHtmlAttribute(hHead.ChildNodes[j], "http-equiv");
									if (string.Compare(name, name2, StringComparison.OrdinalIgnoreCase) == 0)
									{
										found = true;
										setHtmlAttribute(hHead.ChildNodes[j], "content", s);
										break;
									}
								}
							}
							if (found)
								continue;
						}
					}
				}
				CopyXmlNodeToHtml(hDoc, hHead, xHead.ChildNodes[i]);
			}
		}
		private static void mergeXmlBodyToHtml(HtmlDocument hDoc, HtmlNode hBody, XmlNode xBody)
		{
			for (int i = 0; i < xBody.ChildNodes.Count; i++)
			{
				CopyXmlNodeToHtml(hDoc, hBody, xBody.ChildNodes[i]);
			}
		}
		public static HtmlDocument MergeXmlDocumentIntoHtml(string htmlFile,bool enablePageCache, XmlDocument xmlDoc, string saveToFile, IList<HtmlElement_BodyBase> dbhtmlElements, IList<IHtmlElementCreateContents> contentHtmlElements,string iframeId, string serverFile)
		{
			HtmlDocument htmlDoc = null;
			if (xmlDoc != null && xmlDoc.DocumentElement != null)
			{
				htmlDoc = LoadHtmlDocument(htmlFile);
				if (htmlDoc != null && htmlDoc.DocumentNode != null)
				{
					HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("html");
					if (htmlNode != null)
					{
						XmlNode xHead = xmlDoc.DocumentElement.SelectSingleNode("head");
						if (xHead != null)
						{
							HtmlNode hHead = null;
							if (htmlNode.FirstChild != null && string.Compare(htmlNode.FirstChild.Name, "head", StringComparison.OrdinalIgnoreCase) == 0)
							{
								hHead = htmlNode.FirstChild;
							}
							if (hHead == null)
							{
								hHead = htmlDoc.CreateElement("head");
								if (htmlNode.FirstChild != null)
								{
									htmlNode.InsertBefore(hHead, htmlNode.FirstChild);
								}
								else
								{
									htmlNode.AppendChild(hHead);
								}
							}
							if (enablePageCache)
							{
								for (int j = 0; j < hHead.ChildNodes.Count; j++)
								{
									if (string.Compare(hHead.ChildNodes[j].Name, "meta", StringComparison.OrdinalIgnoreCase) == 0)
									{
										string val = getHtmlAttribute(hHead.ChildNodes[j], "http-equiv");
										if (string.Compare(val, "PRAGMA", StringComparison.OrdinalIgnoreCase) == 0)
										{
											hHead.RemoveChild(hHead.ChildNodes[j]);
											break;
										}
									}
								}
							}
							mergeXmlHeadToHtml(htmlDoc, hHead, xHead);
						}
						HtmlNode hBody = htmlNode.SelectSingleNode("body");
						if (hBody == null)
						{
							hBody = htmlDoc.CreateElement("body");
							htmlNode.AppendChild(hBody);
						}
						else
						{
							if (dbhtmlElements != null && dbhtmlElements.Count > 0)
							{
								foreach (HtmlElement_BodyBase hbb in dbhtmlElements)
								{
									HtmlElement_body body = hbb as HtmlElement_body;
									if (body != null)
									{
										setDataBind0(hBody, body);
										break;
									}
								}
								for (int i = 0; i < hBody.ChildNodes.Count; i++)
								{
									setDataBind(hBody.ChildNodes[i], dbhtmlElements);
								}
							}
						}
						XmlNode xBody = xmlDoc.DocumentElement.SelectSingleNode("body");
						if (xBody != null)
						{
							mergeXmlBodyToHtml(htmlDoc, hBody, xBody);
						}
						foreach (IHtmlElementCreateContents hecc in contentHtmlElements)
						{
							HtmlNode hn = hBody.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
								"//*[@id=\"{0}\"]", hecc.id));
							if (hn != null)
							{
								hecc.CreateHtmlContent(hn);
							}
						}
						removeLimnrId(htmlDoc.DocumentNode, iframeId, serverFile);
						htmlDoc.Save(saveToFile);
					}
				}
			}
			return htmlDoc;
		}
	}
}
