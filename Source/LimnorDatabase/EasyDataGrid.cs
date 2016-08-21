using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using VPL;

namespace LimnorDatabase
{
    [Description("It shows data in a grid. The data come from database")]
    [ToolboxBitmapAttribute(typeof(EasyDataGrid), "Resources.datagrid.bmp")]
    public class EasyDataGrid : DataGridView, ICustomContentSerialization
    {
        public EasyDataGrid()
        {
            base.DataMember = "Table";
        }
        [XmlIgnore]
        [ReadOnly(true)]
        [Browsable(false)]
        public new string DataMember
        {
            get
            {
                return "Table";
            }
            set
            {
            }
        }
        public new object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = null;
                Columns.Clear();
                base.DataSource = value;
                EasyQuery qry = value as EasyQuery;
                if (qry != null)
                {
                    base.DataMember = "Table";
                }
                Refresh();
            }
        }

        #region ICustomContentSerialization Members

        public Dictionary<string, object> CustomContents
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
