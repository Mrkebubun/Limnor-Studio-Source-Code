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
using XmlSerializer;
using System.Xml;
using XmlUtility;
using LimnorDesigner.MenuUtil;
using VPL;

namespace LimnorDesigner.EventMap
{
	/// <summary>
	/// represents a TypePointer
	/// </summary>
	public class ComponentIconClassType : ComponentIconEvent
	{
		#region fields and constructors
		private LimnorProject _prj;
		private ClassPointer _root;

		public ComponentIconClassType()
		{
			OnSetImage();
		}
		#endregion

		#region Properties
		public override LimnorProject Project
		{
			get
			{
				return _prj;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				return _root;
			}
		}
		public Type ClassType
		{
			get
			{
				DataTypePointer tp = this.ClassPointer as DataTypePointer;
				if (tp != null)
				{
					return tp.BaseClassType;
				}
				return null;
			}
		}
		/// <summary>
		/// not for creating context menu
		/// </summary>
		public override bool IsForComponent
		{
			get
			{
				return false;
			}
		}

		public override ClassPointer RootClassPointer
		{
			get
			{
				return _root;
			}
		}
		protected override bool IsClassType
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Methods
		const string XML_Type = "Type";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			Type t = this.ClassType;
			if (t != null)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_Type);
				XmlUtil.SetLibTypeAttribute(nd, t);
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			base.OnReadFromXmlNode(reader0, node);
			XmlObjectReader reader = (XmlObjectReader)reader0;
			_prj = reader.ObjectList.Project;
			_root = reader.ObjectList.RootPointer as ClassPointer;
			XmlNode nd = node.SelectSingleNode(XML_Type);
			if (nd != null)
			{
				Type t = XmlUtil.GetLibTypeAttribute(nd);
				if (t != null)
				{
					this.ClassPointer = new DataTypePointer(new TypePointer(t));
					SetLabelText(t.Name);
					OnSetImage();
				}
			}
		}
		protected override void OnInit(ILimnorDesigner designer, IClass pointer)
		{
			_prj = designer.Project;
			_root = designer.GetRootId();
			DataTypePointer cp = pointer as DataTypePointer;
			if (cp != null)
			{
				this.ClassPointer = cp;
				SetLabelText(cp.Name);
				OnSetImage();
			}
		}
		protected override void OnSetImage()
		{
			DataTypePointer cp = this.ClassPointer as DataTypePointer;
			if (cp != null)
			{
				if (cp.ImageIcon != null)
				{
					SetIconImage(cp.ImageIcon);
				}
			}
		}
		protected override LimnorContextMenuCollection CreateMenuData()
		{
			return LimnorContextMenuCollection.GetStaticMenuCollection(this.ClassPointer);
		}
		public override bool IsForThePointer(IObjectPointer pointer)
		{
			DataTypePointer cp = pointer as DataTypePointer;
			if (cp != null && cp.BaseClassType != null)
			{
				return cp.BaseClassType.Equals(this.ClassType);
			}
			return false;
		}
		public override bool IsActionExecuter(IAction act, ClassPointer root)
		{
			if (act.IsStatic)
			{
				DataTypePointer cp = act.ActionMethod.Owner as DataTypePointer;
				if (cp != null && cp.BaseClassType != null)
				{
					return cp.BaseClassType.Equals(this.ClassType);
				}
			}
			return false;
		}
		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			_prj = objMap.Project;
			_root = objMap.RootPointer as ClassPointer;
			Type cp = this.ClassType;
			if (cp != null)
			{
				SetLabelText(cp.Name);
			}
		}
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			_prj = root.Project;
			_root = root;
			Type cp = this.ClassType;
			if (cp != null)
			{
				SetLabelText(cp.Name);
				return true;
			}
			return false;
		}
		#endregion
	}
}
