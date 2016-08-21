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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.CodeDom;
using System.Collections.Specialized;
using MathExp.RaisTypes;
using ProgElements;
using MathExp.Properties;
using WindowsUtility;

namespace MathExp
{
	/// <summary>
	/// viewer for the math expression editor
	/// </summary>
	public partial class MathExpCtrl : UserControl
	{
		public event EventHandler OnSetFocus;
		public event EventHandler OnUndoStateChanged;
		public event EventHandler OnFinish;
		public event EventHandler OnCancel;
		public event EventHandler OnCreateCompound = null;
		private MathNodeRoot mathExp;
		private MathExpEditor _editor;
		private bool _readOnly;
		private Pen pen;
		private Hashtable services;
		private Label _lblForScroll;
		public MathExpCtrl()
		{
			pen = new Pen(Color.Black, 1);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			InitializeComponent();
			_lblForScroll = new Label();
			this.Controls.Add(_lblForScroll);
			_lblForScroll.BorderStyle = BorderStyle.None;
			_lblForScroll.Text = "";
			_lblForScroll.Size = new Size(1, 1);
			_lblForScroll.Location = new Point(0, 0);
			mathExp = new MathNodeRoot();
			mathExp.Position = new Point(8, 8);
			initMathExp();
			//
		}
		private void initMathExp()
		{
			mathExp.Viewer = this;
			mathExp.OnSetFocus += new EventHandler(mathExp_OnSetFocus);
			mathExp.OnUndoStateChanged += new EventHandler(mathExp_OnUndoStateChanged);
			mathExp.OnChanged += new EventHandler(mathExp_OnChanged);
			mathExp.ValueChangedHandler = mathExp_OnChanged;
			if (_editor != null)
			{
				_editor.ShowVariableMap(mathExp.VariableMap);
			}
		}
		private void hideSelectorList()
		{
			List<SelectorListBox> list = new List<SelectorListBox>();
			for (int i = 0; i < Controls.Count; i++)
			{
				SelectorListBox s = Controls[i] as SelectorListBox;
				if (s != null)
				{
					list.Add(s);
				}
			}
			if (list.Count > 0)
			{
				foreach (SelectorListBox s in list)
				{
					Control p = s.Parent;
					if (p != null)
					{
						p.Controls.Remove(s);
					}
				}
			}
		}
		private void mathExp_OnChanged(object sender, EventArgs e)
		{
			_changed = true;
			this.Refresh();
		}
		private bool _changed;
		[Browsable(false)]
		public bool Changed
		{
			get
			{
				return _changed;
			}
			set
			{
				_changed = value;
			}
		}
		private IMethod _sm;
		[Browsable(false)]
		public IMethod ScopeMethod { get { return _sm; } set { _sm = value; } }
		public RaisDataType DataType
		{
			get
			{
				return mathExp.DataType;
			}
		}
		public RaisDataType ExpDataType
		{
			get
			{
				return mathExp.ExpDataType;
			}
		}
		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			this.Refresh();
		}
		public void DoDoubleClick()
		{
			OnDoubleClick(EventArgs.Empty);
		}
		public void SetMathEditor(MathExpEditor editor)
		{
			_editor = editor;
		}
		public void PrepareForCompile(MethodType method)
		{
			mathExp.PrepareForCompile(method);
		}
		public void AddService(Type t, object service)
		{
			if (services == null)
			{
				services = new Hashtable();
			}
			if (!services.ContainsKey(t))
			{
				services.Add(t, service);
			}
			else
			{
				services[t] = service;
			}
		}
		public void AssignCodeExp(CodeExpression code, string codeVarName)
		{
			mathExp[1].AssignCodeExp(code, codeVarName);
		}
		/// <summary>
		/// use a different name to avoid hiding the base version
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public object GetService2(Type t)
		{
			if (services != null)
			{
				if (services.ContainsKey(t))
					return services[t];
			}
			return null;
		}
		public void GenerateInputVariables()
		{
			mathExp.GenerateInputVariables();
		}
		public void ExportAllCodeStatements(MethodType method)
		{
			mathExp.ExportAllCodeStatements(method);
		}
		public void GetAllImports(AssemblyRefList imports)
		{
			mathExp.GetAllImports(imports);
		}
		public CodeExpression ExportCode(MethodType method)
		{
			return mathExp.ExportCode(method);
		}
		public void GetPointers(Dictionary<string, IPropertyPointer> pointers)
		{
			mathExp.GetPointers(pointers);
		}
		public VariableList InputVariables
		{
			get
			{
				return mathExp.InputVariables;
			}
		}
		public MathNodeRoot Root
		{
			get
			{
				return mathExp;
			}
			set
			{
				mathExp = value;
				initMathExp();
				Refresh();
				_changed = false;
			}
		}
		public Point Offset
		{
			get
			{
				return mathExp.Position;
			}
			set
			{
				mathExp.Position = value;
				this.Refresh();
				_changed = true;
			}
		}
		void mathExp_OnUndoStateChanged(object sender, EventArgs e)
		{
			if (OnUndoStateChanged != null)
			{
				OnUndoStateChanged(sender, e);
			}
		}

		void mathExp_OnSetFocus(object sender, EventArgs e)
		{
			this.Refresh();
			if (OnSetFocus != null)
			{
				OnSetFocus(sender, e);
			}
		}
		void mnu_separate(object sender, EventArgs e)
		{
			BinOperatorNode bin = Root.FocusedNode as BinOperatorNode;
			if (bin != null)
			{
				BinOperatorNode bin2 = bin[1] as BinOperatorNode;
				if (bin2 != null)
				{
					//modify it from {0} b {b2} to 
					//{0} b {b2[0]} b2 {b2[1]}
					//
					//new b[1] = {b2[0]}
					//new b2[0] = new b => {0} b {b2[0]}
					//new b2[1] = {b2[1]}
					//MathNode b1 = bin[1];
					//
					MathNode np = bin.Parent;
					int k = -1;
					for (int i = 0; i < np.ChildNodeCount; i++)
					{
						if (np[i] == bin)
						{
							k = i;
							break;
						}
					}
					if (k >= 0)
					{
						BinOperatorNode nb2 = (BinOperatorNode)Activator.CreateInstance(bin2.GetType(), bin.Parent);
						bin[1] = bin2[0];
						nb2[0] = bin;
						nb2[1] = bin2[1];
						//
						np[k] = nb2;
						this.Refresh();
						Root.SetFocus(bin);
					}
				}
			}
		}
		[Browsable(false)]
		public MathNode FocusedNode
		{
			get
			{
				return mathExp.FocusedNode;
			}
		}
		private Type variableMapTargetType
		{
			get
			{
				IMathEditor ime = this.FindForm() as IMathEditor;
				if (ime != null)
				{
					return ime.VariableMapTargetType;
				}
				return null;
			}
		}
		public void SetNodeSelection(MathNode node)
		{
			Root.SetFocus(node);
			_highlighted = node;
		}
		public void RefreshVariableMap()
		{
			mathExp.AdjustVariableMap();
			if (_editor != null)
			{
				_editor.ShowVariableMap(mathExp.VariableMap);
			}
		}
		public MathNode AddMathNode(Type type)
		{
			MathNode node = mathExp.AddMathNode(type);
			mathExp.AddVariableMapItem(node);
			if (_editor != null)
			{
				_editor.ShowVariableMap(mathExp.VariableMap);
			}
			this.Refresh();
			_changed = true;
			return node;
		}
		public MathNode ReplaceMathNode(MathNode curNode, Type type)
		{
			MathNode node = mathExp.ReplaceMathNode(curNode, type);
			if (node != null)
			{
				mathExp.AddVariableMapItem(node);
				if (_editor != null)
				{
					_editor.RefreshPropertyGrid();
					_editor.ShowVariableMap(mathExp.VariableMap);
				}
				this.Refresh();
				_changed = true;
			}
			return node;
		}
		public void Undo()
		{
			mathExp.Undo();
			this.Refresh();
		}
		public void Redo()
		{
			mathExp.Redo();
			this.Refresh();
		}
		public bool HasUndo
		{
			get
			{
				return mathExp.HasUndo;
			}
		}
		public bool HasRedo
		{
			get
			{
				return mathExp.HasRedo;
			}
		}
		public CompileResult CreateMethodCompilerUnit(string classNamespace, string className, string methodName)
		{
			return mathExp.CreateMethodCompilerUnit(classNamespace, className, methodName);
		}
		public void SendLetter(string s)
		{
			mathExp.Update(s);
			_changed = true;
			this.Refresh();
		}
		public void AddEmptyNode()
		{
			mathExp.AddEmptyNode();
			_changed = true;
			this.Refresh();
		}
		public void DeleteSelectedNode()
		{
			mathExp.DeleteSelectedNode();
			mathExp.AdjustVariableMap();
			if (_editor != null)
			{
				_editor.ShowVariableMap(mathExp.VariableMap);
			}
			this.Refresh();
			_changed = true;
		}
		public void SaveLocations()
		{
			for (int i = 0; i < Controls.Count; i++)
			{
				ActiveDrawing a = Controls[i] as ActiveDrawing;
				if (a != null)
				{
					a.SaveLocation();
				}
			}
		}

		public void SaveToXmlNode(XmlNode node)
		{
			SaveLocations();
			mathExp.Save(node);
			Form f = this.FindForm();
			if (f != null)
			{
				XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_EDITORRECT, f.Bounds);
			}
			_changed = false;
		}
		public void LoadFromXmlNode(XmlNode node)
		{
			mathExp.Load(node);
			mathExp.ResetUndo();
			if (OnUndoStateChanged != null)
			{
				OnUndoStateChanged(this, null);
			}
			this.Refresh();
			_changed = false;
		}
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}
		private IActionContext _ac;
		/// <summary>
		/// when this math expression is used in an action, this is the action.
		/// if it is a property-setting action then the method is a set-property action;
		/// if it is a method execution action then this is the method-execution action.
		/// </summary>
		public IActionContext ActionContext { get { return _ac; } set { _ac = value; } }
		public bool IsStringSelected
		{
			get
			{
				if (mathExp != null)
				{
					return mathExp.IsStringSelected;
				}
				return false;
			}
		}
		//FormDebug frmDebug;
		protected override void OnPaint(PaintEventArgs e)
		{
			if (mathExp != null)
			{
				mathExp.AutoScrollPosition = this.AutoScrollPosition;
				System.Drawing.Drawing2D.GraphicsState gt0 = e.Graphics.Save();
				e.Graphics.TranslateTransform(this.AutoScrollPosition.X, 0);
				mathExp.Draw(e.Graphics);
				e.Graphics.Restore(gt0);
				SizeF size = mathExp.ExpSize;
				//
				if (size.Width > 0 && size.Height > 0)
				{
					//
					int left = (int)Math.Floor(size.Width);
					if (left != _lblForScroll.Left)
					{
						_lblForScroll.Left = left;
					}
					int nh = 0;
					while (size.Width + this.AutoScrollPosition.X - (nh * this.ClientSize.Width) > this.ClientSize.Width)
					{
						if (this.ClientSize.Height > (nh + 1) * (size.Height + 8) + size.Height)
						{
							gt0 = e.Graphics.Save();
							e.Graphics.TranslateTransform(this.AutoScrollPosition.X - (this.ClientSize.Width * (nh + 1)), (nh + 1) * (size.Height + 8));
							mathExp.Draw(e.Graphics);
							e.Graphics.Restore(gt0);
						}
						else
							break;
						nh++;
					}
				}
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (this.ClientSize.Height > 2)
			{
				if (_lblForScroll != null && _lblForScroll.Parent == this)
				{
					_lblForScroll.Top = this.ClientSize.Height - 2;
				}
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (_readOnly)
				return;
			hideSelectorList();
			//common keys
			bool bCommonKey = false;
			if (!this.IsStringSelected)
			{
				MathNode node;
				switch (e.KeyChar)
				{
					case '+':
						bCommonKey = true;
						node = mathExp.AddMathNode(typeof(PlusNode));
						mathExp.ClearFocus();
						mathExp.SetFocus(node[1]);
						break;
					case '-':
						bCommonKey = true;
						node = mathExp.AddMathNode(typeof(MinusNode));
						mathExp.ClearFocus();
						mathExp.SetFocus(node[1]);
						break;
					case '*':
						bCommonKey = true;
						node = mathExp.AddMathNode(typeof(MultiplyNode));
						mathExp.ClearFocus();
						mathExp.SetFocus(node[1]);
						break;
					case '/':
						bCommonKey = true;
						node = mathExp.AddMathNode(typeof(DivNode));
						mathExp.ClearFocus();
						mathExp.SetFocus(node[1]);
						break;
				}
			}
			if (!bCommonKey)
			{
				mathExp.UpdateSelected(new string(new char[] { e.KeyChar }));
			}
			this.Refresh();
			RefreshVariableMap();
			_changed = true;
		}
		public MathNode Highlighted
		{
			get
			{
				return _highlighted;
			}
		}
		MathNode _highlighted;
		float _lastOffsetY;
		float _lastOffsetX;
		bool _lastOffsetSaved;
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_readOnly)
				return;
			int x = e.X;
			int y = e.Y;
			int nh = 1;
			float yOffset = 0;
			float xOffset = this.AutoScrollPosition.X;
			SizeF size = mathExp.ExpSize;
			if (size.Height > 0)
			{
				while (y > (nh * (size.Height + 8)))
				{
					yOffset += (size.Height + 8);
					xOffset -= this.ClientSize.Width;
					nh++;
				}
				y -= (int)yOffset;
				x -= (int)xOffset;
			}
			HitTestResult hit = mathExp.HightLight(new PointF(x, y));
			if (hit != null)
			{
				Graphics g = this.CreateGraphics();
				System.Drawing.Drawing2D.GraphicsState gt0 = g.Save();
				if (_lastOffsetSaved)
				{
					g.TranslateTransform(_lastOffsetX, _lastOffsetY);
				}
				if (hit.Replaced != null)
				{
					g.DrawRectangle(Pens.White, hit.Replaced.Bounds);
				}
				g.Restore(gt0);
				g.TranslateTransform(xOffset, yOffset);
				_lastOffsetX = xOffset;
				_lastOffsetY = yOffset;
				_lastOffsetSaved = true;
				if (hit.Current != null)
				{
					_highlighted = hit.Current;
					g.DrawRectangle(pen, hit.Current.Bounds);
				}
				g.Restore(gt0);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_readOnly)
				return;
			hideSelectorList();
			int x = e.X;
			int y = e.Y;
			int nh = 1;
			float yOffset = 0;
			float xOffset = this.AutoScrollPosition.X;
			SizeF size = mathExp.ExpSize;
			if (size.Height > 0)
			{
				while (y > (nh * (size.Height + 8)))
				{
					yOffset += (size.Height + 8);
					xOffset -= this.ClientSize.Width;
					nh++;
				}
				y -= (int)yOffset;
				x -= (int)xOffset;
			}
			MathNode hit = mathExp.HitTest(new PointF(x, y));
			if (hit != null)
			{
				mathExp.SetFocus(hit);
			}
			else
			{
				mathExp.SetFocus(null);
			}
			this.Refresh();
			if (e.Button == MouseButtons.Right)
			{
				MathNode selectedNode = Root.FocusedNode;
				ContextMenu cm = new ContextMenu();
				MenuItem mi;
				BinOperatorNode bin = selectedNode as BinOperatorNode;
				if (bin != null)
				{
					BinOperatorNode bin2 = bin[1] as BinOperatorNode;
					if (bin2 != null)
					{
						//modify it from {0} b {b2} to 
						//{0} b {b2[0]} b2 {b2[1]}
						//new b[1] = {b2[0]}
						//new b2[0] = new b => {0} b {b2[0]}
						//new b2[1] = {b2[1]}
						mi = new MenuItemWithBitmap("Separate", mnu_separate, Resource1._separate.ToBitmap());
						cm.MenuItems.Add(mi);
					}
				}
				if (OnFinish != null)
				{
					if (cm.MenuItems.Count > 0)
					{
						cm.MenuItems.Add("-");
					}
					mi = new MenuItemWithBitmap("Finish", OnFinish, Resource1._ok.ToBitmap());
					cm.MenuItems.Add(mi);
				}
				if (OnCancel != null)
				{
					mi = new MenuItemWithBitmap("Cancel", OnCancel, Resource1._cancel.ToBitmap());
					cm.MenuItems.Add(mi);
				}
				if (OnCreateCompound != null)
				{
				}
				//
				if (cm.MenuItems.Count > 0)
				{
					cm.Show(this, new Point(e.X, e.Y));
				}
			}
		}
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			if (_highlighted != null)
			{
				_highlighted.OnDoubleClick(this);
			}
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (_readOnly)
				return true;
			if (msg.Msg == 256)
			{
				int key = msg.WParam.ToInt32();
				if (key == 37) //left
				{
					mathExp.SelectPrevious();
					this.Refresh();
					return true;
				}
				else if (key == 39) //right
				{
					mathExp.SelectNext();
					this.Refresh();
					return true;
				}
				else if (key == 0x2d) //insert
				{
					mathExp.AddEmptyNode();
					this.Refresh();
					_changed = true;
					return true;
				}
				else if (key == 0x2e) //delete
				{
					mathExp.DeleteSelectedNode();
					RefreshVariableMap();
					this.Refresh();
					_changed = true;
					return true;
				}
				else if (key == 8) //backspace
				{
					mathExp.Backspace();
					RefreshVariableMap();
					this.Refresh();
					_changed = true;
					return true;
				}
				else if (key == 38) //up
				{
					mathExp.OnKeyUp();
					_changed = true;
					this.Refresh();
					return true;
				}
				else if (key == 40) //down
				{
					mathExp.OnKeyDown();
					_changed = true;
					this.Refresh();
					return true;
				}
			}
			else if (msg.Msg == 255)
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
