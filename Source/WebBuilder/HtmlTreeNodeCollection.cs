/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class HtmlTreeNodeCollection : ICustomTypeDescriptor
	{
		private HtmlTreeView _tree;
		private HtmlTreeNode _parentNode;
		public HtmlTreeNodeCollection(HtmlTreeView tree, HtmlTreeNode parent)
		{
			_tree = tree;
			_parentNode = parent;
		}
		public override string ToString()
		{
			if (_parentNode == null)
			{
				return string.Format(CultureInfo.InvariantCulture, "Root nodes:{0}", _tree.Nodes.Count);
			}
			return string.Format(CultureInfo.InvariantCulture, "Level {0}, child nodes:{0}", _parentNode.Level, _parentNode.Nodes.Count);
		}
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			TreeNodeCollection nodes;
			if (_parentNode == null)
			{
				nodes = _tree.Nodes;
			}
			else
			{
				nodes = _parentNode.Nodes;
			}
			int i = 1;
			foreach (TreeNode tn in nodes)
			{
				HtmlTreeNode htn = tn as HtmlTreeNode;
				if (htn != null)
				{
					PropertyDescriptorHtmlTreeNode p = new PropertyDescriptorHtmlTreeNode(htn, i);
					lst.Add(p);
					i++;
				}
			}
			//
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region class PropertyDescriptorHtmlTreeNode
		class PropertyDescriptorHtmlTreeNode : PropertyDescriptor
		{
			private HtmlTreeNode _node;
			public PropertyDescriptorHtmlTreeNode(HtmlTreeNode node, int idx)
				: base(string.Format(CultureInfo.InvariantCulture, "Node {0} - {1}", idx, node.ToString()), new Attribute[] { })
			{
				_node = node;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(HtmlTreeNodeCollection); }
			}

			public override object GetValue(object component)
			{
				return _node;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(HtmlTreeNode); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
}
