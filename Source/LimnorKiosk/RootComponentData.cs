using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlSerializer;
using VSPrj;

namespace LimnorDesigner
{
    /// <summary>
    /// collection of data generated from a root component class
    /// </summary>
    public class RootComponentData
    {
        private XmlNode _node;
        private ObjectIDmap _objectMap;
        private RootClassId _rootClassId;
        //private List<ActionClass> _actionList;
        //private List<EventAction> _eventHandlers;
        private MultiPanes _viewsHolder;
        private LimnorProject _project;
        public RootComponentData(bool isMain,ObjectIDmap map, RootClassId rootId, XmlNode xml,LimnorProject prj)
        {
            IsMainRoot = isMain;
            _objectMap = map;
            _rootClassId = rootId;
            _node = xml;
            _project = prj;
        }
        public bool IsMainRoot { get; private set; }
        public XmlNode XmlData
        {
            get
            {
                return _node;
            }
            //set
            //{
            //    _node = value;
            //}
        }
        public ObjectIDmap ObjectMap
        {
            get
            {
                return _objectMap;
            }
            //set
            //{
            //    _objectMap = value;
            //}
        }
        public RootClassId RootClassID
        {
            get
            {
                return _rootClassId;
            }
            //set
            //{
            //    _rootClassId = value;
            //}
        }

        public List<IAction> ActionList
        {
            get
            {
                ActionEventCollection a = (ActionEventCollection)_objectMap.GetTypedData(typeof(ActionEventCollection));
                if(a != null)
                {
                    return a.Actions;
                }
                return null;//_actionList;
            }
            //set
            //{
            //    _actionList = value;
            //}
        }
        public List<EventAction> EventHandlerList
        {
            get
            {
                ActionEventCollection a = (ActionEventCollection)_objectMap.GetTypedData(typeof(ActionEventCollection));
                if (a != null)
                {
                    return a.EventHandlers;
                }
                return null;//_eventHandlers;
            }
            //set
            //{
            //    _eventHandlers = value;
            //}
        }
        public MultiPanes DesignerHolder
        {
            get
            {
                return _viewsHolder;
            }
            //set
            //{
            //    _viewsHolder = value;
            //}
        }
        public LimnorProject Project
        {
            get
            {
                if (_project == null)
                {
                    if (_objectMap != null)
                    {
                        if (!string.IsNullOrEmpty(_objectMap.DocumentMoniker))
                        {
                            _project = LimnorSolution.GetProjectByComponentFile(_objectMap.DocumentMoniker);
                        }
                    }
                }
                return _project;
            }
            //set
            //{
            //    _project = value;
            //}
        }
        public void SetDesignerHolder(MultiPanes holder)
        {
            _viewsHolder = holder;
        }
        public void ReloadActionList()
        {
            ActionEventCollection a = new ActionEventCollection(_objectMap);
            a.LoadActions();
            _objectMap.SetTypedData(typeof(ActionEventCollection), a);
        }
        //public void ReloadEventHandlerList()
        //{
        //    _eventHandlers = DesignUtil.GetEventHandlers(_node, _objectMap);
        //}
        public void ResetActionsAndHandlers()
        {
            _objectMap.RemoveTypedData(typeof(ActionEventCollection));
            //_actionList = null;
            //_eventHandlers = null;
        }
    }
}
