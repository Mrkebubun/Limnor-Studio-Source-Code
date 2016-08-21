/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using System.Drawing;
using XmlUtility;
using System.Xml;
using XmlSerializer;
using LimnorDesigner.Property;
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// a field for implementing a property
	/// </summary>
	[ToolboxBitmapAttribute(typeof(MathNodePropertyField), "Resources.propertyField.bmp")]
	[Description("It represents a private member of the class for defining a property. \r\nIn editing the Getter and Setter methods of a property, \r\nthis value should be used instead of the property itself.")]
	[MathNodeCategory(enumOperatorCategory.System)]
	public class MathNodePropertyField : MathNode, IVariable
	{
		#region fields and constructors
		public static PropertyClass PropertyInEditing;
		private UInt32 _propertyId;
		private PropertyClass _property;
		private bool _isInport;
		private bool _isParam;
		private bool _isReturn;
		private bool _clonePorts;
		private LinkLineNodeOutPort[] _outports;
		private LinkLineNodeInPort _inport;
		public MathNodePropertyField(MathNode parent)
			: base(parent)
		{
			_property = PropertyInEditing;
		}
		#endregion
		#region methods
		public void SetOwnerProperty(PropertyClass p)
		{
			_property = p;
		}
		protected virtual void OnInPortCreated() { }
		public static void CheckDeclareField(bool isStatic, string fieldName, DataTypePointer fieldType, CodeTypeDeclaration typeDeclaration, string defaultValue)
		{
			CodeMemberField cmf;
			foreach (CodeTypeMember ctm in typeDeclaration.Members)
			{
				cmf = ctm as CodeMemberField;
				if (cmf != null)
				{
					if (string.CompareOrdinal(cmf.Name, fieldName) == 0)
					{
						return;
					}
				}
			}
			cmf = new CodeMemberField(fieldType.TypeString, fieldName);
			if (!string.IsNullOrEmpty(defaultValue))
			{
				bool b;
				object val = VPLUtil.ConvertObject(defaultValue,fieldType.BaseClassType,out b);
				if(b)
				{
					cmf.InitExpression = ObjectCreationCodeGen.ObjectCreationCode(val);
				}
			}
			if (isStatic)
			{
				cmf.Attributes |= MemberAttributes.Static;
			}
			typeDeclaration.Members.Add(cmf);
		}
		#endregion
		#region private properties
		private string displayName
		{
			get
			{
				if (_property != null)
				{
					return _property.FieldMemberName;// .Name;
				}
				return "Property" + _propertyId.ToString();
			}
		}
		#endregion
		#region properties
		[Browsable(false)]
		public UInt32 PropertyId
		{
			get
			{
				return (UInt32)_propertyId;
			}
			set
			{
				_propertyId = value;
			}
		}
		#endregion
		#region Math Node Members
		[Browsable(false)]
		public override MathExp.RaisTypes.RaisDataType DataType
		{
			get
			{
				if (_property != null)
					return new MathExp.RaisTypes.RaisDataType(_property.PropertyType.ObjectType);
				return new MathExp.RaisTypes.RaisDataType(typeof(string));
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get
			{
				if (_property != null)
				{
					return "field of " + _property.ToString();
				}
				return "Property id " + _propertyId.ToString();
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			Font font = this.TextFont;
			SizeF size = g.MeasureString(displayName, font);
			return new SizeF(size.Width + 1, size.Height + 1);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sz = DrawSize;
			System.Drawing.Font font = this.TextFont;
			string s = displayName;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
			}
			else
			{
				g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			if (_property != null)
			{
				CodeExpression ownerCode;
				CheckDeclareField(_property.IsStatic, _property.FieldMemberName, _property.PropertyType, method.TypeDeclaration, _property.DefaultValue);
				if (_property.IsStatic)
				{
					ownerCode = new CodeTypeReferenceExpression(_property.Holder.TypeString);
				}
				else
				{
					ownerCode = new CodeThisReferenceExpression();
				}
				return new CodeFieldReferenceExpression(ownerCode, _property.FieldMemberName);
			}
			return null;
		}
		public string ExportJavaScriptCode(StringCollection method)
		{
			return CreateJavaScript(method);
		}
		public string ExportPhpScriptCode(StringCollection method)
		{
			return CreatePhpScript(method);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			if (_property != null)
			{
				string ownerCode;
				if (_property.IsStatic)
				{
					ownerCode = _property.Holder.TypeString;
				}
				else
				{
					ownerCode = "window";
				}
				return MathNode.FormString("{0}.{1}", ownerCode, _property.FieldMemberName);
			}
			return null;
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (_property != null)
			{
				string ownerCode;
				if (_property.IsStatic)
				{
					ownerCode = _property.Holder.TypeString;
				}
				else
				{
					ownerCode = "$this";
				}
				return MathNode.FormString("{0}->{1}", ownerCode, _property.FieldMemberName);
			}
			return null;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}

		public override string ToString()
		{
			return displayName;
		}

		protected override void OnLoad(System.Xml.XmlNode node)
		{
			_propertyId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_PropId);
			XmlNode nd = SerializeUtil.GetCustomPropertyNode(node, (UInt32)_propertyId);
			if (nd != null)
			{
				XmlSerializer.XmlObjectReader xr = this.root.Reader as XmlSerializer.XmlObjectReader;
				if (xr != null)
				{
					_property = (PropertyClass)xr.ReadObject(nd, ClassPointer.CreateClassPointer(xr.ObjectList));
				}
				else
				{
					throw new DesignerException("Reader not available calling MathNodePropertyField.OnLoad");
				}
			}
		}
		protected override void OnSave(System.Xml.XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_PropId, _propertyId);
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodePropertyField clone = (MathNodePropertyField)base.CloneExp(parent);
			clone.PropertyId = PropertyId;
			if (_property != null)
			{
				clone.SetOwnerProperty(_property);
			}
			return clone;
		}
		#endregion
		#region IVariable Members
		[ReadOnly(true)]
		[Browsable(false)]
		public MathExp.RaisTypes.RaisDataType VariableType
		{
			get
			{
				return DataType;
			}
			set
			{

			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string VariableName
		{
			get
			{
				return _property.FieldMemberName;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string SubscriptName
		{
			get
			{
				return "";
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IMathExpression Scope
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override bool IsLocal
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool NoAutoDeclare
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
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
		[ReadOnly(true)]
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
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsInPort
		{
			get
			{
				return _isInport;
			}
			set
			{
				_isInport = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsConst
		{
			get
			{
				return false;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsCodeVariable
		{
			get
			{
				return false;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool IsMathRootReturn
		{
			get
			{
				return false;
			}
		}
		[ReadOnly(true)]
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
		[ReadOnly(true)]
		[Browsable(false)]
		public string KeyName
		{
			get
			{
				return VariableName;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				return VariableName;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string CodeVariableName
		{
			get
			{
				return VariableName;
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
		public void AssignCode(CodeExpression code)
		{
		}
		public void AssignJavaScriptCode(string code)
		{
		}
		public void AssignPhpScriptCode(string code)
		{
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ID
		{
			get
			{
				return _propertyId;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 RawId
		{
			get
			{
				return _propertyId;
			}
		}

		public void ResetID(UInt32 id)
		{
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
		[Browsable(false)]
		public MathNodeRoot MathExpression
		{
			get { return this.root; }
		}

		#endregion

		#region IPortOwner Members
		[Browsable(false)]
		public bool IsDummyPort
		{
			get { return false; }
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
		public virtual void RemovePort(LinkLineNodePort port)
		{
			if (_outports != null)
			{
				List<LinkLineNodeOutPort> lst = new List<LinkLineNodeOutPort>();
				foreach (LinkLineNodeOutPort p in _outports)
				{
					if (p != port)
					{
						lst.Add(p);
					}
				}
				if (lst.Count < _outports.Length)
				{
					_outports = lst.ToArray();
				}
			}
		}
		[Browsable(false)]
		public UInt32 PortOwnerID
		{
			get { return this.ID; }
		}

		#endregion

		#region IParameter Members
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ParameterID
		{
			get
			{
				return (UInt32)this.ID;
			}
			set
			{
			}
		}

		#endregion

		#region INamedDataType Members
		[ReadOnly(true)]
		[Browsable(false)]
		public string Name
		{
			get
			{
				return this.VariableName;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public Type ParameterLibType
		{
			get
			{
				return this.VariableType.LibType;
			}
			set
			{
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
}
