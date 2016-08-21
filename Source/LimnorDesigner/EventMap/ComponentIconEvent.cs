/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimnorDesigner.MethodBuilder;
using XmlSerializer;
using System.Xml;
using MathExp;
using XmlUtility;
using System.Windows.Forms;
using System.Drawing;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using VPL;
using System.ComponentModel;
using System.Xml.Serialization;
using LimnorDesigner.Web;
using LimnorDesigner.Action;
using WindowsUtility;

namespace LimnorDesigner.EventMap
{
	public class ComponentIconEvent : ComponentIcon, IPortOwner, IXmlNodeHolder
	{
		#region fields and constructors
		public const int PortSize = 11;
		private Pen _linkPen;
		private List<EventPortIn> _inPortList;
		private List<EventIcon> _events;
		private XmlNode _xmlNode;
		public ComponentIconEvent()
		{
			_linkPen = new Pen(Color.LightBlue, 2);
		}
		#endregion
		#region Static Methods
		static Random _ran;
		public static void CreateRandomPoint(double size, out double x, out double y)
		{
			if (_ran == null)
			{
				_ran = new Random();
			}
			x = size * (_ran.NextDouble() - 0.5);
			y = Math.Sqrt(size * size / 4.0 - x * x);
			if (_ran.Next(0, 100) > 50)
			{
				y = -y;
			}
		}
		public static void SetInitialPosition(double center, Control c)
		{
			double x, y;
			CreateRandomPoint(center, out x, out y);
			c.Location = new Point((int)(center + x), (int)(center + y));
		}
		#endregion
		#region Methods
		protected override void OnEstablishObjectOwnership(MethodClass owner)
		{
		}
		public virtual bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			List<IClass> objList = root.GetClassList();
			foreach (IClass c in objList)
			{
				if (IsForThePointer(c))
				{
					Init(designer, c);
					if (IsRootClass)
					{
						if (_events != null && _events.Count > 0)
						{
							foreach (EventIcon ei in _events)
							{
								if (ei.CustomEvent != null)
								{
									List<EventPortIn> epis = ei.DestinationPorts;
									if (epis != null && epis.Count > 0)
									{
										foreach (EventPortIn epi in epis)
										{
											epi.RestoreLocation();
											epi.SetLoaded();
										}
									}
								}
							}
						}
					}
					return true;
				}
			}
			return false;
		}
		public EventIcon CheckCreateEventIcon(EventClass ec)
		{
			if (IsRootClass)
			{
				ClassPointer root = this.ClassPointer.RootPointer;
				EventIcon ei = GetEventIcon(ec);
				if (ei == null)
				{
					ei = new EventIcon(this);
					ei.Event = new CustomEventPointer(ec, root);
					if (_events == null)
					{
						_events = new List<EventIcon>();
					}
					_events.Add(ei);
					ei.SetMoveUnLink(true);
					ComponentIconEvent.SetInitialPosition(100, ei);
					Parent.Controls.Add(ei);
					ei.Initialize();
				}
				return ei;
			}
			return null;
		}
		public EventIcon GetEventIcon(IEvent e)
		{
			if (_events != null)
			{
				foreach (EventIcon ei in _events)
				{
					if (ei.Event.IsSameObjectRef(e))
					{
						return ei;
					}
				}
			}
			return null;
		}
		protected override void OnSelectByMouseDown()
		{
			if (IsForComponent)
			{
				DesignerPane.Loader.NotifySelection(ClassPointer.ObjectInstance);
			}
		}
		protected override List<Control> GetRelatedControls()
		{
			List<Control> lst = base.GetRelatedControls();
			if (_inPortList != null && _inPortList.Count > 0)
			{
				foreach (EventPortIn pi in _inPortList)
				{
					lst.Add(pi);
					if (pi.Label != null)
					{
						lst.Add(pi.Label);
					}
					ILinkLineNode l = pi.PrevNode;
					while (l != null && l.PrevNode != null)
					{
						Control c = l as Control;
						if (c != null)
						{
							lst.Add(c);
						}
						l = l.PrevNode;
					}
					EventPortOut po = l as EventPortOut;
					if (po != null)
					{
						EventIcon ei = po.Owner as EventIcon;
						if (ei != null)
						{
							if (ei.SourcePorts.Count == 1)
							{
								lst.Add(po);
								if (po.Label != null)
								{
									lst.Add(po.Label);
								}
								lst.Add(ei);
								if (ei.Label != null)
								{
									lst.Add(ei.Label);
								}
								ComponentIconEvent cie = ei.RelativeOwner as ComponentIconEvent;
								if (cie != null)
								{
									if (cie._events != null && cie._events.Contains(ei))
									{
										cie._events.Remove(ei);
									}
								}
							}
						}
					}
				}
			}
			if (_events != null && _events.Count > 0)
			{
				foreach (EventIcon ei in _events)
				{
					lst.AddRange(ei.GetRelatedControls());
				}
			}
			return lst;
		}
		public void RemoveDestinationIcon(EventPortIn pi)
		{
			if (_inPortList != null)
			{
				foreach (EventPortIn p in _inPortList)
				{
					if (p.Event.IsSameObjectRef(pi.Event))
					{
						_inPortList.Remove(p);
						break;
					}
				}
			}
		}
		public virtual void RemoveIcon()
		{
			Control p = this.Parent;
			if (p != null)
			{
				List<Control> lst = GetRelatedControls();
				foreach (Control c in lst)
				{
					p.Controls.Remove(c);
				}
			}
		}
		public virtual bool IsForThePointer(IObjectPointer pointer)
		{
			IClass ic = pointer as IClass;
			if (ic != null)
			{
				return (ic.ClassId == this.ClassId && ic.MemberId == this.MemberId);
			}
			return false;
		}
		/// <summary>
		/// check whether the action is executed by the owner of this icon
		/// </summary>
		/// <param name="act"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public virtual bool IsActionExecuter(IAction act, ClassPointer root)
		{
			FireEventMethod fe = act.ActionMethod as FireEventMethod;
			if (fe != null)
			{
			}
			else if (act.ActionMethod != null)
			{
				DataTypePointer dtp = act.ActionMethod.Owner as DataTypePointer;
				if (dtp != null)
				{
					return false;
				}
				ClassPointer cpa = root.GetExternalExecuterClass(act);
				if (cpa != null)
				{
					MemberComponentIdCustom idc = this.ClassPointer as MemberComponentIdCustom;
					if (idc != null)
					{
						return (act.ExecuterClassId == idc.DefinitionClassId);
					}
					return false;
				}
				CustomMethodPointer cmp = act.ActionMethod as CustomMethodPointer;
				if (cmp != null)
				{
					//for the same class, ComponentIconMethod is used
					if (act.ExecuterClassId != this.ClassPointer.ClassId)
					{
						MemberComponentIdCustom idc = this.ClassPointer as MemberComponentIdCustom;
						if (idc != null)
						{
							return (act.ExecuterClassId == idc.DefinitionClassId);
						}
					}
					return false;
				}
				else
				{
					SetterPointer sp = act.ActionMethod as SetterPointer;
					if (sp != null)
					{
						CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
						if (cpp != null)
						{
							//for the same class ComponentIconProperty is used 
							if (act.ExecuterClassId != this.ClassPointer.ClassId)
							{
								MemberComponentIdCustom idc = this.ClassPointer as MemberComponentIdCustom;
								if (idc != null)
								{
									return (act.ExecuterClassId == idc.DefinitionClassId);
								}
							}
							return false;
						}
					}
					return (act.ExecuterMemberId == this.ClassPointer.MemberId);
				}
			}
			else
			{
				ActionAttachEvent aae = act as ActionAttachEvent;
				if (aae != null)
				{
					if (aae.ClassId == this.ClassId && aae.ExecuterMemberId == this.ClassPointer.MemberId)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// check whether the action is executed by the owner of this icon
		/// </summary>
		/// <param name="tid"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public virtual bool IsActionExecuter(TaskID tid, ClassPointer root)
		{
			if (!tid.IsEmbedded)
			{
				if (tid.Action == null)
				{
					//only root scope actions are participating the event map
					tid.SetAction(root.GetActionInstance(tid.ActionId));
				}
				if (tid.Action != null)
				{
					return IsActionExecuter(tid.Action, root);
				}
			}
			return false;
		}
		/// <summary>
		/// get the EventIcon for the event. 
		/// create it if it does not exist.
		/// </summary>
		/// <param name="ea"></param>
		/// <param name="eventData"></param>
		/// <returns></returns>
		public EventIcon ValidateEventFirer(List<EventAction> eas, EventPathData eventData)
		{
			EventIcon ei = null;
			if (_events == null)
			{
				_events = new List<EventIcon>();
			}
			foreach (EventIcon e in _events)
			{
				if (e.Event.IsSameObjectRef(eas[0].Event))
				{
					ei = e;
					ei.SetData(eas, eventData);
					break;
				}
			}
			if (ei == null)
			{
				ei = new EventIcon(this, eas, eventData);
				_events.Add(ei);
			}
			return ei;
		}
		public EventPortIn ValidateActionExecuter(EventAction ea, ClassPointer root)
		{
			EventPortIn missing = null;
			if (_inPortList == null)
			{
				_inPortList = new List<EventPortIn>();
			}
			foreach (TaskID tid in ea.TaskIDList)
			{
				if (this.IsActionExecuter(tid, root))
				{
					bool bFound = false;
					foreach (EventPortIn pi in _inPortList)
					{
						if (pi.Event.IsSameObjectRef(ea.Event))
						{
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						EventPortIn pi = new EventPortIn(this);
						pi.Event = ea.Event;
						_inPortList.Add(pi);
						double x, y;
						ComponentIconEvent.CreateRandomPoint(Width + ComponentIconEvent.PortSize, out x, out y);
						pi.Location = new Point((int)(Center.X + x), (int)(Center.Y + y));
						pi.SetLoaded();
						pi.SaveLocation();
						//
						missing = pi;
					}
					break;
				}
			}
			return missing;
		}
		/// <summary>
		/// this control is already added to Parent.Controls
		/// 1. remove invalid inports
		/// 2. remove invalid EventIcon
		/// 3. add missed EventIcon
		/// 4. add missed EventPortIn
		/// 4. add EventIcon objects to Parent.Controls
		/// 5. initialize EventIcon
		/// </summary>
		/// <param name="viewer"></param>
		public virtual void Initialize(EventPathData eventData)
		{
			bool isRoot = this.IsRootClass;
			//validate events
			ClassPointer root = RootClassPointer;
			Dictionary<string, EventClass> events = root.CustomEvents;
			List<EventAction> ehs = root.EventHandlers;
			if (ehs != null && ehs.Count > 0)
			{
				if (_inPortList == null)
				{
					_inPortList = new List<EventPortIn>();
				}
				else
				{
					//remove invalid inport
					List<EventPortIn> invalidInports = new List<EventPortIn>();
					foreach (EventPortIn pi in _inPortList)
					{
						bool bFound = false;
						foreach (EventAction ea in ehs)
						{
							if (pi.Event.IsSameObjectRef(ea.Event))
							{
								if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
								{
									foreach (TaskID tid in ea.TaskIDList)
									{
										if (this.IsActionExecuter(tid, root))
										{
											bFound = true;
											break;
										}
									}
								}
								if (bFound)
								{
									break;
								}
							}
						}
						if (!bFound)
						{
							invalidInports.Add(pi);
						}
					}
					if (invalidInports.Count > 0)
					{
						foreach (EventPortIn pi in invalidInports)
						{
							_inPortList.Remove(pi);
						}
					}
				}
				//remove invalid EventIcon objects
				if (_events != null)
				{
					List<EventIcon> invalid = new List<EventIcon>();
					foreach (EventIcon ei in _events)
					{
						bool bFound = false;
						if (ei.Event != null)
						{
							EventClass eic = ei.CustomEvent;
							if (eic != null)
							{
								if (isRoot)
								{
									if (this.ClassId == eic.ClassId)
									{
										foreach (EventClass ec in events.Values)
										{
											if (ec.IsSameObjectRef(eic))
											{
												bFound = true;
												break;
											}
										}
									}
								}
							}
							if (!bFound)
							{
								foreach (EventAction ea in ehs)
								{
									if (ei.Event.IsSameObjectRef(ea.Event))
									{
										bFound = true;
										break;
									}
								}
							}
						}
						if (!bFound)
						{
							invalid.Add(ei);
						}
					}
					foreach (EventIcon ei in invalid)
					{
						_events.Remove(ei);
					}
					//remove duplicated ports
					foreach (EventIcon ei in _events)
					{
						ei.ValidateSourcePorts();
					}
				}
				//add missed EventIcon
				//add missed EventPortIn
				Dictionary<string, List<EventAction>> eaGroup = EventPathData.CreateHandlerGroup(ehs);
				foreach (EventAction ea in ehs)
				{
					IClass ic = EventPathData.GetEventFirerRef(ea.Event.Holder);
					if (ic != null)
					{
						if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
						{
							if (ic.IsSameObjectRef(this.ClassPointer))
							{
								List<EventAction> eas;
								if (eaGroup.TryGetValue(ea.Event.ObjectKey, out eas))
								{
									ValidateEventFirer(eas, eventData);
								}
							}
							ValidateActionExecuter(ea, root);
						}
					}
				}

			}
			if (isRoot)
			{
				foreach (EventClass ec in events.Values)
				{
					bool bFound = false;
					if (_events != null)
					{
						foreach (EventIcon ei in _events)
						{
							if (ec.IsSameObjectRef(ei.CustomEvent))
							{
								ei.SetMoveUnLink(true);
								bFound = true;
								break;
							}
						}
					}
					if (!bFound)
					{
						if (_events == null)
						{
							_events = new List<EventIcon>();
						}
						EventIcon ei = new EventIcon(this);
						ei.Event = new CustomEventPointer(ec, root);
						ei.SetMoveUnLink(true);
						_events.Add(ei);
					}
				}
			}
			//add EventIcon objects to Parent.Controls
			//initialize EventIcon
			if (_events != null)
			{
				Parent.Controls.AddRange(_events.ToArray());
				foreach (EventIcon ei in _events)
				{
					ei.AdjustPosition();
					ei.Initialize();
				}
			}
			else
			{
				_events = new List<EventIcon>();
			}
		}
		public void ShowEventOwnerLinks(PaintEventArgs e)
		{
			if (_events != null && _events.Count > 0)
			{
				Point p0 = this.Center;
				foreach (EventIcon ei in _events)
				{
					e.Graphics.DrawLine(_linkPen, p0, ei.Center);
				}
			}
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				int w = 0, h = 0;
				if (this.Left > ep.Size.Width - 80)
				{
					w = this.Left + 80;
				}
				if (this.Top > ep.Size.Height - 80)
				{
					h = this.Top + 80;
				}
				if (w > 0 || h > 0)
				{
					if (w == 0)
					{
						w = ep.Size.Width;
					}
					if (h == 0)
					{
						h = ep.Size.Height;
					}
					ep.Size = new Size(w, h);
				}
				ep.NotifyCurrentChanges();
			}
		}
		protected virtual LimnorContextMenuCollection CreateMenuData()
		{
			return LimnorContextMenuCollection.GetMenuCollection(this.ClassPointer);
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			if ((this.IsForComponent || IsClassType) && !(this.ClassPointer is HtmlElementUnknown))
			{
				if (IsRootClass)
				{
					EventPath ep = this.Parent as EventPath;
					if (ep != null)
					{
						ep.OnCreateContextMenuForRootComponent(mnu, location);
						mnu.MenuItems.Add("-");
					}
				}
				LimnorContextMenuCollection menudata = CreateMenuData();
				if (menudata != null)
				{
					MenuItem mi;
					MenuItem m0;
					MenuItem m1;
					//
					#region Create Action
					List<MenuItemDataMethod> methods = menudata.PrimaryMethods;
					if (methods.Count > 0)
					{
						mi = new MenuItemWithBitmap("Create Action", Resources._newMethodAction.ToBitmap());
						foreach (MenuItemDataMethod kv in methods)
						{
							m0 = new MenuItemWithBitmap(kv.Key, Resources._methodAction.ToBitmap());
							m0.Click += new EventHandler(miAction_Click);
							kv.Location = location;
							m0.Tag = kv;
							mi.MenuItems.Add(m0);
						}
						methods = menudata.SecondaryMethods;
						if (methods.Count > 0)
						{
							m1 = new MenuItemWithBitmap("More methods", Resources._methods.ToBitmap());
							mi.MenuItems.Add(m1);
							foreach (MenuItemDataMethod kv in methods)
							{
								m0 = new MenuItemWithBitmap(kv.Key, Resources._methodAction.ToBitmap());
								m0.Click += new EventHandler(miAction_Click);
								kv.Location = location;
								m0.Tag = kv;
								m1.MenuItems.Add(m0);
							}
							m0 = new MenuItemWithBitmap("*All methods* =>", Resources._dialog.ToBitmap());
							MenuItemDataMethodSelector miAll = new MenuItemDataMethodSelector(m0.Text, menudata);
							miAll.Location = location;
							m0.Tag = miAll;
							m1.MenuItems.Add(m0);
							m0.Click += new EventHandler(miSetMethods_Click);
						}
						//
						mnu.MenuItems.Add(mi);
					}
					#endregion
					//
					#region Create Set Property Action
					List<MenuItemDataProperty> properties = menudata.PrimaryProperties;
					if (properties.Count > 0)
					{
						mi = new MenuItemWithBitmap("Create Set Property Action", Resources._newPropAction.ToBitmap());
						foreach (MenuItemDataProperty kv in properties)
						{
							m0 = new MenuItemWithBitmap(kv.Key, Resources._propAction.ToBitmap());
							m0.Click += new EventHandler(miSetProperty_Click);
							kv.Location = location;
							m0.Tag = kv;
							mi.MenuItems.Add(m0);
						}
						properties = menudata.SecondaryProperties;
						if (properties.Count > 0)
						{
							m1 = new MenuItemWithBitmap("More properties", Resources._properties.ToBitmap());
							mi.MenuItems.Add(m1);

							foreach (MenuItemDataProperty kv in properties)
							{
								m0 = new MenuItemWithBitmap(kv.Key, Resources._propAction.ToBitmap());
								m0.Click += new EventHandler(miSetProperty_Click);
								kv.Location = location;
								m0.Tag = kv;
								m1.MenuItems.Add(m0);
							}
							m0 = new MenuItemWithBitmap("*All properties* =>", Resources._dialog.ToBitmap());
							m1.MenuItems.Add(m0);
							MenuItemDataPropertySelector pAll = new MenuItemDataPropertySelector(m0.Text, menudata);
							pAll.Location = location;
							m0.Tag = pAll;
							m0.Click += new EventHandler(miSetProperties_Click);
						}
						//
						mnu.MenuItems.Add(mi);
					}
					#endregion
					//
					#region Assign Actions
					List<MenuItemDataEvent> events = menudata.PrimaryEvents;
					if (events.Count > 0)
					{
						mi = new MenuItemWithBitmap("Assign Action", Resources._eventHandlers.ToBitmap());
						foreach (MenuItemDataEvent kv in events)
						{
							m0 = new MenuItemWithBitmap(kv.Key, Resources._event1.ToBitmap());
							m0.Click += new EventHandler(miAssignEvent_Click);
							kv.Location = location;
							m0.Tag = kv;
							mi.MenuItems.Add(m0);
							EventItem ei = kv as EventItem;
							if (ei != null)
							{
								IEventInfoTree emi = ei.Value as IEventInfoTree;
								if (emi != null)
								{
									IEventInfoTree[] subs = emi.GetSubEventInfo();
									LimnorContextMenuCollection.createEventTree(m0, location, kv.Owner, subs, new EventHandler(miAssignEvent_Click));
								}
							}
						}
						events = menudata.SecondaryEvents;
						if (events.Count > 0)
						{
							m1 = new MenuItemWithBitmap("More events", Resources._events.ToBitmap());
							mi.MenuItems.Add(m1);
							foreach (MenuItemDataEvent kv in events)
							{
								m0 = new MenuItemWithBitmap(kv.Key, Resources._event1.ToBitmap());
								m0.Click += new EventHandler(miAssignEvent_Click);
								kv.Location = location;
								m0.Tag = kv;
								m1.MenuItems.Add(m0);
								EventItem ei = kv as EventItem;
								if (ei != null)
								{
									IEventInfoTree emi = ei.Value as IEventInfoTree;
									if (emi != null)
									{
										IEventInfoTree[] subs = emi.GetSubEventInfo();
										LimnorContextMenuCollection.createEventTree(m0, location, kv.Owner, subs, new EventHandler(miAssignEvent_Click));
									}
								}
							}
							m0 = new MenuItemWithBitmap("*All events* =>", Resources._dialog.ToBitmap());
							m1.MenuItems.Add(m0);
							MenuItemDataEventSelector eAll = new MenuItemDataEventSelector(m0.Text, location, menudata);
							m0.Tag = eAll;
							m0.Click += new EventHandler(miSetEvents_Click);
						}
						mnu.MenuItems.Add(mi);
					}
					#endregion
				}
			}
		}
		public override void OnRelativeDrawingMove(RelativeDrawing relDraw)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ep.NotifyCurrentChanges();
			}
		}
		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}
		protected virtual bool OnBeforeUseComponent()
		{
			return true;
		}
		public virtual PortCollection GetAllPorts()
		{
			PortCollection pc = new PortCollection();
			List<EventPortIn> list = DestinationPorts;
			if (list != null)
			{
				foreach (LinkLineNodeInPort p in list)
				{
					pc.Add(p);
				}
			}
			if (_events != null)
			{
				if (IsRootClass)
				{
					foreach (EventIcon ei in _events)
					{
						if (ei.CustomEvent != null)
						{
							List<EventPortIn> ports = ei.DestinationPorts;
							if (ports != null && ports.Count > 0)
							{
								pc.AddRange(ports.ToArray());
							}
						}
					}
				}
			}
			List<EventPortOut> l2 = SourcePorts;
			if (l2 != null)
			{
				foreach (LinkLineNodeOutPort p in l2)
				{
					pc.Add(p);
				}
			}
			return pc;
		}
		public const string XML_EVENTS = "Events";
		const string XML_INPORTS = "TargetPorts";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			//write event icons
			if (_events != null && _events.Count > 0)
			{
				XmlNode nodeEvents = XmlUtil.CreateSingleNewElement(node, XML_EVENTS);
				foreach (EventIcon ei in _events)
				{
					XmlNode eNode = nodeEvents.OwnerDocument.CreateElement(XmlTags.XML_Item);
					nodeEvents.AppendChild(eNode);
					ei.WriteToXmlNode((XmlObjectWriter)writer, eNode);
				}
			}
			//write input ports
			if (_inPortList != null && _inPortList.Count > 0)
			{
				XmlNode portsNode = XmlUtil.CreateSingleNewElement(node, XML_INPORTS);
				foreach (EventPortIn port in _inPortList)
				{
					XmlNode pNode = portsNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
					portsNode.AppendChild(pNode);
					port.OnWriteToXmlNode(writer, pNode);
				}
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			XmlNodeList nodes = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", XML_EVENTS, XmlTags.XML_Item));
			if (nodes != null && nodes.Count > 0)
			{
				_events = new List<EventIcon>();
				foreach (XmlNode eNode in nodes)
				{
					EventIcon ei = new EventIcon(this);
					ei.ReadFromXmlNode((XmlObjectReader)reader, eNode);
					_events.Add(ei);
				}
			}
			//
			_inPortList = new List<EventPortIn>();
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", XML_INPORTS, XmlTags.XML_Item));
			foreach (XmlNode n in nds)
			{
				EventPortIn pi = new EventPortIn(this);
				pi.OnReadFromXmlNode(reader, n);
				_inPortList.Add(pi);
			}
		}
		#endregion
		#region private methods
		private void createNewAction(MenuItemDataMethod data)
		{
			if (OnBeforeUseComponent())
			{
				EventPath ep = this.Parent as EventPath;
				if (ep != null)
				{
					ILimnorDesignPane dp = ep.Panes.Loader.DesignPane;
					IAction act = data.CreateMethodAction(dp, this.ClassPointer, null, null);
					if (act != null)
					{

					}
				}
			}
		}
		private void miAction_Click(object sender, EventArgs e)
		{
			MenuItemDataMethod data = (MenuItemDataMethod)(((MenuItem)sender).Tag);
			createNewAction(data);
		}
		private void miSetMethods_Click(object sender, EventArgs e)
		{
			miAction_Click(sender, e);
		}
		private void miSetProperty_Click(object sender, EventArgs e)
		{
			if (OnBeforeUseComponent())
			{
				EventPath ep = this.Parent as EventPath;
				if (ep != null)
				{
					ILimnorDesignPane dp = ep.Panes.Loader.DesignPane;
					MenuItemDataProperty data = (MenuItemDataProperty)(((MenuItem)sender).Tag);
					ActionClass act = data.CreateSetPropertyAction(dp, ClassPointer, null, null);
					if (act != null)
					{
					}
				}
			}
		}
		private void miSetProperties_Click(object sender, EventArgs e)
		{
			miSetProperty_Click(sender, e);
		}
		private void miAssignEvent_Click(object sender, EventArgs e)
		{
			if (OnBeforeUseComponent())
			{
				EventPath ep = this.Parent as EventPath;
				if (ep != null)
				{
					ILimnorDesignPane dp = ep.Panes.Loader.DesignPane;
					MenuItemDataEvent data = (MenuItemDataEvent)(((MenuItem)sender).Tag);
					data.ExecuteMenuCommand(ep.Project, this.ClassPointer, dp.RootXmlNode, ep.Panes, null, null);
				}
			}
		}
		private void miSetEvents_Click(object sender, EventArgs e)
		{
			miAssignEvent_Click(sender, e);
		}
		#endregion
		#region Properties
		public virtual bool IsPortOwner { get { return true; } }
		public bool IsLinked
		{
			get
			{
				if (_events != null && _events.Count > 0)
				{
					return true;
				}
				if (_inPortList != null && _inPortList.Count > 0)
				{
					return true;
				}
				return false;
			}
		}
		public List<EventIcon> EventIcons
		{
			get
			{
				if (_events == null)
				{
					_events = new List<EventIcon>();
				}
				return _events;
			}
		}
		public virtual List<EventPortOut> SourcePorts
		{
			get
			{
				List<EventPortOut> lst = new List<EventPortOut>();
				if (_events != null && _events.Count > 0)
				{
					foreach (EventIcon ei in _events)
					{
						lst.AddRange(ei.SourcePorts);
					}
				}
				return lst;
			}
		}
		public List<EventPortIn> DestinationPorts
		{
			get
			{
				if (_inPortList == null)
				{
					_inPortList = new List<EventPortIn>();
				}
				return _inPortList;
			}
			set
			{
				_inPortList = value;
			}
		}
		protected virtual bool IsClassType
		{
			get
			{
				return false;
			}
		}
		public bool IsRootClass
		{
			get
			{
				if (IsForComponent)
				{
					if (ClassPointer.DefinitionClassId == ClassPointer.ClassId)
					{
						if (ClassPointer.MemberId == ClassPointer.RootPointer.MemberId)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public virtual ClassPointer RootClassPointer
		{
			get
			{
				return this.ClassPointer.RootPointer;
			}
		}
		#endregion
		#region IPortOwner Members

		public bool IsDummyPort
		{
			get { return false; }
		}

		public void AddOutPort(LinkLineNodeOutPort port)
		{
			throw new DesignerException("Cannot call AddOutPort for {0}", this.GetType().Name);
		}

		public void AddInPort(LinkLineNodeInPort port)
		{
			if (port == null)
			{
				throw new DesignerException("Cannot add null to inports for {1}", this.GetType().Name);
			}
			EventPortIn epi = port as EventPortIn;
			if (epi == null)
			{
				throw new DesignerException("Cannot add {0} to inports for {1}", port.GetType().Name, this.GetType().Name);
			}
			if (_inPortList == null)
			{
				_inPortList = new List<EventPortIn>();
			}
			_inPortList.Add(epi);
		}
		public virtual void RemovePort(LinkLineNodePort port)
		{
			if (_inPortList != null)
			{
				foreach (EventPortIn p in _inPortList)
				{
					if (p == port)
					{
						_inPortList.Remove(p);
						break;
					}
				}
			}
		}
		public int InPortCount
		{
			get
			{
				if (_inPortList != null)
				{
					return _inPortList.Count;
				}
				return 0;
			}
		}

		public string TraceInfo
		{
			get
			{
				return this.ClassPointer.Name;
			}
		}

		public uint PortOwnerID
		{
			get
			{
				return ActiveDrawingID;
			}
		}

		#endregion
		#region IXmlNodeHolder Members
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmlNode;
			}
			set
			{
				_xmlNode = value;
			}
		}

		#endregion
	}
}
