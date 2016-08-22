/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Limnor.Windows
{
	[Description("It represents a control found in a window")]
	public class WindowControl
	{
		private IntPtr _controlHandle;
		private IntPtr _parentWindow;
		private int _id;
		private string _text;
		public WindowControl()
		{
		}
		[Description("The window handle for the control")]
		public IntPtr ControlHandle
		{
			get
			{
				return _controlHandle;
			}
			set
			{
				_controlHandle = value;
				_parentWindow = IntPtr.Zero;
				_id = 0;
				_text = string.Empty;
			}
		}
		[Description("The window handle for the parent window of the control")]
		public IntPtr ParentHandle
		{
			get
			{
				if (_parentWindow == IntPtr.Zero && _controlHandle != IntPtr.Zero)
				{
					_parentWindow = NativeMethods.GetParent(_controlHandle);
				}
				return _parentWindow;
			}
		}
		[Description("The id for the control")]
		public int ControlId
		{
			get
			{
				if (_id == 0 && _controlHandle != IntPtr.Zero)
				{
					_id = NativeMethods.GetDlgCtrlID(_controlHandle);
				}
				return _id;
			}
		}
		[Description("The text on the control")]
		public string Text
		{
			get
			{
				if (_controlHandle != IntPtr.Zero && _text == string.Empty)
				{
					_text = NativeMethods.GetWindowText(_controlHandle);
				}
				return _text;
			}
		}
	}
}
