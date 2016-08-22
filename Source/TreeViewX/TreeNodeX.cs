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
using System.ComponentModel;
using VPL;
using System.Reflection;
using System.Xml;
using XmlUtility;
using System.Xml.Serialization;
using System.Drawing.Design;
using LimnorDatabase;
using System.Drawing;

namespace Limnor.TreeViewExt
{
	public class TreeNodeX : TreeNode, ICustomTypeDescriptor, IXmlNodeHolder, IBeforeXmlSerialize, INodeX
	{
		#region fields and constructors
		private Guid _guid = Guid.Empty;
		private Dictionary<string, TypedNamedValue> _data;
		private bool _nextLevelLoaded;
		private XmlNode _xmlNode;
		private TreeNodeXTemplate _template;
		public TreeNodeX()
		{
			init();
		}
		public TreeNodeX(string text)
		{
			Text = text;
			init();
		}
		public TreeNodeX(XmlNode node)
		{
			_xmlNode = node;
			init();
		}
		public TreeNodeX(TreeNodeXTemplate template, FieldList fields)
		{
			_template = template;
			_fields = fields;
			IsLoadedByDataBinding = true;
			init();
		}
		private void init()
		{
			ShowPropertyNodes = true;
			Nodes.Add(new Loader());
		}
		#endregion

		#region protected Methods
		protected virtual void GenerateXmlNode(XmlNode parentXmlNode)
		{
			_xmlNode = parentXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				TreeViewX.XML_Item, TreeViewX.XMLATT_Guid, this.TreeNodeId.ToString("D")));
			if (_xmlNode == null)
			{
				_xmlNode = parentXmlNode.OwnerDocument.CreateElement(TreeViewX.XML_Item);
				parentXmlNode.AppendChild(_xmlNode);
				XmlUtil.SetAttribute(_xmlNode, TreeViewX.XMLATT_Guid, this.TreeNodeId.ToString("D"));
			}
		}
		protected virtual TreeViewX GetOwnerTreeView()
		{
			return this.TreeView as TreeViewX;
		}
		#endregion

		#region Methods
		[Browsable(false)]
		[NotForProgramming]
		public void ApplyTemplate()
		{
			if (_template == null) return;
			if (this.TreeView != null)
			{
				try
				{
					Graphics g = this.TreeView.CreateGraphics();
					if (_template.NodeFont == null)
					{
						_template.NodeFont = this.TreeView.Font;
					}
					SizeF size = g.MeasureString("W", _template.NodeFont);
					int h = (int)size.Height + 2;
					if (this.TreeView.ItemHeight < h)
					{
						this.TreeView.ItemHeight = h;
					}
				}
				catch
				{
				}
			}
			if (_template.NodeFont != null)
			{
				this.NodeFont = _template.NodeFont.Clone() as Font;
			}
			if (_template.BackColor != Color.Empty)
			{
				this.BackColor = _template.BackColor;
			}
			if (_template.ForeColor != Color.Empty)
			{
				this.ForeColor = _template.ForeColor;
			}
			this.Text = _template.Text;
			this.Text += string.Empty;
			this.ImageIndex = _template.ImageIndex;
			this.SelectedImageIndex = _template.SelectedImageIndex;
			this.ImageKey = _template.ImageKey;
			this.SelectedImageKey = _template.SelectedImageKey;
			this.ToolTipText = _template.ToolTipText;
		}
		[Description("Remove all shortcuts specified by id")]
		public void RemoveAllShortcuts(Guid id)
		{
			if (NextLevelLoaded)
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
		}
		/// <summary>
		/// clone a node for the targetHolder
		/// </summary>
		/// <param name="targetHolder"></param>
		/// <returns></returns>
		[Browsable(false)]
		public virtual TreeNodeX CreateDuplicatedNode(TreeViewX targetHolder)
		{
			if (_xmlNode == null)
			{
				TreeNodeX tnx = (TreeNodeX)this.Clone();
				return tnx;
			}
			else
			{
				XmlNode dataXml;
				XmlDocument docTarget = targetHolder.GetXmlDocument();
				if (docTarget == _xmlNode.OwnerDocument)
				{
					dataXml = _xmlNode.CloneNode(true);
				}
				else
				{
					dataXml = docTarget.ImportNode(_xmlNode, true);
				}
				XmlUtil.SetLibTypeAttribute(dataXml, this.GetType());
				XmlNodeList ndLst = dataXml.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"..//*[@{0}]", TreeViewX.XMLATT_Guid));
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
						Guid id2;
						if (!idMaps.TryGetValue(id, out id2))
						{
							id2 = Guid.NewGuid();
							idMaps.Add(id, id2);
						}
					}
					XmlUtil.SetAttribute(nd, TreeViewX.XMLATT_Guid, Guid.NewGuid().ToString("D"));
				}
				TreeNodeX tnx = new TreeNodeX(dataXml);
				VPLUtil.CopyProperties(this, tnx);
				return tnx;
			}
		}
		[Browsable(false)]
		public void DeleteValue(string name)
		{
			if (_data != null)
			{
				if (_data.ContainsKey(name))
				{
					_data.Remove(name);
				}
			}
			if (_xmlNode != null)
			{
				XmlNode xn = _xmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']",
					TreeViewX.XML_Value, TreeViewX.XMLATT_Name, name));
				if (xn != null)
				{
					_xmlNode.RemoveChild(xn);
				}
			}
		}
		[Browsable(false)]
		public void CreateID()
		{
			if (_guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
		}
		[Browsable(false)]
		public void ResetNextLevel()
		{
			if (_nextLevelLoaded)
			{
				Nodes.Clear();
				Nodes.Add(new Loader());
				_nextLevelLoaded = false;
				this.Collapse();
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public void PrepareCurrentNode()
		{
			if (_fields != null)
			{
				TreeViewX tv = this.TreeView as TreeViewX;
				if (tv != null)
				{
					int level = this.Level + 1; //for query loading the next level
					DataQuery dq = tv.GetDataQuery(level);
					if (dq != null)
					{
						FieldList fs = dq.Fields;
						if (fs != null)
						{
							for (int i = 0; i < _fields.Count; i++)
							{
								EPField f = fs[_fields[i].Name];
								if (f != null)
								{
									f.Value = _fields[i].Value;
								}
							}
						}
					}
				}
				if (_template != null)
				{
					FieldList fs = _template.Fields;
					if (fs != null)
					{
						for (int i = 0; i < _fields.Count; i++)
						{
							EPField f = fs[_fields[i].Name];
							if (f != null)
							{
								f.Value = _fields[i].Value;
							}
						}
					}
				}
				TreeNodeX tx = this.Parent as TreeNodeX;
				if (tx != null)
				{
					tx.PrepareCurrentNode();
				}
			}
		}
		/// <summary>
		/// called by the loader
		/// </summary>
		[Browsable(false)]
		public virtual void LoadNextLevelNodes()
		{
			if (_xmlNode != null)
			{
				TreeViewX tv = TreeView as TreeViewX;
				if (tv != null)
				{
					string err = tv.LoadNextLevelNodes(Nodes, _xmlNode, this);
					if (!string.IsNullOrEmpty(err))
					{
						throw new TreeViewXException("Error loading next level. {0}", err);
					}
				}
				else
				{
					throw new TreeViewXException("Calling LoadNextLevelNodes when the node is not added to a TreeViewX. Node name:{0}", Name);
				}
			}
			if (this.IsLoadedByDataBinding)
			{
				TreeViewX tv = TreeView as TreeViewX;
				if (tv != null)
				{
					tv.LoadNodesByQuery(this);
				}
			}
		}
		/// <summary>
		/// called OnBeforeExpand
		/// </summary>
		[Browsable(false)]
		public void LoadNextLevel()
		{
			if (!_nextLevelLoaded)
			{
				_nextLevelLoaded = true;
				List<Loader> l = new List<Loader>();
				foreach (TreeNode tn in Nodes)
				{
					Loader ld = tn as Loader;
					if (ld != null)
					{
						l.Add(ld);
					}
				}
				foreach (Loader ld in l)
				{
					ld.Remove();
				}
				foreach (Loader ld in l)
				{
					ld.LoadNextLevel(this);
				}
			}
		}
		[Browsable(false)]
		public void OnValueNameChanged(Guid categoryId, string oldName, string valueName)
		{
			if (this.NextLevelLoaded)
			{
				if (this.TreeNodeId == categoryId)
				{
					foreach (TreeNode tn in Nodes)
					{
						TreeNodeValue tnv = tn as TreeNodeValue;
						if (tnv != null)
						{
							if (string.CompareOrdinal(valueName, tnv.DataName) == 0)
							{
								if (_data != null)
								{
									if (_data.ContainsKey(oldName))
									{
										TypedNamedValue v = _data[oldName];
										_data.Remove(oldName);
										_data.Add(valueName, v);
									}
								}
								tnv.OnDataNameChanged();
								break;
							}
						}
					}
				}
				else
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
			}
		}
		[Browsable(false)]
		public virtual void OnSubNodeCreated(TreeNodeX parentNode, TreeNodeX subNode)
		{
			if (NextLevelLoaded)
			{
				if (this != parentNode)
				{
					if (this.TreeNodeId == parentNode.TreeNodeId)
					{
						TreeNode tn = subNode.CreatePointer();
						this.Nodes.Add(tn);
					}
					else
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
				}
			}
		}
		[Browsable(false)]
		public void OnRemoveValueNode(TreeNodeX node, string valueName)
		{
			if (this.TreeNodeId == node.TreeNodeId)
			{
				if (NextLevelLoaded)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeValue tnv = Nodes[i] as TreeNodeValue;
						if (tnv != null)
						{
							if (string.CompareOrdinal(valueName, tnv.DataName) == 0)
							{
								tnv.Remove();
								break;
							}
						}
					}
				}
			}
			else
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
		}
		[Browsable(false)]
		public virtual bool SetReferencedNode(TreeNodePointer node)
		{
			if (this.TreeNodeId == node.TreeNodeId)
			{
				node.SetReferenceNode(this);
				return true;
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
				return false;
			}
		}
		[Browsable(false)]
		public virtual void SyncShortcuts(TreeNodeX node)
		{
			if (this != node)
			{
				if (node.TreeNodeId == this.TreeNodeId)
				{
					VPLUtil.CopyProperties(node, this);
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
								tnx.SyncShortcuts(node);
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public virtual void SyncShortcutsValueList(TreeNodeX node)
		{
			if (this != node)
			{
				if (this.TreeNodeId == node.TreeNodeId)
				{
					this.SetValueList(node.GetValueList());
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
								tnx.SyncShortcutsValueList(node);
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public virtual WriteResult WriteToXmlNode(XmlNode parentXmlNode, IXmlCodeWriter writer)
		{
			//create XmlNode
			GenerateXmlNode(parentXmlNode);
			//write properties
			WriteResult ret = writer.WriteObjectToNode(_xmlNode, this);
			//write sub-nodes,shortcuts and values
			if (this.NextLevelLoaded)
			{
				foreach (TreeNode tn in Nodes)
				{
					TreeNodeX tnx = tn as TreeNodeX;
					if (tnx != null)
					{
						if (tnx.WriteToXmlNode(_xmlNode, writer) == WriteResult.WriteFail)
						{
							ret = WriteResult.WriteFail;
						}
					}
					else
					{
						TreeNodeValue tnv = tn as TreeNodeValue;
						if (tnv != null)
						{
							if (tnv.WriteToXmlNode(_xmlNode, writer) == WriteResult.WriteFail)
							{
								ret = WriteResult.WriteFail;
							}
						}
					}
				}
			}
			return ret;
		}
		[Browsable(false)]
		public TreeNodeShortcut CreateShortcut()
		{
			TreeNodeShortcut tns = new TreeNodeShortcut(this.TreeNodeId);
			VPLUtil.CopyProperties(this, tns);
			tns.SetValueList(_data);
			tns.SetReferenceNode(this);
			tns.DataXmlNode = DataXmlNode;
			return tns;
		}
		[Description("Gets category node by guid")]
		public TreeNodeX GetCategoryNodeById(Guid id)
		{
			if (!IsShortcut)
			{
				if (id == this.TreeNodeId)
				{
					return this;
				}
				if (NextLevelLoaded)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeX tnx = Nodes[i] as TreeNodeX;
						if (tnx != null && !tnx.IsShortcut)
						{
							TreeNodeX ret = tnx.GetCategoryNodeById(id);
							if (ret != null)
							{
								return ret;
							}
						}
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		public virtual void OnValueListLoaded(TreeNodeX node)
		{
			if (this != node)
			{
				if (NextLevelLoaded)
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
			}
		}
		[Browsable(false)]
		public virtual void OnCategoryNodeLoaded(TreeNodeX node)
		{
			if (this != node)
			{
				if (NextLevelLoaded)
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
			}
		}
		[Browsable(false)]
		public void OnRemoveCategoryNode(TreeNodeX node)
		{
			if (NextLevelLoaded)
			{
				List<TreeNodePointer> list = new List<TreeNodePointer>();
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeX tnx = Nodes[i] as TreeNodeX;
					if (tnx != null)
					{
						TreeNodePointer tnp = tnx as TreeNodePointer;
						if (tnp != null && tnp.TreeNodeId == node.TreeNodeId)
						{
							list.Add(tnp);
						}
						else
						{
							tnx.OnRemoveCategoryNode(node);
						}
					}
				}
				foreach (TreeNodePointer p in list)
				{
					p.Remove();
				}
			}
		}
		#endregion

		#region Values
		[Description("Gets associated value by value name")]
		public object GetValue(string name)
		{
			if (_data != null)
			{
				TypedNamedValue v;
				if (_data.TryGetValue(name, out v))
				{
					return v.Value.Value;
				}
			}
			return null;
		}
		[Browsable(false)]
		public void ResetValue(string name)
		{
			if (_data != null)
			{
				if (_data.ContainsKey(name))
				{
					_data[name].Value.ResetValue();
				}
			}
		}
		[Description("Associate a value to this node. Use a name to identify the value.")]
		public void SetValue(string name, object value)
		{
			if (_data != null)
			{
				if (_data.ContainsKey(name))
				{
					_data[name].Value.Value = value;
				}
			}
		}
		[Description("Remove all associated values")]
		public void ClearValues()
		{
			_data = null;
		}
		[Browsable(false)]
		public void SetValueList(Dictionary<string, TypedNamedValue> list)
		{
			_data = list;
		}
		[Browsable(false)]
		public Dictionary<string, TypedNamedValue> GetValueList()
		{
			return _data;
		}
		[Browsable(false)]
		public bool IsNameUsed(string name)
		{
			if (_data != null)
			{
				if (_data.ContainsKey(name))
				{
					return true;
				}
			}
			PropertyInfo[] pifs = typeof(TreeNodeX).GetProperties();
			for (int i = 0; i < pifs.Length; i++)
			{
				if (string.CompareOrdinal(pifs[i].Name, name) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public string CreateNewValueName()
		{
			int n = 1;
			string name = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"value{0}", n);
			if (_data != null)
			{
				while (true)
				{
					if (_data.ContainsKey(name))
					{
						n++;
						name = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"value{0}", n);
					}
					else
					{
						break;
					}
				}
			}
			PropertyInfo[] pifs = typeof(TreeNodeX).GetProperties();
			while (true)
			{
				bool b = false;
				for (int i = 0; i < pifs.Length; i++)
				{
					if (string.CompareOrdinal(pifs[i].Name, name) == 0)
					{
						b = true;
						break;
					}
				}
				if (b)
				{
					n++;
					name = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"value{0}", n);
				}
				else
				{
					break;
				}
			}
			return name;
		}
		[Description("Create a Value-Node by specifying the type for the value")]
		public TreeNodeValue CreateValue(Type t)
		{
			this.Expand();
			string name = CreateNewValueName();
			if (_data == null)
			{
				_data = new Dictionary<string, TypedNamedValue>();
			}
			TypedValue v = new TypedValue(t, VPLUtil.GetDefaultValue(t));
			_data.Add(name, new TypedNamedValue(name, v));
			TreeNodeValue tnv = new TreeNodeValue(name, v);
			Nodes.Add(tnv);
			return tnv;
		}
		[Description("Associate a value to this node. The value is identified by name. Value type is specified by type.")]
		public void AddValue(string name, Type type, object value)
		{
			if (_data == null)
			{
				_data = new Dictionary<string, TypedNamedValue>();
			}
			TypedValue v = new TypedValue(type, value);
			if (_data.ContainsKey(name))
			{
				_data[name] = new TypedNamedValue(name, v);
			}
			else
			{
				_data.Add(name, new TypedNamedValue(name, v));
			}
		}
		[Browsable(false)]
		public void AddValue(string name, TypedValue value)
		{
			if (_data == null)
			{
				_data = new Dictionary<string, TypedNamedValue>();
			}
			if (_data.ContainsKey(name))
			{
				_data[name] = new TypedNamedValue(name, value);
			}
			else
			{
				_data.Add(name, new TypedNamedValue(name, value));
			}
		}
		private void checkLoadValues()
		{
			if (_data == null && _xmlNode != null)
			{
				XmlNodeList nds = _xmlNode.SelectNodes(TreeViewX.XML_Value);
				ObjectXmlReader xr = new ObjectXmlReader();
				foreach (XmlNode nd in nds)
				{
					TreeNodeValue tnv = new TreeNodeValue(nd, xr);
					tnv.IsShortcut = IsShortcut;
					if (!tnv.IsShortcut)
					{
						AddValue(tnv.Name, tnv.Value);
					}
				}
				if (!IsShortcut)
				{
					TreeViewX tvx = TreeView as TreeViewX;
					if (tvx != null)
					{
						tvx.OnValueListLoaded(this);
					}
				}
			}
		}
		#endregion

		#region Properties
		private FieldList _fields;
		[Category("Database")]
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(HideUITypeEditor), typeof(UITypeEditor))]
		[Description("Fields in this node if this node is loaded via data-binding.")]
		public FieldList Fields
		{
			get
			{
				if (_fields == null)
				{
					TreeViewX tvx = GetOwnerTreeView();
					if (tvx != null)
					{
						DataQuery dq = tvx.GetDataQuery(this.Level);
						if (dq != null)
						{
							return dq.Fields;
						}
					}
				}
				return _fields;
			}
			set
			{
				_fields = value;
			}
		}
		public int FieldCount
		{
			get
			{
				FieldList fs = Fields;
				if (fs != null)
				{
					return fs.Count;
				}
				return 0;
			}
		}
		[Description("Gets a Boolean indicating whether this node is loaded by data-binding")]
		public bool IsLoadedByDataBinding { get; private set; }
		[Description("Gets a Guid identifying this node")]
		public Guid TreeNodeGuid
		{
			get
			{
				return TreeNodeId;
			}
		}
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets a list of values associated with this node")]
		public List<TypedNamedValue> ValueList
		{
			get
			{
				checkLoadValues();
				List<TypedNamedValue> list = new List<TypedNamedValue>();
				if (_data != null)
				{
					foreach (KeyValuePair<string, TypedNamedValue> kv in _data)
					{
						list.Add(kv.Value);
					}
				}
				return list;
			}
		}
		[DefaultValue(true)]
		[Description("Gets and sets a value indicating whether the property nodes should be displayed")]
		public bool ShowPropertyNodes
		{
			get;
			set;
		}
		[Description("Gets a Boolean value indicates whether its child nodes have been loaded")]
		public bool NextLevelLoaded
		{
			get
			{
				return _nextLevelLoaded;
			}
		}
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Localizable(true)]
		[MergableProperty(false)]
		[Description("Gets the child nodes in this node.")]
		[Category("Behavior")]
		[NotForLightRead]
		public new TreeNodeCollection Nodes
		{
			get
			{
				return base.Nodes;
			}
		}
		[Description("Gets a Boolean value indicating whether this node is generated from a shortcut.")]
		public bool IsByShortcut
		{
			get
			{
				return IsShortcut;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsShortcut
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		/// <summary>
		/// when a value is created, its name should not be a property name
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			checkLoadValues();

			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (IsShortcut && TreeView != null)
			{
				List<PropertyDescriptor> l = new List<PropertyDescriptor>();
				foreach (PropertyDescriptor p in ps)
				{
					object v = p.GetValue(this);
					string s = string.Empty;
					if (v != null && v != DBNull.Value)
					{
						s = v.ToString();
					}
					PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(p.ComponentType, p.Name, s, attributes);
					l.Add(p0);
				}
				if (_data != null)
				{
					foreach (KeyValuePair<string, TypedNamedValue> kv in _data)
					{
						object v = kv.Value.Value.Value;
						string s = string.Empty;
						if (v != null && v != DBNull.Value)
						{
							s = v.ToString();
						}
						PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), kv.Key, s, attributes);
						l.Add(p0);
					}
				}
				ps = new PropertyDescriptorCollection(l.ToArray());
			}
			else
			{
				//values as properties
				if (_data != null)
				{
					List<PropertyDescriptor> l = new List<PropertyDescriptor>();
					foreach (PropertyDescriptor p in ps)
					{
						l.Add(p);
					}
					foreach (KeyValuePair<string, TypedNamedValue> kv in _data)
					{
						l.Add(new PropertyDescriptorValue(kv.Value.Value.ValueType, kv.Key, attributes));
					}
					ps = new PropertyDescriptorCollection(l.ToArray());
				}
			}
			return ps;
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmlNode;
			}
			set
			{
				if (_xmlNode == null)
				{
					_xmlNode = value;
				}
			}
		}

		#endregion

		#region IBeforeXmlSerialize Members
		[Browsable(false)]
		public void OnBeforeXmlSerialize(XmlNode node, IXmlCodeWriter writer)
		{
			XmlUtil.SetAttribute(node, TreeViewX.XMLATT_Guid, TreeNodeId.ToString("D"));
		}

		#endregion

		#region class Loader
		class Loader : TreeNode
		{
			public Loader()
			{
				Text = "Loader";
			}
			public virtual void LoadNextLevel(TreeNodeX parentNode)
			{
				parentNode.LoadNextLevelNodes();

			}
		}
		#endregion

		#region class PropertyDescriptorValue
		class PropertyDescriptorValue : PropertyDescriptor
		{
			private Type _dataType;
			public PropertyDescriptorValue(Type type, string name, Attribute[] attrs)
				: base(name, attrs)
			{
				_dataType = type;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(TreeNodeX); }
			}

			public override object GetValue(object component)
			{
				TreeNodeX tnx = component as TreeNodeX;
				if (tnx != null)
				{
					return tnx.GetValue(this.Name);
				}
				return null;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _dataType; }
			}

			public override void ResetValue(object component)
			{
				TreeNodeX tnx = component as TreeNodeX;
				if (tnx != null)
				{
					tnx.ResetValue(Name);
				}
			}

			public override void SetValue(object component, object value)
			{
				TreeNodeX tnx = component as TreeNodeX;
				if (tnx != null)
				{
					tnx.SetValue(Name, value);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion

		#region INodeX Members
		[Browsable(false)]
		public TreeNode CreatePointer()
		{
			TreeNodePointer obj = new TreeNodePointer(_xmlNode);
			if (this.IsShortcut)
			{
				bool loaded = false;
				TreeViewX tvx = this.TreeView as TreeViewX;
				if (tvx != null)
				{
					TreeNodeX tnx = tvx.GetCategoryNodeById(this.TreeNodeId);
					if (tnx != null)
					{
						VPLUtil.CopyProperties(tnx, obj);
						loaded = true;
					}
				}
				if (!loaded)
				{
					ObjectXmlReader oxr = new ObjectXmlReader();
					oxr.ReadProperties(_xmlNode, obj);
				}
			}
			else
			{
				VPLUtil.CopyProperties(this, obj);
			}
			return obj;
		}
		[Browsable(false)]
		[Description("Guid identifying this node")]
		public Guid TreeNodeId
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
	}
}
