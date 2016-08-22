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
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Specialized;
using System.Drawing;
using VPL;
using System.IO;
using XmlUtility;
using System.Xml;
using System.Globalization;
using System.Drawing.Design;

namespace Limnor.WebBuilder
{
	/// <summary>
	/// include and use javascrit files
	/// </summary>
	[ToolboxBitmap(typeof(JavascriptFiles), "Resources.js.bmp")]
	[Description("This is a component for including and using Javascript files in the web page.")]
	public class JavascriptFiles : IWebClientComponent, IComponent, IXmlNodeSerializable, IDevClassReferencer, IResetOnComponentChange, IWebClientInitializer, IExternalJavascriptIncludes
	{
		#region fields and constructors
		private FilenameCollection _jsIncludes;
		private FilenameCollection _cssIncludes;
		private VplPropertyBag _variabes;
		private VplPropertyBag _pageVariabes;
		private List<WebResourceFile> _resourceFiles;
		public JavascriptFiles()
		{
			_resourceFiles = new List<WebResourceFile>();
		}
		public JavascriptFiles(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			_resourceFiles = new List<WebResourceFile>();
		}
		#endregion

		#region Properties
		[RefreshProperties(RefreshProperties.All)]
		[Description("External Javascript files to be included")]
		public FilenameCollection ExternalJavascriptFiles
		{
			get
			{
				createFilenames();
				return _jsIncludes;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Description("Cascading Style Sheets files to be included")]
		public FilenameCollection CssFiles
		{
			get
			{
				createFilenames();
				return _cssIncludes;
			}
		}
		[Description("A Script element will be generated in the web page for each string in this array. The string value will be the contents of src attribute of the Script element.")]
		public string[] ExternalJavascriptIncludes
		{
			get;
			set;
		}
		[Browsable(false)]
		public string[] Filenames
		{
			get
			{
				if (_jsIncludes == null)
				{
					return new string[] { };
				}
				string[] ret = new string[_jsIncludes.Count];
				for (int i = 0; i < _jsIncludes.Count; i++)
				{
					ret[i] = _jsIncludes[i];
				}
				return ret;
			}
			set
			{
				createFilenames();
				_jsIncludes.SetNames(value);
			}
		}
		[Browsable(false)]
		public string[] CssFilenames
		{
			get
			{
				if (_cssIncludes == null)
				{
					return new string[] { };
				}
				string[] ret = new string[_cssIncludes.Count];
				for (int i = 0; i < _cssIncludes.Count; i++)
				{
					ret[i] = _cssIncludes[i];
				}
				return ret;
			}
			set
			{
				createFilenames();
				_cssIncludes.SetNames(value);
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorManageJsVariables), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("Define Javascript values provided by the external Javascript files for accessing by visual programming objects. Limnor Studio compiler will not declare variables for these values. They must be declared by the external Javascript files.")]
		public VplPropertyBag JavascriptValues
		{
			get
			{
				createVariables();
				return _variabes;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorManageJsVariables), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("Declare Javascript variables in web page scope.")]
		public VplPropertyBag JavascriptPageVariables
		{
			get
			{
				createPageVariables();
				return _pageVariabes;
			}
		}
		[Editor(typeof(TypeSelectorText), typeof(UITypeEditor))]
		[Description("The contents of this property will be included as JavaScript code in web page scope.")]
		public string PageScopeJavascriptCode
		{
			get;
			set;
		}
		#endregion

		#region Methods
		[VariableParameter]
		[Description("Execute a Javascription function specified by the functionName. The functionName may include object name.")]
		[WebClientMember]
		public object ExecuteFunction(string functionName, params object[] parameters)
		{
			return null;
		}
		[MethodParameterJsCode("javascriptCode")]
		[WebClientMember]
		[Description("Execute Javascript code.")]
		public object Execute(string javascriptCode)
		{
			return null;
		}
		#endregion

		#region private methods
		private void createFilenames()
		{
			if (_jsIncludes == null)
			{
				_jsIncludes = new FilenameCollection();
				_jsIncludes.FilePatern = "Javascript files|*.js";
				_jsIncludes.FileDialogTitle = "Select Javascript File";
			}
			if (_cssIncludes == null)
			{
				_cssIncludes = new FilenameCollection();
				_cssIncludes.FilePatern = "Cascading Style Sheets files|*.css";
				_cssIncludes.FileDialogTitle = "Select Cascading Style Sheets File";
			}
		}
		private void createVariables()
		{
			if (_variabes == null)
			{
				_variabes = new VplPropertyBag(this);
				_variabes.EditorType = typeof(TypeSelectorJsVariable);
			}
		}
		private void createPageVariables()
		{
			if (_pageVariabes == null)
			{
				_pageVariabes = new VplPropertyBag(this);
				_pageVariabes.EditorType = typeof(TypeSelectorJsVariable);
			}
		}
		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IWebClientComponent Members
		private string _vaname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vaname = vname;
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(id), attributeName, method, parameters);
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
		[Browsable(false)]
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return new MethodInfo[] { };
		}
		[Browsable(false)]
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal("ExecuteFunction", methodName) == 0)
			{
				if (parameters.Count > 0)
				{
					string md = parameters[0];
					if (!string.IsNullOrEmpty(md))
					{
						if (md[0] == '\'')
						{
							md = md.Substring(1);
						}
						if (md.Length > 1)
						{
							if (md[md.Length - 1] == '\'')
							{
								md = md.Substring(0, md.Length - 1);
							}
						}
						if (md.Length > 0)
						{
							StringBuilder sb = new StringBuilder();
							if (!string.IsNullOrEmpty(returnReceiver))
							{
								sb.Append(returnReceiver);
								sb.Append("=");
							}
							sb.Append(md);
							sb.Append("(");
							if (parameters.Count > 1)
							{
								sb.Append(parameters[1]);
								for (int i = 2; i < parameters.Count; i++)
								{
									sb.Append(",");
									sb.Append(parameters[i]);
								}
							}
							sb.Append(");\r\n");
							code.Add(sb.ToString());
						}
					}
				}
			}
			else if (string.CompareOrdinal("Execute", methodName) == 0)
			{
				if (parameters.Count > 0)
				{
					if (!string.IsNullOrEmpty(parameters[0]))
					{
						string s;
						if (parameters[0].StartsWith("'", StringComparison.Ordinal))
						{
							s = parameters[0].Substring(1);
							if (s.EndsWith("'", StringComparison.Ordinal))
							{
								s = s.Substring(0, s.Length - 1);
							}
						}
						else
						{
							s = parameters[0];
						}
						code.Add(s);
					}
				}
			}
		}

		#endregion

		#region IWebResourceFileUser Members
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			List<WebResourceFile> l = _resourceFiles;
			if (_jsIncludes != null)
			{
				for (int i = 0; i < _jsIncludes.Count; i++)
				{
					if (File.Exists(_jsIncludes[i]))
					{
						bool b;
						WebResourceFile wr;
						wr = new WebResourceFile(_jsIncludes[i], WebResourceFile.WEBFOLDER_Javascript, out b);
						l.Add(wr);
					}
				}
			}
			if (_cssIncludes != null)
			{
				for (int i = 0; i < _cssIncludes.Count; i++)
				{
					if (File.Exists(_cssIncludes[i]))
					{
						bool b;
						WebResourceFile wr;
						wr = new WebResourceFile(_cssIncludes[i], WebResourceFile.WEBFOLDER_Css, out b);
						l.Add(wr);
					}
				}
			}
			return l;
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XML_INCLUDES = "Includes";
		const string XML_FILES = "Files";
		const string XML_CSSFILES = "CSSFiles";
		const string XML_VARS = "Values";
		const string XML_PAGEVARS = "Variables";
		const string XML_PAGECODE = "JsCode";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			createVariables();
			createFilenames();
			createPageVariables();
			XmlNode fn = node.SelectSingleNode(XML_FILES);
			if (fn != null)
			{
				string s = fn.InnerText.Trim();
				string[] ss = s.Split(',');
				_jsIncludes.SetNames(ss);
			}
			fn = node.SelectSingleNode(XML_CSSFILES);
			if (fn != null)
			{
				string s = fn.InnerText.Trim();
				string[] ss = s.Split(',');
				_cssIncludes.SetNames(ss);
			}
			XmlNode vn = node.SelectSingleNode(XML_VARS);
			if (vn != null)
			{
				_variabes.OnReadFromXmlNode(serializer, vn);
			}
			vn = node.SelectSingleNode(XML_PAGEVARS);
			if (vn != null)
			{
				_pageVariabes.OnReadFromXmlNode(serializer, vn);
			}
			fn = node.SelectSingleNode(XML_INCLUDES);
			if (fn != null)
			{
				XmlNodeList nds = fn.SelectNodes(XmlTags.XML_Item);
				ExternalJavascriptIncludes = new string[nds.Count];
				for (int i = 0; i < nds.Count; i++)
				{
					ExternalJavascriptIncludes[i] = nds[i].InnerText;
				}
			}
			vn = node.SelectSingleNode(XML_PAGECODE);
			if (vn == null)
			{
				PageScopeJavascriptCode = string.Empty;
			}
			else
			{
				PageScopeJavascriptCode = vn.InnerText;
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			string[] ss = Filenames;
			if (ss.Length > 0)
			{
				string s = string.Join(",", ss);
				XmlElement fn = node.OwnerDocument.CreateElement(XML_FILES);
				node.AppendChild(fn);
				fn.InnerText = s;
				fn.IsEmpty = false;
			}
			ss = CssFilenames;
			if (ss.Length > 0)
			{
				string s = string.Join(",", ss);
				XmlElement fn = node.OwnerDocument.CreateElement(XML_CSSFILES);
				node.AppendChild(fn);
				fn.InnerText = s;
				fn.IsEmpty = false;
			}
			if (_variabes != null)
			{
				XmlNode vn = node.OwnerDocument.CreateElement(XML_VARS);
				node.AppendChild(vn);
				_variabes.OnWriteToXmlNode(serializer, vn);
			}
			if (_pageVariabes != null)
			{
				XmlNode vn = node.OwnerDocument.CreateElement(XML_PAGEVARS);
				node.AppendChild(vn);
				_pageVariabes.OnWriteToXmlNode(serializer, vn);
			}
			if (ExternalJavascriptIncludes != null)
			{
				XmlElement fn = node.OwnerDocument.CreateElement(XML_INCLUDES);
				node.AppendChild(fn);
				for (int i = 0; i < ExternalJavascriptIncludes.Length; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					fn.AppendChild(nd);
					nd.InnerText = ExternalJavascriptIncludes[i];
				}
			}
			if (!string.IsNullOrEmpty(PageScopeJavascriptCode))
			{
				XmlNode vn = node.OwnerDocument.CreateElement(XML_PAGECODE);
				node.AppendChild(vn);
				vn.InnerText = PageScopeJavascriptCode;
			}
		}

		#endregion

		#region IDevClassReferencer Members
		private IDevClass _devClass;
		public void SetDevClass(IDevClass c)
		{
			_devClass = c;
		}

		public IDevClass GetDevClass()
		{
			return _devClass;
		}

		#endregion

		#region IResetOnComponentChange Members
		[Browsable(false)]
		public bool OnResetDesigner(string memberName)
		{
			if (string.CompareOrdinal(memberName, "JavascriptValues") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(memberName, "JavascriptPageVariables") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(memberName, "New value") == 0)
			{
				return true;
			}
			return false;
		}

		#endregion

		#region IWebClientInitializer Members
		[Browsable(false)]
		public void OnWebPageLoaded(StringCollection sc)
		{
			if (_variabes != null)
			{
				for (int i = 0; i < _variabes.Count; i++)
				{
					TypedNamedValue v = _variabes[i];
					if (v != null && v.Value != null && !v.Value.IsNullOrDefaultValue())
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}={1};\r\n", v.Name, v.Value.GetJsCode()));
					}
				}
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
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
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return "script"; } }

		[Browsable(false)]
		[WebClientMember]
		public string id
		{
			get
			{
				if (!string.IsNullOrEmpty(_vaname))
					return _vaname;
				return string.Empty;
			}
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

		#region Unused web client members
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public Color ForeColor { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public Color BackColor { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public bool Visible { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollHeight { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollTop { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollLeft { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetHeight { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetTop { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetLeft { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int clientWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int clientHeight { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public string innerHTML { get; set; }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int zOrder { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int Opacity { get; set; }
		#endregion
	}
}
