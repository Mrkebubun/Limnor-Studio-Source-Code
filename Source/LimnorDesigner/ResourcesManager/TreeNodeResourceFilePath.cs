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
using MathExp;
using System.Windows.Forms;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	class TreeNodeResourceFilePath : TreeNodeResource
	{
		public TreeNodeResourceFilePath(ResourcePointerFilePath owner)
			: base(owner)
		{

			ImageIndex = TreeViewObjectExplorer.IMG_STRING;
			SelectedImageIndex = ImageIndex;
		}


		public ResourcePointerFilePath FilePathPointer
		{
			get
			{
				return (ResourcePointerFilePath)(this.OwnerPointer);
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
	class TreeNodeResourceFilePathCollection : TreeNodeResourceCollection
	{
		public TreeNodeResourceFilePathCollection(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Filenames";
			ImageIndex = TreeViewObjectExplorer.IMG_STRINGS;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointerFilePath); } }
		public override void AddResource()
		{
			Expand();
			TreeNodeResourceFilePath r = new TreeNodeResourceFilePath(this.ResourceManager.AddNewFilePath());
			Nodes.Add(r);
			if (TreeView != null)
			{
				TreeView.SelectedNode = r;
			}
		}

		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add filename", MenuAddResource, Resources._string.ToBitmap());
			return mns;
		}
		protected override void OnLoadNextLevel()
		{
			this.LoadResourceNodes<ResourcePointerFilePath>();
		}
	}
}
