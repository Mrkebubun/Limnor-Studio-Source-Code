/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Information Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace LimnorWeb
{
	public class VirtualWebDir
	{
		private string _vdir;
		private string _folder;
		public static EventHandler OnValidationError;
		public VirtualWebDir(string virtualDir, string physicalDir)
		{
			_vdir = virtualDir;
			_folder = physicalDir;
		}
		public string WebName
		{
			get
			{
				return Path.GetFileName(_vdir);
			}
		}
		public string VirtualDirectory
		{
			get
			{
				return _vdir;
			}
		}
		public string PhysicalDirectory
		{
			get
			{
				return _folder;
			}
		}
		public bool IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(_vdir) && !string.IsNullOrEmpty(_folder))
				{
					return true;
				}
				if (OnValidationError != null)
				{
					string LastValidationError = string.Format(CultureInfo.InvariantCulture, "(_vdir:{2},_folder:{3}) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, _vdir, _folder);
					OnValidationError(LastValidationError, EventArgs.Empty);
				}
				return false;
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} - {1}", _vdir, _folder);
		}
		public static bool IsNetworkDrive(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				ManagementObject mo = new ManagementObject();
				if (path.StartsWith(@"\\", StringComparison.Ordinal)) { return true; }
				// Get just the drive letter for WMI call
				string driveletter = Directory.GetDirectoryRoot(path).Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);
				mo.Path = new ManagementPath(string.Format(CultureInfo.InvariantCulture, "Win32_LogicalDisk='{0}'", driveletter));
				// Get the data we need
				uint driveType = Convert.ToUInt32(mo["DriveType"]);
				mo = null;
				return driveType == 4;
			}
			return false;
		}
		public static void AddDirectorySecurity(string path)
		{
			try
			{
				DirectoryInfo dInfo = new DirectoryInfo(path);
				DirectorySecurity dSecurity = dInfo.GetAccessControl();
				dSecurity.AddAccessRule(new FileSystemAccessRule("IUSR", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
				dSecurity.AddAccessRule(new FileSystemAccessRule("BUILTIN\\IIS_IUSRS", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
				dInfo.SetAccessControl(dSecurity);
			}
			catch (Exception err)
			{
				MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error setting folder permissions to {0}. {1}", path, err.Message));
			}
		}
	}
}
