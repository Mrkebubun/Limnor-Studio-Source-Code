/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using VPL;

namespace LimnorDatabase.DataTransfer
{
	public enum EnumDataSource { Database, TextFile }

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DTDataSource : IEPDataSource, ICustomTypeDescriptor, IDevClassReferencer
	{
		#region fields and constructors
		private DTSQuery _db;
		private DTSSourceText _text;
		private IDevClassReferencer _owner;
		public DTDataSource()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public IDevClassReferencer Owner { get { return _owner; } }

		[ParenthesizePropertyName(true)]
		[Description("Gets and sets the type of data source")]
		[RefreshProperties(RefreshProperties.All)]
		public EnumDataSource SourceType { get; set; }

		[Description("Gets and sets a data source which uses a database query as the source of the data")]
		public DTSQuery DatabaseSource
		{
			get
			{
				if (_db == null)
				{
					_db = new DTSQuery();
					_db.SetOwner(_owner);
					_db.SetDevClass(_class);
				}
				return _db;
			}
			set
			{
				_db = value;
				if (_db != null)
				{
					_db.SetOwner(_owner);
					if (_class != null)
					{
						_db.SetDevClass(_class);
					}
				}
			}
		}

		[Description("Gets and sets a data source which uses a text file as the source of the data")]
		public DTSSourceText TextSource
		{
			get
			{
				if (_text == null)
				{
					_text = new DTSSourceText();
				}
				_text.SetOwner(_owner);
				return _text;
			}
			set
			{
				_text = value;
				if (_text != null)
				{
					_text.SetOwner(_owner);
				}
			}
		}
		#endregion
		#region Method
		public void SetOwner(IDevClassReferencer owner)
		{
			_owner = owner;
			if (_text == null)
			{
				_text = new DTSSourceText();
			}
			_text.SetOwner(_owner);
		}
		public override string ToString()
		{
			return SourceType.ToString();
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			DTDataSource obj = new DTDataSource();
			obj.SetOwner(_owner);
			obj.SourceType = SourceType;
			obj._db = (DTSQuery)this.DatabaseSource.Clone();
			obj._text = (DTSSourceText)this.TextSource.Clone();
			obj.Timestamp = this.Timestamp;
			return obj;
		}

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (!VPLUtil.GetBrowseableProperties(attributes))
			{
				return ps;
			}
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal(p.Name, "TextSource") == 0)
				{
					if (SourceType == EnumDataSource.Database)
					{
						continue;
					}
				}
				else if (string.CompareOrdinal(p.Name, "DatabaseSource") == 0)
				{
					if (SourceType == EnumDataSource.TextFile)
					{
						continue;
					}
				}
				lst.Add(p);
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IEPDataSource Members
		[Browsable(false)]
		public string LastError
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.LastError;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.LastError;
				return null;
			}
		}
		[Browsable(false)]
		public DataTable DataSource
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.DataSource;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.DataSource;
				return null;
			}
		}
		[Browsable(false)]
		public ParameterList Parameters
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.Parameters;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.Parameters;
				return null;
			}
		}
		[Browsable(false)]
		public FieldList SourceFields
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.SourceFields;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.SourceFields;
				return null;
			}
		}
		[Browsable(false)]
		public bool IsJet
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.IsJet;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.IsJet;
				return false;
			}
		}
		[Browsable(false)]
		public void ClearData()
		{
			if (SourceType == EnumDataSource.Database)
				DatabaseSource.ClearData();
			if (SourceType == EnumDataSource.TextFile)
				TextSource.ClearData();
		}
		[Browsable(false)]
		public DateTime Timestamp
		{
			get
			{
				if (SourceType == EnumDataSource.Database)
					return DatabaseSource.Timestamp;
				if (SourceType == EnumDataSource.TextFile)
					return TextSource.Timestamp;
				return DateTime.MaxValue;
			}
			set
			{
				if (SourceType == EnumDataSource.Database)
					DatabaseSource.Timestamp = value;
				if (SourceType == EnumDataSource.TextFile)
					TextSource.Timestamp = value;
			}
		}

		#endregion

		#region IDevClassReferencer Members
		private IDevClass _class;
		public void SetDevClass(IDevClass c)
		{
			_class = c;
			DatabaseSource.SetDevClass(c);
		}

		public IDevClass GetDevClass()
		{
			return _class;
		}

		#endregion
	}
}
