/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Data;

namespace LimnorDatabase
{
	[TypeConverter(typeof(TypeConverterDataBatch))]
	public class DataBatch
	{
		private bool _enabled;
		private EasyDataSet _owner;
		public DataBatch()
		{
		}
		public DataBatch(EasyDataSet ds)
		{
			_owner = ds;
		}
		public void SetOwner(EasyDataSet ds)
		{
			_owner = ds;
		}
		public EasyDataSet GetOwner()
		{
			return _owner;
		}
		public bool IsOrderDesc
		{
			get
			{
				if (_owner != null)
				{
					if (!string.IsNullOrEmpty(_owner.OrderBy))
					{
						string s = _owner.OrderBy.Trim();
						if (s.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		[Description("Gets and sets the number of records for each batch")]
		public int BatchSize { get; set; }

		[Editor(typeof(TypeSelectorSelectDbType), typeof(UITypeEditor))]
		[Description("The data type for KeyField")]
		public DbType KeyFieldType { get; set; }

		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating the time delay, in milliseconds, between batches. The time delay makes the web page more responsive.")]
		public int BatchDelay { get; set; }

		[Description("Gets and sets a Boolean value indicating whether data streaming is enabled.")]
		public bool Enabled 
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (value)
				{
					if (_owner != null)
					{
						if (string.IsNullOrEmpty(_owner.OrderBy))
						{
							MessageBox.Show("Cannot enable data streaming because ORDER BY is not used in the query", "Data streaming");
						}
						else if (!string.IsNullOrEmpty(_owner.DataPreparer))
						{
							MessageBox.Show("Cannot enable data streaming because DataPreparer is used.", "Data streaming");
						}
						else
						{
							_enabled = value;
						}
					}
					else
					{
						_enabled = value;
					}
				}
				else
				{
					_enabled = value;
				}
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "BatchSize:{0},Enabled:{2}", BatchSize, Enabled);
		}
		public bool DataBatchingAllowed()
		{
			if (_owner != null)
				return BatchSize > 0 && !string.IsNullOrEmpty(_owner.OrderBy) && Enabled;
			return false;
		}
	}
	class TypeConverterDataBatch : ExpandableObjectConverter
	{
		public TypeConverterDataBatch()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				DataBatch db = new DataBatch();
				string s = value as string;
				if (!string.IsNullOrEmpty(s))
				{
					int pos = s.IndexOf(',');
					if (pos >= 0)
					{
						string s0 = s.Substring(0, pos);
						s = s.Substring(pos + 1);
						if (!string.IsNullOrEmpty(s0))
						{
							int n;
							if (int.TryParse(s0, out n))
							{
								db.BatchSize = n;
							}
						}
						pos = s.IndexOf(',');
						if (pos >= 0)
						{
							s0 = s.Substring(0, pos);
							s = s.Substring(pos + 1);
							if (!string.IsNullOrEmpty(s0))
							{
								bool b;
								if (bool.TryParse(s0, out b))
								{
									db.Enabled = b;
								}
							}
							if (!string.IsNullOrEmpty(s))
							{
								pos = s.IndexOf(',');
								if (pos >= 0)
								{
									s0 = s.Substring(0, pos);
									s = s.Substring(pos + 1);
									db.KeyFieldType = (DbType)Enum.Parse(typeof(DbType), s0);
									if (!string.IsNullOrEmpty(s))
									{
										int n0;
										if (int.TryParse(s, out n0))
										{
											db.BatchDelay = n0;
										}
									}
								}
								else
								{
									db.KeyFieldType = (DbType)Enum.Parse(typeof(DbType), s);
								}
							}
						}
					}
				}
				return db;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			DataBatch db = value as DataBatch;
			if (db != null)
			{
				if (typeof(string).Equals(destinationType))
				{
					return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", db.BatchSize, db.Enabled, db.KeyFieldType, db.BatchDelay);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	class TypeSelectorSelectDbType : UITypeEditor
	{
		public TypeSelectorSelectDbType()
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
					DbType t = (DbType)value;
					DbTypeList list = new DbTypeList(edSvc,t);
					edSvc.DropDownControl(list);
					if (list.MadeSelection)
					{
						value = list.Selection;
					}
				}
			}
			return value;
		}
		class DbTypeList : ListBox
		{
			public bool MadeSelection;
			public DbType Selection;
			private IWindowsFormsEditorService _service;
			public DbTypeList(IWindowsFormsEditorService service, DbType v)
			{
				_service = service;
				Array tps = Enum.GetValues(typeof(DbType));
				for (int i = 0; i < tps.Length; i++)
				{
					int n = Items.Add(tps.GetValue(i));
					if (v == (DbType)tps.GetValue(i))
					{
						this.SelectedIndex = n;
					}
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = (DbType)Items[SelectedIndex];
				}
				_service.CloseDropDown();
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				finishSelection();

			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					finishSelection();
				}
			}
		}
	}
}
