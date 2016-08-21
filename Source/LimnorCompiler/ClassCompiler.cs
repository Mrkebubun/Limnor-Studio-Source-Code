/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.Xml;
using XmlUtility;
using LimnorDesigner;
using System.Globalization;
using LimnorWeb;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using TraceLog;
using Limnor.WebBuilder;
using VPL;
using System.Reflection;

namespace LimnorCompiler
{
	/// <summary>
	/// compile one class in a new domain
	/// </summary>
	class ClassCompiler : MarshalByRefObject
	{
		private LimnorProject _project;
		private bool _isApp;
		private UInt32 _startWebPageId;
		private bool _isWebPage;
		private string _htmlFile;
		private string _appConfigFile;
		private UInt32 _classId;
		private List<Guid> _databaseConnectionIDs;
		private bool _usePageNavigator;
		private bool _useCopyProtection;
		private bool _usedatetimepicker;
		private bool _isWebService;
		private List<string> _assemblyLocations;
		private string _use32bit;
		private List<string> _lstResxFiles;
		private List<string> _lstSourceFiles;
		private List<ResourceFile> _embededFiles;
		//
		private string _assemblyQualifiedTypeName;
		private bool _isBaseTypeInGAC;
		private string _assemblyLocation;
		private string _resourceFile;
		private string _resourceFileX;
		private string _sourceFile;
		private string _sourceFileX;
		//
		private List<string> _errors;
		private List<string> _logs;
		private List<string> _assemblySupportFiles;
		private bool _useResources;
		private bool _useResourcesX;
		private bool _isForm;
		private bool _useDrawing;
		private string _rootNamespace;
		private string _outputFullpath;
		private string _applicationIcon;
		private UInt32[] _classIdList;
		private string[] _classNames;
		private string _appName;
		private string _kioskBackgroundName;
		private string _webJsFolder;
		private string _webPhysicalFolder;
		private string _webJavaScriptFolder;
		private string _webPhpFolder;
		private string _appCodeName;
		private string _pluginsFolder;
		private string[] _sessionVarNames;
		private string[] _sessionVarValues;
		private bool _webDebugMode;
		private int _webClientDebugLevel;
		private int _ajaxtimeout;
		private int _sessionTimeoutMinutes;
		private int _sessionVarCount;
		public ClassCompiler()
		{
			_databaseConnectionIDs = new List<Guid>();
			_assemblyLocations = new List<string>();
			_lstResxFiles = new List<string>();
			_lstSourceFiles = new List<string>();
			_embededFiles = new List<LimnorCompiler.ResourceFile>();
			_errors = new List<string>();
			_logs = new List<string>();
			_assemblySupportFiles = new List<string>();
		}
		public int SessionVarCount
		{
			get
			{
				return _sessionVarCount;
			}
		}
		public int SessionTimeoutMinutes
		{
			get
			{
				return _sessionTimeoutMinutes;
			}
		}
		public string AppConfigFile
		{
			get
			{
				return _appConfigFile;
			}
		}
		public string[] SessionVarValues
		{
			get
			{
				return _sessionVarValues;
			}
		}
		public string[] SessionVarNames
		{
			get
			{
				return _sessionVarNames;
			}
		}
		public int WebClientDebugLevel
		{
			get
			{
				return _webClientDebugLevel;
			}
		}
		public bool WebDebugMode
		{
			get
			{
				return _webDebugMode;
			}
		}
		public int AjaxTimeout
		{
			get
			{
				return _ajaxtimeout;
			}
		}
		public string PluginsFolder
		{
			get
			{
				return _pluginsFolder;
			}
		}
		public string AppCodeName
		{
			get
			{
				return _appCodeName;
			}
		}
		public string KioskBackgroundName
		{
			get
			{
				return _kioskBackgroundName;
			}
		}
		public string WebPhpFolder
		{
			get
			{
				return _webPhpFolder;
			}
		}
		public string WebJavaScriptFolder
		{
			get
			{
				return _webJavaScriptFolder;
			}
		}
		public string WebPhysicalFolder
		{
			get
			{
				return _webPhysicalFolder;
			}
		}
		public string WebJsFolder
		{
			get
			{
				return _webJsFolder;
			}
		}
		public string AppName
		{
			get
			{
				return _appName;
			}
		}
		public string ApplicationIcon
		{
			get
			{
				return _applicationIcon;
			}
		}
		public string OutputFullpath
		{
			get
			{
				return _outputFullpath;
			}
		}
		public string RootNamespace
		{
			get
			{
				return _rootNamespace;
			}
		}
		public bool UseDrawing
		{
			get
			{
				return _useDrawing;
			}
		}
		public bool IsForm
		{
			get
			{
				return _isForm;
			}
		}
		public bool IsWebPage
		{
			get
			{
				return _isWebPage;
			}
		}
		public bool UseResourcesX
		{
			get
			{
				return _useResourcesX;
			}
		}
		public bool UseResources
		{
			get
			{
				return _useResources;
			}
		}
		public string[] AssemblySupportFiles
		{
			get
			{
				return _assemblySupportFiles.ToArray();
			}
		}
		public string SourceFileX
		{
			get
			{
				return _sourceFileX;
			}
		}
		public string SourceFile
		{
			get
			{
				return _sourceFile;
			}
		}
		public string ResourceFileX
		{
			get
			{
				return _resourceFileX;
			}
		}
		public string ResourceFile
		{
			get
			{
				return _resourceFile;
			}
		}
		public string AssemblyLocation
		{
			get
			{
				return _assemblyLocation;
			}
		}
		public bool IsBaseTypeInGAC
		{
			get
			{
				return _isBaseTypeInGAC;
			}
		}
		public string AssemblyQualifiedTypeName
		{
			get
			{
				return _assemblyQualifiedTypeName;
			}
		}
		//
		public ResourceFile[] EmbededFiles
		{
			get
			{
				return _embededFiles.ToArray();
			}
		}
		public string[] SourceFiles
		{
			get
			{
				return _lstSourceFiles.ToArray();
			}
		}
		public string[] ResxFiles
		{
			get
			{
				return _lstResxFiles.ToArray();
			}
		}
		public string Use32bit
		{
			get
			{
				return _use32bit;
			}
		}
		public string[] AssemblyLocations
		{
			get
			{
				return _assemblyLocations.ToArray();
			}
		}
		public bool IsWebService
		{
			get
			{
				return _isWebService;
			}
		}
		public bool UseDatetimePicker
		{
			get
			{
				return _usedatetimepicker;
			}
		}
		public bool UseCopyProtection
		{
			get
			{
				return _useCopyProtection;
			}
		}
		public bool UsePageNavigator
		{
			get
			{
				return _usePageNavigator;
			}
		}
		public Guid[] DatabaseConnectionIDs
		{
			get
			{
				return _databaseConnectionIDs.ToArray();
			}
		}
		public bool IsApp
		{
			get
			{
				return _isApp;
			}
		}
		public UInt32 StartWebPageId
		{
			get
			{
				return _startWebPageId;
			}
		}
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
		}
		public string HtmlFilePath
		{
			get
			{
				return _htmlFile;
			}
		}
		public string[] Errors
		{
			get
			{
				return _errors.ToArray();
			}
		}
		public string[] Logs
		{
			get
			{
				return _logs.ToArray();
			}
		}
		public void LogMessage(string msg, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			_logs.Add(msg);
		}
		static public string FormExceptionText(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			if (e.StackTrace != null)
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				if (e.StackTrace != null)
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
		public void LogError(Exception e)
		{
			LogError(FormExceptionText(e));
		}
		public void LogError(string msg, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			_errors.Add(msg);
		}
		private string use32bitAssembly(Dictionary<string, Assembly> assemblyLocations)
		{
			if (assemblyLocations != null && assemblyLocations.Count > 0)
			{
				string osFolder = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System));
				foreach (Assembly a in assemblyLocations.Values)
				{
					try
					{
						if (a.GlobalAssemblyCache)
						{
							if (a.Location.StartsWith(osFolder, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
						}
						AssemblyDetails ad = AssemblyDetails.FromFile(a.Location);
						if (ad != null)
						{
							if (ad.CPUVersion == CPUVersion.x86)
							{
								return a.FullName;
							}
						}
					}
					catch
					{
					}
				}
			}
			return string.Empty;
		}
		private void addAssemblySupportFiles(Dictionary<string, Assembly> assemblyLocations)
		{
			foreach (Assembly a in assemblyLocations.Values)
			{
				object[] objs = a.GetCustomAttributes(typeof(SupportFileAttribute), true);
				if (objs != null && objs.Length > 0)
				{
					SupportFileAttribute fs = objs[0] as SupportFileAttribute;
					foreach (string f in fs.Filenames)
					{
						_assemblySupportFiles.Add(f);
					}
				}
			}
		}
		public void SetUseDrawing()
		{
			_useDrawing = true;
		}
		public void SetAppIcon(string icon)
		{
			_applicationIcon = icon;
		}
		public string GetClassTypeName(UInt32 classId)
		{
			for (int i = 0; i < _classIdList.Length; i++)
			{
				if (_classIdList[i] == classId)
				{
					return _classNames[i];
				}
			}
			return null;
		}
		public string GetCultureFolder(string cultureName)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				return WebJsFolder;
			}
			string folder = Path.Combine(WebPhysicalFolder, cultureName);
			if (!System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.CreateDirectory(folder);
			}
			return folder;
		}
		public bool Compile(string projectFile, string classFile, string kioskBackgroundName,
			string rootNamespace, string mSBuildProjectDirectory, string webPhysicalFolder, string outputFullpath,
			UInt32[] classIds, string[] classNames, string appName, string webJsFolder, string webJavaScriptFolder,
			string webPhpFolder, string appCodeName, string pluginsFolder, string[] sessionVarNames, string[] sessionVarValues,
			int sessionTimeoutMinutes, int sessionVarCount, bool webDebugMode, int webClientDebugLevel, int ajaxtimeout, bool debug)
		{
			_project = new LimnorProject(projectFile);
			_rootNamespace = rootNamespace;
			_outputFullpath = outputFullpath;
			_classIdList = classIds;
			_classNames = classNames;
			_appName = appName;
			_webJsFolder = webJsFolder;
			_webPhysicalFolder = webPhysicalFolder;
			_webJavaScriptFolder = webJavaScriptFolder;
			_webPhpFolder = webPhpFolder;
			_appCodeName = appCodeName;
			_kioskBackgroundName = kioskBackgroundName;
			_pluginsFolder = pluginsFolder;
			_sessionVarNames = sessionVarNames;
			_sessionVarValues = sessionVarValues;
			_sessionTimeoutMinutes = sessionTimeoutMinutes;
			_sessionVarCount = sessionVarCount;
			_webDebugMode = webDebugMode;
			_webClientDebugLevel = webClientDebugLevel;
			_ajaxtimeout = ajaxtimeout;
			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = false;
			doc.Load(classFile);
			try
			{
				Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement);
				_isForm = (typeof(Form).Equals(t) || t.IsSubclassOf(typeof(Form)));
				if (_project.ProjectType == EnumProjectType.Kiosk)
				{
					if (typeof(LimnorKioskApp).Equals(t))
					{
						string bkname = kioskBackgroundName;
						if (!string.IsNullOrEmpty(bkname))
						{
							XmlNode propNode = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}[@{1}='BackGroundType']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
							if (propNode == null)
							{
								propNode = doc.CreateElement(XmlTags.XML_PROPERTY);
								doc.DocumentElement.AppendChild(propNode);
							}
							propNode.InnerText = bkname;
						}
					}
				}
				else if (_project.ProjectType == EnumProjectType.WebAppAspx || _project.ProjectType == EnumProjectType.WebAppPhp)
				{
					if (typeof(LimnorWebApp).IsAssignableFrom(t))
					{
						XmlNode wnNode = doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
							"{0}[@{1}='WebSiteName']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
						if (wnNode != null)
						{
							string wn = wnNode.InnerText.Trim();
							if (wn.StartsWith("$", StringComparison.Ordinal))
							{
								wn = XmlUtil.GetNameAttribute(doc.DocumentElement);
								if (string.IsNullOrEmpty(wn))
								{
									wn = string.Format(CultureInfo.InvariantCulture, "web_{0}", Guid.NewGuid().ToString("x", CultureInfo.InvariantCulture));
								}
								wnNode.InnerText = wn;
								doc.Save(classFile);
							}
						}
					}
				}
				//
				LogMessage("Creating handler: {0}", classFile);
				CodeCompiler cc = new CodeCompiler(this, _project, doc, classFile, rootNamespace, mSBuildProjectDirectory, _appName, _kioskBackgroundName, debug);
				bool bOK = true;
				LogMessage("Loading into handler: {0}", cc.ComponentFile);
				try
				{
					bool b = cc.Load();
					if (!b)
					{
						ArrayList errors = cc.Errors;
						for (int j = 0; j < errors.Count; j++)
						{
							string msg;
							Exception e = errors[j] as Exception;
							if (e != null)
							{
								msg = TraceLogClass.ExceptionMessage0(e);
							}
							else
							{
								msg = errors[j].ToString();
							}
							LogError(msg);
						}
						bOK = false;
					}
					else
					{
						_classId = cc.ClassData.ClassId;
						if (cc.RootObject is LimnorApp)
						{
							_isApp = true;
						}
						LimnorWebApp wapp = cc.RootObject as LimnorWebApp;
						if (wapp != null)
						{
							FormCompile frm = new FormCompile();
							_startWebPageId = wapp.StartPageId;
							try
							{
								bool newName;
								frm.Show();
								VirtualWebDir webSite = wapp.CreateWebSite(frm, out newName);
								if (webSite != null)
								{
								}
							}
							catch (Exception err)
							{
								LogError("Cannot create website [{0}] for testing purpose. The web application is generated at [{1}]. You may copy the files in that folder to a website to test it. Error message:{2} Stack trace: {3}", wapp.WebSiteName, webPhysicalFolder, err.Message, err.StackTrace);
							}
							finally
							{
								frm.Close();
								frm = null;
							}
						}
						else
						{
							IWebPage wp = cc.RootObject as IWebPage;
							if (wp != null)
							{
								_isWebPage = true;
								_htmlFile = cc.HtmlFile;
							}
						}
						LogMessage("Loaded: {0}", cc.ComponentFile);
					}
				}
				catch (Exception err0)
				{
					string msg = TraceLogClass.ExceptionMessage0(err0);
					LogError(msg);
					LogMessage("Loading failed for {0}", cc.ComponentFile);
					bOK = false;
				}
				if (bOK)
				{
					IList<Guid> l = cc.ClassData.GetDatabaseConnectionsUsed();
					if (l.Count > 0)
					{
						foreach (Guid g in l)
						{
							if (!_databaseConnectionIDs.Contains(g))
							{
								_databaseConnectionIDs.Add(g);
							}
						}
					}
					if (cc.UsePageNavigator())
					{
						_usePageNavigator = true;
					}
					if (cc.UseCopyProtection())
					{
						_useCopyProtection = true;
					}
					LogMessage("Compiling {0}", cc.ComponentFile);
					try
					{
						//flush is trigged
						bool b = cc.GenerateCodeFiles();
						if (!b)
						{
							ArrayList errors = cc.Errors;
							for (int j = 0; j < errors.Count; j++)
							{
								string msg;
								Exception e = errors[j] as Exception;
								if (e != null)
								{
									msg = TraceLogClass.ExceptionMessage0(e);
								}
								else
								{
									msg = errors[j].ToString();
								}
								LogError(msg);
							}
							bOK = false;
						}
						else
						{
							//must do it before cleanup
							List<IAppConfigConsumer> acs = cc.AppConfigConsumers;
							if (acs != null && acs.Count > 0)
							{
								List<IAppConfigConsumer> appConsumers = new List<IAppConfigConsumer>();
								foreach (IAppConfigConsumer ia in acs)
								{
									bool bExist = false;
									foreach (IAppConfigConsumer ia0 in appConsumers)
									{
										if (ia0.IsSameConsumer(ia))
										{
											bExist = true;
											break;
										}
									}
									if (!bExist)
									{
										appConsumers.Add(ia);
									}
								}
								if (appConsumers.Count > 0)
								{
									CompilerFolders folders = new CompilerFolders(mSBuildProjectDirectory);
									_appConfigFile = Path.Combine(folders.SourceFolder, "app.config");
									XmlDocument appCfg = new XmlDocument();
									XmlNode cfgRoot;
									if (File.Exists(_appConfigFile))
									{
										appCfg.Load(_appConfigFile);
									}
									cfgRoot = appCfg.DocumentElement;
									if (cfgRoot == null)
									{
										cfgRoot = appCfg.CreateElement("configuration");
										appCfg.AppendChild(cfgRoot);
									}
									foreach (IAppConfigConsumer ia in appConsumers)
									{
										ia.MergeAppConfig(cfgRoot, _project.ProjectFolder, rootNamespace);
									}
									appCfg.Save(_appConfigFile);
								}
							}
							if (cc.UseDatetimePicker)
							{
								_usedatetimepicker = true;
							}
							LogMessage("Compiled: {0}\r\n", cc.ComponentFile);
						}
					}
					catch (Exception err0)
					{
						string msg = TraceLogClass.ExceptionMessage0(err0);
						LogError(msg);
						LogMessage("Compiling failed for {0}\r\n", cc.ComponentFile);
						bOK = false;
					}
					finally
					{
						cc.Cleanup();
					}
				}
				else
				{
					cc.Cleanup();
				}
				if (bOK)
				{
					Dictionary<string, Assembly> assemblyLocations = new Dictionary<string, Assembly>();
					cc.FindReferenceLocations(assemblyLocations);
					foreach (string s in assemblyLocations.Keys)
					{
						_assemblyLocations.Add(s);
					}
					addAssemblySupportFiles(assemblyLocations);
					if (cc.IsWebService)
					{
						_isWebService = true;
					}

					_use32bit = cc.Report32Usage();
					if (string.IsNullOrEmpty(_use32bit))
					{
						_use32bit = use32bitAssembly(assemblyLocations);
					}
					if (cc.UseResources || cc.UseResourcesX)
					{
						LogMessage("Use resources by {0}", cc.ComponentFile);
						if (_project.ProjectType != EnumProjectType.WebAppPhp)
						{
							cc.GenerateResourceFile();
						}
						if (cc.UseResources)
						{
							_lstResxFiles.Add(cc.Resources);
							_useResources = true;
						}
						if (cc.UseResourcesX)
						{
							_lstResxFiles.Add(cc.ResourcesX);
							_useResourcesX = true;
						}
					}
					else
					{
						LogMessage("Not use resources by {0}", cc.ComponentFile);
					}
					_lstSourceFiles.Add(cc.SourceFile);
					if (cc.UseResourcesX)
					{
						_lstSourceFiles.Add(cc.SourceFileX);
					}
					if (cc.ResourceFiles != null)
					{
						_embededFiles = cc.ResourceFiles;
					}
				}
				if (cc.ObjectType != null)
				{
					_assemblyQualifiedTypeName = cc.ObjectType.AssemblyQualifiedName;
					if (cc.ObjectType.Assembly != null)
					{
						_isBaseTypeInGAC = cc.ObjectType.Assembly.GlobalAssemblyCache;
						_assemblyLocation = cc.ObjectType.Assembly.Location;
					}
				}
				_resourceFile = cc.ResourceFile;
				_resourceFileX = cc.ResourceFileX;
				_sourceFile = cc.SourceFile;
				_sourceFileX = cc.SourceFileX;
				LogMessage("Finish compiling file :{0}", classFile);
				return bOK;
			}
			catch (Exception err)
			{
				LogError(err);
			}
			return false;
		}
		public LimnorProject Project
		{
			get
			{
				return _project;
			}
		}
	}
}
