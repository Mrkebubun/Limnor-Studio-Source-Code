using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using XmlSerializer;
using XmlUtility;
using System.Globalization;
using MathExp;
using VSPrj;
using VPL;
using ProgElements;
using System.Windows.Forms;
using System.CodeDom;
using System.Collections.Specialized;
using LimnorDesigner.MethodBuilder;
using System.Drawing.Design;
using LimnorDatabase;
using LimnorDesigner.Event;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// it cannot be used inside a MethodClass.
	/// Inside a MethodClass, use AB_AssignActions
	/// </summary>
	[UseParentObject]
	public class ActionAttachEvent : IAction
	{
		#region fields and constructors
		private UInt32 _actId;
		private ClassPointer _class;
		private EventAction _eventAction;
		private ExpressionValue _condition;
		//private string _name;
		private XmlNode _xmlNode;
		private bool _reading;
		private EnumWebActionType _webActType;
		private EventHandler NameChanging;
		private EventHandler PropertyChanged;
		public ActionAttachEvent(ClassPointer owner)
		{
			_class = owner;
		}
		#endregion
		#region private methods
		private void fireValueChange(EventArgs e)
		{
			if (PropertyChanged != null && !_reading)
			{
				PropertyChanged(this, e);
			}
		}
		protected virtual void loadEventAction()
		{
			if (_eventAction == null)
			{
				//it is public because event-handler does not belong to the scope method of this action branch.
				XmlNode node = _class.XmlData.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}/{1}[@{2}='{3}']",
					XmlTags.XML_HANDLERLISTS, XmlTags.XML_HANDLER, XmlTags.XMLATT_ActionID, this.ActionId));
				if (node != null)
				{
					Guid g = Guid.NewGuid();
					XmlObjectReader xr = _class.ObjectList.Reader;
					xr.ResetErrors();
					xr.ClearDelayedInitializers(g);
					_eventAction = (EventAction)xr.ReadObject(node, null);
					xr.OnDelayedInitializeObjects(g);
					if (xr.Errors != null && xr.Errors.Count > 0)
					{
						MathNode.Log(xr.Errors);
					}
				}
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public virtual bool IsAttach
		{
			get
			{
				return true;
			}
		}
		[Description("Gets name for the event")]
		public string Event
		{
			get
			{
				EventAction ea = AssignedActions;
				if (ea != null && ea.Event != null)
				{
					return ea.Event.DisplayName;
				}
				return string.Empty;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorHandlerMethod), typeof(UITypeEditor))]
		[Description("Gets handler for the event")]
		public string Handler
		{
			get
			{
				EventHandlerMethod ehm = GetHandlerMethod();
				if (ehm != null)
				{
					return ehm.DisplayName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public EventAction AssignedActions
		{
			get
			{
				loadEventAction();
				return _eventAction;
			}
		}
		[Browsable(false)]
		protected EventAction AssignedActions0
		{
			get
			{
				return _eventAction;
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public void SetHandlerOwner(EventAction handlerOwner)
		{
			_eventAction = handlerOwner;
		}
		[Browsable(false)]
		public EventHandlerMethod GetHandlerMethod()
		{
			loadEventAction();
			if (_eventAction != null)
			{
				HandlerMethodID hmid;
				EventHandlerMethod ehm = null;
				if (_eventAction.TaskIDList.Count > 0)
				{
					hmid = _eventAction.TaskIDList[0] as HandlerMethodID;
					if (hmid != null)
					{
						ehm = hmid.HandlerMethod;
					}
				}
				if (ehm == null)
				{
					hmid = new HandlerMethodID();
					ehm = new EventHandlerMethod(_eventAction.RootPointer);
					hmid.HandlerMethod = ehm;
					_eventAction.TaskIDList.Clear();
					_eventAction.TaskIDList.Add(hmid);
				}
				return ehm;
			}
			return null;
		}
		#endregion
		#region IAction Members
		[Browsable(false)]
		public bool IsMethodReturn
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsLocal
		{
			get { return false; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool AsLocal
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public bool IsPublic
		{
			get { return true; }
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				loadEventAction();
				if (_eventAction != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_eventAction is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the action is hidden from the end-users. An end-user programming system allows end-users to arrange actions-events relations. By default all global(public) actions can be used by the end-users. Set this property to True to hide the action from the end-user.")]
		public bool HideFromRuntimeDesigners
		{
			get;
			set;
		}
		[Browsable(false)]
		public bool IsStaticAction
		{
			get
			{
				if (_eventAction != null && _eventAction.Event != null)
				{
					return _eventAction.Event.IsStatic;
				}
				return false;
			}
		}
		[Browsable(false)]
		public uint ActionId
		{
			get
			{
				if (_actId == 0)
				{
					_actId = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _actId;
			}
			set
			{
				_actId = value;
			}
		}
		[Browsable(false)]
		public uint ClassId
		{
			get
			{
				return _class.ClassId;
			}
		}
		[Browsable(false)]
		public ClassPointer Class
		{
			get { return _class; }
		}
		[Browsable(false)]
		public uint ExecuterClassId
		{
			get { return _class.ClassId; }
		}
		[Browsable(false)]
		public uint ExecuterMemberId
		{
			get
			{
				loadEventAction();
				if (_eventAction != null && _eventAction.Event != null)
				{
					if (_eventAction.Event.Owner != null)
					{
						if (_eventAction.Event.Owner.ObjectInstance != null)
						{
							return _class.ObjectList.GetObjectID(_eventAction.Event.Owner.ObjectInstance);
						}
					}
				}
				return _class.MemberId;
			}
		}
		[Browsable(false)]
		public ClassPointer ExecuterRootHost
		{
			get { return _class; }
		}
		[Browsable(false)]
		public IObjectPointer EventOwner
		{
			get
			{
				loadEventAction();
				if (_eventAction != null && _eventAction.Event != null)
				{
					return _eventAction.Event.Owner;
				}
				return null;
			}
		}
		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		public virtual string ActionName
		{
			get
			{
				loadEventAction();
				if (_eventAction != null && _eventAction.Event != null)
				{
					EventHandlerMethod ehm = null;
					if (_eventAction.TaskIDList.Count > 0)
					{
						HandlerMethodID hmd = _eventAction.TaskIDList[0] as HandlerMethodID;
						if (hmd != null)
						{
							ehm = hmd.HandlerMethod;
						}
					}
					if (ehm != null)
					{
						return string.Format(CultureInfo.InvariantCulture, "Attach_{0}_to_{1}", ehm.Name, _eventAction.Event.ExpressionDisplay);
					}
					else
					{
						return string.Format(CultureInfo.InvariantCulture, "Attach_?_to_{0}", _eventAction.Event.ExpressionDisplay);
					}
				}
				else
				{
					return "Attach_?_to_?";
				}
				//if (string.IsNullOrEmpty(_name))
				//{
				//    if (_eventAction != null && _eventAction.Event != null)
				//    {
				//        _name = string.Format(CultureInfo.InvariantCulture, "handle {0}", _eventAction.Event.Name);
				//    }
				//}
				//return _name;
			}
			set
			{
				//if (_name != value)
				//{
				//    bool cancel = false;
				//    if (NameChanging != null)
				//    {
				//        NameBeforeChangeEventArg nc = new NameBeforeChangeEventArg(_name, value);
				//        NameChanging(this, nc);
				//        cancel = nc.Cancel;
				//    }
				//    if (!cancel)
				//    {
				//        _name = value;
				//        fireValueChange(new PropertyChangeEventArg("ActionName"));
				//    }
				//}
			}
		}
		[Browsable(false)]
		public string Display
		{
			get { return ActionName; }
		}

		public string Description
		{
			get;
			set;
		}
		[Browsable(false)]
		public Type ViewerType
		{
			get { return null; }
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get { return null; }
		}
		[Browsable(false)]
		public IObjectPointer MethodOwner
		{
			get { return _class; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get { return 0; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public ParameterValueCollection ParameterValues
		{
			get
			{
				return new ParameterValueCollection();
			}
			set
			{
			}
		}
		[Browsable(false)]
		public XmlNode CurrentXmlData
		{
			get { return _xmlNode; }
		}
		[Browsable(false)]
		public bool HasChangedXmlData
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsSameMethod(IAction act)
		{
			ActionAttachEvent aa = act as ActionAttachEvent;
			if (aa != null)
			{
				return (aa.ActionId == this.ActionId);
			}
			return false;
		}
		[Browsable(false)]
		public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
		{
			return false;
		}
		[Browsable(false)]
		public void Execute(List<ParameterClass> eventParameters)
		{

		}
		[Browsable(false)]
		public virtual void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
		{
			loadEventAction();
			if (_eventAction != null)
			{
				_eventAction.AttachCodeDomAction(methodToCompile, statements, debug);
				//ClassPointer root = _class;
				//CodeExpression methodTarget;
				//CodeEventReferenceExpression ceRef = _eventAction.Event.GetReferenceCode(methodToCompile, statements, false) as CodeEventReferenceExpression;
				//if (_eventAction.Event.IsStatic)
				//    methodTarget = new CodeTypeReferenceExpression(root.CodeName);
				//else
				//    methodTarget = new CodeThisReferenceExpression();
				//CodeAttachEventStatement caes = new CodeAttachEventStatement(ceRef,
				//                        new CodeDelegateCreateExpression(new CodeTypeReference(_eventAction.Event.EventHandlerType.TypeString),
				//                            methodTarget, _eventAction.GetLocalHandlerName()));
				//statements.Add(caes);
			}
		}
		[Browsable(false)]
		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			CreateJavaScript(methodToCompile, data.FormSubmissions, nextAction == null ? null : nextAction.InputName, Indentation.GetIndent());
		}
		[Browsable(false)]
		public virtual void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{

		}
		[Browsable(false)]
		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		[Browsable(false)]
		public uint ScopeMethodId
		{
			get { return 0; }
		}
		[Browsable(false)]
		public uint SubScopeId
		{
			get { return 0; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IMethod ScopeMethod
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IActionsHolder ActionHolder
		{
			get
			{
				return _class;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public void ResetScopeMethod()
		{
		}
		[Browsable(false)]
		public IAction CreateNewCopy()
		{
			ActionAttachEvent aa = new ActionAttachEvent(_class);
			if (_eventAction != null)
			{
				aa._eventAction = _eventAction;
			}
			return aa;
		}

		/// <summary>
		/// compile into if(expression){...} when it is not null
		/// </summary>
		[Editor(typeof(TypeEditorExpressionValue), typeof(UITypeEditor))]
		[Description("Condition for executing this action")]
		public ExpressionValue ActionCondition
		{
			get
			{
				if (_condition == null)
				{
					_condition = new ExpressionValue();
				}
				_condition.ScopeMethod = ScopeMethod;
				return _condition;
			}
			set
			{
				if (value != null)
				{
					_condition = value;
					if (ScopeMethod != null)
					{
						_condition.ScopeMethod = ScopeMethod;
					}
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool Changed
		{
			get;
			set;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer ReturnReceiver
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public void SetParameterValue(string name, object value)
		{
		}
		[Browsable(false)]
		public void ValidateParameterValues()
		{
		}
		[Browsable(false)]
		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			NameChanging = nameChange;
			PropertyChanged = propertyChange;
		}
		[Browsable(false)]
		public EventHandler GetPropertyChangeHandler()
		{
			return PropertyChanged;
		}
		[Browsable(false)]
		public void EstablishObjectOwnership(IActionsHolder scope)
		{

		}
		[Browsable(false)]
		public void ResetDisplay()
		{

		}
		[Browsable(false)]
		public LimnorProject Project
		{
			get { return _class.Project; }
		}
		[Browsable(false)]
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
			_reading = false;
		}
		[Browsable(false)]
		public virtual void CreateJavaScript(StringCollection methodCode, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			loadEventAction();
			if (_eventAction != null)
			{
				_eventAction.AttachJavascriptAction(this.ActionId, methodCode, indent);
				//if (_eventAction.IsExtendWebClientEvent())
				//{
				//    methodCode.Add("JsonDataBinding.attachExtendedEvent('");
				//    methodCode.Add(_eventAction.Event.Name);
				//    methodCode.Add("','");
				//    EasyDataSet eds = _eventAction.Event.Owner.ObjectInstance as EasyDataSet;
				//    if (eds != null)
				//    {
				//        methodCode.Add(eds.TableName);
				//    }
				//    else
				//    {
				//        methodCode.Add(_eventAction.Event.Owner.CodeName);
				//    }
				//    methodCode.Add("',");
				//    methodCode.Add(_eventAction.GetLocalHandlerName());
				//    methodCode.Add(");\r\n");
				//}
				//else
				//{
				//    IJavaScriptEventOwner eo = _eventAction.Event.Owner.ObjectInstance as IJavaScriptEventOwner;
				//    if (eo != null)
				//    {
				//        string fn = EventAction.GetAttachFunctionName(this.ActionId);
				//        eo.AttachJsEvent(_eventAction.Event.Owner.CodeName, _eventAction.Event.Name, fn, methodCode);
				//    }
				//    else
				//    {
				//        methodCode.Add("var ");
				//        methodCode.Add(_eventAction.Event.Owner.CodeName);
				//        methodCode.Add(" = document.getElementById('");
				//        methodCode.Add(_eventAction.Event.Owner.CodeName);
				//        methodCode.Add("');\r\n");
				//        methodCode.Add("JsonDataBinding.AttachEvent(");
				//        methodCode.Add(_eventAction.Event.Owner.CodeName);
				//        methodCode.Add(",'");
				//        methodCode.Add(_eventAction.Event.Name);
				//        methodCode.Add("',");
				//        methodCode.Add(_eventAction.GetLocalHandlerName());
				//        methodCode.Add(");\r\n");
				//    }
				//}
			}
		}
		[Browsable(false)]
		public EnumWebActionType WebActionType
		{
			get { return _webActType; }
		}
		[Browsable(false)]
		public void CheckWebActionType()
		{
			loadEventAction();
			_webActType = EnumWebActionType.Unknown;
			EnumWebValueSources sources = EnumWebValueSources.Unknown;
			if (_condition != null)
			{
				IList<ISourceValuePointer> conditionSource = _condition.MathExp.GetValueSources();
				if (conditionSource != null)
				{
					sources = WebBuilderUtil.GetActionTypeFromSources(conditionSource);
				}
			}
			EnumWebRunAt sr = ScopeRunAt;
			if (sr == EnumWebRunAt.Client)
			{
				if (sources == EnumWebValueSources.HasBothValues || sources == EnumWebValueSources.HasServerValues)
				{
					_webActType = EnumWebActionType.Download;
				}
				else
				{
					_webActType = EnumWebActionType.Client;
				}
			}
			else if (sr == EnumWebRunAt.Server)
			{
				if (sources == EnumWebValueSources.HasBothValues || sources == EnumWebValueSources.HasClientValues)
				{
					_webActType = EnumWebActionType.Upload;
				}
				else
				{
					_webActType = EnumWebActionType.Server;
				}
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetClientProperties(uint taskId)
		{
			return null;
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetServerProperties(uint taskId)
		{
			return null;
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetUploadProperties(uint taskId)
		{
			return null;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public EnumWebRunAt ScopeRunAt
		{
			get
			{
				if (_eventAction != null && _eventAction.Event != null)
				{
					return _eventAction.Event.RunAt;
				}
				return EnumWebRunAt.Unknown;
			}
			set
			{

			}
		}

		#endregion

		#region IObjectIdentity Members
		[Browsable(false)]
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ActionAttachEvent aa = objectIdentity as ActionAttachEvent;
			if (aa != null)
			{
				return (this.ActionId == aa.ActionId);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return _class; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get { return IsStaticAction; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Both; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Action; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return this;
		}

		#endregion

		#region IEventHandler Members
		[Browsable(false)]
		public ulong WholeActionId
		{
			get { return DesignUtil.MakeDDWord(_actId, ClassId); }
		}

		#endregion

		#region IBreakPointOwner Members
		private bool _breakBefore;
		[DefaultValue(false)]
		[Description("This property is used only for visual debugging. If it is True then before executing this action the debugger pauses and shows a break pointer at this action.")]
		public bool BreakBeforeExecute
		{
			get
			{
				return _breakBefore;
			}
			set
			{
				if (_breakBefore != value)
				{
					_breakBefore = value;
					fireValueChange(new PropertyChangeEventArg("BreakBeforeExecute"));
				}
			}
		}
		private bool _breakAfter;
		[DefaultValue(false)]
		[Description("This property is used only for visual debugging. If it is True then after executing this action the debugger pauses and shows a break pointer at this action.")]
		public bool BreakAfterExecute
		{
			get
			{
				return _breakAfter;
			}
			set
			{
				if (_breakAfter != value)
				{
					_breakAfter = value;
					fireValueChange(new PropertyChangeEventArg("BreakAfterExecute"));
				}
			}
		}

		#endregion

		#region IBeforeSerializeNotify Members
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		public virtual void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reading = true;
			_xmlNode = node;
			_reader = reader;
			//_name = XmlUtil.GetNameAttribute(node);
			_actId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ActionID);
		}

		public virtual void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_xmlNode = node;
			_writer = writer;
			//XmlUtil.SetNameAttribute(node, _name);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ActionID, ActionId);
		}

		public virtual void ReloadFromXmlNode()
		{
			if (_reader != null && _xmlNode != null)
			{
				Description = null;
				_breakBefore = false;
				_breakAfter = false;
				_condition = null;
				_reading = true;
				_reader.ReadObjectFromXmlNode(_xmlNode, this, this.GetType(), null);
				//ActionName = XmlUtil.GetAttribute(_xmlNode, XmlTags.XMLATT_NAME);
				Changed = false;
				_reading = false;
			}
		}

		public virtual void UpdateXmlNode(XmlObjectWriter writer)
		{
			if (_xmlNode != null)
			{
				if (writer != null)
				{
					writer.WriteObjectToNode(_xmlNode, this);
					Changed = false;
				}
				else if (_writer != null)
				{
					_writer.WriteObjectToNode(_xmlNode, this);
					Changed = false;
				}
			}
		}
		[Browsable(false)]
		public XmlNode XmlData
		{
			get { return _xmlNode; }
		}

		#endregion

		#region IActionContextExt Members

		public object GetParameterValue(string name)
		{
			return null;
		}

		#endregion

		#region IActionContext Members
		[Browsable(false)]
		public uint ActionContextId
		{
			get { return ActionId; }
		}
		[Browsable(false)]
		public object GetParameterType(uint id)
		{
			return null;
		}
		[Browsable(false)]
		public object GetParameterType(string name)
		{
			return null;
		}
		[Browsable(false)]
		public object ProjectContext
		{
			get { return Project; }
		}
		[Browsable(false)]
		public object OwnerContext
		{
			get { return IdentityOwner; }
		}
		[Browsable(false)]
		public IMethod ExecutionMethod
		{
			get { return null; }
		}
		[Browsable(false)]
		public void OnChangeWithinMethod(bool withinMethod)
		{
			if (_xmlNode != null)
			{
				if (_writer == null)
				{
					if (_reader != null)
					{
						_writer = new XmlObjectWriter(_reader.ObjectList);
					}
				}
				if (_writer != null)
				{
					_writer.WriteObjectToNode(_xmlNode, this);
				}
			}
		}

		#endregion

		#region IXmlNodeHolder Members

		[Browsable(false)]
		[ReadOnly(true)]
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
