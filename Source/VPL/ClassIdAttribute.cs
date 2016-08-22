/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class ClassIdAttribute : Attribute
	{
		private UInt32 _classId;
		private Guid _projectGuid;
		public ClassIdAttribute(UInt32 classId, string projectGuid)
		{
			_classId = classId;
			_projectGuid = new Guid(projectGuid);
		}
		public UInt32 ClassID
		{
			get
			{
				return _classId;
			}
		}
		public Guid ProjectGuid
		{
			get
			{
				return _projectGuid;
			}
		}
	}
}
