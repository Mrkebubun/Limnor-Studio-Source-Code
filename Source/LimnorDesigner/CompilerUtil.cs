/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using VPL;
using System.ComponentModel;
using LimnorDesigner.Action;
using ProgElements;
using System.Windows.Forms;
using LimnorDesigner.Interface;
using System.Web.Services;
using System.Globalization;
using System.ServiceModel;
using Limnor.WebBuilder;
using Limnor.PhpComponents;
using WindowsUtility;
using System.Reflection;
using Limnor.Drawing2D;
using Limnor.Reporting;

namespace LimnorDesigner
{
	public static class CompilerUtil
	{
		public const string InvokeStart = "this.Invoke((MethodInvoker)(delegate(){";//main-thread call
		public const string InvokeEnd = "}));";
		const string GOTOBRANCHES = "GotoBranches";
		const string GOTOBRANCHES_Group = "GotoBranchesInGroup";
		const string SUBMETHODBRANCHES = "SubMethodBranches";
		const string RETURNTYPE = "ReturnType";
		const string PROP_Namespace = "Namespace";

		public static CodeExpression GetDrawItemExpression(DrawingControl dc)
		{
			if (dc != null)
			{
				DrawingItem di = dc.Item as DrawingItem;
				if (di != null)
				{
					DrawDataRepeater ddp = di.Container as DrawDataRepeater;
					if (ddp != null)
					{
						//((item type)<repeater name>[<drawing item name>])
						TypeMappingAttribute tma = null;
						object[] vs = dc.GetType().GetCustomAttributes(typeof(TypeMappingAttribute), true);
						if (vs != null && vs.Length > 0)
						{
							tma = vs[0] as TypeMappingAttribute;
						}
						if (tma != null)
						{
							CodeFieldReferenceExpression repeater = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ddp.Name);
							CodeIndexerExpression cie = new CodeIndexerExpression(repeater, new CodePrimitiveExpression(dc.Name));
							CodeCastExpression cce = new CodeCastExpression(tma.MappedType, cie);
							return cce;
						}
					}
				}
			}
			return null;
		}
		//
		public static string CreateJsCodeName(IObjectPointer op, UInt32 taskId)
		{
			return CreateJsCodeName((uint)(op.ObjectKey.GetHashCode()), taskId);
		}
		public static string CreateJsCodeName(IObjectPointer op, string name)
		{
			string s = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", op.ObjectKey, name);
			return CompilerUtil.CreateJsCodeName((UInt32)s.GetHashCode(), 0);
		}
		public static string CreateJsCodeName(UInt32 id, UInt32 taskId)
		{
			return string.Format(CultureInfo.InvariantCulture, "j{0}", id.ToString("x", CultureInfo.InvariantCulture));
		}
		public static void AddAttribute(IAttributeHolder holder, CodeAttributeDeclarationCollection attributs, bool isWebService, bool includeOperationContract, string namespaceText)
		{
			List<ConstObjectPointer> list = holder.GetCustomAttributeList();
			if (list != null && list.Count > 0)
			{
				foreach (ConstObjectPointer cop in list)
				{
					if (!includeOperationContract)
					{
						if (typeof(OperationContractAttribute).IsAssignableFrom(cop.BaseClassType))
						{
							continue;
						}
					}
					if (!isWebService)
					{
						if (typeof(WebServiceAttribute).Equals(cop.BaseClassType))
						{
							isWebService = true;
						}
					}
					CodeAttributeArgument[] aa = new CodeAttributeArgument[cop.ParameterCount];
					for (int i = 0; i < cop.ParameterCount; i++)
					{
						aa[i] = new CodeAttributeArgument(cop.GetParameterCode(null, null, i));
					}
					CodeAttributeDeclaration a = new CodeAttributeDeclaration(
						cop.TypeString, aa);
					if (cop.ParameterCount == 0)
					{
						AttributeConstructor ac = cop.Value as AttributeConstructor;
						if (ac != null && ac.Values != null)
						{

							IList<CodeAttributeArgument> lst = ac.CreatePropertyExpressions();
							foreach (CodeAttributeArgument caa in lst)
							{
								a.Arguments.Add(caa);
							}

						}
						bool isInterface = (holder is InterfaceClass);
						bool isServiceContract = cop.BaseClassType.FullName.StartsWith("System.ServiceModel.ServiceContractAttribute", StringComparison.Ordinal);
						if (isInterface && isServiceContract)
						{
							bool hasNamespace = false;
							if (ac != null && ac.Values != null)
							{
								if (string.IsNullOrEmpty(ac.Values[PROP_Namespace] as string))
								{
								}
								else
								{
									hasNamespace = true;
								}
							}
							if (!hasNamespace)
							{
								a.Arguments.Add(new CodeAttributeArgument(PROP_Namespace, new CodePrimitiveExpression(string.Format(CultureInfo.InvariantCulture, "http://{0}", namespaceText))));
							}
						}
					}

					attributs.Add(a);
				}
			}
		}
		public static Dictionary<UInt32, MethodSegment> AddSubMethod(CodeMemberMethod method, IMethod0 subMethod)
		{
			Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>> subbranches = (Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>>)method.UserData[SUBMETHODBRANCHES];
			if (subbranches == null)
			{
				subbranches = new Dictionary<IMethod0, Dictionary<uint, MethodSegment>>();
				method.UserData.Add(SUBMETHODBRANCHES, subbranches);
			}
			Dictionary<UInt32, MethodSegment> branches;
			if (!subbranches.TryGetValue(subMethod, out branches))
			{
				branches = new Dictionary<uint, MethodSegment>();
				subbranches.Add(subMethod, branches);
			}
			return branches;
		}
		public static void RemoveSubMethod(CodeMemberMethod method, IMethod0 subMethod)
		{
			Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>> subbranches = (Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>>)method.UserData[SUBMETHODBRANCHES];
			if (subbranches != null)
			{
				if (subbranches.ContainsKey(subMethod))
				{
					subbranches.Remove(subMethod);
				}
			}
		}
		public static bool FinishSubMethod(CodeMemberMethod method, IMethod0 subMethod, CodeStatementCollection statements, bool completed)
		{
			Dictionary<UInt32, MethodSegment> gotoBranches = GetGotoBranches(method, subMethod);
			RemoveSubMethod(method, subMethod);
			if (gotoBranches != null && gotoBranches.Count > 0)
			{
				string lb = "L_" + Guid.NewGuid().GetHashCode().ToString("x");
				bool bUseLabel = false;
				if (!completed)
				{
					//if the last method code is not a return then add a default method return
					//if at least one of the branches in the method (not including goBranches) is not ended (method return or go to)
					bUseLabel = true;
					statements.Add(new CodeGotoStatement(lb));
				}
				foreach (KeyValuePair<UInt32, MethodSegment> kv in gotoBranches)
				{
					//if the last statement for this gotoBranch is not a method return then add a default one
					if (!kv.Value.Completed)
					{
						bUseLabel = true;
						kv.Value.Statements.Add(new CodeGotoStatement(lb));
					}
					//append the label to the end of method code
					statements.Add(new CodeLabeledStatement(ActionBranch.IdToLabel(kv.Key)));
					if (kv.Value.Statements.Count == 0)
					{
						statements.Add(new CodeSnippetStatement(";"));
					}
					//append the staments belonging to this gotoBranch to the end of the method
					for (int i = 0; i < kv.Value.Statements.Count; i++)
					{
						statements.Add(kv.Value.Statements[i]);
					}
				}
				if (bUseLabel)
				{
					statements.Add(new CodeLabeledStatement(lb));
					statements.Add(new CodeSnippetStatement(";"));
				}
			}
			return completed;
		}
		public static void ClearGroupGotoBranches(CodeMemberMethod method, UInt32 groupId)
		{
			if (method.UserData.Contains(GOTOBRANCHES_Group))
			{
				Dictionary<UInt32, Dictionary<UInt32, MethodSegment>> groups = (Dictionary<UInt32, Dictionary<UInt32, MethodSegment>>)method.UserData[GOTOBRANCHES_Group];
				if (groups != null)
				{
					if (groups.ContainsKey(groupId))
					{
						groups.Remove(groupId);
						if (groups.Count == 0)
						{
							method.UserData.Remove(GOTOBRANCHES_Group);
						}
					}
				}
			}
		}
		public static bool FinishActionGroup(CodeMemberMethod method, CodeStatementCollection statements, UInt32 groupId, bool completed)
		{
			if (method.UserData.Contains(GOTOBRANCHES_Group))
			{
				Dictionary<UInt32, Dictionary<UInt32, MethodSegment>> groups = (Dictionary<UInt32, Dictionary<UInt32, MethodSegment>>)method.UserData[GOTOBRANCHES_Group];
				if (groups != null)
				{
					Dictionary<UInt32, MethodSegment> gotoBranches;
					if (groups.TryGetValue(groupId, out gotoBranches))
					{
						if (gotoBranches != null && gotoBranches.Count > 0)
						{
							string lb = "L_" + Guid.NewGuid().GetHashCode().ToString("x");
							bool bUseLabel = false;
							if (!completed)
							{
								//if the last method code is not a return then add a default method return
								//if at least one of the branches in the method (not including goBranches) is not ended (method return or go to)
								//addDefaultMethodReturn(mc, mm.Statements);
								bUseLabel = true;
								statements.Add(new CodeGotoStatement(lb));
							}
							foreach (KeyValuePair<UInt32, MethodSegment> kv in gotoBranches)
							{
								//if the last statement for this gotoBranch is not a method return then add a default one
								if (!kv.Value.Completed)
								{
									bUseLabel = true;
									kv.Value.Statements.Add(new CodeGotoStatement(lb));
								}
								//append the label to the end of method code
								statements.Add(new CodeLabeledStatement(ActionBranch.IdToLabel(kv.Key)));
								if (kv.Value.Statements.Count == 0)
								{
									statements.Add(new CodeSnippetStatement(";"));
								}
								//append the staments belonging to this gotoBranch to the end of the method
								for (int i = 0; i < kv.Value.Statements.Count; i++)
								{
									statements.Add(kv.Value.Statements[i]);
								}
							}
							if (bUseLabel)
							{
								statements.Add(new CodeLabeledStatement(lb));
								statements.Add(new CodeSnippetStatement(";"));
							}
						}
					}
				}
			}
			return completed;
		}
		/// <summary>
		/// if currently in submethod then add goto branch to the sub method; otherwise add it to the method scope
		/// </summary>
		/// <param name="method"></param>
		/// <param name="custMethod"></param>
		/// <param name="branchId"></param>
		/// <param name="statements"></param>
		public static void AddGotoBranch(CodeMemberMethod method, MethodClass custMethod, UInt32 branchId, MethodSegment statements, UInt32 groupId)
		{
			Dictionary<UInt32, MethodSegment> branches = null;
			if (groupId != 0)
			{
				Dictionary<UInt32, Dictionary<UInt32, MethodSegment>> groups;
				if (method.UserData.Contains(GOTOBRANCHES_Group))
				{
					groups = (Dictionary<UInt32, Dictionary<UInt32, MethodSegment>>)method.UserData[GOTOBRANCHES_Group];
					if (groups == null)
					{
						groups = new Dictionary<uint, Dictionary<uint, MethodSegment>>();
						method.UserData[GOTOBRANCHES_Group] = groups;
					}
				}
				else
				{
					groups = new Dictionary<uint, Dictionary<uint, MethodSegment>>();
					method.UserData.Add(GOTOBRANCHES_Group, groups);
				}
				if (!groups.TryGetValue(groupId, out branches))
				{
					branches = new Dictionary<uint, MethodSegment>();
					groups.Add(groupId, branches);
				}
			}
			else
			{
				if (custMethod.SubMethod.Count > 0)
				{
					IMethod0 m0 = custMethod.SubMethod.Peek();
					branches = AddSubMethod(method, m0);
				}
				else
				{
					branches = (Dictionary<UInt32, MethodSegment>)method.UserData[GOTOBRANCHES];
					if (branches == null)
					{
						branches = new Dictionary<uint, MethodSegment>();
						method.UserData.Add(GOTOBRANCHES, branches);
					}
				}
			}
			if (!branches.ContainsKey(branchId))
			{
				branches.Add(branchId, statements);
			}
		}
		/// <summary>
		/// get goto branches in the method scope
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static Dictionary<UInt32, MethodSegment> GetGotoBranches(CodeMemberMethod method)
		{
			if (method.UserData.Contains(GOTOBRANCHES))
			{
				return (Dictionary<UInt32, MethodSegment>)method.UserData[GOTOBRANCHES];
			}
			return null;
		}
		/// <summary>
		/// get goto branches in sub-method scope
		/// </summary>
		/// <param name="method"></param>
		/// <param name="subMethod"></param>
		/// <returns></returns>
		public static Dictionary<UInt32, MethodSegment> GetGotoBranches(CodeMemberMethod method, IMethod0 subMethod)
		{
			Dictionary<UInt32, MethodSegment> branches = null;
			Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>> subbranches = (Dictionary<IMethod0, Dictionary<UInt32, MethodSegment>>)method.UserData[SUBMETHODBRANCHES];
			if (subbranches != null)
			{
				subbranches.TryGetValue(subMethod, out branches);
			}
			return branches;
		}
		/// <summary>
		/// get the goto branch in sub-method by branch Id
		/// </summary>
		/// <param name="method"></param>
		/// <param name="custMethod"></param>
		/// <param name="branchId"></param>
		/// <returns></returns>
		public static MethodSegment GetGotoBranch(CodeMemberMethod method, MethodClass custMethod, UInt32 branchId)
		{
			Dictionary<UInt32, MethodSegment> list = null;
			if (custMethod.SubMethod.Count > 0)
			{
				IMethod0 m0 = custMethod.SubMethod.Peek();
				list = AddSubMethod(method, m0);
			}
			else
			{
				if (method.UserData.Contains(GOTOBRANCHES))
				{
					list = (Dictionary<UInt32, MethodSegment>)method.UserData[GOTOBRANCHES];
				}
			}
			if (list != null)
			{
				MethodSegment ms;
				if (list.TryGetValue(branchId, out ms))
				{
					return ms;
				}
			}
			return null;
		}

		public static void SetReturnType(CodeMemberMethod method, DataTypePointer type)
		{
			if (!method.UserData.Contains(RETURNTYPE))
			{
				method.UserData.Add(RETURNTYPE, type);
			}
		}
		public static DataTypePointer GetReturnType(CodeMemberMethod method)
		{
			return (DataTypePointer)method.UserData[RETURNTYPE];
		}
		public static void AddVariableDeclaration(Type type, string name, CodeStatementCollection statements)
		{
			for (int i = 0; i < statements.Count; i++)
			{
				CodeVariableDeclarationStatement vd = statements[i] as CodeVariableDeclarationStatement;
				if (vd != null)
				{
					if (vd.Name == name)
					{
						return;
					}
				}
			}
			CodeVariableDeclarationStatement p;
			if (type.IsValueType)
			{
				p = new CodeVariableDeclarationStatement(type, name);
			}
			else
			{
				p = new CodeVariableDeclarationStatement(type, name, ObjectCreationCodeGen.GetDefaultValueExpression(type));
			}
			statements.Insert(0, p);
		}
		public static void AddVariableDeclaration(string type, string name, CodeStatementCollection statements)
		{
			for (int i = 0; i < statements.Count; i++)
			{
				CodeVariableDeclarationStatement vd = statements[i] as CodeVariableDeclarationStatement;
				if (vd != null)
				{
					if (vd.Name == name)
					{
						return;
					}
				}
			}
			statements.Insert(0, new CodeVariableDeclarationStatement(type, name));
		}
		public static bool IsPrimaryExp(CodeExpression ce)
		{
			if (ce != null)
			{
				if (ce is CodePrimitiveExpression)
					return true;
				CodeCastExpression cce = ce as CodeCastExpression;
				if (cce != null)
				{
					return IsPrimaryExp(cce.Expression);
				}
			}
			return false;
		}
		public static bool VariableDeclared(string name, CodeStatementCollection statements)
		{
			for (int i = 0; i < statements.Count; i++)
			{
				CodeVariableDeclarationStatement vd = statements[i] as CodeVariableDeclarationStatement;
				if (vd != null)
				{
					if (vd.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static CodeVariableDeclarationStatement CreateVariableDeclaration(string typestring, string name, Type t, object init)
		{
			CodeVariableDeclarationStatement p;
			if (init == null)
			{
				p = new CodeVariableDeclarationStatement(typestring, name, ObjectCreationCodeGen.GetDefaultValueExpression(t));
			}
			else
			{
				p = new CodeVariableDeclarationStatement(typestring, name, ObjectCreationCodeGen.ObjectCreationCode(init));
			}
			return p;
		}
		public static CodeVariableDeclarationStatement CreateVariableDeclaration(CodeTypeReference type, string name, Type t, object init)
		{
			CodeVariableDeclarationStatement p;
			if (init == null)
			{
				p = new CodeVariableDeclarationStatement(type, name, ObjectCreationCodeGen.GetDefaultValueExpression(t));
			}
			else
			{
				p = new CodeVariableDeclarationStatement(type, name, ObjectCreationCodeGen.ObjectCreationCode(init));
			}
			return p;
		}
		
		public static void CreateArrayVariable(CodeStatementCollection sts, Array ar, string elementType, string typeName, string name)
		{
			int[] dimensions = new int[ar.Rank];
			for (int i = 0; i < ar.Rank; i++)
			{
				dimensions[i] = ar.GetUpperBound(i) + 1;
			}
			CreateArrayVariable(sts, dimensions, elementType, typeName, name);
		}
		public static void CreateArrayVariable(CodeStatementCollection sts, int[] dimensions, string elementType, string typeName, string name)
		{
			CodeVariableDeclarationStatement vs = new CodeVariableDeclarationStatement(new CodeTypeReference(new CodeTypeReference(elementType), dimensions.Length), name);
			sts.Add(vs);
			vs.InitExpression = CreateArrayCreationCode(dimensions, elementType, typeName);
		}
		public static CodeMethodInvokeExpression GetWebRequestValue(string valuename)
		{
			return new CodeMethodInvokeExpression(
			   new CodeVariableReferenceExpression("clientRequest"), "GetStringValue", new CodePrimitiveExpression(valuename));
		}
		public static CodeExpression CreateArrayCreationCode(int[] dimensions, string elementType, string typeName)
		{
			if (dimensions.Length == 1)
			{
				return new CodeArrayCreateExpression(elementType, dimensions[0]);
			}
			else
			{
				return new CodeSnippetExpression(CreateArrayCreationCodeString(dimensions, typeName));
			}
		}
		public static string CreateArrayCreationCodeString(int[] dimensions, string typeName)
		{
			StringBuilder sb = new StringBuilder("new ");
			sb.Append(typeName);
			sb.Append("[");
			for (int i = 0; i < dimensions.Length; i++)
			{
				if (i > 0)
					sb.Append(",");
				sb.Append(dimensions[i].ToString());
			}
			sb.Append("]");
			return sb.ToString();
		}
		public static string CreateArrayCreationCodeString(int rank, string typeName)
		{
			StringBuilder sb = new StringBuilder("new ");
			sb.Append(typeName);
			sb.Append("[");
			if (rank > 1)
			{
				sb.Append(new string(',', rank - 1));
			}
			sb.Append("]");
			return sb.ToString();
		}
		public static CodeExpression ConvertToBool(Type t, CodeExpression ce)
		{
			if (t.Equals(typeof(bool)))
			{
				return ce;
			}
			if (t.Equals(typeof(string)))
			{
				CodeBinaryOperatorExpression ret = new CodeBinaryOperatorExpression();
				ret.Operator = CodeBinaryOperatorType.BooleanOr;
				CodeBinaryOperatorExpression b = new CodeBinaryOperatorExpression();
				b.Operator = CodeBinaryOperatorType.IdentityEquality;
				b.Right = new CodePrimitiveExpression(0);
				CodeMethodInvokeExpression mi = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare"),
					ce, new CodePrimitiveExpression("true"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(StringComparison)), "OrdinalIgnoreCase"));
				b.Left = mi;
				ret.Left = b;
				//
				b = new CodeBinaryOperatorExpression();
				b.Operator = CodeBinaryOperatorType.IdentityEquality;
				b.Right = new CodePrimitiveExpression(0);
				mi = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare"),
					ce, new CodePrimitiveExpression("on"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(StringComparison)), "OrdinalIgnoreCase"));
				b.Left = mi;
				ret.Right = b;
				return ret;
			}
			if (typeof(object).Equals(t) || typeof(byte).Equals(t) 
				|| typeof(char).Equals(t) || typeof(DateTime).Equals(t) 
				|| typeof(decimal).Equals(t) || typeof(double).Equals(t) 
				|| typeof(float).Equals(t) || typeof(int).Equals(t) 
				|| typeof(long).Equals(t) || typeof(sbyte).Equals(t) 
				|| typeof(short).Equals(t) || typeof(uint).Equals(t) 
				|| typeof(ulong).Equals(t) || typeof(ushort).Equals(t) 
				)
			{
				CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression();
				mie.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Convert)),"ToBoolean");
				mie.Parameters.Add(ce);
				if (IsPrimaryExp(ce))
				{
					return mie;
				}
				CodeBinaryOperatorExpression bo = new CodeBinaryOperatorExpression();
				bo.Operator = CodeBinaryOperatorType.BooleanAnd;
				bo.Left = new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value"));
				bo.Right = mie;
				return bo;
			}
			CodeExpression def = ObjectCreationCodeGen.GetDefaultValueExpression(t);
			return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityInequality, def);
		}
		public static CodeExpression GetTypeConversion(DataTypePointer targetType, CodeExpression data, DataTypePointer dataType, CodeStatementCollection statements)
		{
			if (targetType.IsLibType && typeof(Type).Equals(targetType.BaseClassType))
			{
				if (data is CodeTypeOfExpression)
				{
					return data;
				}
			}
			if (targetType.IsAssignableFrom(dataType))
			{
				return data;
			}
			if (typeof(string).Equals(targetType.BaseClassType))
			{
				if (typeof(JsString).Equals(dataType.BaseClassType))
				{
					return data;
				}
				if (typeof(PhpString).Equals(dataType.BaseClassType))
				{
					return data;
				}
				if (dataType.IsValueType)
				{
					return new CodeMethodInvokeExpression(data, "ToString");
				}
				else
				{
					string vn = "l" + Guid.NewGuid().GetHashCode().ToString("x");
					string vs = "l" + Guid.NewGuid().GetHashCode().ToString("x");
					CodeExpression val;
					if (data is CodePropertyReferenceExpression || data is CodeFieldReferenceExpression || data is CodeVariableReferenceExpression)
					{
						val = data;
					}
					else
					{
						CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(dataType.GetCodeTypeReference(), vn, data);
						statements.Add(vds);
						val = new CodeVariableReferenceExpression(vn);
					}
					CodeVariableDeclarationStatement vds2 = new CodeVariableDeclarationStatement(typeof(string), vs);
					statements.Add(vds2);
					CodeVariableReferenceExpression re = new CodeVariableReferenceExpression(vs);
					CodeConditionStatement ccs = new CodeConditionStatement(
						new CodeBinaryOperatorExpression(
							val, CodeBinaryOperatorType.IdentityInequality,
							new CodePrimitiveExpression(null)),
						new CodeStatement[] { new CodeAssignStatement(re, new CodeMethodInvokeExpression(val, "ToString")) },
						new CodeStatement[] { new CodeAssignStatement(re, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Empty")) });
					statements.Add(ccs);
					return re;
				}
			}
			if (typeof(bool).Equals(targetType.BaseClassType))
			{
				return ConvertToBool(dataType.BaseClassType, data);
			}
			if ((targetType.BaseClassType.IsPrimitive || targetType.BaseClassType.GetInterface("IConvertible") != null)
				&& (dataType.BaseClassType.IsPrimitive || dataType.BaseClassType.GetInterface("IConvertible") != null))
			{
				CodeExpression ex = VPLUtil.ConvertByType(targetType.BaseClassType, data);
				if (ex != null)
				{
					return ex;
				}
			}
			if (targetType.BaseClassType.IsArray)
			{
				if (!dataType.BaseClassType.IsArray && !typeof(Array).Equals(dataType.BaseClassType))
				{
					Type elementType;
					if (targetType.BaseClassType.HasElementType)
					{
						elementType = targetType.BaseClassType.GetElementType();
					}
					else
					{
						elementType = typeof(object);
					}
					CodeArrayCreateExpression ae = new CodeArrayCreateExpression(elementType, new CodeExpression[] { GetTypeConversion(new DataTypePointer(elementType), data, dataType, statements) });
					return ae;
				}
			}
			TypeConverter converter = TypeDescriptor.GetConverter(targetType);
			if (converter != null)
			{
				if (converter.CanConvertFrom(dataType.BaseClassType))
				{
					string vn = "c" + Guid.NewGuid().GetHashCode().ToString("x");
					CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(typeof(TypeConverter), vn,
						new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter", new CodeTypeOfExpression(targetType.GetCodeTypeReference())));
					statements.Add(vds);
					string vn2 = "c" + Guid.NewGuid().GetHashCode().ToString("x");
					CodeVariableDeclarationStatement vds2 = new CodeVariableDeclarationStatement(typeof(TypeConverterNullable), vn2,
						new CodeObjectCreateExpression(typeof(TypeConverterNullable), new CodeVariableReferenceExpression(vn)));
					statements.Add(vds2);
					return new CodeCastExpression(targetType.GetCodeTypeReference(),
						new CodeMethodInvokeExpression(
							new CodeVariableReferenceExpression(vn2), "ConvertFrom", data));
				}
			}
			converter = TypeDescriptor.GetConverter(dataType);
			if (converter != null)
			{
				if (converter.CanConvertTo(targetType.BaseClassType))
				{
					string vn = "c" + Guid.NewGuid().GetHashCode().ToString("x");
					CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(typeof(TypeConverter), vn,
						new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter", new CodeTypeOfExpression(dataType.GetCodeTypeReference())));
					statements.Add(vds);
					return new CodeMethodInvokeExpression(
							new CodeVariableReferenceExpression(vn), "CanConvertTo", data, new CodeTypeOfExpression(targetType.GetCodeTypeReference()));
				}
			}
			TypeCode tc = Type.GetTypeCode(targetType.BaseClassType);
			switch (tc)
			{
				case TypeCode.Boolean:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToBoolean", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Byte:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToByte", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Char:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToChar", VPLUtil.GetCoreExpressionFromCast(data));

				case TypeCode.DateTime:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToDateTime", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Decimal:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToDecimal", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Double:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToDouble", VPLUtil.GetCoreExpressionFromCast(data));

				case TypeCode.Int16:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToInt16", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Int32:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToInt32", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Int64:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToInt64", VPLUtil.GetCoreExpressionFromCast(data));

				case TypeCode.SByte:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToSByte", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.Single:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToSingle", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.String:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToString", VPLUtil.GetCoreExpressionFromCast(data));

				case TypeCode.UInt16:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToUInt16", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.UInt32:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToUInt32", VPLUtil.GetCoreExpressionFromCast(data));
				case TypeCode.UInt64:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToUInt64", VPLUtil.GetCoreExpressionFromCast(data));

			}
			ConstructorInfo cif = targetType.BaseClassType.GetConstructor(new Type[] { dataType.BaseClassType });
			if (cif != null)
			{
				return new CodeObjectCreateExpression(targetType.BaseClassType, data);
			}
			return new CodeCastExpression(targetType.GetCodeTypeReference(), VPLUtil.GetCoreExpressionFromCast(data));
		}
		public static CodeCastExpression GetTypeConversion(string targetType, CodeExpression data)
		{
			return new CodeCastExpression(targetType, VPLUtil.GetCoreExpressionFromCast(data));
		}
		public static CodeExpression Is(CodeExpression var,
								CodeTypeReference type)
		{
			CodeExpression isNull = new CodeMethodInvokeExpression(
			   new CodeTypeReferenceExpression(typeof(object)),
				  "ReferenceEquals",
				   var,
				   new CodePrimitiveExpression(null));
			CodeExpression isNotNull = new CodeBinaryOperatorExpression(isNull,
			   CodeBinaryOperatorType.ValueEquality,
			   new CodePrimitiveExpression(false));
			CodeExpression isAssignable = new CodeMethodInvokeExpression(
			   new CodeTypeOfExpression(type),
			   "IsAssignableFrom",
			   new CodeMethodInvokeExpression(var, "GetType"));
			return new CodeBinaryOperatorExpression(isNotNull,
													CodeBinaryOperatorType.BooleanAnd,
													isAssignable);
		}
		public static bool IsOwnedByControl(IObjectPointer owner)
		{
			if (owner != null)
			{
				Type tc = typeof(Control);
				if (owner.Owner == null)
				{
					return (tc.IsAssignableFrom(owner.ObjectType));
				}
				Type tc2 = typeof(ToolStripItem);
				IObjectPointer o = owner;
				while (o != null && o.Owner != null)
				{
					if (tc.IsAssignableFrom(o.ObjectType))
					{
						return true;
					}
					if (tc2.IsAssignableFrom(o.ObjectType))
					{
						return true;
					}
					o = o.Owner;
				}
			}
			return false;
		}
		public static void CreateAssignment(CodeExpression left, Type leftType, CodeExpression right, Type rightType, CodeStatementCollection statements, bool bThreadSafe)
		{
			if (leftType.IsAssignableFrom(rightType))
			{
				CodeAssignStatement cas = new CodeAssignStatement(left, right);
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
			else
			{
				if (typeof(string).Equals(leftType) || typeof(PhpString).Equals(leftType))
				{
					if (rightType.IsValueType)
					{
						CodeAssignStatement cas = new CodeAssignStatement(left,
							new CodeMethodInvokeExpression(right, "ToString"));
						statements.Add(cas);
					}
					else
					{
						//if it is null then return null else return var.ToString()
						string vn = "l" + Guid.NewGuid().GetHashCode().ToString("x");
						CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(rightType, vn, right);
						statements.Add(vds);
						CodeConditionStatement ccs = new CodeConditionStatement(
							new CodeBinaryOperatorExpression(
								new CodeVariableReferenceExpression(vn), CodeBinaryOperatorType.IdentityInequality,
								new CodePrimitiveExpression(null)),
							new CodeStatement[] { new CodeAssignStatement(left, new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(vn), "ToString")) },
							new CodeStatement[] { new CodeAssignStatement(left, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Empty")) });
						statements.Add(ccs);
					}
				}
				else if (typeof(bool).Equals(leftType))
				{
					CodeAssignStatement cas = new CodeAssignStatement(left,
							ConvertToBool(rightType, right));
					statements.Add(cas);
				}
				else
				{
					bool converted = false;
					if ((leftType.IsPrimitive || leftType.GetInterface("IConvertible") != null)
						&& (rightType.IsPrimitive || rightType.GetInterface("IConvertible") != null))
					{
						CodeExpression ex = VPLUtil.ConvertByType(leftType, right);
						if (ex != null)
						{
							CodeAssignStatement cas = new CodeAssignStatement(left, ex);
							statements.Add(cas);
							converted = true;
						}
					}
					if (!converted)
					{
						TypeConverter converter = TypeDescriptor.GetConverter(leftType);
						if (converter != null)
						{
							if (converter.CanConvertFrom(rightType))
							{
								string vn = "c" + Guid.NewGuid().GetHashCode().ToString("x");
								CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(typeof(TypeConverter), vn,
									new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter", new CodeTypeOfExpression(leftType)));
								statements.Add(vds);
								CodeAssignStatement cas = new CodeAssignStatement(left,
									new CodeCastExpression(leftType,
									new CodeMethodInvokeExpression(
										new CodeVariableReferenceExpression(vn), "ConvertFrom", right)));
								statements.Add(cas);
								converted = true;
							}
						}
					}
					if (!converted)
					{
						TypeConverter converter = TypeDescriptor.GetConverter(rightType);
						if (converter != null)
						{
							if (converter.CanConvertTo(leftType))
							{
								string vn = "c" + Guid.NewGuid().GetHashCode().ToString("x");
								CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement(typeof(TypeConverter), vn,
									new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter", new CodeTypeOfExpression(rightType)));
								statements.Add(vds);
								CodeAssignStatement cas = new CodeAssignStatement(left,
									new CodeMethodInvokeExpression(
										new CodeVariableReferenceExpression(vn), "CanConvertTo", right, new CodeTypeOfExpression(leftType)));
								statements.Add(cas);
								converted = true;
							}
						}
					}
					if (!converted)
					{
						CodeAssignStatement cas = new CodeAssignStatement(left,
							new CodeCastExpression(leftType, VPLUtil.GetCoreExpressionFromCast(right)));
						statements.Add(cas);
					}
				}
			}
		}
	}
}
