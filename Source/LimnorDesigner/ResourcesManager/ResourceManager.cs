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
using System.CodeDom;
using MathExp;
using ProgElements;
using XmlSerializer;
using System.Xml;
using System.Collections;
using XmlUtility;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Resources;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using WindowsUtility;
using VPL;
using Limnor.WebBuilder;
using System.Drawing.Design;
using LimnorDesigner.Action;

namespace LimnorDesigner.ResourcesManager
{
	[NotForProgramming]
	[WebClientClass]
	[ToolboxBitmapAttribute(typeof(ProjectResources), "_res.bmp")]
	public class ProjectResources : IObjectPointer, IProjectAccessor
	{
		#region fields and constructors
		internal const string XML_STRINGS = "Strings";
		internal const string XML_IMAGES = "Images";
		internal const string XML_ICONS = "Icons";
		internal const string XML_AUDIOS = "Audios";
		internal const string XML_FILES = "Files";
		internal const string XML_FILENAMES = "Filenames";
		private LimnorProject _prj;
		private XmlNode _xmlNode;
		private List<ResourcePointerString> _stringList;
		private List<ResourcePointerImage> _imageList;
		private List<ResourcePointerIcon> _iconList;
		private List<ResourcePointerAudio> _audioList;
		private List<ResourcePointerFile> _fileList;
		private List<ResourcePointerFilePath> _filenameList;
		private List<string> _languageNames;

		private string _designerLanguageName;
		public ProjectResources(LimnorProject project)
		{
			_prj = project;
		}
		#endregion

		#region Static Methods
		[Browsable(false)]
		public static void CollectLanguageIcons(ImageList _imageList)
		{
			_imageList.Images.Add("is", ResourceLanguage1._is);
			_imageList.Images.Add("ar", ResourceLanguage1.ar);
			_imageList.Images.Add("bg", ResourceLanguage1.bg);
			_imageList.Images.Add("ca", ResourceLanguage1.ca);
			_imageList.Images.Add("cs", ResourceLanguage1.cs);
			_imageList.Images.Add("da", ResourceLanguage1.da);
			_imageList.Images.Add("de", ResourceLanguage1.de);
			_imageList.Images.Add("el", ResourceLanguage1.el);
			_imageList.Images.Add("en", ResourceLanguage1.en);
			_imageList.Images.Add("eo", ResourceLanguage1.eo);
			_imageList.Images.Add("es", ResourceLanguage1.es);
			_imageList.Images.Add("et", ResourceLanguage1.et);
			_imageList.Images.Add("eu", ResourceLanguage1.eu);
			_imageList.Images.Add("fa", ResourceLanguage1.fa);
			_imageList.Images.Add("fi", ResourceLanguage1.fi);
			_imageList.Images.Add("fo", ResourceLanguage1.fo);
			_imageList.Images.Add("fr", ResourceLanguage1.fr);
			_imageList.Images.Add("ga", ResourceLanguage1.ga);
			_imageList.Images.Add("gl", ResourceLanguage1.gl);
			_imageList.Images.Add("he", ResourceLanguage1.he);
			_imageList.Images.Add("hr", ResourceLanguage1.hr);
			_imageList.Images.Add("hu", ResourceLanguage1.hu);
			_imageList.Images.Add("id", ResourceLanguage1.id);
			_imageList.Images.Add("it", ResourceLanguage1.it);
			_imageList.Images.Add("ja", ResourceLanguage1.ja);
			_imageList.Images.Add("km", ResourceLanguage1.km);
			_imageList.Images.Add("lb", ResourceLanguage1.lb);
			_imageList.Images.Add("lt", ResourceLanguage1.lt);
			_imageList.Images.Add("lv", ResourceLanguage1.lv);
			_imageList.Images.Add("nb", ResourceLanguage1.nb);
			_imageList.Images.Add("nl", ResourceLanguage1.nl);
			_imageList.Images.Add("nn", ResourceLanguage1.nn);

			_imageList.Images.Add("pl", ResourceLanguage1.pl);
			_imageList.Images.Add("pt-BR", ResourceLanguage1.pt_br);
			_imageList.Images.Add("pt-PT", ResourceLanguage1.pt_pt);
			_imageList.Images.Add("ro", ResourceLanguage1.ro);
			_imageList.Images.Add("ru", ResourceLanguage1.ru);
			_imageList.Images.Add("sk", ResourceLanguage1.sk);
			_imageList.Images.Add("sl", ResourceLanguage1.sl);
			_imageList.Images.Add("sv", ResourceLanguage1.sv);
			_imageList.Images.Add("tg", ResourceLanguage1.tg);
			_imageList.Images.Add("th", ResourceLanguage1.th);
			_imageList.Images.Add("tl", ResourceLanguage1.tl);
			_imageList.Images.Add("tr", ResourceLanguage1.tr);
			_imageList.Images.Add("uk", ResourceLanguage1.uk);
			_imageList.Images.Add("vi", ResourceLanguage1.vi);
			_imageList.Images.Add("zh", ResourceLanguage1.zh);
			_imageList.Images.Add("zh-CHS", ResourceLanguage1.zh_CHS);
			_imageList.Images.Add("zh-CHT", ResourceLanguage1.zh_CHT);
			_imageList.Images.Add("zh-TW", ResourceLanguage1.zh_TW);
			_imageList.Images.Add("zh-CN", ResourceLanguage1.zh_CN);
			_imageList.Images.Add("zh-HK", ResourceLanguage1.zh_HK);
			_imageList.Images.Add("zh-MO", ResourceLanguage1.zh_MO);
			_imageList.Images.Add("zh-SG", ResourceLanguage1.zh_SG);

		}
		#endregion

		#region VPL Interface
		[Description("The culture for getting the resources. This is changed by changing ProjectCultureName.")]
		public static CultureInfo ProjectCulture
		{
			get
			{
				return CultureInfo.CurrentUICulture;
			}
		}

		[Editor(typeof(TypeSelectorLanguage), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("The name of the culture for getting the resources")]
		public static string ProjectCultureName { get; set; }

		[Description("Occurs when the culture for the project is changed by setting property ProjectCultureName")]
		public static event EventHandler ProjectCultureChanged
		{
			add { }
			remove { }
		}
		#endregion

		#region Properties

		public string ResourcesFolder
		{
			get
			{
				return System.IO.Path.Combine(_prj.ProjectFolder, "Resources");
			}
		}
		public LimnorProject Project
		{
			get
			{
				return _prj;
			}
		}
		public XmlNode RootNode
		{
			get
			{
				if (_xmlNode == null)
				{
					_xmlNode = _prj.ResourcesXmlNode;
				}
				return _xmlNode;
			}
		}
		public IList<string> Languages
		{
			get
			{
				if (_languageNames == null)
				{
					IList<string> ss = XmlUtil.GetLanguages(RootNode);
					_languageNames = new List<string>();
					foreach (string s in ss)
					{
						_languageNames.Add(s);
					}
				}
				return _languageNames;
			}
		}
		public string DesignerLanguageName
		{
			get
			{
				return _designerLanguageName;
			}
			set
			{
				_designerLanguageName = value;
			}
		}
		public CultureInfo DesignerCulture
		{
			get
			{
				if (string.IsNullOrEmpty(_designerLanguageName))
				{
					return CultureInfo.CurrentCulture;
				}
				if (string.CompareOrdinal(_designerLanguageName, "zh") == 0)
				{
					return CultureInfo.GetCultureInfo("zh-CHT");
				}
				CultureInfo c = CultureInfo.GetCultureInfo(_designerLanguageName);
				if (c == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return c;
			}
		}
		public string DesingerCultureDisplay
		{
			get
			{
				if (string.CompareOrdinal(_designerLanguageName, "zh") == 0)
				{
					return "中文 zh";
				}
				return CultureDisplay(DesignerCulture);
			}
		}
		public static string CultureDisplay(CultureInfo c)
		{
			if (c == null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0} [{1}]", System.Globalization.CultureInfo.CurrentCulture.NativeName,
					System.Globalization.CultureInfo.CurrentCulture.Name);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0} [{1}]", c.NativeName,
					c.Name);
		}
		public void AddLanguageSelectionMenu(List<MenuItem> l)
		{
			if (Languages.Count > 0)
			{
				l.Add(new MenuItem("-"));
				MenuItemWithBitmap m = new MenuItemWithBitmap("default culture", mnu_changeCulture_click, TreeViewObjectExplorer.GetLangaugeBitmapByName(string.Empty));
				m.Tag = "";
				l.Add(m);
				foreach (string s in Languages)
				{
					if (string.CompareOrdinal(s, "zh") == 0)
					{
						m = new MenuItemWithBitmap("中文 zh", mnu_changeCulture_click, TreeViewObjectExplorer.GetLangaugeBitmapByName(s));
						m.Tag = s;
						l.Add(m);
					}
					else
					{
						CultureInfo c = CultureInfo.GetCultureInfo(s);
						if (c != null)
						{
							m = new MenuItemWithBitmap(ProjectResources.CultureDisplay(c), mnu_changeCulture_click, TreeViewObjectExplorer.GetLangaugeBitmapByName(s));
							m.Tag = s;
							l.Add(m);
						}
					}
				}
			}
		}
		private void mnu_changeCulture_click(object sender, EventArgs e)
		{
			string curLanguageName = DesignerLanguageName;
			MenuItem m = (MenuItem)sender;
			DesignerLanguageName = (string)(m.Tag);
			if (string.CompareOrdinal(curLanguageName, DesignerLanguageName) != 0)
			{
				NotifyLanguageChange();
			}
		}
		#endregion

		#region Compile
		public bool HasResources
		{
			get
			{
				if (this.ResourceStringList.Count > 0)
				{
					return true;
				}
				if (this.ResourceImageList.Count > 0)
				{
					return true;
				}
				if (this.ResourceIconList.Count > 0)
				{
					return true;
				}
				if (this.ResourceAudioList.Count > 0)
				{
					return true;
				}
				if (this.ResourceFileList.Count > 0)
				{
					return true;
				}
				if (this.ResourceFilenameList.Count > 0)
				{
					return true;
				}
				return false;
			}
		}
		public bool WriteResources(IResourceWriter resWriter, string languageName, List<IDistributeFile> distributeFiles)
		{
			bool ret = false;
			IList<ResourcePointerString> strings = ResourceStringList;
			if (strings.Count > 0)
			{
				foreach (ResourcePointerString rs in strings)
				{
					if (rs.HasResource(languageName))//"" always returns true
					{
						string s = rs.GetResourceString(languageName);
						if (string.IsNullOrEmpty(s))
						{
							if (string.IsNullOrEmpty(languageName))
							{
								resWriter.AddResource(rs.CodeName, s);
							}
						}
						else
						{
							resWriter.AddResource(rs.CodeName, s);
						}
					}
				}
			}
			ResXResourceWriter resW = (ResXResourceWriter)resWriter;
			IList<ResourcePointerImage> images = ResourceImageList;
			if (images.Count > 0)
			{
				foreach (ResourcePointerImage rs in images)
				{
					string file = rs.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(file))
					{
						if (System.IO.File.Exists(file))
						{
							ResXFileRef f = new ResXFileRef(file, typeof(Bitmap).AssemblyQualifiedName);
							resW.AddResource(new ResXDataNode(rs.CodeName, f));
							continue;
						}
					}
					Bitmap bp = (Bitmap)rs.GetResourceValue(languageName);
					if (bp == null)
					{
						bp = Resources.f_event;
					}
					if (string.IsNullOrEmpty(languageName))
					{
						resWriter.AddResource(rs.CodeName, bp);
					}
					else
					{
						if (bp != null)
						{
							resWriter.AddResource(rs.CodeName, bp);
						}
					}
				}
			}
			IList<ResourcePointerIcon> icons = ResourceIconList;
			if (icons.Count > 0)
			{
				foreach (ResourcePointerIcon rs in icons)
				{
					string file = rs.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(file))
					{
						if (System.IO.File.Exists(file))
						{
							ResXFileRef f = new ResXFileRef(file, typeof(Icon).AssemblyQualifiedName);
							resW.AddResource(new ResXDataNode(rs.CodeName, f));
							continue;
						}
					}
					Icon bp = (Icon)rs.GetResourceValue(languageName);
					if (bp == null)
					{
						//use default icon
						bp = Resources._ok;
					}
					if (string.IsNullOrEmpty(languageName))
					{
						resWriter.AddResource(rs.CodeName, bp);
					}
					else
					{
						if (bp != null)
						{
							resWriter.AddResource(rs.CodeName, bp);
						}
					}
				}
			}

			IList<ResourcePointerAudio> audios = ResourceAudioList;
			if (audios.Count > 0)
			{
				foreach (ResourcePointerAudio rs in audios)
				{
					string file = rs.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(file))
					{
						if (System.IO.File.Exists(file))
						{
							ResXFileRef f = new ResXFileRef(file, typeof(MemoryStream).AssemblyQualifiedName);
							resW.AddResource(new ResXDataNode(rs.CodeName, f));
							continue;
						}
					}
					MemoryStream bp = (MemoryStream)rs.GetResourceValue(languageName);
					if (string.IsNullOrEmpty(languageName))
					{
						resWriter.AddResource(rs.CodeName, bp);
					}
					else
					{
						if (bp != null)
						{
							resWriter.AddResource(rs.CodeName, bp);
						}
					}
				}
			}

			IList<ResourcePointerFile> files = ResourceFileList;
			if (files.Count > 0)
			{
				foreach (ResourcePointerFile rs in files)
				{
					string file = rs.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(file))
					{
						if (System.IO.File.Exists(file))
						{
							ResXFileRef f = new ResXFileRef(file, rs.ObjectType.AssemblyQualifiedName);
							resW.AddResource(new ResXDataNode(rs.CodeName, f));
							continue;
						}
					}
					if (string.IsNullOrEmpty(languageName))
					{
						resWriter.AddResource(rs.CodeName, file);
					}
				}
			}

			IList<ResourcePointerFilePath> filenames = ResourceFilenameList;
			if (filenames.Count > 0)
			{
				foreach (ResourcePointerFilePath rs in filenames)
				{
					if (rs.HasResource(languageName))//"" always returns true
					{
						string s = rs.GetResourceString(languageName);
						if (string.IsNullOrEmpty(s))
						{
							if (string.IsNullOrEmpty(languageName))
							{
								resWriter.AddResource(rs.CodeName, new Filepath(s));
							}
						}
						else
						{
							IDistributeFile f = null;
							foreach (IDistributeFile f0 in distributeFiles)
							{
								if (string.Compare(s, f0.FileName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									f = f0;
									break;
								}
							}
							if (f != null)
							{
								s = f.GetTargetPathWithToken();

							}
							resWriter.AddResource(rs.CodeName, new Filepath(s));
						}
						ret = true;
					}
				}
			}
			return ret;
		}
		public string CreateHelpClass()
		{
			string c = ClassName;
			if (!string.IsNullOrEmpty(_prj.Namespace))
			{
				c = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _prj.Namespace, ClassName);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				Resources.ResourceHelp, HelpClassName, c);
		}
		/// <summary>
		/// .resx
		/// </summary>
		/// <param name="languageName"></param>
		/// <returns></returns>
		public string FormResourcesFilename(string languageName)
		{
			string fn;
			if (string.IsNullOrEmpty(_prj.Namespace))
				fn = ClassName;
			else
				fn = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _prj.Namespace, ClassName);
			if (string.IsNullOrEmpty(languageName))
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}.resx", fn);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}.{1}.resx", fn, languageName);
		}
		public string FormBinResourcesFilename(string languageName)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}.{1}.{2}.resources",_prj.Namespace, ClassName, languageName);
		}
		public string ResourcesDllFilename
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}.resources.dll", ProjectCodeName);
			}
		}
		public string ClassName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}Resources", ProjectCodeName);
			}
		}
		public string HelpClassName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}ResHelp", ProjectCodeName);
			}
		}
		public string HelpClassFilename
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}ResHelp.cs", ProjectCodeName);
			}
		}
		public string ProjectCodeName
		{
			get
			{
				return _prj.ProjectAssemblyName;
				//return _prj.ProjectName.Replace(' ', '_');
			}
		}
		/// <summary>
		/// .cs
		/// </summary>
		public string SourceCodeFilename
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}.{1}.cs",_prj.Namespace, ClassName);
			}
		}
		/// <summary>
		/// .resources
		/// </summary>
		public string ResourcesFilename
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}.{1}.resources",_prj.Namespace, ClassName);
			}
		}
		#endregion

		#region Methods
		public IList<WebResourceFile> GetWebResourceFiles()
		{
			List<WebResourceFile> files = new List<WebResourceFile>();
			return files;
		}
		public bool CopyResourceFile(Form caller, string filename)
		{
			string target = System.IO.Path.Combine(ResourcesFolder,
				System.IO.Path.GetFileName(filename));
			if (string.Compare(target, filename, StringComparison.OrdinalIgnoreCase) != 0)
			{
				if (System.IO.File.Exists(target))
				{
					if (MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"File exist: {0}\r\nDo you want to overwrite it?", target), "Copy resource file", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return true;
					}
				}
				try
				{
					if (!System.IO.Directory.Exists(ResourcesFolder))
					{
						System.IO.Directory.CreateDirectory(ResourcesFolder);
					}
					System.IO.File.Copy(filename, target, true);
					return true;
				}
				catch (Exception e)
				{
					MessageBox.Show(caller, e.Message, "Copy resource file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			return false;
		}
		public void SetResourceValue(UInt32 id, object value)
		{
			ResourcePointer rp = GetResourcePointerById(id);
			if (rp != null)
			{
				if (rp.SetValue(value))
				{
					_xmlNode.OwnerDocument.Save(_prj.ResourcesFile);
				}
			}
		}
		public void NotifyLanguageChange()
		{
			Dictionary<UInt32, ILimnorDesignPane> designPanes = Project.GetTypedDataList<ILimnorDesignPane>();
			foreach (ILimnorDesignPane pn in designPanes.Values)
			{
				pn.OnCultureChanged();
			}
		}
		public void NotifyResourcesEdited()
		{
			Dictionary<UInt32, ILimnorDesignPane> designPanes = Project.GetTypedDataList<ILimnorDesignPane>();
			foreach (ILimnorDesignPane pn in designPanes.Values)
			{
				pn.OnResourcesEdited();
			}
		}
		[Browsable(false)]
		public bool Save()
		{
			if (_xmlNode == null)
			{
				_xmlNode = _prj.ResourcesXmlNode;
				IsChanged = true;
			}
			if (_stringList != null)
			{
				foreach (ResourcePointerString rs in _stringList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (_imageList != null)
			{
				foreach (ResourcePointerImage rs in _imageList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (_iconList != null)
			{
				foreach (ResourcePointerIcon rs in _iconList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (_audioList != null)
			{
				foreach (ResourcePointerAudio rs in _audioList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (_fileList != null)
			{
				foreach (ResourcePointerFile rs in _fileList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (_filenameList != null)
			{
				foreach (ResourcePointerFilePath rs in _filenameList)
				{
					if (rs.Save())
					{
						IsChanged = true;
					}
				}
			}
			if (IsChanged)
			{
				_xmlNode.OwnerDocument.Save(_prj.ResourcesFile);
				IsChanged = false;
				return true;
			}
			return false;
		}
		public ResourcePointer GetResourcePointerById(UInt32 id)
		{
			IList<ResourcePointerString> ls = ResourceStringList;
			foreach (ResourcePointerString r in ls)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			IList<ResourcePointerImage> li = ResourceImageList;
			foreach (ResourcePointerImage r in li)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			IList<ResourcePointerIcon> lic = ResourceIconList;
			foreach (ResourcePointerIcon r in lic)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			IList<ResourcePointerAudio> la = ResourceAudioList;
			foreach (ResourcePointerAudio r in la)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			IList<ResourcePointerFile> lf = ResourceFileList;
			foreach (ResourcePointerFile r in lf)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			IList<ResourcePointerFilePath> lp = ResourceFilenameList;
			foreach (ResourcePointerFilePath r in lp)
			{
				if (r.MemberId == id)
				{
					return r;
				}
			}
			return null;
		}
		public bool IsNameInUse(string name)
		{
			XmlNodeList ns = RootNode.SelectNodes("//*[@name]");
			foreach (XmlNode nd in ns)
			{
				if (string.Compare(name, XmlUtil.GetNameAttribute(nd), StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public void RemoveResource(UInt32 id)
		{
			XmlNode nd = RootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}/{1}[@name='MemberId' and text() = '{2}']",
				XmlTags.XML_Item, XmlTags.XML_PROPERTY, id));
			if (nd != null)
			{
				XmlNode np = nd.ParentNode.ParentNode;
				np.RemoveChild(nd.ParentNode);
				IsChanged = true;
			}
			bool bFound = false;
			if (_stringList != null)
			{
				foreach (ResourcePointerString s in _stringList)
				{
					if (s.MemberId == id)
					{
						_stringList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
			if (!bFound && _imageList != null)
			{
				foreach (ResourcePointerImage s in _imageList)
				{
					if (s.MemberId == id)
					{
						_imageList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
			if (!bFound && _iconList != null)
			{
				foreach (ResourcePointerIcon s in _iconList)
				{
					if (s.MemberId == id)
					{
						_iconList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
			if (!bFound && _audioList != null)
			{
				foreach (ResourcePointerAudio s in _audioList)
				{
					if (s.MemberId == id)
					{
						_audioList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
			if (!bFound && _fileList != null)
			{
				foreach (ResourcePointerFile s in _fileList)
				{
					if (s.MemberId == id)
					{
						_fileList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
			if (!bFound && _filenameList != null)
			{
				foreach (ResourcePointerFilePath s in _filenameList)
				{
					if (s.MemberId == id)
					{
						_filenameList.Remove(s);
						bFound = true;
						break;
					}
				}
			}
		}
		public void RemoveLanguage(string name)
		{
			if (_languageNames != null)
			{
				if (_languageNames.Contains(name))
				{
					_languageNames.Remove(name);
				}
			}
			if (XmlUtil.RemoveLanguage(RootNode, name))
			{
				IsChanged = true;
			}
		}
		public bool SelectLanguages(Form caller)
		{
			StringCollection sc = DlgLanguages.SelectLanguages(caller, Languages);
			if (sc != null)
			{
				UpdateLanguages(sc);
				return true;
			}
			return false;
		}
		public void UpdateLanguages(StringCollection languages)
		{
			if (languages != null)
			{
				_languageNames = new List<string>();
				foreach (string s in languages)
				{
					if (!string.IsNullOrEmpty(s))
					{
						_languageNames.Add(s);
					}
				}
				XmlUtil.UpdateLanguages(RootNode, languages);
				IsChanged = true;
			}
		}
		public string CreateNewName(string namebase)
		{
			StringCollection sc = new StringCollection();
			XmlNodeList ns = RootNode.SelectNodes("//*[@name]");
			foreach (XmlNode nd in ns)
			{
				sc.Add(XmlUtil.GetNameAttribute(nd));
			}
			string name;
			int n = 1;
			while (true)
			{
				name = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}{1}", namebase, n);
				bool bexists = false;
				for (int i = 0; i < sc.Count; i++)
				{
					if (string.Compare(sc[i], name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						n++;
						bexists = true;
						break;
					}
				}
				if (!bexists)
				{
					break;
				}
			}
			return name;
		}

		public XmlNode GetResourceXmlNode(ResourcePointer pointer)
		{
			XmlNode proot = XmlUtil.CreateSingleNewElement(RootNode, pointer.XmlTag);
			XmlNode nd = proot.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@name='MemberId' and text()='{2}']",
				XmlTags.XML_Item, XmlTags.XML_PROPERTY, pointer.MemberId));
			if (nd == null)
			{
				nd = proot.OwnerDocument.CreateElement(XmlTags.XML_Item);
				proot.AppendChild(nd);
				ObjectXmlWriter xw = new ObjectXmlWriter();
				xw.WriteObjectToNode(nd, pointer);
			}
			else
			{
				nd = nd.ParentNode;
			}
			return nd;
		}
		public IList<T> GetResourceList<T>() where T : ResourcePointer
		{
			if (typeof(T).Equals(typeof(ResourcePointerString)))
			{
				return (IList<T>)ResourceStringList;
			}
			if (typeof(T).Equals(typeof(ResourcePointerImage)))
			{
				return (IList<T>)ResourceImageList;
			}

			if (typeof(T).Equals(typeof(ResourcePointerIcon)))
			{
				return (IList<T>)ResourceIconList;
			}
			if (typeof(T).Equals(typeof(ResourcePointerAudio)))
			{
				return (IList<T>)ResourceAudioList;
			}

			if (typeof(T).Equals(typeof(ResourcePointerFile)))
			{
				return (IList<T>)ResourceFileList;
			}
			if (typeof(T).Equals(typeof(ResourcePointerFilePath)))
			{
				return (IList<T>)ResourceFilenameList;
			}
			throw new DesignerException("Resource type not supported: {0}", typeof(T).FullName);
		}
		#endregion

		#region strings
		public IList<ResourcePointerString> ResourceStringList
		{
			get
			{
				if (_stringList == null)
				{
					_stringList = new List<ResourcePointerString>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_STRINGS, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerString rs = xr.ReadObject<ResourcePointerString>(nd, this);
						_stringList.Add(rs);
					}
				}
				return _stringList;
			}
		}
		public ResourcePointerString AddNewString()
		{
			string name = CreateNewName("String");
			ResourcePointerString r = new ResourcePointerString(this);
			r.Name = name;
			if (_stringList != null)
			{
				_stringList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_STRINGS);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}

		#endregion

		#region images
		public IList<ResourcePointerImage> ResourceImageList
		{
			get
			{
				if (_imageList == null)
				{
					_imageList = new List<ResourcePointerImage>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_IMAGES, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerImage rs = xr.ReadObject<ResourcePointerImage>(nd, this);
						_imageList.Add(rs);
					}
				}
				return _imageList;
			}
		}
		public ResourcePointerImage AddNewImage()
		{
			string name = CreateNewName("Image");
			ResourcePointerImage r = new ResourcePointerImage(this);
			r.Name = name;
			if (_imageList != null)
			{
				_imageList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_IMAGES);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}
		#endregion

		#region icons
		public IList<ResourcePointerIcon> ResourceIconList
		{
			get
			{
				if (_iconList == null)
				{
					_iconList = new List<ResourcePointerIcon>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_ICONS, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerIcon rs = xr.ReadObject<ResourcePointerIcon>(nd, this);
						_iconList.Add(rs);
					}
				}
				return _iconList;
			}
		}
		public ResourcePointerIcon AddNewIcon()
		{
			string name = CreateNewName("Icon");
			ResourcePointerIcon r = new ResourcePointerIcon(this);
			r.Name = name;
			if (_iconList != null)
			{
				_iconList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_ICONS);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}

		#endregion

		#region audios
		public IList<ResourcePointerAudio> ResourceAudioList
		{
			get
			{
				if (_audioList == null)
				{
					_audioList = new List<ResourcePointerAudio>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_AUDIOS, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerAudio rs = xr.ReadObject<ResourcePointerAudio>(nd, this);
						_audioList.Add(rs);
					}
				}
				return _audioList;
			}
		}
		public ResourcePointerAudio AddNewAudio()
		{
			string name = CreateNewName("Audio");
			ResourcePointerAudio r = new ResourcePointerAudio(this);
			r.Name = name;
			if (_audioList != null)
			{
				_audioList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_AUDIOS);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}
		#endregion

		#region files
		public IList<ResourcePointerFile> ResourceFileList
		{
			get
			{
				if (_fileList == null)
				{
					_fileList = new List<ResourcePointerFile>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_FILES, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerFile rs = xr.ReadObject<ResourcePointerFile>(nd, this);
						_fileList.Add(rs);
					}
				}
				return _fileList;
			}
		}
		public ResourcePointerFile AddNewFile()
		{
			string name = CreateNewName("File");
			ResourcePointerFile r = new ResourcePointerFile(this);
			r.Name = name;
			if (_fileList != null)
			{
				_fileList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_FILES);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}
		#endregion

		#region filenames
		public IList<ResourcePointerFilePath> ResourceFilenameList
		{
			get
			{
				if (_filenameList == null)
				{
					_filenameList = new List<ResourcePointerFilePath>();

					ObjectXmlReader xr = new ObjectXmlReader();
					XmlNodeList ns = RootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_FILENAMES, XmlTags.XML_Item));
					foreach (XmlNode nd in ns)
					{
						ResourcePointerFilePath rs = xr.ReadObject<ResourcePointerFilePath>(nd, this);
						_filenameList.Add(rs);
					}
				}
				return _filenameList;
			}
		}
		public ResourcePointerFilePath AddNewFilePath()
		{
			string name = CreateNewName("Filename");
			ResourcePointerFilePath r = new ResourcePointerFilePath(this);
			r.Name = name;
			if (_filenameList != null)
			{
				_filenameList.Add(r);
			}
			XmlNode nstr = XmlUtil.CreateSingleNewElement(RootNode, XML_FILENAMES);
			XmlNode nd = nstr.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nstr.AppendChild(nd);
			XmlUtil.SetNameAttribute(nd, name);
			ObjectXmlWriter xw = new ObjectXmlWriter();
			xw.WriteObjectToNode(nd, r);
			IsChanged = true;
			return r;
		}
		#endregion

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public ClassPointer RootPointer
		{
			get { return null; }
		}
		public bool IsChanged { get; set; }
		public IObjectPointer Owner
		{
			get
			{
				return null;
			}
			set
			{

			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		public Type ObjectType
		{
			get
			{
				return this.GetType();
			}
			set
			{

			}
		}

		public object ObjectInstance
		{
			get
			{
				return this;
			}
			set
			{

			}
		}

		public object ObjectDebug
		{
			get;
			set;
		}

		public string ReferenceName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Res{0}", _prj.ProjectGuid.ToString("N"));
			}
		}

		public string CodeName
		{
			get { return ReferenceName; }
		}

		public string DisplayName
		{
			get { return "Resource Manager"; }
		}

		public string LongDisplayName
		{
			get { return DisplayName; }
		}

		public string ExpressionDisplay
		{
			get { return "ResourceManager"; }
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			return false;
		}

		public string ObjectKey
		{
			get { return ReferenceName; }
		}

		public string TypeString
		{
			get { return ReferenceName; }
		}

		public bool IsValid
		{
			get { return true; }
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			throw new NotImplementedException();
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ProjectResources rm = objectIdentity as ProjectResources;
			if (rm != null)
			{
				return (rm._prj.ProjectGuid == _prj.ProjectGuid);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}

		public bool IsStatic
		{
			get { return true; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Unknown; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ProjectResources rm = new ProjectResources(_prj);
			rm._xmlNode = _xmlNode;
			return rm;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			_xmlNode = objectNode;
		}

		#endregion


		#region IProjectAccessor Members

		ILimnorProject IProjectAccessor.Project
		{
			get
			{
				return _prj;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}

		#endregion
	}
}
