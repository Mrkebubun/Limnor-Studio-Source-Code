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
using MathExp;
using System.ComponentModel;
using System.CodeDom;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public class AB_DecisionTableActions : AB_SingleActionBlock
	{
		#region fields and constructors
		private DecisionTable _decisionTable;
		private EnumIconDrawType _imgLayout = EnumIconDrawType.Left;
		public AB_DecisionTableActions(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_DecisionTableActions(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_DecisionTableActions(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
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
				if (_decisionTable != null)
				{
					for (int i = 0; i < _decisionTable.ConditionCount; i++)
					{
						if (_decisionTable[i].Condition.UseInput)
						{
							return true;
						}
						if (_decisionTable[i].Actions != null)
						{
							for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
							{
								if (AB_SingleAction.ActionUseInput(_decisionTable[i].Actions[k].Action))
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
		}
		/// <summary>
		/// indicates whether the action branch has action output.
		/// a decision table cannot have return value
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
		public override string BaseActionName
		{
			get
			{
				return "decisionTable";
			}
		}
		public DecisionTable DecisionTable
		{
			get
			{
				if (_decisionTable == null)
				{
					_decisionTable = new DecisionTable();
				}
				return _decisionTable;
			}
			set
			{
				_decisionTable = value;
			}
		}
		public EnumIconDrawType IconLayout
		{
			get
			{
				return _imgLayout;
			}
			set
			{
				_imgLayout = value;
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
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (_decisionTable != null)
			{
				_decisionTable.GetActionNames(sc);
			}
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> used)
		{
			if (_decisionTable != null)
			{
				_decisionTable.EstablishObjectOwnership(scope);
			}
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					_decisionTable[i].Condition.SetActionInputName(name, type.BaseClassType);
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							AB_SingleAction.ActionSetInputName(_decisionTable[i].Actions[k].Action, name, type);
						}
					}
				}
			}
		}
		public override void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			base.SetOwnerMethod(used, m);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Condition != null)
					{
						_decisionTable[i].Condition.Project = m.Project;
					}
				}
			}

		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBraches)
		{
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].Action == null)
							{
								_decisionTable[i].Actions[k].Action = GetActionInstance(_decisionTable[i].Actions[k].ActionId);// pointer.GetAction(tid);
							}
						}
					}
				}
			}
		}
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Condition != null)
					{
						Dictionary<UInt32, IMethodPointerNode> ms = new Dictionary<uint, IMethodPointerNode>();
						_decisionTable[i].Condition.GetMethodPointers(ms);
						if (ms.Count > 0)
						{
							foreach (KeyValuePair<UInt32, IMethodPointerNode> mp in ms)
							{
								if (mp.Value.MethodExecuter is T)
								{
									IActionMethodPointer ia = mp.Value.MethodObject as IActionMethodPointer;
									if (ia != null)
									{
										results.Add(ia);
									}
								}
							}
						}
					}
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].Action == null)
							{
								_decisionTable[i].Actions[k].Action = GetActionInstance(_decisionTable[i].Actions[k].ActionId);
							}
							if (_decisionTable[i].Actions[k].Action != null)
							{
								MethodClass.CollectActionsByOwnerType<T>(_decisionTable[i].Actions[k].Action, results, usedMethods);
							}
						}
					}
				}
			}
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (!used.Contains(this.BranchId))
			{
				used.Add(BranchId);
				if (_decisionTable != null)
				{
					ClassPointer list = designer.ActionEventCollection;
					if (list != null)
					{
						for (int i = 0; i < _decisionTable.ConditionCount; i++)
						{
							if (_decisionTable[i].Actions != null)
							{
								for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
								{
									_decisionTable[i].Actions[k].Action = this.ActionsHolder.GetActionInstance(_decisionTable[i].Actions[k].ActionId);
									if (_decisionTable[i].Actions[k].Action == null)
									{
										DesignUtil.WriteToOutputWindowAndLog("Action data for {0} not found [{1}] calling {1}.LoadToDesigner. You may delete it from the method and re-create it.", _decisionTable[i].Actions[k].ActionId, this.GetType().Name);
									}
								}
							}
						}
					}
				}
				return designer.LoadAction(this);
			}
			return false;
		}
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k] != null && _decisionTable[i].Actions[k].Action != null)
							{
								if (!actions.ContainsKey(_decisionTable[i].Actions[k].Action.ActionId))
								{
									if (ClassPointer.IsRelatedAction(varId, _decisionTable[i].Actions[k].Action.CurrentXmlData))
									{
										actions.Add(_decisionTable[i].Actions[k].Action.ActionId, _decisionTable[i].Actions[k].Action);
									}
								}
							}
						}
					}
				}
			}
		}
		private void verifyActionObj(int k, int i)
		{
			if (_decisionTable[i].Actions[k].Action == null)
			{
				if (this.ActionsHolder != null)
				{
					_decisionTable[i].Actions[k].Action = this.ActionsHolder.GetActionInstance(_decisionTable[i].Actions[k].ActionId);
					if (_decisionTable[i].Actions[k].Action == null)
					{
						throw new DesignerException("Action not found for Action Id {0}, in decistion table, decision {1}, action {2}", _decisionTable[i].Actions[k].ActionId, i, k);
					}
				}
				else
				{
					throw new DesignerException("ActionsHolder is null for decision table {0}", this);
				}
			}
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_DecisionTableActions obj = (AB_DecisionTableActions)base.Clone();
			if (_decisionTable != null)
			{
				obj._decisionTable = (DecisionTable)_decisionTable.Clone();
			}
			return obj;
		}
		#endregion
		#region ActionBranch implementation
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null)
			{
				_decisionTable.CollectSourceValues(taskId, mc);
			}
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_decisionTable != null)
				{
					return _decisionTable.UseClientServerValues(client);
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
			if (_decisionTable != null)
			{
				_decisionTable.GetCustomMethods(list);
			}
		}
		public override void SetIsMainThreadForSubBranches(List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
		}
		
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (_decisionTable != null)
			{
				CodeConditionStatement ccs = null;
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					_decisionTable[i].Condition.PrepareForCompile(this.Method);
					CodeExpression c = _decisionTable[i].Condition.ExportCode(this.Method);
					if (ccs == null)
					{
						ccs = new CodeConditionStatement();
						statements.Add(ccs);
					}
					else
					{
						CodeConditionStatement ccs2 = new CodeConditionStatement();
						ccs.FalseStatements.Add(ccs2);
						ccs = ccs2;
					}
					ccs.Condition = c;
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							verifyActionObj(k, i);
							_decisionTable[i].Actions[k].Action.ExportCode(this, nextAction, compiler, this.Method, method, ccs.TrueStatements, compiler.Debug);
						}
					}
				}
			}
			return false;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					_decisionTable[i].Condition.PrepareForCompile(this.Method);
					string c = _decisionTable[i].Condition.CreateJavaScript(methodCode);
					if (i == 0)
					{
						methodCode.Add("if(");
					}
					else
					{
						methodCode.Add("else if(");
					}
					if (string.IsNullOrEmpty(c))
					{
						methodCode.Add("true");
					}
					else
					{
						methodCode.Add(c);
					}
					methodCode.Add(")\r\n{\r\n");
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							verifyActionObj(k, i);
							_decisionTable[i].Actions[k].Action.ExportJavaScriptCode(this, nextAction, jsCode, methodCode, data);
						}
					}
					methodCode.Add("\r\n}\r\n");
				}
			}
			return false;
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					_decisionTable[i].Condition.PrepareForCompile(this.Method);
					string c = _decisionTable[i].Condition.CreatePhpScript(methodCode);
					if (i == 0)
					{
						methodCode.Add("if(");
					}
					else
					{
						methodCode.Add("else if(");
					}
					if (string.IsNullOrEmpty(c))
					{
						methodCode.Add("true");
					}
					else
					{
						methodCode.Add(c);
					}
					methodCode.Add(")\r\n{\r\n");
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							verifyActionObj(k, i);
							_decisionTable[i].Actions[k].Action.ExportPhpScriptCode(this, nextAction, jsCode, methodCode, data);
						}
					}
					methodCode.Add("\r\n}\r\n");
				}
			}
			return false;
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void Execute(List<ParameterClass> eventParameters)
		{
			CompileResult cr = null;
			bool test = true;
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					cr = _decisionTable[i].Condition.DebugCompileUnit;
					cr.Execute();
					test = Convert.ToBoolean(cr.ReturnValue);
					if (test)
					{
						if (_decisionTable[i].Actions != null)
						{
							for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
							{
								_decisionTable[i].Actions[k].Action.Execute(eventParameters);
							}
						}
						break;
					}
				}
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					_decisionTable[i].Condition.FindItemByType<T>(results);
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							AB_SingleAction.ActionFindItemByType<T>(_decisionTable[i].Actions[k].Action, results);
						}
					}
				}
			}
		}
		public override void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return;
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							IAction a;
							if (actions.TryGetValue(_decisionTable[i].Actions[k].ActionId, out a))
							{
								if (a == null)
								{
									throw new DesignerException("Action not loaded for Id {0}", _decisionTable[i].Actions[k].ActionId);
								}
								_decisionTable[i].Actions[k].Action = a;
							}
						}
					}
				}
			}
		}
		public override bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return false;
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].ActionId == actId)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public override IAction GetActionById(UInt32 id, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return null;
			usedBranches.Add(this.BranchId);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].Action == null)
							{
								continue;
							}
							if (id == _decisionTable[i].Actions[k].Action.ActionId)
							{
								return _decisionTable[i].Actions[k].Action;
							}
						}
					}
				}
			}
			return null;
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBranches)
		{
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].Action == null)
							{
								continue;
							}
							bool found = false;
							foreach (IAction a in actions)
							{
								if (a.ActionId == _decisionTable[i].Actions[k].Action.ActionId)
								{
									found = true;
									break;
								}
							}
							if (!found)
							{
								actions.Add(_decisionTable[i].Actions[k].Action);
							}
						}
					}
				}
			}
		}
		public override void ReplaceAction(List<UInt32> usedBranches, uint oldId, IAction newAct)
		{
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k].ActionId == oldId)
							{
								_decisionTable[i].Actions[k].ActionId = newAct.ActionId;
								_decisionTable[i].Actions[k].Action = newAct;
							}
						}
					}
				}
			}
		}
		public override void InitializePorts(Control owner)
		{
			base.InitializePorts(owner);
			List<ActionPortOut> olist = OutPortList;
			foreach (ActionPortOut p in olist)
			{
				p.FixedLocation = true;
			}
			List<ActionPortIn> iList = InPortList;
			foreach (ActionPortIn p in iList)
			{
				p.FixedLocation = true;
			}
		}
		public override void InitializePortPositions(Control owner)
		{
			OutPortList[0].MoveTo(owner.Left, owner.Top + owner.Height / 2);
			OutPortList[0].SaveLocation();
			OutPortList[0].NextNode.Left = OutPortList[0].Left;
			InPortList[0].MoveTo(owner.Left + owner.Width / 2, owner.Top);
			InPortList[0].SaveLocation();
			InPortList[0].PrevNode.Left = InPortList[0].Left;
		}
		public override uint OutportCount
		{
			get
			{
				return 1;
			}
		}
		[ReadOnly(false)]
		public override List<ActionPortOut> OutPortList
		{
			get
			{
				return base.OutPortList;
			}
			set
			{
				base.OutPortList = value;
			}
		}
		[ReadOnly(false)]
		public override List<ActionPortIn> InPortList
		{
			get
			{
				return base.InPortList;
			}
			set
			{
				base.InPortList = value;
			}
		}
		[Browsable(false)]
		public int ConditionCount
		{
			get
			{
				if (_decisionTable == null)
					return 0;
				return _decisionTable.ConditionCount;
			}
		}
		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Decision table condition count: {0}", ConditionCount);
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViewerDecisionTable); }
		}

		#endregion

		#region AB_SingleActionBlock
		public override void GetActionIDs(List<uint> actionIDs, List<uint> usedBraches)
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k] != null)
							{
								if (!actionIDs.Contains(_decisionTable[i].Actions[k].ActionId))
								{
									actionIDs.Add(_decisionTable[i].Actions[k].ActionId);
								}
							}
						}
					}
				}
			}
		}

		public override bool ContainsActionId(uint actId, List<uint> usedBraches)
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k] != null)
							{
								if (actId == _decisionTable[i].Actions[k].ActionId)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null && _decisionTable[i].Actions.Count > 0)
					{
						if (_decisionTable[i].Actions[_decisionTable[i].Actions.Count - 1] == null)
						{
							return false;
						}
						if (_decisionTable[i].Actions[_decisionTable[i].Actions.Count - 1].Action == null)
						{
							return false;
						}
						return _decisionTable[i].Actions[_decisionTable[i].Actions.Count - 1].Action.IsMethodReturn;
					}
				}
			}
			return false;
		}

		public override void LinkJumpBranches()
		{
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

		public override void FindMethodByType<T>(List<T> results)
		{
			if (_decisionTable != null && _decisionTable.ConditionCount > 0)
			{
				for (int i = 0; i < _decisionTable.ConditionCount; i++)
				{
					if (_decisionTable[i].Actions != null)
					{
						for (int k = 0; k < _decisionTable[i].Actions.Count; k++)
						{
							if (_decisionTable[i].Actions[k] != null)
							{

								if (_decisionTable[i].Actions[k].Action != null)
								{
									object v = _decisionTable[i].Actions[k].Action.ActionMethod;
									if (v is T)
									{
										results.Add((T)v);
									}
								}
							}
						}
					}
				}
			}
		}
		public override bool IsMethodReturn
		{
			get
			{
				if (_decisionTable != null && _decisionTable.ConditionCount > 0)
				{
					if (_decisionTable[_decisionTable.ConditionCount - 1].Actions != null && _decisionTable[_decisionTable.ConditionCount - 1].Actions.Count > 0)
					{
						if (_decisionTable[_decisionTable.ConditionCount - 1].Actions[_decisionTable[_decisionTable.ConditionCount - 1].Actions.Count - 1] == null)
						{
							return false;
						}
						if (_decisionTable[_decisionTable.ConditionCount - 1].Actions[_decisionTable[_decisionTable.ConditionCount - 1].Actions.Count - 1].Action == null)
						{
							return false;
						}
						return _decisionTable[_decisionTable.ConditionCount - 1].Actions[_decisionTable[_decisionTable.ConditionCount - 1].Actions.Count - 1].Action.IsMethodReturn;
					}
				}
				return false;
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

		#region ISerializationProcessor Members

		public override void OnDeserialization(XmlNode objectNode)
		{
			if (_decisionTable != null && Method != null)
			{
				_decisionTable.SetScopeMethod(Method);
			}
		}

		#endregion
	}
}
