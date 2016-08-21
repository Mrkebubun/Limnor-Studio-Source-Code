/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MathExp
{
	public class PositionUndo : IUndoUnit
	{
		UInt32 _ctrlKey;
		Point _location;
		IUndoHost _host;
		public PositionUndo(IUndoHost host, UInt32 objKey, Point pos)
		{
			if (objKey == 0)
			{
				throw new MathException("Cannot create PositionUndo with objKey=0");
			}
			_host = host;
			_location = pos;
			_ctrlKey = objKey;
		}

		#region IUndoUnit Members

		public void Apply()
		{
			bool b = _host.DisableUndo;
			_host.DisableUndo = true;
			Control c = _host.GetUndoControl(_ctrlKey);
			if (c != null)
			{
				c.Location = _location;
				if (c.Parent != null)
				{
					ActiveDrawing.RefreshControl(c.Parent);
				}
			}
			_host.DisableUndo = b;
		}

		#endregion
	}
	public class SizeUndo : IUndoUnit
	{
		UInt32 _ctrlKey;
		Size _size;
		IUndoHost _host;
		public SizeUndo(IUndoHost host, UInt32 objKey, Size size)
		{
			_host = host;
			_size = size;
			_ctrlKey = objKey;
		}
		public UInt32 Key
		{
			get
			{
				return _ctrlKey;
			}
		}
		#region IUndoUnit Members

		public void Apply()
		{
			bool b = _host.DisableUndo;
			_host.DisableUndo = true;
			Control c = _host.GetUndoControl(_ctrlKey);
			if (c != null)
			{
				c.Size = _size;
				if (c.Parent != null)
				{
					ActiveDrawing.RefreshControl(c.Parent);
				}
			}
			_host.DisableUndo = b;
		}

		#endregion
	}

}
