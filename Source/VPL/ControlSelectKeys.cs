/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace VPL
{
	public partial class ControlSelectKeys : UserControl
	{
		private IWindowsFormsEditorService _editotService;
		private bool _madeSelect = false;
		private Keys _selectedKeys;
		public ControlSelectKeys()
		{
			InitializeComponent();
			Array ks = Enum.GetValues(typeof(Keys));
			for (int i = 0; i < ks.Length; i++)
			{
				comboBox1.Items.Add(ks.GetValue(i));
			}
		}
		public bool MadeSelection
		{
			get
			{
				return _madeSelect;
			}
		}
		public Keys Selection
		{
			get
			{
				return _selectedKeys;
			}
		}
		public void SetEditorService(IWindowsFormsEditorService editorService, Keys currentKeys)
		{
			_editotService = editorService;
			chkAlt.Checked = ((currentKeys & Keys.Alt) == Keys.Alt);
			chkCtrl.Checked = ((currentKeys & Keys.Control) == Keys.Control);
			chkShift.Checked = ((currentKeys & Keys.Shift) == Keys.Shift);
			for (int i = 1; i < comboBox1.Items.Count; i++)
			{
				Keys k = (Keys)comboBox1.Items[i];
				if ((currentKeys & k) == k)
				{
					if (k != Keys.Shift && k != Keys.ShiftKey && k != Keys.LShiftKey && k != Keys.RShiftKey
						&& k != Keys.Control && k != Keys.ControlKey && k != Keys.LControlKey && k != Keys.RControlKey
						&& k != Keys.Alt)
					{
						comboBox1.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			btOK.Enabled = (comboBox1.SelectedIndex >= 0);
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex >= 0)
			{
				_madeSelect = true;
				_selectedKeys = Keys.None;
				if (chkAlt.Checked)
					_selectedKeys |= Keys.Alt;
				if (chkCtrl.Checked)
					_selectedKeys |= Keys.Control;
				if (chkShift.Checked)
					_selectedKeys |= Keys.Shift;
				_selectedKeys |= (Keys)comboBox1.Items[comboBox1.SelectedIndex];
				_editotService.CloseDropDown();
			}
		}
	}
}
