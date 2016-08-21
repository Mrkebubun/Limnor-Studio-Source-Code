using System;
using System.Collections.Generic;
using System.Text;
using mshtml;
using System.Runtime.InteropServices;
using System.Xml;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

namespace LimnorDesigner.Web
{
    public static class HtmlUtility
    {
        public static HTMLDocumentClass LoadHtmlDocument(string htmlFile, int timeout)
        {
            HTMLDocumentClass doc = null;
            doc = new HTMLDocumentClass();
            //UCOMIPersistFile persistFile = (UCOMIPersistFile)doc;
            IPersistFile persistFile = (IPersistFile)doc;
            persistFile.Load(htmlFile, 0);
            int start = Environment.TickCount;
            while (string.CompareOrdinal(doc.readyState, "complete") != 0)
            {
                System.Windows.Forms.Application.DoEvents();
                if (Environment.TickCount - start > timeout)
                {
                    throw new Exception(string.Format("Error loading {0}. Timeout: {1}.", htmlFile, timeout));
                }
            }
            return doc;
        }

        public static void CopyXmlNodeToHtml(HTMLDocumentClass hDoc, IHTMLDOMNode hNode, XmlNode xNode)
        {
            IHTMLElement ie = hDoc.createElement(xNode.Name);
            IHTMLDOMNode hn = ie as IHTMLDOMNode;
            hNode.appendChild(hn);
            if (xNode.NodeType == XmlNodeType.CDATA)
            {
                ie.innerText = xNode.Value;
            }
            else if (xNode.NodeType == XmlNodeType.Text)
            {
                ie.innerText = xNode.InnerText;
            }
            else
            {
                foreach (XmlNode nc in xNode.ChildNodes)
                {
                    if (nc.NodeType == XmlNodeType.Text)
                    {
                        try
                        {
                            ie.innerText = nc.InnerText;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        CopyXmlNodeToHtml(hDoc, hn, nc);
                    }
                }
            }
            if (xNode.Attributes != null)
            {
                foreach (XmlAttribute xa in xNode.Attributes)
                {
                    ie.setAttribute(xa.Name, xa.Value, 0);
                }
            }
            if (xNode.ChildNodes.Count > 0)
            {
            }
            //ie.outerHTML = xNode.OuterXml;
            //IHTMLAttributeCollection attrs = hn.attributes as IHTMLAttributeCollection;

            //
            //hn.tx xHead.ChildNodes[i].OuterXml 
            
        }
        public static void MergeXmlHeadToHtml(HTMLDocumentClass hDoc, IHTMLDOMNode hHead, XmlNode xHead)
        {
            for (int i = 0; i < xHead.ChildNodes.Count; i++)
            {
                CopyXmlNodeToHtml(hDoc,hHead,xHead.ChildNodes[i]);
            }
        }

        public static HTMLDocumentClass MergeXmlDocumentIntoHtml(string htmlFile, XmlDocument xmlDoc, string saveToFile)
        {
            HTMLDocumentClass htmlDoc = null;
            if (xmlDoc != null && xmlDoc.DocumentElement != null)
            {
                htmlDoc = LoadHtmlDocument(htmlFile, 10000);
                if (htmlDoc != null)
                {
                    IHTMLDOMNode htmlNode = htmlDoc.documentElement as IHTMLDOMNode;
                    if (htmlNode != null)
                    {
                        XmlNode xHead = xmlDoc.DocumentElement.SelectSingleNode("head");
                        if (xHead != null)
                        {
                            IHTMLDOMNode hHead = null;
                            if (htmlNode.firstChild != null && string.Compare(htmlNode.firstChild.nodeName, "head", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                hHead = htmlNode.firstChild;
                            }
                            if (hHead == null)
                            {
                                hHead = htmlDoc.createElement("head") as IHTMLDOMNode;
                                if (htmlNode.firstChild != null)
                                {
                                    htmlNode.insertBefore(hHead, htmlNode.firstChild);
                                }
                                else
                                {
                                    htmlNode.appendChild(hHead);
                                }
                            }
                            MergeXmlHeadToHtml(htmlDoc, hHead, xHead);
                        }
                        IHTMLDocument3 hdoc = htmlDoc as IHTMLDocument3;
                        StreamWriter sw = new StreamWriter(saveToFile);
                        sw.Write(hdoc.documentElement.outerHTML);
                        sw.Close();
                    }
                }
            }
            return htmlDoc;
        }
    }
}
