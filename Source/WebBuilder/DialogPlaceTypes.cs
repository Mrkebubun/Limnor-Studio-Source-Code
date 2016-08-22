/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
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
using google.maps.places;
using System.Globalization;

namespace Limnor.WebBuilder
{
	public partial class DialogPlaceTypes : Form
	{
		public string[] SelectedTypes;
		private int lstColWidth;
		public DialogPlaceTypes()
		{
			InitializeComponent();
		}
		public void LoadData(string[] selected)
		{
			int nSize = 0;
			string maxLenItem = string.Empty;
			SelectedTypes = selected;
			string[] names = Enum.GetNames(typeof(EnumPlaceType));
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i].Length > nSize)
				{
					maxLenItem = names[i];
					nSize = maxLenItem.Length;
				}
				bool chk = false;
				if (selected != null && selected.Length > 0)
				{
					for (int k = 0; k < selected.Length; k++)
					{
						if (string.CompareOrdinal(selected[k], names[i]) == 0)
						{
							chk = true;
							break;
						}
					}
				}
				checkedListBox1.Items.Add(names[i], chk);
			}
			maxLenItem = string.Format(CultureInfo.InvariantCulture, "{0}MM", maxLenItem);
			Graphics g = this.CreateGraphics();
			SizeF sf = g.MeasureString(maxLenItem, checkedListBox1.Font);
			lstColWidth = (int)sf.Width;
			g.Dispose();
			checkedListBox1.MultiColumn = true;
			checkedListBox1.ColumnWidth = lstColWidth;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			List<string> sl = new List<string>();
			for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
			{
				sl.Add(checkedListBox1.CheckedItems[i].ToString());
			}
			SelectedTypes = sl.ToArray();
			this.DialogResult = DialogResult.OK;
		}

		private void DialogPlaceTypes_Resize(object sender, EventArgs e)
		{

		}
	}
}
