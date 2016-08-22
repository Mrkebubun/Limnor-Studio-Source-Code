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
using VPL;
using System.ComponentModel;
using System.Xml;
using XmlUtility;
using System.Xml.Serialization;

namespace Limnor.TreeViewExt
{
	/// <summary>
	/// represents a value
	/// </summary>
	public class TreeNodeValue : TreeNode, INodeX
	{
		#region fields and constructors
		private TypedNamedValue _data;
		private XmlNode _xmlNode;
		private Guid _guid = Guid.Empty;
		public TreeNodeValue(string name, TypedValue value)
		{
			_data = new TypedNamedValue(name, value);
			init();
		}
		public TreeNodeValue(TypedNamedValue value)
		{
			_data = value;
			init();
		}
		public TreeNodeValue(XmlNode node, ObjectXmlReader reader)
		{
			_xmlNode = node;
			string name = XmlUtil.GetNameAttribute(_xmlNode);
			Type type = XmlUtil.GetLibTypeAttribute(_xmlNode);
			object d = reader.ReadValue(_xmlNode, null, type);
			_data = new TypedNamedValue(name, new TypedValue(type, d));
			init();
		}
		private void init()
		{
			_data.BeforeNameChange += new EventHandler(_data_BeforeNameChange);
			_data.AfterNameChange += new EventHandler(_data_AfterNameChange);
			_data.ValueChanged += new EventHandler(_data_ValueChanged);
			OnDataNameChanged();
		}

		void _data_ValueChanged(object sender, EventArgs e)
		{
			showText();
		}
		#endregion

		#region private methods
		void showText()
		{
			if (_data == null)
			{
				Text = Name;
			}
			else
			{
				Text = _data.ToString();
			}
		}
		void _data_AfterNameChange(object sender, EventArgs e)
		{
			string oldName = Name;

			Name = _data.Name;
			showText();
			if (_xmlNode != null)
			{
				XmlUtil.SetNameAttribute(_xmlNode, _data.Name);
			}
			TreeViewX tvx = TreeView as TreeViewX;
			if (tvx != null)
			{
				TreeNodeX tnx = this.Parent as TreeNodeX;
				if (tnx != null)
				{
					tvx.OnValueNameChanged(tnx.TreeNodeId, oldName, _data.Name);
				}
			}
		}

		void _data_BeforeNameChange(object sender, EventArgs e)
		{
			NameChangeEventArgs ce = e as NameChangeEventArgs;
			if (ce != null)
			{
				TreeNodeX tnx = this.Parent as TreeNodeX;
				if (tnx != null)
				{
					if (tnx.IsNameUsed(ce.Name))
					{
						ce.Cancel = true;
					}
				}
			}
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public void OnDataNameChanged()
		{
			Name = _data.Name;
			showText();
		}
		[Browsable(false)]
		public WriteResult WriteToXmlNode(XmlNode parentXmlNode, IXmlCodeWriter writer)
		{
			_xmlNode = parentXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']",
				TreeViewX.XML_Value, TreeViewX.XMLATT_Name, _data.Name));
			if (_xmlNode == null)
			{
				_xmlNode = parentXmlNode.OwnerDocument.CreateElement(TreeViewX.XML_Value);
				parentXmlNode.AppendChild(_xmlNode);
				XmlUtil.SetAttribute(_xmlNode, TreeViewX.XMLATT_Name, _data.Name);
			}

			XmlUtil.SetLibTypeAttribute(_xmlNode, _data.Value.ValueType);
			if (_data.Value.Value == null || _data.Value.Value == DBNull.Value)
			{
				return WriteResult.WriteOK;
			}
			else
			{
				return writer.WriteValue(_xmlNode, _data.Value.Value, null);
			}
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmlNode;
			}
		}
		[Browsable(false)]
		public TypedValue Value
		{
			get
			{
				return _data.Value;
			}
		}
		[Browsable(false)]
		public TypedNamedValue Data
		{
			get
			{
				if (_data != null)
				{
					if (IsShortcut)
					{
						_data.ReadOnly = true;
					}
				}
				return _data;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsShortcut
		{
			get;
			set;
		}
		[Description("Gets the name of the data")]
		public string DataName
		{
			get
			{
				return _data.Name;
			}
		}
		[Description("Gets the value of the data")]
		public object DataValue
		{
			get
			{
				return _data.Value.Value;
			}
		}
		[Description("Gets the type of the data")]
		public Type DataType
		{
			get
			{
				return _data.Value.ValueType;
			}
		}
		#endregion

		#region INodeX Members
		[Browsable(false)]
		public TreeNode CreatePointer()
		{
			TreeNodeValue tnv = new TreeNodeValue(_data);
			tnv.IsShortcut = IsShortcut;
			return tnv;
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
