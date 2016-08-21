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
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Specialized;
using VPL;
using Limnor.PhpComponents;
using Limnor.WebBuilder;
using System.Collections;

namespace LimnorDesigner.Action
{
	public interface IBranchingAction
	{
		void OnPaintBox(Graphics g, float x, float y);
		void OnPaintName(Graphics g, float boxWidth, float boxHeight);
		EnumDrawAction DrawingStyle { get; set; }
	}
	/// <summary>
	/// use a condition to determine jump to one of the two branches
	/// </summary>
	public class AB_ConditionBranch : ActionBranch, IBranchingAction
	{
		#region fields and constructors
		private UInt32 _ifJumpToId;
		private UInt32 _elseJumpToId;
		private ActionBranch _ifActions;
		private ActionBranch _elseActions;
		private ParameterValue _logicExpression;
		private EventHandler _onConditionChanged;
		private EnumDrawAction _drawingStyle = EnumDrawAction.ActionName;
		public AB_ConditionBranch(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_ConditionBranch(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_ConditionBranch(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
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
		[Description("Value used to determine program execution branching")]
		public ParameterValue Condition
		{
			get
			{
				if (_logicExpression == null)
				{
					_logicExpression = new ParameterValue(this);
					_logicExpression.ScopeMethod = Method;
					_logicExpression.DataType = new DataTypePointer(new TypePointer(typeof(bool)));
					_logicExpression.ValueType = EnumValueType.ConstantValue;
					_logicExpression.ConstantValue = new ConstObjectPointer("Condition", typeof(bool));
					_logicExpression.Name = Name;
					_logicExpression.SetParameterValueChangeEvent(_onConditionChanged);
				}
				return _logicExpression;
			}
			set
			{
				_logicExpression = value;
				if (_logicExpression != null)
				{
					_logicExpression.SetParameterValueChangeEvent(_onConditionChanged);
				}
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
				if (_logicExpression != null)
				{
					if (_logicExpression.ValueType == EnumValueType.Property)
					{
						ActionInput ai = _logicExpression.Property as ActionInput;
						if (ai != null)
						{
							return true;
						}
					}
					else if (_logicExpression.ValueType == EnumValueType.MathExpression)
					{
						MathNodeRoot mr = _logicExpression.MathExpression as MathNodeRoot;
						if (mr != null)
						{
							if (mr.UseInput)
							{
								return true;
							}
						}
					}
				}
				return false;
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
			if (_logicExpression != null)
			{
				IList<ISourceValuePointer> vars = _logicExpression.GetValueSources();
				if (vars != null && vars.Count > 0)
				{
					foreach (ISourceValuePointer p in vars)
					{
						IMethodPointerHolder mp = p as IMethodPointerHolder;
						if (mp != null)
						{
							IActionMethodPointer amp = mp.GetMethodPointer();
							if (amp != null)
							{
								if(amp.MethodDeclarer !=null)
								{
									if (amp.MethodDeclarer.ObjectInstance is T)
									{
										results.Add(amp);
									}
								}
							}
						}
					}
				}
			}
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
			if (_logicExpression != null)
			{
				IList<ISourceValuePointer> vars = _logicExpression.GetValueSources();
				if (vars != null && vars.Count > 0)
				{
					foreach (ISourceValuePointer p in vars)
					{

					}
				}
				//TBD
			}
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
			int n = 0;
			if (_ifActions != null)
			{
				n += _ifActions.GetActionCount(usedBranches);
			}
			if (_elseActions != null)
			{
				n += _elseActions.GetActionCount(usedBranches);
			}
			else
			{
				n = 1;
			}
			return n;
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
			if (_logicExpression != null)
			{
			}
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
		public void SetOnConditionChanged(EventHandler h)
		{
			_onConditionChanged = h;
			if (_logicExpression != null)
			{
				_logicExpression.SetParameterValueChangeEvent(h);
			}
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			if (!string.IsNullOrEmpty(name) && type != null && !type.IsVoid)
			{
				if (_logicExpression != null)
				{
					if (_logicExpression.ValueType == EnumValueType.Property)
					{
						ActionInput ai = _logicExpression.Property as ActionInput;
						if (ai != null)
						{
							ai.SetActionInputName(name, type);
						}
					}
					else if (_logicExpression.ValueType == EnumValueType.MathExpression)
					{
						MathNodeRoot mr = _logicExpression.MathExpression as MathNodeRoot;
						if (mr != null)
						{
							mr.SetActionInputName(name, type.BaseClassType);
						}
					}
				}
				else
				{
					_logicExpression = new ParameterValue(this);
				}
			}
			else
			{
#if DEBUG
				DesignUtil.WriteToOutputWindow("Calling AB_ConditionBranch.SetInputName with invalid parameters:{0},{1}", name, type);
#endif
			}
		}

		public override void CreateDefaultInputUsage()
		{
			//create an Input={Default} expression
			//_logicExpression must already initialized with proper data type
			if (_logicExpression == null)
			{
				throw new DesignerException("CreateDefaultInputUsage:logic expression not initialized");
			}
			if (_logicExpression.ValueType == EnumValueType.ConstantValue)
			{
				if (_logicExpression.ConstantValue != null && typeof(bool).Equals(_logicExpression.ConstantValue.BaseClassType))
				{
					if (_logicExpression.ConstantValue.Value != null)
					{
						if (typeof(bool).Equals(_logicExpression.ConstantValue.Value.GetType()))
						{
							if ((bool)(_logicExpression.ConstantValue.Value) == false)
							{
								_logicExpression.ValueType = EnumValueType.MathExpression;
								MathNodeRoot mr = new MathNodeRoot();
								_logicExpression.MathExpression = mr;
								mr.ScopeMethod = Method;
								MathNode compareNode;
								if (typeof(string).Equals(InputType.BaseClassType) || typeof(PhpString).Equals(InputType.BaseClassType) || typeof(JsString).Equals(InputType.BaseClassType))
								{
									compareNode = new MathNodeStringIsEmpty(mr);
								}
								else
								{
									LogicValueEquality ve = new LogicValueEquality(mr);
									compareNode = ve;
									MathNodeValue v = new MathNodeValue(ve);
									ve[1] = v;
									v.ValueType = new MathExp.RaisTypes.RaisDataType(InputType.BaseClassType);
									if (typeof(DialogResult).Equals(InputType.BaseClassType))
									{
										v.Value = DialogResult.OK;
									}
									else if (typeof(bool).Equals(InputType.BaseClassType))
									{
										v.Value = true;
									}
									else
									{
										v.Value = VPLUtil.GetDefaultValue(InputType.BaseClassType);
									}
								}
								mr[1] = compareNode;
								MathNodeActionInput ai = new MathNodeActionInput(compareNode);
								ai.SetActionInputName(InputName, InputType.BaseClassType);
								compareNode[0] = ai;
							}
						}
					}
				}
			}
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
				if (_logicExpression != null)
				{
					_logicExpression.SetCustomMethod(m);
					_logicExpression.SetOwnerAction(this);
				}
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
							if (start == end)
							{
								//already linked to it
								start = pi;
								while (start.PrevNode != null && start.PrevNode != end)
								{
									start = (LinkLineNode)start.PrevNode;
								}
							}
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
							if (start == end)
							{
								//already linked to it
								start = pi;
								while (start.PrevNode != null && start.PrevNode != end)
								{
									start = (LinkLineNode)start.PrevNode;
								}
							}
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
			Condition.DrawingFont = this.TextFont;
			Condition.DrawingBrush = new SolidBrush(this.TextColor);
			SizeF sz = Condition.CalculateDrawSize(g);
			Bitmap img = new Bitmap((int)sz.Width, (int)sz.Height);
			Graphics gImg = Graphics.FromImage(img);
			gImg.FillRectangle(new SolidBrush(Color.White), 0, 0, (int)sz.Width, (int)sz.Height);
			Condition.Draw(gImg);
			gImg.Dispose();
			return img;
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_ConditionBranch obj = (AB_ConditionBranch)base.Clone();
			obj.DrawingStyle = DrawingStyle;
			if (_logicExpression != null)
			{
				_logicExpression.SetOwnerAction(this);
				_logicExpression.SetCloneOwner(obj);
				obj.Condition = (ParameterValue)_logicExpression.Clone();
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
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_logicExpression != null)
			{
				List<ISourceValuePointer> l = new List<ISourceValuePointer>();
				_logicExpression.GetValueSources(l);
				if (taskId != 0)
				{
					foreach (ISourceValuePointer p in l)
					{
						p.SetTaskId(taskId);
					}
				}
				mc.AddValueSources(l);
			}
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
			if (usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_logicExpression != null)
				{
					List<ISourceValuePointer> l = new List<ISourceValuePointer>();
					_logicExpression.GetValueSources(l);
					if (l != null && l.Count > 0)
					{
						foreach (ISourceValuePointer p in l)
						{
							if (client)
							{
								if (p.IsWebClientValue())
								{
									return true;
								}
							}
							else
							{
								if (!p.IsWebClientValue())
								{
									return true;
								}
							}
						}
					}
				}
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
		public override LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
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
				throw new DesignerException("Condition action [{0}, {1}] goes to the same action", this.BranchId, this.Name);
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
				if (_logicExpression == null)
				{
					c = new CodePrimitiveExpression(true);
				}
				else
				{
					_logicExpression.SetDataType(typeof(bool));
					c = _logicExpression.GetReferenceCode(this.Method, sts, true);
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
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			if (_ifActions != null && _elseActions != null && _ifActions.BranchId == _elseActions.BranchId)
			{
				throw new DesignerException("Condition action [{0}, {1}] goes to the same action. Re-arrange your condition branches to avoid it.", this.BranchId, this.Name);
			}
			bool isGotoPoint = this.IsGotoPoint;
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
			try
			{
				string c;
				if (_logicExpression == null)
				{
					c = "true";
				}
				else
				{
					_logicExpression.SetDataType(typeof(bool));
					c = _logicExpression.CreateJavaScript(sts);
				}
				bool b1 = false;
				bool b2 = false;
				string indent = Indentation.GetIndent();
				sts.Add(indent);
				sts.Add("if(");
				sts.Add(c);
				sts.Add(") {\r\n");
				if (_ifActions != null)
				{
					if (_ifActions.IsGotoPoint)
					{
						sts.Add(indent);
						sts.Add("\treturn ");
						sts.Add(ActionBranch.IdToLabel(_ifActions.FirstActionId));
						sts.Add("();\r\n");
						if (!_ifActions.IsCompiled)
						{
							int idt = Indentation.GetIndentation();
							Indentation.SetIndentation(2);
							_ifActions.ExportJavaScriptCode(null, null, jsCode, sts, data);
							Indentation.SetIndentation(idt);
						}
						b1 = true;
					}
					else
					{
						Indentation.IndentIncrease();
						b1 = _ifActions.ExportJavaScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
					}
				}
				sts.Add(indent);
				sts.Add("}\r\n");
				if (_elseActions != null)
				{
					sts.Add(indent);
					sts.Add("else{\r\n");
					if (_elseActions.IsGotoPoint)
					{
						sts.Add(indent);
						sts.Add("\treturn ");
						sts.Add(ActionBranch.IdToLabel(_elseActions.FirstActionId));
						sts.Add("();\r\n");
						if (!_elseActions.IsCompiled)
						{
							int idt = Indentation.GetIndentation();
							Indentation.SetIndentation(2);
							_elseActions.ExportJavaScriptCode(null, null, jsCode, sts, data);
							Indentation.SetIndentation(idt);
						}
						b2 = true;
					}
					else
					{
						Indentation.IndentIncrease();
						b2 = _elseActions.ExportJavaScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
					}
					sts.Add(indent);
					sts.Add("}\r\n");
				}
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
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (IsCompiled)
			{
				return false;
			}
			//preventing of compiling it twice
			IsCompiled = true;
			if (_ifActions != null && _elseActions != null && _ifActions.BranchId == _elseActions.BranchId)
			{
				throw new DesignerException("Condition action [{0}, {1}] goes to the same action", this.BranchId, this.Name);
			}
			bool isGotoPoint = this.IsGotoPoint;
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
			try
			{
				string c;
				if (_logicExpression == null)
				{
					c = "true";
				}
				else
				{
					_logicExpression.SetDataType(typeof(bool));
					c = _logicExpression.CreatePhpScript(sts);
				}
				bool b1 = false;
				bool b2 = false;
				string indent = Indentation.GetIndent();
				sts.Add(indent);
				sts.Add("if(");
				sts.Add(c);
				sts.Add(") {\r\n");
				if (_ifActions != null)
				{
					if (_ifActions.IsGotoPoint)
					{
						sts.Add(indent);
						sts.Add("goto ");
						sts.Add(ActionBranch.IdToLabel(_ifActions.FirstActionId));
						sts.Add(";\r\n");
						if (!_ifActions.IsCompiled)
						{
							int idt = Indentation.GetIndentation();
							Indentation.SetIndentation(2);
							_ifActions.ExportPhpScriptCode(null, null, jsCode, sts, data);
							Indentation.SetIndentation(idt);
						}
						b1 = true;
					}
					else
					{
						Indentation.IndentIncrease();
						b1 = _ifActions.ExportPhpScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
					}
				}
				sts.Add(indent);
				sts.Add("}\r\n");
				if (_elseActions != null)
				{
					sts.Add(indent);
					sts.Add("else{\r\n");
					if (_elseActions.IsGotoPoint)
					{
						sts.Add(indent);
						sts.Add("goto ");
						sts.Add(ActionBranch.IdToLabel(_elseActions.FirstActionId));
						sts.Add(";\r\n");
						if (!_elseActions.IsCompiled)
						{
							int idt = Indentation.GetIndentation();
							Indentation.SetIndentation(2);
							_elseActions.ExportPhpScriptCode(null, null, jsCode, sts, data);
							Indentation.SetIndentation(idt);
						}
						b2 = true;
					}
					else
					{
						Indentation.IndentIncrease();
						b2 = _elseActions.ExportPhpScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
					}
					sts.Add(indent);
					sts.Add("}\r\n");
				}
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
			if (_logicExpression != null)
			{
				if (_logicExpression.ValueType == EnumValueType.Property)
				{
					object v = _logicExpression.Property;
					if (v is T)
					{
						results.Add((T)v);
					}
				}
				else if (_logicExpression.ValueType == EnumValueType.MathExpression)
				{
					MathNodeRoot mr = _logicExpression.MathExpression as MathNodeRoot;
					if (mr != null)
					{
						mr.FindItemByType<T>(results);
					}
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
			bool test = true;
			if (_logicExpression != null)
			{
				if (_logicExpression.ValueType == EnumValueType.ConstantValue)
				{
				}
				else if (_logicExpression.ValueType == EnumValueType.MathExpression)
				{
					if (_logicExpression.MathExpression != null)
					{
						MathNodeRoot mr = _logicExpression.MathExpression as MathNodeRoot;
						if (mr != null)
						{
							CompileResult cr = mr.DebugCompileUnit;
							cr.Execute();
							test = Convert.ToBoolean(cr.ReturnValue);
						}
					}
				}
				else if (_logicExpression.ValueType == EnumValueType.Property)
				{
				}
			}
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
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Condition {0}", _logicExpression);
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViewerIF); }
		}

		#endregion
		#region IPropertiesWrapperOwner Members

		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, false);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (string.CompareOrdinal(oProp.Name, "Condition") == 0)
				{
					newProps.Add(oProp);
					break;
				}
			}
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
			if (Condition != null)
			{
				System.Drawing.Drawing2D.GraphicsState st = g.Save();
				g.TranslateTransform(x / (float)2, y / (float)2);
				Condition.Draw(g);
				g.Restore(st);
			}
		}
		public void OnPaintName(Graphics g, float boxWidth, float boxHeight)
		{
			SizeF size = g.MeasureString(Name, TextFont);
			float x = (boxWidth - size.Width) / (float)2;
			if (x < 0) x = 0;
			float y = (boxHeight - size.Height) / (float)2;
			if (y < 0) y = 0;
			g.DrawString(Name, TextFont, new SolidBrush(TextColor), new RectangleF(x, y, boxWidth, boxHeight));

		}
		#endregion
	}
}
