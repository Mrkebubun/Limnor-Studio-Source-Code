/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
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
		private ClassPointer _rootClassId;
		public RootComponentData(bool isMain, ClassPointer rootId)
		{
			IsMainRoot = isMain;
			_rootClassId = rootId;
		}
		public bool IsMainRoot { get; private set; }
		public XmlNode XmlData
		{
			get
			{
				return _rootClassId.XmlData;
			}
		}
		public ObjectIDmap ObjectMap
		{
			get
			{
				return _rootClassId.ObjectList;
			}
		}
		public ClassPointer RootClassID
		{
			get
			{
				return _rootClassId;
			}
		}

		public Dictionary<UInt32, IAction> ActionList
		{
			get
			{
				return _rootClassId.GetActions();
			}
		}
		public List<EventAction> EventHandlerList
		{
			get
			{
				return _rootClassId.EventHandlers;
			}
		}
		public MultiPanes DesignerHolder
		{
			get
			{
				ILimnorDesignPane dpane = _rootClassId.Project.GetTypedData<ILimnorDesignPane>(_rootClassId.ClassId);
				if (dpane != null)
				{
					return dpane.PaneHolder;
				}
				return null;
			}
		}
		public LimnorProject Project
		{
			get
			{
				return _rootClassId.Project;
			}
		}
		public void ResetPointer(ClassPointer pointer)
		{
			_rootClassId = pointer;
		}
		public void ReloadActionList()
		{
			_rootClassId.LoadActionInstances();
		}
	}
}
