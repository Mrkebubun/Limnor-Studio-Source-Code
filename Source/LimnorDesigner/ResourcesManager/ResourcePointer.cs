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
using System.Xml;
using ProgElements;
using XmlSerializer;
using System.CodeDom;
using MathExp;
using XmlUtility;
using VPL;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using Limnor.WebBuilder;

namespace LimnorDesigner.ResourcesManager
{
	abstract public class ResourcePointer : IObjectPointer, ISerializationProcessor
	{
		#region fields and constructors
		const string XML_VALUES = "Values";
		private ProjectResources _owner;
		private XmlNode _xmlNode;
		private UInt32 _id;
		private string _name;
		private string _defaultValue; //string or file name
		private Dictionary<string, string> _localizedResources;
		protected const int VALUE_DISPLAY_LEN = 20;
		public ResourcePointer()
		{
		}
		public ResourcePointer(ProjectResources owner)
		{
			_owner = owner;
		}
		#endregion

		#region Methods
		public virtual void AddFileContextMenus(List<MenuItem> l) { }
		public abstract void OnPaintPictureBox(PaintEventArgs e, string languageName);
		protected abstract void OnSaveValue();
		public abstract void OnSelected(TextBoxResEditor textBoxDefault, TextBoxResEditor textBoxLocal, PictureBoxResEditor pictureBoxDefault, PictureBoxResEditor pictureBoxLocal, CultureInfo c);
		public abstract bool SelectResourceFile(Form caller, string languageName);
		public virtual void ClearResource(string languageName)
		{
			if (string.IsNullOrEmpty(languageName))
			{
				_defaultValue = string.Empty;
			}
			else
			{
				if (_localizedResources != null)
				{
					if (_localizedResources.ContainsKey(languageName))
					{
						_localizedResources.Remove(languageName);
					}
				}
			}
		}
		public virtual void ClearResource()
		{
			ClearResource(_owner.DesignerLanguageName);
		}
		//called by designer when a mapped property is chaned
		public virtual bool SetValue(object value)
		{
			if (value is string)
			{
				bool changed = false;
				string s = (string)value;
				if (string.IsNullOrEmpty(_owner.DesignerLanguageName))
				{
					if (string.CompareOrdinal(_defaultValue, s) != 0)
					{
						_defaultValue = s;
						changed = true;
					}
				}
				else
				{
					if (_localizedResources == null)
					{
						_localizedResources = new Dictionary<string, string>();
					}
					if (_localizedResources.ContainsKey(_owner.DesignerLanguageName))
					{
						if (string.CompareOrdinal(_localizedResources[_owner.DesignerLanguageName], s) != 0)
						{
							_localizedResources[_owner.DesignerLanguageName] = s;
							changed = true;
						}
					}
					else
					{
						_localizedResources.Add(_owner.DesignerLanguageName, s);
						changed = true;
					}
				}
				if (changed)
				{
					IsChanged = true;
					return Save();
				}
			}
			return false;
		}
		public bool Save()
		{
			if (IsChanged)
			{
				XmlNode valuesNode = XmlUtil.CreateSingleNewElement(XmlData, XML_VALUES);
				valuesNode.RemoveAll();
				XmlNode vnode = valuesNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
				valuesNode.AppendChild(vnode);
				XmlUtil.SetNameAttribute(vnode, string.Empty);
				vnode.InnerText = _defaultValue;
				if (_localizedResources != null)
				{
					foreach (KeyValuePair<string, string> kv in _localizedResources)
					{
						vnode = valuesNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
						valuesNode.AppendChild(vnode);
						XmlUtil.SetNameAttribute(vnode, kv.Key);
						vnode.InnerText = kv.Value;
					}
				}
			}
			OnSaveValue();
			if (IsChanged)
			{
				IsChanged = false;
				return true;
			}
			return false;
		}
		public void UpdateResourceName(string name)
		{
			_name = name;
			if (XmlData != null)
			{
				XmlUtil.SetNameAttribute(XmlData, name);
				XmlNode nd = XmlData.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@name='Name']",
				 XmlTags.XML_PROPERTY));
				if (nd != null)
				{
					nd.InnerText = name;
				}
			}
		}
		public void SetCurrentResourceString(string stringValue)
		{
			SetResourceString(_owner.DesignerLanguageName, stringValue);
		}
		public void SetResourceString(string languageName, string stringValue)
		{
			if (string.IsNullOrEmpty(languageName))
			{
				_defaultValue = stringValue;
			}
			else
			{
				if (_localizedResources == null)
				{
					_localizedResources = new Dictionary<string, string>();
				}
				if (_localizedResources.ContainsKey(languageName))
				{
					_localizedResources[languageName] = stringValue;
				}
				else
				{
					_localizedResources.Add(languageName, stringValue);
				}
			}
		}

		public string GetResourceString(string languageName)
		{
			if (string.IsNullOrEmpty(languageName))
			{
				if (_defaultValue == null)
				{
					return string.Empty;
				}
				return _defaultValue;
			}
			if (_localizedResources != null)
			{
				if (_localizedResources.ContainsKey(languageName))
				{
					return _localizedResources[languageName];
				}
				if (string.CompareOrdinal("zh", languageName) == 0)
				{
					if (_localizedResources.ContainsKey("zh-CHT"))
					{
						return _localizedResources["zh-CHT"];
					}
				}
			}
			return string.Empty;
		}
		public bool HasResource(string languageName)
		{
			if (string.IsNullOrEmpty(languageName))
			{
				return true;
			}
			if (_localizedResources != null)
			{
				return _localizedResources.ContainsKey(languageName);
			}
			return false;
		}
		public abstract object GetResourceValue(string languageName);

		public Dictionary<string, string>.Enumerator GetResourcesEnumerator()
		{
			if (_localizedResources != null)
			{
				return _localizedResources.GetEnumerator();
			}
			return default(Dictionary<string, string>.Enumerator);
		}
		public override string ToString()
		{
			return Name;
		}
		#endregion

		#region Properties
		internal abstract TreeNodeResource CreateTreeNode();
		public virtual bool UsePictureBox { get { return false; } }
		public abstract int TreeNodeIconIndex { get; }
		public abstract bool IsFile { get; }
		public abstract string WebFolderName { get; }
		public abstract string XmlTag { get; }
		public ProjectResources Manager
		{
			get
			{
				return _owner;
			}
		}
		[Browsable(false)]
		public XmlNode XmlData
		{
			get
			{
				if (_xmlNode == null)
				{
					_xmlNode = _owner.GetResourceXmlNode(this);
				}
				return _xmlNode;
			}
		}
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Description("Name of the resource")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public abstract string ValueDisplay
		{
			get;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsChanged { get; set; }
		public string CurrentResourceString
		{
			get
			{
				return GetResourceString(_owner.DesignerLanguageName);
			}
		}
		public object CurrentResourceValue
		{
			get
			{
				return GetResourceValue(_owner.DesignerLanguageName);
			}
		}
		#endregion

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get { return null; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = (ProjectResources)value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		public abstract Type ObjectType
		{
			get;
			set;
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public abstract object ObjectInstance
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get;
			set;
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get { return Name; }
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return ObjectKey; }
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}({1})", Name, ObjectType.Name);
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}:{1}", Name, ValueDisplay);
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get { return Name; }
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		[Browsable(false)]
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.All || target == EnumObjectSelectType.Object);
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				   "res{0}", MemberId.ToString("x", System.Globalization.CultureInfo.InvariantCulture));
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get { return ObjectType.FullName; }
		}
		[Browsable(false)]
		public virtual bool IsValid
		{
			get { return true; }
		}
		[Browsable(false)]
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodePropertyReferenceExpression(
				new CodeTypeReferenceExpression(_owner.ClassName),
				CodeName);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _owner.ClassName, CodeName);
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}->{1}", _owner.ClassName, CodeName);
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members
		[Browsable(false)]
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			ResourcePointer rp = objectIdentity as ResourcePointer;
			if (rp != null)
			{
				return (rp.MemberId == this.MemberId);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			ResourcePointer rp = p as ResourcePointer;
			if (rp != null)
			{
				return (rp.MemberId == this.MemberId);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get { return true; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Unknown; }
		}

		#endregion

		#region ICloneable Members
		[Browsable(false)]
		public virtual object Clone()
		{
			ResourcePointer obj = (ResourcePointer)(Activator.CreateInstance(this.GetType()));
			obj._id = MemberId;
			obj._name = _name;
			obj._xmlNode = _xmlNode;
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members
		//not used
		[Browsable(false)]
		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			_xmlNode = objectNode;
		}

		#endregion

		#region ISerializationProcessor Members
		[Browsable(false)]
		public void OnDeserialization(XmlNode objectNode)
		{
			_xmlNode = objectNode;
			if (_id == 0)
			{
				XmlNode idNode = _xmlNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				XmlUtil.SetNameAttribute(idNode, "MemberId");
				_xmlNode.AppendChild(idNode);
				idNode.InnerText = MemberId.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			XmlNodeList nl = _xmlNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XML_VALUES, XmlTags.XML_Item));
			foreach (XmlNode nd in nl)
			{
				SetResourceString(XmlUtil.GetNameAttribute(nd), nd.InnerText);
			}
		}

		#endregion
	}
}
