/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.ComponentModel;
using System.Globalization;
using VPL;

namespace MathExp
{
	public class TypeDesc
	{
		private Type _type;
		private string _desc;
		public TypeDesc(Type type, string description)
		{
			_type = type;
			_desc = description;
		}
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		public override string ToString()
		{
			return _desc;
		}
	}
	public class ValueTypeUtil
	{
		private ValueTypeUtil()
		{
		}
		public static Type GetTypeByTypeCode(TypeCode tc)
		{
			switch (tc)
			{
				case TypeCode.Boolean:
					return typeof(bool);
				case TypeCode.Byte:
					return typeof(byte);
				case TypeCode.Char:
					return typeof(char);
				case TypeCode.DateTime:
					return typeof(DateTime);
				case TypeCode.Decimal:
					return typeof(Decimal);
				case TypeCode.Double:
					return typeof(double);
				case TypeCode.Int16:
					return typeof(Int16);
				case TypeCode.Int32:
					return typeof(Int32);
				case TypeCode.Int64:
					return typeof(Int64);
				case TypeCode.SByte:
					return typeof(sbyte);
				case TypeCode.Single:
					return typeof(Single);
				case TypeCode.String:
					return typeof(string);
				case TypeCode.UInt16:
					return typeof(UInt16);
				case TypeCode.UInt32:
					return typeof(UInt32);
				case TypeCode.UInt64:
					return typeof(UInt64);
				default:
					return typeof(double);
			}
		}
		public static string GetTypeNameByTypeCode(TypeCode tc)
		{
			switch (tc)
			{
				case TypeCode.Boolean:
					return "bool";
				case TypeCode.Byte:
					return "byte";
				case TypeCode.Char:
					return "char";
				case TypeCode.DateTime:
					return "DateTime";
				case TypeCode.Decimal:
					return "decimal";
				case TypeCode.Double:
					return "double";
				case TypeCode.Int16:
					return "Int16";
				case TypeCode.Int32:
					return "Int32";
				case TypeCode.Int64:
					return "Int64";
				case TypeCode.SByte:
					return "sbyte";
				case TypeCode.Single:
					return "Single";
				case TypeCode.String:
					return "string";
				case TypeCode.UInt16:
					return "UInt16";
				case TypeCode.UInt32:
					return "UInt32";
				case TypeCode.UInt64:
					return "UInt64";
				default:
					return "double";
			}
		}
		public static string GetDefaultJavaScriptValueByType(Type t)
		{
			if (t.IsEnum)
			{
				Array a = Enum.GetValues(t);
				return string.Format(CultureInfo.InvariantCulture,
					"{0}.{1}", t.FullName, a.GetValue(0).ToString());
			}
			else if (t.IsValueType)
			{
				return GetDefaultJavaScriptCodeByType(t);
			}
			return "null";
		}
		public static string GetPhpScriptValue(object o)
		{
			if (o == null || o == System.DBNull.Value)
			{
				return "NULL";
			}
			string s = o as string;
			if (s != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "'{0}'", s.Replace("'", "\\'"));
			}
			Type t = o.GetType();
			if (t.Equals(typeof(DateTime)))
			{
				DateTime dt = (DateTime)o;
				return string.Format(CultureInfo.InvariantCulture,
					"DateTime::createFromFormat('Y-m-d H:i:s', '{0}')", dt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			}
			if (t.Equals(typeof(bool)))
			{
				bool b = (bool)o;
				if (b)
				{
					return "true";
				}
				else
				{
					return "false";
				}
			}
			return o.ToString();
		}
		public static string GetDefaultPhpScriptValueByType(Type t)
		{
			if (t.IsEnum)
			{
				Array a = Enum.GetValues(t);
				return string.Format(CultureInfo.InvariantCulture,
					"{0}::{1}", t.FullName, a.GetValue(0).ToString());
			}
			else if (t.IsValueType)
			{
				return GetDefaultPhpScriptCodeByType(t);
			}
			return "NULL";
		}
		public static CodeExpression GetDefaultValueByType(Type t)
		{
			if (t.IsEnum)
			{
				Array a = Enum.GetValues(t);
				return new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(t), a.GetValue(0).ToString());
			}
			else if (t.IsValueType)
			{
				return GetDefaultCodeByType(t);
			}
			return new CodePrimitiveExpression(null);
		}
		public static CodeExpression GetDefaultCodeByType(Type t)
		{
			if (t.Equals(typeof(void)))
				return new CodePrimitiveExpression(null);

			TypeCode tc = Type.GetTypeCode(t);
			switch (tc)
			{
				case TypeCode.Boolean:
					return new CodePrimitiveExpression(false);
				case TypeCode.Byte:
					return new CodePrimitiveExpression(default(byte));
				case TypeCode.Char:
					return new CodePrimitiveExpression(default(char));
				case TypeCode.DateTime:
					return ObjectCreationCodeGen.ObjectCreationCode(default(DateTime));
				case TypeCode.Decimal:
					return new CodePrimitiveExpression((decimal)0);
				case TypeCode.Double:
					return new CodePrimitiveExpression(default(double));
				case TypeCode.Int16:
					return new CodePrimitiveExpression(default(Int16));
				case TypeCode.Int32:
					if (t.IsEnum)
					{
						Array a = Enum.GetValues(t);
						return new CodeFieldReferenceExpression(
							new CodeTypeReferenceExpression(t), a.GetValue(0).ToString());
					}
					return new CodePrimitiveExpression(default(Int32));
				case TypeCode.Int64:
					return new CodePrimitiveExpression(default(Int64));
				case TypeCode.SByte:
					return new CodePrimitiveExpression(default(sbyte));
				case TypeCode.Single:
					return new CodePrimitiveExpression(default(Single));
				case TypeCode.String:
					return new CodePrimitiveExpression("");
				case TypeCode.UInt16:
					return new CodePrimitiveExpression(default(UInt16));
				case TypeCode.UInt32:
					return new CodePrimitiveExpression(default(UInt32));
				case TypeCode.UInt64:
					return new CodePrimitiveExpression(default(UInt64));
				case TypeCode.Object:
					return ObjectCreationCodeGen.ObjectCreationCode(VPLUtil.GetDefaultValue(t));
				case TypeCode.DBNull:
					return new CodePrimitiveExpression(null);
				case TypeCode.Empty:
					return new CodePrimitiveExpression(null);
				default:
					return new CodeDefaultValueExpression(new CodeTypeReference(t));
			}
		}
		public static string GetDefaultJavaScriptCodeByType(Type t)
		{
			if (t.Equals(typeof(void)))
				return "null";

			TypeCode tc = Type.GetTypeCode(t);
			switch (tc)
			{
				case TypeCode.Boolean:
					return "false";
				case TypeCode.Byte:
					return "0";
				case TypeCode.Char:
					return "";
				case TypeCode.DateTime:
					return string.Format(CultureInfo.InvariantCulture, "'{0}'", VPL.VPLUtil.DateTimeToString(DateTime.MinValue));
				case TypeCode.Decimal:
					return "0.0";
				case TypeCode.Double:
					return "0.0";
				case TypeCode.Int16:
					return "0";
				case TypeCode.Int32:
					if (t.IsEnum)
					{
						Array a = Enum.GetValues(t);
						return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
							t.FullName, a.GetValue(0).ToString());
					}
					return "0";
				case TypeCode.Int64:
					return "0";
				case TypeCode.SByte:
					return "0";
				case TypeCode.Single:
					return "0.0";
				case TypeCode.String:
					return "''";
				case TypeCode.UInt16:
					return "0";
				case TypeCode.UInt32:
					return "0";
				case TypeCode.UInt64:
					return "0";
				case TypeCode.Object:
					return "null";
				case TypeCode.DBNull:
					return "null";
				case TypeCode.Empty:
					return "null";
				default:
					return "null";
			}
		}
		public static string GetDefaultPhpScriptCodeByType(Type t)
		{
			if (t.Equals(typeof(void)))
				return "NULL";

			object[] ptas = t.GetCustomAttributes(typeof(PhpTypeAttribute), true);
			if (ptas != null && ptas.Length > 0)
			{
				PhpTypeAttribute pta = ptas[0] as PhpTypeAttribute;
				if (pta != null)
				{
					return pta.DefaultValue;
				}
			}
			TypeCode tc = Type.GetTypeCode(t);
			switch (tc)
			{
				case TypeCode.Boolean:
					return "false";
				case TypeCode.Byte:
					return "0";
				case TypeCode.Char:
					return "";
				case TypeCode.DateTime:
					return string.Format(CultureInfo.InvariantCulture, "'{0}'", VPL.VPLUtil.DateTimeToString(DateTime.MinValue));
				case TypeCode.Decimal:
					return "0.0";
				case TypeCode.Double:
					return "0.0";
				case TypeCode.Int16:
					return "0";
				case TypeCode.Int32:
					if (t.IsEnum)
					{
						Array a = Enum.GetValues(t);
						return string.Format(CultureInfo.InvariantCulture, "{0}::{1}",
							t.FullName, a.GetValue(0).ToString());
					}
					return "0";
				case TypeCode.Int64:
					return "0";
				case TypeCode.SByte:
					return "0";
				case TypeCode.Single:
					return "0.0";
				case TypeCode.String:
					return "''";
				case TypeCode.UInt16:
					return "0";
				case TypeCode.UInt32:
					return "0";
				case TypeCode.UInt64:
					return "0";
				case TypeCode.Object:
					return "NULL";
				case TypeCode.DBNull:
					return "NULL";
				case TypeCode.Empty:
					return "NULL";
				default:
					return "NULL";
			}
		}
		
		public static object ConvertValueByType(Type t, object v)
		{
			bool ok;
			object ret = VPLUtil.ConvertObject(v, t, out ok);
			if (ok)
			{
				return ret;
			}
			throw new Exception(string.Format(CultureInfo.InvariantCulture, "cannot convert {0} to {1}", v, t));
		}
		//
		public static object ConvertValueByTypeCode(TypeCode tc, object v)
		{
			switch (tc)
			{
				case TypeCode.Boolean:
					return Convert.ToBoolean(v);
				case TypeCode.Byte:
					return Convert.ToByte(v);
				case TypeCode.Char:
					return Convert.ToChar(v);
				case TypeCode.DateTime:
					return Convert.ToDateTime(v);
				case TypeCode.Decimal:
					return Convert.ToDecimal(v);
				case TypeCode.Double:
					return Convert.ToDouble(v);
				case TypeCode.Int16:
					return Convert.ToInt16(v);
				case TypeCode.Int32:
					return Convert.ToInt32(v);
				case TypeCode.Int64:
					return Convert.ToInt64(v);
				case TypeCode.SByte:
					return Convert.ToSByte(v);
				case TypeCode.Single:
					return Convert.ToSingle(v);
				case TypeCode.String:
					return Convert.ToString(v);
				case TypeCode.UInt16:
					return Convert.ToUInt16(v);
				case TypeCode.UInt32:
					return Convert.ToUInt32(v);
				case TypeCode.UInt64:
					return Convert.ToUInt64(v);
				default:
					return Convert.ToDouble(v);
			}
		}
		public static object GetDefaultValueByTypeCode(TypeCode tc)
		{
			switch (tc)
			{
				case TypeCode.Boolean:
					return false;
				case TypeCode.Byte:
					return (byte)0;
				case TypeCode.Char:
					return default(char);
				case TypeCode.DateTime:
					return default(DateTime);
				case TypeCode.Decimal:
					return default(Decimal);
				case TypeCode.Double:
					return default(double);
				case TypeCode.Int16:
					return default(Int16);
				case TypeCode.Int32:
					return default(Int32);
				case TypeCode.Int64:
					return default(Int64);
				case TypeCode.SByte:
					return default(sbyte);
				case TypeCode.Single:
					return default(float);
				case TypeCode.String:
					return "";
				case TypeCode.UInt16:
					return default(UInt16);
				case TypeCode.UInt32:
					return default(UInt32);
				case TypeCode.UInt64:
					return default(UInt64);
				case TypeCode.Object:
					return null;
				default:
					return 0;
			}
		}
		public static bool IsNumber(Type t)
		{
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			return false;
		}
		/// <summary>
		/// compare numeric type size
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns>
		/// t1 > t2 : 1 : t2 should be casted to t1
		/// t1 < t2 : -1: t1 should be casted to t2
		/// t1 == t2 : 0
		/// signed should always be casted to unsigned
		/// </returns>
		public static EnumNumTypeCastDir CompareNumberTypes(Type t1, Type t2)
		{
			TypeCode c2 = Type.GetTypeCode(t2);
			switch (Type.GetTypeCode(t1))
			{
				case TypeCode.SByte:
					if (c2 != TypeCode.SByte)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					break;
				case TypeCode.Byte:
					if (c2 == TypeCode.SByte)
					{
						return EnumNumTypeCastDir.Type2ToType1; //t2 should be casted to t1
					}
					if (c2 != TypeCode.Byte)
					{
						return EnumNumTypeCastDir.Type1ToType2; //t1 should be casted to t2
					}
					break;
				case TypeCode.Decimal:
					if (c2 != TypeCode.Decimal)
					{
						return EnumNumTypeCastDir.Type2ToType1; //t2 should be casted to t1
					}
					break;
				case TypeCode.Double:
					if (c2 == TypeCode.Decimal)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.Double)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.Single:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.Single)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.UInt64:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.UInt64)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.Int64:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 == TypeCode.UInt64)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.Int64)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.UInt32:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single
						|| c2 == TypeCode.UInt64 || c2 == TypeCode.Int64)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.UInt32)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.Int32:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single
						|| c2 == TypeCode.UInt64 || c2 == TypeCode.Int64)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 == TypeCode.UInt32)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.Int32)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.UInt16:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single
						|| c2 == TypeCode.UInt64 || c2 == TypeCode.Int64
						|| c2 == TypeCode.UInt32 || c2 == TypeCode.Int32)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.UInt16)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
				case TypeCode.Int16:
					if (c2 == TypeCode.Decimal || c2 == TypeCode.Double || c2 == TypeCode.Single
							|| c2 == TypeCode.UInt64 || c2 == TypeCode.Int64
							|| c2 == TypeCode.UInt32 || c2 == TypeCode.Int32)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 == TypeCode.UInt16)
					{
						return EnumNumTypeCastDir.Type1ToType2;
					}
					if (c2 != TypeCode.Int16)
					{
						return EnumNumTypeCastDir.Type2ToType1;
					}
					break;
			}
			return EnumNumTypeCastDir.NoCast;
		}
	}
}
