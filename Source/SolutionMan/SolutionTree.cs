/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using VOB;
using MathExp;
using FileUtil;
using LimnorUI;
using VPL;
using XmlUtility;
using VSPrj;
using System.Globalization;
using LimnorDesigner;
using SolutionMan.Solution;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using WindowsUtility;

namespace SolutionMan
{
	public partial class SolutionTree : UserControl, IChangeControl
	{
		#region fields and constructors
		public event EventHandler OnSaved = null;

		private IServiceProvider _serviceProvider;
		private InterfaceVOB _InterfaceVOB;
		private PropertyValueChangedEventHandler _lastPropertyValueHandler = null;

		public const int IMG_FORM = 1;
		public const int IMG_COMP = 2;
		public const int IMG_RESX = 3;
		public const int IMG_DLL_0 = 4;
		public const int IMG_DLL_1 = 5;
		public const int IMG_CTL_0 = 6;
		public const int IMG_CTL_1 = 7;
		public const int IMG_WIN_0 = 8;
		public const int IMG_WIN_1 = 9;
		public const int IMG_DOS_0 = 10;
		public const int IMG_DOS_1 = 11;
		public const int IMG_GO = 12;
		public const int IMG_DOC = 13;
		public const int IMG_DOCS = 14;
		public SolutionTree()
		{
			InitializeComponent();
			this.treeView1.HideSelection = false;
			_dirty = false;
		}
		#endregion

		#region private methods
		private void validateSolution(object sender, EventArgs e)
		{
			EventArgCancel ev = (EventArgCancel)e;
			NodeDataSolution sln = (NodeDataSolution)ev.Value;
			if (string.IsNullOrEmpty(sln.Name))
			{
				ev.Message = "Name missing";
				ev.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(sln.Filename.Filename))
			{
				ev.Message = "Filename missing";
				ev.Cancel = true;
				return;
			}
		}
		/// <summary>
		/// validate project parameters
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void validateProject(object sender, EventArgs e)
		{
			NameCreation nc = new NameCreation();
			EventArgCancel ev = (EventArgCancel)e;
			ProjectNodeData prj = (ProjectNodeData)ev.Value;
			if (string.IsNullOrEmpty(prj.Name))
			{
				ev.Message = "Project Name missing";
				ev.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(prj.Filename.Filename))
			{
				ev.Message = "Filename missing";
				ev.Cancel = true;
				return;
			}
			if (!FileUtil.FileUtilities.IsFilepathValid(prj.Filename.Filename))
			{
				ev.Message = "Invalid project filename or path";
				ev.Cancel = true;
				return;
			}
			if (System.IO.File.Exists(prj.Filename.Filename))
			{
				ev.Message = "Project filename exists";
				ev.Cancel = true;
				return;
			}
			SolutionNode slnNode = GetCurrentSolution();
			if (slnNode != null)
			{
				if (slnNode.ProjectFileExists(prj.Filename.Filename))
				{
					ev.Message = "Project filename is already used";
					ev.Cancel = true;
					return;
				}
				if (slnNode.ProjectNameExists(prj.Name))
				{
					ev.Message = "Project name is already used";
					ev.Cancel = true;
					return;
				}
				if (slnNode.ProjectProgramNameExists(prj.ProgramName))
				{
					ev.Message = "Project program name is already used";
					ev.Cancel = true;
					return;
				}
			}
			if (string.IsNullOrEmpty(prj.Namespace))
			{
				ev.Message = "Namespace missing";
				ev.Cancel = true;
				return;
			}
			if (!nc.IsValidName(prj.Namespace))
			{
				ev.Message = "Invalid namespace.";
				ev.Cancel = true;
				return;
			}
		}
		private SolutionNode createSolutionNode(string solutionFile)
		{
			SolutionNode nd = new SolutionNode(solutionFile);
			NodeDataSolution solutionData = nd.PropertyObject as NodeDataSolution;
			solutionData.OnSaved += new EventHandler(SolutionTree_OnSaved);
			solutionData.OnFilenameChanged += new EventHandler(solutionData_OnFilenameChanged);
			return nd;
		}
		private void solutionData_OnFilenameChanged(object sender, EventArgs e)
		{
			if (_InterfaceVOB != null)
			{
				_InterfaceVOB.SendNotice(enumVobNotice.SolutionFilenameChange, e);
			}
		}

		private void treeView1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
				if (tn != null)
				{
					treeView1.SelectedNode = tn;
					MenuItem mi;
					ContextMenu menu = new ContextMenu();
					SolutionNode slnNode = this.SelectedNode as SolutionNode;
					if (slnNode != null)
					{
						mi = new MenuItemWithBitmap("Add new project", new EventHandler(onAddNewProject), Resource1._newIcon.ToBitmap());
						mi.Tag = slnNode;
						menu.MenuItems.Add(mi);
						mi = new MenuItemWithBitmap("Add existing project", new EventHandler(onAddExistingProject), Resource1._dialog.ToBitmap());
						mi.Tag = slnNode;
						menu.MenuItems.Add(mi);
					}
					ProjectNode prjNode = this.SelectedNode as ProjectNode;
					if (prjNode != null)
					{
						MenuItem mi0 = new MenuItemWithBitmap("Build", new EventHandler(onBuild), Resource1._build.ToBitmap());
						mi0.Tag = prjNode;
						menu.MenuItems.Add(mi0);
						//
						menu.MenuItems.Add("-");
						//
						MenuItem miAdd = new MenuItemWithBitmap("Add", Resource1._newIcon.ToBitmap());
						menu.MenuItems.Add(miAdd);
						//
						mi = new MenuItemWithBitmap("New Item...", new EventHandler(onAddItem), Resource1._newItem.ToBitmap());
						mi.Tag = prjNode;
						miAdd.MenuItems.Add(mi);
						//
						mi = new MenuItemWithBitmap("Existing Item...", new EventHandler(onAddExistItem), Resource1._newItem.ToBitmap());
						mi.Tag = prjNode;
						miAdd.MenuItems.Add(mi);
						//
						miAdd.MenuItems.Add("-");
						//
						ProjectNodeData pnd = prjNode.PropertyObject as ProjectNodeData;
						if (pnd.Project.ProjectType == EnumProjectType.WebAppAspx || pnd.Project.ProjectType == EnumProjectType.WebAppPhp)
						{
							mi = new MenuItemWithBitmap("Web page...", new EventHandler(onAddWebPage), Resource1._newForm.ToBitmap());
							mi.Tag = prjNode;
							miAdd.MenuItems.Add(mi);
						}
						else
						{
							mi = new MenuItemWithBitmap("Windows Form...", new EventHandler(onAddForm), Resource1._newForm.ToBitmap());
							mi.Tag = prjNode;
							miAdd.MenuItems.Add(mi);
							//
							mi = new MenuItemWithBitmap("User Control...", new EventHandler(onAddUserControl), Resource1._newUserControl.ToBitmap());
							mi.Tag = prjNode;
							miAdd.MenuItems.Add(mi);
						}
						//
						miAdd.MenuItems.Add("-");
						//
						mi = new MenuItemWithBitmap("Component...", new EventHandler(onAddComponent), Resource1._newComponent.ToBitmap());
						mi.Tag = prjNode;
						miAdd.MenuItems.Add(mi);
						//
						mi = new MenuItemWithBitmap("Class...", new EventHandler(onAddClass), Resource1._newClass.ToBitmap());
						mi.Tag = prjNode;
						miAdd.MenuItems.Add(mi);
						//
						menu.MenuItems.Add("-");
						//
						mi = new MenuItemWithBitmap("Exclude from solution", new EventHandler(onExcludeProject), Resource1._cancel);
						mi.Tag = prjNode;
						menu.MenuItems.Add(mi);
					}
					TreeNodeClass nd = this.treeView1.SelectedNode as TreeNodeClass;
					if (nd != null)
					{
						if (nd is ComponentNode)
						{
							ComponentNode ndComponent = (ComponentNode)nd;

							NodeObjectComponent data = (NodeObjectComponent)ndComponent.PropertyObject;
							mi = new MenuItemWithBitmap("Open", new EventHandler(onOpenComponent), Resource1._openFolder.ToBitmap());
							mi.Tag = data;
							menu.MenuItems.Add(mi);
							if (nd.CanRemove)
							{
								mi = new MenuItemWithBitmap("Delete", new EventHandler(onExcludeComponent), Resource1._cancel);
								mi.Tag = nd;
								menu.MenuItems.Add(mi);
								menu.MenuItems.Add("-");
								mi = new MenuItemWithBitmap("Make duplication", new EventHandler(onDuplicateComponent), Resource1._duplicate.ToBitmap());
								mi.Tag = nd;
								menu.MenuItems.Add(mi);
							}
						}
					}
					if (menu.MenuItems.Count > 0)
					{
						menu.Show(this, new Point(e.X, e.Y));
					}
				}
			}
		}
		private void SolutionTree_OnSaved(object sender, EventArgs e)
		{
			if (OnSaved != null)
			{
				OnSaved(sender, e);
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeClass nd = e.Node as TreeNodeClass;
			if (nd != null)
			{
				ProjectNode pn = nd as ProjectNode;
				if (pn != null)
				{
					TreeNodeCollection nds = nd.Parent.Nodes;
					for (int i = 0; i < nds.Count; i++)
					{
						((ProjectNodeData)((ProjectNode)nds[i]).PropertyObject).IsActive = (nd.Index == i);
					}
				}

				if (_serviceProvider != null)
				{
					System.Windows.Forms.PropertyGrid pg = _serviceProvider.GetService(typeof(System.Windows.Forms.PropertyGrid)) as System.Windows.Forms.PropertyGrid;
					if (pg != null)
					{
						pg.SelectedObject = nd.PropertyObject;
						if (_lastPropertyValueHandler != null)
						{
							pg.PropertyValueChanged -= _lastPropertyValueHandler;
						}
						_lastPropertyValueHandler = new PropertyValueChangedEventHandler(nd.OnPropertyChanged);
						pg.PropertyValueChanged += _lastPropertyValueHandler;
					}
				}
				if (pn != null)
				{
					if (_InterfaceVOB != null)
					{
						_InterfaceVOB.SendNotice(enumVobNotice.ProjectNodeSelected, pn);
					}
				}
			}
		}

		private void onAddNewProject(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				SolutionNode slnNode = mi.Tag as SolutionNode;
				if (slnNode != null)
				{
					CreateNewProject(slnNode);
				}
			}
		}
		private bool createSolutionFile(SolutionNode solution)
		{
			NodeDataSolution snld = solution.PropertyObject as NodeDataSolution;
			bool bExists = false;
			if (!string.IsNullOrEmpty(snld.Filename.Filename))
			{
				bExists = File.Exists(snld.Filename.Filename);
			}
			if (bExists)
			{
				return true;
			}
			else
			{
				if (MessageBox.Show(this.FindForm(), "A solution file has not created. Do you want to create a solution file now?", "Add project to solution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return false;
				}
				if (string.IsNullOrEmpty(snld.Filename.Filename))
				{
					for (int i = 0; i < solution.Nodes.Count; i++)
					{
						ProjectNode pn = solution.Nodes[i] as ProjectNode;
						if (pn != null)
						{
							string prjFile = ((ProjectNodeData)(pn.PropertyObject)).ProjectFile;
							snld.Filename.Filename = Path.Combine(Path.GetDirectoryName(prjFile), string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Path.GetFileNameWithoutExtension(prjFile), NodeDataSolution.FILE_EXT_SOLUTION));
							break;
						}
					}
				}
				if (snld.SelectFile(this.FindForm()))
				{
					snld.Dirty = true;
					snld.Save();
					solution.RefreshNameDisplay();
					_InterfaceVOB.SendNotice(enumVobNotice.SolutionFileCreated, snld.Filename.Filename);
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		private void onAddExistingProject(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				SolutionNode slnNode = mi.Tag as SolutionNode;
				if (slnNode != null)
				{
					if (createSolutionFile(slnNode))
					{
						OpenFileDialog dlg = new OpenFileDialog();
						dlg.Title = "Select an existing Limnor project";
						dlg.Filter = "Limnor projects|*.lrproj";
						dlg.CheckFileExists = true;
						dlg.CheckPathExists = true;
						if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
						{
							if (!slnNode.ProjectFileExists(dlg.FileName))
							{
								ProjectNode prjNode = new ProjectNode(new LimnorProject(dlg.FileName));
								slnNode.Nodes.Add(prjNode);
								slnNode.Expand();
								slnNode.Dirty = true;
								treeView1.SelectedNode = prjNode;
								slnNode.RefreshNameDisplay();
							}
						}
					}
				}
			}
		}
		private void onBuild(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					SendNotice(enumVobNotice.ProjectBuild, prjNode);
				}
			}
		}
		private void onAddItem(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, null);
				}
			}
		}
		private void addFileToProject(ProjectNode projectNode, string filename, bool open)
		{
			projectNode.Expand();
			//
			ProjectNodeData pnd = projectNode.PropertyObject as ProjectNodeData;
			ClassData cd = pnd.Project.AddClass(Path.GetFileName(filename), open);
			ComponentNode cn = new ComponentNode(cd, pnd);
			//
			projectNode.Nodes.Add(cn);
			//
			this.treeView1.SelectedNode = cn;
			//
			pnd.Project.ToolboxLoaded = false;
			VPLUtil.RemoveDialogCaches(pnd.Project.ProjectGuid, 0);
			IList<ClassData> classes = pnd.Project.GetAllClasses();
			if (classes != null)
			{
			}
			pnd.Project.AddModifiedClass(cd.ComponentId);
			pnd.Project.SaveModifiedClassIDs(false);
			if (open)
			{
				_InterfaceVOB.SendNotice(enumVobNotice.ObjectCreated, cn.PropertyObject);
			}
			//
		}
		private bool importExistingItem(ProjectNode prjNode,string file, bool rename)
		{
			ProjectNodeData pnd = prjNode.PropertyObject as ProjectNodeData;
			string fn = Path.Combine(pnd.Project.ProjectFolder, Path.GetFileName(file));
			if (File.Exists(fn))
			{
				//do not overwrite existing file
				if (string.Compare(fn, file, StringComparison.OrdinalIgnoreCase) != 0)
				{
					MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "File already exists:[{0}]", fn), "Add file to project", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
			}
			//
			string guidstr = null;
			UInt32 classId = 0;
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(file);
				if (doc.DocumentElement == null)
				{
					MessageBox.Show(this.FindForm(), "Document is empty", "Add existing item", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					guidstr = XmlUtil.GetAttribute(doc.DocumentElement, XmlTags.XMLATT_guid);
					classId = XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID);
					if (classId == 0)
					{
						MessageBox.Show(this.FindForm(), "Document is not a Limnor Studio item file", "Add existing item", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "Invalid file: [{0}]. {1}", file, err.Message), "Add file to project", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (classId != 0)
			{
				if (rename)
				{
					string newName = Path.GetFileNameWithoutExtension(file);
					XmlUtil.SetAttribute(doc.DocumentElement, "name", newName);
					XmlNode propNode = doc.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
						"{0}[@name=\"Name\"]", XmlTags.XML_PROPERTY));
					if (propNode != null)
					{
						propNode.InnerText = newName;
					}
				}
				ComponentID cid = pnd.Project.GetComponentByID(classId);
				if (cid != null)
				{
					UInt32 newId=(UInt32)(Guid.NewGuid().GetHashCode());
					//create new class Id in the doc
					XmlNodeList nds = doc.DocumentElement.SelectNodes(string.Format(CultureInfo.InvariantCulture,
						"//*[@ClassID=\"{0}\"]", classId));
					foreach (XmlNode nd in nds)
					{
						XmlUtil.SetAttribute(nd, "ClassID", newId);
					}
					nds = doc.DocumentElement.SelectNodes(string.Format(CultureInfo.InvariantCulture,
						"//{0}[@name=\"ClassId\"]", XmlTags.XML_PROPERTY));
					foreach (XmlNode nd in nds)
					{
						UInt32 u;
						if (UInt32.TryParse(nd.InnerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out u))
						{
							if (u == classId)
							{
								nd.InnerText = newId.ToString(CultureInfo.InvariantCulture);
							}
						}
					}
				}
				bool bOK = false;
				if (string.Compare(fn, file, StringComparison.OrdinalIgnoreCase) == 0)
				{
					bOK = true; //no need copy
				}
				else
				{
					try
					{
						File.Copy(file, fn, false);
						bOK = true;
					}
					catch (Exception err)
					{
						MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "Error copying file: [{0}] to [{1}]. {2}", file, fn, err.Message), "Add file to project", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				if (bOK)
				{
					Guid newGuid = Guid.NewGuid();
					string newguidstr = newGuid.ToString("D",CultureInfo.InvariantCulture);
					XmlUtil.SetAttribute(doc.DocumentElement, XmlTags.XMLATT_guid, newguidstr);
					XmlUtil.SetAttribute(doc.DocumentElement, XmlTags.XMLATT_filename, fn);
						doc.Save(fn);
					if (!string.IsNullOrEmpty(guidstr))
					{
						StreamReader sr = new StreamReader(fn,true);
						Encoding en = sr.CurrentEncoding;
						string contents = sr.ReadToEnd();
						sr.Close();
						if (contents.IndexOf(guidstr, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							contents = StringUtility.Replace(contents, guidstr, newguidstr, StringComparison.OrdinalIgnoreCase);
							StreamWriter sw = new StreamWriter(fn, false, en);
							sw.Write(contents);
							sw.Close();
						}
					}
					addFileToProject(prjNode, fn, false);
				}
				return bOK;
			}
			return false;
		}
		private void onAddExistItem(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					OpenFileDialog dlg = new OpenFileDialog();
					dlg.Title = "Select Limnor Studio item file";
					dlg.Filter = "Limnor Studio Item Files|*.limnor";
					dlg.CheckFileExists = true;
					dlg.ReadOnlyChecked = false;
					if (dlg.ShowDialog(this) == DialogResult.OK)
					{
						importExistingItem(prjNode, dlg.FileName, false);
					}
				}
			}
		}
		private void onAddClass(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, "Class");
				}
			}
		}
		private void onAddWebPage(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, "WebPage");
				}
			}
		}
		private void onAddForm(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, "Form");
				}
			}
		}
		private void onAddUserControl(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, "UserControl");
				}
			}
		}
		private void onAddComponent(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					CreateNewProjectItem(prjNode, "Component");
				}
			}
		}
		private void onExcludeProject(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ProjectNode prjNode = mi.Tag as ProjectNode;
				if (prjNode != null)
				{
					if (prjNode.Dirty)
					{
						if (MessageBox.Show(this.FindForm(), "Do you want to save modifications?", "Exclude project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						{
							((ProjectNodeData)(prjNode.PropertyObject)).Save();
						}
					}
					SolutionNode slnNode = null;
					TreeNode nd = prjNode.Parent;
					while (nd != null)
					{
						slnNode = nd as SolutionNode;
						if (slnNode != null)
							break;
						nd = prjNode.Parent;
					}
					if (slnNode != null)
					{
						//
						for (int i = 0; i < prjNode.Nodes.Count; i++)
						{
							ComponentNode cNode = prjNode.Nodes[i] as ComponentNode;
							if (cNode != null)
							{
								VobService.SendNotice(enumVobNotice.ObjectClose, cNode.PropertyObject);
							}
						}
						prjNode.Remove();
						slnNode.Dirty = true;
						slnNode.RefreshNameDisplay();
					}
				}
			}
		}
		private void onOpenComponent(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				NodeObjectComponent data = mi.Tag as NodeObjectComponent;
				if (data != null)
				{
					if (data.Holder == null)
					{
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectOpen, data);
					}
					else
					{
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectActivate, data);
					}
				}
			}
		}
		private void onDuplicateComponent(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ComponentNode ndComponent = mi.Tag as ComponentNode;
				if (ndComponent != null)
				{
					NodeObjectComponent data = ndComponent.PropertyObject as NodeObjectComponent;
					ProjectNode nodePrj = ndComponent.Parent as ProjectNode;
					ProjectNodeData prjData = ((ProjectNodeData)(nodePrj.PropertyObject));
					LimnorProject prj = prjData.Project;
					StringCollection names = new StringCollection();
					for (int i = 0; i < ndComponent.Parent.Nodes.Count; i++)
					{
						ComponentNode cn = ndComponent.Parent.Nodes[i] as ComponentNode;
						if (cn != null)
						{
							names.Add(cn.PropertyObject.Name);
						}
					}
					FormNewCName dlg = new FormNewCName();
					dlg.LoadData(names,prj.ProjectFolder);
					if (dlg.ShowDialog(ndComponent.TreeView.FindForm()) == DialogResult.OK)
					{
						string htmlFile = Path.Combine(prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}_design.html",Path.GetFileNameWithoutExtension( data.Filepath)));
						string cssFile = Path.Combine(prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}_design.css", Path.GetFileNameWithoutExtension(data.Filepath)));
						string newHtmlFile = null;
						string newCssFile = null;
						string newFile = Path.Combine(prj.ProjectFolder,string.Format(CultureInfo.InvariantCulture,"{0}.limnor", dlg.Ret));
						if (prj.IsWebApplication)
						{
							newHtmlFile = Path.Combine(prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}_design.html", dlg.Ret));
							if (File.Exists(newHtmlFile))
							{
								if (MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "File exists:{0}\r\n\r\nDo you want to override this file?", newHtmlFile), "Duplicate web page", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
								{
									return;
								}
							}
							newCssFile = Path.Combine(prj.ProjectFolder, string.Format(CultureInfo.InvariantCulture, "{0}_design.css", dlg.Ret));
							if (File.Exists(newCssFile))
							{
								if (MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "File exists:{0}\r\n\r\nDo you want to override this file?", newCssFile), "Duplicate web page", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
								{
									return;
								}
							}
						}
						try
						{
							File.Copy(data.Filepath, newFile);
							if (importExistingItem(nodePrj, newFile, true))
							{
								if (prj.IsWebApplication)
								{
									if (File.Exists(htmlFile))
									{
										File.Copy(htmlFile, newHtmlFile, true);
										if (File.Exists(cssFile))
										{
											File.Copy(cssFile, newCssFile, true);
										}
										else
										{
											if (File.Exists(newCssFile))
											{
												File.Delete(newCssFile);
											}
										}
										StreamReader sr = new StreamReader(htmlFile, true);
										Encoding en = sr.CurrentEncoding;
										string html = sr.ReadToEnd();
										sr.Close();
										string cssName = Path.GetFileName(cssFile);
										int pos = html.IndexOf(cssName, StringComparison.OrdinalIgnoreCase);
										if (pos >= 0)
										{
											html = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", html.Substring(0, pos), Path.GetFileName(newCssFile), html.Substring(pos + cssName.Length));
											StreamWriter sw = new StreamWriter(newHtmlFile, false, en);
											sw.Write(html);
											sw.Close();
										}
									}
									else
									{
										if (File.Exists(newHtmlFile))
										{
											File.Delete(newHtmlFile);
										}
										if (File.Exists(newCssFile))
										{
											File.Delete(newCssFile);
										}
									}
								}
							}
						}
						catch (Exception e0)
						{
							MessageBox.Show(this.FindForm(), string.Format(CultureInfo.InvariantCulture, "Error making duplication of file [{0}] into {1}. {2}", data.Filepath, newFile, e0.Message), "Make duplication", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
		}
		private void onExcludeComponent(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ComponentNode ndComponent = mi.Tag as ComponentNode;
				if (ndComponent != null)
				{
					if (MessageBox.Show(this.FindForm(), "Do you want to delete the component?", this.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						NodeObjectComponent data = ndComponent.PropertyObject as NodeObjectComponent;
						ProjectNode nodePrj = ndComponent.Parent as ProjectNode;
						ProjectNodeData prjData = ((ProjectNodeData)(nodePrj.PropertyObject));
						//
						List<ObjectTextID> list = prjData.GetClassUsages(data.Class.ComponentId);
						if (list.Count > 0)
						{
							dlgObjectUsage dlg = new dlgObjectUsage();
							dlg.LoadData("Cannot delete this component. It is being used by the following objects", string.Format("Component - {0}", data.Name), list);
							dlg.ShowDialog();
							return;
						}
						//
						prjData.Project.RemoveClass(Path.GetFileName(data.Class.ComponentFile));
						//
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectClose, data);
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectDeleted, data);

						nodePrj.Nodes.Remove(ndComponent);
						prjData.RemoveComponent(data.Class.ComponentId);
						prjData.Dirty = true;
					}
				}
			}
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNodeClass nd = e.Node as TreeNodeClass;
			if (nd != null)
			{
				IParentNode ipn = nd as IParentNode;
				if (ipn != null)
				{
					if (!ipn.NextLevelLoaded)
					{
						List<TreeNodeLoader> ls = new List<TreeNodeLoader>();
						for (int i = 0; i < nd.Nodes.Count; i++)
						{
							TreeNodeLoader l = nd.Nodes[i] as TreeNodeLoader;
							if (l != null)
							{
								ls.Add(l);
							}
						}
						foreach (TreeNodeLoader l in ls)
						{
							l.Remove();
						}
						foreach (TreeNodeLoader l in ls)
						{
							ipn.LoadNextLevel(l);
						}
					}
				}
			}
		}

		private void treeView1_DoubleClick(object sender, EventArgs e)
		{
			ComponentNode cn = this.SelectedNode as ComponentNode;
			if (cn != null)
			{
				NodeObjectComponent data = cn.PropertyObject as NodeObjectComponent;
				if (data != null)
				{
					if (data.Holder == null)
					{
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectOpen, data);
					}
					else
					{
						_InterfaceVOB.SendNotice(enumVobNotice.ObjectActivate, data);
					}
				}
			}
		}


		#endregion

		#region Methods
		public SolutionNode CreateNewSolution(string file)
		{
			Save();
			if (!Dirty)
			{
				SolutionNode nd = createSolutionNode(file);
				NodeDataSolution solutionData = nd.PropertyObject as NodeDataSolution;
				if (dlgObject.EditObject(this.FindForm(), nd.PropertyObject, "New Solution", new EventHandler(validateSolution)))
				{
					solutionData.OnSaved += new EventHandler(SolutionTree_OnSaved);
					this.treeView1.Nodes.Clear();
					this.treeView1.Nodes.Add(nd);
					_dirty = true;
					nd.Dirty = true;
					return nd;
				}
			}
			return null;
		}

		public bool CreateNewProjectItem(ProjectNode projectNode, string select)
		{
			try
			{
				ProjectNodeData pnd = projectNode.PropertyObject as ProjectNodeData;
				DialogNewClass dlg = new DialogNewClass();
				dlg.Loaddata(pnd.Project, select);
				if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					ItemSelection result = dlg.Result;
					StreamReader sr = new StreamReader(result.ClassTemplate);
					string c = sr.ReadToEnd();
					sr.Close();
					bool isPhp = (projectNode.Project.ProjectType == EnumProjectType.WebAppPhp);
					foreach (KeyValuePair<string, string> kv in result.ReplaceNames)
					{
						if (isPhp)
						{
							if (string.CompareOrdinal(kv.Key, "$safeitemname$") == 0)
							{
								string sn = kv.Value;
								int n=1;
								while (WebPageCompilerUtility.IsReservedPhpWord(sn))
								{
									n++;
									sn = string.Format(CultureInfo.InvariantCulture, "{0}{1}", kv.Value, n);
								}
								c = c.Replace(kv.Key, sn);
								continue;
							}
						}
						c = c.Replace(kv.Key, kv.Value);
					}
					StreamWriter sw = new StreamWriter(result.NewClassFile, false, Encoding.UTF8);
					sw.Write(c);
					sw.Close();
					//
					addFileToProject(projectNode, result.NewClassFile, true);
					return true;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(this.FindForm(), e.Message, "Add item", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return false;
		}
		/// <summary>
		/// create a new project and add it to the current solution.
		/// if there is not a solution then a new solution is created.
		/// </summary>
		/// <returns></returns>
		public bool CreateNewProject(SolutionNode solution)
		{
			DialogNewProject dlg = new DialogNewProject();
			if (solution != null)
			{
				if (createSolutionFile(solution))
				{
					NodeDataSolution snld = solution.PropertyObject as NodeDataSolution;
					dlg.LoadData(snld.Filename.Filename);
				}
				else
				{
					return false;
				}
			}
			else
			{
				dlg.LoadData(null);
			}
			if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
			{
				SolutionSelection result = dlg.Result;
				try
				{
					string solutionFile;
					string projectFile;
					if (result.CreateSolutionFolder)
					{
						string dirSol = Path.Combine(result.Location, result.SolutionName);
						if (!Directory.Exists(dirSol))
						{
							Directory.CreateDirectory(dirSol);
						}
						solutionFile = Path.Combine(dirSol, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}",
								result.SolutionName, NodeDataSolution.FILE_EXT_SOLUTION));
						string dirPrj = Path.Combine(dirSol, result.ProjectName);
						if (!Directory.Exists(dirPrj))
						{
							Directory.CreateDirectory(dirPrj);
						}
						projectFile = Path.Combine(dirPrj, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}",
								result.ProjectName, NodeDataSolution.FILE_EXT_PROJECT));
					}
					else
					{
						string dirPrj = Path.Combine(result.Location, result.ProjectName);
						if (!Directory.Exists(dirPrj))
						{
							Directory.CreateDirectory(dirPrj);
						}
						solutionFile = Path.Combine(dirPrj, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}",
								result.SolutionName, NodeDataSolution.FILE_EXT_SOLUTION));
						projectFile = Path.Combine(dirPrj, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}",
								result.ProjectName, NodeDataSolution.FILE_EXT_PROJECT));
					}
					//Generate project from template =============================================
					ProjectTemplate prjTemplate = new ProjectTemplate(result.ProjectTemplate);
					prjTemplate.LoadName();
					prjTemplate.GenerateProject(projectFile, result.ReplaceNames);
					//
					LimnorProject prj = new LimnorProject(projectFile);
					prj.SetAsMainProjectFile();
					prj.SetProjectAssemblyName(result.ProjectName);
					Guid g = LimnorProject.GetProjectGuid(projectFile);
					prj.SetProjectGuid(g);
					prj.Save();
					//
					SolutionNode solutionTreeNode;
					if (solution == null)
					{
						solutionTreeNode = new SolutionNode(solutionFile);
					}
					else
					{
						solutionTreeNode = solution;
					}
					ProjectNode prjTreeNode = new ProjectNode(prj);
					solutionTreeNode.Nodes.Add(prjTreeNode);
					NodeDataSolution sd = solutionTreeNode.PropertyObject as NodeDataSolution;
					sd.Save();
					//
					if (solution == null)
					{
						this.treeView1.Nodes.Clear();
						this.treeView1.Nodes.Add(solutionTreeNode);
					}
					solutionTreeNode.Expand();
					this.treeView1.SelectedNode = prjTreeNode;
					return true;
				}
				catch (Exception err)
				{
					MathNode.Log(this.FindForm(),err);
				}
			}

			return false;
		}
		public void SendNotice(enumVobNotice notice, object data)
		{
			if (_InterfaceVOB != null)
			{
				_InterfaceVOB.SendNotice(notice, data);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns>solution file</returns>
		public string OpenProject(string file)
		{
			string slnFile = null;
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					SolutionNode nodeSolution = this.GetCurrentSolution();
					if (nodeSolution != null)
					{
						ProjectNode prjNode = nodeSolution.GetProjectNodeByFile(file);
						if (prjNode != null)
						{
							this.treeView1.SelectedNode = prjNode;
							return ((NodeDataSolution)nodeSolution.PropertyObject).FilePath;
						}
						((NodeDataSolution)nodeSolution.PropertyObject).Save();
					}
					//find solution file by project file
					SolutionNode solNde = null;
					ProjectNode prjNd = null;
					string folder = System.IO.Path.GetDirectoryName(file);
					string[] slnFiles = System.IO.Directory.GetFiles(folder, "*." + NodeDataSolution.FILE_EXT_SOLUTION);

					if (slnFiles != null)
					{
						for (int i = 0; i < slnFiles.Length; i++)
						{
							SolutionNode sn = new SolutionNode(slnFiles[i]);
							for (int k = 0; k < sn.Nodes.Count; k++)
							{
								ProjectNode pn = sn.Nodes[k] as ProjectNode;
								if (pn != null)
								{
									ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
									if (string.Compare(file, pnd.ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
									{
										slnFile = slnFiles[i];
										solNde = sn;
										prjNd = pn;
										break;
									}
								}
							}
							if (solNde != null)
							{
								break;
							}
						}
					}
					if (solNde == null)
					{
						prjNd = new ProjectNode(new LimnorProject(file));
						solNde = new SolutionNode(null);
						solNde.Nodes.Add(prjNd);
					}
					this.treeView1.Nodes.Clear();
					this.treeView1.Nodes.Add(solNde);
					solNde.Expand();
					this.treeView1.SelectedNode = prjNd;
				}
			}
			return slnFile;
		}
		public void ShowException(Exception err)
		{
			VOB.OutputWindow O = _serviceProvider.GetService(typeof(VOB.OutputWindow)) as VOB.OutputWindow;
			if (O != null)
			{
				O.ShowException(err);
			}
		}

		public void Save()
		{
			foreach (TreeNode nd in treeView1.Nodes)
			{
				SolutionNode sn = nd as SolutionNode;
				if (sn != null)
				{
					NodeDataSolution sln = sn.PropertyObject as NodeDataSolution;
					if (sln.Dirty)
					{
						if (string.IsNullOrEmpty(sln.Filename.Filename))
						{
							SaveFileDialog dlg = new SaveFileDialog();
							dlg.Title = "Limnor solution file";
							dlg.Filter = string.Format(CultureInfo.InvariantCulture, "Limnor solution file|*.{0}", NodeDataSolution.FILE_EXT_SOLUTION);
							dlg.OverwritePrompt = true;
							if (dlg.ShowDialog(this) == DialogResult.OK)
							{
								sln.Filename.Filename = dlg.FileName;
								sln.Dirty = true;
							}
						}
						if (!string.IsNullOrEmpty(sln.Filename.Filename))
						{
							sln.Save();
						}
						foreach (TreeNode nd2 in sn.Nodes)
						{
							ProjectNode pn = nd2 as ProjectNode;
							if (pn != null)
							{
								if (((ProjectNodeData)(pn.PropertyObject)).Dirty)
								{
									((ProjectNodeData)(pn.PropertyObject)).Save();
								}
							}
						}
					}
					break;
				}
			}
		}
		public ProjectNode GetProjectNodeByGuid(Guid g)
		{
			SolutionNode sn = GetCurrentSolution();
			if (sn != null)
			{
				for (int i = 0; i < sn.Nodes.Count; i++)
				{
					ProjectNode pn = sn.Nodes[i] as ProjectNode;
					if (pn != null)
					{
						ProjectNodeData pnd = (ProjectNodeData)(pn.PropertyObject);
						if (pnd.Project.ProjectGuid == g)
						{
							return pn;
						}
					}
				}
			}
			return null;
		}


		public SolutionNode GetCurrentSolution()
		{
			if (treeView1.SelectedNode != null)
			{
				if (treeView1.SelectedNode is SolutionNode)
					return treeView1.SelectedNode as SolutionNode;
				TreeNode nd = treeView1.SelectedNode;
				while (nd.Parent != null)
				{
					if (nd.Parent is SolutionNode)
						return nd.Parent as SolutionNode;
					nd = nd.Parent;
				}
			}
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				if (treeView1.Nodes[i] is SolutionNode)
					return treeView1.Nodes[i] as SolutionNode;
			}
			return null;
		}
		public void AddEmptyProject()
		{
			SolutionNode nd = GetCurrentSolution();
			if (nd != null)
			{
			}
		}
		public void CloseSolution()
		{
			FormWarning.CloseMessageForm();
			Save();
			this.treeView1.Nodes.Clear();
			LimnorSolution.ResetSolution();
		}
		public SolutionNode LoadFromFile(string file)
		{
			try
			{
				SolutionNode nd = createSolutionNode(file);
				this.treeView1.Nodes.Clear();
				this.treeView1.Nodes.Add(nd);
				this.treeView1.SelectedNode = nd;
				return nd;
			}
			catch (Exception err)
			{
				ShowException(err);
			}
			return null;
		}
		public void ExpandAll()
		{
			treeView1.ExpandAll();
		}
		#endregion

		#region Properties
		public TreeNode SelectedNode
		{
			get
			{
				return treeView1.SelectedNode;
			}
			set
			{
				treeView1.SelectedNode = value;
			}
		}
		public SolutionNode SolutionNode
		{
			get
			{
				for (int i = 0; i < treeView1.Nodes.Count; i++)
				{
					if (treeView1.Nodes[i] is SolutionNode)
					{
						return (SolutionNode)treeView1.Nodes[i];
					}
				}
				return null;
			}
		}
		public NodeDataSolution SolutionNodeObject
		{
			get
			{
				if (this.SolutionNode != null)
				{
					return this.SolutionNode.PropertyObject as NodeDataSolution;
				}
				return null;
			}
		}

		public ProjectNode ActiveProjectNode
		{
			get
			{
				if (treeView1.SelectedNode != null)
				{
					if (treeView1.SelectedNode is ProjectNode)
						return treeView1.SelectedNode as ProjectNode;
				}
				SolutionNode sn = GetCurrentSolution();
				if (sn != null)
				{
					for (int i = 0; i < sn.Nodes.Count; i++)
					{
						if (sn.Nodes[i] is ProjectNode)
						{
							if (((ProjectNodeData)(((ProjectNode)(sn.Nodes[i])).PropertyObject)).IsActive)
							{
								return sn.Nodes[i] as ProjectNode;
							}
						}
					}
					if (sn.Nodes.Count > 0)
					{
						for (int i = 0; i < sn.Nodes.Count; i++)
						{
							if (sn.Nodes[i] is ProjectNode)
							{
								((ProjectNodeData)(((ProjectNode)(sn.Nodes[i])).PropertyObject)).IsActive = true;
								return sn.Nodes[i] as ProjectNode;
							}
						}
					}
				}
				return null;
			}
		}

		public InterfaceVOB VobService
		{
			get
			{
				return _InterfaceVOB;
			}
			set
			{
				_InterfaceVOB = value;
			}
		}
		public IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				_serviceProvider = value;
			}
		}
		#endregion

		#region IChangeControl Members

		private bool _dirty;
		private bool isDirty(TreeNodeCollection nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				IChangeControl icc = nodes[i] as IChangeControl;
				if (icc != null)
				{
					if (icc.Dirty)
						return true;
				}
				if (isDirty(nodes[i].Nodes))
					return true;
			}
			return false;
		}
		public static void resetModified(TreeNodeCollection nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				IChangeControl icc = nodes[i] as IChangeControl;
				if (icc != null)
				{
					icc.ResetModified();
				}
				resetModified(nodes[i].Nodes);
			}
		}
		[Browsable(false)]
		public bool Dirty
		{
			get
			{
				if (isDirty(treeView1.Nodes))
				{
					return true;
				}
				return _dirty;
			}
			set
			{
				_dirty = value;
			}
		}

		public void ResetModified()
		{
			_dirty = false;
			resetModified(treeView1.Nodes);
		}

		#endregion

	}
	public abstract class TreeNodeClass : TreeNode, IChangeControl
	{
		protected NodeData Obj; //initialize into different type
		public TreeNodeClass()
		{
		}
		protected virtual void SetNodeIcon()
		{
		}
		public void OnPropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			Dirty = true;
		}
		public void SendNotice(enumVobNotice notice, object data)
		{
			if (TreeView != null)
			{
				SolutionTree st = TreeView.Parent as SolutionTree;
				if (st != null)
				{
					st.SendNotice(notice, data);
				}
			}
		}
		/// <summary>
		/// provide property view
		/// </summary>
		public NodeData PropertyObject
		{
			get
			{
				return Obj;
			}
		}
		public virtual bool CanRemove
		{
			get
			{
				return true;
			}
		}
		public override string ToString()
		{
			if (Obj.Name == null)
				Obj.Name = "";
			return Obj.Name;
		}

		#region IChangeControl Members

		public bool Dirty
		{
			get
			{
				return Obj.Dirty;
			}
			set
			{
				Obj.Dirty = value;
			}
		}

		public void ResetModified()
		{
			Obj.ResetModified();
			SolutionTree.resetModified(this.Nodes);
		}

		#endregion
	}
	/// <summary>
	/// to provide property view
	/// </summary>
	public class NodeData : IChangeControl
	{
		private string _name = "";
		private bool _dirty;
		public event EventHandler OnNameChanged = null;
		public event EventHandler OnNameChanging = null;
		public NodeData()
		{
		}
		protected virtual void OnSetName(string name, string oldName)
		{

		}
		protected void SetName0(string name)
		{
			_name = name;
		}
		public void RefreshNameDisplay(string newName)
		{
			if (OnNameChanged != null)
			{
				OnNameChanged(this, new EventArgNameChange(newName, _name));
			}
		}
		[Description("object name")]
		[ParenthesizePropertyName(true)]
		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				string old = _name;
				if (OnNameChanging != null)
				{
					EventArgNameChange e = new EventArgNameChange(value, old);
					OnNameChanging(this, e);
					if (e.Cancel)
						return;
				}
				_name = value;
				_dirty = true;
				OnSetName(_name, old);
				if (OnNameChanged != null)
				{
					OnNameChanged(this, new EventArgNameChange(value, old));
				}
			}
		}
		#region IChangeControl Members

		[Browsable(false)]
		public virtual bool Dirty
		{
			get
			{
				return _dirty;
			}
			set
			{
				_dirty = value;
			}
		}

		public virtual void ResetModified()
		{
			_dirty = false;
		}

		#endregion
	}
}