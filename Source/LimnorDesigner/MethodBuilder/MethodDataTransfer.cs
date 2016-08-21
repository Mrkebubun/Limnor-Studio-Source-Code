/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;
using System.Collections;
using LimnorDesigner.Property;
using System.ComponentModel;
using System.Globalization;
using LimnorDesigner.Action;
using VSPrj;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using LimnorDesigner.ResourcesManager;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;
using VPL;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner.MethodBuilder
{
	[UseParentObject]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MethodDataTransfer : IMethod, IMethodPointer, IActionMethodPointer, IActionCompiler, ICustomTypeDescriptor, IXmlNodeSerializable
	{
		#region fields and constructors
		private ClassPointer _rootClass;
		private LimnorProject _prj;
		private Dictionary<IProperty, ParameterValue> _dataTransfers;
		private UInt32 _id;
		private UInt32 _classId;
		private UInt32 _actId;
		private ActionClass _action;
		public MethodDataTransfer()
		{
		}
		public MethodDataTransfer(ActionClass act)
		{
			_action = act;
			_actId = _action.ActionId;
			_prj = _action.Project;
			_classId = _action.ClassId;
			if (_prj != null && _classId != 0)
			{
				_rootClass = _prj.GetTypedData<ClassPointer>(_classId);
			}
		}
		public MethodDataTransfer(ClassPointer owner)
		{
			_rootClass = owner;
			if (_rootClass != null)
			{
				_prj = _rootClass.Project;
				_classId = _rootClass.ClassId;
			}
		}
		#endregion
		#region static methods
		public static void Compile(IProperty SetProperty, CodeExpression rt, ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			CodeAssignStatement cas = new CodeAssignStatement();
			cas.Left = SetProperty.GetReferenceCode(methodToCompile, statements, false);
			if (cas.Left == null)
			{
				compiler.AddError("Error: SetterPointer missing property");
			}
			else
			{
				CodeMethodReferenceExpression cmr = rt as CodeMethodReferenceExpression;
				if (cmr != null)
				{
					rt = new CodeMethodInvokeExpression(cmr);
				}
				if (nextAction != null && nextAction.UseInput)
				{
					CodeVariableDeclarationStatement output = new CodeVariableDeclarationStatement(
						currentAction.OutputType.TypeString, currentAction.OutputCodeName, rt);
					statements.Add(output);
					rt = new CodeVariableReferenceExpression(currentAction.OutputCodeName);
				}
				cas.Right = rt;
				bool bThreadSafe = !methodToCompile.MakeUIThreadSafe;
				if (bThreadSafe && currentAction != null)
				{
					if (!currentAction.IsMainThread && SetProperty.ObjectDevelopType == EnumObjectDevelopType.Library)
					{
						bThreadSafe = !CompilerUtil.IsOwnedByControl(SetProperty.Owner);
					}
				}
				if (!bThreadSafe)
				{
					statements.Add(new CodeSnippetStatement(CompilerUtil.InvokeStart));
				}
				statements.Add(cas);
				if (!bThreadSafe)
				{
					statements.Add(new CodeSnippetStatement(CompilerUtil.InvokeEnd));
				}
			}
		}
		public static void CreateJavaScript(IProperty SetProperty, string v, StringCollection sb, StringCollection parameters)
		{
			if (SetProperty.RootPointer != null && typeof(WebPage).IsAssignableFrom(SetProperty.RootPointer.BaseClassType))
			{
				ClassPointer pp = SetProperty.PropertyOwner as ClassPointer;
				if (pp != null)
				{
					if (pp.ClassId != SetProperty.RootPointer.ClassId)
					{
						sb.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setPropertyByPageId({0},'{1}',{2});\r\n", pp.ClassId, SetProperty.Name, v));
						return;
					}
				}
			}
			if (typeof(ProjectResources).Equals(SetProperty.Owner.ObjectInstance))
			{
				if (string.CompareOrdinal(SetProperty.Name, "ProjectCultureName") == 0)
				{
					sb.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SwitchCulture({0});\r\n", v));
					return;
				}
			}
			IWebClientComponent wcc = SetProperty.Owner.ObjectInstance as IWebClientComponent;
			if (wcc == null && SetProperty.Owner.ObjectInstance == null)
			{
				if (SetProperty.Owner.ObjectType != null)
				{
					if (SetProperty.Owner.ObjectType.GetInterface("IWebClientComponent") != null)
					{
						try
						{
							wcc = Activator.CreateInstance(SetProperty.Owner.ObjectType) as IWebClientComponent;
							if (wcc != null)
							{
								wcc.SetCodeName(SetProperty.Owner.CodeName);
							}
						}
						catch
						{
						}
					}
				}
			}
			if (wcc != null)
			{
				v = wcc.MapJavaScriptVallue(SetProperty.Name, v);
			}
			IWebClientPropertyCustomSetter wpcs = SetProperty.Owner.ObjectInstance as IWebClientPropertyCustomSetter;
			if (wpcs == null && SetProperty.Owner.ObjectInstance == null)
			{
				wpcs = wcc as IWebClientPropertyCustomSetter;
			}
			if (wpcs != null)
			{
				string ownerCode = SetProperty.Owner.GetJavaScriptReferenceCode(sb);
				if (wpcs.CreateSetPropertyJavaScript(ownerCode, SetProperty.Name, v, sb))
				{
					return;
				}
			}
			if (string.CompareOrdinal("Font", SetProperty.Name) == 0)
			{
				IWebClientControl webc = SetProperty.Owner.ObjectInstance as IWebClientControl;
				if (webc != null)
				{
					sb.Add(string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.setFont({0},{1});\r\n", WebPageCompilerUtility.JsCodeRef(webc.CodeName), WebPageCompilerUtility.GetFontJavascriptValues(v)));
					return;
				}
			}

			if (typeof(SessionVariableCollection).Equals(SetProperty.Owner.ObjectType))
			{
				sb.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.setSessionVariable('{0}',{1});\r\n", SetProperty.Name, v));
				return;
			}
			if (typeof(LimnorWebApp).IsAssignableFrom(SetProperty.Owner.ObjectType))
			{
				if (string.CompareOrdinal(SetProperty.Name, "GlobalVariableTimeout") == 0)
				{
					sb.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.setSessionTimeout({0});\r\n", v));
					return;
				}
			}
			string code = null;
			IWebClientPropertyHolder wcph = SetProperty.Owner.ObjectInstance as IWebClientPropertyHolder;
			if (wcph != null)
			{
				code = wcph.CreateSetPropertyJavaScript(SetProperty.Owner.CodeName, SetProperty.Name, v);
				if (!string.IsNullOrEmpty(code))
				{
					sb.Add(code);
				}
			}
			IWebClientPropertySetter wcps = SetProperty.Owner.ObjectInstance as IWebClientPropertySetter;
			if (string.IsNullOrEmpty(code))
			{
				if (wcps == null || !wcps.UseCustomSetter(SetProperty.Name))
				{
					string left = SetProperty.GetJavaScriptReferenceCode(sb);
					if (left.EndsWith(".innerText", StringComparison.Ordinal))
					{
						code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetInnerText({0},{1});\r\n", left.Substring(0, left.Length - 10), v);
					}
					else if (left.StartsWith("JsonDataBinding.GetInnerText(", StringComparison.Ordinal))
					{
						string oc = SetProperty.Owner.GetJavaScriptReferenceCode(sb);
						code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetInnerText({0},{1});\r\n", oc, v);
					}
					else if (left.EndsWith(".Opacity", StringComparison.Ordinal))
					{
						code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setOpacity({0},{1});\r\n", left.Substring(0, left.Length - ".Opacity".Length), v);
					}
					else if (left.StartsWith("JsonDataBinding.getOpacity(", StringComparison.Ordinal))
					{
						string oc = SetProperty.Owner.GetJavaScriptReferenceCode(sb);
						code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setOpacity({0},{1});\r\n", oc, v);
					}
					else if (left.EndsWith(".style.display", StringComparison.Ordinal))
					{
						code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setVisible({0},{1});\r\n", left.Substring(0, left.Length - ".style.display".Length), v);
					}
					else
					{
						if (wcc != null && v != null)
						{
							if (string.CompareOrdinal(SetProperty.Name, "Top") == 0
								|| string.CompareOrdinal(SetProperty.Name, "Left") == 0
								|| string.CompareOrdinal(SetProperty.Name, "Height") == 0
								|| string.CompareOrdinal(SetProperty.Name, "Width") == 0)
							{
								if (!v.EndsWith("px", StringComparison.Ordinal))
								{
									code = string.Format(CultureInfo.InvariantCulture, "{0}=({1})+'px';\r\n", left, v);
								}
							}
						}
						if (code == null && SetProperty.Holder != null)
						{
							if (typeof(HtmlListBox).Equals(SetProperty.Holder.ObjectType) || typeof(HtmlDropDown).Equals(SetProperty.Holder.ObjectType))
							{
								if (string.CompareOrdinal(SetProperty.Name, "SelectedValue") == 0
								|| string.CompareOrdinal(SetProperty.Name, "selectedValue") == 0)
								{
									code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetSelectedListValue({0},{1});\r\n", SetProperty.Holder.GetJavaScriptReferenceCode(sb), v);
								}
								else if (string.CompareOrdinal(SetProperty.Name, "SelectedItem") == 0
								|| string.CompareOrdinal(SetProperty.Name, "selectedItem") == 0)
								{
									code = string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetSelectedListText({0},{1});\r\n", SetProperty.Holder.GetJavaScriptReferenceCode(sb), v);
								}
							}
						}
						if (code == null)
						{
							code = string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", left, v);
						}
					}
					if (!string.IsNullOrEmpty(code))
					{
						sb.Add(code);
					}
				}
			}
			bool isCustomValue = false;
			if (string.CompareOrdinal("tag", SetProperty.Name) == 0)
			{
				isCustomValue = true;
			}
			else if (wcc != null)
			{
				WebClientValueCollection cvc = wcc.CustomValues;
				if (cvc != null)
				{
					if (cvc.ContainsKey(SetProperty.Name))
					{
						isCustomValue = true;
					}
				}
			}
			if (isCustomValue)
			{
				string ownerCode = SetProperty.Owner.GetJavaScriptReferenceCode(sb);
				sb.Add(string.Format(CultureInfo.InvariantCulture, "{0}JsonDataBinding.onSetCustomValue({1},'{2}');\r\n", Indentation.GetIndent(), ownerCode, SetProperty.Name));
			}
			if (wcps != null)
			{
				wcps.OnSetProperty(SetProperty.Name, v, sb);
			}
		}
		public static void CreatePhpScript(IProperty SetProperty, string v, StringCollection sb, StringCollection parameters)
		{
			if (DesignUtil.IsSessionVariable(SetProperty))
			{
				string code = string.Format(CultureInfo.InvariantCulture, "$this->WebAppPhp->SetSessionVariable('{0}',{1});\r\n", SetProperty.Name, v);
				sb.Add(code);
			}
			else
			{
				string left = SetProperty.GetPhpScriptReferenceCode(sb);
				string code = string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", left, v);
				sb.Add(code);
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public UInt32 ID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
			set
			{
				_classId = value;
				if (_classId != 0)
				{
					if (_rootClass == null && _prj != null)
					{
						_rootClass = _prj.GetTypedData<ClassPointer>(_classId);
					}
				}
			}
		}
		[Browsable(false)]
		public UInt32 ActionId
		{
			get
			{
				return _actId;
			}
			set
			{
				if (value != 0)
				{
					_actId = value;
					if (_action == null && _rootClass != null)
					{
						Dictionary<UInt32, IAction> acts = _rootClass.GetActions();
						if (acts != null)
						{
							IAction a;
							acts.TryGetValue(_actId, out a);
							_action = (ActionClass)a;
						}
					}
				}
			}
		}
		public Dictionary<IProperty, ParameterValue> DataTransfers
		{
			get
			{
				if (_dataTransfers == null)
				{
					_dataTransfers = new Dictionary<IProperty, ParameterValue>();
				}
				return _dataTransfers;
			}
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			return this.MethodName;
		}
		public void Reset()
		{
			_dataTransfers = new Dictionary<IProperty, ParameterValue>();
		}
		public ParameterValue CreateDefaultParameter(IProperty p)
		{
			ParameterValue pv = new ParameterValue(this.Action);
			pv.Name = p.ExpressionDisplay;
			pv.SetDataType(p.PropertyType);
			return pv;
		}
		public void ResetProperty(IProperty p)
		{
			if (_dataTransfers != null)
			{
				foreach (IProperty p0 in _dataTransfers.Keys)
				{
					if (p0.IsSameObjectRef(p))
					{
						_dataTransfers[p0] = CreateDefaultParameter(p);
						return;
					}
				}
			}
		}
		public EnumWebActionType CheckWebActionType()
		{
			if (_dataTransfers != null)
			{
				EnumWebRunAt methodRunAt = EnumWebRunAt.Unknown;
				EnumWebValueSources sources = EnumWebValueSources.Unknown;
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					if (kv.Value != null && kv.Key != null)
					{
						if (methodRunAt == EnumWebRunAt.Unknown)
						{
							if (kv.Key.RunAt == EnumWebRunAt.Client)
							{
								methodRunAt = EnumWebRunAt.Client;
							}
							else if (kv.Key.RunAt == EnumWebRunAt.Server)
							{
								methodRunAt = EnumWebRunAt.Server;
							}
						}
						IList<ISourceValuePointer> list = kv.Value.GetValueSources();
						if (list != null && list.Count > 0)
						{
							sources = WebBuilderUtil.GetActionTypeFromSources(list, sources);
						}
					}
				}
				if (methodRunAt == EnumWebRunAt.Unknown)
				{
					if (DesignUtil.IsWebClientObject(this.Owner))
					{
						methodRunAt = EnumWebRunAt.Client;
					}
					else
					{
						methodRunAt = EnumWebRunAt.Server;
					}
				}
				return WebBuilderUtil.GetWebActionType(methodRunAt, sources);
			}
			return EnumWebActionType.Unknown;
		}
		public ParameterValueCollection GetParameters()
		{
			ParameterValueCollection ps = new ParameterValueCollection();
			if (_dataTransfers != null)
			{
				Dictionary<IProperty, ParameterValue>.ValueCollection.Enumerator en = _dataTransfers.Values.GetEnumerator();
				while (en.MoveNext())
				{
					ps.Add(en.Current);
				}
			}
			return ps;
		}
		public void AddProperty(IProperty p)
		{
			if (_dataTransfers == null)
			{
				_dataTransfers = new Dictionary<IProperty, ParameterValue>();
			}
			foreach (IProperty p0 in _dataTransfers.Keys)
			{
				if (p0.IsSameObjectRef(p))
				{
					return;
				}
			}
			if (_rootClass.IsWebPage)
			{
				EnumWebActionType at = EnumWebActionType.Unknown;
				Dictionary<IProperty, ParameterValue>.KeyCollection.Enumerator en = _dataTransfers.Keys.GetEnumerator();
				while (en.MoveNext())
				{
					bool isClient = DesignUtil.IsWebClientObject(en.Current);
					bool isServer = DesignUtil.IsWebServerObject(en.Current);
					if (isClient && !isServer)
					{
						at = EnumWebActionType.Client;
						break;
					}
					else if (!isClient && isServer)
					{
						at = EnumWebActionType.Server;
						break;
					}
				}
				if (at == EnumWebActionType.Server)
				{
					if (DesignUtil.IsWebClientObject(p))
					{
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
							"Cannot add a client property [{0}] because there are server properties in the list", p.ExpressionDisplay), "Data transfer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}
				else if (at == EnumWebActionType.Client)
				{
					if (DesignUtil.IsWebServerObject(p))
					{
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
							"Cannot add a server property [{0}] because there are client properties in the list", p.ExpressionDisplay), "Data transfer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}
			}
			_dataTransfers.Add(p, CreateDefaultParameter(p));
		}
		public void DeleteProperty(PropertyDescriptor p)
		{
			PropertyDescriptorDataTransfer pd = p as PropertyDescriptorDataTransfer;
			if (pd != null)
			{
				DeleteProperty(pd.Property);
			}
		}
		public void DeleteProperty(IProperty p)
		{
			if (_dataTransfers != null)
			{
				IProperty key = null;
				foreach (IProperty p0 in _dataTransfers.Keys)
				{
					if (p0.IsSameObjectRef(p))
					{
						key = p0;
						break;

					}
				}
				if (key != null)
				{
					_dataTransfers.Remove(key);
					OnChanged();
				}
			}
		}
		public void OnChanged()
		{
			if (_rootClass == null && _prj != null)
			{
				_rootClass = _prj.GetTypedData<ClassPointer>(_action.ClassId);
				_classId = _action.ClassId;
			}
			_action.OnPropertyChanged("ActionMethod", null, _rootClass.XmlData, null);
			ILimnorDesignPane pane = _prj.GetTypedData<ILimnorDesignPane>(_classId);
			if (pane != null)
			{
				pane.Loader.NotifyChanges();
			}
		}
		#endregion
		#region IMethod Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IParameter MethodReturnType
		{
			get
			{
				return new ParameterClass(new TypePointer(typeof(void)), this);
			}
			set
			{

			}
		}

		public bool NoReturn
		{
			get { return true; }
		}

		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "dt{0}", ID.ToString("x", CultureInfo.InvariantCulture)); }
		}

		public string MethodSignature
		{
			get { return string.Format(CultureInfo.InvariantCulture, "{0}()", MethodName); }
		}

		public bool IsForLocalAction
		{
			get { return false; }
		}

		public bool HasReturn
		{
			get { return false; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}

		public bool IsSameMethod(IMethod method)
		{
			MethodDataTransfer m = method as MethodDataTransfer;
			if (m != null)
			{
				return m.ID == this.ID;
			}
			return false;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return new Dictionary<string, string>();
		}

		public string ParameterName(int i)
		{
			return string.Empty;
		}

		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			return this;
		}
		[Browsable(false)]
		public object GetParameterType(uint id)
		{
			return null;
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}
		[Browsable(false)]
		public Type ActionType
		{
			get { return typeof(ActionClass); }
		}
		[Browsable(false)]
		public object ModuleProject
		{
			get
			{
				return _prj;
			}
			set
			{
				if (value != null)
				{
					_prj = (LimnorProject)value;
					if (_rootClass == null && _classId != 0)
					{
						_rootClass = _prj.GetTypedData<ClassPointer>(_classId);
					}
				}
			}
		}

		#endregion

		#region IMethod0 Members
		[Browsable(false)]
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return "DataTransfer";
			}
			set
			{

			}
		}
		[Browsable(false)]
		public string CodeName { get { return MethodName; } }
		[Browsable(false)]
		public int ParameterCount
		{
			get { return 0; }
		}
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get { return null; }
		}

		#endregion

		#region IObjectIdentity Members
		[Browsable(false)]
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MethodDataTransfer m = objectIdentity as MethodDataTransfer;
			if (m != null)
			{
				return m.ID == this.ID;
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return _rootClass; }
		}
		public bool IsStatic
		{
			get
			{
				if (_dataTransfers != null)
				{
					foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
					{
						if (!kv.Key.IsStatic)
						{
							return false;
						}
						if (kv.Value != null)
						{
							if (!kv.Value.IsStatic)
							{
								return false;
							}
						}
					}
				}
				return true;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Method; }
		}

		#endregion

		#region ICloneable Members
		[Browsable(false)]
		public object Clone()
		{
			MethodDataTransfer obj = new MethodDataTransfer(_rootClass);
			if (_dataTransfers != null)
			{
				Dictionary<IProperty, ParameterValue> ps = new Dictionary<IProperty, ParameterValue>();
				obj._dataTransfers = ps;
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					ps.Add(kv.Key, kv.Value);
				}
			}
			obj.ModuleProject = this.ModuleProject;
			obj.ClassId = this.ClassId;
			obj.ID = this.ID;
			obj.ActionId = this.ActionId;
			obj.Action = this.Action;
			return obj;
		}

		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			MethodDataTransfer m = pointer as MethodDataTransfer;
			if (m != null)
			{
				return m.ID == this.ID;
			}
			return false;
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
				if (_dataTransfers != null)
				{
					EnumWebRunAt methodRunAt = EnumWebRunAt.Unknown;
					EnumWebValueSources sources = EnumWebValueSources.Unknown;
					foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
					{
						if (kv.Value != null && kv.Key != null)
						{
							if (methodRunAt == EnumWebRunAt.Unknown)
							{
								if (kv.Key.RunAt == EnumWebRunAt.Client)
								{
									methodRunAt = EnumWebRunAt.Client;
								}
								else if (kv.Key.RunAt == EnumWebRunAt.Server)
								{
									methodRunAt = EnumWebRunAt.Server;
								}
							}
							IList<ISourceValuePointer> list = kv.Value.GetValueSources();
							if (list != null && list.Count > 0)
							{
								sources = WebBuilderUtil.GetActionTypeFromSources(list, sources);
							}
						}
					}
					return methodRunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{

		}

		public object GetParameterTypeByIndex(int idx)
		{
			return null;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		public ParameterValue CreateDefaultParameterValue(int i)
		{
			return null;
		}

		public void SetParameterExpressions(System.CodeDom.CodeExpression[] ps)
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
			return null;
		}

		public string GetJavaScriptReferenceCode(StringCollection method)
		{
			return string.Empty;
		}

		public string GetPhpScriptReferenceCode(StringCollection method)
		{
			return string.Empty;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IAction Action
		{
			get
			{
				if (_action == null && _actId != 0 && _rootClass != null)
				{
					Dictionary<UInt32, IAction> acts = _rootClass.GetActions();
					if (acts != null)
					{
						IAction a;
						acts.TryGetValue(_actId, out a);
						_action = (ActionClass)a;
					}
				}
				return _action;
			}
			set
			{
				if (value != null)
				{
					_action = (ActionClass)value;
					_actId = _action.ActionId;
				}
			}
		}

		public string DefaultActionName
		{
			get { return MethodName; }
		}

		public IMethod MethodPointed
		{
			get { return this; }
		}

		public IObjectPointer Owner
		{
			get { return _rootClass; }
		}

		public bool IsArrayMethod
		{
			get { return false; }
		}

		public Type ReturnBaseType
		{
			get { return typeof(void); }
		}

		public bool IsValid
		{
			get { return true; }
		}

		public IObjectPointer MethodDeclarer
		{
			get { return _rootClass; }
		}

		#endregion

		#region IActionCompiler Members

		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			if (_dataTransfers != null)
			{
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					if (kv.Value != null && kv.Key != null)
					{
						MethodDataTransfer.Compile(kv.Key, kv.Value.GetReferenceCode(methodToCompile, statements, true), currentAction, nextAction, compiler, methodToCompile, method, statements, parameters, returnReceiver, debug);
					}
				}
			}
		}

		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_dataTransfers != null)
			{
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					if (kv.Value != null && kv.Key != null)
					{
						MethodDataTransfer.CreateJavaScript(kv.Key, kv.Value.GetJavaScriptReferenceCode(sb), sb, parameters);
					}
				}
			}
		}

		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_dataTransfers != null)
			{
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					if (kv.Value != null && kv.Key != null)
					{
						MethodDataTransfer.CreatePhpScript(kv.Key, kv.Value.GetPhpScriptReferenceCode(sb), sb, parameters);
					}
				}
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			if (_dataTransfers != null)
			{
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					PropertyDescriptorDataTransfer p = new PropertyDescriptorDataTransfer(this, kv.Key);
					lst.Add(p);
				}
			}
			PropertyDescriptorNewDataTransfer pn = new PropertyDescriptorNewDataTransfer(this);
			lst.Add(pn);
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region PropertyDescriptorNewDataTransfer
		class PropertyDescriptorNewDataTransfer : PropertyDescriptor
		{
			private MethodDataTransfer _owner;
			public PropertyDescriptorNewDataTransfer(MethodDataTransfer dt)
				: base("NewDataTransfer", new Attribute[] { new EditorAttribute(typeof(TypeSelectorNewDataTrabsfer), typeof(UITypeEditor)), 
                    new ParenthesizePropertyNameAttribute(true),
                    new RefreshPropertiesAttribute(RefreshProperties.All)})
			{
				_owner = dt;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(MethodDataTransfer); }
			}

			public override object GetValue(object component)
			{
				return "Select a property";
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				IProperty p = value as IProperty;
				if (p != null)
				{
					_owner.AddProperty(p);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
		#region PropertyDescriptorDataTransfer
		class PropertyDescriptorDataTransfer : PropertyDescriptor
		{
			private MethodDataTransfer _owner;
			private IProperty _property;
			public PropertyDescriptorDataTransfer(MethodDataTransfer dt, IProperty property)
				: base(property.ExpressionDisplay, new Attribute[] { new EditorAttribute(typeof(SelectorEnumValueType), typeof(UITypeEditor)) })
			{
				_owner = dt;
				_property = property;
			}
			public IProperty Property
			{
				get
				{
					return _property;
				}
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(MethodDataTransfer); }
			}

			public override object GetValue(object component)
			{
				ParameterValue pv;
				if (_owner.DataTransfers.TryGetValue(_property, out pv))
				{
					return pv;
				}
				return null;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(ParameterValue); }
			}

			public override void ResetValue(object component)
			{
				_owner.ResetProperty(_property);
			}

			public override void SetValue(object component, object value)
			{
				ParameterValue pv;
				if (_owner.DataTransfers.TryGetValue(_property, out pv))
				{
					pv.SetValue(value);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_action.ActionMethod = this;
			_dataTransfers = new Dictionary<IProperty, ParameterValue>();
			XmlNodeList nds = node.SelectNodes(XmlTags.XML_Item);
			if (nds != null)
			{
				foreach (XmlNode item in nds)
				{
					XmlNode pNode = item.SelectSingleNode(XmlTags.XML_PROPERTY);
					if (pNode != null)
					{
						IProperty p = (IProperty)serializer.ReadObject(pNode, this);
						AddProperty(p);
						ParameterValue pv = _dataTransfers[p];
						XmlNode vNode = item.SelectSingleNode(XmlTags.XML_Data);
						if (vNode != null)
						{
							serializer.ReadObjectFromXmlNode(vNode, pv, typeof(ParameterValue), this);
						}
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_dataTransfers != null)
			{
				foreach (KeyValuePair<IProperty, ParameterValue> kv in _dataTransfers)
				{
					XmlNode item = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					node.AppendChild(item);
					XmlNode pNode = node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
					item.AppendChild(pNode);
					serializer.WriteObjectToNode(pNode, kv.Key);
					XmlNode vNode = node.OwnerDocument.CreateElement(XmlTags.XML_Data);
					item.AppendChild(vNode);
					serializer.WriteObjectToNode(vNode, kv.Value);
				}
			}
		}

		#endregion
	}
	class TypeSelectorNewDataTrabsfer : UITypeEditor
	{
		public TypeSelectorNewDataTrabsfer()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					MethodDataTransfer mdt = context.Instance as MethodDataTransfer;
					if (mdt == null)
					{
						IAction act = context.Instance as IAction;
						if (act != null)
						{
							mdt = act.ActionMethod as MethodDataTransfer;
						}
					}
					if (mdt != null)
					{
						FrmObjectExplorer dlg = DesignUtil.GetPropertySelector(null, mdt.Action.ScopeMethod, null);
						dlg.SetMultipleSelection(true);
						dlg.ObjectSubType = EnumObjectSelectSubType.Property;
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							if (dlg.SelectedProperties != null)
							{
								foreach (IProperty p in dlg.SelectedProperties)
								{
									mdt.AddProperty(p);
								}
								PropertyGrid pg = null;
								Type t = edSvc.GetType();
								PropertyInfo pif0 = t.GetProperty("OwnerGrid");
								if (pif0 != null)
								{
									object g = pif0.GetValue(edSvc, null);
									pg = g as PropertyGrid;
								}
								if (pg != null && pg.SelectedGridItem != null)
								{
									pg.SelectedGridItem.Expanded = true;
								}
								mdt.OnChanged();
								return true;
							}
						}
					}
				}
			}
			return string.Empty;
		}
	}
	class PropertyDescriptorDataTransferMethod : PropertyDescriptor
	{
		private MethodDataTransfer _owner;
		public PropertyDescriptorDataTransferMethod(MethodDataTransfer dataTransfer)
			: base("DataTransferList", new Attribute[] { new TypeConverterAttribute(typeof(ExpandableObjectConverter)), new EditorAttribute(typeof(TypeSelectorNewDataTrabsfer), typeof(UITypeEditor)) })
		{
			_owner = dataTransfer;
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get { return typeof(ActionClass); }
		}

		public override object GetValue(object component)
		{
			return _owner;
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return _owner.GetType(); }
		}

		public override void ResetValue(object component)
		{
			_owner.Reset();
		}

		public override void SetValue(object component, object value)
		{

		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
