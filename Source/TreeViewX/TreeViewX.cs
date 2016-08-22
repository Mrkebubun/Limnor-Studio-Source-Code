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
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using XmlUtility;
using LimnorDatabase;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Data.OleDb;
using System.Reflection;
using System.Runtime.InteropServices;
using VPL;
using System.Collections.Specialized;
using System.CodeDom;
using System.Data;

namespace Limnor.TreeViewExt
{
	[ComVisible(true)]
	[Description("Displays a hierarchical collection of labeled items to the user that optionally contain an image and data. It can be saved to/load from XML file. It supports creating shortcuts to nodes.")]
	[DefaultMember("Item")]
	[Docking(DockingBehavior.Ask)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[DefaultProperty("Nodes")]
	[DefaultEvent("AfterSelect")]
	[ToolboxBitmapAttribute(typeof(TreeViewX), "Resources.tv.bmp")]
	public class TreeViewX : TreeView, IDatabaseConnectionUserExt0, IDevClassReferencer, IPropertyValueLinkOwner
	{
		#region fields and constructors
		const string MENU_NAME_ADDROOT = "AddRootNode";
		const string MENU_NAME_ADDSUB = "TreeViewXAddSubNode";
		const string MENU_NAME_MOVETOROOT = "TreeViewXMoveToNode";
		const string MENU_NAME_MOVEUP = "TreeViewXMoveUp";
		const string MENU_NAME_MOVEDOWN = "TreeViewXMoveDown";

		const string MENU_NAME_ADDVALUE = "TreeViewXAddValue";
		const string MENU_NAME_REFRESH = "TreeViewXRefresh";
		const string MENU_NAME_PROPERTIES = "TreeViewXProperties";

		const string MENU_NAME_COPY = "TreeViewXCopy";
		const string MENU_NAME_PASTE = "TreeViewXPaste";

		const string MENU_NAME_REMOVENODE = "TreeViewXRemoveNode";
		const string MENU_NAME_REMOVEVALUE = "TreeViewXRemoveValue";
		const string MENU_NAME_REMOVESHORTCUT = "TreeViewXRemoveShortcut";
		//
		const string CLIPDATA_TVXGUID = "TreeViewXGuid";
		const string CLIPDATA_NODE = "TreeNodeX";
		const string CLIPDATA_SHORTCUT = "TreeNodeShortcut";
		const string CLIPDATA_TYPE = "TreeNodeType";
		//
		const string C_RootNodesSqlStatement = "RootNodesSqlStatement";
		public const string XML_Item = "Item";
		public const string XML_Value = "Value";
		public const string XML_Shortcut = "Shortcut";
		public const string XML_Templates = "Templates";
		public const string XML_PROPERTY = "Property";
		public const string XMLATT_Guid = "guid";
		public const string XMLATT_Name = "name";
		private Guid _guid = Guid.Empty;
		private string _error;
		private bool _readOnly;
		private XmlNode _xmlData;
		private EnumTreeViewMenu _menuItems;
		//drag drop
		private bool _mouseDown;
		private TreeNodeX _ndDrop; //drop target
		private int x0 = 0, y0 = 0;
		private int xd = 0, yd = 0;
		private int nDeltaDragMove = 3;
		//
		private DataQuery[] _dataQueries;
		//
		public TreeViewX()
		{
			ShowPropertyNodes = true;
			AllowDrop = true;
			HideSelection = false;
			_menuItems = (EnumTreeViewMenu)8191; //4096 *2 - 1
		}
		[Browsable(false)]
		public static void Init()
		{
		}
		//
		[Browsable(false)]
		public static void AddKnownTypes()
		{
			XmlUtil.AddKnownType("TreeNodeXTemplate", typeof(TreeNodeXTemplate));
			XmlUtil.AddKnownType("TreeViewX", typeof(TreeViewX));
			XmlUtil.AddKnownType("TreeNodeX", typeof(TreeNodeX));
			XmlUtil.AddKnownType("TreeNodeValue", typeof(TreeNodeValue));
			XmlUtil.AddKnownType("TreeNodeShortcut", typeof(TreeNodeShortcut));
			XmlUtil.AddKnownType("TreeNodeXEventArgs", typeof(TreeNodeXEventArgs));
			XmlUtil.AddKnownType("TreeNodeShortcutEventArgs", typeof(TreeNodeShortcutEventArgs));
			XmlUtil.AddKnownType("TreeNodeValueEventArgs", typeof(TreeNodeValueEventArgs));
			XmlUtil.AddKnownType("TreeNodePropertyMouseClickEventArgs", typeof(TreeNodePropertyMouseClickEventArgs));
			XmlUtil.AddKnownType("TreeNodeXMouseClickEventArgs", typeof(TreeNodeXMouseClickEventArgs));
			XmlUtil.AddKnownType("TreeNodeShortcutMouseClickEventArgs", typeof(TreeNodeShortcutMouseClickEventArgs));
			//
			XmlUtil.AddKnownType("EventHandlerTreeNodeX", typeof(EventHandlerTreeNodeX));
			XmlUtil.AddKnownType("EventHandlerTreeNodeShortcut", typeof(EventHandlerTreeNodeShortcut));
			XmlUtil.AddKnownType("EventHandlerTreeNodeValue", typeof(EventHandlerTreeNodeValue));
			XmlUtil.AddKnownType("MouseEventHandlerTreeNodeShortcut", typeof(MouseEventHandlerTreeNodeShortcut));
			XmlUtil.AddKnownType("MouseEventHandlerTreeNodeX", typeof(MouseEventHandlerTreeNodeX));
			XmlUtil.AddKnownType("MouseEventHandlerTreeNodeValue", typeof(MouseEventHandlerTreeNodeValue));
		}
		static TreeViewX()
		{
			AddKnownTypes();
		}
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewX));
			this.SuspendLayout();
			// 
			// TreeViewX
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.BackgroundImage = null;
			this.Font = null;
			this.ResumeLayout(false);

		}
		#endregion

		#region Events
		[Description("It occurs when a category node is selected")]
		public event EventHandlerTreeNodeX CategoryNodeSelected;
		[Description("It occurs when a shortcut node is selected")]
		public event EventHandlerTreeNodeShortcut ShortcutNodeSelected;
		[Description("It occurs when a value node is selected")]
		public event EventHandlerTreeNodeValue PropertyNodeSelected;
		[Description("Occurs when the user clicks a shortcut TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeShortcut ShortcutMouseClick;
		[Description("Occurs when the user double-clicks a shortcut TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeShortcut ShortcutMouseDoubleClick;
		[Description("Occurs when the user clicks a category TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeX CategoryNodeMouseClick;
		[Description("Occurs when the user double-clicks a category TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeX CategoryNodeMouseDoubleClick;
		[Description("Occurs when the user clicks a property TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeValue PropertyNodeMouseClick;
		[Description("Occurs when the user double-clicks a property TreeNode with the mouse.")]
		public event MouseEventHandlerTreeNodeValue PropertyNodeMouseDoubleClick;
		//
		[Description("Occurs when the mouse hovers over a shortcut TreeNode. ")]
		public event EventHandlerTreeNodeShortcut ShortcutMouseHover;
		[Description("Occurs when the mouse hovers over a category TreeNode. ")]
		public event EventHandlerTreeNodeX CategoryNodeMouseHover;
		[Description("Occurs when the mouse hovers over a property TreeNode. ")]
		public event EventHandlerTreeNodeValue PropertyNodeMouseHover;

		#endregion

		#region Properties
		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public int BaseLevel { get; set; }

		[NotForProgramming]
		[Browsable(false)]
		public DataQuery[] dataQueries
		{
			get
			{
				if (_dataQueries == null)
				{
					_dataQueries = new DataQuery[] { };
				}
				return _dataQueries;
			}
			set
			{
				_dataQueries = value;
				if (_dataQueries != null)
				{
					for (int i = 0; i < _dataQueries.Length; i++)
					{
						_dataQueries[i].SetOwner(this);
						_dataQueries[i].SetDevClass(_class);
					}
				}
			}
		}
		[ParenthesizePropertyName(true)]
		[Category("Database")]
		[ReadOnly(true)]
		[Description("Gets and sets the database query for loading the root nodes")]
		public DataQuery RootNodesQuery
		{
			get
			{
				DataQuery dq = null;
				if (_dataQueries != null && _dataQueries.Length > 0)
				{
					dq = _dataQueries[0];
				}
				else
				{
					_dataQueries = new DataQuery[] { dq };
				}
				if (dq == null)
				{
					dq = new DataQuery();
					dq.SetOwner(this);
					dq.SetDevClass(_class);
					_dataQueries[0] = dq;
				}
				return dq;
			}
			set
			{
				if (_dataQueries == null || _dataQueries.Length == 0)
				{
					if (value != null)
					{
						_dataQueries = new DataQuery[] { value };
					}
				}
				else
				{
					_dataQueries[0] = value;
				}
				if (_dataQueries != null && _dataQueries.Length > 0)
				{
					if (_dataQueries[0] != null)
					{
						_dataQueries[0].SetOwner(this);
						_dataQueries[0].SetDevClass(_class);
					}
				}
			}
		}
		private TreeNodeXTemplate _templates;
		[Category("Database")]
		[ParenthesizePropertyName(true)]
		[Description("Gets and sets the templates for loading the root nodes via data-binding")]
		public TreeNodeXTemplate RootNodesTemplate
		{
			get
			{
				if (_templates == null)
				{
					_templates = new TreeNodeXTemplate();
					_templates.SetOwner(this);
				}
				return _templates;
			}
			set
			{
				_templates = value;
				if (_templates != null)
				{
					_templates.SetOwner(this);
				}
			}
		}
		[Description("Gets a value indicating whether a category node is selected")]
		public bool IsCategoryNodeSelected
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeX tnx = SelectedNode as TreeNodeX;
					if (tnx != null)
					{
						if (!tnx.IsShortcut)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		[Description("Gets a value indicating whether a shortcut node is selected")]
		public bool IsShortcutNodeSelected
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeShortcut tnx = SelectedNode as TreeNodeShortcut;
					if (tnx != null)
					{

						return true;

					}
				}
				return false;
			}
		}
		[Description("Gets a value indicating whether a property node is selected")]
		public bool IsPropertyNodeSelected
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeValue tnx = SelectedNode as TreeNodeValue;
					if (tnx != null)
					{

						return true;

					}
				}
				return false;
			}
		}
		[Description("Gets the selected shortcut node. Returns null if no shortcut is selected")]
		public TreeNodeShortcut SelectedShortcut
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeShortcut tnx = SelectedNode as TreeNodeShortcut;
					if (tnx != null)
					{

						return tnx;

					}
				}
				return null;
			}
		}
		[Description("Gets the selected category node. Returns null if no category node is selected")]
		public TreeNodeX SelectedCategryNode
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeX tnx = SelectedNode as TreeNodeX;
					if (tnx != null)
					{
						if (!tnx.IsShortcut)
						{
							return tnx;
						}
					}
				}
				return null;
			}
		}
		[Description("Gets the selected property node. Returns null if no property node is selected")]
		public TreeNodeValue SelectedPropertyNode
		{
			get
			{
				if (SelectedNode != null)
				{
					TreeNodeValue tnx = SelectedNode as TreeNodeValue;
					if (tnx != null)
					{

						return tnx;

					}
				}
				return null;
			}
		}
		[DefaultValue(true)]
		[Description("Gets and sets a value indicating whether the property nodes should be displayed")]
		public bool ShowPropertyNodes
		{
			get;
			set;
		}
		[Description("Gets a Guid identifying this control")]
		public Guid TreeViewGuid
		{
			get
			{
				return TreeViewId;
			}
		}
		[DefaultValue((EnumTreeViewMenu)8191)]
		[Description("Gets and sets a value indicating which menu items should be enabled")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public EnumTreeViewMenu EnabledMenuItems
		{
			get
			{
				return _menuItems;
			}
			set
			{
				_menuItems = value;
			}
		}
		public override ContextMenuStrip ContextMenuStrip
		{
			get
			{
				if ((Site != null && Site.DesignMode) || _readOnly)
				{
					return base.ContextMenuStrip;
				}
				else
				{
					ContextMenuStrip cms = base.ContextMenuStrip;
					if (cms == null)
					{
						cms = new ContextMenuStrip();
					}
					adjustContextMenu(cms);
					base.ContextMenuStrip = cms;
					return cms;
				}
			}
			set
			{
				if ((Site != null && Site.DesignMode) || _readOnly)
				{
					base.ContextMenuStrip = value;
				}
				else
				{
					ContextMenuStrip cms = value;
					if (cms == null)
					{
						cms = new ContextMenuStrip();
					}
					adjustContextMenu(cms);
					base.ContextMenuStrip = cms;
				}
			}
		}
		[DefaultValue(false)]
		[Description("Gets or sets a Boolean value indicating whether it allows the user to modify the TreeView")]
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
				if (_readOnly)
				{
					AllowDrop = false;
				}
			}
		}
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Localizable(true)]
		[MergableProperty(false)]
		[Description("Gets the root nodes in the TreeView control.")]
		[Category("Behavior")]
		[Editor(typeof(TreeViewXEditor), typeof(UITypeEditor))]
		public new TreeNodeCollection Nodes
		{
			get
			{
				return base.Nodes;
			}
		}
		[Description("Gets error message from SaveToFile/ReadFromFile actions")]
		public string ErrorMessage
		{
			get
			{
				return _error;
			}
		}
		#endregion

		#region Non-Browsable properties
		[Browsable(false)]
		public int Reserved
		{
			get;
			set;
		}
		[NotForLightRead]
		[Browsable(false)]
		public string XmlString
		{
			get
			{
				if (_xmlData != null)
				{
					return _xmlData.OwnerDocument.OuterXml;
				}
				return string.Empty;
			}
			set
			{
				Nodes.Clear();
				if (!string.IsNullOrEmpty(value))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(value);
					LoadFromXmlDocument(doc);
				}
			}
		}
		[Browsable(false)]
		public XmlNode RootXmlNode
		{
			get
			{
				if (_xmlData == null)
				{
					GetXmlDocument();
				}
				return _xmlData;
			}
		}
		[Browsable(false)]
		[Description("Guid identifying this control")]
		public Guid TreeViewId
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}
		#endregion

		#region Methods
		[Description("Add a root node. The new node is selected. Returns the new node.")]
		public virtual TreeNodeX AddRootNode(string text)
		{
			TreeNodeX tnx = new TreeNodeX(text);
			tnx.CreateID();
			if (tnx != null)
			{
				Nodes.Add(tnx);
				SelectedNode = tnx;
			}
			return tnx;
		}
		[Description("Add a sub node to the selected node. The new node is selected. Returns the new node. If there is not a selected node then this method does nothing and returns null.")]
		public virtual TreeNodeX AddSubNode(string text)
		{
			if (SelectedNode != null)
			{
				TreeNodeX p = SelectedNode as TreeNodeX;
				if (p != null && !p.IsShortcut)
				{
					p.Expand();
					p.LoadNextLevel();

					TreeNodeX tnx = new TreeNodeX(text);
					tnx.CreateID();
					p.Nodes.Add(tnx);
					SelectedNode = tnx;

					onSubNodeCreated(p, tnx);
					return tnx;
				}
			}
			return null;
		}
		[Description("Delete selected node. If there is not a selected node or the selected node is for value then this method does nothing. If parameter 'confirm' is true then a message box appears to ask the user to confirm the deletion.")]
		public void DeleteSelectedCategoryNode(bool confirm)
		{
			if (SelectedNode != null)
			{
				TreeNodeX tnx = SelectedNode as TreeNodeX;
				if (tnx != null && !tnx.IsShortcut)
				{
					bool bOK = !confirm;
					if (!bOK)
					{
						string txt = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"Do you want to delete node '{0}' and all its sub-nodes?", tnx.Text);
						if (MessageBox.Show(this.FindForm(), txt, "Delete node", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						{
							bOK = true;
						}
					}
					if (bOK)
					{
						if (tnx.DataXmlNode != null)
						{
							XmlNode xm = tnx.DataXmlNode.ParentNode;
							if (xm != null)
							{
								xm.RemoveChild(tnx.DataXmlNode);
							}
						}
						tnx.Remove();
						onRemoveCategoryNode(tnx);
					}
				}
			}
		}
		[Description("Delete selected shortcut. If there is not a selected node or the selected node is not a shortcut then this method does nothing. If parameter 'confirm' is true then a message box appears to ask the user to confirm the deletion.")]
		public void DeleteSelectedShortcut(bool confirm)
		{
			TreeNodeShortcut tns = SelectedNode as TreeNodeShortcut;
			if (tns != null)
			{
				bool bOK = !confirm;
				if (!bOK)
				{
					string txt = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Do you want to delete shortcut '{0}'?", tns.Text);
					if (MessageBox.Show(this.FindForm(), txt, "Delete shortcut", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						bOK = true;
					}
				}
				if (bOK)
				{
					tns.Remove();
					if (tns.PointerXmlNode != null)
					{
						XmlNode pr = tns.PointerXmlNode.ParentNode;
						if (pr != null)
						{
							pr.RemoveChild(tns.PointerXmlNode);
						}
					}
				}
			}
		}
		[Description("Delete selected value node. If there is not a selected node or the selected node is not for value then this method does nothing. If parameter 'confirm' is true then a message box appears to ask the user to confirm the deletion.")]
		public void DeleteSelectedValue(bool confirm)
		{
			if (SelectedNode != null)
			{
				TreeNodeValue tnv = SelectedNode as TreeNodeValue;
				if (tnv != null && !tnv.IsShortcut)
				{
					TreeNodeX tnx = tnv.Parent as TreeNodeX;
					if (tnx != null && !tnx.IsShortcut)
					{
						bool bOK = !confirm;
						if (!bOK)
						{
							string txt = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Do you want to delete value '{0}'?", tnv.Text);
							if (MessageBox.Show(this.FindForm(), txt, "Delete value", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
							{
								bOK = true;
							}
						}
						if (bOK)
						{
							tnx.DeleteValue(tnv.DataName);
							tnv.Remove();
							onRemoveValueNode(tnx, tnv.DataName);
						}
					}
				}
			}
		}
		[Description("Make selected category node to be a root node")]
		public virtual void MoveSelectedNodeToRoot()
		{
			TreeNodeX tnx = SelectedNode as TreeNodeX;
			if (tnx != null)
			{
				if (!tnx.IsShortcut)
				{
					TreeNodeX tp = tnx.Parent as TreeNodeX;
					if (tp != null)
					{
						if (tp.DataXmlNode != null)
						{
							XmlNode xml = tp.DataXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}']", XML_Item, XMLATT_Guid, tnx.TreeNodeId.ToString("D")));
							if (xml != null)
							{
								tp.DataXmlNode.RemoveChild(xml);
								tp.DataXmlNode.OwnerDocument.DocumentElement.AppendChild(xml);
							}
						}
						tnx.Remove();
						Nodes.Add(tnx);
					}
				}
			}
		}
		[Description("Attach a new property to the selected category node and return the new node representing the new property. A dialogue appears asking for the type of the new data. The default value for the data type is used as the new value. If there is not a category node selected then this method does nothing.")]
		public TreeNodeValue CreateNewProperty()
		{
			if (SelectedNode != null)
			{
				TreeNodeX p = SelectedNode as TreeNodeX;
				if (p != null)
				{
					p.Expand();
					p.LoadNextLevel();
					Form f = this.FindForm();
					Type t = DlgTypeSelector.SelectType(f);
					if (t != null)
					{
						TreeNodeValue tnv = p.CreateValue(t);
						SelectedNode = tnv;
						SyncShortcutsValueList(p);
						return tnv;
					}
				}
			}
			return null;
		}
		[Description("Move the tree node up")]
		public void MoveNodeUp(TreeNode node)
		{
			if (node.Index > 0)
			{
				bool isSelected = (SelectedNode == node);
				XmlNode xml = getNodeXmlData(node);
				TreeNode np = node.Parent;
				if (np != null)
				{
					np.Expand();
					if (np.Nodes.Count > 1)
					{
						int n = node.Index - 1;
						node.Remove();
						np.Nodes.Insert(n, node);
						if (xml != null)
						{
							XmlNode xmlP = xml.ParentNode;
							if (xmlP != null)
							{
								XmlNode xmlBefore = xml.PreviousSibling;
								if (xmlBefore != null)
								{
									xmlP.RemoveChild(xml);
									xmlP.InsertBefore(xml, xmlBefore);
								}
							}
						}
					}
				}
				else
				{
					if (Nodes.Count > 1)
					{
						int n = node.Index - 1;
						node.Remove();
						Nodes.Insert(n, node);
						if (xml != null)
						{
							XmlNode xmlP = xml.ParentNode;
							if (xmlP != null)
							{
								XmlNode xmlBefore = xml.PreviousSibling;
								if (xmlBefore != null)
								{
									xmlP.RemoveChild(xml);
									xmlP.InsertBefore(xml, xmlBefore);
								}
							}
						}
					}
				}
				if (isSelected)
				{
					SelectedNode = node;
				}
			}
		}
		[Description("Move the tree node down")]
		public void MoveNodeDown(TreeNode node)
		{
			bool isSelected = (SelectedNode == node);
			XmlNode xml = getNodeXmlData(node);
			TreeNode np = node.Parent;
			if (np != null)
			{
				np.Expand();
				if (np.Nodes.Count > 1)
				{
					if (node.Index < np.Nodes.Count - 1)
					{
						int n = node.Index + 1;
						node.Remove();
						np.Nodes.Insert(n, node);
						if (xml != null)
						{
							XmlNode xmlP = xml.ParentNode;
							if (xmlP != null)
							{
								XmlNode xmlAfter = xml.NextSibling;
								if (xmlAfter != null)
								{
									xmlP.RemoveChild(xml);
									xmlP.InsertAfter(xml, xmlAfter);
								}
							}
						}
					}
				}
			}
			else
			{
				if (Nodes.Count > 1)
				{
					if (node.Index < Nodes.Count - 1)
					{
						int n = node.Index + 1;
						node.Remove();
						Nodes.Insert(n, node);
						if (xml != null)
						{
							XmlNode xmlP = xml.ParentNode;
							if (xmlP != null)
							{
								XmlNode xmlAfter = xml.NextSibling;
								if (xmlAfter != null)
								{
									xmlP.RemoveChild(xml);
									xmlP.InsertAfter(xml, xmlAfter);
								}
							}
						}
					}
				}
			}
			if (isSelected)
			{
				SelectedNode = node;
			}
		}
		[Description("Move selected node up")]
		public void MoveSelectedNodeUp()
		{
			if (SelectedNode != null)
			{
				MoveNodeUp(SelectedNode);
			}
		}
		[Description("Move selected node down")]
		public void MoveSelectedNodeDown()
		{
			if (SelectedNode != null)
			{
				MoveNodeDown(SelectedNode);
			}
		}
		[Description("Launch Tree Nodes Editor to edit the nodes and values. It returns false if the editing is canceled.")]
		public bool EditNodes()
		{
			Form f = this.FindForm();
			DlgTreeViewXEditor dlg = new DlgTreeViewXEditor();
			dlg.LoadData(this);
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				this.XmlString = dlg.XmlString;
				return true;
			}
			return false;
		}
		[Description("Remove all nodes")]
		public void RemoveAllNodes()
		{
			Nodes.Clear();
		}
		[Description("Remove all shortcuts specified by id")]
		public void RemoveAllShortcuts(Guid id)
		{
			List<TreeNodeShortcut> l = new List<TreeNodeShortcut>();
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeShortcut ts = Nodes[i] as TreeNodeShortcut;
				if (ts != null && ts.TreeNodeId == id)
				{
					l.Add(ts);
				}
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.RemoveAllShortcuts(id);
				}
			}
			foreach (TreeNodeShortcut ts in l)
			{
				ts.Remove();
			}
		}
		[Description("Show properties of the selected node in a dialogue box")]
		public void ShowSelectedObjectProperties()
		{
			string name = "Tree View";
			object v = this;
			if (SelectedNode != null)
			{
				TreeNodeValue tnv = SelectedNode as TreeNodeValue;
				if (tnv != null)
				{
					name = "Associated Value";
					v = tnv.Data;
				}
				else
				{
					if (SelectedNode is TreeNodeShortcut)
					{
						name = "Shortcut";
					}
					else
					{
						name = "Node";
					}
					v = SelectedNode;
				}
			}
			DlgProperties dlg = new DlgProperties();
			dlg.SetObject(v, name);
			dlg.ShowDialog(this.FindForm());
		}
		[Description("Save the contents of this control to an XmlDocument")]
		public virtual XmlDocument SaveToXmlDocument()
		{
			XmlDocument doc = GetXmlDocument();
			//            
			ObjectXmlWriter xw = new ObjectXmlWriter();
			WriteResult r = WriteResult.WriteOK;
			foreach (TreeNode tn in Nodes)
			{
				TreeNodeX tnx = tn as TreeNodeX;
				if (tnx != null)
				{
					if (tnx.WriteToXmlNode(_xmlData, xw) == WriteResult.WriteFail)
					{
						r = WriteResult.WriteFail;
					}
				}
			}
			if (r == WriteResult.WriteFail)
			{
				StringBuilder sb = new StringBuilder();
				if (xw.HasErrors)
				{
					foreach (string s in xw.ErrorCollection)
					{
						sb.Append(s);
						sb.Append("|");
					}
				}
				else
				{
					sb.Append("Error saving to XML file");
				}
				_error = sb.ToString();
				return null;
			}
			_error = string.Empty;
			return doc;
		}
		[Description("Save the contents of this control to an XML file")]
		public bool SaveToFile(string filename)
		{
			XmlDocument doc = SaveToXmlDocument();
			if (doc == null)
			{
				return false;
			}
			else
			{
				doc.Save(filename);
			}
			return true;
		}
		[Description("load contents from an XML file into this control")]
		public bool LoadFromFile(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			return LoadFromXmlDocument(doc);
		}
		[Description("Load contents from an XmlDocument into this control")]
		public virtual bool LoadFromXmlDocument(XmlDocument doc)
		{
			_error = string.Empty;
			if (doc.DocumentElement != null)
			{
				_error = LoadNextLevelNodes(Nodes, doc.DocumentElement, null);
				if (string.IsNullOrEmpty(_error))
				{
					_xmlData = doc.DocumentElement;
					return true;
				}
			}
			return false;
		}
		[Description("Reload all the shortcuts to the specified node. When the node is changed, call this method to synchronize all its shortcuts.")]
		public void SyncShortcuts(TreeNodeX node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.SyncShortcuts(node);
				}
			}
		}
		[Description("Get a category node identified by guid")]
		public TreeNodeX GetCategoryNodeById(Guid id)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					TreeNodeX ret = tnx.GetCategoryNodeById(id);
					if (ret != null)
					{
						return ret;
					}
				}
			}
			return null;
		}
		public void LoadNodesByQuery(TreeNodeX node)
		{
			int level = 0;
			TreeNodeCollection parent;
			if (node == null)
			{
				parent = this.Nodes;
				BaseLevel = 0;
			}
			else
			{
				parent = node.Nodes;

				level = node.Level + 1;

				node.PrepareCurrentNode();
			}
			TreeNodeXTemplate template = GetNodeTemplate(level);
			if (template == null)
			{
				return;
			}
			if (level >= 0)
			{
				TreeNodeXTemplate pt = template.Parent as TreeNodeXTemplate;
				if (pt != null)
				{
					if (pt.ChildLoadStyle == EnumChildLoad.None)
					{
						return;
					}
				}
				DataQuery dq = GetDataQuery(level);
				if (dq != null && dq.IsConnectionReady)
				{
					if (pt != null)
					{
						if (pt.ChildLoadStyle == EnumChildLoad.Recursion)
						{
							BaseLevel = pt.Level;
						}
						else
						{
							BaseLevel = 0;
						}
					}
					if (dq.Query())
					{
						List<TreeNodeX> ns = new List<TreeNodeX>();
						for (int i = 0; i < parent.Count; i++)
						{
							TreeNodeX nx = parent[i] as TreeNodeX;
							if (nx != null && nx.IsLoadedByDataBinding)
							{
								ns.Add(nx);
							}
						}
						foreach (TreeNodeX nx in ns)
						{
							nx.Remove();
						}
						DataTable tbl = dq.QueryDef.DataTable;
						if (tbl != null)
						{
							FieldList qfs = dq.QueryDef.Fields;

							for (int r = 0; r < tbl.Rows.Count; r++)
							{
								FieldList fs = dq.QueryDef.Fields.Clone() as FieldList;
								for (int c = 0; c < tbl.Columns.Count; c++)
								{
									EPField f = fs[tbl.Columns[c].ColumnName];
									if (f != null)
									{
										f.Value = tbl.Rows[r][c];
										EPField f0 = qfs[f.Name];
										if (f0 != null)
										{
											f0.Value = f.Value;
										}
									}
								}
								template.Fields = fs.Clone() as FieldList;
								TreeNodeX tx = new TreeNodeX(template, fs);
								parent.Add(tx);
								tx.ApplyTemplate();
							}
						}
					}
				}
			}
		}
		[Description("Load root nodes from the database")]
		public void LoadRootNodesFromDatabase()
		{
			LoadNodesByQuery(null);
		}
		[Browsable(false)]
		public virtual bool OnBeforeSetSQL()
		{
			return true;
		}
		[Browsable(false)]
		public DataQuery GetDataQuery(int level)
		{
			if (_dataQueries != null && _dataQueries.Length > 0)
			{
				if (_dataQueries.Length > level)
				{
					return _dataQueries[level];
				}
				else
				{
					return _dataQueries[_dataQueries.Length - 1];
				}
			}
			return null;
		}
		#endregion

		#region override Methods
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDown = true;
			xd = e.X;
			yd = e.Y;
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_mouseDown = false;
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left && _mouseDown)
			{
				if (Math.Abs(e.X - xd) > 2 || Math.Abs(e.Y - yd) > 2)
				{
					_mouseDown = false;
					System.Drawing.Point pt = new System.Drawing.Point(e.X, e.Y);
					TreeNode nd = GetNodeAt(pt);
					if (nd != null)
					{
						DataObject obj = null;
						DragData dragData = null;
						TreeNodeShortcut tns = nd as TreeNodeShortcut;
						if (tns != null)
						{
							if (tns.PointerXmlNode != null)
							{
								obj = new DataObject(CLIPDATA_NODE, tns.PointerXmlNode.OuterXml);
								obj.SetData(CLIPDATA_SHORTCUT, "True");
							}
							dragData = new DragDataShortcut(tns);
						}
						else
						{
							TreeNodeX tnx = nd as TreeNodeX;
							if (tnx != null)
							{
								if (tnx.DataXmlNode != null)
								{
									obj = new DataObject(CLIPDATA_NODE, tnx.DataXmlNode.OuterXml);
								}
								dragData = new DragDataCategory(tnx);
							}
						}
						if (dragData != null)
						{
							SelectedNode = nd;
							if (obj != null)
							{
								obj.SetData(CLIPDATA_TYPE, dragData);
								obj.SetData(CLIPDATA_TVXGUID, this.TreeViewId.ToString("D"));
								DoDragDrop(obj, DragDropEffects.All);
							}
							else
							{
								DoDragDrop(dragData, DragDropEffects.All);
							}
						}
					}
				}
			}
		}
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			//
			bool isShortCut = e.Data.GetDataPresent(CLIPDATA_SHORTCUT);
			string xml = e.Data.GetData(CLIPDATA_NODE) as string;
			string strId = e.Data.GetData(CLIPDATA_TVXGUID) as string;
			Guid tvId = Guid.Empty;
			if (!string.IsNullOrEmpty(strId))
			{
				tvId = new Guid(strId);
			}
			bool bCanDrop = true;
			if (isShortCut)
			{
				if (this.TreeViewId != tvId)
				{
					//shortcut can only be dropped to the same tree view
					bCanDrop = false;
				}
			}
			Point pt = new Point(e.X, e.Y);
			pt = PointToClient(pt);
			if (bCanDrop)
			{
				object v = e.Data.GetData(CLIPDATA_TYPE);
				DragData dd = v as DragData;
				bCanDrop = false;
				if (!string.IsNullOrEmpty(xml) || dd != null)
				{
					TreeNode nd = GetNodeAt(pt);
					if (nd != null)
					{
						//check can-drop
						TreeNodeX ndx = nd as TreeNodeX;
						if (ndx != null && !ndx.IsShortcut)
						{
							if (!string.IsNullOrEmpty(xml))
							{
								//shortcut cannot be dropped across applications
								bCanDrop = !isShortCut || dd != null;
							}
							else if (dd != null)
							{
								bCanDrop = dd.CanDrop(ndx);
							}
							if (bCanDrop)
							{
								e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
								if (_ndDrop != nd)
								{
									if (_ndDrop != null)
									{
										_ndDrop.BackColor = System.Drawing.Color.White;
									}
								}
								_ndDrop = ndx;
								_ndDrop.BackColor = System.Drawing.Color.GreenYellow;
							}
						}
					}
				}
			}
			//
			if (!bCanDrop)
			{
				if (_ndDrop != null)
				{
					_ndDrop.BackColor = System.Drawing.Color.White;
					_ndDrop = null;
				}
			}
			//scroll up/down
			int nDltX, nDltY;
			if (pt.Y < ItemHeight)
			{
				//scroll up
				nDltY = pt.Y - y0;
				if (nDltY < 0) nDltY = -nDltY;
				nDltX = pt.X - x0;
				if (nDltX < 0) nDltX = -nDltX;
				if (nDltX < nDltY) nDltX = nDltY;
				if (nDltX > nDeltaDragMove)
				{
					y0 = pt.Y;
					x0 = pt.X;
					TreeNode nd0 = TopNode;
					//find the first invisible node 
					TreeNode nd1;
					while (nd0 != null)
					{
						nd1 = FindInvisibleNodeUp(nd0);
						if (nd1 != null)
						{
							nd1.EnsureVisible();
							break;
						}
						nd0 = nd0.Parent;
					}
				}
			}
			else if (pt.Y > ClientSize.Height - ItemHeight)
			{
				//scroll down
				nDltY = pt.Y - y0;
				if (nDltY < 0) nDltY = -nDltY;
				nDltX = pt.X - x0;
				if (nDltX < 0) nDltX = -nDltX;
				if (nDltX < nDltY) nDltX = nDltY;
				if (nDltX > nDeltaDragMove)
				{
					y0 = pt.Y;
					x0 = pt.X;
					TreeNode nd0 = GetNodeAt(pt);
					if (nd0 != null)
						nd0 = FindInvisibleNodeDn(nd0);
					if (nd0 != null)
						nd0.EnsureVisible();
				}
			}
		}
		protected override void OnDragLeave(EventArgs e)
		{
			base.OnDragLeave(e);
			if (_ndDrop != null)
			{
				_ndDrop.BackColor = System.Drawing.Color.White;
				_ndDrop = null;
			}
		}
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			try
			{
				string xml = e.Data.GetData(CLIPDATA_NODE) as string;
				object v = e.Data.GetData(CLIPDATA_TYPE);
				DragData dragData = v as DragData;
				bool isShortCut = e.Data.GetDataPresent(CLIPDATA_SHORTCUT);
				string strId = e.Data.GetData(CLIPDATA_TVXGUID) as string;
				Guid tvId = Guid.Empty;
				if (!string.IsNullOrEmpty(strId))
				{
					tvId = new Guid(strId);
				}
				bool bCanDrop = true;
				if (isShortCut)
				{
					if (this.TreeViewId != tvId)
					{
						//shortcut can only be dropped to the same tree view
						bCanDrop = false;
					}
				}
				Point pt = new Point(e.X, e.Y);
				pt = PointToClient(pt);
				if (bCanDrop)
				{
					bCanDrop = false;
					if (!string.IsNullOrEmpty(xml) || dragData != null)
					{
						TreeNode nd = GetNodeAt(pt);
						if (nd != null)
						{
							TreeNodeX tnx = nd as TreeNodeX;
							if (tnx != null && !tnx.IsShortcut)
							{
								if (dragData != null)
								{
									bCanDrop = dragData.CanDrop(tnx);
								}
								if (!bCanDrop && !string.IsNullOrEmpty(xml))
								{
									bCanDrop = !isShortCut || dragData != null;
								}

								if (bCanDrop)
								{
									ContextMenu mnu = new ContextMenu();
									MenuItem mi;
									//
									if (dragData != null)
									{
										mi = new MenuItemWithImage("Move to here", mi_move, TreeViewXResources._move.ToBitmap());
										mi.Tag = dragData;
										mnu.MenuItems.Add(mi);
									}
									//
									mi = new MenuItemWithImage("Copy to here", mi_dragcopy, TreeViewXResources._copy.ToBitmap());
									if (!string.IsNullOrEmpty(xml))
									{
										mi.Tag = new DragDropData(tnx, xml);
									}
									else
									{
										mi.Tag = dragData;
									}
									mnu.MenuItems.Add(mi);
									//
									if (dragData != null)
									{
										if (dragData.DragSource != null && !dragData.DragSource.IsShortcut)
										{
											if (dragData.DropTarget != null)
											{
												if (dragData.DropTarget.TreeView == dragData.DragSource.TreeView)
												{
													mi = new MenuItemWithImage("Create shortcut", mi_createShortcut, TreeViewXResources._shortcut.ToBitmap());
													mi.Tag = dragData;
													mnu.MenuItems.Add(mi);
												}
											}
										}
									}
									//
									if (mnu.MenuItems.Count > 0)
									{
										Point pt0 = new Point(e.X, e.Y);
										pt0 = this.PointToClient(pt0);
										mnu.Show(this, pt0);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(FindForm(), err.Message, "DragDrop", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				clearDrag();
			}
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{

			base.OnBeforeExpand(e);//fire event
			TreeNodeX tnx = e.Node as TreeNodeX;
			if (tnx != null)
			{
				tnx.LoadNextLevel();
			}
		}
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			bool isShortcut = false;
			bool isValue = false;
			TreeNodeShortcut tns = e.Node as TreeNodeShortcut;
			if (tns != null)
			{
				isShortcut = true;
				if (ShortcutNodeSelected != null)
				{
					ShortcutNodeSelected(this, new TreeNodeShortcutEventArgs(tns, e.Action));
				}
				if (CategoryNodeSelected != null)
				{
					CategoryNodeSelected(this, new TreeNodeXEventArgs(tns, e.Action));
				}
			}
			else
			{
				TreeNodeX tnx = e.Node as TreeNodeX;
				if (tnx != null)
				{
					isShortcut = tnx.IsShortcut;
					if (CategoryNodeSelected != null)
					{
						CategoryNodeSelected(this, new TreeNodeXEventArgs(tnx, e.Action));
					}
				}
				else
				{
					TreeNodeValue tnv = e.Node as TreeNodeValue;
					if (tnv != null)
					{
						isShortcut = tnv.IsShortcut;
						isValue = true;
						if (PropertyNodeSelected != null)
						{
							PropertyNodeSelected(this, new TreeNodeValueEventArgs(tnv, e.Action));
						}
					}
				}
			}
			enableContextMenu(isShortcut, isValue);
		}
		protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
		{
			base.OnNodeMouseClick(e);
			TreeNodeShortcut tns = e.Node as TreeNodeShortcut;
			if (tns != null)
			{
				if (ShortcutMouseClick != null)
				{
					ShortcutMouseClick(this, new TreeNodeShortcutMouseClickEventArgs(tns, e));
				}
			}
			else
			{
				TreeNodeX tnx = e.Node as TreeNodeX;
				if (tnx != null)
				{
					if (CategoryNodeMouseClick != null)
					{
						CategoryNodeMouseClick(this, new TreeNodeXMouseClickEventArgs(tnx, e));
					}
				}
				else
				{
					TreeNodeValue tnv = e.Node as TreeNodeValue;
					if (tnv != null)
					{
						if (PropertyNodeMouseClick != null)
						{
							PropertyNodeMouseClick(this, new TreeNodePropertyMouseClickEventArgs(tnv, e));
						}
					}
				}
			}
		}
		protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
		{
			base.OnNodeMouseDoubleClick(e);
			TreeNodeShortcut tns = e.Node as TreeNodeShortcut;
			if (tns != null)
			{
				if (ShortcutMouseDoubleClick != null)
				{
					ShortcutMouseDoubleClick(this, new TreeNodeShortcutMouseClickEventArgs(tns, e));
				}
			}
			else
			{
				TreeNodeX tnx = e.Node as TreeNodeX;
				if (tnx != null)
				{
					if (CategoryNodeMouseDoubleClick != null)
					{
						CategoryNodeMouseDoubleClick(this, new TreeNodeXMouseClickEventArgs(tnx, e));
					}
				}
				else
				{
					TreeNodeValue tnv = e.Node as TreeNodeValue;
					if (tnv != null)
					{
						if (PropertyNodeMouseDoubleClick != null)
						{
							PropertyNodeMouseDoubleClick(this, new TreeNodePropertyMouseClickEventArgs(tnv, e));
						}
					}
				}
			}
		}
		protected override void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
		{
			base.OnNodeMouseHover(e);
			TreeNodeShortcut tns = e.Node as TreeNodeShortcut;
			if (tns != null)
			{
				if (ShortcutMouseHover != null)
				{
					ShortcutMouseHover(this, new TreeNodeShortcutEventArgs(tns, TreeViewAction.Unknown));
				}
			}
			else
			{
				TreeNodeX tnx = e.Node as TreeNodeX;
				if (tnx != null)
				{
					if (CategoryNodeMouseHover != null)
					{
						CategoryNodeMouseHover(this, new TreeNodeXEventArgs(tnx, TreeViewAction.Unknown));
					}
				}
				else
				{
					TreeNodeValue tnv = e.Node as TreeNodeValue;
					if (tnv != null)
					{
						if (PropertyNodeMouseHover != null)
						{
							PropertyNodeMouseHover(this, new TreeNodeValueEventArgs(tnv, TreeViewAction.Unknown));
						}
					}
				}
			}
		}
		#endregion

		#region class DragData
		class DragDropData
		{
			public DragDropData(TreeNodeX target, string xml)
			{
				DropTarget = target;
				XmlData = xml;
			}
			public TreeNodeX DropTarget
			{
				get;
				set;
			}
			public string XmlData
			{
				get;
				set;
			}
		}
		abstract class DragData
		{
			private TreeNodeX _dropTarget;
			public DragData()
			{
			}
			public TreeNodeX DropTarget
			{
				get
				{
					return _dropTarget;
				}
				set
				{
					_dropTarget = value;
				}
			}
			public abstract TreeNodeX DragSource { get; }
			public abstract bool CanDrop(TreeNodeX dropTarget);
			public abstract void Move();
			public abstract void CreateShortcut();
		}
		class DragDataCategory : DragData
		{
			private TreeNodeX _drag;
			public DragDataCategory(TreeNodeX drag)
			{
				_drag = drag;
			}
			public override TreeNodeX DragSource
			{
				get
				{
					return _drag;
				}
			}
			/// <summary>
			/// check whether this data can be dropped to the target
			/// </summary>
			/// <param name="dropTarget"></param>
			/// <returns></returns>
			public override bool CanDrop(TreeNodeX dropTarget)
			{
				if (dropTarget == null)
				{
					return false;
				}
				if (dropTarget.IsShortcut)
				{
					return false;
				}
				DropTarget = dropTarget;
				//dropTarget cannot be a child of Data
				//Data (source)
				//  --
				//  --dropTarget
				//
				if (DragSource.Parent == dropTarget)
				{
					return false;
				}
				//
				while (dropTarget != null)
				{
					if (dropTarget.TreeNodeId == DragSource.TreeNodeId)
					{
						return false;
					}
					if (dropTarget.Parent is TreeNodeShortcut)
					{
						return false;
					}
					dropTarget = dropTarget.Parent as TreeNodeX;
				}
				//
				return true;
			}
			public override void Move()
			{
				TreeViewX tvSrc = _drag.TreeView as TreeViewX;
				TreeViewX tv = DropTarget.TreeView as TreeViewX;
				if (tv != null)
				{
					XmlDocument docTarget = tv.GetXmlDocument();
					StringCollection movedIDs = new StringCollection();
					DropTarget.Expand();
					for (int i = 0; i < DropTarget.Nodes.Count; i++)
					{
						TreeNodeX tnx = DropTarget.Nodes[i] as TreeNodeX;
						if (tnx != null)
						{
							if (tnx.TreeNodeId == _drag.TreeNodeId)
							{
								tv.SelectedNode = tnx;
								return;
							}
						}
					}
					_drag.Remove();
					DropTarget.Nodes.Add(_drag);
					XmlNode xml = _drag.DataXmlNode;
					if (xml != null)
					{
						XmlNode p = xml.ParentNode;
						if (p != null)
						{
							p.RemoveChild(xml);
						}

						if (docTarget == xml.OwnerDocument)
						{
							if (DropTarget.DataXmlNode != null)
							{
								DropTarget.DataXmlNode.AppendChild(xml);
							}
						}
						else
						{
							XmlNode xmlNode = docTarget.ImportNode(xml, true);
							_drag.DataXmlNode = xmlNode;
							if (DropTarget.DataXmlNode != null)
							{
								DropTarget.DataXmlNode.AppendChild(xmlNode);
							}
						}
						//remove invalid shortcuts
						if (tv != tvSrc)
						{
							XmlNodeList ndLst = xml.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								". | .//{0}", XML_Item));
							foreach (XmlNode nd in ndLst)
							{
								string id = XmlUtil.GetAttribute(nd, XMLATT_Guid);
								if (!string.IsNullOrEmpty(id))
								{
									//    //not in target
									movedIDs.Add(id);
									XmlNodeList stLst = p.OwnerDocument.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
										"//{0}[@{1}='{2}']", XML_Shortcut, XMLATT_Guid, id));
									foreach (XmlNode st in stLst)
									{
										XmlNode stP = st.ParentNode;
										if (stP != null)
										{
											stP.RemoveChild(st);
										}
									}
								}
							}
						}
					}
					if (tvSrc != null)
					{
						if (tvSrc != tv)
						{
							foreach (string s in movedIDs)
							{
								Guid id = new Guid(s);
								tvSrc.RemoveAllShortcuts(id);
							}
							//remove invalid shortcuts
							List<XmlNode> invShortcuts = new List<XmlNode>();
							XmlNode xml2 = _drag.DataXmlNode;
							XmlNodeList ndLst = xml2.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
									".//{0}", XML_Shortcut));
							foreach (XmlNode nd in ndLst)
							{
								bool bOK = false;
								string id = XmlUtil.GetAttribute(nd, XMLATT_Guid);
								if (!string.IsNullOrEmpty(id))
								{
									XmlNode n0 = docTarget.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
											"//{0}[@{1}='{2}']", XML_Item, XMLATT_Guid, id));
									bOK = (n0 != null);
								}
								if (!bOK)
								{
									invShortcuts.Add(nd);
								}
							}
							foreach (XmlNode nd in ndLst)
							{
								XmlNode np = nd.ParentNode;
								if (np != null)
								{
									np.RemoveChild(nd);
								}
							}
						}
					}
					tv.SelectedNode = _drag;
				}
			}
			public override void CreateShortcut()
			{
				TreeViewX tv = DropTarget.TreeView as TreeViewX;
				if (tv != null)
				{
					DropTarget.Expand();
					for (int i = 0; i < DropTarget.Nodes.Count; i++)
					{
						TreeNodeX tnx = DropTarget.Nodes[i] as TreeNodeX;
						if (tnx != null)
						{
							if (tnx.TreeNodeId == _drag.TreeNodeId)
							{
								tv.SelectedNode = tnx;
								return;
							}
						}
					}
					TreeNodeShortcut tns = _drag.CreateShortcut();
					DropTarget.Nodes.Add(tns);

					tv.SetShortcutImage(tns);
					tv.SelectedNode = tns;
				}
			}
		}
		class DragDataShortcut : DragData
		{
			private TreeNodeShortcut _drag;
			public DragDataShortcut(TreeNodeShortcut drag)
			{
				_drag = drag;
			}
			public override TreeNodeX DragSource
			{
				get
				{
					return _drag;
				}
			}
			public TreeNodeShortcut DragShortcutData
			{
				get
				{
					return _drag;
				}
			}
			public override bool CanDrop(TreeNodeX dropTarget)
			{
				if (dropTarget == null)
				{
					return false;
				}
				if (dropTarget.IsShortcut)
				{
					return false;
				}
				DropTarget = dropTarget;
				if (DragSource.Parent == dropTarget)
				{
					return false;
				}
				//
				while (dropTarget != null)
				{
					if (dropTarget.TreeNodeId == DragSource.TreeNodeId)
					{
						return false;
					}
					if (dropTarget.Parent is TreeNodeShortcut)
					{
						return false;
					}
					dropTarget = dropTarget.Parent as TreeNodeX;
				}
				return true;
			}
			public override void Move()
			{
				DropTarget.Expand();
				_drag.Remove();
				DropTarget.Nodes.Add(_drag);
				//
				XmlNode xml = _drag.PointerXmlNode;
				if (xml != null)
				{
					XmlNode p = xml.ParentNode;
					if (p != null)
					{
						p.RemoveChild(xml);
					}
					if (DropTarget.DataXmlNode != null)
					{
						if (DropTarget.DataXmlNode.OwnerDocument == xml.OwnerDocument)
						{
							DropTarget.DataXmlNode.AppendChild(xml);
						}
						else
						{
							XmlNode xmlNode = DropTarget.DataXmlNode.OwnerDocument.ImportNode(xml, true);
							DropTarget.DataXmlNode.AppendChild(xmlNode);
						}
					}
				}
				TreeView tv = DropTarget.TreeView;
				if (tv != null)
				{
					tv.SelectedNode = _drag;
				}
			}
			public override void CreateShortcut()
			{
				DropTarget.Expand();
				TreeNodeShortcut tns = _drag.CreateShortcut();
				DropTarget.Nodes.Add(tns);
				TreeView tv = DropTarget.TreeView;
				if (tv != null)
				{
					tv.SelectedNode = tns;
				}
			}
		}
		#endregion

		#region Non-Browsable Methods
		[NotForProgramming]
		[Browsable(false)]
		public DataQuery CreateDataQuery(int level)
		{
			if (level >= 0)
			{
				if (_dataQueries == null)
				{
					_dataQueries = new DataQuery[] { };
				}
				if (_dataQueries.Length <= level)
				{
					int n = level + 1;
					DataQuery[] a = new DataQuery[n];
					_dataQueries.CopyTo(a, 0);
					for (int i = _dataQueries.Length; i < a.Length; i++)
					{
						a[i] = new DataQuery();
						a[i].SetOwner(this);
						a[i].SetDevClass(_class);
					}
					_dataQueries = a;
				}
				return _dataQueries[level];
			}
			return null;
		}
		[NotForProgramming]
		[Browsable(false)]
		public void UpdateDataQuery(int level, DataQuery query)
		{
			if (level >= 0)
			{
				CreateDataQuery(level);
				_dataQueries[level] = query;
				if (query != null)
				{
					query.SetOwner(this);
					query.SetDevClass(_class);
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public TreeNodeXTemplate GetNodeTemplateBase(int level0)
		{
			int level = level0 + BaseLevel;
			return GetNodeTemplate(level);
		}
		[NotForProgramming]
		[Browsable(false)]
		public TreeNodeXTemplate GetNodeTemplate(int level)
		{
			if (level >= 0)
			{
				TreeNodeXTemplate tt = RootNodesTemplate;
				while (tt != null)
				{
					if (tt.Level == level)
					{
						return tt;
					}
					TreeNodeXTemplate tt0 = null;
					if (tt.Nodes.Count > 0)
					{
						for (int i = 0; i < tt.Nodes.Count; i++)
						{
							tt0 = tt.Nodes[i] as TreeNodeXTemplate;
							if (tt0 != null)
							{
								break;
							}
						}
					}
					if (tt0 == null)
					{
						tt0 = tt.CloneTemplate();
						tt.Nodes.Add(tt0);
						tt = tt0;
					}
					else
					{
						tt = tt0;
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		public void OnValueListLoaded(TreeNodeX node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnValueListLoaded(node);
				}
			}
		}
		[Browsable(false)]
		protected void SetError(string msg)
		{
			_error = msg;
		}
		[Browsable(false)]
		protected void SetXmlNode(XmlNode xml)
		{
			_xmlData = xml;
		}
		/// <summary>
		/// for copy/paste. Guid's are re-created
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[Browsable(false)]
		public TreeNodeX CreateNodeFromXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNode ndSrc = doc.DocumentElement;
			if (ndSrc != null)
			{
				XmlDocument docThis = GetXmlDocument();
				if (string.CompareOrdinal(ndSrc.Name, XML_Shortcut) == 0)
				{
					Guid id = XmlUtil.GetAttributeGuid(ndSrc, XMLATT_Guid);
					XmlNode ndtest = docThis.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							   "//{0}[@{1}='{2}']",
							   XML_Item, XMLATT_Guid, id.ToString("D")));
					if (ndtest != null)
					{
						XmlNode nd = docThis.ImportNode(ndSrc, true);
						TreeNodeShortcut tns;
						TreeNodeX tnx = GetCategoryNodeById(id);
						if (tnx != null)
						{
							tns = tnx.CreateShortcut();
							tns.SetPointerXml(nd);
						}
						else
						{
							ObjectXmlReader xr = new ObjectXmlReader();
							tns = new TreeNodeShortcut(nd, xr);
							setReferencedNode(tns);
						}
						SetShortcutImage(tns);
						return tns;
					}
					else
					{
						return null;
					}
				}
				else
				{
					XmlNodeList ndLst = ndSrc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						   "//{0}[@{1}]", XML_Item, TreeViewX.XMLATT_Guid));
					Dictionary<Guid, Guid> idMaps = new Dictionary<Guid, Guid>();
					foreach (XmlNode nd in ndLst)
					{
						Guid id = XmlUtil.GetAttributeGuid(nd, TreeViewX.XMLATT_Guid);
						if (id == Guid.Empty)
						{
							throw new TreeViewXException("Guid not found");
						}
						else
						{
							Guid id2 = Guid.Empty;
							if (!idMaps.TryGetValue(id, out id2))
							{
								id2 = Guid.NewGuid();
								idMaps.Add(id, id2);
							}
							XmlUtil.SetAttribute(nd, TreeViewX.XMLATT_Guid, id2.ToString("D"));
						}
					}
					ndLst = ndSrc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						   "//{0}[@name='TreeNodeId']", XML_PROPERTY));
					foreach (XmlNode nd in ndLst)
					{
						if (!string.IsNullOrEmpty(nd.InnerText))
						{
							Guid id2;
							Guid id = new Guid(nd.InnerText);
							if (idMaps.TryGetValue(id, out id2))
							{
								nd.InnerText = id2.ToString("D");
							}
						}
					}
					List<XmlNode> invalidShortcuts = new List<XmlNode>();
					ndLst = ndSrc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						   "//{0}", XML_Shortcut));
					foreach (XmlNode nd in ndLst)
					{
						Guid id2;
						Guid id = XmlUtil.GetAttributeGuid(nd, TreeViewX.XMLATT_Guid);
						if (idMaps.TryGetValue(id, out id2))
						{
							XmlUtil.SetAttribute(nd, TreeViewX.XMLATT_Guid, id2.ToString("D"));
						}
						else
						{
							XmlNode ndtest = docThis.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"//{0}[@{1}='{2}']",
								XML_Item, XMLATT_Guid, id.ToString("D")));
							if (ndtest == null)
							{
								invalidShortcuts.Add(nd);
							}
						}
					}
					foreach (XmlNode nd in invalidShortcuts)
					{
						XmlNode p = nd.ParentNode;
						p.RemoveChild(nd);
					}
					XmlNode dataXml = docThis.ImportNode(ndSrc, true);
					Type type = XmlUtil.GetLibTypeAttribute(dataXml);
					TreeNodeX tnx = (TreeNodeX)Activator.CreateInstance(type, dataXml);
					ObjectXmlReader oxr = new ObjectXmlReader();
					oxr.ReadProperties(dataXml, tnx);
					return tnx;
				}
			}
			return null;
		}
		[Browsable(false)]
		public XmlDocument GetXmlDocument()
		{
			XmlDocument doc;
			if (_xmlData != null)
			{
				doc = _xmlData.OwnerDocument;
			}
			else
			{
				doc = new XmlDocument();
				_xmlData = doc.CreateElement("TreeViewX");
				doc.AppendChild(_xmlData);
				XmlUtil.SetAttribute(_xmlData, XMLATT_Guid, TreeViewId.ToString("D"));
				this.ExpandAll();
			}
			return doc;
		}
		[Browsable(false)]
		public void SyncShortcutsValueList(TreeNodeX node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.SyncShortcutsValueList(node);
				}
			}
		}
		[Browsable(false)]
		public void SetEditMenu()
		{
			if (this.ContextMenuStrip != null)
			{
			}
		}
		[Browsable(false)]
		public virtual string LoadNextLevelNodes(TreeNodeCollection nodes, XmlNode node, TreeNodeX valueOwner)
		{
			bool valueLoaded = false;
			bool isForShortcut = (valueOwner != null && valueOwner.IsShortcut);
			ObjectXmlReader xr = new ObjectXmlReader();
			XmlNodeList nds = node.ChildNodes;//.SelectNodes(XML_Item);
			foreach (XmlNode nd in nds)
			{
				if (string.CompareOrdinal(nd.Name, XML_Item) == 0)
				{
					TreeNodeX tnx;
					if (isForShortcut)
					{
						TreeNodePointer tnp = xr.ReadObject<TreeNodePointer>(nd);
						tnx = tnp;
						setReferencedNode(tnp);
					}
					else
					{
						tnx = xr.ReadObject<TreeNodeX>(nd);
					}
					nodes.Add(tnx);
					if (tnx.IsShortcut)
					{
					}
					else
					{
						onCategoryNodeLoaded(tnx);
					}
				}
				else if (string.CompareOrdinal(nd.Name, XML_Shortcut) == 0)
				{
					TreeNodeShortcut tns;
					Guid id = XmlUtil.GetAttributeGuid(nd, XMLATT_Guid);
					TreeNodeX tnx = GetCategoryNodeById(id);
					if (tnx != null)
					{
						tns = tnx.CreateShortcut();
						tns.SetPointerXml(nd);
					}
					else
					{
						tns = new TreeNodeShortcut(nd, xr);
						setReferencedNode(tns);
					}
					SetShortcutImage(tns);
					nodes.Add(tns);
				}
				else if (string.CompareOrdinal(nd.Name, XML_Value) == 0)
				{
					if (valueOwner != null)
					{
						TreeNodeValue tnv = new TreeNodeValue(nd, xr);
						tnv.IsShortcut = valueOwner.IsShortcut;

						if (!tnv.IsShortcut)
						{
							valueOwner.AddValue(tnv.Name, tnv.Value);
						}
						if (ShowPropertyNodes && valueOwner.ShowPropertyNodes)
						{
							nodes.Add(tnv);
						}
						valueLoaded = true;
					}
				}
			}
			if (valueOwner != null)
			{
				if (valueLoaded)
				{
					if (!valueOwner.IsShortcut)
					{
						OnValueListLoaded(valueOwner);
					}
				}
			}
			if (xr.HasErrors)
			{
				StringBuilder sb = new StringBuilder();
				foreach (string s in xr.Errors)
				{
					sb.Append(s);
					sb.Append("|");
				}
				return sb.ToString();
			}
			return string.Empty;
		}
		[Browsable(false)]
		public void SetShortcutImage(TreeNodeShortcut shortcut)
		{
			if (ImageList != null)
			{

				int imgIdx = -1;
				if (shortcut.ReferencedNode != null)
				{
					imgIdx = shortcut.ReferencedNode.ImageIndex;
				}
				else
				{
					imgIdx = shortcut.ImageIndex;
				}
				string key = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"img{0}", imgIdx);
				if (ImageList.Images.IndexOfKey(key) < 0)
				{
					if (imgIdx >= 0 && imgIdx < ImageList.Images.Count)
					{
						Image img = ImageList.Images[imgIdx];
						Bitmap img2 = new Bitmap(img);
						//
						Graphics g = Graphics.FromImage(img2);
						int x = 0;
						int y = img2.Height - TreeViewXResources._shortcutPart.Height;
						if (y < 0)
						{
							y = 0;
						}
						g.DrawIcon(TreeViewXResources._shortcutPart, x, y);
						ImageList.Images.Add(key, img2);
					}
					else
					{
						ImageList.Images.Add(key, TreeViewXResources._shortcutPart);
					}
				}
				shortcut.ImageIndex = -1;
				shortcut.ImageKey = key;

				int imgIdx2 = -1;
				if (shortcut.ReferencedNode != null)
				{
					imgIdx2 = shortcut.ReferencedNode.SelectedImageIndex;
				}
				else
				{
					imgIdx2 = shortcut.SelectedImageIndex;
				}
				if (imgIdx == imgIdx2)
				{
					shortcut.SelectedImageIndex = -1;
					shortcut.SelectedImageKey = key;
				}
				else
				{
					string key2 = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"img{0}", imgIdx2);
					if (ImageList.Images.IndexOfKey(key2) < 0)
					{
						if (imgIdx2 >= 0 && imgIdx2 < ImageList.Images.Count)
						{
							Image img = ImageList.Images[imgIdx2];
							Bitmap img2 = new Bitmap(img);
							//
							Graphics g = Graphics.FromImage(img2);
							int x = 0;
							int y = img2.Height - TreeViewXResources._shortcutPart.Height;
							if (y < 0)
							{
								y = 0;
							}
							g.DrawIcon(TreeViewXResources._shortcutPart, x, y);
							ImageList.Images.Add(key2, img2);
						}
						else
						{
							ImageList.Images.Add(key2, TreeViewXResources._shortcutPart);
						}
					}
					shortcut.SelectedImageIndex = -1;
					shortcut.SelectedImageKey = key2;
				}
			}
		}
		[Browsable(false)]
		public void OnValueNameChanged(Guid categoryId, string oldName, string valueName)
		{
			foreach (TreeNode tn in Nodes)
			{
				TreeNodeX tnx = tn as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnValueNameChanged(categoryId, oldName, valueName);
				}
			}
		}
		/// <summary>
		/// use sourceNode as a template to create a new node.
		/// sourceNode may be from another TreeViewX
		/// </summary>
		/// <param name="sourceNode"></param>
		[Browsable(false)]
		public void CreateNodeCopy(TreeNodeX sourceNode)
		{
			if (sourceNode != null)
			{
				TreeNodeX parentNode = null;
				TreeNode p = SelectedNode;
				while (p != null)
				{
					TreeNodeX px = p as TreeNodeX;
					if (px != null)
					{
						if (!px.IsShortcut)
						{
							parentNode = px;
							break;
						}
					}
					p = p.Parent;
				}
				TreeNodeX tnx = sourceNode.CreateDuplicatedNode(this);
				if (parentNode != null)
				{
					parentNode.Nodes.Add(tnx);
					if (tnx.DataXmlNode != null)
					{
						if (parentNode.DataXmlNode != null)
						{
							parentNode.DataXmlNode.AppendChild(tnx.DataXmlNode);
						}
					}
				}
				else
				{
					Nodes.Add(tnx);
					if (tnx.DataXmlNode != null)
					{
						_xmlData.AppendChild(tnx.DataXmlNode);
					}
				}
			}
		}
		#endregion

		#region private methods
		private void mi_addRootNode(object sender, EventArgs e)
		{
			AddRootNode("Root node");
		}
		private void mi_addSubNode(object sender, EventArgs e)
		{
			AddSubNode("sub node");
		}
		private void mi_moveToRoot(object sender, EventArgs e)
		{
			MoveSelectedNodeToRoot();
		}
		private void mi_moveUp(object sender, EventArgs e)
		{
			MoveSelectedNodeUp();
		}
		private void mi_moveDown(object sender, EventArgs e)
		{
			MoveSelectedNodeDown();
		}
		private void mi_deleteNode(object sender, EventArgs e)
		{
			DeleteSelectedCategoryNode(true);
		}
		private void mi_deleteShortcut(object sender, EventArgs e)
		{
			DeleteSelectedShortcut(true);
		}
		private void mi_deleteValue(object sender, EventArgs e)
		{
			DeleteSelectedValue(true);
		}
		private void mi_addValue(object sender, EventArgs e)
		{
			CreateNewProperty();
		}
		private void mi_refresh(object sender, EventArgs e)
		{
			if (SelectedNode != null)
			{
				TreeNodeX p = SelectedNode as TreeNodeX;
				if (p != null)
				{
					p.ResetNextLevel();
				}
			}
		}
		private void mi_properties(object sender, EventArgs e)
		{
			ShowSelectedObjectProperties();
		}
		private void mi_copy(object sender, EventArgs e)
		{
			if (SelectedNode != null)
			{
				DataObject obj = null;
				TreeNodeShortcut s = SelectedNode as TreeNodeShortcut;
				if (s != null)
				{
					if (s.PointerXmlNode != null)
					{
						obj = new DataObject(CLIPDATA_NODE, s.PointerXmlNode.OuterXml);
						obj.SetData(CLIPDATA_SHORTCUT, "True");
					}
				}
				else
				{
					TreeNodeX p = SelectedNode as TreeNodeX;
					if (p != null)
					{
						if (p.DataXmlNode != null)
						{
							obj = new DataObject(CLIPDATA_NODE, p.DataXmlNode.OuterXml);
						}
					}
				}
				if (obj != null)
				{
					Clipboard.Clear();
					obj.SetData(CLIPDATA_TVXGUID, this.TreeViewId.ToString("D"));
					Clipboard.SetDataObject(obj);
				}
			}
		}
		private void mi_paste(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(CLIPDATA_NODE))
			{
				DataObject v = Clipboard.GetDataObject() as DataObject;
				if (v != null)
				{
					string xml = v.GetData(CLIPDATA_NODE) as string;
					createNodeFromXml(SelectedNode as TreeNodeX, xml);

				}
			}
		}
		private void createNodeFromXml(TreeNodeX p, string xml)
		{
			if (!string.IsNullOrEmpty(xml))
			{
				TreeNodeX tnx = CreateNodeFromXml(xml);
				if (tnx != null)
				{
					TreeNodeShortcut tnsh = tnx as TreeNodeShortcut;
					if (tnsh != null)
					{
						if (p != null)
						{
							p.Expand();
							for (int i = 0; i < p.Nodes.Count; i++)
							{
								TreeNodeX ts = p.Nodes[i] as TreeNodeX;
								if (ts != null)
								{
									if (ts.TreeNodeId == tnsh.TreeNodeId)
									{
										return;
									}
								}
							}
						}
						else
						{
							for (int i = 0; i < Nodes.Count; i++)
							{
								TreeNodeShortcut ts = Nodes[i] as TreeNodeShortcut;
								if (ts != null)
								{
									if (ts.TreeNodeId == tnsh.TreeNodeId)
									{
										return;
									}
								}
							}
						}
					}
					if (p != null)
					{
						p.Expand();
						p.Nodes.Add(tnx);
						if (p.DataXmlNode != null)
						{
							p.DataXmlNode.AppendChild(tnx.DataXmlNode);
						}
					}
					else
					{
						Nodes.Add(tnx);
						RootXmlNode.AppendChild(tnx.DataXmlNode);
					}
					SelectedNode = tnx;
				}
			}
		}
		private void enableContextMenu(bool isShortcut, bool isValue)
		{
			ContextMenuStrip cms = this.ContextMenuStrip;
			if (cms != null && cms.Items.Count > 0)
			{
				for (int i = 0; i < cms.Items.Count; i++)
				{
					if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDROOT) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.AddRootNode) == EnumTreeViewMenu.AddRootNode)
						{
							cms.Items[i].Visible = true;
						}
						else
						{
							cms.Items[i].Visible = false;
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDSUB) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.AddSubNode) != EnumTreeViewMenu.AddSubNode)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (SelectedNode != null && !isShortcut && !isValue);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_MOVETOROOT) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.MoveNodeToRoot) != EnumTreeViewMenu.MoveNodeToRoot)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (SelectedNode != null && !isValue);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDVALUE) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.AttachValue) != EnumTreeViewMenu.AttachValue)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (SelectedNode != null && !isShortcut && !isValue);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVEVALUE) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.RemoveSelectedValue) != EnumTreeViewMenu.RemoveSelectedValue)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (isValue);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVENODE) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.RemoveSelectedNode) != EnumTreeViewMenu.RemoveSelectedNode)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (SelectedNode != null);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_PASTE) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.Paste) != EnumTreeViewMenu.Paste)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = Clipboard.ContainsData(CLIPDATA_NODE);
						}
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_COPY) == 0)
					{
						cms.Items[i].Visible = ((_menuItems & EnumTreeViewMenu.Copy) == EnumTreeViewMenu.Copy);
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_PROPERTIES) == 0)
					{
						cms.Items[i].Visible = ((_menuItems & EnumTreeViewMenu.Properties) == EnumTreeViewMenu.Properties);
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REFRESH) == 0)
					{
						cms.Items[i].Visible = ((_menuItems & EnumTreeViewMenu.Refresh) == EnumTreeViewMenu.Refresh);
					}
					else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVESHORTCUT) == 0)
					{
						if ((_menuItems & EnumTreeViewMenu.RemoveSelectedShortcut) != EnumTreeViewMenu.RemoveSelectedShortcut)
						{
							cms.Items[i].Visible = false;
						}
						else
						{
							cms.Items[i].Visible = (SelectedNode != null && isShortcut);
						}
					}
				}
			}
		}
		private void adjustContextMenu(ContextMenuStrip cms)
		{
			if (!ReadOnly)
			{
				bool bExist = false;
				if (cms.Items.Count > 0)
				{
					for (int i = 0; i < cms.Items.Count; i++)
					{
						if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDROOT) == 0)
						{
							bExist = true;
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDSUB) == 0)
						{
							cms.Items[i].Enabled = (SelectedNode != null);
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_MOVETOROOT) == 0)
						{
							cms.Items[i].Enabled = (SelectedNode != null);
							if (SelectedNode != null)
							{
								TreeNodeX tnx = SelectedNode as TreeNodeX;
								if (tnx == null)
								{
									cms.Items[i].Enabled = false;
								}
								else
								{
									if (tnx.IsShortcut)
									{
										cms.Items[i].Enabled = false;
									}
								}
							}
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_ADDVALUE) == 0)
						{
							cms.Items[i].Enabled = (SelectedNode != null);
							TreeNodeX tnx = SelectedNode as TreeNodeX;
							if (tnx == null)
							{
								cms.Items[i].Enabled = false;
							}
							else
							{
								if (tnx.IsShortcut)
								{
									cms.Items[i].Enabled = false;
								}
							}
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVEVALUE) == 0)
						{
							TreeNodeValue tnv = SelectedNode as TreeNodeValue;
							cms.Items[i].Enabled = (tnv != null);
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVENODE) == 0)
						{
							cms.Items[i].Enabled = (SelectedNode != null);
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_PASTE) == 0)
						{
							cms.Items[i].Enabled = Clipboard.ContainsData(CLIPDATA_NODE);
						}
						else if (string.CompareOrdinal(cms.Items[i].Name, MENU_NAME_REMOVESHORTCUT) == 0)
						{
							cms.Items[i].Enabled = (SelectedNode != null && (SelectedNode is TreeNodeShortcut));
						}
					}
				}
				if (!bExist)
				{
					if (cms.Items.Count > 0)
					{
						cms.Items.Add("-");
					}
					ToolStripItem tsi = new ToolStripMenuItem(TreeViewXResources.AddRootNode, TreeViewXResources.addroot.ToBitmap(), mi_addRootNode);
					tsi.Name = MENU_NAME_ADDROOT;
					cms.Items.Add(tsi);

					//
					tsi = new ToolStripMenuItem(TreeViewXResources.AddSubNode, TreeViewXResources.addsub.ToBitmap(), mi_addSubNode);
					tsi.Name = MENU_NAME_ADDSUB;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.MoveNodeToRoot, TreeViewXResources._toRoot.ToBitmap(), mi_moveToRoot);
					tsi.Name = MENU_NAME_MOVETOROOT;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					else
					{
						TreeNodeX tnx = SelectedNode as TreeNodeX;
						if (tnx == null)
						{
							tsi.Enabled = false;
						}
						else
						{
							if (tnx.IsShortcut)
							{
								tsi.Enabled = false;
							}
						}
					}
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.MoveNodeUp, TreeViewXResources._upIcon.ToBitmap(), mi_moveUp);
					tsi.Name = MENU_NAME_MOVEUP;
					cms.Items.Add(tsi);
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.MoveNodeDown, TreeViewXResources._downIcon.ToBitmap(), mi_moveDown);
					tsi.Name = MENU_NAME_MOVEDOWN;
					cms.Items.Add(tsi);
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.AttachValue, TreeViewXResources.addvalue.ToBitmap(), mi_addValue);
					tsi.Name = MENU_NAME_ADDVALUE;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					else
					{
						TreeNodeX tnx = SelectedNode as TreeNodeX;
						if (tnx == null)
						{
							tsi.Enabled = false;
						}
						else
						{
							if (tnx.IsShortcut)
							{
								tsi.Enabled = false;
							}
						}
					}
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.Refresh, TreeViewXResources._refresh.ToBitmap(), mi_refresh);
					tsi.Name = MENU_NAME_REFRESH;
					cms.Items.Add(tsi);
					//
					cms.Items.Add("-");
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.Properties, TreeViewXResources._properties.ToBitmap(), mi_properties);
					tsi.Name = MENU_NAME_PROPERTIES;
					cms.Items.Add(tsi);
					//
					cms.Items.Add("-");
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.Copy, TreeViewXResources._copy.ToBitmap(), mi_copy);
					tsi.Name = MENU_NAME_COPY;
					cms.Items.Add(tsi);
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.Paste, TreeViewXResources._paste.ToBitmap(), mi_paste);
					tsi.Name = MENU_NAME_PASTE;
					cms.Items.Add(tsi);
					tsi.Enabled = Clipboard.ContainsData(CLIPDATA_NODE);
					//
					cms.Items.Add("-");
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.RemoveSelectedValue, TreeViewXResources.delValue.ToBitmap(), mi_deleteValue);
					tsi.Name = MENU_NAME_REMOVEVALUE;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					else
					{
						TreeNodeValue tnv = SelectedNode as TreeNodeValue;
						tsi.Enabled = (tnv != null);
					}
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.RemoveSelectedNode, TreeViewXResources.delnode.ToBitmap(), mi_deleteNode);
					tsi.Name = MENU_NAME_REMOVENODE;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					//
					tsi = new ToolStripMenuItem(TreeViewXResources.RemoveSelectedShortcut, TreeViewXResources.delnode.ToBitmap(), mi_deleteShortcut);
					tsi.Name = MENU_NAME_REMOVESHORTCUT;
					cms.Items.Add(tsi);
					if (SelectedNode == null)
					{
						tsi.Enabled = false;
					}
					else
					{
						tsi.Enabled = (SelectedNode is TreeNodeShortcut);
					}
				}
			}
		}
		private void mi_dragcopy(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				DragDropData ddd = mi.Tag as DragDropData;
				if (ddd == null)
				{
					DragData dd = mi.Tag as DragData;
					if (dd != null)
					{
						TreeNodeShortcut tns = dd.DragSource as TreeNodeShortcut;
						if (tns != null)
						{
							if (tns.PointerXmlNode != null)
							{
								ddd = new DragDropData(dd.DropTarget, tns.PointerXmlNode.OuterXml);
							}
						}
						else
						{
							TreeNodeX tnx = dd.DragSource as TreeNodeX;
							if (tnx != null)
							{
								ddd = new DragDropData(dd.DropTarget, tnx.DataXmlNode.OuterXml);
							}
						}
					}
				}

				if (ddd != null)
				{
					createNodeFromXml(ddd.DropTarget, ddd.XmlData);
				}
			}
		}
		private void mi_move(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				DragData dd = mi.Tag as DragData;
				if (dd != null)
				{
					dd.Move();
				}
			}
		}
		private void mi_createShortcut(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				DragData dd = mi.Tag as DragData;
				if (dd != null)
				{
					dd.CreateShortcut();
				}
			}
		}
		private void clearDrag()
		{
			if (_ndDrop != null)
			{
				_ndDrop.BackColor = System.Drawing.Color.White;
				_ndDrop = null;
			}
		}

		private void onCategoryNodeLoaded(TreeNodeX node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnCategoryNodeLoaded(node);
				}
			}
		}
		private void onRemoveCategoryNode(TreeNodeX node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnRemoveCategoryNode(node);
				}
			}
		}
		private void onRemoveValueNode(TreeNodeX node, string valueName)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnRemoveValueNode(node, valueName);
				}
			}
		}
		private void setReferencedNode(TreeNodePointer node)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					if (tnx.SetReferencedNode(node))
					{
						break;
					}
				}
			}
		}
		private void onSubNodeCreated(TreeNodeX parentNode, TreeNodeX subNode)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeX tnx = Nodes[i] as TreeNodeX;
				if (tnx != null)
				{
					tnx.OnSubNodeCreated(parentNode, subNode);
				}
			}
		}
		private XmlNode getNodeXmlData(TreeNode node)
		{
			XmlNode xml = null;
			TreeNodeShortcut tns = node as TreeNodeShortcut;
			if (tns != null)
			{
				xml = tns.PointerXmlNode;
			}
			else
			{
				TreeNodeX nodex = node as TreeNodeX;
				if (nodex != null)
				{
					xml = nodex.DataXmlNode;
				}
				else
				{
					TreeNodeValue tnv = node as TreeNodeValue;
					if (tnv != null)
					{
						xml = tnv.DataXmlNode;
					}
				}
			}
			return xml;
		}
		#endregion

		#region Static Methods

		[Browsable(false)]
		static public TreeNode FindInvisibleNodeDn(TreeNode nd0)
		{
			if (nd0 != null)
			{
				TreeNode nd;
				//find child first
				nd0.Expand();
				nd = nd0.FirstNode;
				if (nd != null)
					return nd;
				//find sibling 
				nd = nd0.NextNode;
				if (nd != null)
					return nd;
				//find parent's sibling
				while (nd0 != null)
				{
					nd0 = nd0.Parent;
					if (nd0 != null)
					{
						nd = nd0.NextNode;
						if (nd != null)
							return nd;
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		static public TreeNode FindInvisibleNodeUp(TreeNode nd0)
		{
			if (nd0.PrevNode != null)
				return nd0.PrevNode;
			return nd0.Parent;
		}
		#endregion

		#region IDatabaseConnectionUserExt0 Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (_dataQueries != null)
			{
				for (int i = 0; i < _dataQueries.Length; i++)
				{
					if (_dataQueries[i] != null)
					{
						string s = _dataQueries[i].Report32Usage();
						if (!string.IsNullOrEmpty(s))
						{
							return s;
						}
					}
				}
			}
			return string.Empty;
		}
		[NotForProgramming]
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				if (_dataQueries != null && _dataQueries.Length > 0)
				{
					List<Type> l = new List<Type>();
					for (int i = 0; i < _dataQueries.Length; i++)
					{
						IList<Type> ls = _dataQueries[i].DatabaseConnectionTypesUsed;
						if (ls != null && ls.Count > 0)
						{
							l.AddRange(ls);
						}
					}
					return l;
				}
				return null;
			}
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[NotForProgramming]
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				if (_dataQueries != null && _dataQueries.Length > 0)
				{
					List<Guid> l = new List<Guid>();
					for (int i = 0; i < _dataQueries.Length; i++)
					{
						IList<Guid> ls = _dataQueries[i].DatabaseConnectionsUsed;
						if (ls != null && ls.Count > 0)
						{
							l.AddRange(ls);
						}
					}
					return l;
				}
				return null;
			}
		}

		#endregion

		#region IDevClassReferencer Members
		private IDevClass _class;
		[NotForProgramming]
		[Browsable(false)]
		public void SetDevClass(IDevClass c)
		{
			_class = c;
			if (_dataQueries != null)
			{
				for (int i = 0; i < _dataQueries.Length; i++)
				{
					_dataQueries[i].SetOwner(this);
					_dataQueries[i].SetDevClass(_class);
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public IDevClass GetDevClass()
		{
			return _class;
		}

		#endregion

		#region IPropertyValueLinkOwner Members

		public Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> GetPropertyValueLinks()
		{
			Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> ret = new Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>>();
			if (_dataQueries != null)
			{
				Dictionary<IPropertyValueLink, CodeExpression> kv;
				for (int i = 0; i < _dataQueries.Length; i++)
				{
					if (_dataQueries[i] != null)
					{
						QueryParameterList qps = _dataQueries[i].QueryParameters;
						string[] names = qps.GetLinkablePropertyNames();
						if (names != null && names.Length > 0)
						{
							kv = new Dictionary<IPropertyValueLink, CodeExpression>();
							for (int k = 0; k < names.Length; k++)
							{
								IPropertyValueLink pvk = qps.GetPropertyLink(names[k]);
								if (pvk != null && pvk.IsValueLinkSet())
								{
									//this.GetDataQuery(int level).QueryParameters to set link to names[k]
									CodePropertyReferenceExpression pre = new CodePropertyReferenceExpression();
									pre.UserData.Add("name", names[k]);
									pre.PropertyName = "QueryParameters";
									//
									CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression();
									mie.Method.MethodName = "GetDataQuery";
									mie.Parameters.Add(new CodePrimitiveExpression(i));
									pre.TargetObject = mie;
									//
									kv.Add(pvk, pre);

								}
							}
							if (kv.Count > 0)
							{
								ret.Add(qps, kv);
							}
						}
					}
				}
			}
			if (_templates != null)
			{
				_templates.CollectPropertyLinks(ret);
			}
			return ret;
		}
		#endregion
	}
}
