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
using System.Data.Common;
using System.Data;

namespace LimnorDatabase
{
	public class ComboLook : ComboBox
	{
		//-1:to use field associated; >=0: field ordinate position
		protected int nUpdatePos = -1;
		public bool bNoEvent = false;
		public ComboLook()
		{
		}
		/// <summary>
		/// 1 = Yes : Corresponding to index 0
		/// 0 = No  : Corresponding to index 1
		/// </summary>
		/// <returns></returns>
		public virtual object GetLookupData()
		{
			if (this.SelectedIndex == 0)
				return 1;
			else
				return 0;
		}
		public virtual void SetSelectedIndex(object v)
		{
			for (int i = 0; i < this.Items.Count; i++)
			{
				if (this.Items[i] == v)
				{
					this.SelectedIndex = i;
					return;
				}
			}
			this.SelectedIndex = -1;
		}
		/// <summary>
		/// field ordinate position
		/// </summary>
		/// <returns>-1:to use field associated; >=0: field ordinate position</returns>
		public virtual int GetUpdateFieldIndex()
		{
			return nUpdatePos;
		}
		public virtual void SetUpdateFieldIndex(int nPos)
		{
			nUpdatePos = nPos;
		}
	}

	public class ComboLookupConsts : ComboLook
	{
		public ComboLookupConsts()
		{
		}
		public void AddValue(string obj)
		{
			this.Items.Add(obj);
		}
		public override object GetLookupData()
		{
			int n = this.SelectedIndex;
			if (n >= 0)
			{
				//string obj = this.Items[n] as string;
				//return obj;
				return this.Items[n];
			}
			return null;
		}
		public override void SetSelectedIndex(object v)
		{
			string obj;
			string v0 = null;
			if (v != null)
			{
				v0 = v.ToString();
			}
			for (int i = 0; i < this.Items.Count; i++)
			{
				obj = this.Items[i] as string;
				if (obj != null)
				{
					if (string.CompareOrdinal(obj, v0) == 0)
					{
						this.SelectedIndex = i;
						return;
					}
				}
				else
				{
					if (v0 == null)
					{
						this.SelectedIndex = i;
						return;
					}
				}
			}
			this.SelectedIndex = -1;
		}
	}
	public class ComboLookupData : ComboLook
	{
		protected System.Data.DataSet ds = null;

		public ComboLookupData()
		{
		}
		public bool LoadData(EasyQuery qry, string displayField)
		{
			try
			{
				bool bLoaded = false;
				qry.ResetCanChangeDataSet(true);
				qry.Query();
				bLoaded = true;
				ds = qry.DataStorage;
				if (ds != null && ds.Tables.Count > 0 && bLoaded)
				{
					if (ds.Tables[0].Columns.Count > 0)
					{
						int fldIdx = 0;
						this.DataSource = ds;
						if (!string.IsNullOrEmpty(displayField))
						{
							for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
							{
								if (string.Compare(displayField, ds.Tables[0].Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									fldIdx = i;
									break;
								}
							}
						}
						this.DisplayMember = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}.{1}", ds.Tables[0].TableName, ds.Tables[0].Columns[fldIdx].ColumnName);
						return true;
					}
				}
			}
			catch (Exception er)
			{
				FormLog.NotifyException(true, er);
			}
			return false;
		}
		/// <summary>
		/// returns a DataRow so that multiple columns can be updated
		/// </summary>
		/// <returns></returns>
		public override object GetLookupData()
		{
			int n = this.SelectedIndex;
			if (n >= 0)
			{
				if (ds != null)
				{
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Columns.Count > 1)
						{
							return ds.Tables[0].Rows[n];
						}
					}
				}
				return this.Items[n];
			}
			return null;
		}
		public override void SetSelectedIndex(object v)
		{
			if (ds != null && this.Items.Count > 0)
			{
				if (ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Columns.Count > 1)
					{
						bool bEQ = false;
						for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
						{
							if (v == null)
							{
								if (ds.Tables[0].Rows[i][1] == null)
									bEQ = true;
							}
							else if (v == System.DBNull.Value)
							{
								if (ds.Tables[0].Rows[i][1] == System.DBNull.Value)
									bEQ = true;
							}
							else if (v.Equals(ds.Tables[0].Rows[i][1]))
								bEQ = true;
							if (bEQ)
							{
								this.SelectedIndex = i;
								return;
							}
						}
					}
				}
			}
			this.SelectedIndex = -1;
		}
	}

}
