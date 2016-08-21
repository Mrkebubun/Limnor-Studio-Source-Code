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
using System.Xml;
using ProgElements;
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// represent a for(int i=0;i < Count; i++) {...}
	/// </summary>
	public class AB_ForLoop : AB_Squential, IActionGroup, ISubMethod, IMethod0
	{
		#region fields and constructors
		private List<ComponentIcon> _iconList;
		private ParameterValue _count;
		private ActionBranchParameter _index;
		public AB_ForLoop(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_ForLoop(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_ForLoop(IActionListHolder ah)
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
				if (_count != null)
				{
					if (_count.MathExpression != null)
					{
						MathNodeRoot r = _count.MathExpression as MathNodeRoot;
						if (r != null)
						{
							if (r.UseInput)
							{
								return true;
							}
						}
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
		[Description("The number of times the actions will be executed")]
		public virtual ParameterValue RepeatCount
		{
			get
			{
				if (_count == null)
				{
					_count = new ParameterValue(this);
					_count.Name = "RepeatCount";
					_count.ConstantValue = new ConstObjectPointer("RepeatCount", typeof(int));
				}
				return _count;
			}
			set
			{
				_count = value;
			}
		}
		[Browsable(false)]
		public ActionBranchParameter RepeatIndex
		{
			get
			{
				if (_index == null)
				{
					_index = new ActionBranchParameter(typeof(int), "RepeatIndex", this);
				}
				return _index;
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
			if (_count != null)
			{
				//TBD
			}
		}
		protected override void OnGetActionNames(StringCollection sc, List<UInt32> usedBranches)
		{
			base.OnGetActionNames(sc, usedBranches);
		}
		protected override void OnEstablishObjectOwnership(IActionsHolder scope, List<UInt32> usedBranches)
		{
			base.OnEstablishObjectOwnership(scope, usedBranches);

			if (RepeatCount.MathExpression != null)
			{
				MathNodeRoot r = RepeatCount.MathExpression as MathNodeRoot;
				if (r != null)
				{
					IList<ISourceValuePointer> l = r.GetValueSources();
					if (l != null)
					{
						foreach (ISourceValuePointer sv in l)
						{
							sv.SetValueOwner(scope.OwnerMethod);
						}
					}
				}
			}
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ci.EstablishObjectOwnership(scope.OwnerMethod);
				}
			}
			List<ComponentIcon> cis = ComponentIconList;
			foreach (ComponentIcon ci in cis)
			{
				ComponentIconActionBranchParameter ab = ci as ComponentIconActionBranchParameter;
				if (ab != null)
				{
					if (ab.ClassPointer == null)
					{
						if (ab.MemberId == this.BranchId)
						{
							ab.ClassPointer = RepeatIndex;
							ab.DoNotSaveData = true;
						}
						else
						{
							ActionBranch ab0 = scope.FindActionBranchById(ab.MemberId);
							if (ab0 == null)
							{
								throw new DesignerException("Action branch [{0}] not found", ab.MemberId);
							}
							else
							{
								AB_ForLoop abf = ab0 as AB_ForLoop;
								if (abf != null)
								{
									ab.ClassPointer = abf.RepeatIndex;
									ab.DoNotSaveData = true;
								}
							}
						}
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
			if (RepeatCount.MathExpression != null)
			{
				MathNodeRoot r = RepeatCount.MathExpression as MathNodeRoot;
				if (r != null)
				{
					r.SetActionInputName(name, type.BaseClassType);
				}
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
			RepeatCount.ScopeMethod = m;
			if (RepeatCount.MathExpression != null)
			{
				MathNodeRoot r = RepeatCount.MathExpression as MathNodeRoot;
				if (r != null)
				{
					r.Project = m.Project;
				}
			}

		}
		public override bool LoadToDesigner(List<UInt32> used, MethodDiagramViewer designer)
		{
			return designer.LoadAction(this);
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			AB_ForLoop obj = (AB_ForLoop)base.Clone();
			if (_count != null)
			{
				_count.SetCloneOwner(obj);
				obj._count = (ParameterValue)_count.Clone();
			}
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
		#endregion
		#region ActionBranch implementation
		public override void CollectSourceValues(UInt32 taskId, List<UInt32> usedBranches, MethodClass mc)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				return;
			}
			usedBranches.Add(this.BranchId);
			if (_count != null)
			{
				List<ISourceValuePointer> l = new List<ISourceValuePointer>();
				_count.GetValueSources(l);
				if (taskId != 0)
				{
					foreach (ISourceValuePointer p in l)
					{
						p.SetTaskId(taskId);
					}
				}
				mc.AddValueSources(l);
			}
			base.CollectSourceValues(taskId, usedBranches, mc);
		}
		public override bool UseClientServerValues(List<UInt32> usedBranches, bool client)
		{
			if (usedBranches.Contains(this.BranchId))
			{
				usedBranches.Add(this.BranchId);
				if (_count != null)
				{
					List<ISourceValuePointer> l = new List<ISourceValuePointer>();
					_count.GetValueSources(l);
					if (l != null && l.Count > 0)
					{
						foreach (ISourceValuePointer p in l)
						{
							if (client)
							{
								if (p.IsWebClientValue())
								{
									return true;
								}
							}
							else
							{
								if (!p.IsWebClientValue())
								{
									return true;
								}
							}
						}
					}
				}
				base.UseClientServerValues(usedBranches, client);
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="compiler"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		public override bool OnExportCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements)
		{
			string indexName = RepeatIndex.CodeName;
			CodeExpression c = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexName), CodeBinaryOperatorType.LessThan, RepeatCount.GetReferenceCode(Method, statements, true));
			CodeIterationStatement cis = new CodeIterationStatement();
			cis.TestExpression = c;
			cis.InitStatement = new CodeVariableDeclarationStatement(typeof(int), indexName, new CodePrimitiveExpression(0));
			cis.IncrementStatement = new CodeSnippetStatement(indexName + "++");
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
			SetWithinLoop();
			Method.SubMethod.Push(this);
			CompilerUtil.AddSubMethod(method, this);
			bool bRet = base.OnExportCode(previousAction, nextAction, compiler, method, cis.Statements);
			Method.SubMethod.Pop();
			bRet = CompilerUtil.FinishSubMethod(method, this, cis.Statements, bRet);
			return bRet;
		}
		public override bool OnExportJavaScriptCode(ActionBranch previousAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
			string indexName = RepeatIndex.CodeName;
			methodCode.Add(Indentation.GetIndent());
			methodCode.Add("for(");
			methodCode.Add(indexName);
			methodCode.Add("=0;");
			methodCode.Add(indexName);
			methodCode.Add("<");
			methodCode.Add(RepeatCount.CreateJavaScript(methodCode));
			methodCode.Add(";");
			methodCode.Add(indexName);
			methodCode.Add("++) {\r\n");
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
			SetWithinLoop();
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
			string indexName = RepeatIndex.CodeName;
			methodCode.Add(Indentation.GetIndent());
			methodCode.Add("for(");
			methodCode.Add(indexName);
			methodCode.Add("=0;");
			methodCode.Add(indexName);
			methodCode.Add("<");
			methodCode.Add(RepeatCount.CreateJavaScript(methodCode));
			methodCode.Add(";");
			methodCode.Add(indexName);
			methodCode.Add("++) {\r\n");
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
			SetWithinLoop();
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
		public override void OnExportClientServerCode(ActionBranch previousAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, CodeMemberMethod method, CodeStatementCollection statements,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public override void Execute(List<ParameterClass> eventParameters)
		{
			CompileResult cr = null;
			int i = 0;
			while (true)
			{
				if (RepeatCount.ValueType == EnumValueType.ConstantValue)
				{
					if (i < (int)RepeatCount.ConstantValue.GetValue(ConstObjectPointer.VALUE_Value))
					{
						break;
					}
				}
				else if (RepeatCount.ValueType == EnumValueType.MathExpression)
				{
					MathNodeRoot r = RepeatCount.MathExpression as MathNodeRoot;
					if (r == null)
						break;
					else
					{
						cr = r.DebugCompileUnit;
						cr.Execute();
						if (!Convert.ToBoolean(cr.ReturnValue))
						{
							break;
						}
					}
				}
				else
				{
					IObjectPointer p = RepeatCount.Property;
					if (p == null)
					{
						break;
					}
					if (p.ObjectInstance == null)
					{
						break;
					}
					if (i < (int)(p.ObjectInstance))
					{
						break;
					}
				}
				ExecuteActions(eventParameters);
				i++;
			}
		}
		/// <summary>
		/// if this branch uses ParameterValue then use it to determine the type
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetParameterType(string name)
		{
			if (string.CompareOrdinal(name, "RepeatCount") == 0)
			{
				return new DataTypePointer(new TypePointer(typeof(int)));
			}
			return null;
		}
		/// <summary>
		/// get action branch parameter.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public override ActionBranchParameter GetActionBranchParameterByName(string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "RepeatIndex") == 0)
			{
				return RepeatIndex;
			}
			return null;
		}
		public override void FindItemByType<T>(List<T> results)
		{
			MathNodeRoot r = RepeatCount.MathExpression as MathNodeRoot;
			if (r != null)
			{
				r.FindItemByType<T>(results);
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
					"Repeat {0} times", RepeatCount.ToString());
			}
		}
		public override Type ViewerType
		{
			get { return typeof(ActionViwerForLoop); }
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
					"Repeat Actions ({0})", Name);
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
		#region IPropertiesWrapperOwner Members
		public override PropertyDescriptorCollection GetWrappedProperties(int id, Attribute[] attributes)
		{
			PropertyDescriptor[] pp = new PropertyDescriptor[1];
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal(p.Name, "RepeatCount") == 0)
				{
					pp[0] = p;
					break;
				}
			}
			return new PropertyDescriptorCollection(pp);
		}
		public override object GetPropertyOwner(int id, string propertyName)
		{
			return this;
		}
		#endregion
		#region ISubMethod members
		[Browsable(false)]
		public List<ComponentIcon> ComponentIconList
		{
			get
			{
				if (_iconList == null)
				{
					_iconList = new List<ComponentIcon>();
					ComponentIconActionBranchParameter cip = new ComponentIconActionBranchParameter(this);
					cip.ClassPointer = RepeatIndex;//.CreatePointer();
					cip.DoNotSaveData = true;
					if (cip.MemberId != RepeatIndex.MemberId)
					{
						throw new DesignerException("Error creating ComponentIconActionBranchParameter");
					}
					_iconList.Add(cip);
				}
				return _iconList;
			}
			set
			{
				_iconList = value;
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconActionBranchParameter cip = ci as ComponentIconActionBranchParameter;
					if (cip != null)
					{
						cip.ClassPointer = RepeatIndex.CreatePointer();
						cip.DoNotSaveData = true;
						break;
					}
				}
			}
		}
		[Browsable(false)]
		public void RemoveLocalVariable(ComponentIconLocal v)
		{
			List<UInt32> usedBranches = new List<uint>();
			this.RemoveLocalVariable(v, usedBranches);
			
		}
		#endregion
		#region ISerializationProcessor Members

		public override void OnDeserialization(XmlNode objectNode)
		{
			if (_iconList != null)
			{
				foreach (ComponentIcon ci in _iconList)
				{
					ComponentIconActionBranchParameter cip = ci as ComponentIconActionBranchParameter;
					if (cip != null)
					{
						cip.ClassPointer = RepeatIndex;
						cip.DoNotSaveData = true;
						break;
					}
				}
			}
		}

		#endregion

		#region IMethod0 Members
		[Browsable(false)]
		public virtual IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> list = new List<IParameter>();
				list.Add(RepeatIndex);
				return list;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string MethodName
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}
		[Browsable(false)]
		public virtual int ParameterCount
		{
			get { return 1; }
		}

		#endregion

	}
}
