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
using MathExp;
using System.Drawing;
using System.ComponentModel;
using System.CodeDom;
using System.Windows.Forms;
using ProgElements;
using System.Drawing.Design;
using System.Collections.Specialized;
using VPL;
using System.Xml;

namespace LimnorDesigner.Action
{
	public class AB_LoopActions : AB_Squential, IActionGroup, ISubMethod, IInlineActionHolder
	{
		#region fields and constructors
		private List<ComponentIcon> _iconList;
		private MathNodeRoot _logicExpression;
		private InlineAction _initAction;
		private InlineAction _increAction;
		public AB_LoopActions(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_LoopActions(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_LoopActions(IActionListHolder ah)
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
				if (_logicExpression != null)
				{
					if (_logicExpression.UseInput)
					{
						return true;
					}
				}
				return base.UseInput;
			}
		}
		/// <summary>
		/// indicates whether the action branch has action output.
		/// a loop cannot have return value
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
				if (_iconList != null)
				{
					foreach (ComponentIcon ci in _iconList)
					{
						ComponentIconLocal cil = ci as ComponentIconLocal;
						if (cil != null)
						{
						}
					}
				}
			}
		}
		[Description("Gets or sets the loop initialization action")]
		public InlineAction InitAction
		{
			get
			{
				if (_initAction == null)
					_initAction = new InlineAction();
				return _initAction;
			}
			set
			{
				_initAction = value;
			}
		}
		[Description("Gets or sets the action that is executed after each loop cycle.")]
		public InlineAction IncrementAction
		{
			get
			{
				if (_increAction == null)
				{
					_increAction = new InlineAction();
				}
				return _increAction;
			}
			set
			{
				_increAction = value;
			}
		}
		[Editor(typeof(UITypeEditorMathExpression2), typeof(UITypeEditor))]
		[Description("A math expression represents the condition for action executions. If this expression evaluates to false then the execution will stop.")]
		public MathNodeRoot Condition
		{
			get
			{
				if (_logicExpression == null)
				{
					_logicExpression = new MathNodeRoot();
					_logicExpression.ScopeMethod = Method;
					_logicExpression.Name = this.Name;
					LogicValueEquality e = new LogicValueEquality(_logicExpression);
					_logicExpression[1] = e;
				}
				return _logicExpression;
			}
			set
			{
				_logicExpression = value;
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
		public override void GetActionsUseLocalVariable(List<UInt32> usedBranches, UInt32 varId, Dictionary<UInt32, IAction> actions)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			base.GetActionsUseLocalVariable(usedBranches, varId, actions);
			if (_increAction != null && _increAction.Action != null)
			{
				if (!actions.ContainsKey(_increAction.Action.ActionId))
				{
					if (ClassPointer.IsRelatedAction(varId, _increAction.Action.CurrentXmlData))
					{
						actions.Add(_increAction.Action.ActionId, _increAction.Action);
					}
				}
			}
			if (_initAction != null && _initAction.Action != null)
			{
				if (!actions.ContainsKey(_initAction.Action.ActionId))
				{
					if (ClassPointer.IsRelatedAction(varId, _initAction.Action.CurrentXmlData))
					{
						actions.Add(_initAction.Action.ActionId, _initAction.Action);
					}
				}
			}
			if (_logicExpression != null)
			{
				//TBD
			}
		}
		public override void OnDeserialization(XmlNode objectNode)
		{
			base.OnDeserialization(objectNode);
			if (_initAction != null)
			{
				_initAction.OnPostRootDeserialize();
			}
			if (_increAction != null)
			{
				_increAction.OnPostRootDeserialize();
			}
		}
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			base.OnGetActionNames(sc, usedBranches);
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			base.OnEstablishObjectOwnership(scope, usedBranches);
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ci.EstablishObjectOwnership(scope.OwnerMethod);
				}
			}
			if (_logicExpression != null)
			{
				IList<ISourceValuePointer> spl = _logicExpression.GetValueSources();
				if (spl != null)
				{
					foreach (ISourceValuePointer sv in spl)
					{
						sv.SetValueOwner(scope.OwnerMethod);
					}
				}
			}
		}
		/// <summary>
		/// set the input variable name to all input variables
		/// </summary>
		/// <param name="name"></param>
		public override void SetInputName(string name, DataTypePointer type)
		{
			if (_logicExpression != null)
			{
				_logicExpression.SetActionInputName(name, type.BaseClassType);
			}
			base.SetInputName(name, type);
		}
		/// <summary>
		/// find the smallest group containing the branch
		/// </summary>
		/// <param name="id"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public override ActionBranch GetBranchInGroup(UInt32 id, ref IActionGroup group)
		{
			if (this.BranchId == id)
			{
				return this;
			}
			IActionGroup g = this;
			ActionBranch a = base.GetBranchInGroup(id, ref g);
			if (a != null)
			{
				group = g;
				return a;
			}
			return null;
		}
		public override void SetOwnerMethod(List<UInt32> used, MethodClass m)
		{
			base.SetOwnerMethod(used, m);
			if (!used.Contains(this.BranchId))
			{
				used.Add(this.BranchId);

				if (_logicExpression != null)
				{
					_logicExpression.Project = m.Project;
				}
			}
		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			if (!used.Contains(BranchId))
			{
				used.Add(BranchId);
				return designer.LoadAction(this);
			}
			return false;
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			SizeF sz = Condition.CalculateDrawSize(g);
			Bitmap img = new Bitmap((int)sz.Width, (int)sz.Height);
			Graphics gImg = Graphics.FromImage(img);
			gImg.FillRectangle(new SolidBrush(Color.White), 0, 0, (int)sz.Width, (int)sz.Height);
			Condition.Draw(gImg);
			gImg.Dispose();
			return img;
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_LoopActions obj = (AB_LoopActions)base.Clone();
			if (_logicExpression != null)
			{
				obj.Condition = (MathNodeRoot)_logicExpression.Clone();
			}
			if (_iconList != null)
			{
				obj._iconList = new List<ComponentIcon>();
				foreach (ComponentIcon c in _iconList)
				{
					obj._iconList.Add((ComponentIcon)c.Clone());
				}
			}
			if (_initAction != null)
			{
				obj._initAction = _initAction;
			}
			if (_increAction != null)
			{
				obj._increAction = _increAction;
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
			if (_logicExpression != null)
			{
				IList<ISourceValuePointer> lst = _logicExpression.GetValueSources();
				if (lst != null && lst.Count > 0)
				{
					if (taskId != 0)
					{
						foreach (ISourceValuePointer sp in lst)
						{
							sp.SetTaskId(taskId);
						}
					}
					mc.AddValueSources(lst);
				}
			}
			base.CollectSourceValues(taskId, usedBranches, mc);
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (!usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_logicExpression != null)
				{
					IList<ISourceValuePointer> lst = _logicExpression.GetValueSources();
					if (lst != null && lst.Count > 0)
					{
						foreach (ISourceValuePointer p in lst)
						{
							if (p.IsWebClientValue())
							{
								if (client)
								{
									return true;
								}
							}
							else
							{
								if (!client)
								{
									return true;
								}
							}
						}
					}
				}
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
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			string c;
			if (_logicExpression == null)
			{
				c = "true";
			}
			else
			{
				_logicExpression.PrepareForCompile(this.Method);
				c = _logicExpression.CreateJavaScript(methodCode);
			}
			methodCode.Add(Indentation.GetIndent());
			string initCodeStr = null;
			string increCodeStr = null;
			if (_initAction != null)
			{
				if (_initAction.Action == null)
				{
					_initAction.OnPostRootDeserialize();
				}
				if (_initAction.Action != null)
				{
					StringCollection initCode = new StringCollection();
					_initAction.Action.ExportJavaScriptCode(null, null, methodCode, initCode, data);
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < initCode.Count; i++)
					{
						sb.Append(initCode[i]);
					}
					initCodeStr = sb.ToString().Replace("\r\n", "");
				}
			}
			if (_increAction != null)
			{
				if (_increAction.Action == null)
				{
					_increAction.OnPostRootDeserialize();
				}
				if (_increAction.Action != null)
				{
					StringCollection increCode = new StringCollection();
					_increAction.Action.ExportJavaScriptCode(null, null, methodCode, increCode, data);
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < increCode.Count; i++)
					{
						sb.Append(increCode[i]);
					}
					increCodeStr = sb.ToString().Replace("\r\n", "");
				}
			}
			if (string.IsNullOrEmpty(initCodeStr))
			{
				initCodeStr = ";";
			}
			if (!string.IsNullOrEmpty(increCodeStr))
			{
				increCodeStr = increCodeStr.Trim();
				while (increCodeStr.EndsWith(";", StringComparison.Ordinal))
				{
					increCodeStr = increCodeStr.Substring(0, increCodeStr.Length - 1);
					increCodeStr = increCodeStr.Trim();
				}
			}
			methodCode.Add("for(");
			methodCode.Add(initCodeStr);
			methodCode.Add(c);
			methodCode.Add(";");
			methodCode.Add(increCodeStr);
			methodCode.Add(") {\r\n");
			Indentation.IndentIncrease();
			string indents = Indentation.GetIndent();
			StringCollection sc = new StringCollection();
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconLocal cil = ci as ComponentIconLocal;
					if (cil != null && cil.ScopeGroupId == this.BranchId)
					{
						sc.Add(indents);
						sc.Add("var ");
						sc.Add(cil.LocalPointer.CodeName);
						sc.Add("=");
						sc.Add(ObjectCreationCodeGen.ObjectCreateJavaScriptCode(VPLUtil.GetDefaultValue(cil.LocalPointer.BaseClassType)));
						sc.Add(";\r\n");
					}
				}
			}
			Method.SubMethod.Push(this);
			data.AddSubMethod(this);
			bool bRet = base.OnExportJavaScriptCode(previousAction, nextAction, jsCode, sc, data);
			Method.SubMethod.Pop();
			//
			for (int i = 0; i < sc.Count; i++)
			{
				methodCode.Add(sc[i]);
			}
			Indentation.IndentDecrease();
			methodCode.Add(Indentation.GetIndent());
			methodCode.Add("}\r\n");
			return bRet;
		}
		public override bool OnExportPhpScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			string c;
			if (_logicExpression == null)
			{
				c = "true";
			}
			else
			{
				_logicExpression.PrepareForCompile(this.Method);
				c = _logicExpression.CreatePhpScript(methodCode);
			}
			methodCode.Add(Indentation.GetIndent());
			string initCodeStr = null;
			string increCodeStr = null;
			if (_initAction != null && _initAction.Action != null)
			{
				StringCollection initCode = new StringCollection();
				_initAction.Action.ExportPhpScriptCode(null, null, methodCode, initCode, data);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < initCode.Count; i++)
				{
					sb.Append(initCode[i]);
				}
				initCodeStr = sb.ToString().Replace("\r\n", "");
			}
			if (_increAction != null && _increAction.Action != null)
			{
				StringCollection increCode = new StringCollection();
				_increAction.Action.ExportPhpScriptCode(null, null, methodCode, increCode, data);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < increCode.Count; i++)
				{
					sb.Append(increCode[i]);
				}
				increCodeStr = sb.ToString().Replace("\r\n", "");
			}
			if (string.IsNullOrEmpty(initCodeStr))
			{
				initCodeStr = ";";
			}
			if (!string.IsNullOrEmpty(increCodeStr))
			{
				increCodeStr = increCodeStr.Trim();
				while (increCodeStr.EndsWith(";", StringComparison.Ordinal))
				{
					increCodeStr = increCodeStr.Substring(0, increCodeStr.Length - 1);
					increCodeStr = increCodeStr.Trim();
				}
			}
			methodCode.Add("for(");
			methodCode.Add(initCodeStr);
			methodCode.Add(c);
			methodCode.Add(";");
			methodCode.Add(increCodeStr);
			methodCode.Add(") {\r\n");
			Indentation.IndentIncrease();
			string indents = Indentation.GetIndent();
			StringCollection sc = new StringCollection();
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconLocal cil = ci as ComponentIconLocal;
					if (cil != null && cil.ScopeGroupId == this.BranchId)
					{
						sc.Add(indents);
						sc.Add("$");
						sc.Add(cil.LocalPointer.CodeName);
						sc.Add("=");
						sc.Add(ValueTypeUtil.GetDefaultPhpScriptValueByType(cil.LocalPointer.BaseClassType));
						sc.Add(";\r\n");
					}
				}
			}
			Method.SubMethod.Push(this);
			data.AddSubMethod(this);
			bool bRet = base.OnExportPhpScriptCode(previousAction, nextAction, jsCode, sc, data);
			Method.SubMethod.Pop();
			//
			for (int i = 0; i < sc.Count; i++)
			{
				methodCode.Add(sc[i]);
			}
			Indentation.IndentDecrease();
			methodCode.Add(Indentation.GetIndent());
			methodCode.Add("}\r\n");
			return bRet;
		}
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			CodeExpression c;
			if (_logicExpression == null)
			{
				c = new CodePrimitiveExpression(true);
			}
			else
			{
				_logicExpression.PrepareForCompile(this.Method);
				c = _logicExpression.ExportCode(this.Method);
			}
			CodeIterationStatement cis = new CodeIterationStatement();
			cis.TestExpression = c;
			if (_initAction != null && _initAction.Action != null)
			{
				CodeStatementCollection sts = new CodeStatementCollection();
				_initAction.Action.ExportCode(null, null, compiler, Method, method, sts, false);
				if (sts.Count > 0)
				{
					for (int i = 0; i < sts.Count; i++)
					{
						if (!(sts[i] is CodeCommentStatement))
						{
							cis.InitStatement = sts[i];
							break;
						}
					}
				}
				else
				{
					cis.InitStatement = new CodeSnippetStatement();
				}
			}
			else
			{
				cis.InitStatement = new CodeSnippetStatement();
			}
			if (_increAction != null && _increAction.Action != null)
			{
				CodeStatementCollection sts = new CodeStatementCollection();
				_increAction.Action.ExportCode(null, null, compiler, Method, method, sts, false);
				if (sts.Count > 0)
				{
					for (int i = 0; i < sts.Count; i++)
					{
						if (!(sts[i] is CodeCommentStatement))
						{
							cis.IncrementStatement = sts[i];
							break;
						}
					}
				}
				else
				{
					cis.IncrementStatement = new CodeSnippetStatement();
				}
			}
			else
			{
				cis.IncrementStatement = new CodeSnippetStatement();
			}
			statements.Add(cis);
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
			Method.SubMethod.Push(this);
			CompilerUtil.AddSubMethod(method, this);
			bool bRet = base.OnExportCode(previousAction, nextAction, compiler, method, cis.Statements);
			Method.SubMethod.Pop();
			bRet = CompilerUtil.FinishSubMethod(method, this, cis.Statements, bRet);
			return bRet;
		}
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
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
		public override void Execute(List<ParameterClass> eventParameters)
		{
			CompileResult cr = null;
			bool test = true;
			if (_logicExpression != null)
			{
				cr = _logicExpression.DebugCompileUnit;
				cr.Execute();
				test = Convert.ToBoolean(cr.ReturnValue);
			}
			if (test)
			{
				while (test)
				{
					ExecuteActions(eventParameters);
					if (cr != null)
					{
						cr.Execute();
						test = Convert.ToBoolean(cr.ReturnValue);
					}
				}
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_logicExpression != null)
			{
				_logicExpression.FindItemByType<T>(results);
			}
			base.FindItemByType<T>(results);
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
		public override string TraceInfo
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Loop condition {0}", _logicExpression);
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViewerLoop); }
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
		#region IMethod0 Members
		[Browsable(false)]
		[ReadOnly(true)]
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
			get { return 0; }
		}
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get { return null; }
		}

		#endregion
		#region IPropertiesWrapperOwner Members
		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			int n = 0;
			Attribute[] attrs = new Attribute[n + 1];
			Attribute[] attrs2 = new Attribute[n + 2];
			attrs[n] = new EditorAttribute(typeof(UITypeEditorMathExpression2), typeof(UITypeEditor));
			attrs2[n] = new EditorAttribute(typeof(TypeEditorSelectAction), typeof(UITypeEditor));
			attrs2[n + 1] = new RefreshPropertiesAttribute(RefreshProperties.All);
			return new PropertyDescriptorCollection(new PropertyDescriptor[]{
             new DataValuePropertyDescriptor(this, attrs),
             new PropertyDescriptorInlineAction(this,attrs2,"InitAction"),
             new PropertyDescriptorInlineAction(this,attrs2,"IncrementAction")
            }
			);

		}
		public override object GetPropertyOwner(int id, string propertyName)
		{
			return this;
		}
		#endregion
		#region PropertyDescriptorInlineAction
		class PropertyDescriptorInlineAction : PropertyDescriptor
		{
			private AB_LoopActions _owner;
			private bool _isInit;
			public PropertyDescriptorInlineAction(AB_LoopActions owner, Attribute[] attributes, string name)
				: base(name, attributes)
			{
				_owner = owner;
				_isInit = string.CompareOrdinal(name, "InitAction") == 0;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				if (_isInit)
					return _owner.InitAction;
				else
					return _owner.IncrementAction;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(InlineAction); }
			}

			public override void ResetValue(object component)
			{
				if (_isInit)
					_owner.InitAction = null;
				else
					_owner.IncrementAction = null;
			}

			public override void SetValue(object component, object value)
			{
				if (_isInit)
					_owner.InitAction = value as InlineAction;
				else
					_owner.IncrementAction = value as InlineAction;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region DataValuePropertyDescriptor class definition

		private class DataValuePropertyDescriptor : PropertyDescriptor
		{
			private AB_LoopActions _owner;
			private ParameterValue _ev;
			public DataValuePropertyDescriptor(AB_LoopActions owner, Attribute[] attributes)
				: base("Condition", attributes)
			{
				_owner = owner;
				_ev = new ParameterValue(_owner);
				_ev.Name = "Expression";
				_ev.ValueType = EnumValueType.MathExpression;
				_ev.MathExpression = _owner.Condition;
				_ev.ScopeMethod = _owner.Method;
				_ev.SetParameterValueChangeEvent(onValueChanged);
			}
			private void onValueChanged(object sender, EventArgs e)
			{
				_owner.Condition = (MathNodeRoot)_ev.MathExpression;
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				return _ev;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(ParameterValue); }
			}

			public override void ResetValue(object component)
			{
				_owner.Condition = null;
			}

			public override void SetValue(object component, object value)
			{
				MathNodeRoot r = value as MathNodeRoot;
				if (r != null)
				{
					_owner.Condition = r;
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
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
