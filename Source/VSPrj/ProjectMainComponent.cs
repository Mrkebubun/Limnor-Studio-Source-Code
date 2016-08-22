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

namespace VSPrj
{
	/// <summary>
	/// use it to hold properties for the main component of a project.
	/// since it is a data holder only and there are not many types of projects,
	/// one class is used for all possible types of projects.
	/// </summary>
	public class ProjectMainComponent
	{
		public ProjectMainComponent()
		{
			ProjectType = EnumProjectType.ClassDLL;
		}
		public ProjectMainComponent(string file, Type type, EnumProjectType projectType, XmlNode node)
		{
			MainComponentNode = node;
			MainComponentFile = file;
			MainType = type;
			ProjectType = projectType;
			if (projectType != EnumProjectType.Setup)
			{
				MainClassId = XmlUtil.GetAttributeUInt(node, "ClassID");
				UpdateStartPageClassId(node);
			}
		}
		public void UpdateStartPageClassId(XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode("Property[@name='StartPage']/ObjProperty/Property[@name='MemberId']");
			if (nd != null)
			{
				if (!string.IsNullOrEmpty(nd.InnerText))
				{
					StartPageMemberId = Convert.ToUInt32(nd.InnerText);
					nd = node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture, "ClassRef[@objectID='{0}']", StartPageMemberId));
					if (nd != null)
					{
						StartPageClassId = XmlUtil.GetAttributeUInt(nd, "ClassID");
					}
				}
			}
		}
		private EnumProjectType _ptype;
		public EnumProjectType ProjectType { get { return _ptype; } private set { _ptype = value; } }
		private string _mf;
		public string MainComponentFile { get { return _mf; } private set { _mf = value; } }
		private XmlNode _mnode;
		public XmlNode MainComponentNode { get { return _mnode; } private set { _mnode = value; } }
		/// <summary>
		/// main component Id
		/// </summary>
		private UInt32 _mcid;
		public UInt32 MainClassId { get { return _mcid; } private set { _mcid = value; } }
		/// <summary>
		/// main component type
		/// </summary>
		private Type _mtype;
		public Type MainType { get { return _mtype; } private set { _mtype = value; } }
		/// <summary>
		/// for WinForm and Kiosk
		/// </summary>
		private UInt32 _scid;
		public UInt32 StartPageClassId { get { return _scid; } private set { _scid = value; } }
		private UInt32 _spmid;
		public UInt32 StartPageMemberId { get { return _spmid; } private set { _spmid = value; } }
	}
}
