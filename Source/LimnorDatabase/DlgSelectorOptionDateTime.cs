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
using VPL;

namespace LimnorDatabase
{
	public partial class DlgSelectorOptionDateTime : DlgSetEditorAttributes
	{
		public DlgSelectorOptionDateTime()
		{
			InitializeComponent();
		}
		public DlgSelectorOptionDateTime(DataEditorDatetime ded)
			: base(ded)
		{
			InitializeComponent();
			//
			if (ded == null)
			{
				ded = new DataEditorDatetime();
				SetSelection(ded);
			}
			radioButtonLarge.Checked = ded.UseLargeDialogue;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DataEditorDatetime edb = (DataEditorDatetime)(this.SelectedEditor);
			edb.UseLargeDialogue = radioButtonLarge.Checked;
			this.DialogResult = DialogResult.OK;
		}
	}
}
