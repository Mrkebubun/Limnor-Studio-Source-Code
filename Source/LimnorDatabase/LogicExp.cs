/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
	public enum enumLogicType2
	{
		Equal = 0,
		NotEqual,
		Contains,
		Included,
		BeginWith,
		EndWith,
		Larger,
		Smaller,
		LargerEqual,
		SmallerEqual,
		BeginOf,
		EndOf,
		Equal_NoCase,
		NotEqual_NoCase,
		Contains_NoCase,
		Included_NoCase,
		BeginWith_NoCase,
		EndWith_NoCase,
		BeginOf_NoCase,
		EndOf_NoCase
	}
	class LogicExp
	{
		public static bool IsInteger(System.TypeCode c)
		{
			if (c == System.TypeCode.Int16)
				return true;
			if (c == System.TypeCode.Int32)
				return true;
			if (c == System.TypeCode.Int64)
				return true;
			if (c == System.TypeCode.SByte)
				return true;
			return false;
		}
		public static bool IsUInteger(System.TypeCode c)
		{
			if (c == System.TypeCode.Byte)
				return true;
			if (c == System.TypeCode.UInt16)
				return true;
			if (c == System.TypeCode.UInt32)
				return true;
			if (c == System.TypeCode.UInt64)
				return true;
			return false;
		}
		public static bool IsDecimal(System.TypeCode c)
		{
			if (c == System.TypeCode.Decimal)
				return true;
			if (c == System.TypeCode.Double)
				return true;
			if (c == System.TypeCode.Single)
				return true;
			return false;
		}
		public static bool Compare(object v1, enumLogicType2 logic, object v2)
		{
			if (v1 == null || v1 == DBNull.Value)
			{
				switch (logic)
				{
					case enumLogicType2.Equal:
						return (v2 == null);
					case enumLogicType2.LargerEqual:
						return (v2 == null);
					case enumLogicType2.NotEqual:
						return (v2 != null);
					case enumLogicType2.SmallerEqual:
						return (v2 == null);
					default:
						return false;
				}
			}
			//v1 != null
			if (v2 == null || v2 == DBNull.Value)
			{
				switch (logic)
				{
					case enumLogicType2.NotEqual:
						return true;
					case enumLogicType2.Larger:
						return true;
					case enumLogicType2.LargerEqual:
						return true;
					default:
						return false;
				}
			}
			if ((int)logic < (int)enumLogicType2.Equal_NoCase)
			{
				System.Type tp1 = v1.GetType();
				System.TypeCode c1 = System.Type.GetTypeCode(tp1);
				System.Type tp2 = v2.GetType();
				System.TypeCode c2 = System.Type.GetTypeCode(tp2);
				if (IsInteger(c1))
				{
					if (IsInteger(c2))
					{
						System.Int64 i1 = Convert.ToInt64(v1);
						System.Int64 i2 = Convert.ToInt64(v2);
						switch (logic)
						{
							case enumLogicType2.Equal:
								return (i1 == i2);
							case enumLogicType2.Larger:
								return (i1 > i2);
							case enumLogicType2.LargerEqual:
								return (i1 >= i2);
							case enumLogicType2.NotEqual:
								return (i1 != i2);
							case enumLogicType2.Smaller:
								return (i1 < i2);
							case enumLogicType2.SmallerEqual:
								return (i1 <= i2);
							default:
								return false;
						}
					}
					else if (IsUInteger(c2))
					{
						System.UInt64 u1 = Convert.ToUInt64(v1);
						System.UInt64 u2 = Convert.ToUInt64(v2);
						switch (logic)
						{
							case enumLogicType2.Equal:
								return (u1 == u2);
							case enumLogicType2.Larger:
								return (u1 > u2);
							case enumLogicType2.LargerEqual:
								return (u1 >= u2);
							case enumLogicType2.NotEqual:
								return (u1 != u2);
							case enumLogicType2.Smaller:
								return (u1 < u2);
							case enumLogicType2.SmallerEqual:
								return (u1 <= u2);
							default:
								return false;
						}
					}
					else
					{
						System.Int64 i1 = Convert.ToInt64(v1);
						try
						{
							System.Int64 i2 = Convert.ToInt64(v2);
							switch (logic)
							{
								case enumLogicType2.Equal:
									return (i1 == i2);
								case enumLogicType2.Larger:
									return (i1 > i2);
								case enumLogicType2.LargerEqual:
									return (i1 >= i2);
								case enumLogicType2.NotEqual:
									return (i1 != i2);
								case enumLogicType2.Smaller:
									return (i1 < i2);
								case enumLogicType2.SmallerEqual:
									return (i1 <= i2);
								default:
									return false;
							}
						}
						catch
						{
						}
						return false;
					}
				}
				else if (IsUInteger(c1))
				{
					if (IsInteger(c2))
					{
						System.UInt64 u1 = Convert.ToUInt64(v1);
						System.UInt64 u2 = Convert.ToUInt64(v2);
						switch (logic)
						{
							case enumLogicType2.Equal:
								return (u1 == u2);
							case enumLogicType2.Larger:
								return (u1 > u2);
							case enumLogicType2.LargerEqual:
								return (u1 >= u2);
							case enumLogicType2.NotEqual:
								return (u1 != u2);
							case enumLogicType2.Smaller:
								return (u1 < u2);
							case enumLogicType2.SmallerEqual:
								return (u1 <= u2);
							default:
								return false;
						}
					}
					else if (IsUInteger(c2))
					{
						System.UInt64 u1 = Convert.ToUInt64(v1);
						System.UInt64 u2 = Convert.ToUInt64(v2);
						switch (logic)
						{
							case enumLogicType2.Equal:
								return (u1 == u2);
							case enumLogicType2.Larger:
								return (u1 > u2);
							case enumLogicType2.LargerEqual:
								return (u1 >= u2);
							case enumLogicType2.NotEqual:
								return (u1 != u2);
							case enumLogicType2.Smaller:
								return (u1 < u2);
							case enumLogicType2.SmallerEqual:
								return (u1 <= u2);
							default:
								return false;
						}
					}
					else
					{
						System.UInt64 u1 = Convert.ToUInt64(v1);
						try
						{
							System.UInt64 u2 = Convert.ToUInt64(v2);
							switch (logic)
							{
								case enumLogicType2.Equal:
									return (u1 == u2);
								case enumLogicType2.Larger:
									return (u1 > u2);
								case enumLogicType2.LargerEqual:
									return (u1 >= u2);
								case enumLogicType2.NotEqual:
									return (u1 != u2);
								case enumLogicType2.Smaller:
									return (u1 < u2);
								case enumLogicType2.SmallerEqual:
									return (u1 <= u2);
								default:
									return false;
							}
						}
						catch
						{
						}
						return false;
					}
				}
				else if (c1 == System.TypeCode.Boolean || c2 == System.TypeCode.Boolean)
				{
					bool b1 = ValueConvertor.ToBool(v1);
					bool b2 = ValueConvertor.ToBool(v2);
					switch (logic)
					{
						case enumLogicType2.Equal:
							return (b1 == b2);
						case enumLogicType2.NotEqual:
							return (b1 != b2);
						default:
							return false;
					}
				}
				else if (c1 == System.TypeCode.DateTime || c2 == System.TypeCode.DateTime)
				{
					System.DateTime d1 = ValueConvertor.ToDateTime(v1);
					System.DateTime d2 = ValueConvertor.ToDateTime(v2);
					switch (logic)
					{
						case enumLogicType2.Equal:
							return (d1 == d2);
						case enumLogicType2.Larger:
							return (d1 > d2);
						case enumLogicType2.LargerEqual:
							return (d1 >= d2);
						case enumLogicType2.NotEqual:
							return (d1 != d2);
						case enumLogicType2.Smaller:
							return (d1 < d2);
						case enumLogicType2.SmallerEqual:
							return (d1 <= d2);
						default:
							return false;
					}
				}
				else if (IsDecimal(c1) || IsDecimal(c2))
				{
					double d1 = ValueConvertor.ToDouble(v1);
					double d2 = ValueConvertor.ToDouble(v2);
					switch (logic)
					{
						case enumLogicType2.Equal:
							return (d1 == d2);
						case enumLogicType2.Larger:
							return (d1 > d2);
						case enumLogicType2.LargerEqual:
							return (d1 >= d2);
						case enumLogicType2.NotEqual:
							return (d1 != d2);
						case enumLogicType2.Smaller:
							return (d1 < d2);
						case enumLogicType2.SmallerEqual:
							return (d1 <= d2);
						default:
							return false;
					}
				}
				else if (tp1.Equals(typeof(System.Drawing.Color)) || tp2.Equals(typeof(System.Drawing.Color)))
				{
					int d1 = ValueConvertor.ToColor(v1).ToArgb();
					int d2 = ValueConvertor.ToColor(v2).ToArgb();
					switch (logic)
					{
						case enumLogicType2.Equal:
							return (d1 == d2);
						case enumLogicType2.Larger:
							return (d1 > d2);
						case enumLogicType2.LargerEqual:
							return (d1 >= d2);
						case enumLogicType2.NotEqual:
							return (d1 != d2);
						case enumLogicType2.Smaller:
							return (d1 < d2);
						case enumLogicType2.SmallerEqual:
							return (d1 <= d2);
						default:
							return false;
					}
				}
			}
			//string comparing
			string s1 = v1.ToString();
			string s2 = v2.ToString();
			bool bCompareAsNumbers = false;
			double dv1 = 0;
			double dv2 = 0;
			if (logic == enumLogicType2.Equal ||
				logic == enumLogicType2.Larger ||
				logic == enumLogicType2.LargerEqual ||
				logic == enumLogicType2.NotEqual ||
				logic == enumLogicType2.Smaller ||
				logic == enumLogicType2.SmallerEqual)
			{
				if (double.TryParse(s1, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out dv1))
				{
					if (double.TryParse(s2, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out dv2))
					{
						bCompareAsNumbers = true;
					}
				}
			}
			if (bCompareAsNumbers)
			{
				switch (logic)
				{
					case enumLogicType2.Equal:
						return (dv1 == dv2);
					case enumLogicType2.Larger:
						return (dv1 > dv2);
					case enumLogicType2.LargerEqual:
						return (dv1 >= dv2);
					case enumLogicType2.NotEqual:
						return (dv1 != dv2);
					case enumLogicType2.Smaller:
						return (dv1 < dv2);
					case enumLogicType2.SmallerEqual:
						return (dv1 <= dv2);
					default:
						bCompareAsNumbers = false;
						break;
				}
			}
			{
				switch (logic)
				{
					case enumLogicType2.BeginWith:
						return (s1.StartsWith(s2, StringComparison.Ordinal));
					case enumLogicType2.BeginWith_NoCase:
						return (s1.StartsWith(s2, StringComparison.OrdinalIgnoreCase));
					case enumLogicType2.Contains:
						return (s1.IndexOf(s2, StringComparison.Ordinal) >= 0);
					case enumLogicType2.Contains_NoCase:
						return (s1.IndexOf(s2, StringComparison.OrdinalIgnoreCase) >= 0);
					case enumLogicType2.EndWith:
						return s1.EndsWith(s2, StringComparison.Ordinal);
					case enumLogicType2.EndWith_NoCase:
						return s1.EndsWith(s2, StringComparison.OrdinalIgnoreCase);
					case enumLogicType2.Included:
						return (s2.IndexOf(s1, StringComparison.Ordinal) >= 0);
					case enumLogicType2.Included_NoCase:
						return (s2.IndexOf(s1, StringComparison.OrdinalIgnoreCase) >= 0);
					case enumLogicType2.Equal:
						return (string.Compare(s1, s2, StringComparison.Ordinal) == 0);
					case enumLogicType2.Equal_NoCase:
						return (string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0);
					case enumLogicType2.Larger:
						return (s1.CompareTo(s2) > 0);
					case enumLogicType2.LargerEqual:
						return (s1.CompareTo(s2) >= 0);
					case enumLogicType2.NotEqual:
						return (string.Compare(s1, s2, StringComparison.Ordinal) != 0);
					case enumLogicType2.NotEqual_NoCase:
						return (string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) != 0);
					case enumLogicType2.Smaller:
						return (s1.CompareTo(s2) < 0);
					case enumLogicType2.SmallerEqual:
						return (s1.CompareTo(s2) <= 0);
					case enumLogicType2.BeginOf:
						return (s2.StartsWith(s1, StringComparison.Ordinal));
					case enumLogicType2.BeginOf_NoCase:
						return (s2.StartsWith(s1, StringComparison.OrdinalIgnoreCase));
					case enumLogicType2.EndOf:
						return s2.EndsWith(s1, StringComparison.Ordinal);
					case enumLogicType2.EndOf_NoCase:
						return s2.EndsWith(s1, StringComparison.OrdinalIgnoreCase);
					default:
						return false;
				}
			}
		}
	}
}
