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
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Specialized;
using XmlUtility;
using System.Xml;
using System.Reflection;
using System.Globalization;
using System.Drawing.Design;
using System.IO;
using VPL;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlContent), "Resources.htmlEditor.bmp")]
	[Description("This is a visual HTML editor.")]
	public class HtmlRichTextEditor : UserControl, IWebClientControl, ICustomTypeDescriptor, IJsFilesResource, IWebClientInitializer
	{
		#region constructors and fields
		private System.ComponentModel.IContainer components = null;
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlRichTextEditor()
		{
			this.components = new System.ComponentModel.Container();
			_resourceFiles = new List<WebResourceFile>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			this.BorderStyle = BorderStyle.FixedSingle;
			this.BackColor = Color.LightGray;
		}
		static HtmlRichTextEditor()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("HeightInPercent");
			_propertyNames.Add("WidthInPercent");
			_propertyNames.Add("Visible");
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion
		#region IXmlNodeHolder Members

		private XmlNode _dataNode;
		[Browsable(false)]
		public XmlNode DataXmlNode { get { return _dataNode; } set { _dataNode = value; } }

		#endregion
		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		private string _vaname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vaname = vname;
		}
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

		private int _opacity = 100;
		[DefaultValue(100)]
		[Description("Gets and sets the opacity of the control. 0 is transparent. 100 is full opacity")]
		public int Opacity
		{
			get
			{
				if (_opacity < 0 || _opacity > 100)
				{
					_opacity = 100;
				}
				return _opacity;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_opacity = value;
				}
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return null;}// _htmlParts; }
		}

		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if(_dataNode != null)
				return XmlUtil.GetNameAttribute(_dataNode);
				return _vaname;
			}
		}

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			return name;
		}

		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return new MethodInfo[] { };
		}

		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			return new EventInfo[] { };
		}

		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
		}
		[Browsable(false)]
		public virtual Dictionary<string, string> DataBindNameMap
		{
			get
			{
				Dictionary<string, string> map = new Dictionary<string, string>();
				map.Add("Text", "innerHTML");
				return map;
			}
		}
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			if (_resourceFiles == null)
			{
				_resourceFiles = new List<WebResourceFile>();
			}
			string[] jsFiles = new string[120];
			jsFiles[0] = "go.gif";
			jsFiles[1] = "go_inact.gif";
			jsFiles[2] = "folder.gif";
			jsFiles[3] = "resizer.gif";
			jsFiles[4] = "newElement.png";
			jsFiles[5] = "moveup.png";
			jsFiles[6] = "movedown.png";
			jsFiles[7] = "undo.png";
			jsFiles[8] = "undo_inact.png";
			jsFiles[9] = "redo.png";
			jsFiles[10] = "redo_inact.png";
			jsFiles[11] = "stopTag.png";
			jsFiles[12] = "stopTag_inact.png";
			jsFiles[13] = "expand_res.png";
			jsFiles[14] = "expand_min.png";
			jsFiles[15] = "cancel.png";
			//
			jsFiles[16] = "newAttr.png";
			jsFiles[17] = "removeTag.png";
			jsFiles[18] = "removeOutTag.png";
			jsFiles[19] = "vSep.png";
			jsFiles[20] = "resizeV.png";
			jsFiles[21] = "ok.png";
			jsFiles[22] = "save.png";
			jsFiles[23] = "reset.png";
			jsFiles[24] = "space.png";
			jsFiles[25] = "tglProp.png";
			jsFiles[26] = "elementLocator.png";
			jsFiles[27] = "redbox.png";
			//
			jsFiles[28] = "validate.png";
			jsFiles[29] = "addmeta.png";
			jsFiles[30] = "addscript.png";
			jsFiles[31] = "addcss.png";
			jsFiles[32] = "addlink.png";
			jsFiles[33] = "tableHead.png";
			jsFiles[34] = "tableFoot.png";
			jsFiles[35] = "addthead.png";
			jsFiles[36] = "addtfoot.png";
			jsFiles[37] = "html_col.png";
			jsFiles[38] = "newAbove.png";
			jsFiles[39] = "newBelow.png";
			//
			jsFiles[40] = "newLeft.png";
			jsFiles[41] = "newRight.png";
			jsFiles[42] = "mergeLeft.png";
			jsFiles[43] = "mergeRight.png";
			jsFiles[44] = "mergeAbove.png";
			jsFiles[45] = "mergeBelow.png";
			jsFiles[46] = "splitH.png";
			jsFiles[47] = "splitV.png";
			jsFiles[48] = "html_img.png";
			jsFiles[49] = "newDefinition.png";
			jsFiles[50] = "newOptionGroup.png";
			jsFiles[51] = "html_h1.png";
			jsFiles[52] = "html_h2.png";
			jsFiles[53] = "html_h3.png";
			jsFiles[54] = "html_h4.png";
			jsFiles[55] = "html_h5.png";
			jsFiles[56] = "html_h6.png";
			//
			jsFiles[57] = "addarea.png";
			jsFiles[58] = "newlegend.png";
			jsFiles[59] = "addparam.png";
			jsFiles[60] = "moveleft.png";
			jsFiles[61] = "moveright.png";
			jsFiles[62] = "html_a.png";
			jsFiles[63] = "html_bdo.png";
			jsFiles[64] = "html_blockquote.png";
			jsFiles[65] = "html_button.png";
			jsFiles[66] = "html_checkbox.png";
			jsFiles[67] = "html_cite.png";
			jsFiles[68] = "html_code.png";
			jsFiles[69] = "html_dl.png";
			jsFiles[70] = "html_div.png";
			jsFiles[71] = "html_fieldset.png";
			jsFiles[72] = "html_file.png";
			//
			jsFiles[73] = "html_form.png";
			jsFiles[74] = "html_h.png";
			jsFiles[75] = "html_hr.png";
			jsFiles[76] = "html_hidden.png";
			jsFiles[77] = "html_iframe.png";
			jsFiles[78] = "html_a.png";
			jsFiles[79] = "html_input.png";
			jsFiles[80] = "html_kbd.png";
			jsFiles[81] = "html_label.png";
			jsFiles[82] = "html_br.png";
			jsFiles[83] = "html_ul.png";
			jsFiles[84] = "html_select.png";
			jsFiles[85] = "html_map.png";
			jsFiles[86] = "html_object.png";
			jsFiles[87] = "html_ol.png";
			jsFiles[88] = "html_p.png";
			//
			jsFiles[89] = "html_password.png";
			jsFiles[90] = "html_pre.png";
			jsFiles[91] = "html_span.png";
			jsFiles[92] = "html_radio.png";
			jsFiles[93] = "html_reset.png";
			jsFiles[94] = "html_samp.png";
			jsFiles[95] = "html_submit.png";
			jsFiles[96] = "html_table.png";
			jsFiles[97] = "html_textarea.png";
			jsFiles[98] = "html_var.png";
			jsFiles[99] = "html_menu.png";
			jsFiles[100] = "html_treeview.png";
			jsFiles[101] = "html_embed.png";
			jsFiles[102] = "html_music.png";
			jsFiles[103] = "html_video.png";
			jsFiles[104] = "bold_act.png";
			//
			jsFiles[105] = "italic_act.png";
			jsFiles[106] = "underline_act.png";
			jsFiles[107] = "strikethrough_act.png";
			jsFiles[108] = "subscript_act.png";
			jsFiles[109] = "superscript_act.png";
			jsFiles[110] = "alignLeft.png";
			jsFiles[111] = "alignCenter.png";
			jsFiles[112] = "alignRight.png";
			jsFiles[113] = "indent.png";
			jsFiles[114] = "dedent.png";
			jsFiles[115] = "editor.png";
			//
			jsFiles[116] = "arrow.gif";
			jsFiles[117] = "cross.gif";
			jsFiles[118] = "hs.png";
			jsFiles[119] = "hv.png";
			//
			string btimg;
			string dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "libjs");
			for (int i = 0; i < jsFiles.Length; i++)
			{
				btimg = Path.Combine(dir, jsFiles[i]);
				if (File.Exists(btimg))
				{
					bool b;
					_resourceFiles.Add(new WebResourceFile(btimg, "libjs", out b));
				}
			}
			//
			StringBuilder sb = new StringBuilder();
			//
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			//
			if (_dataNode != null)
			{
				XmlNode pNode = _dataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@name='Visible']", XmlTags.XML_PROPERTY));
				if (pNode != null)
				{
					string s = pNode.InnerText;
					if (!string.IsNullOrEmpty(s))
					{
						try
						{
							bool b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (!b)
							{
								sb.Append("display:none; ");
							}
						}
						catch
						{
						}
					}
				}
			}
			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
			XmlElement xe = (XmlElement)node;
			xe.IsEmpty = false;
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		public string ElementName
		{
			get { return "div"; }
		}
		[Browsable(false)]
		public virtual string MapJavaScriptVallue(string name, string value)
		{
			string s = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (s != null)
			{
				return s;
			}
			return value;
		}
		#endregion
		#region IWebClientControl Properties
		//
		private SizeType _widthSizeType = SizeType.Percent;
		[Category("Layout")]
		[DefaultValue(SizeType.Percent)]
		[Description("Gets and sets size type for width. Check out its effects by showing the page in a browser.")]
		public SizeType WidthType
		{
			get
			{
				return _widthSizeType;
			}
			set
			{
				_widthSizeType = value;
			}
		}
		private uint _width = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthType is Percent.")]
		public uint WidthInPercent
		{
			get
			{
				return _width;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_width = value;
				}
			}
		}

		private SizeType _heightSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for height. Check out its effects by showing the page in a browser.")]
		public SizeType HeightType
		{
			get
			{
				return _heightSizeType;
			}
			set
			{
				_heightSizeType = value;
			}
		}
		private uint _height = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightType is Percent.")]
		public uint HeightInPercent
		{
			get
			{
				return _height;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_height = value;
				}
			}
		}
		//
		[Description("id of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string id { get { return Name; } }

		[Description("tag name of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return ElementName; } }

		[Description("Returns the viewable width of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientWidth { get { return 0; } }

		[Description("Returns the viewable height of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientHeight { get { return 0; } }

		[Description("Returns the height of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetHeight { get { return 0; } }

		[Description("Returns the width of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetWidth { get { return 0; } }

		[Description("Returns the horizontal offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetLeft { get { return 0; } }

		[Description("Returns the vertical offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetTop { get { return 0; } }

		[Description("Returns the entire height of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollHeight { get { return 0; } }

		[Description("Returns the distance between the actual left edge of an element and its left edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollLeft { get { return 0; } }

		[Description("Returns the distance between the actual top edge of an element and its top edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollTop { get { return 0; } }

		[Description("Returns the entire width of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollWidth { get { return 0; } }

		[Category("Layout")]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[Description("Gets and sets anchor style. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public AnchorStyles PositionAnchor
		{
			get
			{
				return this.Anchor;
			}
			set
			{
				this.Anchor = value;
			}
		}
		[Category("Layout")]
		[DefaultValue(ContentAlignment.TopLeft)]
		[Description("Gets and sets position alignment. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public ContentAlignment PositionAlignment
		{
			get;
			set;
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return WebClientValueCollection.GetWebClientProperties(this, _propertyNames, attributes);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion
		#region IWebClientComponent Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		private WebClientValueCollection _customValues;
		[WebClientMember]
		[RefreshProperties(RefreshProperties.All)]
		[EditorAttribute(typeof(TypeEditorWebClientValue), typeof(UITypeEditor))]
		[Description("A custom value is associated with an Html element. It provides a storage to hold data for the element.")]
		public WebClientValueCollection CustomValues
		{
			get
			{
				if (_customValues == null)
				{
					_customValues = new WebClientValueCollection(this);
				}
				return _customValues;
			}
		}
		[Bindable(true)]
		[Description("Gets and sets HTML contents at runtime.")]
		[WebClientMember]
		public string innerHTML
		{
			get;set;
		}
		[Bindable(true)]
		[WebClientMember]
		[Description("Gets and sets data associated with the element")]
		public string tag
		{
			get;
			set;
		}
		[Description("Associate a named data with the element")]
		[WebClientMember]
		public void SetOrCreateNamedValue(string name, string value)
		{

		}
		[Description("Gets a named data associated with the element")]
		[WebClientMember]
		public string GetNamedValue(string name)
		{
			return string.Empty;
		}
		[Description("Gets all child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getElementsByTagName(string tagName)
		{
			return null;
		}
		[Description("Gets all immediate child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getDirectChildElementsByTagName(string tagName)
		{
			return null;
		}
		#endregion
		#region IJsFilesResource Members

		public Dictionary<string, string> GetJsFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
#if DEBUG
			files.Add("htmlEditorDiv.js", Resource1.htmlEditorDiv);
#else
			files.Add("htmlEditorDiv.js", Resource1.htmleditorDiv_min);
#endif
			return files;
		}

		#endregion
		#region Web Client Events
		[Description("Occurs when the visitor clicks the OK button.")]
		[WebClientMember]
		public event ehFinishHtmlEdit FinishedHtmlEdit { add { } remove { } }

		[Description("Occurs when the visitor clicks the Cancel button.")]
		[WebClientMember]
		public event ehFinishHtmlEdit CancelHtmlEdit { add { } remove { } }
		#endregion
		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }
		//
		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		//
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }
		//

		[Description("Occurs when the mouse is pressed over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }
		#endregion
		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add("\r\n");
			//
			sc.Add(string.Format(CultureInfo.InvariantCulture, "limnorHtmlEditorDIV.loadLibs(['{0}']);\r\n", CodeName));
		}

		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
			
		}

		#endregion
	}
	public delegate void ehFinishHtmlEdit(HtmlRichTextEditor sender, string htmlText);
}
