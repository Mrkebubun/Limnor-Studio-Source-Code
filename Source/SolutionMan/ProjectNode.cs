/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.ComponentModel;
using System.IO;
using MathExp;
using System.Collections.Specialized;
using MathExp.RaisTypes;
using System.Drawing.Design;
using VOB;
using FileUtil;
using LimnorDesigner;
using VSPrj;
using XmlUtility;
using LimnorUI;
using VPL;
using System.Globalization;
using LFilePath;

namespace SolutionMan
{
	public class ProjectNodeData : NodeDataFile, IPropertyGridHolder
	{
		#region fields and constructors
		private LimnorProject _prj;
		private bool bActive = false;
		private bool _canResetDirty = true;
		private bool _autoAdjustNames;
		public event EventHandler OnActivationChanged = null;
		//
		public ProjectNodeData(LimnorProject project, TreeNode nodes)
			: base(nodes)
		{
			_prj = project;
			_prj.SetAsMainProjectFile();
			Name = project.ProjectAssemblyName;
			this._file.Pattern = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Limnor project files|*.{0}", NodeDataSolution.FILE_EXT_PROJECT);
			this._file.Title = "Select Limnor Project File";
			this.OnFilenameChanged += new EventHandler(ProjectNodeData_OnFilenameChanged);
		}
		#endregion
		#region public methods
		public void RenameClassFile(string oldFile, string newFile)
		{
			_prj.RenameClassFile(oldFile, newFile);
		}
		public List<ObjectTextID> GetClassUsages(UInt32 classId)
		{
			List<ObjectTextID> lst = new List<ObjectTextID>();
			for (int i = 0; i < _nodes.Nodes.Count; i++)
			{
				ComponentNode cn = _nodes.Nodes[i] as ComponentNode;
				if (cn != null)
				{
					NodeObjectComponent cnd = cn.PropertyObject as NodeObjectComponent;
					if (cnd.Class.ComponentId != classId)
					{
						XmlNode nd = cnd.Class.ComponentXmlNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "//*[@{0}='{1}']", XmlTags.XMLATT_ClassID, classId));
						if (nd != null)
						{
							lst.Add(new ObjectTextID(cn.Text, cnd.Class.ComponentName, cnd.Class.ComponentName));
						}
					}
				}
			}
			return lst;
		}
		/// <summary>
		/// create an entry point for EXE project
		/// </summary>
		public XmlNode GetEntryPoint()
		{
			return _prj.MainComponent.MainComponentNode;
		}
		public string CreateNewComponentName(string baseName)
		{
			int n = 1;
			string name = baseName + n.ToString();
			StringCollection names = GetAllComponentNames();
			while (names.Contains(name))
			{
				n++;
				name = baseName + n.ToString();
			}
			return name;
		}
		public void RemoveComponent(UInt32 classId)
		{
		}
		public string GetProgramFullpath(bool debug)
		{
			return _prj.GetProgramFullpath(debug);
		}
		public StringCollection GetAllComponentNames()
		{
			StringCollection sc = new StringCollection();
			IList<ComponentID> list = _prj.GetAllComponents();
			foreach (ComponentID c in list)
			{
				sc.Add(c.ComponentName);
			}
			return sc;
		}
		public void SetEditor(IRefreshPropertyGrid e)
		{
		}
		#endregion
		#region Non-browsable properties
		[Browsable(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}
		[Browsable(false)]
		public Guid ProjectGuid
		{
			get
			{
				return _prj.ProjectGuid;
			}
		}
		[Browsable(false)]
		public override FileObject Filename
		{
			get
			{
				return _file;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public bool AutoAdjustNames
		{
			get
			{
				return _autoAdjustNames;
			}
			set
			{
				_autoAdjustNames = value;
			}
		}
		[Browsable(false)]
		public override bool Dirty
		{
			get
			{
				if (_prj.Changed)
				{
					return true;
				}
				if (_nodes != null)
				{
					for (int i = 0; i < _nodes.Nodes.Count; i++)
					{
						ComponentNode node = _nodes.Nodes[i] as ComponentNode;
						if (node != null)
						{
							if (node.PropertyObject.Dirty)
								return true;
						}
					}
				}
				return base.Dirty;
			}
			set
			{
				base.Dirty = value;
			}
		}
		[Browsable(false)]
		public LimnorProject Project
		{
			get
			{
				return _prj;
			}
		}
		[Browsable(false)]
		public bool IsActive
		{
			get
			{
				return bActive;
			}
			set
			{
				if (bActive != value)
				{
					bActive = value;
					if (OnActivationChanged != null)
					{
						OnActivationChanged(this, null);
					}
				}
			}
		}
		#endregion
		#region Properties in Grid
		[Category("Assembly")]
		[ParenthesizePropertyName(true)]
		[Description("Gets and sets program name for this project. The compiled program file will be formed as {AssemblyName}.exe, {AssemblyName}.dll, or {AssemblyName}.msi, depending on the type of the project.")]
		public string AssemblyName
		{
			get
			{
				return _prj.AssemblyName;
			}
			set
			{
				if (value != null)
				{
					string s = value.Trim();
					if (VOB.VobUtil.IsGoodVarName(s))
					{
						_prj.SetProjectAssemblyName(s);
					}
				}
			}
		}
		[Description("All source code generated for this project will be under this namespace")]
		[Category("Assembly")]
		public string Namespace
		{
			get
			{
				return _prj.Namespace;
			}
			set
			{
				if (value != null)
				{
					string s = value.Trim();
					if (VOB.VobUtil.IsGoodNamespace(s))
					{
						_prj.SetProjectNamespace(s);
					}
				}
			}
		}

		[Description("Type of the program the project will be compiled into")]
		[Category("Program")]
		public EnumProjectType ProgramType
		{
			get
			{
				return _prj.ProjectType;
			}
		}
		[Description("Compiled binary executeable filename of the project")]
		[Category("Program")]
		public string ProgramName
		{
			get
			{

				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", AssemblyName, binExt());
			}
		}
		[Description("Name of the project file")]
		[Category("Program")]
		public string ProjectFilename
		{
			get
			{
				return Path.GetFileNameWithoutExtension(_prj.ProjectFile);
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					Form f = null;
					if (this._nodes.TreeView != null)
					{
						f = this._nodes.TreeView.FindForm();
					}
					string nf = Path.Combine(_prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}.{1}", value, NodeDataSolution.FILE_EXT_PROJECT));
					if (File.Exists(nf))
					{
						MessageBox.Show(f, string.Format(CultureInfo.InvariantCulture, "File exists [{0}]", nf), "Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						try
						{
							string oldFile = _prj.ProjectFile;
							File.Copy(oldFile, nf);
							_prj.RenameProjectFile(nf);
							SolutionNode sn = this._nodes.Parent as SolutionNode;
							if (sn != null)
							{
								sn.Dirty = true;
								NodeDataSolution snd = sn.PropertyObject as NodeDataSolution;
								snd.Save();
							}
							File.Delete(oldFile);
							LimnorProject.DeleteVobFile(oldFile);
						}
						catch (Exception err)
						{
							MessageBox.Show(f, string.Format(CultureInfo.InvariantCulture, "Cannot rename the project file to [{0}]. {1}", nf, err.Message), "Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
		}
		[Description("Full path of the project file")]
		[Category("Program")]
		public string ProjectFile
		{
			get
			{
				return _prj.ProjectFile;
			}
		}
		[Editor(typeof(TypeEditorLicenseFiles), typeof(UITypeEditor))]
		[Description("License files needed for Windows forms components")]
		[Category("Assembly")]
		public Dictionary<string, string[]> LicenseFiles
		{
			get
			{
				return _prj.LicenseFiles;
			}
			set
			{
				_prj.SetLicenseFiles(value);
			}
		}
		[Description("Assembly product name")]
		[Category("Assembly")]
		public string AssemblyProduct
		{
			get
			{
				return _prj.AssemblyProduct;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyProduct(s);
			}
		}
		[Description("Assembly product title")]
		[Category("Assembly")]
		public string AssemblyTitle
		{
			get
			{
				return _prj.AssemblyTitle;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyTitle(s);
			}
		}
		[Description("Company name")]
		[Category("Assembly")]
		public string AssemblyCompany
		{
			get
			{
				return _prj.AssemblyCompany;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyCompany(s);
			}
		}
		[Description("Copytight notice")]
		[Category("Assembly")]
		public string AssemblyCopyright
		{
			get
			{
				return _prj.AssemblyCopyright;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyCopyright(s);
			}
		}
		[Description("Assembly description")]
		[Category("Assembly")]
		public string AssemblyDescription
		{
			get
			{
				return _prj.AssemblyDescription;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyDescription(s);
			}
		}
		[Description("Assembly file version frmatted as n.n.n.n")]
		[Category("Assembly")]
		public string AssemblyFileVersion
		{
			get
			{
				return _prj.AssemblyFileVersion;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyFileVersion(s);
			}
		}
		[Description("Assembly version frmatted as n.n.n.n")]
		[Category("Assembly")]
		public string AssemblyVersion
		{
			get
			{
				return _prj.AssemblyVersion;
			}
			set
			{
				string s = value.Trim();
				_prj.SetAssemblyVersion(s);
			}
		}

		[DefaultValue(AssemblyTargetPlatform.AnyCPU)]
		[Description("Assembly target platform")]
		[Category("Assembly")]
		public AssemblyTargetPlatform TargetPlatform
		{
			get
			{
				return _prj.TargetPlatform;
			}
			set
			{
				_prj.SetTargetPlatform(value);
			}
		}
		[DefaultValue("")]
		[FilePath("Key file|*.snk", "Select key file")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Description("Key file for signning the assembly")]
		[Category("Assembly")]
		public string KeyFile
		{
			get
			{
				return _prj.AssemblyKeyFile;
			}
			set
			{
				_prj.SetAssemblyKeyFile(value);
			}
		}
		[DefaultValue(false)]
		[Description("Key file for signning the assembly")]
		[Category("Assembly")]
		public bool DelaySign
		{
			get
			{
				return _prj.AssemblyDelaySign;
			}
			set
			{
				_prj.SetAssemblyDelaySign(value);
			}
		}
		[DefaultValue(false)]
		[Description("Turning on file mapping will copy involved resource files used in the project, such as image files, to folders under the project folder and map file paths based on each development computer. If a project is to be developped from multiple computers then the file mapping should be turned on.")]
		public bool TurnOnFileMapper
		{
			get
			{
				return _prj.EnableFileMapper;
			}
			set
			{
				_prj.EnableFileMapper = value;
			}
		}
#if DEBUG
		[Editor(typeof(TypeEditorTargetFramework), typeof(UITypeEditor))]
		[DefaultValue(AssemblyTargetFramework.V40)]
		[Description("Assembly target framework")]
		[Category("Assembly")]
		public AssemblyTargetFramework TargetFramework
		{
			get
			{
				return LimnorProject.GetTargetFrameworkEnum(_prj.TargetFramework);
			}
			set
			{
				_prj.SetTargetFramework(LimnorProject.GetTargetFrameworkValue(value));
			}
		}
#endif
		[Description("Project support files to be copied to the output folder by the compiler")]
		[Category("SupportFiles")]
		public ProjectSupportFiles SupportFiles
		{
			get
			{
				ProjectSupportFiles fl = _prj.GetTypedProjectData<ProjectSupportFiles>();
				if (fl == null)
				{
					fl = new ProjectSupportFiles(_prj);
					_prj.SetTypedProjectData<ProjectSupportFiles>(fl);
				}
				return fl;
			}
			set
			{

			}
		}
		#endregion
		#region override methods
		public override void Save()
		{
			_prj.Save();
		}
		public override void ResetModified()
		{
			if (!_canResetDirty)
				return;
			base.Dirty = false;
			if (_nodes != null)
			{
				for (int i = 0; i < _nodes.Nodes.Count; i++)
				{
					ComponentNode node = _nodes.Nodes[i] as ComponentNode;
					if (node != null)
					{
						node.PropertyObject.Dirty = false;
						node.ResetModified();
					}
				}
			}
		}
		#endregion
		#region private methods

		void ProjectNodeData_OnFilenameChanged(object sender, EventArgs e)
		{
			this.Dirty = true;
		}
		string binExt()
		{
			EnumProjectType p = ProgramType;
			switch (p)
			{
				case EnumProjectType.ClassDLL:
					return "dll";
				case EnumProjectType.Console:
					return "exe";
				case EnumProjectType.Kiosk:
					return "exe";
				case EnumProjectType.ScreenSaver:
					return "scr";
				case EnumProjectType.Setup:
					return "msi";
				case EnumProjectType.Unknown:
					return "";
				case EnumProjectType.WebAppAspx:
					return "dll";
				case EnumProjectType.WebAppPhp:
					return "php";
				case EnumProjectType.WebService:
					return "exe";
				case EnumProjectType.WinForm:
					return "exe";
				case EnumProjectType.WinService:
					return "exe";
				case EnumProjectType.WpfApp:
					return "exe";
			}
			return string.Empty;
		}
		#endregion

	}
	public class ProjectNode : TreeNodeClass, IParentNode
	{
		#region fields and constructors
		private List<Guid> _dependedProjects; //solution data, not project data
		public ProjectNode(LimnorProject project)
		{
			Obj = new ProjectNodeData(project, this);

			Obj.OnNameChanged += new EventHandler(Obj_OnNameChanged);
			Obj.OnNameChanging += new EventHandler(Obj_OnNameChanging);
			((ProjectNodeData)Obj).OnActivationChanged += new EventHandler(ProjectNode_OnActivationChanged);
			((ProjectNodeData)Obj).OnFilenameChanged += new EventHandler(ProjectNode_OnFilenameChanged);
			this.ImageIndex = SolutionTree.IMG_WIN_0;
			this.SelectedImageIndex = SolutionTree.IMG_WIN_0;
			Text = ToString();
			Dirty = false;
			Obj.Dirty = false;
			Nodes.Add(new TreeNodeLoader());
		}
		#endregion
		#region override functions
		protected override void SetNodeIcon()
		{
			switch (((ProjectNodeData)Obj).ProgramType)
			{
				case EnumProjectType.Console:
					if (((ProjectNodeData)Obj).IsActive)
					{
						this.ImageIndex = SolutionTree.IMG_DOS_1;
						this.SelectedImageIndex = SolutionTree.IMG_DOS_1;
					}
					else
					{
						this.ImageIndex = SolutionTree.IMG_DOS_0;
						this.SelectedImageIndex = SolutionTree.IMG_DOS_0;
					}
					break;
				case EnumProjectType.ClassDLL:
					if (((ProjectNodeData)Obj).IsActive)
					{
						this.ImageIndex = SolutionTree.IMG_DLL_1;
						this.SelectedImageIndex = SolutionTree.IMG_DLL_1;
					}
					else
					{
						this.ImageIndex = SolutionTree.IMG_DLL_0;
						this.SelectedImageIndex = SolutionTree.IMG_DLL_0;
					}
					break;
				case EnumProjectType.WinForm:
					if (((ProjectNodeData)Obj).IsActive)
					{
						this.ImageIndex = SolutionTree.IMG_WIN_1;
						this.SelectedImageIndex = SolutionTree.IMG_WIN_1;
					}
					else
					{
						this.ImageIndex = SolutionTree.IMG_WIN_0;
						this.SelectedImageIndex = SolutionTree.IMG_WIN_0;
					}
					break;
			}
		}
		#endregion
		#region event handlers
		void ProjectNode_OnFilenameChanged(object sender, EventArgs e)
		{

		}
		void Obj_OnNameChanging(object sender, EventArgs e)
		{
			EventArgNameChange en = (EventArgNameChange)e;
			TreeNode nd = this.Parent;
			if (nd != null)
			{
				for (int i = 0; i < nd.Nodes.Count; i++)
				{
					if (i != this.Index)
					{
						if (string.CompareOrdinal(nd.Nodes[i].Text, en.NewName) == 0)
						{
							MessageBox.Show("The project name is already used");
							en.Cancel = true;
						}
					}
				}
			}
		}
		void ProjectNode_OnActivationChanged(object sender, EventArgs e)
		{
			SetNodeIcon();
		}
		void Obj_OnNameChanged(object sender, EventArgs e)
		{
			Text = ToString();
		}
		#endregion
		#region public methods
		public void SetDependencies(List<Guid> projectIDs)
		{
			_dependedProjects = projectIDs;
		}
		public void AddDependedProject(Guid g)
		{
			if (_dependedProjects == null)
			{
				_dependedProjects = new List<Guid>();
			}
			if (!_dependedProjects.Contains(g))
			{
				_dependedProjects.Add(g);
			}
		}
		public ComponentNode GetComponentNodeByName(string name)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ComponentNode cNode = Nodes[i] as ComponentNode;
				if (cNode != null)
				{
					NodeObjectComponent cData = (NodeObjectComponent)cNode.PropertyObject;
					if (string.CompareOrdinal(name, cData.Name) == 0)
					{
						return cNode;
					}
				}
			}
			return null;
		}
		public ComponentNode GetComponentNodeById(UInt32 id)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ComponentNode cNode = Nodes[i] as ComponentNode;
				if (cNode != null)
				{
					NodeObjectComponent cData = (NodeObjectComponent)cNode.PropertyObject;
					if (cData.Class.ComponentId == id)
						return cNode;
				}
			}
			return null;
		}
		#endregion
		#region Properties
		public LimnorProject Project
		{
			get
			{
				return ((ProjectNodeData)Obj).Project;
			}
		}
		public IList<Guid> DependedProjects
		{
			get
			{
				return _dependedProjects;
			}
		}
		#endregion
		#region IParentNode Members
		private bool _nextLevelLoaded;
		public void LoadNextLevel(TreeNodeLoader level)
		{
			if (!_nextLevelLoaded)
			{
				_nextLevelLoaded = true;
				ProjectNodeData pd = Obj as ProjectNodeData;
				IList<ClassData> classes = pd.Project.GetAllClasses();
				foreach (ClassData c in classes)
				{
					ComponentNode cn = new ComponentNode(c, pd);
					this.Nodes.Add(cn);
				}
			}
		}
		public bool NextLevelLoaded { get { return _nextLevelLoaded; } }
		#endregion
	}
}
