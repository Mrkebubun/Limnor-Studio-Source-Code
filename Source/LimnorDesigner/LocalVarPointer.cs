using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using MathExp;
using ProgElements;
using System.CodeDom;

namespace LimnorDesigner
{
    /// <summary>
    /// method local variable
    /// </summary>
    public class LocalVarPointer:IObjectPointer 
    {
        private MethodClass _method;
        private IClass _pointer;
        //private UInt32 _memberId;
        //private UInt32 _classId;
        //private string _name;
        private Image _img;
        public LocalVarPointer(ComponentIconLocal icon, MethodClass method)
        {
            _method = method;
            //_memberId = icon.MemberId;
            //_classId = method.ClassId;
            //_name = icon.Name;
            _pointer = icon.ClassPointer;
            _img = icon.IconImage;
        }
        public LocalVarPointer(IClass pointer, Image img, MethodClass method)
        {
            _method = method;
            _pointer = pointer;
            _img = img;
        }
        public Image IconImage
        {
            get
            {
                return _img;
            }
        }
        #region IObjectPointer Members

        public ClassPointer RootPointer
        {
            get { return _method.RootPointer; }
        }

        public IObjectPointer Owner
        {
            get
            {
                return _method;
            }
            set
            {
            }
        }

        public Type ObjectType
        {
            get
            {
                return _pointer.ObjectType;
            }
            set
            {
            }
        }

        public object ObjectInstance
        {
            get
            {
                return _pointer;
            }
            set
            {
            }
        }

        public object ObjectDebug
        {
            get
            {
                return _pointer.ObjectDebug;
            }
            set
            {
                _pointer.ObjectDebug = value;
            }
        }

        public string ReferenceName
        {
            get { return _pointer.ReferenceName; }
        }

        public string CodeName
        {
            get { return _pointer.CodeName; }
        }

        public string DisplayName
        {
            get { return _pointer.DisplayName; }
        }

        public bool IsTargeted(EnumObjectSelectType target)
        {
            return _pointer.IsTargeted(target);
        }

        public string ObjectKey
        {
            get { return _pointer.ObjectKey; }
        }

        public string TypeString
        {
            get { return _pointer.TypeString; }
        }

        public bool IsValid
        {
            get { return _pointer.IsValid; }
        }

        public CodeExpression GetReferenceCode(IMethodCompile method)
        {
            return _pointer.GetReferenceCode(method);
        }

        #endregion

        #region IObjectIdentity Members

        public bool IsSameObjectRef(IObjectIdentity objectIdentity)
        {
            return _pointer.IsSameObjectRef(objectIdentity);
        }

        public IObjectIdentity IdentityOwner
        {
            get { return _pointer.IdentityOwner; }
        }

        public bool IsStatic
        {
            get { return _pointer.IsStatic; }
        }

        public EnumObjectDevelopType ObjectDevelopType
        {
            get { return _pointer.ObjectDevelopType; }
        }

        public EnumPointerType PointerType
        {
            get { return _pointer.PointerType; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            LocalVarPointer obj = new LocalVarPointer(_pointer, _img, _method);
            return obj;
        }

        #endregion

        #region ISerializerProcessor Members

        public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
        {
        }

        #endregion
    }
}
