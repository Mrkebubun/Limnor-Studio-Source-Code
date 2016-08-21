/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.OleDb;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Windows.Forms;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
	/// query definition used by Drawing2DTable
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DatabaseQuery : IDatabaseAccess, ISqlUser, IDevClassReferencer
	{
		#region fields and constructors
		private EasyQuery _query;
		private IDatabaseAccess _owner;
		private Control _ctrlOwner;
		private int _cnnStr;
		public DatabaseQuery(IDatabaseAccess owner)
		{
			_owner = owner;
			_query = owner.QueryDef;
			if (_query != null)
			{
				_query.ForReadOnly = true;
			}
		}
		public void SetControlOwner(Control owner)
		{
			_ctrlOwner = owner;
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			if (_query != null)
			{
				return _query.SqlQuery;
			}
			return string.Empty;
		}
		[Browsable(false)]
		public virtual bool OnBeforeSetSQL()
		{
			return true;
		}
		#endregion

		#region Hidden Properties
		[Browsable(false)]
		public string TableName
		{
			get
			{
				return _query.TableName;
			}
			set
			{
				_query.TableName = value;
			}
		}
		[Browsable(false)]
		public string SqlQuery
		{
			get
			{
				return _query.SqlQuery;
			}
			set
			{
				_query.SqlQuery = value;
			}
		}
		[Browsable(false)]
		[XmlIgnore]
		public string DefaultConnectionString
		{
			get
			{
				return _query.DefaultConnectionString;
			}
			set
			{
				_query.DefaultConnectionString = value;
			}
		}
		[Browsable(false)]
		[XmlIgnore]
		public Type DefaultConnectionType
		{
			get
			{
				return _query.DefaultConnectionType;
			}
			set
			{
				_query.DefaultConnectionType = value;
			}
		}
		[Browsable(false)]
		public Guid ConnectionID
		{
			get
			{
				return _query.ConnectionID;
			}
			set
			{
				_query.ConnectionID = value;
			}
		}
		[Browsable(false)]
		public int Reserved
		{
			get
			{
				return _cnnStr;
			}
			set
			{
				_cnnStr = value;
				PropertyDescriptorCollection ps;
				if (_ctrlOwner != null)
				{
					ps = TypeDescriptor.GetProperties(_ctrlOwner);
					PropertyDescriptor p = ps["Reserved"];
					if (p != null)
					{
						p.SetValue(_ctrlOwner, value);
					}
				}
				else
				{
					ps = TypeDescriptor.GetProperties(_owner);
					PropertyDescriptor p = ps["Reserved"];
					if (p != null)
					{
						p.SetValue(_owner, value);
					}
				}

			}
		}
		[DefaultValue(false)]
		[Browsable(false)]
		[Description("Indicates whether duplicated records are allowed")]
		public bool Distinct { get { return _query.Distinct; } set { _query.Distinct = value; } } //include DISTINCT

		[DefaultValue(0)]
		[Browsable(false)]
		[Description("Specifies that the query result contain a specific number of rows or a percentage of rows of the query result. Following keyword TOP, you can specify 1 to 32,767 rows, or, if you include the PERCENT option, you can specify 0.01 to 99.99 percent. ")]
		public int Top { get { return _query.Top; } set { _query.Top = value; } } // include TOP <Top>

		[DefaultValue(false)]
		[Browsable(false)]
		[Description("Specifies that the query result contain a specific number of rows or a percentage of rows of the query result. Following keyword TOP, you can specify 1 to 32,767 rows, or, if you include the PERCENT option, you can specify 0.01 to 99.99 percent. ")]
		public bool Percent { get { return _query.Percent; } set { _query.Percent = value; } }

		[DefaultValue(false)]
		[Browsable(false)]
		[Description("Specifies that additional rows be returned from the base result set with the same value in the ORDER BY columns appearing as the last of the TOP n (PERCENT) rows. TOP…WITH TIES can be specified only in SELECT statements, and only if an ORDER BY clause is specified.")]
		public bool WithTies { get { return _query.WithTies; } set { _query.WithTies = value; } }

		[DefaultValue("")]
		[Browsable(false)]
		[Description("'FROM' clause of the SELECT statement")]
		public string From { get { return _query.From; } set { _query.From = value; } }

		[DefaultValue("")]
		[Browsable(false)]
		[Description("'WHERE' clause of the SELECT statement")]
		public string Where { get { return _query.Where; } set { _query.Where = value; } }

		[DefaultValue("")]
		[Browsable(false)]
		[Description("'GROUP BY' clause of the SELECT statement")]
		public string GroupBy { get { return _query.GroupBy; } set { _query.GroupBy = value; } }

		[DefaultValue("")]
		[Browsable(false)]
		[Description("'HAVING' clause of the SELECT statement")]
		public string Having { get { return _query.Having; } set { _query.Having = value; } }

		[DefaultValue("")]
		[Browsable(false)]
		[Description("'ORDER BY' clause of the SELECT statement")]
		public string OrderBy { get { return _query.OrderBy; } set { _query.OrderBy = value; } }
		#endregion

		#region UI Properties
		[ReadOnly(true)]
		[TypeConverter(typeof(TypeConverterSQLString))]
		[XmlIgnore]
		[Description("SQL statement for querying database")]
		[Editor(typeof(UIQueryEditor), typeof(UITypeEditor))]
		public SQLStatement SQL
		{
			get
			{
				return _query.SQL;
			}
			set
			{
				_query.SQL = value;
				if (_owner != null)
				{
					_owner.SQL = value;
				}
			}
		}

		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
		[Description("Connection to the database")]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				return _query.DatabaseConnection;
			}
			set
			{
				_query.DatabaseConnection = value;
			}
		}
		[DefaultValue(true)]
		[Description("Indicates whether to make database query when this object is created.")]
		public bool QueryOnStart
		{
			get { return _query.QueryOnStart; }
			set { _query.QueryOnStart = value; }
		}
		#endregion

		#region Fields Serialization
		[Browsable(false)]
		public string[] Field_Name
		{
			get
			{
				return _query.Field_Name;
			}
			set
			{
				_query.Field_Name = value;
			}
		}
		[Browsable(false)]
		public bool[] Field_ReadOnly
		{
			get
			{
				return _query.Field_ReadOnly;
			}
			set
			{
				_query.Field_ReadOnly = value;
			}
		}
		[Browsable(false)]
		public OleDbType[] Field_OleDbType
		{
			get
			{
				return _query.Field_OleDbType;
			}
			set
			{
				_query.Field_OleDbType = value;
			}
		}
		[Browsable(false)]
		public bool[] Field_IsIdentity
		{
			get
			{
				return _query.Field_IsIdentity;
			}
			set
			{
				_query.Field_IsIdentity = value;
			}
		}
		[Browsable(false)]
		public bool[] Field_Indexed
		{
			get
			{
				return _query.Field_Indexed;
			}
			set
			{
				_query.Field_Indexed = value;
			}
		}
		[Browsable(false)]
		public int[] Field_DataSize
		{
			get
			{
				return _query.Field_DataSize;
			}
			set
			{
				_query.Field_DataSize = value;
			}
		}
		[Browsable(false)]
		public string[] Field_FromTableName
		{
			get
			{
				return _query.Field_FromTableName;
			}
			set
			{
				_query.Field_FromTableName = value;
			}
		}
		[Browsable(false)]
		public string[] Field_FieldText
		{
			get
			{
				return _query.Field_FieldText;
			}
			set
			{
				_query.Field_FieldText = value;
			}
		}
		[Browsable(false)]
		public string[] Field_FieldCaption
		{
			get
			{
				return _query.Field_FieldCaption;
			}
			set
			{
				_query.Field_FieldCaption = value;
			}
		}
		[Browsable(false)]
		public bool[] Field_IsCalculated
		{
			get
			{
				return _query.Field_IsCalculated;
			}
			set
			{
				_query.Field_IsCalculated = value;
			}
		}
		#endregion

		#region Parameters Serialization
		[Browsable(false)]
		public string[] Param_Name
		{
			get
			{
				return _query.Param_Name;
			}
			set
			{
				_query.Param_Name = value;
			}
		}
		[Browsable(false)]
		public OleDbType[] Param_OleDbType
		{
			get
			{
				return _query.Param_OleDbType;
			}
			set
			{
				_query.Param_OleDbType = value;
			}
		}
		[Browsable(false)]
		public int[] Param_DataSize
		{
			get
			{
				return _query.Param_DataSize;
			}
			set
			{
				_query.Param_DataSize = value;
			}
		}

		[Browsable(false)]
		public string[] Param_Value
		{
			get
			{
				return _query.Param_Value;
			}
			set
			{
				_query.Param_Value = value;
			}
		}

		#endregion

		#region IDatabaseAccess Members
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedDesignTimeSQL
		{
			get { return true; }
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateDataTable()
		{
		}
		[Browsable(false)]
		public void SetSqlContext(string name)
		{

		}
		[Browsable(false)]
		public string Name
		{
			get { return _query.Name; }
		}
		[Browsable(false)]
		public bool Query()
		{
			return _owner.Query();
		}
		[Browsable(false)]
		public bool IsConnectionReady
		{
			get { return _query.IsConnectionReady; }
		}
		[Browsable(false)]
		public EasyQuery QueryDef
		{
			get { return _query; }
		}
		[Browsable(false)]
		public void CopyFrom(EasyQuery query)
		{
			_query.CopyFrom(query);
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (_query != null)
			{
				return _query.Report32Usage();
			}
			return string.Empty;
		}
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get { return _query.DatabaseConnectionsUsed; }
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get { return _query.DatabaseConnectionTypesUsed; }
		}
		#endregion

		#region IDevClassReferencer Members
		private IDevClass _class;
		public void SetDevClass(IDevClass c)
		{
			_class = c;
		}

		public IDevClass GetDevClass()
		{
			return _class;
		}
		#endregion
	}
}
