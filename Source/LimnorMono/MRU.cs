/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using VOB;
using SolutionMan;
using MathExp;

namespace LimnorVOB
{
	class MRU
	{
		const string XML_Filepath = "filepath";
		int maxMru = 10;
		string _file;
		public MRU()
		{
			_file = System.IO.Path.Combine(VobUtil.AppDataFolder, "LimnorVOB.mru");
		}
		public MRU(string file)
		{
		}
		public string[] GetMruFiles()
		{
			if (!string.IsNullOrEmpty(_file))
			{
				if (System.IO.File.Exists(_file))
				{
					try
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(_file);
						if (doc.DocumentElement != null)
						{
							List<string> files = new List<string>();
							for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
							{
								XmlNode node = doc.DocumentElement.ChildNodes[i];
								if (node.Name.Equals(NodeDataSolution.XML_Solution))
								{
									string s = XmlSerialization.GetAttribute(node, XML_Filepath);
									if (!string.IsNullOrEmpty(s))
									{
										s = s.Trim();
										if (s.Length > 0 && System.IO.File.Exists(s))
										{
											files.Add(s);
										}
									}
								}
							}
							string[] ss = new string[files.Count];
							files.CopyTo(ss);
							return ss;
						}
					}
					catch
					{
					}
				}
			}
			return new string[0];
		}
		public void SaveMRU(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					string[] ss = GetMruFiles();
					string[] ss2 = new string[ss.Length + 1];
					ss2[0] = file;
					for (int i = 0, j = 1; i < ss.Length; i++)
					{
						if (ss[i] != null && ss[i].Length > 0)
						{
							if (string.Compare(file, ss[i], StringComparison.OrdinalIgnoreCase) != 0)
							{
								ss2[j++] = ss[i];
							}
						}
					}
					XmlDocument doc = new XmlDocument();
					doc.AppendChild(doc.CreateElement("MRU_Solutions"));
					for (int i = 0; i < ss2.Length && i < maxMru; i++)
					{
						if (!string.IsNullOrEmpty(ss2[i]))
						{
							XmlNode node = doc.CreateElement(NodeDataSolution.XML_Solution);
							XmlSerialization.SetAttribute(node, XML_Filepath, ss2[i]);
							doc.DocumentElement.AppendChild(node);
						}
					}
					doc.Save(_file);
				}
			}
		}
		public void ReplaceMRU(string oldFile, string newFile)
		{
			if (!string.IsNullOrEmpty(newFile))
			{
				if (System.IO.File.Exists(newFile))
				{
					string[] ss = GetMruFiles();
					string[] ss2 = new string[ss.Length + 1];
					ss2[0] = newFile;
					for (int i = 0, j = 1; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							if (newFile != ss[i] && oldFile != ss[i])
							{
								ss2[j++] = ss[i];
							}
						}
					}
					XmlDocument doc = new XmlDocument();
					doc.AppendChild(doc.CreateElement("MRU_Solutions"));
					for (int i = 0; i < ss2.Length && i < maxMru; i++)
					{
						if (!string.IsNullOrEmpty(ss2[i]))
						{
							XmlNode node = doc.CreateElement(NodeDataSolution.XML_Solution);
							XmlSerialization.SetAttribute(node, XML_Filepath, ss2[i]);
							doc.DocumentElement.AppendChild(node);
						}
					}
					doc.Save(_file);
				}
			}
		}
	}
}
