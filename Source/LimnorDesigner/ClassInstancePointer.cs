/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using XmlSerializer;
using XmlUtility;
using System.Runtime.Serialization;
using System.CodeDom;
using MathExp;
using LimnorDesigner.Property;
using VSPrj;
using System.Drawing;
using VPL;
using ProgElements;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Event;
using System.Reflection;
using System.Collections.Specialized;
using Limnor.Drawing2D;
using LimnorDesigner.Action;
using System.Globalization;
using Limnor.WebBuilder;

namespace LimnorDesigner
{
	/// <summary>
	/// it represents an instance of a definition class, which is a component member of a hosting class.
	/// 
	/// ClassId -> the class id of the definition class.
	/// Definition -> the definition class.
	/// MemberId -> instance id among the member components of the hosting class.
	/// Owner/Host -> a ClassPointer for the hosting class.
	/// ObjectInstance -> the component member of the hosting class created by the underlying Type of the definition class.
	///         if the Type is not a component then it is an instance of XClass<Type>
	/// 
	/// it is saved as a ClassRef XML element under Root.
	/// It is not used for a property. 
	/// When a class is used as a property, ClassPointer is used in DataTypePointer to indicate the property type.
	/// </summary>
	[ToolboxBitmapAttribute(typeof(ClassInstancePointer), "Resources.object.bmp")]
	[UseParentObject]
	public class ClassInstancePointer : IComponent, ICustomTypeDescriptor, IClassRef, IClassContainer, IXmlNodeSerializable
	{
		#region fields and constructors
		private IClassContainer _hostingClass;
		private ClassPointer _definingClass;
		private string _name;
		private Image _icon;
		private string _typeName;
		private string _iconFilename;
		private List<PropertyClassDescriptor> _properties;
		private UInt32 _definingClassId;
		private UInt32 _memberId;
		//
		private UInt32 _classVersion;
		//
		public ClassInstancePointer()
		{
		}
		public ClassInstancePointer(IClassContainer host)
		{
			_hostingClass = host;
		}
		public ClassInstancePointer(IClassContainer host, ClassPointer definition, UInt32 instanceId)
		{
			if (host == null)
			{
				throw new DesignerException("ClassInstancePointer missing host. Instance id [{0}].", instanceId);
			}
			_hostingClass = host;
			_definingClass = definition;
			_definingClassId = definition.ClassId;
			_memberId = instanceId;
		}
		#endregion
		#region methods
		public string GetTypeName(string scopeNamespace)
		{
			return _definingClass.GetTypeName(scopeNamespace);
		}
		public void DeleteCustomProperty(UInt32 id)
		{
			if (_properties != null)
			{
				foreach (PropertyClassDescriptor p in _properties)
				{
					if (p.MemberId == id)
					{
						_properties.Remove(p);
						break;
					}
				}
			}
		}
		public void ChangeCustomProperty(PropertyClass pc)
		{
			if (_properties != null)
			{
				foreach (PropertyClassDescriptor p in _properties)
				{
					if (p.MemberId == pc.MemberId)
					{
						p.Reset(pc);
						break;
					}
				}
			}
		}
		public void AddCustomProperty(PropertyClass pc)
		{
			if (_properties != null)
			{
				foreach (PropertyClassDescriptor p in _properties)
				{
					if (p.MemberId == pc.MemberId)
					{
						p.Reset(pc);
						return;
					}
				}
				_properties.Add(new PropertyClassDescriptor(pc));
			}
		}
		public void OnSetDefinition()
		{
			if (_definingClass == null)
			{
				throw new DesignerException("Defining class not loaded for accessing properties for class {0}", _definingClassId);
			}
			_classVersion = _definingClass.EditVersion;
			XmlNode classXmlNode = _definingClass.XmlData;
			_typeName = XmlUtil.GetNameAttribute(classXmlNode);
			_properties = new List<PropertyClassDescriptor>();
			Dictionary<string, PropertyClass> ps = _definingClass.CustomProperties;
			foreach (PropertyClass p in ps.Values)
			{
				if (p.Implemented && p.AccessControl == EnumAccessControl.Public)
				{
					PropertyClass p0 = (PropertyClass)p.Clone();
					p0.SetHolder(this);
					_properties.Add(new PropertyClassDescriptor(p0));
				}
			}
			if (string.IsNullOrEmpty(_iconFilename))
			{
				_iconFilename = SerializeUtil.ReadIconFilename(classXmlNode);
			}
		}
		public Dictionary<string, MethodClass> GetCustomMethods()
		{
			Dictionary<string, MethodClass> list = _definingClass.CustomMethods;
			Dictionary<string, MethodClass> list0 = new Dictionary<string, MethodClass>();
			foreach (KeyValuePair<string, MethodClass> kv in list)
			{
				MethodClass m0 = (MethodClass)(kv.Value.Clone());
				m0.SetHolder(this);
				list0.Add(kv.Key, m0);
			}
			return list0;
		}
		public Dictionary<string, EventClass> GetCustomEvents()
		{
			Dictionary<string, EventClass> list = _definingClass.CustomEvents;
			Dictionary<string, EventClass> list0 = new Dictionary<string, EventClass>();
			foreach (KeyValuePair<string, EventClass> kv in list)
			{
				EventClass e = (EventClass)(kv.Value.Clone());
				e.SetHolder(this);
				list0.Add(kv.Key, e);
			}
			return list0;
		}
		public void ReplaceDeclaringClassPointer(ObjectIDmap map)
		{
			_definingClass.SetData(map);
		}
		public bool ReplaceDeclaringClassPointer(ClassPointer pointer)
		{
			if (pointer != null)
			{
				if (_definingClass != pointer)
				{
					_definingClass = pointer;
					return true;
				}
			}
			return false;
		}
		public ClassProperties CreatePropertyHolder()
		{
			return new ClassProperties(this);
		}

		[Browsable(false)]
		public bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			ClassInstancePointer obj = objectPointer as ClassInstancePointer;
			if (obj != null)
			{
				return (this.WholeId == obj.WholeId);
			}
			MemberComponentId mc = objectPointer as MemberComponentId;
			if (mc != null)
			{
				ClassInstancePointer cr = mc.ObjectInstance as ClassInstancePointer;
				if (cr != null)
				{
					return (this.WholeId == cr.WholeId);
				}
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		/// <summary>
		/// since this class is only used by a hosted component which is a private member of
		/// its hosting class, its code is a variable reference
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		[Browsable(false)]
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			ClassInstancePointer cp = _hostingClass as ClassInstancePointer;
			if (cp != null)
			{
				CodeExpression co = cp.GetReferenceCode(method, statements, forValue);
				return new CodeFieldReferenceExpression(co, Name);
			}
			else
			{
				if (_memberId == 0)
				{
					if (_definingClass != null)
					{
						if (typeof(DrawingPage).IsAssignableFrom(_definingClass.BaseClassType))
						{
							return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(_definingClass.TypeString), DrawingPage.DEFAULTFORM);
						}
					}
				}
				ClassPointer root = _hostingClass as ClassPointer;
				if (root != null)
				{
					if (root.IsStatic)
					{
						return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(root.TypeString), Name);
					}
					else
					{
						return new CodeArgumentReferenceExpression(Name);
					}
				}
				else
				{
					return new CodeArgumentReferenceExpression(Name);
				}
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return Name;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return Name;
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		[Browsable(false)]
		public UInt32 GetClassIdFromXmlNode(XmlNode node)
		{
			XmlNode idNode = node.SelectSingleNode("Property[@name='ComponentId']");
			if (idNode == null)
			{
				throw new DesignerException("ComponentId not found");
			}
			return Convert.ToUInt32(idNode.InnerText);
		}
		protected void OnLoad(LimnorProject project, XmlNode classNode)
		{
			_iconFilename = SerializeUtil.ReadIconFilename(classNode);
		}
		/// <summary>
		/// load declaring class
		/// </summary>
		/// <param name="objMap"></param>
		/// <param name="objectNode"></param>
		[Browsable(false)]
		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
				XmlUtil.SetNameAttribute(objectNode, _name);
				XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_instanceId, _memberId);
				XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_ClassID, ClassId);
				ClassInstancePointer ip = _hostingClass as ClassInstancePointer;
				if (ip != null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(objectNode, XmlTags.XML_ClassInstance);
					ip.OnPostSerialize(objMap, nd, saved, serializer);
				}
				else
				{
					ClassPointer root = _hostingClass as ClassPointer;
					if (root != null)
					{
						if (root.ClassId != objMap.ClassId)
						{
							XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_hostClassId, root.ClassId);
						}
					}
				}
				//save custom property values
				if (_properties != null)
				{
					XmlObjectWriter writer = (XmlObjectWriter)serializer;
					PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(_properties.ToArray());
					writer.WriteProperties(objectNode, pdc, this, XmlTags.XML_PROPERTY);
				}
			}
			else
			{
				_name = XmlUtil.GetNameAttribute(objectNode);
				if (_memberId == 0)
				{
					_memberId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_instanceId);
				}
				XmlNode nd = objectNode.SelectSingleNode(XmlTags.XML_ClassInstance);
				if (nd == null)
				{
					//load _hostingClass
					if (_hostingClass == null)
					{
						ClassPointer root;
						UInt32 hostClassId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_hostClassId);
						if (hostClassId == 0)
						{
							root = ClassPointer.CreateClassPointer(objMap);
						}
						else
						{
							if (hostClassId == objMap.ClassId)
							{
								root = ClassPointer.CreateClassPointer(objMap);
							}
							else
							{
								root = ClassPointer.CreateClassPointer(hostClassId, objMap.Project);
							}
						}
						if (root == null)
						{
							throw new DesignerException("Invalid class id: {0}", hostClassId);
						}
						_hostingClass = root;
						if (_definingClassId == 0) //ClassId may be loaded by property deserialize
						{
							IClassRef r = root.ObjectList.GetClassRefById(MemberId);
							if (r != null)
							{
								_definingClassId = r.ClassId;
							}
						}
					}
					//load _definingClass
					if (_definingClass == null)
					{
						if (_definingClassId == 0)
						{
							_definingClassId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ClassID);
						}
						if (_definingClassId == 0)
						{
							ClassPointer root = (ClassPointer)_hostingClass;
							IClassRef r = root.ObjectList.GetClassRefById(MemberId);
							if (r != null)
							{
								_definingClassId = r.ClassId;
							}
						}
						_definingClass = ClassPointer.CreateClassPointer(_definingClassId, objMap.Project);
					}
				}
				else
				{
					_definingClassId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ClassID);
					ClassInstancePointer ip = new ClassInstancePointer();
					ip.OnPostSerialize(objMap, nd, saved, serializer);
					_hostingClass = ip;
					if (this.ClassId == 0) //ClassId may be loaded by property deserialize
					{
						if (_hostingClass.Definition.ObjectList.Count == 0)
						{
							_hostingClass.Definition.ObjectList.LoadObjects();
							if (_hostingClass.Definition.ObjectList.Reader.HasErrors)
							{
								MathNode.Log(_hostingClass.Definition.ObjectList.Reader.Errors);
								_hostingClass.Definition.ObjectList.Reader.ResetErrors();
							}
						}
						IClassRef r = _hostingClass.Definition.ObjectList.GetClassRefById(_memberId);
						_definingClassId = r.ClassId;
					}
					if (_definingClass == null)
					{
						_definingClass = ClassPointer.CreateClassPointer(_definingClassId, objMap.Project);
					}
				}
				if (MemberId == 0 && _definingClassId == 0)
				{
					//backward compatebility
					_definingClassId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ClassID);
					MemberId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ComponentID);
					_definingClass = objMap.Project.GetTypedData<ClassPointer>(_definingClassId);
					_hostingClass = objMap.GetTypedData<ClassPointer>();
				}
				if (_definingClass != null) //not being deleted
				{
					//load custom property definitions
					XmlObjectReader reader = _definingClass.ObjectList.Reader;
					if (reader == null)
					{
						reader = (XmlObjectReader)serializer;
					}
					LoadProperties(reader);
					//load custom property values
					LoadCustomPropertyValues(reader, objectNode);
				}
				if (MemberId == 0)
				{
					MemberId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ComponentID);
				}
			}
		}
		public void LoadCustomPropertyValues(XmlObjectReader reader, XmlNode objectNode)
		{
			if (_properties != null)
			{
				PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(_properties.ToArray());
				reader.ReadProperties(objectNode, this, pdc, XmlTags.XML_PROPERTY);
			}
		}
		public bool NameUsed(string name)
		{
			if (this.Name == name)
				return true;
			PropertyDescriptorCollection props = Properties;
			if (props != null)
			{
				foreach (PropertyDescriptor p in props)
				{
					if (p.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}
		public void LoadProperties(XmlObjectReader xr)
		{
			OnSetDefinition();
		}
		public void HookCustomPropertyValueChange(EventHandler h)
		{
			if (_properties == null)
			{
				//_definingClass == null => this instance is invalid
				if (_definingClass != null)
				{
					throw new DesignerException("properties is null when calling HookCustomPropertyValueChange");
				}
			}
			else
			{
				foreach (PropertyClassDescriptor p in _properties)
				{
					p.HookValueChange(h);
				}
			}
		}
		[Browsable(false)]
		public override string ToString()
		{
			return this.Name;
		}

		#endregion

		#region properties
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return Name;
			}
		}
		/// <summary>
		/// fully qualified variable name
		/// </summary>
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return Name;
			}
		}
		/// <summary>
		/// the definition of the class
		/// </summary>
		[Browsable(false)]
		public ClassPointer Definition
		{
			get
			{
				return _definingClass;
			}
		}
		[Browsable(false)]
		public EnumXType IsXType
		{
			get
			{
				if (this.ObjectInstance != null)
				{
					return VPLUtil.IsXType(this.ObjectInstance.GetType());
				}
				return EnumXType.None;
			}
		}
		/// <summary>
		/// the class hosting this instance
		/// </summary>
		[Browsable(false)]
		public IClass Host
		{
			get
			{
				return _hostingClass;
			}
		}
		[Browsable(false)]
		public ClassPointer RootHost
		{
			get
			{
				return _hostingClass.RootHost;
			}
		}
		[Browsable(false)]
		public List<PropertyClassDescriptor> CustomProperties
		{
			get
			{
				if (_properties == null || (_definingClass != null && _definingClass.EditVersion != _classVersion))
				{
					OnSetDefinition();
				}
				return _properties;
			}
		}

		/// <summary>
		/// A ClassInstancePointer represents an instance of ClassPointer. A ClassInstancePointer has 3 types of properties.
		/// They are placed in different XML elements.
		/// 1. ClassInstancePointer properties, ClassId, MemberId, etc. Placed under <ClassRef> element.
		/// 2. Properties of the lib type of the definition class ClassInstancePointer represents.
		///     Placed under <Object> or Property[name="Control"]\Item element.
		/// 3. Custom properties of the definition class ClassInstancePointer represents, including custom properties from the base classes.
		///     Also placed under <ClassRef> element.
		///     
		/// Properties includes type 2 and 3 properties, plus Name property. It is used for development showing on PropertyGrid.
		/// GetProperties(Attribute[]) includes type 1 and 3 properties. It is used for serialization on <ClassRef> element.
		/// </summary>
		[Browsable(false)]
		public PropertyDescriptorCollection Properties
		{
			get
			{
				Attribute[] attributes = new Attribute[] { };
				//type 3 properties
				int n3 = 0;
				if (_properties != null)
				{
					n3 = _properties.Count;
				}
				int n0 = 0;
				PropertyDescriptor nameProp = null;
				//type 2 properties
				int n1 = 0;
				PropertyDescriptorCollection ps = null;
				if (this.ObjectInstance != null)
				{
					object o = VPLUtil.GetObject(this.ObjectInstance);
					if (o == null)
					{
						Type t = VPLUtil.GetObjectType(this.ObjectInstance);
						if (t != null)
						{
							ps = TypeDescriptor.GetProperties(t, attributes);
							n1 = ps.Count;
						}
					}
					else
					{
						ps = TypeDescriptor.GetProperties(o, attributes, true);
						n1 = ps.Count;
					}

					//add Name property 
					PropertyInfo pifName = this.ObjectInstance.GetType().GetProperty("Name");
					nameProp = new PropertyInfoDescriptor<string>("Name", attributes, pifName, false, Name, true, this.ObjectInstance);
					n0 = 1;
				}

				PropertyDescriptor[] ps3 = new PropertyDescriptor[n1 + n3 + n0];
				if (n0 == 1)
				{
					ps3[0] = nameProp;
				}
				if (n3 > 0)
				{
					for (int i = 0; i < n3; i++)
					{
						ps3[i + n0] = _properties[i];
					}
				}
				if (n1 > 0)
				{
					ps.CopyTo(ps3, n3 + n0);
				}
				return new PropertyDescriptorCollection(ps3);
			}
		}
		[Browsable(false)]
		protected string IconFilename
		{
			get
			{
				if (string.IsNullOrEmpty(_iconFilename))
					return SerializeUtil.ReadIconFilename(_definingClass.XmlData);
				return _iconFilename;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public string TypeString
		{
			get
			{
				if (_definingClass != null)
					return _definingClass.TypeString;
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		//
		//[Browsable(false)]
		[ReadOnly(true)]
		[Description("Variable name for this instance")]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				IComponent ic = ObjectInstance as IComponent;
				if (ic != null)
				{
					if (ic.Site != null)
					{
						_name = ic.Site.Name;
						return _name;
					}
				}
				if (Site != null)
				{
					if (!string.IsNullOrEmpty(Site.Name))
					{
						_name = Site.Name;
						return _name;
					}
				}
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string Namespace
		{
			get
			{
				if (_definingClass != null)
					return _definingClass.Namespace;
				return "Limnor";
			}
			set
			{
				if (_definingClass != null)
					_definingClass.Namespace = value;
			}
		}

		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				string n = Name;
				if (_definingClass != null)
					return n + " of " + _definingClass.Name;
				if (!string.IsNullOrEmpty(_typeName))
					return n + " of " + _typeName;
				return n;
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				return Name;
			}
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[ReadOnly(true)]
		[Browsable(false)]
		public ISite Site
		{
			get
			{
				if (_site == null)
				{
					if (ObjectInstance != null)
					{
						IComponent ic = ObjectInstance as IComponent;
						if (ic != null)
						{
							return ic.Site;
						}
					}
				}
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
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
		/// <summary>
		/// A ClassInstancePointer represents an instance of ClassPointer. 
		/// ClassInstancePointer has 4 types of properties. They are placed in different XML elements.
		/// 1. ClassInstancePointer properties, including ClassId, MemberId, etc. Placed under <ClassRef> element.
		/// 2. Properties of the lib type of the definition class the ClassInstancePointer represents.
		///     Placed under <Object> or Property[name="Control"]\Item element.
		/// 3. Custom properties of the definition class the ClassInstancePointer represents.
		///     Also placed under <ClassRef> element.
		/// 4. Constructor selector, and constructor parameter values
		///     
		/// Properties includes type 2, 3 and 4 properties. It is used for development showing on PropertyGrid, via ClassProperties
		/// GetProperties(Attribute[]) includes type 1, 3 and 4 properties. It is used for serialization on <ClassRef> element.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			//type 1 properties
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			//type 3 properties, remove read-only properties
			List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
			if (_properties != null && _properties.Count > 0)
			{
				for (int i = 0; i < _properties.Count; i++)
				{
					if (!_properties[i].IsReadOnly)
					{
						pds.Add(_properties[i]);
					}
				}
			}
			PropertyDescriptor[] props;
			if (ps == null)
				props = new PropertyDescriptor[pds.Count];
			else
				props = new PropertyDescriptor[ps.Count + pds.Count];
			//custom properties first
			for (int i = 0; i < pds.Count; i++)
			{
				props[i] = pds[i];
			}
			if (ps != null)
			{
				for (int i = 0; i < ps.Count; i++)
				{
					props[i + pds.Count] = ps[i];
				}
			}
			return new PropertyDescriptorCollection(props);
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

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// or the top class for multi-lavel hosting
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				return _hostingClass.Definition;
			}
		}
		/// <summary>
		/// do not support static-hosted-component.
		/// a class can be used as a static property represented by ClassPointer 
		/// in a DataTypePointer of PropertyClass, not by this class
		/// </summary>
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_definingClass != null)
					return _definingClass.ObjectType;
				return null;
			}
			set
			{
				if (_definingClass != null)
					_definingClass.ObjectType = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				if (_hostingClass != null)
				{
					return _hostingClass.Definition.ObjectList.GetObjectByID(MemberId);
				}
				return null;
			}
			set
			{
				//_instance = value;
				MathNode.Log("Do not set class instance");
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_hostingClass != null && _definingClass != null && _memberId != 0)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Some of (_hostingClass, _definingClass, _memberId) are null for [{0}] of [{1}]. ({2},{3},{4})", this.ToString(), this.GetType().Name, _hostingClass, _definingClass, _memberId);
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get;
			set;
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return WholeId.ToString("x", CultureInfo.InvariantCulture); }
		}

		#endregion

		#region IObjectIdentity Members

		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return _hostingClass; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Both; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Class; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ClassInstancePointer ip = new ClassInstancePointer();
			ip._hostingClass = _hostingClass;
			ip._definingClass = _definingClass;
			ip._definingClassId = _definingClassId;
			ip._memberId = _memberId;
			ip.ImageIcon = this.ImageIcon;
			ip._name = _name;
			ip._icon = _icon;
			ip._typeName = _typeName;
			ip._site = _site;
			ip._iconFilename = _iconFilename;
			ip._properties = _properties;
			return ip;
		}

		#endregion

		#region IClassRef Members
		/// <summary>
		/// for class using this instance as a component
		/// </summary>
		[Browsable(false)]
		public UInt32 InstanceHostClassId
		{
			get
			{
				if (_hostingClass != null)
					return _hostingClass.ClassId;
				return 0;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				return _hostingClass;
			}
			set
			{
				if (value == this)
				{
					throw new DesignerException("Cannot set the owner to itself. [{0}]", this);
				}
				_hostingClass = (IClassContainer)value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(_memberId, _definingClassId);
			}
			set
			{
				DesignUtil.ParseDDWord(value, out _memberId, out _definingClassId);
			}
		}
		/// <summary>
		/// the class id for the declaring class
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				return _definingClassId;
			}
			set
			{
				_definingClassId = value;
			}
		}
		[Browsable(false)]
		public UInt32 DefinitionClassId
		{
			get
			{
				return ClassId;
			}
		}
		/// <summary>
		/// the instance id unique among the instance host
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				return _memberId;
			}
			set
			{
				_memberId = value;
			}
		}
		#endregion

		#region IClass Members
		[ReadOnly(true)]
		[Browsable(false)]
		public Image ImageIcon
		{
			get
			{
				if (_icon == null)
				{
					if (_definingClass != null)
					{
						_icon = _definingClass.ImageIcon;
					}
				}
				return _icon;
			}
			set
			{
				if (_definingClass != null)
					_definingClass.ImageIcon = value;
				_icon = value;
			}
		}
		[Browsable(false)]
		public Type VariableLibType
		{
			get { return null; }
		}
		[Browsable(false)]
		public ClassPointer VariableCustomType
		{
			get { return _definingClass; }
		}
		[Browsable(false)]
		public IClassWrapper VariableWrapperType
		{
			get { return null; }
		}

		#endregion
		#region Custom Property Value Serialization
		private XmlNode getClassRefNode(XmlNode rootNode)
		{
			XmlNode node = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}' and @{3}='{4}']", XmlTags.XML_ClassRef, XmlTags.XMLATT_ClassID, ClassId, XmlTags.XMLATT_ComponentID, MemberId));
			if (node == null)
			{
				node = rootNode.OwnerDocument.CreateElement(XmlTags.XML_ClassRef);
				rootNode.AppendChild(node);
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ClassID, ClassId);
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ComponentID, MemberId);
			}
			return node;
		}
		private XmlNode getPropertyNode(XmlNode node, string name)
		{
			XmlNode nodeProp = node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, name));
			if (nodeProp == null)
			{
				nodeProp = node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				node.AppendChild(nodeProp);
			}
			return nodeProp;
		}
		public void SaveCustomPropertyValue(XmlObjectWriter writer, string name, XmlNode rootNode)
		{
			PropertyClassDescriptor p = null;
			if (_properties == null)
			{
				throw new DesignerException("Saving property {0} for class [{1},{2}]: custom properties not loaded", name, ClassId, MemberId);
			}
			foreach (PropertyClassDescriptor pr in _properties)
			{
				if (pr.Name == name)
				{
					p = pr;
					break;
				}
			}
			if (p == null)
			{
				throw new DesignerException("Saving property {0} for class [{1},{2}]: custom property not found", name, ClassId, MemberId);
			}
			saveCustomPropertyValue(writer, p, rootNode);
		}

		private void saveCustomPropertyValue(XmlObjectWriter writer, PropertyClassDescriptor p, XmlNode rootNode)
		{
			XmlNode node = getClassRefNode(rootNode);
			XmlNode nodeProp = getPropertyNode(node, p.Name);
			if (p.CustomProperty.PropertyType.IsLibType)
			{
				DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)p.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
				writer.WriteProperty(p, nodeProp, this, visibility.Visibility, XmlTags.XML_PROPERTY);
			}
		}
		#endregion
		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			OnPostSerialize(((XmlObjectWriter)writer).ObjectList, node, true, writer);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			OnPostSerialize(((XmlObjectReader)reader).ObjectList, node, false, reader);
		}

		#endregion
	}

}
