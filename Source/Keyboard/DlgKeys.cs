/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
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

namespace Limnor.InputDevice
{
	public partial class DlgKeys : Form
	{
		public HotKeyList Results;
		private bool _adjusting;
		public DlgKeys()
		{
			InitializeComponent();
		}
		public void LoadData(HotKeyList list)
		{
			if (list == null)
			{
				Results = new HotKeyList();
			}
			else
			{
				Results = (HotKeyList)list.Clone();
			}
			foreach (Key k in Results.HotKeys.Values)
			{
				listBox1.Items.Add(k);
			}
		}
		private void btAdd_Click(object sender, EventArgs e)
		{
			Key k = new Key();
			if (Results.HotKeys.ContainsKey(k.KeyName))
			{
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					Key k0 = listBox1.Items[i] as Key;
					if (k0 == k)
					{
						listBox1.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				Results.HotKeys.Add(k.KeyName, k);
				listBox1.SelectedIndex = listBox1.Items.Add(k);
				propertyGrid1.SelectedObject = k;
			}
		}

		private void btDel_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0 && n < listBox1.Items.Count)
			{
				Key key = listBox1.Items[n] as Key;
				Results.Remove(key.KeyName);
				listBox1.Items.RemoveAt(n);
				if (n >= listBox1.Items.Count)
				{
					n = listBox1.Items.Count - 1;
				}
				listBox1.SelectedIndex = n;
			}
		}
		int _idx;
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_adjusting)
			{
				_idx = listBox1.SelectedIndex;
				if (_idx >= 0)
				{
					propertyGrid1.SelectedObject = listBox1.SelectedItem;
				}
				else
				{
					propertyGrid1.SelectedObject = null;
				}
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (!_adjusting)
			{
				_adjusting = true;
				if (_idx >= 0 && _idx < listBox1.Items.Count)
				{
					Key v = listBox1.Items[_idx] as Key;
					listBox1.Items.RemoveAt(_idx);
					listBox1.Items.Insert(_idx, v);
					Results.UpdateKeyById(v);
				}
				_adjusting = false;
			}
		}
	}
}
