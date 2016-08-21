/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using XmlSerializer;
using VSPrj;

namespace LimnorDesigner
{
	public class DesignObjects
	{
		private UInt32 _classId;
		private ClassPointer _rootPointer;
		private ILimnorDesignPane _pane;
		private ObjectIDmap _map;
		public DesignObjects()
		{
		}
		public DesignObjects(UInt32 classId, ClassPointer rootPointer, ILimnorDesignPane pane, ObjectIDmap map)
		{
			_classId = classId;
			_rootPointer = rootPointer;
			_pane = pane;
			_map = map;
		}

		public static DesignObjects RemoveDesignObjects(LimnorProject project, UInt32 classId)
		{
			ClassPointer rootPointer = null;
			if (project.HasTypedData<ClassPointer>(classId))
			{
				rootPointer = project.GetTypedData<ClassPointer>(classId);
			}
			ILimnorDesignPane pane = null;
			if (project.HasTypedData<ILimnorDesignPane>(classId))
			{
				pane = project.GetTypedData<ILimnorDesignPane>(classId);
			}
			ObjectIDmap map = null;
			if (project.HasTypedData<ObjectIDmap>(classId))
			{
				map = project.GetTypedData<ObjectIDmap>(classId);
			}
			DesignObjects obj = new DesignObjects(classId, rootPointer, pane, map);
			project.RemoveTypedData<ILimnorDesignPane>(classId);
			project.RemoveTypedData<ClassPointer>(classId);
			project.RemoveTypedData<ObjectIDmap>(classId);
			return obj;
		}
		public void RestoreDesignObjects(LimnorProject project)
		{
			if (_rootPointer != null)
			{
				project.SetTypedData<ClassPointer>(_classId, _rootPointer);
			}
			else
			{
				project.RemoveTypedData<ClassPointer>(_classId);
			}
			if (_pane != null)
			{
				project.SetTypedData<ILimnorDesignPane>(_classId, _pane);
			}
			else
			{
				project.RemoveTypedData<ILimnorDesignPane>(_classId);
			}
			if (_map != null)
			{
				project.SetTypedData<ObjectIDmap>(_classId, _map);
			}
			else
			{
				project.RemoveTypedData<ObjectIDmap>(_classId);
			}
		}
	}
}
