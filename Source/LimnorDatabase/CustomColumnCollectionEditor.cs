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
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using VPL;

namespace LimnorDatabase
{
	class CustomColumnCollectionEditor : UITypeEditor
	{
		private Form dataGridViewColumnCollectionDialog;
		private CustomColumnCollectionEditor() { }
		private static Form CreateColumnCollectionDialog(IServiceProvider provider, DataGridViewColumnCollection cols, DataGridView dv)
		{
			Form frm = null;
			Assembly assembly = Assembly.Load(typeof(ControlDesigner).Assembly.ToString());
			Type type = assembly.GetType("System.Windows.Forms.Design.DataGridViewColumnCollectionDialog");
			ConstructorInfo[] ctr = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
			if (ctr != null && ctr.Length > 0)
			{
				ParameterInfo[] ps = ctr[0].GetParameters();
				if (ps == null || ps.Length == 0)
				{
					frm = (Form)ctr[0].Invoke(new object[] { });
				}
				else
				{
					if (ps != null && ps.Length == 1 && ps[0].ParameterType == typeof(IServiceProvider))
					{
						frm = (Form)ctr[0].Invoke(new object[] { provider });
					}
				}
			}
			return frm;
		}

		public static void SetLiveDataGridView(Form form, DataGridView grid)
		{
			var mi = form.GetType().GetMethod("SetLiveDataGridView", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(form, new object[] { grid });
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null && context != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service == null || context.Instance == null)
					return value;
				IDataGrid eg = context.Instance as IDataGrid;
				DataGridViewColumnCollection cols;
				if (eg != null)
				{
					DlgEditColumns dlgCols = new DlgEditColumns();
					cols = value as DataGridViewColumnCollection;
					if (cols == null)
					{
						cols = new DataGridViewColumnCollection((DataGridView)eg);
					}
					dlgCols.LoadData(cols);
					eg.DisableColumnChangeNotification = true;
					if (service.ShowDialog(dlgCols) == DialogResult.OK)
					{
						value = cols;
						eg.OnChangeColumns(cols);
					}
					eg.DisableColumnChangeNotification = false;
				}
				
				{
					IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
					if (host == null)
					{
						IComponent ic = context.Instance as IComponent;
						if (ic != null && ic.Site != null)
						{
							host = ic.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
						}
					}
					if (host == null)
					{
						return value;
					}
					if (dataGridViewColumnCollectionDialog == null)
						dataGridViewColumnCollectionDialog = CreateColumnCollectionDialog(provider, value as DataGridViewColumnCollection, context.Instance as DataGridView);
					if (dataGridViewColumnCollectionDialog == null)
						return value;
					//Unfortunately we had to make property which returns inner datagridview  
					//to access it here because we need to pass DataGridView into SetLiveDataGridView () method 
					DataGridView grid = context.Instance as DataGridView;
					//we have to set Site property because it will be accessed inside SetLiveDataGridView () method 
					//and by default it's usually null, so if we do not set it here, we will get exception inside SetLiveDataGridView () 
					//var oldSite = grid.Site; 
					//grid.Site = ((UserControl) context.Instance).Site; 
					//execute SetLiveDataGridView () via reflection 
					SetLiveDataGridView(dataGridViewColumnCollectionDialog, grid);
					using (DesignerTransaction transaction = host.CreateTransaction("DataGridViewColumnCollectionTransaction"))
					{
						if (service.ShowDialog(dataGridViewColumnCollectionDialog) == DialogResult.OK)
						{
							transaction.Commit();
							if (eg != null)
							{
								cols = value as DataGridViewColumnCollection;
								eg.OnChangeColumns(cols);
								IDevClass dc = eg.GetDevClass();
								if (dc != null)
								{
									dc.NotifyChange(eg, context.PropertyDescriptor.Name);
								}
							}
						}
						else
							transaction.Cancel();
						
					}
				}
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}
