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
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Drawing.Design;
using System.Reflection;
using ProgElements;
using VPL;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Globalization;

namespace MathExp
{

	public enum EnumIncludeReturnPorts { Inport, OutPort, Both, None }
	/// <summary>
	/// an item in a group
	/// </summary>
	public class MathExpItem
	{
		#region fields and constructors
		private MathExpGroup _mathOwner;
		private IMathExpression _exp;
		private Point _pos;
		private Size _size;
		private object _portSelection;
		private bool _useCompileType;
		private RaisDataType _compileType;
		public MathExpItem(MathExpGroup owner)
		{
			if (owner == null)
				throw new MathException("owner cannot be null when creating MathExpItem");
			_mathOwner = owner;
		}
		#endregion
		#region Methods
		public virtual void Copy(MathExpItem target)
		{
			target.Name = Name;
			target.Description = Description;
			target.Location = Location;
			target.Size = Size;
			if (_exp != null)
			{
				target.MathExpression = (IMathExpression)_exp.Clone();
			}
		}
		public virtual MathExpItem Clone(MathExpGroup owner)
		{
			MathExpItem obj = new MathExpItem(owner);
			Copy(obj);
			obj.MathExpression.ContainerMathItem = obj;
			return obj;
		}
		void setVariableAsLocalById(int id)
		{
			VariableList vs = MathExpression.FindAllInputVariables();
			for (int i = 0; i < vs.Count; i++)
			{
				if (vs[i].ID == id)
				{
					vs[i].IsLocal = true;
				}
			}
		}
		public CodeExpression ReturnCodeExpression(IMethodCompile method)
		{
			IProgramEntity pe = this.MathExpression as IProgramEntity;
			if (pe != null)
			{
				return pe.ReturnCodeExpression(_portSelection, method);
			}
			CodeExpression ce = this.MathExpression.ReturnCodeExpression(method);
			return ce;
		}
		public string ReturnJavaScriptCodeExpression(StringCollection methodCode)
		{
			return CreatejavaScript(methodCode);
		}
		public string CreatejavaScript(StringCollection code)
		{
			IProgramEntity pe = this.MathExpression as IProgramEntity;
			if (pe != null)
			{
				return pe.CreateJavaScript(_portSelection, code);
			}
			return this.MathExpression.CreateJavaScript(code);
		}
		public string CreatePhpScript(StringCollection code)
		{
			IProgramEntity pe = this.MathExpression as IProgramEntity;
			if (pe != null)
			{
				return pe.CreatePhpScript(_portSelection, code);
			}
			return this.MathExpression.CreatePhpScript(code);
		}
		/// <summary>
		/// this method is used for copy/paste
		/// </summary>
		public void ReCreateDefaultPorts()
		{
			PortCollection ports = this.GetAllPorts();
			for (int i = 0; i < ports.Count; i++)
			{
				UInt32 id = ports[i].PortID;
				UInt32 idNew = MathNodeVariable.GetNewID();
				this.MathExpression.ReplaceVariableID(id, idNew);
				ports[i].LinkedPortID = 0;
				((DrawingVariable)ports[i].Label).Variable.ResetID(idNew);
				if (ports[i] is LinkLineNodeInPort)
				{
					ports[i].SetPrevious(new LinkLineNode(ports[i], null));
					((Control)ports[i].PrevNode).Location = ports[i].DefaultNextNodePosition();
				}
				else
				{
					ports[i].SetNext(new LinkLineNode(null, ports[i]));
					((Control)ports[i].NextNode).Location = ports[i].DefaultNextNodePosition();
				}
			}
		}
		/// <summary>
		/// this method is used for copy/paste
		/// </summary>
		public void CreateLinkLines()
		{
			PortCollection ports = this.GetAllPorts();
			ports.CreateLinkLines();
		}
		/// <summary>
		/// this method is used for copy/paste
		/// </summary>
		/// <returns></returns>
		public Control[] GetPortControls()
		{
			PortCollection ports = this.GetAllPorts();
			List<Control> cs = new List<Control>();
			for (int k = 0; k < ports.Count; k++)
			{
				cs.Add(ports[k].Label);
				if (ports[k] is LinkLineNodeInPort)
				{
					LinkLineNode l = ports[k];
					while (l != null)
					{
						cs.Add(l);
						l = (LinkLineNode)l.PrevNode;
					}
				}
				else if (ports[k] is LinkLineNodeOutPort)
				{
					LinkLineNode l = ports[k];
					while (l != null)
					{
						cs.Add(l);
						l = (LinkLineNode)l.NextNode;
					}
				}
			}
			Control[] a = new Control[cs.Count];
			cs.CopyTo(a);
			return a;
		}
		/// <summary>
		/// called before compiling
		/// </summary>
		public void PrepareForCompile(IMethodCompile method)
		{
			MathExpression.PrepareForCompile(method);
		}
		public void PrepareDrawInDiagram()
		{
			if (_exp != null)
			{
				_exp.PrepareDrawInDiagram();
			}
		}
		public void SetDesignServiceProvider(IDesignServiceProvider provider)
		{
			MathNode.RegisterGetGlobalServiceProvider(this, provider);
			if (_exp != null)
			{
				_exp.SetServiceProvider(provider);
			}
		}
		public XmlDocument GetXmlDocument()
		{
			if (_exp != null)
			{
				return _exp.GetXmlDocument();
			}
			return null;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_exp != null)
				{
					return _exp.IsValid;
				}
				return true;
			}
		}
		[Browsable(false)]
		public bool ContainLibraryTypesOnly
		{
			get
			{
				if (_exp != null)
					return _exp.ContainLibraryTypesOnly;
				return true;
			}
		}
		[Browsable(false)]
		public bool UseCompileType
		{
			get
			{
				return _useCompileType;
			}
			set
			{
				_useCompileType = value;
			}
		}
		[Browsable(false)]
		public RaisDataType CompileDataType
		{
			get
			{
				return _compileType;
			}
			set
			{
				_compileType = value;
			}
		}
		[Browsable(false)]
		public RaisDataType ActualCompiledDataType
		{
			get
			{
				if (_exp != null)
					return _exp.ActualCompileDataType;
				return new RaisDataType();
			}
		}
		[Browsable(false)]
		public MathExpGroup Parent
		{
			get
			{
				return _mathOwner;
			}
		}

		public IMathExpression MathExpression
		{
			get
			{
				if (_exp == null)
				{
					_exp = new MathNodeRoot();
					_exp.ContainerMathItem = this;
				}
				return _exp;
			}
			set
			{
				_exp = value;
				_exp.ContainerMathItem = this;
			}
		}
		public MathExpItem GetItemByID(UInt32 id)
		{
			_portSelection = null;
			if (MathExpression.GetVariableByID(id) != null)
			{
				return this;
			}
			IProgramEntity pe = MathExpression as IProgramEntity;
			if (pe != null)
			{
				if (pe.ProgEntity != null)
				{
					ProgramOutPort[] outports = pe.ProgEntity.Outports;
					if (outports != null)
					{
						for (int i = 0; i < outports.Length; i++)
						{
							if (outports[i].PortID == id)
							{
								_portSelection = outports[i];
								return this;
							}
						}
					}
					ProgramInPort[] inports = pe.ProgEntity.Inports;
					if (inports != null)
					{
						for (int i = 0; i < inports.Length; i++)
						{
							if (inports[i].PortID == id)
							{
								_portSelection = inports[i];
								return this;
							}
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// get all input ports and optionally output port.
		/// </summary>
		/// <param name="returnPortType"></param>
		/// <returns></returns>
		public PortCollection GetAllPorts()
		{
			PortCollection ports;
			if (this.MathExpression is MathExpGroup)
			{
				ports = ((MathExpGroup)this.MathExpression).GetAllPorts(true);
			}
			else
			{
				ports = new PortCollection();
			}
			return ports;
		}

		public Point Location
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
			}
		}

		public Size Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.MathExpression != null)
					return this.MathExpression.Name;
				return "";
			}
			set
			{
				if (this.MathExpression != null)
				{
					this.MathExpression.Name = value;
				}
			}
		}
		public string TraceItemName()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			sb.Append(" MathItem for expression ");
			if (MathExpression == null)
			{
				sb.Append("null");
			}
			else
			{
				sb.Append(MathExpression.GetType().Name);
				sb.Append(" name:'");
				sb.Append(MathExpression.Name);
				sb.Append("'");
			}
			return sb.ToString();
		}
		public string TraceInfo
		{
			get
			{
				if (_exp != null)
				{
					return TraceItemName() + " " + _exp.TraceInfo;
				}
				return TraceItemName() + " null expression ";
			}
		}
		public string Description
		{
			get
			{
				if (this.MathExpression != null)
					return this.MathExpression.Description;
				return "";
			}
			set
			{
				if (this.MathExpression != null)
				{
					this.MathExpression.Description = value;
				}
			}
		}
		#endregion
		#region Serialization
		public void SaveToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlNode nd = node.OwnerDocument.CreateElement("Location");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.Location);
			nd = node.OwnerDocument.CreateElement("Size");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, this.Size);
			nd = node.OwnerDocument.CreateElement("MathExpression");
			node.AppendChild(nd);
			XmlSerialization.WriteToXmlNode(serializer, MathExpression, nd);
		}
		public void ReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode("Location");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.Location = (Point)v;
				}
			}
			nd = node.SelectSingleNode("Size");
			if (nd != null)
			{
				object v;
				if (XmlSerialization.ReadValue(nd, out v))
				{
					this.Size = (Size)v;
				}
			}
			nd = node.SelectSingleNode("MathExpression");
			if (nd != null)
			{
				MathExpression = (IMathExpression)XmlSerialization.ReadFromXmlNode(serializer, nd);
			}
		}
		#endregion

	}
	class CRect
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public CRect()
		{
		}
	}
	/// <summary>
	/// several math expressions linked together to form a single math expression.
	/// variables are linked to ports by IDs.
	/// Its _returnIcon's inport is linked to the calculation result of the maxth group.
	/// Its _returnIcon's outport is linked to another MathExpItem when this math group belongs to another math group
	/// </summary>
	public class MathExpGroup : IMathExpression, IMethodPointer, ISourceValuePointersHolder
	{
		#region fields and constructors
		const string XML_PortLocation = "Inports";
		const string XML_Location = "Inport";
		private MathExpItem _container;
		private List<MathExpItem> _expressions;
		private ReturnIcon _returnIcon;
		private ObjectIconData _iconData;
		private string _desc;
		private string _shortDesc;
		private bool _fixed;
		private RaisDataType _compileDataType;
		private RaisDataType _actualCompiledDataType;
		private bool _useCompileDataType;
		private Rectangle _editorBounds;
		private Dictionary<Type, object> _localServices;
		private CodeExpression _compiledCode;
		private string _compiledCodeJS;
		private string _compiledCodePhp;
		private XmlNode _xmlNode;
		//---------------------------------------------------------------------------------
		/// <summary>
		/// input variables of this item is for outside uses and thus not including inter-linked variables (local variables);
		/// GenerateInputVariables() generates this list out of _allInputVariables;
		/// varDummy will be included if needed;
		/// variables in this list is not used when displaying this object in DiagramViewer. the only purpose
		/// of this list is for the parent MathExpGroup to use it to generate _allInputVariables and other variable lists
		/// at the parent level.
		/// </summary>
		private VariableList _inputs;
		//----------------------------------------------------------------------------------
		/// <summary>
		/// the variables in it belong to the lowest level (MathNodeRoot);
		/// it is generated by FindAllInputVariables();
		/// GenerateInputVariables() calls FindAllInputVariables();
		/// it includes all inports from the next level;
		/// </summary>
		private VariableList _allInputVariables;
		//----------------------------------------------------------------------------------
		/// <summary>
		/// it is another version of _allInputVariables, variable by variable;
		/// if a variable is from a MathExpGroup item then the version in this list 
		/// is a duplication of the variable to have the same name and ID but seperated position
		/// and link lines; such variables are saved and re-created in OnReadFromXmlNode();
		/// if a variable is from a MathNodeRoot item then the version in this list
		/// is the same instance as in that in _allInputVariables;
		/// GenerateInportVariables() creates or fills this list;
		/// Inport.LinkedPortID is 0 for all lower levels. when it is not 0 at one level the 
		/// variable disappears at the parent level.
		/// variables in this list completely determine the port positions and link lines
		/// for this object when showing in DiagramViewer.
		/// </summary>
		private VariableList _inportVariables;
		//-----------------------------------------------------------------------------------
		/// <summary>
		/// if there is not an input from any items then this variable is used as the input
		/// for forming execution sequence, not for value passing;
		/// on finishing editing an item, its input variables could be changed. thus both 
		/// _allInputVariables and _inportVariables need to be adjusted accordingly;
		/// editing a MathExpGroup item causes it being re-generated and hence _allInputVariables 
		/// and _inportVariables need to be re-generated so that it may be edited again.
		/// unedited lower level items are fine because the clone of MathExpGroup will correctly
		/// clone _inportVariables.
		/// </summary>
		private MathNodeVariableDummy varDummy;
		//------------------------------------------------------------------------------------
		/// <summary>
		/// the only instance
		/// </summary>
		public MathExpGroup()
		{
			_returnIcon = new ReturnIcon(new RaisDataType(typeof(double)));
			_returnIcon.TypeDefined = false;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public MathNodeVariableDummy Dummy
		{
			get
			{
				return varDummy;
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
		[Browsable(false)]
		public MathExpGroup Root
		{
			get
			{
				if (this.ContainerMathItem == null)
					return this;
				return this.ContainerMathItem.Parent.Root;
			}
		}

		/// <summary>
		/// holding all IMathExoression items.
		/// Currently MathNodeRoot and MathExpGroup implement IMathExoression.
		/// </summary>
		[Browsable(false)]
		public List<MathExpItem> Expressions
		{
			get
			{
				if (_expressions == null)
					_expressions = new List<MathExpItem>();
				return _expressions;
			}
			set
			{
				_expressions = value;
				GenerateInputVariables();
			}
		}
		public int ItemCount
		{
			get
			{
				if (_expressions == null)
					return 0;
				return _expressions.Count;
			}
		}
		/// <summary>
		/// reuturn-icon indicates the IMathExoression whose output is the output 
		/// of the whole group
		/// </summary>
		[Browsable(false)]
		public ReturnIcon ReturnIcon
		{
			get
			{
				return _returnIcon;
			}
			set
			{
				_returnIcon = value;
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
		[Browsable(false)]
		public bool IsContainer
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public string Name
		{
			get
			{
				return Site.Name;
			}
			set
			{
				Site.Name = value;
			}
		}

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
		#endregion
		#region IXmlNodeSerializable Members
		object _serializer;
		public void SetSerializer(object serializer)
		{
			_serializer = serializer;
		}
		[Browsable(false)]
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
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_serializer = serializer;
			_xmlNode = node;
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, this.Name);
			XmlSerialization.SetAttribute(node, MathNodeRoot.XMLATT_FIXED, _fixed);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_EDITORRECT, _editorBounds);
			XmlSerialization.WriteStringValueToChildNode(node, XmlSerialization.XML_DESCRIPT, this.Description);
			XmlSerialization.WriteStringValueToChildNode(node, XmlSerialization.XML_SHORTDESCRIPT, this.ShortDescription);
			//
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
			if (_returnIcon != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_RETURNPORT, _returnIcon);
			}
			if (_expressions != null)
			{
				for (int i = 0; i < _expressions.Count; i++)
				{
					nd = node.OwnerDocument.CreateElement("MathExpItem");
					node.AppendChild(nd);
					_expressions[i].SaveToXmlNode(serializer, nd);
				}
			}
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
				varDummy.Parent.Save(nd);
			}
			nd = node.SelectSingleNode(XML_PortLocation);
			if (nd == null)
			{
				nd = node.OwnerDocument.CreateElement(XML_PortLocation);
				node.AppendChild(nd);
			}
			else
			{
				nd.RemoveAll();
			}
			if (_inportVariables != null && _inportVariables.Count > 0)
			{
				foreach (IVariable v in _inportVariables)
				{
					MathExpItem mi = GetItemByID(v.ID);
					if (mi.MathExpression.IsContainer)
					{
						XmlNode n = XmlSerialization.WriteToChildXmlNode(serializer, nd, XML_Location, v.MathExpression);
						int k = -1;
						if (v == v.MathExpression[0])
							k = 0;
						else if (v == v.MathExpression[1])
							k = 1;
						XmlSerialization.SetAttribute(n, "index", k);
					}
				}
			}
		}
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_serializer = serializer;
			_xmlNode = node;
			this.Name = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
			_fixed = XmlSerialization.GetAttributeBool(node, MathNodeRoot.XMLATT_FIXED);
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
				MathNodeRoot root = new MathNodeRoot();
				root.OnReadFromXmlNode(serializer, nd);
				varDummy = (MathNodeVariableDummy)root[1];
			}
			ReturnIcon ret = (ReturnIcon)XmlSerialization.ReadFromChildXmlNode(serializer, node, XmlSerialization.XML_RETURNPORT);
			if (ret != null)
			{
				_returnIcon = ret;
			}
			_returnIcon.TypeDefined = false;
			_expressions = new List<MathExpItem>();
			XmlNodeList nodes = node.SelectNodes("MathExpItem");
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					MathExpItem item = new MathExpItem(this);
					item.ReadFromXmlNode(serializer, nodes[i]);
					_expressions.Add(item);
				}
			}

			//read inports
			_inportVariables = new VariableList();
			nodes = node.SelectNodes(XmlSerialization.FormatString("{0}/{1}", XML_PortLocation, XML_Location));
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode n in nodes)
				{
					MathNodeRoot r = (MathNodeRoot)XmlSerialization.ReadFromXmlNode(serializer, n);
					int k = XmlSerialization.GetAttributeInt(n, "index");
					IVariable v;
					if (k < 0)
						v = r.Dummy;
					else
						v = (IVariable)r[k];
					_inportVariables.Add(v);
				}
			}
			GenerateInputVariables();
			//create missing ones
			GenerateInportVariables();
			//
			makeLinks();

		}
		/// <summary>
		/// called at the end of reading from file by OnReadFromXmlNode
		/// and at the end of Clone
		/// </summary>
		private void makeLinks()
		{
			PortCollection ports = GetAllPorts(true);
			ports.MakeLinks(null);
			//make the links
			if (_returnIcon.InPort.LinkedPortID != 0 && _returnIcon.InPort.LinkedOutPort == null)
			{
				LinkLineNodePort portOut = ports.GetPortByID(_returnIcon.InPort.LinkedPortID, _returnIcon.InPort.LinkedPortInstanceID);
				if (portOut == null)
				{
					throw new MathException("Return InPort '{0}' is linked to '{1}', but item '{1}' is not found", _returnIcon.InPort.PortID, _returnIcon.InPort.LinkedPortID);
				}
				LinkLineNode start = _returnIcon.InPort.Start;//end point
				LinkLineNode end = portOut.End;
				//portOut.ClearLine();
				end.SetNext(start);
				start.SetPrevious(end);
			}
		}
		#endregion
		#region Mathods
		public CompileResult CreateMethodCompilerUnit(string classNamespace, string className, string methodName)
		{
			MethodType method = new MethodType();
			PrepareForCompile(method);
			MathNode.PrepareMethodCreation();
			MathNode.Trace("Unit Test Code-generation starts at {0} ===Group============", DateTime.Now);
			//
			//create compiling unit
			CodeCompileUnit code = new CodeCompileUnit();
			CodeNamespace ns = new CodeNamespace(classNamespace);
			code.Namespaces.Add(ns);
			//Create a test class to hold the test method
			CodeTypeDeclaration t = new CodeTypeDeclaration(className);
			ns.Types.Add(t);
			//create the test method
			CodeMemberMethod m = new CodeMemberMethod();
			t.Members.Add(m);
			m.Name = methodName;
			m.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			//
			AssemblyRefList imports = new AssemblyRefList();
			VariableList parameters = new VariableList();
			List<IPropertyPointer> pointerList = new List<IPropertyPointer>();
			GetAllImports(imports);
			//
			m.ReturnType = new CodeTypeReference(DataType.Type);
			//
			m.Comments.Add(new CodeCommentStatement("Variable mapping:"));
			m.Comments.Add(new CodeCommentStatement("In formula:  method parameter"));
			//
			MethodType mt = new MethodType();
			mt.MethodCode = m;
			GenerateInputVariables();
			VariableList variables = InputVariables;
			Dictionary<string, IPropertyPointer> pointers = new Dictionary<string, IPropertyPointer>();
			GetPointers(pointers);
			int n = variables.Count;
			MathNode.Trace("Generate arguments from {0} input variables", n);
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
					//
					AssignCodeExp(new CodeArgumentReferenceExpression(paramName), var.CodeVariableName);
				}
			}
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
			//do the compiling
			CodeExpression ce = ReturnCodeExpression(mt);
			//
			MathNode.Trace("Test method returns {0}, compiled type: {1}", DataType.Type, ActualCompileDataType.Type);
			if (ActualCompileDataType.Type.Equals(DataType.Type))
			{
				CodeMethodReturnStatement mr = new CodeMethodReturnStatement(ce);
				m.Statements.Add(mr);
			}
			else
			{
				if (ActualCompileDataType.IsVoid)
				{
					m.Statements.Add(new CodeExpressionStatement(ce));
					CodeMethodReturnStatement mr = new CodeMethodReturnStatement(ValueTypeUtil.GetDefaultValueByType(DataType.Type));
					m.Statements.Add(mr);
				}
				else
				{
					if (DataType.IsVoid)
					{
						m.Statements.Add(new CodeExpressionStatement(ce));
					}
					else
					{
						if (DataType.Type.Equals(typeof(string)))
						{
							CodeMethodReturnStatement mr = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(ce, "ToString", new CodeExpression[] { }));
							m.Statements.Add(mr);
						}
						else
						{
							CodeExpression mie = RaisDataType.GetConversionCode(ActualCompileDataType, ce, DataType, m.Statements);
							if (mie != null)
							{
								CodeMethodReturnStatement mr = new CodeMethodReturnStatement(mie);
								m.Statements.Add(mr);
							}
						}
					}
				}
			}
			//
			MathNode.Trace("Imports:{0}", imports.Count);
			MathNode.IndentIncrement();
			foreach (AssemblyRef sA in imports)
			{
				MathNode.Trace(sA.Name);
				ns.Imports.Add(new CodeNamespaceImport(sA.Name));
			}
			MathNode.IndentDecrement();
			MathNode.Trace("Unit Test Code-generation ends at {0} ===============", DateTime.Now);
			//
			return new CompileResult(classNamespace, className, methodName, code, pointerList, parameters, imports);
		}
		public void SetInportVariables(VariableList vs)
		{
			_inportVariables = vs;
		}
		public void SetInputVariables(VariableList vs)
		{
			_inputs = vs;
		}
		public void SetDummyVar(MathNodeVariableDummy v)
		{
			varDummy = v;
		}
		/// <summary>
		/// get all variables and assign their Scopes
		/// </summary>
		public void GenerateInputVariables()
		{
			VariableList inputs = new VariableList();
			VariableList vs = this.FindAllInputVariables();
			//find out which variables are local ones
			//same name does not mean the same variable
			foreach (IVariable v in vs)
			{
				bool isLocal = false;
				if (v.InPort != null && v.InPort.LinkedPortID != 0)
				{
					if (GetVariableByID(v.InPort.LinkedPortID) != null)
					{
						//linked within this group, not outside
						isLocal = true;
					}
				}
				if (!isLocal)
				{
					if (_inportVariables != null)
					{
						IVariable vi = _inportVariables.GetVariableById(v.ID);
						if (vi != null)
						{
							if (vi.InPort == null)
							{
								vi.InPort = new LinkLineNodeInPort(vi);
							}
							if (GetVariableByID(vi.InPort.LinkedPortID) != null)
							{
								//linked within this group, not outside
								isLocal = true;
							}
						}
					}
				}
				if (isLocal)
				{
					v.Scope = this;
				}
				else
				{
					if (v.Scope == this)
					{
						v.Scope = null;
					}
					if (!v.IsDummyPort && !v.IsConst)
					{
						inputs.Add(v);
					}
				}
			}
			if (inputs.Count == 0)
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
					MathNodeRoot r = new MathNodeRoot();
					varDummy = new MathNodeVariableDummy(r);
					r[1] = varDummy;
				}
				inputs.Add(varDummy);
			}
			_inputs = inputs;
		}
		public MathExpItem GetOutputItem()
		{
			if (this.ReturnIcon.InPort.LinkedPortID != 0)
			{
				return GetItemByID(this.ReturnIcon.InPort.LinkedPortID);
			}
			if (ItemCount > 0)
			{
				//find one end item
				for (int i = 0; i < _expressions.Count; i++)
				{
					List<MathExpItem> list = new List<MathExpItem>();
					MathExpItem item = _expressions[0];
					while (!list.Contains(item))
					{
						list.Add(item);
						if (item.MathExpression.OutputVariable.OutPorts == null || item.MathExpression.OutputVariable.OutPorts.Length == 0)
							return item;
						if (item.MathExpression.OutputVariable.OutPorts.Length == 1 && item.MathExpression.OutputVariable.OutPorts[0].LinkedPortID == 0)
							return item;
						for (int j = 0; j < item.MathExpression.OutputVariable.OutPorts.Length; j++)
						{
							item = GetItemByID(item.MathExpression.OutputVariable.OutPorts[j].LinkedPortID);
							if (!list.Contains(item))
							{
								list.Add(item);
								if (item.MathExpression.OutputVariable.OutPorts == null || item.MathExpression.OutputVariable.OutPorts.Length == 0)
									return item;
								if (item.MathExpression.OutputVariable.OutPorts.Length == 1 && item.MathExpression.OutputVariable.OutPorts[0].LinkedPortID == 0)
									return item;
							}
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// create inport variables
		/// </summary>
		public void GenerateInportVariables()
		{
			if (_inportVariables == null)
			{
				_inportVariables = new VariableList();
			}
			VariableList vList = AllInputVariables;
			foreach (IVariable v in vList)
			{
				IVariable vi = _inportVariables.GetVariableById(v.ID);
				if (vi == null)
				{
					MathExpItem mi = GetItemByID(v.ID);
					if (!mi.MathExpression.IsContainer)
					{
						_inportVariables.Add(v);
					}
					else
					{
						MathNodeRoot r = new MathNodeRoot();
						r[1] = (MathNode)Activator.CreateInstance(v.GetType(), r);
						vi = (IVariable)r[1];
						vi.VariableType = (RaisDataType)v.VariableType.Clone();
						vi.VariableName = v.VariableName;
						vi.SubscriptName = v.SubscriptName;
						vi.ResetID(v.ID);
						_inportVariables.Add(vi);
						vi.InPort = new LinkLineNodeInPort(vi);
					}
				}
			}
		}
		public LinkLineNodePort GetPortByID(UInt32 id, UInt32 index)
		{
			PortCollection ports = GetAllPorts(true);
			return ports.GetPortByID(id, index);
		}
		public PortCollection GetAllPorts(bool includeReturnIcon)
		{
			PortCollection ports = new PortCollection();
			if (_inportVariables == null)
			{
				throw new MathException("error getting ports: port list is null");
			}
			foreach (IVariable v in _inportVariables)
			{
				if (v.InPort == null)
				{
					v.InPort = new LinkLineNodeInPort(v);
				}
				ports.Add(v.InPort);
			}
			if (includeReturnIcon)
			{
				if (_returnIcon == null)
				{
					_returnIcon = new ReturnIcon(new RaisDataType(typeof(double)));
					ports.Add(_returnIcon.InPort);
				}
				else
				{
					if (ReturnIcon.InPort == null)
					{
						ReturnIcon.ReturnVariable.InPort = new LinkLineNodeInPort(ReturnIcon.ReturnVariable);
					}
				}
				ports.Add(ReturnIcon.InPort);
			}
			if (_expressions != null)
			{
				for (int i = 0; i < _expressions.Count; i++)
				{
					if (_expressions[i].MathExpression.OutputVariable.OutPorts != null)
					{
						ports.AddRange(_expressions[i].MathExpression.OutputVariable.OutPorts);
					}
					IProgramEntity pe = _expressions[i].MathExpression as IProgramEntity;
					if (pe != null && pe.ProgEntity != null)
					{
						if (pe.ProgEntity.Outports != null)
						{
							ports.AddRange(pe.ProgEntity.Outports);
						}
						if (pe.ProgEntity.Inports != null)
						{
							ports.AddRange(pe.ProgEntity.Inports);
						}
					}
				}
			}
			return ports;
		}
		/// <summary>
		/// put all controls in one array to be added to the parent.
		/// it also make the line links. But lines are not created yet.
		/// CreateLinkLines() should be called after adding all controls to the parent.
		/// </summary>
		/// <returns></returns>
		public Control[] GetAllLinkNodes(bool includeReturnIcon)
		{
			PortCollection ports = GetAllPorts(includeReturnIcon);
			List<Control> cs = ports.GetAllControls(true);
			if (includeReturnIcon)
			{
				//add return icon
				//make the links
				LinkLineNodePort port = ports.GetPortByID(_returnIcon.InPort.LinkedPortID, _returnIcon.InPort.LinkedPortInstanceID);
				if (port != null)
				{
					LinkLineNode start = _returnIcon.InPort.Start;
					if (!(start is LinkLineNodePort))
					{
						((Control)start).Visible = false;
					}
				}
				LinkLineNode l = (LinkLineNode)_returnIcon.InPort.PrevNode; //_returnIcon.InPort already included in ports
				while (l != null && !(l is LinkLineNodeOutPort))
				{
					cs.Add(l);
					l = (LinkLineNode)l.PrevNode;
				}
				cs.Add(_returnIcon);
			}
			Control[] a = new Control[cs.Count];
			cs.CopyTo(a);
			return a;
		}
		/// <summary>
		/// all controls should be added to the parent when this function is called.
		/// </summary>
		public void CreateLinkLines()
		{
			PortCollection ports = GetAllPorts(true);
			ports.CreateLinkLines();
			if (_returnIcon != null)
			{
				LinkLineNode l = _returnIcon.InPort;
				_returnIcon.InPort.Label.AdjustPosition();
				_returnIcon.InPort.Location = _returnIcon.InPort.DefaultNextNodePosition();
				while (l != null)
				{
					l.CreateBackwardLine();
					bool b = (l is LinkLineNodePort);
					((Control)l).Visible = b;
					l = (LinkLineNode)l.PrevNode;
				}
			}
			_returnIcon.InPort.adjustPosition();
		}
		public void SetServiceProvider(IDesignServiceProvider designServiceProvider)
		{
			MathNode.RegisterGetGlobalServiceProvider(this, designServiceProvider);
			if (_expressions != null)
			{
				for (int i = 0; i < _expressions.Count; i++)
				{
					_expressions[i].SetDesignServiceProvider(designServiceProvider);
				}
			}
		}
		#endregion
		#region IMathExoression Members
		private MethodInfo _methodInfo;
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
				//TBD: generate method
				_methodInfo = null;
			}
			return _methodInfo;
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_expressions != null)
				{
					foreach (MathExpItem mi in _expressions)
					{
						if (!mi.IsValid)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		public void SetParameterReferences(Parameter[] ps)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.SetParameterReferences(ps);
				}
			}
		}
		public void SetParameterMethodID(UInt32 methodID)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.SetParameterMethodID(methodID);
				}
			}
		}
		public void AssignCodeExp(CodeExpression code, string codeVarName)
		{
			VariableList vars = InputVariables;
			foreach (IVariable v in vars)
			{
				if (v.CodeVariableName == codeVarName)
				{
					v.AssignCode(code);
				}
			}
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.AssignCodeExp(code, codeVarName);
				}
			}
		}
		public void AssignJavaScriptCodeExp(string code, string codeVarName)
		{
			VariableList vars = InputVariables;
			foreach (IVariable v in vars)
			{
				if (v.CodeVariableName == codeVarName)
				{
					v.AssignJavaScriptCode(code);
				}
			}
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.AssignJavaScriptCodeExp(code, codeVarName);
				}
			}
		}
		public void AssignPhpScriptCodeExp(string code, string codeVarName)
		{
			VariableList vars = InputVariables;
			foreach (IVariable v in vars)
			{
				if (v.CodeVariableName == codeVarName)
				{
					v.AssignPhpScriptCode(code);
				}
			}
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.AssignPhpScriptCodeExp(code, codeVarName);
				}
			}
		}
		public string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(this.Name);
				sb.Append(" item count:");
				sb.Append(_expressions.Count.ToString());
				return sb.ToString();
			}
		}
		private bool _eu;
		[Browsable(false)]
		public bool EnableUndo { get { return _eu; } set { _eu = value; } }
		[Browsable(false)]
		public UInt32 ID
		{
			get
			{
				return ReturnIcon.ReturnVariable.ID;
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
		public RaisDataType DataType
		{
			get
			{
				if (_returnIcon.ReturnVariable.InPort != null)
				{
					if (_returnIcon.ReturnVariable.InPort.LinkedPortID != 0)
					{
						MathExpItem item = GetItemByID(_returnIcon.ReturnVariable.InPort.LinkedPortID);
						if (item != null && item.MathExpression != null)
						{
							if (item.MathExpression == this)
							{
								throw new MathException("Error searching for item: return port is linked to itself");
							}
							return item.MathExpression.DataType;
						}
					}
				}
				return _returnIcon.ReturnVariable.VariableType;
			}
		}

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
		public bool ContainLibraryTypesOnly
		{
			get
			{
				if (_expressions != null)
				{
					foreach (MathExpItem item in _expressions)
					{
						if (!item.ContainLibraryTypesOnly)
							return false;
					}
				}
				return true;
			}
		}
		public void SetDataType(RaisDataType type)
		{
			if (_returnIcon.ReturnVariable.InPort != null)
			{
				if (_returnIcon.ReturnVariable.InPort.LinkedPortID != 0)
				{
					MathExpItem item = GetItemByID(_returnIcon.ReturnVariable.InPort.LinkedPortID);
					if (item != null && item.MathExpression != null)
					{
						if (item.MathExpression == this)
						{
							throw new MathException("Error searching for item: return port is linked to itself");
						}
						item.MathExpression.SetDataType(type);
						return;
					}
				}
			}
			_returnIcon.ReturnVariable.VariableType = type;
		}
		public void GetAllImports(AssemblyRefList imports)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.GetAllImports(imports);
				}
			}
			Type t = DataType.Type;
			imports.AddRef(t.Assembly.GetName().Name, t.Assembly.Location);
		}
		/// <summary>
		/// find all input variables of all immediate child items
		/// </summary>
		/// <returns></returns>
		public VariableList FindAllInputVariables()
		{
			_allInputVariables = new VariableList();
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					VariableList vs = item.MathExpression.InputVariables;
					if (vs != null)
					{
						_allInputVariables.AddRange(vs);
					}
				}
			}
			return _allInputVariables;
		}
		public VariableList DummyInputs(bool create)
		{
			return null;
		}
		/// <summary>
		/// called beforfe compiling
		/// </summary>
		public void PrepareForCompile(IMethodCompile method)
		{
			_compiledCode = null;
			VariableList vars = InputVariables;
			foreach (IVariable v in vars)
			{
				v.AssignCode(null);
			}
			vars = InportVariables;
			foreach (IVariable v in vars)
			{
				v.AssignCode(null);
			}
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.PrepareForCompile(method);
				}
			}
		}
		/// <summary>
		/// PrepareVariables(...) should be called already
		/// </summary>
		/// <param name="supprtStatements"></param>
		/// <returns></returns>
		public CodeExpression ReturnCodeExpression(IMethodCompile method)
		{
			MathNode.Trace("MathExpGroup.ReturnCodeExpression: {0}, item count:{1}", this.Name, _expressions.Count);
			if (_compiledCode != null)
			{
				MathNode.Trace("Already compiled");
				return _compiledCode;
			}
			//find the item doing the return. getting its code will generate code of other items
			MathExpItem outPutItem = GetOutputItem();
			if (outPutItem == null)
			{
				MathNode.Trace("No math expressions in the group");
				if (UseCompileDataType)
				{
					MathNode.Trace("Use Default by UseCompileDataType: {0}", CompileDataType.Type);
					_actualCompiledDataType = CompileDataType;
					return ValueTypeUtil.GetDefaultValueByType(CompileDataType.Type);
				}
				else
				{
					MathNode.Trace("Use Default by DataType: {0}", DataType.Type);
					_actualCompiledDataType = DataType;
					return ValueTypeUtil.GetDefaultValueByType(DataType.Type);
				}
			}
			//prepare code for linked variables
			VariableList allinputVars = this.AllInputVariables;
			VariableList vars = InportVariables;
			foreach (IVariable v in vars)
			{
				if (v.InPort.LinkedPortID != 0)
				{
					MathExpItem item = GetItemByID(v.InPort.LinkedPortID);
					if (item == null)
					{
						throw new MathException("{0} has an inport {1} linked to {2}. But item {2} is not found", this.Name, v.ID, v.InPort.LinkedPortID);
					}
					CodeExpression code = item.ReturnCodeExpression(method);
					if (v.IsDummyPort)
					{
						method.MethodCode.Statements.Add(new CodeExpressionStatement(code));
					}
					else
					{
						IVariable vi = allinputVars.GetVariableById(v.ID);
						this.AssignCodeExp(code, vi.CodeVariableName);
					}
				}
			}
			//
			MathNode.Trace("Return port is linked to item {0}", outPutItem.TraceInfo);
			outPutItem.MathExpression.UseCompileDataType = UseCompileDataType;
			outPutItem.MathExpression.CompileDataType = CompileDataType;
			outPutItem.UseCompileType = UseCompileDataType;
			outPutItem.CompileDataType = CompileDataType;
			CodeExpression ce = outPutItem.ReturnCodeExpression(method);
			_actualCompiledDataType = outPutItem.ActualCompiledDataType;
			if (UseCompileDataType)
			{
				MathNode.Trace("Item returns the value: {0}. Compilation return type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			else
			{
				MathNode.Trace("Item returns the value: {0}. Data type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			_compiledCode = ce;
			return ce;
		}
		public string ReturnJavaScriptCodeExpression(StringCollection methodCode)
		{
			return CreateJavaScript(methodCode);
		}
		public string CreateJavaScript(StringCollection code)
		{
			MathNode.Trace("MathExpGroup.CreateJavaScript: {0}, item count:{1}", this.Name, _expressions.Count);
			if (!string.IsNullOrEmpty(_compiledCodeJS))
			{
				MathNode.Trace("Already compiled");
				return _compiledCodeJS;
			}
			//find the item doing the return. getting its code will generate code of other items
			MathExpItem outPutItem = GetOutputItem();
			if (outPutItem == null)
			{
				MathNode.Trace("No math expressions in the group");
				if (UseCompileDataType)
				{
					MathNode.Trace("Use Default by UseCompileDataType: {0}", CompileDataType.Type);
					_actualCompiledDataType = CompileDataType;
					return ValueTypeUtil.GetDefaultJavaScriptValueByType(CompileDataType.Type);
				}
				else
				{
					MathNode.Trace("Use Default by DataType: {0}", DataType.Type);
					_actualCompiledDataType = DataType;
					return ValueTypeUtil.GetDefaultJavaScriptValueByType(DataType.Type);
				}
			}
			//prepare code for linked variables
			VariableList allinputVars = this.AllInputVariables;
			VariableList vars = InportVariables;
			foreach (IVariable v in vars)
			{
				if (v.InPort.LinkedPortID != 0)
				{
					MathExpItem item = GetItemByID(v.InPort.LinkedPortID);
					if (item == null)
					{
						throw new MathException("{0} has an inport {1} linked to {2}. But item {2} is not found", this.Name, v.ID, v.InPort.LinkedPortID);
					}
					string code0 = item.CreatejavaScript(code);
					if (v.IsDummyPort)
					{
						code.Add(code0);
					}
					else
					{
						IVariable vi = allinputVars.GetVariableById(v.ID);
						this.AssignJavaScriptCodeExp(code0, vi.CodeVariableName);
					}
				}
			}
			//
			MathNode.Trace("Return port is linked to item {0}", outPutItem.TraceInfo);
			outPutItem.MathExpression.UseCompileDataType = UseCompileDataType;
			outPutItem.MathExpression.CompileDataType = CompileDataType;
			outPutItem.UseCompileType = UseCompileDataType;
			outPutItem.CompileDataType = CompileDataType;
			string ce = outPutItem.CreatejavaScript(code);
			_actualCompiledDataType = outPutItem.ActualCompiledDataType;
			if (UseCompileDataType)
			{
				MathNode.Trace("Item returns the value: {0}. Compilation return type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			else
			{
				MathNode.Trace("Item returns the value: {0}. Data type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			_compiledCodeJS = ce;
			return ce;
		}
		public string ReturnPhpScriptCodeExpression(StringCollection methodCode)
		{
			return CreatePhpScript(methodCode);
		}
		public string CreatePhpScript(StringCollection code)
		{
			MathNode.Trace("MathExpGroup.CreatePhpScript: {0}, item count:{1}", this.Name, _expressions.Count);
			if (!string.IsNullOrEmpty(_compiledCodePhp))
			{
				MathNode.Trace("Already compiled");
				return _compiledCodePhp;
			}
			//find the item doing the return. getting its code will generate code of other items
			MathExpItem outPutItem = GetOutputItem();
			if (outPutItem == null)
			{
				MathNode.Trace("No math expressions in the group");
				if (UseCompileDataType)
				{
					MathNode.Trace("Use Default by UseCompileDataType: {0}", CompileDataType.Type);
					_actualCompiledDataType = CompileDataType;
					return ValueTypeUtil.GetDefaultPhpScriptValueByType(CompileDataType.Type);
				}
				else
				{
					MathNode.Trace("Use Default by DataType: {0}", DataType.Type);
					_actualCompiledDataType = DataType;
					return ValueTypeUtil.GetDefaultPhpScriptValueByType(DataType.Type);
				}
			}
			//prepare code for linked variables
			VariableList allinputVars = this.AllInputVariables;
			VariableList vars = InportVariables;
			foreach (IVariable v in vars)
			{
				if (v.InPort.LinkedPortID != 0)
				{
					MathExpItem item = GetItemByID(v.InPort.LinkedPortID);
					if (item == null)
					{
						throw new MathException("{0} has an inport {1} linked to {2}. But item {2} is not found", this.Name, v.ID, v.InPort.LinkedPortID);
					}
					string code0 = item.CreatePhpScript(code);
					if (v.IsDummyPort)
					{
						code.Add(code0);
					}
					else
					{
						IVariable vi = allinputVars.GetVariableById(v.ID);
						this.AssignPhpScriptCodeExp(code0, vi.CodeVariableName);
					}
				}
			}
			//
			MathNode.Trace("Return port is linked to item {0}", outPutItem.TraceInfo);
			outPutItem.MathExpression.UseCompileDataType = UseCompileDataType;
			outPutItem.MathExpression.CompileDataType = CompileDataType;
			outPutItem.UseCompileType = UseCompileDataType;
			outPutItem.CompileDataType = CompileDataType;
			string ce = outPutItem.CreatePhpScript(code);
			_actualCompiledDataType = outPutItem.ActualCompiledDataType;
			if (UseCompileDataType)
			{
				MathNode.Trace("Item returns the value: {0}. Compilation return type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			else
			{
				MathNode.Trace("Item returns the value: {0}. Data type: {1}", outPutItem.Name, outPutItem.ActualCompiledDataType);
			}
			_compiledCodePhp = ce;
			return ce;
		}
		[Browsable(false)]
		public RaisDataType ActualCompileDataType
		{
			get
			{
				return _actualCompiledDataType;
			}
		}
		public IVariable GetVariableByID(UInt32 id)
		{
			if (_returnIcon.ReturnVariable.ID == id)
				return _returnIcon.ReturnVariable;
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					IVariable v = item.MathExpression.GetVariableByID(id);
					if (v != null)
						return v;
				}
			}
			return null;
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
		public IVariable GetInputVariableByID(int id)
		{
			if (_allInputVariables == null)
			{
				throw new MathException("Calling GetInputVariableByID while _allInputVariables is null");
			}
			foreach (IVariable v in _allInputVariables)
			{
				if (v.ID == id)
				{
					return v;
				}
			}
			return null;
		}
		public MathExpItem GetItemByID(UInt32 id)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					MathExpItem ret = item.GetItemByID(id);
					if (ret != null)
					{
						return ret;
					}
				}
			}
			return null;
		}
		public MathExpItem RemoveItemByID(UInt32 portId)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					MathExpItem ret = item.GetItemByID(portId);
					if (ret != null)
					{
						_expressions.Remove(item);
						return ret;
					}
				}
			}
			return null;
		}
		public void GetPointers(Dictionary<string, IPropertyPointer> pointers)
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.GetPointers(pointers);
				}
			}
		}
		[Browsable(false)]
		public IVariable OutputVariable
		{
			get
			{
				if (_returnIcon != null)
				{
					return _returnIcon.ReturnVariable;
				}
				return null;
			}
		}
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
		public VariableList AllInputVariables
		{
			get
			{
				if (_allInputVariables == null)
				{
					GenerateInputVariables();
				}
				return _allInputVariables;
			}
		}
		[Browsable(false)]
		public VariableList InportVariables
		{
			get
			{
				if (_inportVariables == null)
				{
					GenerateInportVariables();
				}
				return _inportVariables;
			}
		}
		public Bitmap CreateIcon(Graphics g)
		{
			Bitmap img = null;
			string text;
			ObjectIconData icon = this.IconImage;
			switch (icon.IconType)
			{
				case EnumIconType.IconImage:
					img = (Bitmap)icon.IconImage;
					if (img == null)
						goto LB_ShortText;
					break;
				case EnumIconType.ItemImage:
					if (_expressions != null)
					{
						int x = int.MaxValue;
						int y = int.MaxValue;
						int right = 0;
						int bottom = 0;
						Bitmap[] imgs = new Bitmap[_expressions.Count];
						CRect[] rcs = new CRect[_expressions.Count];
						for (int i = 0; i < _expressions.Count; i++)
						{
							rcs[i] = new CRect();
							rcs[i].X = _expressions[i].Location.X;
							rcs[i].Y = _expressions[i].Location.Y;
							rcs[i].Width = _expressions[i].Size.Width;
							rcs[i].Height = _expressions[i].Size.Height;
							//
							imgs[i] = _expressions[i].MathExpression.CreateIcon(g);
							//
						}
						//compact the images
						List<CRect> xs = new List<CRect>();
						List<CRect> ys = new List<CRect>();
						for (int i = 0; i < rcs.Length; i++)
						{
							bool b = true;
							for (int k = 0; k < xs.Count; k++)
							{
								if (rcs[i].X < xs[k].X)
								{
									xs.Insert(k, rcs[i]);
									b = false;
									break;
								}
							}
							if (b)
							{
								xs.Add(rcs[i]);
							}
							b = true;
							for (int k = 0; k < ys.Count; k++)
							{
								if (rcs[i].Y < ys[k].Y)
								{
									ys.Insert(k, rcs[i]);
									b = false;
									break;
								}
							}
							if (b)
							{
								ys.Add(rcs[i]);
							}
						}
						for (int k = 1; k < xs.Count; k++)
						{
							if (xs[k].X > xs[k - 1].X + xs[k - 1].Width)
							{
								if (xs[k].Y >= xs[k - 1].Y + xs[k - 1].Height)
									xs[k].X = xs[k - 1].X;
								else
									xs[k].X = xs[k - 1].X + xs[k - 1].Width;
							}
						}
						for (int k = 1; k < ys.Count; k++)
						{
							if (ys[k].Y > ys[k - 1].Y + ys[k - 1].Height)
							{
								if (ys[k].X >= ys[k - 1].X + ys[k - 1].Width)
									ys[k].Y = ys[k - 1].Y;
								else
									ys[k].Y = ys[k - 1].Y + ys[k - 1].Height;
							}
						}
						for (int i = 0; i < rcs.Length; i++)
						{
							if (rcs[i].X < x)
								x = rcs[i].X;
							if (rcs[i].Y < y)
								y = rcs[i].Y;
							if (rcs[i].X + rcs[i].Width > right)
								right = rcs[i].X + rcs[i].Width;
							if (rcs[i].Y + rcs[i].Height > bottom)
								bottom = rcs[i].Y + rcs[i].Height;
						}
						int bmpWidth = right - x;
						int bmpHeight = bottom - y;
						if (bmpWidth > 0 && bmpHeight > 0)
						{
							img = new Bitmap(bmpWidth, bmpHeight, g);
							Graphics gm = Graphics.FromImage(img);
							for (int i = 0; i < _expressions.Count; i++)
							{
								if (imgs[i] != null)
								{
									gm.DrawImage(imgs[i],
										new Rectangle(rcs[i].X - x, rcs[i].Y - y,
										rcs[i].Width, rcs[i].Height),
										new Rectangle(0, 0, imgs[i].Width, imgs[i].Height), GraphicsUnit.Pixel);
								}
							}
							gm.Dispose();
						}
					}
					if (img == null)
					{
						goto LB_ShortText;
					}
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
					SizeF size = g.MeasureString(text, icon.TextAttributes.Font);
					if (size.Width == 0)
						size.Width = 20;
					if (size.Height == 0)
						size.Height = 20;
					img = new Bitmap((int)size.Width, (int)size.Height, g);
					Graphics gImg = Graphics.FromImage(img);
					gImg.FillRectangle(new SolidBrush(icon.TextAttributes.BackColor), 0, 0, size.Width, size.Height);
					gImg.DrawString(text, icon.TextAttributes.Font, new SolidBrush(icon.TextAttributes.TextColor), (float)0, (float)0);
					gImg.Dispose();
					break;
			}
			return img;
		}
		public IMathEditor CreateEditor(Rectangle rcStart)
		{
			dlgMathExpGroupEditor dlg = new dlgMathExpGroupEditor(rcStart);
			//this assignment loads the group by the host
			dlg.MathExpression = (MathExpGroup)this.Clone();
			if (EitorBounds.Width > 0 && EitorBounds.Height > 0)
			{
				dlg.Location = new Point(EitorBounds.X, EitorBounds.Y);
				dlg.Size = new Size(EitorBounds.Width, EitorBounds.Height);
			}
			return dlg;
		}
		public void ClearFocus()
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.ClearFocus();
				}
			}
		}
		public void ReplaceVariableID(UInt32 currentID, UInt32 newID)
		{
			PortCollection ports = GetAllPorts(true);
			foreach (LinkLineNodePort port in ports)
			{
				if (port.PortID == currentID)
				{
					port.Variable.ResetID(newID);
				}
				if (port.LinkedPortID == currentID)
				{
					port.LinkedPortID = newID;
				}
			}
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.MathExpression.ReplaceVariableID(currentID, newID);
					ports = item.GetAllPorts();
					foreach (LinkLineNodePort port in ports)
					{
						if (port.PortID == currentID)
						{
							port.Variable.ResetID(newID);
						}
						if (port.LinkedPortID == currentID)
						{
							port.LinkedPortID = newID;
						}
					}
				}
			}
			if (ReturnIcon.ReturnVariable.ID == currentID)
			{
				ReturnIcon.ReturnVariable.ResetID(newID);
			}
			if (ReturnIcon.InPort.LinkedPortID == currentID)
			{
				ReturnIcon.InPort.LinkedPortID = newID;
			}
		}
		public void PrepareDrawInDiagram()
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					item.PrepareDrawInDiagram();
				}
			}
		}
		public void AddItem(IMathExpression item)
		{
			MathExpItem exp = new MathExpItem(this);
			exp.MathExpression = item;
			Expressions.Add(exp);
			GenerateInputVariables();
		}
		/// <summary>
		/// find the XML document of the RAIS schema
		/// </summary>
		/// <returns></returns>
		public XmlDocument GetXmlDocument()
		{
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					XmlDocument doc = item.GetXmlDocument();
					if (doc != null)
					{
						return doc;
					}
				}
			}
			return null;
		}
		private IActionContext _ac;
		private IMethod _sm;
		private Type _vtt;
		/// <summary>
		/// when it is used within an action definition, this is the action.
		/// It is needed before loading from XML if variable map is used.
		/// It is needed for creating other objects in action-context, such as ParameterValue
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionContext ActionContext { get { return _ac; } set { _ac = value; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod ScopeMethod { get { return _sm; } set { _sm = value; } }
		/// <summary>
		/// the type for the mapped instance.
		/// currently it is ParameterValue.
		/// It is needed before loading from XML if variable map is used
		/// </summary>
		public Type VariableMapTargetType { get { return _vtt; } set { _vtt = value; } }
		public IList<ISourceValuePointer> GetValueSources()
		{
			List<ISourceValuePointer> list = new List<ISourceValuePointer>();
			if (_expressions != null)
			{
				foreach (MathExpItem m in _expressions)
				{
					if (m.MathExpression != null)
					{
						IList<ISourceValuePointer> l = m.MathExpression.GetValueSources();
						list.AddRange(l);
					}
				}
			}
			return list;
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			List<ISourceValuePointer> l = new List<ISourceValuePointer>();
			if (_expressions != null)
			{
				foreach (MathExpItem m in _expressions)
				{
					if (m.MathExpression != null)
					{
						IList<ISourceValuePointer> ls = m.MathExpression.GetUploadProperties(taskId);
						if (ls != null && ls.Count > 0)
						{
							foreach (ISourceValuePointer v in ls)
							{
								if (v.IsWebClientValue() && !v.IsWebServerValue())
								{
									bool b = false;
									foreach (ISourceValuePointer p in l)
									{
										if (p.IsSameProperty(v))
										{
											b = true;
											break;
										}
									}
									if (!b)
									{
										l.Add(v);
									}
								}
							}
						}
					}
				}
			}
			return l;
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
		#region ICloneable Members

		public object Clone()
		{
			MathExpGroup obj = new MathExpGroup();
			obj.Name = Name;
			obj.Description = Description;
			obj.ShortDescription = ShortDescription;
			obj.IconImage = (ObjectIconData)IconImage.Clone();
			obj.ReturnIcon = (ReturnIcon)ReturnIcon.Clone();
			obj.Fixed = Fixed;
			obj.EitorBounds = _editorBounds;
			if (varDummy != null)
			{
				MathNodeRoot r = (MathNodeRoot)varDummy.Parent;
				MathNodeRoot r2 = (MathNodeRoot)r.Clone();
				obj.SetDummyVar((MathNodeVariableDummy)r2[1]);
			}
			if (_expressions != null)
			{
				List<MathExpItem> exps = new List<MathExpItem>();
				for (int i = 0; i < _expressions.Count; i++)
				{
					MathExpItem item = _expressions[i].Clone(obj);
					exps.Add(item);
				}
				obj.Expressions = exps;
			}

			VariableList inportVariables = new VariableList();
			VariableList vList = InportVariables;
			foreach (IVariable v in vList)
			{
				MathExpItem mi = GetItemByID(v.ID);
				if (!mi.MathExpression.IsContainer)
				{
					//do not create a duplication, use the one already cloned
					IVariable vc = obj.GetVariableByID(v.ID);
					inportVariables.Add(vc);
				}
				else
				{
					//this is a duplication, so must be cloned
					int k = -1;
					if (v == v.MathExpression[0])
						k = 0;
					else if (v == v.MathExpression[1])
						k = 1;
					MathNodeRoot r = (MathNodeRoot)v.MathExpression.Clone();
					if (k < 0)
						inportVariables.Add(r.Dummy);
					else
						inportVariables.Add((IVariable)r[k]);
				}
			}
			obj.SetInportVariables(inportVariables);
			obj.GenerateInputVariables();
			obj.makeLinks();
			//
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(this);
			if (sp != null)
			{
				obj.SetServiceProvider(sp);
			}
			obj.TransferLocalService(_localServices);
			return obj;
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
			if (_expressions != null)
			{
				foreach (MathExpItem item in _expressions)
				{
					if (item.MathExpression != null)
					{
						item.MathExpression.AddLocalService(serviceType, service);
					}
				}
			}
		}
		#endregion

		#region IMethod Members
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MathExpGroup r = (MathExpGroup)this.Clone();
			r.ActionContext = action;
			return r;
		}
		private object _prj;
		[XmlIgnore]
		[ReadOnly(true)]
		public object ModuleProject
		{
			get
			{
				return _prj;
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

		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return null;
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
				VariableList vs = InputVariables;
				if (vs != null)
					return vs.Count;
				return 0;
			}
		}
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
				return this.OutputVariable;
			}
			set
			{
				if (_returnIcon != null)
				{
					_returnIcon.ReturnVariable.VariableName = value.Name;
					_returnIcon.ReturnVariable.VariableType = new RaisDataType(value.ParameterLibType);
				}
			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				VariableList vl = InputVariables;
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
		public IList<IParameterValue> MethodParameterValues
		{
			get { return null; }
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "mg_{0}", this.ID.ToString("x", CultureInfo.InvariantCulture));
			}
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get { return ObjectKey; }
		}
		public bool IsSameMethod(ProgElements.IMethod method)
		{
			MathExpGroup g = method as MathExpGroup;
			if (g != null)
			{
				return (g.ID == this.ID);
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
			MathExpGroup g = objectIdentity as MathExpGroup;
			if (g != null)
			{
				return (g.ID == this.ID);
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
			get { return EnumObjectDevelopType.Custom; }
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
}
