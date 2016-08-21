/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using LimnorDesigner.Action;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using XmlUtility;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// it is used for finally clause
	/// </summary>
	[UseParentObject]
	public class SubscopeActions : IXmlNodeHolder, IActionsHolder
	{
		#region fields anf constructors
		private MethodClass _method;
		private BranchList _actions;
		private XmlNode _node;
		private List<ComponentIconSubscopeVariable> _componentIconList;
		private Dictionary<UInt32, IAction> _acts;
		public SubscopeActions(MethodClass m)
		{
			_method = m;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public BranchList Actions
		{
			get
			{
				if (_actions == null)
				{
					_actions = new BranchList(this);
				}
				return _actions;
			}
			set
			{
				_actions = value;
				if (_actions != null)
				{
					_actions.SetOwnerMethod(_method);
				}
			}
		}
		[Browsable(false)]
		public List<ComponentIconSubscopeVariable> ComponentIconList
		{
			get
			{
				if (_componentIconList == null)
					_componentIconList = new List<ComponentIconSubscopeVariable>();
				return _componentIconList;
			}
			set
			{
				_componentIconList = value;
			}
		}
		#endregion
		#region Methods
		public void EstablishObjectOwnership(MethodClass m)
		{
			if (_componentIconList != null)
			{
				foreach (ComponentIconSubscopeVariable sv in _componentIconList)
				{
					sv.EstablishObjectOwnership(_method);
				}
			}
			if (_actions != null)
			{
				_actions.EstablishObjectOwnership();
			}
			if (_acts != null)
			{
				foreach (IAction act in _acts.Values)
				{
					act.EstablishObjectOwnership(this);
				}
			}
		}
		public void SetOwnerMethod(MethodClass m)
		{
			_method = m;
			Actions.SetOwnerMethod(m);
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Actions:{0}", Actions.Count);
		}
		#endregion

		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _node;
			}
			set
			{
				_node = value;
			}
		}

		#endregion

		#region IActionsHolder Members
		[Browsable(false)]
		public XmlNode ActionsNode
		{
			get
			{
				if (DataXmlNode != null)
				{
					XmlNode node = this.DataXmlNode.SelectSingleNode(XmlTags.XML_ACTIONS);
					if (node == null)
					{
						node = this.DataXmlNode.OwnerDocument.CreateElement(XmlTags.XML_ACTIONS);
						this.DataXmlNode.AppendChild(node);
					}
					return node;
				}
				return null;
			}
		}
		[Browsable(false)]
		public UInt32 SubScopeId
		{
			get
			{
				return 1;
			}
		}

		[Browsable(false)]
		public uint ScopeId
		{
			get { return _method.MemberId; }
		}

		[Browsable(false)]
		public MethodClass OwnerMethod
		{
			get
			{
				return _method;
			}
		}
		[Browsable(false)]
		public Dictionary<uint, IAction> ActionInstances
		{
			get
			{
				return _acts;
			}
		}
		public Dictionary<UInt32, IAction> GetVisibleActionInstances()
		{
			Dictionary<UInt32, IAction> lst = new Dictionary<uint, IAction>();
			if (_acts == null)
			{
				LoadActionInstances();
			}
			if (_acts != null)
			{
				foreach (KeyValuePair<UInt32, IAction> kv in _acts)
				{
					if (!lst.ContainsKey(kv.Key))
					{
						lst.Add(kv.Key, kv.Value);
					}
				}
			}
			Dictionary<uint, IAction> acts = OwnerMethod.RootPointer.ActionInstances;
			if (acts == null)
			{
				OwnerMethod.RootPointer.LoadActionInstances();
			}
			if (acts != null)
			{
				foreach (KeyValuePair<UInt32, IAction> kv in acts)
				{
					if (!lst.ContainsKey(kv.Key))
					{
						lst.Add(kv.Key, kv.Value);
					}
				}
			}
			return lst;
		}
		public void SetActionInstances(Dictionary<uint, IAction> actions)
		{
			_acts = actions;
		}
		public void LoadActionInstances()
		{
			this._method.RootPointer.LoadActions(this);
			if (_actions != null)
			{
				_actions.SetActions(_acts);

				_actions.SetActions(_method.ActionInstances);
				_actions.SetActions(_method.RootPointer.ActionInstances);
			}
		}
		[Browsable(false)]
		public IAction GetActionInstance(UInt32 actId)
		{
			return ClassPointer.GetActionObject(actId, this, this.OwnerMethod.RootPointer);
		}
		public ActionBranch FindActionBranchById(UInt32 branchId)
		{
			if (_actions != null)
			{
				return _actions.SearchBranchById(branchId);
			}
			return null;
		}
		public void GetActionNames(StringCollection sc)
		{
			if (_actions != null)
			{
				_actions.GetActionNames(sc);
			}
			if (_acts == null)
			{
				LoadActionInstances();
			}
			if (_acts != null)
			{
				foreach (IAction a in _acts.Values)
				{
					if (a != null)
					{
						if (!string.IsNullOrEmpty(a.ActionName))
						{
							if (!sc.Contains(a.ActionName))
							{
								sc.Add(a.ActionName);
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public void AddActionInstance(IAction action)
		{
			if (_acts == null)
			{
				//LoadActionInstances();
				_acts = new Dictionary<uint, IAction>();
			}
			if (_acts != null)
			{
				bool found = _acts.ContainsKey(action.ActionId);
				if (!found)
				{
					_acts.Add(action.ActionId, action);
				}
			}
		}
		[Browsable(false)]
		public IAction TryGetActionInstance(UInt32 actId)
		{
			if (_acts != null)
			{
				IAction a;
				if (_acts.TryGetValue(actId, out a))
				{
					return a;
				}
			}
			return null;
		}
		[Browsable(false)]
		public void DeleteActions(List<UInt32> actionIds)
		{

			if (_acts != null)
			{
				foreach (UInt32 id in actionIds)
				{
					IAction a;
					if (_acts.TryGetValue(id, out a))
					{
						if (a != null)
						{
							ClassPointer.DeleteAction(_node, id);
						}
						_acts.Remove(id);
					}
				}
			}
		}
		#endregion
	}
}
