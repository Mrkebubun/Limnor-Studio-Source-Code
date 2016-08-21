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
using System.Xml;
using XmlUtility;
using XmlSerializer;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public class ActionItem : ICloneable, IXmlNodeSerializable
	{
		private UInt32 _actionId; //pointer to the action
		public ActionItem()
		{
		}
		public ActionItem(UInt32 actionId)
		{
			_actionId = actionId;
		}
		public ActionItem(IAction act)
		{
			if (act != null)
			{
				Action = act;
				_actionId = act.ActionId;
			}
		}
		public UInt32 ActionId
		{
			get
			{
				if (_actionId == 0 && Action != null)
				{
					_actionId = Action.ActionId;
				}
				return _actionId;
			}
			set
			{
				_actionId = value;
			}
		}
		public IAction Action { get; set; }
		public override string ToString()
		{
			if (Action != null)
			{
				return Action.ActionName;
			}
			return "Action?";
		}
		public ActionItem CreateNewCopy()
		{
			if (Action != null)
			{
				IAction _act = Action.CreateNewCopy();
				ActionItem ai = new ActionItem(_act.ActionId);
				ai.Action = _act;
				return ai;
			}
			return (ActionItem)Clone();
		}
		#region ICloneable Members

		public object Clone()
		{
			ActionItem obj = new ActionItem(Action);
			if (_actionId != 0)
			{
				obj.ActionId = _actionId;
			}
			return obj;
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ActionID, ActionId);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			_actionId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ActionID);
		}

		#endregion

	}
	public class ActionList : List<ActionItem>, ICloneable, IXmlNodeSerializable
	{
		public ActionList()
		{
		}
		public string Name { get; set; }
		public override string ToString()
		{
			return string.Format("{0} (actions:{1})", Name, Count);
		}
		public void GetActionNames(StringCollection sc)
		{
			foreach (ActionItem ai in this)
			{
				if (ai.Action != null)
				{
					if (!string.IsNullOrEmpty(ai.Action.ActionName))
					{
						if (!sc.Contains(ai.Action.ActionName))
						{
							sc.Add(ai.Action.ActionName);
						}
					}
				}
			}
		}
		public void EstablishObjectOwnership(IActionsHolder scope)
		{
			foreach (ActionItem ai in this)
			{
				if (ai.Action != null)
				{
					ai.Action.EstablishObjectOwnership(scope);
				}
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			ActionList obj = new ActionList();
			obj.Name = Name;
			foreach (ActionItem a in this)
			{
				obj.Add((ActionItem)a.Clone());
			}
			return obj;
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, Name);
			foreach (ActionItem a in this)
			{
				XmlNode actNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
				node.AppendChild(actNode);
				XmlUtil.SetAttribute(actNode, XmlTags.XMLATT_ActionID, a.ActionId);
			}
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			Name = XmlUtil.GetNameAttribute(node);
			XmlNodeList list = node.SelectNodes(XmlTags.XML_Item);
			if (list != null && list.Count > 0)
			{
				foreach (XmlNode actNode in list)
				{
					ActionItem ai = new ActionItem(XmlUtil.GetAttributeUInt(actNode, XmlTags.XMLATT_ActionID));
					Add(ai);
				}
			}
		}

		#endregion
	}
}
