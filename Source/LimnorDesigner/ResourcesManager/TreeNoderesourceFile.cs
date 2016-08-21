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
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	class TreeNodeResourceFile : TreeNodeResource
	{
		public TreeNodeResourceFile(ResourcePointerFile owner)
			: base(owner)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_FILE;
			SelectedImageIndex = ImageIndex;
		}
		public ResourcePointerFile FilePointer
		{
			get
			{
				return (ResourcePointerFile)(this.OwnerPointer);
			}
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Static; } }
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
			return true;
		}
	}

	class TreeNodeResourceFileCollection : TreeNodeResourceCollection
	{
		public TreeNodeResourceFileCollection(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Files";
			ImageIndex = TreeViewObjectExplorer.IMG_FILES;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointerFile); } }
		public override void AddResource()
		{
			Expand();
			TreeNodeResourceFile r = new TreeNodeResourceFile(this.ResourceManager.AddNewFile());
			Nodes.Add(r);
			if (TreeView != null)
			{
				TreeView.SelectedNode = r;
			}
		}

		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add file", MenuAddResource, Resources._file.ToBitmap());
			return mns;
		}
		protected override void OnLoadNextLevel()
		{
			this.LoadResourceNodes<ResourcePointerFile>();
		}
	}
}
