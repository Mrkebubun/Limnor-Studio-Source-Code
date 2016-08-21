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
using ProgElements;

namespace LimnorDesigner.ResourcesManager
{
	class TreeNodeResourceCode : TreeNodeObject
	{
		public TreeNodeResourceCode(ResourcePointer owner)
			: base(new ResourceCodePointer(owner))
		{
			ImageIndex = owner.TreeNodeIconIndex;
			SelectedImageIndex = ImageIndex;
			OnShowText();
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Static; } }
		public ResourceCodePointer Pointer
		{
			get
			{
				return (ResourceCodePointer)(this.OwnerPointer);
			}
		}
		protected void OnShowText()
		{
			Text = Pointer.LongDisplayName;
		}
		public override bool IsTargeted()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			if (tv != null)
			{
				return tv.SelectionType == EnumObjectSelectType.Object;
			}
			return false;
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Unknown; }
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
			return null;
		}
	}
}
