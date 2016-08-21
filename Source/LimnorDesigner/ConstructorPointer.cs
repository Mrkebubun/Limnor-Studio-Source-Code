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
using System.CodeDom;
using MathExp;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using Limnor.WebBuilder;
using TraceLog;

namespace LimnorDesigner
{
	/// <summary>
	/// points to a ConstructorInfo. a subclass of MethodInfo
	/// </summary>
	public class ConstructorPointer : MethodPointer, IConstructor
	{
		private ConstructorInfo _mif;
		private LocalVariable _returnReceiver;
		public ConstructorPointer()
		{
		}
		public ConstructorPointer(ConstructorInfo info, LocalVariable lv)
		{
			SetMethodInfo(info);
			_returnReceiver = lv;
		}
		public static CodeTypeReference CreateGenericTypeReference(string type, string[] typeStrParams)
		{
			CodeTypeReference[] typeParams = new CodeTypeReference[typeStrParams.Length];
			int index = 0;
			foreach (string str in typeStrParams)
			{
				typeParams[index++] = new CodeTypeReference(str);
			}

			return CreateGenericTypeReference(type, typeParams);
		}

		public static CodeTypeReference CreateGenericTypeReference(string type, CodeTypeReference[] typeArgs)
		{
			return new CodeTypeReference(type, typeArgs);
		}
		public static CodeTypeReference CreateGenericTypeReference(string type, Type[] typeArgs)
		{
			CodeTypeReference[] typeParams = new CodeTypeReference[typeArgs.Length];
			for (int i = 0; i < typeParams.Length; i++)
			{
				typeParams[i] = new CodeTypeReference(typeArgs[i]);
			}
			return new CodeTypeReference(type, typeParams);
		}
		public static CodeTypeReference CreateGenericTypeReference(Type type, Type[] typeArgs)
		{
			CodeTypeReference[] typeParams = new CodeTypeReference[typeArgs.Length];
			for (int i = 0; i < typeParams.Length; i++)
			{
				typeParams[i] = new CodeTypeReference(typeArgs[i]);
			}
			if (string.IsNullOrEmpty(type.AssemblyQualifiedName))
			{
				return new CodeTypeReference(type.Name, typeParams);
			}
			else
			{
				return new CodeTypeReference(type.AssemblyQualifiedName, typeParams);
			}
		}
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (_returnReceiver != null)
				{
					return _returnReceiver.RunAt;
				}
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public override bool IsForLocalAction
		{
			get
			{
				return true;
			}
		}
		public override bool CanBeReplacedInEditor
		{
			get
			{
				return false;
			}
		}
		public override object GetParameterTypeByIndex(int idx)
		{
			if (idx >= 0)
			{
				ConstructorInfo info = this.ConstructInfo;
				if (info != null)
				{
					ParameterInfo[] ps = ConstructInfo.GetParameters();
					if (ps != null && idx < ps.Length)
					{
						return ps[idx];
					}
				}
			}
			return null;
		}
		public override void SetParameterTypeByIndex(int idx, object type)
		{
		}
		public override object GetParameterType(UInt32 id)
		{
			ConstructorInfo info = this.ConstructInfo;
			if (info != null)
			{
				ParameterInfo[] ps = ConstructInfo.GetParameters();
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
		public override object GetParameterType(string name)
		{
			ConstructorInfo info = this.ConstructInfo;
			if (info != null)
			{
				ParameterInfo[] ps = ConstructInfo.GetParameters();
				if (ps != null)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						if (string.Compare(ps[i].Name, name, StringComparison.Ordinal) == 0)
						{
							return ps[i].ParameterType;
						}
					}
				}
			}
			return null;
		}
		public void CopyFrom(IConstructor icp)
		{
			ConstructorPointer cp = icp as ConstructorPointer;
			if (cp != null)
			{
				SetMethodInfo(cp.ConstructInfo);
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity obj)
		{
			ConstructorPointer mpp = obj as ConstructorPointer;
			if (mpp != null)
			{
				if (base.IsSameObjectRef(obj))
				{
					return DesignUtil.IsSameValueTypes(ParameterTypes, mpp.ParameterTypes);
				}
			}
			return false;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (ParameterCodeExpressions == null)
			{
				throw new DesignerException("ParameterCodeExpressions is null calling ConstructorPointer.GetReferenceCode");
			}
			CodeObjectCreateExpression oce;
			if (_returnReceiver != null)
			{
				oce = new CodeObjectCreateExpression(_returnReceiver.TypeString, ParameterCodeExpressions);
			}
			else
			{
				oce = new CodeObjectCreateExpression(ReturnType, ParameterCodeExpressions);
			}
			return oce;
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public override void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_returnReceiver != null)
			{
				if (_returnReceiver.BaseClassType.GetInterface("IJavascriptType") != null)
				{
					IJavascriptType js = Activator.CreateInstance(_returnReceiver.BaseClassType) as IJavascriptType;
					string v = js.GetJavascriptMethodRef(_returnReceiver.CodeName, ".ctor", sb, parameters);
					sb.Add(_returnReceiver.CodeName);
					sb.Add("=");
					sb.Add(v);
					sb.Add(";\r\n");
					return;
				}
			}
			if (_mif != null && _returnReceiver != null)
			{
				if (_mif.ReflectedType != null && _mif.ReflectedType.IsArray)
				{
					sb.Add(_returnReceiver.CodeName);
					sb.Add("=new Array();\r\n");
					if (parameters != null && parameters.Count > 0)
					{
						string idx = string.Format(CultureInfo.InvariantCulture, "i{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						sb.Add("for(var ");
						sb.Add(idx);
						sb.Add("=0;");
						sb.Add(idx);
						sb.Add("<");
						sb.Add(parameters[0]);
						sb.Add(";");
						sb.Add(idx);
						sb.Add("++) {\r\n");
						sb.Add(_returnReceiver.CodeName);
						sb.Add(".push(null);\r\n");
						sb.Add("}\r\n");
					}
				}
			}
		}
		public override void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_returnReceiver != null)
			{
				if (_returnReceiver.BaseClassType.GetInterface("IPhpType") != null)
				{
					IPhpType php = Activator.CreateInstance(_returnReceiver.BaseClassType) as IPhpType;
					string v = php.GetMethodRef(_returnReceiver.CodeName, ".ctor", sb, parameters);
					sb.Add(_returnReceiver.CodeName);
					sb.Add("=");
					sb.Add(v);
					sb.Add(";\r\n");
				}
			}
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public override void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			CodeExpression cmi;
			CodeExpression[] ps;
			if (parameters != null)
			{
				ps = new CodeExpression[parameters.Count];
				parameters.CopyTo(ps, 0);
			}
			else
			{
				ps = new CodeExpression[] { };
			}
			CodeExpression oce;
			if (_returnReceiver != null)
			{
				if (_returnReceiver.BaseClassType.IsArray)
				{
					if (ps.Length == 1)
					{
						oce = new CodeArrayCreateExpression(_returnReceiver.TypeString, ps[0]);
					}
					else
					{
						oce = new CodeArrayCreateExpression(_returnReceiver.TypeString, ps);
					}
				}
				else
				{
					//currently custom generic type is not supported
					oce = new CodeObjectCreateExpression(_returnReceiver.GetCodeTypeReference(), ps);
				}
			}
			else
			{
				oce = new CodeObjectCreateExpression(ReturnType, ps);
			}
			cmi = oce;
			CodeExpression cr = _returnReceiver.GetReferenceCode(methodToCompile, statements, true);
			CodeAssignStatement cas = new CodeAssignStatement(cr, cmi);
			statements.Add(cas);
		}
		[Browsable(false)]
		public LocalVariable ReturnReceiver
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
		public ConstructorInfo ConstructInfo
		{
			get
			{
				MethodBase mb = MethodDef;
				if (mb != null)
				{
					return mb as ConstructorInfo;
				}
				return null;
			}
		}
		[Browsable(false)]
		public override string MethodSignature
		{
			get
			{
				ParameterInfo[] ps = ConstructInfo.GetParameters();
				if (ps == null || ps.Length == 0)
					return "()";
				StringBuilder sb = new StringBuilder("(");
				for (int i = 0; i < ps.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(", ");
					}
					sb.Append(ps[i].ParameterType.Name);
					sb.Append(" ");
					sb.Append(ps[i].Name);
				}
				sb.Append(")");
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public override MethodBase MethodDef
		{
			get
			{
				if (_mif == null)
				{
					if (_returnReceiver != null)
					{
						Type t = _returnReceiver.ObjectType;
						if (t != null)
						{
							try
							{
								_mif = t.GetConstructor(this.GetParameterTypes());
							}
							catch (Exception err)
							{
								MathNode.Log(TraceLogClass.MainForm,err);
							}
							if (_mif == null)
							{
								Type tCo = VPLUtil.GetCoClassType(t);
								if (tCo != null)
								{
									_mif = tCo.GetConstructor(this.GetParameterTypes());
								}
							}
							if (_mif != null)
							{
								SetMethodInfo(_mif);
							}
						}
					}
					else
					{
						DesignUtil.WriteToOutputWindowAndLog("Constructor type not set for ConstructorPointer {0}", this.MethodID);
					}
				}
				return _mif;
			}
		}
		[Browsable(false)]
		public override Type ActionBranchType
		{
			get
			{
				return typeof(AB_Constructor);
			}
		}
		[Browsable(false)]
		public override bool NoReturn { get { return true; } }
		[Browsable(false)]
		public override bool HasReturn
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public override Type ReturnType
		{
			get
			{
				if (MethodDef != null)
				{
					ConstructorInfo ci = MethodDef as ConstructorInfo;
					return ci.DeclaringType;
				}
				if (_returnReceiver != null)
				{
					return _returnReceiver.ObjectType;
				}
				return null;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override IObjectPointer Owner
		{
			get
			{
				return _returnReceiver;
			}
			set
			{
				_returnReceiver = value as LocalVariable;
			}
		}
		protected override void OnSetMethodDef(MethodBase method)
		{
			_mif = (ConstructorInfo)method;
		}
		public IList<NamedDataType> GetParameters()
		{
			List<NamedDataType> ps = new List<NamedDataType>();
			ConstructorInfo ci = ConstructInfo;
			if (ci != null)
			{
				ParameterInfo[] pifs = ci.GetParameters();
				if (pifs != null)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						NamedDataType nt = new NamedDataType(pifs[i].ParameterType, pifs[i].Name);
						ps.Add(nt);
					}
				}
			}
			return ps;
		}
	}
	public class ConstructorPointerNull : ConstructorPointer
	{
		public ConstructorPointerNull()
		{
		}
		public ConstructorPointerNull(LocalVariable lv)
			: base(null, lv)
		{
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodePrimitiveExpression(null);
		}
		[Browsable(false)]
		public override string MethodSignature
		{
			get
			{
				return "null";
			}
		}
		public override MethodBase MethodDef
		{
			get
			{
				return null;
			}
		}
		protected override void OnSetMethodDef(MethodBase method)
		{
		}
		public override bool IsSameObjectRef(IObjectIdentity obj)
		{
			return (obj is ConstructorPointerNull);
		}
	}
}
