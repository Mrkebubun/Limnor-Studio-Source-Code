/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using VPL;
using VPLDrawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using System.Drawing.Drawing2D;
using System.Xml;
using XmlUtility;
using LimnorDesigner.Event;
using XmlSerializer;
using LimnorDesigner.Property;
using LimnorDesigner.Action;

namespace LimnorDesigner.EventMap
{
	/// <summary>
	/// source for an event->actionExecuter link, 
	/// represents one actionExecuter on source side.
	/// </summary>
	[UseParentObject]
	public abstract class EventPortOut : LinkLineNodeOutPort
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.Yellow;
		private IEvent _event;
		private List<TaskID> _actions;
		private Point _relativeLocation;
		private bool _moving;
		private bool _loaded;
		public EventPortOut(IEventMapSource owner)
			: base(owner)
		{
			this.Size = new System.Drawing.Size(ComponentIconEvent.PortSize, ComponentIconEvent.PortSize);
			LabelVisible = false;
			//
			Control c = (Control)owner;
			Owner = c;
			c.Move += new EventHandler(ei_Move);
		}

		void ei_Move(object sender, EventArgs e)
		{
			if (this.Parent != null && _loaded)
			{
				if (_relativeLocation != Point.Empty)
				{
					Control ei = sender as Control;
					if (ei != null)
					{
						_moving = true;
						this.Location = new Point(ei.Location.X - _relativeLocation.X, ei.Location.Y - _relativeLocation.Y);
						_moving = false;
					}
				}
			}
		}
		#endregion
		#region properties
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public override enumPositionType PositionType
		{
			get
			{
				return enumPositionType.Circle;
			}
			set
			{
			}
		}
		public Point RelativeLocation
		{
			get
			{
				return _relativeLocation;
			}
			set
			{
				_relativeLocation = value;
			}
		}
		[Description("Event name")]
		public IEvent Event
		{
			get
			{
				return _event;
			}
			set
			{
				_event = value;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				if (_event != null)
				{
					if (_event.RootPointer != null)
					{
						return _event.RootPointer;
					}
				}
				IEventMapSource ei = this.PortOwner as IEventMapSource;
				if (ei != null)
				{
					return ei.RootPointer;
				}
				return null;
			}
		}
		public List<TaskID> Actions
		{
			get
			{
				if (_actions == null)
				{
					_actions = new List<TaskID>();
				}
				return _actions;
			}
		}
		public int ActionCount
		{
			get
			{
				if (_actions == null)
				{
					return 0;
				}
				return _actions.Count;
			}
		}
		#endregion
		#region Static Methods
		public static void LinkPorts(List<EventPortOut> ports, List<ComponentIconEvent> iconList)
		{
			foreach (EventPortOut po in ports)
			{
				bool poLinked = false;
				foreach (ComponentIconEvent cieAction in iconList)
				{
					if (po.IsActionExecuter(cieAction))
					{
						List<EventPortIn> ins = cieAction.DestinationPorts;
						if (ins != null && ins.Count > 0)
						{
							foreach (EventPortIn pi in ins)
							{
								if (po.Event.IsSameObjectRef(pi.Event))
								{
									pi.LinkedPortID = po.PortID;
									pi.LinkedPortInstanceID = po.PortInstanceID;
									po.LinkedPortID = pi.PortID;
									po.LinkedPortInstanceID = pi.PortInstanceID;
									poLinked = true;
									break;
								}
							}
							if (poLinked)
							{
								break;
							}
						}
					}
				}
				if (!poLinked)
				{
					MathNode.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Mapping of event -> action executer not found: {0}", po.ToString()));
				}
			}
		}
		public static EventPortOut CreateOutPort(TaskID a, EventIcon owner)
		{
			EventPortOut po = null;
			CustomMethodPointer cmp = null;
			HandlerMethodID hid = a as HandlerMethodID;
			if (hid != null)
			{
				EventPortOutExecuteMethod em = new EventPortOutExecuteMethod(owner);
				em.SetMethod(hid.HandlerMethod);
				po = em;
			}
			else
			{
				if (a.Action == null)
				{
				}
				else
				{
					IEventMapSource eir = owner as IEventMapSource;
					ClassPointer root = eir.RootPointer;
					ClassPointer cpre = root.GetExternalExecuterClass(a.Action);
					if (a.Action.IsStatic && cpre != null)
					{
						EventPortOutClassTypeAction pct = new EventPortOutClassTypeAction(owner);
						pct.SetOwnerClassPointer(cpre);
						po = pct;
					}
					else if (a.Action.ActionMethod != null)
					{
						DataTypePointer dtp = a.Action.ActionMethod.Owner as DataTypePointer;
						if (dtp != null)
						{
							EventPortOutTypeAction pot = new EventPortOutTypeAction(owner);
							pot.SetOwnerType(dtp.BaseClassType);
							po = pot;
						}
						else
						{
							cmp = a.Action.ActionMethod as CustomMethodPointer;
							if (cmp != null && cmp.Holder.DefinitionClassId == a.ClassId)
							{
								EventPortOutExecuteMethod em = new EventPortOutExecuteMethod(owner);
								MethodClass mc = cmp.MethodPointed as MethodClass;
								if (mc != null)
								{
									em.SetMethod(mc);
								}
								po = em;
							}
							else
							{
								po = null;
								SetterPointer sp = a.Action.ActionMethod as SetterPointer;
								if (sp != null)
								{
									CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
									if (cpp != null)
									{
										EventPortOutSetProperty posp = new EventPortOutSetProperty(owner);
										posp.SetProperty(cpp.Property);
										posp.PropertyId = cpp.MemberId;
										po = posp;
									}
								}
								if (po == null)
								{
									EventPortOutExecuter pe = new EventPortOutExecuter(owner);
									pe.ActionExecuterId = a.Action.ExecuterMemberId;
									po = pe;
								}
							}
						}
					}
					else
					{
						ActionAttachEvent aae = a.Action as ActionAttachEvent;
						if (aae != null)
						{
							EventPortOutExecuter pe = new EventPortOutExecuter(owner);
							pe.ActionExecuterId = a.Action.ExecuterMemberId;
							po = pe;
						}
					}
				}
			}
			if (po != null)
			{
				po.AddAction(a);
				ActiveDrawing ei = owner as ActiveDrawing;
				double x, y;
				ComponentIconEvent.CreateRandomPoint(ei.Width + ComponentIconEvent.PortSize, out x, out y);
				po.Location = new Point((int)(ei.Center.X + x), (int)(ei.Center.Y + y));
				po.SetLoaded();
				po.SaveLocation();
			}
			return po;
		}
		#endregion
		#region methods
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOut po = (EventPortOut)base.CreateDuplicateOutPort();
			EventIcon ei = (EventIcon)po.PortOwner;
			ei.AddOutPort(po);
			return po;
		}
		const string XMLATTR_RELX = "relX";
		const string XMLATTR_RELY = "relY";
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			base.OnWriteToXmlNode(serializer, node);
			if (_relativeLocation != Point.Empty)
			{
				XmlUtil.SetAttribute(node, XMLATTR_RELX, _relativeLocation.X);
				XmlUtil.SetAttribute(node, XMLATTR_RELY, _relativeLocation.Y);
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			int x = XmlUtil.GetAttributeInt(node, XMLATTR_RELX);
			int y = XmlUtil.GetAttributeInt(node, XMLATTR_RELY);
			_relativeLocation = new Point(x, y);
		}
		public override void SaveLocation()
		{
			if (_loaded)
			{
				if (!_moving && !AdjustingPosition)
				{
					base.SaveLocation();
					Control ei = PortOwner as Control;
					_relativeLocation = new Point(ei.Location.X - this.Location.X, ei.Location.Y - this.Location.Y);
				}
			}
		}
		public override void RestoreLocation()
		{
			Control ei = PortOwner as Control;
			this.Location = new Point(ei.Location.X - _relativeLocation.X, ei.Location.Y - _relativeLocation.Y);
		}
		protected override enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			return enumPositionType.Circle;
		}
		protected override void OnMove(EventArgs e)
		{
			if (_loaded)
			{
				if (!_moving && !AdjustingPosition)
				{
					SaveLocation();
				}
				base.OnMove(e);
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			GraphicsState st = e.Graphics.Save();
			ActiveDrawing ei = PortOwner as ActiveDrawing;
			Point p0 = ei.Center;
			Point p1 = this.Center;
			double a;
			if (p0.Y == p1.Y)
			{
				if (p1.X > p0.X)
				{
					a = -Math.PI / 2.0;
				}
				else
				{
					a = Math.PI / 2.0;
				}
			}
			else
			{
				a = -Math.Atan2((double)(p1.X - p0.X), ((double)(p1.Y - p0.Y)));
			}
			//move to the center of this port icon. (0,0) co-ordinates is at the left-top of the port icon.
			e.Graphics.TranslateTransform(this.Size.Width / 2, this.Size.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Prepend);
			//rotate
			e.Graphics.RotateTransform((float)(a * 180.0 / Math.PI));
			//move to the left-top corner
			e.Graphics.TranslateTransform(-this.Size.Width / 2, -this.Size.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Prepend);
			//do the drawing
			VplDrawing.DrawOutArrow(e.Graphics, _drawBrush, this.Size, enumPositionType.Bottom);
			//restore the transformations
			e.Graphics.Restore(st);
		}

		protected void SetActions(List<TaskID> acts)
		{
			_actions = acts;
		}
		public override void SetLoaded()
		{
			_loaded = true;
		}
		public bool AddAction(TaskID act)
		{
			if (_actions == null)
			{
				_actions = new List<TaskID>();
			}
			foreach (TaskID tid in _actions)
			{
				if (tid.TaskId == act.TaskId)
				{
					return false;
				}
			}
			_actions.Add(act);
			return true;
		}
		public abstract bool CanActionBeLinked(TaskID act);
		public abstract bool IsActionExecuter(ComponentIconEvent obj);
		public void RefreshActionlist(EventAction ea, ClassPointer root)
		{
			List<TaskID> acts = new List<TaskID>();
			foreach (TaskID tid in ea.TaskIDList)
			{
				if (!tid.IsEmbedded)
				{
					if (CanActionBeLinked(tid))
					{
						acts.Add(tid);
					}
				}
			}
			SetActions(acts);
		}
		public bool IsForTheAction(TaskID taskId)
		{
			List<TaskID> acts = this.Actions;
			if (acts != null && acts.Count > 0)
			{
				foreach (TaskID a in acts)
				{
					if (a.TaskId == taskId.TaskId)
					{
						return true;
					}
				}
			}
			return false;
		}
		public bool RemoveAction(TaskID tid)
		{
			if (_actions != null)
			{
				foreach (TaskID t in _actions)
				{
					if (t.IsSameTask(tid))
					{
						_actions.Remove(t);
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
	/// <summary>
	/// execute a lib method
	/// </summary>
	public class EventPortOutExecuter : EventPortOut
	{
		#region fields and constructors
		const string XMLATTR_ExecuterId = "executerId";
		public EventPortOutExecuter(EventIcon owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		/// <summary>
		/// memberId of the component
		/// </summary>
		public UInt32 ActionExecuterId
		{
			get;
			set;
		}
		#endregion
		#region Methods
		public override bool CanActionBeLinked(TaskID act)
		{
			if (act.Action == null)
			{
				act.LoadActionInstance(this.RootPointer);
			}
			ActionAttachEvent aae = act.Action as ActionAttachEvent;
			if (aae != null)
			{
				return (aae.ExecuterMemberId == ActionExecuterId);
			}
			if (act.Action != null && act.Action.ActionMethod != null)
			{
				ClassPointer cpr = this.RootPointer.GetExternalExecuterClass(act.Action);
				if (cpr != null)
				{
					return false;
				}
				DataTypePointer dtp = act.Action.ActionMethod.Owner as DataTypePointer;
				if (dtp != null)
				{
					return false;
				}
				CustomMethodPointer cp = act.Action.ActionMethod as CustomMethodPointer;
				if (cp != null)
				{
					if (cp.ClassId != act.ClassId)
					{
						if (act.Action.ExecuterMemberId == ActionExecuterId)
						{
							return true;
						}
					}
				}
				else
				{
					SetterPointer sp = act.Action.ActionMethod as SetterPointer;
					if (sp != null)
					{
						CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
						if (cpp == null || cpp.ClassId != act.ClassId)
						{
							if (act.Action.ExecuterMemberId == ActionExecuterId)
							{
								return true;
							}
						}
					}
					else
					{
						if (act.Action.ExecuterMemberId == ActionExecuterId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			if (obj.IsForComponent)
			{
				if (Actions.Count > 0)
				{
					foreach (TaskID tid in Actions)
					{
						if (obj.IsActionExecuter(tid, tid.Action.Class))
						{
							return true;
						}
					}
				}
				return false;
			}
			else
			{
				ComponentIconFireEvent cife = obj as ComponentIconFireEvent;
				if (cife != null)
				{
					if (Actions.Count > 0)
					{
						foreach (TaskID tid in Actions)
						{
							if (!tid.IsEmbedded)
							{
								if (tid.Action == null)
								{
									tid.LoadActionInstance(this.RootPointer);
								}
								if (tid.Action != null)
								{
									FireEventMethod fe = tid.Action.ActionMethod as FireEventMethod;
									if (fe != null)
									{
										if (fe.EventId == cife.EventId && fe.MemberId == cife.FirerId)
										{
											return true;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					ComponentIconClass cic = obj as ComponentIconClass;
					if (cic != null)
					{
						if (Actions.Count > 0)
						{
							foreach (TaskID tid in Actions)
							{
								if (tid.Action == null)
								{
									tid.LoadActionInstance(this.RootPointer);
								}
								if (tid.Action != null)
								{
									ClassPointer cp = this.RootPointer.GetExternalExecuterClass(tid.Action);
									if (cp != null)
									{
										if (cp.ClassId == cic.ClassId)
										{
											return true;
										}
									}
								}
							}
						}
					}
					else
					{
						ComponentIconClassType cict = obj as ComponentIconClassType;
						if (cict != null)
						{
							if (Actions.Count > 0)
							{
								foreach (TaskID tid in Actions)
								{
									if (tid.Action == null)
									{
										tid.LoadActionInstance(this.RootPointer);
									}
									if (tid.Action != null)
									{
										ActionAttachEvent aae = tid.Action as ActionAttachEvent;
										if (aae != null)
										{
											if (aae.Class.BaseClassType.Equals(cict.ClassType))
											{
												return true;
											}
										}
										else if (tid.Action.ActionMethod != null)
										{
											DataTypePointer dtp = tid.Action.ActionMethod.Owner as DataTypePointer;
											if (dtp != null && dtp.BaseClassType != null)
											{
												if (dtp.BaseClassType.Equals(cict.ClassType))
												{
													return true;
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
			return false;
		}
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOutExecuter po = (EventPortOutExecuter)base.CreateDuplicateOutPort();
			po.ActionExecuterId = this.ActionExecuterId;
			return po;
		}

		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XMLATTR_ExecuterId, ActionExecuterId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			ActionExecuterId = XmlUtil.GetAttributeUInt(node, XMLATTR_ExecuterId);
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> action:{1}", Event.DisplayName, ActionExecuterId);
		}
		#endregion
	}
	/// <summary>
	/// execute a custom method or an event handler
	/// </summary>
	public class EventPortOutExecuteMethod : EventPortOut
	{
		#region fields and constructor
		private MethodClass _method;
		public EventPortOutExecuteMethod(EventIcon owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		public MethodClass Method
		{
			get
			{
				return _method;
			}
		}
		public UInt32 MethodId
		{
			get;
			set;
		}
		#endregion
		#region Methods
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
		}
		public void SetMethod(MethodClass method)
		{
			if (method != null)
			{
				_method = method;
				MethodId = method.MemberId;
			}
		}
		public override string ToString()
		{
			if (_method != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> method:{1}", Event.DisplayName, _method.MethodName);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> method:{1}", Event.DisplayName, MethodId);
		}
		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			if (obj.IsForComponent)
			{
				MemberComponentIdCustom idc = obj.ClassPointer as MemberComponentIdCustom;
				if (idc != null && idc.DefinitionClassId != obj.ClassId)
				{
					return (idc.DefinitionClassId == _method.ClassId);
				}
			}
			else
			{
				ComponentIconMethod cim = obj as ComponentIconMethod;
				if (cim != null)
				{
					return (cim.MethodId == this.MethodId);
				}
			}
			return false;
		}

		public override bool CanActionBeLinked(TaskID act)
		{
			HandlerMethodID hid = act as HandlerMethodID;
			if (hid != null)
			{
				return (hid.ActionId == this.MethodId);
			}
			if (act.Action != null)
			{
				DataTypePointer dtp = act.Action.ActionMethod.Owner as DataTypePointer;
				if (dtp != null)
				{
					return false;
				}
				ClassPointer cpr = this.RootPointer.GetExternalExecuterClass(act.Action);
				if (cpr != null)
				{
					return false;
				}
				CustomMethodPointer cmp = act.Action.ActionMethod as CustomMethodPointer;
				if (cmp != null)
				{
					if (cmp.MemberId == MethodId)
					{
						return true;
					}
				}
			}
			return false;
		}
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOutExecuteMethod po = (EventPortOutExecuteMethod)base.CreateDuplicateOutPort();
			po.MethodId = this.MethodId;
			return po;
		}


		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_handlerId, MethodId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			MethodId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_handlerId);
			XmlObjectReader reader = (XmlObjectReader)serializer;
			ClassPointer root = reader.ObjectList.RootPointer as ClassPointer;
			MethodClass mc = root.GetCustomMethodById(MethodId);
			if (mc == null)
			{
				MathNode.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Method {0} not found in class {1}", MethodId, root.ClassId));
			}
			else
			{
				_method = mc;
			}
		}
		#endregion
	}
	/// <summary>
	/// execute a set property action
	/// </summary>
	public class EventPortOutSetProperty : EventPortOut
	{
		#region fields and constructors
		private PropertyClass _property;
		const string XMLATTR_PropertyId = "propertyId";
		public EventPortOutSetProperty(EventIcon owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		public PropertyClass Property
		{
			get
			{
				return _property;
			}
		}
		public UInt32 PropertyId
		{
			get;
			set;
		}
		public UInt32 PropertyClassId
		{
			get;
			set;
		}
		#endregion
		#region Methods
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
		}
		public void SetProperty(PropertyClass property)
		{
			_property = property;
			PropertyId = property.MemberId;
			PropertyClassId = property.ClassId;
		}

		public override string ToString()
		{
			if (_property != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> property:{1}", Event.DisplayName, _property.Name);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> property:{1}", Event.DisplayName, PropertyId);
		}
		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			if (obj.IsForComponent)
			{
				MemberComponentIdCustom idc = obj.ClassPointer as MemberComponentIdCustom;
				if (idc != null && idc.DefinitionClassId != obj.ClassId)
				{
					return (idc.DefinitionClassId == _property.ClassId);
				}
			}
			else
			{
				ComponentIconProperty cim = obj as ComponentIconProperty;
				if (cim != null)
				{
					return (cim.PropertyId == this.PropertyId);
				}
			}
			return false;
		}

		public override bool CanActionBeLinked(TaskID act)
		{
			if (act.Action != null)
			{
				ClassPointer cpr = this.RootPointer.GetExternalExecuterClass(act.Action);
				if (cpr != null)
				{
					return false;
				}
				DataTypePointer dtp = act.Action.ActionMethod.Owner as DataTypePointer;
				if (dtp != null)
				{
					return false;
				}
				SetterPointer sp = act.Action.ActionMethod as SetterPointer;
				if (sp != null)
				{
					CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
					if (cpp != null)
					{
						return (cpp.MemberId == this.PropertyId);
					}
				}
			}
			return false;
		}
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOutSetProperty po = (EventPortOutSetProperty)base.CreateDuplicateOutPort();
			po.PropertyId = this.PropertyId;
			return po;
		}

		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ClassID, PropertyClassId);
			XmlUtil.SetAttribute(node, XMLATTR_PropertyId, PropertyId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			PropertyId = XmlUtil.GetAttributeUInt(node, XMLATTR_PropertyId);
			PropertyClassId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);
			XmlObjectReader reader = (XmlObjectReader)serializer;
			if (PropertyClassId == 0)
			{
				PropertyClassId = reader.ObjectList.ClassId;
			}
			ClassPointer root = ClassPointer.CreateClassPointer(PropertyClassId, reader.ObjectList.Project);

			PropertyClass mc = root.GetCustomPropertyById(PropertyId);
			if (mc == null)
			{
				MathNode.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Property {0} not found in class {1}", PropertyId, root.ClassId));
			}
			else
			{
				_property = mc;
			}
		}
		#endregion
	}

	/// <summary>
	/// a static action from a type 
	/// </summary>
	public class EventPortOutTypeAction : EventPortOut
	{
		#region fields and constructor
		private Type _type;
		const string XML_OwnerType = "OwnerType";
		public EventPortOutTypeAction(EventIcon owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		public Type OwnerType
		{
			get
			{
				return _type;
			}
		}
		#endregion
		#region Methods
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
		}
		public void SetOwnerType(Type t)
		{
			_type = t;
		}
		public override string ToString()
		{
			if (_type != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> Type:{1}", Event.DisplayName, _type.Name);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> Type:?", Event.DisplayName);
		}
		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			if (_type != null)
			{
				ComponentIconClassType cim = obj as ComponentIconClassType;
				if (cim != null)
				{
					return _type.Equals(cim.ClassType);
				}
			}
			return false;
		}

		public override bool CanActionBeLinked(TaskID act)
		{
			HandlerMethodID hid = act as HandlerMethodID;
			if (hid == null)
			{
				if (act.Action == null)
				{
					act.LoadActionInstance(this.RootPointer);
				}
				if (act.Action != null)
				{
					DataTypePointer cmp = act.Action.ActionMethod.Owner as DataTypePointer;
					if (cmp != null)
					{
						if (cmp.BaseClassType.Equals(_type))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOutTypeAction po = (EventPortOutTypeAction)base.CreateDuplicateOutPort();
			po.SetOwnerType(this.OwnerType);
			return po;
		}


		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_OwnerType);
			XmlUtil.SetLibTypeAttribute(nd, _type);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			XmlNode nd = node.SelectSingleNode(XML_OwnerType);
			if (nd != null)
			{
				_type = XmlUtil.GetLibTypeAttribute(nd);
			}
			else
			{
				_type = typeof(object);
			}
		}
		#endregion
	}

	/// <summary>
	/// a static action from a ClassPointer other than the current root 
	/// </summary>
	public class EventPortOutClassTypeAction : EventPortOut
	{
		#region fields and constructor
		private UInt32 _classId;
		private ClassPointer _classPointer;
		const string XMLATT_ownerClassId = "ownerClassId";
		public EventPortOutClassTypeAction(EventIcon owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		public UInt32 OwnerClassId
		{
			get
			{
				return _classId;
			}
		}
		public ClassPointer OwnerClassPointer
		{
			get
			{
				if (_classPointer == null)
				{
					if (_classId != 0)
					{
					}
				}
				return _classPointer;
			}
		}
		#endregion
		#region Methods
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
		}
		public void SetOwnerClassPointer(ClassPointer p)
		{
			_classPointer = p;
			_classId = p.ClassId;
		}
		public void SetOwnerClassId(UInt32 id)
		{
			_classId = id;
		}
		public override string ToString()
		{
			if (_classPointer != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> Class:{1}", Event.DisplayName, _classPointer.Name);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-> Class:{1}", Event.DisplayName, _classId);
		}
		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			ComponentIconClass cim = obj as ComponentIconClass;
			if (cim != null)
			{
				return _classId == cim.ClassId;
			}
			return false;
		}

		public override bool CanActionBeLinked(TaskID act)
		{
			HandlerMethodID hid = act as HandlerMethodID;
			if (hid == null)
			{
				if (act.Action == null)
				{
					act.LoadActionInstance(this.RootPointer);
				}
				if (act.Action != null)
				{
					ClassPointer cmp = this.RootPointer.GetExternalExecuterClass(act.Action);
					if (cmp != null)
					{
						if (_classId == act.Action.ExecuterClassId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public override LinkLineNodeOutPort CreateDuplicateOutPort()
		{
			EventPortOutClassTypeAction po = (EventPortOutClassTypeAction)base.CreateDuplicateOutPort();
			if (_classPointer != null)
			{
				po.SetOwnerClassPointer(_classPointer);
			}
			else
			{
				po.SetOwnerClassId(_classId);
			}
			return po;
		}


		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XMLATT_ownerClassId, _classId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_classId = XmlUtil.GetAttributeUInt(node, XMLATT_ownerClassId);
			XmlObjectReader r = serializer as XmlObjectReader;
			if (r != null)
			{
				_classPointer = ClassPointer.CreateClassPointer(_classId, r.ObjectList.Project);
			}
		}
		#endregion
	}
	/// <summary>
	/// used by ComponentIconFireEvent, to be linked to EventPortInFireEvent
	/// </summary>
	[UseLabel(false)]
	public class EventPortOutFirer : EventPortOut
	{
		#region fields and constructors
		public EventPortOutFirer(IEventMapSource owner)
			: base(owner)
		{

		}
		#endregion
		#region properties
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region Methods
		public override bool CanActionBeLinked(TaskID act)
		{
			return false;
		}

		public override bool IsActionExecuter(ComponentIconEvent obj)
		{
			ComponentIconFireEvent cife = obj as ComponentIconFireEvent;
			if (cife != null)
			{
				ComponentIconFireEvent cife0 = this.PortOwner as ComponentIconFireEvent;
				if (cife0 != null)
				{
					if (cife.FirerId == cife0.FirerId && cife.EventId == cife0.EventId)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
	/// <summary>
	/// actions owner node
	/// the actual actions to be executed are saved in EventPortOut, the actions serve as a link to EventPortIn
	/// through the action executer.
	/// </summary>
	[UseParentObject]
	public class EventPortIn : LinkLineNodeInPort
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.Blue;
		private IEvent _event;
		private Point _relativeLocation;
		private bool _moving;
		private bool _loaded;
		public EventPortIn(IPortOwner owner)
			: base(owner)
		{
			this.Size = new System.Drawing.Size(ComponentIconEvent.PortSize, ComponentIconEvent.PortSize);
			LabelVisible = false;
			//
			Control c = owner as Control;
			Owner = c;
			c.Move += new EventHandler(c_Move);
		}

		void c_Move(object sender, EventArgs e)
		{
			if (this.Parent != null && _loaded)
			{
				if (_relativeLocation != Point.Empty)
				{
					Control c = sender as Control;
					if (c != null)
					{
						_moving = true;
						this.Location = new Point(c.Location.X - _relativeLocation.X, c.Location.Y - _relativeLocation.Y);
						_moving = false;
					}
				}
			}
		}
		#endregion
		#region properties
		public Point RelativeLocation
		{
			get
			{
				return _relativeLocation;
			}
			set
			{
				_relativeLocation = value;
			}
		}
		public override bool CanCreateDuplicatedLink
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public override enumPositionType PositionType
		{
			get
			{
				return enumPositionType.Circle;
			}
			set
			{
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
			}
		}
		#endregion
		#region methods
		const string XMLATTR_RELX = "relX";
		const string XMLATTR_RELY = "relY";
		const string XML_Event = "Event";
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			base.OnWriteToXmlNode(serializer, node);
			XmlNode eNode = XmlUtil.CreateSingleNewElement(node, XML_Event);
			XmlObjectWriter writer = (XmlObjectWriter)serializer;
			writer.WriteObjectToNode(eNode, _event);
			if (_relativeLocation != Point.Empty)
			{
				XmlUtil.SetAttribute(node, XMLATTR_RELX, _relativeLocation.X);
				XmlUtil.SetAttribute(node, XMLATTR_RELY, _relativeLocation.Y);
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			XmlNode eNode = node.SelectSingleNode(XML_Event);
			XmlObjectReader reader = (XmlObjectReader)serializer;
			_event = (IEvent)reader.ReadObject(eNode, null);
			int x = XmlUtil.GetAttributeInt(node, XMLATTR_RELX);
			int y = XmlUtil.GetAttributeInt(node, XMLATTR_RELY);
			_relativeLocation = new Point(x, y);
		}

		public override void SetLoaded()
		{
			_loaded = true;
		}
		public override void SaveLocation()
		{
			if (_loaded)
			{
				if (!_moving && !AdjustingPosition)
				{
					base.SaveLocation();
					Control ei = PortOwner as Control;
					_relativeLocation = new Point(ei.Location.X - this.Location.X, ei.Location.Y - this.Location.Y);
				}
			}
		}
		public override void RestoreLocation()
		{
			Control ei = PortOwner as Control;
			this.Location = new Point(ei.Location.X - _relativeLocation.X, ei.Location.Y - _relativeLocation.Y);
		}
		protected override enumPositionType GetPosTypeByCornerPos(int cornerIndex)
		{
			return enumPositionType.Circle;
		}
		protected override void OnMove(EventArgs e)
		{
			if (_loaded)
			{
				base.OnMove(e);
				if (!_moving && !AdjustingPosition)
				{
					SaveLocation();
					EventPath ep = this.Parent as EventPath;
					if (ep != null)
					{
						ep.NotifyCurrentChanges();
					}
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			GraphicsState st = e.Graphics.Save();
			ActiveDrawing ei = PortOwner as ActiveDrawing;
			Point p0 = ei.Center;
			Point p1 = this.Center;
			double a;
			if (p0.Y == p1.Y)
			{
				if (p1.X > p0.X)
				{
					a = -Math.PI / 2.0;
				}
				else
				{
					a = Math.PI / 2.0;
				}
			}
			else
			{
				a = -Math.Atan2((double)(p1.X - p0.X), ((double)(p1.Y - p0.Y)));
			}
			e.Graphics.TranslateTransform(this.Size.Width / 2, this.Size.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Prepend);
			e.Graphics.RotateTransform((float)(a * 180.0 / Math.PI));
			e.Graphics.TranslateTransform(-this.Size.Width / 2, -this.Size.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Prepend);
			VplDrawing.DrawInArrow(e.Graphics, _drawBrush, this.Size, enumPositionType.Bottom);
			e.Graphics.Restore(st);
		}
		#endregion
	}
	/// <summary>
	/// used by EventIcon, to be linked to EventPortOutFirer
	/// </summary>
	public class EventPortInFireEvent : EventPortIn
	{
		#region fields and constructors
		private UInt32 _actionId;
		private FireEventMethod _fireEventMethod;
		public EventPortInFireEvent(IPortOwner owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		public UInt32 FireEventMethodId
		{
			get
			{
				return _actionId;
			}
			set
			{
				_actionId = value;
			}
		}
		public FireEventMethod FireEventMethod
		{
			get
			{
				return _fireEventMethod;
			}
			set
			{
				_fireEventMethod = value;
				if (_fireEventMethod != null)
				{
					_actionId = _fireEventMethod.MemberId;
					Event = _fireEventMethod.Event;
				}
			}
		}
		#endregion
		#region methods
		const string XMLATTR_FirerId = "firerId";
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XMLATTR_FirerId, _actionId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_actionId = XmlUtil.GetAttributeUInt(node, XMLATTR_FirerId);
		}
		#endregion
	}

}
