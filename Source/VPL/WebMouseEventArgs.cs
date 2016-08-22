/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public enum WebMouseButton { Left = 1, Middle = 4, Right = 2 }
	public class WebMouseEventArgs : EventArgs
	{
		bool _altKey;
		bool _ctrlKey;
		bool _shiftKey;
		WebMouseButton _button;
		int _clientX;
		int _clientY;
		int _screenX;
		int _screenY;
		public WebMouseEventArgs(bool alt, bool ctrl, bool shift, WebMouseButton bt, int clientx, int clienty, int screenx, int screeny)
		{
			_altKey = alt;
			_ctrlKey = ctrl;
			_shiftKey = shift;
			_button = bt;
			_clientX = clientx;
			_clientY = clienty;
			_screenX = screenx;
			_screenY = screeny;
		}
		public bool altKey { get { return _altKey; } }
		public bool ctrlKey { get { return _ctrlKey; } }
		public bool shiftKey { get { return _shiftKey; } }
		public WebMouseButton button { get { return _button; } }
		public int clientX { get { return _clientX; } }
		public int clientY { get { return _clientY; } }
		public int screenX { get { return _screenX; } }
		public int screenY { get { return _screenY; } }
	}

}
