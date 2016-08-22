/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using VPL;
using System.Xml.Serialization;

namespace Limnor.TreeViewExt
{
	/// <summary>
	/// a pointer to a category node TreeNodeX
	/// it represents a sub-node under a shortcut
	/// </summary>
	public class TreeNodePointer : TreeNodeX
	{
		#region fields and constructors
		private TreeNodeX _dataNode;
		public TreeNodePointer()
		{
		}
		public TreeNodePointer(Guid id)
		{
			TreeNodeId = id;
		}
		public TreeNodePointer(XmlNode node)
			: base(node)
		{
		}

		#endregion

		#region Methods
		[Description("Load this node from an XmlNode")]
		public void LoadFromDataXmlNode(XmlNode node)
		{
			ObjectXmlReader rd = new ObjectXmlReader();
			rd.ReadProperties(node, this);
		}
		[Browsable(false)]
		public void SetReferenceNode(TreeNodeX node)
		{
			_dataNode = node;
		}
		[Browsable(false)]
		public override void SyncShortcuts(TreeNodeX node)
		{
			if (node.TreeNodeId == this.TreeNodeId)
			{
				_dataNode = node;
				VPLUtil.CopyProperties(node, this);
			}
			else
			{
				base.SyncShortcuts(node);
			}
		}
		[Browsable(false)]
		public override void SyncShortcutsValueList(TreeNodeX node)
		{
			if (node.TreeNodeId == this.TreeNodeId)
			{
				SetValueList(node.GetValueList());
				_dataNode = node;
				ResetNextLevel();
			}
			else
			{
				base.SyncShortcuts(node);
			}
		}
		[Browsable(false)]
		public override void LoadNextLevelNodes()
		{
			if (_dataNode != null && _dataNode.NextLevelLoaded)
			{
				Nodes.Clear();
				for (int i = 0; i < _dataNode.Nodes.Count; i++)
				{
					INodeX nx = _dataNode.Nodes[i] as INodeX;
					if (nx != null)
					{
						TreeNode tn = nx.CreatePointer();
						Nodes.Add(tn);
					}
				}
			}
			else
			{
				base.LoadNextLevelNodes();
			}
		}

		[Browsable(false)]
		public override void OnCategoryNodeLoaded(TreeNodeX node)
		{
			if (this.TreeNodeId == node.TreeNodeId)
			{
				_dataNode = node;
			}
			else
			{
				base.OnCategoryNodeLoaded(node);
			}
		}

		[Browsable(false)]
		public override void OnValueListLoaded(TreeNodeX node)
		{
			if (this.TreeNodeId == node.TreeNodeId)
			{
				this.SetValueList(node.GetValueList());
				if (this.NextLevelLoaded)
				{
					this.ResetNextLevel();
				}
			}
			else
			{
				base.OnValueListLoaded(node);
			}
		}
		[Browsable(false)]
		public override void OnSubNodeCreated(TreeNodeX parentNode, TreeNodeX subNode)
		{
			if (this.TreeNodeId == parentNode.TreeNodeId)
			{
				TreeNode tnx = subNode.CreatePointer();
				this.Nodes.Add(tnx);
			}
			else
			{
				base.OnSubNodeCreated(parentNode, subNode);
			}
		}
		[Browsable(false)]
		public override bool SetReferencedNode(TreeNodePointer node)
		{
			if (this.TreeNodeId == node.TreeNodeId)
			{
				if (this._dataNode != null)
				{
					node.SetReferenceNode(_dataNode);
					return true;
				}
			}
			else
			{
				if (NextLevelLoaded)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeX tnx = Nodes[i] as TreeNodeX;
						if (tnx != null)
						{
							if (tnx.SetReferencedNode(node))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public override TreeNodeX CreateDuplicatedNode(TreeViewX targetHolder)
		{
			throw new TreeViewXException("Cannot duplicate a pointer node");
		}
		#endregion

		#region Properties
		[ReadOnly(true)]
		[Browsable(false)]
		[XmlIgnore]
		public override bool IsShortcut
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public TreeNodeX ReferencedNode
		{
			get
			{
				return _dataNode;
			}
		}
		#endregion
	}
	public class TreeNodeShortcut : TreeNodePointer
	{
		#region fields and constructors
		private XmlNode _pointerXmlNode;
		public TreeNodeShortcut(Guid id)
			: base(id)
		{
		}
		public TreeNodeShortcut(XmlNode node)
			: this(node, new ObjectXmlReader())
		{
		}
		public TreeNodeShortcut(XmlNode node, ObjectXmlReader reader)
		{
			LoadShortCutFromXmlNode(node, reader);
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public void SetPointerXml(XmlNode node)
		{
			_pointerXmlNode = node;
		}
		[Browsable(false)]
		public void LoadShortCutFromXmlNode(XmlNode node, ObjectXmlReader reader)
		{
			_pointerXmlNode = node;
			Guid id = XmlUtil.GetAttributeGuid(node, TreeViewX.XMLATT_Guid);
			if (id == Guid.Empty)
			{
				throw new TreeViewXException("Guid not found in the shortcut node. Shortcut Path:{0}", XmlUtil.GetPath(node));
			}
			XmlNode dataNode = node.OwnerDocument.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}[@{1}='{2}']",
				TreeViewX.XML_Item, TreeViewX.XMLATT_Guid, id.ToString("D")));
			if (dataNode == null)
			{
				throw new TreeViewXException("XmlNode not found for {0}. Shortcut Path:{1}", id, XmlUtil.GetPath(node));
			}
			DataXmlNode = dataNode;
			reader.ReadProperties(dataNode, this);
			ClearValues();
		}
		[Browsable(false)]
		public override WriteResult WriteToXmlNode(XmlNode parentXmlNode, IXmlCodeWriter writer)
		{
			_pointerXmlNode = parentXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				TreeViewX.XML_Shortcut, TreeViewX.XMLATT_Guid, this.TreeNodeId.ToString("D")));
			if (_pointerXmlNode == null)
			{
				_pointerXmlNode = parentXmlNode.OwnerDocument.CreateElement(TreeViewX.XML_Shortcut);
				parentXmlNode.AppendChild(_pointerXmlNode);
			}
			XmlUtil.SetAttribute(_pointerXmlNode, TreeViewX.XMLATT_Guid, this.TreeNodeId.ToString("D"));
			return WriteResult.WriteOK;
		}
		/// <summary>
		/// duplicate the shortcut
		/// </summary>
		/// <param name="targetHolder"></param>
		/// <returns></returns>
		[Browsable(false)]
		public override TreeNodeX CreateDuplicatedNode(TreeViewX targetHolder)
		{
			XmlDocument docTarget = targetHolder.GetXmlDocument();
			if (docTarget != _pointerXmlNode.OwnerDocument)
			{
				throw new TreeViewXException("Cannot duplicate shortcut bewteen different tree views");
			}
			ObjectXmlReader oxr = new ObjectXmlReader();
			XmlNode pointerXml = _pointerXmlNode.Clone();
			TreeNodeShortcut tns = new TreeNodeShortcut(pointerXml, oxr);
			return tns;
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public XmlNode PointerXmlNode
		{
			get
			{
				return _pointerXmlNode;
			}
		}
		#endregion
	}
}
