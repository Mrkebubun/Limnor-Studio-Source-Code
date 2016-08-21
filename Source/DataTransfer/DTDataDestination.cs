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
using System.Globalization;

namespace LimnorDatabase.DataTransfer
{
	public enum EnumDataDestination { Database, TextFile }

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DTDataDestination : IEPDataDest, ICustomTypeDescriptor//, IXmlNodeSerializable
	{
		#region fields and constructors
		private DTDest _db;
		private DTDestTextFile _text;
		private IDevClassReferencer _owner;
		public DTDataDestination()
		{
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public IDevClassReferencer Owner { get { return _owner; } }

		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		public EnumDataDestination DestinationType
		{
			get;
			set;
		}
		public DTDest DatabaseDestination
		{
			get
			{
				if (_db == null)
				{
					_db = new DTDest();
					_db.SetOwner(_owner);
				}
				return _db;
			}
			set
			{
				_db = value;
				if (_db != null)
				{
					_db.SetOwner(_owner);
				}
			}
		}
		public DTDestTextFile TextDestination
		{
			get
			{
				if (_text == null)
				{
					_text = new DTDestTextFile();
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
				_text = new DTDestTextFile();
			}
			_text.SetOwner(_owner);
			if (_db == null)
			{
				_db = new DTDest();
			}
			_db.SetOwner(_owner);
		}
		public override string ToString()
		{
			return this.DestinationType.ToString();
		}
		#endregion

		#region IEPDataDest Members
		[Browsable(false)]
		public bool IsReady
		{
			get
			{
				if (DestinationType == EnumDataDestination.Database)
					return DatabaseDestination.IsReady;
				else if (DestinationType == EnumDataDestination.TextFile)
					return TextDestination.IsReady;
				return false;
			}
		}

		public string ReceiveData(DataTable tblSrc, bool bSilent)
		{
			if (DestinationType == EnumDataDestination.Database)
				return DatabaseDestination.ReceiveData(tblSrc, bSilent);
			else if (DestinationType == EnumDataDestination.TextFile)
				return TextDestination.ReceiveData(tblSrc, bSilent);
			return string.Format(CultureInfo.InvariantCulture, "Unsupported destination {0}", DestinationType);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			DTDataDestination obj = new DTDataDestination();
			obj.SetOwner(_owner);
			obj.DestinationType = DestinationType;
			obj._db = (DTDest)DatabaseDestination.Clone();
			obj._text = (DTDestTextFile)TextDestination.Clone();
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
				if (string.CompareOrdinal(p.Name, "TextDestination") == 0)
				{
					if (DestinationType == EnumDataDestination.Database)
					{
						continue;
					}
				}
				else if (string.CompareOrdinal(p.Name, "DatabaseDestination") == 0)
				{
					if (DestinationType == EnumDataDestination.TextFile)
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
	}
}
