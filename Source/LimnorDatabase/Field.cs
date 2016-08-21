/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Data.Odbc;
using VPL;
using System.CodeDom;
using System.Runtime.Serialization;
using System.Globalization;
using System.Collections.Specialized;
using MathExp;
using Limnor.WebBuilder;

namespace LimnorDatabase
{
	public class EPField : ICloneable
	{
		#region fields and constructors
		private string _name = "Field1";
		private string field; //field definition
		private string caption;
		private OleDbType _oleType = OleDbType.VarWChar;
		private int _dataSize = 255;
		private HorizontalAlignment _txtAlignment = HorizontalAlignment.Left;
		private HorizontalAlignment _headerAlignment = HorizontalAlignment.Center;
		private bool _visible = true;
		private bool _isCalculated;
		private int _colWidth = 80;
		private object _value = null;
		private string _format = string.Empty;
		private EasyQuery _query;
		public EPField()
		{
			FieldExpression = string.Empty;
		}
		public EPField(string sqlDef, string fieldCaption)
		{
			FieldExpression = string.Empty;
			field = sqlDef;
			caption = fieldCaption;
		}
		public EPField(int i, string sName)
		{
			Index = i;
			Name = sName;
			FieldText = sName;
			FieldCaption = sName;
			FieldExpression = string.Empty;
		}
		#endregion

		#region Properties
		//database attributes================================
		[Description("ordinal in the SELECT clause, not in the table")]
		public int Index { get; set; }

		[Description("Field name")]
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
		public string PureName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					return string.Empty;
				}
				int n = _name.LastIndexOf('.');
				if (n >= 0)
				{
					return _name.Substring(n + 1);
				}
				return _name;
			}
		}
		[Browsable(false)]
		[Description("Indicates whether this is an Identity field")]
		public bool IsIdentity { get; set; }

		[Browsable(false)]
		[Description("Indicates whether this field is part of Primary Key")]
		public bool Indexed { get; set; } //is part of row ID

		[Browsable(false)]
		public Type SystemType
		{
			get
			{
				return EPField.ToSystemType(this.OleDbType);
			}
		}

		[Browsable(false)]
		public OleDbType OleDbType
		{
			get
			{
				return _oleType;
			}
			set
			{
				_oleType = value;
			}
		}

		[Browsable(false)]
		public int DataSize
		{
			get
			{
				return FieldDataSize(this.OleDbType, _dataSize);
			}
			set
			{
				_dataSize = value;
			}
		}

		[Browsable(false)]
		public string FromTableName { get; set; }

		[Browsable(false)]
		public HorizontalAlignment TxtAlignment
		{
			get { return _txtAlignment; }
			set { _txtAlignment = value; }
		}

		[Browsable(false)]
		public HorizontalAlignment HeaderAlignment
		{
			get { return _headerAlignment; }
			set { _headerAlignment = value; }
		}
		[Browsable(false)]
		public bool IsFile { get; set; }

		[Browsable(false)]
		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		[Browsable(false)]
		public bool ReadOnly
		{
			get;
			set;
		}

		[Browsable(false)]
		public int ColumnWidth
		{
			get { return _colWidth; }
			set { _colWidth = value; }
		}
		[ReadOnly(true)]
		public DataEditor editor { get; set; }

		[ReadOnly(true)]
		public FieldCalculator calculator { get; set; }

		[Browsable(false)]
		public int Attributes { get; set; }

		[Description("Indicates if the current value for this field is null or not")]
		public bool IsNull
		{
			get
			{
				if (Value == null)
					return true;
				if (Value == DBNull.Value)
					return true;
				return false;
			}
		}
		[Description("Indicates if the current value for this field is null or an empty string")]
		public bool IsNullOrEmpty
		{
			get
			{
				if (Value == null)
					return true;
				if (Value == DBNull.Value)
					return true;
				if (EPField.IsString(this.OleDbType))
				{
					string s = Value as string;
					return string.IsNullOrEmpty(s);
				}
				return false;
			}
		}
		public bool IsNotNull
		{
			get
			{
				return (Value != null && Value != DBNull.Value);
			}
		}
		public bool IsNotNullOrEmpty
		{
			get
			{
				if (Value != null && Value != DBNull.Value)
				{
					if (EPField.IsString(this.OleDbType))
					{
						string s = Value as string;
						return !string.IsNullOrEmpty(s);
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public string ValueString
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return string.Empty;
				}
				return _value.ToString();
			}
		}
		public Int64 ValueInt64
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToInt64(_value);
			}
		}
		public byte[] ValueBytes
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return null;// new byte[] { };
				}
				return _value as byte[];
			}
		}
		public bool ValueBool
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return false;
				}
				return Convert.ToBoolean(_value);
			}
		}
		public char ValueChar
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return '\0';
				}
				return Convert.ToChar(_value);
			}
		}
		public double ValueDouble
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToDouble(_value);
			}
		}
		public DateTime ValueDateTime
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return DateTime.MinValue;
				}
				return Convert.ToDateTime(_value);
			}
		}
		public TimeSpan ValueTime
		{
			get
			{
				if (_value != DBNull.Value && _value != null)
				{
					if (_value is TimeSpan)
					{
						return (TimeSpan)_value;
					}
					string s = _value as string;
					if (!string.IsNullOrEmpty(s))
					{
						string[] ss = s.Split(':');
						int h = 0, m = 0, se = 0;
						if (ss.Length > 0)
						{
							int h0;
							if (int.TryParse(ss[0], out h0))
							{
								h = h0;
							}
							if (ss.Length > 1)
							{
								int m0;
								if (int.TryParse(ss[1], out m0))
								{
									m = m0;
								}
								if (ss.Length > 2)
								{
									int s0;
									if (int.TryParse(ss[2], out s0))
									{
										se = s0;
									}
								}
							}
						}
						return new TimeSpan(h, m, se);
					}
					else
					{
						try
						{
							long tk = Convert.ToInt64(_value);
							return new TimeSpan(tk);
						}
						catch
						{
						}
					}
				}
				return new TimeSpan(0, 0, 0);
			}
		}
		public Int32 ValueInt32
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToInt32(_value);
			}
		}
		public float ValueFloat
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToSingle(_value);
			}
		}
		public Int16 ValueInt16
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToInt16(_value);
			}
		}
		public sbyte ValueSByte
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToSByte(_value);
			}
		}
		public UInt64 ValueUInt64
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToUInt64(_value);
			}
		}
		public UInt32 ValueUInt32
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToUInt32(_value);
			}
		}
		public UInt16 ValueUInt16
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToUInt16(_value);
			}
		}
		public byte ValueByte
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					return 0;
				}
				return Convert.ToByte(_value);
			}
		}
		[ReadOnly(true)]
		[Description("The value for this field")]
		public object Value
		{
			get
			{
				if (_value == DBNull.Value || _value == null)
				{
					if (EPField.IsString(this.OleDbType))
					{
						return string.Empty;
					}
					if (EPField.IsDatetime(this.OleDbType))
					{
						return null;
					}
					return EPField.DefaultFieldValue(this.OleDbType);
				}
				return _value;
			}
			set
			{
				if (_query != null)
				{
					_query.SetFieldValue(this.Name, value);
				}
				_value = value;
			}
		}
		public string Format
		{
			get
			{
				return _format;
			}
			set
			{
				_format = value;
			}
		}
		/// <summary>
		/// for query builder. It is the complete field text.
		/// i.e. "sh.SalesHeaderID AS SHID"
		/// </summary>
		[Browsable(false)]
		public string FieldText
		{
			get
			{
				return field;
			}
			set
			{
				field = value;
			}
		}
		/// <summary>
		/// for setting Expression property of the column
		/// </summary>
		[Browsable(false)]
		public string FieldExpression { get; set; }
		/// <summary>
		/// return a string to be used as value part
		/// in SQL statements INSERT/UPDATE
		/// </summary>
		[Browsable(false)]
		public string GetFieldTextAsValue(string b1, string b2)
		{
			if (string.IsNullOrEmpty(field))
			{
				if (EPField.IsString(this.OleDbType))
					return "''";
				return "NULL";
			}
			if (string.IsNullOrEmpty(b2))
			{
				int n = field.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
				if (n > 0)
				{
					return field.Substring(0, n).Trim();
				}
			}
			else
			{
				if (field.EndsWith(b2))
				{
					int k = field.LastIndexOf(b1);
					if (k > 0)
					{
						string s = field.Substring(0, k).Trim();
						if (s.EndsWith(" AS", StringComparison.OrdinalIgnoreCase))
						{
							return s.Substring(0, s.Length - 3).Trim();
						}
					}
				}
				else
				{
					int k = field.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
					if (k > 0)
					{
						return field.Substring(0, k).Trim();
					}
				}
			}
			return field;
		}

		[Browsable(false)]
		public string FieldCaption
		{
			get
			{
				return caption;
			}
			set
			{
				caption = value;
			}
		}
		[Browsable(false)]
		public bool IsCalculated
		{
			get
			{
				return _isCalculated;
			}
			set
			{
				_isCalculated = value;
			}
		}
		#endregion

		#region Methods
		public DataColumn CreateDataColumn()
		{
			DataColumn c = new DataColumn(this.Name, this.SystemType);
			if (typeof(string).Equals(c.DataType))
			{
				c.MaxLength = _dataSize;
			}
			c.ReadOnly = this.ReadOnly;
			return c;
		}
		[Browsable(false)]
		internal void SetOwner(EasyQuery owner)
		{
			_query = owner;
		}
		public string GetParameterName(EnumParameterStyle pstyle)
		{
			switch (pstyle)
			{
				case EnumParameterStyle.QuestionMark:
					return "?";
				case EnumParameterStyle.LeadingAt:
					if (_name.StartsWith("@", StringComparison.Ordinal))
						return _name;
					return string.Format(CultureInfo.InvariantCulture, "@{0}", _name.GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				case EnumParameterStyle.LeadingQuestionMark:
					if (_name.StartsWith("?", StringComparison.Ordinal))
						return _name;
					return string.Format(CultureInfo.InvariantCulture, "?{0}", _name.GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				default:
					throw new ExceptionLimnorDatabase("Unsupported parameter style {0} for converting a field into a parameter", pstyle);
			}
		}
		public string GetParameterNameForCommand(EnumParameterStyle pstyle)
		{
			if (pstyle == EnumParameterStyle.QuestionMark)
			{
				string.Format(System.Globalization.CultureInfo.InvariantCulture, "@{0}", Name);
			}
			return GetParameterName(pstyle);
		}
		public void CheckDataTypeAdjustValue()
		{
			_value = CheckValueByOleDbType(Value, this.OleDbType);
		}
		public void SetValue(object v)
		{
			if (EPField.IsDatetime(this.OleDbType) && (v == null || v == DBNull.Value || (v is string && string.IsNullOrEmpty(v as string))))
			{
				_value = null;
			}
			else
			{
				bool b;
				_value = VPLUtil.ConvertObject(v, EPField.ToSystemType(OleDbType), out b);
			}
		}
		public void SetToRandomValue()
		{
			_value = VPLUtil.CreateRandomValue(this.SystemType);
		}
		public SqlDbType GetSqlDbType()
		{
			switch (OleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return System.Data.SqlDbType.BigInt;
				case System.Data.OleDb.OleDbType.Binary:
					return System.Data.SqlDbType.Binary;
				case System.Data.OleDb.OleDbType.Boolean:
					return System.Data.SqlDbType.Bit;
				case System.Data.OleDb.OleDbType.BSTR:
					return System.Data.SqlDbType.NVarChar;
				case System.Data.OleDb.OleDbType.Char:
					return System.Data.SqlDbType.Char;
				case System.Data.OleDb.OleDbType.Currency:
					return System.Data.SqlDbType.Money;
				case System.Data.OleDb.OleDbType.Date:
					return System.Data.SqlDbType.DateTime;
				case System.Data.OleDb.OleDbType.DBDate:
					return System.Data.SqlDbType.DateTime;
				case System.Data.OleDb.OleDbType.DBTime:
					return System.Data.SqlDbType.Time;
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return System.Data.SqlDbType.DateTime;
				case System.Data.OleDb.OleDbType.Decimal:
					return System.Data.SqlDbType.Decimal;
				case System.Data.OleDb.OleDbType.Double:
					return System.Data.SqlDbType.Float;
				case System.Data.OleDb.OleDbType.Filetime:
					return System.Data.SqlDbType.DateTime;
				case System.Data.OleDb.OleDbType.Integer:
					return System.Data.SqlDbType.Int;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return System.Data.SqlDbType.VarBinary;
				case System.Data.OleDb.OleDbType.LongVarChar:
					return System.Data.SqlDbType.Text;
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return System.Data.SqlDbType.NText;
				case System.Data.OleDb.OleDbType.Numeric:
					return System.Data.SqlDbType.Decimal;
				case System.Data.OleDb.OleDbType.Single:
					return System.Data.SqlDbType.Real;
				case System.Data.OleDb.OleDbType.SmallInt:
					return System.Data.SqlDbType.SmallInt;
				case System.Data.OleDb.OleDbType.TinyInt:
					return System.Data.SqlDbType.TinyInt;
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return System.Data.SqlDbType.BigInt;
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return System.Data.SqlDbType.Int;
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return System.Data.SqlDbType.SmallInt;
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return System.Data.SqlDbType.TinyInt;
				case System.Data.OleDb.OleDbType.VarBinary:
					return System.Data.SqlDbType.VarBinary;
				case System.Data.OleDb.OleDbType.VarChar:
					return System.Data.SqlDbType.VarChar;
				case System.Data.OleDb.OleDbType.Variant:
					return System.Data.SqlDbType.Variant;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return System.Data.SqlDbType.Decimal;
				case System.Data.OleDb.OleDbType.VarWChar:
					return System.Data.SqlDbType.NVarChar;
				case System.Data.OleDb.OleDbType.WChar:
					return System.Data.SqlDbType.NChar;
				default:
					return System.Data.SqlDbType.NVarChar;
			}
		}
		public OdbcType GetOdbcDbType()
		{
			switch (OleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return System.Data.Odbc.OdbcType.BigInt;
				case System.Data.OleDb.OleDbType.Binary:
					return System.Data.Odbc.OdbcType.Binary;
				case System.Data.OleDb.OleDbType.Boolean:
					return System.Data.Odbc.OdbcType.Bit;
				case System.Data.OleDb.OleDbType.BSTR:
					return System.Data.Odbc.OdbcType.NVarChar;
				case System.Data.OleDb.OleDbType.Char:
					return System.Data.Odbc.OdbcType.Char;
				case System.Data.OleDb.OleDbType.Currency:
					return System.Data.Odbc.OdbcType.Decimal;
				case System.Data.OleDb.OleDbType.Date:
					return System.Data.Odbc.OdbcType.Date;
				case System.Data.OleDb.OleDbType.DBDate:
					return System.Data.Odbc.OdbcType.DateTime;
				case System.Data.OleDb.OleDbType.DBTime:
					return System.Data.Odbc.OdbcType.Time;
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return System.Data.Odbc.OdbcType.DateTime;//.Timestamp;
				case System.Data.OleDb.OleDbType.Decimal:
					return System.Data.Odbc.OdbcType.Decimal;
				case System.Data.OleDb.OleDbType.Double:
					return System.Data.Odbc.OdbcType.Double;
				case System.Data.OleDb.OleDbType.Filetime:
					return System.Data.Odbc.OdbcType.DateTime;
				case System.Data.OleDb.OleDbType.Integer:
					return System.Data.Odbc.OdbcType.Int;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return System.Data.Odbc.OdbcType.VarBinary;
				case System.Data.OleDb.OleDbType.LongVarChar:
					return System.Data.Odbc.OdbcType.Text;
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return System.Data.Odbc.OdbcType.NText;
				case System.Data.OleDb.OleDbType.Numeric:
					return System.Data.Odbc.OdbcType.Numeric;
				case System.Data.OleDb.OleDbType.Single:
					return System.Data.Odbc.OdbcType.Real;
				case System.Data.OleDb.OleDbType.SmallInt:
					return System.Data.Odbc.OdbcType.SmallInt;
				case System.Data.OleDb.OleDbType.TinyInt:
					return System.Data.Odbc.OdbcType.TinyInt;
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return System.Data.Odbc.OdbcType.BigInt;
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return System.Data.Odbc.OdbcType.Int;
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return System.Data.Odbc.OdbcType.SmallInt;
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return System.Data.Odbc.OdbcType.TinyInt;
				case System.Data.OleDb.OleDbType.VarBinary:
					return System.Data.Odbc.OdbcType.VarBinary;
				case System.Data.OleDb.OleDbType.VarChar:
					return System.Data.Odbc.OdbcType.VarChar;
				case System.Data.OleDb.OleDbType.Variant:
					return System.Data.Odbc.OdbcType.VarBinary;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return System.Data.Odbc.OdbcType.Numeric;
				case System.Data.OleDb.OleDbType.VarWChar:
					return System.Data.Odbc.OdbcType.NVarChar;
				case System.Data.OleDb.OleDbType.WChar:
					return System.Data.Odbc.OdbcType.NChar;
				default:
					return System.Data.Odbc.OdbcType.NVarChar;
			}
		}
		public DbType GetDbType()
		{
			switch (OleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return DbType.Int64;
				case System.Data.OleDb.OleDbType.Binary:
					return DbType.Binary;
				case System.Data.OleDb.OleDbType.Boolean:
					return DbType.Boolean;
				case System.Data.OleDb.OleDbType.BSTR:
					return DbType.String;
				case System.Data.OleDb.OleDbType.Char:
					return DbType.Byte;
				case System.Data.OleDb.OleDbType.Currency:
					return DbType.Currency;
				case System.Data.OleDb.OleDbType.Date:
					return DbType.Date;
				case System.Data.OleDb.OleDbType.DBDate:
					return DbType.DateTime;
				case System.Data.OleDb.OleDbType.DBTime:
					return DbType.Time;
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return DbType.DateTime;
				case System.Data.OleDb.OleDbType.Decimal:
					return DbType.Decimal;
				case System.Data.OleDb.OleDbType.Double:
					return DbType.Double;
				case System.Data.OleDb.OleDbType.Filetime:
					return DbType.DateTime;
				case System.Data.OleDb.OleDbType.Integer:
					return DbType.Int32;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return DbType.Binary;
				case System.Data.OleDb.OleDbType.LongVarChar:
					return DbType.String;
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return DbType.String;
				case System.Data.OleDb.OleDbType.Numeric:
					return DbType.Decimal;
				case System.Data.OleDb.OleDbType.Single:
					return DbType.Single;
				case System.Data.OleDb.OleDbType.SmallInt:
					return DbType.Int16;
				case System.Data.OleDb.OleDbType.TinyInt:
					return DbType.SByte;
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return DbType.UInt64;
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return DbType.UInt32;
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return DbType.UInt16;
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return DbType.Byte;
				case System.Data.OleDb.OleDbType.VarBinary:
					return DbType.Binary;
				case System.Data.OleDb.OleDbType.VarChar:
					return DbType.String;
				case System.Data.OleDb.OleDbType.Variant:
					return DbType.String;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return DbType.VarNumeric;
				case System.Data.OleDb.OleDbType.VarWChar:
					return DbType.String;
				case System.Data.OleDb.OleDbType.WChar:
					return DbType.String;
				default:
					return DbType.String;
			}
		}
		public int GetMySqlPhpType()
		{
			switch (OleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return 3;
				case System.Data.OleDb.OleDbType.Binary:
					return 252;
				case System.Data.OleDb.OleDbType.Boolean:
					return 1;
				case System.Data.OleDb.OleDbType.BSTR:
					return 253;
				case System.Data.OleDb.OleDbType.Char:
					return 254;
				case System.Data.OleDb.OleDbType.Currency:
					return 0;
				case System.Data.OleDb.OleDbType.Date:
					return 12;
				case System.Data.OleDb.OleDbType.DBDate:
					return 12;
				case System.Data.OleDb.OleDbType.DBTime:
					return 12;
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return 12;
				case System.Data.OleDb.OleDbType.Decimal:
					return 0;
				case System.Data.OleDb.OleDbType.Double:
					return 5;
				case System.Data.OleDb.OleDbType.Filetime:
					return 12;
				case System.Data.OleDb.OleDbType.Integer:
					return 3;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return 251;
				case System.Data.OleDb.OleDbType.LongVarChar:
					return 252;
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return 252;
				case System.Data.OleDb.OleDbType.Numeric:
					return 0;
				case System.Data.OleDb.OleDbType.Single:
					return 4;
				case System.Data.OleDb.OleDbType.SmallInt:
					return 2;
				case System.Data.OleDb.OleDbType.TinyInt:
					return 1;
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return 3;
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return 3;
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return 2;
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return 1;
				case System.Data.OleDb.OleDbType.VarBinary:
					return 252;
				case System.Data.OleDb.OleDbType.VarChar:
					return 253;
				case System.Data.OleDb.OleDbType.Variant:
					return 253;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return 254;
				case System.Data.OleDb.OleDbType.VarWChar:
					return 253;
				case System.Data.OleDb.OleDbType.WChar:
					return 254;
				default:
					return 253;
			}
		}
		public bool TableSet()
		{
			return !string.IsNullOrEmpty(FromTableName);
		}


		public string GetFieldTextInSelect(EnumQulifierDelimiter useNameSep)
		{
			string s = field;
			if (!string.IsNullOrEmpty(s))
			{
				if (FieldsParser.FindStringIndex(s, " AS ", 0, DatabaseEditUtil.SepBegin(useNameSep), DatabaseEditUtil.SepEnd(useNameSep)) < 0)
				{
					string sName = FieldsParser.GetFieldNameFromFieldText(s, DatabaseEditUtil.SepBegin(useNameSep), DatabaseEditUtil.SepEnd(useNameSep));
					if (string.Compare(sName, Name, StringComparison.OrdinalIgnoreCase) != 0)
					{
						return s + " AS " + DatabaseEditUtil.SepBegin(useNameSep) + Name + DatabaseEditUtil.SepEnd(useNameSep);
					}
				}
			}
			else
			{
				s = "";
			}
			if (s.Length == 0)
			{
				if (DatabaseEditUtil.SepBegin(useNameSep).Length > 0)
					s = DatabaseEditUtil.SepBegin(useNameSep) + Name + DatabaseEditUtil.SepEnd(useNameSep);
				else
					s = Name;
			}
			return s;
		}
		/// <summary>
		/// return a string to be used in SELECT statement as a field
		/// </summary>
		public string FieldTextInSelect(EnumQulifierDelimiter sep)
		{
			return GetFieldTextInSelect(sep);
		}

		public bool FieldMatch(EPField fld)
		{
			if (string.Compare(fld.Name, Name, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.IsNullOrEmpty(fld.FromTableName) && string.IsNullOrEmpty(FromTableName))
					return true;
				if (!string.IsNullOrEmpty(FromTableName) && !string.IsNullOrEmpty(fld.FromTableName))
				{
					if (string.Compare(fld.FromTableName, FromTableName, StringComparison.OrdinalIgnoreCase) == 0)
						return true;
				}
				else
					return false;
			}
			return false;
		}
		public void SetDefaultEditor()
		{
			if (IsDatetime(OleDbType))
			{
				editor = new DataEditorDatetime();
			}
			else if (this.IsFile)
			{
				editor = new DataEditorFile();
			}
		}
		public string ConstQuote(bool isOleDb)
		{
			if (isOleDb)
			{
				if ((OleDbType == System.Data.OleDb.OleDbType.Date) ||
					(OleDbType == System.Data.OleDb.OleDbType.DBDate) ||
					(OleDbType == System.Data.OleDb.OleDbType.DBTime) ||
					(OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp))
					return "#";
			}
			if (NeedQuote())
				return "'";
			else
				return "";
		}
		public bool NeedQuote()
		{
			if (
				(OleDbType == System.Data.OleDb.OleDbType.VarBinary) ||
				(OleDbType == System.Data.OleDb.OleDbType.Binary) ||
				(OleDbType == System.Data.OleDb.OleDbType.Char) ||
				(OleDbType == System.Data.OleDb.OleDbType.VarChar) ||
				(OleDbType == System.Data.OleDb.OleDbType.WChar) ||
				(OleDbType == System.Data.OleDb.OleDbType.VarWChar) ||
				(OleDbType == System.Data.OleDb.OleDbType.Date) ||
				(OleDbType == System.Data.OleDb.OleDbType.DBDate) ||
				(OleDbType == System.Data.OleDb.OleDbType.DBTime) ||
				(OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp) ||
				(OleDbType == System.Data.OleDb.OleDbType.LongVarBinary) ||
				(OleDbType == System.Data.OleDb.OleDbType.LongVarChar) ||
				(OleDbType == System.Data.OleDb.OleDbType.LongVarWChar)
				)
				return true;
			else
				return false;

		}
		public override string ToString()
		{
			return Name;
		}
		#endregion

		#region Static Methods
		static public object CreateRandomValue(Type t)
		{
			return VPLUtil.CreateRandomValue(t);
		}
		static public int FieldDataSize(OleDbType tp, int size)
		{
			switch (tp)
			{
				case OleDbType.BigInt:
					return 8;
				case OleDbType.Boolean:
					return 1;
				case OleDbType.Char:
					return 1;
				case OleDbType.Currency:
					return 8;
				case OleDbType.Date:
					return 8;
				case OleDbType.DBDate:
					return 8;
				case OleDbType.DBTime:
					return 8;
				case OleDbType.DBTimeStamp:
					return 8;
				case OleDbType.Decimal:
					return 8;
				case OleDbType.Double:
					return 8;
				case OleDbType.Filetime:
					return 8;
				case OleDbType.Guid:
					return 16;
				case OleDbType.Integer:
					return 4;
				case OleDbType.Numeric:
					return 8;
				case OleDbType.Single:
					return 4;
				case OleDbType.SmallInt:
					return 2;
				case OleDbType.TinyInt:
					return 1;
				case OleDbType.UnsignedBigInt:
					return 8;
				case OleDbType.UnsignedInt:
					return 4;
				case OleDbType.UnsignedSmallInt:
					return 2;
				case OleDbType.UnsignedTinyInt:
					return 1;
			}
			return size;
		}
		static public bool IsString(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.BSTR ||
				tp == System.Data.OleDb.OleDbType.Char ||
				tp == System.Data.OleDb.OleDbType.VarChar ||
				tp == System.Data.OleDb.OleDbType.VarWChar ||
				tp == System.Data.OleDb.OleDbType.LongVarChar ||
				tp == System.Data.OleDb.OleDbType.LongVarWChar ||
				tp == System.Data.OleDb.OleDbType.WChar)
			{
				return true;
			}
			return false;
		}
		static public bool IsDatetime(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.Date ||
				tp == System.Data.OleDb.OleDbType.DBDate ||
				tp == System.Data.OleDb.OleDbType.DBTime ||
				tp == System.Data.OleDb.OleDbType.DBTimeStamp ||
				tp == System.Data.OleDb.OleDbType.Filetime)
				return true;
			return false;
		}
		static public bool IsBinary(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.Binary ||
				tp == System.Data.OleDb.OleDbType.LongVarBinary)
				return true;
			return false;
		}
		static public bool IsBoolean(System.Data.OleDb.OleDbType tp)
		{
			return (tp == System.Data.OleDb.OleDbType.Boolean);
		}
		static public bool IsNumber(Type t)
		{
			TypeCode tc = Type.GetTypeCode(t);
			if (tc == TypeCode.Byte || tc == TypeCode.Decimal || tc == TypeCode.Double
				|| tc == TypeCode.Int16 || tc == TypeCode.Int32
				|| tc == TypeCode.Int64 || tc == TypeCode.SByte
				|| tc == TypeCode.Single || tc == TypeCode.UInt16
				|| tc == TypeCode.UInt32 || tc == TypeCode.UInt64)
			{
				return true;
			}
			return false;
		}
		static public bool IsNumber(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.BigInt ||
				tp == System.Data.OleDb.OleDbType.Currency ||
				tp == System.Data.OleDb.OleDbType.Decimal ||
				tp == System.Data.OleDb.OleDbType.Double ||
				tp == System.Data.OleDb.OleDbType.Integer ||
				tp == System.Data.OleDb.OleDbType.Numeric ||
				tp == System.Data.OleDb.OleDbType.Single ||
				tp == System.Data.OleDb.OleDbType.SmallInt ||
				tp == System.Data.OleDb.OleDbType.TinyInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedBigInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedSmallInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedTinyInt ||
				tp == System.Data.OleDb.OleDbType.VarNumeric
				)
			{
				return true;
			}
			return false;
		}
		static public bool IsInteger(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.BigInt ||
				tp == System.Data.OleDb.OleDbType.Integer ||
				tp == System.Data.OleDb.OleDbType.SmallInt ||
				tp == System.Data.OleDb.OleDbType.TinyInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedBigInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedSmallInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedTinyInt
				)
			{
				return true;
			}
			return false;
		}
		static public object StringToTypedValue(System.Data.OleDb.OleDbType tp, string s)
		{
			if (s == null)
				return null;
			if (EPField.IsString(tp))
				return s;
			if (s.Length == 0)
				return null;
			if (EPField.IsBoolean(tp))
			{
				return ValueConvertor.ToBool(s);
			}
			if (EPField.IsDatetime(tp))
			{
				return ValueConvertor.ToDateTime(s);
			}
			if (EPField.IsNumber(tp))
			{
				try
				{
					double d = Convert.ToDouble(s);
					return d;
				}
				catch
				{
					return null;
				}
			}
			return null;
		}
		static public string TypeString(System.Data.OleDb.OleDbType tp)
		{
			if (IsInteger(tp))
				return "Integer";
			if (IsNumber(tp))
				return "Numeric";
			if (IsBoolean(tp))
				return "Yes/No";
			if (IsBinary(tp))
				return "Binary";
			if (IsDatetime(tp))
			{
				if (tp == System.Data.OleDb.OleDbType.DBDate)
					return "Date";
				else if (tp == System.Data.OleDb.OleDbType.DBTime)
					return "Time";
				else if (tp == System.Data.OleDb.OleDbType.DBTimeStamp)
					return "Timestamp";
				else
					return "Date/Time";
			}
			if (IsString(tp))
				return "String";
			return "Unknown";
		}
		public static object DefaultFieldValue(System.Data.OleDb.OleDbType tp)
		{
			if (tp == System.Data.OleDb.OleDbType.BigInt ||
				tp == System.Data.OleDb.OleDbType.Boolean ||
				tp == System.Data.OleDb.OleDbType.Currency ||
				tp == System.Data.OleDb.OleDbType.Decimal ||
				tp == System.Data.OleDb.OleDbType.Double ||
				tp == System.Data.OleDb.OleDbType.Integer ||
				tp == System.Data.OleDb.OleDbType.Numeric ||
				tp == System.Data.OleDb.OleDbType.Single ||
				tp == System.Data.OleDb.OleDbType.SmallInt ||
				tp == System.Data.OleDb.OleDbType.TinyInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedBigInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedSmallInt ||
				tp == System.Data.OleDb.OleDbType.UnsignedTinyInt ||
				tp == System.Data.OleDb.OleDbType.VarNumeric
				)
				return 0;
			if (tp == System.Data.OleDb.OleDbType.BSTR ||
				tp == System.Data.OleDb.OleDbType.LongVarChar ||
				tp == System.Data.OleDb.OleDbType.LongVarWChar ||
				tp == System.Data.OleDb.OleDbType.VarChar ||
				tp == System.Data.OleDb.OleDbType.VarWChar)
				return string.Empty;
			if (tp == System.Data.OleDb.OleDbType.Char ||
				tp == System.Data.OleDb.OleDbType.WChar
				)
				return ' ';
			if (tp == System.Data.OleDb.OleDbType.Date ||
				tp == System.Data.OleDb.OleDbType.DBDate ||
				tp == System.Data.OleDb.OleDbType.DBTimeStamp ||
				tp == System.Data.OleDb.OleDbType.Filetime)
				return System.DateTime.MinValue;
			if (tp == OleDbType.DBTime)
			{
				return new TimeSpan(0, 0, 0);
			}
			return string.Empty;
		}

		static public System.Data.OleDb.OleDbType GetOleDbType(int n)
		{
			System.Data.OleDb.OleDbType t = (System.Data.OleDb.OleDbType)n;
			return t;
		}
		static public DbType ToDBType(System.Type tp)
		{
			if (tp.Equals(typeof(byte[])))
				return DbType.Binary;
			if (tp.Equals(typeof(TimeSpan)) || tp.Equals(typeof(JsTimeSpan)))
			{
				return DbType.Time;
			}
			if (tp.Equals(typeof(string)) || tp.Equals(typeof(JsString)))
			{
				return DbType.String;
			}
			if (tp.Equals(typeof(DateTime)) || tp.Equals(typeof(JsDateTime)))
			{
				return DbType.DateTime;
			}
			if (tp.Equals(typeof(bool)) || tp.Equals(typeof(JsBool)))
			{
				return DbType.Boolean;
			}
			System.TypeCode tc = System.Type.GetTypeCode(tp);
			if (tc == System.TypeCode.Boolean)
				return DbType.Boolean;
			if (tc == System.TypeCode.Byte)
				return DbType.Byte;
			if (tc == System.TypeCode.DateTime)
				return DbType.DateTime;
			if (tc == System.TypeCode.Decimal)
				return DbType.Decimal;
			if (tc == System.TypeCode.Double)
				return DbType.Double;
			if (tc == System.TypeCode.Int16)
				return DbType.Int16;
			if (tc == System.TypeCode.Int32)
				return DbType.Int32;
			if (tc == System.TypeCode.Int64)
				return DbType.Int64;
			if (tc == System.TypeCode.SByte)
				return DbType.SByte;
			if (tc == System.TypeCode.Single)
				return DbType.Single;
			if (tc == System.TypeCode.UInt16)
				return DbType.UInt16;
			if (tc == System.TypeCode.UInt32)
				return DbType.UInt32;
			if (tc == System.TypeCode.UInt64)
				return DbType.UInt64;
			return DbType.String;
		}
		static public System.Type ToSystemType(DbType otp)
		{
			switch (otp)
			{
				case DbType.AnsiString:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case DbType.AnsiStringFixedLength:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case DbType.Binary:
					return typeof(byte[]);
				case DbType.Boolean:
					return typeof(bool);
				case DbType.Byte:
					return typeof(byte);
				case DbType.Currency:
					return typeof(double);
				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsDateTime);
					return typeof(DateTime);
				case DbType.Decimal:
					return typeof(Decimal);
				case DbType.Double:
					return typeof(double);
				case DbType.Guid:
					return typeof(Guid);
				case DbType.Int16:
					return typeof(Int16);
				case DbType.Int32:
					return typeof(Int32);
				case DbType.Int64:
					return typeof(Int64);
				case DbType.Object:
					return typeof(object);
				case DbType.SByte:
					return typeof(sbyte);
				case DbType.Single:
					return typeof(float);
				case DbType.String:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case DbType.StringFixedLength:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case DbType.Time:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsTimeSpan);
					return typeof(TimeSpan);
				case DbType.UInt16:
					return typeof(UInt16);
				case DbType.UInt32:
					return typeof(UInt32);
				case DbType.UInt64:
					return typeof(UInt64);
				case DbType.VarNumeric:
					return typeof(double);
				case DbType.Xml:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
			}
			if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
				return typeof(JsString);
			return typeof(string);
		}
		static public System.Type ToSystemType(System.Data.OleDb.OleDbType otp)
		{
			switch (otp)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return typeof(System.Int64);
				case System.Data.OleDb.OleDbType.Binary:
					return typeof(byte[]);
				case System.Data.OleDb.OleDbType.Boolean:
					return typeof(bool);
				case System.Data.OleDb.OleDbType.BSTR:
					return typeof(string);
				case System.Data.OleDb.OleDbType.Char:
					return typeof(char);
				case System.Data.OleDb.OleDbType.Currency:
					return typeof(double);
				case System.Data.OleDb.OleDbType.Date:
					return typeof(System.DateTime);
				case System.Data.OleDb.OleDbType.DBDate:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsDateTime);
					return typeof(System.DateTime);
				case System.Data.OleDb.OleDbType.DBTime:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsTimeSpan);
					return typeof(System.TimeSpan);
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsDateTime);
					return typeof(System.DateTime);
				case System.Data.OleDb.OleDbType.Decimal:
					return typeof(double);
				case System.Data.OleDb.OleDbType.Double:
					return typeof(double);
				case System.Data.OleDb.OleDbType.Filetime:
					return typeof(System.DateTime);
				case System.Data.OleDb.OleDbType.Integer:
					return typeof(System.Int32);
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return typeof(byte[]);
				case System.Data.OleDb.OleDbType.LongVarChar:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case System.Data.OleDb.OleDbType.LongVarWChar:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case System.Data.OleDb.OleDbType.Numeric:
					return typeof(double);
				case System.Data.OleDb.OleDbType.Single:
					return typeof(float);
				case System.Data.OleDb.OleDbType.SmallInt:
					return typeof(System.Int16);
				case System.Data.OleDb.OleDbType.TinyInt:
					return typeof(sbyte);
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return typeof(System.UInt64);
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return typeof(System.UInt32);
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return typeof(System.UInt16);
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return typeof(byte);
				case System.Data.OleDb.OleDbType.VarBinary:
					return typeof(byte[]);
				case System.Data.OleDb.OleDbType.VarChar:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case System.Data.OleDb.OleDbType.VarWChar:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
				case System.Data.OleDb.OleDbType.WChar:
					if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
						return typeof(JsString);
					return typeof(string);
			}
			if (VPLUtil.CurrentRunContext == EnumRunContext.Client)
				return typeof(JsString);
			return typeof(string);
		}
		static public System.Data.OleDb.OleDbType ToOleDBType(System.Type tp)
		{
			if (tp.Equals(typeof(byte[])))
				return System.Data.OleDb.OleDbType.LongVarBinary;
			if (tp.Equals(typeof(TimeSpan)) || tp.Equals(typeof(JsTimeSpan)))
			{
				return OleDbType.DBTime;
			}
			if (tp.Equals(typeof(string)) || tp.Equals(typeof(JsString)))
			{
				return OleDbType.VarWChar;
			}
			if (tp.Equals(typeof(DateTime)) || tp.Equals(typeof(JsDateTime)))
			{
				return OleDbType.DBDate;
			}
			if (tp.Equals(typeof(bool)) || tp.Equals(typeof(JsBool)))
			{
				return OleDbType.Boolean;
			}
			System.TypeCode tc = System.Type.GetTypeCode(tp);
			if (tc == System.TypeCode.Boolean)
				return System.Data.OleDb.OleDbType.Boolean;
			if (tc == System.TypeCode.Byte)
				return System.Data.OleDb.OleDbType.UnsignedTinyInt;
			if (tc == System.TypeCode.DateTime)
				return System.Data.OleDb.OleDbType.DBTimeStamp;
			if (tc == System.TypeCode.Decimal)
				return System.Data.OleDb.OleDbType.Double;
			if (tc == System.TypeCode.Double)
				return System.Data.OleDb.OleDbType.Double;
			if (tc == System.TypeCode.Int16)
				return System.Data.OleDb.OleDbType.SmallInt;
			if (tc == System.TypeCode.Int32)
				return System.Data.OleDb.OleDbType.Integer;
			if (tc == System.TypeCode.Int64)
				return System.Data.OleDb.OleDbType.BigInt;
			if (tc == System.TypeCode.SByte)
				return System.Data.OleDb.OleDbType.TinyInt;
			if (tc == System.TypeCode.Single)
				return System.Data.OleDb.OleDbType.Single;
			if (tc == System.TypeCode.UInt16)
				return System.Data.OleDb.OleDbType.UnsignedSmallInt;
			if (tc == System.TypeCode.UInt32)
				return System.Data.OleDb.OleDbType.UnsignedInt;
			if (tc == System.TypeCode.UInt64)
				return System.Data.OleDb.OleDbType.UnsignedBigInt;
			return System.Data.OleDb.OleDbType.VarWChar;
		}
		static public EPField MakeField(EPField obj, System.Data.DataView dv, int i)
		{
			//0"TABLE_CATALOG",1"TABLE_SCHEMA",2"TABLE_NAME",3"COLUMN_NAME",4"COLUMN_GUID"
			//5"COLUMN_PROPID",6"ORDINAL_POSITION",7"COLUMN_HASDEFAULT",
			//8"COLUMN_DEFAULT",9"COLUMN_FLAGS",10"IS_NULLABLE",
			//11"DATA_TYPE",12"TYPE_GUID",13"CHARACTER_MAXIMUM_LENGTH"
			//14"CHARACTER_OCTET_LENGTH",15"NUMERIC_PRECISION"
			//16"NUMERIC_SCALE",17"DATETIME_PRECISION"
			//18"CHARACTER_SET_CATALOG",19"CHARACTER_SET_SCHEMA"
			//20"CHARACTER_SET_NAME",21"COLLATION_CATALOG"
			//22"COLLATION_SCHEMA",23"COLLATION_NAME"
			//24"DOMAIN_CATALOG",25"DOMAIN_SCHEMA"
			//26"DOMAIN_NAME",27"DESCRIPTION"
			if (obj == null)
			{
				obj = new EPField();
				obj.Name = dv[i]["COLUMN_NAME"].ToString();
				obj.FromTableName = dv[i]["TABLE_NAME"].ToString();
				obj.Index = Convert.ToInt32(dv[i]["ORDINAL_POSITION"]) - 1;
			}
			int nType = (int)dv[i]["DATA_TYPE"];
			long nFlag = (long)dv[i]["COLUMN_FLAGS"];

			if ((nFlag & 0x100) != 0)
				obj.IsIdentity = true;
			obj.OleDbType = (System.Data.OleDb.OleDbType)nType;
			if (dv[i]["CHARACTER_MAXIMUM_LENGTH"] != System.DBNull.Value)
			{
				long nSize = (long)dv[i]["CHARACTER_MAXIMUM_LENGTH"];
				if (nSize < int.MaxValue)
					obj.DataSize = (int)nSize;
				else
					obj.OleDbType = System.Data.OleDb.OleDbType.LongVarBinary;
			}
			return obj;
		}
		static public EPField MakeFieldFromColumnInfo(int index, DataRow row)
		{
			//			0	ColumnName
			//			1	ColumnOrdinal
			//			2	ColumnSize
			//			4	NumericPrecision
			//			4	NumericScale
			//			5	DataType
			//			6	ProviderType
			//			7	IsLong
			//			8	AllDBNull
			//			9	IsReadOnly
			//			10	IsRowVersion
			//			11	IsUnique
			//			12	IsKey
			//			13	IsAutoIncrement
			//			14	BaseSchemaName
			//			15	BaseCatalogName
			//			16	BaseTableName
			//			17	BaseColumnName
			/*
			 MySQL
+		[0]	{ColumnName}	object {System.Data.DataColumn}
+		[1]	{ColumnOrdinal}	object {System.Data.DataColumn}
+		[2]	{ColumnSize}	object {System.Data.DataColumn}
+		[3]	{NumericPrecision}	object {System.Data.DataColumn}
+		[4]	{NumericScale}	object {System.Data.DataColumn}
+		[5]	{IsUnique}	object {System.Data.DataColumn}
+		[6]	{IsKey}	object {System.Data.DataColumn}
+		[7]	{BaseCatalogName}	object {System.Data.DataColumn}
+		[8]	{BaseColumnName}	object {System.Data.DataColumn}
+		[9]	{BaseSchemaName}	object {System.Data.DataColumn}
+		[10]	{BaseTableName}	object {System.Data.DataColumn}
+		[11]	{DataType}	object {System.Data.DataColumn}
+		[12]	{AllowDBNull}	object {System.Data.DataColumn}
+		[13]	{ProviderType}	object {System.Data.DataColumn}
+		[14]	{IsAliased}	object {System.Data.DataColumn}
+		[15]	{IsExpression}	object {System.Data.DataColumn}
+		[16]	{IsIdentity}	object {System.Data.DataColumn}
+		[17]	{IsAutoIncrement}	object {System.Data.DataColumn}
+		[18]	{IsRowVersion}	object {System.Data.DataColumn}
+		[19]	{IsHidden}	object {System.Data.DataColumn}
+		[20]	{IsLong}	object {System.Data.DataColumn}
+		[21]	{IsReadOnly}	object {System.Data.DataColumn}
			 */
			/*
				Name		   CLS type		C_Type			SQL_Type			DbType
				BigInt,		// Int64		CInt64			SQLBigInt			Int64
				Binary,		// byte[]		CBinary			SQLBinary			Binary
				Bit,		// bool			CBit			SQLBit				Boolean
				Char,		// string		CChar			SQLChar				AnsiStringFixedLength
				WideChar,	// string		CWChar			SQLWideChar			StringFixedLength
				Date,		// DateTime		CTimestamp		SQLTimestamp		Date
				Decimal,	// decimal		CChar			SQLDecimal			Decimal
				Double,		// double		CDouble			SQLDouble			Double
				Int,		// Int32		CInt32			SQLInteger			Int32
				Numeric,	// decimal		CChar			SQLNumeric			Decimal
				Real,		// single		CFloat			SQLReal				Single
				SmallInt,	// Int16		CInt16			SQLSmallInt			Int16
				Time,		// TimeSpan		CTime			SQLTime				Time
				Timestamp,	// DateTime		CTimestamp		SQLTimestamp		DateTime
				TinyInt,	// byte			CByte			SQLTinyInt			SByte
				VarBinary,	// byte[]		CBinary			SQLVarBinary		Binary
				VarChar,	// string		CChar			SQLVarChar			AnsiString
				VarWideChar	// string		CWChar			SQLVarWideChar		String
			*/

			EPField obj = new EPField();
			obj.Name = row["ColumnName"].ToString();
			obj.Index = index;
			obj.IsIdentity = (bool)row["IsAutoIncrement"];
			if (obj.IsIdentity)
				obj.ReadOnly = true;
			else
			{
				obj.ReadOnly = (bool)row["IsReadOnly"];
			}
			obj.OleDbType = EPField.ToOleDBType((System.Type)row["DataType"]);
			if (obj.OleDbType == OleDbType.Decimal)
			{
				int dpt = VPLUtil.ObjectToInt(row["ProviderType"]);
				if (dpt == 6)
				{
					obj.OleDbType = OleDbType.Currency;
				}
				else
				{
					obj.OleDbType = OleDbType.Double;
				}
			}
			try
			{
				obj.DataSize = (int)row["ColumnSize"];
				if (obj.DataSize > 10000)
				{
					if (obj.OleDbType == OleDbType.VarWChar || obj.OleDbType == OleDbType.WChar)
					{
						obj.OleDbType = OleDbType.LongVarWChar;
					}
					else if (obj.OleDbType == OleDbType.VarChar || obj.OleDbType == OleDbType.Char)
					{
						obj.OleDbType = OleDbType.LongVarChar;
					}
					else
					{
						obj.OleDbType = System.Data.OleDb.OleDbType.LongVarBinary;
					}
					obj.DataSize = -1;
				}
			}
			catch
			{
				obj.OleDbType = System.Data.OleDb.OleDbType.LongVarBinary;
			}
			string bn = VPLUtil.ObjectToString(row["BaseColumnName"]);
			if ((string.IsNullOrEmpty(bn) || string.Compare(obj.Name, bn, StringComparison.OrdinalIgnoreCase) != 0) && obj.ReadOnly)
			{
				obj.IsCalculated = true;
				obj.FromTableName = "";
			}
			else
			{
				if (row["BaseTableName"] != null && row["BaseTableName"] != System.DBNull.Value)
				{
					obj.FromTableName = row["BaseTableName"].ToString();
				}
				else
					obj.FromTableName = "";
			}
			obj.Indexed = VPLUtil.ObjectToBool(row["IsKey"]);
			obj.FieldText = "";

			return obj;
		}
		public static object CheckValueByOleDbType(object v, OleDbType t)
		{
			Type tp = ToSystemType(t);
			if (v == null || v == DBNull.Value)
			{
				return VPLUtil.GetDefaultValue(tp);
			}
			else
			{
				if (tp.IsAssignableFrom(v.GetType()))
				{
					return v;
				}
				else
				{
					return VPLUtil.GetDefaultValue(tp);
				}
			}
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			EPField obj = new EPField(field, caption);
			obj.Index = Index;
			obj.Name = Name;
			obj.IsCalculated = IsCalculated;
			obj.IsIdentity = IsIdentity;
			obj.Indexed = Indexed;
			obj.OleDbType = OleDbType;
			obj.DataSize = DataSize;
			obj.FromTableName = FromTableName;
			obj.IsFile = IsFile;
			obj.Visible = Visible;
			obj.ReadOnly = ReadOnly;
			obj.ColumnWidth = ColumnWidth;
			if (editor != null)
				obj.editor = (DataEditor)editor.Clone();
			if (Value != null)
				obj.SetValue(Value);
			obj.Attributes = Attributes;
			obj.TxtAlignment = TxtAlignment;
			obj.HeaderAlignment = HeaderAlignment;
			obj.Format = Format;
			return obj;
		}

		#endregion
	}
	//============================================================
	[Serializable]
	public class FieldList : List<EPField>, ICloneable, ICustomTypeDescriptor, IExtendedPropertyOwner, IFieldList, IDatabaseFieldProvider
	{
		#region constructor
		public FieldList()
		{
		}
		#endregion

		#region Non-Browsable Methods
		[Browsable(false)]
		public EPField this[string name]
		{
			get
			{
				int n = this.Count;
				for (int i = 0; i < n; i++)
				{
					if (string.Compare(this[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return this[i];
					}
				}
				return null;
			}
		}
		[Browsable(false)]
		public int FindMatchingFieldIndex(EPField f, string b1, string b2)
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				EPField f0 = this[i];
				if (string.IsNullOrEmpty(f.FromTableName))
				{
					if (string.IsNullOrEmpty(f0.FromTableName))
					{
						if (string.Compare(f0.GetFieldTextAsValue(b1, b2), f.GetFieldTextAsValue(b1, b2), StringComparison.OrdinalIgnoreCase) == 0)
						{
							return i;
						}
					}
				}
				else
				{
					if (string.Compare(f0.FromTableName, f.FromTableName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (string.Compare(f0.Name, f.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return i;
						}
						else
						{
							string s0 = f0.GetFieldTextAsValue(b1, b2);
							string s1 = f.GetFieldTextAsValue(b1, b2);
							if (string.Compare(s0, s1, StringComparison.OrdinalIgnoreCase) == 0)
							{
								return i;
							}
						}
					}
				}
			}
			return -1;
		}
		[Browsable(false)]
		public EPField FindField(string table, string name)
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (string.Compare(this[i].FromTableName, table, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (string.Compare(this[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return this[i];
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		public int FindFieldIndex(string name)
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (string.Compare(this[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}
		[Browsable(false)]
		public void SetDefaultEditor()
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				this[i].SetDefaultEditor();
			}
		}
		[Browsable(false)]
		public void AdjustEditor()
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (this[i].editor != null)
				{
					this[i].editor.SetFieldsAttribute(this);
				}
			}
		}
		[Browsable(false)]
		public bool HasRowID()
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (this[i].Indexed)
					return true;
				if (this[i].IsIdentity)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public bool HasIdentity()
		{
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (this[i].IsIdentity)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Duplicate names are not allowed
		/// </summary>
		/// <param name="fld"></param>
		[Browsable(false)]
		public int AddField(EPField fld)
		{
			if (fld == null)
				return -1;
			if (this[fld.Name] == null)
			{
				fld.Index = Count;
				Add(fld);
				return Count;
			}
			else
			{
				return -1;
			}
		}
		[Browsable(false)]
		public int AddFieldDup(EPField fld)
		{
			Add(fld);
			return Count;
		}
		[Browsable(false)]
		public bool DeleteField(int k)
		{
			int n = Count;
			if (k >= 0 && k < n)
			{
				RemoveAt(k);
				return true;
			}
			return false;
		}
		[Browsable(false)]
		public bool DeleteField(string name)
		{
			int i = FindFieldIndex(name);
			if (i >= 0)
			{
				return DeleteField(i);
			}
			return false;
		}
		[Browsable(false)]
		public void AppendList(FieldList lst)
		{
			AddRange((FieldList)lst.Clone());
		}
		[Browsable(false)]
		public void SwapFields(int i, int j)
		{
			if (i >= 0 && i < Count && j >= 0 && j < Count && i != j)
			{
				EPField fld = this[i];
				this[i] = this[j];
				this[i].Index = i;
				this[j] = fld;
				this[j].Index = j;
			}
		}
		#endregion

		#region Methods
		public bool FromSingleTable()
		{
			string tbl = string.Empty;
			foreach (EPField f in this)
			{
				if (!string.IsNullOrEmpty(f.FromTableName))
				{
					if (string.IsNullOrEmpty(tbl))
					{
						tbl = f.FromTableName;
					}
					else
					{
						if (string.Compare(f.FromTableName, tbl, StringComparison.OrdinalIgnoreCase) != 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public bool HasFieldsWithMissingValue()
		{
			foreach (EPField f in this)
			{
				if (f.Value == null || f.Value == DBNull.Value)
				{
					return true;
				}
				if (VPLUtil.IsDefaultValue(f.Value))
				{
					return true;
				}
			}
			return false;
		}
		public EPField GetIdentityField()
		{
			foreach (EPField f in this)
			{
				if (f.IsIdentity)
				{
					return f;
				}
			}
			return null;
		}
		public override string ToString()
		{
			string s = "";
			int n = this.Count;
			for (int i = 0; i < n; i++)
			{
				if (s.Length > 0)
					s += ",";
				s += this[i].Name;
			}
			return s;
		}
		#endregion

		#region ICloneable Members
		[Browsable(false)]
		public object Clone()
		{
			FieldList list = (FieldList)Activator.CreateInstance(this.GetType());
			for (int i = 0; i < Count; i++)
			{
				list.Add((EPField)this[i].Clone());
			}
			return list;
		}

		#endregion

		#region PropertyDescriptorField
		class PropertyDescriptorField : PropertyDescriptor
		{
			EPField _field;
			public PropertyDescriptorField(string name, Attribute[] attrs, EPField field)
				: base(name, attrs)
			{
				_field = field;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(FieldList); }
			}

			public override object GetValue(object component)
			{
				return _field.Value;
			}

			public override bool IsReadOnly
			{
				get { return _field.ReadOnly; }
			}

			public override Type PropertyType
			{
				get { return EPField.ToSystemType(_field.OleDbType); }
			}

			public override void ResetValue(object component)
			{
				_field.SetValue(VPLUtil.GetDefaultValue(PropertyType));
			}

			public override void SetValue(object component, object value)
			{
				_field.SetValue(value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
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

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				list.Add(p);
			}
			foreach (EPField fld in this)
			{
				list.Add(new PropertyDescriptorField(fld.Name, attributes, fld));
			}
			return new PropertyDescriptorCollection(list.ToArray());
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

		#region IExtendedPropertyOwner Members

		/// <summary>
		/// generate FieldList[Field1].Value
		/// </summary>
		/// <param name="method">an IMethodCompile</param>
		/// <param name="statements"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public CodeExpression GetReferenceCode(object method, CodeStatementCollection statements, string propertyName, CodeExpression target, bool forValue)
		{
			EPField fld = this[propertyName];
			if (fld != null)
			{
				if (forValue)
				{
					string vname = "ValueString";
					{
						switch (fld.OleDbType)
						{
							case System.Data.OleDb.OleDbType.BigInt:
								vname = "ValueInt64"; break;
							case System.Data.OleDb.OleDbType.Binary:
								vname = "ValueBytes"; break;
							case System.Data.OleDb.OleDbType.Boolean:
								vname = "ValueBool"; break;
							case System.Data.OleDb.OleDbType.BSTR:
								vname = "ValueString"; break;
							case System.Data.OleDb.OleDbType.Char:
								vname = "ValueChar"; break;
							case System.Data.OleDb.OleDbType.Currency:
								vname = "ValueDouble"; break;
							case System.Data.OleDb.OleDbType.Date:
								vname = "ValueDateTime"; break;
							case System.Data.OleDb.OleDbType.DBDate:
								vname = "ValueDateTime"; break;
							case System.Data.OleDb.OleDbType.DBTime:
								vname = "ValueTime"; break;
							case System.Data.OleDb.OleDbType.DBTimeStamp:
								vname = "ValueDateTime"; break;
							case System.Data.OleDb.OleDbType.Decimal:
								vname = "ValueDouble"; break;
							case System.Data.OleDb.OleDbType.Double:
								vname = "ValueDouble"; break;
							case System.Data.OleDb.OleDbType.Filetime:
								vname = "ValueDateTime"; break;
							case System.Data.OleDb.OleDbType.Integer:
								vname = "ValueInt32"; break;
							case System.Data.OleDb.OleDbType.LongVarBinary:
								vname = "ValueBytes"; break;
							case System.Data.OleDb.OleDbType.LongVarChar:
								vname = "ValueString"; break;
							case System.Data.OleDb.OleDbType.LongVarWChar:
								vname = "ValueString"; break;
							case System.Data.OleDb.OleDbType.Numeric:
								vname = "ValueDouble"; break;
							case System.Data.OleDb.OleDbType.Single:
								vname = "ValueFloat"; break;
							case System.Data.OleDb.OleDbType.SmallInt:
								vname = "ValueInt16"; break;
							case System.Data.OleDb.OleDbType.TinyInt:
								vname = "ValueSByte"; break;
							case System.Data.OleDb.OleDbType.UnsignedBigInt:
								vname = "ValueUInt64"; break;
							case System.Data.OleDb.OleDbType.UnsignedInt:
								vname = "ValueUInt32"; break;
							case System.Data.OleDb.OleDbType.UnsignedSmallInt:
								vname = "ValueUInt16"; break;
							case System.Data.OleDb.OleDbType.UnsignedTinyInt:
								vname = "ValueByte"; break;
							case System.Data.OleDb.OleDbType.VarBinary:
								vname = "ValueBytes"; break;
							case System.Data.OleDb.OleDbType.VarChar:
								vname = "ValueString"; break;
							case System.Data.OleDb.OleDbType.VarWChar:
								vname = "ValueString"; break;
							case System.Data.OleDb.OleDbType.WChar:
								vname = "ValueString"; break;
							default:
								vname = "ValueString"; break;
						}
					}
					return new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), vname);
				}
				else
				{
					return new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), "Value");
				}
			}
			else
			{
				return target;
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.columnValue('{0}','{1}')", refCode, propertyName);

		}
		public string GetPhpScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			EPField fld = this[propertyName];
			if (fld != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", refCode, propertyName);
			}
			else
			{
				return refCode;
			}
		}
		public Type PropertyCodeType(string propertyName)
		{
			EPField fld = this[propertyName];
			if (fld != null)
			{
				return EPField.ToSystemType(fld.OleDbType);
			}
			return null;
		}
		#endregion

		#region IFieldList Members


		public string GetFieldname(int i)
		{
			return this[i].Name;
		}

		#endregion

		#region IDatabaseFieldProvider Members

		public CodeExpression GetIsNullCheck(object method, CodeStatementCollection statements, string propertyName, CodeExpression target)
		{
			return new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), "IsNull");
		}
		public CodeExpression GetIsNotNullCheck(object method, CodeStatementCollection statements, string propertyName, CodeExpression target)
		{
			return new CodePropertyReferenceExpression(new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName)), "IsNotNull");
		}
		#endregion
	}
	public class ParameterList : FieldList
	{
		public ParameterList()
		{
		}
		public static string GetParameterName(EnumParameterStyle paramStyle, string paramName)
		{
			switch (paramStyle)
			{
				case EnumParameterStyle.LeadingQuestionMark:
					if (paramName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
					{
						paramName = string.Format(CultureInfo.InvariantCulture, "?{0}", paramName.Substring(1));
					}
					else if (!paramName.StartsWith("?", StringComparison.OrdinalIgnoreCase))
					{
						paramName = string.Format(CultureInfo.InvariantCulture, "?{0}", paramName);
					}
					break;
				case EnumParameterStyle.LeadingAt:
					if (!paramName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
					{
						paramName = string.Format(CultureInfo.InvariantCulture, "@{0}", paramName);
					}
					break;
			}
			return paramName;
		}
	}
}
