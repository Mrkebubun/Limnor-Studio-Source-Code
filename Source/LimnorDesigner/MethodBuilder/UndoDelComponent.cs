/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// for redo deleting component
	/// </summary>
	class UndoDelComponent : IUndoUnit
	{
		IUndoHost _host;
		List<UInt32> _idList;
		IDesignerHost _designer;
		public UndoDelComponent(IUndoHost host, List<UInt32> idList, IDesignerHost designer)
		{
			_host = host;
			_idList = idList;
			_designer = designer;
		}

		#region IUndoUnit Members

		public void Apply()
		{
			bool b = _host.DisableUndo;
			_host.DisableUndo = true;
			List<IComponent> icList = new List<IComponent>();
			foreach (IComponent c in _designer.Container.Components)
			{
				IActiveDrawing a = c as IActiveDrawing;
				if (a != null)
				{
					if (_idList.Contains(a.ActiveDrawingID))
					{
						icList.Add(c);
					}
				}
			}
			if (icList.Count > 0)
			{
				foreach (IComponent ic in icList)
				{
					_designer.DestroyComponent(ic);
				}
			}
			_host.DisableUndo = b;
		}

		#endregion
	}
}
