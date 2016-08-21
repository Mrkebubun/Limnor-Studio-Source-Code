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
	public class DbTypeComboBox : ComboBox
	{
		//mapping to index
		public const int FLD_Unknown = -1;
		public const int FLD_String = 0;
		public const int FLD_Integer = 1;
		public const int FLD_Long_integer = 2;
		public const int FLD_Decimal = 3;
		public const int FLD_Currency = 4;
		public const int FLD_Date = 5;
		public const int FLD_Time = 6;
		public const int FLD_Date_time = 7;
		public const int FLD_Bool = 8;
		public const int FLD_Text = 9;
		public const int FLD_Binary = 10;

		//
		private IWindowsFormsEditorService _service;
		public OleDbType SelectedOleDbType;
		//
		public DbTypeComboBox()
		{
			init();
		}
		public DbTypeComboBox(IWindowsFormsEditorService service)
			: this()
		{
			_service = service;
		}
		public static object[] GetFieldNames()
		{
			return new object[] {
														"String", //FLD_String
														"Integer", //FLD_Integer
														"Long integer", //FLD_Long_integer
														"Decimal", //FLD_Decimal
														"Currency", //FLD_Currency
														"Date", //FLD_Date
														"Time", //FLD_Time
														"Date and time", //FLD_Date_time
														"Yes/No", //FLD_Bool
														"Large text", //FLD_Text
														"Large binary" //FLD_Binary
				};
		}
		private void init()
		{
			Items.AddRange(GetFieldNames());
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
		public static OleDbType GetSelectedType(int idx)
		{
			switch (idx)
			{
				case FLD_String:
					return OleDbType.VarWChar;
				case FLD_Integer:
					return OleDbType.Integer;
				case FLD_Long_integer:
					return OleDbType.BigInt;
				case FLD_Decimal:
					return OleDbType.Double;
				case FLD_Currency:
					return OleDbType.Currency;
				case FLD_Date:
					return OleDbType.DBDate;
				case FLD_Time:
					return OleDbType.DBTime;
				case FLD_Date_time:
					return OleDbType.DBTimeStamp;
				case FLD_Bool:
					return OleDbType.Boolean;
				case FLD_Text:
					return OleDbType.LongVarWChar;
				case FLD_Binary:
					return OleDbType.LongVarBinary;
			}
			return OleDbType.VarWChar;
		}
		public OleDbType CurrentSelectOleDbType()
		{
			return GetSelectedType(_selectedIndex);
		}
		public bool CurrentSelectAllowChangeSize()
		{
			return (_selectedIndex == FLD_String);
		}
		public static int GetSelectedDataSize(int idx)
		{
			switch (idx)
			{
				case FLD_Integer:
					return 4;
				case FLD_Decimal:
					return 8;
				case FLD_Currency:
					return 8;
				case FLD_Date:
				case FLD_Time:
				case FLD_Date_time:
					return 8;
				case FLD_Bool:
					return 1;
				case FLD_Text:
					return 0;
				case FLD_Binary:
					return 0;
				case FLD_Long_integer:
					return 8;
			}
			return 0;
		}
		public int CurrentSelectDataSize()
		{
			return GetSelectedDataSize(_selectedIndex);
		}
		public static int GetOleDbTypeIndex(OleDbType type)
		{
			if (type == System.Data.OleDb.OleDbType.BigInt ||
				type == System.Data.OleDb.OleDbType.UnsignedBigInt
				)
				return FLD_Long_integer;
			else if (EPField.IsInteger(type))
				return FLD_Integer;
			else if (type == System.Data.OleDb.OleDbType.Currency)
				return FLD_Currency;
			else if (EPField.IsNumber(type))
				return FLD_Decimal;
			else if (EPField.IsDatetime(type))
			{
				if (type == System.Data.OleDb.OleDbType.DBTimeStamp)
					return FLD_Date_time;
				else if (type == OleDbType.DBTime)
					return FLD_Time;
				else
					return FLD_Date;
			}
			else if (type == System.Data.OleDb.OleDbType.Boolean)
				return FLD_Bool;
			else if (type == System.Data.OleDb.OleDbType.LongVarChar ||
				type == System.Data.OleDb.OleDbType.LongVarWChar)
				return FLD_Text;
			else if (type == System.Data.OleDb.OleDbType.Binary ||
				type == System.Data.OleDb.OleDbType.LongVarBinary ||
				type == System.Data.OleDb.OleDbType.VarBinary
				)
				return FLD_Binary;
			else if (EPField.IsString(type))
				return FLD_String;
			else
				return FLD_Unknown;
		}
		public void SetSelectionByOleDbType(OleDbType type)
		{
			_selectedIndex = GetOleDbTypeIndex(type);
			
		}
	}
}
