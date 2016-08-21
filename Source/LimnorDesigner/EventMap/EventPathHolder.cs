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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;
using LimnorDesigner.Event;
using MathExp;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Property;
using LimnorDesigner.Web;
using VPL;
using LimnorDatabase;
using WindowsUtility;
using System.ComponentModel.Design;

namespace LimnorDesigner.EventMap
{
	[Description("Event Path graphically displays event-action-executers relationship, showing control-flow of your program. It manages action-creation and action-assignments.")]
	public partial class EventPathHolder : UserControl, IXDesignerViewer
	{
		#region fields and constructors
		private EventPath _eventPath;
		private MultiPanes _panes;
		private UInt32 _classId;
		private Guid _prjId;
		private bool _loading;
		const string XMLATT_Width = "width";
		const string XMLATT_Height = "height";
		const string EGSCR = "EasyGridCols";
		public EventPathHolder()
		{
			InitializeComponent();
		}
		#endregion
		#region properties
		public MultiPanes Panes { get { return _panes; } }
		#endregion
		#region Methods
		public bool CacheGridColumns(EasyGrid eg)
		{
			return _eventPath.CacheGridColumns(eg);
		}
		public bool RestoreGridColumns(EasyGrid eg)
		{
			return _eventPath.RestoreGridColumns(eg);
		}
		#endregion
		#region IXDesignerViewer Members
		public void OnClosing()
		{
			if (_eventPath != null)
			{
				XmlNode epNode = _eventPath.XmlData;
				if (epNode != null)
				{
					XmlUtil.SetAttribute(epNode, XMLATT_Width, _eventPath.Size.Width);
					XmlUtil.SetAttribute(epNode, XMLATT_Height, _eventPath.Size.Height);
				}
			}
		}
		public void OnHtmlElementSelected(HtmlElement_Base element)
		{
			_eventPath.OnHtmlElementSelected(element);
		}
		public void OnSelectedHtmlElement(Guid guid, object selector)
		{
			_eventPath.OnSelectedHtmlElement(guid, selector);
		}
		//an html element is used. guid and id should have been created
		public void OnUseHtmlElement(HtmlElement_BodyBase element)
		{
			_eventPath.OnUseHtmlElement(element);
		}
		public void OnResetDesigner(object obj)
		{
			_eventPath.OnResetDesigner(obj);
		}
		public void OnRemoveAllEventHandlers(IEvent e)
		{
			if (_eventPath.SelectedLinkLine != null)
			{
				EventPortOut po = _eventPath.SelectedLinkLine.StartNode as EventPortOut;
				if (po != null)
				{
					if (e.IsSameObjectRef(po.Event))
					{
						listBoxActions.Items.Clear();
						_eventPath.ClearLineSelection();
					}
				}
			}
			else if (_eventPath.SelectedEvent != null)
			{
				if (e.IsSameObjectRef(_eventPath.SelectedEvent.Event))
				{
					listBoxActions.Items.Clear();
					_eventPath.ClearEventIconSelection();
				}
			}
			_eventPath.OnRemoveAllEventHandlers(e);
		}
		public void SetDesigner(MultiPanes mp)
		{
			_panes = mp;
			_classId = mp.Loader.ClassID;
			_prjId = mp.Loader.Project.ProjectGuid;
			_eventPath = new EventPath();
			//
			_eventPath.SetDesignerEx(mp, this);
			_eventPath.Dock = DockStyle.None;//.Fill;
			//
			int width = 0, height = 0;
			XmlNode epNode = _eventPath.XmlData;
			if (epNode != null)
			{
				width = XmlUtil.GetAttributeInt(epNode, XMLATT_Width);
				height = XmlUtil.GetAttributeInt(epNode, XMLATT_Height);
			}
			if (width < splitContainer1.Panel1.Width)
			{
				width = splitContainer1.Panel1.Width * 2;
			}
			if (height < splitContainer1.Panel1.Height)
			{
				height = splitContainer1.Panel1.Height * 2;
			}
			_eventPath.Size = new Size(width, height);
			splitContainer1.Panel1.Controls.Add(_eventPath);
			Dictionary<UInt32, EventPathHolder> eps;
			EventMapService lst = VPLUtil.GetServiceByName(EGSCR) as EventMapService;
			if (lst == null)
			{
				lst = new EventMapService();
				VPLUtil.SetServiceByName(EGSCR, lst);
			}
			if (!lst.TryGetValue(_prjId, out eps))
			{
				eps = new Dictionary<uint, EventPathHolder>();
				lst.Add(_prjId, eps);
			}
			if (eps.ContainsKey(_classId))
			{
				eps[_classId] = this;
			}
			else
			{
				eps.Add(_classId, this);
			}
		}

		public void OnLineSelectionChanged(EventPortOut port)
		{
			listBoxActions.Items.Clear();
			List<TaskID> lst = port.Actions;
			if (lst != null && lst.Count > 0)
			{
				foreach (TaskID a in lst)
				{
					listBoxActions.Items.Add(a);
				}
			}
		}
		/// <summary>
		/// icon selected by mouse, called from EventPath
		/// </summary>
		/// <param name="ci"></param>
		public void OnComponentIconSelection(ComponentIcon ci)
		{
			if (!_loading)
			{
				listBoxActions.Items.Clear();
				ClassPointer root = _panes.Loader.GetRootId();
				List<EventAction> eas = root.EventHandlers;
				if (eas != null)
				{
					ComponentIconEventhandle h = ci as ComponentIconEventhandle;
					if (h != null)
					{
						foreach (EventAction ea in eas)
						{
							if (ea.TaskIDList != null)
							{
								bool b = false;
								foreach (TaskID tid in ea.TaskIDList)
								{
									HandlerMethodID hid = tid as HandlerMethodID;
									if (hid != null)
									{
										if (hid.ActionId == h.MethodId)
										{
											listBoxActions.Items.Add(tid);
											b = true;
											break;
										}
									}
								}
								if (b)
								{
									break;
								}
							}
						}
					}
					else
					{
						Dictionary<UInt32, IAction> acts = root.GetActions();
						if (acts != null)
						{
							ComponentIconMethod cim = ci as ComponentIconMethod;
							if (cim != null)
							{
								foreach (IAction a in acts.Values)
								{
									if (a != null && a.IsPublic)
									{
										CustomMethodPointer cmp = a.ActionMethod as CustomMethodPointer;
										if (cmp != null)
										{
											if (cmp.MemberId == cim.MethodId)
											{
												listBoxActions.Items.Add(a);
											}
										}
									}
								}
							}
							else
							{
								ComponentIconProperty cip = ci as ComponentIconProperty;
								if (cip != null)
								{
									foreach (IAction a in acts.Values)
									{
										if (a != null && a.IsPublic)
										{
											if (cip.Property.IsActionOwner(a))
											{
												listBoxActions.Items.Add(a);
											}
										}
									}
								}
								else
								{
									ComponentIconClass cic = ci as ComponentIconClass;
									if (cic != null)
									{
										foreach (IAction a in acts.Values)
										{
											if (a != null && a.IsPublic)
											{
												if (cic.IsActionExecuter(a, root))
												{
													listBoxActions.Items.Add(a);
												}
											}
										}
									}
									else
									{
										ComponentIconClassType cict = ci as ComponentIconClassType;
										if (cict != null)
										{
											foreach (IAction a in acts.Values)
											{
												if (a != null && a.IsPublic)
												{
													if (cict.IsActionExecuter(a, root))
													{
														listBoxActions.Items.Add(a);
													}
												}
											}
										}
										else
										{
											ComponentIconEvent cie = ci as ComponentIconEvent;
											if (cie != null && cie.IsForComponent)
											{
												foreach (IAction a in acts.Values)
												{
													if (a != null && a.IsPublic)
													{
														if (cie.IsActionExecuter(a, root))
														{
															listBoxActions.Items.Add(a);
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
				}
			}
		}
		public void OnEventIconSelection(EventIcon ei)
		{
			listBoxActions.Items.Clear();
			ClassPointer root = _panes.Loader.GetRootId();
			List<EventAction> eas = root.EventHandlers;
			if (eas != null)
			{
				foreach (EventAction ea in eas)
				{
					if (ei.Event.IsSameObjectRef(ea.Event))
					{
						if (ea.TaskIDList != null)
						{
							foreach (TaskID tid in ea.TaskIDList)
							{
								if (tid.Action == null)
								{
									HandlerMethodID hmid = tid as HandlerMethodID;
									if (hmid != null)
									{
									}
									else
									{
										//it is a public action
										tid.SetAction(root.GetActionInstance(tid.ActionId));
									}
								}
								listBoxActions.Items.Add(tid);
							}
						}
						break;
					}
				}
			}
		}
		public Control GetDesignSurfaceUI()
		{
			return this;
		}

		public void OnDataLoaded()
		{
			_eventPath.OnDataLoaded();
		}
		public void OnUIConfigLoaded()
		{
			bool b = _loading;
			_loading = true;
			XmlNode node = _eventPath.XmlData;
			if (node != null)
			{
				int n = XmlUtil.GetAttributeInt(node, XMLATTR_SplitterDistance);
				if (n > 10 && n < splitContainer1.ClientSize.Height - 10)
				{
					splitContainer1.SplitterDistance = n;
				}
			}
			_loading = b;
		}
		public EnumMaxButtonLocation MaxButtonLocation
		{
			get { return _eventPath.MaxButtonLocation; }
		}
		public void OnClassLoadedIntoDesigner(UInt32 classId)
		{
			_eventPath.OnClassLoadedIntoDesigner(classId);
		}
		public void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade)
		{
			_eventPath.OnDefinitionChanged(classId, relatedObject, changeMade);
		}
		public void OnComponentAdded(object obj)
		{
			_eventPath.OnComponentAdded(obj);
		}

		public void OnComponentRename(object obj, string newName)
		{
			_eventPath.OnComponentRename(obj, newName);
		}

		public void OnComponentRemoved(object obj)
		{
			_eventPath.OnComponentRemoved(obj);
		}

		public void OnComponentSelected(object obj)
		{
			if (!_loading)
			{
				_eventPath.OnComponentSelected(obj);
			}
		}

		public void OnActionSelected(IAction action)
		{
			if (!_loading)
			{
				for (int i = 0; i < listBoxActions.Items.Count; i++)
				{
					IAction a = listBoxActions.Items[i] as IAction;
					if (a != null)
					{
						if (a.ActionId == action.ActionId)
						{
							bool b = _loading;
							_loading = true;
							listBoxActions.SelectedIndex = i;
							_loading = b;
							break;
						}
					}
					else
					{
						TaskID tid = listBoxActions.Items[i] as TaskID;
						if (tid != null)
						{
							if (tid.ActionId == action.ActionId)
							{
								bool b = _loading;
								_loading = true;
								listBoxActions.SelectedIndex = i;
								_loading = b;
								break;
							}
						}
					}
				}
			}
		}
		public void OnAssignAction(EventAction ea)
		{
			_eventPath.RefreshEventHandlers(ea);
			_eventPath.OnAssignAction(ea);
			onActionAssignmentChange(ea.Event);
		}

		public void OnRemoveEventHandler(EventAction ea, TaskID task)
		{
			_eventPath.RefreshEventHandlers(ea);
			_eventPath.OnRemoveEventHandler(ea, task);
			onActionAssignmentChange(ea.Event);
		}

		public void OnActionListOrderChanged(object sender, EventAction ea)
		{
			_eventPath.RefreshEventHandlers(ea);
			onActionAssignmentChange(ea.Event);
		}

		public void OnActionChanged(uint classId, IAction act, bool isNewAction)
		{
			if (_panes.Loader.ClassID == classId)
			{
				if (isNewAction)
				{
					if (_eventPath.SelectedEvent == null && _eventPath.SelectedLinkLine == null)
					{
						ComponentIconEvent cie = _eventPath.GetSelectedComponentIcon();
						if (cie != null)
						{
							if (cie.IsForComponent)
							{
								if (cie.ClassPointer.MemberId == act.ExecuterMemberId)
								{
									listBoxActions.Items.Add(act);
								}
							}
							else
							{
								ComponentIconMethod cim = cie as ComponentIconMethod;
								if (cim != null)
								{
									CustomMethodPointer cmp = act.ActionMethod as CustomMethodPointer;
									if (cmp != null)
									{
										if (cmp.MemberId == cim.MethodId)
										{
											listBoxActions.Items.Add(act);
										}
									}
								}
								else
								{
									ComponentIconProperty cip = cie as ComponentIconProperty;
									if (cip != null)
									{
										if (cip.Property.IsActionOwner(act))
										{
											listBoxActions.Items.Add(act);
										}
									}
								}
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < listBoxActions.Items.Count; i++)
					{
						IAction a = listBoxActions.Items[i] as IAction;
						if (a != null)
						{
							if (a.ActionId == act.ActionId)
							{
								if (string.CompareOrdinal(a.ActionName, act.ActionName) != 0)
								{

								}
							}
						}
					}
					//if(_eventPath.
				}
				_eventPath.OnActionChanged(classId, act, isNewAction);
			}
		}

		public void OnMethodChanged(MethodClass method, bool isNewMethod)
		{
			_eventPath.OnMethodChanged(method, isNewMethod);
		}
		public void OnMethodSelected(MethodClass method)
		{
			_eventPath.OnMethodSelected(method);
		}
		public void OnDeleteMethod(MethodClass method)
		{
			_eventPath.OnDeleteMethod(method);
		}
		public void OnActionDeleted(IAction action)
		{
			if (_panes.Loader.ClassID == action.ClassId)
			{
				int i = 0;
				while (i < listBoxActions.Items.Count)
				{
					bool b = false;
					IAction act = listBoxActions.Items[i] as IAction;
					if (act != null)
					{
						if (act.ActionId == action.ActionId)
						{
							listBoxActions.Items.RemoveAt(i);
							b = true;
						}
					}
					else
					{
						TaskID tid = listBoxActions.Items[i] as TaskID;
						if (tid != null)
						{
							if (tid.ActionId == action.ActionId)
							{
								listBoxActions.Items.RemoveAt(i);
								b = true;
							}
						}
					}
					if (!b)
					{
						i++;
					}
				}
			}
			_eventPath.OnActionDeleted(action);
		}
		public void OnDeleteEventMethod(LimnorDesigner.Event.EventHandlerMethod method)
		{
			_eventPath.OnDeleteEventMethod(method);
		}

		public void OnDeleteProperty(LimnorDesigner.Property.PropertyClass property)
		{
			_eventPath.OnDeleteProperty(property);
		}

		public void OnAddProperty(LimnorDesigner.Property.PropertyClass property)
		{
			_eventPath.OnAddProperty(property);
		}

		public void OnPropertyChanged(INonHostedObject property, string name)
		{
			_eventPath.OnPropertyChanged(property, name);
		}
		public void OnPropertySelected(PropertyClass property)
		{
			_eventPath.OnPropertySelected(property);
		}
		public void OnDeleteEvent(LimnorDesigner.Event.EventClass eventObject)
		{
			_eventPath.OnDeleteEvent(eventObject);
		}

		public void OnAddEvent(LimnorDesigner.Event.EventClass eventObject)
		{
			_eventPath.OnAddEvent(eventObject);
		}
		public void OnEventSelected(EventClass eventObject)
		{
			_eventPath.OnEventSelected(eventObject);
		}
		public void OnFireEventActionSelected(FireEventMethod method)
		{
			if (!_loading)
			{
				_eventPath.OnFireEventActionSelected(method);
			}
		}
		public void OnObjectNameChanged(INonHostedObject obj)
		{
			_eventPath.OnObjectNameChanged(obj);
			ActionClass act = obj as ActionClass;
			if (act != null)
			{
				for (int i = 0; i < listBoxActions.Items.Count; i++)
				{
					ActionClass a = listBoxActions.Items[i] as ActionClass;
					if (a != null)
					{
						if (a.ActionId == act.ActionId)
						{
							listBoxActions.Items.RemoveAt(i);
							listBoxActions.Items.Insert(i, act);
						}
					}
					else
					{
						TaskID tid = listBoxActions.Items[i] as TaskID;
						if (tid != null)
						{
							if (tid.ActionId == act.ActionId && tid.ClassId == act.ClassId)
							{
								listBoxActions.Items.RemoveAt(i);
								listBoxActions.Items.Insert(i, tid);
							}
						}
					}
				}
			}
		}

		public void OnIconChanged(uint classId)
		{
			_eventPath.OnIconChanged(classId);
		}

		public void OnAddExternalType(uint classId, Type t)
		{
			_eventPath.OnAddExternalType(classId, t);
		}

		public void OnRemoveExternalType(uint classId, Type t)
		{
			_eventPath.OnRemoveExternalType(classId, t);
		}

		public void SetClassRefIcon(uint classId, Image img)
		{
			_eventPath.SetClassRefIcon(classId, img);
		}

		public void OnResetMap()
		{
			_eventPath.OnResetMap();
		}

		public void OnDatabaseListChanged()
		{
			_eventPath.OnDatabaseListChanged();
		}
		public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
		{
			_eventPath.OnDatabaseConnectionNameChanged(connectionid, newName);
		}
		public void OnInterfaceAdded(LimnorDesigner.Interface.InterfacePointer interfacePointer)
		{
			_eventPath.OnInterfaceAdded(interfacePointer);
		}

		public void OnRemoveInterface(LimnorDesigner.Interface.InterfacePointer interfacePointer)
		{
			_eventPath.OnRemoveInterface(interfacePointer);
		}

		public void OnInterfaceMethodDeleted(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			_eventPath.OnInterfaceMethodDeleted(method);
		}

		public void OnInterfaceMethodChanged(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			_eventPath.OnInterfaceMethodChanged(method);
		}

		public void OnInterfaceEventDeleted(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			_eventPath.OnInterfaceEventDeleted(eventType);
		}

		public void OnInterfaceEventAdded(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			_eventPath.OnInterfaceEventAdded(eventType);
		}

		public void OnInterfaceEventChanged(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			_eventPath.OnInterfaceEventChanged(eventType);
		}

		public void OnInterfacePropertyDeleted(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			_eventPath.OnInterfacePropertyDeleted(property);
		}

		public void OnInterfacePropertyAdded(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			_eventPath.OnInterfacePropertyAdded(property);
		}

		public void OnInterfacePropertyChanged(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			_eventPath.OnInterfacePropertyChanged(property);
		}

		public void OnInterfaceMethodCreated(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			_eventPath.OnInterfaceMethodCreated(method);
		}

		public void OnBaseInterfaceAdded(LimnorDesigner.Interface.InterfaceClass owner, LimnorDesigner.Interface.InterfacePointer baseInterface)
		{
			_eventPath.OnBaseInterfaceAdded(owner, baseInterface);
		}

		public void OnEventListChanged(VPL.ICustomEventMethodDescriptor owner, uint objectId)
		{
			_eventPath.OnEventListChanged(owner, objectId);
		}

		#endregion
		#region event handling
		const string XMLATTR_SplitterDistance = "height";
		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (_eventPath != null && _eventPath.Loaded && !_loading)
			{
				XmlNode node = _eventPath.XmlData;
				if (node != null)
				{
					XmlUtil.SetAttribute(node, XMLATTR_SplitterDistance, this.splitContainer1.SplitterDistance);
					_eventPath.NotifyCurrentChanges();
				}
			}
		}
		private void listBoxActions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!_loading)
			{
				int n = listBoxActions.SelectedIndex;
				if (n >= 0)
				{
					IAction a = listBoxActions.Items[n] as IAction;
					if (a == null)
					{
						TaskID tid = listBoxActions.Items[n] as TaskID;
						if (tid != null)
						{
							if (!tid.IsEmbedded)
							{
								a = tid.Action;
							}
						}
					}
					if (a != null)
					{
						_loading = true;
						_panes.OnActionSelected(a);
						_loading = false;
					}
				}
			}
		}
		void listBoxActions_MouseDown(object sender, MouseEventArgs e)
		{
			int n = listBoxActions.SelectedIndex;
			int m = (int)((double)(e.Y) / (double)(listBoxActions.ItemHeight)) + listBoxActions.TopIndex;
			if (m < listBoxActions.Items.Count)
			{
				if (m != n)
				{
					listBoxActions.SelectedIndex = m;
					n = m;
				}
				if (e.Button == MouseButtons.Right)
				{
					ContextMenu menu = new ContextMenu();
					MenuItemWithBitmap mi;
					if (_eventPath.SelectedLinkLine != null || _eventPath.SelectedEvent != null)
					{
						HandlerMethodID hid = listBoxActions.Items[n] as HandlerMethodID;
						if (hid != null)
						{
							mi = new MenuItemWithBitmap("Delete", mi_removeAction, Resources._cancel.ToBitmap());
							mi.Tag = listBoxActions.Items[n];
							menu.MenuItems.Add(mi);
						}
						else
						{
							mi = new MenuItemWithBitmap("Remove from event", mi_removeActionFromEvent, Resources._removeFromEvent.ToBitmap());
							mi.Tag = listBoxActions.Items[n];
							menu.MenuItems.Add(mi);
						}
						if (listBoxActions.Items.Count > 1)
						{
							menu.MenuItems.Add("-");
						}
						if (n > 0)
						{
							mi = new MenuItemWithBitmap("Move up", mi_moveActionUp, Resources._upIcon.ToBitmap());
							mi.Tag = listBoxActions.Items[n];
							menu.MenuItems.Add(mi);
						}
						if (n < listBoxActions.Items.Count - 1)
						{
							mi = new MenuItemWithBitmap("Move down", mi_moveActionDown, Resources._downIcon.ToBitmap());
							mi.Tag = listBoxActions.Items[n];
							menu.MenuItems.Add(mi);
						}
					}
					else
					{
						mi = new MenuItemWithBitmap("Delete", mi_removeAction, Resources._cancel.ToBitmap());
						mi.Tag = listBoxActions.Items[n];
						menu.MenuItems.Add(mi);
					}
					if (menu.MenuItems.Count > 0)
					{
						menu.Show(listBoxActions, e.Location);
					}
				}
			}
		}
		private void mi_removeAction(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ClassPointer root = _panes.Loader.GetRootId();
				IAction act = mi.Tag as IAction;
				if (act != null)
				{
					root.AskDeleteAction(act, this.FindForm());
				}
				else
				{
					TaskID tid = mi.Tag as TaskID;
					if (tid != null)
					{
						HandlerMethodID hid = tid as HandlerMethodID;
						if (hid != null)
						{
							root.AskDeleteMethod(hid.HandlerMethod, this.FindForm());
						}
						else
						{
							if (tid.Action == null)
							{
								tid.LoadActionInstance(root);
							}
							if (tid.Action != null)
							{
								root.AskDeleteAction(tid.Action, this.FindForm());
							}
						}
					}
				}
			}
		}
		private void mi_moveActionDown(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				IAction act = mi.Tag as IAction;
				TaskID tid = mi.Tag as TaskID;
				if (act != null || tid != null)
				{
					IEvent ie = null;
					if (this._eventPath.SelectedLinkLine != null)
					{
						EventPortOut po = this._eventPath.SelectedLinkLine.StartNode as EventPortOut;
						if (po != null)
						{
							ie = po.Event;
						}
					}
					else if (_eventPath.SelectedEvent != null)
					{
						ie = _eventPath.SelectedEvent.Event;
					}
					if (ie != null)
					{
						ClassPointer root = _panes.Loader.GetRootId();
						List<EventAction> eas = root.EventHandlers;
						if (eas != null)
						{
							foreach (EventAction ea in eas)
							{
								if (ea.Event.IsSameObjectRef(ie))
								{
									if (ea.TaskIDList != null)
									{
										foreach (TaskID tid0 in ea.TaskIDList)
										{
											if (act != null)
											{
												if (!tid0.IsEmbedded)
												{
													if (tid0.ActionId == act.ActionId)
													{
														ea.MoveActionDown(tid0);
														_panes.Loader.NotifyChanges();
														_panes.OnActionListOrderChanged(this, ea);
														break;
													}
												}
											}
											else
											{
												if (tid0.IsSameTask(tid))
												{
													ea.MoveActionDown(tid0);
													_panes.Loader.NotifyChanges();
													_panes.OnActionListOrderChanged(this, ea);
													break;
												}
											}
										}
									}
									break;
								}
							}
						}
					}
				}
			}
		}
		private void mi_moveActionUp(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				IAction act = mi.Tag as IAction;
				TaskID tid = mi.Tag as TaskID;
				if (act != null || tid != null)
				{
					IEvent ie = null;
					if (this._eventPath.SelectedLinkLine != null)
					{
						EventPortOut po = this._eventPath.SelectedLinkLine.StartNode as EventPortOut;
						if (po != null)
						{
							ie = po.Event;
						}
					}
					else if (_eventPath.SelectedEvent != null)
					{
						ie = _eventPath.SelectedEvent.Event;
					}
					if (ie != null)
					{
						ClassPointer root = _panes.Loader.GetRootId();
						List<EventAction> eas = root.EventHandlers;
						if (eas != null)
						{
							foreach (EventAction ea in eas)
							{
								if (ea.Event.IsSameObjectRef(ie))
								{
									if (ea.TaskIDList != null)
									{
										foreach (TaskID tid0 in ea.TaskIDList)
										{
											if (act != null)
											{
												if (!tid0.IsEmbedded)
												{
													if (tid0.ActionId == act.ActionId)
													{
														ea.MoveActionUp(tid0);
														_panes.Loader.NotifyChanges();
														_panes.OnActionListOrderChanged(this, ea);
														break;
													}
												}
											}
											else
											{
												if (tid0.IsSameTask(tid))
												{
													ea.MoveActionUp(tid0);
													_panes.Loader.NotifyChanges();
													_panes.OnActionListOrderChanged(this, ea);
													break;
												}
											}
										}
									}
									break;
								}
							}
						}
					}
				}
			}
		}
		private void mi_removeActionFromEvent(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				TaskID tid0 = mi.Tag as TaskID;
				if (tid0 != null)
				{
					for (int i = 0; i < listBoxActions.Items.Count; i++)
					{
						TaskID tid1 = listBoxActions.Items[i] as TaskID;
						if (tid1 != null)
						{
							if (tid1.TaskId == tid0.TaskId)
							{
								listBoxActions.Items.RemoveAt(i);
								break;
							}
						}
					}
					ClassPointer root = _panes.Loader.GetRootId();
					List<EventAction> eas = root.EventHandlers;
					if (eas != null)
					{
						foreach (EventAction ea in eas)
						{
							if (ea.TaskIDList != null)
							{
								foreach (TaskID tid in ea.TaskIDList)
								{
									if (tid.IsSameTask(tid0))
									{
										ea.RemoveAction(tid.TaskId);
										_panes.Loader.NotifyChanges();
										_panes.OnRemoveEventHandler(ea, tid);
										break;
									}
								}
							}
						}
					}
				}
			}
		}
		private void removeService()
		{
			EventMapService lst = VPLUtil.GetServiceByName(EGSCR) as EventMapService;
			if (lst != null)
			{
				Dictionary<UInt32, EventPathHolder> eps;
				if (lst.TryGetValue(_prjId, out eps))
				{
					if (eps.ContainsKey(_classId))
					{
						if (eps[_classId] == this)
						{
							eps.Remove(_classId);
						}
					}
				}
			}
		}
		private void onActionAssignmentChange(IEvent e)
		{
			if (this._eventPath.SelectedLinkLine != null)
			{
				EventPortOut po = this._eventPath.SelectedLinkLine.StartNode as EventPortOut;
				if (po != null)
				{
					if (po.Event.IsSameObjectRef(e))
					{
						listBoxActions.Items.Clear();
						List<TaskID> lst = po.Actions;
						if (lst != null && lst.Count > 0)
						{
							foreach (TaskID a in lst)
							{
								listBoxActions.Items.Add(a);
							}
						}
					}
				}
			}
			else if (this._eventPath.SelectedEvent != null)
			{
				if (e.IsSameObjectRef(_eventPath.SelectedEvent.Event))
				{
					listBoxActions.Items.Clear();
					ClassPointer root = _panes.Loader.GetRootId();
					List<EventAction> eas = root.EventHandlers;
					if (eas != null)
					{
						foreach (EventAction ea in eas)
						{
							if (ea.Event.IsSameObjectRef(e))
							{
								if (ea.TaskIDList != null)
								{
									foreach (TaskID tid in ea.TaskIDList)
									{
										listBoxActions.Items.Add(tid);
									}
								}
								break;
							}
						}
					}
				}
			}
		}
		#endregion
	}
	class EventMapService : Dictionary<Guid, Dictionary<UInt32, EventPathHolder>>, IVplDesignerService
	{
		public EventMapService()
		{
		}

		#region IVplDesignerService Members

		public void OnRequestService(object component, string serviceRequest)
		{
			bool before = string.CompareOrdinal("BeforeSetSQL", serviceRequest) == 0;
			bool after = string.CompareOrdinal("AfterSetSQL", serviceRequest) == 0;
			if (before || after)
			{
				EasyGrid eg = component as EasyGrid;
				if (eg != null)
				{
					bool handled = false;
					foreach (KeyValuePair<Guid, Dictionary<UInt32, EventPathHolder>> kv1 in this)
					{
						foreach (KeyValuePair<UInt32, EventPathHolder> kv2 in kv1.Value)
						{
							if (before)
								handled = kv2.Value.CacheGridColumns(eg);
							else if (after)
								handled = kv2.Value.RestoreGridColumns(eg);
						}
						if (handled)
						{
							break;
						}
					}
				}
			}
		}

		#endregion
	}
}
