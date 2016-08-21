/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Windows.Forms.Design;

namespace LimnorDatabase
{
	public class DbTypeListBox : ListBox
	{
		private IWindowsFormsEditorService _service;
		public OleDbType SelectedOleDbType;
		//
		public DbTypeListBox()
		{
			init();
		}
		public DbTypeListBox(IWindowsFormsEditorService service)
			: this()
		{
			_service = service;
		}
		private void init()
		{
			Items.AddRange(DbTypeComboBox.GetFieldNames());
		}
		private int _selectedIndex
		{
			get
			{
				return SelectedIndex;
			}
			set
			{
				SelectedIndex = value;
			}
		}
		private void setSelection()
		{
			if (_selectedIndex >= 0)
			{
				SelectedOleDbType = CurrentSelectOleDbType();
			}
			else
			{
				SelectedOleDbType = OleDbType.IUnknown;
			}
			if (_service != null)
			{
				_service.CloseDropDown();
			}
		}
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			setSelection();
		}
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			setSelection();
		}
		public OleDbType CurrentSelectOleDbType()
		{
			return DbTypeComboBox.GetSelectedType(_selectedIndex);
		}
		public bool CurrentSelectAllowChangeSize()
		{
			return (_selectedIndex == DbTypeComboBox.FLD_String);
		}
		public int CurrentSelectDataSize()
		{
			return DbTypeComboBox.GetSelectedDataSize(_selectedIndex);
		}
		public void SetSelectionByOleDbType(OleDbType type)
		{
			_selectedIndex = DbTypeComboBox.GetOleDbTypeIndex(type);
		}
	}
}
