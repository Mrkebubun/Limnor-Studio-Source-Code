/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;

namespace SolutionMan.Solution
{
	public class ItemTemplate : EntityTemplate
	{
		public ItemTemplate(string file)
			: base(file)
		{
		}
		public string GetWizardName()
		{
			XmlNode contentNode = Document.DocumentElement.SelectSingleNode("x:WizardExtension/x:FullClassName", XmlNamespace);
			if (contentNode != null)
			{
				return contentNode.InnerText.Trim();
			}
			return string.Empty;
		}
		public string GetClassTemplate()
		{
			XmlNode contentNode = Document.DocumentElement.SelectSingleNode("x:TemplateContent/x:ProjectItem", XmlNamespace);
			if (contentNode != null)
			{
				return contentNode.InnerText.Trim();
			}
			return string.Empty;
		}
	}
}
