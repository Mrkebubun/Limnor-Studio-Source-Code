/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using MathExp;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;
using VPL;
using System.Xml;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataQuery : ICloneable, IDevClassReferencer, IQuery, IPostXmlNodeSerialize
	{
		#region fields and constructors
		private EasyQuery _qry = null;
		private IDevClassReferencer _owner;
		public DataQuery()
		{
			_qry = new EasyQuery();
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public bool IsJet
		{
			get
			{
				return _qry.IsJet;
			}
		}
		[Browsable(false)]
		public ParameterList Parameters
		{
			get
			{
				return _qry.Parameters;
			}
		}
		[Browsable(false)]
		public string LastError
		{
			get
			{
				return _qry.LastError;
			}
		}
		[Browsable(false)]
		public EasyQuery QueryDef
		{
			get
			{
				if (_qry == null)
					_qry = new EasyQuery();
				return _qry;
			}
		}
		[Browsable(false)]
		public string TableName
		{
			get
			{
				return QueryDef.TableName;
			}
			set
			{
				QueryDef.TableName = value;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Category("Database")]
		[ReadOnly(true)]
		[TypeConverter(typeof(TypeConverterSQLString))]
		[XmlIgnore]
		[Description("SQL statement for querying database")]
		[Editor(typeof(UIQueryEditor), typeof(UITypeEditor))]
		public SQLStatement SQL
		{
			get
			{
				return QueryDef.SQL;
			}
			set
			{
				QueryDef.SQL = value;
				onPropertyChanged();
			}
		}
		private QueryParameterList _qryParams;
		[Description("Gets the parameters used in the data query")]
		public QueryParameterList QueryParameters
		{
			get
			{
				if (_qryParams == null)
				{
					_qryParams = new QueryParameterList(QueryDef.Parameters);
				}
				else
				{
					_qryParams.MergeParameterList(QueryDef.Parameters);
				}
				_qryParams.SetOwner(this);
				return _qryParams;
			}
		}

		[Browsable(false)]
		public Guid ConnectionID
		{
			get
			{
				return QueryDef.ConnectionID;
			}
			set
			{
				QueryDef.ConnectionID = value;
				onPropertyChanged();
			}
		}
		[Category("Database")]
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
		[Description("Connection to the database")]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				return QueryDef.DatabaseConnection;
			}
			set
			{
				QueryDef.DatabaseConnection = value;
			}
		}
		[DefaultValue(null)]
		[Category("Database")]
		[Description("Description for this query")]
		public string Description
		{
			get { return _qry.Description; }
			set { _qry.Description = value; }
		}

		[DefaultValue(false)]
		[Description("Indicates whether duplicated records are allowed")]
		public bool Distinct
		{
			get { return _qry.Distinct; }
			set { _qry.Distinct = value; }
		}

		[DefaultValue(0)]
		[Description("Specifies that the query result contain a specific number of rows or a percentage of rows of the query result. Following keyword TOP, you can specify 1 to 32,767 rows, or, if you include the PERCENT option, you can specify 0.01 to 99.99 percent. ")]
		public int Top
		{
			get { return _qry.Top; }
			set { _qry.Top = value; }
		}

		[DefaultValue(false)]
		[Description("Specifies that the query result contain a specific number of rows or a percentage of rows of the query result. Following keyword TOP, you can specify 1 to 32,767 rows, or, if you include the PERCENT option, you can specify 0.01 to 99.99 percent. ")]
		public bool Percent
		{
			get { return _qry.Percent; }
			set { _qry.Percent = value; }
		}

		[DefaultValue(false)]
		[Description("Specifies that additional rows be returned from the base result set with the same value in the ORDER BY columns appearing as the last of the TOP n (PERCENT) rows. TOP…WITH TIES can be specified only in SELECT statements, and only if an ORDER BY clause is specified.")]
		public bool WithTies
		{
			get { return _qry.WithTies; }
			set { _qry.WithTies = value; }
		}
		[Category("Database")]
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(HideUITypeEditor), typeof(UITypeEditor))]
		[Description("Fields in this data query. It represents the current row at runtime.")]
		public FieldList Fields
		{
			get
			{
				return _qry.Fields;
			}
			set
			{
				_qry.Fields = value;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public FieldCollection ParamList
		{
			get
			{
				FieldCollection fs = new FieldCollection();
				if (_qry.Parameters != null)
				{
					for (int i = 0; i < _qry.Parameters.Count; i++)
					{
						fs.AddField(_qry.Parameters[i]);
					}
				}
				return fs;
			}
			set
			{
				if (value != null)
				{
					_qry.Parameters.Clear();
					for (int i = 0; i < value.Count; i++)
					{
						_qry.Parameters.AddField(value[i]);
					}
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public FieldCollection FieldList
		{
			get
			{
				FieldCollection fs = new FieldCollection();
				LimnorDatabase.FieldList fl = _qry.Fields;
				if (fl != null)
				{
					for (int i = 0; i < fl.Count; i++)
					{
						fs.AddField(fl[i]);
					}
				}
				return fs;
			}
			set
			{
				if (value != null)
				{
					LimnorDatabase.FieldList fl = new LimnorDatabase.FieldList();
					for (int i = 0; i < value.Count; i++)
					{
						fl.AddField(value[i]);
					}
					_qry.Fields = fl;
				}
			}
		}
		public int FieldCount
		{
			get
			{
				return _qry.Fields.Count;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		[Category("Database")]
		[Description("Parameters for filtering the data.")]
		public DbParameterList DbParameters
		{
			get
			{
				return new DbParameterList(_qry.Parameters);
			}
		}

		[DefaultValue(null)]
		[Description("'FROM' clause of the SELECT statement")]
		public string From
		{
			get { return _qry.From; }
			set { _qry.From = value; }
		}
		[DefaultValue(null)]
		[Description("'LIMIT' clause of the SELECT statement")]
		public string Limit
		{
			get { return _qry.Limit; }
			set { _qry.Limit = value; }
		}
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		[Description("'WHERE' clause of the SELECT statement")]
		public string Where
		{
			get { return _qry.Where; }
			set { _qry.Where = value; }
		}
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		[Description("'GROUP BY' clause of the SELECT statement")]
		public string GroupBy
		{
			get { return _qry.GroupBy; }
			set { _qry.GroupBy = value; }
		}
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		[Description("'HAVING' clause of the SELECT statement")]
		public string Having
		{
			get { return _qry.Having; }
			set { _qry.Having = value; }
		}
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		[Description("'ORDER BY' clause of the SELECT statement")]
		public string OrderBy
		{
			get { return _qry.OrderBy; }
			set { _qry.OrderBy = value; }
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public void SetOwner(IDevClassReferencer owner)
		{
			_owner = owner;
		}
		[Description("Compose the query commands and query the database for data")]
		public bool Query()
		{
			ParameterList ps = QueryDef.Parameters;
			if (_qryParams != null && ps != null && ps.Count > 0)
			{
				for (int i = 0; i < ps.Count; i++)
				{
					EPField f = ps[i];
					if (f != null)
					{
						f.Value = _qryParams.Value(f.Name);
					}
				}
			}
			return QueryDef.Query();
		}
		public override string ToString()
		{
			FieldList fl = this.Fields;
			if (fl == null)
				return string.Empty;
			if (fl.Count <= 0)
				return string.Empty;
			StringBuilder s = new StringBuilder(fl[0].Name);
			for (int i = 1; i < fl.Count; i++)
			{
				s.Append(",");
				s.Append(fl[i].Name);
			}
			return s.ToString();
		}
		#endregion

		#region private methods
		private void onPropertyChanged()
		{
			if (_owner != null)
			{
				IDevClass dev = _owner.GetDevClass();
				if (dev != null)
				{
					dev.NotifyChange(this, string.Empty);
				}
			}
		}
		#endregion

		#region ICloneable Members
		[Browsable(false)]
		public virtual object Clone()
		{
			DataQuery obj = new DataQuery();
			obj.SetOwner(_owner);
			obj._qry = _qry.Clone() as EasyQuery;
			return obj;
		}

		#endregion

		#region IDevClassReferencer Members
		private IDevClass _class;
		[Browsable(false)]
		public void SetDevClass(IDevClass c)
		{
			_class = c;
		}
		[Browsable(false)]
		public IDevClass GetDevClass()
		{
			return _class;
		}

		#endregion

		#region IQuery Members

		[Browsable(false)]
		public void CopyFrom(EasyQuery query)
		{
			_qry.CopyFrom(query);
		}
		[Browsable(false)]
		public bool IsConnectionReady
		{
			get { return _qry.IsConnectionReady; }
		}

		#endregion

		#region IDatabaseConnectionUserExt0 Members
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				return _qry.DatabaseConnectionTypesUsed;
			}
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (_qry != null)
			{
				return _qry.Report32Usage();
			}
			return string.Empty;
		}
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				return _qry.DatabaseConnectionsUsed;
			}
		}

		#endregion


		#region IPostXmlNodeSerialize Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			QueryParameters.OnReadFromXmlNode(serializer, node);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_qryParams != null)
			{
				_qryParams.OnWriteToXmlNode(serializer, node);
			}
		}

		#endregion
	}
}
