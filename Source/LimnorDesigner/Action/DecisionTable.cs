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
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Drawing;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public class DecisionItem : ICloneable
	{
		public DecisionItem()
		{
		}
		public DecisionItem(MathNodeRoot r, ActionList a)
		{
			Condition = r;
			Actions = a;
		}
		public MathNodeRoot Condition { get; set; }
		public ActionList Actions { get; set; }
		public void GetActionNames(StringCollection sc)
		{
			if (Actions != null && Actions.Count > 0)
			{
				foreach (ActionItem ai in Actions)
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
		}
		public void EstablishObjectOwnership(IActionsHolder scope)
		{
			if (Actions != null && Actions.Count > 0)
			{
				foreach (ActionItem ai in Actions)
				{
					if (ai.Action != null)
					{
						ai.Action.EstablishObjectOwnership(scope);
					}
				}
			}
		}
		public void GetCustomMethods(List<MethodClass> list)
		{
			if (Actions != null && Actions.Count > 0)
			{
				foreach (ActionItem ai in Actions)
				{
					if (ai.Action != null)
					{
						MethodClass mc = ai.Action.ActionMethod.MethodPointed as MethodClass;
						if (mc != null)
						{
							bool found = false;
							foreach (MethodClass m in list)
							{
								if (m.MethodID == mc.MethodID)
								{
									found = true;
									break;
								}
							}
							if (!found)
							{
								list.Add(mc);
							}
						}
					}
				}
			}
		}
		public void CollectSourceValues(UInt32 taskId, MethodClass m)
		{
			if (Actions != null && Actions.Count > 0)
			{
				if (Condition != null)
				{
					IList<ISourceValuePointer> list = Condition.GetValueSources();
					if (list != null && list.Count > 0)
					{
						foreach (ISourceValuePointer p in list)
						{
							if (taskId != 0)
							{
								p.SetTaskId(taskId);
							}
							if (p.IsWebClientValue())
							{
								m.AddUpload(p);
							}
							else
							{
								m.AddDownload(p);
							}
						}
					}
				}
				foreach (ActionItem ai in Actions)
				{
					if (ai.Action != null)
					{
						m.AddUploads(ai.Action.GetClientProperties(taskId));
						m.AddDownloads(ai.Action.GetServerProperties(taskId));
					}
				}
			}
		}
		public bool UseClientServerValues(bool client)
		{
			if (Actions != null && Actions.Count > 0)
			{
				if (Condition != null)
				{
					IList<ISourceValuePointer> list = Condition.GetValueSources();
					if (list != null && list.Count > 0)
					{
						foreach (ISourceValuePointer p in list)
						{
							if (p.IsWebClientValue())
							{
								if (client)
								{
									return true;
								}
							}
							else
							{
								if (!client)
								{
									return true;
								}
							}
						}
					}
				}
				foreach (ActionItem ai in Actions)
				{
					if (ai.Action != null)
					{
						if (client)
						{
							IList<ISourceValuePointer> l = ai.Action.GetClientProperties(0);
							if (l != null && l.Count > 0)
							{
								return true;
							}
						}
						else
						{
							IList<ISourceValuePointer> l = ai.Action.GetServerProperties(0);
							if (l != null && l.Count > 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		#region ICloneable Members

		public object Clone()
		{
			return new DecisionItem((MathNodeRoot)Condition.Clone(), (ActionList)Actions.Clone());
		}

		#endregion
	}
	public class DecisionTable : ICloneable, IXmlNodeSerializable
	{
		private List<DecisionItem> _decisionTable;
		private int _conditionWidth = 80;
		private int _actionWidth = 80;
		private Size _editorSize;
		public DecisionTable()
		{
			_editorSize = new Size(292, 266);
		}
		public void GetActionNames(StringCollection sc)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					di.GetActionNames(sc);
				}
			}
		}
		public void EstablishObjectOwnership(IActionsHolder scope)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					di.EstablishObjectOwnership(scope);
				}
			}
		}
		public void Add(MathNodeRoot condition, ActionList act)
		{
			if (_decisionTable == null)
				_decisionTable = new List<DecisionItem>();
			_decisionTable.Add(new DecisionItem(condition, act));
		}
		public void SetScopeMethod(MethodClass m)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					if (di.Condition != null)
					{
						di.Condition.ScopeMethod = m;
					}
				}
			}
		}
		public void CollectSourceValues(UInt32 taskId, MethodClass m)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					di.CollectSourceValues(taskId, m);
				}
			}
		}
		public bool UseClientServerValues(bool client)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					if (di.UseClientServerValues(client))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void GetCustomMethods(List<MethodClass> list)
		{
			if (_decisionTable != null)
			{
				foreach (DecisionItem di in _decisionTable)
				{
					di.GetCustomMethods(list);
				}
			}
		}
		public DecisionItem this[int index]
		{
			get
			{
				return _decisionTable[index];
			}
		}
		public Size ControlSize
		{
			get
			{
				return _editorSize;
			}
			set
			{
				_editorSize = value;
			}
		}
		public int ConditionColumnWidth
		{
			get
			{
				return _conditionWidth;
			}
			set
			{
				_conditionWidth = value;
			}
		}
		public int ActionColumnWidth
		{
			get
			{
				return _actionWidth;
			}
			set
			{
				_actionWidth = value;
			}
		}
		public int ConditionCount
		{
			get
			{
				if (_decisionTable == null)
					return 0;
				return _decisionTable.Count;
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			DecisionTable dt = new DecisionTable();
			dt._conditionWidth = _conditionWidth;
			if (_decisionTable != null)
			{
				List<DecisionItem> tbl = new List<DecisionItem>();
				for (int i = 0; i < _decisionTable.Count; i++)
				{
					tbl.Add((DecisionItem)_decisionTable[i].Clone());
				}
				dt._decisionTable = tbl;
			}
			return dt;
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_colWidth = "colWidth";
		const string XMLATT_col2Width = "col2Width";
		const string XML_Condition = "Condition";
		const string XMLATT_width = "width";
		const string XMLATT_height = "height";
		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_colWidth, _conditionWidth);
			XmlUtil.SetAttribute(node, XMLATT_col2Width, _actionWidth);
			XmlUtil.SetAttribute(node, XMLATT_width, _editorSize.Width);
			XmlUtil.SetAttribute(node, XMLATT_height, _editorSize.Height);
			if (_decisionTable != null)
			{
				for (int i = 0; i < _decisionTable.Count; i++)
				{
					XmlNode item = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					node.AppendChild(item);
					XmlNode nodeCOndition = XmlUtil.CreateSingleNewElement(item, XML_Condition);
					writer.WriteValue(nodeCOndition, _decisionTable[i].Condition, null);
					if (_decisionTable[i].Actions != null)
					{
						XmlNode nodeActions = XmlUtil.CreateSingleNewElement(item, XmlTags.XML_ACTIONS);
						_decisionTable[i].Actions.OnWriteToXmlNode((XmlObjectWriter)writer, nodeActions);
					}
				}
			}
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)reader0;
			_conditionWidth = XmlUtil.GetAttributeInt(node, XMLATT_colWidth);
			_actionWidth = XmlUtil.GetAttributeInt(node, XMLATT_col2Width);
			int w = XmlUtil.GetAttributeInt(node, XMLATT_width);
			if (w > 100)
			{
				_editorSize.Width = w;
			}
			w = XmlUtil.GetAttributeInt(node, XMLATT_height);
			if (w > 100)
			{
				_editorSize.Height = w;
			}
			_decisionTable = new List<DecisionItem>();
			XmlNodeList itemsNodes = node.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode item in itemsNodes)
			{
				XmlNode nodeCOndition = item.SelectSingleNode(XML_Condition);
				if (nodeCOndition != null)
				{
					ActionList acts = new ActionList();
					MathNodeRoot r = (MathNodeRoot)reader.ReadValue(nodeCOndition, this);
					XmlNode nodeActions = item.SelectSingleNode(XmlTags.XML_ACTIONS);
					if (nodeActions != null)
					{
						acts.OnReadFromXmlNode((XmlObjectReader)reader, nodeActions);
					}
					_decisionTable.Add(new DecisionItem(r, acts));
				}
				else
				{
					reader.addErrStr2("Decision table missing condition node");
				}
			}

		}

		#endregion
	}
}
