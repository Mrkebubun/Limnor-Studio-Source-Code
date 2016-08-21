/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Data;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using VPL;
using TraceLog;
using System.Windows.Forms;
using Limnor.Net;
using System.Xml;
using XmlUtility;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Reflection;
using LFilePath;

namespace LimnorDatabase.DataTransfer
{
	/*
	 Define linkable properties:
	 * 1. initialize an instance of PropertyValueLinks, proving a list of properties names
	 * 2. Declare the corresponding properties - the code is semi-independent
	 * 3. Implement IPropertyValueLinkHolder - the code is class-independent
	 * 4. Implement ICustomTypeDescriptor - the code is class-independent
	 */
	/// <summary>
	/// export DataTable to a CSV file
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DTDestTextFile : IEPDataDest, IPropertyValueLinkHolder, ICustomTypeDescriptor
	{
		#region fields and constructors
		const int DEFBUFSIZE = 10240;
		const string FILENAME = "Filename";

		private enumSourceTextDelimiter _delimiter = enumSourceTextDelimiter.TAB;
		private bool _bAppend = false;
		private EnumCharEncode _encodingType = EnumCharEncode.ASCII;
		private int _bufferSize = DEFBUFSIZE;
		protected FieldList fields = null;
		//
		private PropertyValueLinks _links;
		private IDevClassReferencer _owner;
		public DTDestTextFile()
		{
			_links = new PropertyValueLinks(this, FILENAME);
		}
		#endregion
		#region Properties
		[DefaultValue(DEFBUFSIZE)]
		public int BufferSize
		{
			get
			{
				return _bufferSize;
			}
			set
			{
				if (value > 0)
				{
					_bufferSize = value;
				}
			}
		}
		[DefaultValue(EnumCharEncode.ASCII)]
		public EnumCharEncode EncodingType
		{
			get
			{
				return _encodingType;
			}
			set
			{
				_encodingType = value;
			}
		}
		[DefaultValue(enumSourceTextDelimiter.TAB)]
		public enumSourceTextDelimiter Delimiter
		{
			get { return _delimiter; }
			set { _delimiter = value; }
		}
		[DefaultValue(false)]
		public bool HasHeader
		{
			get;
			set;
		}
		/// <summary>
		/// compiler sets it by compiling IPropertyValueLink as PropertyValue
		/// </summary>
		[FilePath("text file|*.txt", "Select Destination File", true)]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		public string Filename
		{
			get
			{
				return _links.GetValue(FILENAME) as string;
			}
			set
			{
				_links.SetConstValue(FILENAME, value);
			}
		}
		[DefaultValue(false)]
		public bool Append
		{
			get
			{
				return _bAppend;
			}
			set
			{
				_bAppend = value;
			}
		}
		#endregion
		#region Methods
		public void SetOwner(IDevClassReferencer owner)
		{
			_owner = owner;
		}
		public override string ToString()
		{
			IPropertyValueLink pl = _links.GetValueLink(FILENAME);
			return pl.ToString();
		}
		#endregion
		#region IEPDataDest members
		[Browsable(false)]
		public bool IsReady
		{
			get
			{
				return _links.IsReady(FILENAME);
			}
		}
		public string ReceiveData(DataTable tblSrc, bool bSilent)
		{
			string error = null;
			System.IO.StreamWriter sw = null;
			if (tblSrc == null)
				return "Source data is null";
			if (tblSrc.Columns.Count == 0)
				return "Source data is empty";
			try
			{
				string sf = _links.GetValue(FILENAME) as string;
				bool bWriteHeader = HasHeader;
				if (bWriteHeader)
				{

					if (System.IO.File.Exists(sf))
					{
						System.IO.FileInfo fi = new System.IO.FileInfo(sf);
						if (fi.Length > 0)
						{
							bWriteHeader = false;
						}
					}
				}
				System.Text.StringBuilder sb;
				//
				sw = new System.IO.StreamWriter(sf, _bAppend, EncodeUtility.GetEncoding(EncodingType), BufferSize);
				if (bWriteHeader)
				{
					sb = new System.Text.StringBuilder();
					sb.Append(DataTransferConvert.Encode(_delimiter, tblSrc.Columns[0].ColumnName));
					for (int i = 1; i < tblSrc.Columns.Count; i++)
					{
						sb.Append(DataTransferConvert.Delimiter(_delimiter));
						sb.Append(DataTransferConvert.Encode(_delimiter, tblSrc.Columns[i].ColumnName));
					}
					sw.WriteLine(sb.ToString());
				}
				for (int r = 0; r < tblSrc.Rows.Count; r++)
				{
					sb = new System.Text.StringBuilder();
					sb.Append(DataTransferConvert.Encode(_delimiter, StringUtility.ToString(tblSrc.Rows[r][0])));
					for (int i = 1; i < tblSrc.Columns.Count; i++)
					{
						sb.Append(DataTransferConvert.Delimiter(_delimiter));
						sb.Append(DataTransferConvert.Encode(_delimiter, StringUtility.ToString(tblSrc.Rows[r][i])));
					}
					sw.WriteLine(sb.ToString());
				}
			}
			catch (Exception er)
			{
				error = ExceptionLimnorDatabase.FormExceptionText(er, "Error saving data to file {0}", Filename);
				TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
				TraceLogClass.TraceLog.Log(er);
			}
			finally
			{
				if (sw != null)
				{
					sw.Close();
				}
			}
			return error;
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			DTDestTextFile obj = new DTDestTextFile();
			obj.SetOwner(_owner);
			obj._links = new PropertyValueLinks(obj);
			obj._links.CopyData(_links);
			obj._delimiter = _delimiter;
			obj.HasHeader = HasHeader;
			obj._bAppend = _bAppend;
			obj.EncodingType = EncodingType;
			obj.BufferSize = BufferSize;
			return obj;
		}

		#endregion

		#region IPropertyValueLinkHolder Members
		public bool IsLinkableProperty(string propertyName)
		{
			return _links.IsLinkableProperty(propertyName);
		}
		public bool IsValueLinkSet(string propertyName)
		{
			return _links.IsValueLinkSet(propertyName);
		}
		public void SetPropertyLink(string propertyName, IPropertyValueLink link)
		{
			_links.SetPropertyLink(propertyName, link);
		}
		public IPropertyValueLink GetPropertyLink(string propertyName)
		{
			return _links.GetValueLink(propertyName);
		}
		public void OnDesignTimePropertyValueChange(string propertyName)
		{
			IDevClass c = _owner.GetDevClass();
			if (c != null)
			{
				c.NotifyChange(_owner, propertyName);
			}
		}
		public void SetPropertyGetter(string propertyName, fnGetPropertyValue getter)
		{
			_links.SetPropertyGetter(propertyName, getter);
		}

		public Type GetPropertyType(string propertyName)
		{
			PropertyInfo pif = this.GetType().GetProperty(propertyName);
			if (pif != null)
			{
				return pif.PropertyType;
			}
			return null;
		}
		public string[] GetLinkablePropertyNames()
		{
			return _links.GetLinkablePropertyNames();
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
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				lst.Add(_links.GetPropertyDescriptor(p));
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
