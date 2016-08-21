/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
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
using DirectX.Capture;

namespace Limnor.DirectXCapturer
{
	public partial class DialogMakeSelection : Form
	{
		public Filter SelectedFilter;
		public Source SelectedSource;
		enum EnumSelection { Filter, Source }
		EnumSelection _selection = EnumSelection.Filter;
		public DialogMakeSelection()
		{
			InitializeComponent();
		}
		public void LoadFilters(IList<Filter> filters, string current)
		{
			_selection = EnumSelection.Filter;
			listBox1.Items.Clear();
			listBox1.Items.Add("");
			listBox1.SelectedIndex = 0;
			foreach (Filter f in filters)
			{
				int n = listBox1.Items.Add(f);
				if (listBox1.SelectedIndex == 0)
				{
					if (string.Compare(f.Name, current, StringComparison.OrdinalIgnoreCase) == 0)
					{
						listBox1.SelectedIndex = n;
					}
				}
			}
		}
		public void LoadSources(SourceCollection sources, Source current)
		{
			_selection = EnumSelection.Source;
			listBox1.Items.Clear();
			if (sources != null)
			{
				foreach (Source s in sources)
				{
					int n = listBox1.Items.Add(s);
					if (listBox1.SelectedIndex == 0 && current != null)
					{
						if (string.Compare(s.Name, current.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							listBox1.SelectedIndex = n;
						}
					}
				}
			}
			if (listBox1.SelectedIndex < 0 && listBox1.Items.Count > 0)
			{
				listBox1.SelectedIndex = 0;
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (_selection == EnumSelection.Filter)
			{
				if (listBox1.SelectedIndex > 0)
				{
					SelectedFilter = listBox1.Items[listBox1.SelectedIndex] as Filter;
				}
				else
				{
					SelectedFilter = null;
				}
				DialogResult = DialogResult.OK;
			}
			else
			{
				if (listBox1.SelectedIndex >= 0)
				{
					SelectedSource = listBox1.Items[listBox1.SelectedIndex] as Source;
					DialogResult = DialogResult.OK;
				}
				else
				{
					SelectedSource = null;
				}
			}

		}
	}
}
