/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OracleClient;
using VPL;
using System.Data.Common;

namespace LimnorDatabase
{
	class ConnectionTypeSelector : UITypeEditor
	{
		public ConnectionTypeSelector()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					ConnectionTypes lst = new ConnectionTypes(service);
					service.DropDownControl(lst);
					if (lst.SelectedType != null)
					{
						value = lst.SelectedType;
					}
				}
			}
			return value;
		}
		class ConnectionTypes : ListBox
		{
			private IWindowsFormsEditorService _service;
			public Type SelectedType = null;
			public ConnectionTypes(IWindowsFormsEditorService service)
			{
				_service = service;
				Type tySql = Type.GetType(Connection.MySqlConnectionTypeName);
				if (tySql != null)
				{
					Items.Add(new TypeData(tySql));
				}
				Items.Add(new TypeData(typeof(OleDbConnection)));
				Items.Add(new TypeData(typeof(SqlConnection)));
				Items.Add(new TypeData(typeof(OdbcConnection)));
#if NET35
				Items.Add(new TypeData(typeof(OracleConnection)));
#endif
				Items.Add("Other connection type");
			}
			protected override void OnClick(EventArgs e)
			{
				if (SelectedIndex >= 0)
				{
					TypeData data = Items[SelectedIndex] as TypeData;
					if (data != null)
					{
						SelectedType = data.Type;
						_service.CloseDropDown();
					}
					else
					{
						FormTypeSelection dlg = new FormTypeSelection();
						dlg.Text = "Select database connection type";
						dlg.SetSelectionBaseType(typeof(DbConnection));
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							SelectedType = dlg.SelectedType;
							_service.CloseDropDown();
						}
					}
				}
			}

		}
	}
	class TypeData
	{
		private Type _type;
		public TypeData(Type t)
		{
			_type = t;
		}
		public override string ToString()
		{
			return _type.Name;
		}
		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}
