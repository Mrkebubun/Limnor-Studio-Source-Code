/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VPL;
using VSPrj;
using WindowsUtility;
using XmlUtility;

namespace LimnorWix
{
	public interface IWixFolder
	{
		IWixFolder Parent { get; }
		IList<WixFolderNode> Folders { get; }
		IList<WixSourceFileNode> Files { get; }
		void ResetSubFolders();
		string FullPathWithToken { get; }
	}
	public interface IWixTopFolder : IWixFolder
	{
		string TopFolderToken { get; }
	}
	#region WixNode
	public abstract class WixNode
	{
		public const string XML_SourceFile = "SourceFile";
		public const string XML_SourceFolder = "SourceFolder";
		public const string XML_PROJECT = "Project";
		public const string WixUrl_Localize = "http://schemas.microsoft.com/wix/2006/localization";
		private XmlNode _node;
		public WixNode(XmlNode node)
		{
			_node = node;
		}
		public abstract void OnBeforeSave();
		/// <summary>
		/// adjust XML before processing by WixUtil. WixUtil uses Simplified Wix XML
		/// WixXmlRoot uses Limnor Setup XML. This function adjusts Limnor Setup XML to be Simplified Wix XML.
		/// WixUtil converts Simplified Wix XML to be Wix XML. Candle.exe compiles Wix XML.
		/// </summary>
		/// <param name="setupName"></param>
		/// <param name="mode"></param>
		/// <param name="log"></param>
		public virtual void Preprocess(string setupName, string mode, IShowMessage log)
		{
		}
		public virtual void Postprocess(string setupName, string mode, IShowMessage log)
		{
		}
		[Browsable(false)]
		public XmlNode XmlData
		{
			get
			{
				return _node;
			}
		}
	}
	#endregion
	#region WixXmlRoot
	public class WixXmlRoot : WixNode
	{
		private string _xmlFile;
		private WixGeneral _general;
		private WixAppFolder _appFolder;
		private WixIconCollection _icons;
		private WixShortcutCollection _shortcuts;
		private WixSystemFolder _sysfolder;
		private WixCommonAppDataFolder _commonDataFolder;
		private WixCultureCollection _cultures;
		public WixXmlRoot(XmlNode node, string xmlFile)
			: base(node)
		{
			_xmlFile = xmlFile;
		}
		public bool CheckOldSchema()
		{
			int ver = XmlUtil.GetAttributeInt(XmlData, "ver");
			bool needAdjust = (ver < 2);
			if (needAdjust)
			{
				XmlUtil.SetAttribute(XmlData, "ver", 2);
				XmlUtil.SetAttribute(XmlData, "wixbin", Path.GetDirectoryName(Application.ExecutablePath));
				XmlNode product = XmlData.OwnerDocument.CreateElement("Product");
				XmlData.AppendChild(product);
				//Create Product attribute
				XmlUtil.SetAttribute(product, "Name", "MySetup");
				//
				XmlNode valNode = XmlData.SelectSingleNode("General/ProductVersion");
				if (valNode != null)
				{
					XmlUtil.SetAttribute(product, "Version", valNode.InnerText);
				}
				else
				{
					XmlUtil.SetAttribute(product, "Version", "1.0.0");
				}
				//
				valNode = XmlData.SelectSingleNode("General/ProductCode");
				if (valNode != null)
				{
					XmlUtil.SetAttribute(product, "Id", valNode.InnerText);
				}
				else
				{
					XmlUtil.SetAttribute(product, "Id", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
				}
				//
				valNode = XmlData.SelectSingleNode("General/UpgradeCode");
				if (valNode != null && !string.IsNullOrEmpty(valNode.InnerText))
				{
					XmlUtil.SetAttribute(product, "UpgradeCode", valNode.InnerText);
				}
				else
				{
					XmlUtil.SetAttribute(product, "UpgradeCode", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
				}
				XmlNode prgmenu = XmlData.OwnerDocument.CreateElement("MenuGroup");
				product.AppendChild(prgmenu);
				XmlUtil.SetAttribute(prgmenu, "Name", "!(loc.ApplicationName)");
				XmlUtil.SetAttribute(prgmenu, "Guid", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
				//
				XmlUtil.SetAttribute(product, "companyFolderName", "My Company");
				XmlUtil.SetAttribute(product, "licFile", "");
				XmlUtil.SetAttribute(product, "netfrm", "NETFRAMEWORK40FULL");
				//
				XmlUtil.SetAttribute(product, "Manufacturer", "!(loc.ManufacturerName)");
				XmlUtil.SetAttribute(product, "Description", "!(loc.SetupDesc)");
				XmlUtil.SetAttribute(product, "Comments", "!(loc.SetupComments)");
				XmlUtil.SetAttribute(product, "productName", "!(loc.ApplicationName)");
				//
				XmlUtil.SetAttribute(product, "banner", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "banner.bmp"));
				XmlUtil.SetAttribute(product, "dialog", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "dialog.bmp"));
				XmlUtil.SetAttribute(product, "ARPPRODUCTICON", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "LimnorMain.ico"));
				//
				XmlNodeList fileNodes = XmlData.SelectNodes("SystemFolder/File");
				XmlNode FilesHolder = XmlUtil.CreateSingleNewElement(product, "SystemFolder");
				foreach (XmlNode nd in fileNodes)
				{
					XmlNode fn = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFile);
					FilesHolder.AppendChild(fn);
					fn.InnerText = nd.InnerText;
				}
				//
				fileNodes = XmlData.SelectNodes("CommonProgramDataFolder/File");
				FilesHolder = XmlUtil.CreateSingleNewElement(product, "CommonAppDataFolder");
				foreach (XmlNode nd in fileNodes)
				{
					XmlNode fn = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFile);
					FilesHolder.AppendChild(fn);
					fn.InnerText = nd.InnerText;
				}
				//
				fileNodes = XmlData.SelectNodes("AppFolder/File");
				FilesHolder = XmlUtil.CreateSingleNewElement(product, "appFolder");
				foreach (XmlNode nd in fileNodes)
				{
					XmlNode fn = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFile);
					FilesHolder.AppendChild(fn);
					fn.InnerText = nd.InnerText;
				}
				fileNodes = XmlData.SelectNodes("AppFolder/Project");
				foreach (XmlNode nd in fileNodes)
				{
					XmlNode fn = XmlData.OwnerDocument.CreateElement("Project");
					FilesHolder.AppendChild(fn);
					fn.InnerText = nd.InnerText;
				}
				//generate a default culture
				createCultureXmlNode(string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name) ? "en-US" : CultureInfo.CurrentCulture.Name, true);
				XmlData.OwnerDocument.Save(_xmlFile);
				return true;
			}
			return false;
		}
		public XmlNode createCultureXmlNode(string culturename, bool isUpgrade)
		{
			XmlNode Cultures = XmlUtil.CreateSingleNewElement(XmlData, "Cultures");
			XmlNode item = XmlData.OwnerDocument.CreateElement("Item");
			Cultures.AppendChild(item);
			XmlUtil.SetAttribute(item, "name", culturename);
			string lng = Path.Combine(Path.GetDirectoryName(XmlFile), string.Format(CultureInfo.InvariantCulture, "{0}_{1}.wxl", Path.GetFileNameWithoutExtension(XmlFile), culturename));
			XmlNode lngNode = XmlData.OwnerDocument.CreateElement("LanguageFile");
			item.AppendChild(lngNode);
			lngNode.InnerText = lng;
			XmlNode lic = XmlData.OwnerDocument.CreateElement("LicFile");
			item.AppendChild(lic);
			if (isUpgrade)
			{
				XmlNode lic0 = XmlData.SelectSingleNode("//LicenseFile");
				if (lic0 != null && !string.IsNullOrEmpty(lic0.InnerText))
				{
					lic.InnerText = lic0.InnerText;
				}
			}
			XmlDocument docLng = new XmlDocument();
			XmlDeclaration xdl = docLng.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
			docLng.AppendChild(xdl);
			XmlNamespaceManager xm = new XmlNamespaceManager(docLng.NameTable);
			xm.AddNamespace("x", WixNode.WixUrl_Localize);
			XmlNode localNode = docLng.CreateElement("WixLocalization", WixNode.WixUrl_Localize);
			docLng.AppendChild(localNode);
			XmlUtil.SetAttribute(localNode, "Culture", culturename);
			XmlNode strNode = docLng.CreateElement("String", WixNode.WixUrl_Localize);
			localNode.AppendChild(strNode);
			XmlUtil.SetAttribute(strNode, "Id", "ApplicationName");
			strNode.InnerText = "My Product Name";
			//
			strNode = docLng.CreateElement("String", WixNode.WixUrl_Localize);
			localNode.AppendChild(strNode);
			XmlUtil.SetAttribute(strNode, "Id", "SetupDesc");
			strNode.InnerText = "My Product Setup Subject";
			//
			strNode = docLng.CreateElement("String", WixNode.WixUrl_Localize);
			localNode.AppendChild(strNode);
			XmlUtil.SetAttribute(strNode, "Id", "SetupComments");
			strNode.InnerText = "My Product Setup Description";
			//
			strNode = docLng.CreateElement("String", WixNode.WixUrl_Localize);
			localNode.AppendChild(strNode);
			XmlUtil.SetAttribute(strNode, "Id", "ManufacturerName");
			strNode.InnerText = "My Company Name";
			if (isUpgrade)
			{
				XmlNode str0 = XmlData.SelectSingleNode("General/Manufacturer");
				if (str0 != null)
				{
					strNode.InnerText = str0.InnerText;
				}
			}
			//
			strNode = docLng.CreateElement("String", WixNode.WixUrl_Localize);
			localNode.AppendChild(strNode);
			XmlUtil.SetAttribute(strNode, "Id", "WelcomeDlgDescription");
			strNode.InnerText = "The installer will guide you through the steps required to install [ProductName] on your computer.";
			if (isUpgrade)
			{
				XmlNode str0 = XmlData.SelectSingleNode("UI/Welcome");
				if (str0 != null)
				{
					strNode.InnerText = str0.InnerText;
				}
			}
			//
			docLng.Save(lng);
			return item;
		}
		/// <summary>
		/// adjust XML before processing by WixUtil. WixUtil uses Simplified Wix XML
		/// WixXmlRoot uses Limnor Setup XML. This function adjusts Limnor Setup XML to be Simplified Wix XML.
		/// WixUtil converts Simplified Wix XML to be Wix XML. Candle.exe compiles Wix XML.
		/// </summary>
		/// <param name="setupName"></param>
		/// <param name="mode"></param>
		/// <param name="log"></param>
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			CheckOldSchema();
			AppFolder.ValidateProjectNames();
			SystemFolder.ValidateProjectNames();
			CommonDataFolder.ValidateProjectNames();
			Shortcuts.Preprocess(setupName, mode, log);
			General.Preprocess(setupName, mode, log);
			_appFolder.Preprocess(setupName, mode, log);
			_sysfolder.Preprocess(setupName, mode, log);
			_commonDataFolder.Preprocess(setupName, mode, log);
		}
		public override void Postprocess(string setupName, string mode, IShowMessage log)
		{
			General.Postprocess(setupName, mode, log);
			XmlData.OwnerDocument.Save(_xmlFile);
		}
		public void CollectAddedFiles(List<WixSourceFileNode> list)
		{
			AppFolder.CollectAddedFiles(list);
			SystemFolder.CollectAddedFiles(list);
			CommonDataFolder.CollectAddedFiles(list);
		}
		public void OnIconIdChanged(string oldIconId, string newIconId)
		{
			Shortcuts.OnIconIdChanged(oldIconId, newIconId);
		}
		public override void OnBeforeSave()
		{
			if (_cultures != null)
			{
				_cultures.OnBeforeSave();
			}
			if (_appFolder != null)
			{
				_appFolder.OnBeforeSave();
			}
			if (_sysfolder != null)
			{
				_sysfolder.OnBeforeSave();
			}
			if (_commonDataFolder != null)
			{
				_commonDataFolder.OnBeforeSave();
			}
		}
		public string XmlFile
		{
			get
			{
				return _xmlFile;
			}
		}
		public WixGeneral General
		{
			get
			{
				if (_general == null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					_general = new WixGeneral(nd);
				}
				return _general;
			}
		}
		public WixAppFolder AppFolder
		{
			get
			{
				if (_appFolder == null)
				{
					XmlNode prod = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					XmlNode nd = XmlUtil.CreateSingleNewElement(prod, "appFolder");
					_appFolder = new WixAppFolder(nd);
				}
				return _appFolder;
			}
		}
		public WixIconCollection Icons
		{
			get
			{
				if (_icons == null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					_icons = new WixIconCollection(nd);
				}
				return _icons;
			}
		}
		public WixShortcutCollection Shortcuts
		{
			get
			{
				if (_shortcuts == null)
				{
					XmlNode prod = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					XmlNode nd = XmlUtil.CreateSingleNewElement(prod, "shortCuts");
					_shortcuts = new WixShortcutCollection(nd);
				}
				return _shortcuts;
			}
		}

		public WixSystemFolder SystemFolder
		{
			get
			{
				if (_sysfolder == null)
				{
					XmlNode prod = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					XmlNode nd = XmlUtil.CreateSingleNewElement(prod, "SystemFolder");
					_sysfolder = new WixSystemFolder(nd);
				}
				return _sysfolder;
			}
		}
		public WixCommonAppDataFolder CommonDataFolder
		{
			get
			{
				if (_commonDataFolder == null)
				{
					XmlNode prod = XmlUtil.CreateSingleNewElement(XmlData, "Product");
					XmlNode nd = XmlUtil.CreateSingleNewElement(prod, "CommonAppDataFolder");
					_commonDataFolder = new WixCommonAppDataFolder(nd);
				}
				return _commonDataFolder;
			}
		}
		public WixCultureCollection Cultures
		{
			get
			{
				if(_cultures == null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(XmlData, "Cultures");
					_cultures = new WixCultureCollection(nd);
				}
				return _cultures;
			}
		}
	}
	#endregion
	#region WixGeneral
	public class WixGeneral : WixNode
	{
		public event EventHandler VersionChanged;
		public WixGeneral(XmlNode node)
			: base(node)
		{
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		public override void Postprocess(string setupName, string mode, IShowMessage log)
		{
			XmlUtil.SetAttribute(XmlData, "LastVersion", this.Version);
		}
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			XmlUtil.SetAttribute(XmlData, "Name", setupName);
			XmlNode nd = XmlData.SelectSingleNode("MenuGroup");
			if (nd != null)
			{
				XmlUtil.SetAttribute(nd, "Name", "!(loc.ApplicationName)");
			}
		}
		public override string ToString()
		{
			return "General";
		}
		[Browsable(false)]
		public string ProductId
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Id");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "Id", value);
			}
		}
		[Browsable(false)]
		public string LastProductId
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "LastId");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "LastId", value);
			}
		}
		[Description("The installer version must be in a format of 3 numbers separated by dots. For example, \"1.2.3\" (without quotes). The Version must be changed to be different than the version of previously generated installer so that the new installer can upgrade previously installed product.")]
		public string Version
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Version");
			}
			set
			{
				if (string.CompareOrdinal(this.Version, value) != 0)
				{
					XmlUtil.SetAttribute(XmlData, "Version", value);
					var lastVer = this.LastCompiledVersion;
					if (string.CompareOrdinal(value, lastVer) != 0)
					{
						if (VersionChanged != null)
						{
							VersionChanged(this, new EventArgsNameChange(lastVer, value));
						}
					}
				}
			}
		}
		[Description("Gets the version of the last time the installer was generated")]
		public string LastCompiledVersion
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "LastVersion");
			}
		}
		[Description("Gets and sets a folder name for parent folder of application folder")]
		public string CompanyFolderName
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "companyFolderName");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "companyFolderName", value);
			}
		}
		[Description("Gets and sets an image file to be displayed on top of dialogues.")]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		public string BannerImageFile
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "banner");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "banner", value);
			}
		}
		[Description("Gets and sets an image file to be displayed as background of dialogues.")]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		public string DialogImageFile
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "dialog");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "dialog", value);
			}
		}
		[Description("Gets and sets an icon file for the setup")]
		[FileSelectionAttribute("Icon files|*.ico","Select Setup Icon")]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		public string SetupIconFile
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "ARPPRODUCTICON");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "ARPPRODUCTICON", value);
			}
		}
	}
	#endregion
	#region WixShutcut
	public class WixShortcut : WixNode, IValueEnumProvider
	{
		public event EventHandler IdChanged;
		public WixShortcut(XmlNode node)
			: base(node)
		{
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			string f = Target;
			if (!string.IsNullOrEmpty(f))
			{
				if (f.StartsWith("PROJECT - ", StringComparison.OrdinalIgnoreCase))
				{
					f = f.Substring("PROJECT - ".Length).Trim();
					if (string.Compare(".lrproj", Path.GetExtension(f), StringComparison.OrdinalIgnoreCase) == 0)
					{
						LimnorProject prj = null;
						try
						{
							prj = new LimnorProject(f);
						}
						catch (Exception err)
						{
							throw new ProjectException("Shortcut target is a project [{0}]. Error loading the project file. Error message: {1}. Stack trace: {2}", f, err.Message, err.StackTrace);
						}
						string exe = null;
						try
						{
							exe = Path.Combine(Path.GetDirectoryName(f), string.Format(CultureInfo.InvariantCulture, "bin\\{0}\\{1}.exe", mode, prj.ProjectAssemblyName));
						}
						catch (Exception err)
						{
							throw new ProjectException("Shortcut target is a project [{0}]. Error finding the project assembly name. Error message: {1}. Stack trace: {2}", f, err.Message, err.StackTrace);
						}
						if (!File.Exists(exe))
						{
							throw new ProjectException("Shortcut target is a project [{0}] but the project has not been compiled to generate an EXE file:[{1}]", f, exe);
						}
						else
						{
							Target = exe;
						}
					}
					else
					{
						throw new ProjectException("Shortcut target is a project but it is not a *.lrproj file:[{0}]", f);
					}
				}
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Shortcut:{0}", Name);
		}

		[Description("Specify where this chortcut should be placed")]
		[Editor(typeof(TypeEditorValueEnum), typeof(UITypeEditor))]
		public string Location
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Directory");
			}
			set
			{
				object[] vs = GetValueEnum("Location");
				if (vs != null)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						string s = (string)vs[i];
						if (string.CompareOrdinal(s, value) == 0)
						{
							XmlUtil.SetAttribute(XmlData, "Directory", value);
							break;
						}
					}
				}
			}
		}
		[Editor(typeof(TypeEditorShortcutTarget), typeof(UITypeEditor))]
		[Description("Specify a program to run for this shortcut")]
		public string Target
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "file");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "file", value);
			}
		}
		public string Name
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Name");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string id0 = XmlUtil.GetAttribute(XmlData, "Name");
					if (string.Compare(value, id0, StringComparison.OrdinalIgnoreCase) != 0)
					{
						XmlUtil.SetAttribute(XmlData, "Name", value);
						if (IdChanged != null)
						{
							IdChanged(this, new EventArgsNameChange(id0, value));
						}
					}
				}
			}
		}
		[ReadOnly(true)]
		public string WorkingDirectory
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "WorkingDirectory");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "WorkingDirectory", value);
			}
		}
		[Description("Specify the icon for this shortcut")]
		[Editor(typeof(TypeEditorValueEnum),typeof(UITypeEditor))]
		public string Icon
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Icon");
			}
			set
			{
				object[] vs = GetValueEnum("Icon");
				if (vs != null)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						string s = (string)vs[i];
						if (string.CompareOrdinal(s, value) == 0)
						{
							XmlUtil.SetAttribute(XmlData, "Icon", value);
							break;
						}
					}
				}
			}
		}
		#region IValueEnumProvider
		public object[] GetValueEnum(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Icon") == 0)
			{
				List<object> lst = new List<object>();
				XmlNodeList ns = this.XmlData.ParentNode.ParentNode.SelectNodes("Icon");
				foreach (XmlNode nd in ns)
				{
					lst.Add(XmlUtil.GetAttribute(nd, "Id"));
				}
				return lst.ToArray();
			}
			else if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				object[] vs = new object[2];
				vs[0] = "DesktopFolder";
				vs[1] = "ProgramMenuDir";
				return vs;
			}
			return new object[] { };
		}

		public void SetValueEnum(string propertyName, object[] values)
		{

		}
		#endregion
	}
	#endregion
	#region WixShortcutCollection
	public class WixShortcutCollection : WixNode
	{
		private List<WixShortcut> _shortcuts;
		public WixShortcutCollection(XmlNode node)
			: base(node)
		{
		}
		public void OnIconIdChanged(string oldIconId, string newIconId)
		{
			foreach (WixShortcut st in Shortcuts)
			{
				if (string.CompareOrdinal(st.Icon, oldIconId) == 0)
				{
					st.Icon = newIconId;
				}
			}
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			if (Shortcuts.Count > 0)
			{
				foreach (WixShortcut st in _shortcuts)
				{
					st.Preprocess(setupName, mode, log);
				}
			}
		}
		public override string ToString()
		{
			return "Shortcuts";
		}
		[Browsable(false)]
		public IList<WixShortcut> Shortcuts
		{
			get
			{
				if (_shortcuts == null)
				{
					XmlNodeList ns = XmlData.SelectNodes("Item");
					_shortcuts = new List<WixShortcut>();
					foreach (XmlNode nd in ns)
					{
						_shortcuts.Add(new WixShortcut(nd));
					}
				}
				return _shortcuts;
			}
		}
		public WixShortcut AddShortcut()
		{
			int n = 1;
			string name = string.Format(CultureInfo.InvariantCulture, "My App {0}", n);
			IList<WixShortcut> ss = Shortcuts;
			if (ss.Count > 0)
			{
				bool b = true;
				while (b)
				{
					b = false;
					foreach (WixShortcut s in ss)
					{
						if (string.Compare(s.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							n++;
							name = string.Format(CultureInfo.InvariantCulture, "My App {0}", n);
							b = true;
							break;
						}
					}
				}
			}
			XmlNode nd = XmlData.OwnerDocument.CreateElement("Item");
			XmlData.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "Name", name);
			XmlUtil.SetAttribute(nd, "WorkingDirectory", "INSTALLDIR");
			WixShortcut win = new WixShortcut(nd);
			_shortcuts.Add(win);
			return win;
		}
	}
	#endregion
	#region WixIconNode
	public class WixIconNode : WixNode
	{
		public event EventHandler FileChanged;
		public event EventHandler IdChanged;
		public WixIconNode(XmlNode node)
			: base(node)
		{
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Icon:{0}", Id);
		}
		public string Id
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Id");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string id0 = XmlUtil.GetAttribute(XmlData, "Id");
					if (string.Compare(value, id0, StringComparison.OrdinalIgnoreCase) != 0)
					{
						XmlNodeList ns = this.XmlData.ParentNode.SelectNodes("Icon");
						foreach (XmlNode nd in ns)
						{
							string id = XmlUtil.GetAttribute(nd, "Id");
							if (string.Compare(id, value, StringComparison.OrdinalIgnoreCase) == 0)
							{
								throw new Exception("The name is in use");
							}
						}
						XmlUtil.SetAttribute(XmlData, "Id", value);
						if (IdChanged != null)
						{
							IdChanged(this, new EventArgsNameChange(id0, value));
						}
					}
				}
			}
		}
		[Description("Gets and sets an icon file to be used in the setup")]
		[FileSelectionAttribute("Icon files|*.ico", "Select Setup Icon")]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		public string SourceFile
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "SourceFile");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "SourceFile", value);
				if (FileChanged != null)
				{
					FileChanged(this, EventArgs.Empty);
				}
			}
		}
	}
	#endregion
	#region WixIconCollection
	public class WixIconCollection : WixNode
	{
		private List<WixIconNode> _icons;
		public WixIconCollection(XmlNode node)
			: base(node)
		{
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		public override string ToString()
		{
			return "Icons";
		}
		[Browsable(false)]
		public IList<WixIconNode> Icons
		{
			get
			{
				if (_icons == null)
				{
					XmlNodeList ns = XmlData.SelectNodes("Icon");
					_icons = new List<WixIconNode>();
					foreach (XmlNode nd in ns)
					{
						_icons.Add(new WixIconNode(nd));
					}
				}
				return _icons;
			}
		}
		public WixIconNode AddIcon()
		{
			int n = 1;
			string id = string.Format(CultureInfo.InvariantCulture, "icon{0}.ico", n);
			IList<WixIconNode> icons = Icons;
			if (icons.Count > 0)
			{
				bool b = true;
				while (b)
				{
					b = false;
					foreach (WixIconNode ico in icons)
					{
						if (string.Compare(ico.Id, id, StringComparison.OrdinalIgnoreCase) == 0)
						{
							n++;
							id = string.Format(CultureInfo.InvariantCulture, "icon{0}.ico", n);
							b = true;
							break;
						}
					}
				}
			}
			XmlNode nd = XmlData.OwnerDocument.CreateElement("Icon");
			XmlData.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "Id", id);
			WixIconNode win = new WixIconNode(nd);
			_icons.Add(win);
			return win;
		}
	}
	#endregion
	#region WixFileNode
	public class WixSourceFileNode : WixNode, IDistributeFile
	{
		#region fields and constructors
		const string XMLATTR_removed = "removed";
		private IWixFolder _folder;
		public WixSourceFileNode(XmlNode node, IWixFolder folder)
			: base(node)
		{
			_folder = folder;
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			if (IsFolder)
			{
				return string.Format(CultureInfo.InvariantCulture, "FILES - {0}", Path.Combine(Filename, "*.*"));
			}
			else if (IsProject)
			{
				return string.Format(CultureInfo.InvariantCulture, "PROJECT - {0}", Filename);
			}
			return string.Format(CultureInfo.InvariantCulture, "FILE - {0}", Filename);
		}
		public override void OnBeforeSave()
		{
			throw new NotImplementedException();
		}
		#endregion
		#region Properties
		public string Filename
		{
			get
			{
				return XmlData.InnerText;
			}
		}
		public bool IsFolder
		{
			get
			{
				return string.CompareOrdinal(XmlData.Name, "SourceFolder") == 0;
			}
		}
		public bool IsProject
		{
			get
			{
				return string.CompareOrdinal(XmlData.Name, "Project") == 0;
			}
		}
		[Browsable(false)]
		public bool IsRemoved
		{
			get
			{
				return XmlUtil.GetAttributeBoolDefFalse(this.XmlData, XMLATTR_removed);
			}
			set
			{
				XmlUtil.SetAttribute(this.XmlData, XMLATTR_removed, value);
			}
		}
		#endregion
		#region IDistributeFile
		[Browsable(false)]
		public string FileName
		{
			get
			{
				return this.Filename;
			}
			set
			{
				if (IsFolder)
				{
					XmlUtil.SetAttribute(XmlData, "folder", value);
				}
				else
				{
					XmlData.InnerText = value;
				}
			}
		}

		public string GetTargetPathWithToken()
		{
			if (_folder != null)
			{
				return Path.Combine(_folder.FullPathWithToken, Path.GetFileName(this.Filename));
			}
			return Path.Combine(Filepath.FOLD_APPLICATION, Path.GetFileName(this.Filename));
		}
		#endregion
	}
	#endregion
	#region WixFolderNode
	public enum EnumFolderType { Data, Application, System }
	public class WixFolderNode : WixNode, IWixFolder
	{
		private IWixFolder _parent;
		private List<WixSourceFileNode> _files;
		private List<WixFolderNode> _subFolders;
		public WixFolderNode(XmlNode node, IWixFolder parent)
			: base(node)
		{
			_parent = parent;
		}
		public override void OnBeforeSave()
		{
			RemoveUnusedFiles();
		}
		public void CollectAddedFiles(List<WixSourceFileNode> list)
		{
			if (Files.Count > 0)
			{
				list.AddRange(_files);
			}
			if (Folders.Count > 0)
			{
				foreach (WixFolderNode o in _subFolders)
				{
					o.CollectAddedFiles(list);
				}
			}
		}
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			//replace Project nodes with SourceFolder node
			XmlNodeList prjs = this.XmlData.SelectNodes(WixNode.XML_PROJECT);
			List<XmlNode> usedNodes = new List<XmlNode>();
			foreach (XmlNode nd in prjs)
			{
				if (!XmlUtil.GetAttributeBoolDefFalse(nd, "removed"))
				{
					string prjFile = nd.InnerText;
					XmlNode nd2 = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFolder);
					XmlUtil.SetAttribute(nd2, "folder", Path.Combine(Path.GetDirectoryName(prjFile), string.Format(CultureInfo.InvariantCulture, "bin\\{0}", mode)));
					XmlData.AppendChild(nd2);
					XmlNode exl = XmlData.OwnerDocument.CreateElement("ExcludeFile");
					nd2.AppendChild(exl);
					XmlUtil.SetAttribute(exl, "ext", ".pdb");
				}
				usedNodes.Add(nd);
			}
			foreach (XmlNode nd in usedNodes)
			{
				XmlData.RemoveChild(nd);
			}
			if (Folders.Count > 0)
			{
				foreach (WixFolderNode f in Folders)
				{
					f.Preprocess(setupName, mode, log);
				}
			}
		}
		public string FullPathWithToken
		{
			get
			{
				if (_parent == null)
				{
					IWixTopFolder top = this as IWixTopFolder;
					if (top != null)
					{
						return top.TopFolderToken;
					}
					else
					{
						return string.Empty;
					}
				}
				else
				{
					string s = _parent.FullPathWithToken;
					if (string.IsNullOrEmpty(s))
					{
						return FolderName;
					}
					else
					{
						return Path.Combine(s, FolderName);
					}
				}
			}
		}
		public virtual EnumFolderType FolderType
		{
			get
			{
				return EnumFolderType.Data;
			}
		}
		public IWixFolder Parent
		{
			get
			{
				return _parent;
			}
		}
		private void loadSubFolders()
		{
			if (_subFolders == null)
			{
				_subFolders = new List<WixFolderNode>();
				XmlNodeList ns = XmlData.SelectNodes("Folder");
				foreach (XmlNode nd in ns)
				{
					WixFolderNode fn = new WixFolderNode(nd, this);
					_subFolders.Add(fn);
				}
			}
		}
		public IList<WixFolderNode> Folders
		{
			get
			{
				loadSubFolders();
				
				return _subFolders;
			}
		}
		public bool FileExists(string file)
		{
			IList<WixSourceFileNode> fs = Files;
			foreach (WixSourceFileNode f in fs)
			{
				if (string.Compare(f.Filename, file, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public WixSourceFileNode GetWixNodeByName(string filename)
		{
			IList<WixSourceFileNode> fs = Files;
			foreach (WixSourceFileNode f in fs)
			{
				if (string.Compare(f.Filename, filename, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return f;
				}
			}
			return null;
		}
		protected virtual void GetFiles(List<WixSourceFileNode> files)
		{
			XmlNodeList ns = XmlData.SelectNodes("SourceFolder");
			foreach (XmlNode nd in ns)
			{
				WixSourceFileNode fn = new WixSourceFileNode(nd, this);
				files.Add(fn);
			}
			ns = XmlData.SelectNodes("SourceFile");
			foreach (XmlNode nd in ns)
			{
				WixSourceFileNode fn = new WixSourceFileNode(nd, this);
				files.Add(fn);
			}
		}
		public IList<WixSourceFileNode> Files
		{
			get
			{
				if (_files == null)
				{
					_files = new List<WixSourceFileNode>();
					GetFiles(_files);
				}
				return _files;
			}
		}
		public string FolderName
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "folder");
			}
			set
			{
				XmlUtil.SetAttribute(XmlData, "folder", value);
			}
		}
		public WixSourceFileNode AddFile(string file)
		{
			XmlNode fn;
			IList<WixSourceFileNode> curFiles = Files;
			foreach(WixSourceFileNode f in curFiles)
			{
				if (string.Compare(file, f.Filename, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return f;
				}
			}
			if (Directory.Exists(file))
			{
				fn = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFolder);
			}
			else
			{
				fn = XmlData.OwnerDocument.CreateElement(WixNode.XML_SourceFile);
			}
			XmlData.AppendChild(fn);
			fn.InnerText = file;
			if (_files == null)
			{
				_files = new List<WixSourceFileNode>();
				GetFiles(_files);
			}
			WixSourceFileNode wfn = new WixSourceFileNode(fn, this);
			_files.Add(wfn);
			return wfn;
		}
		public WixFolderNode AddFolder(string dir)
		{
			XmlNode dn = XmlData.OwnerDocument.CreateElement("Folder");
			XmlData.AppendChild(dn);
			XmlUtil.SetAttribute(dn, "folder", dir);
			loadSubFolders();
			WixFolderNode wdn = new WixFolderNode(dn, this);
			_subFolders.Add(wdn);
			return wdn;
		}
		public void ResetSubFolders()
		{
			_subFolders = null;
		}
		public void ResetSourceFiles()
		{
			_files = null;
		}
		public IList<WixSourceFileNode> RemoveUnusedFiles()
		{
			List<WixSourceFileNode> unused = new List<WixSourceFileNode>();
			if (_files != null && _files.Count > 0)
			{
				foreach (WixSourceFileNode f in _files)
				{
					if (f.IsRemoved)
					{
						if (!f.IsProject)
						{
							unused.Add(f);
						}
					}
				}
			}
			if (unused.Count > 0)
			{
				foreach (WixSourceFileNode f in unused)
				{
					_files.Remove(f);
					if (f.XmlData.ParentNode == XmlData)
					{
						XmlData.RemoveChild(f.XmlData);
					}
				}
			}
			return unused;
		}
		public bool ValidateProjectNames()
		{
			bool bChanged = false;
			if (this.FolderType == EnumFolderType.System || this.FolderType == EnumFolderType.Application)
			{
				bool forScreenSaver = (this.FolderType == EnumFolderType.System);
				StringCollection names;
				if (LimnorSolution.Solution != null)
				{
					names = new StringCollection();
					for (int i = 0; i < LimnorSolution.Solution.Count; i++)
					{
						if (LimnorSolution.Solution[i].ProjectType != EnumProjectType.Setup)
						{
							names.Add(LimnorSolution.Solution[i].ProjectFile);
						}
					}
				}
				else
				{
					names = null;
				}
				List<WixSourceFileNode> invalds = new List<WixSourceFileNode>();
				List<WixSourceFileNode> prjs = new List<WixSourceFileNode>();
				IList<WixSourceFileNode> files = Files;
				foreach (WixSourceFileNode mp in files)
				{
					if (mp.IsProject)
					{
						bool bInSolution = false;
						if (names != null && names.Count > 0)
						{
							foreach (string s in names)
							{
								if (string.Compare(s, mp.Filename, StringComparison.OrdinalIgnoreCase) == 0)
								{
									bInSolution = true;
									break;
								}
							}
						}
						if (!bInSolution && mp.IsRemoved)
						{
							invalds.Add(mp);
						}
					}
				}
				if (invalds.Count > 0)
				{
					bChanged = true;
					foreach (WixSourceFileNode f in invalds)
					{
						_files.Remove(f);
						if (f.XmlData.ParentNode == XmlData)
						{
							XmlData.RemoveChild(f.XmlData);
						}
					}
				}
				if (names != null && names.Count > 0)
				{
					foreach (string s in names)
					{
						bool b = false;
						foreach (WixSourceFileNode mp in _files)
						{
							if (string.Compare(mp.Filename, s, StringComparison.OrdinalIgnoreCase) == 0)
							{
								b = true;
								break;
							}
						}
						if (!b)
						{
							LimnorProject prj = new LimnorProject(s);
							if (forScreenSaver)
							{
								b = (prj.ProjectType == EnumProjectType.ScreenSaver);
							}
							else
							{
								b = (prj.ProjectType != EnumProjectType.ScreenSaver);
							}
							if (b)
							{
								AddProject(s);
								bChanged = true;
							}
						}
					}
				}
			}
			return bChanged;
		}
		public WixSourceFileNode AddProject(string projectFile)
		{
			IList<WixSourceFileNode> curFiles = Files;
			foreach (WixSourceFileNode f in curFiles)
			{
				if (string.Compare(projectFile, f.Filename, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return f;
				}
			}
			XmlNode nd = XmlData.OwnerDocument.CreateElement(WixNode.XML_PROJECT);
			nd.InnerText = projectFile;
			XmlData.AppendChild(nd);
			WixSourceFileNode p = new WixSourceFileNode(nd, this);
			Files.Add(p);
			return p;
		}
		public void OnSelected()
		{
			ValidateProjectNames();
		}
		public override string ToString()
		{
			return XmlUtil.GetAttribute(XmlData, "folder");
		}
	}
	#endregion
	#region WixSystemFolder
	public class WixSystemFolder : WixFolderNode, IWixTopFolder
	{
		public WixSystemFolder(XmlNode node)
			: base(node, null)
		{
		}
		public override string ToString()
		{
			return "System Folder";
		}
		public string TopFolderToken
		{
			get { return Filepath.FOLD_SYSTEM; }
		}
		public override EnumFolderType FolderType
		{
			get
			{
				return EnumFolderType.System;
			}
		}
	}
	#endregion
	#region WixCommonAppDataFolder
	public class WixCommonAppDataFolder : WixFolderNode, IWixTopFolder
	{
		public WixCommonAppDataFolder(XmlNode node)
			: base(node, null)
		{
		}
		public override string ToString()
		{
			return "Common App Data Folder";
		}
		public string TopFolderToken
		{
			get { return Filepath.FOLD_CPMMPMAPPDATA; }
		}
	}
	#endregion
	#region WixAppFolder
	public class WixAppFolder : WixFolderNode, IWixTopFolder
	{
		public WixAppFolder(XmlNode node)
			: base(node, null)
		{
		}
		public virtual string TopFolderToken
		{
			get
			{
				return Filepath.FOLD_APPLICATION;
			}
		}
		public override string ToString()
		{
			return "Application Folder";
		}
		public override void Preprocess(string setupName, string mode, IShowMessage log)
		{
			XmlUtil.SetAttribute(XmlData, "Name", setupName);
			XmlUtil.SetAttribute(XmlData, "mainId", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
			base.Preprocess(setupName, mode, log);
		}
		public override EnumFolderType FolderType
		{
			get
			{
				return EnumFolderType.Application;
			}
		}
		public StringCollection ProjectFiles
		{
			get
			{
				XmlNodeList ns = XmlData.SelectNodes("Project");
				StringCollection sc = new StringCollection();
				foreach (XmlNode n in ns)
				{
					sc.Add(n.InnerText);
				}
				return sc;
			}
			set
			{
				int n = (value == null ? 0 : value.Count);
				XmlNodeList ns = XmlData.SelectNodes("Project");
				for (int i = 0; i < ns.Count && i < n; i++)
				{
					ns[i].InnerText = value[i];
				}
				for (int i = ns.Count; i < n; i++)
				{
					XmlNode nd = XmlData.OwnerDocument.CreateElement("Project");
					nd.InnerText = value[i];
					XmlData.AppendChild(nd);
				}
				int n2 = ns.Count;
				for (int i = n; i < n2; i++)
				{
					XmlData.RemoveChild(ns[i]);
				}
			}
		}
		protected override void GetFiles(List<WixSourceFileNode> files)
		{
			XmlNodeList ns = XmlData.SelectNodes("Project");
			foreach(XmlNode nd in ns)
			{
				files.Add(new WixSourceFileNode(nd, this));
			}
			base.GetFiles(files);
		}
	}
	#endregion
	#region WixCulture
	public class WixCulture : IValueWithDescEnumProvider
	{
		private XmlDocument _doc;
		private XmlNamespaceManager _xm;
		private WixCultureNode _owner;
		private bool _changed;
		public WixCulture(WixCultureNode owner)
		{
			_owner = owner;
			_doc = new XmlDocument();
			_doc.Load(owner.LanguageFile);
			_xm = new XmlNamespaceManager(_doc.NameTable);
			_xm.AddNamespace("x", WixNode.WixUrl_Localize);
		}
		public void Save()
		{
			if (_changed)
			{
				_doc.Save(_owner.LanguageFile);
				_changed = false;
			}
		}
		[Browsable(false)]
		public bool Changed
		{
			get
			{
				return _changed;
			}
		}
		[Browsable(false)]
		public XmlNode XmlData
		{
			get
			{
				return _doc.DocumentElement;
			}
		}
		[Description("Culture for generate an MSI file")]
		public string Culture
		{
			get
			{
				return XmlUtil.GetAttribute(XmlData, "Culture");
			}
		}
		[Description("Product name displayed in the installer user interface")]
		public string ApplicationName
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='ApplicationName']", _xm);
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='ApplicationName']", _xm);
				if (nd == null)
				{
					nd = _doc.CreateElement("ApplicationName", WixNode.WixUrl_Localize);
					XmlData.AppendChild(nd);
				}
				nd.InnerText = value;
				_changed = true;
			}
		}
		[Description("Manufacturer name for the MSI file")]
		public string MsiFileInfoManufacturer
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='ManufacturerName']", _xm);
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='ManufacturerName']", _xm);
				if (nd == null)
				{
					nd = _doc.CreateElement("ManufacturerName", WixNode.WixUrl_Localize);
					XmlData.AppendChild(nd);
				}
				nd.InnerText = value;
				_changed = true;
			}
		}
		[Description("Subject for the MSI file")]
		public string MsiFileInfoSubject
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='SetupDesc']", _xm);
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='SetupDesc']", _xm);
				if (nd == null)
				{
					nd = _doc.CreateElement("SetupDesc", WixNode.WixUrl_Localize);
					XmlData.AppendChild(nd);
				}
				nd.InnerText = value;
				_changed = true;
			}
		}
		[Description("Description for the MSI file")]
		public string MsiFileInfoDescription
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='SetupComments']", _xm);
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='SetupComments']", _xm);
				if (nd == null)
				{
					nd = _doc.CreateElement("SetupComments", WixNode.WixUrl_Localize);
					XmlData.AppendChild(nd);
				}
				nd.InnerText = value;
				_changed = true;
			}
		}
		[Editor(typeof(TypeSelectorText),typeof(UITypeEditor))]
		[Description("The welcome message to be displayed when the installer is launched.")]
		public string WelcomeMessage
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='WelcomeDlgDescription']", _xm);
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlData.SelectSingleNode("x:String[@Id='WelcomeDlgDescription']", _xm);
				if (nd == null)
				{
					nd = _doc.CreateElement("WelcomeDlgDescription", WixNode.WixUrl_Localize);
					XmlData.AppendChild(nd);
				}
				nd.InnerText = value;
				_changed = true;
			}
		}
		[Description("Gets and sets a license file in Rich Formatted Text (RTF) format. The license file will be displayed during setup.")]
		[FileSelectionAttribute("RTF files|*.rtf", "Select License File")]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		public string SetupLicenseFile
		{
			get
			{
				return _owner.LicenseFile;
			}
			set
			{
				_owner.LicenseFile = value;
				_changed = true;
			}
		}
		[Editor(typeof(TypeEditorValueWithDescEnum), typeof(UITypeEditor))]
		[Description("Code page for displaying msi file summary information. See https://msdn.microsoft.com/en-us/library/windows/desktop/dd317756(v=vs.85).aspx")]
		public string MsiFileInfoCodePage
		{
			get
			{
				return XmlUtil.GetAttribute(_owner.XmlData, "SummaryCodepage");
			}
			set
			{
				XmlUtil.SetAttribute(_owner.XmlData, "SummaryCodepage", value);
			}
		}
		[Description("A text file for recording the modifications for this culture")]
		public string LanguageFile
		{
			get
			{
				return _owner.LanguageFile;
			}
		}
		#region IValueWithDescEnumProvider
		public ValueWithDesc[] GetValueWithDescEnum(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "MsiFileInfoCodePage") == 0)
			{
				ValueWithDesc[] vs = new ValueWithDesc[14];
				vs[0] = new ValueWithDesc("874", "Thai (ISO 8859-11)");
				vs[1] = new ValueWithDesc("932","Japanese (Shift-JIS)");
				vs[2] = new ValueWithDesc("936", "Simplified Chinese (PRC, Singapore, GB2312)");
				vs[3] = new ValueWithDesc("949", "Korean (Unified Hangul Code)");
				vs[4] = new ValueWithDesc("950", "Traditional Chinese (Taiwan; Hong Kong SAR, PRC, Big5)");
				vs[5] = new ValueWithDesc("1250", "Central European (Windows)");
				vs[6] = new ValueWithDesc("1251", "Cyrillic (Windows)");
				vs[7] = new ValueWithDesc("1252", "Western European (Windows)");
				vs[8] = new ValueWithDesc("1253", "Greek (Windows)");
				vs[9] = new ValueWithDesc("1254", "Turkish (Windows)");
				vs[10] = new ValueWithDesc("1255", "Hebrew (Windows)");
				vs[11] = new ValueWithDesc("1256", "Arabic (Windows)");
				vs[12] = new ValueWithDesc("1257", "Baltic (Windows)");
				vs[13] = new ValueWithDesc("1258", "Vietnamese (Windows)");
				return vs;
			}
			return new ValueWithDesc[] { };
		}

		#endregion
	}
	#endregion
	#region WixCultureNode
	public class WixCultureNode : WixNode
	{
		private WixCulture _cultureNode;
		public WixCultureNode(XmlNode node)
			: base(node)
		{
		}
		public override void OnBeforeSave()
		{
			if (_cultureNode != null)
			{
				_cultureNode.Save();
			}
		}
		public override string ToString()
		{
			return XmlUtil.GetAttribute(XmlData,"name");
		}
		public string LanguageFile
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("LanguageFile");
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
		}
		public string LicenseFile
		{
			get
			{
				XmlNode nd = XmlData.SelectSingleNode("LicFile");
				if (nd != null)
				{
					return nd.InnerText;
				}
				return string.Empty;
			}
			set
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(XmlData, "LicFile");
				nd.InnerText = value;
			}
		}
		
		public WixCulture CultureNode
		{
			get
			{
				if (_cultureNode == null)
				{
					_cultureNode = new WixCulture(this);
				}
				return _cultureNode;
			}
		}
	}
	#endregion
	#region WixCultureCollection
	public class WixCultureCollection : WixNode
	{
		private List<WixCultureNode> _cultures;
		public WixCultureCollection(XmlNode node)
			: base(node)
		{
		}
		public override string ToString()
		{
			return "Cultures";
		}
		public override void OnBeforeSave()
		{
			if (_cultures != null)
			{
				foreach (WixCultureNode c in _cultures)
				{
					c.OnBeforeSave();
				}
			}
		}
		private void getCultures()
		{
			if (_cultures == null)
			{
				_cultures = new List<WixCultureNode>();
				XmlNodeList ns = XmlData.SelectNodes("Item");
				foreach (XmlNode nd in ns)
				{
					_cultures.Add(new WixCultureNode(nd));
				}
			}
		}
		[Browsable(false)]
		public IList<WixCultureNode> CultureList
		{
			get
			{
				getCultures();
				return _cultures;
			}
		}
		public bool CultureExists(string name)
		{
			getCultures();
			foreach (WixCultureNode wcn in _cultures)
			{
				if (string.Compare(name, wcn.CultureNode.Culture, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
	#endregion
}
