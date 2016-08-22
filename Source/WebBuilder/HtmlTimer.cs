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
using System.Reflection;
using System.Collections.Specialized;
using VPL;
using System.Globalization;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTimer), "Resources.timer.bmp")]
	[Description("This is a timer on a web page.")]
	public class HtmlTimer : IComponent, IWebClientComponent, IWebClientInitializer, ICustomCodeName, IWebEventProcess
	{
		#region fields and constructors
		private string _codename;
		public HtmlTimer()
		{
			Interval = 1000;
		}
		public HtmlTimer(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			Interval = 1000;
		}
		#endregion

		#region Web Properties
		[Description("The frequency of onelapsed events in milliseconds")]
		[DefaultValue(1000)]
		[WebClientMember]
		public int Interval { get; set; }

		[Description("The maximum number of onelapsed events. 0 indicates unlimited.")]
		[DefaultValue(0)]
		[WebClientMember]
		public int MaxElapses { get; set; }

		[Description("Gets a Boolean indicating whether the timer has started")]
		[WebClientMember]
		public bool Started { get { return false; } }
		#endregion

		#region Web Events
		[Description("Occurs whenever the specified interval time elapses.")]
		[WebClientMember]
		public event SimpleCall onelapsed { add { } remove { } }

		[Description("Occurs when the specified interval time elapses MaxElapses times if MaxElapses > 0.")]
		[WebClientMember]
		public event SimpleCall onmaxelapsed { add { } remove { } }
		#endregion

		#region Web Methods
		[Description("Start the timer")]
		[WebClientMember]
		public void StartTimer()
		{
		}
		[Description("Stop the timer")]
		[WebClientMember]
		public void StopTimer()
		{
		}

		[Description("Start the timer if it is not started and stop the timer if it is started.")]
		[WebClientMember]
		public void Toggle()
		{
		}
		#endregion
		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		[ReadOnly(true)]
		[NotForProgramming]
		[Browsable(false)]
		public ISite Site
		{
			get;
			set;
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
		string _vname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vname = vname;
		}
		[NotForProgramming]
		[Browsable(false)]
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return WebPageCompilerUtility.GetWebClientMethods(this.GetType(), isStatic);
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "StartTimer") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"if(!{0}.handle) {{", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.elapsed = 0;\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.handle = setInterval({0}.onelapsed,{0}.Interval);\r\n", CodeName));
				code.Add("}\r\n");
			}
			else if (string.CompareOrdinal(methodName, "StopTimer") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"if({0}.handle) {{\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\tclearInterval({0}.handle);\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.handle = 0;\r\n", CodeName));
				code.Add("}\r\n");
			}
			else if (string.CompareOrdinal(methodName, "Toggle") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"if({0}.handle) {{\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\tclearInterval({0}.handle);\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.handle = 0;\r\n", CodeName));
				code.Add("}\r\nelse {\r\n");
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.elapsed = 0;\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.handle = setInterval({0}.onelapsed,{0}.Interval);\r\n", CodeName));
				code.Add("}\r\n");

			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "Started") == 0)
			{
				return "handle";
			}
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		[NotForProgramming]
		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			return null;
		}
		[NotForProgramming]
		[Browsable(false)]
		public string MapJavaScriptVallue(string name, string value)
		{
			return null;
		}

		#endregion

		#region IWebResourceFileUser Members
		[NotForProgramming]
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			return null;
		}

		#endregion

		#region IWebClientSupport Members
		[NotForProgramming]
		[Browsable(false)]
		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			if (string.CompareOrdinal(propertyName, "Started") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.handle", CodeName);
			}
			return null;
		}

		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add(string.Format(CultureInfo.InvariantCulture,
				"var {0} = {{}};\r\n{0}.Interval={1};\r\n{0}.MaxElapses={2};\r\n", CodeName, this.Interval, this.MaxElapses));
		}

		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{

		}

		#endregion

		#region ICustomCodeName Members
		[Browsable(false)]
		[NotForProgramming]
		public string CodeName
		{
			get
			{
				if (Site != null && !string.IsNullOrEmpty(Site.Name))
				{
					_codename = Site.Name;
					return Site.Name;
				}
				if (!string.IsNullOrEmpty(_codename))
					return _codename;
				return _vname;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool DeclareJsVariable(string context)
		{
			return false;
		}

		#endregion

		#region IWebEventProcess Members

		public void OnFinishEvent(string eventName, StringCollection handler)
		{
			if (string.CompareOrdinal(eventName, "onelapsed") == 0)
			{
				handler.Add(string.Format(CultureInfo.InvariantCulture,
					"if({0}.MaxElapses > 0) {{\r\n", CodeName));
				handler.Add(string.Format(CultureInfo.InvariantCulture,
					"\t{0}.elapsed ++;\r\n", CodeName));
				handler.Add(string.Format(CultureInfo.InvariantCulture,
					"\tif({0}.elapsed >= {0}.MaxElapses) {{\r\n", CodeName));
				handler.Add(string.Format(CultureInfo.InvariantCulture,
					"\t\tclearInterval({0}.handle);\r\n", CodeName));
				handler.Add(string.Format(CultureInfo.InvariantCulture,
					"\t\t{0}.handle = 0;\r\n", CodeName));
				handler.Add("\t}\r\n");
				handler.Add("}\r\n");
			}
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
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return string.Empty; } }

		[Browsable(false)]
		[WebClientMember]
		public string id { get { return this.CodeName; } }

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
