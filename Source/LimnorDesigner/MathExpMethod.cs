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
using ProgElements;
using System.ComponentModel;
using System.CodeDom;
using System.Collections.Specialized;
using VPL;
using System.Globalization;

namespace LimnorDesigner
{
	/// <summary>
	/// make Math expression into a method pointer to be used in action ActionExecMath
	/// </summary>
	public class MathExpMethod : IActionMethodPointer
	{
		#region fields and constructors
		IMathExpression _exp;
		IAction _action;
		public MathExpMethod()
		{
		}
		#endregion

		#region Properties
		public IMathExpression MathExpression
		{
			get
			{
				if (_exp == null)
				{
					_exp = new MathNodeRoot();
				}
				return _exp;
			}
			set
			{
				_exp = value;
			}
		}
		public string Name
		{
			get
			{
				return MathExpression.Name;
			}
			set
			{
				MathExpression.Name = value;
			}
		}
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				if (_exp != null)
				{
					return _exp.MethodParameterTypes;
				}
				return new List<IParameter>();
			}
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
			get { return EnumWebRunAt.Inherit; }
		}
		public IObjectPointer MethodDeclarer { get { return null; } }
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			ParameterValueCollection.ValidateParameterValues(parameterValues, MethodParameterTypes, this);
		}
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			IParameter p = MethodParameterTypes[i];
			ParameterValue pv = new ParameterValue(_action);
			pv.Name = p.Name;
			pv.ParameterID = p.ParameterID;
			pv.SetDataType(p.ParameterLibType);
			pv.MathExpression = _exp;
			pv.SetOwnerAction(_action);
			pv.ValueType = EnumValueType.MathExpression;
			return pv;
		}
		public bool CanBeReplacedInEditor
		{
			get { return true; }
		}

		public object GetParameterType(uint id)
		{
			if (_exp != null && _exp.MethodParameterTypes != null)
			{
				foreach (IParameter p in _exp.MethodParameterTypes)
				{
					if (p.ParameterID == id)
					{
						return p;
					}
				}
			}
			return null;
		}
		public object GetParameterTypeByIndex(int idx)
		{
			if (idx >= 0 && _exp != null && _exp.MethodParameterTypes != null && idx < _exp.MethodParameterTypes.Count)
			{
				return _exp.MethodParameterTypes[idx];
			}
			return null;
		}
		public object GetParameterType(string name)
		{
			if (_exp != null && _exp.MethodParameterTypes != null)
			{
				foreach (IParameter p in _exp.MethodParameterTypes)
				{
					if (string.Compare(name, p.Name, StringComparison.Ordinal) == 0)
					{
						return p;
					}
				}
			}
			return null;
		}
		public Dictionary<string, string> GetParameterDescriptions()
		{
			return null;
		}

		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}

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

		public string DefaultActionName
		{
			get { return "Calculate"; }
		}

		public IMethod MethodPointed
		{
			get { return _exp; }
		}
		public void SetParameterJS(string[] ps)
		{
			VariableList vs = _exp.InportVariables;
			if (vs != null || ps != null)
			{
				if (vs != null && ps != null)
				{
					if (vs.Count != ps.Length)
					{
						throw new DesignerException("{0}.SetParameterJS with mismatching parameters vs:{1} ps:{2}", this.GetType(), vs.Count, ps.Length);
					}
					for (int i = 0; i < vs.Count; i++)
					{
						vs[i].AssignJavaScriptCode(ps[i]);
					}
				}
				else
				{
					throw new DesignerException("{0}.SetParameterJS with mismatching parameters {1}:{2}", this.GetType(), vs, ps);
				}
			}
		}
		public void SetParameterPhp(string[] ps)
		{
			VariableList vs = _exp.InportVariables;
			if (vs != null || ps != null)
			{
				if (vs != null && ps != null)
				{
					if (vs.Count != ps.Length)
					{
						throw new DesignerException("{0}.SetParameterPhp with mismatching parameters vs:{1} ps:{2}", this.GetType(), vs.Count, ps.Length);
					}
					for (int i = 0; i < vs.Count; i++)
					{
						vs[i].AssignPhpScriptCode(ps[i]);
					}
				}
				else
				{
					throw new DesignerException("{0}.SetParameterPhp with mismatching parameters {1}:{2}", this.GetType(), vs, ps);
				}
			}
		}
		public void SetParameterExpressions(CodeExpression[] ps)
		{
			VariableList vs = _exp.InportVariables;
			if (vs != null || ps != null)
			{
				if (vs != null && ps != null)
				{
					if (vs.Count != ps.Length)
					{
						throw new DesignerException("{0}.SetParameterExpressions with mismatching parameters vs:{1} ps:{2}", this.GetType(), vs.Count, ps.Length);
					}
					for (int i = 0; i < vs.Count; i++)
					{
						vs[i].AssignCode(ps[i]);
					}
				}
				else
				{
					throw new DesignerException("{0}.SetParameterExpressions with mismatching parameters {1}:{2}", this.GetType(), vs, ps);
				}
			}
		}
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_exp != null)
			{
				return _exp.ReturnCodeExpression(method);
			}
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection method)
		{
			if (_exp != null)
			{
				return _exp.CreateJavaScript(method);
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection method)
		{
			if (_exp != null)
			{
				return _exp.CreatePhpScript(method);
			}
			return null;
		}
		public Type ReturnBaseType
		{
			get
			{
				return typeof(void);
			}
		}
		public IObjectPointer Owner
		{
			get
			{
				if (_action != null)
				{
					return _action.MethodOwner;
				}
				return null;
			}
		}
		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}
		public string MethodName
		{
			get
			{
				if (_exp != null)
				{
					return _exp.Name;
				}
				return string.Empty;
			}
		}
		public string CodeName { get { return MethodName; } }
		public bool IsValid
		{
			get
			{
				if (_exp != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_exp is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		#endregion

		#region IMethodPointer Members

		public bool NoReturn
		{
			get { return true; }
		}

		public int ParameterCount
		{
			get { return 0; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsForLocalAction
		{
			get { return true; }
		}

		public bool IsSameMethod(IMethodPointer pointer)
		{
			MathExpMethod mem = pointer as MathExpMethod;
			if (mem != null && mem.MathExpression != null)
			{
				return mem.MathExpression.IsSameMethod(_exp);
			}
			return false;
		}

		public bool IsSameMethod(IMethod method)
		{
			if (_exp != null)
			{
				return _exp.IsSameMethod(method);
			}
			return false;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			MathExpMethod mem = new MathExpMethod();
			if (_exp != null)
			{
				mem._exp = (IMathExpression)_exp.Clone();
			}
			mem._action = _action;
			return mem;
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MathExpMethod mem = objectIdentity as MathExpMethod;
			if (mem != null)
			{
				return IsSameMethod(mem);
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_action != null)
				{
					return _action.IdentityOwner;
				}
				return null;
			}
		}

		public bool IsStatic
		{
			get
			{
				return false;
			}
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Method; }
		}

		#endregion

	}
}
