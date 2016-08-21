/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Information Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace LimnorWeb
{
	/*
	 * Samples:
	 CreateAppPool("IIS://Localhost/W3SVC/AppPools", "MyAppPool");
	 CreateVDir("IIS://Localhost/W3SVC/1/Root", "MyVDir", "D:\\Inetpub\\Wwwroot");
	 AssignVDirToAppPool("IIS://Localhost/W3SVC/1/Root/MyVDir", "MyAppPool");

	 */
	public enum EnumWindowsVersion
	{
		Unknown, Win31, Win95, Win98, WinME, WinNT351, WinNT40,
		Win2000, WinXP, Win2003, Vista_2008Srv
	}
	public enum EnumIIS { Unknown, IIS51, IIS6, IIS7 }
	public sealed class IisUtility
	{
		internal const string LocalWebPath = "IIS://localhost/W3SVC/1/Root";
		public static EnumIIS GetIisVersion()
		{
			EnumWindowsVersion os = GetOSVersion();
			switch (os)
			{
				case EnumWindowsVersion.WinXP:
					return EnumIIS.IIS51;
				case EnumWindowsVersion.Vista_2008Srv:
					return EnumIIS.IIS6;
			}
			if (Environment.OSVersion.Version.Major == 6)
			{
				return EnumIIS.IIS6;
			}
			if (Environment.OSVersion.Version.Major > 6)
			{
				return EnumIIS.IIS7;
			}
			return EnumIIS.Unknown;
		}
		public static EnumWindowsVersion GetOSVersion()
		{
			EnumWindowsVersion ret = EnumWindowsVersion.Unknown;
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
					ret = EnumWindowsVersion.Win31;
					break;
				case PlatformID.Win32Windows:
					{
						if (Environment.OSVersion.Version.Minor == 0)
						{
							ret = EnumWindowsVersion.Win95;
						}
						else if (Environment.OSVersion.Version.Minor == 10)
						{
							ret = EnumWindowsVersion.Win98;
						}
						else if (Environment.OSVersion.Version.Minor == 90)
						{
							ret = EnumWindowsVersion.WinME;
						}
					}
					break;
				case PlatformID.Win32NT:
					switch (Environment.OSVersion.Version.Major)
					{
						case 3:
							ret = EnumWindowsVersion.WinNT351;
							break;
						case 4:
							ret = EnumWindowsVersion.WinNT40;
							break;
						case 5:
							switch (Environment.OSVersion.Version.Minor)
							{
								case 0:
									ret = EnumWindowsVersion.Win2000;
									break;
								case 1:
									ret = EnumWindowsVersion.WinXP;
									break;
								case 2:
									ret = EnumWindowsVersion.Win2003;
									break;
							}
							break;
						case 6:
							ret = EnumWindowsVersion.Vista_2008Srv;
							break;
					}
					break;
			}
			return ret;
		}
		public static void CreateAppPool(string metabasePath, string appPoolName)
		{
			//  metabasePath is of the form "IIS://<servername>/W3SVC/AppPools"
			//    for example "IIS://localhost/W3SVC/AppPools" 
			//  appPoolName is of the form "<name>", for example, "MyAppPool"

			if (!metabasePath.EndsWith("/W3SVC/AppPools", StringComparison.OrdinalIgnoreCase))
			{
				metabasePath = string.Format(CultureInfo.InvariantCulture, "{0}/W3SVC/AppPools", metabasePath);
			}
			if (!metabasePath.StartsWith("IIS://", StringComparison.OrdinalIgnoreCase))
			{
				metabasePath = string.Format(CultureInfo.InvariantCulture, "IIS://{0}", metabasePath);
			}
			DirectoryEntry newpool = null;
			DirectoryEntry apppools = new DirectoryEntry(metabasePath);
			IEnumerator ie = apppools.Children.GetEnumerator();
			while (ie.MoveNext())
			{
				DirectoryEntry de = ie.Current as DirectoryEntry;
				if (de != null)
				{
					if (string.Compare(de.Name, appPoolName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						newpool = de;
						break;
					}
				}
			}
			if (newpool == null)
			{
				newpool = apppools.Children.Add(appPoolName, "IIsApplicationPool");
				newpool.Properties["enable32BitAppOnWin64"].Value = true;
#if DOTNET40
				newpool.Properties["managedRuntimeVersion"].Value = "v4.0";
#else
				newpool.Properties["managedRuntimeVersion"].Value = "v2.0";
#endif
				newpool.CommitChanges();
			}

		}
		public static void CreateVDir(string metabasePath, string vDirName, string physicalPath)
		{
			//  metabasePath is of the form "IIS://<servername>/<service>/<siteID>/Root[/<vdir>]"
			//    for example "IIS://localhost/W3SVC/1/Root" 
			//  vDirName is of the form "<name>", for example, "MyNewVDir"
			//  physicalPath is of the form "<drive>:\<path>", for example, "C:\Inetpub\Wwwroot"
			DirectoryEntry site = new DirectoryEntry(metabasePath);
			string className = site.SchemaClassName.ToString();
			if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
			{
				DirectoryEntries vdirs = site.Children;
				DirectoryEntry newVDir = vdirs.Add(vDirName, (className.Replace("Service", "VirtualDir")));
				newVDir.Properties["Path"][0] = physicalPath;
				newVDir.Properties["AccessScript"][0] = true;
				// These properties are necessary for an application to be created.
				newVDir.Properties["AppFriendlyName"][0] = vDirName;
				newVDir.Properties["AppIsolated"][0] = "1";
				newVDir.Properties["AppRoot"][0] = "/LM" + metabasePath.Substring(metabasePath.IndexOf("/", ("IIS://".Length)));

				newVDir.CommitChanges();

			}
			else
			{
				throw new Exception(" Failed. A virtual directory can only be created in a site or virtual directory node.");
			}
		}
		/// <summary>
		/// Path property is the physical file path
		/// </summary>
		/// <param name="metabasePath"></param>
		/// <param name="vDirName"></param>
		/// <returns></returns>
		public static DirectoryEntry GetVDir(string metabasePath, string vDirName)
		{
			DirectoryEntry site = new DirectoryEntry(metabasePath);
			string className = site.SchemaClassName.ToString();
			if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
			{
				string path = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", metabasePath, vDirName);
				DirectoryEntries vdirs = site.Children;
				IEnumerator ie = vdirs.GetEnumerator();
				while (ie.MoveNext())
				{
					DirectoryEntry de = ie.Current as DirectoryEntry;
					if (de != null)
					{
						if (string.Compare(path, de.Path, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return de;
						}
					}
				}
			}
			return null;
		}
		public static IEnumerator GetSites(string metabasePath)
		{
			DirectoryEntry site = new DirectoryEntry(metabasePath);
			string className = site.SchemaClassName.ToString();
			if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
			{
				DirectoryEntries vdirs = site.Children;
				return vdirs.GetEnumerator();
			}
			return null;
		}
		public static IList<VirtualWebDir> GetVirtualDirectories(string metabasePath)
		{
			List<VirtualWebDir> list = new List<VirtualWebDir>();
			DirectoryEntry site = new DirectoryEntry(metabasePath);
			string className = site.SchemaClassName.ToString();
			if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
			{
				DirectoryEntries vdirs = site.Children;
				IEnumerator ie = vdirs.GetEnumerator();
				while (ie.MoveNext())
				{
					DirectoryEntry de = ie.Current as DirectoryEntry;
					if (de != null)
					{
						string dir = null;
						PropertyValueCollection pv = de.Properties["Path"];
						if (pv != null)
						{
							if (pv.Value != null)
							{
								dir = pv.Value.ToString();
								VirtualWebDir vd = new VirtualWebDir(de.Path, dir);
								list.Add(vd);
							}
						}
					}
				}
			}
			return list;
		}
		public static void AssignVDirToAppPool(string metabasePath, string appPoolName)
		{
			//  metabasePath is of the form "IIS://<servername>/W3SVC/<siteID>/Root[/<vDir>]"
			//    for example "IIS://localhost/W3SVC/1/Root/MyVDir" 
			//  appPoolName is of the form "<name>", for example, "MyAppPool"
			DirectoryEntry vDir = new DirectoryEntry(metabasePath);
			string className = vDir.SchemaClassName.ToString();
			if (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase))
			{
				object[] param = { 0, appPoolName, true };
				vDir.Invoke("AppCreate3", param);
				vDir.Properties["AppIsolated"][0] = "2";
			}
			else
			{
				throw new Exception(" Failed in AssignVDirToAppPool; only virtual directories can be assigned to application pools");
			}
		}

		public static void CreateLocalWebSite(string virtualDir, string physicalDir)
		{
			EnumIIS iis = IisUtility.GetIisVersion();
			IisUtility.CreateVDir(LocalWebPath, virtualDir, physicalDir);
			if (iis == EnumIIS.IIS6 || iis == EnumIIS.IIS7)
			{
#if DOTNET40
				string appName = "Limnor Studio 5";
#else
				string appName = "Limnor Studio";// string.Format(CultureInfo.InvariantCulture, "app{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
#endif
				IisUtility.CreateAppPool("localhost", appName);
				IisUtility.AssignVDirToAppPool(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", LocalWebPath, virtualDir), appName);
			}
		}
		public static string FindLocalRootWebPath(Form owner, out bool iisError)
		{
			iisError = false;
			using (ShowMessage frm = new ShowMessage(owner, "Checking web site name..."))
			{
				DirectoryEntry site = new DirectoryEntry(LocalWebPath);
				PropertyValueCollection pv = site.Properties["Path"];
				if (pv != null)
				{
					if (pv.Value != null)
					{
						return pv.Value.ToString();
					}
				}
			}
			return null;
		}
		public static VirtualWebDir FindLocalWebSiteByName(Form owner, string webName, out bool iisError)
		{
			iisError = false;
			using (ShowMessage frm = new ShowMessage(owner, "Checking web site name..."))
			{
				DirectoryEntry site = new DirectoryEntry(LocalWebPath);
				string className = string.Empty;
				try
				{
					className = site.SchemaClassName.ToString();
				}
				catch (Exception err)
				{
					StringBuilder sb = new StringBuilder();
					while (err != null)
					{
						sb.Append("Error finding web site via Active Directory. Please install 'IIS6 Metabase compatibility' if it is not installed.\r\nError message:");
						sb.Append(err.Message);
						sb.Append("\r\nStack trace:");
						if (!string.IsNullOrEmpty(err.StackTrace))
						{
							sb.Append(err.StackTrace);
						}
						sb.Append("===============\r\n");
						err = err.InnerException;
					}
					MessageBox.Show(sb.ToString(), "Find web site", MessageBoxButtons.OK, MessageBoxIcon.Error);
					iisError = true;
					return null;
				}
				if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
				{
					DirectoryEntries vdirs = site.Children;
					IEnumerator ie = vdirs.GetEnumerator();
					while (ie.MoveNext())
					{
						DirectoryEntry de = ie.Current as DirectoryEntry;
						if (de != null)
						{
							if (string.Compare(de.Name, webName, StringComparison.OrdinalIgnoreCase) == 0)
							{
								string dir = null;
								PropertyValueCollection pv = de.Properties["Path"];
								if (pv != null)
								{
									if (pv.Value != null)
									{
										dir = pv.Value.ToString();
										VirtualWebDir vd = new VirtualWebDir(de.Path, dir);
										return vd;
									}
								}
								else
								{
								}
							}
						}
					}
				}
			}
			return null;
		}
		public static VirtualWebDir FindLocalWebSite(string virtualDir)
		{
			DirectoryEntry site = new DirectoryEntry(LocalWebPath);
			string className = site.SchemaClassName.ToString();
			if ((className.EndsWith("Server", StringComparison.OrdinalIgnoreCase)) || (className.EndsWith("VirtualDir", StringComparison.OrdinalIgnoreCase)))
			{
				DirectoryEntries vdirs = site.Children;
				IEnumerator ie = vdirs.GetEnumerator();
				while (ie.MoveNext())
				{
					DirectoryEntry de = ie.Current as DirectoryEntry;
					if (de != null)
					{
						string sn = Path.GetFileName(de.Path);
						if (string.Compare(virtualDir, sn, StringComparison.OrdinalIgnoreCase) == 0)
						{
							string dir = null;
							PropertyValueCollection pv = de.Properties["Path"];
							if (pv != null)
							{
								if (pv.Value != null)
								{
									dir = pv.Value.ToString();
									VirtualWebDir vd = new VirtualWebDir(de.Path, dir);
									return vd;
								}
							}
						}
					}
				}
			}
			return null;
		}
	}
}
