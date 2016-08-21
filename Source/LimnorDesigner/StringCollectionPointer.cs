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
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// a data type for StringCollection variable.
	/// </summary>
	public class StringCollectionPointer : WrapDataTypePointer
	{
		#region fields and constructors
		public StringCollectionPointer()
			: base(new TypePointer(typeof(StringCollection)))
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override string BaseName
		{
			get
			{
				return "StringCollection";
			}
		}
		[Browsable(false)]
		public override TypePointer LibTypePointer
		{
			get
			{
				return new TypePointer(typeof(StringCollection));
			}
		}
		[Browsable(false)]
		public override Type BaseClassType
		{
			get
			{
				return typeof(StringCollection);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override string TypeString
		{
			get
			{
				return typeof(StringCollection).AssemblyQualifiedName;
			}
		}
		[Browsable(false)]
		public override string DataTypeName
		{
			get
			{
				return typeof(StringCollection).Name;
			}
		}
		[Browsable(false)]
		public override Image ImageIcon
		{
			get
			{
				return Resources.sc;
			}
		}
		#endregion
		#region Methods
		public override LocalVariable CreateVariable(string name, UInt32 classId, UInt32 memberId)
		{
			if (memberId == 0)
				memberId = (UInt32)Guid.NewGuid().GetHashCode();
			LocalVariable v = new StringCollectionVariable(this, name, classId, memberId);
			v.Owner = Owner;
			return v;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeVariableReferenceExpression(this.CodeName);
		}
		#endregion
	}
}
