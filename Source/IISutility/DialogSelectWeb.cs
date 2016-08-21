/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Information Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LimnorWeb
{
	public partial class DialogSelectWeb : Form
	{
		public VirtualWebDir Ret;
		public DialogSelectWeb()
		{
			InitializeComponent();
		}
		public void LoadData(string initSelection)
		{
			ListViewItem viSel = null;
			IList<VirtualWebDir> vs = IisUtility.GetVirtualDirectories(IisUtility.LocalWebPath);
			foreach (VirtualWebDir v in vs)
			{
				ListViewItem vi = new ListViewItem(new string[] { v.WebName, v.PhysicalDirectory });
				vi.Tag = v;
				listView1.Items.Add(vi);
				if (viSel == null)
				{
					if (!string.IsNullOrEmpty(initSelection))
					{
						if (string.Compare(initSelection, v.WebName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							viSel = vi;
						}
					}
				}
			}
			if (viSel != null)
			{
				viSel.Selected = true;
			}
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
			{
				Ret = listView1.SelectedItems[0].Tag as VirtualWebDir;
				if (Ret != null)
				{
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			DialogNewWeb dlg = new DialogNewWeb();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				Ret = dlg.Ret;
				ListViewItem vi = new ListViewItem(new string[] { Ret.WebName, Ret.PhysicalDirectory });
				vi.Tag = Ret;
				listView1.Items.Add(vi);
				vi.Selected = true;
			}
		}
	}
}
