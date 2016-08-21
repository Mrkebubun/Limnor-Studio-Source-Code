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
using LimnorDesigner.Action;
using System.Xml;
using XmlSerializer;
using ProgElements;
using System.Windows.Forms;
using MathExp;
using System.CodeDom;
using System.Collections.Specialized;
using System.Drawing.Design;
using XmlUtility;
using VPL;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// method as a single action, 
	/// no need to create an action from it, 
	/// no parameters,
	/// may have condition
	/// </summary>
	public abstract class MethodAction : MethodClass, IAction, INonHostedObject
	{
		#region fields and constrcutors
		private IObjectPointer _returnReceiver;
		private XmlNode _xmlNodeChanged;
		private bool _breakAfter;
		private bool _breakBefore;
		private ExpressionValue _condition;
		private EnumWebActionType _actType;
		private MethodActionPointer _pointer;
		public MethodAction(ClassPointer owner)
			: base(owner)
		{
			this.ActionList = new BranchList(this);
			_actType = EnumWebActionType.Unknown;
			_pointer = new MethodActionPointer(this);
			ActionHolder = owner;
		}
		#endregion

		#region Properties
		public abstract bool RunAtServer { get; }
		[Browsable(false)]
		protected override string XmlTag
		{
			get
			{
				return XmlTags.XML_ACTION;
			}
		}
		#endregion

		#region IAction Members
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			if (ScopeRunAt == EnumWebRunAt.Server)
			{
				ParameterValueCollection pvs = ParameterValues;
				if (pvs != null && pvs.Count > 0)
				{
					List<ISourceValuePointer> lst = new List<ISourceValuePointer>();
					foreach (ParameterValue pv in pvs)
					{
						IList<ISourceValuePointer> l = pv.GetValueSources();
						if (l != null && l.Count > 0)
						{
							foreach (ISourceValuePointer v in l)
							{
								if (!v.IsWebServerValue() && v.IsWebClientValue())
								{
									lst.Add(v);
								}
							}
						}
					}
					return lst;
				}
			}
			return null;
		}
		[Browsable(false)]
		public void ResetDisplay()
		{

		}
		public abstract EnumWebRunAt ScopeRunAt
		{
			get;
			set;
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
		public bool IsLocal
		{
			get { return false; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
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
		public abstract bool IsStaticAction
		{
			get;
		}
		[Browsable(false)]
		public uint ActionId
		{
			get
			{
				return MemberId;
			}
			set
			{
				MemberId = value;
			}
		}
		[Browsable(false)]
		public ClassPointer Class
		{
			get { return this.Owner as ClassPointer; }
		}
		[Browsable(false)]
		public uint ExecuterClassId
		{
			get { return Class.ClassId; }
		}
		[Browsable(false)]
		public virtual uint ExecuterMemberId
		{
			get { return Class.MemberId; }
		}
		[Browsable(false)]
		public ClassPointer ExecuterRootHost
		{
			get { return Class; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string ActionName
		{
			get
			{
				return MethodName;
			}
			set
			{
				MethodName = value;
			}
		}
		[Browsable(false)]
		public string Display
		{
			get { return MethodName; }
		}
		[Browsable(false)]
		public Type ViewerType
		{
			get { return typeof(TopActionGroupDiagramViewer); }
		}
		[Browsable(false)]
		public virtual IObjectPointer MethodOwner
		{
			get { return Class; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return _pointer;
			}
			set
			{

			}
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
		[ReadOnly(true)]
		public XmlNode CurrentXmlData
		{
			get
			{
				if (_xmlNodeChanged != null)
				{
					return _xmlNodeChanged;
				}
				return XmlData;
			}
		}

		public bool HasChangedXmlData
		{
			get { return (_xmlNodeChanged != null); }
		}

		public bool IsSameMethod(IAction act)
		{
			MethodAction m = act as MethodAction;
			if (m != null)
			{
				return (m.MemberId == this.MemberId);
			}
			return false;
		}

		public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
		{
			return false;
		}

		public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
		{
			base.ExportCode(compiler, method, RootPointer.IsWebPage);
		}

		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			base.ExportJavaScriptCode(jsCode, methodToCompile, data);
		}

		public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			base.ExportPhpScriptCode(jsCode, methodToCompile, data);
		}

		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{

		}

		public uint ScopeMethodId
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
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
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionsHolder ActionHolder
		{
			get;
			set;
		}

		public void ResetScopeMethod()
		{

		}
		[Browsable(false)]
		public IAction CreateNewCopy()
		{
			MethodAction a = (MethodAction)Activator.CreateInstance(this.GetType(), Owner);
			a.MemberId = (UInt32)(Guid.NewGuid().GetHashCode());
			a.Description = this.Description;
			if (string.IsNullOrEmpty(MethodName))
			{
				a.MethodName = "Action" + Guid.NewGuid().GetHashCode().ToString("x");
			}
			else
			{
				if (MethodName.Length > 30)
				{
					a.MethodName = MethodName.Substring(0, 30) + Guid.NewGuid().GetHashCode().ToString("x");
				}
				else
				{
					a.MethodName = MethodName + "_" + Guid.NewGuid().GetHashCode().ToString("x");
				}
			}
			if (_returnReceiver != null)
			{
				a._returnReceiver = (IObjectPointer)_returnReceiver.Clone();
			}
			if (_condition != null)
			{
				a._condition = (ExpressionValue)_condition.Clone();
			}
			return a;
		}

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
		[Browsable(false)]
		public IObjectPointer ReturnReceiver
		{
			get
			{
				return _returnReceiver;
			}
			set
			{
				_returnReceiver = value;
			}
		}
		[Browsable(false)]
		public object GetParameterValue(string name)
		{
			return null;
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
		public void EstablishObjectOwnership(IActionsHolder scope)
		{

		}
		[Browsable(false)]
		public void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			string r;
			if (_returnReceiver != null)
			{
				r = _returnReceiver.GetJavaScriptReferenceCode(sb);
			}
			else
			{
				r = nextActionInput;
			}
			if (string.IsNullOrEmpty(r))
				base.CreateActionJavaScript(this.Name, sb, new StringCollection(), null);
			else
			{
				base.CreateActionJavaScript(this.Name, sb, new StringCollection(), r);
				if (_returnReceiver != null && !string.IsNullOrEmpty(nextActionInput))
				{
					sb.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}={1};\r\n", nextActionInput, r));
				}
			}
		}
		[Browsable(false)]
		public EnumWebActionType WebActionType
		{
			get { return _actType; }
		}
		[Browsable(false)]
		public void CheckWebActionType()
		{
			if (RunAtServer)
			{
				IList<ISourceValuePointer> l = GetClientProperties(0);
				if (l != null && l.Count > 0)
				{
					_actType = EnumWebActionType.Upload;
				}
				else
				{
					_actType = EnumWebActionType.Server;
				}
			}
			else
			{
				IList<ISourceValuePointer> l = GetServerProperties(0);
				if (l != null && l.Count > 0)
				{
					_actType = EnumWebActionType.Download;
				}
				else
				{
					_actType = EnumWebActionType.Client;
				}
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetClientProperties(UInt32 taskId)
		{
			return getSourceValuePointers(taskId, EnumWebValueSources.HasClientValues);
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetServerProperties(UInt32 taskId)
		{
			return getSourceValuePointers(taskId, EnumWebValueSources.HasServerValues);
		}
		private IList<ISourceValuePointer> getSourceValuePointers(UInt32 taskId, EnumWebValueSources scope)
		{
			List<ISourceValuePointer> list = new List<ISourceValuePointer>();
			if (_condition != null)
			{
				IList<ISourceValuePointer> l1 = _condition.GetValueSources();
				if (l1 != null && l1.Count > 0)
				{
					foreach (ISourceValuePointer p in l1)
					{
						if (taskId != 0)
						{
							p.SetTaskId(taskId);
						}

						if (scope == EnumWebValueSources.HasClientValues)
						{
							if (p.IsWebClientValue())
							{
								list.Add(p);
							}
						}
						else if (scope == EnumWebValueSources.HasServerValues)
						{
							if (!p.IsWebClientValue())
							{
								list.Add(p);
							}
						}
						else
						{
							list.Add(p);
						}
					}
				}
			}
			ParameterValueCollection pvs = ParameterValues;
			if (pvs != null && pvs.Count > 0)
			{
				foreach (ParameterValue pv in pvs)
				{
					IList<ISourceValuePointer> l = pv.GetValueSources();
					if (l != null && l.Count > 0)
					{
						foreach (ISourceValuePointer p in l)
						{
							if (scope == EnumWebValueSources.HasClientValues)
							{
								if (p.IsWebClientValue())
								{
									list.Add(p);
								}
							}
							else if (scope == EnumWebValueSources.HasServerValues)
							{
								if (!p.IsWebClientValue())
								{
									list.Add(p);
								}
							}
							else
							{
								list.Add(p);
							}
						}
					}
				}
			}
			OnGetSourceValuePointers(taskId, scope, list);
			return list;
		}
		protected abstract void OnGetSourceValuePointers(UInt32 taskId, EnumWebValueSources scope, List<ISourceValuePointer> list);
		#endregion
		#region INonHostedObject Members

		/// <summary>
		/// update the action XML
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootNode"></param>
		/// <param name="writer"></param>
		public override void OnPropertyChanged(string name, object property, XmlNode rootNode, XmlObjectWriter writer)
		{
			base.OnPropertyChanged(name, property, rootNode, writer);
			if (XmlData != null)
			{
				if (string.CompareOrdinal(name, "ActionName") == 0)
				{
					XmlUtility.XmlUtil.SetNameAttribute(XmlData, this.ActionName);
				}

				else if (string.CompareOrdinal(name, "BreakBeforeExecute") == 0)
				{
					XmlNode propNode = SerializeUtil.CreatePropertyNode(XmlData, name);
					propNode.InnerText = this.BreakBeforeExecute.ToString();
				}
				else if (string.CompareOrdinal(name, "BreakAfterExecute") == 0)
				{
					XmlNode propNode = SerializeUtil.CreatePropertyNode(XmlData, name);
					propNode.InnerText = this.BreakAfterExecute.ToString();
				}
				else if (string.CompareOrdinal(name, "ActionMethod") == 0)
				{
					writer.ClearErrors();
					((ClassPointer)(Owner)).SaveAction(this, writer);
				}
			}
		}
		#endregion
		#region IBreakPointOwner Members
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
					FirePropertyChanged("BreakBeforeExecute");
				}
			}
		}
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
					FirePropertyChanged("BreakAfterExecute");
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
		[Browsable(false)]
		public object ProjectContext
		{
			get
			{
				ClassPointer root = (ClassPointer)Owner;
				return root.Project;
			}
		}
		[Browsable(false)]
		public object OwnerContext
		{
			get { return IdentityOwner; }
		}
		[Browsable(false)]
		public IMethod ExecutionMethod
		{
			get { return this; }
		}
		[Browsable(false)]
		public void OnChangeWithinMethod(bool withinmethod)
		{
			if (this.XmlData != null)
			{
				XmlObjectWriter writer = GetWriter();
				if (writer != null)
				{
					if (withinmethod)
					{
						_xmlNodeChanged = XmlData.OwnerDocument.CreateElement(XmlData.Name);
						writer.WriteObjectToNode(_xmlNodeChanged, this);
					}
					else
					{
						writer.WriteObjectToNode(XmlData, this);
					}
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
				return this.XmlData;
			}
			set
			{
				this.SetXmlNode(value);
			}
		}

		#endregion

	}
	public class MethodActionPointer : IActionMethodPointer
	{
		#region fields and constructors
		private MethodAction _method;
		public MethodActionPointer(MethodAction method)
		{
			_method = method;
		}
		#endregion

		#region IActionMethodPointer Members
		public bool HasFormControlParent
		{
			get
			{
				return false;
			}
		}
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_method != null)
				{
					return _method.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{

		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}

		public object GetParameterType(uint id)
		{
			return null;
		}

		public object GetParameterTypeByIndex(int idx)
		{
			return null;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return null;
		}

		public ParameterValue CreateDefaultParameterValue(int i)
		{
			return null;
		}

		public void SetParameterExpressions(CodeExpression[] ps)
		{

		}

		public void SetParameterJS(string[] ps)
		{

		}

		public void SetParameterPhp(string[] ps)
		{

		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return _method.GetReferenceCode(method, statements, forValue);
		}

		public string GetJavaScriptReferenceCode(StringCollection method)
		{
			return _method.GetJavaScriptReferenceCode(method);
		}

		public string GetPhpScriptReferenceCode(StringCollection method)
		{
			return _method.GetPhpScriptReferenceCode(method);
		}

		public Type ActionBranchType
		{
			get { return _method.ActionBranchType; }
		}
		[ReadOnly(true)]
		public IAction Action
		{
			get
			{
				return _method;
			}
			set
			{

			}
		}

		public string DefaultActionName
		{
			get { return _method.DefaultActionName; }
		}

		public IMethod MethodPointed
		{
			get { return _method; }
		}

		public IObjectPointer Owner
		{
			get { return _method.Owner; }
		}

		public bool IsArrayMethod
		{
			get { return false; }
		}

		public string MethodName
		{
			get { return _method.MethodName; }
		}
		[Browsable(false)]
		public string CodeName { get { return MethodName; } }
		public Type ReturnBaseType
		{
			get { return typeof(void); }
		}

		public bool IsValid
		{
			get { return _method.IsValid; }
		}

		public IObjectPointer MethodDeclarer
		{
			get { return _method.Owner; }
		}

		#endregion

		#region IMethodPointer Members

		public bool NoReturn
		{
			get { return true; }
		}

		public int ParameterCount
		{
			get { return 0; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsForLocalAction
		{
			get { return false; }
		}

		public bool IsSameMethod(IMethodPointer pointer)
		{
			MethodAction m = pointer as MethodAction;
			if (m != null)
			{
				return _method.IsSameMethod(m);
			}
			MethodActionPointer mp = pointer as MethodActionPointer;
			if (mp != null)
			{
				return _method.IsSameMethod(mp.MethodPointed);
			}
			return false;
		}

		public bool IsSameMethod(IMethod method)
		{
			MethodAction m = method as MethodAction;
			if (m != null)
			{
				return _method.IsSameMethod(m);
			}
			MethodActionPointer mp = method as MethodActionPointer;
			if (mp != null)
			{
				return _method.IsSameMethod(mp.MethodPointed);
			}
			return false;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			MethodActionPointer mp = new MethodActionPointer(_method);
			return mp;
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MethodAction m = objectIdentity as MethodAction;
			if (m != null)
			{
				return _method.IsSameMethod(m);
			}
			MethodActionPointer mp = objectIdentity as MethodActionPointer;
			if (mp != null)
			{
				return _method.IsSameMethod(mp.MethodPointed);
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get { return _method.Owner; }
		}

		public bool IsStatic
		{
			get { return _method.IsStatic; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return _method.ObjectDevelopType; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Method; }
		}

		#endregion
	}
}
