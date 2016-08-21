/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.ComponentModel;
using VPL;

namespace MathExp
{
	public class CodeValue
	{
		/// <summary>Generates a property setter or event setter statement</summary> 
		/// <param name="cx"><see cref="CodeExpression"/> for which property has to be set</param> 
		/// <param name="objType"><see cref="Type"/> of <see cref="object"/>, <paramref name="cx"/> is 
		/// representing</param> 
		/// <param name="x">Property Name</param> 
		/// <param name="y">Property Value</param> 
		private static CodeStatement setPropEvent(CodeExpression cx, Type objType, string x, string y)
		{
			CodeStatement as1 = null;
			MemberTypes result = ReflectionHelper.GetMember(x, objType);
			if (result == MemberTypes.Property)
			{
				// A Property Encountered. 
				CodePropertyReferenceExpression prop = new CodePropertyReferenceExpression(cx, x);
				Type t = ReflectionHelper.GetPropertyType(x, objType);
				if (t.IsPrimitive || t.Equals(typeof(System.String)))
				{
					// A Primitive or string Property is Encountered 
					object o = ReflectionHelper.ConvertFrom(y, t);
					if (t.Equals(typeof(IntPtr)))
					{
						IntPtr val = (IntPtr)o;
						if (IntPtr.Size == 4)
							as1 = new CodeAssignStatement(prop, new CodeObjectCreateExpression(typeof(IntPtr), new CodePrimitiveExpression(val.ToInt32())));
						else
							as1 = new CodeAssignStatement(prop, new CodeObjectCreateExpression(typeof(IntPtr), new CodePrimitiveExpression(val.ToInt64())));
					}
					else if (t.Equals(typeof(UIntPtr)))
					{
						UIntPtr val = (UIntPtr)o;
						if (IntPtr.Size == 4)
							as1 = new CodeAssignStatement(prop, new CodeObjectCreateExpression(typeof(UIntPtr), new CodePrimitiveExpression(val.ToUInt32())));
						else
							as1 = new CodeAssignStatement(prop, new CodeObjectCreateExpression(typeof(UIntPtr), new CodePrimitiveExpression(val.ToUInt64())));
					}
					else
						as1 = new CodeAssignStatement(prop, new CodePrimitiveExpression(o));
				}
				else if (t.IsEnum)
				{
					// An Enum Property Encountered 
					CodeTypeReferenceExpression xpType = new CodeTypeReferenceExpression(t);
					CodeFieldReferenceExpression xpField = new CodeFieldReferenceExpression(xpType, y);
					as1 = new CodeAssignStatement(prop, xpField);
				}
				else
				{
					InstanceDescriptor idc = ReflectionHelper.GetInstance(y, t);
					CodeExpression xpRite = null;
					if (idc.Arguments == null || idc.Arguments.Count <= 0)
						xpRite = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(t), idc.MemberInfo.Name);
					else
					{
						CodeExpression[] cpargs = new CodeExpression[idc.Arguments.Count];
						int i = 0;
						foreach (object o in idc.Arguments)
						{
							cpargs[i++] = ObjectCreationCodeGen.ObjectCreationCode(o);
						}
						xpRite = new CodeObjectCreateExpression(t, cpargs);
					}
					as1 = new CodeAssignStatement(prop, xpRite);
				}
			}
			else if (result == MemberTypes.Event)
			{
				Type typeEvent = ReflectionHelper.GetEventType(x, objType);
				CodeDelegateCreateExpression xpDele = new CodeDelegateCreateExpression(
				new CodeTypeReference(typeEvent), new CodeThisReferenceExpression(), y);
				as1 = new CodeAttachEventStatement(cx, x, xpDele);
			}
			return as1;
		}
		public class ReflectionHelper
		{

			/// <summary><see cref="System.Type"/> of 
			/// <see cref="System.ComponentModel.Design.Serialization.InstanceDescriptor"/></summary> 
			private static Type INSTANCE_DESCRIPTOR_TYPE = typeof(InstanceDescriptor);

			/// <summary>Hashtable of pre-created TypeConverters</summary> 
			static Hashtable hashConverters = null;

			/// <summary>Reflection Helper Static Constructor</summary> 
			static ReflectionHelper()
			{
				hashConverters = new Hashtable();
			}

			/// <summary>Prevent Instantiation</summary> 
			private ReflectionHelper()
			{
			}

			/// <summary>Convert from <paramref name="x"/> to <paramref name="type"/> 
			/// </summary> 
			/// <param name="x">String representation of the object</param> 
			/// <param name="type">Desired type</param> 
			/// <returns> 
			/// Null if no type converter is found or conversion unsuccessful 
			/// otherwise a converted object. 
			/// </returns> 
			public static object ConvertFrom(string x, Type type)
			{
				TypeConverter conv = (TypeConverter)hashConverters[type];
				if (conv == null)
				{
					// We need to create a new type converter 
					conv = TypeDescriptor.GetConverter(type);
					if (conv == null)
					{
						return null;
					}
					hashConverters[type] = conv;
				}
				return conv.ConvertFrom(x);
			}

			/// <summary>Convert from <paramref name="x" /> to the 
			/// <see cref="System.ComponentModel.Design.Serialization.InstanceDescriptor"/> 
			/// describing construction of <see cref="System.Type"/> type of object.</summary> 
			/// <param name="x">String representation of the object</param> 
			/// <param name="type">Desired type</param> 
			/// <returns> 
			/// Null if no type converter is found or conversion unsuccessful 
			/// otherwise an instance of <see cref="System.ComponentModel.Design.Serialization.InstanceDescriptor"/> 
			/// describing construction of converted object. 
			/// </returns> 
			public static InstanceDescriptor GetInstance(string x, Type type)
			{
				object o = ConvertFrom(x, type);
				if (o == null)
					return null;
				TypeConverter conv = (TypeConverter)hashConverters[type];
				return (conv.ConvertTo(o, INSTANCE_DESCRIPTOR_TYPE) as InstanceDescriptor);
			}

			/// <summary>Returns the <see cref="System.Reflection.MemberInfo.MemberType" /> of <paramref name="member"/> 
			/// of <paramref name="type"/>.</summary> 
			/// <param name="member">Member to query for</param> 
			/// <param name="type"><see cref="System.Type" /> to which <paramref name="member"/> 
			/// belongs to.</param> 
			/// <returns> 
			/// True, if a property (or field) encountered otherwise false. 
			/// </returns> 
			public static MemberTypes GetMember(string member, Type type)
			{
				MemberInfo[] info = type.GetMember(member);
				// All except events are properties, a lenient check. 
				// At least one member is returned. 
				if (info != null && info.Length > 0)
				{
					return info[0].MemberType;
				}
				else
				{
					throw new Exception("No such property[" + member + "] exception");
				}
			}

			/// <summary>Looks up <see cref="System.Type"/> of the <paramref name="property"/> 
			/// of <paramref name="clazz"/>.</summary> 
			/// <param name="property">Name of the property for which type lookup has to 
			/// be done.</param> 
			/// <param name="clazz">Type which contains <paramref name="property"/></param> 
			/// <returns>An instance of <see cref="System.Type"/> if found, or null 
			/// if not found.</returns> 
			public static Type GetPropertyType(string property, Type clazz)
			{
				PropertyInfo prop = clazz.GetProperty(property);
				return prop.PropertyType;
			}

			/// <summary>Looks up <see cref="System.Type"/> of the <paramref name="Event"/> 
			/// of <paramref name="clazz"/>.</summary> 
			/// <param name="Event">Name of the event for which type lookup has to 
			/// be done.</param> 
			/// <param name="clazz">Type which contains <paramref name="Event"/></param> 
			/// <returns>An instance of <see cref="System.Type"/> if found, or null 
			/// if not found.</returns> 
			public static Type GetEventType(string Event, Type clazz)
			{
				EventInfo prop = clazz.GetEvent(Event);
				return prop.EventHandlerType;
			}
		}
	}
}
