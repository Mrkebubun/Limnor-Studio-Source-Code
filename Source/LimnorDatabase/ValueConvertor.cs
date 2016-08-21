/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data.OleDb;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;

namespace LimnorDatabase
{
	/// <summary>
	/// data conversion utility
	/// </summary>
	public abstract class ValueConvertor
	{
		static public bool IsDateTime(DbType t)
		{
			switch (t)
			{
				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return true;
				default:
					return false;
			}
		}
		static public string DbTypeToPhpMySqlType(DbType t)
		{
			switch (t)
			{
				case DbType.Binary:
					return "b";
				case DbType.Byte:
				case DbType.Boolean:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return "i";
				case DbType.VarNumeric:
				case DbType.Currency:
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Single:
					return "d";
				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				default:
					return "s";
			}
		}
		static public string OleDbTypeToPhpMySqlType(System.Data.OleDb.OleDbType oleDbType)
		{
			switch (oleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return "i";
				case System.Data.OleDb.OleDbType.Binary:
					return "b";
				case System.Data.OleDb.OleDbType.Boolean:
					return "i";
				case System.Data.OleDb.OleDbType.BSTR:
					return "s";
				case System.Data.OleDb.OleDbType.Char:
					return "s";
				case System.Data.OleDb.OleDbType.Currency:
					return "d";
				case System.Data.OleDb.OleDbType.Date:
					return "s";
				case System.Data.OleDb.OleDbType.DBDate:
					return "s";
				case System.Data.OleDb.OleDbType.DBTime:
					return "s";
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return "s";
				case System.Data.OleDb.OleDbType.Decimal:
					return "d";
				case System.Data.OleDb.OleDbType.Double:
					return "d";
				case System.Data.OleDb.OleDbType.Empty:
					return "s";
				case System.Data.OleDb.OleDbType.Filetime:
					return "s";
				case System.Data.OleDb.OleDbType.Guid:
					return "s";
				case System.Data.OleDb.OleDbType.IDispatch:
					return "b";
				case System.Data.OleDb.OleDbType.Integer:
					return "i";
				case System.Data.OleDb.OleDbType.IUnknown:
					return "b";
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return "b";
				case System.Data.OleDb.OleDbType.LongVarChar:
					return "s";
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return "s";
				case System.Data.OleDb.OleDbType.Numeric:
					return "d";
				case System.Data.OleDb.OleDbType.PropVariant:
					return "b";
				case System.Data.OleDb.OleDbType.Single:
					return "d";
				case System.Data.OleDb.OleDbType.SmallInt:
					return "i";
				case System.Data.OleDb.OleDbType.TinyInt:
					return "i";
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return "i";
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return "i";
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return "i";
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return "i";
				case System.Data.OleDb.OleDbType.VarBinary:
					return "b";
				case System.Data.OleDb.OleDbType.VarChar:
					return "s";
				case System.Data.OleDb.OleDbType.Variant:
					return "s";
				case System.Data.OleDb.OleDbType.VarNumeric:
					return "d";
				case System.Data.OleDb.OleDbType.VarWChar:
					return "s";
				case System.Data.OleDb.OleDbType.WChar:
					return "s";
				default:
					return "s";
			}
		}
		static public System.Data.DbType OleDbTypeToDbType(System.Data.OleDb.OleDbType oleDbType)
		{
			switch (oleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return System.Data.DbType.Int64;
				case System.Data.OleDb.OleDbType.Binary:
					return System.Data.DbType.Binary;
				case System.Data.OleDb.OleDbType.Boolean:
					return System.Data.DbType.Boolean;
				case System.Data.OleDb.OleDbType.BSTR:
					return System.Data.DbType.String;
				case System.Data.OleDb.OleDbType.Char:
					return System.Data.DbType.StringFixedLength;
				case System.Data.OleDb.OleDbType.Currency:
					return System.Data.DbType.Currency;
				case System.Data.OleDb.OleDbType.Date:
					return System.Data.DbType.Date;
				case System.Data.OleDb.OleDbType.DBDate:
					return System.Data.DbType.DateTime;
				case System.Data.OleDb.OleDbType.DBTime:
					return System.Data.DbType.Time;
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return System.Data.DbType.DateTime;
				case System.Data.OleDb.OleDbType.Decimal:
					return System.Data.DbType.Decimal;
				case System.Data.OleDb.OleDbType.Double:
					return System.Data.DbType.Double;
				case System.Data.OleDb.OleDbType.Empty:
					return System.Data.DbType.AnsiString;
				case System.Data.OleDb.OleDbType.Filetime:
					return System.Data.DbType.DateTime;
				case System.Data.OleDb.OleDbType.Guid:
					return System.Data.DbType.Guid;
				case System.Data.OleDb.OleDbType.IDispatch:
					return System.Data.DbType.Object;
				case System.Data.OleDb.OleDbType.Integer:
					return System.Data.DbType.Int32;
				case System.Data.OleDb.OleDbType.IUnknown:
					return System.Data.DbType.Object;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return System.Data.DbType.Binary;
				case System.Data.OleDb.OleDbType.LongVarChar:
					return System.Data.DbType.String;
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return System.Data.DbType.String;
				case System.Data.OleDb.OleDbType.Numeric:
					return System.Data.DbType.Decimal;
				case System.Data.OleDb.OleDbType.PropVariant:
					return System.Data.DbType.Object;
				case System.Data.OleDb.OleDbType.Single:
					return System.Data.DbType.Single;
				case System.Data.OleDb.OleDbType.SmallInt:
					return System.Data.DbType.Int16;
				case System.Data.OleDb.OleDbType.TinyInt:
					return System.Data.DbType.SByte;
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return System.Data.DbType.UInt64;
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return System.Data.DbType.UInt32;
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return System.Data.DbType.UInt16;
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return System.Data.DbType.Byte;
				case System.Data.OleDb.OleDbType.VarBinary:
					return System.Data.DbType.Binary;
				case System.Data.OleDb.OleDbType.VarChar:
					return System.Data.DbType.String;
				case System.Data.OleDb.OleDbType.Variant:
					return System.Data.DbType.Object;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return System.Data.DbType.Decimal;
				case System.Data.OleDb.OleDbType.VarWChar:
					return System.Data.DbType.String;
				case System.Data.OleDb.OleDbType.WChar:
					return System.Data.DbType.String;
				default:
					return System.Data.DbType.String;
			}
		}
		static public object ConvertByOleDbType(object v, OleDbType oleDbType)
		{
			if (v == null || v == DBNull.Value)
			{
				return DBNull.Value;
			}
			switch (oleDbType)
			{
				case System.Data.OleDb.OleDbType.BigInt:
					return ToLong(v);
				case System.Data.OleDb.OleDbType.Binary:
					return ToBytes(v);
				case System.Data.OleDb.OleDbType.Boolean:
					return ToBool(v);
				case System.Data.OleDb.OleDbType.BSTR:
					return ToString(v);
				case System.Data.OleDb.OleDbType.Char:
					return ToChar(v);
				case System.Data.OleDb.OleDbType.Currency:
					return ToDecimal(v);
				case System.Data.OleDb.OleDbType.Date:
					return ToDateTime(v);
				case System.Data.OleDb.OleDbType.DBDate:
					return ToDateTime(v);
				case System.Data.OleDb.OleDbType.DBTime:
					return ToTime(v);
				case System.Data.OleDb.OleDbType.DBTimeStamp:
					return ToDateTime(v);
				case System.Data.OleDb.OleDbType.Decimal:
					return ToDecimal(v);
				case System.Data.OleDb.OleDbType.Double:
					return ToDouble(v);
				case System.Data.OleDb.OleDbType.Empty:
					return EPField.DefaultFieldValue(oleDbType);
				case System.Data.OleDb.OleDbType.Filetime:
					return ToDateTime(v);
				case System.Data.OleDb.OleDbType.Guid:
					return ToGuid(v);
				case System.Data.OleDb.OleDbType.IDispatch:
					return v;
				case System.Data.OleDb.OleDbType.Integer:
					return ToInt(v);
				case System.Data.OleDb.OleDbType.IUnknown:
					return v;
				case System.Data.OleDb.OleDbType.LongVarBinary:
					return ToBytes(v);
				case System.Data.OleDb.OleDbType.LongVarChar:
					return ToString(v);
				case System.Data.OleDb.OleDbType.LongVarWChar:
					return ToString(v);
				case System.Data.OleDb.OleDbType.Numeric:
					return ToDecimal(v);
				case System.Data.OleDb.OleDbType.PropVariant:
					return v;
				case System.Data.OleDb.OleDbType.Single:
					return ToFloat(v);
				case System.Data.OleDb.OleDbType.SmallInt:
					return ToInt16(v);
				case System.Data.OleDb.OleDbType.TinyInt:
					return ToSByte(v);
				case System.Data.OleDb.OleDbType.UnsignedBigInt:
					return ToU64(v);
				case System.Data.OleDb.OleDbType.UnsignedInt:
					return ToU32(v);
				case System.Data.OleDb.OleDbType.UnsignedSmallInt:
					return ToU16(v);
				case System.Data.OleDb.OleDbType.UnsignedTinyInt:
					return ToByte(v);
				case System.Data.OleDb.OleDbType.VarBinary:
					return ToBytes(v);
				case System.Data.OleDb.OleDbType.VarChar:
					return ToString(v);
				case System.Data.OleDb.OleDbType.Variant:
					return v;
				case System.Data.OleDb.OleDbType.VarNumeric:
					return ToDecimal(v);
				case System.Data.OleDb.OleDbType.VarWChar:
					return ToString(v);
				case System.Data.OleDb.OleDbType.WChar:
					return ToString(v);
				default:
					return ToString(v);
			}
		}
		static public object ToObject(object v, Type target)
		{
			switch (Type.GetTypeCode(target))
			{
				case TypeCode.Boolean:
					return ToBool(v);
				case TypeCode.Byte:
					return ToByte(v);
				case TypeCode.Char:
					return ToChar(v);
				case TypeCode.DateTime:
					return ToDateTime(v);
				case TypeCode.Decimal:
					return ToDecimal(v);
				case TypeCode.Double:
					return ToDouble(v);
				case TypeCode.Int16:
					return ToInt16(v);
				case TypeCode.Int32:
					return ToInt(v);
				case TypeCode.Int64:
					return ToLong(v);
				case TypeCode.SByte:
					return ToSByte(v);
				case TypeCode.Single:
					return ToFloat(v);
				case TypeCode.String:
					return ToString(v);
				case TypeCode.UInt16:
					return ToU16(v);
				case TypeCode.UInt32:
					return ToU32(v);
				case TypeCode.UInt64:
					return ToU64(v);
			}
			return v;
		}
		static public byte[] ToBytes(object v)
		{
			byte[] bs = null;
			if (v != null)
			{
				Type t = v.GetType();
				if (typeof(byte[]).Equals(t))
				{
					bs = (byte[])v;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(v);
					if (converter != null && converter.CanConvertTo(typeof(byte[])))
					{
						bs = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, v, typeof(byte[]));
					}
					else
					{
						if (t.IsSerializable)
						{
							BinaryFormatter formatter = new BinaryFormatter();
							MemoryStream stream = new MemoryStream();

							formatter.Serialize(stream, v);
							bs = stream.ToArray();
						}
						else
						{
							try
							{
								string s = v.ToString();
								char[] cs = s.ToCharArray();
								bs = new byte[cs.Length];
								for (int i = 0; i < cs.Length; i++)
									bs[i] = Convert.ToByte(cs[i]);
							}
							catch
							{
							}
						}
					}
				}
			}
			return bs;
		}
		static public byte ToByte(object v)
		{
			byte u = 0;
			try
			{
				u = Convert.ToByte(v);
			}
			catch
			{
			}
			return u;
		}
		static public sbyte ToSByte(object v)
		{
			sbyte u = 0;
			try
			{
				u = Convert.ToSByte(v);
			}
			catch
			{
			}
			return u;
		}
		static public System.UInt16 ToU16(object v)
		{
			System.UInt16 u = 0;
			try
			{
				u = Convert.ToUInt16(v);
			}
			catch
			{
			}
			return u;
		}
		static public System.UInt32 ToU32(object v)
		{
			System.UInt32 u = 0;
			try
			{
				u = Convert.ToUInt32(v);
			}
			catch
			{
			}
			return u;
		}
		static public System.UInt64 ToU64(object v)
		{
			System.UInt64 u = 0;
			try
			{
				u = Convert.ToUInt64(v);
			}
			catch
			{
			}
			return u;
		}
		static public TimeSpan ToTime(object v)
		{
			if (v != DBNull.Value && v != null)
			{
				string s = v as string;
				if (!string.IsNullOrEmpty(s))
				{
					string[] ss = s.Split(':');
					if (ss.Length > 1)
					{
						int h = 0, m = 0, se = 0;
						int h0;
						if (int.TryParse(ss[0], out h0))
						{
							h = h0;
						}
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
						return new TimeSpan(h, m, se);
					}
					long tk;
					if (long.TryParse(s, out tk))
					{
						return new TimeSpan(tk);
					}
				}
				else
				{
					try
					{
						long tk = Convert.ToInt64(v);
						return new TimeSpan(tk);
					}
					catch
					{
					}
				}
			}
			return new TimeSpan(0, 0, 0);
		}
		static public System.DateTime ToDateTime(object v)
		{
			if (v == null || v == DBNull.Value)
				return new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
			TypeCode tc = Type.GetTypeCode(v.GetType());
			if (tc == TypeCode.DateTime)
			{
				return (DateTime)v;
			}
			try
			{
				System.DateTime dt = Convert.ToDateTime(v);
				return dt;
			}
			catch
			{
				return new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
			}
		}
		static public Guid ToGuid(object v)
		{
			if (v == null)
			{
				return Guid.Empty;
			}
			Type t = v.GetType();
			if (typeof(Guid).Equals(t))
			{
				return (Guid)v;
			}
			try
			{
				Guid g = new Guid(v.ToString());
				return g;
			}
			catch
			{
			}
			return Guid.Empty;
		}
		static public char ToChar(object v)
		{
			if (v == null)
				return '\0';
			System.TypeCode tp = Convert.GetTypeCode(v);
			if (tp == System.TypeCode.Char)
				return (char)v;
			if (tp == System.TypeCode.Byte)
				return (char)(byte)v;
			if (tp == System.TypeCode.Boolean)
			{
				if ((bool)v)
					return 'Y';
				else
					return 'N';
			}
			if (tp == System.TypeCode.Int16)
			{
				System.Int16 n16 = (System.Int16)v;
				return (char)n16;
			}
			if (tp == System.TypeCode.String)
			{
				string s = v.ToString();
				if (s.Length == 0)
					return '\0';
				else
					return s[0];
			}
			try
			{
				int n = (int)v;
				return (char)n;
			}
			catch
			{
			}
			return '\0';
		}
		static public bool ToBool(object v)
		{
			if (v == null)
				return false;
			System.TypeCode tp = Convert.GetTypeCode(v);
			if (tp == System.TypeCode.Boolean)
			{
				return (bool)v;
			}
			else if (tp == System.TypeCode.String)
			{
				string s = (string)v;
				s = s.Trim();
				if (s.Length == 0)
					return false;
				try
				{
					System.Int64 n = Convert.ToInt64(s);
					return (n != 0);
				}
				catch
				{
				}
				try
				{
					double n = Convert.ToDouble(s);
					return (n != 0);
				}
				catch
				{
				}
				if (string.Compare(s, "no", StringComparison.OrdinalIgnoreCase) == 0) //yes
					return false;
				else if (string.Compare(s, "false", StringComparison.OrdinalIgnoreCase) == 0) //true
					return false;
				else if (string.Compare(s, "off", StringComparison.OrdinalIgnoreCase) == 0) //on
					return false;
				return true;
			}
			else
			{
				try
				{
					bool b = Convert.ToBoolean(v);
					return b;
				}
				catch
				{
				}
			}
			try
			{
				int n = Convert.ToInt32(v);
				if (n != 0)
					return true;
			}
			catch
			{
			}
			return false;
		}
		static public System.Drawing.Color ToColor(object v)
		{
			if (v == null)
				return System.Drawing.Color.Black;
			System.Drawing.Color c = System.Drawing.Color.Black;

			System.Type tp = v.GetType();
			if (tp.Equals(typeof(Color)))
			{
				c = (System.Drawing.Color)v;
			}
			else
			{
				try
				{
					int n = Convert.ToInt32(v);
					c = System.Drawing.Color.FromArgb(n);
				}
				catch
				{
					string s = ValueConvertor.ToString(v);
					s = s.Trim();
					if (s.StartsWith("COLOR", StringComparison.OrdinalIgnoreCase))
					{
						s = s.Substring(5);
						s = s.Trim();
						if (s.Length > 2)
						{
							if (s[0] == '[')
							{
								s = s.Substring(1, s.Length - 2);
							}
						}
					}
					if (s.StartsWith("A=", StringComparison.OrdinalIgnoreCase))
					{
						bool bOK = false;
						s = s.Substring(2);
						int pos = s.IndexOf(',');
						if (pos > 0)
						{
							try
							{
								int A = Convert.ToInt32(s.Substring(0, pos));
								s = s.Substring(pos + 1);
								s = s.Trim();
								if (s.StartsWith("R=", StringComparison.OrdinalIgnoreCase))
								{
									s = s.Substring(2);
									pos = s.IndexOf(',');
									if (pos > 0)
									{
										int R = Convert.ToInt32(s.Substring(0, pos));
										s = s.Substring(pos + 1);
										s = s.Trim();
										if (s.StartsWith("G=", StringComparison.OrdinalIgnoreCase))
										{
											s = s.Substring(2);
											pos = s.IndexOf(',');
											if (pos > 0)
											{
												int G = Convert.ToInt32(s.Substring(0, pos));
												s = s.Substring(pos + 1);
												s = s.Trim();
												if (s.StartsWith("B=", StringComparison.OrdinalIgnoreCase))
												{
													s = s.Substring(2);
													int B = Convert.ToInt32(s);
													c = System.Drawing.Color.FromArgb(A, R, G, B);
													bOK = true;
												}
											}
										}
									}
								}
							}
							catch
							{
								c = System.Drawing.Color.Black;
							}
						}
						if (!bOK)
						{
							c = System.Drawing.Color.Black;
						}
					}
					else
					{
						try
						{
							c = System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)Enum.Parse(typeof(System.Drawing.KnownColor), s, true));
						}
						catch
						{
							c = System.Drawing.Color.Black;
						}
					}
				}
			}
			return c;
		}
		static public double ToDouble(object v)
		{
			double d = 0;
			if (v != null)
			{
				try
				{
					d = Convert.ToDouble(v);
				}
				catch
				{
				}
			}
			return d;
		}
		static public Decimal ToDecimal(object v)
		{
			Decimal d = 0;
			if (v != null)
			{
				try
				{
					d = Convert.ToDecimal(v);
				}
				catch
				{
				}
			}
			return d;
		}
		static public float ToFloat(object v)
		{
			float d = 0;
			if (v != null)
			{
				try
				{
					d = Convert.ToSingle(v);
				}
				catch
				{
				}
			}
			return d;
		}
		static public int ToInt(object v)
		{
			int d = 0;
			if (v != null)
			{
				System.TypeCode tp = Convert.GetTypeCode(v);
				if (tp == System.TypeCode.Int32)
					return (System.Int32)v;
				else if (v is System.IntPtr)
				{
					return ((System.IntPtr)v).ToInt32();
				}
				else
				{
					try
					{
						d = Convert.ToInt32(v);
					}
					catch
					{
					}
				}
			}
			return d;
		}
		static public System.Int16 ToInt16(object v)
		{
			System.Int16 d = 0;
			if (v != null)
			{
				System.TypeCode tp = Convert.GetTypeCode(v);
				if (tp == System.TypeCode.Int16)
					return (System.Int16)v;
				else
				{
					try
					{
						d = Convert.ToInt16(v);
					}
					catch
					{
					}
				}
			}
			return d;
		}
		static public long ToLong(object v)
		{
			long d = 0;
			if (v != null)
			{
				System.TypeCode tp = Convert.GetTypeCode(v);
				if (tp == System.TypeCode.Int64)
					return (System.Int64)v;
				else if (v is System.IntPtr)
				{
					return ((System.IntPtr)v).ToInt64();
				}
				else
				{
					try
					{
						d = Convert.ToInt64(v);
					}
					catch
					{
					}
				}
			}
			return d;
		}
		static public string ToString(object v)
		{
			if (v == null)
				return "";
			if (v == System.DBNull.Value)
				return "";
			return v.ToString();
		}
	}
}
