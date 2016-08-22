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
	public class WebKeyEventArgs : EventArgs
	{
		bool _altKey;
		bool _ctrlKey;
		bool _shiftKey;
		bool _metaKey;
		int _keyCode;
		public WebKeyEventArgs(bool alt, bool ctrl, bool shift, bool meta, int keyIdentifier, int keyLocation, int key)
		{
			_altKey = alt;
			_ctrlKey = ctrl;
			_shiftKey = shift;
			_metaKey = meta;
			_keyCode = key;
		}
		public bool altKey { get { return _altKey; } }
		public bool ctrlKey { get { return _ctrlKey; } }
		public bool shiftKey { get { return _shiftKey; } }
		public bool metaKey { get { return _metaKey; } }
		public int keyCode { get { return _keyCode; } }
	}

}
