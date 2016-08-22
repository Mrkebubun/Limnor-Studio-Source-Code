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
using System.Text;
using System.Windows.Forms;

namespace VOB
{
	public partial class UserControlUserGuide : UserControl
	{
		public UserControlUserGuide()
		{
			InitializeComponent();
		}
		private bool _showBackgroundText = true;
		public bool ShowBackgroundText { get { return _showBackgroundText; } set { _showBackgroundText = value; } }

		private string _folder;
		public string HelpFolder { get { return _folder; } set { _folder = value; } }

		private string _exe;
		public string Exe { get { return _exe; } set { _exe = value; } }
	}
}
