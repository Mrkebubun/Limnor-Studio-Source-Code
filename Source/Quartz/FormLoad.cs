/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Play audio and video using Media Control Interface
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Quartz
{
	public partial class FormLoad : Form
	{
		public string Filename;
		public FormLoad()
		{
			InitializeComponent();
		}
		private bool _loaded;
		private void FormLoad_Activated(object sender, EventArgs e)
		{
			if (!_loaded)
			{
				_loaded = true;
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = "Select media file";
				dlg.Filter = "Media file|*.avi;*.wav";
				dlg.FileName = Filename;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Filename = dlg.FileName;
					this.DialogResult = DialogResult.OK;
				}
				else
				{
					this.DialogResult = DialogResult.Cancel;
				}
			}
		}
	}
}
