/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using MathExp;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.IO;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace MathComponent
{
	[Description("This control shows/edits an expression graphically. It can be a math expression for numeric value, a logic expression for boolean, or a text expression for string. Evaluation of the expression (calculations) can be done at runtime to get results from variable inputs. Inputs can be linked to properties from objects.")]
	[ToolboxBitmapAttribute(typeof(MathematicExpression), "Resources.MathNodeFunction.bmp")]
	public partial class MathematicExpression : Control, ICustomTypeDescriptor, IXmlCodeReaderWriterHolder, ISourceValueEnumProvider
	{
		#region fields and constructors
		public const string XML_Root = "Root";
		public const string XML_Variables = "Variables";
		public const string XML_Item = "Item";
		//
		private List<DrawingSurface> _surfaces = new List<DrawingSurface>();
		private MathNodeRoot _root = new MathNodeRoot();
		private bool _autoSize;
		private object _result;
		private Image _image;
		private FormulaProperty _formula;
		private bool _compileOK;
		private CompileResult _compileResult;
		private Dictionary<string, MathPropertyPointer> _variables;
		private PropertyGrid _propGrid;
		private string _lastError;
		static void addKnownType(Type t)
		{
			XmlUtil.AddKnownType(t.Name, t);
		}
		static MathematicExpression()
		{
			addKnownType(typeof(FormulaProperty));
			addKnownType(typeof(MathematicExpression));
			addKnownType(typeof(MathPropertyPointer));
			addKnownType(typeof(MathPropertyPointerForm));
			addKnownType(typeof(MathPropertyPointerComponent));
			addKnownType(typeof(MathPropertyPointerControl));
			addKnownType(typeof(MathPropertyPointerProperty));
			addKnownType(typeof(MathPropertyPointerField));
		}
		public MathematicExpression()
		{
			InitializeComponent();
		}

		public MathematicExpression(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
		}
		#endregion
		#region DrawingSurface class
		class DrawingSurface
		{
			private Control _surface;
			private Point _location;
			private MathematicExpression _owner;
			private bool _disabled;
			public DrawingSurface(Control c, Point loc, MathematicExpression owner)
			{
				_surface = c;
				_location = loc;
				_owner = owner;
				_surface.Paint += new PaintEventHandler(_surface_Paint);
			}

			void _surface_Paint(object sender, PaintEventArgs e)
			{
				if (!_disabled)
				{
					Point l = _owner._root.Position;
					_owner._root.Position = _location;
					_owner._root.Draw(e.Graphics);
					_owner._root.Position = l;
				}
			}
			public void Disable()
			{
				_disabled = true;
			}
			public Control Surface
			{
				get
				{
					return _surface;
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
				}
			}
		}
		#endregion
		#region Variables
		[Browsable(false)]
		public object GetVariable(string name)
		{
			if (_variables != null)
			{
				MathPropertyPointer p;
				if (_variables.TryGetValue(name, out p))
				{
					return p;
				}
			}
			return null;
		}
		[Browsable(false)]
		public void ResetValue(string name)
		{
			if (_variables != null)
			{
				if (_variables.ContainsKey(name))
				{
					_variables[name] = new MathPropertyPointer();
				}
				else
				{
					_variables.Add(name, new MathPropertyPointer());
				}
				refreshXml();
			}
		}
		[Description("Assign the value to the input identified by name")]
		public void SetVariable(string name, object value)
		{
			if (_variables == null)
			{
				createInputs();
			}
			MathPropertyPointer mp = value as MathPropertyPointer;
			if (mp == null)
			{
				mp = new MathPropertyPointerConstant();
				mp.Instance = value;
			}
			if (_variables.ContainsKey(name))
			{
				_variables[name] = mp;
			}
			else
			{
				_variables.Add(name, mp);
			}
			refreshXml();
		}
		[Description("Launch a dialogue box for selecting a value for the input identified by name")]
		public void SelectVariableValue(string name)
		{
			Form f = this.FindForm();
			if (f != null)
			{
				DlgSelectProperty dlg = new DlgSelectProperty();
				dlg.LoadData(f);
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					SetVariable(name, dlg.SelectedProperty);
				}
			}
		}
		#endregion
		#region Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			_root.Draw(e.Graphics);
			if (_autoSize)
			{
				SizeF sf = _root.DrawSize;
				Size sz = new Size((int)sf.Width, (int)sf.Height + 8);
				if (sz.Width != this.Size.Width || sz.Height != this.Size.Height)
				{
					this.Size = sz;
				}
			}
			else
			{
				SizeF size = _root.ExpSize;
				int nh = 0;
				while (size.Width - (nh * this.ClientSize.Width) > this.ClientSize.Width)
				{
					if (this.ClientSize.Height > (nh + 1) * (size.Height + 8) + size.Height)
					{
						GraphicsState gt0 = e.Graphics.Save();
						e.Graphics.TranslateTransform(-(this.ClientSize.Width * (nh + 1)), (nh + 1) * (size.Height + 8));
						_root.Draw(e.Graphics);
						e.Graphics.Restore(gt0);
					}
					else
						break;
					nh++;
				}
			}
		}
		[Browsable(false)]
		public void SetPropertyGrid(PropertyGrid propGrid)
		{
			_propGrid = propGrid;
		}
		[Description("Launch math expression editor for modify the math expression. It returns False if the user cancels the editing.")]
		public bool EditFormula()
		{
			Form f = this.FindForm();
			Rectangle size = this.Bounds;
			Form dlg = _root.CreateEditor(size) as Form;
			if (dlg != null)
			{
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					return true;
				}
			}
			return false;
		}

		[Description("Save math expression to an xml file")]
		public void SaveToXml(string xmlFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(Formula.Xml);
			doc.Save(xmlFile);
		}
		[Description("Load math expression from xml file")]
		public void LoadXml(string xmlFile)
		{
			StreamReader sr = new StreamReader(xmlFile);
			Formula = new FormulaProperty(sr.ReadToEnd());
			sr.Close();

		}
		[Description("Calling this function will cause the math expression being displayed on the specified control starting at the location.")]
		public void AddDrawingSurface(Control surface, Point location)
		{
			if (surface != null)
			{
				if (!surface.IsDisposed)
				{
					foreach (DrawingSurface ds in _surfaces)
					{
						if (ds.Surface == surface)
						{
							ds.Location = location;
							return;
						}
					}
					_surfaces.Add(new DrawingSurface(surface, location, this));
				}
			}
		}
		[Description("Stop displaying the math expression on the specified control")]
		public void RemoveDrawingSurface(Control surface)
		{
			foreach (DrawingSurface ds in _surfaces)
			{
				if (ds.Surface == surface)
				{
					ds.Disable();
					_surfaces.Remove(ds);
					surface.Refresh();
					break;
				}
			}
		}
		[Description("Create math expression image")]
		public Image CreateMathExpressionImage(Graphics g)
		{
			if (_image == null)
			{
				_image = _root.CreateIcon(g);
			}
			return _image;
		}
		[Description("Create math expression editor")]
		public IMathEditor CreateEditor(Rectangle rc)
		{
			return _root.CreateEditor(rc);
		}
		[Description("Compile the math expression")]
		public void Compile()
		{
			try
			{
				_compileResult = _root.CreateMethodCompilerUnit("Test_" + Process.GetCurrentProcess().Id.ToString("x", CultureInfo.InvariantCulture), "TestClass", "TestMethod");
				_compileResult.DebugCompile = false;
				_compileResult.compile();
				_compileOK = (_compileResult.Method != null);
			}
			catch (Exception err)
			{
				_lastError = err.Message;
				_compileOK = false;
			}
		}
		[Description("Display source code for the math expression")]
		public void ShowSourceCode()
		{
			try
			{
				_compileResult = _root.CreateMethodCompilerUnit("Test_" + Process.GetCurrentProcess().Id.ToString("x"), "TestClass", "TestMethod");
				_compileResult.DebugCompile = true;
				_compileResult.compile();
				_compileResult.DebugCompile = false;
			}
			catch (Exception err)
			{
				_lastError = err.Message;
				_compileOK = false;
			}
		}
		[Description("Execute the math expression and return the calculation result")]
		public object Execute()
		{
			if (!_compileOK)
			{
				Compile();
			}
			if (_compileOK)
			{
				object[] ps = getParameterValues(_compileResult.Method);
				_result = _compileResult.Method.Invoke(null, ps);
				return _result;
			}
			return null;
		}
		public override string ToString()
		{
			return _root.ToString();
		}
		#endregion
		#region private methods
		private void findPropertyGrid(ControlCollection cs)
		{
			foreach (Control c in cs)
			{
				PropertyGrid pg = c as PropertyGrid;
				if (pg != null)
				{
					if (pg.SelectedObject == this)
					{
						_propGrid = pg;
						break;
					}
				}
				if (c.Controls.Count > 0)
				{
					findPropertyGrid(c.Controls);
					if (_propGrid != null)
					{
						break;
					}
				}
			}
		}
		private void createInputs()
		{
			VariableList vs = _root.InputVariables;
			if (vs != null && vs.Count > 0)
			{
				StringCollection scLocals = new StringCollection();
				if (_variables == null)
				{
					_variables = new Dictionary<string, MathPropertyPointer>();
					foreach (IVariable v in vs)
					{
						if (!v.IsDummyPort && !v.IsLocal && !v.IsParam && !v.IsReturn)
						{
							if (!_variables.ContainsKey(v.KeyName))
							{
								_variables.Add(v.KeyName, new MathPropertyPointer());
							}
						}
						else
						{
							scLocals.Add(v.KeyName);
						}
					}
				}
				else
				{
					StringCollection sc = new StringCollection();
					foreach (string s in _variables.Keys)
					{
						bool b = false;
						foreach (IVariable v in vs)
						{
							if (string.CompareOrdinal(s, v.KeyName) == 0)
							{
								b = true;
								break;
							}
						}
						if (!b)
						{
							sc.Add(s);
						}
					}
					foreach (string s in sc)
					{
						_variables.Remove(s);
					}
					foreach (IVariable v in vs)
					{
						if (!v.IsDummyPort && !v.IsLocal && !v.IsParam && !v.IsReturn)
						{
							if (!_variables.ContainsKey(v.KeyName))
							{
								_variables.Add(v.KeyName, new MathPropertyPointer());
							}
						}
						else
						{
							scLocals.Add(v.KeyName);
						}
					}
				}
				foreach (string sk in scLocals)
				{
					if (_variables.ContainsKey(sk))
					{
						_variables.Remove(sk);
					}
				}
			}
			else
			{
				_variables = new Dictionary<string, MathPropertyPointer>();
			}
		}
		private object[] getParameterValues(MethodInfo mif)
		{
			if (_variables == null)
			{
				createInputs();
			}
			if (_variables.Count > 0)
			{
				VariableList vars = _root.InputVariables;
				ParameterInfo[] pifs = mif.GetParameters();
				int n = 0;
				if (pifs != null)
				{
					n = pifs.Length;
				}
				if (n != _variables.Count)
				{
					throw new MathException("Parameter count mismatch [{0}] != [{1}]", _variables.Count, n);
				}
				Form f = this.FindForm();
				if (f != null)
				{
					object[] vs = new object[_variables.Count];
					foreach (MathPropertyPointer mpp in _variables.Values)
					{
						mpp.Top.GetInstance(f);
					}
					for (int i = 0; i < vs.Length; i++)
					{
						string sv = vars.GetKeyByCodeVariableName(pifs[i].Name);
						if (_variables.ContainsKey(sv))
						{
							vs[i] = _variables[sv].Bottom.Instance;
							if (vs[i] == null)
							{
								vs[i] = VPL.VPLUtil.GetDefaultValue(pifs[i].ParameterType);
							}
							else
							{
								Type t = vs[i].GetType();
								if (!pifs[i].ParameterType.IsAssignableFrom(t))
								{
									bool bt;
									vs[i] = VPL.VPLUtil.ConvertObject(vs[i], pifs[i].ParameterType, out bt);
									if (!bt)
									{
									}
								}
							}
						}
						else
						{
							throw new MathException("Method parameter [{0}] not found", pifs[i].Name);
						}
					}
					return vs;
				}
			}
			return new object[] { };
		}
		private void refreshXml()
		{
			FormulaProperty pr = new FormulaProperty(createXmlString());
			PropertyDescriptor myProp = TypeDescriptor.GetProperties(this)["Formula"];
			myProp.SetValue(this, pr);
		}
		private string createXmlString()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode rootNode = doc.CreateElement(XML_Root);
			doc.AppendChild(rootNode);
			_root.SetWriter(_writer);
			_root.Save(rootNode);
			if (_variables != null && _variables.Count > 0)
			{
				XmlNode nodeVars = XmlUtil.CreateSingleNewElement(doc.DocumentElement, XML_Variables);
				nodeVars.RemoveAll();
				foreach (KeyValuePair<string, MathPropertyPointer> kv in _variables)
				{
					XmlNode node = nodeVars.OwnerDocument.CreateElement(XML_Item);
					nodeVars.AppendChild(node);
					XmlUtil.SetNameAttribute(node, kv.Key);
					MathPropertyPointer top = kv.Value.Top;
					top.SaveToXml(node);
				}
			}
			return doc.OuterXml;
		}
		#endregion
		#region Properties
		[Description("The evaluation result of the math expression by executing Execute() method")]
		public object Result
		{
			get
			{
				return _result;
			}
		}
		[Editor(typeof(TypeEditorMathXmlString), typeof(UITypeEditor))]
		[Description("Xml string representing this math expression")]
		public string XmlString
		{
			get
			{
				return Formula.Xml;
			}
		}
		[Description("Indicates whether the math expression is compiled and ready to do calculation")]
		public bool Compiled
		{
			get
			{
				return _compileOK;
			}
		}
		[ParenthesizePropertyName(true)]
		[Category("Design")]
		[Editor(typeof(PropEditorMathExp), typeof(UITypeEditor))]
		[Description("The formula for this math expression")]
		public FormulaProperty Formula
		{
			get
			{
				if (_formula == null)
				{
					_formula = new FormulaProperty(createXmlString());
				}
				else
				{
					_formula.Xml = createXmlString();
				}
				return _formula;
			}
			set
			{
				_formula = value;

				_compileOK = false;
				try
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(_formula.Xml);
					if (doc.DocumentElement != null)
					{
						_root.SetReader(_reader);
						_root.Load(doc.DocumentElement);
						_image = null;
						//do not refresh _variables because value may not include variables
						XmlNode nodeVars = doc.DocumentElement.SelectSingleNode(XML_Variables);
						if (nodeVars != null)
						{
							XmlNodeList nodeItems = nodeVars.SelectNodes(XML_Item);
							if (nodeItems != null && nodeItems.Count > 0)
							{
								if (_variables == null)
								{
									_variables = new Dictionary<string, MathPropertyPointer>();
								}
								foreach (XmlNode nd in nodeItems)
								{
									string name = XmlUtil.GetNameAttribute(nd);
									Type t = XmlUtil.GetLibTypeAttribute(nd);
									MathPropertyPointer mpp = (MathPropertyPointer)Activator.CreateInstance(t);
									if (_variables.ContainsKey(name))
									{
										_variables[name] = mpp;
									}
									else
									{
										_variables.Add(name, mpp);
									}
									mpp.LoadFromXml(nd);
								}
							}
						}

						if (AutoSize)
						{
						}
					}
				}
				catch (Exception err)
				{
					_lastError = MathException.FormExceptionText("Load math expression", err);
				}
				Refresh();
				if (_propGrid != null)
				{
					if (_propGrid.SelectedObject != this)
					{
						_propGrid = null;
					}
				}
				if (_propGrid == null)
				{
					Form f = this.FindForm();
					if (f != null)
					{
						findPropertyGrid(f.Controls);
					}
				}
				if (_propGrid != null)
				{
					_propGrid.Refresh();
				}

			}
		}
		[Browsable(false)]
		public bool AutoSize0
		{
			get
			{
				return _autoSize;
			}
			set
			{
				_autoSize = value;
				this.Refresh();
			}
		}
		[Browsable(true)]
		public override bool AutoSize
		{
			get
			{
				return _autoSize;
			}
			set
			{
				_autoSize = value;
				this.Refresh();
			}
		}
		[Description("The number of the input parameters for this math expression")]
		public int VariableCount
		{
			get
			{
				if (_variables == null)
				{
					createInputs();
				}
				return _variables.Count;
			}
		}
		[Description("The names of the the input parameters for this math expression")]
		public string[] VariableNames
		{
			get
			{
				if (_variables == null)
				{
					createInputs();
				}
				string[] ss = new string[_variables.Count];
				_variables.Keys.CopyTo(ss, 0);
				return ss;
			}
		}
		[Description("Error message of the last operation error")]
		public string LastError
		{
			get
			{
				return _lastError;
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		private bool IsBrowsable(Attribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					BrowsableAttribute ba = attributes[i] as BrowsableAttribute;
					if (ba != null)
					{
						return ba.Browsable;
					}
				}
			}
			return false;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (IsBrowsable(attributes))
			{
				createInputs();
				if (_variables.Count > 0)
				{
					List<PropertyDescriptor> list = new List<PropertyDescriptor>();
					foreach (PropertyDescriptor p in ps)
					{
						list.Add(p);
					}
					int n = 0;
					if (attributes != null)
					{
						n = attributes.Length;
					}
					Attribute[] attrs = new Attribute[n + 2];
					if (n > 0)
					{
						attributes.CopyTo(attrs, 0);
					}
					attrs[n] = new EditorAttribute(typeof(PropEditorMathPropertyPointer), typeof(UITypeEditor));
					attrs[n + 1] = new TypeConverterAttribute(typeof(TypeConverterMathPropertyPointer));
					foreach (string s in _variables.Keys)
					{
						PropertyDescriptorPropertyPointer p = new PropertyDescriptorPropertyPointer(s, attrs);
						list.Add(p);
					}
					ps = new PropertyDescriptorCollection(list.ToArray());
				}
			}
			return ps;
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region PropertyDescriptorPropertyPointer
		class PropertyDescriptorPropertyPointer : PropertyDescriptor
		{
			public PropertyDescriptorPropertyPointer(string name, Attribute[] attrs)
				: base(name, attrs)
			{
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(MathematicExpression); }
			}

			public override object GetValue(object component)
			{
				return ((MathematicExpression)component).GetVariable(Name);
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(object);
				}
			}

			public override void ResetValue(object component)
			{
				((MathematicExpression)component).ResetValue(Name);
			}

			public override void SetValue(object component, object value)
			{
				((MathematicExpression)component).SetVariable(Name, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region IXmlCodeReaderWriterHolder Members
		private IXmlCodeWriter _writer;
		private IXmlCodeReader _reader;
		[Browsable(false)]
		[NotForProgramming]
		public IXmlCodeWriter GetWriter()
		{
			return _writer;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetWriter(IXmlCodeWriter writer)
		{
			if (writer != null)
			{
				_writer = writer;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public IXmlCodeReader GetReader()
		{
			return _reader;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetReader(IXmlCodeReader reader)
		{
			if (reader != null)
			{
				_reader = reader;
			}
		}
		#endregion

		#region ISourceValueEnumProvider Members
		[NotForProgramming]
		[Browsable(false)]
		public object[] GetValueEnum(string section, string item)
		{
			if (string.CompareOrdinal(section, "SetVariable") == 0)
			{
				if (string.CompareOrdinal(item, "name") == 0)
				{
					if (_variables == null)
					{
						createInputs();
					}
					string[] vs = new string[_variables.Count];
					_variables.Keys.CopyTo(vs, 0);
					return vs;
				}
			}
			return null;
		}

		#endregion
	}
}
