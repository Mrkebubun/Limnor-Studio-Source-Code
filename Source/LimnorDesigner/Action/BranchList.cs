/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using XmlSerializer;
using System.ComponentModel;
using VSPrj;
using System.CodeDom;
using MathExp;
using VPL;
using System.Collections.Specialized;
using System.Globalization;
using LimnorDesigner.Event;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// all actions in a method.
	/// the list contains ActionString and ActionCondition.
	/// single block actions should be included in ActionString.
	/// </summary>
	[SaveAsProperties(true)]
	public class BranchList : List<ActionBranch>, ICloneable, ISerializerProcessor, IWithProject
	{
		#region fields and constructors
		private MethodClass _method;
		private IActionsHolder _actsHolder;
		private Dictionary<UInt32, ActionBranch> _threads;
		private List<ActionBranch> _independentThreads;
		public BranchList(IActionsHolder scope)
		{
			_method = scope.OwnerMethod;
			_actsHolder = scope;
		}
		public BranchList(IActionsHolder scope, List<ActionBranch> actions)
		{
			_method = scope.OwnerMethod;
			_actsHolder = scope;
			if (actions.Count > 0)
			{
				_threads = new Dictionary<uint, ActionBranch>();
				_threads.Add(actions[0].BranchId, actions[0]);
				for (int i = 0; i < actions.Count; i++)
				{
					Add(actions[i]);
				}
			}
		}
		public BranchList(IActionListHolder act)
			: this(act.ActionsHolder)
		{
		}
		#endregion
		#region methods
		public int GetActionCount()
		{
			int n = 0;
			List<UInt32> usedBranches = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				n += ab.GetActionCount(usedBranches);
			}
			return n;
		}
		public void GetActionNames(StringCollection sc)
		{
			List<UInt32> usedBranches = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.GetActionNames(sc, usedBranches);
			}
		}
		public void EstablishObjectOwnership()
		{
			List<UInt32> used = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.EstablishObjectOwnership(_actsHolder, used);
			}
		}
		public override string ToString()
		{
			int n = GetActionCount();
			return string.Format(CultureInfo.InvariantCulture, "Actions:{0}", n);
		}
		public bool UseClientValues()
		{
			if (this.Count > 0)
			{
				List<UInt32> used = new List<uint>();
				foreach (ActionBranch a in this)
				{
					if (a.UseClientValues(used))
					{
						return true;
					}
				}
			}
			return false;
		}
		public bool UseServerValues()
		{
			if (this.Count > 0)
			{
				List<UInt32> used = new List<uint>();
				foreach (ActionBranch a in this)
				{
					if (a.UseServerValues(used))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void GetCustomMethods(List<MethodClass> list)
		{
			if (this.Count > 0)
			{
				List<UInt32> used = new List<uint>();
				foreach (ActionBranch a in this)
				{
					a.GetCustomMethods(used, list);
				}
			}
		}
		public LocalVariable GetLocalVariable(UInt32 id)
		{
			List<UInt32> usedBranches = new List<uint>();
			return GetLocalVariable(usedBranches, id);
		}
		public LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (this.Count > 0)
			{
				foreach (ActionBranch a in this)
				{
					LocalVariable v = a.GetLocalVariable(usedBranches, id);
					if (v != null)
					{
						return v;
					}
				}
			}
			return null;
		}
		public void GetActionsUseLocalVariable(UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (this.Count > 0)
			{
				List<UInt32> usedBranches = new List<uint>();
				foreach (ActionBranch a in this)
				{
					a.GetActionsUseLocalVariable(usedBranches, varId, actions);
				}
			}
		}
		public ActionPortIn GetActionInport(List<UInt32> used, UInt32 portId, UInt32 portInstanceId)
		{
			if (this.Count > 0)
			{
				foreach (ActionBranch a in this)
				{
					ActionPortIn ai = a.GetActionInport(used, portId, portInstanceId);
					if (ai != null)
					{
						return ai;
					}
				}
			}
			return null;
		}
		public void SetIsMainThreadForSubBranches(List<UInt32> usedBranches, bool isMainThread)
		{
			if (this.Count > 0)
			{
				foreach (ActionBranch a in this)
				{
					a.IsMainThread = isMainThread;
					a.SetIsMainThreadForSubBranches(usedBranches);
				}
			}
		}
		/// <summary>
		/// find action values of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public void FindItemByType<T>(List<T> results)
		{
			foreach (ActionBranch ab in this)
			{
				ab.FindItemByType<T>(results);
			}
		}
		/// <summary>
		/// find actions whose method owner is of a specified type
		/// </summary>
		/// <typeparam name="T">method owner type, i.e. EasyDataSet</typeparam>
		/// <param name="results">all actions thus found</param>
		public void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedMethods)
		{
			List<UInt32> usedBranches = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.FindActionsByOwnerType<T>(results, usedBranches, usedMethods);
			}
		}
		/// <summary>
		/// find action methods of the specific type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="results"></param>
		public void FindMethodByType<T>(List<T> results)
		{
			foreach (ActionBranch ab in this)
			{
				ab.FindMethodByType<T>(results);
			}
		}
		public void ReplaceAction(UInt32 oldId, IAction newAct)
		{
			List<UInt32> usedBraches = new List<UInt32>();
			foreach (ActionBranch ab in this)
			{
				ab.ReplaceAction(usedBraches, oldId, newAct);
			}
		}
		public bool ContainsActionId(UInt32 actId)
		{
			List<UInt32> usedBraches = new List<UInt32>();
			foreach (ActionBranch ab in this)
			{
				if (ab.ContainsActionId(actId, usedBraches))
				{
					return true;
				}
			}
			return false;
		}
		public List<UInt32> GetActionIDs()
		{
			List<UInt32> actionIDs = new List<UInt32>();
			List<UInt32> usedBraches = new List<UInt32>();
			foreach (ActionBranch ab in this)
			{
				ab.GetActionIDs(actionIDs, usedBraches);
			}
			return actionIDs;
		}
		public List<UInt32> GetLocalActionIDs(UInt32 subScopeId)
		{
			List<UInt32> lids = new List<UInt32>();
			List<IAction> acts = GetActions();
			foreach (IAction a in acts)
			{
				if (a == null)
				{
					throw new DesignerException("null action in BranchList. SetActions must be called before calling GetLocalActionIDs or GetActions");
				}
				if (!a.IsPublic)
				{
					if (a.SubScopeId == subScopeId)
					{
						lids.Add(a.ActionId);
					}
				}
			}
			return lids;
		}
		public void LoadActions(ClassPointer pointer)
		{
			List<UInt32> usedBraches = new List<UInt32>();
			foreach (ActionBranch ab in this)
			{
				ab.LoadActionData(pointer, usedBraches);
			}
		}
		public List<IAction> GetActions()
		{
			List<IAction> actions = new List<IAction>();
			List<UInt32> usedBraches = new List<UInt32>();
			foreach (ActionBranch ab in this)
			{
				ab.GetActions(actions, usedBraches);
			}
			return actions;
		}
		public IAction GetActionById(UInt32 id, List<UInt32> usedBraches)
		{
			foreach (ActionBranch ab in this)
			{
				IAction a = ab.GetActionById(id, usedBraches);
				if (a != null)
				{
					return a;
				}
			}
			return null;
		}
		public IAction GetActionById(UInt32 id)
		{
			List<UInt32> usedBraches = new List<UInt32>();
			return GetActionById(id, usedBraches);
		}
		public bool IsActionUsed(UInt32 actId)
		{
			List<UInt32> usedBraches = new List<UInt32>();
			return IsActionUsed(actId, usedBraches);
		}
		public bool IsActionUsed(UInt32 actId, List<UInt32> usedBraches)
		{
			foreach (ActionBranch ab in this)
			{
				if (ab.IsActionUsed(actId, usedBraches))
				{
					return true;
				}
			}
			return false;
		}
		public void InitializeBranches()
		{
			foreach (ActionBranch ab in this)
			{
				ab.InitializeBranches(this);
			}
		}
		public void OnExport()
		{
			List<UInt32> usedBranches = new List<uint>();
			List<ActionBranch> list = new List<ActionBranch>();
			foreach (ActionBranch ab in this)
			{
				List<ActionBranch> l = ab.OnExport(this, usedBranches);
				if (l != null && l.Count > 0)
				{
					list.AddRange(l);
				}
			}
			BranchList list2 = new BranchList(this._actsHolder);
			foreach (ActionBranch ab in list)
			{
				if (list2.GetBranchById(ab.BranchId) == null)
				{
					if (list2.GetBranchById(ab.FirstActionId) == null)
					{
						list2.Add(ab);
					}
					else
					{
						DesignUtil.WriteToOutputWindow("Duplicated first branch {0}", ab.FirstActionId);
					}
				}
				else
				{
					DesignUtil.WriteToOutputWindow("Duplicated branch {0}", ab.BranchId);
				}
			}
			foreach (ActionBranch ab in list2)
			{
				if (GetBranchById(ab.BranchId) == null)
				{
					this.Add(ab);
				}
			}
		}
		public ActionBranch GetBranchById(UInt32 id)
		{
			foreach (ActionBranch ab in this)
			{
				if (ab.BranchId == id)
				{
					return ab;
				}
			}
			return null;
		}
		public ActionBranch GetNextBranchById(UInt32 id)
		{
			int n = this.Count - 1;
			for (int i = 0; i < n; i++)
			{
				if (this[i].BranchId == id)
				{
					return this[i + 1];
				}
			}
			return null;
		}
		/// <summary>
		/// find the smallest group containing the branch
		/// </summary>
		/// <param name="id"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public ActionBranch GetBranchInGroup(UInt32 id, ref IActionGroup group)
		{
			foreach (ActionBranch ab in this)
			{
				ActionBranch a = ab.GetBranchInGroup(id, ref group);
				if (a != null)
				{
					return a;
				}
			}
			return null;
		}
		public void LinkJumpedBranches()
		{
			foreach (ActionBranch ab in this)
			{
				ab.LinkJumpedBranches(this);
			}
		}
		public void LinkJumpBranches()
		{
			foreach (ActionBranch ab in this)
			{
				ab.LinkJumpBranches();
			}
		}
		public void SetActions(Dictionary<UInt32, IAction> actions)
		{
			List<UInt32> usedBranches = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.SetActions(actions, usedBranches);
			}
		}
		public void CollectSourceValues(UInt32 taskid)
		{
			List<UInt32> used = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.CollectSourceValues(taskid, used, _method);
			}
		}
		public void ResetBeforeCompile()
		{
			foreach (ActionBranch ab in this)
			{
				ab.ResetBeforeCompile();
			}
			foreach (ActionBranch ab in this)
			{
				ab.LinkActions(this);
			}
			foreach (ActionBranch ab in this)
			{
				ab.OnBeforCompile();
			}
			foreach (ActionBranch ab in this)
			{
				ab.SetBranchOwner();
			}
		}
		/// <summary>
		/// parse independent branches
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		public List<ActionBranch> FindoutActionThreads(bool log)
		{
			ResetBeforeCompile();
			//
			//find out all threads
			List<ActionBranch> threads = new List<ActionBranch>();
			foreach (ActionBranch ab in this)
			{
				//calling StartingBranchId will establish the attributes
				if (ab.BranchId == ab.StartingBranchId)
				{
					threads.Add(ab);
				}
			}
			if (log)
			{
				MathNode.Trace("Thread count: {0}", threads.Count);
			}
			//find out independent threads
			List<ActionBranch> independentThreads = new List<ActionBranch>();
			foreach (ActionBranch ab in threads)
			{
				bool bContained = false;
				foreach (ActionBranch th in independentThreads)
				{
					if (th.ContainsThread(ab.BranchId))
					{
						bContained = true;
						break;
					}
				}
				if (!bContained)
				{
					independentThreads.Add(ab);
				}
			}
			if (log)
			{
				MathNode.Trace("Independent thread count: {0}", independentThreads.Count);
			}
			//collection threads at waiting points
			foreach (ActionBranch th in independentThreads)
			{
				th.CollectionWaitingPoints();
			}
			//validate waiting points
			foreach (ActionBranch ab in this)
			{
				if (ab.IsBranchingPoint)
				{
					bool bOK = true;
					List<ActionBranch> list = ab.NextActions;
					if (list != null)
					{
						List<UInt32> usedBranches = new List<uint>();
						int nCount = (int)(ab.OutportCount);
						if (nCount != list.Count)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].FindFirstWaitingPoint(usedBranches) != 0)
								{
									bOK = false;
									break;
								}
							}
						}
						else
						{
							UInt32 id = list[0].FindFirstWaitingPoint(usedBranches);
							for (int i = 1; i < list.Count; i++)
							{
								if (id != list[i].FindFirstWaitingPoint(usedBranches))
								{
									bOK = false;
									break;
								}
							}
						}
					}
					if (!bOK)
					{
						throw new DesignerException("Branching point [{0},{1}] branches to different threads", ab.BranchId, ab.Name);
					}
				}
			}
			IsMultiThreads = (independentThreads.Count > 1);
			_independentThreads = independentThreads;
			return independentThreads;
		}
		/// <summary>
		/// multi-thread support:
		/// 
		/// thread_{branch id}(object data)
		/// {
		///     object[] ps = (object[])data;
		///     type1 var1 = ps[0];
		///     type2 var2 = ps[1];
		///     ...
		///     {branch code}
		/// }
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <returns>true:all barnches of the main thread have method return or goto; false: at least one branch of the main thread does not have method return or goto</returns>
		public bool ExportCode(ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			bool bRet = false;
			MathNode.Trace("BranchList.ExportCode. Method {0}, action blocks {1}================", method.Name, this.Count);
			//create code threads
			List<ActionBranch> independentThreads;
			if (_independentThreads == null)
			{
				independentThreads = FindoutActionThreads(true);
			}
			else
			{
				independentThreads = _independentThreads;
			}
			IsMultiThreads = (independentThreads.Count > 1);
			//the case of Count == 0 (empty method) is handled by MethodClass.ExportCode
			if (independentThreads.Count > 0)
			{
				int k0 = 0;// main thread index
				this.ActionThreads.Clear();
				foreach (ActionBranch a in independentThreads)
				{
					_threads.Add(a.BranchId, a);
				}
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (independentThreads[k].IsMainThread)
					{
						k0 = k;
						break;
					}
				}
				if (k0 == 0)
				{
					independentThreads[0].IsMainThread = true;
				}
				this.MainThreadId = independentThreads[k0].BranchId;
				List<UInt32> usedBranches = new List<uint>();
				for (int k = 0; k < independentThreads.Count; k++)
				{
					independentThreads[k].IsMainThread = (k == k0);
					independentThreads[k].SetIsMainThreadForSubBranches(usedBranches);
				}
				//generate additional threads
				CodeExpression[] obs = null;
				CodeExpression[] ps = null;
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (k != k0)
					{
						ActionBranch ab = independentThreads[k];
						ab.IsMainThread = false;
						//method for this additional thread
						CodeMemberMethod mt = new CodeMemberMethod();
						mt.Name = "thread_" + ab.BranchId.ToString("x");
						compiler.TypeDeclaration.Members.Add(mt);
						mt.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "data"));
						//use an object[] to hold the method parameters which is passed to the thread method via the data parameter
						string pas = "ps_" + Guid.NewGuid().GetHashCode().ToString("x");
						mt.Statements.Add(new CodeVariableDeclarationStatement(typeof(object[]), pas,
							new CodeCastExpression(typeof(object[]), new CodeArgumentReferenceExpression("data"))));
						//create variables named after the method parameters and assign values to them
						//so that the actions within the method can use them as if they are method parameters.
						for (int i = 0; i < _method.ParameterCount; i++)
						{
							mt.Statements.Add(new CodeVariableDeclarationStatement(_method.Parameters[i].TypeString,
								_method.Parameters[i].Name,
								new CodeCastExpression(_method.Parameters[i].TypeString,
								new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(pas),
									new CodePrimitiveExpression(i)))));
						}
						//method contents: a single thread
						ab.ExportThreadCode(compiler, mt, mt.Statements);
						//
						if (compiler.Debug)
						{
							mt.Statements.Add(new CodeExpressionStatement(
								new CodeMethodInvokeExpression(
									new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), LimnorDebugger.Debugger), "ThreadFinished"
								//,new CodePrimitiveExpression(ab.BranchId)
										)
									)
								);
						}
						//code to launch this thread
						if (obs == null)
						{
							//create an object[] to hold all method parameters to be passed into the thread method
							obs = new CodeExpression[_method.ParameterCount];
							for (int i = 0; i < _method.ParameterCount; i++)
							{
								obs[i] = new CodeArgumentReferenceExpression(_method.Parameters[i].Name);
							}
						}
						//parameters for launch the thread
						ps = new CodeExpression[2];
						pas = "ps_" + Guid.NewGuid().GetHashCode().ToString("x");
						statements.Add(
							new CodeVariableDeclarationStatement(typeof(object[]), pas, new CodeArrayCreateExpression(typeof(object), obs)));
						//delegate to the thread method
						ps[0] = new CodeDelegateCreateExpression(new CodeTypeReference(typeof(System.Threading.WaitCallback)),
							new CodeThisReferenceExpression(), mt.Name);
						//parameter to the thread method
						ps[1] = new CodeVariableReferenceExpression(pas);
						//queue the thread
						statements.Add(new CodeMethodInvokeExpression(
							new CodeTypeReferenceExpression(typeof(System.Threading.ThreadPool)),
							"QueueUserWorkItem", ps));
					}
				}
				//main thread code
				bRet = independentThreads[k0].ExportThreadCode(compiler, method, statements);
				//
				if (_method == null)
				{
					MathNode.LogError("method is null");
				}
				else //
				{
					if (_method.ReturnValue != null)
					{
						if (!typeof(void).Equals(_method.ReturnValue.ObjectType))
						{
							//check whether all branches ends with a method return statement
							if (!independentThreads[k0].AllBranchesEndWithMethodReturnStatement())
							{
								CodeExpression pe;
								if (_method.ReturnValue.ObjectType.IsArray)
								{
									pe = new CodePrimitiveExpression(null);
								}
								else
								{
									pe = ObjectCreationCodeGen.ObjectCreationCode(VPLUtil.GetDefaultValue(_method.ReturnValue.ObjectType));
								}
								statements.Add(new CodeMethodReturnStatement(pe));
							}
						}
					}
				}
			}
			MathNode.Trace("End of BranchList.ExportCode. Method {0}================", method.Name);
			return bRet;
		}
		public bool ExportJavaScriptCode(StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			MathNode.Trace("BranchList.ExportJavaScriptCode. Method {0}, action blocks {1}================", _method.Name, this.Count);
			//create code threads
			List<ActionBranch> independentThreads;
			if (_independentThreads == null)
			{
				independentThreads = FindoutActionThreads(true);
			}
			else
			{
				independentThreads = _independentThreads;
			}
			IsMultiThreads = (independentThreads.Count > 1);
			//the case of Count == 0 (empty method) is handled by MethodClass.ExportCode
			if (independentThreads.Count > 0)
			{
				int k0 = 0;// main thread index
				this.ActionThreads.Clear();
				foreach (ActionBranch a in independentThreads)
				{
					_threads.Add(a.BranchId, a);
				}
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (independentThreads[k].IsMainThread)
					{
						k0 = k;
						break;
					}
				}
				if (k0 == 0)
				{
					independentThreads[0].IsMainThread = true;
				}
				this.MainThreadId = independentThreads[k0].BranchId;
				List<UInt32> usedBranches = new List<uint>();
				for (int k = 0; k < independentThreads.Count; k++)
				{
					independentThreads[k].IsMainThread = (k == k0);
					independentThreads[k].SetIsMainThreadForSubBranches(usedBranches);
				}
				//javascript does not support threading
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (k != k0)
					{
						ActionBranch ab = independentThreads[k];
						ab.IsMainThread = false;
						////method contents: a single thread
						ab.ExportJavaScriptCode(null, null, jsCode, methodCode, data);
					}
				}
				//main thread code
				bRet = independentThreads[k0].ExportJavaScriptCode(null, null, jsCode, methodCode, data);
				//
				if (_method == null)
				{
					MathNode.LogError("method is null");
				}
			}
			MathNode.Trace("End of BranchList.ExportJavaScriptCode. Method {0}================", _method.Name);
			return bRet;
		}
		public bool ExportPhpScriptCode(StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet = false;
			MathNode.Trace("BranchList.ExportPhpScriptCode. Method {0}, action blocks {1}================", _method.Name, this.Count);
			//create code threads
			List<ActionBranch> independentThreads;
			if (_independentThreads == null)
			{
				independentThreads = FindoutActionThreads(true);
			}
			else
			{
				independentThreads = _independentThreads;
			}
			IsMultiThreads = (independentThreads.Count > 1);
			//the case of Count == 0 (empty method) is handled by MethodClass.ExportCode
			if (independentThreads.Count > 0)
			{
				int k0 = 0;// main thread index
				this.ActionThreads.Clear();
				foreach (ActionBranch a in independentThreads)
				{
					_threads.Add(a.BranchId, a);
				}
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (independentThreads[k].IsMainThread)
					{
						k0 = k;
						break;
					}
				}
				if (k0 == 0)
				{
					independentThreads[0].IsMainThread = true;
				}
				this.MainThreadId = independentThreads[k0].BranchId;
				List<UInt32> usedBranches = new List<uint>();
				for (int k = 0; k < independentThreads.Count; k++)
				{
					independentThreads[k].IsMainThread = (k == k0);
					independentThreads[k].SetIsMainThreadForSubBranches(usedBranches);
				}
				//javascript does not support threading
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (k != k0)
					{
						ActionBranch ab = independentThreads[k];
						ab.IsMainThread = false;
						////method contents: a single thread
						ab.ExportPhpScriptCode(null, null, jsCode, methodCode, data);
					}
				}
				//main thread code
				bRet = independentThreads[k0].ExportPhpScriptCode(null, null, jsCode, methodCode, data);
				//
				if (_method == null)
				{
					MathNode.LogError("method is null");
				}
				else //
				{
					if (_method.ReturnValue != null)
					{
						if (!typeof(void).Equals(_method.ReturnValue.ObjectType))
						{
							//check whether all branches ends with a method return statement
							if (!independentThreads[k0].AllBranchesEndWithMethodReturnStatement())
							{
								methodCode.Add("return ");
								methodCode.Add(ValueTypeUtil.GetDefaultPhpScriptValueByType(_method.ReturnValue.ObjectType));
								methodCode.Add(";\r\n");
							}
						}
					}
				}
			}
			MathNode.Trace("End of BranchList.ExportPhpScriptCode. Method {0}================", _method.Name);
			return bRet;
		}
		public void ExportClientServerCode(ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			MathNode.Trace("BranchList.ExportJavaScriptCode. Method {0}, action blocks {1}================", _method.Name, this.Count);
			//create code threads
			List<ActionBranch> independentThreads;
			if (_independentThreads == null)
			{
				independentThreads = FindoutActionThreads(true);
			}
			else
			{
				independentThreads = _independentThreads;
			}
			IsMultiThreads = (independentThreads.Count > 1);
			//the case of Count == 0 (empty method) is handled by MethodClass.ExportCode
			if (independentThreads.Count > 0)
			{
				int k0 = 0;// main thread index
				this.ActionThreads.Clear();
				foreach (ActionBranch a in independentThreads)
				{
					_threads.Add(a.BranchId, a);
				}
				for (int k = 0; k < independentThreads.Count; k++)
				{
					if (independentThreads[k].IsMainThread)
					{
						k0 = k;
						break;
					}
				}
				if (k0 == 0)
				{
					independentThreads[0].IsMainThread = true;
				}
				this.MainThreadId = independentThreads[k0].BranchId;
				List<UInt32> usedBranches = new List<uint>();
				for (int k = 0; k < independentThreads.Count; k++)
				{
					independentThreads[k].IsMainThread = (k == k0);
					independentThreads[k].SetIsMainThreadForSubBranches(usedBranches);
				}
				//client/server does not support threading, process all threads one by one
				for (int k = 0; k < independentThreads.Count; k++)
				{
					independentThreads[k].ExportClientServerCode(null, null, compiler, method, statements, jsCode, methodCode, data);
				}
			}
		}
		public void Execute(List<ParameterClass> eventParameters)
		{
			bool first = true;
			foreach (ActionBranch ab in this)
			{
				if (ab.IsStartingPoint)
				{
					if (first)
					{
						first = false;
						ab.Execute(eventParameters);
					}
					else
					{
						System.Threading.ThreadPool.QueueUserWorkItem(ab.ExecuteInThread, eventParameters);
					}
				}
			}
		}
		/// <summary>
		/// search all level for the branch
		/// </summary>
		/// <param name="branchId"></param>
		/// <returns></returns>
		public bool ContainsAction(UInt32 branchId)
		{
			foreach (ActionBranch ab in this)
			{
				if (ab.ContainsAction(branchId))
				{
					return true;
				}
			}
			return false;
		}
		public ActionBranch SearchBranchById(UInt32 branchId)
		{
			foreach (ActionBranch ab in this)
			{
				ActionBranch a = ab.SearchBranchById(branchId);
				if (a != null)
				{
					return a;
				}
			}
			return null;
		}
		public ActionBranch GetJumpToActionBranch(UInt32 branchId)
		{
			foreach (ActionBranch a in this)
			{
				if (a.BranchId == branchId)
				{
					return a;
				}
				AB_ActionString abs = a as AB_ActionString;
				if (abs != null)
				{
					if (abs.ActionCount > 0)
					{
						if (abs.ActionList[0].BranchId == branchId)
						{
							return a;
						}
					}
				}
				else
				{
					IActionGroup ia = a as IActionGroup;
					if (ia != null)
					{
						if (ia.ActionList != null)
						{
							ActionBranch ab = ia.ActionList.GetJumpToActionBranch(branchId);
							if (ab != null)
							{
								return ab;
							}
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// search the top level for the branch
		/// </summary>
		/// <param name="branchId"></param>
		/// <returns></returns>
		public bool BranchInList(UInt32 branchId)
		{
			foreach (ActionBranch ab in this)
			{
				if (ab.BranchId == branchId)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// get/create a single thread group of actions containing the given branch id.
		/// it is used for loading debugger viewer
		/// </summary>
		/// <param name="branchId">the given branch id to be included in the result group</param>
		/// <returns>the action group containing the given branch id</returns>
		public IActionGroup GetThreadGroup(UInt32 branchId)
		{
			List<ActionBranch> independentThreads = FindoutActionThreads(false);
			foreach (ActionBranch a in independentThreads)
			{
				if (a.ContainsAction(branchId))
				{
					IActionGroup g = a as IActionGroup;
					if (g != null)
					{
						g = (IActionGroup)g.Clone();
						g.ResetGroupId(_method.MethodID);//all threads has the same group id. they can be distinguished at runtime by thread id
						return g;
					}
					AB_ActionString s = a as AB_ActionString;
					if (s != null)
					{
						IActionGroup sg = s.GetThreadGroup(branchId);
						sg.ResetGroupId(_method.MethodID);//all threads has the same group id. they can be distinguished at runtime by thread id
						return sg;
					}
					List<UInt32> used = new List<uint>();
					AB_ActionGroup ag = new AB_ActionGroup(_actsHolder);
					ag.SetOwnerMethod(used, _method);
					ag.BranchId = _method.MethodID; //all threads has the same group id. they can be distinguished at runtime by thread id
					ag.AppendAction(a);
					return ag;
				}
			}
			return null;
		}
		public void LoadToDesigner(MethodDiagramViewer designer)
		{
			foreach (ActionBranch ab in this)
			{
				ab.IsNotForDesigner = false;
			}
			foreach (ActionBranch ab in this)
			{
				ab.OnBeforeLoadIntoDesigner(this);
			}
			List<UInt32> used = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				if (!ab.IsNotForDesigner)
				{
					ab.LoadToDesigner(used, designer);
				}
			}
		}
		public void RemoveOutOfGroupBranches()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].RemoveOutOfGroupBranches(this);
			}
		}
		public void LoadToDesignerAsSingleThread(MethodDiagramViewer designer)
		{
			if (this.Count == 0)
			{
				return;
			}
			List<UInt32> used = new List<uint>();
			for (int i = 0; i < this.Count; i++)
			{
				this[i].MakePortLinkForSingleThread(used, this);
			}
			used.Clear();
			for (int i = 0; i < this.Count; i++)
			{
				this[i].LoadToDesigner(used, designer);
			}
		}
		public void ClearLinkPortIDs()
		{
			foreach (ActionBranch ab in this)
			{
				ab.ClearLinkPortIDs();
			}
		}
		public void SetOwnerMethod(MethodClass m)
		{
			_method = m;
			List<UInt32> used = new List<uint>();
			foreach (ActionBranch ab in this)
			{
				ab.SetOwnerMethod(used, _method);
			}
		}
		public void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			_method = m;
			foreach (ActionBranch ab in this)
			{
				ab.SetOwnerMethod(used, _method);
			}
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			BranchList obj = new BranchList(_actsHolder);
			foreach (ActionBranch ab in this)
			{
				obj.Add((ActionBranch)ab.Clone());
			}
			obj.SetOwnerMethod(_method);
			obj.LinkJumpedBranches();
			return obj;
		}

		#endregion
		#region Properties
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 MainThreadId { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsMultiThreads { get; set; }
		[Browsable(false)]
		public Dictionary<UInt32, ActionBranch> ActionThreads
		{
			get
			{
				if (_threads == null)
				{
					_threads = new Dictionary<UInt32, ActionBranch>();
				}
				return _threads;
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
		#endregion
		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				if (_method != null)
				{
					SetOwnerMethod(_method);
				}
				LinkJumpedBranches();
			}
		}

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
		public IActionsHolder ActionsHolder
		{
			get
			{
				return _actsHolder;
			}
		}
		#endregion
	}
}
