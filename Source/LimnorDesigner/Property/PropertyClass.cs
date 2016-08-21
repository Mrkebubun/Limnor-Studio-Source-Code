/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.ComponentModel;
using System.CodeDom;
using XmlSerializer;
using System.Drawing.Design;
using XmlUtility;
using System.Xml;
using VPL;
using MathExp;
using ProgElements;
using LimnorDesigner.Action;
using LimnorDesigner.Interface;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using System.Globalization;
using Limnor.WebBuilder;
using LimnorDesigner.DesignTimeType;
using Limnor.PhpComponents;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// define a custom property
	/// </summary>
	[UseParentObject]
	public class PropertyClass : IObjectPointer, IWithProject, INonHostedObject, IPropertyEx, ICustomObject, ICustomPointer, IAttributeHolder, ICustomTypeDescriptor
	{
		#region fields and constructors
		private UInt32 _memberId;
		private string _name = "Property";
		private DataTypePointer _propertyType;
		private ClassPointer _owner;
		private IClass _holder;
		private SetterClass _setter;
		private GetterClass _getter;
		private bool _canRead = true;
		private bool _canWrite = true;
		private bool _isFinal;
		private bool _isStatic;
		private string _desc;
		private string _defValue;
		public PropertyClass(ClassPointer owner)
		{
			_owner = owner;
		}
		#endregion

		#region Static Utility
		public static UInt32 GetPropertyHolderClassId(IProperty p, UInt32 defaultId)
		{
			MemberComponentIdCustom mcc = p.Holder as MemberComponentIdCustom;
			if (mcc != null)
			{
				return mcc.Pointer.ClassId;
			}
			IObjectPointer op = p.Holder;
			ICustomObject co = p.Holder as ICustomObject;
			while (co == null && op != null)
			{
				op = op.Owner;
				co = op as ICustomObject;
			}
			if (co != null)
				return co.ClassId;
			else
				return defaultId;
		}
		public static UInt32 GetPropertyHolderMemberId(IProperty p)
		{
			MemberComponentIdCustom mcc = p.Holder as MemberComponentIdCustom;
			if (mcc != null)
			{
				return mcc.Pointer.MemberId;
			}
			IObjectPointer op = p.Holder;
			ICustomObject co = p.Holder as ICustomObject;
			while (co == null && op != null)
			{
				op = op.Owner;
				co = op as ICustomObject;
			}
			if (co != null)
				return co.MemberId;
			else
				return 0;
		}
		#endregion

		#region IWithProject Members
		[Browsable(false)]
		public LimnorProject Project
		{
			get
			{
				if (_owner != null)
				{
					if (_owner.ObjectList != null)
					{
						return _owner.ObjectList.Project;
					}
				}
				return null;
			}
		}

		#endregion

		#region properties
		[Browsable(false)]
		public virtual bool HasBaseImplementation
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public virtual bool Implemented
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public virtual bool DoNotCompile
		{
			get
			{
				if (!CanWrite && !CanRead)
					return true;
				if (!Implemented)
					return true;
				return false;
			}
		}
		[DefaultValue(EnumAccessControl.Public)]
		[Description("Public: all objects can access it; Protected: only this class and its derived classes can access it; Private: only this class can access it.")]
		public virtual EnumAccessControl AccessControl
		{
			get;
			set;
		}
		[Browsable(false)]
		public virtual bool CanRemove
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public virtual MemberAttributes PropertyAttributes
		{
			get
			{
				MemberAttributes a;
				if (this.AccessControl == EnumAccessControl.Public)
				{
					a = MemberAttributes.Public;
				}
				else if (this.AccessControl == EnumAccessControl.Private)
				{
					a = MemberAttributes.Private;
				}
				else
				{
					a = MemberAttributes.Family;
				}
				if (IsStatic)
				{
					a |= MemberAttributes.Static;
				}
				else
				{
					if (IsAbstract)
					{
						a |= MemberAttributes.Abstract;
					}
					if (!IsFinal)
					{
						a |= MemberAttributes.VTableMask;
					}
				}
				return a;
			}
		}
		[DefaultValue(false)]
		[Browsable(false)]//hide it for this version
		public virtual bool IsAbstract
		{
			get;
			set;
		}
		[DefaultValue(false)]
		[Browsable(false)]//hide it for this version
		[Description("A final property cannot be overriden by a derived class")]
		public bool IsFinal
		{
			get
			{
				return _isFinal;
			}
			set
			{
				if (_isFinal != value)
				{
					_isFinal = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("IsFinal"));
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public virtual ClassPointer Declarer
		{
			get
			{
				return _owner;
			}
		}
		[Browsable(false)]
		public IClass Holder
		{
			get
			{
				if (_holder != null)
				{
					return _holder;
				}
				return (IClass)Owner;
			}
		}
		[Browsable(false)]
		public string FieldMemberName
		{
			get
			{
				return "valueOf" + _name;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool Changed { get; set; }

		[DefaultValue(false)]
		[Browsable(false)]//hide it for this version
		[Description("Indicates whether it is a static property shared across all instances of this class.")]
		public virtual bool IsStatic
		{
			get
			{
				return _isStatic;
			}
			set
			{
				if (_isStatic != value)
				{
					_isStatic = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("IsStatic"));
					}
				}
			}
		}
		[Description("Description of this property")]
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				if (_desc != value)
				{
					_desc = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("Description"));
					}
				}
			}
		}
		[ParenthesizePropertyName(true)]
		[Description("Property name")]
		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (_name != value)
				{
					bool cancel = false;
					if (NameChanging != null)
					{
						NameBeforeChangeEventArg nc = new NameBeforeChangeEventArg(_name, value, true);
						NameChanging(this, nc);
						cancel = nc.Cancel;
					}
					if (!cancel)
					{
						_name = value;
						if (PropertyChanged != null)
						{
							PropertyChanged(this, new PropertyChangeEventArg("Name"));
						}
					}
					OnNameSet();
				}
			}
		}
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("The data type of the property")]
		public virtual DataTypePointer PropertyType
		{
			get
			{
				if (_propertyType == null)
				{
					_propertyType = new DataTypePointer(new TypePointer(typeof(object), Declarer));
				}
				return _propertyType;
			}
			set
			{
				_propertyType = value;
				if (_propertyType != null)
				{
					_propertyType.TypeChanged += new EventHandler(_propertyType_OnTypeChange);
					OnPropertyTypeChanged();
				}
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangeEventArg("PropertyType"));
				}
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
		public IList<DataTypePointer> GetGenericTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetGenericTypes();
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _propertyType_OnTypeChange(object sender, EventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangeEventArg("PropertyType"));
			}
		}
		[PropertyReadOrder(100)]
		[NotForLightReadAttribute]
		[Editor(typeof(TypeEditorSetter), typeof(UITypeEditor))]
		[SaveAsObject]
		[Description("This is the method to set the value of this property")]
		public SetterClass Setter
		{
			get
			{
				return _setter;
			}
			set
			{
				_setter = value;
			}
		}
		[PropertyReadOrder(101)]
		[NotForLightReadAttribute]
		[Editor(typeof(TypeEditorGetter), typeof(UITypeEditor))]
		[SaveAsObject]
		[Description("This is the method to get the value of this property")]
		public GetterClass Getter
		{
			get
			{
				return _getter;
			}
			set
			{
				_getter = value;
			}
		}

		[DefaultValue(true)]
		[Description("Specifies whether the value of this property can be used in action parameters and math expressions")]
		public virtual bool CanRead
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
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("CanRead"));
					}
				}
			}
		}

		[DefaultValue(true)]
		[Description("Specifies whether the value of this property can be changed by a Set Property action")]
		public virtual bool CanWrite
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
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("CanWrite"));
					}
				}
			}
		}
		[Description("Gets and sets the initial value for the property")]
		public string DefaultValue
		{
			get
			{
				if (this.PropertyType != null)
				{
					if (_defValue == null)
					{
						object v = this.PropertyType.GetDefaultValue();
						if (v != null)
						{
							TypeConverter tc = TypeDescriptor.GetConverter(this.PropertyType.BaseClassType);
							if (tc != null && tc.CanConvertTo(typeof(string)))
							{
								_defValue = tc.ConvertTo(v, typeof(string)) as string;
							}
							else
							{
								_defValue = v.ToString();
							}
						}
					}
					else
					{
						bool b;
						VPLUtil.ConvertObject(_defValue, this.PropertyType.BaseClassType, out b);
						if (!b)
						{
							object v = this.PropertyType.GetDefaultValue();
							if (v != null)
							{
								object val = VPLUtil.ConvertObject(v, typeof(string), out b);
								if (b)
								{
									_defValue = val as string;
								}
								else
								{
									_defValue = string.Empty;
								}
							}
							else
							{
								_defValue = string.Empty;
							}
						}
					}
				}
				return _defValue;
			}
			set
			{
				if (_defValue != value)
				{
					_defValue = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("DefaultValue"));
					}
				}
			}
		}
		#endregion

		#region ICloneable Members

		public virtual object Clone()
		{
			ClassPointer owner = (ClassPointer)_owner.Clone();
			PropertyClass obj = (PropertyClass)Activator.CreateInstance(this.GetType(), owner);
			obj.IsStatic = this.IsStatic;
			obj.ClassId = ClassId;
			obj.MemberId = MemberId;
			obj.SetName(_name);
			obj.Description = Description;
			obj.CanRead = _canRead;
			obj.CanWrite = _canWrite;
			if (_propertyType != null)
			{
				obj._propertyType = (DataTypePointer)_propertyType.Clone();
			}
			if (_setter != null)
			{
				obj._setter = (SetterClass)_setter.Clone();
			}
			if (_getter != null)
			{
				obj._getter = (GetterClass)_getter.Clone();
			}
			return obj;
		}

		#endregion

		#region IObjectPointer Members
		public virtual EnumWebRunAt RunAt
		{
			get
			{
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
				if (Owner != null)
					return Owner.ReferenceName + "." + Name;
				return Name;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_propertyType != null && _owner != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(_propertyType={2}, _owner={3}) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, _propertyType, _owner);
				return false;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = (ClassPointer)value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				return PropertyType.DataTypeEx;
			}
			set
			{
				PropertyType.SetDataType(value);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				return PropertyType.ObjectInstance;
			}
			set
			{
				PropertyType.ObjectInstance = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				return PropertyType.ObjectDebug;
			}
			set
			{
				PropertyType.ObjectDebug = value;
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					if (_propertyType == null)
						return "?:?";
					return "?:" + _propertyType.DisplayName;
				}
				if (_propertyType == null)
					return _name + ":?";
				return _name + ":" + _propertyType.DisplayName;
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
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Owner.ObjectKey, MemberId.ToString("x", CultureInfo.InvariantCulture));
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				return _owner.TypeString + "." + _name;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <returns>CodePropertyReferenceExpression</returns>
		public virtual CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeExpression p;
			if (IsStatic)
			{
				p = new CodeTypeReferenceExpression(Declarer.TypeString);
			}
			else
			{
				p = Holder.GetReferenceCode(method, statements, forValue);
			}
			return new CodePropertyReferenceExpression(OnGetTargetObject(p), this.Name);
		}
		public virtual string GetJavaScriptReferenceCode(StringCollection code)
		{
			return Holder.GetJavaScriptReferenceCode(code);
		}
		public virtual string GetPhpScriptReferenceCode(StringCollection code)
		{
			return Holder.GetPhpScriptReferenceCode(code);
		}
		public virtual void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public virtual void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		protected virtual CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return targetObject;
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			PropertyClass mc = objectIdentity as PropertyClass;
			if (mc != null)
			{
				return (mc.WholeId == this.WholeId);
			}
			CustomPropertyPointer cp = objectIdentity as CustomPropertyPointer;
			if (cp != null)
			{
				return cp.WholeId == this.WholeId;
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
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return Owner;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Custom; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Property; } }
		#endregion

		#region ISerializerProcessor Members

		public virtual void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
				XmlUtil.RemoveAttribute(objectNode, XmlTags.XMLATT_New);
			}
			else
			{
				bool b = XmlUtil.GetAttributeBoolDefFalse(objectNode, XmlTags.XMLATT_New);
				if (b)
				{
					if (this.Getter != null)
					{
						this.Getter.IsNewMethod = b;
					}
					if (this.Setter != null)
					{
						this.Setter.IsNewMethod = b;
					}
				}
			}
		}

		#endregion

		#region Methods
		/// <summary>
		/// importing a page from PHP project to Aspx
		/// </summary>
		[Browsable(false)]
		[NotForProgramming]
		public void SwitchPhpType()
		{
			if (typeof(PhpString).IsAssignableFrom(this.ObjectType))
			{
				ObjectType = typeof(string);
			}
			else if(typeof(PhpArray).IsAssignableFrom(this.ObjectType))
			{
			}
		}
		protected virtual void OnNameSet()
		{
		}
		public PropertyInfoX GetPropertyInfoX()
		{
			return new PropertyInfoX(this);
		}
		public bool IsActionOwner(IAction act)
		{
			SetterPointer sp = act.ActionMethod as SetterPointer;
			if (sp != null)
			{
				CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
				if (cpp != null)
				{
					return (cpp.MemberId == this.MemberId);
				}
			}
			return false;
		}
		public void SetHolder(IClass holder)
		{
			_holder = holder;
		}
		public virtual CustomPropertyPointer CreatePointer()
		{
			return new CustomPropertyPointer(this, Holder);
		}
		public PropertyClassInherited CreateInherited(ClassPointer owner)
		{
			PropertyClassInherited pi = new PropertyClassInherited((ClassPointer)this.Owner);
			pi.ShiftOwnerDeclarer(owner);
			pi._canRead = _canRead;
			pi._canWrite = _canWrite;
			pi._desc = _desc;
			pi._isFinal = _isFinal;
			pi._name = _name;
			pi._propertyType = _propertyType;
			pi.AccessControl = AccessControl;
			pi._memberId = _memberId;
			pi._holder = owner;
			return pi;
		}

		/// <summary>
		/// change name without trigging events
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			_name = name;
		}
		public virtual void SetMembers(bool canRead, bool canWrite, EnumAccessControl access, DataTypePointer type)
		{
			_canRead = canRead;
			_canWrite = canWrite;
			AccessControl = access;
			_propertyType = type;
		}
		protected virtual void OnPropertyTypeChanged()
		{
			DataTypePointer type = PropertyType;
			if (Getter != null)
			{
				//CustomPropertyOverridePointer for base value
				Getter.ReturnValue.SetDataType(type);
				List<ComponentIcon> icons = Getter.ComponentIconList;
				if (icons != null)
				{
					foreach (ComponentIcon ic in icons)
					{
						ComponentIconParameter cp = ic as ComponentIconParameter;
						if (cp != null)
						{
							cp.SetParameterType(type);
							break;
						}
					}
				}

			}
			if (Setter != null)
			{
				foreach (ParameterClass pc in Setter.Parameters)
				{
					pc.SetDataType(type);
				}

				//PropertyValueClass
				//CustomPropertyOverridePointer for base value
				List<ComponentIcon> icons = Setter.ComponentIconList;
				if (icons != null)
				{
					foreach (ComponentIcon ic in icons)
					{
						ComponentIconParameter cp = ic as ComponentIconParameter;
						if (cp != null)
						{
							cp.SetParameterType(type);
						}
					}
				}
			}
		}
		public virtual SetterPointer CreateSetterMethodPointer(IAction act)
		{
			SetterPointer mp = new SetterPointer(act);
			mp.SetProperty = CreatePointer();
			return mp;
		}
		public override string ToString()
		{
			if (_propertyType == null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:?", Name);
			}
			else
			{
				if (_propertyType.IsLibType)
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", Name, _propertyType.LibTypePointer.ClassType.Name);
				else
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", Name, _propertyType.TypeString);
			}
		}
		#endregion

		#region INonHostedObject Members
		EventHandler NameChanging;
		EventHandler PropertyChanged;
		public void OnPropertyChanged(string name, object property, XmlNode rootNode, XmlObjectWriter writer)
		{
			XmlNode node = SerializeUtil.GetCustomPropertyNode(rootNode, this.MemberId);
			if (string.CompareOrdinal(name, "Name") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "Name");
				nd.InnerText = this.Name;
				//========================================================================
				Type t = typeof(MathNodePropertyField);
				Type tGet = typeof(ActionMethodReturn);
				Type tSet = typeof(ActionAssignment);
				//set getter/setter action names:Actions/Action/Property name="ActionMethod"/Data/Math propertyId="906060879"
				XmlNodeList actionNodeList = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}[@{3}='ActionMethod']/{4}/{5}[@{6}='{7}']",
					XmlTags.XML_ACTIONS,
					XmlTags.XML_ACTION,
					XmlTags.XML_PROPERTY,
					XmlTags.XMLATT_NAME,
					XmlTags.XML_Data,
					XmlTags.XML_Math,
					XmlTags.XMLATT_PropId,
					this.MemberId));
				foreach (XmlNode mnode in actionNodeList)
				{
					Type t1 = XmlUtil.GetLibTypeAttribute(mnode);
					if (t1 != null && t1.Equals(t))
					{
						XmlNode actNode = mnode.ParentNode;
						while (actNode != null && actNode.Name != XmlTags.XML_ACTION)
						{
							actNode = actNode.ParentNode;
						}
						t1 = XmlUtil.GetLibTypeAttribute(actNode);
						if (t1 != null)
						{
							if (t1.Equals(tGet))
							{
								XmlUtil.SetNameAttribute(actNode, "Get" + this.Name);
							}
							else if (t1.Equals(tSet))
							{
								XmlUtil.SetNameAttribute(actNode, "Set" + this.Name);
							}
						}
					}
				}
				//change existing action names
				if (writer.ObjectList != null)
				{
					ClassPointer aec = this.RootPointer;
					if (aec != null)
					{
						Dictionary<UInt32, IAction> acts = aec.GetActions();
						if (acts != null)
						{
							foreach (IAction a in acts.Values)
							{
								if (a != null && !a.IsPublic)
								{
									ActionMethodReturn am = a as ActionMethodReturn;
									if (am != null)
									{
										MathNodeRoot mr = am.ActionMethod as MathNodeRoot;
										if (mr != null)
										{
											MathNodePropertyField pf = mr[1] as MathNodePropertyField;
											if (pf != null)
											{
												if (pf.PropertyId == this.MemberId)
												{
													a.ActionName = "Get" + this.Name;
												}
											}
										}
									}
									else
									{
										ActionAssignment aa = a as ActionAssignment;
										if (aa != null)
										{
											MathNodeRoot mr = aa.ActionMethod as MathNodeRoot;
											if (mr != null)
											{
												MathNodePropertyField pf = mr[0] as MathNodePropertyField;
												if (pf != null)
												{
													if (pf.PropertyId == this.MemberId)
													{
														a.ActionName = "Set" + this.Name;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				//change all property name references
				XmlNodeList nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"//*[@{0}='{1}']",
					XmlTags.XMLATT_PropId, MemberId));
				foreach (XmlNode ndp in nodes)
				{
					XmlUtil.SetNameAttribute(ndp, Name);
				}
			}
			else if (string.CompareOrdinal(name, "CanRead") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "CanRead");
				nd.InnerText = this.CanRead.ToString();
			}
			else if (string.CompareOrdinal(name, "CanWrite") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "CanWrite");
				nd.InnerText = this.CanWrite.ToString();
			}
			else if (string.CompareOrdinal(name, "IsStatic") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "IsStatic");
				nd.InnerText = this.IsStatic.ToString();
			}
			else if (string.CompareOrdinal(name, "IsAbstract") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "IsAbstract");
				nd.InnerText = this.IsStatic.ToString();
			}
			else if (string.CompareOrdinal(name, "IsFinal") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "IsFinal");
				nd.InnerText = this.IsFinal.ToString();
			}
			else if (string.CompareOrdinal(name, "Description") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "Description");
				nd.InnerText = this.Description;
			}
			else if (string.CompareOrdinal(name, "DefaultValue") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "DefaultValue");
				nd.InnerText = this.DefaultValue;
			}
			else if (string.CompareOrdinal(name, "PropertyType") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "PropertyType");
				writer.WriteValue(nd, this.PropertyType, null);
			}
		}
		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			NameChanging = nameChange;
			PropertyChanged = propertyChange;
		}
		#endregion

		#region ICustomObject Members
		[ReadOnly(true)]
		[Browsable(false)]
		public virtual UInt64 WholeId
		{
			get
			{
				if (Declarer != null)
				{
					return DesignUtil.MakeDDWord(MemberId, Declarer.ClassId);
				}
				return DesignUtil.MakeDDWord(MemberId, ClassId);
			}
			set
			{
			}
		}

		[ReadOnly(true)]
		[Browsable(false)]
		public virtual UInt32 ClassId
		{
			get
			{
				if (_owner != null)
					return _owner.ClassId;
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public virtual UInt32 MemberId
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

		#region IProperty Members
		public bool IsReadOnly { get { return !this.CanWrite; } }
		[Browsable(false)]
		public Type CodeType
		{
			get
			{
				return this.ObjectType;
			}
		}
		[Browsable(false)]
		public bool IsCustomProperty
		{
			get { return true; }
		}
		[Browsable(false)]
		public virtual bool IsOverride
		{
			get { return false; }
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

		#region IAttributeHolder Members

		public void AddAttribute(ConstObjectPointer attr)
		{
			XmlNode propNode = SerializeUtil.GetCustomPropertyNode(_owner.XmlData, MemberId);
			if (propNode == null)
				throw new DesignerException("Property node not found for [{0},{1}] *", ClassId, MemberId);
			XmlNode node = SerializeUtil.CreateAttributeNode(propNode, attr.ValueId);
			XmlObjectWriter wr = new XmlObjectWriter(_owner.ObjectList);
			wr.WriteObjectToNode(node, attr);
		}

		public void RemoveAttribute(ConstObjectPointer attr)
		{
			XmlNode propNode = SerializeUtil.GetCustomPropertyNode(_owner.XmlData, MemberId);
			if (propNode == null)
				throw new DesignerException("Property node not found for [{0},{1}]. *", ClassId, MemberId);
			SerializeUtil.RemoveAttributeNode(propNode, attr.ValueId);
		}

		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			XmlNode propNode = SerializeUtil.GetCustomPropertyNode(_owner.XmlData, MemberId);
			if (propNode == null)
				throw new DesignerException("Property node not found for [{0},{1}].. *", ClassId, MemberId);
			List<ConstObjectPointer> attributes = new List<ConstObjectPointer>();
			XmlObjectReader xr = _owner.ObjectList.Reader;
			XmlNodeList nodes = SerializeUtil.GetAttributeNodeList(propNode);
			foreach (XmlNode nd in nodes)
			{
				ConstObjectPointer a = xr.ReadObject<ConstObjectPointer>(nd, this);
				attributes.Add(a);
			}
			if (xr.HasErrors)
			{
				MathNode.Log(xr.Errors);
			}
			return attributes;
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

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			bool isWebPage = false;
			if (this.RootPointer != null)
			{
				isWebPage = this.RootPointer.IsWebPage;
			}
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (VPLUtil.GetBrowseableProperties(attributes))
			{
				if (isWebPage)
				{
					List<PropertyDescriptor> list = new List<PropertyDescriptor>();
					foreach (PropertyDescriptor p in ps)
					{
						if (string.CompareOrdinal(p.Name, "Name") == 0)
						{
							list.Add(p);
						}
						else if (string.CompareOrdinal(p.Name, "Description") == 0)
						{
							list.Add(p);
						}
						else if (string.CompareOrdinal(p.Name, "DefaultValue") == 0)
						{
							list.Add(p);
						}
						else if (string.CompareOrdinal(p.Name, "RunAt") == 0)
						{
							list.Add(p);
						}
						else if (string.CompareOrdinal(p.Name, "PropertyType") == 0)
						{
							list.Add(p);
						}
					}
					ps = new PropertyDescriptorCollection(list.ToArray());
				}
				else
				{
					if (CanRead || CanWrite)
					{
						List<PropertyDescriptor> list = new List<PropertyDescriptor>();
						foreach (PropertyDescriptor p in ps)
						{
							if (string.CompareOrdinal(p.Name, "Getter") == 0 && (!CanRead || !Implemented))
							{
								continue;
							}
							if (string.CompareOrdinal(p.Name, "Setter") == 0 && (!CanWrite || !Implemented))
							{
								continue;
							}
							if (IsOverride)
							{
								if (string.CompareOrdinal(p.Name, "Name") == 0)
								{
									ReadOnlyPropertyDesc p0 = new ReadOnlyPropertyDesc(p);
									list.Add(p0);
									continue;
								}
								if (string.CompareOrdinal(p.Name, "Description") == 0)
								{
									if (!Implemented)
									{
										continue;
									}
								}
							}
							list.Add(p);
						}
						ps = new PropertyDescriptorCollection(list.ToArray());
					}
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
	}
	public class PropertyClassWebClient : PropertyClass
	{
		public PropertyClassWebClient(ClassPointer owner)
			: base(owner)
		{
		}
		public override EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Client;
			}
		}
	}
	public class PropertyClassWebServer : PropertyClass
	{
		public PropertyClassWebServer(ClassPointer owner)
			: base(owner)
		{
		}
		public override EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Server;
			}
		}
	}
	/// <summary>
	/// represent a custom property defined in a base class.
	/// it is only dynamically loaded, not saved
	/// </summary>
	public class PropertyClassInherited : PropertyClass
	{
		//the class declare the property
		private ClassPointer _declarer;

		public PropertyClassInherited(ClassPointer owner)
			: base(owner)
		{
		}
		/// <summary>
		/// it is loaded by declarer.
		/// after loading the real owner (the leaf) switches Owner and Declarer
		/// </summary>
		/// <param name="leaf"></param>
		public void ShiftOwnerDeclarer(ClassPointer leaf)
		{
			_declarer = (ClassPointer)Owner;
			Owner = leaf;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get
			{
				if (_declarer != null)
					return _declarer.ClassId;
				return 0;
			}
			set
			{
			}
		}
		/// <summary>
		/// called when the base custom property changed
		/// </summary>
		/// <param name="baseProperty"></param>
		public void Update(PropertyClass baseProperty)
		{
			SetName(baseProperty.Name);
			SetMembers(baseProperty.CanRead, baseProperty.CanWrite, baseProperty.AccessControl, baseProperty.PropertyType);
		}
		/// <summary>
		/// true: base is virtual
		/// false: base is abstract
		/// </summary>
		[Browsable(false)]
		public override bool HasBaseImplementation
		{
			get
			{
				return !IsAbstract;
			}
		}
		[Browsable(false)]
		public override bool IsOverride
		{
			get { return true; }
		}
		[ReadOnly(true)]
		public override EnumAccessControl AccessControl
		{
			get
			{
				return base.AccessControl;
			}
			set
			{
				base.AccessControl = value;
			}
		}
		[ReadOnly(true)]
		public override bool CanRead
		{
			get
			{
				return base.CanRead;
			}
			set
			{
				base.CanRead = value;
			}
		}
		[ReadOnly(true)]
		public override bool CanWrite
		{
			get
			{
				return base.CanWrite;
			}
			set
			{
				base.CanWrite = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override DataTypePointer PropertyType
		{
			get
			{
				return base.PropertyType;
			}
			set
			{
				base.PropertyType = value;
			}
		}
		/// <summary>
		/// for display
		/// </summary>
		[Description("Data type of the property")]
		public string DataType
		{
			get
			{
				return PropertyType.TypeName;
			}
		}
		[Browsable(false)]
		public override bool Implemented
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override bool DoNotCompile
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public override bool CanRemove
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override ClassPointer Declarer
		{
			get
			{
				return _declarer;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 MemberId
		{
			get
			{
				return base.MemberId;
			}
			set
			{
				base.MemberId = value;
			}
		}
		#region ICloneable Members

		public override object Clone()
		{
			PropertyClassInherited pi = (PropertyClassInherited)base.Clone();
			pi._declarer = _declarer;
			return pi;
		}
		#endregion
	}
	public class PropertyClassDescriptor : PropertyDescriptor, IIdentityByInteger
	{
		private PropertyClass _property;
		private object _value;
		private EventHandler _onValueChange;
		public PropertyClassDescriptor(PropertyClass property)
			: base(property.Name, getCustomAttributes(property))
		{
			_property = property;
		}
		/// <summary>
		/// for clone
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="name"></param>
		public PropertyClassDescriptor(PropertyClassDescriptor pc, string name)
			: base(name, pc.AttributeArray)
		{
			_property = pc._property;
			_onValueChange = pc._onValueChange;
			_value = pc._value;
		}
		static Attribute[] getCustomAttributes(PropertyClass property)
		{
			//TBD: Custom attribute support
			if (property.PropertyType.IsLibType)
			{
				object[] vs = property.PropertyType.LibTypePointer.ClassType.GetCustomAttributes(true);
				if (vs == null)
				{
					return new Attribute[] { };
				}
				Attribute[] attrs = new Attribute[vs.Length];
				for (int i = 0; i < vs.Length; i++)
				{
					attrs[i] = (Attribute)vs[i];
				}
				return attrs;
			}
			return new Attribute[] { new TypeConverterAttribute(typeof(ExpandableObjectConverter)) };
		}
		public void HookValueChange(EventHandler h)
		{
			_onValueChange = h;
		}
		public EventHandler ValueChangeHandler
		{
			get
			{
				return _onValueChange;
			}
		}
		public PropertyClass CustomProperty
		{
			get
			{
				return _property;
			}
		}
		public void Reset(PropertyClass p)
		{
			_property = p;
		}
		public UInt32 ClassId
		{
			get
			{
				return _property.ClassId;
			}
		}
		public UInt32 MemberId
		{
			get
			{
				return _property.MemberId;
			}
		}
		public override bool CanResetValue(object component)
		{
			return _property.CanWrite;
		}

		public override Type ComponentType
		{
			get
			{
				return _property.Owner.ObjectType;
			}
		}

		public override object GetValue(object component)
		{
			return _value;
		}

		public override bool IsReadOnly
		{
			get
			{
				return !_property.CanWrite;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return _property.PropertyType.ObjectType;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			_value = value;
			if (_onValueChange != null)
			{
				_onValueChange(_property, new PropertyChangeEventArg(this.Name));
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return (_property.CanWrite && !VPLUtil.IsDefaultValue(_value));
		}


		#region IIdentityByInteger Members

		public ulong WholeId
		{
			get
			{
				return _property.WholeId;
			}
		}

		#endregion
	}
}
