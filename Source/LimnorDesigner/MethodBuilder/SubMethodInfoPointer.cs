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
using System.ComponentModel;
using System.Reflection;
using ProgElements;
using VPL;
using System.Globalization;
using Limnor.PhpComponents;
using Limnor.WebBuilder;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// method pointer for loop and other actions 
	/// it is used as ActionMethod for ActionSubMethod:IAction
	/// </summary>
	public class SubMethodInfoPointer : MethodInfoPointer
	{
		private List<ParameterClassSubMethod> _parameters;
		public SubMethodInfoPointer()
		{
		}
		public override bool ContainsGenericParameters
		{
			get
			{
				Type[] ts = ParameterTypes;
				if (ts != null && ts.Length > 0)
				{
					for (int i = 0; i < ts.Length; i++)
					{
						if (ts[i].IsGenericParameter)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		/// <summary>
		/// the ActionSubMethod this method is used for.
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public ActionSubMethod ActionOwner
		{
			get;
			set;
		}
		[Browsable(false)]
		public override string DefaultActionName
		{
			get
			{
				if (ActionOwner != null && ActionOwner.MethodOwner != null)
				{
					return string.Format(CultureInfo.InvariantCulture,
						"{0}.{1}", ActionOwner.MethodOwner.ExpressionDisplay, this.MemberName);
				}
				return this.MemberName;
			}
		}
		[Browsable(false)]
		public override Type ActionBranchType
		{
			get
			{
				return typeof(AB_SubMethodAction);
			}
		}
		[Browsable(false)]
		public override Type ActionType
		{
			get
			{
				return typeof(ActionSubMethod);
			}
		}
		public override ParameterValue CreateDefaultParameterValue(int i)
		{
			ParameterInfo[] ps = this.Info;
			if (ps != null && ps.Length > i)
			{
				ParameterInfo p = ps[i];
				ParameterValue pv = new ParameterValue(Action);
				pv.Name = p.Name;
				pv.ParameterID = (UInt32)(p.GetHashCode());
				pv.SetDataType(p.ParameterType);
				pv.ValueType = EnumValueType.ConstantValue;
				return pv;
			}
			return null;
		}
		[Browsable(false)]
		public override MethodBase MethodDef
		{
			get
			{
				if (this.MethodInfo == null)
				{
					CollectionPointer cp = this.Owner as CollectionPointer;
					if (cp != null)
					{
						if (string.CompareOrdinal(this.MethodName, SubMethodInfo.ExecuteForEachItem) == 0)
						{
							Type ta = cp.ObjectType;
							DataTypePointer ti;
							PhpArray pa = cp.ObjectInstance as PhpArray;
							if (pa != null)
							{
								ti = new DataTypePointer(pa.ItemType);
							}
							else
							{
								ti = new DataTypePointer(VPLUtil.GetElementType(ta));
								if (ti.IsGenericParameter)
								{
									DataTypePointer dp = this.GetConcreteType(ti.BaseClassType);
									if (dp != null)
									{
										ti.SetConcreteType(dp);
									}
								}
							}
							string sk = cp.ObjectKey;
							if (string.IsNullOrEmpty(sk))
							{
								sk = Guid.NewGuid().GetHashCode().ToString("x").ToString(CultureInfo.InvariantCulture);
							}
							CollectionForEachMethodInfo mif = new CollectionForEachMethodInfo(ti, ta, sk);
							SetMethodInfo(mif);
						}
					}
					else
					{
						if (typeof(PhpArray).Equals(this.Owner.ObjectType) || typeof(JsArray).Equals(this.Owner.ObjectType))
						{
							ArrayForEachMethodInfo af = new ArrayForEachMethodInfo(typeof(object), Owner.ObjectKey);
							SetMethodInfo(af);
						}
					}
				}
				return base.MethodDef;
			}
		}
		[Browsable(false)]
		public override IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> ps = new List<IParameter>();
				ParameterInfo[] info = Info;
				if (info != null)
				{
					for (int i = 0; i < info.Length; i++)
					{
						ParameterLib p = new ParameterLib(info[i], i);
						p.ParameterID = (UInt32)info[i].GetHashCode();
						ps.Add(p);
					}
				}
				return ps;
			}
		}
		public ParameterClassSubMethod GetParameterById(UInt32 id)
		{
			if (_parameters == null)
			{
				throw new DesignerException("Accessing GetParameterById without calling CreateParameters");
			}
			foreach (ParameterClassSubMethod p in _parameters)
			{
				if (p.ParameterID == id)
				{
					return p;
				}
			}
			return null;
		}
		public List<ParameterClassSubMethod> GetParameters(AB_SubMethodAction actionBranch)
		{
			if (_parameters == null)
			{
				CreateParameters(actionBranch);
			}
			return _parameters;
		}
		public void CreateParameters(AB_SubMethodAction actionBranch)
		{
			SubMethodInfo sm = (SubMethodInfo)(this.MethodInformation);
			List<ParameterClassSubMethod> ps = new List<ParameterClassSubMethod>();
			ParameterInfo[] info = Info;
			if (info != null)
			{
				for (int i = 0; i < info.Length; i++)
				{
					UInt32 pid;
					pid = (UInt32)info[i].GetHashCode();
					ParameterClassSubMethod p = sm.GetParameterType(pid, this, actionBranch);
					ps.Add(p);
				}
			}
			_parameters = ps;
		}
		public List<ParameterClassSubMethod> Parameters
		{
			get
			{
				if (_parameters == null)
				{
					throw new DesignerException("Accessing Parameters without calling CreateParameters");
				}
				return _parameters;
			}
		}
		protected override void OnSetMethodDef(MethodBase method)
		{
			base.OnSetMethodDef((SubMethodInfo)method);
		}
		[Browsable(false)]
		public override Type ReturnType
		{
			get
			{
				return typeof(void);
			}
		}
		[Browsable(false)]
		public override bool NoReturn
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public override bool HasReturn
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public bool SaveParameterValues
		{
			get
			{
				SubMethodInfo sm = (SubMethodInfo)(this.MethodInformation);
				return sm.SaveParameterValues;
			}
		}
	}
	public class SubMethodInfoPointerGlobal : SubMethodInfoPointer
	{
		private EnumWebActionType _actType;
		public SubMethodInfoPointerGlobal()
		{
			_actType = EnumWebActionType.Unknown;
		}
		[Browsable(false)]
		public override Type ActionType
		{
			get
			{
				return typeof(ActionSubMethodGlobal);
			}
		}
		[Browsable(false)]
		public EnumWebActionType WebActionType
		{
			get
			{
				ActionSubMethodGlobal a = ActionOwner as ActionSubMethodGlobal;
				if (a != null)
				{
					if (_actType != EnumWebActionType.Unknown)
					{
						a.ActionType = _actType;
					}
					else
					{
						a.CheckWebActionType();
						_actType = a.ActionType;
					}
				}
				return _actType;
			}
			set
			{
				ActionSubMethodGlobal a = ActionOwner as ActionSubMethodGlobal;
				if (a != null)
				{
					a.ActionType = value;
				}
				_actType = value;
			}
		}
	}
}
