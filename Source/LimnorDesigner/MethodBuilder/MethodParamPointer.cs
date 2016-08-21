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
using System.CodeDom;
using ProgElements;
using System.Reflection;
using MathExp;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// Owner is a MethodPointer;
	/// MemberName is the parameter name
	/// </summary>
	public class MethodParamPointer : MemberPointer
	{
		#region fields and constructors
		public MethodParamPointer()
		{
		}
		#endregion
		#region methods
		protected override void OnCopy(MemberPointer obj)
		{
		}
		public override bool IsSameObjectRef(IObjectIdentity obj)
		{
			MethodParamPointer mpp = obj as MethodParamPointer;
			if (mpp != null)
			{
				if (base.IsSameObjectRef(obj))
				{
					return true;
				}
			}
			return false;
		}
		public override bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
		}
		#endregion
		#region properties
		public ParameterInfo[] Info
		{
			get
			{
				return ((MethodPointer)Owner).Info;
			}
		}
		[ReadOnly(true)]
		public Type[] ParameterTypes
		{
			get
			{
				return ((MethodPointer)Owner).ParameterTypes;
			}
			set
			{
				((MethodPointer)Owner).ParameterTypes = value;
			}
		}

		[ReadOnly(true)]
		public override object ObjectInstance
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
		public override Type ObjectType
		{
			get
			{
				ParameterInfo[] pifs = Info;
				if (pifs != null)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						if (pifs[i].Name == MemberName)
						{
							return pifs[i].ParameterType; ;
						}
					}
				}
				return typeof(void);
			}
			set
			{

			}
		}
		#endregion
		#region IObjectPointer Members
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				return ObjectType.AssemblyQualifiedName;
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(MemberName))
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MemberName is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeArgumentReferenceExpression(this.MemberName);
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return this.MemberName;
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			return this.MemberName;
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Method; } }
		#endregion
	}
}
