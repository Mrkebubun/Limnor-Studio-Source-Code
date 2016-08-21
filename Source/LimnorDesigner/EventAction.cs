/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
/*
	Limnor Studio Event Handler Designer
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using XmlSerializer;
using System.Xml;
using LimnorDesigner.Event;
using VPL;
using XmlUtility;
using LimnorDesigner.Action;
using System.Reflection;
using LimnorDesigner.MethodBuilder;
using System.Xml.Serialization;
using Limnor.WebBuilder;
using MathExp;
using System.Globalization;
using LimnorDesigner.Property;
using System.Windows.Forms;
using System.Collections.Specialized;
using LimnorDatabase;
using System.CodeDom;

namespace LimnorDesigner
{
	/// <summary>
	/// event handler can be a single action or a group of action
	/// </summary>
	public interface IEventHandler
	{
		UInt64 WholeActionId { get; }
	}
	/// <summary>
	/// one action in one event handler
	/// </summary>
	[SaveAsProperties]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class TaskID : ICloneable, IXmlNodeHolder, IBeforeSerializeNotify
	{
		private UInt64 _id;
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		private XmlNode _xmlNode;
		private UInt32 _taskId;//unique id for this task. different tasks may have the same _id representing the same action
		private IEvent _eventToHandle;
		private IAction _action;
		public TaskID()
		{
		}
		public TaskID(UInt64 id)
		{
			_id = id;
		}
		public TaskID(UInt32 actionId, UInt32 classId)
		{
			_id = DesignUtil.MakeDDWord(actionId, classId);
		}
		public virtual void EstablishObjectOwnership(IActionsHolder scope)
		{
			if (Action != null)
			{
				Action.EstablishObjectOwnership(scope);
			}
		}
		public void SetEvent(IEvent e)
		{
			_eventToHandle = e;
		}
		public virtual bool IsSameTask(TaskID task)
		{
			if (task != null)
			{
				return (TaskId == task.TaskId);
			}
			return false;
		}
		public override string ToString()
		{
			if (_action != null)
			{
				return _action.Display;
			}
			return string.Format("[action {0},{1}]", this.ActionId, this._eventToHandle);
		}
		private List<UInt32> _guidLoadAction;
		public IAction LoadActionInstance(IActionsHolder holder)
		{
			if (!IsNoAction)
			{
				if (_action == null)
				{
					if (_guidLoadAction == null)
					{
						_guidLoadAction = new List<UInt32>();
					}
					if (_guidLoadAction.Contains(this.ActionId))
					{
						_guidLoadAction.Remove(this.ActionId);
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Loading Action [{0}] failed in circular call", this.ActionId));
						_action = new NoAction();
					}
					else
					{
						_guidLoadAction.Add(this.ActionId);
						try
						{
							_action = holder.GetActionInstance(this.ActionId);
						}
						finally
						{
							_guidLoadAction.Remove(this.ActionId);
						}
					}
				}
			}
			return _action;
		}
		public IAction GetPublicAction(ClassPointer root)
		{
			if (IsNoAction)
			{
				return NoAction.Value;
			}
			if (Action == null)
			{
				return LoadActionInstance(root);
			}
			return Action;
		}
		public void SetAction(IAction a)
		{
			_action = a;
		}
		[XmlIgnore]
		[Browsable(false)]
		[ReadOnly(true)]
		public int TaskOrder { get; set; }
		public IAction Action
		{
			get
			{
				return _action;
			}
		}
		protected XmlObjectWriter Writer
		{
			get
			{
				if (_writer == null)
				{
					_writer = XmlSerializerUtility.GetWriter(_reader) as XmlObjectWriter;
				}
				return _writer;
			}
		}
		public virtual bool IsNoAction
		{
			get
			{
				return (_id == 0);
			}
		}
		public virtual bool IsEmbedded
		{
			get
			{
				return false;
			}
		}
		public bool IsBreak
		{
			get
			{
				return (_id == 1);
			}
		}
		[Browsable(false)]
		public IEvent EventToHandle
		{
			get
			{
				return _eventToHandle;
			}
		}
		[Browsable(false)]
		public virtual UInt32 TaskId
		{
			get
			{
				if (_taskId == 0)
				{
					_taskId = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _taskId;
			}
			set
			{
				_taskId = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual UInt64 WholeTaskId
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Browsable(false)]
		public virtual UInt32 ClassId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_id, out a, out c);
				return c;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_id, out a, out c);
				_id = DesignUtil.MakeDDWord(a, value);
			}
		}
		[Browsable(false)]
		public virtual UInt32 ActionId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_id, out a, out c);
				return a;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_id, out a, out c);
				_id = DesignUtil.MakeDDWord(value, c);
			}
		}
		[DefaultValue(false)]
		public bool BreakAsEventAction { get; set; }

		#region ICloneable Members

		public object Clone()
		{
			TaskID obj = new TaskID(_id);
			obj.BreakAsEventAction = BreakAsEventAction;
			obj.DataXmlNode = DataXmlNode;
			obj.TaskId = TaskId;
			return obj;
		}

		#endregion

		#region IXmlNodeHolder Members
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

		#region IBeforeSerializeNotify Members

		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reader = reader;
			_xmlNode = node;
		}

		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_writer = writer;
			_xmlNode = node;
		}
		public void ReloadFromXmlNode()
		{
			if (_reader != null && _xmlNode != null)
			{
				_reader.ReadObjectFromXmlNode(_xmlNode, this, this.GetType(), null);
			}
		}
		public void UpdateXmlNode(XmlObjectWriter writer)
		{
			if (_xmlNode != null)
			{
				if (writer != null)
				{
					writer.WriteObjectToNode(_xmlNode, this);
				}
				else if (Writer != null)
				{
					_writer.WriteObjectToNode(_xmlNode, this);
				}
			}
		}
		[Browsable(false)]
		public XmlNode XmlData { get { return _xmlNode; } }

		#endregion
	}
	/// <summary>
	/// embed an event handler method within an event handler 
	/// </summary>
	public class HandlerMethodID : TaskID, ISerializeParent, ITransferBeforeWrite
	{
		private EventHandlerMethod _handlerMethod;
		public HandlerMethodID()
		{
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override UInt64 WholeTaskId
		{
			get
			{
				return _handlerMethod.WholeId;
			}
			set
			{
			}
		}
		public override bool IsNoAction
		{
			get
			{
				return false;
			}
		}
		public override bool IsEmbedded
		{
			get
			{
				return true;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get
			{
				return _handlerMethod.ClassId;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 ActionId
		{
			get
			{
				return _handlerMethod.MemberId;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 TaskId
		{
			get
			{
				return ActionId;
			}
			set
			{
			}
		}
		[NoRecreateProperty]
		[Browsable(false)]
		public EventHandlerMethod HandlerMethod
		{
			get
			{
				return _handlerMethod;
			}
			set
			{
				_handlerMethod = value;
			}
		}

		#region ISerializeParent Members

		public void OnMemberCreated(object member)
		{
			EventHandlerMethod h = member as EventHandlerMethod;
			if (h != null)
			{
				h.SetEvent(EventToHandle);
			}

		}

		#endregion
		public override bool IsSameTask(TaskID task)
		{
			HandlerMethodID hid = task as HandlerMethodID;
			if (hid != null)
			{
				return (hid.ActionId == this.ActionId);
			}
			return false;
		}
		public override string ToString()
		{
			if (_handlerMethod != null)
			{
				return _handlerMethod.MethodName;
			}
			return base.ToString();
		}
	}
	/// <summary>
	/// action list for an event handler
	/// </summary>
	[Serializable]
	public class TaskIdList : List<TaskID>, ICloneable, ISerializeParent
	{
		private IEvent _eventToHandle;
		public TaskIdList()
		{
		}
		public void SetEvent(IEvent e)
		{
			_eventToHandle = e;
		}
		public IEvent EventToHandle
		{
			get
			{
				return _eventToHandle;
			}
		}
		public void SetTaskOrder()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].TaskOrder = i;
			}
		}
		public void RemoveInvalidTasks()
		{
			List<TaskID> lst = new List<TaskID>();
			foreach (TaskID tid in this)
			{
				if (tid.ActionId == 0)
				{
					lst.Add(tid);
				}
			}
			if (lst.Count > 0)
			{
				foreach (TaskID tid in lst)
				{
					Remove(tid);
				}
			}
		}
		public TaskID RemoveAction(UInt32 taskId)
		{
			TaskID ret = null;
			foreach (TaskID t in this)
			{
				if (t.TaskId == taskId)
				{
					ret = t;
					this.Remove(t);
					break;
				}
			}
			return ret;
		}
		#region ICloneable Members

		public object Clone()
		{
			TaskIdList obj = new TaskIdList();
			obj.SetEvent(_eventToHandle);
			foreach (TaskID tid in this)
			{
				obj.Add((TaskID)tid.Clone());
			}
			return obj;
		}

		#endregion

		#region ISerializeParent Members

		public void OnMemberCreated(object member)
		{
			TaskID t = member as TaskID;
			if (t != null)
			{
				t.SetEvent(_eventToHandle);
			}
		}

		#endregion
	}
	/// <summary>
	/// event handler defined by event and handler operations
	/// </summary>
	[Serializable]
	public class EventAction : IBreakPointOwner, ISerializeParent, IBeforeSerializeNotify
	{
		#region fields and constructors
		public const string JS_EVENT_KEY = "key8923abc";
		const string XMLATTVAL_TaskIDList = "TaskIDList";
		private IEvent _eventPointer; //EventPointer or EventClass
		private TaskIdList _taskIdList;
		private XmlNode _handlerXmlNode;
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		public EventAction()
		{
		}
		public EventAction(XmlNode handlerNode)
		{
			_handlerXmlNode = handlerNode;
		}
		#endregion

		#region Properties
		protected XmlObjectWriter Writer
		{
			get
			{
				if (_writer == null)
				{
					_writer = XmlSerializerUtility.GetWriter(_reader) as XmlObjectWriter;
				}
				return _writer;
			}
		}
		private UInt32 _ehId;
		/// <summary>
		/// for making client/server value names based on event handling
		/// </summary>
		[Browsable(false)]
		public UInt32 EventHandlerId
		{
			get
			{
				if (_ehId == 0)
				{
					_ehId = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _ehId;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool Disbale { get; set; }
		[ReadOnly(true)]
		[Browsable(false)]
		public bool CompileToFunction { get; set; }

		[Browsable(false)]
		public string EventName
		{
			get
			{
				if (string.CompareOrdinal(Event.Name, "onenterkey") == 0)
				{
					return "onkeyup";
				}
				return Event.Name;
			}
		}
		private string _eventHandlername;
		[Browsable(false)]
		public string EventHandlerName
		{
			get
			{
				if (string.IsNullOrEmpty(_eventHandlername))
				{
					_eventHandlername = string.Format(CultureInfo.InvariantCulture, "{0}{1}", EventName, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				}
				return _eventHandlername;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode XmlNode
		{
			get
			{
				return _handlerXmlNode;
			}
			set
			{
				_handlerXmlNode = value;
			}
		}
		public XmlNode XmlNodeActionList
		{
			get
			{
				return this.XmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XMLATTVAL_TaskIDList));
			}
		}
		/// <summary>
		/// event to be handled
		/// </summary>
		public IEvent Event
		{
			get
			{
				return _eventPointer;
			}
			set
			{
				_eventPointer = value;
			}
		}
		/// <summary>
		/// operations for handling the event
		/// </summary>
		[NoRecreateProperty]
		[PropertyReadOrder(100)]
		public TaskIdList TaskIDList
		{
			get
			{
				if (_taskIdList == null)
				{
					_taskIdList = new TaskIdList();
				}
				else
				{
					_taskIdList.RemoveInvalidTasks();
				}
				return _taskIdList;
			}
			set
			{
				_taskIdList = value;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				if (_eventPointer != null && _eventPointer.RootPointer != null)
				{
					return _eventPointer.RootPointer;
				}
				if (Writer != null && _writer.ObjectList != null && _writer.ObjectList.RootPointer != null)
				{
					return _writer.ObjectList.RootPointer as ClassPointer;
				}
				if (_reader != null && _reader.ObjectList != null && _reader.ObjectList.RootPointer != null)
				{
					return _reader.ObjectList.RootPointer as ClassPointer;
				}
				return null;
			}
		}
		#endregion

		#region Methods
		public static bool IsFileUploadAction(IAction act)
		{
			if (act.ActionMethod != null && act.ActionMethod.Owner != null)
			{
				if (string.CompareOrdinal(act.ActionMethod.MethodName, "Upload") == 0)
				{
					if (act.ActionMethod.Owner.ObjectInstance != null)
					{
						IFormSubmitter ifs = act.ActionMethod.Owner.ObjectInstance as IFormSubmitter;
						if (ifs != null)
						{
							return true;
						}
					}
					else
					{
						Type t = act.ActionMethod.Owner.ObjectType;
						if (t != null)
						{
							if (t.GetInterface("IFormSubmitter") != null)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public bool HasUploadAction()
		{
			if (_taskIdList != null)
			{
				foreach (TaskID tid in _taskIdList)
				{
					if (tid.Action != null)
					{
						if (EventAction.IsFileUploadAction(tid.Action))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public bool IsWebMouseKeyboardEvent()
		{
			return WebPageCompilerUtility.IsWebClientMouseKeyboardEvent(this.Event.Name);
		}
		public bool IsWebMouseEvent()
		{
			if (string.CompareOrdinal(this.Event.Name, "onclick") == 0
				|| string.CompareOrdinal(this.Event.Name, "onmousedown") == 0
				|| string.CompareOrdinal(this.Event.Name, "onmouseup") == 0
				|| string.CompareOrdinal(this.Event.Name, "onmouseover") == 0
				|| string.CompareOrdinal(this.Event.Name, "onmousemove") == 0
				|| string.CompareOrdinal(this.Event.Name, "onmouseout") == 0
				)
			{
				return true;
			}
			return false;
		}
		public bool IsWebKeyboardEvent()
		{
			if (string.CompareOrdinal(this.Event.Name, "onkeypress") == 0
				|| string.CompareOrdinal(this.Event.Name, "onkeydown") == 0
				|| string.CompareOrdinal(this.Event.Name, "onkeyup") == 0
				)
			{
				return true;
			}
			return false;
		}
		public void AttachCodeDomAction(IMethodCompile methodToCompile, CodeStatementCollection statements, bool debug)
		{
			CodeExpression methodTarget;
			CodeEventReferenceExpression ceRef = Event.GetReferenceCode(methodToCompile, statements, false) as CodeEventReferenceExpression;
			if (Event.IsStatic)
				methodTarget = new CodeTypeReferenceExpression(this.RootPointer.CodeName);
			else
				methodTarget = new CodeThisReferenceExpression();
			CodeAttachEventStatement caes = new CodeAttachEventStatement(ceRef,
									new CodeDelegateCreateExpression(new CodeTypeReference(Event.EventHandlerType.TypeString),
										methodTarget, GetLocalHandlerName()));
			statements.Add(caes);
		}
		public void AttachJavascriptAction(UInt32 actId, StringCollection methodCode, string indent)
		{
			if (IsExtendWebClientEvent())
			{
				methodCode.Add("JsonDataBinding.attachExtendedEvent('");
				methodCode.Add(Event.Name);
				methodCode.Add("','");
				EasyDataSet eds = Event.Owner.ObjectInstance as EasyDataSet;
				if (eds != null)
				{
					methodCode.Add(eds.TableName);
				}
				else
				{
					methodCode.Add(Event.Owner.CodeName);
				}
				methodCode.Add("',");
				methodCode.Add(GetLocalHandlerName());
				methodCode.Add(");\r\n");
			}
			else
			{
				IJavaScriptEventOwner eo = Event.Owner.ObjectInstance as IJavaScriptEventOwner;
				if (eo != null)
				{
					string fn = EventAction.GetAttachFunctionName(actId);
					eo.AttachJsEvent(Event.Owner.CodeName, Event.Name, fn, methodCode);
				}
				else
				{
					IJavascriptEventHolder jeh = ObjectCompilerAttribute.GetJavascriptEventHolder(Event.Owner.ObjectType);
					if (jeh != null)
					{
						jeh.AttachJsEvent(Event.Owner.CodeName, Event.Name, GetLocalHandlerName(), methodCode);
					}
					else
					{
						if (Event.Owner.ObjectType != null && Event.Owner.ObjectType.GetInterface("IWebClientControl") != null)
						{
							if (!(Event.Owner is LocalVariable))
							{
								methodCode.Add("var ");
								methodCode.Add(Event.Owner.CodeName);
								methodCode.Add(" = document.getElementById('");
								methodCode.Add(Event.Owner.CodeName);
								methodCode.Add("');\r\n");
							}
						}
						methodCode.Add("JsonDataBinding.AttachEvent(");
						methodCode.Add(Event.Owner.CodeName);
						methodCode.Add(",'");
						methodCode.Add(Event.Name);
						methodCode.Add("',");
						methodCode.Add(GetLocalHandlerName());
						methodCode.Add(");\r\n");
					}
				}
			}
		}
		public static string GetAttachFunctionName(UInt32 attachActionId)
		{
			return string.Format(CultureInfo.InvariantCulture, "attach{0}", attachActionId.ToString("x", CultureInfo.InvariantCulture));
		}
		private UInt32 _actionBranchId;
		public UInt32 GetAssignActionId()
		{
			if (_actionBranchId == 0)
			{
				if (this.XmlNode != null)
				{
					_actionBranchId = XmlUtil.GetAttributeUInt(this.XmlNode, XmlTags.XMLATT_ActionID);
				}
			}
			return _actionBranchId;
		}
		public void SetAssignActionId(UInt32 abId)
		{
			_actionBranchId = abId;
			if (this.XmlNode != null)
			{
				XmlUtil.SetAttribute(this.XmlData, XmlTags.XMLATT_ActionID, abId);
			}
		}
		public const string FuncNameBeforeInitPage = "_beforeInitPage83021";
		public string GetLocalHandlerName()
		{
			if (typeof(fnInitPage).Equals(this.Event.EventHandlerType.BaseClassType))
			{
				return FuncNameBeforeInitPage;
			}
			UInt32 asId = GetAssignActionId();
			if (asId != 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", EventName, asId.ToString("x", CultureInfo.InvariantCulture));
			}
			return string.Empty;
		}
		public bool IsExtendWebClientEvent()
		{
			bool isWebClientEvent = false;
			EventPointer ep = Event as EventPointer;
			if (ep != null)
			{
				EventInfo ei = ep.Info;
				if (ei != null)
				{
					object[] vs = ei.GetCustomAttributes(typeof(WebClientEventByServerObjectAttribute), true);
					if (vs != null && vs.Length > 0)
					{
						isWebClientEvent = true;
					}
				}
			}
			return isWebClientEvent;
		}
		public void SaveEventBreakPointsToXml()
		{
			XmlNode node = this.XmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}[@{1}='BreakBeforeExecute']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
			if (this.BreakBeforeExecute)
			{
				if (node == null)
				{
					node = this.XmlNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
					XmlUtility.XmlUtil.SetAttribute(node, XmlTags.XMLATT_NAME, "BreakBeforeExecute");
					this.XmlNode.AppendChild(node);
				}
				node.InnerText = "True";
			}
			else
			{
				if (node != null)
				{
					XmlNode np = node.ParentNode;
					np.RemoveChild(node);
				}
			}
			node = this.XmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='BreakAfterExecute']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
			if (this.BreakAfterExecute)
			{
				if (node == null)
				{
					node = this.XmlNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
					XmlUtility.XmlUtil.SetAttribute(node, XmlTags.XMLATT_NAME, "BreakAfterExecute");
					this.XmlNode.AppendChild(node);
				}
				node.InnerText = "True";
			}
			else
			{
				if (node != null)
				{
					XmlNode np = node.ParentNode;
					np.RemoveChild(node);
				}
			}
		}
		public void AddAction(IEventHandler action)
		{
			if (_taskIdList == null)
			{
				_taskIdList = new TaskIdList();
			}
			TaskID t = null;
			EventHandlerMethod ehm = action as EventHandlerMethod;
			if (ehm != null)
			{
				HandlerMethodID hmd;
				foreach (TaskID tid in _taskIdList)
				{
					hmd = tid as HandlerMethodID;
					if (hmd != null)
					{
						if (hmd.HandlerMethod.MethodID == ehm.MethodID)
						{
							hmd.HandlerMethod = ehm;
							return;
						}
					}
				}
				hmd = new HandlerMethodID();
				hmd.HandlerMethod = ehm;
				t = hmd;
			}
			else
			{
				t = new TaskID(action.WholeActionId);
				ActionClass ac = action as ActionClass;
				if (ac != null)
				{
					t.SetAction(ac);
					MethodReturnMethod mr = ac.ActionMethod as MethodReturnMethod;
					if (mr != null)
					{
						if (mr.RootPointer == null)
						{
							if (_reader != null && _reader.ObjectList != null)
							{
								mr.SetRoot(_reader.ObjectList.RootPointer as ClassPointer);
							}
						}
						if (mr.RootPointer != null)
						{
							XmlObjectWriter xw = new XmlObjectWriter(mr.RootPointer.ObjectList);
							mr.RootPointer.SaveAction(ac, xw);
						}
					}
				}
			}
			if (t != null)
			{
				if (this.XmlNode != null)
				{
					XmlNode actNode = null;
					XmlNode actionsNode = this.XmlNodeActionList;
					if (actionsNode == null)
					{
						actionsNode = this.XmlNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
						this.XmlNode.AppendChild(actionsNode);
						XmlUtility.XmlUtil.SetAttribute(actionsNode, XmlTags.XMLATT_NAME, XMLATTVAL_TaskIDList);
						actNode = actionsNode.OwnerDocument.CreateElement("Item");
						actionsNode.AppendChild(actNode);
						XmlObjectWriter xw = _writer;
						if (xw == null)
						{
							ClassPointer root = this.RootPointer;
							if (root == null)
							{
								//root = action.
							}
							if (root != null)
							{
								xw = new XmlObjectWriter(root.ObjectList);
							}
						}
						if (xw != null)
						{
							xw.WriteObjectToNode(actNode, t);
						}
						t.DataXmlNode = actNode;
					}
				}
				_taskIdList.Add(t);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="taskIdDel">taskId of the task to be deleted</param>
		public void RemoveAction(UInt32 taskIdDel)
		{
			TaskID taskId = null;
			if (_taskIdList != null)
			{
				taskId = _taskIdList.RemoveAction(taskIdDel);
			}
			if (taskId != null)
			{
				if (taskId.DataXmlNode != null)
				{
					XmlNode nodeActList = taskId.DataXmlNode.ParentNode;
					if (nodeActList != null)
					{
						nodeActList.RemoveChild(taskId.DataXmlNode);
					}
				}
			}
			if (_taskIdList == null || _taskIdList.Count == 0)
			{
				XmlNode np = this.XmlNode.ParentNode;
				if (np != null)
				{
					np.RemoveChild(this.XmlNode);
				}
			}
		}
		public void RemoveAction(IAction act)
		{
			if (_taskIdList != null && _taskIdList.Count > 0)
			{
				List<TaskID> tids = new List<TaskID>();
				foreach (TaskID tid in _taskIdList)
				{
					if (!tid.IsEmbedded)
					{
						if (tid.ActionId == act.ActionId)
						{
							tids.Add(tid);
						}
					}
				}
				if (tids.Count > 0)
				{
					foreach (TaskID tid in tids)
					{
						RemoveAction(tid.TaskId);
					}
				}
			}
		}
		public void MoveActionUp(TaskID t)
		{
			if (_taskIdList != null && t != null)
			{
				int idx = _taskIdList.IndexOf(t);
				if (idx > 0 && idx < _taskIdList.Count)
				{
					TaskID t1 = _taskIdList[idx - 1];
					XmlNode node = t1.DataXmlNode;
					_taskIdList.RemoveAt(idx);
					_taskIdList.Insert(idx - 1, t);
					XmlNode nodeActList = node.ParentNode;
					nodeActList.RemoveChild(t.DataXmlNode);
					nodeActList.InsertBefore(t.DataXmlNode, node);
				}
			}
		}
		public void MoveActionDown(TaskID t)
		{
			if (_taskIdList != null && t != null)
			{
				int idx = _taskIdList.IndexOf(t);
				if (idx >= 0 && idx < _taskIdList.Count - 1)
				{
					TaskID t1 = _taskIdList[idx + 1];
					_taskIdList.RemoveAt(idx);
					_taskIdList.Insert(idx + 1, t);
					XmlNode node = t1.DataXmlNode;
					XmlNode nodeActList = node.ParentNode;
					nodeActList.RemoveChild(t.DataXmlNode);
					nodeActList.InsertAfter(t.DataXmlNode, node);
				}
			}
		}
		enum EnumActClass { Dialog, Submit, Other }
		public WebHandlerBlockList CreateWebClientHandlerActionBlocks(ClassPointer root)
		{
			EnumWebRunAt lastRunAt = EnumWebRunAt.Client;
			WebHandlerBlockList blocks = new WebHandlerBlockList();
			if (_taskIdList != null && _taskIdList.Count > 0)
			{
				WebHandlerBlock block = new WebHandlerBlock();
				blocks.Add(block);
				for (int i = 0; i < _taskIdList.Count; i++)
				{
					IAction act = null;
					EnumActClass actclass = EnumActClass.Other;
					EnumWebActionType actType = EnumWebActionType.Unknown;
					HandlerMethodID hmd = _taskIdList[i] as HandlerMethodID;
					if (hmd != null)
					{
						hmd.HandlerMethod.LoadActionInstances();
						WebClientEventHandlerMethod wceh = hmd.HandlerMethod as WebClientEventHandlerMethod;
						if (wceh == null)
						{
							if (this.Event != null && this.Event.RunAt == EnumWebRunAt.Client)
							{
								actType = EnumWebActionType.Client;
							}
							else
							{
								actType = EnumWebActionType.Server;
							}
						}
						else
						{
							if (wceh.WebUsage == EnumMethodWebUsage.Server)
							{
								actType = EnumWebActionType.Server;
							}
							else
							{
								if (wceh.UseClientPropertyOnly || !wceh.UseServerValues()) //client handler
								{
									actType = EnumWebActionType.Client;
								}
								else //download handler
								{
									actType = EnumWebActionType.Download;
								}
							}
						}
						if (actType == EnumWebActionType.Server || actType == EnumWebActionType.Upload)
						{
							lastRunAt = EnumWebRunAt.Server;
						}
						else
						{
							lastRunAt = EnumWebRunAt.Client;
						}
					}
					else
					{
						act = _taskIdList[i].LoadActionInstance(root);
						if (act != null)
						{
							ActionAttachEvent aae = act as ActionAttachEvent;
							if (aae != null)
							{
								act.CheckWebActionType();
								actType = act.WebActionType;
								if (actType == EnumWebActionType.Server || actType == EnumWebActionType.Upload)
								{
									lastRunAt = EnumWebRunAt.Server;
								}
								else
								{
									lastRunAt = EnumWebRunAt.Client;
								}
							}
							else
							{
								if (act.ActionMethod != null && act.ActionMethod.Owner != null)
								{
									IFormSubmitter fs = act.ActionMethod.Owner.ObjectInstance as IFormSubmitter;
									if (fs != null && fs.IsSubmissionMethod(act.ActionMethod.MethodName))
									{
										actclass = EnumActClass.Submit;
										lastRunAt = EnumWebRunAt.Server;
									}
								}
								if (actclass != EnumActClass.Submit)
								{
									if (act.ActionMethod != null && act.ActionMethod.Owner != null)
									{
										if (typeof(WebPage).IsAssignableFrom(act.ActionMethod.Owner.ObjectType))
										{
											if (string.CompareOrdinal(act.ActionMethod.MethodName, "ShowChildDialog") == 0)
											{
												actclass = EnumActClass.Dialog;
												lastRunAt = EnumWebRunAt.Client;
											}
										}
									}
									if (actclass == EnumActClass.Other)
									{
										if (act.ActionMethod is MethodReturnMethod)
										{
											act.ScopeRunAt = lastRunAt;
										}
										act.CheckWebActionType();
										actType = act.WebActionType;
									}
								}
							}
						}
					}
					//add actionn to apprepriate section of the current block
					//0:client  1:server  2:download
					switch (actclass)
					{
						case EnumActClass.Other:
							switch (actType)
							{
								case EnumWebActionType.Client: //a client action may only be in client/download scetion
									switch (block.SectIndex)
									{
										case 0:
											block.AddAction(_taskIdList[i]); //add it to client action
											break;
										case 1:
											block.SectIndex = 2; //move to download, add it to download section
											block.AddAction(_taskIdList[i]);
											break;
										case 2:
											block.AddAction(_taskIdList[i]); //add it to download section 
											break;
									}
									break;
								case EnumWebActionType.Server:
								case EnumWebActionType.Upload: //can only be in server section
									switch (block.SectIndex)
									{
										case 0: //move to server section, add it to server section
											block.SectIndex = 1;
											block.AddAction(_taskIdList[i]);
											break;
										case 1: //add it to server section
											block.AddAction(_taskIdList[i]);
											break;
										case 2: //create a new block, move the new block to server section, add it to new block
											block.BlockType = EnumWebHandlerBlockType.XmlHttp;
											block = new WebHandlerBlock();
											blocks.Add(block);
											block.SectIndex = 1;
											block.AddAction(_taskIdList[i]);
											break;
									}
									List<IActionMethodPointer> tbls = new List<IActionMethodPointer>();
									List<UInt32> usedMethods = new List<uint>();
									MethodClass.CollectActionsByOwnerType<EasyDataSet>(act, tbls, usedMethods);
									if (hmd != null && hmd.HandlerMethod != null)
									{
										hmd.HandlerMethod.FindActionsByOwnerType<EasyDataSet>(tbls, usedMethods);
									}
									if (tbls.Count > 0)
									{
										foreach (IActionMethodPointer tbl in tbls)
										{
											if (string.CompareOrdinal("Update", tbl.MethodName) == 0)
											{
												if (tbl.Owner != null)
												{
													EasyDataSet eds = tbl.Owner.ObjectInstance as EasyDataSet;
													if (eds != null)
													{
														block.AddUpdateTableName(eds.TableName);
													}
												}
											}
										}
									}
									if (act != null && act.ReturnReceiver != null)
									{
										if (act.ReturnReceiver.RunAt == EnumWebRunAt.Client)
										{
											PropertyPointer pp = act.ReturnReceiver as PropertyPointer;
											if (pp != null)
											{
												TaskID tidNew = new TaskID();
												ActionClass actNew = new ActionClass(root);
												tidNew.TaskId = (UInt32)(Guid.NewGuid().GetHashCode());
												actNew.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
												tidNew.ClassId = root.ClassId;
												tidNew.ActionId = actNew.ActionId;
												SetterPointer sp = new SetterPointer(actNew);
												sp.SetProperty = pp;
												ParameterValue pv = new ParameterValue(actNew);
												pv.ValueType = EnumValueType.ConstantValue;
												pv.SetConstValueAndType(new ConstObjectPointerJsValue(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", pp.DataPassingCodeName)));
												sp.Value = pv;
												tidNew.SetAction(actNew);
												block.Sect2.Add(tidNew);
											}
										}
									}
									break;
								case EnumWebActionType.Download: //can only be added to download section
									switch (block.SectIndex)
									{
										case 0: //move to download and add
											block.SectIndex = 2;
											block.AddAction(_taskIdList[i]);
											break;
										case 1: //move to download and add
											block.SectIndex = 2;
											block.AddAction(_taskIdList[i]);
											break;
										case 2: //add
											block.AddAction(_taskIdList[i]);
											break;
									}
									if (act != null && act.MethodOwner != null && act.ActionMethod != null)
									{
										//find server functions in parameters and condition
										ParameterValueCollection pvs = act.ParameterValues;
										if (pvs != null && pvs.Count > 0)
										{
											foreach (ParameterValue p in pvs)
											{
												if (p.ValueType == EnumValueType.MathExpression)
												{
													MathNodeRoot r = p.MathExpression as MathNodeRoot;
													block.AddUpdateTableNamesFromExpression(r);
												}
											}
										}
										if (act.ActionCondition != null)
										{
											MathNodeRoot r = act.ActionCondition.GetExpression();
											block.AddUpdateTableNamesFromExpression(r);
										}
									}
									break;
							}
							break;
						case EnumActClass.Dialog: //can be in client/download, must create a new block after it
							switch (block.SectIndex)
							{
								case 0:
									block.AddAction(_taskIdList[i]); //add to client
									//create new block
									block.BlockType = EnumWebHandlerBlockType.Dialog;
									block = new WebHandlerBlock();
									blocks.Add(block);
									break;
								case 1:
									//move to download, add it
									block.SectIndex = 2;
									block.AddAction(_taskIdList[i]);
									//create new block
									block.BlockType = EnumWebHandlerBlockType.Dialog;
									block = new WebHandlerBlock();
									blocks.Add(block);
									break;
								case 2:
									//add to download
									block.AddAction(_taskIdList[i]);
									//create new block
									block.BlockType = EnumWebHandlerBlockType.Dialog;
									block = new WebHandlerBlock();
									blocks.Add(block);
									break;
							}
							break;
						case EnumActClass.Submit:
							switch (block.SectIndex)
							{
								case 0:
									//add it to client
									block.AddAction(_taskIdList[i]);
									//move it to server section
									block.SectIndex = 1;
									break;
								case 1:
									//create a new block
									block = new WebHandlerBlock();
									//put it on client section
									block.AddAction(_taskIdList[i]);
									//move it to server section
									block.SectIndex = 1;
									blocks.Add(block);
									break;
								case 2:
									//add it to download section
									block.AddAction(_taskIdList[i]);
									//move it to server section
									block.SectIndex = 1;
									break;
							}
							break;
					}
					if (block.SectIndex == 1)
						lastRunAt = EnumWebRunAt.Server;
					else
						lastRunAt = EnumWebRunAt.Client;
				}
			}
			if (blocks.Count > 0)
			{
				if (blocks[blocks.Count - 1].IsEmpty)
				{
					blocks.RemoveAt(blocks.Count - 1);
				}
			}
			if (blocks.Count > 0)
			{
				int serverBlocks = 0;
				SourceValuePointerList states = new SourceValuePointerList();
				foreach (WebHandlerBlock wb in blocks)
				{
					if (wb.UseServer)
					{
						serverBlocks++;
					}
				}
				if (serverBlocks > 1)
				{
					foreach (WebHandlerBlock wb in blocks)
					{
						if (wb.UseServer)
						{
							IList<ISourceValuePointer> l = wb.GetServerVariables(root);
							if (l != null && l.Count > 0)
							{
								foreach (ISourceValuePointer v in l)
								{
									states.AddUnique(v);
								}
							}
						}
					}
				}
				if (this.RootPointer.Project.ProjectType == VSPrj.EnumProjectType.WebAppAspx)
				{
					states.RemoveReadOnlyValues();
				}
				blocks.SetServerStates(states);
				if (states.Count > 0)
				{
					List<IAction> uploadActions = new List<IAction>();
					foreach (ISourceValuePointer v in states)
					{
						ActionClass act = new ActionClass(root);
						act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
						act.ActionName = string.Format(CultureInfo.InvariantCulture, "Upload{0}", v.DataPassingCodeName);
						SetterPointer sp = new SetterPointer(act);
						IProperty prop = v as IProperty;
						if (prop == null)
						{
							prop = v.ValueOwner as IProperty;
						}
						if (prop == null)
						{
							prop = new SeverStatePointer(v, root, EnumWebRunAt.Server);
							sp.Owner = v.ValueOwner as IObjectPointer;
						}
						else
						{
							sp.Owner = prop.Owner;
						}
						sp.SetProperty = prop;
						ParameterValue pv = new ParameterValue(act);
						pv.Name = "value";
						pv.SetDataType(prop.PropertyType);
						pv.ValueType = EnumValueType.Property;
						pv.Property = new SeverStatePointer(v, root, EnumWebRunAt.Client);
						ParameterValueCollection pc = new ParameterValueCollection();
						pc.Add(pv);
						act.ParameterValues = pc;
						uploadActions.Add(act);
					}
					//check download needs
					for (int k = 0; k < blocks.Count - 1; k++)
					{
						if (blocks[k].UseServer)
						{
							for (int n = k + 1; n < blocks.Count; n++)
							{
								if (blocks[n].UseServer)
								{
									blocks[k].BlockProcess |= EnumWebBlockProcess.Download;
									break;
								}
							}
						}
					}
					//check upload needs
					for (int k = blocks.Count - 1; k > 0; k--)
					{
						if (blocks[k].UseServer)
						{
							for (int n = 0; n < k; n++)
							{
								if (blocks[n].UseServer)
								{
									blocks[k].BlockProcess |= EnumWebBlockProcess.Upload;
									//add upload action
									foreach (IAction a in uploadActions)
									{
										TaskID tid = new TaskID(a.ActionId, root.ClassId);
										tid.SetAction(a);
										blocks[k].Sect1.Insert(0, tid);
									}
									break;
								}
							}
						}
					}
				}
			}
			return blocks;
		}
		#endregion

		#region IBreakPointOwner Members
		[DefaultValue(false)]
		public bool BreakBeforeExecute { get; set; }
		[DefaultValue(false)]
		public bool BreakAfterExecute { get; set; }

		#endregion

		#region ISerializeParent Members

		public void OnMemberCreated(object member)
		{
			TaskIdList tl = member as TaskIdList;
			if (tl != null)
			{
				tl.SetEvent(_eventPointer);
			}
			else
			{
				TaskID tid = member as TaskID;
				if (tid != null)
				{
					tid.SetEvent(_eventPointer);
				}
			}
		}

		#endregion

		#region IBeforeSerializeNotify Members

		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reader = reader;
			_handlerXmlNode = node;
		}
		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_writer = writer;
			_handlerXmlNode = node;
		}
		public void ReloadFromXmlNode()
		{
			if (_reader != null && _handlerXmlNode != null)
			{
				_taskIdList = new TaskIdList();
				_reader.ReadObjectFromXmlNode(_handlerXmlNode, this, this.GetType(), null);
			}
		}
		public void UpdateXmlNode(XmlObjectWriter writer)
		{
			if (_handlerXmlNode != null)
			{
				if (writer != null)
				{
					writer.WriteObjectToNode(_handlerXmlNode, this);
				}
				else if (Writer != null)
				{
					_writer.WriteObjectToNode(_handlerXmlNode, this);
				}
			}
		}
		[Browsable(false)]
		public XmlNode XmlData { get { return _handlerXmlNode; } }

		#endregion
	}
}
