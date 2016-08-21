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
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using System.Drawing;
using System.CodeDom;
using MathExp;
using XmlUtility;
using XmlSerializer;
using System.Xml;
using System.Globalization;

namespace LimnorDesigner
{
	/// <summary>
	/// a data type for List<T> variable.
	/// it can only be used in a ClassPointer or MethodClass.
	/// represent typeof(List<T>)
	/// the type represents the item type T
	/// </summary>
	public class ListTypePointer : WrapDataTypePointer
	{
		#region fields and constructors
		public ListTypePointer()
		{
		}
		public ListTypePointer(TypePointer type)
			: base(type)
		{
		}
		public ListTypePointer(ClassPointer component)
			: base(component)
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override string BaseName
		{
			get
			{
				return "List";
			}
		}
		[Browsable(false)]
		public override TypePointer LibTypePointer
		{
			get
			{
				return new TypePointer(BaseClassType);
			}
		}
		[Browsable(false)]
		public Type ItemBaseType
		{
			get
			{
				return base.BaseClassType;
			}
		}
		[Browsable(false)]
		public string ItemBaseTypeString
		{
			get
			{
				return base.TypeString;
			}
		}
		[Browsable(false)]
		public string ItemBaseTypeName
		{
			get
			{
				return base.TypeName;
			}
		}
		[Browsable(false)]
		public override Type BaseClassType
		{
			get
			{
				Type tb = typeof(List<>);
				return tb.MakeGenericType(ItemBaseType);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override string TypeString
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "List<{0}>", ItemBaseType.FullName);
			}
		}
		[Browsable(false)]
		public override string DataTypeName
		{
			get
			{
				return "List of " + base.DataTypeName;
			}
		}
		[Browsable(false)]
		public override Image ImageIcon
		{
			get
			{
				return Resources.list;
			}
		}
		#endregion
		#region Methods
		public override bool ContainsGenericParameters()
		{
			return false;
		}
		public override LocalVariable CreateVariable(string name, UInt32 classId, UInt32 memberId)
		{
			if (memberId == 0)
				memberId = (UInt32)Guid.NewGuid().GetHashCode();
			LocalVariable v = new ListVariable(this, name, classId, memberId);
			v.Owner = Owner;
			return v;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeVariableReferenceExpression(this.CodeName);
		}
		public CodeExpression CreateListCreationCode()
		{
			return new CodeObjectCreateExpression(TypeString, new CodeExpression[] { });
		}
		#endregion
	}
}
