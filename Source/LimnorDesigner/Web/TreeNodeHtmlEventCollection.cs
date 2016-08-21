/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using Limnor.WebBuilder;
using VPL;

namespace LimnorDesigner.Web
{
	public class TreeNodeHtmlEventCollection : TreeNodeEventCollection
	{
		public TreeNodeHtmlEventCollection(TreeViewObjectExplorer tv, TreeNodeObject parentNode, HtmlElement_Base objectPointer, UInt32 scopeMethodId)
			: base(tv, parentNode, false, objectPointer, scopeMethodId)
		{
			Text = "Events inherited";
		}
		protected override void ShowIconNoAction()
		{
			ImageIndex = TreeViewObjectExplorer.IMG_EVENTS;
			SelectedImageIndex = ImageIndex;
		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Event; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return !(objectPointer is ClassPointer);
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> lst = new List<TreeNodeLoader>();
			lst.Add(new EventsLoader(false));
			return lst;
		}
		public override void OnShowActionIcon()
		{
			base.OnShowActionIcon();
			this.ImageIndex = TreeViewObjectExplorer.IMG_A_Events;
			SelectedImageIndex = this.ImageIndex;
		}
		public override bool IsPointerNode { get { return false; } }
	}
}
