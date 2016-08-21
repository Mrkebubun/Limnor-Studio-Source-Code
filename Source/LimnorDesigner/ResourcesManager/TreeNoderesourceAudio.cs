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
	class TreeNodeResourceAudio : TreeNodeResource
	{
		public TreeNodeResourceAudio(ResourcePointerAudio owner)
			: base(owner)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_AUDIO;
			SelectedImageIndex = ImageIndex;
		}
		public ResourcePointerAudio AudioPointer
		{
			get
			{
				return (ResourcePointerAudio)(this.OwnerPointer);
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

	class TreeNodeResourceAudioCollection : TreeNodeResourceCollection
	{
		public TreeNodeResourceAudioCollection(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Audios";
			ImageIndex = TreeViewObjectExplorer.IMG_AUDIOS;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointerAudio); } }
		public override void AddResource()
		{
			Expand();
			TreeNodeResourceAudio r = new TreeNodeResourceAudio(this.ResourceManager.AddNewAudio());
			Nodes.Add(r);
			if (TreeView != null)
			{
				TreeView.SelectedNode = r;
			}
		}

		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add audio", MenuAddResource, Resources.audio.ToBitmap());
			return mns;
		}
		protected override void OnLoadNextLevel()
		{
			this.LoadResourceNodes<ResourcePointerAudio>();
		}
	}

}
