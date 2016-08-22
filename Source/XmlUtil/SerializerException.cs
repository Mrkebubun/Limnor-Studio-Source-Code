using System;
using System.Collections.Generic;
using System.Text;

namespace XmlUtility
{
	public class SerializerException : Exception
	{
		public SerializerException(string msg, Exception e)
			: base(msg, e)
		{
		}
		public SerializerException(string msg, params object[] values)
			: base(string.Format(msg, values))
		{
		}
		static public string FormExceptionText(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			sb.Append("\r\n");
			sb.Append(e.GetType().AssemblyQualifiedName);
			sb.Append("\r\n");
			if (e.StackTrace != null)
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				sb.Append("\r\n");
				sb.Append(e.GetType().AssemblyQualifiedName);
				sb.Append("\r\n");
				if (e.StackTrace != null)
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
	}
}
