/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MathExp;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.CodeDom;
using System.Collections.Specialized;
using VPLDrawing;
using System.Reflection;
using ProgElements;
using VPL;
using System.Xml.Serialization;
using System.Globalization;

namespace MathExp.RaisTypes
{
	public class EventArgName : EventArgs
	{
		public object Owner;
		public string Name;
		public bool Cancel = false;
		public EventArgName(object owner, string name)
		{
			Owner = owner;
			Name = name;
		}
	}
	/// <summary>
	/// method parameter to be displayed in math group editor.
	/// add visual properties to a RaisDataType.
	/// another class implementing IProgramEntity is DotNetCompiler.VplItem
	/// </summary>
	public class Parameter : ParameterDef, IMathExpression, INonHostedItem, IPortOwner, IProgramEntity, IMethodPointer
	{
		#region Fields and constructors
		private const string XML_Cast = "Cast";
		private Color _color;
		private CodeExpression _code;
		private string _codeJS;
		private string _codePhp;
		private Point _location;
		private MethodType _method;
		private IMethod _methodScope;
		private MathNodeVariable _codeVariable;
		ParameterDrawing pd;
		private RaisDataType _actualCompiledType;
		private RaisDataType _compileDataType;
		private bool _useCompileDataType;
		private bool _fixed;
		private Dictionary<Type, object> _localServices;
		//program entity =============================
		private ProgramEntity _castTo;
		//============================================
		private XmlNode _xmlNode;
		public Parameter(MethodType method)
			: this()
		{
			_method = method;
		}
		public Parameter(MethodType method, RaisDataType rt)
			: this(method)
		{
			_method = null;
			SetType(rt);
			_method = method;
		}
		public Parameter(Type t, string n)
			: base(t, n)
		{
			Init();
		}
		public Parameter(RaisDataType t)
			: base(t)
		{
			Init();
		}
		public Parameter()
		{
			Init();
		}
		protected void Init()
		{
			LinkLineNodeOutPort[] _outports = new LinkLineNodeOutPort[] { new LinkLineNodeOutPort(this) };
			this.Variable.OutPorts = _outports;
			this.Variable.MathExpression.SetFont(new Font("Times New Roman", 12));
			_color = System.Drawing.Color.Blue;
			_outports[0].PositionType = enumPositionType.Circle;
			this.Variable.VariableName = this.Name;
			this.Variable.VariableType = this.DataType;
			((DrawingVariable)(_outports[0].Label)).SetVariable(this.Variable);
			((DrawingVariable)(_outports[0].Label)).SetOwner(_outports[0]);
		}
		#endregion
		#region INonHostedItem members
		/// <summary>
		/// create ParameterDrawing and ports
		/// </summary>
		/// <returns></returns>
		public Control[] GetControls()
		{
			List<LinkLineNodeOutPort> _outports = new List<LinkLineNodeOutPort>();
			LinkLineNodeOutPort[] _outportsV = this.Variable.OutPorts;
			List<Control> cs = new List<Control>();
			pd = new ParameterDrawing();
			pd.Parameter = this;
			cs.Add(pd);
			_outports.AddRange(_outportsV);
			//cast type==================================
			if (_castTo != null)
			{
				if (_castTo.Outports != null)
				{
					_outports.AddRange(_castTo.Outports);
				}
				//new out port
				if (_castTo.NewOutport == null)
				{
					ProgramNewOutPort op = new ProgramNewOutPort(this);
					_castTo.NewOutport = op;
				}
				pd.XmlData = _castTo.OwnerObject.GetXmlNode();
				_outports.Add(_castTo.NewOutport);
			}
			//===========================================
			for (int i = 0; i < _outports.Count; i++)
			{
				cs.Add(_outports[i].Label);
				_outports[i].Owner = pd;
				if (_outports[i] is ProgramOutPort)
				{
				}
				else
				{
					if (_outports[i].Label != null)
					{
						((DrawingVariable)_outports[i].Label).OnMouseSelect += new EventHandler(Parameter_OnMouseSelect);
						if (((DrawingVariable)_outports[i].Label).Variable != null)
						{
							((DrawingVariable)_outports[i].Label).Variable.VariableName = this.Name;
							((MathNodeVariable)(((DrawingVariable)_outports[i].Label).Variable)).VariableType = this.DataType;
						}
					}
				}
				if (_outports[i].LinkedInPort == null)
				{
					LinkLineNode l = _outports[i];
					while (l != null)
					{
						cs.Add(l);
						l = (LinkLineNode)l.NextNode;
					}
				}
				else
				{
					cs.Add(_outports[i]);
				}
			}
			Control[] a = new Control[cs.Count];
			cs.CopyTo(a);
			return a;
		}

		void Parameter_OnMouseSelect(object sender, EventArgs e)
		{
			if (pd != null && pd.Parent != null)
			{
				IMathDesigner dr = pd.Parent as IMathDesigner;
				if (dr != null)
				{
					dr.ShowProperties(this);
				}
			}
		}
		/// <summary>
		/// it should be called after the controls are added to Parent.Controls
		/// </summary>
		public void InitLocation()
		{
			if (pd != null)
			{
				pd.Location = _location;
			}
		}
		public void InitLinkLine()
		{
			if (pd != null)
			{
				LinkLineNodeOutPort[] _outports = this.Variable.OutPorts;
				for (int i = 0; i < _outports.Length; i++)
				{
					if (_outports[i].LinkedInPort == null)
					{
						LinkLineNode l = _outports[i];
						while (l != null)
						{
							l.CreateForwardLine();
							((Control)l).Visible = (l is LinkLineNodePort);
							l = (LinkLineNode)l.NextNode;
						}
					}
				}
			}
		}
		#endregion
		#region public methods
		public static Parameter[] CreateParaneters(MethodType method, ParameterDef[] ps)
		{
			if (ps == null)
				return null;
			Parameter[] prs = new Parameter[ps.Length];
			for (int i = 0; i < ps.Length; i++)
			{
				prs[i] = new Parameter(method, ps[i]);
				prs[i].Direction = ps[i].Direction;
			}
			return prs;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
				return base.ToString();
			return this.Name + ":" + base.ToString();
		}
		public void SetType(RaisDataType t)
		{
			this.DevType = t.DevType;
			this.Name = t.Name;
			this.LibType = t.LibType;
			Variable.VariableType = t;
		}
		[Browsable(false)]
		public LinkLineNodeOutPort[] OutPorts
		{
			get
			{
				return this.Variable.OutPorts;
			}
			set
			{
				this.Variable.OutPorts = value;
			}
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			Parameter p = (Parameter)base.Clone();
			p.Method = this.Method;
			p.Fixed = Fixed;
			p.TextFont = TextFont;
			p.TextColor = TextColor;
			p.Location = Location;
			p.TransferLocalService(_localServices);
			MathNodeRoot r = Variable.MathExpression;
			MathNodeRoot r0 = (MathNodeRoot)r.Clone();
			MathNodeVariable v = r0[0] as MathNodeVariable;
			p.SetCodeVariable(v);
			if (_castTo != null)
			{
				p.ProgEntity = (ProgramEntity)_castTo.Clone();
			}
			return p;
		}
		#endregion
		#region properties
		[Browsable(false)]
		public IVariable Variable
		{
			get
			{
				if (_codeVariable == null)
				{
					MathNodeRoot r = new MathNodeRoot();
					_codeVariable = new MathNodeVariable(r);
					_codeVariable.VariableName = this.Name;
					_codeVariable.VariableType = this.DataType;
					r.IsVariableHolder = true;
					r[0] = _codeVariable;
				}
				return _codeVariable;
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
		public UInt32 ID
		{
			get
			{
				return this.Variable.ID;
			}
			set
			{
				this.Variable.ResetID(value);
			}
		}
		[Browsable(false)]
		public MethodType Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}
		public Point Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
				InitLocation();
			}
		}
		public Font TextFont
		{
			get
			{
				return this.Variable.MathExpression.TextFont;
			}
			set
			{
				this.Variable.MathExpression.SetFont(value);
				LinkLineNodeOutPort[] _outports = this.Variable.OutPorts;
				for (int i = 0; i < _outports.Length; i++)
				{
					((DrawingVariable)(_outports[i].Label)).SetTextFont(value);
					_outports[i].Label.Refresh();
				}
			}
		}
		public Color TextColor
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				LinkLineNodeOutPort[] _outports = this.Variable.OutPorts;
				for (int i = 0; i < _outports.Length; i++)
				{
					((DrawingVariable)(_outports[i].Label)).SetTextColor(_color);
					_outports[i].Label.Refresh();
				}
			}
		}

		#endregion
		#region override functions
		protected override void OnBeforeNameChange(string oldName, string newName, ref bool cancel)
		{
			if (oldName != newName)
			{
				if (_method != null)
				{
					if (_method.ParameterNameExist(newName, this.ID))
					{
						System.Windows.Forms.MessageBox.Show(string.Format("Name '{0}' is in use", newName));
						cancel = true;
					}
					if (!cancel)
					{
						if (!_method.ParameterNameValid(newName))
						{
							System.Windows.Forms.MessageBox.Show(string.Format("Name '{0}' is not valid", newName));
							cancel = true;
						}
					}
				}
			}
		}
		protected override void OnAfterNameChange()
		{
			Variable.VariableName = this.Name;
			LinkLineNodeOutPort[] _outports = this.Variable.OutPorts;
			for (int i = 0; i < _outports.Length; i++)
			{
				if (_outports[i] != null && _outports[i].Label != null)
				{
					if (((DrawingVariable)_outports[i].Label).Variable != null)
					{
						((DrawingVariable)_outports[i].Label).Variable.VariableName = this.Name;
						((DrawingVariable)_outports[i].Label).Refresh();
					}
				}
			}
			base.OnAfterNameChange();
		}
		#endregion
		#region IXmlNodeSerializable Members
		object _serializer;
		public void SetSerializer(object serializer)
		{
			_serializer = serializer;
		}
		public object Serializer
		{
			get
			{
				return _serializer;
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
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_xmlNode = node;
			base.OnReadFromXmlNode(serializer, node);
			_fixed = XmlSerialization.GetAttributeBool(node, MathNodeRoot.XMLATT_FIXED);
			object v;
			if (XmlSerialization.ReadValueFromChildNode(node, "Color", out v))
			{
				_color = (Color)v;
			}
			if (XmlSerialization.ReadValueFromChildNode(node, "Location", out v))
			{
				_location = (Point)v;
			}
			TextColor = _color;
			ReadVariable(node);
			XmlNode nd = node.SelectSingleNode(XML_Cast);
			if (nd != null)
			{
				_castTo = (ProgramEntity)XmlSerialization.ReadFromXmlNode(serializer, nd);
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_xmlNode = node;
			base.OnWriteToXmlNode(serializer, node);
			//
			XmlSerialization.SetAttribute(node, MathNodeRoot.XMLATT_FIXED, _fixed);
			XmlSerialization.WriteValueToChildNode(node, "Color", _color);
			XmlSerialization.WriteValueToChildNode(node, "Location", _location);
			SaveVariable(node);
			if (_castTo != null)
			{
				XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, XML_Cast, _castTo);
			}
		}
		protected void SaveVariable(XmlNode node)
		{
			MathNodeVariable v = Variable as MathNodeVariable;
			MathNodeRoot r = v.Parent as MathNodeRoot;
			XmlSerialization.WriteValueToChildNode(node, "Font", r.TextFont);
			XmlNode nd = node.SelectSingleNode("Var");
			if (nd == null)
			{
				nd = node.OwnerDocument.CreateElement("Var");
				node.AppendChild(nd);
			}
			else
			{
				nd.RemoveAll();
			}
			v.Save(nd);
		}
		protected void ReadVariable(XmlNode node)
		{
			MathNodeVariable v = Variable as MathNodeVariable;
			MathNodeRoot r = v.Parent as MathNodeRoot;
			XmlNode nd = node.SelectSingleNode("Font");
			if (nd != null)
			{
				object v0;
				if (XmlSerialization.ReadValue(nd, out v0))
				{
					if (v0 != null)
					{
						r.SetFont((Font)v0);
					}
				}
			}
			nd = node.SelectSingleNode("Var");
			if (nd != null)
			{
				v.Load(nd);
			}
			v.VariableName = this.Name;
		}
		#endregion
		#region IMathExpression Members
		public bool IsValid
		{
			get { return true; }
		}
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_codeVariable != null)
				{
					ISourceValuePointer v = _codeVariable as ISourceValuePointer;
					if (v != null)
					{
						if (v.IsWebClientValue())
						{
							if (!v.IsWebServerValue())
							{
								return EnumWebRunAt.Client;
							}
						}
						else
						{
							if (v.IsWebServerValue())
							{
								return EnumWebRunAt.Server;
							}
						}
					}
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public void SetDataType(RaisDataType type)
		{
			SetType(type);
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			if (this.RunAt == EnumWebRunAt.Server)
			{
				List<ISourceValuePointer> l = new List<ISourceValuePointer>();
				IList<ISourceValuePointer> ls = GetValueSources();
				if (ls != null && ls.Count > 0)
				{
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
				}
				return l;
			}
			return null;
		}
		public IList<ISourceValuePointer> GetValueSources()
		{
			List<ISourceValuePointer> l = new List<ISourceValuePointer>();
			if (_codeVariable != null)
			{
				_codeVariable.GetValueSources(l);
			}
			return l;
		}
		public CompileResult CreateMethodCompilerUnit(string classNamespace, string className, string methodName)
		{
			return null;
		}
		public MethodInfo GetCalculationMethod(object propertyOwner, List<IPropertyPointer> pointers)
		{
			return null;
		}
		public void GetPointers(Dictionary<string, IPropertyPointer> pointers)
		{
		}
		public void SetParameterReferencesJS(Parameter[] ps)
		{
			for (int i = 0; i < ps.Length; i++)
			{
				if (Variable.KeyName == ps[i].Variable.KeyName)
				{
					_codeJS = string.Format(CultureInfo.InvariantCulture,
						"{0}[{1}]", MathNode.THREAD_ARGUMENT, i + 1);

					break;
				}
			}
		}
		public void SetParameterReferences(Parameter[] ps)
		{
			for (int i = 0; i < ps.Length; i++)
			{
				if (Variable.KeyName == ps[i].Variable.KeyName)
				{
					_code = new CodeArrayIndexerExpression(
						new CodeVariableReferenceExpression(MathNode.THREAD_ARGUMENT),
						new CodePrimitiveExpression(i + 1));
					this.DataType = new RaisDataType(typeof(object));
					break;
				}
			}
		}
		public void SetParameterMethodID(UInt32 methodID)
		{
			Variable.MathExpression.Dummy.ResetID(methodID);
		}
		public string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder("parameter name:'");
				sb.Append(this.Name);
				sb.Append("' type:");
				sb.Append(this.DataType.ToString());
				return sb.ToString();
			}
		}
		private bool _eu;
		[Browsable(false)]
		public bool EnableUndo { get { return _eu; } set { _eu = value; } }
		[Browsable(false)]
		public bool IsContainer
		{
			get
			{
				return false;
			}
		}
		public void ClearFocus()
		{
		}

		public IMathEditor CreateEditor(Rectangle rcStart)
		{
			return null;
		}

		public Bitmap CreateIcon(Graphics g)
		{
			if (pd != null)
			{
			}
			return null;
		}
		[Browsable(false)]
		public bool ContainLibraryTypesOnly
		{
			get
			{
				if (this.IsLibType)
					return true;
				return !this.DevType.NeedCompile;
			}
		}
		public void GetAllImports(AssemblyRefList imports)
		{
		}
		public VariableList FindAllInputVariables()
		{
			return new VariableList();
		}
		public VariableList DummyInputs(bool create)
		{
			return null;
		}
		public IVariable GetVariableByID(UInt32 id)
		{
			LinkLineNodeOutPort[] _outports = this.OutPorts;
			for (int i = 0; i < _outports.Length; i++)
			{
				if (_outports[i].PortID == id)
					return _outports[i].Variable;
			}
			return null;
		}
		public IVariable GetVariableByKeyName(string keyname)
		{
			return null;
		}
		public void SetCodeVariable(MathNodeVariable v)
		{
			_codeVariable = v;
		}
		[Browsable(false)]
		public VariableList InputVariables
		{
			get { return null; }
		}
		[Browsable(false)]
		public VariableList InportVariables
		{
			get { return null; }
		}
		[Browsable(false)]
		public IVariable OutputVariable
		{
			get
			{
				return Variable;
			}
		}
		public void PrepareDrawInDiagram()
		{
		}
		public void GenerateInputVariables()
		{
		}
		public void ReplaceVariableID(UInt32 currentID, UInt32 newID)
		{
			if (this.Variable.ID == currentID)
			{
				this.Variable.ResetID(newID);
			}
		}
		public void PrepareForCompile(IMethodCompile method)
		{
			Variable.MathExpression.Dummy.ResetID(method.MethodID);
		}

		public virtual CodeExpression ReturnCodeExpression(IMethodCompile method)
		{
			string s = method.GetParameterCodeNameById(this.ID);
			bool bFound = !string.IsNullOrEmpty(s);
			if (bFound)
			{
				if (_code != null)
					return _code;
				_actualCompiledType = null;
				return new CodeArgumentReferenceExpression(Variable.CodeVariableName);
			}
			else
			{
				_actualCompiledType = new RaisDataType(this.DataType.Type);
				return ValueTypeUtil.GetDefaultCodeByType(this.DataType.Type);
			}
		}
		public virtual string ReturnJavaScriptCodeExpression(StringCollection methodCode)
		{
			return CreateJavaScript(methodCode);
		}
		public virtual string ReturnPhpScriptCodeExpression(StringCollection methodCode)
		{
			return CreatePhpScript(methodCode);
		}
		public string CreateJavaScript(StringCollection code)
		{
			if (!string.IsNullOrEmpty(_codeJS))
				return _codeJS;
			return (Variable.CodeVariableName);
		}
		public string CreatePhpScript(StringCollection code)
		{
			if (!string.IsNullOrEmpty(_codePhp))
				return _codePhp;
			return (Variable.CodeVariableName);
		}
		public void AssignCodeExp(CodeExpression code, string codeVarName)
		{
			if (Variable.CodeVariableName == codeVarName)
				_code = code;
		}
		public void AssignJavaScriptCodeExp(string code, string codeVarName)
		{
			if (string.CompareOrdinal(Variable.CodeVariableName, codeVarName) == 0)
				_codeJS = code;
		}
		public void AssignPhpScriptCodeExp(string code, string codeVarName)
		{
			if (string.CompareOrdinal(Variable.CodeVariableName, codeVarName) == 0)
				_codePhp = code;
		}
		[Browsable(false)]
		public RaisDataType ActualCompileDataType
		{
			get
			{
				if (_actualCompiledType != null)
					return _actualCompiledType;
				return this.DataType;
			}
		}
		[Browsable(false)]
		public RaisDataType CompileDataType
		{
			get
			{
				return _compileDataType;
			}
			set
			{
				_compileDataType = value;
			}
		}

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
		[Browsable(false)]
		public VariableList Variables
		{
			get
			{
				VariableList list = new VariableList();
				list.Add(this.Variable);
				return list;
			}
		}
		public MathExpItem GetItemByID(UInt32 portId)
		{
			return null;
		}
		public MathExpItem RemoveItemByID(UInt32 portId)
		{
			return null;
		}
		[Browsable(false)]
		public MathExpItem ContainerMathItem
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public IMathExpression RootContainer
		{
			get
			{
				return null;
			}
		}
		public void AddItem(IMathExpression item)
		{
		}
		private IActionContext _ac;
		/// <summary>
		/// when it is used within an action definition, this is the action.
		/// It is needed before loading from XML if variable map is used
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionContext ActionContext { get { return _ac; } set { _ac = value; } }
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
			}
		}
		private Type _vtt;
		/// <summary>
		/// the type for the mapped instance.
		/// currently it is ParameterValue.
		/// It is needed before loading from XML if variable map is used
		/// </summary>
		public Type VariableMapTargetType { get { return _vtt; } set { _vtt = value; } }
		public void SetParameterValueChangeEvent(EventHandler h)
		{
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
		public object GetLocalService(Type serviceType)
		{
			if (_localServices != null && _localServices.ContainsKey(serviceType))
			{
				return _localServices[serviceType];
			}
			return null;
		}
		public void AddLocalService(Type serviceType, object service)
		{
			if (_localServices == null)
				_localServices = new Dictionary<Type, object>();
			_localServices.Add(serviceType, service);
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
				return _site;
			}
			set
			{
				_site = value;
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
		#region IPortOwner Members
		public UInt32 PortOwnerID
		{
			get
			{
				return this.ID;
			}
		}
		public bool IsDummyPort
		{
			get { return false; }
		}
		public void AddOutPort(LinkLineNodeOutPort port)
		{
			LinkLineNodeOutPort[] _outports = Variable.OutPorts;
			int n = _outports.Length;
			LinkLineNodeOutPort[] o = new LinkLineNodeOutPort[n + 1];
			for (int i = 0; i < n; i++)
			{
				o[i] = _outports[i];
			}
			_outports = o;
			_outports[n] = port;
			Variable.OutPorts = _outports;
		}
		public void AddInPort(LinkLineNodeInPort port)
		{
		}
		public void RemovePort(LinkLineNodePort port)
		{
		}
		public int InPortCount
		{
			get
			{
				return 0;
			}
		}
		#endregion
		#region IProgramEntity Members

		public ProgramEntity ProgEntity
		{
			get
			{
				return _castTo;
			}
			set
			{
				_castTo = value;
			}
		}
		public bool CanAddInputs
		{
			get
			{
				return false;
			}
		}
		public bool CanAddOutputs
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// implement IProgramEntity.ReturnCodeExpression
		/// if _castTo is null then this function returns the same as IMathExpression.ReturnCodeExpression
		/// </summary>
		/// <param name="portSelection"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public CodeExpression ReturnCodeExpression(object portSelection, IMethodCompile method)
		{
			CodeExpression ce = ReturnCodeExpression(method);
			if (_castTo != null)
			{
				IProgramPort p = portSelection as IProgramPort;
				if (p != null)
				{
					ObjectRef prop = p.PortProperty;
					if (prop != null)
					{
						StringCollection props = new StringCollection();
						ce = new CodeCastExpression(_castTo.EntityType.TypeString, VPLUtil.GetCoreExpressionFromCast(ce));
						while (true)
						{
							props.Add(prop.localName);
							prop = prop.Owner;
							if (prop == null)
								break;
							if (prop.IsSameType(_castTo.EntityType))
								break;
						}
						for (int i = props.Count - 1; i >= 0; i--)
						{
							ce = new CodePropertyReferenceExpression(ce, props[i]);
						}
					}
				}
			}
			return ce;
		}

		public string CreateJavaScript(object portSelection, StringCollection method)
		{
			string ce = CreateJavaScript(method);
			if (_castTo != null)
			{
				IProgramPort p = portSelection as IProgramPort;
				if (p != null)
				{
					ObjectRef prop = p.PortProperty;
					if (prop != null)
					{
						StringCollection props = new StringCollection();
						while (true)
						{
							props.Add(prop.localName);
							prop = prop.Owner;
							if (prop == null)
								break;
							if (prop.IsSameType(_castTo.EntityType))
								break;
						}
						for (int i = props.Count - 1; i >= 0; i--)
						{
							ce = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ce, props[i]);
						}
					}
				}
			}
			return ce;
		}
		public string CreatePhpScript(object portSelection, StringCollection method)
		{
			string ce = CreatePhpScript(method);
			if (_castTo != null)
			{
				IProgramPort p = portSelection as IProgramPort;
				if (p != null)
				{
					ObjectRef prop = p.PortProperty;
					if (prop != null)
					{
						StringCollection props = new StringCollection();
						while (true)
						{
							props.Add(prop.localName);
							prop = prop.Owner;
							if (prop == null)
								break;
							if (prop.IsSameType(_castTo.EntityType))
								break;
						}
						for (int i = props.Count - 1; i >= 0; i--)
						{
							ce = string.Format(CultureInfo.InvariantCulture, "{0}->{1}", ce, props[i]);
						}
					}
				}
			}
			return ce;
		}
		public Bitmap CreateDrawIcon()
		{
			return null;
		}
		#endregion

		#region IMethod Members
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			Parameter p = (Parameter)this.Clone();
			p.ActionContext = action;
			return p;
		}
		private object _prj;
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public object ModuleProject
		{
			get
			{
				if (_prj != null)
				{
					return _prj;
				}
				if (_methodScope != null && _methodScope.ModuleProject != null)
				{
					return _methodScope.ModuleProject;
				}
				return null;
			}
			set
			{
				_prj = value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }
		public bool NoReturn { get { return true; } }
		public bool HasReturn { get { return false; } }
		public IObjectIdentity ReturnPointer { get { return null; } set { } }
		public object GetParameterValue(string name) { return null; }
		public object GetParameterType(UInt32 id)
		{
			return this.DataType.LibType;
		}
		public void SetParameterValue(string name, object value) { }
		public Dictionary<string, string> GetParameterDescriptions() { return null; }
		public string ParameterName(int i) { return null; }
		public void ValidateParameterValues() { }
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
		public IParameter MethodReturnType
		{
			get
			{
				return this.Variable;
			}
			set
			{

			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get { return null; }
		}
		public IList<IParameterValue> MethodParameterValues
		{
			get { return null; }
		}
		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", this.Variable.CodeVariableName, ID.ToString("x", CultureInfo.InvariantCulture)); }
		}
		public string MethodSignature
		{
			get { return this.Variable.CodeVariableName; }
		}
		public bool IsSameMethod(ProgElements.IMethod method)
		{
			Parameter p = method as Parameter;
			if (p != null)
			{
				return (p.Variable.ID == this.ID);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				return 0;
			}
		}
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
			Parameter p = objectIdentity as Parameter;
			if (p != null)
			{
				return (p.ID == this.ID);
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Both; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Unknown; }
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
	}
	public class ValueForSetProperty : Parameter
	{
		public ValueForSetProperty(MethodType method)
			: base(method)
		{
		}
		public ValueForSetProperty(MethodType method, RaisDataType rt)
			: base(method, rt)
		{
		}
		public ValueForSetProperty(Type t, string n)
			: base(t, n)
		{
		}
		public ValueForSetProperty()
			: base()
		{
		}
		public override CodeExpression ReturnCodeExpression(IMethodCompile method)
		{
			return new CodePropertySetValueReferenceExpression();
		}
	}
}
