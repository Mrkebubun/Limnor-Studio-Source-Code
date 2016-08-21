/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.IO;
using VPL;
using System.Xml;
using XmlUtility;
using Limnor.Net;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using LFilePath;
using System.Drawing.Design;
using System.Globalization;

namespace LimnorDatabase.DataTransfer
{

	/// <summary>
	/// Summary description for DTSSourceText.
	/// </summary>
	[UseParentObject]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DTSSourceText : ICloneable, IEPDataSource, IPropertyValueLinkHolder, ICustomTypeDescriptor
	{
		#region fields and constructors
		const string FILENAME = "Filename";
		private enumSourceTextDelimiter _delimiter = enumSourceTextDelimiter.TAB;
		private EnumCharEncode _encodingType = EnumCharEncode.Default;
		//working variables: results from parsing the text file ===
		protected FieldList fields = null;
		protected System.Data.DataTable tblSrc = null;
		private string _error;
		//=====================
		private PropertyValueLinks _links;
		private IDevClassReferencer _owner;
		public DTSSourceText()
		{
			DetectEncodingByByteOrderMark = false;
			_links = new PropertyValueLinks(this, FILENAME);
		}
		#endregion
		#region Properties
		[DefaultValue(EnumCharEncode.Default)]
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
		[DefaultValue(false)]
		public bool DetectEncodingByByteOrderMark
		{
			get;
			set;
		}
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
		[FilePath("text file|*.txt", "Select Source File", false)]
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

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			DTSSourceText obj = new DTSSourceText();
			obj.SetOwner(_owner);
			obj._links = new PropertyValueLinks(obj);
			obj._links.CopyData(_links);
			obj.Delimiter = Delimiter;
			obj.HasHeader = HasHeader;
			obj._encodingType = _encodingType;
			obj.DetectEncodingByByteOrderMark = DetectEncodingByByteOrderMark;
			return obj;
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

		protected string popField(ref string sIn)
		{
			if (sIn == null)
				return "";
			if (sIn.Length == 0)
				return "";
			int startIndex = 0;
			int qi2 = -1;
			int qi = sIn.IndexOf('"');
			if (qi >= 0)
			{
				qi2 = sIn.IndexOf('"', qi + 1);
			}
			int nTab = 0;
			while (true)
			{
				switch (_delimiter)
				{
					case enumSourceTextDelimiter.TAB:
						nTab = sIn.IndexOf('\t', startIndex);
						break;
					case enumSourceTextDelimiter.Comma:
						nTab = sIn.IndexOf(',', startIndex);
						break;
					default:
						throw new Exception("Unsupported delimiter");
				}
				if (qi<0 || qi2<0 || nTab <= 0)
				{
					break;
				}
				if (nTab > qi && nTab < qi2)
				{
					startIndex = qi2 + 1;
				}
				else
				{
					break;
				}
			}
			if (nTab == 0)
			{
				sIn = sIn.Substring(1);
				return "";
			}
			string sRet;
			if (nTab > 0)
			{
				sRet = sIn.Substring(0, nTab);
				if (nTab == sIn.Length - 1)
				{
					sIn = "";
				}
				else
					sIn = sIn.Substring(nTab + 1);
			}
			else
			{
				sRet = sIn;
				sIn = "";
			}
			if (!string.IsNullOrEmpty(sRet) && sRet.Length > 1)
			{
				if (sRet.StartsWith("\"", StringComparison.Ordinal) && sRet.EndsWith("\"", StringComparison.Ordinal))
				{
					sRet = sRet.Substring(1, sRet.Length - 2);
				}
			}
			return DataTransferConvert.Decode(_delimiter, sRet);
		}
		protected void parseFields(string sLine)
		{
			string s;
			for (int i = 0; i < fields.Count; i++)
			{
				s = popField(ref sLine);
				if (s.Length == 0)
					break;
				fields[i].Value = s;
			}
		}
		protected void parseFieldHeaders(string sFirstLine)
		{
			string s;
			EPField fld;
			int i = 0;
			fields = new FieldList();
			while (true)
			{
				s = popField(ref sFirstLine);
				if (s.Length == 0)
					break;
				if (this.HasHeader)
				{
					fld = new EPField(i, s);
					fields.AddField(fld);
				}
				else
				{
					fld = new EPField(i, "Column" + i.ToString());
					fld.Value = s;
					fields.AddField(fld);
				}
				i++;
			}
		}
		#endregion
		#region IEPDataSource Members
		[Browsable(false)]
		public string LastError
		{
			get
			{
				return _error;
			}
		}
		[Browsable(false)]
		public DataTable DataSource
		{
			get
			{
				if (tblSrc == null)
				{
					string sf = Filename;
					if (System.IO.File.Exists(sf))
					{
						string s = null;
						int i;
						DataRow rw;
						StreamReader sr = new StreamReader(sf);
						try
						{
							if (!sr.EndOfStream)
							{
								tblSrc = new System.Data.DataTable("TextSource");
								while (!sr.EndOfStream)
								{
									s = sr.ReadLine();
									if (!string.IsNullOrEmpty(s))
									{
										break;
									}
								}
								if (!string.IsNullOrEmpty(s))
								{
									parseFieldHeaders(s);
									for (i = 0; i < fields.Count; i++)
									{
										tblSrc.Columns.Add(fields[i].Name, typeof(string));
									}
									if (!this.HasHeader)
									{
										rw = tblSrc.NewRow();
										rw.BeginEdit();
										for (i = 0; i < fields.Count; i++)
										{
											rw[i] = fields[i].Value;
										}
										rw.EndEdit();
										tblSrc.Rows.Add(rw);
									}
									while (!sr.EndOfStream)
									{
										//read in one line
										s = sr.ReadLine();
										if (!string.IsNullOrEmpty(s))
										{
											parseFields(s);
											rw = tblSrc.NewRow();
											rw.BeginEdit();
											for (i = 0; i < fields.Count; i++)
											{
												rw[i] = fields[i].Value;
											}
											rw.EndEdit();
											tblSrc.Rows.Add(rw);
										}
									}
								}
							}
						}
						finally
						{
							sr.Close();
						}
					}
					else
					{
						_error = string.Format(CultureInfo.InvariantCulture, "File not found:{0}", sf);
					}
				}
				return tblSrc;
			}
		}
		[Browsable(false)]
		public ParameterList Parameters
		{
			get
			{
				return new ParameterList();
			}
		}
		[Browsable(false)]
		public FieldList SourceFields
		{
			get
			{
				if (fields == null)
				{
					fields = new FieldList();
					string sf = Filename;
					if (System.IO.File.Exists(sf))
					{
						string s;
						StreamReader sr;
						if (DetectEncodingByByteOrderMark)
							sr = new StreamReader(sf, true);
						else
						{
							if (_encodingType == EnumCharEncode.Default)
								sr = new StreamReader(sf, false);
							else
							{
								sr = new StreamReader(sf, EncodeUtility.GetEncoding(_encodingType));
							}
						}
						try
						{
							while (!sr.EndOfStream)
							{
								s = sr.ReadLine();
								if (!string.IsNullOrEmpty(s))
								{
									parseFieldHeaders(s);
									break;
								}
							}
						}
						finally
						{
							sr.Close();
						}
					}
				}
				return fields;
			}
		}
		[Browsable(false)]
		public void ClearData()
		{
			fields = null;
			if (tblSrc != null)
			{
				tblSrc.Dispose();
				tblSrc = null;
			}
		}
		[Browsable(false)]
		public DateTime Timestamp
		{
			get
			{
				// TODO:  Add DTSSourceText.Timestamp getter implementation
				return new DateTime();
			}
			set
			{
				// TODO:  Add DTSSourceText.Timestamp setter implementation
			}
		}
		[Browsable(false)]
		public bool IsJet
		{
			get
			{
				return false;
			}
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
	/// <summary>
	/// This class provides a ReadLine function that will not take
	/// a single line feed as a line terminator. It will only take
	/// a pair of CR LF as line terminator
	/// </summary>
	public class clsTextReader
	{
		System.IO.StreamReader sr = null;
		int sizeBuffer = 4096;
		int sizeRead = 0;
		int nStart = 0;
		string sBuffer = null;
		char[] cBuffer = new char[4096];
		bool bHasTrailingCR = false;
		public clsTextReader()
		{
		}
		public void Open(string filepath)
		{
			sr = new System.IO.StreamReader(filepath, System.Text.Encoding.ASCII);
		}
		public void Close()
		{
			if (sr != null)
			{
				sr.Close();
				sr = null;
			}
		}
		public string ReadLine()
		{
			bool bFoundLN = false;
			string sRet = null;
			int nCount;
			int i, j;
			char[] tmp = null;
			if (nStart < sizeRead) //has data in buffer
			{
				if (bHasTrailingCR) //the last character consumed is CR
				{
					bHasTrailingCR = false;
					if (cBuffer[nStart] == '\n') //got CR LF
					{
						nStart++;
						sRet = sBuffer;
						sBuffer = null;
						return sRet;
					}
				}
				for (i = nStart + 1; i < sizeRead; i++)
				{
					//check for CR LF
					if (cBuffer[i] == '\n' && cBuffer[i - 1] == '\r')
					{
						//found the end of line, now form a string
						bFoundLN = true;
						nCount = i - nStart - 1;
						if (nCount == 0)
						{
							sRet = "";
						}
						else
						{
							char[] ss = new Char[nCount];
							for (j = 0; j < nCount; j++)
								ss[j] = cBuffer[nStart + j];
							sRet = new String(ss);
						}
						nStart = i + 1;
						break;
					}
				}
				if (bFoundLN) //found a line
				{
					if (sBuffer != null)
					{
						sRet = sBuffer + sRet;
						sBuffer = null;
					}
					return sRet;
				}
				else
				{
					//Save the contents to buffer
					//before reading more from the file
					nCount = sizeRead - nStart;
					if (cBuffer[sizeRead - 1] == '\r')
					{
						bHasTrailingCR = true;
						nCount--;
					}
					if (nCount > 0)
					{
						tmp = new char[nCount];
						for (j = 0; j < nCount; j++)
							tmp[j] = cBuffer[j + nStart];
						if (sBuffer != null)
						{
							sBuffer += new string(tmp);
						}
						else
							sBuffer = new string(tmp);
					}
				}
			}
			//read more from the file
			sizeRead = 0;
			nStart = 0;
			sizeRead = sr.Read(cBuffer, 0, sizeBuffer);
			if (sizeRead == 0)
			{
				//that is all
				sRet = sBuffer;
				sBuffer = null;
				return sRet;
			}
			//do it again
			return ReadLine();
		}
	}
}
