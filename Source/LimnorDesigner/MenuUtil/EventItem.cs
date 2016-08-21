/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Parser;
using VPL;
using System.Drawing;
using VSPrj;
using System.Xml;
using System.Windows.Forms;
using ProgElements;
using LimnorDesigner.Event;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// for lib-type
	/// </summary>
	public class EventItem : MenuItemDataEvent
	{
		private Type _type;
		private EventInfo _val;
		private string _tooltips;
		public EventItem(string key, IClass owner, EventInfo ei)
			: base(key, owner)
		{
			if (owner.VariableLibType != null)
			{
				_type = owner.VariableLibType;
			}
			else if (owner.VariableCustomType != null)
			{
				_type = owner.VariableCustomType.BaseClassType;
			}
			else if (owner.VariableWrapperType != null)
			{
				_type = owner.VariableWrapperType.WrappedType;
			}
			else
			{
				throw new DesignerException("Invalid object type for EventItem");
			}
			_val = ei;
		}
		public override IEvent CreateEventPointer(IClass holder)
		{
			EventPointer ep = new EventPointer();
			ep.SetEventInfo(_val);
			ep.Owner = holder;
			return ep;
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			EventPointer ep = new EventPointer();
			ep.SetEventInfo(_val);
			ep.Owner = Owner;
			return pane.AssignActions(ep, pane.FindForm());
		}
		public Type ObjectType
		{
			get
			{
				return _type;
			}
		}
		public EventInfo Value
		{
			get
			{
				return _val;
			}
		}
		public override string Tooltips
		{
			get
			{
				if (string.IsNullOrEmpty(_tooltips))
				{
					string desc;
					Dictionary<string, string> paramsDesc = PMEXmlParser.GetEventDescription(VPLUtil.GetObjectType(ObjectType), _val, out desc);
					if (paramsDesc.Count > 0)
					{
						StringBuilder sb = new StringBuilder();
						if (paramsDesc != null)
						{
							bool bFirst = true;
							foreach (KeyValuePair<string, string> kv in paramsDesc)
							{
								if (bFirst)
								{
									bFirst = false;
								}
								else
								{
									sb.Append(",");
								}
								sb.Append(kv.Key);
								sb.Append(":");
								sb.Append(kv.Value);
							}
						}
						_tooltips = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}({1})", desc, sb.ToString());
					}
					else
					{
						_tooltips = desc;
					}
				}
				return _tooltips;
			}
		}
	}
}
