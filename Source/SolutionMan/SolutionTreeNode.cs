/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using System.IO;
using MathExp;
using System.Collections.Specialized;
using XmlUtility;
using VSPrj;
using System.Globalization;
using LimnorDesigner;
using VPL;
using TraceLog;

namespace SolutionMan
{
	public class EventArgvFilenameChange : EventArgs
	{
		private string _oldfile;
		private string _newfile;
		public EventArgvFilenameChange(string oldFile, string newFile)
		{
			_oldfile = oldFile;
			_newfile = newFile;
		}
		public string OldFilename
		{
			get
			{
				return _oldfile;
			}
		}
		public string NewFilename
		{
			get
			{
				return _newfile;
			}
		}
	}
	public class FileObject
	{
		private string _file = "";
		private string _pattern = "All|*.*";
		private string _title = "Select file";
		private bool _checkFileExists = false;
		private bool _forOpen;
		public event EventHandler OnFilenameChanged = null;
		public FileObject()
		{
		}
		public bool CheckFileExists
		{
			get
			{
				return _checkFileExists;
			}
			set
			{
				_checkFileExists = value;
			}
		}
		public bool ForOpen
		{
			get
			{
				return _forOpen;
			}
			set
			{
				_forOpen = value;
			}
		}
		public void AssignFilename(string filename)
		{
			_file = filename;
		}
		[ReadOnly(true)]
		public string Filename
		{
			get
			{
				return _file;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_file.ToLower() != value.ToLower())
					{
						EventArgvFilenameChange e = new EventArgvFilenameChange(_file, value);
						_file = value;
						if (OnFilenameChanged != null)
						{
							OnFilenameChanged(this, e);
						}
					}
				}
			}
		}
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (_title != null)
				{
					_title = value;
				}
			}
		}
		public string Pattern
		{
			get
			{
				return _pattern;
			}
			set
			{
				if (_pattern != null)
				{
					_pattern = value;
				}
			}
		}
		public override string ToString()
		{
			return _file;
		}
	}
	/// <summary>
	/// Data with file
	/// </summary>
	public abstract class NodeDataFile : NodeData
	{
		public event EventHandler OnFilenameChanged = null;
		protected TreeNode _nodes;
		protected FileObject _file;
		public NodeDataFile(TreeNode nodeParent)
		{
			_nodes = nodeParent;
			_file = new FileObject();
			_file.OnFilenameChanged += new EventHandler(_file_OnFilenameChanged);
		}

		void _file_OnFilenameChanged(object sender, EventArgs e)
		{
			if (OnFilenameChanged != null)
			{
				OnFilenameChanged(this, e);
			}
		}
		[Description("A solution file records one or more independent projects under development")]
		[Editor(typeof(FileChooser), typeof(UITypeEditor))]
		public virtual FileObject Filename
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
		public string Folder
		{
			get
			{
				if (_file.Filename != null && _file.Filename.Length > 0)
				{
					string folder = System.IO.Path.GetDirectoryName(_file.Filename);
					if (System.IO.Directory.Exists(folder))
						return folder;
				}
				return "";
			}
		}
		[Browsable(false)]
		public string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(_file.Filename))
				{
					return string.Empty;
				}
				return _file.Filename;
			}
		}
		[Browsable(false)]
		public virtual bool HasFile
		{
			get
			{
				if (_file.Filename != null && _file.Filename.Length > 0)
				{
					string folder = System.IO.Path.GetDirectoryName(_file.Filename);
					if (System.IO.Directory.Exists(folder))
						return true;
				}
				return false;
			}
		}
		public virtual bool SelectFile(IWin32Window owner)
		{
			if (_file.ForOpen)
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = _file.Title;
				dlg.FileName = _file.Filename;
				dlg.Filter = _file.Pattern;
				dlg.CheckFileExists = _file.CheckFileExists;
				if (dlg.ShowDialog(owner) == DialogResult.OK)
				{
					_file.Filename = dlg.FileName;
					return true;
				}
			}
			else
			{
				System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
				dlg.Title = _file.Title;
				dlg.Filter = _file.Pattern;
				dlg.FileName = _file.Filename;
				dlg.CheckFileExists = _file.CheckFileExists;
				dlg.CheckPathExists = _file.CheckFileExists;
				dlg.OverwritePrompt = true;
				if (dlg.ShowDialog(owner) == DialogResult.OK)
				{
					_file.Filename = dlg.FileName;
					return true;
				}
			}
			return false;
		}
		protected virtual void LoadFromXmlNode(XmlNode node)
		{
		}
		public abstract void Save();
		public virtual bool SaveAs(IWin32Window owner)
		{
			if (SelectFile(owner))
			{
				Save();
				return true;
			}
			return false;
		}
	}
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	public class FileChooser : System.Drawing.Design.UITypeEditor
	{
		public FileChooser()
		{
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			FileObject obj = value as FileObject;
			if (obj == null)
				return value;

			if (obj.ForOpen)
			{
				System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = obj.Title;
				dlg.Filter = obj.Pattern;
				dlg.FileName = obj.Filename;
				dlg.CheckFileExists = obj.CheckFileExists;
				dlg.CheckPathExists = true;

				if (dlg.ShowDialog() == DialogResult.OK)
				{
					obj.Filename = dlg.FileName;
					return obj;
				}
			}
			else
			{
				System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
				dlg.Title = obj.Title;
				dlg.Filter = obj.Pattern;
				dlg.FileName = obj.Filename;
				dlg.CheckFileExists = obj.CheckFileExists;
				dlg.CheckPathExists = true;
				dlg.OverwritePrompt = true;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					obj.Filename = dlg.FileName;
					return obj;
				}
			}
			return value;
		}
	}
	/// <summary>
	/// Data for solution
	/// </summary>
	public class NodeDataSolution : NodeDataFile
	{
		#region constants
		public const string XMLATT_File = "file";
		public const string XML_Solution = "Solution";
		public const string FILE_EXT_SOLUTION = "LimnorMain_sln";
		public const string FILE_EXT_PROJECT = "lrproj";
		public const string FILE_EXT_CLASS = "limnor";
		//
		const string PRJ_BEGIN = "Project(";
		const string PRJ_END = "EndProject";
		const string PRJ_DEPEND = "ProjectSection(ProjectDependencies)";
		const string PRJ_ENDSECT = "EndProjectSection";
		#endregion
		//
		#region fields and constructors
		public event EventHandler OnSaved = null;
		//private XmlDocument _doc;
		private Guid _guid;
		/// <summary>
		/// for loading existing solution
		/// </summary>
		/// <param name="solutionFile"></param>
		/// <param name="nodes"></param>
		public NodeDataSolution(string solutionFile, TreeNode node)
			: base(node)
		{
			this.Filename.Filename = solutionFile;
			init();
			//load projects
			Form f = null;
			if (node.TreeView != null)
			{
				f = node.TreeView.FindForm();
			}
			loadProjects(f);
		}
		void init()
		{
			SetName0(Path.GetFileNameWithoutExtension(Filename.Filename));
			this._file.Pattern = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Solution files|*.{0}", FILE_EXT_SOLUTION);
			this._file.Title = "Select solution file";
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public override string Name
		{
			get
			{
				return SolutionFilename;
			}
			set
			{
			}
		}
		[Description("Solution file name")]
		public string SolutionFilename
		{
			get
			{
				return Path.GetFileNameWithoutExtension(Filename.Filename);
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string s = value.Trim();
					if (s.Length > 0)
					{
						s = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", s, NodeDataSolution.FILE_EXT_SOLUTION);
						string newFile = Path.Combine(Path.GetDirectoryName(Filename.Filename), s);
						if (string.Compare(newFile, Filename.Filename, StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (File.Exists(newFile))
							{
								throw new DesignerException("New file exists [{0}]", newFile);
							}
							File.Move(Filename.Filename, newFile);
							Filename.Filename = newFile;
						}
					}
				}
			}
		}
		[Description("Full path for the solution file ")]
		public string SolutionFilePath
		{
			get
			{
				return Filename.Filename;
			}
		}
		[Browsable(false)]
		public override FileObject Filename
		{
			get
			{
				return base.Filename;
			}
			set
			{
				base.Filename = value;
			}
		}

		#endregion

		#region Methods
		public override void Save()
		{
			StreamWriter sw = null;
			try
			{
				string solutionFolder = Path.GetDirectoryName(Filename.Filename);
				StringCollection scSol = new StringCollection();
				string sol = solutionFolder;
				scSol.Add(sol);
				while (sol.Length > 4)
				{
					sol = Path.GetDirectoryName(sol);
					scSol.Add(sol);
				}
				sw = new StreamWriter(Filename.Filename, false, Encoding.Unicode);
				sw.Write(Resource1.SolutionHeader);
				sw.WriteLine();
				for (int i = 0; i < _nodes.Nodes.Count; i++)
				{
					ProjectNode pn = _nodes.Nodes[i] as ProjectNode;
					if (pn != null)
					{
						ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
						sw.Write(PRJ_BEGIN);
						sw.Write("\"");
						if (_guid == Guid.Empty)
						{
							_guid = Guid.NewGuid();
						}
						sw.Write(_guid.ToString("B", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
						sw.Write("\") = \"");
						sw.Write(pnd.Project.ProjectName);
						sw.Write("\", \"");
						//write project file
						string prjFolder = Path.GetDirectoryName(pnd.Project.ProjectFile);
						if (string.Compare(prjFolder, solutionFolder, StringComparison.OrdinalIgnoreCase) == 0)
						{
							sw.Write(Path.GetFileName(pnd.Project.ProjectFile));
						}
						else if (prjFolder[0] != solutionFolder[0])
						{
							sw.Write(pnd.Project.ProjectFile);
						}
						else
						{
							bool bFound = false;
							string sp = prjFolder;
							StringCollection scP = new StringCollection();
							scP.Add(sp);
							while (sp.Length > 4)
							{
								sp = Path.GetDirectoryName(sp);
								scP.Add(sp);
								if (string.Compare(sp, solutionFolder, StringComparison.OrdinalIgnoreCase) == 0)
								{
									bFound = true;
									break;
								}
							}
							if (bFound)
							{
								string sp0 = prjFolder.Substring(sp.Length);
								if (sp0.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
								{
									sp0 = sp0.Substring(1);
								}
								else if (sp0.StartsWith("/", StringComparison.OrdinalIgnoreCase))
								{
									sp0 = sp0.Substring(1);
								}
								sw.Write(Path.Combine(sp0, Path.GetFileName(pnd.Project.ProjectFile)));
							}
							else
							{
								//project not under solution folder
								//solution:  d1\d2\d3\d4
								//           d1\d2\d3
								//           d1\d2
								//project:   d1\d2\s1\s2
								//           d1\d2\s1
								//           d1\d2
								bFound = false;
								string sd = "..";
								string sk = "";
								for (int k = 1; k < scSol.Count; k++)
								{
									for (int m = 0; m < scP.Count; m++)
									{
										if (string.Compare(scSol[k], scP[m], StringComparison.OrdinalIgnoreCase) == 0)
										{
											for (int h = 1; h < k; h++)
											{
												sd += "\\..";
											}
											sk = scSol[k];
											bFound = true;
											break;
										}
									}
									if (bFound)
									{
										break;
									}
								}
								if (bFound)
								{
									string p0 = prjFolder.Substring(sk.Length);
									if (!p0.StartsWith("\\", StringComparison.OrdinalIgnoreCase) && !p0.StartsWith("/", StringComparison.OrdinalIgnoreCase))
									{
										p0 = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\", sd, p0);
									}
									else
									{
										p0 = string.Format(CultureInfo.InvariantCulture, "{0}{1}\\", sd, p0);
									}
									sw.Write(p0);
									sw.Write(Path.GetFileName(pnd.Project.ProjectFile));
								}
								else
								{
									sw.Write(Path.GetFileName(pnd.Project.ProjectFile));
								}
							}
						}
						sw.Write("\", \"");
						sw.Write(pnd.Project.ProjectGuid.ToString("B", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
						sw.WriteLine("\"");
						//write dependences
						IList<Guid> gs = pn.DependedProjects;
						if (gs != null && gs.Count > 0)
						{
							sw.Write("\t");
							sw.Write(PRJ_DEPEND);
							sw.WriteLine(" = postProject");
							foreach (Guid g in gs)
							{
								sw.Write("\t\t");
								sw.Write(g.ToString("B", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
								sw.Write(" = ");
								sw.WriteLine(g.ToString("B", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
							}
							sw.Write("\t");
							sw.WriteLine(PRJ_ENDSECT);
						}
						//
						sw.WriteLine(PRJ_END);

					}
				}
			}
			catch (Exception err)
			{
				MathNode.Log(TraceLogClass.MainForm, err);
			}
			if (sw != null)
			{
				sw.Close();
			}
			if (OnSaved != null)
			{
				OnSaved(this, EventArgs.Empty);
			}
			Dirty = false;
		}
		#endregion

		#region private methods
		private void loadProjects(Form caller)
		{
			if (!string.IsNullOrEmpty(Filename.Filename) && File.Exists(Filename.Filename))
			{
				StreamReader sr = null;
				try
				{
					string solutionDir = Path.GetDirectoryName(Filename.Filename);
					sr = new StreamReader(Filename.Filename, Encoding.Unicode, true);
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						if (!string.IsNullOrEmpty(line))
						{
							line = line.Trim();
							if (line.StartsWith(PRJ_BEGIN, StringComparison.OrdinalIgnoreCase))
							{
								ProjectNode pn = null;
								line = line.Substring(PRJ_BEGIN.Length);
								int n = line.IndexOf('=');
								if (n > 2)
								{
									if (_guid == Guid.Empty)
									{
										string sg = line.Substring(0, n - 1).Trim();
										if (sg.EndsWith(")", StringComparison.OrdinalIgnoreCase))
										{
											sg = sg.Substring(0, sg.Length - 1);
										}
										if (sg.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
										{
											sg = sg.Substring(1);
											if (sg.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
											{
												sg = sg.Substring(0, sg.Length - 1);
											}
										}
										_guid = new Guid(sg);
									}
									line = line.Substring(n + 1).Trim();
									string[] ss = line.Split(',');
									if (ss != null && ss.Length == 3)
									{
										if (!string.IsNullOrEmpty(ss[1]))
										{
											string file = ss[1].Trim();
											if (file.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
											{
												file = file.Substring(1);
												if (file.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
												{
													file = file.Substring(0, file.Length - 1);
												}
											}
											file = file.Trim();
											if (file.IndexOf(':') < 0)
											{
												string folder = solutionDir;
												//file is a relative path
												while (file.StartsWith("..", StringComparison.OrdinalIgnoreCase))
												{
													file = file.Substring(2);
													if (file.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
													{
														file = file.Substring(1);
													}
													else if (file.StartsWith("/", StringComparison.OrdinalIgnoreCase))
													{
														file = file.Substring(1);
													}
													folder = Path.GetDirectoryName(folder);
												}
												file = Path.Combine(folder, file);
											}
											if (File.Exists(file))
											{
												LimnorProject lp = new LimnorProject(file);
												pn = new ProjectNode(lp);
												_nodes.Nodes.Add(pn);
											}
											else
											{
												MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Project file not exist [{0}]", file), "Load projects", MessageBoxButtons.OK, MessageBoxIcon.Error);
											}
										}
									}
								}
								//
								while (!sr.EndOfStream)
								{
									line = sr.ReadLine();
									if (!string.IsNullOrEmpty(line))
									{
										line = line.Trim();
										if (line.StartsWith(PRJ_DEPEND, StringComparison.OrdinalIgnoreCase))
										{
											while (!sr.EndOfStream)
											{
												line = sr.ReadLine();
												if (!string.IsNullOrEmpty(line))
												{
													line = line.Trim();
													if (line.StartsWith(PRJ_ENDSECT, StringComparison.OrdinalIgnoreCase))
													{
														break;
													}
													if (pn != null)
													{
														int n0 = line.IndexOf('=');
														if (n0 > 0)
														{
															line = line.Substring(0, n0).Trim();
														}
														Guid gd = new Guid(line);
														pn.AddDependedProject(gd);
													}
												}
											}
										}
										else
										{
											if (line.StartsWith(PRJ_END, StringComparison.OrdinalIgnoreCase))
											{
												break;
											}
										}
									}
								}
							}
						}
					}
					sr.Close();
				}
				catch (Exception err)
				{
					MathNode.Log(TraceLogClass.MainForm, err);
				}
				if (sr != null)
				{
					sr.Close();
				}
			}
		}
		#endregion
	}
	/// <summary>
	/// node for solution
	/// </summary>
	public class SolutionNode : TreeNodeClass
	{
		private XmlDocument _cfgDoc;
		const string XMLATT_include = "include";
		public SolutionNode(string solutionFile)
		{
			Obj = new NodeDataSolution(solutionFile, this);
			Obj.OnNameChanged += new EventHandler(Obj_OnNameChanged);
			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;
			RefreshNameDisplay();
		}
		private void Obj_OnNameChanged(object sender, EventArgs e)
		{
			RefreshNameDisplay();
		}
		private string getConfigFile()
		{
			NodeDataSolution nd = Obj as NodeDataSolution;
			return string.Format(CultureInfo.InvariantCulture, "{0}.cfg", nd.FilePath);
		}
		private XmlDocument cfgDoc
		{
			get
			{
				if (_cfgDoc == null)
				{
					_cfgDoc = new XmlDocument();
					string f = getConfigFile();
					if (File.Exists(f))
					{
						_cfgDoc.Load(f);
					}

				}
				if (_cfgDoc.DocumentElement == null)
				{
					XmlNode r = _cfgDoc.CreateElement("Projects");
					_cfgDoc.AppendChild(r);
				}
				return _cfgDoc;
			}
		}
		public bool IsProjectBuildIncluded(ProjectNode projectNode)
		{
			ProjectNodeData pdata = projectNode.PropertyObject as ProjectNodeData;
			XmlDocument doc = cfgDoc;
			XmlNode pn = doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_Item, XmlTags.XMLATT_guid, VPLUtil.GuidToString(pdata.Project.ProjectGuid)));
			if (pn == null)
			{
				return true;
			}
			return XmlUtil.GetAttributeBoolDefTrue(pn, XMLATT_include);
		}
		public void SetProjectInclude(Guid gid, bool include)
		{
			XmlDocument doc = cfgDoc;
			XmlNode pn = doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_Item, XmlTags.XMLATT_guid, VPLUtil.GuidToString(gid)));
			if (pn == null)
			{
				pn = doc.CreateElement(XmlTags.XML_Item);
				XmlUtil.SetAttribute(pn, XmlTags.XMLATT_guid, VPLUtil.GuidToString(gid));
				doc.DocumentElement.AppendChild(pn);
			}
			XmlUtil.SetAttribute(pn, XMLATT_include, include);
		}
		public void SaveConfig()
		{
			XmlDocument doc = cfgDoc;
			doc.Save(getConfigFile());
		}
		public bool ProjectConfig()
		{
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			DialogBuildConfig dlg = new DialogBuildConfig();
			dlg.LoadData(this);
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				return true;
			}
			return false;
		}
		public ProjectNode[] GetProjectBuilderOrder()
		{
			List<ProjectDependencies> projects = GetDependencies();
			List<ProjectDependencies> orders = SolutionNode.GetBuildOrder(projects);
			ProjectNode[] pnodes = new ProjectNode[orders.Count];
			for (int i = 0; i < orders.Count; i++)
			{
				pnodes[i] = GetProjectByGuid(orders[i].Project.ProjectGuid);
			}
			return pnodes;
		}
		public List<ProjectDependencies> GetDependencies()
		{
			List<ProjectDependencies> lst = new List<ProjectDependencies>();
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode pn = Nodes[i] as ProjectNode;
				if (pn != null)
				{
					ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
					LimnorProject lp = pnd.Project;
					IList<Guid> pids = pn.DependedProjects;
					List<LimnorProject> ps = new List<LimnorProject>();
					if (pids != null && pids.Count > 0)
					{
						foreach (Guid g in pids)
						{
							LimnorProject dp = LimnorSolution.Solution.GetProjectByGuid(g);
							if (dp != null)
							{
								ps.Add(dp);
							}
						}
					}
					ProjectDependencies pds = new ProjectDependencies(lp, ps);
					lst.Add(pds);
				}
			}
			return lst;
		}
		public static List<ProjectDependencies> GetBuildOrder(List<ProjectDependencies> projects)
		{
			List<ProjectDependencies> orders = new List<ProjectDependencies>();
			orders.AddRange(projects);
			//go through each project
			foreach (ProjectDependencies pd in projects)
			{
				//pd is one project to be tested
				List<LimnorProject> pds = pd.Dependencies;
				if (pds.Count > 0)
				{
					int n = -1;
					for (int i = 0; i < orders.Count; i++)
					{
						if (pd.Project.ProjectGuid == orders[i].Project.ProjectGuid)
						{
							n = i;
							break;
						}
					}
					//n is the index of pd in orders
					if (n >= 0)
					{
						//go through dependencies of pd
						foreach (LimnorProject p in pds)
						{
							ProjectDependencies pdx = null;
							int k = -1;
							for (int i = 0; i < orders.Count; i++)
							{
								if (p.ProjectGuid == orders[i].Project.ProjectGuid)
								{
									pdx = orders[i];
									k = i;
									break;
								}
							}
							//k is the dependent index in orders
							if (k >= 0)
							{
								if (k > n)
								{
									orders.RemoveAt(k);

									orders.Insert(n, pdx);
									n++;
								}
							}
						}
					}
				}
			}
			return orders;
		}
		public void ChangeProjectBuildOrder()
		{
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			DialogProjectBuildOrder dlg = new DialogProjectBuildOrder();
			dlg.LoadData(this);
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					ProjectNode prjNode = Nodes[i] as ProjectNode;
					if (prjNode != null)
					{
						ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
						foreach (ProjectDependencies pd0 in dlg.ProjectDependencies)
						{
							if (pd0.Project.ProjectGuid == prjData.Project.ProjectGuid)
							{
								prjNode.SetDependencies(pd0.GetDependentProjectIDlist());
								break;
							}
						}
					}
				}
				Dirty = true;
			}
		}
		public bool ProjectFileExists(string file)
		{
			return (GetProjectNodeByFile(file) != null);
		}
		public ProjectNode GetProjectNodeByFile(string file)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode prjNode = Nodes[i] as ProjectNode;
				if (prjNode != null)
				{
					ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
					if (string.Compare(file, prjData.Filename.Filename, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return prjNode;
					}
				}
			}
			return null;
		}
		public bool ProjectNameExists(string name)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode prjNode = Nodes[i] as ProjectNode;
				if (prjNode != null)
				{
					ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
					if (string.Compare(name, prjData.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		public ProjectNode GetProjectByName(string name)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode prjNode = Nodes[i] as ProjectNode;
				if (prjNode != null)
				{
					ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
					if (string.Compare(name, prjData.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return prjNode;
					}
				}
			}
			return null;
		}
		public ProjectNode GetProjectByGuid(Guid g)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode prjNode = Nodes[i] as ProjectNode;
				if (prjNode != null)
				{
					ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
					if (prjData.Project.ProjectGuid == g)
					{
						return prjNode;
					}
				}
			}
			return null;
		}
		public bool ProjectProgramNameExists(string name)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				ProjectNode prjNode = Nodes[i] as ProjectNode;
				if (prjNode != null)
				{
					ProjectNodeData prjData = (ProjectNodeData)prjNode.PropertyObject;
					if (string.Compare(name, prjData.ProgramName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void RefreshNameDisplay()
		{
			Text = ToString();
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Obj.Name))
				Obj.Name = "Solution1";
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Solution '{0}' ({1} projects)", Obj.Name, LimnorSolution.Solution.Count);
		}
	}
}
