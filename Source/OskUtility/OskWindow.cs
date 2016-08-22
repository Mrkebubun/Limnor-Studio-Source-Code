/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	On-Screen-Keyboard component
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace OskUtility
{
	[ToolboxBitmapAttribute(typeof(OskWindow), "Resources.osk.bmp")]
	[Description("This object manages on screen keyboard.")]
	public class OskWindow : IComponent
	{
		#region fields and constructors
		private string _errStr;
		private IntPtr _oskHandle;
		private Process _oskProcess;
		private int _borderStyle;
		//
		private Point _pos = new Point(0, 0);
		private bool _disableCloseButton;
		private bool _disableMiniButton;
		private bool _visible = true;
		private bool _movable = true;
		public OskWindow()
		{
		}
		public OskWindow(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			CloseOsk();
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region private methods
		private bool findWindwo(IntPtr hWnd, IntPtr lParam)
		{
			int processID;
			WindowsNative.GetWindowThreadProcessId(hWnd, out processID);
			if (_oskProcess.Id == processID)
			{
				_oskHandle = hWnd;
				return false;
			}
			return true;
		}
		private void loadOsk()
		{
			_errStr = string.Empty;
			if (Site != null && Site.DesignMode)
			{
			}
			else
			{
				if (_oskHandle == IntPtr.Zero)
				{
					string sys32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
					string oskFile = System.IO.Path.Combine(sys32, "osk.exe");
					if (!System.IO.File.Exists(oskFile))
					{
						_errStr = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"On-screen-keyboad not found at {0}", oskFile);
					}
					else
					{
						_oskProcess = new Process();
						try
						{
							_oskProcess.StartInfo.FileName = oskFile;
							_oskProcess.Exited += new EventHandler(_oskProcess_Exited);
							_oskProcess.Start();
							if (_oskProcess.WaitForInputIdle(3000))
							{
								if (_oskProcess.MainWindowHandle != IntPtr.Zero)
								{
									_oskHandle = _oskProcess.MainWindowHandle;
								}
								else
								{
									WindowsNative.EnumWindows(new EnumWindowsProc(findWindwo), 0);
									if (_oskHandle == IntPtr.Zero)
									{
										_errStr = "Handle for on-screen-keyboard not found";
									}
								}
								if (_oskHandle != IntPtr.Zero)
								{
									_borderStyle = WindowsNative.GetWindowBoarderStyle(_oskHandle);
									WindowsNative.MoveWindow(_oskHandle, _pos.X, _pos.Y);
									if (!_movable)
									{
										WindowsNative.RemoveWindowBoarder(_oskHandle, _borderStyle);
									}
									if (_disableCloseButton)
									{
										WindowsNative.DisableCloseButton(_oskHandle);
									}
									if (_disableMiniButton)
									{
										WindowsNative.DisableMinimizeButton(_oskHandle);
									}
								}
							}
						}
						catch (InvalidOperationException)
						{
						}
						catch (Exception er)
						{
							_errStr = string.Format(CultureInfo.InvariantCulture, "{0}. {1}", er.Message, er.GetType().FullName);
						}
					}
				}
			}
		}

		void _oskProcess_Exited(object sender, EventArgs e)
		{
			_oskHandle = IntPtr.Zero;
		}
		#endregion

		#region Methods
		[Description("Start on-screen-keyboard")]
		public void StartOsk()
		{
			loadOsk();
			if (_oskHandle == IntPtr.Zero && !string.IsNullOrEmpty(_errStr))
			{
				MessageBox.Show(null, _errStr, "Load OSK", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		[Description("Close on-screen-keyboard")]
		public void CloseOsk()
		{
			if (_oskHandle != IntPtr.Zero)
			{
				WindowsNative.CloseWindow2(_oskHandle);
				_oskHandle = IntPtr.Zero;
			}
		}
		#endregion

		#region Properties
		[Description("Gets a Boolean value indicating whether the on screen keyboard is loaded successfully")]
		public bool OskLoaded
		{
			get
			{
				return (_oskHandle != IntPtr.Zero && WindowsNative.IsWindow(_oskHandle));
			}
		}
		[Description("Gets a string value representing the error message for the last action")]
		public string ErrorMessage
		{
			get
			{
				return _errStr;
			}
		}
		[Description("The coordinates of the upper-left corner of the on-screen-keyboard")]
		public Point Location
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
				if (OskLoaded)
				{
					WindowsNative.MoveWindow(_oskHandle, _pos.X, _pos.Y);
				}
			}
		}
		[Description("Gets a value indicating whether the close button should be disabled")]
		public bool DisableCloseButton
		{
			get
			{
				return _disableCloseButton;
			}
			set
			{
				if (_disableCloseButton != value)
				{
					_disableCloseButton = value;
					if (OskLoaded)
					{
						if (_disableCloseButton)
						{
							WindowsNative.DisableCloseButton(_oskHandle);
						}
						else
						{
							WindowsNative.EnableCloseButton(_oskHandle);
						}
					}
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets a value indicating whether the on-screen-keyboard is visible")]
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if (_visible != value)
				{
					_visible = value;
					if (OskLoaded)
					{
						WindowsNative.SetWindowVisible(_oskHandle, _visible);
					}
				}
			}
		}
		[DefaultValue(false)]
		[Description("Gets a value indicating whether the minimize button is disabled for the on-screen-keyboard")]
		public bool DisableMinimizeButton
		{
			get
			{
				return _disableMiniButton;
			}
			set
			{
				if (_disableMiniButton != value)
				{
					_disableMiniButton = value;
					if (OskLoaded)
					{
						if (_disableMiniButton)
						{
							WindowsNative.EnableMinimizeButton(_oskHandle);
						}
						else
						{
							WindowsNative.DisableMinimizeButton(_oskHandle);
						}
					}
				}
			}
		}
		#endregion
	}
}
