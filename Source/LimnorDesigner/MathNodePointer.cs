using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.CodeDom;
using XmlSerializer;
using System.Xml;

namespace LimnorDesigner
{
    public class PropertyPointWrapper
    {
        public PropertyPointWrapper()
        {
        }
        public Type ObjectType { get; set; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [MathNodeCategoryAttribute(enumOperatorCategory.Other)]
    [Description("A property")]
    [ToolboxBitmapAttribute(typeof(MathNodePointer), "Resources.property.bmp")]
    public class MathNodePointer : MathNode, IDataScope, IPropertyPointer, IXmlNodeSerialization
    {
        #region fields and constructors
        private IObjectPointer _valuePointer;
        public MathNodePointer(MathNode parent)
            : base(parent)
        {
        }
        #endregion
        #region properties
        [IgnoreReadOnlyAttribute]
        [ReadOnly(true)]
        [Editor(typeof(PropEditorPropertyPointer), typeof(UITypeEditor))]
        [Description("It points to a property of an object")]
        public IObjectPointer Property
        {
            get
            {
                if (_valuePointer == null)
                {
                    ObjectIDmap map = (ObjectIDmap)MathNode.GetService(typeof(ObjectIDmap));
                    if (map != null && map.Count > 0)
                    {
                        object v = map.GetObjectByID((uint)map.MemberId);
                        string name = DesignUtil.GetObjectNameById(map.MemberId, map.XmlData);
                        _valuePointer = new PropertyPointer();
                        RootClassId cid = new RootClassId(map.ClassId, map.MemberId, name, map, map.XmlData, v);
                        cid.ObjectInstance = map.GetObjectByID(1);
                        _valuePointer.Owner = cid;
                    }
                }
                return _valuePointer;
            }
            set
            {
                if (value != null)
                {
                    _valuePointer = value;
                }
            }
        }
        public override MathExp.RaisTypes.RaisDataType DataType
        {
            get 
            {
                if (_valuePointer == null)
                    return new MathExp.RaisTypes.RaisDataType(typeof(double));
                return new MathExp.RaisTypes.RaisDataType(_valuePointer.ObjectType);
            }
        }
        [Browsable(false)]
        public override string TraceInfo
        {
            get 
            {
                if (_valuePointer == null)
                    return "{null}";
                return _valuePointer.ToString();
            }
        }
        #endregion
        #region methods
        protected override void InitializeChildren()
        {
            ChildNodeCount = 0;
        }
        public override string ToString()
        {
            string s;
            if (_valuePointer == null)
                s = "{null}";
            else
                s = _valuePointer.ToString();
            return s;
        }
        public override SizeF OnCalculateDrawSize(Graphics g)
        {
            string s = this.ToString();
            return g.MeasureString(s, TextFont);
        }

        public override void OnDraw(Graphics g)
        {
            string s = this.ToString();
            if (IsFocused)
            {
                g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
                g.DrawString(s, TextFont, TextBrushFocus, (float)0, (float)0);
            }
            else
            {
                g.DrawString(s, TextFont, TextBrush, (float)0, (float)0);
            }
        }

        public override CodeExpression ExportCode(MathExp.RaisTypes.MethodType method)
        {
            return new CodeArgumentReferenceExpression(CodeVariableName);
        }
        /// <summary>
        /// do nothing
        /// </summary>
        /// <param name="replaced">the node replaced by this</param>
        public override void OnReplaceNode(MathNode replaced)
        {
        }
        protected override void OnSave(XmlNode node)
        {
            if (_valuePointer != null)
            {
                XmlObjectWriter xw = this.root.Serializer as XmlObjectWriter;
                if (xw != null)
                {
                    //    try
                    //    {
                    //        ObjectIDmap map = (ObjectIDmap)MathNode.GetService(typeof(ObjectIDmap));
                    //        XmlObjectWriter xw = new XmlObjectWriter(map);
                    XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Property);
                    node.AppendChild(nd);
                    xw.WriteObjectToNode(nd, _valuePointer);
                    //        if (xw.HasErrors)
                    //        {
                    //            MathNode.Log(xw.ErrorCollection);
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        MathNode.Log(e);
                }
            }
        }
        protected override void OnLoad(XmlNode node) 
        {
            XmlObjectReader xr = this.root.Serializer as XmlObjectReader;
            if (xr != null)
            {
                XmlNode nd = node.SelectSingleNode(XmlTags.XML_Property);
                if (nd != null)
                {
                    //    ObjectIDmap map = (ObjectIDmap)XmlObjectReader.GetService(typeof(ObjectIDmap));
                    //    XmlObjectReader xr = new XmlObjectReader(map);
                    _valuePointer = (IObjectPointer)xr.ReadObject(nd, null);
                }
            }
        }
        public override object CloneExp(MathNode parent)
        {
            MathNodePointer node = (MathNodePointer)base.CloneExp(parent);
            if (_valuePointer != null)
            {
                node.Property = (IObjectPointer)_valuePointer.Clone();
            }
            return node;
        }
        #endregion
        #region IDataScope Members
        [Browsable(false)]
        public Type ScopeDataType
        {
            get
            {
                if(_valuePointer != null)
                    return _valuePointer.ObjectType;
                return typeof(double);
            }
            set
            {
                if (_valuePointer != null)
                    _valuePointer.ObjectType = value;
            }
        }
        [Browsable(false)]
        public IObjectPointer ScopeOwner 
        {
            get
            {
                return _valuePointer;
            }
            set
            {
                _valuePointer = value;
            }
        }

        #endregion
        #region IPropertyPointer Members

        public string CodeVariableName
        {
            get { return "h_" + ((uint)(_valuePointer.ObjectKey.GetHashCode())).ToString("x"); }
        }
        public Type PointerDataType
        {
            get
            {
                return ScopeDataType;
            }
        }
        public object PointerValue
        {
            get
            {
                return _valuePointer.ObjectInstance;
            }
        }
        #endregion

        #region IXmlNodeSerialization Members

        public void WriteToXmlNode(XmlObjectWriter writer, XmlNode node)
        {
            if (_valuePointer != null)
            {
                //try
                //{
                XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Property);
                node.AppendChild(nd);
                writer.WriteObjectToNode(nd, _valuePointer);
                //    if (xw.HasErrors)
                //    {
                //        MathNode.Log(xw.ErrorCollection);
                //    }
                //}
                //catch (Exception e)
                //{
                //    MathNode.Log(e);
                //}
            }
        }

        public void ReadFromXmlNode(XmlObjectReader reader, XmlNode node)
        {
            XmlNode nd = node.SelectSingleNode(XmlTags.XML_Property);
            if (nd != null)
            {
                _valuePointer = (IObjectPointer)reader.ReadObject(nd, null);
            }
        }

        #endregion
    }
}
