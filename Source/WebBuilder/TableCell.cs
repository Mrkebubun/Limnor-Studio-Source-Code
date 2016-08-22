/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using VPL;
using System.Drawing.Design;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LimnorWebTableCell : TableCellBase
	{
		public LimnorWebTableCell()
		{
		}
		[Description("Gets and sets the text of the hean of the cell")]
		[DefaultValue(null)]
		public string Text { get; set; }

		protected override void OnCreateHtmlContect(XmlNode td)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				td.InnerText = Text;
			}
			XmlElement xe = (XmlElement)td;
			xe.IsEmpty = false;
		}
		protected override void OnCreateHtmlAttributeString(StringBuilder sb)
		{
		}
		protected override void OnCreateHtmlContentString(StringBuilder sb)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				sb.Append(Text);
			}
		}
		public override string ToString()
		{
			if (Text == null)
			{
				return string.Empty;
			}
			return Text;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LimnorWebTableRow
	{
		private LimnorWebTableCell[] _cells;
		public LimnorWebTableRow()
		{
		}
		public void SetSize(int size)
		{
			int n;
			if (_cells == null)
				n = 0;
			else
				n = _cells.Length;
			if (n < size)
			{
				LimnorWebTableCell[] a = new LimnorWebTableCell[size];
				for (int i = 0; i < n; i++)
				{
					a[i] = _cells[i];
				}
				_cells = a;
			}
			else if (n > size)
			{
				LimnorWebTableCell[] a = new LimnorWebTableCell[size];
				for (int i = 0; i < size; i++)
				{
					a[i] = _cells[i];
				}
				_cells = a;
			}
			for (int i = 0; i < size; i++)
			{
				if (_cells[i] == null)
				{
					_cells[i] = new LimnorWebTableCell();
				}
			}
		}
		public LimnorWebTableCell[] Cells
		{
			get
			{
				if (_cells == null)
				{
					_cells = new LimnorWebTableCell[] { };
				}
				return _cells;
			}
			set
			{
				_cells = value;
			}
		}
		public void CreateHtmlContent(XmlNode node)
		{
			XmlNode tr = node.OwnerDocument.CreateElement("tr");
			node.AppendChild(tr);
			if (_cells != null)
			{
				int n = 0;
				int i = 0;
				while (n < _cells.Length)
				{
					_cells[i].CreateHtmlContent(tr);
					if (_cells[i].ColumnSpan < 2)
					{
						n++;
					}
					else
					{
						n += _cells[i].ColumnSpan;
					}
					i++;
				}
			}
		}
		public void CreateHtmlString(StringBuilder sb)
		{
			sb.Append("<tr>");
			if (_cells != null)
			{
				int n = 0;
				int i = 0;
				while (n < _cells.Length)
				{
					_cells[i].CreateHtmlString(sb);
					if (_cells[i].ColumnSpan < 2)
					{
						n++;
					}
					else
					{
						n += _cells[i].ColumnSpan;
					}
					i++;
				}
			}
			sb.Append("</tr>");
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Cells.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(",");
				}
				sb.Append(Cells[i].Text);
			}
			return sb.ToString();
		}
	}
	public class LimnorWebTableRowCollection : List<LimnorWebTableRow>
	{
		public int _columnCount;
		public LimnorWebTableRowCollection()
		{
		}
		public void SetColumnSize(int n)
		{
			_columnCount = n;
			foreach (LimnorWebTableRow r in this)
			{
				r.SetSize(n);
			}
		}
	}
}
