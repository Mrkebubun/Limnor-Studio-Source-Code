/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Programming elements
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ProgElements
{
	/// <summary>
	/// wrap a Type with IParameter
	/// </summary>
	public class ReturnParameter : IParameter
	{
		private Type _type;
		public ReturnParameter(Type type)
		{
			_type = type;
		}
		public Type ReturnType
		{
			get
			{
				return _type;
			}
		}

		#region IParameter Members
		[Browsable(false)]
		[ReadOnly(true)]
		public uint ParameterID
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return "return"; }
		}
		public void SetDataType(object type)
		{
			_type = type as Type;
		}
		#endregion

		#region INamedDataType Members
		[ParenthesizePropertyName(true)]
		[ReadOnly(true)]
		public string Name
		{
			get
			{
				return CodeName;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ParameterLibType
		{
			get
			{
				return _type;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string ParameterTypeString
		{
			get
			{
				return _type.FullName;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new ReturnParameter(_type);
		}

		#endregion
	}
}
