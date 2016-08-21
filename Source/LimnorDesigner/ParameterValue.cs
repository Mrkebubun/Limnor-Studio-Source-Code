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
using XmlSerializer;
using System.ComponentModel;
using MathExp;
using ProgElements;
using System.Drawing;
using System.CodeDom;
using System.Xml;
using System.Drawing.Design;
using LimnorDesigner.Property;
using System.Windows.Forms;
using System.Reflection;
using VPL;
using System.Windows.Forms.Design;
using XmlUtility;
using LimnorDesigner.Action;
using VSPrj;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Event;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using System.Globalization;
using System.Collections;

namespace LimnorDesigner
{
	public enum EnumValueType { ConstantValue = 0, Property, MathExpression }
	/// <summary>
	/// generic value; can be a constant, a math exp or a property. used as action parameters.
	/// it cannot be an array. 
	/// mandatory fields: 
	///     _method - action method provided in constructor
	///     _name - parameter name
	///     _id - parameter ID
	///     _value - defines the data type. must be set after _id and _name are set
	/// it must be used in a context of IAction or ActionBranch    
	/// </summary>
	[Editor(typeof(SelectorEnumValueType), typeof(UITypeEditor))]
	[UseParentObject]
	[TypeConverter(typeof(TypeConverterParameterValue))]
	public class ParameterValue : IObjectPointer, ICustomTypeDescriptor, IDataScope, ISerializeNotify, IParameter, IMethodElement, IFromString, IXmlNodeSerializable, ICompileableItem, IXmlSerializeItem, ISerializeParent, IParameterValue, IWithProject, IValueEnumProvider, ISourceValuePointersHolder, IGenericTypePointer
	{
		#region fields and constructors
		//properties to be displayed in PropertyGrid
		enum EnumProp { DataType, DataValueType, ConstantValue, DataPointer, MathExpression }
		//
		//three types of data, use _valueDataType to indicate which one is used
		private EnumValueType _valueDataType = EnumValueType.ConstantValue; //use enum instead of sub classes because user needs to choose the enum type
		private ConstObjectPointer _value; //constant value; also defines the data type
		private IObjectPointer _valuePointer; //property, method/event parameter
		private IMathExpression _mathExpression; //math expression to produce the value 
		//
		private MathNodeRoot _mapOwner; //this instance is an item in variable mapping.
		private DataTypePointer _compiledDataType;
		//
		//for determing the parameter data type
		private IActionContext _actionContext;
		private IActionContext _cloneActionContext;
		private ConstObjectPointer _parentObject; //this parameter is for a contructor of this object
		//
		private IMethod _scopeMethod; //MethodClass using the action, defines variable scope
		//
		private bool _readingProperties;
		private UInt32 _id;
		private string _name;
		private EventHandler PropertyChanged;
		private Bitmap _dataicon;
		private List<Attribute> _constValueAttributes;
		public ParameterValue(IActionContext act)
		{
			_actionContext = act;
		}
		public ParameterValue(ConstObjectPointer obj)
		{
			_parentObject = obj;
			_actionContext = obj.OwnerParameter.ActionContext;
		}
		#endregion
		#region Serializable properties
		[Description("A value can be expressed in one of the following 3 ways: 1. a constant value (ConstantValue); 2. a value of a property of an object (Property); 3. a value calculated by a math expression involcing properties and constants (MathExpression)")]
		public EnumValueType ValueType
		{
			get
			{
				return _valueDataType;
			}
			set
			{
				if (_valueDataType != value)
				{
					_valueDataType = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		[Description("If ValueType is ConstantValue then this property is the constant value to be used")]
		public ConstObjectPointer ConstantValue
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		[Description("The data type of this parameter")]
		[IgnoreReadOnly]
		public DataTypePointer DataType
		{
			get
			{
				return _value;
			}
			set
			{
				if (value != null && !typeof(void).Equals(value.BaseClassType))
				{
					if (_value == null)
					{
						_value = new ConstObjectPointer();
					}
					_value.SetDataType(value);
					_value.SetOwnerParameter(this);
					PropertyPointer pp = _valuePointer as PropertyPointer;
					if (pp != null)
					{
						pp.Scope = _value.BaseClassType;
					}
				}
			}
		}
		[IgnoreReadOnly()]
		[Editor(typeof(PropEditorPropertyPointer), typeof(UITypeEditor))]
		[Description("If ValueType is Property then the value to be used is a property of an object pointed to by this property")]
		public IObjectPointer Property
		{
			get
			{
				PropertyPointer pp = _valuePointer as PropertyPointer;
				if (pp != null)
				{
					pp.Scope = _value.BaseClassType;
				}
				return _valuePointer;
			}
			set
			{
				_valuePointer = value;
				PropertyPointer pp = _valuePointer as PropertyPointer;
				if (pp != null)
				{
					pp.Scope = _value.BaseClassType;
				}
				if (PropertyChanged != null)
				{
					PropertyChanged(this, EventArgs.Empty);
				}
			}
		}
		[TypeConverter(typeof(TypeConverterNoExpand))]
		[IgnoreReadOnlyAttribute]
		[ReadOnly(true)]
		[Editor(typeof(UITypeEditorMathExpression2), typeof(UITypeEditor))]
		[Description("If ValueType is MathExpression then the value to be used is a calculation a math expression defined by this property")]
		public IMathExpression MathExpression
		{
			get
			{
				if (_mathExpression == null)
				{
					MathNodeRoot r = new MathNodeRoot();
					r.Name = Name;
					r.VariableMapTargetType = typeof(ParameterValue);
					r.ActionContext = _actionContext;
					r.ScopeMethod = ScopeMethod;
					((MathNodeVariable)r[0]).VariableType = new MathExp.RaisTypes.RaisDataType(_value.BaseClassType);
					_mathExpression = r;
				}
				return _mathExpression;
			}
			set
			{
				_mathExpression = value;
				if (_mathExpression == null)
				{
					MathNodeRoot r = new MathNodeRoot();
					r.Name = Name;
					r.VariableMapTargetType = typeof(ParameterValue);
					r.ActionContext = _actionContext;
					r.ScopeMethod = ScopeMethod;
					((MathNodeVariable)r[0]).VariableType = new MathExp.RaisTypes.RaisDataType(_value.BaseClassType);
					_mathExpression = r;
				}
				if (PropertyChanged != null)
				{
					PropertyChanged(this, EventArgs.Empty);
				}
			}
		}
		#endregion
		#region properties
		[Browsable(false)]
		public DataTypePointer CompileddataType
		{
			get
			{
				return _compiledDataType;
			}
		}
		[Browsable(false)]
		public IActionContext ActionContext
		{
			get
			{
				return _actionContext;
			}
		}
		private Font _font;
		[Browsable(false)]
		[ReadOnly(true)]
		public Font DrawingFont
		{
			get
			{
				if (_font == null)
				{
					_font = new Font("Times New Roman", 12);
				}
				return _font;
			}
			set
			{
				_font = value;
			}
		}
		private Brush _brush = Brushes.Black;
		[Browsable(false)]
		[ReadOnly(true)]
		public Brush DrawingBrush
		{
			get
			{
				if (_brush == null)
				{
					_brush = Brushes.Black;
				}
				return _brush;
			}
			set
			{
				_brush = value;
			}
		}
		[Browsable(false)]
		public IMethod ScopeMethod
		{
			get
			{
				if (_scopeMethod == null)
				{
					IAction ac = _actionContext as IAction;
					if (ac != null)
					{
						_scopeMethod = ac.ScopeMethod;
						if (_scopeMethod == null)
						{
							_scopeMethod = ac.ActionHolder as IMethod;
						}
					}
				}
				return _scopeMethod;
			}
			set
			{
				_scopeMethod = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public string CodeTypeName { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public string CodeName { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public Bitmap DataIcon
		{
			get
			{
				return _dataicon;
			}
			set
			{
				_dataicon = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_value != null)
				{
					return _value.DataTypeEx;
				}
				if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
					{
						return _valuePointer.ObjectType;
					}
				}
				if (_mathExpression != null)
				{
					return _mathExpression.DataType.Type;
				}
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					return _value;
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.ObjectInstance;
					return null;
				}
				else
				{
					if (_mathExpression != null)
					{
						List<IPropertyPointer> pointers = new List<IPropertyPointer>();
						MethodInfo mif = _mathExpression.GetCalculationMethod(this, pointers);
						if (mif != null)
						{
							object[] vs = new object[pointers.Count];
							for (int i = 0; i < pointers.Count; i++)
							{
								vs[i] = pointers[i].ObjectInstance;
							}
							return mif.Invoke(null, vs);
						}
					}
				}
				return null;
			}
			set
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{

					_value = value as ConstObjectPointer;
					_value.Name = Name;
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						_valuePointer.ObjectInstance = value;
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod OwnerMethod
		{
			get
			{
				if (_actionContext != null)
				{
					return _actionContext.ExecutionMethod;
				}
				return null;
			}
		}
		#endregion
		#region Methods
		public static Type GetValueSelector()
		{
			return typeof(SelectorEnumValueType);
		}
		public object GetConstValue()
		{
			if (_value == null)
			{
				return null;
			}
			return _value.Value;
		}
		public void SetConstValueAndType(ConstObjectPointer value)
		{
			_value = value;
		}
		public void SetConstValue(object value)
		{
			if (_value != null)
			{
				_value.Value = value;
			}
		}

		public void SetEditor(EditorAttribute editor)
		{
			if (_constValueAttributes == null)
			{
				_constValueAttributes = new List<Attribute>();
			}
			for (int i = 0; i < _constValueAttributes.Count; i++)
			{
				if (_constValueAttributes[i] is EditorAttribute)
				{
					_constValueAttributes[i] = editor;
					return;
				}
			}
			_constValueAttributes.Add(editor);
		}
		public void MergeValueAttributes(Attribute[] attrs)
		{
			if (attrs == null) return;
			if (attrs.Length == 0) return;
			for (int i = 0; i < attrs.Length; i++)
			{
				MergeValueAttribute(attrs[i]);
			}
		}
		public void MergeValueAttribute(Attribute a)
		{
			if (a == null) return;
			if (_constValueAttributes == null)
			{
				_constValueAttributes = new List<Attribute>();
			}
			Type t = a.GetType();
			for (int i = 0; i < _constValueAttributes.Count; i++)
			{
				if (t.IsAssignableFrom(_constValueAttributes[i].GetType()))
				{
					return;
				}
			}
			_constValueAttributes.Add(a);
		}
		public void SetValueAttribute(Attribute a)
		{
			if (a == null) return;
			if (_constValueAttributes == null)
			{
				_constValueAttributes = new List<Attribute>();
			}
			Type t = a.GetType();
			for (int i = 0; i < _constValueAttributes.Count; i++)
			{
				if (t.IsAssignableFrom(_constValueAttributes[i].GetType()))
				{
					_constValueAttributes[i] = a;
					return;
				}
			}
			_constValueAttributes.Add(a);
		}
		public bool IsAssignableFrom(DataTypePointer type)
		{
			if (type == null)
			{
				return false;
			}
			if (_value != null)
			{
				if (_value.IsLibType || type.IsLibType)
				{
					return _value.BaseClassType.IsAssignableFrom(type.BaseClassType);
				}
				else
				{
					if (type.IsSameObjectRef(_value))
					{
						return true;
					}
					else
					{
						//type is derived from _value?
						return (type.VariableCustomType.GetBaseClass(_value.ClassId) != null);
					}
				}
			}
			return true;
		}
		public bool IsUsingObject(IObjectPointer p)
		{
			if (p != null)
			{
				if (_valueDataType == EnumValueType.MathExpression)
				{
					if (_mathExpression != null)
					{
						MathNodeRoot root = _mathExpression as MathNodeRoot;
						if (root != null)
						{
							List<IObjectPointer> objs = new List<IObjectPointer>();
							root.FindItemByType<IObjectPointer>(objs);
							foreach (IObjectPointer o in objs)
							{
								if (p.IsSameObjectRef(o))
								{
									return true;
								}
							}
						}
					}
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (p.IsSameObjectRef(_valuePointer))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void Draw(Graphics g)
		{
			string s;
			switch (_valueDataType)
			{
				case EnumValueType.ConstantValue:
					if (_value == null)
						s = "null";
					else
						s = _value.ToString();
					g.DrawString(s, DrawingFont, DrawingBrush, 1, 1);
					break;
				case EnumValueType.MathExpression:
					MathNodeRoot mr = _mathExpression as MathNodeRoot;
					if (mr == null)
					{
						s = "null";
						g.DrawString(s, DrawingFont, DrawingBrush, 1, 1);
					}
					else
					{
						mr.SetFont(DrawingFont);
						mr.SetBrush(DrawingBrush);
						mr.Draw(g);
					}
					break;
				case EnumValueType.Property:
					if (_valuePointer == null)
					{
						s = "null";
					}
					else
					{
						s = _valuePointer.DisplayName;
					}
					g.DrawString(s, DrawingFont, DrawingBrush, 1, 1);
					break;
			}
		}
		public SizeF CalculateDrawSize(Graphics g)
		{
			SizeF size;
			string s;
			switch (_valueDataType)
			{
				case EnumValueType.ConstantValue:
					if (_value == null)
						s = "null";
					else
						s = _value.DisplayName;
					size = g.MeasureString(s, DrawingFont);
					break;
				case EnumValueType.MathExpression:
					MathNodeRoot mr = _mathExpression as MathNodeRoot;
					if (mr == null)
					{
						s = "null";
						size = g.MeasureString(s, DrawingFont);
					}
					else
					{
						mr.SetFont(DrawingFont);
						size = mr.CalculateDrawSize(g);
					}
					break;
				case EnumValueType.Property:
					if (_valuePointer == null)
					{
						s = "null";
					}
					else
					{
						s = _valuePointer.DisplayName;
					}
					size = g.MeasureString(s, DrawingFont);
					break;
				default:
					size = new SizeF(3, 3);
					break;
			}
			return size;
		}
		public void SetConstructorTypeScope(Type t)
		{
			if (_value == null)
			{
				_value = new ConstObjectPointer(Name, t);
				_value.Name = Name;
			}
			_value.SetOwnerParameter(this);
			_value.SetTypeScope(t);
		}
		public void SetDataType(DataTypePointer tp)
		{
			if (_value == null)
			{
				_value = new ConstObjectPointer(tp);
				_value.Name = Name;
			}
			else
			{
				_value.SetDataType(tp);
			}
			_value.SetOwnerParameter(this);
		}
		public void SetDataType(Type tp)
		{
			if (_value == null)
			{
				_value = new ConstObjectPointer(new TypePointer(tp));
				_value.Name = Name;
			}
			else
			{
				_value.SetDataType(tp);
			}
			_value.SetOwnerParameter(this);
		}
		public void SetTargetType(Type t)
		{
			if (_value != null)
			{
				_value.SetTargetType(t);
			}
		}
		public Type GetTargetType()
		{
			if (_value != null)
			{
				return _value.TargetCompileType;
			}
			return null;
		}
		/// <summary>
		/// set type and do not change the value
		/// </summary>
		/// <param name="type"></param>
		public bool ForceDataType(DataTypePointer type)
		{
			if (_value == null)
			{
				_value = new ConstObjectPointer(type);
				_value.SetOwnerParameter(this);
				return true;
			}
			else
			{
				_value.SetOwnerParameter(this);
				return _value.ForceDataType(type);
			}
		}
		public void SetValue(object value)
		{
			if (value == this)
			{
				throw new DesignerException("Do not assign the parameter value to itself");
			}
			if (this.DataType != null && typeof(Type).Equals(this.DataType.BaseClassType))
			{
				_value.SetValue(ConstObjectPointer.VALUE_Type, value);
			}
			else
			{
				CustomMethodParameterPointer cmpp = value as CustomMethodParameterPointer;
				if (cmpp != null)
				{
					_valuePointer = cmpp;
					_valueDataType = EnumValueType.Property;
				}
				else
				{
					MathNodeRoot r = value as MathNodeRoot;
					if (r != null)
					{
						ValueType = EnumValueType.MathExpression;
						MathExpression = r;
					}
					else
					{
						IMathExpression ime = value as IMathExpression;
						if (ime != null)
						{
							ValueType = EnumValueType.MathExpression;
							MathExpression = ime;
						}
						else
						{
							ParameterValue pv = value as ParameterValue;
							if (pv != null)
							{
								CopyData(pv);
							}
							else
							{
								IProperty prop = value as IProperty;
								if (prop != null)
								{
									ValueType = EnumValueType.Property;
									_valuePointer = value as IObjectPointer;
								}
								else
								{
									//assume it is a constant
									ValueType = EnumValueType.ConstantValue;
									if (value == null)
									{
										_value.MakeNull();
									}
									else
									{
										ConstObjectPointer cop = value as ConstObjectPointer;
										if (cop != null)
										{
											_value = cop;
											_value.Name = Name;
										}
										else
										{
											_value.SetValue(ConstObjectPointer.VALUE_Value, value);
										}
									}
								}
							}
						}
					}
				}
			}
			FireValueChange();
		}
		/// <summary>
		/// copy contents without cloning
		/// </summary>
		/// <param name="v"></param>
		public void CopyData(ParameterValue v)
		{
			_valueDataType = v._valueDataType;
			if (v._valuePointer != null)
			{
				_valuePointer = (IObjectPointer)v._valuePointer.Clone();
			}
			if (v._mathExpression != null)
			{
				_mathExpression = (IMathExpression)v._mathExpression.Clone();
			}
			if (_mathExpression != null)
			{
				_mathExpression.ActionContext = _actionContext;
				_mathExpression.ScopeMethod = _scopeMethod;
			}
			if (v._value != null)
			{
				_value = (ConstObjectPointer)v._value.Clone();
			}
		}
		public void SetCustomMethod(IMethod m)
		{
			_scopeMethod = m;
			if (_mathExpression != null)
			{
				_mathExpression.ScopeMethod = _scopeMethod;
			}
		}
		public void FireValueChange()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, EventArgs.Empty);
			}
		}
		public IEventParameter AsEventParameter()
		{
			if (_valueDataType == EnumValueType.Property)
			{
				return _valuePointer as IEventParameter;
			}
			return null;
		}
		public void SetOwnerAction(IActionContext act)
		{
			_cloneActionContext = act;
		}
		public void SetParameterType(ParameterClass type)
		{
			if (_value == null)
			{
				_value = new ConstObjectPointer(type);
				_value.Name = Name;
			}
			else
			{
				_value.SetDataType(type);
			}
			_name = type.Name;
			_value.SetOwnerParameter(this);
		}
		public void SetParameterValueChangeEvent(EventHandler h)
		{
			PropertyChanged = h;
			if (_mathExpression != null)
			{
				_mathExpression.SetParameterValueChangeEvent(h);
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
		}
		public bool IsSameObjectRef(IObjectIdentity prop)
		{
			ParameterValue d = prop as ParameterValue;
			if (d != null)
			{
				if (this._valueDataType == d.ValueType)
				{
					if (this.ValueType == EnumValueType.ConstantValue)
					{
						return (this.ConstantValue == d.ConstantValue);
					}
					else if (this.ValueType == EnumValueType.Property)
					{
						if (_valuePointer != null)
						{
							return _valuePointer.IsSameObjectRef(d._valuePointer);
						}
						return (d._valuePointer == null);
					}
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
		public override string ToString()
		{
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_value != null)
				{
					return _value.ToString();
				}
				return "{null}";
			}
			return ExpressionDisplay;
		}
		#endregion
		#region ICloneable Members
		public virtual void Copy(ParameterValue d)
		{
			_id = d._id;
			_name = d._name;
			_dataicon = d._dataicon;
			_constValueAttributes = d._constValueAttributes;
			_scopeMethod = d._scopeMethod;
			PropertyChanged = d.PropertyChanged;

			if (d._value != null)
			{
				_value = (ConstObjectPointer)d._value.Clone();
			}
			else
			{
				_value.MakeNull();
			}
			if (d._valuePointer != null)
			{
				_valuePointer = (IObjectPointer)d._valuePointer.Clone();
			}
			else
			{
				_valuePointer = null;
			}
			if (d._mathExpression != null)
			{
				_mathExpression = (IMathExpression)d._mathExpression.Clone();
				_mathExpression.ActionContext = _actionContext;
				_mathExpression.ScopeMethod = d._scopeMethod;
			}
			else
			{
				_mathExpression = null;
			}
			if (_mapOwner != null)
			{
				_mapOwner = (MathNodeRoot)d._mapOwner.Clone();
			}
			_valueDataType = d._valueDataType;
		}
		public void SetCloneOwner(IActionContext o)
		{
			_cloneActionContext = o;
		}
		public ParameterValue Clone(IActionContext o)
		{
			ParameterValue pv = new ParameterValue(o);
			pv.Copy(this);
			return pv;
		}
		public virtual object Clone()
		{
			if (_cloneActionContext == null)
			{
				throw new DesignerException("Could not clone ParameterValue: _cloneActionContext not set");
			}
			ParameterValue obj = new ParameterValue(_cloneActionContext);
			_cloneActionContext = null;
			obj.Copy(this);
			return obj;
		}
		#endregion
		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					return EnumWebRunAt.Inherit;
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
					{
						return _valuePointer.RunAt;
					}
				}
				else
				{
					if (_mathExpression != null)
					{
						return _mathExpression.RunAt;
					}
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
				IObjectPointer root = this.Owner;
				if (root != null)
				{
					if (root.RootPointer != null)
					{
						return root.RootPointer;
					}
				}
				if (ActionContext != null)
				{
					if (ActionContext.OwnerContext != null)
					{
						ClassPointer cp = ActionContext.OwnerContext as ClassPointer;
						if (cp != null)
						{
							return cp;
						}
					}
				}
				return null;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return Name;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					return true;
				}
				if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.IsStatic;
					return true;
				}
				if (_mathExpression != null)
				{
					return _mathExpression.IsStatic;
				}
				return true;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug { get; set; }
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					if (_value != null)
					{
						return _value.GetType().AssemblyQualifiedName;
					}
					return typeof(object).AssemblyQualifiedName;
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.TypeString;
					return typeof(object).AssemblyQualifiedName;
				}
				else
				{
					if (_mathExpression != null)
					{
						return _mathExpression.DataType.TypeName;
					}
					return typeof(object).AssemblyQualifiedName;
				}
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					if (_value != null && _value.IsValid)
					{
						return true;
					}
					if (_value == null)
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(const: _value is null) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					}
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null && _valuePointer.IsValid)
					{
						return true;
					}
					if (_valuePointer == null)
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(property: _valuePointer is null) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					}
				}
				else
				{
					if (_mathExpression != null && _mathExpression.IsValid)
					{
						return true;
					}
					if (_mathExpression == null)
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(exp:_mathExpression is null) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					}
				}
				return false;
			}
		}
		public string CreateJavaScript(StringCollection sb)
		{
			string ret = string.Empty;
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_actionContext != null)
				{
					if (typeof(string).Equals(_value.BaseClassType))
					{
						SetterPointer sp = _actionContext.ExecutionMethod as SetterPointer;
						if (sp != null)
						{
							if (sp.SetProperty != null)
							{
								if (sp.SetProperty.Holder != null)
								{
									IWebClientPropertySetter ps = sp.SetProperty.Holder.ObjectInstance as IWebClientPropertySetter;
									if (ps != null)
									{
										ret = ps.ConvertSetPropertyActionValue(sp.SetProperty.Name, _value.Value as string);
										if (!string.IsNullOrEmpty(ret))
										{
											return ret;
										}
									}
								}
							}
						}
					}
				}
				_value.SetTargetType(this.ObjectType);
				ret = _value.CreateJavaScript(sb);
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					if (_valuePointer.IsValid)
					{
						try
						{
							ret = _valuePointer.GetJavaScriptReferenceCode(sb);
						}
						catch (Exception err)
						{
							if (VPLUtil.ErrorLogger != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue {0}. {1}", Name, DesignerException.FormExceptionText(err));
							}
							throw;
						}
					}
					else
					{
						ActionInput ai = _valuePointer as ActionInput;
						if (VPLUtil.ErrorLogger != null)
						{
							if (ai != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. ActionInput is not linked to a previous action");
							}
							else
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. Property is invalid. {0}", _valuePointer.GetType().FullName);
							}
						}
						if (ai != null)
						{
							throw new DesignerException("Invalid ParameterValue {0}. ActionInput is not linked to a previous action", Name);
						}
						else
						{
							throw new DesignerException("Invalid ParameterValue {0}. Property is invalid. {1}", Name, _valuePointer.GetType().FullName);
						}
					}
				}
			}
			else
			{
				if (_mathExpression != null)
				{
					ret = _mathExpression.CreateJavaScript(sb);
				}
			}
			return ret;
		}
		public string CreatePhpScript(StringCollection sb)
		{
			string ret = string.Empty;
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				_value.SetTargetType(this.ObjectType);
				ret = _value.CreatePhpScript(sb);
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					if (_valuePointer.IsValid)
					{
						try
						{
							ret = _valuePointer.GetPhpScriptReferenceCode(sb);
						}
						catch (Exception err)
						{
							if (VPLUtil.ErrorLogger != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue {0}. {1}", Name, DesignerException.FormExceptionText(err));
							}
							throw;
						}
					}
					else
					{
						ActionInput ai = _valuePointer as ActionInput;
						if (VPLUtil.ErrorLogger != null)
						{
							if (ai != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. ActionInput is not linked to a previous action");
							}
							else
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. Property is invalid. {0}", _valuePointer.GetType().FullName);
							}
						}
						if (ai != null)
						{
							throw new DesignerException("Invalid ParameterValue {0}. ActionInput is not linked to a previous action", Name);
						}
						else
						{
							throw new DesignerException("Invalid ParameterValue {0}. Property is invalid. {1}", Name, _valuePointer.GetType().FullName);
						}
					}
				}
			}
			else
			{
				if (_mathExpression != null)
				{
					ret = _mathExpression.CreatePhpScript(sb);
				}
			}
			return ret;
		}
		[Browsable(false)]
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeExpression ret = null;
			Type vType = null;
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (!string.IsNullOrEmpty(CodeTypeName) && !string.IsNullOrEmpty(CodeName))
				{
					ret = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(CodeTypeName), CodeName);
				}
				else
				{
					ret = _value.GetReferenceCode(method, statements, forValue);
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					if (_valuePointer.IsValid)
					{
						vType = _valuePointer.ObjectType;
						if (typeof(PropertyValue).Equals(vType))
						{
							if (_valuePointer.Owner != null)
							{
								IPropertyValueLinkHolder plh = _valuePointer.Owner.ObjectInstance as IPropertyValueLinkHolder;
								if (plh != null)
								{
									vType = plh.GetPropertyType(_valuePointer.CodeName);
								}
							}
						}
						try
						{
							if (this.DataType != null && this.DataType.BaseClassType != null && typeof(Delegate).IsAssignableFrom(this.DataType.BaseClassType))
							{
								if (_valuePointer.Owner != null)
								{
									CodeExpression target;
									target = _valuePointer.Owner.GetReferenceCode(method, statements, false);
									ret = new CodePropertyReferenceExpression(target, _valuePointer.CodeName);
								}
								else
								{
									ret = new CodeVariableReferenceExpression(_valuePointer.CodeName);
								}
								vType = this.DataType.BaseClassType;
							}
							else
							{
								ret = _valuePointer.GetReferenceCode(method, statements, true);
								IPropertyEx pp = _valuePointer as IPropertyEx;
								if (pp != null)
								{
									if (pp.CodeType != null)
									{
										vType = pp.CodeType;
									}
								}
							}
						}
						catch (Exception err)
						{
							if (VPLUtil.ErrorLogger != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue {0}. {1}", Name, DesignerException.FormExceptionText(err));
							}
							throw;
						}
					}
					else
					{
						ActionInput ai = _valuePointer as ActionInput;
						if (VPLUtil.ErrorLogger != null)
						{
							if (ai != null)
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. ActionInput is not linked to a previous action");
							}
							else
							{
								VPLUtil.ErrorLogger.LogError("Invalid ParameterValue. Property is invalid. {0}", _valuePointer.GetType().FullName);
							}
						}
						if (ai != null)
						{
							throw new DesignerException("Invalid ParameterValue {0}. ActionInput is not linked to a previous action", Name);
						}
						else
						{
							throw new DesignerException("Invalid ParameterValue {0}. Property is invalid. {1}", Name, _valuePointer.GetType().FullName);
						}
					}
				}
			}
			else
			{
				if (_mathExpression != null)
				{
					_mathExpression.PrepareForCompile(method);
					ret = _mathExpression.ReturnCodeExpression(method);
					vType = _mathExpression.ActualCompileDataType.LibType;
				}
			}
			if (ret == null)
			{
				object defValue = VPLUtil.GetDefaultValue(_value.BaseClassType);
				ret = ObjectCreationCodeGen.ObjectCreationCode(defValue);
				if (defValue != null)
				{
					vType = defValue.GetType();
				}
			}
			bool bNeedCast = true;
			bool isByRef = false;
			bool isNull = false;
			DataTypePointer tTarget = new DataTypePointer(_value.TargetCompileType);
			DataTypePointer tSource = new DataTypePointer(vType);
			CodePrimitiveExpression cpe = ret as CodePrimitiveExpression;
			if (cpe != null && cpe.Value == null)
			{
				isNull = true;
				bNeedCast = false;
				if (tTarget != null && tTarget.IsValueType)
				{
					ret = ObjectCreationCodeGen.ObjectCreationCode(VPLUtil.GetDefaultValue(tTarget.BaseClassType));
				}
			}
			if (!isNull)
			{
				if (_value.TargetCompileType != null)
				{
					isByRef = _value.TargetCompileType.IsByRef;
					if (isByRef)
					{
						tTarget = new DataTypePointer(_value.TargetCompileType.GetElementType());
					}
					if (tTarget.IsGenericParameter || tTarget.IsGenericType)
					{
						DataTypePointer dtp = this.GetConcreteType(tTarget.BaseClassType);
						if (dtp != null)
						{
							tTarget = dtp;
						}
					}
					else
					{
						if (tTarget.IsArray)
						{
							Type itemType = tTarget.BaseClassType.GetElementType();
							if (itemType.IsGenericType || itemType.IsGenericParameter)
							{
								DataTypePointer dtp = this.GetConcreteType(itemType);
								if (dtp != null)
								{
									int rank = tTarget.BaseClassType.GetArrayRank();
									if (rank > 1)
									{
										tTarget = new DataTypePointer(dtp.BaseClassType.MakeArrayType(rank));
									}
									else
									{
										tTarget = new DataTypePointer(dtp.BaseClassType.MakeArrayType());
									}
								}
							}
						}
					}
				}
				if (vType != null && tTarget != null)
				{
					if (vType.IsGenericParameter || vType.IsGenericType)
					{
						DataTypePointer dtp = this.GetConcreteType(vType);
						if (dtp != null)
						{
							tSource = dtp;
						}
					}
					if (tTarget.IsAssignableFrom(tSource))
					{
						bNeedCast = false;
					}
				}
				else
				{
					bNeedCast = false;
				}
				if (bNeedCast && tTarget != null)
				{
					if (tTarget.BaseClassType.Equals(typeof(bool)))
					{
						ret = CompilerUtil.ConvertToBool(vType, ret);
					}
					else if (_value.IsLibType)
					{
						if (tTarget.IsGenericParameter)
						{
							DataTypePointer dp = this.GetConcreteType(tTarget.BaseClassType);
							if (dp != null)
							{
								tTarget.SetConcreteType(dp);
							}
						}
						else if (tTarget.IsGenericType)
						{
						}
						if (tSource.IsGenericParameter)
						{
							DataTypePointer dp = this.GetConcreteType(tSource.BaseClassType);
							if (dp != null)
							{
								tSource.SetConcreteType(dp);
							}
						}
						else if (tSource.IsGenericType)
						{
						}
						ret = CompilerUtil.GetTypeConversion(tTarget, ret, tSource, statements);
					}
					else
					{
						ret = CompilerUtil.GetTypeConversion(_value.TypeString, ret);
					}
				}
			}
			if (isByRef)
			{
				bool needVar = true;
				if (!bNeedCast)
				{
					if (_valueDataType == EnumValueType.Property)
					{
						if (_valuePointer is LocalVariable)
						{
							needVar = false;
						}
					}
				}
				if (needVar)
				{
					string var = string.Format(CultureInfo.InvariantCulture, "ref{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(tTarget.GetCodeTypeReference(), var, ret);
					statements.Add(cvds);
					ret = new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression(var));
				}
			}
			return ret;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_value != null)
				{
					_value.SetTargetType(this.ObjectType);
					string ce = _value.GetJavaScriptReferenceCode(code);
					_compiledDataType = new DataTypePointer(_value.BaseClassType);
					return ce;
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					string ce = _valuePointer.GetJavaScriptReferenceCode(code);
					_compiledDataType = new DataTypePointer(_valuePointer.ObjectType);
					return ce;
				}
			}
			else if (_valueDataType == EnumValueType.MathExpression)
			{
				if (_mathExpression != null)
				{
					string ce = _mathExpression.CreateJavaScript(code);
					_compiledDataType = new DataTypePointer(_mathExpression.ActualCompileDataType.LibType);
					return ce;
				}
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_value != null)
				{
					_value.SetTargetType(this.ObjectType);
					return _value.GetPhpScriptReferenceCode(code);
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					return _valuePointer.GetPhpScriptReferenceCode(code);
				}
			}
			else if (_valueDataType == EnumValueType.MathExpression)
			{
				if (_mathExpression != null)
				{
					return _mathExpression.CreatePhpScript(code);
				}
			}
			return "NULL";
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
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
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				if (_actionContext != null)
				{
					IObjectPointer p = _actionContext.OwnerContext as IObjectPointer;
					if (p != null)
					{
						return p.Owner;
					}
				}
				if (_valuePointer != null)
				{
					return _valuePointer.Owner;
				}
				return null;
			}
			set
			{
				if (_valuePointer != null)
				{
					_valuePointer.Owner = value;
				}
				if (_mathExpression != null)
				{
				}
				if (_actionContext != null)
				{
					IObjectPointer p = _actionContext.OwnerContext as IObjectPointer;
					if (p != null)
					{
						p.Owner = value;
					}
				}
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					if (_value != null)
						return _value.ToString();
					return "{null}";
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.DisplayName;
					return "{?}";
				}
				else
				{
					if (_mathExpression == null)
						return "null";
					return _mathExpression.ToString();
				}
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					if (_value != null)
						return _value.ToString();
					return "{null}";
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.LongDisplayName;
					return "{?}";
				}
				else
				{
					if (_mathExpression == null)
						return "null";
					return _mathExpression.ToString();
				}
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				if (_valueDataType == EnumValueType.ConstantValue)
				{
					if (_value != null)
						return _value.ToString();
					return "{null}";
				}
				else if (_valueDataType == EnumValueType.Property)
				{
					if (_valuePointer != null)
						return _valuePointer.ExpressionDisplay;
					return "{?}";
				}
				else
				{
					if (_mathExpression == null)
						return "null";
					return _mathExpression.ToString();
				}
			}
		}
		/// <summary>
		/// don't know how it is use yet.
		/// </summary>
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (_actionContext != null)
				{
					return string.Format(CultureInfo.InvariantCulture,
						"v{0}_{1}", _actionContext.ActionContextId.ToString("x", CultureInfo.InvariantCulture), ParameterID.ToString("x", CultureInfo.InvariantCulture));
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture,
						"v{0}", ParameterID.ToString("x", CultureInfo.InvariantCulture));
				}
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				return EnumObjectDevelopType.Both;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Unknown; } }
		#endregion
		#region DataValuePropertyDescriptor class definition
		private class DataValuePropertyDescriptor : PropertyDescriptor
		{
			private ParameterValue _owner;
			private object _defaultValue;
			private Type _dataType;
			private EnumProp _prop;
			public DataValuePropertyDescriptor(ParameterValue owner, string name, Type propType, EnumProp property, object defaultValue, Attribute[] attrs) :
				base(name, attrs)
			{
				_owner = owner;
				_defaultValue = defaultValue;
				_dataType = propType;
				_prop = property;
			}

			public override Type ComponentType
			{
				get { return typeof(ParameterValue); }
			}

			public override bool IsReadOnly
			{
				get
				{
					switch (_prop)
					{
						case EnumProp.DataType:
							return true;
						case EnumProp.DataValueType:
							return false;
						case EnumProp.ConstantValue:
							return false;
						case EnumProp.DataPointer:
							return false;
						case EnumProp.MathExpression:
							return true;
					}
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					if (_prop == EnumProp.ConstantValue)
					{
						return _owner.DataType.BaseClassType;
					}
					return _dataType;
				}
			}

			public override bool CanResetValue(object component)
			{
				if (_defaultValue == null)
					return false;
				else
				{
					object v = this.GetValue(component);
					if (v != null)
					{
						return !v.Equals(_defaultValue);
					}
					return true;
				}
			}

			public override object GetValue(object component)
			{
				switch (_prop)
				{
					case EnumProp.DataType:
						return _owner.DataType;
					case EnumProp.DataValueType:
						return _owner._valueDataType;
					case EnumProp.ConstantValue:
						return _owner._value;
					case EnumProp.DataPointer:
						return _owner._valuePointer;
					case EnumProp.MathExpression:
						return _owner.MathExpression;
				}
				return null;
			}

			public override void ResetValue(object component)
			{
				SetValue(component, _defaultValue);
			}

			public override void SetValue(object component, object value)
			{
				switch (_prop)
				{
					case EnumProp.DataType:
						_owner.DataType.SetDataType(value);
						break;
					case EnumProp.DataValueType:
						_owner._valueDataType = (EnumValueType)value;
						break;
					case EnumProp.ConstantValue:
						_owner.SetValue(value);
						if (!_owner.ReadingProperties)
						{
							_owner._valueDataType = EnumValueType.ConstantValue;
						}
						break;
					case EnumProp.DataPointer:
						_owner._valuePointer = (IObjectPointer)value;
						if (!_owner.ReadingProperties)
						{
							_owner._valueDataType = EnumValueType.Property;
						}
						break;
					case EnumProp.MathExpression:
						_owner._mathExpression = (IMathExpression)value;
						_owner._mathExpression.ActionContext = _owner.ActionContext;
						_owner._mathExpression.ScopeMethod = _owner._scopeMethod;
						if (!_owner.ReadingProperties)
						{
							_owner._valueDataType = EnumValueType.MathExpression;
						}
						break;
				}
				_owner.FireValueChange();
				if (_owner.ActionContext != null)
				{
					_owner.ActionContext.OnChangeWithinMethod(true);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				if (string.CompareOrdinal(Name, "ConstantValue") == 0)
				{
					return _owner._valueDataType == EnumValueType.ConstantValue;
				}
				else if (string.CompareOrdinal(Name, "Property") == 0)
				{
					return _owner._valueDataType == EnumValueType.Property;
				}
				else if (string.CompareOrdinal(Name, "MathExpression") == 0)
				{
					return _owner._valueDataType == EnumValueType.MathExpression;
				}
				return true;
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
		/// Depending on the ValueType it shows different properties.
		/// ConstantValue:
		///     ConstObjectHolder.GetProperties
		/// Property:
		///     a PropertyDescriptor for selecting a property
		/// MathExpression:
		///     a PropertyDescriptor for launching math editor
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			int n;
			Attribute[] attrs;
			switch (ValueType)
			{
				case EnumValueType.ConstantValue:
					_value.UseValueEnum = (_valueEnum != null && _valueEnum.Length > 0);
					_value.SetOnValueChanged(PropertyChanged);
					_value.UseValueEditorAttribute = null;
					if (this.OwnerMethod != null && !(this.OwnerMethod is MethodClass) && this.OwnerMethod.IdentityOwner != null)
					{
						IObjectPointer op = this.OwnerMethod.IdentityOwner as IObjectPointer;
						if (op != null)
						{
							IValueUIEditorOwner vu = op.ObjectInstance as IValueUIEditorOwner;
							if (vu != null)
							{
								_value.UseValueEditorAttribute = vu.GetValueUIEditor(this.Name);
							}
						}
					}
					if (_constValueAttributes != null)
					{
						n = 0;
						if (attributes != null)
						{
							n = attributes.Length;
							attrs = new Attribute[n + _constValueAttributes.Count];
							attributes.CopyTo(attrs, 0);
							_constValueAttributes.CopyTo(attrs, n);
						}
						else
						{
							attrs = new Attribute[_constValueAttributes.Count];
							attributes.CopyTo(attrs, 0);
						}
					}
					else
					{
						attrs = attributes;
					}
					return _value.GetProperties(attrs);
				case EnumValueType.Property:
					if (attributes == null)
						n = 0;
					else
						n = attributes.Length;
					attrs = new Attribute[n + 1];
					if (n > 0)
					{
						attributes.CopyTo(attrs, 0);
					}
					attrs[n] = new EditorAttribute(typeof(PropEditorPropertyPointer), typeof(UITypeEditor));
					if (_valuePointer == null)
					{
						_valuePointer = new PropertyPointer();
					}
					return new PropertyDescriptorCollection(new PropertyDescriptor[]{
						new DataValuePropertyDescriptor(this, "Property", typeof(object), EnumProp.DataPointer, this._valuePointer, attrs)});
				case EnumValueType.MathExpression:
					if (attributes == null)
						n = 0;
					else
						n = attributes.Length;
					attrs = new Attribute[n + 1];
					if (n > 0)
					{
						attributes.CopyTo(attrs, 0);
					}
					attrs[n] = new EditorAttribute(typeof(UITypeEditorMathExpression2), typeof(UITypeEditor));
					return new PropertyDescriptorCollection(new PropertyDescriptor[]{
						new DataValuePropertyDescriptor(this, "MathExpression", typeof(object), EnumProp.MathExpression, this._mathExpression, attrs)});
			}
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
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
		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				if (_value != null)
				{
					if (_value.BaseClassType == null)
					{
					}
					if (_value.BaseClassType.IsGenericParameter)
					{
						if (_value.ConcreteType == null)
						{
							XmlObjectReader xr = serializer as XmlObjectReader;
							if (xr != null)
							{
								if (xr.ObjectStack.Count > 0)
								{
									IEnumerator ie = xr.ObjectStack.GetEnumerator();
									while (ie.MoveNext())
									{
										if (ie.Current != this)
										{
											ActionClass act = ie.Current as ActionClass;
											if (act != null)
											{
												DataTypePointer dp = act.GetConcreteType(_value.BaseClassType);
												if (dp != null)
												{
													_value.SetConcreteType(dp);
													break;
												}
											}
											else
											{
												MethodClass mc = ie.Current as MethodClass;
												if (mc != null)
												{
													DataTypePointer dp = mc.GetConcreteType(_value.BaseClassType);
													if (dp != null)
													{
														_value.SetConcreteType(dp);
														break;
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
			}
		}

		#endregion
		#region IDataScope Members
		public IObjectPointer ScopeOwner
		{
			get
			{
				if (_actionContext != null)
				{
					IObjectPointer p = _actionContext.OwnerContext as IObjectPointer;
					if (p != null)
					{
						return p.Owner;
					}
				}
				return null;
			}
			set
			{
				if (_actionContext != null)
				{
					IObjectPointer p = _actionContext.OwnerContext as IObjectPointer;
					if (p != null)
					{
						p.Owner = value;
					}
				}
			}
		}
		public Type ScopeDataType
		{
			get
			{
				return DataType.BaseClassType;
			}
			set
			{
				throw new NotImplementedException("ScopeDataType");
			}
		}
		#endregion
		#region ISerializeNotify Members
		[Browsable(false)]
		[ReadOnly(true)]
		public bool ReadingProperties
		{
			get
			{
				return _readingProperties;
			}
			set
			{
				_readingProperties = value;
			}
		}

		#endregion
		#region IParameter Members

		public UInt32 ParameterID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public string Name
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
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ParameterLibType
		{
			get
			{
				if (DataType != null)
				{
					return DataType.DataTypeEx;
				}
				return null;
			}
			set
			{
				throw new NotImplementedException("ParameterLibType");
			}
		}

		public string ParameterTypeString
		{
			get
			{
				return _value.TypeString;
			}
		}

		#endregion
		#region IFromString Members

		public object FromString(string value)
		{
			SetValue(value);
			return _value;
		}

		#endregion
		#region IXmlNodeSerializable Members
		protected const string XML_Value = "Value";
		const string XMLATT_ValueType = "valueType";
		protected virtual void OnWrite(IXmlCodeWriter writer, XmlNode node)
		{
		}
		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, _name);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ValueID, ParameterID);
			XmlUtil.SetAttribute(node, XMLATT_ValueType, _valueDataType.ToString());
			if (_value == null)
			{
				_value = new ConstObjectPointer(new TypePointer(typeof(object)));
				_value.Name = Name;
			}
			_value.SetOwnerParameter(this);
			XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_Value);
			nd.RemoveAll();
			_value.NotSaveType = true;
			_value.SetOwnerParameter(this);
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				_value.OnWriteToXmlNode((XmlObjectWriter)writer, nd);
			}
			else if (_valueDataType == EnumValueType.MathExpression)
			{
				if (_mathExpression != null)
				{
					writer.WriteObjectToNode(nd, _mathExpression, true);
				}
				else
				{
					XmlUtil.SetAttribute(nd, XmlTags.XMLATT_IsNull, true);
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					writer.WriteObjectToNode(nd, _valuePointer, true);
				}
				else
				{
					XmlUtil.SetAttribute(nd, XmlTags.XMLATT_IsNull, true);
				}
			}
			OnWrite(writer, node);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			string sn = XmlUtil.GetNameAttribute(node);
			if (!string.IsNullOrEmpty(sn))
			{
				_name = sn;
			}
			if (string.IsNullOrEmpty(_name))
			{
				if (string.CompareOrdinal(node.Name, "Data") == 0)
				{
					if (node.ParentNode != null && string.CompareOrdinal(node.ParentNode.Name, "Item") == 0)
					{
						_name = XmlUtil.GetNameAttribute(node.ParentNode);
					}
				}
			}
			_id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ValueID);
			_valueDataType = (EnumValueType)Enum.Parse(typeof(EnumValueType), XmlUtil.GetAttribute(node, XMLATT_ValueType));
			OnRead(reader, node);
		}
		protected virtual void OnRead(IXmlCodeReader reader, XmlNode node)
		{
			//resolve data type from the method ===========================
			if (_actionContext == null && _mapOwner == null)
			{
				throw new DesignerException("Reading ParameterValue [{0},{1}] failed: missing action", _id, _name);
			}
			object vt = null;
			if (_parentObject == null)
			{
				if (_mapOwner != null)
				{
					IVariable v = _mapOwner.GetVariableByKeyName(_name);
					if (v != null)
					{
						vt = v.VariableType.LibType;
					}
				}
				if (vt == null)
				{
					if (_actionContext != null)
					{
						IAction act = _actionContext as IAction;
						if (act != null)
						{
							MethodDataTransfer mdt = act.ActionMethod as MethodDataTransfer;
							if (mdt != null)
							{
								vt = this._value.DataType;
							}
						}
						if (vt == null)
						{
							vt = _actionContext.GetParameterType(_id);
							if (vt == null)
							{
								vt = _actionContext.GetParameterType(_name);
							}
						}
					}
				}
			}
			XmlNode nd = node.SelectSingleNode(XML_Value);
			if (vt == null)
			{
				if (nd != null)
				{
					if (_parentObject == null)
					{
						Type t = XmlUtil.GetLibTypeAttribute(nd);
						if (t != null)
						{
							vt = Activator.CreateInstance(t);
						}
					}
					else
					{
						vt = reader.ReadObject(nd, this);
					}
				}
			}
			if (vt == null)
			{
				XmlNode parentNode = node.ParentNode;
				if (parentNode != null)
				{
					if (string.CompareOrdinal(parentNode.Name, XmlSerializerUtility.XML_VarMap) == 0)
					{
					}
				}
			}
			if (vt == null)
			{
				if (_actionContext != null)
				{
					IAction act = _actionContext as IAction;
					if (act != null)
					{
						if (DesignUtil.IsComComponentOwned(act.MethodOwner))
						{
							vt = typeof(object);
						}
					}
				}
			}
			if (vt == null)
			{
				MethodInfoPointer mip = _actionContext.ExecutionMethod as MethodInfoPointer;
				if (mip != null)
				{
					PropertyPointer pp = mip.Owner as PropertyPointer;
					if (pp != null && pp.RunAt == EnumWebRunAt.Client)
					{
						IExtendedPropertyOwner epo = pp.Owner.ObjectInstance as IExtendedPropertyOwner;
						if (epo != null)
						{
							Type t = epo.PropertyCodeType(pp.MemberName);
							if (t != null)
							{
								if (t.Equals(typeof(DateTime)))
								{
									MethodInfo mif = typeof(JsDateTime).GetMethod(mip.MemberName);
									if (mif != null)
									{
										ParameterInfo[] pmifs = mif.GetParameters();
										if (pmifs != null && pmifs.Length > 0)
										{
											for (int i = 0; i < pmifs.Length; i++)
											{
												if (string.CompareOrdinal(pmifs[i].Name, this.Name) == 0)
												{
													vt = new DataTypePointer(pmifs[i].ParameterType);
													break;
												}
											}
										}
									}
								}
								if (vt == null)
								{
									MethodInfo mif = t.GetMethod(mip.MemberName);
									if (mif != null)
									{
										ParameterInfo[] pmifs = mif.GetParameters();
										if (pmifs != null && pmifs.Length > 0)
										{
											for (int i = 0; i < pmifs.Length; i++)
											{
												if (string.CompareOrdinal(pmifs[i].Name, this.Name) == 0)
												{
													vt = new DataTypePointer(pmifs[i].ParameterType);
													break;
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
			if (vt == null)
			{
				MethodPointer mp = _actionContext.ExecutionMethod as MethodPointer;
				if (mp != null)
				{
					IDynamicMethodParameters dmp = mp.ObjectInstance as IDynamicMethodParameters;
					if (dmp != null)
					{
						ParameterInfo[] pifs = dmp.GetDynamicMethodParameters(mp.MemberName, null);
						if (pifs != null && pifs.Length > 0)
						{
							for (int i = 0; i < pifs.Length; i++)
							{
								if (string.CompareOrdinal(pifs[i].Name, this.Name) == 0)
								{
									vt = new DataTypePointer(pifs[i].ParameterType);
									break;
								}
							}
						}
					}
				}
			}
			if (vt == null)
			{
				string sAct = string.Empty;
				if (_actionContext != null)
				{
					sAct = string.Format(CultureInfo.InvariantCulture, "{0}; {1}, Action ID:{2}", _actionContext.GetType().Name, _actionContext.ToString(), _actionContext.ActionContextId);
				}
				if (ScopeMethod != null)
				{
					sAct = string.Format(CultureInfo.InvariantCulture, "{0}; Scope:{1}", sAct, ScopeMethod.MethodName);
				}
				else
				{
					sAct = string.Format(CultureInfo.InvariantCulture, "{0}; Scope:null", sAct);
				}
				DesignUtil.WriteToOutputWindowAndLog("Reading ParameterValue [{0},{1}] failed: parameter type not resolved. ActionContext: [{2}]. Path:[{3}]. If this action is no longer used then it should be deleted.", _id, _name, sAct, XmlUtil.GetPath(node));
			}
			else
			{
				CustomPropertyPointer cpp = vt as CustomPropertyPointer;
				if (cpp != null)
				{
					_valueDataType = EnumValueType.Property;
					reader.ReadObjectFromXmlNode(nd, cpp, typeof(CustomPropertyPointer), this);
					_valuePointer = cpp;
					_value = new ConstObjectPointer(cpp.Property.PropertyType);
					_value.SetOwnerParameter(this);
				}
				else
				{
					DataTypePointer dp = vt as DataTypePointer;
					if (dp != null)
					{
						_value = new ConstObjectPointer(dp);
						_value.SetOwnerParameter(this);
					}
					else
					{
						Type t = vt as Type;
						if (t != null)
						{
							_value = new ConstObjectPointer(new DataTypePointer(new TypePointer(t)));
							_value.SetOwnerParameter(this);
						}
						else
						{
							TypePointer tp = vt as TypePointer;
							if (tp != null)
							{
								_value = new ConstObjectPointer(new DataTypePointer(tp));
								_value.SetOwnerParameter(this);
							}
							else
							{
								ClassPointer cp = vt as ClassPointer;
								if (cp != null)
								{
									_value = new ConstObjectPointer(cp);
									_value.SetOwnerParameter(this);
								}
								else
								{
									PropertyPointer pp = vt as PropertyPointer;
									if (pp != null)
									{
										_valuePointer = (IObjectPointer)reader.ReadObject(nd, this);
										_value = new ConstObjectPointer(new TypePointer(_valuePointer.ObjectType));
										_value.Name = _name;
										_value.SetOwnerParameter(this);
										return;
									}
								}
							}
						}
					}
					if (_value != null)
					{
						_value.NotSaveType = true;
						_value.Name = _name;
					}
					//=============================================================
					if (nd != null)
					{
						if (_valueDataType == EnumValueType.ConstantValue)
						{
							if (_value != null)
							{
								_value.OnReadFromXmlNode((XmlObjectReader)reader, nd);
							}
						}
						else
						{
							if (!XmlUtil.GetAttributeBoolDefFalse(nd, XmlTags.XMLATT_IsNull))
							{
								if (_valueDataType == EnumValueType.MathExpression)
								{
									_mathExpression = (IMathExpression)reader.ReadObject(nd, this);
									_mathExpression.ScopeMethod = _scopeMethod;
									_mathExpression.ActionContext = _actionContext;
								}
								else if (_valueDataType == EnumValueType.Property)
								{
									_valuePointer = (IObjectPointer)reader.ReadObject(nd, this);
								}
							}
						}
					}
				}
			}
			if (_value == null)
			{
				if (vt != null)
				{
					_value = new ConstObjectPointer();
					_value.Name = _name;
					_value.SetOwnerParameter(this);
					IMathExpression ime = vt as IMathExpression;
					if (ime != null)
					{
						_value.SetValue(ConstObjectPointer.VALUE_Type, ime.DataType.Type);
					}
					else
					{
						DataTypePointer dp = vt as DataTypePointer;
						if (dp != null)
						{
							_value.SetValue(ConstObjectPointer.VALUE_Type, dp);
						}
						else
						{
							IObjectPointer op = vt as IObjectPointer;
							if (op != null)
							{
								_value.SetValue(ConstObjectPointer.VALUE_Type, op.ObjectType);
							}
							else
							{
								_value.SetValue(ConstObjectPointer.VALUE_ValueType, vt);
							}
						}
					}
				}
			}
		}

		#endregion

		#region IXmlSerializeItem Members
		public void SetMapOwner(MathNodeRoot owner)
		{
			_mapOwner = owner;
		}
		public void ItemSerialize(object serializer, XmlNode node, bool saving)
		{
			if (saving)
			{
				OnWriteToXmlNode((XmlObjectWriter)serializer, node);
			}
			else
			{
				OnReadFromXmlNode((XmlObjectReader)serializer, node);
			}
		}

		#endregion

		#region ISerializeParent Members

		public void OnMemberCreated(object member)
		{
			IMathExpression ime = member as IMathExpression;
			if (ime != null)
			{
				ime.ActionContext = _actionContext;
				ime.ScopeMethod = _scopeMethod;
				ime.Name = _name;
				ime.VariableMapTargetType = typeof(ParameterValue);
			}
		}

		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get
			{
				if (_scopeMethod != null)
				{
					LimnorProject prj = _scopeMethod.ModuleProject as LimnorProject;
					if (prj != null)
					{
						return prj;
					}
				}
				if (_actionContext != null)
				{
					LimnorProject prj = _actionContext.ProjectContext as LimnorProject;
					if (prj != null)
					{
						return prj;
					}
				}
				return null;
			}
		}

		#endregion

		#region IValueEnumProvider Members
		private object[] _valueEnum;
		public object[] GetValueEnum(string propertyName)
		{
			return _valueEnum;
		}
		public void SetValueEnum(string propertyName, object[] values)
		{
			_valueEnum = values;
		}
		#endregion

		#region ICompileableItem Members

		public IList<ISourceValuePointer> GetValueSources()
		{
			List<ISourceValuePointer> list = new List<ISourceValuePointer>();
			switch (_valueDataType)
			{
				case EnumValueType.MathExpression:
					if (_mathExpression != null)
					{
						IList<ISourceValuePointer> l = _mathExpression.GetValueSources();
						if (l != null && l.Count > 0)
						{
							list.AddRange(l);
						}
					}
					break;
				case EnumValueType.Property:
					if (_valuePointer != null)
					{
						ISourceValuePointer sp = _valuePointer as ISourceValuePointer;
						if (sp != null)
						{
							list.Add(sp);
						}
					}
					break;
			}
			return list;
		}
		public bool IsSessionVariable()
		{
			if (_valueDataType == EnumValueType.Property)
			{
				if (_valuePointer != null)
				{
					return DesignUtil.IsSessionVariable(_valuePointer);
				}
			}
			return false;
		}
		public void GetValueSources(List<ISourceValuePointer> list)
		{
			switch (_valueDataType)
			{
				case EnumValueType.MathExpression:
					if (_mathExpression != null)
					{
						IList<ISourceValuePointer> l = _mathExpression.GetValueSources();
						if (l != null && l.Count > 0)
						{
							list.AddRange(l);
						}
					}
					break;
				case EnumValueType.Property:
					if (_valuePointer != null)
					{
						if (VPLUtil.SessionDataStorage == EnumSessionDataStorage.HTML5Storage || !DesignUtil.IsSessionVariable(_valuePointer))
						{
							ISourceValuePointer sp = DesignUtil.GetSourceValue(_valuePointer);
							if (sp != null)
							{
								list.Add(sp);
							}
						}
					}
					break;
			}
		}

		#endregion

		#region IGenericTypePointer Members
		public IList<DataTypePointer> GetGenericTypes()
		{
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_value != null)
				{
					return _value.GetGenericTypes();
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				IGenericTypePointer igp = _valuePointer as IGenericTypePointer;
				if (igp != null)
				{
					return igp.GetGenericTypes();
				}
			}
			else
			{
				if (_mathExpression != null)
				{
					List<DataTypePointer> lst = new List<DataTypePointer>();
					IList<ISourceValuePointer> values = _mathExpression.GetValueSources();
					if (values != null && values.Count > 0)
					{
						foreach (ISourceValuePointer sv in values)
						{
							IGenericTypePointer igp = sv as IGenericTypePointer;
							if (igp != null)
							{
								IList<DataTypePointer> dps = igp.GetGenericTypes();
								if (dps != null && dps.Count > 0)
								{
									foreach (DataTypePointer d0 in dps)
									{
										bool found = false;
										foreach (DataTypePointer d1 in lst)
										{
											if (string.CompareOrdinal(d0.DisplayName, d1.DisplayName) == 0)
											{
												found = true;
												break;
											}
										}
										if (!found)
										{
											lst.Add(d0);
										}
									}
								}
							}
						}
					}
					return lst.ToArray();
				}
			}
			return null;
		}
		public DataTypePointer[] GetConcreteTypes()
		{
			if (_valueDataType == EnumValueType.ConstantValue)
			{
				if (_value != null)
				{
					return _value.GetConcreteTypes();
				}
			}
			else if (_valueDataType == EnumValueType.Property)
			{
				IGenericTypePointer igp = _valuePointer as IGenericTypePointer;
				if (igp != null)
				{
					return igp.GetConcreteTypes();
				}
			}
			else
			{
				if (_mathExpression != null)
				{
					List<DataTypePointer> lst = new List<DataTypePointer>();
					IList<ISourceValuePointer> values = _mathExpression.GetValueSources();
					if (values != null && values.Count > 0)
					{
						foreach (ISourceValuePointer sv in values)
						{
							IGenericTypePointer igp = sv as IGenericTypePointer;
							if (igp != null)
							{
								DataTypePointer[] dps = igp.GetConcreteTypes();
								if (dps != null && dps.Length > 0)
								{
									lst.AddRange(dps);
								}
							}
						}
					}
					return lst.ToArray();
				}
			}
			return null;
		}

		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			DataTypePointer dp = null;
			if (_value != null)
			{
				dp = _value.GetConcreteType(typeParameter);
			}

			if (dp == null && _valueDataType == EnumValueType.Property)
			{
				IGenericTypePointer igp = _valuePointer as IGenericTypePointer;
				if (igp != null)
				{
					dp = igp.GetConcreteType(typeParameter);
				}
			}
			if (dp == null && _valueDataType == EnumValueType.MathExpression && _mathExpression != null)
			{
				IList<ISourceValuePointer> values = _mathExpression.GetValueSources();
				if (values != null && values.Count > 0)
				{
					foreach (ISourceValuePointer sv in values)
					{
						IGenericTypePointer igp = sv as IGenericTypePointer;
						if (igp != null)
						{
							dp = igp.GetConcreteType(typeParameter);
							if (dp != null)
							{
								return dp;
							}
						}
					}
				}
			}
			if (dp == null)
			{
				MethodClass mc = ScopeMethod as MethodClass;
				if (mc != null)
				{
					dp = mc.GetConcreteType(typeParameter);
				}
			}
			if (dp == null)
			{
				if (_actionContext != null && _actionContext.ExecutionMethod != null && _actionContext.ExecutionMethod.IdentityOwner != null)
				{
					CustomPropertyPointer cpp = _actionContext.ExecutionMethod.IdentityOwner as CustomPropertyPointer;
					if (cpp != null)
					{
						if (cpp.PropertyType != null && cpp.PropertyType.IsGenericType)
						{
							if (cpp.PropertyType.TypeParameters != null && cpp.PropertyType.TypeParameters.Length > 0)
							{
								dp = cpp.PropertyType.GetConcreteType(typeParameter);
							}
						}
					}
					if (dp == null)
					{
						LocalVariable lv = null;
						IObjectIdentity oi = _actionContext.ExecutionMethod.IdentityOwner;
						while (oi != null)
						{
							lv = oi as LocalVariable;
							if (lv != null)
							{
								break;
							}
							oi = oi.IdentityOwner;
						}
						if (lv != null)
						{
							dp = lv.GetConcreteType(typeParameter);
							if (dp == null)
							{
								DataTypePointer[] dps = lv.GetConcreteTypes();
								if (dps != null && dps.Length > 0)
								{
								}
							}
						}
					}
				}
			}
			return dp;
		}
		public CodeTypeReference GetCodeTypeReference()
		{
			DataTypePointer p = _value;
			if (p != null)
			{
				return p.GetCodeTypeReference();
			}
			return null;
		}
		#endregion
	}
	#region SelectorEnumValueType
	/// <summary>
	/// select value type (constant, property, or math expression) for ParameterValue
	/// </summary>
	class SelectorEnumValueType : UITypeEditor
	{
		Font f;
		public SelectorEnumValueType()
		{
			f = new Font("Times New Roman", 8);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				VariableMap map = context.Instance as VariableMap;
				if (map != null)
				{
					return true;
				}
				else
				{
					object obj = context.PropertyDescriptor.GetValue(context.Instance);
					ParameterValue pv = obj as ParameterValue;
					if (pv != null)
					{
						return (pv.ValueType != EnumValueType.ConstantValue);
					}
					else
					{
						IPropertyValueLinkHolder plh = context.Instance as IPropertyValueLinkHolder;
						if (plh != null)
						{
							PropertyValue pv2 = plh.GetPropertyLink(context.PropertyDescriptor.Name) as PropertyValue;
							if (pv2 != null)
							{
								return (pv2.ValueType != EnumValueType.ConstantValue);
							}
						}
					}
				}
			}
			return base.GetPaintValueSupported(context);
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			ParameterValue pv = null;
			if (e.Value != null)
			{
				MapItem item = e.Value as MapItem;
				if (item != null)
				{
					MathNode mn = item.Item.Key as MathNode;
					if (mn != null)
					{
						if (mn.CachedImage == null)
						{
							mn.CachedImage = mn.CreateIcon(e.Graphics);
						}
						e.Graphics.DrawImage(mn.CachedImage, e.Bounds);
					}
					return;
				}
				else
				{
					pv = e.Value as ParameterValue;
				}
			}
			if (pv == null)
			{
				if (e.Context != null && e.Context.PropertyDescriptor != null)
				{
					IPropertyValueLinkHolder plh = e.Context.Instance as IPropertyValueLinkHolder;
					if (plh != null)
					{
						pv = plh.GetPropertyLink(e.Context.PropertyDescriptor.Name) as ParameterValue;
					}
				}
			}
			if (pv != null)
			{
				string s = pv.ToString();
				SizeF size = e.Graphics.MeasureString(s, f);
				if (pv.ValueType == EnumValueType.MathExpression)
				{
					e.Graphics.DrawImage(Resources.math, e.Bounds);
					Rectangle rc = new Rectangle(e.Bounds.Left + Resources.math.Width + 1, e.Bounds.Top, (int)size.Width + 1, e.Bounds.Height);
					e.Graphics.DrawString(pv.ToString(), f, Brushes.Black, rc);
				}
				else if (pv.ValueType == EnumValueType.Property)
				{
					e.Graphics.DrawImage(Resources.property, e.Bounds);
					Rectangle rc = new Rectangle(e.Bounds.Left + Resources.property.Width + 1, e.Bounds.Top, (int)size.Width + 1, e.Bounds.Height);
					e.Graphics.DrawString(pv.ToString(), f, Brushes.Black, rc);
				}
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				IPropertyValueLinkHolder plHolder = null;
				ParameterValue pv = context.Instance as ParameterValue;
				ParameterValue pvOwner = null;
				ConstObjectPointer constOwner = null;
				EnumValueType vt = EnumValueType.ConstantValue;
				ConstObjectPointer.ParameterValueDescriptor pvdesc = context.PropertyDescriptor as ConstObjectPointer.ParameterValueDescriptor;
				if (pvdesc != null)
				{
					constOwner = pvdesc.Owner;
					object v = constOwner.GetParameterVlaue(pvdesc.Name);
					pvOwner = v as ParameterValue;
					if (pvOwner != null)
					{
						vt = pvOwner.ValueType;
					}
				}
				else
				{
					plHolder = context.Instance as IPropertyValueLinkHolder;
					if (plHolder != null)
					{
						pv = plHolder.GetPropertyLink(context.PropertyDescriptor.Name) as ParameterValue;
					}
					if (pv == null)
					{
						object obj = context.PropertyDescriptor.GetValue(context.Instance);
						if (obj != null)
						{
							pv = obj as ParameterValue;
							if (pv == null)
							{
								MapItem item = obj as MapItem;
								if (item != null)
								{
									pv = item.Item.Value as ParameterValue;
								}
							}
						}
					}
					if (pv != null)
					{
						vt = pv.ValueType;
					}
				}
				if (pv != null)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (service != null)
					{
						MethodDataTransfer mdt = null;
						Type t = service.GetType();
						PropertyInfo pif0 = t.GetProperty("OwnerGrid");
						TypeList list = new TypeList(service, vt);
						if (pv.ActionContext != null)
						{
							IAction act = pv.ActionContext as IAction;
							if (act != null)
							{
								mdt = act.ActionMethod as MethodDataTransfer;
								if (mdt != null)
								{
									list.Items.Add("---------------------");
									list.Items.Add("Delete");
								}
							}
						}
						service.DropDownControl(list);
						if (list.SelectedDelete)
						{
							mdt.DeleteProperty(context.PropertyDescriptor);
							if (pif0 != null)
							{
								object g = pif0.GetValue(service, null);
								PropertyGrid pg = g as PropertyGrid;
								if (pg != null)//scope method only uses MathPropertyGrid
								{
									pg.Refresh();
								}
							}
						}
						else
						{
							bool bChanged = (vt != list.SelectedValueType);
							if (bChanged)
							{
								if (pvdesc != null)
								{
									if (pvOwner != null)
									{
										pvOwner.ValueType = list.SelectedValueType;
									}
								}
								else
								{
									ParameterValueArrayItem pvai = value as ParameterValueArrayItem;
									if (pvai != null)
									{
										pvai.ValueType = list.SelectedValueType;
									}
									else
									{
										pv.ValueType = list.SelectedValueType;
									}
								}
								vt = list.SelectedValueType;
							}
							if (list.MadeSelection || bChanged)
							{
								bool bValueChanged = false;
								IMethod scopeMethod = pv.ScopeMethod;
								if (scopeMethod == null)
								{
									IScopeMethodHolder mh = context.Instance as IScopeMethodHolder;
									if (mh != null)
									{
										scopeMethod = mh.GetScopeMethod();
									}
								}
								if (scopeMethod == null)
								{
									if (pif0 != null)
									{
										object g = pif0.GetValue(service, null);
										MathPropertyGrid pg = g as MathPropertyGrid;
										if (pg != null)//scope method only uses MathPropertyGrid
										{
											scopeMethod = pg.ScopeMethod;
										}
									}
								}
								ParameterValue pvx = pv;
								if (vt != EnumValueType.ConstantValue)
								{
									if (pvdesc != null)
									{
										if (pvOwner == null)
										{
											pvOwner = new ParameterValue(pv.ActionContext);
											pvOwner.SetDataType(pvdesc.PropertyType);
										}
										pvOwner.ValueType = vt;
										pvdesc.Owner.SetParameterVlaue(pvdesc.Name, pvOwner);
										pvx = pvOwner;
									}
								}
								if (vt == EnumValueType.ConstantValue)
								{
									bValueChanged = true;
								}
								else if (vt == EnumValueType.Property)
								{
									IObjectPointer mm = pvx.Property;
									DataTypePointer tscope = null;
									if (pvx.DataType != null && pvx.DataType.BaseClassType != null)
									{
										if (typeof(Delegate).IsAssignableFrom(pvx.DataType.BaseClassType))
										{
											tscope = pvx.DataType;
										}
									}
									FrmObjectExplorer dlg = DesignUtil.GetPropertySelector(mm, scopeMethod, tscope);
									if (dlg != null)
									{
										if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
										{
											bValueChanged = true;
											IObjectPointer op = dlg.SelectedObject as IObjectPointer;
											if (op != null)
											{
												bool isValid = true;
												string msg = string.Empty;
												if (MethodEditContext.IsWebPage)
												{
													if (MethodEditContext.UseClientPropertyOnly)
													{
														if (op.RunAt == EnumWebRunAt.Server)
														{
															isValid = false;
															msg = "Server value is not allowed";
														}
													}
													else if (MethodEditContext.UseServerPropertyOnly)
													{
														if (op.RunAt == EnumWebRunAt.Client)
														{
															isValid = false;
															msg = "Client value is not allowed";
														}
													}
												}
												if (isValid)
												{
													ParameterValueArrayItem pai = value as ParameterValueArrayItem;
													if (pai != null)
													{
														pai.SetValue(op);
													}
													else
													{
														pvx.Property = op;
													}
												}
												else
												{
													bValueChanged = false;
													MessageBox.Show(msg, "Select value", MessageBoxButtons.OK, MessageBoxIcon.Error);
												}
											}
										}
									}
								}
								else if (vt == EnumValueType.MathExpression)
								{
									IMathExpression mew = pvx.MathExpression;
									Rectangle rc = new Rectangle(0, 0, 100, 30);
									System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
									rc.X = curPoint.X;
									rc.Y = curPoint.Y;
									IMathEditor dlg = mew.CreateEditor(rc);
									dlg.ActionContext = pv.ActionContext;
									dlg.SetScopeMethod(scopeMethod);
									dlg.VariableMapTargetType = typeof(ParameterValue);
									dlg.MathExpression = (IMathExpression)mew.Clone();
									dlg.MathExpression.ScopeMethod = scopeMethod;
									dlg.MathExpression.EnableUndo = true;
									if (service.ShowDialog((Form)dlg/*dlgA*/) == DialogResult.OK)
									{
										bool isValid = true;
										string msg = string.Empty;
										if (MethodEditContext.IsWebPage)
										{
											IList<ISourceValuePointer> vs = dlg.MathExpression.GetValueSources();
											if (vs != null && vs.Count > 0)
											{
												if (MethodEditContext.UseClientPropertyOnly)
												{
													foreach (ISourceValuePointer v in vs)
													{
														if (!v.IsWebClientValue())
														{
															isValid = false;
															msg = "Server value is not allowed";
															break;
														}
													}
												}
												else if (MethodEditContext.UseServerPropertyOnly)
												{
													foreach (ISourceValuePointer v in vs)
													{
														if (!v.IsWebServerValue())
														{
															isValid = false;
															msg = "Client value is not allowed";
															break;
														}
													}
												}
											}
										}
										if (isValid)
										{
											dlg.MathExpression.SetDataType(new MathExp.RaisTypes.RaisDataType(pvx.DataType.DataTypeEx));
											ParameterValueArrayItem pai = value as ParameterValueArrayItem;
											if (pai != null)
											{
												pai.SetValue(dlg.MathExpression);
											}
											else
											{
												pvx.MathExpression = dlg.MathExpression;
											}
											bValueChanged = true;
										}
										else
										{
											MessageBox.Show(msg, "Select value", MessageBoxButtons.OK, MessageBoxIcon.Error);
										}
									}
								}
								if (bValueChanged)
								{
									if (pv.ActionContext != null)
									{
										pv.ActionContext.OnChangeWithinMethod((scopeMethod != null));
										IAction ia = pv.ActionContext as IAction;
										if (ia != null && pv.RootPointer != null)
										{
											pv.RootPointer.SaveAction(ia, null);
											pv.RootPointer.NotifyChange(context.Instance, context.PropertyDescriptor.Name);
										}
									}
									if (plHolder != null)
									{
										plHolder.SetPropertyLink(context.PropertyDescriptor.Name, pv as IPropertyValueLink);
									}
									IPropertyValueLink pvl = pv as IPropertyValueLink;
									if (pvl != null)
									{
										pvl.OnDesignTimeValueChanged();
									}
								}
								if (pif0 != null)
								{
									PropertyGrid pg = pif0.GetValue(service, null) as PropertyGrid;
									if (pg != null)
									{
										pg.Refresh();
										if (pvx.ValueType == EnumValueType.MathExpression)
										{
											MathPropertyGrid mpg = pg as MathPropertyGrid;
											if (mpg != null)
											{
												mpg.OnValueChanged(pvx.MathExpression, EventArgs.Empty);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return value;
		}
		/// <summary>
		/// drop down list for showing EnumValueType (constant, property, and math expression)
		/// </summary>
		class TypeList : ListBox
		{
			IWindowsFormsEditorService _service;
			public EnumValueType SelectedValueType;
			public bool SelectedDelete;
			public bool MadeSelection;
			public TypeList(IWindowsFormsEditorService service, EnumValueType init)
			{
				_service = service;
				this.DrawMode = DrawMode.OwnerDrawFixed;
				Items.Add(EnumValueType.ConstantValue);
				Items.Add(EnumValueType.Property);
				Items.Add(EnumValueType.MathExpression);
				SelectedValueType = init;
				SelectedIndex = (int)init;
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				if (e.Index >= 0 && e.Index < this.Items.Count)
				{
					bool selected = ((e.State & DrawItemState.Selected) != 0);
					EnumValueType item = EnumValueType.ConstantValue;
					bool isSeperator = (e.Index == 3);
					bool isDelete = (e.Index == 4);
					string text;
					if (isSeperator)
					{
						text = "---------------";
					}
					else if (isDelete)
					{
						text = "Delete";
					}
					else
					{
						item = (EnumValueType)(this.Items[e.Index]);
						text = item.ToString();
					}
					Rectangle rcBK = new Rectangle(e.Bounds.Left, e.Bounds.Top, 1, this.ItemHeight);
					if (e.Bounds.Width > this.ItemHeight)
					{
						rcBK.Width = e.Bounds.Width - this.ItemHeight;
						rcBK.X = this.ItemHeight;
						if (selected)
						{
							e.Graphics.FillRectangle(Brushes.LightBlue, rcBK);
						}
						else
						{
							e.Graphics.FillRectangle(Brushes.White, rcBK);
						}
					}
					Rectangle rc = new Rectangle(e.Bounds.Left, e.Bounds.Top, this.ItemHeight, this.ItemHeight);
					float w = (float)(e.Bounds.Width - this.ItemHeight);
					if (w > 0)
					{
						RectangleF rcf = new RectangleF((float)(rc.Left + this.ItemHeight + 2), (float)(rc.Top), w, (float)this.ItemHeight);
						if (selected)
						{
							e.Graphics.DrawString(text, this.Font, Brushes.White, rcf);
						}
						else
						{
							e.Graphics.DrawString(text, this.Font, Brushes.Black, rcf);
						}
					}
					if (!isSeperator)
					{
						if (isDelete)
						{
							e.Graphics.DrawImage(Resources._cancel.ToBitmap(), rc);
						}
						else
						{
							switch (item)
							{
								case EnumValueType.ConstantValue:
									e.Graphics.DrawImage(Resources.abc, rc);
									break;
								case EnumValueType.Property:
									e.Graphics.DrawImage(Resources.property, rc);
									break;
								case EnumValueType.MathExpression:
									e.Graphics.DrawImage(Resources.math, rc);
									break;
							}
						}
					}
				}
			}
			private void getSelection()
			{
				if (SelectedIndex >= 0)
				{
					if (SelectedIndex != 3)
					{
						if (SelectedIndex == 4)
						{
							SelectedDelete = true;
						}
						else
						{
							SelectedDelete = false;
							SelectedValueType = (EnumValueType)(Items[SelectedIndex]);
						}
						MadeSelection = true;
					}
				}
			}
			protected override void OnClick(EventArgs e)
			{
				getSelection();
				if (MadeSelection)
				{
					_service.CloseDropDown();
				}
			}
			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					getSelection();
					if (MadeSelection)
					{
						_service.CloseDropDown();
					}
				}
			}
		}
	}
	#endregion
}
