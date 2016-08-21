using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Data.OleDb;

namespace LimnorDatabase
{
    public class QueryWrapper
    {
        private EasyQuery _query;
        public QueryWrapper()
        {
        }
        public QueryWrapper(EasyQuery query)
        {
            _query = query;
        }
        public void SetQuery(EasyQuery query)
        {
            _query = query;
        }
        [Browsable(false)]
        public EasyQuery Query
        {
            get
            {
                if (_query == null)
                {
                    _query = new EasyQuery();
                }
                return _query;
            }
        }
        [Browsable(false)]
        public FieldList Fields
        {
            get
            {
                return Query.Fields;
            }
        }
        [Browsable(false)]
        public string SqlQuery
        {
            get
            {
                return Query.SQL.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Query.SQL = new SQLStatement(value);
                }
            }
        }
        [Browsable(false)]
        public bool ForReadOnly
        {
            get
            {
                return Query.ForReadOnly;
            }
            set
            {
                Query.ForReadOnly = value;
            }
        }
        [XmlIgnore]
        [Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
        [Description("Connection to the database")]
        public ConnectionItem DatabaseConnection
        {
            get
            {
                return Query.DatabaseConnection;
            }
            set
            {
                Query.DatabaseConnection = value;
            }
        }
        [Browsable(false)]
        public Guid ConnectionID
        {
            get
            {
                return Query.ConnectionID;
            }
            set
            {
                Query.ConnectionID = value;
            }
        }
        [TypeConverter(typeof(TypeConverterSQLString))]
        [XmlIgnore]
        [Description("SQL statement for querying database")]
        [Editor(typeof(UIQueryEditor), typeof(UITypeEditor))]
        public SQLStatement SQL
        {
            get
            {
                return Query.SQL;
            }
            set
            {
                Query.SQL = value;
            }
        }
    }
}
