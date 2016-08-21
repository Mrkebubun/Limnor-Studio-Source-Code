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
using System.Windows.Forms;
using XmlSerializer;
using DynamicEventLinker;
using System.CodeDom.Compiler;
using System.Reflection;
using MathExp;
using VSPrj;
using XmlUtility;
using VPL;
using System.ComponentModel.Design;

namespace LimnorDesigner
{
	/// <summary>
	/// a wrapper to a component (RootObject property).
	/// it becomes an execution unit.
	/// </summary>
	public class DebugRun : MarshalByRefObject
	{
		private ClassPointer _rootId;
		public DebugRun()
		{
		}
		public object RootObject
		{
			get
			{
				return _rootId.ObjectInstance;
			}
		}
		public virtual void Run()
		{
		}
		/// <summary>
		/// it must be called from a separated app domain other than the designer.
		/// </summary>
		/// <param name="xmlFile"></param>
		public void Load(string xmlFile)
		{
			ProjectEnvironment.RunMode = true;
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFile);
			XmlNode _node = doc.DocumentElement;
			LimnorProject project = new LimnorProject(LimnorProject.GetProjectFileByComponentFile(xmlFile));
			//create root object
			UInt32 classId = XmlUtil.GetAttributeUInt(_node, XmlTags.XMLATT_ClassID);
			UInt32 memberId = XmlUtil.GetAttributeUInt(_node, XmlTags.XMLATT_ComponentID);
			ObjectIDmap map = new ObjectIDmap(project, DesignUtil.MakeDDWord(memberId, classId), _node);
			XmlObjectReader xr = new XmlObjectReader(map, ClassPointer.OnAfterReadRootComponent, ClassPointer.OnRootComponentCreated);
			map.SetReader(xr);
			_rootId = new ClassPointer(classId, memberId, map, null);
			string file = project.GetComponentFileByClassId(classId);
			ComponentLoader loader = new ComponentLoader(xr, file);
			DesignSurface ds = new DesignSurface();
			ds.BeginLoad(loader);
			if (xr.Errors != null && xr.Errors.Count > 0)
			{
				MathNode.Log(xr.Errors);
			}
			_rootId.LoadActionInstances();
		}
	}
}
