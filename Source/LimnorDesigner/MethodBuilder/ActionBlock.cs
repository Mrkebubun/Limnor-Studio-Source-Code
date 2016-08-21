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
using System.Collections.Specialized;
using TraceLog;

namespace LimnorDesigner.MethodBuilder
{
	abstract class ActionBlock
	{
		private ActionBranch _act;
		public ActionBlock(ActionBranch act)
		{
			_act = act;
		}

		public static ActionBlock CreateActionBlock(ActionBranch act)
		{
			ActionBlock ab;
			AB_ConditionBranch c = act as AB_ConditionBranch;
			if (c != null)
			{
				ab = new IfElseActionBlock(c);
			}
			else
			{
				AB_SingleActionBlock sa = act as AB_SingleActionBlock;
				if (sa != null)
				{
					ab = new StringActionBlock(sa);
				}
				else
				{
					throw new DesignerException("Unsupported action:{0}", act.GetType());
				}
			}
			ab.CreateBlocks();
			return ab;
		}
		public bool IsCompiled
		{
			get;
			set;
		}
		public UInt32 FirstActionBranchId
		{
			get
			{
				if (_act != null)
				{
					return _act.BranchId;
				}
				return 0;
			}
		}
		public ActionBranch FirstActionBranch
		{
			get
			{
				return _act;
			}
		}
		public abstract bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);
		public abstract void CreateBlocks();
		public abstract ActionBlock GetNextBlock();
	}
	class EmptyActionBlock : ActionBlock
	{
		private ActionBlock _nextAction;
		public EmptyActionBlock(ActionBranch nextAction)
			: base(nextAction)
		{
			_nextAction = ActionBlock.CreateActionBlock(nextAction);
		}
		public override void CreateBlocks()
		{

		}
		public override ActionBlock GetNextBlock()
		{
			return _nextAction;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_nextAction != null && _nextAction.FirstActionBranch != null && _nextAction.FirstActionBranch.InPortCount < 2)
			{
				return _nextAction.OnExportJavaScriptCode(previousAction, nextAction, jsCode, methodCode, data);
			}
			return false;
		}
	}
	class StringActionBlock : ActionBlock
	{
		private AB_SingleActionBlock _act;
		private ActionBlock _mergeBlock;
		public StringActionBlock(AB_SingleActionBlock act)
			: base(act)
		{
			_act = act;
		}
		public override void CreateBlocks()
		{
			_mergeBlock = null;
			if (_act != null)
			{
				List<ActionBranch> nexts = _act.NextActions;
				while (nexts != null && nexts.Count > 0)
				{
					if (nexts[0] != null)
					{
						if (nexts[0].InPortCount > 1 || nexts[0].OutportCount > 1)
						{
							_mergeBlock = ActionBlock.CreateActionBlock(nexts[0]);
							break;
						}
						else
						{
							nexts = nexts[0].NextActions;
						}
					}
					else
					{
						break;
					}
				}
			}
		}
		public override ActionBlock GetNextBlock()
		{
			if (_mergeBlock != null)
			{
				ActionBlock ab = _mergeBlock.GetNextBlock();
				if (ab != null)
				{
					return ab;
				}
				else
				{
					return _mergeBlock;
				}
			}
			return null;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_act != null)
			{
				bool bRet = _act.ExportJavaScriptCode(previousAction, nextAction, jsCode, methodCode, data);
				if (!bRet)
				{
					ActionBranch prev = _act;
					List<ActionBranch> nexts = _act.NextActions;
					while (nexts != null && nexts.Count > 0)
					{
						if (nexts[0] != null)
						{
							if (nexts[0].InPortCount > 1 || nexts[0].OutportCount > 1)
							{
								if (nexts[0].InPortCount < 2)
								{
									bRet = _mergeBlock.OnExportJavaScriptCode(_act, null, jsCode, methodCode, data);
								}
								break;
							}
							else
							{
								bRet = nexts[0].ExportJavaScriptCode(_act, null, jsCode, methodCode, data);
								if (bRet)
								{
									break;
								}
								else
								{
									prev = nexts[0];
									nexts = nexts[0].NextActions;
								}
							}
						}
						else
						{
							break;
						}
					}
				}
				return bRet;
			}
			return false;
		}
	}
	class IfElseActionBlock : ActionBlock
	{
		private AB_ConditionBranch _conditionAction;
		private ActionBlock _ifBlock;
		private ActionBlock _elseBlock;
		private ActionBlock _mergeBlock;
		public IfElseActionBlock(AB_ConditionBranch ca)
			: base(ca)
		{
			_conditionAction = ca;
		}
		public override ActionBlock GetNextBlock()
		{
			if (_mergeBlock != null)
			{
				return _mergeBlock.GetNextBlock();
			}
			return null;
		}
		public override void CreateBlocks()
		{
			_mergeBlock = null;
			_ifBlock = null;
			_elseBlock = null;
			if (_conditionAction != null)
			{
				if (_conditionAction.TrueActions != null)
				{
					if (_conditionAction.TrueActions.InPortCount > 1 || _conditionAction.TrueActions.OutportCount > 1)
					{
						_ifBlock = new EmptyActionBlock(_conditionAction.TrueActions);
					}
					else
					{
						_ifBlock = ActionBlock.CreateActionBlock(_conditionAction.TrueActions);
					}
				}
				if (_conditionAction.FalseActions != null)
				{
					if (_conditionAction.FalseActions.InPortCount > 1 || _conditionAction.FalseActions.OutportCount > 1)
					{
						_elseBlock = new EmptyActionBlock(_conditionAction.FalseActions);
					}
					else
					{
						_elseBlock = ActionBlock.CreateActionBlock(_conditionAction.FalseActions);
					}
				}
				if (_ifBlock != null && _elseBlock != null)
				{
					ActionBlock ifNext = _ifBlock.GetNextBlock();
					ActionBlock elseNext = _elseBlock.GetNextBlock();
					if (ifNext != null && elseNext != null)
					{
						if (ifNext.FirstActionBranchId == elseNext.FirstActionBranchId)
						{
							_mergeBlock = ifNext;
						}
					}
				}
			}
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			if (_conditionAction != null)
			{
				if (IsCompiled)
				{
					return false;
				}
				//preventing of compiling it twice
				IsCompiled = true;
				if (_conditionAction.TrueActions != null && _conditionAction.FalseActions != null && _conditionAction.TrueActions.BranchId == _conditionAction.FalseActions.BranchId)
				{
					throw new DesignerException("Condition action [{0}, {1}] goes to the same action", this.FirstActionBranchId, _conditionAction.Name);
				}
				try
				{
					StringCollection sts = methodCode;
					string c;
					if (_conditionAction.Condition == null)
					{
						c = "true";
					}
					else
					{
						_conditionAction.Condition.SetDataType(typeof(bool));
						c = _conditionAction.Condition.CreateJavaScript(sts);
					}
					bool b1 = false;
					bool b2 = false;
					string indent = Indentation.GetIndent();
					sts.Add(indent);
					sts.Add("if(");
					sts.Add(c);
					sts.Add(") {\r\n");
					if (_ifBlock != null)
					{
						Indentation.IndentIncrease();
						b1 = _ifBlock.OnExportJavaScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
					}
					sts.Add(indent);
					sts.Add("}\r\n");
					if (_elseBlock != null)
					{
						sts.Add(indent);
						sts.Add("else {\r\n");
						Indentation.IndentIncrease();
						b2 = _elseBlock.OnExportJavaScriptCode(previousAction, null, jsCode, sts, data);
						Indentation.IndentDecrease();
						sts.Add(indent);
						sts.Add("}\r\n");
					}
					if (_mergeBlock != null)
					{
						return _mergeBlock.OnExportJavaScriptCode(null, null, jsCode, sts, data);
					}
					else
					{
						return (b1 && b2);
					}
				}
				catch (Exception err)
				{
					throw new DesignerException(err, "Error compiling Condition action {0}. See inner exception for details", _conditionAction.TraceInfo);
				}
			}
			return false;
		}
	}
}
