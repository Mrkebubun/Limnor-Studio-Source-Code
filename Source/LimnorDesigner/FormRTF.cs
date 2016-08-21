/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public partial class FormRTF : Form
	{
		private string _key;
		public FormRTF()
		{
			InitializeComponent();
		}
		public void LoadData(string key, string file)
		{
			_key = key;
			if (File.Exists(file))
			{
				rte.LoadFile(file);
			}
			else
			{
				rte.SetSaveFile(file);
			}
			this.Text = Path.GetFileName(file);
		}
		public void SetFile(string file)
		{
			this.Text = Path.GetFileName(file);
			rte.SetSaveFile(file);
		}
		public string Key
		{
			get
			{
				return _key;
			}
		}
		public void AskSave()
		{
			if (rte.Modified)
			{
				bool bSave = true;
				if (rte.CancelEdit)
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "{0} was modified. Do you want to save the modifications?", this.Text), "Cancel Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
					{
						bSave = false;
						rte.ResetModified();
					}
				}
				if (bSave)
				{
					rte.SaveFile(null);
				}
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			AskSave();
			base.OnClosing(e);
		}
	}
}
