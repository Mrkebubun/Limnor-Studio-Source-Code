/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Design;
using VSPrj;
using System.Reflection;

namespace LimnorDesigner
{
	public partial class DlgSelectTypeParameters : Form
	{
		public TypeParameters _holder;
		public DlgSelectTypeParameters()
		{
			InitializeComponent();
		}
		public void LoadData(Type t, LimnorProject project)
		{
			_holder = new TypeParameters(t, project);
			lblType.Text = t.Name;
			propertyGrid1.SelectedObject = _holder;
		}
		public void LoadData(MethodBase method, LimnorProject project)
		{
			_holder = new TypeParameters(method, project);
			lblType.Text = method.ToString();
			propertyGrid1.SelectedObject = _holder;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < _holder.Results.Length; i++)
			{
				if (_holder.Results[i] == null)
				{
					MessageBox.Show(this, "Not all types are specified", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			this.DialogResult = DialogResult.OK;
		}


	}
	public class TypeParameters : ICustomTypeDescriptor, IWithProject
	{
		#region fields and constructors
		public DataTypePointer[] Results;
		private Type _type;
		private MethodBase _method;
		private LimnorProject _prj;
		public TypeParameters(Type t, LimnorProject project)
		{
			_type = t;
			_prj = project;
			Type[] ts = _type.GetGenericArguments();
			if (ts != null)
			{
				Results = new DataTypePointer[ts.Length];
				for (int i = 0; i < Results.Length; i++)
				{
					Results[i] = new DataTypePointer();
				}
			}
		}
		public TypeParameters(MethodBase method, LimnorProject project)
		{
			_method = method;
			_prj = project;
			Type[] ts = _method.GetGenericArguments();
			if (ts != null)
			{
				Results = new DataTypePointer[ts.Length];
				for (int i = 0; i < Results.Length; i++)
				{
					Results[i] = new DataTypePointer();
				}
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
			Type[] ts;
			if (_type != null)
				ts = _type.GetGenericArguments();
			else if (_method != null)
				ts = _method.GetGenericArguments();
			else ts = null;
			if (ts != null)
			{
				PropertyDescriptor[] ps = new PropertyDescriptor[ts.Length];
				for (int i = 0; i < ts.Length; i++)
				{
					List<Attribute> ls = new List<Attribute>();
					ls.Add(new EditorAttribute(typeof(PropEditorDataType), typeof(UITypeEditor)));
					ls.Add(new TypeScopeAttribute(ts[i]));
					ps[i] = new PropertyDescriptorTypeParam(ts[i], ls.ToArray());
				}
				return new PropertyDescriptorCollection(ps);
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
		#region class PropertyDescriptorTypeParam
		class PropertyDescriptorTypeParam : PropertyDescriptor
		{
			private Type _typeParam;
			public PropertyDescriptorTypeParam(Type typeParam, Attribute[] attrs)
				: base(string.Format(CultureInfo.InvariantCulture, "Type{0}({1})[{2}]", typeParam.GenericParameterPosition, typeParam.Name, GetConstraintDisplay(typeParam)),
				attrs)
			{
				_typeParam = typeParam;
			}
			static string GetConstraintDisplay(Type t)
			{
				Type[] ts = t.GetGenericParameterConstraints();
				if (ts == null || ts.Length == 0)
				{
					return "any";
				}
				else
				{
					StringBuilder sb = new StringBuilder(ts[0].Name);
					for (int i = 1; i < ts.Length; i++)
					{
						sb.Append(";");
						sb.Append(ts[i].Name);
					}
					return sb.ToString();
				}
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(TypeParameters); }
			}

			public override object GetValue(object component)
			{
				TypeParameters tp = component as TypeParameters;
				if (tp != null)
				{
					if (tp.Results[_typeParam.GenericParameterPosition] == null)
					{
						tp.Results[_typeParam.GenericParameterPosition] = new DataTypePointer();
					}
					return tp.Results[_typeParam.GenericParameterPosition];
				}
				return null;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(DataTypePointer); }
			}

			public override void ResetValue(object component)
			{
				TypeParameters tp = component as TypeParameters;
				if (tp != null)
				{
					tp.Results[_typeParam.GenericParameterPosition] = new DataTypePointer();
				}
			}

			public override void SetValue(object component, object value)
			{
				TypeParameters tp = component as TypeParameters;
				if (tp != null)
				{
					tp.Results[_typeParam.GenericParameterPosition] = (DataTypePointer)value;
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _prj; }
		}

		#endregion
	}

}
