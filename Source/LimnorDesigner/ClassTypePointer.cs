/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;

namespace LimnorDesigner
{
	public class ClassTypePointer
	{
		private UInt32 _classId;
		private LimnorProject _prj;
		public ClassTypePointer()
		{
		}
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
		}
		public void SetData(UInt32 classId, LimnorProject project)
		{
			_classId = classId;
			_prj = project;
		}
		public override string ToString()
		{
			if (_classId != 0 && _prj != null)
			{
				ClassPointer cp = ClassPointer.CreateClassPointer(_classId, _prj);
				if (cp != null)
				{
					return cp.Name;
				}
			}
			return string.Empty;
		}
	}
}
