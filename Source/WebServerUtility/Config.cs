
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System;

/*
'--
'-- processes custom .config section as follows:
'--
'-- <configSections>    
'--	    <section name="UnhandledException" type="System.Configuration.NameValueSectionHandler, System, 
'--        Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
'-- </configSections>
'--
'-- <!-- settings for UnhandledExceptionManager -->
'--	<UnhandledException>
'--		<add key="ContactInfo" value="Ima Testguy at 123-555-1212" />
'--		<add key="IgnoreDebug" value="False" />
'--		<add key="IgnoreRegex" value="get_aspx_ver\.aspx" />
'--		<add key="EmailTo" value="me@mydomain.com" />
'--	</UnhandledException>   
'--
'-- Complete configuration options are..
'--  
'--  <!-- Handler defaults -->
'--  <add key="AppName" value="" />
'--  <add key="ContactInfo" value="" />
'--  <add key="EmailTo" value="" />
'--  <add key="LogToEventLog" value="False" />
'--  <add key="LogToFile" value="True" />
'--  <add key="LogToEmail" value="True" />
'--  <add key="LogToUI" value="True" />
'--	 <add key="PathLogFile" value="" />
'--  <add key="IgnoreRegex" value="" />
'--  <add key="IgnoreDebug" value="True" />
'--  <add key="WhatHappened" value="There was an unexpected error in this website. This may be due to a programming bug." />
'--  <add key="HowUserAffected" value="The current page will not load." />
'--  <add key="WhatUserCanDo" value="Close your browser, navigate back to this website, and try repeating your last action. Try alternative methods of performing the same action. If problems persist, contact (contact)." />
'--
'--  <!-- SMTP email configuration defaults -->
'--  <add key="SmtpDefaultDomain" value="mydomain.com" />
'--  <add key="SmtpServer" value="mail.mydomain.com" />
'--  <add key="SmtpPort" value="25" />
'--  <add key="SmtpAuthUser" value="" />
'--  <add key="SmtpAuthPassword" value="" />
'--
*/

namespace Limnor.WebServer
{
	/// <summary>
	/// Retrieving typed values from the 
	/// <UnhandledException></UnhandledException> custom NameValueCollection .config section
	/// </summary>
	internal class Config
	{
		private const string _strSectionName = "UnhandledException";
		private static NameValueCollection _nvc;

		private static void Load()
		{
			if (_nvc != null)
				return;

			object o = null;
			try
			{
				o = System.Configuration.ConfigurationManager.GetSection(_strSectionName);
			}
			catch
			{
				//-- we are in an unhandled exception handler
			}
			if (o == null)
			{
				//-- we can work without any configuration at all (all defaults)
				_nvc = new NameValueCollection();
				return;
			}

			try
			{
				_nvc = (NameValueCollection)o;
			}
			catch (Exception ex)
			{
				//System.Configuration.ConfigurationErrorsException
				throw new ConfigurationErrorsException("The <" + _strSectionName + "> section is present in the .config file, but it does not appear to be a name value collection.", ex);
			}
		}

		/// <summary>
		/// retrieves integer value from the 
		/// <UnhandledException></UnhandledException> custom NameValueCollection .config section
		/// </summary>
		public static int GetInteger(string Key, int Default)
		{
			Load();
			string strTemp = _nvc.Get(Key);
			if (string.IsNullOrEmpty(strTemp))
			{
				return Default;
			}

			try
			{
				return Convert.ToInt32(strTemp);
			}
			catch
			{
				return Default;
			}
		}

		/// <summary>
		/// retrieves boolean value from the 
		/// <UnhandledException></UnhandledException> custom NameValueCollection .config section
		/// </summary>
		public static bool GetBoolean(string Key, params bool[] Defaults)
		{
			bool Default = false;
			if (Defaults != null && Defaults.Length > 0)
			{
				Default = Defaults[0];
			}
			Load();
			string strTemp = _nvc.Get(Key);
			if (string.IsNullOrEmpty(strTemp))
			{
				return Default;
			}
			if (string.Compare(strTemp, "true", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(strTemp, "1") == 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// retrieves string value from the 
		/// <UnhandledException></UnhandledException> custom NameValueCollection .config section
		/// </summary>
		public static string GetString(string Key, params string[] Defaults)
		{
			string Default = string.Empty;
			if (Defaults != null && Defaults.Length > 0)
			{
				Default = Defaults[0];
			}
			Load();
			string strTemp = _nvc.Get(Key);
			if (string.IsNullOrEmpty(strTemp))
			{
				return Default;
			}

			return strTemp;
		}

		/// <summary>
		/// retrieves relative or absolute path string from the 
		/// <UnhandledException></UnhandledException> custom NameValueCollection .config section
		/// </summary>
		public static string GetPath(string Key)
		{
			Load();
			string strPath = GetString(Key, "");

			//-- strip this because it's unnecessary (we assume website root, if path isn't rooted)
			if (strPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
			{
				strPath = strPath.Replace("~/", "");
			}
			if (Path.IsPathRooted(strPath))
			{
				return strPath;
			}
			else
			{
				return Path.Combine(AppBase, strPath);
			}
		}

		private static string AppBase
		{
			get
			{
				return Convert.ToString(System.AppDomain.CurrentDomain.GetData("APPBASE"));
			}
		}
	}
}
