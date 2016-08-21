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
using MathExp;
using System.Windows.Forms;
using XmlSerializer;
using System.Xml;
using System.Drawing;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Event;
using TraceLog;
using WindowsUtility;

namespace LimnorDesigner.EventMap
{
	/// <summary>
	/// one event for a component
	/// </summary>
	public class EventIcon : RelativeDrawing, IEventMapSource
	{
		#region fields and constructors
		const string XML_RelativePosition = "RelativePosition";
		const string XML_Position = "Position";
		const string XML_OutPorts = "OutPorts";
		const string XML_InPorts = "InPorts";
		const int ICONSIZE = 16;
		Color C_SelectedByMouse = Color.Yellow;
		Color C_Selected = Color.Cyan;
		private IEvent _event;
		private DrawingLabel _label;
		private bool _selected;
		private List<EventPortOut> _outPortList;
		private List<EventPortIn> _inPortList;
		//
		private List<EventAction> _eventActionList;
		private EventPathData _data;
		public EventIcon(Control owner)
			: base(owner)
		{
			init();
		}

		public EventIcon(Control owner, List<EventAction> ea, EventPathData eventPathData)
			: base(owner)
		{
			init();
			SetData(ea, eventPathData);
		}
		#endregion
		#region private methods
		private void init()
		{
			this.Size = new System.Drawing.Size(ICONSIZE, ICONSIZE);
			_label = new DrawingLabel(this);
			_label.RelativePosition.IsXto0 = true;
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			_label.Cursor = System.Windows.Forms.Cursors.Hand;
		}
		private void refreshLabel()
		{
			if (_label != null && _label.Parent != null)
			{
				_label.IsSelected = IsSelectedByMouseMove;
				if (IsSelectedByMouseMove)
				{
					_label.BackColor = C_SelectedByMouse;
				}
				else if (IsSelected)
				{
					_label.BackColor = C_Selected;
				}
				else
				{
					_label.BackColor = _label.Parent.BackColor;
				}
				_label.Refresh();
			}
			this.Refresh();
		}
		#endregion
		#region Properties
		public EventClass CustomEvent
		{
			get
			{
				CustomEventPointer cep = Event as CustomEventPointer;
				if (cep != null)
				{
					return cep.Event;
				}
				return null;
			}
		}
		public UInt32 DeclarerClassId
		{
			get
			{
				return _event.Declarer.ClassId;
			}
		}
		public bool IsSelectedByMouseMove
		{
			get
			{
				if (this.IsMouseIn)
					return true;
				if (_label != null && _label.IsMouseIn)
					return true;
				return false;
			}
		}
		public bool IsSelected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
				refreshLabel();
				this.Invalidate();
			}
		}
		public bool IsCustomEventForRoot
		{
			get
			{
				if (Event != null && Event.IsCustomEvent)
				{
					ComponentIconEvent cie = this.RelativeOwner as ComponentIconEvent;
					if (cie != null)
					{
						if (cie.IsRootClass)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public override UInt32 OwnerID
		{
			get
			{
				IControlWithID c = _owner as IControlWithID;
				if (c != null)
				{
					return c.ControlID;
				}
				return 0;
			}
		}
		public IEvent Event
		{
			get
			{
				return _event;
			}
			set
			{
				_event = value;
				RefreshLabelText();
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				return _data.RootPointer;
			}
		}
		public List<EventPortOut> SourcePorts
		{
			get
			{
				if (_outPortList == null)
				{
					_outPortList = new List<EventPortOut>();
				}
				return _outPortList;
			}
			set
			{
				_outPortList = value;
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
		public DrawingLabel Label
		{
			get
			{
				return _label;
			}
		}
		public EventAction Handlers
		{
			get
			{
				if (_eventActionList != null && _eventActionList.Count > 0)
				{
					foreach (EventAction ea in _eventActionList)
					{
						if (ea.GetAssignActionId() == 0)
						{
							return ea;
						}
					}
				}
				return null;
			}
		}
		#endregion
		#region Methods
		/// <summary>
		/// TBD
		/// </summary>
		public void ValidateSourcePorts()
		{
		}
		public void RefreshLabelText()
		{
			_label.Text = _event.ShortDisplayName;
			_label.Refresh();
		}
		public void RemoveInPort(LinkLineNodePort port)
		{
			if (_inPortList != null && _inPortList.Count > 0)
			{
				foreach (EventPortIn ei in _inPortList)
				{
					if (port.PortID == ei.PortID && port.PortInstanceID == ei.PortInstanceID)
					{
						_inPortList.Remove(ei);
						break;
					}
				}
			}
		}
		protected override void OnSelectByMouseDown()
		{
			ComponentIconEvent cie = RelativeOwner as ComponentIconEvent;
			if (cie != null)
			{
				if (cie.IsRootClass)
				{
					EventClass ec = CustomEvent;
					if (ec != null)
					{
						EventPath ep = this.Parent as EventPath;
						if (ep != null)
						{
							ep.Panes.OnEventSelected(ec);
							ep.Panes.Loader.NotifySelection(ec);
						}
					}
				}
			}
		}
		public List<Control> GetRelatedControls()
		{
			List<Control> lst = new List<Control>();
			lst.Add(this);
			lst.Add(_label);
			if (_outPortList != null && _outPortList.Count > 0)
			{
				foreach (EventPortOut po in _outPortList)
				{
					lst.Add(po);
					if (po.Label != null)
					{
						lst.Add(po.Label);
					}
					ILinkLineNode l = po.NextNode;
					while (l != null && l.NextNode != null)
					{
						Control c = l as Control;
						if (c != null)
						{
							lst.Add(c);
						}
						l = l.NextNode;
					}
					EventPortIn pi = l as EventPortIn;
					if (pi != null)
					{
						lst.Add(pi);
						if (pi.Label != null)
						{
							lst.Add(pi.Label);
						}
						ComponentIconEventhandle cieh = pi.Owner as ComponentIconEventhandle;
						if (cieh != null)
						{
							if (cieh.Label != null)
							{
								lst.Add(cieh.Label);
							}
							lst.Add(cieh);
						}
						else
						{
							ComponentIconEvent cie = pi.Owner as ComponentIconEvent;
							if (cie != null)
							{
								cie.RemoveDestinationIcon(pi);
							}
						}
						if (this.Parent != null)
						{
							foreach (Control c in this.Parent.Controls)
							{
								ComponentIconEvent cie = c as ComponentIconEvent;
								if (cie != null)
								{
									List<EventPortIn> pis = cie.DestinationPorts;
									if (pis != null && pis.Count > 0)
									{
										bool b = false;
										foreach (EventPortIn p in pis)
										{
											if (p == pi)
											{
												b = true;
												break;
											}
										}
										if (b)
										{
											pis.Remove(pi);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			return lst;
		}
		public void SetData(List<EventAction> eas, EventPathData eventPathData)
		{
			_eventActionList = eas;
			_data = eventPathData;
			_event = eas[0].Event;
			_label.Text = eas[0].Event.ShortDisplayName;
		}
		/// <summary>
		/// remove invalid outports
		/// add missing outports
		/// </summary>
		/// <returns>missing outports added</returns>
		public List<EventPortOut> Initialize()
		{
			if (_inPortList != null && _inPortList.Count > 0)
			{
				foreach (EventPortIn epi in _inPortList)
				{
					epi.RestoreLocation();
				}
			}
			List<EventPortOut> missing = new List<EventPortOut>();
			if (_data != null && _data.Owner != null && _data.Owner.Loader != null)
			{
				ClassPointer root = _data.Owner.Loader.GetRootId();
				Dictionary<string, List<EventAction>> eaGroup = EventPathData.CreateHandlerGroup(root.EventHandlers);
				bool hasAction = false;
				if (eaGroup.TryGetValue(_event.ObjectKey, out _eventActionList))
				{
					foreach (EventAction ea in _eventActionList)
					{
						if (ea.TaskIDList.Count > 0)
						{
							hasAction = true;
							break;
						}
					}
				}
				else
				{
					_eventActionList = new List<EventAction>();
				}
				if (!hasAction)
				{
					_outPortList.Clear();
				}
				else
				{
					foreach (EventAction ea in _eventActionList)
					{
						for (int i = 0; i < ea.TaskIDList.Count; i++)
						{
							if (ea.TaskIDList[i].Action == null)
							{
								ea.TaskIDList[i].LoadActionInstance(root);
							}
						}
					}
					//remove invalid out ports===============================
					if (_outPortList == null)
					{
						_outPortList = new List<EventPortOut>();
					}
					else
					{
						if (_outPortList.Count > 0)
						{
							List<EventPortOut> invalidPorts = new List<EventPortOut>();
							foreach (EventPortOut po in _outPortList)
							{
								bool bLinked = false;
								//for all actions for this event, find one that is for this port
								foreach (EventAction ea in _eventActionList)
								{
									for (int i = 0; i < ea.TaskIDList.Count; i++)
									{
										bLinked = po.CanActionBeLinked(ea.TaskIDList[i]);
										if (bLinked)
										{
											break;
										}
									}
									if (bLinked)
									{
										break;
									}
								}
								if (!bLinked)
								{
									invalidPorts.Add(po);
								}
							}
							if (invalidPorts.Count > 0)
							{
								foreach (EventPortOut po in invalidPorts)
								{
									_outPortList.Remove(po);
								}
							}
						}
					}
					//===add missing ports=======================================
					foreach (EventAction ea in _eventActionList)
					{
						if (ea.TaskIDList.Count > 0)
						{
							//
							//add missing outports, generate EventPortOut instances to link to EventPortIn instances
							//
							for (int i = 0; i < ea.TaskIDList.Count; i++)
							{
								EventPortOut po = null;
								bool bLinked = false;
								foreach (EventPortOut eo in _outPortList)
								{
									bLinked = eo.IsForTheAction(ea.TaskIDList[i]);
									if (bLinked)
									{
										break;
									}
									if (po == null)
									{
										if (eo.CanActionBeLinked(ea.TaskIDList[i]))
										{
											po = eo;
										}
									}
								}
								if (!bLinked)
								{
									if (po != null)
									{
										po.AddAction(ea.TaskIDList[i]);
									}
									else
									{
										po = EventPortOut.CreateOutPort(ea.TaskIDList[i], this);
										if (po != null)
										{
											po.Event = _event;
											_outPortList.Add(po);
											missing.Add(po);
										}
									}
								}
							}
						}
					}
				}
			}
			return missing;
		}
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (this.Parent != null)
			{
				if (_label.Parent != this.Parent)
				{
					this.Parent.Controls.Add(_label);
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
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			refreshLabel();
		}
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			refreshLabel();
		}
		public override void OnRelativeDrawingMouseEnter(RelativeDrawing relDraw)
		{
			refreshLabel();
		}
		public override void OnRelativeDrawingMouseLeave(RelativeDrawing relDraw)
		{
			refreshLabel();
		}
		public override void OnRelativeDrawingMouseDown(RelativeDrawing relDraw, MouseEventArgs e)
		{
			OnMouseDown(e);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			int x = (this.Width - Resources._event1.Width) / 2;
			int y = (this.Height - Resources._event1.Height) / 2;
			if (_event is CustomEventPointer)
			{
				if (IsSelectedByMouseMove)
				{
					e.Graphics.DrawIcon(Resources._custEventSelectedbyMouse, x, y);
				}
				else if (IsSelected)
				{
					e.Graphics.DrawIcon(Resources._custEventSelected, x, y);
				}
				else
				{
					e.Graphics.DrawIcon(Resources._custEvent, x, y);
				}
			}
			else
			{
				if (IsSelectedByMouseMove)
				{
					e.Graphics.DrawIcon(Resources._eventSelectedByMouse, x, y);
				}
				else if (IsSelected)
				{
					e.Graphics.DrawIcon(Resources._eventSelected, x, y);
				}
				else
				{
					e.Graphics.DrawIcon(Resources._event1, x, y);
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			bool isWebComponent = DesignUtil.IsWebClientObject(_event.Owner);
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ep.SelectEventIcon(this);
			}
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu menu = new ContextMenu();
				MenuItemWithBitmap mi;
				mi = new MenuItemWithBitmap("Assign actions", mi_assignActions, Resources._eventHandlers.ToBitmap());
				menu.MenuItems.Add(mi);
				if (isWebComponent)
				{
					mi = new MenuItemWithBitmap("Add client handler actions", mi_addWebClientHandlerClientActs, Resources._handlerMethod.ToBitmap());
					menu.MenuItems.Add(mi);
					mi = new MenuItemWithBitmap("Add server handler actions", mi_addWebClientHandlerServerActs, Resources._webServerHandler.ToBitmap());
					menu.MenuItems.Add(mi);
					mi = new MenuItemWithBitmap("Add client and download handler actions", mi_addWebClientHandlerDownloadActs, Resources._webHandler2.ToBitmap());
				}
				else
				{
					mi = new MenuItemWithBitmap("Add handler method", mi_addHandler, Resources._handlerMethod.ToBitmap());
				}
				menu.MenuItems.Add(mi);
				menu.MenuItems.Add("-");
				if (IsCustomEventForRoot)
				{
					mi = new MenuItemWithBitmap("Create event-firing action", mi_createEventFireMethod, Resources._createEventFireAction.ToBitmap());
					menu.MenuItems.Add(mi);
					menu.MenuItems.Add("-");
					mi = new MenuItemWithBitmap("Delete event", mi_removeEvent, Resources._delEvent.ToBitmap());
					menu.MenuItems.Add(mi);
				}
				else
				{
					mi = new MenuItemWithBitmap("Remove event handling", mi_remove, Resources._cancel.ToBitmap());
					menu.MenuItems.Add(mi);
				}
				menu.Show(this, e.Location);
			}
		}
		private void mi_addHandler(object sender, EventArgs e)
		{
			addEventHandler(typeof(WebClientEventHandlerMethodServerActions));
		}
		private void mi_addWebClientHandlerClientActs(object sender, EventArgs e)
		{
			addEventHandler(typeof(WebClientEventHandlerMethodClientActions));
		}
		private void mi_addWebClientHandlerServerActs(object sender, EventArgs e)
		{
			addEventHandler(typeof(WebClientEventHandlerMethodServerActions));
		}
		private void mi_addWebClientHandlerDownloadActs(object sender, EventArgs e)
		{
			addEventHandler(typeof(WebClientEventHandlerMethodDownloadActions));
		}
		private void addEventHandler(Type actionType)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				root.AddEventhandlerMethod(actionType, this.Event, 0, this.Bounds, this.FindForm());
			}
		}
		private void mi_assignActions(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ep.AssignActions(this.Event);
			}
		}
		private void mi_remove(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ep.RemoveEventhandlers(this.Event);
			}
		}
		private void mi_removeEvent(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				EventClass ec = this.CustomEvent;
				if (ec.ClassId == root.ClassId)
				{
					root.AskDeleteEvent(ec, this.FindForm());
				}
			}
		}
		private void mi_createEventFireMethod(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				EventClass ec = this.CustomEvent;
				if (ec.ClassId == root.ClassId)
				{
					root.CreateFireEventAction(ec, ep.Panes.Loader.Writer, null, ep.FindForm());
				}
			}
		}
		public const string XML_Event = "Event";
		public void WriteToXmlNode(XmlObjectWriter writer, XmlNode node)
		{
			//
			XmlSerialization.WriteValueToChildNode(node, XML_RelativePosition, _label.RelativePosition.Location);
			XmlNode nd = node.SelectSingleNode(XML_RelativePosition);
			XmlSerialization.SetAttribute(nd, "xTo0", _label.RelativePosition.IsXto0);
			XmlSerialization.SetAttribute(nd, "yTo0", _label.RelativePosition.IsYto0);
			//
			XmlNode eNode = XmlUtil.CreateSingleNewElement(node, XML_Event);
			writer.WriteObjectToNode(eNode, _event);
			//
			XmlNode ndPos = XmlUtil.CreateSingleNewElement(node, XML_Position);
			XmlSerialization.WriteValueToChildNode(ndPos, XML_RelativePosition, RelativePosition.Location);
			nd = ndPos.SelectSingleNode(XML_RelativePosition);
			XmlSerialization.SetAttribute(nd, "xTo0", RelativePosition.IsXto0);
			XmlSerialization.SetAttribute(nd, "yTo0", RelativePosition.IsYto0);
			//
			if (_outPortList != null && _outPortList.Count > 0)
			{
				XmlNode portsNode = XmlUtil.CreateSingleNewElement(node, XML_OutPorts);
				foreach (EventPortOut port in _outPortList)
				{
					XmlNode pNode = portsNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
					portsNode.AppendChild(pNode);
					port.OnWriteToXmlNode(writer, pNode);
				}
			}
			if (_inPortList != null && _inPortList.Count > 0)
			{
				XmlNode portsNode = XmlUtil.CreateSingleNewElement(node, XML_InPorts);
				foreach (EventPortIn port in _inPortList)
				{
					XmlNode pNode = portsNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
					portsNode.AppendChild(pNode);
					port.OnWriteToXmlNode(writer, pNode);
				}
			}
		}
		public void ReadFromXmlNode(XmlObjectReader reader, XmlNode node)
		{
			XmlNode nd;
			object v;
			if (_label == null)
			{
				_label = new DrawingLabel(this);
			}
			if (XmlSerialization.ReadValueFromChildNode(node, XML_RelativePosition, out v))
			{
				_label.RelativePosition.Location = (Point)v;
				nd = node.SelectSingleNode(XML_RelativePosition);
				_label.RelativePosition.IsXto0 = XmlSerialization.GetAttributeBool(nd, "xTo0", true);
				_label.RelativePosition.IsYto0 = XmlSerialization.GetAttributeBool(nd, "yTo0", true);
			}
			//
			XmlNode eNode = node.SelectSingleNode(XML_Event);
			try
			{
				_event = (IEvent)reader.ReadObject(eNode, null);
				_label.Text = _event.ShortDisplayName;
			}
			catch (Exception err)
			{
				MathNode.Log(TraceLogClass.MainForm, err);
			}
			//
			XmlNode ndPos = node.SelectSingleNode(XML_Position);
			if (ndPos != null)
			{
				if (XmlSerialization.ReadValueFromChildNode(ndPos, XML_RelativePosition, out v))
				{
					RelativePosition.Location = (Point)v;
					nd = ndPos.SelectSingleNode(XML_RelativePosition);
					RelativePosition.IsXto0 = XmlSerialization.GetAttributeBool(nd, "xTo0", true);
					RelativePosition.IsYto0 = XmlSerialization.GetAttributeBool(nd, "yTo0", true);
				}
			}
			//
			_outPortList = new List<EventPortOut>();
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", XML_OutPorts, XmlTags.XML_Item));
			foreach (XmlNode n in nds)
			{
				Type t = XmlUtil.GetLibTypeAttribute(n);
				EventPortOut po = (EventPortOut)Activator.CreateInstance(t, this);
				po.Event = this.Event;
				po.OnReadFromXmlNode(reader, n);
				_outPortList.Add(po);
			}
			nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", XML_InPorts, XmlTags.XML_Item));
			if (nds != null && nds.Count > 0)
			{
				_inPortList = new List<EventPortIn>();
				foreach (XmlNode n in nds)
				{
					Type t = XmlUtil.GetLibTypeAttribute(n);
					EventPortIn po = (EventPortIn)Activator.CreateInstance(t, this);
					po.Event = this.Event;
					po.OnReadFromXmlNode(reader, n);
					_inPortList.Add(po);
				}
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
			EventPortOut po = port as EventPortOut;
			if (po == null)
			{
				throw new DesignerException("Cannot add non-EventPortOut to EventIcon");
			}
			if (_outPortList == null)
			{
				_outPortList = new List<EventPortOut>();
			}
			_outPortList.Add(po);
		}

		public void AddInPort(LinkLineNodeInPort port)
		{
			if (_inPortList == null)
			{
				_inPortList = new List<EventPortIn>();
			}
			EventPortIn epi = port as EventPortIn;
			if (epi == null)
			{
				throw new DesignerException("Cannot add non-EventPortIn to EventIcon");
			}
			else
			{
				_inPortList.Add(epi);
			}
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
						return;
					}
				}
			}
			if (_outPortList != null)
			{
				foreach (EventPortOut p in _outPortList)
				{
					if (p == port)
					{
						_outPortList.Remove(p);
						return;
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
				ComponentIcon ci = _owner as ComponentIcon;
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", ci.ClassPointer.Name, _event.DisplayName);
			}
		}

		public uint PortOwnerID
		{
			get { return ActiveDrawingID; }
		}

		#endregion
	}
}
