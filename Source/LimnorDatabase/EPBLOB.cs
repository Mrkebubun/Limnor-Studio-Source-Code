/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Data.Common;
using System.Text;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for EPBLOB.
	/// </summary>
	public class EPBLOB
	{
		public byte[] bs = null;
		public EPBLOB()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
	public class BLOBRow
	{
		public EPBLOB[] row = null;
		public BLOBRow()
		{
		}
		public BLOBRow(int n)
		{
			row = new EPBLOB[n];
		}
		public void LoadData(DbDataReader dr, int i)
		{
			row[i] = new EPBLOB();
			row[i].bs = GetBytes(dr, i);
		}
		public void LoadData(object v, int i)
		{
			row[i] = new EPBLOB();
			string s = v as string;
			if (s != null)
				row[i].bs = GetBytes(s);
		}
		static public byte[] GetBytes(string s)
		{
			byte[] bs = null;
			if (s != null)
			{
				short nc;
				bs = new Byte[2 * s.Length];
				int k = 0;
				for (int i = 0; i < s.Length; i++)
				{
					nc = (short)s[i];
					bs[k] = (Byte)nc;
					bs[k + 1] = (Byte)(nc >> 8);
					k += 2;
				}
			}
			return bs;
		}
		static public byte[] GetBytes(DbDataReader dr, int fld)
		{
			if (dr[fld] != System.DBNull.Value)
			{
				string s = (string)dr[fld];
				return GetBytes(s);
			}
			return null;
		}
	}
	public class BLOBTable
	{
		protected DbCommand[] cmdUpdates = null;
		protected FieldList rowIDFields = null;
		public FieldList fields = new FieldList();
		public BLOBRow[] rows = null;
		public BLOBTable()
		{
		}
		public BLOBRow GetRow(System.Data.DataRow row)
		{
			BLOBRow rw = new BLOBRow(fields.Count);
			int i;
			for (i = 0; i < fields.Count; i++)
			{
				rw.LoadData(row[fields[i].Name], i);
			}
			return rw;
		}
		public BLOBRow GetRow(int Index, System.Data.DataRow row)
		{
			if (rows != null)
			{
				if (Index >= 0 && Index < rows.Length)
				{
					if (rows[Index] == null)
					{
						int i;
						rows[Index] = new BLOBRow(fields.Count);
						for (i = 0; i < fields.Count; i++)
						{
							rows[Index].LoadData(row[fields[i].Name], i);
						}
					}
					if (rows[Index] != null)
						return rows[Index];
				}
			}
			return null;
		}
		public void MakeCommand(string table, FieldList rowID, Connection cn)
		{
			if (fields.Count > 0)
			{
				int i;
				rowIDFields = rowID;
				//================================
				cmdUpdates = new DbCommand[fields.Count];
				for (int k = 0; k < fields.Count; k++)
				{
					cmdUpdates[k] = cn.CreateCommand();
					StringBuilder sSQL = new StringBuilder();
					sSQL.Append(QueryParser.SQL_Update());
					sSQL.Append(DatabaseEditUtil.SepBegin(cn.NameDelimiterStyle));
					sSQL.Append(table);
					sSQL.Append(DatabaseEditUtil.SepEnd(cn.NameDelimiterStyle));
					sSQL.Append(QueryParser.SQL_Set());
					sSQL.Append(DatabaseEditUtil.SepBegin(cn.NameDelimiterStyle));
					sSQL.Append(fields[k].Name);
					sSQL.Append(DatabaseEditUtil.SepEnd(cn.NameDelimiterStyle));
					sSQL.Append("=?");
					sSQL.Append(QueryParser.SQL_Where());
					sSQL.Append(DatabaseEditUtil.SepBegin(cn.NameDelimiterStyle));
					sSQL.Append(rowIDFields[0].Name);
					sSQL.Append(DatabaseEditUtil.SepEnd(cn.NameDelimiterStyle));
					sSQL.Append("=? ");
					for (i = 1; i < rowIDFields.Count; i++)
					{
						sSQL.Append(QueryParser.SQL_And());
						sSQL.Append(DatabaseEditUtil.SepBegin(cn.NameDelimiterStyle));
						sSQL.Append(rowIDFields[i].Name);
						sSQL.Append(DatabaseEditUtil.SepEnd(cn.NameDelimiterStyle));
						sSQL.Append("=?");
					}
					cmdUpdates[k].CommandText = sSQL.ToString();
					DbParameter pam = cmdUpdates[k].CreateParameter();
					pam.ParameterName = fields[k].Name;
					pam.DbType = ValueConvertor.OleDbTypeToDbType(fields[k].OleDbType);
					pam.Size = fields[k].DataSize;
					pam.SourceColumn = fields[k].Name;
					cmdUpdates[k].Parameters.Add(pam);
					for (i = 0; i < rowIDFields.Count; i++)
					{
						pam = cmdUpdates[k].CreateParameter();
						pam.ParameterName = rowIDFields[i].Name;
						pam.DbType = ValueConvertor.OleDbTypeToDbType(rowIDFields[i].OleDbType);
						pam.Size = rowIDFields[i].DataSize;
						pam.SourceColumn = rowIDFields[i].Name;
						cmdUpdates[k].Parameters.Add(pam);
					}
				}
			}
		}
		public void ExecuteCommand(FieldList rowID, int index, EPBLOB blob)
		{
			bool bClosed = (cmdUpdates[index].Connection.State == System.Data.ConnectionState.Closed);
			try
			{
				int i;
				cmdUpdates[index].Parameters[0].Value = blob.bs;
				for (i = 0; i < rowID.Count; i++)
				{
					cmdUpdates[index].Parameters[1 + i].Value = rowID[i].Value;
				}
				if (bClosed)
					cmdUpdates[index].Connection.Open();
				cmdUpdates[index].ExecuteNonQuery();
				if (bClosed)
					cmdUpdates[index].Connection.Close();
			}
			catch (Exception er)
			{
				FormLog.NotifyException(true,er);
			}
			finally
			{
				if (bClosed)
				{
					if (cmdUpdates[index].Connection.State != System.Data.ConnectionState.Closed)
					{
						cmdUpdates[index].Connection.Close();
					}
				}
			}
		}
	}
}
