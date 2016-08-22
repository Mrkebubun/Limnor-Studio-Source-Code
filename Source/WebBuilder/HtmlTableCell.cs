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

namespace Limnor.WebBuilder
{
	public class HtmlTableCell : HtmlClientObject
	{
		#region fields and constructors
		private HtmlTableRow _row;
		public HtmlTableCell()
		{
			_row = new HtmlTableRow();
		}
		#endregion

		[Description("The row containing this cell")]
		public HtmlTableRow parentNode
		{
			get
			{
				return _row;
			}
		}
		public override string ElementName
		{
			get { return "td"; }
		}
	}
	public delegate void TableCellEventHandler(HtmlTableCell sender, object data);
}
