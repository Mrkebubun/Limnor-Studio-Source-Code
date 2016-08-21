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
using System.Collections.Specialized;
using MathExp;
using System.Xml;
using System.Drawing.Design;
using System.Collections;
using MathExp.RaisTypes;
using VPL;
using LimnorDesigner.Action;
using System.Windows.Forms.Design.Behavior;
using System.Collections.ObjectModel;
using System.Reflection;
using XmlUtility;
using LimnorDesigner.Property;
using LimnorDesigner.EventMap;
using ProgElements;
using VSPrj;
using LimnorDesigner.Event;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	[Designer(typeof(IRootDesigner))]
	[Designer(typeof(ComponentDesigner))]
	public partial class MethodDiagramViewer : Diagram, ICustomTypeDescriptor, IMessageReceiver, IMathDesigner, INameCreator, IWithProject, IMethodDiagram
	{
		#region Fields and constructor
		private DesignSurface dsf;
		private DesignMessageFilter _msgFilter;
		private List<UInt32> _actionBranchesLoaded;
		private bool _forSubMethod; //loop, inside the main scope
		private MethodDiagramViewer _parentEditor;
		private MethodDesignerHolder holder;
		private StringCollection propertyNames;
		private bool _nameReadOnly;
		private bool _attributesReadOnly;
		private bool _hideName;
		private bool _noCompound;
		private System.Drawing.SolidBrush _bkTextBrush;
		private System.Drawing.Font _bkTextFont;
		private DebugBehaviorService behavior;
		private Guid _guid;
		private Dictionary<UInt64, string> _actionNames;
		private Dictionary<UInt32, IAction> _changedActions;
		//
		private EventHandlerLinkPorts _handlePortsLinked;
		//
		public MethodDiagramViewer()
		{
			_guid = Guid.NewGuid();
			InitializeComponent();
			_bkTextBrush = new SolidBrush(Color.LightGray);
			_bkTextFont = new Font("Times New Roman", 16);
			gridImage = this.BackgroundImage;
			this.BackColor = Color.White;
			propertyNames = new StringCollection();
			propertyNames.Add("Name");
			propertyNames.Add("ReturnType");
			propertyNames.Add("ShowArrowAtLineBreaks");
			propertyNames.Add("Description");
			propertyNames.Add("ReturnType");
			//
			_handlePortsLinked = new EventHandlerLinkPorts(onPortsLinked);
			LinkLineNode.PortsLinked += _handlePortsLinked;
		}
		~MethodDiagramViewer()
		{
			if (IsMain)
			{
				LinkLineNode.PortsLinked -= _handlePortsLinked;
			}
		}
		#endregion
		#region Protected Methods

		/// <summary>
		/// multi-threads actions can be loaded
		/// </summary>
		/// <param name="actions">actions to be loaded. it may contain multi-thread actions</param>
		protected virtual void OnLoadActionList(BranchList actions)
		{
			if (actions != null)
			{
				actions.LoadToDesigner(this);
				actions.LinkJumpBranches();
			}
		}
		protected override void OnDisconnectLine(LinkLineNodeInPort port)
		{
			Changed = true;
			//remove the port if there is another inport
			if (port.PortOwner.InPortCount > 1)
			{
				port.ClearLine();
				Controls.Remove(port);
				Controls.Remove(port.Label);
				LinkLineNode l = (LinkLineNode)port.PrevNode;
				while (l != null)
				{
					Controls.Remove(l);
					l = (LinkLineNode)l.PrevNode;
				}
			}
		}
		#endregion
		#region Public Methods
		public void FixMissingPortLine()
		{
			foreach (Control c in this.Controls)
			{
				ActionViewer v = c as ActionViewer;
				if (v != null)
				{
					PortCollection ps = v.Ports;
					if (ps != null && ps.Count > 0)
					{
						for (int i = 0; i < ps.Count; i++)
						{
							LinkLineNodeInPort pi = ps[i] as LinkLineNodeInPort;
							if (pi != null)
							{
								if (pi.PrevNode == null && pi.Parent != null)
								{
									if (pi.LinkedPortID == 0 && pi.LinkedOutPort == null)
									{
										//add a line
										LinkLineNode prev = new LinkLineNode(pi, null);
										pi.SetPrevious(prev);
										prev.Location = new Point(pi.Location.X, pi.Location.Y - 30);
										pi.Parent.Controls.Add(prev);
										prev.CreateForwardLine();
									}
								}
							}
							else
							{
								LinkLineNodeOutPort po = ps[i] as LinkLineNodeOutPort;
								if (po != null)
								{
									if (po.NextNode == null && po.Parent != null)
									{
										if (po.LinkedPortID == 0 && po.LinkedInPort == null)
										{
											//add a line
											LinkLineNode next = new LinkLineNode(null, po);
											po.SetNext(next);
											next.Location = new Point(po.Location.X, po.Location.Y + 30);
											po.Parent.Controls.Add(next);
											next.CreateBackwardLine();
										}
									}
								}
							}
						}
					}
				}
			}
		}
		public ActionViewer GetLastAction(ActionViewer v0, Point loc)
		{
			ActionViewer vlast = null;
			double l0 = double.MaxValue;
			foreach (Control c in this.Controls)
			{
				ActionViewer v = c as ActionViewer;
				if (v != null && v != v0)
				{
					PortCollection ps = v.Ports;
					if (ps != null)
					{
						bool hasOut = false;
						LinkLineNode op0 = null;
						for (int i = 0; i < ps.Count; i++)
						{
							LinkLineNodeOutPort op = ps[i] as LinkLineNodeOutPort;
							if (op != null)
							{
								if (op.LinkedInPort != null)
								{
									hasOut = false;
									break;
								}
								else
								{
									if (op.End != null)
									{
										op0 = op.End;
										hasOut = true;
										break;
									}
								}
							}
						}
						if (hasOut)
						{
							if (vlast == null)
							{
								vlast = v;
								l0 = (op0.Location.X - loc.X) * (op0.Location.X - loc.X) + (op0.Location.Y - loc.Y) * (op0.Location.Y - loc.Y);
							}
							else
							{
								double l1 = (op0.Location.X - loc.X) * (op0.Location.X - loc.X) + (op0.Location.Y - loc.Y) * (op0.Location.Y - loc.Y);
								if (l1 < l0)
								{
									vlast = v;
									l0 = l1;
								}
							}
						}
					}
				}
			}
			return vlast;
		}
		public void LinkNewAction(ActionViewer v, Point loc)
		{
			if (v != null)
			{
				LinkLineNodeInPort ip = null;
				PortCollection ps = v.Ports;
				if (ps != null)
				{
					for (int i = 0; i < ps.Count; i++)
					{
						ip = ps[i] as LinkLineNodeInPort;
						if (ip != null)
						{
							break;
						}
					}
				}
				if (ip != null && ip.Start != null)
				{
					ActionViewer last;
					last = GetLastAction(v, loc);
					if (last != null)
					{
						v.Left = last.Left;
						v.Top = last.Top + last.Height + 30;
						ps = last.Ports;
						LinkLineNodeOutPort op = null;
						for (int i = 0; i < ps.Count; i++)
						{
							op = ps[i] as LinkLineNodeOutPort;
							if (op != null)
							{
								break;
							}
						}
						if (op != null && op.End != null)
						{
							LinkLineNode.JoinToEnd(op.End, ip.Start);
						}
					}
				}
			}
		}
		public void AddAssignActionsAction(IEvent ep)
		{
			ClassPointer root = this.RootClass;
			Type ehType;
			if (this.Project.IsWebApplication)
			{
				if (this.Method.RunAt == EnumWebRunAt.Client)
				{
					ehType = typeof(WebClientEventHandlerMethodClientActions);
				}
				else
				{
					ehType = typeof(WebClientEventHandlerMethodServerActions);
				}
			}
			else
			{
				ehType = typeof(EventHandlerMethod);
			}
			AB_AssignActions abaa = new AB_AssignActions(Method);
			EventHandlerMethod eh = root.AddEventhandlerMethod(ehType, ep, abaa.BranchId, this.ClientRectangle, this.FindForm());
			if (eh != null)
			{
				abaa.Name = eh.Name;
				eh.ActionBranchId = abaa.BranchId;
				double x0, y0;
				ComponentIconEvent.CreateRandomPoint((double)((this.Width - 20) / 2), out x0, out y0);
				if (x0 < 0) x0 = 10;
				if (y0 < 0) y0 = 10;
				ActionViewer av = MethodViewer.AddNewAction(abaa, new Point((this.Width - 20) / 2 + (int)x0, (this.Height - 20) / 2 + (int)y0));
				if (av.Parent == null)
				{
#if DEBUG
					MessageBox.Show("Adding action viewer failed (1)");
#endif
					this.Controls.Add(av);
				}
			}
		}
		public bool LoadAction(ActionBranch av)
		{
			if (_actionBranchesLoaded == null)
			{
				_actionBranchesLoaded = new List<uint>();
			}
			if (!_actionBranchesLoaded.Contains(av.BranchId))
			{
				_actionBranchesLoaded.Add(av.BranchId);
				IDesignerHost host = (IDesignerHost)DesignSurface.GetService(typeof(IDesignerHost));
				ActionViewer v = (ActionViewer)host.CreateComponent(av.ViewerType, "A" + Guid.NewGuid().GetHashCode().ToString("x"));
				Controls.Add(v);
				List<UInt32> used = new List<uint>();
				av.SetOwnerMethod(used, Method);
				v.ImportAction(av, Method.IsNewMethod);
				return true;
			}
			else
			{
#if DEBUG
				DesignUtil.WriteToOutputWindow("Action Branch {0} already loaded", av.BranchId);
#endif
				return false;
			}
		}
		public void OnLocalVariableRename(LocalVariable lv)
		{
			foreach (Control c in Controls)
			{
				ActionViewerSingleAction av = c as ActionViewerSingleAction;
				if (av != null)
				{
					av.OnLocalVariableRename(lv);
				}
			}
		}
		public void SetForSubScope()
		{
		}
		public void SetAttributesReadOnly(bool readOnly)
		{
			_attributesReadOnly = readOnly;
		}
		public void SetForSubMethod(MethodDiagramViewer parentEditor)
		{
			_forSubMethod = true;
			_parentEditor = parentEditor;
		}
		public void GetCurrentActionNames(StringCollection sc)
		{
			List<ComponentIcon> cis = IconList;
			if (cis != null)
			{
				foreach (ComponentIcon ci in cis)
				{
					if (!sc.Contains(ci.ClassPointer.Name))
					{
						sc.Add(ci.ClassPointer.Name);
					}
				}
			}
			foreach (Control c in Controls)
			{
				ActionViewer av = c as ActionViewer;
				if (av != null)
				{
					if (!sc.Contains(av.ActionName))
					{
						sc.Add(av.ActionName);
					}
				}
			}
		}
		public void ClearIconSelection()
		{
			holder.ClearIconSelection();
		}

		public void SetViewerBackColor(EnumDesignViewerStatus status)
		{
			switch (status)
			{
				case EnumDesignViewerStatus.Inactive:
					BackColor = Color.White;
					_bkTextBrush = new SolidBrush(Color.LightGray);
					break;
				case EnumDesignViewerStatus.Active:
					BackColor = Color.LightYellow;
					_bkTextBrush = new SolidBrush(Color.LightGray);
					break;
				case EnumDesignViewerStatus.Finished:
					BackColor = Color.LightGray;
					_bkTextBrush = new SolidBrush(Color.White);
					break;
			}
			Refresh();
		}
		/// <summary>
		/// load actions into designer and make link lines.
		/// </summary>
		/// <param name="actions">actions to be loaded.</param>
		public void LoadActions(BranchList actions)
		{
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			//
			OnLoadActionList(actions);
			//
			PortCollection pc = new PortCollection();
			foreach (Control c in Controls)
			{
				LinkLineNodePort p = c as LinkLineNodePort;
				if (p != null)
				{
					pc.Add(p);
				}
			}
			pc.CreateLinkLines();
			Refresh();
			holder.DisableUndo = b;
		}
		public virtual List<ComponentIcon> IconList
		{
			get
			{
				return holder.Method.ComponentIconList;
			}
		}
		public ILimnorDesignerLoader Loader
		{
			get
			{
				return holder.Loader;
			}
		}
		public void ValidateControlPositions()
		{
			foreach (Control c in Controls)
			{
				if (c.Left < 0)
				{
					c.Left = 1;
				}
				else if (c.Left > Screen.PrimaryScreen.Bounds.Width)
				{
					c.Left = this.ClientSize.Width - c.Width;
				}
				if (c.Top < 0)
				{
					c.Top = 1;
				}
				else if (c.Top > Screen.PrimaryScreen.Bounds.Height)
				{
					c.Top = this.ClientSize.Height - c.Height;
				}
			}
		}
		public bool LocalVariableDeclared(LocalVariable lv)
		{
			List<ComponentIcon> iconList = IconList;
			foreach (ComponentIcon ci in iconList)
			{
				ComponentIconLocal cil = ci as ComponentIconLocal;
				if (cil != null)
				{
					if (lv.IsSameObjectRef(cil.LocalPointer))
					{
						lv.ScopeGroupId = cil.ScopeGroupId;
						return true;
					}
				}
			}
			return false;
		}
		public bool LocalVariableDeclared(ComponentIconLocal lv)
		{
			List<ComponentIcon> iconList = IconList;
			foreach (ComponentIcon ci in iconList)
			{
				ComponentIconLocal cil = ci as ComponentIconLocal;
				if (cil != null)
				{
					if (lv.LocalPointer.IsSameObjectRef(cil.LocalPointer))
					{
						lv.ScopeGroupId = cil.ScopeGroupId;
						return true;
					}
				}
			}
			return false;
		}
		public void ReloadActions(BranchList actions)
		{
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			holder.ClearAllComponent();
			Controls.Clear();
			LoadActions(actions);

			//load component icons
			List<ComponentIcon> iconList = IconList;
			List<ComponentIcon> iconInvalid = new List<ComponentIcon>();
			//find root
			ClassPointer root = holder.Designer.GetRootId();
			List<IClass> objList;
			if (Method.IsStatic)
			{
				objList = new List<IClass>();
			}
			else
			{
				objList = root.GetClassList();
			}
			//initialize existing icons, creating ComponentIcon.ClassPointer
			foreach (ComponentIcon ic in iconList)
			{
				bool bValid = false;
				if (Method.IsStatic)
				{
					ComponentIconPublic cip = ic as ComponentIconPublic;
					if (cip == null)
					{
						bValid = true;
					}
				}
				else
				{
					if (ic.ClassPointer != null && ic.ClassPointer.IsValid)
					{
						bValid = true;
					}
				}
				if (bValid)
				{
					ic.SetDesigner(holder.Designer);
				}
				else
				{
					iconInvalid.Add(ic);
				}
			}
			foreach (ComponentIcon ic in iconInvalid)
			{
				iconList.Remove(ic);
			}
			//add new icons
			int x0 = 10;
			int y0 = 30;
			int x = x0;
			int y = y0;
			int dx = 30;
			int dy = 30;
			foreach (IClass c in objList)
			{
				bool bFound = false;
				foreach (ComponentIcon ic in iconList)
				{
					if (ic.ClassPointer.IsSameObjectRef(c))
					{
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					ComponentIconPublic cip = new ComponentIconPublic(holder.Designer, c, holder.Method);
					cip.Location = new Point(x, y);
					x += dx;
					x += cip.Width;
					if (x >= this.Width)
					{
						x = x0;
						y += dy;
						y += cip.Height;
					}
					iconList.Add(cip);
				}
			}
			//
			if (Method.ParameterCount > 0)
			{
				foreach (ParameterClass c in Method.Parameters)
				{
					bool bFound = false;
					foreach (ComponentIcon ic in iconList)
					{
						if (ic.ClassPointer.IsSameObjectRef(c))
						{
							EventHandlerMethod ehm = Method as EventHandlerMethod;
							if (ehm != null)
							{
								ParameterClass pc = ic.ClassPointer as ParameterClass;
								if (pc != null)
								{
									if (string.CompareOrdinal(c.Name, "sender") == 0 && string.CompareOrdinal(pc.Name, "sender") == 0)
									{
										pc.SetDataType(c.ObjectType);
									}
									else if (string.CompareOrdinal(c.Name, "e") == 0 && string.CompareOrdinal(pc.Name, "e") == 0)
									{
										ICustomEventMethodDescriptor ce = ehm.Event.Owner.ObjectInstance as ICustomEventMethodDescriptor;
										if (ce != null)
										{
											Type pType = ce.GetEventArgumentType(ehm.Event.Name);
											if (pType != null)
											{
												pc.SetDataType(pType);
											}
										}
									}
								}
							}
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						ComponentIconParameter cip = new ComponentIconParameter(holder.Designer, c, holder.Method);
						cip.Location = new Point(x, y);
						x += dx;
						x += cip.Width;
						if (x >= this.Width)
						{
							x = x0;
							y += dy;
							y += cip.Height;
						}
						iconList.Add(cip);
					}
				}
			}
			//add icons
			holder.AddControlsToIconsHolder(iconList.ToArray());
			foreach (ComponentIcon ic in iconList)
			{
				if (ic.Left < 0)
					ic.Left = 2;
				if (ic.Top < 0)
					ic.Top = 2;
				ic.BringToFront();
				ic.RefreshLabelPosition();
				ic.RefreshLabelText();
				ComponentIconLocal cil = ic as ComponentIconLocal;
				if (cil != null)
				{
					cil.HookNameChecker();
					LocalVariable v = cil.ClassPointer as LocalVariable;
					if (v != null)
					{
						v.SetTypeChangeNotify(onLocalVariabletypeChanged);
					}
				}
			}
			InitializeInputTypes();
			ValidateControlPositions();
			holder.DisableUndo = b;
		}
		protected void InitializeInputTypes()
		{
			for (int i = 0; i < this.Controls.Count; i++)
			{
				ActionViewer av = this.Controls[i] as ActionViewer;
				if (av != null)
				{
					av.InitializeInput();
				}
			}
		}
		public BranchList ExportActions()
		{
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			//
			holder.SaveIconsLocations();
			//
			//starting branch for each thread
			List<ActionBranch> startingBranches = new List<ActionBranch>();
			//merging points where more than one thread meet
			List<ActionBranch> linkedBranches = new List<ActionBranch>();
			for (int i = 0; i < this.Controls.Count; i++)
			{
				ActiveDrawing ad = this.Controls[i] as ActiveDrawing;
				if (ad != null)
				{
					ad.SaveLocation();
				}
				ActionViewer av = this.Controls[i] as ActionViewer;
				if (av != null)
				{
					av.ExportAction(); //save properties from action-viewer to ActionBranch
					if (av.ActionObject.IsStartingPoint)
					{
						startingBranches.Add(av.ActionObject);
					}
					else
					{
						if (av.ActionObject.IsMergingPoint || av.ActionObject.IsBranchingPoint)
						{
							linkedBranches.Add(av.ActionObject);
						}
					}
				}
			}
			Dictionary<UInt32, ActionBranch> collected = new Dictionary<uint, ActionBranch>();
			//create ActionString for single-linked actions
			BranchList startingBranches2 = new BranchList(ActionsHolder);
			foreach (ActionBranch ab in startingBranches)
			{
				startingBranches2.Add(ab.CollectActions(null, collected));
			}
			List<ActionBranch> linkedBranches2 = new List<ActionBranch>();
			foreach (ActionBranch ab in linkedBranches)
			{
				ActionBranch a;
				if (!collected.TryGetValue(ab.FirstActionId, out a))
				{
					a = ab.CollectActions(null, collected);
				}
				linkedBranches2.Add(a);
			}
			startingBranches2.AddRange(linkedBranches2);
			holder.DisableUndo = b;
			startingBranches2.OnExport();
			startingBranches2.SetOwnerMethod(holder.Method);
			return startingBranches2;
		}
		public BranchList ExportSelectedActions()
		{
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			MethodClass mc = (MethodClass)holder.Method.Clone();
			List<ActionBranch> startingBranches = new List<ActionBranch>();
			List<ActionBranch> linkedBranches = new List<ActionBranch>();
			List<ActionBranch> actions = new List<ActionBranch>();
			for (int i = 0; i < this.Controls.Count; i++)
			{
				ActiveDrawing ad = this.Controls[i] as ActiveDrawing;
				if (ad != null)
				{
					ad.SaveLocation();
				}
			}
			List<ActionViewer> selectedActions = holder.GetSelectedActions();
			foreach (ActionViewer av in selectedActions)
			{
				av.ActionObject.Location = av.Location;
				av.ActionObject.Size = av.Size;
				actions.Add(av.ActionObject);
			}
			List<UInt32> used = new List<uint>();
			foreach (ActionBranch a in actions)
			{
				a.SetOwnerMethod(used, mc);
			}
			foreach (ActionBranch a in actions)
			{
				List<ActionPortIn> ports = a.InPortList;
				bool bIsStart = a.IsStartingPoint;
				if (!bIsStart)
				{
					foreach (ActionPortIn p in ports)
					{
						if (p.LinkedOutPort != null)
						{
							bool bLinked = false;
							ActionViewer av = p.LinkedOutPort.Owner as ActionViewer;
							if (av != null && selectedActions.Contains(av))
							{
								bLinked = true;
							}
							if (!bLinked)
							{
								bIsStart = true;
							}
						}
						else
						{
							bIsStart = true;
						}
					}
				}
				if (bIsStart)
				{
					a.IsStartingPointByScope = true;
					startingBranches.Add(a);
				}
				else
				{
					a.IsStartingPointByScope = false;
					if (a.IsMergingPoint)
					{
						linkedBranches.Add(a);//every ActionBranch has its own unique BranchId. Linking is done via BranchId
					}
				}
			}
			Dictionary<UInt32, ActionBranch> collected = new Dictionary<uint, ActionBranch>();
			//create ActionString for single-linked actions
			BranchList startingBranches2 = new BranchList(ActionsHolder);
			foreach (ActionBranch ab in startingBranches)
			{
				startingBranches2.Add(ab.CollectActions(selectedActions, collected));
			}
			List<ActionBranch> linkedBranches2 = new List<ActionBranch>();
			foreach (ActionBranch ab in linkedBranches)
			{
				linkedBranches2.Add(ab.CollectActions(selectedActions, collected));
			}
			startingBranches2.AddRange(linkedBranches2);
			startingBranches2.SetOwnerMethod(mc);
			BranchList branches = (BranchList)startingBranches2.Clone();
			branches.ClearLinkPortIDs();
			branches.SetOwnerMethod(mc);//holder.Method);
			holder.DisableUndo = b;
			return branches;
		}
		public ClassPointer RootClass
		{
			get
			{
				return holder.ActionEventCollection;
			}
		}

		public virtual void LoadMethod()
		{
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			MethodClass method = holder.Method;
			if (method is GetterClass || method is SetterClass)
			{
				this.Name = method.MethodName;
			}
			this.Name = method.Name;
			this.Site.Name = Name;
			Description = method.Description;
			//
			method.EstablishObjectOwnership();
			//
			//reset actions to remove PropertyChanged handlers
			ClassPointer av = holder.ActionEventCollection;
			av.LoadActionInstances();
			//
			method.LoadActionInstances();
			if (method.ActionList != null)
			{
				Dictionary<UInt32, IAction> acts = av.GetActions();
				method.SetActions(acts);
			}
			//
			ReloadActions(method.ActionList);
			//
			RemoveDisconnectedPorts();
			//
			FixMissingPortLine();
			//
			holder.DisableUndo = b;
			//
		}
		/// <summary>
		/// if an action has more than one inport then check and remove any disconnected inports 
		/// until only one inport left or only connected inports left
		/// </summary>
		protected void RemoveDisconnectedPorts()
		{
			foreach (Control c in Controls)
			{
				ActionViewer av = c as ActionViewer;
				if (av != null && av.ActionObject != null && av.ActionObject.InPortList != null)
				{
					if (av.ActionObject.InPortList.Count > 1)
					{
						bool bHasLink = false;
						for (int i = 0; i < av.ActionObject.InPortList.Count; i++)
						{
							if (av.ActionObject.InPortList[i].LinkedOutPort != null)
							{
								bHasLink = true;
								break;
							}
						}
						List<ActionPortIn> disconnected = new List<ActionPortIn>();
						if (bHasLink)
						{
							for (int i = 0; i < av.ActionObject.InPortList.Count; i++)
							{
								if (av.ActionObject.InPortList[i].LinkedOutPort == null)
								{
									disconnected.Add(av.ActionObject.InPortList[i]);
								}
							}
						}
						else
						{
							for (int i = 1; i < av.ActionObject.InPortList.Count; i++)
							{
								disconnected.Add(av.ActionObject.InPortList[i]);
							}
						}
						if (disconnected.Count > 0)
						{
							foreach (ActionPortIn pi in disconnected)
							{
								av.ActionObject.InPortList.Remove(pi);
								LinkLineNode l = pi;
								l.ClearLine();
								Controls.Remove(pi);
								Controls.Remove(pi.Label);
								l = (LinkLineNode)l.PrevNode;
								while (l != null)
								{
									Controls.Remove(l);
									l = (LinkLineNode)l.PrevNode;
								}
							}
						}
					}
				}
			}
		}
		public virtual void CancelEdit()
		{
		}

		/// <summary>
		/// construction of action branches:
		/// 1. identify starting branches; each starting branch is one thread
		/// 2. identidfy non-starting (linked) branches: a)outport number >1; or b) inport is linked to more than one outport
		/// </summary>
		public virtual bool Save()
		{
			bool hasAct = false;
			ClassPointer root = holder.Loader.GetRootId();
			try
			{
				if (!ReadOnly)
				{
					MethodClass method = holder.Method;
					method.Name = this.Site.Name;
					method.Description = this.Description;
					if (method.MemberId == 0)
					{
						method.MemberId = (UInt32)Guid.NewGuid().GetHashCode();
					}
					//
					method.ActionList = ExportActions();
					List<IAction> actList = new List<IAction>();
					foreach (IAction a in ActionsHolder.ActionInstances.Values)
					{
						actList.Add(a);
					}
					hasAct = (actList.Count > 0);
					root.InBatchSaving = true;
					for (int k = 0; k < actList.Count; k++)
					{
						IAction a = actList[k];
						if (a.ScopeMethodId == 0)
						{
							//check if a method parameter is used
							bool bIsLocal = false;
							if (a.ParameterCount > 0 && a.ParameterValues != null)
							{
								for (int i = 0; i < a.ParameterCount; i++)
								{
									if (a.ParameterValues[i] != null)
									{
										if (a.ParameterValues[i].ValueType == EnumValueType.Property)
										{
											if (a.ParameterValues[i].Property is ParameterClass)
											{
												bIsLocal = true;
												break;
											}
										}
										else if (a.ParameterValues[i].ValueType == EnumValueType.MathExpression)
										{
											if (a.ParameterValues[i].MathExpression != null)
											{
											}
										}
									}
								}
							}
							if (bIsLocal)
							{
								//make a copy of the action.
								UInt32 oldId = a.ActionId;
								UInt32 newId = (UInt32)Guid.NewGuid().GetHashCode();
								IAction a1 = (IAction)a.Clone();
								a1.AsLocal = true;
								a1.ActionId = newId;
								a1.ScopeMethod = method;
								a1.ActionHolder = this.ActionsHolder;
								actList[k] = a1;
								method.ActionList.ReplaceAction(oldId, a1);
							}
						}
						if (_actionNames != null)
						{
							string newName;
							if (_actionNames.TryGetValue(actList[k].WholeActionId, out newName))
							{
								actList[k].ActionName = newName;
							}
						}
						root.SaveAction(actList[k], holder.Loader.Writer);
					}
					//
					Form f = this.FindForm();
					if (f != null)
					{
						method.EditorBounds = f.Bounds;
					}
				}
				return true;
			}
			catch (Exception e)
			{
				MathNode.Log(this.FindForm(), e);
				return false;
			}
			finally
			{
				root.InBatchSaving = false;
			}
		}
		public void AssignHolder(MethodDesignerHolder h, DesignSurface surf)
		{
			holder = h;
			dsf = surf;
		}
		public ISelectionService GetSelectionService()
		{
			return (ISelectionService)dsf.GetService(typeof(ISelectionService));
		}
		public void OnActivate()
		{
			if (_msgFilter == null)
			{
				_msgFilter = new DesignMessageFilter(dsf);
			}
			Application.AddMessageFilter(_msgFilter);
		}
		public void OnDeactivate()
		{
			if (_msgFilter != null)
			{
				Application.RemoveMessageFilter(_msgFilter);
			}
		}
		public void OnClosing()
		{
			if (_msgFilter != null)
			{
				Application.RemoveMessageFilter(_msgFilter);
				_msgFilter = null;
			}
		}
		public void CreateParameterIcon(Parameter p)
		{
			this.Controls.AddRange(p.GetControls());
			p.InitLocation();
		}
		public ClassPointer ActionEventCollection
		{
			get
			{
				return holder.ActionEventCollection;
			}
		}
		public DesignSurface DesignSurface
		{
			get
			{
				return dsf;
			}
		}
		public MethodClass Method
		{
			get
			{
				if (holder != null)
				{
					return holder.Method;
				}
				return null;
			}
		}
		public void UpdateBreakpoint(ActionBranch branch)
		{
			foreach (Control c in Controls)
			{
				ActionViewer av = c as ActionViewer;
				if (av != null)
				{
					if (av.ActiveDrawingID != branch.BranchId)
					{
						av.ClearBreakpointStatus();
					}
					else
					{
						av.SetBreakpointStatus(branch.AtBreak);
					}
				}
			}
			this.Refresh();
		}
		public void ClearBreakpoint()
		{
			foreach (Control c in Controls)
			{
				ActionViewer av = c as ActionViewer;
				if (av != null)
				{
					av.ClearBreakpointStatus();
				}
			}
			this.Refresh();
		}
		public void SetDebugBehavior()
		{
			if (behavior == null)
			{
				BehaviorService svc = GetService(typeof(BehaviorService)) as BehaviorService;
				behavior = new DebugBehaviorService();
				if (svc != null)
				{
					svc.PushBehavior(behavior);
					svc.BeginDrag += new BehaviorDragDropEventHandler(svc_BeginDrag);
					svc.EndDrag += new BehaviorDragDropEventHandler(svc_EndDrag);
				}
			}
		}
		public ActionViewer AddNewAction(IAction item, Point pos)
		{
			ActionViewer av = holder.AddNewAction(item, pos);
			av.Location = pos;
			if (av.Parent == null)
			{
#if DEBUG
				MessageBox.Show("Adding action viewer failed");
#endif
				Controls.Add(av);
			}
			else
			{
			}
			return av;
		}
		void svc_EndDrag(object sender, BehaviorDragDropEventArgs e)
		{
			System.Windows.Forms.Design.Behavior.BehaviorService bh = sender as System.Windows.Forms.Design.Behavior.BehaviorService;
			if (bh != null)
			{
				DebugBehaviorService bs = bh.CurrentBehavior as DebugBehaviorService;
				if (bs != null)
				{
					bs.ComponentsDraged = null;
				}
			}
		}

		void svc_BeginDrag(object sender, BehaviorDragDropEventArgs e)
		{
			System.Windows.Forms.Design.Behavior.BehaviorService bh = sender as System.Windows.Forms.Design.Behavior.BehaviorService;
			if (bh != null)
			{
				DebugBehaviorService bs = bh.CurrentBehavior as DebugBehaviorService;
				if (bs != null)
				{
					bs.ComponentsDraged = e.DragComponents;
				}
			}
		}
		#endregion
		#region Properties
		protected virtual bool IsMain
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public virtual IActionsHolder ActionsHolder
		{
			get
			{
				return holder.ActionsHolder;
			}
		}
		[Browsable(false)]
		public MethodDiagramViewer ParentEditor
		{
			get
			{
				return _parentEditor;
			}
		}
		[Browsable(false)]
		public Dictionary<UInt32, IAction> ChangedActions
		{
			get
			{
				if (_changedActions == null)
				{
					_changedActions = new Dictionary<uint, IAction>();
				}
				return _changedActions;
			}
		}
		public Guid GUID
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
		}
		public string BKTEXT { get; set; }
		public bool ReadOnly
		{
			get
			{
				return holder.ContentReadOnly;
			}
		}
		public MethodDesignerHolder DesignerHolder
		{
			get
			{
				return holder;
			}
		}
		protected override IUndoHost UndoHost
		{
			get
			{
				return holder;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool ForLoop
		{
			get
			{
				return _forSubMethod;
			}
		}
		[MethodReturn]
		[Description("The type of the method return")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		public IObjectPointer ReturnType
		{
			get
			{
				return holder.Method.ReturnValue.DataType;
			}
			set
			{
				if (value != null)
				{
					holder.Method.ReturnValue.SetDataType(value);
					onReturnTypeChanged();
					Changed = true;
				}
			}
		}
		public bool ShowArrowAtLineBreaks
		{
			get
			{
				return LinkLine.ShowArrows;
			}
			set
			{
				if (LinkLine.ShowArrows != value)
				{
					LinkLine.ShowArrows = value;
					Refresh();
				}
			}
		}

		private string _desc;
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}
		Image gridImage;
		public bool ShowGrid
		{
			get
			{
				return (this.BackgroundImage != null);
			}
			set
			{
				if (value)
				{
					this.BackgroundImage = gridImage;
				}
				else
				{
					this.BackgroundImage = null;
				}
			}
		}
		private ObjectIconData _iconData;
		public ObjectIconData IconImage
		{
			get
			{
				if (_iconData == null)
					_iconData = new ObjectIconData();
				return _iconData;
			}
			set
			{
				_iconData = value;
			}
		}
		[Browsable(false)]
		public bool NameReadOnly
		{
			get
			{
				return _nameReadOnly;
			}
			set
			{
				_nameReadOnly = value;
			}
		}
		[Browsable(false)]
		public bool HideName
		{
			get
			{
				return _hideName;
			}
			set
			{
				_hideName = value;
			}
		}
		[Browsable(false)]
		public Dictionary<UInt64, string> NewActionNames
		{
			get
			{
				return _actionNames;
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					if (string.CompareOrdinal(oProp.Name, "Name") == 0)
					{
						if (!_hideName)
						{
							int n;
							Attribute[] ats;
							if (Method is GetterClass || Method is SetterClass)
							{
								n = 0;
								if (attributes != null)
								{
									n = attributes.Length;
									ats = new Attribute[n + 1];
									attributes.CopyTo(ats, 0);
								}
								else
								{
									n = 0;
									ats = new Attribute[n + 1];
								}
								ats[n] = new ParenthesizePropertyNameAttribute(true);
								newProps.Add(new PropertyDescriptorForDisplay(typeof(SetterClass), "Name", Method.MethodName, ats));
							}
							else
							{
								if (_attributesReadOnly || _nameReadOnly || Method == null || Method.IsOverride)
								{
									newProps.Add(new ReadOnlyPropertyDesc(oProp));
								}
								else
								{
									newProps.Add(oProp);
								}
							}
						}
					}
					else if (string.CompareOrdinal(oProp.Name, "ReturnType") == 0)
					{
						if (_attributesReadOnly)
						{
							PropertyDescriptorForDisplay pdfd = new PropertyDescriptorForDisplay(oProp.ComponentType, oProp.Name, Method.ReturnValue.TypeName, attributes);
							newProps.Add(pdfd);
						}
						else
						{
							if (holder != null && holder.Method != null)
							{
								if (Method.IsOverride)
								{
									PropertyDescriptorForDisplay pdfd = new PropertyDescriptorForDisplay(oProp.ComponentType, oProp.Name, Method.ReturnValue.TypeName, attributes);
									newProps.Add(pdfd);
								}
								else if (!holder.Method.NoReturn)
								{
									bool bR = false;
									GetterClass gc = holder.Method as GetterClass;
									if (gc != null)
									{
										bR = true;
									}
									else
									{
										SetterClass sc = holder.Method as SetterClass;
										if (sc != null)
										{
											bR = true;
										}
									}
									if (bR)
									{
										object v = oProp.GetValue(this);
										string s;
										if (v == null)
											s = "";
										else
											s = v.ToString();
										PropertyDescriptorForDisplay p = new PropertyDescriptorForDisplay(oProp.ComponentType, oProp.Name, s, attributes);
										newProps.Add(p);
									}
									else
									{
										newProps.Add(oProp);
									}
								}
							}
						}
					}
					else
					{
						if (string.CompareOrdinal(oProp.Name, "Description") == 0 || string.CompareOrdinal(oProp.Name, "ShowArrowAtLineBreaks") == 0)
						{
							newProps.Add(oProp);
						}
						else
						{
							if (_attributesReadOnly)
							{
								object v = oProp.GetValue(this);
								string s = v == null ? "" : v.ToString();
								PropertyDescriptorForDisplay p = new PropertyDescriptorForDisplay(oProp.ComponentType, oProp.Name, s, attributes);
								newProps.Add(p);
							}
							else
							{
								newProps.Add(oProp);
							}
						}
					}
				}
			}
			return newProps;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;

		}

		#endregion
		#region IMessageReceiver members
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			holder.OnDesignerSelected();
			OnMouseDown(e);
			return false;
		}


		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseMove(e);
			return false;
		}
		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseUp(e);

			return false;
		}
		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDoubleClick(e);
			return false;
		}
		public bool FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			return false;
		}
		public bool FireKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
			return false;
		}
		#endregion
		#region Design events
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyData == Keys.Delete)
			{
				ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				if (selectionService != null && selectionService.SelectionCount > 0)
				{
					ICollection sels = selectionService.GetSelectedComponents();
					List<IComponent> avs = new List<IComponent>();
					foreach (object v in sels)
					{
						ActionViewer a = v as ActionViewer;
						if (a != null)
						{
							avs.Add(a);
						}
					}
					if (avs.Count > 0)
					{
						IComponent[] cs = new IComponent[avs.Count];
						avs.CopyTo(cs);
						DeleteComponents(cs);
					}
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!string.IsNullOrEmpty(BKTEXT))
			{
				string s = BKTEXT + " thread " + this.holder.ThreadId.ToString();
				System.Drawing.Drawing2D.GraphicsState gstate = e.Graphics.Save();
				SizeF sf = e.Graphics.MeasureString(s, _bkTextFont);
				e.Graphics.RotateTransform(90);
				e.Graphics.DrawString(s, _bkTextFont, _bkTextBrush, (float)3, (float)(-3) - sf.Height);
				e.Graphics.Restore(gstate);
			}
			base.OnPaint(e);
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point pt)
		{
			base.OnCreateContextMenu(mnu, pt);
			MenuItem mi;
			if (this.ReadOnly)
			{
				if (holder != null && holder.ActionGroup.GroupFinished)
				{
					mi = new MenuItemWithBitmap("Close", Resources.close);
					mi.Click += new EventHandler(miClose_Click);
					mi.Tag = pt;
					mnu.MenuItems.Add(mi);
				}
			}
			else
			{
				//
				mi = new MenuItemWithBitmap("Add local variable", miNewObject_Click, Resources._newVar.ToBitmap());
				mi.Tag = new Point(5, 5); //pt;
				//mi.
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Add an action", miNewAction_Click, Resources._newAction.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Create condition", miNewCondition_Click, Resources._condition.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Execute actions repeatedly", miNewLoop_Click, Resources._loop.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Execute actions a number of times", miNewForLoop_Click, Resources._loop.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Execute actions for all items", miNewForEachLoop_Click, Resources._loop.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Create decision-table", miDecisionTable_Click, Resources._decisionTable.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Create action-list", miActionList_Click, Resources._actLst.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				mi = new MenuItemWithBitmap("Add method exit", miNewMethodReturn_Click, Resources._door.ToBitmap());
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
				//
				if (ForLoop)
				{
					mi = new MenuItemWithBitmap("Add break", miNewBreak_Click, Resources._loopBreak.ToBitmap());
					mi.Tag = pt;
					mnu.MenuItems.Add(mi);
				}
				//
				if (holder != null && holder.HasClipboardData())
				{
					mi = new MenuItemWithBitmap("Paste", miPaste_Click, Resources.paste);
					mi.Tag = pt;
					mnu.MenuItems.Add(mi);
				}
			}
		}

		private StringCollection getNames()
		{
			StringCollection sc = new StringCollection();
			MethodClass.GetActionNamesInEditors(sc, Method.MemberId);
			return sc;
		}

		/// <summary>
		/// add a new local vriable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNewObject_Click(object sender, EventArgs e)
		{
			Point loc = (Point)(((MenuItem)sender).Tag);
			CreateLocalVariable(loc);
		}
		public ComponentIconLocal CreateLocalVariable(DataTypePointer type, Point location, bool createConstructor)
		{
			ComponentIconLocal c = null;
			Changed = true;
			ClassPointer root = holder.Loader.GetRootId();
			StringCollection names = getNames();
			string name = holder.AskNewName(type.BaseName, names);
			type.Owner = root;
			LocalVariable v = type.CreateVariable(name, holder.Loader.ClassID, (UInt32)Guid.NewGuid().GetHashCode());
			v.Owner = root;
			LocalVariable.SaveLocalVariable(v);
			//
			//create constructor action
			ActionClass act = new ActionClass(root);
			act.ActionId = (UInt32)Guid.NewGuid().GetHashCode();
			act.ActionName = "Create " + name;
			act.ScopeMethodId = holder.Method.MethodID;
			act.ActionHolder = this.ActionsHolder;
			bool bOK = createConstructor;
			bool bCreateComponent = false;
			if (createConstructor)
			{
				dlgConstructorParameters dlg = new dlgConstructorParameters();
				dlg.SetMethod(this.DesignerHolder.Method);
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
							dlg.Ret.SetHolder(root);
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
					holder.Loader.GetRootId().SaveAction(act, holder.Loader.Writer);
					double x0, y0;
					int x2 = this.ClientSize.Width / 4;
					int y2 = this.ClientSize.Height / 4;
					ComponentIconEvent.CreateRandomPoint(Math.Min(x2, y2), out x0, out y0);
					holder.AddNewAction(act, new Point((int)x0 + this.ClientSize.Width / 2, (int)y0 + this.ClientSize.Height / 2));
				}
			}
			else
			{
				bCreateComponent = true;
			}
			if (bCreateComponent)
			{
				v.ScopeGroupId = holder.ActionGroup.GroupId;
				holder.PrepareNewIconLocation(location);
				c = v.CreateComponentIcon(holder.Designer, Method);
				c.Location = location;
				holder.AddControlToIconsHolder(c);
				holder.ActionGroup.ComponentIconList.Add(c);
				c.Visible = true;
				c.BringToFront2();
				c.ScopeGroupId = holder.ActionGroup.GroupId;
				v.SetTypeChangeNotify(onLocalVariabletypeChanged);
			}
			return c;
		}
		public ComponentIconLocal CreateLocalVariable(Point loc)
		{
			Type typeAttr = null;
			if (this.RootClass.IsWebObject)
			{
				if (holder.Project.ProjectType == EnumProjectType.WebAppPhp)
				{
					if (Method.RunAt == EnumWebRunAt.Server)
					{
						typeAttr = typeof(PhpTypeAttribute);
					}
					else
					{
						typeAttr = typeof(JsTypeAttribute);
					}
				}
				else if (holder.Project.ProjectType == EnumProjectType.WebAppAspx)
				{
					if (Method.RunAt == EnumWebRunAt.Client)
					{
						typeAttr = typeof(JsTypeAttribute);
					}
				}
			}
			DataTypePointer dtp = DesignUtil.SelectDataType(holder.Project, this.RootClass, null, EnumObjectSelectType.InstanceType, null, null, typeAttr, this.FindForm());
			if (dtp != null)
			{
				if (dtp.IsStatic)
				{
					MessageBox.Show(this.FindForm(), "A static class cannot be used to create a variable. You may use a static class as if it is an instance. A static class only has static members.", "Create variable", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					return CreateLocalVariable(dtp, loc, true);
				}
			}
			return null;
		}
		public void onLocalVariabletypeChanged(object sender, EventArgs e)
		{
			ArrayVariable av = sender as ArrayVariable;
			if (av != null)
			{
				foreach (Control c in Controls)
				{
					ActionViewerConstructor avc = c as ActionViewerConstructor;
					if (avc != null)
					{
						AB_Constructor abc = avc.ActionObject as AB_Constructor;
						if (abc != null)
						{
							ActionClass ac = abc.ActionData as ActionClass;
							if (ac != null)
							{
								ConstructorPointer cp = ac.ActionMethod as ConstructorPointer;
								if (cp != null)
								{
									if (cp.ReturnReceiver != null && cp.ReturnReceiver.MemberId == av.MemberId)
									{
										ConstructorInfo cif = av.ObjectType.GetConstructor(cp.ParameterTypes);
										if (cif != null)
										{
											cp.SetMethodInfo(cif);
										}
										break;
									}
								}
							}
						}
					}
				}
			}
		}
		private void miClose_Click(object sender, EventArgs e)
		{
			if (holder != null)
			{
				holder.UnloadFromSplitContainer();
			}
		}
		private void miNewBreak_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewBreak(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewMethodReturn_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewMethodReturn(pt);
					this.Changed = true;
				}
			}
		}
		private void miActionList_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewActionList(pt);
					this.Changed = true;
				}
			}
		}
		private void miDecisionTable_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewDecisionTable(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewForEachLoop_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewForEachLoop(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewForLoop_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewForLoop(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewLoop_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewLoop(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewCondition_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.LoadNewCondition(pt);
					this.Changed = true;
				}
			}
		}
		private void miNewAction_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					try
					{
						holder.AddNewAction(pt);
					}
					catch (Exception err)
					{
						DesignUtil.WriteToOutputWindowAndLog(err, "Error adding action.");
					}
				}
			}
		}
		private void miPaste_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				//Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.PasteFromClipboard();
					this.Changed = true;
				}
			}
		}

		/// <summary>
		/// initialize the input data type and default values
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void onPortsLinked(object sender, EventArgcLinkPorts e)
		{
			if (e.InPort != null && e.OutPort != null)
			{
				ActionViewer avIn = e.InPort.Owner as ActionViewer;
				ActionViewer avOut = e.OutPort.Owner as ActionViewer;
				if (avIn != null && avOut != null)
				{
					if (!string.IsNullOrEmpty(avOut.ActionObject.OutputCodeName) && avOut.ActionObject.OutputType != null && !avOut.ActionObject.OutputType.IsVoid)
					{
						bool bIn = false;
						bool bOut = false;
						for (int i = 0; i < Controls.Count; i++)
						{
							if (avIn == Controls[i])
							{
								bIn = true;
							}
							if (avOut == Controls[i])
							{
								bOut = true;
							}
							if (bIn && bOut)
							{
								break;
							}
						}
						if (bIn && bOut)
						{
							avIn.ActionObject.InputName = avOut.ActionObject.OutputCodeName;
							avIn.ActionObject.InputType = avOut.ActionObject.OutputType;
							avIn.ActionObject.SetInputName(avOut.ActionObject.OutputCodeName, avOut.ActionObject.OutputType);
							avIn.CreateDefaultInputUsage();
						}
					}
				}
			}
		}
		private void onReturnTypeChanged()
		{
			foreach (Control c in Controls)
			{
				ActionViewerSingleAction av = c as ActionViewerSingleAction;
				if (av != null)
				{
					AB_MethodReturn abr = av.ActionObject as AB_MethodReturn;
					if (abr != null)
					{
						MethodReturnMethod mrm = abr.ActionData.ActionMethod as MethodReturnMethod;
						if (mrm != null)
						{
							mrm.SetValueDataType(holder.Method.ReturnValue);
							abr.ActionData.ResetScopeMethod();
						}
					}
				}
			}
		}
		#endregion
		#region IMathDesigner Members
		[Browsable(false)]
		public bool NoCompoundCreation
		{
			get
			{
				return _noCompound;
			}
			set
			{
				_noCompound = value;
			}
		}
		public void ShowProperties(object v)
		{
			PropertyGrid pr = (PropertyGrid)this.GetService(typeof(PropertyGrid));
			if (pr != null)
			{
				pr.SelectedObject = v;
			}
		}
		/// <summary>
		/// create an instance of Actions from selected actions
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		public void CreateCompound(Point pos, Size size)
		{
		}
		public void CreateActionGroup(List<ActionViewer> selectedActions)
		{
			ActionViewer lastAction = null;
			ActionViewer firstAction = null;
			foreach (ActionViewer av in selectedActions)
			{
				ActionBranch sab = av.ActionObject;
				bool isTheFirst = true;
				for (int k = 0; k < sab.InPortList.Count; k++)
				{
					if (sab.InPortList[k].LinkedOutPort != null)
					{
						foreach (ActionViewer av2 in selectedActions)
						{
							for (int i = 0; i < av2.ActionObject.OutPortList.Count; i++)
							{
								if (av2.ActionObject.OutPortList[i].PortID == sab.InPortList[k].LinkedOutPort.PortID)
								{
									isTheFirst = false;
									break;
								}
							}
							if (!isTheFirst)
							{
								break;
							}
						}
					}
					if (!isTheFirst)
					{
						break;
					}
				}
				if (isTheFirst)
				{
					if (firstAction != null && firstAction != av)
					{
						MessageBox.Show(this.FindForm(), "More than one beginning action is selected. Only actions with single beginning action and single ending action can be included in an action group.", "Make Action Group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					firstAction = av;
				}
				bool isTheLast = true;
				for (int k = 0; k < sab.OutPortList.Count; k++)
				{
					if (sab.OutPortList[k].LinkedInPort != null)
					{
						foreach (ActionViewer av2 in selectedActions)
						{
							for (int i = 0; i < av2.ActionObject.InPortList.Count; i++)
							{
								if (av2.ActionObject.InPortList[i].PortID == sab.OutPortList[k].LinkedInPort.PortID)
								{
									isTheLast = false;
									break;
								}
							}
							if (!isTheLast)
							{
								break;
							}
						}
					}
					if (!isTheLast)
					{
						break;
					}
				}
				if (isTheLast)
				{
					if (lastAction != null && lastAction != av)
					{
						MessageBox.Show(this.FindForm(), "More than one ending action is selected. Only actions with single beginning action and single ending action can be included in an action group.", "Make Action Group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					lastAction = av;
				}
			}
			//must have one entry point
			if (firstAction == null || lastAction == null)
			{
				MessageBox.Show(this.FindForm(), "Circular execution path is selected. Such execution path cannot be included in an action group", "Make Action Group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (lastAction.ActionObject.OutPortList.Count > 1)
			{
				MessageBox.Show(this.FindForm(), "The last action has more than one out port. It cannot be used as the last action an action group.", "Make Action Group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			//create ordered action list
			Point pos = firstAction.Location;
			//remember the linked input and output ports
			List<ActionPortIn> inports = firstAction.ActionObject.InPortList;
			List<ActionPortOut> outports = lastAction.ActionObject.OutPortList;
			List<IPortOwner> linkedOutportOwners = new List<IPortOwner>();
			List<LinkLineNodeOutPort> linkedOutports = new List<LinkLineNodeOutPort>();
			for (int k = 0; k < firstAction.ActionObject.InPortList.Count; k++)
			{
				if (firstAction.ActionObject.InPortList[k].LinkedOutPort != null)
				{
					linkedOutports.Add(firstAction.ActionObject.InPortList[k].LinkedOutPort);
					linkedOutportOwners.Add(firstAction.ActionObject.InPortList[k].LinkedOutPort.PortOwner);
				}
			}
			List<LinkLineNodeInPort> linkedInports = new List<LinkLineNodeInPort>();
			for (int k = 0; k < lastAction.ActionObject.OutPortList.Count; k++)
			{
				if (lastAction.ActionObject.OutPortList[k].LinkedInPort != null)
				{
					linkedInports.Add(lastAction.ActionObject.OutPortList[k].LinkedInPort);
				}
			}
			//delete selected actions
			foreach (ActionViewer av in selectedActions)
			{
				av.KeepRemovedAction = true;
				DeleteComponent(av);
			}
			//create a new action list
			ActionViewer newAv = holder.addNewActionGroup(selectedActions, pos);
			//reset port id
			UInt32 inportId = (UInt32)(Guid.NewGuid().GetHashCode());
			UInt32 outportId = (UInt32)(Guid.NewGuid().GetHashCode());
			foreach (ActionPortIn ai in newAv.ActionObject.InPortList)
			{
				ai.SetInstanceId(inportId, (UInt32)(Guid.NewGuid().GetHashCode()));
			}
			foreach (ActionPortOut ai in newAv.ActionObject.OutPortList)
			{
				ai.SetInstanceId(outportId, (UInt32)(Guid.NewGuid().GetHashCode()));
			}
			// outport->next->next->...->inport
			if (linkedOutportOwners.Count > 0)
			{
				int n = 0;
				foreach (LinkLineNodeOutPort linkedOutport in linkedOutports)
				{
					LinkLineNode end = linkedOutport.End;
					if (n >= newAv.ActionObject.InPortList.Count)
					{
						ActionPortIn ai = new ActionPortIn(newAv.ActionObject);
						ai.SetInstanceId(inportId, (UInt32)(Guid.NewGuid().GetHashCode()));
						newAv.ActionObject.AddInPort(ai);
					}
					else
					{
					}
					LinkLineNode start = newAv.ActionObject.InPortList[n].Start;
					LinkLineNode.JoinToEnd(start, end);
					n++;
				}
			}
			if (linkedInports.Count > 0)
			{
				foreach (LinkLineNodeInPort linkedInport in linkedInports)
				{
					LinkLineNode start = newAv.ActionObject.OutPortList[0].End;
					LinkLineNode end = linkedInport.Start;
					LinkLineNode.JoinToEnd(start, end);
				}
			}
			//remove existing links
			for (int i = 0; i < firstAction.ActionObject.InPortList.Count; i++)
			{
				firstAction.ActionObject.InPortList[i].LinkedPortID = 0;
				firstAction.ActionObject.InPortList[i].LinkedPortInstanceID = 0;
				firstAction.ActionObject.InPortList[i].SetPrevious(null);
			}
			for (int i = 0; i < lastAction.ActionObject.OutPortList.Count; i++)
			{
				lastAction.ActionObject.OutPortList[i].LinkedPortID = 0;
				lastAction.ActionObject.OutPortList[i].LinkedPortInstanceID = 0;
				lastAction.ActionObject.OutPortList[i].SetNext(null);
			}
			//adjust out port position
			if (newAv.ActionObject.OutportCount == 1)
			{
				if (newAv.ActionObject.OutPortList[0].LinkedPortID == 0)
				{
					if (newAv.ActionObject.OutPortList[0].NextNode != null)
					{
						newAv.ActionObject.OutPortList[0].NextNode.Left = newAv.ActionObject.OutPortList[0].Left;
						newAv.ActionObject.OutPortList[0].NextNode.Top = newAv.ActionObject.OutPortList[0].Top + 30;
					}
				}
			}
			//adjust in port position
			if (newAv.ActionObject.InPortCount == 1)
			{
				if (newAv.ActionObject.InPortList[0].LinkedPortID == 0)
				{
					if (newAv.ActionObject.InPortList[0].PrevNode != null)
					{
						newAv.ActionObject.InPortList[0].PrevNode.Left = newAv.ActionObject.InPortList[0].Left;
						if (newAv.ActionObject.InPortList[0].Top < 0)
						{
							newAv.ActionObject.InPortList[0].Top = 0;
						}
						int top = newAv.ActionObject.InPortList[0].Top - 30;
						if (top < 0) top = 0;
						if (newAv.ActionObject.InPortList[0].Top > 30)
						{
							newAv.ActionObject.InPortList[0].PrevNode.Top = top;
						}
					}
				}
			}
			//
			FixMissingPortLine();
			//refresh UI
			Refresh();
			//
			holder.OpenActionGroup(newAv.ActionObject as AB_Group);
		}
		/// <summary>
		/// make actions into one single action list
		/// </summary>
		/// <param name="selectedActions"></param>
		public void CreateActionList(List<ActionViewer> selectedActions)
		{
			List<IAction> al = new List<IAction>();
			List<ActionViewer> avList = new List<ActionViewer>();
			ActionViewer firstAction = null;
			//make sure the actions are singlely linked actions
			foreach (ActionViewer av in selectedActions)
			{
				//must be a simple action
				ActionViewerSingleAction sa = av as ActionViewerSingleAction;

				if (sa == null && !(av is ActionViewerActionList))
				{
					MessageBox.Show(this.FindForm(), av.GetType().Name + " cannot be included in an action list. Only simple actions and action lists can be included in an action list", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				else
				{
					if (av.ActionObject == null)
					{
						MessageBox.Show(this.FindForm(), av.ActionName + " cannot be included in an action list. This action is invalid and should be deleted and re-created", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					AB_SingleAction sab = av.ActionObject as AB_SingleAction;
					if (sab == null && !(av.ActionObject is AB_ActionList))
					{
						MessageBox.Show(this.FindForm(), av.ActionObject.GetType().Name + " cannot be included in an action list. Only simple actions and action lists can be included in an action list", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					//must be singlely-linked
					if (av.ActionObject.InPortList.Count > 1 || av.ActionObject.OutportCount > 1)
					{
						MessageBox.Show(this.FindForm(), av.ActionName + " cannot be included in an action list. Only actions with single execution path can be included in an action list", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					avList.Add(av);
				}
			}
			//only one entry point is allowed
			foreach (ActionViewer av in avList)
			{
				ActionBranch sab = av.ActionObject;
				bool isTheFirst = true;
				if (sab.InPortList[0].LinkedOutPort != null)
				{
					foreach (ActionViewer av2 in avList)
					{
						if (av2.ActionObject.OutPortList[0].PortID == sab.InPortList[0].LinkedOutPort.PortID)
						{
							isTheFirst = false;
							break;
						}
					}
				}
				if (isTheFirst)
				{
					if (firstAction != null)
					{
						MessageBox.Show(this.FindForm(), "More than one execution path is selected. Only actions with single execution path can be included in an action list", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					firstAction = av;
				}
			}
			//must have one entry point
			if (firstAction == null)
			{
				MessageBox.Show(this.FindForm(), "Circular execution path is selected. Only actions with single execution path can be included in an action list", "Make Action List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			//create ordered action list
			Point pos = firstAction.Location;
			ActionViewer lastAction = null;
			ActionViewer a = firstAction;
			while (true)
			{
				lastAction = a;
				AB_SingleAction si = a.ActionObject as AB_SingleAction;
				if (si != null)
				{
					al.Add(si.ActionData);
				}
				else
				{
					AB_ActionList aa = a.ActionObject as AB_ActionList;
					if (aa != null)
					{
						ActionList aal = aa.Actions;
						if (aal != null)
						{
							foreach (ActionItem ai in aal)
							{
								if (ai.Action != null)
								{
									al.Add(ai.Action);
								}
							}
						}
					}
				}
				if (a.ActionObject.OutPortList[0].LinkedInPort == null)
				{
					break;
				}
				ActionViewer aNext = null;
				foreach (ActionViewer av in avList)
				{
					if (av.ActionObject.InPortList[0].PortID == a.ActionObject.OutPortList[0].LinkedInPort.PortID)
					{
						aNext = av;
						break;
					}
				}
				if (aNext == null)
				{
					break;
				}
				a = aNext;
			}
			//remember the linked input and output ports
			IPortOwner linkedOutportOwner = null;
			LinkLineNodeOutPort linkedOutport = firstAction.ActionObject.InPortList[0].LinkedOutPort;
			LinkLineNodeInPort linkedInport = lastAction.ActionObject.OutPortList[0].LinkedInPort;
			if (firstAction.ActionObject.InPortList[0].LinkedOutPort != null)
			{
				linkedOutportOwner = firstAction.ActionObject.InPortList[0].LinkedOutPort.PortOwner;
			}

			//delete selected actions
			foreach (ActionViewer av in avList)
			{
				av.KeepRemovedAction = true;
				DeleteComponent(av);
			}
			//create a new action list
			ActionViewer newAv = holder.addNewActionList(al, pos);
			//link the ports
			// outport->next->next->...->inport
			if (linkedOutportOwner != null)
			{
				LinkLineNode end = linkedOutport.End;
				LinkLineNode start = newAv.ActionObject.InPortList[0].Start;
				LinkLineNode.JoinToEnd(start, end);
			}
			if (linkedInport != null)
			{
				LinkLineNode end = newAv.ActionObject.OutPortList[0].End;
				LinkLineNode start = linkedInport.Start;
				LinkLineNode.JoinToEnd(start, end);
			}
			//refresh UI
			Refresh();
		}
		public void DeleteComponent(IComponent c)
		{
			holder.DeleteComponent(c);
		}
		public void DeleteSelectedComponents()
		{
			holder.DeleteSelectedComponents();
		}
		public bool IsNameUsed(string name)
		{
			return holder.IsNameUsed(name);
		}
		public void ReSelectComponent(IComponent c)
		{
			ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SetSelectedComponents(null);
				Application.DoEvents();
				selectionService.SetSelectedComponents(new IComponent[] { c });
			}
		}
		public void DeleteComponents(IComponent[] components)
		{
			ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SetSelectedComponents(null);
				Application.DoEvents();
				selectionService.SetSelectedComponents(components);
				Application.DoEvents();
				holder.DeleteSelectedComponents();
			}
		}
		private bool _changed;
		[Browsable(false)]
		public bool Changed
		{
			get
			{
				return _changed;
			}
			set
			{
				_changed = value;
			}
		}
		List<EventHandler> onShowHandlers;
		public void AddOnShowHandler(EventHandler handler)
		{
			if (onShowHandlers == null)
			{
				onShowHandlers = new List<EventHandler>();
			}
			onShowHandlers.Add(handler);
		}
		public void ExecuteOnShowHandlers()
		{
			if (onShowHandlers != null)
			{
				bool b = Changed;
				foreach (EventHandler handler in onShowHandlers)
				{
					handler(this, null);
				}
				onShowHandlers = null;
				Changed = b;
			}
		}
		public object GetDesignerService(Type serviceType)
		{
			if (serviceType.Equals(typeof(ILimnorDesigner)))
				return holder.Designer;
			if (serviceType.Equals(typeof(ObjectRef)))
				return holder.MethodOwnerRef;
			if (_services != null)
				if (_services.ContainsKey(serviceType))
					return _services[serviceType];
			return this.GetService(serviceType);

		}
		public void CreateValue(RaisDataType type, LinkLineNodeInPort inPort, Point pos)
		{
		}
		private Hashtable _services;
		public void AddDesignerService(Type serviceType, object service)
		{
			if (_services == null)
				_services = new Hashtable();
			if (_services.ContainsKey(serviceType))
				_services[serviceType] = service;
			else
				_services.Add(serviceType, service);
		}
		public bool TestDisabled
		{
			get { return holder.TestDisabled; }
		}
		#endregion
		#region IUseXType Members

		public IObjectPointer OwnerXtype
		{
			get
			{
				return holder.MethodOwner;
			}
		}
		public ILimnorDesigner XTypeDesigner
		{
			get
			{
				return holder.Designer;
			}
		}
		#endregion
		#region Undo Engine
		public Control GetUndoControl(UInt32 key)
		{
			foreach (Control c in Controls)
			{
				IActiveDrawing ia = c as IActiveDrawing;
				if (ia != null)
				{
					if (ia.ActiveDrawingID == key)
					{
						return c;
					}
				}
			}
			return null;
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			holder.AddUndoEntity(entity);
		}
		public bool DisableUndo
		{
			get
			{
				return holder.DisableUndo;
			}
			set
			{
				holder.DisableUndo = value;
			}
		}
		#endregion
		#region INameCreator Members

		public string CreateNewName(string baseName)
		{
			return holder.CreateName(baseName, getNames());
		}

		#endregion
		#region IWithProject Members

		public VSPrj.LimnorProject Project
		{
			get
			{
				return holder.Project;
			}
		}

		#endregion
		#region IMethodDiagram Members

		public XmlNode RootXmlNode
		{
			get { return holder.RootXmlNode; }
		}
		public void OnActionNameChanged(string newActionName, UInt64 WholeActionId)
		{
			for (int i = 0; i < this.Controls.Count; i++)
			{
				ActionViewerSingleAction av = this.Controls[i] as ActionViewerSingleAction;
				if (av != null)
				{
					ISingleAction avl = av.ActionObject as ISingleAction;
					if (avl != null)
					{
						if (avl.ActionId.WholeTaskId == WholeActionId)
						{
							av.SetActionName(newActionName);
							av.ResetDisplay();
							av.Refresh();
							this.Refresh();
							break;
						}
					}
				}
			}
			if (_actionNames == null)
				_actionNames = new Dictionary<ulong, string>();
			if (_actionNames.ContainsKey(WholeActionId))
				_actionNames[WholeActionId] = newActionName;
			else
				_actionNames.Add(WholeActionId, newActionName);
		}
		#endregion
		#region IMethodDesignerHolder Members

		public MethodDesignerHolder MethodViewer
		{
			get { return this.DesignerHolder; }
		}

		#endregion
	}
}
