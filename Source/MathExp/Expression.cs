using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Xml;
using System.ComponentModel;
using ProgElements;
using XmlUtility;
using System.CodeDom;
using System.Collections.Specialized;

namespace MathExp
{
    public class Expression : IXmlNodeSerializable, ICloneable
    {
        #region fields and constructors
        private MathNodeRoot _exp;
        private bool _hasData;
        public Expression()
        {
        }
        #endregion
        #region Methods
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
        #endregion
        #region IXmlNodeSerializable Members
        const string XMLATT_HasData = "hasData";
        public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
        {
            _exp = new MathNodeRoot();
            _hasData = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_HasData);
            if (_hasData)
            {
                _exp.OnReadFromXmlNode(serializer, node);
            }
        }

        public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
        {
            if (_hasData)
            {
                if (_exp != null)
                {
                    _exp.OnWriteToXmlNode(serializer, node);
                }
            }
            XmlUtil.SetAttribute(node, XMLATT_HasData, _hasData);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
