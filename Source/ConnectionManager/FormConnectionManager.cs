/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database connection manager
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
using LimnorDatabase;

namespace ConnectionManager
{
	public partial class FormConnectionManager : DlgConnectionManager
	{
		public FormConnectionManager()
		{
			InitializeComponent();
			HideOKCancelButtons();
		}
	}
}
