/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MathExp
{
	[Serializable]
	public class MathException : Exception//,ISerializable
	{
		public MathException(string message)
			: base(message)
		{
		}
		public MathException(string message, params object[] values)
			: base(XmlSerialization.FormatString(message, values))
		{
		}
		public MathException(Exception e, string message, params object[] values)
			: base(XmlSerialization.FormatString(message, values), e)
		{
		}
		static public string FormExceptionText(string message, Exception e)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(message))
			{
				sb.Append(message);
				sb.Append("\r\n");
			}
			sb.Append(e.GetType().AssemblyQualifiedName);
			sb.Append(":");
			sb.Append(e.Message);
			if (!string.IsNullOrEmpty(e.StackTrace))
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:(");
				sb.Append(e.GetType().AssemblyQualifiedName);
				sb.Append(")\r\n");
				sb.Append(e.Message);
				if (!string.IsNullOrEmpty(e.StackTrace))
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
		#region ISerializable Members

		protected MathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}
		#endregion
	}
}
