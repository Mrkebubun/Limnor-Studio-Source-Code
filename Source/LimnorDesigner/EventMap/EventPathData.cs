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
using System.ComponentModel;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner;
using System.Drawing;
using MathExp;
using System.Windows.Forms;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using System.Xml;
using LimnorDesigner.Web;
using LimnorDesigner.Action;
using TraceLog;

namespace LimnorDesigner.EventMap
{
	public class EventPathData
	{
		#region fields and constructors
		private List<ComponentIconEvent> _componentIconList;
		private EventPath _owner;
		public EventPathData()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public List<ComponentIconEvent> ComponentIconList
		{
			get
			{
				if (_componentIconList == null)
					_componentIconList = new List<ComponentIconEvent>();
				return _componentIconList;
			}
			set
			{
				_componentIconList = value;
			}
		}
		public EventPath Owner
		{
			get
			{
				return _owner;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				return _owner.RootPointer;
			}
		}
		#endregion
		#region Methods
		public ComponentIconEvent GetRootComponentIcon()
		{
			if (_componentIconList != null)
			{
				foreach (ComponentIconEvent ci in _componentIconList)
				{
					if (ci.IsRootClass)
					{
						return ci;
					}
				}
			}
			return null;
		}
		public static void MakePortLinks(PortCollection ports)
		{
			foreach (LinkLineNodePort p in ports)
			{
				EventPortIn epi = p as EventPortIn;
				if (epi != null)
				{
					EventPortInFireEvent epife = epi as EventPortInFireEvent;
					//
					bool bLinked = false;
					foreach (LinkLineNodePort p2 in ports)
					{
						bool bMatch = false;
						if (epife != null)
						{
							EventPortOutFirer epof = p2 as EventPortOutFirer;
							if (epof != null)
							{
								//it is a fire-event action
								ComponentIconFireEvent cife = epof.PortOwner as ComponentIconFireEvent;
								if (cife != null)
								{
									FireEventMethod fe1 = epife.FireEventMethod;
									FireEventMethod fe2 = cife.Firer;
									if (fe1 != null && fe2 != null && fe1.EventId == fe2.EventId && fe2.MemberId == fe2.MemberId)
									{
										bMatch = true;
									}
								}
							}
						}
						else
						{
							EventPortOut epo = p2 as EventPortOut;
							if (epo != null)
							{
								if (epi.Event.IsSameObjectRef(epo.Event))
								{
									ComponentIconEvent cieDest = (ComponentIconEvent)(epi.PortOwner);
									if (epo.IsActionExecuter(cieDest))
									{
										bMatch = true;
									}
								}
							}
						}
						if (bMatch)
						{
							if (p2.LinkedPortID == epi.PortID && p2.LinkedPortInstanceID == epi.PortInstanceID
								&& epi.LinkedPortID == p2.PortID && epi.LinkedPortInstanceID == p2.PortInstanceID)
							{
								bLinked = true;
								break;
							}
							else
							{
								//try to fix it
								if (p2.LinkedPortID == 0 && epi.LinkedPortID == 0)
								{
									p2.LinkedPortID = epi.PortID;
									p2.LinkedPortInstanceID = epi.PortInstanceID;
									epi.LinkedPortID = p2.PortID;
									epi.LinkedPortInstanceID = p2.PortInstanceID;
									bLinked = true;
									break;
								}
								else
								{
									LinkLineNodePort lp;
									lp = ports.GetPortByID(epi.LinkedPortID, epi.LinkedPortInstanceID);
									if (lp == null)
									{
										if (p2.LinkedPortID == 0)
										{
											p2.LinkedPortID = epi.PortID;
											p2.LinkedPortInstanceID = epi.PortInstanceID;
											epi.LinkedPortID = p2.PortID;
											epi.LinkedPortInstanceID = p2.PortInstanceID;
											bLinked = true;
											break;
										}
										else
										{
											lp = ports.GetPortByID(p2.LinkedPortID, p2.LinkedPortInstanceID);
											if (lp == null)
											{
												p2.LinkedPortID = epi.PortID;
												p2.LinkedPortInstanceID = epi.PortInstanceID;
												epi.LinkedPortID = p2.PortID;
												epi.LinkedPortInstanceID = p2.PortInstanceID;
												bLinked = true;
												break;
											}
										}
									}
									else
									{
										if (lp.LinkedPortID == epi.PortID && lp.LinkedPortInstanceID == epi.PortInstanceID)
										{
											bLinked = true;
											break;
										}
									}

								}
							}
						}
					}
					if (!bLinked)
					{
						if (epi.LinkedPortID != 0)
						{
							LinkLineNodePort lp;
							lp = ports.GetPortByID(epi.LinkedPortID, epi.LinkedPortInstanceID);
							if (lp == null || lp != epi)
							{
								epi.LinkedPortInstanceID = 0;
								epi.LinkedPortID = 0;
							}
						}
						if (epi.LinkedPortID == 0)
						{
						}
					}
				}
			}
			foreach (LinkLineNodePort p in ports)
			{
				EventPortOut epo = p as EventPortOut;
				if (epo != null)
				{
					if (epo.LinkedPortID != 0)
					{
						if (ports.GetPortByID(epo.LinkedPortID, epo.LinkedPortInstanceID) == null)
						{
							epo.LinkedPortInstanceID = 0;
							epo.LinkedPortID = 0;
						}
					}
				}
			}
		}
		public void ShowLinks()
		{
			List<ComponentIconEvent> iconList = ComponentIconList;
			//collect all ports
			PortCollection pc = new PortCollection();
			foreach (ComponentIconEvent ic in iconList)
			{
				if (ic.Left < 0)
					ic.Left = 2;
				if (ic.Top < 0)
					ic.Top = 2;
				ic.Visible = true;
				ic.BringToFront();
				ic.RefreshLabelPosition();
				//
				if (ic.IsPortOwner)
				{
					//validate the input/output ports of the icon.
					//it will also add EventIcon controls to the parent
					ic.Initialize(this);
					//collect ports from the icon
					pc.AddRange(ic.GetAllPorts());
				}
			}
			//add all ports to the parent
			List<Control> cs = pc.GetAllControls(false);
			_owner.Controls.AddRange(cs.ToArray());
			//apply port locations
			foreach (ComponentIconEvent ic in iconList)
			{
				//output ports
				List<EventIcon> eis = ic.EventIcons;
				if (eis != null && eis.Count > 0)
				{
					foreach (EventIcon ei in eis)
					{
						List<EventPortOut> pos = ei.SourcePorts;
						if (pos != null)
						{
							foreach (EventPortOut po in pos)
							{
								po.RestoreLocation();
								po.SetLoaded();
							}
						}
					}
				}
				//input ports
				List<EventPortIn> epis = ic.DestinationPorts;
				if (epis != null && epis.Count > 0)
				{
					foreach (EventPortIn pi in epis)
					{
						pi.RestoreLocation();
						pi.SetLoaded();
					}
				}
			}
			pc.ValidatePortLinks();
			//link ports
			MakePortLinks(pc);
			//link lines 
			pc.MakeLinks(TraceLogClass.MainForm);
			//set line color
			SetDynamicEventHandlerLineColor(pc);
			//
			pc.CreateLinkLines();
		}
		/// <summary>
		/// remove invalid icons
		/// add missing icons
		/// make links
		/// </summary>
		/// <param name="eventPath"></param>
		public void OnLoadData(EventPath eventPath)
		{
			_owner = eventPath;
			//all icons saved in XML
			List<ComponentIconEvent> iconList = ComponentIconList;
			//all components, each corresponding to one icon
			ClassPointer root = _owner.Loader.GetRootId();
			List<IClass> objList = root.GetClassList();
			//all custom methods, each corresponding to one icon
			Dictionary<string, MethodClass> methods = root.CustomMethods;
			//all event handlers, each HandlerMathodID object corresponds to one icon
			List<EventAction> handlers = root.EventHandlers;
			//all custom properties 
			Dictionary<string, PropertyClass> props = root.CustomProperties;
			//all used html elements
			IList<HtmlElement_BodyBase> htmlElements = null;
			if (root.IsWebPage)
			{
				htmlElements = root.UsedHtmlElements;
			}
			//
			//remove invalid icons
			List<ComponentIconEvent> iconInvalid = new List<ComponentIconEvent>();
			foreach (ComponentIconEvent ic in iconList)
			{
				if (!ic.OnDeserialize(root, _owner.Loader))
				{
					iconInvalid.Add(ic);
				}
			}
			foreach (ComponentIconEvent ic in iconInvalid)
			{
				iconList.Remove(ic);
			}
			//remove invalid events. unknown reason causing invalid events
			foreach (ComponentIconEvent ic in iconList)
			{
				if (ic.MemberId != 0 && ic.GetType().Equals(typeof(ComponentIconEvent)))
				{
					MemberComponentId mid0 = ic.ClassPointer as MemberComponentId;
					if (mid0 != null)
					{
						List<EventIcon> eis = ic.EventIcons;
						if (eis != null && eis.Count > 0)
						{
							List<EventIcon> invalidEis = new List<EventIcon>();
							foreach (EventIcon ei in eis)
							{
								if (ei.Event != null)
								{
									MemberComponentId mid = ei.Event.Owner as MemberComponentId;
									if (mid != null && mid.MemberId != 0)
									{
										if (mid.MemberId != ic.MemberId)
										{
											invalidEis.Add(ei);
										}
									}
								}
							}
							foreach (EventIcon ei in invalidEis)
							{
								eis.Remove(ei);
							}
						}
					}
				}
			}
			//
			//add new icons
			int x0 = 20;
			int y0 = 20;
			int x = x0;
			int y = y0;
			int dx = 30;
			int dy = 30;
			//add missing component icon
			foreach (IClass c in objList)
			{
				bool bFound = false;
				foreach (ComponentIconEvent ic in iconList)
				{
					if (ic.IsForComponent)
					{
						if (ic.MemberId == c.MemberId)
						{
							ic.Designer = _owner.Loader;
							bFound = true;
							break;
						}
					}
				}
				if (!bFound)
				{
					ComponentIconEvent cip;
					if (c is HtmlElement_Base)
					{
						cip = new ComponentIconHtmlElement();
					}
					else
					{
						cip = new ComponentIconEvent();
					}
					cip.Init(_owner.Loader, c);
					cip.Location = new Point(x, y);
					x += dx;
					x += cip.Width;
					if (x > _owner.Width)
					{
						x = x0;
						y += dy;
						y += cip.Height;
					}
					iconList.Add(cip);
				}
			}
			ComponentIconEvent rootIcon = null;
			foreach (ComponentIconEvent ci in iconList)
			{
				if (ci.IsForComponent)
				{
					if (ci.IsRootClass)
					{
						rootIcon = ci;
						break;
					}
				}
			}
			if (rootIcon == null)
			{
				throw new DesignerException("Root component icon not found for class {0}", root.ClassId);
			}
#if USEHTMLEDITOR
			//add missing html element
			if (htmlElements != null)
			{
				foreach (HtmlElement_Base he in htmlElements)
				{
					bool bFound = false;
					foreach (ComponentIconEvent ic in iconList)
					{
						ComponentIconHtmlElement cihe = ic as ComponentIconHtmlElement;
						if (cihe != null)
						{
							if (ic.MemberId == cihe.MemberId)
							{
								ic.Designer = _owner.Loader;
								bFound = true;
								break;
							}
						}
					}
					if (!bFound)
					{
						ComponentIconHtmlElement cip = new ComponentIconHtmlElement();
						cip.Init(_owner.Loader, he);
						cip.Location = new Point(x, y);
						x += dx;
						x += cip.Width;
						if (x > _owner.Width)
						{
							x = x0;
							y += dy;
							y += cip.Height;
						}
						iconList.Add(cip);
					}
				}
			}
			//add missing current html element
			if (root.IsWebPage)
			{
				bool bFound = false;
				foreach (ComponentIconEvent ic in iconList)
				{
					ComponentIconHtmlElementCurrent cic = ic as ComponentIconHtmlElementCurrent;
					if (cic != null)
					{
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					HtmlElement_body heb = new HtmlElement_body(root);
					ComponentIconHtmlElementCurrent cip = new ComponentIconHtmlElementCurrent();
					cip.ClassPointer = heb;
					cip.Init(_owner.Loader, heb);
					cip.Location = new Point(x, y);
					x += dx;
					x += cip.Width;
					if (x > _owner.Width)
					{
						x = x0;
						y += dy;
						y += cip.Height;
					}
					iconList.Add(cip);
				}
			}
#endif
			//add missing FireEventMethod, ComponentIconClass and ComponentIconClassType
			Dictionary<UInt32, IAction> actions = root.GetActions();
			foreach (IAction act in actions.Values)
			{
				if (act != null)
				{
					FireEventMethod fe = act.ActionMethod as FireEventMethod;
					if (fe != null)
					{
						ComponentIconFireEvent cife = null;
						foreach (ComponentIconEvent ic in iconList)
						{
							if (!ic.IsForComponent)
							{
								ComponentIconFireEvent cie = ic as ComponentIconFireEvent;
								if (cie != null)
								{
									if (fe.MemberId == cie.FirerId && fe.EventId == cie.EventId)
									{
										cie.Firer = fe;
										ic.Designer = _owner.Loader;
										cife = cie;
										break;
									}
								}
							}
						}
						if (cife == null)
						{
							cife = new ComponentIconFireEvent();
							cife.Firer = fe;
							cife.Init(_owner.Loader, root);
							cife.Location = new Point(x, y);
							//cip.FirerPort 
							x += dx;
							x += cife.Width;
							if (x > _owner.Width)
							{
								x = x0;
								y += dy;
								y += cife.Height;
							}
							iconList.Add(cife);
						}
						//make port link
						EventPortInFireEvent epife = null;
						EventIcon ei = rootIcon.GetEventIcon(fe.Event);
						if (ei == null)
						{
							ei = new EventIcon(rootIcon);
							ei.Event = fe.Event;
							ComponentIconEvent.SetInitialPosition(100, ei);
							rootIcon.EventIcons.Add(ei);
						}
						List<EventPortIn> ports = ei.DestinationPorts;
						foreach (EventPortIn epi in ports)
						{
							EventPortInFireEvent epife0 = epi as EventPortInFireEvent;
							if (epife0 != null)
							{
								if (epife0.FireEventMethodId == fe.MemberId)
								{
									epife0.FireEventMethod = fe;
									epife = epife0;
									break;
								}
							}
						}
						if (epife == null)
						{
							epife = new EventPortInFireEvent(ei);
							epife.FireEventMethod = fe;
							ei.DestinationPorts.Add(epife);

						}
						epife.LinkedPortID = cife.FirerPort.PortID;
						epife.LinkedPortInstanceID = cife.FirerPort.PortInstanceID;
						cife.FirerPort.LinkedPortID = epife.PortID;
						cife.FirerPort.LinkedPortInstanceID = epife.PortInstanceID;
						//
						epife.RestoreLocation();
						epife.SetLoaded();
						//
						cife.FirerPort.RestoreLocation();
						cife.FirerPort.SetLoaded();
					}
					else if (act.ActionMethod != null)
					{
						ClassPointer cp = root.GetExternalExecuterClass(act);
						if (cp != null)
						{
							ComponentIconClass cic = null;
							foreach (ComponentIconEvent ic in iconList)
							{
								ComponentIconClass cic0 = ic as ComponentIconClass;
								if (cic0 != null)
								{
									if (cic0.ClassId == act.ExecuterClassId)
									{
										cic = cic0;
										cic.Designer = _owner.Loader;
										break;
									}
								}
							}
							if (cic == null)
							{
								cic = new ComponentIconClass();
								cic.Init(_owner.Loader, cp);
								cic.Location = new Point(x, y);
								x += dx;
								x += cic.Width;
								if (x > _owner.Width)
								{
									x = x0;
									y += dy;
									y += cic.Height;
								}
								iconList.Add(cic);
							}
						}
						else
						{
							DataTypePointer tp = act.ActionMethod.Owner as DataTypePointer;
							if (tp != null)
							{
								ComponentIconClassType cict = null;
								foreach (ComponentIconEvent ic in iconList)
								{
									ComponentIconClassType cict0 = ic as ComponentIconClassType;
									if (cict0 != null)
									{
										if (cict0.ClassType.Equals(tp.BaseClassType))
										{
											cict = cict0;
											cict.Designer = _owner.Loader;
											break;
										}
									}
								}
								if (cict == null)
								{
									cict = new ComponentIconClassType();
									cict.Init(_owner.Loader, tp);
									cict.Location = new Point(x, y);
									x += dx;
									x += cict.Width;
									if (x > _owner.Width)
									{
										x = x0;
										y += dy;
										y += cict.Height;
									}
									iconList.Add(cict);
								}
							}
						}
					}
					else
					{
						ActionAttachEvent aae = act as ActionAttachEvent;
						if (aae != null)
						{
							IObjectPointer eventOwner = aae.EventOwner;
							if (eventOwner != null)
							{
							}
						}
					}
				}
			}
			//add missing method icon
			foreach (MethodClass mc in methods.Values)
			{
				bool bFound = false;
				foreach (ComponentIconEvent ic in iconList)
				{
					if (!ic.IsForComponent)
					{
						ComponentIconMethod cie = ic as ComponentIconMethod;
						if (cie != null)
						{
							if (mc.MethodID == cie.MethodId)
							{
								ic.Designer = _owner.Loader;
								bFound = true;
								break;
							}
						}
					}
				}
				if (!bFound)
				{
					ComponentIconMethod cip = new ComponentIconMethod();
					cip.Init(_owner.Loader, mc.RootPointer);
					cip.Method = mc;
					cip.MethodId = mc.MethodID;
					cip.Location = new Point(x, y);
					x += dx;
					x += cip.Width;
					if (x > _owner.Width)
					{
						x = x0;
						y += dy;
						y += cip.Height;
					}
					iconList.Add(cip);
				}
			}
			//add missing handler icon
			foreach (EventAction ea in handlers)
			{
				if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
				{
					foreach (TaskID tid in ea.TaskIDList)
					{
						HandlerMethodID hid = tid as HandlerMethodID;
						if (hid != null)
						{
							bool bFound = false;
							foreach (ComponentIconEvent ic in iconList)
							{
								ComponentIconEventhandle cie = ic as ComponentIconEventhandle;
								if (cie != null)
								{
									if (ic.MemberId == cie.MemberId && cie.MethodId == hid.ActionId)
									{
										ic.Designer = _owner.Loader;
										bFound = true;
										break;
									}
								}
							}
							if (!bFound)
							{
								ComponentIconEventhandle cip = new ComponentIconEventhandle();
								cip.Init(_owner.Loader, root);
								cip.Method = hid.HandlerMethod;
								cip.MethodId = hid.ActionId;
								cip.Location = new Point(x, y);
								x += dx;
								x += cip.Width;
								if (x > _owner.Width)
								{
									x = x0;
									y += dy;
									y += cip.Height;
								}
								iconList.Add(cip);
							}
						}
					}
				}
			}
			//add missing property icon
			foreach (PropertyClass p in props.Values)
			{
				bool bFound = false;
				foreach (ComponentIconEvent ic in iconList)
				{
					if (!ic.IsForComponent)
					{
						ComponentIconProperty cie = ic as ComponentIconProperty;
						if (cie != null)
						{
							if (p.MemberId == cie.PropertyId)
							{
								ic.Designer = _owner.Loader;
								bFound = true;
								break;
							}
						}
					}
				}
				if (!bFound)
				{
					ComponentIconProperty cip = new ComponentIconProperty();
					cip.Init(_owner.Loader, root);
					cip.Property = p;
					cip.PropertyId = p.MemberId;
					cip.Location = new Point(x, y);
					x += dx;
					x += cip.Width;
					if (x > _owner.Width)
					{
						x = x0;
						y += dy;
						y += cip.Height;
					}
					iconList.Add(cip);
				}
			}

			//add icons to the parent
			_owner.Controls.AddRange(iconList.ToArray());
			//collect all ports
			PortCollection pc = new PortCollection();
			foreach (ComponentIconEvent ic in iconList)
			{
				if (ic.Left < 0)
					ic.Left = 2;
				if (ic.Top < 0)
					ic.Top = 2;
				ic.Visible = true;
				ic.BringToFront();
				ic.RefreshLabelPosition();
				//
				if (ic.IsPortOwner)
				{
					//validate the input/output ports of the icon.
					//it will also add EventIcon controls to the parent
					ic.Initialize(this);
					//collect ports from the icon
					pc.AddRange(ic.GetAllPorts());
				}
			}
			//add all ports to the parent
			List<Control> cs = pc.GetAllControls(false);
			_owner.Controls.AddRange(cs.ToArray());
			//apply port locations
			foreach (ComponentIconEvent ic in iconList)
			{
				//output ports
				List<EventIcon> eis = ic.EventIcons;
				if (eis != null && eis.Count > 0)
				{
					foreach (EventIcon ei in eis)
					{
						List<EventPortOut> pos = ei.SourcePorts;
						if (pos != null)
						{
							foreach (EventPortOut po in pos)
							{
								po.RestoreLocation();
								po.SetLoaded();
							}
						}
					}
				}
				//input ports
				List<EventPortIn> epis = ic.DestinationPorts;
				if (epis != null && epis.Count > 0)
				{
					foreach (EventPortIn pi in epis)
					{
						pi.RestoreLocation();
						pi.SetLoaded();
					}
				}
			}
			pc.ValidatePortLinks();
			//link ports
			MakePortLinks(pc);
			//link lines 
			pc.MakeLinks(TraceLogClass.MainForm);
			//set line color
			SetDynamicEventHandlerLineColor(pc);
			//
			pc.CreateLinkLines();
			//
			//remove unlinked inports
			foreach (ComponentIconEvent cieSource in iconList)
			{
				List<EventPortIn> epis = cieSource.DestinationPorts;
				if (epis != null && epis.Count > 0)
				{
					List<EventPortIn> unlinked = new List<EventPortIn>();
					foreach (EventPortIn ei in epis)
					{
						if (ei.LinkedOutPort == null)
						{
							unlinked.Add(ei);
						}
					}
					if (unlinked.Count > 0)
					{
						foreach (EventPortIn ei in unlinked)
						{
							epis.Remove(ei);
							if (ei.Parent != null)
							{
								ei.Parent.Controls.Remove(ei);
							}
						}
					}
				}
			}
			//monitor and notify link line changes
			eventPath.SetupLineNodeMonitor();
			//
			//remedy for unsolved bug: remove duplicated ComponentIconEvent
			Dictionary<UInt64, List<ComponentIconEvent>> cieList = new Dictionary<UInt64, List<ComponentIconEvent>>();
			foreach (ComponentIconEvent cieSource in iconList)
			{
				if (typeof(ComponentIconEvent).Equals(cieSource.GetType()))
				{
					UInt64 id = cieSource.WholeId;
					List<ComponentIconEvent> list;
					if (!cieList.TryGetValue(id, out list))
					{
						list = new List<ComponentIconEvent>();
						cieList.Add(id, list);
					}
					list.Add(cieSource);
				}
			}
			foreach (KeyValuePair<UInt64, List<ComponentIconEvent>> kv in cieList)
			{
				if (kv.Value.Count > 1)
				{
					//duplicate icons. Find those can be removed
					List<ComponentIconEvent> deleting = new List<ComponentIconEvent>();
					foreach (ComponentIconEvent cie in kv.Value)
					{
						if (cie.DestinationPorts.Count == 0)
						{
							if (cie.EventIcons.Count == 0)
							{
								deleting.Add(cie);
							}
						}
					}
					if (deleting.Count == kv.Value.Count)
					{
						deleting.RemoveAt(0);
					}
					foreach (ComponentIconEvent cie in deleting)
					{
						cie.Parent.Controls.Remove(cie.Label);
						cie.Parent.Controls.Remove(cie);
						_componentIconList.Remove(cie);
						if (cie.DataXmlNode != null)
						{
							XmlNode xmp = cie.DataXmlNode.ParentNode;
							xmp.RemoveChild(cie.DataXmlNode);
						}
					}
				}
			}
			eventPath.EnlargeForChildren();
		}
		#endregion
		#region static utility
		public static void SetDynamicEventHandlerLineColor(PortCollection pc)
		{
			foreach (LinkLineNodePort p in pc)
			{
				LinkLineNodeInPort ip = p as LinkLineNodeInPort;
				if (ip != null)
				{
					ComponentIconEventhandle ieh = ip.PortOwner as ComponentIconEventhandle;
					if (ieh != null)
					{
						EventHandlerMethod ehm = ieh.Method as EventHandlerMethod;
						if (ehm != null)
						{
							UInt32 actId = ehm.GetDynamicActionId();
							if (actId != 0)
							{
								ip.SetLineColor(Color.Yellow);
							}
						}
					}
				}
				else
				{
					EventPortOutExecuteMethod op = p as EventPortOutExecuteMethod;
					if (op != null)
					{
						EventHandlerMethod ehm = op.Method as EventHandlerMethod;
						if (ehm != null)
						{
							UInt32 actId = ehm.GetDynamicActionId();
							if (actId != 0)
							{
								op.SetLineColor(Color.Yellow);
							}
						}
					}
				}
			}
		}
		public static Dictionary<string, List<EventAction>> CreateHandlerGroup(List<EventAction> ehs)
		{
			Dictionary<string, List<EventAction>> eaGroup = new Dictionary<string, List<EventAction>>();
			foreach (EventAction ea in ehs)
			{
				List<EventAction> eas;
				if (!eaGroup.TryGetValue(ea.Event.ObjectKey, out eas))
				{
					eas = new List<EventAction>();
					eaGroup.Add(ea.Event.ObjectKey, eas);
				}
				eas.Add(ea);
			}
			return eaGroup;
		}
		/// <summary>
		/// EventPath only support the first level custom class instance
		/// </summary>
		/// <param name="pointer"></param>
		/// <returns></returns>
		public static IClass GetEventFirerRef(IObjectPointer pointer)
		{
			IClass ic = pointer as IClass;
			while (pointer != null)
			{
				if (ic != null)
				{
					if (ic.Owner == null || (ic.Owner is ClassPointer))
					{
						return ic;
					}
				}
				pointer = pointer.Owner;
				ic = pointer as IClass;
			}

			return ic;
		}
		#endregion
	}
}
