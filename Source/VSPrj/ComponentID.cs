/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlUtility;
using System.IO;
using System.Globalization;
using VPL;

namespace VSPrj
{
	public class ComponentID : IComponentID
	{
		private LimnorProject _prj;
		public ComponentID(LimnorProject project)
		{
			_prj = project;
		}
		public ComponentID(LimnorProject project, UInt32 id, string name, Type type, string file)
		{
			_prj = project;
			_cid = id;
			_name = name;
			_type = type;
			_file = file;
		}
		public LimnorProject Project { get { return _prj; } }
		private UInt32 _cid;
		public UInt32 ComponentId
		{
			get
			{
				return _cid;
			}
		}
		private string _name;
		public string ComponentName { get { return _name; } }
		private string _file;
		public string ComponentFile { get { return _file; } }
		private Type _type;
		public Type ComponentType { get { return _type; } }
		public string TypeString
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _prj.Namespace, _name);
			}
		}
		public virtual void Rename(string name)
		{
			_name = name;
		}
		public virtual void SetFilename(string name)
		{
			_file = name;
		}
		public override string ToString()
		{
			return ComponentName;
		}
	}
	public class ClassData : ComponentID
	{
		private XmlDocument _doc;
		private XmlNode _nodeInProject;

		public ClassData(LimnorProject project, XmlNode node, XmlDocument doc, string file)
			: base(project, XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID), XmlUtil.GetNameAttribute(doc.DocumentElement), XmlUtil.GetLibTypeAttribute(doc.DocumentElement), file)
		{
			_nodeInProject = node;
			_doc = doc;
			Guid g = XmlUtil.GetAttributeGuid(_doc.DocumentElement, XmlTags.XMLATT_guid);
			if (g == Guid.Empty)
			{
				g = Guid.NewGuid();
				XmlUtil.SetAttribute(_doc.DocumentElement, XmlTags.XMLATT_guid, g);
				_doc.Save(file);
			}
		}
		public XmlNode ComponentXmlNode
		{
			get
			{
				return _doc.DocumentElement;
			}
		}
		public bool IsSetup
		{
			get
			{
				return XmlUtil.GetAttributeBoolDefFalse(ComponentXmlNode, XmlTags.XMLATT_isSetup);
			}
		}
		public XmlNode NodeInProject
		{
			get
			{
				return _nodeInProject;
			}
		}
		public void Save()
		{
			_doc.Save(ComponentFile);
		}
		public void ReloadXmlFile()
		{
			_doc = new XmlDocument();
			_doc.Load(ComponentFile);
		}
		public override void Rename(string name)
		{
			base.Rename(name);
			XmlUtil.SetNameAttribute(_doc.DocumentElement, name);
		}
		public override void SetFilename(string name)
		{
			base.SetFilename(name);
			XmlUtil.SetAttribute(_nodeInProject, LimnorProject.XMLATT_Include, Path.GetFileName(name));
		}
	}
}
