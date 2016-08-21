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
using System.Reflection;
using ProgElements;
using System.ComponentModel;
using MathExp;
using System.CodeDom;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner
{
	/// <summary>
	/// represents an assembly
	/// </summary>
	public class AssemblyPointer : IObjectPointer
	{
		private IObjectPointer _owner;
		private Assembly _assembly;
		private string _name;
		public AssemblyPointer(string assemblyName)
		{
			_name = assemblyName;
		}

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Server;
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
				IObjectPointer root = this.Owner;
				while (root != null && root.Owner != null)
				{
					root = root.Owner;
				}
				ClassPointer c = root as ClassPointer;
				return c;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		public string CodeName
		{
			get
			{
				return _name;
			}
		}
		/// <summary>
		/// fully qualified variable name
		/// </summary>
		public string ReferenceName
		{
			get
			{
				return _name;
			}
		}
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
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		public Type ObjectType
		{
			get
			{
				return typeof(Assembly);
			}
			set
			{

			}
		}

		public object ObjectInstance
		{
			get
			{
				if (_assembly == null)
				{
					_assembly = Assembly.Load(_name);
				}

				return _assembly;
			}
			set
			{
				_assembly = (Assembly)value;
			}
		}

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

		public string DisplayName
		{
			get { return _name; }
		}
		public string LongDisplayName
		{
			get { return _name; }
		}
		public string ExpressionDisplay
		{
			get { return _name; }
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return false;
		}

		public string ObjectKey
		{
			get { return _name; }
		}

		public string TypeString
		{
			get { return typeof(Assembly).AssemblyQualifiedName; }
		}

		public bool IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(_name))
					return true;
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_name null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public System.CodeDom.CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
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

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			AssemblyPointer ap = objectIdentity as AssemblyPointer;
			if (ap != null)
				return string.Compare(ap._name, _name, StringComparison.OrdinalIgnoreCase) == 0;
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			AssemblyPointer ap = p as AssemblyPointer;
			if (ap != null)
				return string.Compare(ap._name, _name, StringComparison.OrdinalIgnoreCase) == 0;
			return false;
		}
		public IObjectIdentity IdentityOwner
		{
			get { return _owner; }
		}

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

		public object Clone()
		{
			AssemblyPointer ap = new AssemblyPointer(_name);
			ap.Owner = _owner;
			return ap;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion
	}
}
