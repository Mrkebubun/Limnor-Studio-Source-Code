/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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

namespace LimnorDatabase
{
	public partial class UserControlSelectFieldNames : UserControl
	{
		public string[] SelectedStrings;
		private IWindowsFormsEditorService _service;
		public UserControlSelectFieldNames()
		{
			InitializeComponent();
		}
		public void LoadData(IWindowsFormsEditorService service, string[] allNames, string[] selectedNames)
		{
			_service = service;
			if (allNames != null)
			{
				for (int i = 0; i < allNames.Length; i++)
				{
					int n = checkedListBox1.Items.Add(allNames[i]);
					if (selectedNames != null && selectedNames.Length > 0)
					{
						for (int j = 0; j < selectedNames.Length; j++)
						{
							if (string.CompareOrdinal(allNames[i], selectedNames[j]) == 0)
							{
								checkedListBox1.SelectedIndices.Add(n);
								checkedListBox1.SetItemChecked(n, true);
								break;
							}
						}
					}
				}
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			SelectedStrings = null;
			_service.CloseDropDown();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			SelectedStrings = new string[checkedListBox1.CheckedItems.Count];
			for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
			{
				SelectedStrings[i] = checkedListBox1.CheckedItems[i].ToString();
			}
			_service.CloseDropDown();
		}

		private void buttonCheckAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, true);
			}
		}

		private void buttonUncheckAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, false);
			}
		}
	}
}
