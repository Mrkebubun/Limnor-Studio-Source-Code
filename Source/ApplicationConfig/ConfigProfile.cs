/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Limnor.Application
{
	public class ConfigProfile
	{
		public ConfigProfile(string name, string file, EnumProfileType type)
		{
			ProfileName = name;
			ProfilePath = file;
			ProfileType = type;
		}
		public string ProfileName { get; set; }
		public string ProfilePath { get; set; }
		public EnumProfileType ProfileType { get; set; }
		public override string ToString()
		{
			if (ProfileType == EnumProfileType.User)
			{
				return string.Format(CultureInfo.InvariantCulture, "<{0}>", ProfileName);
			}
			else if (ProfileType == EnumProfileType.Default)
			{
				return "{default}";
			}
			return ProfileName;
		}
	}
}
