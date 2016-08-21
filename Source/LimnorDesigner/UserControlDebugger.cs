/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Reflection;
using LimnorDesigner.Action;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner
{
	public partial class UserControlDebugger : UserControl
	{
		#region fields and constructors
		private LimnorDebugger _debugger;
		private UInt32 _classId;
		private ClassPointer _rootClassId;
		private Dictionary<string, MethodClass> _methods;
		private ITreeNodeBreakPoint _breakPointNode = null;
		private Dictionary<string, object> _componentList;
		private MethodDebugDesigner _designer;
		private int _currentThreadId;
		private Dictionary<int, ThreadDebug> _threadData;
		private bool _stopping;
		private MethodDesignerHolder _currentViewer;
		private int _runningEvents;
		//threading
		private fnOnObject miSetSelectedObject;
		private fnShowBreakPointInMethod miShowBreakPointInMethod;
		private fnShowBreakPointInMethod miClearBreakPointInMethod;
		private fnOnControl miShowActiveControl;
		public UserControlDebugger(LimnorDebugger debugger)
			: base()
		{
			_threadData = new Dictionary<int, ThreadDebug>();
			_debugger = debugger;
			//
			miSetSelectedObject = new fnOnObject(setSelectedObj);
			miShowBreakPointInMethod = new fnShowBreakPointInMethod(showBreakPointInMethod);
			miClearBreakPointInMethod = new fnShowBreakPointInMethod(clearBreakPointInMethod);
			miShowActiveControl = new fnOnControl(showActiveControl0);
			//
			InitializeComponent();
			//
			//
			_rootClassId = ClassPointer.CreateClassPointer(_debugger.Project, _debugger.RootXmlNode);
			_classId = _rootClassId.ClassId;
			_rootClassId.LoadActions(_rootClassId);
			_methods = _rootClassId.CustomMethods;
			_designer = new MethodDebugDesigner(_rootClassId, _debugger.Project);
			TreeNodeClassRoot r = treeView1.CreateClassRoot(true, _rootClassId, false);
			treeView1.Nodes.Add(r);
			treeView1.GotFocus += new EventHandler(treeView1_GotFocus);
			//
		}

		void treeView1_GotFocus(object sender, EventArgs e)
		{
			_currentViewer = null;
			FormDebugger f = this.FindForm() as FormDebugger;
			if (f != null)
			{
				_currentThreadId = f.MainThreadId;
				treeView1.BackColor = Color.LightYellow;
				showActiveControl(splitContainer1, splitContainer1.Panel1);
				f.setButtonImages();
			}
		}
		#endregion
		#region Properties
		public ClassPointer RootClass
		{
			get
			{
				return _rootClassId;
			}
		}
		public bool CurrentViewerFinished
		{
			get
			{
				if (_currentViewer != null)
				{
					return _currentViewer.ActionGroup.GroupFinished;
				}
				return (_runningEvents == 0);
			}
		}
		public bool IsWaitingAtBreakPoint
		{
			get
			{
				if (_threadData != null)
				{
					foreach (KeyValuePair<int, ThreadDebug> kv in _threadData)
					{
						if (kv.Value.WaitingAtBreakPoint)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public int CurrentThreadId
		{
			get
			{
				if (_currentThreadId == 0)
				{
					_currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
				}
				return _currentThreadId;
			}
		}
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
		}

		public EnumRunStatus GetRunStatus(int threadId)
		{
			return ThreadData(threadId).Status;
		}
		public void SetRunStatus(int threadId, EnumRunStatus status)
		{
			if (status == EnumRunStatus.Stop)
			{
				_stopping = true;
				foreach (KeyValuePair<int, ThreadDebug> kv in _threadData)
				{
					kv.Value.Status = EnumRunStatus.Stop;
				}
			}
			else
			{
				ThreadDebug t = ThreadData(threadId);
				t.Status = status;
				if (status == EnumRunStatus.StepOver || status == EnumRunStatus.StepOut)
				{
					t.StepOverLevel = t.StackLevel;
				}
				else if (status == EnumRunStatus.Finished)
				{
					//a none-main thread finishes
					setThreadGroupFinish(threadId);
					setViewerBackColor();
				}
			}
			FormDebugger f = this.FindForm() as FormDebugger;
			if (f != null)
			{
				f.setButtonImages();
			}
		}
		public bool ReachStepOver(int threadId)
		{
			ThreadDebug t = ThreadData(threadId);
			return (t.StackLevel <= t.StepOverLevel);
		}
		public bool ReachStepOut(int threadId)
		{
			ThreadDebug t = ThreadData(threadId);
			return (t.StackLevel < t.StepOverLevel);
		}
		public string ComponentFile
		{
			get
			{
				return _debugger.ComponentFile;
			}
		}
		public bool GetAtBreak(int threadId)
		{
			return ThreadData(threadId).AtBreak;
		}
		public void SetAtBreak(int threadId, bool value)
		{
			ThreadData(threadId).AtBreak = value;
		}
		public bool GetWaitingAtBreakPoint(int threadId)
		{
			return ThreadData(threadId).WaitingAtBreakPoint;
		}
		public void SetWaitingAtBreakPoint(int threadId, bool value)
		{
			ThreadData(threadId).WaitingAtBreakPoint = value;
		}

		public bool Stopping
		{
			get
			{
				return _stopping;
			}
		}
		public bool ShouldBreak(int threadId)
		{
			EnumRunStatus status = ThreadData(threadId).Status;
			if (status == EnumRunStatus.Pause || status == EnumRunStatus.StepInto)
			{
				return true;
			}
			return false;
		}
		public int GetCallStackLevel(int threadId)
		{
			ThreadDebug t = ThreadData(threadId);
			return t.StackLevel;
		}
		#endregion
		#region Public Methods
		public void EnterEvent()
		{
			_runningEvents++;
		}
		public void LeaveEvent()
		{
			_runningEvents--;
		}
		public void UpdateViewersBackColor()
		{
			setViewerBackColor();
		}
		public void IncrementStackLevel(int threadId)
		{
			ThreadDebug t = ThreadData(threadId);
			t.StackLevel = t.StackLevel + 1;
		}
		public void DecrementStackLevel(int threadId)
		{
			ThreadDebug t = ThreadData(threadId);
			t.StackLevel = t.StackLevel - 1;
		}
		public void Stop()
		{
			_stopping = true;
			stopViewers();
			if (_threadData != null)
			{
				foreach (KeyValuePair<int, ThreadDebug> kv in _threadData)
				{
					kv.Value.Status = EnumRunStatus.Stop;
					kv.Value.AtBreak = false;
				}
			}
		}
		public MethodClass GetMethod(UInt64 methodId)
		{
			foreach (MethodClass mc in _methods.Values)
			{
				if (mc.WholeActionId == methodId)
				{
					return mc;
				}
			}
			return null;
		}
		public MethodDesignerHolder GetViewer(int threadId, UInt32 groupId)
		{
			if (splitContainer1.Panel2.Controls.Count > 0)
			{
				for (int i = 0; i < splitContainer1.Panel2.Controls.Count; i++)
				{
					SplitContainer s = splitContainer1.Panel2.Controls[i] as SplitContainer;
					if (s != null)
					{
						MethodDesignerHolder h = GetViewer(threadId, groupId, s);
						if (h != null)
						{
							return h;
						}
					}
				}
			}
			return null;
		}
		public ActionBranch GetActionBranch(UInt64 methodId, UInt32 branchId)
		{
			MethodClass mc = GetMethod(methodId);
			if (mc != null)
			{
				return mc.GetBranchById(branchId);
			}
			return null;
		}
		public void AddComponent(string objectKey, object obj)
		{
			if (_componentList == null)
			{
				_componentList = new Dictionary<string, object>();
			}
			if (_componentList.ContainsKey(objectKey))
			{
				_componentList[objectKey] = obj;
			}
			else
			{
				_componentList.Add(objectKey, obj);
			}
		}
		public object GetComponentByKey(string objKey)
		{
			object v = null;
			if (_componentList != null)
			{
				if (!_componentList.TryGetValue(objKey, out v))
				{
					v = null;
				}
			}
			return v;
		}
		public void ClearBreakpointDisplay()
		{
			int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			if (!GetAtBreak(threadId) && _breakPointNode != null)
			{
				_breakPointNode.ShowBreakPoint(false);
				_breakPointNode = null;
			}
		}
		public void RefreshUI()
		{
			FormDebugger f = this.FindForm() as FormDebugger;
			f.RefreshUI();
		}
		public void SetSelectedObject(object v)
		{
			this.Invoke(miSetSelectedObject, v);
		}
		public void ShowEventBreakPointInTreeView(string objectKey, string eventName, object executer, bool forEntering)
		{
			TreeNodeClassComponent dc = treeView1.GetObjectNodeByKey(objectKey);
			if (dc != null)
			{
				TreeNodeEvent ev = dc.GetEventNode(eventName);
				if (ev != null)
				{
					if (_breakPointNode != null)
					{
						_breakPointNode.ShowBreakPoint(false);
					}
					_breakPointNode = ev.ShowEventBreakPoint(forEntering);
				}
				else
				{
					dc.TreeView.SelectedNode = dc;
				}
			}
		}
		public void ShowEventBreakPointInTreeView(string objectKey, string eventName, object executer, int actionIndex)
		{
			TreeNodeClassComponent dc = treeView1.GetObjectNodeByKey(objectKey);
			if (dc != null)
			{
				TreeNodeEvent ev = dc.GetEventNode(eventName);
				if (ev != null)
				{
					if (_breakPointNode != null)
					{
						_breakPointNode.ShowBreakPoint(false);
					}
					_breakPointNode = ev.ShowEventBreakPoint(actionIndex);
				}
				else
				{
					dc.TreeView.SelectedNode = dc;
				}
			}
		}
		public void ShowBreakPointInMethod(int threadId, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			this.Invoke(miShowBreakPointInMethod, threadId, method, group, branch);
		}
		public void ClearBreakPointInMethod(int threadId, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			this.Invoke(miClearBreakPointInMethod, threadId, method, group, branch);
		}

		#endregion
		#region Private Methods
		private MethodDesignerHolder GetViewer(int threadId, UInt32 groupId, SplitContainer s)
		{
			for (int i = 0; i < s.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = s.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					if (h.ThreadId == threadId && h.ActionGroup.GroupId == groupId)
					{
						return h;
					}
				}
			}
			for (int i = 0; i < s.Panel2.Controls.Count; i++)
			{
				SplitContainer s0 = s.Panel2.Controls[i] as SplitContainer;
				if (s0 != null)
				{
					MethodDesignerHolder h = GetViewer(threadId, groupId, s0);
					if (h != null)
					{
						return h;
					}
				}
			}
			return null;
		}
		private ThreadDebug ThreadData(int threadId)
		{
			ThreadDebug d;
			if (_threadData == null)
			{
				_threadData = new Dictionary<int, ThreadDebug>();
			}
			if (!_threadData.TryGetValue(threadId, out d))
			{
				d = new ThreadDebug();
				_threadData.Add(threadId, d);
			}
			return d;
		}
		private void stopViewers(SplitContainer c)
		{
			for (int i = 0; i < c.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = c.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					h.Stopping = true;
					break;
				}
			}
			for (int i = 0; i < c.Panel2.Controls.Count; i++)
			{
				SplitContainer s = c.Panel2.Controls[i] as SplitContainer;
				if (s != null)
				{
					stopViewers(s);
					break;
				}
			}
		}

		private void stopViewers()
		{
			stopViewers(splitContainer1);
		}
		private void panel2NotCollapse(SplitContainer c)
		{
			c.Panel2Collapsed = false;
		}
		private void panel2Collapse(SplitContainer c)
		{
			c.Panel2Collapsed = true;
		}
		private void addToPanel1(SplitContainer s, Control c)
		{
			c.Dock = DockStyle.Fill;
			s.Panel1.Controls.Add(c);
		}

		private void showBreakPointInMethod(int threadId, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			MethodDesignerHolder h = getViewer(threadId, method, group, branch);
			if (h == null)
			{
				IActionGroup g = (IActionGroup)(group.Clone());
				g = g.GetThreadGroup(branch.BranchId);
				//create a viewer
				SplitContainer c = getLastContainer(splitContainer1);
				SplitContainer newContainer = new SplitContainer();
				newContainer.Dock = DockStyle.Fill;
				c.Panel2.Controls.Add(newContainer);
				c.Panel2Collapsed = false;
				newContainer.Panel2Collapsed = true;
				h = Activator.CreateInstance(g.ViewerHolderType, this, _designer) as MethodDesignerHolder;
				h.ThreadId = threadId;
				h.SetBackgroundText(group.GroupName);
				h.Dock = DockStyle.Fill;
				newContainer.Panel1.Controls.Add(h);
				h.LoadActions(g);
				_currentThreadId = threadId;
				h.DesignerSelected += new EventHandler(h_DesignerSelected);
				newContainer.Panel1.GotFocus += new EventHandler(Panel1_GotFocus);
				newContainer.SplitterMoved += new SplitterEventHandler(newContainer_SplitterMoved);
				newContainer.Resize += new EventHandler(newContainer_Resize);
			}
			else
			{
				if (h.ActionGroup.GroupFinished)
				{
					h.ActionGroup.GroupFinished = false;
					ThreadDebug td = ThreadData(threadId);
					if (td.Status == EnumRunStatus.Finished)
					{
						FormDebugger f = this.FindForm() as FormDebugger;
						if (f != null)
						{
							td.Status = f.DebugCommandStatus;
						}
						else
						{
							td.Status = EnumRunStatus.Run;
						}
					}
					UpdateViewersBackColor();
				}
			}
			h.UpdateBreakpoint(branch);
		}
		private void adjustViewerSize(SplitContainer sp)
		{
			for (int i = 0; i < sp.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = sp.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					h.AdjustViewerSize();
				}
			}
		}
		void newContainer_Resize(object sender, EventArgs e)
		{
			SplitContainer sp = sender as SplitContainer;
			if (sp != null)
			{
				adjustViewerSize(sp);
			}
		}

		void newContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			SplitContainer sp = sender as SplitContainer;
			if (sp != null)
			{
				adjustViewerSize(sp);
			}
		}

		private void addToPanel2(SplitContainer s, Control c)
		{
			c.Dock = DockStyle.Fill;
			s.Panel2.Controls.Add(c);
		}

		private void clearBreakPointInMethod(int threadId, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			MethodDesignerHolder h = getViewer(threadId, method, group, branch);
			if (h != null)
			{
				h.ClearBreakpoint();
			}
		}
		private void h_DesignerSelected(object sender, EventArgs e)
		{
			MethodDesignerHolder h = sender as MethodDesignerHolder;
			if (h != null)
			{
				_currentViewer = h;
				_currentThreadId = h.ThreadId;
				if (treeView1.ThreadId == _currentThreadId)
				{
					treeView1.BackColor = Color.LightYellow;
				}
				else
				{
					treeView1.BackColor = Color.White;
				}
				h.SetViewerBackColor();
				showActiveControl(splitContainer1, h.Parent);
				FormDebugger f = this.FindForm() as FormDebugger;
				if (f != null)
				{
					f.setButtonImages();
				}
			}
		}
		private void setThreadGroupFinish(int threadId)
		{
			if (splitContainer1.Panel2.Controls.Count > 0)
			{
				for (int i = 0; i < splitContainer1.Panel2.Controls.Count; i++)
				{
					SplitContainer s = splitContainer1.Panel2.Controls[i] as SplitContainer;
					if (s != null)
					{
						setThreadGroupFinish(s, threadId);
					}
				}
			}
		}
		private void setThreadGroupFinish(SplitContainer s, int threadId)
		{
			for (int i = 0; i < s.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = s.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					if (h.ThreadId == threadId)
					{
						h.ActionGroup.GroupFinished = true;
					}
				}
			}
			for (int i = 0; i < s.Panel2.Controls.Count; i++)
			{
				SplitContainer s2 = s.Panel2.Controls[i] as SplitContainer;
				if (s2 != null)
				{
					setThreadGroupFinish(s2, threadId);
				}
			}
		}
		private void setViewerBackColor()
		{
			if (splitContainer1.Panel2.Controls.Count > 0)
			{
				for (int i = 0; i < splitContainer1.Panel2.Controls.Count; i++)
				{
					SplitContainer s = splitContainer1.Panel2.Controls[i] as SplitContainer;
					if (s != null)
					{
						setViewerBackColor(s);
					}
				}
			}
		}
		private void setViewerBackColor(SplitContainer s)
		{
			for (int i = 0; i < s.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = s.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					h.SetViewerBackColor();
				}
			}
			for (int i = 0; i < s.Panel2.Controls.Count; i++)
			{
				SplitContainer s2 = s.Panel2.Controls[i] as SplitContainer;
				if (s2 != null)
				{
					setViewerBackColor(s2);
				}
			}
		}
		private void setSelectedObj(object v)
		{
			propertyGrid1.SelectedObject = v;
		}
		private void Panel1_GotFocus(object sender, EventArgs e)
		{
			Control c = sender as Control;
			for (int i = 0; i < c.Controls.Count; i++)
			{
				MethodDesignerHolder h = c.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					_currentThreadId = h.ThreadId;
					h.SetViewerBackColor();//Color.LightYellow);
					showActiveControl(splitContainer1, c);
					break;
				}
			}
		}
		private void showActiveControl0(Control c)
		{
			showActiveControl(splitContainer1, c);
		}
		private void showActiveControl(Control c)
		{
			this.Invoke(miShowActiveControl, c);
		}
		private void showActiveControl(SplitContainer s, Control c)
		{
			if (s.Panel1 != c)
			{
				if (s.Panel1.Controls.Count > 0)
				{
					bool bFound = false;
					for (int i = 0; i < s.Panel1.Controls.Count; i++)
					{
						SplitContainer v = s.Panel1.Controls[i] as SplitContainer;
						if (v != null)
						{
							for (int k = 0; k < v.Panel1.Controls.Count; k++)
							{
								TreeView tv = v.Panel1.Controls[k] as TreeView;
								if (tv != null)
								{
									//tv.BackColor = Color.White;
									tv.Refresh();
									break;
								}
							}
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						for (int i = 0; i < s.Panel1.Controls.Count; i++)
						{
							MethodDesignerHolder h = s.Panel1.Controls[i] as MethodDesignerHolder;
							if (h != null)
							{
								h.SetViewerBackColor();//Color.White);
								break;
							}
						}
					}
				}
			}
			if (s.Panel2.Controls.Count > 0)
			{
				for (int i = 0; i < s.Panel2.Controls.Count; i++)
				{
					SplitContainer sp = s.Panel2.Controls[i] as SplitContainer;
					if (sp != null)
					{
						showActiveControl(sp, c);
						break;
					}
				}
			}
		}
		private SplitContainer getLastContainer(SplitContainer c)
		{
			if (c.Panel2.Controls.Count == 0)
			{
				return c;
			}
			for (int i = 0; i < c.Panel2.Controls.Count; i++)
			{
				SplitContainer h = c.Panel2.Controls[i] as SplitContainer;
				if (h != null)
				{
					return getLastContainer(h);
				}
			}
			throw new DesignerException("SplitContainer.Panel2 must contain another SplitContainer");
		}
		private MethodDesignerHolder getViewer(int threadId, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			return getViewer(threadId, splitContainer1, method, group, branch);
		}
		private MethodDesignerHolder getViewer(int threadId, SplitContainer c, MethodClass method, IActionGroup group, ActionBranch branch)
		{
			for (int i = 0; i < c.Panel1.Controls.Count; i++)
			{
				MethodDesignerHolder h = c.Panel1.Controls[i] as MethodDesignerHolder;
				if (h != null)
				{
					if (h.ThreadId == threadId)
					{
						if (h.Method.WholeActionId == method.WholeActionId)
						{
							if (group.GroupId == h.ActionGroup.GroupId)
							{
								if (h.ActionList.ContainsAction(branch.BranchId))
								{
									return h;
								}
							}
						}
					}
				}
			}
			for (int i = 0; i < c.Panel2.Controls.Count; i++)
			{
				SplitContainer s = c.Panel2.Controls[i] as SplitContainer;
				if (s != null)
				{
					MethodDesignerHolder h = getViewer(threadId, s, method, group, branch);
					if (h != null)
					{
						return h;
					}
				}
			}
			return null;
		}
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeClassComponent node = e.Node as TreeNodeClassComponent;
			if (node != null)
			{
				object v = GetComponentByKey(node.OwnerPointer.ObjectKey);
				node.OwnerPointer.ObjectDebug = v;
				propertyGrid1.SelectedObject = v;
			}
		}
		#endregion
	}
	public enum EnumRunStatus { Run, Pause, StepInto, StepOver, StepOut, Stop, Finished }
	class ThreadDebug
	{
		public ThreadDebug()
		{
		}
		public bool AtBreak { get; set; }
		public EnumRunStatus Status { get; set; }
		public bool WaitingAtBreakPoint { get; set; }
		public int StackLevel { get; set; }
		public int StepOverLevel { get; set; }
	}
}
