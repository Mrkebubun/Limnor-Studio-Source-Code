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
using VPL;
using System.IO;

namespace LimnorDesigner
{
	/// <summary>
	/// a data type for array variable.
	/// it can only be used in a ClassPointer or MethodClass.
	/// represent typeof(T[])
	/// the type represents the item type
	/// </summary>
	public class ArrayPointer : WrapDataTypePointer
	{
		#region fields and constructors
		const string XML_Rank = "Rank";
		const string XMLATT_Dim = "size";
		private int[] _sizes; //sizes for all dimensions
		public ArrayPointer()
		{
		}
		public ArrayPointer(TypePointer type)
			: base(type)
		{
		}
		public ArrayPointer(ClassPointer component)
			: base(component)
		{
		}
		#endregion
		#region Properties
		[Description("Array rank. A rank 1 array uses one index to get and set array items; a rank 2 array uses two indices to get and set array items; and so on")]
		public int Rank
		{
			get
			{
				if (_sizes == null)
					_sizes = new int[1];
				return _sizes.Length;
			}
			set
			{
				if (value > 0)
				{
					if (_sizes == null)
						_sizes = new int[value];
					else
					{
						int[] a = new int[value];
						int n = _sizes.Length;
						if (n <= value)
						{
							_sizes.CopyTo(a, 0);
							for (int i = n; i < value; i++)
							{
								a[i] = 0;
							}
						}
						else
						{
							for (int i = 0; i < value; i++)
							{
								a[i] = _sizes[i];
							}
						}
						_sizes = a;
					}
				}
			}
		}
		[Browsable(false)]
		public int[] Dimnesions
		{
			get
			{
				return _sizes;
			}
			set
			{
				_sizes = value;
			}
		}
		[Browsable(false)]
		public override string BaseName
		{
			get
			{
				return "Array";
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
				Type tb = base.BaseClassType;
				if (tb != null)
				{
					string s = tb.AssemblyQualifiedName;
					string r;
					if (Rank > 1)
					{
						r = "[" + new string(',', Rank - 1) + "]";
					}
					else
					{
						r = "[]";
					}
					int n = s.IndexOf(',');
					if (n > 0)
					{
						s = s.Substring(0, n) + r + s.Substring(n);
					}
					else
					{
						s = s + r;
					}
					VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(tb.Assembly.Location));
					try
					{
						return Type.GetType(s);
					}
					catch
					{
						throw;
					}
					finally
					{
						VPLUtil.RemoveExternalDllResolve();
					}
				}
				return null;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override string TypeString
		{
			get
			{
				string r;
				if (Rank > 1)
				{
					r = "[" + new string(',', Rank - 1) + "]";
				}
				else
				{
					r = "[]";
				}
				string s = base.TypeString;
				int n = s.IndexOf(',');
				if (n > 0)
				{
					return s.Substring(0, n) + r + s.Substring(n);
				}
				return s + r;
			}
		}
		[Browsable(false)]
		public override string DataTypeName
		{
			get
			{
				return "Array of " + base.DataTypeName;
			}
		}
		[Browsable(false)]
		public override Image ImageIcon
		{
			get
			{
				return Resources.array;
			}
		}
		#endregion
		#region Methods
		public override LocalVariable CreateVariable(string name, UInt32 classId, UInt32 memberId)
		{
			if (memberId == 0)
				memberId = (UInt32)Guid.NewGuid().GetHashCode();
			LocalVariable v = new ArrayVariable(this, name, classId, memberId);
			v.Owner = Owner;
			return v;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeVariableReferenceExpression(this.CodeName);
		}
		public CodeExpression CreateArrayCreationCode()
		{
			return CompilerUtil.CreateArrayCreationCode(Dimnesions, ItemBaseTypeString, ItemBaseTypeName);
		}
		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlNode ndDim = node.SelectSingleNode(XML_Rank);
			if (ndDim == null)
			{
				ndDim = node.OwnerDocument.CreateElement(XML_Rank);
				node.AppendChild(ndDim);
			}
			else
			{
				ndDim.RemoveAll();
			}
			if (_sizes == null || _sizes.Length == 0)
			{
				_sizes = new int[1];
				_sizes[0] = 0;
			}
			for (int i = 0; i < _sizes.Length; i++)
			{
				XmlNode nd = ndDim.OwnerDocument.CreateElement(XmlTags.XML_Item);
				ndDim.AppendChild(nd);
				XmlUtil.SetAttribute(nd, XMLATT_Dim, _sizes[i]);
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			XmlNodeList ndList = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}", XML_Rank, XmlTags.XML_Item));
			if (ndList == null)
			{
				_sizes = new int[1];
				_sizes[0] = 0;
			}
			else
			{
				_sizes = new int[ndList.Count];
				for (int i = 0; i < ndList.Count; i++)
				{
					_sizes[i] = XmlUtil.GetAttributeInt(ndList[i], XMLATT_Dim);
				}
			}
		}

		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			ArrayPointer ap = (ArrayPointer)base.Clone();
			int[] sz = new int[Rank];
			_sizes.CopyTo(sz, 0);
			ap.Dimnesions = sz;
			return ap;
		}

		#endregion
	}
}
