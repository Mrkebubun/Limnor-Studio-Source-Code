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
using System.Globalization;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	class TreeNodeCulture : TreeNode, ITreeNodeObject
	{
		private ProjectResources _resman;
		private CultureInfo _culture;
		private string _name;
		public TreeNodeCulture(CultureInfo c, ProjectResources rm)
		{
			_culture = c;
			_resman = rm;
			Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0} [{1}]", c.NativeName, c.Name);
			if (string.CompareOrdinal(_culture.Name, _resman.DesignerLanguageName) == 0)
			{
				ImageIndex = TreeViewObjectExplorer.GetSelectedImageIndex(TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name));
			}
			else
			{
				ImageIndex = TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name);
			}
			SelectedImageIndex = ImageIndex;
		}
		public TreeNodeCulture(string nm, ProjectResources rm)
		{
			_name = nm;
			_resman = rm;
			if (string.CompareOrdinal("zh", nm) == 0)
			{
				_culture = CultureInfo.GetCultureInfo("zh-CHT");
				Text = "中文 zh";
				if (string.CompareOrdinal(_culture.Name, _resman.DesignerLanguageName) == 0)
				{
					ImageIndex = TreeViewObjectExplorer.GetSelectedImageIndex(TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name));
				}
				else
				{
					ImageIndex = TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name);
				}
				SelectedImageIndex = ImageIndex;
			}
		}
		private void mn_setLanguage(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_name))
				_resman.DesignerLanguageName = _culture.Name;
			else
				_resman.DesignerLanguageName = _name;

			TreeNode np = this.Parent;
			if (np != null)
			{
				for (int i = 0; i < np.Nodes.Count; i++)
				{
					TreeNodeCulture tnc = np.Nodes[i] as TreeNodeCulture;
					if (tnc != null)
					{
						tnc.ResetImageIndex();
					}
				}
			}
			ResetImageIndex();
		}
		private void mn_delLanguage(object sender, EventArgs e)
		{
			if (this.TreeView != null)
			{
				if (MessageBox.Show(this.TreeView.FindForm(), "Do you want to remove this language?", "Language", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if (string.IsNullOrEmpty(_name))
						_resman.RemoveLanguage(_culture.Name);
					else
						_resman.RemoveLanguage(_name);
					TreeNodeResourceManager tm = null;
					if (this.Parent != null && this.Parent.Parent != null)
					{
						tm = this.Parent.Parent as TreeNodeResourceManager;
					}
					this.Remove();
					if (tm != null)
					{
						tm.ResetResourcesCollectionNodes();
					}
				}
			}
		}
		public void ResetImageIndex()
		{
			if (string.IsNullOrEmpty(_name))
				ImageIndex = TreeViewObjectExplorer.GetSelectedImageIndex(TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name));
			else
				ImageIndex = TreeViewObjectExplorer.GetSelectedImageIndex(TreeViewObjectExplorer.GetLangaugeImageByName(_name));
			SelectedImageIndex = ImageIndex;
		}
		#region ITreeNodeObject Members

		public MenuItem[] GetContextMenuItems(bool bReadOnly)
		{
			MenuItem[] mns = new MenuItem[2];
			if (string.IsNullOrEmpty(_name))
				mns[0] = new MenuItemWithBitmap("Set UI language", mn_setLanguage, TreeViewObjectExplorer.GetLangaugeBitmapByName(_culture.Name));
			else
				mns[0] = new MenuItemWithBitmap("Set UI language", mn_setLanguage, TreeViewObjectExplorer.GetLangaugeBitmapByName(_name));
			mns[1] = new MenuItemWithBitmap("Remove", mn_delLanguage, Resources._cancel.ToBitmap());
			return mns;
		}

		public bool IsInDesign
		{
			get { return true; }
		}

		#endregion
	}
	class TreeNodeCultureCollection : TreeNode, ITreeNodeObject
	{
		private ProjectResources _resman;
		public TreeNodeCultureCollection(ProjectResources rm)
		{
			_resman = rm;
			Text = "Languages";
			ImageIndex = TreeViewObjectExplorer.IMG_LANGUAGES;
			SelectedImageIndex = ImageIndex;
			LoadLanguages();
		}
		private void mn_setToDefault(object sender, EventArgs e)
		{
			_resman.DesignerLanguageName = string.Empty;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeCulture tnc = Nodes[i] as TreeNodeCulture;
				if (tnc != null)
				{
					tnc.ResetImageIndex();
				}
			}
		}
		private void mn_selLanguages(object sender, EventArgs e)
		{
			if (this.TreeView != null)
			{
				if (_resman.SelectLanguages(this.TreeView.FindForm()))
				{
					TreeNodeResourceManager tm = null;
					for (int i = 0; i < this.TreeView.Nodes.Count; i++)
					{
						tm = this.TreeView.Nodes[i] as TreeNodeResourceManager;
						if (tm != null)
						{
							break;
						}
					}
					LoadLanguages();
					if (tm != null)
					{
						tm.ResetResourcesCollectionNodes();
					}
				}
			}
		}
		public void LoadLanguages()
		{
			Nodes.Clear();
			IList<string> ls = _resman.Languages;
			for (int i = 0; i < ls.Count; i++)
			{
				CultureInfo cif;
				if (string.CompareOrdinal("zh", ls[i]) == 0)
				{
					TreeNodeCulture tn = new TreeNodeCulture("zh", _resman);
					Nodes.Add(tn);
				}
				else
				{
					cif = CultureInfo.GetCultureInfo(ls[i]);
					if (cif != null)
					{
						Nodes.Add(new TreeNodeCulture(cif, _resman));
					}
				}

			}
		}

		#region ITreeNodeObject Members

		public MenuItem[] GetContextMenuItems(bool bReadOnly)
		{

			MenuItem[] mns = new MenuItem[2];
			mns[0] = new MenuItemWithBitmap("Select languages", mn_selLanguages, Resources._regionLanguage.ToBitmap());
			mns[1] = new MenuItemWithBitmap("Set UI language to default", mn_setToDefault, Resources._language);
			return mns;
		}

		public bool IsInDesign
		{
			get { return true; }
		}

		#endregion
	}
}
