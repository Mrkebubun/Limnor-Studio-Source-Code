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
using XmlSerializer;
using LimnorDesigner.MethodBuilder;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;
using LimnorDesigner.Property;

namespace LimnorDesigner
{
	/// <summary>
	/// a wrapper for presenting IClassRef.Properties to PropertyGrid. 
	/// IClassRef is implemented by ClassInstancePointer
	/// </summary>
	public class ClassProperties : ICustomTypeDescriptor, IConstructorOwner
	{
		#region fields and constructors
		private ClassInstancePointer _pointer;
		private object _owner;
		private ConstructorInstance _constructor;
		public ClassProperties(ClassInstancePointer owner)
		{
			_pointer = owner;
			_owner = VPLUtil.GetObject(owner.ObjectInstance);

		}
		#endregion
		#region Methods
		public List<ConstructorClass> GetCustomConstructors()
		{
			if (_pointer != null && _pointer.Definition != null)
			{
				return _pointer.Definition.GetConstructors();
			}
			return null;
		}
		#endregion
		#region Properties
		public IClassRef Pointer
		{
			get
			{
				return _pointer;
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(_owner, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(_owner, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(_owner, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(_owner, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(_owner, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(_owner, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(_owner, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(_owner, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(_owner, true);
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection _properties;
			IClassInstance ci = _owner as IClassInstance;
			if (ci != null)
			{
				_properties = ci.GetInstanceProperties(new Attribute[] { });
				if (_pointer.IsXType != EnumXType.None)
				{
					//remove Name property
					for (int i = 0; i < _properties.Count; i++)
					{
						if (string.CompareOrdinal(_properties[i].Name, "Name") == 0)
						{
							_properties.Remove(_properties[i]);
							break;
						}
					}
					//add Name property
					PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(_pointer.ObjectInstance, true);
					foreach (PropertyDescriptor p in ps)
					{
						if (string.CompareOrdinal(p.Name, "Name") == 0)
						{
							Attribute[] attrs;
							if (p.Attributes != null)
							{
								attrs = new Attribute[p.Attributes.Count];
								p.Attributes.CopyTo(attrs, 0);
							}
							else
							{
								attrs = new Attribute[] { };
							}
							_properties.Add(new PropertyDescriptorXName(this, p, attrs));
							break;
						}
					}
				}
				List<PropertyClassDescriptor> cps = _pointer.CustomProperties;
				if (cps != null && cps.Count > 0)
				{
					for (int i = 0; i < cps.Count; i++)
					{
						_properties.Add(cps[i]);
					}
				}
			}
			else
			{
				_properties = _pointer.Properties;
			}
			_constructor = new ConstructorInstance(this);
			if (_pointer.Definition != null)
			{
				List<ConstructorClass> ccs = _pointer.Definition.GetConstructors();
				if (ccs != null && ccs.Count > 0)
				{
					if (ccs.Count == 1 && ccs[0].ParameterCount == 0)
					{
					}
					else
					{
						_constructor.SetConstructor(ccs[0]);
						PropertyDescriptorConstructorInstance pconstruct = new PropertyDescriptorConstructorInstance(this);
						_properties.Add(pconstruct);
					}
				}
			}
			return _properties;
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _owner;
		}

		#endregion
		#region ConstructorInstance
		class ConstructorInstance : ConstValueSelector
		{
			private ClassProperties _owner;
			private ConstructorClass _constructor;
			public ConstructorInstance(ClassProperties owner)
				: base(owner._pointer.ObjectType, owner, "Constructor")
			{
				_owner = owner;
				setConstructorValues();
			}
			private void setConstructorValues()
			{
				if (_owner != null && _owner.Pointer != null)
				{
					ClassInstancePointer cip = _owner.Pointer as ClassInstancePointer;
					if (cip != null && cip.Host != null)
					{
						ClassPointer cp = cip.Host as ClassPointer;
						if (cp != null)
						{
							IXType xt = cp.ObjectList.GetObjectByID(cip.MemberId) as IXType;
							if (xt != null)
							{
								Dictionary<string, object> cvs = xt.GetConstructorParameterValues();
								if (cvs != null)
								{
									foreach (KeyValuePair<string, object> kv in cvs)
									{
										this.SetConstructorValue(kv.Key, kv.Value);
									}
								}
							}
						}
					}
				}
			}
			public void SetConstructor(ConstructorClass constructor)
			{
				_constructor = constructor;
				List<ParameterClass> ps = _constructor.Parameters;
				if (ps != null && ps.Count > 0)
				{
					this.ResetParameterTypes();
					foreach (ParameterClass p in ps)
					{
						this.Parameters.Add(p.Name, p.BaseClassType);
					}
				}
			}
		}
		#endregion
		#region TypeEditorConstructorInstanceSelect
		class TypeEditorConstructorInstanceSelect : UITypeEditor
		{
			public TypeEditorConstructorInstanceSelect()
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
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
						ClassProperties owner = context.Instance as ClassProperties;
						if (owner != null)
						{
							List<ConstructorClass> cifs = owner.GetCustomConstructors();
							if (cifs != null && cifs.Count > 0)
							{
								ValueList list = new ValueList(edSvc, cifs.ToArray());
								edSvc.DropDownControl(list);
								if (list.MadeSelection)
								{
									value = list.Selection;
								}
							}
						}
					}
				}
				return value;
			}
			class ValueList : ListBox
			{
				public bool MadeSelection;
				public ConstructorClass Selection;
				private IWindowsFormsEditorService _service;
				class ConstructorDisplay
				{
					public ConstructorDisplay(ConstructorClass c)
					{
						Constructor = c;
					}
					public ConstructorClass Constructor { get; set; }
					public override string ToString()
					{
						StringBuilder sb = new StringBuilder();
						List<ParameterClass> pifs = Constructor.Parameters;
						if (pifs != null && pifs.Count > 0)
						{
							sb.Append(pifs[0].ToString());
							for (int i = 1; i < pifs.Count; i++)
							{
								sb.Append(", ");
								sb.Append(pifs[i].ToString());
							}
						}
						return sb.ToString();
					}
				}
				public ValueList(IWindowsFormsEditorService service, ConstructorClass[] values)
				{
					_service = service;
					if (values != null && values.Length > 0)
					{
						for (int i = 0; i < values.Length; i++)
						{
							Items.Add(new ConstructorDisplay(values[i]));
						}
					}
				}
				private void finishSelection()
				{
					if (SelectedIndex >= 0)
					{
						MadeSelection = true;
						Selection = ((ConstructorDisplay)Items[SelectedIndex]).Constructor;
					}
					_service.CloseDropDown();
				}
				protected override void OnClick(EventArgs e)
				{
					base.OnClick(e);
					finishSelection();

				}
				protected override void OnKeyPress(KeyPressEventArgs e)
				{
					base.OnKeyPress(e);
					if (e.KeyChar == '\r')
					{
						finishSelection();
					}
				}
			}
		}
		#endregion
		#region PropertyDescriptorConstructorInstance
		class PropertyDescriptorConstructorInstance : PropertyDescriptor
		{
			private ClassProperties _owner;
			public PropertyDescriptorConstructorInstance(ClassProperties owner)
				: base("Constructor", new Attribute[]{
                    new RefreshPropertiesAttribute(RefreshProperties.All),
                    new EditorAttribute(typeof(TypeEditorConstructorInstanceSelect),typeof(UITypeEditor)),
                    new ParenthesizePropertyNameAttribute(true),
                    new TypeConverterAttribute(typeof(ExpandableObjectConverter))
            })
			{
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(ClassProperties); }
			}

			public override object GetValue(object component)
			{
				return _owner._constructor;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(ConstructorInstance); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				ConstructorClass cc = value as ConstructorClass;
				if (cc != null)
				{
					_owner._constructor.SetConstructor(cc);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region IConstructorOwner Members

		public ConstValueSelector GetCurrentConstructor()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IConstructorUser Members

		public string GetConstructorDisplay()
		{
			return _constructor.GetConstructorDisplay();
		}

		public ConstructorInfo[] GetConstructors()
		{
			if (_pointer != null && _pointer.Definition != null && _pointer.Definition.BaseClassType != null)
			{
				return _pointer.Definition.BaseClassType.GetConstructors();
			}
			return null;
		}

		public void SetConstructor(ConstructorInfo c)
		{

		}

		public void OnCustomValueChanged(string name, object value)
		{
			if (_pointer != null && _pointer.Host != null)
			{
				ClassPointer h = _pointer.Host as ClassPointer;
				if (h != null)
				{
					IXType xt = h.ObjectList.GetObjectByID(_pointer.MemberId) as IXType;
					if (xt != null)
					{
						xt.OnCustomValueChanged(name, value);
					}
					h.NotifyChange(_pointer, _pointer.Name);
				}
			}
		}

		public object GetConstructorValue(string name)
		{
			return _constructor.GetConstructorValue(name);
		}

		public void ResetConstructorValue(string name)
		{

		}

		public void SetConstructorValue(string name, object value)
		{
			_constructor.SetConstructorValue(name, value);
			if (_pointer != null && _pointer.Host != null)
			{
				ClassPointer h = _pointer.Host as ClassPointer;
				if (h != null)
				{
					IXType xt = h.ObjectList.GetObjectByID(_pointer.MemberId) as IXType;
					if (xt != null)
					{
						xt.OnConstructorValueChanged(name, value);
					}
				}
			}
		}

		public object GetValue()
		{
			return _constructor.GetValue();
		}

		#endregion

		#region class PropertyDescriptorXName
		class PropertyDescriptorXName : PropertyDescriptor
		{
			private ClassProperties _owner;
			private PropertyDescriptor _prop;
			public PropertyDescriptorXName(ClassProperties owner, PropertyDescriptor p, Attribute[] attrs)
				: base(p.Name, attrs)
			{
				_prop = p;
				_owner = owner;
			}

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
				return _prop.GetValue(_owner.Pointer.ObjectInstance);
			}

			public override bool IsReadOnly
			{
				get { return _prop.IsReadOnly; }
			}

			public override Type PropertyType
			{
				get { return _prop.PropertyType; }
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
				_prop.SetValue(_owner.Pointer.ObjectInstance, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
}
