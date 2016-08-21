/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.CodeDom;
using ProgElements;
using System.ComponentModel;
using XmlUtility;
using System.Collections.Specialized;
using VPL;
using System.Xml.Serialization;

namespace MathExp
{
	/// <summary>
	/// math expression used as a value
	/// </summary>
	[TypeConverter(typeof(ExpressionValueConverter))]
	public class ExpressionValue : IXmlNodeSerializable, ICloneable, ISourceValuePointersHolder
	{
		#region fields and constructors
		private MathNodeRoot _exp;
		private bool _hasData;
		public ExpressionValue()
		{
		}
		#endregion
		#region Methods
		public void SetActionInputName(string name, Type type)
		{
			if (_exp != null)
			{
				_exp.SetActionInputName(name, type);
			}
		}
		public MathNodeRoot GetExpression()
		{
			return _exp;
		}
		public override string ToString()
		{
			if (_hasData && _exp != null)
			{
				return _exp.ToString();
			}
			return "true";
		}
		public IList<ISourceValuePointer> GetValueSources()
		{
			if (_exp != null)
			{
				return _exp.GetValueSources();
			}
			return null;
		}
		public void SetExpression(MathNodeRoot expression)
		{
			_exp = expression;
			_hasData = (_exp != null);
		}
		public CodeExpression ExportCode(IMethodCompile method)
		{
			if (_hasData && _exp != null)
			{
				MathNodeNumber mn = _exp[1] as MathNodeNumber;
				if (mn == null || !mn.IsPlaceHolder)
				{
					return _exp.ExportCode(method);
				}
			}
			return null;
		}
		public string CreateJavaScriptCode(StringCollection method)
		{
			if (_hasData && _exp != null)
			{
				MathNodeNumber mn = _exp[1] as MathNodeNumber;
				if (mn == null || !mn.IsPlaceHolder)
				{
					return _exp.CreateJavaScript(method);
				}
			}
			return null;
		}
		public string CreatePhpScriptCode(StringCollection method)
		{
			if (_hasData && _exp != null)
			{
				MathNodeNumber mn = _exp[1] as MathNodeNumber;
				if (mn == null || !mn.IsPlaceHolder)
				{
					return _exp.CreatePhpScript(method);
				}
			}
			return null;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public MathNodeRoot MathExp
		{
			get
			{
				return _exp;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool HasData
		{
			get
			{
				return _hasData;
			}
		}
		[XmlIgnore]
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod ScopeMethod
		{
			get
			{
				if (_exp != null)
					return _exp.ScopeMethod;
				return null;
			}
			set
			{
				if (_exp == null)
				{
					_exp = new MathNodeRoot();
				}
				_exp.ScopeMethod = value;
			}
		}
		public Type DataType
		{
			get
			{
				if (_exp == null)
				{
					return typeof(object);
				}
				return _exp[1].DataType.Type;
			}
		}
		public bool IsValid
		{
			get
			{
				if (_hasData)
				{
					if (_exp != null)
					{
						return _exp.IsValid;
					}
				}
				return true;
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		const string XMLATT_HasData = "hasData";
		public virtual void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (_hasData)
			{
				if (_exp != null)
				{
					_exp.OnWriteToXmlNode(writer, node);
				}
			}
			XmlUtil.SetAttribute(node, XMLATT_HasData, _hasData);
		}

		public virtual void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			_exp = new MathNodeRoot();
			_hasData = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_HasData);
			if (_hasData)
			{
				_exp.OnReadFromXmlNode(reader, node);
			}
		}

		#endregion

		#region ICloneable Members

		public virtual object Clone()
		{
			ExpressionValue ev = (ExpressionValue)Activator.CreateInstance(this.GetType());
			if (_exp != null)
			{
				ev._exp = (MathNodeRoot)_exp.Clone();
			}
			ev._hasData = _hasData;
			return ev;
		}

		#endregion
	}
}
