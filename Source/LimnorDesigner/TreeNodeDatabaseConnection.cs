/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDatabase;
using System.Windows.Forms;
using MathExp;
using VSPrj;
using WindowsUtility;

namespace LimnorDesigner
{
	public class TreeNodeDatabaseConnectionList : TreeNodeObject
	{
		public TreeNodeDatabaseConnectionList(ClassPointer root)
			: base(false, root)
		{
			Text = "Database Connections Used";
			ImageIndex = TreeViewObjectExplorer.IMG_CONNECTS;
			SelectedImageIndex = TreeViewObjectExplorer.IMG_CONNECTS;
			this.Nodes.Add(new CLoader());
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Instance; } }
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				MenuItem[] mnus = new MenuItem[1];
				mnus[0] = new MenuItemWithBitmap("Database Connections", mnu_connections, Resources._connections.ToBitmap());
				return mnus;
			}
			return null;
		}
		private void mnu_connections(object sender, EventArgs e)
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			if (tv != null)
			{
				ClassPointer root = OwnerIdentity as ClassPointer;
				LimnorProject.SetActiveProject(root.Project);
				DlgConnectionManager dlg = new DlgConnectionManager();
				dlg.UseProjectScope = true;
				dlg.EnableCancel(false);
				dlg.ShowDialog(TreeView.FindForm());
				ResetNextLevel(tv);
			}
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
				: base(true)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				ClassPointer root = parentNode.OwnerIdentity as ClassPointer;
				IList<Guid> gl = root.GetDatabaseConnectionsUsed();
				IList<Guid> gl2 = root.Project.GetObjectGuidList();
				List<Guid> lst = new List<Guid>();
				lst.AddRange(gl);
				foreach (Guid g in gl2)
				{
					if (!lst.Contains(g))
					{
						lst.Add(g);
					}
				}
				if (lst.Count > 0)
				{
					foreach (Guid g in lst)
					{
						ConnectionItem ci = ConnectionItem.LoadConnection(g, false, false);
						if (ci != null)
						{
							parentNode.Nodes.Add(new TreeNodeDatabaseConnection(ci));
						}
					}
				}
			}
		}

		public override ProgElements.EnumPointerType NodeType
		{
			get { return ProgElements.EnumPointerType.Unknown; }
		}

		public override ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get { return ProgElements.EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(ProgElements.IObjectIdentity objectPointer)
		{
			return false;
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new CLoader());
			return l;
		}
	}
	public class TreeNodeDatabaseConnection : TreeNodeExplorer, ITreeNodeObject
	{
		private ConnectionItem _connect;
		public TreeNodeDatabaseConnection(ConnectionItem ci)
			: base(false)
		{
			_connect = ci;
			Text = ci.ToString();
			ImageIndex = TreeViewObjectExplorer.IMG_CONNECT;
			SelectedImageIndex = TreeViewObjectExplorer.IMG_CONNECT;
		}
		public ConnectionItem Connect
		{
			get
			{
				return _connect;
			}
		}
		public override IRootNode RootClassNode
		{
			get
			{
				if (TreeView != null)
				{
					for (int i = 0; i < TreeView.Nodes.Count; i++)
					{
						IRootNode r = TreeView.Nodes[i] as IRootNode;
						if (r != null)
						{
							return r;
						}
					}
				}
				return null;
			}
		}

		private void mnu_editConnect(object sender, EventArgs e)
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			if (TreeView != null)
			{
				TreeNodeObject np = null;
				ClassPointer root = null;
				TreeNode tn = this.Parent;
				while (tn != null)
				{
					np = tn as TreeNodeObject;
					if (np != null)
					{
						root = np.OwnerIdentity as ClassPointer;
						if (root != null)
						{
							break;
						}
					}
					tn = tn.Parent;
				}
				if (root != null)
				{
					LimnorProject.SetActiveProject(root.Project);
					DlgConnectionManager dlg = new DlgConnectionManager();
					dlg.UseProjectScope = true;
					dlg.EnableCancel(false);
					//set selection of _connect
					dlg.SetSelection(_connect);
					dlg.ShowDialog(TreeView.FindForm());
					np.ResetNextLevel(tv);
				}
			}
		}
		public override void OnNodeSelection(ILimnorDesignerLoader loader)
		{
			loader.NotifySelection(_connect);
		}

		#region ITreeNodeObject Members


		public bool IsInDesign
		{
			get { return true; }
		}
		public MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				MenuItem[] mnus = new MenuItem[1];
				mnus[0] = new MenuItemWithBitmap("Edit", mnu_editConnect, Resources._connections.ToBitmap());
				return mnus;
			}
			return null;
		}
		#endregion
	}
}
