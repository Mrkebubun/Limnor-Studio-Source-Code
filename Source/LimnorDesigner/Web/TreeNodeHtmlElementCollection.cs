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
using Limnor.WebBuilder;

namespace LimnorDesigner.Web
{
	class TreeNodeHtmlElementCollection : TreeNodeObjectCollection
	{
		public TreeNodeHtmlElementCollection(TreeViewObjectExplorer tv, TreeNodeObject parentNode, ClassPointer objectPointer, UInt32 scopeMethodId)
			: base(tv, parentNode, false, objectPointer, scopeMethodId)
		{
			Text = "Html Document";
			ImageIndex = TreeNodeHtmlElement.GetHtmlElementIconbyKey("html");
			SelectedImageIndex = ImageIndex;
			List<TreeNodeLoader> lst = GetLoaderNodes();
			foreach (TreeNodeLoader tnl in lst)
			{
				Nodes.Add(tnl);
			}
		}
		public virtual IList<HtmlElement_BodyBase> UsedHtmlElements
		{
			get
			{
				ClassPointer cp = (ClassPointer)(this.OwnerPointer);
				return cp.UsedHtmlElements;
			}
		}
		protected override void ShowIconNoAction()
		{

		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Class; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return false;
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new TreeNodeHtmlElementLoader());
			return l;
		}
		class TreeNodeHtmlElementLoader : TreeNodeLoader
		{
			public TreeNodeHtmlElementLoader()
				: base(false)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				if (tv.RootClassNode != null && tv.RootClassNode.RootObjectId != null)
				{
					parentNode.Nodes.Add(new TreeNodeHtmlElementCurrent(new HtmlElementUnknown(tv.RootClassNode.RootObjectId)));
				}
				TreeNodeHtmlElementCollection tnhec = parentNode as TreeNodeHtmlElementCollection;
				IList<HtmlElement_BodyBase> hl = tnhec.UsedHtmlElements;
				if (hl != null && hl.Count > 0)
				{
					for (int i = 0; i < hl.Count; i++)
					{
						parentNode.Nodes.Add(new TreeNodeHtmlElement(hl[i]));
					}
				}
			}
		}
	}

	class TreeNodeHtmlMemberLoader : TreeNodeLoader
	{
		public TreeNodeHtmlMemberLoader()
			: base(false)
		{
		}

		public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
		{
			UInt32 scopeId = parentNode.ScopeMethodId;
			HtmlElement_Base objRef = (HtmlElement_Base)parentNode.OwnerIdentity;
			parentNode.Nodes.Add(new TreeNodeHtmlPropertyCollection(tv, parentNode, objRef, scopeId));
			if (parentNode.SelectionTarget == EnumObjectSelectType.All || parentNode.SelectionTarget == EnumObjectSelectType.Event || !string.IsNullOrEmpty(parentNode.EventScope))
			{
				parentNode.Nodes.Add(new TreeNodeHtmlEventCollection(tv, parentNode, objRef, scopeId));
			}
			if (parentNode.SelectionTarget == EnumObjectSelectType.All || parentNode.SelectionTarget == EnumObjectSelectType.Method || parentNode.SelectionTarget == EnumObjectSelectType.Action || parentNode.SelectionTarget == EnumObjectSelectType.Object)
			{
				parentNode.Nodes.Add(new TreeNodeHtmlMethodCollection(tv, parentNode, objRef, scopeId));
			}
		}
	}
}
