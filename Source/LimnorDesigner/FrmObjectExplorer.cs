/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlSerializer;
using VSPrj;
using ProgElements;
using XmlUtility;
using System.Reflection;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Interface;
using System.Collections;
using VPL;
using LimnorDesigner.ResourcesManager;
using System.Globalization;
using LimnorDesigner.Action;
using System.Collections.Specialized;
using MathExp;
using LimnorDesigner.Web;
using System.IO;

namespace LimnorDesigner
{
	/// <summary>
	/// for object selection
	/// </summary>
	public partial class FrmObjectExplorer : Form, IXDesignerViewer
	{
		#region fields and constructors
		private ILimnorDesignPane pane;
		private TreeViewObjectExplorer _objExplorer;
		private TreeNodeMethodReturn _abortActionNode;
		private EnumObjectSelectType _selectType;
		private LimnorProject _project;
		//selection result
		public IObjectIdentity SelectedObject;
		public List<IProperty> SelectedProperties;
		public IAction SelectedAction;
		public IMethod SelectedMethod;
		public DataTypePointer SelectedDataType;
		public DataTypePointer TypeSelectionScope;
		static List<Type> _usedtypes;
		public FrmObjectExplorer()
		{
			InitializeComponent();
			ObjectSubType = EnumObjectSelectSubType.All;
			btOK.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btOK.ImageIndex = TreeViewObjectExplorer.IMG_OK;
			btOK.Enabled = false;
			btCancel.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btCancel.ImageIndex = TreeViewObjectExplorer.IMG_CANCEL;
			btBack.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btBack.ImageIndex = TreeViewObjectExplorer.IMG_ARROWLT;
			//
			txtName.SetDelimiterSearch('.', new EventHandler(searchDot));
		}
		#endregion
		#region Dialog Caches
		private static Dictionary<Guid, Dictionary<UInt32, List<FrmObjectExplorer>>> _rootObjectDialogs;
		public static FrmObjectExplorer LoadRootObject(ILimnorDesignerLoader loader, MethodClass scopeMethod, EnumObjectSelectType selectType, DataTypePointer scope, string eventName)
		{
			FormProgress.ShowProgress("Loading class data, please wait ...");
			try
			{
				FrmObjectExplorer dlg = null;
				if (eventName == null) eventName = string.Empty;
				Guid prjGuid = loader.Project.ProjectGuid;
				UInt32 classId = loader.ClassID;
				bool isStaticScope = (scopeMethod != null && scopeMethod.IsStatic);
				if (_rootObjectDialogs == null)
				{
					_rootObjectDialogs = new Dictionary<Guid, Dictionary<uint, List<FrmObjectExplorer>>>();
				}
				Dictionary<UInt32, List<FrmObjectExplorer>> dlgs;
				if (!_rootObjectDialogs.TryGetValue(prjGuid, out dlgs))
				{
					dlgs = new Dictionary<uint, List<FrmObjectExplorer>>();
					_rootObjectDialogs.Add(prjGuid, dlgs);
				}
				List<FrmObjectExplorer> lst;
				if (dlgs.TryGetValue(classId, out lst))
				{
					List<FrmObjectExplorer> deletes = new List<FrmObjectExplorer>();
					foreach (FrmObjectExplorer f in lst)
					{
						if (f.Disposing || f.IsDisposed)
						{
							deletes.Add(f);
						}
					}
					foreach (FrmObjectExplorer f in deletes)
					{
						lst.Remove(f);
					}
				}
				else
				{
					lst = new List<FrmObjectExplorer>();
					dlgs.Add(classId, lst);
				}
				foreach (FrmObjectExplorer f in lst)
				{
					if (f.IsTargetDialog(selectType, eventName, isStaticScope))
					{
						if (!f.Visible)
						{
							dlg = f;
							break;
						}
					}
				}
				if (dlg == null)
				{
					DesignUtil.LogIdeProfile("Create new object dialogue");
					dlg = new FrmObjectExplorer();
					lst.Add(dlg);
				}
				else
				{
					DesignUtil.LogIdeProfile("Use cached object dialogue");
				}
				dlg.LoadRootObjectX(loader, scopeMethod, selectType, scope, eventName);
				return dlg;
			}
			finally
			{
				FormProgress.HideProgress();
			}
		}
		public static void RemoveDialogueCaches(Guid g, object data)
		{
			RemoveDialogCaches(g, 0);
		}
		public static void RemoveDialogCaches(Guid prjGuid, UInt32 classId)
		{
			if (_rootObjectDialogs != null)
			{
				if (classId == 0)
				{
					if (prjGuid == Guid.Empty)
					{
						_rootObjectDialogs.Clear();
					}
					else if (_rootObjectDialogs.ContainsKey(prjGuid))
					{
						_rootObjectDialogs.Remove(prjGuid);
					}
				}
				else
				{
					Dictionary<UInt32, List<FrmObjectExplorer>> dlgs;
					if (_rootObjectDialogs.TryGetValue(prjGuid, out dlgs))
					{
						if (dlgs.ContainsKey(classId))
						{
							dlgs.Remove(classId);
						}
						foreach (List<FrmObjectExplorer> dlgList in dlgs.Values)
						{
							if (dlgList != null)
							{
								foreach (FrmObjectExplorer dlg in dlgList)
								{
									dlg.ResetDefaultInstance(classId);
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region overrides
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}
		#endregion
		#region private methods

		private void createTreeView()
		{
			if (_objExplorer == null)
			{
				_objExplorer = new TreeViewObjectExplorer();
				if (_project != null)
				{
					_objExplorer.SetProject(_project);
				}
				splitContainer2.Panel1.Controls.Add(_objExplorer);
				_objExplorer.Dock = DockStyle.Fill;
				_objExplorer.Visible = true;
				_objExplorer.AfterSelect += new TreeViewEventHandler(_objExplorer_AfterSelect);
			}
			else
			{
				if (_project != null)
				{
					_objExplorer.SetProject(_project);
				}
			}
		}
		#endregion
		#region public methods
		public void LoadClass(string classFile, ClassPointer objId, UInt32 scopeMethodId, int nodeIndex)
		{
			this.Cursor = Cursors.WaitCursor;
			XmlDocument doc = new XmlDocument();
			doc.Load(classFile);
			UInt32 classId = XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID);
			if (classId == 0)
			{
				classId = LimnorProject.GetUnsavedClassId(classFile);
			}
			if (classId == 0)
			{
				throw new DesignerException("class id not found for file {0}", classFile);
			}
			if (classId != objId.ClassId)
			{
				ClassPointer cid = ClassPointer.CreateClassPointer(classId, _project);
				TreeNodeClassRoot tnc = _objExplorer.CreateClassRoot(false, cid, true);
				_objExplorer.Nodes.RemoveAt(nodeIndex);
				_objExplorer.Nodes.Insert(nodeIndex, tnc);
				object[] attrs = cid.BaseClassType.GetCustomAttributes(typeof(UseDefaultInstanceAttribute), true);
				if (attrs != null && attrs.Length > 0)
				{
					for (int k = 0; k < attrs.Length; k++)
					{
						UseDefaultInstanceAttribute defatt = attrs[k] as UseDefaultInstanceAttribute;
						if (defatt != null)
						{
							ClassInstancePointer cip = new ClassInstancePointer(objId, cid, 0);
							cip.Name = string.Format(CultureInfo.InvariantCulture, "Def{0}", cid.Name);
							MemberComponentIdDefaultInstance mcc = new MemberComponentIdDefaultInstance(cip);
							TreeNodeDefaultInstance tndef = new TreeNodeDefaultInstance(_objExplorer, mcc, scopeMethodId);
							tnc.Nodes.Add(tndef);
							break;
						}
					}
				}
				tnc.Expand();
				_objExplorer.SelectedNode = tnc;
			}
			else
			{
				_objExplorer.Nodes.RemoveAt(nodeIndex);
				for (int i = 0; i < _objExplorer.Nodes.Count; i++)
				{
					TreeNodeClassRoot tcr = _objExplorer.Nodes[i] as TreeNodeClassRoot;
					if (tcr != null)
					{
						if (tcr.ClassId == objId.ClassId)
						{
							tcr.Expand();
							_objExplorer.SelectedNode = tcr;
							break;
						}
					}
				}
			}
			this.Cursor = Cursors.Default;
		}
		public void ResetDefaultInstance(UInt32 classId)
		{
			_objExplorer.ResetDefaultInstance(classId);
		}
		public bool IsTargetDialog(EnumObjectSelectType selectType, string eventName, bool isStaticScope)
		{
			if (string.CompareOrdinal(_objExplorer.SelectionEventScope, eventName) == 0 && _objExplorer.SelectionType == selectType && _objExplorer.StaticScope == isStaticScope)
			{
				return true;
			}
			return false;
		}
		public void SetSelectLValue()
		{
			_objExplorer.SetSelectLValue();
		}
		public void ResetSelectLValue()
		{
			_objExplorer.ResetSelectLValue();
		}
		public void SetRootPointer(ClassPointer root)
		{
			_objExplorer.SetRoortID(root);
		}
		public void reloadObjects(MethodClass m, ClassPointer objId)
		{
			TreeNodeMethodReturn tnAbort = null;
			bool methodLoaded = false;
			List<TreeNode> deletes = new List<TreeNode>();
			for (int i = 0; i < _objExplorer.Nodes.Count; i++)
			{
				TreeNodeAddEventHandler tn = _objExplorer.Nodes[i] as TreeNodeAddEventHandler;
				if (tn != null)
				{
					deletes.Add(tn);
				}

				TreeNodeCustomMethod tno = _objExplorer.Nodes[i] as TreeNodeCustomMethod;
				if (tno != null)
				{
					if (m == null)
					{
						deletes.Add(tno);
					}
					else
					{
						if (tno.Method.IsSameMethod(m))
						{
							//changed:always reload method so that variables can be added/deleted
							deletes.Add(tno);
						}
						else
						{
							deletes.Add(tno);
						}
					}
				}
				if (tnAbort == null)
				{
					TreeNodeMethodReturn tnmr = _objExplorer.Nodes[i] as TreeNodeMethodReturn;
					if (tnmr != null && string.CompareOrdinal(tnmr.Text, "Abort") == 0)
					{
						tnAbort = tnmr;
						deletes.Add(tnmr);
					}
				}
			}
			foreach (TreeNode tn in deletes)
			{
				_objExplorer.Nodes.Remove(tn);
			}
			if (m != null && !methodLoaded)
			{
				addMethodNode(m);
			}
			if ((_objExplorer.SelectionType == EnumObjectSelectType.Method || _objExplorer.SelectionType == EnumObjectSelectType.Action) && (m == null))
			{
				addAbortNode(objId);
			}
			loadRecentSelections(objId.ObjectList);
		}
		public void AddEventHandlerNode(bool isWebPage, IEvent e)
		{
			if (isWebPage)
			{
				_objExplorer.Nodes.Add(new TreeNodeAddWebClientEventHandlerClientActs(e));
				_objExplorer.Nodes.Add(new TreeNodeAddWebClientEventHandlerServerActs(e));
				_objExplorer.Nodes.Add(new TreeNodeAddWebClientEventHandlerDownloadActs(e));
			}
			else
			{
				_objExplorer.Nodes.Add(new TreeNodeAddEventHandler(e));
			}
		}
		public void AddNullPointerNode(ClassPointer root)
		{
			for (int i = 0; i < _objExplorer.Nodes.Count; i++)
			{
				if (_objExplorer.Nodes[i] is TreeNodeNullObject)
				{
					return;
				}
			}
			_objExplorer.Nodes.Add(new TreeNodeNullObject(root));
		}
		public ArrayList GetSelectedNodes()
		{
			return _objExplorer.SelectedNodes;
		}
		public void SetMultipleSelection(bool multipleSelection)
		{
			_objExplorer.MultipleSelection = multipleSelection;
		}
		public void SetCheckTaget(fnCheckNode checker)
		{
			_objExplorer.CheckTarget = checker;
		}
		public void LoadAttributeParameterTypes(EnumWebRunAt runAt)
		{
			createTreeView();
			_objExplorer.LoadAttributeParameterTypes(runAt, null);
		}
		void addAbortNode(ClassPointer objId)
		{
			ActionClass ar = new ActionClass(objId);
			ar.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			ar.ActionName = "Abort";
			ar.ActionHolder = objId;
			MethodReturnMethod mr = new MethodReturnMethod(ar);
			mr.SetProject(_project);
			mr.SetRoot(objId);
			mr.IsAbort = true;
			ar.ActionMethod = mr;
			TreeNodeMethodReturn tnAbort = new TreeNodeMethodReturn(ar);
			_objExplorer.Nodes.Add(tnAbort);
			_abortActionNode = tnAbort;
		}
		private void loadRecentSelections(ObjectIDmap map)
		{
			lstRecent.Items.Clear();
			if (map != null)
			{
				List<IObjectIdentity> recentSelections = map.GetRecentSelectionList();
				if (recentSelections != null)
				{
					foreach (IObjectIdentity id in recentSelections)
					{
						lstRecent.Items.Add(id);
					}
				}
			}
		}
		/// <summary>
		/// for object selection
		/// </summary>
		/// <param name="loader">define the scope</param>
		/// <param name="selectType"></param>
		/// <param name="scope"></param>
		/// <param name="eventName"></param>
		public void LoadRootObjectX(ILimnorDesignerLoader loader, MethodClass scopeMethod,
			EnumObjectSelectType selectType, DataTypePointer scope, string eventName)
		{
			UInt32 scopeMethodId = 0;
			ClassPointer objId = loader.GetRootId();//this is the scope
			ObjectIDmap map = loader.ObjectMap;
			//
			_project = loader.Project;
			_selectType = selectType;
			DesignUtil.LogIdeProfile("Create tree view");
			createTreeView();
			_objExplorer.SelectionTypeScope = scope;
			_objExplorer.SelectionType = selectType;
			_objExplorer.SelectionEventScope = eventName;
			bool bLoaded = _objExplorer.Nodes.Count > 0;
			if (scopeMethod != null)
			{
				scopeMethodId = scopeMethod.MemberId;
				_objExplorer.StaticScope = scopeMethod.IsStatic;
			}
			else
			{
				_objExplorer.StaticScope = false;
			}
			//load the scope-defining class
			TreeNodeClassRoot r;
			if (bLoaded)
			{
				DesignUtil.LogIdeProfile("reload objects");
				reloadObjects(scopeMethod, objId);
			}
			else
			{
				DesignUtil.LogIdeProfile("load objects");
				_objExplorer.SetProject(loader.Project);
				r = _objExplorer.CreateClassRoot(true, objId, _objExplorer.StaticScope);
				DesignUtil.LogIdeProfile("add main tree nodes");
				_objExplorer.Nodes.Add(r);
				_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, r, false, 0));
				if (_project.ProjectType == EnumProjectType.WebAppAspx || _project.ProjectType == EnumProjectType.WebAppPhp)
				{
					_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, r, EnumWebRunAt.Client));
					if (_project.ProjectType == EnumProjectType.WebAppPhp)
					{
						_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, r, EnumWebRunAt.Server));
					}
				}
				DesignUtil.LogIdeProfile("load references");
				List<Type> loadedTypes = r.LoadExternalTypes();
				if (scopeMethod != null)
				{
					LoadMethodParameters(scopeMethod);
				}
				//for aborting untyped event handler
				if ((selectType == EnumObjectSelectType.Method || selectType == EnumObjectSelectType.Action) && (scopeMethod == null))
				{
					addAbortNode(objId);
				}
				//
				DesignUtil.LogIdeProfile("load classes");
				//load out-of-scope classes
				if (selectType != EnumObjectSelectType.All)
				{
					string[] ss = _project.GetComponentFiles();
					for (int i = 0; i < ss.Length; i++)
					{
						FormProgress.ShowProgress(string.Format(CultureInfo.InvariantCulture, "Loading {0}", ss[i]));
						TreeNodeClassLoader tcl = new TreeNodeClassLoader(ss[i], objId, scopeMethodId);
						_objExplorer.Nodes.Add(tcl);
					}
				}
				pane = loader.DesignPane;
				if (pane != null)
				{
					pane.AddDesigner(this, true);
				}
				DesignUtil.LogIdeProfile("load recent selection");
				loadRecentSelections(map);
				//
				if (selectType == EnumObjectSelectType.Action)
				{
					//load all existing actions
					TreeNodeAllActionCollection nodeAllActs = new TreeNodeAllActionCollection(_objExplorer, null, objId, scopeMethodId);
					_objExplorer.Nodes.Add(nodeAllActs);
				}
				//load resources
				ProjectResources rm = _project.GetProjectSingleData<ProjectResources>();
				TreeNodeResourceManager resourcesNode = new TreeNodeResourceManager(rm, true);
				_objExplorer.Nodes.Add(resourcesNode);
				//
				DesignUtil.LogIdeProfile("add project reference node");
				_objExplorer.Nodes.Add(new TreeNodeNamespaceList(new NamespaceList()));
				_objExplorer.LoadExternalTypes(_project, loadedTypes);
				//add GAC
				_objExplorer.Nodes.Add(new TreeNodeGac(false));
				DesignUtil.LogIdeProfile("get toolbox items");
				//add used types
				StringCollection errors = new StringCollection();
				List<Type> types = _project.GetToolboxItems(errors);
				if (errors.Count > 0)
				{
					MathNode.Log(errors);
				}
				DesignUtil.LogIdeProfile("load assemblies");
				List<Assembly> loadedAssemblies = new List<Assembly>();
				loadedAssemblies.Add(typeof(object).Assembly);
				foreach (Type t in types)
				{
					if (!loadedAssemblies.Contains(t.Assembly))
					{
						loadedAssemblies.Add(t.Assembly);
						_objExplorer.Nodes.Add(new TreeNodeAssembly(t.Assembly));
					}
				}
				if (_usedtypes != null)
				{
					DesignUtil.LogIdeProfile("load used types");
					foreach (Type t in _usedtypes)
					{
						if (!loadedAssemblies.Contains(t.Assembly))
						{
							loadedAssemblies.Add(t.Assembly);
							_objExplorer.Nodes.Add(new TreeNodeAssembly(t.Assembly));
						}
					}
				}
			}
			DesignUtil.LogIdeProfile("Finish load root object");
			_objExplorer.SelectedNode = null;
		}
		/// <summary>
		/// load method parameters and local variables
		/// </summary>
		/// <param name="method"></param>
		public void LoadMethodParameters(IMethod method)
		{
			bool methodLoaded = false;
			bool actLoaded = false;
			MethodClass mc = method as MethodClass;
			if (mc != null)
			{
				_objExplorer.ScopeMethod = method;
				//_objExplorer.ActionsHolder = mc.
				for (int i = 0; i < _objExplorer.Nodes.Count; i++)
				{
					TreeNodeCustomMethod tno = _objExplorer.Nodes[i] as TreeNodeCustomMethod;
					if (tno != null)
					{
						if (tno.Method.IsSameMethod(mc))
						{
							methodLoaded = true;
						}
					}
					else
					{
						TreeNodeActionInput tai = _objExplorer.Nodes[i] as TreeNodeActionInput;
						if (tai != null)
						{
							actLoaded = true;
						}
					}
					if (actLoaded && methodLoaded)
					{
						break;
					}
				}
				if (!methodLoaded)
				{
					addMethodNode(mc);

				}
			}
			if (!actLoaded)
			{
				_objExplorer.Nodes.Add(new TreeNodeActionInput());
			}
		}
		public void addMethodNode(MethodClass mc)
		{
			TreeNodeClassRoot r = _objExplorer.RootClassNode;
			if (r != null)
			{
				ClassPointer cp = _objExplorer.RootId;
				TreeNodeCustomMethod nm = new TreeNodeCustomMethod(_objExplorer, mc.IsStatic, mc, cp, r.ScopeMethodId);
				if (mc is GetterClass || mc is SetterClass)
				{
					nm.Text = mc.MethodName;
				}
				else
				{
					if (cp.MethodInEditing != null && cp.MethodInEditing.CurrentEditor != null && cp.MethodInEditing.CurrentEditor.Site != null)
					{
						if (!string.IsNullOrEmpty(cp.MethodInEditing.CurrentEditor.Site.Name))
						{
							nm.Text = cp.MethodInEditing.CurrentEditor.Site.Name;
						}
					}
				}
				nm.ShowVariables = true;
				nm.ForScopeMethod = true;
				_objExplorer.Nodes.Insert(r.Index + 1, nm);
			}
			else
			{
				ClassPointer cp = mc.RootPointer;
				TreeNodeCustomMethod nm = new TreeNodeCustomMethod(_objExplorer, mc.IsStatic, mc, cp, mc.MemberId);
				if (mc is GetterClass || mc is SetterClass)
				{
					nm.Text = mc.MethodName;
				}
				else
				{
					if (cp.MethodInEditing != null && cp.MethodInEditing.CurrentEditor != null && cp.MethodInEditing.CurrentEditor.Site != null)
					{
						if (!string.IsNullOrEmpty(cp.MethodInEditing.CurrentEditor.Site.Name))
						{
							nm.Text = cp.MethodInEditing.CurrentEditor.Site.Name;
						}
					}
				}
				nm.ShowVariables = true;
				nm.ForScopeMethod = true;
				_objExplorer.Nodes.Add(nm);
			}
		}
		public void SetScopeMethod(IMethod method, IActionsHolder actsHolder)
		{
			_objExplorer.ScopeMethod = method;
			_objExplorer.ActionsHolder = actsHolder;
		}
		public void SetReadOnly(bool readOnly)
		{
			_objExplorer.ReadOnly = readOnly;
		}
		/// <summary>
		/// select lib type or root component for non-hosted usages such as 
		/// method parameters, return values, customer properties, MathNodeVar and sub-classes
		/// </summary>
		/// <param name="project"></param>
		/// 
		//===================


		/// <summary>
		/// select lib type or root component for non-hosted usages such as 
		/// method parameters, return values, customer properties, MathNodeVar and sub-classes
		/// </summary>
		/// <param name="project"></param>
		/// <param name="forBaseClass">for selecting a type as a base class</param>
		/// <param name="forMethodReturn">for selecting a type as a method return type</param>
		public void LoadProject(LimnorProject project, MethodClass scopeMethod, EnumObjectSelectType selectType, bool forMethodReturn, DataTypePointer scope, Type typeAttribute)
		{
			FormProgress.ShowProgress("Loading project data, please wait ...");
			try
			{
				TypeSelectionScope = scope;
				_project = project;
				createTreeView();
				_objExplorer.SelectionTypeAttribute = typeAttribute;
				_objExplorer.ForMethodReturn = forMethodReturn;
				_objExplorer.SetProject(project);
				_objExplorer.SelectionType = selectType;
				_objExplorer.SelectionTypeScope = scope;
				_objExplorer.SelectionEventScope = null;
				_objExplorer.Nodes.Clear();
				if (scopeMethod != null)
				{
					_objExplorer.StaticScope = scopeMethod.IsStatic;
				}
				//
				List<ClassPointer> classes = DesignUtil.GetAllComponentClassRef(project);
				foreach (ClassPointer rd in classes)
				{
					bool b = false;
					if (!rd.IsStatic)
					{
						if (scope == null || (scope != null && scope.IsAssignableFrom(new DataTypePointer(rd))))
						{
							if (selectType == EnumObjectSelectType.Interface)
							{
								b = rd.IsInterface;
							}
							else
							{
								b = true;
							}
						}
					}
					if (b)
					{
						TreeNodeClassRoot r = _objExplorer.CreateClassRoot(false, rd, _objExplorer.StaticScope);
						_objExplorer.Nodes.Add(r);
					}
				}
				if (selectType != EnumObjectSelectType.Interface)
				{
					if (_project.ProjectType == EnumProjectType.WebAppPhp)
					{
						_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, null, EnumWebRunAt.Client));
						_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, null, EnumWebRunAt.Server));
					}
					else
					{
						_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, null, false, 0));
						if (_project.ProjectType == EnumProjectType.WebAppAspx)
						{
							_objExplorer.Nodes.Add(new TreeNodePrimaryCollection(_objExplorer, null, EnumWebRunAt.Client));
						}
					}
				}
				_objExplorer.Nodes.Add(new TreeNodeNamespaceList(new NamespaceList()));
				if (scopeMethod != null)
				{
					LoadMethodParameters(scopeMethod);
				}
				_objExplorer.LoadExternalTypes(project, null);
				//add GAC
				_objExplorer.Nodes.Add(new TreeNodeGac(false));
				//add used types
				StringCollection errors = new StringCollection();
				List<Type> types = _project.GetToolboxItems(errors);
				if (errors.Count > 0)
				{
					MathNode.Log(errors);
				}
				List<Assembly> loadedAssemblies = new List<Assembly>();
				loadedAssemblies.Add(typeof(object).Assembly);
				foreach (Type t in types)
				{
					if (!loadedAssemblies.Contains(t.Assembly))
					{
						loadedAssemblies.Add(t.Assembly);
						_objExplorer.Nodes.Add(new TreeNodeAssembly(t.Assembly));
					}
				}
				if (_usedtypes != null)
				{
					foreach (Type t in _usedtypes)
					{
						if (!loadedAssemblies.Contains(t.Assembly))
						{
							loadedAssemblies.Add(t.Assembly);
							_objExplorer.Nodes.Add(new TreeNodeAssembly(t.Assembly));
						}
					}
				}
				_objExplorer.LoadTypeNodes();
			}
			finally
			{
				FormProgress.HideProgress();
			}
		}
		public void SetSelection(IObjectPointer pointer)
		{
			if (_objExplorer != null)
			{
				_objExplorer.SetSelection(_objExplorer.Nodes, pointer);
			}
		}
		public void SetSelection(IMethod method)
		{
			if (_objExplorer != null)
			{
				IObjectPointer p = method as IObjectPointer;
				if (p != null)
				{
					_objExplorer.SetSelection(_objExplorer.Nodes, p);
				}
			}
		}
		public void SetSelection(IAction action)
		{
			if (_objExplorer != null)
			{
				TreeNodeAction node = null;
				if (action != null)
				{
					node = _objExplorer.FindActionNode(action);
				}
				if (node != null)
				{
					_objExplorer.SelectedNode = node;
				}
				else
				{
					_objExplorer.SelectActionInList(action);
				}
			}
		}
		public void SelectActionCollectionNode(UInt32 classId, bool isStatic)
		{
			if (_objExplorer != null)
			{
				for (int i = 0; i < _objExplorer.Nodes.Count; i++)
				{
					TreeNodeClassRoot r = _objExplorer.Nodes[i] as TreeNodeClassRoot;
					if (r != null)
					{
						if (r.ClassId == classId)
						{
							r.LoadNextLevel();
							TreeNodePME pmeNode = r.GetMemberNode(isStatic);
							if (pmeNode != null)
							{
								pmeNode.LoadNextLevel();
								TreeNodeActionCollection ac = pmeNode.GetActionsNode();
								if (ac != null)
								{
									ac.LoadNextLevel();
									ac.EnsureVisible();
									if (ac.Nodes.Count > 0)
									{
										_objExplorer.SelectedNode = ac.Nodes[0];
									}
								}
							}
							break;
						}
					}
				}
			}
		}
		#endregion
		#region IXDesignerViewer Members
		public void OnClosing()
		{
		}
		public void OnEventListChanged(ICustomEventMethodDescriptor owner, UInt32 objectId)
		{
			_objExplorer.OnEventListChanged(owner, objectId);
		}
		public void OnResetMap()
		{
			_objExplorer.OnResetMap();
		}
		public void OnResetDesigner(object obj)
		{
			_objExplorer.OnResetDesigner(obj);
		}
		public void OnDatabaseListChanged()
		{
			_objExplorer.OnDatabaseListChanged();
		}
		public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
		{
			_objExplorer.OnDatabaseConnectionNameChanged(connectionid, newName);
		}
		public void OnObjectNameChanged(INonHostedObject obj)
		{
			_objExplorer.OnObjectNameChanged(obj);
		}
		public void SetDesigner(MultiPanes mp)
		{
		}
		public void OnIconChanged(UInt32 classId)
		{
			if (_objExplorer != null)
			{
				_objExplorer.OnIconChanged(classId);
			}
		}
		public void SetClassRefIcon(UInt32 classId, System.Drawing.Image img)
		{
			if (_objExplorer != null)
			{
				_objExplorer.SetClassRefIcon(classId, img);
			}
		}
		public Control GetDesignSurfaceUI()
		{
			return _objExplorer;
		}

		public void OnDataLoaded()
		{

		}
		public void OnUIConfigLoaded()
		{
		}
		public EnumObjectSelectSubType ObjectSubType { get; set; }
		public EnumMaxButtonLocation MaxButtonLocation
		{
			get { return EnumMaxButtonLocation.Left; }
		}
		public void OnClassLoadedIntoDesigner(UInt32 classId)
		{
			_objExplorer.OnClassLoadedIntoDesigner(classId);
		}
		public void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade)
		{
			_objExplorer.OnDefinitionChanged(classId, relatedObject, changeMade);
		}
		public void OnHtmlElementSelected(HtmlElement_Base element)
		{
			_objExplorer.OnHtmlElementSelected(element);
		}
		public void OnSelectedHtmlElement(Guid guid, object selector)
		{
			_objExplorer.OnSelectedHtmlElement(guid, selector);
		}
		//an html element is used. guid and id should have been created
		public void OnUseHtmlElement(HtmlElement_BodyBase element)
		{
			_objExplorer.OnUseHtmlElement(element);
		}
		public void OnComponentAdded(object obj)
		{
			_objExplorer.OnComponentAdded(obj);
		}
		public void OnComponentSelected(object obj)
		{
			_objExplorer.OnComponentSelected(obj);
		}
		public void OnComponentRename(object obj, string newName)
		{
			ObjectIDmap map = _objExplorer.ObjectIDList.GetChildMapByObject(obj);
			if (map == null)
			{
				map = _objExplorer.ObjectIDList;
			}
			IObjectPointer o = DesignUtil.CreateObjectPointer(map, obj);
			TreeNode node = _objExplorer.LocateNode(o);
			if (node != null)
			{
				node.Text = newName;
			}
		}

		public void OnComponentRemoved(object obj)
		{
			ObjectIDmap map = _objExplorer.ObjectIDList.GetChildMapByObject(obj);
			if (map == null)
			{
				map = _objExplorer.ObjectIDList;
			}
			if (map != null)
			{
				IObjectPointer o = DesignUtil.CreateObjectPointer(map, obj);
				TreeNode node = _objExplorer.LocateNode(o);
				if (node != null)
				{
					node.Remove();
				}
			}
		}
		public void OnActionSelected(IAction act)
		{
		}
		public void OnAssignAction(EventAction ea)
		{
			_objExplorer.OnAssignAction(ea);
		}

		public void OnRemoveEventHandler(EventAction ea, TaskID task)
		{
			_objExplorer.OnRemoveEventHandler(ea, task);
		}
		public void OnRemoveAllEventHandlers(IEvent e)
		{
			_objExplorer.OnRemoveAllEventHandlers(e);
		}
		public void OnActionListOrderChanged(object sender, EventAction ea)
		{
			_objExplorer.OnActionListOrderChanged(sender, ea);
		}

		public void OnActionChanged(UInt32 classId, IAction act, bool isNewAction)
		{
			_objExplorer.OnActionChanged(classId, act, isNewAction);
		}
		public void OnInterfaceAdded(InterfacePointer interfacePointer)
		{
			_objExplorer.OnInterfaceAdded(interfacePointer);
		}
		public void OnRemoveInterface(InterfacePointer interfacePointer)
		{
			_objExplorer.OnRemoveInterface(interfacePointer);
		}
		public void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface)
		{
			_objExplorer.OnBaseInterfaceAdded(owner, baseInterface);
		}
		public void OnInterfaceEventDeleted(InterfaceElementEvent eventType)
		{
			_objExplorer.OnInterfaceEventDeleted(eventType);
		}
		public void OnInterfaceEventAdded(InterfaceElementEvent eventType)
		{
			_objExplorer.OnInterfaceEventAdded(eventType);
		}
		public void OnInterfaceEventChanged(InterfaceElementEvent eventType)
		{
			_objExplorer.OnInterfaceEventChanged(eventType);
		}
		public void OnInterfacePropertyDeleted(InterfaceElementProperty property)
		{
			_objExplorer.OnInterfacePropertyDeleted(property);
		}
		public void OnInterfacePropertyChanged(InterfaceElementProperty property)
		{
			_objExplorer.OnInterfacePropertyChanged(property);
		}
		public void OnInterfacePropertyAdded(InterfaceElementProperty property)
		{
			_objExplorer.OnInterfacePropertyAdded(property);
		}
		public void OnInterfaceMethodDeleted(InterfaceElementMethod method)
		{
			_objExplorer.OnInterfaceMethodDeleted(method);
		}
		public void OnInterfaceMethodChanged(InterfaceElementMethod method)
		{
			_objExplorer.OnInterfaceMethodChanged(method);
		}
		public void OnInterfaceMethodCreated(InterfaceElementMethod method)
		{
			_objExplorer.OnInterfaceMethodCreated(method);
		}
		public void OnMethodChanged(MethodClass method, bool isNewAction)
		{
			_objExplorer.OnMethodChanged(/*creator,*/ method, isNewAction);
		}
		public void OnDeleteMethod(MethodClass method)
		{
			_objExplorer.OnDeleteMethod(method);
		}
		public void OnMethodSelected(MethodClass method)
		{
			_objExplorer.OnMethodSelected(method);
		}
		public void OnActionDeleted(IAction action)
		{
			_objExplorer.OnActionDeleted(action);
		}
		public void OnDeleteEventMethod(EventHandlerMethod method)
		{
			_objExplorer.OnDeleteEventMethod(method);
		}
		public void OnDeleteProperty(PropertyClass property)
		{
			_objExplorer.OnDeleteProperty(property);
		}
		public void OnAddProperty(PropertyClass property)
		{
			_objExplorer.OnAddProperty(property);
		}
		public void OnPropertyChanged(INonHostedObject property, string name)
		{
			_objExplorer.OnPropertyChanged(property, name);
		}
		public void OnPropertySelected(PropertyClass property)
		{
			_objExplorer.OnPropertySelected(property);
		}
		//public void OnActionNameChanged(string newActionName, UInt64 WholeActionId)
		//{
		//    //_objExplorer.OnActionNameChanged(newActionName, WholeActionId);
		//}
		public void OnDeleteEvent(EventClass eventObject)
		{
			_objExplorer.OnDeleteEvent(eventObject);
		}
		public void OnAddEvent(EventClass eventObject)
		{
			_objExplorer.OnAddEvent(eventObject);
		}
		public void OnEventSelected(EventClass eventObject)
		{
			_objExplorer.OnEventSelected(eventObject);
		}
		public void OnFireEventActionSelected(FireEventMethod method)
		{
			_objExplorer.OnFireEventActionSelected(method);
		}
		public void OnAddExternalType(UInt32 classId, Type t)
		{
		}
		public void OnRemoveExternalType(UInt32 classId, Type t)
		{
		}
		#endregion
		#region event handlers
		private void btOK_Click(object sender, EventArgs e)
		{
			if (this.ObjectSubType == EnumObjectSelectSubType.Property && this._objExplorer.MultipleSelection)
			{
				SelectedProperties = new List<IProperty>();
				ArrayList nodes = GetSelectedNodes();
				if (nodes != null && nodes.Count > 0)
				{
					for (int i = 0; i < nodes.Count; i++)
					{
						TreeNodeProperty ta = nodes[i] as TreeNodeProperty;
						if (ta != null)
						{
							if (ta.Property != null)
							{
								SelectedProperties.Add(ta.Property);
							}
						}
						else
						{
							TreeNodeCustomProperty cp = nodes[i] as TreeNodeCustomProperty;
							if (cp != null)
							{
								if (cp.Property != null)
								{
									SelectedProperties.Add(cp.Property.CreatePointer());
								}
							}
						}
					}
				}
				if (SelectedProperties.Count > 0)
				{
					this.DialogResult = DialogResult.OK;
				}
				else
				{
					MessageBox.Show(this, "There are not properties selected.", "Select properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				return;
			}
			TreeNodeAddEventHandler tnae = _objExplorer.SelectedNode as TreeNodeAddEventHandler;
			if (tnae != null)
			{
				IEvent evt = (IEvent)(tnae.OwnerPointer);
				ClassPointer root = pane.Loader.GetRootId();
				TreeNodeAddWebClientEventHandler tnwc = tnae as TreeNodeAddWebClientEventHandler;
				if (tnwc != null)
				{
					if (root.AddEventhandlerMethod(tnwc.ActionType, evt, 0, RectangleToScreen(tnae.Bounds), this) != null)
					{
						this.DialogResult = DialogResult.OK;
					}
				}
				else
				{
					if (root.AddEventhandlerMethod(typeof(EventHandlerMethod), evt, 0, RectangleToScreen(tnae.Bounds), this) != null)
					{
						this.DialogResult = DialogResult.OK;
					}
				}
			}
			else
			{
				TreeNodeMethod tnm = _objExplorer.SelectedNode as TreeNodeMethod;
				TreeNodeCustomMethod tncm = _objExplorer.SelectedNode as TreeNodeCustomMethod;
				TreeNodeProperty tnp = _objExplorer.SelectedNode as TreeNodeProperty;
				TreeNodeCustomProperty tncp = _objExplorer.SelectedNode as TreeNodeCustomProperty;
				TreeNodeCustomPropertyPointer tncpp = _objExplorer.SelectedNode as TreeNodeCustomPropertyPointer;
				TreeNodeCustomMethodPointer tncmp = _objExplorer.SelectedNode as TreeNodeCustomMethodPointer;
				TreeNodeLocalVariable tnloc = _objExplorer.SelectedNode as TreeNodeLocalVariable;
				TreeNodeMethodReturn tnmr = _objExplorer.SelectedNode as TreeNodeMethodReturn;
				TreeNodeCustomEventPointer tncep = _objExplorer.SelectedNode as TreeNodeCustomEventPointer;
				TreeNodeEvent tne = _objExplorer.SelectedNode as TreeNodeEvent;
				TreeNodeInterfaceMethod tim = _objExplorer.SelectedNode as TreeNodeInterfaceMethod;
				if (_objExplorer.SelectionType == EnumObjectSelectType.Action && (tne != null || tncep != null || tnmr != null || tnloc != null || tncpp != null || tnm != null || tncm != null || tnp != null || tncp != null || tncmp != null || tim != null))
				{
					if (tne != null)
					{
						SelectedAction = tne.CreateAction();
					}
					else if (tncep != null)
					{
						SelectedAction = tncep.CreateAction();
					}
					else if (tnmr != null)
					{
						SelectedAction = tnmr.ReturnAction;
					}
					else if (tnloc != null)
					{
						SelectedAction = tnloc.CreateAction();
					}
					else if (tnm != null)
					{
						SelectedAction = tnm.CreateAction();
					}
					else if (tncm != null)
					{
						SelectedAction = tncm.CreateAction();
					}
					else if (tnp != null)
					{
						SelectedAction = tnp.CreateAction();
					}
					else if (tncp != null)
					{
						SelectedAction = tncp.CreateAction();
					}
					else if (tncpp != null)
					{
						SelectedAction = tncpp.CreateAction();
					}
					else if (tncmp != null)
					{
						SelectedAction = tncmp.CreateAction();
					}
					else if (tim != null)
					{
						SelectedAction = tim.CreateAction();
					}
				}

				if (SelectedObject != null || SelectedAction != null || SelectedMethod != null)
				{
					if (pane != null && pane.Loader != null && pane.Loader.ObjectMap != null)
					{
						if (SelectedAction != null && !SelectedAction.IsLocal)
							pane.Loader.ObjectMap.AddRecentSelection(SelectedAction);
						else if (SelectedMethod != null)
							pane.Loader.ObjectMap.AddRecentSelection(SelectedMethod);
						else if (SelectedObject != null)
						{
							IObjectPointer op = SelectedObject as IObjectPointer;
							bool islocal = false;
							while (op != null)
							{
								CustomMethodParameterPointer cp = op as CustomMethodParameterPointer;
								if (cp != null)
								{
									islocal = true;
									break;
								}
								LocalVariable loc = op as LocalVariable;
								if (loc != null)
								{
									islocal = true;
									break;
								}
								op = op.Owner;
							}
							if (!islocal)
							{
								pane.Loader.ObjectMap.AddRecentSelection(SelectedObject);
							}
						}
					}
					if (_objExplorer.SelectionType == EnumObjectSelectType.Type || _objExplorer.SelectionType == EnumObjectSelectType.InstanceType)
					{
						if (SelectedDataType != null)
						{
							if (SelectedDataType.IsGenericType)
							{
								DlgSelectTypeParameters dlgP = new DlgSelectTypeParameters();
								dlgP.LoadData(SelectedDataType.BaseClassType, _project);
								if (dlgP.ShowDialog(this) == DialogResult.OK)
								{
									SelectedDataType.TypeParameters = dlgP._holder.Results;
								}
								else
								{
									return;
								}
							}
							if (!(SelectedDataType is WrapDataTypePointer) && SelectedDataType.IsLibType && !typeof(string).Equals(SelectedDataType.BaseClassType))
							{
								DialogSelectCollectiontype dlgTypes = new DialogSelectCollectiontype();
								//if it is a collection, create proper type
								Type t = SelectedDataType.LibTypePointer.ClassType;
								Type[] tpinfs = t.GetInterfaces();
								if (tpinfs != null && tpinfs.Length > 0)
								{
									for (int i = 0; i < tpinfs.Length; i++)
									{
										if (tpinfs[i].IsGenericType)
										{
											Type tg = tpinfs[i].GetGenericTypeDefinition();
											if (typeof(ICollection<>).Equals(tg))
											{
												Type[] tgs = tg.GetGenericArguments();
												if (tgs != null && tgs.Length > 0)
												{
													dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tgs[0]), SelectedDataType.LibTypePointer.ClassType));
												}
											}
											if (typeof(IEnumerable<>).Equals(tg))
											{
												Type[] tgs = tg.GetGenericArguments();
												if (tgs != null && tgs.Length > 0)
												{
													dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tgs[0]), SelectedDataType.LibTypePointer.ClassType));
												}
											}
											if (typeof(IEnumerator<>).Equals(tg))
											{
												Type[] tgs = tg.GetGenericArguments();
												if (tgs != null && tgs.Length > 0)
												{
													dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tgs[0]), SelectedDataType.LibTypePointer.ClassType));
												}
											}
										}
										if (typeof(ICollection).Equals(tpinfs[i]))
										{
											//specify item type
											Type tpItem = typeof(object);
											string st = t.AssemblyQualifiedName;
											int n = st.IndexOf(',');
											if (n > 0)
											{
												string sn = st.Substring(0, n);
												if (sn.EndsWith("Collection", StringComparison.Ordinal))
												{
													sn = sn.Substring(0, sn.Length - "Collection".Length) + st.Substring(n);
													tpItem = Type.GetType(sn);
												}
											}
											dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tpItem), SelectedDataType.LibTypePointer.ClassType));
										}
										if (typeof(IEnumerable).Equals(tpinfs[i]))
										{
											//specify item type
											Type tpItem = typeof(object);
											string st = t.AssemblyQualifiedName;
											int n = st.IndexOf(',');
											if (n > 0)
											{
												string sn = st.Substring(0, n);
												if (sn.EndsWith("Collection", StringComparison.Ordinal))
												{
													sn = sn.Substring(0, sn.Length - "Collection".Length) + st.Substring(n);
													tpItem = Type.GetType(sn);
												}
											}
											dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tpItem), SelectedDataType.LibTypePointer.ClassType));
										}
										if (typeof(IEnumerator).Equals(tpinfs[i]))
										{
											//specify item type
											Type tpItem = typeof(object);
											string st = t.AssemblyQualifiedName;
											int n = st.IndexOf(',');
											if (n > 0)
											{
												string sn = st.Substring(0, n);
												if (sn.EndsWith("Collection", StringComparison.Ordinal))
												{
													sn = sn.Substring(0, sn.Length - "Collection".Length) + st.Substring(n);
													tpItem = Type.GetType(sn);
												}
											}
											dlgTypes.AddType(new CollectionTypePointer(new TypePointer(tpItem), SelectedDataType.LibTypePointer.ClassType));
										}
									}
								}
								if (dlgTypes.Count > 0)
								{
									dlgTypes.AddType(SelectedDataType);
									if (dlgTypes.ShowDialog(this) == DialogResult.OK)
									{
										SelectedDataType = dlgTypes.Ret;
									}
								}
							}
						}
						if (SelectedDataType != null && SelectedDataType.IsLibType)
						{
							if (_usedtypes == null)
							{
								_usedtypes = new List<Type>();
							}
							if (!_usedtypes.Contains(SelectedDataType.BaseClassType))
							{
								_usedtypes.Add(SelectedDataType.BaseClassType);
							}
						}
					}
					else if (_objExplorer.SelectionType == EnumObjectSelectType.Action)
					{
						if (_objExplorer.ScopeMethod != null)
						{
							if (MethodEditContext.IsWebPage)
							{
								if (_objExplorer.MultipleSelection)
								{
									ArrayList nodes = GetSelectedNodes();
									if (nodes != null && nodes.Count > 0)
									{
										for (int i = 0; i < nodes.Count; i++)
										{
											TreeNodeAction ta = nodes[i] as TreeNodeAction;
											if (ta != null && ta.Action != null)
											{
												if (!MethodEditContext.CheckAction(ta.Action, this))
												{
													return;
												}
											}
										}
									}
								}
								else
								{
									if (SelectedAction != null)
									{
										if (!MethodEditContext.CheckAction(SelectedAction, this))
										{
											return;
										}
									}
								}
							}
						}
					}
					else if (_objExplorer.SelectionType == EnumObjectSelectType.Method && tnm != null)
					{
						if (tnm.MethodRef != null)
						{
							if (tnm.MethodRef.ContainsGenericParameters)
							{
								Type[] ts = tnm.MethodRef.GetGenericArguments();
								if (ts != null && ts.Length > 0)
								{
									DlgSelectTypeParameters dlgP = new DlgSelectTypeParameters();
									dlgP.LoadData(tnm.MethodRef.MethodDef, _project);
									if (dlgP.ShowDialog(this) == DialogResult.OK)
									{
										tnm.MethodRef.TypeParameters = dlgP._holder.Results;
									}
								}
							}
						}
					}
					this.DialogResult = DialogResult.OK;
				}
			}
		}
		void _objExplorer_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeObject node = e.Node as TreeNodeObject;
			if (node != null)
			{
				IObjectIdentity o = node.OwnerIdentity;
				bool b = node.CheckIsTargeted();
				if (b)
				{
					if (_objExplorer.SelectionTypeScope != null)
					{
						if (o is IProperty)
						{
							b = (_objExplorer.SelectionTypeScope.IsAssignableFrom(typeof(IProperty)));
						}
						if (!b)
						{
							IObjectPointer op = o as IObjectPointer;
							if (op != null)
							{
								b = _objExplorer.SelectionTypeScope.IsAssignableFrom(op.ObjectType);
							}
						}
					}
				}
				btOK.Enabled = b;
				if (b)
				{
					SelectedDataType = null;
					SelectedMethod = null;
					SelectedAction = null;
					PropertyClass prop = o as PropertyClass;
					if (prop != null)
					{
						SelectedObject = new CustomPropertyPointer(prop, node.GetHolder());
					}
					else
					{
						EventClass ec = o as EventClass;
						if (ec != null)
						{
							SelectedObject = new CustomEventPointer(ec, node.GetHolder());
						}
						else
						{
							MethodClass mc = o as MethodClass;
							if (mc != null)
							{
								SelectedObject = new CustomMethodPointer(mc, node.GetHolder());
							}
							else
							{
								SelectedObject = o;
							}
						}
					}
					if (node is TreeNodeAction)
					{
						SelectedAction = ((TreeNodeAction)node).Action;
						if (SelectedAction.ActionMethod != null)
						{
							SelectedMethod = SelectedAction.ActionMethod.MethodPointed;
						}
					}
					if (node is TreeNodeMethod)
					{
						SelectedMethod = (MethodPointer)node.OwnerPointer;
					}
					if (node is TreeNodeClassRoot)
					{
						SelectedDataType = new DataTypePointer(((TreeNodeClassRoot)node).RootObjectId);
					}
					else if (node is TreeNodeClassType)
					{
						SelectedDataType = (DataTypePointer)((TreeNodeClassType)node).OwnerPointer;
					}
					else
					{
						TypePointer tp = SelectedObject as TypePointer;
						if (tp != null)
						{
							SelectedDataType = new DataTypePointer(tp);
						}
					}
				}
			}
			else
			{
				btOK.Enabled = false;
			}
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			DlgSelAssemblies dlg = new DlgSelAssemblies();
			dlg.LoadUsedTypes(_project, _objExplorer.ScopeMethodId);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				if (_usedtypes == null)
				{
					_usedtypes = new List<Type>();
				}
				if (!_usedtypes.Contains(dlg.SelectedType))
				{
					_usedtypes.Add(dlg.SelectedType);
				}
				TreeNodeClassType ct = new TreeNodeClassType(_objExplorer, dlg.SelectedType, _objExplorer.ScopeMethodId);
				_objExplorer.Nodes.Add(ct);
				_objExplorer.SelectedNode = ct;
				if (_objExplorer.RootId != null)
				{
					_objExplorer.RootId.AddExternalType(dlg.SelectedType);
					if (_project != null)
					{
						Dictionary<UInt32, ILimnorDesignPane> designPanes = _project.GetTypedDataList<ILimnorDesignPane>();
						if (designPanes != null)
						{
							foreach (KeyValuePair<UInt32, ILimnorDesignPane> kv in designPanes)
							{
								if (kv.Key == _objExplorer.RootId.ClassId)
								{
									kv.Value.OnAddExternalType(dlg.SelectedType);
									kv.Value.Loader.NotifyChanges();
									break;
								}
							}
						}
					}
				}
			}
		}
		private void lstRecent_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = lstRecent.SelectedIndex;
			if (n >= 0)
			{
				IObjectPointer op = lstRecent.Items[n] as IObjectPointer;
				if (op != null)
				{
					_objExplorer.SetSelection(_objExplorer.Nodes, op);
					if (_objExplorer.SelectedNode == null)
					{
						btOK.Enabled = false;
						if (op.IsTargeted(_objExplorer.SelectionType))
						{
							SelectedObject = op;
							btOK.Enabled = true;
						}
					}
				}
				else
				{
					IAction act = lstRecent.Items[n] as IAction;
					if (act != null)
					{
						if (act.ActionMethod != null)
						{
							_objExplorer.SetSelection(_objExplorer.Nodes, act.ActionMethod.Owner);
							TreeNodeObject tno = _objExplorer.SelectedNode as TreeNodeObject;
							if (tno != null)
							{
								TreeNodeAction tna = tno.FindActionNode(act);
								if (tna != null)
								{
									_objExplorer.SelectedNode = tna;
								}
							}
						}
					}
				}
			}
		}
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			if (txtName.Resetting)
				return;
			TreeNodeCollection tc;
			TreeNode selected = _objExplorer.SelectedNode;
			if (selected != null)
			{
				if (selected.Parent != null)
				{
					tc = selected.Parent.Nodes;
				}
				else
				{
					tc = _objExplorer.Nodes;
				}
			}
			else
			{
				tc = _objExplorer.Nodes;
			}
			for (int i = 0; i < tc.Count; i++)
			{
				string key = tc[i].Text;
				if (key.StartsWith(txtName.Text, true, System.Globalization.CultureInfo.InvariantCulture))
				{
					_objExplorer.SelectedNode = tc[i];
					txtName.SetText(tc[i].Text);
					break;
				}
			}
		}
		private void searchDot(object sender, EventArgs e)
		{
			KeyPressEventArgs ke = (KeyPressEventArgs)e;
			if (txtName.Resetting)
				return;
			TreeNodeCollection tc;
			TreeNode selected = _objExplorer.SelectedNode;
			if (selected != null)
			{
				if (selected.Parent != null)
				{
					tc = selected.Parent.Nodes;
				}
				else
				{
					tc = _objExplorer.Nodes;
				}
			}
			else
			{
				tc = _objExplorer.Nodes;
			}
			int n = txtName.SelectionStart;
			for (int i = 0; i < tc.Count; i++)
			{
				string key = tc[i].Text;
				if (key.StartsWith(txtName.Text, true, System.Globalization.CultureInfo.InvariantCulture))
				{
					int m = key.IndexOf(ke.KeyChar, n);
					if (m > 0)
					{
						_objExplorer.SelectedNode = tc[i];
						txtName.SetText(tc[i].Text, m);
						break;
					}
				}
			}
		}
		#endregion
	}
	public class TreeNodeClassLoader : TreeNode
	{
		private string _classFile;
		private UInt32 _scopeMethodId;
		private ClassPointer _obj;
		public TreeNodeClassLoader(string classfile, ClassPointer obj, UInt32 scopeMethodId)
		{
			_classFile = classfile;
			_obj = obj;
			_scopeMethodId = scopeMethodId;
			Text = Path.GetFileNameWithoutExtension(classfile);
			ImageIndex = TreeViewObjectExplorer.IMG_CLASS;
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new TreeNode());
		}
		public void LoadClass()
		{
			if (this.TreeView != null)
			{
				FrmObjectExplorer f = this.TreeView.FindForm() as FrmObjectExplorer;
				if (f != null)
				{
					f.LoadClass(_classFile, _obj, _scopeMethodId, this.Index);
				}
			}
		}
	}
	public class TreeNodeMethodReturn : TreeNodeAction
	{
		public TreeNodeMethodReturn(ActionClass returnAct)
			: base(false, returnAct)
		{
			Text = "Abort";
			ImageIndex = TreeViewObjectExplorer.IMG_CANCEL;
			SelectedImageIndex = ImageIndex;
		}
		public ActionClass ReturnAction
		{
			get
			{
				return (ActionClass)(this.OwnerIdentity);
			}
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Action; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return false;
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			return null;
		}
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
