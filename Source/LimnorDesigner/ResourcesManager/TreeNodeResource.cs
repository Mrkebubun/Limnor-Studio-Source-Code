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
using System.Globalization;
using System.Collections;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	abstract class TreeNodeResource : TreeNodeObject
	{
		public TreeNodeResource(ResourcePointer owner)
			: base(owner)
		{
			OnShowText();
			Nodes.Add(new CLoad());
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Static; } }
		public void OnSelected(TextBoxResEditor textBoxDefault, TextBoxResEditor textBoxLocal, PictureBoxResEditor pictureBoxDefault, PictureBoxResEditor pictureBoxLocal, CultureInfo c)
		{
			Pointer.OnSelected(textBoxDefault, textBoxLocal, pictureBoxDefault, pictureBoxLocal, c);
		}
		public void OnLoadNextLevel()
		{
			IList<string> languages = Manager.Languages;
			foreach (string s in languages)
			{
				if (string.CompareOrdinal(s, "zh") == 0)
				{
					TreeNodeLocalizeResource tn = new TreeNodeLocalizeResource(Pointer, s);
					Nodes.Add(tn);
				}
				else
				{
					CultureInfo c = CultureInfo.GetCultureInfo(s);
					if (c != null)
					{
						TreeNodeLocalizeResource tn = new TreeNodeLocalizeResource(Pointer, c);
						Nodes.Add(tn);
					}
				}
			}
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
		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new CLoad());
			return l;
		}
		private void mn_remove(object sender, EventArgs e)
		{
			if (this.TreeView != null)
			{
				if (MessageBox.Show(this.TreeView.FindForm(), "Do not want to remove this resource?", "Remove resource", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					ResourcePointer rp = (ResourcePointer)(this.OwnerPointer);
					rp.Manager.RemoveResource(rp.MemberId);
					Remove();
					rp.IsChanged = true;
				}
			}

		}
		private void onNameChanged(object sender, NameBeforeChangeEventArg e)
		{
			if (Manager.IsNameInUse(e.NewName))
			{
				e.Cancel = true;
				e.Message = "The name is in use";
			}
			else if (e.IsIdentifierName)
			{
				if (!VOB.VobUtil.IsGoodVarName(e.NewName))
				{
					e.Cancel = true;
					e.Message = "The name is invalid";
				}
			}
		}
		private void mn_rename(object sender, EventArgs e)
		{
			if (this.TreeView != null)
			{
				DlgRename dlg = new DlgRename();
				dlg.LoadData(Pointer.Name, onNameChanged, true);
				if (dlg.ShowDialog(this.TreeView.FindForm()) == DialogResult.OK)
				{
					Pointer.UpdateResourceName(dlg.NewName);
					OnShowText();
				}
			}
		}
		protected void OnShowText()
		{
			Text = Pointer.LongDisplayName;
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			MenuItem[] mns = new MenuItem[2];
			mns[0] = new MenuItemWithBitmap("Remove", mn_remove, Resources._cancel.ToBitmap());
			mns[1] = new MenuItemWithBitmap("Rename", mn_rename, Resources._rename.ToBitmap());
			return mns;
		}
		public ResourcePointer Pointer
		{
			get
			{
				return (ResourcePointer)(this.OwnerPointer);
			}
		}
		public ProjectResources Manager
		{
			get
			{
				return Pointer.Manager;
			}
		}

		class CLoad : TreeNodeLoader
		{
			public CLoad()
				: base(false)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				TreeNodeResource tc = (TreeNodeResource)parentNode;
				tc.OnLoadNextLevel();
			}
		}
	}

	abstract class TreeNodeResourceCollection : TreeNodeObject
	{
		private bool _forSelection;
		public TreeNodeResourceCollection(ProjectResources owner, bool isForSelection)
			: base(owner)
		{
			_forSelection = isForSelection;
			Nodes.Add(new CLoad());
		}
		public static void AddResourceCollections(ProjectResources manager, TreeNodeCollection parent, bool forSelection)
		{
			parent.Add(new TreeNodeResourceStringCollection(manager, forSelection));
			parent.Add(new TreeNodeResourceImageCollection(manager, forSelection));
			parent.Add(new TreeNodeResourceIconCollection(manager, forSelection));
			parent.Add(new TreeNodeResourceAudioCollection(manager, forSelection));
			parent.Add(new TreeNodeResourceFileCollection(manager, forSelection));
			parent.Add(new TreeNodeResourceFilePathCollection(manager, forSelection));

		}
		protected abstract Type PointerType { get; }
		public abstract void AddResource();

		protected void MenuAddResource(object sender, EventArgs e)
		{
			AddResource();
		}
		public bool IsForSelection
		{
			get
			{
				return _forSelection;
			}
		}
		public ProjectResources ResourceManager
		{
			get
			{
				return (ProjectResources)(this.OwnerIdentity);
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
		protected abstract void OnLoadNextLevel();

		public void LoadResourceNodes<T>() where T : ResourcePointer
		{
			ProjectResources rm = this.ResourceManager;
			IList<T> rs = rm.GetResourceList<T>();
			SortedList<string, TreeNode> s = new SortedList<string, TreeNode>();
			foreach (T r in rs)
			{
				if (IsForSelection)
				{
					TreeNodeResourceCode tn = new TreeNodeResourceCode(r);
					s.Add(tn.Text, tn);
				}
				else
				{
					TreeNodeResource tn = r.CreateTreeNode();
					s.Add(tn.Text, tn);
				}
			}
			IEnumerator<KeyValuePair<string, TreeNode>> en = s.GetEnumerator();
			while (en.MoveNext())
			{
				Nodes.Add(en.Current.Value);
			}
		}

		class CLoad : TreeNodeLoader
		{
			public CLoad()
				: base(false)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				TreeNodeResourceCollection tc = (TreeNodeResourceCollection)parentNode;
				tc.OnLoadNextLevel();
			}
		}
	}

	class TreeNodeResourceManager : TreeNodeResourceCollection
	{
		public TreeNodeResourceManager(ProjectResources owner, bool isForSelection)
			: base(owner, isForSelection)
		{
			Text = "Resources";
			ImageIndex = TreeViewObjectExplorer.IMG_RESMAN;
			SelectedImageIndex = ImageIndex;
		}
		protected override Type PointerType { get { return typeof(ResourcePointer); } }
		public override void AddResource()
		{
		}
		public void SelectLanguages()
		{
			Form f = this.TreeView.FindForm();
			if (ResourceManager.SelectLanguages(f))
			{
				if (NextLevelLoaded)
				{
					TreeNodeCultureCollection tnc = GetCulturesNode();
					if (tnc != null)
					{
						tnc.LoadLanguages();
					}
					ResetResourcesCollectionNodes();
				}
			}
		}
		public TreeNodeCultureCollection GetCulturesNode()
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeCultureCollection tn = Nodes[i] as TreeNodeCultureCollection;
				if (tn != null)
				{
					return tn;
				}
			}
			return null;
		}
		public T GetCollectionNode<T>() where T : TreeNodeResourceCollection
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				T tn = Nodes[i] as T;
				if (tn != null)
				{
					return tn;
				}
			}
			return default(T);
		}
		public void ResetResourcesCollectionNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeResourceCollection tnc = Nodes[i] as TreeNodeResourceCollection;
				if (tnc != null)
				{
					tnc.ResetNextLevel(tv);
				}
			}
		}
		protected override void OnLoadNextLevel()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			if (IsForSelection)
			{
				Nodes.Add(new TreeNodePropertyCollection(tv, this, true, ResourceManager, 0));
			}
			else
			{
				Nodes.Add(new TreeNodeCultureCollection(ResourceManager));
			}
			AddResourceCollections(this.ResourceManager, Nodes, IsForSelection);
		}
	}
}
