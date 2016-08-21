/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSerializer;
using VSPrj;
using System.Collections.Specialized;
using System.Xml;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public class MethodDebugDesigner : ILimnorDesigner
	{
		private ClassPointer _class;
		private ObjectIDmap _map;
		private LimnorProject _prj;
		public MethodDebugDesigner(ClassPointer owner, LimnorProject project)
		{
			_class = owner;
			_map = owner.ObjectList;
			_prj = project;
		}
		#region ILimnorDesigner Members

		public IObjectPointer SelectDataType(System.Windows.Forms.Form caller)
		{
			return null;
		}

		public bool IsMethodNameUsed(UInt32 methodId, string name)
		{
			return false;
		}

		public string CreateMethodName(string baseName, StringCollection names)
		{
			return baseName + "1";
		}

		public void OnActionNameChanged(string newActionName, ulong WholeActionId)
		{
		}

		public ObjectIDmap ObjectMap
		{
			get { return _map; }
		}
		public ClassPointer GetRootId()
		{
			return _class;
		}
		public void InitToolbox()
		{
		}
		public IXmlCodeReader CodeReader
		{
			get
			{
				if (_map != null && _map.Reader != null)
					return _map.Reader;
				if (_class != null)
					return _class.CreateReader();
				return null;
			}
		}

		public IXmlCodeWriter CodeWriter
		{
			get
			{
				if (_map != null)
				{
					return new XmlObjectWriter(_map);
				}
				if (_class != null)
				{
					return _class.CreateWriter();
				}
				return null;
			}
		}
		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _prj; }
		}

		#endregion

	}
}
