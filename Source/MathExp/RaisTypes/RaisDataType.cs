/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Xml;
using System.ComponentModel;
using System.Drawing.Design;
using System.CodeDom;
using System.Collections.Specialized;
using System.Globalization;
using System.Drawing;
using ProgElements;
using XmlUtility;
using VPL;

namespace MathExp.RaisTypes
{
	/// <summary>
	///   <xs:complexType name="DataType">
	///     <!-- defines data type for defining parameters -->
	///     <xs:sequence>
	///       <xs:choice minOccurs="0" maxOccurs="1">
	///         <xs:element name="Type" type="ObjectRef" />
	///         <!-- use a type from current project -->
	///         <xs:element name="LibType" type="xs:string" />
	///         <!-- use a type from a library -->
	///       </xs:choice>
	///     </xs:sequence>
	///     <xs:attribute name="name" type="xs:string" use="optional" />
	///     <xs:attribute name="ID" type="xs:int" use="optional" />
	///     <!-- parameter/property name -->
	///   </xs:complexType>
	/// </summary>
	public class RaisDataType : IXmlNodeSerializable, ICloneable, ICustomTypeDescriptor, IDesignServiceProvider
	{
		#region fields and constructors
		public const string PROP_NAME = "Name";
		//RAIS definition
		private ObjectRef _devType;
		private string _name;
		private Type _libType = typeof(void);
		//
		private string _desc;
		//RAIS VPE properties
		private bool _allowTypeChange;
		private IDesignServiceProvider _serviceProvider;
		public event EventHandler OnNameChanging;
		public event EventHandler OnNameChanged;
		public event EventHandler OnTypeChange;
		public event EventHandler OnDescriptionChanged;
		//
		public RaisDataType()
		{
		}
		public RaisDataType(Type t)
		{
			_libType = t;
		}
		public RaisDataType(Type t, string name)
		{
			_libType = t;
			_name = name;
		}
		#endregion
		#region methods
		public bool IsSameType(RaisDataType type)
		{
			if (type == null)
				return false;
			if (this.IsLibType)
			{
				if (_libType == null)
					return false;
				if (type.IsLibType)
				{
					return this.LibType.Equals(type.LibType);
				}
			}
			else
			{
				if (!type.IsLibType)
				{
					if (DevType == null)
						return false;
					return this.DevType.IsSameType(type.DevType);
				}
			}
			return false;
		}
		public XmlDocument GetXmlDocument()
		{
			if (_devType != null)
			{
				return _devType.GetXmlDocument();
			}
			return null;
		}
		public void SetServiceProvider(IDesignServiceProvider designServiceProvider)
		{
			_serviceProvider = designServiceProvider;
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0} ({1})", this.Name, this.TypeName);
		}
		protected virtual void OnBeforeNameChange(string oldName, string newName, ref bool cancel)
		{
			if (OnNameChanging != null)
			{
				EventArgName e = new EventArgName(this, newName);
				OnNameChanging(this, e);
				cancel = e.Cancel;
			}
		}
		protected virtual void OnAfterNameChange()
		{
			if (OnNameChanged != null)
			{
				OnNameChanged(this, new EventArgName(this, this.Name));
			}
		}
		public void SetName(string name)
		{
			_name = name;
		}
		#endregion
		#region properties
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
				if (OnDescriptionChanged != null)
				{
					OnDescriptionChanged(this, new EventArgs());
				}
			}
		}
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
		public RaisDataType DataType
		{
			get
			{
				RaisDataType dt = new RaisDataType();
				dt.DataType = this;
				return dt;
			}
			set
			{
				if (value == null)
				{
					throw new MathException("Data type cannot be null");
				}
				this.DevType = value.DevType;
				this.LibType = value.LibType;
				if (OnTypeChange != null)
				{
					OnTypeChange(this, null);
				}
			}
		}
		[Browsable(false)]
		public bool AllowTypeChange
		{
			get
			{
				return _allowTypeChange;
			}
			set
			{
				_allowTypeChange = value;
			}
		}
		[Browsable(false)]
		public bool IsVoid
		{
			get
			{
				if (_devType == null)
				{
					if (_libType.Equals(typeof(void)))
					{
						return true;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsLibType
		{
			get
			{
				return (_devType == null);
			}
		}
		[Browsable(false)]
		public ObjectRef DevType
		{
			get
			{
				return _devType;
			}
			set
			{
				_devType = value;
			}
		}
		[ReadOnly(false)]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				bool cancel = false;
				OnBeforeNameChange(_name, value, ref cancel);
				if (!cancel)
				{
					_name = value;
					OnAfterNameChange();
				}
			}
		}
		[Browsable(false)]
		public Type LibType
		{
			get
			{
				return _libType;
			}
			set
			{
				_libType = value;
			}
		}
		[Browsable(false)]
		public string TypeName
		{
			get
			{
				if (_devType != null)
				{
					return _devType.Name;
				}
				if (_libType != null)
				{
					return _libType.Name;
				}
				return "";
			}
		}
		[Browsable(false)]
		public Type Type
		{
			get
			{
				if (_devType != null)
				{
					return _devType.DataType;
				}
				if (_libType != null)
				{
					return _libType;
				}
				return typeof(void);
			}
		}
		[Browsable(false)]
		public bool IsBoolean
		{
			get
			{
				if (IsLibType)
				{
					return typeof(bool).Equals(this.Type);
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsString
		{
			get
			{
				if (IsLibType)
				{
					return typeof(string).Equals(this.Type);
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsNumber
		{
			get
			{
				if (IsLibType)
				{
					TypeCode tc = Type.GetTypeCode(this.Type);
					if (tc == TypeCode.Byte || tc == TypeCode.Decimal
						|| tc == TypeCode.Double || tc == TypeCode.Int16
						|| tc == TypeCode.Int32 || tc == TypeCode.Int64
						|| tc == TypeCode.SByte || tc == TypeCode.Single
						|| tc == TypeCode.UInt16 || tc == TypeCode.UInt32
						|| tc == TypeCode.UInt64)
						return true;
				}
				return false;
			}
		}
		[Browsable(false)]
		public bool IsInteger
		{
			get
			{
				if (IsLibType)
				{
					TypeCode tc = Type.GetTypeCode(this.Type);
					if (tc == TypeCode.Byte || tc == TypeCode.Int16
						|| tc == TypeCode.Int32 || tc == TypeCode.Int64
						|| tc == TypeCode.SByte
						|| tc == TypeCode.UInt16 || tc == TypeCode.UInt32
						|| tc == TypeCode.UInt64)
						return true;
				}
				return false;
			}
		}
		#endregion
		#region IXmlNodeSerializable Members

		public virtual void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_devType = (ObjectRef)XmlSerialization.ReadFromChildXmlNode(serializer, node, XmlSerialization.XML_TYPE, new object[] { null });
			XmlNode child = node.SelectSingleNode(XmlSerialization.LIBTYPE);
			if (child != null)
			{
				_libType = XmlUtility.XmlUtil.GetLibTypeAttribute(child);
			}
			_desc = XmlSerialization.ReadStringValueFromChildNode(node, XmlSerialization.XML_DESCRIPT);
			_name = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
		}

		public virtual void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_devType != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_TYPE, _devType);
				XmlSerialization.RemoveNode(node, XmlSerialization.LIBTYPE);
			}
			else if (_libType != null)
			{
				XmlNode child = XmlUtil.CreateSingleNewElement(node, XmlSerialization.LIBTYPE);
				XmlUtil.SetLibTypeAttribute(child, _libType);
				XmlSerialization.RemoveNode(node, XmlSerialization.XML_TYPE);
			}
			if (!string.IsNullOrEmpty(_name))
			{
				XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, _name);
			}
			XmlSerialization.WriteStringValueToChildNode(node, XmlSerialization.XML_DESCRIPT, _desc);
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (oProp.Name == PROP_NAME || oProp.Name == XmlSerialization.XML_DATATYPE || oProp.Name == XmlSerialization.XML_DESCRIPT)
				{
					if (AllowTypeChange)
					{
						newProps.Add(oProp);
					}
					else
					{
						newProps.Add(new CustomPropertyDescriptor(oProp, true));
					}
				}
				else
				{
					if (!AllowTypeChange)
					{
						newProps.Add(oProp);
					}
				}
			}
			return newProps;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (oProp.Name == PROP_NAME || oProp.Name == XmlSerialization.XML_DATATYPE || oProp.Name == XmlSerialization.XML_DESCRIPT)
				{
					if (AllowTypeChange)
					{
						newProps.Add(oProp);
					}
					else
					{
						newProps.Add(new CustomPropertyDescriptor(oProp, true));
					}
				}
				else
				{
					if (!AllowTypeChange)
					{
						newProps.Add(oProp);
					}
				}
			}
			return newProps;
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;

		}

		#endregion
		#region ICloneable Members

		public virtual object Clone()
		{
			RaisDataType obj = (RaisDataType)Activator.CreateInstance(this.GetType(), _libType, _name);
			obj.Description = Description;
			if (_devType != null)
			{
				obj.DevType = (ObjectRef)_devType.Clone();
			}
			obj.SetServiceProvider(_serviceProvider);
			return obj;
		}

		#endregion
		#region static methods
		/// <summary>
		/// create conversion code 
		/// </summary>
		/// <param name="sourceType"></param>
		/// <param name="sourceCode"></param>
		/// <param name="targetType"></param>
		/// <returns>conversion code converting sourceCode to the targetType</returns>
		public static CodeExpression GetConversionCode(RaisDataType sourceType, CodeExpression sourceCode, RaisDataType targetType, CodeStatementCollection supprtStatements)
		{
			CodeExpression codeRet;
			if (sourceType.IsSameType(targetType))
				return sourceCode;
			if (targetType.IsLibType || sourceType.IsLibType)
			{
				if (MathNode.GetTypeConversion != null)
				{
					return MathNode.GetTypeConversion(targetType.Type, sourceCode, sourceType.Type, supprtStatements);
				}
				return CastOrConvert(sourceCode, sourceType.Type, targetType.Type, supprtStatements);
			}
			string srcType = sourceType.DevType.TypeString;
			string tgtType = targetType.DevType.TypeString;
			if (srcType.StartsWith(tgtType))
			{
				return sourceCode;
			}
			if (tgtType.StartsWith(srcType))
			{
				return new CodeCastExpression(tgtType, VPLUtil.GetCoreExpressionFromCast(sourceCode));
			}
			TypeConverter converter = TypeDescriptor.GetConverter(targetType.Type);
			if (converter.CanConvertFrom(sourceType.Type))
			{
				MathNode.AddImportLocation(typeof(TypeConverter).Assembly.Location);
				string converterName = "conv" + targetType.Type.Name;
				if (!MathNodeVariable.VariableDeclared(supprtStatements, converterName))
				{
					CodeVariableDeclarationStatement cs1 = new CodeVariableDeclarationStatement(
						typeof(TypeConverter), converterName,
						new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter",
						new CodeExpression[] { new CodeSnippetExpression("typeof(" + tgtType + ")") }));
					supprtStatements.Add(cs1);
				}
				//=================================================
				//
				codeRet = new CodeCastExpression(tgtType,
				new CodeMethodInvokeExpression(
					new CodeVariableReferenceExpression(converterName), "ConvertFrom",
					new CodeExpression[] { sourceCode }));
				MathNode.Trace("\tcode 102: source type:{0}, target type:{1} result=converter.ConvertFrom(code);", sourceType, targetType);
			}
			else
			{
				codeRet = CastOrConvert(sourceCode, sourceType.Type, targetType.Type, supprtStatements);
			}
			return codeRet;
		}
		/// <summary>
		/// knowing the code is of different type, make conversion code
		/// </summary>
		/// <param name="sourceValue"></param>
		/// <param name="targetType"></param>
		/// <param name="csc"></param>
		/// <returns></returns>
		static CodeExpression CodeExpConvertTo(CodeExpression sourceCode, Type sourceType, Type targetType, CodeStatementCollection supprtStatements)
		{
			if (targetType.Equals(typeof(void)))
			{
				if (sourceCode == null)
				{
					MathNode.Trace("code 00: target type is void. Input code is null. Return null for code.");
				}
				else
				{
					MathNode.Trace("code 00: target type is void. Input code becomes a statement (0). Return null for code.", sourceCode.ToString());
					supprtStatements.Add(new CodeExpressionStatement(sourceCode));
				}
				return null;
			}
			CodeExpression codeRet;
			TypeConverter converter = TypeDescriptor.GetConverter(targetType);
			if (converter.CanConvertFrom(sourceType))
			{
				MathNode.AddImportLocation(typeof(TypeConverter).Assembly.Location);
				string converterName = "conv" + targetType.Name;
				if (!MathNodeVariable.VariableDeclared(supprtStatements, converterName))
				{
					CodeVariableDeclarationStatement cs1 = new CodeVariableDeclarationStatement(
						typeof(TypeConverter), converterName,
						new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter",
						new CodeExpression[] { new CodeSnippetExpression("typeof(" + targetType.FullName + ")") }));
					supprtStatements.Add(cs1);
				}
				//=================================================
				codeRet = new CodeCastExpression(targetType,
				new CodeMethodInvokeExpression(
					new CodeVariableReferenceExpression(converterName), "ConvertFrom",
					new CodeExpression[] { sourceCode }));
				MathNode.Trace("code 2: source type:{0}, target type:{1} result=converter.ConvertFrom(code);", sourceType, targetType);
			}
			else
			{
				if (sourceCode is CodePrimitiveExpression)
				{
					CodePrimitiveExpression codep = sourceCode as CodePrimitiveExpression;
					if (targetType.Equals(typeof(string)))
					{
						if (codep.Value == null)
						{
							codeRet = new CodeCastExpression(targetType, sourceCode);
							MathNode.Trace("code 3: source type:{0}, target type:{1} result=null;", sourceType, targetType);
						}
						else
						{
							if (codep.Value is string)
							{
								codeRet = sourceCode;
								//
								MathNode.Trace("code 4: source type:{0}, target type:{1} result=code;", sourceType, targetType);
							}
							else
							{
								codeRet = new CodeMethodInvokeExpression(sourceCode, "ToString", new CodeExpression[] { });
								MathNode.Trace("code 5: source type:{0}, target type:{1} result=code.ToString();", sourceType, targetType);
							}
						}
					}
					else //none-string primative
					{
						codeRet = new CodeCastExpression(targetType, sourceCode);
						MathNode.Trace("code 6: source type:{0}, target type:{1}, result=({1})code;", sourceType, targetType);
					}
				}
				else
				{
					if (sourceType.Equals(typeof(void)))
					{
						codeRet = null;
						if (sourceCode == null)
						{
							MathNode.Trace("code 7a: source type:{0}, target type:{1} code is null;", sourceType, targetType);
						}
						else
						{
							supprtStatements.Add(new CodeExpressionStatement(sourceCode));
							MathNode.Trace("code 7: source type:{0}, target type: {1}. code used as a statement; return null as CodeExpression.", sourceType, targetType);
						}
					}
					else if (targetType.Equals(typeof(string)))
					{
						codeRet = new CodeMethodInvokeExpression(sourceCode, "ToString", new CodeExpression[] { });
						MathNode.Trace("code 8: source type:{0}, target type:{1} result=code.ToString();", sourceType, targetType);
					}
					else
					{
						//check to see if casting can be done
						codeRet = new CodeCastExpression(targetType, sourceCode);
						MathNode.Trace("code 9: source type:{0}, target type:{1}. cast it: result=({1})code;", sourceType, targetType);
					}
				}
			}
			return codeRet;
		}
		public static bool IsNumeric(TypeCode tc)
		{
			switch (tc)
			{
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			return false;
		}
		public static string GetConverToName(TypeCode tc)
		{
			switch (tc)
			{
				case TypeCode.Byte:
					return "ToByte";
				case TypeCode.Decimal:
					return "ToDecimal";
				case TypeCode.Double:
					return "ToDouble";
				case TypeCode.Int16:
					return "ToInt16";
				case TypeCode.Int32:
					return "ToInt32";
				case TypeCode.Int64:
					return "ToInt64";
				case TypeCode.SByte:
					return "ToSByte";
				case TypeCode.Single:
					return "ToSingle";
				case TypeCode.UInt16:
					return "ToUInt16";
				case TypeCode.UInt32:
					return "ToUInt32";
				case TypeCode.UInt64:
					return "ToUInt64";
			}
			return "ToBase64CharArray";
		}
		static CodeExpression CastOrConvert(CodeExpression code, Type sourceType, Type targetType, CodeStatementCollection csc)
		{
			if (sourceType.Equals(targetType))
			{
				MathNode.Trace("CastOrConvert 1: same type: {0}", targetType);
				return code;
			}
			if (targetType.IsByRef)
			{
				MathNode.Trace("CastOrConvert 2: by ref type: {0}", targetType);
				return code;
			}
			if (targetType.Equals(typeof(string)))
			{
				MathNode.Trace("CastOrConvert 3: to string from: {0}", sourceType);
				return new CodeMethodInvokeExpression(code, "ToString", new CodeExpression[] { });
			}
			TypeCode tcTarget = Type.GetTypeCode(targetType);
			TypeCode tcSource = Type.GetTypeCode(sourceType);
			if (IsNumeric(tcTarget))
			{
				if (IsNumeric(tcSource))
					return new CodeCastExpression(targetType, VPLUtil.GetCoreExpressionFromCast(code));
				else
				{
					if (tcSource == TypeCode.String)
					{
						return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), GetConverToName(tcTarget), code);
					}
				}
			}
			switch (tcTarget)
			{
				case TypeCode.Boolean:
					if (IsNumeric(tcSource) || tcSource == TypeCode.Char)
					{
						return new CodeBinaryOperatorExpression(code, CodeBinaryOperatorType.IdentityInequality, new CodeCastExpression(sourceType, new CodePrimitiveExpression(0)));
					}
					switch (tcSource)
					{
						case TypeCode.Boolean:
							return code;
						case TypeCode.DateTime:
							return new CodeBinaryOperatorExpression(code, CodeBinaryOperatorType.GreaterThan, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DateTime)), "MinValue"));
						case TypeCode.Object:
							return new CodeBinaryOperatorExpression(code, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
					}
					break;
				case TypeCode.String:
					return new CodeMethodInvokeExpression(code, "ToString");
				case TypeCode.Char:
					if (code is CodePrimitiveExpression)
					{
						CodePrimitiveExpression cp = (CodePrimitiveExpression)code;
						if (cp.Value == null)
						{
							return new CodePrimitiveExpression('\0');
						}
						if (ValueTypeUtil.IsNumber(cp.Value.GetType()))
						{
							short v = Convert.ToInt16(cp.Value);
							return new CodePrimitiveExpression((char)v);
						}
						string s = cp.Value.ToString();
						if (string.IsNullOrEmpty(s))
						{
							return new CodePrimitiveExpression('\0');
						}
						else
						{
							return new CodePrimitiveExpression(s[0]);
						}
					}
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToChar", code);
				case TypeCode.DateTime:
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToDateTime", code);
			}
			if (sourceType.IsSubclassOf(targetType))
			{
				return code;
			}
			if (targetType.IsSubclassOf(sourceType))
			{
				return new CodeCastExpression(targetType, code);
			}
			return CodeExpConvertTo(code, sourceType, targetType, csc);
		}
		#endregion
		#region IDesignServiceProvider Members

		public virtual object GetDesignerService(Type serviceType)
		{
			if (_serviceProvider != null)
			{
				return _serviceProvider.GetDesignerService(serviceType);
			}
			return null;
		}

		public void AddDesignerService(Type serviceType, object service)
		{
			if (_serviceProvider == null)
				_serviceProvider = new ServiceHolder();
			_serviceProvider.AddDesignerService(serviceType, service);
		}

		#endregion

	}
}
