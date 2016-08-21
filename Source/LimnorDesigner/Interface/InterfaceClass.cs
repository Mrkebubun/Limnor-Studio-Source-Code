/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.ComponentModel;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using MathExp;
using VPL;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Design;
using VSPrj;
using LimnorDesigner.Property;
using ProgElements;
using LimnorDesigner.Event;
using LimnorDesigner.MethodBuilder;
using System.Globalization;
using LimnorDesigner.Action;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// define a custom interface.
	/// use XClass to design it. The root object for the ClassPointer is an XClass holding the InterfaceClass
	/// </summary>
	public class InterfaceClass : IAttributeHolder
	{
		#region fields and constructors
		const string XML_Bases = "BaseInterfaces";
		const string XML_Properties = "Properties";
		const string XML_Methods = "Methods";
		const string XML_Events = "Events";
		public const string XML_VAROWNER = "VarOwner";
		private UInt32 _classId;
		private ClassPointer _holder;
		public InterfaceClass()
		{
		}
		#endregion
		#region Properties
		/// <summary>
		/// the ClassPointer holding the XClass which in turn holding this instance of InterfaceClass
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public ClassPointer Holder
		{
			get
			{
				return _holder;
			}
			set
			{
				_holder = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
			set
			{
				_classId = value;
			}
		}
		[Browsable(false)]
		public List<InterfacePointer> BaseInterfaces { get; set; }
		[Browsable(false)]
		public string InterfaceName { get; set; }
		[Browsable(false)]
		public List<InterfaceElementProperty> Properties { get; set; }
		[Browsable(false)]
		public List<InterfaceElementMethod> Methods { get; set; }
		[Browsable(false)]
		public List<InterfaceElementEvent> Events { get; set; } //each InterfaceElementEvent points to a delegate type
		#endregion
		#region Methods
		public CodeTypeDeclaration Compile()
		{
			CodeTypeDeclaration td = new CodeTypeDeclaration(InterfaceName);
			td.Attributes |= MemberAttributes.Public;
			td.IsInterface = true;
			td.TypeAttributes = System.Reflection.TypeAttributes.Interface | System.Reflection.TypeAttributes.Public;
			//
			if (BaseInterfaces != null)
			{
				foreach (InterfacePointer ip in BaseInterfaces)
				{
					td.BaseTypes.Add(ip.TypeString);
				}
			}
			//
			if (Properties != null)
			{
				foreach (InterfaceElementProperty p in Properties)
				{
					if (p.CanRead || p.CanWrite)
					{
						CodeMemberProperty mp = new CodeMemberProperty();
						mp.Name = p.Name;
						mp.Type = new CodeTypeReference(p.TypeString);
						mp.HasGet = p.CanRead;
						mp.HasSet = p.CanWrite;
						CompilerUtil.AddAttribute(p, mp.CustomAttributes, false, true, "");
						td.Members.Add(mp);
					}
				}
			}
			if (Methods != null)
			{
				foreach (InterfaceElementMethod m in Methods)
				{
					CodeMemberMethod mm = new CodeMemberMethod();
					mm.Name = m.Name;
					if (m.ReturnType != null)
					{
						mm.ReturnType = new CodeTypeReference(m.ReturnType.TypeString);
					}
					else
					{
						mm.ReturnType = new CodeTypeReference(typeof(void));
					}
					if (m.Parameters != null)
					{
						foreach (DataTypePointer p in m.Parameters)
						{
							mm.Parameters.Add(new CodeParameterDeclarationExpression(p.TypeString, p.Name));
						}
					}
					CompilerUtil.AddAttribute(m, mm.CustomAttributes, false, true, "");
					td.Members.Add(mm);
				}
			}
			if (Events != null)
			{
				foreach (InterfaceElementEvent e in Events)
				{
					CodeMemberEvent me = new CodeMemberEvent();
					me.Name = e.Name;
					me.Attributes = 0;
					me.Type = new CodeTypeReference(e.TypeString);
					CompilerUtil.AddAttribute(e, me.CustomAttributes, false, true, "");
					td.Members.Add(me);
				}
			}
			return td;
		}
		void saveProperties(ILimnorDesignerLoader loader)
		{
			XmlNode nodeCollection = loader.Node.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XML_Properties));
			if (nodeCollection == null)
			{
				nodeCollection = loader.Node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				loader.Node.AppendChild(nodeCollection);
				XmlUtil.SetNameAttribute(nodeCollection, XML_Properties);
			}
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor pd in pdc)
			{
				if (pd.Name == XML_Properties)
				{
					DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)pd.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
					loader.Writer.WriteProperty(pd, nodeCollection, this, visibility.Visibility, XmlTags.XML_PROPERTY);
					break;
				}
			}
		}
		public InterfaceElementProperty AddNewProperty(ILimnorDesignerLoader loader)
		{
			string baseName = "Property";
			int n = 1;
			string name = baseName + n.ToString();
			if (Properties != null)
			{
				bool exists = true;
				while (exists)
				{
					exists = false;
					foreach (InterfaceElementProperty m in Properties)
					{
						if (m.Name == name)
						{
							n++;
							name = baseName + n.ToString();
							exists = true;
							break;
						}
					}
				}
			}
			else
			{
				Properties = new List<InterfaceElementProperty>();
			}
			InterfaceElementProperty m0 = new InterfaceElementProperty(this);
			m0.Name = name;
			m0.SetDataType(typeof(string));
			m0.CanWrite = true;
			m0.CanRead = true;
			Properties.Add(m0);
			saveProperties(loader);
			loader.DesignPane.OnInterfacePropertyAdded(m0);
			loader.DesignPane.OnNotifyChanges();
			return m0;
		}
		private void saveMethods(ILimnorDesignerLoader loader)
		{
			XmlNode nodeCollection = loader.Node.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XML_Methods));
			if (nodeCollection == null)
			{
				nodeCollection = loader.Node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				loader.Node.AppendChild(nodeCollection);
				XmlUtil.SetNameAttribute(nodeCollection, XML_Methods);
			}
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor pd in pdc)
			{
				if (pd.Name == XML_Methods)
				{
					DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)pd.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
					loader.Writer.WriteProperty(pd, nodeCollection, this, visibility.Visibility, XmlTags.XML_PROPERTY);
					break;
				}
			}
		}
		public void OnMethodChanged(ILimnorDesignerLoader loader, InterfaceElementMethod method)
		{
			saveMethods(loader);
			loader.DesignPane.OnInterfaceMethodChanged(method);
			loader.DesignPane.OnNotifyChanges();
		}
		public void DeleteMethod(ILimnorDesignerLoader loader, UInt32 methodId)
		{
			if (Methods != null)
			{
				for (int i = 0; i < Methods.Count; i++)
				{
					if (Methods[i].MethodId == methodId)
					{
						InterfaceElementMethod method = Methods[i];
						Methods.Remove(method);
						saveMethods(loader);
						loader.DesignPane.OnInterfaceMethodDeleted(method);
						loader.DesignPane.OnNotifyChanges();
						break;
					}
				}
			}
		}
		public InterfaceElementMethod AddNewMethod(ILimnorDesignerLoader loader)
		{
			string name = loader.CreateMethodName("Method", null);
			InterfaceElementMethod m0 = new InterfaceElementMethod(this);
			m0.Name = name;
			XmlNode nodeMethodCollection = loader.Node.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XmlTags.XML_METHODS));
			if (nodeMethodCollection == null)
			{
				nodeMethodCollection = loader.Node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				loader.Node.AppendChild(nodeMethodCollection);
				XmlUtil.SetNameAttribute(nodeMethodCollection, XmlTags.XML_METHODS);
			}
			XmlNode nodeMethod = nodeMethodCollection.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}' and text()='{4}']",
				XmlTags.XML_Item, XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, "MethodId", m0.MethodId));
			if (nodeMethod == null)
			{
				nodeMethod = nodeMethodCollection.OwnerDocument.CreateElement(XmlTags.XML_Item);
				nodeMethodCollection.AppendChild(nodeMethod);
			}
			else
			{
				nodeMethod.RemoveAll();
			}
			XmlObjectWriter wr = loader.Writer;
			wr.WriteObjectToNode(nodeMethod, m0);
			if (wr.HasErrors)
			{
				MathNode.Log(wr.ErrorCollection);
			}
			if (Methods == null)
			{
				Methods = new List<InterfaceElementMethod>();
			}
			Methods.Add(m0);
			loader.DesignPane.OnInterfaceMethodCreated(m0);
			loader.DesignPane.OnNotifyChanges();
			return m0;
		}
		public void DeleteEvent(ILimnorDesignerLoader loader, UInt32 eventId)
		{
			if (Events != null)
			{
				for (int i = 0; i < Events.Count; i++)
				{
					if (Events[i].EventId == eventId)
					{
						InterfaceElementEvent e = Events[i];
						Events.Remove(e);
						saveEvents(loader);
						loader.DesignPane.OnInterfaceEventDeleted(e);
						loader.DesignPane.OnNotifyChanges();
						break;
					}
				}
			}
		}
		public void DeleteProperty(ILimnorDesignerLoader loader, UInt32 propertyId)
		{
			if (Properties != null)
			{
				for (int i = 0; i < Properties.Count; i++)
				{
					if (Properties[i].PropertyId == propertyId)
					{
						InterfaceElementProperty e = Properties[i];
						Properties.Remove(e);
						saveProperties(loader);
						loader.DesignPane.OnInterfacePropertyDeleted(e);
						loader.DesignPane.OnNotifyChanges();
						break;
					}
				}
			}
		}

		void saveEvents(ILimnorDesignerLoader loader)
		{
			XmlNode nodeCollection = loader.Node.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XML_Events));
			if (nodeCollection == null)
			{
				nodeCollection = loader.Node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				loader.Node.AppendChild(nodeCollection);
				XmlUtil.SetNameAttribute(nodeCollection, XML_Events);
			}
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor pd in pdc)
			{
				if (pd.Name == XML_Events)
				{
					DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)pd.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
					loader.Writer.WriteProperty(pd, nodeCollection, this, visibility.Visibility, XmlTags.XML_PROPERTY);
					break;
				}
			}
		}
		public InterfaceElementEvent AddNewEvent(ILimnorDesignerLoader loader)
		{
			string baseName = "Event";
			int n = 1;
			string name = baseName + n.ToString();
			if (Events != null)
			{
				bool exists = true;
				while (exists)
				{
					exists = false;
					foreach (InterfaceElementEvent m in Events)
					{
						if (m.Name == name)
						{
							n++;
							name = baseName + n.ToString();
							exists = true;
							break;
						}
					}
				}
			}
			else
			{
				Events = new List<InterfaceElementEvent>();
			}
			InterfaceElementEvent m0 = new InterfaceElementEvent(this);
			m0.Name = name;
			m0.SetDataType(typeof(EventHandler));
			Events.Add(m0);

			saveEvents(loader);
			loader.DesignPane.OnInterfaceEventAdded(m0);
			loader.DesignPane.OnNotifyChanges();
			return m0;
		}
		public void AddBaseInterface(InterfacePointer ip, ILimnorDesignerLoader loader)
		{
			if (BaseInterfaces == null)
			{
				BaseInterfaces = new List<InterfacePointer>();
			}
			foreach (InterfacePointer p in BaseInterfaces)
			{
				if (p.IsSameObjectRef(ip))
				{
					return;
				}
			}
			BaseInterfaces.Add(ip);
			XmlNode nodeCollection = loader.Node.SelectSingleNode(
				string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XML_Bases));
			if (nodeCollection == null)
			{
				nodeCollection = loader.Node.OwnerDocument.CreateElement(XML_Bases);
				loader.Node.AppendChild(nodeCollection);
				XmlUtil.SetNameAttribute(nodeCollection, XML_Bases);
			}
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor pd in pdc)
			{
				if (pd.Name == XML_Bases)
				{
					DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)pd.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
					loader.Writer.WriteProperty(pd, nodeCollection, this, visibility.Visibility, XmlTags.XML_PROPERTY);
					break;
				}
			}
			loader.DesignPane.OnBaseInterfaceAdded(this, ip);
			loader.DesignPane.OnNotifyChanges();
		}
		public void NotifyPropertyChange(ILimnorDesignerLoader loader, InterfaceElementProperty p)
		{
			saveProperties(loader);
			loader.DesignPane.OnInterfacePropertyChanged(p);
			loader.DesignPane.OnNotifyChanges();
		}
		public void NotifyEventChange(ILimnorDesignerLoader loader, InterfaceElementEvent e)
		{
			saveEvents(loader);
			loader.DesignPane.OnInterfaceEventChanged(e);
			loader.DesignPane.OnNotifyChanges();
		}
		public void NotifyMethodChange(ILimnorDesignerLoader loader, InterfaceElementMethod m)
		{
			saveMethods(loader);
			loader.DesignPane.OnInterfaceMethodChanged(m);
			loader.DesignPane.OnNotifyChanges();
		}
		#endregion
		#region IAttributeHolder Members

		public void AddAttribute(ConstObjectPointer attr)
		{
			_holder.AddAttribute(attr);
		}

		public void RemoveAttribute(ConstObjectPointer attr)
		{
			_holder.RemoveAttribute(attr);
		}

		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			return _holder.GetCustomAttributeList();
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
		{
			return _holder.IsSameObjectRef(objectIdentity);
		}

		public ProgElements.IObjectIdentity IdentityOwner
		{
			get { return null; }
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				if (_holder != null)
					return _holder.ObjectDevelopType;
				return EnumObjectDevelopType.Custom;
			}
		}

		public ProgElements.EnumPointerType PointerType
		{
			get { return ProgElements.EnumPointerType.Interface; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			InterfaceClass obj = new InterfaceClass();
			obj._holder = _holder;
			obj._classId = _classId;
			if (BaseInterfaces != null)
			{
				obj.BaseInterfaces = new List<InterfacePointer>(BaseInterfaces.ToArray());
			}
			obj.InterfaceName = InterfaceName;
			if (Properties != null)
			{
				obj.Properties = new List<InterfaceElementProperty>(Properties.ToArray());
			}
			if (Methods != null)
			{
				obj.Methods = new List<InterfaceElementMethod>(Methods.ToArray());
			}
			if (Events != null)
			{
				obj.Events = new List<InterfaceElementEvent>(Events.ToArray());
			}
			return obj;
		}

		#endregion

	}

	/// <summary>
	/// define a method prototype
	/// </summary>
	[UseParentObject]
	public class InterfaceElementMethod : IWithProject, IAttributeHolder, IMethod
	{
		#region fields and constructors
		private UInt32 _id;
		private InterfacePointer _owner;
		private string _name;
		private DataTypePointer _returnType;
		private IObjectPointer _variablePointer;
		public InterfaceElementMethod()
		{
		}
		public InterfaceElementMethod(InterfacePointer owner)
		{
			_owner = owner;
		}
		public InterfaceElementMethod(InterfaceClass owner)
		{
			_owner = new InterfacePointer(owner);
		}
		public InterfaceElementMethod(XClass<InterfaceClass> owner)
			: this((InterfaceClass)(owner.ObjectValue))
		{
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public string DefaultActionName
		{
			get
			{
				if (_variablePointer != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _variablePointer.ExpressionDisplay, _name);
				}
				return _name;
			}
		}
		[Browsable(false)]
		public IObjectPointer VariablePointer
		{
			get
			{
				return _variablePointer;
			}
			set
			{
				_variablePointer = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get
			{
				if (_owner != null && _owner.ClassId == 0)
				{
					_owner.SetInterfaceClass();
				}
				return _owner;
			}
		}
		[Browsable(false)]
		public UInt32 MethodId
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[ParenthesizePropertyName(true)]
		[Description("Method name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (Interface != null)
				{
					if (_name != value)
					{
						if (Interface.IsNameInUse(value))
						{
							MessageBox.Show("The name is in use");
						}
						else
						{
							_name = value;
							Interface.NotifyMethodChange(this);
						}
					}
				}
			}
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("The data type for the value returned by this method")]
		public DataTypePointer ReturnType
		{
			get
			{
				return _returnType;
			}
			set
			{
				_returnType = value;
				if (Interface != null)
					Interface.NotifyMethodChange(this);
			}
		}
		[Browsable(false)]
		public List<InterfaceElementMethodParameter> Parameters { get; set; }
		[Browsable(false)]
		public Type[] ParameterTypeArray
		{
			get
			{
				if (Parameters != null)
				{
					Type[] types = new Type[Parameters.Count];
					for (int i = 0; i < types.Length; i++)
					{
						types[i] = Parameters[i].VariableLibType;
					}
					return types;
				}
				return Type.EmptyTypes;
			}
		}
		[Browsable(false)]
		public List<ConstObjectPointer> AttributeList { get; set; }
		#endregion

		#region Methods
		public void SetVariablePointer(IObjectPointer v)
		{
			_variablePointer = v;
		}
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_variablePointer == null)
			{
				return null;
			}
			CodeMethodReferenceExpression cmr = new CodeMethodReferenceExpression(_variablePointer.GetReferenceCode(method, statements, forValue), this.Name);
			return cmr;
		}
		public CustomInterfaceMethodPointer CreatePointer(ActionClass act)
		{
			return new CustomInterfaceMethodPointer(this, _owner, act);
		}
		public void SetOwner(InterfacePointer owner)
		{
			_owner = owner;
		}
		public string GetMethodSignature()
		{
			StringBuilder sb = new StringBuilder();
			if (string.IsNullOrEmpty(_name))
				sb.Append("?");
			else
				sb.Append(_name);
			if ((Parameters != null && Parameters.Count > 0) || (ReturnType != null && !ReturnType.IsVoid))
			{
				sb.Append("(");
				if (Parameters != null && Parameters.Count > 0)
				{
					sb.Append(Parameters[0].TypeName);
					for (int i = 1; i < Parameters.Count; i++)
					{
						sb.Append(",");
						sb.Append(Parameters[i].TypeName);
					}
				}
				sb.Append(")");
				if (ReturnType != null && !ReturnType.IsVoid)
				{
					sb.Append(ReturnType.TypeName);
				}
			}
			return sb.ToString();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(Name);
			if ((Parameters != null && Parameters.Count > 0) || (ReturnType != null && !ReturnType.BaseClassType.Equals(typeof(void))))
			{
				sb.Append("(");
				if (Parameters != null && Parameters.Count > 0)
				{
					sb.Append(Parameters[0].ToString());
					for (int i = 1; i < Parameters.Count; i++)
					{
						sb.Append(",");
						sb.Append(Parameters[i].ToString());
					}
				}
				sb.Append(")");
				if (ReturnType != null && !ReturnType.BaseClassType.Equals(typeof(void)))
				{
					sb.Append(ReturnType.ToString());
				}
			}
			return sb.ToString();
		}
		/// <summary>
		/// do not do name checking
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			_name = name;
		}

		#endregion

		#region IWithProject Members
		[Browsable(false)]
		public LimnorProject Project
		{
			get
			{
				if (Interface != null)
					return Interface.ClassTypePointer.Project;
				return null;
			}
		}

		#endregion

		#region IAttributeHolder Members

		public void AddAttribute(ConstObjectPointer attr)
		{
			if (AttributeList == null)
			{
				AttributeList = new List<ConstObjectPointer>();
			}
			foreach (ConstObjectPointer c in AttributeList)
			{
				if (c.ValueId == attr.ValueId)
				{
					return;
				}
			}
			AttributeList.Add(attr);
		}

		public void RemoveAttribute(ConstObjectPointer attr)
		{
			if (AttributeList != null)
			{
				foreach (ConstObjectPointer c in AttributeList)
				{
					if (c.ValueId == attr.ValueId)
					{
						AttributeList.Remove(c);
						return;
					}
				}
			}
		}

		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			return AttributeList;
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			InterfaceElementMethod m = objectIdentity as InterfaceElementMethod;
			if (m != null)
			{
				return (m._id == _id);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return _owner; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get { return false; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Custom; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Interface; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			InterfaceElementMethod obj = new InterfaceElementMethod(_owner);
			obj._name = _name;
			obj._id = _id;
			obj._returnType = _returnType;
			obj._variablePointer = _variablePointer;
			if (Parameters != null)
			{
				obj.Parameters = new List<InterfaceElementMethodParameter>();
				foreach (InterfaceElementMethodParameter p in Parameters)
				{
					obj.Parameters.Add((InterfaceElementMethodParameter)p.Clone());
				}
			}
			return obj;
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)serializer;
			_id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_MethodID);
			_name = XmlUtil.GetNameAttribute(node);
			UInt32 cid = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);
			ClassPointer cp = ClassPointer.CreateClassPointer(cid, reader.ObjectList.Project);
			XClass<InterfaceClass> xf = cp.ObjectInstance as XClass<InterfaceClass>;
			if (xf != null)
			{
				InterfaceClass ifc = xf.ObjectValue as InterfaceClass;
				if (ifc != null)
				{
					_owner = new InterfacePointer(ifc);
					if (ifc.Methods != null)
					{
						for (int i = 0; i < ifc.Methods.Count; i++)
						{
							if (ifc.Methods[i].MethodId == _id)
							{
								_name = ifc.Methods[i].Name;
								_returnType = ifc.Methods[i].ReturnType;
								Parameters = ifc.Methods[i].Parameters;
								break;
							}
						}
					}
				}
			}
			if (_variablePointer == null)
			{
				XmlNode varNode = node.SelectSingleNode(InterfaceClass.XML_VAROWNER);
				if (varNode != null)
				{
					object v = reader.ReadObject(varNode, this);
					_variablePointer = v as IObjectPointer;
				}
			}
		}
		[Browsable(false)]
		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ClassID, _owner.ClassId);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_MethodID, _id);
			XmlUtil.SetNameAttribute(node, _name);
			if (_variablePointer != null)
			{
				XmlNode varNode = XmlUtil.CreateSingleNewElement(node, InterfaceClass.XML_VAROWNER);
				writer.WriteObjectToNode(varNode, _variablePointer, true);
			}
		}

		#endregion

		#region IMethod Members
		[Browsable(false)]
		[ReadOnly(true)]
		public IParameter MethodReturnType
		{
			get
			{
				if (_returnType != null)
				{
					InterfaceElementMethodParameter p = new InterfaceElementMethodParameter(_returnType.BaseClassType, "return", this);
					return p;
				}
				return null;
			}
			set
			{
				
			}
		}
		[Browsable(false)]
		public bool NoReturn
		{
			get { return _returnType == null || typeof(void).Equals(_returnType.BaseClassType); }
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "mi{0}", this.MethodId.ToString("x", CultureInfo.InvariantCulture)); }
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(this.Name);
				sb.Append("(");
				if (this.Parameters != null && this.Parameters.Count > 0)
				{
					for (int i = 0; i < this.Parameters.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(",");
						}
						sb.Append(this.Parameters[i].Name);
						sb.Append(" ");
						sb.Append(this.Parameters[i].ObjectType.Name);
					}
				}
				sb.Append(")");
				if (_returnType != null && typeof(void).Equals(_returnType.BaseClassType))
				{
					sb.Append(_returnType.BaseClassType.Name);
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public bool HasReturn
		{
			get
			{
				if (this.NoReturn)
					return false;
				return true;
			}
		}
		[Browsable(false)]
		public bool IsMethodReturn
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor
		{
			get
			{
				return false;
			}
		}

		public bool IsSameMethod(IMethod method)
		{
			InterfaceElementMethod em = method as InterfaceElementMethod;
			if (em != null)
				return em.MethodId == this.MethodId;
			return false;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return new Dictionary<string,string>();
		}

		public string ParameterName(int i)
		{
			if (this.Parameters != null && i >= 0 && i < this.Parameters.Count)
				return this.Parameters[i].Name;
			return null;
		}

		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			return new CustomInterfaceMethodPointer(this, _owner, action as IAction);
		}

		public object GetParameterType(uint id)
		{
			if (this.Parameters != null)
			{
				for (int i = 0; i < this.Parameters.Count; i++)
				{
					if (id == this.Parameters[i].MemberId)
					{
						return this.Parameters[i];
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}
		[Browsable(false)]
		public Type ActionType
		{
			get { return typeof(ActionClass); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ModuleProject
		{
			get
			{
				if (_owner != null)
				{
					return _owner.Project;
				}
				return null;
			}
			set
			{
				
			}
		}

		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}

		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return string.Empty;
		}

		#endregion

		#region IMethod0 Members
		[Browsable(false)]
		[ReadOnly(true)]
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

		public int ParameterCount
		{
			get
			{
				if(this.Parameters != null)
					return this.Parameters.Count;
				return 0;
			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> l = new List<IParameter>();
				if (this.ParameterCount > 0)
				{
					for (int i = 0; i < this.Parameters.Count; i++)
					{
						l.Add(this.Parameters[i]);
					}
				}
				return l;
			}
		}

		#endregion
	}
	[UseParentObject]
	public class InterfaceElementMethodParameter : NamedDataType, IParameter
	{
		private InterfaceElementMethod _method;
		public InterfaceElementMethodParameter(InterfaceElementMethod method)
		{
			_method = method;
		}
		public InterfaceElementMethodParameter(Type t, string name, InterfaceElementMethod method)
			: base(t, name)
		{
			_method = method;
		}
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get
			{
				return _method.Interface;
			}
		}
		protected override void OnBeforeNameChange(EventArgNameChange e)
		{
			if (_method != null && _method.Parameters != null && _method.Parameters.Count > 0)
			{
				for (int i = 0; i < _method.Parameters.Count; i++)
				{
					if (_method.Parameters[i].Name == e.NewName)
					{
						e.Cancel = true;
						MessageBox.Show("The name is in use");
					}
				}
			}
		}
		protected override void OnDataTypeChanged()
		{
			base.OnDataTypeChanged();
			if (Interface != null)
				Interface.NotifyMethodChange(_method);
		}
		protected override void OnNameChanged()
		{
			if (Interface != null)
				Interface.NotifyMethodChange(_method);
		}

		#region INamedDataType Members


		public Type ParameterLibType
		{
			get
			{
				return this.BaseClassType;
			}
			set
			{
				this.SetDataType(value);
			}
		}

		public string ParameterTypeString
		{
			get { return this.TypeString; }
		}

		#endregion
	}
	/// <summary>
	/// one property used in an interface
	/// </summary>
	[UseParentObject]
	public class InterfaceElementProperty : NamedDataType, IAttributeHolder, IPropertyEx
	{
		#region fields and constructors
		private InterfacePointer _owner;
		const string XMLATT_CanRead = "canRead";
		const string XMLATT_CanWrite = "canWrite";
		
		private bool _canRead;
		private bool _canWrite;
		private IObjectPointer _variablePointer;
		public InterfaceElementProperty(InterfacePointer owner)
		{
			_owner = owner;
		}
		public InterfaceElementProperty(InterfacePointer owner, PropertyInfo info)
		{
			_owner = owner;
			SetDataType(info.PropertyType);
			SetName(info.Name);
			_canRead = info.CanRead;
			_canWrite = info.CanWrite;
		}
		public InterfaceElementProperty(InterfaceClass owner)
		{
			_owner = new InterfacePointer(owner);
		}
		public InterfaceElementProperty(XClass<InterfaceClass> owner)
			: this((InterfaceClass)(owner.ObjectValue))
		{
		}
		/// <summary>
		/// points to a lib type
		/// </summary>
		/// <param name="type"></param>
		public InterfaceElementProperty(TypePointer type)
			: base(type)
		{
		}
		/// <summary>
		/// points to a custom type
		/// </summary>
		/// <param name="component"></param>
		public InterfaceElementProperty(ClassPointer component)
			: base(component)
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override string DisplayName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", Name, TypeName);
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get
			{
				if (_owner != null && _owner.ClassId == 0)
				{
					_owner.SetInterfaceClass();
				}
				return _owner;
			}
		}
		[Description("true:this property is readable; false: this property is not readable")]
		public bool CanRead
		{
			get
			{
				return _canRead;
			}
			set
			{
				if (_canRead != value)
				{
					_canRead = value;
					if (Interface != null)
						Interface.NotifyPropertyChange(this);
				}
			}
		}
		[Description("true:this property is writable; false: this property is not writable")]
		public bool CanWrite
		{
			get
			{
				return _canWrite;
			}
			set
			{
				if (_canWrite != value)
				{
					_canWrite = value;
					if (Interface != null)
						Interface.NotifyPropertyChange(this);
				}
			}
		}
		[Browsable(false)]
		public UInt32 PropertyId
		{
			get
			{
				return ParameterID;
			}
			set
			{
				ParameterID = value;
			}
		}
		[Browsable(false)]
		public List<ConstObjectPointer> AttributeList { get; set; }

		[Browsable(false)]
		public IObjectPointer VariablePointer
		{
			get
			{
				return _variablePointer;
			}
		}
		#endregion
		#region overrides
		[Browsable(false)]
		protected override DataTypePointer CreateCloneObject()
		{
			InterfaceElementProperty obj = new InterfaceElementProperty(_owner);
			obj._canRead = _canRead;
			obj._canWrite = _canWrite;
			obj._variablePointer = _variablePointer;
			obj.AttributeList = this.AttributeList;
			obj.PropertyId = this.PropertyId;
			obj.SetName(this.Name);
			obj.SetDataType(this.DataType);
			return obj;
		}
		[Browsable(false)]
		protected override void OnBeforeNameChange(EventArgNameChange e)
		{
			if (Interface != null && Interface.IsNameInUse(e.NewName))
			{
				e.Cancel = true;
				MessageBox.Show("The name is in use");
			}
		}
		protected override void OnDataTypeChanged()
		{
			base.OnDataTypeChanged();
			if (Interface != null)
				Interface.NotifyPropertyChange(this);
		}
		protected override void OnNameChanged()
		{
			if (Interface != null)
				Interface.NotifyPropertyChange(this);
		}
		[Browsable(false)]
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_variablePointer != null)
			{
				return new CodePropertyReferenceExpression(_variablePointer.GetReferenceCode(method, statements, forValue), this.Name);
			}
			if (Interface != null)
				throw new DesignerException("Interface property not assigned an instance. {0}.{1}", Interface.Name, this.Name);
			else
				throw new DesignerException("Interface property not assigned an instance. ?.{0}", this.Name);
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public void SetOwner(InterfacePointer owner)
		{
			_owner = owner;
		}
		[Browsable(false)]
		public void SetVariablePointer(IObjectPointer v)
		{
			_variablePointer = v;
		}
		#endregion
		#region IXmlNodeSerializable Members
		[Browsable(false)]
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_CanRead, CanRead);
			XmlUtil.SetAttribute(node, XMLATT_CanWrite, CanWrite);
			if (_owner != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ClassID, _owner.ClassId);
			}
			if (_variablePointer != null)
			{
				XmlNode varNode = XmlUtil.CreateSingleNewElement(node, InterfaceClass.XML_VAROWNER);
				writer.WriteObjectToNode(varNode, _variablePointer, true);
			}
		}
		[Browsable(false)]
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			_canRead = XmlUtil.GetAttributeBoolDefTrue(node, XMLATT_CanRead);
			_canWrite = XmlUtil.GetAttributeBoolDefTrue(node, XMLATT_CanWrite);
			if (_owner == null)
			{
				ClassPointer cp = null;
				UInt32 cid = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);
				if (cid != 0)
				{
					XmlObjectReader xr = reader as XmlObjectReader;
					if (xr != null)
					{
						cp = ClassPointer.CreateClassPointer(cid, xr.ObjectList.Project);
					}
				}
				if (cp == null)
				{
					cp = this.Owner as ClassPointer;
				}
				if (cp != null)
				{
					XClass<InterfaceClass> xc = cp.ObjectInstance as XClass<InterfaceClass>;
					if (xc != null)
					{
						InterfaceClass ifc = xc.ObjectValue as InterfaceClass;
						if (ifc != null)
						{
							_owner = new InterfacePointer(ifc);
						}
					}
				}
			}
			if (_variablePointer == null)
			{
				XmlNode varNode = node.SelectSingleNode(InterfaceClass.XML_VAROWNER);
				if (varNode != null)
				{
					object v = reader.ReadObject(varNode, this);
					_variablePointer = v as IObjectPointer;
				}
			}
		}
		#endregion

		#region IAttributeHolder Members
		[Browsable(false)]
		public void AddAttribute(ConstObjectPointer attr)
		{
			if (AttributeList == null)
			{
				AttributeList = new List<ConstObjectPointer>();
			}
			foreach (ConstObjectPointer c in AttributeList)
			{
				if (c.ValueId == attr.ValueId)
				{
					return;
				}
			}
			AttributeList.Add(attr);
		}
		[Browsable(false)]
		public void RemoveAttribute(ConstObjectPointer attr)
		{
			if (AttributeList != null)
			{
				foreach (ConstObjectPointer c in AttributeList)
				{
					if (c.ValueId == attr.ValueId)
					{
						AttributeList.Remove(c);
						return;
					}
				}
			}
		}
		[Browsable(false)]
		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			return AttributeList;
		}

		#endregion

		#region IPropertyEx Members
		[Browsable(false)]
		public SetterPointer CreateSetterMethodPointer(IAction act)
		{
			SetterPointer mp = new SetterPointer(act);
			MemberComponentIdCustom.AdjustHost(this);
			mp.Owner = Owner;
			mp.SetProperty = this;
			return mp;
		}
		[Browsable(false)]
		public Type CodeType
		{
			get
			{
				return this.ObjectType;
			}
		}

		#endregion

		#region IProperty Members
		[Browsable(false)]
		public DataTypePointer PropertyType
		{
			get { return this; }
		}
		[Browsable(false)]
		public bool IsReadOnly
		{
			get { return !this.CanWrite; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public EnumAccessControl AccessControl
		{
			get
			{
				return EnumAccessControl.Public;
			}
			set
			{
				
			}
		}
		[Browsable(false)]
		public bool IsCustomProperty
		{
			get { return true; }
		}
		[Browsable(false)]
		public bool Implemented
		{
			get { return true; }
		}
		[Browsable(false)]
		public ClassPointer Declarer
		{
			get
			{
				if (_owner != null)
				{
					return _owner.GetHolder();
				}
				return null;
			}
		}
		[Browsable(false)]
		public IClass Holder
		{
			get
			{
				if (_owner != null)
				{
					return _owner.Host;
				}
				return null;
			}
		}
		[Browsable(false)]
		public void SetValue(object value)
		{
			
		}
		[Browsable(false)]
		public IList<Attribute> GetUITypeEditor()
		{
			return null;
		}

		#endregion
	}

	[UseParentObject]
	public class InterfaceElementEvent : NamedDataType, IAttributeHolder
	{
		#region fields and constructors
		private InterfacePointer _owner;
		private IObjectPointer _variablePointer;
		public InterfaceElementEvent(InterfaceClass owner)
		{
			_owner = new InterfacePointer(owner);
		}
		public InterfaceElementEvent(InterfacePointer owner)
		{
			_owner = owner;
		}
		public InterfaceElementEvent(XClass<InterfaceClass> owner)
			: this((InterfaceClass)(owner.ObjectValue))
		{
		}
		/// <summary>
		/// points to a lib type
		/// </summary>
		/// <param name="type"></param>
		public InterfaceElementEvent(TypePointer type)
			: base(type)
		{
		}
		/// <summary>
		/// points to a custom type
		/// </summary>
		/// <param name="component"></param>
		public InterfaceElementEvent(CustomEventHandlerType component)
			: base(component)
		{
		}
		public InterfaceElementEvent()
		{
		}
		#endregion
		#region overrides
		protected override void OnBeforeNameChange(EventArgNameChange e)
		{
			if (Interface != null && Interface.IsNameInUse(e.NewName))
			{
				e.Cancel = true;
				MessageBox.Show("The name is in use");
			}
		}
		protected override void OnDataTypeChanged()
		{
			base.OnDataTypeChanged();
			if (Interface != null)
			{
				Interface.NotifyEventChange(this);
			}
		}
		protected override void OnNameChanged()
		{
			if (Interface != null)
			{
				Interface.NotifyEventChange(this);
			}
		}
		[Browsable(false)]
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(Name);
			if (Parameters != null && Parameters.Count > 0)
			{
				sb.Append("(");
				sb.Append(Parameters[0].ToString());
				for (int i = 1; i < Parameters.Count; i++)
				{
					sb.Append(",");
					sb.Append(Parameters[i].ToString());
				}
				sb.Append(")");
			}
			return sb.ToString();
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public IObjectPointer VariablePointer
		{
			get
			{
				return _variablePointer;
			}
			set
			{
				_variablePointer = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get
			{
				if (_owner != null)
				{
					if (_owner.ClassId == 0)
					{
						_owner.SetInterfaceClass();
					}
				}
				return _owner;
			}
		}
		[TypeScope(typeof(Delegate))]
		[Description("The type of event handler")]
		public override IObjectPointer DataType
		{
			get
			{
				return base.DataType;
			}
			set
			{
				base.DataType = value;
			}
		}
		[Browsable(false)]
		public UInt32 EventId
		{
			get
			{
				return ParameterID;
			}
			set
			{
				ParameterID = value;
			}
		}
		[Browsable(false)]
		public List<NamedDataType> Parameters
		{
			get
			{
				if (this.IsLibType)
				{
					List<NamedDataType> l = new List<NamedDataType>();
					MethodInfo mif = this.VariableLibType.GetMethod("Invoke");
					ParameterInfo[] pifs = mif.GetParameters();
					if (pifs != null && pifs.Length > 0)
					{
						for (int i = 0; i < pifs.Length; i++)
						{
							NamedDataType dp = new NamedDataType(pifs[i].ParameterType, pifs[i].Name);
							l.Add(dp);
						}
					}
					return l;
				}
				else
				{
					//TBD:
					CustomEventHandlerType cp = this.VariableCustomType as CustomEventHandlerType;
					if (cp != null)
					{
						return cp.Parameters;
					}
					return null;
				}
			}
		}
		[Browsable(false)]
		public List<ConstObjectPointer> AttributeList { get; set; }
		#endregion
		[Browsable(false)]
		public void SetOwner(InterfacePointer owner)
		{
			_owner = owner;
		}
		#region IAttributeHolder Members
		[Browsable(false)]
		public void AddAttribute(ConstObjectPointer attr)
		{
			if (AttributeList == null)
			{
				AttributeList = new List<ConstObjectPointer>();
			}
			foreach (ConstObjectPointer c in AttributeList)
			{
				if (c.ValueId == attr.ValueId)
				{
					return;
				}
			}
			AttributeList.Add(attr);
		}
		[Browsable(false)]
		public void RemoveAttribute(ConstObjectPointer attr)
		{
			if (AttributeList != null)
			{
				foreach (ConstObjectPointer c in AttributeList)
				{
					if (c.ValueId == attr.ValueId)
					{
						AttributeList.Remove(c);
						return;
					}
				}
			}
		}
		[Browsable(false)]
		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			return AttributeList;
		}

		#endregion

	}
}
