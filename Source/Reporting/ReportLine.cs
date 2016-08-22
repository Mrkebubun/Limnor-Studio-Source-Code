/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data Reporting
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limnor.Reporting
{
	[Flags]
	enum EnumColumnDisplayType
	{
		Pending = 1, //pending decision
		Blank = 2,   //blank
		Text = 4,    //display text
		TopLine = 8, // draw top line. used with Blank and Text
		BottomLine = 16 //draw bottom line. used with Blank and Text
	}
	abstract class ReportLine
	{
		public ReportLine(int rowNum)
		{
			RowNumber = rowNum;
		}
		public int RowNumber { get; private set; }
		public virtual bool UseAltColor(bool b) { return !b; }
	}
	class ReportLineSummary : ReportLine
	{
		public ReportLineSummary(int rowNum, SummaryResult[] sums)
			: base(rowNum)
		{
			Summaries = sums;
		}
		public SummaryResult[] Summaries { get; private set; }
		public override bool UseAltColor(bool b) { return b; }
	}
	class ReportLineData : ReportLine
	{
		public ReportLineData(int rowNum, EnumColumnDisplayType[] colTypes)
			: base(rowNum)
		{
			ColumnDisplayTypes = colTypes;
		}
		public EnumColumnDisplayType[] ColumnDisplayTypes { get; private set; }
	}
}
