/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data Reporting
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Data;
using LimnorDatabase;
using System.Globalization;
using System.Drawing;

namespace Limnor.Reporting
{
	/// <summary>
	/// identify one page
	/// </summary>
	class PageRecord
	{
		#region fields and constructors
		private PageRecordList _owner;
		private List<ReportLine> _reportLines;
		public PageRecord(PageRecordList owner, int startNo, int fieldCount, FieldList sortFields)
		{
			_owner = owner;
			StartRecordNo = startNo;
			StartColumnSummaries = new ColumnSummary[sortFields.Count];
			for (int i = 0; i < StartColumnSummaries.Length; i++)
			{
				StartColumnSummaries[i] = new ColumnSummary(sortFields[i].Index, fieldCount);
			}
			EndColumnSummaries = new ColumnSummary[sortFields.Count];
			for (int i = 0; i < EndColumnSummaries.Length; i++)
			{
				EndColumnSummaries[i] = new ColumnSummary(sortFields[i].Index, fieldCount);
			}
			StartSummaryKeys = new Variant[fieldCount];
			EndSummaryKeys = new Variant[fieldCount];
		}
		public PageRecord(PageRecordList owner, int startNo, int fieldCount, FieldList sortFields, Variant[] keys, ColumnSummary[] sums)
			: this(owner, startNo, fieldCount, sortFields)
		{
			if (sums != null)
			{
				for (int k = 0; k < StartColumnSummaries.Length; k++)
				{
					sums[k].ColumnSummaries.CopyTo(StartColumnSummaries[k].ColumnSummaries, 0);
					sums[k].ColumnSummaries.CopyTo(EndColumnSummaries[k].ColumnSummaries, 0);
					//
					sums[k].ColumnRecordCount.CopyTo(StartColumnSummaries[k].ColumnRecordCount, 0);
					sums[k].ColumnRecordCount.CopyTo(EndColumnSummaries[k].ColumnRecordCount, 0);
				}
			}
			if (keys != null)
			{
				keys.CopyTo(StartSummaryKeys, 0);
				keys.CopyTo(EndSummaryKeys, 0);
			}
		}
		#endregion
		#region Methods
		public void ResetEndSummaries(int sortLevel)
		{
			for (int k = sortLevel; k < EndColumnSummaries.Length; k++)
			{
				for (int i = 0; i < EndColumnSummaries[k].ColumnSummaries.Length; i++)
				{
					EndColumnSummaries[k].ColumnSummaries[i] = 0;
					EndColumnSummaries[k].ColumnRecordCount[i] = 0;
				}
			}
		}
		public void AddLine(ReportLine line)
		{
			if (_reportLines == null)
			{
				_reportLines = new List<ReportLine>();
			}
			_reportLines.Add(line);
		}
		public void SetStartKeyValues(DataTable tbl)
		{
			for (int i = 0; i < tbl.Columns.Count; i++)
			{
				StartSummaryKeys[i] = new Variant(Type.GetTypeCode(tbl.Columns[i].DataType), tbl.Rows[0][i]);
				EndSummaryKeys[i] = new Variant(Type.GetTypeCode(tbl.Columns[i].DataType), tbl.Rows[0][i]);
			}
		}

		public SummaryResult[] CreateLastSummaries()
		{
			SummaryResult[] ret;
			ret = new SummaryResult[StartColumnSummaries.Length];
			for (int k = 0; k < EndColumnSummaries.Length; k++)
			{
				ret[k] = new SummaryResult(EndColumnSummaries[k].ColumnSummaries, EndColumnSummaries[k].ColumnRecordCount);
			}
			_owner.AddResults(-1, ret);
			return ret;
		}
		/// <summary>
		/// do summary for each sorting field
		/// </summary>
		/// <param name="r"></param>
		/// <param name="cac"></param>
		/// <returns>Each item represent one summary result for a sorting field. If an item is null then the summary not finish</returns>
		public SummaryResult[] ProcessRow(DataTable tbl, int rowNumber, ColumnAttributesCollection cac)
		{
			bool hasSum = false;
			DataRow r = tbl.Rows[rowNumber];
			SummaryResult[] ret;
			ret = new SummaryResult[StartColumnSummaries.Length];
			for (int k = 0; k < EndColumnSummaries.Length; k++)
			{
				ret[k] = null;
				//sort value
				object v = r[EndColumnSummaries[k].SortFieldIndex];
				//sort field ordinal
				int kI = EndColumnSummaries[k].SortFieldIndex;
				if (EndSummaryKeys[kI].IsEqual(v))
				{
					//key not changed, do summary
					for (int i = 0; i < cac.Count; i++)
					{
						if (cac[i].ShowSummaries)
						{
							EndColumnSummaries[k].ColumnSummaries[i] += Convert.ToDouble(r[i], CultureInfo.InvariantCulture);
							EndColumnSummaries[k].ColumnRecordCount[i] = EndColumnSummaries[k].ColumnRecordCount[i] + 1;
						}
					}
				}
				else
				{
					//key changed, return EndColumnSummaries[k].ColumnSummaries
					for (int h = k; h < EndColumnSummaries.Length; h++)
					{
						ret[h] = new SummaryResult(EndColumnSummaries[h].ColumnSummaries, EndColumnSummaries[h].ColumnRecordCount);
						int ki = EndColumnSummaries[h].SortFieldIndex;
						EndSummaryKeys[ki].SetValue(r[ki]);
					}
					//reset initial value for the summaries of all sub-elevels
					for (int h = k; h < EndColumnSummaries.Length; h++)
					{
						for (int i = 0; i < cac.Count; i++)
						{
							if (cac[i].ShowSummaries)
							{
								if (r[i] == null || r[i] == DBNull.Value)
								{
									EndColumnSummaries[h].ColumnSummaries[i] = 0;
								}
								else
								{
									EndColumnSummaries[h].ColumnSummaries[i] = Convert.ToDouble(r[i], CultureInfo.InvariantCulture);
								}
								EndColumnSummaries[h].ColumnRecordCount[i] = 1;
							}
						}
					}
					hasSum = true;
					//rowNumber is actually the next row of the summing rows
					_owner.AddResults(rowNumber - 1, ret);
					break; //no need to process sub-levels
				}
			}
			if (hasSum)
			{
				return ret;
			}
			return null;
		}
		#endregion
		#region Properties
		public IList<ReportLine> Lines
		{
			get
			{
				if (_reportLines == null)
				{
					_reportLines = new List<ReportLine>();
				}
				return _reportLines;
			}
		}
		/// <summary>
		/// this page start number
		/// </summary>
		public int StartRecordNo { get; private set; }
		/// <summary>
		/// next page start number
		/// </summary>
		public int EndRecordNo { get; set; }
		//
		public bool Processed { get; set; }
		//
		public ColumnSummary[] StartColumnSummaries { get; private set; }
		public ColumnSummary[] EndColumnSummaries { get; private set; }
		public Variant[] StartSummaryKeys { get; private set; }
		public Variant[] EndSummaryKeys { get; private set; }
		#endregion
	}
	class PageRecordList : List<PageRecord>
	{
		private Dictionary<int, SummaryResult[]> _summaries;
		private Dictionary<int, int> _summariesOnPageTop;
		public PageRecordList()
		{
		}
		public void AddPageTopSummaries(int pageIndex, int recordNo)
		{
			if (_summariesOnPageTop == null)
			{
				_summariesOnPageTop = new Dictionary<int, int>();
			}
			if (!_summariesOnPageTop.ContainsKey(pageIndex))
			{
				_summariesOnPageTop.Add(pageIndex, recordNo);
			}
		}
		public SummaryResult[] GetPageTopSummaries(int pageIndex)
		{
			int rowNumber;
			if (_summariesOnPageTop != null)
			{
				if (_summariesOnPageTop.TryGetValue(pageIndex, out rowNumber))
				{
					return GetResults(rowNumber);
				}
			}
			return null;
		}
		public void AddResults(int rowNumber, SummaryResult[] ret)
		{
			if (_summaries == null)
			{
				_summaries = new Dictionary<int, SummaryResult[]>();
			}
			if (!_summaries.ContainsKey(rowNumber))
			{
				_summaries.Add(rowNumber, ret);
			}
		}
		public SummaryResult[] GetResults(int rowNumber)
		{
			if (_summaries != null)
			{
				SummaryResult[] ret;
				if (_summaries.TryGetValue(rowNumber, out ret))
				{
					return ret;
				}
			}
			return null;
		}
		public SummaryResult[] GetLastSummaries()
		{
			return GetResults(-1);
		}
	}
	class SummaryResult
	{
		public SummaryResult(double[] v, int[] recordCount)
		{
			Sum = new double[v.Length];
			v.CopyTo(Sum, 0);
			RowCount = new int[recordCount.Length];
			recordCount.CopyTo(RowCount, 0);
		}
		public double[] Sum { get; private set; }
		public int[] RowCount { get; private set; }
	}
	/// <summary>
	/// summaries corresponding to one sort field
	/// </summary>
	class ColumnSummary
	{
		public ColumnSummary()
		{
		}
		public ColumnSummary(int fieldIndex, int columns)
		{
			SortFieldIndex = fieldIndex;
			ColumnSummaries = new double[columns];
			ColumnRecordCount = new int[columns];
		}
		public int SortFieldIndex { get; private set; }
		public double[] ColumnSummaries { get; private set; }
		public int[] ColumnRecordCount { get; private set; }
	}
}
