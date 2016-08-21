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
using System.Globalization;

namespace LimnorDatabase
{
	[TypeConverter(typeof(TypeConverterSQLString))]
	public class SQLStatement : ICloneable
	{
		private string _sql;
		private StringBuilder _sb;
		public SQLStatement()
		{
		}
		public SQLStatement(string sql)
		{
			_sql = sql;
		}
		public string GetSQL()
		{
			return _sql;
		}
		public void SetSQL(string sql)
		{
			_sql = sql;
		}
		public bool IsCommand(string sep1, string sep2)
		{
			if (!string.IsNullOrEmpty(_sql))
			{
				return FieldsParser.FindStringIndex(_sql, QueryParser.SQL_Select(), 0, sep1, sep2) < 0;
			}
			return false;
		}
		public void StartBuilder()
		{
			_sb = new StringBuilder(_sql);
		}
		public void Append(string s)
		{
			if (_sb == null)
			{
				StartBuilder();
			}
			_sb.Append(s);
		}
		public void FinishBuilder()
		{
			if (_sb != null)
			{
				_sql = _sb.ToString();
				_sb = null;
			}
		}
		public override string ToString()
		{
			return _sql;
		}

		#region ICloneable Members

		public object Clone()
		{
			SQLStatement obj = new SQLStatement(_sql);
			return obj;
		}

		#endregion
	}
	/// <summary>
	/// convert a string to SQLStatement
	/// </summary>
	public class TypeConverterSQLString : TypeConverter
	{
		public TypeConverterSQLString()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			if (context != null)
			{
				if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(sourceType))
					return true;
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				if (converter.CanConvertFrom(context, sourceType))
				{
					return true;
				}
			}

			return base.CanConvertFrom(context, sourceType);
		}
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			string s = value as string;
			if (!string.IsNullOrEmpty(s))
			{
				return new SQLStatement(s);
			}
			if (context != null)
			{
				if (value != null)
				{
					if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(value.GetType()))
						return value;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				return converter.ConvertFrom(context, culture, value);
			}
			return value;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
					return "";

				return value.ToString();
			}
			if (context != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				if (converter.CanConvertTo(destinationType))
				{
					return (string)converter.ConvertTo(context, CultureInfo.InvariantCulture, value, typeof(string));
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

}
