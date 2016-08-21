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
	class TreeNodeResourceIcon : TreeNodeResource
	{
		public TreeNodeResourceIcon(ResourcePointerIcon owner)
			: base(owner)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_ICON;
			SelectedImageIndex = ImageIndex;
		}
		public ResourcePointerIcon IconPointer
		{
			get
			{
				return (ResourcePointerIcon)(this.OwnerPointer);
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

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new CLoad());
			return l;
		}

		class CLoad : TreeNodeLoader
		{
			public CLoad()
				: base(false)
			{
			}
			//load languages
			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{

			}
		}
	}

	class TreeNodeResourceIconCollection : TreeNodeResourceCollection
	{
		public TreeNodeResourceIconCollection(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Icons";
			ImageIndex = TreeViewObjectExplorer.IMG_ICONS;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointerIcon); } }
		public override void AddResource()
		{
			Expand();
			TreeNodeResourceIcon r = new TreeNodeResourceIcon(this.ResourceManager.AddNewIcon());
			Nodes.Add(r);
			if (TreeView != null)
			{
				TreeView.SelectedNode = r;
			}
		}

		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add icon", MenuAddResource, Resources._icon.ToBitmap());
			return mns;
		}
		protected override void OnLoadNextLevel()
		{
			this.LoadResourceNodes<ResourcePointerIcon>();
		}
	}

}
