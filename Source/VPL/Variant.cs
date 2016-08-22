/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace VPL
{
	/// <summary>
	/// use space to gain speed
	/// </summary>
	public class Variant
	{
		public Variant()
		{
			TypeCode = TypeCode.Int32;
		}
		public Variant(TypeCode type, object value)
		{
			TypeCode = type;
			SetValue(value);
		}
		public Variant(object value)
		{
			if (value == null)
			{
				TypeCode = TypeCode.Object;
			}
			else
			{
				TypeCode = Type.GetTypeCode(value.GetType());
				SetValue(value);
			}
		}

		public bool IsEqual(object v)
		{
			if (v == null)
			{
				return false;
			}
			Variant var = new Variant(this.TypeCode, v);
			return IsEqual(var);
		}
		public bool IsEqual(Variant var)
		{
			switch (this.TypeCode)
			{
				case TypeCode.Boolean:
					return this.ValueBool == var.ValueBool;
				case TypeCode.Byte:
					return this.ValueByte == var.ValueByte;
				case TypeCode.Char:
					return this.ValueChar == var.ValueChar;
				case TypeCode.DateTime:
					return this.ValueDateTime == var.ValueDateTime;
				case TypeCode.Decimal:
					return this.ValueDecimal == var.ValueDecimal;
				case TypeCode.Double:
					return this.ValueDouble == var.ValueDouble;
				case TypeCode.Int16:
					return this.ValueInt16 == var.ValueInt16;
				case TypeCode.Int32:
					return this.ValueInt32 == var.ValueInt32;
				case TypeCode.Int64:
					return this.ValueInt64 == var.ValueInt64;
				case TypeCode.SByte:
					return this.ValueSByte == var.ValueSByte;
				case TypeCode.Single:
					return this.ValueSingle == var.ValueSingle;
				case TypeCode.String:
					return string.CompareOrdinal(this.ValueString, var.ValueString) == 0;
				case TypeCode.UInt16:
					return this.ValueUInt16 == var.ValueUInt16;
				case TypeCode.UInt32:
					return this.ValueUInt32 == var.ValueUInt32;
				case TypeCode.UInt64:
					return this.ValueUInt64 == var.ValueUInt64;
			}
			return false;
		}

		public void SetValue(object v)
		{
			try
			{
				switch (this.TypeCode)
				{
					case TypeCode.Boolean:
						ValueBool = Convert.ToBoolean(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Byte:
						ValueByte = Convert.ToByte(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Char:
						ValueChar = Convert.ToChar(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.DateTime:
						ValueDateTime = Convert.ToDateTime(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Decimal:
						ValueDecimal = Convert.ToDecimal(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Double:
						ValueDouble = Convert.ToDouble(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Int16:
						ValueInt16 = Convert.ToInt16(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Int32:
						ValueInt32 = Convert.ToInt32(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Int64:
						ValueInt64 = Convert.ToInt64(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.SByte:
						ValueSByte = Convert.ToSByte(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.Single:
						ValueSingle = Convert.ToSingle(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.String:
						ValueString = Convert.ToString(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.UInt16:
						ValueUInt16 = Convert.ToUInt16(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.UInt32:
						ValueUInt32 = Convert.ToUInt32(v, CultureInfo.InvariantCulture);
						break;
					case TypeCode.UInt64:
						ValueUInt64 = Convert.ToUInt64(v, CultureInfo.InvariantCulture);
						break;
					default:
						ValueObject = v;
						break;
				}
			}
			catch (Exception e)
			{
				throw new Exception(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Cannot convert {0} to {1}", v, this.TypeCode), e);
			}
		}
		private TypeCode _tcode;
		public TypeCode TypeCode { get { return _tcode; } set { _tcode = value; } }
		//
		private object _vo;
		public object ValueObject { get { return _vo; } set { _vo = value; } }
		private bool _vb;
		public bool ValueBool { get { return _vb; } set { _vb = value; } }
		private byte _vby;
		public byte ValueByte { get { return _vby; } set { _vby = value; } }
		private char _vc;
		public char ValueChar { get { return _vc; } set { _vc = value; } }
		private DateTime _vd;
		public DateTime ValueDateTime { get { return _vd; } set { _vd = value; } }
		private decimal _vdec;
		public decimal ValueDecimal { get { return _vdec; } set { _vdec = value; } }
		private double _vdbl;
		public double ValueDouble { get { return _vdbl; } set { _vdbl = value; } }
		private Int16 _vi16;
		public Int16 ValueInt16 { get { return _vi16; } set { _vi16 = value; } }
		private Int32 _vi32;
		public Int32 ValueInt32 { get { return _vi32; } set { _vi32 = value; } }
		private Int64 _vi64;
		public Int64 ValueInt64 { get { return _vi64; } set { _vi64 = value; } }
		private sbyte _vsb;
		public sbyte ValueSByte { get { return _vsb; } set { _vsb = value; } }
		private Single _vsgl;
		public Single ValueSingle { get { return _vsgl; } set { _vsgl = value; } }
		private string _vstr;
		public string ValueString { get { return _vstr; } set { _vstr = value; } }
		private UInt16 _vu16;
		public UInt16 ValueUInt16 { get { return _vu16; } set { _vu16 = value; } }
		private UInt32 _vu32;
		public UInt32 ValueUInt32 { get { return _vu32; } set { _vu32 = value; } }
		private UInt64 _vu64;
		public UInt64 ValueUInt64 { get { return _vu64; } set { _vu64 = value; } }
	}
}
