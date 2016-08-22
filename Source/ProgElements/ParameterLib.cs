/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Programming elements
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace ProgElements
{
	/// <summary>
	/// Wrap ParameterInfo with IParameter 
	/// </summary>
	public class ParameterLib : IParameter
	{
		private ParameterInfo _param;
		private int _idx;
		private UInt32 _id;
		public ParameterLib(ParameterInfo parameter, int index)
		{
			_param = parameter;
			_idx = index;
		}
		[Browsable(false)]
		public ParameterInfo Info
		{
			get
			{
				return _param;
			}
		}
		public int Index
		{
			get
			{
				return _idx;
			}
		}
		#region IParameter Members
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 ParameterID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(_idx + 1);
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return Name;
			}
		}
		public void SetDataType(object type)
		{

		}
		#endregion

		#region INamedDataType Members
		[ParenthesizePropertyName(true)]
		[ReadOnly(true)]
		public string Name
		{
			get
			{
				return _param.Name;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		public Type ParameterLibType
		{
			get
			{
				return _param.ParameterType;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		public string ParameterTypeString
		{
			get { return _param.ParameterType.FullName; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new ParameterLib(_param, _idx);
		}

		#endregion
	}
}
