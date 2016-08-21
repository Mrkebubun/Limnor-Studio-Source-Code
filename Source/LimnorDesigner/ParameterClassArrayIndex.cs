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
using MathExp;
using LimnorDesigner.MethodBuilder;
using ProgElements;
using System.ComponentModel;
using VPL;
using LimnorDesigner.Action;
using System.Globalization;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// for(int i=0;i<array.Leng;i++)
	/// {
	///     ...array[i]
	/// }
	/// represent i
	/// </summary>
	public class ParameterClassArrayIndex : ParameterClassSubMethod
	{
		#region fields and constructors
		public ParameterClassArrayIndex(SubMethodInfoPointer method)
			: base(method)
		{
		}
		public ParameterClassArrayIndex(ActionBranch branch)
			: base(branch)
		{
		}
		public ParameterClassArrayIndex(ComponentIconActionBranchParameter componentIcon)
			: base(componentIcon)
		{
		}
		public ParameterClassArrayIndex(Type type, string name, ActionBranch branch)
			: base(type, name, branch)
		{
		}
		#endregion
		public override string CodeName
		{
			get
			{
				SubMethodInfoPointer smip = Method as SubMethodInfoPointer;
				ArrayForEachMethodInfo afe = smip.MethodInformation as ArrayForEachMethodInfo;
				return ArrayForEachMethodInfo.codeName2(afe.IndexCodeNames["index"], this.MemberId);
			}
		}
	}
	[UseParentObject]
	public class ParameterClassArrayItem : ParameterClassSubMethod
	{
		#region fields and constructors
		/// <summary>
		/// for loading
		/// </summary>
		public ParameterClassArrayItem()
		{
		}
		public ParameterClassArrayItem(IMethod method)
			: base(method)
		{
		}
		public ParameterClassArrayItem(ActionBranch branch)
			: base(branch)
		{
		}
		public ParameterClassArrayItem(ComponentIconActionBranchParameter componentIcon)
			: base(componentIcon)
		{
		}
		public ParameterClassArrayItem(Type type, string name, ActionBranch branch)
			: base(type, name, branch)
		{
		}
		#endregion
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (this.Method == null)
			{
				DesignUtil.WriteToOutputWindowAndLog("Method not found. ParameterID:{0}, ActionID:{1}, ClassId:{2}", this.ParameterID, ActionId, this.ClassId);
			}
			else
			{
				SubMethodInfoPointer smi = this.Method as SubMethodInfoPointer;
				if (smi == null)
				{
					DesignUtil.WriteToOutputWindowAndLog("method [{0}][{1}] found by parameter [{2}] is not a SubMethodInfoPointer", this.Method.MethodName, this.Method.ObjectKey, this.ParameterID);
				}
				else
				{
					SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
					CodeExpression[] ps = mi.GetIndexCodes(smi, method, MemberId);
					CodeExpression ce = new CodeArrayIndexerExpression(smi.Owner.GetReferenceCode(method, statements, forValue), ps);
					return ce;
				}
			}
			return null;
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (this.Method == null)
			{
			}
			if (this.Method == null)
			{
				DesignUtil.WriteToOutputWindowAndLog("GetJavaScriptReferenceCode failed. Method not found. ParameterID:{0}, ActionID:{1}, ClassId:{2}", this.ParameterID, ActionId, this.ClassId);
				throw new DesignerException("GetJavaScriptReferenceCode failed. Method not found. ParameterID:{0}, ActionID:{1}, ClassId:{2}", this.ParameterID, ActionId, this.ClassId);
			}
			else
			{
				SubMethodInfoPointer smi = this.Method as SubMethodInfoPointer;
				if (smi == null)
				{
					MethodClass mc = this.Method as MethodClass;
					if (ActionId != 0 && mc != null && mc.ActionList != null)
					{
						IAction act = mc.ActionList.GetActionById(ActionId);
						if (act != null)
						{
							smi = act.ActionMethod as SubMethodInfoPointer;
						}
					}
				}
				if (smi == null)
				{
					DesignUtil.WriteToOutputWindowAndLog("GetJavaScriptReferenceCode failed. method [{0}][{1}] found by parameter [{2}] is not a SubMethodInfoPointer", this.Method.MethodName, this.Method.ObjectKey, this.ParameterID);
				}
				else
				{
					if (this.ActionBranch == null)
					{
						MethodClass mc = this.Method as MethodClass;
						if (mc != null && mc.SubMethod != null && mc.SubMethod.Count > 0)
						{
							IMethod0 m0 = mc.SubMethod.Peek();
							AB_SubMethodAction asa = m0 as AB_SubMethodAction;
							if (asa != null)
							{
								this.SetActionBranch(asa);
							}
						}
					}
					if (this.ActionBranch == null)
					{
						if (smi.ActionOwner != null)
						{
							MethodClass mc = smi.ActionOwner.ActionHolder as MethodClass;
							if (mc != null && mc.SubMethod != null && mc.SubMethod.Count > 0)
							{
								IMethod0 m0 = mc.SubMethod.Peek();
								AB_SubMethodAction asa = m0 as AB_SubMethodAction;
								if (asa != null)
								{
									this.SetActionBranch(asa);
								}
							}
						}
					}
					if (this.ActionBranch == null)
					{
						DesignUtil.WriteToOutputWindowAndLog("GetJavaScriptReferenceCode failed. parameter [{2}] for method [{0}][{1}] missing ActionBranch", this.Method.MethodName, this.Method.ObjectKey, this.ParameterID);
					}
					else
					{
						SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
						string target = smi.Owner.GetJavaScriptReferenceCode(code);
						return string.Format(CultureInfo.InvariantCulture,
							"{0}[{1}]", target, mi.GetIndexCodeJS(smi, this.ActionBranch.BranchId));
					}
				}
			}
			return CodeName;
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (this.Method == null)
			{
				DesignUtil.WriteToOutputWindowAndLog("GetPhpScriptReferenceCode failed. Method not found. ParameterID:{0}, ActionID:{1}, ClassId:{2}", this.ParameterID, ActionId, this.ClassId);
			}
			else
			{
				SubMethodInfoPointer smi = this.Method as SubMethodInfoPointer;
				if (smi == null)
				{
					MethodClass mc = this.Method as MethodClass;
					if (ActionId != 0 && mc != null && mc.ActionList != null)
					{
						IAction act = mc.ActionList.GetActionById(ActionId);
						if (act != null)
						{
							smi = act.ActionMethod as SubMethodInfoPointer;
						}
					}
				}
				if (smi == null)
				{
					DesignUtil.WriteToOutputWindowAndLog("GetPhpScriptReferenceCode failed. method [{0}][{1}] found by parameter [{2}] is not a SubMethodInfoPointer", this.Method.MethodName, this.Method.ObjectKey, this.ParameterID);
				}
				else
				{
					if (this.ActionBranch == null)
					{
						MethodClass mc = this.Method as MethodClass;
						if (mc != null && mc.SubMethod != null && mc.SubMethod.Count > 0)
						{
							IMethod0 m0 = mc.SubMethod.Peek();
							AB_SubMethodAction asa = m0 as AB_SubMethodAction;
							if (asa != null)
							{
								this.SetActionBranch(asa);
							}
						}
					}
					if (this.ActionBranch == null)
					{
						DesignUtil.WriteToOutputWindowAndLog("GetPhpScriptReferenceCode failed. parameter [{2}] for method [{0}][{1}] missing ActionBranch", this.Method.MethodName, this.Method.ObjectKey, this.ParameterID);
					}
					else
					{
						SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
						string target = smi.Owner.GetPhpScriptReferenceCode(code);
						return string.Format(CultureInfo.InvariantCulture,
							"{0}[{1}]", target, mi.GetIndexCodePHP(smi, this.ActionBranch.BranchId));
					}
				}
			}
			return CodeName;
		}
	}
}
