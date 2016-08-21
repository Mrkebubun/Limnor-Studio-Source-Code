/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;
using Limnor.WebServerBuilder;
using System.Drawing.Design;
using Limnor.WebBuilder;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using System.Collections.Specialized;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LimnorDesigner.Web
{
	public class HtmlElement_table : HtmlElement_ItemBase, IDataSourceBinder, IWebClientInitializer, IUseDatetimePicker, IWebDataEditorsHolder, IUseJavascriptFiles
	{
		#region fields and constructors
		//background colors
		private Color _highlightCellColor;
		private Color _highlightRowColor;
		private Color _selectRowColor;
		private Color _alternateColor;
		//
		private bool _useDatetimePicker;
		public HtmlElement_table(ClassPointer owner)
			: base(owner)
		{
			init();
		}
		public HtmlElement_table(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
			init();
		}
		private void init()
		{
			_highlightCellColor = Color.FromArgb(255, 255, 192);
			_highlightRowColor = Color.FromArgb(192, 255, 192);
			_selectRowColor = Color.FromArgb(192, 192, 255);
		}
		private void validatingEditors()
		{
			if (_editorList == null)
			{
				_editorList = new WebDataEditorList(this);
			}
			else
			{
				int n = FieldCount;
				StringCollection fieldNames = new StringCollection();
				for (int j = 0; j < n; j++)
				{
					fieldNames.Add(GetFieldNameByIndex(j).ToLowerInvariant());
				}
				//remove invalid editors
				StringCollection sc = new StringCollection();
				for (int i = 0; i < _editorList.Count; i++)
				{
					bool isValid = false;
					if (!string.IsNullOrEmpty(_editorList[i].ValueField))
					{
						string s = _editorList[i].ValueField.ToLowerInvariant();
						isValid = fieldNames.Contains(s);
					}
					if (!isValid)
					{
						sc.Add(_editorList[i].ValueField);
					}
				}
				for (int i = 0; i < sc.Count; i++)
				{
					_editorList.RemoveEditorByName(sc[i]);
				}
				//add missing editors
				for (int j = 0; j < n; j++)
				{
					if (_editorList.GetEditorByName(fieldNames[j]) == null)
					{
						WebDataEditorNone wn = new WebDataEditorNone();
						wn.ValueField = GetFieldNameByIndex(j);
						_editorList.AddEditor(wn);
					}
				}
			}
		}

		#endregion

		#region Action Buttons
		[DefaultValue(0)]
		[Description("Gets and sets column width, in pixels, for the action column")]
		public int ActionColumnWidth { get; set; }
		private CollectionComponentNames _acs;
		[RefreshProperties(RefreshProperties.All)]
		[Description("Select web controls to be displayed at the right end of the selected row")]
		public CollectionComponentNames ActionControls
		{
			get
			{
				if (_acs == null)
				{
					_acs = new CollectionComponentNames();
					_acs.SetScope(typeof(IWebClientControl), this);
				}
				return _acs;
			}
			set
			{
				_acs = value;
				if (_acs != null)
				{
					_acs.SetScope(typeof(IWebClientControl), this);
				}
			}
		}
		#endregion

		#region IUseDatetimePicker Members
		[Browsable(false)]
		public bool UseDatetimePicker
		{
			get { return _useDatetimePicker; }
		}

		#endregion

		#region IWebDataEditorsHolder Members

		public int FieldCount
		{
			get
			{
				IDataSetSource ds = DataSource as IDataSetSource;
				if (ds != null)
				{
					return ds.FieldCount;
				}
				return 0;
			}
		}
		public string GetFieldNameByIndex(int index)
		{
			IDataSetSource ds = DataSource as IDataSetSource;
			if (ds != null)
			{
				return ds.GetFieldNameByIndex(index);
			}
			return string.Empty;
		}
		public WebDataEditor GetWebDataEditor(string fieldName)
		{
			return FieldEditors.GetEditorByName(fieldName);
		}
		public void OnEditorChanged(string fieldName)
		{
		}
		#endregion

		#region Properties
		public override string tagName
		{
			get { return "table"; }
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
				updateTableHtml();
			}
		}

		[DefaultValue(false)]
		[WebClientMember]
		public bool ReadOnly { get; set; }

		private int _dpftsize = 12;
		[NotForProgramming]
		[DefaultValue(12)]
		[Description("Gets and sets font size, in pixels, for date-picker used in field editors. Use 16 for large dialogue. Use 10 for small dialogue. You may adjust the number to fit your desired size.")]
		public int DatePickerFonstSize
		{
			get
			{
				return _dpftsize;
			}
			set
			{
				if (value > 3 && value < 50)
				{
					_dpftsize = value;
				}
			}
		}

		private WebDataEditorList _editorList;
		[Browsable(false)] //TBD: datetimepicker not showing correct button and dialog size
		[Category("Database")]
		[Description("Field editors for editing cells at runtime by the user.")]
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		public WebDataEditorList FieldEditors
		{
			get
			{
				validatingEditors();
				return _editorList;
			}
		}

		[WebClientMember]
		[Description("Gets and sets alternate background color for rows when data binding is applied to the table.")]
		public Color AlternateBackgroundColor
		{
			get
			{
				return _alternateColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_alternateColor = value;
				}
			}
		}

		[WebClientMember]
		[Description("Gets and sets cell background color for highlighted cell when data binding is applied to the table.")]
		public Color HighlightCellColor
		{
			get
			{
				return _highlightCellColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_highlightCellColor = value;
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets row background color for highlighted row when data binding is applied to the table.")]
		public Color HighlightRowColor
		{
			get
			{
				return _highlightRowColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_highlightRowColor = value;
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets row background color for selected row when data binding is applied to the table.")]
		public Color SelectedRowColor
		{
			get
			{
				return _selectRowColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_selectRowColor = value;
				}
			}
		}
		#endregion

		#region Methods
		private void updateTableHtml()
		{
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

		#region IXmlNodeSerializable Members
		const string XML_ATTR_DataSource = "dataSource";
		const string XML_ATTR_ActColSize = "actColSize";
		const string XML_ATTR_ACTS = "acts";
		const string XML_ATT_readonly = "readonly";
		const string XML_ATT_dpfsize = "dpfsize";
		const string XML_ATT_altColor = "altcolor";
		const string XML_ATT_htColor = "htcolor";
		const string XML_ATT_hrColor = "hrcolor";
		const string XML_ATT_srColor = "srcolor";
		const string XML_Editors = "Editors";
		private Color readColor(XmlNode node, string name)
		{
			string s = XmlUtil.GetAttribute(node, name);
			if (!string.IsNullOrEmpty(s))
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				object v = converter.ConvertFromInvariantString(s);
				if (v != null)
				{
					Color c = (Color)v;
					return c;
				}
			}
			return Color.Empty;
		}
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
			ActionColumnWidth = XmlUtil.GetAttributeInt(node, XML_ATTR_ActColSize);
			string s = XmlUtil.GetAttribute(node, XML_ATTR_ACTS);
			if (!string.IsNullOrEmpty(s))
			{
				string[] ss = s.Split(';');
				for (int i = 0; i < ss.Length; i++)
				{
					ActionControls.AddComponent(ss[i]);
				}
			}
			if (XmlUtil.GetAttributeBoolDefFalse(node, XML_ATT_readonly))
			{
				this.ReadOnly = true;
			}
			DatePickerFonstSize = XmlUtil.GetAttributeInt(node, XML_ATT_dpfsize);
			XmlNodeList nds = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,"{0}/{1}", XML_Editors,XmlTags.XML_Item));
			if (nds != null && nds.Count > 0)
			{
				WebDataEditor[] eds = new WebDataEditor[nds.Count];
				serializer.ReadArray(nds, eds, this);
				_editorList = new WebDataEditorList();
				_editorList.SetFields(this);
				for (int i = 0; i < eds.Length; i++)
				{
					eds[i].SetHolder(this);
					_editorList.Add(eds[i]);
				}
			}
			Color c = readColor(node, XML_ATT_altColor);
			if (c != Color.Empty)
			{
				_alternateColor = c;
			}
			c = readColor(node, XML_ATT_htColor);
			if (c != Color.Empty)
			{
				_highlightCellColor = c;
			}
			c = readColor(node, XML_ATT_hrColor);
			if (c != Color.Empty)
			{
				_highlightRowColor = c;
			}
			c = readColor(node, XML_ATT_srColor);
			if (c != Color.Empty)
			{
				_selectRowColor = c;
			}
		}
		private void writeColor(XmlNode node, Color value, string name)
		{
			if (value != Color.Empty)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(value);
				string txt = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
				if (!string.IsNullOrEmpty(txt))
				{
					txt = txt.Trim();
					if (!string.IsNullOrEmpty(txt))
					{
						XmlUtil.SetAttribute(node, name, txt);
					}
				}
			}
			else
			{
				XmlUtil.RemoveAttribute(node, name);
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
			if (ActionColumnWidth > 0)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_ActColSize, ActionColumnWidth);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_ActColSize);
			}
			if (_acs != null && _acs.Count > 0)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_ACTS, _acs.ToStringForm());
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_ACTS);
			}
			if (this.ReadOnly)
			{
				XmlUtil.SetAttribute(node, XML_ATT_readonly, this.ReadOnly);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATT_readonly);
			}
			if (_dpftsize != 12)
			{
				XmlUtil.SetAttribute(node, XML_ATT_dpfsize, _dpftsize);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATT_dpfsize);
			}
			if (_editorList != null && _editorList.Count > 0)
			{
				XmlNode nds = serializer.CreateSingleNewElement(node, XML_Editors);
				WebDataEditor[] eds = _editorList.ToArray();
				serializer.WriteArray(nds, eds);
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XML_Editors);
				if (nd != null)
				{
					node.RemoveChild(nd);
				}
			}
			writeColor(node, _alternateColor, XML_ATT_altColor);
			writeColor(node, _highlightCellColor, XML_ATT_htColor);
			writeColor(node, _highlightRowColor, XML_ATT_hrColor);
			writeColor(node, _selectRowColor, XML_ATT_srColor);
		}
		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			ClassPointer cp = this.Owner as ClassPointer;
			//
			if (_resourceFiles == null)
				_resourceFiles = new List<WebResourceFile>();
			bool b;
			string btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "dropdownbutton.jpg");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "calendar.jpg");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "datepicker.css");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Css, out b));
			}
			string[] jsFiles = new string[14];
			jsFiles[0] = "backstripes.gif";
			jsFiles[1] = "bg_header.jpg";
			jsFiles[2] = "bullet1.gif";
			jsFiles[3] = "bullet2.gif";
			jsFiles[4] = "cal.gif";
			jsFiles[5] = "cal-grey.gif";
			jsFiles[6] = "datepicker.js";
			jsFiles[7] = "gradient-e5e5e5-ffffff.gif";
			jsFiles[8] = "ok.png";
			jsFiles[9] = "cancel.png";
			jsFiles[10] = "qry.jpg";
			jsFiles[11] = "chklist.jpg";
			jsFiles[12] = "plus.gif";
			jsFiles[13] = "minus.gif";
			for (int i = 0; i < jsFiles.Length; i++)
			{
				btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), jsFiles[i]);
				if (File.Exists(btimg))
				{
					WebResourceFile w = new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Javascript, out b);
					_resourceFiles.Add(w);
					string tgt = Path.Combine(Path.Combine(cp.Project.WebPhysicalFolder(null), w.WebFolder), Path.GetFileName(btimg));
					File.Copy(btimg, tgt, true);
				}
			}
			//
			sc.Add("\r\n");
			sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
			if (this.ReadOnly)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ReadOnly=true;\r\n", CodeName));
			}
			if (_highlightCellColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.HighlightCellColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_highlightCellColor)));
			}
			if (_alternateColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.AlternateBackgroundColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_alternateColor)));
			}
			if (_highlightRowColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.HighlightRowColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_highlightRowColor)));
			}
			if (_selectRowColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.SelectedRowColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_selectRowColor)));
			}
			if (DatePickerFonstSize != 12)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.DatePickerFonstSize={1};\r\n", CodeName, DatePickerFonstSize));
			}
			IDataSetSource ds = DataSource as IDataSetSource;
			if (ds != null)
			{
				string[] flds = ds.GetReadOnlyFields();
				if (flds != null && flds.Length > 0)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.ReadOnlyFields = [\r\n", CodeName));
					for (int i = 0; i < flds.Length; i++)
					{
						if (i > 0)
						{
							sc.Add(",");
						}
						sc.Add("'");
						sc.Add(flds[i]);
						sc.Add("'");
					}
					sc.Add("\r\n];\r\n");
				}
			}
			_useDatetimePicker = false;
			if (_editorList != null)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.FieldEditors = {{}};\r\n", CodeName));
				for (int i = 0; i < FieldCount; i++)
				{
					string cn = this.GetFieldNameByIndex(i);
					for (int j = 0; j < _editorList.Count; j++)
					{
						if (_editorList[j] != null && !(_editorList[j] is WebDataEditorNone))
						{
							if (string.Compare(cn, _editorList[j].ValueField, StringComparison.OrdinalIgnoreCase) == 0)
							{
								string s = _editorList[j].CreateJavascriptEditor(sc);
								sc.Add(string.Format(CultureInfo.InvariantCulture,
									"{0}.FieldEditors[{1}] = {2};\r\n",
									CodeName, i, s));
								if (_editorList[j] is WebDataEditorDatetime)
								{
									_useDatetimePicker = true;
								}
								break;
							}
						}
					}
				}
			}
			if (ActionControls.Count > 0)
			{
				bool first = true;
				StringBuilder sbAc = new StringBuilder();
				for (int j = 0; j < ActionControls.Count; j++)
				{
					string nmAct = ActionControls[j];
					if (!string.IsNullOrEmpty(nmAct))
					{
						if (first)
							first = false;
						else
							sbAc.Append(",");
						sbAc.Append(string.Format(CultureInfo.InvariantCulture, "'{0}'", nmAct));
					}
				}
				if (!first)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.ActControls = [{1}];\r\n", CodeName, sbAc.ToString()));
					if (ActionColumnWidth > 0)
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.ActColWidth={1};\r\n", CodeName, ActionColumnWidth));
					}
				}
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
		}
		#endregion

		#region IUseJavascriptFiles Members

		public IList<string> GetJavascriptFiles()
		{
			List<string> l = new List<string>();
			l.Add("datepicker.js");
			return l;
		}

		#endregion
	}
}
