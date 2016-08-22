/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using XmlUtility;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using VPL;
using System.ComponentModel;
using System.Drawing.Design;

namespace Limnor.WebBuilder
{
	public static class WebPageCompilerUtility
	{
		public const string AMPSAND = "A92E0F20147C4a589D0D91B37F27275C";
		private static StringCollection _usedPhpClassNames;
		static WebPageCompilerUtility()
		{
			_usedPhpClassNames = new StringCollection();
			//PHP classes
			_usedPhpClassNames.Add("datasource");
			_usedPhpClassNames.Add("datatableupdator");
			_usedPhpClassNames.Add("fastjson");
			_usedPhpClassNames.Add("jsondatarow");
			_usedPhpClassNames.Add("jsondatarowupdate");
			_usedPhpClassNames.Add("jsondatacolumn");
			_usedPhpClassNames.Add("jsondatatable");
			_usedPhpClassNames.Add("jsondatatableupdate");
			_usedPhpClassNames.Add("jsondataset");
			_usedPhpClassNames.Add("requestcommand");
			_usedPhpClassNames.Add("databaseexecuter");
			_usedPhpClassNames.Add("webrequestorresponse");
			_usedPhpClassNames.Add("easyupdator");
			_usedPhpClassNames.Add("keyvalue");
			_usedPhpClassNames.Add("jsonprocesspage");
			_usedPhpClassNames.Add("credential");

			_usedPhpClassNames.Add("jsonsourcemysql");
			_usedPhpClassNames.Add("sqlclientparameter");
			_usedPhpClassNames.Add("sqlscriptprocess");
			_usedPhpClassNames.Add("webapp");
			_usedPhpClassNames.Add("htmlfileupload");
			_usedPhpClassNames.Add("htmlfileuploaditem");
			_usedPhpClassNames.Add("htmlfileuploadgroup");
			_usedPhpClassNames.Add("htmlfilesselector");
			_usedPhpClassNames.Add("phpcomponent");
			_usedPhpClassNames.Add("sendmail");
			_usedPhpClassNames.Add("serverfile");
			_usedPhpClassNames.Add("webrequestorresponse");
			_usedPhpClassNames.Add("easyupdator");
			_usedPhpClassNames.Add("keyvalue");
			_usedPhpClassNames.Add("jsonprocesspage");
			_usedPhpClassNames.Add("credential");

		}
		public static bool IsReservedPhpWord(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				s = s.ToLowerInvariant();
				return _usedPhpClassNames.Contains(s);
			}
			return false;
		}
		public static string StripSingleQuotes(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value.StartsWith("'", StringComparison.Ordinal))
				{
					value = value.Substring(1);
				}
				if (value.EndsWith("'", StringComparison.Ordinal))
				{
					value = value.Substring(0, value.Length - 1);
				}
			}
			return value;
		}
		public static string GetMethodParameterValueString(string p, StringCollection code)
		{
			string x = null;
			if (p == null)
			{
				x = "null";
			}
			else if (string.IsNullOrEmpty(p))
			{
				x = "''";
			}
			else
			{
				if (p.StartsWith("'", StringComparison.Ordinal))
				{
					x = p;
				}
				else
				{
					x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
				}
			}
			return x;
		}
		public static string GetMethodParameterValueInt(string p, StringCollection code)
		{
			string x = null;
			int n;
			if (int.TryParse(p, out n))
			{
				x = p;
			}
			else
			{
				x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
			}
			return x;
		}
		public static string GetMethodParameterValueBool(string p, StringCollection code)
		{
			string x = null;
			if (string.IsNullOrEmpty(p))
			{
				x = "false";
			}
			else
			{
				if (string.Compare(p, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					x = "true";
				}
				else if (string.Compare(p, "fals", StringComparison.OrdinalIgnoreCase) == 0)
				{
					x = "false";
				}
				else
				{
					int n;
					if (int.TryParse(p, out n))
					{
						if (n == 0)
						{
							x = "false";
						}
						else
						{
							x = "true";
						}
					}
					else
					{
						x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
					}
				}
			}
			return x;
		}
		public static bool TryGetMethodParameterAttributes(Type wcc, string methodName, string parameterName, out Attribute[] attrs)
		{
			attrs = null;
			if (string.CompareOrdinal(methodName, "SwitchEventHandler") == 0)
			{
				if (string.CompareOrdinal(parameterName, "eventName") == 0)
				{
					attrs = new Attribute[] {
						new TypeAttribute(wcc),
						new EditorAttribute(typeof(TypeSelectorSelEvent),typeof(UITypeEditor))
					};
					return true;
				}
				if (string.CompareOrdinal(parameterName, "handler") == 0)
				{
					attrs = new Attribute[] {
						new RunAtWebClientAttribute(),
						new TypeAttribute(wcc),
						new EditorAttribute(typeof(TypeSelectorSelMethod),typeof(UITypeEditor))
					};
					return true;
				}
			}
			return false;
		}
		public static bool IsWebClientMouseKeyboardEvent(string eventName)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				if (eventName.StartsWith("'", StringComparison.Ordinal))
				{
					eventName = eventName.Substring(1);
				}
				if (eventName.EndsWith("'", StringComparison.Ordinal))
				{
					eventName = eventName.Substring(0, eventName.Length - 1);
				}
				if (string.CompareOrdinal(eventName, "onclick") == 0
						|| string.CompareOrdinal(eventName, "onmousedown") == 0
						|| string.CompareOrdinal(eventName, "onmouseup") == 0
						|| string.CompareOrdinal(eventName, "onmouseover") == 0
						|| string.CompareOrdinal(eventName, "onmousemove") == 0
						|| string.CompareOrdinal(eventName, "onmouseout") == 0
						|| string.CompareOrdinal(eventName, "onkeypress") == 0
						|| string.CompareOrdinal(eventName, "onkeydown") == 0
						|| string.CompareOrdinal(eventName, "onkeyup") == 0
						|| string.CompareOrdinal(eventName, "onenterkey") == 0
						)
				{
					return true;
				}
			}
			return false;
		}
		public static string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, string[] parameters)
		{
			StringCollection ps = new StringCollection();
			ps.AddRange(parameters);
			return GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, ps);
		}
		public static string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "GetNamedValue") == 0)
			{
				if (parameters != null && parameters.Count > 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "({0})[{1}]", ownerCodeName, parameters[0]);
				}
			}
			else if (string.CompareOrdinal(methodName, "getElementsByTagName") == 0)
			{
				if (parameters != null && parameters.Count > 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}.getElementsByTagName({1})", ownerCodeName, parameters[0]);
				}
			}
			else if (string.CompareOrdinal(methodName, "getDirectChildElementsByTagName") == 0)
			{
				if (parameters != null && parameters.Count > 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getDirectChildElementsByTagName({0}, {1})", ownerCodeName, parameters[0]);
				}
			}
			else if (string.CompareOrdinal(methodName, "Visible") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.style.display", ownerCodeName);
			}
			if (string.CompareOrdinal(methodName, "BackColor") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.style.backgroundColor", ownerCodeName);
			}
			if (string.CompareOrdinal(methodName, "Text") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetInnerText({0})", ownerCodeName);
			}
			if (string.CompareOrdinal(methodName, "ForeColor") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.style.color", ownerCodeName);
			}
			if (string.CompareOrdinal(methodName, "cursor") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.style.cursor", ownerCodeName);
			}
			return null;
		}
		public static string GetFontJavascriptValues(string fontStyles)
		{
			if (!string.IsNullOrEmpty(fontStyles))
			{
				string[] ss = fontStyles.Split(';');
				if (ss != null && ss.Length > 0)
				{
					bool first = true;
					StringBuilder sb = new StringBuilder();
					sb.Append("[");
					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							string[] sv = ss[i].Split(':');
							if (sv != null && sv.Length == 2)
							{
								if (!string.IsNullOrEmpty(sv[0]) && !string.IsNullOrEmpty(sv[1]))
								{
									string sf = sv[0].Trim();
									string fv = sv[1].Trim();
									if (fv.Length > 0 && sf.Length > 0)
									{
										string fn = null;
										if (string.Compare(sf, "font-family", StringComparison.OrdinalIgnoreCase) == 0)
										{
											fn = "fontFamily";
										}
										else if (string.Compare(sf, "font-size", StringComparison.OrdinalIgnoreCase) == 0)
										{
											fn = "fontSize";
										}
										else if (string.Compare(sf, "font-weight", StringComparison.OrdinalIgnoreCase) == 0)
										{
											fn = "fontWeight";
										}
										else if (string.Compare(sf, "font-style", StringComparison.OrdinalIgnoreCase) == 0)
										{
											fn = "fontStyle";
										}
										else if (string.Compare(sf, "text-decoration", StringComparison.OrdinalIgnoreCase) == 0)
										{
											fn = "textDecoration";
										}
										if (!string.IsNullOrEmpty(fn))
										{
											if (first)
											{
												first = false;
											}
											else
											{
												sb.Append(",");
											}
											sb.Append("{name:'");
											sb.Append(fn);
											sb.Append("',value:'");
											sb.Append(fv);
											sb.Append("'}");
										}
									}
								}
							}
						}
					}
					sb.Append("]");
					return sb.ToString();
				}
			}
			return "[]";
		}
		public static string JsCodeRef(string codeName)
		{
			return string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}')", codeName);
		}
		public static bool CreateActionJavaScript(string element, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "GetNamedValue") == 0)
			{
				if (parameters != null && parameters.Count > 0 && !string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}={1}[{2}];\r\n", returnReceiver, element, parameters[0]));
				}
				return true;
			}
			else if (string.CompareOrdinal(methodName, "SetOrCreateNamedValue") == 0)
			{
				if (parameters != null && parameters.Count > 1)
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}[{1}]={2};\r\n", element, parameters[0], parameters[1]));
				}
				return true;
			}
			else if (string.CompareOrdinal(methodName, "focus") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.focus();\r\n", element));
				return true;
			}
			else if (string.CompareOrdinal(methodName, "setStyle") == 0)
			{
				if (!string.IsNullOrEmpty(parameters[0]) && !string.IsNullOrEmpty(parameters[1]))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}.style[{1}]={2};\r\n", element, parameters[0], parameters[1]));
				}
				return true;
			}
			else if (string.CompareOrdinal(methodName, "getElementsByTagName") == 0)
			{
				if (parameters != null && parameters.Count > 0 && !string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}={1}.getElementsByTagName({2});\r\n", returnReceiver, element, parameters[0]));
					return true;
				}
			}
			else if (string.CompareOrdinal(methodName, "getDirectChildElementsByTagName") == 0)
			{
				if (parameters != null && parameters.Count > 0 && !string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}=JsonDataBinding.getDirectChildElementsByTagName({1},{2});\r\n", returnReceiver, element, parameters[0]));
					return true;
				}
			}
			else if (string.CompareOrdinal(methodName, "Print") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.print({0});\r\n", element));
			}
			else if (string.CompareOrdinal(methodName, "SwitchEventHandler") == 0)
			{
				if (parameters != null && parameters.Count > 1)
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.ClearEvent({0},{1});\r\n", element, parameters[0]));
					string fn = parameters[1];
					if (IsWebClientMouseKeyboardEvent(parameters[0]))
					{
						if (!string.IsNullOrEmpty(fn) && string.CompareOrdinal(fn, "''") != 0 && string.CompareOrdinal(fn, "null") != 0)
						{
							if (fn.StartsWith("'", StringComparison.Ordinal))
							{
								fn = fn.Substring(1);
							}
							if (fn.EndsWith("'", StringComparison.Ordinal))
							{
								fn = fn.Substring(0, fn.Length - 1);
							}
							StringBuilder sd = new StringBuilder();
							sd.Append("function(event){");
							sd.Append("var e=(typeof event !=='undefined'&&event!= null)?event:(window.event||(document.parentWindow?document.parentWindow.event:null));var sender=JsonDataBinding.getSender(e);");
							sd.Append(fn);
							sd.Append("(sender,e);}");
							fn = sd.ToString();
						}
					}
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.AttachEvent({0},{1},{2});\r\n", element, parameters[0], fn));

					return true;
				}
			}
			return false;
		}
		public static void AddWebControlProperties(StringCollection _propertyNames)
		{
			_propertyNames.Add("className");
			_propertyNames.Add("Location");
			_propertyNames.Add("Size");
			_propertyNames.Add("Left");
			_propertyNames.Add("Top");
			_propertyNames.Add("Width");
			_propertyNames.Add("Height");
			_propertyNames.Add("WidthInPercent");
			_propertyNames.Add("HeightInPercent");
			_propertyNames.Add("WidthType");
			_propertyNames.Add("HeightType");
			_propertyNames.Add("ClientSize");
			_propertyNames.Add("ClientRectangle");
			_propertyNames.Add("Bounds");
			_propertyNames.Add("cursor");
			_propertyNames.Add("PositionAnchor");
			_propertyNames.Add("PositionAlignment");
			_propertyNames.Add("RightToLeft");
			_propertyNames.Add("zOrder");
			_propertyNames.Add("textAlign");
			_propertyNames.Add("Overflow");
			//
			_propertyNames.Add("id");
			_propertyNames.Add("tag");
			_propertyNames.Add("tagName");
			_propertyNames.Add("CustomValues");
			_propertyNames.Add("clientWidth");
			_propertyNames.Add("clientHeight");
			_propertyNames.Add("innerHTML");
			_propertyNames.Add("offsetHeight");
			_propertyNames.Add("offsetWidth");
			_propertyNames.Add("offsetLeft");
			_propertyNames.Add("offsetTop");
			_propertyNames.Add("scrollHeight");
			_propertyNames.Add("scrollLeft");
			_propertyNames.Add("scrollTop");
			_propertyNames.Add("scrollWidth");
			//
			_propertyNames.Add("Box");
		}
		public static string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "BackgroundImageFile") == 0)
			{
				return "style.backgroundImage";
			}
			else if (string.CompareOrdinal(name, "BackColor") == 0)
			{
				return "style.backgroundColor";
			}
			else if (string.CompareOrdinal(name, "Visible") == 0)
			{
				return "style.display";
			}
			else if (string.CompareOrdinal(name, "Text") == 0)
			{
				return "value";
			}
			if (string.CompareOrdinal(name, "ForeColor") == 0)
			{
				return "style.color";
			}
			else if (string.CompareOrdinal(name, "Left") == 0)
			{
				return "style.left";
			}
			else if (string.CompareOrdinal(name, "Top") == 0)
			{
				return "style.top";
			}
			else if (string.CompareOrdinal(name, "Width") == 0)
			{
				return "style.width";
			}
			else if (string.CompareOrdinal(name, "Height") == 0)
			{
				return "style.height";
			}
			if (string.CompareOrdinal(name, "Cursor") == 0)
			{
				return "style.cursor";
			}
			if (string.CompareOrdinal(name, "cursor") == 0)
			{
				return "style.cursor";
			}
			if (string.CompareOrdinal(name, "Float") == 0)
			{
				return "style.cssFloat";
			}
			if (string.CompareOrdinal(name, "RightToLeft") == 0)
			{
				return "dir";
			}
			if (string.CompareOrdinal(name, "zOrder") == 0)
			{
				return "style.zIndex";
			}
			if (string.CompareOrdinal(name, "textAlign") == 0)
			{
				return "style.textAlign";
			}
			if (string.CompareOrdinal(name, "Overflow") == 0)
			{
				return "style.overflow";
			}
			if (string.CompareOrdinal(name, "Checked") == 0)
			{
				return "checked";
			}
			return null;
		}
		public static string MapJavaScriptVallue(string name, string value, List<WebResourceFile> resourceFiles)
		{
			if (string.CompareOrdinal(name, "BackgroundImageFile") == 0)
			{
				StringBuilder sb;
				if (string.IsNullOrEmpty(value))
				{
					return "'none'";
				}
				if (value.StartsWith("'", StringComparison.Ordinal))
				{
					string sf = value.Substring(1);
					if (sf.EndsWith("'", StringComparison.Ordinal))
					{
						if (sf.Length == 1)
						{
							sf = "";
						}
						else
						{
							sf = sf.Substring(0, sf.Length - 1);
						}
					}
					if (string.IsNullOrEmpty(sf))
					{
						return "'none'";
					}
					if (File.Exists(sf))
					{
						bool b;
						resourceFiles.Add(new WebResourceFile(sf, WebResourceFile.WEBFOLDER_Images, out b));
						sb = new StringBuilder();
						sb.Append("\"url('");
						sb.Append(WebResourceFile.WEBFOLDER_Images);
						sb.Append("/");
						sb.Append(Path.GetFileName(sf));
						sb.Append("')\"");
						return sb.ToString();
					}
				}
				sb = new StringBuilder();
				sb.Append("\"url('\" + ");
				sb.Append(value);
				sb.Append(" + \"')\"");
				return sb.ToString();
			}
			else if (string.CompareOrdinal(name, "Visible") == 0)
			{
				if (string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'none'";
				}
				if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'block'";
				}
				if (string.IsNullOrEmpty(value))
					return "''";
				return string.Format(CultureInfo.InvariantCulture, "(({0})?\"block\":\"none\")", value);
			}
			if (string.CompareOrdinal(name, "RightToLeft") == 0)
			{
				if (string.Compare(value, "Yes", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'RTL'";
				}
				if (string.Compare(value, "No", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'LTR'";
				}
				if (string.IsNullOrEmpty(value))
					return "''";
				return string.Format(CultureInfo.InvariantCulture, "(({0})?\"RTL\":\"LTR\")", value);
			}
			if (string.CompareOrdinal(name, "disabled") == 0)
			{
				if (string.CompareOrdinal(value, "false") != 0 && string.CompareOrdinal(value, "true") != 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.isValueTrue({0})", value);
				}
			}
			return null;
		}
		public static MethodInfo[] GetWebClientMethods(Type t, bool isStatic)
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
			MethodInfo[] ret = t.GetMethods(flags);
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
		public static void WriteDataBindings(XmlNode node, ControlBindingsCollection DataBindings, Dictionary<string, string> nameMap)
		{
			if (DataBindings != null && DataBindings.Count > 0)
			{
				bool first = true;
				StringBuilder dbs = new StringBuilder();
				for (int i = 0; i < DataBindings.Count; i++)
				{
					if (DataBindings[i].DataSource != null)
					{
						if (nameMap != null && nameMap.ContainsKey(DataBindings[i].PropertyName))
						{
							if (string.IsNullOrEmpty(nameMap[DataBindings[i].PropertyName]))
							{
								continue;
							}
						}
						if (first)
						{
							first = false;
						}
						else
						{
							dbs.Append(";");
						}
						dbs.Append(DataBindings[i].BindingMemberInfo.BindingPath);
						dbs.Append(":");
						dbs.Append(DataBindings[i].BindingMemberInfo.BindingField);
						dbs.Append(":");
						if (nameMap != null && nameMap.ContainsKey(DataBindings[i].PropertyName))
						{
							dbs.Append(nameMap[DataBindings[i].PropertyName]);
						}
						else
						{
							dbs.Append(DataBindings[i].PropertyName);
						}
					}
				}
				if (!first) //has data binding
				{
					XmlUtil.SetAttribute(node, "jsdb", dbs.ToString());
				}
			}
		}

		public static void SetTextContentsToNode(XmlNode node, string text)
		{
			string txt = text.Replace("\r", "");
			string[] ss = txt.Split('\n');
			for (int i = 0; i < ss.Length; i++)
			{
				XmlNode ntxt = node.OwnerDocument.CreateTextNode(ss[i]);
				node.AppendChild(ntxt);
				if (i < ss.Length - 1)
				{
					XmlNode nbr = node.OwnerDocument.CreateElement("br");
					node.AppendChild(nbr);
				}
			}
		}
		public static string FormStringContents(string text)
		{
			text = text.Replace("\r", "\\r");
			text = text.Replace("\n", "\\n");
			text = text.Replace("'", "\\'");
			return text;
		}
		public static void CreateWebElementZOrder(int zorder, StringBuilder sb)
		{
			if (zorder > 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "z-index:{0}; ", zorder));
			}
		}
		public static void CreateWebElementCursor(EnumWebCursor c, StringBuilder sb, bool defaultCursor)
		{
			if (c != EnumWebCursor.@default || defaultCursor)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "cursor:{0}; ", c));
			}
		}
		public static void CreateElementAnchor(IWebClientControl c, XmlNode node)
		{
			bool bUseAnchor = false;
			StringBuilder sb = new StringBuilder();
			if ((c.PositionAnchor & AnchorStyles.Left) == AnchorStyles.Left)
			{
				sb.Append("left,");
			}
			if ((c.PositionAnchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
			{
				sb.Append("bottom,");
				bUseAnchor = true;
			}
			if ((c.PositionAnchor & AnchorStyles.Right) == AnchorStyles.Right)
			{
				sb.Append("right,");
				bUseAnchor = true;
			}
			if ((c.PositionAnchor & AnchorStyles.Top) == AnchorStyles.Top)
			{
				sb.Append("top,");
			}
			if (!bUseAnchor)
			{
				switch (c.PositionAlignment)
				{
					case ContentAlignment.BottomCenter:
						sb.Append("bottomcenter");
						break;
					case ContentAlignment.BottomLeft:
						sb.Append("leftbottom");
						break;
					case ContentAlignment.BottomRight:
						sb.Append("bottomright");
						break;
					case ContentAlignment.MiddleCenter:
						sb.Append("center");
						break;
					case ContentAlignment.MiddleLeft:
						sb.Append("leftcenter");
						break;
					case ContentAlignment.MiddleRight:
						sb.Append("centerright");
						break;
					case ContentAlignment.TopCenter:
						sb.Append("topcenter");
						break;
					case ContentAlignment.TopRight:
						sb.Append("topright");
						break;
				}
			}
			if (sb.Length > 0)
			{
				string anchor = sb.ToString();
				if (string.CompareOrdinal(anchor, "left,top,") != 0)
				{
					XmlUtil.SetAttribute(node, "anchor", anchor);
				}
			}
		}
		public static void SetWebControlAttributes(IWebClientControl wcc, XmlNode node)
		{
			if (!string.IsNullOrEmpty(wcc.className))
			{
				XmlUtil.SetAttribute(node, "class", wcc.className);
			}
		}
		public static void GetElementStyleSize(IWebClientControl wcc, out string widthStyle, out string heightStyle)
		{
			widthStyle = string.Empty;
			heightStyle = string.Empty;
			if (!(wcc is ICustomSize))
			{
				Control c = wcc as Control;
				if (wcc.WidthType != SizeType.AutoSize)
				{
					if (wcc.WidthType == SizeType.Absolute)
					{
						widthStyle = string.Format(CultureInfo.InvariantCulture, "{0}px; ", c.Width);
					}
					else
					{
						widthStyle = string.Format(CultureInfo.InvariantCulture, "{0}%; ", wcc.WidthInPercent);
					}
				}
				//
				if (wcc.HeightType != SizeType.AutoSize)
				{
					if (wcc.HeightType == SizeType.Absolute)
					{
						heightStyle = string.Format(CultureInfo.InvariantCulture, "{0}px; ", c.Height);
					}
					else
					{
						heightStyle = string.Format(CultureInfo.InvariantCulture, "{0}%; ", wcc.HeightInPercent);
					}
				}
			}
		}
		public static void CreateElementPosition(Control c, StringBuilder sb, EnumWebElementPositionType positionType)
		{
			IWebClientControl wcc = c as IWebClientControl;
			if (wcc != null)
			{
				if (wcc.textAlign != EnumTextAlign.left)
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "text-align:{0};", wcc.textAlign));
				}
				//
				string widthStyle, heightStyle;
				GetElementStyleSize(wcc, out widthStyle, out heightStyle);
				if (!string.IsNullOrEmpty(widthStyle))
				{
					sb.Append("width:");
					sb.Append(widthStyle);
				}
				if (!string.IsNullOrEmpty(heightStyle))
				{
					sb.Append("height:");
					sb.Append(heightStyle);
				}
				//
			}
			IWebBox wb = c as IWebBox;
			if (wb != null)
			{
				if (wb.Box.BorderRadius > 0)
				{
					sb.Append("border-radius:");
					sb.Append(wb.Box.BorderRadius.ToString(CultureInfo.InvariantCulture));
					sb.Append("px;");
				}
				if (wb.Box.BoxShadow > 0)
				{
					int bs = 1;
					if (wb.Box.BoxShadow > 2)
					{
						bs = wb.Box.BoxShadow / 2;
					}
					string cs;
					if (wb.Box.ShadowColor == Color.Empty)
					{
						cs = ObjectCreationCodeGen.GetColorString(Color.DarkGray);
					}
					else
					{
						cs = ObjectCreationCodeGen.GetColorString(wb.Box.ShadowColor);
					}
					sb.Append(string.Format(CultureInfo.InvariantCulture, "box-shadow:{0}px {0}px {1}px {2};", wb.Box.BoxShadow, bs, cs));
				}
				if (wb.Box.GradientEndColor != Color.Empty || wb.Box.GradientStartColor != Color.Empty)
				{
					string c1;
					if (wb.Box.GradientStartColor != Color.Empty)
					{
						c1 = ObjectCreationCodeGen.GetColorString(wb.Box.GradientStartColor);
					}
					else
					{
						c1 = ObjectCreationCodeGen.GetColorString(Color.White);
					}
					string c2;
					if (wb.Box.GradientEndColor != Color.Empty)
					{
						c2 = ObjectCreationCodeGen.GetColorString(wb.Box.GradientEndColor);
					}
					else
					{
						c2 = ObjectCreationCodeGen.GetColorString(Color.White);
					}
					sb.Append(string.Format(CultureInfo.InvariantCulture, "background-image:-ms-linear-gradient({0}deg, {1}, {2});", wb.Box.GradientAngle, c1, c2));
					sb.Append(string.Format(CultureInfo.InvariantCulture, "background-image:-o-linear-gradient({0}deg, {1}, {2});", wb.Box.GradientAngle, c1, c2));
					sb.Append(string.Format(CultureInfo.InvariantCulture, "background-image:-moz-linear-gradient({0}deg, {1}, {2});", wb.Box.GradientAngle, c1, c2));
					sb.Append(string.Format(CultureInfo.InvariantCulture, "background-image:-webkit-linear-gradient({0}deg, {1}, {2});", wb.Box.GradientAngle, c1, c2));
				}
			}
			IScrollableWebControl sw = c as IScrollableWebControl;
			if (sw != null)
			{
				if (sw.Overflow != EnumOverflow.visible)
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "overflow:{0};", sw.Overflow));
				}
			}
			if (c.RightToLeft == RightToLeft.Yes)
			{
				sb.Append("direction:rtl;");
			}
			if (positionType != EnumWebElementPositionType.Auto)
			{
				if (positionType == EnumWebElementPositionType.Absolute)
				{
					sb.Append("position: absolute; ");
					Point pointForm = c.Location;
					sb.Append("left:");
					sb.Append(pointForm.X.ToString(CultureInfo.InvariantCulture));
					sb.Append("px; ");
					sb.Append("top:");
					sb.Append(pointForm.Y.ToString(CultureInfo.InvariantCulture));
					sb.Append("px; ");
				}
				else if (positionType == EnumWebElementPositionType.Relative)
				{
					sb.Append("position: relative; ");
					sb.Append("left:");
					sb.Append(c.Left.ToString(CultureInfo.InvariantCulture));
					sb.Append("px; ");
					//
					sb.Append("top:");
					sb.Append(c.Top.ToString(CultureInfo.InvariantCulture));
					sb.Append("px; ");
				}
				else if (positionType == EnumWebElementPositionType.FloatLeft)
				{
					sb.Append("float:left; ");
				}
				else if (positionType == EnumWebElementPositionType.FloatRight)
				{
					sb.Append("float:right; ");
				}
				else if (positionType == EnumWebElementPositionType.FloatCenter)
				{
					sb.Append("float:center; ");
				}
			}
			IWebClientControl wc = c as IWebClientControl;
			if (wc != null)
			{
				if (wc.Opacity < 100)
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "opacity:{0};filter:alpha(opacity={1}); ", ((double)wc.Opacity / (double)100).ToString("#.##", CultureInfo.InvariantCulture), wc.Opacity));
				}
			}
		}
	}
}
