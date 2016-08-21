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
using ProgElements;
using VSPrj;
using System.Windows.Forms;
using MathExp;
using System.CodeDom;
using XmlSerializer;
using System.Xml;
using System.ComponentModel;
using XmlUtility;
using System.Reflection;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using LimnorDesigner.Property;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// for action created for SubMethodInfo
	/// </summary>
	public class AB_SubMethodAction : AB_Squential, ISingleAction, ISubMethod, IActionGroup, IPostRootDeserialize
	{
		#region fields and constructors
		private TaskID _actId; //serialized
		private ActionSubMethod _actionData;//not serialized with this object
		private EnumIconDrawType _imgLayout = EnumIconDrawType.Left;
		private List<ComponentIcon> _iconList;
		private List<IParameter> _parameters;
		private bool _showText = true;
		//
		public AB_SubMethodAction(ActionSubMethodGlobal act)
			: base(act.ActionHolder)
		{
			_actionData = act;
			_actId = new TaskID(act.ActionId, act.ClassId);
		}
		public AB_SubMethodAction(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_SubMethodAction(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_SubMethodAction(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion

		#region Properties
		public override bool IsValid
		{
			get
			{
				if (this.ActionData == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "ActionData is null for [{0}] of [{1}]", this.ToString(), this.GetType().Name);
				}
				return (this.ActionData != null);
			}
		}
		[Browsable(false)]
		public override Type ViewerType
		{
			get { return typeof(ActionViewerSubMethod); }
		}
		/// <summary>
		/// indicates whether the action branch has action output.
		/// a sub-method is for array/list loop, it cannot have return value
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
		[PropertyReadOrder(100)]
		[Browsable(false)]
		public List<ComponentIcon> ComponentIconList
		{
			get
			{
				if (_iconList == null)
				{
					_iconList = new List<ComponentIcon>();
				}
				return _iconList;
			}
			set
			{
				_iconList = value;
			}
		}
		#endregion

		#region Methods
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			base.OnGetActionNames(sc, usedBranches);
			if (_actId != null)
			{
				HandlerMethodID hid = _actId as HandlerMethodID;
				if (hid != null)
				{
					if (hid.HandlerMethod != null)
					{
						hid.HandlerMethod.GetActionNames(sc);
					}
				}
				else
				{
					if (_actId.Action != null)
					{
						if (!string.IsNullOrEmpty(_actId.Action.ActionName))
						{
							if (!sc.Contains(_actId.Action.ActionName))
							{
								sc.Add(_actId.Action.ActionName);
							}
						}
					}
				}
			}
		}
		public override bool IsActionUsed(UInt32 actId, List<UInt32> usedBranches)
		{
			if (_actId != null)
			{
				if (_actId.ActionId == actId)
				{
					return true;
				}
			}
			if (_actionData != null && _actionData.ActionId == actId)
			{
				return true;
			}
			return base.IsActionUsed(actId, usedBranches);
		}
		public override IAction GetActionById(UInt32 id, List<UInt32> usedBranches)
		{
			if (_actId != null)
			{
				if (_actId.ActionId == id)
				{
					if (_actId.Action != null)
					{
						return _actId.Action;
					}
					return _actionData;
				}
			}
			if (_actionData != null && _actionData.ActionId == id)
			{
				return _actionData;
			}
			return base.GetActionById(id, usedBranches);
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			base.OnEstablishObjectOwnership(scope, usedBranches);
			if (_actId != null)
			{
				_actId.EstablishObjectOwnership(scope);
			}
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ci.EstablishObjectOwnership(scope.OwnerMethod);
				}
			}

		}
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			base.CollectSourceValues(taskId, usedBranches, mc);
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				return base.UseClientServerValues(usedBranches, client);
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
			base.GetCustomMethods(usedBranches, list);
		}
		public ParameterClassSubMethod GetParameterById(UInt32 id)
		{
			IList<IParameter> ps = MethodParameterTypes;
			foreach (IParameter p in ps)
			{
				if (p.ParameterID == id)
				{
					return (ParameterClassSubMethod)p;
				}
			}
			return null;
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			if (_actionData != null && string.Compare(_actionData.MethodName, SubMethodInfo.ExecuteForEachItem, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Resources._av_forEach.ToBitmap();
			}
			return Resources._method.ToBitmap();
			//return null;
		}
		public override LocalVariable GetLocalVariable(List<UInt32> usedBranches, UInt32 id)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return null;
			}
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconLocal cil = ci as ComponentIconLocal;
					if (cil != null && cil.LocalPointer != null)
					{
						if (cil.LocalPointer.MemberId == id)
						{
							return cil.LocalPointer;
						}
					}
				}
			}
			return base.GetLocalVariable(usedBranches, id);
		}
		public override ActionBranchParameter GetActionBranchParameterByName(string parameterName)
		{
			if (_actionData != null)
			{
				SubMethodInfoPointer smi = _actionData.ActionMethod as SubMethodInfoPointer;
				List<ParameterClassSubMethod> ps = smi.GetParameters(this);
				if (ps != null && ps.Count > 0)
				{
					foreach (ParameterClassSubMethod p in ps)
					{
						if (string.CompareOrdinal(p.Name, parameterName) == 0)
						{
							return p;
						}
					}
				}
			}
			return null;
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			bool bRet;
			if (_actionData == null)
			{
				_actionData = (ActionSubMethod)this.Method.GetActionInstance(_actId.ActionId);// (ActionSubMethod)compiler.ActionEventList.GetAction(_actId);
			}
			SubMethodInfoPointer smi = _actionData.ActionMethod as SubMethodInfoPointer;
			SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
			if (mi.IsForeach)
			{
				ParameterClassSubMethod p = mi.GetParameterType(0, smi, this);
				StringBuilder sb = new StringBuilder();
				string s1 = smi.Owner.CodeName;
				CustomMethodParameterPointer cmpp = smi.Owner.Owner as CustomMethodParameterPointer;
				if (cmpp == null)
				{
					IObjectPointer op = smi.Owner;
					sb.Append(s1);
					while (!(op is ILocalvariable) && op.Owner != null && !(op.Owner is MethodClass) && op.Owner.Owner != null)
					{
						if (string.CompareOrdinal(s1, op.Owner.CodeName) != 0)
						{
							s1 = op.Owner.CodeName;
							sb.Insert(0, ".");
							sb.Insert(0, s1);
						}
						op = op.Owner;
					}
					s1 = sb.ToString();
				}
				string itemTypeString = null;
				if (mi.ItemType.IsGenericParameter)
				{
					CollectionPointer cp = smi.Owner as CollectionPointer;
					if (cp != null)
					{
						CustomPropertyPointer cpp = cp.Owner as CustomPropertyPointer;
						if (cpp != null)
						{
							DataTypePointer dtp = cpp.GetConcreteType(mi.ItemType);
							if (dtp != null)
							{
								itemTypeString = dtp.TypeString;
							}
						}
					}
				}
				if (string.IsNullOrEmpty(itemTypeString))
				{
					itemTypeString = VPLUtil.GetTypeCSharpName(mi.ItemType);
				}
				string s0 = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}foreach({1} {2} in {3}) {{", Indentation.GetIndent(), itemTypeString, p.CodeName, s1);
				CodeStatement cis0 = new CodeSnippetStatement(s0);
				statements.Add(cis0);
				//
				if (_iconList != null)
				{
					foreach (ComponentIcon ci in _iconList)
					{
						ComponentIconLocal cil = ci as ComponentIconLocal;
						if (cil != null && cil.ScopeGroupId == this.BranchId)
						{
							cil.LocalPointer.AddVariableDeclaration(statements);
						}
					}
				}
				//
				CodeStatementCollection sc = new CodeStatementCollection();
				Method.SubMethod.Push(this);//smi);
				CompilerUtil.AddSubMethod(method, this);
				bRet = base.OnExportCode(previousAction, nextAction, compiler, method, sc);
				Method.SubMethod.Pop();

				bRet = CompilerUtil.FinishSubMethod(method, this, sc, bRet);
				statements.AddRange(sc);
				//
				statements.Add(new CodeSnippetStatement(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}}}", Indentation.GetIndent())));
			}
			else
			{
				CodeIterationStatement cis = new CodeIterationStatement();
				cis.TestExpression = mi.GetTestExpression(smi, compiler, this.Method, method, statements, this.BranchId);
				cis.InitStatement = mi.GetInitStatement(smi, compiler, this.Method, method, statements, this.BranchId);
				cis.IncrementStatement = mi.GetIncrementalStatement(smi, compiler, this.Method, method, statements, this.BranchId);
				//
				Method.SubMethod.Push(this);//smi);
				CompilerUtil.AddSubMethod(method, this);
				if (_iconList != null)
				{
					foreach (ComponentIcon ci in _iconList)
					{
						ComponentIconLocal cil = ci as ComponentIconLocal;
						if (cil != null && cil.ScopeGroupId == this.BranchId)
						{
							cil.LocalPointer.AddVariableDeclaration(cis.Statements);
						}
					}
				}
				bRet = base.OnExportCode(previousAction, nextAction, compiler, method, cis.Statements);
				Method.SubMethod.Pop();

				bRet = CompilerUtil.FinishSubMethod(method, this, cis.Statements, bRet);
				//
				//
				statements.Add(cis);
			}

			return bRet;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet;
			if (_actionData == null)
			{
				_actionData = (ActionSubMethod)this.Method.GetActionInstance(_actId.ActionId);// (ActionSubMethod)compiler.ActionEventList.GetAction(_actId);
			}
			if (_actionData == null)
			{
				return false;
			}
			SubMethodInfoPointer smi = _actionData.ActionMethod as SubMethodInfoPointer;
			SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
			if (mi == null)
			{
				return false;
			}
			if (mi.IsForeach)
			{
				ParameterClassSubMethod p = mi.GetParameterType(0, smi, this);
				StringBuilder sb = new StringBuilder();
				string s1 = smi.Owner.CodeName;
				IObjectPointer op = smi.Owner;
				sb.Append(s1);
				while (!(op is ILocalvariable) && op.Owner != null && op.Owner.Owner != null)
				{
					if (!s1.StartsWith(op.Owner.CodeName, StringComparison.Ordinal))
					{
						s1 = op.Owner.CodeName;
						sb.Insert(0, ".");
						sb.Insert(0, s1);
					}
					op = op.Owner;
				}
				s1 = sb.ToString();
				string indents = Indentation.GetIndent();
				string a = string.Format(CultureInfo.InvariantCulture,
									"a{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				string idx = string.Format(CultureInfo.InvariantCulture,
					"i{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				string s0 = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{4}var {3} = {1};\r\n{4}if({3}) for(var {0}=0;{0}<{3}.length;{0}++) {{\r\n{4}var {2}={3}[{0}]; \r\n", idx, s1, p.CodeName, a, indents);
				methodCode.Add(s0);
				Method.SubMethod.Push(this);
				data.AddSubMethod(this);
				Indentation.IndentIncrease();
				bRet = base.OnExportJavaScriptCode(previousAction, nextAction, jsCode, methodCode, data);
				Indentation.IndentDecrease();
				Method.SubMethod.Pop();
				//
				methodCode.Add(indents);
				methodCode.Add("}\r\n");
			}
			else
			{
				string indents = Indentation.GetIndent();
				methodCode.Add(indents);
				methodCode.Add("for(var ");
				methodCode.Add(mi.GetInitStatementJS(smi, jsCode, methodCode, data, this.BranchId));
				methodCode.Add(mi.GetTestExpressionJS(smi, jsCode, methodCode, data, this.BranchId));
				methodCode.Add(mi.GetIncrementalStatementJS(smi, jsCode, methodCode, data, this.BranchId));
				methodCode.Add(") {\r\n");
				Method.SubMethod.Push(this);
				data.AddSubMethod(this);
				Indentation.IndentIncrease();
				if (_iconList != null)
				{
					foreach (ComponentIcon ci in _iconList)
					{
						ComponentIconLocal cil = ci as ComponentIconLocal;
						if (cil != null && cil.ScopeGroupId == this.BranchId)
						{
							methodCode.Add(Indentation.GetIndent());
							methodCode.Add("var ");
							methodCode.Add(cil.LocalPointer.CodeName);
							methodCode.Add("=");
							methodCode.Add(ValueTypeUtil.GetDefaultJavaScriptValueByType(cil.LocalPointer.BaseClassType));
							methodCode.Add(";\r\n");
						}
					}
				}
				bRet = base.OnExportJavaScriptCode(previousAction, nextAction, jsCode, methodCode, data);
				Method.SubMethod.Pop();
				//
				Indentation.IndentDecrease();
				methodCode.Add(indents);
				methodCode.Add("}\r\n");
			}

			return bRet;
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			bool bRet;
			if (_actionData == null)
			{
				_actionData = (ActionSubMethod)this.Method.GetActionInstance(_actId.ActionId);
			}
			SubMethodInfoPointer smi = _actionData.ActionMethod as SubMethodInfoPointer;
			SubMethodInfo mi = smi.MethodInformation as SubMethodInfo;
			if (mi == null)
			{
				return false;
			}
			if (mi.IsForeach)
			{
				ParameterClassSubMethod p = mi.GetParameterType(0, smi, this);
				StringBuilder sb = new StringBuilder();
				string s1 = smi.Owner.CodeName;
				IObjectPointer op = smi.Owner;
				sb.Append(s1);
				while (!(op is ILocalvariable) && op.Owner != null && op.Owner.Owner != null)
				{
					if (string.CompareOrdinal(s1, op.Owner.CodeName) != 0)
					{
						s1 = op.Owner.CodeName;
						sb.Insert(0, "->");
						sb.Insert(0, s1);
					}
					op = op.Owner;
				}
				if (op is ILocalvariable || op.Owner is ILocalvariable)
				{
				}
				else
				{
					sb.Insert(0, "$this->");
				}
				s1 = sb.ToString();
				string indents = Indentation.GetIndent();
				string s0 = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"\r\n{2}foreach ({0} as {1}) {{\r\n", s1, p.CodeName, indents);
				methodCode.Add(s0);
				//
				Method.SubMethod.Push(this);
				data.AddSubMethod(this);
				Indentation.IndentIncrease();
				bRet = base.OnExportPhpScriptCode(previousAction, nextAction, jsCode, methodCode, data);
				Indentation.IndentDecrease();
				Method.SubMethod.Pop();
				//
				methodCode.Add(indents);
				methodCode.Add("}\r\n");
			}
			else
			{
				string indents = Indentation.GetIndent();
				StringBuilder sb = new StringBuilder();
				string s1 = smi.Owner.CodeName;
				IObjectPointer op = smi.Owner;
				sb.Append(s1);
				while (!(op is ILocalvariable) && op.Owner != null && op.Owner.Owner != null)
				{
					if (string.CompareOrdinal(s1, op.Owner.CodeName) != 0)
					{
						s1 = op.Owner.CodeName;
						sb.Insert(0, "->");
						sb.Insert(0, s1);
					}
					op = op.Owner;
				}
				if (op is ILocalvariable)
				{
				}
				else
				{
					sb.Insert(0, "$this->");
				}
				s1 = sb.ToString();

				ParameterClassSubMethod p = mi.GetParameterType(1, smi, this);
				p.ParameterID = _actionData.ParameterValues[1].ParameterID;
				string v = p.CodeName;
				string s0 = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"\r\n{3}foreach ({0} as {1} => {2} ) {{\r\n", s1, mi.GetIndexCodePHP(smi, this.BranchId), v, indents);
				methodCode.Add(s0);
				Method.SubMethod.Push(this);
				data.AddSubMethod(this);
				//
				Indentation.IndentIncrease();
				bRet = base.OnExportPhpScriptCode(previousAction, nextAction, jsCode, methodCode, data);
				Method.SubMethod.Pop();
				//
				Indentation.IndentDecrease();
				methodCode.Add(indents);
				methodCode.Add("}\r\n");
			}

			return bRet;
		}

		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void LoadActionData(ClassPointer pointer, List<UInt32> usedBraches)
		{
			if (_actionData == null)
			{
				_actionData = (ActionSubMethod)ActionsHolder.GetActionInstance(_actId.ActionId);
			}
			base.LoadActionData(pointer, usedBraches);
		}
		public override void GetActions(List<IAction> actions, List<UInt32> usedBraches)
		{
			if (!usedBraches.Contains(this.BranchId))
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
				base.GetActions(actions, usedBraches);
			}
		}
		public override void GetActionIDs(List<UInt32> actionIDs, List<UInt32> usedBraches)
		{
			if (_actId != null)
			{
				if (!actionIDs.Contains(_actId.ActionId))
				{
					actionIDs.Add(_actId.ActionId);
				}
			}
			base.GetActionIDs(actionIDs, usedBraches);
		}
		public override bool AllBranchesEndWithMethodReturnStatement()
		{
			return IsMethodReturn;
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
			return base.ContainsActionId(actId, usedBraches);
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (!used.Contains(BranchId))
			{
				used.Add(BranchId);
				ActionSubMethod act = (ActionSubMethod)_actId.LoadActionInstance(this.ActionsHolder);
				if (act != null)
				{
					_actionData = act;
				}
				else
				{
					if (_actionData != null)
					{
						_actId.SetAction(_actionData);
					}
				}
				if (_actionData == null)
				{
					_actionData = (ActionSubMethod)designer.DesignerHolder.GetAction(_actId);
				}
				if (_actionData == null)
				{
					DesignUtil.WriteToOutputWindowAndLog("Action data for {0} not found for [{1}] calling {2}.LoadToDesigner", ActionId, this.Name, this.GetType().Name);
				}
				return designer.LoadAction(this);
			}
			return false;
		}
		public override void LinkActions(BranchList branches)
		{
		}
		public override void LinkJumpedBranches(BranchList branches)
		{
		}
		public override void RemoveOutOfGroupBranches(BranchList branches)
		{
		}
		public override void LinkJumpBranches()
		{
		}
		#endregion

		#region IPortOwner Members

		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Branch:{0}, Action count:{1}", BranchId, ActionCount);
			}
		}

		#endregion

		#region ISingleAction Members
		[ReadOnly(true)]
		public virtual IAction ActionData
		{
			get
			{
				if (_actionData == null)
				{
					_actionData = (ActionSubMethod)_actId.LoadActionInstance(this.ActionsHolder);
				}
				return _actionData;
			}
			set
			{
				_actionData = (ActionSubMethod)value;
				_parameters = null;
			}
		}

		public bool CanEditAction
		{
			get { return true; }
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
		[Browsable(false)]
		public EnumIconDrawType IconLayout
		{
			get
			{
				return _imgLayout;
			}
			set
			{
			}
		}

		public bool ShowText
		{
			get { return _showText; }
			set { _showText = value; }
		}

		#endregion

		#region IActionGroup Members
		public bool IsSubGroup
		{
			get
			{
				return true;
			}
		}

		public string GroupName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Loop Actions {0}", Name);
			}
		}
		public Type ViewerHolderType
		{
			get
			{
				return typeof(ActionGroupDesignerHolder);
			}
		}

		public UInt32 GroupId
		{
			get
			{
				return this.BranchId;
			}
		}
		public bool GroupFinished { get; set; }
		public void ResetGroupId(UInt32 groupId)
		{
			this.BranchId = groupId;
		}
		public override IActionGroup GetThreadGroup(UInt32 branchId)
		{
			if (this.ContainsAction(branchId))
			{
				return this;
			}
			return null;
		}
		#endregion

		#region ICloneable Members
		public override object Clone()
		{
			AB_SubMethodAction obj = (AB_SubMethodAction)base.Clone();
			obj._actId = _actId;
			obj._actionData = _actionData;
			obj._imgLayout = _imgLayout;
			if (_iconList != null)
			{
				obj._iconList = new List<ComponentIcon>();
				foreach (ComponentIcon c in _iconList)
				{
					obj._iconList.Add((ComponentIcon)c.Clone());
				}
			}
			return obj;
		}
		public void CopyActionsFrom(AB_Squential actions, List<ComponentIcon> icons)
		{
			this.Name = actions.Name;
			this.Description = actions.Description;
			this.EditorBounds = actions.EditorBounds;
			_iconList = icons;
			ActionList = actions.ActionList;
		}
		#endregion

		#region IMethod0 Members
		[ReadOnly(true)]
		[Browsable(false)]
		public string MethodName
		{
			get
			{
				return Name;
			}
			set
			{
				Name = value;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				ActionSubMethod asm = (ActionSubMethod)ActionData;
				if (asm == null)
					return 0;
				if (asm.ActionMethod == null)
					return 0;
				if (asm.ActionMethod.MethodPointed == null)
					return 0;
				return asm.ActionMethod.MethodPointed.ParameterCount;
			}
		}
		/// <summary>
		/// a list of ParameterClassSubMethod
		/// </summary>
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				if (_parameters == null)
				{
					List<IParameter> ips = new List<IParameter>();
					IAction a = ActionData;
					if (a == null)
					{
						a = ActionData;
					}
					ActionSubMethod asm = a as ActionSubMethod;
					if (asm != null)
					{
						List<ParameterClassSubMethod> ps = asm.GetParameters(this);
						if (ps != null && ps.Count > 0)
						{
							foreach (ParameterClassSubMethod p in ps)
							{
								ips.Add(p);
							}
							_parameters = ips;
						}
					}
				}
				return _parameters;
			}
		}

		#endregion

		#region IPostRootDeserialize Members

		public void OnPostRootDeserialize()
		{
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconActionBranchParameter ciap = ci as ComponentIconActionBranchParameter;
					if (ciap != null)
					{
						ParameterClassCollectionItem pcci = ciap.ClassPointer as ParameterClassCollectionItem;
						if (pcci != null && pcci.BaseClassType != null)
						{
							if (pcci.BaseClassType.IsGenericParameter)
							{
								if (pcci.ConcreteType == null)
								{
									if (ActionData != null)
									{
										MethodPointer mp = this.ActionData.ActionMethod as MethodPointer;
										if (mp != null)
										{
											DataTypePointer dp = mp.GetConcreteType(pcci.BaseClassType);
											if (dp != null)
											{
												pcci.SetConcreteType(dp);
											}
										}
									}
								}
							}
							else if (pcci.BaseClassType.IsGenericType && pcci.BaseClassType.ContainsGenericParameters)
							{
								if (pcci.TypeParameters == null)
								{
									if (ActionData != null)
									{
										MethodPointer mp = this.ActionData.ActionMethod as MethodPointer;
										if (mp != null)
										{
											DataTypePointer dp = mp.GetConcreteType(pcci.BaseClassType);
											if (dp != null)
											{
												pcci.TypeParameters = dp.TypeParameters;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		#endregion

		#region ISubMethod
		[Browsable(false)]
		public void RemoveLocalVariable(ComponentIconLocal v)
		{
			List<UInt32> usedBranches = new List<uint>();
			this.RemoveLocalVariable(v, usedBranches);

		}
		#endregion
	}
}
