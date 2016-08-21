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
using System.Reflection;
using System.CodeDom;
using MathExp;
using ProgElements;
using LimnorDesigner.Action;
using System.Globalization;
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// for creating an action which is a sub-method
	/// </summary>
	public abstract class SubMethodInfo : MethodInfo
	{
		public const string ExecuteForEachItem = "ExecuteForEachItem";
		public SubMethodInfo()
		{
		}
		public abstract CodeStatement GetInitStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId);
		public abstract CodeExpression GetTestExpression(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId);
		public abstract CodeStatement GetIncrementalStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId);
		public abstract CodeExpression[] GetIndexCodes(MethodInfoPointer owner, IMethodCompile method, UInt32 branchId);
		//
		public abstract string GetInitStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string GetTestExpressionJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string GetIncrementalStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string[] GetIndexCodesJS(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId);
		public abstract string GetIndexCodeJS(MethodInfoPointer owner, UInt32 branchId);
		public abstract string GetIndexCodePHP(MethodInfoPointer owner, UInt32 branchId);
		public abstract string GetValueCodePHP(MethodInfoPointer owner, UInt32 branchId);
		//
		public abstract string GetInitStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string GetTestExpressionPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string GetIncrementalStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId);
		public abstract string[] GetIndexCodesPhp(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId);
		//
		public abstract ParameterClassSubMethod GetParameterType(UInt32 id, SubMethodInfoPointer owner, AB_SubMethodAction action);
		public abstract Dictionary<string, string> IndexCodeNames { get; }
		public abstract bool SaveParameterValues { get; }
		public abstract bool IsForeach { get; }
		public abstract Type ItemType { get; }
		public override MethodInfo GetBaseDefinition()
		{
			return null;
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get { return null; }
		}

		public override MethodAttributes Attributes
		{
			get { return MethodAttributes.Public; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Managed;
		}
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			return null;
		}
		private RuntimeMethodHandle _rmh = default(RuntimeMethodHandle);
		public override RuntimeMethodHandle MethodHandle
		{
			get { return _rmh; }
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return null;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

	}
	/// <summary>
	/// for parameters of SubMethod.
	/// for "foreach" method of an array, there are two parameters: index (_type=int) and value (_type=item type)
	/// </summary>
	public class SubMethodParameterInfo : ParameterInfo
	{
		private string _name; //parameter name
		private Type _type; //parameter type
		private string _codeName; //method name?
		private string _desc;
		public SubMethodParameterInfo(string name, Type type, string codeName, string desc)
		{
			_name = name;
			_type = type;
			_codeName = codeName;
			_desc = desc;
		}
		public string Description
		{
			get
			{
				return _desc;
			}
		}
		public override string Name
		{
			get
			{
				return _name;
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _type;
			}
		}
		public override int GetHashCode()
		{
			return CodeName.GetHashCode();
		}
		public void InitCodeName(string codeName)
		{
			_codeName = codeName;
		}
		public string CodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"${0}_{1}", _name, _codeName);
				}
				else
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}_{1}", _name, _codeName);
				}
			}
		}
	}
	/// <summary>
	/// MethodInfo for "foreach" as a method
	/// </summary>
	public class ArrayForEachMethodInfo : SubMethodInfo
	{
		private Type _itemType;
		private ParameterInfo[] _parameters;
		private string _methodKey;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemType">type of each item</param>
		/// <param name="methodKey">method id (codeName, name of the method?)</param>
		public ArrayForEachMethodInfo(Type itemType, string methodKey)
		{
			_itemType = itemType;
			_methodKey = methodKey;
		}
		public override Type ReflectedType
		{
			get
			{
				string s = _itemType.AssemblyQualifiedName;
				int n = s.IndexOf(',');
				s = string.Format(CultureInfo.InvariantCulture, "{0}[]{1}",
					s.Substring(0, n), s.Substring(n));
				return Type.GetType(s);
			}
		}
		private string codeName
		{
			get
			{
				return _methodKey;
			}
		}
		public override Type ItemType { get { return _itemType; } }
		public override bool IsForeach { get { return false; } }
		public override bool SaveParameterValues { get { return false; } }
		/// <summary>
		/// parameters of the method
		/// </summary>
		/// <returns></returns>
		public override ParameterInfo[] GetParameters()
		{
			if (_parameters == null)
			{
				_parameters = new ParameterInfo[2];
				_parameters[0] = new SubMethodParameterInfo("index", typeof(int), codeName, "Index for the current array/list item, starting from 0");
				_parameters[1] = new SubMethodParameterInfo("value", _itemType, codeName, "Current array/list item");
			}
			return _parameters;
		}

		public override string Name
		{
			get { return SubMethodInfo.ExecuteForEachItem; }
		}
		public override Type ReturnType
		{
			get
			{
				return typeof(void);
			}
		}
		public override Type DeclaringType
		{
			get { return typeof(Array); }
		}

		public override Dictionary<string, string> IndexCodeNames
		{
			get
			{
				//only one index: one-dimentional
				ParameterInfo[] ps = GetParameters();
				SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
				Dictionary<string, string> d = new Dictionary<string, string>();
				d.Add("index", p.CodeName);
				return d;
			}
		}
		public override ParameterClassSubMethod GetParameterType(UInt32 id, SubMethodInfoPointer owner, AB_SubMethodAction action)
		{
			ParameterClassSubMethod p;
			ParameterInfo[] ps = GetParameters();
			int i;
			string s = ((SubMethodParameterInfo)ps[0]).CodeName;
			UInt32 id0;
			id0 = (UInt32)(s.GetHashCode());
			if (id0 == id)
				i = 0;
			else
			{
				s = ((SubMethodParameterInfo)ps[1]).CodeName;
				id0 = (UInt32)(s.GetHashCode());
				if (id0 == id)
				{
					i = 1;
				}
				else
				{
					i = (int)id;
					if (i < 0 || i >= ps.Length)
					{
						i = 1;
					}
				}
			}
			if (i == 0)
				p = new ParameterClassArrayIndex(ps[i].ParameterType, ps[i].Name, action);
			else
				p = new ParameterClassArrayItem(ps[i].ParameterType, ps[i].Name, action);
			p.ParameterID = id;
			p.Method = owner;
			p.ActionId = owner.ActionOwner.ActionId;
			return p;
		}
		/// <summary>
		/// indices for accessing array item
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public override CodeExpression[] GetIndexCodes(MethodInfoPointer owner, IMethodCompile method, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			CodeExpression[] pss = new CodeExpression[1];
			pss[0] = new CodeVariableReferenceExpression(codeName2(p.CodeName, branchId));
			return pss;
		}
		public override string[] GetIndexCodesJS(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string[] pss = new string[1];
			pss[0] = codeName2(p.CodeName, branchId);
			return pss;
		}
		public override string GetIndexCodeJS(MethodInfoPointer owner, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			return codeName2(p.CodeName, branchId);
		}
		public override string[] GetIndexCodesPhp(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string[] pss = new string[1];
			pss[0] = codeName2(p.CodeName, branchId);
			return pss;
		}
		public override string GetIndexCodePHP(MethodInfoPointer owner, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			return codeName2(p.CodeName, branchId);
		}
		public override string GetValueCodePHP(MethodInfoPointer owner, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[1];
			return codeName2(p.CodeName, branchId);
		}
		public override CodeStatement GetInitStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			CodeVariableDeclarationStatement vd = new CodeVariableDeclarationStatement(typeof(int), codeName2(p.CodeName, branchId), new CodePrimitiveExpression(0));
			return vd;
		}
		public override string GetInitStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string vd = string.Format(CultureInfo.InvariantCulture,
				"{0}=0;", codeName2(p.CodeName, branchId));
			return vd;
		}
		public override string GetInitStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string vd = string.Format(CultureInfo.InvariantCulture,
				"{0}=0;", codeName2(p.CodeName, branchId));
			return vd;
		}
		public override CodeExpression GetTestExpression(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			CodeBinaryOperatorExpression bin = new CodeBinaryOperatorExpression(
				new CodeVariableReferenceExpression(codeName2(p.CodeName, branchId)),
				 CodeBinaryOperatorType.LessThan,
				 new CodePropertyReferenceExpression(owner.Owner.GetReferenceCode(methodToCompile, statements, true), "Length"));
			return bin;
		}
		public override string GetTestExpressionJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string bin = string.Format(CultureInfo.InvariantCulture,
				"{0} < {1}.length;", codeName2(p.CodeName, branchId), owner.Owner.GetJavaScriptReferenceCode(jsCode));
			return bin;
		}
		public override string GetTestExpressionPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string bin = string.Format(CultureInfo.InvariantCulture,
				"{0} < count({1});", codeName2(p.CodeName, branchId), owner.Owner.GetPhpScriptReferenceCode(jsCode));
			return bin;
		}
		public override CodeStatement GetIncrementalStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			CodeExpressionStatement incr = new CodeExpressionStatement(new CodeSnippetExpression(codeName2(p.CodeName, branchId) + "++"));
			return incr;
		}
		public override string GetIncrementalStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string incr = string.Format(CultureInfo.InvariantCulture,
				"{0}++", codeName2(p.CodeName, branchId));
			return incr;
		}
		public override string GetIncrementalStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			string incr = string.Format(CultureInfo.InvariantCulture,
				"{0}++", codeName2(p.CodeName, branchId));
			return incr;
		}
		internal static string codeName2(string name, UInt32 branchId)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}_{2}", MathNode.VariableNamePrefix, name, branchId.ToString("x", CultureInfo.InvariantCulture));
		}
	}
}
