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
using System.Drawing;
using ProgElements;
using System.CodeDom;
using MathExp;
using System.Drawing.Design;
using VSPrj;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.MenuUtil;
using VPL;
using System.Reflection;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using System.Globalization;
using System.Collections;
using Limnor.PhpComponents;

namespace LimnorDesigner
{
	public interface IDynamicProperties
	{
		void SetPropertyGrid(PropertyGrid pg);
	}
	/// <summary>
	/// represent a data type
	/// it can be a TypePointer or a ClassPointer
	/// when it is used as a property, use [TypeScope(typeof(T))] to restrict the types can be selected from the type selector
	/// Derived from it:
	/// ConstObjectPointer - represent a constant object using a constructor
	/// ParameterClass - represent a method/event parameter. its value is not well defined. Action parameters use ParameterValue
	/// </summary>
	public class DataTypePointer : IXmlNodeSerializable, IClass, IWithProject
	{
		#region fields and constructors
		const string TYPE = "Type";
		//at any time these two pointers can only have one non-null
		private TypePointer _typePointer;
		private ClassPointer _classRef;
		private DataTypePointer _concretTypeForTypeParameter; //for generic parameter
		public event EventHandler TypeChanged;
		public DataTypePointer()
		{
		}
		public DataTypePointer(Type type)
			: this(new TypePointer(type))
		{
		}
		public DataTypePointer(TypePointer type)
		{
			_typePointer = type;
			if (_typePointer != null)
			{
				_classRef = null;
			}
		}
		public DataTypePointer(ClassPointer component)
		{
			_classRef = component;
			if (_classRef != null)
			{
				_typePointer = null;
			}
		}
		#endregion
		#region Methods
		public object GetDefaultValue()
		{
			if (_typePointer == null)
			{
				return null;
			}
			if (_typePointer.ClassType == null)
				return null;
			object[] vs = _typePointer.ClassType.GetCustomAttributes(typeof(TypeMappingAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				TypeMappingAttribute tm = vs[0] as TypeMappingAttribute;
				if (tm != null)
				{
					return VPLUtil.GetDefaultValue(tm.MappedType);
				}
			}
			return VPLUtil.GetDefaultValue(_typePointer.ClassType);
		}
		public virtual bool ContainsGenericParameters()
		{
			if (this.BaseClassType != null)
			{
				if (this.BaseClassType.IsGenericType)
				{
					Type[] ts = this.BaseClassType.GetGenericArguments();
					if (ts != null && ts.Length > 0)
					{
						for (int i = 0; i < ts.Length; i++)
						{
							if (ts[i].IsGenericParameter)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public void SetConcreteType(DataTypePointer type)
		{
			_concretTypeForTypeParameter = type;
		}
		public IList<DataTypePointer> GetGenericTypes()
		{
			if (this.BaseClassType == null)
				return null;
			if (this.BaseClassType.IsGenericParameter)
			{
				if (_concretTypeForTypeParameter != null)
				{
					List<DataTypePointer> l = new List<DataTypePointer>();
					l.Add(this);
					return l;
				}
			}
			else if (this.BaseClassType.IsGenericType)
			{
				if (TypeParameters != null && TypeParameters.Length > 0)
				{
					Type[] tcs = this.BaseClassType.GetGenericArguments();
					if (tcs != null && tcs.Length == TypeParameters.Length)
					{
						List<DataTypePointer> l = new List<DataTypePointer>();
						for (int i = 0; i < tcs.Length; i++)
						{
							DataTypePointer dp = new DataTypePointer(tcs[i]);
							dp._concretTypeForTypeParameter = TypeParameters[i];
							l.Add(dp);
						}
						return l;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// this type is a generic argument for a generic type or a generic parameter for a generic method.
		/// this property is the corresponding concrete type
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public DataTypePointer ConcreteType
		{
			get
			{
				return _concretTypeForTypeParameter;
			}
			set
			{
				_concretTypeForTypeParameter = value;
			}
		}
		/// <summary>
		/// this type is a generic type
		/// </summary>
		/// <returns>corresponding concrete types for generic arguments</returns>
		public DataTypePointer[] GetConcreteTypes()
		{
			return TypeParameters;
		}
		/// <summary>
		/// this type is a generic type
		/// </summary>
		/// <param name="typeParameter">a generic argument</param>
		/// <returns>the corresponding concrete type</returns>
		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			if (typeParameter.IsGenericType)
			{
				if (this.BaseClassType.Equals(typeParameter))
				{
					return this;
				}
				DataTypePointer dtp = new DataTypePointer(new TypePointer(typeParameter));
				Type[] tcs = typeParameter.GetGenericArguments();
				if (tcs != null && tcs.Length > 0)
				{
					if (TypeParameters != null)
					{
						DataTypePointer[] dtps = new DataTypePointer[tcs.Length];
						for (int i = 0; i < tcs.Length; i++)
						{
							dtps[i] = null;
							for (int k = 0; k < TypeParameters.Length; k++)
							{
								if (tcs[i].Equals(TypeParameters[k]))
								{
									dtps[i] = TypeParameters[k];
									break;
								}
							}
							if (dtps[i] == null)
							{
								return null;
							}
						}
						dtp.TypeParameters = dtps;
					}
					else
					{
						return null;
					}
				}
				return dtp;
			}
			else if (typeParameter.IsGenericParameter)
			{
				if (TypeParameters != null && TypeParameters.Length > 0)
				{
					Type[] tcs = this.BaseClassType.GetGenericArguments();
					if (tcs != null && tcs.Length > 0)
					{
						for (int i = 0; i < tcs.Length; i++)
						{
							if (tcs[i].Equals(typeParameter))
							{
								return TypeParameters[tcs[i].GenericParameterPosition];
							}
						}
					}
				}
			}
			return null;
		}
		public virtual bool IsAssignableFrom(Type type)
		{
			if (type != null)
			{
				Type bt = BaseClassType;
				if (bt != null)
				{
					if (bt.IsGenericParameter)
					{
						if (type.IsAssignableFrom(bt))
						{
							return true;
						}
						Type[] tcs = bt.GetGenericParameterConstraints();
						if (tcs != null && tcs.Length > 0)
						{
							for (int i = 0; i < tcs.Length; i++)
							{
								if (tcs[i].IsAssignableFrom(type))
								{
									return true;
								}
							}
						}
						else
						{
							return true;
						}
					}
					if (bt.IsAssignableFrom(type))
					{
						return true;
					}

				}
				else
				{
					return true;
				}
			}
			return false;
		}
		public virtual bool IsAssignableFrom(DataTypePointer type)
		{
			if (type == null)
			{
				return false;
			}
			if (IsLibType || type.IsLibType)
			{
				if (BaseClassType == null)
				{
					return true;
				}
				if (BaseClassType.IsAssignableFrom(type.BaseClassType))
				{
					return true;
				}
				if (type.IsGenericParameter || type.IsGenericType || BaseClassType.IsGenericParameter || BaseClassType.IsGenericType)
				{
					return BaseClassType.Equals(type.BaseClassType);
				}
				return false;
			}
			else
			{
				if (type.IsSameObjectRef(this))
				{
					return true;
				}
				else
				{
					//type is derived from _value?
					return (type.VariableCustomType.GetBaseClass(ClassId) != null);
				}
			}
		}
		public void SetDataType(IObjectPointer p)
		{
			DataTypePointer t = p as DataTypePointer;
			if (t != null)
			{
				SetDataType(t);
			}
			else
			{
				ClassPointer cp = p as ClassPointer;
				if (cp != null)
				{
					SetDataType(cp);
				}
				else
				{
					TypePointer tp = p as TypePointer;
					if (tp != null)
					{
						SetDataType(tp);
					}
				}
			}
		}
		public void SetCustomType(ClassPointer cp)
		{
			_classRef = cp;
		}
		public bool ForceDataType(DataTypePointer t)
		{
			bool bTypeChanged = false;
			if (t == null)
			{
				_classRef = null;
				_typePointer = new TypePointer(typeof(void));
				bTypeChanged = true;
			}
			else
			{
				if (t.ClassTypePointer != null)
				{
					_typePointer = null;
					if (_classRef != null)
					{
						if (!_classRef.IsSameObjectRef(t.ClassTypePointer))
						{
							_classRef = t.ClassTypePointer;
							bTypeChanged = true;
						}
					}
					else
					{
						_classRef = t.ClassTypePointer;
						bTypeChanged = true;
					}
				}
				else
				{
					if (t.LibTypePointer != null)
					{
						_classRef = null;
						if (_typePointer == null)
						{
							_typePointer = t.LibTypePointer;
							bTypeChanged = true;
						}
						else
						{
							if (!t.BaseClassType.IsAssignableFrom(_typePointer.VariableLibType))
							{
								_typePointer = t.LibTypePointer;
								bTypeChanged = true;
							}
						}
					}
				}
				if (t.TypeParameters != null)
				{
					this.TypeParameters = t.TypeParameters;
				}
			}
			return bTypeChanged;
		}
		public void SetDataType(DataTypePointer t)
		{
			if (ForceDataType(t))
			{
				OnDataTypeChanged();
			}
		}
		public void SetDataType(TypePointer t)
		{
			bool bTypeChanged = false;
			_classRef = null;
			if (t == null)
			{
				_typePointer = new TypePointer(typeof(void));
				bTypeChanged = true;
			}
			else
			{
				if (_typePointer == null)
				{
					_typePointer = t;
					bTypeChanged = true;
				}
				else
				{
					if (!t.VariableLibType.IsAssignableFrom(_typePointer.VariableLibType))
					{
						_typePointer = t;
						bTypeChanged = true;
					}
				}
			}
			if (bTypeChanged)
			{
				OnDataTypeChanged();
			}
		}
		public void SetDataType(ClassPointer c)
		{
			bool bTypeChanged = false;
			if (c != null)
			{
				_typePointer = null;
				if (_classRef == null)
				{
					_classRef = c;
					bTypeChanged = true;
				}
				else
				{
					if (!_classRef.IsSameObjectRef(c))
					{
						_classRef = c;
						bTypeChanged = true;
					}
				}
			}
			if (bTypeChanged)
			{
				OnDataTypeChanged();
			}
		}
		public void SetDataType(Type t)
		{
			bool bTypeChanged = false;
			if (t != null)
			{
				if (_typePointer != null)
				{
					if (!t.IsAssignableFrom(_typePointer.ClassType))
					{
						_typePointer.ClassType = t;
						bTypeChanged = true;
					}
				}
				else
				{
					_typePointer = new TypePointer(t);
					if (_classRef != null)
					{
						_typePointer.Owner = _classRef.Owner;
					}
					bTypeChanged = true;
				}
				_classRef = null;
			}
			if (bTypeChanged)
			{
				OnDataTypeChanged();
			}
		}
		public void SetDataType(object v)
		{
			if (v == null)
				return;
			ClassPointer p = v as ClassPointer;
			if (p != null)
			{
				SetDataType(p);
			}
			else
			{
				DataTypePointer dp = v as DataTypePointer;
				if (dp != null)
				{
					SetDataType(dp);
				}
				else
				{
					Type t = v as Type;
					if (t != null)
					{
						SetDataType(t);
					}
					else
					{
						TypePointer tpr = v as TypePointer;
						if (tpr != null)
						{
							SetDataType(tpr);
						}
						else
						{
							throw new DesignerException("Invalid data type pointer {0}", v.GetType());
						}
					}
				}
			}
		}
		protected virtual void OnDataTypeChanged()
		{
			if (TypeChanged != null)
			{
				TypeChanged(this, EventArgs.Empty);
			}
		}
		public object CreateInstance()
		{
			if (_classRef != null)
			{
				return _classRef.ObjectInstance;
			}
			else
			{
				if (_typePointer != null)
				{
					return _typePointer.TryCreateInstance();
				}
			}
			return null;
		}
		public virtual LocalVariable CreateVariable(string name, UInt32 classId, UInt32 memberId)
		{
			if (memberId == 0)
				memberId = (UInt32)Guid.NewGuid().GetHashCode();
			LocalVariable v = new LocalVariable(this, name, classId, memberId);
			v.Owner = this.Owner;
			return v;
		}
		public override string ToString()
		{
			return DataTypeName;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Type DataTypeEx
		{
			get
			{
				if (_classRef != null)
				{
					return _classRef.TypeDesigntime;
				}
				else if (_typePointer != null)
				{
					if (_typePointer.ClassType != null && _typePointer.ClassType.IsGenericType)
					{
						if (TypeParameters != null && TypeParameters.Length > 0)
						{
							Type[] ts = new Type[TypeParameters.Length];
							for (int i = 0; i < ts.Length; i++)
							{
								ts[i] = TypeParameters[i].DataTypeEx;
							}
							Type t = _typePointer.ClassType.MakeGenericType(ts);
							return t;
						}
					}
					return _typePointer.ClassType;
				}
				return null;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public DataTypePointer[] TypeParameters
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsInterface
		{
			get
			{
				ClassPointer cp = this.VariableCustomType;
				if (cp != null)
				{
					return cp.IsInterface;
				}
				if (this.VariableLibType != null)
				{
					return VariableLibType.IsInterface;
				}
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual bool ReadOnly { get; set; }
		[Browsable(false)]
		public virtual string BaseName
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.Name;
				}
				if (_classRef != null)
				{
					return _classRef.Name;
				}
				return "var";
			}
		}
		[Browsable(false)]
		public virtual Type BaseClassType
		{
			get
			{
				return ObjectType;
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
		[Browsable(false)]
		public virtual UInt32 ClassId
		{
			get
			{
				if (_classRef != null)
					return _classRef.ClassId;
				return 0;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public virtual string CodeName
		{
			get
			{
				if (_classRef != null)
					return _classRef.Name;
				if (_typePointer != null)
				{
					return VPLUtil.NameToCodeName(_typePointer.Name);
				}
				return "?";
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
				return CodeName;
			}
		}
		[Browsable(false)]
		public virtual TypePointer LibTypePointer
		{
			get
			{
				return _typePointer;
			}
		}
		[Browsable(false)]
		public ClassPointer ClassTypePointer
		{
			get
			{
				return _classRef;
			}
		}
		[Browsable(false)]
		public virtual bool IsLibType
		{
			get
			{
				return (_typePointer != null);
			}
		}
		[Browsable(false)]
		public bool IsValueType
		{
			get
			{
				return BaseClassType.IsValueType;
			}
		}
		[Browsable(false)]
		public bool IsPrimitive
		{
			get
			{
				return BaseClassType.IsPrimitive;
			}
		}
		[Browsable(false)]
		public bool IsArray
		{
			get
			{
				return BaseClassType.IsArray;
			}
		}
		[Browsable(false)]
		public bool IsCollection
		{
			get
			{
				if (IsArray)
					return true;
				if (typeof(IList).IsAssignableFrom(BaseClassType))
				{
					return true;
				}
				if (typeof(ICollection).IsAssignableFrom(BaseClassType))
				{
					return true;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsGenericType
		{
			get
			{
				if (BaseClassType != null)
				{
					Type t = VPLUtil.GetObjectType(BaseClassType);
					return t.IsGenericType;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsGenericParameter
		{
			get
			{
				if (BaseClassType != null)
					return BaseClassType.IsGenericParameter;
				return false;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (_typePointer != null)
				{
					return (_typePointer.ObjectType.IsSealed && _typePointer.ObjectType.IsAbstract);
				}
				if (_classRef != null)
				{
					return _classRef.IsStatic;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsVoid
		{
			get
			{
				if (_classRef != null)
				{
					return false;
				}
				if (_typePointer != null)
				{
					if (_typePointer.ClassType.Equals(typeof(void)))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				return true;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_classRef != null || _typePointer != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_classRef and _typePointer are null for [{0}] of [{1}]. ({2},{3})", this.ToString(), this.GetType().Name, _classRef, _typePointer);
				return false;
			}
		}
		[Browsable(false)]
		public virtual string DataTypeName
		{
			get
			{
				if (_typePointer != null)
				{
					if (_typePointer.ObjectType != null)
					{
						return VPLUtil.GetTypeDisplay(_typePointer.ObjectType);
					}
				}
				if (_classRef != null)
				{
					return _classRef.DisplayName;
				}
				return "Unknown";
			}
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("The type of this value")]
		public virtual IObjectPointer DataType
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer;
				}
				if (_classRef != null)
				{
					return _classRef;
				}
				_typePointer = new TypePointer(typeof(object));
				return _typePointer;
			}
			set
			{
				SetDataType(value);
				OnDataTypeChanged();
			}
		}

		#endregion

		#region IObjectPointer Members
		[Browsable(false)]
		public virtual EnumWebRunAt RunAt
		{
			get
			{
				if (VPLUtil.CompilerContext_JS)
					return EnumWebRunAt.Client;
				if (_typePointer != null)
				{
					return _typePointer.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public virtual ClassPointer RootPointer
		{
			get
			{
				IObjectPointer root = this.Owner;
				if (root != null)
					return root.RootPointer;
				return null;
			}
		}
		[Browsable(false)]
		public IClass Host
		{
			get
			{
				return RootPointer;
			}
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
		public virtual IObjectPointer Owner
		{
			get
			{
				if (_classRef != null)
					return _classRef.Owner;
				if (_typePointer != null)
					return _typePointer.Owner;
				return null;
			}
			set
			{
				if (_classRef != null)
					_classRef.Owner = value;
				else if (_typePointer != null)
					_typePointer.Owner = value;

			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				if (_classRef != null)
					return _classRef.ObjectType;
				if (_typePointer != null)
					return _typePointer.ObjectType;
				_typePointer = new TypePointer(typeof(object));
				return _typePointer.ObjectType;
			}
			set
			{
				if (_classRef != null)
				{
					_classRef.ObjectType = value;
					if (TypeChanged != null)
					{
						TypeChanged(this, EventArgs.Empty);
					}
				}
				else if (_typePointer != null)
				{
					_typePointer.ObjectType = value;
					if (TypeChanged != null)
					{
						TypeChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug { get; set; }
		[Browsable(false)]
		public virtual object ObjectInstance
		{
			get
			{
				if (_classRef != null)
					return _classRef.ObjectInstance;
				if (_typePointer != null)
					return _typePointer.ObjectInstance;
				return null;
			}
			set
			{
				if (_classRef != null)
					_classRef.ObjectInstance = value;
				else if (_typePointer != null)
					_typePointer.ObjectInstance = value;
			}
		}

		public virtual bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			if (_classRef != null)
				return _classRef.IsSameObjectRef(objectPointer);
			if (_typePointer != null)
				return _typePointer.IsSameObjectRef(objectPointer);
			return false;
		}
		[Browsable(false)]
		public virtual bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				if (_classRef != null)
					return _classRef.IsSameObjectRef(objectPointer);
				if (_typePointer != null)
					return _typePointer.IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public virtual string DisplayName
		{
			get
			{
				if (_classRef != null)
					return _classRef.DisplayName;
				if (_typePointer != null)
					return _typePointer.DisplayName;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public virtual string LongDisplayName
		{
			get
			{
				if (_classRef != null)
					return _classRef.LongDisplayName;
				if (_typePointer != null)
					return _typePointer.LongDisplayName;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public virtual string ExpressionDisplay
		{
			get
			{
				if (_classRef != null)
					return _classRef.ExpressionDisplay;
				if (_typePointer != null)
					return _typePointer.ExpressionDisplay;
				return string.Empty;
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (_classRef != null)
				return _classRef.IsTargeted(target);
			if (_typePointer != null)
				return _typePointer.IsTargeted(target);
			return false;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (_classRef != null)
					return _classRef.ObjectKey;
				if (_typePointer != null)
					return _typePointer.ObjectKey;
				return "?";
			}
		}
		[Browsable(false)]
		public Type MakeGenericType()
		{
			if (_typePointer != null)
			{
				Type t = _typePointer.ClassType;
				if (t != null && t.IsGenericType)
				{
					if (this.TypeParameters != null && this.TypeParameters.Length > 0)
					{
						Dictionary<string, string> typeMaps = new Dictionary<string, string>();
						Type[] ts = new Type[this.TypeParameters.Length];
						for (int i = 0; i < this.TypeParameters.Length; i++)
						{
							if (this.TypeParameters[i].IsLibType)
							{
								ts[i] = this.TypeParameters[i].BaseClassType;
							}
							else
							{
								if (i == 0)
								{
									ts[i] = typeof(dataType0);
								}
								else if (i == 1)
								{
									ts[i] = typeof(dataType1);
								}
								else if (i == 2)
								{
									ts[i] = typeof(dataType2);
								}
								else if (i == 3)
								{
									ts[i] = typeof(dataType3);
								}
								else
								{
									throw new DesignerException("too many generic type parameters");
								}
								typeMaps.Add(ts[i].AssemblyQualifiedName, this.TypeParameters[i].TypeString);
							}
						}
						return t.MakeGenericType(ts);
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		public virtual string TypeString
		{
			get
			{
				if (_classRef != null)
					return _classRef.TypeString;
				if (_typePointer != null)
				{
					Type t = _typePointer.ClassType;
					if (t != null && t.IsGenericType)
					{
						if (this.TypeParameters != null && this.TypeParameters.Length > 0)
						{
							bool bInvolveNonLib = false;
							Dictionary<string, string> typeMaps = new Dictionary<string, string>();
							Type[] ts = new Type[this.TypeParameters.Length];
							for (int i = 0; i < this.TypeParameters.Length; i++)
							{
								if (this.TypeParameters[i].IsLibType)
								{
									ts[i] = this.TypeParameters[i].BaseClassType;
								}
								else
								{
									if (i == 0)
									{
										ts[i] = typeof(dataType0);
									}
									else if (i == 1)
									{
										ts[i] = typeof(dataType1);
									}
									else if (i == 2)
									{
										ts[i] = typeof(dataType2);
									}
									else if (i == 3)
									{
										ts[i] = typeof(dataType3);
									}
									else
									{
										throw new DesignerException("too many generic type parameters");
									}
									typeMaps.Add(ts[i].AssemblyQualifiedName, this.TypeParameters[i].TypeString);
									bInvolveNonLib = true;
								}
							}
							Type tg = t.MakeGenericType(ts);
							string s = tg.FullName;
							if (bInvolveNonLib)
							{
								foreach (KeyValuePair<string, string> kv in typeMaps)
								{
									if (s.IndexOf(string.Format(CultureInfo.InvariantCulture, "[{0}]", kv.Key), StringComparison.Ordinal) >= 0)
									{
										s = s.Replace(string.Format(CultureInfo.InvariantCulture, "[{0}]", kv.Key), kv.Value);
									}
									else
									{
										s = s.Replace(kv.Key, kv.Value);
									}
								}
							}
							return s;
						}
					}
					return _typePointer.ClassType.FullName;
				}
				return "";
			}
		}
		[Browsable(false)]
		public virtual string TypeName
		{
			get
			{
				if (_classRef != null)
					return _classRef.Name;
				if (_typePointer != null && _typePointer.ClassType != null)
					return _typePointer.ClassType.Name;
				return "?";
			}
		}
		public virtual CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_classRef != null)
			{
				return _classRef.GetReferenceCode(method, statements, forValue);
			}
			if (_typePointer != null)
			{
				return _typePointer.GetReferenceCode(method, statements, forValue);
			}
			throw new DesignerException("TypePointer missing type");
		}
		[Browsable(false)]
		public bool HasInterface(string name)
		{
			Type t = this.BaseClassType;
			if (t != null)
			{
				return t.GetInterface(name) != null;
			}
			return true;
		}
		public CodeTypeReference GetCodeTypeReference()
		{
			if (BaseClassType.IsGenericType)
			{
				if (this.TypeParameters != null)
				{
					Type[] tas = new Type[this.TypeParameters.Length];
					for (int i = 0; i < tas.Length; i++)
					{
						if (this.TypeParameters[i] == null)
						{
							tas[i] = typeof(object);
						}
						else
						{
							tas[i] = this.TypeParameters[i].BaseClassType;
						}
					}
					return ConstructorPointer.CreateGenericTypeReference(BaseClassType, tas);
				}
			}
			else if (BaseClassType.IsGenericParameter)
			{
				if (_concretTypeForTypeParameter != null)
				{
					return _concretTypeForTypeParameter.GetCodeTypeReference();
				}
				else
				{
					throw new DesignerException("Generic parameter [{0}] not resolved", BaseClassType);
				}
			}
			return new CodeTypeReference(this.TypeString);
		}
		public virtual string GetJavaScriptReferenceCode(StringCollection code)
		{
			return CreateJavaScript(code);
		}
		public virtual string GetPhpScriptReferenceCode(StringCollection code)
		{
			return CreatePhpScript(code);
		}
		public virtual string CreateJavaScript(StringCollection sb)
		{
			if (_classRef != null)
			{
				return _classRef.CreateJavaScript(sb);
			}
			if (_typePointer != null)
			{
				return _typePointer.CreateJavaScript(sb);
			}
			throw new DesignerException("Error creating javascript code. TypePointer missing type");
		}
		public virtual string CreatePhpScript(StringCollection sb)
		{
			if (_classRef != null)
			{
				return _classRef.CreatePhpScript(sb);
			}
			if (_typePointer != null)
			{
				return _typePointer.CreatePhpScript(sb);
			}
			throw new DesignerException("Error creating php code. TypePointer missing type");
		}
		public virtual void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Append(returnReceiver);
				sb.Append("=");
			}
			sb.Append(methodName);
			sb.Append("(");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Append(parameters[0]);
				for (int i = 1; i < parameters.Count; i++)
				{
					sb.Append(",");
					sb.Append(parameters[i]);
				}
			}
			sb.Append(");\r\n");
			code.Add(sb.ToString());
		}
		public virtual void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Append(returnReceiver);
				sb.Append("=");
			}
			sb.Append(methodName);
			sb.Append("(");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Append(parameters[0]);
				for (int i = 1; i < parameters.Count; i++)
				{
					sb.Append(",");
					sb.Append(parameters[i]);
				}
			}
			sb.Append(");\r\n");
			code.Add(sb.ToString());
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				if (this.IsLibType)
					return EnumObjectDevelopType.Library;
				return EnumObjectDevelopType.Custom;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Unknown; } }
		#endregion

		#region ICloneable Members
		protected virtual DataTypePointer CreateCloneObject()
		{
			return (DataTypePointer)Activator.CreateInstance(this.GetType());
		}
		public virtual object Clone()
		{
			DataTypePointer dtp = CreateCloneObject();
			if (_classRef != null)
				dtp.SetDataType((ClassPointer)_classRef.Clone());
			else if (_typePointer != null)
				dtp.SetDataType((TypePointer)_typePointer.Clone());
			dtp.ReadOnly = ReadOnly;
			dtp._concretTypeForTypeParameter = this._concretTypeForTypeParameter;
			dtp.TypeParameters = this.TypeParameters;
			return dtp;
		}

		#endregion

		#region ISerializerProcessor Members

		public virtual void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
				return;
			if (_classRef != null)
			{
				if (_classRef.IsInterface)
				{
					_classRef.EnsureLoaded();
				}
				_classRef.OnPostSerialize(objMap, objectNode, saved, serializer);
			}
			else if (_typePointer != null)
			{
				_typePointer.OnPostSerialize(objMap, objectNode, saved, serializer);
			}
		}

		#endregion

		#region ICustomContentSerialization Members
		[Browsable(false)]
		public virtual Dictionary<string, object> CustomContents
		{
			get
			{
				Dictionary<string, object> obj = new Dictionary<string, object>();
				if (_classRef != null)
				{
					obj.Add(TYPE, _classRef);
				}
				else if (_typePointer != null)
				{
					obj.Add(TYPE, _typePointer);
				}
				else
				{
					_typePointer = new TypePointer(typeof(void));
					obj.Add(TYPE, _typePointer);
				}
				return obj;
			}
			set
			{
				object v;
				if (value.TryGetValue(TYPE, out v))
				{
					if (v != null)
					{
						_classRef = v as ClassPointer;
						if (_classRef == null)
						{
							_typePointer = v as TypePointer;
							if (_typePointer == null)
							{
								Type t = v as Type;
								if (t != null)
								{
									_typePointer = new TypePointer(t);
								}
								else
								{
									throw new DesignerException("Invalid DataTypePointer {0}", v.GetType().Name);
								}
							}
						}
					}
				}
			}
		}

		#endregion

		#region IClass Members

		[Browsable(false)]
		public virtual Image ImageIcon
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.ImageIcon;
				}
				if (_classRef != null)
				{
					return _classRef.ImageIcon;
				}
				return null;
			}
			set
			{
				if (_typePointer != null)
				{
					_typePointer.ImageIcon = value;
				}
				if (_classRef != null)
				{
					_classRef.ImageIcon = value;
				}
			}
		}
		[Browsable(false)]
		public Type VariableLibType
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.ClassType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public ClassPointer VariableCustomType
		{
			get
			{
				if (_classRef != null)
				{
					return _classRef;
				}
				if (this.VariableLibType != null && this.VariableLibType.IsGenericParameter && ConcreteType != null)
				{
					return ConcreteType.VariableCustomType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IClassWrapper VariableWrapperType
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		public virtual ulong WholeId
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.WholeId;
				}
				if (_classRef != null)
				{
					return _classRef.WholeId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public virtual uint MemberId
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.ClassId;
				}
				if (_classRef != null)
				{
					return _classRef.MemberId;
				}
				return 0;
			}
		}
		public virtual string Name
		{
			get
			{
				if (_typePointer != null)
				{
					return _typePointer.Name;
				}
				if (_classRef != null)
				{
					return _classRef.Name;
				}
				return "?";
			}
			set
			{
				if (_classRef != null)
				{
					_classRef.Name = value;
				}
			}
		}

		#endregion

		#region IWithProject Members
		[Browsable(false)]
		public virtual LimnorProject Project
		{
			get
			{
				if (_classRef != null)
				{
					return _classRef.Project;
				}
				if (_typePointer != null)
				{
					ClassPointer p = _typePointer.RootPointer;
					if (p != null)
					{
						return p.Project;
					}
				}
				return LimnorProject.ActiveProject;
			}
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XML_TypeParams = "TypeParams";
		public virtual void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlNode ndType = node.SelectSingleNode(TYPE);
			if (ndType == null)
			{
				ndType = node.OwnerDocument.CreateElement(TYPE);
				node.AppendChild(ndType);
			}
			if (_classRef != null)
			{
				writer.WriteObjectToNode(ndType, _classRef);
			}
			else
			{
				if (_typePointer == null)
					_typePointer = new TypePointer(typeof(void));
				writer.WriteObjectToNode(ndType, _typePointer);
			}
			if (TypeParameters != null)
			{
				XmlNode tpsNode = XmlUtil.CreateSingleNewElement(node, XML_TypeParams);
				tpsNode.RemoveAll();
				for (int i = 0; i < TypeParameters.Length; i++)
				{
					XmlNode tNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					tpsNode.AppendChild(tNode);
					TypeParameters[i].OnWriteToXmlNode(writer, tNode);
				}
			}
		}

		public virtual void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)reader0;
			XmlNode ndType = node.SelectSingleNode(TYPE);
			if (ndType != null)
			{
				object v;
				string classIds;
				Type t = XmlUtil.GetLibTypeAttribute(ndType, out classIds);
				if (t != null && typeof(ClassPointer).Equals(t))
				{
					UInt32 classId = XmlUtil.GetAttributeUInt(ndType, XmlTags.XMLATT_ClassID);
					v = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
				}
				else
				{
					v = reader.ReadObject(ndType, this);
				}
				if (v == null)
				{
					reader.addErrStr2("Cannot load data type {0}", node.InnerXml);
				}
				else
				{
					SetDataType(v);
					if (this.BaseClassType == null)
					{
						reader.addErrStr2("Data type is not loaded from {0}. path:[{1}]", node.InnerXml, XmlUtil.GetPath(node));
					}
				}
			}
			XmlNodeList nds = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}/{1}", XML_TypeParams, XmlTags.XML_Item));
			if (nds != null && nds.Count > 0)
			{
				TypeParameters = new DataTypePointer[nds.Count];
				for (int i = 0; i < nds.Count; i++)
				{
					TypeParameters[i] = new DataTypePointer();
					TypeParameters[i].OnReadFromXmlNode(reader0, nds[i]);
				}
			}
			else
			{
				TypeParameters = null;
			}
		}
		#endregion
	}
	class dataType0
	{
	}
	class dataType1
	{
	}
	class dataType2
	{
	}
	class dataType3
	{
	}
}
