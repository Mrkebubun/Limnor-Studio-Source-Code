using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using LimnorDesigner.Action;
using LimnorDesigner.Interface;
using System.Xml;
using XmlUtility;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;
using ProgElements;
using System.Globalization;
using LimnorDesigner.MenuUtil;

namespace LimnorDesigner.MethodBuilder
{
	public class CustomInterfaceMethodPointer : IXmlNodeSerializable, ICloneable, IActionMethodPointer, IActionCompiler
	{
		#region fields and constructors
		private InterfaceElementMethod _method;
		private UInt64 _wholeId;
		private IClass _holder;
		private IAction _act;
		private CodeExpression[] _parameterExpressions;
		public CustomInterfaceMethodPointer()
		{
		}
		public CustomInterfaceMethodPointer(InterfaceElementMethod method, IClass holder, IAction act)
		{
			_act = act;
			_method = method;
			_wholeId = DesignUtil.MakeDDWord(_method.MethodId, holder.ClassId);
			_holder = holder;
		}
		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_InterfaceId = "interfaceId";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Data);
			if (dataNode != null)
			{
				_method = new InterfaceElementMethod();
				_method.OnReadFromXmlNode(serializer, dataNode);
				//object v = serializer.ReadObject(dataNode, this);
				//_method = v as InterfaceElementMethod;
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_method != null)
			{
				XmlNode dataNode = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				_method.OnWriteToXmlNode(serializer, dataNode);
			}
			//serializer.WriteObjectToNode(dataNode, _method);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CustomInterfaceMethodPointer obj = new CustomInterfaceMethodPointer(_method, _holder, _act);
			return obj;
		}

		#endregion

		#region IActionMethodPointer Members

		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			
		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}

		public object GetParameterType(uint id)
		{
			if(_method != null && _method.Parameters != null)
			{
				for(int i=0;i<_method.Parameters.Count;i++)
				{
					if(id == _method.Parameters[i].ParameterID)
						return _method.Parameters[i].ObjectType;
				}
			}
			return null;
		}

		public object GetParameterTypeByIndex(int idx)
		{
			if (_method != null && _method.Parameters != null)
			{
				if (idx >= 0 && idx < _method.Parameters.Count)
					return _method.Parameters[idx].ObjectType;
			}
			return null;
		}

		public object GetParameterType(string name)
		{
			if (_method != null && _method.Parameters != null)
			{
				for (int i = 0; i < _method.Parameters.Count; i++)
				{
					if (string.CompareOrdinal(name, _method.Parameters[i].Name) == 0)
						return _method.Parameters[i].ObjectType;
				}
			}
			return null;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return new Dictionary<string, string>();
		}

		public ParameterValue CreateDefaultParameterValue(int idx)
		{
			IParameter p = null;
			if (_method != null && _method.Parameters != null)
			{
				if (idx >= 0 && idx < _method.Parameters.Count)
				{
					p = _method.Parameters[idx];
				}
			}
			if (p != null)
			{
				ParameterValue pv = new ParameterValue(this.Action);
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
				pv.SetOwnerAction(_act);
				pv.ValueType = EnumValueType.ConstantValue;
				return pv;
			}
			return null;
		}

		public void SetParameterExpressions(CodeExpression[] ps)
		{
			_parameterExpressions = ps;
		}

		public void SetParameterJS(string[] ps)
		{
			
		}

		public void SetParameterPhp(string[] ps)
		{
			
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_method == null)
			{
				return null;
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression();
			cmi.Method = (CodeMethodReferenceExpression)_method.GetReferenceCode(method, statements, false);
			if (_parameterExpressions != null)
			{
				cmi.Parameters.AddRange(_parameterExpressions);
			}
			else
			{
				if (_method.Parameters != null && _method.Parameters.Count > 0)
				{
					throw new DesignerException("Calling CustomMethodPointer.GetReferenceCode without setting parameters");
				}
			}
			return cmi;
		}

		public string GetJavaScriptReferenceCode(StringCollection method)
		{
			return null;
		}

		public string GetPhpScriptReferenceCode(StringCollection method)
		{
			return null;
		}

		public Type ActionBranchType
		{
			get
			{
				return typeof(AB_SingleAction);
			}
		}

		public IAction Action
		{
			get
			{
				return _act;
			}
			set
			{
				_act = value;
			}
		}

		public string DefaultActionName
		{
			get { return string.Format(CultureInfo.InvariantCulture,"{0}",this.MethodName); }
		}

		public IMethod MethodPointed
		{
			get { return _method; }
		}

		public IObjectPointer Owner
		{
			get
			{
				if (_method != null && _method.VariablePointer != null)
					return _method.VariablePointer;
				if (_act != null)
				{
					return _act.MethodOwner;
				}
				return _holder;
			}
		}

		public bool IsArrayMethod
		{
			get
			{
				if (_method != null)
				{
					if (_method.ReturnType != null)
					{
						return _method.ReturnType.BaseClassType.IsArray;
					}
				}
				return false;
			}
		}

		public string MethodName
		{
			get
			{
				if (_method != null)
					return _method.MethodName;
				return string.Empty;
			}
		}

		public string CodeName
		{
			get
			{
				if (_method != null)
					return _method.MethodName;
				return string.Empty;
			}
		}

		public Type ReturnBaseType
		{
			get
			{
				if (_method != null && _method.ReturnType != null)
					return _method.ReturnType.BaseClassType;
				return typeof(void);
			}
		}

		public bool IsValid
		{
			get
			{
				if (_method != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_method is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public IObjectPointer MethodDeclarer
		{
			get
			{
				if (_method != null)
					return _method.VariablePointer;
				return null;
			}
		}

		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Server;
			}
		}

		public bool HasFormControlParent
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region IMethodPointer Members

		public bool NoReturn
		{
			get
			{
				if (_method != null)
				{
					if (_method.ReturnType == null)
						return true;
					if (typeof(void).Equals(_method.ReturnType.BaseClassType))
						return true;
					return false;
				}
				return true;
			}
		}

		public int ParameterCount
		{
			get
			{
				if (_method != null)
					return _method.ParameterCount;
				return 0;
			}
		}

		public bool IsMethodReturn
		{
			get
			{
				return false;
			}
		}

		public bool IsForLocalAction
		{
			get
			{
				return false;
			}
		}

		public bool IsSameMethod(IMethodPointer pointer)
		{
			CustomInterfaceMethodPointer cp = pointer as CustomInterfaceMethodPointer;
			if (cp != null)
			{
				if (_method != null)
				{
					return _method.IsSameMethod(cp.MethodPointed);
				}
			}
			return false;
		}

		public bool IsSameMethod(IMethod method)
		{
			if (_method != null)
			{
				return _method.IsSameMethod(method);
			}
			return false;
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			CustomInterfaceMethodPointer cp = objectIdentity as CustomInterfaceMethodPointer;
			if (cp != null)
			{
				if (_method != null)
				{
					return _method.IsSameMethod(cp.MethodPointed);
				}
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_method != null)
					return _method.VariablePointer;
				return null;
			}
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Custom; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Method; }
		}

		#endregion

		#region IActionCompiler Members

		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			if (_method == null)
			{
				return;
			}
			IObjectPointer op = _method.VariablePointer;
			if (op == null)
			{
				return;
			}
			ClassPointer cp = op as ClassPointer;
			CustomMethodPointer cmp = null;
			MethodClass mc = null;
			if (cp != null)
			{
				mc = cp.GetCustomMethodById(_method.MethodId);
				if (mc != null)
				{
					cmp = new CustomMethodPointer(mc, _holder);
					cmp.SetParameterExpressions(_parameterExpressions);
					cmp.Compile(currentAction, nextAction, compiler, methodToCompile, method, statements, parameters, returnReceiver, debug);
					return;
				}
			}
			//CustomMethodParameterPointer cmpp = _method.VariablePointer as CustomMethodParameterPointer;
			//if (cmpp != null)
			//{
			CodeExpression cmi = null;
			if (_method != null)
			{
				//_method.SetHolder(_holder);
				CodeMethodReferenceExpression mref = (CodeMethodReferenceExpression)_method.GetReferenceCode(methodToCompile, statements, false);
				CodeMethodInvokeExpression cmim = new CodeMethodInvokeExpression();
				cmim.Method = mref;
				cmim.Parameters.AddRange(parameters);
				cmi = cmim;
			}
			bool useOutput = false;
			if (!NoReturn && nextAction != null && nextAction.UseInput)
			{
				CodeVariableDeclarationStatement output = new CodeVariableDeclarationStatement(
					currentAction.OutputType.TypeString, currentAction.OutputCodeName, cmi);
				statements.Add(output);
				cmi = new CodeVariableReferenceExpression(currentAction.OutputCodeName);
				useOutput = true;
			}
			if (_method.HasReturn && returnReceiver != null)
			{
				CodeExpression cr = returnReceiver.GetReferenceCode(methodToCompile, statements, true);
				if (_method.ReturnType != null)
				{
					Type target;
					IClassWrapper wrapper = returnReceiver as IClassWrapper;
					if (wrapper != null)
					{
						target = wrapper.WrappedType;
					}
					else
					{
						target = returnReceiver.ObjectType;
					}
					Type dt;
					if (useOutput)
						dt = currentAction.OutputType.BaseClassType;
					else
						dt = _method.ReturnType.BaseClassType;
					CompilerUtil.CreateAssignment(cr, target, cmi, dt, statements, true);
				}
				else
				{
					CodeAssignStatement cas = new CodeAssignStatement(cr, cmi);
					statements.Add(cas);
				}
			}
			else
			{
				if (!useOutput)
				{
					CodeExpressionStatement ces = new CodeExpressionStatement(cmi);
					statements.Add(ces);
				}
			}
			//}
		}

		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
