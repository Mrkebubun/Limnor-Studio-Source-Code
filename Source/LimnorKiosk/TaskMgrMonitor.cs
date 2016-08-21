/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Kiosk Support
 * License: GNU General Public License v3.0
 
 */
using System;

namespace LimnorKiosk
{
	/// <summary>
	/// Summary description for TaskMgrMonitor.
	/// </summary>
	public class TaskMgrMonitor : RegistryMonitor , IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		public TaskMgrMonitor()
		{
			//
			RegChanged +=new EventHandler(TaskMgrMonitor_RegChanged);
			//
		}
		#region IDisposable Members

		#endregion
		/// <summary>
		/// Start monitoring
		/// </summary>
		public override void Start()
		{
			if (!IsMonitoring)
			{
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", false);
				if (key == null)
				{
				}
				else
				{
					RegistryKey = key;
					base.Start();
				}
			}
		}

		private void TaskMgrMonitor_RegChanged(object sender, EventArgs e)
		{
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
			if (key == null)
			{
				throw new Exception("Invalid registry key");
			}
			else
			{
				try
				{
					key.SetValue("DisableTaskMgr",1);
					key.SetValue("DisableLockWorkstation",1);
					key.SetValue("DisableChangePassword",1);
				}
				catch
				{
				}
				finally
				{
					key.Close();
				}
			}
		}
	}
}
