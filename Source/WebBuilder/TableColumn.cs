using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using XmlUtility;
using VPL;
using System.Globalization;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class TableCellBase
	{
		#region constructors
		public TableCellBase()
		{
			Align = EnumAlign.Default;
			VerticalAlign = EnumVAlign.Default;
		}
		#endregion
		#region Properties
		[Description("Gets and sets the alignment of the column caption")]
		[DefaultValue(EnumAlign.Default)]
		public EnumAlign Align { get; set; }

		[Description("Gets and sets the vertical alignment of the column caption")]
		[DefaultValue(EnumVAlign.Default)]
		public EnumVAlign VerticalAlign { get; set; }

		[DefaultValue(null)]
		public Font TextFont { get; set; }

		[DefaultValue(null)]
		public Color TextColor { get; set; }

		[DefaultValue(null)]
		public Color BackColor { get; set; }

		[Description("Gets and sets an integer indicating how many columns this cell ocupies.")]
		[DefaultValue(0)]
		public int ColumnSpan { get; set; }
		//[DefaultValue(null)]
		//public string Image { get; set; }
		#endregion
		#region Methods

		protected abstract void OnCreateHtmlContect(XmlNode td);
		protected abstract void OnCreateHtmlAttributeString(StringBuilder sb);
		protected abstract void OnCreateHtmlContentString(StringBuilder sb);
		protected virtual void OnCreateHtmlContentStyle(StringBuilder sb) { }
		public void CreateHtmlContent(XmlNode tr)
		{
			XmlNode td = tr.OwnerDocument.CreateElement("td");
			tr.AppendChild(td);
			if (Align != EnumAlign.Default)
			{
				XmlUtil.SetAttribute(td, "align", Align);
			}
			if (VerticalAlign != EnumVAlign.Default)
			{
				XmlUtil.SetAttribute(td, "valign", VerticalAlign);
			}
			if (ColumnSpan > 1)
			{
				XmlUtil.SetAttribute(td, "colspan", ColumnSpan);
			}
			StringBuilder slc = new StringBuilder();
			if (TextFont != null)
			{
				slc.Append(ObjectCreationCodeGen.GetFontStyleString(TextFont));
			}
			if (TextColor != Color.Empty && TextColor != Color.Black)
			{
				slc.Append(" color:");
				slc.Append(ObjectCreationCodeGen.GetColorString(TextColor));
				slc.Append("; ");
			}
			if (BackColor != Color.Empty)// && BackColor != this.HeadBackColor)
			{
				slc.Append("background-color:");
				slc.Append(ObjectCreationCodeGen.GetColorString(BackColor));
				slc.Append("; ");
			}
			OnCreateHtmlContentStyle(slc);
			if (slc.Length > 0)
			{
				XmlUtil.SetAttribute(td, "style", slc.ToString());
			}
			OnCreateHtmlContect(td);
		}
		public virtual void CreateHtmlString(StringBuilder sb)
		{
			sb.Append("<td ");
			if (Align != EnumAlign.Default)
			{
				sb.Append("align=\"");
				sb.Append(Align);
				sb.Append("\" ");
			}
			if (VerticalAlign != EnumVAlign.Default)
			{
				sb.Append("valign=\"");
				sb.Append(VerticalAlign);
				sb.Append("\" ");
			}
			if (ColumnSpan > 1)
			{
				sb.Append("colspan=\"");
				sb.Append(ColumnSpan);
				sb.Append("\" ");
			}
			OnCreateHtmlAttributeString(sb);

			StringBuilder sl = new StringBuilder();
			if (TextFont != null)
			{
				sl.Append(ObjectCreationCodeGen.GetFontStyleString(TextFont));
			}
			if (TextColor != Color.Empty && TextColor != Color.Black)
			{
				sl.Append("color:");
				sl.Append(ObjectCreationCodeGen.GetColorString(TextColor));
				sl.Append("; ");
			}
			if (BackColor != Color.Empty)// && col.BackColor != this.HeadBackColor)
			{
				sl.Append("background-color:");
				sl.Append(ObjectCreationCodeGen.GetColorString(BackColor));
				sl.Append("; ");
			}
			OnCreateHtmlContentStyle(sl);
			if (sl.Length > 0)
			{
				sb.Append("style=\"");
				sb.Append(sl.ToString());
				sb.Append("\"");
			}
			sb.Append(">");
			OnCreateHtmlContentString(sb);
			sb.Append("</td>");
		}
		#endregion
	}
	public class HtmlTableCellCollectionBase<T> : List<T> where T : TableCellBase
	{
		public HtmlTableCellCollectionBase()
		{
		}
		public void CreateHtmlContent(XmlNode node)
		{
			XmlNode tr = node.OwnerDocument.CreateElement("tr");
			node.AppendChild(tr);
			int n = 0;
			while (n < this.Count)
			{
				this[n].CreateHtmlContent(tr);
				if (this[n].ColumnSpan < 2)
				{
					n++;
				}
				else
				{
					n += this[n].ColumnSpan;
				}
			}
		}
		public void CreateHtmlString(StringBuilder sb)
		{
			sb.Append("<tr>");
			int n = 0;
			while (n < this.Count)
			{
				this[n].CreateHtmlString(sb);
				if (this[n].ColumnSpan < 2)
				{
					n++;
				}
				else
				{
					n += this[n].ColumnSpan;
				}
			}
			sb.Append("</tr>");
		}
	}
	public class HtmlTableColumn : TableCellBase
	{
		public HtmlTableColumn()
		{
			Visible = true;
		}
		[Description("Gets and sets a Boolean indicating whether the column is visible")]
		[DefaultValue(true)]
		public bool Visible { get; set; }

		[Description("Gets and sets the field name when the table is bound to a database query")]
		[DefaultValue(null)]
		public string FieldName { get; set; }
		[Description("Gets and sets the text of the head of the column")]
		[DefaultValue(null)]
		public string Caption { get; set; }
		[Description("Gets and sets the width of a column of the table. It can be in pixels, i.e. 80px; or in percentage, i.e. 100%")]
		[DefaultValue(null)]
		public string Width { get; set; }

		[Description("If this property is set the True then data-bind text will be taken as HTML contents.")]
		[DefaultValue(false)]
		public bool DataBindAsHtml { get; set; }

		protected override void OnCreateHtmlContentStyle(StringBuilder sb)
		{
			if (!HtmlTable.UseFixedColumnWidths && !string.IsNullOrEmpty(Width))
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "width:{0}px;", Width));
			}
			if (!Visible)
			{
				sb.Append("display:none;");
			}
		}
		protected override void OnCreateHtmlContect(XmlNode td)
		{
			//if (!string.IsNullOrEmpty(Width))
			//{
			//    XmlUtil.SetAttribute(td, "width", Width);
			//}
			if (!string.IsNullOrEmpty(Caption))
			{
				td.InnerText = Caption;
			}
			XmlElement xe = (XmlElement)td;
			xe.IsEmpty = false;
		}
		protected override void OnCreateHtmlAttributeString(StringBuilder sb)
		{
			if (!string.IsNullOrEmpty(Width))
			{
				sb.Append("width=\"");
				sb.Append(Width);
				sb.Append("\" ");
			}
		}
		protected override void OnCreateHtmlContentString(StringBuilder sb)
		{
			if (!string.IsNullOrEmpty(Caption))
			{
				sb.Append(Caption);
			}
		}
		public override string ToString()
		{
			if (Caption == null)
			{
				return string.Empty;
			}
			return Caption;
		}
	}
	public class HtmlTableColumnCollection : HtmlTableCellCollectionBase<HtmlTableColumn>
	{
		public HtmlTableColumnCollection()
		{
		}
	}
}
