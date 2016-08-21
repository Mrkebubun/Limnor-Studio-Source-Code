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
using System.Reflection;
using Limnor.WebBuilder;
using ProgElements;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;
using System.Drawing;
using LimnorDesigner.MenuUtil;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.IO;
using System.Drawing.Design;

namespace LimnorDesigner.Web
{
	public enum EnumElementValidation { Fail, New, Pass }
	[UseParentObject]
	public abstract class HtmlElement_Base : IClass, IXmlNodeSerializable, IGuidIdentified
	{
		#region fields and constructors
		public static GetWebDataRepeaterHolder GetDataRepeater;
		private Guid _guid;
		private Image _iconImage;
		private ClassPointer _pagePointer;
		protected List<WebResourceFile> _resourceFiles;
		public HtmlElement_Base(ClassPointer owner)
		{
			_pagePointer = owner;
		}
		public HtmlElement_Base(ClassPointer owner, Guid guid)
		{
			_pagePointer = owner;
			_guid = guid;
		}
		#endregion

		#region Static Utility
		[Browsable(false)]
		[NotForProgramming]
		public static string GetGuidString(Guid g)
		{
			if (g == Guid.Empty)
			{
				return string.Empty;
			}
			return g.ToString("N", CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public static Type GetHtmlElementTypeByTagName(string tagname, string type, string subtype)
		{
			string typeName;
			if (string.CompareOrdinal(tagname, "fileupload") == 0)
			{
				return typeof(HtmlElement_fileupload);
			}
			else if (string.CompareOrdinal(tagname, "nav") == 0 && string.CompareOrdinal(type, "menubar") == 0)
			{
				return typeof(HtmlElement_menubar);
			}
			else if (string.CompareOrdinal(tagname, "ul") == 0 && string.CompareOrdinal(type, "tv") == 0)
			{
				return typeof(HtmlElement_tv);
			}
			else if (string.CompareOrdinal(tagname, "embed") == 0 && string.CompareOrdinal(type, "audio/x-pn-realaudio-plugin") == 0)
			{
				return typeof(HtmlElement_music);
			}
			else if (string.CompareOrdinal(tagname, "embed") == 0 && string.CompareOrdinal(type, "application/x-shockwave-flash") == 0)
			{
				return typeof(HtmlElement_flash);
			}
			else if (!string.IsNullOrEmpty(subtype))
			{
				typeName = string.Format(CultureInfo.InvariantCulture, "HtmlElement_{0}_{1}_{2}", tagname, type, subtype);
			}
			else if (!string.IsNullOrEmpty(type) && string.CompareOrdinal(tagname, "iframe") != 0 && string.CompareOrdinal(tagname, "select") != 0 && string.CompareOrdinal(tagname, "button") != 0)
			{
				typeName = string.Format(CultureInfo.InvariantCulture, "HtmlElement_{0}_{1}", tagname, type);
			}
			else
			{
				if (string.CompareOrdinal(tagname, "input") == 0)
				{
					typeName = string.Format(CultureInfo.InvariantCulture, "HtmlElement_{0}_text", tagname);
				}
				else
				{
					typeName = string.Format(CultureInfo.InvariantCulture, "HtmlElement_{0}", tagname);
				}
			}
			Type t = XmlUtil.GetKnownType(typeName);
			return t;
		}
		[Browsable(false)]
		[NotForProgramming]
		public static HtmlElement_Base CreateHtmlElementInstance(ClassPointer owner, string tag, string type, string subtype, Guid g, string id)
		{
			Type t = GetHtmlElementTypeByTagName(tag, type, subtype);
			if (t != null)
			{
				HtmlElement_Base heb;
				if (typeof(HtmlElement_head).Equals(t))
					heb = new HtmlElement_head(owner);
				else if (typeof(HtmlElement_script).Equals(t))
					heb = new HtmlElement_script(owner, g);
				else if (typeof(HtmlElement_link).Equals(t))
					heb = new HtmlElement_link(owner, g);
				else
					heb = Activator.CreateInstance(t, owner, id, g) as HtmlElement_Base;
				return heb;
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public static HtmlElement_Base CreateHtmlHeadElement(ClassPointer owner, string selectedObject)
		{
			if (!string.IsNullOrEmpty(selectedObject))
			{
				string[] ss = selectedObject.Split(',');
				if (ss.Length > 4)
				{
					string tagName = ss[0];
					string type = ss[1];
					string subtype = ss[2];
					string id = ss[3];
					string guid = ss[4];
					Dictionary<string, string> props = new Dictionary<string, string>();
					if (ss.Length > 5)
					{
						int i = 5;
						while (i < ss.Length)
						{
							string name = ss[i];
							string value = "";
							if (i < ss.Length - 1)
							{
								value = ss[i + 1];
							}
							props.Add(name, value);
							i += 2;
						}
					}
					return CreateHtmlHeadElement(owner, tagName, type, subtype, id, guid, props);
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public static HtmlElement_Base CreateHtmlHeadElement(ClassPointer owner, string tag, string type, string subtype, string id, string guid, Dictionary<string, string> properties)
		{
			HtmlElement_Base heb = null;
			Guid g = Guid.Empty;
			if (!string.IsNullOrEmpty(guid))
			{
				g = new Guid(guid);
			}
			heb = CreateHtmlElementInstance(owner, tag, type, subtype, g, id);
			if (heb != null)
			{
				heb.ImportProperties(properties);
			}
			return heb;
		}
		[Browsable(false)]
		[NotForProgramming]
		public static HtmlElement_Base CreateHtmlElement(ClassPointer owner, string selectedObject)
		{
			if (!string.IsNullOrEmpty(selectedObject))
			{
				string[] ss = selectedObject.Split(',');
				if (ss.Length > 4)
				{
					string tagName = ss[0];
					string type = ss[1];
					string subtype = ss[2];
					string id = ss[3];
					string guid = ss[4];
					Dictionary<string, string> props = new Dictionary<string, string>();
					if (ss.Length > 5)
					{
						int i = 5;
						while (i < ss.Length)
						{
							string name = ss[i];
							string value = "";
							if (i < ss.Length - 1)
							{
								value = ss[i + 1];
							}
							props.Add(name, value);
							i += 2;
						}
					}
					if (string.CompareOrdinal(tagName, "script") == 0 ||
						string.CompareOrdinal(tagName, "link") == 0)
					{
						return CreateHtmlHeadElement(owner, tagName, type, subtype, id, guid, props);
					}
					else
					{
						return CreateHtmlElement(owner, tagName, type, subtype, id, guid, props);
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public static HtmlElement_BodyBase CreateHtmlElement(ClassPointer owner, string tag, string type, string subtype, string id, string guid, Dictionary<string, string> properties)
		{
			HtmlElement_BodyBase heb = null;
			Guid g = Guid.Empty;
			if (!string.IsNullOrEmpty(guid))
			{
				g = new Guid(guid);
				if (g != Guid.Empty)
				{
					heb = owner.FindHtmlElementByGuid(g);
				}
			}
			if (heb == null)
			{
				if (string.CompareOrdinal(tag, "body") == 0)
				{
					heb = new HtmlElement_body(owner);
				}
				else
				{
					heb = CreateHtmlElementInstance(owner, tag, type, subtype, g, id) as HtmlElement_BodyBase;
				}
			}
			if (heb != null)
			{
				if (!string.IsNullOrEmpty(subtype))
				{
					HtmlElement_menubar menubar = heb as HtmlElement_menubar;
					if (menubar != null)
					{
						menubar.LoadMenuItems(subtype);
					}
				}
				heb.ImportProperties(properties);
				if (g != Guid.Empty)
				{
					owner.AddUsedHtmlElement(heb);
				}
			}
			return heb;
		}

		#endregion

		#region Methods - Not for programming
		[Browsable(false)]
		[NotForProgramming]
		public void SetWebProperty(string name, string value)
		{
			if (_pagePointer != null)
			{
				WebPage p = _pagePointer.ObjectInstance as WebPage;
				if (p != null)
				{
					p.SetWebProperty(name, value);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual bool IsSameElement(HtmlElement_Base e)
		{
			if (this.ElementGuid != Guid.Empty)
			{
				return (this.ElementGuid == e.ElementGuid);
			}
			if (e.ElementGuid != Guid.Empty)
			{
				return false;
			}
			return string.CompareOrdinal(this.tagName, e.tagName) == 0;
		}
		/// <summary>
		/// all such properties must be of string. Real types may be converted by setters.
		/// </summary>
		/// <param name="properties"></param>
		[Browsable(false)]
		[NotForProgramming]
		public void ImportProperties(Dictionary<string, string> properties)
		{
			foreach (KeyValuePair<string, string> kv in properties)
			{
				PropertyInfo pif = this.GetType().GetProperty(kv.Key);
				if (pif != null)
				{
					object v = kv.Value;
					if (!string.IsNullOrEmpty(kv.Value))
					{
						if (pif.PropertyType.IsEnum)
						{
							v = Enum.Parse(pif.PropertyType, kv.Value.ToLower());
						}
						else if (pif.PropertyType.IsArray)
						{
							Type ti = pif.PropertyType.GetElementType();
							string[] ss = kv.Value.Split('_');
							Array ay = Array.CreateInstance(ti, ss.Length);
							for (int i = 0; i < ss.Length; i++)
							{
								bool b;
								ay.SetValue(VPLUtil.ConvertObject(ss[i], ti, out b), i);
							}
							v = ay;
						}
					}
					pif.SetValue(this, v, null);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual EnumElementValidation Validate(Form caller)
		{
			if (ElementGuid == Guid.Empty)
			{
				Guid g = Guid.NewGuid();
				_guid = g;
				return EnumElementValidation.New;
			}
			else
			{
			}
			return EnumElementValidation.Pass;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetGuid(Guid g)
		{
			_guid = g;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetPageOwner(ClassPointer root)
		{
			_pagePointer = root;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			return this.tagName;
		}
		#endregion

		#region Properties
		[XmlIgnore]
		[NotForProgramming]
		[ReadOnly(true)]
		[Browsable(false)]
		public bool Changed
		{
			get;
			set;
		}

		[NotForProgramming]
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				return _pagePointer;
			}
		}

		[NotForProgramming]
		[Browsable(false)]
		public WebPage Page
		{
			get
			{
				if (_pagePointer != null)
				{
					return _pagePointer.ObjectInstance as WebPage;
				}
				return null;
			}
		}

		[NotForProgramming]
		[Browsable(false)]
		public string WebPhysicalFolder
		{
			get
			{
				if (_pagePointer != null)
				{
					Form f = null;
					if (_pagePointer != null)
					{
						f = _pagePointer.ObjectInstance as Form;
					}
					return _pagePointer.Project.WebPhysicalFolder(f);
				}
				return string.Empty;
			}
		}

		[Browsable(false)]
		[NotForProgramming]
		public virtual Guid ElementGuid
		{
			get
			{
				return _guid;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string ElementGuidString
		{
			get
			{
				return HtmlElement_Base.GetGuidString(ElementGuid);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual string ImageKey
		{
			get
			{
				string imgKey = tagName;
				if (imgKey.StartsWith("h", StringComparison.Ordinal))
				{
					if (imgKey.Length == 2 && !imgKey.EndsWith("r", StringComparison.Ordinal))
					{
						for (int i = 1; i <= 6; i++)
						{
							if (string.CompareOrdinal(imgKey, string.Format(CultureInfo.InvariantCulture, "h{0}", i)) == 0)
							{
								imgKey = "h";
								break;
							}
						}
					}
				}
				return imgKey;
			}
		}
		#endregion

		#region Web properties
		[Browsable(false)]
		[NotForProgramming]
		public virtual string ServerTypeName
		{
			get
			{
				return null;
			}
		}
		[WebClientMember]
		[ParenthesizePropertyName(true)]
		[Description("tag name of the html element")]
		public abstract string tagName { get; }

		[WebClientMember]
		[ParenthesizePropertyName(true)]
		[Description("Gets the id of the element")]
		public abstract string id { get; }

		#endregion

		#region IObjectIdentity Members
		[Browsable(false)]
		[NotForProgramming]
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			HtmlElement_Base heb = objectIdentity as HtmlElement_Base;
			if (heb == null)
			{
				HtmlElementPointer hep = objectIdentity as HtmlElementPointer;
				if (hep != null)
				{
					heb = hep.Element;
				}
			}
			if (heb != null)
			{
				return this.IsSameElement(heb);
			}
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public IObjectIdentity IdentityOwner
		{
			get { return _pagePointer; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsStatic
		{
			get { return false; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Class; }
		}

		#endregion

		#region ICloneable Members
		[Browsable(false)]
		[NotForProgramming]
		public virtual object Clone()
		{
			object heb = Activator.CreateInstance(this.GetType(), _pagePointer, _guid);
			return heb;
		}
		#endregion

		#region IWebClientComponent Members
		[Browsable(false)]
		[NotForProgramming]
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return WebPageCompilerUtility.GetWebClientMethods(this.GetType(), isStatic);
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver))
			{
			}
			else
			{
				if (string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}();\r\n", this.GetJavaScriptReferenceCode(code), methodName));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}.{2}();\r\n", returnReceiver, this.GetJavaScriptReferenceCode(code), methodName));
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual string GetJavaScriptReferenceCode(StringCollection method)
		{
			return WebPageCompilerUtility.JsCodeRef(CodeName);
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string MapJavaScriptCodeName(string name)
		{
			string v;
			v = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (!string.IsNullOrEmpty(v))
			{
				return v;
			}
			return name;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string MapJavaScriptVallue(string name, string value)
		{
			if (_resourceFiles == null)
				_resourceFiles = new List<WebResourceFile>();
			string v;
			if (string.CompareOrdinal(name, "Cursor") == 0)
			{
				if (string.IsNullOrEmpty(value))
				{
					return "default";
				}
				return value.Replace('_', '-');
			}
			v = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (!string.IsNullOrEmpty(v))
				return v;
			return value;
		}

		#endregion

		#region IWebResourceFileUser Members
		[Browsable(false)]
		[NotForProgramming]
		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		#endregion

		#region IWebClientSupport Members
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "toString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').toString()", this.CodeName);
			}
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			string js = WebPageCompilerUtility.JsCodeRef(CodeName);
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(js, propertyName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (string.CompareOrdinal("CustomValues", propertyName) != 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", js, propertyName);
			}
			return null;
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public virtual void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			string g = XmlUtil.GetAttribute(node, XmlTags.XMLATT_guid);
			if (!string.IsNullOrEmpty(g))
			{
				_guid = new Guid(g);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_guid, this.ElementGuidString);
		}

		#endregion

		#region IClass Members
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public Image ImageIcon
		{
			get
			{
				if (_iconImage == null)
				{
					string imgKey = ImageKey;
					if (string.CompareOrdinal(imgKey, "body") == 0)
					{
						_iconImage = Resources._body.ToBitmap();
					}
					else if (string.CompareOrdinal(imgKey, "html") == 0)
					{
						_iconImage = Resources._html.ToBitmap();
					}
					else if (string.CompareOrdinal(imgKey, "unknown") == 0)
					{
						_iconImage = Resources._htmlElement.ToBitmap();
					}
					else
					{
						string imgFile = Path.Combine(this.WebPhysicalFolder, string.Format(CultureInfo.InvariantCulture, "libjs\\html_{0}.png", imgKey));
						if (File.Exists(imgFile))
						{
							_iconImage = Image.FromFile(imgFile);
						}
						if (_iconImage == null)
						{
							_iconImage = Resources._html.ToBitmap();
						}
					}
				}
				return _iconImage;
			}
			set
			{
				_iconImage = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public Type VariableLibType
		{
			get { return this.GetType(); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public ClassPointer VariableCustomType
		{
			get { return null; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public IClassWrapper VariableWrapperType
		{
			get { return null; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public uint DefinitionClassId
		{
			get
			{
				if (_pagePointer != null)
					return _pagePointer.ClassId;
				return 0;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public IClass Host
		{
			get { return _pagePointer; }
		}

		#endregion

		#region IObjectPointer Members
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public abstract string ReferenceName { get; }

		[Browsable(false)]
		[NotForProgramming]
		public string PropertyName
		{
			get
			{
				return ReferenceName;
			}
		}

		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public IObjectPointer Owner
		{
			get
			{
				return _pagePointer;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public object ObjectDebug
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string DisplayName
		{
			get { return ToString(); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string LongDisplayName
		{
			get { return ToString(); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public virtual string ExpressionDisplay
		{
			get
			{
				return tagName;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.All || target == EnumObjectSelectType.Object);
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string ObjectKey
		{
			get
			{
				return ElementGuidString;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string TypeString
		{
			get { return this.GetType().FullName; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public virtual bool IsValid
		{
			get { return true; }
		}
		[Browsable(false)]
		[NotForProgramming]
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (method.IsStatic)
			{
				return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ServerTypeName), this.CodeName);
			}
			else
			{
				return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.CodeName);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{

		}

		[Browsable(false)]
		[NotForProgramming]
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public EnumWebRunAt RunAt
		{
			get { return EnumWebRunAt.Client; }
		}

		#endregion

		#region ISerializerProcessor Members
		[Browsable(false)]
		[NotForProgramming]
		public virtual void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion

		#region IPropertyPointer Members
		[Browsable(false)]
		[NotForProgramming]
		public abstract string CodeName
		{
			get;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public Type ObjectType
		{
			get
			{
				return this.GetType();
			}
			set
			{

			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public object ObjectInstance
		{
			get
			{
				return this;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public IPropertyPointer PropertyOwner
		{
			get { return _pagePointer; }
		}

		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public ulong WholeId
		{
			get { return DesignUtil.MakeDDWord(MemberId, ClassId); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public uint ClassId
		{
			get { return _pagePointer.ClassId; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public uint MemberId
		{
			get { return (uint)ElementGuid.GetHashCode(); }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public virtual string Name
		{
			get { return tagName; }
		}

		#endregion


	}
	public abstract class HtmlElement_BodyBase : HtmlElement_Base, IWebClientComponent, IBindableComponent, IDevClassReferencer, ICustomTypeDescriptor, IDelayedInitialize, IHtmlElement
	{
		#region fields and constructors
		private ControlBindingsCollection _databindings;
		private BindingContext _bindingContext;
		//
		public HtmlElement_BodyBase(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_BodyBase(ClassPointer owner, Guid guid)
			: base(owner, guid)
		{
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
		[NotForProgramming]
		public virtual bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual string CreateWebFileAddress(string localFilePath, string parameterName)
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
		private string _tag = null;
		[Bindable(true)]
		[WebClientMember]
		[Description("Gets and sets data associated with the element")]
		public string tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
				if (!string.IsNullOrEmpty(_tag))
				{
					if (this.ElementGuid == Guid.Empty)
					{
						this.RootPointer.OnUseHtmlElement(this);
					}
				}
			}
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
		#region Web Properties -- for programming, not for setting

		[Description("Returns the viewable width of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int clientWidth
		{
			get { return 0; }
		}
		[Description("Returns the viewable height of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int clientHeight
		{
			get { return 0; }
		}
		[Bindable(true)]
		[Description("Sets or returns the HTML contents (+text) of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string innerHTML
		{
			get;
			set;
		}
		[Description("Sets or returns the class attribute of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string className
		{
			get;
			set;
		}
		[Description("Sets or returns an accesskey for an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string accessKey
		{
			get;
			set;
		}
		[Description("Sets or returns background color for an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public Color BackColor
		{
			get;
			set;
		}
		[Description("Sets or returns the cursor for an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public EnumHtmlCursor Cursor
		{
			get;
			set;
		}
		[Description("Sets or returns color for an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public Color ForeColor
		{
			get;
			set;
		}
		[Description("Sets or returns the text-direction for an element.")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public EnumTextDir dir
		{
			get;
			set;
		}
		[Description("Sets or returns the language code for an element. For example, en-US for US English.")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string lang
		{
			get;
			set;
		}
		[Description("Returns the height of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int offsetHeight
		{
			get { return 0; }
		}
		[Description("Returns the width of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int offsetWidth
		{
			get { return 0; }
		}
		[Description("Returns the horizontal offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int offsetLeft
		{
			get { return 0; }
		}
		[Description("Returns the vertical offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int offsetTop
		{
			get { return 0; }
		}
		[Description("Returns the entire height of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int scrollHeight
		{
			get { return 0; }
		}
		[Description("Returns the distance between the actual left edge of an element and its left edge currently in view")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int scrollLeft
		{
			get { return 0; }
		}
		[Description("Returns the distance between the actual top edge of an element and its top edge currently in view")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int scrollTop
		{
			get { return 0; }
		}
		[Description("Returns the entire width of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int scrollWidth
		{
			get { return 0; }
		}
		[Description("Sets or returns the style attribute of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string style
		{
			get;
			set;
		}
		[Description("Sets or returns the title attribute of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public string title
		{
			get;
			set;
		}
		[Description("Sets or returns the tab order of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int tabIndex
		{
			get;
			set;
		}

		[Description("Sets or returns the visibility of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public bool Visible
		{
			get;
			set;
		}

		[Description("Sets or returns the opacity of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int Opacity
		{
			get;
			set;
		}

		[Description("Sets or returns the z-order of an element")]
		[Browsable(false)]
		[XmlIgnore]
		[WebClientMember]
		public int zOrder
		{
			get;
			set;
		}
		#endregion
		#region Web Methods
		[Description("Converts an element to a string")]
		[WebClientMember]
		public string toString()
		{
			return string.Empty;
		}
		[Description("It removes the focus from the element")]
		[WebClientMember]
		public void blur()
		{
		}
		[Description("It gives focus to the element")]
		[WebClientMember]
		public void focus()
		{
		}
		[Description("It simulates a mouse-click on the element")]
		[WebClientMember]
		public void click()
		{
		}
		[Description("It prints the element")]
		[WebClientMember]
		public void Print()
		{
		}
		#endregion
		#region Web Events
		[Description("It occurs when an element loses focus")]
		[WebClientMember]
		public event WebSimpleEventHandler onblur { add { } remove { } }
		[Description("It occurs when an element gets focus")]
		[WebClientMember]
		public event WebSimpleEventHandler onfocus { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onclick { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler ondblclick { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onmousedown { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onmousemove { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onmouseover { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onmouseout { add { } remove { } }
		[WebClientMember]
		public event WebMouseEventHandler onmouseup { add { } remove { } }
		[WebClientMember]
		public event WebKeyEventHandler onkeydown { add { } remove { } }
		[WebClientMember]
		public event WebKeyEventHandler onkeypress { add { } remove { } }
		[WebClientMember]
		public event WebKeyEventHandler onkeyup { add { } remove { } }
		#endregion
		#region Properties

		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string ExpressionDisplay
		{
			get
			{
				return tagName;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string ReferenceName
		{
			get { return tagName; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string Name
		{
			get { return tagName; }
		}
		/// <summary>
		/// when it is used in the current html icon in event path, it requires it to be valid for generating menus
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override bool IsValid
		{
			get
			{
				if (ElementGuid == Guid.Empty)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "ElementGuid is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				if (RootPointer == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "RootPointer is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				if (RootPointer.FindHtmlElementByGuid(ElementGuid) != null)
				{
					return true;
				}
				else
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "HtmlElement not found for {2} for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, ElementGuid);
				}
				return false;
			}
		}
		private Color _hbck = Color.Empty;
		[Description("Gets and sets a color indicating the background color when mouse is over the element. Set it to Transparent to not use this property.")]
		public Color HighlightBackColor
		{
			get
			{
				return _hbck;
			}
			set
			{
				_hbck = value;
				if (_hbck != Color.Empty && _hbck != Color.Transparent)
				{
					if (this.ElementGuid == Guid.Empty)
					{
						ClassPointer cp = this.Owner as ClassPointer;
						if (cp != null)
						{
							cp.UseHtmlElement(this, null);
						}
					}
				}
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		[NotForProgramming]
		public virtual void CreateContextMenu(Menu.MenuItemCollection mnu, EventHandler handler)
		{
		}
		[Browsable(false)]
		[NotForProgramming]
		public override bool IsSameElement(HtmlElement_Base e)
		{
			HtmlElement_BodyBase eb = e as HtmlElement_BodyBase;
			if (eb == null)
			{
				return false;
			}
			if (this.ElementGuid != Guid.Empty)
			{
				return (this.ElementGuid == e.ElementGuid);
			}
			if (e.ElementGuid != Guid.Empty)
			{
				return false;
			}
			return string.CompareOrdinal(this.tagName, e.tagName) == 0;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			return this.tagName;
		}
		#endregion
		#region Compile
		[Browsable(false)]
		[NotForProgramming]
		public static void CompilerCreateActionJavaScript(string codeOwner, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "Print") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.print({0});\r\n", codeOwner));
			}
			else if (string.IsNullOrEmpty(returnReceiver))
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}();\r\n", codeOwner, methodName));
			}
			else
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}.{2}();\r\n", returnReceiver, codeOwner, methodName));
			}
		}
		#endregion

		#region IBindableComponent Members

		[XmlIgnore]
		[Browsable(false)]
		[NotForProgramming]
		[ReadOnly(true)]
		public BindingContext BindingContext
		{
			get
			{
				if (_bindingContext == null)
				{
					if (DataBindings.Count > 0)
					{
						for (int i = 0; i < DataBindings.Count; i++)
						{
							if (DataBindings[i] != null)
							{
								IBindingContextHolder bch = DataBindings[i].DataSource as IBindingContextHolder;
								if (bch != null)
								{
									_bindingContext = bch.BindingContext;
									return _bindingContext;
								}
							}
						}
					}
					if (Page != null)
					{
						_bindingContext = Page.BindingContext;
					}
					else
					{
						_bindingContext = new BindingContext();
					}
				}
				return _bindingContext;
			}
			set
			{
				_bindingContext = value;
			}
		}
		[NotForProgramming]
		[ParenthesizePropertyName(true)]
		[XmlIgnore]
		public ControlBindingsCollection DataBindings
		{
			get
			{
				if (_databindings == null)
				{
					_databindings = new ControlBindingsCollection(this);
				}
				return _databindings;
			}
		}

		#endregion

		#region IComponent Members
		[Browsable(false)]
		[NotForProgramming]
		public event EventHandler Disposed;
		private ISite _site;
		[ReadOnly(true)]
		[Browsable(false)]
		[NotForProgramming]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				if (_site == null)
				{
					IContainer cr = null;
					IComponent c = Page as IComponent;
					if (c != null && c.Site != null)
					{
						cr = c.Site.Container;
					}
					_site = new XSite(this, cr);
					_site.Name = this.id;
				}
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
		[NotForProgramming]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XML_ATTR_tag = "tag";
		const string XML_CustValues = "CustomValues";
		const string XML_DataBindings = "DataBindings";
		const string XML_ATTR_PropID = "propertyID";
		const string XML_ATTR_member = "member";
		const string XML_Ref = "Reference";
		const string XML_ATTR_hbk = "hbk";
		[Browsable(false)]
		[NotForProgramming]
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			string hbkstr = XmlUtil.GetAttribute(node, XML_ATTR_hbk);
			if (!string.IsNullOrEmpty(hbkstr))
			{
				this.HighlightBackColor = VPLUtil.GetColor(hbkstr);
			}
			this.tag = XmlUtil.GetAttribute(node, XML_ATTR_tag);
			XmlNode nd = node.SelectSingleNode(XML_CustValues);
			if (nd != null)
			{
				_customValues = new WebClientValueCollection(this);
				serializer.ReadObjectFromXmlNode(nd, _customValues, _customValues.GetType(), this);
			}
			XmlNodeList nds = node.SelectNodes(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", XML_DataBindings, XmlTags.XML_Item));
			if (nds != null && nds.Count > 0)
			{
				XmlObjectReader xr = serializer as XmlObjectReader;
				if (xr != null)
				{
					xr.AddDelayedInitializer(this, node);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			if (this.HighlightBackColor != Color.Empty)
			{
				XmlUtil.SetAttribute(node, XML_ATTR_hbk, VPLUtil.GetColorString(this.HighlightBackColor));
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_hbk);
			}
#if NET35
			if(string.IsNullOrEmpty(this.tag))
#else
			if (!string.IsNullOrWhiteSpace(this.tag))
#endif
			{
				XmlUtil.SetAttribute(node, XML_ATTR_tag, this.tag);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XML_ATTR_tag);
			}
			if (_customValues != null && _customValues.Count > 0)
			{
				XmlNode nd = serializer.CreateSingleNewElement(node, XML_CustValues);
				nd.RemoveAll();
				serializer.WriteObjectToNode(nd, _customValues);
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XML_CustValues);
				if (nd != null)
				{
					node.RemoveChild(nd);
				}
			}
			if (_databindings != null && _databindings.Count > 0)
			{
				XmlNode nd = serializer.CreateSingleNewElement(node, XML_DataBindings);
				nd.RemoveAll();
				foreach (Binding bd in _databindings)
				{
					XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
					nd.AppendChild(item);
					if (!string.IsNullOrEmpty(bd.PropertyName))
					{
						IComponent ic = bd.DataSource as IComponent;
						if (ic != null && ic.Site != null && !string.IsNullOrEmpty(ic.Site.Name))
						{
							if (bd.BindingMemberInfo != null && !string.IsNullOrEmpty(bd.BindingMemberInfo.BindingMember))
							{
								string mem = bd.BindingMemberInfo.BindingMember;
								if (mem.IndexOf('.') < 0)
								{
									if (!string.IsNullOrEmpty(bd.BindingMemberInfo.BindingField))
									{
										mem = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", bd.BindingMemberInfo.BindingMember, bd.BindingMemberInfo.BindingField);
									}
								}
								XmlUtil.SetAttribute(item, XML_ATTR_PropID, bd.PropertyName);
								XmlUtil.SetAttribute(item, XML_ATTR_member, mem);
								XmlNode refNode = serializer.CreateSingleNewElement(item, XML_Ref);
								XmlUtil.SetNameAttribute(refNode, ic.Site.Name);
							}
						}
					}
				}
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XML_DataBindings);
				if (nd != null)
				{
					node.RemoveChild(nd);
				}
			}
		}

		#endregion

		#region IDevClassReferencer
		private IDevClass _class;
		[Browsable(false)]
		[NotForProgramming]
		public void SetDevClass(IDevClass c)
		{
			_class = c;
		}
		[Browsable(false)]
		[NotForProgramming]
		public IDevClass GetDevClass()
		{
			if (_class == null)
			{
				if (Page != null)
				{
					return Page.GetDevClass();
				}
			}
			return _class;
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
			return WebClientValueCollection.GetWebClientProperties(this, null, attributes);
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
		#region IDelayedInitialize
		public virtual void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			XmlNodeList nds = objectNode.SelectNodes(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", XML_DataBindings, XmlTags.XML_Item));
			if (nds != null && nds.Count > 0)
			{
				_databindings = new ControlBindingsCollection(this);
				foreach (XmlNode nd1 in nds)
				{
					XmlNode refNode = nd1.SelectSingleNode(XML_Ref);
					if (refNode != null)
					{
						string refName = XmlUtil.GetNameAttribute(refNode);
						if (!string.IsNullOrEmpty(refName))
						{
							IComponent ic = RootPointer.GetComponentByName(refName);
							if (ic != null)
							{
								string prop = XmlUtil.GetAttribute(nd1, XML_ATTR_PropID);
								if (!string.IsNullOrEmpty(prop))
								{
									string mem = XmlUtil.GetAttribute(nd1, XML_ATTR_member);
									if (!string.IsNullOrEmpty(mem))
									{
										Binding bd = new Binding(prop, ic, mem);
										_databindings.Add(bd);
									}
								}
							}
						}
					}
				}
			}
		}

		public void SetReader(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{

		}
		#endregion

		#region IHtmlElement Members
		public virtual void OnSetProperty(string propName)
		{
			if (this.ElementGuid == Guid.Empty)
			{
				this.RootPointer.OnUseHtmlElement(this);
			}
		}
		#endregion
	}
	public abstract class HtmlElement_ItemBase : HtmlElement_BodyBase
	{
		#region fields and constructors
		private string _id;
		public HtmlElement_ItemBase(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_ItemBase(ClassPointer owner, string id, Guid guid)
			: base(owner, guid)
		{
			_id = id;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string ExpressionDisplay
		{
			get
			{
				if (!string.IsNullOrEmpty(_id))
					return id;
				return tagName;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string ReferenceName
		{
			get { return _id; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override string Name
		{
			get { return _id; }
		}
		/// <summary>
		/// when it is used in the current html icon in event path, it requires it to be valid for generating menus
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public override bool IsValid
		{
			get
			{
				if (ElementGuid != Guid.Empty && !string.IsNullOrEmpty(_id))
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(ElementGuid={2}, _id={3}) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, ElementGuid, _id);
				return false;
			}
		}

		[WebClientMember]
		[ParenthesizePropertyName(true)]
		[Description("Gets the id of the element")]
		public override string id
		{
			get
			{
				return _id;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string CodeName
		{
			get
			{
				return _id;
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_id = XmlUtil.GetAttribute(node, XmlTags.XMLATT_ComponentID);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ComponentID, this.id);
		}

		#endregion
		#region Methods
		[Browsable(false)]
		[NotForProgramming]
		public override EnumElementValidation Validate(Form caller)
		{
			if (ElementGuid == Guid.Empty)
			{
				if (Page.EditorStarted)
				{
					base.Validate(caller);
					//it is the currently selected html element
					if (string.IsNullOrEmpty(_id))
					{
						string id = Page.CreateOrGetIdForCurrentElement(this.ElementGuidString);
						if (string.IsNullOrEmpty(id))
						{
							MessageBox.Show(caller, "Error getting html element id. It could be that Html element selection changed.", "Create Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return EnumElementValidation.Fail;
						}
						else
						{
							_id = id;
							return EnumElementValidation.New;
						}
					}
					else
					{
						if (Page.SetGuidById(_id, this.ElementGuidString))
						{
							return EnumElementValidation.New;
						}
						else
						{
							MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error setting guid for html element {0}. It could be that Html element has been deleted.", _id), "Create Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return EnumElementValidation.Fail;
						}
					}
				}
				else
				{
					return EnumElementValidation.Fail;
				}
			}
			else
			{
				//it is a previously used element
				if (Page.EditorStarted)
				{
					//the id might be changed. It is a double-check because id change should have been passed to the IDE element
					string id = Page.getIdByGuid(this.ElementGuidString);
					if (string.IsNullOrEmpty(id))
					{
						MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Html element [{0}] (id:[{1}]) was not found. It may be deleted.", this.tagName, _id), "Create Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return EnumElementValidation.Fail;
					}
					else
					{
						_id = id;
					}
				}
			}
			return EnumElementValidation.Pass;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override bool IsSameElement(HtmlElement_Base e)
		{
			HtmlElement_ItemBase eb = e as HtmlElement_ItemBase;
			if (eb == null)
			{
				return false;
			}
			if (this.ElementGuid != Guid.Empty)
			{
				return (this.ElementGuid == e.ElementGuid);
			}
			if (e.ElementGuid != Guid.Empty)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(this.id))
			{
				return string.CompareOrdinal(this.id, eb.id) == 0;
			}
			if (!string.IsNullOrEmpty(eb.id))
			{
				return false;
			}
			return string.CompareOrdinal(this.tagName, e.tagName) == 0;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string GetJavaScriptReferenceCode(StringCollection code, string attributeName, string[] parameters)
		{
			return string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}')", _id);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(_id))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}({1})", this.tagName, _id);
			}
			return this.tagName;
		}


		[Browsable(false)]
		[NotForProgramming]
		public void SetId(string newId)
		{
			_id = newId;
		}
		#endregion
		#region ICloneable Members
		[Browsable(false)]
		[NotForProgramming]
		public override object Clone()
		{
			object heb = Activator.CreateInstance(this.GetType(), this.RootPointer, this.id, this.ElementGuid);
			return heb;
		}
		#endregion
	}
	public delegate bool WebSimpleEventHandler(HtmlElement_BodyBase sender);
	public delegate bool WebMouseEventHandler(HtmlElement_BodyBase sender, WebMouseEventArgs e);
	public delegate bool WebKeyEventHandler(HtmlElement_BodyBase sender, WebKeyEventArgs e);
	public delegate IWebDataRepeater GetWebDataRepeaterHolder(HtmlElement_Base element);
}
