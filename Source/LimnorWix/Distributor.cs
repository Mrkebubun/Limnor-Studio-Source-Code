/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using VPL;
using WixLib;

namespace LimnorWix
{
	public class Distributor : IDistributor
	{
		private WixXmlRoot _root;
		public Distributor()
		{
		}
		
		public void CollectAddedFiles(string contentFile, List<IDistributeFile> distrubtedFiles)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(contentFile);
			_root = new WixXmlRoot(doc.DocumentElement, contentFile);
			List<WixSourceFileNode> lst = new List<WixSourceFileNode>();
			_root.CollectAddedFiles(lst);
			foreach (WixSourceFileNode f in lst)
			{
				distrubtedFiles.Add(f);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pl"></param>
		/// <param name="log">logging messages</param>
		/// <param name="xmlContentFile"></param>
		/// <param name="targetName"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public string Compile(EnumPlatform pl, IShowMessage log, string xmlContentFile, string targetName, string mode)
		{
			string sError = string.Empty;
			string targetFolder = Path.Combine(Path.GetDirectoryName(xmlContentFile), string.Format(CultureInfo.InvariantCulture, "bin\\{0}", mode));
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlContentFile);
			_root = new WixXmlRoot(doc.DocumentElement, xmlContentFile);
			_root.Preprocess(targetName, mode, log);
			sError = WixUtil.start(doc, targetFolder, pl == EnumPlatform.x64 ? "x64" : "x86", log);
			if (string.IsNullOrEmpty(sError))
			{
				string curId = _root.General.ProductId;
				doc = new XmlDocument();
				doc.Load(xmlContentFile);
				_root = new WixXmlRoot(doc.DocumentElement, xmlContentFile);
				_root.General.LastProductId = curId;
				_root.Postprocess(targetName, mode, log);
			}
			return sError;
		}
	}
}
