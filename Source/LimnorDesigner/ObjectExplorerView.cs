using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Longflow.VSMain;
using System.ComponentModel.Design;
using System.Xml;
using XmlSerializer;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using LimnorDesigner.Interface;
using VPL;
using XmlUtility;

namespace LimnorDesigner
{
    [Description("Object Explorer manages all programming entities via a tree view.")]
    public partial class ObjectExplorerView : UserControl, IVsTextLinesEvents, IXDesignerViewer
    {
        #region fields and constructors
        uint cookie;
        MultiPanes _holder;
        IVsTextLines _textLines;
        LimnorXmlDesignerLoader _loader;
        TreeViewObjectExplorer _objExplorer;
        public ObjectExplorerView(IVsTextLines textLines, LimnorXmlDesignerLoader designerLoader)
        {
            InitializeComponent();
            _loader = designerLoader;
            _textLines = textLines;
            BackColor = Color.LightGray;
            _objExplorer = new TreeViewObjectExplorer();
            _objExplorer.SetProject(designerLoader.Project);
            this.splitContainer1.Panel1.Controls.Add(_objExplorer);
            _objExplorer.Dock = DockStyle.Fill;
            AdviseTextLinesEvents(true);
            txtDesc.AcceptsTab = true;
            txtDesc.AcceptsReturn = true;
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add(new MenuItem("View/Edit",mnu_editDesc));
            txtDesc.ContextMenu = cm;
        }
        private void mnu_editDesc(object sender, EventArgs e)
        {
            DlgText dlg = new DlgText();
            dlg.SetText(txtDesc.Text);
            if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                txtDesc.Text = dlg.TextRet;
                SerializeUtil.SetNodeDescription(_loader.Node, txtDesc.Text);
                _loader.NotifyChanges();
            }
        }
        #endregion
        #region Setup IVsTextLines Event Sink
        /// <summary>
        /// Helper routine to register our view within the visual studio
        /// </summary>
        /// <param name="subscribe">Subscribe or to Unsubscribe flag</param>
        private void AdviseTextLinesEvents(bool subscribe)
        {
            IConnectionPointContainer conPtCont;
            conPtCont = (IConnectionPointContainer)_textLines;
            if (conPtCont != null)
            {
                IConnectionPoint conPt;
                Guid iidConPt = typeof(IVsTextLinesEvents).GUID;
                conPtCont.FindConnectionPoint(ref iidConPt, out conPt);
                if (subscribe)
                {
                    conPt.Advise((IVsTextLinesEvents)this, out cookie);
                }
                else
                {
                    // If the goal is to unsubscribe, but there is no subscription active, exit
                    if (cookie == VSConstants.VSCOOKIE_NIL)
                    {
                        return;
                    }
                    conPt.Unadvise(cookie);
                    cookie = VSConstants.VSCOOKIE_NIL;
                }
            }
            //// register our independent view with the IVsTextManager so that it knows
            //// the user is working with a view over the text buffer. this will trigger
            //// the text buffer to prompt the user whether to reload the file if it is
            //// edited outside of the development Environment
            //IVsTextManager textManager = (IVsTextManager)GetService(typeof(SVsTextManager));

            //if (textManager != null)
            //{
            //    if (subscribe)
            //    {
            //        textManager.RegisterIndependentView((IVsWindowPane)this, (IVsTextLines)_textLines);
            //    }
            //    else
            //    {
            //        textManager.UnregisterIndependentView((IVsWindowPane)this, (IVsTextLines)_textLines);
            //    }
            //}
        }
        #endregion
        #region IVsTextLinesEvents Members

        public void OnChangeLineAttributes(int iFirstLine, int iLastLine)
        {
        }

        public void OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
        {
//            MessageBox.Show("OnText");
            //textBox1.Text = GetText();

            //if (_objExplorer != null)
            //{
            //    _objExplorer.ResetClassNodes();
            //}
        }

        #endregion
        #region IXDesignerViewer Members
        public void OnEventListChanged(ICustomEventMethodDescriptor owner, UInt32 objectId)
        {
            _objExplorer.OnEventListChanged(owner, objectId);
        }
        public void OnResetMap()
        {
            _objExplorer.OnResetMap();
        }
        public void OnDatabaseListChanged()
        {
            _objExplorer.OnDatabaseListChanged();
        }
        public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
        {
            _objExplorer.OnDatabaseConnectionNameChanged(connectionid, newName);
        }
        public void OnObjectNameChanged(INonHostedObject obj)
        {
            _objExplorer.OnObjectNameChanged(obj);
        }
        /// <summary>
        /// called by MultiPanes inside AddViewer 
        /// </summary>
        /// <param name="mp"></param>
        public void SetDesigner(MultiPanes mp)
        {
            _holder = mp;
            //_objExplorer.RootClassNode.ClassData.SetDesignerHolder(mp);
        }

        public Control GetDesignSurfaceUI()
        {
            return this;
        }
        public void OnDataLoaded()
        {
            ClassPointer objId = _loader.GetRootId();
            TreeNodeClassRoot r = TreeNodeClassRoot.CreateClassRoot(true, objId, EnumObjectSelectType.All, null, null, _objExplorer.StaticScope);
            //r.ClassData.SetDesignerHolder(_holder);
            r.SetDesigner(_loader);
            _objExplorer.Nodes.Add(r);
            if (!objId.IsStatic)
            {
                _objExplorer.Nodes.Add(new TreeNodeDatabaseConnectionList(objId));
            }
            _objExplorer.Nodes.Add(new TreeNodeActionCollection(r, false, objId, 0, EnumObjectSelectType.All, null, null));
            _objExplorer.LoadTypeNodes();
            txtDesc.Text = SerializeUtil.GetNodeDescription(_loader.Node);
        }
        public void OnUIConfigLoaded()
        {
            XmlNode node = _loader.Node;
            if (node != null)
            {
                int n = XmlUtil.GetAttributeInt(node, XMLATTR_SplitterDistance);
                if (n > 10 && n < splitContainer1.ClientSize.Height - 10)
                {
                    splitContainer1.SplitterDistance = n;
                }
            }
        }
        public EnumMaxButtonLocation MaxButtonLocation
        {
            get
            {
                return EnumMaxButtonLocation.Right;
            }
        }
        public void OnActionSelected(IAction action)
        {
            _loader.NotifySelection(action);
        }
        public void OnClassLoadedIntoDesigner(UInt32 classId)
        {
            _objExplorer.OnClassLoadedIntoDesigner(classId);
        }
        public void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade)
        {
            _objExplorer.OnDefinitionChanged(classId, relatedObject, changeMade);
        }
        public void OnComponentAdded(object obj)
        {
            _objExplorer.OnComponentAdded(obj);
        }
        /// <summary>
        /// find the tree node for the object to adjust text
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newName"></param>
        public void OnComponentRename(object obj, string newName)
        {
            ObjectIDmap map = _loader.ObjectMap.GetMap(obj);
            if (map == null)
            {
                throw new DesignerException("object map not found for {0} when setting new name {1}", obj, newName);
            }
            IObjectPointer o = DesignUtil.CreateObjectPointer(map,obj);
            TreeNodeObject node = _objExplorer.LocateNode(o) as TreeNodeObject;
            if (node != null)
            {
                node.ShowText();
            }
        }
        public void OnComponentSelected(object obj)
        {
            _objExplorer.OnComponentSelected(obj);
        }
        public void OnComponentRemoved(object obj)
        {
            ObjectIDmap map = null;
            ClassInstancePointer cr = obj as ClassInstancePointer;
            if (cr != null)
            {
                map = _loader.ObjectMap.GetMap(cr.ObjectInstance);
            }
            else
            {
                map = _loader.ObjectMap.GetMap(obj);
            }
            //if (map == null)
            //{
            //    throw new DesignerException("object map not found for {0} when removing component", obj);
            //}
            if (map != null)
            {
                TreeNode node = null;
                if (cr != null)
                {
                    node = _objExplorer.LocateNode(cr);
                }
                else
                {
                    IObjectPointer o = DesignUtil.CreateObjectPointer(map, obj);
                    node = _objExplorer.LocateNode(o);
                }
                if (node != null)
                {
                    node.Remove();
                }
            }
        }
        /// <summary>
        /// adjust display
        /// </summary>
        /// <param name="ea"></param>
        public void OnAssignAction(EventAction ea)
        {
            _objExplorer.OnAssignAction(ea);
        }
        public void OnRemoveEventHandler(EventAction ea, TaskID task)
        {
            _objExplorer.OnRemoveEventHandler(ea, task);
        }
        public void OnRemoveAllEventHandlers(IEvent e)
        {
            _objExplorer.OnRemoveAllEventHandlers(e);
        }
        public void OnActionListOrderChanged(object sender, EventAction ea)
        {
            _objExplorer.OnActionListOrderChanged(sender, ea);
        }
        public void OnActionChanged(UInt32 classId, IAction act, bool isNewAction)
        {
            _objExplorer.OnActionChanged(classId, act, isNewAction);
        }
        public void OnInterfaceAdded(InterfacePointer interfacePointer)
        {
            _objExplorer.OnInterfaceAdded(interfacePointer);
        }
        public void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface)
        {
            _objExplorer.OnBaseInterfaceAdded(owner, baseInterface);
        }
        public void OnRemoveInterface(InterfacePointer interfacePointer)
        {
            _objExplorer.OnRemoveInterface(interfacePointer);
        }
        public void OnInterfaceEventDeleted(InterfaceElementEvent eventType)
        {
            _objExplorer.OnInterfaceEventDeleted(eventType);
        }
        public void OnInterfaceEventAdded(InterfaceElementEvent eventType)
        {
            _objExplorer.OnInterfaceEventAdded(eventType);
        }
        public void OnInterfaceEventChanged(InterfaceElementEvent eventType)
        {
            _objExplorer.OnInterfaceEventChanged(eventType);
        }
        public void OnInterfacePropertyDeleted(InterfaceElementProperty property)
        {
            _objExplorer.OnInterfacePropertyDeleted(property);
        }
        public void OnInterfacePropertyAdded(InterfaceElementProperty property)
        {
            _objExplorer.OnInterfacePropertyAdded(property);
        }
        public void OnInterfacePropertyChanged(InterfaceElementProperty property)
        {
            _objExplorer.OnInterfacePropertyChanged(property);
        }
        public void OnInterfaceMethodDeleted(InterfaceElementMethod method)
        {
            _objExplorer.OnInterfaceMethodDeleted(method);
        }
        public void OnInterfaceMethodChanged(InterfaceElementMethod method)
        {
            _objExplorer.OnInterfaceMethodChanged(method);
        }
        public void OnInterfaceMethodCreated(InterfaceElementMethod method)
        {
            _objExplorer.OnInterfaceMethodCreated(method);
        }
        public void OnMethodChanged(/*object creator,*/ MethodClass method, bool isNewMethod)
        {
            _objExplorer.OnMethodChanged(/*creator,*/ method, isNewMethod);
        }
        public void OnDeleteMethod(MethodClass method)
        {
            _objExplorer.OnDeleteMethod(method);
        }
        public void OnMethodSelected(MethodClass method)
        {
            _objExplorer.OnMethodSelected(method);
        }
        public void OnActionDeleted(IAction action)
        {
            _objExplorer.OnActionDeleted(action);
        }
        public void OnDeleteEventMethod(EventHandlerMethod method)
        {
            _objExplorer.OnDeleteEventMethod(method);
        }
        public void OnDeleteProperty(PropertyClass property)
        {
            _objExplorer.OnDeleteProperty(property);
        }
        public void OnAddProperty(PropertyClass property)
        {
            _objExplorer.OnAddProperty(property);
        }
        public void OnPropertySelected(PropertyClass property)
        {
            _objExplorer.OnPropertySelected(property);
        }
        public void OnPropertyChanged(INonHostedObject property, string name)
        {
            _objExplorer.OnPropertyChanged(property, name);
        }
        //public void OnActionNameChanged(string newActionName, UInt64 wholeActionId)
        //{
        //    //_objExplorer.OnActionNameChanged(newActionName, WholeActionId);
        //}
        public void OnDeleteEvent(EventClass eventObject)
        {
            _objExplorer.OnDeleteEvent(eventObject);
        }
        public void OnAddEvent(EventClass eventObject)
        {
            _objExplorer.OnAddEvent(eventObject);
        }
        public void OnEventSelected(EventClass eventObject)
        {
            _objExplorer.OnEventSelected(eventObject);
        }
        public void OnFireEventActionSelected(FireEventMethod method)
        {
            _objExplorer.OnFireEventActionSelected(method);
        }
        public void OnIconChanged(UInt32 classId)
        {
            _objExplorer.OnIconChanged(classId);
            Image bmp = _objExplorer.GetRootObjectIcon();
            if (bmp != null)
            {
                _loader.ViewerHolder.ObjectIcon = bmp;
            }
        }
        public void SetClassRefIcon(UInt32 classId, System.Drawing.Image img)
        {
            _objExplorer.SetClassRefIcon(classId, img);
        }
        public void OnAddExternalType(UInt32 classId, Type t)
        {
            _objExplorer.OnAddExternalType(classId, t);
        }
        public void OnRemoveExternalType(UInt32 classId, Type t)
        {
            _objExplorer.OnRemoveExternalType(classId, t);
        }
        #endregion
        #region Description change handling
        bool _enterDesc;
        bool _descChanged;
        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (_enterDesc)
            {
                _descChanged = true;
            }
        }

        private void txtDesc_Leave(object sender, EventArgs e)
        {
            _enterDesc = false;
            if (_descChanged)
            {
                _descChanged = false;
                SerializeUtil.SetNodeDescription(_loader.Node, txtDesc.Text);
                _loader.NotifyChanges();
            }
        }

        private void txtDesc_Enter(object sender, EventArgs e)
        {
            _enterDesc = true;
        }
        #endregion
        #region splitter handling
        const string XMLATTR_SplitterDistance = "objectExploerHeight";
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (_loader != null && _holder != null && !_holder.IsReSizing && _holder.Loaded)
            {
                XmlNode node = _loader.Node;
                if (node != null)
                {
                    XmlUtil.SetAttribute(node, XMLATTR_SplitterDistance, this.splitContainer1.SplitterDistance);
                    _loader.NotifyChanges();
                }
            }
        }
        #endregion
    }
}
