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
using System.CodeDom;
using MathExp;
using System.ComponentModel;
using System.Collections.Specialized;
using XmlUtility;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// a list actions as a single action
	/// </summary>
	public abstract class AB_Squential : AB_SingleActionBlock, IActionListHolder
	{
		#region fields and constructors
		//only a single starting point is allowed for this BranchList,
		//_list is a list of ActionBranch all linked together to form one thread 
		private BranchList _list;
		public AB_Squential(BranchList actions)
			: base(actions.ActionsHolder)
		{
			_list = actions;
		}
		public AB_Squential(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_Squential(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_Squential(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Methods
		public override ActionBranch TopAction()
		{
			if (_list != null && _list.Count > 0)
			{
				ActionBranch topAb = null;
				foreach (ActionBranch ab in _list)
				{
					if (topAb == null)
					{
						topAb = ab;
					}
					else
					{
						ActionBranch a = ab.TopAction();
						if (a.Location.Y < topAb.Location.Y)
						{
							topAb = a;
						}
					}
				}
				return topAb;
			}
			return this;
		}
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_list != null)
			{
				_list.FindActionsByOwnerType<T>(results, usedMethods);
			}
		}
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_list != null)
			{
				_list.GetActionsUseLocalVariable(varId, actions);
			}
		}
		protected override int OnGetActionCount(List<UInt32> usedBranches)
		{
			if (_list != null)
			{
				return _list.GetActionCount();
			}
			return 1;
		}

		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (_list != null)
			{
				_list.GetActionNames(sc);
			}
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			if (_list != null)
			{
				_list.EstablishObjectOwnership();
			}
		}
		public override bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return false;
			usedBranches.Add(this.BranchId);
			if (_list != null)
			{
				return _list.IsActionUsed(actId, usedBranches);
			}
			return false;
		}
		public override IAction GetActionById(UInt32 id, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return null;
			usedBranches.Add(this.BranchId);
			if (_list != null)
			{
				return _list.GetActionById(id, usedBranches);
			}
			return null;
		}
		public override ActionPortIn GetActionInport(List<UInt32> used, UInt32 portId, UInt32 portInstanceId)
		{
			ActionPortIn ai = base.GetActionInport(used, portId, portInstanceId);
			if (ai == null)
			{
				if (!used.Contains(BranchId))
				{
					used.Add(BranchId);
					if (_list != null)
					{
						ai = _list.GetActionInport(used, portId, portInstanceId);
					}
				}
			}
			return ai;
		}
		public override void MakePortLinkForSingleThread(List<UInt32> used, BranchList branch)
		{
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);
				base.MakePortLinkForSingleThread(used, branch);
				if (_list != null)
				{
					foreach (ActionBranch ab in _list)
					{
						ab.MakePortLinkForSingleThread(used, branch);
					}
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
				if (_list != null)
				{
					v = _list.GetLocalVariable(usedBranches, id);
				}
			}
			return v;
		}
		public override void SetIsMainThreadForSubBranches(List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_list != null)
			{
				_list.SetIsMainThreadForSubBranches(usedBranches, this.IsMainThread);
			}
		}
		public BranchList GetActionsClone()
		{
			if (_list != null)
			{
				return (BranchList)_list.Clone();
			}
			return null;
		}
		[Browsable(false)]
		public void RemoveLocalVariable(ComponentIconLocal v, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			ISubMethod af = this as ISubMethod;
			if (af != null)
			{
				af.ComponentIconList.Remove(v);
				if (v.ClassPointer != null)
				{
					ComponentIconLocal cx = null;
					foreach (ComponentIcon c in af.ComponentIconList)
					{
						ComponentIconLocal c0 = c as ComponentIconLocal;
						if (c0 != null && c0.ClassPointer != null)
						{
							if (c0.ClassPointer.IsSameObjectRef(v.ClassPointer))
							{
								cx = c0;
								break;
							}
						}
					}
					if (cx != null)
					{
						af.ComponentIconList.Remove(cx);
					}
				}
			}
			BranchList acts = ActionList;
			if (acts != null)
			{
				foreach (ActionBranch ab in acts)
				{
					AB_Squential abs = ab as AB_Squential;
					if (abs != null)
					{
						abs.RemoveLocalVariable(v, usedBranches);
					}
				}
			}
		}
		/// <summary>
		/// convert ActionnString to ActionGroup to be displayed in a debug viewer.
		/// it should appear as a deeper level call stack for a same thread
		/// </summary>
		/// <param name="branchId">the given branch id to be included in the result group</param>
		/// <returns>the action group containing the given branch id</returns>
		public virtual IActionGroup GetThreadGroup(UInt32 branchId)
		{
			if (this.ContainsAction(branchId))
			{
				List<UInt32> used = new List<uint>();
				AB_ActionGroup g = new AB_ActionGroup(ActionsHolder);
				g.SetOwnerMethod(used, this.Method);
				g.Name = this.Name;
				g.Description = this.Description;
				g.BranchId = this.BranchId;//the calling function may change it according to the situations
				g.ActionList = GetActionsClone();
				return g;
			}
			return null;
		}
		/// <summary>
		/// find action methods of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public override void FindMethodByType<T>(List<T> results)
		{
			if (_list != null && _list.Count > 0)
			{
				_list.FindMethodByType<T>(results);
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_list != null && _list.Count > 0)
			{
				_list.FindItemByType<T>(results);
			}
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_list != null && _list.Count > 0)
			{
				return _list[_list.Count - 1].AllBranchesEndWithMethodReturnStatement();
			}
			return false;
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
			if (a != null)
			{
				return a;
			}
			if (_list != null)
			{
				return _list.GetBranchInGroup(id, ref group);
			}
			return null;
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			bool bRet = false;
			if (_list != null)
			{
				ActionBranch p = previousAction;
				ActionBranch n;
				_list.ResetBeforeCompile();
				for (int i = 0, j = 1; i < _list.Count; i++, j++)//ActionBranch a in _list)
				{
					if (j < _list.Count)
						n = _list[j];
					else
						n = nextAction;
					if (!_list[i].IsCompiled)
					{
						bRet = _list[i].ExportCode(p, n, compiler, method, statements);
					}
					p = _list[i];
				}
			}
			return bRet;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			if (_list != null)
			{
				ActionBranch p = previousAction;
				ActionBranch n;
				_list.ResetBeforeCompile();
				for (int i = 0, j = 1; i < _list.Count; i++, j++)
				{
					if (j < _list.Count)
						n = _list[j];
					else
						n = nextAction;
					if (!_list[i].IsCompiled)
					{
						bRet = _list[i].ExportJavaScriptCode(p, n, jsCode, methodCode, data);
					}
					p = _list[i];
				}
			}
			return bRet;
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			if (_list != null)
			{
				ActionBranch p = previousAction;
				ActionBranch n;
				_list.ResetBeforeCompile();
				for (int i = 0, j = 1; i < _list.Count; i++, j++)//ActionBranch a in _list)
				{
					if (j < _list.Count)
						n = _list[j];
					else
						n = nextAction;
					if (!_list[i].IsCompiled)
					{
						bRet = _list[i].ExportPhpScriptCode(p, n, jsCode, methodCode, data);
					}
					p = _list[i];
				}
			}
			return bRet;
		}
		public override void SetBranchOwner()
		{
			if (_list != null)
			{
				foreach (ActionBranch a in _list)
				{
					a.OwnerBranch = this;
					a.SetBranchOwner();
				}
			}
		}
		public override void SetWithinLoop()
		{
			if (_list != null)
			{
				foreach (ActionBranch a in _list)
				{
					a.WithinLoop = true;
				}
			}
		}
		public CodeStatement[] ExportStatements(ILimnorCodeCompiler compiler, IMethodCompile methodToCompile)
		{
			List<CodeStatement> ss = new List<CodeStatement>();
			//TBD: create statements for _list
			return ss.ToArray();
		}

		public override void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);
				base.SetOwnerMethod(used, m);
				if (_list != null)
				{
					_list.SetOwnerMethod(used, m);
				}
			}
		}
		public override void LinkJumpBranches()
		{
			if (_list != null)
			{
				_list.LinkJumpBranches();
			}
		}
		public override bool ContainsAction(UInt32 branchId)//ActionBranch branch)
		{
			if (base.ContainsAction(branchId))
			{
				return true;
			}
			if (_list != null)
			{
				return _list.ContainsAction(branchId);
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
			if (_list != null)
			{
				ab = _list.SearchBranchById(branchId);
				if (ab != null)
				{
					return ab;
				}
			}
			return null;
		}
		public override void ClearLinkPortIDs()
		{
			if (_list != null && _list.Count > 0)
			{
				_list[0].ClearLinkPortIDs();
				_list[_list.Count - 1].ClearLinkPortIDs();
			}
		}
		public override void AdjustInOutPorts()
		{
			base.AdjustInOutPorts();
			if (_list != null && _list.Count > 0)
			{
				_list[0].AdjustInOutPorts();
				_list[_list.Count - 1].AdjustInOutPorts();
			}
		}
		public override void LinkJumpedBranches(BranchList branches)
		{
			if (_list != null)
			{
				foreach (ActionBranch br in _list)
				{
					br.LinkJumpedBranches(branches);
				}
			}
		}
		public override void RemoveOutOfGroupBranches(BranchList branches)
		{
			if (_list != null)
			{
				List<ActionBranch> l = new List<ActionBranch>();
				foreach (ActionBranch ab in _list)
				{
					if (!branches.ContainsAction(ab.BranchId))
					{
						l.Add(ab);
					}
				}
				foreach (ActionBranch ab in l)
				{
					_list.Remove(ab);
				}
			}
		}
		/// <summary>
		/// set NextActions and PreviousActions properties before compiling
		/// </summary>
		public override void LinkActions(BranchList branches)
		{
			if (_list != null)
			{
				for (int i = 1; i < _list.Count; i++)
				{
					MathNode.Trace("link action string [{0},{1}] <-> [{2},{3}]", _list[i - 1].BranchId, _list[i - 1].Name, _list[i].BranchId, _list[i].Name);
					_list[i - 1].SetNextAction(_list[i]);
					_list[i].SetPreviousAction(_list[i - 1]);
				}
			}
		}
		public override void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches)
		{
			if (_list != null)
			{
				if (!usedBranches.Contains(this.BranchId))
				{
					usedBranches.Add(this.BranchId);
					foreach (ActionBranch b in _list)
					{
						b.SetActions(actions, usedBranches);
					}
				}
			}
		}
		/// <summary>
		/// create action components and make port links
		/// </summary>
		/// <param name="designer"></param>
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (!used.Contains(BranchId))
			{
				used.Add(BranchId);
				//this logic assumes _list is a single thread action string to be executed one by one
				if (_list != null && _list.Count > 0)
				{
					_list.LoadToDesignerAsSingleThread(designer);
				}
				return true;
			}
			return false;
		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBraches)
		{
			if (_list != null && _list.Count > 0)
			{
				if (!usedBraches.Contains(this.BranchId))
				{
					usedBraches.Add(this.BranchId);
					foreach (ActionBranch ab in _list)
					{
						ab.LoadActionData(pointer, usedBraches);
					}
				}
			}
		}
		public ActionBranch GetNextBranchById(UInt32 id)
		{
			if (_list != null)
			{
				return _list.GetNextBranchById(id);
			}
			return null;
		}
		public void AppendAction(ActionBranch a)
		{
			if (_list == null)
			{
				_list = new BranchList(this.ActionsHolder);
			}
			if (a.IsMergingPoint)
			{
				if (_list.Count > 0)
				{
					throw new DesignerException("Do not append a branching point to sequential actions");
				}
			}
			_list.Add(a);
		}
		public override void GetActionIDs(List<UInt32> actionIDs, List<UInt32> usedBraches)
		{
			if (_list != null)
			{
				if (!usedBraches.Contains(this.BranchId))
				{
					usedBraches.Add(this.BranchId);
					foreach (ActionBranch ab in _list)
					{
						ab.GetActionIDs(actionIDs, usedBraches);
					}
				}
			}
		}
		public override bool ContainsActionId(UInt32 actId, List<UInt32> usedBraches)
		{
			if (_list != null)
			{
				if (!usedBraches.Contains(this.BranchId))
				{
					usedBraches.Add(this.BranchId);
					foreach (ActionBranch ab in _list)
					{
						if (ab.ContainsActionId(actId, usedBraches))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBraches)
		{
			if (_list != null)
			{
				if (!usedBraches.Contains(this.BranchId))
				{
					usedBraches.Add(this.BranchId);
					foreach (ActionBranch ab in _list)
					{
						ab.GetActions(actions, usedBraches);
					}
				}
			}
		}
		public override void ReplaceAction(List<UInt32> usedBraches, uint oldId, IAction newAct)
		{
			if (_list != null)
			{
				if (!usedBraches.Contains(this.BranchId))
				{
					usedBraches.Add(this.BranchId);
					foreach (ActionBranch ab in _list)
					{
						ab.ReplaceAction(usedBraches, oldId, newAct);
					}
				}
			}
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			if (_list != null && _list.Count > 0)
			{
				_list[0].InputName = name;
				_list[0].InputType = type;
				_list[0].SetInputName(name, type);
			}
		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (_list != null)
			{
				_list.SetOwnerMethod(mc);
				_list.CollectSourceValues(taskId);
			}
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (_list != null)
			{
				if (client)
				{
					return _list.UseClientValues();
				}
				else
				{
					return _list.UseServerValues();
				}
			}
			return false;
		}
		public override void GetCustomMethods(List<UInt32> usedBranches, List<MethodClass> list)
		{
			if (_list != null)
			{
				_list.GetCustomMethods(list);
			}
		}
		#endregion
		#region Properties
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
				if (_list != null && _list.Count > 0)
				{
					return _list[0].UseInput;
				}
				return false;
			}
		}
		/// <summary>
		/// indicates whether the action branch has action output.
		/// </summary>
		[Browsable(false)]
		public override bool HasOutput
		{
			get
			{
				if (_list != null && _list.Count > 0)
				{
					return _list[_list.Count - 1].HasOutput;
				}
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
				if (_list != null && _list.Count > 0)
				{
					return _list[_list.Count - 1].OutputType;
				}
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
				if (_list != null && _list.Count > 0)
				{
					return _list[_list.Count - 1].OutputCodeName;
				}
				return null;
			}
		}
		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				if (_list == null)
					return true;
				if (_list.Count == 0)
					return true;
				return _list[_list.Count - 1].IsMethodReturn;
			}
		}
		[PropertyReadOrder(1000)]
		[Description("Task list for execution. A task may a simple action or an action group containing multiple actions and branches inside")]
		public BranchList ActionList
		{
			get
			{
				return _list;
			}
			set
			{
				_list = value;
			}
		}
		public override UInt32 FirstActionId
		{
			get
			{
				if (_list != null && _list.Count > 0)
				{
					return _list[0].BranchId;
				}
				return BranchId;
			}
		}
		public ActionBranch LastAction
		{
			get
			{
				if (_list != null && _list.Count > 0)
				{
					return _list[_list.Count - 1];
				}
				return null;
			}
		}
		public ActionBranch FirstAction
		{
			get
			{
				if (_list != null && _list.Count > 0)
				{
					return _list[0];
				}
				return null;
			}
		}
		public int ActionCount
		{
			get
			{
				if (_list == null)
					return 0;
				return _list.Count;
			}
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_Squential obj = (AB_Squential)base.Clone();
			if (_list != null)
			{
				obj.ActionList = (BranchList)_list.Clone();
			}
			return obj;
		}
		#endregion
		protected void ExecuteActions(List<ParameterClass> eventParameters)
		{
			if (_list != null)
			{
				foreach (ActionBranch a in _list)
				{
					a.Execute(eventParameters);
				}
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
		public override void Execute(List<ParameterClass> eventParameters)
		{
			ExecuteActions(eventParameters);
		}
	}
}
