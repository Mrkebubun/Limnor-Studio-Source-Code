/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Limnor.WebServerBuilder
{
	public static class ServerCodeutility
	{
		public static string CreateVariableName(string prefix)
		{
			return string.Format(CultureInfo.InvariantCulture,
					"{0}_{1}", prefix, ((UInt32)(Guid.NewGuid().GetHashCode())).ToString("x", CultureInfo.InvariantCulture));
		}
		public static string GetPhpMySqlConnectionName(Guid g)
		{
			return string.Format(CultureInfo.InvariantCulture, "cr_{0}", g.ToString("N", CultureInfo.InvariantCulture));
		}
	}
}
