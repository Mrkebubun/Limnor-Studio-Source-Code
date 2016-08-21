/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using LimnorDesigner.Event;

namespace LimnorDesigner.MethodBuilder
{
	class AssignHandler : NoAction
	{
		private IEvent _event;
		public AssignHandler(IEvent e)
		{
			_event = e;
		}
		public IEvent Event
		{
			get
			{
				return _event;
			}
		}
	}
}
