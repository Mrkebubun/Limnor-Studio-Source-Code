/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Globalization;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for SQLParser.
	/// </summary>
	public static class SQLParser
	{
		public static string ParseError = "";
		public static int FindParameterIndex(string sSource, int nStart, string sepStart, string sepEnd, out string sMsg)
		{
			int k;
			char c1 = '\0';
			char c2 = '\0';
			if (!string.IsNullOrEmpty(sepStart) && !string.IsNullOrEmpty(sepEnd))
			{
				c1 = sepStart[0];
				c2 = sepEnd[0];
			}
			sMsg = "";
			for (int i = nStart; i < sSource.Length; i++)
			{
				if (sSource[i] == '\'')
				{
					k = i;
					while (true)
					{
						k = sSource.IndexOf('\'', k + 1);
						if (k < 0)
						{
							sMsg = "Cannot find matching single-quote";
							return -1;
						}
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
					{
						sMsg = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Cannot find matching '{0}'", c2);
						return i;
					}
				}
				else if (sSource[i] == '@')
					return i;
			}
			return -1;
		}
		public static int FindNameEnd(string sSQL, int nStart)
		{
			if (nStart >= sSQL.Length)
			{
				return sSQL.Length;
			}
			int i;
			string sSP = ",= +-*/><^|~)(;}\\";
			for (i = nStart; i < sSQL.Length; i++)
			{
				if (sSP.IndexOf(sSQL[i]) >= 0)
					return i;
			}
			return i;
		}
		public static ParameterList ParseParameters(string sSQL, string sepStart, string sepEnd)
		{
			ParameterList paramList = new ParameterList();
			int i = 0, j;
			string s;
			EPField fld;
			while (true)
			{
				i = FindParameterIndex(sSQL, i, sepStart, sepEnd, out ParseError);
				if (i >= 0)
				{
					j = FindNameEnd(sSQL, i + 1);
					if (j > i + 1)
					{
						s = sSQL.Substring(i, j - i);
						fld = paramList[s];
						if (fld == null)
						{
							fld = new EPField();
							fld.Name = s;
							paramList.AddField(fld);
						}
						i = j;
					}
					else
						break;
				}
				else
					break;
			}
			return paramList;
		}
		/// <summary>
		/// replace parameters with ? and collect non-unique parameter names in paramMap
		/// </summary>
		/// <param name="sSQL">the SQL string to parse, it can be any part of SQL</param>
		/// <param name="paramList">for validation. parameter names must be in this list</param>
		/// <param name="paramMap">this list is filled with the non-unique parameter names</param>
		/// <returns>the SQL string with parameters replaced with ?</returns>
		public static string MapParameters(string sSQL, EnumParameterStyle paramStyle, string sepStart, string sepEnd, FieldList paramList, FieldList paramMap, ParameterList psOld)
		{
			if (sSQL == null)
				sSQL = "";
			string S = sSQL;
			if (S.Length > 0)
			{
				string name;
				EPField fld;
				int n = SQLParser.FindParameterIndex(S, 0, sepStart, sepEnd, out ParseError);
				while (n >= 0)
				{
					int i = SQLParser.FindNameEnd(S, n);
					name = S.Substring(n, i - n);
					name = name.Trim();
					if (paramStyle == EnumParameterStyle.QuestionMark)
					{
						S = string.Format(CultureInfo.InvariantCulture, "{0}? {1}", S.Substring(0, n), S.Substring(i));
						n++;
					}
					else if (paramStyle == EnumParameterStyle.LeadingQuestionMark)
					{
						string pname = ParameterList.GetParameterName(paramStyle, name);
						S = string.Format(CultureInfo.InvariantCulture, "{0}{1} {2}", S.Substring(0, n), pname, S.Substring(i));
						n += pname.Length + 1;
					}
					else
					{
						n += name.Length + 1;
					}
					fld = paramList[name];
					if (fld == null)
					{
						EPField f0 = null;
						if (psOld != null && psOld.Count > 0)
						{
							f0 = psOld[name];
						}
						if (f0 == null)
						{
							fld = new EPField(paramList.Count, name);
						}
						else
						{
							fld = f0.Clone() as EPField;
							fld.Index = paramList.Count;
						}
						paramList.Add(fld);
					}
					if (paramStyle == EnumParameterStyle.QuestionMark)
					{
						//non-unique name is added
						paramMap.AddFieldDup(fld);
					}
					n = SQLParser.FindParameterIndex(S, n, sepStart, sepEnd, out ParseError);
				}
			}
			return S;
		}
	}
}
