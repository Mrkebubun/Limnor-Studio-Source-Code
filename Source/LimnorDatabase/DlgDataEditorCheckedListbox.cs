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
using Limnor.WebBuilder;

namespace LimnorDatabase
{
	public partial class DlgDataEditorCheckedListbox : DlgSetEditorAttributes
	{
		public DlgDataEditorCheckedListbox()
		{
			InitializeComponent();
		}
		public DlgDataEditorCheckedListbox(WebDataEditorCheckedListbox editor)
			: base(editor)
		{
			InitializeComponent();
			//
			if (editor == null)
			{
				editor = new WebDataEditorCheckedListbox();
				SetSelection(editor);
			}
			loadData();
		}
		private void loadData()
		{
			WebDataEditorCheckedListbox wd = (WebDataEditorCheckedListbox)SelectedEditor;
			querySelector.LoadQuery(wd);
			txtList.Text = wd.GetListItems();
			chkUseDb.Checked = wd.UseDataFromDatabase;
		}
		public static DlgSetEditorAttributes GetCheckedListDataDialog(DataEditor current, DataEditor caller)
		{
			WebDataEditorCheckedListbox c = current as WebDataEditorCheckedListbox;
			WebDataEditorCheckedListbox u = caller as WebDataEditorCheckedListbox;
			if (c == null)
			{
				c = (WebDataEditorCheckedListbox)caller.Clone();
			}

			DlgDataEditorCheckedListbox dlg = new DlgDataEditorCheckedListbox(c);
			return dlg;
		}
		private void chkUseDb_CheckedChanged(object sender, EventArgs e)
		{
			querySelector.Visible = chkUseDb.Checked;
			lblDb.Visible = chkUseDb.Checked;
			txtList.Visible = !chkUseDb.Checked;
			lblList.Visible = !chkUseDb.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			WebDataEditorCheckedListbox wd = (WebDataEditorCheckedListbox)SelectedEditor;
			wd.UseDataFromDatabase = chkUseDb.Checked;
			wd.SetListItems(txtList.Lines);
			this.DialogResult = DialogResult.OK;
		}
	}
}
