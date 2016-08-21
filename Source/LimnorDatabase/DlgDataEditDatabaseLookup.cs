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
using System.Globalization;

namespace LimnorDatabase
{
	public partial class DlgDataEditDatabaseLookup : DlgSetEditorAttributes
	{
		private DataBind objRet = null;
		private EasyQuery _webQuery = null;
		//
		private ComboBox cbx2;
		private DataSet ds = new DataSet("Relation");
		public DlgDataEditDatabaseLookup()
		{
			InitializeComponent();
		}
		public DlgDataEditDatabaseLookup(WebDataEditorLookupDB editor)
			: base(editor)
		{
			InitializeComponent();
			//
			if (editor == null)
			{
				editor = new WebDataEditorLookupDB();
				SetSelection(editor);
			}
			loadData();
		}
		public DlgDataEditDatabaseLookup(DataEditorLookupDB editor)
			: base(editor)
		{
			InitializeComponent();
			//
			if (editor == null)
			{
				editor = new DataEditorLookupDB();
				SetSelection(editor);
			}
			LoadData(editor.valuesMaps);
			if (editor.Query == null)
			{
				editor.Query = new EasyQuery();
			}
			editor.Query.SqlChanged += new EventHandler(qry_SqlChanged);
			propertyGrid1.SelectedObject = editor.Query;
			editor.Query.SetLoaded();
		}
		public WebDataEditorLookupDB WebLookup
		{
			get
			{
				return SelectedEditor as WebDataEditorLookupDB;
			}
		}
		public void loadData()
		{
			const string TABLENAME = "Links";
			//
			WebDataEditorLookupDB wd = WebLookup;
			//
			ds.Tables.Clear();
			ds.Tables.Add(TABLENAME);
			//
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			//
			ds.Tables[0].Columns[0].Caption = "Fields to be Updated";
			ds.Tables[0].Columns[0].ColumnName = "Destination";
			ds.Tables[0].Columns[0].DataType = typeof(string);
			ds.Tables[0].Columns[0].MaxLength = 120;
			ds.Tables[0].Columns[0].ReadOnly = true;
			//
			ds.Tables[0].Columns[1].Caption = "Source Fields";
			ds.Tables[0].Columns[1].ColumnName = "Source";
			ds.Tables[0].Columns[1].DataType = typeof(string);
			ds.Tables[0].Columns[1].MaxLength = 120;
			//
			//
			dataGridView1.DataSource = ds;
			dataGridView1.DataMember = TABLENAME;
			dataGridView1.ReadOnly = true;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			//
			cbx2 = new System.Windows.Forms.ComboBox();
			cbx2.Parent = dataGridView1;
			cbx2.Visible = false;
			cbx2.Left = 0;
			cbx2.SelectedIndexChanged += new EventHandler(cbx2_SelectedIndexChanged);
			//
			_webQuery = new EasyQuery();
			_webQuery.ConnectionID = wd.ConnectionID;
			_webQuery.ForReadOnly = true;
			_webQuery.SQL = new SQLStatement(wd.SqlString);
			_webQuery.Description = "Database lookup";
			_webQuery.SqlChanged += new EventHandler(_webQuery_SqlChanged);
			propertyGrid1.SelectedObject = _webQuery;
			_webQuery.SetLoaded();
			//
			loadWebDataFields();
		}

		void _webQuery_SqlChanged(object sender, EventArgs e)
		{
			WebDataEditorLookupDB wd = WebLookup;
			wd.SqlString = _webQuery.SqlQuery;
			wd.ConnectionID = _webQuery.ConnectionID;
			loadWebDataFields();
		}
		private void loadWebDataFields()
		{
			WebDataEditorLookupDB wd = WebLookup;
			cbx2.Items.Clear();

			int i;
			FieldsParser fp = new FieldsParser();
			string[] fields = null;
			string msg = string.Empty;
			if (!string.IsNullOrEmpty(wd.SqlString))
			{
				if (fp.ParseQuery(wd.SqlString, _webQuery.NameDelimiterBegin, _webQuery.NameDelimiterEnd, ref msg))
				{
					fields = new string[fp.Count];
					for (i = 0; i < fields.Length; i++)
					{
						fields[i] = fp.Fields[i].FieldName;
					}
				}
				else
				{
					MessageBox.Show(string.Format(CultureInfo.InstalledUICulture, "{0}. Error parsing SQL Statement [{1}].", msg, wd.SqlString), "Parse SQL Statement");
				}
			}
			int n = wd.Hold.FieldCount;
			cbx2.Items.Add("");
			if (fields != null)
			{
				for (i = 0; i < fields.Length; i++)
				{
					cbx2.Items.Add(fields[i]);
				}
			}
			if (n == 0)
			{
				MessageBox.Show("Linked data source not found (DestinationFieldCount is 0)");
				return;
			}
			else
			{
				ds.Tables[0].Rows.Clear();
				object[] vs;
				for (i = 0; i < n; i++)
				{
					string nm = wd.Hold.GetFieldNameByIndex(i);
					vs = new object[2];
					vs[0] = nm; //field to be updated
					vs[1] = string.Empty;
					if (wd.FieldsMap != null)
					{
						for (int j = 0; j < wd.FieldsMap.Count; j++)
						{
							if (string.Compare(wd.FieldsMap[j].Target, nm, StringComparison.OrdinalIgnoreCase) == 0)
							{
								bool ok = false;
								if (fields != null)
								{
									for (int k = 0; k < fields.Length; k++)
									{
										if (string.Compare(fields[k], wd.FieldsMap[j].Source, StringComparison.OrdinalIgnoreCase) == 0)
										{
											ok = true;
											break;
										}
									}
								}
								if (ok)
								{
									vs[1] = wd.FieldsMap[j].Source;
								}
								break;
							}
						}
					}
					ds.Tables[0].Rows.Add(vs);
				}
			}
		}
		void qry_SqlChanged(object sender, EventArgs e)
		{
			DataEditorLookupDB edb = (DataEditorLookupDB)(this.SelectedEditor);
			FieldList fl = edb.Query.Fields;
			objRet.SourceFields = new string[fl.Count];
			for (int i = 0; i < objRet.SourceFields.Length; i++)
			{
				objRet.SourceFields[i] = fl[i].Name;
			}
			loadFields();
		}
		public void LoadData(DataBind dbd)
		{
			const string TABLENAME = "Links";
			objRet = dbd;
			//
			ds.Tables.Clear();
			ds.Tables.Add(TABLENAME);
			//
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			//
			ds.Tables[0].Columns[0].Caption = "Fields to be Updated";
			ds.Tables[0].Columns[0].ColumnName = "Destination";
			ds.Tables[0].Columns[0].DataType = typeof(string);
			ds.Tables[0].Columns[0].MaxLength = 120;
			ds.Tables[0].Columns[0].ReadOnly = true;
			//
			ds.Tables[0].Columns[1].Caption = "Source Fields";
			ds.Tables[0].Columns[1].ColumnName = "Source";
			ds.Tables[0].Columns[1].DataType = typeof(string);
			ds.Tables[0].Columns[1].MaxLength = 120;
			//
			//
			dataGridView1.DataSource = ds;
			dataGridView1.DataMember = TABLENAME;
			dataGridView1.ReadOnly = true;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			//
			cbx2 = new System.Windows.Forms.ComboBox();
			cbx2.Parent = dataGridView1;
			cbx2.Visible = false;
			cbx2.Left = 0;
			cbx2.SelectedIndexChanged += new EventHandler(cbx2_SelectedIndexChanged);
			//
			loadFields();
		}
		private void loadFields()
		{
			cbx2.Items.Clear();
			int i;
			int n = objRet.DestinationFieldCount;
			cbx2.Items.Add("");
			for (i = 0; i < objRet.SourceFieldCount; i++)
			{
				cbx2.Items.Add(objRet.SourceFields[i]);
			}
			if (n == 0)
			{
				MessageBox.Show("Linked data source not found (DestinationFieldCount is 0)");
				return;
			}
			else
			{
				ds.Tables[0].Rows.Clear();
				object[] vs;
				for (i = 0; i < n; i++)
				{
					vs = new object[2];
					vs[0] = objRet.AdditionalJoins[i].Target; //field to be updated
					if (objRet.IsSourceValid(objRet.AdditionalJoins[i].Source))
					{
						vs[1] = objRet.AdditionalJoins[i].Source; //source field
					}
					ds.Tables[0].Rows.Add(vs);
				}
			}
		}
		private void cbx2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbx2.SelectedIndex;
			if (n < 0)
				return;
			if (dataGridView1.CurrentCell.RowIndex < ds.Tables[0].Rows.Count)
			{
				ds.Tables[0].Rows[dataGridView1.CurrentCell.RowIndex].BeginEdit();
				ds.Tables[0].Rows[dataGridView1.CurrentCell.RowIndex][1] = cbx2.Items[n].ToString();
				ds.Tables[0].Rows[dataGridView1.CurrentCell.RowIndex].EndEdit();
			}
		}

		public override void SetEditorAttributes(DataEditor current)
		{
			DataEditorLookupDB ddb = current as DataEditorLookupDB;
			if (ddb != null)
			{
			}
		}

		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (dataGridView1.CurrentCell.RowIndex >= 0)
			{
				if (dataGridView1.CurrentCell.ColumnIndex == 1)
				{
					System.Drawing.Rectangle rc = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, true);
					cbx2.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
					cbx2.SelectedIndex = -1;
					if (dataGridView1.CurrentCell.RowIndex < ds.Tables[0].Rows.Count)
					{
						string s = ValueConvertor.ToString(ds.Tables[0].Rows[dataGridView1.CurrentCell.RowIndex][dataGridView1.CurrentCell.ColumnIndex]);
						for (int i = 0; i < cbx2.Items.Count; i++)
						{
							if (string.CompareOrdinal(ValueConvertor.ToString(cbx2.Items[i]), s) == 0)
							{
								cbx2.SelectedIndex = i;
								break;
							}
						}
					}
					cbx2.Visible = true;
					cbx2.BringToFront();
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			StringMapList maps = new StringMapList();
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				string val = ValueConvertor.ToString(ds.Tables[0].Rows[i][1]);
				if (!string.IsNullOrEmpty(val))
				{
					maps.AddFieldMap(ValueConvertor.ToString(ds.Tables[0].Rows[i][0]), val);
				}
			}
			DataEditorLookupDB edb = this.SelectedEditor as DataEditorLookupDB;
			if (edb != null)
			{
				edb.valuesMaps.AdditionalJoins = maps;
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				WebDataEditorLookupDB wd = this.SelectedEditor as WebDataEditorLookupDB;
				if (wd != null)
				{
					wd.FieldsMap = maps;
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
		{
			cbx2.Visible = false;
		}
	}
}
