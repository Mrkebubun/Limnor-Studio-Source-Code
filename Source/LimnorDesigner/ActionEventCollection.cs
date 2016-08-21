using System;
using System.Collections.Generic;
using System.Text;
using SerializeInterface;
using System.ComponentModel;
using XmlSerializer;
using DynamicEventLinker;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;
using MathExp;
using System.Collections.Specialized;
using System.Xml;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Event;

namespace LimnorDesigner
{
    /// <summary>
    /// programming entities
    /// </summary>
    public class ActionEventCollection : ISerializeAsObject
    {
        /// <summary>
        /// all actions belonging to a root class
        /// </summary>
        private List<IAction> actions;
        /// <summary>
        /// event-action links
        /// </summary>
        private List<EventAction> eventHandlers;
        private ObjectIDmap _map;
        public ActionEventCollection(ObjectIDmap map)
        {
            _map = map;
        }
        public List<IAction> Actions
        {
            get
            {
                return actions;
            }
        }
        //public List<ActionGroup> ActionGroups
        //{
        //    get
        //    {
        //        return actionGroups;
        //    }
        //}
        public List<EventAction> EventHandlers
        {
            get
            {
                return eventHandlers;
            }
        }
        [Browsable(false)]
        public IAction GetAction(TaskID taskId)
        {
            if (taskId.ClassId != _map.ClassId)
            {
                //XmlDocument doc = _map.Project.GetDocumentByClassId(taskId.ClassId, true);
                throw new DesignerException("calling GetAction([{0}]) from class {1}", taskId, _map.ClassId);
            }
            else
            {
                if (actions == null)
                {
                    LoadActions();
                }
                foreach (IAction a in actions)
                {
                    if (a.WholeActionId == taskId.WholeTaskId)
                    {
                        return a;
                    }
                }
            }
            return null;
        }
        [Browsable(false)]
        public void LoadActions()
        {
            actions = DesignUtil.GetActions(_map);
            eventHandlers = DesignUtil.GetEventHandlers(_map);
            if (ProjectEnvironment.RunMode )//&& _map.ParanetMap == null)
            {
                //if an action is for executing a custom method then the actions inside the method
                //only holds actions ID's. We need associate actions objects to those action ID's
                //by going through all action branches
                foreach (IAction a in actions)
                {
                    //ActionCustom ac = a as ActionCustom;
                    //if (ac != null)
                    //{
                    //    ac.SetActions(actions);
                    //}
                    CustomMethodPointer cmp = a.ActionMethod as CustomMethodPointer;
                    if (cmp != null)
                    {
                        cmp.MethodDef.SetActions(actions);
                    }
                }
                //link events to actions
                foreach (EventAction ea in eventHandlers)
                {
                    if (ea.Event.ObjectInstance != null)
                    {
                        SetEventLink(ea.Event);
                    }
                }
            }
        }
        public void SetEventLink(IEvent e)
        {
            IEventPointer ep = e as IEventPointer;
            if (ep != null)
            {
                CompilerErrorCollection errors = DynamicLink.LinkEvent(ep, taskExecuter);
                if (errors != null && errors.Count > 0)
                {
                    StringCollection sc = new StringCollection();
                    for (int i = 0; i < errors.Count; i++)
                    {
                        sc.Add(errors[i].ErrorText);
                    }
                    MathNode.Log(sc);
                }
            }
        }
        public void SaveEventBreakPointsToXml(string eventName, string objKey)
        {
            if (eventHandlers == null)
            {
                eventHandlers = DesignUtil.GetEventHandlers(_map);
            }
            foreach (EventAction ea in eventHandlers)
            {
                if (ea.Event.Name == eventName)
                {
                    if (ea.Event.Owner.ObjectKey == objKey)
                    {
                        ea.SaveEventBreakPointsToXml();
                        break;
                    }
                }
            }
        }
        public EventAction GetEventHandler(string eventName, string objKey)
        {
            if (eventHandlers == null)
            {
                eventHandlers = DesignUtil.GetEventHandlers(_map);
            }
            foreach (EventAction ea in eventHandlers)
            {
                if (ea.Event.Name == eventName)
                {
                    if (ea.Event.Owner.ObjectKey == objKey)
                    {
                        return ea;
                    }
                }
            }
            return null;
        }
        private void taskExecuter(IEventPointer eventPointer, object[] eventParameters)
        {
            try
            {
                foreach (EventAction ea in eventHandlers)
                {
                    EventPointer ep = eventPointer as EventPointer;
                    if (ea.Event.IsSameObjectRef(ep))
                    {
                        List<ParameterClass> eventValues = new List<ParameterClass>();
                        if (eventParameters != null && eventParameters.Length > 0)
                        {
                            ParameterInfo[] pifs = ep.Parameters;
                            if (pifs.Length != eventParameters.Length)
                            {
                                throw new DesignerException("Event {0} parameter count mismatch", ep.MemberName);
                            }
                            for (int i = 0; i < pifs.Length; i++)
                            {
                                ParameterClass p = new ParameterClass(new TypePointer(pifs[i].ParameterType));
                                p.Name = pifs[i].Name;
                                p.ObjectInstance = eventParameters[i];
                                eventValues.Add(p);
                            }
                        }
                        //execute the event handlers
                        TaskIdList actIdList = ea.TaskIDList;
                        foreach (TaskID tid in actIdList)
                        {
                            UInt32 classId = tid.ClassId;
                            //if (tid.IsGroup)
                            //{
                            //}
                            //else
                            //{
                            //find the action in the list of all actions
                            List<IAction> acts = null;
                            if (classId == _map.ClassId)
                            {
                                acts = actions;
                            }
                            else
                            {
                                ObjectIDmap classmap = _map.GetMapByClassId(classId);
                                if (classmap != null)
                                {
                                    ActionEventCollection av = classmap.GetTypedData<ActionEventCollection>();
                                    if (av == null)
                                    {
                                        av = new ActionEventCollection(classmap);
                                        classmap.SetTypedData<ActionEventCollection>(av);
                                        av.LoadActions();
                                    }
                                    acts = av.actions;
                                }
                            }
                            if (acts != null)
                            {
                                foreach (IAction a in acts)
                                {
                                    if (a.ActionId == tid.ActionId)
                                    {
                                        a.Execute(eventValues);
                                        break;
                                    }
                                }
                            }
                            //}
                        }
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                MathNode.Log(err);
            }
        }
        #region ISerializeAsObject Members

        public bool NeedSerializeAsObject
        {
            get { return false; }
        }

        #endregion
    }
}
