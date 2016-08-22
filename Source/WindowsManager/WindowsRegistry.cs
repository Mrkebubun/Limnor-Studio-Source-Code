/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace Limnor.Windows
{
	[ToolboxBitmapAttribute(typeof(WindowsManager), "Resources.reg16.bmp")]
	[Description("This object provides Windows Registry operations.")]
	public partial class WindowsRegistry : Component
	{
		#region fields and constructors
		private EnumRegistryCategory _cat = EnumRegistryCategory.CurrentUser;
		public WindowsRegistry()
		{
			InitializeComponent();
			init();
		}

		public WindowsRegistry(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
			init();
		}
		private void init()
		{

		}
		#endregion

		#region Properties
		[Description("Gets and sets registry category")]
		[DefaultValue(EnumRegistryCategory.CurrentUser)]
		public EnumRegistryCategory Category
		{
			get
			{
				return _cat;
			}
			set
			{
				if (_cat != value)
				{
					_cat = value;
				}
			}
		}
		[Description("Gets and sets registry subkey")]
		public string SubKey
		{
			get;
			set;
		}
		[Description("Gets and sets registry value name.")]
		public string ValueName
		{
			get;
			set;
		}
		[XmlIgnore]
		[Description("Gets and sets a registry value. Registry location is determined by ValueName, SubKey and Category")]
		public object Value
		{
			get
			{
				return GetRegistryValue(this.Category, this.SubKey, this.ValueName);
			}
			set
			{
				if (VPL.VPLUtil.IsInDesignMode(this))
				{
				}
				else
				{
					SetRegistryValue(this.Category, this.SubKey, this.ValueName, value);
				}
			}
		}
		[Description("Gets and sets a registry value as a string. Registry location is determined by ValueName, SubKey and Category")]
		public string ValueString
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return v.ToString();
					}
				}
				return null;
			}
		}
		[Description("Gets and sets a registry value as a 16-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public Int16 ValueInt16
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToInt16(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a 32-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public Int32 ValueInt32
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToInt32(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a 64-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public Int64 ValueInt64
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToInt64(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a DateTime. Registry location is determined by ValueName, SubKey and Category")]
		public DateTime ValueDateTime
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToDateTime(v);
					}
				}
				return DateTime.MinValue;
			}
		}
		[Description("Gets and sets a registry value as a double-precision value. Registry location is determined by ValueName, SubKey and Category")]
		public double ValueDouble
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToDouble(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a Boolean. Registry location is determined by ValueName, SubKey and Category")]
		public bool ValueBoolean
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToBoolean(v);
					}
				}
				return false;
			}
		}
		[Description("Gets and sets a registry value as a byte. Registry location is determined by ValueName, SubKey and Category")]
		public byte ValueByte
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToByte(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a character. Registry location is determined by ValueName, SubKey and Category")]
		public char ValueChar
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToChar(v);
					}
				}
				return '\0';
			}
		}
		[Description("Gets and sets a registry value as a decimal. Registry location is determined by ValueName, SubKey and Category")]
		public decimal ValueDecimal
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToDecimal(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a single-precision floating-point number. Registry location is determined by ValueName, SubKey and Category")]
		public float ValueFloat
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToSingle(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a signed byte. Registry location is determined by ValueName, SubKey and Category")]
		public sbyte ValueSByte
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToSByte(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a unsigned 64-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public UInt64 ValueUInt64
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToUInt64(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a unsigned 32-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public UInt32 ValueUInt32
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToUInt32(v);
					}
				}
				return 0;
			}
		}
		[Description("Gets and sets a registry value as a unsigned 16-bit integer. Registry location is determined by ValueName, SubKey and Category")]
		public UInt16 ValueUInt16
		{
			get
			{
				if (!VPL.VPLUtil.IsInDesignMode(this))
				{
					object v = this.Value;
					if (v != null)
					{
						return Convert.ToUInt16(v);
					}
				}
				return 0;
			}
		}
		#endregion
		#region Methods
		public static object GetRegistryValue(EnumRegistryCategory category, string subkey, string valueName)
		{
			if (!string.IsNullOrEmpty(subkey) && !string.IsNullOrEmpty(valueName))
			{
				object v = null;
				RegistryKey key = null;
				switch (category)
				{
					case EnumRegistryCategory.ClassRoot:
						key = Registry.ClassesRoot.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.CurrentConfig:
						key = Registry.CurrentConfig.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.CurrentUser:
						key = Registry.CurrentUser.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.DynData:
						key = Registry.DynData.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.LocalMachine:
						key = Registry.LocalMachine.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.PerformanceData:
						key = Registry.PerformanceData.OpenSubKey(subkey, false);
						break;
					case EnumRegistryCategory.Users:
						key = Registry.Users.OpenSubKey(subkey, false);
						break;
				}
				if (key != null)
				{
					try
					{
						v = key.GetValue(valueName);
					}
					catch
					{
						throw;
					}
					finally
					{
						key.Close();
					}
					return v;
				}
			}
			return null;
		}
		public static void SetRegistryValue(EnumRegistryCategory category, string subkey, string valueName, object value)
		{
			if (!string.IsNullOrEmpty(subkey) && !string.IsNullOrEmpty(valueName))
			{
				RegistryKey key = null;
				switch (category)
				{
					case EnumRegistryCategory.ClassRoot:
						key = Registry.ClassesRoot.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.CurrentConfig:
						key = Registry.CurrentConfig.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.CurrentUser:
						key = Registry.CurrentUser.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.DynData:
						key = Registry.DynData.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.LocalMachine:
						key = Registry.LocalMachine.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.PerformanceData:
						key = Registry.PerformanceData.CreateSubKey(subkey);
						break;
					case EnumRegistryCategory.Users:
						key = Registry.Users.CreateSubKey(subkey);
						break;
				}
				if (key != null)
				{
					try
					{
						key.SetValue(valueName, value);
					}
					catch
					{
						throw;
					}
					finally
					{
						key.Close();
					}
				}
			}
		}
		public object GetValue(string valueName)
		{
			return GetRegistryValue(this.Category, this.SubKey, valueName);
		}
		public object GetValue(string subkey, string valueName)
		{
			return GetRegistryValue(this.Category, subkey, valueName);
		}
		public void SetValue(string valueName, object value)
		{
			SetRegistryValue(this.Category, this.SubKey, valueName, value);
		}
		public void SetValue(string subkey, string valueName, object value)
		{
			SetRegistryValue(this.Category, subkey, valueName, value);
		}
		#endregion
	}
	public enum EnumRegistryCategory
	{
		LocalMachine = 0,
		Users = 1,
		ClassRoot = 2,
		CurrentUser = 3,
		CurrentConfig = 4,
		DynData=5,
		PerformanceData =6
	}
}
