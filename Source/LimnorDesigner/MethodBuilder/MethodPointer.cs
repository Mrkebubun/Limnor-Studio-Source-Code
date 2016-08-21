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
using System.Reflection;
using System.ComponentModel;
using MathExp;
using LimnorDesigner.Action;
using System.CodeDom;
using Parser;
using VPL;
using XmlSerializer;
using System.Web.Services;
using XmlUtility;
using VSPrj;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;
using LimnorDesigner.Property;
using System.Xml;
using System.Windows.Forms;
using LimnorDesigner.Web;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// method
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class MethodPointer : MemberPointer, IMethod, IMethodCompile, IActionCompiler, IActionMethodPointer, IGenericTypePointer
	{
		#region fields and constructors
		private Type[] _paramTypes = Type.EmptyTypes;
		private Type[] _genericParams = Type.EmptyTypes;//for reloading the MethodInfo
		private Type _returnType;
		private DataTypePointer _concreteReturnType;
		private UInt32 _id;
		private CodeTypeDeclaration _td;
		private CodeMemberMethod _methodCode;
		private CodeExpression[] _parameterExpressions; //for using in math exp compile
		private string[] _parameterJS;
		private string[] _parameterPhp;
		private IAction _action;
		public MethodPointer()
		{
		}
		#endregion
		#region IObjectPointer Members
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (MethodDef != null)
				{
					object[] a = MethodDef.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
					if (a != null && a.Length > 0)
					{
						return EnumWebRunAt.Client;
					}
					a = MethodDef.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
					if (a != null && a.Length > 0)
					{
						return EnumWebRunAt.Server;
					}
				}
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		protected CodeExpression[] ParameterCodeExpressions
		{
			get
			{
				return _parameterExpressions;
			}
		}
		[Browsable(false)]
		protected string[] ParameterJS
		{
			get
			{
				return _parameterJS;
			}
		}
		[Browsable(false)]
		protected string[] ParameterPhp
		{
			get
			{
				return _parameterPhp;
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (MethodDef != null)
				{
					HtmlElement_ItemBase hei = this.Owner as HtmlElement_ItemBase;
					if (hei != null)
					{
						return hei.IsValid;
					}
					ActionBranchParameterPointer abpp = this.Owner as ActionBranchParameterPointer;
					if (abpp != null)
					{
						if (abpp.Owner != null)
						{
							return true;
						}
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "abpp.Owner is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
						return false;
					}
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MethodDef is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				if (MethodDef != null)
					return MethodDef.IsStatic;
				return false;
			}
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				return ObjectType.AssemblyQualifiedName;
			}
		}

		public override bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Method || target == EnumObjectSelectType.All);
		}
		[Browsable(false)]
		public override string ObjectKey
		{
			get
			{
				if (string.IsNullOrEmpty(ManualObjectKey))
				{
					int c;
					return Owner.ObjectKey + "." + GetMethodSignature(out c);
				}
				else
				{
					return ManualObjectKey;
				}
			}
		}
		[Browsable(false)]
		public virtual string MethodSignature
		{
			get
			{
				int c;
				return GetMethodSignature(out c);
			}
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Method; } }
		#endregion
		#region methods
		/// <summary>
		/// some objects may change method parameter types, i.e. query parameters changed.
		/// this function should adjust parameters accordingly.
		/// </summary>
		/// </summary>
		/// <param name="ps">correct parameters. it cannot be null. it can be empty array</param>
		/// <returns>true: parameter changed; false: parameter not changed</returns>
		public bool AdjustParameterTypes(ParameterInfo[] ps)
		{
			bool bChanged = false;
			if (_paramTypes == null || _paramTypes.Length == 0)
			{
				if (ps.Length != 0)
				{
					bChanged = true;
				}
			}
			else
			{
				if (ps.Length != _paramTypes.Length)
				{
					bChanged = true;
				}
				else
				{
					for (int i = 0; i < ps.Length; i++)
					{
						if (!ps[i].ParameterType.Equals(_paramTypes[i]))
						{
							bChanged = true;
							break;
						}
					}
				}
			}
			if (bChanged)
			{
				_paramTypes = new Type[ps.Length];
				for (int i = 0; i < ps.Length; i++)
				{
					_paramTypes[i] = ps[i].ParameterType;
				}
			}
			return bChanged;
		}
		public void ResolveGenericParameters(LimnorProject project)
		{
			MethodBase mb = MethodDef;
			if (mb != null)
			{
				if (_concreteReturnType == null && _returnType != null && (_returnType.IsGenericParameter || _returnType.IsGenericType))
				{
					IGenericTypePointer igp = this.Owner as IGenericTypePointer;
					if (igp != null)
					{
						_concreteReturnType = igp.GetConcreteType(_returnType);
					}
				}
				if (TypeParameters == null && mb.ContainsGenericParameters && !(mb is ConstructorInfo))
				{
					_genericParams = mb.GetGenericArguments();
					if (_genericParams != null && _genericParams.Length > 0)
					{
						DlgSelectTypeParameters dlg = new DlgSelectTypeParameters();
						dlg.LoadData(mb, project);
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							TypeParameters = dlg._holder.Results;
						}

						if (TypeParameters == null)
						{
							TypeParameters = new DataTypePointer[_genericParams.Length];
							for (int i = 0; i < _genericParams.Length; i++)
							{
								TypeParameters[i] = new DataTypePointer(typeof(object));
							}
						}
						if (_concreteReturnType == null && _returnType != null && (_returnType.IsGenericParameter || _returnType.IsGenericType))
						{
							if (_returnType.IsGenericType)
							{
								_concreteReturnType = new DataTypePointer(_returnType);
								Type[] tcs = _returnType.GetGenericArguments();
								if (tcs != null && tcs.Length > 0)
								{
									_concreteReturnType.TypeParameters = new DataTypePointer[tcs.Length];
									for (int i = 0; i < tcs.Length; i++)
									{
										for (int k = 0; k < _genericParams.Length; k++)
										{
											if (_genericParams[k].Equals(tcs[i]))
											{
												_concreteReturnType.TypeParameters[i] = TypeParameters[k];
												break;
											}
										}
									}
								}
							}
							else if (_returnType.IsGenericParameter)
							{
								for (int i = 0; i < _genericParams.Length; i++)
								{
									if (_returnType.Equals(_genericParams[i]))
									{
										_concreteReturnType = TypeParameters[i];
									}
								}
							}
						}
					}
				}
			}
		}
		public virtual Dictionary<string, string> GetParameterDescriptions()
		{
			string methodDesc;
			string returnDesc;
			Dictionary<string, string> ps = PMEXmlParser.GetMethodDescription(VPLUtil.GetObjectType(Owner.ObjectType), MethodDef, out methodDesc, out returnDesc);
			IDynamicMethodParameters d = Owner.ObjectInstance as IDynamicMethodParameters;
			if (d != null)
			{
				Dictionary<string, string> pc = d.GetParameterDescriptions(this.MemberName);
				if (pc != null)
				{
					foreach (KeyValuePair<string, string> kv in pc)
					{
						if (ps.ContainsKey(kv.Key))
						{
							ps[kv.Key] = kv.Value;
						}
						else
						{
							ps.Add(kv.Key, kv.Value);
						}
					}
				}
			}
			return ps;
		}
		protected void CreateParameterExpressions()
		{
			MethodBase mb = MethodDef;
			if (mb != null)
			{
			}
			else
			{
				_parameterExpressions = null;
				_parameterJS = null;
			}
		}
		/// <summary>
		/// for using in math exp compile
		/// </summary>
		/// <param name="ps"></param>
		public void SetParameterExpressions(CodeExpression[] ps)
		{
			_parameterExpressions = ps;
		}
		public void SetParameterJS(string[] ps)
		{
			_parameterJS = ps;
		}
		public void SetParameterPhp(string[] ps)
		{
			_parameterPhp = ps;
		}
		public string GetMethodSignature(out int paramCount)
		{
			string name = MemberName;
			MethodBase mb = MethodDef;
			if (mb != null && !(mb is ConstructorInfo))
			{
				if (mb.ContainsGenericParameters)
				{
					Type[] tcs = mb.GetGenericArguments();
					StringBuilder sb = new StringBuilder(name);
					sb.Append("<");
					if (tcs.Length > 0)
					{
						sb.Append(tcs[0].Name);
						for (int i = 1; i < tcs.Length; i++)
						{
							sb.Append(";");
							sb.Append(tcs[i].Name);
						}
					}
					sb.Append(">");
					name = sb.ToString();
				}
			}
			return GetMethodSignature(name, true, _paramTypes, ReturnType, out paramCount);
		}
		public static string GetMethodSignature(MethodInfo mif, bool includeMethodName)
		{
			int n;
			return GetMethodSignature(mif, null, includeMethodName, out n);
		}
		public static string GetMethodSignature(MethodInfo mif)
		{
			int n;
			return GetMethodSignature(mif, null, true, out n);
		}
		public static string GetMethodSignature(MethodInfo mif, string name, bool includeMethodName, out int paramCount)
		{
			ParameterInfo[] pifs = mif.GetParameters();
			Type[] pp = new Type[pifs.Length];
			for (int i = 0; i < pifs.Length; i++)
			{
				pp[i] = pifs[i].ParameterType;
			}
			if (string.IsNullOrEmpty(name))
			{
				name = mif.Name;
			}
			if (mif.ContainsGenericParameters)
			{
				Type[] tcs = mif.GetGenericArguments();
				StringBuilder sb = new StringBuilder(name);
				sb.Append("<");
				if (tcs.Length > 0)
				{
					sb.Append(VPLUtil.GetTypeDisplay(tcs[0]));
					for (int i = 1; i < tcs.Length; i++)
					{
						sb.Append(";");
						sb.Append(VPLUtil.GetTypeDisplay(tcs[i]));
					}
				}
				sb.Append(">");
				name = sb.ToString();
			}
			return GetMethodSignature(name, includeMethodName, pp, mif.ReturnType, out paramCount);
		}
		public static string GetMethodSignature(string methodName, bool includeMethodName, Type[] paramTypes, Type returnType, out int paramCount)
		{
			StringBuilder sb = new StringBuilder();
			if (includeMethodName)
			{
				sb.Append(methodName);
			}
			paramCount = 0;
			if ((paramTypes != null && paramTypes.Length > 0) || (returnType != null && !returnType.Equals(typeof(void))))
			{
				sb.Append("(");
				if (paramTypes != null && paramTypes.Length > 0)
				{
					paramCount = paramTypes.Length;
					sb.Append(VPLUtil.GetTypeDisplay(paramTypes[0]));
					for (int i = 1; i < paramTypes.Length; i++)
					{
						sb.Append(",");
						if (paramTypes[i] == null)
						{
							sb.Append("null");
						}
						else
						{
							sb.Append(VPLUtil.GetTypeDisplay(paramTypes[i]));
						}
					}
				}
				sb.Append(")");
				if (returnType != null && !returnType.Equals(typeof(void)))
				{
					sb.Append(VPLUtil.GetTypeDisplay(returnType));
				}
			}
			return sb.ToString();
		}
		protected abstract void OnSetMethodDef(MethodBase method);
		public void SetMethodInfo(MethodBase method)
		{
			OnSetMethodDef(method);
			if (method != null)
			{
				MemberName = method.Name;
			}
			loadParameterTypes();

		}
		public abstract void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver);
		public abstract void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver);
		public abstract void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug);
		public override string ToString()
		{
			if (Owner == null)
			{
				return "?.?";
			}
			if (string.IsNullOrEmpty(this.MemberName))
				return Owner.DisplayName + ".?";
			else
				return Owner.DisplayName + "." + this.DisplayName;
		}
		protected override void OnCopy(MemberPointer obj)
		{
			MethodPointer mp = obj as MethodPointer;
			if (mp != null)
			{
				this.SetMethodInfo(mp.MethodDef);
				_id = mp.MethodID;
				_returnType = mp._returnType;
				_paramTypes = mp._paramTypes;
				_prj = mp._prj;
				_parameterJS = mp._parameterJS;
				TypeParameters = mp.TypeParameters;
			}
		}
		public virtual string GetParameterName(int parameterPosition)
		{
			ParameterInfo[] pifs = Info;
			return pifs[parameterPosition].Name;
		}
		protected void loadParameterTypes()
		{
			MethodBase mb = MethodDef;
			if (mb != null && !(mb is ConstructorInfo))
			{
				if (mb.ContainsGenericParameters)
				{
					_genericParams = mb.GetGenericArguments();
				}
			}
			ParameterInfo[] pifs = Info;
			if (pifs != null && pifs.Length > 0)
			{
				Type[] pts = new Type[pifs.Length];
				for (int j = 0; j < pifs.Length; j++)
				{
					pts[j] = pifs[j].ParameterType;
				}
				ParameterTypes = pts;
				//
			}
			else
			{
				ParameterTypes = Type.EmptyTypes;
			}
		}
		#endregion
		#region properties
		[Browsable(false)]
		public virtual Type ReturnType { get { return _returnType; } }
		[Browsable(false)]
		public abstract MethodBase MethodDef { get; }
		[Browsable(false)]
		public abstract bool NoReturn { get; }
		[Browsable(false)]
		public abstract bool HasReturn { get; }
		[Browsable(false)]
		public virtual bool IsWebMethod
		{
			get
			{
				if (MethodDef != null)
				{
					object[] vs = MethodDef.GetCustomAttributes(typeof(WebMethodAttribute), true);
					if (vs != null && vs.Length > 0)
					{
						return true;
					}
				}
				return false;
			}
		}
		public virtual bool ContainsGenericParameters
		{
			get
			{
				MethodBase mb = MethodDef;
				if (mb != null && !(mb is ConstructorInfo))
				{
					if (mb.ContainsGenericParameters)
					{
						Type[] ts = mb.GetGenericArguments();
						return (ts != null && ts.Length > 0);
					}
				}
				return false;
			}
		}
		public Type[] GetGenericArguments()
		{
			MethodBase mb = MethodDef;
			if (mb != null && !(mb is ConstructorInfo))
			{
				if (mb.ContainsGenericParameters)
				{
					return mb.GetGenericArguments();
				}
			}
			return null;
		}
		[Browsable(false)]
		public bool IsArrayMethod
		{
			get
			{
				if (MethodDef != null && MethodDef.DeclaringType != null)
				{
					return MethodDef.DeclaringType.IsArray;
				}
				return false;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (_paramTypes != null)
					return _paramTypes.Length;
				return 0;
			}
		}
		[Browsable(false)]
		public override string DisplayName
		{
			get
			{
				int c;
				return this.GetMethodSignature(out c);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return Owner.ObjectType;
			}
			set
			{
				Owner.ObjectType = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				if (Owner != null)
				{
					return Owner.ObjectInstance;
				}
				return null;
			}
			set
			{
				if (Owner != null)
				{
					Owner.ObjectInstance = value;
				}
			}
		}
		[Browsable(false)]
		public ParameterInfo[] Info
		{
			get
			{
				IDynamicMethodParameters dmp = ObjectInstance as IDynamicMethodParameters;
				if (dmp != null)
				{
					ParameterInfo[] ps = dmp.GetDynamicMethodParameters(MethodName, Action);
					if (ps != null)
					{
						return ps;
					}
				}
				if (MethodDef != null)
				{
					return MethodDef.GetParameters();
				}
				return null;
			}
		}
		[PropertyReadOrder(101)]
		[Browsable(false)]
		public Type[] GenericParameters
		{
			get
			{
				if (_genericParams == null)
				{
					MethodBase mb = MethodDef;
					if (mb != null)
					{
						_genericParams = mb.GetGenericArguments();
					}
				}
				return _genericParams;
			}
			set
			{
				if (value == null)
					_genericParams = Type.EmptyTypes;
				else
					_genericParams = value;
			}
		}
		[PropertyReadOrder(100)]
		[Browsable(false)]
		public Type[] ParameterTypes
		{
			get
			{
				ParameterInfo[] info = Info;
				if (info != null)
				{
					_paramTypes = new Type[info.Length];
					for (int i = 0; i < info.Length; i++)
					{
						_paramTypes[i] = info[i].ParameterType;
					}
				}
				return _paramTypes;
			}
			set
			{
				if (value == null)
					_paramTypes = Type.EmptyTypes;
				else
					_paramTypes = value;
			}
		}
		[Browsable(false)]
		public Type ReturnType2
		{
			get
			{
				return _returnType;
			}
			set
			{
				_returnType = value;
			}
		}
		#endregion
		#region IMethod Members
		LimnorProject _prj;
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public object ModuleProject
		{
			get
			{
				if (_prj != null)
				{
					return _prj;
				}
				if (_action != null)
				{
					return _action.Project;
				}
				return null;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		public virtual bool CanBeReplacedInEditor { get { return true; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public virtual Type ActionBranchType
		{
			get
			{
				return typeof(AB_SingleAction);
			}
		}
		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return typeof(ActionClass);
			}
		}
		public void ValidateParameterValues(ParameterValueCollection parameters)
		{
			Type[] tps = this.ParameterTypes;
			ParameterValueCollection.ValidateParameterValues(parameters, tps, this);
		}
		public virtual object GetParameterTypeByIndex(int idx)
		{
			if (idx >= 0)
			{
				ParameterInfo[] ps = this.Info;
				if (ps != null && idx < ps.Length)
				{
					return ps[idx];
				}
			}
			return null;
		}
		public virtual void SetParameterTypeByIndex(int idx, object type)
		{
		}
		public virtual object GetParameterType(UInt32 id)
		{
			ParameterInfo[] ps = this.Info;
			if (ps != null)
			{
				int idi = (int)id;
				if (ps != null)
				{
					if (ps.Length == 1)
					{
						return ps[0].ParameterType;
					}
					for (int i = 0; i < ps.Length; i++)
					{
						if (string.IsNullOrEmpty(ps[i].Name))
						{
							if (ps[i].ParameterType.GetHashCode() == idi)
							{
								return ps[i].ParameterType;
							}
							else if (ps[i].GetHashCode() == idi)
							{
								return ps[i].ParameterType;
							}
							else if (ps[i].MetadataToken == idi)
							{
								return ps[i].ParameterType;
							}
						}
						else if (ps[i].Name.GetHashCode() == idi)
						{
							return ps[i].ParameterType;
						}
					}
				}
			}
			return null;
		}
		public virtual object GetParameterType(string name)
		{
			ParameterInfo[] ps = this.Info;
			if (ps != null)
			{
				if (ps != null)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						if (string.Compare(name, ps[i].Name, StringComparison.Ordinal) == 0)
						{
							return ps[i].ParameterType;
						}
					}
				}
			}
			return null;
		}
		public bool IsSameMethod(IMethod method)
		{
			MethodPointer mp = method as MethodPointer;
			if (mp != null)
			{
				return this.IsSameObjectRef(mp);
			}
			return false;
		}
		[Browsable(false)]
		public virtual bool IsForLocalAction
		{
			get
			{
				if (Owner == null)
				{
					return true;
				}
				else
				{
					LocalVariable lv = Owner as LocalVariable;
					if (lv != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		/// set via dialogue box
		/// </summary>
		[Browsable(false)]
		[NotForProgramming]
		public DataTypePointer[] TypeParameters
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return MemberName;
			}
			set
			{
				MemberName = value;
			}
		}
		[Browsable(false)]
		public virtual string DefaultActionName
		{
			get
			{
				if (Owner != null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					  "{0}.{1}", Owner.ExpressionDisplay, this.MemberName);
				}
				IClass c = Holder;
				if (c != null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					  "{0}.{1}", c.ExpressionDisplay, this.MemberName);
				}
				else
				{
					IObjectPointer op = Owner;
					if (op != null)
					{
						return string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}.{1}", op.ExpressionDisplay, this.MemberName);
					}
					else
					{
						return this.MemberName;
					}
				}
			}
		}
		#endregion
		#region IMethodCompile Members
		public object CompilerData(string key)
		{
			return null;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public CodeTypeDeclaration TypeDeclaration
		{
			get
			{
				return _td;
			}
			set
			{
				_td = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public CodeMemberMethod MethodCode
		{
			get
			{
				return _methodCode;
			}
			set
			{
				_methodCode = value;
			}
		}
		[Browsable(false)]
		public UInt32 MethodID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		private Stack<IMethod0> _subMethods;
		/// <summary>
		/// when compiling a sub method, set the sub-method here to pass ot into compiling elements
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Stack<IMethod0> SubMethod
		{
			get
			{
				if (_subMethods == null)
					_subMethods = new Stack<IMethod0>();
				return _subMethods;
			}
		}
		public IMethod GetSubMethodByParameterId(UInt32 parameterId)
		{
			IMethod smi = null;
			Stack<IMethod0>.Enumerator en = SubMethod.GetEnumerator();
			while (en.MoveNext())
			{
				IMethod mp = en.Current as IMethod;
				if (mp != null)
				{
					IList<IParameter> ps = mp.MethodParameterTypes;
					if (ps != null && ps.Count > 0)
					{
						foreach (IParameter p in ps)
						{
							if (p.ParameterID == parameterId)
							{
								smi = mp;
								break;
							}
						}
					}
					if (smi != null)
					{
						break;
					}
				}
			}
			return smi;
		}
		#endregion
		#region IActionMethodPointer Members
		public bool HasFormControlParent
		{
			get
			{
				return DesignUtil.HasFormControlParent(this.Owner);
			}
		}
		public IObjectPointer MethodDeclarer { get { return this.RootPointer; } }
		public virtual ParameterValue CreateDefaultParameterValue(int i)
		{
			ParameterInfo[] ps = this.Info;
			if (ps == null || i >= ps.Length)
			{
				return null;
			}
			else
			{
				ParameterInfo p = ps[i];
				ParameterValue pv = new ParameterValue(_action);
				if (string.IsNullOrEmpty(p.Name))
				{
					pv.Name = "parameter" + i.ToString();
				}
				else
				{
					pv.Name = p.Name;
				}
				pv.ParameterID = (UInt32)p.GetHashCode();
				Type dt = null;
				if (p.ParameterType.IsGenericParameter)
				{
					if (GenericParameters != null && GenericParameters.Length > 0)
					{
						for (int k = 0; k < _genericParams.Length; k++)
						{
							if (_genericParams[k].Equals(p.ParameterType))
							{
								if (TypeParameters == null)
								{
									throw new DesignerException("Error resolving generic parameter [{0}]. Concret types are not set", p.Name);
								}
								if (k < TypeParameters.Length)
								{
									dt = TypeParameters[k].BaseClassType;
								}
								else
								{
									throw new DesignerException("Error resolving generic parameter [{0}]. Concret types:[{1}]. Parameter position:[{2}]", p.Name, TypeParameters.Length, k);
								}
								break;
							}
						}
					}
					if (dt == null)
					{
						ILocalvariable lv = this.Owner as ILocalvariable;
						if (lv != null)
						{
							if (lv.ValueType == null)
							{
								throw new DesignerException("Error resolving generic parameter [{0}]. Data type for variable [{1}] is null", p.Name, lv);
							}
							DataTypePointer dtp = lv.ValueType.GetConcreteType(p.ParameterType);
							if (dtp != null)
							{
								dt = dtp.BaseClassType;
							}
							else
							{
								throw new DesignerException("Variable type [{0}] does not support generic parameter [{1}]", lv.ValueType, p.Name);
							}
						}
						else
						{
							IProperty ip = this.Owner as IProperty;
							if (ip != null)
							{
								if (ip.PropertyType == null)
								{
									throw new DesignerException("Error resolving generic parameter [{0}]. Data type for Property [{1}] is null", p.Name, ip);
								}
								DataTypePointer dtp = ip.PropertyType.GetConcreteType(p.ParameterType);
								if (dtp != null)
								{
									dt = dtp.BaseClassType;
								}
								else
								{
									throw new DesignerException("Property type [{0}] does not support generic parameter [{1}]", ip.PropertyType, p.Name);
								}
							}
							else
							{
								throw new DesignerException("Unsupported owner [{0}] for generic parameter [{1}]", this.Owner, p.Name);
							}
						}
					}
				}
				else
				{
					dt = p.ParameterType;
				}
				pv.SetDataType(dt);
				pv.ValueType = EnumValueType.ConstantValue;
				return pv;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}
		public Type ReturnBaseType
		{
			get
			{
				return ReturnType;
			}
		}

		public DataTypePointer ReturnTypeConcrete
		{
			get
			{
				return _concreteReturnType;
			}
		}
		#endregion
		#region IMethodPointer Members
		[Browsable(false)]
		public IMethod MethodPointed { get { return this; } }
		public bool IsSameMethod(IMethodPointer pointer)
		{
			MethodPointer mp = pointer as MethodPointer;
			if (mp != null)
			{
				return (string.Compare(mp.MethodSignature, this.MethodSignature, StringComparison.Ordinal) == 0);
			}
			return false;
		}

		#endregion
		#region IMethodCompile Members
		[DefaultValue(false)]
		[Description("If this property is True then Control updating actions are executed under UI thread.")]
		public virtual bool MakeUIThreadSafe
		{
			get;
			set;
		}
		public string GetParameterCodeNameById(uint id)
		{
			ParameterInfo[] ps = Info;
			int i = (int)(id - 1);
			return ps[i].Name;
		}

		#endregion
		#region IMethod Members
		public virtual bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public virtual string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IParameter MethodReturnType
		{
			get
			{
				return new ReturnParameter(this.ReturnType);
			}
			set
			{
			}
		}

		public string ParameterName(int i)
		{
			return Info[i].Name;
		}
		public Type[] GetParameterTypes()
		{
			return _paramTypes;
		}
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MethodPointer fe = (MethodPointer)this.Clone();
			MemberComponentIdCustom.AdjustHost(fe);
			fe.Action = (IAction)action;
			return fe;
		}

		#endregion
		#region IMethod0 Members

		[Browsable(false)]
		public virtual IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> ps = new List<IParameter>();
				ParameterInfo[] info = Info;
				if (info != null)
				{
					for (int i = 0; i < info.Length; i++)
					{
						ps.Add(new ParameterLib(info[i], i));
					}
				}
				return ps;
			}
		}

		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			ResolveGenericParameters(objMap.Project);
		}

		#endregion
		#region IGenericTypePointer Members
		/// <summary>
		/// returns corresponding concrete types for generic arguments
		/// 1. method parameters
		/// 2. method return type
		/// 3. owner type arguments
		/// different sections may overlap, duplications may exist
		/// </summary>
		/// <returns></returns>
		public DataTypePointer[] GetConcreteTypes()
		{
			List<DataTypePointer> l = new List<DataTypePointer>();
			if (_paramTypes != null && _paramTypes.Length > 0)
			{
				for (int i = 0; i < _paramTypes.Length; i++)
				{
					if (_paramTypes[i].IsGenericParameter || _paramTypes[i].IsGenericType)
					{
						DataTypePointer dp = GetConcreteType(_paramTypes[i]);
						if (dp != null)
						{
							l.Add(dp);
						}
					}
				}
			}
			if (_concreteReturnType != null)
			{
				l.Add(_concreteReturnType);
			}
			IGenericTypePointer igp = this.Owner as IGenericTypePointer;
			if (igp != null)
			{
				DataTypePointer[] dps = igp.GetConcreteTypes();
				if (dps != null && dps.Length > 0)
				{
					DataTypePointer[] ps = new DataTypePointer[l.Count + dps.Length];
					l.CopyTo(ps, 0);
					dps.CopyTo(ps, l.Count);
					return ps;
				}
			}
			return l.ToArray();
		}
		/// <summary>
		/// returns corresponding concrete type for the given generic argument or generic parameter
		/// </summary>
		/// <param name="typeParameter"></param>
		/// <returns></returns>
		public virtual DataTypePointer GetConcreteType(Type typeParameter)
		{
			if (typeParameter != null)
			{
				IObjectPointer op;
				if (TypeParameters != null)
				{
					if (typeParameter.IsGenericParameter || typeParameter.IsGenericType)
					{
						MethodBase mb = MethodDef;
						if (mb != null)
						{
							if (mb.ContainsGenericParameters)
							{
								Type[] tcs = mb.GetGenericArguments();
								if (tcs != null && tcs.Length > 0)
								{
									for (int i = 0; i < tcs.Length; i++)
									{
										if (tcs[i].Equals(typeParameter))
										{
											if (i < TypeParameters.Length)
											{
												return TypeParameters[i];
											}
										}
									}
								}
							}
						}
					}
					op = this.Owner;
					IGenericTypePointer igp = null;
					while (op != null)
					{
						igp = op as IGenericTypePointer;
						if (igp != null)
							break;
						op = op.Owner;
					}
					if (igp != null)
					{
						return igp.GetConcreteType(typeParameter);
					}
				}
				LocalVariable lv = null;
				op = this.Owner;
				while (op != null)
				{
					lv = op as LocalVariable;
					if (lv != null)
					{
						break;
					}
					op = op.Owner;
				}
				if (lv != null)
				{
					return lv.GetConcreteType(typeParameter);
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public CodeTypeReference GetCodeTypeReference()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// each generic type is defined by a DataTypePointer with its BaseClassType being the generic parameter and its _concretTypeForTypeParameter being the concrete type
		/// </summary>
		/// <returns></returns>
		public IList<DataTypePointer> GetGenericTypes()
		{
			List<DataTypePointer> l = new List<DataTypePointer>();
			if (_paramTypes != null && _paramTypes.Length > 0)
			{
				for (int i = 0; i < _paramTypes.Length; i++)
				{
					if (_paramTypes[i] != null && (_paramTypes[i].IsGenericParameter || _paramTypes[i].IsGenericType))
					{
						DataTypePointer dp = new DataTypePointer(_paramTypes[i]);
						DataTypePointer cp = GetConcreteType(_paramTypes[i]);
						if (cp != null)
						{
							dp.SetConcreteType(cp);
							l.Add(dp);
						}
					}
				}
			}
			if (_returnType != null && _concreteReturnType != null)
			{
				bool found = false;
				foreach (DataTypePointer p in l)
				{
					if (_returnType.Equals(p.BaseClassType))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					DataTypePointer dp = new DataTypePointer(_returnType);
					dp.SetConcreteType(_concreteReturnType);
					l.Add(dp);
				}
			}
			if (TypeParameters != null && TypeParameters.Length > 0)
			{
				if (_genericParams != null && _genericParams.Length == TypeParameters.Length)
				{
					for (int i = 0; i < _genericParams.Length; i++)
					{
						bool found = false;
						foreach (DataTypePointer p in l)
						{
							if (_genericParams[i].Equals(p.BaseClassType))
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							DataTypePointer dp = new DataTypePointer(_genericParams[i]);
							dp.SetConcreteType(TypeParameters[i]);
							l.Add(dp);
						}
					}
				}
			}
			IGenericTypePointer igp = this.Owner as IGenericTypePointer;
			if (igp != null)
			{
				IList<DataTypePointer> ls = igp.GetGenericTypes();
				if (ls != null && ls.Count > 0)
				{
					foreach (DataTypePointer p0 in ls)
					{
						bool found = false;
						foreach (DataTypePointer p in l)
						{
							if (p0.Equals(p.BaseClassType))
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							l.Add(p0);
						}
					}
				}
			}
			return l;
		}

		#endregion
	}
}
