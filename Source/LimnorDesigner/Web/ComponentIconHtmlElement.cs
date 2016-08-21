/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.EventMap;
using VPL;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using System.Globalization;
using MathExp;
using System.Windows.Forms;
using System.Drawing;
using Limnor.WebBuilder;
using WindowsUtility;

namespace LimnorDesigner.Web
{
	public class ComponentIconHtmlElement : ComponentIconEvent
	{
		public ComponentIconHtmlElement()
		{
		}
		public HtmlElement_Base HtmlElement
		{
			get
			{
				return (HtmlElement_Base)(this.ClassPointer);
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			string guid = XmlUtil.GetAttribute(node, XmlTags.XMLATT_guid);
			XmlObjectReader xr = reader as XmlObjectReader;
			ClassPointer root = xr.ObjectList.RootPointer as ClassPointer;
			HtmlElement_Base he = null;
			if (!string.IsNullOrEmpty(guid))
			{
				he = root.FindHtmlElementByGuid(new Guid(guid));
			}
			if (he == null)
			{
				he = new HtmlElement_body(root);
			}
			this.ClassPointer = he;
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			HtmlElement_Base he = this.ClassPointer as HtmlElement_Base;
			if (he != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_guid, he.ElementGuidString);
			}
		}
		protected override void OnSelectByMouseDown()
		{
			base.OnSelectByMouseDown();
			HtmlElement_Base heb = this.HtmlElement;
			if (heb != null)
			{
				DesignerPane.OnSelectedHtmlElement(heb.ElementGuid, this.Parent);
			}
		}
	}
	public class ComponentIconHtmlElementCurrent : ComponentIconHtmlElement
	{
		public ComponentIconHtmlElementCurrent()
		{
			base.SetLabelText("current element");
		}
		public void OnSelectHtmlElement(HtmlElement_Base he)
		{
			this.ClassPointer = he;
			OnSetImage();
			base.SetLabelText(string.Format(CultureInfo.InvariantCulture, "current element:{0}", he.ToString()));
			this.Refresh();
		}
		public override void SetLabelText(string name)
		{
			HtmlElement_Base heb = this.ClassPointer as HtmlElement_Base;
			if (heb != null)
			{
				base.SetLabelText(string.Format(CultureInfo.InvariantCulture, "current element:{0}", heb.ToString()));
			}
			else
			{
				base.SetLabelText("current element:");
			}
		}
		protected override void OnSelectByMouseDown()
		{
		}
		public override PortCollection GetAllPorts()
		{
			PortCollection pc = new PortCollection();
			return pc;
		}
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			HtmlElement_Base c = this.HtmlElement;
			if (c == null)
			{
				c = new HtmlElement_body(root);
			}
			Init(designer, c);
			return true;
		}
		public override bool IsPortOwner { get { return false; } }
		protected override bool OnBeforeUseComponent()
		{
			if (this.HtmlElement != null && (this.HtmlElement.RootPointer != null))
			{
				return this.HtmlElement.RootPointer.OnBeforeUseComponent(this.HtmlElement, this.FindForm());
			}
			return false;
		}
		private void mi_useIt(object sender, EventArgs e)
		{
			if (!(this.HtmlElement is HtmlElementUnknown) && this.HtmlElement.ElementGuid == Guid.Empty)
			{
				HtmlElement_BodyBase hbb = this.HtmlElement as HtmlElement_BodyBase;
				if (hbb != null)
				{
					ClassPointer root = this.RootClassPointer;
					if (root != null)
					{
						root.UseHtmlElement(hbb, this.FindForm());
					}
				}
			}
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			if (this.HtmlElement != null && !(this.HtmlElement is HtmlElementUnknown))
			{
				if (this.HtmlElement.ElementGuid == Guid.Empty)
				{
					if (mnu.MenuItems.Count > 0)
					{
						mnu.MenuItems.Add("-");
					}
					MenuItem mi = new MenuItemWithBitmap("Use it in programming", mi_useIt, Resources._createEventFireAction.ToBitmap());
					mnu.MenuItems.Add(mi);
				}
			}
		}
	}
}
