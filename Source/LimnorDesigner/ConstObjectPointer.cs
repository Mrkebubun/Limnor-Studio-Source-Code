/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Reflection;
using LimnorDesigner.MethodBuilder;
using System.Drawing.Design;
using MathExp;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using XmlUtility;
using System.Xml;
using XmlSerializer;
using System.CodeDom;
using System.Globalization;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using Limnor.PhpComponents;
using ProgElements;
using LimnorDesigner.Property;
using System.Collections;
using VSPrj;
using System.IO;
using TraceLog;

namespace LimnorDesigner
{
	/// <summary>
	/// represent a constant object
	/// </summary>
	public class ConstObjectPointer : DataTypePointer, ICustomTypeDescriptor, IDynamicProperties, IBeforeSerializeNotify
	{
		#region fields and constructors
		public const string VALUE_Value = "Value";
		public const string VALUE_Constructor = "Constructor";
		public const string VALUE_Type = "Type";
		public const string VALUE_ValueType = "ValueType";
		const string XMLATT_isFile = "isFile";
		private UInt32 _id;
		private object _value;//value for primary type, string, UITypeEditor created value, an instance of ClassPointer

		private string _name;
		private Type _scope; //limit the type selection
		private Type _targetType; //compiling type
		//
		private NullObjectPointer _nullPointer;
		//
		private ConstructorInfo[] _constructors;
		private ConstructorInfoClass _constructorToUse;
		//
		private List<ConstructorClass> _customConstructors;
		private ConstructorClass _customConstructorToUse;
		//
		private ParameterValue _ownerParameter;
		private ParameterValueArrayItem[] _arrayItems;
		//
		private Dictionary<string, object> _values; //values for constructor parameters
		private PropertyGrid _propertyGrid;
		private EventHandler _onValueChanged;
		public ConstObjectPointer()
		{
		}
		public ConstObjectPointer(DataTypePointer type)
		{
			SetDataType(type);
			initDefaultValue();
		}
		public ConstObjectPointer(TypePointer type)
			: base(type)
		{
			initDefaultValue();
		}
		public ConstObjectPointer(ClassPointer type)
			: base(type)
		{
			initDefaultValue();
		}
		public ConstObjectPointer(string name, Type type)
			: base(new TypePointer(type))
		{
			_name = name;
			initDefaultValue();
		}

		#endregion
		#region Properties
		public override bool IsLibType
		{
			get
			{
				if (ConcreteType != null)
					return ConcreteType.IsLibType;
				return base.IsLibType;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool UseValueEnum { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type TargetCompileType
		{
			get
			{
				if (_targetType == null)
				{
					if (ConcreteType != null)
					{
						return ConcreteType.BaseClassType;
					}
					return BaseClassType;
				}
				return _targetType;
			}
		}

		[Browsable(false)]
		[ReadOnly(true)]
		public EditorAttribute UseValueEditorAttribute { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public AttributeConstructor NamedValues
		{
			get
			{
				AttributeConstructor ac = _value as AttributeConstructor;
				if (ac == null)
				{
					if (_constructorToUse != null)
					{
						if (_constructorToUse.ParameterCount == 0)
						{
							ac = new AttributeConstructor(this);
							_value = ac;
						}
					}
				}
				return ac;
			}
		}
		[Browsable(false)]
		public bool IsNull
		{
			get
			{
				if (_nullPointer != null)
				{
					return true;
				}
				if (IsLibType)
				{
					return (_constructorToUse != null);
				}
				return (_customConstructorToUse != null);
			}
		}
		public int ParameterCount
		{
			get
			{
				if (_nullPointer != null)
					return 0;
				if (this.IsLibType)
				{
					if (_constructorToUse != null)
					{
						return _constructorToUse.ParameterCount;
					}
				}
				else
				{
					if (_customConstructors != null)
					{
						return _customConstructorToUse.ParameterCount;
					}
				}
				return 0;
			}
		}
		[Browsable(false)]
		public UInt32 ValueId
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
		}
		static List<ConstObjectPointer> _dup = null;
		[Browsable(false)]
		[ReadOnly(true)]
		public object Value
		{
			get
			{
				if (_value == null)
				{
					if (_values != null && _values.Count > 0)
					{
						if (_constructorToUse != null && _constructorToUse.ParameterCount > 0)
						{
							if (_constructorToUse.ParameterCount == _values.Count)
							{
								object[] ps = new object[_values.Count];
								_values.Values.CopyTo(ps, 0);
								bool start = false;
								try
								{
									if (_dup == null)
									{
										_dup = new List<ConstObjectPointer>();
										_dup.Add(this);
										start = true;
									}
									else
									{
										if (_dup.Contains(this))
										{
											return null;
										}
										else
										{
											_dup.Add(this);
										}
									}
									for (int i = 0; i < ps.Length; i++)
									{
										ParameterValue pv = ps[i] as ParameterValue;
										if (pv != null)
										{
											ConstObjectPointer cp = pv.ConstantValue;
											if (cp != null)
											{
												ps[i] = cp.Value;//possible cyclic reference
												if (ps[i] == null)
												{
													ps[i] = VPLUtil.GetDefaultValue(cp.ObjectType);
												}
											}
											else
											{
												ps[i] = VPLUtil.GetDefaultValue(pv.ObjectType);
											}
										}
									}
									ParameterInfo[] pifs = _constructorToUse.Constructor.GetParameters();
									for (int i = 0; i < ps.Length; i++)
									{
										if (ps[i] == null)
										{
											ps[i] = VPLUtil.GetDefaultValue(pifs[i].ParameterType);
										}
									}
									_value = _constructorToUse.Constructor.Invoke(ps);
								}
								catch
								{
								}
								finally
								{
									if (start)
									{
										_dup = null;
									}
								}
							}
						}
					}
					else
					{
						if (_constructorToUse != null && _constructorToUse.ParameterCount == 0)
						{
							object[] ps = new object[] { };
							Type gt = this.MakeGenericType();
							if (gt != null)
							{
								_value = Activator.CreateInstance(gt);
							}
							else
							{
								_value = _constructorToUse.Constructor.Invoke(ps);
							}
						}
					}
				}
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public List<ConstructorClass> CustomConstructors
		{
			get
			{
				return _customConstructors;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public ConstructorInfo[] Constructors
		{
			get
			{
				if (_constructors == null)
				{
					if (ConcreteType != null)
					{
						return ConcreteType.BaseClassType.GetConstructors();
					}
					_constructors = this.BaseClassType.GetConstructors();
				}
				return _constructors;
			}
			set
			{
				_constructors = value;
			}
		}
		/// <summary>
		/// when it is used as a parameter, the type is determined by the parameter name
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public bool NotSaveType
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		#endregion
		#region Methods
		public void OnParameterChanged()
		{
			if (_ownerParameter != null)
			{
				IAction act = _ownerParameter.ActionContext as IAction;
				if (act != null)
				{
					EventHandler h = act.GetPropertyChangeHandler();
					if (h != null)
					{
						h(this, EventArgs.Empty);
					}
				}
			}
		}
		public void SetOwnerParameter(ParameterValue owner)
		{
			_ownerParameter = owner;
		}
		public int GetArraySize()
		{
			Array a = _value as Array;
			if (a != null)
			{
				if (_arrayItems != null)
				{
					return _arrayItems.Length;
				}
				return a.Length;
			}
			return 0;
		}
		public void SetArraySize(int size)
		{
			if (size >= 0)
			{
				Array a = _value as Array;
				if (a != null && a.Length != size)
				{
					Type tItem = _value.GetType().GetElementType();
					Array a0 = Array.CreateInstance(tItem, size);
					if (a.Length < size)
					{
						a.CopyTo(a0, 0);
						for (int i = a.Length; i < size; i++)
						{
							a0.SetValue(VPLUtil.GetDefaultValue(tItem), i);
						}
					}
					else
					{
						for (int i = 0; i < size; i++)
						{
							a0.SetValue(a.GetValue(i), i);
						}
					}
					_value = a0;
					validateArrayItems();
				}
			}
		}
		public ParameterValueArrayItem GetArrayItem(int index)
		{
			validateArrayItems();
			if (_arrayItems != null)
			{
				if (index >= 0 && index < _arrayItems.Length)
				{
					return _arrayItems[index];
				}
			}
			return null;
		}
		public void SetArrayItem(int index, object value)
		{
			validateArrayItems();
			if (_arrayItems != null && index >= 0 && index < _arrayItems.Length)
			{
				ParameterValueArrayItem pv = value as ParameterValueArrayItem;
				if (pv != null)
				{
					_arrayItems[index] = pv;
				}
				else
				{
					_arrayItems[index].SetValue(value);
				}
			}
		}
		public Type ValueType
		{
			get
			{
				if (ConcreteType != null)
					return ConcreteType.DataTypeEx;
				return this.DataTypeEx;// BaseClassType;
			}
		}
		public void AddArrayItem(object value)
		{
			Array a = _value as Array;
			if (a != null)
			{
				Type tItem = _value.GetType().GetElementType();
				Array a0 = Array.CreateInstance(tItem, a.Length + 1);
				a.CopyTo(a0, 0);
				a0.SetValue(value, a.Length);
				_value = a0;
				validateArrayItems();
			}
		}
		[Browsable(false)]
		public ParameterValueArrayItem[] ArrayValues
		{
			get
			{
				return _arrayItems;
			}
		}
		[Browsable(false)]
		public ParameterValue OwnerParameter
		{
			get
			{
				return _ownerParameter;
			}
		}
		public void SetPropertyGrid(PropertyGrid pg)
		{
			_propertyGrid = pg;
		}
		public void MakeNull()
		{
			if (_nullPointer == null)
				_nullPointer = new NullObjectPointer(this);
		}
		public object GetValue(string name)
		{
			if (string.Compare(VALUE_Value, name, StringComparison.Ordinal) == 0)
			{
				return _value;
			}
			if (string.Compare(name, VALUE_Constructor, StringComparison.Ordinal) == 0)
			{
				if (_nullPointer != null)
					return _nullPointer;
				if (this.IsLibType)
					return _constructorToUse;
				else
					return _customConstructorToUse;
			}
			if (string.Compare(name, VALUE_Type, StringComparison.Ordinal) == 0)
			{
				return _value;
			}
			return null;
		}
		public void SetOnValueChanged(EventHandler handler)
		{
			_onValueChanged = handler;
		}
		public void SetTypeScope(Type t)
		{
			_scope = t;
		}
		public void SetTargetType(Type t)
		{
			_targetType = t;
		}
		public void SetValue(string name, object value)
		{
			if (string.Compare(name, VALUE_ValueType, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_value = value;
			}
			else if (string.Compare(name, VALUE_Value, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null)
				{
					_nullPointer = new NullObjectPointer(this);
				}
				else
				{
					if (typeof(Type).Equals(ValueType))
					{
						Type t = value as Type;
						if (t != null)
						{
							_value = new DataTypePointer(new TypePointer(t));
						}
						else
						{
							DataTypePointer dp = value as DataTypePointer;
							if (dp != null)
							{
								_value = dp;
							}
							else
							{
								TypePointer tp = value as TypePointer;
								if (tp != null)
								{
									dp = new DataTypePointer(tp);
									_value = dp;
								}
								else
								{
									ClassPointer cp = value as ClassPointer;
									if (cp != null)
									{
										dp = new DataTypePointer(cp);
										_value = dp;
									}
									else
									{
										throw new DesignerException("Value {0} is not a type pointer", value.GetType().Name);
									}
								}
							}
						}
					}
					else
					{
						bool b;
						object val = VPLUtil.ConvertObject(value, ValueType, out b);
						if (b)
						{
							_value = val;
							_nullPointer = null;
						}
					}
				}
				if (_onValueChanged != null)
				{
					PropertyChangeEventArg pe = new PropertyChangeEventArg(_name, _value);
					_onValueChanged(this, pe);
				}
			}
			else if (string.Compare(name, VALUE_Constructor, StringComparison.OrdinalIgnoreCase) == 0)
			{
				ConstructorInfo cif = value as ConstructorInfo;
				if (cif != null)
				{
					_constructorToUse = new ConstructorInfoClass(cif);
					_customConstructorToUse = null;
					_nullPointer = null;
					_propertyCollection = null;
				}
				else
				{
					_constructorToUse = value as ConstructorInfoClass;
					_customConstructorToUse = value as ConstructorClass;
					_nullPointer = value as NullObjectPointer;
					_propertyCollection = null;
				}
				if (_onValueChanged != null)
				{
					_onValueChanged(this, EventArgs.Empty);
				}
				if (_propertyGrid != null)
				{
					_propertyGrid.Refresh();
				}
			}
			else if (string.Compare(name, VALUE_Type, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null)
				{
					_value = new DataTypePointer(new TypePointer(typeof(object)));
				}
				else
				{
					DataTypePointer dp = value as DataTypePointer;
					if (dp != null)
					{
						_value = dp;
					}
					else
					{
						TypePointer tp = value as TypePointer;
						if (tp != null)
						{
							_value = new DataTypePointer(tp);
						}
						else
						{
							Type t = value as Type;
							if (t != null)
							{
								_value = new DataTypePointer(new TypePointer(t));
							}
							else
							{
								ClassPointer cp = value as ClassPointer;
								if (cp != null)
								{
									_value = new DataTypePointer(cp);
								}
								else
								{
									throw new DesignerException("{0} is not a type", value);
								}
							}
						}
					}
				}
			}
		}
		public void ResetValue(string name)
		{
		}
		public bool ShouldSerializeValue(string name)
		{
			return true;
		}
		public object GetParameterVlaue(string name)
		{
			if (name == null)
			{
				name = "";
			}
			if (_values != null)
			{
				object v;
				if (_values.TryGetValue(name, out v))
				{
					return v;
				}
			}
			if (this.IsLibType)
			{
				if (_constructorToUse != null)
				{
					ParameterInfo pif = _constructorToUse.GetParameter(name);
					if (pif == null)
						throw new DesignerException("Parameter not found: {0}", name);
					return VPLUtil.GetDefaultValue(pif.ParameterType);
				}
			}
			else
			{
				if (_customConstructorToUse != null)
				{
					int i = _customConstructorToUse.GetParameterIndexByName(name);
					if (i < 0)
					{
						throw new DesignerException("Parameter not found: {0}", name);
					}
					ParameterClass pc = _customConstructorToUse.Parameters[i];
					if (pc.IsLibType)
					{
						return VPLUtil.GetDefaultValue(pc.BaseClassType);
					}
				}
			}
			return null;
		}
		public void SetParameterVlaue(string name, object value)
		{
			if (_values == null)
			{
				_values = new Dictionary<string, object>();
			}
			if (_values.ContainsKey(name))
			{
				_values[name] = value;
			}
			else
			{
				_values.Add(name, value);
			}
		}
		public string GetParameterName(int i)
		{
			if (IsLibType)
			{
				if (_constructorToUse != null)
				{
					ParameterInfo pif = _constructorToUse.GetParameter(i);
					if (pif != null)
					{
						return pif.Name;
					}
				}
			}
			else
			{
				if (_customConstructorToUse != null)
				{
					ParameterClass p = _customConstructorToUse.Parameters[i];
					return p.Name;
				}
			}
			return null;
		}
		public string ToValueDisplay()
		{
			if (_nullPointer != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"({0})null", this.TypeName);
			}
			if (_constructorToUse != null)
			{
				return _constructorToUse.ToString();
			}
			if (_customConstructorToUse != null)
			{
				return _customConstructorToUse.ToString();
			}
			return "";
		}
		public override string ToString()
		{
			StringBuilder sb;
			if (this.IsArray && _arrayItems != null && _arrayItems.Length > 0)
			{
				sb = new StringBuilder();
				sb.Append(this.TypeName);
				sb.Append("=[");
				for (int i = 0; i < _arrayItems.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(",");
					}
					sb.Append(_arrayItems[i].ToString());
				}
				sb.Append("]");
				return sb.ToString();
			}
			if (_nullPointer != null)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"({0})null", this.TypeName);
			}
			if (this.IsLibType)
			{
				if (typeof(Type).Equals(ValueType))
				{
					if (_value == null)
						return "(Type)null";
					return _value.ToString();
				}
				else if (this.ValueType.IsPrimitive || typeof(string).Equals(this.ValueType) || typeof(Type).Equals(ValueType) || typeof(object).Equals(ValueType))
				{
					if (_value == null)
						return "";
					return _value.ToString();
				}
				else
				{
					if (VPLUtil.UseTypeEditor(this.ValueType))
					{
						if (_value == null)
							return "";
						return _value.ToString();
					}
					if (ConstValueAttribute.IsConstValue(this.ValueType))
					{
						if (_value != null)
						{
							return _value.ToString();
						}
					}
				}
			}
			sb = new StringBuilder();
			string s;
			if (this.IsLibType)
				s = this.ObjectType.Name;
			else
				s = this.ClassTypePointer.Name;
			Type tx = VPLUtil.GetObjectType(ValueType);
			if (typeof(Attribute).IsAssignableFrom(tx))
			{
				if (s.EndsWith("Attribute"))
				{
					s = s.Substring(0, s.Length - "Attribute".Length);
				}
			}
			sb.Append(s);
			sb.Append("(");
			if (this.IsLibType)
			{
				if (_constructorToUse != null)
				{
					int n = _constructorToUse.ParameterCount;
					if (n > 0)
					{
						ParameterInfo pif = _constructorToUse.GetParameter(0);
						object v = GetParameterVlaue(pif.Name);
						if (v == null)
						{
							sb.Append("null");
						}
						else
						{
							if (v is string)
							{
								sb.Append("\"" + v.ToString() + "\"");
							}
							else
							{
								sb.Append(v.ToString());
							}
						}
						for (int i = 1; i < n; i++)
						{
							pif = _constructorToUse.GetParameter(i);
							v = GetParameterVlaue(pif.Name);
							sb.Append(",");
							if (v == null)
							{
								sb.Append("null");
							}
							else
							{
								if (v is string)
								{
									sb.Append("\"" + v.ToString() + "\"");
								}
								else
								{
									sb.Append(v.ToString());
								}
							}
						}
					}
				}
			}
			else
			{
				if (_customConstructorToUse != null)
				{
					int n = _customConstructorToUse.ParameterCount;
					if (n > 0)
					{
						List<ParameterClass> list = _customConstructorToUse.Parameters;
						ParameterClass pif = list[0];
						object v = GetParameterVlaue(pif.Name);
						if (v == null)
						{
							sb.Append("null");
						}
						else
						{
							if (v is string)
							{
								sb.Append("\"" + v.ToString() + "\"");
							}
							else
							{
								sb.Append(v.ToString());
							}
						}
						for (int i = 1; i < n; i++)
						{
							pif = list[i];
							v = GetParameterVlaue(pif.Name);
							sb.Append(",");
							if (v == null)
							{
								sb.Append("null");
							}
							else
							{
								if (v is string)
								{
									sb.Append("\"" + v.ToString() + "\"");
								}
								else
								{
									sb.Append(v.ToString());
								}
							}
						}
					}
				}
			}
			sb.Append(")");
			return sb.ToString();
		}
		protected override void OnDataTypeChanged()
		{
			base.OnDataTypeChanged();
			bool bt;
			_value = VPLUtil.ConvertObject(_value, ValueType, out bt);
			if (!bt)
			{
			}
			_propertyCollection = null;
		}
		#endregion
		#region PropertyValueDescriptor
		class PropertyValueDescriptor : PropertyDescriptor
		{
			private ConstObjectPointer _owner;
			private Type _valueType;
			private bool _readOnly;
			private string _valueName;
			public PropertyValueDescriptor(ConstObjectPointer owner, string valueName, Type valueType, bool readOnly, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_owner = owner;
				_valueType = valueType;
				_valueName = valueName;
				_readOnly = readOnly;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(ConstObjectPointer); }
			}

			public override object GetValue(object component)
			{
				return _owner.GetValue(_valueName);
			}

			public override bool IsReadOnly
			{
				get { return _readOnly; }
			}

			public override Type PropertyType
			{
				get { return _valueType; }
			}

			public override void ResetValue(object component)
			{
				_owner.ResetValue(_valueName);
			}

			public override void SetValue(object component, object value)
			{
				_owner.SetValue(_valueName, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return _owner.ShouldSerializeValue(_valueName);
			}
		}
		#endregion
		#region PropertyValueDescriptor
		class CustomConstructorValueDescriptor : PropertyDescriptor
		{
			private ConstObjectPointer _owner;
			//private Type _valueType;
			private bool _readOnly;
			private string _valueName;
			public CustomConstructorValueDescriptor(ConstObjectPointer owner, string valueName, bool readOnly, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_owner = owner;
				//_valueType = valueType;
				_valueName = valueName;
				_readOnly = readOnly;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(ConstObjectPointer); }
			}

			public override object GetValue(object component)
			{
				return _owner.GetValue(_valueName);
			}

			public override bool IsReadOnly
			{
				get { return _readOnly; }
			}

			public override Type PropertyType
			{
				get { return typeof(ConstructorClass); }
			}

			public override void ResetValue(object component)
			{
				_owner.ResetValue(_valueName);
			}

			public override void SetValue(object component, object value)
			{
				_owner.SetValue(_valueName, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return _owner.ShouldSerializeValue(_valueName);
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
		private PropertyDescriptorCollection _propertyCollection;
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (this.IsLibType)
			{
				if (typeof(object).Equals(ValueType))
				{
					Attribute[] attrs;
					int n;
					if (attributes == null)
						n = 0;
					else
						n = attributes.Length;

					if (UseValueEnum)
					{
						attrs = new Attribute[n + 2];
						if (n > 0)
						{
							attributes.CopyTo(attrs, 0);
						}
						EditorAttribute a = new EditorAttribute(typeof(TypeEditorValueEnum), typeof(UITypeEditor));
						attrs[n + 1] = a;
					}
					else
					{
						attrs = new Attribute[n + 1];
						if (n > 0)
						{
							attributes.CopyTo(attrs, 0);
						}
					}
					TypeConverterAttribute tca = new TypeConverterAttribute(typeof(ObjectStringTypeConverter));
					attrs[n] = tca;
					//from UI, the only reasonable type is string
					PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, typeof(object), false, Name, attrs);
					_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
				}
				else if (typeof(Type).Equals(ValueType))
				{
					int n;
					if (attributes == null)
						n = 0;
					else
						n = attributes.Length;
					Attribute[] attrs = new Attribute[n + 2];
					EditorAttribute a = new EditorAttribute(typeof(PropEditorDataType), typeof(UITypeEditor));
					attrs[0] = a;
					TypeScopeAttribute sa = new TypeScopeAttribute(_scope);
					attrs[1] = sa;
					if (n > 0)
					{
						attributes.CopyTo(attrs, 2);
					}
					PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, ValueType, false, Name, attrs);
					_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
				}
				else if (this.ValueType.IsPrimitive || typeof(string).Equals(this.ValueType))
				{
					Attribute[] attrs;
					if (UseValueEnum)
					{
						int n;
						if (attributes == null)
							n = 0;
						else
							n = attributes.Length;
						attrs = new Attribute[n + 1];
						if (n > 0)
						{
							attributes.CopyTo(attrs, 0);
						}
						EditorAttribute a = new EditorAttribute(typeof(TypeEditorValueEnum), typeof(UITypeEditor));
						attrs[n] = a;
					}
					else
					{
						if (UseValueEditorAttribute != null)
						{
							int n;
							if (attributes == null)
								n = 0;
							else
								n = attributes.Length;
							attrs = new Attribute[n + 1];
							if (n > 0)
							{
								attributes.CopyTo(attrs, 0);
							}
							attrs[n] = UseValueEditorAttribute;
						}
						else
						{
							if ((typeof(string).Equals(ValueType)))
							{
								bool hasEditor = false;
								if (attributes != null && attributes.Length > 0)
								{
									for (int i = 0; i < attributes.Length; i++)
									{
										if (attributes[i] is EditorAttribute)
										{
											hasEditor = true;
											break;
										}
									}
								}
								if (hasEditor)
								{
									attrs = attributes;
								}
								else
								{
									int n;
									if (attributes == null)
										n = 0;
									else
										n = attributes.Length;
									attrs = new Attribute[n + 1];
									if (n > 0)
									{
										attributes.CopyTo(attrs, 0);
									}
									attrs[n] = new EditorAttribute(typeof(TypeSelectorText), typeof(UITypeEditor));
								}
							}
							else
							{
								attrs = attributes;
							}
						}
					}
					PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, ValueType, false, Name, attrs);
					_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
					if (_value == null || !_value.GetType().Equals(ValueType))
					{
						_value = VPLUtil.GetDefaultValue(ValueType);
					}
				}
				else if (ValueType.IsArray && UseValueEditorAttribute == null)
				{
					if (_value == null || !_value.GetType().Equals(ValueType))
					{
						_value = VPLUtil.GetDefaultValue(ValueType);
					}
					Type tIem = ValueType.GetElementType();
					List<PropertyDescriptor> l = new List<PropertyDescriptor>();
					l.Add(new PropertyDescriptorArraySize(this));
					Array a = (Array)_value;
					for (int i = 0; i < a.Length; i++)
					{
						l.Add(new PropertyDescriptorArrayItem(this, i, tIem, a.GetValue(i)));
					}
					_propertyCollection = new PropertyDescriptorCollection(l.ToArray());

				}
				else
				{
					if (UseValueEditorAttribute != null)
					{
						int n;
						if (attributes == null)
							n = 0;
						else
							n = attributes.Length;
						Attribute[] attrs = new Attribute[n + 1];
						if (n > 0)
						{
							attributes.CopyTo(attrs, 0);
						}
						attrs[n] = UseValueEditorAttribute;
						PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, ValueType, false, Name, attrs);
						_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
						if (_value == null || !_value.GetType().Equals(ValueType))
						{
							_value = VPLUtil.GetDefaultValue(ValueType);
						}
					}
					else
					{
						PropertyDescriptor vdUI = findUIEditor(this.ValueType, null);
						if (vdUI == null)
						{
							//use constructor to input data
							ConstructorInfo[] cifs = this.ValueType.GetConstructors();
							Constructors = cifs;
							//
							if (cifs == null || cifs.Length == 0)
							{
								PropertyDescriptor propValue = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, ValueType, false, "Value", attributes);
								_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { propValue });
							}
							//
							else
							{
								Attribute[] attrs = new Attribute[2];
								attrs[0] = new EditorAttribute(typeof(ConstructorSelection), typeof(UITypeEditor));
								attrs[1] = new RefreshPropertiesAttribute(RefreshProperties.All);
								PropertyValueDescriptor vd = new PropertyValueDescriptor(this, VALUE_Constructor, typeof(ConstructorInfo), false, "ConstructorOf" + TypeName, attrs);
								if (_constructorToUse == null || _constructorToUse.ParameterCount == 0)
								{
									_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
								}
								else
								{
									PropertyDescriptor[] props = new PropertyDescriptor[_constructorToUse.ParameterCount + 1];
									props[0] = vd; //show current constructor and enable constructor selection from the constructor list
									for (int i = 0; i < _constructorToUse.ParameterCount; i++)
									{
										ParameterInfo p = _constructorToUse.GetParameter(i);
										props[i + 1] = createParameterDescriptor(p.ParameterType, p.Name);
									}
									_propertyCollection = new PropertyDescriptorCollection(props);
								}
							}
						}
					}
				}
				return _propertyCollection;
			}
			else //ClassPointer
			{
				//use constructor to input data
				_customConstructors = this.ClassTypePointer.GetConstructors();
				if (_customConstructors == null || _customConstructors.Count == 0)
				{
					MathNode.Log(TraceLogClass.MainForm, new DesignerException("Constructor not found for type {0}", ClassTypePointer.Name));
					_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
				}
				else
				{
					Attribute[] attrs = new Attribute[2];
					attrs[0] = new EditorAttribute(typeof(CustomConstructorSelection), typeof(UITypeEditor));
					attrs[1] = new RefreshPropertiesAttribute(RefreshProperties.All);
					CustomConstructorValueDescriptor vd = new CustomConstructorValueDescriptor(this, VALUE_Constructor, false, "ConstructorOf" + Name, attrs);
					if (_customConstructorToUse == null || _customConstructorToUse.ParameterCount == 0)
					{
						_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
					}
					else
					{
						PropertyDescriptor[] props = new PropertyDescriptor[_customConstructorToUse.ParameterCount + 1];
						props[0] = vd;
						for (int i = 0; i < _customConstructorToUse.ParameterCount; i++)
						{
							props[i + 1] = createParameterDescriptor(_customConstructorToUse.Parameters[i]);
						}
						_propertyCollection = new PropertyDescriptorCollection(props);
					}
				}

				return _propertyCollection;
			}
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
		#region private methods
		private void initDefaultValue()
		{
			_value = VPLUtil.GetDefaultValue(ValueType);
		}
		private void validateArrayItems()
		{
			if (_ownerParameter != null && _value != null)
			{
				Array a = _value as Array;
				if (a != null)
				{
					if (_arrayItems == null)
					{
						Type itemType = _value.GetType().GetElementType();
						_arrayItems = new ParameterValueArrayItem[a.Length];
						for (int i = 0; i < _arrayItems.Length; i++)
						{
							_arrayItems[i] = new ParameterValueArrayItem(_ownerParameter.ActionContext, this, i, itemType);
						}
					}
					else if (_arrayItems.Length != a.Length)
					{
						Type itemType = _value.GetType().GetElementType();
						ParameterValueArrayItem[] ais = new ParameterValueArrayItem[a.Length];
						int m = Math.Min(_arrayItems.Length, a.Length);
						for (int i = 0; i < m; i++)
						{
							ais[i] = _arrayItems[i];
						}
						for (int i = m; i < ais.Length; i++)
						{
							ais[i] = new ParameterValueArrayItem(_ownerParameter.ActionContext, this, i, itemType);
						}
						_arrayItems = ais;
					}
				}
			}
		}
		private PropertyDescriptor createParameterDescriptor(Type t, string name)
		{
			EditorAttribute ea0 = new EditorAttribute(typeof(SelectorEnumValueType), typeof(UITypeEditor));
			if (typeof(Type).Equals(t))
			{
				ParameterValueDescriptor vd = new ParameterValueDescriptor(this, t, name, new Attribute[] { ea0 });
				return vd;
			}
			else if (t.IsPrimitive || typeof(string).Equals(t))
			{
				ParameterValueDescriptor vd = new ParameterValueDescriptor(this, t, name, new Attribute[] { ea0 });
				return vd;
			}
			else
			{
				PropertyDescriptor uiProp = findUIEditor(t, name);
				if (uiProp != null)
				{
					return uiProp;
				}
				else
				{
					Attribute[] attrs = new Attribute[2];
					attrs[0] = new TypeConverterAttribute(typeof(ExpandableObjectConverter));
					attrs[1] = ea0;
					ConstObjectPointer cop = GetParameterVlaue(name) as ConstObjectPointer;
					if (cop == null)
					{
						cop = new ConstObjectPointer(name, t);
						SetParameterVlaue(name, cop);
					}
					cop.SetPropertyGrid(_propertyGrid);
					ParameterValueDescriptor vd = new ParameterValueDescriptor(this, typeof(ConstObjectPointer), name, attrs);
					return vd;
				}
			}
		}
		private PropertyDescriptor findUIEditor(Type t, string name)
		{
			//search for typeof(UITypeEditor)
			AttributeCollection ac = TypeDescriptor.GetAttributes(t);
			if (ac != null && ac.Count > 0)
			{
				bool bFound = false;
				foreach (Attribute a in ac)
				{
					if (a is EditorAttribute)
					{
						bFound = true;
						break;
					}
				}
				if (bFound)
				{
					Attribute[] attrs = new Attribute[ac.Count];
					ac.CopyTo(attrs, 0);
					if (typeof(Image).IsAssignableFrom(t))
					{
						for (int i = 0; i < attrs.Length; i++)
						{
							EditorAttribute ea = attrs[i] as EditorAttribute;
							if (ea != null)
							{
								if (ea.EditorTypeName.Contains("Forms.ResourceEditorSwitch"))
								{
									attrs[i] = new EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor));
								}
							}
						}
					}
					if (string.IsNullOrEmpty(name))
					{
						PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, t, false, Name, attrs);
						_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
						if (_value == null || !_value.GetType().Equals(t))
						{
							_value = VPLUtil.GetDefaultValue(t);
						}
						return vd;
					}
					else
					{
						ParameterValueDescriptor vd = new ParameterValueDescriptor(this, t, name, attrs);
						return vd;
					}
				}
			}
			else
			{
				object[] objs = t.GetCustomAttributes(true);
				if (objs != null && objs.Length > 0)
				{
					bool bFound = false;
					Attribute[] attrs = new Attribute[objs.Length];
					for (int i = 0; i < objs.Length; i++)
					{
						attrs[i] = (Attribute)objs[i];
						if (objs[i] is EditorAttribute)
						{
							bFound = true;
						}
					}
					if (bFound)
					{
						if (string.IsNullOrEmpty(name))
						{
							if (typeof(Image).IsAssignableFrom(t))
							{
								for (int i = 0; i < attrs.Length; i++)
								{
									EditorAttribute ea = attrs[i] as EditorAttribute;
									if (ea != null)
									{
										if (ea.EditorTypeName.Contains("Forms.ResourceEditorSwitch"))
										{
											attrs[i] = new EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor));
										}
									}
								}
							}
							PropertyValueDescriptor vd = new PropertyValueDescriptor(this, ConstObjectPointer.VALUE_Value, t, false, Name, attrs);
							_propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[] { vd });
							if (_value == null || !_value.GetType().Equals(t))
							{
								_value = VPLUtil.GetDefaultValue(t);
							}
							return vd;
						}
						else
						{
							ParameterValueDescriptor vd = new ParameterValueDescriptor(this, t, name, attrs);
							return vd;
						}
					}
				}
			}
			return null;
		}
		private PropertyDescriptor createParameterDescriptor(ParameterClass p)
		{
			if (p == null || p.BaseClassType == null)
			{
				throw new DesignerException("Type not defined");
			}
			if (p.IsLibType)
			{
				return createParameterDescriptor(p.BaseClassType, p.Name);
			}
			List<ConstructorClass> constructors = p.ClassTypePointer.GetConstructors();
			Attribute[] attrs = new Attribute[1];
			attrs[0] = new TypeConverterAttribute(typeof(ExpandableObjectConverter));
			if (constructors.Count == 0)
			{
				//no constructor defined, assume it has a default constructor
				ParameterValueConstDescriptor vd1 = new ParameterValueConstDescriptor(this, typeof(ClassPointer), p.Name, p.ClassTypePointer);
				return vd1;
			}
			ConstObjectPointer cop = GetParameterVlaue(p.Name) as ConstObjectPointer;
			if (cop == null)
			{
				cop = new ConstObjectPointer(p.ClassTypePointer);
				SetParameterVlaue(p.Name, cop);
			}
			cop.SetPropertyGrid(_propertyGrid);
			ParameterValueDescriptor vd = new ParameterValueDescriptor(this, typeof(ConstObjectPointer), p.Name, attrs);
			return vd;
		}

		#endregion
		#region ConstructorInfoClass
		class ConstructorInfoClass
		{
			private ConstructorInfo _cif;
			private string _key;
			private int _paramCount;
			private ParameterInfo[] _pifs;
			public ConstructorInfoClass(ConstructorInfo c)
			{
				if (c != null)
				{
					_cif = c;
					StringBuilder sb = new StringBuilder("(");
					_pifs = c.GetParameters();
					if (_pifs != null && _pifs.Length > 0)
					{
						_paramCount = _pifs.Length;
						sb.Append(_pifs[0].ParameterType.Name);
						sb.Append(" ");
						sb.Append(_pifs[0].Name);
						for (int i = 1; i < _pifs.Length; i++)
						{
							sb.Append(",");
							sb.Append(_pifs[i].ParameterType.Name);
							sb.Append(" ");
							sb.Append(_pifs[i].Name);
						}
					}
					sb.Append(")");
					_key = sb.ToString();
				}
			}
			public virtual string Name
			{
				get
				{
					return _key;
				}
			}
			public virtual int ParameterCount
			{
				get
				{
					return _paramCount;
				}
			}
			public virtual ConstructorInfo Constructor
			{
				get
				{
					return _cif;
				}
			}
			public virtual ParameterInfo GetParameter(int i)
			{
				return _pifs[i];
			}
			public virtual ParameterInfo GetParameter(string name)
			{
				for (int i = 0; i < _pifs.Length; i++)
				{
					if (_pifs[i].Name == name)
					{
						return _pifs[i];
					}
				}
				return null;
			}
			public override string ToString()
			{
				return _key;
			}
		}
		#endregion
		#region ConstructorSelection
		class ConstructorSelection : UITypeEditor
		{
			private ConstructorInfo[] _cifs;
			public ConstructorSelection()
			{
			}
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (context != null && context.Instance != null && provider != null)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (service != null)
					{
						ConstructorInfoClass c0 = value as ConstructorInfoClass;
						ConstObjectPointer cop = context.Instance as ConstObjectPointer;
						if (cop == null)
						{
							ParameterValue pv = context.Instance as ParameterValue;
							if (pv != null)
							{
								cop = pv.ConstantValue;
							}
						}
						if (cop != null)
						{
							ListBox list = new ListBox();
							list.Tag = service;
							list.Click += new EventHandler(list_Click);
							if (!cop.ValueType.IsValueType)
							{
								list.Items.Add(new NullObjectPointer(cop));
								list.SelectedIndex = 0;
							}
							_cifs = cop.Constructors;
							for (int i = 0; i < _cifs.Length; i++)
							{
								ConstructorInfoClass c = new ConstructorInfoClass(_cifs[i]);
								int n = list.Items.Add(c);
								if (c0 != null && c.Name == c0.Name)
								{
									list.SelectedIndex = n;
								}
							}
							service.DropDownControl(list);
							if (list.SelectedIndex >= 0)
							{
								value = list.Items[list.SelectedIndex];
								Type t = service.GetType();
								PropertyInfo pif0 = t.GetProperty("OwnerGrid");
								if (pif0 != null)
								{
									PropertyGrid pg = pif0.GetValue(service, null) as PropertyGrid;
									if (pg != null)
									{
										cop.SetPropertyGrid(pg);
									}
								}
							}
						}
					}
				}
				return value;
			}

			void list_Click(object sender, EventArgs e)
			{
				ListBox list = sender as ListBox;
				if (list.SelectedIndex >= 0)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)(list.Tag);
					service.CloseDropDown();
				}
			}
		}
		#endregion
		#region CustomConstructorSelection
		class CustomConstructorSelection : UITypeEditor
		{
			private List<ConstructorClass> _cifs;
			public CustomConstructorSelection()
			{
			}
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (context != null && context.Instance != null && provider != null)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (service != null)
					{
						ConstructorClass c0 = value as ConstructorClass;
						NullObjectPointer np = value as NullObjectPointer;
						ConstObjectPointer cop = context.Instance as ConstObjectPointer;
						if (cop == null)
						{
							ParameterValue pv = context.Instance as ParameterValue;
							if (pv != null)
							{
								cop = pv.ConstantValue;
							}
						}
						if (np == null && cop != null)
						{
							np = new NullObjectPointer(cop);
						}
						ListBox list = new ListBox();
						list.Tag = service;
						list.Click += new EventHandler(list_Click);
						list.Items.Add(np);
						list.SelectedIndex = 0;
						if (cop != null)
						{
							_cifs = cop.CustomConstructors;
						}
						if (_cifs != null)
						{
							for (int i = 0; i < _cifs.Count; i++)
							{
								int n = list.Items.Add(_cifs[i]);
								if (c0 != null && _cifs[i].MethodSignature == c0.MethodSignature)
								{
									list.SelectedIndex = n;
								}
							}
						}
						service.DropDownControl(list);
						if (list.SelectedIndex >= 0)
						{
							value = list.Items[list.SelectedIndex];
						}
					}
				}
				return value;
			}

			void list_Click(object sender, EventArgs e)
			{
				ListBox list = sender as ListBox;
				if (list.SelectedIndex >= 0)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)(list.Tag);
					service.CloseDropDown();
				}
			}
		}
		#endregion
		#region ParameterValueDescriptor
		internal class ParameterValueDescriptor : PropertyDescriptor
		{
			private ConstObjectPointer _owner;
			private Type _dataType;
			public ParameterValueDescriptor(ConstObjectPointer owner, Type dataType, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_owner = owner;
				_dataType = dataType;
			}
			public ConstObjectPointer Owner
			{
				get
				{
					return _owner;
				}
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(ConstObjectPointer); }
			}

			public override object GetValue(object component)
			{
				return _owner.GetParameterVlaue(Name);
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _dataType; }
			}

			public override void ResetValue(object component)
			{
				_owner.SetParameterVlaue(Name, VPLUtil.GetDefaultValue(_dataType));
			}

			public override void SetValue(object component, object value)
			{
				_owner.SetParameterVlaue(Name, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region ParameterValueConstDescriptor
		class ParameterValueConstDescriptor : ParameterValueDescriptor
		{
			private object _value;
			public ParameterValueConstDescriptor(ConstObjectPointer owner, Type dataType, string name, object val)
				: base(owner, dataType, name, new Attribute[] { })
			{
				_value = val;
			}
			public override bool CanResetValue(object component)
			{
				return false;
			}
			public override bool IsReadOnly
			{
				get { return true; }
			}
			public override void ResetValue(object component)
			{
			}
			public override object GetValue(object component)
			{
				return _value;
			}
			public override void SetValue(object component, object value)
			{
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XML_ARRAY = "Array";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (!NotSaveType)
			{
				//write type
				base.OnWriteToXmlNode(writer, node);
			}
			//write ID
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ValueID, ValueId);
			//write value
			XmlNode valNode = node.SelectSingleNode(XmlTags.XML_Data);
			if (valNode != null)
			{
				node.RemoveChild(valNode);
			}
			valNode = node.OwnerDocument.CreateElement(XmlTags.XML_Data);
			ComponentID cid = _value as ComponentID;
			if (cid != null)
			{
				XmlUtil.SetAttribute(valNode, XmlTags.XMLATT_ComponentID, cid.ComponentId);
				node.AppendChild(valNode);
			}
			else
			{
				if (this.IsLibType)
				{
					if (typeof(Type).Equals(ValueType))
					{
						if (_value != null)
						{
							if (writer.WriteValue(valNode, _value, this) == WriteResult.WriteOK)
							{
								node.AppendChild(valNode);
							}
						}
					}
					else if (this.ValueType.IsPrimitive || typeof(string).Equals(this.ValueType) || (_value != null && _value is JsString) || (_value != null && _value is PhpString))
					{
						if (_value != null)
						{
							bool isFile = false;
							string sv = null;
							if (XmlObjectReader.ADJUSTPATH)
							{
								JsString js = _value as JsString;
								if (js != null)
								{
									sv = js.Value;
								}
								else
								{
									PhpString ps = _value as PhpString;
									if (ps != null)
									{
										sv = ps.Value;
									}
									else
									{
										sv = _value as string;
									}
								}
								XmlObjectWriter xow = writer as XmlObjectWriter;
								if (xow != null && xow.ObjectList != null)
								{
									LimnorProject prj = xow.ObjectList.Project;
									if (prj != null)
									{
										if (!string.IsNullOrEmpty(sv) && sv.Length > 2)
										{
											if (sv.StartsWith("\\\\") || sv[1] == ':')
											{
												try
												{
													if (File.Exists(sv))
													{
														if (!sv.StartsWith(prj.ProjectFolder, StringComparison.OrdinalIgnoreCase))
														{
															string resDir = Path.Combine(prj.ProjectFolder, XmlObjectReader.PRJRESOURCESFOLDERNAME);
															if (!Directory.Exists(resDir))
															{
																Directory.CreateDirectory(resDir);
															}
															string resPath = Path.Combine(resDir, Path.GetFileName(sv));
															if (!File.Exists(resPath))
															{
																File.Copy(sv, resPath);
															}
															sv = string.Format(CultureInfo.InvariantCulture, "$$${0}\\{1}", XmlObjectReader.PRJRESOURCESFOLDERNAME, Path.GetFileName(sv));
															isFile = true;
														}
													}
												}
												catch
												{
												}
											}
										}
									}
								}
							}
							WriteResult bret;
							if (isFile)
							{
								bret = writer.WriteValue(valNode, sv, this);
								XmlUtil.SetAttribute(valNode, XMLATT_isFile, true);
							}
							else
							{
								bret = writer.WriteValue(valNode, _value, this);
							}
							if (bret == WriteResult.WriteOK)
							{
								node.AppendChild(valNode);
							}
						}
					}
					else if (this.ValueType.IsArray)
					{
						if (_value != null)
						{
							if (writer.WriteValue(valNode, _value, this) == WriteResult.WriteOK)
							{
								node.AppendChild(valNode);
							}
						}
					}
					else
					{
						if (VPLUtil.UseTypeEditor(this.ValueType))
						{
							if (_value != null)
							{
								if (writer.WriteValue(valNode, _value, this) == WriteResult.WriteOK)
								{
									node.AppendChild(valNode);
								}
							}
						}
						else
						{
							if (this.ValueType.IsValueType && (_constructorToUse == null || _constructorToUse.ParameterCount == 0))
							{
								if (_value == null)
								{
									_value = VPLUtil.GetDefaultValue(this.ValueType);
								}
								if (_value != null)
								{
									TypeConverter converter = TypeDescriptor.GetConverter(_value);
									valNode.InnerText = converter.ConvertToInvariantString(_value);
								}
							}
							else
							{
								if (_nullPointer != null)
								{
									XmlUtil.SetAttribute(valNode, XmlTags.XMLATT_IsNull, true);
								}
								else
								{
									if (_constructorToUse != null)
									{
										XmlUtil.RemoveAttribute(valNode, XmlTags.XMLATT_IsNull);
										if (_constructorToUse.ParameterCount > 0)
										{
											for (int i = 0; i < _constructorToUse.ParameterCount; i++)
											{
												ParameterInfo pif = _constructorToUse.GetParameter(i);
												object v = GetParameterVlaue(pif.Name);
												XmlNode paramNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
												valNode.AppendChild(paramNode);
												XmlUtil.SetNameAttribute(paramNode, pif.Name);
												XmlUtil.SetLibTypeAttribute(paramNode, pif.ParameterType);
												if (typeof(Type).Equals(pif.ParameterType))
												{
													if (v == null)
													{
														XmlUtil.SetAttribute(paramNode, XmlTags.XMLATT_IsNull, true);
													}
													else
													{
														if (v is ConstObjectPointer)
														{
															writer.WriteObjectToNode(paramNode, v);
														}
														else
														{
															writer.WriteValue(paramNode, v, this);
														}
													}
												}
												else
												{
													writer.WriteValue(paramNode, v, this);
												}
											}
										}
										else
										{
											AttributeConstructor ac = _value as AttributeConstructor;
											if (ac != null)
											{
												writer.WriteObjectToNode(valNode, ac);
											}
											else
											{
												XmlNode nodeV = XmlUtil.CreateSingleNewElement(valNode, VALUE_Value);
												writer.WriteObjectToNode(nodeV, _value);
											}
										}
									}
									else
									{
										if (_value != null)
										{
											string s = _value as string;
											if (s != null)
											{
												valNode.InnerText = s;
											}
											else
											{
												XmlNode nodeV = XmlUtil.CreateSingleNewElement(valNode, VALUE_Value);
												writer.WriteObjectToNode(nodeV, _value);
											}
										}
										else
										{
											XmlUtil.SetAttribute(valNode, XmlTags.XMLATT_IsNull, true);
										}
									}
								}
							}
							node.AppendChild(valNode);
						}
					}
				}
				else //ClassPointer
				{
					if (_nullPointer != null)
					{
						XmlUtil.SetAttribute(valNode, XmlTags.XMLATT_IsNull, true);
					}
					else
					{
						if (_customConstructorToUse != null)
						{
							XmlUtil.RemoveAttribute(valNode, XmlTags.XMLATT_IsNull);
							List<ParameterClass> parameters = _customConstructorToUse.Parameters;
							if (parameters != null && parameters.Count > 0)
							{
								for (int i = 0; i < parameters.Count; i++)
								{
									ParameterClass pif = parameters[i];
									object v = GetParameterVlaue(pif.Name);
									XmlNode paramNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
									valNode.AppendChild(paramNode);
									XmlUtil.SetNameAttribute(paramNode, pif.Name);
									XmlUtil.SetLibTypeAttribute(paramNode, pif.BaseClassType);
									if (pif.IsLibType && typeof(Type).Equals(pif.BaseClassType))
									{
										if (v == null)
										{
											XmlUtil.SetLibTypeAttribute(paramNode, typeof(Type));
											XmlUtil.SetAttribute(paramNode, XmlTags.XMLATT_IsNull, true);
										}
										else
										{
											if (v is ConstObjectPointer)
											{
												writer.WriteObjectToNode(paramNode, v);
											}
											else
											{
												ConstObjectPointer cop = new ConstObjectPointer(pif.Name, typeof(Type));
												cop.SetValue(ConstObjectPointer.VALUE_Value, v);
												writer.WriteObjectToNode(paramNode, cop);
											}
										}
									}
									else
									{
										if (v != null)
										{
											writer.WriteObjectToNode(paramNode, v);
										}
									}
								}
							}
						}
						else
						{
							XmlUtil.SetAttribute(valNode, XmlTags.XMLATT_IsNull, true);
						}
					}
					node.AppendChild(valNode);
				}
			}
			if (_arrayItems != null)
			{
				XmlNode arrayNode = XmlUtil.CreateSingleNewElement(node, XML_ARRAY);
				for (int i = 0; i < _arrayItems.Length; i++)
				{
					XmlNode itemNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					arrayNode.AppendChild(itemNode);
					_arrayItems[i].OnWriteToXmlNode(writer, itemNode);
				}
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			if (!NotSaveType)
			{
				//get type
				base.OnReadFromXmlNode(reader0, node);
			}
			XmlObjectReader reader = (XmlObjectReader)reader0;
			Type valueType = this.BaseClassType;
			if (valueType == null)
			{
				reader.addErrStr2("Error reading ConstObjectPointer. Type is not null for[{0}]", this.Name);
				return;
			}
			if (valueType.IsGenericParameter)
			{
				if (reader.ObjectStack == null || reader.ObjectStack.Count == 0)
				{
					reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Reader does not have ObjectStack.", this.ToString());
					return;
				}
				IEnumerator ie = reader.ObjectStack.GetEnumerator();
				IAction act = null;
				while (ie.MoveNext())
				{
					act = ie.Current as IAction;
					if (act != null)
						break;
				}
				if (act == null)
				{
					reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Not in act context.", this.ToString());
					return;
				}
				if (act.ActionMethod == null)
				{
					reader.addErrStr2("Error reading ConstObjectPointer [{0}]. ActionMethod is null.", this.ToString());
					return;
				}
				ConcreteType = null;
				MethodPointer mp = act.ActionMethod as MethodPointer;
				if (mp != null)
				{
					ConcreteType = mp.GetConcreteType(valueType);
				}
				if (ConcreteType != null)
				{
					valueType = ConcreteType.BaseClassType;
				}
				else
				{
					ILocalvariable lv = act.ActionMethod.Owner as ILocalvariable;
					if (lv == null)
					{
						if (act.ActionMethod.Owner != null)
						{
							IObjectPointer op = act.ActionMethod.Owner.Owner as IObjectPointer;
							while (op != null)
							{
								lv = op as LocalVariable;
								if (lv != null)
								{
									break;
								}
								else
								{
									op = op.Owner;
								}
							}
						}
					}
					if (lv != null)
					{
						if (lv.ValueType == null)
						{
							reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Action receiver[{1}]'ValueType is null.", this.ToString(), lv);
							return;
						}
						else
						{
							ConcreteType = lv.ValueType.GetConcreteType(valueType);
							if (ConcreteType == null)
							{
								reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Generic parameter [{1}] does not match object type [{2}].", this.ToString(), valueType, lv.ValueType);
								return;
							}
							else
							{
								valueType = ConcreteType.BaseClassType;
							}
						}
					}
					else
					{
						IProperty ip = act.ActionMethod.Owner as IProperty;
						if (ip != null)
						{
							if (ip.PropertyType == null)
							{
								reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Action receiver[{1}]'s PropertyType is null.", this.ToString(), ip);
								return;
							}
							else
							{
								ConcreteType = ip.PropertyType.GetConcreteType(valueType);
								if (ConcreteType == null)
								{
									reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Generic parameter [{1}] does not match property type [{2}].", this.ToString(), valueType, ip.PropertyType);
									return;
								}
								else
								{
									valueType = ConcreteType.BaseClassType;
								}
							}
						}
						else
						{
							IGenericTypePointer igp = act.ActionMethod.Owner as IGenericTypePointer;
							if (igp != null)
							{
								ConcreteType = igp.GetConcreteType(valueType);
								if (ConcreteType == null)
								{
									reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Generic parameter [{1}] does not a concrete type the owner [{2}] holds.", this.ToString(), valueType, igp);
									return;
								}
								else
								{
									valueType = ConcreteType.BaseClassType;
								}
							}
							else
							{
								reader.addErrStr2("Error reading ConstObjectPointer [{0}]. Unsupported action executor [{1}] for using generic parameters.", this.ToString(), act.ActionMethod.Owner);
								return;
							}
						}
					}
				}
			}
			//get ID
			_id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ValueID);
			//get value
			XmlNode valNode = node.SelectSingleNode(XmlTags.XML_Data);
			if (valNode != null)
			{
				UInt32 cpid = XmlUtil.GetAttributeUInt(valNode, XmlTags.XMLATT_ComponentID);
				if (cpid != 0)
				{
					_value = reader.ObjectList.Project.GetComponentByID(cpid);
				}
				else
				{
					if (this.IsLibType)
					{
						if (typeof(Type).Equals(valueType))
						{
							_value = reader.ReadValue(valNode, this);
						}
						else if (valueType.IsPrimitive || typeof(string).Equals(valueType))
						{
							_value = reader.ReadValue(valNode, this, valueType);
							if (XmlUtil.GetAttributeBoolDefFalse(valNode, XMLATT_isFile))
							{
								string sf = _value as string;
								if (!string.IsNullOrEmpty(sf))
								{
									if (sf.StartsWith(XmlObjectReader.PRJPATHSYMBOLE, StringComparison.Ordinal))
									{
										sf = sf.Substring(XmlObjectReader.PRJPATHSYMBOLE.Length);
										if (reader.ObjectList != null && reader.ObjectList.Project != null)
										{
											sf = Path.Combine(reader.ObjectList.Project.ProjectFolder, sf);
										}
										_value = sf;
									}
								}
							}
						}
						else if (valueType.IsArray)
						{
							_value = reader.ReadValue(valNode, this, valueType);
						}
						else
						{
							if (VPLUtil.UseTypeEditor(valueType))
							{
								_value = reader.ReadValue(valNode, this, valueType);
							}
							else
							{
								string[] pnames;
								Type[] ts;
								XmlNodeList nList = valNode.SelectNodes(XmlTags.XML_Item);
								if (nList == null || nList.Count == 0)
								{
									ts = Type.EmptyTypes;
									pnames = new string[] { };
								}
								else
								{
									ts = new Type[nList.Count];
									pnames = new string[nList.Count];
									for (int i = 0; i < nList.Count; i++)
									{
										string name = XmlUtil.GetNameAttribute(nList[i]);
										object v = reader.ReadValue(nList[i], this);
										pnames[i] = name;
										ConstObjectPointer cop = v as ConstObjectPointer;
										if (cop != null)
										{
											v = cop.Value;
										}
										if (v != null)
										{
											SetParameterVlaue(name, v);
											DataTypePointer dp = v as DataTypePointer;
											if (dp != null)
											{
												ts[i] = typeof(Type);
											}
											else
											{
												ParameterValue parValue = v as ParameterValue;
												if (parValue != null)
												{
													ts[i] = parValue.ObjectType;
												}
												else
												{
													ts[i] = v.GetType();
												}
											}
										}
										else
										{
											ts[i] = XmlUtil.GetLibTypeAttribute(nList[i]);
										}
									}
								}
								if (valueType.IsValueType && ts == Type.EmptyTypes)
								{
									if (!valueType.Equals(typeof(void)))
									{
										if (string.IsNullOrEmpty(valNode.InnerText))
										{
											_value = VPLUtil.GetDefaultValue(valueType);
										}
										else
										{
											if (typeof(EnumHideDialogButtons).Equals(valueType) && (string.CompareOrdinal(valNode.InnerText, "False") == 0 || string.CompareOrdinal(valNode.InnerText, "True") == 0))
											{
												if (string.CompareOrdinal(valNode.InnerText, "False") == 0)
												{
													_value = EnumHideDialogButtons.ShowOKCancel;
												}
												else
												{
													_value = EnumHideDialogButtons.HideOKCancel;
												}
											}
											else
											{
												try
												{
													TypeConverter converter = TypeDescriptor.GetConverter(valueType);
													_value = converter.ConvertFromInvariantString(valNode.InnerText);
												}
												catch (Exception er0)
												{
													throw new DesignerException(er0, "Error converting [{0}] to type [{1}]. message:{2}", valNode.InnerText, valueType, er0.Message);
												}
											}
										}
									}
								}
								else
								{
									if (XmlUtil.GetAttributeBoolDefFalse(valNode, XmlTags.XMLATT_IsNull))
									{
										_nullPointer = new NullObjectPointer(this);
									}
									else
									{
										if (typeof(object).Equals(valueType))
										{
											XmlNode nodeV = valNode.SelectSingleNode(VALUE_Value);
											if (nodeV != null)
											{
												_value = reader.ReadObject(node, null);
											}
											else
											{
												_value = valNode.InnerText;
											}
										}
										else
										{
											ConstructorInfo cif = valueType.GetConstructor(ts);
											if (cif == null)
											{
												//try fix it witht he parameter names
												ConstructorInfo[] cifs = valueType.GetConstructors();
												if (cifs != null && cifs.Length > 0)
												{
													for (int i = 0; i < cifs.Length; i++)
													{
														ParameterInfo[] pifs = cifs[i].GetParameters();
														if (pifs != null && pifs.Length == pnames.Length)
														{
															bool matched = true;
															for (int k = 0; k < pifs.Length; k++)
															{
																if (!string.IsNullOrEmpty(pnames[k]))
																{
																	if (string.CompareOrdinal(pifs[k].Name, pnames[k]) != 0)
																	{
																		matched = false;
																		break;
																	}
																}
																else
																{
																	if (!pifs[k].ParameterType.Equals(ts[k]))
																	{
																		matched = false;
																		break;
																	}
																}
															}
															if (matched)
															{
																cif = cifs[i];
																break;
															}
														}
													}
												}
											}
											if (cif == null)
											{
												reader.addErrStr2("Constructor not found for [{0}] (", valueType.Name);
												for (int i = 0; i < ts.Length; i++)
												{
													reader.addErrStr2("param {0}:{1}", i, ts[i].Name);
												}
												reader.addErrStr2(")");
											}
											else
											{
												_constructorToUse = new ConstructorInfoClass(cif);
												if (_constructorToUse.ParameterCount == 0)
												{
													Type tx = XmlUtil.GetLibTypeAttribute(valNode);
													if (typeof(AttributeConstructor).Equals(tx))
													{
														_value = reader.ReadObject(valNode, this);
													}
													else
													{
														XmlNode nodeV = valNode.SelectSingleNode(VALUE_Value);
														if (nodeV != null)
														{
															_value = reader.ReadObject(nodeV, this);
														}
														else
														{
															_value = valNode.InnerText;
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
					else //class pointer
					{
						if (XmlUtil.GetAttributeBoolDefFalse(valNode, XmlTags.XMLATT_IsNull))
						{
							_nullPointer = new NullObjectPointer(this);
						}
						else
						{
							DataTypePointer[] ts;
							XmlNodeList nList = valNode.SelectNodes(XmlTags.XML_Item);
							if (nList == null || nList.Count == 0)
							{
								ts = new DataTypePointer[] { };
							}
							else
							{
								ts = new DataTypePointer[nList.Count];
								for (int i = 0; i < nList.Count; i++)
								{
									string name = XmlUtil.GetNameAttribute(nList[i]);
									object v = reader.ReadObject(nList[i], this);
									if (v != null)
									{
										SetParameterVlaue(name, v);
										DataTypePointer dp = v as DataTypePointer;
										if (dp != null)
										{
											ts[i] = dp;
										}
										else
										{
											//v is a primary/string
											ts[i] = new DataTypePointer(new TypePointer(v.GetType(), this));
										}
									}
									else
									{
										ts[i] = new DataTypePointer(new TypePointer(XmlUtil.GetLibTypeAttribute(nList[i]), this));
									}
								}
							}
							_customConstructors = this.ClassTypePointer.GetConstructors();
							foreach (ConstructorClass cc in _customConstructors)
							{
								if (cc.ParametersMatch(ts))
								{
									_customConstructorToUse = cc;
									break;
								}
							}
							if (_customConstructorToUse == null)
							{
								reader.addErrStr2("Constructor not found for [{0}] (", this.ClassTypePointer.Name);
								for (int i = 0; i < ts.Length; i++)
								{
									reader.addErrStr2("param {0}:{1}", i, ts[i].TypeName);
								}
								reader.addErrStr2(")");
								_nullPointer = new NullObjectPointer(this);
							}
						}
					}
				}
			}
			else
			{
				_constructorToUse = null;
				_customConstructorToUse = null;
				_nullPointer = new NullObjectPointer(this);
			}
			if (_ownerParameter != null)
			{
				Type itemType = null;
				if (_value != null)
				{
					itemType = _value.GetType().GetElementType();
				}
				if (itemType != null)
				{
					XmlNode arrayNode = node.SelectSingleNode(XML_ARRAY);
					if (arrayNode != null)
					{
						XmlNodeList itemNodeList = arrayNode.SelectNodes(XmlTags.XML_Item);
						_arrayItems = new ParameterValueArrayItem[itemNodeList.Count];
						for (int i = 0; i < itemNodeList.Count; i++)
						{
							_arrayItems[i] = new ParameterValueArrayItem(_ownerParameter.ActionContext, this, i, itemType);
							_arrayItems[i].OnReadFromXmlNode(reader, itemNodeList[i]);
						}
						SetArraySize(_arrayItems.Length);
					}
				}
			}
		}

		#endregion
		#region Compile
		public static CodeExpression GetTypeRefCode(object v)
		{
			if (v == null || v == null || v is NullObjectPointer)
			{
				return new CodeCastExpression(typeof(Type), new CodePrimitiveExpression(null));
			}
			Type t = v as Type;
			if (t != null)
			{
				return new CodeTypeOfExpression(t);
			}
			ConstObjectPointer cop = v as ConstObjectPointer;
			if (cop != null)
			{
				return cop.GetTypeRefCode();
			}
			DataTypePointer dp = v as DataTypePointer;
			if (dp != null)
			{
				if (dp.IsLibType)
				{
					return new CodeTypeOfExpression(dp.BaseClassType);
				}
				else
				{
					return new CodeTypeOfExpression(dp.ClassTypePointer.TypeString);
				}
			}
			throw new DesignerException("Invalid Type value:{0}", v);
		}
		/// <summary>
		/// the value is an instance of Type
		/// </summary>
		/// <returns></returns>
		public CodeExpression GetTypeRefCode()
		{
			return GetTypeRefCode(_value);
		}
		public CodeExpression GetParameterCode(IMethodCompile method, CodeStatementCollection statements, int idx)
		{
			if (IsLibType)
			{
				if (_constructorToUse != null)
				{
					ParameterInfo pif = _constructorToUse.GetParameter(idx);
					object v = GetParameterVlaue(pif.Name);
					if (typeof(Type).Equals(pif.ParameterType))
					{
						return GetTypeRefCode(v);
					}
					if (v == null)
					{
						return ObjectCreationCodeGen.GetDefaultValueExpression(pif.ParameterType);
					}
					ConstObjectPointer cop = v as ConstObjectPointer;
					if (cop != null)
					{
						return cop.GetReferenceCode(method, statements, true);
					}
					else
					{
						IObjectPointer pv = v as IObjectPointer;
						if (pv != null)
						{
							CodeExpression ce = pv.GetReferenceCode(method, statements, true);
							if (ce != null)
							{
								if (!VPLUtil.IsSameType(pv.ObjectType, pif.ParameterType))
								{
									if (!pif.ParameterType.IsAssignableFrom(pv.ObjectType))
									{
										ce = CompilerUtil.GetTypeConversion(new DataTypePointer(pif.ParameterType), ce, new DataTypePointer(pv.ObjectType), statements);
									}
								}
							}
							return ce;
						}
						else
						{
							return ObjectCreationCodeGen.ObjectCreationCode(v);
						}
					}
				}
			}
			else
			{
				if (_customConstructorToUse != null)
				{
					ParameterClass pif = _customConstructorToUse.Parameters[idx];
					object v = GetParameterVlaue(pif.Name);
					if (pif.IsLibType && typeof(Type).Equals(pif.BaseClassType))
					{
						return GetTypeRefCode(v);
					}
					if (v == null)
					{
						if (pif.IsLibType)
						{
							return ObjectCreationCodeGen.GetDefaultValueExpression(pif.BaseClassType);
						}
						else
						{
							return new CodePrimitiveExpression(null);
						}
					}
					ConstObjectPointer cop = v as ConstObjectPointer;
					if (cop != null)
					{
						return cop.GetReferenceCode(method, statements, true);
					}
					else
					{
						IObjectPointer pv = v as IObjectPointer;
						if (pv != null)
						{
							return pv.GetReferenceCode(method, statements, true);
						}
						else
						{
							return ObjectCreationCodeGen.ObjectCreationCode(v);
						}
					}
				}
			}
			return new CodePrimitiveExpression(null);
		}
		public override string CreateJavaScript(StringCollection sb)
		{
			if (this.IsArray)
			{
				if (_arrayItems == null)
				{
					return "null";
				}
				else
				{
					StringBuilder ret = new StringBuilder();
					ret.Append("[");
					if (_arrayItems.Length > 0)
					{
						ret.Append(_arrayItems[0].CreateJavaScript(sb));
						for (int i = 1; i < _arrayItems.Length; i++)
						{
							ret.Append(",");
							ret.Append(_arrayItems[i].CreateJavaScript(sb));
						}
					}
					ret.Append("]");
					return ret.ToString();
				}
			}
			if (_nullPointer != null)
			{
				if (typeof(string).Equals(_targetType))
					return "''";
				if (typeof(string).Equals(_nullPointer.BaseClassType))
					return "''";
				if (typeof(JsString).Equals(_targetType))
					return "''";
				if (typeof(JsString).Equals(_nullPointer.BaseClassType))
					return "''";
				if (typeof(Color).Equals(_targetType))
					return "''";
				if (typeof(Color).Equals(_nullPointer.BaseClassType))
					return "''";
				return "null";
			}
			if (Value != null)
			{
				string sf = _value as string;
				if (!string.IsNullOrEmpty(sf) && !string.IsNullOrEmpty(_name))
				{
					if (_ownerParameter != null && _ownerParameter.ActionContext != null && _ownerParameter.ActionContext.ExecutionMethod != null)
					{
						if (_ownerParameter.ActionContext.ExecutionMethod.IsParameterFilePath(_name))
						{
							string webFile = _ownerParameter.ActionContext.ExecutionMethod.CreateWebFileAddress(sf, _name);
							return string.Format(CultureInfo.InvariantCulture, "'{0}'", webFile);
						}
					}
				}
				return ObjectCreationCodeGen.ObjectCreateJavaScriptCode(_value);
			}
			return "null";
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return CreateJavaScript(code);
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			return CreatePhpScript(code);
		}
		public override string CreatePhpScript(StringCollection sb)
		{
			if (this.IsArray)
			{
				if (_arrayItems == null)
				{
					return "null";
				}
				else
				{
					StringBuilder ret = new StringBuilder();
					ret.Append("array(");
					if (_arrayItems.Length > 0)
					{
						ret.Append(_arrayItems[0].CreatePhpScript(sb));
						for (int i = 1; i < _arrayItems.Length; i++)
						{
							ret.Append(",");
							ret.Append(_arrayItems[i].CreatePhpScript(sb));
						}
					}
					ret.Append(")");
					return ret.ToString();
				}
			}
			if (_nullPointer != null)
			{
				if (typeof(string).Equals(_targetType))
					return "''";
				if (typeof(string).Equals(_nullPointer.BaseClassType))
					return "''";
				return "null";
			}

			if (_values != null && _values.Count > 0)
			{
				if (_constructorToUse != null && _constructorToUse.ParameterCount > 0)
				{
					if (_constructorToUse.ParameterCount == _values.Count)
					{
						object[] ps = new object[_values.Count];
						_values.Values.CopyTo(ps, 0);
						for (int i = 0; i < ps.Length; i++)
						{
							ParameterValue pv = ps[i] as ParameterValue;
							if (pv != null)
							{
								ps[i] = pv.CreatePhpScript(sb);
							}
						}
						_value = _constructorToUse.Constructor.Invoke(ps);
					}
				}
			}
			if (_value != null)
			{
				return ObjectCreationCodeGen.ObjectCreatePhpScriptCode(_value);
			}
			return "''";
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (IsLibType)
			{
				if (this.ValueType.IsArray)
				{
					Type itemType = this.ValueType.GetElementType();
					if (_value == null || !_value.GetType().Equals(ValueType))
					{
						CodeArrayCreateExpression cae = new CodeArrayCreateExpression(this.ValueType, new CodePrimitiveExpression(0));
						return cae;
					}
					Array a = _arrayItems;
					if (a == null)
					{
						a = _value as Array;
					}
					if (a != null)
					{
						if (a.Length == 0)
						{
							CodeArrayCreateExpression cae = new CodeArrayCreateExpression(this.ValueType, new CodePrimitiveExpression(0));
							return cae;
						}
						else
						{
							CodeExpression[] ces = new CodeExpression[a.Length];
							for (int i = 0; i < a.Length; i++)
							{
								object v = a.GetValue(i);
								ParameterValue pv = v as ParameterValue;
								if (pv != null)
								{
									ces[i] = pv.GetReferenceCode(method, statements, true);
								}
								else
								{
									ces[i] = ObjectCreationCodeGen.ObjectCreationCode(v);
								}
							}
							CodeArrayCreateExpression cae = new CodeArrayCreateExpression(this.ValueType, ces);
							return cae;
						}
					}
					return ObjectCreationCodeGen.ObjectCreationCode(_value);
				}
			}
			if (_nullPointer != null)
			{
				if (IsLibType)
				{
					return ObjectCreationCodeGen.GetDefaultValueExpression(ValueType);
				}
				else
				{
					return new CodeCastExpression(ClassTypePointer.TypeString, new CodePrimitiveExpression(null));
				}
			}
			if (IsLibType)
			{
				if (typeof(Type).Equals(ValueType))
				{
					return GetTypeRefCode();
				}
				if (this.ValueType.IsPrimitive || typeof(string).Equals(this.ValueType) || VPLUtil.UseTypeEditor(ValueType) || _constructorToUse == null)
				{
					if (_value == null || !ValueType.IsAssignableFrom(_value.GetType()))
					{
						_value = VPLUtil.GetDefaultValue(ValueType);
					}
					return ObjectCreationCodeGen.ObjectCreationCode(_value);
				}
				CodeExpression[] ps = new CodeExpression[_constructorToUse.ParameterCount];
				if (_constructorToUse.ParameterCount > 0)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						ps[i] = this.GetParameterCode(method, statements, i);
					}
				}
				CodeObjectCreateExpression oce = new CodeObjectCreateExpression(
					new CodeTypeReference(this.ValueType), ps);
				return oce;
			}
			else
			{
				if (_customConstructorToUse != null)
				{
					CodeExpression[] ps = new CodeExpression[_customConstructorToUse.ParameterCount];
					if (_customConstructorToUse.ParameterCount > 0)
					{
						for (int i = 0; i < ps.Length; i++)
						{
							ps[i] = this.GetParameterCode(method, statements, i);
						}
					}
					CodeObjectCreateExpression oce = new CodeObjectCreateExpression(
						new CodeTypeReference(this.ClassTypePointer.TypeString), ps);
					return oce;
				}
			}
			if (IsLibType)
			{
				return new CodeCastExpression(ValueType, new CodePrimitiveExpression(null));
			}
			else
			{
				return new CodeCastExpression(ClassTypePointer.TypeString, new CodePrimitiveExpression(null));
			}
		}
		public CodeExpression[] GetConstructorParameters(IMethodCompile method, CodeStatementCollection statements)
		{
			CodeExpression[] ps = null;
			if (_customConstructorToUse != null)
			{
				ps = new CodeExpression[_customConstructorToUse.ParameterCount];
				if (_customConstructorToUse.ParameterCount > 0)
				{
					for (int i = 0; i < ps.Length; i++)
					{
						ps[i] = this.GetParameterCode(method, statements, i);
					}
				}
			}
			return ps;
		}
		#endregion
		#region IClonable
		public override object Clone()
		{
			ConstObjectPointer obj = (ConstObjectPointer)base.Clone();
			obj._id = _id;
			obj._name = _name;
			obj._value = _value;
			obj._nullPointer = _nullPointer;
			obj._constructors = _constructors;
			obj._constructorToUse = _constructorToUse;
			obj._customConstructors = _customConstructors;
			obj._customConstructorToUse = _customConstructorToUse;
			obj._values = _values;
			obj._propertyGrid = _propertyGrid;
			obj.NotSaveType = NotSaveType;
			return obj;
		}
		#endregion
		#region IBeforeSerializeNotify Members
		private XmlObjectReader _reader;
		private XmlObjectWriter _writer;
		private XmlNode _xmlData;
		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			_reader = reader;
			_xmlData = node;
		}

		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			_writer = writer;
			_xmlData = node;
		}

		public void ReloadFromXmlNode()
		{
			if (_reader != null && _xmlData != null)
			{
				_reader.ReadObjectFromXmlNode(_xmlData, this, this.GetType(), null);
			}
		}

		public void UpdateXmlNode(XmlObjectWriter writer)
		{
			if (_xmlData != null)
			{
				if (writer == null)
				{
					writer = _writer;
				}
				if (writer != null)
				{
					writer.WriteObjectToNode(_xmlData, this);
				}
			}
		}

		public XmlNode XmlData
		{
			get { return _xmlData; }
		}

		#endregion
	}
	#region class TypeSelectorNewArrayItem
	class TypeSelectorArraySize : UITypeEditor
	{
		public TypeSelectorArraySize()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.None;
		}
	}
	#endregion
	#region class PropertyDescriptorNewArrayItem
	class PropertyDescriptorArraySize : PropertyDescriptor
	{
		private ConstObjectPointer _owner;
		public PropertyDescriptorArraySize(ConstObjectPointer owner)
			: base("Size", new Attribute[]{
                new ParenthesizePropertyNameAttribute(true)
                ,new RefreshPropertiesAttribute(RefreshProperties.All)
                ,new EditorAttribute(typeof(TypeSelectorArraySize),typeof(UITypeEditor))
            })
		{
			_owner = owner;
		}
		public ConstObjectPointer Owner { get { return _owner; } }

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _owner.GetType(); }
		}

		public override object GetValue(object component)
		{
			return _owner.GetArraySize();
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return typeof(int); }
		}

		public override void ResetValue(object component)
		{

		}

		public override void SetValue(object component, object value)
		{
			try
			{
				int n = Convert.ToInt32(value, CultureInfo.InvariantCulture);
				_owner.SetArraySize(n);
			}
			catch
			{
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
	#endregion
	#region class PropertyDescriptorArrayItem
	class PropertyDescriptorArrayItem : PropertyDescriptor
	{
		private ConstObjectPointer _owner; //represents the array
		private Type _itemType;
		private object _initValue;
		private int _idx;
		public PropertyDescriptorArrayItem(ConstObjectPointer owner, int index, Type itemType, object value)
			: base(string.Format(CultureInfo.InvariantCulture, "[{0}]", index), new Attribute[]{
                new EditorAttribute(typeof(SelectorEnumValueType), typeof(UITypeEditor))
            })
		{
			_owner = owner;
			_idx = index;
			_itemType = itemType;
			_initValue = value;
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get { return _owner.GetType(); }
		}

		public override object GetValue(object component)
		{
			return _owner.GetArrayItem(_idx);
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(ParameterValueArrayItem);
			}
		}

		public override void ResetValue(object component)
		{
			_owner.SetArrayItem(_idx, _initValue);
			_owner.OnParameterChanged();
		}

		public override void SetValue(object component, object value)
		{
			_owner.SetArrayItem(_idx, value);
			_owner.OnParameterChanged();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
	#endregion
	/// <summary>
	/// for array item selection
	/// </summary>
	public class ParameterValueArrayItem : ParameterValue
	{
		private ConstObjectPointer _owner; //represents the array
		private int _index;
		public ParameterValueArrayItem(IActionContext act, ConstObjectPointer owner, int index, Type itemType)
			: base(act)
		{
			_owner = owner;
			_index = index;
			Name = string.Format(CultureInfo.InvariantCulture, "[{0}]", index);
			SetDataType(itemType);
		}
		protected override void OnRead(IXmlCodeReader reader, XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode(XML_Value);
			if (nd != null)
			{
				if (ValueType == EnumValueType.ConstantValue)
				{
					ConstantValue.OnReadFromXmlNode(reader, nd);
				}
				else if (ValueType == EnumValueType.MathExpression)
				{
					MathExpression.OnReadFromXmlNode(reader, nd);
				}
				else if (ValueType == EnumValueType.Property)
				{
					if (!XmlUtil.GetAttributeBoolDefFalse(nd, XmlTags.XMLATT_IsNull))
					{
						Property = reader.ReadObject(nd, this) as IObjectPointer;
					}
				}
			}
		}
	}
}
