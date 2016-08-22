/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;

namespace Limnor.WebBuilder
{
	public class HtmlTableRow : HtmlClientObject
	{
		#region fields and constructors
		private HtmlTableLayout _table;
		public HtmlTableRow()//HtmlTableLayout tbl)
		{
			_table = new HtmlTableLayout();
		}
		#endregion

		#region Properties
		[Description("The table containing this row")]
		public HtmlTableLayout parent
		{
			get
			{
				return _table;
			}
		}
		#endregion

		#region IWebClientControl Members

		public override void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{

		}

		public override string ElementName
		{
			get { return "tr"; }
		}

		#endregion

	}
	public delegate void TableRowEventHandler(HtmlTableRow sender, object data);
}
