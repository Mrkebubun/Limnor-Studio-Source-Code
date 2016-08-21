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
using System.Data;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	class ExecParameter
	{
		public ExecParameter()
		{
		}
		public override string ToString()
		{
			return _name;
		}

		private string _name;
		[ParenthesizePropertyName(true)]
		[Description("Parameter name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		private OleDbType _type = OleDbType.VarWChar;
		[Description("Parameter Type")]
		public OleDbType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		private int _size;
		[Description("Parameter data size for string parameter")]
		public int DataSize
		{
			get
			{
				return EPField.FieldDataSize(_type, _size);
			}
			set
			{
				_size = value;
			}
		}

		private ParameterDirection _direction;
		[Description("Parameter direction")]
		public ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
	}
}
