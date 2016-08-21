/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using ProgElements;
using XmlSerializer;
using System.Xml;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;

namespace LimnorDesigner
{
	/// <summary>
	/// represent "this"
	/// </summary>
	public class ThisPointer : IObjectPointer
	{
		IObjectPointer _owner;
		public ThisPointer()
		{
		}
		public override string ToString()
		{
			return "ThisObject";
		}
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
				_owner = value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectType;
				}
				return null;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectDebug
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string PropertyName
		{
			get { return "ThisObject"; }
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get { return "ThisObject"; }
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return "ThisObject"; }
		}
		[Browsable(false)]
		public string DisplayName
		{
			get { return "ThisObject"; }
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get { return "this"; }
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return target == EnumObjectSelectType.All || target == EnumObjectSelectType.Object;
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return "{ECD3B9CF-98DF-4c1c-BEBF-08F20BD0FF4B}"; }
		}
		[Browsable(false)]
		public string TypeString
		{
			get { return "this"; }
		}
		[Browsable(false)]
		public bool IsValid
		{
			get { return true; }
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeThisReferenceExpression();
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return "this";
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return "$this";
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

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			return objectIdentity is ThisPointer;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			return p is ThisPointer;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get { return false; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Both; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Unknown; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return this;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion
	}
}
