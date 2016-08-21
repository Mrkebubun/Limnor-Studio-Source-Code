using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Event;
using System.ComponentModel;
using System.Globalization;
using LimnorDesigner.Action;
using System.Xml;
using XmlSerializer;
using ProgElements;
using System.Windows.Forms;
using MathExp;
using System.CodeDom;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
    class ActionAssignHandler:IAction
    {
        private IEvent _event;
        public ActionAssignHandler(IEvent e)
        {
            _event = e;
        }
        public IEvent Event
        {
            get
            {
                return _event;
            }
        }
        #region IAction Members

        public bool IsMethodReturn
        {
            get { return false; }
        }

        public bool IsLocal
        {
            get { return true; }
        }
        [ReadOnly(true)]
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

        public bool IsValid
        {
            get { return (_event != null && _event.IsValid); }
        }
        [ReadOnly(true)]
        public bool HideFromRuntimeDesigners
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public bool IsStaticAction
        {
            get 
            {
                if (_event == null)
                    return false;
                return _event.IsStatic; 
            }
        }
        [ReadOnly(true)]
        public uint ActionId
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public uint ClassId
        {
            get 
            {
                if (_event != null && _event.RootPointer != null)
                    return _event.RootPointer.ClassId;
                return 0;
            }
        }

        public ClassPointer Class
        {
            get
            {
                if (_event != null && _event.RootPointer != null)
                    return _event.RootPointer;
                return null;
            }
        }

        public uint ExecuterClassId
        {
            get 
            {
                return this.ClassId;
            }
        }

        public uint ExecuterMemberId
        {
            get 
            {
                return 0;
            }
        }

        public ClassPointer ExecuterRootHost
        {
            get 
            {
                return this.Class;
            }
        }
        [ReadOnly(true)]
        public string ActionName
        {
            get
            {
                if (_event == null)
                    return "HandleEvent";
                return string.Format(CultureInfo.InvariantCulture, "Handle_{0}", _event.Name);
            }
            set
            {
                
            }
        }

        public string Display
        {
            get { return ActionName; }
        }
        [ReadOnly(true)]
        public string Description
        {
            get
            {
                return Display;
            }
            set
            {
            }
        }

        public Type ViewerType
        {
            get { return typeof(ActionViewerAssignAction); }
        }

        public Type ActionBranchType
        {
            get { return typeof(AB_AssignActions); }
        }

        public IObjectPointer MethodOwner
        {
            get 
            {
                if (_event != null)
                    return _event.Owner;
                return null;
            }
        }
        [ReadOnly(true)]
        public IActionMethodPointer ActionMethod
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public int ParameterCount
        {
            get { return 0; }
        }
        [ReadOnly(true)]
        public ParameterValueCollection ParameterValues
        {
            get
            {
                return new ParameterValueCollection();
            }
            set
            {
                
            }
        }

        public XmlNode CurrentXmlData
        {
            get { return null; }
        }

        public bool HasChangedXmlData
        {
            get { return false; }
        }

        public bool IsSameMethod(IAction act)
        {
            ActionAssignHandler ah = act as ActionAssignHandler;
            if (ah.Event != null)
            {
                return ah.Event.IsSameObjectRef(_event);
            }
            return false;
        }

        public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
        {
            return false;
        }

        public void Execute(List<ParameterClass> eventParameters)
        {
           
        }

        public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
        {
            
        }

        public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
        {
            
        }

        public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
        {
            
        }

        public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
        {
            
        }

        public uint ScopeMethodId
        {
            get { return 0; }
        }

        public uint SubScopeId
        {
            get { return 0; }
        }
        [ReadOnly(true)]
        public IMethod ScopeMethod
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
        [ReadOnly(true)]
        public IActionsHolder ActionHolder
        {
            get
            {
                return null;
            }
            set
            {
               
            }
        }

        public void ResetScopeMethod()
        {
            
        }

        public IAction CreateNewCopy()
        {
            return this;
        }
        [ReadOnly(true)]
        public ExpressionValue ActionCondition
        {
            get
            {
                return null;
            }
            set
            {
               
            }
        }
        [ReadOnly(true)]
        public bool Changed
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        public IObjectPointer ReturnReceiver
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

        public void SetParameterValue(string name, object value)
        {
            throw new NotImplementedException();
        }

        public void ValidateParameterValues()
        {
            throw new NotImplementedException();
        }

        public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
        {
            throw new NotImplementedException();
        }

        public EventHandler GetPropertyChangeHandler()
        {
            throw new NotImplementedException();
        }

        public void EstablishObjectOwnership(LimnorDesigner.Action.IActionsHolder scope)
        {
            throw new NotImplementedException();
        }

        public void ResetDisplay()
        {
            throw new NotImplementedException();
        }

        public VSPrj.LimnorProject Project
        {
            get { throw new NotImplementedException(); }
        }

        public void OnAfterDeserialize(LimnorDesigner.Action.IActionsHolder actionsHolder)
        {
            throw new NotImplementedException();
        }

        public void CreateJavaScript(System.Collections.Specialized.StringCollection sb, Dictionary<string, System.Collections.Specialized.StringCollection> formSubmissions, string nextActionInput, string indent)
        {
            throw new NotImplementedException();
        }

        public LimnorDesigner.Action.EnumWebActionType WebActionType
        {
            get { throw new NotImplementedException(); }
        }

        public void CheckWebActionType()
        {
            throw new NotImplementedException();
        }

        public IList<MathExp.ISourceValuePointer> GetClientProperties(uint taskId)
        {
            throw new NotImplementedException();
        }

        public IList<MathExp.ISourceValuePointer> GetServerProperties(uint taskId)
        {
            throw new NotImplementedException();
        }

        public IList<MathExp.ISourceValuePointer> GetUploadProperties(uint taskId)
        {
            throw new NotImplementedException();
        }

        public VPL.EnumWebRunAt ScopeRunAt
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

        #region IActionContextExt Members

        public object GetParameterValue(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IActionContext Members

        public uint ActionContextId
        {
            get { throw new NotImplementedException(); }
        }

        public object GetParameterType(uint id)
        {
            throw new NotImplementedException();
        }

        public object GetParameterType(string name)
        {
            throw new NotImplementedException();
        }

        public object ProjectContext
        {
            get { throw new NotImplementedException(); }
        }

        public object OwnerContext
        {
            get { throw new NotImplementedException(); }
        }

        public ProgElements.IMethod ExecutionMethod
        {
            get { throw new NotImplementedException(); }
        }

        public void OnChangeWithinMethod(bool withinMethod)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IXmlNodeHolder Members

        public System.Xml.XmlNode DataXmlNode
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
    }
}
