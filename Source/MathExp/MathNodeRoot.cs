/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Drawing.Design;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using ProgElements;
using XmlUtility;
using VPL;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Globalization;

namespace MathExp
{
	public interface IPlaceHolder
	{
		bool IsPlaceHolder { get; set; }
	}
	/// <summary>
	/// root for the whole expression
	/// </summary>
	[Description("a math expression")]
	public class MathNodeRoot : MathNode, IMathExpression, IUndoable<MathNodeRoot>, ICustomSerialization, IMethodPointer, ISourceValuePointersHolder, IXmlCodeReaderWriterHolder
	{
		#region Fields
		public const string XMLATT_FIXED = "fixed";
		public event EventHandler OnSetFocus;
		public event EventHandler OnChanged;
		//
		System.Drawing.Font _font;
		System.Drawing.Font _fontSuperscript;
		System.Drawing.Brush _brush;
		System.Drawing.Brush _brushFocus;
		System.Drawing.Brush _brushBKFocus;
		System.Drawing.Brush _brushBKFocus0;
		System.Drawing.Pen _pen;
		System.Drawing.Pen _penFocus;
		private bool _showAsEquation;
		private bool _enableUndo = true;
		private MathExpItem _container;
		private string _name = "math";
		private string _desc;
		private string _shortDesc;
		private Rectangle _editorBounds;
		private ObjectIconData _iconData;
		private bool _fixed;
		private bool _variableHolder;
		private bool _showVarMap = true;
		//
		private MathNode _selectedNode;
		private MathNode _highlightedNode;
		private string keybuffer = "";
		private List<MathNode> _usedNodes;
		private RaisDataType _actualCompiledDataType;
		private MathNodeVariableDummy varDummy;
		private Dictionary<Type, object> _localServices;
		private VariableList _inputs;
		private CompileResult _debugCompile;
		private object _project;
		private IMethod _methodScope;
		private VariableMap _varMap;
		private EventHandler _valueChangedHandler;
		//
		private MathExpCtrl _viewer;
		//
		private XmlNode _xmlNode;
		private SizeF _size;
		#endregion

		#region Constructors and initializers

		static MathNodeRoot()
		{
			MathNode.Init();
		}
		public MathNodeRoot()
			: base(null)
		{
			initDefault();
		}
		public MathNodeRoot(MathNode parent)
			: base(null)
		{
			initDefault();
		}
		private void initDefault()
		{
			_font = new System.Drawing.Font("Times New Roman", 12);
			_fontSuperscript = new System.Drawing.Font("Times New Roman", 8);
			_brush = System.Drawing.Brushes.Black;
			_brushFocus = System.Drawing.Brushes.White;
			_brushBKFocus = System.Drawing.Brushes.Blue;
			_brushBKFocus0 = System.Drawing.Brushes.LightGray;
			_pen = System.Drawing.Pens.Black;
			_penFocus = System.Drawing.Pens.White;
		}
		#endregion

		#region Code generation register
		/// <summary>
		/// called before compiling
		/// </summary>
		public void PrepareForCompile(IMethodCompile method)
		{
			_usedNodes = null;
			this.ResetVariableAttributes(method);
		}
		public void RegisterNodeUsage(MathNode node)
		{
			if (_usedNodes == null)
			{
				_usedNodes = new List<MathNode>();
			}
			_usedNodes.Add(node);
		}
		public bool IsNodeUsed(MathNode node)
		{
			if (_usedNodes != null)
			{
				return _usedNodes.Contains(node);
			}
			return false;
		}
		#endregion

		#region Public properties
		[XmlIgnore]
		[NotForProgramming]
		[ReadOnly(true)]
		[Browsable(false)]
		public Point AutoScrollPosition { get; set; }

		[XmlIgnore]
		[NotForProgramming]
		[Browsable(false)]
		public SizeF ExpSize
		{
			get
			{
				return _size;
			}
		}
		[Browsable(false)]
		public bool IsStringSelected
		{
			get
			{
				if (_selectedNode != null)
				{
					return (_selectedNode is MathNodeStringValue);
				}
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public MathExpCtrl Viewer
		{
			get
			{
				return _viewer;
			}
			set
			{
				_viewer = value;
			}
		}
		/// <summary>
		/// hook to the editor
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public EventHandler ValueChangedHandler
		{
			get
			{
				return _valueChangedHandler;
			}
			set
			{
				_valueChangedHandler = value;
				if (_varMap != null)
				{
					foreach (KeyValuePair<IVariable, ICompileableItem> kv in _varMap)
					{
						kv.Value.SetParameterValueChangeEvent(_valueChangedHandler);
					}
				}
			}
		}
		private Type _vtt;
		private IActionContext _ac;
		/// <summary>
		/// when it is used within an action definition, this is the action.
		/// It is needed before loading from XML if variable map is used.
		/// It is needed for creating other objects in action-context, such as ParameterValue
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionContext ActionContext { get { return _ac; } set { _ac = value; } }
		/// <summary>
		/// the type for the mapped instance.
		/// currently it is ParameterValue.
		/// It is needed before loading from XML if variable map is used
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Type VariableMapTargetType
		{
			get
			{
				if (_vtt == null)
				{
					return VPLUtil.VariableMapTargetType;
				}
				return _vtt;
			}
			set
			{
				_vtt = value;
			}
		}
		[Browsable(false)]
		public VariableMap VariableMap
		{
			get
			{
				if (_varMap == null)
				{
					_varMap = new VariableMap();
				}
				return _varMap;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod ScopeMethod
		{
			get
			{
				return _methodScope;
			}
			set
			{
				_methodScope = value;
				SetScopeMethod(_methodScope);
			}
		}
		[Browsable(false)]
		public object Project
		{
			get
			{
				return _project;
			}
			set
			{
				_project = value;
			}
		}
		[Browsable(false)]
		public bool IsVariableHolder
		{
			get
			{
				return _variableHolder;
			}
			set
			{
				_variableHolder = value;
			}
		}
		[Browsable(false)]
		public bool EnableUndo
		{
			get
			{
				return _enableUndo;
			}
			set
			{
				_enableUndo = value;
			}
		}
		[Browsable(false)]
		public Point Offset
		{
			get
			{
				return Position;
			}
			set
			{
				Position = value;
			}
		}
		public System.Drawing.Font FontSuperscript
		{
			get
			{
				return _fontSuperscript;
			}
			set
			{
				_fontSuperscript = value;
			}
		}
		public System.Drawing.Font Font
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

		[Browsable(false)]
		public bool ShowAsEquation
		{
			get
			{
				return _showAsEquation;
			}
			set
			{
				_showAsEquation = value;
			}
		}
		[Browsable(false)]
		public CompileResult DebugCompileUnit
		{
			get
			{
				if (_debugCompile == null)
				{
					_debugCompile = CreateMethodCompilerUnit("DebugRun", "Debug" + ID.ToString("x"), "Run" + ID.ToString("x"));
				}
				return _debugCompile;
			}
		}
		#endregion

		#region public methods

		public CompileResult CreateMethodCompilerUnit(string classNamespace, string className, string methodName)
		{
			MathNode.Trace("Unit Test Code-generation starts at {0} ===Expression============", DateTime.Now);
			//
			//remember parent nodes owning pointers
			Dictionary<MathNode, Dictionary<Int32, MathNode>> pointerOwners = new Dictionary<MathNode, Dictionary<int, MathNode>>();
			ReplacePointers(pointerOwners);
			//
			try
			{
				GenerateInputVariables();
				//
				MethodType mt = new MethodType();
				MathNode.PrepareMethodCreation();
				PrepareForCompile(mt);
				CodeCompileUnit code = new CodeCompileUnit();
				CodeNamespace ns = new CodeNamespace(classNamespace);
				code.Namespaces.Add(ns);
				//
				AssemblyRefList imports = new AssemblyRefList();
				GetAllImports(imports);
				MathNode.Trace("Imports:{0}", imports.Count);
				MathNode.IndentIncrement();
				foreach (AssemblyRef sA in imports)
				{
					MathNode.Trace(sA.Name);
					ns.Imports.Add(new CodeNamespaceImport(sA.Name));
				}
				MathNode.IndentDecrement();
				//type declaration
				CodeTypeDeclaration t = new CodeTypeDeclaration(className);
				ns.Types.Add(t);
				//test method 
				CodeMemberMethod m = new CodeMemberMethod();
				mt.MethodCode = m;
				t.Members.Add(m);
				m.ReturnType = new CodeTypeReference(DataType.Type);
				m.Name = methodName;
				m.Attributes = MemberAttributes.Static | MemberAttributes.Public;
				//
				m.Comments.Add(new CodeCommentStatement("Variable mapping:"));
				m.Comments.Add(new CodeCommentStatement("In formula: method parameter"));
				//
				mt.MethodCode = m;
				//
				VariableList variables = InputVariables;
				VariableList parameters = new VariableList();
				//
				foreach (IVariable v in variables)
				{
					v.NoAutoDeclare = true;
					this[1].SetNoAutoDeclare(v.KeyName);
				}
				//
				MathNode.Trace("Generate statements");
				ExportAllCodeStatements(mt);
				MathNode.Trace("\tstatements.Count={0}", m.Statements.Count);
				int n = variables.Count;
				MathNode.Trace("Generate arguments from {0} variables", n);
				MathNode.IndentIncrement();
				for (int k = 0; k < n; k++)
				{
					IVariable var = variables[k];
					MathNode.Trace(k, var);
					if (!(var is MathNodeVariableDummy) && !var.IsParam && !var.IsConst)
					{
						string paramName = "";
						parameters.Add(var);
						paramName = var.CodeVariableName;
						var.NoAutoDeclare = true;
						CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(new CodeTypeReference(var.VariableType.Type), paramName);
						m.Parameters.Add(p);
						//add comment
						string sub = var.SubscriptName;
						if (string.IsNullOrEmpty(sub))
							sub = " ";
						m.Comments.Add(new CodeCommentStatement(string.Format("{0}{1}:\t {2}", var.VariableName, sub, var.CodeVariableName)));
						MathNode.IndentIncrement();
						MathNode.Trace("Argument {0} {1} for {2}, {3}", var.VariableType.Type, paramName, var.TraceInfo, var.GetType());
						MathNode.IndentDecrement();
						AssignCodeExp(new CodeArgumentReferenceExpression(paramName), var.CodeVariableName);
					}
				}
				//create parameters for pointers
				Dictionary<string, IPropertyPointer> pointers = new Dictionary<string, IPropertyPointer>();
				List<IPropertyPointer> pointerList = new List<IPropertyPointer>();
				GetPointers(pointers);
				MathNode.Trace("Generate arguments from {0} pointers", pointers.Count);
				foreach (KeyValuePair<string, IPropertyPointer> kv in pointers)
				{
					string paramName = "";
					pointerList.Add(kv.Value);
					paramName = kv.Value.CodeName;
					CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(new CodeTypeReference(kv.Value.ObjectType), paramName);
					m.Parameters.Add(p);
					//add comment
					m.Comments.Add(new CodeCommentStatement(string.Format("{0}:\t {1}", kv.Value.ToString(), paramName)));
					MathNode.IndentIncrement();
					MathNode.Trace("Argument {0} {1} for {2}", kv.Value.ObjectType, paramName, kv.Value.ToString());
					MathNode.IndentDecrement();
				}
				MathNode.IndentDecrement();
				MathNode.Trace("Generate code");
				CodeExpression exp = ExportCode(mt);
				if (!DataType.Type.Equals(ExpDataType.Type))
				{
					if (ExpDataType.Type.Equals(typeof(void)))
					{
						MathNode.Trace("Function returns {0}. Test method returns {1}", ExpDataType.Type, DataType.Type);
						m.Statements.Add(new CodeExpressionStatement(exp));
						exp = ValueTypeUtil.GetDefaultCodeByType(DataType.Type);
					}
					else
					{
						MathNode.Trace("Convert {0} to type {1}", ExpDataType.Type, DataType.Type);
						exp = VPLUtil.ConvertByType(DataType.Type, exp);
					}
				}
				CodeMethodReturnStatement mr = new CodeMethodReturnStatement(exp);
				m.Statements.Add(mr);
				MathNode.Trace("Unit Test Code-generation ends at {0} ===============", DateTime.Now);
				return new CompileResult(classNamespace, className, methodName, code, pointerList, parameters, imports);
			}
			finally
			{
				if (pointerOwners.Count > 0)
				{
					foreach (KeyValuePair<MathNode, Dictionary<int, MathNode>> kv in pointerOwners)
					{
						foreach (KeyValuePair<int, MathNode> kv2 in kv.Value)
						{
							kv.Key[kv2.Key] = kv2.Value;
						}
					}
					GenerateInputVariables();
				}
			}
		}
		public void AddVariableMapItem(MathNode node)
		{
			IVariable v = node as IVariable;
			if (v != null)
			{
				AddVariableMapItem(v);
			}
			else
			{
				AdjustVariableMap();
			}
		}
		public void AddVariableMapItem(IVariable v)
		{
			if (VariableMapTargetType != null && ActionContext != null)
			{
				if (_varMap == null)
				{
					_varMap = new VariableMap();
				}
				if (!_varMap.VariableExists(v))
				{
					//create ParameterValue
					object mapped = Activator.CreateInstance(VariableMapTargetType, ActionContext);
					ICompileableItem c = (ICompileableItem)mapped;
					c.SetCustomMethod(ScopeMethod);
					c.ParameterID = v.ParameterID;
					c.Name = v.KeyName;
					c.SetParameterValueChangeEvent(ValueChangedHandler);
					c.SetDataType(this.DataType.LibType);
					_varMap.Add(v, c);
				}
			}
		}
		/// <summary>
		/// remove deleted variables and add new variables
		/// </summary>
		public void AdjustVariableMap()
		{
			VariableList vlist = FindAllInputVariables();
			//remove duplicated ones
			StringCollection scDup = new StringCollection();
			VariableList vr = new VariableList();
			foreach (IVariable v1 in vlist)
			{
				if (!v1.IsLocal && !v1.IsParam && !v1.IsDummyPort && !v1.IsReturn && !v1.NoAutoDeclare)
				{
					if (vr.ContainsVariable(v1) == null)
					{
						vr.Add(v1);
					}
				}
				else
				{
					if (!scDup.Contains(v1.KeyName))
					{
						scDup.Add(v1.KeyName);
					}
				}
			}
			VariableList dup = new VariableList();
			foreach (string vk in scDup)
			{
				foreach (IVariable v1 in vr)
				{
					if (string.CompareOrdinal(v1.KeyName, vk) == 0)
					{
						dup.Add(v1);
					}
				}
			}
			foreach (IVariable v1 in dup)
			{
				vr.Remove(v1);
			}
			vlist = vr;
			if (vlist.Count == 0)
			{
				_varMap = new VariableMap();
			}
			else
			{
				if (_varMap == null || _varMap.Count == 0)
				{
					if (_varMap == null)
					{
						_varMap = new VariableMap();
					}
				}
				else
				{
					dup = new VariableList();
					List<IVariable> removedKeys = new List<IVariable>();
					foreach (KeyValuePair<IVariable, ICompileableItem> kv in _varMap)
					{
						if (vlist.ContainsVariable(kv.Key) == null)
						{
							removedKeys.Add(kv.Key);
						}
						else
						{
							if (dup.ContainsVariable(kv.Key) == null)
							{
								dup.Add(kv.Key);
							}
							else
							{
								removedKeys.Add(kv.Key);
							}
						}
					}
					foreach (IVariable v in removedKeys)
					{
						_varMap.DeleteVariable(v);
					}
				}
				foreach (IVariable v in vlist)
				{
					if (!(v is MathNodeVariableDummy))
					{
						if (!v.IsLocal && !v.IsParam && !v.IsDummyPort && !v.IsReturn && !v.NoAutoDeclare)
						{
							AddVariableMapItem(v);
						}
					}
				}
				foreach (KeyValuePair<IVariable, ICompileableItem> kv in _varMap)
				{
					MathNode mn = kv.Key as MathNode;
					if (mn != null)
					{
						mn.CachedImage = null;
					}
					kv.Value.Name = kv.Key.KeyName;//mapping
					kv.Value.SetParameterValueChangeEvent(mapItemChanged);
				}
			}
		}
		/// <summary>
		/// grow a simple math expression into a group to hold several math expressions
		/// </summary>
		/// <returns></returns>
		public MathExpGroup CreateMathExpGroup(string name)
		{
			MathExpGroup g = new MathExpGroup();
			g.AddItem(this);
			MathExpItem item = g.Expressions[0];
			//
			g.Name = name;
			item.Location = new System.Drawing.Point(100, 60);
			item.Size = new System.Drawing.Size(357, 38);
			//
			//create in/out ports
			//===create dummy input========================================
			VariableList vs = root.FindAllInputVariables();
			if (vs[0].InPort == null)
			{
				vs[0].InPort = new LinkLineNodeInPort(vs[0]);
			}
			vs[0].InPort.Position = item.Size.Width / 2 - vs[0].InPort.Size.Width / 2;
			vs[0].InPort.Location = new System.Drawing.Point(item.Location.X + vs[0].InPort.Position, item.Location.Y - vs[0].InPort.Size.Height);
			vs[0].InPort.SaveLocation();
			vs[0].InPort.Label.SaveLocation();
			((Control)vs[0].InPort.PrevNode).Location = new System.Drawing.Point(vs[0].InPort.Location.X, vs[0].InPort.Location.Y - 30);
			((ActiveDrawing)((Control)vs[0].InPort.PrevNode)).SaveLocation();
			//===make the link from method call to the action return=======
			g.ReturnIcon.ReturnVariable.InPort.LinkedPortID = ((MathNodeVariable)root[0]).ID;
			((MathNodeVariable)root[0]).OutPorts = new LinkLineNodeOutPort[] { new LinkLineNodeOutPort((MathNodeVariable)root[0]) };
			((MathNodeVariable)root[0]).OutPorts[0].LinkedPortID = g.ReturnIcon.ReturnVariable.InPort.PortID;
			((MathNodeVariable)root[0]).OutPorts[0].Location = new System.Drawing.Point(vs[0].InPort.Location.X, item.Location.Y + item.Size.Height);
			((MathNodeVariable)root[0]).OutPorts[0].Position = vs[0].InPort.Position;
			((MathNodeVariable)root[0]).OutPorts[0].SaveLocation();
			return g;
		}
		#endregion

		#region Drawing Attributes
		public void SetFont(System.Drawing.Font font)
		{
			_font = font;
			_fontSuperscript = new Font(font.FontFamily, font.Size / (float)2, font.Unit);
		}
		public override System.Drawing.Font TextFont
		{
			get
			{
				return _font;
			}
		}
		[Browsable(false)]
		public System.Drawing.Font TextFontSuperscript
		{
			get
			{
				return _fontSuperscript;
			}
		}
		public void SetBrush(System.Drawing.Brush brush)
		{
			_brush = brush;
		}
		public override System.Drawing.Brush TextBrush
		{
			get
			{
				return _brush;
			}
		}
		public void SetBrushFocus(System.Drawing.Brush brush)
		{
			_brushFocus = brush;
		}
		public override System.Drawing.Brush TextBrushFocus
		{
			get
			{
				return _brushFocus;
			}
		}
		public void SetBrushBKFocus(System.Drawing.Brush brush)
		{
			_brushBKFocus = brush;
		}
		public override System.Drawing.Brush TextBrushBKFocus
		{
			get
			{
				return _brushBKFocus;
			}
		}
		public override System.Drawing.Brush TextBrushBKFocus0
		{
			get
			{
				return _brushBKFocus0;
			}
		}
		public void SetBrushBKFocus0(Brush brush)
		{
			_brushBKFocus0 = brush;
		}
		public void SetPen(System.Drawing.Pen pen)
		{
			_pen = pen;
		}
		public override System.Drawing.Pen TextPen
		{
			get
			{
				return _pen;
			}
		}
		public void SetPenFocus(System.Drawing.Pen pen)
		{
			_penFocus = pen;
		}
		public override System.Drawing.Pen TextPenFocus
		{
			get
			{
				return _penFocus;
			}
		}
		#endregion

		#region Selection
		public void FireSetFocus(MathNode sender)
		{
			if (OnSetFocus != null)
			{
				OnSetFocus(sender, null);
			}
		}
		public void FireChanged(MathNode sender)
		{
			if (OnChanged != null)
			{
				OnChanged(sender, null);
			}
		}
		[Browsable(false)]
		public MathNode FocusedNode
		{
			get
			{
				return _selectedNode;
			}
		}
		public void SetFocus(MathNode node)
		{
			if (node != _selectedNode)
			{
				if (_selectedNode != null)
				{
					_selectedNode.IsFocused = false;
				}
				_selectedNode = node;
				if (_selectedNode != null)
				{
					_selectedNode.IsFocused = true;

				}
				else
				{
					if (OnSetFocus != null)
					{
						OnSetFocus(null, null);
					}
				}
			}
			else
			{
				if (OnSetFocus != null)
				{
					OnSetFocus(node, EventArgs.Empty);
				}
			}
			keybuffer = "";
		}
		public void SelectNext()
		{
			if (_selectedNode == null)
				_selectedNode = this[1];
			else
				_selectedNode = _selectedNode.FocusNext(_selectedNode);
			ClearFocus();
			if (_selectedNode != null)
			{
				_selectedNode.IsFocused = true;
				keybuffer = "";
			}
		}
		public void SelectPrevious()
		{
			if (_selectedNode == null)
				_selectedNode = this[1];
			else
				_selectedNode = _selectedNode.FocusPrevios(_selectedNode);
			ClearFocus();
			if (_selectedNode != null)
			{
				_selectedNode.IsFocused = true;
				keybuffer = "";
			}
		}
		public void UpdateSelected(string s)
		{
			if (_selectedNode != null && !_selectedNode.ReadOnly)
			{
				string ss = keybuffer + s;
				if (_selectedNode.Update(ss))
				{
					keybuffer = ss;
				}
			}
		}
		#endregion

		#region Highlighting
		[Browsable(false)]
		public MathNode HightLighted
		{
			get
			{
				return _highlightedNode;
			}
		}
		public HitTestResult HightLight(PointF point)
		{
			HitTestResult ret = null;
			MathNode h = this.HitTest(point);
			if (h != null && h != _highlightedNode)
			{
				if (_highlightedNode != null)
				{
					_highlightedNode.IsHighlighted = false;
				}
				h.IsHighlighted = true;
				ret = new HitTestResult(_highlightedNode, h);
				_highlightedNode = h;
			}
			return ret;
		}
		#endregion

		#region Editing

		public void AddEmptyNode()
		{
			SaveCurrentStateForUndo(); //save the current state
			MathNode current = _selectedNode;
			if (current == null || current == this[0])
				current = this[1];
			PlusNode node = new PlusNode(current.Parent);
			int n = current.Parent.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (current.Parent[i] == current)
				{
					current.Parent[i] = node;
					break;
				}
			}
			node[0] = current;
			SetFocus(node);
			FireChanged(node);
		}
		public static MathNode CreateNewNode(Type type, MathNode parent)
		{
			MathNode node = (MathNode)Activator.CreateInstance(type, new object[] { parent });
			return node;
		}
		public MathNode ReplaceMathNode(MathNode curNode, Type type)
		{
			if (curNode is MathNodeRoot)
			{
				curNode = curNode[1];
			}
			if (!curNode.OnReplaceNode(type))
			{
				MathNode node = CreateNewNode(type, curNode.Parent);
				if (replaceNode(curNode, node))
				{
					return node;
				}
			}
			return null;
		}
		private Form getForm()
		{
			if (_viewer != null)
			{
				return _viewer.FindForm();
			}
			return null;
		}
		private bool replaceNode(MathNode curNode, MathNode node)
		{
			if (curNode.ReplaceMe(node))
			{
				bool b = true;
				if (curNode is MathNodeNumber)
				{
					if (((MathNodeNumber)curNode).IsPlaceHolder)
						b = false;
				}
				if (b)
				{
					if (!curNode.IsChild(node))
					{
						node.OnReplaceNode(curNode);
					}
				}
				return true;
			}
			return false;
		}
		public MathNode AddMathNode(Type type)
		{
			SaveCurrentStateForUndo(); //save the current state
			MathNode current = _selectedNode;
			if (current == null || current == this[0])
			{
				current = this[1];
			}
			MathNode node = CreateNewNode(type, current.Parent);
			//replace the current node
			int n = current.Parent.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (current.Parent[i] == current)
				{
					if (current.Parent.CanReplaceNode(i, node))
					{
						current.Parent[i] = node;
					}
					break;
				}
			}
			//put the current node back as the a child of the new node
			if (node.ChildNodeCount > 0)
			{
				bool b = true;
				if (current is MathNodeNumber)
				{
					if (((MathNodeNumber)current).IsPlaceHolder)
						b = false;
				}
				if (b)
				{
					if (node is MathNodeRoot)
					{
						if (node.CanReplaceNode(1, current))
						{
							node[1] = current;
						}
					}
					else
					{
						if (node.CanReplaceNode(0, current))
						{
							node[0] = current;
						}
					}
				}
			}
			SetFocus(node);
			FireChanged(node);
			return node;
		}
		public void DeleteSelectedNode()
		{
			MathNode current = _selectedNode;
			if (current != null && current != this[0] && current != this && !current.ReadOnly)
			{
				SaveCurrentStateForUndo(); //save the current state
				FireChanged(current);
				if (!current.OnProcessDelete())
				{
					//find a parent with multiple children
					MathNode parent = current.Parent;
					while (parent != null && parent.ChildNodeCount <= 1)
					{
						current = parent;
						parent = parent.Parent;
					}
					if (parent == null || parent == this)
					{
						this[1] = new MathNodeNumber(this);
						((MathNodeNumber)this[1]).IsPlaceHolder = true;
						this.SetFocus(this[1]);
					}
					else
					{
						if (parent != this[0])//just a precausion
						{
							//choose a node to replace the parent
							int n = parent.ChildNodeCount;
							for (int i = 0; i < n; i++)
							{
								if (parent[i] != current)
								{
									//use parent[i]
									if (parent.ReplaceMe(parent[i]))
									{
										break;
									}
								}
							}
						}
					}
				}
				AdjustVariableMap();
			}
		}
		public void Backspace()
		{
			if (!string.IsNullOrEmpty(keybuffer))
			{
				keybuffer = keybuffer.Substring(0, keybuffer.Length - 1);
			}
			MathNodeStringValue sv = _selectedNode as MathNodeStringValue;
			if (sv != null)
			{
				if (sv.Backspace())
				{
					return;
				}
			}
			MathNode nodeToDelete = null;
			MathNode current = _selectedNode;
			while (current != null && current != this)
			{
				if (current != null && current != this[0] && current != this && !current.ReadOnly)
				{
					if (current.ChildNodeCount > 0)
					{
						nodeToDelete = current;
						break;
					}
					else
					{
						current = current.Parent;
					}
				}
				else
				{
					break;
				}
			}
			if (nodeToDelete != null)
			{

				MathNode node = nodeToDelete[0];
				if (node == _selectedNode && nodeToDelete.ChildNodeCount > 1)
				{
					node = nodeToDelete[1];
				}
				if (replaceNode(nodeToDelete, node))
				{
					this.SetFocus(node);
					AdjustVariableMap();
				}
			}
		}
		#endregion

		#region MathNode Override and Abstract Members

		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override bool UseInput
		{
			get
			{
				if (_varMap != null)
				{
					foreach (ICompileableItem item in _varMap.Values)
					{
						IActionInput a = item as IActionInput;
						if (a != null)
						{
							return true;
						}
					}
				}
				return base.UseInput;
			}
		}
		public override void FindItemByType<T>(List<T> results)
		{
			if (_varMap != null)
			{
				foreach (ICompileableItem item in _varMap.Values)
				{
					if (item is T)
					{
						results.Add((T)item);
					}
				}
			}
			base.FindItemByType<T>(results);
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("root:{0}={1}", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		[Browsable(false)]
		protected override List<IVariable> ExtraVariables
		{
			get
			{
				if (varDummy != null)
				{
					List<IVariable> l = new List<IVariable>();
					l.Add(varDummy);
					return l;
				}
				return null;
			}
		}
		public VariableList DummyInputs(bool create)
		{
			VariableList vs = new VariableList();
			if (varDummy == null)
			{
				if (create)
				{
					varDummy = new MathNodeVariableDummy(this);
				}
			}
			if (varDummy != null)
			{
				vs.Add(varDummy);
			}
			return vs;
		}
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
		public RaisDataType ResultType
		{
			get
			{
				return DataType;
			}
			set
			{
				MathNodeVariable v = this[1] as MathNodeVariable;
				if (v != null)
				{
					v.VariableType = value;
				}
			}
		}
		[Browsable(false)]
		public MathNodeVariableDummy Dummy
		{
			get
			{
				if (varDummy == null)
				{
					varDummy = new MathNodeVariableDummy(this);
				}
				return varDummy;
			}
		}
		[Browsable(false)]
		public override RaisDataType DataType
		{
			get
			{
				if (this[1] == null)
				{
					RaisDataType rt = new RaisDataType();
					rt.LibType = typeof(double);
					return rt;
				}
				return this[1].DataType;
			}
		}
		[Browsable(false)]
		public RaisDataType ExpDataType
		{
			get
			{
				if (this[1] == null)
				{
					RaisDataType rt = new RaisDataType();
					rt.LibType = typeof(double);
					return rt;
				}
				return this[1].DataType;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void OnPrepareVariable(IMethodCompile method)
		{
			OnLoaded();
		}
		protected override void OnLoaded()
		{
			IVariable v = this[0] as IVariable;
			if (v == null)
			{
				v = new MathNodeVariable(this);
				this[0] = (MathNode)v;
				v.VariableName = "X";
			}
			v.IsParam = true;
			v.IsReturn = true;
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 0)
			{
				MathNodeVariable n = new MathNodeVariable(this);
				n.VariableName = "X";
				n.IsParam = true;
				n.IsReturn = true;
				return n;
			}
			MathNodeNumber v = new MathNodeNumber(this);
			v.IsPlaceHolder = true;
			return v;
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override bool CanReplaceNode(int childIndex, MathNode newNode)
		{
			if (childIndex == 0)
			{
				return XmlSerialization.CanConvert(newNode.DataType.Type, this.DataType.Type);
			}
			return true;
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			float w = 0;
			float h = 0;
			if (_showAsEquation)
			{
				SizeF size = this[0].CalculateDrawSize(g);
				SizeF sizeE = g.MeasureString("=", TextFont);
				h = size.Height;
				w = size.Width + sizeE.Width;
				if (h < sizeE.Height)
					h = sizeE.Height;
			}
			SizeF size0 = this[1].CalculateDrawSize(g);
			w = w + size0.Width;
			if (h < size0.Height)
				h = size0.Height;
			return new System.Drawing.SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			System.Drawing.Drawing2D.GraphicsState gt0 = g.Save();
			g.TranslateTransform(this.Position.X, this.Position.Y);
			//
			SizeF size = new SizeF();
			SizeF sizeE = new SizeF();
			float w = 0;
			float h = 0;
			if (_showAsEquation)
			{
				size = this[0].CalculateDrawSize(g);
				sizeE = g.MeasureString("=", TextFont);
				h = size.Height;
				w = size.Width + sizeE.Width;
				if (h < sizeE.Height)
					h = sizeE.Height;
			}
			SizeF size0 = this[1].CalculateDrawSize(g);
			w = w + size0.Width;
			if (h < size0.Height)
				h = size0.Height;
			//
			_size = new SizeF(w, h);
			//
			float x = 0;
			float y = 0;
			System.Drawing.Drawing2D.GraphicsState gt;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, 0, 0, (int)w, (int)h);
			}
			if (_showAsEquation)
			{
				if (size.Height < h)
					y = (h - size.Height) / (float)2;
				gt = g.Save();
				g.TranslateTransform(x, y);
				this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
				this[0].Draw(g);
				g.Restore(gt);
				y = 0;
				x = size.Width;
				if (sizeE.Height < h)
					y = (h - sizeE.Height) / (float)2;
				if (IsFocused)
				{
					g.FillRectangle(this.TextBrushBKFocus, x, 0, (int)(sizeE.Width), (int)h);
					g.DrawString("=", this.TextFont, this.TextBrushFocus, new PointF(x, y));
				}
				else
				{
					g.DrawString("=", this.TextFont, this.TextBrush, new PointF(x, y));
				}
				x = x + sizeE.Width;
				y = 0;
			}
			if (size0.Height < h)
				y = (h - size0.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
			g.Restore(gt0);
		}
		public CodeExpression GetMappedCode(IVariable v, IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_varMap == null)
			{
				AdjustVariableMap();
			}
			ICompileableItem item = _varMap.GetItem(v);
			if (item != null)
			{
				return item.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}
		public string GetMappedJavaScriptCode(IVariable v, StringCollection method)
		{
			if (_varMap == null)
			{
				AdjustVariableMap();
			}
			ICompileableItem item = _varMap.GetItem(v);
			if (item != null)
			{
				return item.CreateJavaScript(method);
			}
			return null;
		}
		public string GetMappedPhpScriptCode(IVariable v, StringCollection method)
		{
			if (_varMap == null)
			{
				AdjustVariableMap();
			}
			ICompileableItem item = _varMap.GetItem(v);
			if (item != null)
			{
				return item.CreatePhpScript(method);
			}
			return null;
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("MathNodeRoot ExportCode call {0}", this[1].TraceInfo);
			if (!string.IsNullOrEmpty(this.Description))
			{
				method.MethodCode.Statements.Add(new CodeCommentStatement(this.Description));
			}
			return this[1].ExportCode(method);
		}

		public override bool Update(string newValue)
		{
			MathNode m = GetSelectedNode();
			if (m != null)
			{
				if (m == this)
				{
				}
				else
				{
					if (!m.ReadOnly)
					{
						return m.Update(newValue);
					}
				}
			}
			return false;
		}
		public override void OnKeyUp()
		{
			MathNode m = GetSelectedNode();
			if (m != null)
			{
				if (m != this)
				{
					if (!m.ReadOnly)
					{
						m.OnKeyUp();
					}
				}
			}
		}
		public override void OnKeyDown()
		{
			MathNode m = GetSelectedNode();
			if (m != null)
			{
				if (m != this)
				{
					if (!m.ReadOnly)
					{
						m.OnKeyDown();
					}
				}
			}
		}
		public void CopyAttributesToTarget(MathNodeRoot target)
		{
			target.ShowAsEquation = this.ShowAsEquation;
			target.Font = (Font)Font.Clone();
			target.FontSuperscript = (Font)FontSuperscript.Clone();
			target.SetBrush((Brush)_brush.Clone());
			target.SetBrushFocus((Brush)_brushFocus.Clone());
			target.SetBrushBKFocus((Brush)_brushBKFocus.Clone());
			target.SetBrushBKFocus0((Brush)_brushBKFocus0.Clone());
			target.SetPen((Pen)_pen.Clone());
			target.SetPenFocus((Pen)_penFocus.Clone());
			target.Name = Name;
			target.Description = Description;
			target.Fixed = Fixed;
			target.EitorBounds = _editorBounds;
			target.TransferLocalService(_localServices);
			target.IsVariableHolder = this.IsVariableHolder;
			///assume Action method does not change. Is it true when editing a method?
			///it is true when editing an Action
			target.ActionContext = ActionContext;
			target.ScopeMethod = ScopeMethod;
			target.VariableMapTargetType = VariableMapTargetType;
			target._loader = _loader;
			target._writer = _writer;
			target._xmlNode = _xmlNode;
			target._shortDesc = _shortDesc;
			target._project = _project;
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeRoot node = (MathNodeRoot)base.CloneExp(null);
			CopyAttributesToTarget(node);
			if (varDummy != null)
			{
				node.SetDummyVar((MathNodeVariableDummy)varDummy.CloneExp(node));
			}
			if (_varMap != null)
			{
				node._varMap = _varMap.CloneExp(node);
			}
			//
			node.SetFocus(node.GetFocusedNode());
			return node;
		}
		public override string ToString()
		{
			return this[1].ToString();
		}
		#endregion

		#region IMathExoression Members
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (this.ChildNodeCount > 1)
				{
					return this[1].IsValid;
				}
				return false;
			}
		}
		[Browsable(false)]
		public UInt32 ID
		{
			get
			{
				if (varDummy == null)
				{
					varDummy = new MathNodeVariableDummy(this);
				}
				return varDummy.ID;
			}
		}
		[Browsable(false)]
		public bool IsContainer
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public bool Fixed
		{
			get
			{
				return _fixed;
			}
			set
			{
				_fixed = value;
			}
		}
		[Browsable(false)]
		[Description("Show/hide the variabke map in the editor")]
		public bool ShowVariableMap
		{
			get
			{
				return _showVarMap;
			}
			set
			{
				_showVarMap = value;
			}
		}
		[Browsable(false)]
		public MathExpItem ContainerMathItem
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}
		[Browsable(false)]
		public IMathExpression RootContainer
		{
			get
			{
				if (_container != null)
				{
					if (_container.Parent != null)
						return _container.Parent.RootContainer;
				}
				return this;
			}
		}
		[ReadOnly(true)]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				Site.Name = value;
				_name = value;
			}
		}
		[Browsable(false)]
		public Rectangle EitorBounds
		{
			get
			{
				return _editorBounds;
			}
			set
			{
				_editorBounds = value;
			}
		}
		[Description("Description for this math expression. It will be the comment when generating source code.")]
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
		[Description("This short description can be used as image icon by setting the property IconType to ShortDescription")]
		public string ShortDescription
		{
			get
			{
				return _shortDesc;
			}
			set
			{
				_shortDesc = value;
			}
		}
		[Description("Image representing this group of math expressions")]
		public ObjectIconData IconImage
		{
			get
			{
				if (_iconData == null)
					_iconData = new ObjectIconData();
				return _iconData;
			}
			set
			{
				_iconData = value;
			}
		}
		private bool _useCompileDataType;
		[Browsable(false)]
		public bool UseCompileDataType
		{
			get
			{
				return _useCompileDataType;
			}
			set
			{
				_useCompileDataType = value;
			}
		}
		public void SetDataType(RaisDataType type)
		{
			IVariable v = this[1] as IVariable;
			if (v != null)
			{
				v.VariableType = type;
			}
			v = this[0] as IVariable;
			if (v != null)
			{
				v.VariableType = type;
			}
		}
		public IVariable GetVariableByKeyName(string keyname)
		{
			VariableList inputs = InputVariables;
			foreach (IVariable v in inputs)
			{
				if (v.KeyName == keyname)
				{
					return v;
				}
			}
			return null;
		}
		bool _searching;
		public override IVariable GetVariableByID(UInt32 id)
		{
			if (_searching)
			{
				throw new MathException("MathNodeRoot.GetVariableByID enters into an infinite loop");
			}
			_searching = true;
			IVariable v;
			if (varDummy != null && varDummy.ID == id)
			{
				v = varDummy;
			}
			else
			{
				v = base.GetVariableByID(id);
			}
			_searching = false;
			return v;
		}
		public CodeExpression ReturnCodeExpression(IMethodCompile method)
		{
			MathNode.Trace("MathNodeRoot ReturnCodeExpression generate code for {0}", this[1].TraceInfo);
			this[1].ExportAllCodeStatements(method);
			CodeExpression ce = this[1].ExportCode(method);
			RaisDataType t = this[1].CompileDataType;
			if (t.Type.Equals(typeof(void)))
			{
				_actualCompiledDataType = t;
				return ce;
			}
			if (UseCompileDataType)
			{
				_actualCompiledDataType = this.CompileDataType;
				return RaisDataType.GetConversionCode(t, ce, _actualCompiledDataType, method.MethodCode.Statements);
			}
			else
			{
				_actualCompiledDataType = this.DataType;
				return RaisDataType.GetConversionCode(t, ce, this.DataType, method.MethodCode.Statements);
			}
		}
		public string ReturnJavaScriptCodeExpression(StringCollection methodCode)
		{
			return CreateJavaScript(methodCode);
		}
		public string ReturnPhpScriptCodeExpression(StringCollection methodCode)
		{
			return CreatePhpScript(methodCode);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("MathNodeRoot CreateJavaScript generate code for {0}", this[1].TraceInfo);
			this[1].ExportAllJavaScriptCodeStatements(method);
			string ce = this[1].CreateJavaScript(method);
			RaisDataType t = this[1].CompileDataType;
			if (t.Type.Equals(typeof(void)))
			{
				_actualCompiledDataType = t;
				return ce;
			}
			if (UseCompileDataType)
			{
				_actualCompiledDataType = this.CompileDataType;
				return ce;
			}
			else
			{
				_actualCompiledDataType = this.DataType;
				return ce;
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("MathNodeRoot CreatePhpScript generate code for {0}", this[1].TraceInfo);
			this[1].ExportAllPhpScriptCodeStatements(method);
			string ce = this[1].CreatePhpScript(method);
			RaisDataType t = this[1].CompileDataType;
			if (t.Type.Equals(typeof(void)))
			{
				_actualCompiledDataType = t;
				return ce;
			}
			if (UseCompileDataType)
			{
				_actualCompiledDataType = this.CompileDataType;
				return ce;
			}
			else
			{
				_actualCompiledDataType = this.DataType;
				return ce;
			}
		}
		[Browsable(false)]
		public RaisDataType ActualCompileDataType
		{
			get
			{
				return _actualCompiledDataType;
			}
		}
		/// <summary>
		/// find all input variables including internal ones
		/// </summary>
		/// <returns></returns>
		public VariableList FindAllInputVariables()
		{
			VariableList varList = new VariableList();
			this[1].GetAllVariables(varList);
			if (varList.Count == 0)
			{
				//add a dummy variable for generating an inport by the viewer.
				//a dummy inport is for the situations when inputs are not needed but
				//execution order has to be kept.
				//Suppose MathExpGroup A assigns result to a property/field,
				//then MathExpGroup B is executed. If MathExpGroup B does not
				//have inputs then it needs a dummy inport so that MathExpGroup A 
				//can link to it
				if (varDummy == null)
				{
					varDummy = new MathNodeVariableDummy(this);
				}
				varList.Add(varDummy);
			}
			return varList;
		}
		[Browsable(false)]
		public IVariable OutputVariable
		{
			get
			{
				return (IVariable)this[0];
			}
		}
		/// <summary>
		/// unique input variables excluding internal onces, for creating inports for outside linking.
		/// the elements used in the math expression determine their own variables' scopes.
		/// MathNodeRoot does not have logic to determine the scope of the variables.
		/// </summary>
		[Browsable(false)]
		public VariableList InputVariables
		{
			get
			{
				if (_inputs == null)
				{
					GenerateInputVariables();
				}
				return _inputs;
			}
		}
		[Browsable(false)]
		public VariableList InportVariables
		{
			get
			{
				if (_inputs == null)
				{
					GenerateInputVariables();
				}
				return _inputs;
			}
		}
		public void SetDummyVar(MathNodeVariableDummy v)
		{
			varDummy = v;
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			Bitmap img = null;
			string text;
			SizeF size;
			Graphics gImg;
			ObjectIconData icon = this.IconImage;
			switch (icon.IconType)
			{
				case EnumIconType.IconImage:
					img = (Bitmap)icon.IconImage;
					if (img == null)
						goto LB_ShortText;
					break;
				case EnumIconType.ItemImage:
					size = CalculateDrawSize(g);
					img = new Bitmap((int)size.Width, (int)size.Height, g);
					gImg = Graphics.FromImage(img);
					Point p = Offset;
					Offset = new Point(0, 0);
					Draw(gImg);
					gImg.Dispose();
					Offset = p;
					break;
				case EnumIconType.ShortDescription:
				LB_ShortText:
					text = this.ShortDescription;
					if (string.IsNullOrEmpty(text))
					{
						text = this.Name;
					}
					goto LB_NameText;
				default://including case EnumIconType.NameText:
					text = this.Name;
				LB_NameText:
					size = g.MeasureString(text, icon.TextAttributes.Font);
					if (size.Width == 0)
						size.Width = 20;
					if (size.Height == 0)
						size.Height = 20;
					img = new Bitmap((int)size.Width, (int)size.Height, g);
					gImg = Graphics.FromImage(img);
					gImg.FillRectangle(new SolidBrush(icon.TextAttributes.BackColor), 0, 0, size.Width, size.Height);
					gImg.DrawString(text, icon.TextAttributes.Font, new SolidBrush(icon.TextAttributes.TextColor), (float)0, (float)0);
					gImg.Dispose();
					break;
			}
			return img;
		}
		public IMathEditor CreateEditor(Rectangle rcStart)
		{
			using (ShowWaitMessage frm = new ShowWaitMessage(null, "Loading Expression Editor, please wait..."))
			{
				dlgMathEditor dlg = new dlgMathEditor(rcStart);
				dlg.MathExpression = (MathNodeRoot)this.Clone();
				if (_editorBounds.Width > 0 && _editorBounds.Height > 0)
				{
					dlg.Location = new Point(_editorBounds.X, _editorBounds.Y);
					dlg.Size = new Size(_editorBounds.Width, _editorBounds.Height);
				}
				return dlg;
			}
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.SetName(node, this.Name);
			XmlSerialization.SetAttribute(node, XMLATT_FIXED, _fixed);
			XmlSerialization.WriteStringValueToChildNode(node, XmlSerialization.XML_DESCRIPT, this.Description);
			XmlSerialization.WriteStringValueToChildNode(node, XmlSerialization.XML_SHORTDESCRIPT, this.ShortDescription);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_EDITORRECT, _editorBounds);
			XmlNode nd = node.SelectSingleNode("IconImage");
			if (nd == null)
			{
				nd = node.OwnerDocument.CreateElement("IconImage");
				node.AppendChild(nd);
			}
			else
			{
				nd.RemoveAll();
			}
			this.IconImage.SaveToXmlNode(nd);
			//
			XmlSerialization.WriteValueToChildNode(node, "Position", Position);
			XmlSerialization.WriteValueToChildNode(node, "Font", _font);
			XmlSerialization.WriteValueToChildNode(node, "SuperscriptFont", _fontSuperscript);
			if (varDummy != null)
			{
				nd = node.SelectSingleNode("Inport");
				if (nd == null)
				{
					nd = node.OwnerDocument.CreateElement("Inport");
					node.AppendChild(nd);
				}
				else
				{
					nd.RemoveAll();
				}
				varDummy.Save(nd);
			}
		}
		protected override void OnLoad(XmlNode node)
		{
			_inputs = null;
			_varMap = null;
			Name = XmlSerialization.GetName(node);
			_fixed = XmlSerialization.GetAttributeBool(node, XMLATT_FIXED);
			this.Description = XmlSerialization.ReadStringValueFromChildNode(node, XmlSerialization.XML_DESCRIPT);
			this.ShortDescription = XmlSerialization.ReadStringValueFromChildNode(node, XmlSerialization.XML_SHORTDESCRIPT);
			_editorBounds = XmlSerialization.GetAttributeRect(node, XmlSerialization.XMLATT_EDITORRECT);
			XmlNode nd = node.SelectSingleNode("IconImage");
			if (nd != null)
			{
				IconImage.LoadFromXmlNode(nd);
			}
			nd = node.SelectSingleNode("Inport");
			if (nd != null)
			{
				varDummy = new MathNodeVariableDummy(this);
				varDummy.Load(nd);
			}
			object v;
			nd = node.SelectSingleNode("Position");
			if (nd != null)
			{
				if (XmlSerialization.ReadValue(nd, out v))
				{
					if (v != null)
					{
						Position = (Point)v;
					}
				}
			}
			nd = node.SelectSingleNode("Font");
			if (nd != null)
			{
				if (XmlSerialization.ReadValue(nd, out v))
				{
					if (v != null)
					{
						_font = (Font)v;
					}
				}
			}
			nd = node.SelectSingleNode("SuperscriptFont");
			if (nd != null)
			{
				if (XmlSerialization.ReadValue(nd, out v))
				{
					if (v != null)
					{
						_fontSuperscript = (Font)v;
					}
				}
			}
		}
		public void ReplaceVariableID(UInt32 currentID, UInt32 newID)
		{
			if (((IVariable)this[0]).ID == currentID)
			{
				((IVariable)this[0]).ResetID(newID);
			}
			else
			{
				VariableList vs = this.FindAllInputVariables();
				for (int i = 0; i < vs.Count; i++)
				{
					if (vs[i].ID == currentID)
					{
						vs[i].ResetID(newID);
						break;
					}
				}
			}
		}
		public void PrepareDrawInDiagram()
		{
			this.ShowAsEquation = false;
		}
		public void SetServiceProvider(IDesignServiceProvider designServiceProvider)
		{
			MathNode.RegisterGetGlobalServiceProvider(this, designServiceProvider);
		}
		public void AddItem(IMathExpression item)
		{
		}
		/// <summary>
		/// get all variables and assign their Scopes
		/// </summary>
		public void GenerateInputVariables()
		{
			_inputs = new VariableList();
			Dictionary<string, VariableList> variablesByName = new Dictionary<string, VariableList>();
			VariableList vs = this.FindAllInputVariables();
			//find out the inputs which act as inports
			for (int i = 0; i < vs.Count; i++)
			{
				//exclude local variables and parameters
				if (!vs[i].IsLocal && !vs[i].IsParam)
				{
					//it is an input variable
					//group variables by KeyName
					if (!variablesByName.ContainsKey(vs[i].KeyName))
					{
						variablesByName.Add(vs[i].KeyName, new VariableList());
					}
					variablesByName[vs[i].KeyName].Add(vs[i]);
					if (vs[i].Scope == this)
					{
						vs[i].Scope = null; //scope will be set by the MathExpGroup which claims the variable local
					}
				}
				else
				{
					vs[i].Scope = this;
					if (!vs[i].IsLocal)
					{
						if (!variablesByName.ContainsKey("?" + vs[i].ID.ToString()))
						{
							variablesByName.Add("?" + vs[i].ID.ToString(), new VariableList());
						}
						variablesByName["?" + vs[i].ID.ToString()].Add(vs[i]);
					}
				}
			}
			//find/create port variables
			foreach (KeyValuePair<string, VariableList> de in variablesByName)
			{
				UInt32 portID = 0;
				//find one with LinkedPortID
				foreach (IVariable v in de.Value)
				{
					if (v.InPort != null)
					{
						if (v.InPort.LinkedPortID != 0)
						{
							_inputs.Add(v);
							v.IsInPort = true;
							portID = v.ID;
							break;
						}
					}
				}
				if (portID == 0)
				{
					//find one with IsInPort
					foreach (IVariable v in de.Value)
					{
						if (v.IsInPort)
						{
							_inputs.Add(v);
							portID = v.ID;
							break;
						}
					}
				}
				if (portID == 0)
				{
					IVariable v = de.Value[0];
					v.IsInPort = true;
					portID = v.ID;
					_inputs.Add(v);
				}
				foreach (IVariable v in de.Value)
				{
					if (v.ID != portID)
					{
						v.IsInPort = false;
					}
				}
			}
			if (_inputs.Count == 0)
			{
				if (varDummy == null)
				{
					varDummy = new MathNodeVariableDummy(this);
				}
				_inputs.Add(varDummy);
			}
		}
		private MethodInfo _methodInfo;
		private List<IPropertyPointer> _pointers;
		/// <summary>
		/// when this object is used as an action parameter value, a static method is created
		/// using ReturnCodeExpression as if creating a test method. This function returns the
		/// method created.
		/// </summary>
		/// <param name="propertyOwner">object for accessing the properties</param>
		/// <param name="pointers">property pointers. every one corresponds to a method parameter</param>
		/// <returns></returns>
		public MethodInfo GetCalculationMethod(object propertyOwner, List<IPropertyPointer> pointers)
		{
			if (_methodInfo == null)
			{
				try
				{
					MathNode.Trace("MathNodeRoot debug Code-generation starts at {0} ===Expression============", DateTime.Now);
					{
						string SNname = "LimnorMathExpression";
						string className = "Test";
						string methodName = "TestMathExpression";
						GenerateInputVariables();
						//
						MethodType mt = new MethodType();
						MathNode.PrepareMethodCreation();
						PrepareForCompile(mt);
						CodeCompileUnit code = new CodeCompileUnit();
						CodeNamespace ns = new CodeNamespace(SNname);
						code.Namespaces.Add(ns);
						//
						AssemblyRefList imports = new AssemblyRefList();
						GetAllImports(imports);
						MathNode.Trace("Imports:{0}", imports.Count);
						MathNode.IndentIncrement();
						foreach (AssemblyRef sA in imports)
						{
							MathNode.Trace(sA.Name);
							ns.Imports.Add(new CodeNamespaceImport(sA.Name));
						}
						MathNode.IndentDecrement();
						//type declaration
						CodeTypeDeclaration t = new CodeTypeDeclaration(className);
						ns.Types.Add(t);
						//test method 
						CodeMemberMethod m = new CodeMemberMethod();
						mt.MethodCode = m;
						t.Members.Add(m);
						m.ReturnType = new CodeTypeReference(DataType.Type);
						m.Name = methodName;
						m.Attributes = MemberAttributes.Static | MemberAttributes.Public;
						//
						m.Comments.Add(new CodeCommentStatement("Variable mapping:"));
						m.Comments.Add(new CodeCommentStatement("In formula: method parameter"));
						//
						mt.MethodCode = m;
						//
						VariableList variables = InputVariables;
						//
						MathNode.Trace("Generate statements");
						ExportAllCodeStatements(mt);
						MathNode.Trace("\tstatements.Count={0}", m.Statements.Count);
						int n = variables.Count;
						MathNode.Trace("Generate local variables from {0} variables", n);
						MathNode.IndentIncrement();
						for (int k = 0; k < n; k++)
						{
							IVariable var = variables[k];
							MathNode.Trace(k, var);
							if (!(var is MathNodeVariableDummy) && !var.IsParam && !var.IsConst)
							{
								string paramName = "";
								paramName = var.CodeVariableName;
								CodeVariableDeclarationStatement p;
								if (var.VariableType.Type.IsValueType)
								{
									p = new CodeVariableDeclarationStatement(
										var.VariableType.Type, paramName);
								}
								else
								{
									p = new CodeVariableDeclarationStatement(
										var.VariableType.Type, paramName, ValueTypeUtil.GetDefaultCodeByType(var.VariableType.Type));
								}
								m.Statements.Insert(0, p);
								//add comment
								string sub = var.SubscriptName;
								if (string.IsNullOrEmpty(sub))
									sub = " ";
								m.Comments.Add(new CodeCommentStatement(string.Format("{0}{1}:\t {2}", var.VariableName, sub, var.CodeVariableName)));
								MathNode.IndentIncrement();
								MathNode.Trace("Argument {0} {1} for {2}, {3}", var.VariableType.Type, paramName, var.TraceInfo, var.GetType());
								MathNode.IndentDecrement();
								AssignCodeExp(new CodeVariableReferenceExpression(paramName), var.CodeVariableName);
							}
						}
						//create parameters for pointers
						Dictionary<string, IPropertyPointer> pointerList = new Dictionary<string, IPropertyPointer>();
						GetPointers(pointerList);
						_pointers = new List<IPropertyPointer>();
						MathNode.Trace("Generate arguments from {0} pointers", pointers.Count);
						foreach (KeyValuePair<string, IPropertyPointer> kv in pointerList)
						{
							string paramName = "";
							_pointers.Add(kv.Value);
							paramName = kv.Value.CodeName;
							CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(new CodeTypeReference(kv.Value.ObjectType), paramName);
							m.Parameters.Add(p);
							//add comment
							m.Comments.Add(new CodeCommentStatement(string.Format("{0}:\t {1}", kv.Value.ToString(), paramName)));
							MathNode.IndentIncrement();
							MathNode.Trace("Argument {0} {1} for {2}", kv.Value.ObjectType, paramName, kv.Value.ToString());
							MathNode.IndentDecrement();
						}
						MathNode.IndentDecrement();
						MathNode.Trace("Generate code");
						CodeExpression exp = ExportCode(mt);
						if (!DataType.Type.Equals(ExpDataType.Type))
						{
							if (ExpDataType.Type.Equals(typeof(void)))
							{
								MathNode.Trace("Function returns {0}. Test method returns {1}", ExpDataType.Type, DataType.Type);
								m.Statements.Add(new CodeExpressionStatement(exp));
								exp = ValueTypeUtil.GetDefaultCodeByType(DataType.Type);
							}
							else
							{
								MathNode.Trace("Convert {0} to type {1}", ExpDataType.Type, DataType.Type);
								exp = VPLUtil.ConvertByType(DataType.Type, exp);
							}
						}
						CodeMethodReturnStatement mr = new CodeMethodReturnStatement(exp);
						m.Statements.Add(mr);
						MathNode.Trace("MathNodeRoot debug Code-generation ends at {0} ===============", DateTime.Now);
						//
						//compile it
						CSharpCodeProvider provider = new CSharpCodeProvider();
						CompilerParameters cp = new CompilerParameters(new string[] { 
                            "System.dll" });
						cp.GenerateInMemory = true;
						cp.OutputAssembly = "AutoGenerated";
						//
						CompilerResults results = provider.CompileAssemblyFromDom(cp, code);
						if (results.Errors.HasErrors)
						{
#if DEBUG
							//
							//debug
							StringWriter sw;
							sw = new StringWriter();
							CodeGeneratorOptions o = new CodeGeneratorOptions();
							o.BlankLinesBetweenMembers = false;
							o.BracingStyle = "C";
							o.ElseOnClosing = false;
							o.IndentString = "    ";
							provider.GenerateCodeFromCompileUnit(code, sw, o);
							sw.Close();
							//
							MathNode.Trace("Cannot form calculation method for expression.");
							MathNode.IndentIncrement();
							for (int i = 0; i < results.Errors.Count; i++)
							{
								MathNode.Trace(results.Errors[i].ToString());
							}
							MathNode.IndentDecrement();
#endif
						}
						else
						{
							Type[] types = results.CompiledAssembly.GetExportedTypes();
							if (types != null)
							{
								for (int i = 0; i < types.Length; i++)
								{
									if (types[i].Name == className)
									{
										_methodInfo = types[i].GetMethod(methodName);
										if (_methodInfo != null)
										{
											break;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception err)
				{
					MathNode.Log(getForm(), err);
				}
			}
			if (_pointers != null && _pointers.Count > 0)
			{
				foreach (IPropertyPointer pp in _pointers)
				{
					pointers.Add(pp);
				}
			}
			return _methodInfo;
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			List<ISourceValuePointer> l = new List<ISourceValuePointer>();
			List<MathNode> nodes = new List<MathNode>();
			this[1].GetWebNodes(nodes, EnumWebRunAt.Server, true);
			List<ISourceValuePointer> ls = new List<ISourceValuePointer>();
			foreach (MathNode nd in nodes)
			{
				nd.GetValueSources(ls);
			}
			foreach (ISourceValuePointer v in ls)
			{
				if (v.IsWebClientValue())
				{
					if (!v.IsWebServerValue())
					{
						l.Add(v);
					}
				}
			}
			return l;
		}
		public IList<ISourceValuePointer> GetValueSources()
		{
			List<ISourceValuePointer> list = new List<ISourceValuePointer>();
			this[1].GetValueSources(list);
			if (_varMap != null)
			{
				foreach (KeyValuePair<IVariable, ICompileableItem> kv in _varMap)
				{
					kv.Value.GetValueSources(list);
				}
			}
			return list;
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		public ISite Site
		{
			get
			{
				if (_site == null)
					_site = new MathSite(this);
				if (!string.IsNullOrEmpty(_name))
				{
					_site.Name = _name;
				}
				return _site;
			}
			set
			{
				_site = value;
				if (!string.IsNullOrEmpty(_name))
				{
					_site.Name = _name;
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
				Disposed(this, new EventArgs());
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			MathNodeRoot obj = (MathNodeRoot)CloneExp(null);
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(this);
			if (sp != null)
			{
				MathNode.RegisterGetGlobalServiceProvider(obj, sp);
			}

			return obj;
		}

		#endregion

		#region ICustomSerialization Members
		const string XML_Item = "Item";
		IXmlCodeReader _loader;
		IXmlCodeWriter _writer;
		[Browsable(false)]
		public IXmlCodeReader Reader
		{
			get
			{
				if (_loader == null)
					return XmlSerializerUtility.ActiveReader;
				return _loader;
			}
		}
		[Browsable(false)]
		public IXmlCodeWriter Writer
		{
			get
			{
				if (_writer == null)
				{
					_writer = XmlSerializerUtility.GetWriter(_loader);
				}
				return _writer;
			}
		}
		[Browsable(false)]
		public XmlNode CachedXmlNode
		{
			get
			{
				return _xmlNode;
			}
		}
		/// <summary>
		/// call XmlSerialization.ReadFromXmlNode(node) to create this object
		/// and then call this function to load contents
		/// </summary>
		/// <param name="node"></param>
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_loader = serializer;
			_xmlNode = node;
			Load(node);
			_varMap = new VariableMap();
			XmlNodeList items = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}", XmlSerializerUtility.XML_VarMap, XML_Item));
			if (items != null && items.Count > 0)
			{
				if (this.ActionContext == null)
				{
					if (_loader != null)
					{
						if (_loader.ObjectStack != null)
						{
							IActionContext ia = null;
							IEnumerator ie = _loader.ObjectStack.GetEnumerator();
							while (ie.MoveNext())
							{
								ia = ie.Current as IActionContext;
								if (ia != null)
								{
									break;
								}
							}
							if (ia != null)
							{
								this.ActionContext = ia;
							}
						}
					}
				}
				foreach (XmlNode itemNode in items)
				{
					object mapped = Activator.CreateInstance(VariableMapTargetType, ActionContext);
					IXmlSerializeItem si = (IXmlSerializeItem)mapped;
					si.SetMapOwner(this);
					si.ItemSerialize(_loader, itemNode, false);
					ICompileableItem c = (ICompileableItem)mapped;
					IVariable v = GetVariableByKeyName(c.Name);
					if (v != null)
					{
						c.SetDataType(v.VariableType.LibType);
						_varMap.Add(v, c);
					}
				}
			}
			AdjustVariableMap();
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_writer = serializer;
			_xmlNode = node;
			OnSave(node);
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				XmlNode nd = node.OwnerDocument.CreateElement(XmlSerialization.XML_Math);
				node.AppendChild(nd);
				this[i].Save(nd); //recursive saving
			}
			AdjustVariableMap();
			if (_varMap != null)
			{
				XmlNode mapNode = XmlUtil.CreateSingleNewElement(node, XmlSerializerUtility.XML_VarMap);
				mapNode.RemoveAll();
				foreach (KeyValuePair<IVariable, ICompileableItem> kv in _varMap)
				{
					XmlNode itemNode = mapNode.OwnerDocument.CreateElement(XML_Item);
					mapNode.AppendChild(itemNode);
					kv.Value.Name = kv.Key.KeyName; //mapping to the key
					IXmlSerializeItem si = (IXmlSerializeItem)kv.Value;
					si.ItemSerialize(_writer, itemNode, true);
				}
			}
			else
			{
				XmlUtil.RemoveChildNode(node, XmlSerializerUtility.XML_VarMap);
			}
		}
		/// <summary>
		/// this function always search downwards. thus it returns null
		/// </summary>
		/// <param name="portId"></param>
		/// <returns></returns>
		public MathExpItem GetItemByID(UInt32 portId)
		{
			return null;
		}
		public MathExpItem RemoveItemByID(UInt32 portId)
		{
			return null;
		}
		#endregion

		#region IUndoable<MathNodeRoot> Members
		public event EventHandler OnUndoStateChanged;
		private MathUndoEngine<MathNodeRoot> undoEngin;
		public void ResetUndo()
		{
			if (undoEngin != null)
				undoEngin.ResetUndo();
		}
		[Browsable(false)]
		public bool HasUndo
		{
			get
			{
				if (undoEngin != null)
					return undoEngin.HasUndo;
				return false;
			}
		}
		[Browsable(false)]
		public bool HasRedo
		{
			get
			{
				if (undoEngin != null)
					return undoEngin.HasRedo;
				return false;
			}
		}
		public void Undo()
		{
			if (EnableUndo)
			{
				if (undoEngin != null)
					undoEngin.Undo(this);
			}
		}
		public void Redo()
		{
			if (EnableUndo)
			{
				if (undoEngin != null)
					undoEngin.Redo(this);
			}
		}
		public void SaveCurrentStateForUndo()
		{
			if (EnableUndo)
			{
				if (undoEngin == null)
					undoEngin = new MathUndoEngine<MathNodeRoot>(this);
				undoEngin.SaveCurrentState(this);
			}
		}
		public void Apply(MathNodeRoot node)
		{
			if (node != null)
			{
				this[0] = node[0];
				this[1] = node[1];
				this.SetFocus(node.FocusedNode);
			}
		}

		public void FireStateChange()
		{
			if (OnUndoStateChanged != null)
			{
				OnUndoStateChanged(this, null);
			}
		}

		#endregion

		#region local services
		/// <summary>
		/// for clone
		/// </summary>
		/// <param name="services"></param>
		public void TransferLocalService(Dictionary<Type, object> services)
		{
			_localServices = services;
		}
		public override object GetLocalService(Type serviceType)
		{
			if (_localServices != null && _localServices.ContainsKey(serviceType))
			{
				return _localServices[serviceType];
			}
			return null;
		}
		public override void AddLocalService(Type serviceType, object service)
		{
			if (_localServices == null)
				_localServices = new Dictionary<Type, object>();
			if (_localServices.ContainsKey(serviceType))
				_localServices[serviceType] = service;
			else
				_localServices.Add(serviceType, service);
		}
		#endregion

		#region IMethod Members
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MathNodeRoot r = (MathNodeRoot)this.Clone();
			r.ActionContext = action;
			return r;
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public object ModuleProject
		{
			get
			{
				if (_project != null)
				{
					return _project;
				}
				if (_methodScope != null && _methodScope.ModuleProject != null)
				{
					return _methodScope.ModuleProject;
				}
				return null;
			}
			set
			{
				_project = value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public bool NoReturn { get { return true; } }
		[Browsable(false)]
		public bool HasReturn { get { return false; } }
		[Browsable(false)]
		public IObjectIdentity ReturnPointer { get { return null; } set { } }
		public object GetParameterType(UInt32 id)
		{
			VariableList vs = InputVariables;
			if (vs != null)
			{
				IVariable v = vs.GetVariableById(id);
				if (v != null)
				{
					return v.ParameterLibType;
				}
			}
			return null;
		}
		public Dictionary<string, string> GetParameterDescriptions() { return null; }
		public string ParameterName(int i)
		{
			VariableList vs = InputVariables;
			if (vs != null)
			{
				if (i >= 0 && i < vs.Count)
				{
					return vs[i].Name;
				}
			}
			return null;
		}
		public void SetParameterValueChangeEvent(EventHandler h)
		{
			_valueChangedHandler = h;
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get
			{
				return this[1].IsLocal;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				VariableList vs = InputVariables;
				if (vs != null)
					return vs.Count;
				return 0;
			}
		}
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
		public string DefaultActionName
		{
			get
			{
				return this.Name;
			}
		}
		[Browsable(false)]
		public IParameter MethodReturnType
		{
			get
			{
				IVariable v = this[0] as IVariable;
				//if (v != null)
				{
					return v;
				}

			}
			set
			{
				MathNodeVariable v = this[0] as MathNodeVariable;
				if (v != null)
				{
					//v.VariableType = value;
				}
			}
		}
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				VariableList vl = this.InputVariables;
				int n = vl.Count;
				List<IParameter> ps = new List<IParameter>();
				for (int i = 0; i < n; i++)
				{
					ps.Add(vl[i]);
				}
				return ps;
			}
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "mr_{0}", this.ID.ToString("x", CultureInfo.InvariantCulture)); }
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get { return ObjectKey; }
		}
		public bool IsSameMethod(ProgElements.IMethod method)
		{
			MathNodeRoot r = method as MathNodeRoot;
			if (r != null)
			{
				return (r.ID == this.ID);
			}
			return false;
		}
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MathNodeRoot r = objectIdentity as MathNodeRoot;
			if (r != null)
			{
				return (r.ID == this.ID);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				int n = ParameterCount;
				if (n == 0)
				{
					return true;
				}
				return false;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Custom; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Unknown; }
		}

		#endregion

		#region private methods
		private void mapItemChanged(object sender, EventArgs e)
		{
			if (OnChanged != null)
			{
				OnChanged(sender, e);
			}
		}
		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			IObjectIdentity o = pointer as IObjectIdentity;
			if (o != null)
			{
				return IsSameObjectRef(o);
			}
			return false;
		}

		#endregion

		#region IXmlCodeReaderWriterHolder Members
		public new IXmlCodeWriter GetWriter()
		{
			if (_writer == null)
			{
				_writer = XmlSerializerUtility.GetWriter(_loader);
			}
			return _writer;
		}

		public new IXmlCodeReader GetReader()
		{
			if (_loader == null)
				return XmlSerializerUtility.ActiveReader;
			return _loader;
		}

		public void SetWriter(IXmlCodeWriter writer)
		{
			if (writer != null)
			{
				_writer = writer;
			}
		}
		public void SetReader(IXmlCodeReader reader)
		{
			if (reader != null)
			{
				_loader = reader;
			}
		}
		#endregion
	}
}
