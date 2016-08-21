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
using MathExp;
using LimnorDesigner.MethodBuilder;
using XmlSerializer;
using System.Drawing;
using System.CodeDom;
using System.Threading;
using VPL;
using System.Xml;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// list of actions as a part of method contents.
	/// it does not have any branches.
	/// it may have one or more inports and one outport.
	/// it may or may not link to other ActionString. scenarios:
	/// 1. it does not link to a previous ActionStrings:
	///     it is an initial ActionString
	/// 2. it links to one previous ActionString:
	///     it should be combined to the previous ActionString to form a new ActionString
	/// 3. it links to more than one previous ActionString
	///     the previous ActionStrings will jump to this ActionString
	///     
	/// It may only link to one outport
	/// 1. it does not link to a next ActionString:
	///     it is a method return
	/// 2. it links to a next ActionString:
	///     2.1. the next ActionString only links to this ActionString
	///        the two ActionStrings should be combined
	///     2.2. the next ActionString also links to other ActionStrings
	///        it jumps to the next ActionString
	/// 
	/// </summary>
	public class AB_ActionString : AB_Squential, ICloneable
	{
		#region fields and constructors
		private UInt32 _jumpToId;//branch id to jump to
		private ActionBranch _jumpToActionBranch;
		public AB_ActionString(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_ActionString(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_ActionString(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Methods
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			base.OnGetActionNames(sc, usedBranches);
			if (_jumpToActionBranch != null)
			{
				_jumpToActionBranch.GetActionNames(sc, usedBranches);
			}
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			base.OnEstablishObjectOwnership(scope, usedBranches);
			if (_jumpToActionBranch != null)
			{
				_jumpToActionBranch.EstablishObjectOwnership(scope, usedBranches);
			}
		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			base.CollectSourceValues(taskId, usedBranches, mc);
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				return base.UseClientServerValues(usedBranches, client);
			}
			return false;
		}
		public override void GetCustomMethods(List<UInt32> usedBranches, List<MethodClass> list)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			base.GetCustomMethods(usedBranches, list);
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (base.LoadToDesigner(used, designer))
			{
				if (_jumpToActionBranch != null)
				{
					_jumpToActionBranch.LoadToDesigner(used, designer);
				}
				return true;
			}
			return false;
		}
		public override ActionPortIn GetActionInport(List<UInt32> used, UInt32 portId, UInt32 portInstanceId)
		{
			ActionBranch ai = FirstAction;
			if (ai != null)
			{
				return ai.GetActionInport(used, portId, portInstanceId);
			}
			return null;
		}
		public override void MakePortLinkForSingleThread(List<UInt32> used, BranchList branch)
		{
			ActionBranch ai = FirstAction;
			if (ai != null)
			{
				ai.MakePortLinkForSingleThread(used, branch);
			}
		}
		protected override void OnVerifyJump(BranchList branch)
		{
			if (_jumpToId == 0)
			{
				List<ActionPortOut> aos = OutPortList;
				if (aos != null && aos.Count > 0)
				{
					if (aos[0].LinkedInPort != null)
					{
						ActionBranch ab = aos[0].LinkedInPort.PortOwner as ActionBranch;
						if (ab != null)
						{
							_jumpToId = ab.BranchId;
							_jumpToActionBranch = ab;
						}
					}
				}
			}
			else
			{
				if (_jumpToId == FirstActionId)
				{
					_jumpToId = 0;
				}
			}
		}
		public override LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			LocalVariable v = base.GetLocalVariable(usedBranches, id);
			if (v == null)
			{
				if (_jumpToActionBranch != null)
				{
					v = _jumpToActionBranch.GetLocalVariable(usedBranches, id);
				}
			}
			return v;
		}
		public override void SetIsMainThreadForSubBranches(List<UInt32> usedBranches)
		{
			base.SetIsMainThreadForSubBranches(usedBranches);
			if (_jumpToActionBranch != null)
			{
				_jumpToActionBranch.IsMainThread = this.IsMainThread;
				_jumpToActionBranch.SetIsMainThreadForSubBranches(usedBranches);
			}
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_jumpToActionBranch != null)
			{
				return _jumpToActionBranch.AllBranchesEndWithMethodReturnStatement();
			}
			return base.AllBranchesEndWithMethodReturnStatement();
		}
		/// <summary>
		/// returns extra bracnhes to be added into branches
		/// </summary>
		/// <param name="branches"></param>
		/// <returns></returns>
		public override List<ActionBranch> OnExport(BranchList branches, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			usedBranches.Add(this.BranchId);
			if (_jumpToActionBranch != null)
			{
				_jumpToId = _jumpToActionBranch.FirstActionId;
				List<ActionBranch> list = new List<ActionBranch>();
				List<ActionBranch> l = _jumpToActionBranch.OnExport(branches, usedBranches);
				if (l != null && l.Count > 0)
				{
					list.AddRange(l);
				}
				if (!_jumpToActionBranch.IsMergingPoint)
				{
					list.Add(_jumpToActionBranch);
				}
				return list;
			}
			else
			{
				_jumpToId = 0;
			}
			return null;
		}
		public override void InitializeBranches(BranchList branches)
		{
			if (_jumpToId != 0)
			{
				if (_jumpToActionBranch == null)
				{
					_jumpToActionBranch = branches.GetJumpToActionBranch(_jumpToId);
					if (_jumpToActionBranch == null)
					{
						throw new DesignerException("JumpTo Action for action-list not found (id={0})", _jumpToId);
					}
				}
			}
			else
			{
				_jumpToActionBranch = null;
			}
		}
		/// <summary>
		/// it is a fake action block, do not treat it like a normal one, do not add debug breaks
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		public override bool ExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			return OnExportCode(previousAction, nextAction, compiler, method, statements);
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			bool isGotoPoint = this.IsGotoPoint;
			ActionBranch nt = nextAction;
			if (_jumpToId != 0)
			{
				//next action is the one it jumps to
				nt = _jumpToActionBranch;
			}
			MethodSegment ms0 = null;
			CodeStatementCollection sts = statements;
			if (isGotoPoint)
			{
				//two or more branches in the same thread linked to this branch
				//since goto-label must be in the method scope, not sub-scope, this branch code must be
				//in the method scope
				ms0 = CompilerUtil.GetGotoBranch(method, Method, this.FirstActionId);
				if (ms0 == null)
				{
					sts = new CodeStatementCollection();
					ms0 = new MethodSegment(sts);
					CompilerUtil.AddGotoBranch(method, Method, this.FirstActionId, ms0, this.GroupBranchId);
				}
				else
				{
					throw new DesignerException("Action list as goto branch {0} compiled twice", this.FirstActionId);
				}
				//use goto statement to jump to this branch is the responsibility of the branches jumping to it.
			}
			if (this.IsWaitingPoint)
			{
				sts.Add(new CodeExpressionStatement(
					new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression(typeof(WaitHandle)), "WaitAll",
							new CodeVariableReferenceExpression(XmlSerialization.FormatString("wh_{0}_{1}",
								IdToKey(this.StartingBranchId), this.BranchKey)))));
			}
			bool b0 = base.OnExportCode(previousAction, nt, compiler, method, sts);
			if (ms0 != null)
			{
				ms0.Completed = b0;
			}
			if (b0)
			{
				return true;
			}
			else
			{
				//not all sub-branches of this branch completed.
				//check jumping
				if (_jumpToId != 0)
				{
					bool bRet = false;
					//same thread: use goto or fall through; otherwise use waiting point
					if (_jumpToActionBranch.StartingBranchId == this.StartingBranchId)
					{
						//a goto branch, use goto
						if (_jumpToActionBranch.IsGotoPoint)
						{
							sts.Add(new CodeGotoStatement(ActionBranch.IdToLabel(_jumpToId)));
							bRet = true;
						}
						if (!_jumpToActionBranch.IsCompiled)
						{
							bool b = _jumpToActionBranch.ExportCode(this, null, compiler, method, sts);
							if (!_jumpToActionBranch.IsGotoPoint)
							{
								bRet = b;
							}
						}
					}
					return bRet;
				}
				else
				{
					//not completed
					return false;
				}
			}
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			bool isGotoPoint = this.IsGotoPoint;
			ActionBranch nt = nextAction;
			if (_jumpToId != 0)
			{
				//next action is the one it jumps to
				nt = _jumpToActionBranch;
			}
			StringCollection sts = methodCode;
			// -- not use ActionBlock
			JsMethodSegment ms0 = null;
			if (isGotoPoint)
			{
				//two or more branches in the same thread linked to this branch
				//since goto-label must be in the method scope, not sub-scope, this branch code must be
				//in the method scope
				//ms0 = CompilerUtil.GetGotoBranch(method, Method, this.FirstActionId);
				ms0 = data.GetGotoBranch(this.FirstActionId);
				if (ms0 == null)
				{
					sts = new StringCollection();
					ms0 = new JsMethodSegment(sts);
					data.AddGotoBranch(this.FirstActionId, ms0);
				}
				else
				{
					throw new DesignerException("Action list as goto branch {0} compiled twice", this.FirstActionId);
				}
				//use goto statement to jump to this branch is the responsibility of the branches jumping to it.
			}

			bool b0 = base.OnExportJavaScriptCode(previousAction, nt, jsCode, sts, data);
			// -- not use ActionBlock
			if (ms0 != null)
			{
				ms0.Completed = b0;
			}
			if (b0)
			{
				return true;
			}
			else
			{
				//not all sub-branches of this branch completed.
				//check jumping
				if (_jumpToId != 0)
				{
					bool bRet = false;
					//same thread: use goto or fall through; otherwise use waiting point
					if (_jumpToActionBranch.StartingBranchId == this.StartingBranchId)
					{
						//a goto branch, use goto
						if (_jumpToActionBranch.IsGotoPoint)
						{
							sts.Add(Indentation.GetIndent());
							sts.Add("return ");
							sts.Add(ActionBranch.IdToLabel(_jumpToId));
							sts.Add("();\r\n");
							bRet = true;
						}
						if (!_jumpToActionBranch.IsCompiled)
						{
							if (_jumpToActionBranch.IsGotoPoint)
							{
								Indentation.IndentIncrease();
							}
							bool b = _jumpToActionBranch.ExportJavaScriptCode(this, null, jsCode, sts, data);
							if (_jumpToActionBranch.IsGotoPoint)
							{
								Indentation.IndentDecrease();
							}
							if (!_jumpToActionBranch.IsGotoPoint)
							{
								bRet = b;
							}
						}
					}
					return bRet;
				}
				else
				{
					//not completed
					return false;
				}
			}
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			bool isGotoPoint = this.IsGotoPoint;
			ActionBranch nt = nextAction;
			if (_jumpToId != 0)
			{
				//next action is the one it jumps to
				nt = _jumpToActionBranch;
			}
			JsMethodSegment ms0 = null;
			StringCollection sts = methodCode;
			if (isGotoPoint)
			{
				//two or more branches in the same thread linked to this branch
				//since goto-label must be in the method scope, not sub-scope, this branch code must be
				//in the method scope
				ms0 = data.GetGotoBranch(this.FirstActionId);
				if (ms0 == null)
				{
					sts = new StringCollection();
					ms0 = new JsMethodSegment(sts);
					data.AddGotoBranch(this.FirstActionId, ms0);
				}
				else
				{
					throw new DesignerException("Action list as goto branch {0} compiled twice", this.FirstActionId);
				}
				//use goto statement to jump to this branch is the responsibility of the branches jumping to it.
			}
			bool b0 = base.OnExportPhpScriptCode(previousAction, nt, jsCode, sts, data);
			if (ms0 != null)
			{
				ms0.Completed = b0;
			}
			if (b0)
			{
				return true;
			}
			else
			{
				//not all sub-branches of this branch completed.
				//check jumping
				if (_jumpToId != 0)
				{
					bool bRet = false;
					//same thread: use goto or fall through; otherwise use waiting point
					if (_jumpToActionBranch.StartingBranchId == this.StartingBranchId)
					{
						//a goto branch, use goto
						if (_jumpToActionBranch.IsGotoPoint)
						{
							sts.Add(Indentation.GetIndent());
							sts.Add("goto ");
							sts.Add(ActionBranch.IdToLabel(_jumpToId));
							sts.Add(";\r\n");
							bRet = true;
						}
						if (!_jumpToActionBranch.IsCompiled)
						{
							if (_jumpToActionBranch.IsGotoPoint)
							{
								Indentation.IndentIncrease();
							}
							bool b = _jumpToActionBranch.ExportPhpScriptCode(this, null, jsCode, sts, data);
							if (_jumpToActionBranch.IsGotoPoint)
							{
								Indentation.IndentDecrease();
							}
							if (!_jumpToActionBranch.IsGotoPoint)
							{
								bRet = b;
							}
						}
					}
					return bRet;
				}
				else
				{
					//not completed
					return false;
				}
			}
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void LinkJumpBranches()
		{
			if (_jumpToId != 0)
			{
				if (_jumpToActionBranch == null)
				{
					throw new DesignerException("Cannot set portlink from {0} to {1}. Linked actions not set", this.BranchId, _jumpToId);
				}
				ActionPortOut po = this.OutPortList[0];
				ActionPortIn pi = _jumpToActionBranch.GetInPort(po.LinkedPortID, po.LinkedPortInstanceID);//.InPortList[po.LinkedPortIndex];
				if (pi == null)
				{
					throw new DesignerException("out-port {0} is linked to [{1},{2}] of the jump-to-branch {3}, but in-port [{1},{2}] is not among the in-port list of the branch",
						po.PortID, po.LinkedPortID, po.LinkedPortInstanceID, _jumpToActionBranch.BranchId);
				}
				LinkLineNode start = po.End;
				LinkLineNode end;
				LinkLineNodeOutPort pipo = pi.Start as LinkLineNodeOutPort;
				if (pipo != null)
				{
					if (pipo.LinkedPortID != pi.PortID || pipo.LinkedPortInstanceID != pi.PortInstanceID)
					{
						throw new DesignerException("Input [{0},{1}] is already linked to [{2},{3}], cannot link it to [{4},{5}]",
							pi.PortID, pi.PortInstanceID, pipo.PortID, pipo.PortInstanceID, po.PortID, po.PortInstanceID);
					}
				}
				else
				{
					end = pi.Start;
					if (end != start)
					{
						start.SetNext(end);
						end.SetPrevious(start);
						po.LinkedPortID = pi.PortID;
						pi.LinkedPortID = po.PortID;
					}
					else
					{
						//it is not an error, it is already linked
					}
				}
			}
			base.LinkJumpBranches();
		}
		public override void RemoveOutOfGroupBranches(BranchList branches)
		{
			if (_jumpToActionBranch != null)
			{
				_jumpToActionBranch.RemoveOutOfGroupBranches(branches);
			}
			base.RemoveOutOfGroupBranches(branches);
		}
		public override void LinkJumpedBranches(BranchList branches)
		{
			if (_jumpToId != 0)
			{
				if (_jumpToActionBranch == null || (_jumpToActionBranch.BranchId != _jumpToId && _jumpToActionBranch.FirstActionId != _jumpToId))
				{
					_jumpToActionBranch = branches.GetJumpToActionBranch(_jumpToId);
					if (_jumpToActionBranch == null)
					{
						throw new DesignerException("Invalid jump id [{0}] for branch [{1}]", _jumpToId, this.BranchId);
					}

					MathNode.Trace("Action string jump from [{0},{1}] to [{2},{3}]", this.BranchId, this.Name, _jumpToActionBranch.BranchId, _jumpToActionBranch.Name);
				}
				_jumpToActionBranch.SetPreviousAction(this);
				this.SetNextAction(_jumpToActionBranch);
			}
			base.LinkJumpedBranches(branches);
		}
		/// <summary>
		/// set NextActions and PreviousActions properties before compiling
		/// </summary>
		public override void LinkActions(BranchList branches)
		{
			LinkJumpedBranches(branches);
			base.LinkActions(branches);
		}


		public override Type ViewerType
		{
			get
			{
				throw new NotImplementedException("ViewerType not implemented by ActionString");
			}
		}

		#endregion
		#region Properties
		public override bool IsValid
		{
			get
			{
				return true;
			}
		}
		[ReadOnly(true)]
		public override bool IsMainThread
		{
			get
			{
				ActionBranch a = FirstAction;
				if (a != null)
				{
					return a.IsMainThread;
				}
				return false;
			}
			set
			{
				ActionBranch a = FirstAction;
				if (a != null)
				{
					a.IsMainThread = value;
				}
			}
		}
		[ReadOnly(true)]
		public override List<ActionPortIn> InPortList
		{
			get
			{
				ActionBranch a = FirstAction;
				if (a != null)
				{
					return a.InPortList;
				}
				else
				{
					return null;
				}
			}
			set
			{
				ActionBranch a = FirstAction;
				if (a != null)
				{
					a.InPortList = value;
				}
			}
		}
		[ReadOnly(true)]
		public override List<ActionPortOut> OutPortList
		{
			get
			{
				ActionBranch a = LastAction;
				if (a != null)
				{
					return a.OutPortList;
				}
				else
				{
					return null;
				}
			}
			set
			{
				ActionBranch a = LastAction;
				if (a != null)
				{
					a.OutPortList = value;
				}
			}
		}
		/// <summary>
		/// if it jumps then it is not the end; 
		/// otherwise it is determined by the last action branch
		/// </summary>
		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				// if it jumps then it is not the end
				if (_jumpToActionBranch != null)
					return false;
				return base.IsMethodReturn;
			}
		}

		[DefaultValue(0)]
		public UInt32 JumpToStringId
		{
			get
			{
				return _jumpToId;
			}
			set
			{
				_jumpToId = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public ActionBranch JumpTo
		{
			get
			{
				return _jumpToActionBranch;
			}
			set
			{
				_jumpToActionBranch = value;
			}
		}

		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_ActionString obj = (AB_ActionString)base.Clone();
			obj.JumpToStringId = _jumpToId;
			obj.JumpTo = _jumpToActionBranch;
			return obj;
		}

		#endregion
		#region IAction Members
		public override void Execute(List<ParameterClass> eventParameters)
		{
			base.Execute(eventParameters);
			ExecuteJump(eventParameters);
		}

		protected void ExecuteJump(List<ParameterClass> eventParameters)
		{
			if (_jumpToId != 0)
			{
				if (_jumpToActionBranch == null)
				{
					throw new DesignerException("Action branch {0} has not been linked", _jumpToId);
				}
				_jumpToActionBranch.Execute(eventParameters);
			}
		}
		#endregion
		#region IPortOwner Members

		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Branch:{0}, Jump to:{1}", BranchId, _jumpToId);
			}
		}

		#endregion
		#region ISerializationProcessor Members

		public override void OnDeserialization(XmlNode objectNode)
		{
			if (_jumpToId != 0)
			{
				if (_jumpToId == FirstActionId)
				{
					_jumpToId = 0;
				}
			}
		}

		#endregion
	}
	public class AB_ActionGroup : AB_ActionString, IActionGroup
	{
		private List<ComponentIcon> _iconList;
		public AB_ActionGroup(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_ActionGroup(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		public override LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconLocal cil = ci as ComponentIconLocal;
					if (cil != null && cil.LocalPointer != null)
					{
						if (cil.LocalPointer.MemberId == id)
						{
							return cil.LocalPointer;
						}
					}
				}
			}
			return base.GetLocalVariable(usedBranches, id);
		}
		#region IActionGroup Members


		public string GroupName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Action list {0}", Name);
			}
		}

		public Type ViewerHolderType
		{
			get
			{
				return typeof(ActionGroupDesignerHolder);
			}
		}

		public UInt32 GroupId
		{
			get
			{
				return (UInt32)this.BranchId;
			}
		}

		public bool IsSubGroup
		{
			get
			{
				return true;
			}
		}

		public bool GroupFinished { get; set; }
		public void ResetGroupId(UInt32 groupId)
		{
			this.BranchId = groupId;
		}
		[Browsable(false)]
		public List<ComponentIcon> ComponentIconList
		{
			get
			{
				if (_iconList == null)
				{
					_iconList = new List<ComponentIcon>();
				}
				return _iconList;
			}
			set
			{
				_iconList = value;
			}
		}
		#endregion
	}
}
