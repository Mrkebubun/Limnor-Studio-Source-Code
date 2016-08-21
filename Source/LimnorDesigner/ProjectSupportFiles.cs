/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.Xml;
using XmlUtility;
using System.Globalization;
using System.IO;
using VPL;

namespace LimnorDesigner
{
	public class ProjectSupportFiles : FilePathList
	{
		const string XML_SupportFiles = "SupportFiles";
		private LimnorProject _prj;
		public ProjectSupportFiles(LimnorProject project)
		{
			_prj = project;
			using (XmlDoc d = _prj.GetVob())
			{
				XmlNode node = d.Doc.DocumentElement;
				if (node != null)
				{
					XmlNodeList nds = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
						"{0}/{1}", XML_SupportFiles, XmlTags.XML_Item));
					if (nds != null && nds.Count > 0)
					{
						bool adjusted = false;
						foreach (XmlNode nd in nds)
						{
							string s = nd.InnerText.Trim();
							if (!string.IsNullOrEmpty(s))
							{
								if (!File.Exists(s))
								{
									string sm = XmlUtil.GetFileMapping(s);
									if (!string.IsNullOrEmpty(sm))
									{
										s = sm;
									}
									else
									{
										DialogFilePath dlg = new DialogFilePath();
										dlg.LoadData(s, null);
										if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
										{
											s = dlg.AdjustedPath;
											nd.InnerText = s;
											adjusted = true;
										}
										if (VPLUtil.Shutingdown)
										{
											return;
										}
									}
								}
								this.AddFile(s);
							}
						}
						if (adjusted)
						{
							d.Save();
						}
					}
				}
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Project support files ({0})", Count);
		}
		protected override void OnFileListChanged()
		{
			using (XmlDoc d = _prj.GetVob())
			{
				XmlNode node = LimnorProject.createVobRootNode(d);
				XmlNode ndFiles = XmlUtil.CreateSingleNewElement(node, XML_SupportFiles);
				ndFiles.RemoveAll();
				for (int i = 0; i < this.Count; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					ndFiles.AppendChild(nd);
					nd.InnerText = this[i];
				}
				d.Save();
			}
		}
	}
}
