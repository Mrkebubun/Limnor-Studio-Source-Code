/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MathExp;
using ProgElements;
using System.Globalization;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	class TreeNodeResourceString : TreeNodeResource
	{
		public TreeNodeResourceString(ResourcePointerString owner)
			: base(owner)
		{

			ImageIndex = TreeViewObjectExplorer.IMG_STRING;
			SelectedImageIndex = ImageIndex;
		}


		public ResourcePointerString StringPointer
		{
			get
			{
				return (ResourcePointerString)(this.OwnerPointer);
			}
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

	class TreeNodeResourceStringCollection : TreeNodeResourceCollection
	{
		public TreeNodeResourceStringCollection(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Strings";
			ImageIndex = TreeViewObjectExplorer.IMG_STRINGS;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointerString); } }
		public override void AddResource()
		{
			Expand();
			TreeNodeResourceString r = new TreeNodeResourceString(this.ResourceManager.AddNewString());
			Nodes.Add(r);
			if (TreeView != null)
			{
				TreeView.SelectedNode = r;
			}
		}

		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add string", MenuAddResource, Resources._string.ToBitmap());
			return mns;
		}
		protected override void OnLoadNextLevel()
		{
			this.LoadResourceNodes<ResourcePointerString>();
		}
	}
}
