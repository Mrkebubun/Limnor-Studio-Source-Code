/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.ComponentModel;
using XmlUtility;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Globalization;
using VPL;
using LimnorWeb;
using System.Windows.Forms;
using System.Threading;

namespace VSPrj
{
	public interface IXmlDesignerLoader
	{
		void NotifyChanges();
		void NotifySelection(object selectedObject);
		XmlNode Node { get; }
		bool DataModified { get; set; }
		string CreateNewComponentName(string basename, StringCollection names);
		IComponent CreateComponent(Type type, string name);
		void DeleteComponent(IComponent component);
		bool IsClosing { get; }
		LimnorProject ActiveProject { get; }
		IXmlDesignerLoader ActiveXmlDesignerLoader { get; }
		object DesignerObject { get; }
		UInt32 ClassID { get; }
	}
	public class ProjectMainComponentAttribute : Attribute
	{
		private EnumProjectType _prjType;
		public ProjectMainComponentAttribute(EnumProjectType projectType)
		{
			_prjType = projectType;
		}
		public EnumProjectType ProjectType
		{
			get
			{
				return _prjType;
			}
		}
	}
	public enum AssemblyTargetPlatform
	{
		X86,
		X64,
		AnyCPU
	}
	public enum AssemblyTargetFramework
	{
		V35,
		V40
	}
	public enum EnumProjectType
	{
		Unknown = 0,
		WinForm,
		Console,
		Kiosk,
		ClassDLL,
		WpfApp,
		WebAppAspx,
		WebAppPhp,
		WebService,
		WinService,
		Setup,
		ScreenSaver
	}
	class ProjectProperties
	{
		private bool _tm;
		public bool TraceMethodLoad { get { return _tm; } set { _tm = value; } }
	}

	public class OutputFolder
	{
		public OutputFolder()
		{
		}
		public OutputFolder(bool disable, string dir)
		{
			Disabled = disable;
			Folder = dir;
		}
		public override string ToString()
		{
			return Folder;
		}
		public bool Disabled { get; set; }
		public string Folder { get; set; }
	}
	/// <summary>
	/// hold project wide data
	/// </summary>
	[NotForProgramming]
	public class LimnorProject : ILimnorProject, ILimnorStudioProject
	{
		#region constants
		public const string PRJEXT = "lrproj";
		const string XML_ItemGroup = "ItemGroup";
		const string XML_VobGroup = "VobGroup";
		const string XML_PropertyGroup = "PropertyGroup";
		const string XML_ProjectGuid = "ProjectGuid";
		const string XML_Content = "Content";
		const string XML_Toolbox = "Toolbox";
		const string XML_Assembly = "Assembly";
		const string XML_AssemblyFileVersion = "AssemblyFileVersion";
		const string XMLATT_Version = "version";
		const string XMLATT_WebFolderGranted = "webFolderGranted";
		const string XML_Item = "Item";
		const string XML_Project = "Project";
		const string XML_ModifiedClasses = "ModifiedClasses";
		public const string XMLATT_Include = "Include";
		const string XMLATT_Gac = "gac";
		const string XMLATT_ActiveConfig = "activeConfig";
		const string XMLATT_TraceLevel = "traceLevel";
		const string XMLATT_EnableFileMapper = "enableFileMapper";
		const string PRJNAMESPACEURI = "http://schemas.microsoft.com/developer/msbuild/2003";
		const string XMLATT_TraceMethodLoad = "traceMethodLoad";
		const string XMLATT_isNotNew = "isNotNew";
		#endregion
		#region fields and constructors
		private static IXmlDesignerLoader _activeDesignerLoader;
		private static LimnorProject _activeProject;
		//
		private string _folder;
		private Guid _guid;
		private string _prjFile;
		private ProjectMainComponent _mainComponent;
		private ProjectProperties _properties = new ProjectProperties();
		//
		private bool _isMainProjectFile;
		private XmlDocument _mainDocument;
		private XmlNode _rootNode;
		private XmlNamespaceManager _xnm;
		private bool _changed;
		//
		public static EventHandler DatabaseListChanged;
		//
		public LimnorProject(string filepath)
		{
			if (string.IsNullOrEmpty(filepath))
			{
				throw new ProjectException("Cannot create a LimnorProject without a project file");
			}
			ProjectFile = filepath;
			LimnorSolution.AddProject(this);
		}
		#endregion
		#region private properties
		private XmlNamespaceManager xnm
		{
			get
			{
				if (_xnm == null)
				{
					if (_mainDocument == null)
					{
						SetAsMainProjectFile();
					}
					if (_mainDocument != null)
					{
						_xnm = new XmlNamespaceManager(_mainDocument.NameTable);
					}
					_xnm.AddNamespace("x", PRJNAMESPACEURI);
				}
				return _xnm;
			}
		}
		#endregion
		#region Named data storage
		const string ND_TOOLBOXLOADED = "ToolboxLoaded";
		public bool ToolboxLoaded
		{
			get
			{
				return GetProjectNamedData<bool>(ND_TOOLBOXLOADED);
			}
			set
			{
				SetProjectNamedData<bool>(ND_TOOLBOXLOADED, value);
			}
		}
		/// <summary>
		/// use static so that the value is not tied to object instances
		/// </summary>
		private static Dictionary<Guid, Dictionary<string, object>> _projectNamedData;
		public void SetProjectNamedData<T>(string name, T value)
		{
			if (_projectNamedData == null)
			{
				_projectNamedData = new Dictionary<Guid, Dictionary<string, object>>();
			}
			Dictionary<string, object> v;
			if (!_projectNamedData.TryGetValue(_guid, out v))
			{
				v = new Dictionary<string, object>();
				_projectNamedData.Add(_guid, v);
			}
			if (v.ContainsKey(name))
			{
				v[name] = value;
			}
			else
			{
				v.Add(name, value);
			}
		}
		public T GetProjectNamedData<T>(string name)
		{
			if (_projectNamedData != null)
			{
				Dictionary<string, object> v;
				if (_projectNamedData.TryGetValue(_guid, out v))
				{
					object o;
					if (v.TryGetValue(name, out o))
					{
						return (T)o;
					}
				}
			}
			return default(T);
		}
		#endregion
		#region Typed data storage
		/// <summary>
		/// Dictionary of {Project, Dictionary of {Class Id, Dictionary of {Type, object} } }
		/// this storage provides 3 purposes:
		/// 1. a static memory read/write by all object instances through keys of project guid, class id, and type,
		///    making the data virtually instance-based
		/// 2. provide a strongly-typed programming interface for types that do not need to be accessible from this module,
		///    for example, ILimnorDesignPane
		/// 3. maintain a single instance of a Type
		/// </summary>
		private static Dictionary<Guid, Dictionary<UInt32, Dictionary<Type, object>>> _typedData;
		public T GetTypedData<T>(UInt32 classId)
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					Dictionary<Type, object> td;
					if (tdp.TryGetValue(classId, out td))
					{
						object v;
						if (td.TryGetValue(typeof(T), out v))
						{
							return (T)v;
						}
					}
				}
			}
			return default(T);
		}
		public bool HasTypedData<T>(UInt32 classId)
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					Dictionary<Type, object> td;
					if (tdp.TryGetValue(classId, out td))
					{
						if (td.ContainsKey(typeof(T)))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public Dictionary<UInt32, T> GetTypedDataList<T>()
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					Type t = typeof(T);
					Dictionary<UInt32, T> list = new Dictionary<UInt32, T>();
					foreach (KeyValuePair<UInt32, Dictionary<Type, object>> kv in tdp)
					{
						object v;
						if (kv.Value.TryGetValue(t, out v))
						{
							list.Add(kv.Key, (T)v);
						}
					}
					return list;
				}
			}
			return null;
		}
		public Dictionary<Type, object> GetTypedDataCollection(UInt32 classId)
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					Dictionary<Type, object> td;
					if (tdp.TryGetValue(classId, out td))
					{
						return td;
					}
				}
			}
			return null;
		}
		public void SetTypedData<T>(UInt32 classId, T v)
		{
			Type t = typeof(T);
			if (_typedData == null)
			{
				_typedData = new Dictionary<Guid, Dictionary<UInt32, Dictionary<Type, object>>>();
			}
			Dictionary<UInt32, Dictionary<Type, object>> tdp;
			if (!_typedData.TryGetValue(_guid, out tdp))
			{
				tdp = new Dictionary<UInt32, Dictionary<Type, object>>();
				_typedData.Add(_guid, tdp);
			}
			Dictionary<Type, object> td;
			if (!tdp.TryGetValue(classId, out td))
			{
				td = new Dictionary<Type, object>();
				tdp.Add(classId, td);
			}
			if (td.ContainsKey(t))
			{
				td[t] = v;
			}
			else
			{
				td.Add(t, v);
			}
		}
		public void RemoveTypedData<T>(UInt32 classId)
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					Dictionary<Type, object> td;
					if (tdp.TryGetValue(classId, out td))
					{
						Type t = typeof(T);
						if (td.ContainsKey(t))
						{
							td.Remove(t);
							if (td.Count == 0)
							{
								tdp.Remove(classId);
							}
						}
					}
					if (tdp.Count == 0)
					{
						_typedData.Remove(_guid);
					}
				}
			}
		}
		public void ClearTypedData(UInt32 classId)
		{
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					if (tdp.ContainsKey(classId))
					{
						tdp.Remove(classId);
					}
				}
			}
		}
		//
		//project scope data
		private static Dictionary<Guid, Dictionary<Type, object>> _typedProjectData;
		public static void ClearTypeProjectData()
		{
			_typedProjectData = null;
		}
		public T GetTypedProjectData<T>()
		{
			if (_typedProjectData != null)
			{
				Dictionary<Type, object> data;
				if (_typedProjectData.TryGetValue(_guid, out data))
				{
					object v;
					Type t = typeof(T);
					if (data.TryGetValue(t, out v))
					{
						return (T)v;
					}
				}
			}
			return default(T);
		}
		public void SetTypedProjectData<T>(T value)
		{
			if (_typedProjectData == null)
			{
				_typedProjectData = new Dictionary<Guid, Dictionary<Type, object>>();
			}
			Dictionary<Type, object> data;
			if (!_typedProjectData.TryGetValue(_guid, out data))
			{
				data = new Dictionary<Type, object>();
				_typedProjectData.Add(_guid, data);
			}
			Type t = typeof(T);
			if (data.ContainsKey(t))
			{
				data[t] = value;
			}
			else
			{
				data.Add(t, value);
			}
		}
		public bool HasTypedProjectData<T>()
		{
			if (_typedProjectData != null)
			{
				Dictionary<Type, object> data;
				if (_typedProjectData.TryGetValue(_guid, out data))
				{
					object v;
					Type t = typeof(T);
					if (data.TryGetValue(t, out v))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void RemoveTypedProjectData<T>()
		{
			if (_typedProjectData != null)
			{
				Dictionary<Type, object> data;
				if (_typedProjectData.TryGetValue(_guid, out data))
				{
					Type t = typeof(T);
					if (data.ContainsKey(t))
					{
						data.Remove(t);
					}
				}
			}
		}
		public T GetProjectSingleData<T>()
		{
			T data = GetTypedProjectData<T>();
			if (data == null)
			{
				data = (T)Activator.CreateInstance(typeof(T), this);
				SetTypedProjectData<T>(data);
			}
			return data;
		}
		#endregion
		#region File rename storage
		private static Dictionary<Guid, Dictionary<string, string>> _fileRenames;
		public void AddFileRename(string oldFilename, string newFilename)
		{
			if (_fileRenames == null)
			{
				_fileRenames = new Dictionary<Guid, Dictionary<string, string>>();
			}
			Dictionary<string, string> mappings;
			if (!_fileRenames.TryGetValue(_guid, out mappings))
			{
				mappings = new Dictionary<string, string>();
				_fileRenames.Add(_guid, mappings);
			}
			string key = oldFilename.ToLowerInvariant();
			if (mappings.ContainsKey(key))
			{
				mappings[key] = newFilename;
			}
			else
			{
				mappings.Add(key, newFilename);
			}
			if (_fileProjectFile != null)
			{
				foreach (KeyValuePair<string, string> kv in _fileProjectFile)
				{
					if (string.Compare(kv.Value, ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (string.Compare(oldFilename, System.IO.Path.GetFileName(kv.Key), StringComparison.OrdinalIgnoreCase) == 0)
						{
							_fileProjectFile.Remove(kv.Key);
							_fileProjectFile.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(kv.Key), newFilename), ProjectFile);
							break;
						}
					}
				}
			}
		}
		public string GetFileRename(string oldFilename)
		{
			if (_fileRenames != null)
			{
				Dictionary<string, string> mappings;
				if (_fileRenames.TryGetValue(_guid, out mappings))
				{
					string key = oldFilename.ToLowerInvariant();
					if (mappings.ContainsKey(key))
					{
						return mappings[key];
					}
				}
			}
			return string.Empty;
		}
		#endregion
		#region Build Config
		public string ActiveBuildConfig
		{
			get
			{
				string prjFile = this.ProjectFile;
				if (string.IsNullOrEmpty(prjFile))
				{
					return "Release";
				}
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						return "Release";
					}
					return XmlUtil.GetAttribute(doc.DocumentElement, XMLATT_ActiveConfig);
				}
			}
		}
		public void RemoveActiveBuildConfig()
		{
			string prjFile = this.ProjectFile;
			if (!string.IsNullOrEmpty(prjFile))
			{
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						XmlNode nodeRoot = d.Doc.CreateElement(XML_Project);
						d.Doc.AppendChild(nodeRoot);
					}
					else
					{
						if (doc.DocumentElement.Attributes != null)
						{
							XmlAttribute xa = doc.DocumentElement.Attributes[XMLATT_ActiveConfig];
							if (xa != null)
							{
								doc.DocumentElement.Attributes.Remove(xa);
								d.Save();
							}
						}
					}
				}
			}
		}
		public void SetActiveConfig(string config)
		{
			string prjFile = this.ProjectFile;
			if (!string.IsNullOrEmpty(prjFile))
			{
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						XmlNode nodeRoot = d.Doc.CreateElement(XML_Project);
						d.Doc.AppendChild(nodeRoot);
					}
					XmlUtil.SetAttribute(doc.DocumentElement, XMLATT_ActiveConfig, config);
					d.Save();
				}
			}
		}
		#endregion
		#region static functions
		public static string GetWebSiteName(XmlNode appNode)
		{
			if (appNode != null)
			{
				XmlNode wnNode = appNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
							"{0}[@{1}='WebSiteName']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
				if (wnNode != null)
				{
					return wnNode.InnerText.Trim();
				}
			}
			return string.Empty;
		}
		/// <summary>
		/// {component file,project file}
		/// </summary>
		private static Dictionary<string, string> _fileProjectFile;
		private static Dictionary<string, List<object>> _designPanes;
		private static Dictionary<string, string> _genericFileRenames;
		public static bool IsWinformApp(EnumProjectType p)
		{
			return (p == EnumProjectType.Kiosk || p == EnumProjectType.WinForm || p == EnumProjectType.ScreenSaver);
		}
		public static bool IsApp(EnumProjectType p)
		{
			return (p == EnumProjectType.Kiosk
				|| p == EnumProjectType.WinForm
				|| p == EnumProjectType.ScreenSaver
				|| p == EnumProjectType.Console
				|| p == EnumProjectType.WebService
				|| p == EnumProjectType.WpfApp
				);
		}
		public static AssemblyTargetFramework GetTargetFrameworkEnum(string tf)
		{
			if (string.IsNullOrEmpty(tf))
			{
				return AssemblyTargetFramework.V40;
			}
			if (string.Compare(tf, "v3.5", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return AssemblyTargetFramework.V35;
			}
			if (string.Compare(tf, "v4.0", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return AssemblyTargetFramework.V40;
			}
			return AssemblyTargetFramework.V40;
		}
		public static string GetTargetFrameworkValue(AssemblyTargetFramework tf)
		{
			if (tf == AssemblyTargetFramework.V35)
			{
				return "v3.5";
			}
			if (tf == AssemblyTargetFramework.V40)
			{
				return "v4.0";
			}
			return "v4.0";
		}
		public static string ProgramExtension(EnumProjectType p)
		{
			if (p == EnumProjectType.ClassDLL || p == EnumProjectType.WebService || p == EnumProjectType.WebAppAspx)
			{
				return ".dll";
			}
			if (p == EnumProjectType.WebAppPhp)
			{
				return ".php";
			}
			return ".exe";
		}
		public static void RecordFileRename(string oldFile, string newFile)
		{
			if (_genericFileRenames == null)
			{
				_genericFileRenames = new Dictionary<string, string>();
			}
			oldFile = oldFile.ToLowerInvariant();
			if (_genericFileRenames.ContainsKey(oldFile))
			{
				_genericFileRenames[oldFile] = newFile;
			}
			else
			{
				_genericFileRenames.Add(oldFile, newFile);
			}
		}
		public static bool TryGetRenamedFilenameByOldName(string oldFile, out string newFile)
		{
			if (_genericFileRenames != null)
			{
				return _genericFileRenames.TryGetValue(oldFile.ToLowerInvariant(), out newFile);
			}
			newFile = oldFile;
			return false;
		}
		public static void SetFileProjectLink(string file, string projectFile)
		{
			if (_fileProjectFile == null)
			{
				_fileProjectFile = new Dictionary<string, string>();
			}
			if (_fileProjectFile.ContainsKey(file))
			{
				_fileProjectFile[file] = projectFile;
			}
			else
			{
				_fileProjectFile.Add(file, projectFile);
			}
		}
		public static void RemoveFileProjectLink(string file)
		{
			if (_fileProjectFile != null)
			{
				if (_fileProjectFile.ContainsKey(file))
				{
					_fileProjectFile.Remove(file);
				}
			}
		}
		public static string GetProjectFileByLinkedFile(string file)
		{
			string prj;
			if (_fileProjectFile != null)
			{
				if (_fileProjectFile.TryGetValue(file, out prj))
				{
					return prj;
				}
			}
			return null;
		}
		/// <summary>
		/// {component file,classId}
		/// </summary>
		private static Dictionary<string, UInt32> _fileClassId;
		public static void SetFileClassIdLink(string file, UInt32 classId)
		{
			if (_fileClassId == null)
			{
				_fileClassId = new Dictionary<string, UInt32>();
			}
			if (_fileClassId.ContainsKey(file))
			{
				_fileClassId[file] = classId;
			}
			else
			{
				_fileClassId.Add(file, classId);
			}
		}
		public static UInt32 GetUnsavedClassId(string file)
		{
			if (_fileClassId != null)
			{
				UInt32 classId;
				if (_fileClassId.TryGetValue(file, out classId))
				{
					return classId;
				}
			}
			return 0;
		}
		public static IXmlDesignerLoader ActiveDesignerLoader
		{
			get
			{
				return _activeDesignerLoader;
			}
			set
			{
				_activeDesignerLoader = value;
			}
		}
		public static LimnorProject ActiveProject
		{
			get
			{
				return _activeProject;
			}
		}
		public static void SetActiveProject(LimnorProject project)
		{
			_activeProject = project;
		}
		public static void NotifyChanges()
		{
			if (_activeDesignerLoader != null)
			{
				_activeDesignerLoader.NotifyChanges();
			}
		}
		public static ProjectMainComponentAttribute GetMainComponentAttribute(string componentFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(componentFile);
			if (doc.DocumentElement != null)
			{
				Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement);
				if (t != null)
				{
					object[] attrs = t.GetCustomAttributes(typeof(ProjectMainComponentAttribute), true);
					if (attrs != null && attrs.Length > 0)
					{
						return (ProjectMainComponentAttribute)attrs[0];
					}
				}
			}
			return null;
		}
		public static ComponentID GetComponentID(LimnorProject project, string componentFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(componentFile);
			UInt32 classId = XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID);//"ClassID");
			if (classId == 0)
			{
				classId = GetUnsavedClassId(componentFile);
				if (classId == 0)
				{
					throw new ProjectException("Class id not found for componentFile [{0}].", componentFile);
				}
			}
			return new ComponentID(project, classId, GetAttribute(doc.DocumentElement, "name"), XmlUtil.GetLibTypeAttribute(doc.DocumentElement), componentFile);
		}
		private static string projectConfigFile(string prjFile)
		{
			if (string.IsNullOrEmpty(prjFile))
			{
				return null;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.vob", prjFile);
		}
		private static XmlDoc getVobDoc(string prjFile)
		{
			if (string.IsNullOrEmpty(prjFile))
			{
				throw new ProjectException("Calling getVobDoc with null project file");
			}
			else
			{
				string sFile = projectConfigFile(prjFile);
				XmlDocument doc = new XmlDocument();
				if (System.IO.File.Exists(sFile))
				{
					try
					{
						doc.Load(sFile);
					}
					catch
					{
					}
				}
				XmlNamespaceManager xm = new XmlNamespaceManager(doc.NameTable);
				return new XmlDoc(sFile, doc, xm);
			}
		}

		public static Guid GetProjectGuid(string prjFile)
		{
			if (string.IsNullOrEmpty(prjFile))
			{
				return Guid.Empty;
			}
			using (XmlDoc d = getVobDoc(prjFile))
			{
				XmlDocument doc = d.Doc;
				XmlNode node = null;
				if (doc.DocumentElement != null)
				{
					node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						 "{0}/{1}", XML_VobGroup, XML_ProjectGuid));
				}
				if (node == null)
				{
					XmlNode nodeGroup = createGroupNode(d);
					if (nodeGroup != null)
					{
						node = doc.CreateElement(XML_ProjectGuid);
						nodeGroup.AppendChild(node);
						Guid g = Guid.NewGuid();
						node.InnerText = g.ToString("D");
						d.Save();
						return g;
					}
				}
				return new Guid(node.InnerText);
			}
		}
		public static XmlNamespaceManager GetProjectNamespace()
		{
			XmlNamespaceManager xm = new XmlNamespaceManager(new NameTable());
			xm.AddNamespace("x", PRJNAMESPACEURI);
			return xm;
		}
		public static string GetProjectFileByComponentFile(string componentFile)
		{
			string componentFileName = System.IO.Path.GetFileName(componentFile);
			string key = componentFileName.ToLowerInvariant();
			string folder = System.IO.Path.GetDirectoryName(componentFile);
			string[] ss = System.IO.Directory.GetFiles(folder, "*." + PRJEXT);
			XmlNamespaceManager xm = new XmlNamespaceManager(new NameTable());
			xm.AddNamespace("x", PRJNAMESPACEURI);
			for (int i = 0; i < ss.Length; i++)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(ss[i]);
				XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"x:{0}/x:{1}[@{2}='{3}']", XML_ItemGroup, XML_Content, XMLATT_Include, componentFileName), xm);
				if (node != null)
				{
					return ss[i];
				}
				if (_fileRenames != null)
				{
					Guid gid = GetProjectGuid(ss[i]);
					Dictionary<string, string> mappings;
					if (_fileRenames.TryGetValue(gid, out mappings))
					{
						if (mappings.ContainsKey(key))
						{
							string newName = mappings[key];
							node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"x:{0}/x:{1}[@{2}='{3}']", XML_ItemGroup, XML_Content, XMLATT_Include, newName), xm);
							if (node != null)
							{
								return ss[i];
							}
						}
					}
				}
			}
			return null;
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					return xa.Value;
				}
			}
			return "";
		}
		public static IList<Guid> GetObjectGuidList(out object listOwner)
		{
			listOwner = _activeProject;
			if (_activeProject != null)
			{
				return _activeProject.GetObjectGuidList();
			}
			else
			{
				return null;
			}
		}
		public static void SetObjectGuidList(IList<Guid> list, object listOwner)
		{
			LimnorProject prj = listOwner as LimnorProject;
			if (prj != null)
			{
				prj.SetProjectObjectGuidList(list);
				if (DatabaseListChanged != null)
				{
					DatabaseListChanged(prj, EventArgs.Empty);
				}
			}
		}
		public static void MergeObjectGuidList(IList<Guid> list)
		{
			if (_activeProject != null)
			{
				_activeProject.MergeProjectObjectGuidList(list);
			}
		}
		public static void DeleteVobFile(string prjFile)
		{
			string vob = LimnorProject.projectConfigFile(prjFile);
			if (File.Exists(vob))
			{
				File.Delete(vob);
			}
		}
		#endregion
		#region Main Project File (for Mono)
		private XmlNode GetProjectRootXmlNode()
		{
			if (_mainDocument == null)
			{
				SetAsMainProjectFile();
			}
			return _rootNode;
		}
		public void SetAsMainProjectFile()
		{
			_isMainProjectFile = true;
			_mainDocument = new XmlDocument();
			_mainDocument.Load(ProjectFile);
			_rootNode = _mainDocument.DocumentElement;
			if (_rootNode == null)
			{
				throw new ProjectException("Invalid project xml file. DocumentElement not found");
			}

		}
		public IList<ClassData> GetAllClasses()
		{
			List<ClassData> classes = GetTypedProjectData<List<ClassData>>();
			if (classes == null)
			{
				List<ClassData> cls = new List<ClassData>();
				XmlNodeList list = _rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}/x:{1}[@{2}]", XML_ItemGroup, XML_Content, XMLATT_Include), xnm);
				for (int i = 0; i < list.Count; i++)
				{
					string s = System.IO.Path.Combine(ProjectFolder, GetAttribute(list[i], XMLATT_Include));
					if (System.IO.File.Exists(s))
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(s);
						ClassData c = new ClassData(this, list[i], doc, s);
						cls.Add(c);
					}
				}
				classes = cls;
				SetTypedProjectData<List<ClassData>>(cls);
			}
			return classes;
		}
		/// <summary>
		/// return null if exists
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public ClassData AddClass(string filename, bool isNewFile)
		{
			IList<ClassData> classes = GetAllClasses();
			foreach (ClassData c in classes)
			{
				string s = Path.GetFileName(c.ComponentFile);
				if (string.Compare(s, filename, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return null;
				}
			}
			XmlNode itemGroup = getProjectRootNode().SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}", XML_ItemGroup), xnm);
			if (itemGroup == null)
			{
				itemGroup = _rootNode.OwnerDocument.CreateElement(XML_ItemGroup, PRJNAMESPACEURI);
				_rootNode.AppendChild(itemGroup);
			}
			XmlNode itemNode = itemGroup.OwnerDocument.CreateElement(XML_Content, PRJNAMESPACEURI);
			itemGroup.AppendChild(itemNode);
			XmlUtil.SetAttribute(itemNode, XMLATT_Include, filename);
			XmlNode subNode = itemNode.OwnerDocument.CreateElement("SubType", PRJNAMESPACEURI);
			itemNode.AppendChild(subNode);
			subNode.InnerText = "Code";
			string path = Path.Combine(ProjectFolder, filename);
			if (!File.Exists(path))
			{
				throw new ProjectException("File not found: {0}", path);
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			if (doc.DocumentElement == null)
			{
				throw new ProjectException("Invalid class file: {0}", path);
			}
			if (isNewFile)
			{
				XmlUtil.SetAttribute(doc.DocumentElement, XmlTags.XMLATT_ClassID, (UInt32)(Guid.NewGuid().GetHashCode()));
				string sType = XmlUtil.GetLibTypeAttributeString(doc.DocumentElement);
				string subType;
				Type t;
				if (XmlUtil.TryGetTypeNameFromXClass(sType, out subType))
				{
					sType = subType;
				}
				if (XmlUtil.TryGetKnownType(sType, out t))
				{
					t = VPLUtil.GetObjectType(t);
					if (!t.Assembly.GlobalAssemblyCache)
					{
						if (t.Assembly.Location.StartsWith(AppDomain.CurrentDomain.BaseDirectory, StringComparison.OrdinalIgnoreCase))
						{
							XmlNode tNode = doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
								"Types/{0}[@{1}='{2}']",
								XmlTags.XML_Item, XmlTags.XMLATT_type, sType));
							if (tNode != null)
							{
								XmlNode pNode = tNode.ParentNode;
								pNode.RemoveChild(tNode);
							}
						}
					}
				}
				doc.Save(path);
			}
			ClassData cl = new ClassData(this, itemNode, doc, path);
			classes.Add(cl);
			Save();
			return cl;
		}
		public void RemoveClass(string filename)
		{
			XmlNode itemGroup = getProjectRootNode().SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}", XML_ItemGroup), xnm);
			if (itemGroup != null)
			{
				//[@{1}='{2}'], XMLATT_Include,filename
				XmlNodeList itemNodes = itemGroup.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}", XML_Content), xnm);
				if (itemNodes != null)
				{
					List<ClassData> classes = GetTypedProjectData<List<ClassData>>();
					foreach (XmlNode itemNode in itemNodes)
					{
						string s = XmlUtil.GetAttribute(itemNode, XMLATT_Include);
						if (string.Compare(filename, s, StringComparison.OrdinalIgnoreCase) == 0)
						{
							itemGroup.RemoveChild(itemNode);
							if (classes != null)
							{
								foreach (ClassData c in classes)
								{
									if (string.Compare(Path.GetFileName(c.ComponentFile), filename, StringComparison.OrdinalIgnoreCase) == 0)
									{
										classes.Remove(c);
										break;
									}
								}
							}
							Save();
							break;
						}
					}
				}
			}
		}
		public void RenameClass(string filename, string newName)
		{
			XmlNode itemGroup = getProjectRootNode().SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}", XML_ItemGroup), xnm);
			if (itemGroup != null)
			{//[@{1}='{2}'], XMLATT_Include,filename
				XmlNodeList itemNodes = itemGroup.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"x:{0}", XML_Content), xnm);
				if (itemNodes != null)
				{
					foreach (XmlNode itemNode in itemNodes)
					{
						string s = XmlUtil.GetAttribute(itemNode, XMLATT_Include);
						if (string.Compare(filename, s, StringComparison.OrdinalIgnoreCase) == 0)
						{
							XmlUtil.SetAttribute(itemNode, XMLATT_Include, newName);
							Save();
							break;
						}
					}
				}
			}
		}
		public void RenameClassFile(string oldFile, string newFile)
		{
			XmlNodeList nodes = _mainDocument.SelectNodes(string.Format(CultureInfo.InvariantCulture, "//x:Content[@Include='{0}']", Path.GetFileName(oldFile)), xnm);
			string fn = Path.GetFileName(newFile);
			foreach (XmlNode nd in nodes)
			{
				XmlUtil.SetAttribute(nd, "Include", fn);
			}
		}
		public void Save()
		{
			_mainDocument.Save(ProjectFile);
			ResetChanged();
			SaveModifiedClassIDs(false);
		}
		public string MainFile
		{
			get
			{
				XmlNode node = getProjectRootNode().SelectSingleNode("x:PropertyGroup/x:MainFile", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public Dictionary<string, string[]> LicenseFiles
		{
			get
			{
				using (XmlDoc doc = getVobDoc(this.ProjectFile))
				{
					XmlNode nodeRoot = doc.Doc.DocumentElement;
					if (nodeRoot != null)
					{
						XmlNode node = nodeRoot.SelectSingleNode("LicenseFiles");
						if (node != null)
						{
							Dictionary<string, string[]> files = new Dictionary<string, string[]>();
							XmlNodeList nds = node.SelectNodes(XmlTags.XML_Item);
							foreach (XmlNode nd in nds)
							{
								string key = XmlUtil.GetAttribute(nd, "AssemblyFile");
								if (!string.IsNullOrEmpty(key))
								{
									List<string> ls = new List<string>();
									XmlNodeList ndFs = nd.SelectNodes(XmlTags.XML_File);
									foreach (XmlNode nf in ndFs)
									{
										if (!string.IsNullOrEmpty(nf.InnerText))
										{
											ls.Add(nf.InnerText);
										}
									}
									files.Add(key, ls.ToArray());
								}
							}
							return files;
						}
					}
				}
				return null;
			}
		}
		public void SetLicenseFiles(Dictionary<string, string[]> files)
		{
			using (XmlDoc doc = getVobDoc(this.ProjectFile))
			{
				XmlNode nodeRoot;
				if (doc.Doc.DocumentElement == null)
				{
					nodeRoot = doc.Doc.CreateElement(XML_Project);
					doc.Doc.AppendChild(nodeRoot);
				}
				else
				{
					nodeRoot = doc.Doc.DocumentElement;
				}
				XmlNode node = XmlUtil.CreateSingleNewElement(nodeRoot, "LicenseFiles");
				node.RemoveAll();
				if (files != null)
				{
					foreach (KeyValuePair<string, string[]> kv in files)
					{
						XmlNode nd = doc.Doc.CreateElement(XmlTags.XML_Item);
						XmlUtil.SetAttribute(nd, "AssemblyFile", kv.Key);
						node.AppendChild(nd);
						if (kv.Value != null)
						{
							for (int i = 0; i < kv.Value.Length; i++)
							{
								XmlNode ndf = doc.Doc.CreateElement(XmlTags.XML_File);
								ndf.InnerText = kv.Value[i];
								nd.AppendChild(ndf);
							}
						}
					}
				}
				doc.Save();
			}
			saveChanged();
		}
		public string Namespace
		{
			get
			{
				XmlNode node = getProjectRootNode().SelectSingleNode("x:PropertyGroup/x:RootNamespace", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}

		public void SetProjectNamespace(string name)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:RootNamespace", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("RootNamespace", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = name;
			saveChanged();
		}
		public string AssemblyName
		{
			get
			{
				XmlNode node = getProjectRootNode().SelectSingleNode("x:PropertyGroup/x:AssemblyName", xnm);
				if (node == null)
				{
					throw new ProjectException("Invalid project xml file. AssemblyName not found");
				}
				if (!string.IsNullOrEmpty(node.InnerText))
				{
					if (node.InnerText.IndexOf(' ') >= 0)
					{
						node.InnerText = node.InnerText.Replace(' ', '_');
					}
				}
				return node.InnerText;
			}
		}
		public void SetProjectAssemblyName(string name)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyName", xnm);
			if (node == null)
			{
				throw new ProjectException("Invalid project xml file. AssemblyName not found");
			}
			node.InnerText = name;
			saveChanged();
		}
		public void SetProjectGuid(Guid g)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:ProjectGuid", xnm);
			if (node == null)
			{
				throw new ProjectException("Invalid project xml file. ProjectGuid not found");
			}
			node.InnerText = g.ToString("B", CultureInfo.InvariantCulture);
			saveChanged();
		}
		public string AssemblyProduct
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyProduct", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyProduct(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyProduct", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyProduct", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}

		public string AssemblyTitle
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyTitle", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyTitle(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyTitle", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyTitle", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}

		public string AssemblyCompany
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyCompany", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyCompany(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyCompany", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyCompany", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}

		public string AssemblyCopyright
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyCopyright", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyCopyright(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyCopyright", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyCopyright", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}

		public string AssemblyDescription
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyDescription", xnm);
				if (node == null)
				{
					return string.Empty;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyDescription(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyDescription", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyDescription", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}

		public string AssemblyFileVersion
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ProjectFile))
				{
					using (XmlDoc d = getVobDoc(this.ProjectFile))
					{
						XmlDocument doc = d.Doc;
						if (doc.DocumentElement != null)
						{
							XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}", XML_VobGroup, XML_AssemblyFileVersion));
							if (node != null)
							{
								return XmlUtil.GetAttribute(node, XMLATT_Version);
							}
						}
					}
				}
				return null;
			}
		}
		public void SetAssemblyFileVersion(string value)
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}", XML_VobGroup, XML_AssemblyFileVersion));
					if (node == null)
					{
						XmlNode nodeG = doc.DocumentElement.SelectSingleNode(XML_VobGroup);
						if (nodeG == null)
						{
							nodeG = doc.CreateElement(XML_VobGroup);
							doc.DocumentElement.AppendChild(nodeG);
						}
						node = doc.CreateElement(XML_AssemblyFileVersion);
						nodeG.AppendChild(node);
					}
					XmlUtil.SetAttribute(node, XMLATT_Version, value);
					d.Save();
				}
			}
		}
		public bool WebFolderGranted
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ProjectFile))
				{
					using (XmlDoc d = getVobDoc(this.ProjectFile))
					{
						XmlDocument doc = d.Doc;
						if (doc.DocumentElement != null)
						{
							XmlNode node = doc.DocumentElement.SelectSingleNode(XML_VobGroup);
							if (node != null)
							{
								return XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_WebFolderGranted);
							}
						}
					}
				}
				return true;
			}
		}
		public void SetWebFolderGranted()
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNode nodeG = doc.DocumentElement.SelectSingleNode(XML_VobGroup);
					if (nodeG == null)
					{
						nodeG = doc.CreateElement(XML_VobGroup);
						doc.DocumentElement.AppendChild(nodeG);
					}
					XmlUtil.SetAttribute(nodeG, XMLATT_WebFolderGranted, true);
					d.Save();
				}
			}
		}
		public string IncreaseAssemblyFileVersion()
		{
			string fv = AssemblyFileVersion;
			if (string.IsNullOrEmpty(fv))
			{
				fv = "1.0.0.0";
			}
			else
			{
				string[] ss = fv.Split('.');
				int verMajor = 1;
				int verMin = 0;
				int revMajor = 0;
				int revMin = 0;
				if (ss.Length > 0)
				{
					if (!int.TryParse(ss[0], out verMajor))
					{
						verMajor = 1;
					}
					else
					{
						if (verMajor < 0)
							verMajor = 0;
					}
					if (ss.Length > 1)
					{
						if (!int.TryParse(ss[1], out verMin))
						{
							verMin = 0;
						}
						else
						{
							if (verMin < 0)
								verMin = 0;
						}
						if (ss.Length > 2)
						{
							if (!int.TryParse(ss[2], out revMajor))
							{
								revMajor = 0;
							}
							else
							{
								if (revMajor < 0)
									revMajor = 0;
							}
							if (ss.Length > 3)
							{
								if (!int.TryParse(ss[3], out revMin))
								{
									revMin = 0;
								}
								else
								{
									if (revMin < 0)
										revMin = 0;
								}
							}
						}
					}
				}
				if (revMin > 998)
				{
					revMin = 0;
					if (revMajor > 998)
					{
						revMajor = 0;
						if (verMin > 998)
						{
							verMin = 0;
							verMajor++;
						}
						else
						{
							verMin++;
						}
					}
					else
					{
						revMajor++;
					}
				}
				else
				{
					revMin++;
				}
				fv = string.Format(CultureInfo.InvariantCulture,
					"{0}.{1}.{2}.{3}", verMajor, verMin, revMajor, revMin);
			}
			SetAssemblyFileVersion(fv);
			return fv;
		}
		public string AssemblyVersion
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyVersion", xnm);
				if (node == null)
				{
					return null;
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyVersion(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyVersion", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyVersion", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}
		public string AssemblyKeyFile
		{
			get
			{
				XmlNode node = GetProjectRootXmlNode().SelectSingleNode("x:PropertyGroup/x:AssemblyKeyFile", xnm);
				if (node == null)
				{
					return "";
				}
				return node.InnerText;
			}
		}
		public void SetAssemblyKeyFile(string value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyKeyFile", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyKeyFile", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value;
			saveChanged();
		}
		public bool AssemblyDelaySign
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyDelaySign", xnm);
				if (node == null)
				{
					return false;
				}
				return bool.Parse(node.InnerText);
			}
		}
		public void SetAssemblyDelaySign(bool value)
		{
			XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:AssemblyDelaySign", xnm);
			if (node == null)
			{
				XmlNode pg = _rootNode.SelectSingleNode("x:PropertyGroup", xnm);
				if (pg == null)
				{
					throw new ProjectException("Invalid project xml file. PropertyGroup not found");
				}
				node = _mainDocument.CreateElement("AssemblyDelaySign", PRJNAMESPACEURI);
				pg.AppendChild(node);
			}
			node.InnerText = value.ToString(CultureInfo.InvariantCulture);
			saveChanged();
		}
		public AssemblyTargetPlatform TargetPlatform
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:PlatformTarget", xnm);
				if (node == null)
				{
					return AssemblyTargetPlatform.AnyCPU;
				}
				string s = node.InnerText.Trim();
				if (s.Length == 0)
				{
					return AssemblyTargetPlatform.AnyCPU;
				}
				return (AssemblyTargetPlatform)Enum.Parse(typeof(AssemblyTargetPlatform), s, true);
			}
		}
		public void SetTargetPlatform(AssemblyTargetPlatform value)
		{
			XmlNodeList pgs = _rootNode.SelectNodes("x:PropertyGroup[@Condition]", xnm);
			if (pgs == null || pgs.Count == 0)
			{
				throw new ProjectException("Invalid project xml file. PropertyGroup[@Condition] not found");
			}
			string sv = value.ToString();
			foreach (XmlNode pg in pgs)
			{
				XmlNode node = pg.SelectSingleNode("x:PlatformTarget", xnm);
				if (node == null)
				{
					node = _mainDocument.CreateElement("PlatformTarget", PRJNAMESPACEURI);
					pg.AppendChild(node);
					node.InnerText = sv;
					saveChanged();
				}
				else
				{
					if (string.CompareOrdinal(node.InnerText.Trim(), sv) != 0)
					{
						node.InnerText = sv;
						saveChanged();
					}
				}
			}
		}
		public AssemblyTargetFramework AssemblyTargetFramework
		{
			get
			{
				return GetTargetFrameworkEnum(TargetFramework);
			}
		}
		public string TargetFramework
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:TargetFrameworkVersion", xnm);
				if (node == null)
				{
					return "v4.0";
				}
				string s = node.InnerText.Trim();
				if (s.Length == 0)
				{
					return "v4.0";
				}
				return s;
			}
		}
		public void SetTargetFramework(string value)
		{
			XmlNodeList pgs = _rootNode.SelectNodes("x:PropertyGroup", xnm);
			if (pgs == null || pgs.Count == 0)
			{
				throw new ProjectException("Invalid project xml file. PropertyGroup not found");
			}
			string sv = value.Trim();
			foreach (XmlNode pg in pgs)
			{
				if (!XmlUtil.HasAttribute(pg, "Condition"))
				{
					XmlNode node = pg.SelectSingleNode("x:TargetFrameworkVersion", xnm);
					if (node == null)
					{
						node = _mainDocument.CreateElement("TargetFrameworkVersion", PRJNAMESPACEURI);
						pg.AppendChild(node);
						node.InnerText = sv;
						saveChanged();
					}
					else
					{
						if (string.CompareOrdinal(node.InnerText.Trim(), sv) != 0)
						{
							node.InnerText = sv;
							saveChanged();
						}
					}
				}
			}
		}
		public string OutputType
		{
			get
			{
				XmlNode node = _rootNode.SelectSingleNode("x:PropertyGroup/x:OutputType", xnm);
				if (node == null)
				{
					return "WinExe";
				}
				return node.InnerText;
			}
		}
		public string GetOutputPath(string config)
		{
			XmlNodeList pgs = _rootNode.SelectNodes("x:PropertyGroup[@Condition]", xnm);
			if (pgs == null || pgs.Count == 0)
			{
				throw new ProjectException("Invalid project xml file. PropertyGroup[@Condition] not found");
			}
			foreach (XmlNode pg in pgs)
			{
				string condition = XmlUtil.GetAttribute(pg, "Condition");
				if (!string.IsNullOrEmpty(condition))
				{
					if (condition.IndexOf(config, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						XmlNode node = pg.SelectSingleNode("x:OutputPath", xnm);
						if (node != null)
						{
							string s = node.InnerText.Trim();
							if (!string.IsNullOrEmpty(s))
							{
								return s;
							}
						}
						break;
					}
				}

			}
			return string.Format(CultureInfo.InvariantCulture, "bin\\{0}", config);
		}
		public bool Changed
		{
			get
			{
				return _changed;
			}
		}
		public void ResetChanged()
		{
			_changed = false;
		}
		private void saveChanged()
		{
			if (_mainDocument != null)
			{
				_mainDocument.Save(ProjectFile);
			}
			else
			{
				_changed = true;
			}
		}
		public string GetProgramFullpath(bool debug)
		{
			string ext;
			if (ProjectType == EnumProjectType.ClassDLL)
				ext = ".dll";
			else
				ext = ".exe";
			string mode;
			if (debug)
				mode = "Debug";
			else
				mode = "Release";
			string file = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"bin\\{0}\\{1}{2}", mode, AssemblyName, ext);
			return Path.Combine(Path.GetDirectoryName(ProjectFile), file);
		}
		#endregion
		#region Properties
		const string start_file = "StartFile";
		public string StartFile
		{
			get
			{
				return GetProjectNamedData<string>(start_file);
			}
			set
			{
				SetProjectNamedData<string>(start_file, value);
			}
		}

		public string GetWebSiteName(Form caller)
		{
			VirtualWebDir web = GetTestWebSite(caller);
			if (web != null)
			{
				return web.WebName;
			}
			return null;
		}
		public string GetTestUrl(Form owner, string pageName)
		{
			VirtualWebDir web = GetTestWebSite(owner);
			if (web != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "http://localhost/{0}/{1}?debugRef={2}", web.WebName, pageName, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			}
			return null;
		}
		public string WebPhysicalFolder(Form caller)
		{
			VirtualWebDir web = GetTestWebSite(caller);
			if (web != null)
			{
				return web.PhysicalDirectory;
			}
			return Path.Combine(ProjectFolder, "WebFiles");
		}
		/// <summary>
		/// should be set by IDE
		/// </summary>
		public AssemblyTargetPlatform DefaultAssemblyTargetPlatform
		{
			get
			{
				if (HasTypedProjectData<AssemblyTargetPlatform>())
				{
					return GetTypedProjectData<AssemblyTargetPlatform>();
				}
				return AssemblyTargetPlatform.AnyCPU;
			}
			set
			{
				SetTypedProjectData<AssemblyTargetPlatform>(value);
			}
		}
		private bool _d;
		public bool Debugging { get { return _d; } set { _d = value; } }
		public string ProjectName
		{
			get
			{
				if (_isMainProjectFile)
				{
					return AssemblyName;
				}
				XmlDocument doc;
				XmlNamespaceManager xm;
				if (_mainDocument != null)
				{
					doc = _mainDocument;
					xm = xnm;
				}
				else
				{
					doc = new XmlDocument();
					doc.Load(ProjectFile);
					xm = new XmlNamespaceManager(doc.NameTable);
					xm.AddNamespace("x", PRJNAMESPACEURI);
				}
				XmlNode node = doc.SelectSingleNode("//x:PropertyGroup/x:AssemblyName", xm);
				return node.InnerText;
			}
		}
		public string ProjectAssemblyName
		{
			get
			{
				return ProjectName.Replace(' ', '_');
			}
		}
		public string StartupComponentFile
		{
			get
			{
				if (ProjectType != EnumProjectType.Unknown)
				{
					return _mainComponent.MainComponentFile;
				}
				return null;
			}
		}
		public string CommandArgumentsForDebug
		{
			get
			{
				string startFile = StartupComponentFile;
				if (!string.IsNullOrEmpty(startFile))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(startFile);
					XmlNode node = doc.DocumentElement.SelectSingleNode("Property[@name='CommandArgumentsForDebug']");
					if (node != null)
					{
						return node.InnerText;
					}
				}
				return "";
			}
		}
		private void initializeMainComponent()
		{
			if (_mainComponent == null)
			{
				string[] ss = GetComponentFiles();
				for (int i = 0; i < ss.Length; i++)
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(ss[i]);
					if (doc.DocumentElement != null)
					{
						if (XmlUtil.GetAttributeBoolDefFalse(doc.DocumentElement, XmlTags.XMLATT_isSetup))
						{
							_mainComponent = new ProjectMainComponent(ss[i], typeof(void), EnumProjectType.Setup, doc.DocumentElement);
							break;
						}
						else
						{
							Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement);
							if (t != null)
							{
								try
								{
									object[] attrs = t.GetCustomAttributes(typeof(ProjectMainComponentAttribute), true);
									if (attrs != null && attrs.Length > 0)
									{
										ProjectMainComponentAttribute a = (ProjectMainComponentAttribute)attrs[0];
										_mainComponent = new ProjectMainComponent(ss[i], t, a.ProjectType, doc.DocumentElement);
										break;
									}
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			if (_mainComponent == null)
			{
				_mainComponent = new ProjectMainComponent();
			}
		}

		public EnumProjectType ProjectType
		{
			get
			{
				initializeMainComponent();
				if (_mainComponent == null)
				{
					return EnumProjectType.ClassDLL;
				}
				return _mainComponent.ProjectType;
			}
		}
		public bool IsWinFormApp
		{
			get
			{
				EnumProjectType pt = ProjectType;
				return IsWinformApp(pt);
			}
		}
		public bool IsWebApplication
		{
			get
			{
				EnumProjectType p = ProjectType;
				return (p == EnumProjectType.WebAppAspx || p == EnumProjectType.WebAppPhp);
			}
		}
		public ProjectMainComponent MainComponent
		{
			get
			{
				return _mainComponent;
			}
		}
		public UInt32 MainClassId
		{
			get
			{
				initializeMainComponent();
				return _mainComponent.MainClassId;
			}
		}
		public UInt32 StartPageClassId
		{
			get
			{
				initializeMainComponent();
				return _mainComponent.StartPageClassId;
			}
		}
		public UInt32 StartPageMemberId
		{
			get
			{
				initializeMainComponent();
				return _mainComponent.StartPageMemberId;
			}
		}
		public Guid ProjectGuid
		{
			get
			{
				return _guid;
			}
		}

		public string ProjectFile
		{
			get
			{
				if (!string.IsNullOrEmpty(_prjFile))
				{
					while (!System.IO.File.Exists(_prjFile))
					{
						string s;
						if (TryGetRenamedFilenameByOldName(_prjFile, out s))
						{
							_prjFile = s;
						}
						else
						{
							break;
						}
					}
				}
				return _prjFile;
			}
			set
			{
				_prjFile = value;
				_guid = GetProjectGuid();
				loadProjectProperties();
				//
			}
		}
		public bool TraceMethodLoading
		{
			get
			{
				return _properties.TraceMethodLoad;
			}
		}
		public string ProjectFolder
		{
			get
			{
				if (string.IsNullOrEmpty(_folder))
				{
					_folder = System.IO.Path.GetDirectoryName(ProjectFile);
				}
				return _folder;
			}
		}
		public int TraceLevel
		{
			get
			{
				if (string.IsNullOrEmpty(this.ProjectFile))
					return 0;
				using (XmlDoc doc = getVobDoc(this.ProjectFile))
				{
					if (doc.Doc.DocumentElement != null)
					{
						XmlNode nodeGroup = doc.Doc.DocumentElement.SelectSingleNode(XML_VobGroup);
						if (nodeGroup != null)
						{
							return XmlUtil.GetAttributeInt(nodeGroup, XMLATT_TraceLevel);
						}
					}
				}
				return 0;
			}
		}
		public bool IsNewProject
		{
			get
			{
				if (string.IsNullOrEmpty(this.ProjectFile))
					return true;
				using (XmlDoc doc = getVobDoc(this.ProjectFile))
				{
					if (doc.Doc.DocumentElement != null)
					{
						return !XmlUtil.GetAttributeBoolDefFalse(doc.Doc.DocumentElement, XMLATT_isNotNew);
					}
				}
				return true;
			}
		}
		private bool _filemapperread;
		private bool _enableFileMapper;
		public bool EnableFileMapper
		{
			get
			{
				if (_filemapperread)
					return _enableFileMapper;
				_enableFileMapper = false;
				_filemapperread = true;
				if (string.IsNullOrEmpty(this.ProjectFile))
					return false;
				using (XmlDoc doc = getVobDoc(this.ProjectFile))
				{
					if (doc.Doc.DocumentElement != null)
					{
						XmlNode nodeGroup = doc.Doc.DocumentElement.SelectSingleNode(XML_VobGroup);
						if (nodeGroup != null)
						{
							_enableFileMapper = XmlUtil.GetAttributeBoolDefFalse(nodeGroup, XMLATT_EnableFileMapper);
						}
					}
				}
				return _enableFileMapper;
			}
			set
			{
				_filemapperread = false;
				if (!string.IsNullOrEmpty(this.ProjectFile))
				{
					using (XmlDoc doc = getVobDoc(this.ProjectFile))
					{
						XmlNode groupNode = createGroupNode(doc);
						XmlUtil.SetAttribute(groupNode, XMLATT_EnableFileMapper, value);
						doc.Save();
					}
				}
			}
		}
		#endregion
		#region Peivate Methods
		private XmlNode getProjectRootNode()
		{
			if (_rootNode == null)
			{
				_mainDocument = new XmlDocument();
				_mainDocument.Load(ProjectFile);
				_rootNode = _mainDocument.DocumentElement;
				if (_rootNode == null)
				{
					throw new ProjectException("Invalid project xml file. Roort node not found");
				}
			}
			return _rootNode;
		}
		private Guid GetProjectGuid()
		{
			return GetProjectGuid(ProjectFile);
		}
		private void loadProjectProperties()
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement != null)
					{
						XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}/{1}", XML_VobGroup, XML_ProjectGuid));//, d.NamespaceManager);
						_properties.TraceMethodLoad = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_TraceMethodLoad);
					}
				}
			}
		}
		public static XmlNode createVobRootNode(XmlDoc d)
		{
			if (d != null && d.Doc != null)
			{
				XmlNode nodeRoot = d.Doc.DocumentElement;
				if (nodeRoot == null)
				{
					nodeRoot = d.Doc.CreateElement(XML_Project);
					d.Doc.AppendChild(nodeRoot);
					XmlUtil.SetAttribute(nodeRoot, XMLATT_ActiveConfig, "Release");
					d.Doc.Save(d.FilePath);
				}
				return nodeRoot;
			}
			return null;
		}
		private static XmlNode createGroupNode(XmlDoc doc)
		{
			XmlNode root = createVobRootNode(doc);
			if (root != null)
			{
				XmlNode nodeGroup = root.SelectSingleNode(XML_VobGroup);
				if (nodeGroup == null)
				{
					nodeGroup = root.OwnerDocument.CreateElement(XML_VobGroup);
					root.AppendChild(nodeGroup);
					XmlUtil.SetAttribute(nodeGroup, XMLATT_TraceLevel, 0);
				}
				return nodeGroup;
			}
			return null;
		}
		#endregion
		#region Public Methods
		public VirtualWebDir VerifyWebSite(Form caller)
		{
			if (this.IsWebApplication)
			{
				string wn = null;
				VirtualWebDir web = GetTestWebSite(caller);
				if (web != null)
				{
					if (!string.IsNullOrEmpty(web.WebName))
					{
						wn = web.WebName;
						if (!string.IsNullOrEmpty(web.PhysicalDirectory))
						{
							if (!VirtualWebDir.IsNetworkDrive(web.PhysicalDirectory))
							{
								return web;
							}
						}
					}
				}
				if (string.IsNullOrEmpty(wn))
				{
					wn = this.ProjectName;
				}
				DialogProjectOutput dlg = new DialogProjectOutput();
				dlg.LoadData(this, wn);
				if (dlg.ShowDialog(caller) == DialogResult.OK)
				{
					SetTypedProjectData<VirtualWebDir>(dlg.WebSite);
					return dlg.WebSite;
				}
				else
				{
					MessageBox.Show(caller, "A web project cannot be properly developed without a web site on local computer", "Web Project", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}
			}
			else
			{
				return null;
			}
		}
		public StringCollection GetOutputFolderList()
		{
			StringCollection lst = new StringCollection();
			IList<OutputFolder> fs = GetOutputFolders();
			foreach (OutputFolder f in fs)
			{
				if (!f.Disabled)
				{
					if (!string.IsNullOrEmpty(f.Folder))
					{
						string s = f.Folder.ToLowerInvariant();
						if (!lst.Contains(s))
						{
							lst.Add(s);
						}
					}
				}
			}
			return lst;
		}
		const string XML_OutputFolders = "OutputFolders";
		const string XMLATT_disabled = "disabled";
		const string XMLATT_folder = "folder";
		public IList<OutputFolder> GetOutputFolders()
		{
			List<OutputFolder> lst = new List<OutputFolder>();
			using (XmlDoc d = GetVob())
			{
				XmlNode wroot = getMachineNode(d);
				if (wroot != null)
				{
					XmlNodeList nds = wroot.SelectNodes(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", XML_OutputFolders, XmlTags.XML_Item));
					foreach (XmlNode nd in nds)
					{
						lst.Add(new OutputFolder(XmlUtil.GetAttributeBoolDefFalse(nd, XMLATT_disabled), XmlUtil.GetAttribute(nd, XMLATT_folder)));
					}
				}
			}
			return lst;
		}
		public void SetOutputFolders(List<OutputFolder> lst)
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					if (d.Doc != null)
					{
						XmlNode node = null;
						string cn0 = Environment.MachineName;
						XmlNode oNW = XmlUtil.CreateSingleNewElement(d.Doc.DocumentElement, XML_Webs);
						XmlNodeList nds = oNW.SelectNodes(XmlTags.XML_Item);
						foreach (XmlNode nd in nds)
						{
							string cn = XmlUtil.GetNameAttribute(nd);
							if (string.Compare(cn0, cn, StringComparison.OrdinalIgnoreCase) == 0)
							{
								node = nd;
								break;
							}
						}
						if (node == null)
						{
							node = d.Doc.CreateElement(XmlTags.XML_Item);
							oNW.AppendChild(node);
							XmlUtil.SetNameAttribute(node, cn0);
						}
						XmlNode oN = XmlUtil.CreateSingleNewElement(node, XML_OutputFolders);
						oN.RemoveAll();
						foreach (OutputFolder f in lst)
						{
							XmlNode nd = d.Doc.CreateElement(XmlTags.XML_Item);
							oN.AppendChild(nd);
							if (f.Disabled)
							{
								XmlUtil.SetAttribute(nd, XMLATT_disabled, f.Disabled);
							}
							XmlUtil.SetAttribute(nd, XMLATT_folder, f.Folder);
						}
						d.Save();
					}
				}
			}
		}
		const string XML_Webs = "Webs";
		const string XMLATT_webName = "webName";
		private XmlNode getMachineNode(XmlDoc d)
		{
			XmlNode root = d.Doc.DocumentElement;
			if (root != null)
			{
				string cn0 = Environment.MachineName;
				XmlNodeList nds = root.SelectNodes(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", XML_Webs, XmlTags.XML_Item));
				foreach (XmlNode nd in nds)
				{
					string cn = XmlUtil.GetNameAttribute(nd);
					if (string.Compare(cn0, cn, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return nd;
					}
				}
			}
			return null;
		}
		public VirtualWebDir GetTestWebSite(Form owner)
		{
			if (this.IsWebApplication)
			{
				VirtualWebDir ret = GetTypedProjectData<VirtualWebDir>();
				if (ret == null)
				{
					using (XmlDoc d = GetVob())
					{
						XmlNode nd = getMachineNode(d);
						if (nd != null)
						{
							VirtualWebDir vw = new VirtualWebDir(XmlUtil.GetAttribute(nd, XMLATT_webName), XmlUtil.GetAttribute(nd, XMLATT_folder));
							SetTypedProjectData<VirtualWebDir>(vw);
							return vw;
						}
						if (!VirtualWebDir.IsNetworkDrive(this.ProjectFile))
						{
							if (_mainComponent != null)
							{
								string wn = GetWebSiteName(_mainComponent.MainComponentNode);
								if (!string.IsNullOrEmpty(wn))
								{
									bool bErr;
									VirtualWebDir w = IisUtility.FindLocalWebSiteByName(owner, wn, out bErr);
									SetTypedProjectData<VirtualWebDir>(w);
									return w;
								}
							}
						}
					}
				}
				else
				{
					return ret;
				}
			}
			return null;
		}
		public void SaveTestWebSite(VirtualWebDir web)
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				SetTypedProjectData<VirtualWebDir>(web);
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					if (d.Doc != null)
					{
						XmlNode node = null;
						string cn0 = Environment.MachineName;
						XmlNode oN = XmlUtil.CreateSingleNewElement(d.Doc.DocumentElement, XML_Webs);
						XmlNodeList nds = oN.SelectNodes(XmlTags.XML_Item);
						foreach (XmlNode nd in nds)
						{
							string cn = XmlUtil.GetNameAttribute(nd);
							if (string.Compare(cn0, cn, StringComparison.OrdinalIgnoreCase) == 0)
							{
								node = nd;
								break;
							}
						}
						if (node == null)
						{
							node = d.Doc.CreateElement(XmlTags.XML_Item);
							oN.AppendChild(node);
						}
						XmlUtil.SetNameAttribute(node, cn0);
						XmlUtil.SetAttribute(node, XMLATT_webName, web.WebName);
						XmlUtil.SetAttribute(node, XMLATT_folder, web.PhysicalDirectory);
						d.Save();
					}
				}
			}
		}
		public override string ToString()
		{
			return this.ProjectName;
		}
		public void MergeProjectObjectGuidList(IList<Guid> list)
		{
			if (list != null && list.Count > 0)
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlNode xmlData = d.Doc.DocumentElement;
					if (xmlData == null)
					{
						xmlData = d.Doc.CreateElement(XML_Project);
						d.Doc.AppendChild(xmlData);
					}
					XmlNode connectionsNode = xmlData.SelectSingleNode(XmlTags.XML_DbConnectionList);
					if (connectionsNode == null)
					{
						connectionsNode = d.Doc.CreateElement(XmlTags.XML_DbConnectionList);
						xmlData.AppendChild(connectionsNode);
						foreach (Guid g in list)
						{
							if (g != Guid.Empty)
							{
								XmlNode gNode = d.Doc.CreateElement(XmlTags.XML_Item);
								connectionsNode.AppendChild(gNode);
								XmlUtil.SetAttribute(gNode, XmlTags.XMLATT_guid, g.ToString("D"));
							}
						}
					}
					else
					{
						XmlNodeList nl = connectionsNode.SelectNodes(XmlTags.XML_Item);
						List<Guid> gl = new List<Guid>();
						foreach (XmlNode nd in nl)
						{
							gl.Add(XmlUtil.GetAttributeGuid(nd, XmlTags.XMLATT_guid));
						}
						foreach (Guid g in list)
						{
							if (g != Guid.Empty)
							{
								if (!gl.Contains(g))
								{
									XmlNode gNode = d.Doc.CreateElement(XmlTags.XML_Item);
									connectionsNode.AppendChild(gNode);
									XmlUtil.SetAttribute(gNode, XmlTags.XMLATT_guid, g.ToString("D"));
								}
							}
						}
					}
					d.Save();
				}
			}
		}
		public void SetProjectObjectGuidList(IList<Guid> list)
		{
			using (XmlDoc d = getVobDoc(this.ProjectFile))
			{
				XmlNode xmlData = d.Doc.DocumentElement;
				if (xmlData == null)
				{
					xmlData = d.Doc.CreateElement(XML_Project);
					d.Doc.AppendChild(xmlData);
				}
				XmlNode connectionsNode = xmlData.SelectSingleNode(XmlTags.XML_DbConnectionList);
				if (connectionsNode == null)
				{
					connectionsNode = d.Doc.CreateElement(XmlTags.XML_DbConnectionList);
					xmlData.AppendChild(connectionsNode);
				}
				else
				{
					connectionsNode.RemoveAll();
				}
				if (list != null && list.Count > 0)
				{
					foreach (Guid g in list)
					{
						if (g != Guid.Empty)
						{
							XmlNode gNode = d.Doc.CreateElement(XmlTags.XML_Item);
							connectionsNode.AppendChild(gNode);
							XmlUtil.SetAttribute(gNode, XmlTags.XMLATT_guid, g.ToString("D"));
						}
					}
				}
				d.Save();
			}
		}
		public IList<Guid> GetObjectGuidList()
		{
			List<Guid> list = new List<Guid>();
			using (XmlDoc d = GetVob())
			{
				XmlNode xmlData = d.Doc.DocumentElement;
				if (xmlData != null)
				{
					XmlNodeList nl = xmlData.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}",
						XmlTags.XML_DbConnectionList, XmlTags.XML_Item));
					if (nl != null && nl.Count > 0)
					{
						foreach (XmlNode nd in nl)
						{
							Guid g = XmlUtil.GetAttributeGuid(nd, XmlTags.XMLATT_guid);
							if (g != Guid.Empty)
							{
								list.Add(g);
							}
						}
					}
				}
			}
			return list;
		}
		public void AddWcfService(Dictionary<string, string> webService)
		{
			addServer(webService, XmlTags.XML_WcfServiceList);
		}
		public void AddWebService(Dictionary<string, string> webService)
		{
			addServer(webService, XmlTags.XML_WebServiceList);
		}
		private void addServer(Dictionary<string, string> webService, string tag)
		{
			string proxyFile = webService[XmlTags.XMLATT_filename];
			using (XmlDoc d = getVobDoc(this.ProjectFile))
			{
				XmlNode XmlData = d.Doc.DocumentElement;
				XmlNodeList list = XmlData.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}",
					tag, XmlTags.XML_Item));
				if (list != null && list.Count > 0)
				{
					foreach (XmlNode node in list)
					{
						string s = XmlUtil.GetAttribute(node, XmlTags.XMLATT_filename);
						if (string.Compare(proxyFile, s, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return;
						}
					}
				}
				XmlNode nodes = XmlUtil.CreateSingleNewElement(XmlData, tag);
				XmlNode nd = XmlData.OwnerDocument.CreateElement(XmlTags.XML_Item);
				nodes.AppendChild(nd);
				foreach (KeyValuePair<string, string> kv in webService)
				{
					XmlUtil.SetAttribute(nd, kv.Key, kv.Value);
				}
				d.Save();
			}
		}
		public PropertyBag GetWebServiceFiles()
		{
			return getList(XmlTags.XML_WebServiceList);
		}
		public PropertyBag GetWcfServiceFiles()
		{
			return getList(XmlTags.XML_WcfServiceList);
		}
		public PropertyBag GetMySqlConnections()
		{
			return getList(XmlTags.XML_DbConnectionList);
		}
		public void SetMySqlConnections(PropertyBag connections)
		{
			setList(XmlTags.XML_DbConnectionList, connections);
		}
		public void AddMySqlConnection(Dictionary<string, string> connection)
		{
			addServer(connection, XmlTags.XML_DbConnectionList);
		}
		private void setList(string name, PropertyBag bag)
		{
			using (XmlDoc d = getVobDoc(ProjectFile))
			{
				XmlNode XmlData = d.Doc.DocumentElement;
				XmlNode node = XmlData.SelectSingleNode(name);
				if (node == null)
				{
					node = XmlData.OwnerDocument.CreateElement(name);
					XmlData.AppendChild(node);
				}
				node.RemoveAll();
				foreach (Dictionary<string, string> props in bag)
				{
					XmlNode nd = XmlData.OwnerDocument.CreateElement(XmlTags.XML_Item);
					node.AppendChild(nd);
					foreach (KeyValuePair<string, string> kv in props)
					{
						XmlUtil.SetAttribute(nd, kv.Key, kv.Value);
					}
				}
				d.Save();
			}
		}
		private PropertyBag getList(string name)
		{
			PropertyBag sc = new PropertyBag();
			using (XmlDoc d = GetVob())
			{
				XmlNode XmlData = d.Doc.DocumentElement;
				XmlNodeList list = XmlData.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}",
					name, XmlTags.XML_Item));
				if (list != null && list.Count > 0)
				{
					foreach (XmlNode node in list)
					{
						Dictionary<string, string> props = new Dictionary<string, string>();
						foreach (XmlAttribute xa in node.Attributes)
						{
							props.Add(xa.Name, xa.Value);
						}
						sc.Add(props);
					}
				}
			}
			return sc;
		}
		public void RemoveWebServices(StringCollection files)
		{
			using (XmlDoc d = getVobDoc(this.ProjectFile))
			{
				XmlNode XmlData = d.Doc.DocumentElement.SelectSingleNode(XmlTags.XML_WebServiceList);
				if (XmlData != null)
				{
					foreach (string s in files)
					{
						XmlNode node = XmlData.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}']",
								XmlTags.XML_Item, XmlTags.XMLATT_filename, s));
						if (node != null)
						{
							XmlData.RemoveChild(node);
						}
					}
				}
				d.Save();
			}
		}
		public void RenameProjectFile(string newFile)
		{
			string vob0 = projectConfigFile(this.ProjectFile);
			string vob1 = projectConfigFile(newFile);
			if (File.Exists(vob0))
			{
				File.Copy(vob0, vob1, true);
			}
			LimnorProject p = LimnorSolution.GetProjectByProjectFile(this.ProjectFile);
			if (p != null)
			{
				p._prjFile = newFile;
			}
			_prjFile = newFile;
		}
		public void ReloadToolbox()
		{
			ToolboxLoaded = false;
			if (_typedData != null)
			{
				Dictionary<UInt32, Dictionary<Type, object>> tdp;
				if (_typedData.TryGetValue(_guid, out tdp))
				{
					bool b = false;
					foreach (KeyValuePair<UInt32, Dictionary<Type, object>> kv in tdp)
					{
						foreach (KeyValuePair<Type, object> kv2 in kv.Value)
						{
							IDesignPane dp = kv2.Value as IDesignPane;
							if (dp != null)
							{
								dp.LoadProjectToolbox();
								b = true;
								break;
							}
						}
						if (b)
						{
							break;
						}
					}
				}
			}
		}
		public void UpdateStartPageClassId(XmlNode mainClassNode)
		{
			initializeMainComponent();
			_mainComponent.UpdateStartPageClassId(mainClassNode);
		}
		public XmlDoc GetVob()
		{
			return getVobDoc(this.ProjectFile);
		}
		public void AddReference(string a)
		{
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNodeList nodes = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}/{1}", XML_Assembly, XML_Item));
					foreach (XmlNode nd in nodes)
					{
						if (string.Compare(a, nd.InnerText, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return;
						}
					}
					XmlNode nodeA = doc.DocumentElement.SelectSingleNode(XML_Assembly);
					if (nodeA == null)
					{
						nodeA = doc.CreateElement(XML_Assembly);
						doc.DocumentElement.AppendChild(nodeA);
					}
					XmlNode node = doc.CreateElement(XML_Item);
					nodeA.AppendChild(node);
					node.InnerText = a;
					d.Save();
				}
			}
		}
		public List<Assembly> GetReferences(StringCollection errors)
		{
			List<Assembly> list = new List<Assembly>();
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement != null)
					{
						string curDir = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
						XmlNodeList nodes = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}/{1}", XML_Assembly, XML_Item));
						foreach (XmlNode nd in nodes)
						{
							string loc = nd.InnerText;
							if (!string.IsNullOrEmpty(loc))
							{
								if (!System.IO.File.Exists(loc))
								{
									errors.Add("File not found:" + loc);
									continue;
								}
								else
								{
									try
									{
										string path = System.IO.Path.GetDirectoryName(loc);
										AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = path;
										Assembly a = Assembly.LoadFile(loc);
										if (a != null)
										{
											list.Add(a);
										}
										else
										{
											errors.Add(string.Format("Assembly not loaded from {0}", loc));
										}
									}
									catch (Exception err)
									{
										errors.Add(string.Format("Cannot load assembly from {0}", loc));
										errors.Add(err.Message);
									}
									AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = curDir;
								}
							}
						}
					}
				}
			}
			return list;
		}
		public List<Type> GetToolboxItems(StringCollection errors)
		{
			List<Type> list = new List<Type>();
			if (!string.IsNullOrEmpty(this.ProjectFile))
			{
				using (XmlDoc d = getVobDoc(this.ProjectFile))
				{
					XmlDocument doc = d.Doc;
					if (doc.DocumentElement != null)
					{
						string curDir = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
						XmlNodeList nodes = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}/{1}", XML_Toolbox, XML_Item));
						foreach (XmlNode nd in nodes)
						{
							try
							{
								Type t = XmlUtil.GetLibTypeAttribute(nd);
								if (t == null)
								{
									errors.Add(string.Format("Cannot load toolbox item type:[{0}]", XmlUtil.GetLibTypeAttributeString(nd)));
								}
								else
								{
									if (!list.Contains(t))
									{
										list.Add(t);
									}
								}
							}
							catch (Exception err)
							{
								errors.Add(string.Format("Cannot toolbox load type:[{0}]", XmlUtil.GetLibTypeAttributeString(nd)));
								errors.Add(err.Message);
							}
						}
						AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = curDir;
					}
				}
			}
			return list;
		}
		public void AddToolboxItem(Type item)
		{
			string prjFile = this.ProjectFile;
			if (!string.IsNullOrEmpty(prjFile))
			{
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					XmlNode node = null;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNodeList nodeLst = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}/{1}", XML_Toolbox, XML_Item));
					if (nodeLst != null && nodeLst.Count > 0)
					{
						for (int i = 0; i < nodeLst.Count; i++)
						{
							Type t = XmlUtil.GetLibTypeAttribute(nodeLst[i]);
							if (t != null && VPLUtil.IsSameType(t, item))
							{
								node = nodeLst[i];
								break;
							}
						}
					}
					if (node == null)
					{
						XmlNode nodeToolbox = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}", XML_Toolbox));
						if (nodeToolbox == null)
						{
							nodeToolbox = doc.CreateElement(XML_Toolbox);
							doc.DocumentElement.AppendChild(nodeToolbox);
						}
						node = doc.CreateElement(XML_Item);
						XmlUtil.SetLibTypeAttribute(node, item);
						//TBD: remove it and test
						if (!item.Assembly.GlobalAssemblyCache)
						{
							if (!VPLUtil.IsDynamicAssembly(item.Assembly))
							{
								try
								{
									node.InnerText = item.Assembly.Location;
								}
								catch
								{
								}
							}
						}
						nodeToolbox.AppendChild(node);
						d.Save();
					}
				}
			}
		}
		public void AddToolboxItems(Type[] items, bool append)
		{
			string prjFile = this.ProjectFile;
			if (!string.IsNullOrEmpty(prjFile))
			{
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					XmlNode node = null;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNode nodeToolbox = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}", XML_Toolbox));
					if (nodeToolbox == null)
					{
						nodeToolbox = doc.CreateElement(XML_Toolbox);
						doc.DocumentElement.AppendChild(nodeToolbox);
					}
					else
					{
						if (!append)
						{
							nodeToolbox.RemoveAll();
						}
					}
					if (items != null)
					{
						List<Type> added = new List<Type>();
						XmlNodeList typeNodeList = nodeToolbox.SelectNodes(XML_Item);
						for (int i = 0; i < items.Length; i++)
						{
							if (items[i] != null)
							{
								if (!added.Contains(items[i]))
								{
									added.Add(items[i]);
									node = XmlUtil.FindTypeNode(typeNodeList, items[i]);
									if (node == null)
									{
										node = doc.CreateElement(XML_Item);
										XmlUtil.SetLibTypeAttribute(node, items[i]);
										nodeToolbox.AppendChild(node);
									}
								}
							}
						}
					}
					d.Save();
				}
			}
		}
		public void RemoveToolboxItem(Type item)
		{
			string prjFile = this.ProjectFile;
			if (!string.IsNullOrEmpty(prjFile))
			{
				using (XmlDoc d = getVobDoc(prjFile))
				{
					XmlDocument doc = d.Doc;
					XmlNode node = null;
					if (doc.DocumentElement == null)
					{
						createVobRootNode(d);
					}
					XmlNodeList typeNodeList = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}", XML_Toolbox, XML_Item));
					node = XmlUtil.FindTypeNode(typeNodeList, item);
					if (node != null)
					{
						XmlNode nodeToolbox = node.ParentNode;
						nodeToolbox.RemoveChild(node);
						d.Save();
					}
				}
			}
		}
		public void OnClose()
		{
			if (_fileProjectFile != null)
			{
				while (true)
				{
					string s = string.Empty;
					foreach (KeyValuePair<string, string> kv in _fileProjectFile)
					{
						if (string.Compare(kv.Value, ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
						{
							s = kv.Key;
							break;
						}
					}
					if (string.IsNullOrEmpty(s))
					{
						break;
					}
					else
					{
						_fileProjectFile.Remove(s);
					}
				}
			}
			if (ActiveProject != null)
			{
				if (ActiveProject._guid == this._guid)
				{
					SetActiveProject(null);
				}
			}
		}
		public void DesignPaneCreated(object pane)
		{
			if (_designPanes == null)
			{
				_designPanes = new Dictionary<string, List<object>>();
			}
			string key = this.ProjectFile.ToLowerInvariant();
			List<object> panes;
			if (!_designPanes.TryGetValue(key, out panes))
			{
				panes = new List<object>();
				_designPanes.Add(key, panes);
			}
			if (!panes.Contains(pane))
			{
				panes.Add(pane);
			}
		}
		public void DesignPaneClosed(object pane)
		{
			if (_designPanes != null)
			{
				string key = this.ProjectFile.ToLowerInvariant();
				List<object> panes;
				if (_designPanes.TryGetValue(key, out panes))
				{
					if (panes.Contains(pane))
					{
						panes.Remove(pane);
					}
				}
			}
			if (_activeDesignerLoader != null)
			{
				if (_activeDesignerLoader.ActiveProject == null)
				{
					_activeDesignerLoader = null;
				}
				else if (_activeDesignerLoader.ActiveProject == this)
				{
					if (_activeDesignerLoader.DesignerObject == pane)
					{
						_activeDesignerLoader = null;
					}
				}
			}
		}
		public int DesignPaneCount()
		{
			if (_designPanes != null)
			{
				string key = this.ProjectFile.ToLowerInvariant();
				List<object> panes;
				if (_designPanes.TryGetValue(key, out panes))
				{
					return panes.Count;
				}
			}
			return 0;
		}

		public XmlDocument GetDocumentByClassId(UInt32 classId)
		{
			string file = GetComponentFileByClassId(classId);
			if (!string.IsNullOrEmpty(file))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				return doc;
			}
			return null;
		}
		public string GetComponentFileByClassId(UInt32 classId)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(ProjectFile);
			XmlNamespaceManager xm = new XmlNamespaceManager(doc.NameTable);
			xm.AddNamespace("x", PRJNAMESPACEURI);
			XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"x:{0}/x:{1}[@{2}]", XML_ItemGroup, XML_Content, XMLATT_Include), xm);
			for (int i = 0; i < list.Count; i++)
			{
				string f = System.IO.Path.Combine(ProjectFolder, GetAttribute(list[i], XMLATT_Include));
				if (!System.IO.File.Exists(f))
				{
					f = GetFileRename(System.IO.Path.GetFileName(f));
				}
				if (System.IO.File.Exists(f))
				{
					XmlDocument doc1 = new XmlDocument();
					doc1.Load(f);
					if (Convert.ToUInt32(GetAttribute(doc1.DocumentElement, "ClassID")) == classId)
					{
						return f;
					}
				}
			}
			//unsaved
			if (_fileProjectFile != null)
			{
				foreach (KeyValuePair<string, string> kv in _fileProjectFile)
				{
					if (kv.Value == ProjectFile)
					{
						XmlDocument doc1 = new XmlDocument();
						doc1.Load(kv.Key);
						if (Convert.ToUInt32(GetAttribute(doc1.DocumentElement, "ClassID")) == classId)
						{
							return kv.Key;
						}
					}
				}
			}
			return null;
		}

		public string[] GetComponentFiles()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(ProjectFile);
			XmlNamespaceManager xm = new XmlNamespaceManager(doc.NameTable);
			xm.AddNamespace("x", PRJNAMESPACEURI);
			XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"x:{0}/x:{1}[@{2}]", XML_ItemGroup, XML_Content, XMLATT_Include), xm);
			List<string> ss = new List<string>();
			for (int i = 0; i < list.Count; i++)
			{
				string s = System.IO.Path.Combine(ProjectFolder, GetAttribute(list[i], XMLATT_Include));
				if (!System.IO.File.Exists(s))
				{
					s = GetFileRename(System.IO.Path.GetFileName(s));
				}
				if (System.IO.File.Exists(s))
				{
					ss.Add(s);
				}
			}
			//unsaved
			if (_fileProjectFile != null)
			{
				foreach (KeyValuePair<string, string> kv in _fileProjectFile)
				{
					if (string.Compare(kv.Value, ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (!ss.Contains(kv.Key))
						{
							ss.Add(kv.Key);
						}
					}
				}
			}
			string[] cfs = new string[ss.Count];
			ss.CopyTo(cfs);
			return cfs;
		}
		public bool IsComponentNameInUse(ComponentID component)
		{
			string[] ss = GetComponentFiles();
			for (int i = 0; i < ss.Length; i++)
			{
				ComponentID cid = GetComponentID(this, ss[i]);
				if (cid.ComponentId != component.ComponentId)
				{
					if (cid.ComponentName == component.ComponentName)
					{
						return true;
					}
				}
			}
			return false;
		}
		public string GetNewComponentName(string componentFile)
		{
			ComponentID newComponent = GetComponentID(this, componentFile);
			StringCollection names = new StringCollection();
			string[] ss = GetComponentFiles();
			for (int i = 0; i < ss.Length; i++)
			{
				ComponentID cid = GetComponentID(this, ss[i]);
				if (cid.ComponentId != newComponent.ComponentId)
				{
					names.Add(cid.ComponentName);
				}
			}
			int n = 1;
			string namebase = newComponent.ComponentName.Replace("1", "");
			string nm = newComponent.ComponentName;
			while (true)
			{
				if (names.Contains(nm))
				{
					n++;
					nm = namebase + n.ToString();
				}
				else
				{
					break;
				}
			}
			newComponent.Rename(nm);
			return newComponent.ComponentName;
		}
		public string MakeNameComponentName(string namebase)
		{
			StringCollection names = new StringCollection();
			string[] ss = GetComponentFiles();
			for (int i = 0; i < ss.Length; i++)
			{
				ComponentID cid = GetComponentID(this, ss[i]);
				names.Add(cid.ComponentName);
			}
			int n = 1;
			string newComponentComponentName = namebase;
			while (true)
			{
				if (names.Contains(newComponentComponentName))
				{
					n++;
					newComponentComponentName = namebase + n.ToString();
				}
				else
				{
					break;
				}
			}
			return newComponentComponentName;
		}
		public IList<ComponentID> GetAllComponents()
		{
			List<ComponentID> list = new List<ComponentID>();
			string[] ss = GetComponentFiles();
			for (int i = 0; i < ss.Length; i++)
			{
				ComponentID cid = GetComponentID(this, ss[i]);
				list.Add(cid);
			}
			return list;
		}
		public ComponentID GetComponentByID(UInt32 id)
		{
			string[] ss = GetComponentFiles();
			for (int i = 0; i < ss.Length; i++)
			{
				ComponentID cid = GetComponentID(this, ss[i]);
				if (cid.ComponentId == id)
				{
					return cid;
				}
			}
			return null;
		}
		#endregion
		#region Resources Manager
		public string ResourcesFile
		{
			get
			{
				return System.IO.Path.Combine(this.ProjectFolder, "resources.lresx");
			}
		}
		public XmlNode ResourcesXmlNode
		{
			get
			{
				XmlDocument doc = new XmlDocument();
				string f = ResourcesFile;
				if (System.IO.File.Exists(f))
				{
					doc.Load(f);
				}
				if (doc.DocumentElement == null)
				{
					XmlNode n = doc.CreateElement("Resources");
					doc.AppendChild(n);
				}
				return doc.DocumentElement;
			}
		}
		#endregion
		#region Modification tracking
		private static Dictionary<Guid, List<UInt32>> _modifiedClasses;
		public List<UInt32> GetModifiedClasses()
		{
			if (_modifiedClasses == null)
			{
				_modifiedClasses = new Dictionary<Guid, List<uint>>();
			}
			List<UInt32> clss;
			if (!_modifiedClasses.TryGetValue(this.ProjectGuid, out clss))
			{
				clss = new List<uint>();
				_modifiedClasses.Add(this.ProjectGuid, clss);
				using (XmlDoc doc = getVobDoc(this.ProjectFile))
				{
					if (doc.Doc.DocumentElement != null)
					{
						XmlNode nodeModified = doc.Doc.DocumentElement.SelectSingleNode(XML_ModifiedClasses);
						if (nodeModified != null)
						{
							string ids = nodeModified.InnerText;
							if (!string.IsNullOrEmpty(ids))
							{
								string[] ss = ids.Split(';');
								for (int i = 0; i < ss.Length; i++)
								{
									if (!string.IsNullOrEmpty(ss[i]))
									{
										ss[i] = ss[i].Trim();
										if (ss[i].Length > 0)
										{
											UInt32 id;
											if (UInt32.TryParse(ss[i], out id))
											{
												if (!clss.Contains(id))
												{
													clss.Add(id);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return clss;
		}
		public void AddModifiedClass(UInt32 id)
		{
			List<UInt32> classes = GetModifiedClasses();
			if (!classes.Contains(id))
			{
				classes.Add(id);
			}
		}
		public void SaveModifiedClassIDs(bool compiled)
		{
			List<UInt32> classes = GetModifiedClasses();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < classes.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(";");
				}
				sb.Append(classes[i]);
			}
			using (XmlDoc doc = getVobDoc(this.ProjectFile))
			{
				if (doc.Doc.DocumentElement == null)
				{
					XmlNode nodeRoot = doc.Doc.CreateElement(XML_Project);
					doc.Doc.AppendChild(nodeRoot);
				}
				if (compiled)
				{
					XmlUtil.SetAttribute(doc.Doc.DocumentElement, XMLATT_isNotNew, true);
				}
				XmlNode nodeModified = doc.Doc.DocumentElement.SelectSingleNode(XML_ModifiedClasses);
				if (nodeModified == null)
				{
					nodeModified = doc.Doc.CreateElement(XML_ModifiedClasses);
					doc.Doc.DocumentElement.AppendChild(nodeModified);
				}
				nodeModified.InnerText = sb.ToString();
				doc.Save();
			}
		}
		public void ClearModifyFlags()
		{
			List<UInt32> classes = GetModifiedClasses();
			classes.Clear();
			SaveModifiedClassIDs(true);
		}
		public void SetProjectCompiled()
		{
			using (XmlDoc doc = getVobDoc(this.ProjectFile))
			{
				if (doc.Doc.DocumentElement == null)
				{
					XmlNode nodeRoot = doc.Doc.CreateElement(XML_Project);
					doc.Doc.AppendChild(nodeRoot);
				}
				XmlUtil.SetAttribute(doc.Doc.DocumentElement, XMLATT_isNotNew, true);
				doc.Save();
			}
		}
		#endregion
	}
	public class XmlDoc : IDisposable
	{
		public XmlDoc()
		{
		}
		public XmlDoc(string f, XmlDocument d, XmlNamespaceManager n)
		{
			FilePath = f;
			Doc = d;
			NamespaceManager = n;
		}
		private XmlDocument _doc;
		public XmlDocument Doc { get { return _doc; } set { _doc = value; } }
		private string _f;
		public string FilePath { get { return _f; } set { _f = value; } }
		private XmlNamespaceManager _xm;
		public XmlNamespaceManager NamespaceManager { get { return _xm; } set { _xm = value; } }
		public void Save()
		{
			int n = 0;
			while (n < 3)
			{
				try
				{
					Doc.Save(FilePath);
					break;
				}
				catch (Exception err)
				{
					n++;
					if (n < 3)
					{
						System.GC.Collect();
						Application.DoEvents();
						Thread.Sleep(500);
					}
					else
					{
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
							"Error saving project configuration file {0}. Error message:{1}. It might be caused by user permissions or file locking. You may close the project and re-open it.",
							FilePath, err.Message));
					}
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			_xm = null;
			_doc = null;
			System.GC.Collect();
		}

		#endregion
	}
}
