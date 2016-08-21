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
using VPL;
using ProgElements;
using MathExp;
using System.ComponentModel;
using System.CodeDom;
using VSPrj;
using LimnorDesigner.MethodBuilder;
using XmlSerializer;
using XmlUtility;
using System.Xml;
using LimnorDesigner.Action;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Globalization;
using Limnor.WebBuilder;

namespace LimnorDesigner.Event
{
	/// <summary>
	/// this method is associated with a custom event
	/// </summary>
	//[UseParentObject]
	public class FireEventMethod : ICloneable, IObjectPointer, IWithProject, IMethod, IActionMethodPointer, IMethodCompile, ICustomObject, IActionCompiler, IXmlNodeSerializable
	{
		#region fields and constructors
		private UInt32 _memberId;
		private string _signature; //buffer for MethodSignature
		private CodeTypeDeclaration _td;
		private CodeMemberMethod _codeMemberMethod;
		private ParameterClass _returnType;
		private object _compiler;
		private CustomEventPointer _event; //custom event to be associated
		private IAction _action;
		public FireEventMethod()
		{
		}
		public FireEventMethod(CustomEventPointer e)
		{
			_event = e;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public CustomEventPointer Event
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
		[ReadOnly(true)]
		[Browsable(false)]
		public IAction Action
		{
			get
			{
				if (_action == null)
				{
				}
				return _action;
			}
			set
			{
				_action = value;
			}
		}
		#endregion
		#region Methods
		public bool IsSameEvent(EventClass e)
		{
			return _event.IsSameObjectRef(e);
		}
		public void SetCompilerData(object c)
		{
			_compiler = c;
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			FireEventMethod obj = new FireEventMethod(_event);
			return obj;
		}

		#endregion

		#region IObjectPointer Members
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				return _event.RootPointer;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				return _event.Owner;
			}
			set
			{
				_event.Owner = value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				return _event.ObjectType;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				return _event.ObjectInstance;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectDebug
		{
			get
			{
				return _event.ObjectDebug;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return _event.ReferenceName;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return _event.CodeName; }
		}
		[Browsable(false)]
		public string DisplayName
		{
			get { return _event.DisplayName; }
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
		[Browsable(false)]
		public virtual string ExpressionDisplay
		{
			get
			{
				return _event.Name;
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return _event.IsTargeted(target);
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "ef_{0}", _event.ObjectKey); }
		}
		[Browsable(false)]
		public string TypeString
		{
			get { return _event.TypeString; }
		}
		[Browsable(false)]
		public bool IsValid
		{
			get { return _event.IsValid; }
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return _event.GetReferenceCode(method, statements, forValue);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return _event.GetJavaScriptReferenceCode(code);
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return _event.GetPhpScriptReferenceCode(code);
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			FireEventMethod fe = objectIdentity as FireEventMethod;
			if (fe != null)
			{
				return fe.Event.IsSameObjectRef(_event);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return _event.IdentityOwner; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get { return _event.IsStatic; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return _event.ObjectDevelopType; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return _event.PointerType; }
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get
			{
				if (_prj != null)
				{
					return _prj;
				}
				if (_event != null)
				{
					if (_event.Declarer != null && _event.Declarer.Project != null)
					{
						return _event.Declarer.Project;
					}
					if (_event.Holder != null && _event.Holder.RootPointer != null && _event.Holder.RootPointer.Project != null)
					{
						return _event.Holder.RootPointer.Project;
					}
				}
				return null;
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
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			FireEventMethod fe = (FireEventMethod)this.Clone();
			fe._action = (IAction)action;
			return fe;
		}
		private LimnorProject _prj;
		[XmlIgnore]
		[ReadOnly(true)]
		public object ModuleProject
		{
			get
			{
				return Project;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public Type ActionType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return null;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string MethodName
		{
			get
			{
				return Name;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get
			{
				return "FireEvent_" + _event.Holder.Name + "." + _event.Name;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IParameter MethodReturnType
		{
			get
			{
				if (_returnType == null)
				{
					_returnType = new ParameterClass(typeof(void), "return", this);
				}
				return _returnType;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectIdentity ReturnPointer
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
		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> parameters = new List<IParameter>();
				List<ParameterClass> ps = _event.GetParameters(this);
				foreach (ParameterClass p in ps)
				{
					parameters.Add(p);
				}
				return parameters;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get { return _event.ParameterCount; }
		}
		/// <summary>
		/// {event name}(parameters)
		/// </summary>
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				if (string.IsNullOrEmpty(_signature))
				{
					StringBuilder sb = new StringBuilder(_event.Name);
					sb.Append("(");
					List<ParameterClass> ps = _event.GetParameters(this);
					for (int i = 0; i < ps.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						sb.Append(ps[i].TypeString);
						sb.Append(" ");
						sb.Append(ps[i].Name);
					}
					sb.Append(")");
					_signature = sb.ToString();
				}
				return _signature;
			}
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool NoReturn { get { return true; } }
		[Browsable(false)]
		public bool HasReturn
		{
			get { return false; }
		}

		public bool IsSameMethod(IMethodPointer method)
		{
			FireEventMethod fe = method as FireEventMethod;
			if (fe != null)
			{
				return (fe.Event.IsSameObjectRef(_event));
			}
			return false;
		}

		public void ValidateParameterValues(ParameterValueCollection parameters)
		{
			List<ParameterClass> ps = _event.GetParameters(this);
			if (ps.Count > 0)
			{
				if (ps[0].IsLibType && typeof(object).Equals(ps[0].BaseClassType))
				{
					if (string.Compare("sender", ps[0].Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (parameters.Count == 0)
						{
							ParameterValue pv = new ParameterValue(_action);
							pv.Name = ps[0].Name;
							pv.SetDataType(typeof(object));
							pv.ValueType = EnumValueType.Property;
							pv.Property = _event.RootPointer;
							parameters.Add(pv);
						}
						if (ps.Count > 1)
						{
							if (ps[1].IsLibType && typeof(EventArgs).Equals(ps[1].BaseClassType))
							{
								if (parameters.Count < 2)
								{
									ParameterValue pv = new ParameterValue(_action);
									pv.Name = ps[1].Name;
									pv.SetDataType(typeof(EventArgs));
									pv.ValueType = EnumValueType.Property;
									FieldPointer fp = new FieldPointer();
									fp.Owner = new DataTypePointer(new TypePointer(typeof(EventArgs)));
									fp.MemberName = "Empty";
									pv.Property = fp;
									parameters.Add(pv);
								}
							}
						}
					}
				}
			}
			ParameterValueCollection.ValidateParameterValues(parameters, ps, this);
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			List<ParameterClass> ps = _event.GetParameters(this);
			Dictionary<string, string> desc = new Dictionary<string, string>();
			for (int i = 0; i < ps.Count; i++)
			{
				if (string.IsNullOrEmpty(ps[i].Description))
					desc.Add(ps[i].Name, ps[i].Name);
				else
					desc.Add(ps[i].Name, ps[i].Description);
			}
			return desc;
		}

		public string ParameterName(int i)
		{
			List<ParameterClass> ps = _event.GetParameters(this);
			if (i >= 0 && i < ps.Count)
			{
				return ps[i].Name;
			}
			return null;
		}

		public object GetParameterType(UInt32 id)
		{
			foreach (IParameter p in MethodParameterTypes)
			{
				if (p.ParameterID == id)
				{
					return p;
				}
			}
			return null;
		}
		public object GetParameterTypeByIndex(int idx)
		{
			if (idx >= 0 && idx < MethodParameterTypes.Count)
			{
				return MethodParameterTypes[idx];
			}
			return null;
		}
		public object GetParameterType(string name)
		{
			foreach (IParameter p in MethodParameterTypes)
			{
				if (string.Compare(name, p.Name, StringComparison.Ordinal) == 0)
				{
					return p;
				}
			}
			return null;
		}

		#endregion

		#region IMethodCompile Members
		[ReadOnly(true)]
		[Browsable(false)]
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
		[ReadOnly(true)]
		[Browsable(false)]
		public CodeMemberMethod MethodCode
		{
			get
			{
				return _codeMemberMethod;
			}
			set
			{
				_codeMemberMethod = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public uint MethodID
		{
			get
			{
				return MemberId;
			}
			set
			{
			}
		}

		public string GetParameterCodeNameById(uint id)
		{
			List<ParameterClass> ps = _event.GetParameters(this);
			foreach (ParameterClass p in ps)
			{
				if (p.MemberId == id)
				{
					return p.CodeName;
				}
			}
			return null;
		}

		public object CompilerData(string key)
		{
			return _compiler;
		}
		//
		private Stack<IMethod0> _subMethods;
		/// <summary>
		/// when compiling a sub method, set the sub-method here to pass it into compiling elements
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
		[Description("If this property is True then Control updating actions are executed under UI thread.")]
		public virtual bool MakeUIThreadSafe
		{
			get;
			set;
		}
		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(MemberId, ClassId);
			}
		}
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				return _event.ClassId;
			}
		}
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				if (_memberId == 0)
				{
					_memberId = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _memberId;
			}
		}
		[Browsable(false)]
		public UInt32 EventId
		{
			get
			{
				return _event.MemberId;
			}
		}
		[Browsable(false)]
		public string Name
		{
			get { return "FireEvent_" + _event.ReferenceName; }
		}

		#endregion

		#region IActionCompiler Members

		public void CreateJavaScript(StringCollection jsCode, StringCollection parameters, string returnReceiver)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(CodeName);
			sb.Append("(");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Append(parameters[0]);
				for (int i = 1; i < parameters.Count; i++)
				{
					sb.Append(",");
					sb.Append(parameters[i]);
				}
			}
			sb.Append(");\r\n");
			jsCode.Add(sb.ToString());
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
		}
		/// <summary>
		/// generate code:
		/// if( {event name} != null)
		/// {
		///     {event name}(parameters);
		/// }
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="methodToCompile"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="debug"></param>
		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			CodeConditionStatement cs = new CodeConditionStatement();
			cs.Condition = new CodeBinaryOperatorExpression(
				_event.GetReferenceCode(methodToCompile, statements, true), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			CodeMethodInvokeExpression cme = new CodeMethodInvokeExpression();
			if (_event.IsStatic || (_event.Declarer != null && _event.Declarer.IsStatic))
			{
				if (parameters != null && parameters.Count > 0)
				{
					CodeTypeReferenceExpression sender = parameters[0] as CodeTypeReferenceExpression;
					if (sender != null)
					{
						if (string.CompareOrdinal(sender.Type.BaseType, _event.Owner.TypeString) == 0)
						{
							parameters[0] = new CodeTypeOfExpression(sender.Type.BaseType);
						}
					}
				}
				cme.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(_event.Owner.TypeString), _event.Name);
			}
			else
				cme.Method = new CodeMethodReferenceExpression(_event.Owner.GetReferenceCode(methodToCompile, statements, false), _event.Name);
			if (parameters != null)
			{
				cme.Parameters.AddRange(parameters);
			}
			cs.TrueStatements.Add(cme);
			statements.Add(cs);
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XML_EventPointer = "EventPointer";
		const string XMLATT_firerId = "firerId";
		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlNode epNode = node.SelectSingleNode(XML_EventPointer);
			if (epNode == null)
			{
				epNode = node.OwnerDocument.CreateElement(XML_EventPointer);
				node.AppendChild(epNode);
			}
			else
			{
				epNode.RemoveAll();
			}
			XmlUtil.SetAttribute(node, XMLATT_firerId, MemberId);
			writer.WriteObjectToNode(epNode, _event, false);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			XmlNode epNode = node.SelectSingleNode(XML_EventPointer);
			if (epNode == null)
			{
				throw new DesignerException("EventPointer node not found");
			}
			_memberId = XmlUtil.GetAttributeUInt(node, XMLATT_firerId);
			_event = reader.ReadObject<CustomEventPointer>(epNode);
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
				return EnumWebRunAt.Inherit;
			}
		}
		public IObjectPointer MethodDeclarer { get { return this.RootPointer; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			IParameter p = MethodParameterTypes[i];
			ParameterValue pv = new ParameterValue(_action);
			pv.Name = p.Name;
			pv.ParameterID = p.ParameterID;
			ParameterClass pc = p as ParameterClass;
			if (pc != null)
			{
				pv.SetDataType(pc);
			}
			else
			{
				pv.SetDataType(p.ParameterLibType);
			}
			pv.SetOwnerAction(_action);
			pv.ValueType = EnumValueType.ConstantValue;
			return pv;
		}
		public IMethod MethodPointed
		{
			get { return this; }
		}

		public void SetParameterExpressions(CodeExpression[] ps)
		{
			throw new DesignerException("Calling FireEventMethod.SetParameterExpressions");
		}
		public void SetParameterJS(string[] ps)
		{
			throw new DesignerException("Calling FireEventMethod.SetParameterJS");
		}
		public void SetParameterPhp(string[] ps)
		{
			throw new DesignerException("Calling FireEventMethod.SetParameterJS");
		}
		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}

		public Type ReturnBaseType
		{
			get
			{
				return typeof(void);
			}
		}

		#endregion

		#region IMethodPointer Members

		public bool IsSameMethod(IMethod method)
		{
			FireEventMethod fe = method as FireEventMethod;
			if (fe != null)
			{
				return (fe.WholeId == this.WholeId);
			}
			return false;
		}

		#endregion
	}
}
