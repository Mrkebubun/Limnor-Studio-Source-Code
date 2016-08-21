/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.ComponentModel;
using XmlSerializer;
using System.Xml;
using MathExp;
using XmlUtility;
using ProgElements;
using LimnorDesigner.Interface;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using TraceLog;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// a pointer to a PropertyClass
	/// 1. represent a custom property of an instance of ClassPointer as a member of another ClassPointer;
	/// 2. for PropertyClass selection
	/// 3. in setter and getter, represent the related custom property
	/// </summary>
	public class CustomPropertyPointer : IProperty, IXmlNodeSerializable, ICustomObject, ICustomPointer, ISourceValuePointer
	{
		#region fields and constructors
		private IClass _holder;
		private UInt64 _wholeId;
		private UInt32 _taskId;
		private PropertyClass _property;//getter/setter are not loaded
		public CustomPropertyPointer()
		{
		}
		public CustomPropertyPointer(PropertyClass p, IClass holder)
		{
			_property = p;
			_wholeId = p.WholeId;
			_holder = holder;
		}
		#endregion
		#region Properties
		/// <summary>
		/// the instance of ClassPointer as a member of another ClassPointer
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IClass PropertyHolder
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
		[Browsable(false)]
		[ReadOnly(true)]
		public PropertyClass Property
		{
			get
			{
				return _property;
			}
		}
		#endregion
		#region Methods
		public void SetHolder(IClass holder)
		{
			_holder = holder;
		}
		public SetterPointer CreateSetterMethodPointer(IAction act)
		{
			SetterPointer mp = new SetterPointer(act);
			mp.SetProperty = this;
			return mp;
		}
		public void SetProperty(PropertyClass p)
		{
			_property = p;
		}
		public override string ToString()
		{
			return Name;
		}
		#endregion
		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_property != null)
				{
					return _property.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				IObjectPointer root = this.Holder;
				while (root != null && root.Owner != null)
				{
					root = root.Owner;
				}
				ClassPointer c = root as ClassPointer;
				return c;
			}
		}
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
				if (Holder != null)
					return Holder.ReferenceName + "." + Name;
				return Name;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (Property != null)
				{
					return Property.IsStatic;
				}
				return false;
			}
		}
		/// <summary>
		/// the declarer of the property
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				if (Property != null)
				{
					return Property.Owner;
				}
				return null;
			}
			set
			{
				if (Property != null)
				{
					Property.Owner = value;
				}
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				if (Property != null)
				{
					return Property.ObjectType;
				}
				return null;
			}
			set
			{
				if (Property != null)
				{
					Property.ObjectType = value;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				if (Property != null)
				{
					return Property.ObjectInstance;
				}
				return null;
			}
			set
			{
				if (Property != null)
				{
					Property.ObjectInstance = value;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectDebug
		{
			get
			{
				if (Property != null)
				{
					return Property.ObjectDebug;
				}
				return null;
			}
			set
			{
				if (Property != null)
				{
					Property.ObjectDebug = value;
				}
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (Property != null)
				{
					return Property.DisplayName;
				}
				return "?";
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
				if (Property != null)
				{
					if (Property.Owner != null)
					{
						if (typeof(LimnorWebApp).IsAssignableFrom(Property.Owner.ObjectType))
						{
							return string.Format(CultureInfo.InvariantCulture, "WebApp.{0}", Property.Name);
						}
					}
					return Property.Name;
				}
				return "?";
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (Property != null)
			{
				return Property.IsTargeted(target);
			}
			return false;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (Property != null)
				{
					return Property.ObjectKey;
				}
				return null;
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				if (Property != null)
				{
					return Property.TypeString;
				}
				return null;
			}
		}

		public virtual CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (Property != null)
			{
				bool isWeb = false;
				if (this.RootPointer != null)
				{
					isWeb = this.RootPointer.IsWebObject;
				}
				else if (this.Property.RootPointer != null)
				{
					isWeb = this.Property.RootPointer.IsWebObject;
				}
				if (isWeb)
				{
					if (forValue && Property.RunAt == EnumWebRunAt.Client)
					{
						if (this.ObjectType.IsArray)
						{
							CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
									new CodeVariableReferenceExpression("clientRequest"), "GetStringArrayValue", new CodePrimitiveExpression(DataPassingCodeName));
							return cmie;
						}
						else
						{
							CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
									new CodeVariableReferenceExpression("clientRequest"), "GetStringValue", new CodePrimitiveExpression(DataPassingCodeName));
							return cmie;
						}
					}
				}
				bool inside = false;
				SetterClass sc = method as SetterClass;
				if (sc != null)
				{
					inside = sc.Property.IsSameObjectRef(Property);
				}
				else
				{
					GetterClass gc = method as GetterClass;
					if (gc != null)
					{
						inside = gc.Property.IsSameObjectRef(Property);
					}
				}
				if (inside)
				{
					CodeExpression ownerCode;
					MathNodePropertyField.CheckDeclareField(Property.IsStatic, Property.FieldMemberName, Property.PropertyType, method.TypeDeclaration, Property.DefaultValue);
					if (Property.IsStatic)
					{
						ownerCode = new CodeTypeReferenceExpression(Property.Holder.TypeString);
					}
					else
					{
						ownerCode = new CodeThisReferenceExpression();
					}
					return new CodeFieldReferenceExpression(ownerCode, Property.FieldMemberName);
				}
				else
				{
					Property.SetHolder(_holder);
					return Property.GetReferenceCode(method, statements, forValue);
				}
			}
			return null;
		}
		public virtual string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (Property != null)
			{
				if (Property.RunAt == EnumWebRunAt.Client)
				{
					if (Property.Owner != null)
					{
						if (typeof(LimnorWebApp).IsAssignableFrom(Property.Owner.ObjectType))
						{
							return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Property.Owner.CodeName, Property.CodeName);
						}
					}
					if (this.RootPointer.ClassId == this.Property.RootPointer.ClassId)
					{
						return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.PageValues.{0}", Property.CodeName);
					}
					else
					{
						return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getPropertyByPageId({0},'{1}')", this.Property.RootPointer.ClassId, Property.CodeName);
					}
				}
				else
				{
					if (this.RootPointer.ClassId == this.Property.RootPointer.ClassId)
					{
						return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", this.DataPassingCodeName);
					}
					else
					{
						return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getServerPropertyByPageId({0},'{1}')", this.Property.RootPointer.ClassId, this.DataPassingCodeName);
					}
				}
			}
			return null;
		}
		public virtual string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (Property != null)
			{
				if (Property.RunAt == EnumWebRunAt.Client)
				{
					return string.Format(CultureInfo.InvariantCulture, "$this->jsonFromClient->values->{0}", this.DataPassingCodeName);
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "$this->{0}", Property.CodeName);
				}
			}
			return null;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			IJavascriptType ijt = this.ObjectInstance as IJavascriptType;
			if (ijt == null)
			{
				Type t = this.ObjectInstance as Type;
				if (t != null)
				{
					if (t.GetInterface("IJavascriptType") != null)
					{
						ijt = Activator.CreateInstance(t) as IJavascriptType;
					}
				}
			}
			if (ijt != null)
			{
				string m = ijt.GetJavascriptMethodRef(this.GetJavaScriptReferenceCode(code), methodName, code, parameters);
				if (!string.IsNullOrEmpty(m))
				{
					if (string.IsNullOrEmpty(returnReceiver))
					{
						code.Add(string.Format(CultureInfo.InvariantCulture, "{0};\r\n", m));
					}
					else
					{
						code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", returnReceiver, m));
					}
					return;
				}
			}
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (Property != null && Property.IsValid)
				{
					return true;
				}
				if (Property == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Property is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}
		#endregion

		#region IObjectIdentity Members

		public virtual bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ICustomObject cp = objectIdentity as ICustomObject;
			if (cp != null)
			{
				return (cp.WholeId == this.WholeId);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(ISourceValuePointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (Property != null)
				{
					return Property.IdentityOwner;
				}
				return null;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Custom; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Property; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CustomPropertyPointer cp = new CustomPropertyPointer(_property, _holder);
			return cp;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IProperty Members
		public bool IsReadOnly { get { return !this.CanWrite; } }
		[Description("Public: all objects can access it; Protected: only this class and its derived classes can access it; Private: only this class can access it.")]
		public EnumAccessControl AccessControl
		{
			get
			{
				if (_property != null)
				{
					return _property.AccessControl;
				}
				return EnumAccessControl.Public;
			}
			set
			{
				if (_property != null)
				{
					_property.AccessControl = value;
				}
			}
		}
		public DataTypePointer PropertyType
		{
			get
			{
				if (_property != null)
				{
					return _property.PropertyType;
				}
				return null;
			}
		}
		public DataTypePointer[] GetConcreteTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.TypeParameters;
			}
			return null;
		}
		public IList<DataTypePointer> GetGenericTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetGenericTypes();
			}
			return null;
		}
		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetConcreteType(typeParameter);
			}
			return null;
		}
		public CodeTypeReference GetCodeTypeReference()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetCodeTypeReference();
			}
			return null;
		}
		/// <summary>
		/// change the property name without trigging events
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			if (_property != null)
				_property.SetName(name);
		}
		/// <summary>
		/// the instance of the property declarer
		/// </summary>
		[Browsable(false)]
		public ClassPointer Declarer
		{
			get
			{
				if (_property != null)
				{
					return _property.Declarer;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IClass Holder
		{
			get { return _holder; }
		}
		[Browsable(false)]
		public string Name
		{
			get
			{
				if (_property != null)
				{
					return _property.Name;
				}
				return null;
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
			get
			{
				if (_property != null)
				{
					return _property.Implemented;
				}
				return false;
			}
		}
		[Browsable(false)]
		public void SetValue(object value)
		{
			if (_property != null)
			{
				_property.SetValue(value);
			}
		}
		[Browsable(false)]
		public IList<Attribute> GetUITypeEditor()
		{
			return null;
		}
		#endregion

		#region IXmlNodeSerializable Members

		public virtual void OnWriteToXmlNode(IXmlCodeWriter writer0, XmlNode node)
		{
			XmlObjectWriter writer = writer0 as XmlObjectWriter;
			if (writer == null)
			{
				throw new DesignerException("Error writing CustomPropertyPointer. writer0 is not a XmlObjectWriter");
			}
			if (writer.ObjectList == null)
			{
				throw new DesignerException("Error writing CustomPropertyPointer. writer0 does not contain object map");
			}
			if (node == null)
			{
				throw new DesignerException("Error writing CustomPropertyPointer. Target XmlNode is null.");
			}
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_PropId, MemberId);
			if (ClassId != writer.ObjectList.ClassId)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ownerClassID, ClassId);
			}
			LocalVariable var = _holder as LocalVariable;
			if (var != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_varId, _holder.MemberId);
			}
			else
			{
				ClassInstancePointer cp = _holder as ClassInstancePointer;
				if (cp != null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_ClassInstance);
					cp.OnPostSerialize(writer.ObjectList, nd, true, writer);
				}
			}
		}
		private string getDebugInfo(IXmlCodeReader reader0, XmlNode node)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(Name))
			{
				sb.Append("name=");
				sb.Append(Name);
				sb.Append(";");
			}
			if (MemberId != 0)
			{
				sb.Append("MemberId=");
				sb.Append(MemberId);
				sb.Append(";");
			}
			if (ClassId != 0)
			{
				sb.Append("ClassId=");
				sb.Append(ClassId);
				sb.Append(";");
			}
			if (node != null)
			{
				sb.Append("xml node=");
				sb.Append(XmlUtil.GetPath(node));
				sb.Append(";");
				XmlNode root = node.OwnerDocument.DocumentElement;
				string s = XmlUtil.GetNameAttribute(root);
				sb.Append("class=");
				sb.Append(s);
				sb.Append(";");
				UInt32 cid = XmlUtil.GetAttributeUInt(root, XmlTags.XMLATT_ClassID);
				sb.Append("classId=");
				sb.Append(cid);
				sb.Append(";");
			}
			return sb.ToString();
		}
		/// <summary>
		/// called from the holding class
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="node"></param>
		public virtual void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)reader0;
			//retrieve _holder, _property
			UInt32 memberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_PropId);//declaring property id
			MemberId = memberId; //id PropertyClass 
			//
			UInt32 varId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_varId);
			if (varId != 0)
			{
				ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
				if (root0 == null)
				{
					root0 = reader.ObjectList.Project.GetTypedData<ClassPointer>(reader.ObjectList.ClassId);
					if (root0 == null)
					{
						root0 = reader.ObjectList.RootPointer as ClassPointer;
						if (root0 == null)
						{
							MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Class Id = {0}. More info:{1}", reader.ObjectList.ClassId, getDebugInfo(reader0, node)));
						}
					}
				}
				if (root0 != null)
				{
					LocalVariable lv = root0.GetLocalVariable(varId);
					_holder = lv;
					if (lv.ClassType == null)
					{
						MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Local variable [{0}]'s data type is null. More info:{1}", lv, getDebugInfo(reader0, node)));
					}
					else
					{
						ClassId = lv.ClassType.DefinitionClassId;
						ClassPointer r = reader.ObjectList.Project.GetTypedData<ClassPointer>(ClassId);
						if (r == null)
						{
							MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Class ID [{0}] of a local variable [{1}] does not point to a class. More info:{2}", ClassId, lv, getDebugInfo(reader0, node)));
						}
						else
						{
							_property = r.GetCustomPropertyById(memberId);
						}
					}
				}
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XmlTags.XML_ClassInstance);
				if (nd != null)
				{
					ClassInstancePointer cp = new ClassInstancePointer();
					cp.OnPostSerialize(reader.ObjectList, nd, false, reader);
					if (cp.Definition == null && cp.DefinitionClassId == 0)
					{
						MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer: Definition ID is 0. More info:{0}", getDebugInfo(reader0, node)));
					}
					else
					{
						if (cp.Definition == null)
						{
							MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer: Definition class not found. Definition class ID = {0}. If the class was removed from the project then the places using the class should be modified. More info:{1}", cp.DefinitionClassId, getDebugInfo(reader0, node)));
						}
						else
						{
							_property = cp.Definition.GetCustomPropertyById(memberId);
						}
						_holder = cp;
						ClassId = cp.DefinitionClassId;
					}
				}
				else
				{
					UInt32 cid = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ownerClassID);
					if (cid != 0)
					{
						ClassPointer root0 = ClassPointer.CreateClassPointer(cid, reader.ObjectList.Project);
						_holder = root0;
						if (root0 == null)
						{
							MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Owner class ID [{0}] does not point to a class. More info:{1}", cid, getDebugInfo(reader0, node)));
						}
						else
						{
							ClassId = root0.ClassId;
							_property = root0.GetCustomPropertyById(memberId);
						}
					}
					else
					{
						if (XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID) == 0)
						{
							ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
							_holder = root0;
							if (root0 == null)
							{
								root0 = reader.ObjectList.Project.GetTypedData<ClassPointer>(reader.ObjectList.ClassId);
								if (root0 == null)
								{
									root0 = reader.ObjectList.RootPointer as ClassPointer;
									if (root0 == null)
									{
										MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Current class ID [{0}] does not point to a class. More info:{1}", reader.ObjectList.ClassId, getDebugInfo(reader0, node)));
									}
								}
							}
							if (root0 != null)
							{
								ClassId = root0.ClassId;
								_property = root0.GetCustomPropertyById(memberId);
							}
						}
					}
				}
			}
			if (_holder == null)
			{
				//==backward compatibility===========================================================
				UInt32 classId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);//declaring class id
				UInt32 holderMemberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ComponentID);
				ClassId = classId;   //definition class
				//root holding class
				ClassPointer root = reader.ObjectList.GetTypedData<ClassPointer>();
				if (root == null)
				{
					root = reader.ObjectList.Project.GetTypedData<ClassPointer>(reader.ObjectList.ClassId);
					if (root == null)
					{
						root = reader.ObjectList.RootPointer as ClassPointer;
						if (root == null)
						{
							MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer. Holding class, class ID [{0}], does not point to a class. More info:{1}", reader.ObjectList.ClassId, getDebugInfo(reader0, node)));
						}
					}
				}
				//
				UInt32 holderClassId = XmlUtil.GetAttributeUInt(node.OwnerDocument.DocumentElement, XmlTags.XMLATT_ClassID);
				if (holderClassId == classId) //holder is in the same class
				{
					//
					if (root != null)
					{
						_property = root.GetCustomPropertyById(MemberId);
						if (_property != null && _property.IsStatic)
						{
							_holder = root;
						}
					}
				}
				else //holder and declarer are different classes
				{
					//root is the holding class
					ClassPointer declarer = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
					if (declarer == null)
					{
						MathNode.Log(TraceLogClass.MainForm, new DesignerException("Error reading CustomPropertyPointer: classId {0} does not point to a class. If the class was removed from the class then please modify places using the class. More info:{1}", classId, getDebugInfo(reader0, node)));
					}
					else
					{
						_property = declarer.GetCustomPropertyById(memberId);
						if (_property != null && _property.IsStatic)
						{
							_holder = declarer;
						}
					}
				}
				if (_property != null && !_property.IsStatic)
				{
					if (holderMemberId == 0 || holderMemberId == reader.ObjectList.MemberId)//not an instance
					{
						_holder = root;
					}
					else
					{
						_holder = (IClass)reader.ObjectList.GetClassRefById(holderMemberId);
					}
					if (_holder == null)
					{
						//holder is a local variable
						LocalVariable lv = root.GetLocalVariable(holderMemberId);
						_holder = lv;
					}
					_property.SetHolder(_holder);
				}
			}

			if (_holder == null)
			{
				MathNode.Log(TraceLogClass.MainForm, new DesignerException("Invalid property pointer [{0}]. More info:{1}", node.InnerXml, getDebugInfo(reader0, node)));
			}
		}

		#endregion

		#region ICustomObject Members
		/// <summary>
		/// class id for referencing the PropertyClass
		/// </summary>
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				return c;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				_wholeId = DesignUtil.MakeDDWord(a, value);
			}
		}
		/// <summary>
		/// property id for referencing the PropertyClass
		/// </summary>
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				return a;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				_wholeId = DesignUtil.MakeDDWord((UInt32)value, c);
			}
		}
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				return _wholeId;
			}
		}
		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public bool CanWrite
		{
			get
			{
				if (_property != null)
				{
					return _property.CanWrite;
				}
				return false;
			}
		}
		public UInt32 TaskId { get { return _taskId; } }
		public void SetTaskId(UInt32 taskId)
		{
			_taskId = taskId;
		}
		public bool IsWebClientValue()
		{
			if (RunAt == EnumWebRunAt.Client)
			{
				return true;
			}
			if (RunAt == EnumWebRunAt.Server)
			{
				return false;
			}
			if (_property != null)
			{
				if (DesignUtil.IsWebClientObject(_property.Declarer))
				{
					return true;
				}
			}
			return false;
		}
		public bool IsWebServerValue()
		{
			if (RunAt == EnumWebRunAt.Server)
			{
				return true;
			}
			if (RunAt == EnumWebRunAt.Client)
			{
				return false;
			}
			if (_property != null)
			{
				if (DesignUtil.IsWebServerObject(_property.Declarer))
				{
					return true;
				}
			}
			return false;
		}
		public string CreateJavaScript(StringCollection method)
		{
			return GetJavaScriptReferenceCode(method);
		}
		public string CreatePhpScript(StringCollection method)
		{
			return GetPhpScriptReferenceCode(method);
		}
		public void SetValueOwner(object o)
		{
			if (Owner == null)
			{
				IObjectPointer p = o as IObjectPointer;
				if (p != null)
				{
					Owner = p;
				}
			}
		}
		[Browsable(false)]
		public object ValueOwner
		{
			get
			{
				return Owner;
			}
		}
		[Browsable(false)]
		public string DataPassingCodeName
		{
			get
			{
				return CompilerUtil.CreateJsCodeName(this, _taskId);
			}
		}
		#endregion
	}
	/// <summary>
	/// for overriden property, allow pointing to the base version
	/// </summary>
	public class CustomPropertyOverridePointer : CustomPropertyPointer
	{
		#region fields and constructors
		const string XMLATT_FromBase = "useBaseValue";
		const string XMLATT_BaseClassId = "baseClassId";
		const string XMLATT_BasePropId = "basePropId";
		public CustomPropertyOverridePointer()
		{
		}
		public CustomPropertyOverridePointer(PropertyOverride p, IClass holder)
			: base(p, holder)
		{
		}
		#endregion
		[Description("If this property is True then the value from the base class is used")]
		public bool UseBaseValue
		{
			get;
			set;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (Property != null)
			{
				bool inside = false;
				SetterClass sc = method as SetterClass;
				if (sc != null)
				{
					inside = sc.Property.IsSameObjectRef(Property);
				}
				else
				{
					GetterClass gc = method as GetterClass;
					if (gc != null)
					{
						inside = gc.Property.IsSameObjectRef(Property);
					}
				}
				if (inside)
				{
					CodeExpression ownerCode;
					if (Property.IsStatic)
					{
						MathNodePropertyField.CheckDeclareField(Property.IsStatic, Property.FieldMemberName, Property.PropertyType, method.TypeDeclaration, Property.DefaultValue);
						ownerCode = new CodeTypeReferenceExpression(Property.Holder.TypeString);
						return new CodeFieldReferenceExpression(ownerCode, Property.FieldMemberName);
					}
					else
					{
						ownerCode = new CodeBaseReferenceExpression();
						return new CodeFieldReferenceExpression(ownerCode, Property.Name);
					}
				}
				else
				{
					Property.SetHolder(Holder);
					return Property.GetReferenceCode(method, statements, forValue);
				}
			}
			return null;
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "base.{0}", Name);
		}
		public override bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (objectIdentity is CustomPropertyOverridePointer)
			{
				return true;
			}
			if (objectIdentity is ParameterClassBaseProperty)
			{
				return true;
			}
			return false;
		}
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_FromBase, UseBaseValue);
			PropertyOverride po = (PropertyOverride)Property;
			XmlUtil.SetAttribute(node, XMLATT_BaseClassId, po.BaseClassId);
			XmlUtil.SetAttribute(node, XMLATT_BasePropId, po.BasePropertyId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			UseBaseValue = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_FromBase);
		}
		#endregion
	}
}
