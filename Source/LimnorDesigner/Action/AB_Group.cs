/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using System.CodeDom;
using LimnorDesigner.MethodBuilder;
using MathExp;
using System.Globalization;
using System.ComponentModel;
using XmlUtility;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// represent a group of actions
	/// </summary>
	public class AB_Group : AB_Squential
	{
		#region fields and constructors
		public AB_Group(BranchList acts)
			: base(acts)
		{
		}
		public AB_Group(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_Group(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_Group(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region private methods
		private IList<ActionBranch> getFirstAction()
		{
			List<ActionBranch> firstAction = new List<ActionBranch>();
			BranchList _list = this.ActionList;
			foreach (ActionBranch sab in _list)
			{
				bool isTheFirst = true;
				for (int k = 0; k < sab.InPortList.Count; k++)
				{
					if (sab.InPortList[k].LinkedPortID != 0)
					{
						foreach (ActionBranch av2 in _list)
						{
							for (int i = 0; i < av2.OutPortList.Count; i++)
							{
								if (av2.OutPortList[i].PortID == sab.InPortList[k].LinkedPortID)
								{
									isTheFirst = false;
									break;
								}
							}
							if (!isTheFirst)
							{
								break;
							}
						}
					}
					if (!isTheFirst)
					{
						break;
					}
				}
				if (isTheFirst)
				{
					firstAction.Add(sab);
				}
			}
			return firstAction;
		}
		private IList<ActionBranch> getLastActions()
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				List<ActionBranch> lst = new List<ActionBranch>();
				foreach (ActionBranch sab in _list)
				{
					bool isTheLast = true;
					for (int k = 0; k < sab.OutPortList.Count; k++)
					{
						if (sab.OutPortList[k].LinkedPortID != 0)
						{
							foreach (ActionBranch av2 in _list)
							{
								for (int i = 0; i < av2.InPortList.Count; i++)
								{
									if (av2.InPortList[i].PortID == sab.OutPortList[k].LinkedPortID)
									{
										isTheLast = false;
										break;
									}
								}
								if (!isTheLast)
								{
									break;
								}
							}
						}
						if (!isTheLast)
						{
							break;
						}
					}
					if (isTheLast)
					{
						lst.Add(sab);
					}
				}
				return lst;
			}
			return null;
		}
		#endregion
		#region IPortOwner Members

		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Action group:{0}", BranchId);
			}
		}

		#endregion
		#region ICloneable Members
		#endregion
		#region Overrides
		public override bool IsValid
		{
			get
			{
				return true;
			}
		}
		public override UInt32 FirstActionId
		{
			get
			{
				IList<ActionBranch> f = getFirstAction();
				if (f != null && f.Count > 0)
				{
					foreach (ActionBranch ab in f)
					{
						if (ab.IsMainBranch)
						{
							return ab.BranchId;
						}
					}
					return f[0].BranchId;
				}
				return BranchId;
			}
		}
		public override void MakePortLinkForSingleThread(List<UInt32> used, BranchList branch)
		{
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);
				base.MakePortLinkForSingleThread(used, branch);
			}
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			return designer.LoadAction(this);
		}
		public override Type ViewerType
		{
			get
			{
				return typeof(ActionViewerGroup);
			}
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			IList<ActionBranch> lasts = getLastActions();
			if (lasts != null && lasts.Count > 0)
			{
				foreach (ActionBranch ab in lasts)
				{
					if (!ab.AllBranchesEndWithMethodReturnStatement())
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				//link both actions
				if (nextAction != null)
				{
					if (!string.IsNullOrEmpty(OutputCodeName) && OutputType != null && !OutputType.IsVoid)
					{
						nextAction.InputName = OutputCodeName;
						nextAction.InputType = OutputType;
						nextAction.SetInputName(OutputCodeName, OutputType);
					}
				}
				//
				CompilerUtil.ClearGroupGotoBranches(method, this.BranchId);
				bool bRet = _list.ExportCode(compiler, method, statements);
				bRet = CompilerUtil.FinishActionGroup(method, statements, this.BranchId, bRet);
				return bRet;
			}
			return false;
		}

		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				//link both actions
				if (nextAction != null)
				{
					if (!string.IsNullOrEmpty(OutputCodeName) && OutputType != null && !OutputType.IsVoid)
					{
						nextAction.InputName = OutputCodeName;
						nextAction.InputType = OutputType;
						nextAction.SetInputName(OutputCodeName, OutputType);
					}
				}
				return _list.ExportJavaScriptCode(jsCode, methodCode, data);
			}
			return false;
		}

		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				//link both actions
				if (nextAction != null)
				{
					if (!string.IsNullOrEmpty(OutputCodeName) && OutputType != null && !OutputType.IsVoid)
					{
						nextAction.InputName = OutputCodeName;
						nextAction.InputType = OutputType;
						nextAction.SetInputName(OutputCodeName, OutputType);
					}
				}
				return _list.ExportPhpScriptCode(jsCode, methodCode, data);
			}
			return false;
		}

		public override void Execute(List<ParameterClass> eventParameters)
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				//get the first action
				IList<ActionBranch> firstAction = getFirstAction();
				if (firstAction != null)
				{
					foreach (ActionBranch ab in firstAction)
					{
						ab.Execute(eventParameters);
					}
				}
			}
		}
		public override void LinkJumpedBranches(BranchList branches)
		{

		}

		public override void RemoveOutOfGroupBranches(BranchList branches)
		{

		}

		public override void LinkActions(BranchList branches)
		{

		}

		public override void SetInputName(string name, DataTypePointer type)
		{
			//get the first action
			IList<ActionBranch> firstAction = getFirstAction();
			if (firstAction != null)
			{
				foreach (ActionBranch ab in firstAction)
				{
					ab.InputName = name;
					ab.InputType = type;
					ab.SetInputName(name, type);
				}
			}
		}
		public override void SetBranchOwner()
		{
			BranchList _list = this.ActionList;
			if (_list != null)
			{
				foreach (ActionBranch a in _list)
				{
					a.GroupBranch = this;
					a.SetBranchOwner();
				}
			}
		}

		public override bool UseInput
		{
			get
			{
				IList<ActionBranch> firstAction = getFirstAction();
				if (firstAction != null)
				{
					foreach (ActionBranch ab in firstAction)
					{
						if (ab.UseInput)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public override bool HasOutput
		{
			get
			{
				IList<ActionBranch> lst = getLastActions();
				if (lst != null)
				{
					if (lst.Count == 1)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override DataTypePointer OutputType
		{
			get
			{
				IList<ActionBranch> lst = getLastActions();
				if (lst != null)
				{
					if (lst.Count == 1)
					{
						return lst[0].OutputType;
					}
				}
				return new DataTypePointer(typeof(void));
			}
		}
		/// <summary>
		/// variable name for generating variable by output action and for referencing by input action
		/// </summary>
		[Browsable(false)]
		public override string OutputCodeName
		{
			get
			{
				IList<ActionBranch> lst = getLastActions();
				if (lst != null)
				{
					if (lst.Count == 1)
					{
						return lst[0].OutputCodeName;
					}
				}
				return null;
			}
		}
		public override bool IsMethodReturn
		{
			get
			{
				IList<ActionBranch> lst = getLastActions();
				if (lst != null && lst.Count > 0)
				{
					foreach (ActionBranch ab in lst)
					{
						if (!ab.IsMethodReturn)
						{
							return false;
						}
					}
					return true;
				}
				return true;
			}
		}

		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
		}

		public override object GetPropertyOwner(int id, string propertyName)
		{
			return null;
		}
		#endregion
	}
}
