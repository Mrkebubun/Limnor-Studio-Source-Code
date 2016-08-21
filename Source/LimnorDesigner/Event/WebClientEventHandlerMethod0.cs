using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using System.Xml;
using XmlSerializer;
using System.ComponentModel;
using XmlUtility;

namespace LimnorDesigner.Event
{
    
    public class WebClientEventHandlerMethod0 : EventHandlerMethod
    {
        #region fields and constructors
        private BranchList _branchListClient;
        private BranchList _branchListServer;
        private BranchList _branchListDownload;
        private EnumEditWebHandler _editing = EnumEditWebHandler.EditServer;
        public WebClientEventHandlerMethod0(ClassPointer owner)
            : base(owner)
        {
        }
        public WebClientEventHandlerMethod0(ClassPointer owner, IEvent eventOwner)
            : base(owner)
        {
        }
        public WebClientEventHandlerMethod0(HandlerMethodID taskId)
            : base(taskId)
        {
        }
        #endregion
        #region Properties
        [PropertyReadOrder(102, true)]
        [Browsable(false)]
        public BranchList ClientActions
        {
            get
            {
                return _branchListClient;
            }
            set
            {
                _branchListClient = value;
                if (_branchListClient != null)
                {
                    _branchListClient.SetOwnerMethod(this);
                }
            }
        }
        [Browsable(false)]
        public BranchList ServerActions
        {
            get
            {
                return _branchListServer;
            }
        }
        [PropertyReadOrder(106, true)]
        [Browsable(false)]
        public BranchList DownloadActions
        {
            get
            {
                return _branchListDownload;
            }
            set
            {
                _branchListDownload = value;
                if (_branchListDownload != null)
                {
                    _branchListDownload.SetOwnerMethod(this);
                }
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public EnumEditWebHandler Editing
        {
            get
            {
                return _editing;
            }
            set
            {
                _editing = value;
            }
        }
        #endregion
        #region IBeforeSerializeNotify Members

        public override void OnBeforeRead(XmlObjectReader reader, XmlNode node)
        {
            base.OnBeforeRead(reader, node);
        }

        public override void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
        {
            //server actions are saved with MethodClass
            //when editing, ActionList is set to corresponding actions
            ActionList = _branchListServer;
            base.OnBeforeWrite(writer, node);
        }
        #endregion
        #region Protected Methods
        protected override void OnAfterRead()
        {
            //server actions are saved with MethodClass
            //when editing, ActionList is set to corresponding actions
            _branchListServer = ActionList;
        }
        #endregion
    }
}
