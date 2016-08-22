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
using XmlUtility;
using LimnorDesigner;

namespace SolutionMan.Solution
{
	public class ProjectTemplate : EntityTemplate
	{
		public ProjectTemplate(string file)
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
		public List<NewComponentResult> GenerateProject(string projectFile, Dictionary<string, string> replaceNames)
		{
			List<NewComponentResult> lst = new List<NewComponentResult>();
			XmlNode prjNode = Document.DocumentElement.SelectSingleNode("x:TemplateContent/x:Project", XmlNamespace);
			if (prjNode == null)
			{
				throw new DesignerException("Invalid project template file. Project node missing. [{0}]", EntityTemplateFile);
			}
			string prjFile = XmlUtil.GetAttribute(prjNode, "File");
			if (string.IsNullOrEmpty(prjFile))
			{
				throw new DesignerException("Invalid project template file. Project file missing. [{0}]", EntityTemplateFile);
			}
			string templateDir = TemplateFolder;
			string prjfilePath = Path.Combine(templateDir, prjFile);
			if (!File.Exists(prjfilePath))
			{
				throw new DesignerException("Invalid project template file. Project file not found [{0}]. [{1}]", prjfilePath, EntityTemplateFile);
			}
			string sTargetDir = Path.GetDirectoryName(projectFile);
			File.Copy(prjfilePath, projectFile);
			string vobfile = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.vob", EntityTemplateFile);
			if (File.Exists(vobfile))
			{
				string vobfil2 = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.vob", projectFile);
				File.Copy(vobfile, vobfil2);
			}
			XmlNodeList nodes = prjNode.SelectNodes("x:ProjectItem", XmlNamespace);
			foreach (XmlNode nd in nodes)
			{
				string f = nd.InnerText;
				if (string.IsNullOrEmpty(f))
				{
					throw new DesignerException("Invalid project template file. ProjectItem missing file. [{0}]", EntityTemplateFile);
				}
				string f1 = Path.Combine(templateDir, f);
				string f0 = Path.Combine(sTargetDir, f);
				if (replaceNames != null && replaceNames.Count > 0)
				{
					StreamReader sr = new StreamReader(f1);
					string cnt = sr.ReadToEnd();
					StreamWriter sw = new StreamWriter(f0, false, sr.CurrentEncoding);
					sr.Close();
					foreach (KeyValuePair<string, string> kv in replaceNames)
					{
						cnt = cnt.Replace(kv.Key, kv.Value);
					}
					sw.Write(cnt);
					sw.Close();
				}
				else
				{
					File.Copy(f1, f0);
				}
				bool open = XmlUtil.GetAttributeBoolDefFalse(nd, "OpenInEditor");
				NewComponentResult nc = new NewComponentResult(f, open);
				lst.Add(nc);
			}
			return lst;
		}
	}
}
