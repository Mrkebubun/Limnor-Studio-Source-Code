/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using XmlSerializer;
using System.Xml;
using ProgElements;
using System.CodeDom;
using MathExp;
using System.ComponentModel;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.ResourcesManager
{
	/// <summary>
	/// to be used in programming
	/// </summary>
	public class ResourceCodePointer : IObjectPointer
	{
		private UInt32 _id;
		private ResourcePointer _resource;
		public ResourceCodePointer()
		{
		}
		public ResourceCodePointer(ResourcePointer pointer)
		{
			_resource = pointer;
			_id = pointer.MemberId;
		}
		public ResourcePointer Resource
		{
			get
			{
				return _resource;
			}
		}
		public UInt32 MemberId
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
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
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_resource != null)
				{
					return _resource.ObjectType;
				}
				return typeof(object);
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				return _resource;
			}
			set
			{
			}
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
			get
			{
				if (_resource != null)
				{
					return _resource.ReferenceName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_resource != null)
				{
					return _resource.CodeName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (_resource != null)
				{
					return _resource.DisplayName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				if (_resource != null)
				{
					return _resource.LongDisplayName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				if (_resource != null)
				{
					return _resource.ExpressionDisplay;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		[Browsable(false)]
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (_resource != null)
			{
				return _resource.IsTargeted(target);
			}
			return false;
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
			get
			{
				if (_resource != null)
				{
					return _resource.TypeString;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_resource != null)
				{
					return _resource.IsValid;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_resource is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_resource != null)
			{
				return _resource.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_resource != null)
			{
				return _resource.GetJavaScriptReferenceCode(code);
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_resource != null)
			{
				return _resource.GetPhpScriptReferenceCode(code);
			}
			return null;
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
			ResourceCodePointer o = objectIdentity as ResourceCodePointer;
			if (o != null)
			{
				return o.MemberId == _id;
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectIdentity = p as IObjectIdentity;
			if (objectIdentity != null)
			{
				ResourceCodePointer o = objectIdentity as ResourceCodePointer;
				if (o != null)
				{
					return o.MemberId == _id;
				}
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
		public object Clone()
		{
			ResourceCodePointer obj = new ResourceCodePointer();
			obj._id = _id;
			obj._resource = _resource;
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members
		[Browsable(false)]
		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (_resource == null)
			{
				ProjectResources rh = objMap.Project.GetProjectSingleData<ProjectResources>();
				_resource = rh.GetResourcePointerById(_id);
			}
		}

		#endregion
	}
}
