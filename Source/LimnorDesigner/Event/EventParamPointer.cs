/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using ProgElements;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.Event
{

	public interface IEventParameter
	{
		bool IsSameParameter(ParameterClass p);
	}

	/// <summary>
	/// its Owner is an EventPointer 
	/// </summary>
	[Serializable]
	public class EventParamPointer : MemberPointer, IEventParameter
	{
		#region fields and constructors
		public EventParamPointer()
		{
		}
		#endregion
		#region properties
		public ParameterInfo[] Parameters
		{
			get
			{
				return ((EventPointer)Owner).Parameters;
			}
		}
		[ReadOnly(true)]
		[Description("Event arguments. Their values are generated when the event occurs at runtime.")]
		public object[] ParameterValues
		{
			get
			{
				return ((EventPointer)Owner).ParameterValues;
			}
			set
			{
				((EventPointer)Owner).ParameterValues = value;
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
		protected override void OnCopy(MemberPointer obj)
		{
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
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MemberName is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
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
		public override bool IsSameObjectRef(IObjectIdentity obj)
		{
			EventParamPointer ep = obj as EventParamPointer;
			if (ep != null)
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
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				ParameterInfo[] pifs = Parameters;
				if (pifs != null)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						if (pifs[i].Name == MemberName)
						{
							return pifs[i].ParameterType;
						}
					}
				}
				return typeof(object);
			}
			set
			{

			}
		}
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				ParameterInfo[] pifs = Parameters;
				object[] vs = this.ParameterValues;
				if (pifs != null && vs != null)
				{
					if (pifs.Length == vs.Length)
					{
						for (int i = 0; i < pifs.Length; i++)
						{
							if (pifs[i].Name == MemberName)
							{
								return vs[i];
							}
						}
					}
				}
				return null;
			}
			set
			{
				ParameterInfo[] pifs = Parameters;
				object[] vs = this.ParameterValues;
				if (pifs != null && vs != null)
				{
					if (pifs.Length == vs.Length)
					{
						for (int i = 0; i < pifs.Length; i++)
						{
							if (pifs[i].Name == MemberName)
							{
								vs[i] = value;
								break;
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Event; } }
		#endregion
		#region IEventParameter Members
		/// <summary>
		/// use name and type to match an event argument to this event parameter
		/// </summary>
		/// <param name="p">the value representing an event argument</param>
		/// <returns></returns>
		public bool IsSameParameter(ParameterClass p)
		{
			if (p.Name == this.MemberName)
			{
				if (this.ObjectType.Equals(p.ObjectType))
				{
					return true;
				}
			}
			return false;
		}

		#endregion
	}

}
