/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public class AB_CastAs : ActionBranch, IBranchingAction
	{
		#region fields and constructors
		private UInt32 _ifJumpToId;
		private UInt32 _elseJumpToId;
		private ActionBranch _ifActions;
		private ActionBranch _elseActions;
		private IObjectPointer _source;
		private LocalVariable _target;
		private EnumDrawAction _drawingStyle = EnumDrawAction.ActionName;
		public AB_CastAs(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_CastAs(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_CastAs(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Properties
		[Description("Indicates how to display this action on the screen")]
		public EnumDrawAction DrawingStyle
		{
			get
			{
				return _drawingStyle;
			}
			set
			{
				_drawingStyle = value;
			}
		}
		/// <summary>
		/// indicates whether the action branch has action output.
		/// a condition branch does not have its own action
		/// </summary>
		[Browsable(false)]
		public override bool HasOutput
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// the data type for the action output
		/// </summary>
		[Browsable(false)]
		public override DataTypePointer OutputType
		{
			get
			{
				return new DataTypePointer(new TypePointer(typeof(void)));
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
				return null;
			}
		}
		/// <summary>
		/// last action in a thread?
		/// </summary>
		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				if (_ifJumpToId == 0 && _elseJumpToId == 0)
					return true;
				return false;
			}
		}
		public override bool IsBranchingPoint
		{
			get
			{
				return true;
			}
		}
		public UInt32 TrueJumpToId
		{
			get
			{
				return _ifJumpToId;
			}
			set
			{
				_ifJumpToId = value;
			}
		}
		public UInt32 FalseJumpToId
		{
			get
			{
				return _elseJumpToId;
			}
			set
			{
				_elseJumpToId = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public ActionBranch TrueActions
		{
			get
			{
				return _ifActions;
			}
			set
			{
				_ifActions = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public ActionBranch FalseActions
		{
			get
			{
				return _elseActions;
			}
			set
			{
				_elseActions = value;
			}
		}
		[Description("The source object to be used as target object")]
		public IObjectPointer SourceObject
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}
		[Description("The target object to be casted from the source object")]
		public LocalVariable TargetObject
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}
		/// <summary>
		/// indicates whether the action branch uses action input
		/// For each action branch, examine its ParameterValue objects. For example, the _condition of the AB_Condition.
		/// For each action, examine each ParameterValue for its parameters.
		/// For a ParameterValue, if the valueType is Property then check is the value is of ActionInout; if the valueType is MathExpression then search the MathNodeRoot to see if it contains MathNodeActionInput.
		/// </summary>
		public override bool UseInput
		{
			get
			{
				return false;
			}
		}
		public override bool IsValid
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region Methods
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				_ifActions.FindActionsByOwnerType<T>(results, usedBranches, usedMethods);
			}
			if (_elseActions != null)
			{
				_elseActions.FindActionsByOwnerType<T>(results, usedBranches, usedMethods);
			}
		}
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				_ifActions.GetActionsUseLocalVariable(usedBranches, varId, actions);
			}
			if (_elseActions != null)
			{
				_elseActions.GetActionsUseLocalVariable(usedBranches, varId, actions);
			}
		}
		protected override int OnGetActionCount(List<UInt32> usedBranches)
		{
			return 1;
		}
		protected override void OnVerifyJump(BranchList branch)
		{
			if (_ifJumpToId == 0)
			{
				if (OutPortList[0].LinkedInPort != null)
				{
					ActionBranch ab = OutPortList[0].LinkedInPort.PortOwner as ActionBranch;
					if (ab != null)
					{
						_ifJumpToId = ab.BranchId;
						_ifActions = ab;
					}
				}
			}
			if (_elseJumpToId == 0)
			{
				if (OutPortList[1].LinkedInPort != null)
				{
					ActionBranch ab = OutPortList[1].LinkedInPort.PortOwner as ActionBranch;
					if (ab != null)
					{
						_elseJumpToId = ab.BranchId;
						_elseActions = ab;
					}
				}
			}
		}
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (_ifActions != null)
			{
				_ifActions.GetActionNames(sc, usedBranches);
			}
			if (_elseActions != null)
			{
				_elseActions.GetActionNames(sc, usedBranches);
			}
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			if (_ifActions != null)
			{
				_ifActions.EstablishObjectOwnership(scope, usedBranches);
			}
			if (_elseActions != null)
			{
				_elseActions.EstablishObjectOwnership(scope, usedBranches);
			}
		}
		public override void SetIsMainThreadForSubBranches(List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				_ifActions.IsMainThread = this.IsMainThread;
				_ifActions.SetIsMainThreadForSubBranches(usedBranches);
			}
			if (_elseActions != null)
			{
				_elseActions.IsMainThread = this.IsMainThread;
				_elseActions.SetIsMainThreadForSubBranches(usedBranches);
			}
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
		}
		public override void CreateDefaultInputUsage()
		{
		}
		/// <summary>
		/// if this branch uses ParameterValue then use it to determine the type
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetParameterType(string name)
		{
			return new TypePointer(typeof(bool));
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_ifActions != null)
			{
				if (!_ifActions.AllBranchesEndWithMethodReturnStatement())
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			if (_elseActions != null)
			{
				if (!_elseActions.AllBranchesEndWithMethodReturnStatement())
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			return true;
		}
		/// <summary>
		/// find the smallest group containing the branch
		/// </summary>
		/// <param name="id"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public override ActionBranch GetBranchInGroup(UInt32 id, ref IActionGroup group)
		{
			ActionBranch a = base.GetBranchInGroup(id, ref group);
			if (a == null)
			{
				if (_ifActions != null)
				{
					a = _ifActions.GetBranchInGroup(id, ref group);
				}
			}
			if (a == null)
			{
				if (_elseActions != null)
				{
					a = _elseActions.GetBranchInGroup(id, ref group);
				}
			}
			return a;
		}
		/// <summary>
		/// find out whther the thread represented by the id is linked to one waiting point 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override bool ContainsThread(UInt32 id)
		{
			if (base.ContainsThread(id))
			{
				return true;
			}
			if (_ifActions != null)
			{
				if (_ifActions.ContainsThread(id))
				{
					return true;
				}
			}
			if (_elseActions != null)
			{
				if (_elseActions.ContainsThread(id))
				{
					return true;
				}
			}
			return false;
		}
		public override void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);
				base.SetOwnerMethod(used, m);
				//if (_target != null)
				//{
				//    _target.Owner = m;
				//}
				if (_ifActions != null)
				{
					_ifActions.SetOwnerMethod(used, m);
				}
				if (_elseActions != null)
				{
					_elseActions.SetOwnerMethod(used, m);
				}
			}
		}
		public override void LinkJumpBranches()
		{
			if (_ifActions != null)
			{
				_ifActions.LinkJumpBranches();
			}
			if (_elseActions != null)
			{
				_elseActions.LinkJumpBranches();
			}
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
			List<ActionBranch> list = new List<ActionBranch>();
			if (_ifActions != null)
			{
				_ifJumpToId = _ifActions.FirstActionId;
				List<ActionBranch> l = _ifActions.OnExport(branches, usedBranches);
				if (l != null && l.Count > 0)
				{
					list.AddRange(l);
				}
				if (!_ifActions.IsMergingPoint)
				{
					list.Add(_ifActions);
				}
			}
			else
			{
				_ifJumpToId = 0;
			}
			if (_elseActions != null)
			{
				_elseJumpToId = _elseActions.FirstActionId;
				List<ActionBranch> l = _elseActions.OnExport(branches, usedBranches);
				if (l != null && l.Count > 0)
				{
					list.AddRange(l);
				}
				if (!_elseActions.IsMergingPoint)
				{
					list.Add(_elseActions);
				}
			}
			else
			{
				_elseJumpToId = 0;
			}
			return list;
		}
		public override void InitializeBranches(BranchList branches)
		{
			if (_ifJumpToId != 0)
			{
				if (_ifActions == null)
				{
					_ifActions = branches.GetJumpToActionBranch(_ifJumpToId);
					if (_ifActions == null)
					{
						throw new DesignerException("Action for True branch not found (id={0})", _ifJumpToId);
					}
				}

			}
			else
			{
				_ifActions = null;
			}
			if (_elseJumpToId != 0)
			{
				if (_elseActions == null)
				{
					_elseActions = branches.GetJumpToActionBranch(_elseJumpToId);
					if (_elseActions == null)
					{
						throw new DesignerException("Action for False branch not found (id={0})", _elseJumpToId);
					}
				}
			}
			else
			{
				_elseActions = null;
			}
		}
		/// <summary>
		/// set the branch content to the jump-to branches by matching branch IDs
		/// it is called after serialization
		/// </summary>
		/// <param name="branches"></param>
		public override void LinkJumpedBranches(BranchList branches)
		{
			InitializeBranches(branches);

			if (_ifActions != null)
			{
				_ifActions.LinkJumpedBranches(branches);
			}
			if (_elseActions != null)
			{
				_elseActions.LinkJumpedBranches(branches);
			}
		}
		public override void RemoveOutOfGroupBranches(BranchList branches)
		{
			if (_ifActions != null)
			{
				_ifActions.RemoveOutOfGroupBranches(branches);
			}
			if (_elseActions != null)
			{
				_elseActions.RemoveOutOfGroupBranches(branches);
			}
		}
		/// <summary>
		/// set NextActions and PreviousActions properties before compiling
		/// </summary>
		public override void LinkActions(BranchList branches)
		{
			LinkJumpedBranches(branches);

			if (_ifActions != null)
			{
				this.SetNextAction(_ifActions);
				_ifActions.SetPreviousAction(this);
				_ifActions.LinkActions(branches);
			}
			if (_elseActions != null)
			{
				this.SetNextAction(_elseActions);
				_elseActions.SetPreviousAction(this);
				_elseActions.LinkActions(branches);
			}
		}
		public override bool ContainsAction(UInt32 branchId)
		{
			if (base.ContainsAction(branchId))
			{
				return true;
			}
			if (_ifActions != null)
			{
				if (_ifActions.ContainsAction(branchId))
				{
					return true;
				}
			}
			if (_elseActions != null)
			{
				if (_elseActions.ContainsAction(branchId))
				{
					return true;
				}
			}
			return false;
		}
		public override ActionBranch SearchBranchById(UInt32 branchId)
		{
			ActionBranch ab = base.SearchBranchById(branchId);
			if (ab != null)
			{
				return ab;
			}
			if (_ifActions != null)
			{
				ab = _ifActions.SearchBranchById(branchId);
				if (ab != null)
				{
					return ab;
				}
			}
			if (_elseActions != null)
			{
				ab = _elseActions.SearchBranchById(branchId);
				if (ab != null)
				{
					return ab;
				}
			}
			return null;
		}
		public override void AdjustInOutPorts()
		{
			if (IsStartingPoint)
			{
				ResetInport();
			}
			if (_ifActions == null)
			{
				List<ActionPortOut> ports = this.OutPortList;
				if (ports != null && ports.Count > 0)
				{
					ports[0].LinkedPortID = 0;
					if (ports[0].PrevNode != null)
					{
						((LinkLineNode)ports[0].PrevNode).SetPrevious(null);
					}
				}
			}
			else
			{
				_ifActions.AdjustInOutPorts();
			}
			if (_elseActions == null)
			{
				List<ActionPortOut> ports = this.OutPortList;
				if (ports != null && ports.Count > 1)
				{
					ports[1].LinkedPortID = 0;
					if (ports[1].PrevNode != null)
					{
						((LinkLineNode)ports[1].PrevNode).SetPrevious(null);
					}
				}
			}
			else
			{
				_elseActions.AdjustInOutPorts();
			}
		}
		public override void ClearLinkPortIDs()
		{
			base.ClearLinkPortIDs();
			if (_ifActions == null)
			{
				List<ActionPortOut> ports = this.OutPortList;
				if (ports != null && ports.Count > 0)
				{
					ports[0].LinkedPortID = 0;
				}
			}
			else
			{
				_ifActions.ClearLinkPortIDs();
			}
			if (_elseActions == null)
			{
				List<ActionPortOut> ports = this.OutPortList;
				if (ports != null && ports.Count > 1)
				{
					ports[1].LinkedPortID = 0;
				}
			}
			else
			{
				_elseActions.ClearLinkPortIDs();
			}
		}
		public override void OnBeforeLoadIntoDesigner(BranchList branches)
		{
			if (_ifActions != null)
			{
				_ifActions.IsNotForDesigner = true;
			}
			if (_elseActions != null)
			{
				_elseActions.IsNotForDesigner = true;
			}
		}
		/// <summary>
		/// create action components and make port links
		/// </summary>
		/// <param name="designer"></param>
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (designer.LoadAction(this))
			{
				if (_ifActions != null)
				{
					if (_ifActions.LoadToDesigner(used, designer))
					{
						//make port link
						if (this.OutPortList[0].LinkedInPort == null)
						{
							LinkLineNode end = this.OutPortList[0].End;
							ActionPortIn pi = _ifActions.GetInPort(this.OutPortList[0].LinkedPortID, this.OutPortList[0].LinkedPortInstanceID);
							if (pi == null)
							{
								throw new MathException("Out port {0} is linked to [{1},{2}] of Branch {3}; but [{1},{2}] is not among the in-ports of the branch", this.OutPortList[0].PortID, this.OutPortList[0].LinkedPortID, this.OutPortList[0].LinkedPortInstanceID, _ifActions.BranchId);
							}
							LinkLineNode start = pi.Start;
							end.SetNext(start);
							start.SetPrevious(end);
						}
					}
				}
				if (_elseActions != null)
				{
					if (_elseActions.LoadToDesigner(used, designer))
					{
						//make port link
						if (this.OutPortList[1].LinkedInPort == null)
						{
							LinkLineNode end = this.OutPortList[1].End;
							ActionPortIn pi = _elseActions.GetInPort(this.OutPortList[1].LinkedPortID, this.OutPortList[1].LinkedPortInstanceID);
							if (pi == null)
							{
								throw new MathException("Out port {0} is linked to [{1},{2}] of Branch {3}; but [{1},{2}] is not among the in-ports of the branch", this.OutPortList[1].PortID, this.OutPortList[1].LinkedPortID, this.OutPortList[1].LinkedPortInstanceID, _elseActions.BranchId);
							}
							LinkLineNode start = pi.Start;
							end.SetNext(start);
							start.SetPrevious(end);
						}
					}
				}
				return true;
			}
			return false;
		}
		public override ActionBranch CollectActions(List<ActionViewer> scope, Dictionary<UInt32, ActionBranch> collected)
		{
			_ifActions = null;
			_elseActions = null;
			List<ActionPortOut> ps = OutPortList;
			if (ps != null && ps.Count == 2)
			{
				if (ps[0].LinkedInPort != null)
				{
					ActionViewer av = ps[0].LinkedInPort.Owner as ActionViewer;
					if (av != null)
					{
						if (scope == null || scope.Contains(av))
						{
							ActionBranch a;
							if (!collected.TryGetValue(av.ActionObject.FirstActionId, out a))
							{
								a = av.ActionObject.CollectActions(scope, collected);
							}
							_ifActions = a;
						}
					}
				}
				if (ps[1].LinkedInPort != null)
				{
					ActionViewer av = ps[1].LinkedInPort.Owner as ActionViewer;
					if (av != null)
					{
						if (scope == null || scope.Contains(av))
						{
							ActionBranch a;
							if (!collected.TryGetValue(av.ActionObject.FirstActionId, out a))
							{
								a = av.ActionObject.CollectActions(scope, collected);
							}
							_elseActions = a;
						}
					}
				}
			}
			collected.Add(this.FirstActionId, this);
			return this;
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			StringBuilder sb = new StringBuilder();
			if (_source == null)
			{
				sb.Append("?");
			}
			else
			{
				sb.Append(_source.ExpressionDisplay);
			}
			sb.Append(" as ");
			if (_target == null)
			{
				sb.Append("?");
			}
			else
			{
				sb.Append(_target.ExpressionDisplay);
			}
			string s = sb.ToString();
			Brush br = new SolidBrush(this.TextColor);
			SizeF sz = g.MeasureString(s, this.TextFont);
			Bitmap img = new Bitmap((int)sz.Width, (int)sz.Height);
			Graphics gImg = Graphics.FromImage(img);
			gImg.FillRectangle(new SolidBrush(Color.White), 0, 0, (int)sz.Width, (int)sz.Height);
			gImg.DrawString(s, this.TextFont, br, (float)2, (float)2);
			gImg.Dispose();
			return img;
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_CastAs obj = (AB_CastAs)base.Clone();
			obj.DrawingStyle = DrawingStyle;
			if (_source != null)
			{
				obj._source = (IObjectPointer)_source.Clone();
			}
			if (_target != null)
			{
				obj._target = (LocalVariable)_target.Clone();
			}
			if (_ifActions != null)
			{
				obj.TrueActions = (ActionBranch)_ifActions.Clone();
			}
			if (_elseActions != null)
			{
				obj.FalseActions = (ActionBranch)_elseActions.Clone();
			}
			return obj;
		}
		#endregion
		#region ActionBranch
		public override LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (_target != null)
			{
				if (_target.MemberId == id)
				{
					return _target;
				}
			}
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			LocalVariable v = base.GetLocalVariable(usedBranches, id);
			if (v == null)
			{
				if (_ifActions != null)
				{
					v = _ifActions.GetLocalVariable(usedBranches, id);
				}
				if (v == null)
				{
					if (_elseActions != null)
					{
						v = _elseActions.GetLocalVariable(usedBranches, id);
					}
				}
			}
			return v;
		}
		public override bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return false;
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				if (_ifActions.IsActionUsed(actId, usedBranches))
					return true;
			}
			if (_elseActions != null)
			{
				if (_elseActions.IsActionUsed(actId, usedBranches))
					return true;
			}
			return false;
		}
		public override IAction GetActionById(UInt32 id, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return null;
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				IAction a = _ifActions.GetActionById(id, usedBranches);
				if (a != null)
				{
					return a;
				}
			}
			if (_elseActions != null)
			{
				IAction a = _elseActions.GetActionById(id, usedBranches);
				if (a != null)
				{
					return a;
				}
			}
			return null;
		}
		public override void GetActionIDs(List<UInt32> actionIDs, List<UInt32> usedBranches)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					_ifActions.GetActionIDs(actionIDs, usedBranches);
				}
				if (_elseActions != null)
				{
					_elseActions.GetActionIDs(actionIDs, usedBranches);
				}
			}
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBranches)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					_ifActions.GetActions(actions, usedBranches);
				}
				if (_elseActions != null)
				{
					_elseActions.GetActions(actions, usedBranches);
				}
			}
		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBranches)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					_ifActions.LoadActionData(pointer, usedBranches);
				}
				if (_elseActions != null)
				{
					_elseActions.LoadActionData(pointer, usedBranches);
				}
			}
		}
		public override bool ContainsActionId(UInt32 actId, List<UInt32> usedBranches)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					if (_ifActions.ContainsActionId(actId, usedBranches))
					{
						return true;
					}
				}
				if (_elseActions != null)
				{
					if (_elseActions.ContainsActionId(actId, usedBranches))
					{
						return true;
					}
				}
			}
			return false;
		}
		public override void ReplaceAction(List<uint> usedBranches, uint oldId, IAction newAct)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					_ifActions.ReplaceAction(usedBranches, oldId, newAct);
				}
				if (_elseActions != null)
				{
					_elseActions.ReplaceAction(usedBranches, oldId, newAct);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <returns></returns>
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			if (_ifActions != null && _elseActions != null && _ifActions.BranchId == _elseActions.BranchId)
			{
				throw new DesignerException("'Cast as' action [{0}, {1}] goes to the same action", this.BranchId, this.Name);
			}
			bool isGotoPoint = this.IsGotoPoint;
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
			try
			{
				CodeExpression c;
				if (_source == null || _target == null)
				{
					c = new CodePrimitiveExpression(false);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(_source.CodeName);
					sb.Append(" as ");
					sb.Append(_target.BaseClassType.FullName);
					CodeAssignStatement cas = new CodeAssignStatement();
					cas.Left = new CodeVariableReferenceExpression(_target.CodeName);
					cas.Right = new CodeSnippetExpression(sb.ToString());
					sts.Add(cas);
					CodeBinaryOperatorExpression cb = new CodeBinaryOperatorExpression();
					cb.Left = new CodeVariableReferenceExpression(_target.CodeName);
					cb.Operator = CodeBinaryOperatorType.IdentityInequality;
					cb.Right = new CodePrimitiveExpression(null);
					c = cb;
				}
				bool b1 = false;
				bool b2 = false;
				CodeConditionStatement ccs = new CodeConditionStatement();
				ccs.Condition = c;
				if (_ifActions != null)
				{
					if (_ifActions.IsGotoPoint)
					{
						CodeGotoStatement gotoCode = new CodeGotoStatement(ActionBranch.IdToLabel(_ifActions.FirstActionId));
						ccs.TrueStatements.Add(gotoCode);
						if (!_ifActions.IsCompiled)
						{
							_ifActions.ExportCode(null, null, compiler, method, null);
						}
						b1 = true;
					}
					else
					{
						b1 = _ifActions.ExportCode(previousAction, null, compiler, method, ccs.TrueStatements);
					}
				}
				if (_elseActions != null)
				{
					if (_elseActions.IsGotoPoint)
					{
						CodeGotoStatement gotoCode = new CodeGotoStatement(ActionBranch.IdToLabel(_elseActions.FirstActionId));
						ccs.FalseStatements.Add(gotoCode);
						if (!_elseActions.IsCompiled)
						{
							_elseActions.ExportCode(null, null, compiler, method, null);
						}
						b2 = true;
					}
					else
					{
						b2 = _elseActions.ExportCode(previousAction, null, compiler, method, ccs.FalseStatements);
					}
				}
				sts.Add(ccs);
				if (b1 && b2)
				{
					if (ms0 != null)
					{
						ms0.Completed = true;
					}
					return true;
				}
			}
			catch (Exception err)
			{
				throw new DesignerException(err, "Error compiling Condition action {0}. See inner exception for details", TraceInfo);
			}
			return false;
		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				_ifActions.CollectSourceValues(taskId, usedBranches, mc);
			}
			if (_elseActions != null)
			{
				_elseActions.CollectSourceValues(taskId, usedBranches, mc);
			}
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_ifActions != null)
				{
					if (_ifActions.UseClientServerValues(usedBranches, client))
					{
						return true;
					}
				}
				if (_elseActions != null)
				{
					if (_elseActions.UseClientServerValues(usedBranches, client))
					{
						return true;
					}
				}
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
			if (_ifActions != null)
			{
				_ifActions.GetCustomMethods(usedBranches, list);
			}
			if (_elseActions != null)
			{
				_elseActions.GetCustomMethods(usedBranches, list);
			}
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			//this action is not available in php
			return false;
		}
		/// <summary>
		/// javascript does not use cast
		/// </summary>
		/// <param name="previousAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="jsCode"></param>
		/// <param name="methodCode"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			//this action is not available in javascript
			return false;
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		/// <summary>
		/// find action methods of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public override void FindMethodByType<T>(List<T> results)
		{
			if (_ifActions != null)
			{
				_ifActions.FindMethodByType<T>(results);
			}
			if (_elseActions != null)
			{
				_elseActions.FindMethodByType<T>(results);
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_source != null)
			{
				if (_source is T)
				{
					results.Add((T)_source);
				}
			}

			if (_ifActions != null)
			{
				_ifActions.FindItemByType<T>(results);
			}
			if (_elseActions != null)
			{
				_elseActions.FindItemByType<T>(results);
			}
		}
		public override void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_ifActions != null)
			{
				_ifActions.SetActions(actions, usedBranches);
			}
			if (_elseActions != null)
			{
				_elseActions.SetActions(actions, usedBranches);
			}
		}
		public override void Execute(List<ParameterClass> eventParameters)
		{
			bool test = false;

			if (test)
			{
				if (_ifActions != null)
				{
					_ifActions.Execute(eventParameters);
				}
			}
			else
			{
				if (_elseActions != null)
				{
					_elseActions.Execute(eventParameters);
				}
			}
		}
		public override void InitializePorts(Control owner)
		{
			base.InitializePorts(owner);
			foreach (ActionPortOut p in OutPortList)
			{
				p.FixedLocation = true;
				p.SetRemoveLineJoint(false);
			}
			foreach (ActionPortIn p in InPortList)
			{
				p.FixedLocation = true;

			}
		}
		public override void InitializePortPositions(Control owner)
		{
			OutPortList[0].MoveTo(owner.Left, owner.Top + owner.Height / 2);
			OutPortList[0].SaveLocation();
			OutPortList[0].NextNode.Left = OutPortList[0].Left;
			OutPortList[1].MoveTo(owner.Left + owner.Width, owner.Top + owner.Height / 2);
			OutPortList[1].SaveLocation();
			OutPortList[1].NextNode.Left = OutPortList[1].Left;
			InPortList[0].MoveTo(owner.Left + owner.Width / 2, owner.Top);
			InPortList[0].SaveLocation();
			InPortList[0].PrevNode.Left = InPortList[0].Left;
		}
		public override uint OutportCount
		{
			get
			{
				return 2;
			}
		}
		public override string TraceInfo
		{
			get
			{
				string s1;
				if (_source == null)
				{
					s1 = "?";
				}
				else
				{
					s1 = _source.ExpressionDisplay;
				}
				string s2;
				if (_target == null)
				{
					s2 = "?";
				}
				else
				{
					s2 = _target.ExpressionDisplay;
				}
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0} as {1}", s1, s2);
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViewerCastAs); }
		}

		#endregion
		#region IPropertiesWrapperOwner Members

		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			return newProps;
		}
		public override object GetPropertyOwner(int id, string propertyName)
		{
			return this;
		}
		#endregion

		#region IBranchingAction Members

		public void OnPaintBox(Graphics g, float x, float y)
		{
			Brush br = new SolidBrush(this.TextColor);
			g.DrawString(TraceInfo, this.TextFont, br, x / (float)2, y / (float)2);
		}
		public void OnPaintName(Graphics g, float boxWidth, float boxHeight)
		{
			string s = TraceInfo;
			SizeF size = g.MeasureString(s, TextFont);
			float x = (boxWidth - size.Width) / (float)2;
			if (x < 0) x = 0;
			float y = (boxHeight - size.Height) / (float)2;
			if (y < 0) y = 0;
			g.DrawString(s, TextFont, new SolidBrush(TextColor), new RectangleF(x, y, boxWidth, boxHeight));

		}
		#endregion
	}
}
