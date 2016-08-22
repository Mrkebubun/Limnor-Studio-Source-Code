using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;

namespace WixGen
{
	class Program
	{
		static XmlDocument doc;
		static XmlNamespaceManager _xm;
		static ShortCutList _shortcuts;
		static StringBuilder _errors;
		static string _projectFolder;
		public const string uri = "http://schemas.microsoft.com/wix/2006/wi";
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
			Console.WriteLine(msg);
			MessageBox.Show(msg, "Setup project compilation", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}", fn, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
		}
		static void addFiles(XmlNode featureNode, XmlNode cNode, string folder, StringCollection excludeFolders, StringCollection excludeFiles, StringCollection fileLst, StringCollection duplicated)
		{
			string[] files = Directory.GetFiles(folder);
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string ext = Path.GetExtension(files[i]).ToLowerInvariant();
					if (excludeFiles.Contains(ext))
					{
						continue;
					}
					if (_shortcuts.IsShortcut(files[i], cNode.ParentNode, featureNode))
					{
						continue;
					}
					XmlNode fn = cNode.OwnerDocument.CreateElement("File", uri);
					string fn0 = files[i].ToLowerInvariant();
					if (fileLst.Contains(fn0))
					{
						duplicated.Add(fn0);
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
					addFiles(featureNode, subc, folders[i], excludeFolders, excludeFiles, fileLst, duplicated);
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
			Console.WriteLine(exe);
			Console.WriteLine(c);
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
		static void Main(string[] args)
		{
			if (args == null || args.Length < 2)
			{
				showError("Usage: WixGen 'Setup Project File' 'output folder'");
			}
			else
			{
				try
				{
					string outDir = args[1];
					Console.Write("Project file:");
					Console.WriteLine(args[0]);
					Console.Write("Output folder:");
					Console.WriteLine(args[1]);
					_projectFolder = Path.GetDirectoryName(args[0]);
					_errors = new StringBuilder();
					doc = new XmlDocument();
					doc.LoadXml(Resource1.wixTemplate);
					_xm = new XmlNamespaceManager(doc.NameTable);
					_xm.AddNamespace("x", uri);
					XmlNode tempProd = doc.DocumentElement.SelectSingleNode("x:Product", _xm);
					//
					//replace project folders references in the setup XML file
					StreamReader sr = new StreamReader(args[0]);
					string setupXml = sr.ReadToEnd();
					sr.Close();
					setupXml = setupXml.Replace("$$SRC$$", _projectFolder);
					//
					XmlDocument docPrj = new XmlDocument();
					docPrj.LoadXml(setupXml);
					//
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
					string outWix = Path.Combine(outDir, string.Format(CultureInfo.InvariantCulture, "{0}.wix", msiName));
					//
					string prodId;
					string existVer = getExistVersion(outWix, out prodId);
					string newVer = getVer(prodNode);
					if (string.CompareOrdinal(existVer, newVer) == 0)
					{
						XmlUtil.SetAttribute(tempProd, "Id", prodId);
					}
					else
					{
						XmlUtil.SetAttribute(tempProd, "Id", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
					}
					passAttribute(tempProd, prodNode, "UpgradeCode");
					XmlUtil.SetAttribute(tempProd, "Name", prodName);
					passAttribute(tempProd, prodNode, "Version");
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
					XmlNode wixUI = doc.CreateElement("WixVariable", uri);
					tempProd.AppendChild(wixUI);
					XmlUtil.SetAttribute(wixUI, "Id", "WixUILicenseRtf");
					XmlUtil.SetAttribute(wixUI, "Value", XmlUtil.GetAttribute(prodNode, "licFile"));
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
					// XmlUtil.SetAttribute(package, "Id", Guid.NewGuid().ToString("N"));
					XmlUtil.SetAttribute(package, "Description", XmlUtil.GetAttribute(prodNode, "Description"));
					XmlUtil.SetAttribute(package, "Comments", XmlUtil.GetAttribute(prodNode, "Comments"));
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
					StringCollection files = new StringCollection();
					StringCollection duplicated = new StringCollection();
					XmlNodeList dirs = appSrc.SelectNodes("SourceFolder");
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
						addFiles(mainF, mainC, folder, excludeFolders, excludeFiles, files, duplicated);
					}
					//
					if (duplicated.Count > 0)
					{
						_errors.Append("Duplicated files used.\r\n");
						foreach (string f in duplicated)
						{
							_errors.Append(f);
							_errors.Append("\r\n");
						}
						//showError("Duplicated files are used
					}
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
					doc.Save(outWix);
					//
					string outObj = Path.Combine(outDir, string.Format(CultureInfo.InvariantCulture, "{0}.wixobj", Path.GetFileNameWithoutExtension(outWix)));
					//
					string binDir = XmlUtil.GetAttribute(docPrj.DocumentElement, "wixbin");
					string exe = Path.Combine(binDir, "candle.exe");
					string c = string.Format(CultureInfo.InvariantCulture, "-out \"{0}\" \"{1}\"", outObj, outWix);
					if (exec(exe, c))
					{
						string msi = Path.Combine(outDir, string.Format(CultureInfo.InvariantCulture, "{0}.msi", Path.GetFileNameWithoutExtension(outWix)));
						exe = Path.Combine(binDir, "light.exe");
						c = string.Format(CultureInfo.InvariantCulture, "-ext WixUIExtension -ext WiXNetFxExtension -out \"{0}\" \"{1}\"", msi, outObj);
						if (exec(exe, c))
						{
							if (_errors.Length == 0)
							{
#if DEBUG
                                MessageBox.Show("OK");
#endif
							}
						}
					}
					if (_errors.Length > 0)
					{
						MessageBox.Show(_errors.ToString(), "Installation Compilation", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch (Exception err)
				{
					showError(formErrTxt(err));
				}
			}
		}
	}
}
