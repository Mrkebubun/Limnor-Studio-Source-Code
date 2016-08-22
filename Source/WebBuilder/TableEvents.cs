using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limnor.WebBuilder
{
	public delegate void TableCellValueChanged(HtmlTable sender, int rowIndex, int cellIndex, string cellValue);
	public delegate void DatabaseLookupValuesSelected(HtmlTable sender, int cellIndex);
}
