/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using MathExp;
using System.Xml;
using System.ComponentModel.Design.Serialization;
using LimnorDesigner;
using LimnorDesigner.Action;
using XmlSerializer;
using System.Reflection;
using VPL;
using VSPrj;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LimnorDesigner.Event;
using LimnorDesigner.EventMap;
using ProgElements;
using LimnorDesigner.MenuUtil;
using System.Globalization;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public partial class MethodDesignerHolder : UserControl, IObjectTypeUnitTester, IUndoHost, IWithProject, IIconHolder
	{
		#region fields and constructor
		public event EventHandler DesignerSelected;
		private MethodDiagramViewer root;
		private MethodDiagramViewer _currentViewer;
		private MethodTestData _testData;
		private System.Threading.Thread thTest;
		private MethodClass _method;
		private bool bLoading = false;
		private ILimnorDesignerLoader _designer;
		private EnumParameterEditType _parameterEditorType;
		private bool _disableUndo = true;
		private UndoEngine2 _undoEngine;
		private bool _changed;
		private bool _readOnly;
		private int _fixedParameterCount = -1;
		//
		private List<object> _changedObjects;
		private List<UInt32> _originalActionIds;
		//
		private fnVoid miSetViewerBackColor;
		private Button btCloseTab;
		//
		public const string CLIPBOARD_DATA_ACTION = "VOBAction";
		//
		const int IMG_Delete_Enable = 8;
		const int IMG_Delete_Disable = 7;
		const int IMG_Copy_Enable = 4;
		const int IMG_Copy_Disable = 3;
		const int IMG_Cut_Enable = 6;
		const int IMG_Cut_Disable = 5;
		const int IMG_Paste_Enable = 10;
		const int IMG_Paste_Disable = 9;
		const int IMG_UndoEnable = 13;
		const int IMG_UndoDisable = 2;
		const int IMG_RedoEnable = 11;
		const int IMG_RedoDisable = 12;
		//
		public MethodDesignerHolder(ILimnorDesignerLoader designer, UInt32 scopeId)
		{
			_designer = designer;
			//
			miSetViewerBackColor = new fnVoid(setViewerBackColor);
			//
			InitializeComponent();
			_undoEngine = new UndoEngine2();
			//create Editor
			openViewer(ViewerType, null);
			//
			splitContainer1.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);
			splitContainer4.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);
			splitContainer5.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);
			this.Resize += new EventHandler(designview_Resize);
			splitContainer1.SplitterWidth = 3;
			//
			iconsHolder.SetMethodViewer(this);
			//
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
			propertyGrid1.HelpVisible = false;
			propertyGrid1.PropertySort = PropertySort.Alphabetical;
			propertyGrid1.SelectedObject = GetCurrentViewer();
			//
			propertyGrid2.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid2_PropertyValueChanged);
			propertyGrid2.SetOnValueChange(onValueChanged);
			//
			btUndo.Enabled = false;
			btRedo.Enabled = false;
			btTest.Enabled = false;
			//
			btCloseTab = new Button();
			btCloseTab.Size = new Size(16, 16);
			btCloseTab.Image = Resources._close.ToBitmap();
			//
			btCloseTab.Click += new EventHandler(btCloseTab_Click);
			btCloseTab.Enabled = true;
			btCloseTab.Visible = false;
			tabControl1.Parent.Controls.Add(btCloseTab);
			btCloseTab.Top = tabControl1.Top;
			btCloseTab.Left = tabControl1.Left + tabControl1.Width - btCloseTab.Width;
			btCloseTab.BringToFront();
			btCloseTab.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			//
			root.OnActivate();
			_currentViewer = root;
			//
			tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_TabIndexChanged);
		}

		void tabControl1_TabIndexChanged(object sender, EventArgs e)
		{
			if (_currentViewer != null)
			{
				_currentViewer.OnDeactivate();
			}
			TabPageActionGroup tp = tabControl1.SelectedTab as TabPageActionGroup;
			btCloseTab.Visible = (tp != null);
			_currentViewer = this.GetCurrentViewer();
			_currentViewer.OnActivate();
		}
		void btCloseTab_Click(object sender, EventArgs e)
		{
			TabPageActionGroup tp = tabControl1.SelectedTab as TabPageActionGroup;
			if (tp != null)
			{
				CloseActionGroup(tp.ActionGroup.BranchId);
			}
		}
		#endregion
		#region properties
		public bool ReadOnly
		{
			get
			{
				return ContentReadOnly;
			}
		}
		public List<ComponentIcon> IconList
		{
			get
			{
				return iconsHolder.ComponentIcons;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 SubScopeId
		{
			get;
			set;
		}
		private IActionsHolder _actsHolder;
		[Browsable(false)]
		public virtual IActionsHolder ActionsHolder
		{
			get
			{
				if (_actsHolder == null)
				{
					if (SubScopeId == 0)
					{
						_actsHolder = Method;
					}
					else if (SubScopeId == this.Method.MemberId)
					{
						_actsHolder = Method;
					}
					else if (SubScopeId == 1)
					{
						_actsHolder = Method.FinalActions;
					}
					else
					{
						foreach (ExceptionHandler eh in Method.ExceptionHandlers)
						{
							if (SubScopeId == eh.ExceptionType.MemberId)
							{
								_actsHolder = eh;
								break;
							}
						}
						if (_actsHolder == null)
						{
							_actsHolder = Method;
						}
					}
				}
				return _actsHolder;
			}
		}
		public List<ComponentIcon> ComponentIcons
		{
			get
			{
				return iconsHolder.ComponentIcons;
			}
		}
		protected ILimnorDesignerLoader DesignerLoader
		{
			get
			{
				return _designer;
			}
		}
		protected virtual Type ViewerType
		{
			get
			{
				return typeof(MethodDiagramViewer);
			}
		}
		public EnumDesignViewerStatus ViewerStatus
		{
			get
			{
				UserControlDebugger c = getDebugUIControl();
				if (c != null)
				{
					//a none-main thread may finish
					if (c.GetRunStatus(this.ThreadId) == EnumRunStatus.Finished)
					{
						return EnumDesignViewerStatus.Finished;
					}
					//a sub-group may finish
					if (ActionGroup.GroupFinished)
					{
						return EnumDesignViewerStatus.Finished;
					}
					//
					if (c.CurrentThreadId == this.ThreadId)
					{
						return EnumDesignViewerStatus.Active;
					}
					return EnumDesignViewerStatus.Inactive;
				}
				return EnumDesignViewerStatus.Inactive;
			}
		}
		/// <summary>
		/// this is only used when editing an action string
		/// </summary>
		public virtual AB_Squential Actions
		{
			get
			{
				return null;
			}
		}
		public virtual IActionGroup ActionGroup
		{
			get
			{
				return _method;
			}
		}

		public int ThreadId { get; set; }
		public bool Stopping { get; set; }
		public bool TestDisabled
		{
			get
			{
				return !btTest.Enabled;
			}
		}
		public bool Changed
		{
			get
			{
				if (_changed)
					return true;
				if (root.Changed)
					return true;
				for (int i = 1; i < tabControl1.TabCount; i++)
				{
					TabPageActionGroup ta = tabControl1.TabPages[i] as TabPageActionGroup;
					if (ta != null)
					{
						if (ta.Viewer.Changed)
						{
							return true;
						}
					}
				}
				return false;
			}
			set
			{
				_changed = true;
			}
		}
		public IObjectPointer MethodOwner
		{
			get
			{
				return _method.Owner;
			}
		}
		public MethodClass Method
		{
			get
			{
				return _method;
			}
		}
		public virtual BranchList ActionList
		{
			get
			{
				return _method.ActionList;
			}
		}
		public ClassPointer RootClass
		{
			get
			{
				return (ClassPointer)_method.Owner;
			}
		}
		public XmlNode RootXmlNode
		{
			get
			{
				return _method.RootXmlNode;
			}
		}
		public bool DisableUndo
		{
			get
			{
				return _disableUndo;
			}
			set
			{
				_disableUndo = value;
			}
		}
		public bool NoCompound
		{
			get
			{
				if (root != null)
					return root.NoCompoundCreation;
				return true;
			}
			set
			{
				if (root != null)
					root.NoCompoundCreation = value;
			}
		}
		public ILimnorDesigner Designer
		{
			get
			{
				return _designer;
			}
		}
		public ILimnorDesignerLoader Loader
		{
			get
			{
				return _designer as ILimnorDesignerLoader;
			}
		}
		public MethodDiagramViewer MainDiagramViewer
		{
			get
			{
				return root;
			}
		}
		public MethodDiagramViewer CurrentDiagramViewer
		{
			get
			{
				return this.GetCurrentViewer();
			}
		}
		public IObjectPointer MethodOwnerRef
		{
			get
			{
				return _method.Owner;
			}
		}
		public ClassPointer ActionEventCollection
		{
			get
			{
				return _designer.GetRootId();
			}
		}
		public bool ContentReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
				if (_readOnly)
				{
					btCancel.Enabled = false;
					btCopy.Enabled = false;
					btCut.Enabled = false;
					btDelete.Enabled = false;
					btDown.Enabled = false;
					btInsert.Enabled = false;
					btOK.Enabled = false;
					btPaste.Enabled = false;
					btRedo.Enabled = false;
					btTest.Enabled = false;
					btUndo.Enabled = false;
					btUp.Enabled = false;
				}
			}
		}
		///// <summary>
		///// together with thread id, this id can be the used to identify this object
		///// </summary>
		//public int StartingBranchId { get; set; }
		#endregion
		#region Design component event handlers
		void componentChangeService_ComponentRemoving(object sender, ComponentEventArgs e)
		{
			if (bLoading)
				return;
			SaveCurrentStateForDeleteUndo();
			//
			MethodDiagramViewer wv = GetCurrentViewer();
			//disconnect ports
			ActionViewer mv = e.Component as ActionViewer;
			if (mv != null)
			{
				PortCollection ports = mv.Ports;
				for (int i = 0; i < ports.Count; i++)
				{
					LinkLineNode l = ports[i];
					l.ClearLine();
					wv.Controls.Remove(ports[i]);
					wv.Controls.Remove(ports[i].Label);
					if (l is LinkLineNodeInPort)
					{
						if (l.LinkedOutPort == null)
						{
							//it is not linked to an output, remove all nodes
							l = (LinkLineNode)l.PrevNode;
							while (l != null)
							{
								wv.Controls.Remove(l);
								l = (LinkLineNode)l.PrevNode;
							}
						}
						else
						{
							//this inport is linked to an outport. disconnect it from the outport
							if (mv.KeepRemovedAction)
							{
								LinkLineNodeOutPort lo = l.LinkedOutPort;
								l = (LinkLineNode)l.PrevNode;
								while (l != null && l != lo)
								{
									l.ClearLine();
									wv.Controls.Remove(l);
									l = (LinkLineNode)l.PrevNode;
								}
								lo.ClearAllLinks();
							}
							else
							{
								l.LinkedOutPort.LinkedPortID = 0;
								//replace this port with a node
								LinkLineNode prev = (LinkLineNode)l.PrevNode;
								LinkLineNode end = new LinkLineNode(null, prev);
								end.Location = l.Location;
								wv.Controls.Add(end);
								prev.SetNext(end);
								if (prev.Line == null)
								{
									prev.CreateForwardLine();
								}
								else
								{
									if (prev.Line.EndPoint == l)
									{
										prev.Line.SetEnd(end);
									}
									else
									{
										end.CreateBackwardLine();
									}
								}
							}
						}
					}
					else if (l is LinkLineNodeOutPort)
					{
						if (l.LinkedInPort == null)
						{
							//it is not linked to an input, remove all nodes
							l = (LinkLineNode)l.NextNode;
							while (l != null)
							{
								wv.Controls.Remove(l);
								l = (LinkLineNode)l.NextNode;
							}
						}
						else
						{
							if (mv.KeepRemovedAction)
							{
								LinkLineNodeInPort li = l.LinkedInPort;
								l = (LinkLineNode)l.NextNode;
								while (l != null && l != li)
								{
									l.ClearLine();
									wv.Controls.Remove(l);
									l = (LinkLineNode)l.NextNode;
								}
								li.ClearAllLinks();
							}
							else
							{
								//disconnect this outport from the inport
								l.LinkedInPort.LinkedPortID = 0;
								//replace this port with a node
								LinkLineNode next = (LinkLineNode)l.NextNode;
								LinkLineNode end = new LinkLineNode(next, null);
								end.Location = l.Location;
								wv.Controls.Add(end);
								next.SetPrevious(end);
								if (next.Line == null)
								{
									next.CreateBackwardLine();
								}
								else
								{
									if (next.Line.StartPoint == l)
									{
										next.Line.SetStart(end);
									}
									else
									{
										end.CreateForwardLine();
									}
								}
							}
						}
					}
				}
			}
		}
		void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			ActionViewer av = e.Component as ActionViewer;
			if (av != null)
			{
				av.OnDeleteAction(_method.RootPointer);
			}
			MethodDiagramViewer wv = GetCurrentViewer();
			wv.Refresh();
		}

		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
		{
			if (bLoading)
				return;
		}

		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			root.Changed = true;
			if (e.ChangedItem.Parent.Value == root)
			{
				if (string.Compare(e.ChangedItem.PropertyDescriptor.Name, "Name", StringComparison.Ordinal) == 0)
				{
					EventHandlerMethod ehm = _method as EventHandlerMethod;
					if (ehm != null)
					{
						Form f = this.FindForm();
						f.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							Resources.TitleEditEventMethod, ehm.Event.Name, e.ChangedItem.Value);
					}
					else
					{
						Form f = this.FindForm();
						f.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							Resources.TitleEditMethod, e.ChangedItem.Value);
					}
				}
			}
			else
			{
				ComponentIconLocal cil = e.ChangedItem.Parent.Value as ComponentIconLocal;
				if (cil != null)
				{
					if (string.Compare(e.ChangedItem.PropertyDescriptor.Name, "Name", StringComparison.Ordinal) == 0)
					{
						cil.ChangeName(e.ChangedItem.Value.ToString());
						root.OnLocalVariableRename(cil.LocalPointer);
					}
				}
				else
				{
					ActionViewer av = e.ChangedItem.Parent.Value as ActionViewer;
					if (av != null)
					{
						if (string.Compare(e.ChangedItem.PropertyDescriptor.Name, "ActionName", StringComparison.Ordinal) == 0)
						{
							AB_SingleAction abs = av.ActionObject as AB_SingleAction;
							if (abs != null)
							{
								IAction act = abs.ActionData;
								if (act != null)
								{
									NameBeforeChangeEventArg nc = new NameBeforeChangeEventArg(e.OldValue as string, e.ChangedItem.Value as string, false);
									if (nc != null)
									{
										if (DesignUtil.IsActionNameInUse(_designer.GetRootId().XmlData, nc.NewName, act.ActionId))
										{
											MessageBox.Show("The action name is in use", "Change action name");
											nc.Cancel = true;
											return;
										}
									}
									root.OnActionNameChanged(nc.NewName, act.WholeActionId);
								}
							}
							av.ResetDisplay();
							av.Refresh();
							av.Parent.Refresh();
						}
					}
				}
			}
			root.Changed = true;
		}
		void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			root.Changed = true;
			IBeforeSerializeNotify bs = propertyGrid2.SelectedObject as IBeforeSerializeNotify;
			if (bs == null)
			{
				PropertiesWrapper pw = propertyGrid2.SelectedObject as PropertiesWrapper;
				if (pw != null)
				{
					object o = pw.Owner.GetPropertyOwner(0, "");
					bs = o as IBeforeSerializeNotify;
					if (bs == null)
					{
						ICustomSerialization cs = o as ICustomSerialization;
						if (cs != null)
						{
							addChangedObject(cs);
						}
					}
				}
			}
			if (bs != null)
			{
				addChangedObject(bs);
			}
		}
		void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			if (!Stopping)
			{
				MethodDiagramViewer wv = GetCurrentViewer();
				ISelectionService selectionService = wv.GetSelectionService();
				if (selectionService != null)
				{
					if (!(selectionService.PrimarySelection is ComponentIcon))
					{
						ClearIconSelection();
					}
					propertyGrid1.SelectedObject = selectionService.PrimarySelection;


					if (!(propertyGrid1.SelectedObject is ParameterClass))
					{
						ComponentInterfaceWrapper iw = propertyGrid1.SelectedObject as ComponentInterfaceWrapper;
						if (iw != null && (iw.Value is ParameterClass))
						{
						}
						else
						{
							listBoxParams.SelectedIndex = -1;
						}
					}
					if (!ContentReadOnly)
					{

						if (propertyGrid1.SelectedObject == null || propertyGrid1.SelectedObject == root || propertyGrid1.SelectedObject == wv)
						{
							btCopy.Enabled = false;
							btCopy.ImageIndex = IMG_Copy_Disable;
							btCut.Enabled = false;
							btCut.ImageIndex = IMG_Cut_Disable;
							btDelete.Enabled = false;
							btDelete.ImageIndex = IMG_Delete_Disable;
						}
						else
						{
							btCopy.Enabled = true;
							btCopy.ImageIndex = IMG_Copy_Enable;
							btCut.Enabled = true;
							btCut.ImageIndex = IMG_Cut_Enable;
							btDelete.Enabled = true;
							btDelete.ImageIndex = IMG_Delete_Enable;
						}
					}
					IPropertiesWrapperOwner wo = selectionService.PrimarySelection as IPropertiesWrapperOwner;
					if (wo == null)
					{
						ActionViewer av = selectionService.PrimarySelection as ActionViewer;
						if (av != null && av.ActionObject != null)
						{
							wo = av.ActionObject;
							AB_SingleAction sa = av.ActionObject as AB_SingleAction;
							if (sa != null && sa.ActionData != null)
							{
								sa.ActionData.SetChangeEvents(p_OnNameChange, onValueChanged);
							}
						}
					}
					if (wo != null && wo.AsWrapper)
					{
						propertyGrid2.SelectedObject = new PropertiesWrapper(wo, 0);
					}
					else
					{
						propertyGrid2.SelectedObject = null;
					}
				}
			}
		}
		#endregion
		#region local event handlers
		void onValueChanged(object sender, EventArgs e)
		{
			_changed = true;
		}
		void designview_Resize(object sender, EventArgs e)
		{
			timerAdjustFrame.Enabled = false;
			timerAdjustFrame.Enabled = true;
		}
		void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			AdjustViewerSize();
		}
		private void timerAdjustFrame_Tick(object sender, EventArgs e)
		{
			if (!Stopping)
			{
				timerAdjustFrame.Enabled = false;
				adjustFrame();
			}
		}
		public void adjustFrame()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ISelectionService selectionService = wv.GetSelectionService();
			bool b1 = (selectionService.PrimarySelection == root);
			bool b2 = selectionService.PrimarySelection == wv;
			if (b1 || b2 || selectionService.PrimarySelection == null)
			{
				IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
				for (int i = 0; i < host.Container.Components.Count; i++)
				{
					if (host.Container.Components[i] != root && host.Container.Components[i] != wv)
					{
						selectionService.SetSelectedComponents(new IComponent[] { host.Container.Components[i] });
						Application.DoEvents();
						Application.DoEvents();
						break;
					}
				}
				if (b1)
				{
					selectionService.SetSelectedComponents(new IComponent[] { root });
				}
				else if (b2)
				{
					selectionService.SetSelectedComponents(new IComponent[] { wv });
				}
			}
		}
		#endregion
		#region Clipboard
		private void enablePaste()
		{
			if (!ContentReadOnly)
			{
				if (HasClipboardData())
				{
					btPaste.Enabled = true;
					btPaste.ImageIndex = IMG_Paste_Enable;
				}
				else
				{
					btPaste.Enabled = false;
					btPaste.ImageIndex = IMG_Paste_Disable;
				}
			}
		}
		private string ClipboardActionDataName
		{
			get
			{
				if (_method != null)
				{
					return CLIPBOARD_DATA_ACTION + _method.ClassId.ToString();
				}
				return CLIPBOARD_DATA_ACTION;
			}
		}
		private void copyToClipboard()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			BranchList selectedActions = wv.ExportSelectedActions();
			if (selectedActions.Count > 0)
			{
				XmlObjectWriter xw = new XmlObjectWriter(this.RootClass.ObjectList);
				XmlNode node = this.RootXmlNode.OwnerDocument.CreateElement(CLIPBOARD_DATA_ACTION);
				xw.WriteObjectToNode(node, selectedActions);
				Clipboard.SetData(ClipboardActionDataName, node.OuterXml);
				enablePaste();
			}
		}
		public bool HasClipboardData()
		{
			return Clipboard.ContainsData(ClipboardActionDataName);
		}
		public void PasteFromClipboard()
		{
			bool b = this.DisableUndo;
			this.DisableUndo = true;
			try
			{
				if (Clipboard.ContainsData(ClipboardActionDataName))
				{
					object data = Clipboard.GetData(ClipboardActionDataName);
					if (data != null)
					{
						MethodDiagramViewer wv = GetCurrentViewer();
						XmlObjectReader xr = this.RootClass.ObjectList.Reader;
						string s = data.ToString();
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(s);
						if (doc.DocumentElement != null)
						{
							BranchList actions = (BranchList)xr.ReadObject(doc.DocumentElement, null);
							if (actions.Count > 0)
							{
								UndoActionGroupState state0 = null;
								if (!b)
								{
									state0 = new UndoActionGroupState(this, wv, wv.ExportActions());
								}
								//cut-off extra linked nodes for inports/outports
								foreach (ActionBranch ab in actions)
								{
									ab.AdjustInOutPorts();
								}
								wv.LoadActions(actions);
								List<object> viewers = new List<object>();
								//shift positions to avoid complete overlap
								foreach (Control c in wv.Controls)
								{
									ActionViewer av = c as ActionViewer;
									if (av != null)
									{
										if (actions.ContainsAction(av.ActionObject.BranchId))
										{
											foreach (ActionPortOut po in av.ActionObject.OutPortList)
											{
												po.ResetActiveDrawingID();
											}
											viewers.Add(av);
											av.BringToFront();
											av.Location = new Point(av.Location.X + 5, av.Location.Y + 5);
											foreach (ActionPortIn pi in av.ActionObject.InPortList)
											{
												pi.ResetActiveDrawingID();
												if (pi.LinkedOutPort != null)
												{
													ILinkLineNode pp = pi.PrevNode;
													while (pp != null && !(pp is LinkLineNodePort))
													{
														pp.Left = pp.Left + 5;
														pp.Top = pp.Top + 5;
														pp = pp.PrevNode;
													}
												}
											}
										}
									}
								}
								ISelectionService selectionService = wv.GetSelectionService();
								selectionService.SetSelectedComponents(viewers);
								if (!b)
								{
									UndoActionGroupState state1 = new UndoActionGroupState(this, wv, wv.ExportActions());
									this.DisableUndo = b;
									AddUndoEntity(new UndoEntity(state0, state1));
								}
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
			this.DisableUndo = b;
		}
		#endregion
		#region Public Methods
		public void FixMissingPortLine()
		{
			MethodDiagramViewer mv = GetCurrentViewer();
			if (mv != null)
			{
				mv.FixMissingPortLine();
			}
		}
		public void SetIconSelection(ComponentIcon v)
		{
			SetSelection(v);
		}
		public void LinkAction(ActionViewer v, Point loc)
		{
			MethodDiagramViewer mv = v.Parent as MethodDiagramViewer;
			if (mv != null)
			{
				mv.LinkNewAction(v, loc);
			}
		}

		protected virtual void OnRemoveLocalVariable(ComponentIconLocal v)
		{
			Method.RemoveLocalVariable(v);
		}
		public void RemoveLocalVariable(ComponentIconLocal v)
		{
			OnRemoveLocalVariable(v);
			
		}
		public IList<MethodDiagramViewer> GetViewer()
		{
			List<MethodDiagramViewer> l = new List<MethodDiagramViewer>();
			l.Add(root);
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					l.Add(tpa.Viewer);
				}
			}
			return l;
		}
		public string CreateNewName(string baseName)
		{
			return CreateName(baseName, GetNames());
		}
		public ActionAssignInstance CreateSetValueAction(LocalVariable lv)
		{
			string newName = CreateNewName("Value");
			ActionAssignInstance act = new ActionAssignInstance(Method.RootPointer);
			act.ActionOwner = lv;
			act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			act.ActionName = newName;
			act.ScopeMethod = Method;
			act.ActionHolder = ActionsHolder;
			act.ValidateParameterValues();
			return act;
		}
		public ActionBranch SearchBranchById(UInt32 id)
		{
			BranchList bl = ExportActions();
			if (bl != null)
			{
				return bl.SearchBranchById(id);
			}
			return null;
		}
		public void GetCurrentActionNames(StringCollection sc)
		{
			root.GetCurrentActionNames(sc);
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup page = tabControl1.TabPages[i] as TabPageActionGroup;
				if (page != null)
				{
					page.Viewer.GetCurrentActionNames(sc);
				}
			}
		}
		public void OpenActionGroup(AB_Group group)
		{
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup page = tabControl1.TabPages[i] as TabPageActionGroup;
				if (page != null)
				{
					if (page.ActionGroup.BranchId == group.BranchId)
					{
						tabControl1.SelectTab(i);
						return;
					}
				}
			}
			openViewer(typeof(MethodDiagramViewerActionGroup), group);
		}
		private void openViewer(Type t, AB_Group group)
		{
			DesignSurface dsf = new DesignSurface(t); //ViewerType is a MethodDiagramViewer
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			MethodDiagramViewer mv = (MethodDiagramViewer)host.RootComponent;
			mv.AssignHolder(this, dsf);
			mv.Dock = DockStyle.Fill;
			Control control = dsf.View as Control; //DesignerFrame
			TabPageActionGroup tp = null;
			if (group == null)
			{
				root = mv;
				tabControl1.TabPages[0].Controls.Add(control);
			}
			else
			{
				tp = new TabPageActionGroup(group, mv as MethodDiagramViewerActionGroup, control);
				tabControl1.TabPages.Add(tp);
			}
			control.Dock = DockStyle.Fill;
			control.Visible = true;
			//
			host.AddService(typeof(PropertyGrid), propertyGrid1);
			host.AddService(typeof(INameCreationService), new MethodNameCreation(this));
			//
			ISelectionService selectionService = (ISelectionService)dsf.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
			}
			IComponentChangeService componentChangeService = (IComponentChangeService)dsf.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded += new ComponentEventHandler(componentChangeService_ComponentAdded);
			componentChangeService.ComponentRemoving += new ComponentEventHandler(componentChangeService_ComponentRemoving);
			componentChangeService.ComponentRemoved += new ComponentEventHandler(componentChangeService_ComponentRemoved);
			//
			if (group != null)
			{
				((MethodDiagramViewerActionGroup)mv).LoadActionGroup(group);
				tabControl1.SelectTab(tp);
			}
			mv.FixMissingPortLine();
		}
		public void CloseActionGroup(UInt32 branchId)
		{
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup page = tabControl1.TabPages[i] as TabPageActionGroup;
				if (page != null)
				{
					if (page.ActionGroup.BranchId == branchId)
					{
						page.Viewer.Save();
						tabControl1.TabPages.RemoveAt(i);
						return;
					}
				}
			}
		}
		public StringCollection GetNames()
		{
			StringCollection sc = new StringCollection();
			MethodClass.GetActionNamesInEditors(sc, Method.MemberId);
			return sc;
		}
		public ComponentIconLocal CreateLocalVariable(DataTypePointer type, Point location, MethodClass actionMethod, bool createConstructor)
		{
			ComponentIconLocal c = null;
			Changed = true;
			ClassPointer rootClass = Loader.GetRootId();
			StringCollection names = GetNames();
			string name;
			LocalVariable v;
			type.Owner = rootClass;
			if (actionMethod != null)
			{
				createConstructor = false;
				name = AskNewName(string.Format(CultureInfo.InvariantCulture, "{0}Ret", actionMethod.Name), names);
				CustomMethodReturnPointer v0 = new CustomMethodReturnPointer(type, name, Loader.ClassID, (UInt32)Guid.NewGuid().GetHashCode());
				v0.MethodId = actionMethod.MethodID;
				v = v0;
			}
			else
			{
				name = AskNewName(type.BaseName, names);
				v = type.CreateVariable(name, Loader.ClassID, (UInt32)Guid.NewGuid().GetHashCode());
			}
			v.Owner = rootClass;
			LocalVariable.SaveLocalVariable(v);
			//
			bool bOK = createConstructor;
			bool bCreateComponent = false;
			if (createConstructor)
			{
				//create constructor action
				ActionClass act = new ActionClass(rootClass);
				act.ActionId = (UInt32)Guid.NewGuid().GetHashCode();
				act.ActionName = "Create " + name;
				act.ScopeMethodId = Method.MethodID;
				act.SubScopeId = SubScopeId;
				act.ActionHolder = ActionsHolder;
				dlgConstructorParameters dlg = new dlgConstructorParameters();
				dlg.SetMethod(root.DesignerHolder.Method);
				if (dlg.LoadData(v))
				{
					if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
					{
						//add component
						bCreateComponent = true;
						//
						if (dlg.Ret is ConstructorPointerNull)
						{
							bOK = false;
						}
						else
						{
							dlg.Ret.SetHolder(rootClass);
							act.ActionMethod = dlg.Ret;
							act.ReturnReceiver = v;
							if (dlg.Ret.ParameterCount > 0)
							{
								for (int i = 0; i < dlg.Ret.ParameterCount; i++)
								{
									ParameterInfo pif = dlg.Ret.GetParameterTypeByIndex(i) as ParameterInfo;
									if (pif != null)
									{
										if (pif.ParameterType.IsGenericParameter)
										{
											Type[] gas = v.BaseClassType.GetGenericArguments();
											for (int k = 0; k < gas.Length; k++)
											{
												if (gas[k].Equals(pif.ParameterType))
												{
													act.SetParameterTypeByIndex(i, v.ClassType.TypeParameters[gas[k].GenericParameterPosition]);
													break;
												}
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bOK = false;
					}
				}
				else
				{
					//add component
					bCreateComponent = true;

					bOK = false;
				}
				if (bOK)
				{
					//add the action
					Loader.GetRootId().SaveAction(act, Loader.Writer);
					double x0, y0;
					int x2 = this.ClientSize.Width / 4;
					int y2 = this.ClientSize.Height / 4;
					ComponentIconEvent.CreateRandomPoint(Math.Min(x2, y2), out x0, out y0);
					AddNewAction(act, new Point((int)x0 + this.ClientSize.Width / 2, (int)y0 + this.ClientSize.Height / 2));
				}
			}
			else
			{
				bCreateComponent = true;
			}
			if (bCreateComponent)
			{
				v.ScopeGroupId = ActionGroup.GroupId;
				PrepareNewIconLocation(location);
				c = v.CreateComponentIcon(Designer, Method);
				c.Location = location;
				AddControlToIconsHolder(c);
				ActionGroup.ComponentIconList.Add(c);
				c.Visible = true;
				c.BringToFront2();
				c.ScopeGroupId = ActionGroup.GroupId;
				v.SetTypeChangeNotify(onLocalVariabletypeChanged);
			}
			return c;
		}
		private void onLocalVariabletypeChanged(object sender, EventArgs e)
		{
			root.onLocalVariabletypeChanged(sender, e);
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.onLocalVariabletypeChanged(sender, e);
				}
			}
		}
		public void SetForSubScope()
		{
			root.SetForSubScope();
		}
		public void SetAttributesReadOnly(bool readOnly)
		{
			root.SetAttributesReadOnly(readOnly);
		}
		public void SetSubScopeId(UInt32 id)
		{
			SubScopeId = id;
		}
		public void SetForSubMethod(MethodDiagramViewer parentEditor)
		{
			root.SetForSubMethod(parentEditor);
		}
		public void OnClosing()
		{
			root.OnClosing();
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.OnClosing();
				}
			}
			iconsHolder.SaveIconLocations();
		}
		public void PrepareNewIconLocation(Point loc)
		{
			iconsHolder.PrepareNewIconLocation(loc);
		}
		public void SetRootSelection()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ISelectionService selectionService = wv.GetSelectionService();
			if (selectionService != null)
			{
				selectionService.SetSelectedComponents(new object[] { wv });
			}
			else
			{
				propertyGrid1.SelectedObject = wv;
			}
		}
		public void SetSelection(object p)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ISelectionService selectionService = wv.GetSelectionService();
			if (selectionService != null)
			{
				if (p is ParameterClass)
					propertyGrid1.SelectedObject = p;
				else
					selectionService.SetSelectedComponents(new object[] { ComponentInterfaceWrapper.WrappObject(p) });
			}
			else
			{
				propertyGrid1.SelectedObject = p;
			}
			if (!(p is ComponentIcon))
			{
				ClearIconSelection();
			}
		}
		public ReadOnlyCollection<object> GetAllObjects()
		{
			return _designer.ObjectMap.GetAllObjects();
		}
		public void AdjustViewerSize()
		{
			if (!Stopping)
			{
				timerAdjustFrame.Enabled = false;
				timerAdjustFrame.Enabled = true;
			}
		}
		/// <summary>
		/// remove it from SplitContainer.Panel1.
		/// 1. SplitContainer.Panel2.Controls.Count == 0
		///     remove the SplitContainer
		/// 2. SplitContainer.Panel2.Controls.Count == 1
		///     SplitContainer.Panel2.Controls[0] should be a SplitContainer
		///     remove the SplitContainer
		///     add SplitContainer.Panel2.Controls[0] to replace SplitContainer
		/// </summary>
		public void UnloadFromSplitContainer()
		{
			//it should be hosted in a SplitContainer.Panel1
			SplitterPanel panel = this.Parent as SplitterPanel;
			if (panel != null)
			{
				SplitContainer sl = panel.Parent as SplitContainer;
				if (sl != null)
				{
					Control cp = sl.Parent;
					if (cp != null)
					{
						if (sl.Panel2.Controls.Count == 0)
						{
							cp.Controls.Remove(sl);
							panel = cp as SplitterPanel;
							if (panel != null)
							{
								sl = panel.Parent as SplitContainer;
								if (sl != null)
								{
									sl.Panel2Collapsed = true;
								}
							}
						}
						else if (sl.Panel2.Controls.Count == 1)
						{
							Control cn = sl.Panel2.Controls[0];
							sl.Panel2.Controls.Remove(cn);
							cp.Controls.Remove(sl);
							cp.Controls.Add(cn);
						}
					}
				}
			}
		}
		public void SetViewerBackColor()
		{
			this.Invoke(miSetViewerBackColor);
		}

		public List<ActionViewer> GetSelectedActions()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ISelectionService selectionService = wv.GetSelectionService();
			List<ActionViewer> list = new List<ActionViewer>();
			System.Collections.ICollection ic = selectionService.GetSelectedComponents();
			if (ic != null && ic.Count > 0)
			{
				System.Collections.IEnumerator ie = ic.GetEnumerator();
				while (ie.MoveNext())
				{
					ActionViewer av = ie.Current as ActionViewer;
					if (av != null)
					{
						list.Add(av);
					}
				}
			}
			return list;
		}
		public void SaveCurrentStateForDeleteUndo()
		{
			if (!this.DisableUndo)
			{
				MethodDiagramViewer wv = GetCurrentViewer();
				ISelectionService selectionService = wv.GetSelectionService();
				System.Collections.ICollection ic = selectionService.GetSelectedComponents();
				if (ic != null && ic.Count > 0)
				{
					List<UInt32> idList = new List<UInt32>();
					System.Collections.IEnumerator ie = ic.GetEnumerator();
					while (ie.MoveNext())
					{
						IActiveDrawing a = ie.Current as IActiveDrawing;
						if (a != null)
						{
							idList.Add(a.ActiveDrawingID);
						}
					}
					if (idList.Count > 0)
					{
						IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
						AddUndoEntity(new UndoEntity(new UndoActionGroupState(this, wv, wv.ExportActions()), new UndoDelComponent(this, idList, host)));
					}
				}
			}
		}
		public void AddControlToIconsHolder(Control control)
		{
			iconsHolder.Controls.Add(control);
		}
		public void AddControlsToIconsHolder(Control[] controls)
		{
			iconsHolder.Controls.AddRange(controls);
		}
		public void AddNewParameterIcon(ParameterClass pr)
		{
			ComponentIconParameter cip = new ComponentIconParameter(Designer, pr, Method);
			double x, y;
			int x0 = iconsHolder.ClientSize.Width / 2;
			int y0 = iconsHolder.ClientSize.Height / 2;
			ComponentIconEvent.CreateRandomPoint(Math.Min(x0, y0), out x, out y);
			cip.Location = new Point(x0 + (int)x, y0 + (int)y);
			iconsHolder.Controls.Add(cip);
			cip.Show();
		}
		public MethodDiagramViewer GetCurrentViewer()
		{
			if (tabControl1.SelectedIndex == 0)
			{
				return root;
			}
			TabPageActionGroup tag = tabControl1.SelectedTab as TabPageActionGroup;
			if (tag != null)
			{
				return tag.Viewer;
			}
			return root;
		}
		public void SaveIconsLocations()
		{
			foreach (Control c in iconsHolder.Controls)
			{
				ActiveDrawing ad = c as ActiveDrawing;
				if (ad != null)
				{
					ad.SaveLocation();
				}
			}
		}
		public void ClearIconSelection()
		{
			for (int i = 0; i < iconsHolder.Controls.Count; i++)
			{
				ComponentIcon ci = iconsHolder.Controls[i] as ComponentIcon;
				if (ci != null)
				{
					if (ci.IsSelected)
					{
						ci.IsSelected = false;
						ci.Invalidate();
					}
				}
			}
		}
		public void RemoveParameterIcon(UInt32 parameterId)
		{
			for (int i = 0; i < iconsHolder.Controls.Count; i++)
			{
				ComponentIconParameter cip = iconsHolder.Controls[i] as ComponentIconParameter;
				if (cip != null)
				{
					if (cip.ParameterType.ParameterID == parameterId)
					{
						Method.ComponentIconList.Remove(cip);
						iconsHolder.Controls.Remove(cip);
						break;
					}
				}
			}
		}
		public void SetChangedAction(IAction act)
		{
			if (act != null)
			{
				if (!root.ChangedActions.ContainsKey(act.ActionId))
				{
					root.ChangedActions.Add(act.ActionId, act);
				}
			}
		}
		protected void OnSave()
		{
			if (_changedObjects != null)
			{
				foreach (object v in _changedObjects)
				{
					IBeforeSerializeNotify bs = v as IBeforeSerializeNotify;
					if (bs != null)
					{
						bs.UpdateXmlNode(_designer.Writer);
					}
					else
					{
						ICustomSerialization cs = v as ICustomSerialization;
						if (cs != null)
						{
							cs.OnWriteToXmlNode(_designer.Writer, cs.CachedXmlNode);
						}
					}
				}
			}
			foreach (IAction a in root.ChangedActions.Values)
			{
				a.UpdateXmlNode(_designer.Writer);
			}
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					foreach (IAction a in tpa.Viewer.ChangedActions.Values)
					{
						a.UpdateXmlNode(_designer.Writer);
					}
				}
			}
		}
		public bool SaveViewers()
		{
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.Save();
				}
			}
			return root.Save();
		}
		public virtual bool Save()
		{
			OnSave();

			Form f = this.FindForm();
			if (f != null)
			{
				_method.EditorBounds = f.Bounds;
			}
			_method.ViewerHeight = splitContainer1.SplitterDistance;
			_method.IconsHolderWidth = splitContainer5.SplitterDistance;
			_method.ViewerWidth = splitContainer4.SplitterDistance;

			if (SaveViewers())
			{
				return true;
			}
			return false;
		}
		public void DisableTest()
		{
			btTest.Enabled = false;
		}
		public string AskNewName(string baseName, StringCollection names)
		{
			DlgVariableName dlg = new DlgVariableName();
			dlg.LoadData(this, CreateName(baseName, names));
			dlg.ShowDialog(this.FindForm());
			return dlg.VariableName;
		}
		public string CreateName(string baseName, StringCollection names)
		{
			return _designer.CreateMethodName(baseName, names);
		}
		public bool IsMethodNameUsed(string name)
		{
			return _designer.IsMethodNameUsed(_method.MemberId, name);
		}
		public bool IsNameUsed(string name)
		{
			return _designer.IsNameUsed(name, this._method.MemberId);
		}
		public virtual void LoadActions(AB_Squential actions)
		{
		}
		public virtual void LoadActions(IActionGroup actions)
		{
			ContentReadOnly = true;
			LoadMethod(actions.ActionList.Method, EnumParameterEditType.TypeReadOnly);
		}
		public void SetBackgroundText(string text)
		{
			root.BKTEXT = text;
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.BKTEXT = text;
				}
			}
		}
		public void LoadMethod(MethodClass method, EnumParameterEditType parameterEditType)
		{
			if (method == null)
			{
				throw new DesignerException("method is null calling LoadMethod by {0}", this.GetType().FullName);
			}
			FormProgress.ShowProgress("Loading Method Editor, please wait ...");
			bLoading = true;
			btUp.Visible = !method.IsOverride;
			btDown.Visible = !method.IsOverride;
			_originalActionIds = new List<uint>();
			Dictionary<UInt32, IAction> acts = method.RootPointer.GetActions();
			foreach (UInt32 a in acts.Keys)
			{
				_originalActionIds.Add(a);
			}
			propertyGrid1.ScopeMethod = method;
			propertyGrid2.ScopeMethod = method;
			_parameterEditorType = parameterEditType;
			ConstructorClass constructor = method as ConstructorClass;
			if (constructor != null)
			{
				_fixedParameterCount = constructor.FixedParameters;
				root.HideName = true;
			}
			IDesignerHost host = (IDesignerHost)root.DesignSurface.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				ComponentCollection cc = host.Container.Components;
				foreach (IComponent c in cc)
				{
					if (c != root)
					{
						host.DestroyComponent(c);
					}
				}
			}
			_method = method;
			//
			root.LoadMethod();
			//
			LoadParameters();
			//
			if (_method.EditorBounds != Rectangle.Empty)
			{
				Form f = this.FindForm();
				if (f != null)
				{
					f.Bounds = _method.EditorBounds;
					if (_method.ViewerHeight > 2 && _method.ViewerHeight < f.ClientSize.Height)
					{
						splitContainer1.SplitterDistance = _method.ViewerHeight;
					}
					if (_method.IconsHolderWidth > 2 && _method.IconsHolderWidth < f.ClientSize.Width)
					{
						splitContainer5.SplitterDistance = _method.IconsHolderWidth;
					}
					if (_method.ViewerWidth > 2 && _method.ViewerWidth < splitContainer4.ClientSize.Width)
					{
						splitContainer4.SplitterDistance = _method.ViewerWidth;
					}
				}
			}
			//
			enablePaste();
			//
			root.Changed = false;
			bLoading = false;
			FormProgress.HideProgress();
		}
		public virtual void CancelEdit()
		{
			//find out new actions
			Dictionary<UInt32, IAction> acts = root.ActionsHolder.ActionInstances;
			foreach (IAction a in acts.Values)
			{
				if (a != null)
				{
					if (isOriginalAction(a.ActionId))
					{
						if (a.HasChangedXmlData)
						{
							if (!root.ChangedActions.ContainsKey(a.ActionId))
							{
								root.ChangedActions.Add(a.ActionId, a);
							}
						}
					}
				}
			}
			foreach (IAction a in root.ChangedActions.Values)
			{
				a.ReloadFromXmlNode();
			}
			if (_method.MethodID != 0)
			{
				_method.ReloadMethod(Loader);
			}
		}
		public ActionViewerIF LoadNewCondition(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("Condition");
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViewerIF v = (ActionViewerIF)host.CreateComponent(typeof(ActionViewerIF), "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			v.Location = pos;
			//
			wv.Controls.Add(v);
			//
			AB_ConditionBranch branch = new AB_ConditionBranch(root.ActionsHolder, pos, new Size(150, 50));
			branch.Name = newName;
			branch.ActionsHolder = root.ActionsHolder;
			List<UInt32> used = new List<uint>();
			branch.SetOwnerMethod(used, _method);
			v.ImportAction(branch, true);
			wv.ValidateControlPositions();
			return v;
		}
		public ActionViewerCastAs LoadNewCastAs(ParameterClassSubMethod source, LocalVariable target)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("cast");
			double x, y;
			ComponentIconEvent.CreateRandomPoint((double)(wv.ClientSize.Width / 2), out x, out y);
			Point pos = new Point((int)x, (int)y);
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViewerCastAs v = (ActionViewerCastAs)host.CreateComponent(typeof(ActionViewerCastAs), "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			v.Location = pos;
			//
			wv.Controls.Add(v);
			//
			AB_CastAs branch = new AB_CastAs(root.ActionsHolder, pos, new Size(150, 50));
			branch.Name = newName;
			branch.SourceObject = source.CreatePointer();
			branch.TargetObject = target;
			List<UInt32> used = new List<uint>();
			branch.SetOwnerMethod(used, _method);
			v.ImportAction(branch, true);
			wv.ValidateControlPositions();
			return v;
		}
		public ActionViewerLoop LoadNewLoop(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("Repeat");
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViewerLoop v = (ActionViewerLoop)host.CreateComponent(typeof(ActionViewerLoop), "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			v.Location = pos;
			//
			wv.Controls.Add(v);
			//
			AB_LoopActions branch = new AB_LoopActions(root.ActionsHolder, pos, new Size(150, 50));
			branch.Name = newName;
			branch.Description = "Repeated actions";
			List<UInt32> used = new List<uint>();
			branch.SetOwnerMethod(used, _method);
			v.ImportAction(branch, true);
			wv.ValidateControlPositions();
			return v;
		}
		public ActionViewer LoadNewForEachLoop(Point pos)
		{
			CollectionScoper scope = new CollectionScoper();
			FrmObjectExplorer dlg = DesignUtil.GetPropertySelector(null, Method, scope);
			if (dlg != null)
			{
				if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					IObjectPointer itemOwner = dlg.SelectedObject as IObjectPointer;
					if (itemOwner != null)
					{
						MethodDiagramViewer wv = GetCurrentViewer();
						Type ta = itemOwner.ObjectType;
						DataTypePointer ti = new DataTypePointer(VPLUtil.GetElementType(ta));
						if (ti.IsGenericParameter)
						{
							IGenericTypePointer igp = null;
							IObjectPointer op = itemOwner;
							while (op != null)
							{
								igp = op as IGenericTypePointer;
								if (igp != null)
									break;
								op = op.Owner;
							}
							if (igp != null)
							{
								DataTypePointer dp = igp.GetConcreteType(ti.BaseClassType);
								if (dp != null)
								{
									ti.SetConcreteType(dp);
								}
							}
						}
						string sk = itemOwner.ObjectKey;
						if (string.IsNullOrEmpty(sk))
						{
							sk = Guid.NewGuid().GetHashCode().ToString("x").ToString(CultureInfo.InvariantCulture);
						}
						CollectionForEachMethodInfo mif = new CollectionForEachMethodInfo(ti, ta, sk);
						//
						SubMethodInfoPointer mp = new SubMethodInfoPointer();
						mp.Owner = new CollectionPointer(itemOwner);
						mp.MemberName = mif.Name;
						ParameterInfo[] pifs = mif.GetParameters();
						Type[] ts = new Type[pifs.Length];
						for (int i = 0; i < ts.Length; i++)
						{
							ts[i] = pifs[i].ParameterType;
						}
						mp.ParameterTypes = ts;
						mp.SetMethodInfo(mif);
						//
						IAction act = DesignUtil.OnCreateAction(this.RootClass, mp, Method, root.ActionsHolder, this.Loader.DesignPane.PaneHolder, this.Loader.Node);
						//
						act.ScopeMethod = Method;
						act.ActionHolder = root.ActionsHolder;
						double x0, y0;
						ComponentIconEvent.CreateRandomPoint((double)((root.Width - 20) / 2), out x0, out y0);
						ActionViewer v = wv.AddNewAction(act, new Point((wv.Width - 20) / 2 + (int)x0, (wv.Height - 20) / 2 + (int)y0));
						wv.ValidateControlPositions();
						return v;
					}
				}
			}
			return null;
		}
		public ActionViwerForLoop LoadNewForLoop(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("LimitedRepeat");
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViwerForLoop v = (ActionViwerForLoop)host.CreateComponent(typeof(ActionViwerForLoop), "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			v.Location = pos;
			//
			wv.Controls.Add(v);
			//
			AB_ForLoop branch = new AB_ForLoop(root.ActionsHolder, pos, new Size(150, 50));
			branch.Name = newName;
			branch.Description = "Limited repeating actions";
			List<UInt32> used = new List<uint>();
			branch.SetOwnerMethod(used, _method);
			v.ImportAction(branch, true);
			wv.ValidateControlPositions();
			return v;
		}
		public ActionViewer LoadNewDecisionTable(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("Decision");
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViewerDecisionTable v = (ActionViewerDecisionTable)host.CreateComponent(typeof(ActionViewerDecisionTable), "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			v.Location = pos;
			//
			wv.Controls.Add(v);
			//
			AB_DecisionTableActions branch = new AB_DecisionTableActions(root.ActionsHolder, pos, new Size(150, 50));
			branch.Name = newName;
			branch.Description = "Decision table";
			List<UInt32> used = new List<uint>();
			branch.SetOwnerMethod(used, _method);
			v.ImportAction(branch, true);
			wv.ValidateControlPositions();
			return v;
		}
		public ActionViewer LoadNewBreak(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ActionClass act = new ActionClass(RootClass);
			act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			act.ActionName = "break";
			BreakActionMethod mrm = new BreakActionMethod(act);
			act.ActionMethod = mrm;
			act.ScopeMethod = _method;
			act.ScopeMethodId = _method.MemberId;
			//act.SubScopeId = root.SubScopeId;
			act.ActionHolder = root.ActionsHolder;
			//
			RootClass.SaveAction(act, _designer.Writer);
			//
			ActionBranch ab = (ActionBranch)Activator.CreateInstance(mrm.ActionBranchType, root.ActionsHolder, pos, new Size(150, 50));
			ab.Description = "break the iteration of the execution";
			List<UInt32> used = new List<uint>();
			ab.SetOwnerMethod(used, _method);
			AB_SingleAction abs = ab as AB_SingleAction;
			abs.ActionId = new TaskID(act.WholeActionId);
			abs.ActionData = act;
			//
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			//
			ActionViewer av = (ActionViewer)host.CreateComponent(ab.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			av.Location = pos;
			//
			wv.Controls.Add(av);
			//
			av.ImportAction(ab, true);
			wv.ValidateControlPositions();
			return av;
		}
		public ActionViewer LoadNewMethodReturn(Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			ActionClass act = new ActionClass(RootClass);
			act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			act.ActionName = "MethodReturn";
			act.ScopeMethod = _method;
			act.ScopeMethodId = _method.MemberId;
			MethodReturnMethod mrm = new MethodReturnMethod(act);
			act.ActionMethod = mrm;
			mrm.ValidateParameterValues(act.ParameterValues);
			act.ScopeMethod = _method;
			act.ScopeMethodId = _method.MemberId;
			act.ActionHolder = root.ActionsHolder;
			//
			RootClass.SaveAction(act, _designer.Writer);
			//
			ActionBranch ab = (ActionBranch)Activator.CreateInstance(mrm.ActionBranchType, root.ActionsHolder, pos, new Size(150, 50));
			ab.Description = "Method return action";
			List<UInt32> used = new List<uint>();
			ab.SetOwnerMethod(used, _method);
			AB_SingleAction abs = ab as AB_SingleAction;
			abs.ActionId = new TaskID(act.WholeActionId);
			abs.ActionData = act;
			//
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			//
			ActionViewer av = (ActionViewer)host.CreateComponent(ab.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			av.Location = pos;
			//
			wv.Controls.Add(av);
			//
			av.ImportAction(ab, true);
			wv.ValidateControlPositions();
			return av;
			//
		}
		public ActionViewer addNewActionGroup(List<ActionViewer> selectedActions, Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			List<ActionBranch> la = new List<ActionBranch>();
			foreach (ActionViewer av in selectedActions)
			{
				la.Add(av.ActionObject);
			}
			BranchList l = new BranchList(_method, la);
			AB_Group ag = new AB_Group(l);
			l.RemoveOutOfGroupBranches();
			string newName = CreateNewName("group");
			ag.Name = newName;
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			ActionViewer v = (ActionViewer)host.CreateComponent(ag.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
			//
			v.Size = new Size(120, 38);
			wv.Controls.Add(v);
			List<UInt32> used = new List<uint>();
			ag.SetOwnerMethod(used, _method);
			v.ImportAction(ag, true);
			wv.ValidateControlPositions();
			v.Location = pos;
			v.Size = new Size(120, 38);
			//
			ag.InPortList[0].Position = 60;
			ag.InPortList[0].adjustPosition();
			ag.OutPortList[0].Position = 60;
			ag.OutPortList[0].adjustPosition();
			return v;
		}
		public ActionViewer addNewActionList(List<IAction> actionList, Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			string newName = root.CreateNewName("ActionList");
			bool b = this.DisableUndo;
			this.DisableUndo = true;
			UndoActionGroupState state0 = null;
			if (!b)
			{
				state0 = new UndoActionGroupState(this, wv, wv.ExportActions());
			}
			try
			{
				IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
				Type t = typeof(AB_ActionList);
				AB_ActionList av = (AB_ActionList)Activator.CreateInstance(t, root.ActionsHolder, pos, new Size(180, 30));
				foreach (IAction a in actionList)
				{
					av.Actions.Add(new ActionItem(a));
				}
				av.ActionsHolder = root.ActionsHolder;
				ActionViewer v = (ActionViewer)host.CreateComponent(av.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
				v.Location = pos;
				av.Name = newName;
				//
				List<UInt32> used = new List<uint>();
				av.SetOwnerMethod(used, _method);
				wv.Controls.Add(v);
				v.ImportAction(av, true);
				wv.ValidateControlPositions();
				if (!b)
				{
					UndoActionGroupState state1 = new UndoActionGroupState(this, wv, wv.ExportActions());
					this.DisableUndo = b;
					AddUndoEntity(new UndoEntity(state0, state1));
				}
				wv.Changed = true;
				return v;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
			this.DisableUndo = b;
			return null;
		}
		public ActionViewer AddNewAction(ActionBranch item, Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			bool b = this.DisableUndo;
			this.DisableUndo = true;
			UndoActionGroupState state0 = null;
			if (!b)
			{
				state0 = new UndoActionGroupState(this, wv, wv.ExportActions());
			}
			try
			{
				IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
				ActionViewer v = (ActionViewer)host.CreateComponent(item.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
				v.Location = pos;
				v.Size = new Size(120, 30);
				if (v.Parent == null)
				{
					wv.Controls.Add(v);
				}
				item.Location = pos;
				item.Size = v.Size;
				//
				v.ImportAction(item, true);
				wv.ValidateControlPositions();
				//
				if (!b)
				{
					UndoActionGroupState state1 = new UndoActionGroupState(this, wv, wv.ExportActions());
					this.DisableUndo = b;
					AddUndoEntity(new UndoEntity(state0, state1));
				}
				wv.Changed = true;
				v.Visible = true;

				if (!v.Visible)
				{
					MessageBox.Show(this.FindForm(), "New action does not appear in the Action Pane. Please right-click the Action Pane and choose 'Add action' to select the new action from the Actions list under the current method. Sorry for the inconvenience. Close and save this method, close the designer, then re-open the designer and re-open the method will fix the problem.", "Add new action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}

				return v;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
			finally
			{
				this.DisableUndo = b;
			}
			return null;
		}
		public ActionViewer AddNewAction(IAction item, Point pos)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			bool b = this.DisableUndo;
			this.DisableUndo = true;
			UndoActionGroupState state0 = null;
			if (!b)
			{
				state0 = new UndoActionGroupState(this, wv, wv.ExportActions());
			}
			try
			{
				_designer.GetRootId().SaveAction(item, _designer.Writer);
				IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
				Type t = item.ActionBranchType;
				if (t == null)
				{
					t = typeof(AB_SingleAction);
				}
				AB_SingleActionBlock av = (AB_SingleActionBlock)Activator.CreateInstance(t, root.ActionsHolder, pos, new Size(180, 30));
				ActionViewer v = (ActionViewer)host.CreateComponent(av.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
				v.Location = pos;
				//
				ISingleAction sa = av as ISingleAction;
				if (sa != null)
				{
					sa.ActionId = new TaskID(item.WholeActionId);
					sa.ActionData = item;
				}
				List<UInt32> used = new List<uint>();
				av.SetOwnerMethod(used, _method);
				if (v.Parent == null)
				{
					wv.Controls.Add(v);
				}
				v.ImportAction(av, true);
				wv.ValidateControlPositions();
				if (!b)
				{
					UndoActionGroupState state1 = new UndoActionGroupState(this, wv, wv.ExportActions());
					this.DisableUndo = b;
					AddUndoEntity(new UndoEntity(state0, state1));
				}
				wv.Changed = true;
				v.Visible = true;

				if (v.Visible)
				{
					LinkAction(v, pos);
				}
				else
				{
					MessageBox.Show(this.FindForm(), "New action does not appear in the Action Pane. Please right-click the Action Pane and choose 'Add action' to select the new action from the Actions list under the current method. Sorry for the inconvenience. Close and save this method, then re-open the method may fix the problem.", "Add new action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}

				return v;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
			finally
			{
				this.DisableUndo = b;
			}
			return null;
		}
		public BranchList ExportActions()
		{
			if (root != null)
			{
				for (int i = 1; i < tabControl1.TabCount; i++)
				{
					TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
					if (tpa != null)
					{
						tpa.Viewer.Save();
					}
				}
				return root.ExportActions();
			}
			return null;
		}
		public void ExecuteOnShowHandlers()
		{
			if (root != null)
			{
				root.ExecuteOnShowHandlers();
			}
		}
		public void DeleteSelectedComponents()
		{
			SaveCurrentStateForDeleteUndo();
			bool b = this.DisableUndo;
			this.DisableUndo = true;
			MethodDiagramViewer wv = GetCurrentViewer();
			ISelectionService selectionService = wv.GetSelectionService();
			System.Collections.ICollection ic = selectionService.GetSelectedComponents();
			if (ic != null && ic.Count > 0)
			{
				IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
				System.Collections.IEnumerator ie = ic.GetEnumerator();
				while (ie.MoveNext())
				{
					host.DestroyComponent((IComponent)ie.Current);
				}
				wv.Changed = true;
			}
			this.DisableUndo = b;
		}
		public IAction GetAction(TaskID id)
		{
			IAction a = root.ActionsHolder.GetActionInstance(id.ActionId);// _designer.GetRootId().GetAction(id);
			if (a == null)
			{
				List<IAction> lst = Method.GetActions();
				if (lst != null && lst.Count > 0)
				{
					foreach (IAction act in lst)
					{
						if (act.ActionId == id.ActionId)
						{
							a = act;
							break;
						}
					}
				}
				if (a == null && Method.SubMethod != null)
				{
					Stack<IMethod0>.Enumerator ms = Method.SubMethod.GetEnumerator();
					while (ms.MoveNext())
					{
						ISubMethod sub = ms.Current as ISubMethod;
						if (sub != null)
						{
							a = sub.GetActionById(id.ActionId);
							if (a != null)
							{
								return a;
							}
						}
					}
				}
			}
			return a;
		}
		public void LoadNewActionList(Point location)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			List<IAction> actList = DesignUtil.SelectAction(this.Loader, null, null, true, _method, root.ActionsHolder, this.FindForm());
			if (actList != null && actList.Count > 0)
			{
				addNewActionList(actList, location);
				wv.Changed = true;
			}
		}
		/// <summary>
		/// add an existing action to the method
		/// </summary>
		/// <param name="location"></param>
		public void AddNewAction(Point location)
		{
			List<IAction> actList = DesignUtil.SelectAction(this.Loader, null, null, true, _method, root.ActionsHolder, this.FindForm());
			if (actList != null && actList.Count > 0)
			{
				MethodDiagramViewer wv = GetCurrentViewer();
				if (actList.Count == 1)
				{
					AssignHandler ah = actList[0] as AssignHandler;
					if (ah != null)
					{
						wv.AddAssignActionsAction(ah.Event);
					}
					else
					{
						AddNewAction(actList[0], location);
					}
				}
				else
				{
					addNewActionList(actList, location);
				}
				wv.Changed = true;
			}
		}
		public void ClearAllComponent()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				IComponent[] clst = new IComponent[host.Container.Components.Count];
				host.Container.Components.CopyTo(clst, 0);
				for (int i = 0; i < clst.Length; i++)
				{
					if (clst[i] != root)
					{
						host.DestroyComponent(clst[i]);
					}
				}
			}
		}
		public void DeleteComponent(IComponent c)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			IDesignerHost host = (IDesignerHost)wv.DesignSurface.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				host.DestroyComponent(c);
			}
		}
		public void UpdateBreakpoint(ActionBranch branch)
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			wv.UpdateBreakpoint(branch);
		}
		public void ClearBreakpoint()
		{
			MethodDiagramViewer wv = GetCurrentViewer();
			wv.ClearBreakpoint();
		}
		public void OnDesignerSelected()
		{
			if (DesignerSelected != null)
			{
				DesignerSelected(this, EventArgs.Empty);
			}
			MethodDiagramViewer wv = GetCurrentViewer();
			SetSelection(wv);
		}
		public void SetDebugBehavior()
		{
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.SetDebugBehavior();
				}
			}
			root.SetDebugBehavior();
		}
		public void SetNameReadOnly()
		{
			root.NameReadOnly = true;
		}
		#endregion
		#region private methods
		private bool isOriginalAction(UInt32 id)
		{
			if (_originalActionIds != null && _originalActionIds.Count > 0)
			{
				foreach (UInt32 x in _originalActionIds)
				{
					if (x == id)
					{
						return true;
					}
				}
			}
			return false;
		}
		private void addChangedObject(object obj)
		{
			if (_changedObjects == null)
			{
				_changedObjects = new List<object>();
			}
			if (!_changedObjects.Contains(obj))
			{
				_changedObjects.Add(obj);
			}
		}
		private void setViewerBackColor()
		{
			EnumDesignViewerStatus status = ViewerStatus;
			for (int i = 1; i < tabControl1.TabCount; i++)
			{
				TabPageActionGroup tpa = tabControl1.TabPages[i] as TabPageActionGroup;
				if (tpa != null)
				{
					tpa.Viewer.SetViewerBackColor(status);
				}
			}
			root.SetViewerBackColor(status);
		}
		private UserControlDebugger getDebugUIControl()
		{
			Control c = this.Parent;
			while (c != null)
			{
				UserControlDebugger u = c as UserControlDebugger;
				if (u != null)
				{
					return u;
				}
				c = c.Parent;
			}
			return null;
		}
		/// <summary>
		/// select a type for a parameter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addParameter(object sender, EventArgs e)
		{
			ConstructorClass cc = _method as ConstructorClass;
			if (cc != null)
			{
				ClassPointer cp = cc.Owner as ClassPointer;
				if (cp != null)
				{
					if (typeof(Attribute).IsAssignableFrom(cp.BaseClassType))
					{
						FrmObjectExplorer dlg = new FrmObjectExplorer();
						dlg.LoadAttributeParameterTypes(Method.RunAt);
						if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
						{
							ParameterClass pr = _method.AddNewParameter(dlg.SelectedDataType);
							pr.Owner = _method;
							pr.Method = _method;
							pr.NameChanged += new EventHandler(p_OnNameChange);
							pr.NameChanging += new EventHandler(p_OnNameChanging);
							pr.TypeChanged += new EventHandler(p_OnTypeChange);
							//
							listBoxParams.Items.Add(pr);
							AddNewParameterIcon(pr);
							//
							root.Changed = true;
						}
						return;
					}
				}
			}
			IObjectPointer dataType = _designer.SelectDataType(this.FindForm());
			if (dataType != null)
			{
				ParameterClass pr = _method.AddNewParameter(dataType);
				pr.Owner = _method;
				pr.Method = _method;
				pr.NameChanged += new EventHandler(p_OnNameChange);
				pr.NameChanging += new EventHandler(p_OnNameChanging);
				pr.TypeChanged += new EventHandler(p_OnTypeChange);
				//
				listBoxParams.Items.Add(pr);
				AddNewParameterIcon(pr);
				//
				root.Changed = true;
			}
		}

		void p_OnTypeChange(object sender, EventArgs e)
		{
			root.Changed = true;
		}
		void p_OnNameChanging(object sender, EventArgs e)
		{
			EventArgNameChange p = e as EventArgNameChange;
			if (p != null && p.OldName != p.NewName)
			{
				INameCreationService nc = (INameCreationService)root.DesignSurface.GetService(typeof(INameCreationService));
				if (!nc.IsValidName(p.NewName))
				{
					MessageBox.Show(string.Format("Invalid parameter name {0}", p.NewName));
					p.Cancel = true;
				}
				else
				{
					for (int i = 0; i < _method.ParameterCount; i++)
					{
						if (_method.Parameters[i].Name == p.NewName)
						{
							MessageBox.Show(string.Format("Name {0} in use", p.NewName));
							p.Cancel = true;
							break;
						}
					}
				}
			}
		}
		void onParameterNameChanged(ParameterClass pr)
		{
			foreach (Control c in iconsHolder.Controls)
			{
				ComponentIconParameter cip = c as ComponentIconParameter;
				if (cip != null)
				{
					ParameterClass p = cip.ClassPointer as ParameterClass;
					if (p != null)
					{
						if (p.ParameterID == pr.ParameterID)
						{
							cip.SetLabelText(pr.Name);
							break;
						}
					}
				}
			}
		}
		void p_OnNameChange(object sender, EventArgs e)
		{
			ParameterClass p = sender as ParameterClass;
			if (p != null)
			{
				for (int i = 0; i < listBoxParams.Items.Count; i++)
				{
					ParameterClass pr = listBoxParams.Items[i] as ParameterClass;
					if (pr != null)
					{
						if (pr.ParameterID == p.ParameterID)
						{
							listBoxParams.Items.RemoveAt(i);
							listBoxParams.Items.Insert(i, p);
							listBoxParams.SelectedIndex = i;
							onParameterNameChanged(pr);
							root.Changed = true;
							break;
						}
					}
				}
			}
			else
			{
				IAction act = sender as IAction;
				if (act != null)
				{
					NameBeforeChangeEventArg nc = e as NameBeforeChangeEventArg;
					if (nc != null)
					{
						if (DesignUtil.IsActionNameInUse(_designer.GetRootId().XmlData, nc.NewName, act.ActionId))
						{
							MessageBox.Show("The action name is in use", "Change action name");
							nc.Cancel = true;
							return;
						}
					}
					root.OnActionNameChanged(nc.NewName, act.WholeActionId);
				}
			}
		}
		private void changeParameterType(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				UInt32 id = (UInt32)mi.Tag;
				ParameterClass p = _method.GetParameterByID(id);
				if (p != null)
				{
					Type typeAttr = null;
					if (root.RootClass.IsWebPage)
					{
						if (_designer.Project.ProjectType == EnumProjectType.WebAppPhp)
						{
							if (Method.WebUsage == EnumMethodWebUsage.Server)
							{
								typeAttr = typeof(PhpTypeAttribute);
							}
							else
							{
								typeAttr = typeof(JsTypeAttribute);
							}
						}
						else if (_designer.Project.ProjectType == EnumProjectType.WebAppAspx)
						{
							if (Method.WebUsage == EnumMethodWebUsage.Client)
							{
								typeAttr = typeof(JsTypeAttribute);
							}
						}
					}
					DataTypePointer p1 = DesignUtil.SelectDataType(_designer.Project, this.RootClass, null, EnumObjectSelectType.InstanceType, p, null, typeAttr, this.FindForm());
					if (p1 != null)
					{
						p.SetDataType(p1);
						p_OnNameChange(p, EventArgs.Empty);
						root.Changed = true;
					}
				}
			}
		}
		private void deleteParameter(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				UInt32 id = (UInt32)mi.Tag;
				_method.RemoveParameter(id);
				RemoveParameterIcon(id);
				for (int i = 0; i < listBoxParams.Items.Count; i++)
				{
					ParameterClass p = listBoxParams.Items[i] as ParameterClass;
					if (p.ParameterID == id)
					{
						listBoxParams.Items.RemoveAt(i);
						if (i < listBoxParams.Items.Count)
						{
							listBoxParams.SelectedIndex = i;
						}
						else
						{
							listBoxParams.SelectedIndex = listBoxParams.Items.Count - 1;
						}
						root.Changed = true;
						break;
					}
				}
			}
		}

		protected virtual void LoadParameters()
		{
			btDown.Enabled = (_parameterEditorType == EnumParameterEditType.Edit);
			btUp.Enabled = (_parameterEditorType == EnumParameterEditType.Edit);
			listBoxParams.Items.Clear();
			EventHandlerMethod handler = _method as EventHandlerMethod;
			List<ParameterClass> ps = _method.Parameters;
			if (ps != null)
			{
				for (int i = 0; i < ps.Count; i++)
				{
					if (handler != null)
					{
						ps[i].ReadOnly = true;
						listBoxParams.Items.Add(ps[i]);
					}
					else
					{
						ps[i].AllowTypeChange = (_parameterEditorType == EnumParameterEditType.Edit);
						listBoxParams.Items.Add(ps[i]);
						if (!ps[i].ReadOnly)
						{
							ps[i].NameChanged += new EventHandler(p_OnNameChange);
							ps[i].NameChanging += new EventHandler(p_OnNameChanging);
							ps[i].TypeChanged += new EventHandler(p_OnTypeChange);
						}
					}
					//
				}
			}
			listBoxParams.FixedParameterCount = _fixedParameterCount;
		}
		#endregion
		#region design event handlers

		private void listBoxParams_MouseDown(object sender, MouseEventArgs e)
		{
			ParameterClass p = null;
			int n = listBoxParams.IndexFromPoint(e.X, e.Y);
			if (listBoxParams.SelectedIndex != n)
			{
				listBoxParams.SelectedIndex = n;
			}
			if (n >= 0 && n >= _fixedParameterCount)
			{
				p = listBoxParams.Items[n] as ParameterClass;
			}
			if (e.Button == MouseButtons.Right)
			{
				if (_parameterEditorType == EnumParameterEditType.Edit && Method != null && !Method.IsOverride)
				{
					ContextMenu mnu = new ContextMenu();
					MenuItem mi = new MenuItemWithBitmap("Add parameter", addParameter, Resources._newParam.ToBitmap());
					mnu.MenuItems.Add(mi);
					//
					if (p != null)
					{
						mi = new MenuItemWithBitmap("Delete parameter", deleteParameter, Resources._cancel.ToBitmap());
						mi.Tag = p.ParameterID;
						mnu.MenuItems.Add(mi);
						//
						mi = new MenuItemWithBitmap("Change parameter type", changeParameterType, Resources._class.ToBitmap());
						mi.Tag = p.ParameterID;
						mnu.MenuItems.Add(mi);
					}
					//
					mnu.Show(listBoxParams, new Point(e.X, e.Y));
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (this.Changed)
			{
				if (Save())
				{
					_changed = true;
					_method.Changed = true;
					_method.IsNewMethod = false;
				}
				else
				{
					return;
				}
			}
			Form f = this.FindForm();
			if (f != null)
			{
				f.DialogResult = DialogResult.OK;
			}
		}

		private void btInsert_Click(object sender, EventArgs e)
		{
			AddNewAction(new Point(5, 5));
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedComponents();
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			try
			{
				Undo();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btRedo_Click(object sender, EventArgs e)
		{
			try
			{
				Redo();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btCut_Click(object sender, EventArgs e)
		{
			try
			{
				copyToClipboard();
				DeleteSelectedComponents();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}

		private void btCopy_Click(object sender, EventArgs e)
		{
			try
			{
				copyToClipboard();
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}

		private void btPaste_Click(object sender, EventArgs e)
		{
			try
			{
				PasteFromClipboard();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}

		private void btUp_Click(object sender, EventArgs e)
		{
			int n = listBoxParams.SelectedIndex;
			if (n > 0 && n > _fixedParameterCount)
			{
				List<ParameterClass> ps = _method.Parameters;
				if (ps != null)
				{
					int n0 = n - 1;
					ParameterClass p = ps[n];
					ps[n] = ps[n0];
					ps[n0] = p;
					LoadParameters();
					listBoxParams.SelectedIndex = n0;
					root.Changed = true;
				}
			}
		}

		private void btDown_Click(object sender, EventArgs e)
		{
			int n = listBoxParams.SelectedIndex;
			if (n >= _fixedParameterCount)
			{
				int n0 = n + 1;
				if (n >= 0 && n0 < listBoxParams.Items.Count)
				{
					List<ParameterClass> ps = _method.Parameters;
					if (ps != null)
					{
						ParameterClass p = ps[n];
						ps[n] = ps[n0];
						ps[n0] = p;
						LoadParameters();
						listBoxParams.SelectedIndex = n0;
						root.Changed = true;
					}
				}
			}
		}
		private void listBoxParams_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = listBoxParams.SelectedIndex;
			if (n >= 0)
			{
				if (_parameterEditorType == EnumParameterEditType.Edit)
				{
					if (n >= _fixedParameterCount && n < listBoxParams.Items.Count - 1)
					{
						btDown.Enabled = true;
					}
					else
					{
						btDown.Enabled = false;
					}
					if (n > 0 && n > _fixedParameterCount)
					{
						btUp.Enabled = true;
					}
					else
					{
						btUp.Enabled = false;
					}
				}
				ParameterClass p = listBoxParams.Items[n] as ParameterClass;
				if (p != null)
				{
					if (n < _fixedParameterCount)
					{
						p.ReadOnly = true;
					}
					SetSelection(p);
				}
			}

		}
		#endregion
		#region Unit test
		private void btTest_Click(object sender, EventArgs e)
		{
			startTest(null);
		}
		private void startTest(IUnitTestOwner testOwner)
		{
			btTest.Enabled = false;
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			Save();
			thTest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(test));
			thTest.SetApartmentState(System.Threading.ApartmentState.STA);
			Type methodCreator = null;
			string mname = _method.Name;
			if (testOwner != null)
			{
				//create test method======================
				//create method name
				string[] methodNames;
				XmlNodeList methodNodes = _method.RootXmlNode.SelectNodes("Method");
				if (methodNodes != null && methodNodes.Count > 0)
				{
					methodNames = new string[methodNodes.Count];
					for (int i = 0; i < methodNodes.Count; i++)
					{
						methodNames[i] = XmlSerialization.GetAttribute(methodNodes[i], XmlSerialization.XMLATT_NAME);
					}
				}
				else
				{
					methodNames = new string[0];
				}
				bool bNameUsed = true;
				int nNum = 0;
				while (bNameUsed)
				{
					bNameUsed = false;
					nNum++;
					mname = "UnitTest" + nNum.ToString();
					for (int i = 0; i < methodNames.Length; i++)
					{
						if (mname == methodNames[i])
						{
							bNameUsed = true;
							break;
						}
					}
				}
				methodCreator = testOwner.GetType();
				XmlNode expNode = _method.RootXmlNode.OwnerDocument.CreateElement("UnitTest");
				XmlSerialization.WriteToXmlNode(_designer.Writer, testOwner.MathExpression, expNode);
				XmlSerialization.SetAttribute(expNode, "UnitTestName", mname);
				XmlSerialization.SetAttribute(expNode, "UnitTestCreator", methodCreator.AssemblyQualifiedName);
				XmlNode existUT = _method.RootXmlNode.SelectSingleNode("UnitTest");
				if (existUT != null)
				{
					_method.RootXmlNode.RemoveChild(existUT);
				}
				_method.RootXmlNode.AppendChild(expNode);
			}
			_testData = new MethodTestData(_method.RootXmlNode.OwnerDocument.OuterXml, "UnitTest"/*XmlSerialization.GetPathFromNode(_method.OwnerType.XmlData)*/, mname, methodCreator);

			//start unit test thread
			thTest.Start(_testData);
			timer1.Enabled = true;
		}
		/// <summary>
		/// domain, MethodTest, unloaded. signal it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ad2_DomainUnload(object sender, EventArgs e)
		{
			if (_testData != null)
			{
				_testData.Finished = true;
			}
		}
		/// <summary>
		/// start testing. it is from another thread
		/// </summary>
		/// <param name="data">data for re-constructing test objects</param>
		private void test(object data)
		{
			// Construct and initialize settings for a second AppDomain.
			AppDomainSetup ads = new AppDomainSetup();
			ads.ApplicationBase =
				"file:///" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			ads.DisallowBindingRedirects = false;
			ads.DisallowCodeDownload = true;
			ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			//
			// Create the second AppDomain.
			AppDomain ad2 = AppDomain.CreateDomain("MethodTest", null, ads);

			//signal the unloading of the unit test dialogue
			ad2.DomainUnload += new EventHandler(ad2_DomainUnload);
			// Create an instance of MarshalbyRefType in the second AppDomain. 
			// A proxy to the object is returned.
			MethodTest mtest = (MethodTest)ad2.CreateInstanceAndUnwrap(
					"XDesigner",
					typeof(MethodTest).FullName
				);

			try
			{
				//pass test data from this domain to the new domain
				mtest.Start((MethodTestData)data);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
			AppDomain.Unload(ad2);
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (_testData != null)
			{
				if (_testData.Finished)
				{
					thTest = null;
					timer1.Enabled = false;
					btTest.Enabled = true;
					this.Cursor = System.Windows.Forms.Cursors.Default;
				}
			}
			else
			{
				thTest = null;
				timer1.Enabled = false;
				btTest.Enabled = true;
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		public void SetLoading(bool loading)
		{
			bLoading = loading;
		}
		public void AbortTest()
		{
			if (thTest != null)
			{
				if (thTest.ThreadState != System.Threading.ThreadState.Unstarted && thTest.ThreadState != System.Threading.ThreadState.Aborted && thTest.ThreadState != System.Threading.ThreadState.AbortRequested && thTest.ThreadState != System.Threading.ThreadState.Stopped && thTest.ThreadState != System.Threading.ThreadState.StopRequested)
				{
					thTest.Abort();
				}
			}
		}
		#endregion
		#region Undo Engine
		public void ResetUndo()
		{
			_undoEngine.ClearStack();
		}
		[Browsable(false)]
		public bool HasUndo
		{
			get
			{
				return _undoEngine.HasUndo;
			}
		}
		[Browsable(false)]
		public bool HasRedo
		{
			get
			{
				return _undoEngine.HasRedo;
			}
		}
		public void Undo()
		{
			IUndoUnit obj = _undoEngine.UseUndo();
			if (obj != null)
			{
				obj.Apply();
				enableUndo();
			}
		}
		public void Redo()
		{
			IUndoUnit obj = _undoEngine.UseRedo();
			if (obj != null)
			{
				obj.Apply();
				enableUndo();
			}
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			if (!_disableUndo)
			{
				_undoEngine.AddUndoEntity(entity);
				enableUndo();
			}
		}
		public Control GetUndoControl(UInt32 key)
		{
			return root.GetUndoControl(key);
		}
		private void enableUndo()
		{
			if (!_disableUndo)
			{
				if (HasUndo)
				{
					btUndo.Enabled = true;
					btUndo.ImageIndex = IMG_UndoEnable;
				}
				else
				{
					btUndo.Enabled = false;
					btUndo.ImageIndex = IMG_UndoDisable;
				}
				if (HasRedo)
				{
					btRedo.Enabled = true;
					btRedo.ImageIndex = IMG_RedoEnable;
				}
				else
				{
					btRedo.Enabled = false;
					btRedo.ImageIndex = IMG_RedoDisable;
				}
			}
		}
		#endregion
		#region IObjectTypeUnitTester Members
		/// <summary>
		/// check to see if members are used. 
		/// if so then create unit test dialogue and return an ITestData object.
		/// if not then return null
		/// </summary>
		/// <param name="mathExp">the math expression to be tested</param>
		/// <returns>null: no test dialogue is created</returns>
		public ITestData UseMemberTest(IUnitTestOwner testOwner)
		{
			if (testOwner.MathExpression.ContainLibraryTypesOnly)
				return null;
			startTest(testOwner);
			return _testData;
		}

		#endregion
		#region IWithProject Members

		public LimnorProject Project
		{
			get
			{
				return _designer.Project;
			}
		}

		#endregion
	}
	/// <summary>
	/// for showing background color
	/// </summary>
	public enum EnumDesignViewerStatus { Inactive, Active, Finished }
}
