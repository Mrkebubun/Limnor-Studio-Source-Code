using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgElements;

namespace LimnorDesigner.Action
{
    /// <summary>
    /// action returning a value as a method return
    /// </summary>
    public class ActionMethodReturn2:IAction
    {
        private UInt32 _id;
        private ClassPointer _class;
        private ParameterValue _returnValue;
        public ActionMethodReturn2()
        {
        }

        #region IAction Members

        public bool IsMethodReturn
        {
            get { return true; }
        }

        public bool IsLocal
        {
            get { return true; }
        }

        public bool AsLocal
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public bool IsPublic
        {
            get { return false; }
        }

        public uint ActionId
        {
            get
            {
                if (_id == 0)
                {
                    _id = (UInt32)(Guid.NewGuid().GetHashCode());
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public uint ClassId
        {
            get 
            {
                if (_class != null)
                {
                    return _class.ClassId;
                }
                return 0;
            }
        }

        public ClassPointer Class
        {
            get { return _class; }
        }

        public uint ExecuterClassId
        {
            get 
            {
                return ClassId;
            }
        }

        public uint ExecuterMemberId
        {
            get 
            {
                if (_class != null)
                {
                    return _class.MemberId;
                }
                return 0;
            }
        }

        public string ActionName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                
            }
        }

        public string Display
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Type ViewerType
        {
            get { throw new NotImplementedException(); }
        }

        public IObjectPointer MethodOwner
        {
            get { throw new NotImplementedException(); }
        }

        public IMethod ActionMethod
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int ParameterCount
        {
            get { throw new NotImplementedException(); }
        }

        public List<ParameterValue> ParameterValues
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSameMethod(IAction act)
        {
            throw new NotImplementedException();
        }

        public void SetOwner(ClassPointer owner)
        {
            throw new NotImplementedException();
        }

        public bool Edit(XmlSerializer.XmlObjectWriter writer, ProgElements.IMethod context, System.Windows.Forms.Form caller)
        {
            throw new NotImplementedException();
        }

        public void Execute(List<ParameterClass> eventParameters)
        {
            throw new NotImplementedException();
        }

        public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
        {
            throw new NotImplementedException();
        }

        public uint ScopeMethodId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IObjectIdentity Members

        public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
        {
            throw new NotImplementedException();
        }

        public ProgElements.IObjectIdentity IdentityOwner
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsStatic
        {
            get { throw new NotImplementedException(); }
        }

        public ProgElements.EnumObjectDevelopType ObjectDevelopType
        {
            get { throw new NotImplementedException(); }
        }

        public ProgElements.EnumPointerType PointerType
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEventHandler Members

        public ulong WholeActionId
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IBreakPointOwner Members

        public bool BreakBeforeExecute
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool BreakAfterExecute
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IBeforeSerializeNotify Members

        public void OnBeforeRead(XmlSerializer.XmlObjectReader reader, System.Xml.XmlNode node)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeWrite(XmlSerializer.XmlObjectWriter writer, System.Xml.XmlNode node)
        {
            throw new NotImplementedException();
        }

        public void ReloadFromXmlNode()
        {
            throw new NotImplementedException();
        }

        public void UpdateXmlNode(XmlSerializer.XmlObjectWriter writer)
        {
            throw new NotImplementedException();
        }

        public System.Xml.XmlNode XmlData
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
