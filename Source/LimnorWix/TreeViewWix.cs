/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VPL;
using WindowsUtility;
using XmlUtility;

namespace LimnorWix
{
	class TreeViewWix : TreeView
	{
		public const int IMG_MSI = 0;
		public const int IMG_SET = 1;
		public const int IMG_FILESYS = 2;
		public const int IMG_FOLDER = 3;
		public const int IMG_FOLDER_OPEN = 4;
		public const int IMG_TOPFILE = 5;
		public const int IMG_TOPFILE_OPEN = 6;
		public const int IMG_UI = 7;
		public const int IMG_FILES = 8;
		public const int IMG_SHORTCUT = 9;
		public const int IMG_SHORTCUTS = 10;
		public const int IMG_ICON = 11;
		public const int IMG_ICONS = 12;
		public const int IMG_LANG = 13;
		public const int IMG_LANGS = 14;
		//
		public event EventHandler SelectFile;
		public event EventHandler PropertyChanged;
		public event EventHandler FilesRemoved;
		//
		private WixXmlRoot _rootNode;
		public TreeViewWix()
		{
		}
		public void LoadData(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			LoadData(doc.DocumentElement, filename);
		}
		public bool LoadData(XmlNode rootXmlNode, string filename)
		{
			_rootNode = new WixXmlRoot(rootXmlNode, filename);
			return init();
		}
		private bool init()
		{
			bool needAdjust = _rootNode.CheckOldSchema();
			//load nodes
			this.HideSelection = false;
			Nodes.Clear();
			Nodes.Add(new TreeNodeWixGeneral(_rootNode.General));
			TreeNodeWixAppFolder na = new TreeNodeWixAppFolder(_rootNode.AppFolder);
			Nodes.Add(na);
			Nodes.Add(new TreeNodeWixIconCollection(_rootNode.Icons));
			Nodes.Add(new TreeNodeWixShortcutCollection(_rootNode.Shortcuts));
			TreeNodeWixSystemFolder nsys = new TreeNodeWixSystemFolder(_rootNode.SystemFolder);
			Nodes.Add(nsys);
			//
			TreeNodeWixCommonDataFolder ncpd = new TreeNodeWixCommonDataFolder(_rootNode.CommonDataFolder);
			Nodes.Add(ncpd);
			//
			TreeNodeWixCultureList cl = new TreeNodeWixCultureList(_rootNode.Cultures);
			Nodes.Add(cl);
			//
			return needAdjust;
		}
		public XmlNode createCultureXmlNode(string culturename, bool isUpgrade)
		{
			return _rootNode.createCultureXmlNode(culturename, isUpgrade);
		}
		public void Save()
		{
		}
		public void OnBeforeSave()
		{
			_rootNode.OnBeforeSave();
		}
		public bool ReadOnly
		{
			get
			{
				return false;
			}
		}
		public void OnFilesRemoved(IList<WixSourceFileNode> files)
		{
			if (FilesRemoved != null)
			{
				EventArgsFiles ef = new EventArgsFiles(files);
				FilesRemoved(this, ef);
			}
		}
		public void OnPropertyValueChanged()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, EventArgs.Empty);
			}
		}
		public void OnSelectFile()
		{
			if (SelectFile != null)
			{
				SelectFile(this, EventArgs.Empty);
			}
		}
		public void OnVersionChanged(string oldver, string newver)
		{
		}
		public void OnIconIdChanged(string oldIconId, string newIconId)
		{
			//
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeWixShortcutCollection tnic = Nodes[i] as TreeNodeWixShortcutCollection;
				if (tnic != null)
				{
					if (tnic.NextLevelLoaded)
					{
						foreach (TreeNode tn in tnic.Nodes)
						{
							TreeNodeWixShortcut tnwi = tn as TreeNodeWixShortcut;
							if (tnwi != null)
							{
								if (string.CompareOrdinal(oldIconId, tnwi.ShortcutNode.Icon) == 0)
								{
									tnwi.ShortcutNode.Icon = newIconId;
								}
							}
						}
					}
					break;
				}
			}
			_rootNode.OnIconIdChanged(oldIconId, newIconId);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!ReadOnly)
			{
				TreeNode nd = GetNodeAt(e.X, e.Y);
				if (nd != null)
				{
					if (e.Button == MouseButtons.Right)
					{
						ITreeNodeWithMenu tno = nd as ITreeNodeWithMenu;
						if (tno != null)
						{
							MenuItem[] mi = tno.GetContextMenuItems();
							if (mi != null)
							{
								ContextMenu cm = new ContextMenu();
								cm.MenuItems.AddRange(mi);
								cm.Show(this, new Point(e.X, e.Y));
							}
						}
					}
				}
			}
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			IWithLoader l = e.Node as IWithLoader;
			if (l != null)
			{
				if (!l.NextLevelLoaded)
				{
					List<NodeLoader> ls = new List<NodeLoader>();
					for (int i = 0; i < l.Nodes.Count; i++)
					{
						NodeLoader nl = l.Nodes[i] as NodeLoader;
						if (nl != null)
						{
							ls.Add(nl);
						}
					}
					foreach (NodeLoader nl in ls)
					{
						nl.Remove();
					}
					foreach (NodeLoader nl in ls)
					{
						l.OnLoadNextLevel(nl);
					}
				}
			}
		}
	}
	#region Loader
	abstract class NodeLoader : TreeNode
	{
	}
	#endregion
	#region intefaces
	interface IWithLoader
	{
		bool NextLevelLoaded { get; }
		TreeNodeCollection Nodes { get; }
		void OnLoadNextLevel(NodeLoader loader);
	}
	interface ITreeNodeWithMenu
	{
		MenuItem[] GetContextMenuItems();
	}
	interface ITreeNodeFolder
	{
		IList<WixSourceFileNode> Files { get; }
		void OnSelected();
	}
	class EventArgsFiles : EventArgs
	{
		private IList<WixSourceFileNode> _files;
		public EventArgsFiles(IList<WixSourceFileNode> files)
		{
			_files = files;
		}
		public IList<WixSourceFileNode> Files
		{
			get
			{
				return _files;
			}
		}
	}
	#endregion
	#region TreeNodeWix
	abstract class TreeNodeWix : TreeNode
	{
		private WixNode _wixNode;
		public TreeNodeWix(WixNode node)
		{
			Text = node.ToString();
			_wixNode = node;
		}
		public WixNode WixNode
		{
			get
			{
				return _wixNode;
			}
		}
	}
	#endregion
	#region TreeNodeWixIcon
	class TreeNodeWixIcon : TreeNodeWix, ITreeNodeWithMenu
	{
		public TreeNodeWixIcon(WixIconNode node, int idx)
			: base(node)
		{
			if (idx > 0)
				ImageIndex = idx;
			else
				ImageIndex = TreeViewWix.IMG_ICON;
			SelectedImageIndex = ImageIndex;
			node.FileChanged += node_FileChanged;
			node.IdChanged += node_IdChanged;
		}

		void node_IdChanged(object sender, EventArgs e)
		{
			Text = IconNode.ToString();
			TreeViewWix tv = this.TreeView as TreeViewWix;
			if (tv != null)
			{
				EventArgsNameChange en = e as EventArgsNameChange;
				if (en != null)
				{
					tv.OnIconIdChanged(en.OldName, en.NewName);
				}
			}
		}

		void node_FileChanged(object sender, EventArgs e)
		{
			if (this.TreeView != null)
			{
				ImageList imgs = this.TreeView.ImageList;
				if (imgs != null)
				{
					WixIconNode ico = IconNode;
					if (!string.IsNullOrEmpty(ico.SourceFile))
					{
						if (File.Exists(ico.SourceFile))
						{
							Image img = Image.FromFile(ico.SourceFile);
							imgs.Images.Add(img);
							ImageIndex = imgs.Images.Count - 1;
							SelectedImageIndex = ImageIndex;
						}
					}
				}
			}
		}
		public WixIconNode IconNode
		{
			get
			{
				return (WixIconNode)WixNode;
			}
		}
		#region ITreeNodeWithMenu
		private void mnu_removeIcon(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			if (MessageBox.Show(f, "Do you want to remove this icon from the setup?", "Remove Icon", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				TreeNodeWixIconCollection ip = this.Parent as TreeNodeWixIconCollection;
				WixIconNode ico = IconNode;
				XmlNode p = ico.XmlData.ParentNode;
				p.RemoveChild(ico.XmlData);
				ip.IconNodes.Icons.Remove(ico);
				this.Remove();
				if (tv != null)
				{
					tv.OnPropertyValueChanged();
				}
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Remove", mnu_removeIcon, Resource1._cancel.ToBitmap());
			return mns;
		}
		#endregion
	}
	#endregion
	#region TreeNodeWixIconCollection
	class TreeNodeWixIconCollection : TreeNodeWix, IWithLoader, ITreeNodeWithMenu
	{
		private bool _nextLevelLoaded;
		public TreeNodeWixIconCollection(WixIconCollection node)
			: base(node)
		{
			ImageIndex = TreeViewWix.IMG_ICONS;
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		class CLoader : NodeLoader
		{
			public CLoader()
			{
			}
		}
		public WixIconCollection IconNodes
		{
			get
			{
				return (WixIconCollection)WixNode;
			}
		}
		#region IWithLoader Members
		public bool NextLevelLoaded
		{
			get { return _nextLevelLoaded; }
		}

		public void OnLoadNextLevel(NodeLoader loader)
		{
			if (!_nextLevelLoaded)
			{
				_nextLevelLoaded = true;
				WixIconCollection icons = this.IconNodes;
				if (icons != null && icons.Icons.Count > 0)
				{
					ImageList imgs = null;
					if (this.TreeView != null)
					{
						imgs = this.TreeView.ImageList;
					}
					foreach (WixIconNode ico in icons.Icons)
					{
						int idx = -1;
						if (imgs != null)
						{
							if (!string.IsNullOrEmpty(ico.SourceFile))
							{
								if (File.Exists(ico.SourceFile))
								{
									Image img = Image.FromFile(ico.SourceFile);
									imgs.Images.Add(img);
									idx = imgs.Images.Count - 1;
								}
							}
						}
						TreeNodeWixIcon iconNode = new TreeNodeWixIcon(ico, idx);
						Nodes.Add(iconNode);
					}
				}
			}
		}
		#endregion
		#region ITreeNodeWithMenu
		private void mnu_addIcon(object sender, EventArgs e)
		{
			WixIconNode ico = this.IconNodes.AddIcon();
			if (_nextLevelLoaded)
			{
				TreeNodeWixIcon iconNode = new TreeNodeWixIcon(ico, -1);
				Nodes.Add(iconNode);
				this.TreeView.SelectedNode = iconNode;
			}
			else
			{
				this.Expand();
				if (this.TreeView != null)
				{
					for (int i = 0; i < this.Nodes.Count; i++)
					{
						TreeNodeWixIcon iconNode = this.Nodes[i] as TreeNodeWixIcon;
						if (iconNode != null)
						{
							if (string.CompareOrdinal(iconNode.IconNode.Id, ico.Id) == 0)
							{
								this.TreeView.SelectedNode = iconNode;
								break;
							}
						}
					}
				}
			}
			TreeViewWix tv = this.TreeView as TreeViewWix;
			if (tv != null)
			{
				tv.OnPropertyValueChanged();
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add Icon", mnu_addIcon,Resource1._newIcon.ToBitmap());
			return mns;
		}
		#endregion
	}
	#endregion
	#region TreeNodeWixShortcut
	class TreeNodeWixShortcut : TreeNodeWix, ITreeNodeWithMenu
	{
		public TreeNodeWixShortcut(WixShortcut node)
			: base(node)
		{
			ImageIndex = TreeViewWix.IMG_SHORTCUT;
			SelectedImageIndex = ImageIndex;
			node.IdChanged += node_IdChanged;
		}
		void node_IdChanged(object sender, EventArgs e)
		{
			Text = WixNode.ToString();
		}
		public WixShortcut ShortcutNode
		{
			get
			{
				return (WixShortcut)WixNode;
			}
		}
		#region ITreeNodeWithMenu
		private void mnu_removeShortcut(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			if (MessageBox.Show(f, "Do you want to remove this shortcut from the setup?", "Remove Shortcut", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				TreeNodeWixShortcutCollection ip = this.Parent as TreeNodeWixShortcutCollection;
				WixShortcut ico = ShortcutNode;
				XmlNode p = ico.XmlData.ParentNode;
				p.RemoveChild(ico.XmlData);
				ip.ShortcutNodes.Shortcuts.Remove(ico);
				this.Remove();
				if (tv != null)
				{
					tv.OnPropertyValueChanged();
				}
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Remove", mnu_removeShortcut,Resource1._cancel.ToBitmap());
			return mns;
		}
		#endregion
	}
	#endregion
	#region TreeNodeWixShortcutCollection
	class TreeNodeWixShortcutCollection : TreeNodeWix, IWithLoader, ITreeNodeWithMenu
	{
		private bool _nextLevelLoaded;
		public TreeNodeWixShortcutCollection(WixShortcutCollection node)
			: base(node)
		{
			ImageIndex = TreeViewWix.IMG_SHORTCUTS;
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		class CLoader : NodeLoader
		{
			public CLoader()
			{
			}
		}
		public WixShortcutCollection ShortcutNodes
		{
			get
			{
				return (WixShortcutCollection)WixNode;
			}
		}
		#region IWithLoader Members
		public bool NextLevelLoaded
		{
			get { return _nextLevelLoaded; }
		}

		public void OnLoadNextLevel(NodeLoader loader)
		{
			_nextLevelLoaded = true;
			WixShortcutCollection shortcuts = this.ShortcutNodes;
			if (shortcuts != null && shortcuts.Shortcuts.Count > 0)
			{
				foreach (WixShortcut s in shortcuts.Shortcuts)
				{
					TreeNodeWixShortcut sn = new TreeNodeWixShortcut(s);
					Nodes.Add(sn);
				}
			}
		}
		#endregion
		#region ITreeNodeWithMenu
		private void mnu_addShortcut(object sender, EventArgs e)
		{
			this.Expand();
			WixShortcut st = this.ShortcutNodes.AddShortcut();
			TreeNodeWixShortcut ts = new TreeNodeWixShortcut(st);
			Nodes.Add(ts);
			if (this.TreeView != null)
			{
				this.TreeView.SelectedNode = ts;
			}
			TreeViewWix tv = this.TreeView as TreeViewWix;
			if (tv != null)
			{
				tv.OnPropertyValueChanged();
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add Shortcut", mnu_addShortcut,Resource1._newIcon.ToBitmap());
			return mns;
		}
		#endregion
	}
	#endregion
	#region TreeNodeWixGeneral
	class TreeNodeWixGeneral : TreeNodeWix
	{
		public TreeNodeWixGeneral(WixGeneral node)
			: base(node)
		{
			ImageIndex = TreeViewWix.IMG_MSI;
			SelectedImageIndex = ImageIndex;
			General.VersionChanged += General_VersionChanged;
		}

		void General_VersionChanged(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			if (tv != null)
			{
				EventArgsNameChange en = e as EventArgsNameChange;
				if (en != null)
				{
					tv.OnVersionChanged(en.OldName, en.NewName);
				}
			}
		}
		public WixGeneral General
		{
			get
			{
				return (WixGeneral)WixNode;
			}
		}
	}
	#endregion
	#region TreeNodeWixAppFolder
	class TreeNodeWixAppFolder : TreeNodeWixFolder
	{
		public TreeNodeWixAppFolder(WixAppFolder node)
			: base(node)
		{
		}
		public WixAppFolder AppFolder
		{
			get
			{
				return (WixAppFolder)WixNode;
			}
		}
	}
	#endregion
	#region TreeNodeWixFolder
	class TreeNodeWixFolder : TreeNodeWix, IWithLoader, ITreeNodeFolder, ITreeNodeWithMenu
	{
		private bool _nextLevelLoaded;
		public event EventHandler SelectFile;
		public TreeNodeWixFolder(WixFolderNode node)
			: base(node)
		{
			ImageIndex = TreeViewWix.IMG_FOLDER;
			SelectedImageIndex = TreeViewWix.IMG_FOLDER_OPEN;
			Nodes.Add(new CLoader());
		}
		class CLoader : NodeLoader
		{
			public CLoader()
			{
			}
		}
		public WixFolderNode Folder
		{
			get
			{
				return (WixFolderNode)WixNode;
			}
		}

		#region IWithLoader Members
		public bool NextLevelLoaded
		{
			get
			{
				return _nextLevelLoaded;
			}
		}
		public virtual void OnLoadNextLevel(NodeLoader loader)
		{
			_nextLevelLoaded = true;
			WixFolderNode f = Folder;
			IList<WixFolderNode> fs = f.Folders;
			if (fs != null && fs.Count > 0)
			{
				foreach (WixFolderNode fl in fs)
				{
					TreeNodeWixFolder tn = new TreeNodeWixFolder(fl);
					tn.SelectFile += new EventHandler(tn_SelectFile);
					Nodes.Add(tn);
				}
			}
		}
		void tn_SelectFile(object sender, EventArgs e)
		{
			WixSourceFileNode f = (WixSourceFileNode)sender;
			if (SelectFile != null)
			{
				SelectFile(f, EventArgs.Empty);
			}
		}
		#endregion

		#region ITreeNodeFolder Members

		public IList<WixSourceFileNode> Files
		{
			get { return Folder.Files; }
		}
		public void OnSelected()
		{
			Folder.OnSelected();
		}
		#endregion
		#region menu handlers
		private UserControlSetupProperties getHolder()
		{
			Control c = this.TreeView;
			while (c != null)
			{
				UserControlSetupProperties u = c as UserControlSetupProperties;
				if (u != null)
					return u;
				c = c.Parent;
			}
			return null;
		}
		public void mnu_removeUncheckedFiles(object sender, EventArgs e)
		{
			WixFolderNode app = Folder;
			IList<WixSourceFileNode> unused = app.RemoveUnusedFiles();
			TreeViewWix tv = this.TreeView as TreeViewWix;
			tv.OnFilesRemoved(unused);
		}
		private bool sourceFileExists(string f)
		{
			return Folder.FileExists(f);
		}
		public void mnu_addSourceFolder(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "All files in the selected folder will be included in the setup";
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				bool b = true;
				WixSourceFileNode fn = Folder.GetWixNodeByName(dlg.SelectedPath);
				if (fn != null)
				{
					MessageBox.Show(f, "Source folder is already included", "Add Source Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
					b = false;
				}
				else
				{
					fn = Folder.AddFile(dlg.SelectedPath);
				}
				if (SelectFile != null)
				{
					SelectFile(fn, EventArgs.Empty);
				}
				if (b && tv != null)
				{
					tv.OnPropertyValueChanged();
					UserControlSetupProperties ucp = getHolder();
					if (ucp != null)
					{
						ucp.OnSelectTreeNode(this);
					}
				}
			}
		}
		public void mnu_addFiles(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Multiselect = true;
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				string[] ss = dlg.FileNames;
				if (ss != null && ss.Length > 0)
				{
					bool b = false;
					WixFolderNode app = Folder;
					for (int i = 0; i < ss.Length; i++)
					{
						WixSourceFileNode existFile = app.GetWixNodeByName(ss[i]);
						if (existFile == null)
						{
							existFile = app.AddFile(ss[i]);
							b = true;
						}
						if (SelectFile != null)
						{
							SelectFile(existFile, EventArgs.Empty);
						}
					}
					if (b && tv != null)
					{
						tv.OnPropertyValueChanged();
						UserControlSetupProperties ucp = getHolder();
						if (ucp != null)
						{
							ucp.OnSelectTreeNode(this);
						}
					}
				}
			}
		}
		private void mnu_addFolder(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			DlgFolderName dlg = new DlgFolderName();
			dlg.LoadData(Folder.Folders);
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				this.Expand();
				WixFolderNode fo = Folder.AddFolder(dlg.NewName);
				TreeNodeWixFolder tn = new TreeNodeWixFolder(fo);
				tn.SelectFile += new EventHandler(tn_SelectFile);
				Nodes.Add(tn);
				if (this.TreeView != null)
				{
					this.TreeView.SelectedNode = tn;
				}
				if (tv != null)
				{
					tv.OnPropertyValueChanged();
					UserControlSetupProperties ucp = getHolder();
					if (ucp != null)
					{
						ucp.OnSelectTreeNode(tn);
					}
				}
			}
		}
		private void mnu_addSourceProject(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			Form f = null;
			if (this.TreeView != null)
			{
				f = this.TreeView.FindForm();
			}
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Limnor Project|*.lrproj";
			dlg.Title = "Select Limnor Project File";
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				Folder.AddProject(dlg.FileName);
				if (tv != null)
				{
					tv.OnPropertyValueChanged();
				}
				UserControlSetupProperties ucp = getHolder();
				if (ucp != null)
				{
					ucp.OnSelectTreeNode(this);
				}
			}
		}
		private void mnu_delFolder(object sender, EventArgs e)
		{
			IWixFolder wf = Folder.Parent;
			if (wf != null)
			{
				TreeViewWix tv = this.TreeView as TreeViewWix;
				Form f = null;
				if (this.TreeView != null)
				{
					f = this.TreeView.FindForm();
				}
				if (MessageBox.Show(f, "Do you want to remove this folder?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					TreeNode tnp = this.Parent;
					XmlNode p = Folder.XmlData.ParentNode;
					this.Remove();
					p.RemoveChild(Folder.XmlData);
					wf.ResetSubFolders();
					if (tv != null)
					{
						tv.OnPropertyValueChanged();
					}
				}
			}
		}
		private void mnu_rename(object sender, EventArgs e)
		{
			if (this.Folder.Parent != null)
			{
				TreeViewWix tv = this.TreeView as TreeViewWix;
				Form f = null;
				if (this.TreeView != null)
				{
					f = this.TreeView.FindForm();
				}
				DlgFolderName dlg = new DlgFolderName();
				dlg.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Current folder name:{0}", this.Text);
				dlg.LoadData(Folder.Parent.Folders);
				dlg.SetName(Text);
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					Text = dlg.NewName;
					Folder.FolderName = dlg.NewName;
					if (tv != null)
					{
						tv.OnPropertyValueChanged();
					}
				}
			}
		}
		#endregion
		#region ITreeNodeWithMenu Members

		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns;
			if (Folder.Parent != null)
			{
				mns = new MenuItem[8];
			}
			else
			{
				mns = new MenuItem[5];
			}
			mns[0] = new MenuItemWithBitmap("Add source files", mnu_addFiles,Resource1._newIcon.ToBitmap());
			mns[1] = new MenuItemWithBitmap("Add source files from a folder", mnu_addSourceFolder, Resource1._newIcon.ToBitmap());
			mns[2] = new MenuItemWithBitmap("Add source project", mnu_addSourceProject, Resource1._newIcon.ToBitmap());
			mns[3] = new MenuItem("-");
			mns[4] = new MenuItemWithBitmap("Add sub folder", mnu_addFolder, Resource1._newIcon.ToBitmap());
			if (Folder.Parent != null)
			{
				mns[5] = new MenuItemWithBitmap("Rename", mnu_rename,Resource1._rename.ToBitmap());
				mns[6] = new MenuItem("-");
				mns[7] = new MenuItemWithBitmap("Remove", mnu_delFolder,Resource1._cancel.ToBitmap());
			}
			return mns;
		}

		#endregion
	}
	#endregion
	#region TreeNodeWixSystemFolder
	class TreeNodeWixSystemFolder : TreeNodeWixFolder
	{
		public TreeNodeWixSystemFolder(WixSystemFolder node)
			: base(node)
		{

		}
		public WixSystemFolder SystemFolder
		{
			get
			{
				return (WixSystemFolder)WixNode;
			}
		}
	}
	#endregion
	#region TreeNodeWixCommonDataFolder
	class TreeNodeWixCommonDataFolder : TreeNodeWixFolder
	{
		public TreeNodeWixCommonDataFolder(WixCommonAppDataFolder node)
			: base(node)
		{
		}
		public WixCommonAppDataFolder CommonDataFolder
		{
			get
			{
				return (WixCommonAppDataFolder)WixNode;
			}
		}
	}
	#endregion
	#region TreeNodeWixCulture
	class TreeNodeWixCulture : TreeNodeWix, ITreeNodeWithMenu
	{
		public TreeNodeWixCulture(WixCultureNode node, int imageIndex)
			: base(node)
		{
			if (imageIndex > 0)
				ImageIndex = imageIndex;
			else
				ImageIndex = TreeViewWix.IMG_LANG;
			SelectedImageIndex = ImageIndex;
		}
		public WixCultureNode CultureNode
		{
			get
			{
				return (WixCultureNode)WixNode;
			}
		}
		#region ITreeNodeWithMenu
		private void mnu_removeCulture(object sender, EventArgs e)
		{
			TreeNode pn = this.Parent;
			if (pn.Nodes.Count > 1)
			{
				TreeViewWix tv = this.TreeView as TreeViewWix;
				Form f = null;
				if(this.TreeView != null)
				{
					f = this.TreeView.FindForm();
				}
				if (MessageBox.Show(f, "Do you want to remove this culture from the setup?", "Remove Culture", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WixCultureNode cn = this.CultureNode;
					TreeNodeWixCultureList tcl = pn as TreeNodeWixCultureList;
					if (tcl != null)
					{
						tcl.Cultures.CultureList.Remove(cn);
					}
					XmlNode wp = cn.XmlData.ParentNode;
					wp.RemoveChild(cn.XmlData);
					this.Remove();
					if (tv != null)
					{
						tv.OnPropertyValueChanged();
					}
				}
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			if (this.Parent.Nodes.Count > 1)
			{
				MenuItem[] mns = new MenuItem[1];
				mns[0] = new MenuItemWithBitmap("Remove", mnu_removeCulture, Resource1._cancel.ToBitmap());
				return mns;
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region TreeNodeWixCultureList
	class TreeNodeWixCultureList : TreeNodeWix,IWithLoader,ITreeNodeWithMenu
	{
		private bool _nextLevelLoaded;
		public TreeNodeWixCultureList(WixCultureCollection cultures)
			: base(cultures)
		{
			ImageIndex = TreeViewWix.IMG_LANGS;
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		class CLoader : NodeLoader
		{
			public CLoader()
			{
			}
		}
		public WixCultureCollection Cultures
		{
			get
			{
				return (WixCultureCollection)WixNode;
			}
		}
		#region IWithLoader Members
		public bool NextLevelLoaded
		{
			get { return _nextLevelLoaded; }
		}

		public void OnLoadNextLevel(NodeLoader loader)
		{
			_nextLevelLoaded = true;
			WixCultureCollection cultures = this.Cultures;
			if (cultures != null && cultures.CultureList.Count > 0)
			{
				ImageList imgs = null;
				if (this.TreeView != null)
				{
					imgs = this.TreeView.ImageList;
				}
				foreach (WixCultureNode s in cultures.CultureList)
				{
					int idx = -1;
					if (imgs != null)
					{
						Image img = VPLUtil.GetLanguageImageByName(s.CultureNode.Culture);
						if (img != null)
						{
							idx = imgs.Images.Add(img, Color.White);
						}
					}
					TreeNodeWixCulture sn = new TreeNodeWixCulture(s, idx);
					Nodes.Add(sn);
				}
			}
		}
		#endregion
		#region ITreeNodeWithMenu
		private void mnu_addCulture(object sender, EventArgs e)
		{
			TreeViewWix tv = this.TreeView as TreeViewWix;
			if (tv != null)
			{
				WixCultureCollection cc = this.Cultures;
				StringCollection ss = new StringCollection();
				foreach (WixCultureNode cn in cc.CultureList)
				{
					ss.Add(cn.CultureNode.Culture);
				}
				DlgLanguages dlg = new DlgLanguages();
				dlg.SetUseSpecificCulture();
				dlg.LoadData(ss);
				Form f = null;
				if (this.TreeView != null)
				{
					f = this.TreeView.FindForm();
				}
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					ss = dlg.SelectedLanguages;
					if (ss.Count > 0)
					{
						this.Expand();
						bool added = false;
						ImageList imgs = null;
						if (this.TreeView != null)
						{
							imgs = this.TreeView.ImageList;
						}
						foreach (string s in ss)
						{
							if (!cc.CultureExists(s))
							{
								int idx = -1;
								if (imgs != null)
								{
									Image img = VPLUtil.GetLanguageImageByName(s);
									if (img != null)
									{
										idx = imgs.Images.Add(img, Color.White);
									}
								}
								XmlNode xn = tv.createCultureXmlNode(s, false);
								WixCultureNode wcn = new WixCultureNode(xn);
								cc.CultureList.Add(wcn);
								TreeNodeWixCulture twc = new TreeNodeWixCulture(wcn, idx);
								this.Nodes.Add(twc);
								added = true;
							}
						}
						if (added)
						{
							tv.OnPropertyValueChanged();
						}
					}
				}
			}
		}
		public MenuItem[] GetContextMenuItems()
		{
			MenuItem[] mns = new MenuItem[1];
			mns[0] = new MenuItemWithBitmap("Add Culture", mnu_addCulture,Resource1._newIcon.ToBitmap());
			return mns;
		}
		#endregion
	}
	#endregion
}
