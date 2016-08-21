/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using XmlSerializer;
using System.Xml;
using ProgElements;
using LimnorDesigner.Property;
using VPL;
using VSPrj;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using System.CodeDom;
using System.Reflection;
using XmlUtility;
using MathExp;
using System.Drawing.Design;
using LimnorDesigner.Event;
using System.Collections.Specialized;
using System.Globalization;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// an action as a sub-method
	/// Array's ExecuteForEachItem, for, while, etc. 
	/// </summary>
	[UseParentObject]
	public class ActionSubMethod : IAction, ICustomTypeDescriptor, INonHostedObject, IUseClassId
	{
		#region fields and constructors
		private UInt32 _actId;
		private UInt32 _classId;
		private ClassPointer _class;
		private string _name;
		private string _desc;
		private bool _breakBefore;
		private bool _breakAfter;
		private bool _asLocal;
		private SubMethodInfoPointer _methodPointer;//SubMethodInfoPointer.ActionOwner points back to this class
		private ParameterValueCollection _parameterValues;
		//
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		private XmlNode _xmlNode;
		private XmlNode _xmlNodeChanged;
		//
		public ActionSubMethod(ClassPointer owner)
		{
			_class = owner;
			_classId = _class.ClassId;
		}
		#endregion
		#region properties (serialized)
		[Browsable(false)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return _methodPointer;
			}
			set
			{
				_methodPointer = (SubMethodInfoPointer)value;
				if (_methodPointer != null)
				{
					_methodPointer.ActionOwner = this;
					if (_parameterValues == null)
					{
						_parameterValues = new ParameterValueCollection();
					}
					_methodPointer.ValidateParameterValues(_parameterValues);
					OnSetActionMethod();
				}
			}
		}
		[Description("The name to identify this action")]
		[DesignOnly(true)]
		[ParenthesizePropertyName(true)]
		public string ActionName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					if (_methodPointer != null)
					{
						_name = _methodPointer.ToString();
					}
				}
				return _name;
			}
			set
			{
				if (_name != value)
				{
					bool cancel = false;
					if (NameChanging != null)
					{
						NameBeforeChangeEventArg nc = new NameBeforeChangeEventArg(_name, value, false);
						NameChanging(this, nc);
						cancel = nc.Cancel;
					}
					if (!cancel)
					{
						_name = value;
						if (PropertyChanged != null)
						{
							PropertyChanged(this, new PropertyChangeEventArg("ActionName"));
						}
					}
				}
			}
		}
		[Description("Description of this action.")]
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				if (_desc != value)
				{
					_desc = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("Description"));
					}
				}
			}
		}
		[Browsable(false)]
		[Description("A static action only invokes a static method and uses only static members for action arguments")]
		public bool IsStatic
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.IsStatic;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsStaticAction
		{
			get
			{
				if (_methodPointer != null)
				{
					if (_methodPointer.IsStatic)
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
		#endregion
		#region properties
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
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_methodPointer != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_methodPointer null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 ScopeMethodId { get; set; }

		[Browsable(false)]
		public UInt32 SubScopeId { get; set; }

		private IMethod _scopeMethod;
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
				MethodClass mc = _scopeMethod as MethodClass;
				if (mc != null)
				{
					ScopeMethodId = mc.MemberId;
				}
			}
		}
		//
		[Description("Data type of the result of this action if it returns a value")]
		public DataTypePointer ReturnValueType
		{
			get
			{
				return new DataTypePointer(new TypePointer(typeof(void)));
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ReturnValue
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
		public ClassPointer Class
		{
			get
			{
				if (_class == null && LimnorProject.ActiveProject != null)
				{
					_class = LimnorProject.ActiveProject.GetTypedData<ClassPointer>(_classId);
				}
				return _class;
			}
		}
		/// <summary>
		/// the class declaring this action
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				if (_class != null)
				{
					return _class.ClassId;
				}
				return _classId;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(_actId, ClassId);
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt64 WholeActionId
		{
			get
			{
				return WholeId;
			}
		}
		/// <summary>
		/// the class of the method/property declarer
		/// </summary>
		[Browsable(false)]
		public UInt32 ExecuterClassId
		{
			get
			{
				IClass holder = this.getActionExecuter();
				return holder.ClassId;
			}
		}
		/// <summary>
		/// the member id for the instance among hosting class
		/// [ClassId, ExecuterMemberId] identify the object instance
		/// ExecuterClassId identifies the declaring class of the object instance
		/// </summary>
		[Browsable(false)]
		public UInt32 ExecuterMemberId
		{
			get
			{
				IClass holder = this.getActionExecuter();
				return holder.MemberId;
			}
		}
		/// <summary>
		/// find the type of the immediate property/method.
		/// Library: The immediate property/method is from library
		/// Custom: The immediate property/method is of customer
		/// </summary>
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				IObjectIdentity o = this.ActionMethod as IObjectIdentity;
				if (o != null)
				{
					IClass c = o as IClass;
					if (c != null)
					{
						return c.ObjectDevelopType;
					}
					while (o != null)
					{
						c = o.IdentityOwner as IClass;
						if (c != null)
						{
							return o.ObjectDevelopType;
						}
						o = o.IdentityOwner;
					}
				}
				return EnumObjectDevelopType.Both;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Action; } }

		[Browsable(false)]
		public string MethodName
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.MethodName;
				}
				return string.Empty;
			}
		}
		#endregion
		#region methods
		void _methodPointer_ParameterValueChanged(object sender, EventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangeEventArg("ActionMethod"));
			}
		}

		protected virtual void OnSetActionMethod()
		{
		}
		public List<ParameterClassSubMethod> GetParameters(AB_SubMethodAction actionBranch)
		{
			if (_methodPointer == null)
				return new List<ParameterClassSubMethod>();
			return _methodPointer.GetParameters(actionBranch);
		}
		public override string ToString()
		{
			return Name;
		}
		public void ResetScopeMethod()
		{
		}
		#endregion
		#region ICloneable Members
		public object Clone()
		{
			//do not Clone action. keep a single instance of each action in ClassPointer so that it can be edited from many places
			return this;
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

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (VPLUtil.GetBrowseableProperties(attributes))
			{
				PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach (PropertyDescriptor p in ps)
				{
					if (string.CompareOrdinal(p.Name, "ReturnValueType") == 0)
					{
						if (_methodPointer == null || _methodPointer.NoReturn || !_methodPointer.HasReturn)
						{
							continue;
						}
					}
					list.Add(new PropertyDescriptorWrapper(p, this));
				}
				ps = new PropertyDescriptorCollection(list.ToArray());
				return ps;
			}
			else
			{
				return TypeDescriptor.GetProperties(this, attributes, true);
			}
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region IEventHandler Members
		[Browsable(false)]
		public UInt32 ActionId
		{
			get { return _actId; }
			set
			{
				_actId = value;
				if (_methodPointer != null)
				{
					IList<IParameter> ps = _methodPointer.MethodPointed.MethodParameterTypes;
					if (ps != null && ps.Count > 0)
					{
						foreach (IParameter p in ps)
						{
							ParameterClassSubMethod pcm = p as ParameterClassSubMethod;
							if (pcm != null)
							{
								pcm.Method = _methodPointer;
								pcm.ActionId = ActionId;
								pcm.SetClassId(ClassId);
							}
						}
					}
				}
			}
		}
		#endregion
		#region IAction Members
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
		}
		[Browsable(false)]
		public void ResetDisplay()
		{

		}
		private EnumWebRunAt _scopeRunat = EnumWebRunAt.Unknown;
		public EnumWebRunAt ScopeRunAt
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.RunAt;
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
		[Browsable(false)]
		public ClassPointer ExecuterRootHost { get { return _class; } }
		[Browsable(false)]
		public LimnorProject Project
		{
			get
			{
				return _class.Project;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool Changed { get; set; }
		[ReadOnly(true)]
		[Bindable(false)]
		public ExpressionValue ActionCondition
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
		public virtual bool IsMethodReturn
		{
			get
			{
				return false;
			}
		}
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
				if (_methodPointer != null)
				{
					return _methodPointer.IsForLocalAction;
				}
				return false;
			}
		}
		[Browsable(false)]
		public string Display
		{
			get
			{
				return this.ActionName;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (_methodPointer == null)
					return 0;
				return _methodPointer.ParameterCount;
			}
		}
		[Browsable(false)]
		public ParameterValueCollection ParameterValues
		{
			get
			{
				if (_parameterValues == null)
				{
					_parameterValues = new ParameterValueCollection();
					if (_methodPointer != null)
					{
						_methodPointer.ValidateParameterValues(_parameterValues);
					}
				}
				return _parameterValues;
			}
			set
			{
				_parameterValues = value;
			}
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return this._methodPointer;
			}
		}
		public IObjectPointer ReturnReceiver { get; set; }
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
		public void ValidateParameterValues()
		{
			if (_parameterValues == null)
			{
				_parameterValues = new ParameterValueCollection();
			}
			if (_methodPointer != null)
			{
				_methodPointer.ValidateParameterValues(_parameterValues);
			}
		}
		/// <summary>
		/// for easier programming
		/// </summary>
		/// <returns></returns>
		public IAction CreateNewCopy()
		{
			ActionSubMethod a = new ActionSubMethod(_class);
			a._classId = _classId;
			a._actId = (UInt32)(Guid.NewGuid().GetHashCode());
			a._asLocal = _asLocal;
			a._desc = _desc;
			if (_methodPointer != null)
			{
				a._methodPointer = (SubMethodInfoPointer)_methodPointer.Clone();
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
			return a;
		}
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ActionClass ac = objectIdentity as ActionClass;
			if (ac != null)
			{
				return (ac.ClassId == this.ClassId && ac.ActionId == this.ActionId);
			}
			return false;
		}
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
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("BreakBeforeExecute"));
					}
				}
			}
		}
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
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("BreakAfterExecute"));
					}
				}
			}
		}
		public bool IsSameMethod(IAction act)
		{
			return this.ActionMethod.IsSameMethod(act.ActionMethod);
		}
		public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
		{
			_class.SaveAction(this, writer);
			return true;
		}
		/// <summary>
		/// Actions do not use return values. Only math expressions use return values.
		/// If an action is the last action of a method then its return value is the return value of the method. But this logic is not implemented here. It is implemented in the higher level.
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
		{
			MessageBox.Show("ExportCode should not be called because ActionSubMethod can only be for AB_SubMethodAction", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			throw new DesignerException("ExportJavaScriptCode should not be called because ActionSubMethod can only be for AB_SubMethodAction");
		}
		public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			throw new DesignerException("ExportPhpScriptCode should not be called because ActionSubMethod can only be for AB_SubMethodAction");
		}
		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formsumissions, string nextActionInput, string indent)
		{
			//TBD:
		}
		public IList<ISourceValuePointer> GetClientProperties(UInt32 taskId)
		{
			return null;
		}
		public IList<ISourceValuePointer> GetServerProperties(UInt32 taskId)
		{
			return null;
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			return null;
		}
		public void Execute(List<ParameterClass> eventParameters)
		{
			MethodPointer mp = (MethodPointer)ActionMethod;
			MethodBase mif = mp.MethodDef;
			ParameterInfo[] pifs = mp.Info;
			object[] vs = new object[mp.ParameterCount];
			if (_parameterValues == null)
			{
				_parameterValues = new ParameterValueCollection();
			}
			mp.ValidateParameterValues(_parameterValues);
			for (int k = 0; k < mp.ParameterCount; k++)
			{
				vs[k] = null;
				IEventParameter iep = _parameterValues[k].AsEventParameter();
				if (iep != null)
				{
					if (eventParameters != null)
					{
						foreach (ParameterClass p in eventParameters)
						{
							if (iep.IsSameParameter(p))
							{
								vs[k] = p.ObjectInstance;
								break;
							}
						}
					}
				}
				else
				{
					vs[k] = _parameterValues[k].ObjectInstance;
				}
				if (vs[k] != null)
				{
					if (!pifs[k].ParameterType.Equals(vs[k].GetType()))
					{
						vs[k] = ValueTypeUtil.ConvertValueByType(pifs[k].ParameterType, vs[k]);
					}
				}
			}
			object ret;
			if (mif.IsStatic)
			{
				ret = mif.Invoke(null, vs);
			}
			else
			{
				ret = mif.Invoke(mp.ObjectInstance, vs);
			}
			MethodInfo minfo = mif as MethodInfo;
			if (minfo != null)
			{
				if (!typeof(void).Equals(minfo.ReturnType))
				{
					ReturnValue = ret;
				}
			}
		}
		private void onVauleChange(object sender, EventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangeEventArg("actionParameter"));
			}
		}
		/// <summary>
		/// It can be a ClassPointer, a MemberComponentId, or a TypePointer.
		/// 
		/// </summary>
		/// <returns></returns>
		private IClass getActionExecuter()
		{
			IPropertySetter sp = ActionMethod as IPropertySetter;
			if (sp != null)
			{
				MemberComponentIdCustom c = sp.SetProperty.Holder as MemberComponentIdCustom;
				if (c != null)
				{
					return c.Pointer;
				}
				return sp.SetProperty.Holder;
			}
			else
			{
				IObjectIdentity mp = ActionMethod.IdentityOwner;
				IMemberPointer p = ActionMethod as IMemberPointer;
				if (p != null)
				{
					return p.Holder;
				}
				MemberComponentIdCustom mcc = mp as MemberComponentIdCustom;
				if (mcc != null)
				{
					return mcc.Pointer;
				}
				MemberComponentId mmc = mp as MemberComponentId;
				if (mmc != null)
				{
					return mmc;
				}
				p = mp as IMemberPointer;
				if (p != null)
				{
					return p.Holder;
				}
				IObjectPointer op = mp as IObjectPointer;
				IClass co = mp as IClass;
				while (co == null && op != null)
				{
					op = op.Owner;
					co = op as IClass;
					p = op as IMemberPointer;
					if (p != null)
					{
						return p.Holder;
					}
				}
				if (co != null)
				{
					return co;
				}
				else
				{
					throw new DesignerException("Cannot find holder for Action [{0}]", ActionId);
				}
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
		[Browsable(false)]
		public IObjectPointer MethodOwner
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.Owner;
				}
				return null;
			}
		}
		public object GetParameterValue(string name)
		{
			if (_parameterValues == null)
			{
				_parameterValues = new ParameterValueCollection();
			}
			if (_methodPointer != null)
			{
				_methodPointer.ValidateParameterValues(_parameterValues);
			}
			return _parameterValues.GetParameterValue(name);
		}
		public void SetParameterValue(string name, object value)
		{
			if (_parameterValues == null)
			{
				_parameterValues = new ParameterValueCollection();
			}
			if (_methodPointer != null)
			{
				_methodPointer.ValidateParameterValues(_parameterValues);
			}
			_parameterValues.SetParameterValue(name, value);
		}

		public virtual EnumWebActionType WebActionType
		{
			get
			{
				return EnumWebActionType.Unknown;
			}
		}
		public virtual void CheckWebActionType()
		{
		}
		public void EstablishObjectOwnership(IActionsHolder scope)
		{
		}
		#endregion
		#region INonHostedObject Members

		EventHandler NameChanging;
		EventHandler PropertyChanged;
		/// <summary>
		/// update the action XML
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootNode"></param>
		/// <param name="writer"></param>
		public void OnPropertyChanged(string name, object property, XmlNode rootNode, XmlObjectWriter writer)
		{
			if (string.CompareOrdinal(name, "parameter") == 0)
				return;
			XmlNode actNode = SerializeUtil.GetActionNode(rootNode, this.ActionId);
			if (actNode != null)
			{
				if (string.CompareOrdinal(name, "ActionName") == 0)
				{
					XmlUtility.XmlUtil.SetNameAttribute(actNode, this.ActionName);
				}
				else if (string.CompareOrdinal(name, "Description") == 0)
				{
					XmlNode propNode = SerializeUtil.CreatePropertyNode(actNode, name);
					propNode.InnerText = this.Description;
				}
				else if (string.CompareOrdinal(name, "BreakBeforeExecute") == 0)
				{
					XmlNode propNode = SerializeUtil.CreatePropertyNode(actNode, name);
					propNode.InnerText = this.BreakBeforeExecute.ToString();
				}
				else if (string.CompareOrdinal(name, "BreakAfterExecute") == 0)
				{
					XmlNode propNode = SerializeUtil.CreatePropertyNode(actNode, name);
					propNode.InnerText = this.BreakAfterExecute.ToString();
				}
				else if (string.CompareOrdinal(name, "ActionMethod") == 0)
				{
					writer.ClearErrors();
					_class.SaveAction(this, writer);
				}
			}
		}
		[Browsable(false)]
		public string Name
		{
			get { return ActionName; }
		}
		[Browsable(false)]
		public uint MemberId
		{
			get { return this.ActionId; }
		}
		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			NameChanging = nameChange;
			PropertyChanged = propertyChange;
		}
		public EventHandler GetPropertyChangeHandler()
		{
			return PropertyChanged;
		}
		#endregion

		#region IBeforeSerializeNotify Members
		[Browsable(false)]
		public XmlNode XmlData { get { return _xmlNode; } }
		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reader = reader;
			_xmlNode = node;
			ScopeMethodId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ScopeId);
			SubScopeId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_SubScopeId);
		}

		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_writer = writer;
			if (_xmlNodeChanged != node)
			{
				_xmlNode = node;
			}
			if (ScopeMethodId != 0)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ScopeId, ScopeMethodId);
			}
			if (SubScopeId != 0)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_SubScopeId, SubScopeId);
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
				else if (Writer != null)
				{
					_writer.WriteObjectToNode(_xmlNode, this);
				}
			}
		}

		#endregion

		#region IUseClassId Members

		public void SetClassId(uint classId)
		{
			_classId = classId;
		}

		#endregion

		#region IActionContext Members

		public uint ActionContextId
		{
			get { return ActionId; }
		}

		public object GetParameterType(uint id)
		{
			if (_methodPointer == null)
				return null;
			return _methodPointer.GetParameterType(id);
		}

		public object GetParameterType(string name)
		{
			if (_methodPointer == null)
				return null;
			return _methodPointer.GetParameterType(name);
		}

		public object ProjectContext
		{
			get { return Project; }
		}

		public object OwnerContext
		{
			get { return _class; }
		}

		public IMethod ExecutionMethod
		{
			get
			{
				if (_methodPointer == null)
					return null;
				return _methodPointer.MethodPointed;
			}
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
	public class ActionSubMethodGlobal : ActionSubMethod
	{
		private EnumWebActionType _actType;
		public ActionSubMethodGlobal(ClassPointer owner)
			: base(owner)
		{
			_actType = EnumWebActionType.Unknown;
		}
		protected override void OnSetActionMethod()
		{
			SubMethodInfoPointerGlobal mg = ActionMethod as SubMethodInfoPointerGlobal;
			if (mg != null)
			{
				if (_actType == EnumWebActionType.Unknown)
				{
					_actType = mg.WebActionType;
				}
			}
		}
		public override void CheckWebActionType()
		{
			if (_actType == EnumWebActionType.Unknown)
			{
				if (ScopeMethod is WebClientEventHandlerMethodClientActions)
				{
					_actType = EnumWebActionType.Client;
				}
				else if (ScopeMethod is WebClientEventHandlerMethodServerActions)
				{
					_actType = EnumWebActionType.Server;
				}
			}
		}
		public override EnumWebActionType WebActionType
		{
			get
			{
				return _actType;
			}
		}
		[Browsable(false)]
		public EnumWebActionType ActionType
		{
			get
			{
				return _actType;
			}
			set
			{
				_actType = value;
				if (_actType == EnumWebActionType.Client)
				{
					if (this.ScopeMethod == null)
					{
						this.ScopeMethod = new WebClientEventHandlerMethodClientActions(Class);
					}
				}
				else if (_actType == EnumWebActionType.Server)
				{
					if (this.ScopeMethod == null)
					{
						this.ScopeMethod = new WebClientEventHandlerMethodServerActions(Class);
					}
				}
			}
		}
	}
}
