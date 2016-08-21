/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Event;
using XmlUtility;
using XmlSerializer;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using MathExp;
using VPL;
using WindowsUtility;

namespace LimnorDesigner.EventMap
{
	/// <summary>
	/// represent a fire-event action
	/// </summary>
	public class ComponentIconFireEvent : ComponentIconEvent, IEventMapSource
	{
		#region fields and constructors
		private UInt32 _eventId;
		private UInt32 _firerId;
		private FireEventMethod _firer;
		private EventPortOutFirer _firerPort;
		private IAction _act;
		public ComponentIconFireEvent()
		{
			SetIconImage(Resources._createEventFireAction.ToBitmap());
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override bool IsForComponent
		{
			get
			{
				return false;
			}
		}
		public override List<EventPortOut> SourcePorts
		{
			get
			{
				List<EventPortOut> eps = new List<EventPortOut>();
				eps.Add(FirerPort);
				return eps;
			}
		}
		public EventPortOutFirer FirerPort
		{
			get
			{
				if (_firerPort == null)
				{
					_firerPort = new EventPortOutFirer(this);
					if (_firer != null)
					{
						_firerPort.Event = _firer.Event;
					}
				}
				return _firerPort;
			}
		}
		public FireEventMethod Firer
		{
			get
			{
				return _firer;
			}
			set
			{
				_firer = value;
				if (_firer != null)
				{
					_eventId = _firer.EventId;
					_firerId = _firer.MemberId;
					SetLabelText(_firer.Name);
				}
			}
		}
		public UInt32 EventId
		{
			get
			{
				return _eventId;
			}
		}
		public UInt32 FirerId
		{
			get
			{
				return _firerId;
			}
		}
		#endregion
		#region private methods
		private void mi_delete(object sender, EventArgs e)
		{
			if (_act != null)
			{
				EventPath ep = this.Parent as EventPath;
				if (ep != null)
				{
					ClassPointer root = ep.Loader.GetRootId();
					root.AskDeleteAction(_act, this.FindForm());
				}
			}
		}
		#endregion
		#region Methods
		public override bool IsActionExecuter(IAction act, ClassPointer root)
		{
			FireEventMethod fe = act.ActionMethod as FireEventMethod;
			if (fe != null)
			{
				if (fe.MemberId == this.FirerId && fe.EventId == this.EventId)
				{
					return true;
				}
			}
			return false;
		}
		protected override List<Control> GetRelatedControls()
		{
			List<Control> ctrls = base.GetRelatedControls();
			if (_firerPort != null)
			{
				LinkLineNode l = _firerPort;
				ctrls.Add(l);
				if (_firerPort.Label != null && _firerPort.Label.Parent != null)
				{
					ctrls.Add(_firerPort.Label);
				}
				l = l.NextNode as LinkLineNode;
				while (l != null)
				{
					LinkLineNodePort p = l as LinkLineNodePort;
					if (p != null && p.Label != null && p.Label.Parent != null)
					{
						ctrls.Add(p.Label);
					}
					ctrls.Add(l);
					l = l.NextNode as LinkLineNode;
				}
			}
			return ctrls;
		}
		public override void RemoveIcon()
		{
			if (FirerPort != null && FirerPort.LinkedInPort != null)
			{
				EventIcon ei = FirerPort.LinkedInPort.PortOwner as EventIcon;
				if (ei != null)
				{
					ei.RemoveInPort(FirerPort.LinkedInPort);
				}
			}
			base.RemoveIcon();
		}
		protected override void OnSelectByMouseDown()
		{
			if (_act != null)
			{
				DesignerPane.Loader.NotifySelection(_act);
			}
			if (_firer != null)
			{
				DesignerPane.PaneHolder.OnFireEventActionSelected(_firer);
			}
		}
		/// <summary>
		/// link: from this action to the event
		/// it does not have in-ports
		/// it has one out-port linking to the event
		/// </summary>
		/// <param name="eventData"></param>
		public override void Initialize(EventPathData eventData)
		{
			List<EventPortIn> inports = this.DestinationPorts;
			List<IEvent> events = new List<IEvent>();
			ClassPointer root = eventData.Owner.RootPointer;
			if (_firer != null)
			{
				List<EventAction> eas = root.EventHandlers;
				foreach (EventAction ea in eas)
				{
					if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
					{
						//this event handling is not empty
						//check to see if there is an event-firing action using this firer
						//if there is one then this event should be linked to this firer
						//that is, this ea causes the firer to fire another event
						foreach (TaskID tid in ea.TaskIDList)
						{
							if (tid.Action == null)
							{
								tid.LoadActionInstance(root);
							}
							if (tid.Action != null)
							{
								FireEventMethod fe = tid.Action.ActionMethod as FireEventMethod;
								if (fe != null)
								{
									if (fe.EventId == _firer.EventId && fe.MemberId == _firer.MemberId)
									{
										events.Add(ea.Event);
										break;
									}
								}
							}
						}
					}
				}
			}
			List<EventPortIn> invalid = new List<EventPortIn>();
			foreach (EventPortIn epi in inports)
			{
				bool found = false;
				foreach (IEvent e in events)
				{
					if (e.IsSameObjectRef(epi.Event))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					invalid.Add(epi);
				}
			}
			if (invalid.Count > 0)
			{
				foreach (EventPortIn epi in invalid)
				{
					inports.Remove(epi);
				}
			}
			foreach (IEvent e in events)
			{
				bool found = false;
				foreach (EventPortIn epi in inports)
				{
					if (e.IsSameObjectRef(epi.Event))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					EventPortIn epi = new EventPortIn(this);
					epi.Event = e;
					epi.SetLoaded();
					inports.Add(epi);
				}
			}
		}
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			Dictionary<UInt32, IAction> actions = root.GetActions();
			foreach (IAction act in actions.Values)
			{
				if (act != null)
				{
					FireEventMethod fe = act.ActionMethod as FireEventMethod;
					if (fe != null)
					{
						if (fe.EventId == _eventId && fe.MemberId == _firerId)
						{
							_firer = fe;
							_act = act;
							SetLabelText(_firer.Name);
							Init(designer, root);
							FirerPort.RestoreLocation();
							FirerPort.SetLoaded();
							return true;
						}
					}
				}
			}
			return false;
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			MenuItemWithBitmap mi;
			mi = new MenuItemWithBitmap("Delete", mi_delete, Resources._cancel.ToBitmap());
			mnu.MenuItems.Add(mi);
		}
		protected override void OnSetImage()
		{
		}
		const string XMLATT_eventId = "eventId";
		const string XMLATT_firerId = "firerId";
		const string XML_FirerPort = "OutPort";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_eventId, _eventId);
			XmlUtil.SetAttribute(node, XMLATT_firerId, _firerId);
			if (_firerPort != null)
			{
				XmlNode portNode = XmlUtil.CreateSingleNewElement(node, XML_FirerPort);
				_firerPort.OnWriteToXmlNode(writer, portNode);
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			_eventId = XmlUtil.GetAttributeUInt(node, XMLATT_eventId);
			_firerId = XmlUtil.GetAttributeUInt(node, XMLATT_firerId);
			XmlNode portNode = node.SelectSingleNode(XML_FirerPort);
			if (portNode != null)
			{
				_firerPort = new EventPortOutFirer(this);
				_firerPort.OnReadFromXmlNode(reader, portNode);
			}
		}
		#endregion
		#region IEventMapSource Members

		public ClassPointer RootPointer
		{
			get { return RootClassPointer; }
		}

		#endregion
	}
}
