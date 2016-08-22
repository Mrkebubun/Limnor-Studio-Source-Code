/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer by WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using VPL;
using XmlUtility;
/*
 * Special folder IDs can be found below
 https://msdn.microsoft.com/en-us/library/windows/desktop/aa370905(v=vs.85).aspx#system_folder_properties
 */
namespace WixLib
{
	public class WixUtil
	{
		static XmlDocument doc;
		static XmlNamespaceManager _xm;
		static ShortCutList _shortcuts;
		static StringBuilder _errors;
		static IShowMessage _caller;
		static List<SpecialFolderInfo> _specialFolders;
		public const string uri = "http://schemas.microsoft.com/wix/2006/wi";
		static WixUtil()
		{
			_specialFolders = new List<SpecialFolderInfo>();
			_specialFolders.Add(new SpecialFolderInfo("SystemFolder", "System64Folder", "SystemFolder", "A5688785-BB0F-4E7D-9CD3-A0122FAE8DCB"));
			_specialFolders.Add(new SpecialFolderInfo("CommonAppDataFolder", "CommonAppDataFolder", "CommonAppDataComponent", "EF5085B3-A6D6-46BD-AD41-CD3590DF283F"));
		}
		static string formErrTxt(Exception e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(e.Message);
			sb.Append("\r\nStack trace:\r\n");
			sb.Append(e.StackTrace);
			if (e.InnerException != null)
			{
				sb.Append("\r\nInner exception\r\n");
				sb.Append(formErrTxt(e.InnerException));
			}
			return sb.ToString();
		}
		static void showError(string msg, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			if (_errors == null)
				_errors = new StringBuilder();
			_errors.Append(msg);
			_errors.Append("\r\n");
		}
		public static string GetNewId()
		{
			return string.Format(CultureInfo.InvariantCulture, "_{0}", Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
		}
		public static void passAttribute(XmlNode target, XmlNode src, string name)
		{
			XmlUtil.SetAttribute(target, name, XmlUtil.GetAttribute(src, name));
		}
		static void setProperty(XmlNode pn, string name, string value)
		{
			XmlNode p = pn.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "x:Property[@Id='{0}']", name), _xm);
			if (p == null)
			{
				p = pn.OwnerDocument.CreateElement("Property", uri);
				pn.AppendChild(p);
				XmlUtil.SetAttribute(p, "Id", name);
			}
			XmlUtil.SetAttribute(p, "Value", value);
		}
		public static string formFileId(string f)
		{
			string fn = Path.GetFileNameWithoutExtension(f);
			fn = Regex.Replace(fn, @"[^\w\.]", "_");
			return string.Format(CultureInfo.InvariantCulture, "F{0}{1}", fn, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
		}
		static void addSpecialFolder(SpecialFolderInfo sfolder, bool is64, XmlNode targetDirNode, XmlNode tempProd, XmlNode prodNode, XmlNode featureNode)
		{
			XmlNode sourceNode = prodNode.SelectSingleNode(sfolder.FolderName);
			if (sourceNode != null)
			{
				if (sourceNode.SelectNodes("SourceFile").Count > 0 || sourceNode.SelectNodes("SourceFolder").Count > 0)
				{
					XmlNode fn = tempProd.OwnerDocument.CreateElement("Directory", uri);
					targetDirNode.AppendChild(fn);
					if (is64)
					{
						XmlUtil.SetAttribute(fn, "Id", sfolder.FolderName64);
					}
					else
					{
						XmlUtil.SetAttribute(fn, "Id", sfolder.FolderName);
					}
					XmlUtil.SetAttribute(fn, "Name", sfolder.FolderName);
					XmlNode cn = tempProd.OwnerDocument.CreateElement("Component", uri);
					fn.AppendChild(cn);
					XmlUtil.SetAttribute(cn, "Id", sfolder.ComponentName);
					XmlUtil.SetAttribute(cn, "Guid", sfolder.GuidString);
					XmlNode cf = tempProd.OwnerDocument.CreateElement("ComponentRef", uri);
					featureNode.AppendChild(cf);
					XmlUtil.SetAttribute(cf, "Id", sfolder.ComponentName);
					addSources(sourceNode, featureNode, cn);
				}
			}
		}
		static void addFiles(XmlNode featureNode, XmlNode cNode, string[] files, StringCollection excludeFiles, StringCollection fileLst, StringCollection duplicated)
		{
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string ext = Path.GetExtension(files[i]).ToLowerInvariant();
					if (excludeFiles != null && excludeFiles.Contains(ext))
					{
						continue;
					}
					if (_shortcuts.IsShortcut(files[i], cNode.ParentNode, featureNode))
					{
						continue;
					}
					XmlNode fn = cNode.OwnerDocument.CreateElement("File", uri);
					string fn0 = Path.GetFileName(files[i]).ToLowerInvariant();
					if (fileLst.Contains(fn0))
					{
						duplicated.Add(files[i]);
					}
					else
					{
						fileLst.Add(fn0);
						XmlUtil.SetAttribute(fn, "Id", formFileId(files[i]));
						XmlUtil.SetAttribute(fn, "Name", Path.GetFileName(files[i]));
						XmlUtil.SetAttribute(fn, "DiskId", "1");
						XmlUtil.SetAttribute(fn, "Source", files[i]);
						if (cNode.ChildNodes.Count == 0)
						{
							XmlUtil.SetAttribute(fn, "KeyPath", "yes");
						}
						cNode.AppendChild(fn);
					}
				}
			}
		}
		static void addFilesFromFolder(XmlNode featureNode, XmlNode cNode, string folder, StringCollection excludeFolders, StringCollection excludeFiles, StringCollection fileLst, StringCollection duplicated)
		{
			if (Directory.Exists(folder))
			{
				string[] files = Directory.GetFiles(folder);
				addFiles(featureNode, cNode, files, excludeFiles, fileLst, duplicated);
				string[] folders = Directory.GetDirectories(folder);
				if (folders != null && folders.Length > 0)
				{
					for (int i = 0; i < folders.Length; i++)
					{
						string fn = Path.GetFileName(folders[i]);
						if (excludeFolders.Contains(fn.ToLowerInvariant()))
						{
							continue;
						}
						XmlNode dNode = cNode.OwnerDocument.CreateElement("Directory", uri);
						cNode.ParentNode.AppendChild(dNode);
						XmlUtil.SetAttribute(dNode, "Id", formFileId(folders[i]));
						XmlUtil.SetAttribute(dNode, "Name", Path.GetFileName(folders[i]));
						XmlNode subc = cNode.OwnerDocument.CreateElement("Component", uri);
						XmlUtil.SetAttribute(subc, "Id", formFileId(folders[i]));
						XmlUtil.SetAttribute(subc, "Guid", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
						dNode.AppendChild(subc);
						XmlNode cr = cNode.OwnerDocument.CreateElement("ComponentRef", uri);
						XmlUtil.SetAttribute(cr, "Id", XmlUtil.GetAttribute(subc, "Id"));
						featureNode.AppendChild(cr);
						XmlNode crF = cNode.OwnerDocument.CreateElement("CreateFolder", uri);
						subc.AppendChild(crF);
						addFilesFromFolder(featureNode, subc, folders[i], excludeFolders, excludeFiles, fileLst, duplicated);
					}
				}
			}
		}
		static string getVer(XmlNode prod)
		{
			if (prod != null)
			{
				string ver = XmlUtil.GetAttribute(prod, "Version");
				if (!string.IsNullOrEmpty(ver))
				{
					string[] ss = ver.Split('.');
					if (ss.Length > 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", ss[0], ss[1], ss[2]);
					}
				}
			}
			return null;
		}

		static string getExistVersion(string file, out string prodId)
		{
			prodId = string.Empty;
			if (File.Exists(file))
			{
				XmlDocument wixDoc = new XmlDocument();
				wixDoc.Load(file);
				if (wixDoc.DocumentElement != null)
				{
					XmlNamespaceManager xm = new XmlNamespaceManager(wixDoc.NameTable);
					xm.AddNamespace("x", uri);
					XmlNode prod = wixDoc.DocumentElement.SelectSingleNode("x:Product", xm);
					if (prod != null)
					{
						prodId = XmlUtil.GetAttribute(prod, "Id");
						if (!string.IsNullOrEmpty(prodId))
						{
							return getVer(prod);
						}
					}
				}
			}
			return null;
		}

		static bool exec(string exe, string c)
		{
			if (_caller != null)
			{
				_caller.ShowMessage(exe);
				_caller.ShowMessage(c);
			}
			if (!File.Exists(exe))
			{
				showError("File not found:{0}", exe);
				return false;
			}
			Process proc = new Process();
			ProcessStartInfo psI = new ProcessStartInfo("cmd");
			psI.UseShellExecute = false;
			psI.RedirectStandardInput = false;
			psI.RedirectStandardOutput = true;
			psI.RedirectStandardError = true;
			psI.CreateNoWindow = true;
			proc.StartInfo = psI;
			proc.StartInfo.FileName = exe;
			proc.StartInfo.Arguments = c;
			//
			string stdout;
			string errout;
			proc.Start();
			stdout = proc.StandardOutput.ReadToEnd();
			errout = proc.StandardError.ReadToEnd();
			proc.WaitForExit();
			if (proc.ExitCode != 0)
			{
				showError("Error code {0}, output:{1}, error:{2} for calling {3} {4}", proc.ExitCode, stdout, errout, proc.StartInfo.FileName, proc.StartInfo.Arguments);
				return false;
			}
			else
			{
				return true;
			}
		}
		private static void addSources(XmlNode sourceNode, XmlNode featureNode, XmlNode componentNode)
		{
			StringCollection files = new StringCollection();
			StringCollection duplicated = new StringCollection();
			XmlNodeList dirs = sourceNode.SelectNodes("SourceFolder");
			foreach (XmlNode dNode in dirs)
			{
				StringCollection excludeFolders = new StringCollection();
				StringCollection excludeFiles = new StringCollection();
				XmlNodeList exNs = dNode.SelectNodes("ExcludeFolder");
				foreach (XmlNode n in exNs)
				{
					excludeFolders.Add(XmlUtil.GetAttribute(n, "name"));
				}
				exNs = dNode.SelectNodes("ExcludeFile");
				foreach (XmlNode n in exNs)
				{
					excludeFiles.Add(XmlUtil.GetAttribute(n, "ext"));
				}
				string folder = XmlUtil.GetAttribute(dNode, "folder");
				addFilesFromFolder(featureNode, componentNode, folder, excludeFolders, excludeFiles, files, duplicated);
			}
			XmlNodeList fileNodes = sourceNode.SelectNodes("SourceFile");
			if (fileNodes.Count > 0)
			{
				string[] srcfiles = new string[fileNodes.Count];
				for (int i = 0; i < fileNodes.Count; i++)
				{
					srcfiles[i] = fileNodes[i].InnerText;
				}
				addFiles(featureNode, componentNode, srcfiles, null, files, duplicated);
			}
			XmlNodeList folders = sourceNode.SelectNodes("Folder");
			foreach (XmlNode dNode in folders)
			{
				string folder = XmlUtil.GetAttribute(dNode, "folder");
				XmlNode dnNode = componentNode.OwnerDocument.CreateElement("Directory", uri);
				componentNode.ParentNode.AppendChild(dnNode);
				XmlUtil.SetAttribute(dnNode, "Id", formFileId(folder));
				XmlUtil.SetAttribute(dnNode, "Name", Path.GetFileName(folder));
				XmlNode subc = componentNode.OwnerDocument.CreateElement("Component", uri);
				XmlUtil.SetAttribute(subc, "Id", formFileId(folder));
				XmlUtil.SetAttribute(subc, "Guid", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
				dnNode.AppendChild(subc);
				XmlNode cr = componentNode.OwnerDocument.CreateElement("ComponentRef", uri);
				XmlUtil.SetAttribute(cr, "Id", XmlUtil.GetAttribute(subc, "Id"));
				featureNode.AppendChild(cr);
				XmlNode crF = componentNode.OwnerDocument.CreateElement("CreateFolder", uri);
				subc.AppendChild(crF);
				addSources(dNode, featureNode, subc);
			}
			//
			if (duplicated.Count > 0)
			{
			}
		}

		public static string start(string projectFile, string outputfolder, string targetPlatform, IShowMessage caller)
		{
			XmlDocument docPrj = new XmlDocument();
			docPrj.Load(projectFile);
			return start(docPrj, outputfolder, targetPlatform, caller);
		}
		public static string start(XmlDocument docPrj, string outputfolder, string targetPlatform, IShowMessage caller)
		{
			_errors = new StringBuilder();
			_caller = caller;
			try
			{
				string msiFolder = outputfolder;
				outputfolder = Path.Combine(outputfolder, "obj");
				if (!Directory.Exists(outputfolder))
				{
					Directory.CreateDirectory(outputfolder);
				}
				doc = new XmlDocument();
				doc.LoadXml(Resource1.wixTemplate);
				_xm = new XmlNamespaceManager(doc.NameTable);
				_xm.AddNamespace("x", uri);
				XmlNode tempProd = doc.DocumentElement.SelectSingleNode("x:Product", _xm);
				XmlNode prodNode = docPrj.DocumentElement.SelectSingleNode("Product");
				_shortcuts = new ShortCutList(prodNode.SelectSingleNode("shortCuts"));
				//
				string netVer = XmlUtil.GetAttribute(prodNode, "netfrm");
				if (string.CompareOrdinal(netVer, "NETFRAMEWORK40FULL") != 0)
				{
					XmlNode netV = tempProd.SelectSingleNode("x:PropertyRef", _xm);
					XmlUtil.SetAttribute(netV, "Id", netV);
				}
				//
				string prodName = XmlUtil.GetAttribute(prodNode, "productName");
				string msiName = XmlUtil.GetAttribute(prodNode, "Name");
				//
				string upgId = XmlUtil.GetAttribute(prodNode, "UpgradeCode");

				XmlUtil.SetAttribute(tempProd, "UpgradeCode", upgId);
				string lastVer = XmlUtil.GetAttribute(prodNode, "LastVersion");
				if (string.CompareOrdinal(lastVer, XmlUtil.GetAttribute(prodNode, "Version")) == 0)
				{
					string lastId = XmlUtil.GetAttribute(prodNode, "LastId");
					if (string.IsNullOrEmpty(lastId))
					{
						XmlUtil.SetAttribute(tempProd, "Id", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
					}
					else
					{
						XmlUtil.SetAttribute(tempProd, "Id", lastId);
					}
				}
				else
				{
					XmlUtil.SetAttribute(tempProd, "Id", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
				}
				string ver = XmlUtil.GetAttribute(prodNode, "Version");
				XmlUtil.SetAttribute(tempProd, "Name", prodName);
				XmlUtil.SetAttribute(tempProd, "Version", ver);
				passAttribute(tempProd, prodNode, "Manufacturer");
				setProperty(tempProd, "DiskPrompt", string.Format(CultureInfo.InvariantCulture, "{0} Installation [1]", prodName));
				//
				string appIconPath = XmlUtil.GetAttribute(prodNode, "ARPPRODUCTICON");
				if (!string.IsNullOrEmpty(appIconPath) && File.Exists(appIconPath))
				{
					string appIconId = string.Format(CultureInfo.InvariantCulture, "app{0}.ico", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					XmlNode appIconNode = tempProd.OwnerDocument.CreateElement("Icon", uri);
					tempProd.AppendChild(appIconNode);
					XmlUtil.SetAttribute(appIconNode, "Id", appIconId);
					XmlUtil.SetAttribute(appIconNode, "SourceFile", appIconPath);
					XmlNode appIconPropNode = tempProd.OwnerDocument.CreateElement("Property", uri);
					tempProd.AppendChild(appIconPropNode);
					XmlUtil.SetAttribute(appIconPropNode, "Id", "ARPPRODUCTICON");
					XmlUtil.SetAttribute(appIconPropNode, "Value", appIconId);
				}
				//
				XmlNode uiref = doc.CreateElement("UIRef", uri);
				tempProd.AppendChild(uiref);
				XmlUtil.SetAttribute(uiref, "Id", "WixUI_InstallDir");
				setProperty(tempProd, "WIXUI_INSTALLDIR", "INSTALLDIR");
				//
				XmlNode wixUI;
				string licFIle = XmlUtil.GetAttribute(prodNode, "licFile");
				if (!string.IsNullOrEmpty(licFIle))
				{
					wixUI = doc.CreateElement("WixVariable", uri);
					tempProd.AppendChild(wixUI);
					XmlUtil.SetAttribute(wixUI, "Id", "WixUILicenseRtf");
					XmlUtil.SetAttribute(wixUI, "Value", licFIle);
				}
				//
				wixUI = doc.CreateElement("WixVariable", uri);
				tempProd.AppendChild(wixUI);
				XmlUtil.SetAttribute(wixUI, "Id", "WixUIBannerBmp");
				XmlUtil.SetAttribute(wixUI, "Value", XmlUtil.GetAttribute(prodNode, "banner"));
				//
				wixUI = doc.CreateElement("WixVariable", uri);
				tempProd.AppendChild(wixUI);
				XmlUtil.SetAttribute(wixUI, "Id", "WixUIDialogBmp");
				XmlUtil.SetAttribute(wixUI, "Value", XmlUtil.GetAttribute(prodNode, "dialog"));
				//
				XmlNode coDir = tempProd.SelectSingleNode("//x:Directory[@Id='CompanyFolder']", _xm);
				XmlUtil.SetAttribute(coDir, "Name", XmlUtil.GetAttribute(prodNode, "companyFolderName"));
				//
				XmlNode package = tempProd.SelectSingleNode("x:Package", _xm);
				string desc = XmlUtil.GetAttribute(prodNode, "Description");
				if (string.IsNullOrEmpty(desc))
				{
					desc = string.Empty;
				}
				XmlUtil.SetAttribute(package, "Description", desc);
				string comments = XmlUtil.GetAttribute(prodNode, "Comments");
				XmlUtil.SetAttribute(package, "Comments", comments);
				XmlUtil.SetAttribute(package, "Manufacturer", XmlUtil.GetAttribute(prodNode, "Manufacturer"));
				//
				XmlNode appSrc = prodNode.SelectSingleNode("appFolder");
				XmlNode appNode = tempProd.SelectSingleNode("//x:Directory[@Id='INSTALLDIR']", _xm);
				XmlUtil.SetAttribute(appNode, "Name", XmlUtil.GetAttribute(appSrc, "Name"));
				//
				XmlNode mainC = appNode.SelectSingleNode("x:Component[@Id='MainComponent']", _xm);
				XmlUtil.SetAttribute(mainC, "Guid", XmlUtil.GetAttribute(appSrc, "mainId"));
				//
				XmlNode mainF = tempProd.SelectSingleNode("x:Feature[@Id='MainFeature']", _xm);
				XmlUtil.SetAttribute(mainF, "Title", prodName);
				XmlUtil.SetAttribute(mainF, "Description", XmlUtil.GetAttribute(prodNode, "Description"));
				XmlUtil.SetAttribute(mainF, "ConfigurableDirectory", "INSTALLDIR");
				//
				string platform;
				if (string.IsNullOrEmpty(targetPlatform))
				{
					platform = XmlUtil.GetAttribute(docPrj.DocumentElement, "platform");
					if (string.IsNullOrEmpty(platform))
					{
						platform = "x86";
					}
				}
				else
				{
					platform = targetPlatform;
				}
				if (string.Compare(platform, "AnyCPU", StringComparison.OrdinalIgnoreCase) == 0)
				{
					platform = "x86";
				}
				bool is64 = (string.Compare(platform, "x64", StringComparison.OrdinalIgnoreCase) == 0);
				XmlNode targetDirNode = tempProd.SelectSingleNode("x:Directory[@Id='TARGETDIR']", _xm);
				//add SystemFolder/System64Folder and CommonAppDataFolder
				for (int i = 0; i < _specialFolders.Count; i++)
				{
					addSpecialFolder(_specialFolders[i], is64, targetDirNode, tempProd, prodNode, mainF);
				}
				//
				addSources(appSrc, mainF, mainC);
				XmlNodeList iconNodes = prodNode.SelectNodes("Icon");
				foreach (XmlNode inod in iconNodes)
				{
					XmlNode icN = tempProd.OwnerDocument.CreateElement("Icon", uri);
					tempProd.AppendChild(icN);
					passAttribute(icN, inod, "Id");
					passAttribute(icN, inod, "SourceFile");
				}
				//
				XmlNode menuDir = tempProd.SelectSingleNode("//x:Directory[@Id='ProgramMenuDir']", _xm);
				XmlNode menuGroup = prodNode.SelectSingleNode("MenuGroup");
				passAttribute(menuDir, menuGroup, "Name");
				XmlNode menuComp = menuDir.SelectSingleNode("//x:Component[@Id='ProgramMenuDir']", _xm);
				passAttribute(menuComp, menuGroup, "Guid");
				//
				_shortcuts.Create(_errors);
				//
				if (is64)
				{
					XmlNode prgNode = tempProd.SelectSingleNode("//x:Directory[@Id='ProgramFilesFolder']", _xm);
					XmlUtil.SetAttribute(prgNode, "Id", "ProgramFiles64Folder");
				}
				//
				//
				XmlNodeList cultrueNodes = docPrj.DocumentElement.SelectNodes("Cultures/Item");
				if (cultrueNodes == null || cultrueNodes.Count == 0)
				{
					throw new Exception("Cultures element is empty");
				}
				//
				XmlNode packageNode = tempProd.SelectSingleNode("x:Package", _xm);
				if (packageNode == null)
				{
					throw new Exception("Package element not found");
				}
				//
				string binDir = XmlUtil.GetAttribute(docPrj.DocumentElement, "wixbin");
				bool bOK = true;
				foreach (XmlNode cultureNode in cultrueNodes)
				{
					string cultureName = XmlUtil.GetAttribute(cultureNode, "name");
					if (string.IsNullOrEmpty(cultureName))
					{
						throw new Exception("Culture name is missing");
					}
					string codepage = XmlUtil.GetAttribute(cultureNode, "SummaryCodepage");
					if (string.IsNullOrEmpty(codepage))
					{
						XmlUtil.RemoveAttribute(packageNode, "SummaryCodepage");
					}
					else
					{
						XmlUtil.SetAttribute(packageNode, "SummaryCodepage", codepage);
					}
					string outWix = Path.Combine(outputfolder, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.wix", msiName, cultureName));
					doc.Save(outWix);
					string outObj = Path.Combine(outputfolder, string.Format(CultureInfo.InvariantCulture, "{0}.wixobj", Path.GetFileNameWithoutExtension(outWix)));
					string c = string.Format(CultureInfo.InvariantCulture, "-arch {0} -out \"{1}\" \"{2}\"", platform, outObj, outWix);
					string exe = Path.Combine(binDir, "candle.exe");
					if (exec(exe, c))
					{
						string langFile;
						string licFile;
						XmlNode nd = cultureNode.SelectSingleNode("LicFile");
						if (nd != null)
						{
							licFile = nd.InnerText;
						}
						else
						{
							licFile = string.Empty;
						}
						nd = cultureNode.SelectSingleNode("LanguageFile");
						if (nd != null)
						{
							langFile = nd.InnerText;
						}
						else
						{
							throw new Exception(string.Format(CultureInfo.InvariantCulture, "Language file not specified for culture {0}", cultureName));
						}
						string msi;
						msi = Path.Combine(msiFolder, string.Format(CultureInfo.InvariantCulture, "{0}.msi", Path.GetFileNameWithoutExtension(outWix)));
						string uriLng = "http://schemas.microsoft.com/wix/2006/localization";
						XmlDocument docLng = new XmlDocument();
						docLng.Load(langFile);
						XmlNamespaceManager xmlLng = new XmlNamespaceManager(docLng.NameTable);
						xmlLng.AddNamespace("x", uriLng);
						XmlNode ndLng = docLng.DocumentElement.SelectSingleNode("x:String[@Id='Version']", xmlLng);
						if (ndLng == null)
						{
							ndLng = docLng.CreateElement("String", uriLng);
							XmlUtil.SetAttribute(ndLng, "Id", "Version");
							docLng.DocumentElement.AppendChild(ndLng);
						}
						ndLng.InnerText = ver;
						//
						docLng.Save(langFile);
						//
						exe = Path.Combine(binDir, "light.exe");
						StringBuilder sb = new StringBuilder();
						//
						sb.Append("-ext WixUIExtension -ext WiXNetFxExtension -cultures:");
						sb.Append(cultureName);
						sb.Append(" -loc \"");
						sb.Append(langFile);
						sb.Append("\"");
						if (!string.IsNullOrEmpty(licFile))
						{
							sb.Append(" -dWixUILicenseRtf=\"");
							sb.Append(licFile);
							sb.Append("\"");
						}
						sb.Append(" -out \"");
						sb.Append(msi);
						sb.Append("\" \"");
						sb.Append(outObj);
						sb.Append("\"");
						c = sb.ToString();
						if (exec(exe, c))
						{
							if (_caller != null)
							{
								_caller.ShowMessage("MSI generated.");
							}
						}
						else
						{
							if (_caller != null)
							{
								_caller.LogError("Error calling light.exe for culture {0}, msi={1}", cultureName, msi);
							}
						}
						if (_errors.Length > 0)
						{
							if (_caller != null)
							{
								_caller.LogError(_errors.ToString());
								_errors = new StringBuilder();
							}
							bOK = false;
						}
					}
				}
				if (bOK)
				{
				}
				else
				{
					_errors.Append("Error building installer");
				}
			}
			catch (Exception err)
			{
				showError(formErrTxt(err));
			}
			return _errors.ToString();
		}
	}
}
