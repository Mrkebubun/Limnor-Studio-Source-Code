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
using System.Windows.Forms.Design;
using System.ComponentModel;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
	/// for SQL property of EasyQuery
	/// </summary>
	public class UIQueryEditor : UITypeEditor
	{
		public UIQueryEditor()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					IQuery qry = context.Instance as IQuery;
					if (qry == null)
					{
						IClassId classPointer = context.Instance as IClassId;
						if (classPointer != null)
						{
							qry = VPLUtil.GetObject(classPointer.ObjectInstance) as IQuery;
						}
					}
					if (qry == null)
					{
						ICustomEventMethodType cemt = context.Instance as ICustomEventMethodType;
						if (cemt != null)
						{
							qry = cemt.ObjectValue as IQuery;
						}
					}
					if (qry != null)
					{
						IDatabaseAccess da = qry as IDatabaseAccess;
						if (da != null)
						{
							if (da.OnBeforeSetSQL())
							{
								if (context.PropertyDescriptor != null)
								{
									da.SetSqlContext(context.PropertyDescriptor.Name);
								}
							}
							else
							{
								return value;
							}
						}
						bool bOK = qry.IsConnectionReady;
						if (!bOK)
						{
							DlgConnectionManager dlgC = new DlgConnectionManager();
							dlgC.UseProjectScope = true;
							if (edSvc.ShowDialog(dlgC) == System.Windows.Forms.DialogResult.OK)
							{
								qry.DatabaseConnection = dlgC.SelectedConnection;
								bOK = true;
							}
						}
						if (bOK)
						{
							if (qry.IsConnectionReady)
							{
								dlgQueryBuilder dlg = new dlgQueryBuilder();
								QueryParser qp = new QueryParser();
								qp.LoadData(qry.QueryDef);
								dlg.LoadData(qp);
								if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
								{
									IDevClassReferencer dcr = qry as IDevClassReferencer;
									if (dcr != null)
									{
										IDevClass dc = dcr.GetDevClass();
										if (dc != null)
										{
											dc.NotifyBeforeChange(context.Instance, "SqlQuery");
										}
									}
									qp.checkQueryreadOnly(qp.query);
									qry.CopyFrom(qp.query);
									if (da != null && !(da is EasyGrid))
									{
										da.Query();//make the query and get SQL
									}
									value = qp.query.SQL.Clone(); //qry.SQL.Clone();
									
									if (context.PropertyDescriptor.IsReadOnly)
									{
										IDatabaseAccess eq = qry as IDatabaseAccess;
										if (eq != null)
										{
											if (eq.NeedDesignTimeSQL)
											{
												eq.SQL = (SQLStatement)value;
											}
											else
											{
												PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(qry);
												ISqlUser eds = qry as ISqlUser;
												if (eds != null)
												{
												}
												else
												{
													//SQL property is read-only and will not get designer notified
													//For EasyGrid it will cause other properties changes.
													eq.SQL = (SQLStatement)value;
												}
											}
										}
										
										if (dcr != null)
										{
											IDevClass dc = dcr.GetDevClass();
											if (dc != null)
											{
												dc.NotifyChange(context.Instance, "SqlQuery");
											}
										}
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
