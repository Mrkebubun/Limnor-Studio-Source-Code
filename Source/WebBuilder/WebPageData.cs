/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using XmlUtility;
using VPL;
using System.Data;

namespace Limnor.WebBuilder
{
	/// <summary>
	/// multi-language data is in datatable's Rows
	/// </summary>
	public class WebPageDataSet : ICloneable
	{
		private WebDataTable[] _data;
		public WebPageDataSet()
		{
		}
		public void SetData(WebDataTable[] data)
		{
			_data = data;
		}
		public void SetCurrentLanguage(string name)
		{
			if (_data != null)
			{
				for (int i = 0; i < _data.Length; i++)
				{
					_data[i].CurrentCulture = name;
				}
			}
		}
		public WebDataTable[] GetData()
		{
			if (_data == null)
			{
				_data = new WebDataTable[] { };
			}
			return _data;
		}
		public void RemoveData(string tableName)
		{
			if (_data != null && !string.IsNullOrEmpty(tableName))
			{
				int n = -1;
				for (int i = 0; i < _data.Length; i++)
				{
					if (string.Compare(_data[i].TableName, tableName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						n = i;
						break;
					}
				}
				if (n >= 0)
				{
					WebDataTable[] a = new WebDataTable[_data.Length - 1];
					for (int i = 0; i < _data.Length; i++)
					{
						if (i < n)
							a[i] = _data[i];
						else if (i > n)
							a[i - 1] = _data[i];
					}
					_data = a;
				}
			}
		}
		public const string XML_Item = "Item";
		public const string XMLATT_dtype = "dataType";
		public void ReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			XmlNodeList ns = node.SelectNodes(XML_Item);
			_data = new WebDataTable[ns.Count];
			for (int i = 0; i < ns.Count; i++)
			{
				int dt = XmlUtil.GetAttributeInt(ns[i], XMLATT_dtype);
				if (dt == 1)
				{
					_data[i] = new WebDataTableSingleRow(this);
				}
				else
				{
					_data[i] = new WebDataTable(this);
				}
				_data[i].ReadFromXmlNode(reader, ns[i]);
			}
		}

		public void WriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (_data != null)
			{
				for (int i = 0; i < _data.Length; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XML_Item);
					node.AppendChild(nd);
					_data[i].WriteToXmlNode(writer, nd);
				}
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			WebPageDataSet d = new WebPageDataSet();
			if (_data != null)
			{
				d._data = new WebDataTable[_data.Length];
				for (int i = 0; i < _data.Length; i++)
				{
					d._data[i] = _data[i].Clone(d);
				}
			}
			return d;
		}

		#endregion
		public override string ToString()
		{
			if (_data != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "Data Table contained:{0}", _data.Length);
			}
			return string.Empty;
		}
	}
	public class WebDataRelation
	{
		public WebDataRelation()
		{
		}
		public string ChildTableName; //name of the detail table name
		public string PrimaryKey; //string array forming key on main table
		public string ForeignKey; //string array forming key on detail table 
		public WebDataRelation Clone()
		{
			WebDataRelation r = new WebDataRelation();
			r.ChildTableName = ChildTableName;
			r.PrimaryKey = PrimaryKey;
			r.ForeignKey = ForeignKey;
			return r;
		}
		public void ReadFromXmlNode(XmlNode node)
		{

		}

		public void WriteToXmlNode(XmlNode node)
		{
		}
	}
	public class WebPageDataRow //a record of data passing from server (source) to client (target)
	{
		const string XML_Values = "Values";
		public WebPageDataRow()
		{
		}
		public object[] ItemArray; //an array of values
		public WebDataTable[] ChildTables; //an array of JsonDataTable. Get relations from JsonDataTable identified by table name
		public WebPageDataRow Clone()
		{
			WebPageDataRow r = new WebPageDataRow();
			if (ChildTables != null)
			{
				r.ChildTables = new WebDataTable[ChildTables.Length];
				for (int i = 0; i < ChildTables.Length; i++)
				{
					r.ChildTables[i] = ChildTables[i];
				}
			}
			if (ItemArray != null)
			{
				r.ItemArray = new object[ItemArray.Length];
				for (int i = 0; i < ItemArray.Length; i++)
				{
					ICloneable ic = ItemArray[i] as ICloneable;
					if (ic != null)
					{
						r.ItemArray[i] = ic.Clone();
					}
					else
					{
						r.ItemArray[i] = ItemArray[i];
					}
				}
			}
			return r;
		}
		public void ReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			XmlNodeList ns = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}/{1}", XML_Values, WebPageDataSet.XML_Item));
			ItemArray = new object[ns.Count];
			for (int i = 0; i < ns.Count; i++)
			{
				ItemArray[i] = reader.ReadValue(ns[i]);
			}
		}

		public void WriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (ItemArray != null)
			{
				XmlNode ns = XmlUtil.CreateSingleNewElement(node, XML_Values);
				ns.RemoveAll();
				for (int i = 0; i < ItemArray.Length; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(WebPageDataSet.XML_Item);
					ns.AppendChild(nd);
					writer.WriteValue(nd, ItemArray[i], null);
				}
			}
		}
	}

	public class WebDataColumn
	{
		public string ColumnName; //string
		public bool ReadOnly; //Boolean
		public string Type; //string: int,string,datetime,float
		public Type SystemType
		{
			get
			{
				if (string.IsNullOrEmpty(Type))
				{
					return typeof(string);
				}
				else if (string.Compare("Integer", Type, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return typeof(int);
				}
				else if (string.Compare("Datetime", Type, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return typeof(DateTime);
				}
				return typeof(string);
			}
		}
		const string XMLATT_type = "vtype";
		const string XMLATT_readonly = "readOnly";
		public void ReadFromXmlNode(XmlNode node)
		{
			ColumnName = XmlUtil.GetNameAttribute(node);
			ReadOnly = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_readonly);
			Type = XmlUtil.GetAttribute(node, XMLATT_type);
		}

		public void WriteToXmlNode(XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, ColumnName);
			XmlUtil.SetAttribute(node, XMLATT_readonly, ReadOnly);
			XmlUtil.SetAttribute(node, XMLATT_type, Type);
		}
		public WebDataColumn Clone()
		{
			WebDataColumn c = new WebDataColumn();
			c.ColumnName = ColumnName;
			c.ReadOnly = ReadOnly;
			c.Type = Type;
			return c;
		}
	}

	public class WebDataTable
	{
		#region fields and constructors
		private WebPageDataSet _dataset;
		private Dictionary<string, DataTable> _dataTables;
		public WebDataTable(WebPageDataSet dataSet)
		{
			_dataset = dataSet;
		}
		#endregion
		#region Properties
		public string TableName { get; set; }
		public WebDataColumn[] Columns { get; set; }
		public string PrimaryKey { get; set; }
		public WebDataRelation[] DataRelations { get; set; }
		protected Dictionary<string, WebPageDataRow[]> Rows { get; set; }
		public string CurrentCulture { get; set; }
		#endregion
		//
		#region Methods
		public DataTable CreateDataTable(string cultureName)
		{
			if (_dataTables == null)
			{
				_dataTables = new Dictionary<string, DataTable>();
			}
			DataTable dt;
			if (!_dataTables.TryGetValue(cultureName, out dt))
			{
				dt = new DataTable(this.TableName);
				_dataTables.Add(cultureName, dt);
			}
			if (Columns == null || Columns.Length == 0)
			{
				dt.Rows.Clear();
				dt.Columns.Clear();
			}
			else
			{
				if (dt.Columns.Count == 0)//includes new table situation
				{
					for (int i = 0; i < Columns.Length; i++)
					{
						dt.Columns.Add(Columns[i].ColumnName, Columns[i].SystemType);
					}
					//no existing data. generate rows from original data
					if (Rows != null)
					{
						WebPageDataRow[] rows;
						if (Rows.TryGetValue(cultureName, out rows))
						{
							if (rows != null)
							{
								for (int i = 0; i < rows.Length; i++)
								{
									if (rows[i].ItemArray != null)
									{
										if (rows[i].ItemArray.Length < Columns.Length)
										{
											object[] vs = new object[Columns.Length];
											for (int j = 0; j < rows[i].ItemArray.Length; j++)
											{
												vs[j] = rows[i].ItemArray[j];
											}
											rows[i].ItemArray = vs;
										}
										else if (rows[i].ItemArray.Length > Columns.Length)
										{
											object[] vs = new object[Columns.Length];
											for (int j = 0; j < vs.Length; j++)
											{
												vs[j] = rows[i].ItemArray[j];
											}
											rows[i].ItemArray = vs;
										}
										dt.Rows.Add(rows[i].ItemArray);
									}
								}
							}
						}
					}
				}
				else
				{
					//verify data in data table
					bool changed = true;
					if (Columns.Length == dt.Columns.Count)
					{
						changed = false;
						for (int i = 0; i < Columns.Length; i++)
						{
							if (string.Compare(Columns[i].ColumnName, dt.Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase) != 0)
							{
								changed = true;
							}
						}
					}
					if (changed)//columns changed, recreate table
					{
						DataTable dt2 = new DataTable(dt.TableName);
						for (int i = 0; i < Columns.Length; i++)
						{
							dt2.Columns.Add(Columns[i].ColumnName, Columns[i].SystemType);
						}
						bool hasData = false;
						int[] map = new int[Columns.Length];
						for (int i = 0; i < Columns.Length; i++)
						{
							map[i] = -1;
							for (int j = 0; j < dt.Columns.Count; j++)
							{
								if (string.Compare(dt.Columns[j].ColumnName, Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									map[i] = j;
									hasData = true;
									break;
								}
							}
						}
						if (hasData)
						{
							for (int i = 0; i < dt.Rows.Count; i++)
							{
								object[] vs = new object[Columns.Length];
								for (int j = 0; j < vs.Length; j++)
								{
									if (map[j] < 0)
									{
										vs[j] = null;
									}
									else
									{
										vs[j] = dt.Rows[i][map[j]];
									}
								}
								dt2.Rows.Add(vs);
							}
						}
						_dataTables[cultureName] = dt2;
						dt = dt2;
					}
				}
			}
			return dt;
		}
		public void FinishEdit()
		{
			Rows = new Dictionary<string, WebPageDataRow[]>();
			if (_dataTables != null)
			{
				foreach (KeyValuePair<string, DataTable> kv in _dataTables)
				{
					WebPageDataRow[] rs = new WebPageDataRow[kv.Value.Rows.Count];
					for (int i = 0; i < kv.Value.Rows.Count; i++)
					{
						rs[i] = new WebPageDataRow();
						rs[i].ItemArray = kv.Value.Rows[i].ItemArray;
					}
					Rows.Add(kv.Key, rs);
				}
			}
		}
		public void UpdateColumns(WebDataColumn[] tableColumns)
		{
			if (tableColumns == null)
			{
				tableColumns = new WebDataColumn[] { };
			}
			if (Columns == null || Columns.Length == 0)
			{
				Columns = tableColumns;
			}
			else
			{
				bool changed = true;
				if (Columns.Length == tableColumns.Length)
				{
					changed = false;
					for (int i = 0; i < Columns.Length; i++)
					{
						if (string.Compare(Columns[i].ColumnName, tableColumns[i].ColumnName, StringComparison.OrdinalIgnoreCase) != 0)
						{
							changed = true;
						}
					}
				}
				if (changed)
				{
					bool hasData = false;
					int[] map = new int[tableColumns.Length];
					for (int i = 0; i < tableColumns.Length; i++)
					{
						map[i] = -1;
						for (int j = 0; j < Columns.Length; j++)
						{
							if (string.Compare(tableColumns[i].ColumnName, Columns[j].ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
							{
								map[i] = j;
								hasData = true;
								break;
							}
						}
					}
					if (hasData)
					{
						if (Rows != null)
						{
							Dictionary<string, WebPageDataRow[]> newRows = new Dictionary<string, WebPageDataRow[]>();
							foreach (KeyValuePair<string, WebPageDataRow[]> kv in Rows)
							{
								WebPageDataRow[] rs;
								if (kv.Value == null)
								{
									rs = new WebPageDataRow[] { };
								}
								else
								{
									rs = new WebPageDataRow[kv.Value.Length];
									for (int i = 0; i < rs.Length; i++)
									{
										rs[i] = new WebPageDataRow();
										rs[i].ItemArray = new object[tableColumns.Length];
										for (int j = 0; j < tableColumns.Length; j++)
										{
											if (map[j] < 0)
											{
												rs[i].ItemArray[j] = null;
											}
											else
											{
												rs[i].ItemArray[j] = kv.Value[i].ItemArray[map[j]];
											}
										}
									}
								}
								newRows.Add(kv.Key, rs);
							}
							Rows = newRows;
						}
					}
					else
					{
						Rows = null;
					}
					Columns = tableColumns;
				}
			}
		}
		public WebPageDataRow[] GetCurrentRows()
		{
			if (Rows != null)
			{
				WebPageDataRow[] rs;
				if (CurrentCulture == null)
				{
					CurrentCulture = string.Empty;
				}
				if (Rows.TryGetValue(CurrentCulture, out rs))
				{
					return rs;
				}
			}
			return new WebPageDataRow[] { };
		}
		const string XML_Columns = "Columns";
		const string XML_Rows = "Rows";
		const string XML_RowsC = "RowsItem";
		const string XMLATTR_culture = "culture";
		public void ReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			TableName = XmlUtil.GetNameAttribute(node);
			XmlNodeList ns = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}/{1}", XML_Columns, WebPageDataSet.XML_Item));
			Columns = new WebDataColumn[ns.Count];
			for (int i = 0; i < ns.Count; i++)
			{
				Columns[i] = new WebDataColumn();
				Columns[i].ReadFromXmlNode(ns[i]);
			}
			Rows = new Dictionary<string, WebPageDataRow[]>();
			XmlNodeList ndrcs = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}/{1}", XML_Rows, XML_RowsC));
			foreach (XmlNode ndc in ndrcs)
			{
				string culture = XmlUtil.GetAttribute(ndc, XMLATTR_culture);
				XmlNodeList nrs = ndc.SelectNodes(WebPageDataSet.XML_Item);
				WebPageDataRow[] rs = new WebPageDataRow[nrs.Count];
				for (int i = 0; i < nrs.Count; i++)
				{
					rs[i] = new WebPageDataRow();
					rs[i].ReadFromXmlNode(reader, nrs[i]);
				}
				Rows.Add(culture, rs);
			}
		}

		public virtual void WriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, TableName);
			if (Columns != null)
			{
				XmlNode ndc = XmlUtil.CreateSingleNewElement(node, XML_Columns);
				ndc.RemoveAll();
				for (int i = 0; i < Columns.Length; i++)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(WebPageDataSet.XML_Item);
					ndc.AppendChild(nd);
					Columns[i].WriteToXmlNode(nd);
				}
				if (Rows != null)
				{
					XmlNode ndr = XmlUtil.CreateSingleNewElement(node, XML_Rows);
					ndr.RemoveAll();
					foreach (KeyValuePair<string, WebPageDataRow[]> kv in Rows)
					{
						XmlNode ndrc = node.OwnerDocument.CreateElement(XML_RowsC);
						ndr.AppendChild(ndrc);
						XmlUtil.SetAttribute(ndrc, XMLATTR_culture, kv.Key);
						if (kv.Value != null)
						{
							for (int i = 0; i < kv.Value.Length; i++)
							{
								if (kv.Value[i] != null)
								{
									XmlNode rn = node.OwnerDocument.CreateElement(WebPageDataSet.XML_Item);
									ndrc.AppendChild(rn);
									kv.Value[i].WriteToXmlNode(writer, rn);
								}
							}
						}
					}
				}
			}
		}
		#endregion
		//
		#region ICloneable Members

		public virtual WebDataTable Clone(WebPageDataSet dataset)
		{
			WebDataTable tbl = (WebDataTable)Activator.CreateInstance(this.GetType(), dataset);
			tbl.TableName = TableName;
			tbl.PrimaryKey = PrimaryKey;
			if (Columns != null)
			{
				tbl.Columns = new WebDataColumn[Columns.Length];
				for (int i = 0; i < Columns.Length; i++)
				{
					tbl.Columns[i] = Columns[i].Clone();
				}
			}
			if (DataRelations != null)
			{
				tbl.DataRelations = new WebDataRelation[DataRelations.Length];
				for (int i = 0; i < DataRelations.Length; i++)
				{
					tbl.DataRelations[i] = DataRelations[i].Clone();
				}
			}
			if (Rows != null)
			{
				tbl.Rows = new Dictionary<string, WebPageDataRow[]>();
				foreach (KeyValuePair<string, WebPageDataRow[]> kv in Rows)
				{
					WebPageDataRow[] dr;
					if (kv.Value == null)
					{
						dr = new WebPageDataRow[] { };
					}
					else
					{
						dr = new WebPageDataRow[kv.Value.Length];
						for (int i = 0; i < kv.Value.Length; i++)
						{
							dr[i] = kv.Value[i].Clone();
						}
					}
					tbl.Rows.Add(kv.Key, dr);
				}
			}
			return tbl;
		}

		#endregion

		#region PropertyDescriptorColumn
		class PropertyDescriptorColumn : PropertyDescriptor
		{
			private Type _valueType;
			public PropertyDescriptorColumn(Type valueType, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_valueType = valueType;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(WebDataTable); }
			}

			public override object GetValue(object component)
			{
				return string.Empty;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return _valueType; }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

	}
	public class WebDataTableSingleRow : WebDataTable, ICustomTypeDescriptor
	{
		public WebDataTableSingleRow(WebPageDataSet dataSet)
			: base(dataSet)
		{
		}
		public override void WriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.WriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, WebPageDataSet.XMLATT_dtype, 1);
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			WebPageDataRow[] rows = GetCurrentRows();
			if (rows == null || rows.Length == 0)
			{
				rows = new WebPageDataRow[1];
				rows[0] = new WebPageDataRow();
				if (this.Rows == null)
				{
					this.Rows = new Dictionary<string, WebPageDataRow[]>();
				}
				if (CurrentCulture == null)
				{
					CurrentCulture = string.Empty;
				}
				if (this.Rows.ContainsKey(CurrentCulture))
				{
					this.Rows[CurrentCulture] = rows;
				}
				else
				{
					this.Rows.Add(CurrentCulture, rows);
				}
			}
			WebPageDataRow r = rows[0];
			if (r == null)
			{
				r = new WebPageDataRow();
				rows[0] = r;
			}

			if (Columns == null)
			{
				Columns = new WebDataColumn[] { };
			}
			if (r.ItemArray == null)
			{
				r.ItemArray = new object[Columns.Length];
			}
			else
			{
				if (r.ItemArray.Length < Columns.Length)
				{
					object[] vs = new object[Columns.Length];
					for (int i = 0; i < r.ItemArray.Length; i++)
					{
						vs[i] = r.ItemArray[i];
					}
					r.ItemArray = vs;
				}
			}
			PropertyDescriptor[] ps = new PropertyDescriptor[Columns.Length];
			for (int i = 0; i < Columns.Length; i++)
			{
				ps[i] = new PropertyDescriptorRowValue(r, i, Columns[i].SystemType, Columns[i].ColumnName, attributes);
			}
			return new PropertyDescriptorCollection(ps);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region PropertyDescriptorRowValue
		class PropertyDescriptorRowValue : PropertyDescriptor
		{
			private WebPageDataRow _row;
			private int _idx;
			private Type _dataType;
			public PropertyDescriptorRowValue(WebPageDataRow row, int col, Type valueType, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_row = row;
				_idx = col;
				_dataType = valueType;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(WebDataTableSingleRow); }
			}

			public override object GetValue(object component)
			{
				return _row.ItemArray[_idx];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _dataType; }
			}

			public override void ResetValue(object component)
			{
				if (typeof(string).Equals(_dataType))
				{
					_row.ItemArray[_idx] = string.Empty;
				}
				else if (typeof(DateTime).Equals(_dataType))
				{
					_row.ItemArray[_idx] = new DateTime(2000, 1, 1);
				}
				else
				{
					_row.ItemArray[_idx] = 0;
				}
			}

			public override void SetValue(object component, object value)
			{
				_row.ItemArray[_idx] = value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
	}
}
