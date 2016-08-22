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

namespace Limnor.WebBuilder
{
	public partial class DialogMapMarkers : Form
	{
		private HtmlGoogleMap _gmap;
		public GoogleMapsMarkerCollection Result;
		public DialogMapMarkers()
		{
			InitializeComponent();
		}
		public void LoadData(HtmlGoogleMap gmap)
		{
			_gmap = gmap;
			foreach (GoogleMapsMarker mm in _gmap.GoogleMapsMarkers)
			{
				listBox1.Items.Add(mm);
			}
		}
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				if (MessageBox.Show(this, "Do you want to remove selected map marker?", "Google Map Marker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					listBox1.Items.RemoveAt(n);
					if (n < listBox1.Items.Count)
					{
						listBox1.SelectedIndex = n;
					}
					else
					{
						listBox1.SelectedIndex = listBox1.Items.Count - 1;
					}
				}
			}
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			listBox1.Items.Add(new GoogleMapsMarker(_gmap));
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Result = new GoogleMapsMarkerCollection(_gmap);
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				Result.Add(listBox1.Items[i] as GoogleMapsMarker);
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
