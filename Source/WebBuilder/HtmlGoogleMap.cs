/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Xml;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Globalization;
using System.Xml.Serialization;
using System.Drawing.Design;
using WindowsUtility;
using google.maps;
using LFilePath;
using System.IO;
using google.maps.places;

namespace Limnor.WebBuilder
{
	[ToolboxBitmap(typeof(HtmlGoogleMap), "Resources.googlemap.bmp")]
	[Description("This is a Google Map on a web page.")]
	public class HtmlGoogleMap : PictureBox, IWebClientControl, ICustomTypeDescriptor, IExternalJavascriptIncludes, IWebClientInitializer, IJavaScriptEventOwner, IJsFilesResource, IValueUIEditorOwner, IJavascriptPropertyHolder
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private bool _usePlaces;
		public HtmlGoogleMap()
		{
			Text = "";
			_resourceFiles = new List<WebResourceFile>();
			this.SizeMode = PictureBoxSizeMode.StretchImage;
			this.Image = WinUtil.SetImageOpacity(Resource1.googlemap, (float)0.5);
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			GoogleMapsZoom = 8;
			GoogleMapsMapType = MapTypeId.ROADMAP;
			GoogleMapsCenter = new GoogleMapsLatLng(0, 0);
		}
		static HtmlGoogleMap()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Visible");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("BorderStyle");
			_propertyNames.Add("WidthInPercent");
			_propertyNames.Add("HeightInPercent");
			//
			_propertyNames.Add("GoogleMapsMapType");
			_propertyNames.Add("GoogleMapsZoom");
			_propertyNames.Add("GoogleMapsCenter");
			_propertyNames.Add("GoogleMapsUseSensor");
			_propertyNames.Add("GoogleMapsApiKey");
			_propertyNames.Add("GoogleMapsMarkers");
			//
			_propertyNames.Add("GoogleMapsDirectionsResult");
			_propertyNames.Add("GoogleMapsDirectionsQuerytStatus");
			//
			_propertyNames.Add("GoogleMapsPlacesQuerytStatus");
			_propertyNames.Add("GoogleMapsPlacesHasNextPage");
			//
			_propertyNames.Add("placesCountLastPage");
			_propertyNames.Add("placesCount");
			_propertyNames.Add("placesOnLastPage");
			_propertyNames.Add("placesAll");
			//
			_propertyNames.Add("FirstRoute");
			_propertyNames.Add("RouteCount");
		}
		#endregion
		#region Properties

		private SizeType _widthSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
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
		#endregion
		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

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
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

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
		private XmlNode _dataNode;
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode DataXmlNode { get { return _dataNode; } set { _dataNode = value; } }

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
		public bool WebContentLoaded
		{
			get
			{
				return true;
			}
		}

		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return null; }
		}

		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (!string.IsNullOrEmpty(_vaname))
					return _vaname;
				return getMapVarBaseName();
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "div"; } }

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
			List<MethodInfo> lst = new List<MethodInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			}
			MethodInfo[] ret = this.GetType().GetMethods(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}

		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			List<EventInfo> lst = new List<EventInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			}
			EventInfo[] ret = this.GetType().GetEvents(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}

		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			if (isStatic)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			}
			else
			{
				List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
				PropertyDescriptorCollection ps = GetProperties(new Attribute[] { });
				foreach (PropertyDescriptor p in ps)
				{
					if (p.Attributes != null)
					{
						bool bDesignOnly = false;
						foreach (Attribute a in p.Attributes)
						{
							DesignerOnlyAttribute da = a as DesignerOnlyAttribute;
							if (da != null)
							{
								bDesignOnly = true;
								break;
							}
						}
						if (bDesignOnly)
						{
							continue;
						}
					}
					bool bExists = false;
					foreach (PropertyDescriptor p0 in lst)
					{
						if (string.CompareOrdinal(p0.Name, p.Name) == 0)
						{
							bExists = true;
							break;
						}
					}
					if (!bExists)
					{
						lst.Add(p);
					}
				}
				return new PropertyDescriptorCollection(lst.ToArray());
			}
		}

		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			bool b;
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			XmlUtil.SetAttribute(node, "id", this.VarName);
			_resourceFiles = new List<WebResourceFile>();
			//
			StringBuilder sb = new StringBuilder();
			//
			if (this.BorderStyle != BorderStyle.None)
			{
				sb.Append("border:1px solid black; ");
			}
			if (!this.AutoSize)
			{
				sb.Append("overflow:auto;");
			}
			sb.Append("width:");
			if (WidthInPercent == 0)
			{
				sb.Append(this.Width.ToString(CultureInfo.InvariantCulture));
				sb.Append("px; ");
			}
			else
			{
				sb.Append(this.WidthInPercent.ToString(CultureInfo.InvariantCulture));
				sb.Append("%; ");
			}
			//
			sb.Append("height:");
			if (HeightInPercent == 0)
			{
				sb.Append(this.Height.ToString(CultureInfo.InvariantCulture));
				sb.Append("px; ");
			}
			else
			{
				sb.Append(this.HeightInPercent.ToString(CultureInfo.InvariantCulture));
				sb.Append("%; ");
			}
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
							b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
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
				//
				pNode = _dataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@name='disabled']", XmlTags.XML_PROPERTY));
				if (pNode != null)
				{
					string s = pNode.InnerText;
					if (!string.IsNullOrEmpty(s))
					{
						try
						{
							b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (b)
							{
								XmlUtil.SetAttribute(node, "disabled", "disabled");
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
			if (string.CompareOrdinal(methodName, "GetDirections") == 0)
			{
				code.Add("limnorgooglemaps.getDirections('");
				code.Add(this.VarName);
				code.Add("'");
				for (int i = 0; i < parameters.Count; i++)
				{
					code.Add(",");
					code.Add(parameters[i]);
				}
				code.Add(");\r\n");
			}
			else if (string.CompareOrdinal(methodName, "GetMarkerByUuid") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(returnReceiver);
					code.Add("=");
					code.Add("limnorgooglemaps.getMarker('");
					code.Add(VarName);
					code.Add("',");
					code.Add(parameters[0]);
					code.Add(");\r\n");
				}
			}
			else if (string.CompareOrdinal(methodName, "AddMarker") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(returnReceiver);
					code.Add("=");
				}
				code.Add("limnorgooglemaps.addMarker('");
				code.Add(VarName);
				code.Add("',");
				code.Add(parameters[0]);
				code.Add(",");
				code.Add(parameters[1]);
				code.Add(",");
				code.Add(parameters[2]); //name
				code.Add(",");
				code.Add(parameters[3]); //title
				code.Add(",null"); //uuid
				if (!string.IsNullOrEmpty(parameters[4]))
				{
					code.Add(",");
					bool isFile = false;
					try
					{
						string sFile;
						if (parameters[4].StartsWith("'", StringComparison.Ordinal))
						{
							if (parameters[4].EndsWith("'", StringComparison.Ordinal))
							{
								sFile = parameters[4].Substring(1, parameters[4].Length - 2);
							}
							else
							{
								sFile = parameters[4];
							}
						}
						else
						{
							sFile = parameters[4];
						}
						if (File.Exists(sFile))
						{
							bool b;
							WebResourceFile wrf = new WebResourceFile(sFile, WebResourceFile.WEBFOLDER_Images, out b);
							_resourceFiles.Add(wrf);
							code.Add(string.Format(CultureInfo.InvariantCulture, "'{0}'", wrf.WebAddress));
							isFile = true;
						}
					}
					catch
					{
					}
					if (!isFile)
					{
						code.Add(parameters[4]);
					}
				}
				if (parameters.Count > 6)
				{
					code.Add(",");
					code.Add(parameters[5]);
					code.Add(",");
					code.Add(parameters[6]);
					if (parameters.Count > 7)
					{
						code.Add(",");
						if (string.IsNullOrEmpty(parameters[7]) || string.CompareOrdinal(parameters[7], "''") == 0)
						{
							code.Add("'FFFF00'");
						}
						else if (parameters[7].StartsWith("'#", StringComparison.Ordinal))
						{
							code.Add(parameters[7].Replace("#", ""));
						}
						else
						{
							code.Add(parameters[7]);
						}
					}
				}
				code.Add(");\r\n");
			}
			else if (string.CompareOrdinal(methodName, "HideAllMarkers") == 0)
			{
				code.Add("limnorgooglemaps.hideAllMarkers('");
				code.Add(this.VarName);
				code.Add("');\r\n");
			}
			else if (string.CompareOrdinal(methodName, "ShowAllMarkers") == 0)
			{
				code.Add("limnorgooglemaps.showAllMarkers('");
				code.Add(this.VarName);
				code.Add("');\r\n");
			}
			else if (string.CompareOrdinal(methodName, "RemoveAllMarkers") == 0)
			{
				code.Add("limnorgooglemaps.removeAllMarkers('");
				code.Add(this.VarName);
				code.Add("');\r\n");
			}
			else if (string.CompareOrdinal(methodName, "SearchPlacesByLocation") == 0)
			{
				_usePlaces = true;
				code.Add("limnorgooglemaps.searchPlacesByLocation('");
				code.Add(this.VarName);
				code.Add("',");
				code.Add(parameters[0]); //latitude
				code.Add(",");
				code.Add(parameters[1]); //longitude
				code.Add(",");
				code.Add(parameters[2]); //radius
				code.Add(",");
				if (string.CompareOrdinal(parameters[3], "''") == 0)//keyword
				{
					code.Add("null");
				}
				else
				{
					code.Add(parameters[3]);
				}
				code.Add(",");
				if (string.CompareOrdinal(parameters[4], "''") == 0)//name
				{
					code.Add("null");
				}
				else
				{
					code.Add(parameters[4]);
				}
				code.Add(",");
				code.Add(parameters[5]);
				code.Add(",");
				if (!string.IsNullOrEmpty(parameters[6]))
				{
					code.Add(parameters[6]);
				}
				else
				{
					code.Add("null");
				}
				code.Add(");\r\n");
			}
			else if (string.CompareOrdinal(methodName, "GetNextPlacesPage") == 0)
			{
				_usePlaces = true;
				code.Add("limnorgooglemaps.nextPlacesPage('");
				code.Add(this.VarName);
				code.Add("');\r\n");
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(VarName), methodName, code, parameters, returnReceiver);
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(VarName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
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
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		#endregion

		#region IWebClientControl Properties
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

		[XmlIgnore]
		[Description("Sets or returns the HTML contents (+text) of an element")]
		[Browsable(false)]
		[WebClientMember]
		public string innerHTML { get; set; }

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
		#endregion

		#region IWebClientSupport Members
		/// <summary>
		/// it is for math expression usage of methods
		/// </summary>
		/// <param name="ownerCodeName"></param>
		/// <param name="methodName"></param>
		/// <param name="code"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "AddMarker") == 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("limnorgooglemaps.addMarker('");
				sb.Append(ownerCodeName);
				sb.Append("',");
				sb.Append(parameters[0]);
				sb.Append(",");
				sb.Append(parameters[1]);
				sb.Append(",");
				sb.Append(parameters[2]);
				sb.Append(",");
				sb.Append(parameters[3]);
				sb.Append(")");
				return sb.ToString();
			}
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}
		/// <summary>
		/// it is for math expression usage of properties
		/// </summary>
		/// <param name="method"></param>
		/// <param name="propertyName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			string cn = GetJavascriptPropertyCodeName(propertyName);
			if (string.IsNullOrEmpty(cn))
			{
				cn = GetJavaScriptReferenceCode(method, propertyName, parameters);
			}
			return cn;
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

		#region Googl Maps Properties
		[Category("GoogleMaps")]
		[NotForProgramming]
		[Description("Gets and sets a Google Maps API Key. See https://developers.google.com/maps/documentation/javascript/tutorial#api_key")]
		public string GoogleMapsApiKey { get; set; }

		[Category("GoogleMaps")]
		[NotForProgramming]
		[Description("Gets and sets a Boolean indicating wether sensor is used")]
		public bool GoogleMapsUseSensor { get; set; }

		private GoogleMapsLatLng _center;
		[Category("GoogleMaps")]
		[NotForProgramming]
		[Description("Gets and sets a location point as the center of a map when a web page is loaded")]
		public GoogleMapsLatLng GoogleMapsCenter { get { return _center; } set { _center = value; } }

		[Category("GoogleMaps")]
		[DefaultValue(8)]
		[NotForProgramming]
		[Description("Gets and sets an integer as map zoom factor when a web page is loaded.")]
		public int GoogleMapsZoom { get; set; }

		[Category("GoogleMaps")]
		[NotForProgramming]
		[Description("Gets and sets Google Maps Type used by a map")]
		public MapTypeId GoogleMapsMapType { get; set; }

		[Category("GoogleMaps")]
		[WebClientMember]
		[Description("Gets Google Maps directions service query status")]
		public DirectionsStatus GoogleMapsDirectionsQuerytStatus { get { return DirectionsStatus.OK; } }

		[Category("GoogleMaps")]
		[WebClientMember]
		[Description("Gets Google Maps directions service query results")]
		public GoogleMapsDirectionsRoute[] GoogleMapsDirectionsRoutes { get { return null; } }

		[Category("GoogleMaps")]
		[WebClientMember]
		[Description("Gets routes from a Google Maps directions service query results")]
		public GoogleMapsDirectionsResult GoogleMapsDirectionsResult { get { return null; } }
		[WebClientMember]
		[Description("Gets the first route from Google Maps directions service query results")]
		public GoogleMapsDirectionsRoute FirstRoute { get { return null; } }

		[Description("Gets the number of routes from Google Maps directions service query results")]
		[WebClientMember]
		public int RouteCount { get { return 0; } }

		[Category("GoogleMaps")]
		[WebClientMember]
		[Description("Gets Google Maps place service query status")]
		public PlacesServiceStatus GoogleMapsPlacesQuerytStatus { get { return PlacesServiceStatus.OK; } }
		#endregion

		#region Google Maps Methods
		[Description("Query directions on Google Maps")]
		[WebClientMember]
		public void GetDirections(string origin, string destination,
			TravelMode travelMode,
			JsDateTime transitDepartureTime, JsDateTime transitArrivalTime,
			UnitSystem unitSystem, bool durationInTraffic,
			bool provideRouteAlternatives, bool avoidHighways,
			bool avoidTolls, string region
			)
		{
		}
		[Description("Add a marker on Google Maps")]
		[WebClientMember]
		public GoogleMapsMarker AddMarker(float latitude,
			float longitude,
			string name,
			string title, string iconUrl, int width, int height, Color color)
		{
			return null;
		}
		[Description("Hide all markers on Google Maps")]
		[WebClientMember]
		public void HideAllMarkers()
		{
		}
		[Description("Show all markers on Google Maps")]
		[WebClientMember]
		public void ShowAllMarkers()
		{
		}
		[Description("Remove all markers on Google Maps")]
		[WebClientMember]
		public void RemoveAllMarkers()
		{
		}

		[Description("Search places around a specific location on Google Maps")]
		[WebClientMember]
		public void SearchPlacesByLocation(float latitude,
			float longitude, int radius, string keyword, string name, RankBy rankBy, string[] placeTypes)
		{
		}
		#endregion

		#region Google Maps Events
		[Description("Occurs when a marker is clicked on")]
		[WebClientMember]
		public event GoogleMapsMarkerEvent MarkerClick { add { } remove { } }

		[Description("Occurs when a call to GetDirections returns. Check property GoogleMapsDirectionsQuerytStatus for query result.")]
		[WebClientMember]
		public event SimpleCall DirectionsQueryReturned { add { } remove { } }

		[Description("Occurs when a call to a places search action returns. Check property GoogleMapsPlacesQuerytStatus for query result.")]
		[WebClientMember]
		public event SimpleCall PlacesQueryReturned { add { } remove { } }

		[Description("Occurs when a marker representing a place is clicked on. A marker representing a place is among markers generated by a place search action.")]
		[WebClientMember]
		public event GoogleMapsMarkerEvent PlaceMarkerClick { add { } remove { } }

		[Description("Occurs when place details are available")]
		[WebClientMember]
		public event GoogleMapsPlacesEvent gotPlaceDetails { add { } remove { } }
		#endregion

		#region Markers
		public bool IsMarkerNameInUse(string markerName)
		{
			foreach (GoogleMapsMarker s in GoogleMapsMarkers)
			{
				if (string.CompareOrdinal(s.name, markerName) == 0)
				{
					return true;
				}
			}
			return false;
		}
		private GoogleMapsMarkerCollection _markers;
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Editor(typeof(TypeEditorMapMarker), typeof(UITypeEditor))]
		[Category("GoogleMaps")]
		public GoogleMapsMarkerCollection GoogleMapsMarkers
		{
			get
			{
				if (_markers == null)
				{
					_markers = new GoogleMapsMarkerCollection(this);
				}
				return _markers;
			}
			set
			{
				_markers = value;
			}
		}

		[Description("Returns a marker with given uuid.")]
		[WebClientMember]
		public GoogleMapsMarker GetMarkerByUuid(string uuid)
		{
			return null;
		}
		#endregion

		#region Places
		[Description("Gets a Boolean indicating whether there are more pages of places found by a places search action")]
		[WebClientMember]
		public bool GoogleMapsPlacesHasNextPage
		{
			get { return false; }
		}

		[Description("Gets an integer indicating how many places produced by the last places search or the last GetNextPlacesPage action.")]
		[WebClientMember]
		public int placesCountLastPage { get { return 0; } }

		[Description("Gets an integer indicating how many places currently retrieved by a places search action and all GetNextPlacesPage actions.")]
		[WebClientMember]
		public int placesCount { get { return 0; } }

		[Description("Gets an array of places produced by the last places search or the last GetNextPlacesPage action.")]
		[WebClientMember]
		public GoogleMapsPlace[] placesOnLastPage { get { return null; } }

		[Description("Gets an array of places currently retrieved by a places search action and all GetNextPlacesPage actions.")]
		[WebClientMember]
		public GoogleMapsPlace[] placesAll { get { return null; } }

		[Description("Fetch next page ")]
		[WebClientMember]
		public void GetNextPlacesPage()
		{
		}
		#endregion

		#region Methods
		public string getMapVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.map", getMapVarBaseName());
		}
		public string getMapPlacesServiceName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.placesService", getMapVarBaseName());
		}
		public string getMapDirectionsServiceVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.directionsService", getMapVarBaseName());
		}
		public string getMapDirectionsRendererVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.directionsRenderer", getMapVarBaseName());
		}
		public string getMapDirectionsQueryStatusVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.directionsQuerytStatus", getMapVarBaseName());
		}
		public string getMapDirectionsResultVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.directionsResult", getMapVarBaseName());
		}
		public string getMapDirectionsQueryReturnEventName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.directionsQueryReturned", getMapVarBaseName());
		}
		public string getMapMarkerEventName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.markerClicked", getMapVarBaseName());
		}
		//
		public string getMapPlacesQueryStatusVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.placesQuerytStatus", getMapVarBaseName());
		}
		public string getMapPlacesCountVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.placesCount", getMapVarBaseName());
		}
		public string getMapPlacesMorePagesVarName()
		{
			return string.Format(CultureInfo.InvariantCulture, "limnorgooglemaps.hasNextPage('{0}')", this.VarName);
		}
		public string getMapPlacesQueryReturnEventName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.placesQueryReturned", getMapVarBaseName());
		}
		public string getMapPlaceMarkerClickEventName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.placeMarkerClick", getMapVarBaseName());
		}
		public string getMapPlaceDetailsEventName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.gotPlaceDetails", getMapVarBaseName());
		}
		//
		public string getMapVarBaseName()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", getMapNamespace(), VarName);
		}
		public string getMapNamespace()
		{
			return "limnorPage.googlemaps";
		}

		public string VarName
		{
			get
			{
				return XmlUtil.GetNameAttribute(_dataNode);
			}
		}
		#endregion

		#region private methods

		#endregion

		#region IExternalJavascriptIncludes Members
		[Browsable(false)]
		[NotForProgramming]
		[ReadOnly(true)]
		public string[] ExternalJavascriptIncludes
		{
			get
			{
				string[] ss = new string[1];
				StringBuilder sb = new StringBuilder("https://maps.googleapis.com/maps/api/js?");
				if (!string.IsNullOrEmpty(GoogleMapsApiKey))
				{
					sb.Append("key=");
					sb.Append(GoogleMapsApiKey);
					sb.Append(WebPageCompilerUtility.AMPSAND);
				}
				sb.Append("sensor=");
				if (GoogleMapsUseSensor)
				{
					sb.Append("true");
				}
				else
				{
					sb.Append("false");
				}
				if (_usePlaces)
				{
					sb.Append(WebPageCompilerUtility.AMPSAND);
					sb.Append("libraries=places");
				}
				ss[0] = sb.ToString();
				//
				return ss;
			}
			set
			{

			}
		}

		#endregion

		#region IWebClientInitializer Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnWebPageLoaded(StringCollection sc)
		{
			string mapOptions = string.Format(CultureInfo.InvariantCulture, "map{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			sc.Add("var ");
			sc.Add(mapOptions);
			sc.Add(" = {\r\n");
			sc.Add("    center: new google.maps.LatLng(");
			sc.Add(GoogleMapsCenter.lat().ToString(CultureInfo.InvariantCulture));
			sc.Add(",");
			sc.Add(GoogleMapsCenter.lng().ToString(CultureInfo.InvariantCulture));
			sc.Add("),\r\n");
			sc.Add("    zoom: ");
			sc.Add(GoogleMapsZoom.ToString(CultureInfo.InvariantCulture));
			sc.Add(",\r\n");
			sc.Add("    mapTypeId: google.maps.MapTypeId.");
			sc.Add(GoogleMapsMapType.ToString());
			sc.Add("\r\n};\r\n");
			//initialize map namespace
			sc.Add(getMapNamespace());
			sc.Add("=");
			sc.Add(getMapNamespace());
			sc.Add("||{};\r\n");
			//initialize this map
			sc.Add(getMapVarBaseName());
			sc.Add("={};\r\n");
			//
			sc.Add(getMapVarBaseName());
			sc.Add(".markers= new Array();\r\n");
			//
			sc.Add(getMapVarName());
			sc.Add("=new google.maps.Map(document.getElementById('");
			sc.Add(this.VarName);
			sc.Add("'),");
			sc.Add(mapOptions);
			sc.Add(");\r\n");
			//
			foreach (GoogleMapsMarker mm in this.GoogleMapsMarkers)
			{
				mm.OnWebPageLoaded(sc);
				IList<WebResourceFile> fs = mm.GetWebResourceFiles();
				if (fs != null)
				{
					_resourceFiles.AddRange(fs);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
			foreach (GoogleMapsMarker mm in this.GoogleMapsMarkers)
			{
				mm.OnWebPageLoadedAfterEventHandlerCreations(sc);
			}
			if (_eventHandlers != null)
			{
				foreach (KeyValuePair<string, string> kv in _eventHandlers)
				{
					AttachJsEvent(null, kv.Key, kv.Value, sc);
				}
			}
		}

		#endregion

		#region IJavaScriptEventOwner Members
		private Dictionary<string, string> _eventHandlersDynamic;
		private Dictionary<string, string> _eventHandlers;
		public void LinkJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode, bool isDynamic)
		{
			if (isDynamic)
			{
				if (_eventHandlersDynamic == null)
				{
					_eventHandlersDynamic = new Dictionary<string, string>();
				}
				_eventHandlersDynamic.Add(eventName, handlerName);
			}
			else
			{
				if (_eventHandlers == null)
				{
					_eventHandlers = new Dictionary<string, string>();
				}
				_eventHandlers.Add(eventName, handlerName);
			}
		}
		public void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode)
		{
			if (string.CompareOrdinal(eventName, "MarkerClick") == 0)
			{
				jsCode.Add(getMapMarkerEventName());
				jsCode.Add("=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}
			else if (string.CompareOrdinal(eventName, "DirectionsQueryReturned") == 0)
			{
				jsCode.Add(getMapDirectionsQueryReturnEventName());
				jsCode.Add("=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}
			else if (string.CompareOrdinal(eventName, "PlacesQueryReturned") == 0)
			{
				jsCode.Add(getMapPlacesQueryReturnEventName());
				jsCode.Add("=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}
			else if (string.CompareOrdinal(eventName, "PlaceMarkerClick") == 0)
			{
				jsCode.Add(getMapPlaceMarkerClickEventName());
				jsCode.Add("=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}//
			else if (string.CompareOrdinal(eventName, "gotPlaceDetails") == 0)
			{
				jsCode.Add(getMapPlaceDetailsEventName());
				jsCode.Add("=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}
		}
		#endregion

		#region IJsFilesResource Members

		public Dictionary<string, string> GetJsFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
#if DEBUG
			files.Add("limnorgooglemaps.js", Resource1.limnorgooglemaps);
#else
            files.Add("limnorgooglemaps.js", Resource1.limnorgooglemaps_min);
#endif
			return files;
		}

		#endregion

		#region IValueUIEditorOwner Members

		public EditorAttribute GetValueUIEditor(string valueName)
		{
			if (string.CompareOrdinal(valueName, "iconUrl") == 0)
			{
				return new EditorAttribute(typeof(PropEditorFilePath), typeof(UITypeEditor));
			}
			if (string.CompareOrdinal(valueName, "placeTypes") == 0)
			{
				return new EditorAttribute(typeof(TypeEditorSelectPlaceTypes), typeof(UITypeEditor));
			}
			return null;
		}

		#endregion

		#region IJavascriptPropertyHolder Members

		public string GetJavascriptPropertyCodeName(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "GoogleMapsDirectionsQuerytStatus") == 0)
			{
				return getMapDirectionsQueryStatusVarName();
			}
			if (string.CompareOrdinal(propertyName, "GoogleMapsDirectionsResult") == 0)
			{
				return getMapDirectionsResultVarName();
			}
			if (string.CompareOrdinal(propertyName, "GoogleMapsDirectionsRoutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.routes", getMapVarBaseName());
			}
			if (string.CompareOrdinal(propertyName, "GoogleMapsPlacesQuerytStatus") == 0)
			{
				return getMapPlacesQueryStatusVarName();
			}
			if (string.CompareOrdinal(propertyName, "placesCount") == 0)
			{
				return getMapPlacesCountVarName();
			}
			if (string.CompareOrdinal(propertyName, "placesCountLastPage") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.placesCountLastPage", getMapVarBaseName());
			}
			if (string.CompareOrdinal(propertyName, "placesOnLastPage") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.placesOnLastPage", getMapVarBaseName());
			}
			if (string.CompareOrdinal(propertyName, "placesAll") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.placesAll", getMapVarBaseName());
			}
			if (string.CompareOrdinal(propertyName, "GoogleMapsPlacesHasNextPage") == 0)
			{
				return getMapPlacesMorePagesVarName();
			}
			if (string.CompareOrdinal(propertyName, "FirstRoute") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.firstRoute", getMapVarBaseName());
			}
			if (string.CompareOrdinal(propertyName, "RouteCount") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.routeCount", getMapVarBaseName());
			}
			return null;
		}

		#endregion
	}
}
