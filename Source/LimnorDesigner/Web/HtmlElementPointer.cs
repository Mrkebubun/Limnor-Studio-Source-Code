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
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;
using ProgElements;
using XmlSerializer;
using System.Xml;
using System.Drawing;
using LimnorDesigner.MenuUtil;
using System.Globalization;

namespace LimnorDesigner.Web
{
	[UseParentObjectAttribute]
	public class HtmlElementPointer : IObjectPointer, IClass
	{
		#region fields and constructors
		private HtmlElement_BodyBase _element;
		private ClassPointer _root;
		public HtmlElementPointer(HtmlElement_BodyBase element)
		{
			_element = element;
		}
		public HtmlElementPointer(ClassPointer root)
		{
			_root = root;
		}
		#endregion
		#region Properties
		public HtmlElement_BodyBase Element
		{
			get
			{
				return _element;
			}
		}
		#endregion

		#region IObjectPointer Members

		public ClassPointer RootPointer
		{
			get
			{
				if (_element == null)
					return null;
				return _element.RootPointer;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				if (_element == null)
					return null;
				return _element.RootPointer;
			}
			set
			{

			}
		}
		private object _debug;
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public object ObjectDebug
		{
			get
			{
				return _debug;
			}
			set
			{
				_debug = value;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string ReferenceName
		{
			get
			{
				if (_element == null)
					return null;
				HtmlElement_ItemBase hei = _element as HtmlElement_ItemBase;
				if (hei != null)
					return hei.id;
				return _element.tagName;
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

		public string DisplayName
		{
			get
			{
				if (_element == null)
					return null;
				return _element.ToString();
			}
		}

		public string LongDisplayName
		{
			get { return DisplayName; }
		}

		public string ExpressionDisplay
		{
			get
			{
				return ReferenceName;
			}
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object);
		}

		public string ObjectKey
		{
			get
			{
				return ReferenceName;
			}
		}

		public string TypeString
		{
			get { return string.Empty; }
		}

		public bool IsValid
		{
			get
			{
				if (_element == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_element is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				return _element.IsValid;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			throw new NotImplementedException();
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (_element != null)
				_element.CreateActionJavaScript(methodName, code, parameters, returnReceiver);
		}

		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_element == null)
				return null;
			return _element.GetJavaScriptReferenceCode(code);
		}

		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			throw new NotImplementedException();
		}

		public EnumWebRunAt RunAt
		{
			get { return EnumWebRunAt.Client; }
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (_element == null)
				return false;
			return _element.IsSameObjectRef(objectIdentity);
		}

		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_element == null)
					return null;
				return _element.IdentityOwner;
			}
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Class; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			if (_element == null)
				return new HtmlElementPointer(_root);
			return new HtmlElementPointer(_element.Clone() as HtmlElement_BodyBase);
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion

		#region IPropertyPointer Members

		public string CodeName
		{
			get
			{
				return ReferenceName;
			}
		}

		public Type ObjectType
		{
			get
			{
				if (_element == null)
					return null;
				return _element.GetType();
			}
			set
			{
				if (_element == null)
				{
					_element = Activator.CreateInstance(value, _root) as HtmlElement_BodyBase;
				}
			}
		}

		public object ObjectInstance
		{
			get
			{
				return _element;
			}
			set
			{
				_element = value as HtmlElement_BodyBase;
				if (_element != null)
				{
					if (_root != null)
					{
						_element.SetPageOwner(_root);
					}
				}
			}
		}

		public IPropertyPointer PropertyOwner
		{
			get { return null; }
		}

		#endregion

		#region IClass Members
		[Browsable(false)]
		[ReadOnly(true)]
		public Image ImageIcon
		{
			get
			{
				if (_element != null)
				{
					return _element.ImageIcon;
				}
				return null;
			}
			set
			{
				if (_element != null)
				{
					_element.ImageIcon = value;
				}
			}
		}
		[Browsable(false)]
		public Type VariableLibType
		{
			get
			{
				if (_element != null)
				{
					return _element.VariableLibType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public ClassPointer VariableCustomType
		{
			get
			{
				if (_element != null)
				{
					return _element.VariableCustomType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IClassWrapper VariableWrapperType
		{
			get
			{
				if (_element != null)
				{
					return _element.VariableWrapperType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public uint DefinitionClassId
		{
			get
			{
				if (_element != null)
				{
					return _element.DefinitionClassId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public IClass Host
		{
			get
			{
				if (_element != null)
				{
					return _element.Host;
				}
				return null;
			}
		}

		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		public ulong WholeId
		{
			get
			{
				if (_element != null)
				{
					return _element.WholeId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public uint ClassId
		{
			get
			{
				if (_element != null)
				{
					return _element.ClassId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public uint MemberId
		{
			get
			{
				if (_element != null)
				{
					return _element.MemberId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public string Name
		{
			get
			{
				if (_element != null)
				{
					return _element.Name;
				}
				return null;
			}
		}

		#endregion
	}
}
