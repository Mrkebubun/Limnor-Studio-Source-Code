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
using System.CodeDom;
using MathExp;
using System.Windows.Forms;
using ProgElements;
using System.Collections.Specialized;
using System.Collections;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// a list of actions forming a single string of actions, appears in the method editor as one single action
	/// </summary>
	public class AB_ActionList : AB_SingleActionBlock
	{
		#region fields and constructors
		private ActionList _actionList;
		private EnumIconDrawType _imgLayout = EnumIconDrawType.Left;
		public AB_ActionList(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_ActionList(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_ActionList(IActionListHolder ah)
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
				if (_actionList != null)
				{
					for (int i = 0; i < _actionList.Count; i++)
					{
						if (AB_SingleAction.ActionUseInput(_actionList[i].Action))
						{
							return true;
						}
					}
				}
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
		/// <summary>
		/// indicates whether the action branch has action output.
		/// </summary>
		[Browsable(false)]
		public override bool HasOutput
		{
			get
			{
				DataTypePointer dp = OutputType;
				if (dp != null)
				{
					return typeof(void).Equals(dp.BaseClassType);
				}
				return false;
			}
		}

		[Browsable(false)]
		public override string BaseActionName
		{
			get
			{
				return "ActionList";
			}
		}
		public ActionList Actions
		{
			get
			{
				if (_actionList == null)
				{
					_actionList = new ActionList();
				}
				return _actionList;
			}
			set
			{
				_actionList = value;
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
		#endregion
		#region Methods
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionList != null)
			{
				foreach (ActionItem ai in _actionList)
				{
					if (ai.Action != null)
					{
						MethodClass.CollectActionsByOwnerType<T>(ai.Action, results, usedMethods);
					}
				}
			}
		}
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action == null)
					{
						IAction a = this.ActionsHolder.GetActionInstance(_actionList[k].ActionId);
						if (a == null)
						{
						}
						else
						{
							_actionList[k].Action = a;
						}
					}
					if (_actionList[k].Action != null)
					{
						if (!actions.ContainsKey(_actionList[k].Action.ActionId))
						{
							if (ClassPointer.IsRelatedAction(varId, _actionList[k].Action.CurrentXmlData))
							{
								actions.Add(_actionList[k].Action.ActionId, _actionList[k].Action);
							}
						}
					}
				}
			}
		}
		
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (_actionList != null)
			{
				_actionList.GetActionNames(sc);
			}
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			if (_actionList != null)
			{
				_actionList.EstablishObjectOwnership(scope);
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
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					AB_SingleAction.ActionSetInputName(_actionList[k].Action, name, type);
				}
			}
		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBraches)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action == null)
					{
						IAction a = this.ActionsHolder.GetActionInstance(_actionList[k].ActionId);// pointer.GetAction(tid);
						if (a == null)
						{
							throw new DesignerException("Action [{0}] not found in class [{1}]", _actionList[k].ActionId, pointer.ClassId);
						}
						else
						{
							_actionList[k].Action = a;
						}
					}
				}
			}
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (!used.Contains(BranchId))
			{
				used.Add(BranchId);
				if (_actionList != null)
				{
					ClassPointer list = designer.ActionEventCollection;
					if (list != null)
					{
						for (int k = 0; k < _actionList.Count; k++)
						{
							TaskID tid = new TaskID(_actionList[k].ActionId, list.ClassId);
							_actionList[k].Action = GetActionInstance(_actionList[k].ActionId);// list.GetAction(tid);

							if (_actionList[k].Action == null)
							{
								_actionList[k].Action = designer.DesignerHolder.GetAction(tid);
								if (_actionList[k].Action == null)
								{
									DesignUtil.WriteToOutputWindowAndLog("Action data for {0} not found for [{1}] calling {2}.LoadToDesigner. You may delete it from the method and re-create it.", _actionList[k].ActionId, this.Name, this.GetType().Name);
								}
							}
						}
					}
				}
				return designer.LoadAction(this);
			}
			return false;
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_ActionList obj = (AB_ActionList)base.Clone();
			if (_actionList != null)
			{
				obj._actionList = (ActionList)_actionList.Clone();
			}
			return obj;
		}
		#endregion
		#region ActionBranch implementation
		public override DataTypePointer OutputType
		{
			get
			{
				DataTypePointer dp = new DataTypePointer(typeof(void));
				if (_actionList != null && _actionList.Count > 0)
				{
					if (_actionList[_actionList.Count - 1].Action != null && _actionList[_actionList.Count - 1].Action.ActionMethod.MethodPointed != null)
					{
						IParameter p = _actionList[_actionList.Count - 1].Action.ActionMethod.MethodPointed.MethodReturnType;
						if (p == null)
						{
							dp = new DataTypePointer(new TypePointer(typeof(void)));
						}
						else
						{
							dp = p as DataTypePointer;
							if (dp == null)
							{
								dp = new DataTypePointer(this.OwnerMethod.RootPointer);
								dp.SetDataType(p.ParameterLibType);
								dp.Name = p.Name;
							}
						}
					}
				}
				else
				{
					dp = new DataTypePointer(new TypePointer(typeof(void)));
				}
				return dp;
			}
		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						mc.AddUploads(_actionList[k].Action.GetClientProperties(taskId));
						mc.AddDownloads(_actionList[k].Action.GetServerProperties(taskId));
					}
				}
			}
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_actionList != null)
				{
					for (int k = 0; k < _actionList.Count; k++)
					{
						if (_actionList[k].Action != null)
						{
							IList<ISourceValuePointer> l;
							if (client)
							{
								l = _actionList[k].Action.GetClientProperties(0);
							}
							else
							{
								l = _actionList[k].Action.GetServerProperties(0);
							}
							if (l != null && l.Count > 0)
							{
								return true;
							}
						}
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
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						MethodClass mc = _actionList[k].Action.ActionMethod.MethodPointed as MethodClass;
						if (mc != null)
						{
							bool found = false;
							foreach (MethodClass m in list)
							{
								if (m.MethodID == mc.MethodID)
								{
									found = true;
									break;
								}
							}
							if (!found)
							{
								list.Add(mc);
							}
						}
					}
				}
			}
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			bool bRet = false;
			if (_actionList != null)
			{
				int last = _actionList.Count - 1;
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						bRet = _actionList[k].Action.IsMethodReturn;
						ActionBranch nt = null;
						if (k == last)
						{
							nt = nextAction;
							if (nt != null && nt.UseInput)
							{
								nt.InputName = this.OutputCodeName;
								nt.InputType = this.OutputType;
								nt.SetInputName(OutputCodeName, OutputType);
							}
						}
						_actionList[k].Action.ExportCode(this, nt, compiler, this.Method, method, statements, compiler.Debug);
					}
				}
			}
			return bRet;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			if (_actionList != null)
			{
				int last = _actionList.Count - 1;
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						bRet = _actionList[k].Action.IsMethodReturn;
						ActionBranch nt = null;
						if (k == last)
						{
							nt = nextAction;
							if (nt != null && nt.UseInput)
							{
								nt.InputName = this.OutputCodeName;
								nt.InputType = this.OutputType;
								nt.SetInputName(OutputCodeName, OutputType);
							}
						}
						_actionList[k].Action.ExportJavaScriptCode(this, nt, jsCode, methodCode, data);
					}
				}
			}
			return bRet;
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			if (_actionList != null)
			{
				int last = _actionList.Count - 1;
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						bRet = _actionList[k].Action.IsMethodReturn;
						ActionBranch nt = null;
						if (k == last)
						{
							nt = nextAction;
							if (nt != null && nt.UseInput)
							{
								nt.InputName = this.OutputCodeName;
								nt.InputType = this.OutputType;
								nt.SetInputName(OutputCodeName, OutputType);
							}
						}
						_actionList[k].Action.ExportPhpScriptCode(this, nt, jsCode, methodCode, data);
					}
				}
			}
			return bRet;
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_actionList != null)
			{
				int last = _actionList.Count - 1;
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						ActionBranch nt = null;
						if (k == last)
						{
							nt = nextAction;
							if (nt != null && nt.UseInput)
							{
								nt.InputName = this.OutputCodeName;
								nt.InputType = this.OutputType;
								nt.SetInputName(OutputCodeName, OutputType);
							}
						}
						_actionList[k].Action.ExportJavaScriptCode(this, nt, jsCode, methodCode, data);
					}
				}
			}
		}
		public override void Execute(List<ParameterClass> eventParameters)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						_actionList[k].Action.Execute(eventParameters);
					}
				}
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].Action != null)
					{
						AB_SingleAction.ActionFindItemByType<T>(_actionList[k].Action, results);
					}
				}
			}
		}
		public override void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					IAction a;
					if (actions.TryGetValue(_actionList[k].ActionId, out a))
					{
						if (a == null)
						{
							throw new DesignerException("Action not loaded for Id {0}", _actionList[k].ActionId);
						}
						_actionList[k].Action = a;
					}
					else
					{
						if (_actionList[k].Action == null)
						{
							if (Method != null)
							{
								a = Method.GetActionInstance(_actionList[k].ActionId);
								if (a != null)
								{
									_actionList[k].Action = a;
								}
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
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].ActionId == actId)
					{
						return true;
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
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].ActionId == id)
					{
						if (_actionList[k].Action != null)
						{
							return _actionList[k].Action;
						}
#if DEBUG
						MathNode.Log(null, "Action {0} not loaded for Action list branch", id);
#endif
					}
				}
			}
			return null;
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return;
			usedBranches.Add(this.BranchId);
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k] != null && _actionList[k].Action != null)
					{
						bool found = false;
						foreach (IAction a in actions)
						{
							if (a != null && a.ActionId == _actionList[k].Action.ActionId)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							actions.Add(_actionList[k].Action);
						}
					}
				}
			}
		}
		public override void ReplaceAction(List<UInt32> usedBranches, uint oldId, IAction newAct)
		{
			if (_actionList != null)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k].ActionId == oldId)
					{
						_actionList[k].ActionId = newAct.ActionId;
						_actionList[k].Action = newAct;
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
		public int ActionCount
		{
			get
			{
				if (_actionList == null)
					return 0;
				return _actionList.Count;
			}
		}
		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Action count: {0}", ActionCount);
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViewerActionList); }
		}

		#endregion
		#region AB_SingleActionBlock
		public override void GetActionIDs(List<uint> actionIDs, List<uint> usedBraches)
		{
			if (_actionList != null && _actionList.Count > 0)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (!actionIDs.Contains(_actionList[k].ActionId))
					{
						actionIDs.Add(_actionList[k].ActionId);
					}
				}
			}
		}

		public override bool ContainsActionId(uint actId, List<uint> usedBraches)
		{
			if (_actionList != null && _actionList.Count > 0)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (actId == _actionList[k].ActionId)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_actionList != null && _actionList.Count > 0)
			{
				if (_actionList[_actionList.Count - 1] == null)
				{
					return false;
				}
				if (_actionList[_actionList.Count - 1].Action == null)
				{
					return false;
				}
				return _actionList[_actionList.Count - 1].Action.IsMethodReturn;
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
			if (_actionList != null && _actionList.Count > 0)
			{
				for (int k = 0; k < _actionList.Count; k++)
				{
					if (_actionList[k] != null)
					{
						if (_actionList[k].Action != null)
						{
							object v = _actionList[k].Action.ActionMethod;
							if (v is T)
							{
								results.Add((T)v);
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
				if (_actionList != null && _actionList.Count > 0)
				{
					if (_actionList[_actionList.Count - 1] == null)
					{
						return false;
					}
					if (_actionList[_actionList.Count - 1].Action == null)
					{
						return false;
					}
					return _actionList[_actionList.Count - 1].Action.IsMethodReturn;
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
	}
}
