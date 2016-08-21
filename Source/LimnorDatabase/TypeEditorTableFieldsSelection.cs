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

namespace LimnorDatabase
{
	public class TypeEditorTableFieldsSelection : UITypeEditor
	{
		public TypeEditorTableFieldsSelection()
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
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					IDatabaseTableUser tbu = context.Instance as IDatabaseTableUser;
					if (tbu != null)
					{
						if (!(tbu.DatabaseConnection != null && tbu.DatabaseConnection.ConnectionObject != null && tbu.DatabaseConnection.ConnectionObject.IsConnectionReady))
						{
							ConnectionItem ci = tbu.DatabaseConnection;
							DlgConnectionManager dlg = new DlgConnectionManager();
							dlg.UseProjectScope = true;
							dlg.SetSelection(ci);
							if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
							{
								tbu.DatabaseConnection = dlg.SelectedConnection;
							}
						}
						if (string.IsNullOrEmpty(tbu.TableName))
						{
							TypeSelectorTable ts = new TypeSelectorTable();
							tbu.TableName = ts.EditValue(context, provider, string.Empty) as string;
						}
						if (!string.IsNullOrEmpty(tbu.TableName))
						{
							if (tbu.DatabaseConnection != null &&
								tbu.DatabaseConnection.ConnectionObject != null &&
								tbu.DatabaseConnection.ConnectionObject.IsConnectionReady)
							{
								FieldList fl = tbu.DatabaseConnection.ConnectionObject.GetTableFields(tbu.TableName);
								if (fl != null)
								{
									string[] names = new string[fl.Count];
									for (int i = 0; i < names.Length; i++)
									{
										names[i] = fl[i].Name;
									}
									FieldList sels = value as FieldList;
									string[] ss;
									if (sels == null)
									{
										FieldCollection fc = value as FieldCollection;
										if (fc == null)
										{
											ss = new string[] { };
										}
										else
										{
											ss = new string[fc.Count];
											for (int i = 0; i < fc.Count; i++)
											{
												ss[i] = fc[i].Name;
											}
										}
									}
									else
									{
										ss = new string[sels.Count];
										for (int i = 0; i < sels.Count; i++)
										{
											ss[i] = sels[i].Name;
										}
									}
									UserControlSelectFieldNames fns = new UserControlSelectFieldNames();

									fns.LoadData(edSvc, names, ss);
									edSvc.DropDownControl(fns);
									if (fns.SelectedStrings != null)
									{
										FieldCollection newFields = new FieldCollection();
										for (int i = 0; i < fns.SelectedStrings.Length; i++)
										{
											EPField f = fl[fns.SelectedStrings[i]];
											if (f != null)
											{
												newFields.AddField(f);
											}
										}
										value = newFields;
									}
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
}
