/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using VPL;
using System.Xml;

namespace LimnorDesigner.MethodBuilder
{
	public class ComponentIconMethodReturnPointer : ComponentIconLocal
	{
		#region fields and constructors
		public ComponentIconMethodReturnPointer()
		{
			SetIconImage(Resources._methodReturn.ToBitmap());
		}
		public ComponentIconMethodReturnPointer(MethodClass method)
			: base(method)
		{
			SetIconImage(Resources._methodReturn.ToBitmap());
		}
		public ComponentIconMethodReturnPointer(ActionBranch branch)
			: base(branch)
		{
			SetIconImage(Resources._methodReturn.ToBitmap());
		}
		public ComponentIconMethodReturnPointer(ILimnorDesigner designer, CustomMethodReturnPointer pointer, MethodClass method)
			: base(designer, pointer, method)
		{
			SetNameType(pointer.ClassType, pointer.Name);
			SetIconImage(Resources._methodReturn.ToBitmap());
		}
		#endregion
		public override string ToString()
		{
			return this.GetVariableName();
		}
		protected override void OnWrite(IXmlCodeWriter writer, XmlNode dataNode)
		{
			CustomMethodReturnPointer v = this.ClassPointer as CustomMethodReturnPointer;
			if (v != null)
			{
				v.OnWriteToXmlNode(writer, dataNode);
			}
		}
		protected override void OnRead(IXmlCodeReader reader, XmlNode dataNode)
		{
			CustomMethodReturnPointer v = new CustomMethodReturnPointer();
			this.ClassPointer = v;
			v.OnReadFromXmlNode(reader, dataNode);
			v.SetName(this.GetVariableName());
		}
	}
}
