/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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

namespace LimnorDatabase
{
	public partial class DlgParameters : Form
	{
		private ParameterList _ps;
		private DbParameterList _dl;
		public DlgParameters()
		{
			InitializeComponent();
		}
		public void NoCancel()
		{
			buttonCancel.Enabled = false;
		}
		public void LoadData(ParameterList parameters)
		{
			if (parameters == null)
			{
				throw new ExceptionLimnorDatabase("parameters is null calling LoadData");
			}
			_ps = parameters;
			ParameterList ps = (ParameterList)_ps.Clone();
			_dl = new DbParameterList(ps);
			propertyGrid1.SelectedObject = _dl;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < _dl.Count; i++)
			{
				_ps[i].SetValue(_dl[i].DefaultValue);
				_ps[i].OleDbType = _dl[i].Type;
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
