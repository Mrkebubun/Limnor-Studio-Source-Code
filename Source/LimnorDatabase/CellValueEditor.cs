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
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Drawing;
using VPL;
using System.ComponentModel;
using Limnor.WebBuilder;

namespace LimnorDatabase
{
	[ToolboxBitmapAttribute(typeof(DataEditorDatetime), "Resources.calendar.bmp")]
	public class DataEditorDatetime : DataEditorButton
	{
		public DataEditorDatetime()
		{
		}
		public bool UseLargeDialogue { get; set; }
		public override string ToString()
		{
			return "Date Selector";
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			DataEditorDatetime ded = current as DataEditorDatetime;
			if (ded == null)
			{
				ded = new DataEditorDatetime();
			}
			return new DlgSelectorOptionDateTime(ded);
		}
		public override Button MakeButton(Form owner)
		{
			Button bt = base.MakeButton(owner);
			bt.Click += new System.EventHandler(onButtonClick);
			return bt;
		}
		protected override void OnClone(DataEditor cloned)
		{
			DataEditorDatetime ed = cloned as DataEditorDatetime;
			if (ed != null)
			{
				ed.UseLargeDialogue = this.UseLargeDialogue;
			}
		}
		void onButtonClick(object sender, System.EventArgs e)
		{
			if (UseLargeDialogue)
			{
				dlgSelectDateTime dlg = new dlgSelectDateTime();
				System.DateTime dt = ValueConvertor.ToDateTime(currentValue);
				dlg.LoadData(dt);
				if (dlg.ShowDialog(OwnerForm) == DialogResult.OK)
				{
					FirePickValue(dlg.dRet);
				}
			}
			else
			{
				dlgSelectDateTimeSmall dlg = new dlgSelectDateTimeSmall();
				System.DateTime dt = ValueConvertor.ToDateTime(currentValue);
				dlg.LoadData(dt);
				if (dlg.ShowDialog(OwnerForm) == DialogResult.OK)
				{
					FirePickValue(dlg.dRet);
				}
			}
		}
	}

	[ToolboxBitmapAttribute(typeof(DataEditorFile), "Resources.file.bmp")]
	public class DataEditorFile : DataEditorButton
	{
		public DataEditorFile()
		{
		}
		public override string ToString()
		{
			return "File name selector";
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return null;
		}
		public override Button MakeButton(Form owner)
		{
			Button bt = base.MakeButton(owner);
			bt.Click += new System.EventHandler(onButtonClick);
			return bt;
		}
		void onButtonClick(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (this.currentValue != null)
			{
				try
				{
					string s = this.currentValue.ToString();
					if (System.IO.File.Exists(s))
					{
						dlg.FileName = s;
					}
				}
				catch
				{
				}
			}
			if (dlg.ShowDialog(OwnerForm) == DialogResult.OK)
			{
				FirePickValue(dlg.FileName);
			}
		}
	}

	public class DataEditorLookupYesNo : DataEditor
	{
		public DataEditorLookupYesNo()
		{
		}
		public virtual System.Windows.Forms.ComboBox MakeComboBox()
		{
			ComboLook cb = new ComboLook();
			cb.SetUpdateFieldIndex(this.nUpdatePos);
			return cb;
		}
	}

	[ToolboxBitmapAttribute(typeof(DataEditorLookup), "Resources.dropDown.bmp")]
	public class DataEditorLookup : DataEditorLookupYesNo
	{
		public string[] values = null;
		public DataEditorLookup()
		{
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return new DlgEditNameValueList(current);
		}
		public override ComboBox MakeComboBox()
		{
			ComboLookupConsts obj = new ComboLookupConsts();
			obj.SetUpdateFieldIndex(this.nUpdatePos);
			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
					obj.Items.Add(values[i]);
			}
			return obj;
		}
		#region ICloneable Members
		protected override void OnClone(DataEditor cloned)
		{
			DataEditorLookup obj = cloned as DataEditorLookup;
			if (values != null)
			{
				obj.values = new string[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					obj.values[i] = values[i];
				}
			}
		}

		#endregion

		public override string ToString()
		{
			return "Options";
		}
	}

	[ToolboxBitmapAttribute(typeof(DataEditorLookupEnum), "Resources._enum.bmp")]
	public class DataEditorLookupEnum : DataEditorLookupYesNo
	{
		private Type _enumType;
		public DataEditorLookupEnum()
		{
		}
		public Type EnumType
		{
			get
			{
				return _enumType;
			}
		}
		public void SetType(Type t)
		{
			_enumType = t;
		}
		public string TypeString
		{
			get
			{
				if (_enumType != null)
				{
					return _enumType.AssemblyQualifiedName;
				}
				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_enumType = Type.GetType(value);
				}
			}
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return new DlgFieldEditorEnum(current);
		}
		public override ComboBox MakeComboBox()
		{
			ComboLookupConsts obj = new ComboLookupConsts();
			obj.SetUpdateFieldIndex(this.nUpdatePos);
			if (EnumType != null && EnumType.IsEnum)
			{
				Array values = Enum.GetValues(EnumType);
				if (values != null && values.Length > 0)
				{
					for (int i = 0; i < values.Length; i++)
						obj.Items.Add(values.GetValue(i));
				}
			}
			return obj;
		}
		#region ICloneable Members
		protected override void OnClone(DataEditor cloned)
		{
			DataEditorLookupEnum obj = cloned as DataEditorLookupEnum;
			obj._enumType = _enumType;
		}

		#endregion

		public override string ToString()
		{
			return "Enumerator";
		}
	}

	[ToolboxBitmapAttribute(typeof(DataEditorLookupDB), "Resources.qry.bmp")]
	public class DataEditorLookupDB : DataEditorLookupYesNo
	{
		private EasyQuery _qry; //for getting lookup data
		private DataBind _maps;
		public DataEditorLookupDB()
		{
		}
		private string displayFieldName
		{
			get
			{
				string displayName = string.Empty;
				if (!string.IsNullOrEmpty(this.ValueField))
				{
					if (this.valuesMaps != null)
					{
						displayName = this.valuesMaps.GetMappedField(this.ValueField);
					}
				}
				return displayName;
			}
		}
		protected override void OnGetTypeForXmlSerializarion(List<Type> types)
		{
			base.OnGetTypeForXmlSerializarion(types);
			if (!types.Contains(typeof(EasyQuery)))
				types.Add(typeof(EasyQuery));
			if (!types.Contains(typeof(ConnectionItem)))
				types.Add(typeof(ConnectionItem));
			if (!types.Contains(typeof(SQLStatement)))
				types.Add(typeof(SQLStatement));
			if (!types.Contains(typeof(DataBind)))
				types.Add(typeof(DataBind));
			if (!types.Contains(typeof(StringMapList)))
				types.Add(typeof(StringMapList));
			if (!types.Contains(typeof(StringMap)))
				types.Add(typeof(StringMap));
		}
		//first field=display; second field=value
		public EasyQuery Query
		{
			get
			{
				if (_qry == null)
				{
					_qry = new EasyQuery();
					_qry.ForReadOnly = true;
				}
				return _qry;
			}
			set
			{
				_qry = value;
			}
		}
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				if (_qry != null)
				{
					return _qry.DatabaseConnectionsUsed;
				}
				return new List<Guid>();
			}
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				if (_qry != null)
				{
					return _qry.DatabaseConnectionTypesUsed;
				}
				return new List<Type>();
			}
		}
		public DataBind valuesMaps
		{
			get
			{
				if (_maps == null)
				{
					_maps = new DataBind();
				}
				return _maps;
			}
			set
			{
				_maps = value;
			}
		}
		public override void OnLoad()
		{
			if (_qry != null)
			{
				_qry.ReloadConnection();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flds">destination fields</param>
		public override void SetFieldsAttribute(IFieldList flds)
		{
			base.SetFieldsAttribute(flds);
			if (_maps == null)
			{
				_maps = new DataBind();
			}
			if (_maps.AdditionalJoins == null)
			{
				_maps.AdditionalJoins = new StringMapList();
			}
			//merge mapping
			for (int i = 0; i < flds.Count; i++)
			{
				bool b = false;
				for (int j = 0; j < _maps.AdditionalJoins.Count; j++)
				{
					if (string.Compare(flds.GetFieldname(i), _maps.AdditionalJoins[j].Target, StringComparison.OrdinalIgnoreCase) == 0)
					{
						b = true;
						break;
					}
				}
				if (!b)
				{
					_maps.AdditionalJoins.AddFieldMap(flds.GetFieldname(i), string.Empty);
				}
			}
			List<int> ns = new List<int>();
			for (int j = 0; j < _maps.AdditionalJoins.Count; j++)
			{
				bool b = false;
				for (int i = 0; i < flds.Count; i++)
				{
					if (string.Compare(flds.GetFieldname(i), _maps.AdditionalJoins[j].Target, StringComparison.OrdinalIgnoreCase) == 0)
					{
						b = true;
						break;
					}
				}
				if (b)
				{
					ns.Add(j);
				}
			}
			if (ns.Count < _maps.AdditionalJoins.Count)
			{
				int n = 0;
				StringMap[] sm = new StringMap[ns.Count];
				for (int j = 0; j < _maps.AdditionalJoins.Count; j++)
				{
					if (ns.Contains(j))
					{
						sm[n++] = _maps.AdditionalJoins[j];
					}
				}
				_maps.AdditionalJoins.StringMaps = sm;
			}
		}
		public static DlgSetEditorAttributes GetDatabaseLookupDataDialog(DataEditor current, DataEditor caller)
		{
			DataEditorLookupDB dbe0 = caller as DataEditorLookupDB;
			if (dbe0 != null)
			{
				DataEditorLookupDB dbe = current as DataEditorLookupDB;
				if (dbe == null)
				{
					dbe = new DataEditorLookupDB();
					dbe.valuesMaps.AdditionalJoins = dbe0.valuesMaps.AdditionalJoins;
				}
				else
				{
					if (dbe0.valuesMaps.AdditionalJoins != null)
					{
						for (int i = 0; i < dbe0.valuesMaps.AdditionalJoins.Count; i++)
						{
							if (string.IsNullOrEmpty(dbe0.valuesMaps.AdditionalJoins[i].Source))
							{
								string val = dbe.valuesMaps.GetMappedField(dbe0.valuesMaps.AdditionalJoins[i].Target);
								if (!string.IsNullOrEmpty(val))
								{
									dbe0.valuesMaps.AdditionalJoins[i].Source = val;
								}
							}
						}
						dbe.valuesMaps.AdditionalJoins = dbe0.valuesMaps.AdditionalJoins;
					}
				}
				return new DlgDataEditDatabaseLookup(dbe);
			}
			else
			{
				WebDataEditorLookupDB wd0 = caller as WebDataEditorLookupDB;
				if (wd0 != null)
				{
					WebDataEditorLookupDB wd = current as WebDataEditorLookupDB;
					if (wd == null)
					{
						wd = (WebDataEditorLookupDB)wd0.Clone();
					}
					else
					{
						if (wd0.FieldsMap != null)
						{
							wd.FieldsMap = wd0.FieldsMap;
						}
					}
					return new DlgDataEditDatabaseLookup(wd);
				}
			}
			return null;
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return GetDatabaseLookupDataDialog(current, this);
		}
		public override ComboBox MakeComboBox()
		{
			if (Query.DatabaseConnection != null)
			{
				string sSQL = Query.MakeSelectionQuery(null);
				if (!string.IsNullOrEmpty(sSQL))
				{
					ComboLookupData cbx = new ComboLookupData();
					cbx.SetUpdateFieldIndex(this.nUpdatePos);
					if (cbx.LoadData(Query, this.displayFieldName))
					{
						return cbx;
					}
				}
			}
			return null;
		}
		public override void UpdateComboBox(ComboBox cbb)
		{
			ComboLookupData cbx = cbb as ComboLookupData;
			if (cbx != null && Query.DatabaseConnection != null)
			{
				string sSQL = Query.MakeSelectionQuery(null);
				if (!string.IsNullOrEmpty(sSQL))
				{
					string s = cbx.Text;
					if (cbx.LoadData(Query, displayFieldName))
					{
						cbx.SelectedIndex = cbx.FindStringExact(s);
					}
				}
			}
		}
		#region ICloneable Members
		protected override void OnClone(DataEditor cloned)
		{
			DataEditorLookupDB obj = cloned as DataEditorLookupDB;
			obj._qry = (EasyQuery)Query.Clone();
			if (valuesMaps != null)
			{
				obj.valuesMaps = (DataBind)valuesMaps.Clone();
			}
		}

		#endregion

		public override string ToString()
		{
			return "Database lookup";
		}

	}

}
