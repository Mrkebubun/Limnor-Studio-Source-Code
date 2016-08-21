/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ProgElements;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using VPL;
using System.Xml;
using System.Collections;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// used by for loop as index, the branch is an ISubMethod
	/// </summary>
	public class ActionBranchParameter : ParameterClass
	{
		#region fields and constructors
		private ActionBranch _actionBranch;
		//for loading
		public ActionBranchParameter()
			: this((IMethod)null)
		{
		}
		/// <summary>
		/// for clone
		/// </summary>
		/// <param name="method"></param>
		public ActionBranchParameter(IMethod method)
			: base(method)
		{
			AllowTypeChange = false;
		}
		public ActionBranchParameter(ActionBranch branch)
			: base(branch.Method)
		{
			_actionBranch = branch;
			AllowTypeChange = false;
		}
		public ActionBranchParameter(ComponentIconActionBranchParameter componentIcon)
			: this(componentIcon.ActionBranch)
		{
		}
		public ActionBranchParameter(Type type, string name, ActionBranch branch)
			: base(type, name, branch.Method)
		{
			_actionBranch = branch;
			this.ParameterID = branch.BranchId;
			AllowTypeChange = false;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override uint MemberId
		{
			get
			{
				return _actionBranch.BranchId;
			}
		}
		[Browsable(false)]
		public override string ExpressionDisplay
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				if (string.IsNullOrEmpty(Name))
				{
					sb.Append("?");
				}
				else
				{
					sb.Append(Name);
				}
				if (_actionBranch != null)
				{
					if (!string.IsNullOrEmpty(_actionBranch.Name))
					{
						sb.Insert(0, ".");
						sb.Insert(0, _actionBranch.Name);
					}
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public ActionBranch ActionBranch
		{
			get
			{
				return _actionBranch;
			}
		}
		[Browsable(false)]
		public override Image ImageIcon
		{
			get
			{
				return Resources._sub_param.ToBitmap();
			}
		}
		[Browsable(false)]
		public override string CodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "${0}_{1}", Name, _actionBranch.BranchId.ToString("x"));
				}
				else
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", Name, _actionBranch.BranchId.ToString("x"));
				}
			}
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			return DisplayName;
		}
		public void SetActionBranch(ActionBranch act)
		{
			_actionBranch = act;
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			ActionBranchParameter abp = objectPointer as ActionBranchParameter;
			if (abp != null)
			{
				if (abp.ParameterID == this.ParameterID)
				{
					if (_actionBranch.BranchId == abp.ActionBranch.BranchId)
					{
						return true;
					}
				}
			}
			return false;
		}
		public ActionBranchParameterPointer CreatePointer()
		{
			ActionBranchParameterPointer p = new ActionBranchParameterPointer(this, _actionBranch.Method.RootPointer);
			p.MethodId = _actionBranch.Method.MethodID;
			p.BranchId = _actionBranch.BranchId;
			p.ClassId = _actionBranch.Method.ClassId;
			p.ParameterName = this.Name;
			return p;
		}
		public override object Clone()
		{
			ActionBranchParameter dtp = (ActionBranchParameter)base.Clone();
			dtp._actionBranch = _actionBranch;
			return dtp;
		}
		#endregion
	}
}
