/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace VPL
{
	public partial class DialogSelectIDs : Form
	{
		public List<UInt32> Selections;
		public DialogSelectIDs()
		{
			InitializeComponent();
		}
		public void LoadData(Dictionary<UInt32, string> all, List<UInt32> selected)
		{
			if (selected != null)
			{
				Selections = selected;
				foreach (KeyValuePair<UInt32, string> kv in all)
				{
					listBoxAll.Items.Add(new KeyValue(kv));
				}
				for (int i = 0; i < selected.Count; i++)
				{
					if (all.ContainsKey(selected[i]))
					{
						listBoxSelected.Items.Add(new KeyValue(new KeyValuePair<UInt32, string>(selected[i], all[selected[i]])));
					}
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Selections = new List<uint>();
			for (int i = 0; i < listBoxSelected.Items.Count; i++)
			{
				KeyValue kv = listBoxSelected.Items[i] as KeyValue;
				Selections.Add(kv.ID);
			}
			DialogResult = DialogResult.OK;
		}
		class KeyValue
		{
			private KeyValuePair<UInt32, string> _kv;
			public KeyValue(KeyValuePair<UInt32, string> k)
			{
				_kv = k;
			}
			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} - {1}", _kv.Value, _kv.Key);
			}
			public UInt32 ID
			{
				get
				{
					return _kv.Key;
				}
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			int n = listBoxAll.SelectedIndex;
			if (n >= 0)
			{
				listBoxSelected.Items.Add(listBoxAll.Items[n]);
			}
		}

		private void buttonDel_Click(object sender, EventArgs e)
		{
			int n = listBoxSelected.SelectedIndex;
			if (n >= 0)
			{
				listBoxSelected.Items.RemoveAt(n);
				if (listBoxSelected.Items.Count > n)
				{
					listBoxSelected.SelectedIndex = n;
				}
				else
				{
					listBoxSelected.SelectedIndex = listBoxSelected.Items.Count - 1;
				}
			}
		}

		private void buttonUp_Click(object sender, EventArgs e)
		{
			int n = listBoxSelected.SelectedIndex;
			if (n > 0)
			{
				object v = listBoxSelected.Items[n];
				listBoxSelected.Items.RemoveAt(n);
				n--;
				listBoxSelected.Items.Insert(n, v);
				listBoxSelected.SelectedIndex = n;
			}
		}

		private void buttonDown_Click(object sender, EventArgs e)
		{
			int n = listBoxSelected.SelectedIndex;
			if (n >= 0 && n < listBoxSelected.Items.Count - 1)
			{
				object v = listBoxSelected.Items[n];
				listBoxSelected.Items.RemoveAt(n);
				n++;
				listBoxSelected.Items.Insert(n, v);
				listBoxSelected.SelectedIndex = n;
			}
		}
	}
}
