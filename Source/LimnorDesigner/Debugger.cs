/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VSPrj;
using System.Windows.Forms;
using LimnorDesigner.Action;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner
{
	public class LimnorDebugger
	{
		public const string Debugger = "_debugger";
		private static Dictionary<string, FormDebugger> _debuggerList = new Dictionary<string, FormDebugger>();
		private string _projectFile;
		private string _componentFile;

		private XmlDocument _doc;
		private FormDebugger _debugUI;
		private UserControlDebugger _ComponentDebugger;
		private string _key;
		public LimnorDebugger(string project, string component)
		{
			_componentFile = component;
			_projectFile = project;
			bool fileFound = System.IO.File.Exists(_componentFile);
			if (fileFound)
			{
				_doc = new XmlDocument();
				_doc.Load(_componentFile);
				_projectFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_componentFile),
					System.IO.Path.GetFileName(_projectFile));
				_projectFile = _projectFile.ToLowerInvariant();
				if (!_debuggerList.TryGetValue(_projectFile, out _debugUI))
				{
					_debugUI = null;
					System.Threading.Thread th = new System.Threading.Thread(showDebugWindow);
					th.SetApartmentState(System.Threading.ApartmentState.STA);
					th.Start();
					while (_debugUI == null)
					{
						Application.DoEvents();
					}
					_debugUI.MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
					_debuggerList.Add(_projectFile, _debugUI);
					while (!_debugUI.FormReady)
					{
						Application.DoEvents();
					}
				}

				_ComponentDebugger = _debugUI.AddTab(this);
			}
		}
		private void showDebugWindow(object v)
		{
			_debugUI = new FormDebugger(_projectFile);
			_debugUI.ShowDialog();
		}
		public bool AtBreak(int threadId)
		{
			return _ComponentDebugger.GetAtBreak(threadId);
		}
		public EnumRunStatus RunStatus(int threadId)
		{
			return _ComponentDebugger.GetRunStatus(threadId);
		}
		public void ExitDebug(object sender, FormClosingEventArgs e)
		{
			if (_debugUI != null)
			{
				if (!_debugUI.IsDisposed)
				{
					_debugUI.ExitDebug();
				}
			}
		}
		public string Key
		{
			get
			{
				if (string.IsNullOrEmpty(_key))
				{
					_key = "K" + Guid.NewGuid().ToString();
				}
				return _key;
			}
		}
		public LimnorProject Project
		{
			get
			{
				return _debugUI.Project;
			}
		}
		public XmlNode RootXmlNode
		{
			get
			{
				return _doc.DocumentElement;
			}
		}
		public string ComponentFile
		{
			get
			{
				return _componentFile;
			}
		}
		public string ComponentName
		{
			get
			{
				return System.IO.Path.GetFileNameWithoutExtension(_componentFile);
			}
		}
		private void waitForBreakPoint(int threadId)
		{
			_ComponentDebugger.SetWaitingAtBreakPoint(threadId, true);
			_ComponentDebugger.RefreshUI();
			while (_ComponentDebugger.GetAtBreak(threadId))
			{
				System.Threading.Thread.Sleep(0);
				Application.DoEvents();
			}
			_ComponentDebugger.RefreshUI();
			_ComponentDebugger.SetWaitingAtBreakPoint(threadId, false);
		}
		/// <summary>
		/// it should assign component to IObjectPointer.ObjectDebug
		/// </summary>
		/// <param name="objectKey"></param>
		/// <param name="component"></param>
		public void OnCreateComponent(string objectKey, object component)
		{
			_ComponentDebugger.AddComponent(objectKey, component);
		}
		/// <summary>
		/// before executing an action in a method.
		/// it can be called from different threads
		/// </summary>
		/// <param name="objectKey">ObjectKey of the owner of the method</param>
		/// <param name="methodWholeId">method id</param>
		/// <param name="branchId">action branch id of the action to be executed</param>
		/// <param name="executer">object instance executing the method</param>
		public void BeforeExecuteAction(string objectKey, UInt64 methodWholeId, UInt32 branchId, object executer)
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			_ComponentDebugger.IncrementStackLevel(threadId);
			//check if it is at a break point
			MethodClass mc = _ComponentDebugger.GetMethod(methodWholeId);
			if (mc != null)
			{
				//MethodClass mc = mc0.Clone() as MethodClass;
				IActionGroup g = mc;
				ActionBranch branch = mc.GetBranchByIdInGroup(branchId, ref g);
				if (branch != null)
				{
					if (_ComponentDebugger.ShouldBreak(threadId) || branch.BreakBeforeExecute)
					{
						_ComponentDebugger.SetSelectedObject(executer);
						_ComponentDebugger.SetAtBreak(threadId, true);
						_debugUI.ShowBreakPoint(_ComponentDebugger);
						branch.AtBreak = EnumActionBreakStatus.Before;
						_ComponentDebugger.ShowBreakPointInMethod(threadId, mc, g, branch);
						waitForBreakPoint(threadId);
					}
				}
			}
		}
		/// <summary>
		/// finish execute an action
		/// </summary>
		/// <param name="objectKey"></param>
		/// <param name="methodWholeId"></param>
		/// <param name="branchId"></param>
		/// <param name="executer"></param>
		public void AfterExecuteAction(string objectKey, UInt64 methodWholeId, UInt32 branchId, object executer)
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			_ComponentDebugger.DecrementStackLevel(threadId);
			//check if it is at a break point
			MethodClass mc = _ComponentDebugger.GetMethod(methodWholeId);
			if (mc != null)
			{
				IActionGroup g = mc;
				ActionBranch branch = mc.GetBranchByIdInGroup(branchId, ref g);
				if (branch != null)
				{
					IActionGroup g0 = branch as IActionGroup;
					if (g0 != null)
					{
						//finished calling a group of actions
						MethodDesignerHolder h = _ComponentDebugger.GetViewer(threadId, g0.GroupId);
						if (h != null)
						{
							h.ActionGroup.GroupFinished = true;
							_ComponentDebugger.UpdateViewersBackColor();
						}
					}
					else
					{
						ISingleAction sa = branch as ISingleAction;
						if (sa != null)
						{
							CustomMethodPointer ac = sa.ActionData.ActionMethod as CustomMethodPointer;
							if (ac != null)
							{
								//finished calling a custom action
								MethodDesignerHolder h = _ComponentDebugger.GetViewer(threadId, ac.MethodDef.MethodID);
								if (h != null)
								{
									h.ActionGroup.GroupFinished = true;
									_ComponentDebugger.UpdateViewersBackColor();
								}
							}
						}
					}
					EnumRunStatus status = _ComponentDebugger.GetRunStatus(threadId);
					bool b = (status == EnumRunStatus.Pause || status == EnumRunStatus.StepInto);
					if (!b)
					{
						if (status == EnumRunStatus.StepOver && _ComponentDebugger.ReachStepOver(threadId))
						{
							b = true;
						}
						if (!b)
						{
							if (status == EnumRunStatus.StepOut && _ComponentDebugger.ReachStepOut(threadId))
							{
								b = true;
							}
						}
					}

					if (b || branch.BreakAfterExecute)
					{
						_ComponentDebugger.SetSelectedObject(executer);
						_ComponentDebugger.SetAtBreak(threadId, true);
						_debugUI.ShowBreakPoint(_ComponentDebugger);
						branch.AtBreak = EnumActionBreakStatus.After;
						_ComponentDebugger.ShowBreakPointInMethod(threadId, mc, g, branch);
						waitForBreakPoint(threadId);
						branch.AtBreak = EnumActionBreakStatus.None;
						_ComponentDebugger.ShowBreakPointInMethod(threadId, mc, g, branch);
					}
					else
					{
						branch.AtBreak = EnumActionBreakStatus.None;
						_ComponentDebugger.ClearBreakPointInMethod(threadId, mc, g, branch);
					}
				}
			}
		}
		/// <summary>
		/// called when a none-main thread finishes
		/// </summary>
		public void ThreadFinished()
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			_ComponentDebugger.SetRunStatus(threadId, EnumRunStatus.Finished);
		}
		/// <summary>
		/// called from event action list, not from a custom method.
		/// called only between event actions
		/// </summary>
		/// <param name="objectKey"></param>
		/// <param name="eventName"></param>
		/// <param name="taskId"></param>
		/// <param name="executer"></param>
		public void BeforeExecuteEventAction(string objectKey, string eventName, int actionIndex, object executer)
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			_ComponentDebugger.ClearBreakpointDisplay();
			EventAction ea = _ComponentDebugger.RootClass.GetEventHandler(eventName, objectKey);
			if (ea != null)
			{
				int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
				if (actionIndex > 0)
				{
					//check the action just finished is an custom method or not
					if (!ea.TaskIDList[actionIndex - 1].IsEmbedded)
					{
						IAction a = _ComponentDebugger.RootClass.GetActionInstance(ea.TaskIDList[actionIndex - 1].ActionId);
						if (a != null)
						{
							CustomMethodPointer cmp = a.ActionMethod as CustomMethodPointer;
							if (cmp != null)
							{
								MethodDesignerHolder h = _ComponentDebugger.GetViewer(threadId, cmp.MethodDef.MethodID);
								if (h != null)
								{
									h.ActionGroup.GroupFinished = true;
									_ComponentDebugger.UpdateViewersBackColor();
								}
							}
						}
					}
				}
				EnumRunStatus status = _ComponentDebugger.GetRunStatus(threadId);
				if ((status != EnumRunStatus.Stop && status != EnumRunStatus.Run) || ea.TaskIDList[actionIndex].BreakAsEventAction)
				{
					_ComponentDebugger.SetSelectedObject(executer);
					_ComponentDebugger.SetAtBreak(threadId, true);
					_debugUI.ShowBreakPoint(_ComponentDebugger);
					_ComponentDebugger.ShowEventBreakPointInTreeView(objectKey, eventName, executer, actionIndex);
					waitForBreakPoint(threadId);
				}
			}
		}
		public void EnterEvent(string objectKey, string eventName, object executer)
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			_ComponentDebugger.EnterEvent();
			_ComponentDebugger.AddComponent(objectKey, executer);
			EventAction ea = _ComponentDebugger.RootClass.GetEventHandler(eventName, objectKey);
			if (ea != null)
			{
				int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
				EnumRunStatus status = _ComponentDebugger.GetRunStatus(threadId);
				if ((status != EnumRunStatus.Stop && status != EnumRunStatus.Run) || ea.BreakBeforeExecute)
				{
					_ComponentDebugger.SetSelectedObject(executer);
					_ComponentDebugger.SetAtBreak(threadId, true);
					_debugUI.ShowBreakPoint(_ComponentDebugger);
					_ComponentDebugger.ShowEventBreakPointInTreeView(objectKey, eventName, executer, true);
					waitForBreakPoint(threadId);
				}
			}
		}
		public void LeaveEvent(string objectKey, string eventName, object executer)
		{
			if (_ComponentDebugger.Stopping)
			{
				return;
			}
			EventAction ea = _ComponentDebugger.RootClass.GetEventHandler(eventName, objectKey);
			if (ea != null)
			{
				int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
				EnumRunStatus status = _ComponentDebugger.GetRunStatus(threadId);
				if ((status != EnumRunStatus.Stop && status != EnumRunStatus.Run) || ea.BreakAfterExecute)
				{
					_ComponentDebugger.SetSelectedObject(executer);
					_ComponentDebugger.SetAtBreak(threadId, true);
					_debugUI.ShowBreakPoint(_ComponentDebugger);
					_ComponentDebugger.ShowEventBreakPointInTreeView(objectKey, eventName, executer, false);
					waitForBreakPoint(threadId);
				}
			}
			_ComponentDebugger.ClearBreakpointDisplay();
			_ComponentDebugger.LeaveEvent();
			FormDebugger f = _ComponentDebugger.FindForm() as FormDebugger;
			if (f != null)
			{
				f.setButtonImages();
			}
		}
	}
}
