/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data.OleDb;
using VPL;
using System.CodeDom;
using System.Globalization;
using System.Collections.Specialized;
using System.Data;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DbCommandParam : ICustomTypeDescriptor
	{
		private EPField _p;
		public DbCommandParam()
		{
		}
		public DbCommandParam(EPField parameter)
		{
			_p = parameter;
		}
		public event EventHandler DataSizeChange;
		public event EventHandler DirectionChange;
		public event EventHandler BeforeNameChange;
		public event EventHandler AfterNameChange;
		//
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		[Browsable(false)]
		public bool CanChangeName
		{
			get;
			set;
		}
		//
		[NotForProgramming]
		[Browsable(false)]
		public EPField Field
		{
			get
			{
				return _p;
			}
		}
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				return _p.Name;
			}
			set
			{
				if (BeforeNameChange != null)
				{
					NameChangeEventArgs nce = new NameChangeEventArgs(value);
					BeforeNameChange(this, nce);
					if (nce.Cancel)
					{
						return;
					}
				}
				if (!string.IsNullOrEmpty(value))
				{
					if (value.StartsWith("@", StringComparison.Ordinal))
					{
						_p.Name = value;
					}
					else
					{
						_p.Name = string.Format(CultureInfo.InvariantCulture, "@{0}", value);
					}
					if (AfterNameChange != null)
					{
						AfterNameChange(this, EventArgs.Empty);
					}
				}
			}
		}
		[Description("Parameter data size for string parameter")]
		public int DataSize
		{
			get
			{
				return _p.DataSize;
			}
			set
			{
				if (_p.DataSize != value)
				{
					_p.DataSize = value;
					if (DataSizeChange != null)
					{
						DataSizeChange(this, EventArgs.Empty);
					}
				}
			}
		}

		private ParameterDirection _direction;
		[Description("Parameter direction")]
		public ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				if (_direction != value)
				{
					_direction = value;
					if (DirectionChange != null)
					{
						DirectionChange(this, EventArgs.Empty);
					}
				}
			}
		}
		[NotForProgramming]
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorSelectDbDataType), typeof(UITypeEditor))]
		public OleDbType Type
		{
			get
			{
				return _p.OleDbType;
			}
			set
			{
				_p.OleDbType = value;
				_p.CheckDataTypeAdjustValue();
			}
		}

		[NotForProgramming]
		public object DefaultValue
		{
			get
			{
				return _p.Value;
			}
			set
			{
				_p.SetValue(value);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public object Value
		{
			get
			{
				return _p.Value;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		[Description("Indicates if the current value for this field is null or not")]
		public bool IsNulll
		{
			get
			{
				return _p.IsNull;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsNotNulll
		{
			get
			{
				return _p.IsNotNull;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string ValueString
		{
			get
			{
				return _p.ValueString;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Int64 ValueInt64
		{
			get
			{
				return _p.ValueInt64;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public byte[] ValueBytes
		{
			get
			{
				return _p.ValueBytes;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool ValueBool
		{
			get
			{
				return _p.ValueBool;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public char ValueChar
		{
			get
			{
				return _p.ValueChar;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public double ValueDouble
		{
			get
			{
				return _p.ValueDouble;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public DateTime ValueDateTime
		{
			get
			{
				return _p.ValueDateTime;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Int32 ValueInt32
		{
			get
			{
				return _p.ValueInt32;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public float ValueFloat
		{
			get
			{
				return _p.ValueFloat;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Int16 ValueInt16
		{
			get
			{
				return _p.ValueInt16;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public sbyte ValueSByte
		{
			get
			{
				return _p.ValueSByte;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public UInt64 ValueUInt64
		{
			get
			{
				return _p.ValueUInt64;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public UInt32 ValueUInt32
		{
			get
			{
				return _p.ValueUInt32;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public UInt16 ValueUInt16
		{
			get
			{
				return _p.ValueUInt16;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public byte ValueByte
		{
			get
			{
				return _p.ValueByte;
			}
		}
		[Browsable(false)]
		public override string ToString()
		{
			return _p.Name;
		}
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
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.Compare(p.Name, "DefaultValue", StringComparison.Ordinal) == 0)
				{
					PropertyDescriptorValue p0 = new PropertyDescriptorValue(p.Name, attributes, EPField.ToSystemType(_p.OleDbType), this);
					list.Add(p0);
				}
				else if (string.Compare(p.Name, "Name", StringComparison.Ordinal) == 0)
				{
					if (this.CanChangeName)
					{
						list.Add(p);
					}
					else
					{
						PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), "Name", this.Name, attributes);
						list.Add(p0);
					}
				}
				else
				{
					list.Add(p);
				}
			}
			ps = new PropertyDescriptorCollection(list.ToArray());
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

		#region PropertyDescriptorValue
		class PropertyDescriptorValue : PropertyDescriptor
		{
			private Type _type;
			private DbCommandParam _owner;
			public PropertyDescriptorValue(string name, Attribute[] attrs, Type type, DbCommandParam owner)
				: base(name, attrs)
			{
				_type = type;
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(DbCommandParam); }
			}

			public override object GetValue(object component)
			{
				return _owner.DefaultValue;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _type; }
			}

			public override void ResetValue(object component)
			{
				_owner.DefaultValue = VPLUtil.GetDefaultValue(_type);
			}

			public override void SetValue(object component, object value)
			{
				_owner.DefaultValue = value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DbParameterList : ICustomTypeDescriptor, IExtendedPropertyOwner
	{
		private ParameterList _ps;
		private DbCommandParam[] _pp;
		public DbParameterList(ParameterList parameters)
		{
			_ps = parameters;
			if (_ps != null)
			{
				_pp = new DbCommandParam[_ps.Count];
				for (int i = 0; i < _ps.Count; i++)
				{
					_pp[i] = new DbCommandParam(_ps[i]);
				}
			}
			else
			{
				_ps = new ParameterList();
			}
		}
		public int Count
		{
			get
			{
				if (_ps == null)
					return 0;
				return _ps.Count;
			}
		}
		public DbCommandParam this[int index]
		{
			get
			{
				return _pp[index];
			}
		}
		public DbCommandParam this[string name]
		{
			get
			{
				for (int i = 0; i < _pp.Length; i++)
				{
					if (string.Compare(name, _pp[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return _pp[i];
					}
				}
				return null;
			}
		}
		protected virtual void OnAddedParameter(DbCommandParam p)
		{
		}
		public void Add(DbCommandParam p)
		{
			if (p != null)
			{
				if (_ps == null)
				{
					_ps = new ParameterList();
				}
				_ps.Add(p.Field);
				if (_pp == null)
				{
					_pp = new DbCommandParam[] { p };
				}
				else
				{
					DbCommandParam[] pp = new DbCommandParam[_pp.Length + 1];
					_pp.CopyTo(pp, 0);
					pp[_pp.Length] = p;
					_pp = pp;
				}
				OnAddedParameter(p);
			}
		}
		public int GetIndex(string name)
		{
			for (int i = 0; i < _pp.Length; i++)
			{
				if (string.Compare(name, _pp[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}
		public override string ToString()
		{
			if (Count > 0)
			{
				StringBuilder sb = new StringBuilder(this[0].Name);
				for (int i = 1; i < this.Count; i++)
				{
					sb.Append(",");
					sb.Append(this[i].Name);
				}
				return sb.ToString();
			}
			return string.Empty;
		}
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
			int n = Count;
			PropertyDescriptor[] ps = new PropertyDescriptor[n];
			for (int i = 0; i < n; i++)
			{
				ps[i] = new PropertyDescriptorDbParam(_pp[i].Name, attributes, _pp[i]);
			}
			return new PropertyDescriptorCollection(ps);
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

		#region PropertyDescriptorDbParam
		class PropertyDescriptorDbParam : PropertyDescriptor
		{
			DbCommandParam _param;
			public PropertyDescriptorDbParam(string name, Attribute[] attrs, DbCommandParam parameter)
				: base(name, attrs)
			{
				_param = parameter;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(DbParameterList); }
			}

			public override object GetValue(object component)
			{
				return _param;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(DbCommandParam); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region IExtendedPropertyOwner Members
		public Type PropertyCodeType(string propertyName)
		{
			if (_ps != null)
			{
				return _ps.PropertyCodeType(propertyName);
			}
			if (_pp != null)
			{
				for (int i = 0; i < _pp.Length; i++)
				{
					if (string.Compare(_pp[i].Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return EPField.ToSystemType(_pp[i].Type);
					}
				}
			}
			return null;
		}
		public CodeExpression GetReferenceCode(object method, CodeStatementCollection statements, string propertyName, CodeExpression target, bool forValue)
		{
			if (_ps != null)
			{
				return _ps.GetReferenceCode(method, statements, propertyName, target, forValue);
			}
			if (_pp != null)
			{
				for (int i = 0; i < _pp.Length; i++)
				{
					if (string.Compare(_pp[i].Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0)
					{

						if (forValue)
						{
							return VPLUtil.ConvertByType(EPField.ToSystemType(_pp[i].Type), new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), "Value"));
						}
						else
						{
							return new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), "Value");
						}

					}
				}
			}
			return target;
		}
		public string GetJavaScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			if (_ps != null)
			{
				return _ps.GetJavaScriptReferenceCode(code, propertyName, refCode);
			}
			if (_pp != null)
			{
				for (int i = 0; i < _pp.Length; i++)
				{
					if (string.Compare(_pp[i].Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", refCode, propertyName);
					}
				}
			}
			return refCode;
		}
		public string GetPhpScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			if (_ps != null)
			{
				return _ps.GetPhpScriptReferenceCode(code, propertyName, refCode);
			}
			if (_pp != null)
			{
				for (int i = 0; i < _pp.Length; i++)
				{
					if (string.Compare(_pp[i].Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", refCode, propertyName);
					}
				}
			}
			return refCode;
		}
		#endregion
	}
	class DbParamsConverter : ExpandableObjectConverter
	{
		public DbParamsConverter()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string data = value as string;
				if (!string.IsNullOrEmpty(data))
				{
					string[] ss = data.Split('|');
					DbParameterListExt ps = new DbParameterListExt();
					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							string[] ss2 = ss[i].Split(';');
							if (ss2.Length > 1)
							{
								EPField f = new EPField();
								DbCommandParam p = new DbCommandParam(f);
								if (!string.IsNullOrEmpty(ss2[0]))
								{
									if (!ss2[0].StartsWith("@", StringComparison.Ordinal))
									{
										f.Name = string.Format(CultureInfo.InvariantCulture, "@{0}", ss2[0]);
									}
									else
									{
										f.Name = ss2[0];
									}
									if (!string.IsNullOrEmpty(ss2[1]))
									{
										f.OleDbType = (OleDbType)Enum.Parse(typeof(OleDbType), ss2[1]);
										if (ss2.Length > 2)
										{
											int n;
											if (int.TryParse(ss2[2], out n))
											{
												f.DataSize = n;
											}
											if (ss2.Length > 3)
											{
												p.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), ss2[3]);
												if (ss2.Length > 4)
												{
													f.SetValue(ss2[4]);
												}
											}
										}
										ps.Add(p);
									}
								}
							}
						}
					}
					return ps;
				}
				else
				{
					return new DbParameterListExt();
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				DbParameterListExt list = value as DbParameterListExt;
				if (list != null)
				{
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < list.Count; i++)
					{
						if (i > 0)
						{
							sb.Append("|");
						}
						sb.Append(list[i].Name);
						sb.Append(";");
						sb.Append(list[i].Type);
						sb.Append(";");
						sb.Append(list[i].DataSize);
						sb.Append(";");
						sb.Append(list[i].Direction);
						if (list[i].Value != null)
						{
							sb.Append(";");
							sb.Append(list[i].Value);
						}
					}
					return sb.ToString();
				}
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(DbParamsConverter))]
	public class DbParameterListExt : DbParameterList
	{
		public DbParameterListExt()
			: base(null)
		{
		}
		public DbParameterListExt(ParameterList parameters)
			: base(parameters)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (!string.IsNullOrEmpty(this[i].Name))
				{
					if (!this[i].Name.StartsWith("@", StringComparison.Ordinal))
					{
						this[i].Name = string.Format(CultureInfo.InvariantCulture, "@{0}", this[i].Name);
					}
				}
				this[i].CanChangeName = true;
				this[i].BeforeNameChange += new EventHandler(DbParameterListExt_BeforeNameChange);
			}
		}
		protected override void OnAddedParameter(DbCommandParam p)
		{
			p.CanChangeName = true;
			p.BeforeNameChange += DbParameterListExt_BeforeNameChange;
		}
		private bool validateName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show("Name is empty", "Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (name.StartsWith("@", StringComparison.Ordinal))
			{
				name = name.Substring(1);
			}
			if (!VPLUtil.IsValidVariableName(name))
			{
				MessageBox.Show("Name is invalid", "Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			name = string.Format(CultureInfo.InvariantCulture, "@{0}", name);
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(name, this[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					MessageBox.Show("Name is in use", "Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
			return true;
		}
		void DbParameterListExt_BeforeNameChange(object sender, EventArgs e)
		{
			NameChangeEventArgs nce = e as NameChangeEventArgs;
			if (nce != null)
			{
				if (!validateName(nce.Name))
				{
					nce.Cancel = true;
				}
			}
		}
	}
}
