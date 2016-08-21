/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using XmlSerializer;
using VSPrj;
using System.CodeDom;
using System.Threading;
using System.Drawing.Design;
using System.Reflection;
using VPL;
using System.Xml;
using ProgElements;
using System.Collections.Specialized;
using System.Globalization;

namespace LimnorDesigner.Action
{
	public enum EnumBranchResult { Loop, Start }
	public enum EnumActionBreakStatus { None, Before, After }
	public enum EnumDrawAction { ActionName, Description, Image }
	/// <summary>
	/// represents actions in various linking arrangements: 
	/// simple list - SingleActionBlock
	/// conditional branching - ConditionBranch
	/// looping - LoopActions
	/// </summary>
	[LookupOwnerStatckOnRead]
	[SaveAsProperties(true)]
	public abstract class ActionBranch : ICloneable, IPortOwner, IWithProject, IPropertiesWrapperOwner, ISerializationProcessor, IActionContext, IOwnerProviderConstructorChild
	{
		#region fields and constructors
		private UInt32 _id;
		private UInt32 _startingBranchId;
		private bool _isStartingPoint;
		private bool _isMainThread;
		private string _name;
		private string _desc;
		private Rectangle _rect;
		private Font _font = new Font("Times New Roman", 12);
		private Color _color = Color.Black;
		private EnumActionBreakStatus _breakStatus = EnumActionBreakStatus.None;
		private Dictionary<UInt32, List<UInt32>> _waitingPointsThreads;
		/// <summary>
		/// port owner is ActionViewer
		/// </summary>
		private List<ActionPortIn> _inportList; //links to previous actions
		private List<ActionPortOut> _outportList; //conditional branches link to next actions, one port links to one next action
		private MethodClass _method; //all action-branches must belong to a custom method 
		private bool _allBranchesCompleted;
		//
		private bool _isLoopStart;
		protected ActionBranch(IActionsHolder actsHolder)
		{
			_method = actsHolder.OwnerMethod;
			ActionsHolder = actsHolder;
		}
		public ActionBranch(ActionBranch parentAction)
		{
			_method = parentAction.Method;
			ActionsHolder = parentAction.ActionsHolder;
		}
		public ActionBranch(IActionsHolder actsHolder, Point pos, Size size)
			: this(actsHolder)
		{
			Location = pos;
			Size = size;
		}
		public ActionBranch(IActionListHolder ah)
			: this(ah.ActionsHolder)
		{
		}

		#endregion
		#region Methods
		public static string IdToLabel(UInt32 id)
		{
			string s = "l_" + id.ToString("x");
			return s;
		}
		public static string IdToKey(UInt32 id)
		{
			string s = id.ToString("x");
			return s;
		}
		public IAction GetActionInstance(UInt32 actionId)
		{
			return ActionsHolder.GetActionInstance(actionId);
		}
		public virtual ActionBranch TopAction()
		{
			return this;
		}
		/// <summary>
		/// get action branch parameter. For example, the RepeatIndex for AB_ForLoop
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public virtual ActionBranchParameter GetActionBranchParameterByName(string parameterName)
		{
			return null;
		}
		/// <summary>
		/// if it is not a waiting point then it returns null
		/// if it is a waiting point then it returns independent 
		/// </summary>
		/// <returns></returns>
		public List<ActionBranch> GetPreviousThreadActions()
		{
			if (PreviousActions == null && PreviousActions.Count > 1)
			{
			}
			return null;
		}
		/// <summary>
		/// find the smallest group containing the branch
		/// </summary>
		/// <param name="id"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public virtual ActionBranch GetBranchInGroup(UInt32 id, ref IActionGroup group)
		{
			if (this.BranchId == id)
			{
				return this;
			}
			IActionGroup g = this as IActionGroup;
			if (g != null)
			{
				if (g.ActionList != null)
				{
					ActionBranch a = g.ActionList.GetBranchInGroup(id, ref g);
					if (a != null)
					{
						group = g;
						return a;
					}
				}
			}
			return null;
		}

		public virtual void InitializeBranches(BranchList branches)
		{
		}
		/// <summary>
		/// returns extra bracnhes to be added into branches
		/// </summary>
		/// <param name="branches"></param>
		/// <returns></returns>
		public virtual List<ActionBranch> OnExport(BranchList branches, List<UInt32> usedBranches)
		{
			return null;
		}
		/// <summary>
		/// make a signature to call Execute in a thread function 
		/// </summary>
		/// <param name="data"></param>
		public void ExecuteInThread(object data)
		{
			Execute((List<ParameterClass>)data);
		}
		/// <summary>
		/// for validating branching points
		/// </summary>
		/// <returns></returns>
		public UInt32 FindFirstWaitingPoint(List<UInt32> usedBranches)
		{
			if (this.IsWaitingPoint)
			{
				return this.BranchId;
			}
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (NextActions != null && NextActions.Count > 0)
				{
					foreach (ActionBranch a in NextActions)
					{
						UInt32 id = a.FindFirstWaitingPoint(usedBranches);
						if (id != 0)
						{
							return id;
						}
					}
				}
			}
			return 0;
		}
		public void EstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return;
			usedBranches.Add(this.BranchId);
			OnEstablishObjectOwnership(scope, usedBranches);
		}
		public void GetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return;
			usedBranches.Add(this.BranchId);
			OnGetActionNames(sc, usedBranches);
		}
		public int GetActionCount(List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
				return 0;
			usedBranches.Add(this.BranchId);
			return OnGetActionCount(usedBranches);
		}

		protected abstract int OnGetActionCount(List<UInt32> usedBranches);
		protected abstract void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches);
		protected abstract void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches);
		public abstract bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches);

		public abstract IAction GetActionById(UInt32 id, List<UInt32> usedBranches);
		public abstract void GetActionIDs(List<UInt32> actionIDs, List<UInt32> usedBranches);
		public abstract void GetActions(List<IAction> actions, List<UInt32> usedBranches);
		public abstract void ReplaceAction(List<UInt32> usedBranches, UInt32 oldId, IAction newAct);
		public abstract bool ContainsActionId(UInt32 actId, List<UInt32> usedBranches);
		public abstract void GetCustomMethods(List<UInt32> usedBranches, List<MethodClass> list);
		public abstract bool UseClientServerValues(List<UInt32> usedBranches, bool client);
		public abstract void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions);

		public virtual LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
			}
			return null;
		}
		public bool UseClientValues(List<UInt32> usedBranches)
		{
			return UseClientServerValues(usedBranches, true);
		}
		public bool UseServerValues(List<UInt32> usedBranches)
		{
			return UseClientServerValues(usedBranches, false);
		}
		public IAction GetActionById(UInt32 id)
		{
			List<UInt32> usedBraches = new List<uint>();
			return GetActionById(id, usedBraches);
		}
		public bool ContainsActionId(UInt32 actId)
		{
			List<UInt32> usedBranches = new List<uint>();
			return ContainsActionId(actId, usedBranches);
		}
		public void CollectionWaitingPoints()
		{
			if (this.StartingBranchId != this.BranchId)
			{
				throw new DesignerException("calling CollectionWaitingPoints from a non-starting point");
			}
			List<UInt32> usedBranches = new List<UInt32>();
			_waitingPointsThreads = new Dictionary<UInt32, List<UInt32>>();
			OnCollectionWaitingPoints(this, usedBranches);
		}
		public virtual void OnCollectionWaitingPoints(ActionBranch startBranch, List<UInt32> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (this.IsWaitingPoint)
			{
				MathNode.Trace("Waiting point: [{0},{1}] for thread [{2},{3}]", this.BranchId, this.Name, startBranch.BranchId, startBranch.Name);
				List<UInt32> list;
				if (!startBranch.WaitingPointsThreads.TryGetValue(this.BranchId, out list))
				{
					list = new List<UInt32>();
					startBranch.WaitingPointsThreads.Add(this.BranchId, list);
				}
				foreach (ActionBranch a in PreviousActions)
				{
					if (a.StartingBranchId != startBranch.BranchId)
					{
						if (!list.Contains(a.StartingBranchId))
						{
							ActionBranch a0 = _method.GetBranchById(a.StartingBranchId);
							if (a0 == null)
							{
								throw new DesignerException("ActionBranch not found for {0} as starting point for [{1},{2}]", a.StartingBranchId, a.BranchId, a.Name);
							}
							if (a0.MainBranchId == 0)
							{
								a0.MainBranchId = startBranch.BranchId;
								MathNode.Trace("sub-thread: [{0},{1}]", a0.BranchId, a0.Name);
								list.Add(a0.BranchId);
							}
						}
					}
				}
				MathNode.Trace("sub-thread count:{0}", list.Count);
			}
			//
			if (NextActions != null)
			{
				MathNode.Trace("[{0},{1}] Next actions:{2}", this.BranchId, this.Name, NextActions.Count);
				foreach (ActionBranch a in NextActions)
				{
					a.OnCollectionWaitingPoints(startBranch, usedBranches);
				}
			}
		}

		/// <summary>
		/// the branch calling this method must be the starting point
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <returns>true:all barnches of the main thread have method return or goto; false: at least one branch of the main thread does not have method return or goto</returns>
		public bool ExportThreadCode(ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (this.StartingBranchId != this.BranchId)
			{
				throw new DesignerException("calling ExportThreadCode from a non-starting point");
			}
			if (this.IsMergingPoint)
			{
				statements.Add(new CodeLabeledStatement(ActionBranch.IdToLabel(this.BranchId)));
				statements.Add(new CodeSnippetStatement(";"));
			}
			//launch dependent threads and wait for them to finish
			if (this.WaitingPointsThreads != null && this.WaitingPointsThreads.Count > 0)
			{
				//load waiting branches by ID's: 
				//load <ActionBranch, List<ActionBranch>> from <UInt32, List<UInt32>>
				Dictionary<ActionBranch, List<ActionBranch>> waitingPoints = new Dictionary<ActionBranch, List<ActionBranch>>();
				foreach (KeyValuePair<UInt32, List<UInt32>> kv in this.WaitingPointsThreads)
				{
					List<ActionBranch> list = new List<ActionBranch>();
					foreach (UInt32 id in kv.Value)
					{
						list.Add(_method.GetBranchById(id));
					}
					waitingPoints.Add(_method.GetBranchById(kv.Key), list);
				}
				MathNode.Trace("Wait points: {0}", waitingPoints.Count);
				foreach (KeyValuePair<ActionBranch, List<ActionBranch>> waitPoint in waitingPoints)
				{
					//Create merged threads for MergeActon[i](List<ActionBranch> _mergedThreads), 
					// i = 1,2,...(use process in Waiting for Merged Threads):
					ActionBranch mergeAction = waitPoint.Key;
					//1. Setup waiting handles
					/*	WaitHandle[] wh_<thread id>_<merge action id> = new WaitHandle[] 
					 *  {
					 *      new AutoResetEvent(false),
					 *      new AutoResetEvent(false), ..., (n-1 elements)
					 *  };
					 */
					//every merge action may have one or more merged threads
					List<ActionBranch> mergedThreads = waitPoint.Value;
					MathNode.Trace("Wait point {0}, Sub thread count {1}", waitPoint.Key.BranchId, mergedThreads.Count);
					string waitHandleArrayName = XmlSerialization.FormatString("wh_{0}_{1}", this.BranchKey, mergeAction.BranchKey);
					CodeExpression[] whInit = new CodeExpression[mergedThreads.Count];
					for (int k = 0; k < whInit.Length; k++)
					{
						whInit[k] = new CodeObjectCreateExpression(typeof(AutoResetEvent), new CodePrimitiveExpression(false));
					}
					CodeVariableDeclarationStatement wh = new CodeVariableDeclarationStatement(
						typeof(WaitHandle[]),
						waitHandleArrayName,
						new CodeArrayCreateExpression(typeof(WaitHandle), whInit));
					statements.Add(wh);
					//2. For each additional thread, create one thread function
					/*  void t_<main thread id>_<merge action id>_<merge thread id>(Object state)
					 *  {
					 *  	object[] args = (object[])state;
					 *  	
					 *      do actions for i-th thread:
					 *  	ActionGraph ag = new ActionGraph();
					 *      ag.CreateGraph(mi);
					 *  	ag.PrepareCodeCompile();
					 *      ag.ExportCode(mi);
					 *      
					 *      AutoResetEvent are = (AutoResetEvent)((object[])state)[0];
					 *      are.Set();
					 *  }
					 *  i = 0,1,...,n-1
					 */
					foreach (ActionBranch th in mergedThreads)
					{
						CodeMemberMethod mm0 = new CodeMemberMethod();
						mm0.Name = XmlSerialization.FormatString("t_{0}_{1}_{2}", this.BranchKey, mergeAction.BranchKey, th.BranchKey);
						if (_method.IsStatic)
						{
							mm0.Attributes = MemberAttributes.Static;
						}
						mm0.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "state"));
						mm0.ReturnType = new CodeTypeReference(typeof(void));
						compiler.TypeDeclaration.Members.Add(mm0);
						mm0.Statements.Add(new CodeVariableDeclarationStatement(typeof(object[]), MathNode.THREAD_ARGUMENT, new CodeCastExpression(typeof(object[]), new CodeVariableReferenceExpression("state"))));
						//
						//recover method parameters
						for (int i = 0, j = 1; i < _method.ParameterCount; i++, j++)
						{
							mm0.Statements.Add(new CodeVariableDeclarationStatement(_method.Parameters[i].ObjectType,
								_method.Parameters[i].Name,
								new CodeCastExpression(_method.Parameters[i].ObjectType,
								new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(MathNode.THREAD_ARGUMENT),
									new CodePrimitiveExpression(j)))));
						}
						//
						MathNode.Trace("Create merged thread method {0}", mm0.Name);
						MathNode.IndentIncrement();
						//generate code for mm0========================================================
						//
						//MethodType mi0 = method.CloneMethod();
						//mi0.MethodCode = mm0;
						//mi0.ThreadType = EnumThreadType.Merged;
						//set parameter references: parameters[i] => args[i+1]
						//if (_method.ParameterCount > 0)
						//{
						//    th.SetParameterReferences(method.Parameters);
						//}
						//th.ExportCode(mi0);
						//
						th.ExportThreadCode(compiler, mm0, mm0.Statements);
						//=============================================================================
						//signal thread ending
						mm0.Statements.Add(new CodeExpressionStatement(
							new CodeMethodInvokeExpression(
							new CodeCastExpression(typeof(AutoResetEvent),
							new CodeArrayIndexerExpression(
							new CodeCastExpression(typeof(object[]), new CodeVariableReferenceExpression("state")),
							new CodePrimitiveExpression(0))), "Set", new CodeExpression[] { })));
						MathNode.IndentDecrement();
					}
					//3. lauch all the merged threads
					/* Use 
					 *  ThreadPool.QueueUserWorkItem(
					 *     new WaitCallback(t_<main thread id>_<merge action id>_<merge thread id>), 
					 *       new object[] {wh_<main thread id>_<merge action id>[i], args of methods });
					 *  i = 0, 1, ..., n-1
					 *  to lauch all the additional threads.
					 */
					List<ParameterClass> ps = _method.Parameters;
					CodeExpression[] initializers = null;
					if (ps != null && ps.Count > 0)
					{
						initializers = new CodeExpression[ps.Count + 1];
						for (int i = 1; i < initializers.Length; i++)
						{
							initializers[i] = new CodeArgumentReferenceExpression(ps[i - 1].Name);
						}
					}
					else
					{
						initializers = new CodeExpression[1];
					}
					int K = 0;
					foreach (ActionBranch th in mergedThreads)
					{
						initializers[0] = new CodeArrayIndexerExpression(
							new CodeVariableReferenceExpression(waitHandleArrayName),
							new CodePrimitiveExpression(K));
						CodeExpressionStatement st = new CodeExpressionStatement(
						new CodeMethodInvokeExpression(
							new CodeTypeReferenceExpression(typeof(ThreadPool)), "QueueUserWorkItem",
								new CodeDelegateCreateExpression(
									new CodeTypeReference(typeof(WaitCallback)), new CodeThisReferenceExpression(),
										XmlSerialization.FormatString("t_{0}_{1}_{2}", this.BranchKey, mergeAction.BranchKey, th.BranchKey)),
										new CodeArrayCreateExpression(typeof(object), initializers)));
						statements.Add(st);
						K++;
					}
				}
			}
			//generate code
			bool bRet = ExportCode(null, null, compiler, method, statements);
			//finishAssemblyCompile(compiler, method, bRet);
			return bRet;
		}
		/// <summary>
		/// generate code for this branch
		/// </summary>
		/// <param name="previousAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <returns>true:all barnches have method return or goto; false: at least one branch does not have method return or goto</returns>
		public virtual bool ExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (compiler.Debug)
			{
				CodeExpression acter;
				if (this._method.IsStatic)
				{
					acter = new CodeTypeOfExpression(_method.Owner.ObjectType);
				}
				else
				{
					acter = new CodeThisReferenceExpression();
				}
				statements.Add(new CodeExpressionStatement(
							new CodeMethodInvokeExpression(
								new CodeVariableReferenceExpression(LimnorDebugger.Debugger), "BeforeExecuteAction",
								new CodePrimitiveExpression(_method.Owner.ObjectKey),
								new CodePrimitiveExpression(_method.WholeActionId),
								new CodePrimitiveExpression(this.BranchId),
								acter)
								)
							);
			}
			if (UseInput && InPortList != null && InPortList.Count > 1)
			{
				VPLUtil.ErrorLogger.LogError("An action cannot use action-input if more than one execution path leads to the action [Action ID:{0}, Action Name:{1}]. ", BranchId, this.Name);
			}
			if (this.IsCompiled)
			{
				CodeGotoStatement cgs = new CodeGotoStatement(IdToLabel(this.BranchId));
				statements.Add(cgs);
			}
			else
			{
				if (_isLoopStart)
				{
					//setup jump to label
					string jlb = IdToLabel(this.BranchId);
					CodeLabeledStatement cls = new CodeLabeledStatement(jlb);
					statements.Add(cls);
				}
				_allBranchesCompleted = OnExportCode(previousAction, nextAction, compiler, method, statements);
			}
			if (compiler.Debug)
			{
				CodeExpression acter;
				if (this._method.IsStatic)
				{
					acter = new CodeTypeOfExpression(_method.Owner.ObjectType);
				}
				else
				{
					acter = new CodeThisReferenceExpression();
				}
				statements.Add(new CodeExpressionStatement(
							new CodeMethodInvokeExpression(
								new CodeVariableReferenceExpression(LimnorDebugger.Debugger), "AfterExecuteAction",
								new CodePrimitiveExpression(_method.Owner.ObjectKey),
								new CodePrimitiveExpression(_method.WholeActionId),
								new CodePrimitiveExpression(this.BranchId),
								acter)
								)
							);
			}
			return _allBranchesCompleted;
		}
		public virtual bool ExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (UseInput && InPortList != null && InPortList.Count > 1)
			{
				VPLUtil.ErrorLogger.LogError("An action cannot use action-input if more than one execution path leads to the action [Action ID:{0}, Action Name:{1}]. ", BranchId, this.Name);
			}
			_allBranchesCompleted = OnExportJavaScriptCode(previousAction, nextAction, jsCode, methodCode, data);
			//
			return _allBranchesCompleted;
		}
		public virtual bool ExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (UseInput && InPortList != null && InPortList.Count > 1)
			{
				VPLUtil.ErrorLogger.LogError("An action cannot use action-input if more than one execution path leads to the action [Action ID:{0}, Action Name:{1}]. ", BranchId, this.Name);
			}
			_allBranchesCompleted = OnExportPhpScriptCode(previousAction, nextAction, jsCode, methodCode, data);
			//
			return _allBranchesCompleted;
		}
		public virtual void ExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			OnExportClientServerCode(previousAction, nextAction, compiler, method, statements, jsCode, methodCode, data);
		}
		public bool AllBranchesCompleted
		{
			get
			{
				return _allBranchesCompleted;
			}
		}
		public abstract bool AllBranchesEndWithMethodReturnStatement();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <returns>true:the branch ends with a method return statement or a goto; false: it does not end with a method return statement or goto</returns>
		public abstract bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements);
		public abstract bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);
		public abstract bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);
		public abstract void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);
		public abstract void Execute(List<ParameterClass> eventParameters);
		public abstract bool LoadToDesigner(List<UInt32> usedBranches, MethodDiagramViewer designer);
		public abstract void LoadActionData(ClassPointer pointer, List<UInt32> usedBranches);
		/// <summary>
		/// use linked out ports to find next actions and create an ActionString for each out port
		/// </summary>
		/// <param name="scope">to include actions in the collection the owners of the linked ports must be contained in this scope</param>
		/// <returns>usually a ActionString</returns>
		public abstract ActionBranch CollectActions(List<ActionViewer> scope, Dictionary<UInt32, ActionBranch> collected);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="branchId"></param>
		/// <returns></returns>
		public virtual bool ContainsAction(UInt32 branchId)//ActionBranch branch)
		{
			return (branchId == this.BranchId);
		}
		public virtual ActionBranch SearchBranchById(UInt32 branchId)
		{
			if (branchId == this.BranchId)
			{
				return this;
			}
			return null;
		}
		/// <summary>
		/// set the action content by matching the action id
		/// </summary>
		/// <param name="actions"></param>
		public abstract void SetActions(Dictionary<UInt32, IAction> actions, List<UInt32> usedBranches);
		/// <summary>
		/// set link IDs to link jump-to action branches
		/// </summary>
		public abstract void LinkJumpBranches();
		/// <summary>
		/// set the branch content to the jump-to branches by matching branch IDs
		/// it is called after serialization
		/// </summary>
		/// <param name="branches"></param>
		public abstract void LinkJumpedBranches(BranchList branches);
		/// <summary>
		/// when creating a new AB_Group, remove out of group action branches from AB_String jumped to
		/// </summary>
		/// <param name="branches"></param>
		public abstract void RemoveOutOfGroupBranches(BranchList branches);
		/// <summary>
		/// set NextAction and PreviousActions properties before compiling
		/// </summary>
		public abstract void LinkActions(BranchList branches);
		/// <summary>
		/// used in getting StartBranchId by a single thread branch list
		/// </summary>
		public virtual void SetBranchOwner()
		{
		}
		public virtual void ResetBeforeCompile()
		{
			PreviousActions = null;
			NextActions = null;
			_startingBranchId = 0;
			IsMainBranch = false;
			_isStartingPoint = false;
			IsCompiled = false;
			MainBranchId = 0;
			_isLoopStart = false;
		}
		public void OnBeforCompile()
		{
			//force calling findThread()
			if (StartingBranchId == this.BranchId)
			{
				_isStartingPoint = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mc"></param>
		/// <param name="statements"></param>
		private void addDefaultMethodReturn(CodeStatementCollection statements)
		{
			if (statements.Count > 0)
			{
				//it should rely on MethodSegment.Completed
				//this checking is redundunt, just an added precausion. 
				CodeMethodReturnStatement rs;
				rs = statements[statements.Count - 1] as CodeMethodReturnStatement;
				if (rs == null)
				{
					CodeGotoStatement gs = statements[statements.Count - 1] as CodeGotoStatement;
					if (gs == null)
					{
						rs = new CodeMethodReturnStatement();
						if (_method.ReturnValue != null && !_method.ReturnValue.IsVoid)
						{
							rs.Expression = ObjectCreationCodeGen.GetDefaultValueExpression(_method.ReturnValue.BaseClassType);
						}
						statements.Add(rs);
					}
				}
			}
		}
		private void finishAssemblyCompile(ILimnorCodeCompiler compiler, CodeMemberMethod method, bool bRet)
		{
			Dictionary<UInt32, MethodSegment> gotoBranches = CompilerUtil.GetGotoBranches(method);
			if (gotoBranches != null && gotoBranches.Count > 0)
			{
				if (!bRet)
				{
					//if the last method code is not a return then add a default method return
					//if at least one of the branches in the method (not including goBranches) is not ended (method return or go to)
					addDefaultMethodReturn(method.Statements);
				}
				foreach (KeyValuePair<UInt32, MethodSegment> kv in gotoBranches)
				{
					//if the last statement for this gotoBranch is not a method return then add a default one
					if (!kv.Value.Completed)
					{
						addDefaultMethodReturn(kv.Value.Statements);
					}
					//append the label to the end of method code
					method.Statements.Add(new CodeLabeledStatement(ActionBranch.IdToLabel(kv.Key)));
					if (kv.Value.Statements.Count == 0)
					{
						method.Statements.Add(new CodeSnippetStatement(";"));
					}
					//append the staments belonging to this gotoBranch to the end of the method
					for (int i = 0; i < kv.Value.Statements.Count; i++)
					{
						method.Statements.Add(kv.Value.Statements[i]);
					}
				}
			}
		}
		static private Dictionary<UInt32, ActionBranch> _checkedBranches = null;
		/// <summary>
		/// find the starting branch
		/// </summary>
		private void findThread()
		{
			if (PreviousActions == null || PreviousActions.Count <= 0)
			{
				///this is the first action
				_startingBranchId = this.BranchId;
				_checkedBranches = null;
			}
			else if (PreviousActions.Count == 1)
			{
				//it only has one previous action, use that action to relay to the first action
				bool bStart = false;
				if (_checkedBranches == null)
				{
					_checkedBranches = new Dictionary<uint, ActionBranch>();
					bStart = true;
				}
				if (_checkedBranches.ContainsKey(this.BranchId))
				{
					ActionBranch ab = null;
					//a loop is detected, use the top most action as the starting action.
					foreach (KeyValuePair<UInt32, ActionBranch> kv in _checkedBranches)
					{
						if (ab == null)
						{
							ab = kv.Value.TopAction();
						}
						else
						{
							ActionBranch a = kv.Value.TopAction();
							if (a.Location.Y < ab.Location.Y)
							{
								ab = a;
							}
						}
					}
					if (ab != null)
					{
						_startingBranchId = ab.BranchId;
						ab._isLoopStart = true;
					}
				}
				else
				{
					_checkedBranches.Add(this.BranchId, this);
					_startingBranchId = PreviousActions[0].StartingBranchId;
					if (bStart)
					{
						_checkedBranches = null;
					}
				}
			}
			else
			{
				_checkedBranches = null;
				//for more than one previous action, there is at least one main branch to be in the same thread of this action.
				//if there are previous actions not being main branch then this action is a waiting point. they are for other threads.
				//IsMainBranch is set by FindStartingBranchId
				foreach (ActionBranch a in PreviousActions)
				{
					if (a.IsMainBranch)
					{
						_startingBranchId = a.StartingBranchId;
						break;
					}
				}
				//IsMainBranch is not set for previous actions, call FindStartingBranchId to set it
				if (_startingBranchId == 0)
				{
					//find a branch which is not cycle back
					List<UInt32> branches = new List<UInt32>(); //for checking cyclic actions
					if (FindStartingBranchId(branches, ref _startingBranchId) != EnumBranchResult.Start)
					{
						throw new DesignerException("FindStartingBranchId failed at branch {0}", this.BranchId);
					}
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// find the starting branch when this branch has more than one previous action
		/// </summary>
		/// <param name="branchIdList">remember searched branches for checking cyclic actions</param>
		/// <param name="startingId">result</param>
		/// <returns></returns>
		public EnumBranchResult FindStartingBranchId(List<UInt32> branchIdList, ref UInt32 startingId)
		{
			//if actions form a cycle then there must have one action with _isStartingPoint being true
			if (_isStartingPoint)
			{
				startingId = this.BranchId;
				return EnumBranchResult.Start;
			}
			//a cycle is detected
			if (branchIdList.Contains(this.BranchId))
			{
				return EnumBranchResult.Loop;
			}
			//remember this action for checking cycle
			branchIdList.Add(this.BranchId);
			//if this is the first action then search stops
			if (PreviousActions == null || PreviousActions.Count <= 0)
			{
				startingId = this.BranchId;
				return EnumBranchResult.Start;
			}
			else if (PreviousActions.Count == 1)
			{
				//if previous action does not lead to a loop then we get the search result
				if (PreviousActions[0].FindStartingBranchId(branchIdList, ref startingId) == EnumBranchResult.Start)
				{
					return EnumBranchResult.Start;
				}
			}
			else
			{
				//among all previous actions, if there is a branch set as the main branch then use it to do the searching
				foreach (ActionBranch a in PreviousActions)
				{
					if (a.IsMainBranch)
					{
						if (a.FindStartingBranchId(branchIdList, ref startingId) == EnumBranchResult.Start)
						{
							return EnumBranchResult.Start;
						}
						break;
					}
				}
				//set IsMainBranch
				foreach (ActionBranch a in PreviousActions)
				{
					List<UInt32> l = new List<UInt32>();
					l.AddRange(branchIdList);
					if (a.FindStartingBranchId(l, ref startingId) == EnumBranchResult.Start)
					{
						a.IsMainBranch = true;
						return EnumBranchResult.Start;
					}
				}
			}
			//all branches are in loops
			//search for each branch to find one with manually set starting point
			foreach (ActionBranch a in PreviousActions)
			{
				List<UInt32> ids = new List<UInt32>();
				ids.Add(this.BranchId);
				startingId = a.GetManuallySetStartingId(ids);
				if (startingId != 0)
				{
					a.IsMainBranch = true;
					return EnumBranchResult.Start;
				}
			}
			//starting point is not set manually, randomly pick one
			PreviousActions[0].IsMainBranch = true;
			PreviousActions[0]._isStartingPoint = true;
			startingId = PreviousActions[0].BranchId;
			return EnumBranchResult.Start;
		}
		public UInt32 GetManuallySetStartingId(List<UInt32> ids)
		{
			if (ids.Contains(this.BranchId))
			{
				return 0;
			}
			ids.Add(this.BranchId);
			if (PreviousActions == null || PreviousActions.Count <= 0)
			{
				throw new DesignerException("calling GetManuallySetStartingId on a starting point");
			}
			foreach (ActionBranch a in PreviousActions)
			{
				if (a.IsEntryAction) //check _isStartingPoint 
				{
					return a.BranchId;
				}
				List<UInt32> l = new List<UInt32>();
				l.AddRange(ids);
				UInt32 id = a.GetManuallySetStartingId(l);
				if (id != 0)
				{
					return id;
				}
			}
			return 0;
		}
		public void SetPreviousAction(ActionBranch a)
		{
			if (PreviousActions == null)
			{
				PreviousActions = new List<ActionBranch>();
			}
			foreach (ActionBranch act in PreviousActions)
			{
				if (act.BranchId == a.BranchId)
				{
					return;
				}
			}
			PreviousActions.Add(a);
		}
		public void SetNextAction(ActionBranch a)
		{
			if (NextActions == null)
			{
				NextActions = new List<ActionBranch>();
			}
			foreach (ActionBranch act in NextActions)
			{
				if (act.BranchId == a.BranchId)
				{
					return;
				}
			}
			NextActions.Add(a);
		}
		/// <summary>
		/// if a previous action is removed then call this function to remove the link port IDs
		/// </summary>
		public virtual void ClearLinkPortIDs()
		{
			if (IsStartingPointByScope)
			{
				List<ActionPortIn> ports = this.InPortList;
				foreach (ActionPortIn p in ports)
				{
					p.LinkedPortID = 0;
				}
			}
		}
		/// <summary>
		/// when doing copy/paste, all port IDs must be re-created to avoid duplications
		/// </summary>
		public virtual void ResetLinkLineNodeIDs()
		{
			///? _id = Guid.NewGuid().GetHashCode();
			List<ActionPortIn> list = InPortList;
			if (list != null)
			{
				foreach (LinkLineNodeInPort p in list)
				{
					p.ResetActiveDrawingID();
				}
			}
			List<ActionPortOut> l2 = OutPortList;
			if (l2 != null)
			{
				foreach (LinkLineNodeOutPort p in l2)
				{
					p.ResetActiveDrawingID();
				}
			}
		}
		/// <summary>
		/// multiple in-ports link to multiple previous-actions.
		/// use this function to find the inport among them
		/// </summary>
		/// <param name="portId"></param>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		public ActionPortIn GetInPort(UInt32 portId, UInt32 instanceId)
		{
			List<ActionPortIn> list = InPortList;
			if (list != null)
			{
				foreach (ActionPortIn p in list)
				{
					if (p.PortID == portId && p.PortInstanceID == instanceId)
					{
						return p;
					}
				}
			}
			return null;
		}

		public PortCollection GetAllPorts()
		{
			PortCollection pc = new PortCollection();
			List<ActionPortIn> list = InPortList;
			if (list != null)
			{
				foreach (LinkLineNodeInPort p in list)
				{
					pc.Add(p);
				}
			}
			List<ActionPortOut> l2 = OutPortList;
			if (l2 != null)
			{
				foreach (LinkLineNodeOutPort p in l2)
				{
					pc.Add(p);
				}
			}
			return pc;
		}
		public virtual Bitmap CreateIcon(Graphics g)
		{
			return null;
		}
		public virtual void InitializePortPositions(Control owner)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		public abstract void AdjustInOutPorts();
		public virtual void InitializePorts(Control owner)
		{
			_outportList = new List<ActionPortOut>();
			if (OutportCount > 0)
			{
				double dx = ((double)owner.Width) / (double)(OutportCount + 1);
				for (int i = 0; i < OutportCount; i++)
				{
					ActionPortOut p = new ActionPortOut(this);
					p.Owner = owner;
					p.HideLabel();
					p.Position = (int)(dx + i * dx) - LinkLineNodePort.dotSize / 2;
					p.Left = owner.Left + p.Position;
					p.SaveLocation();
					p.CheckCreateNextNode();
					p.NextNode.Left = p.Left;
					_outportList.Add(p);
				}
			}
			_inportList = new List<ActionPortIn>();
			ActionPortIn ip = new ActionPortIn(this);
			ip.Owner = owner;
			ip.HideLabel();
			ip.Position = owner.Width / 2 - LinkLineNodePort.dotSize / 2;
			ip.Left = owner.Left + ip.Position;
			ip.SaveLocation();
			ip.CheckCreatePreviousNode();
			ip.PrevNode.Left = ip.Left;
			_inportList.Add(ip);
		}
		/// <summary>
		/// re-create all out-ports
		/// </summary>
		public void ResetOutport()
		{
			if (OutportCount > 0)
			{
				//int width = 100;
				Control owner = null;
				Point[] locations = new Point[OutportCount];
				int[] pos = new int[OutportCount];
				//create locations
				if (_outportList != null)
				{
					if (_outportList.Count > 0)
					{
						if (_outportList[0].Owner != null)
						{
							owner = _outportList[0].Owner;
						}
						for (int i = 0; i < OutportCount; i++)
						{
							if (i < _outportList.Count)
							{
								locations[i] = _outportList[i].Location;
								pos[i] = _outportList[i].Position;
							}
							else
							{
								locations[i] = new Point(30, 30);
								pos[i] = 15;
							}
						}
					}
				}
				_outportList = new List<ActionPortOut>();
				for (int i = 0; i < OutportCount; i++)
				{
					ActionPortOut p = new ActionPortOut(this);
					if (owner != null)
					{
						p.Owner = owner;
					}
					p.HideLabel();
					p.Position = pos[i];
					p.Location = locations[i];
					p.SaveLocation();
					p.CheckCreateNextNode();
					p.NextNode.Left = p.Left;
					_outportList.Add(p);
				}
			}
		}
		/// <summary>
		/// re-create all in-ports
		/// </summary>
		public void ResetInport()
		{
			Control owner = null;
			int pos = 0;
			Point loc = new Point(30, 30);
			if (_inportList != null && _inportList.Count > 0)
			{
				pos = _inportList[0].Position;
				loc = _inportList[0].Location;
				if (_inportList[0].Owner != null)
				{
					owner = _inportList[0].Owner;
				}
			}
			_inportList = new List<ActionPortIn>();
			ActionPortIn ip = new ActionPortIn(this);
			if (owner != null)
			{
				ip.Owner = owner;
			}
			ip.HideLabel();
			ip.Position = pos;
			ip.Location = loc;
			ip.SaveLocation();
			ip.CheckCreatePreviousNode();
			ip.PrevNode.Left = ip.Left;
			_inportList.Add(ip);
		}
		/// <summary>
		/// this function is used after visual editing. it should not be used for code compiling.
		/// if this action class does not branch to multiple braches then this function should return the next action
		/// linked to it. It is used to form a single ActionString.
		/// if this action class may branch to multiple braches, such as the ConditionBranch class, then it should return null
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		public virtual ActionBranch NextActionInScope(List<ActionViewer> scope)
		{
			List<ActionPortOut> ps = OutPortList;
			if (ps != null && ps.Count == 1)
			{
				if (ps[0].LinkedInPort != null)
				{
					ActionViewer av = ps[0].LinkedInPort.Owner as ActionViewer;
					if (av != null)
					{
						if (scope == null || scope.Contains(av))
						{
							return av.ActionObject;
						}
					}
				}
			}
			return null;
		}
		public virtual void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			if (m == null)
			{
				throw new DesignerException("Calling SetOwnerMethod with null MethodClass");
			}
			_method = m;
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);
			}
		}
		/// <summary>
		/// find out whther the thread represented by the id is linked to one waiting point 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual bool ContainsThread(UInt32 id)
		{
			if (id == this.StartingBranchId)
			{
				throw new DesignerException("Calling SingleActionBlock.ContainsThread on the same thread");
			}
			if (this.IsWaitingPoint)
			{
				foreach (ActionBranch a in PreviousActions)
				{
					if (a.StartingBranchId == id)
					{
						return true;
					}
				}
			}
			return false;
		}
		public virtual void OnBeforeLoadIntoDesigner(BranchList branches)
		{
		}
		public virtual void SetWithinLoop()
		{
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public abstract void SetInputName(string name, DataTypePointer type);
		/// <summary>
		/// called when it is linked to a previous action
		/// Condition branch uses it to create a "Input = {default value}" test
		/// </summary>
		public virtual void CreateDefaultInputUsage() { }
		/// <summary>
		/// find action values of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public abstract void FindItemByType<T>(List<T> results);
		/// <summary>
		/// find action method of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public abstract void FindMethodByType<T>(List<T> results);
		public abstract void SetIsMainThreadForSubBranches(List<UInt32> usedBranches);
		public abstract void CollectSourceValues(UInt32 taskid, List<UInt32> used, MethodClass mc);

		public abstract void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods);

		protected virtual void OnVerifyJump(BranchList branch) { }
		public virtual ActionPortIn GetActionInport(List<UInt32> used, UInt32 portId, UInt32 portInstanceId)
		{
			ActionPortIn ai = GetInPort(portId, portInstanceId);
			if (ai != null)
			{
				return ai;
			}
			return null;
		}
		public virtual void MakePortLinkForSingleThread(List<UInt32> used, BranchList branch)
		{
			if (_outportList != null && _outportList.Count > 0)
			{
				for (int k = 0; k < _outportList.Count; k++)
				{
					if (_outportList[k].LinkedInPort == null)
					{
						LinkLineNode end = _outportList[k].End;
						if (_outportList[k].LinkedPortID != 0 && _outportList[k].LinkedPortInstanceID != 0)
						{
							List<UInt32> used2 = new List<uint>();
							ActionPortIn pi = branch.GetActionInport(used2, _outportList[k].LinkedPortID, _outportList[k].LinkedPortInstanceID);
							if (pi == null)
							{
							}
							else
							{
								LinkLineNode start = pi.Start;
								end.SetNext(start);
								start.SetPrevious(end);
							}
						}
					}
				}
				OnVerifyJump(branch);
			}
		}
		#endregion
		#region Properties
		private IActionsHolder _actsHolder;
		/// <summary>
		/// try/catch/finally clause
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionsHolder ActionsHolder
		{
			get
			{
				return _actsHolder;
			}
			set
			{
				if (_actsHolder == null)
				{
					if (value != null)
					{
						_actsHolder = value;
					}
				}
			}
		}
		[Description("An invalid action should be removed")]
		public abstract bool IsValid { get; }

		/// <summary>
		/// if it is true then do not add method return at the end of compiling.
		/// the code logic changed, it is not used anymore
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public bool WithinLoop { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public string InputName { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public DataTypePointer InputType { get; set; }
		/// <summary>
		/// indicates whether the action branch uses action input
		/// </summary>
		public abstract bool UseInput { get; }
		/// <summary>
		/// indicates whether the action branch has action output
		/// </summary>
		[Browsable(false)]
		public abstract bool HasOutput { get; }
		/// <summary>
		/// the data type for the action output
		/// </summary>
		[Browsable(false)]
		public abstract DataTypePointer OutputType { get; }
		/// <summary>
		/// variable name for generating variable by output action and for referencing by input action
		/// </summary>
		[Browsable(false)]
		public virtual string OutputCodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "${0}ret{1}", MathNode.VariableNamePrefix, this.BranchId.ToString("x"));
				}
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}ret{1}", MathNode.VariableNamePrefix, this.BranchId.ToString("x"));
			}
		}
		[Browsable(false)]
		public abstract bool IsMethodReturn { get; }
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual bool IsReadOnly { get { return false; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsNotForDesigner { get; set; }

		[DefaultValue(false)]
		[Description("when several actions linked in a way of forming a loop, one action should be specified as the starting action. Use this property to indicate that this action is the starting action.")]
		public bool IsEntryAction
		{
			get
			{
				return _isStartingPoint;
			}
			set
			{
				_isStartingPoint = value;
			}
		}
		//===start of thread properties==============================
		/// <summary>
		/// when this instance is a starting point, this property collects all the waiting points and related threads
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public Dictionary<UInt32, List<UInt32>> WaitingPointsThreads
		{
			get
			{
				return _waitingPointsThreads;
			}
		}
		/// <summary>
		/// if this thread is a sub thread then this property indicates the action waiting for this thread to finish
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 WaitBranchId { get; set; }
		/// <summary>
		/// if this thread is a sub thread then this property indicates the main thread
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 MainBranchId { get; set; }
		/// <summary>
		/// indicates whether the code is compiled so that it will not be compiled twice
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsCompiled { get; set; }
		//===end of thread properties===============================
		/// <summary>
		/// it indicates the thread this branch belongs to
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 StartingBranchId
		{
			get
			{
				if (_startingBranchId == 0)
				{
					if (OwnerBranch != null)
					{
						_startingBranchId = OwnerBranch.StartingBranchId;
					}
					else
					{
						findThread();
					}
				}
				return _startingBranchId;
			}
		}
		/// <summary>
		/// if it is true then its next action belongs to the same thread of this action 
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsMainBranch { get; set; }
		[Description("When this action is the first action in a thread, this value indicates that whether this thread goes with the main execution thread of the running application.")]
		public virtual bool IsMainThread
		{
			get
			{
				return _isMainThread;
			}
			set
			{
				_isMainThread = value;
			}
		}
		/// <summary>
		/// when this action belongs to a single thread action list, this property is the ActionBranch owns the action list.
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public ActionBranch OwnerBranch { get; set; }

		private AB_Group _group;
		/// <summary>
		/// when this action belongs to a single thread action list, this property is the ActionBranch owns the action list.
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public AB_Group GroupBranch
		{
			get
			{
				if (_group == null)
				{
					if (OwnerBranch != null)
					{
						return OwnerBranch.GroupBranch;
					}
				}
				return _group;
			}
			set
			{
				_group = value;
			}
		}
		public UInt32 GroupBranchId
		{
			get
			{
				AB_Group g = GroupBranch;
				if (g == null)
				{
					return 0;
				}
				return g.BranchId;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public MethodClass Method
		{
			get
			{
				return _method;
			}
		}
		[Browsable(false)]
		public abstract Type ViewerType { get; }

		[DefaultValue(false)]
		[Browsable(false)]
		public bool BreakBeforeExecute { get; set; }

		[DefaultValue(false)]
		[Browsable(false)]
		public bool BreakAfterExecute { get; set; }

		[Browsable(false)]
		[ReadOnly(true)]
		public EnumActionBreakStatus AtBreak
		{
			get
			{
				return _breakStatus;
			}
			set
			{
				_breakStatus = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Bitmap BreakPointImage
		{
			get
			{
				switch (AtBreak)
				{
					case EnumActionBreakStatus.None:
						if (BreakBeforeExecute && BreakAfterExecute)
						{
							return Resources.ActionBreakStartEnd;
						}
						else if (BreakBeforeExecute)
						{
							return Resources.ActionBreakStart;
						}
						else if (BreakAfterExecute)
						{
							return Resources.ActionBreakEnd;
						}
						break;
					case EnumActionBreakStatus.Before:
						if (BreakBeforeExecute && BreakAfterExecute)
						{
							return Resources.ActionBreakStartEndPause1;
						}
						else
						{
							return Resources.ActionBreakStartPause;
						}
					case EnumActionBreakStatus.After:
						if (BreakBeforeExecute && BreakAfterExecute)
						{
							return Resources.ActionBreakStartEndPause2;
						}
						else
						{
							return Resources.ActionBreakEndPause;
						}
				}
				return null;
			}
		}
		public virtual uint OutportCount
		{
			get
			{
				return 1;
			}
		}
		public bool IsStartingPoint
		{
			get
			{
				if (LinkedInportCount == 0)
				{
					return true;
				}
				return _isStartingPoint;
			}
		}
		public bool IsEndingPoint
		{
			get
			{
				return (LinkedOutportCount == 0);
			}
		}
		/// <summary>
		/// a branching point uses a consitional to determine the next execution branch among more than one branches.
		/// SingleActionBlock and its derives are not branching point.
		/// ConditionBranch and SwitchBranch are branching point.
		/// </summary>
		public abstract bool IsBranchingPoint { get; }
		/// <summary>
		/// a merging point is linked by two or more previous actions.
		/// the linking is done through jumping
		/// </summary>
		public virtual bool IsMergingPoint
		{
			get
			{
				return (LinkedInportCount > 1);
			}
		}
		/// <summary>
		/// goto point is defined by that the action-branch has two or more previous actions within the same thread.
		/// a branch may ends only in one of the following two ways: 1). another goto point; 2) a method return.
		/// a branch may branching into two branches via AB_ConditionBranch
		/// </summary>
		public bool IsGotoPoint
		{
			get
			{
				if (PreviousActions == null || PreviousActions.Count < 2)
				{
					return false;
				}
				UInt32 id = this.StartingBranchId;
				int n = 0;
				foreach (ActionBranch a in PreviousActions)
				{
					if (a.StartingBranchId == id)
					{
						n++;
						if (n > 1)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		/// <summary>
		/// different threads leading to this action.
		/// </summary>
		public bool IsWaitingPoint
		{
			get
			{
				if (PreviousActions == null)
				{
					return false;
				}
				if (PreviousActions.Count < 2)
				{
					return false;
				}
				UInt32 id = 0;
				foreach (ActionBranch a in PreviousActions)
				{
					if (id == 0)
					{
						id = a.StartingBranchId;
					}
					else
					{
						if (id != a.StartingBranchId)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public virtual List<ActionPortOut> OutPortList
		{
			get
			{
				if (_outportList == null)
				{
					_outportList = new List<ActionPortOut>();
				}
				return _outportList;
			}
			set
			{
				_outportList = value;
			}
		}
		public virtual List<ActionPortIn> InPortList
		{
			get
			{
				if (_inportList == null)
				{
					_inportList = new List<ActionPortIn>();
				}
				return _inportList;
			}
			set
			{
				_inportList = value;
			}
		}
		public int InPortCount
		{
			get
			{
				return InPortList.Count;
			}
		}
		/// <summary>
		/// count the inports with LinkedPortID != 0
		/// </summary>
		public int LinkedInportCount
		{
			get
			{
				List<ActionPortIn> list = InPortList;
				if (list == null)
				{
					return 0;
				}
				int n = 0;
				foreach (ActionPortIn p in list)
				{
					if (p.LinkedPortID != 0)
					{
						n++;
					}
				}
				return n;
			}
		}
		/// <summary>
		/// count the out ports with LinkedPortID != 0
		/// </summary>
		public int LinkedOutportCount
		{
			get
			{
				List<ActionPortOut> l2 = OutPortList;
				if (l2 == null)
				{
					return 0;
				}
				int n = 0;
				foreach (ActionPortOut p in l2)
				{
					if (p.LinkedPortID != 0)
					{
						n++;
					}
				}
				return n;
			}
		}
		public UInt32 BranchId
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		public string BranchKey
		{
			get
			{
				return IdToKey(BranchId);
			}
		}
		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					_name = BaseActionName + "1";
				}
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}
		public Rectangle EditorBounds
		{
			get
			{
				return _rect;
			}
			set
			{
				_rect = value;
			}
		}
		public virtual string BaseActionName
		{
			get
			{
				return "Action";
			}
		}
		public virtual UInt32 FirstActionId
		{
			get
			{
				return BranchId;
			}
		}
		public Point Location { get; set; }
		public Size Size { get; set; }
		public Font TextFont
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}
		public Color TextColor
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsStartingPointByScope { get; set; }
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsEndingPointByScope { get; set; }
		[ReadOnly(true)]
		[Browsable(false)]
		public virtual bool IsNameReadOnly { get { return false; } }
		/// <summary>
		/// for compiling
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public List<ActionBranch> NextActions { get; set; }
		/// <summary>
		/// for compiling
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public List<ActionBranch> PreviousActions { get; set; }
		#endregion
		#region ICloneable Members

		public virtual object Clone()
		{
			List<UInt32> used = new List<uint>();
			ActionBranch obj = (ActionBranch)Activator.CreateInstance(this.GetType(), this.ActionsHolder);
			obj.SetOwnerMethod(used, _method);
			obj.BranchId = this.BranchId;
			obj.Name = _name;
			obj.Description = _desc;
			obj.EditorBounds = _rect;
			obj._isStartingPoint = _isStartingPoint;
			obj.IsNotForDesigner = IsNotForDesigner;
			obj.BreakBeforeExecute = BreakBeforeExecute;
			obj.BreakAfterExecute = BreakAfterExecute;
			if (_inportList != null)
			{
				List<ActionPortIn> ps = new List<ActionPortIn>();
				foreach (ActionPortIn ip in _inportList)
				{
					ip.ConstructorParameters = new object[] { obj };
					ActionPortIn p = (ActionPortIn)ip.Clone();
					ps.Add(p);
				}
				obj.InPortList = ps;
			}
			if (_outportList != null)
			{
				List<ActionPortOut> ps = new List<ActionPortOut>();
				foreach (ActionPortOut ip in _outportList)
				{
					ip.ConstructorParameters = new object[] { obj };
					ActionPortOut p = (ActionPortOut)ip.Clone();
					ps.Add(p);
				}
				obj.OutPortList = ps;
			}
			obj.Location = Location;
			obj.Size = Size;
			obj.TextColor = TextColor;
			if (TextFont != null)
			{
				obj.TextFont = (Font)TextFont.Clone();
			}
			obj.IsEndingPointByScope = IsEndingPointByScope;
			obj.IsStartingPointByScope = IsStartingPointByScope;
			return obj;
		}

		#endregion
		#region IPortOwner Members
		public UInt32 PortOwnerID
		{
			get
			{
				return this.BranchId;
			}
		}
		public bool IsDummyPort
		{
			get
			{
				return false;
			}
		}

		public void AddOutPort(LinkLineNodeOutPort port)
		{
			if (_outportList == null)
			{
				_outportList = new List<ActionPortOut>();
			}
			if (OutportCount > 0)
			{
				if (_outportList.Count < OutportCount)
				{
					port.SetPortOwner(this);
					_outportList.Add((ActionPortOut)port);
				}
			}
		}

		public void AddInPort(LinkLineNodeInPort port)
		{
			if (_inportList == null)
			{
				_inportList = new List<ActionPortIn>();
			}
			port.SetPortOwner(this);
			_inportList.Add((ActionPortIn)port);
		}
		public void RemovePort(LinkLineNodePort port)
		{
			if (_inportList != null)
			{
				foreach (ActionPortIn p in _inportList)
				{
					if (p == port)
					{
						_inportList.Remove(p);
						return;
					}
				}
			}
			if (_outportList != null)
			{
				foreach (ActionPortOut p in _outportList)
				{
					if (p == port)
					{
						_outportList.Remove(p);
						return;
					}
				}
			}
		}
		public abstract string TraceInfo { get; }

		#endregion
		#region IWithProject Members

		public LimnorProject Project
		{
			get
			{
				if (_method != null)
				{
					return _method.Project;
				}
				return null;
			}
		}

		#endregion
		#region IActionOwner Members

		public MethodClass OwnerMethod
		{
			get { return _method; }
		}

		#endregion
		#region IPropertiesWrapperOwner Members

		public abstract PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes);
		public abstract object GetPropertyOwner(int id, string propertyName);
		public virtual bool AsWrapper { get { return true; } }
		#endregion
		#region ISerializationProcessor Members

		public virtual void OnDeserialization(XmlNode objectNode)
		{
		}

		#endregion
		#region IActionContext Members
		[Browsable(false)]
		public uint ActionContextId
		{
			get { return BranchId; }
		}

		public object GetParameterType(uint id)
		{
			return null;
		}
		/// <summary>
		/// if this branch uses ParameterValue then use it to determine the type.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual object GetParameterType(string name)
		{
			return null;
		}
		[Browsable(false)]
		public object ProjectContext
		{
			get { return Project; }
		}
		[Browsable(false)]
		public object OwnerContext
		{
			get
			{
				return this.OwnerMethod;
			}
		}
		[Browsable(false)]
		public IMethod ExecutionMethod
		{
			get { return _method; }
		}
		[Browsable(false)]
		public void OnChangeWithinMethod(bool withinmethod)
		{

		}
		#endregion
		#region IOwnerProviderConstructorChild Members
		[Browsable(false)]
		public object GetChildConstructorOwner()
		{
			return _method;
		}

		#endregion
	}

}
