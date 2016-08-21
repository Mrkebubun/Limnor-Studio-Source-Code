/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Kiosk Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using WindowsUtility;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Globalization;

namespace LimnorKiosk
{
	[SupportForm]
	[ToolboxBitmapAttribute(typeof(FormKioskBackground), "FormKioskBackground.bmp")]
	public partial class FormKioskBackground : Form, ICustomTypeDescriptor
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool RegisterHotKey(IntPtr hWnd, // handle to window    
			int id,            // hot key identifier    
			KeyModifiers fsModifiers,  // key-modifier options    
			Keys vk            // virtual-key code    
			);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool UnregisterHotKey(IntPtr hWnd,  // handle to window    
			int id      // hot key identifier    
			);

		[Flags()]
		enum KeyModifiers
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			Windows = 8
		}
		const int nHotkey = 100;
		private bool bExitHotkeySet;
		private bool bInitialized;
		private bool bClosing;
		private fnSimpleFunction _loadFirstForm;
		private Keys _exitHotKey = Keys.F10;
		private System.Threading.Timer _timLoader;
		public FormKioskBackground()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		private void _loadingHomePage(object state)
		{
			this.Invoke((MethodInvoker)(delegate()
			{
				_loadFirstForm();
				_timLoader = null;
			}));
		}
		public FormKioskBackground(fnSimpleFunction loadFirstForm)
		{
			_loadFirstForm = loadFirstForm;
			InitializeComponent();
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			LKiosk.Enterkiosk();
		}
		public static bool ObjectToBool(object v)
		{
			if (v == null || v == DBNull.Value)
			{
				return false;
			}
			string s = v as string;
			if (!string.IsNullOrEmpty(s))
			{
				if (string.Compare(s, "on", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				if (string.Compare(s, "yes", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				if (string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				return false;
			}
			TypeCode tc = Type.GetTypeCode(v.GetType());
			switch (tc)
			{
				case TypeCode.Byte: return ((byte)v) != 0;
				case TypeCode.Decimal: return ((decimal)v) != 0;
				case TypeCode.Double: return ((double)v) != 0;
				case TypeCode.Int16: return ((Int16)v) != 0;
				case TypeCode.Int32: return ((Int32)v) != 0;
				case TypeCode.Int64: return ((Int64)v) != 0;
				case TypeCode.SByte: return ((sbyte)v) != 0;
				case TypeCode.Single: return ((float)v) != 0;
				case TypeCode.UInt16: return (UInt16)v != 0;
				case TypeCode.UInt32: return (UInt32)v != 0;
				case TypeCode.UInt64: return (UInt64)v != 0;
			}
			return Convert.ToBoolean(v, CultureInfo.InvariantCulture);
		}
		private void onExit()
		{
			try
			{
				//
				string regPath;
				regPath = @"SOFTWARE\Longflow\LimnorStudio\Kiosk";
				RegistryKey reg = Registry.LocalMachine.OpenSubKey(regPath, false);
				if (reg != null)
				{
					object v = reg.GetValue("RebootOnExit", false, RegistryValueOptions.None);
					if (v != null)
					{
						bool reboot = ObjectToBool(v);
						if (reboot)
						{
							WinUtil.Reboot();
							return;
						}
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Exit kiosk", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (bExitHotkeySet)
			{
				if (UnregisterHotKey(Handle, nHotkey))
				{
					bExitHotkeySet = false;
				}
			}
			if (LKiosk.UseCustomKiosk)
			{
				string kioskMode = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "KioskMode.exe");
				if (File.Exists(kioskMode))
				{
					Process p = new Process();
					p.StartInfo.FileName = kioskMode;
					p.Start();
				}
			}
		}
		public string ExitCode { get; set; }
		public void SetExitKey(string k)
		{
			_exitHotKey = (Keys)Enum.Parse(typeof(Keys), k);
		}
		public void ExitKiosk()
		{
			timerClose.Enabled = true;
		}
		private bool _closed;
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (!_closed)
			{
				onExit();
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			onExit();
			_closed = true;
		}
		protected override void OnActivated(EventArgs e)
		{
			if (!bInitialized)
			{
				bInitialized = true;
				_timLoader = new System.Threading.Timer(new System.Threading.TimerCallback(_loadingHomePage), null, 300, Timeout.Infinite);
			}
			if (!bExitHotkeySet)
			{
				KeyModifiers km = KeyModifiers.None;
				if ((Keys.Alt & _exitHotKey) != 0)
				{
					km |= KeyModifiers.Alt;
				}
				if ((Keys.Shift & _exitHotKey) != 0)
				{
					km |= KeyModifiers.Shift;
				}
				if ((Keys.Control & _exitHotKey) != 0)
				{
					km |= KeyModifiers.Control;
				}
				Keys k = _exitHotKey & ~(Keys.Alt) & ~(Keys.Shift) & ~(Keys.Control);
				bExitHotkeySet = RegisterHotKey(Handle, nHotkey, km, k);
			}
			base.OnActivated(e);
		}
		protected override void WndProc(ref Message m)
		{
			if (!bClosing)
			{
				try
				{
					const int WM_HOTKEY = 0x0312;
					switch (m.Msg)
					{
						case WM_HOTKEY:
							if ((int)(m.WParam) == nHotkey)
							{
								timerClose.Enabled = true;
							}
							break;
					}
				}
				catch
				{
				}
			}
			base.WndProc(ref m);
		}

		private void timerClose_Tick(object sender, EventArgs e)
		{
			timerClose.Enabled = false;
			if (!bClosing)
			{
				bClosing = true;
				FormExitCode dlg = new FormExitCode();
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (dlg.ExitCode == this.ExitCode)
					{
						this.Close();
					}
					else
					{
						bClosing = false;
					}
				}
				else
				{
					bClosing = false;
				}
			}
		}

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			StringCollection ss = new StringCollection();
			ss.Add("BackColor");
			ss.Add("BackgroundImage");
			ss.Add("BackgroundImageLayout");
			ss.Add("ContextMenu");
			ss.Add("ContextMenuStrip");
			ss.Add("Cursor");
			ss.Add("DefaultCursor");
			ss.Add("Font");
			ss.Add("FontHeight");
			ss.Add("ForeColor");
			ss.Add("KeyPreview");
			ss.Add("MainMenuStrip");
			ss.Add("Menu");
			ss.Add("MergedMenu");
			ss.Add("OwnedForms");
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (ss.Contains(p.Name))
				{
					props.Add(p);
				}
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
}
