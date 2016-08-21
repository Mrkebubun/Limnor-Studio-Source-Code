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
using ProgElements;
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;
using LimnorDesigner.MenuUtil;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Collections.Specialized;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// base method used as IAction.ActionMethod
	/// </summary>
	public class BaseMethod : MethodClassInherited, IActionCompiler, IActionMethodPointer
	{
		private CodeExpression[] _parameterExpressions; //for using in math exp compile
		private string[] _parameterJS;
		private string[] _parameterPhp;
		private IAction _action;
		public BaseMethod(ActionClass act)
			: base((ClassPointer)(act.MethodOwner))
		{
			_action = act;
		}
		public BaseMethod(ClassPointer owner)
			: base(owner)
		{
		}
		public override string DefaultActionName
		{
			get
			{
				return "base." + this.Name;
			}
		}

		/// <summary>
		/// for using in math exp compile
		/// </summary>
		[Browsable(false)]
		protected CodeExpression[] ParameterCodeExpressions
		{
			get
			{
				return _parameterExpressions;
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
			pv.ValueType = EnumValueType.Property;
			CustomMethodParameterPointer cmpp = new CustomMethodParameterPointer(pc);
			pv.SetValue(cmpp);
			return pv;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeBaseReferenceExpression br = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(br, this.Name);
			if (_parameterExpressions != null)
			{
				cmi.Parameters.AddRange(_parameterExpressions);
			}
			else
			{
				throw new DesignerException("Calling BaseMethod.GetReferenceCode without setting parameters");
			}
			return cmi;
		}
		public IMethod MethodPointed
		{
			get
			{
				return this;
			}
		}
		#region IActionCompiler Members
		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
		}
		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			bool useOutput = false;
			CodeExpression cmi;
			CodeBaseReferenceExpression br = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression cmim = new CodeMethodInvokeExpression(br, this.Name);
			if (parameters != null)
			{
				cmim.Parameters.AddRange(parameters);
			}
			cmi = cmim;
			if (!NoReturn && nextAction != null && nextAction.UseInput)
			{
				CodeVariableDeclarationStatement output = new CodeVariableDeclarationStatement(
					currentAction.OutputType.TypeString, currentAction.OutputCodeName, cmi);
				statements.Add(output);
				cmi = new CodeVariableReferenceExpression(currentAction.OutputCodeName);

				useOutput = true;
			}
			if (HasReturn && returnReceiver != null)
			{
				CodeExpression cr = returnReceiver.GetReferenceCode(methodToCompile, statements, true);
				if (ReturnValue != null)
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
						dt = ReturnValue.BaseClassType;
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

		}
		public Type ReturnBaseType
		{
			get
			{
				return this.MethodReturnType.ParameterLibType;
			}
		}
		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
				XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_MethodID, MemberId);
				XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_ClassID, ClassId);
			}
			else
			{
				UInt32 classId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ClassID);
				MemberId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_MethodID);
				ClassPointer root = RootPointer;
				if (root == null)
				{
					throw new DesignerException("Owner class not set for base method [{0},{1}]", classId, MemberId);
				}
				if (classId == root.ClassId)
				{
					SetDeclarerDirect(root);
				}
				else
				{
					ClassPointer declarer = root.GetBaseClass(classId);
					if (declarer == null)
					{
						throw new DesignerException("Base class not set for base method [{0},{1}]", classId, MemberId);
					}
					SetDeclarerDirect(declarer);
				}
				MethodOverride baseMethod = root.GetCustomMethodByBaseMethodId(classId, MemberId);
				if (baseMethod == null)
				{
					throw new DesignerException("Base method not found [{0},{1}]", classId, MemberId);
				}
				DataTypePointer[] dps = new DataTypePointer[baseMethod.ParameterCount];
				for (int i = 0; i < baseMethod.ParameterCount; i++)
				{
					dps[i] = baseMethod.Parameters[i];
				}
				ParametersMatch(dps);
				Parameters = baseMethod.Parameters;
			}
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
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			ParameterValueCollection.ValidateParameterValues(parameterValues, this.Parameters, this);
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

		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			MethodClassInherited mci = pointer as MethodClassInherited;
			if (mci != null)
			{
				return (mci.WholeId == this.WholeId);
			}
			return false;
		}

		#endregion

	}
}
