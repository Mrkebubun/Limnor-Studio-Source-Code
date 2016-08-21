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
using MathExp;
using System.CodeDom;
using System.ComponentModel;
using VPL;
using System.Collections.Specialized;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// use the math exp as the method or get-property return.
	/// </summary>
	public class ActionMethodReturn : ActionExecMath
	{
		public ActionMethodReturn(ClassPointer owner)
			: base(owner)
		{
		}
		/// <summary>
		/// there should not be more actions after this one, so as to avoid "unreachable code" error
		/// </summary>
		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				return true;
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
				return true;
			}
		}
		public override void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
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
					if (ceCondition == null)
					{
						statements.Add(new CodeMethodReturnStatement(ce));
					}
					else
					{
						CodeConditionStatement cd = new CodeConditionStatement();
						cd.Condition = ceCondition;
						cd.TrueStatements.Add(new CodeMethodReturnStatement(ce));
						statements.Add(cd);
					}
				}
			}
		}
		public override void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			CreateJavaScript(methodToCompile, data.FormSubmissions, nextAction == null ? null : nextAction.InputName, "\t");
		}
		public override void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			CreatePhpScript(methodToCompile);
		}
		public override void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void CreateJavaScript(StringCollection methodToCompile, Dictionary<string, StringCollection> formsumissions, string nextActionInput, string indent)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				string ce = mathExp.ReturnJavaScriptCodeExpression(methodToCompile);
				if (!string.IsNullOrEmpty(ce))
				{
					string ceCondition = null;
					if (Condition != null)
					{
						ceCondition = Condition.CreateJavaScriptCode(methodToCompile);
					}
					if (string.IsNullOrEmpty(ceCondition))
					{
						methodToCompile.Add(indent);
						methodToCompile.Add(ce);
					}
					else
					{
						methodToCompile.Add(indent);
						methodToCompile.Add("if(");
						methodToCompile.Add(ceCondition);
						methodToCompile.Add(") {\r\n");
						methodToCompile.Add(indent);
						methodToCompile.Add("\t");
						methodToCompile.Add(ce);
						methodToCompile.Add("\r\n");
						methodToCompile.Add(indent);
						methodToCompile.Add("}\r\n");
					}
				}
			}
		}
		public override void CreatePhpScript(StringCollection methodToCompile)
		{
			IMathExpression mathExp = MathExp;
			if (mathExp != null)
			{
				string ce = mathExp.ReturnPhpScriptCodeExpression(methodToCompile);
				if (!string.IsNullOrEmpty(ce))
				{
					string ceCondition = null;
					if (Condition != null)
					{
						ceCondition = Condition.CreatePhpScriptCode(methodToCompile);
					}
					if (string.IsNullOrEmpty(ceCondition))
					{
						methodToCompile.Add(ce);
					}
					else
					{
						methodToCompile.Add("if(");
						methodToCompile.Add(ceCondition);
						methodToCompile.Add(")\r\n{\r\n");
						methodToCompile.Add(ce);
						methodToCompile.Add("\r\n}\r\n");
					}
				}
			}
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
