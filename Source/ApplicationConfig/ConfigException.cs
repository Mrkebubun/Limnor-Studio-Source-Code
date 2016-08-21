/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limnor.Application
{
	public class ConfigException : Exception
	{
		public ConfigException(string message, params object[] values)
			: base(values == null ? message : values.Length == 0 ? message : string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values))
		{
		}
	}
}
