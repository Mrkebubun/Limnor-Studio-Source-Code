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
using ProgElements;
using System.ComponentModel;
using System.Drawing.Design;
using System.CodeDom;
using MathExp;
using VSPrj;
using XmlSerializer;
using System.Xml;
using System.Reflection;
using VPL;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using System.Globalization;
using Limnor.WebBuilder;
using LimnorDesigner.DesignTimeType;

namespace LimnorDesigner.Event
{
	/// <summary>
	/// implemented by EventPointer, EventClass and CustomEventPointer
	/// </summary>
	public interface IEvent : IObjectPointer, IMemberPointer
	{
		bool IsCustomEvent { get; }
		bool IsOverride { get; }
		string Name { get; }
		string ShortDisplayName { get; }
		MethodClass CreateHandlerMethod(object compiler);
		DataTypePointer EventHandlerType { get; }
		List<NamedDataType> GetEventParameters();
		List<ParameterClass> GetParameters(IMethod method);
	}
	/// <summary>
	/// a custom event.
	/// its event handler type, _handlerType, can be a lib type or custom type.
	/// when it is a lib type, it must be a type of delegate.
	/// when it is a custom type, it must be a CustomEventHandlerType.
	/// </summary>
	[UseParentObject]
	public class EventClass : IEvent, INonHostedObject, ICustomObject, ICustomPointer, IWithProject, ICustomTypeDescriptor
	{
		#region fields and constructors
		private UInt32 _memberId;
		private string _name = "Event";
		private ClassPointer _owner;
		private IClass _holder;
		private DataTypePointer _handlerType; //EventHandlerType or CustomEventHandlerType
		private bool _isStatic;
		private string _desc;
		EventHandler NameChanging;
		EventHandler PropertyChanged;
		private string _display;
		private IClass _compilerHolder;
		public EventClass(ClassPointer owner)
		{
			_owner = owner;
		}
		#endregion
		#region properties
		[Description("Public: all objects can access it; Protected: only this class and its derived classes can access it; Private: only this class can access it.")]
		public virtual EnumAccessControl AccessControl
		{
			get;
			set;
		}
		[Description("Description of this event")]
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
		[TypeScope(typeof(Delegate))]
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("The event handler type of the event. It specifies the parameters of the event. Usually two parameters are used; the first parameter is an object for the event sender; the second parameter is EventArgs or a class derived from EventArgs")]
		public DataTypePointer EventHandlerType
		{
			get
			{
				if (_handlerType == null)
				{
					_handlerType = new DataTypePointer(new TypePointer(typeof(EventHandler), Declarer));
				}
				return _handlerType;
			}
			set
			{
				_handlerType = value;
				if (_handlerType != null)
				{
					_handlerType.TypeChanged += new EventHandler(_handlerType_OnTypeChange);
				}
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangeEventArg("EventHandlerType"));
				}
				_display = null;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (EventHandlerType.IsLibType)
				{
					ParameterInfo[] pifs = null;
					MethodInfo mif = EventHandlerType.LibTypePointer.ClassType.GetMethod("Invoke");
					pifs = mif.GetParameters();
					if (pifs != null)
					{
						return pifs.Length;
					}
					return 0;
				}
				else
				{
					if (EventHandlerType.ClassTypePointer == null)
					{
						throw new DesignerException("Custom event handler type cannot be null");
					}
					CustomEventHandlerType ceht = EventHandlerType.ClassTypePointer as CustomEventHandlerType;
					if (ceht == null)
					{
						throw new DesignerException("Invalid custom event handler type: {0}", EventHandlerType.ClassTypePointer.GetType().Name);
					}
					return ceht.ParameterCount;
				}
			}
		}
		public ParameterInfo[] GetParameters()
		{
			if (EventHandlerType.IsLibType)
			{
				MethodInfo mif = EventHandlerType.LibTypePointer.ClassType.GetMethod("Invoke");
				return mif.GetParameters();
			}
			else
			{
				if (EventHandlerType.ClassTypePointer == null)
				{
					throw new DesignerException("GetParameters-Custom event handler type cannot be null");
				}
				CustomEventHandlerType ceht = EventHandlerType.ClassTypePointer as CustomEventHandlerType;
				if (ceht == null)
				{
					throw new DesignerException("GetParameters-Invalid custom event handler type: {0}", EventHandlerType.ClassTypePointer.GetType().Name);
				}
				List<ParameterInfo> lst = new List<ParameterInfo>();
				List<NamedDataType> ps = ceht.Parameters;
				if (ps != null && ps.Count > 0)
				{
					foreach (NamedDataType p in ps)
					{
						lst.Add(new ParameterInfoX(p));
					}
				}
				return lst.ToArray();
			}
		}
		[Browsable(false)]
		public virtual bool DoNotCompile
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region public methods
		public EventInfoX GetEventInfoX()
		{
			return new EventInfoX(this);
		}
		public void SetName(string name)
		{
			_name = name;
		}
		public EventClassInherited CreateInherited(ClassPointer owner)
		{
			EventClassInherited ei = new EventClassInherited(this.RootPointer);
			ei._desc = _desc;
			ei._handlerType = _handlerType;
			ei.AccessControl = AccessControl;
			ei._name = _name;
			ei._memberId = _memberId;
			ei.ShiftOwnerDeclarer(owner);
			return ei;
		}
		public override string ToString()
		{
			return DisplayName;
		}
		public void SetHolder(IClass holder)
		{
			_holder = holder;
		}
		public List<NamedDataType> GetEventParameters()
		{
			if (EventHandlerType.IsLibType)
			{
				List<NamedDataType> pcs = new List<NamedDataType>();
				ParameterInfo[] pifs = null;
				MethodInfo mif = EventHandlerType.LibTypePointer.ClassType.GetMethod("Invoke");
				pifs = mif.GetParameters();
				if (pifs != null && pifs.Length > 0)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						pcs.Add(new ParameterClass(pifs[i].ParameterType, pifs[i].Name, this, new MethodInfoPointer(mif)));
					}
				}
				return pcs;
			}
			else
			{
				//use custom delegate type, it is a list of parameters
				if (EventHandlerType.ClassTypePointer == null)
				{
					throw new DesignerException("Custom event handler type cannot be null");
				}
				CustomEventHandlerType ceht = EventHandlerType.ClassTypePointer as CustomEventHandlerType;
				if (ceht == null)
				{
					throw new DesignerException("Invalid custom event handler type: {0}", EventHandlerType.ClassTypePointer.GetType().Name);
				}
				return ceht.Parameters;
			}
		}
		public List<ParameterClass> GetParameters(IMethod method)
		{
			List<ParameterClass> pcs = new List<ParameterClass>();
			if (EventHandlerType.IsLibType)
			{
				ParameterInfo[] pifs = null;
				MethodInfo mif = EventHandlerType.LibTypePointer.ClassType.GetMethod("Invoke");
				pifs = mif.GetParameters();
				if (pifs != null && pifs.Length > 0)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						if (pifs[i].ParameterType.IsGenericParameter)
						{
							DataTypePointer dt = EventHandlerType.GetConcreteType(pifs[i].ParameterType);
							if (dt != null)
							{
								pcs.Add(new ParameterClass(dt.BaseClassType, pifs[i].Name, this, new MethodInfoPointer(mif)));
								continue;
							}
						}
						pcs.Add(new ParameterClass(pifs[i].ParameterType, pifs[i].Name, this, new MethodInfoPointer(mif)));
					}
				}
			}
			else
			{
				//use custom delegate type, it is a list of parameters
				if (EventHandlerType.ClassTypePointer == null)
				{
					throw new DesignerException("Custom event handler type cannot be null");
				}
				CustomEventHandlerType ceht = EventHandlerType.ClassTypePointer as CustomEventHandlerType;
				if (ceht == null)
				{
					throw new DesignerException("Invalid custom event handler type: {0}", EventHandlerType.ClassTypePointer.GetType().Name);
				}
				if (ceht.Parameters != null)
				{
					foreach (NamedDataType ndt in ceht.Parameters)
					{
						pcs.Add(new ParameterClass(ndt.BaseClassType, ndt.Name, method));
					}
				}
			}
			return pcs;
		}
		#endregion
		#region private methods
		void _handlerType_OnTypeChange(object sender, EventArgs e)
		{
			_display = null;
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangeEventArg("EventHandlerType"));
			}
		}
		private string getHandlerTypeDisplay()
		{
			if (_handlerType == null)
				return "(?)";
			if (_handlerType.IsLibType)
			{
				MethodInfo mif = _handlerType.LibTypePointer.ClassType.GetMethod("Invoke");
				return MethodPointer.GetMethodSignature(mif, false);
			}
			else
			{
				CustomEventHandlerType ceht = _handlerType.ClassTypePointer as CustomEventHandlerType;
				if (_handlerType.ClassTypePointer == null)
				{
					return "(?)";
				}
				if (ceht == null)
				{
					throw new DesignerException("Invalid custom event handler type:{0}", _handlerType.ClassTypePointer.GetType().Name);
				}
				StringBuilder sb = new StringBuilder("(");
				if (ceht.ParameterCount > 0)
				{
					for (int i = 0; i < ceht.ParameterCount; i++)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						sb.Append(ceht.Parameters[i].DataTypeName);
						sb.Append(" ");
						sb.Append(ceht.Parameters[i].Name);
					}
				}
				sb.Append(")");
				return sb.ToString();
			}
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
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				IObjectPointer root = this.Holder;
				if (root != null)
				{
					return root.RootPointer;
				}
				return null;
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
		[Browsable(false)]//hide it for this version
		[Description("Indicates whether it is a static event shared across all instances of this class.")]
		public bool IsStatic
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
				ClassId = _owner.ClassId;
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
				return EventHandlerType.BaseClassType;
			}
			set
			{
				if (value != null)
				{
					if (typeof(EventHandler).IsAssignableFrom(value))
					{
						EventHandlerType.SetDataType(value);
					}
					else
					{
						throw new DesignerException("Invalid event handler type:{0}", value);
					}
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				return EventHandlerType.ObjectInstance;
			}
			set
			{
				EventHandlerType.ObjectInstance = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectDebug
		{
			get
			{
				return EventHandlerType.ObjectDebug;
			}
			set
			{
				EventHandlerType.ObjectDebug = value;
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_display))
				{
					if (string.IsNullOrEmpty(_name))
					{
						if (_handlerType == null)
							_display = "?(?)";
						else
							_display = "?" + getHandlerTypeDisplay();
					}
					else
					{
						if (_handlerType == null)
							_display = _name + "(?)";
						else
							_display = _name + getHandlerTypeDisplay();
					}
				}
				return _display;
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
			return target == EnumObjectSelectType.All || target == EnumObjectSelectType.Event;
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
		public void SetCompileHolder(IClass h)
		{
			_compilerHolder = h;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <returns>CodeEventReferenceExpression</returns>
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeExpression p;
			if (IsStatic)
			{
				p = new CodeTypeReferenceExpression(Declarer.TypeString);
			}
			else
			{
				if (_compilerHolder != null)
				{
					p = _compilerHolder.GetReferenceCode(method, statements, forValue);
				}
				else
				{
					p = Holder.GetReferenceCode(method, statements, forValue);
				}
			}
			return new CodeEventReferenceExpression(p, this.Name);
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
		public bool IsValid
		{
			get
			{
				if (_handlerType != null && _owner != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Some of (_handlerType, _owner) are null for [{0}] of [{1}]. ({2},{3})", this.ToString(), this.GetType().Name, _handlerType, _owner);
				return false;
			}
		}
		[Browsable(false)]
		public virtual MemberAttributes EventAttributes
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
				return a;
			}
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			EventClass ec = objectIdentity as EventClass;
			if (ec != null)
			{
				return (ec.WholeId == this.WholeId);
			}
			CustomEventPointer cp = objectIdentity as CustomEventPointer;
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
			get { return Owner; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Custom; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Event; } }
		#endregion

		#region ICloneable Members

		public virtual object Clone()
		{
			ClassPointer owner = (ClassPointer)_owner.Clone();
			EventClass obj = (EventClass)Activator.CreateInstance(this.GetType(), owner);
			obj.IsStatic = this.IsStatic;
			obj.MemberId = MemberId;
			obj.SetName(_name);
			obj.Description = Description;
			if (_handlerType != null)
			{
				obj._handlerType = (DataTypePointer)_handlerType.Clone();
			}
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region INonHostedObject Members
		/// <summary>
		/// save the changes to XML
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootNode"></param>
		/// <param name="writer"></param>
		public void OnPropertyChanged(string name, object property, XmlNode rootNode, XmlObjectWriter writer)
		{
			XmlNode node = SerializeUtil.GetCustomEventNode(rootNode, this.MemberId);
			if (string.CompareOrdinal(name, "Name") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "Name");
				nd.InnerText = this.Name;
				_display = null;
			}
			else if (string.CompareOrdinal(name, "IsStatic") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "IsStatic");
				nd.InnerText = this.IsStatic.ToString();
			}
			else if (string.CompareOrdinal(name, "Description") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "Description");
				nd.InnerText = this.Description;
			}
			else if (string.CompareOrdinal(name, "EventHandlerType") == 0)
			{
				XmlNode nd = SerializeUtil.CreatePropertyNode(node, "EventHandlerType");
				writer.WriteValue(nd, this.EventHandlerType, null);
				_display = null;
			}
		}

		[ParenthesizePropertyName(true)]
		[Description("Property name")]
		public string Name
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
						_display = null;
						if (PropertyChanged != null)
						{
							PropertyChanged(this, new PropertyChangeEventArg("Name"));
						}
					}
				}
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

		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			NameChanging = nameChange;
			PropertyChanged = propertyChange;
		}

		#endregion

		#region IEvent Members
		[Browsable(false)]
		public virtual bool IsCustomEvent
		{
			get { return true; }
		}
		[Browsable(false)]
		public virtual bool IsOverride
		{
			get { return false; }
		}
		[Browsable(false)]
		public string ShortDisplayName
		{
			get
			{
				StringBuilder sb = new StringBuilder(Name);
				IObjectPointer op = this.Owner;
				while (op != null && op.Owner != null)
				{
					if (op is IClass)
					{
						break;
					}
					sb.Append(".");
					sb.Append(op.CodeName);
					op = op.Owner;
				}
				return sb.ToString();
			}
		}
		public MethodClass CreateHandlerMethod(object compiler)
		{
			UInt32 id = (uint)(Guid.NewGuid().GetHashCode());
			string methodName = Name + "_" + id.ToString("x");
			MethodClass mc = new MethodClass(RootPointer);
			mc.MethodName = methodName;
			mc.SetCompilerData(compiler);
			mc.MethodID = id;
			mc.Parameters = GetParameters(mc);
			return mc;
		}
		#endregion

		#region ICustomObject Members
		[ReadOnly(true)]
		[Browsable(false)]
		public ulong WholeId
		{
			get { return DesignUtil.MakeDDWord(MemberId, ClassId); }
		}

		#endregion

		#region ICustomPointer Members

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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (VPLUtil.GetBrowseableProperties(attributes))
			{
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach (PropertyDescriptor p in ps)
				{
					if (IsOverride)
					{
						if (string.CompareOrdinal(p.Name, "EventHandlerType") == 0)
						{
							PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), p.Name, this.EventHandlerType.TypeName, attributes);
							list.Add(p0);

						}
						else
						{
							ReadOnlyPropertyDesc p0 = new ReadOnlyPropertyDesc(p);
							list.Add(p0);
						}
					}
					else
					{
						list.Add(p);
					}
				}
				if (this.EventHandlerType.IsGenericType)
				{
					DataTypePointer[] tps = this.EventHandlerType.TypeParameters;
					if (tps != null && tps.Length > 0)
					{
						Type[] tps0 = this.EventHandlerType.BaseClassType.GetGenericArguments();
						if (tps0 != null && tps0.Length == tps.Length)
						{
							for (int i = 0; i < tps.Length; i++)
							{
								PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), tps0[i].Name, tps[i].BaseClassType.Name, attributes);
								list.Add(p0);
							}
						}
					}
				}
				ps = new PropertyDescriptorCollection(list.ToArray());
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

	/// <summary>
	/// represent a custom property defined in a base class.
	/// it is only dynamically loaded, not saved
	/// </summary>
	public class EventClassInherited : EventClass
	{
		private ClassPointer _declarer;
		public EventClassInherited(ClassPointer owner)
			: base(owner)
		{
		}
		public void ShiftOwnerDeclarer(ClassPointer leaf)
		{
			_declarer = (ClassPointer)Owner;
			Owner = leaf;
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

		[Browsable(false)]
		public override bool DoNotCompile
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public override bool IsOverride
		{
			get { return true; }
		}
		#region ICloneable Members

		public override object Clone()
		{
			EventClassInherited ei = (EventClassInherited)base.Clone();
			ei._declarer = _declarer;
			return ei;
		}
		#endregion
	}
}
