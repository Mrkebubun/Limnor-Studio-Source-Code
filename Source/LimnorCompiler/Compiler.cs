/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using TraceLog;
using VPL;
using VSPrj;
using LimnorDesigner;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections;
using LimnorDesigner.ResourcesManager;
using Microsoft.CSharp;
using System.Diagnostics;
using System.Resources;
using MathExp;
using System.Xml;
using LimnorKiosk;
using XmlUtility;
using LimnorDatabase;
using System.Drawing;
using System.Threading;
using Limnor.Drawing2D;
using WindowsUtility;
using LimnorDesigner.Action;
using System.Windows.Forms;
using XmlSerializer;
using LimnorWeb;
using System.Globalization;
using Limnor.WebBuilder;
using System.CodeDom;
using Limnor.WebServerBuilder;
using Limnor.Windows;
using LimnorVisualProgramming;
using FileUtil;

namespace LimnorCompiler
{
	public class LimnorXmlCompiler : IErrorLog, ILimnorBuilder, IShowMessage
	{
		const string msBuildUri = "http://schemas.microsoft.com/developer/msbuild/2003";
		const string appRegExe = "AppLicense.exe";
		const string appRegExeDebug = "AppLicenseDebug.exe";
		const string appRegDll1 = "w98.dll";
		const string appRegDll2 = "w9864.dll";
		const string appRegDll3 = "NativeWindowsUtility.dll";
		const string appRegDll4 = "NativeWindowsUtility64.dll";
		//
		public ICompilerLog Log;
		public static string LastCompiledHtmlFile;
		public bool CopyBinFileFailed;
		//
		public IOutputWindow OutputWindow;
		private Dictionary<Type, object> _services;
		private List<UInt32> _modifiedPages;
		//
		private LimnorProject _project;
		private bool _usedatetimepicker;
		private string _use32bit = string.Empty;
		private ArrayList _lstSourceFiles = new ArrayList();
		private ArrayList _lstResxFiles = new ArrayList();
		private List<string> _embededFiles = new List<string>();
		private Dictionary<UInt32, string> _webPages;
		private UInt32 _startWebPageId;
		private string _appName;
		private string _appCodeName;
		private string _kioskBackgroundName;
		private bool _increment;
		private SortedDictionary<UInt32, ComponentID> _classList;
		private List<ComponentCompileResult> _componentCompileResults;
		private StringCollection _excludeFiles;
		private string appcfgFile;
		private string _appcfgFileInSourceFolder;
		private bool _debug;
		private int _sessionTimeoutMinutes;
		private int _sessionVarCount;
		private bool _copyProtectionDebug;
		private string _webJsFolder;
		private UInt32[] _classIds;
		private string[] _classNames;
		private string[] _sessionVarNames;
		private string[] _sessionVarValues;
		static private StringCollection _assemblySupportFiles;
		static private StringCollection _limnorFiles;
		static private StringCollection _existReferences;
		static private StringCollection _excludeNamespaces;
		static private StringCollection _copiedFiles;
		private List<Guid> databaseConnectionIDs;
		private bool shouldCompileBinary;
		private StringCollection dbConfigFiles;
		private bool isWebService;
		private List<IAppConfigConsumer> appConsumers;
		private string webTargetFolder;
		private Dictionary<string, Assembly> _assemblyLocations;
		private StringCollection _usedAssemblies;

		private string[] _languageNames;
		private bool _hasProjectResources;
		private Type disType = null;
		private EnumCompileTarget _compileTarget;
		private string _targetFileName;
		private bool _webDebugMode; //for web app 
		private int _webClientDebugLevel; //for web app
		private int _ajaxTimeout = 0; //for web app
		private int _skippedClasses = 0;
		private int _compiledClasses = 0;
		//
		public const string SOURCE = "source";
		//
		static LimnorXmlCompiler()
		{
			_assemblySupportFiles = new StringCollection();
			//
			_limnorFiles = new StringCollection();
			VPLUtil.SessionDataStorage = EnumSessionDataStorage.Cookies;
			//
			_limnorFiles.Add("DynamicEventLinker.dll".ToLowerInvariant());
			_limnorFiles.Add("FileUtil.dll".ToLowerInvariant());
			_limnorFiles.Add("LFilePath.dll".ToLowerInvariant());
			_limnorFiles.Add("LimnorBuild.dll".ToLowerInvariant());
			_limnorFiles.Add("LimnorDesigner.dll".ToLowerInvariant());
			_limnorFiles.Add("LimnorKiosk.dll".ToLowerInvariant());
			_limnorFiles.Add("MathExp.dll".ToLowerInvariant());
			_limnorFiles.Add("MathItem.dll".ToLowerInvariant());
			_limnorFiles.Add("PMEXmlParser.dll".ToLowerInvariant());
			_limnorFiles.Add("ProgElements.dll".ToLowerInvariant());
			_limnorFiles.Add("SerializeInterface.dll".ToLowerInvariant());
			_limnorFiles.Add("TraceLog.dll".ToLowerInvariant());
			_limnorFiles.Add("UIUtil.dll".ToLowerInvariant());
			_limnorFiles.Add("VPL.dll".ToLowerInvariant());
			_limnorFiles.Add("VPLDrawing.dll".ToLowerInvariant());
			_limnorFiles.Add("VsPrj.dll".ToLowerInvariant());
			_limnorFiles.Add("XmlSerializer.dll".ToLowerInvariant());
			_limnorFiles.Add("XmlUtility.dll".ToLowerInvariant());
			//
			_existReferences = new StringCollection();
			_existReferences.Add("mscorlib");
			_existReferences.Add("System");
			_existReferences.Add("System.Windows.Forms");
			_existReferences.Add("System.Data");
			_existReferences.Add("System.Deployment");
			_existReferences.Add("System.Xml");
			_existReferences.Add("System.Core");
			_existReferences.Add("System.Xml.Linq");
			_existReferences.Add("System.Data.DataSetExtensions");
			//
			_excludeNamespaces = new StringCollection();
			_excludeNamespaces.Add("Microsoft.Office.Interop.");

		}
		public static bool IsLimnorFile(string filename)
		{
			string name = System.IO.Path.GetFileName(filename).ToLowerInvariant();
			return _limnorFiles.Contains(name);
		}
		public static void AddSupportFile(string file)
		{
			foreach (string s in _assemblySupportFiles)
			{
				if (string.Compare(s, file, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			_assemblySupportFiles.Add(file);
		}
		public static StringCollection ExcludeNamespaces { get { return _excludeNamespaces; } }
		public static void ResetCopiedFiles()
		{
			_copiedFiles = new StringCollection();
		}
		public static bool FileNotCopied(string f)
		{
			f = f.ToLower(CultureInfo.InvariantCulture);
			if (_copiedFiles.Contains(f))
			{
				return false;
			}
			_copiedFiles.Add(f);
			return true;
		}
		public static string GetJsonJs()
		{
			return Resources1.json_min;
		}
		public static string GetModalJs()
		{
			return Resources1.modal_min;
		}
		private EnumRunContext _originRunContext = EnumRunContext.Server;
		public LimnorXmlCompiler(string projectfile, ICompilerLog log)
		{
			ProjectFile = projectfile;
			_project = new LimnorProject(projectfile);
			_originRunContext = VPLUtil.CurrentRunContext;
			if (_project.IsWebApplication)
			{
				VPLUtil.CurrentRunContext = EnumRunContext.Client;
			}
			else
			{
				VPLUtil.CurrentRunContext = EnumRunContext.Server;
			}
			try
			{
				Log = log;
				_languageNames = new string[1];
				_languageNames[0] = "fr-FR";
				_excludeFiles = new StringCollection();
				ResetCopiedFiles();
			}
			finally
			{
				VPLUtil.CurrentRunContext = _originRunContext;
			}
		}

		public bool UsePageNavigator { get; set; }
		public bool UseCopyProtection { get; set; }
		public bool UseDrawing { get; set; }
		public string BuildExpPath { get; set; }
		public string MSBuildAllProjects { get; set; }
		public string MSBuildBinPath { get; set; }
		public string MainFile { get; set; }
		public string OutputType { get; set; }
		public string MSBuildProjectDirectory { get; set; }
		public string RootNamespace { get; set; }
		public string CoreCompileDependsOn { get; set; }
		public string Configuration { get; set; }
		public string OutputPath { get; set; }
		public string PlatformTarget { get; set; }
		public string TargetFramework { get; set; }
		public string ApplicationIcon { get; set; }
		//
		public string ProjectFile { get; set; }
		//
		public string AssemblyTitle { get; set; }
		public string AssemblyDescription { get; set; }
		public string AssemblyCompany { get; set; }
		public string AssemblyProduct { get; set; }
		public string AssemblyCopyright { get; set; }
		public string AssemblyVersion { get; set; }
		public string AssemblyFileVersion { get; set; }

		public string AssemblyKeyFile { get; set; }
		public string AssemblyDelaySign { get; set; }
		//
		//
		public IList<ComponentID> RootClasses
		{
			get
			{
				List<ComponentID> lst = new List<ComponentID>();
				if (_classList != null)
				{
					SortedDictionary<UInt32, ComponentID>.ValueCollection.Enumerator vc = _classList.Values.GetEnumerator();
					while (vc.MoveNext())
					{
						lst.Add(vc.Current);
					}
				}
				return lst;
			}
		}
		public EnumWebServerProcessor WebServerTechnology
		{
			get
			{
				if (_project.ProjectType == EnumProjectType.WebAppPhp)
					return EnumWebServerProcessor.PHP;
				return EnumWebServerProcessor.Aspx;
			}
		}
		public string WebPhysicalFolder
		{
			get
			{
				return _project.WebPhysicalFolder(OutputForm);
			}
		}
		public string WebJavaScriptFolder
		{
			get
			{
				return _webJsFolder;
			}
		}
		public string WebPhpFolder
		{
			get
			{
				return Path.Combine(WebPhysicalFolder, "libphp");
			}
		}
		public int AjaxTimeout
		{
			get
			{
				return _ajaxTimeout;
			}
		}
		public bool WebDebugMode
		{
			get
			{
				return _webDebugMode;
			}
		}
		public int WebClientDebugLevel
		{
			get
			{
				return _webClientDebugLevel;
			}
		}
		public int SkippedClassCount
		{
			get
			{
				return _skippedClasses;
			}
		}
		public int CompiledClassCount
		{
			get
			{
				return _compiledClasses;
			}
		}
		public Dictionary<string, Assembly> AssemblyLocations
		{
			get
			{
				return _assemblyLocations;
			}
		}
		public void SetIncrement()
		{
			_increment = true;
		}
		public void LogError(string msg, params object[] values)
		{
			if (OutputWindow != null)
			{
				OutputWindow.AppendMessage(msg, values);
			}
			if (values != null && values.Length > 0)
			{
				msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, msg, values);
			}
			TraceLog.TraceLogClass.TraceLog.LogError(msg);
			Log.LogError(msg);
		}
		public string AppName
		{
			get
			{
				return _appName;
			}
		}
		public int SessionTimeoutMinutes
		{
			get
			{
				return _sessionTimeoutMinutes;
			}
		}
		public string OutputFullpath
		{
			get
			{
				return System.IO.Path.Combine(MSBuildProjectDirectory, OutputPath);
			}
		}
		public string PluginsFolder
		{
			get
			{
				return Path.Combine(OutputFullpath, "Plugins");
			}
		}
		public string WebJsFolder
		{
			get
			{
				_webJsFolder = Path.Combine(WebPhysicalFolder, "libjs");
				if (_project.IsWebApplication)
				{
					if (!System.IO.Directory.Exists(_webJsFolder))
					{
						System.IO.Directory.CreateDirectory(_webJsFolder);
					}
				}
				return _webJsFolder;
			}
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
		private string exeFilePathInObject
		{
			get
			{
				return System.IO.Path.Combine(objectFolder, string.Format(CultureInfo.InvariantCulture, "{0}.exe", _project.ProjectAssemblyName));
			}
		}
		private string exeFilePathInBin
		{
			get
			{
				return System.IO.Path.Combine(outputFolder, string.Format(CultureInfo.InvariantCulture, "{0}.exe", _project.ProjectAssemblyName));
			}
		}
		//
		private string objectFolder
		{
			get
			{
				string s = System.IO.Path.Combine(MSBuildProjectDirectory, "obj\\" + Configuration);
				if (!System.IO.Directory.Exists(s))
				{
					System.IO.Directory.CreateDirectory(s);
				}
				return s;
			}
		}
		private string outputFolder
		{
			get
			{
				string s = System.IO.Path.Combine(MSBuildProjectDirectory, OutputPath);
				if (!System.IO.Directory.Exists(s))
				{
					System.IO.Directory.CreateDirectory(s);
				}
				return s;
			}
		}
		private void copyLocalizedResources(string langaugeName)
		{
			string sDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, langaugeName);
			if (System.IO.Directory.Exists(sDir))
			{
				string tgtDir = System.IO.Path.Combine(outputFolder, langaugeName);
				bool b = false;
				string[] ss = System.IO.Directory.GetFiles(outputFolder, "*.dll");
				if (ss != null && ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						string file = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}.resources{1}",
							System.IO.Path.GetFileNameWithoutExtension(ss[i]), System.IO.Path.GetExtension(ss[i]));
						string lf = System.IO.Path.Combine(sDir, file);
						if (System.IO.File.Exists(lf))
						{
							if (!b)
							{
								if (!System.IO.Directory.Exists(tgtDir))
								{
									System.IO.Directory.CreateDirectory(tgtDir);
								}
								b = true;
							}
							System.IO.File.Copy(lf, System.IO.Path.Combine(tgtDir, file), true);
						}
					}
				}
			}
		}
		/// <summary>
		/// for adding references into csproj.
		/// exclude references already in the template.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		private bool isImportedAssembly(Assembly a)
		{
			string name = a.GetName().Name;
			return !_existReferences.Contains(name);
		}
		//
		public void ShowMessage(string message, params object[] values)
		{
			logMsg(message, values);
		}
		public void logMsg(string msg, params object[] values)
		{
			if (OutputWindow != null)
			{
				OutputWindow.AppendMessage(msg, values);
			}
			TraceLogClass.TraceLog.Trace(msg, values);
		}
		private bool copyFile(string f, string outputFolder)
		{
			bool bOK = true;
			string src = AppDomain.CurrentDomain.BaseDirectory;
			try
			{
				string sf = System.IO.Path.Combine(src, f);
				string tf = System.IO.Path.Combine(outputFolder, f);
				System.IO.File.Copy(sf, tf, true);
			}
			catch (Exception ef)
			{
				string msg = TraceLogClass.ExceptionMessage0(ef);
				TraceLogClass.TraceLog.Trace(msg);
				Log.LogError(msg);
				bOK = false;
			}
			return bOK;
		}
		private IDistributor loadDistributor()
		{
			IDistributor dis = null;
			if (disType == null)
			{
				string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "LimnorWix*.dll");
				if (files != null && files.Length > 0)
				{
					for (int i = 0; i < files.Length; i++)
					{
						try
						{
							Assembly a = Assembly.LoadFile(files[i]);
							if (a != null)
							{
								Type[] tps = a.GetExportedTypes();
								if (tps != null && tps.Length > 0)
								{
									for (int k = 0; k < tps.Length; k++)
									{
										if (!tps[k].IsInterface)
										{
											if (typeof(IDistributor).IsAssignableFrom(tps[k]))
											{
												dis = Activator.CreateInstance(tps[k]) as IDistributor;
												if (dis != null)
												{
													disType = tps[k];
													break;
												}
											}
										}
									}
									if (dis != null)
									{
										break;
									}
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			if (dis == null)
			{
				if (disType != null)
				{
					dis = Activator.CreateInstance(disType) as IDistributor;
				}
			}
			return dis;
		}
		private bool copySupportFiles(string outputFolder, StringCollection files)
		{
			bool bOK = true;
			string src = AppDomain.CurrentDomain.BaseDirectory;
			foreach (string f in files)
			{
				try
				{
					string fn = Path.GetFileName(f);
					string sf;
					if (string.CompareOrdinal(fn, f) == 0)
					{
						//no path
						sf = System.IO.Path.Combine(src, f);
					}
					else
					{
						//f includes path
						sf = f;
					}
					string tf = System.IO.Path.Combine(outputFolder, fn);
					System.IO.File.Copy(sf, tf, true);
				}
				catch (Exception ef)
				{
					string msg = TraceLogClass.ExceptionMessage0(ef);
					TraceLogClass.TraceLog.Trace(msg);
					Log.LogError(msg);
					bOK = false;
				}
			}
			return bOK;
		}
		private bool copyFiles(string outputFolder, StringCollection files)
		{
			bool bOK = true;
			foreach (string f in files)
			{
				try
				{
					string tf = System.IO.Path.Combine(outputFolder, Path.GetFileName(f));
					System.IO.File.Copy(f, tf, true);
				}
				catch (Exception ef)
				{
					string msg = TraceLogClass.ExceptionMessage0(ef);
					TraceLogClass.TraceLog.Trace(msg);
					Log.LogError(msg);
					bOK = false;
				}
			}
			return bOK;
		}
		private void addUsedAssembly(Assembly a)
		{
			string s = a.Location.ToLowerInvariant();
			if (!_usedAssemblies.Contains(s))
			{
				_usedAssemblies.Add(s);
			}
		}
		private void createLicenseResource(string targetFile,string dllFile, string[] licFiles)
		{
			//generate one license resource and add it to _embededFiles
			Process p = new Process();
			//
			ProcessStartInfo psI = new ProcessStartInfo("cmd");
			psI.UseShellExecute = false;
			psI.RedirectStandardInput = false;
			psI.RedirectStandardOutput = true;
			psI.RedirectStandardError = true;
			psI.CreateNoWindow = true;
			p.StartInfo = psI;
			//
			if (_project.TargetPlatform == AssemblyTargetPlatform.X86)
			{
				p.StartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "lc.exe");
			}
			else
			{
				p.StartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "X64\\lc.exe");
			}
			if (!System.IO.File.Exists(p.StartInfo.FileName))
			{
				throw new DesignerException("File not found:{0}", p.StartInfo.FileName);
			}
			CompilerFolders clfolders = new CompilerFolders(MSBuildProjectDirectory);
			string licResFile =Path.Combine(clfolders.ObjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}.licenses", Path.GetFileName(targetFile)));
			string stdout;
			string errout;
			StringBuilder sb = new StringBuilder();
			sb.Append(" /target:\"");
			sb.Append(Path.GetFileName(targetFile));
			sb.Append("\" /i:\"");
			sb.Append(dllFile);
			sb.Append("\" /outdir:\"");
			sb.Append(clfolders.ObjectFolder);
			sb.Append("\"");
			for (int i = 0; i < licFiles.Length; i++)
			{
				sb.Append(" /complist:\"");
				sb.Append(licFiles[i]);
				sb.Append("\"");
			}
			p.StartInfo.Arguments = sb.ToString();
			logMsg("generating license by [{0}] [{1}]", p.StartInfo.FileName, p.StartInfo.Arguments);
			p.Start();
			stdout = p.StandardOutput.ReadToEnd();
			errout = p.StandardError.ReadToEnd();
			p.WaitForExit();
			if (p.ExitCode != 0)
			{
				LogError("Error code {0}, output:{1}, error:{2} for calling {3} {4}", p.ExitCode, stdout, errout, p.StartInfo.FileName, p.StartInfo.Arguments);
			}
			else
			{
				if (!File.Exists(licResFile))
				{
					LogError("License file not created: {0}", licResFile);
				}
				else
				{
					_embededFiles.Add(licResFile);
				}
			}
		}
		private void generateLicenseResources(string targetFile)
		{
			//generate license resources and add to _embededFiles
			Dictionary<string, string[]> licList = _project.LicenseFiles;
			if (licList != null && licList.Count > 0)
			{
				Dictionary<string, string[]> files = new Dictionary<string, string[]>();
				foreach (KeyValuePair<string, string[]> kv in licList)
				{
					if (kv.Value != null && kv.Value.Length > 0)
					{
						if (!File.Exists(kv.Key))
						{
							throw new DesignerException("Library file [{0}] does not exist. Correct it by editing LicenseFiles property of the project", kv.Key);
						}
						List<string> fs = new List<string>();
						for (int i = 0; i < kv.Value.Length; i++)
						{
							if (!string.IsNullOrEmpty(kv.Value[i]))
							{
								if (!File.Exists(kv.Value[i]))
								{
									throw new DesignerException("License file [{0}] does not exist. Correct it by editing LicenseFiles property of the project", kv.Value[i]);
								}
								fs.Add(kv.Value[i]);
							}
						}
						if (fs.Count > 0)
						{
							files.Add(kv.Key, fs.ToArray());
						}
					}
				}
				foreach (KeyValuePair<string, string[]> kv in files)
				{
					createLicenseResource(targetFile, kv.Key, kv.Value);
				}
			}
		}
		/// <summary>
		/// generate binary files
		/// </summary>
		private void compile0()
		{
			try
			{
				//prepare compilation parameters
				CompilerParameters cp = new CompilerParameters();
				//Add System
				Assembly aly = Assembly.GetAssembly(typeof(System.ComponentModel.Component));
				addUsedAssembly(aly);
				aly = Assembly.GetAssembly(typeof(System.Linq.Expressions.Expression));
				addUsedAssembly(aly);
				foreach (string loc in _assemblyLocations.Keys)
				{
					if (!_usedAssemblies.Contains(loc))
					{
						_usedAssemblies.Add(loc);
					}
				}
				if (VPLUtil.IsCopyProtectorClient)
				{
					string clientDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CopyProtectionClient.dll");
					if (File.Exists(clientDll))
					{
						foreach (string s in _usedAssemblies)
						{
							string fn = Path.GetFileName(s);
							if (string.Compare(fn, "CopyProtection.dll", StringComparison.OrdinalIgnoreCase) == 0)
							{
								_usedAssemblies.Remove(s);
								break;
							}
						}
						_usedAssemblies.Add(clientDll);
						_copyProtectionDebug = false;
					}
					else
					{
						if (!LimnorStudioRuntimeConfig.CopyProtectionModuleWarningDisabled)
						{
							bool b = DialogOneTimeMessage.ShowMessage("Your licensing module is for debugging only. You may contact support@limnor.com and request a module for release.");
							if (b)
							{
								LimnorStudioRuntimeConfig.CopyProtectionModuleWarningDisabled = true;
							}
						}
						_copyProtectionDebug = true;
					}
				}
				foreach (string s in _usedAssemblies)
				{
					cp.ReferencedAssemblies.Add(s);
				}
				//
				if (_hasProjectResources)
				{
					ProjectResources rm = _project.GetProjectSingleData<ProjectResources>();
					CompilerFolders folders = new CompilerFolders(MSBuildProjectDirectory);
					_lstSourceFiles.Add(System.IO.Path.Combine(folders.SourceFolder, rm.SourceCodeFilename));
					_lstSourceFiles.Add(System.IO.Path.Combine(folders.SourceFolder, rm.HelpClassFilename));
					_lstResxFiles.Add(System.IO.Path.Combine(folders.SourceFolder, rm.ResourcesFilename));
					logMsg("Project resource source: {0}", rm.SourceCodeFilename);
					logMsg("Project resource help: {0}", rm.HelpClassFilename);
					logMsg("Project resource: {0}", rm.ResourcesFilename);
				}
				else
				{
					logMsg("Project resource: None");
				}
				
				//
				string binfile = string.Empty;
				StringBuilder sb = new StringBuilder();
				string plt = PlatformTarget;
				if (string.IsNullOrEmpty(plt) || string.Compare(plt, "AnyCPU", StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (string.IsNullOrEmpty(_use32bit))
					{
						_use32bit = use32bitAssembly();
					}
					if (!string.IsNullOrEmpty(_use32bit))
					{
						plt = "x86";
						logMsg("Warning: Compile to x86 because 32-bit software is used. {0}", _use32bit);
						this.Log.LogWarning("Warning: Compile to x86 because 32-bit software is used. {0}", _use32bit);
					}
				}
				if (!string.IsNullOrEmpty(plt))
				{
					sb.Append(" /platform:");
					sb.Append(plt);
					sb.Append(" ");
				}
				EnumCompileTarget compileTarget = EnumCompileTarget.ConsoleExe;
				switch (_project.ProjectType)
				{
					case EnumProjectType.ClassDLL:
						compileTarget = EnumCompileTarget.Dll;
						break;
					case EnumProjectType.Console:
						compileTarget = EnumCompileTarget.ConsoleExe;
						break;
					case EnumProjectType.Kiosk:
						compileTarget = EnumCompileTarget.FormExe;
						break;
					case EnumProjectType.ScreenSaver:
						compileTarget = EnumCompileTarget.FormExe;
						break;
					case EnumProjectType.Setup:
						compileTarget = EnumCompileTarget.None;
						break;
					case EnumProjectType.Unknown:
						compileTarget = EnumCompileTarget.None;
						break;
					case EnumProjectType.WebAppAspx:
					case EnumProjectType.WebService:
						compileTarget = EnumCompileTarget.Dll;
						break;
					case EnumProjectType.WinForm:
						compileTarget = EnumCompileTarget.FormExe;
						break;
					case EnumProjectType.WinService:
						compileTarget = EnumCompileTarget.ConsoleExe;
						break;
					case EnumProjectType.WpfApp:
						compileTarget = EnumCompileTarget.WpfExe;
						break;
					default:
						compileTarget = EnumCompileTarget.None;
						break;
				}
				_compileTarget = compileTarget;
				if (compileTarget == EnumCompileTarget.Dll)
				{
					sb.Append(" /t:library ");
					binfile = System.IO.Path.Combine(objectFolder, _project.ProjectAssemblyName + ".dll");
					cp.GenerateExecutable = false;
				}
				else if (compileTarget != EnumCompileTarget.None)
				{
					if (compileTarget == EnumCompileTarget.ConsoleExe)
					{
						sb.Append("/t:exe");
					}
					else
					{
						sb.Append("/t:winexe");
					}
					if (!string.IsNullOrEmpty(ApplicationIcon) && System.IO.File.Exists(ApplicationIcon))
					{
						if (string.Compare(System.IO.Path.GetExtension(ApplicationIcon), ".ico", StringComparison.OrdinalIgnoreCase) == 0)
						{
							sb.Append(" /win32icon:\"");
							sb.Append(ApplicationIcon);
							sb.Append("\" ");
						}
					}
					binfile = exeFilePathInObject;
					cp.GenerateExecutable = true;
				}
				generateLicenseResources(binfile);
				foreach (string s in _embededFiles)
				{
					logMsg("Embed resource: {0}", s);
					cp.EmbeddedResources.Add(s);
				}
				string sKeyFile = _project.AssemblyKeyFile;
				if (!string.IsNullOrEmpty(sKeyFile))
				{
					if (!File.Exists(sKeyFile))
					{
						string sKeyFile2 = Path.Combine(_project.ProjectFolder, Path.GetFileName(sKeyFile));
						if (File.Exists(sKeyFile2))
						{
							sKeyFile = sKeyFile2;
						}
					}
					sb.Append(" /keyfile:\"");
					sb.Append(sKeyFile);
					sb.Append("\" ");
					bool bDelaySign = _project.AssemblyDelaySign;
					if (bDelaySign)
					{
						sb.Append(" /delaysign- ");
					}
				}

				//RemotingHost and WcfServiceProxy need *.exe.config
				if (!string.IsNullOrEmpty(_appcfgFileInSourceFolder) && File.Exists(_appcfgFileInSourceFolder))
				{
					//_appcfgFileInSourceFolder is in sources folder, containing app configs used by all components
					string targetCfg = Path.Combine(Path.GetDirectoryName(_appcfgFileInSourceFolder), string.Format(CultureInfo.InvariantCulture, "{0}.config", Path.GetFileName(binfile)));
					//.Net 4.0 needs some settings in targetCfg for loading lower version DLLs
					if (File.Exists(targetCfg))
					{
						//merge configs
						XmlDocument targetDoc = new XmlDocument();
						targetDoc.Load(targetCfg);
						if (targetDoc.DocumentElement == null)
						{
							File.Move(_appcfgFileInSourceFolder, targetCfg);
						}
						else
						{
							XmlDocument objsDoc = new XmlDocument();
							objsDoc.Load(_appcfgFileInSourceFolder);
							if (objsDoc.DocumentElement != null)
							{
								//merge targetDoc into objsDoc
								for (int i = 0; i < targetDoc.DocumentElement.ChildNodes.Count; i++)
								{
									XmlNode srcNode = targetDoc.DocumentElement.ChildNodes[i];
									if (srcNode.NodeType == XmlNodeType.Element)
									{
										XmlNode tgtNode = objsDoc.DocumentElement.SelectSingleNode(srcNode.Name);
										if (tgtNode == null)
										{
											tgtNode = objsDoc.ImportNode(srcNode, true);
											objsDoc.DocumentElement.AppendChild(tgtNode);
										}
									}
								}
								objsDoc.Save(targetCfg);
							}
						}
					}
					else
					{
						File.Move(_appcfgFileInSourceFolder, targetCfg);
					}
				}
				cp.CompilerOptions = sb.ToString();
				cp.OutputAssembly = binfile;
				_targetFileName = Path.GetFileName(binfile);
				foreach (string s in _lstResxFiles)
				{
					cp.EmbeddedResources.Add(s);
				}
				cp.GenerateInMemory = false;
				cp.IncludeDebugInformation = true;
				//
				string[] sourceFiles = (string[])_lstSourceFiles.ToArray(typeof(string));
				//
				//use C# code provider
#if DOTNET40
                CSharpCodeProvider ccp;
#if TESTTARGETFRAMEWORK
                //TargetFramework 
                //CSharpCodeProvider ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                if (_project.AssemblyTargetFramework == AssemblyTargetFramework.V35)
                    ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                else
                    ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
#else
                ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
#endif
#else
				CSharpCodeProvider ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
#endif
				CompilerResults cr = ccp.CompileAssemblyFromFile(cp, sourceFiles);
				//
				if (cr.Errors.HasErrors)
				{
					string msg = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Error compiling project {0}", _project.ProjectFile);
					LogError(msg);
					foreach (CompilerError error in cr.Errors)
					{
						LogError(error.ToString());
					}
				}
				//copy to output folder
				string target = null;
				if (compileTarget != EnumCompileTarget.None)
				{
					string name = System.IO.Path.GetFileName(binfile);
					target = System.IO.Path.Combine(this.outputFolder, name);
					try
					{
						System.IO.File.Copy(binfile, target, true);
					}
					catch (Exception err)
					{
						LogError("Error copying compiled program. \r\nIf the last debug process has not fully unloaded then please wait a few seconds and try it again. \r\n{0}", err);
						CopyBinFileFailed = true;
					}
					if (VPLUtil.IsCompilingToDebug)
					{
						string src = System.IO.Path.GetFileNameWithoutExtension(binfile) + ".pdb";
						string srcFile = System.IO.Path.Combine(objectFolder, src);
						if (File.Exists(srcFile))
						{
							System.IO.File.Copy(srcFile, System.IO.Path.Combine(outputFolder, src), true);
						}
					}
				}
				//
				//generate satellite assemblies
				if (_hasProjectResources)
				{
					string stdout;
					string errout;
					ProjectResources rm = _project.GetProjectSingleData<ProjectResources>();
					CompilerFolders folders = new CompilerFolders(MSBuildProjectDirectory);
					IList<string> languages = rm.Languages;
					foreach (string languageName in languages)
					{
						string localFolder = System.IO.Path.Combine(OutputFullpath, languageName);
						if (!System.IO.Directory.Exists(localFolder))
						{
							System.IO.Directory.CreateDirectory(localFolder);
						}
						if (_project.ProjectType != EnumProjectType.WebAppPhp)
						{
							Process p = new Process();
							ProcessStartInfo psI = new ProcessStartInfo("cmd");
							psI.UseShellExecute = false;
							psI.RedirectStandardInput = false;
							psI.RedirectStandardOutput = true;
							psI.RedirectStandardError = true;
							psI.CreateNoWindow = true;
							p.StartInfo = psI;

							p.StartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "al.exe");
							if (!System.IO.File.Exists(p.StartInfo.FileName))
							{
								throw new DesignerException("File not found:{0}", p.StartInfo.FileName);
							}
							if (string.IsNullOrEmpty(plt))
							{
								plt = "AnyCPU";
							}
							p.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"/t:lib /platform:{3} /embed:\"{0}\" /culture:{1} /out:\"{2}\" /template:\"{4}\"",
								System.IO.Path.Combine(folders.SourceFolder, rm.FormBinResourcesFilename(languageName)), languageName, System.IO.Path.Combine(localFolder, rm.ResourcesDllFilename), plt, binfile);
							logMsg("generating project localized resoureces by [{0}] [{1}] for {2}", p.StartInfo.FileName, p.StartInfo.Arguments, languageName);
							p.Start();
							stdout = p.StandardOutput.ReadToEnd();
							errout = p.StandardError.ReadToEnd();
							p.WaitForExit();
							if (p.ExitCode != 0)
							{
								LogError("Error code {0}, output:{1}, error:{2} for calling {3} {4}", p.ExitCode, stdout, errout, p.StartInfo.FileName, p.StartInfo.Arguments);
							}
						}
					}
				}
				if (_project.ProjectType == EnumProjectType.Kiosk)
				{
					//add application manifest 
					if (File.Exists(target))
					{
						string stdout;
						string errout;
						string appmanifest = Path.Combine(OutputFullpath, string.Format(CultureInfo.InvariantCulture, "{0}.manifest", Path.GetFileName(target)));
						StreamWriter sw = new StreamWriter(appmanifest, false, Encoding.ASCII);
						sw.Write(Resources1.appmanifest);
						sw.Flush();
						sw.Close();
						Application.DoEvents();
						Process p = new Process();
						ProcessStartInfo psI = new ProcessStartInfo("cmd");
						psI.UseShellExecute = false;
						psI.RedirectStandardInput = false;
						psI.RedirectStandardOutput = true;
						psI.RedirectStandardError = true;
						psI.CreateNoWindow = true;
						p.StartInfo = psI;
						//
						p.StartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "mt.exe");
						if (!System.IO.File.Exists(p.StartInfo.FileName))
						{
							throw new DesignerException("File not found:{0}", p.StartInfo.FileName);
						}
						p.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"-manifest \"{0}\" -outputresource:\"{1}\";#1",
							appmanifest, target);
						logMsg("adding manifest using [{0}] [{1}] ...", p.StartInfo.FileName, p.StartInfo.Arguments);
						p.Start();
						stdout = p.StandardOutput.ReadToEnd();
						errout = p.StandardError.ReadToEnd();
						p.WaitForExit();
						if (p.ExitCode != 0)
						{
							LogError("Error code {0}, output:{1}, error:{2} for calling {3} {4}", p.ExitCode, stdout, errout, p.StartInfo.FileName, p.StartInfo.Arguments);
						}
					}
				}
			}
			catch (Exception er)
			{
				string msg = TraceLogClass.ExceptionMessage0(er);
				LogError(msg);
			}
		}
		private void addRef(Assembly a)
		{
			string loc = a.Location.ToLowerInvariant();
			if (!_assemblyLocations.ContainsKey(loc))
				_assemblyLocations.Add(loc, a);
		}
		private string use32bitAssembly()
		{
			if (_assemblyLocations != null && _assemblyLocations.Count > 0)
			{
				string osFolder = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System));
				foreach (Assembly a in _assemblyLocations.Values)
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
		private bool generateResources(ProjectResources rm, CompilerFolders folders, string languageName, List<IDistributeFile> distrubtedFiles)
		{
			if (_project.ProjectType != EnumProjectType.WebAppPhp)
			{
				string resFilename = System.IO.Path.Combine(folders.SourceFolder, rm.FormResourcesFilename(languageName));
				ResourceService resService = new ResourceService(resFilename);
				IResourceWriter resWriter = resService.GetResourceWriter(System.Globalization.CultureInfo.InvariantCulture);

				bool ret = rm.WriteResources(resWriter, languageName, distrubtedFiles);
				resWriter.Generate();
				resWriter.Close();
				//
				string sCurDir = Environment.CurrentDirectory;
				Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Process p = new Process();
				ProcessStartInfo psI = new ProcessStartInfo("cmd");
				psI.UseShellExecute = false;
				psI.RedirectStandardInput = false;
				psI.RedirectStandardOutput = true;
				psI.RedirectStandardError = true;
				psI.CreateNoWindow = true;
				psI.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
				p.StartInfo = psI;

				p.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resgen.exe");
				if (!System.IO.File.Exists(p.StartInfo.FileName))
				{
					throw new DesignerException("File not found:{0}", p.StartInfo.FileName);
				}
				string resFilePath = Path.Combine(SourceCodeFolder, rm.SourceCodeFilename);
				if (string.IsNullOrEmpty(languageName))
				{
					p.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"\"{0}\"  /str:c#,{3},{1},\"{2}\"",
						resFilename, rm.ClassName, resFilePath, _project.Namespace);
				}
				else
				{
					p.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"\"{0}\"",
						resFilename);
				}
				logMsg("generating project resoureces by [{0}] [{1}]", p.StartInfo.FileName, p.StartInfo.Arguments);
				p.Start();
				string stdout = p.StandardOutput.ReadToEnd();
				string errout = p.StandardError.ReadToEnd();
				p.WaitForExit();
				if (p.ExitCode != 0)
				{
					logMsg("Error code {0}, output:{1}, error:{2} for calling {3} {4}", p.ExitCode, stdout, errout, p.StartInfo.FileName, p.StartInfo.Arguments);
				}
				Environment.CurrentDirectory = sCurDir;
				return ret;
			}
			return false;
		}
		private bool compileResources()
		{
			bool bUseFilename = false;
			if (System.IO.File.Exists(_project.ResourcesFile))
			{
				ProjectResources rm = _project.GetProjectSingleData<ProjectResources>();
				if (rm.HasResources)
				{
					//find files in setup project
					List<IDistributeFile> distrubtedFiles = new List<IDistributeFile>();
					if (LimnorSolution.Solution.Count > 0)
					{
						List<LimnorProject> prjs = new List<LimnorProject>();
						foreach (LimnorProject p in LimnorSolution.Solution)
						{
							prjs.Add(p);
						}
						foreach (LimnorProject p in prjs)
						{
							if (p.ProjectType == EnumProjectType.Setup)
							{
								string[] cfs = p.GetComponentFiles();
								if (cfs != null && cfs.Length > 0)
								{
									for (int i = 0; i < cfs.Length; i++)
									{
										IDistributor dis = loadDistributor();
										if (dis != null)
										{
											dis.CollectAddedFiles(cfs[i], distrubtedFiles);
										}
									}
								}
							}
						}
					}
					_hasProjectResources = true;
					CompilerFolders folders = new CompilerFolders(MSBuildProjectDirectory);
					//
					bUseFilename = generateResources(rm, folders, string.Empty, distrubtedFiles);
					//
					IList<string> languageName = rm.Languages;
					foreach (string name in languageName)
					{
						if (generateResources(rm, folders, name, distrubtedFiles))
						{
							bUseFilename = true;
						}
					}
					if (_project.ProjectType != EnumProjectType.WebAppPhp)
					{
						//
						//create help class
						//
						string helpFile = System.IO.Path.Combine(folders.SourceFolder, rm.HelpClassFilename);
						StreamWriter sw = new StreamWriter(helpFile, false, Encoding.ASCII);
						sw.Write(rm.CreateHelpClass());
						sw.Close();
						//
					}
				}
			}
			return bUseFilename;
		}
		/// <summary>
		/// set kiosk app BackGroundType property 
		/// </summary>
		/// <param name="docs"></param>
		private void setKioskBackground(XmlDocument[] docs)
		{
			XmlNode kioskAppNode = null;
			XmlNode backgroundNode = null;
			for (int i = 0; i < docs.Length; i++)
			{
				Type t = XmlUtil.GetLibTypeAttribute(docs[i].DocumentElement);
				if (kioskAppNode == null && typeof(LimnorKioskApp).Equals(t))
				{
					kioskAppNode = docs[i].DocumentElement;
				}
				if (backgroundNode == null && typeof(FormKioskBackground).Equals(t))
				{
					backgroundNode = docs[i].DocumentElement;
				}
				if (backgroundNode != null && kioskAppNode != null)
				{
					XmlNode propNode = kioskAppNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}[@{1}='BackGroundType']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
					if (propNode == null)
					{
						propNode = kioskAppNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
						kioskAppNode.AppendChild(propNode);
					}
					propNode.InnerText = XmlUtil.GetNameAttribute(backgroundNode);
					break;
				}
			}
		}
		private void loadConfigs()
		{
			dbConfigFiles = new StringCollection();
			if (databaseConnectionIDs.Count > 0)
			{
				foreach (Guid g in databaseConnectionIDs)
				{
					string f = ConnectionItem.GetConnectionFilename(g);
					if (System.IO.File.Exists(f))
					{
						dbConfigFiles.Add(f);
					}
				}
			}
		}
		private void addAssemblies0(bool bUseFilenames)
		{
			addRef(Assembly.GetAssembly(typeof(System.Xml.XmlDocument)));
			if (_project.ProjectType == EnumProjectType.Kiosk)
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(FormKioskBackground));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(Color));
			}
			if (LimnorProject.IsWinformApp(_project.ProjectType))
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(ThreadExceptionEventArgs));
			}
			if (_project.ProjectType == EnumProjectType.ScreenSaver)
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(ScreenSaverBackgroundForm));
			}
			if (bUseFilenames)
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(Filepath));
			}
			//
			if (_debug)
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(LimnorDesigner.LimnorDebugger));
			}
			if (LimnorProject.IsWinformApp(_project.ProjectType))
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(Form));
			}
			if (UseCopyProtection)
			{
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(VPLUtil));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(WinUtil));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(MathNode));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(DrawingItem));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(WindowControl));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(EasyDataSet));
				DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(WindowsManager));
			}
			DesignUtil.FindReferenceLocations(_assemblyLocations, typeof(CodeDomHelper));
		}
		private void addAssemblySupportFiles()
		{
			foreach (Assembly a in _assemblyLocations.Values)
			{
				object[] objs = a.GetCustomAttributes(typeof(SupportFileAttribute), true);
				if (objs != null && objs.Length > 0)
				{
					SupportFileAttribute fs = objs[0] as SupportFileAttribute;
					foreach (string f in fs.Filenames)
					{
						string f2 = f.ToLowerInvariant();
						if (!_assemblySupportFiles.Contains(f2))
						{
							_assemblySupportFiles.Add(f2);
						}
					}
				}
			}
		}
		private bool copyDebugFiles(StringCollection errors)
		{
			bool bOK = true;
			StringCollection afiles = new StringCollection();
			Type t = typeof(FormDebugger);
			if (!DesignUtil.CopyAssembly(afiles, t, OutputFullpath, errors))
			{
				bOK = false;
			}
			return bOK;
		}
		private void addCopyProtection()
		{
			bool b = false;
			bool b1 = false;
			bool b2 = false;
			bool b3 = false;
			bool b4 = false;
			string appLic;
			if (!VPLUtil.IsCopyProtectorClient || _copyProtectionDebug)
			{
				appLic = appRegExeDebug;
			}
			else
			{
				appLic = appRegExe;
			}
			StringCollection deletes = new StringCollection();
			foreach (string sf in _assemblySupportFiles)
			{
				if (string.Compare(appRegExe, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (!VPLUtil.IsCopyProtectorClient || _copyProtectionDebug)
					{
						deletes.Add(sf);
					}
				}
				if (string.Compare(appRegExeDebug, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (!_copyProtectionDebug)
					{
						deletes.Add(sf);
					}
				}
				if (string.Compare(appLic, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					b = true;
				}
				else if (string.Compare(appRegDll1, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					b1 = true;
				}
				else if (string.Compare(appRegDll2, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					b2 = true;
				}
				else if (string.Compare(appRegDll3, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					b3 = true;
				}
				else if (string.Compare(appRegDll4, sf, StringComparison.OrdinalIgnoreCase) == 0)
				{
					b4 = true;
				}
			}
			foreach (string sf in deletes)
			{
				_assemblySupportFiles.Remove(sf);
			}
			if (!b)
			{
				_assemblySupportFiles.Add(appLic);
			}
			if (!b1)
			{
				_assemblySupportFiles.Add(appRegDll1);
			}
			if (!b2)
			{
				_assemblySupportFiles.Add(appRegDll2);
			}
			if (!b3)
			{
				_assemblySupportFiles.Add(appRegDll3);
			}
			if (!b4)
			{
				_assemblySupportFiles.Add(appRegDll4);
			}
		}
		private bool copyAssemblySupportFiles()
		{
			bool bOK = true;
			if (_assemblySupportFiles.Count > 0)
			{
				if (!copySupportFiles(OutputFullpath, _assemblySupportFiles))
				{
					bOK = false;
				}
			}
			if (dbConfigFiles.Count > 0)
			{
				if (!copyFiles(OutputFullpath, dbConfigFiles))
				{
					bOK = false;
				}
			}
			return bOK;
		}
		private bool copyAspxFiles(StringCollection errors)
		{
			bool bOK = true;
			string webBin = Path.Combine(WebPhysicalFolder, "bin");
			if (!Directory.Exists(webBin))
			{
				try
				{
					Directory.CreateDirectory(webBin);
				}
				catch (Exception err)
				{
					errors.Add(err.Message);
					bOK = false;
				}
			}
			if (Directory.Exists(webBin))
			{
				webTargetFolder = webBin;
				string[] wfiles = Directory.GetFiles(OutputFullpath);
				if (wfiles != null && wfiles.Length > 0)
				{
					for (int i = 0; i < wfiles.Length; i++)
					{
						string targetWebFile = Path.Combine(webBin, Path.GetFileName(wfiles[i]));
						try
						{
							File.Copy(wfiles[i], targetWebFile, true);
						}
						catch (Exception err)
						{
							errors.Add(err.Message);
							bOK = false;
						}
					}
				}
			}
			return bOK;
		}
		private void loadAppParameters(string classFile)
		{
			ApplicationParameters appObj = new ApplicationParameters();
			appObj.GetParameters(ProjectFile, classFile);
			_appCodeName = appObj.AppCodeName;
			_sessionVarNames = appObj.SessionVarNames;
			_sessionVarValues = appObj.SessionVarValues;
			_sessionTimeoutMinutes = appObj.SessionTimeoutMinutes;
			_sessionVarCount = appObj.SessionVarCount;
		}
		private void addClassName(string file)
		{
			UInt32 order;
			XmlDocument doc = new XmlDocument();
			doc.Load(file);
			UInt32 classId = XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID);
			string name = XmlUtil.GetNameAttribute(doc.DocumentElement);
			try
			{
				Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement);
				order = classId;
				if (typeof(FormKioskBackground).Equals(t))
				{
					_kioskBackgroundName = LimnorKioskApp.formBKName;
				}
				else if (t.IsSubclassOf(typeof(LimnorApp)))
				{
					order = 0;
					_appName = name;
					loadAppParameters(file);
					if (typeof(LimnorWebApp).IsAssignableFrom(t))
					{
						XmlNode node = doc.DocumentElement.SelectSingleNode("Property[@name='DebugMode']");
						if (node != null)
						{
							_webDebugMode = Convert.ToBoolean(node.InnerText);
						}
						node = doc.DocumentElement.SelectSingleNode("Property[@name='ClientDebugLevel']");
						if (node != null)
						{
							_webClientDebugLevel = Convert.ToInt32(node.InnerText);
						}
						node = doc.DocumentElement.SelectSingleNode("Property[@name='AjaxTimeout']");
						if (node != null)
						{
							_ajaxTimeout = Convert.ToInt32(node.InnerText);
						}
						else
						{
							_ajaxTimeout = 0;
						}
						_project.VerifyWebSite(OutputForm);
						StreamWriter sw;
						string pdir = _project.WebPhysicalFolder(OutputForm);
						if (!System.IO.Directory.Exists(pdir))
						{
							System.IO.Directory.CreateDirectory(pdir);
						}
						sw = new StreamWriter(Path.Combine(WebJsFolder, "json2.js"), false, Encoding.ASCII);
#if DEBUG
						sw.Write(Resources1.json2);
						string webAppDir = string.Format(CultureInfo.InvariantCulture, "{0}\\..\\..\\..\\LimnorCompiler\\WebApp", Path.GetDirectoryName(Application.ExecutablePath));
						string jsdbSrc = Path.Combine(webAppDir, "jsonDataBind.js");
						string jsdbTgt = Path.Combine(_webJsFolder, "jsonDataBind.js");
						File.Copy(jsdbSrc, jsdbTgt, true);
#else
					sw.Write(Resources1.json_min);
#endif
						sw.Close();
						sw = new StreamWriter(Path.Combine(WebJsFolder, "modal.js"), false, Encoding.ASCII);
#if DEBUG
						sw.Write(Resources1.modal);
#else
					sw.Write(Resources1.modal_min);
#endif
						sw.Close();
						if (_project.ProjectType == EnumProjectType.WebAppPhp)
						{
							string _webPhpFolder = WebPhpFolder;
							if (!System.IO.Directory.Exists(_webPhpFolder))
							{
								System.IO.Directory.CreateDirectory(_webPhpFolder);
							}
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "dataSourceInterface.php"), false, Encoding.ASCII);
							sw.Write(Resources1.dataSourceInterface_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "FastJSON.class.php"), false, Encoding.ASCII);
							sw.Write(Resources1.FastJSON_class_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "jsonDataBind.php"), false, Encoding.ASCII);
							sw.Write(Resources1.jsonDataBind_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "JsonProcessPage.php"), false, Encoding.ASCII);
							sw.Write(Resources1.JsonProcessPage_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "jsonSource_mySqlI.php"), false, Encoding.ASCII);
							sw.Write(Resources1.jsonSource_mySqlI_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "sqlClient.php"), false, Encoding.ASCII);
							sw.Write(Resources1.sqlClient_php);
							sw.Close();
							sw = new StreamWriter(Path.Combine(_webPhpFolder, "WebApp.php"), false, Encoding.ASCII);
							sw.Write(Resources1.WebApp_php);
							sw.Close();
							//
						}
					}
				}
				bool needCompile = true;
				ComponentID c = new ComponentID(_project, classId, name, t, file);
				if (_increment && _modifiedPages != null)
				{
					if (c.ComponentType != null && c.ComponentType.GetInterface("IWebPage") != null)
					{
						needCompile = _modifiedPages.Contains(classId);
						if (!needCompile)
						{
							string wf = string.Format(CultureInfo.InvariantCulture, "{0}.html", name);//Path.GetFileNameWithoutExtension(c.ComponentFile));
							_webPages.Add(classId, wf);
							logMsg("Skip compiling web page {0}, file:{1}", wf, c.ComponentFile);
							_skippedClasses++;
						}
					}
				}
				if (needCompile)
				{
					_compiledClasses++;
					_classList.Add(order, c);
				}
			}
			catch (Exception e)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Error calling addClassName({0})", file), e);
			}
		}
		private void initCompile(string[] files)
		{
			if (_project.ProjectType == EnumProjectType.WebAppPhp)
			{
				shouldCompileBinary = false;
			}
			_usedatetimepicker = false;
			appConsumers = new List<IAppConfigConsumer>();
			databaseConnectionIDs = new List<Guid>();
			_webPages = new Dictionary<uint, string>();
			_classList = new SortedDictionary<UInt32, ComponentID>();
			_componentCompileResults = new List<ComponentCompileResult>();
			for (int i = 0; i < files.Length; i++)
			{
				addClassName(files[i]);
			}
		}
		private void createHomePage0()
		{
			string startFile = null;
			if (_startWebPageId != 0)
			{
				_webPages.TryGetValue(_startWebPageId, out startFile);
			}
			if (string.IsNullOrEmpty(startFile))
			{
				if (_webPages.Count > 0)
				{
					Dictionary<UInt32, string>.ValueCollection.Enumerator e = _webPages.Values.GetEnumerator();
					if (e.MoveNext())
					{
						startFile = e.Current;
					}
				}
			}
			if (string.IsNullOrEmpty(startFile))
			{
				_project.StartFile = string.Empty;
			}
			else
			{
				_project.StartFile = Path.Combine(_project.WebPhysicalFolder(OutputForm), startFile);
			}
		}
		private void checkUnusedDllFiles()
		{
			if (_compileTarget == EnumCompileTarget.Dll)
			{
				StringCollection fs = new StringCollection();
				StringCollection outfiles = new StringCollection();
				outfiles.Add(_targetFileName.ToLowerInvariant());
				foreach (KeyValuePair<string, Assembly> kv in _assemblyLocations)
				{
					if (!kv.Value.GlobalAssemblyCache)
					{
						outfiles.Add(Path.GetFileName(kv.Key).ToLowerInvariant());
					}
				}
				foreach (string s in _usedAssemblies)
				{
					outfiles.Add(Path.GetFileName(s).ToLowerInvariant());
				}
				foreach (string s in _assemblySupportFiles)
				{
					outfiles.Add(Path.GetFileName(s).ToLowerInvariant());
				}
				string[] dlls = Directory.GetFiles(this.outputFolder, "*.dll");
				if (dlls != null && dlls.Length > 0)
				{
					for (int i = 0; i < dlls.Length; i++)
					{
						string s = Path.GetFileName(dlls[i]).ToLowerInvariant();
						if (!outfiles.Contains(s))
						{
							try
							{
								Assembly a = Assembly.ReflectionOnlyLoadFrom(dlls[i]);
								if (a != null)
								{
									fs.Add(dlls[i]);
								}
							}
							catch
							{
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(webTargetFolder))
				{
					dlls = Directory.GetFiles(webTargetFolder, "*.dll");
					if (dlls != null && dlls.Length > 0)
					{
						for (int i = 0; i < dlls.Length; i++)
						{
							string s = Path.GetFileName(dlls[i]).ToLowerInvariant();
							if (!outfiles.Contains(s))
							{
								try
								{
									Assembly a = Assembly.ReflectionOnlyLoadFrom(dlls[i]);
									if (a != null)
									{
										fs.Add(dlls[i]);
									}
								}
								catch
								{
								}
							}
						}
					}
				}
				if (fs.Count > 0)
				{
					if (_project.ProjectType == EnumProjectType.WebAppAspx)
					{
						string cpDll = Path.Combine(this.webTargetFolder, "LicenseRequestHandlerPlugin.dll");
						if (StringUtility.StringCollectionContainsI(fs, cpDll))
						{
							if (fs.Count == 1)
							{
								fs.Clear();
							}
							else
							{
								StringCollection fs0 = new StringCollection();
								StringCollection copyProtects = new StringCollection();
								copyProtects.Add(cpDll);
								//find dependent DLLs
								for (int i = 0; i < fs.Count; i++)
								{
								}
								for (int i = 0; i < fs.Count; i++)
								{
									if (!StringUtility.StringCollectionContainsI(copyProtects, fs[i]))
									{
										fs0.Add(fs[i]);
									}
								}
								fs = fs0;
							}
						}
					}
				}
				if (fs.Count > 0)
				{
					logMsg("Unused files in output folders:{0}", fs.Count);
					for (int i = 0; i < fs.Count; i++)
					{
						logMsg("Unused file:{0}", fs[i]);
					}
					DlgUnusedFiles.ShowList(fs, null);
				}
			}
		}
		public string SourceCodeFolder
		{
			get
			{
				return System.IO.Path.Combine(MSBuildProjectDirectory, SOURCE);
			}
		}
		private bool createCsProject(StringCollection afiles, StringCollection errors)
		{
			bool bOK = true;
			//load csproj template
			XmlDocument docPrj = new XmlDocument();
			if (_project.ProjectType == EnumProjectType.Kiosk)
			{
				if (_debug)
				{
					docPrj.LoadXml(Resources1.KioskAppD);
				}
				else
				{
					docPrj.LoadXml(Resources1.KioskApp);
				}
			}
			else if (_project.ProjectType == EnumProjectType.Console)
			{
				if (_debug)
				{
					docPrj.LoadXml(Resources1.ConsoleAppD);
				}
				else
				{
					docPrj.LoadXml(Resources1.ConsoleApp);
				}
			}
			else if (_project.ProjectType == EnumProjectType.ClassDLL)
			{
				if (_debug)
				{
					docPrj.LoadXml(Resources1.ClassLibD);
				}
				else
				{
					docPrj.LoadXml(Resources1.ClassLib);
				}
			}
			else
			{
				if (_debug)
				{
					docPrj.LoadXml(Resources1.WinAppD);
				}
				else
				{
					docPrj.LoadXml(Resources1.WinApp);
				}
			}
			XmlNode itemGroup = docPrj.CreateElement("ItemGroup", msBuildUri);
			docPrj.DocumentElement.AppendChild(itemGroup);

			//
			XmlNamespaceManager xm = new XmlNamespaceManager(docPrj.NameTable);
			xm.AddNamespace("x", msBuildUri);
			//
			XmlNode gNode = docPrj.DocumentElement.SelectSingleNode("x:PropertyGroup", xm);
			if (gNode != null)
			{
				XmlNode nameNode = gNode.SelectSingleNode("x:RootNamespace", xm);
				if (nameNode != null)
				{
					nameNode.InnerText = RootNamespace;
				}
				nameNode = gNode.SelectSingleNode("x:AssemblyName", xm);
				if (nameNode != null)
				{
					nameNode.InnerText = _project.ProjectAssemblyName;
				}
			}
			//
			if (string.CompareOrdinal(OutputType, "WinExe") == 0 || string.CompareOrdinal(OutputType, "Exe") == 0)
			{
				if (!string.IsNullOrEmpty(ApplicationIcon) && System.IO.File.Exists(ApplicationIcon))
				{
					string iconfile = System.IO.Path.GetFileName(ApplicationIcon);
					if (string.Compare(System.IO.Path.GetExtension(iconfile), ".ico", StringComparison.OrdinalIgnoreCase) == 0)
					{
						gNode = docPrj.DocumentElement.SelectSingleNode("x:PropertyGroup", xm);
						if (gNode != null)
						{
							XmlNode iconNode = gNode.SelectSingleNode("x:ApplicationIcon", xm);
							if (iconNode == null)
							{
								iconNode = docPrj.CreateElement("ApplicationIcon", msBuildUri);
								gNode.AppendChild(iconNode);
							}
							iconNode.InnerText = iconfile;
						}
					}
				}
			}
			//
			string sourceFolder = null;
			foreach (ComponentCompileResult cc in _componentCompileResults)
			{
				//add source file
				XmlNode compileNode = docPrj.CreateElement("Compile", msBuildUri);
				itemGroup.AppendChild(compileNode);
				XmlUtil.SetAttribute(compileNode, "Include", System.IO.Path.GetFileName(cc.SourceFile));
				if (cc.IsForm)
				{
					XmlNode subTypeNode = docPrj.CreateElement("SubType", msBuildUri);
					compileNode.AppendChild(subTypeNode);
					subTypeNode.InnerText = "Form";
				}
				//add resource file
				if (cc.UseResources)
				{
					XmlNode resNode = docPrj.CreateElement("EmbeddedResource", msBuildUri);
					itemGroup.AppendChild(resNode);
					XmlUtil.SetAttribute(resNode, "Include", System.IO.Path.GetFileName(cc.ResourceFile));
					XmlNode depNode = docPrj.CreateElement("DependentUpon", msBuildUri);
					resNode.AppendChild(depNode);
					depNode.InnerText = System.IO.Path.GetFileName(cc.SourceFile);
					XmlNode subTypeNode = docPrj.CreateElement("SubType", msBuildUri);
					resNode.AppendChild(subTypeNode);
					subTypeNode.InnerText = "Designer";
				}
				if (cc.UseResourcesX)
				{
					XmlNode resNode = docPrj.CreateElement("EmbeddedResource", msBuildUri);
					itemGroup.AppendChild(resNode);
					XmlUtil.SetAttribute(resNode, "Include", System.IO.Path.GetFileName(cc.ResourceFileX));
					XmlNode depNode = docPrj.CreateElement("Generator", msBuildUri);
					resNode.AppendChild(depNode);
					depNode.InnerText = "ResXFileCodeGenerator";
					XmlNode subTypeNode = docPrj.CreateElement("LastGenOutput", msBuildUri);
					resNode.AppendChild(subTypeNode);
					subTypeNode.InnerText = System.IO.Path.GetFileName(cc.SourceFileX);
					//
					resNode = docPrj.CreateElement("Compile", msBuildUri);
					itemGroup.AppendChild(resNode);
					XmlUtil.SetAttribute(resNode, "Include", System.IO.Path.GetFileName(cc.SourceFileX));
					depNode = docPrj.CreateElement("AutoGen", msBuildUri);
					resNode.AppendChild(depNode);
					depNode.InnerText = "True";
					depNode = docPrj.CreateElement("DesignTime", msBuildUri);
					resNode.AppendChild(depNode);
					depNode.InnerText = "True";
					subTypeNode = docPrj.CreateElement("DependentUpon", msBuildUri);
					resNode.AppendChild(subTypeNode);
					subTypeNode.InnerText = System.IO.Path.GetFileName(cc.ResourceFileX);
				}
				if (sourceFolder == null)
				{
					sourceFolder = System.IO.Path.GetDirectoryName(cc.SourceFile);
				}
				List<ResourceFile> embededFiles = cc.ResourceFiles;
				if (embededFiles != null)
				{
					foreach (ResourceFile f in embededFiles)
					{
						if (f.BuildAction == EnumResourceBuildType.Embed)
						{
							string s0 = System.IO.Path.Combine(sourceFolder, f.Name);
							System.IO.File.Copy(f.ResourceFilename, s0, true);
							XmlNode resNode = docPrj.CreateElement("EmbeddedResource", msBuildUri);
							itemGroup.AppendChild(resNode);
							XmlUtil.SetAttribute(resNode, "Include", f.Name);
						}
					}
				}
			}
			if (_hasProjectResources)
			{
				ProjectResources rm = _project.GetProjectSingleData<ProjectResources>();
				XmlNode resNode = docPrj.CreateElement("EmbeddedResource", msBuildUri);
				itemGroup.AppendChild(resNode);
				XmlUtil.SetAttribute(resNode, "Include", rm.FormResourcesFilename(string.Empty));
				XmlNode nd = docPrj.CreateElement("Generator", msBuildUri);
				resNode.AppendChild(nd);
				nd.InnerText = "ResXFileCodeGenerator";
				nd = docPrj.CreateElement("LastGenOutput", msBuildUri);
				resNode.AppendChild(nd);
				nd.InnerText = rm.SourceCodeFilename;
				//
				resNode = docPrj.CreateElement("Compile", msBuildUri);
				itemGroup.AppendChild(resNode);
				XmlUtil.SetAttribute(resNode, "Include", rm.HelpClassFilename);
				resNode = docPrj.CreateElement("Compile", msBuildUri);
				itemGroup.AppendChild(resNode);
				XmlUtil.SetAttribute(resNode, "Include", rm.SourceCodeFilename);
				nd = docPrj.CreateElement("AutoGen", msBuildUri);
				resNode.AppendChild(nd);
				nd.InnerText = "True";
				nd = docPrj.CreateElement("DesignTime", msBuildUri);
				resNode.AppendChild(nd);
				nd.InnerText = "True";
				nd = docPrj.CreateElement("DependentUpon", msBuildUri);
				resNode.AppendChild(nd);
				nd.InnerText = rm.FormResourcesFilename(string.Empty);
				//
				IList<string> languageNames = rm.Languages;
				foreach (string lname in languageNames)
				{
					resNode = docPrj.CreateElement("EmbeddedResource", msBuildUri);
					itemGroup.AppendChild(resNode);
					XmlUtil.SetAttribute(resNode, "Include", rm.FormResourcesFilename(lname));
				}
			}
			if (_project.ProjectType == EnumProjectType.Kiosk)
			{
				//copy CPP DLL files
				string sourceDebugFolder;
				int q = 0;
				while (q < 2)
				{
					if (q == 0)
					{
						sourceDebugFolder = System.IO.Path.Combine(sourceFolder, "bin\\Debug");
					}
					else
					{
						sourceDebugFolder = System.IO.Path.Combine(sourceFolder, "bin\\Release");
					}
					q++;
					if (!System.IO.Directory.Exists(sourceDebugFolder))
					{
						System.IO.Directory.CreateDirectory(sourceDebugFolder);
					}
				}
			}
			if (_assemblySupportFiles.Count > 0)
			{
				string sourceDebugFolder;
				int q = 0;
				while (q < 2)
				{
					if (q == 0)
					{
						if (VPLUtil.IsCompilingToDebug)
						{
							sourceDebugFolder = System.IO.Path.Combine(sourceFolder, "bin\\Debug");
						}
						else
						{
							q++;
							continue;
						}
					}
					else
					{
						sourceDebugFolder = System.IO.Path.Combine(sourceFolder, "bin\\Release");
					}
					q++;
					if (!System.IO.Directory.Exists(sourceDebugFolder))
					{
						System.IO.Directory.CreateDirectory(sourceDebugFolder);
					}
					if (!copySupportFiles(sourceDebugFolder, _assemblySupportFiles))
					{
						bOK = false;
					}
				}
			}

			//create references
			foreach (string loc in _usedAssemblies)
			{
				if (!_assemblyLocations.ContainsKey(loc))
				{
					VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(loc));
					Assembly a = Assembly.LoadFile(loc);
					_assemblyLocations.Add(loc, a);
				}
			}
			if (_assemblyLocations.Count > 0)
			{
				XmlNode ndRef = docPrj.DocumentElement.SelectSingleNode("x:ItemGroup/x:Reference[@Include='System']", xm);
				if (ndRef == null)
				{
					throw new DesignerException("System reference not found");
				}
				XmlNode ndItemGroup = ndRef.ParentNode;
				foreach (KeyValuePair<string, Assembly> kv in _assemblyLocations)
				{
					if (isImportedAssembly(kv.Value))
					{
						DesignUtil.CopyAssembly(afiles, kv.Value, OutputFullpath, errors);
						ndRef = docPrj.CreateElement("Reference", msBuildUri);
						ndItemGroup.AppendChild(ndRef);
						XmlUtil.SetAttribute(ndRef, "Include", kv.Value.FullName);
						XmlNode nd = docPrj.CreateElement("SpecificVersion", msBuildUri);
						ndRef.AppendChild(nd);
						nd.InnerText = "False";
						nd = docPrj.CreateElement("HintPath", msBuildUri);
						ndRef.AppendChild(nd);
						nd.InnerText = kv.Value.Location;
					}
				}
				//
				if (isWebService)
				{
					//add exception handling 
					if (afiles == null)
					{
						afiles = new StringCollection();
					}
					afiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebServerUtility.dll"));
					StringCollection error2 = new StringCollection();
					string sBin = Path.Combine(SourceCodeFolder, "bin");
					if (!System.IO.Directory.Exists(sBin))
					{
						try
						{
							System.IO.Directory.CreateDirectory(sBin);
						}
						catch (Exception er)
						{
							error2.Add(er.Message);
						}
					}
					if (System.IO.Directory.Exists(sBin))
					{
						foreach (string s in afiles)
						{
							try
							{
								System.IO.File.Copy(s, System.IO.Path.Combine(sBin, System.IO.Path.GetFileName(s)), true);
							}
							catch (Exception er)
							{
								error2.Add(er.Message);
							}
						}
						copyFiles(sBin, dbConfigFiles);
					}
					if (error2.Count > 0)
					{
						MathNode.Log(error2);
					}
				}
			}
			if (!string.IsNullOrEmpty(appcfgFile) && File.Exists(appcfgFile))
			{
				itemGroup = docPrj.CreateElement("ItemGroup", msBuildUri);
				docPrj.DocumentElement.AppendChild(itemGroup);
				XmlNode nodeCfg = docPrj.CreateElement("None", msBuildUri);
				itemGroup.AppendChild(nodeCfg);
				XmlUtil.SetAttribute(nodeCfg, "Include", Path.GetFileName(appcfgFile));
			}
			//
			docPrj.Save(System.IO.Path.Combine(sourceFolder, System.IO.Path.GetFileNameWithoutExtension(_project.ProjectFile) + ".csproj"));
			return bOK;
		}
		private bool compileComponent(string componentFile)
		{
			logMsg("Compiling file :{0}", componentFile);
			//
			ComponentCompileResult ccr = new ComponentCompileResult();
			_componentCompileResults.Add(ccr);
			ClassCompiler classCompiler = new ClassCompiler();
			bool bOK = classCompiler.Compile(_project.ProjectFile, componentFile,
				_kioskBackgroundName, RootNamespace, MSBuildProjectDirectory, WebPhysicalFolder,
				OutputFullpath, _classIds, _classNames, AppName, WebJsFolder, WebJavaScriptFolder,
				WebPhpFolder, _appCodeName, PluginsFolder, _sessionVarNames, _sessionVarValues, _sessionTimeoutMinutes,
				_sessionVarCount, WebDebugMode, WebClientDebugLevel,_ajaxTimeout, _debug);
			//
			foreach (string s in classCompiler.Errors)
			{
				LogError(s);
			}
			foreach (string s in classCompiler.Logs)
			{
				logMsg(s);
			}
			if (bOK)
			{
				//all components using app config will be merged into this file
				string cfgFile = classCompiler.AppConfigFile;
				if (!string.IsNullOrEmpty(cfgFile))
				{
					if (File.Exists(cfgFile))
					{
						_appcfgFileInSourceFolder = cfgFile;
					}
				}
				if (classCompiler.UseDrawing)
				{
					this.UseDrawing = true;
				}
				UInt32 startPageId = classCompiler.StartWebPageId;
				if (startPageId != 0)
				{
					_startWebPageId = startPageId;
				}
				if (classCompiler.IsWebPage)
				{
					UInt32 classId = classCompiler.ClassId;
					string htmlPath = classCompiler.HtmlFilePath;
					if (!string.IsNullOrEmpty(htmlPath))
					{
						string designHtml = Path.GetFileName(htmlPath);
						designHtml = string.Format(CultureInfo.InvariantCulture, "{0}_design.html", Path.GetFileNameWithoutExtension(designHtml).ToLowerInvariant());
						_excludeFiles.Add(designHtml);
						_webPages.Add(classId, htmlPath);
					}
				}
				Guid[] dbIds = classCompiler.DatabaseConnectionIDs;
				for (int i = 0; i < dbIds.Length; i++)
				{
					if (!databaseConnectionIDs.Contains(dbIds[i]))
					{
						databaseConnectionIDs.Add(dbIds[i]);
					}
				}
				if (classCompiler.UsePageNavigator)
				{
					UsePageNavigator = true;
				}
				if (classCompiler.UseCopyProtection)
				{
					UseCopyProtection = true;
				}
				if (string.IsNullOrEmpty(_use32bit))
				{
					if (!string.IsNullOrEmpty(classCompiler.Use32bit))
					{
						_use32bit = classCompiler.Use32bit;
					}
				}
				string sf = classCompiler.ApplicationIcon;
				if (!string.IsNullOrEmpty(sf))
				{
					if (File.Exists(sf))
					{
						this.ApplicationIcon = sf;
					}
				}
				string[] ff = classCompiler.AssemblySupportFiles;
				for (int i = 0; i < ff.Length; i++)
				{
					string f2 = ff[i].ToLowerInvariant();
					if (!_assemblySupportFiles.Contains(f2))
					{
						_assemblySupportFiles.Add(f2);
					}
				}
				ff = classCompiler.AssemblyLocations;
				for (int i = 0; i < ff.Length; i++)
				{
					string f2 = ff[i].ToLowerInvariant();
					if (!_usedAssemblies.Contains(f2))
					{
						_usedAssemblies.Add(f2);
					}
				}
				if (classCompiler.UseDatetimePicker)
				{
					_usedatetimepicker = true;
				}
				if (classCompiler.IsWebService)
				{
					isWebService = true;
				}
				if (classCompiler.UseResources || classCompiler.UseResourcesX)
				{
					ff = classCompiler.ResxFiles;
					for (int i = 0; i < ff.Length; i++)
					{
						_lstResxFiles.Add(ff[i]);
					}
				}
				_lstSourceFiles.Add(classCompiler.SourceFile);
				if (classCompiler.UseResourcesX)
				{
					_lstSourceFiles.Add(classCompiler.SourceFileX);
				}
				ResourceFile[] embededFiles = classCompiler.EmbededFiles;
				List<ResourceFile> resFiles = new List<ResourceFile>();
				resFiles.AddRange(embededFiles);
				foreach (ResourceFile f in embededFiles)
				{
					if (f.BuildAction == EnumResourceBuildType.Embed)
					{
						bool bExists = false;
						foreach (string s in _embededFiles)
						{
							if (string.Compare(s, f.TargetFilename, StringComparison.OrdinalIgnoreCase) == 0)
							{
								bExists = true;
								break;
							}
						}
						if (!bExists)
						{
							_embededFiles.Add(f.TargetFilename);
						}
					}
				}
				ccr.IsForm = classCompiler.IsForm;
				ccr.ResourceFile = classCompiler.ResourceFile;
				ccr.ResourceFiles = resFiles;
				ccr.ResourceFileX = classCompiler.ResourceFileX;
				ccr.SourceFile = classCompiler.SourceFile;
				ccr.SourceFileX = classCompiler.SourceFileX;
				ccr.UseResources = classCompiler.UseResources;
				ccr.UseResourcesX = classCompiler.UseResourcesX;
			}
			return bOK;
		}
		private void copyOutputFiles()
		{
			if (_project.IsWebApplication)
			{
				string ph = WebPhysicalFolder;
				StringCollection folders = _project.GetOutputFolderList();
				foreach (string s in folders)
				{
					if (string.Compare(s, ph, StringComparison.OrdinalIgnoreCase) != 0)
					{
						if (!Directory.Exists(s))
						{
							LogError("Output folder does not exist:{0}", s);
						}
						else
						{
							logMsg("Copy web files from [{0}] to [{1}]", ph, s);
							FileUtilities.CopyDirs(ph, s, "*.*", _excludeFiles);
						}
					}
				}
			}
		}
		private Form OutputForm
		{
			get
			{
				if (OutputWindow != null)
				{
					return OutputWindow.FindForm();
				}
				return null;
			}
		}
		public bool Execute()
		{
			VPLUtil.IsCompiling = true;
			VPLUtil.IsCopyProtectorClient = false;
			ILimnorStudioProject currentProject = VPLUtil.CurrentProject;
			VPLUtil.JsFiles = new StringCollection();
			if (_services != null)
			{
				if (_services.ContainsKey(typeof(IOutputWindow)))
				{
					OutputWindow = (IOutputWindow)_services[typeof(IOutputWindow)];
				}
			}
			VPLUtil.ErrorLogger = this;
			try
			{
				TraceLogClass.TraceLog.SwitchLogFiles(MathNode.FILE_DEFAULT_COMPILERLOG);
				TraceLogClass.TraceLog.ClearLogContents();
				MathNode.TrimOneTime();
				logMsg("\r\nProject Build start at {0} for [{1}] ............", System.DateTime.Now.ToString("u"), ProjectFile);
				shouldCompileBinary = true;
				CompilerFolders.NeedClearFolders = true;
				_assemblyLocations = new Dictionary<string, Assembly>();
				_usedAssemblies = new StringCollection();
				VPLUtil.IsCompilingToDebug = (string.Compare(Configuration, "Debug", StringComparison.OrdinalIgnoreCase) == 0);
				_debug = false; //for visual debugger
				if (_project == null)
				{
					_project = new LimnorProject(ProjectFile);
				}
				DialogFileMap.LoadFileMappings(ProjectFile);
				VPLUtil.CurrentProject = _project;
				if (_project.ProjectType == EnumProjectType.Setup)
				{
					string plt = PlatformTarget;
					EnumPlatform pl = EnumPlatform.x32;
					if (string.IsNullOrEmpty(plt))
					{
						pl = EnumPlatform.x32;
						AssemblyTargetPlatform pf = _project.DefaultAssemblyTargetPlatform;
						if (pf == AssemblyTargetPlatform.X64)
						{
							pl = EnumPlatform.x64;
						}
					}
					else
					{
						if (string.Compare(plt, "x64", StringComparison.OrdinalIgnoreCase) == 0)
						{
							pl = EnumPlatform.x64;
						}
					}
					IDistributor dis = loadDistributor();
					if (dis == null)
					{
						LogError("Distributor LimnorWix.dll not found in :{0} ", AppDomain.CurrentDomain.BaseDirectory);
					}
					else
					{
						string ret = dis.Compile(pl, this, _project.MainComponent.MainComponentFile, _project.ProjectName, Configuration);
						if (!string.IsNullOrEmpty(ret))
						{
							LogError(ret);
						}
					}
				}
				else
				{
					TraceLogClass.TraceLog.ResetIndent();
					logMsg("Project file={0}", _project.ProjectFile);
					logMsg("$(MSBuildExtensionsPath)={0}", BuildExpPath);
					logMsg("$(MSBuildAllProjects)={0}", MSBuildAllProjects);
					logMsg("$(MainFile)={0}", MainFile);
					logMsg("$(MSBuildBinPath)={0}", MSBuildBinPath);
					logMsg("$(OutputType)={0}", OutputType);
					logMsg("$(MSBuildProjectDirectory)={0}", MSBuildProjectDirectory);
					logMsg("$(CoreCompileDependsOn)={0}", CoreCompileDependsOn);
					logMsg("$(RootNamespace)={0}", RootNamespace);
					//
					if (!Directory.Exists(OutputFullpath))
					{
						Directory.CreateDirectory(OutputFullpath);
					}
					//copy config file
					string[] cfgFile = System.IO.Directory.GetFiles(MSBuildProjectDirectory, "*.cfg");
					if (cfgFile != null && cfgFile.Length > 0)
					{
						for (int i = 0; i < cfgFile.Length; i++)
						{
							System.IO.File.Copy(cfgFile[i],
								System.IO.Path.Combine(OutputFullpath, System.IO.Path.GetFileName(cfgFile[i])),
								true);
						}
					}
					if (LimnorProject.IsApp(_project.ProjectType))
					{
						string appCfg = System.IO.Path.Combine(OutputFullpath, string.Format(CultureInfo.InvariantCulture, "{0}.exe.config", _project.ProjectAssemblyName));
						if (File.Exists(appCfg))
						{
							XmlDocument cfgDoc = new XmlDocument();
							cfgDoc.Load(appCfg);
							XmlNode cfgNode = cfgDoc.DocumentElement;
							if (cfgNode == null || string.CompareOrdinal(cfgNode.Name, "configuration") != 0)
							{
								LogError("Invalid application configuration file: [{0}]", appCfg);
							}
							else
							{
								XmlNode startNode = cfgNode.SelectSingleNode("startup");
#if DOTNET40
                                if (startNode == null)
                                {
                                    startNode = cfgDoc.CreateElement("startup");
                                    cfgNode.AppendChild(startNode);
                                }
                                XmlUtil.SetAttribute(startNode, "useLegacyV2RuntimeActivationPolicy", true);
                                XmlNode runTimeNode = startNode.SelectSingleNode("supportedRuntime");
                                if (runTimeNode == null)
                                {
                                    runTimeNode = cfgDoc.CreateElement("supportedRuntime");
                                    startNode.AppendChild(runTimeNode);
                                }
                                XmlUtil.SetAttribute(runTimeNode, "version", "v4.0");
                                XmlUtil.SetAttribute(runTimeNode, "sku", ".NETFramework,Version=v4.0");
                                runTimeNode = cfgNode.SelectSingleNode("runtime");
                                if (runTimeNode == null)
                                {
                                    runTimeNode = cfgDoc.CreateElement("runtime");
                                    cfgNode.AppendChild(runTimeNode);
                                }
                                XmlNode lNode = runTimeNode.SelectSingleNode("loadFromRemoteSources");
                                if (lNode == null)
                                {
                                    lNode = cfgDoc.CreateElement("loadFromRemoteSources");
                                    runTimeNode.AppendChild(lNode);
                                }
                                XmlUtil.SetAttribute(lNode, "enabled", true);
                                cfgDoc.Save(appCfg);
#else
								if (startNode != null)
								{
									XmlNode np = startNode.ParentNode;
									np.RemoveChild(startNode);
									cfgDoc.Save(appCfg);
								}
#endif
							}
						}
						else
						{
#if DOTNET40
                            StreamWriter sw = new StreamWriter(appCfg);
                            sw.Write(Resources1.app_config);
                            sw.Close();
#endif
						}
					}
					//copy support files
					ProjectSupportFiles psf = _project.GetTypedProjectData<ProjectSupportFiles>();
					if (psf == null)
					{
						psf = new ProjectSupportFiles(_project);
					}
					if (psf.Count > 0)
					{
						for (int i = 0; i < psf.Count; i++)
						{
							System.IO.File.Copy(psf[i],
								System.IO.Path.Combine(OutputFullpath, System.IO.Path.GetFileName(psf[i])),
								true);
						}
					}
					if (_project.ProjectType == EnumProjectType.Kiosk)
					{
						string kioskMode = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "KioskMode.exe");
						if (!File.Exists(kioskMode))
						{
							LogError("File not found:{0}", kioskMode);
						}
						else
						{
							File.Copy(kioskMode,
								Path.Combine(OutputFullpath, Path.GetFileName(kioskMode)),
								true);
						}
					}
					CompilerFolders clfolders = new CompilerFolders(MSBuildProjectDirectory);
					appcfgFile = Path.Combine(clfolders.SourceFolder, "app.config");
					if (File.Exists(appcfgFile))
					{
						File.Delete(appcfgFile);
					}
					if (_project.ProjectType == EnumProjectType.WebAppAspx)
					{
						if (!Directory.Exists(WebPhysicalFolder))
						{
							Directory.CreateDirectory(WebPhysicalFolder);
						}
						webTargetFolder = Path.Combine(WebPhysicalFolder, "bin");
						if (!Directory.Exists(webTargetFolder))
						{
							Directory.CreateDirectory(webTargetFolder);
						}
					}
					//
					//Create project resources
					bool bUseFilenames = compileResources();
					//
					if (_project.ProjectType == EnumProjectType.WinForm || _project.ProjectType == EnumProjectType.Kiosk || _project.ProjectType == EnumProjectType.Console)
					{
						string f = exeFilePathInObject;
						if (File.Exists(f))
						{
							try
							{
								File.Delete(f);
							}
							catch (Exception err)
							{
								throw new DesignerException(err, "Error removing {0}. If the last debugging has just finished not long ago then please wait a few seconds and re-compile the project.", f);
							}
						}
						f = exeFilePathInBin;
						if (File.Exists(f))
						{
							try
							{
								File.Delete(f);
							}
							catch (Exception err)
							{
								throw new DesignerException(err, "Error removing {0}. If the last debugging has just finished not long ago then please wait a few seconds and re-compile the project.", f);
							}
						}
					}
					//
					string[] files = _project.GetComponentFiles();
					if (files == null || files.Length == 0)
					{
						LogError("No limnor files in the project");
					}
					else
					{
						bool bOK = true;
						logMsg("Files to be compiled:{0}", files.Length);
						if (_project.ProjectType == EnumProjectType.WebAppPhp)
						{
							_modifiedPages = _project.GetModifiedClasses();
						}
						initCompile(files);
						_classIds = new UInt32[_classList.Count];
						_classNames = new string[_classList.Count];
						int k = 0;
						foreach (KeyValuePair<UInt32, ComponentID> kv in _classList)
						{
							_classIds[k] = kv.Key;
							_classNames[k] = GetClassTypeName(kv.Key);
							k++;
						}
						SortedDictionary<UInt32, ComponentID>.Enumerator classes = _classList.GetEnumerator();
						while (classes.MoveNext())
						{
							if (!compileComponent(classes.Current.Value.ComponentFile))
							{
								bOK = false;
							}
						}
						if (bOK)
						{
							postCompile();
							if (shouldCompileBinary)
							{
								loadConfigs();
								addAssemblies0(bUseFilenames);
								//create AssemblyInfo.cs
								AssemblyInfoBuilder aib = new AssemblyInfoBuilder(this, _project, MSBuildProjectDirectory);
								_lstSourceFiles.Add(aib.SourceFile);
								compile0();
								//
								Type t;
								StringCollection afiles;
								StringCollection errors = new StringCollection();
								//
								//add assembly support file=================================
								addAssemblySupportFiles();
								//copy debug support file
								if (_debug)
								{
									if (!copyDebugFiles(errors))
									{
										bOK = false;
									}
								}
								afiles = new StringCollection();
								if (_project.ProjectType == EnumProjectType.Kiosk)
								{
									t = typeof(FormKioskBackground);
									if (!DesignUtil.CopyAssembly(afiles, t, OutputFullpath, errors))
									{
										bOK = false;
									}
								}
								if (UseCopyProtection)
								{
									addCopyProtection();
								}
								if (!copyAssemblySupportFiles())
								{
									bOK = false;
								}
								//
								if (_project.ProjectType == EnumProjectType.WebAppAspx)
								{
									webTargetFolder = Path.Combine(WebPhysicalFolder, "bin");
								}
								//
								if (bOK)
								{
									checkUnusedDllFiles();
								}
								else
								{
									MathNode.Log(errors);
								}
								//
								//generate csproj file=======================================
								if (!createCsProject(afiles, errors))
								{
								}
								////
								//copy localized resources
								for (int i = 0; i < _languageNames.Length; i++)
								{
									copyLocalizedResources(_languageNames[i]);
								}
								if (_project.ProjectType == EnumProjectType.WebAppAspx)
								{
									copyAspxFiles(errors);
								}
							}
						}
						copyOutputFiles();
					}
				}
			}
			catch (Exception err)
			{
				string msg = TraceLogClass.ExceptionMessage0(err);
				LogError(msg);
			}
			finally
			{
				DialogFileMap.SaveFileMappings(ProjectFile);
				VPLUtil.CurrentProject = currentProject;
				VPLUtil.ErrorLogger = null;
				VPLUtil.IsCompiling = false;
				TraceLogClass.TraceLog.ResetIndent();
				if (Log.HasLoggedErrors && MathNode.ErrorFileExist())
				{
					MathNode.ViewErrorLog();
				}
				TraceLogClass.TraceLog.RestoreLogFiles();
			}
			ClassPointer.OnCompileFinish(_project);
			return !Log.HasLoggedErrors;
		}
		private void postCompile()
		{
			_project.ClearModifyFlags();
			//TBD optimization: copy lang folder only _usedatetimepicker, need to examine the concequence of missing lang
			if (_usedatetimepicker)
			{
			}
			{
				string lgDir = Path.Combine(WebPhysicalFolder, WebResourceFile.WEBFOLDER_Javascript);
				lgDir = Path.Combine(lgDir, "lang");
				if (!Directory.Exists(lgDir))
				{
					Directory.CreateDirectory(lgDir);
				}
				string[] jsfiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "datepicker_lang"));
				if (jsfiles != null)
				{
					foreach (string f in jsfiles)
					{
						string tf = Path.Combine(lgDir, Path.GetFileName(f));
						if (!File.Exists(tf))
						{
							File.Copy(f, tf, false);
						}
					}
				}
			}
			if (_project.ProjectType == EnumProjectType.WebAppAspx || _project.ProjectType == EnumProjectType.WebAppPhp)
			{
				createHomePage0();
				//
				if (_project.ProjectType == EnumProjectType.WebAppPhp && !_increment)
				{
					if (databaseConnectionIDs.Count > 0)
					{
						string crFile = Path.Combine(WebPhpFolder, "mySqlcredential.php");
						if (File.Exists(crFile))
						{
							string crFileBk = Path.Combine(WebPhpFolder, "mySqlcredential.php.backup");
							if (!File.Exists(crFileBk))
							{
								File.Copy(crFile, crFileBk);
							}
							File.Delete(crFile);
						}
						StreamWriter sw = new StreamWriter(crFile, false, Encoding.ASCII);
						bool saveConnection = false;
						VSPrj.PropertyBag connections = this._project.GetMySqlConnections();
						sw.WriteLine("<?php");
						sw.WriteLine("/*");
						sw.WriteLine("	Edit this file to provide MySQL credential");
						sw.WriteLine("*/");
						foreach (Guid g in databaseConnectionIDs)
						{
							ConnectionItem cn = ConnectionItem.LoadConnection(g, false, false);
							string codeName = ServerCodeutility.GetPhpMySqlConnectionName(g);
							sw.Write("// MySql database and credential for connection ");
							sw.Write(cn.Name);
							sw.Write(", ");
							sw.WriteLine(codeName);
							if (!string.IsNullOrEmpty(cn.Description))
							{
								sw.Write("/*\r\n ");
								sw.WriteLine(cn.Description);
								sw.WriteLine("*/\r\n");
							}
							sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0} = new Credential();", codeName));
							string host = ConnectionStringSelector.GetServerName(cn.ConnectionString);
							string db = ConnectionStringSelector.GetDatabaseName(cn.ConnectionString);
							string user = ConnectionStringSelector.GetSQLUser(cn.ConnectionString);
							string pass = ConnectionStringSelector.GetSQLUserPassword(cn.ConnectionString);
							if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
							{
								for (int i = 0; i < connections.Count; i++)
								{
									if (connections[i].ContainsKey("ID"))
									{
										Guid g0 = new Guid(connections[i]["ID"]);
										if (g0 == cn.ConnectionGuid)
										{
											if (connections[i].ContainsKey("HOST"))
											{
												host = connections[i]["HOST"];
											}
											if (connections[i].ContainsKey("DB"))
											{
												db = connections[i]["DB"];
											}
											if (connections[i].ContainsKey("USER"))
											{
												user = connections[i]["USER"];
											}
											if (connections[i].ContainsKey("PASS"))
											{
												pass = connections[i]["PASS"];
											}
											break;
										}
									}
								}
								DlgDatabaseConnection dlg = new DlgDatabaseConnection();
								dlg.LoadData("MySQL Database Connection for Testing PHP Web Application", cn.Name, cn.Description, host, db, user, pass);
								if (dlg.ShowDialog() == DialogResult.OK)
								{
									host = dlg.Host;
									db = dlg.Db;
									user = dlg.User;
									pass = dlg.Pass;
									Dictionary<string, string> savedConnect = new Dictionary<string, string>();
									savedConnect.Add("ID", cn.ConnectionGuid.ToString("D", CultureInfo.InvariantCulture));
									savedConnect.Add("HOST", host);
									savedConnect.Add("DB", db);
									savedConnect.Add("USER", user);
									savedConnect.Add("PASS", pass);
									for (int i = 0; i < connections.Count; i++)
									{
										if (connections[i].ContainsKey("ID"))
										{
											Guid g0 = new Guid(connections[i]["ID"]);
											if (g0 == cn.ConnectionGuid)
											{
												connections.RemoveAt(i);
												break;
											}
										}
									}
									connections.Add(savedConnect);
									saveConnection = true;
								}
							}
							if (string.IsNullOrEmpty(host))
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->host = 'localhost';", codeName));
							}
							else
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->host = '{1}';", codeName, host));
							}
							if (string.IsNullOrEmpty(user))
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->user = 'root';", codeName));
							}
							else
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->user = '{1}';", codeName, user));
							}
							if (string.IsNullOrEmpty(pass))
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->password = '123';", codeName));
							}
							else
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->password = '{1}';", codeName, pass));
							}
							if (string.IsNullOrEmpty(db))
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->database = 'test1';", codeName));
							}
							else
							{
								sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "${0}->database = '{1}';", codeName, db));
							}
							sw.WriteLine();
						}
						sw.WriteLine("?>");
						if (saveConnection)
						{
							_project.SetMySqlConnections(connections);
						}
						sw.Close();
					}
				}
			}
		}
		//Deprecation
		public string GetClassTypeName(UInt32 classId)
		{
			foreach (KeyValuePair<UInt32, ComponentID> kv in _classList)
			{
				if (kv.Value.ComponentId == classId)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _project.Namespace, kv.Value.ComponentName);
				}
			}
			return null;
		}
		#region ILimnorBuilder Members

		public void AddService(Type serviceType, object service)
		{
			if (_services == null)
			{
				_services = new Dictionary<Type, object>();
			}
			if (_services.ContainsKey(serviceType))
			{
				_services[serviceType] = service;
			}
			else
			{
				_services.Add(serviceType, service);
			}
		}
		public void SetProject(LimnorProject project, string config)
		{
			_project = project;
			ProjectFile = _project.ProjectFile;
			AssemblyCompany = project.AssemblyCompany;
			AssemblyCopyright = project.AssemblyCopyright;
			AssemblyDescription = project.AssemblyDescription;
			AssemblyFileVersion = project.IncreaseAssemblyFileVersion();

			AssemblyProduct = project.AssemblyProduct;
			AssemblyTitle = project.AssemblyTitle;
			AssemblyVersion = project.AssemblyVersion;
			//
			BuildExpPath = AppDomain.CurrentDomain.BaseDirectory;
			MSBuildBinPath = AppDomain.CurrentDomain.BaseDirectory;
			MSBuildProjectDirectory = Path.GetDirectoryName(project.ProjectFile);
			OutputPath = project.GetOutputPath(config);
			OutputType = project.OutputType;
			PlatformTarget = project.TargetPlatform.ToString();
			TargetFramework = project.TargetFramework;
			RootNamespace = project.Namespace;
			MainFile = project.MainFile;
			if (!string.IsNullOrEmpty(MainFile))
			{
				MainFile = Path.Combine(MSBuildProjectDirectory, MainFile);
			}
			//
			Configuration = config;
		}
		#endregion
	}
}
