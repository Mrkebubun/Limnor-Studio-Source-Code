/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Data.Common;
using System.Data;
using System.Data.Odbc;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dbWrapper.
	/// </summary>
	public class dbWrapper
	{
		private DbCommand cmd = null;
		protected EasyQuery query = null;
		private DbDataReader _dr = null;
		private IDataAdapter _adapter = null;
		public dbWrapper()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public bool IsOdbc
		{
			get
			{
				if (cmd != null)
				{
					return (cmd.Connection is OdbcConnection);
				}
				return false;
			}
		}
		public void CreateCommand(EasyQuery qry)
		{
			query = qry;
		}
		public void CreateCommand(Connection cnt)
		{
			cmd = cnt.TheConnection.CreateCommand();
		}
		public void SetCommandText(string sSQL)
		{
			cmd.CommandText = sSQL;
		}
		public void SetCommandType(System.Data.CommandType tp)
		{
			cmd.CommandType = tp;
		}
		public void AddCommandParameter(EPField p, EnumParameterStyle style)
		{
			DbParameter pam = cmd.CreateParameter();
			pam.ParameterName = p.GetParameterNameForCommand(style);
			pam.DbType = ValueConvertor.OleDbTypeToDbType(p.OleDbType);
			pam.Size = p.DataSize;
			pam.Value = p.Value;
			cmd.Parameters.Add(pam);
		}
		public void SetParameterValue(int index, object value)
		{
			cmd.Parameters[index].Value = value;
		}
		public DbParameterCollection Parameters
		{
			get
			{
				return cmd.Parameters;
			}
		}
		public object ExecuteScalar()
		{
			return cmd.ExecuteScalar();
		}
		public int ExecuteNonQuery()
		{
			return cmd.ExecuteNonQuery();
		}
		public void CreateAdapter()
		{
			_adapter = DataAdapterFinder.CreateDataAdapter(cmd);
		}
		public void Fill(System.Data.DataSet ds, string tblName)
		{
			_adapter.Fill(ds);
		}
		public void AssignParameters()
		{
			if (query != null)
			{
				if (query.Parameters != null)
				{
					int i;
					for (i = 0; i < query.Parameters.Count; i++)
					{
						cmd.Parameters[i].Value = query.Parameters[i].Value;
					}
				}
			}
		}
		public void OpenReader()
		{
			if (cmd.Connection.State == ConnectionState.Closed)
			{
				cmd.Connection.Open();
				_dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}
			else
			{
				_dr = cmd.ExecuteReader();
			}
		}
		public void OpenReader(System.Data.CommandBehavior br)
		{
			if (cmd.Connection.State == ConnectionState.Closed)
			{
				cmd.Connection.Open();
			}
			else
			{
			}
			_dr = cmd.ExecuteReader(br);
		}
		public System.Data.DataTable GetSchemaTable()
		{
			try
			{
				return _dr.GetSchemaTable();
			}
			finally
			{
				Close();
			}
		}
		public bool Read()
		{
			return _dr.Read();
		}
		public object GetValue(string field)
		{
			return _dr[field];
		}
		public void AddDateParam(string name, System.DateTime dt)
		{
			DbParameter pam = cmd.CreateParameter();
			pam.ParameterName = name;
			pam.Size = 8;
			pam.DbType = ValueConvertor.OleDbTypeToDbType(System.Data.OleDb.OleDbType.DBTimeStamp);
			pam.Value = dt;
			cmd.Parameters.Add(pam);
		}
		public void CloseReader()
		{
			if (_dr != null)
			{
				_dr.Close();
				_dr = null;
			}
		}
		public void Close()
		{
			CloseReader();
			if (cmd != null)
			{
				if (cmd.Connection.State != ConnectionState.Closed)
				{
					cmd.Connection.Close();
				}
				cmd = null;
			}
		}
	}
}
