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
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.CodeDom;
using MathExp;
using ProgElements;
using System.Drawing;
using LimnorDesigner.MenuUtil;
using System.Reflection;
using VPL;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using LimnorDesigner.Web;
using System.Globalization;

namespace LimnorDesigner
{
	/// <summary>
	/// pointer to a Type for accessing static methods, properties and events
	/// </summary>
	[SaveAsProperties]
	[Serializable]
	public class TypePointer : IClass
	{
		#region fields and constructors
		private IObjectPointer _owner;
		private ClassPointer _root;
		private Type _type;
		public TypePointer()
		{
		}
		public TypePointer(Type t)
		{
			_type = t;
		}
		public TypePointer(Type t, IObjectPointer owner)
			: this(t)
		{
			_owner = owner;
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			if (_type == null)
				return "?";
			return VPLUtil.GetTypeDisplay(_type);
		}
		public object TryCreateInstance()
		{
			if (_type != null)
			{
				object v = VPL.VPLUtil.TryCreateInstance(_type);
				return v;
			}
			return null;
		}
		public bool IsJsType()
		{
			if (_type != null)
			{
				return _type.GetInterface("IJavascriptType") != null;
			}
			return false;
		}
		public bool IsPhpType()
		{
			if (_type != null)
			{
				return _type.GetInterface("IPhpType") != null;
			}
			return false;
		}
		#endregion

		#region Properties
		[Browsable(false)]
		[Description("A ClassType is a template for creating class instances (objects). A ClassType may contain methods, properties or events which belong to the ClassType and do not belong to any instances")]
		public Type ClassType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (JsTypeAttribute.IsJsType(this.ClassType))
				{
					return EnumWebRunAt.Client;
				}
				if (PhpTypeAttribute.IsPhpType(this.ClassType))
				{
					return EnumWebRunAt.Server;
				}
				if (this.ClassType != null)
				{
					if (this.ClassType.GetInterface("IWebClientComponent") != null)
					{
						return EnumWebRunAt.Client;
					}
					if (WebClientClassAttribute.IsClientType(this.ClassType))
					{
						return EnumWebRunAt.Client;
					}
					if (WebClientMemberAttribute.IsClientType(this.ClassType))
					{
						return EnumWebRunAt.Client;
					}
					if (typeof(HtmlElement_Base).IsAssignableFrom(this.ClassType))
					{
						return EnumWebRunAt.Client;
					}
					if (typeof(HtmlElement_document).IsAssignableFrom(this.ClassType))
					{
						return EnumWebRunAt.Client;
					}
				}
				return EnumWebRunAt.Server;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return Name;
			}
		}
		/// <summary>
		/// fully qualified variable name
		/// </summary>
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return Name;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string PropertyName
		{
			get
			{
				return ReferenceName;
			}
		}
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				if (_root != null)
				{
					return _root;
				}
				IObjectPointer root = this.Owner;
				if (root != null)
				{
					return root.RootPointer;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IClass Host
		{
			get
			{
				return RootPointer;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return Owner;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
                if (_type == null) _type = typeof(object);
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				return _type.AssemblyQualifiedName;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				return _type;
			}
			set
			{
				if (value is Type)
				{
					_type = (Type)value;
				}
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_type != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_type is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		public bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			TypePointer tp = objectPointer as TypePointer;
			if (tp == null)
			{
				DataTypePointer dp = objectPointer as DataTypePointer;
				if (dp != null)
				{
					tp = dp.LibTypePointer;
				}
			}
			if (tp != null)
			{
				Type t1 = this.ObjectInstance as Type;
				Type t2 = tp.ObjectInstance as Type;
				if (t1 != null && t2 != null)
				{
					return t1.Equals(t2);
				}
			}

			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				Type t1 = this.ObjectInstance as Type;
				if (t1 != null)
				{
					return VPLUtil.GetTypeDisplay(t1);
				}
				return "?";
			}
		}
		[Browsable(false)]
		public virtual string LongDisplayName
		{
			get
			{
				Type t1 = this.ObjectInstance as Type;
				if (t1 != null)
				{
					return t1.FullName;
				}
				return "?";
			}
		}
		[Browsable(false)]
		public virtual string ExpressionDisplay
		{
			get
			{
				Type t1 = this.ObjectInstance as Type;
				if (t1 != null)
				{
					return t1.Name;
				}
				return "?";
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return true;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return ObjectType.AssemblyQualifiedName;
			}
		}
		//return default non-null value
		public string CreateJavaScript(StringCollection sb)
		{
			if (typeof(string).Equals(ClassType))
			{
				return "''";
			}
			if (GlobalFunctionAttribute.IsGlobalFunctionType(ClassType))
			{
				return string.Empty;
			}
			if (ClassType.GetInterface("IJavascriptType") != null)
			{
				IJavascriptType js = Activator.CreateInstance(ClassType) as IJavascriptType;
				return js.CreateDefaultObject();
			}
			return ClassType.Name;
		}
		public string CreatePhpScript(StringCollection sb)
		{
			if (typeof(string).Equals(ClassType))
			{
				return "''";
			}
			if (GlobalFunctionAttribute.IsGlobalFunctionType(ClassType))
			{
				return string.Empty;
			}
			if (typeof(DateTime).Equals(ClassType) || typeof(JsDateTime).Equals(ClassType))
			{
				return "date(\"Y-m-d H:i:s\")";
			}
			return ClassType.Name;
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (forValue)
			{
				return new CodeTypeOfExpression(ClassType);
			}
			return new CodeTypeReferenceExpression(ClassType);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return ClassType.Name;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return ClassType.Name;
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Class; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			TypePointer tp = new TypePointer();
			tp.ObjectInstance = this.ObjectInstance;
			tp.Owner = this.Owner;
			return tp;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				_root = objMap.RootPointer as ClassPointer;
				if (_owner == null)
				{
					_owner = objMap.RootPointer as IObjectPointer;
				}
			}
		}

		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		public ulong WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(MemberId, ClassId);
			}
		}
		[Browsable(false)]
		public UInt32 DefinitionClassId
		{
			get
			{
				return ClassId;
			}
		}
		[Browsable(false)]
		public uint ClassId
		{
			get
			{
				if (_type != null)
				{
					return (uint)_type.GUID.GetHashCode();
				}
				return 0;
			}
		}
		[Browsable(false)]
		public uint MemberId
		{
			get
			{
				return 1;
			}
		}
		[Browsable(false)]
		public string Name
		{
			get { return ToString(); }
		}

		#endregion

		#region IClass Members
		[ReadOnly(true)]
		[Browsable(false)]
		public Image ImageIcon
		{
			get
			{
				if (_type != null)
				{
					return TreeViewObjectExplorer.GetTypeImage(_type);
				}
				return null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public Type VariableLibType
		{
			get
			{
				return _type;
			}
		}
		[Browsable(false)]
		public ClassPointer VariableCustomType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public IClassWrapper VariableWrapperType
		{
			get
			{
				return null;
			}
		}

		#endregion
	}
	public class TypePointerCollection : List<TypePointer>, ICustomSerialization
	{
		const string XML_TypePointer = "Type";
		private XmlNode _xmlNode;
		public TypePointerCollection()
		{
		}

		#region ICustomSerialization Members
		object _serializer;
		public void SetSerializer(object serializer)
		{
			_serializer = serializer;
		}
		[Browsable(false)]
		public object Serializer
		{
			get
			{
				return _serializer;
			}
		}
		[Browsable(false)]
		public XmlNode CachedXmlNode
		{
			get
			{
				return _xmlNode;
			}
		}
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_xmlNode = node;
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
			foreach (TypePointer t in this)
			{
				XmlNode n = node.OwnerDocument.CreateElement(XML_TypePointer);
				node.AppendChild(n);
				XmlUtil.SetLibTypeAttribute(n, t.ClassType);
			}
		}

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_xmlNode = node;
			XmlNodeList nodes = node.SelectNodes(XML_TypePointer);
			foreach (XmlNode nd in nodes)
			{
				Type tp = XmlUtil.GetLibTypeAttribute(nd);
				TypePointer t = new TypePointer(tp);
				this.Add(t);
			}
		}

		#endregion
	}
}
