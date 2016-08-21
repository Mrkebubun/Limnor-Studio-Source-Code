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
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;
using MathExp.RaisTypes;
using VPL;
using System.Collections.Specialized;

namespace MathExp
{
	[Description("a number in double precision")]
	[ToolboxBitmapAttribute(typeof(MathNodeNumber), "Resources.MathNodeNumber.bmp")]
	public class MathNodeNumber : MathNode, IPlaceHolder
	{
		#region fields and constructors
		private double _value;
		private bool _placeHolder;
		public MathNodeNumber(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region properties
		[Browsable(false)]
		public override bool IsConstant { get { return true; } }
		[Browsable(false)]
		public bool IsPlaceHolder
		{
			get
			{
				return _placeHolder;
			}
			set
			{
				_placeHolder = value;
			}
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(double);
				}
				return _dataType;
			}
		}
		public double Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_placeHolder = false;
			}
		}
		#endregion
		#region methods
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeNumber m = (MathNodeNumber)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeNumber node = (MathNodeNumber)base.CloneExp(parent);
			node.Value = _value;
			node.IsPlaceHolder = IsPlaceHolder;
			return node;
		}

		protected override void InitializeChildren()
		{
		}
		protected override void OnSave(XmlNode node)
		{
			node.InnerText = _value.ToString();
		}
		protected override void OnLoad(XmlNode node)
		{
			_value = double.Parse(node.InnerText);
		}
		public override string ToString()
		{
			return _value.ToString();
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(this.GetType().Name);
				sb.Append(" ");
				sb.Append(ToString());
				return sb.ToString();
			}
		}
		public override SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			string s = this.ToString();
			System.Drawing.Font font = this.TextFont;
			return g.MeasureString(s, font);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			if (IsFocused)
			{
				SizeF sz = DrawSize;
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(this.ToString(), this.TextFont, this.TextBrushFocus, new PointF(0, 0));
			}
			else
			{
				g.DrawString(this.ToString(), this.TextFont, this.TextBrush, new PointF(0, 0));
			}
		}

		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for value [{1}] as type [{2}]", this.GetType().Name, _value, this.CompileDataType.Type);
			if (CompileDataType.IsInteger)
			{
				double d = Math.Floor(_value);
				if (d == _value)
				{
					switch (Type.GetTypeCode(CompileDataType.Type))
					{
						case TypeCode.Byte:
							if (_value >= byte.MinValue && _value <= byte.MaxValue)
							{
								byte by = (byte)_value;
								ActualCompiledType = new RaisDataType(typeof(byte));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.Int16:
							if (_value >= Int16.MinValue && _value <= Int16.MaxValue)
							{
								Int16 by = (Int16)_value;
								ActualCompiledType = new RaisDataType(typeof(Int16));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.Int32:
							if (_value >= Int32.MinValue && _value <= Int32.MaxValue)
							{
								Int32 by = (Int32)_value;
								ActualCompiledType = new RaisDataType(typeof(Int32));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.Int64:
							if (_value >= Int64.MinValue && _value <= Int64.MaxValue)
							{
								Int64 by = (Int64)_value;
								ActualCompiledType = new RaisDataType(typeof(Int64));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.SByte:
							if (_value >= sbyte.MinValue && _value <= sbyte.MaxValue)
							{
								sbyte by = (sbyte)_value;
								ActualCompiledType = new RaisDataType(typeof(sbyte));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.UInt16:
							if (_value >= UInt16.MinValue && _value <= UInt16.MaxValue)
							{
								UInt16 by = (UInt16)_value;
								ActualCompiledType = new RaisDataType(typeof(UInt16));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.UInt32:
							if (_value >= UInt32.MinValue && _value <= UInt32.MaxValue)
							{
								UInt32 by = (UInt32)_value;
								ActualCompiledType = new RaisDataType(typeof(UInt32));
								return new CodePrimitiveExpression(by);
							}
							break;
						case TypeCode.UInt64:
							if (_value >= UInt64.MinValue && _value <= UInt64.MaxValue)
							{
								UInt64 by = (UInt64)_value;
								ActualCompiledType = new RaisDataType(typeof(UInt64));
								return new CodePrimitiveExpression(by);
							}
							break;
					}
				}
			}
			ActualCompiledType = CompileDataType;
			return new CodeCastExpression(CompileDataType.Type, new CodePrimitiveExpression(_value));
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		public override bool Update(string newValue)
		{
			if (!string.IsNullOrEmpty(newValue))
			{
				if (Char.IsLetter(newValue, 0))
				{
					MathNodeVariable node = new MathNodeVariable(this.Parent);
					node.VariableName = newValue;
					return this.ReplaceMe(node);
				}
				else
				{
					try
					{
						double d = Convert.ToDouble(newValue);
						_value = d;
						_placeHolder = false;
						return true;
					}
					catch
					{
					}
				}
			}
			return false;
		}
		#endregion
	}
	/// <summary>
	/// usually a different variable class is used for different value type,
	/// and usually derived from this calss.
	/// </summary>
	[MathNodeCategory(enumOperatorCategory.Decimal)]
	[Description("a variable")]
	[ToolboxBitmap(typeof(MathNodeVariable), "Resources.MathNodeVariable.bmp")]
	public class MathNodeVariable : MathNode, IVariable
	{
		#region ID Manager
		public static UInt32 GetNewID()
		{
			return (UInt32)Guid.NewGuid().GetHashCode();
		}
		#endregion
		#region fields and constructors
		private string _value = "x";
		protected string _subscript = "";
		private UInt32 _id;
		private bool _isReturn;
		private bool _isParam;
		private bool _isLocal;
		private bool _noAutoDeclare;
		private bool _isInPort;
		protected CodeExpression _passin;
		protected string _passinJS;
		protected string _passinPhp;
		protected Font _subscriptFont;
		private RaisDataType _dataType;
		private LinkLineNodeOutPort[] _outports;
		private LinkLineNodeInPort _inport;
		private bool _typeDefined;
		private bool _clonePorts = true;
		private IMathExpression _scope;
		public MathNodeVariable(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region methods
		protected virtual void OnInPortCreated() { }
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeVariable m = (MathNodeVariable)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this.VariableType = replaced.DataType;
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeVariable node = (MathNodeVariable)base.CloneExp(parent);
			node.VariableName = _value;
			node.SubscriptName = _subscript;
			node._id = ID;
			node.IsLocal = IsLocal;
			node.IsParam = IsParam;
			node.IsReturn = IsReturn;
			node._passin = _passin;
			if (_subscriptFont != null)
				node._subscriptFont = (Font)_subscriptFont.Clone();
			node.IsSuperscript = IsSuperscript;
			node.Position = new Point(Position.X, Position.Y);
			if (VariableType == null)
				throw new MathException("VariableType is null when clone it. {0}", _value);
			else
			{
				node.VariableType = (RaisDataType)VariableType.Clone();
			}
			if (ClonePorts)
			{
				if (_outports != null)
				{
					LinkLineNodeOutPort[] ports = new LinkLineNodeOutPort[_outports.Length];
					for (int i = 0; i < ports.Length; i++)
					{
						_outports[i].ConstructorParameters = new object[] { node };
						ports[i] = (LinkLineNodeOutPort)_outports[i].Clone();
					}
					node.OutPorts = ports;
				}
				if (_inport != null)
				{
					_inport.ConstructorParameters = new object[] { node };
					node.InPort = (LinkLineNodeInPort)_inport.Clone();
				}
			}
			return node;
		}

		protected override void InitializeChildren()
		{
		}
		public void SavePorts(XmlNode node)
		{
			IXmlCodeWriter w = GetWriter();
			if (_inport != null)
			{
				XmlSerialization.WriteToChildXmlNode(w, node, XmlSerialization.XML_PORT, _inport);
			}
			if (_outports != null)
			{
				for (int i = 0; i < _outports.Length; i++)
				{
					XmlSerialization.WriteToChildXmlNode(w, node, XmlSerialization.XML_PORT, _outports[i]);
				}
			}
		}
		public void LoadPorts(XmlNode node)
		{
			object v;
			XmlNodeList nds = node.SelectNodes(XmlSerialization.XML_PORT);
			List<LinkLineNodeOutPort> outports = new List<LinkLineNodeOutPort>();
			if (nds != null)
			{
				IXmlCodeReader r = GetReader();
				foreach (XmlNode nd in nds)
				{
					v = XmlSerialization.ReadFromXmlNode(r, nd, this);
					if (v != null)
					{
						if (v is LinkLineNodeInPort)
							InPort = (LinkLineNodeInPort)v;
						else if (v is LinkLineNodeOutPort)
							outports.Add((LinkLineNodeOutPort)v);
					}
				}
			}
			LinkLineNodeOutPort[] o = new LinkLineNodeOutPort[outports.Count];
			outports.CopyTo(o);
			OutPorts = o;
		}
		/// <summary>
		/// the base does nothing, so it is not called
		/// </summary>
		/// <param name="node"></param>
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, _value);
			XmlSerialization.SetAttribute(node, "subscript", _subscript);
			XmlSerialization.SetAttribute(node, "asInport", _isInPort);
			if (_typeDefined)
			{
				XmlSerialization.SetAttribute(node, "typeDefined", "true");
			}
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_ID, ID.ToString());
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, "ValueType", DataType);
			SavePorts(node);
		}
		protected override void OnLoad(XmlNode node)
		{
			object v;
			string s;
			_value = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
			if (string.IsNullOrEmpty(_value))
			{
				_value = "_";
			}
			_subscript = XmlSerialization.GetAttribute(node, "subscript");
			_isInPort = XmlSerialization.GetAttributeBool(node, "asInport");
			s = XmlSerialization.GetAttribute(node, "typeDefined");
			if (!string.IsNullOrEmpty(s))
			{
				if (string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					_typeDefined = true;
				}
			}
			s = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_ID);
			if (!string.IsNullOrEmpty(s))
			{
				_id = Convert.ToUInt32(s);
			}
			v = XmlSerialization.ReadFromChildXmlNode(GetReader(), node, "ValueType");
			if (v != null)
			{
				_dataType = (RaisDataType)v;
			}
			else
			{
				_dataType = new RaisDataType();
				_dataType.LibType = typeof(double);
			}
			LoadPorts(node);
		}
		public override string ToString()
		{
			return KeyName;
		}

		public override SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			System.Drawing.Font font = this.TextFont;
			SizeF size = g.MeasureString(VariableName, font);
			_subscriptFont = SubscriptFontMatchHeight(size.Height);
			SizeF ssize = g.MeasureString(_subscript, _subscriptFont);
			return new SizeF(size.Width + ssize.Width + 1, size.Height + ssize.Height / 2);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sz = DrawSize;
			System.Drawing.Font font = this.TextFont;
			string s = this.VariableName;
			SizeF size = g.MeasureString(s, font);
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
				if (!string.IsNullOrEmpty(_subscript))
				{
					g.DrawString(_subscript, _subscriptFont, this.TextBrushFocus, new PointF(size.Width, size.Height / (float)2));
				}
			}
			else
			{
				g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
				if (!string.IsNullOrEmpty(_subscript))
				{
					g.DrawString(_subscript, _subscriptFont, this.TextBrush, new PointF(size.Width, size.Height / (float)2));
				}
			}
		}

		public void AssignCode(CodeExpression code)
		{
			_passin = code;
		}
		public void AssignJavaScriptCode(string code)
		{
			_passinJS = code;
		}
		public void AssignPhpScriptCode(string code)
		{
			_passinPhp = code;
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			CodeStatementCollection supprtStatements = method.MethodCode.Statements;
			if (_passin != null)
			{
				MathNode.Trace("MathNodeVariable.ExportCode returns _passin");
				return _passin;
			}
			CodeExpression code = this.MathExpression.GetMappedCode(this, method, supprtStatements, true);
			if (code != null)
			{
				return code;
			}
			//if this variable links to a method parameter then the code is a reference to that parameter
			if (this.InPort != null)
			{
				MathExpItem item = this.root.RootContainer.GetItemByID(this.InPort.LinkedPortID);
				if (item != null)
				{
					MathNode.Trace("variable:{0} linked to MathExpItem: {1}", this.TraceInfo, item.MathExpression.TraceInfo);
					return item.ReturnCodeExpression(method);
				}
			}
			if (!NoAutoDeclare)
			{
				DeclareVariable(supprtStatements, this);
			}
			MathNode.Trace("MathNodeVariable.ExportCode returns variable reference to {0}", this.CodeVariableName);
			return new CodeVariableReferenceExpression(this.CodeVariableName);
		}
		public string ExportJavaScriptCode(StringCollection method)
		{
			return CreateJavaScript(method);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			if (!string.IsNullOrEmpty(_passinJS))
			{
				MathNode.Trace("MathNodeVariable.CreateJavaScript returns _passinJS");
				return _passinJS;
			}
			string code = this.MathExpression.GetMappedJavaScriptCode(this, method);
			if (code != null)
			{
				return code;
			}
			//if this variable links to a method parameter then the code is a reference to that parameter
			if (this.InPort != null)
			{
				MathExpItem item = this.root.RootContainer.GetItemByID(this.InPort.LinkedPortID);
				if (item != null)
				{
					MathNode.Trace("variable:{0} linked to MathExpItem: {1}", this.TraceInfo, item.MathExpression.TraceInfo);
					return item.CreatejavaScript(method);
				}
			}
			if (!NoAutoDeclare)
			{
				if (string.IsNullOrEmpty(_value))
				{
					DeclareJavaScriptVariable(method, this, null);
				}
				else
				{
					DeclareJavaScriptVariable(method, this, string.Format(CultureInfo.InvariantCulture, "'{0}'", _value.Replace("'", "\\'")));
				}
			}
			MathNode.Trace("MathNodeVariable.CreateJavaScript returns variable reference to {0}", this.CodeVariableName);
			return this.CodeVariableName;
		}
		public string ExportPhpScriptCode(StringCollection method)
		{
			return CreatePhpScript(method);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (!string.IsNullOrEmpty(_passinPhp))
			{
				MathNode.Trace("MathNodeVariable.CreatePhpScript returns _passinPhp");
				return _passinPhp;
			}
			string code = this.MathExpression.GetMappedPhpScriptCode(this, method);
			if (code != null)
			{
				return code;
			}
			//if this variable links to a method parameter then the code is a reference to that parameter
			if (this.InPort != null)
			{
				MathExpItem item = this.root.RootContainer.GetItemByID(this.InPort.LinkedPortID);
				if (item != null)
				{
					MathNode.Trace("variable:{0} linked to MathExpItem: {1}", this.TraceInfo, item.MathExpression.TraceInfo);
					return item.CreatePhpScript(method);
				}
			}
			if (!NoAutoDeclare)
			{
				DeclarePhpScriptVariable(method, this);
			}
			MathNode.Trace("MathNodeVariable.CreateJavaScript returns variable reference to {0}", this.CodeVariableName);
			return this.CodeVariableName;
		}
		public void SetVariableType(RaisDataType t)
		{
			_dataType = t;
		}
		public override bool Update(string newValue)
		{
			if (!string.IsNullOrEmpty(newValue))
			{
				bool bIsNumber = false;
				char c = newValue[0];
				if (c == '.' || c == '-')
					bIsNumber = true;
				else
				{
					UnicodeCategory uc = Char.GetUnicodeCategory(c);
					bIsNumber = (uc == UnicodeCategory.DecimalDigitNumber);
				}
				if (bIsNumber)
				{
					MathNodeNumber node = new MathNodeNumber(this.Parent);
					try
					{
						node.Value = double.Parse(newValue);
						return this.ReplaceMe(node);
					}
					catch
					{
					}
				}
				else
				{
					if (nameValid(newValue))
					{
						_value = newValue;
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}
		private bool nameValid(string name)
		{
			for (int i = 0; i < name.Length; i++)
			{
				if (!Char.IsLetter(name, i))
				{
					int a = (int)(char)name[i];
					if (!(a >= 0xC1 && a <= 0xF9))
					{
						if (!(i > 0 && name[i] >= '0' && name[i] <= '9'))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public override XmlDocument OnFindXmlDocument()
		{
			if (_dataType != null)
			{
				return _dataType.GetXmlDocument();
			}
			return null;
		}
		#endregion
		#region properties
		[Browsable(false)]
		public UInt32 PortOwnerID
		{
			get
			{
				return this.ID;
			}
		}
		/// <summary>
		/// true: a variable will be generated; false: no variable will be generated
		/// </summary>
		[Browsable(false)]
		public virtual bool IsCodeVariable { get { return true; } }
		/// <summary>
		/// MathNodeRoot[0] is for the return value of the math expression
		/// </summary>
		[Browsable(false)]
		public bool IsMathRootReturn
		{
			get
			{
				if (this.Parent is MathNodeRoot)
				{
					if (this == this.Parent[0])
					{
						if (!((MathNodeRoot)this.Parent).IsVariableHolder)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool TypeDefined
		{
			get
			{
				return _typeDefined;
			}
			set
			{
				_typeDefined = value;
			}
		}
		[Browsable(false)]
		public bool ClonePorts
		{
			get
			{
				return _clonePorts;
			}
			set
			{
				_clonePorts = value;
			}
		}
		[Browsable(false)]
		public override RaisDataType DataType
		{
			get
			{
				if (IsMathRootReturn)
				{
					return this.Parent[1].DataType;
				}
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(double);
				}
				return _dataType;
			}
		}
		#endregion
		#region IVariable Members
		[Browsable(false)]
		public MathNodeRoot MathExpression
		{
			get
			{
				return this.root;
			}
		}
		[Browsable(false)]
		public virtual bool IsDummyPort { get { return false; } }
		[Browsable(false)]
		public bool IsInPort
		{
			get
			{
				return _isInPort;
			}
			set
			{
				_isInPort = value;
			}
		}
		[Browsable(false)]
		public IMathExpression Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(this.GetType().Name);
				sb.Append(" ");
				sb.Append(this.DataType.TypeName);
				sb.Append(":");
				if (string.IsNullOrEmpty(this.VariableName))
					sb.Append("?");
				else
					sb.Append(this.VariableName);
				sb.Append(":");
				sb.Append(" ID:");
				sb.Append(this.ID.ToString());
				sb.Append(" ");
				sb.Append(this.CodeVariableName);
				return sb.ToString();
			}
		}
		public void ResetID(UInt32 id)
		{
			_id = id;
		}
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
		public virtual RaisDataType VariableType
		{
			get
			{
				return DataType;
			}
			set
			{
				_dataType = value;
				MathNodeRoot r = this.root;
				if (r != null)
				{
					List<MathNodeVariable> lst = new List<MathNodeVariable>();
					r.FindItemByType<MathNodeVariable>(lst);
					foreach (MathNodeVariable v in lst)
					{
						if (string.CompareOrdinal(v.KeyName, this.KeyName) == 0)
						{
							v.SetVariableType(value);
						}
					}
				}
			}
		}
		public string VariableName
		{
			get
			{
				return _value;
			}
			set
			{
				if (IsValidVariableName(value))
				{
					_value = value;
				}
			}
		}
		public string SubscriptName
		{
			get
			{
				return _subscript;
			}
			set
			{
				if (IsValidSubscriptVariableName(value))
				{
					_subscript = value;
				}
			}
		}

		[Browsable(false)]
		public override bool IsLocal
		{
			get
			{
				return _isLocal;
			}
			set
			{
				_isLocal = value;
			}
		}

		[Browsable(false)]
		public bool NoAutoDeclare
		{
			get
			{
				return _noAutoDeclare;
			}
			set
			{
				_noAutoDeclare = value;
			}
		}

		[Browsable(false)]
		public bool IsParam
		{
			get
			{
				return _isParam;
			}
			set
			{
				_isParam = value;
			}
		}

		[Browsable(false)]
		public bool IsReturn
		{
			get
			{
				return _isReturn;
			}
			set
			{
				_isReturn = value;
			}
		}
		[Browsable(false)]
		public virtual bool IsConst
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// identify this variable in a single string
		/// </summary>
		[Browsable(false)]
		public string KeyName
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder();
				if (string.IsNullOrEmpty(VariableName))
					sb.Append("_");
				else
					sb.Append(VariableName);
				if (!string.IsNullOrEmpty(SubscriptName))
				{
					sb.Append("_");
					sb.Append(SubscriptName);
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(VariableName))
					sb.Append(VariableName);
				if (!string.IsNullOrEmpty(SubscriptName))
				{
					sb.Append("|");
					sb.Append(SubscriptName);
				}
				return sb.ToString();
			}
		}
		/// <summary>
		/// variable name used in code compiling.
		/// in the same scope for all variables with the same KeyName they must
		/// use the same CodeVariableName, the CodeVariableName of the variable with IsInPort = true
		/// </summary>
		[Browsable(false)]
		public string CodeVariableName
		{
			get
			{
				if (root == null)
				{
					throw new MathException("Error getting CodeVariableName: root is null for variable [{0},{1}]", VariableName, SubscriptName);
				}
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(CultureInfo.InvariantCulture,
						"${0}v{1}_{2}", MathNode.VariableNamePrefix, root.ID.ToString("x", CultureInfo.InvariantCulture), ((UInt32)(KeyName.GetHashCode())).ToString("x", CultureInfo.InvariantCulture));
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture,
						"{0}v{1}_{2}", MathNode.VariableNamePrefix, root.ID.ToString("x", CultureInfo.InvariantCulture), ((UInt32)(KeyName.GetHashCode())).ToString("x", CultureInfo.InvariantCulture));
				}
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return CodeVariableName;
			}
		}
		[Browsable(false)]
		public virtual UInt32 ID
		{
			get
			{
				if (_id == 0)
					_id = GetNewID();
				return _id;
			}
		}
		[Browsable(false)]
		public virtual UInt32 RawId
		{
			get
			{
				return _id;
			}
		}
		[Browsable(false)]
		public LinkLineNodeInPort InPort
		{
			get
			{
				return _inport;
			}
			set
			{
				_inport = value;
				if (_inport != null)
				{
					_inport.SetVariable(this);
					OnInPortCreated();
				}
			}
		}
		[Browsable(false)]
		public int InPortCount
		{
			get
			{
				if (_inport == null)
				{
					return 0;
				}
				return 1;
			}
		}
		[Browsable(false)]
		public LinkLineNodeOutPort[] OutPorts
		{
			get
			{
				return _outports;
			}
			set
			{
				_outports = value;
				if (_outports != null)
				{
					for (int i = 0; i < _outports.Length; i++)
					{
						_outports[i].SetVariable(this);
					}
				}
			}
		}
		public void AddOutPort(LinkLineNodeOutPort port)
		{
			int n = _outports.Length;
			LinkLineNodeOutPort[] o = new LinkLineNodeOutPort[n + 1];
			for (int i = 0; i < n; i++)
			{
				o[i] = _outports[i];
			}
			_outports = o;
			_outports[n] = port;
		}
		public void AddInPort(LinkLineNodeInPort port)
		{
		}
		public void RemovePort(LinkLineNodePort port)
		{
		}
		#endregion
		#region static utilities
		const string InvalidVariableLetters = "!^&*()-.+={}[]|\\/?< >,;:\"'";
		public static bool IsValidVariableName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;
			if (name[0] >= '0' && name[0] <= '9')
				return false;
			for (int i = 0; i < name.Length; i++)
			{
				if (InvalidVariableLetters.IndexOf(name[i]) >= 0)
					return false;
			}
			return true;
		}
		public static bool IsValidSubscriptVariableName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return true;
			for (int i = 0; i < name.Length; i++)
			{
				if (InvalidVariableLetters.IndexOf(name[i]) >= 0)
					return false;
			}
			return true;
		}
		public static bool VariableDeclared(CodeStatementCollection supprtStatements, string varName)
		{
			CodeVariableDeclarationStatement cv;
			for (int i = 0; i < supprtStatements.Count; i++)
			{
				if (supprtStatements[i] is CodeVariableDeclarationStatement)
				{
					cv = (CodeVariableDeclarationStatement)supprtStatements[i];
					if (cv.Name == varName)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// declare a variable and initialize it with default value.
		/// </summary>
		/// <param name="supprtStatements"></param>
		/// <param name="var"></param>
		public static void DeclareVariable(CodeStatementCollection supprtStatements, IVariable var)
		{
			if (var is MathNodeVariableDummy)
			{
				return;
			}
			if (!VariableDeclared(supprtStatements, var.CodeVariableName))
			{
				MathNode.Trace("Declare variable {0}", var.TraceInfo);
				CodeVariableDeclarationStatement p;
				if (var.VariableType.Type.IsValueType)
				{
					p = new CodeVariableDeclarationStatement(new CodeTypeReference(var.VariableType.Type), var.CodeVariableName);
				}
				else
				{
					p = new CodeVariableDeclarationStatement(new CodeTypeReference(var.VariableType.Type), var.CodeVariableName, ValueTypeUtil.GetDefaultCodeByType(var.VariableType.Type));
				}
				supprtStatements.Add(p);
			}
		}
		public static void DeclareJavaScriptVariable(StringCollection statements, IVariable var, string val)
		{
			if (var is MathNodeVariableDummy)
			{
				return;
			}
			//use JavaScript's auto variable declaration
			string v;
			if (string.IsNullOrEmpty(val))
			{
				v = ValueTypeUtil.GetDefaultJavaScriptValueByType(var.VariableType.Type);
			}
			else
			{
				v = val;
			}
			string s;
			s = MathNode.FormString("{0}={1};\r\n", var.CodeVariableName, v);
			statements.Add(s);
		}
		public static void DeclarePhpScriptVariable(StringCollection statements, IVariable var)
		{
			if (var is MathNodeVariableDummy)
			{
				return;
			}
		}
		/// <summary>
		/// assign value to a variable. declare it if not already eclared
		/// </summary>
		/// <param name="supprtStatements"></param>
		/// <param name="var"></param>
		/// <param name="code"></param>
		public static void AssignVariable(CodeStatementCollection supprtStatements, IVariable var, CodeExpression code)
		{
			if (var is MathNodeVariableDummy)
			{
				MathNode.Trace("Assign dummy variable {0} becomes code statement {1}", var.TraceInfo, code);
				supprtStatements.Add(new CodeExpressionStatement(code));
				return;
			}
			if (var.VariableType.IsLibType)
			{
				if (var.VariableType.LibType.Equals(typeof(void)))
				{
					MathNode.Trace("Assign void variable {0} becomes code statement {1}", var.TraceInfo, code);
					supprtStatements.Add(new CodeExpressionStatement(code));
					return;
				}
			}
			CodeVariableDeclarationStatement cv;
			for (int i = 0; i < supprtStatements.Count; i++)
			{
				if (supprtStatements[i] is CodeVariableDeclarationStatement)
				{
					cv = (CodeVariableDeclarationStatement)supprtStatements[i];
					if (cv.Name == var.CodeVariableName)
					{
						MathNode.Trace("Assign variable {0}={1}", var.TraceInfo, code);
						CodeAssignStatement cs = new CodeAssignStatement(
										new CodeVariableReferenceExpression(var.CodeVariableName),
										 code);
						supprtStatements.Add(cs);
						return;
					}
				}
			}
			MathNode.Trace("Declare and assign variable {0}={1}", var.TraceInfo, code);
			cv = new CodeVariableDeclarationStatement(new CodeTypeReference(var.VariableType.Type), var.CodeVariableName, code);
			supprtStatements.Add(cv);
		}
		#endregion

		#region IParameter Members
		[Browsable(false)]
		public UInt32 ParameterID
		{
			get
			{
				return this.ID;
			}
			set
			{
				_id = value;
			}
		}

		#endregion

		#region INamedDataType Members
		[Browsable(false)]
		public string Name
		{
			get
			{
				return this.VariableName;
			}
			set
			{
				this.VariableName = value;
			}
		}
		[Browsable(false)]
		public Type ParameterLibType
		{
			get
			{
				return this.VariableType.LibType;
			}
			set
			{
				this.VariableType.LibType = value;
			}
		}
		[Browsable(false)]
		public string ParameterTypeString
		{
			get { return ParameterLibType.AssemblyQualifiedName; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return CloneExp(this.root);
		}

		#endregion
	}
	/// <summary>
	/// for an inputless item, create a dummy inport so that another item may be linked to it.
	/// </summary>
	[MathNodeCategory(enumOperatorCategory.Internal)]
	public class MathNodeVariableDummy : MathNodeVariable
	{
		//const string XMLATTR_CanLink = "canLinkOutPort";
		public MathNodeVariableDummy(MathNode parent)
			: base(parent)
		{
			VariableName = "dummy";
			InPort = new LinkLineNodeInPort(this);
		}
		public override bool IsDummyPort { get { return true; } }
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			return null;
		}
		protected override void OnInPortCreated()
		{
			((DrawingVariable)(InPort.Label)).LabelVisible = false;
		}
	}
}
