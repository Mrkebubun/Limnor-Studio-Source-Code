/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LimnorDatabase
{
	class TreeNodeConnectionItem : TreeNode
	{
		public const int IMG_CNN = 0;
		public const int IMG_APP = 1;
		public const int IMG_ERR = 2;
		public const int IMG_CNN_ERR = 3;
		public const int IMG_CNN_OK = 4;
		//
		private ConnectionItem _item;
		private bool _nextLevelLoaded;
		private bool _merged;
		public TreeNodeConnectionItem(ConnectionItem item)
		{
			_item = item;
			Text = _item.ToString();
			if (item.IsValid)
			{
				ImageIndex = IMG_CNN;
			}
			else
			{
				ImageIndex = IMG_CNN_ERR;
			}
			SelectedImageIndex = ImageIndex;
			this.Nodes.Add(new CLoad());
		}
		public ConnectionItem OwnerItem
		{
			get
			{
				return _item;
			}
		}
		public void OnSelected()
		{
			if (!_merged)
			{
				_merged = true;
			}
			if (!_item.ConnectionChecked)
			{
				_item.CheckConnection();
				if (_item.IsValid)
				{
					ImageIndex = IMG_CNN;
				}
				else
				{
					ImageIndex = IMG_CNN_ERR;
				}
				SelectedImageIndex = ImageIndex;
			}
		}
		public void LoadNextLevel()
		{
			if (!_nextLevelLoaded)
			{
				_nextLevelLoaded = true;
				List<CLoad> lst = new List<CLoad>();
				for (int i = 0; i < Nodes.Count; i++)
				{
					CLoad c = Nodes[i] as CLoad;
					if (c != null)
					{
						lst.Add(c);
					}
				}
				foreach (CLoad c in lst)
				{
					c.Remove();
				}
				foreach (CLoad c in lst)
				{
					c.LoadNextLevel(this);
				}
			}
		}
		class CLoad : TreeNode
		{
			public CLoad()
			{
			}
			public void LoadNextLevel(TreeNodeConnectionItem p)
			{
				if (p.OwnerItem.UsedByApplications != null && p.OwnerItem.UsedByApplications.Length > 0)
				{
					for (int i = 0; i < p.OwnerItem.UsedByApplications.Length; i++)
					{
						TreeNode tn;
						int n = IMG_APP;
						if (!System.IO.File.Exists(p.OwnerItem.UsedByApplications[i]))
						{
							n = IMG_ERR;
						}
						tn = new TreeNode(p.OwnerItem.UsedByApplications[i], n, n);
						p.Nodes.Add(tn);
					}
				}
			}
		}
	}
}
