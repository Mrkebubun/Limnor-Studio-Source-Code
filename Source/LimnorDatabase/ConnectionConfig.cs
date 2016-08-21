/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LimnorDatabase
{
	static class ConnectionConfig
	{
		private static Configuration _config;
		private static string connectionFile
		{
			get
			{
				return System.IO.Path.Combine(ConnectionItem.GetConnectionFileFolder(), "connections.xml");
			}
		}
		private static Configuration Configuration
		{
			get
			{
				if (_config == null)
				{
					ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
					fileMap.ExeConfigFilename = connectionFile;
					_config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
					if (!_config.ConnectionStrings.SectionInformation.IsProtected)
					{
						_config.ConnectionStrings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
						_config.Save();
					}
				}
				return _config;
			}
		}
		public static void SetConnection(Guid g, string connectionString)
		{
			string key = g.ToString();
			ConnectionStringSettings cs = Configuration.ConnectionStrings.ConnectionStrings[key];
			if (cs == null)
			{
				cs = new ConnectionStringSettings(key, connectionString);
				_config.ConnectionStrings.ConnectionStrings.Add(cs);
			}
			else
			{
				cs.ConnectionString = connectionString;
			}
			try
			{
				_config.Save();
			}
			catch (System.Configuration.ConfigurationErrorsException)
			{
			}
		}
		public static void RemoveConnection(Guid g)
		{
			string key = g.ToString();
			ConnectionStringSettings cs = Configuration.ConnectionStrings.ConnectionStrings[key];
			if (cs != null)
			{
				_config.ConnectionStrings.ConnectionStrings.Remove(cs);
				_config.Save();
			}
		}
		public static string GetConnectionString(Guid g)
		{
			string key = g.ToString();
			ConnectionStringSettings cs = Configuration.ConnectionStrings.ConnectionStrings[key];
			if (cs != null)
			{
				return cs.ConnectionString;
			}
			return string.Empty;
		}
	}
}
