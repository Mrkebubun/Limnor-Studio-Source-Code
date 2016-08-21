/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using HtmlAgilityPack;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Xml;
using VPL;
using XmlSerializer;
using XmlUtility;

namespace LimnorDesigner.Web
{
	public class HtmlElement_div : HtmlElement_ItemBase, IWebClientInitializer, IHtmlElementCreateContents, IWebDataRepeater
	{
		#region fields and constructors
		const int PNUM = 5;
		public HtmlElement_div(ClassPointer owner)
			: base(owner)
		{
			IsRepeaterHolder = false;
		}
		public HtmlElement_div(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
			IsRepeaterHolder = false;
		}
		#endregion
		#region Properties
		public override string tagName
		{
			get { return "div"; }
		}
		private IComponent _dataSource;
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
				if (_dataSource != null && this.ElementGuid == Guid.Empty)
				{
					this.RootPointer.OnUseHtmlElement(this);
				}
			}
		}
		private bool _isrepeater = false;
		[Description("Gets and sets a Boolean indicating whether this DIV is used as a container for data repeaters. DataSource property must point to an EasyDataSet on the web page for a DIV to be a container for data repeaters.")]
		[DefaultValue(false)]
		[Category("Data Repeater")]
		public bool IsRepeaterHolder
		{
			get
			{
				return _isrepeater;
			}
			set
			{
				_isrepeater = value;
				if (_isrepeater && this.ElementGuid == Guid.Empty)
				{
					this.RootPointer.OnUseHtmlElement(this);
				}
			}
		}

		private string _displayStyle = string.Empty;
		[Description("Gets and sets a string indicating a display style when an item of data repeater is displayed. For example, keep it empty to use default display style or use a style identified by class; use inline-block to support multiple columns, etc.")]
		[DefaultValue("")]
		[Category("Data Repeater")]
		public string RepeaterDisplayStyle
		{
			get
			{
				return _displayStyle;
			}
			set
			{
				_displayStyle = value;
				if (!string.IsNullOrEmpty(_displayStyle) && this.ElementGuid == Guid.Empty)
				{
					this.RootPointer.OnUseHtmlElement(this);
				}
			}
		}

		private int _columnCount = 1;
		[Category("Data Repeater")]
		[DefaultValue(1)]
		[Description("Gets and sets the number of columns")]
		public int ColumnCount
		{
			get
			{
				return _columnCount;
			}
			set
			{
				if (value > 0)
				{
					_columnCount = value;
					if (_columnCount != 1 && this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
		}
		private int _rowCount = 3;
		[Category("Data Repeater")]
		[DefaultValue(3)]
		[Description("Gets and sets the number of rows")]
		public int RowCount
		{
			get
			{
				return _rowCount;
			}
			set
			{
				if (value > 0)
				{
					_rowCount = value;
					if (_rowCount != 3 && this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
		}
		private bool _autoColRow = true;
		[DefaultValue(true)]
		[Category("Data Repeater")]
		[Description("Gets and sets a Boolean indicating whether number of columns is determined by width of data repeater holder, i.e. a web page. If this property is True then properties ColumnCount and RowCount are not used to specify columns and rows; ColumnCount * RowCount calculates records displayed.")]
		public bool AutoColumnsAndRows
		{
			get
			{
				return _autoColRow;
			}
			set
			{
				_autoColRow = value;
				if (!_autoColRow && this.ElementGuid == Guid.Empty)
				{
					this.RootPointer.OnUseHtmlElement(this);
				}
			}
		}
		private bool _showAllRecs = false;
		[Category("Data Repeater")]
		[Description("Gets and sets a Boolean indicating whether all records are to be displayed in one page.")]
		[DefaultValue(false)]
		public bool ShowAllRecords 
		{
			get
			{
				return _showAllRecs;
			}
			set
			{
				_showAllRecs = value;
				if (_showAllRecs && this.ElementGuid == Guid.Empty)
				{
					this.RootPointer.OnUseHtmlElement(this);
				}
			}
		}
		private int _pnp = PNUM;
		[Category("Data Repeater")]
		[DefaultValue(PNUM)]
		[WebClientMember]
		[Description("Gets and sets the number of page numbers displayed on the page navigator")]
		public int PageNavigatorPages
		{
			get
			{
				return _pnp;
			}
			set
			{
				_pnp = value;
				if (_pnp != PNUM)
				{
					if (this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
		}
		private EnumPageNavigatorPosition _navigatorLocation = EnumPageNavigatorPosition.TopAndBottom;
		[Category("Data Repeater")]
		[Description("Gets and sets the location of the page navigator")]
		[DefaultValue(EnumPageNavigatorPosition.TopAndBottom)]
		public EnumPageNavigatorPosition PageNavigatorLocation
		{
			get
			{
				return _navigatorLocation;
			}
			set
			{
				_navigatorLocation = value;
				if (_navigatorLocation != EnumPageNavigatorPosition.TopAndBottom)
				{
					if (this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
		}
		private Font _navigateFont = null;
		[Category("Data Repeater")]
		[WebClientMember]
		[Description("Gets and sets the font of the page navigator")]
		public Font PageNavigatorFont
		{
			get
			{
				return _navigateFont;
			}
			set
			{
				_navigateFont = value;
				if (_navigateFont != null)
				{
					if (this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XML_ATTR_DataSource = "dataSource";
		const string XML_ATTR_rcount = "rowcount";
		const string XML_ATTR_ccount = "colcount";
		const string XML_ATTR_repeater = "repeater";
		const string XML_ATTR_autoCR = "itemsfloat";
		const string XML_ATTR_showall = "showall";
		const string XML_ATTR_pagenum = "pgnum";
		const string XML_ATTR_pageloc = "pgloc";
		const string XML_ATTR_pnfont = "pgfont";
		const string XML_ATTR_dispStyle = "dispstyle";
		[Browsable(false)]
		[NotForProgramming]
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			string refName = XmlUtil.GetAttribute(node, XML_ATTR_DataSource);
			if (!string.IsNullOrEmpty(refName))
			{
				XmlObjectReader xr = serializer as XmlObjectReader;
				if (xr != null)
				{
					xr.AddDelayedInitializer(this, node);
				}
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_dispStyle))
			{
				_displayStyle = XmlUtil.GetAttribute(node, XML_ATTR_dispStyle);
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_rcount))
			{
				_rowCount = XmlUtil.GetAttributeInt(node, XML_ATTR_rcount);
			}
			IsRepeaterHolder = XmlUtil.GetAttributeBoolDefFalse(node, XML_ATTR_repeater);
			if (XmlUtil.HasAttribute(node, XML_ATTR_ccount))
			{
				_columnCount = XmlUtil.GetAttributeInt(node, XML_ATTR_ccount);
			}

			if (XmlUtil.HasAttribute(node, XML_ATTR_autoCR))
			{
				_autoColRow = XmlUtil.GetAttributeBoolDefTrue(node, XML_ATTR_autoCR);
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_showall))
			{
				_showAllRecs = XmlUtil.GetAttributeBoolDefFalse(node, XML_ATTR_showall);
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_pagenum))
			{
				_pnp = XmlUtil.GetAttributeInt(node, XML_ATTR_pagenum);
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_pageloc))
			{
				_navigatorLocation = XmlUtil.GetAttributeEnum<EnumPageNavigatorPosition>(node, XML_ATTR_pageloc);
			}
			if (XmlUtil.HasAttribute(node, XML_ATTR_pnfont))
			{
				_navigateFont = XmlUtil.GetAttributeFont(node, XML_ATTR_pnfont);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			if (_dataSource != null && _dataSource.Site != null && !string.IsNullOrEmpty(_dataSource.Site.Name))
			{
				XmlUtil.SetAttribute(node, XML_ATTR_DataSource, _dataSource.Site.Name);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_DataSource);
			}
			if (IsRepeaterHolder)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_repeater, IsRepeaterHolder);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_repeater);
			}
#if NET35
			if(string.IsNullOrEmpty(_displayStyle))
#else
			if (string.IsNullOrWhiteSpace(_displayStyle))
#endif
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_dispStyle);
			}
			else
			{
				XmlUtil.SetAttribute(node, XML_ATTR_dispStyle, _displayStyle);
			}
			if (_rowCount != 3)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_rcount, _rowCount);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_rcount);
			}
			if (_columnCount != 1)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_ccount, _columnCount);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_ccount);
			}
			if (_autoColRow)
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_autoCR);
			}
			else
			{
				XmlUtil.SetAttribute(node, XML_ATTR_autoCR, _autoColRow);
			}
			if (_showAllRecs)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_showall, _showAllRecs);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_showall);
			}
			if (_pnp != PNUM)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_pagenum, _pnp);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_pagenum);
			}
			if (_navigatorLocation != EnumPageNavigatorPosition.TopAndBottom)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_pageloc, _navigatorLocation);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_pageloc);
			}
			if (_navigateFont != null)
			{
				XmlUtil.SetAttributeFont(node, XML_ATTR_pnfont, _navigateFont);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_pnfont);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			base.OnDelayedPostSerialize(objMap, objectNode, reader);
			string refName = XmlUtil.GetAttribute(objectNode, XML_ATTR_DataSource);
			if (!string.IsNullOrEmpty(refName))
			{
				IComponent ic = RootPointer.GetComponentByName(refName);
				if (ic != null)
				{
					_dataSource = ic;
				}
			}
		}
		#endregion
		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			IDataSetSource ids = DataSource as IDataSetSource;
			if (this.IsRepeaterHolder && ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				sc.Add("\r\n");
				//
				sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.IsDataRepeater=true;\r\n", CodeName));
				if (ShowAllRecords)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ShowAllRecords=true;\r\n", CodeName));
				}
				if (this.AutoColumnsAndRows)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.AutoColumnsAndRows=true;\r\n", CodeName));
				}
				if (_displayStyle != null)
				{
					_displayStyle = _displayStyle.Trim();
				}
				if (string.IsNullOrEmpty(_displayStyle))
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.displayStyle='';\r\n", CodeName));
				}
				else
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.displayStyle='{1}';\r\n", CodeName, _displayStyle));
				}
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.repeatColumnCount={1};\r\n", CodeName, _columnCount));
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData=JsonDataBinding.DataRepeater({0},null);\r\n", CodeName));
				if (PageNavigatorPages != PNUM)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.setNavigatorPages({1});\r\n", CodeName, PageNavigatorPages));
				}
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
		}
		#endregion
		#region IHtmlElementCreateContents
		private void createPageNavigator(HtmlNode node, int pos)
		{
			HtmlNode nd;
			if (this.AutoColumnsAndRows && pos == 1)
			{
				nd = node.OwnerDocument.CreateElement("br");
				node.AppendChild(nd);
			}
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			nd.SetAttributeValue("type", "button");
			nd.SetAttributeValue("value", "|<");
			nd.SetAttributeValue("onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoFirstPage();", this.CodeName));
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			nd.SetAttributeValue("type", "button");
			nd.SetAttributeValue("value", "<");
			nd.SetAttributeValue("onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoPrevPage();", this.CodeName));
			//
			nd = node.OwnerDocument.CreateElement("span");
			node.AppendChild(nd);
			nd.SetAttributeValue("id", string.Format(CultureInfo.InvariantCulture, "{0}_sp{1}", this.CodeName, pos));
			nd.SetAttributeValue("name", string.Format(CultureInfo.InvariantCulture, "{0}_sp", this.CodeName));
			StringBuilder sb = new StringBuilder();
			if (PageNavigatorFont != null)
			{
				sb.Append(ObjectCreationCodeGen.GetFontStyleString(PageNavigatorFont));
			}
			sb.Append("color:blue;");
			nd.SetAttributeValue("style", sb.ToString());
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			nd.SetAttributeValue("type", "button");
			nd.SetAttributeValue("value", ">");
			nd.SetAttributeValue("onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoNextPage();", this.CodeName));
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			nd.SetAttributeValue("type", "button");
			nd.SetAttributeValue("value", ">|");
			nd.SetAttributeValue("onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoLastPage();", this.CodeName));
			if (this.AutoColumnsAndRows && pos == 0)
			{
				nd = node.OwnerDocument.CreateElement("br");
				node.AppendChild(nd);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node">a td</param>
		/// <param name="index"></param>
		/// <returns></returns>
		private HtmlNode createGroup(HtmlNode node, HtmlNode itemNode, int index)
		{
			HtmlNode divNode = itemNode.CloneNode(true);
			divNode.Attributes.Remove("id");
			divNode.SetStyleValue("display", "none");
			if (index >= 0)
			{
				divNode.SetAttributeValue("name", this.CodeName);
			}
			foreach (HtmlNode ct in divNode.ChildNodes)
			{
				createControlWebContents(ct, index, divNode);
			}
			node.AppendChild(divNode);
			return divNode;
		}
		private void createControlWebContents(HtmlNode ct,int index,HtmlNode divNode)
		{
			if (!string.IsNullOrEmpty(ct.Id))
			{
				if (index >= 0)
				{
					ct.Id = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", ct.Id, index);
				}
			}
			foreach (HtmlNode c in ct.ChildNodes)
			{
				createControlWebContents(c, index, divNode);
			}
		}
		public void CreateHtmlContent(HtmlNode node)
		{
			IDataSetSource ids = DataSource as IDataSetSource;
			if (IsRepeaterHolder && ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				node.SetAttributeValue("jsdb", ids.TableName);
				//
				if (_columnCount > 0 && _rowCount > 0)
				{
					HtmlNode parent = node.ParentNode;
					if (parent != null)
					{
						//create a new div to hold the items
						string id = node.Id;
						HtmlNode repeaterNode = parent.OwnerDocument.CreateElement("div");
						parent.InsertBefore(repeaterNode, node);
						parent.RemoveChild(node);
						node.Id = "";
						repeaterNode.Id = id;
						repeaterNode.SetAttributeValue("id",id);
						repeaterNode.SetAttributeValue("jsdb",node.GetAttributeValue("jsdb",""));
						node.Attributes.Remove("jsdb");
						//create page navigator buttons on top
						if (this.PageNavigatorLocation == EnumPageNavigatorPosition.Top || this.PageNavigatorLocation == EnumPageNavigatorPosition.TopAndBottom)
						{
							createPageNavigator(repeaterNode, 0);
						}
						//create items
						HtmlNode tblNode=null;
						if (!this.AutoColumnsAndRows)
						{
							tblNode = repeaterNode.OwnerDocument.CreateElement("table");
							repeaterNode.AppendChild(tblNode);
							tblNode.SetAttributeValue("border", "0");
							tblNode.SetAttributeValue("width", "100%");
							tblNode.SetAttributeValue("cellpadding", "0");
							tblNode.SetAttributeValue("cellspacing", "0");
							tblNode.SetAttributeValue("style", "padding:0px;border-collapse:collapse; border-spacing: 0;");
						}
						//
						int index = 0;
						for (int r = 0; r < _rowCount; r++)
						{
							HtmlNode trNode=null;
							if (tblNode != null)
							{
								trNode = parent.OwnerDocument.CreateElement("tr");
								tblNode.AppendChild(trNode);
							}
							for (int c = 0; c < _columnCount; c++)
							{
								HtmlNode holdNd;
								if (tblNode == null)
								{
									holdNd = repeaterNode;
								}
								else
								{
									holdNd = parent.OwnerDocument.CreateElement("td");
									trNode.AppendChild(holdNd);
								}
								createGroup(holdNd, node, index);
								index++;
							}
						}
						//create page navigator buttons on bottom
						if (this.PageNavigatorLocation == EnumPageNavigatorPosition.Bottom || this.PageNavigatorLocation == EnumPageNavigatorPosition.TopAndBottom)
						{
							createPageNavigator(repeaterNode, 1);
						}
						//create template
						createGroup(repeaterNode, node, -1);
					}
				}
			}
		}
		#endregion
		public string GetElementGetter()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"document.getElementById('{0}').jsData.getElement", this.id);
		}
	}
}
