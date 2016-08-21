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
using MathExp;
using System.ComponentModel;
using LimnorDesigner.MethodBuilder;
using XmlSerializer;
using System.CodeDom;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public enum EnumIconDrawType { None, Fill, Left, Center }
	/// <summary>
	/// single block may have multiple inports and only one outport
	/// </summary>
	public abstract class AB_SingleActionBlock : ActionBranch, ICloneable
	{
		#region fields and constructors
		public AB_SingleActionBlock(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_SingleActionBlock(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_SingleActionBlock(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Methods
		protected override int OnGetActionCount(List<UInt32> usedBranches)
		{
			return 1;
		}
		public override void SetIsMainThreadForSubBranches(List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
		}
		public override void ClearLinkPortIDs()
		{
			base.ClearLinkPortIDs();
			if (IsEndingPointByScope)
			{
				List<ActionPortOut> ports = this.OutPortList;
				if (ports != null && ports.Count > 0)
				{
					foreach (ActionPortOut p in ports)
					{
						p.LinkedPortID = 0;
					}
				}
			}
		}
		public override void AdjustInOutPorts()
		{
			if (IsEndingPoint)
			{
				ResetOutport();
			}
			if (IsStartingPoint)
			{
				ResetInport();
			}
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
			if (NextActions != null)
			{
				foreach (ActionBranch a in NextActions)
				{
					if (a.ContainsThread(id))
					{
						return true;
					}
				}
			}
			return false;
		}
		public override ActionBranch CollectActions(List<ActionViewer> scope, Dictionary<UInt32, ActionBranch> collected)
		{
			AB_ActionString th = new AB_ActionString(ActionsHolder);
			th.AppendAction(this);
			collected.Add(this.FirstActionId, th);
			ActionBranch an = NextActionInScope(scope);
			if (an == null)
			{
				this.IsEndingPointByScope = true;
				th.IsEndingPointByScope = true;
				return th;
			}
			this.IsEndingPointByScope = false;
			while (an != null)
			{
				if (an.IsMergingPoint || an.IsBranchingPoint)
				{
					th.JumpToStringId = an.BranchId;
					th.JumpTo = an;
					break;
				}
				else
				{
					th.AppendAction(an);
					ActionBranch an0 = an.NextActionInScope(scope);
					if (an0 == null)
					{
						an.IsEndingPointByScope = true;
					}
					if (an == an0)
					{
						break;
					}
					an = an0;
				}
			}
			return th;
		}
		#endregion
		#region Properties
		public override bool IsBranchingPoint
		{
			get { return false; }
		}
		#endregion
	}

	/// <summary>
	/// represent a single action
	/// for action viewer control and saving action pointer in methods
	/// </summary>
	public class AB_SingleAction : AB_SingleActionBlock, ISingleAction, ITypeScopeHolder, IScopeMethodHolder
	{
		#region fields and constructors
		private TaskID _actId;
		private IAction _actionData;//not serialized with this object
		private EnumIconDrawType _imgLayout = EnumIconDrawType.Left;
		private bool _showText = true;
		public AB_SingleAction(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_SingleAction(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_SingleAction(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region static methods
		public static void ActionFindItemByType<T>(IAction a, List<T> results)
		{
			if (a != null)
			{
				List<ParameterValue> pvs = a.ParameterValues;
				if (pvs != null && pvs.Count > 0)
				{
					foreach (ParameterValue p in pvs)
					{
						if (p.ValueType == EnumValueType.MathExpression)
						{
							MathNodeRoot mr = p.MathExpression as MathNodeRoot;
							if (mr != null)
							{
								mr.FindItemByType<T>(results);
							}
						}
						else if (p.ValueType == EnumValueType.Property)
						{
							object v = p.Property;
							if (v is T)
							{
								results.Add((T)v);
							}
						}
					}
				}
			}
		}
		public static void ActionSetInputName(IAction a, string name, DataTypePointer type)
		{
			if (a != null)
			{
				if (a.ActionCondition != null)
				{
					a.ActionCondition.SetActionInputName(name, type.BaseClassType);
				}
				List<ParameterValue> pvs = a.ParameterValues;
				if (pvs != null && pvs.Count > 0)
				{
					foreach (ParameterValue p in pvs)
					{
						if (p.ValueType == EnumValueType.MathExpression)
						{
							MathNodeRoot mr = p.MathExpression as MathNodeRoot;
							if (mr != null)
							{
								mr.SetActionInputName(name, type.BaseClassType);
							}
						}
						else if (p.ValueType == EnumValueType.Property)
						{
							ActionInput ai = p.Property as ActionInput;
							if (ai != null)
							{
								ai.SetActionInputName(name, type);
							}
						}
					}
				}
			}
		}
		public static bool ActionUseInput(IAction a)
		{
			if (a != null)
			{
				List<ParameterValue> pvs = a.ParameterValues;
				if (pvs != null && pvs.Count > 0)
				{
					foreach (ParameterValue p in pvs)
					{
						if (p.ValueType == EnumValueType.MathExpression)
						{
							MathNodeRoot mr = p.MathExpression as MathNodeRoot;
							if (mr != null)
							{
								return mr.UseInput;
							}
						}
						else if (p.ValueType == EnumValueType.Property)
						{
							ActionInput ai = p.Property as ActionInput;
							if (ai != null)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
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
				return ActionUseInput(_actionData);
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
				if (_actionData != null && _actionData.ActionMethod != null)
				{
					if (!(_actionData.ActionMethod.NoReturn))
					{
						if (_actionData.ActionMethod.MethodPointed.MethodReturnType != null)
						{
							if (!typeof(void).Equals(_actionData.ActionMethod.MethodPointed.MethodReturnType.ParameterLibType))
							{
								return true;
							}
						}
					}
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
				if (_actionData != null && _actionData.ActionMethod != null && !_actionData.ActionMethod.NoReturn)
				{
					if (_actionData.ActionMethod.MethodPointed.MethodReturnType != null)
					{
						DataTypePointer dp = _actionData.ActionMethod.MethodPointed.MethodReturnType as DataTypePointer;
						if (dp != null)
						{
							return dp;
						}
						return new DataTypePointer(new TypePointer(_actionData.ActionMethod.MethodPointed.MethodReturnType.ParameterLibType));
					}
				}
				return new DataTypePointer(new TypePointer(typeof(void)));
			}
		}
		/// <summary>
		/// this is called only when this is the last action
		/// </summary>
		[Browsable(false)]
		public override bool IsMethodReturn
		{
			get
			{
				if (_actionData == null)
					return true;
				return _actionData.IsMethodReturn;
			}
		}
		public override bool IsNameReadOnly
		{
			get
			{
				if (_actionData is ActionMethodReturn)
					return true;
				return base.IsNameReadOnly;
			}
		}
		public virtual EnumIconDrawType IconLayout
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
		public virtual bool ShowText
		{
			get
			{
				return _showText;
			}
			set
			{
				_showText = value;
			}
		}
		public override bool IsBranchingPoint
		{
			get
			{
				return false;
			}
		}

		public TaskID ActionId
		{
			get
			{
				return _actId;
			}
			set
			{
				_actId = value;
			}
		}

		public override Type ViewerType
		{
			get
			{
				if (_actionData != null)
				{
					Type t = _actionData.ViewerType;
					if (t != null)
					{
						if (typeof(ActionViewer).IsAssignableFrom(t))
						{
							return t;
						}
					}
				}
				return typeof(ActionViewerSingleAction);
			}
		}
		public virtual string ActionDisplay
		{
			get
			{
				if (_actionData != null)
					return _actionData.Display;//.ActionName;
				return "";
			}
		}
		public virtual bool CanEditAction
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
				if (this.ActionData == null)
				{
					MathNode.LastValidationError = "ActionData is null for AB_SingleAction";
				}
				return (this.ActionData != null);
			}
		}
		#endregion
		#region Methods
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionData != null)
			{
				if (!actions.ContainsKey(_actionData.ActionId))
				{
					if (ClassPointer.IsRelatedAction(varId, _actionData.CurrentXmlData))
					{
						actions.Add(_actionData.ActionId, _actionData);
					}
				}
			}
		}
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (_actionData != null)
			{
				if (!string.IsNullOrEmpty(_actionData.ActionName))
				{
					if (!sc.Contains(_actionData.ActionName))
					{
						sc.Add(_actionData.ActionName);
					}
				}
			}
		}

		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			if (_actionData != null)
			{
				_actionData.EstablishObjectOwnership(scope);
			}
		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionData != null)
			{
				mc.AddUploads(_actionData.GetClientProperties(taskId));
				mc.AddDownloads(_actionData.GetServerProperties(taskId));
			}
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_actionData != null)
				{
					IList<ISourceValuePointer> l;
					if (client)
					{
						l = _actionData.GetClientProperties(0);
					}
					else
					{
						l = _actionData.GetServerProperties(0);
					}
					if (l != null && l.Count > 0)
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
			if (_actionData != null)
			{
				MethodClass mc = _actionData.ActionMethod.MethodPointed as MethodClass;
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
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_actionData != null)
			{
				MethodClass.CollectActionsByOwnerType<T>(_actionData, results, usedMethods);
			}
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			if (this.ActionData != null)
			{
				MethodCreateValue mcv = this.ActionData.ActionMethod as MethodCreateValue;
				if (mcv != null)
				{
					LocalVariable lv = mcv.Owner as LocalVariable;
					if (lv != null)
					{
						Bitmap img = lv.ImageIcon as Bitmap;
						if (img != null)
						{
							IconLayout = EnumIconDrawType.Left;
							return img;
						}
					}
				}
				else
				{
					CustomConstructorPointer ccp = this.ActionData.ActionMethod as CustomConstructorPointer;
					if (ccp != null)
					{
						return Resources._constructor.ToBitmap();
					}
				}
			}
			return base.CreateIcon(g);
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			ActionSetInputName(_actionData, name, type);
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			if (_actionData == null)
			{
				return false;
			}
			return _actionData.IsMethodReturn;
		}
		public override bool ContainsActionId(UInt32 actId, List<UInt32> usedBraches)
		{
			if (_actId != null)
			{
				if (_actId.ActionId == actId)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// find action methods of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public override void FindMethodByType<T>(List<T> results)
		{
			if (_actionData != null && _actionData.ActionMethod != null)
			{
				object v = _actionData.ActionMethod;
				if (v is T)
				{
					results.Add((T)v);
				}
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			ActionFindItemByType<T>(_actionData, results);
		}
		public override bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches)
		{
			if (_actionData != null)
			{
				if (_actionData.ActionId == actId)
				{
					return true;
				}
			}
			if (_actId != null)
			{
				if (_actId.ActionId == actId)
				{
					return true;
				}
			}
			return false;
		}
		public override IAction GetActionById(UInt32 id, List<UInt32> usedBranches)
		{
			if (_actionData != null)
			{
				if (_actionData.ActionId == id)
				{
					return _actionData;
				}
			}
			if (_actId != null)
			{
				if (_actId.ActionId == id)
				{
					return _actId.Action;
				}
			}
			return null;
		}
		public override void GetActionIDs(List<UInt32> actionIDs, List<UInt32> usedBranches)
		{
			if (_actId != null)
			{
				if (!actionIDs.Contains(_actId.ActionId))
				{
					actionIDs.Add(_actId.ActionId);
				}
			}
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBranches)
		{
			if (_actionData != null)
			{
				bool found = false;
				foreach (IAction a in actions)
				{
					if (a.ActionId == _actionData.ActionId)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					actions.Add(_actionData);
				}
			}
		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBranches)
		{
			if (_actionData == null)
			{
				_actionData = _actId.LoadActionInstance(this.ActionsHolder);
			}
		}
		public override void ReplaceAction(List<UInt32> usedBranches, uint oldId, IAction newAct)
		{
			if (_actId.ActionId == oldId)
			{
				_actId.ActionId = newAct.ActionId;
				_actionData = newAct;
			}
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			ClassPointer list = compiler.ActionEventList;
			if (list != null)
			{
				if (_actionData == null)
				{
					_actionData = _actId.LoadActionInstance(this.ActionsHolder);
				}
			}
			if (_actionData != null)
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
				if (_actionData.ActionMethod != null && _actionData.ActionMethod.IsValid)
				{
					_actionData.ExportCode(this, nextAction, compiler, this.Method, method, statements, compiler.Debug);
				}
				if (_actionData.IsMethodReturn)
				{
					return true;
				}
				return this.IsMethodReturn;
			}
			return false;
		}
		public override bool OnExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			ClassPointer root = data.ActionEventList;
			if (root != null)
			{
				_actionData = _actId.LoadActionInstance(this.ActionsHolder);
			}
			if (_actionData != null)
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
				if (_actionData.ActionMethod != null && _actionData.ActionMethod.IsValid)
				{
					_actionData.ExportJavaScriptCode(this, nextAction, jsCode, methodToCompile, data);
				}
				if (_actionData.IsMethodReturn)
				{
					return true;
				}
				return this.IsMethodReturn;
			}
			return false;
		}
		public override bool OnExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
			ClassPointer list = data.ActionEventList;
			if (list != null && _actId != null)
			{
				_actionData = _actId.LoadActionInstance(this.ActionsHolder);
			}
			if (_actionData != null)
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
				if (_actionData.ActionMethod != null && _actionData.ActionMethod.IsValid)
				{
					_actionData.ExportPhpScriptCode(this, nextAction, jsCode, methodToCompile, data);
				}
				if (_actionData.IsMethodReturn)
				{
					return true;
				}
				return this.IsMethodReturn;
			}
			return false;
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void LinkJumpBranches()
		{
		}
		public override void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches)
		{
			//do not throw because this function will be called multiple times with different actions
			IAction a;
			if (_actId != null)
			{
				if (actions.TryGetValue(_actId.ActionId, out a))
				{
					if (a != null)
					{
						_actionData = a;
					}
				}
			}
		}
		public override void Execute(List<ParameterClass> eventParameters)
		{
			if (_actionData != null)
			{
				_actionData.Execute(eventParameters);
			}
		}
		public override void LinkJumpedBranches(BranchList branches)
		{
		}
		public override void RemoveOutOfGroupBranches(BranchList branches)
		{
		}
		/// <summary>
		/// set NextActions and PreviousActions properties before compiling
		/// </summary>
		public override void LinkActions(BranchList branches)
		{
		}
		/// <summary>
		/// create action component
		/// </summary>
		/// <param name="designer"></param>
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (_actionData == null)
			{
				ClassPointer root = designer.ActionEventCollection;
				if (root != null && _actId != null)
				{
					_actionData = _actId.LoadActionInstance(this.ActionsHolder);
				}
				if (_actionData == null && _actId != null)
				{
					_actionData = designer.DesignerHolder.GetAction(_actId);
				}
				if (_actionData == null)
				{
					DesignUtil.WriteToOutputWindowAndLog("Action data for {0} not found for [{1}] calling {2}.LoadToDesigner. You may delete the action from the method and re-create it.", ActionId, this.Name, this.GetType().Name);
				}
			}
			return designer.LoadAction(this);
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			AB_SingleAction obj = (AB_SingleAction)base.Clone();
			if (_actId != null)
			{
				obj.ActionId = (TaskID)_actId.Clone();
			}
			return obj;
		}

		#endregion
		#region IPortOwner Members

		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "action item {0}", _actId);
			}
		}

		#endregion
		#region IPropertiesWrapperOwner Members
		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			ICustomTypeDescriptor ct = _actionData as ICustomTypeDescriptor;
			if (ct != null)
			{
				return ct.GetProperties(attributes);
			}
			return TypeDescriptor.GetProperties(_actionData, attributes, true);
		}
		public override object GetPropertyOwner(int id, string propertyName)
		{
			return _actionData;
		}
		#endregion

		#region ISingleAction Members

		[ReadOnly(true)]
		public IAction ActionData
		{
			get
			{
				return _actionData;
			}
			set
			{
				_actionData = value;
			}
		}
		#endregion

		#region ITypeScopeHolder Members

		public DataTypePointer GetTypeScope(string name)
		{
			ITypeScopeHolder th = _actionData as ITypeScopeHolder;
			if (th != null)
			{
				return th.GetTypeScope(name);
			}
			return null;
		}

		#endregion

		#region IScopeMethodHolder Members

		public MethodClass GetScopeMethod()
		{
			return this.Method;
		}

		#endregion
	}
}
