/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Kiosk Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsUtility;
using System.ComponentModel;

namespace LimnorKiosk
{
	//http://www.itechtalk.com/thread8801.html
	/*
	 HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\WinLogon
AutoAdminLogon = 1
DefaultUserName = USERNAME
DefaultPassword = PASSWORD
DefaultDomainName = PCNAME
	 IgnoreShiftOverride = 1
	 ForceAutoLogon = 1
	 * 
	 * 
	 */
	/*
	 Each mapping DWORD consists of two parts the output scancode, and an input scancode. 
	 To disable a key set the output scan code to 00 00. 

Scancode Description 
5c e0 Windows Key 
5c e0 Windows Key 
5d e0 Windows Menu Key 
44 00 F10 
1d 00 Left Ctrl 
38 00 Left Alt  
1d e0 Right Ctrl 
38 e0 Right Alt 

To disable the list of keys above save the following lines to a file called "SCANCODE.REG".
	 * Right click on the REG file and selecte "Merge" to add the specified key and vlaue to your registry. 
	 * After you reboot, the keys will be disabled.

Registry Editor Version 5.00 
[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]
"Scancode Map"=hex:00,00,00,00,00,00,00,00,09,00,00,00,00,00,5b,e0,00,00,5c,e0,00,00,5d,e0,00,00, 44,00,00,00,1d,00,00,00,38,00,00,00,1d,e0,00,00,38,e0,00,00,00,00

	 */
	/// <summary>
	/// provide kiosk functionality
	/// </summary>
	public class LKiosk
	{
		static TaskMgrMonitor objTaskMgr;
		static bool _useOldWay = true;
		public LKiosk()
		{
		}
		static public void Enterkiosk()
		{
			if (_useOldWay)
			{
				if (objTaskMgr == null)
				{
					objTaskMgr = new TaskMgrMonitor();
				}
				objTaskMgr.Start();
				string s = string.Empty;
				try
				{
					int n1 = WinUtil.fnSetupKiosk();
					if (n1 != 0)
					{
						s = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error setup kiosk mode.\r\n{0}", WinUtil.GetWinAPIErrorMessage(n1));
					}
				}
				catch (Exception err)
				{
					s = err.Message;
				}
				if (!string.IsNullOrEmpty(s))
				{
					MessageBox.Show(null, s, "Enter kiosk", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		static public void Exitkiosk()
		{
			if (_useOldWay)
			{
				string s = string.Empty;
				if (objTaskMgr != null)
				{
					objTaskMgr.Stop();
					objTaskMgr = null;
				}
				try
				{
					int n1 = WinUtil.fnExitKiosk();
					if (n1 != 0)
					{
						s = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error exiting kiosk mode.\r\n{0}", WinUtil.GetWinAPIErrorMessage(n1));
					}
				}
				catch (Exception err)
				{
					s = string.Format("{0}\r\n{1}\r\n{2}", err.GetType().Name, err.Message, err.StackTrace);
				}
				if (!string.IsNullOrEmpty(s))
				{
					MessageBox.Show(null, s, "Exit kiosk", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		public static bool UseCustomKiosk
		{
			get
			{
				return _useOldWay;
			}
			set
			{
				_useOldWay = value;
			}
		}
	}
}
