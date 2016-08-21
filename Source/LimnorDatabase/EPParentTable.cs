using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
    public class EPParentTable : ICloneable
    {
        public EasyDataTable Owner = null; //child table
        public EasyDataTable ParentTable = null; //parent table
        public EPJoin[] JoinFields = null;
        public EPParentTable()
        {
        }
        
        #region ICloneable Members

        public object Clone()
        {
            EPParentTable obj = new EPParentTable();
            obj.Owner = Owner;
            if (ParentTable != null)
                obj.ParentTable = ParentTable;
            if (JoinFields != null)
            {
                obj.JoinFields = new EPJoin[JoinFields.Length];
                for (int i = 0; i < JoinFields.Length; i++)
                    obj.JoinFields[i] = (EPJoin)JoinFields[i].Clone();
            }
            return obj;
        }

        #endregion
        public override string ToString()
        {
            if (ParentTable == null)
                return "";
            StringBuilder s = new StringBuilder(ParentTable.Name);
            if (JoinFields != null)
            {
                s.Append("(");
                for (int i = 0; i < JoinFields.Length; i++)
                {
                    if (JoinFields[i] != null)
                    {
                        if (JoinFields[i].field1 != null)
                        {
                            s.Append(JoinFields[i].field1.Name);
                        }
                        s.Append("=");
                        if (JoinFields[i].field2 != null)
                        {
                            s.Append(JoinFields[i].field2.Name);
                        }
                    }
                }
                s.Append(")");
            }
            return s.ToString();
        }
    }
}
