using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel;
using System.Drawing;
using XmlSerializer;
using System.Xml;
using System.CodeDom;
using MathExp.RaisTypes;
using LimnorDesigner;
using System.Drawing.Design;
using VSPrj;
using XmlUtility;

namespace MathItem
{
    /*
    /// <summary>
    /// a variable
    /// </summary>
    [MathNodeCategory(enumOperatorCategory.System)]
    [Description("a variable")]
    [ToolboxBitmap(typeof(MathNodeVar), "Resources.MathNodeVariable.bmp")]
    public class MathNodeVar : MathNodeVariable, IWithProject
    {
        #region fields and constructors
        const string XMLATT_Subcript = "subscript";
        const string XML_ValueType = "ValueType";
        private string _value = "x";
        //protected string _subscript = "";
        //private int _id;
        //protected Font _subscriptFont;
        private ParameterClass _dataType;
        static MathNodeVar()
        {
            XmlUtil.AddKnownType("MathNodeVar", typeof(MathNodeVar));
        }
        public MathNodeVar(MathNode parent)
            : base(parent)
        {
        }
        #endregion
        #region Properties
        //public UInt64 InstanceId
        //{
        //    get
        //    {
        //        if (_dataType != null)
        //        {
        //            if (!_dataType.IsLibType)
        //            {
        //                return DesignUtil.MakeDDWord((uint)ID, (uint)_dataType.ClassTypePointer.ClassId);
        //            }
        //        }
        //        return DesignUtil.MakeDDWord((uint)ID, (uint)0);
        //    }
        //}
        public override MathExp.RaisTypes.RaisDataType DataType
        {
            get
            {
                if (_dataType == null)
                    return new MathExp.RaisTypes.RaisDataType(typeof(double));
                return new MathExp.RaisTypes.RaisDataType(_dataType.ObjectType);
            }
        }
        //[Description("Variable name")]
        //public string VariableName
        //{
        //    get
        //    {
        //        return _value;
        //    }
        //    set
        //    {
        //        _value = value;
        //    }
        //}
        //[Description("Variable subscript name")]
        //public string SubscriptName
        //{
        //    get
        //    {
        //        return _subscript;
        //    }
        //    set
        //    {
        //        _subscript = value;
        //    }
        //}
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
        [Description("Data type of the variable")]
        public IObjectPointer VariableDataType
        {
            get
            {
                return VariableType.DataType;
            }
            set
            {
                VariableType.DataType = value;
            }
        }
        [Browsable(false)]
        public new ParameterClass VariableType
        {
            get
            {
                if (_dataType == null)
                {
                    _dataType = new ParameterClass(new TypePointer(typeof(double)));
                }
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }
        [Browsable(false)]
        public int ID
        {
            get
            {
                return (int)VariableType.ParameterID;
            }
            set
            {
                VariableType.ParameterID = (UInt32)value;
            }
        }
        /// <summary>
        /// identify this variable in a single string
        /// it does not include ID. 
        /// it only include variable name and subscript name, so, multiple variables may have 
        /// the same KeyName.
        /// Variables with the same KeyName in the same scope can be considered the same variable.
        /// </summary>
        [Browsable(false)]
        public string KeyName
        {
            get
            {
                System.Text.StringBuilder sb = new StringBuilder();
                if (string.IsNullOrEmpty(VariableName))
                    sb.Append("?");
                else
                    sb.Append(VariableName);
                sb.Append(",");
                if (string.IsNullOrEmpty(SubscriptName))
                    sb.Append("?");
                else
                    sb.Append(SubscriptName);
                return sb.ToString();
            }
        }
        /// <summary>
        /// variable name used in code compiling.
        /// in the same scope for all variables with the same KeyName they must
        /// use the same CodeVariableName, the CodeVariableName of the variable with IsInPort = true
        /// </summary>
        [Browsable(false)]
        public string CodeVariableName
        {
            get
            {
                if (root == null)
                {
                    throw new MathException("Error getting CodeVariableName: root is null for variable [{0},{1}]", VariableName, SubscriptName);
                }
                return "v" + root.ID.ToString("x") + "_" + KeyName.GetHashCode().ToString("x");
            }
        }
        #endregion
        #region Methods
        public override void OnReplaceNode(MathNode replaced)
        {
            if (replaced.DataType.IsLibType)
            {
                _dataType.SetDataType(new TypePointer(replaced.DataType.LibType));
            }
        }
        public override object CloneExp(MathNode parent)
        {
            MathNodeVar node = (MathNodeVar)base.CloneExp(parent);
            node.VariableName = _value;
            node.SubscriptName = _subscript;
            //node._id = ID;
            if (_subscriptFont != null)
                node._subscriptFont = (Font)_subscriptFont.Clone();
            node.IsSuperscript = IsSuperscript;
            node.Position = new Point(Position.X, Position.Y);
            node.VariableType = (ParameterClass)VariableType.Clone();
            return node;
        }

        protected override void InitializeChildren()
        {
        }
        protected override void OnSave(XmlNode node)
        {
            XmlUtil.SetAttribute(node, XmlTags.XMLATT_NAME, _value);
            XmlUtil.SetAttribute(node, XMLATT_Subcript, _subscript);
            //XmlUtil.SetAttribute(node, XmlTags.XMLATT_VARID, ID);
            XmlObjectWriter xw = this.root.Serializer as XmlObjectWriter;
            if (xw != null)
            {
                XmlNode nd = node.OwnerDocument.CreateElement(XML_ValueType);
                node.AppendChild(nd);
                xw.WriteObjectToNode(nd, VariableType);
            }
        }
        protected override void OnLoad(XmlNode node)
        {
            _value = XmlUtil.GetAttribute(node, XmlTags.XMLATT_NAME);
            if (string.IsNullOrEmpty(_value))
            {
                _value = "x";
            }
            _subscript = XmlUtil.GetAttribute(node, XMLATT_Subcript);
            //_id = XmlUtil.GetAttributeInt(node, XmlTags.XMLATT_VARID);
            //if (_id == 0)
            //{
            //    _id = Guid.NewGuid().GetHashCode();
            //}
            XmlObjectReader xr = this.root.Serializer as XmlObjectReader;
            if (xr != null)
            {
                XmlNode nd = node.SelectSingleNode(XML_ValueType);
                if (nd != null)
                {
                    _dataType = (ParameterClass)xr.ReadObject(nd, null);
                }
            }
        }
        public override SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
        {
            System.Drawing.Font font = this.TextFont;
            SizeF size = g.MeasureString(ToString(), font);
            _subscriptFont = SubscriptFontMatchHeight(size.Height);
            SizeF ssize = g.MeasureString(_subscript, _subscriptFont);
            return new SizeF(size.Width + ssize.Width + 1, size.Height + ssize.Height / 2);
        }

        public override void OnDraw(System.Drawing.Graphics g)
        {
            SizeF sz = DrawSize;
            System.Drawing.Font font = this.TextFont;
            string s = ToString();
            SizeF size = g.MeasureString(s, font);
            if (IsFocused)
            {
                g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
                g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
                if (!string.IsNullOrEmpty(_subscript))
                {
                    g.DrawString(_subscript, _subscriptFont, this.TextBrushFocus, new PointF(size.Width, size.Height / (float)2));
                }
            }
            else
            {
                g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
                if (!string.IsNullOrEmpty(_subscript))
                {
                    g.DrawString(_subscript, _subscriptFont, this.TextBrush, new PointF(size.Width, size.Height / (float)2));
                }
            }
        }
        public override CodeExpression ExportCode(IMethodCompile method)
        {
            CodeStatementCollection supprtStatements = method.MethodCode.Statements;
            if (!MathNodeVariable.VariableDeclared(supprtStatements, this.CodeVariableName))
            {
                MathNode.Trace("Declare variable {0}", this.TraceInfo);
                supprtStatements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(VariableType.ObjectType), CodeVariableName, ValueTypeUtil.GetDefaultCodeByType(VariableType.ObjectType)));
            }
            MathNode.Trace("MathNodeVar.ExportCode returns variable reference to {0}", this.CodeVariableName);
            return new CodeVariableReferenceExpression(this.CodeVariableName);
        }
        public override string ToString()
        {
            return _value;
        }
        [Browsable(false)]
        public override string TraceInfo
        {
            get
            {
                System.Text.StringBuilder sb = new StringBuilder(this.GetType().Name);
                sb.Append(" ");
                sb.Append(this.DataType.TypeName);
                sb.Append(":");
                if (string.IsNullOrEmpty(this.VariableName))
                    sb.Append("?");
                else
                    sb.Append(this.VariableName);
                sb.Append(":");
                //if (string.IsNullOrEmpty(this.CodeVariableName))
                //    sb.Append("?");
                //else
                //    sb.Append(this.CodeVariableName);
                sb.Append(" ID:");
                sb.Append(this.ID.ToString());
                sb.Append(" ");
                sb.Append(this.CodeVariableName);
                return sb.ToString();
            }
        }
        #endregion
        #region IWithProject Members
        [Browsable(false)]
        public LimnorProject Project
        {
            get 
            {
                if (root == null)
                {
                    throw new MathException("Error getting Project: root is null for variable [{0},{1}]", VariableName, SubscriptName);
                }
                return this.root.Project as LimnorProject;
            }
        }

        #endregion
    }
    */
}
