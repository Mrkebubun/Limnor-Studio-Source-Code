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
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using VPL;
using System.Collections.Specialized;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// Its OutputVariable represents the assignment target.
	/// this class is no longer used??
	/// </summary>
	public class ActionAssignment : ActionExecMath
	{
		public ActionAssignment(ClassPointer owner)
			: base(owner)
		{
		}

		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// local action is for one method only
		/// </summary>
		[Browsable(false)]
		public override bool IsLocal
		{
			get
			{
				MathNodeRoot mathExp = MathExp as MathNodeRoot;
				if (mathExp != null)
				{
					if (mathExp[0].IsLocal)
						return true;
					if (mathExp[1].IsLocal)
						return true;
				}
				return false;
			}
		}
		public override void CreateJavaScript(StringCollection methodToCompile, Dictionary<string, StringCollection> formsumissions, string nextActionInput, string indent)
		{

		}
		public override void CreatePhpScript(StringCollection methodToCompile)
		{

		}
		public override void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				string ce = mathExp.ReturnJavaScriptCodeExpression(methodToCompile);
				if (!string.IsNullOrEmpty(ce))
				{
					string target = null;
					string output = null;
					if (nextAction != null)
					{
						if (nextAction.UseInput)
						{
							methodToCompile.Add("var ");
							methodToCompile.Add(currentAction.OutputCodeName);
							methodToCompile.Add("=");
							methodToCompile.Add(ce);
							methodToCompile.Add(";\r\n");
							output = currentAction.OutputCodeName;
						}
					}
					IVariable v = mathExp.OutputVariable;
					if (v != null)
					{
						string ceCondition = null;
						if (Condition != null)
						{
							ceCondition = Condition.CreateJavaScriptCode(methodToCompile);
						}
						target = v.ExportJavaScriptCode(methodToCompile);
						string jsLine;
						if (output != null)
						{
							jsLine = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}={1};\r\n", target, currentAction.OutputCodeName);
						}
						else
						{
							jsLine = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}={1};\r\n", target, ce);
						}
						if (string.IsNullOrEmpty(ceCondition))
						{
							methodToCompile.Add(jsLine);
						}
						else
						{
							methodToCompile.Add("if(");
							methodToCompile.Add(ceCondition);
							methodToCompile.Add(")\r\n{\r\n");
							methodToCompile.Add(jsLine);
							methodToCompile.Add("\r\n}\r\n");
						}
					}
				}
			}
		}
		public override void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				string ce = mathExp.ReturnPhpScriptCodeExpression(methodToCompile);
				if (!string.IsNullOrEmpty(ce))
				{
					string target = null;
					string output = null;
					if (nextAction != null)
					{
						if (nextAction.UseInput)
						{
							methodToCompile.Add(currentAction.OutputCodeName);
							methodToCompile.Add("=");
							methodToCompile.Add(ce);
							methodToCompile.Add(";\r\n");
							output = currentAction.OutputCodeName;
						}
					}
					IVariable v = mathExp.OutputVariable;
					if (v != null)
					{
						string ceCondition = null;
						if (Condition != null)
						{
							ceCondition = Condition.CreatePhpScriptCode(methodToCompile);
						}
						target = v.ExportPhpScriptCode(methodToCompile);
						string jsLine;
						if (output != null)
						{
							jsLine = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}={1};\r\n", target, currentAction.OutputCodeName);
						}
						else
						{
							jsLine = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}={1};\r\n", target, ce);
						}
						if (string.IsNullOrEmpty(ceCondition))
						{
							methodToCompile.Add(jsLine);
						}
						else
						{
							methodToCompile.Add("if(");
							methodToCompile.Add(ceCondition);
							methodToCompile.Add(")\r\n{\r\n");
							methodToCompile.Add(jsLine);
							methodToCompile.Add("\r\n}\r\n");
						}
					}
				}
			}
		}
		public override void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				CodeExpression ceCondition = null;
				if (Condition != null)
				{
					ceCondition = Condition.ExportCode(methodToCompile);
					if (ceCondition != null)
					{
						ceCondition = CompilerUtil.ConvertToBool(Condition.DataType, ceCondition);
					}
				}
				CodeExpression ce = mathExp.ReturnCodeExpression(methodToCompile);
				if (ce != null)
				{
					CodeExpression target = null;
					CodeVariableDeclarationStatement output = null;
					if (nextAction != null)
					{
						if (nextAction.UseInput)
						{
							output = new CodeVariableDeclarationStatement(currentAction.OutputType.TypeString, currentAction.OutputCodeName, ce);
							statements.Add(output);
						}
					}
					IVariable v = mathExp.OutputVariable;
					if (v != null)
					{
						CodeStatement cs;
						target = v.ExportCode(methodToCompile);
						if (output != null)
						{
							cs = new CodeAssignStatement(target, new CodeVariableReferenceExpression(currentAction.OutputCodeName));
						}
						else
						{
							cs = new CodeAssignStatement(target, ce);
						}
						if (ceCondition == null)
						{
							statements.Add(cs);
						}
						else
						{
							CodeConditionStatement cd = new CodeConditionStatement();
							cd.Condition = ceCondition;
							cd.TrueStatements.Add(cs);
							statements.Add(cd);
						}
					}
				}
			}
		}
		public override void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override string ToString()
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				IVariable v = mathExp.OutputVariable;
				if (v != null)
				{
					return v.VariableName + " = " + mathExp.ToString();
				}
				return mathExp.ToString();
			}
			return base.ToString();
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (VPLUtil.GetBrowseableProperties(attributes))
			{
				MathNodeRoot mr = MathExp as MathNodeRoot;
				if (mr != null)
				{
				}
			}
			return ps;
		}
	}
}
