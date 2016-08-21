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
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using LimnorDesigner.MethodBuilder;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// a data type for IEnumerable/ICollection variable.
	/// it can only be used in a ClassPointer or MethodClass.
	/// represent a type implementing IEnumerable/ICollection
	/// the type represents the item type .
	///   //to find out item types:
	///    //Type[] tps = typeOfCollection.GetGenericArguments();
	///    //each tps is an item type
	/// </summary>
	public class CollectionTypePointer : DataTypePointer
	{
		#region fields and constructors
		private Type _collectionType;
		public CollectionTypePointer()
		{
		}
		public CollectionTypePointer(TypePointer type)
			: base(type)
		{
		}
		public CollectionTypePointer(TypePointer type, Type collectionType)
			: base(type)
		{
			_collectionType = collectionType;
		}
		public CollectionTypePointer(ClassPointer component)
			: base(component)
		{
		}
		#endregion
		#region Properties
		[Description("Collection type")]
		public Type CollectionType
		{
			get
			{
				return _collectionType;
			}
			set
			{
				_collectionType = value;
			}
		}
		[Browsable(false)]
		public override string BaseName
		{
			get
			{
				return "Collection";
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
				return Type.GetType(TypeString);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override string TypeString
		{
			get
			{
				if (_collectionType != null)
				{
					return _collectionType.AssemblyQualifiedName;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public override string DataTypeName
		{
			get
			{
				return "Collection of " + base.DataTypeName;
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
		public override LocalVariable CreateVariable(string name, UInt32 classId, UInt32 memberId)
		{
			if (memberId == 0)
				memberId = (UInt32)Guid.NewGuid().GetHashCode();
			LocalVariable v = new CollectionVariable(this, name, classId, memberId);
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
		#region IXmlNodeSerialization Members
		const string XML_CollectionType = "CollectionType";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlNode collectionTypeNode = XmlUtil.CreateSingleNewElement(node, XML_CollectionType);
			if (_collectionType != null)
			{
				XmlUtil.SetLibTypeAttribute(collectionTypeNode, _collectionType);
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			XmlNode collectionTypeNode = node.SelectSingleNode(XML_CollectionType);
			if (collectionTypeNode != null)
			{
				_collectionType = XmlUtil.GetLibTypeAttribute(collectionTypeNode);
			}
			base.OnReadFromXmlNode(reader, node);
		}

		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			CollectionTypePointer ap = (CollectionTypePointer)base.Clone();
			ap.CollectionType = _collectionType;
			return ap;
		}

		#endregion
	}
}
