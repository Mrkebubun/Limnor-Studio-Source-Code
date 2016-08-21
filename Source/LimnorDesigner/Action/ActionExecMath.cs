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
using MathExp;
using LimnorDesigner.MethodBuilder;
using ProgElements;
using System.CodeDom;
using System.Drawing.Design;
using VPL;
using VSPrj;
using System.Drawing;
using System.Windows.Forms;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using LimnorDesigner.Property;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// execute a math exp, IMathExpression.
	/// IMathExpression.OutputVariable is the variable to assign to.
	/// use ICustomTypeDescriptor to design action parameters.
	/// </summary>
	[UseParentObject]
	public abstract class ActionExecMath : ICustomTypeDescriptor, IEventHandler, IAction
	{
		#region fields and constructors
		private UInt32 _actId;
		private ClassPointer _class;
		private string _name;
		private string _desc;
		private bool _asLocal;
		private MathExpMethod _mathExp;
		private ExpressionValue _condition;
		private ParameterValueCollection _parameters;
		private IMethod _scopeMethod;
		private EventHandler _nameChanged;
		private EventHandler _valueChanged;
		//
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		private XmlNode _xmlNode;
		private XmlNode _xmlNodeChanged;
		private EnumWebActionType _webActType = EnumWebActionType.Unknown;
		//
		public ActionExecMath(ClassPointer owner)
		{
			_class = owner;
		}
		#endregion

		#region Properties
		[Browsable(false)]
		protected ExpressionValue Condition
		{
			get
			{
				return _condition;
			}
		}
		[Browsable(false)]
		public IMathExpression MathExp
		{
			get
			{
				if (_mathExp == null)
				{
					_mathExp = new MathExpMethod();
					_mathExp.Action = this;
				}
				return _mathExp.MathExpression;
			}
		}
		#endregion
		#region private methods
		private IList<ISourceValuePointer> getProperties(UInt32 taskId, bool client)
		{
			List<ISourceValuePointer> list = new List<ISourceValuePointer>();
			if (Condition != null)
			{
				IList<ISourceValuePointer> l1 = Condition.GetValueSources();
				if (l1 != null && l1.Count > 0)
				{
					foreach (ISourceValuePointer p in l1)
					{
						if (taskId != 0)
						{
							p.SetTaskId(taskId);
						}
						if (p.IsWebClientValue())
						{
							if (client)
							{
								list.Add(p);
							}
						}
						else
						{
							if (!client)
							{
								list.Add(p);
							}
						}
					}
				}
			}
			MathNodeRoot mathExp = MathExp as MathNodeRoot;
			if (mathExp != null)
			{
				List<ISourceValuePointer> l2 = new List<ISourceValuePointer>();
				mathExp.GetValueSources(l2);
				if (l2.Count > 0)
				{
					foreach (ISourceValuePointer p in l2)
					{
						bool found = false;
						if (taskId != 0)
						{
							p.SetTaskId(taskId);
						}
						foreach (ISourceValuePointer p2 in list)
						{
							if (p2.IsSameProperty(p))
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							if (p.IsWebClientValue())
							{
								if (client)
								{
									list.Add(p);
								}
							}
							else
							{
								if (!client)
								{
									list.Add(p);
								}
							}
						}
					}
				}
			}
			return list;
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			if (_mathExp != null)
			{
				return _mathExp.ToString();
			}
			return base.ToString();
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
		public virtual void ResetScopeMethod()
		{
		}
		#endregion

		#region IAction Members
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
		}
		private EnumWebRunAt _scopeRunat = EnumWebRunAt.Unknown;
		public EnumWebRunAt ScopeRunAt
		{
			get
			{
				if (_scopeMethod != null)
				{
					MethodClass mc = _scopeMethod as MethodClass;
					if (mc != null)
					{
						return mc.RunAt;
					}
				}
				return _scopeRunat;
			}
			set
			{
				_scopeRunat = value;
			}
		}
		/// <summary>
		/// An end-user programming system allows end-user to arrange actions-events relations. 
		/// By default all global(public) actions can be used by the end-users.
		/// Set this property to True to hide the action from the end-user
		/// </summary>
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the action is hidden from the end-users. An end-user programming system allows end-users to arrange actions-events relations. By default all global(public) actions can be used by the end-users. Set this property to True to hide the action from the end-user.")]
		public bool HideFromRuntimeDesigners { get; set; }

		public void EstablishObjectOwnership(IActionsHolder scope)
		{
		}
		public EnumWebActionType WebActionType
		{
			get { return _webActType; }
		}
		public void CheckWebActionType()
		{
			_webActType = EnumWebActionType.Unknown;
			EnumWebValueSources sources = EnumWebValueSources.Unknown;
			IList<ISourceValuePointer> conditionSource = null;
			if (_condition != null)
			{
				conditionSource = _condition.MathExp.GetValueSources();
				if (conditionSource != null)
				{
					sources = WebBuilderUtil.GetActionTypeFromSources(conditionSource);
				}
			}
			if (_mathExp != null)
			{
				if (_mathExp.MathExpression != null)
				{
					IList<ISourceValuePointer> lst = _mathExp.MathExpression.GetValueSources();
					if (lst != null && lst.Count > 0)
					{
						sources = WebBuilderUtil.GetActionTypeFromSources(lst, sources);
					}
				}
			}
			bool executerIsClient = false;
			MethodClass mc = ScopeMethod as MethodClass;
			if (mc != null)
			{
				if (mc.WebUsage == EnumMethodWebUsage.Client)
				{
					executerIsClient = true;
				}
			}
			else
			{
				if (_mathExp != null && _mathExp.Owner != null)
				{
					if (DesignUtil.IsWebClientObject(_mathExp.Owner))
					{
						executerIsClient = true;
					}
				}
				else
				{
					if (_reader != null)
					{
						if (DesignUtil.IsWebClientObject(_reader.ObjectList.RootPointer as IClass))
						{
							executerIsClient = true;
						}
					}
				}
			}
			switch (sources)
			{
				case EnumWebValueSources.HasBothValues:
					if (executerIsClient)
					{
						_webActType = EnumWebActionType.Download;
					}
					else
					{
						_webActType = EnumWebActionType.Upload;
					}
					break;
				case EnumWebValueSources.HasClientValues:
					_webActType = EnumWebActionType.Client;
					break;
				case EnumWebValueSources.HasServerValues:
					_webActType = EnumWebActionType.Server;
					break;
			}
		}


		[Browsable(false)]
		public virtual bool IsValid
		{
			get
			{
				if (_mathExp != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_mathExp is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public ClassPointer ExecuterRootHost { get { return _class; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public bool Changed { get; set; }
		public ExpressionValue ActionCondition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}
		[Browsable(false)]
		public abstract bool IsMethodReturn { get; }
		[Browsable(false)]
		public bool IsPublic
		{
			get
			{
				return !IsLocal && !AsLocal;
			}
		}
		/// <summary>
		/// the action uses method/event parameters and is exclusively used for one method/event only
		/// </summary>
		[Browsable(false)]
		public bool AsLocal
		{
			get
			{
				return _asLocal;
			}
			set
			{
				_asLocal = value;
			}
		}
		/// <summary>
		/// the executer is a local variable
		/// </summary>
		[Browsable(false)]
		public virtual bool IsLocal
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public UInt32 SubScopeId { get; set; }

		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 ScopeMethodId { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod ScopeMethod
		{
			get
			{
				return _scopeMethod;
			}
			set
			{
				_scopeMethod = value;
				if (_scopeMethod != null)
				{
					MethodClass mc = _scopeMethod as MethodClass;
					if (mc != null)
					{
						ScopeMethodId = mc.MemberId;
					}
					List<ParameterValue> ps = ParameterValues;
					if (ps != null)
					{
						foreach (ParameterValue p in ps)
						{
							p.ScopeMethod = _scopeMethod;
						}
					}
				}
			}
		}
		[Browsable(false)]
		public UInt32 ActionId
		{
			get { return _actId; }
			set { _actId = value; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				return _class.ClassId;
			}
		}
		[Browsable(false)]
		public ClassPointer Class
		{
			get
			{
				return _class;
			}
		}
		[Browsable(false)]
		public UInt32 ExecuterClassId
		{
			get
			{
				return ClassId; //executer is the same
			}
		}
		[Browsable(false)]
		public UInt32 ExecuterMemberId
		{
			get
			{
				return 1; //host is the executer
			}
		}
		[Browsable(false)]
		public string Display
		{
			get
			{
				return ToString();
			}
		}
		[Browsable(false)]
		public void ResetDisplay()
		{

		}
		[DesignOnly(true)]
		[ParenthesizePropertyName(true)]
		public string ActionName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					if (_mathExp != null)
					{
						_name = _mathExp.Name;
					}
				}
				return _name;
			}
			set
			{
				if (string.Compare(_name, value, StringComparison.Ordinal) == 0)
				{
					_name = value;
					if (_nameChanged != null)
					{
						_nameChanged(this, EventArgs.Empty);
					}
				}
			}
		}

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
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (_scopeMethod != null)
				{
					return _scopeMethod.IsStatic;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsStaticAction
		{
			get
			{
				if (_scopeMethod != null)
				{
					if (_scopeMethod.IsStatic)
					{
						int n = ParameterCount;
						if (n == 0)
						{
							return true;
						}
						ParameterValueCollection pvs = ParameterValues;
						for (int i = 0; i < pvs.Count; i++)
						{
							ParameterValue pv = pvs[i];
							if (pv != null)
							{
								if (!pv.IsStatic)
								{
									return false;
								}
							}
						}
						return true;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public Type ViewerType
		{
			get
			{
				return typeof(ActionViewerSingleAction);
			}
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return ActionMethod.ActionBranchType;
			}
		}
		/// <summary>
		/// math exp does not have a method owner
		/// </summary>
		[Browsable(false)]
		public IObjectPointer MethodOwner
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return _mathExp;
			}
			set
			{
				_mathExp = (MathExpMethod)value;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (_mathExp != null)
				{
					if (_mathExp.MethodParameterTypes != null)
					{
						return _mathExp.MethodParameterTypes.Count;
					}
				}
				return 0;
			}
		}
		[Browsable(false)]
		public ParameterValueCollection ParameterValues
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new ParameterValueCollection();
				}
				_mathExp.ValidateParameterValues(_parameters);
				return _parameters;
			}
			set
			{
				_parameters = value;
				if (_valueChanged != null)
				{
					_valueChanged(this, EventArgs.Empty);
				}
			}
		}
		public void ValidateParameterValues()
		{
			if (_parameters == null)
			{
				_parameters = new ParameterValueCollection();
			}
			_mathExp.ValidateParameterValues(_parameters);
		}
		public virtual IAction CreateNewCopy()
		{
			ActionExecMath a = (ActionExecMath)Activator.CreateInstance(this.GetType(), _class);
			a._actId = (UInt32)(Guid.NewGuid().GetHashCode());
			a._asLocal = _asLocal;
			a._desc = _desc;
			if (_mathExp != null)
			{
				a._mathExp = (MathExpMethod)_mathExp.Clone();
			}
			if (string.IsNullOrEmpty(_name))
			{
				a._name = "Action" + Guid.NewGuid().GetHashCode().ToString("x");
			}
			else
			{
				if (_name.Length > 30)
				{
					a._name = _name.Substring(0, 30) + Guid.NewGuid().GetHashCode().ToString("x");
				}
				else
				{
					a._name = _name + "_" + Guid.NewGuid().GetHashCode().ToString("x");
				}
			}
			a.ScopeMethodId = ScopeMethodId;
			a.SubScopeId = SubScopeId;
			if (_parameters != null)
			{
				ParameterValueCollection ps = new ParameterValueCollection();
				foreach (ParameterValue pv in _parameters)
				{
					pv.SetCloneOwner(a);
					ps.Add((ParameterValue)pv.Clone());
				}
				a._parameters = ps;
			}
			return a;
		}
		public bool IsSameMethod(IAction act)
		{
			ActionExecMath ae = act as ActionExecMath;
			if (ae != null)
			{
				if (ae.WholeActionId == this.WholeActionId)
				{
					return true;
				}
			}
			return false;
		}
		public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				ActionExecMath a2 = (ActionExecMath)this.Clone();
				Rectangle rc = new Rectangle(0, 0, 100, 30);
				System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
				rc.X = curPoint.X;
				rc.Y = curPoint.Y;
				a2.MathExp.ScopeMethod = context;
				IMathEditor dlg = a2.MathExp.CreateEditor(rc);
				if (((Form)dlg).ShowDialog(caller) == DialogResult.OK)
				{
					MathExpMethod mem = new MathExpMethod();
					mem.Action = this;
					mem.MathExpression = dlg.MathExpression;
					ActionMethod = mem;
					if (_class != null)
					{
						LimnorProject project = _class.Project;
						ILimnorDesignPane pane = project.GetTypedData<ILimnorDesignPane>(_class.ClassId);
						if (pane != null)
						{
							pane.OnActionChanged(_class.ClassId, this, isNewAction);
							pane.OnNotifyChanges();
						}
						else
						{
							DesignUtil.WriteToOutputWindowAndLog("Error Editng ActionExecMath. ClassPointer [{0}] is not in design mode when creating an action. Please close the design pane and re-open it.", _class.ClassId);
						}
					}
					return true;
				}
			}
			return false;
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			if (_mathExp != null && _mathExp.MathExpression != null && _mathExp.MathExpression.OutputVariable != null)
			{
				ISourceValuePointer v = _mathExp.MathExpression.OutputVariable as ISourceValuePointer;
				if (v != null)
				{
					if (v.IsWebServerValue() && !v.IsWebClientValue())
					{
						return GetClientProperties(taskId);
					}
				}
			}
			return null;
		}
		public IList<ISourceValuePointer> GetClientProperties(UInt32 taskId)
		{
			return getProperties(taskId, true);
		}
		public IList<ISourceValuePointer> GetServerProperties(UInt32 taskId)
		{
			return getProperties(taskId, false);
		}
		public void Execute(List<ParameterClass> eventParameters)
		{
		}
		/// <summary>
		/// execute the math exp.
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="methodToCompile"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="debug"></param>
		public abstract void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug);
		public abstract void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data);
		public abstract void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data);
		public abstract void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);


		public abstract void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formsumissions, string nextActionInput, string indent);
		public abstract void CreatePhpScript(StringCollection sb);
		[Browsable(false)]
		[ReadOnly(true)]
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

		public object GetParameterValue(string name)
		{
			if (_parameters == null)
			{
				_parameters = new ParameterValueCollection();
			}
			_mathExp.ValidateParameterValues(_parameters);
			return _parameters.GetParameterValue(name);
		}

		public void SetParameterValue(string name, object value)
		{
			if (_parameters == null)
			{
				_parameters = new ParameterValueCollection();
			}
			_mathExp.ValidateParameterValues(_parameters);
			_parameters.SetParameterValue(name, value);
		}
		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			_nameChanged = nameChange;
			_valueChanged = propertyChange;
		}
		public EventHandler GetPropertyChangeHandler()
		{
			return _valueChanged;
		}
		public LimnorProject Project
		{
			get
			{
				if (_class != null)
				{
					return _class.Project;
				}
				return null;
			}
		}
		/// <summary>
		/// XmlNode that represents action changes within a method.
		/// The changes may not have saved to the XmlDocument tree.
		/// </summary>
		public XmlNode CurrentXmlData
		{
			get
			{
				if (_xmlNodeChanged != null)
				{
					return _xmlNodeChanged;
				}
				return _xmlNode;
			}
		}
		public bool HasChangedXmlData
		{
			get
			{
				return (_xmlNodeChanged != null);
			}
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ActionExecMath aem = objectIdentity as ActionExecMath;
			if (aem != null)
			{
				return (aem.WholeActionId == this.WholeActionId);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Action; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return this; //do not clone action so that it can be edited in all places
		}

		#endregion

		#region IEventHandler Members
		[ReadOnly(true)]
		[Browsable(false)]
		public ulong WholeActionId
		{
			get
			{
				return DesignUtil.MakeDDWord(_actId, ClassId);
			}
		}

		#endregion

		#region IBreakPointOwner Members
		[Description("In visual debugging, pause the execution before this action if this property is true")]
		public bool BreakBeforeExecute
		{
			get;
			set;
		}
		[Description("In visual debugging, pause the execution after this action if this property is true")]
		public bool BreakAfterExecute
		{
			get;
			set;
		}

		#endregion

		#region DataValuePropertyDescriptor class definition
		class DataValuePropertyDescriptor : PropertyDescriptor
		{
			private ParameterValue _owner;
			public DataValuePropertyDescriptor(ParameterValue owner, string name, Attribute[] attrs) :
				base(name, attrs)
			{
				_owner = owner;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;// true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(ParameterValue);
				}
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override object GetValue(object component)
			{
				return _owner;
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
				_owner = (ParameterValue)value;
			}

			public override bool ShouldSerializeValue(object component)
			{

				return false;
			}
		}
		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public abstract PropertyDescriptorCollection GetProperties(Attribute[] attributes);

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IBeforeSerializeNotify Members
		[Browsable(false)]
		public XmlNode XmlData { get { return _xmlNode; } }
		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reader = reader;
			_xmlNode = node;
		}

		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_writer = writer;
			if (_xmlNodeChanged != node)
			{
				_xmlNode = node;
			}
		}
		public void ReloadFromXmlNode()
		{
			if (_reader != null && _xmlNode != null)
			{
				_reader.ReadObjectFromXmlNode(_xmlNode, this, this.GetType(), null);
				ActionName = XmlUtil.GetAttribute(_xmlNode, XmlTags.XMLATT_NAME);
				_xmlNodeChanged = null;
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
				else
				{
					if (Writer != null)
					{
						_writer.WriteObjectToNode(_xmlNode, this);
					}
				}
			}
		}

		#endregion

		#region IActionContext Members
		[Browsable(false)]
		public uint ActionContextId
		{
			get { return ActionId; }
		}

		public object GetParameterType(uint id)
		{
			if (_mathExp != null)
			{
				return _mathExp.GetParameterType(id);
			}
			return null;
		}

		public object GetParameterType(string name)
		{
			if (_mathExp != null)
			{
				return _mathExp.GetParameterType(name);
			}
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
			get { return _class; }
		}
		[Browsable(false)]
		public IMethod ExecutionMethod
		{
			get { return MathExp; }
		}
		[Browsable(false)]
		public void OnChangeWithinMethod(bool withinmethod)
		{
			if (Writer != null && _xmlNode != null)
			{
				if (withinmethod)
				{
					_xmlNodeChanged = _xmlNode.OwnerDocument.CreateElement(_xmlNode.Name);
					_writer.WriteObjectToNode(_xmlNodeChanged, this);
				}
				else
				{
					_writer.WriteObjectToNode(_xmlNode, this);
				}
			}
		}
		#endregion

		#region IAction Members
		private IActionsHolder _actsHolder;
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionsHolder ActionHolder
		{
			get
			{
				return _actsHolder;
			}
			set
			{
				_actsHolder = value;
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
