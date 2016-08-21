/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for FieldsParser.
	/// </summary>
	public class FieldsParser
	{
		public QryField[] Fields = null;
		public bool Distinct = false;
		public int TopRec = -1;
		public bool Percent = false;
		public bool WithTies = false;
		public FieldsParser()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public int Count
		{
			get
			{
				if (Fields == null)
					return 0;
				return Fields.Length;
			}
		}
		public QryField this[int Index]
		{
			get
			{
				if (Fields != null)
					if (Index >= 0 && Index < Fields.Length)
						return Fields[Index];
				return null;
			}
		}
		public void AddField(QryField fld)
		{
			int n;
			if (Fields == null)
			{
				Fields = new QryField[1];
				n = 0;
			}
			else
			{
				n = Fields.Length;
				QryField[] a = new QryField[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = Fields[i];
				Fields = a;
			}
			Fields[n] = fld;
		}
		public static string[] ParseFieldList(string fieldList, string sepStart, string sepEnd)
		{
			if (string.IsNullOrEmpty(fieldList))
			{
				return new string[] { };
			}
			else
			{
				List<string> ss = new List<string>();
				while (fieldList.Length > 0)
				{
					string s = FieldsParser.PopValue(ref fieldList, ",", sepStart, sepEnd);
					ss.Add(s.Trim());
				}
				return ss.ToArray();
			}
		}
		public static FieldList ParseFieldsStringIntoFieldList(string fieldList, string sepStart, string sepEnd)
		{
			string[] ss = ParseFieldList(fieldList, sepStart, sepEnd);
			FieldList list = new FieldList();
			for (int i = 0; i < ss.Length; i++)
			{
				QryField qf = new QryField(ss[i], sepStart, sepEnd);
				list.Add(qf.MakeField());
			}
			return list;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sSQL">must start with "SELECT "</param>
		/// <param name="fieldsText"></param>
		/// <param name="sInvalid"></param>
		/// <returns></returns>
		public bool ParseQuery(string sSQL, string sepStart, string sepEnd, ref string sInvalid)
		{
			Fields = null;
			sSQL = sSQL.Trim();
			if (sSQL.Length < 7)
			{
				sInvalid = sSQL;
				return false;
			}
			int i;
			string sField = "";
			string sBuf;
			//remove SELECT
			if (!sSQL.StartsWith(QueryParser.SQL_Select(), StringComparison.OrdinalIgnoreCase))
			{
				sInvalid = sSQL.Substring(0, 10);
				return false;
			}
			sSQL = sSQL.Substring(7);
			sSQL = sSQL.Trim();
			//parse ALL | DISTINCT
			Distinct = false;
			if (sSQL.Length > 4)
			{
				if (sSQL.StartsWith("ALL ", StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(4);
					sSQL = sSQL.Trim();
				}
			}
			if (sSQL.Length > 9)
			{
				if (sSQL.StartsWith(QueryParser.SQL_Distinct(), StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(9);
					sSQL = sSQL.Trim();
					Distinct = true;
				}
			}
			if (sSQL.Length > 4)
			{
				//parse TOP n [ PERCENT ]
				if (sSQL.StartsWith(QueryParser.SQL_Top_MS(), StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(4);
					sSQL = sSQL.Trim();
					i = sSQL.IndexOf(' ');
					if (i < 0)
					{
						sInvalid = sSQL;
						return false;
					}
					sBuf = sSQL.Substring(0, i);
					try
					{
						TopRec = Convert.ToInt32(sBuf);
						sSQL = sSQL.Substring(i);
						sSQL = sSQL.Trim();
						//parse PERCENT
						if (sSQL.Length > 8)
						{
							if (sSQL.StartsWith(QueryParser.SQL_Percent(), StringComparison.OrdinalIgnoreCase))
							{
								sSQL = sSQL.Substring(8);
								sSQL = sSQL.Trim();
								Percent = true;
							}
						}
					}
					catch
					{
						sInvalid = sSQL;
						return false;
					}
				}
			}
			if (sSQL.Length > 10)
			{
				//parse WITH TIES
				if (sSQL.StartsWith(QueryParser.SQL_WithTies(), StringComparison.OrdinalIgnoreCase))
				{
					sSQL = sSQL.Substring(10);
					sSQL = sSQL.Trim();
					WithTies = true;
				}
			}
			//parse fields
			i = FindStringIndex(sSQL, QueryParser.SQL_From(), 0, sepStart, sepEnd);
			if (i > 0)
			{
				sSQL = sSQL.Substring(0, i);
				sSQL = sSQL.Trim();
			}
			int nPos;
			while (true)
			{
				nPos = FindStringIndex(sSQL, ",", 0, sepStart, sepEnd);
				if (nPos > 0)
				{
					sField = sSQL.Substring(0, nPos);
					sSQL = sSQL.Substring(nPos + 1);
					sSQL = sSQL.Trim();
					AddField(new QryField(sField, sepStart, sepEnd));
				}
				else
				{
					sField = sSQL;
					AddField(new QryField(sField, sepStart, sepEnd));
					break;
				}
			}

			return true;
		}
		public string GetFieldListText()
		{
			string s = "";
			if (Fields != null)
			{
				if (Fields.Length > 0)
				{
					s = Fields[0].Field;
					for (int i = 1; i < Fields.Length; i++)
					{
						s += "," + Fields[i].Field;
					}
				}
			}
			return s;
		}
		public QryField FindField(string fldName)
		{
			if (Fields != null)
			{
				for (int i = 0; i < Fields.Length; i++)
				{
					if (string.Compare(Fields[i].FieldName, fldName, StringComparison.OrdinalIgnoreCase) == 0)
						return Fields[i];
				}
			}
			return null;
		}
		public QryField FindFieldByText(string fldText)
		{
			if (Fields != null)
			{
				for (int i = 0; i < Fields.Length; i++)
				{
					if (string.Compare(Fields[i].FieldText, fldText, StringComparison.OrdinalIgnoreCase) == 0)
						return Fields[i];
				}
			}
			return null;
		}
		public static bool MatchStringIndex(string sSource, string val, int index)
		{
			if (index + val.Length - 1 >= sSource.Length)
				return false;
			return (string.Compare(sSource, index, val, 0, val.Length, StringComparison.OrdinalIgnoreCase) == 0);
		}
		/// <summary>
		/// Find val position statrting from nStart
		/// </summary>
		/// <param name="sSource"></param>
		/// <param name="val"></param>
		/// <param name="nStart"></param>
		/// <returns>-1: not found</returns>
		public static int FindStringIndex(string sSource, string val, int nStart, string sepStart, string sepEnd)
		{
			int k;
			char c1 = '\0';
			char c2 = '\0';
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				c1 = sepStart[0];
				c2 = sepEnd[0];
			}
			for (int i = nStart; i < sSource.Length; i++)
			{
				if (sSource[i] == '\'')
				{
					k = i;
					while (true)
					{
						k = sSource.IndexOf('\'', k + 1);
						if (k < 0)
							throw new Exception("Cannot find matching single-quote");
						else
						{
							if (k == sSource.Length - 1)
								return -1;
							else
							{
								if (sSource[k + 1] != '\'')
								{
									i = k;
									break;
								}
								else
									k++;
							}
						}
					}
				}
				else if (sSource[i] == c1)
				{
					i = sSource.IndexOf(c2, i + 1);
					if (i < 0)
						throw new ExceptionLimnorDatabase("Cannot find matching '{0}'", sepEnd);
				}
				else if (sSource[i] == '(')
				{
					i = FindStringIndex(sSource, ")", i + 1, sepStart, sepEnd);
					if (i < 0)
						throw new ExceptionLimnorDatabase("Cannot find matching ')'");
				}
				else if (MatchStringIndex(sSource, val, i))
					return i;
			}
			return -1;
		}
		public static bool IsJoinKeyword(string s)
		{
			if (string.Compare(s, "ON", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "RIGHT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "OUTER", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "LEFT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "INNER", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "JOIN", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "WHERE", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsFieldKeyword(string s)
		{
			if (string.Compare(s, "ON", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "CASE", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "WHEN", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "THEN", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "END", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "AS", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "AND", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "OR", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(s, "JOIN", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
		public static string PopValue(ref string s, string sep, string sepStart, string sepEnd)
		{
			string sRet;
			int i = FindStringIndex(s, sep, 0, sepStart, sepEnd);
			if (i < 0)
			{
				sRet = s;
				s = "";
			}
			else
			{
				if (i == 0)
				{
					sRet = "";
					s = s.Substring(sep.Length);
				}
				else
				{
					sRet = s.Substring(0, i);
					s = s.Substring(i + sep.Length);
				}
			}
			return sRet;
		}
		/// <summary>
		/// 1.[TableName].[fieldname]; 2.[TableName].fieldname;
		/// 3.Tablename.[fieldname]; 4. TableName.fieldname;
		/// 5.fieldName (including expression)
		/// </summary>
		/// <param name="sFieldText"></param>
		/// <returns></returns>
		public static string GetFieldNameFromFieldText(string sFieldText, string sepStart, string sepEnd)
		{
			int i = FindStringIndex(sFieldText, " AS ", 0, sepStart, sepEnd);
			if (i > 0)
			{
				sFieldText = sFieldText.Substring(0, i).Trim();
			}
			i = sFieldText.IndexOf('.');
			if (i > 0)
			{
				sFieldText = sFieldText.Substring(i + 1);
			}
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd) && sFieldText.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase) && sFieldText.Length > sepStart.Length + sepEnd.Length)
			{
				sFieldText = sFieldText.Substring(sepStart.Length, sFieldText.Length - sepStart.Length - sepEnd.Length);
			}
			return sFieldText;
		}
	}
	public class QryField
	{
		public string Table = ""; //table name/alias
		public string Field = ""; //whole text
		public string FieldName = ""; //field name/alias
		public string FieldText = ""; //field text without "AS ***"
		public QryField()
		{
		}
		public QryField(string f, string sepStart, string sepEnd)
		{
			//find table name/alias
			int n = -1;
			string t = "";
			int sepLeng = 0;
			FieldText = f;
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				sepLeng = sepStart.Length + sepEnd.Length;
			}
			f = f.Trim();
			if (sepLeng > 0 && f.Length > sepLeng)
			{
				if (f.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase))
				{
					n = f.IndexOf(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.{1}", sepEnd, sepStart), StringComparison.OrdinalIgnoreCase);
					if (n > 1)
					{
						t = f.Substring(sepStart.Length, n - sepEnd.Length);
						f = f.Substring(n + 1 + sepEnd.Length);
					}
				}
			}
			if (n < 2)
			{
				int i = FieldsParser.FindStringIndex(f, ".", 0, sepStart, sepEnd);
				if (i > 0)
				{
					t = f.Substring(0, i);
				}
			}
			Table = t;
			string fn = "";
			//find field name by looking for " AS " backward
			if (ParseExplicitFieldName(f, sepStart, sepEnd, ref fn, ref t))
			{
				FieldName = fn;
				FieldText = t;
			}
			else
			{
				if (sepLeng > 0)
				{
					if (f.StartsWith(sepStart) && f.EndsWith(sepEnd))
					{
						f = f.Substring(sepStart.Length, f.Length - sepLeng);
					}
				}
				FieldName = f;
			}
			Field = f;
		}
		public EPField MakeField()
		{
			EPField f = new EPField();
			f.Name = FieldName;
			f.FieldText = FieldText;
			f.FromTableName = Table;
			return f;
		}
		public static bool ParseExplicitFieldName(string f, string sepStart, string sepEnd, ref string name, ref string text)
		{
			int i = FieldsParser.FindStringIndex(f, " AS ", 0, sepStart, sepEnd);
			if (i > 0)
			{
				text = f.Substring(0, i);
				text = text.Trim();
				name = f.Substring(i + 4);
				name = name.Trim();
				int sepLeng = 0;
				if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
				{
					sepLeng = sepStart.Length + sepEnd.Length;
				}
				if (sepLeng > 0 && name.Length > sepLeng)
				{
					if (name.StartsWith(sepStart, StringComparison.OrdinalIgnoreCase) && name.EndsWith(sepEnd, StringComparison.OrdinalIgnoreCase))
						name = name.Substring(sepStart.Length, name.Length - sepEnd.Length);
				}
				return true;
			}
			return false;
		}
		public override string ToString()
		{
			if (Table.Length == 0)
				return Field;
			return Table + "->" + Field;
		}

	}
}
