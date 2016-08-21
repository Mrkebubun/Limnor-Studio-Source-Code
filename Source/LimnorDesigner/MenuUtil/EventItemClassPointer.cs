/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using LimnorDesigner.Event;
using VSPrj;
using System.Xml;
using ProgElements;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	public class EventItemClassPointer : MenuItemDataEvent
	{
		#region constructors and fields
		private EventClass _event;
		public EventItemClassPointer(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
			if (owner.VariableCustomType == null)
			{
				throw new DesignerException("Cannot create EventItemClassPointer because VariableCustomType is null");
			}
		}
		public EventItemClassPointer(string key, IClass owner, EventClass ev)
			: base(key, owner)
		{
			_event = ev;
			if (owner.VariableCustomType == null)
			{
				throw new DesignerException("Cannot create EventItemClassPointer because VariableCustomType is null.");
			}
		}
		#endregion
		public override IEvent CreateEventPointer(IClass holder)
		{
			return new CustomEventPointer(_event, holder);
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			CustomEventPointer ep = new CustomEventPointer(_event, holder);
			return pane.AssignActions(ep, pane.FindForm());
		}
		public ClassPointer Pointer
		{
			get
			{
				return Owner.VariableCustomType;
			}
		}
		public EventClass Event
		{
			get
			{
				return _event;
			}
		}
		public override string Tooltips
		{
			get { return _event.Description; }
		}
	}
}
