/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using MathExp;
using System.CodeDom;

namespace LimnorDesigner.MethodBuilder
{
	public class CustomConstructorPointer : CustomMethodPointer, IConstructor, IObjectPointer
	{
		public CustomConstructorPointer()
		{
		}
		public CustomConstructorPointer(ConstructorClass method, IClass holder)
			: base(method, holder)
		{
		}
		public override void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			if (returnReceiver == null)
			{
				throw new DesignerException("Error compiling object creation. Variable not specified. {0}", this.DisplayName);
			}
			CodeExpression[] ps = null;
			if (parameters != null)
			{
				ps = new CodeExpression[parameters.Count];
				parameters.CopyTo(ps, 0);
			}
			CodeExpression cr = returnReceiver.GetReferenceCode(methodToCompile, statements, true);
			CodeObjectCreateExpression coe = new CodeObjectCreateExpression(Declarer.TypeString, ps);
			CodeAssignStatement cas = new CodeAssignStatement(cr, coe);
			statements.Add(cas);
		}
		public IList<NamedDataType> GetParameters()
		{
			List<NamedDataType> ps = new List<NamedDataType>();
			List<ParameterClass> pms = Parameters;
			if (pms != null)
			{
				foreach (ParameterClass p in pms)
				{
					ps.Add(p);
				}
			}
			return ps;
		}
		public void CopyFrom(IConstructor cp)
		{
			CustomConstructorPointer ccp = cp as CustomConstructorPointer;
			if (ccp != null)
			{
			}
		}
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public override string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
	}
}
