/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using XmlUtility;
using System.Xml;
using LimnorDesigner.MenuUtil;
using XmlSerializer;
using System.ComponentModel;
using VPL;
using LimnorDesigner.Action;
using System.Windows.Forms;
using WindowsUtility;
using System.Drawing;

namespace LimnorDesigner.MethodBuilder
{
	public class ComponentIconParameter : ComponentIconForMethod
	{
		#region fields and constructors
		public ComponentIconParameter()
			: base()
		{
		}
		public ComponentIconParameter(MethodClass method)
			: base(method)
		{
		}
		public ComponentIconParameter(ActionBranch branch)
			: base(branch)
		{
		}
		public ComponentIconParameter(ILimnorDesigner designer, ParameterClass type, MethodClass method)
			: base(designer, type, method)
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override bool IsForComponent
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override bool IsForMethodParameter
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool DoNotSaveData
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override bool ReadOnly
		{
			get
			{
				if (this.Method != null)
				{
					if (this.Method.IsOverride)
					{
						return true;
					}
				}
				return false;
			}
			set
			{
			}
		}
		public ParameterClass ParameterType
		{
			get
			{
				return (ParameterClass)ClassPointer;
			}
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			ComponentIconParameter obj = (ComponentIconParameter)base.Clone();
			obj.DoNotSaveData = DoNotSaveData;
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			if (!DoNotSaveData)
			{
				XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Var);
				if (dataNode == null)
				{
					dataNode = node.OwnerDocument.CreateElement(XmlTags.XML_Var);
					node.AppendChild(dataNode);
				}
				writer.WriteObjectToNode(dataNode, ClassPointer);
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			if (!DoNotSaveData)
			{
				XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Var);
				if (dataNode != null)
				{
					ClassPointer = (IClass)reader.ReadObject(dataNode, this);
				}
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{

			}
		}

		#endregion
		#region ComponentIcon members
		protected override void OnEstablishObjectOwnership(MethodClass owner)
		{
		}

		protected override IAction OnCreateAction(MenuItemDataMethod data, ILimnorDesignPane designPane)
		{
			IAction act = data.CreateMethodAction(designPane, ClassPointer, MethodViewer.Method, MethodViewer.ActionsHolder);
			return act;
		}

		protected override ActionClass OnCreateSetPropertyAction(MenuItemDataProperty data)
		{
			ActionClass act = data.CreateSetPropertyAction(MethodViewer.Loader.DesignPane, ClassPointer, MethodViewer.Method, MethodViewer.ActionsHolder);
			if (act != null)
			{
				act.SetScopeMethod(MethodViewer.Method);
			}
			return act;
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			if (!(this.ClassPointer is ClassPointer))
			{
				MenuItem mi;
				//
				mi = new MenuItemWithBitmap("Create Set Value Action", Resources._setVar.ToBitmap());
				mi.Click += new EventHandler(miNewInstance_Click);
				mnu.MenuItems.Add(mi);
			}
		}
		protected override LimnorContextMenuCollection GetMenuData()
		{
			return LimnorContextMenuCollection.GetMenuCollection(ClassPointer);
		}

		public override PropertyPointer CreatePropertyPointer(string propertyName)
		{
			PropertyDescriptor pd = VPLUtil.GetProperty(ParameterType.BaseClassType, propertyName);
			PropertyPointer pp = new PropertyPointer();
			pp.Owner = ClassPointer;
			pp.SetPropertyInfo(pd);
			return pp;
		}
		#region private methods
		void miNewInstance_Click(object sender, EventArgs e)
		{
			MethodDesignerHolder mv = MethodViewer;
			if (mv != null)
			{
				MethodParamVariable lv = new MethodParamVariable(ParameterType, this.Method.ClassId);
				ActionAssignInstance act = mv.CreateSetValueAction(lv);
				//
				Point p = mv.PointToClient(System.Windows.Forms.Cursor.Position);
				if (p.X < 0 || p.X > (mv.Width / 2))
					p.X = 10;
				if (p.Y < 0 || p.Y > (mv.Height / 2))
					p.Y = 10;
				ActionViewer av = mv.AddNewAction(act, p);
#if DEBUG
				if (av != null)
				{
					if (!av.Visible)
					{
						MessageBox.Show("Action Viewer not visible");
					}
				}
#endif
			}
		}
		#endregion
		#endregion
		#region Methods
		public void SetParameterType(DataTypePointer type)
		{
			ParameterType.SetDataType(type);
			RefreshLabelText();
		}
		#endregion
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			ParameterClass pc = ClassPointer as ParameterClass;
			if (pc != null)
			{
				return pc.GetProperties(attributes);
			}
			return base.GetProperties(attributes);
		}
	}
}
