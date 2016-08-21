/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using System.Xml;
using System.Globalization;
using XmlUtility;
using XmlSerializer;
using MathExp;
using System.Collections.Specialized;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using LimnorDatabase;
using LimnorDesigner.Event;

namespace LimnorDesigner.MethodBuilder
{
	public class AB_AssignActions : AB_SingleActionBlock
	{
		#region fields and constructors
		private EventAction _eventAction;
		public AB_AssignActions(IActionsHolder actsHolder)
			: base(actsHolder)
		{
		}
		public AB_AssignActions(IActionsHolder actsHolder, Point pos, Size size)
			: base(actsHolder, pos, size)
		{
		}
		public AB_AssignActions(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Methods
		public EventHandlerMethod GetHandlerMethod()
		{
			if (_eventAction != null)
			{
				HandlerMethodID hmid;
				EventHandlerMethod ehm = null;
				if (_eventAction.TaskIDList.Count > 0)
				{
					hmid = _eventAction.TaskIDList[0] as HandlerMethodID;
					if (hmid != null)
					{
						ehm = hmid.HandlerMethod;
					}
				}
				if (ehm == null)
				{
					hmid = new HandlerMethodID();
					ehm = new EventHandlerMethod(_eventAction.RootPointer);
					hmid.HandlerMethod = ehm;
					_eventAction.TaskIDList.Clear();
					_eventAction.TaskIDList.Add(hmid);
				}
				return ehm;
			}
			return null;
		}
		#endregion
		#region Properties
		public EventAction AssignedActions
		{
			get
			{
				if (_eventAction == null)
				{
					if (this.OwnerMethod != null)
					{
						ClassPointer root = this.OwnerMethod.RootPointer;
						if (root != null)
						{
							//it is public because event-handler does not belong to the scope method of this action branch.
							XmlNode node = root.XmlData.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
								"{0}/{1}[@{2}='{3}']",
								XmlTags.XML_HANDLERLISTS, XmlTags.XML_HANDLER, XmlTags.XMLATT_ActionID, this.BranchId));
							if (node != null)
							{
								Guid g = Guid.NewGuid();
								XmlObjectReader xr = root.ObjectList.Reader;
								xr.ResetErrors();
								xr.ClearDelayedInitializers(g);
								_eventAction = (EventAction)xr.ReadObject(node, null);
								xr.OnDelayedInitializeObjects(g);
								if (xr.Errors != null && xr.Errors.Count > 0)
								{
									MathNode.Log(xr.Errors);
								}
							}
						}
					}
				}
				return _eventAction;
			}
		}
		#endregion
		#region Overrides
		public override void FindActionsByOwnerType<T>(List<IActionMethodPointer> results, List<UInt32> usedBranches, List<UInt32> usedMethods)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.FindActionsByOwnerType<T>(results, usedMethods);
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							MethodClass.CollectActionsByOwnerType<T>(a, results, usedMethods);
						}
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
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.GetActionsUseLocalVariable(varId, actions);
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (!actions.ContainsKey(a.ActionId))
							{
								if (ClassPointer.IsRelatedAction(varId, a.CurrentXmlData))
								{
									actions.Add(a.ActionId, a);
								}
							}
						}
					}
				}
			}
		}
		protected override void OnGetActionNames(StringCollection sc, List<uint> usedBranches)
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.GetActionNames(sc);
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							string s = a.ActionName;
							if (!string.IsNullOrEmpty(s))
							{
								if (!sc.Contains(s))
								{
									sc.Add(s);
								}
							}
						}
					}
				}
			}
		}

		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<uint> usedBranches)
		{

		}

		public override bool IsActionUsed(uint actId, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return false;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							if (hmid.HandlerMethod.IsActionUsed(actId))
							{
								return true;
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (a.ActionId == actId)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public override IAction GetActionById(uint actId, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							IAction act = hmid.HandlerMethod.GetActionById(actId);
							if (act != null)
							{
								return act;
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (a.ActionId == actId)
							{
								return a;
							}
						}
					}
				}
			}
			return null;
		}

		public override void GetActionIDs(List<uint> actionIDs, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							List<UInt32> actIds = hmid.HandlerMethod.GetActionIDs();
							if (actIds != null && actIds.Count > 0)
							{
								for (int i = 0; i < actIds.Count; i++)
								{
									if (!actionIDs.Contains(actIds[i]))
									{
										actionIDs.Add(actIds[i]);
									}
								}
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (!actionIDs.Contains(a.ActionId))
							{
								actionIDs.Add(a.ActionId);
							}
						}
					}
				}
			}
		}
		private bool containsAction(List<IAction> actions, UInt32 actId)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i] != null && actions[i].ActionId == actId)
				{
					return true;
				}
			}
			return false;
		}
		public override void GetActions(List<IAction> actions, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							List<IAction> acts = hmid.HandlerMethod.GetActions();
							if (acts != null && acts.Count > 0)
							{
								for (int i = 0; i < acts.Count; i++)
								{
									if (!containsAction(actions, acts[i].ActionId))
									{
										actions.Add(acts[i]);
									}
								}
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (!containsAction(actions, a.ActionId))
							{
								actions.Add(a);
							}
						}
					}
				}
			}
		}

		public override void ReplaceAction(List<uint> usedBranches, uint oldId, IAction newAct)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.ReplaceAction(oldId, newAct);
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (oldId == a.ActionId)
							{
								tid.SetAction(newAct);
								tid.ActionId = newAct.ActionId;
							}
						}
					}
				}
			}
		}

		public override bool ContainsActionId(uint actId, List<uint> usedBranches)
		{
			return IsActionUsed(actId, usedBranches);
		}
		private bool containsCustMethod(List<MethodClass> list, UInt32 methodId)
		{
			foreach (MethodClass mc in list)
			{
				if (mc != null && mc.MethodID == methodId)
				{
					return true;
				}
			}
			return false;
		}
		public override void GetCustomMethods(List<uint> usedBranches, List<MethodClass> list)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							if (!containsCustMethod(list, hmid.HandlerMethod.MethodID))
							{
								list.Add(hmid.HandlerMethod);
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							MethodClass mc = a.ActionMethod.MethodPointed as MethodClass;
							if (mc != null)
							{
								if (!containsCustMethod(list, mc.MethodID))
								{
									list.Add(mc);
								}
							}
						}
					}
				}
			}
		}

		public override bool UseClientServerValues(List<uint> usedBranches, bool client)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return false;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							if (client)
							{
								if (hmid.HandlerMethod.UseClientValues())
								{
									return true;
								}
							}
							else
							{
								if (hmid.HandlerMethod.UseServerValues())
								{
									return true;
								}
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							IList<ISourceValuePointer> l;
							if (client)
							{
								l = a.GetClientProperties(0);
							}
							else
							{
								l = a.GetServerProperties(0);
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

		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod.IsMethodReturn)
						{
							return true;
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							if (a.IsMethodReturn)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			if (_eventAction != null)
			{
				_eventAction.AttachCodeDomAction(this.Method, statements, false);
			}
			return false;
		}

		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_eventAction != null)
			{
				_eventAction.AttachJavascriptAction(this.BranchId, methodCode, Indentation.GetIndent());
			}
			return false;
		}

		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			return false;
		}

		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{

		}

		public override void Execute(List<ParameterClass> eventParameters)
		{

		}

		public override bool LoadToDesigner(List<uint> usedBranches, MethodDiagramViewer designer)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return false;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
						}
					}
				}
			}
			return designer.LoadAction(this);
		}

		public override void LoadActionData(ClassPointer pointer, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.LoadActionInstances();
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
						}
					}
				}
			}
		}

		public override void SetActions(Dictionary<uint, IAction> actions, List<uint> usedBranches)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.LoadActionInstances();
						}
					}
					else
					{
						IAction a;
						if (actions.TryGetValue(tid.ActionId, out a))
						{
							if (a != null)
							{
								tid.SetAction(a);
							}
						}
					}
				}
			}
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

		public override void SetInputName(string name, DataTypePointer type)
		{
		}

		public override void FindItemByType<T>(List<T> results)
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							AB_SingleAction.ActionFindItemByType<T>(a, results);
						}
					}
				}
			}
		}

		public override void FindMethodByType<T>(List<T> results)
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							object v = hmid.HandlerMethod;
							if (v is T)
							{
								results.Add((T)v);
							}
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							object v = a.ActionMethod;
							if (v is T)
							{
								results.Add((T)v);
							}
						}
					}
				}
			}
		}

		public override void CollectSourceValues(uint taskid, List<uint> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				foreach (TaskID tid in ea.TaskIDList)
				{
					HandlerMethodID hmid = tid as HandlerMethodID;
					if (hmid != null)
					{
						if (hmid.HandlerMethod != null)
						{
							hmid.HandlerMethod.CollectSourceValues(taskid);
							mc.AddUploads(hmid.HandlerMethod.UploadProperties);
							mc.AddDownloads(hmid.HandlerMethod.DownloadProperties);
						}
					}
					else
					{
						IAction a = tid.GetPublicAction(this.OwnerMethod.RootPointer);
						if (a != null)
						{
							mc.AddUploads(a.GetClientProperties(taskid));
							mc.AddDownloads(a.GetServerProperties(taskid));
						}
					}
				}
			}
		}

		public override bool IsValid
		{
			get
			{
				EventAction ea = AssignedActions;
				if (ea != null)
				{
					if (ea.TaskIDList != null)
					{
						if (ea.TaskIDList.Count > 0)
						{
							return true;
						}
						else
						{
							MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Count of TaskIDList of AssignedActions is 0 for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
						}
					}
					else
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "TaskIDList of AssignedActions is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
					}
				}
				else
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "AssignedActions is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}

		public override bool UseInput
		{
			get { return false; }
		}

		public override bool HasOutput
		{
			get { return false; }
		}

		public override DataTypePointer OutputType
		{
			get { return new DataTypePointer(new TypePointer(typeof(void))); }
		}

		public override bool IsMethodReturn
		{
			get { return false; }
		}

		public override Type ViewerType
		{
			get { return typeof(ActionViewerAssignAction); }
		}

		public override string TraceInfo
		{
			get { return "Assign actions"; }
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
